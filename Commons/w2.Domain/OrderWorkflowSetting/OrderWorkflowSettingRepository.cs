/*
=========================================================================================================
  Module      : 注文ワークフロー設定リポジトリ (OrderWorkflowSettingRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.OrderWorkflowSetting
{
	/// <summary>
	/// 注文ワークフロー設定リポジトリ
	/// </summary>
	internal class OrderWorkflowSettingRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "OrderWorkflowSetting";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal OrderWorkflowSettingRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		internal OrderWorkflowSettingRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="workflowKbn">ワークフロー区分</param>
		/// <param name="workflowNo">枝番</param>
		/// <returns>モデル</returns>
		internal OrderWorkflowSettingModel Get(string shopId, string workflowKbn, int workflowNo)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDERWORKFLOWSETTING_SHOP_ID, shopId},
				{Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_KBN, workflowKbn},
				{Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_NO, workflowNo},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new OrderWorkflowSettingModel(dv[0]);
		}

		/// <summary>
		/// 定期ワークフロー取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="workflowKbn">ワークフロー区分</param>
		/// <param name="workflowNo">枝番</param>
		/// <returns>モデル</returns>
		internal OrderWorkflowSettingModel GetFixedPurchaseWorkflow(string shopId, string workflowKbn, int workflowNo)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDERWORKFLOWSETTING_SHOP_ID, shopId},
				{Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_KBN, workflowKbn},
				{Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_NO, workflowNo},
			};
			var dv = Get(XML_KEY_NAME, "GetFixedPurcase", ht);
			if (dv.Count == 0) return null;
			return new OrderWorkflowSettingModel(dv[0]);
		}
		#endregion

		#region ~GetTotalOrderWorkflowSettingCount 注文ワークフロー設定の総数を取得
		/// <summary>
		/// 注文ワークフロー設定の総件数を取得する
		/// </summary>
		/// <returns>注文ワークフロー設定の総件数</returns>
		internal int GetTotalOrderWorkflowSettingCount()
		{
			var dv = Get(XML_KEY_NAME, "GetTotalOrderWorkflowSettingCount");
			var count = (int)dv[0][0];
			return count;
		}
		#endregion

		#region ~GetForScenarioRegistration シナリオ登録(編集)様のGet
		/// <summary>
		/// シナリオ登録(編集)様のGet
		/// </summary>
		/// <returns>モデル(複数)</returns>
		internal OrderWorkflowSettingModel[] GetForScenarioRegistration()
		{
			var dv = Get(XML_KEY_NAME, "GetForScenarioRegistration");
			return dv.Cast<DataRowView>().Select(drv => new OrderWorkflowSettingModel(drv)).ToArray();
		}
		#endregion

		#region ~GetOrderWorkflowSettingInDataView 受注ワークフローの設定を取得 HACK: 例外的にDataViwで取得
		/// <summary>
		/// 受注ワークフローの設定を取得する HACK: 例外的にDataViewを返す
		/// </summary>
		/// <param name="shopId">ショップID</param>
		/// <param name="workFlowKbn">ワークフロー区分</param>
		/// <param name="workFlowNo">ワークフローNo</param>
		/// <returns>DataView</returns>
		internal DataView GetOrderWorkflowSettingInDataView(string shopId, string workFlowKbn, string workFlowNo)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_ORDERWORKFLOWSETTING_SHOP_ID, shopId },
				{ Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_KBN, workFlowKbn },
				{ Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_NO, workFlowNo }
			};
			var dv = Get(XML_KEY_NAME, "GetOrderWorkflowSetting", ht);
			return dv;
		}
		#endregion

		#region ~GetAll 全てGet
		/// <summary>
		/// 全てGet
		/// </summary>
		/// <returns>モデル(複数)</returns>
		internal OrderWorkflowSettingModel[] GetAll()
		{
			var dv = Get(XML_KEY_NAME, "GetAll");
			return dv.Cast<DataRowView>().Select(drv => new OrderWorkflowSettingModel(drv)).ToArray();
		}
		#endregion

		#region ~GetOrderWorkflowSettings
		/// <summary>
		/// Get order workflow settings
		/// </summary>
		/// <param name="workflowKbn">A workflow kbn</param>
		/// <param name="workflowName">A workflow name</param>
		/// <returns>A collection of order workflow setting models</returns>
		internal OrderWorkflowSettingModel[] GetOrderWorkflowSettings(
			string workflowKbn,
			string workflowName)
		{
			var dv = Get(
				XML_KEY_NAME,
				"GetOrderWorkflowSettings",
				new Hashtable
				{
					{ Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_KBN, workflowKbn },
					{ Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_NAME, workflowName }
				});
			var orderWorkflowSettings = dv.Cast<DataRowView>()
				.Select(drv => new OrderWorkflowSettingModel(drv))
				.ToArray();
			return orderWorkflowSettings;
		}
		#endregion

		#region ~GetOrderWorkflowSetting
		/// <summary>
		/// Get order workflow setting
		/// </summary>
		/// <param name="workflowKbn">A workflow kbn</param>
		/// <param name="workflowNo">A workflow no</param>
		/// <returns>A order workflow setting model</returns>
		internal OrderWorkflowSettingModel GetOrderWorkflowSetting(
			string workflowKbn,
			string workflowNo)
		{
			var dv = Get(
				XML_KEY_NAME,
				"GetOrderWorkflowSetting",
				new Hashtable
				{
					{ Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_KBN, workflowKbn },
					{ Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_NO, workflowNo }
				});
			var result = (dv.Count != 0)
				? new OrderWorkflowSettingModel(dv[0])
				: null;
			return result;
		}
		#endregion
	}
}
