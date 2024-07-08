/*
=========================================================================================================
  Module      : プログラム本体(Program.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Logger;
using w2.App.Common.Util;
using w2.Commerce.Batch.DeleteData.Commands;

namespace w2.Commerce.Batch.DeleteData
{
	/// <summary>
	/// プログラム本体
	/// </summary>
	class Program
	{
		#region +メイン処理
		/// <summary>
		/// メイン処理
		/// </summary>
		public static void Main(string[] args)
		{
			try
			{
				var program = new Program();

				AppLogger.WriteInfo("起動");

				// 二重起動禁止
				bool isSuccess = ProcessUtility.ExcecWithProcessMutex(program.Start);
				if (isSuccess == false)
				{
					throw new Exception("他プロセスが起動しているため、起動に失敗しました。二重起動は禁止されています。");
				}

				AppLogger.WriteInfo("終了");
			}
			catch (Exception ex)
			{
				AppLogger.WriteError(ex);
			}
		}
		#endregion

		#region +コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Program()
		{
			// 初期化処理
			Initialize();
		}
		#endregion

		#region -初期化処理
		/// <summary>
		/// 初期化処理
		/// </summary>
		private static void Initialize()
		{
			try
			{
				// アプリケーション設定読み込み
				Constants.APPLICATION_NAME = Properties.Settings.Default.Application_Name;

				// アプリケーション共通の設定
				var appSetting
					= new w2.App.Common.ConfigurationSetting(
						Properties.Settings.Default.ConfigFileDirPath,
						w2.App.Common.ConfigurationSetting.ReadKbn.C000_AppCommon,
						w2.App.Common.ConfigurationSetting.ReadKbn.C100_BatchCommon,
						w2.App.Common.ConfigurationSetting.ReadKbn.C200_DeleteData);

				// アプリケーション固有の設定
				Constants.MAILSENDLOG_RETENTION_PERIOD_MONTHS = int.Parse(appSetting.GetAppStringSetting("MailSendLog_Retention_Period_Months"));
				Constants.DELETE_TARGET_INTERVAL_DAYS = int.Parse(appSetting.GetAppStringSetting("Delete_Target_Interval_Days"));
				Constants.DELETE_TARGET_INTERVAL_HOUR = int.Parse(appSetting.GetAppStringSetting("Delete_Target_Interval_Hour"));
				Constants.DELETE_TARGET_INTERVAL_MINUTE = int.Parse(appSetting.GetAppStringSetting("Delete_Target_Interval_Minute"));
				Constants.DELETE_DATA_SQL_TIMEOUT_SECOND = appSetting.GetAppStringSetting("Delete_TaskScheduleHistory_Data_Sql_Timeout_Second");
				Constants.TASK_SCHEDULE_HISTORY_RETENTION_PERIOD_MONTHS = int.Parse(appSetting.GetAppStringSetting("TaskScheduleHistory_Retention_Period_Months"));
				Constants.DELETE_TASK_SCHEDULE_HISTORY_DATA_SQL_TIMEOUT_SECOND = appSetting.GetAppStringSetting("Delete_TaskScheduleHistory_Data_Sql_Timeout_Second");
				Constants.DELETE_TASK_SCHEDULE_HISTORY_DATA_MAX_COUNT_BY_TRANSACTION = int.Parse(appSetting.GetAppStringSetting("Delete_TaskScheduleHistory_Data_Max_Count_By_Transaction"));
				Constants.INTERVAL_RECORDS_OF_OUTPUT_LOGS_FOR_DELETE_TASK_SCHEDULE_HISTORY_DATA
					= int.Parse(appSetting.GetAppStringSetting("Interval_Records_Of_Output_Logs_For_Delete_TaskScheduleHistory_Data"));
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Configファイルの読み込みに失敗しました。", ex);
			}
		}
		#endregion

		#region -プログラム開始
		/// <summary>
		/// プログラム開始
		/// </summary>
		private void Start()
		{
			// メール送信ログ削除
			Console.WriteLine("メール送信ログ削除を開始");
			new DeleteMailSendLogCommand().Exec();
			Console.WriteLine("メール送信ログ削除を終了");

			// 自動翻訳ワードの利用されていない古いデータを削除
			Console.WriteLine("自動翻訳ワードの利用されていない古いデータを削除を開始");
			new DeleteAutoTranslationOldWord().Exec();
			Console.WriteLine("自動翻訳ワードの利用されていない古いデータを削除を終了");

			// タスクスケジュール実行履歴を削除(集計して別テーブルに保存)
			Console.WriteLine("タスクスケジュール実行履歴の削除を開始");
			new DeleteTaskScheduleHistoryCommand().Exec();
			Console.WriteLine("タスクスケジュール実行履歴の削除を終了");
		}
		#endregion
	}
}