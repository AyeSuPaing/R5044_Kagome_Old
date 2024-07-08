/*
=========================================================================================================
  Module      : 受注ワークフロー実行履歴サービスのインターフェース (IOrderWorkflowExecHistoryService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Sql;
using w2.Domain.OrderWorkflowExecHistory.Helper;

namespace w2.Domain.OrderWorkflowExecHistory
{
	/// <summary>
	/// 受注ワークフロー実行履歴サービスのインターフェース
	/// </summary>
	public interface IOrderWorkflowExecHistoryService : IService
	{
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		int GetSearchHitCount(OrderWorkflowExecHistoryListSearchCondition condition);

		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		OrderWorkflowExecHistoryModel[] Search(OrderWorkflowExecHistoryListSearchCondition condition);

		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="orderWorkflowExecHistoryId">受注ワークフロー実行履歴ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		OrderWorkflowExecHistoryModel Get(
			long orderWorkflowExecHistoryId,
			SqlAccessor accessor = null);

		/// <summary>
		/// シナリオIDで取得
		/// </summary>
		/// <param name="scenarioId">受注ワークフローシナリオ設定ID</param>
		/// <returns>モデル</returns>
		OrderWorkflowExecHistoryModel[] GetByScenarioId(string scenarioId);

		/// <summary>
		/// 実行ステータスで取得
		/// </summary>
		/// <param name="execStatus">実行ステータス</param>
		/// <returns>モデル</returns>
		OrderWorkflowExecHistoryModel GetByExecStatus(string execStatus);

		/// <summary>
		/// 実行ステータスがRunningかHoldのデータを持つか
		/// </summary>
		/// <returns>実行ステータスがRunningかHoldのデータを持っていればtrue</returns>
		bool HasExecStatusForRunningOrHold();

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		OrderWorkflowExecHistoryModel Insert(
			OrderWorkflowExecHistoryModel model,
			SqlAccessor accessor = null);

		/// <summary>
		/// 更新（汎用的に利用）
		/// </summary>
		/// <param name="orderWorkflowExecHistoryId">受注ワークフロー実行履歴ID</param>
		/// <param name="updateAction">更新操作</param>
		/// <returns>影響を受けた件数</returns>
		int Modify(
			long orderWorkflowExecHistoryId,
			Action<OrderWorkflowExecHistoryModel> updateAction);

		/// <summary>
		/// 更新（汎用的に利用）
		/// </summary>
		/// <param name="orderWorkflowExecHistoryId">受注ワークフロー実行履歴ID</param>
		/// <param name="updateAction">更新操作</param>
		/// <returns>影響を受けた件数</returns>
		void ModifyForChangeFromRunningOrHoldToNg(
			long orderWorkflowExecHistoryId,
			Action<OrderWorkflowExecHistoryModel> updateAction);

		/// <summary>
		/// 更新した後、更新したレコードを取得（汎用的に利用）
		/// </summary>
		/// <param name="orderWorkflowExecHistoryId">受注ワークフロー実行履歴ID</param>
		/// <param name="updateAction">更新操作</param>
		/// <returns>影響を受けた件数</returns>
		OrderWorkflowExecHistoryModel ModifyAndGetUpdatedRecords(
			long orderWorkflowExecHistoryId,
			Action<OrderWorkflowExecHistoryModel> updateAction);

		/// <summary>
		/// 更新（汎用的に利用）
		/// </summary>
		/// <param name="orderWorkflowExecHistoryId">受注ワークフロー実行履歴ID</param>
		/// <param name="updateAction">更新操作</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		int Modify(
			long orderWorkflowExecHistoryId,
			Action<OrderWorkflowExecHistoryModel> updateAction,
			SqlAccessor accessor);

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		int Update(OrderWorkflowExecHistoryModel model, SqlAccessor accessor = null);

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="orderWorkflowExecHistoryId">受注ワークフロー実行履歴ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		int Delete(long orderWorkflowExecHistoryId, SqlAccessor accessor = null);
	}
}
