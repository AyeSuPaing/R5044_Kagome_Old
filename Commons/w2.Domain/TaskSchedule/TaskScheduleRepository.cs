/*
=========================================================================================================
  Module      : タスクスケジュールリポジトリ (TaskScheduleRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;
//using w2.Domain.TaskSchedule.Helper;

namespace w2.Domain.TaskSchedule
{
	/// <summary>
	/// タスクスケジュールリポジトリ
	/// </summary>
	public class TaskScheduleRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "TaskSchedule";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public TaskScheduleRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public TaskScheduleRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="actionKbn">実行区分</param>
		/// <param name="actionMasterId">実行マスタID</param>
		/// <param name="actionNo">実行履歴NO</param>
		/// <returns>モデル</returns>
		internal TaskScheduleModel Get(string deptId, string actionKbn, string actionMasterId, int actionNo)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_TASKSCHEDULE_DEPT_ID, deptId},
				{Constants.FIELD_TASKSCHEDULE_ACTION_KBN, actionKbn},
				{Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID, actionMasterId},
				{Constants.FIELD_TASKSCHEDULE_ACTION_NO, actionNo},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new TaskScheduleModel(dv[0]);
		}
		#endregion

		#region ~GetMasterId 取得(実行区分、実行マスタID)
		/// <summary>
		/// 取得(実行区分、実行マスタID)
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="actionKbn">実行区分</param>
		/// <param name="actionMasterId">実行マスタID</param>
		/// <returns>モデル</returns>
		internal TaskScheduleModel[] GetMasterId(string deptId, string actionKbn, string actionMasterId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_TASKSCHEDULE_DEPT_ID, deptId},
				{Constants.FIELD_TASKSCHEDULE_ACTION_KBN, actionKbn},
				{Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID, actionMasterId},
			};
			var dv = Get(XML_KEY_NAME, "GetMasterId", ht);
			return dv.Cast<DataRowView>().Select(drv => new TaskScheduleModel(drv)).ToArray();
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(TaskScheduleModel model)
		{
			Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="deptId">識別ID</param>
		/// <param name="actionKbn">実行区分</param>
		/// <param name="actionMasterId">実行マスタID</param>
		/// <param name="actionNo">実行履歴NO</param>
		public int Delete(string deptId, string actionKbn, string actionMasterId, int actionNo)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_TASKSCHEDULE_DEPT_ID, deptId},
				{Constants.FIELD_TASKSCHEDULE_ACTION_KBN, actionKbn},
				{Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID, actionMasterId},
				{Constants.FIELD_TASKSCHEDULE_ACTION_NO, actionNo},
			};
			var result = Exec(XML_KEY_NAME, "Delete", ht);
			return result;
		}
		#endregion

		#region ~Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int Update(TaskScheduleModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion

		#region +UpdateTaskStatusBegin タスクステータス更新
		/// <summary>
		/// タスクステータス更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int UpdateTaskStatusBegin(TaskScheduleModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpdateTaskStatusBegin", model.DataSource);
			return result;
		}
		#endregion

		#region ~UpdatePrepareTaskStatus 準備ステータス更新
		/// <summary>
		/// 準備ステータス更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int UpdatePrepareTaskStatus(TaskScheduleModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpdatePrepareTaskStatus", model.DataSource);
			return result;
		}
		#endregion

		#region +GetMaxActionNo
		/// <summary>
		/// 最大アクションNO取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="actionKbn">実行区分</param>
		/// <param name="actionMasterId">実行マスタID</param>
		/// <returns></returns>
		internal int GetMaxActionNo(string deptId, string actionKbn, string actionMasterId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_TASKSCHEDULE_DEPT_ID, deptId},
				{Constants.FIELD_TASKSCHEDULE_ACTION_KBN, actionKbn},
				{Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID, actionMasterId},
			};
			var result = Get(XML_KEY_NAME, "GetMaxActionNo", ht);
			return (int)result[0][0];
		}
		#endregion
	}
}
