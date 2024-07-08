/*
=========================================================================================================
  Module      : 受注ワークフローシナリオ設定サービス (OrderWorkflowScenarioSettingService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Linq;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.FixedPurchaseWorkflowSetting;
using w2.Domain.OrderWorkflowSetting;
using w2.Domain.TaskSchedule;
using w2.Domain.TaskSchedule.Helper;

namespace w2.Domain.OrderWorkflowScenarioSetting
{
	/// <summary>
	/// 受注ワークフローシナリオ設定サービス
	/// </summary>
	public class OrderWorkflowScenarioSettingService : ServiceBase
	{
		#region +GetWithChild 取得
		/// <summary>
		/// 取得(子テーブルも取得)
		/// </summary>
		/// <param name="scenarioSettingId">シナリオ設定ID</param>
		/// <returns>モデル</returns>
		public OrderWorkflowScenarioSettingModel GetWithChild(string scenarioSettingId)
		{
			using (var repository = new OrderWorkflowScenarioSettingRepository())
			{
				var model = repository.Get(scenarioSettingId);
				if (model == null) return null;

				var items = repository.GetItems(scenarioSettingId);
				model.Items = items;
				return model;
			}
		}
		#endregion

		#region +GetByOrderworkflowSettingPrimaryKey 受注ワークフロー設定PKで取得
		/// <summary>
		/// 受注ワークフロー設定PKで取得
		/// </summary>
		/// <returns>モデル</returns>
		public OrderWorkflowScenarioSettingModel[] GetByOrderworkflowSettingPrimaryKey(
			string shopId,
			string workflowKbn,
			int workflowNo)
		{
			using (var repository = new OrderWorkflowScenarioSettingRepository())
			{
				var items = repository.GetByOrderworkflowSettingPrimaryKey(shopId, workflowKbn, workflowNo);
				var scenarios = items.Select(item => repository.Get(item.ScenarioSettingId))
					.Where(item => (item != null))
					.ToArray();
				return scenarios;
			}
		}
		#endregion

		#region +GetForDatails 取得
		/// <summary>
		/// 詳細情報取得
		/// </summary>
		/// <param name="scenarioSettingId">シナリオ設定ID</param>
		/// <returns>モデル</returns>
		public OrderWorkflowScenarioSettingModel GetForDatails(string scenarioSettingId)
		{
			using (var repository = new OrderWorkflowScenarioSettingRepository())
			{
				var model = repository.Get(scenarioSettingId);
				if (model == null) return null;
				model.Items = repository.GetItems(scenarioSettingId)
					.Select(
						item =>
						{
							if (item.TargetWorkflowKbn == Constants.FLG_ORDERWORKFLOWSCENARIOSETTING_TARGETKBN_NORMAL)
							{
								var orderWorkflowSetting = new OrderWorkflowSettingService().Get(
									item.ShopId,
									item.WorkflowKbn,
									item.WorkflowNo.Value);

								item.OrderWorkflowSetting = orderWorkflowSetting;
								item.WorkflowName = orderWorkflowSetting.WorkflowName;
								item.Desc1 = orderWorkflowSetting.Desc1;
							}
							else
							{
								var fixedPurchaseWorkflowSetting = new FixedPurchaseWorkflowSettingService().Get(
									item.ShopId,
									item.WorkflowKbn,
									item.WorkflowNo.Value);

								item.FixedPurchaseWorkflowSetting = fixedPurchaseWorkflowSetting;
								item.WorkflowName = fixedPurchaseWorkflowSetting.WorkflowName;
								item.Desc1 = fixedPurchaseWorkflowSetting.Desc1;
							}
							
							return item;
						})
					.ToArray();
				return model;
			}
		}
		#endregion

		#region +GetAll 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <returns>モデル</returns>
		public OrderWorkflowScenarioSettingModel[] GetAll()
		{
			using (var repository = new OrderWorkflowScenarioSettingRepository())
			{
				var models = repository.GetAll();
				return models;
			}
		}
		#endregion

		#region +Insert 登録

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">OrderWorkflowScenarioSettingModel</param>
		/// <param name="shopId">ショップID</param>
		public void Insert(OrderWorkflowScenarioSettingModel model, string shopId)
		{
			using (var accessor = new SqlAccessor())
			using (var repository = new OrderWorkflowScenarioSettingRepository(accessor))
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var scenarioSettingId = CreateNewScenarioSettingId(shopId, Constants.NUMBER_KEY_SCENARIO_SETTING_ID);
				model.ScenarioSettingId = scenarioSettingId;
				repository.Insert(model);
				foreach (var item in model.Items)
				{
					item.ScenarioSettingId = scenarioSettingId;
					repository.InsertItem(item);
				}

				accessor.CommitTransaction();
			}
		}
		#endregion

		#region +Update 更新

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="loginOperatorDeptId">ログインオペレーターデプトID</param>
		/// <param name="loginOperatorName">ログインオペレーター名</param>
		public int Update(OrderWorkflowScenarioSettingModel model, string loginOperatorDeptId, string loginOperatorName)
		{
			using (var accessor = new SqlAccessor())
			using (var repository = new OrderWorkflowScenarioSettingRepository(accessor))
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var updated = repository.Update(model);
				repository.DeleteItems(model.ScenarioSettingId);
				foreach (var item in model.Items)
				{
					repository.InsertItem(item);
				}

				// 未実行スケジュール削除
				new TaskScheduleService().DeleteTaskScheduleUnexecuted(
					loginOperatorDeptId,
					Constants.FLG_TASKSCHEDULE_ACTION_KBN_ORDERWORKFLOW_EXEC_SCENARIO_TO_SCHEDULE,
					model.ScenarioSettingId,
					accessor);

				// スケジュール追加
				if (CanInsertTaskSchedule(model)
				    && (model.ValidFlg == Constants.FLG_ORDERWORKFLOWSCENARIOSETTING_VALID_FLG_VALID)
				    && model.ExecTiming == Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_TIMING_SCHEDULE)
				{
					new TaskScheduleService().InsertFirstTaskSchedule(
						loginOperatorDeptId,
						Constants.FLG_TASKSCHEDULE_ACTION_KBN_ORDERWORKFLOW_EXEC_SCENARIO_TO_SCHEDULE,
						model.ScenarioSettingId,
						loginOperatorName,
						new TaskScheduleRule(model),
						accessor);
				}

				accessor.CommitTransaction();

				return updated;
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="scenarioSettingId">シナリオ設定ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Delete(string scenarioSettingId, SqlAccessor accessor = null)
		{
			using (var repository = new OrderWorkflowScenarioSettingRepository(accessor))
			{
				repository.Delete(scenarioSettingId);
				repository.DeleteItems(scenarioSettingId);
			}
		}
		#endregion

		#region +CreateNewScenarioSettingId 新しいシナリオIDを作成する
		/// <summary>
		/// 新しいシナリオIDを作成する
		/// </summary>
		/// <param name="shopId">ショップID</param>
		/// <param name="deptId">採番ID</param>
		/// <returns>scenarioSettingId</returns>
		public string CreateNewScenarioSettingId(string shopId, string deptId)
		{
			var scenarioSettingId = NumberingUtility.CreateNewNumber(shopId, deptId);
			return scenarioSettingId.ToString();
		}
		#endregion

		#region +CanInsertTaskSchedule タスクスケジュール作成判定（過去であれば作成したくない）
		/// <summary>
		/// タスクスケジュール作成判定（過去であれば作成したくない）
		/// </summary>
		/// <param name="scenarioSetting">受注ワークフローシナリオ設定モデル</param>
		/// <returns>判定結果</returns>
		private bool CanInsertTaskSchedule(OrderWorkflowScenarioSettingModel scenarioSetting)
		{
			if ((scenarioSetting.ExecTiming == Constants.FLG_POINTRULESCHEDULE_EXEC_TIMING_SCHEDULE)
			    && (scenarioSetting.ScheduleKbn == Constants.FLG_POINTRULESCHEDULE_SCHEDULE_KBN_ONCE))
			{
				var schedule = new DateTime(
					scenarioSetting.ScheduleYear.Value,
					scenarioSetting.ScheduleMonth.Value,
					scenarioSetting.ScheduleDay.Value,
					scenarioSetting.ScheduleHour.Value,
					scenarioSetting.ScheduleMinute.Value,
					scenarioSetting.ScheduleSecond.Value);

				return (DateTime.Now.CompareTo(schedule) <= 0);
			}
			return true;
		}
		#endregion
	}
}
