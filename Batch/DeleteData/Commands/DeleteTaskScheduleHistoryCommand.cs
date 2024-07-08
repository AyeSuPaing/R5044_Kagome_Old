/*
=========================================================================================================
  Module      : タスクスケジュール実行履歴削除コマンド(DeleteTaskScheduleHistoryCommand.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Domain.TaskScheduleHistory;
using w2.Domain.TaskScheduleHistorySummary;

namespace w2.Commerce.Batch.DeleteData.Commands
{
	/// <summary>
	/// タスクスケジュール実行履歴削除コマンド
	/// </summary>
	public class DeleteTaskScheduleHistoryCommand
	{
		#region +実行
		/// <summary>
		/// 実行
		/// </summary>
		public void Exec()
		{
			// データ保存期限日付を取得
			var targetDate = DateTime.Now.AddMonths(Constants.TASK_SCHEDULE_HISTORY_RETENTION_PERIOD_MONTHS * -1);

			// SQLタイムアウト秒を取得(ブランクの場合、Nullとし指定なし)
			int parsedTimeOutSec;
			var timeOutSec = int.TryParse(Constants.DELETE_TASK_SCHEDULE_HISTORY_DATA_SQL_TIMEOUT_SECOND, out parsedTimeOutSec)
				? (int?)parsedTimeOutSec
				: null;

			// データ保存期限日付以前のデータを削除
			// ※一括で削除するとタイムアウトエラーが発生するため、一度のトランザクションでは設定値で指定されたレコード数を削除する
			var deleteDataCount = -1;
			var deleteDataCountForOutPutLog = 0;
			do
			{
				using (var accessor = new SqlAccessor())
				{
					accessor.OpenConnection();
					accessor.BeginTransaction();

					new TaskScheduleHistorySummaryService().MergeFromHistoryData(
						targetDate,
						Constants.DELETE_TASK_SCHEDULE_HISTORY_DATA_MAX_COUNT_BY_TRANSACTION,
						timeOutSec,
						accessor);
					deleteDataCount = new TaskScheduleHistoryService().DeleteByBatch(
						targetDate,
						Constants.DELETE_TASK_SCHEDULE_HISTORY_DATA_MAX_COUNT_BY_TRANSACTION,
						timeOutSec,
						accessor);

					accessor.CommitTransaction();
				}

				deleteDataCountForOutPutLog += deleteDataCount;

				if (deleteDataCountForOutPutLog
					>= Constants.INTERVAL_RECORDS_OF_OUTPUT_LOGS_FOR_DELETE_TASK_SCHEDULE_HISTORY_DATA)
				{
					AppLogger.WriteInfo(string.Format(
						"TaskScheduleHistory Deleted：delete_count「{0}」", deleteDataCountForOutPutLog));
					deleteDataCountForOutPutLog = 0;
				}
			} while (deleteDataCount != 0);

			AppLogger.WriteInfo(string.Format(
				"TaskScheduleHistory Deleted：delete_count「{0}」", deleteDataCountForOutPutLog));
		}
		#endregion
	}
}