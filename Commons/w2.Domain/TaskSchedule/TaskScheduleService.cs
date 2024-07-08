/*
=========================================================================================================
  Module      : タスクスケジュールサービス (TaskScheduleService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using System.Transactions;
using w2.Common.Sql;
using w2.Domain.TaskSchedule.Helper;

namespace w2.Domain.TaskSchedule
{
	/// <summary>
	/// タスクスケジュールサービス
	/// </summary>
	public class TaskScheduleService : ServiceBase
	{
		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="actionKbn">実行区分</param>
		/// <param name="actionMasterId">実行マスタID</param>
		/// <param name="actionNo">実行履歴NO</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public TaskScheduleModel Get(string deptId, string actionKbn, string actionMasterId, int actionNo, SqlAccessor accessor = null)
		{
			using (var repository = new TaskScheduleRepository(accessor))
			{
				var model = repository.Get(deptId, actionKbn, actionMasterId, actionNo);
				return model;
			}
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(TaskScheduleModel model)
		{
			using (var repository = new TaskScheduleRepository())
			{
				repository.Insert(model);
			}
		}
		#endregion

		#region +Modify (汎用的に利用)
		/// <summary>
		/// 更新（汎用的に利用）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="actionKbn">実行区分</param>
		/// <param name="actionMasterId">実行マスタID</param>
		/// <param name="actionNo">実行履歴NO</param>
		/// <param name="updateAction">更新操作</param>
		/// <returns>影響を受けた件数</returns>
		public int Modify(string deptId, string actionKbn, string actionMasterId, int actionNo, Action<TaskScheduleModel> updateAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var updated = Modify(deptId, actionKbn, actionMasterId, actionNo, updateAction, accessor);

				accessor.CommitTransaction();
				return updated;
			}
		}
		#endregion

		#region +Modify (汎用的に利用)
		/// <summary>
		/// 更新（汎用的に利用）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="actionKbn">実行区分</param>
		/// <param name="actionMasterId">実行マスタID</param>
		/// <param name="actionNo">実行履歴NO</param>
		/// <param name="updateAction">更新操作</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int Modify(string deptId, string actionKbn, string actionMasterId, int actionNo, Action<TaskScheduleModel> updateAction, SqlAccessor accessor)
		{
			var model = Get(deptId, actionKbn, actionMasterId, actionNo, accessor);

			updateAction(model);

			var updated = Update(model, accessor);

			return updated;
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public int Update(TaskScheduleModel model, SqlAccessor accessor = null)
		{
			using (var repository = new TaskScheduleRepository(accessor))
			{
				var result = repository.Update(model);
				return result;
			}
		}
		#endregion

		#region +UpdateTaskStatusBegin タスクステータス更新
		/// <summary>
		/// タスクステータス更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int UpdateTaskStatusBegin(TaskScheduleModel model, SqlAccessor accessor = null)
		{
			using (var repository = new TaskScheduleRepository(accessor))
			{
				var result = repository.UpdateTaskStatusBegin(model);
				return result;
			}
		}
		#endregion

		#region +UpdatePrepareTaskStatus 準備ステータス更新
		/// <summary>
		/// 準備ステータス更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int UpdatePrepareTaskStatus(TaskScheduleModel model, SqlAccessor accessor = null)
		{
			using (var repository = new TaskScheduleRepository(accessor))
			{
				var result = repository.UpdatePrepareTaskStatus(model);
				return result;
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="actionKbn">実行区分</param>
		/// <param name="actionMasterId">実行マスタID</param>
		/// <param name="actionNo">実行履歴NO</param>
		public void Delete(string deptId, string actionKbn, string actionMasterId, int actionNo)
		{
			using (var repository = new TaskScheduleRepository())
			{
				var result = repository.Delete(deptId, actionKbn, actionMasterId, actionNo);
				//return result;
			}
		}
		#endregion

		#region +DeleteMasterId 削除(実行区分、実行マスタID)
		/// <summary>
		/// 削除(実行区分、実行マスタID)
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="actionKbn">実行区分</param>
		/// <param name="actionMasterId">実行マスタID</param>
		public void DeleteMasterId(string deptId, string actionKbn, string actionMasterId)
		{
			using (var repository = new TaskScheduleRepository())
			{
				var models = repository.GetMasterId(deptId, actionKbn, actionMasterId);

				foreach (var model in models)
				{
					repository.Delete(model.DeptId, model.ActionKbn, model.ActionMasterId, model.ActionNo);
				}
			}
		}
		#endregion

		#region +DeleteTaskScheduleUnexecuted 削除(未実行タスクスケジュール)
		/// <summary>
		/// 削除(未実行タスクスケジュール)
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="actionKbn">実行区分</param>
		/// <param name="actionMasterId">実行マスタID</param>
		/// <param name="accessor">アクセッサ</param>
		public void DeleteTaskScheduleUnexecuted(string deptId, string actionKbn, string actionMasterId, SqlAccessor accessor = null)
		{
			using (var repository = new TaskScheduleRepository(accessor))
			{
				var models = repository.GetMasterId(deptId, actionKbn, actionMasterId);

				foreach (var model in models.Where(i => i.ExecuteStatus == Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_INIT))
				{
					repository.Delete(model.DeptId, model.ActionKbn, model.ActionMasterId, model.ActionNo);
				}
			}
		}
		#endregion

		#region +InsertFirstTaskSchedule 初回タスクスケジュール登録
		/// <summary>
		/// 初回タスクスケジュール登録
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="actionKbn">実行区分</param>
		/// <param name="actionMasterId">実行マスタID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="taskScheduleRule">タスクスケジュールルール</param>
		/// <param name="accessor">アクセッサ</param>
		public void InsertFirstTaskSchedule(string deptId, string actionKbn, string actionMasterId, string lastChanged, TaskScheduleRule taskScheduleRule, SqlAccessor accessor = null)
		{
			// スケジュール実行ではない場合抜ける
			if (taskScheduleRule.IsSchedule == false) return;

			var nowDate = DateTime.Now;
			DateTime scheduleDate;
			// 毎日？
			if (taskScheduleRule.IsEveryDay)
			{
				scheduleDate = new DateTime(nowDate.Year,
					nowDate.Month,
					nowDate.Day,
					taskScheduleRule.ScheduleHour ?? 0,
					taskScheduleRule.ScheduleMinute ?? 0,
					taskScheduleRule.ScheduleSecond ?? 0);

				// 過去なら翌日
				if (scheduleDate < nowDate) scheduleDate = scheduleDate.AddDays(1);
			}
			// 毎週？
			else if (taskScheduleRule.IsEveryWeek)
			{
				scheduleDate = new DateTime(nowDate.Year,
					nowDate.Month,
					nowDate.Day,
					taskScheduleRule.ScheduleHour ?? 0,
					taskScheduleRule.ScheduleMinute ?? 0,
					taskScheduleRule.ScheduleSecond ?? 0);
				DayOfWeek tmpDayOfWeek;
				if (Enum.TryParse<DayOfWeek>(taskScheduleRule.ScheduleDayOfWeek, out tmpDayOfWeek))
				{
					var tmpDayOfWeekValue = (int)tmpDayOfWeek;
					var scheduleDayOfWeekValue = (int)scheduleDate.DayOfWeek;
					scheduleDate = scheduleDate.AddDays(-1 * scheduleDayOfWeekValue).AddDays(tmpDayOfWeekValue);
				}

				// 過去なら翌週
				if (scheduleDate < nowDate) scheduleDate = scheduleDate.AddDays(7);
			}
			// 毎月？
			else if (taskScheduleRule.IsEveryMonth)
			{
				// 登録時の月で設定した日付が存在しない場合は初回を作成しない
				if (taskScheduleRule.CheckCanCreateFirstTaskScheduleForScheduleMonth(nowDate) == false) return;

				scheduleDate = new DateTime(nowDate.Year,
					nowDate.Month,
					taskScheduleRule.ScheduleDay ?? 0,
					taskScheduleRule.ScheduleHour ?? 0,
					taskScheduleRule.ScheduleMinute ?? 0,
					taskScheduleRule.ScheduleSecond ?? 0);

				// 過去なら翌月
				if (scheduleDate < nowDate) scheduleDate = scheduleDate.AddMonths(1);
			}
			// 一度だけ？
			else if (taskScheduleRule.IsOnce)
			{
				scheduleDate = new DateTime(taskScheduleRule.ScheduleYear ?? 0,
					taskScheduleRule.ScheduleMonth ?? 0,
					taskScheduleRule.ScheduleDay ?? 0,
					taskScheduleRule.ScheduleHour ?? 0,
					taskScheduleRule.ScheduleMinute ?? 0,
					taskScheduleRule.ScheduleSecond ?? 0);
			}
			else
			{
				// ここに来ることはないはず
				scheduleDate = nowDate;
			}

			using (var repository = new TaskScheduleRepository(accessor))
			{
				// 最大アクションNO取得
				var maxActionNo = repository.GetMaxActionNo(deptId, actionKbn, actionMasterId);

				var model = new TaskScheduleModel
				{
					ScheduleDate = scheduleDate,
					DeptId = deptId,
					ActionKbn = actionKbn,
					ActionMasterId = actionMasterId,
					ActionNo = maxActionNo + 1,
					ExecuteStatus = Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_INIT,
					LastChanged = lastChanged,
				};

				// 登録
				repository.Insert(model);
			}
		}
		#endregion

		#region +InsertTaskScheduleForExecute 即時実行用タスクスケジュール登録
		/// <summary>
		/// 即時実行用タスクスケジュール登録
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="actionKbn">実行区分</param>
		/// <param name="actionMasterId">実行マスタID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns></returns>
		public TaskScheduleModel InsertTaskScheduleForExecute(string deptId, string actionKbn, string actionMasterId, string lastChanged)
		{
			using (var repository = new TaskScheduleRepository())
			{
				// 最大アクションNO取得
				var maxActionNo = repository.GetMaxActionNo(deptId, actionKbn, actionMasterId);

				var model = new TaskScheduleModel
				{
					ScheduleDate = DateTime.Now,
					DeptId = deptId,
					ActionKbn = actionKbn,
					ActionMasterId = actionMasterId,
					ActionNo = maxActionNo + 1,
					ExecuteStatus = Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_INIT,
					LastChanged = lastChanged,
				};

				// 登録
				repository.Insert(model);

				var result = repository.Get(deptId, actionKbn, actionMasterId, model.ActionNo);

				return result;
			}
		}
		#endregion

		#region +SetTaskScheduleStopped タスクスケジュール停止
		/// <summary>
		/// タスクスケジュール停止
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="actionKbn">実行区分</param>
		/// <param name="actionMasterId">実行マスタID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">アクセッサ</param>
		public void SetTaskScheduleStopped(string deptId, string actionKbn, string actionMasterId, string lastChanged, SqlAccessor accessor = null)
		{
			using (var repository = new TaskScheduleRepository(accessor))
			{
				var models = repository.GetMasterId(deptId, actionKbn, actionMasterId);

				foreach (var model in models.Where(i => (i.ScheduleDate <= DateTime.Now) && (i.ExecuteStatus != Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_DONE)))
				{
					model.StopFlg = "1";
					repository.Update(model);
				}
			}
		}
		#endregion

		#region +SetTaskScheduleStoppedByActionNo アクションNoも指定してタスクスケジュール停止
		/// <summary>
		/// アクションNoも指定してタスクスケジュール停止
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="actionKbn">実行区分</param>
		/// <param name="actionMasterId">実行マスタID</param>
		/// <param name="actionNo">アクションNo</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">アクセッサ</param>
		public void SetTaskScheduleStoppedByPrimarykey(
			string deptId,
			string actionKbn,
			string actionMasterId,
			int actionNo,
			string lastChanged,
			SqlAccessor accessor = null)
		{
			using (var repository = new TaskScheduleRepository(accessor))
			{
				var model = repository.Get(deptId, actionKbn, actionMasterId, actionNo);
				model.StopFlg = Constants.FLG_TASKSCHEDULE_STOP_FLG_ON;
				repository.Update(model);
			}
		}
		#endregion

		#region +GetTaskScheduleAndUpdateProgress 進捗更新&スクスケジュール取得
		/// <summary>
		/// 進捗更新&amp;タスクスケジュール取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="actionKbn">実行区分</param>
		/// <param name="actionMasterId">実行マスタID</param>
		/// <param name="actionNo">実行履歴NO</param>
		/// <param name="progress">進捗</param>
		/// <returns>モデル</returns>
		public TaskScheduleModel GetTaskScheduleAndUpdateProgress(
			string deptId,
			string actionKbn,
			string actionMasterId,
			int actionNo,
			string progress)
		{
			using (var repository = new TaskScheduleRepository())
			{
				if (string.IsNullOrEmpty(progress) == false)
				{
					var model = repository.Get(deptId, actionKbn, actionMasterId, actionNo);
					model.Progress = progress;

					repository.Update(model);
				}

				var result = repository.Get(deptId, actionKbn, actionMasterId, actionNo);

				return result;
			}
		}
		#endregion
	}
}
