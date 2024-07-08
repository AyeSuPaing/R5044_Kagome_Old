/*
=========================================================================================================
  Module      : 受注ワークフロー実行履歴サービス (OrderWorkflowExecHistoryService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.Common.Sql;
using w2.Domain.OrderWorkflowExecHistory.Helper;
using w2.Domain.OrderWorkflowScenarioSetting;
using w2.Domain.OrderWorkflowSetting;


namespace w2.Domain.OrderWorkflowExecHistory
{
	/// <summary>
	/// 受注ワークフロー実行履歴サービス
	/// </summary>
	public class OrderWorkflowExecHistoryService : ServiceBase, IOrderWorkflowExecHistoryService
	{
		#region +GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		public int GetSearchHitCount(OrderWorkflowExecHistoryListSearchCondition condition)
		{
			using (var repository = new OrderWorkflowExecHistoryRepository())
			{
				var count = repository.GetSearchHitCount(condition);
				return count;
			}
		}
		#endregion

		#region +Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		public OrderWorkflowExecHistoryModel[] Search(OrderWorkflowExecHistoryListSearchCondition condition)
		{
			using (var repository = new OrderWorkflowExecHistoryRepository())
			{
				var execHistories = repository.Search(condition);
				return execHistories;
			}
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="orderWorkflowExecHistoryId">受注ワークフロー実行履歴ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public OrderWorkflowExecHistoryModel Get(long orderWorkflowExecHistoryId, SqlAccessor accessor = null)
		{
			using (var repository = new OrderWorkflowExecHistoryRepository(accessor))
			{
				var model = repository.Get(orderWorkflowExecHistoryId);
				return model;
			}
		}
		#endregion

		#region +GetByScenarioId シナリオIDで取得
		/// <summary>
		/// シナリオIDで取得
		/// </summary>
		/// <param name="scenarioId">受注ワークフローシナリオ設定ID</param>
		/// <returns>モデル</returns>
		public OrderWorkflowExecHistoryModel[] GetByScenarioId(string scenarioId)
		{
			using (var repository = new OrderWorkflowExecHistoryRepository())
			{
				var models = repository.GetByScenarioId(scenarioId);
				foreach (var model in models)
				{
					model.WorkflowName = new OrderWorkflowSettingService()
						.Get(model.ShopId, model.WorkflowKbn, model.WorkflowNo).WorkflowName;
					model.ScenarioName = new OrderWorkflowScenarioSettingService().GetWithChild(scenarioId).ScenarioName;
				}
				return models;
			}
		}
		#endregion

		#region +GetByExecStatus 実行ステータスで取得
		/// <summary>
		/// 実行ステータスで取得
		/// </summary>
		/// <param name="execStatus">実行ステータス</param>
		/// <returns>モデル</returns>
		public OrderWorkflowExecHistoryModel GetByExecStatus(string execStatus)
		{
			using (var repository = new OrderWorkflowExecHistoryRepository())
			{
				var model = repository.GetByExecStatus(execStatus);

				if (model == null) return model;

				switch (model.WorkflowType)
				{
					case Constants.FLG_ORDERWORKFLOWEXECHISTORY_WORKFLOW_TYPE_ORDER_WORKFLOW:
						model.WorkflowName
							= new OrderWorkflowSettingService().Get(model.ShopId, model.WorkflowKbn, model.WorkflowNo).WorkflowName;
						break;

					case Constants.FLG_ORDERWORKFLOWEXECHISTORY_WORKFLOW_TYPE_FIXED_PURCHASE_WORKFLOW:
						model.WorkflowName
							= new OrderWorkflowSettingService().GetFixedPurchaseWorkflow(model.ShopId, model.WorkflowKbn, model.WorkflowNo).WorkflowName;
						break;
				}
				return model;
			}
		}
		#endregion

		#region +HasExecStatusForRunningOrHold 実行ステータスがRunningかHoldのデータを持つか
		/// <summary>
		/// 実行ステータスがRunningかHoldのデータを持つか
		/// </summary>
		/// <returns>実行ステータスがRunningかHoldのデータを持っていればtrue</returns>
		public bool HasExecStatusForRunningOrHold()
		{
			using (var repository = new OrderWorkflowExecHistoryRepository())
			{
				var models = repository.GetByExecStatusForRunningOrHold();
				return models.Any();
			}
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public OrderWorkflowExecHistoryModel Insert(OrderWorkflowExecHistoryModel model, SqlAccessor accessor = null)
		{
			using (var repository = new OrderWorkflowExecHistoryRepository(accessor))
			{
				repository.Insert(model);
				var history = repository.GetByNewest();
				return history;
			}
		}
		#endregion

		#region +Modify (汎用的に利用)
		/// <summary>
		/// 更新（汎用的に利用）
		/// </summary>
		/// <param name="orderWorkflowExecHistoryId">受注ワークフロー実行履歴ID</param>
		/// <param name="updateAction">更新操作</param>
		/// <returns>影響を受けた件数</returns>
		public int Modify(long orderWorkflowExecHistoryId, Action<OrderWorkflowExecHistoryModel> updateAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var updated = Modify(orderWorkflowExecHistoryId, updateAction, accessor);

				accessor.CommitTransaction();
				return updated;
			}
		}
		#endregion

		#region +ModifyForChangeFromRunningToNg (汎用的に利用)
		/// <summary>
		/// 更新（汎用的に利用）
		/// </summary>
		/// <param name="orderWorkflowExecHistoryId">受注ワークフロー実行履歴ID</param>
		/// <param name="updateAction">更新操作</param>
		/// <returns>影響を受けた件数</returns>
		public void ModifyForChangeFromRunningOrHoldToNg(long orderWorkflowExecHistoryId, Action<OrderWorkflowExecHistoryModel> updateAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var history = Get(orderWorkflowExecHistoryId, accessor);
				if (history == null) return;

				if (history.ExecStatus == Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_RUNNING
					|| history.ExecStatus == Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_HOLD)
				{
					Modify(orderWorkflowExecHistoryId, updateAction, accessor);
				}

				accessor.CommitTransaction();
			}
		}
		#endregion

		#region +ModifyAndGetUpdatedRecords (汎用的に利用)
		/// <summary>
		/// 更新した後、更新したレコードを取得（汎用的に利用）
		/// </summary>
		/// <param name="orderWorkflowExecHistoryId">受注ワークフロー実行履歴ID</param>
		/// <param name="updateAction">更新操作</param>
		/// <returns>影響を受けた件数</returns>
		public OrderWorkflowExecHistoryModel ModifyAndGetUpdatedRecords(
			long orderWorkflowExecHistoryId,
			Action<OrderWorkflowExecHistoryModel> updateAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				Modify(orderWorkflowExecHistoryId, updateAction, accessor);
				var history = Get(orderWorkflowExecHistoryId, accessor);

				accessor.CommitTransaction();
				return history;
			}
		}
		#endregion

		#region +Modify (汎用的に利用)
		/// <summary>
		/// 更新（汎用的に利用）
		/// </summary>
		/// <param name="orderWorkflowExecHistoryId">受注ワークフロー実行履歴ID</param>
		/// <param name="updateAction">更新操作</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int Modify(long orderWorkflowExecHistoryId, Action<OrderWorkflowExecHistoryModel> updateAction, SqlAccessor accessor)
		{
			var model = Get(orderWorkflowExecHistoryId, accessor);
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
		/// <returns>影響を受けた件数</returns>
		public int Update(OrderWorkflowExecHistoryModel model, SqlAccessor accessor = null)
		{
			using (var repository = new OrderWorkflowExecHistoryRepository(accessor))
			{
				var result = repository.Update(model);
				return result;
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="orderWorkflowExecHistoryId">受注ワークフロー実行履歴ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int Delete(long orderWorkflowExecHistoryId, SqlAccessor accessor = null)
		{
			using (var repository = new OrderWorkflowExecHistoryRepository(accessor))
			{
				var result = repository.Delete(orderWorkflowExecHistoryId);
				return result;
			}
		}
		#endregion
	}
}