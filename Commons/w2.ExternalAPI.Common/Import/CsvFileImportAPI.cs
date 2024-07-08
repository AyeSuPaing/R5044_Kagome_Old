/*
=========================================================================================================
  Module      : Csvファイル用インポート処理(CsvFileImportAPI.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Data;
using w2.ExternalAPI.Common.FrameWork.File;
using w2.ExternalAPI.Common.Entry;
using w2.ExternalAPI.Common.Logging;
using w2.ExternalAPI.Common.Extension;

namespace w2.ExternalAPI.Common.Import
{
	/// <summary>
	///	CSVファイルインポート処理クラス
	/// </summary>
	/// <remarks>
	/// CSVファイルを読み込み、1レコード単位で渡されたコマンドビルダーを実行する
	/// </remarks>
	public class CsvFileImportAPI : ImportApiBase
	{
		#region メンバ変数
		private readonly BackUp m_backUp;
		private readonly CsvSetting m_csvSetting;
		#endregion

		#region コンストラクタ
		/// <summary>コンストラクタ</summary>
		/// <param name="executeTarget">ターゲット情報</param>
		///<param name="ApiImportCommandBuilder">インポート行単位で実行したいコマンドビルダー</param>
		///<param name="csvSetting">CSV設定</param>
		public CsvFileImportAPI(ExecuteTarget executeTarget,ApiImportCommandBuilder ApiImportCommandBuilder,CsvSetting csvSetting)
			: base(executeTarget, ApiImportCommandBuilder)
		{
			m_backUp = new FileBackUp(executeTarget);
			m_csvSetting = csvSetting;
		}
		#endregion

		#region #PreExecute 実行前処理の実装
		/// <summary>
		/// 実行前処理の実装
		/// </summary>
		protected override void PreExecute()
		{
			// ワークファイルを削除
			new FileInfo(base.m_executeTarget.WorkFilePath).ForceDelete();

			// バックアップ実行
			m_backUp.Do();

			// ターゲットファイルをワークに移動
			File.Move(base.m_executeTarget.TargetFilePath, base.m_executeTarget.WorkFilePath);

			// コマンドビルダー実行前処理
			m_commandBuilder.PreDo();
		}
		#endregion

		#region #Execute インポート実行の実装
		/// <summary>
		/// インポート実行の実装
		/// </summary>
		protected override bool Execute()
		{
			bool result = true;

			w2.Common.Logger.FileLogger.Write("ImportStock", "Execute START");

			// Csvリーダー生成
			using (CsvReader csvReader = CsvStreamBuilder.BuildCsvReader(m_executeTarget.WorkFilePath, m_csvSetting))
			{
				int lineNumber = 1;
				DataRow row = csvReader.ReadLine();

				w2.Common.Logger.FileLogger.Write("ImportStock", "row：" + row);

				while (row != null)
				{
					try
					{
						w2.Common.Logger.FileLogger.Write("ImportStock", "ApiEntry作成内容ログ");

						// ApiEntry作成内容ログ
						ApiLogger.Write(LogLevel.info,
										"ファイル読み込み情報ログ",
						                string.Format("BackUpPath:'{0}',lineData:'{1}',"
						                              + ",WorkPath:'{2}',excutedTime:'{3}',lineNumber:'{4}',TargetFilePath:'{5}'",
						                              m_backUp.BackUpPath ?? "Null",
						                              string.Join(",", row),
						                              m_executeTarget.TargetFilePath ?? "Null",
						                              m_executeTarget.ExecutedTime,
						                              lineNumber,
						                              m_executeTarget.TargetFilePath ?? "Null"));

						// 行データをもとにApiEntry生成
						ApiEntry apiEntry = new ApiEntry() {Data = row };

						// コマンドビルダー実行
						base.m_commandBuilder.Do(apiEntry);

					}
					catch(Exception ex)
					{
						// エラーをCatchしログを書く
						// 上位にはthrowしない
						w2.Common.Logger.FileLogger.Write("ImportStock", "エラーをCatch");

						ApiLogger.Write(LogLevel.error, 
							string.Format("{0}行目出力でエラー発生、エラーメッセージ'{1}'",lineNumber.ToString(),ex.Message),
							string.Format("読み込みデータ内容:'{0}'", (row == null) ? "Null" : string.Join("','", row)),
							ex);

						result = false;
					}
					finally
					{
						row = csvReader.ReadLine();
						lineNumber++;
						w2.Common.Logger.FileLogger.Write("ImportStock", "finally row：" + row);
						w2.Common.Logger.FileLogger.Write("ImportStock", "finally lineNumber：" + lineNumber);
					}
				}
			}

			w2.Common.Logger.FileLogger.Write("ImportStock", "Execute END");

			return result;
		}

		/// <summary>
		/// 実行後処理の実装
		/// </summary>
		/// <param name="resultExecute">Execute実行結果（成功：true、失敗：false）</param>
		protected override void PostExecute(bool resultExecute)
		{
			// コマンドビルダー実行後処理
			m_commandBuilder.PostDo();

			// ワークファイルをサクセス or エラー移動
			File.Move(base.m_executeTarget.WorkFilePath,
				(resultExecute ? base.m_executeTarget.SuccessPath : base.m_executeTarget.ErrorPath));
		}

		#endregion
	}
}
