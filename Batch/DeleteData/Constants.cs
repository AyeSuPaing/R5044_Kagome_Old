/*
=========================================================================================================
  Module      : 定数定義(Constants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace w2.Commerce.Batch.DeleteData
{
	/// <summary>
	/// 定数定義
	/// </summary>
	class Constants : w2.App.Common.Constants
	{
		/// <summary>メール配信ログ保持期間の月数</summary>
		public static int MAILSENDLOG_RETENTION_PERIOD_MONTHS = 0;
		/// <summary>メール配信ログの削除対象する間隔(1度に何日何時間何分分削除するかの日) </summary>
		public static int DELETE_TARGET_INTERVAL_DAYS = 0;
		/// <summary>メール配信ログの削除対象する間隔(1度に何日何時間何分分削除するかの時間) </summary>
		public static int DELETE_TARGET_INTERVAL_HOUR = 0;
		/// <summary>メール配信ログの削除対象する間隔(1度に何日何時間何分分削除するかの分) </summary>
		public static int DELETE_TARGET_INTERVAL_MINUTE = 0;
		/// <summary> メール配信ログSQLタイムアウト時間(秒) </summary>
		public static string DELETE_DATA_SQL_TIMEOUT_SECOND = "";
		/// <summary> タスクスケジュール履歴保持期間の月数</summary>
		public static int TASK_SCHEDULE_HISTORY_RETENTION_PERIOD_MONTHS = 0;
		/// <summary> タスクスケジュール履歴削除SQLタイムアウト時間(秒) </summary>
		public static string DELETE_TASK_SCHEDULE_HISTORY_DATA_SQL_TIMEOUT_SECOND = "";
		/// <summary> タスクスケジュール履歴を一度のトランザクションで削除する最大レコード数</summary>
		public static int DELETE_TASK_SCHEDULE_HISTORY_DATA_MAX_COUNT_BY_TRANSACTION = 0;
		/// <summary> タスクスケジュール履歴削除時にログに出力するレコード件数</summary>
		public static int INTERVAL_RECORDS_OF_OUTPUT_LOGS_FOR_DELETE_TASK_SCHEDULE_HISTORY_DATA = 0;
	}
}