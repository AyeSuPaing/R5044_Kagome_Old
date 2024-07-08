/*
=========================================================================================================
  Module      : タスクスケジュールログリポジトリクラス(TaskScheduleLogRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.TaskScheduleLog
{
	/// <summary>
	/// タスクスケジュールログリポジトリクラス
	/// </summary>
	public class TaskScheduleLogRepository : RepositoryBase
	{
		/// <returns>クエリ用XML</returns>
		private const string XML_KEY_NAME = "TaskScheduleLog";

		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal TaskScheduleLogRepository()
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal TaskScheduleLogRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}

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
			var ht = new Hashtable
			{
				{ Constants.FIELD_TASKSCHEDULELOG_DEPT_ID, deptId },
				{ Constants.FIELD_TASKSCHEDULELOG_ACTION_KBN, actionKbn },
				{ Constants.FIELD_TASKSCHEDULELOG_ACTION_MASTER_ID, actionMasterId },
				{ Constants.FIELD_TASKSCHEDULELOG_ACTION_NO, actionNo },
				{ Constants.FIELD_TASKSCHEDULELOG_MESSAGING_APP_KBN, messagingAppKbn }
			};
			var dv = Get(XML_KEY_NAME, "GetLogEachMessagingAppKbn", ht);
			return (dv.Count > 0)
				? dv.Cast<DataRowView>().Select(x => new TaskScheduleLogModel(x)).ToArray()
				: new TaskScheduleLogModel[0];
		}

		/// <summary>
		/// タスクスケジュールログ登録
		/// </summary>
		/// <param name="model">タスクスケジュールログ</param>
		/// <returns>処理件数</returns>
		public int InsertLog(TaskScheduleLogModel model)
		{
			var result = Exec(XML_KEY_NAME, "InsertLog", model.DataSource);
			return result;
		}
	}
}
