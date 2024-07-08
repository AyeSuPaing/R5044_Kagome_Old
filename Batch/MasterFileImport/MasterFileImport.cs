/*
=========================================================================================================
  Module      : マスタ取込バッチ処理(MasterFileImport.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Web;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using w2.Common;
using w2.Common.Logger;
using w2.Common.Util;
using w2.Common.Sql;
using w2.Domain.UpdateHistory;
using w2.App.Common.CrossPoint.Helper;
using w2.App.Common.CrossPoint.User;
using w2.App.Common;
using w2.App.Common.Option;
using w2.App.Common.Option.CrossPoint;
using w2.App.Common.User;
using w2.App.Common.TargetList;
using w2.Commerce.Batch.MasterFileImport.ImportSettings;
using w2.Domain;
using w2.Domain.User;
using w2.Domain.UpdateHistory.Helper;
using w2.Common.Web;

namespace w2.Commerce.Batch.MasterFileImport
{
	/// <summary>
	/// MasterFileImport の概要の説明です。
	/// </summary>
	/// <remarks>
	/// 取込可能CSV
	/// ・１行目がフィールド名とする。
	/// ・２行目以降がデータとする。
	/// ・１行目の１列目は「TBL」固定値とする。
	/// ・２行目以降各行の１列目は更新区分（IU、Dのどれか）とする。
	/// </remarks>
	class MasterFileImport
	{
		// 在庫連携フラグ
		const string CONST_STR_COMMAND_STOCK_COOPERATION = "-s";

		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			try
			{
				// インスタンス作成
				MasterFileImport objImport = new MasterFileImport();

				// バッチ起動をイベントログ出力
				AppLogger.WriteInfo("起動");

				//------------------------------------------------------
				// 取込処理
				//------------------------------------------------------
				if (args.Length == 0)
				{
					objImport.ImportAll();
				}
				else if (args.Length == 1)
				{
					objImport.ImportActiveFile(args[0]);	// 引数１：アップロード済みファイルパス
				}
				else if (args.Length == 2)
				{
					objImport.ImportActiveFile(args[0], args[1]);	// 引数２：在庫連携フラグあり
				}
				else if (args.Length == 5)
				{
					// 引数1:アップロード済みファイルパス 引数2:ShopId 引数3:DeptId 引数4:更新オペレータ名 引数5:アップロード済みファイル名
					objImport.ProcessingUploadTargetList(args[0], args[1], args[2], args[3], args[4]);
					objImport.ImportActiveFile(args[0]);
				}

				// バッチ終了をイベントログ出力
				AppLogger.WriteInfo("正常終了");
			}
			catch (Exception ex)
			{
				// エラーイベントログ出力
				AppLogger.WriteError(ex);
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MasterFileImport()
		{
			//------------------------------------------------------
			// 初期化
			//------------------------------------------------------
			Initialize();
		}

		/// <summary>
		/// 初期化
		/// </summary>
		private void Initialize()
		{
			try
			{
				//------------------------------------------------------
				// アプリケーション設定読み込み
				//------------------------------------------------------
				// アプリケーション名設定
				Constants.APPLICATION_NAME = Properties.Settings.Default.Application_Name;

				// アプリケーション共通の設定			
				ConfigurationSetting csSetting = new ConfigurationSetting(
					Properties.Settings.Default.ConfigFileDirPath,
					ConfigurationSetting.ReadKbn.C000_AppCommon,
					ConfigurationSetting.ReadKbn.C100_BatchCommon,
					ConfigurationSetting.ReadKbn.C200_MasterFileImport,
					ConfigurationSetting.ReadKbn.C200_CommonManager);

				//------------------------------------------------------
				// アプリケーション固有の設定
				//------------------------------------------------------
				// マスタアップロードセッティングファイルパス（EC用）
				Constants.PHYSICALDIRPATH_MASTERUPLOAD_SETTING_EC = csSetting.GetAppStringSetting("MasterFileImport_SettingFilePath");
				// マスタアップロードセッティングファイルパス（MP用）
				Constants.PHYSICALDIRPATH_MASTERUPLOAD_SETTING_MP = csSetting.GetAppStringSetting("MasterFileImport_SettingFilePath_MP");
				// マスタアップロードセッティングファイルパス（CMS用）
				Constants.PHYSICALDIRPATH_MASTERUPLOAD_SETTING_CMS = csSetting.GetAppStringSetting("MasterFileImport_SettingFilePath_CMS");
				// エラーログファイルURL
				Constants.MASTERUPLOAD_ERRORLOGFILE_URL = csSetting.GetAppStringSetting("MasterFileImport_DispErrorLogUrl");
				// SQLパラメータ設定ファイルパス
				Constants.PHYSICALDIRPATH_PARAMETERSFILE = AppDomain.CurrentDomain.BaseDirectory + @"Xml\Setting\Parameters.xml";
				// SQLステートメントファイルパス
				Constants.PHYSICALDIRPATH_SQL_STATEMENT = csSetting.GetAppStringSetting("Directory_w2cManagerSqlStatementXml");
				// 外部レコメンド自動連携フラグ
				Constants.MASTERFILEIMPORT_AUTO_RECOMMEND_FLG = csSetting.GetAppBoolSetting("MasterFileImport_AutoRecommendFlg");
				// 実在庫利用設定有無
				Constants.REALSTOCK_OPTION_ENABLED = csSetting.GetAppBoolSetting("RealStockOption_Enabled");
				// ポイント有効期間計算_追加月数
				Constants.CALC_POINT_EXP_ADDMONTH = csSetting.GetAppIntSetting("CalcPointExp_AddMonth");
				// エラーログファイルURL
				Constants.DATAMIGRATION_DISP_ERROR_LOG_URL = csSetting.GetAppStringSetting("DataMigration_DispErrorLogUrl");
				// Physical dir path data migration setting file path EC
				Constants.PHYSICALDIRPATH_DATAMIGRATION_SETTINGFILEPATH_EC = csSetting.GetAppStringSetting("MasterFileImport_DataMigration_SettingFilePath_EC");
				// Physical dir path data migration setting file path CS
				Constants.PHYSICALDIRPATH_DATAMIGRATION_SETTINGFILEPATH_CS = csSetting.GetAppStringSetting("MasterFileImport_DataMigration_SettingFilePath_CS");
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Configファイルの読み込みに失敗しました。", ex);
			}
		}

		/// <summary>
		/// アップロードファイル(header:user_id 内容:ユーザIDリスト)をMasterFileImportバッチにて取り込める形式に加工する
		/// </summary>
		/// <param name="uploadFilePath">アップロードファイルのフルパス</param>
		/// <param name="shopId">ショップID</param>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorName">更新者名</param>
		/// <param name="targetListName">ターゲットリスト名</param>
		private void ProcessingUploadTargetList(string uploadFilePath, string shopId, string deptId, string operatorName, string targetListName)
		{
			try
			{
				using (var accessor = new SqlAccessor())
				{
					// 新規ターゲットリストを挿入
					var newTargetId = TargetListUtility.InsertNewTargetList(shopId, deptId, operatorName, targetListName);

					var userIds = UploadFileUserId(uploadFilePath);

					accessor.OpenConnection();
					// ターゲットユーザリストの一時保持テーブルをTruncate(初期化)
					using (var statement = new SqlStatement("TargetList", "TruncateTempTargetUser"))
					{
						statement.ExecStatement(accessor, null);
					}

					using (var bulkCopy = new SqlBulkCopy(accessor.Connection.GetRealConnection()))
					{
						bulkCopy.BulkCopyTimeout = 0;
						bulkCopy.DestinationTableName = Constants.TABLE_TEMPTARGETUSER;
						bulkCopy.WriteToServer(userIds);
					}

					var tempCsvFilePath = Path.GetTempFileName();
					var header = TargetListUtility.BuildCsvHeader();
					var totalCount = 0;
					using (var statement = new SqlStatement("TargetList", "GetAllTargetUserList"))
					using (var reader = new SqlStatementDataReader(accessor, statement))
					using (var sw = new StreamWriter(tempCsvFilePath, false, Encoding.GetEncoding("Shift_JIS")))
					{
						sw.WriteLine(header);

						while (reader.Read())
						{
							var ht = new Hashtable();
							foreach (int i in Enumerable.Range(0, reader.FieldCount))
							{
								ht[reader.GetName(i)] = reader[i];
							}

							if (ht[Constants.FIELD_USER_USER_ID] == System.DBNull.Value)
							{
								continue;
							}

							var bulidCsvbody = TargetListUtility.BuildCsvBody(
								deptId,
								newTargetId,
								(string)ht[Constants.FIELD_USER_USER_ID],
								(string)ht[Constants.FIELD_USER_MAIL_ADDR],
								(string)ht[Constants.FIELD_USER_MAIL_ADDR2]);

							foreach (var line in bulidCsvbody)
							{
								sw.WriteLine(line);
								totalCount++;
							}
						}
					}

					// ターゲットユーザリストの一時保持テーブルをTruncate
					using (var statement = new SqlStatement("TargetList", "TruncateTempTargetUser"))
					{
						statement.ExecStatement(accessor, null);
					}

					File.Delete(uploadFilePath);
					File.Move(tempCsvFilePath, uploadFilePath);

					// ターゲット リストのデータ カウントを更新します。
					TargetListUtility.UpdateTargetListDataCount(newTargetId, totalCount, deptId);
				}
			}
			catch (Exception ex)
			{
				throw new ApplicationException("アクティブファイルへの変換処理に失敗しました。", ex);
			}
		}


		/// <summary>
		/// アップロードファイルのユーザIDを列挙する
		/// </summary>
		/// <param name="uploadFilePath">アップロードファイル</param>
		/// <returns>アップロードファイルのユーザIDの列挙</returns>
		private DataTable UploadFileUserId(string uploadFilePath)
		{
			DataTable dt = new DataTable();
			using (var reader = new StreamReader(uploadFilePath))
			{
				var line = reader.ReadLine();
				var header = line;
				var dtHeader = dt.Columns.Add(header, typeof(string));
				dt.PrimaryKey = new DataColumn[] { dt.Columns[header] };
				while ((line = reader.ReadLine()) != null)
				{
					var lines = StringUtility.SplitCsvLine(line);
					try
					{
						dt.Rows.Add(lines[0]);
					}
					catch (Exception) { }
				}
				return dt;
			}
		}

		/// <summary>
		/// マスタファイル取込（※作業ディレクトリファイルのみ）
		/// </summary>
		/// <param name="strUplodedFilePath">アップロード済みファイルパス</param>
		public void ImportActiveFile(string strUplodedFilePath)
		{
			//------------------------------------------------------
			// マスタディレクトリパス取得
			//------------------------------------------------------
			string[] strFilePathSplit = strUplodedFilePath.Split('\\');
			string strShopId = strFilePathSplit[strFilePathSplit.Length - 4];
			string strMaster = strFilePathSplit[strFilePathSplit.Length - 3];
			string strFileName = strFilePathSplit[strFilePathSplit.Length - 1];
			string strMasterPaths = Constants.PHYSICALDIRPATH_MASTERUPLOAD_DIR + @"\" + strShopId + @"\" + strMaster;

			//------------------------------------------------------
			// 各ディレクトリ作成
			//------------------------------------------------------
			CreateMoveDirectories(strMasterPaths);

			//------------------------------------------------------
			// １ファイル取込共通処理（作業ディレクトリファイル取込）
			//------------------------------------------------------
			ImportFile(strShopId, strMaster, strFileName, true);
		}
		/// <summary>
		/// マスタファイル取込（在庫連携用）
		/// </summary>
		/// <param name="strUplodedFilePath">アップロード済みファイルパス</param>
		/// <param name="strStockCooperation">在庫連携判定フラグ</param>
		public void ImportActiveFile(string strUplodedFilePath, string strStockCooperation)
		{
			this.StockCooperationFlg = (strStockCooperation == CONST_STR_COMMAND_STOCK_COOPERATION);
			ImportActiveFile(strUplodedFilePath);
		}

		/// <summary>
		/// マスタファイル取込（※全ファイル）
		/// </summary>
		/// <remarks>
		/// マスタファイルアップロードディレクトリパスに格納されている全てのファイルを取込
		/// </remarks>
		public void ImportAll()
		{
			// 店舗ID抽出
			string[] strShopIds = null;
			if (Directory.Exists(Constants.PHYSICALDIRPATH_MASTERUPLOAD_DIR) == false)
			{
				Directory.CreateDirectory(Constants.PHYSICALDIRPATH_MASTERUPLOAD_DIR);
			}
			string[] strShopIdPaths = Directory.GetDirectories(Constants.PHYSICALDIRPATH_MASTERUPLOAD_DIR);
			if (strShopIdPaths.Length != 0)
			{
				//------------------------------------------------------
				// 店舗IDについて繰り返し
				//------------------------------------------------------
				strShopIds = new string[strShopIdPaths.Length];
				for (int iShopLoop = 0; iShopLoop < strShopIds.Length; iShopLoop++)
				{
					strShopIds[iShopLoop] = strShopIdPaths[iShopLoop].Substring(strShopIdPaths[iShopLoop].LastIndexOf(@"\") + 1);
				}

				// 店舗IDについて繰り返し
				for (int iShopLoop = 0; iShopLoop < strShopIds.Length; iShopLoop++)
				{
					// マスタ種類抽出
					string[] strMasters = null;
					string[] strMasterPaths = Directory.GetDirectories(strShopIdPaths[iShopLoop]);
					if (strMasterPaths.Length != 0)
					{
						//------------------------------------------------------
						// 各マスタ種類について繰り返し
						//------------------------------------------------------
						strMasters = new string[strMasterPaths.Length];
						for (int iMasterLoop = 0; iMasterLoop < strMasters.Length; iMasterLoop++)
						{
							strMasters[iMasterLoop] = strMasterPaths[iMasterLoop].Substring(strMasterPaths[iMasterLoop].LastIndexOf(@"\") + 1);
						}

						// マスタについて繰り返し
						for (int iMasterLoop = 0; iMasterLoop < strMasters.Length; iMasterLoop++)
						{
							//------------------------------------------------------
							// 各ディレクトリ作成
							//------------------------------------------------------
							CreateMoveDirectories(strMasterPaths[iMasterLoop]);

							//------------------------------------------------------
							// 各ファイルについて繰り返し
							//------------------------------------------------------
							// ファイル名抽出
							string[] strFiles = null;
							string[] strFilePaths = Directory.GetFiles(strMasterPaths[iMasterLoop] + @"\" + Constants.DIRNAME_MASTERIMPORT_UPLOAD);
							if (strFilePaths.Length != 0)
							{
								strFiles = new string[strFilePaths.Length];
								for (int iFileLoop = 0; iFileLoop < strFilePaths.Length; iFileLoop++)
								{
									strFiles[iFileLoop] = strFilePaths[iFileLoop].Substring(strFilePaths[iFileLoop].LastIndexOf(@"\") + 1);
								}

								// ファイルについて繰り返し
								for (int iFileLoop = 0; iFileLoop < strFilePaths.Length; iFileLoop++)
								{
									//------------------------------------------------------
									// １ファイル取込共通処理
									//------------------------------------------------------
									ImportFile(strShopIds[iShopLoop], strMasters[iMasterLoop], strFiles[iFileLoop], false);
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// 指定ファイルを処理する
		/// </summary>
		/// <param name="strShopId">店舗ID</param>
		/// <param name="strMasterName">マスタ名（ディレクトリ名）</param>
		/// <param name="strFileName">ファイル名</param>
		/// <param name="blImportActive">作業ディレクトリファイル取込フラグ</param>
		public void ImportFile(string strShopId, string strMasterName, string strFileName, bool blImportActive)
		{
			//------------------------------------------------------
			// 各作業パス取得
			//------------------------------------------------------
			string strMasterUploadDirPath = Constants.PHYSICALDIRPATH_MASTERUPLOAD_DIR + strShopId + @"\" + strMasterName + @"\";
			string strUplodedFilePath = strMasterUploadDirPath + Constants.DIRNAME_MASTERIMPORT_UPLOAD + @"\" + strFileName;
			string strActiveFilePath = strMasterUploadDirPath + Constants.DIRNAME_MASTERIMPORT_ACTIVE + @"\" + strFileName;
			string strCompleteDirPath = strMasterUploadDirPath + Constants.DIRNAME_MASTERIMPORT_COMPLETE + @"\";

			//------------------------------------------------------
			// マスタ名取得（日本語）
			//------------------------------------------------------
			string strMasterNameJp = "";

			Dictionary<string, string> nodeList = new Dictionary<string, string>();
			SetXmlNode(nodeList, Constants.PHYSICALDIRPATH_MASTERUPLOAD_SETTING_EC);
			SetXmlNode(nodeList, Constants.PHYSICALDIRPATH_MASTERUPLOAD_SETTING_MP);
			SetXmlNode(nodeList, Constants.PHYSICALDIRPATH_MASTERUPLOAD_SETTING_CMS);
			SetXmlNode(nodeList, Constants.PHYSICALDIRPATH_DATAMIGRATION_SETTINGFILEPATH_EC);
			SetXmlNode(nodeList, Constants.PHYSICALDIRPATH_DATAMIGRATION_SETTINGFILEPATH_CS);

			strMasterNameJp = GetMasterName(strMasterName, nodeList);

			// 取込開始日時設定
			this.BeginDate = DateTime.Now;

			//------------------------------------------------------
			// マスタファイル取込
			//------------------------------------------------------
			Exception exception = null;
			bool blResult = false;
			try
			{
				// 別で同一マスタの処理が実行中の場合、処理が終了するまで待機
				var mutexName = CreateGlobalMutexNameForEachMaster(strMasterName);
				var mutex = new Mutex(false, mutexName);
				mutex.WaitOne();
				try
				{
					if (blImportActive == false)
					{
						// ファイル移動（アップロードディレクトリ⇒作業ディレクトリ）
						File.Move(strUplodedFilePath, strActiveFilePath);
					}

					// マスタファイル取込
					blResult = ImportCsvFile(strMasterName, strShopId, strActiveFilePath);
				}

				finally
				{
					// Mutexの解放
					mutex.ReleaseMutex();
				}
			}
			catch (Exception ex)
			{
				exception = ex;
			}

			//------------------------------------------------------
			// ファイル移動（作業ディレクトリ⇒完了ディレクトリ）
			//------------------------------------------------------
			// 完了ファイル名取得（"yyyyMMddHHmmss" + 完了ファイル付加コード + ファイル名）
			string strCompleteFileName = DateTime.Now.ToString("yyyyMMddHHmmss")
				+ (blResult ? Constants.IMPORT_RESULT_KBN_SUCCESS : Constants.IMPORT_RESULT_KBN_FAILED)
				+ strFileName;

			// 在庫連携の場合にはファイル存在チェックを行う。
			if ((this.StockCooperationFlg)
				&& (File.Exists(strActiveFilePath) == false))
			{
				// ファイルが存在しない場合はログを落として処理を終了させる。
				FileLogger.WriteError("在庫連携にて在庫ファイルが存在しませんでした。：" + strFileName);
				return;
			}

			// ファイル移動（作業ディレクトリ⇒完了ディレクトリ）
			try
			{
				File.Move(strActiveFilePath, strCompleteDirPath + strCompleteFileName);

				// ユーザマスタ(初期移行用、非入力チェック)ファイルは削除し、成功ログファイルを残す
				if ((strMasterName == Constants.TABLE_USER)
					|| (strMasterName == Constants.TABLE_USEREXTEND)
					|| (strMasterName == Constants.TABLE_USERNOTVALIDATOR))
				{
					File.Delete(strCompleteDirPath + strCompleteFileName);
					if (blResult)
					{
						File.CreateText(strCompleteDirPath + strCompleteFileName + ".log");
					}
				}
			}
			catch
			{
				// ファイルが移動できなかったらとりあえずエラーログに出力
				FileLogger.WriteError("ファイルを移動することができませんでした。：" + strFileName);
			}

			//------------------------------------------------------
			// エラーログ出力
			//------------------------------------------------------
			string strLogFileName = null;
			string messages = GetErrorMessage(strFileName, strMasterNameJp, exception);

			// 何らかのメッセージがある場合はログに出力
			if (messages.Length > 0)
			{
				// エラーログファイルを出力（ファイル名：同名＋.log）
				strLogFileName = strCompleteFileName + ".log";
				StreamWriter sw = new StreamWriter(strCompleteDirPath + strLogFileName);
				sw.WriteLine(messages.ToString());
				sw.Close();

				// バッチのエラーログにも出力
				FileLogger.WriteError(messages.ToString());
			}

			// 取込終了日時設定
			this.EndDate = DateTime.Now;

			// Create array table in data migration
			var tableInDataMigration = new string[]
			{
				Constants.TABLE_USERCREDITCARD,
				Constants.TABLE_ORDER,
				Constants.TABLE_FIXEDPURCHASE,
				Constants.TABLE_FIXEDPURCHASEITEM,
				Constants.TABLE_FIXEDPURCHASESHIPPING,
				Constants.TABLE_FIXEDPURCHASEHISTORY,
				Constants.TABLE_CSINCIDENT,
				Constants.TABLE_CSMESSAGE,
			};

			// Is table in data migration
			var isTableInDataMigration = tableInDataMigration.Contains(strMasterName);

			//送信メール設定を確認します。
			if (this.IsSendMail == false) return;

			//------------------------------------------------------
			// 完了メール送信
			//------------------------------------------------------
			StringBuilder mailBody = new StringBuilder();
			Hashtable htInput = new Hashtable();
			// マスタ名（日本語）
			htInput.Add(Constants.IMPORT_MAILTEMPLATE_KEY_MASTER_NAME, strMasterNameJp);
			// ファイル名
			htInput.Add(Constants.IMPORT_MAILTEMPLATE_KEY_FILE_NAME, strFileName);
			// 処理結果
			htInput.Add(Constants.IMPORT_MAILTEMPLATE_KEY_RESULT, (blResult ? "成功" : "失敗"));
			// 成功時
			if (blResult)
			{
				mailBody.Append("取込開始時間：").Append(this.BeginDate.ToString()).Append("\r\n");
				mailBody.Append("取込終了時間：").Append(this.EndDate.ToString()).Append("\r\n");
				mailBody.Append("\r\n");
				mailBody.Append("INS/UPD  ：").Append(this.ExecuteInsertUpdate.ToString()).Append("件").Append("\r\n");
				mailBody.Append("DELETE   ：").Append(this.ExecuteDelete.ToString()).Append("件");

				// 成功ログの出力
				FileLogger.WriteInfo(strMasterName + "マスタ取込に成功しました。（ファイル：" + strFileName + "）\r\n" + mailBody.ToString());

				if (messages.Length > 0) mailBody.AppendLine("　一部に警告が発生しています。");
			}

			if (messages.Length > 0)
			{
				if (isTableInDataMigration == false)
				{
					mailBody.Append("　エラーログをご確認ください↓").Append("\r\n");
					mailBody.Append(Constants.MASTERUPLOAD_ERRORLOGFILE_URL);
					mailBody.Append("?");
					mailBody.Append(Constants.REQUEST_KEY_MASTERIMPORTOUTPUTLOG_SHOP_ID).Append("=").Append(HttpUtility.UrlEncode(strShopId));
					mailBody.Append("&");
					mailBody.Append(Constants.REQUEST_KEY_MASTERIMPORTOUTPUTLOG_MASTER).Append("=").Append(HttpUtility.UrlEncode(strMasterName));
					mailBody.Append("&");
					mailBody.Append(Constants.REQUEST_KEY_MASTERIMPORTOUTPUTLOG_FILE_NAME).Append("=").Append(HttpUtility.UrlEncode(strLogFileName));
				}
				else
				{
					var logUrl = new UrlCreator(Constants.DATAMIGRATION_DISP_ERROR_LOG_URL)
						.AddParam(Constants.REQUEST_KEY_DATA_MIGRATION_SHOP_ID, HttpUtility.UrlEncode(strShopId))
						.AddParam(Constants.REQUEST_KEY_DATA_MIGRATION_MASTER, HttpUtility.UrlEncode(strMasterName))
						.AddParam(Constants.REQUEST_KEY_DATA_MIGRATION_OUTPUT_LOG_FILE_NAME, HttpUtility.HtmlAttributeEncode(strLogFileName));

					mailBody.AppendLine("　エラーログをご確認ください↓");
					mailBody.Append(Environment.NewLine);
					mailBody.Append(logUrl.CreateUrl());
				}
			}
			// メッセージ
			htInput.Add(Constants.IMPORT_MAILTEMPLATE_MSG, mailBody.ToString());

			if (isTableInDataMigration)
			{
				htInput[Constants.IMPORT_MAILTEMPLATE_KEY_MIGRATION_NAME] = strMasterNameJp;
			}

			// 該当店舗のメールテンプレート読込・メール送信
			string strMailTemplaitId = (this.StockCooperationFlg)
				? Constants.CONST_MAIL_ID_MASTER_UPLOAD_STOCK_COOPERATION
				: isTableInDataMigration
					? Constants.CONST_MAIL_ID_DATA_MIGRATION
					: Constants.CONST_MAIL_ID_MASTER_IMPORT;

			using (MailSendUtility msMailSend = new MailSendUtility(strShopId, strMailTemplaitId, "", htInput, true, Constants.MailSendMethod.Auto))
			{
				// 送信エラー？
				if (msMailSend.SendMail() == false)
				{
					// バッチのエラーログに出力
					FileLogger.WriteError(this.GetType().BaseType.ToString() + " : " + msMailSend.MailSendException.ToString());
				}
			}
		}

		/// <summary>
		/// マスターアップロード設定一覧を取得しまとめる
		/// </summary>
		/// <param name="nodeList">マスターアップロード設定一覧</param>
		/// <param name="xmlPath">Xmlが格納されているアドレス</param>
		public void SetXmlNode(Dictionary<string, string> nodeList, string xmlPath)
		{
			var parameter = from xe in XElement.Load(xmlPath).Elements()
							select new
							{
								Directory = xe.Element("Directory").Value,
								Name = xe.Element("Name").Value
							};

			foreach (var node in parameter)
			{
				if (nodeList.ContainsKey(node.Directory))
				{
					continue;
				}

				nodeList.Add(node.Directory, node.Name);
			}
		}

		/// <summary>
		/// マスタ名（日本語）取得
		/// </summary>
		/// <param name="masterName">区分値</param>
		/// <param name="dictionary">xmlのノード一覧</param>
		/// <returns>マスタ日本語名</returns>
		public string GetMasterName(string masterName, Dictionary<string, string> dictionary)
		{
			if (masterName == Constants.TABLE_TARGETLISTDATA) return "ターゲットリスト";
			return dictionary.ContainsKey(masterName) ? dictionary[masterName] : "不明なマスタ（" + masterName + "）";
		}

		/// <summary>
		/// Mutex名作成
		/// </summary>
		/// <param name="masterName">マスタ名</param>
		/// <returns>Mutex名</returns>
		public static string CreateGlobalMutexNameForEachMaster(string masterName)
		{
			var mutexName = string.Format(
				@"Global\{0}_{1}",
				Assembly.GetEntryAssembly().Location.Replace(@"\", "_").ToLower(),
				masterName.ToLower());
			return mutexName;
		}

		/// <summary>
		/// エラーログ用メッセージ取得
		/// </summary>
		/// <param name="strFileName">ファイル名</param>
		/// <param name="strMasterNameJp">対象マスタ名</param>
		/// <param name="exception">例外インスタンス</param>
		/// <returns>エラーメッセージ</returns>
		private string GetErrorMessage(string strFileName, string strMasterNameJp, Exception exception)
		{
			StringBuilder messages = new StringBuilder();
			// エラーログヘッダ
			if (HasError() || HasWarning() || exception != null)
			{
				// ファイル名、マスタ種別
				messages.Append("ファイル名：").Append(strFileName).AppendLine();
				messages.Append("マスタ種別：").Append(strMasterNameJp).AppendLine().AppendLine();
			}
			// エラー
			if (HasError())
			{
				messages.AppendLine("■エラー");
				messages.AppendLine("====================================================================================================");
				messages.AppendLine(this.ErrorMessages.ToString());
				messages.AppendLine("====================================================================================================");
				messages.AppendLine("⇒該当ファイルの処理はすべてロールバックされました。");
				messages.AppendLine();
			}
			// 警告？
			if (HasWarning())
			{
				messages.AppendLine("■警告");
				messages.AppendLine("====================================================================================================");
				messages.AppendLine(this.WarningMessage.ToString());
				messages.AppendLine("====================================================================================================");
				messages.AppendLine();
			}
			// 例外エラー？
			if (exception != null)
			{
				messages.AppendLine("■例外エラー");
				messages.AppendLine("====================================================================================================");
				messages.AppendLine(exception.ToString());
				messages.AppendLine("====================================================================================================");
				messages.AppendLine("⇒該当ファイルの処理はすべてロールバックされました。");
				messages.AppendLine();
			}

			return messages.ToString();
		}

		/// <summary>
		/// エラーがあったか
		/// </summary>
		/// <returns>エラーフラグ</returns>
		private bool HasError()
		{
			return this.ErrorMessages.Length > 0;
		}

		/// <summary>
		/// 警告があるか
		/// </summary>
		/// <returns>警告フラグ</returns>
		private bool HasWarning()
		{
			return this.WarningMessage.Length > 0;
		}

		/// <summary>
		/// 各ディレクトリ作成
		/// </summary>
		/// <param name="strMasterPath">マスタディレクトリパス</param>
		private void CreateMoveDirectories(string strMasterPath)
		{
			// 作業ディレクトリ作成
			if (Directory.Exists(strMasterPath + @"\" + Constants.DIRNAME_MASTERIMPORT_ACTIVE) == false)
			{
				Directory.CreateDirectory(strMasterPath + @"\" + Constants.DIRNAME_MASTERIMPORT_ACTIVE);
			}
			// 完了ディレクトリ作成
			if (Directory.Exists(strMasterPath + @"\" + Constants.DIRNAME_MASTERIMPORT_COMPLETE) == false)
			{
				Directory.CreateDirectory(strMasterPath + @"\" + Constants.DIRNAME_MASTERIMPORT_COMPLETE);
			}
		}

		/// <summary>
		/// CSVファイル解析＆取込処理
		/// </summary>
		/// <param name="tableName">テーブル名</param>
		/// <param name="shopId">店舗ID</param>
		/// <param name="csvFilePath">CSVファイルパス</param>
		/// <returns>ファイル取込結果</returns>
		private bool ImportCsvFile(string tableName, string shopId, string csvFilePath)
		{
			//------------------------------------------------------
			// 初期化
			//------------------------------------------------------
			this.ExecuteInsertUpdate = this.ExecuteDelete = 0;
			this.ErrorMessages = new StringBuilder();
			this.WarningMessage = new StringBuilder();

			//------------------------------------------------------
			// ファイルの存在チェック
			//------------------------------------------------------
			if (File.Exists(csvFilePath) == false)
			{
				SetErrorMessages("ファイルが存在しません。");
				return false;	// 処理を抜ける
			}

			//------------------------------------------------------
			// マスタ取込設定インスタンス取得
			//------------------------------------------------------
			// トランザクション設定
			this.Transaction = "マスタ種別チェック";
			// 現在対象行設定
			this.CurrentLine = 0;
			ImportSettingBase importSetting;
			try
			{
				importSetting = ImportSettingFactory.CreateInstance(tableName);

				// 実行前処理
				importSetting.OnBeforeExecute(csvFilePath);
				this.IsSendMail = importSetting.IsSendMail;
			}
			catch (w2Exception ex)
			{
				SetErrorMessages(ex.Message);
				return false;
			}

			//------------------------------------------------------
			// ヘッダフィールドチェック
			//------------------------------------------------------
			// トランザクション設定
			this.Transaction = "ヘッダフィールドチェック";
			// 現在対象行設定（１行目＝ヘッダフィールド）
			this.CurrentLine = 1;

			// エンコーダ取得
			byte[] csvByteStream = null;
			using (FileStream csvFileStream = new FileStream(csvFilePath, FileMode.Open, FileAccess.Read))
			{
				csvByteStream = new byte[csvFileStream.Length];
				csvFileStream.Read(csvByteStream, 0, csvByteStream.Length);
			}
			Encoding enc = StringUtility.GetCode(csvByteStream);
			if (enc == null)
			{
				SetErrorMessages("ファイルの読取に失敗しました。[Encoding Error.]");
				return false;	// 処理を抜ける
			}

			// CSVファイル読込
			List<Hashtable> fieldKeyValues = new List<Hashtable>();
			using (StreamReader reader = new StreamReader(csvFilePath, enc))
			{
				try
				{
					// ヘッダ行からフィールド名のリストを取得
					List<string> fields = GetFieldNames(reader.ReadLine());
					importSetting.SetFieldNames(fields);

					// 行情報を取得する
					fieldKeyValues = GetRowKeyValueList(reader, importSetting);
				}
				catch (w2Exception ex)
				{
					SetErrorMessages(ex.Message);
					return false;
				}
			}

			//------------------------------------------------------
			// データ変換
			//------------------------------------------------------
			// トランザクション設定
			this.Transaction = "データ変換＆入力チェック";
			// 現在対象行設定
			this.CurrentLine = 0;

			// エラーの場合
			if (this.ErrorMessages.Length != 0)
			{
				return false;	// 処理を抜ける
			}

			//------------------------------------------------------
			// 画像削除処理(処理を抜ける)
			//------------------------------------------------------
			if (tableName == Constants.ACTION_KBN_DELETE_PRODUCT_IMAGE)
			{
				return DeleteProductImage(shopId, fieldKeyValues);
			}

			//------------------------------------------------------
			// Insert用ヘッダフィールド設定
			//------------------------------------------------------
			importSetting.SetFieldNamesForInsert();

			//------------------------------------------------------
			// SQL文作成
			//------------------------------------------------------
			importSetting.CreateSql();

			// Create array tables relate order
			var orderTableList = new string[]
			{
				Constants.TABLE_ORDER,
				Constants.TABLE_ORDERITEM,
				Constants.TABLE_ORDERSHIPPING,
				Constants.TABLE_ORDERCOUPON,
				Constants.TABLE_ORDEROWNER,
				Constants.TABLE_ORDERPRICEBYTAXRATE,
			};

			if ((tableName != Constants.TABLE_DMSHIPPINGHISTORY)
				&& (orderTableList.Contains(tableName) == false))
			{
				//------------------------------------------------------
				// ワークテーブル更新
				//------------------------------------------------------
				// トランザクション設定
				this.Transaction = "ワークテーブル更新";
				// 現在対象行設定
				this.CurrentLine = 0;

				using (SqlAccessor sqlAccessor = new SqlAccessor())
				{
					try
					{
						//------------------------------------------------------
						// コネクションオープン & 「コミット済みデータ読み取り可能」でトランザクション開始
						//------------------------------------------------------
						sqlAccessor.OpenConnection();
						sqlAccessor.BeginTransaction(IsolationLevel.ReadCommitted);

						//------------------------------------------------------
						// ワークテーブル初期化
						// ※ワークテーブルをTrancate⇒各テーブルのデータをワークテーブルにInsert
						//------------------------------------------------------
						using (SqlStatement sqlStatement = new SqlStatement(importSetting.InitlializeWorkTableSql))
						{
							// タイムアウト時間を300秒に設定
							sqlStatement.CommandTimeout = 300;
							sqlStatement.ExecStatement(sqlAccessor);
						}

						//------------------------------------------------------
						// ワークテーブル更新
						//------------------------------------------------------
						using (SqlStatement sqlStatement = new SqlStatement(""))
						{
							//------------------------------------------------------
							// SQLパラメータ設定
							//------------------------------------------------------
							foreach (SqlStatement.SqlParam sqlParam in importSetting.InputParamDefines.Values)
							{
								// サイズ指定がある場合のみ
								if (sqlParam.Size.HasValue)
								{
									sqlStatement.AddInputParameters(sqlParam.Name, sqlParam.Type, (int)sqlParam.Size);
								}
								else
								{
									sqlStatement.AddInputParameters(sqlParam.Name, sqlParam.Type);
								}
							}

							// ヘッダ行を飛ばす
							this.CurrentLine = 1;
							string strImportKbn;
							foreach (Hashtable htInput in fieldKeyValues)
							{
								this.CurrentLine++; // 現在対象行カウンタをインクリメント

								//------------------------------------------------------
								// SQLステートメント設定
								//------------------------------------------------------
								strImportKbn = (string)htInput[Constants.IMPORT_KBN];
								switch (strImportKbn)
								{
									// Insert/Update
									case Constants.IMPORT_KBN_INSERT_UPDATE:
										sqlStatement.Statement = importSetting.InsertUpdateWorkSql;
										break;

									// Delete
									case Constants.IMPORT_KBN_DELETE:
										sqlStatement.Statement = importSetting.DeleteWorkSql;
										break;
								}

								//------------------------------------------------------
								// SQL実行
								//------------------------------------------------------
								int iResult = sqlStatement.ExecStatement(sqlAccessor, htInput);

								// 処理件数が０件の場合
								if (iResult == 0)
								{
									throw new ApplicationException("ワークテーブル更新処理に失敗しました。（例外エラー参照）");
								}

								// CrossPoint連携
								if (Constants.CROSS_POINT_OPTION_ENABLED)
								{
									var errorMessage = string.Empty;
									if (importSetting.TableName == Constants.TABLE_USER && strImportKbn == Constants.IMPORT_KBN_INSERT_UPDATE)
									{
										errorMessage = InsertUpdateUserCrossPoint(htInput, sqlAccessor);
									}

									if (string.IsNullOrEmpty(errorMessage) == false)
									{
										var error = ErrorHelper.CreateCrossPointApiError(
											errorMessage,
											StringUtility.ToEmpty(htInput[Constants.FIELD_USER_USER_ID]));

										FileLogger.WriteError(error);
										throw new Exception();
									}
								}
							} // foreach (Hashtable htInput in lFieldKeyValues)
						}

						// ワークテーブルをTruncate
						using (var statement = new SqlStatement(importSetting.TruncateWorkTableSql))
						{
							statement.ExecStatement(sqlAccessor, null);
						}

						//------------------------------------------------------
						// トランザクションコミット
						//------------------------------------------------------
						// ここまでエラーがなければはじめてコミット
						sqlAccessor.CommitTransaction();
					}
					catch (Exception ex)
					{
						// エラーメッセージを格納
						SetErrorMessages(ex.Message);

						// トランザクションロールバック
						sqlAccessor.RollbackTransaction();
						throw ex;
					}
				} //using (sqlAccessor = new SqlAccessor())
			}

			//------------------------------------------------------
			// 整合性チェック
			//------------------------------------------------------
			// 現在対象行設定
			this.CurrentLine = 0;

			// トランザクション設定
			this.Transaction = "整合性チェック";

			// エラーの場合 ※ユーザマスタ(非入力チェック)の場合は整合性チェックを行わない
			string strErrorMessages = "";
			if (tableName != Constants.TABLE_USERNOTVALIDATOR)
			{
				strErrorMessages = importSetting.CheckDataConsistency();
			}

			if (strErrorMessages != "")
			{
				SetErrorMessages(strErrorMessages);
				return false;	// 処理を抜ける
			}

			//------------------------------------------------------
			// テーブル更新
			//------------------------------------------------------
			// トランザクション設定
			this.Transaction = "テーブル更新";
			// 現在対象行設定
			this.CurrentLine = 0;

			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement(""))		// SQLはダミー
			using (SqlStatement sqlStatementHistory = new SqlStatement(""))	// SQLはダミー
			{
				//------------------------------------------------------
				// SQLパラメータ設定
				//------------------------------------------------------
				foreach (SqlStatement.SqlParam sqlParam in importSetting.InputParamDefines.Values)
				{
					// サイズ指定がある場合のみ
					if (sqlParam.Size.HasValue)
					{
						// 対象テーブル、対象履歴テーブル
						sqlStatement.AddInputParameters(sqlParam.Name, sqlParam.Type, (int)sqlParam.Size);
						sqlStatementHistory.AddInputParameters(sqlParam.Name, sqlParam.Type, (int)sqlParam.Size);
					}
					else
					{
						// 対象テーブル、対象履歴テーブル
						sqlStatement.AddInputParameters(sqlParam.Name, sqlParam.Type);
						sqlStatementHistory.AddInputParameters(sqlParam.Name, sqlParam.Type);
					}
				}

				// コネクションオープン
				sqlAccessor.OpenConnection();

				// ヘッダ行を飛ばす
				this.CurrentLine = 1;
				string strImportKbn;
				foreach (Hashtable htInput in fieldKeyValues)
				{
					// 現在対象行をインクリメント
					this.CurrentLine++;

					//------------------------------------------------------
					// SQLステートメント設定
					//------------------------------------------------------
					// 対象テーブル
					strImportKbn = (string)htInput[Constants.IMPORT_KBN];
					switch (strImportKbn)
					{
						// Insert/Update
						case Constants.IMPORT_KBN_INSERT_UPDATE:
							sqlStatement.Statement = importSetting.InsertUpdateSql;
							break;

						// Delete
						case Constants.IMPORT_KBN_DELETE:
							sqlStatement.Statement = importSetting.DeleteSql;
							break;
					}

					// 対象履歴テーブル
					sqlStatementHistory.Statement = null;
					// 商品在庫情報？
					if (importSetting.TableName == Constants.TABLE_PRODUCTSTOCK)
					{
						switch (strImportKbn)
						{
							// Insert/Update
							case Constants.IMPORT_KBN_INSERT_UPDATE:
								sqlStatementHistory.Statement =
									((ImportSettingProductStock)importSetting).ProductStockHistoryInsertSql;
								break;

							// Delete
							case Constants.IMPORT_KBN_DELETE:
								sqlStatementHistory.Statement =
									((ImportSettingProductStock)importSetting).ProductStockHistoryDeleteSql;
								break;
						}
					}
					// ユーザーポイント情報？
					else if (importSetting.TableName == Constants.TABLE_USERPOINT)
					{
						switch (strImportKbn)
						{
							// Insert/Update
							case Constants.IMPORT_KBN_INSERT_UPDATE:
								sqlStatementHistory.Statement =
									((ImportSettingUserPoint)importSetting).UserPointHistoryInsertSql;
								break;

							// Delete
							case Constants.IMPORT_KBN_DELETE:
								sqlStatementHistory.Statement =
									((ImportSettingUserPoint)importSetting).UserPointHistoryDeleteSql;
								// ユーザーテーブルの最終更新者を更新
								var userService = new UserService();
								userService.Modify((string)htInput[Constants.FIELD_USERPOINT_USER_ID],
									model =>
									{
										model.LastChanged = Constants.FLG_LASTCHANGED_BATCH;
									}, UpdateHistoryAction.DoNotInsert, sqlAccessor);
								break;
						}
					}

					//------------------------------------------------------
					// SQL実行
					//------------------------------------------------------
					// 対象テーブル
					int iResult = sqlStatement.ExecStatement(sqlAccessor, htInput);
					// 処理成功時は強制的に処理件数１件とする
					// ※在庫マスタ更新時にw2_AddMallCooperationLogTrトリガが実行され、戻り処理件数が一致しなくなるため
					iResult = iResult > 0 ? 1 : 0;

					// 対象履歴テーブル
					if (sqlStatementHistory.Statement != null)
					{
						sqlStatementHistory.ExecStatement(sqlAccessor, htInput);
					}

					//------------------------------------------------------
					// 処理件数加算
					//------------------------------------------------------
					switch (strImportKbn)
					{
						// Insert/Update
						case Constants.IMPORT_KBN_INSERT_UPDATE:
							this.ExecuteInsertUpdate += iResult;
							break;

						// Delete
						case Constants.IMPORT_KBN_DELETE:
							this.ExecuteDelete += iResult;
							break;
					}

					//------------------------------------------------------
					// モール出品設定情報更新
					//------------------------------------------------------
					//  モール出品設定使用有＆対象テーブルが商品情報の場合
					if ((Constants.MALLCOOPERATION_OPTION_ENABLED)
						&& (importSetting.TableName == Constants.TABLE_PRODUCT))
					{
						switch (strImportKbn)
						{
							// Insert/Update
							case Constants.IMPORT_KBN_INSERT_UPDATE:
								// モール出品設定情報登録
								((ImportSettingProduct)importSetting).InsertMallExhibitsConfig(sqlAccessor, htInput);
								break;

							// Delete
							case Constants.IMPORT_KBN_DELETE:
								// モール出品設定情報削除
								((ImportSettingProduct)importSetting).DeleteMallExhibitsConfig(sqlAccessor, htInput);
								break;
						}
					}

					// ユーザークーポン利用履歴登録(ユーザークーポン情報更新時)
					if (importSetting.TableName == Constants.TABLE_USERCOUPON)
					{
						var flgDeleteHistory = false;
						switch (strImportKbn)
						{
							// Insert/Update
							case Constants.IMPORT_KBN_INSERT_UPDATE:
								((ImportSettingUserCoupon)importSetting).InsertUserCouponHistory(sqlAccessor, htInput, flgDeleteHistory);
								break;

							// Delete
							case Constants.IMPORT_KBN_DELETE:
								flgDeleteHistory = true;
								((ImportSettingUserCoupon)importSetting).InsertUserCouponHistory(sqlAccessor, htInput, flgDeleteHistory);
								break;
						}
					}

					// ユーザークーポン利用履歴登録(クーポン利用ユーザー情報更新時)
					if ((importSetting.TableName == Constants.TABLE_COUPONUSEUSER)
						&& (strImportKbn == Constants.IMPORT_KBN_INSERT_UPDATE))
					{
						((ImportSettingCouponUseUser)importSetting).InsertUserCouponHistory(sqlAccessor, htInput);
					}

					// 更新履歴登録
					if (iResult > 0)
					{
						switch (importSetting.TableName)
						{
							case Constants.TABLE_USER:
							case Constants.TABLE_USEREXTEND:
							case Constants.TABLE_USERCOUPON:
							case Constants.TABLE_USERPOINT:
							case Constants.TABLE_USERSHIPPING:
								new UpdateHistoryService().InsertForUser(
									(string)htInput[Constants.FIELD_USER_USER_ID],
									Constants.FLG_LASTCHANGED_BATCH,
									sqlAccessor);
								break;
						}
					}

					// 最新のポイントとユーザー情報を取得し更新
					if (Constants.CROSS_POINT_OPTION_ENABLED && (importSetting.TableName != Constants.TABLE_USERPOINT))
					{
						try
						{
							var user = DomainFacade.Instance.UserService.Get(StringUtility.ToEmpty(htInput[Constants.FIELD_USER_USER_ID]));

							UserUtility.AdjustPointAndMemberRankByCrossPointApi(user);
						}
						catch (Exception ex)
						{
							// エラーメッセージを格納
							SetErrorMessages(ex.Message);

							throw ex;
						}
					}
				}
			}

			// 完了時実行処理
			importSetting.OnCompleteExecute();

			return true;
		}

		/// <summary>
		/// 全ての行情報のリストを取得する
		/// </summary>
		/// <param name="reader">CSVテキストリーダ</param>
		/// <param name="importSetting">取込設定</param>
		/// <returns>行情報のリスト</returns>
		private List<Hashtable> GetRowKeyValueList(TextReader reader, ImportSettingBase importSetting)
		{
			List<Hashtable> result = new List<Hashtable>();
			int errorCount = 0;
			// 行情報を１行ずつ読込
			while (reader.Peek() != -1)
			{
				// 現在の対象行を表すためインクリメント
				this.CurrentLine++;

				var errorMessages = new StringBuilder();

				// １行をCSV分割
				var datas = StringUtility.SplitCsvLine(GetRowData(reader)).ToList();

				//------------------------------------------------------
				// テーブル操作（TBL）が存在するかチェック
				//------------------------------------------------------
				var importKbn = "";
				if (this.IsExistTbl)
				{
					importKbn = StringUtility.ToHankaku(datas[0].Trim()).ToUpper();
					switch (importKbn)
					{
						// Insert/Update
						case Constants.IMPORT_KBN_INSERT_UPDATE:
						// Delete
						case Constants.IMPORT_KBN_DELETE:
							break;
						// 上記以外
						default:
							errorMessages.Append((errorMessages.Length != 0) ? "\r\n" : "");
							errorMessages.Append("TBL「").Append(importKbn).Append("」は存在しないテーブル操作です。");
							break;
					}
				}
				else
				{
					// テーブル操作（TBL）が存在しなければIUとして補完 
					importKbn = Constants.IMPORT_KBN_INSERT_UPDATE;
				}

				// フィールド数がヘッダフィールド数と合っているかチェック
				var fieldsCount = (this.IsExistTbl) ? (importSetting.HeadersCsv.Count + 1) : (importSetting.HeadersCsv.Count);
				if (fieldsCount != datas.Count)
				{
					errorMessages.Append("行のフィールド数が定義と一致しませんでした。");
					errorMessages.Append("フィールド定義数は").Append(((int)(importSetting.HeadersCsv.Count + 1))).Append("ですがデータのフィールド数は").Append(((int)(datas.Count))).Append("でした。");
				}

				// ここまでの処理でエラーが発生していない？
				if (errorMessages.Length == 0)
				{
					//------------------------------------------------------
					// データをハッシュテーブルに格納
					//------------------------------------------------------
					Hashtable htInput = new Hashtable();
					// 取込区分は別キーで格納し、テーブル操作（TBL）が存在すれば配列のテーブル操作（TBL）を削除
					htInput.Add(Constants.IMPORT_KBN, importKbn);

					if (this.IsExistTbl) datas.RemoveAt(0);

					// データをハッシュテーブルに格納
					foreach (string strFieldName in importSetting.HeadersCsv)
					{
						htInput.Add(strFieldName, datas[importSetting.HeadersCsv.IndexOf(strFieldName)]);
					}

					//------------------------------------------------------
					// データ変換＆入力チェック
					//------------------------------------------------------
					importSetting.ConvertAndCheck(htInput);

					// エラーメッセージを格納
					if (importSetting.ErrorMessages != "")
					{
						errorMessages.Append(importSetting.ErrorMessages);
					}

					// 警告メッセージを格納
					if (StringUtility.ToEmpty(importSetting.WarningMessages) != "")
					{
						SetWarningMessages(importSetting.WarningMessages);
					}
				}

				// エラーメッセージを格納
				if (errorMessages.Length != 0)
				{
					SetErrorMessages(errorMessages.ToString(), importSetting.ErrorOccurredIdInfo);
					errorCount++;
					if (errorCount >= 100)
					{
						this.ErrorMessages.Append("\r\n");
						this.ErrorMessages.Append("※エラーが100件以上あるため、取込処理を打ち切ります。").Append("\r\n");
						break;
					}
					continue;
				}

				//------------------------------------------------------
				// データ変換後データを格納
				//------------------------------------------------------
				result.Add(importSetting.Data);
			}
			return result;
		}

		/// <summary>
		/// 1行のデータを取得（行の終端までストリームを読み進める）
		/// </summary>
		/// <remarks>
		/// 文字列に改行が含まれるケースに対応するため、
		/// ダブルクオートの数を数えて行の終わりを判定。
		/// </remarks>
		/// <param name="reader">CSVテキストリーダ</param>
		/// <returns>行データ</returns>
		private string GetRowData(TextReader reader)
		{
			StringBuilder rowData = new StringBuilder(reader.ReadLine());

			// 一行のデータを完全に取得できるまでReadLine()
			while (IsCompleteRow(rowData.ToString()) == false)
			{
				// 最後の行まで到達してもなおWhileが回っている場合はエラーを投げる
				if (reader.Peek() == -1) throw new w2Exception(this.CurrentLine.ToString() + "行目に含まれるダブルクオーテーション（\"）の数が奇数のため、正常に解析を行うことが出来ませんでした。");

				rowData.Append("\r\n").Append(reader.ReadLine());
			}

			return rowData.ToString();
		}

		/// <summary>
		/// 一行のデータが全てセットされているか
		/// </summary>
		/// <param name="rowData">行データ</param>
		/// <returns>行完成フラグ</returns>
		private bool IsCompleteRow(string rowData)
		{
			// （CSVの行に改行がある場合、「"」は奇数個のはず）
			return rowData.Count(ch => ch == '"') % 2 == 0;
		}

		/// <summary>
		/// フィールド名取得
		/// </summary>
		/// <param name="header">ヘッダ行</param>
		/// <returns>フィールド名リスト</returns>
		private List<string> GetFieldNames(string header)
		{
			if (header == null) throw new w2Exception("ファイルの読取に失敗しました。");

			var fields = StringUtility.SplitCsvLine(header).ToList();

			// TBL列の有無
			this.IsExistTbl = (fields[0] == Constants.IMPORT_HEADER_FIELD);

			string errorMessages;
			if (IsDuplication(fields, out errorMessages)) throw new w2Exception(errorMessages);

			return fields;
		}

		/// <summary>
		/// 対象のリストが重複しているか
		/// </summary>
		/// <param name="fields">フィールドリスト</param>
		/// <param name="errorMessage">エラーメッセージ</param>
		/// <returns>重複フラグ</returns>
		private bool IsDuplication(List<string> fields, out string errorMessage)
		{
			StringBuilder errorMessages = new StringBuilder();
			// ヘッダ項目が重複していないかチェック
			foreach (string field in fields.Distinct())
			{
				if (fields.Count(temp => temp == field) > 1)
				{
					errorMessages.Append((errorMessages.Length != 0) ? "\r\n" : "");
					errorMessages.Append("更新フィールドが重複しています。：").Append(field);
				}
			}
			errorMessage = errorMessages.ToString();
			return errorMessage.Length != 0;
		}

		/// <summary>
		/// 商品画像削除処理
		/// </summary>
		/// <param name="strShopId"></param>
		private bool DeleteProductImage(string strShopId, List<Hashtable> lFieldKeyValues)
		{
			//------------------------------------------------------
			// 各ディレクトリパス設定
			//------------------------------------------------------
			// 商品画像ディレクトリパス
			string strProductImageDirectoryPath = Constants.PHYSICALDIRPATH_CONTENTS_ROOT + Constants.PATH_PRODUCTIMAGES + strShopId + @"\";
			// 商品サブ画像ディレクトリパス
			string strProductSubImageDirectoryPath = Constants.PHYSICALDIRPATH_CONTENTS_ROOT + Constants.PATH_PRODUCTSUBIMAGES + strShopId + @"\";
			// ヘッダ行を飛ばす
			this.CurrentLine = 1;
			foreach (Hashtable htInput in lFieldKeyValues)
			{
				// 現在対象行をインクリメント
				this.CurrentLine++;

				// 区分が"D"のみ画像削除
				if ((string)htInput[Constants.IMPORT_KBN] == Constants.IMPORT_KBN_DELETE)
				{
					//------------------------------------------------------
					// PC商品画像
					//------------------------------------------------------
					if (htInput.ContainsKey(Constants.FIELD_PRODUCT_IMAGE_HEAD))
					{
						// メイン画像削除([PC商品画像ヘッダ] + [フッタ])
						DeleteFile(strProductImageDirectoryPath, (string)htInput[Constants.FIELD_PRODUCT_IMAGE_HEAD] + Constants.PRODUCTIMAGE_FOOTER_S);
						DeleteFile(strProductImageDirectoryPath, (string)htInput[Constants.FIELD_PRODUCT_IMAGE_HEAD] + Constants.PRODUCTIMAGE_FOOTER_M);
						DeleteFile(strProductImageDirectoryPath, (string)htInput[Constants.FIELD_PRODUCT_IMAGE_HEAD] + Constants.PRODUCTIMAGE_FOOTER_L);
						DeleteFile(strProductImageDirectoryPath, (string)htInput[Constants.FIELD_PRODUCT_IMAGE_HEAD] + Constants.PRODUCTIMAGE_FOOTER_LL);

						// サブ画像削除([PC商品画像ヘッダ]_sub*)
						DeleteFile(strProductSubImageDirectoryPath, (string)htInput[Constants.FIELD_PRODUCT_IMAGE_HEAD] + Constants.PRODUCTSUBIMAGE_FOOTER + "*");
					}

					//------------------------------------------------------
					// PCバリエーション商品画像
					//------------------------------------------------------
					if (htInput.ContainsKey(Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD))
					{
						// メイン画像削除([PCバリエーション画像ヘッダ] + [フッタ])
						DeleteFile(strProductImageDirectoryPath, (string)htInput[Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD] + Constants.PRODUCTIMAGE_FOOTER_S);
						DeleteFile(strProductImageDirectoryPath, (string)htInput[Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD] + Constants.PRODUCTIMAGE_FOOTER_M);
						DeleteFile(strProductImageDirectoryPath, (string)htInput[Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD] + Constants.PRODUCTIMAGE_FOOTER_L);
						DeleteFile(strProductImageDirectoryPath, (string)htInput[Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD] + Constants.PRODUCTIMAGE_FOOTER_LL);
					}
				}
			}

			return true;
		}

		/// <summary>
		/// ファイル削除
		/// </summary>
		/// <param name="strImageFileDirectoryPath">商品画像ディレクトリパス</param>
		/// <param name="strImageFileSearchPattern">>商品画像検索条件</param>
		private void DeleteFile(string strImageFileDirectoryPath, string strImageFileSearchPattern)
		{
			// 正確なディレクトリパスと検索条件を生成
			string strDirectoryPath = Path.GetDirectoryName(strImageFileDirectoryPath + strImageFileSearchPattern);
			string strSearchPattern = Path.GetFileName(strImageFileDirectoryPath + strImageFileSearchPattern);

			if (Directory.Exists(strDirectoryPath))
			{
				foreach (string strImageFileName in Directory.GetFiles(strDirectoryPath, strSearchPattern))
				{
					this.ExecuteDelete++;
					File.Delete(strImageFileName);
				}
			}
		}

		/// <summary>
		/// 現在対象行のエラーメッセージ設定
		/// </summary>
		/// <param name="strErrorMessages">エラーメッセージ</param>
		/// <param name="errorOccuredIdInfo">エラー発生ID情報</param>
		private void SetErrorMessages(string strErrorMessages, string errorOccuredIdInfo = "")
		{
			if (this.ErrorMessages.Length != 0)
			{
				this.ErrorMessages.Append("\r\n");
			}
			if (this.Transaction != "")
			{
				this.ErrorMessages.Append("[").Append(this.Transaction).Append("]");
			}
			if (this.CurrentLine > 0)
			{
				this.ErrorMessages.Append("(CSVファイルの").Append(this.CurrentLine.ToString()).Append("行目でエラー発生)");
			}
			if (errorOccuredIdInfo != "")
			{
				this.ErrorMessages.Append(" ").Append(errorOccuredIdInfo);
			}
			this.ErrorMessages.Append("\r\n");
			this.ErrorMessages.Append("----------------------------------------------------------------------------------------------------").Append("\r\n");
			this.ErrorMessages.Append(strErrorMessages).Append("\r\n");
			this.ErrorMessages.Append("----------------------------------------------------------------------------------------------------").Append("\r\n");
		}

		/// <summary>
		/// 外部レコメンド連携時のアイテムログ作成
		/// </summary>
		/// <param name="strMasterKbn">マスタ区分</param>
		/// <param name="strMasterId">マスタID</param>
		private void RecommendItemCoop(string strMasterKbn, string strMasterId)
		{
			// 外部レコメンド連携用の商品ログを作成
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("RecommendCoopUpdateLog", "InsertRecommendItemLog"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_RECOMMENDCOOPUPDATELOG_SHOP_ID, Constants.CONST_DEFAULT_SHOP_ID);
				htInput.Add(Constants.FIELD_RECOMMENDCOOPUPDATELOG_MASTER_KBN, strMasterKbn);	// マスタ区分(商品マスタ)
				htInput.Add(Constants.FIELD_RECOMMENDCOOPUPDATELOG_MASTER_ID, strMasterId);		// マスタID(商品ID)
				htInput.Add(Constants.FIELD_RECOMMENDCOOPUPDATELOG_LAST_CHANGED, "batch");		// 最終更新者

				int iResult = sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
			}
		}

		/// <summary>
		/// 現在対象行の警告メッセージ設定
		/// </summary>
		/// <param name="strErrorMessages">警告メッセージ</param>
		private void SetWarningMessages(string strWarningMessages)
		{
			if (this.WarningMessage.Length != 0)
			{
				this.WarningMessage.Append("\r\n");
			}
			if (this.Transaction != "")
			{
				this.WarningMessage.Append("[").Append(this.Transaction).Append("]");
			}
			if (this.CurrentLine > 0)
			{
				this.WarningMessage.Append("(CSVファイルの").Append(this.CurrentLine.ToString()).Append("行目で警告発生)");
			}
			this.WarningMessage.Append("\r\n");
			this.WarningMessage.Append("----------------------------------------------------------------------------------------------------").Append("\r\n");
			this.WarningMessage.Append(strWarningMessages).Append("\r\n");
			this.WarningMessage.Append("----------------------------------------------------------------------------------------------------").Append("\r\n");
		}

		/// <summary>
		/// Insert update user cross point
		/// </summary>
		/// <param name="userInput">User input</param>
		/// <param name="accessor">accessor</param>
		/// <returns>Error message</returns>
		private string InsertUpdateUserCrossPoint(Hashtable userInput, SqlAccessor accessor)
		{
			var crossPointUser = new CrossPointUserApiService().Get(StringUtility.ToEmpty(userInput[Constants.FIELD_USER_USER_ID]));
			var user = new UserService().GetWorkUser(
				StringUtility.ToEmpty(userInput[Constants.FIELD_USER_USER_ID]),
				accessor);

			if (crossPointUser == null)
			{
				var insertResult = new CrossPointUserApiService().Insert(user);

				if (insertResult.IsSuccess == false)
				{
					return insertResult.ErrorMessages;
				}
			}
			else
			{
				var updateResult = new CrossPointUserApiService().Update(user);

				if (updateResult.IsSuccess == false)
				{
					return updateResult.ErrorMessages;
				}
			}

			return string.Empty;
		}

		/// <summary>
		/// Delete user cross point
		/// </summary>
		/// <param name="userInput">User input</param>
		/// <returns>Error message</returns>
		private string DeleteUserCrossPoint(Hashtable userInput)
		{
			var userModel = new UserModel(userInput);
			var deleteResult = new CrossPointUserApiService().Withdraw(userModel);

			if (deleteResult.IsSuccess == false)
			{
				return deleteResult.ErrorMessages;
			}

			return string.Empty;
		}

		/// <summary>
		/// Update user point cross point
		/// </summary>
		/// <param name="userPointInput">User point input</param>
		/// <param name="accessor">accesser</param>
		/// <returns>Error message</returns>
		private string UpdateUserPointCrossPoint(Hashtable userPointInput, SqlAccessor accessor)
		{
			var pointValid = userPointInput.Contains(Constants.FIELD_USERPOINT_POINT);
			var userId = StringUtility.ToEmpty(userPointInput[Constants.FIELD_USER_USER_ID]);
			var user = DomainFacade.Instance.UserService.Get(userId, accessor);

			if ((user != null) && pointValid)
			{
				var point = decimal.Parse(StringUtility.ToEmpty(userPointInput[Constants.FIELD_USERPOINT_POINT]));
				var result = CrossPointUtility.UpdateCrossPointApiWithWebErrorMessage(
					user,
					point,
					w2.App.Common.Constants.CROSS_POINT_REASON_KBN_UPLOAD);

				if (string.IsNullOrEmpty(result))
				{
					UserUtility.AdjustPointAndMemberRankByCrossPointApi(user);
				}

				return result;
			}
			else
			{
				return MessageManager.GetMessages(w2.App.Common.Constants.ERRMSG_CROSSPOINT_LINKAGE_ERROR);
			}
		}

		/// <summary>トランザクション</summary>
		private string Transaction { get; set; }
		/// <summary>現在対象行</summary>
		private int CurrentLine { get; set; }
		/// <summary>Insert/Update実行回数</summary>
		private int ExecuteInsertUpdate { get; set; }
		/// <summary>Delete実行回数</summary>
		private int ExecuteDelete { get; set; }
		/// <summary>エラーメッセージ</summary>
		private StringBuilder ErrorMessages { get; set; }
		/// <summary>警告メッセージ</summary>
		private StringBuilder WarningMessage { get; set; }
		/// <summary>取込開始日時</summary>
		private DateTime BeginDate { get; set; }
		/// <summary>取込終了日時</summary>
		private DateTime EndDate { get; set; }
		/// <summary>在庫連携フラグ</summary>
		private bool StockCooperationFlg { get; set; }
		/// <summary> 送信メール設定 </summary>
		private bool IsSendMail { get; set; }
		/// <summary> TBL列が存在するか</summary>
		private bool IsExistTbl { get; set; }
	}
}
