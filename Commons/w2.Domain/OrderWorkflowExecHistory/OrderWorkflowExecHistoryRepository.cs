/*
=========================================================================================================
  Module      : 受注ワークフロー実行履歴リポジトリ (OrderWorkflowExecHistoryRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;
using w2.Domain.OrderWorkflowExecHistory.Helper;

namespace w2.Domain.OrderWorkflowExecHistory
{
	/// <summary>
	/// 受注ワークフロー実行履歴リポジトリ
	/// </summary>
	internal class OrderWorkflowExecHistoryRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "OrderWorkflowExecHistory";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal OrderWorkflowExecHistoryRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		internal OrderWorkflowExecHistoryRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		internal int GetSearchHitCount(OrderWorkflowExecHistoryListSearchCondition condition)
		{
			var replace = GetSearchReplace(condition);

			var dv = Get(XML_KEY_NAME, "GetSearchHitCount", condition.CreateHashtableParams(), replaces: replace);
			return (int)dv[0][0];
		}
		#endregion

		#region ~Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		internal OrderWorkflowExecHistoryModel[] Search(OrderWorkflowExecHistoryListSearchCondition condition)
		{
			var replace = GetSearchReplace(condition);

			var dv = Get(XML_KEY_NAME, "Search", condition.CreateHashtableParams(), replaces: replace);
			return dv.Cast<DataRowView>().Select(drv => new OrderWorkflowExecHistoryModel(drv)).ToArray();
		}
		#endregion

		#region ~Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="orderWorkflowExecHistoryId">受注ワークフロー実行履歴ID</param>
		/// <returns>モデル</returns>
		internal OrderWorkflowExecHistoryModel Get(long orderWorkflowExecHistoryId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDERWORKFLOWEXECHISTORY_ORDER_WORKFLOW_EXEC_HISTORY_ID, orderWorkflowExecHistoryId},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new OrderWorkflowExecHistoryModel(dv[0]);
		}
		#endregion

		#region ~GetByScenarioId 取得

		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="scenarioId">受注ワークフローシナリオ設定ID</param>
		/// <returns>モデル</returns>
		internal OrderWorkflowExecHistoryModel[] GetByScenarioId(string scenarioId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDERWORKFLOWEXECHISTORY_SCENARIO_SETTING_ID, scenarioId},
			};
			var dv = Get(XML_KEY_NAME, "GetByScenarioId", ht);
			return dv.Cast<DataRowView>().Select(drv => new OrderWorkflowExecHistoryModel(drv)).ToArray();
		}
		#endregion

		#region ~GetByExecStatus 実行ステータスで取得
		/// <summary>
		/// 実行ステータスで取得
		/// </summary>
		/// <param name="execStatus">実行ステータス</param>
		/// <returns>モデル</returns>
		internal OrderWorkflowExecHistoryModel GetByExecStatus(string execStatus)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS, execStatus},
			};
			var dv = Get(XML_KEY_NAME, "GetByExecStatus", ht);
			if (dv.Count == 0) return null;
			return new OrderWorkflowExecHistoryModel(dv[0]);
		}
		#endregion

		#region ~GetByExecStatusForRunningOrHold 実行ステータスのRunningかHoldのデータを取得
		/// <summary>
		/// 実行ステータスのRunningかHoldのデータを取得
		/// </summary>
		/// <returns>モデル</returns>
		internal OrderWorkflowExecHistoryModel[] GetByExecStatusForRunningOrHold()
		{
			var dv = Get(XML_KEY_NAME, "GetByExecStatusForRunningOrHold");
			return dv.Cast<DataRowView>().Select(drv => new OrderWorkflowExecHistoryModel(drv)).ToArray();
		}
		#endregion

		#region ~GetByNewest 一番新しいレコードを取得
		/// <summary>
		/// 一番新しいレコードを取得
		/// </summary>
		/// <returns>モデル</returns>
		internal OrderWorkflowExecHistoryModel GetByNewest()
		{
			var dv = Get(XML_KEY_NAME, "GetByNewest");
			if (dv.Count == 0) return null;
			return new OrderWorkflowExecHistoryModel(dv[0]);
		}
		#endregion

		#region ~Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void Insert(OrderWorkflowExecHistoryModel model)
		{
			Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}
		#endregion

		#region ~Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int Update(OrderWorkflowExecHistoryModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion

		#region ~Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="orderWorkflowExecHistoryId">受注ワークフロー実行履歴ID</param>
		/// <returns>影響を受けた件数</returns>
		internal int Delete(long orderWorkflowExecHistoryId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDERWORKFLOWEXECHISTORY_ORDER_WORKFLOW_EXEC_HISTORY_ID, orderWorkflowExecHistoryId},
			};
			var result = Exec(XML_KEY_NAME, "Delete", ht);
			return result;
		}
		#endregion

		#region ~GetSearchReplace 検索用のreplaceを取得
		/// <summary>
		/// 検索用のreplaceを取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>リプレイス</returns>
		private KeyValuePair<string, string>[] GetSearchReplace(OrderWorkflowExecHistoryListSearchCondition condition)
		{
			var execStatus = condition.ExecStatus.Split(',');
			var execPlace = condition.ExecPlace.Split(',');
			var execTiming = condition.ExecTiming.Split(',');
			var workflowType = condition.WorkflowType.Split(',');

			var replace = new[]
			{
				new KeyValuePair<string, string>(
					"@@ exec_status @@",
					string.Join(", ", execStatus.Select(status => string.Format("'{0}'", status)))),
				new KeyValuePair<string, string>(
					"@@ exec_place @@",
					string.Join(", ", execPlace.Select(place => string.Format("'{0}'", place)))),
				new KeyValuePair<string, string>(
					"@@ exec_timing @@",
					string.Join(", ", execTiming.Select(timing => string.Format("'{0}'", timing)))),
				new KeyValuePair<string, string>(
					"@@ workflow_type @@",
					string.Join(", ", workflowType.Select(type => string.Format("'{0}'", type)))),
			};

			return replace;
		}
		#endregion
	}
}
