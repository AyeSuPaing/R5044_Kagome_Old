/*
=========================================================================================================
  Module      : シナリオ設定アイテム入力クラス (OrderWorkflowScenarioSettingItemInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using w2.App.Common.Input;
using w2.Domain.FixedPurchaseWorkflowSetting;
using w2.Domain.OrderWorkflowScenarioSetting;
using w2.Domain.OrderWorkflowSetting;

/// <summary>
/// シナリオ設定アイテム入力クラス
/// </summary>
public class OrderWorkflowScenarioSettingItemInput : InputBase<OrderWorkflowScenarioSettingItemModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public OrderWorkflowScenarioSettingItemInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public OrderWorkflowScenarioSettingItemInput(OrderWorkflowScenarioSettingItemModel model)
		: this()
	{
		this.ScenarioSettingId = model.ScenarioSettingId;
		this.ScenarioNo = model.ScenarioNo.ToString();
		this.ShopId = model.ShopId;
		this.WorkflowKbn = model.WorkflowKbn;
		this.WorkflowNo = model.WorkflowNo.ToString();
		this.TargetWorkflowKbn = model.TargetWorkflowKbn;
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override OrderWorkflowScenarioSettingItemModel CreateModel()
	{
		if (this.TargetWorkflowKbn == Constants.FLG_ORDERWORKFLOWSCENARIOSETTING_TARGETKBN_NORMAL)
		{
			var orderWorkflowSetting = new OrderWorkflowSettingService().Get(this.ShopId, this.WorkflowKbn, int.Parse(this.WorkflowNo));
			var model = new OrderWorkflowScenarioSettingItemModel
			{
				ScenarioSettingId = this.ScenarioSettingId,
				ScenarioNo = int.Parse(this.ScenarioNo),
				ShopId = this.ShopId,
				WorkflowKbn = this.WorkflowKbn,
				WorkflowNo = int.Parse(this.WorkflowNo),
				WorkflowName = orderWorkflowSetting.WorkflowName,
				Desc1 = orderWorkflowSetting.Desc1,
				TargetWorkflowKbn = this.TargetWorkflowKbn,
			};
			return model;
		}
		else
		{
			var orderWorkflowSetting = new FixedPurchaseWorkflowSettingService().Get(this.ShopId, this.WorkflowKbn, int.Parse(this.WorkflowNo));
			var model = new OrderWorkflowScenarioSettingItemModel
			{
				ScenarioSettingId = this.ScenarioSettingId,
				ScenarioNo = int.Parse(this.ScenarioNo),
				ShopId = this.ShopId,
				WorkflowKbn = this.WorkflowKbn,
				WorkflowNo = int.Parse(this.WorkflowNo),
				WorkflowName = orderWorkflowSetting.WorkflowName,
				Desc1 = orderWorkflowSetting.Desc1,
				TargetWorkflowKbn = this.TargetWorkflowKbn,
			};
			return model;
		}
	}

	/// <summary>
	/// 検証
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	public string Validate()
	{
		var errorMessage = Validator.Validate("OrderWorkflowScenarioRegist", this.DataSource);
		return errorMessage;
	}
	#endregion

	#region プロパティ
	/// <summary>シナリオ設定ID</summary>
	public string ScenarioSettingId
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTINGITEM_SCENARIO_SETTING_ID]; }
		set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTINGITEM_SCENARIO_SETTING_ID] = value; }
	}
	/// <summary>シナリオ枝番</summary>
	public string ScenarioNo
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTINGITEM_SCENARIO_NO]; }
		set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTINGITEM_SCENARIO_NO] = value; }
	}
	/// <summary>店舗ID</summary>
	public string ShopId
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTINGITEM_SHOP_ID]; }
		set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTINGITEM_SHOP_ID] = value; }
	}
	/// <summary>ワークフロー区分</summary>
	public string WorkflowKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTINGITEM_WORKFLOW_KBN]; }
		set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTINGITEM_WORKFLOW_KBN] = value; }
	}
	/// <summary>ワークフロー枝番</summary>
	public string WorkflowNo
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTINGITEM_WORKFLOW_NO]; }
		set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTINGITEM_WORKFLOW_NO] = value; }
	}
	/// <summary>実行対象ワークフロー区分</summary>
	public string TargetWorkflowKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTINGITEM_TARGET_WORKFLOW_KBN]; }
		set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTINGITEM_TARGET_WORKFLOW_KBN] = value; }
	}
	#endregion
}
