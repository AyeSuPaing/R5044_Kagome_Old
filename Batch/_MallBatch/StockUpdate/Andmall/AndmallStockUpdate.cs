/*
=========================================================================================================
  Module      : ＆mall在庫連携クラス(AndmallStockUpdate.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using w2.App.Common.MallCooperation;
using w2.App.Common.Option;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.MallCooperationSetting;
using w2.SFTPClientWrapper;

namespace w2.Commerce.MallBatch.StockUpdate.Andmall
{
	/// <summary>
	/// ＆mall在庫連携クラス
	/// </summary>
	class AndmallStockUpdate
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="andmallSetting">＆mall連携設定</param>
		public AndmallStockUpdate(MallCooperationSettingModel andmallSetting)
		{
			this.AndmallSetting = andmallSetting;
			this.MallWatchingLogManager = new MallWatchingLogManager();
			var sftpSettings = new SFTPSettings
			{
				HostName = this.AndmallSetting.SftpHost,
				LoginUser = this.AndmallSetting.SftpUserName,
				KeyPassphrase = this.AndmallSetting.SftpPassPhrase,
				PortNumber = this.AndmallSetting.SftpPort,
				PrivateKeyFilePath = this.AndmallSetting.SftpPrivateKeyFilePath
			};
			this.SftpManager = new SFTPManager(sftpSettings);
			this.ChangeDateTime = DateTime.Now;
			this.StockFileNameNotExtension = string.Format("stock{0}_{1}_{2}-{3}",
				this.AndmallSetting.AndmallTenantCode,
				this.AndmallSetting.AndmallBaseStoreCode,
				this.ChangeDateTime.ToString("yyyyMMdd"),
				this.ChangeDateTime.ToString("HHmmss"));
		}

		/// <summary>
		/// 在庫連携データの連携実行
		/// </summary>
		public void Exec()
		{
			//規定日数以前の連携ファイル群を削除
			RemoveStockFiles();

			// ＆mall SFTPサーバに未処理の在庫連携データが存在する場合は発行処理をスキップ
			if (IsExistServerCompleteFile())
			{
				this.MallWatchingLogManager.Insert(
					Constants.FLG_MALLWATCHINGLOG_BATCH_ID_STOCKUPDATE,
					this.AndmallSetting.MallId,
					Constants.FLG_MALLWATCHINGLOG_LOG_KBN_SUCCESS, "＆mall SFTPサーバに未処理の在庫連携データが存在するため処理を終了しました。");
				return;
			}

			// 在庫連携データのTempディレクトリを作成
			CreateTempDir();

			// 在庫連携データを作成
			var productCount = CreateStockCsv();
			if (productCount > 0)
			{
				// 在庫連携データ完了ファイルを作成
				CreateCompleteText();

				try
				{
					// 在庫連携データのアップロード
					PutServerStockFile();
				}
				catch (Exception ex)
				{
					this.MallWatchingLogManager.Insert(
						Constants.FLG_MALLWATCHINGLOG_BATCH_ID_STOCKUPDATE,
						this.AndmallSetting.MallId,
						Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR,
						"SFTP接続に失敗",
						"SFTP接続に失敗しました。");
					throw ex;
				}

				try
				{
					// 在庫連携データ完了ファイルをアップロード
					PutServerCompleteFile();
				}
				catch (Exception ex)
				{
					// 在庫連携データ完了ファイルをアップロードに失敗した場合は在庫連携データも合わせて削除
					// ただし、w2側にはファイルを残します。
					if (this.SftpManager.CreateSFTPClient().IsExistsServerFile(this.ServerStockFileFullPath))
					{
						this.SftpManager.CreateSFTPClient().RemoveServerFile(this.ServerStockFileFullPath);
						this.MallWatchingLogManager.Insert(
							Constants.FLG_MALLWATCHINGLOG_BATCH_ID_STOCKUPDATE,
							this.AndmallSetting.MallId,
							Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR,
							"在庫連携データ完了ファイルのアップロードに失敗",
							string.Format("整合性を保つため、在庫連携データ「{0}」を＆mallサーバより削除しました。", this.StockFileName));
					}
					throw ex;
				}

				// アップロード完了後、w2側の完了ファイルを削除
				RemoveCompleteText();

				this.MallWatchingLogManager.Insert(
					Constants.FLG_MALLWATCHINGLOG_BATCH_ID_STOCKUPDATE,
					this.AndmallSetting.MallId,
					Constants.FLG_MALLWATCHINGLOG_LOG_KBN_SUCCESS,
					string.Format("処理件数：{0}件", productCount.ToString()),
					string.Format("＆mall在庫連携データのアップロードに成功しました。 在庫連携データファイル名:{0} 在庫連携データ完了ファイル名:{1}",
						this.StockFileName,
						this.CompleteFileName));
			}
			else
			{
				this.MallWatchingLogManager.Insert(
					Constants.FLG_MALLWATCHINGLOG_BATCH_ID_STOCKUPDATE,
					this.AndmallSetting.MallId,
					Constants.FLG_MALLWATCHINGLOG_LOG_KBN_SUCCESS,
					"処理件数：0件",
					"在庫連携対象件数0件のため処理を終了しました。");
				return;
			}
		}

		/// <summary>
		/// 在庫連携データファイルの作成
		/// </summary>
		/// <returns>連携商品数</returns>
		private int CreateStockCsv()
		{
			var productCount = 0;
			var notFoundSkuList = new List<string>();

			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				using (var statement = new SqlStatement("Andmall", "GetStockList"))
				{
					// 商品設定を指定
					statement.ReplaceStatement("## EXHIBITS_FLG ##", (string.IsNullOrEmpty(this.AndmallSetting.MallExhibitsConfig) == false)
						? string.Format(" AND {0}.{1} = {2} ",
							Constants.TABLE_MALLEXHIBITSCONFIG,
							MallOptionUtility.GetExhibitsConfigField(this.AndmallSetting.MallExhibitsConfig),
							Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON)
						: string.Empty);
					// 商品IDの場合の連携カラム名を指定
					statement.ReplaceStatement("## SKU_PRODUCT_ID ##", string.Format(" {0}.{1} ", Constants.TABLE_PRODUCT, this.AndmallSetting.AndmallCooperation));
					// 商品バリエーションIDの場合のバリエーション連携カラム名を指定
					statement.ReplaceStatement("## SKU_VARIATION_ID ##", string.Format(" {0}.{1} ", Constants.TABLE_PRODUCTVARIATION, this.AndmallSetting.AndmallVariationCooperation));
					using (var reader = new SqlStatementDataReader(accessor, statement))
					using (var sw = new StreamWriter(this.ClientStockFileFullPath, false, Encoding.GetEncoding("Shift_JIS")))
					{
						while (reader.Read())
						{
							var ht = new Hashtable();
							foreach (int i in Enumerable.Range(0, reader.FieldCount))
							{
								ht[reader.GetName(i)] = reader[i].ToString();
							}

							if (string.IsNullOrEmpty((string)ht["sku_id"]))
							{
								notFoundSkuList.Add((string)ht[Constants.FIELD_PRODUCT_PRODUCT_ID]);
								continue;
							}

							var dataColumns = new string[]
							{
								this.AndmallSetting.AndmallTenantCode,
								this.AndmallSetting.AndmallBaseStoreCode,
								this.AndmallSetting.AndmallShopNo,
								(string)ht["sku_id"],
								(string)ht["stock"],
								(string)ht["reservation_flg"],
								this.ChangeDateTime.ToString("yyyyMMddHHmm"),
								"1"
							};
							var line = StringUtility.CreateEscapedCsvString(dataColumns);
							sw.WriteLine(line);
							productCount++;
						}
					}
				}
			}

			// もしSKUが設定されていないものが存在する場合は警告
			if (notFoundSkuList.Count > 0)
			{
				this.MallWatchingLogManager.Insert(
					Constants.FLG_MALLWATCHINGLOG_BATCH_ID_STOCKUPDATE,
					this.AndmallSetting.MallId,
					Constants.FLG_MALLWATCHINGLOG_LOG_KBN_WARNING,
					"商品連携IDが設定されていません。",
					string.Format("商品ID{1}:", string.Join(",", notFoundSkuList.ToArray())));
			}

			// 件数が0件の場合は発行ファイルを削除
			if ((productCount == 0) && File.Exists(this.ClientStockFileFullPath))
			{
				File.Delete(this.ClientStockFileFullPath);
			}

			return productCount;
		}

		/// <summary>
		/// 在庫連携データ完了ファイルの作成
		/// </summary>
		private void CreateCompleteText()
		{
			new StreamWriter(this.ClientCompleteFileFullPath).Close();
		}

		/// <summary>
		/// 在庫連携データ完了ファイルの削除
		/// </summary>
		private void RemoveCompleteText()
		{
			if (File.Exists(this.ClientCompleteFileFullPath))
			{
				File.Delete(this.ClientCompleteFileFullPath);
			}
		}

		/// <summary>
		/// 規定日数以前の在庫連携データファイルの削除
		/// </summary>
		private void RemoveStockFiles()
		{
			if (Directory.Exists(Constants.ANDMALL_CLIENT_TEMP_DIR_PATH) == false) return;

			var files = new DirectoryInfo(Constants.ANDMALL_CLIENT_TEMP_DIR_PATH).EnumerateFiles("*.csv",SearchOption.TopDirectoryOnly);
			var deleteFiles = files.Where(f => f.CreationTime < DateTime.Now.AddDays(-1*Constants.ANDMALL_CLIENT_TEMP_FILE_BACKUP_DAY));
			foreach(var deleteFile in deleteFiles)
			{
				if(File.Exists(deleteFile.FullName))
				{
					File.Delete(deleteFile.FullName);
				}
			}
		}

		/// <summary>
		/// w2側 在庫データ保持ディレクトリの作成
		/// </summary>
		private void CreateTempDir()
		{
			if (Directory.Exists(Constants.ANDMALL_CLIENT_TEMP_DIR_PATH) == false)
			{
				Directory.CreateDirectory(Constants.ANDMALL_CLIENT_TEMP_DIR_PATH);
			}
		}

		/// <summary>
		/// ＆mall側 未処理在庫連携データの有無をチェック
		/// </summary>
		/// <returns>未処理データがある:True ない:False</returns>
		private bool IsExistServerCompleteFile()
		{
			var listFileName = this.SftpManager.CreateSFTPClient().ListFileName(Path.Combine("./", Constants.ANDMALL_SERVER_DIR_PATH));
			var regex = new Regex(string.Format("stock{0}_{1}_.*.txt", this.AndmallSetting.AndmallTenantCode, this.AndmallSetting.AndmallBaseStoreCode));
			var result = listFileName.Any(f => regex.IsMatch(f));
			return result;
		}

		/// <summary>
		/// 在庫連携データファイルのアップロード
		/// </summary>
		private void PutServerStockFile()
		{
			this.SftpManager.CreateSFTPClient().PutFile(this.ServerStockFileFullPath, this.ClientStockFileFullPath);
		}

		/// <summary>
		/// 在庫連携データ完了ファイルのアップロード
		/// </summary>
		private void PutServerCompleteFile()
		{
			this.SftpManager.CreateSFTPClient().PutFile(this.ServerCompleteFileFullPath, this.ClientCompleteFileFullPath);
		}

		/// <summary>＆mall連携設定</summary>
		private MallCooperationSettingModel AndmallSetting { get; set; }
		/// <summary>モール監視ログマネージャ</summary>
		private MallWatchingLogManager MallWatchingLogManager { get; set; }
		/// <summary>SFTPマネージャ</summary>
		private SFTPManager SftpManager { get; set; }
		/// <summary>更新日時</summary>
		private DateTime ChangeDateTime { get; set; }
		/// <summary>在庫連携データファイル名 拡張子なし</summary>
		private string StockFileNameNotExtension { get; set; }
		/// <summary>在庫連携データファイル名 拡張子(.csv)</summary>
		private string StockFileName
		{
			get { return string.Format("{0}.csv", this.StockFileNameNotExtension); }
		}
		/// <summary>在庫連携データ完了ファイル名 拡張子(.txt)</summary>
		private string CompleteFileName
		{
			get { return string.Format("{0}.txt", this.StockFileNameNotExtension); }
		}
		/// <summary>w2側 在庫連携データファイル フルパス</summary>
		private string ClientStockFileFullPath
		{
			get { return Path.Combine(Constants.ANDMALL_CLIENT_TEMP_DIR_PATH, this.StockFileName); }
		}
		/// <summary>＆mall側 在庫連携データファイル フルパス</summary>
		private string ServerStockFileFullPath
		{
			get { return Path.Combine("./", Constants.ANDMALL_SERVER_DIR_PATH, this.StockFileName); }
		}
		/// <summary>w2側 在庫連携データ完了ファイル フルパス</summary>
		private string ClientCompleteFileFullPath
		{
			get { return Path.Combine(Constants.ANDMALL_CLIENT_TEMP_DIR_PATH, this.CompleteFileName); }
		}
		/// <summary>＆mall側 在庫連携データ完了ファイル フルパス</summary>
		private string ServerCompleteFileFullPath
		{
			get { return Path.Combine("./", Constants.ANDMALL_SERVER_DIR_PATH, this.CompleteFileName); }
		}
	}
}
