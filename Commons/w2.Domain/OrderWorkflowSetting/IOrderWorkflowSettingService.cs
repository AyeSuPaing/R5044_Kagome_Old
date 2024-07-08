/*
=========================================================================================================
  Module      : 注文ワークフロー設定サービスのインターフェース (IOrderWorkflowSettingService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Data;

namespace w2.Domain.OrderWorkflowSetting
{
	/// <summary>
	/// 注文ワークフロー設定サービスのインターフェース
	/// </summary>
	public interface IOrderWorkflowSettingService : IService
	{
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="workflowKbn">ワークフロー区分</param>
		/// <param name="workflowNo">枝番</param>
		/// <returns>モデル</returns>
		OrderWorkflowSettingModel Get(string shopId, string workflowKbn, int workflowNo);

		/// <summary>
		/// 注文ワークフロー設定の総件数を取得する
		/// </summary>
		/// <returns>注文ワークフロー設定の総件数</returns>
		int GetTotalOrderWorkflowSettingCount();

		/// <summary>
		/// 検索
		/// </summary>
		/// <returns>モデル(複数)</returns>
		OrderWorkflowSettingModel[] GetForScenarioRegistration();

		/// <summary>
		/// 受注ワークフローの設定を取得する HACK: 例外的にDataViewを返す
		/// </summary>
		/// <param name="shopId">ショップID</param>
		/// <param name="workFlowKbn">ワークフロー区分</param>
		/// <param name="workFlowNo">ワークフローNo</param>
		/// <returns>DataView</returns>
		DataView GetOrderWorkflowSettingInDataView(
			string shopId,
			string workFlowKbn,
			string workFlowNo);

		/// <summary>
		/// 全てGet
		/// </summary>
		/// <returns>モデル(複数)</returns>
		OrderWorkflowSettingModel[] GetAll();

		/// <summary>
		/// Get order workflow settings
		/// </summary>
		/// <param name="workflowKbn">A workflow kbn</param>
		/// <param name="workflowName">A workflow name</param>
		/// <returns>A collection of order workflow setting models</returns>
		OrderWorkflowSettingModel[] GetOrderWorkflowSettings(
			string workflowKbn,
			string workflowName);

		/// <summary>
		/// Get order workflow setting
		/// </summary>
		/// <param name="workflowKbn">A workflow kbn</param>
		/// <param name="workflowNo">A workflow no</param>
		/// <returns>A order workflow setting model</returns>
		OrderWorkflowSettingModel GetOrderWorkflowSetting(string workflowKbn, string workflowNo);
	}
}
