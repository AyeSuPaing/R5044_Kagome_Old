/*
=========================================================================================================
  Module      : タスクスケジュール履歴集計テーブルリポジトリ (TaskScheduleHistorySummaryRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using w2.Common.Sql;

namespace w2.Domain.TaskScheduleHistorySummary
{
	/// <summary>
	/// タスクスケジュール履歴集計テーブルリポジトリ
	/// </summary>
	internal class TaskScheduleHistorySummaryRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "TaskScheduleHistorySummary";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal TaskScheduleHistorySummaryRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		internal TaskScheduleHistorySummaryRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~MergeFromHistoryData タスクスケジュール実行履歴テーブルからデータをマージ
		/// <summary>
		/// タスクスケジュール実行履歴テーブルからデータをマージ
		/// </summary>
		/// <param name="scheduleDate">対象スケジュール実行日</param>
		/// <param name="limitCount">マージ対象レコード上限数</param>
		/// <param name="timeOutSec">SQLタイムアウト秒数</param>
		/// <returns>影響を受けた件数</returns>
		internal int MergeFromHistoryData(DateTime scheduleDate, int limitCount, int? timeOutSec)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_TASKSCHEDULEHISTORY_SCHEDULE_DATE, scheduleDate },
				{ "limit_count", limitCount },
			};
			this.CommandTimeout = timeOutSec;
			var result = Exec(XML_KEY_NAME, "MergeFromHistoryData", ht);
			return result;
		}
		#endregion
	}
}
