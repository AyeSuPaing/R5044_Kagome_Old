/*
=========================================================================================================
  Module      : タスクスケジュール履歴サービス (TaskScheduleHistoryService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Sql;

namespace w2.Domain.TaskScheduleHistory
{
	/// <summary>
	/// タスクスケジュール履歴サービス
	/// </summary>
	public class TaskScheduleHistoryService : ServiceBase
	{
		#region +DeleteByBatch 削除(削除バッチ実行時)
		/// <summary>
		/// 削除(削除バッチ実行時)
		/// </summary>
		/// <param name="scheduleDate">対象スケジュール実行日</param>
		/// <param name="limitCount">マージ対象レコード上限数</param>
		/// <param name="timeOutSec">SQLタイムアウト秒数</param>
		/// <param name="accessor">Sqlアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int DeleteByBatch(
			DateTime scheduleDate,
			int limitCount,
			int? timeOutSec,
			SqlAccessor accessor = null)
		{
			using (var repository = new TaskScheduleHistoryRepository(accessor))
			{
				var result = repository.DeleteByBatch(scheduleDate, limitCount, timeOutSec);
				return result;
			}
		}
		#endregion
	}
}