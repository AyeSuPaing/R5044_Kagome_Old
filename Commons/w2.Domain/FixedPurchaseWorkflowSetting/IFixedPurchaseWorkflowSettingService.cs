/*
=========================================================================================================
  Module      : 定期ワークフロー設定サービスのインターフェース (IFixedPurchaseWorkflowSettingService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Data;

namespace w2.Domain.FixedPurchaseWorkflowSetting
{
	/// <summary>
	/// 定期ワークフロー設定サービスのインターフェース
	/// </summary>
	public interface IFixedPurchaseWorkflowSettingService : IService
	{
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="workflowKbn">ワークフロー区分</param>
		/// <param name="workflowNo">枝番</param>
		/// <returns>モデル</returns>
		FixedPurchaseWorkflowSettingModel Get(string shopId, string workflowKbn, int workflowNo);

		/// <summary>
		/// Get Fixed Purchase Workflow Setting In DataView HACK: 例外的にDataViewを返す
		/// </summary>
		/// <param name="shopId">ショップID</param>
		/// <param name="workFlowKbn">ワークフロー区分</param>
		/// <param name="workFlowNo">ワークフローNo</param>
		/// <returns>DataView</returns>
		DataView GetFixedPurchaseWorkflowSettingInDataView(
			string shopId,
			string workFlowKbn,
			string workFlowNo);

		/// <summary>
		/// Get fixed purchase workflow setting
		/// </summary>
		/// <param name="workflowKbn">A workflow kbn</param>
		/// <param name="workflowNo">A workflow no</param>
		/// <returns>A fixed purchase workflow setting model</returns>
		FixedPurchaseWorkflowSettingModel GetFixedPurchaseWorkflowSetting(string workflowKbn, string workflowNo);
	}
}
