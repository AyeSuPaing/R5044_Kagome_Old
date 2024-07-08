/*
=========================================================================================================
  Module      : 受注ワークフローシナリオ設定入力クラス (OrderWorkflowScenarioSettingInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using w2.App.Common.Input;
using w2.Domain.OrderWorkflowScenarioSetting;

/// <summary>
/// 受注ワークフローシナリオ設定入力クラス
/// </summary>
public class OrderWorkflowScenarioSettingInput : InputBase<OrderWorkflowScenarioSettingModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public OrderWorkflowScenarioSettingInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public OrderWorkflowScenarioSettingInput(OrderWorkflowScenarioSettingModel model)
		: this()
	{
		this.ScenarioName = model.ScenarioName;
		this.ExecTiming = model.ExecTiming;
		this.ScheduleKbn = model.ScheduleKbn;
		this.ScheduleDayOfWeek = model.ScheduleDayOfWeek;
		this.ScheduleYear = (model.ScheduleYear != null) ? model.ScheduleYear.ToString() : null;
		this.ScheduleMonth = (model.ScheduleMonth != null) ? model.ScheduleMonth.ToString() : null;
		this.ScheduleDay = (model.ScheduleDay != null) ? model.ScheduleDay.ToString() : null;
		this.ScheduleHour = (model.ScheduleHour != null) ? model.ScheduleHour.ToString() : null;
		this.ScheduleMinute = (model.ScheduleMinute != null) ? model.ScheduleMinute.ToString() : null;
		this.ScheduleSecond = (model.ScheduleSecond != null) ? model.ScheduleSecond.ToString() : null;
		this.ValidFlg = model.ValidFlg;
		this.DateCreated = model.DateCreated.ToString();
		this.DateChanged = model.DateChanged.ToString();
		this.LastChanged = model.LastChanged;
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override OrderWorkflowScenarioSettingModel CreateModel()
	{
		var model = new OrderWorkflowScenarioSettingModel
		{
			ScenarioSettingId = this.ScenarioSettingId,
			ScenarioName = this.ScenarioName,
			ExecTiming = this.ExecTiming,
			ScheduleKbn = this.ScheduleKbn,
			ScheduleDayOfWeek = this.ScheduleDayOfWeek,
			ScheduleYear = (this.ScheduleYear != null) ? int.Parse(this.ScheduleYear) : (int?)null,
			ScheduleMonth = (this.ScheduleMonth != null) ? int.Parse(this.ScheduleMonth) : (int?)null,
			ScheduleDay = (this.ScheduleDay != null) ? int.Parse(this.ScheduleDay) : (int?)null,
			ScheduleHour = (this.ScheduleHour != null) ? int.Parse(this.ScheduleHour) : (int?)null,
			ScheduleMinute = (this.ScheduleMinute != null) ? int.Parse(this.ScheduleMinute) : (int?)null,
			ScheduleSecond = (this.ScheduleSecond != null) ? int.Parse(this.ScheduleSecond) : (int?)null,
			ValidFlg = this.ValidFlg,
			LastChanged = this.LastChanged,
			Items = this.Items.Select(item => item.CreateModel()).ToArray(),
		};
		return model;
	}

	/// <summary>
	/// 検証
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	public string Validate()
	{
		var errorMessage = Validator.Validate("OrderWorkflowScenarioRegist", this.DataSource);
		if (this.Items.Any(item => string.IsNullOrEmpty(item.Validate()) == false))
		{
			errorMessage += WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERWORKFLOWSCENARIOSETTING_SELECT_NECESSARY);
		}

		if (this.ScheduleKbn == Constants.FLG_POINTRULESCHEDULE_SCHEDULE_KBN_ONCE)
		{
			this.ScheduleDate = string.Format("{0}/{1}/{2}", this.ScheduleYear, this.ScheduleMonth, this.ScheduleDay);
			errorMessage += Validator.Validate("DateTimeInput", this.DataSource);
		}
		return errorMessage;
    }
	#endregion

	#region プロパティ
	/// <summary>シナリオID</summary>
	public string ScenarioSettingId
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCENARIO_SETTING_ID]; }
		set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCENARIO_SETTING_ID] = value; }
	}
	/// <summary>シナリオ名</summary>
	public string ScenarioName
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCENARIO_NAME]; }
		set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCENARIO_NAME] = value; }
	}
	/// <summary>実行タイミング</summary>
	public string ExecTiming
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_EXEC_TIMING]; }
		set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_EXEC_TIMING] = value; }
	}
	/// <summary>スケジュール区分</summary>
	public string ScheduleKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_KBN]; }
		set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_KBN] = value; }
	}
	/// <summary>スケジュール曜日</summary>
	public string ScheduleDayOfWeek
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_DAY_OF_WEEK]; }
		set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_DAY_OF_WEEK] = value; }
	}
	/// <summary>スケジュール日程(年)</summary>
	public string ScheduleYear
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_YEAR]; }
		set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_YEAR] = value; }
	}
	/// <summary>スケジュール日程(月)</summary>
	public string ScheduleMonth
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_MONTH]; }
		set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_MONTH] = value; }
	}
	/// <summary>スケジュール日程(日)</summary>
	public string ScheduleDay
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_DAY]; }
		set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_DAY] = value; }
	}
	/// <summary>スケジュール日程(時)</summary>
	public string ScheduleHour
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_HOUR]; }
		set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_HOUR] = value; }
	}
	/// <summary>スケジュール日程(分)</summary>
	public string ScheduleMinute
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_MINUTE]; }
		set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_MINUTE] = value; }
	}
	/// <summary>スケジュール日程(秒)</summary>
	public string ScheduleSecond
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_SECOND]; }
		set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_SECOND] = value; }
	}
	/// <summary>スケジュール日程(年月日)</summary>
	public string ScheduleDate
	{
		get { return (string)this.DataSource["scnedule_date"]; }
		set { this.DataSource["scnedule_date"] = value; }
	}
	/// <summary>有効フラグ</summary>
	public string ValidFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_VALID_FLG]; }
		set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_VALID_FLG] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_DATE_CHANGED] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_LAST_CHANGED] = value; }
	}
	/// <summary>受注ワークフローシナリオ設定アイテム</summary>
	public OrderWorkflowScenarioSettingItemInput[] Items
	{
		get { return (OrderWorkflowScenarioSettingItemInput[])this.DataSource[Constants.TABLE_ORDERWORKFLOWSCENARIOSETTINGITEM]; }
		set { this.DataSource[Constants.TABLE_ORDERWORKFLOWSCENARIOSETTINGITEM] = value; }
	}
	#endregion
}
