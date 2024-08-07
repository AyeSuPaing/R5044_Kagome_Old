/*
=========================================================================================================
  Module      : タスクスケジュール履歴集計テーブルサービス (TaskScheduleHistorySummaryService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.Common.Sql;

namespace w2.Domain.TaskScheduleHistorySummary
{
	/// <summary>
	/// タスクスケジュール履歴集計テーブルサービス
	/// </summary>
	public class TaskScheduleHistorySummaryService : ServiceBase
	{
		#region +MergeFromHistoryData タスクスケジュール実行履歴テーブルからデータをマージ
		/// <summary>
		/// タスクスケジュール実行履歴テーブルからデータをマージ
		/// </summary>
		/// <param name="scheduleDate">対象スケジュール実行日</param>
		/// <param name="limitCount">マージ対象レコード上限数</param>
		/// <param name="timeOutSec">SQLタイムアウト秒数</param>
		/// <param name="accessor">Sqlアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int MergeFromHistoryData(
			DateTime scheduleDate,
			int limitCount,
			int? timeOutSec,
			SqlAccessor accessor = null)
		{
			using (var repository = new TaskScheduleHistorySummaryRepository(accessor))
			{
				var result = repository.MergeFromHistoryData(scheduleDate, limitCount, timeOutSec);
				return result;
			}
		}
		#endregion
	}
}