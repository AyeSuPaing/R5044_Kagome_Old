/*
=========================================================================================================
  Module      : 受注ワークフローシナリオ設定モデル (OrderWorkflowScenarioSettingModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.OrderWorkflowScenarioSetting
{
	/// <summary>
	/// 受注ワークフローシナリオ設定モデル
	/// </summary>
	public partial class OrderWorkflowScenarioSettingModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>アイテムリスト</summary>
		public OrderWorkflowScenarioSettingItemModel[] Items
		{
			get { return (OrderWorkflowScenarioSettingItemModel[])this.DataSource["EX_Items"]; } 
			set { this.DataSource["EX_Items"] = value; }
		}
		#endregion
	}
}
