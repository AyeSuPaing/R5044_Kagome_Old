/*
=========================================================================================================
  Module      : シナリオ設定アイテムモデル (OrderWorkflowScenarioSettingItemModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using w2.Domain.FixedPurchaseWorkflowSetting;
using w2.Domain.OrderWorkflowSetting;

namespace w2.Domain.OrderWorkflowScenarioSetting
{
	/// <summary>
	/// シナリオ設定アイテムモデル
	/// </summary>
	public partial class OrderWorkflowScenarioSettingItemModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>受注ワークフロー設定</summary>
		public OrderWorkflowSettingModel OrderWorkflowSetting
		{
			get { return (OrderWorkflowSettingModel)this.DataSource["EX_OrderWorkflowSetting"]; }
			set { this.DataSource["EX_OrderWorkflowSetting"] = value; }
		}
		/// <summary>定期ワークフロー設定</summary>
		public FixedPurchaseWorkflowSettingModel FixedPurchaseWorkflowSetting
		{
			get { return (FixedPurchaseWorkflowSettingModel)this.DataSource["EX_FixedPurchaseWorkflowSetting"]; }
			set { this.DataSource["EX_FixedPurchaseWorkflowSetting"] = value; }
		}
		/// <summary>ワークフロー名</summary>
		public string WorkflowName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_NAME] = value; }
		}
		/// <summary>説明1</summary>
		public string Desc1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_DESC1]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSETTING_DESC1] = value; }
		}
		#endregion
	}
}
