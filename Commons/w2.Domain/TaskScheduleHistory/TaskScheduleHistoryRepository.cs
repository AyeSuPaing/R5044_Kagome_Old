/*
=========================================================================================================
  Module      : タスクスケジュール履歴リポジトリ (TaskScheduleHistoryRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using w2.Common.Sql;

namespace w2.Domain.TaskScheduleHistory
{
	/// <summary>
	/// タスクスケジュール履歴リポジトリ
	/// </summary>
	internal class TaskScheduleHistoryRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "TaskScheduleHistory";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal TaskScheduleHistoryRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		internal TaskScheduleHistoryRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}

		#endregion
		#region ~DeleteByBatch 削除(削除バッチ実行時)
		/// <summary>
		/// 削除(削除バッチ実行時)
		/// </summary>
		/// <param name="scheduleDate">対象スケジュール実行日</param>
		/// <param name="limitCount">マージ対象レコード上限数</param>
		/// <param name="timeOutSec">SQLタイムアウト秒数</param>
		/// <returns>影響を受けた件数</returns>
		internal int DeleteByBatch(DateTime scheduleDate, int limitCount, int? timeOutSec)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_TASKSCHEDULEHISTORY_SCHEDULE_DATE, scheduleDate },
				{ "limit_count", limitCount },
			};

			this.CommandTimeout = timeOutSec;
			var result = Exec(XML_KEY_NAME, "DeleteByBatch", ht);
			return result;
		}
		#endregion
	}
}
