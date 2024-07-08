/*
=========================================================================================================
  Module      : 定期ワークフロー設定サービス (FixedPurchaseWorkflowSettingService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Data;
namespace w2.Domain.FixedPurchaseWorkflowSetting
{
	/// <summary>
	/// 定期ワークフロー設定サービス
	/// </summary>
	public class FixedPurchaseWorkflowSettingService : ServiceBase, IFixedPurchaseWorkflowSettingService
	{
		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="workflowKbn">ワークフロー区分</param>
		/// <param name="workflowNo">枝番</param>
		/// <returns>モデル</returns>
		public FixedPurchaseWorkflowSettingModel Get(string shopId, string workflowKbn, int workflowNo)
		{
			using (var repository = new FixedPurchaseWorkflowSettingRepository())
			{
				var model = repository.Get(shopId, workflowKbn, workflowNo);
				return model;
			}
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
		public DataView GetFixedPurchaseWorkflowSettingInDataView(
			string shopId,
			string workFlowKbn,
			string workFlowNo)
		{
			using (var repository = new FixedPurchaseWorkflowSettingRepository())
			{
				var result = repository.GetFixedPurchaseWorkflowSettingInDataView(
					shopId,
					workFlowKbn,
					workFlowNo);
				return result;
			}
		}
		#endregion

		#region ~GetFixedPurchaseWorkflowSetting
		/// <summary>
		/// Get fixed purchase workflow setting
		/// </summary>
		/// <param name="workflowKbn">A workflow kbn</param>
		/// <param name="workflowNo">A workflow no</param>
		/// <returns>A fixed purchase workflow setting model</returns>
		public FixedPurchaseWorkflowSettingModel GetFixedPurchaseWorkflowSetting(
			string workflowKbn,
			string workflowNo)
		{
			using (var repository = new FixedPurchaseWorkflowSettingRepository())
			{
				var result = repository.GetFixedPurchaseWorkflowSetting(
					workflowKbn,
					workflowNo);
				return result;
			}
		}
		#endregion

		#region +GetForScenarioRegistration シナリオ登録(編集)用のGet
		/// <summary>
		/// シナリオ登録用のGET
		/// </summary>
		/// <returns>定期ワークフロー設定モデル(複数)</returns>
		public FixedPurchaseWorkflowSettingModel[] GetForScenarioRegistration()
		{
			using (var repository = new FixedPurchaseWorkflowSettingRepository())
			{
				var results = repository.GetForScenarioRegistration();
				return results;
			}
		}
		#endregion
	}
}
