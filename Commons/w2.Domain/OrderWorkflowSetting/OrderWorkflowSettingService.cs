/*
=========================================================================================================
  Module      : 注文ワークフロー設定サービス (OrderWorkflowSettingService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Data;

namespace w2.Domain.OrderWorkflowSetting
{
	/// <summary>
	/// 注文ワークフロー設定サービス
	/// </summary>
	public class OrderWorkflowSettingService : ServiceBase, IOrderWorkflowSettingService
	{
		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="workflowKbn">ワークフロー区分</param>
		/// <param name="workflowNo">枝番</param>
		/// <returns>モデル</returns>
		public OrderWorkflowSettingModel Get(string shopId, string workflowKbn, int workflowNo)
		{
			using (var repository = new OrderWorkflowSettingRepository())
			{
				var model = repository.Get(shopId, workflowKbn, workflowNo);
				return model;
			}
		}

		/// <summary>
		/// 定期ワークフロー取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="workflowKbn">ワークフロー区分</param>
		/// <param name="workflowNo">枝番</param>
		/// <returns>モデル</returns>
		public OrderWorkflowSettingModel GetFixedPurchaseWorkflow(string shopId, string workflowKbn, int workflowNo)
		{

			using (var repository = new OrderWorkflowSettingRepository())
			{
				var model = repository.GetFixedPurchaseWorkflow(shopId, workflowKbn, workflowNo);
				return model;
			}
		}
		#endregion

		#region ~GetTotalOrderWorkflowSettingCount 注文ワークフロー設定の総数を取得
		/// <summary>
		/// 注文ワークフロー設定の総件数を取得する
		/// </summary>
		/// <returns>注文ワークフロー設定の総件数</returns>
		public int GetTotalOrderWorkflowSettingCount()
		{
			using (var repository = new OrderWorkflowSettingRepository())
			{
				var count = repository.GetTotalOrderWorkflowSettingCount();
				return count;
			}
		}
		#endregion

		#region +GetForScenarioRegistration シナリオ登録(編集)用のGet
		/// <summary>
		/// 検索
		/// </summary>
		/// <returns>モデル(複数)</returns>
		public OrderWorkflowSettingModel[] GetForScenarioRegistration()
		{
			using (var repository = new OrderWorkflowSettingRepository())
			{
				var results = repository.GetForScenarioRegistration();
				return results;
			}
		}
		#endregion

		#region +GetOrderWorkflowSettingInDataView HACK: 例外的にDataViewを返す
		/// <summary>
		/// 受注ワークフローの設定を取得する HACK: 例外的にDataViewを返す
		/// </summary>
		/// <param name="shopId">ショップID</param>
		/// <param name="workFlowKbn">ワークフロー区分</param>
		/// <param name="workFlowNo">ワークフローNo</param>
		/// <returns>DataView</returns>
		public DataView GetOrderWorkflowSettingInDataView(string shopId, string workFlowKbn, string workFlowNo)
		{
			using (var repository = new OrderWorkflowSettingRepository())
			{
				var results = repository.GetOrderWorkflowSettingInDataView(shopId, workFlowKbn, workFlowNo);
				return results;
			}
		}
		#endregion

		#region +GetAll 全てGet
		/// <summary>
		/// 全てGet
		/// </summary>
		/// <returns>モデル(複数)</returns>
		public OrderWorkflowSettingModel[] GetAll()
		{
			using (var repository = new OrderWorkflowSettingRepository())
			{
				var results = repository.GetAll();
				return results;
			}
		}
		#endregion

		#region +GetOrderWorkflowSettings
		/// <summary>
		/// Get order workflow settings
		/// </summary>
		/// <param name="workflowKbn">A workflow kbn</param>
		/// <param name="workflowName">A workflow name</param>
		/// <returns>A collection of order workflow setting models</returns>
		public OrderWorkflowSettingModel[] GetOrderWorkflowSettings(
			string workflowKbn,
			string workflowName)
		{
			using (var repository = new OrderWorkflowSettingRepository())
			{
				var results = repository.GetOrderWorkflowSettings(workflowKbn, workflowName);
				return results;
			}
		}
		#endregion

		#region +GetOrderWorkflowSetting
		/// <summary>
		/// Get order workflow setting
		/// </summary>
		/// <param name="workflowKbn">A workflow kbn</param>
		/// <param name="workflowNo">A workflow no</param>
		/// <returns>A order workflow setting model</returns>
		public OrderWorkflowSettingModel GetOrderWorkflowSetting(
			string workflowKbn,
			string workflowNo)
		{
			using (var repository = new OrderWorkflowSettingRepository())
			{
				var results = repository.GetOrderWorkflowSetting(workflowKbn, workflowNo);
				return results;
			}
		}
		#endregion
	}
}
