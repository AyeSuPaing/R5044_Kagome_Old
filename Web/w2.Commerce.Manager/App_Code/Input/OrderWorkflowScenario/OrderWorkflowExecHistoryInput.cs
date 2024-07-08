/*
=========================================================================================================
  Module      : 受注ワークフロー実行履歴入力クラス (OrderWorkflowExecHistoryInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Input;
using w2.Domain.OrderWorkflowExecHistory;

/// <summary>
/// 受注ワークフロー実行履歴入力クラス
/// </summary>
public class OrderWorkflowExecHistoryInput : InputBase<OrderWorkflowExecHistoryModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public OrderWorkflowExecHistoryInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public OrderWorkflowExecHistoryInput(OrderWorkflowExecHistoryModel model)
		: this()
	{
		this.OrderWorkflowExecHistoryId = model.OrderWorkflowExecHistoryId.ToString();
		this.ShopId = model.ShopId;
		this.WorkflowKbn = model.WorkflowKbn;
		this.WorkflowNo = model.WorkflowNo.ToString();
		this.ScenarioSettingId = model.ScenarioSettingId;
		this.ExecStatus = model.ExecStatus;
		this.SuccessRate = model.SuccessRate;
		this.ExecPlace = model.ExecPlace;
		this.ExecTiming = model.ExecTiming;
		this.Message = model.Message;
		this.DateCreated = model.DateCreated.ToString();
		this.LastChanged = model.LastChanged;
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override OrderWorkflowExecHistoryModel CreateModel()
	{
		var model = new OrderWorkflowExecHistoryModel
		{
			OrderWorkflowExecHistoryId = long.Parse(this.OrderWorkflowExecHistoryId),
			ShopId = this.ShopId,
			WorkflowKbn = this.WorkflowKbn,
			WorkflowNo = int.Parse(this.WorkflowNo),
			ScenarioSettingId = this.ScenarioSettingId,
			ExecStatus = this.ExecStatus,
			SuccessRate = this.SuccessRate,
			ExecPlace = this.ExecPlace,
			ExecTiming = this.ExecTiming,
			Message = this.Message,
			LastChanged = this.LastChanged,
		};
		return model;
	}
	#endregion

	#region プロパティ
	/// <summary>受注ワークフロー実行履歴ID</summary>
	public string OrderWorkflowExecHistoryId
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_ORDER_WORKFLOW_EXEC_HISTORY_ID]; }
		set { this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_ORDER_WORKFLOW_EXEC_HISTORY_ID] = value; }
	}
	/// <summary>店舗ID</summary>
	public string ShopId
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_SHOP_ID]; }
		set { this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_SHOP_ID] = value; }
	}
	/// <summary>ワークフロー区分</summary>
	public string WorkflowKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_WORKFLOW_KBN]; }
		set { this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_WORKFLOW_KBN] = value; }
	}
	/// <summary>ワークフロー枝番</summary>
	public string WorkflowNo
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_WORKFLOW_NO]; }
		set { this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_WORKFLOW_NO] = value; }
	}
	/// <summary>シナリオ設定ID</summary>
	public string ScenarioSettingId
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_SCENARIO_SETTING_ID]; }
		set { this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_SCENARIO_SETTING_ID] = value; }
	}
	/// <summary>実行ステータス</summary>
	public string ExecStatus
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS]; }
		set { this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS] = value; }
	}
	/// <summary>成功件率</summary>
	public string SuccessRate
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_SUCCESS_RATE]; }
		set { this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_SUCCESS_RATE] = value; }
	}
	/// <summary>実行場所</summary>
	public string ExecPlace
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_EXEC_PLACE]; }
		set { this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_EXEC_PLACE] = value; }
	}
	/// <summary>実行タイミング</summary>
	public string ExecTiming
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_EXEC_TIMING]; }
		set { this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_EXEC_TIMING] = value; }
	}
	/// <summary>メッセージ</summary>
	public string Message
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_MESSAGE]; }
		set { this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_MESSAGE] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_DATE_CREATED] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_LAST_CHANGED] = value; }
	}
	#endregion
}
