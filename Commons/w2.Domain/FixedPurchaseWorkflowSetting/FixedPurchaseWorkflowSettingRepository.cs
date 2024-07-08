/*
=========================================================================================================
  Module      : 定期ワークフロー設定リポジトリ (FixedPurchaseWorkflowSettingRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.FixedPurchaseWorkflowSetting
{
	/// <summary>
	/// 定期ワークフロー設定リポジトリ
	/// </summary>
	internal class FixedPurchaseWorkflowSettingRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "FixedPurchaseWorkflowSetting";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal FixedPurchaseWorkflowSettingRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		internal FixedPurchaseWorkflowSettingRepository(SqlAccessor accessor)
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
		internal FixedPurchaseWorkflowSettingModel Get(string shopId, string workflowKbn, int workflowNo)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_SHOP_ID, shopId},
				{Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_WORKFLOW_KBN, workflowKbn},
				{Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_WORKFLOW_NO, workflowNo},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new FixedPurchaseWorkflowSettingModel(dv[0]);
		}
		#endregion

		#region ~GetFixedPurchaseWorkflowSettingInDataView HACK: 例外的にDataViewを返す
		/// <summary>
		/// Get Fixed Purchase Workflow Setting In DataView HACK: 例外的にDataViewを返す
		/// </summary>
		/// <param name="shopId">ショップID</param>
		/// <param name="workFlowKbn">ワークフロー区分</param>
		/// <param name="workFlowNo">ワークフローNo</param>
		/// <returns>DataView</returns>
		internal DataView GetFixedPurchaseWorkflowSettingInDataView(
			string shopId,
			string workFlowKbn,
			string workFlowNo)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_SHOP_ID, shopId },
				{ Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_WORKFLOW_KBN, workFlowKbn },
				{ Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_WORKFLOW_NO, workFlowNo }
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			return dv;
		}
		#endregion

		#region ~GetFixedPurchaseWorkflowSetting
		/// <summary>
		/// Get fixed purchase workflow setting
		/// </summary>
		/// <param name="workflowKbn">A workflow kbn</param>
		/// <param name="workflowNo">A workflow no</param>
		/// <returns>A fixed purchase workflow setting model</returns>
		internal FixedPurchaseWorkflowSettingModel GetFixedPurchaseWorkflowSetting(
			string workflowKbn,
			string workflowNo)
		{
			var dv = Get(
				XML_KEY_NAME,
				"GetFixedPurchaseWorkflowSetting",
				new Hashtable
				{
					{ Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_WORKFLOW_KBN, workflowKbn },
					{ Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_WORKFLOW_NO, workflowNo }
				});
			var result = (dv.Count != 0)
				? new FixedPurchaseWorkflowSettingModel(dv[0])
				: null;
			return result;
		}
		#endregion

		#region ~GetForScenarioRegistration シナリオ登録(編集)用のGet
		/// <summary>
		/// シナリオ登録(編集)用のGet
		/// </summary>
		/// <returns>定期ワークフロー設定モデル(複数)</returns>
		internal FixedPurchaseWorkflowSettingModel[] GetForScenarioRegistration()
		{
			var dv = Get(XML_KEY_NAME, "GetForScenarioRegistration");
			return dv.Cast<DataRowView>().Select(drv => new FixedPurchaseWorkflowSettingModel(drv)).ToArray();
		}
		#endregion
	}
}
