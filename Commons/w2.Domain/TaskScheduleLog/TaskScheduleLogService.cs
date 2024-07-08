/*
=========================================================================================================
  Module      : タスクスケジュールログサービスクラス(TaskScheduleLogService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.TaskScheduleLog
{
	/// <summary>
	/// タスクスケジュールログサービスクラス
	/// </summary>
	public class TaskScheduleLogService : ServiceBase
	{
		/// <summary>
		/// タスクスケジュールログ取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="actionKbn">実行区分</param>
		/// <param name="actionMasterId">実行マスタID</param>
		/// <param name="actionNo">実行履歴NO</param>
		/// <param name="messagingAppKbn">メッセージアプリ区分</param>
		/// <returns></returns>
		public TaskScheduleLogModel[] GetLogEachMessagingAppKbn(string deptId, string actionKbn, string actionMasterId, int actionNo, string messagingAppKbn)
		{
			using (var repository = new TaskScheduleLogRepository())
			{
				var models = repository.GetLogEachMessagingAppKbn(deptId, actionKbn, actionMasterId, actionNo, messagingAppKbn);
				return models;
			}
		}

		/// <summary>
		/// タスクスケジュールログ登録
		/// </summary>
		/// <param name="model">タスクスケジュールログ</param>
		/// <returns>処理件数</returns>
		public int InsertLog(TaskScheduleLogModel model)
		{
			using (var repository = new TaskScheduleLogRepository())
			{
				var result = repository.InsertLog(model);
				return result;
			}
		}
	}
}
