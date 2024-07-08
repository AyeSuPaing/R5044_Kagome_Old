/*
=========================================================================================================
  Module      : ターゲットリスト設定入力クラス (TargetListInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using w2.App.Common.Input;
using w2.Domain.TargetList;

/// <summary>
/// ターゲットリスト設定マスタ入力クラス
/// </summary>
public class TargetListInput : InputBase<TargetListModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public TargetListInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public TargetListInput(TargetListModel model)
		: this()
	{
		this.DeptId = model.DeptId;
		this.TargetId = model.TargetId;
		this.TargetName = model.TargetName;
		this.Status = model.Status;
		this.TargetType = model.TargetType;
		this.TargetCondition = model.TargetCondition;
		this.DataCount = (model.DataCount != null) ? model.DataCount.ToString() : null;
		this.DataCountDate = (model.DataCountDate != null) ? model.DataCountDate.ToString() : null;
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
		this.DelFlg = model.DelFlg;
		this.LastChanged = model.LastChanged;
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override TargetListModel CreateModel()
	{
		var model = new TargetListModel
		{
			DeptId = this.DeptId,
			TargetId = this.TargetId,
			TargetName = this.TargetName,
			Status = this.Status,
			TargetType = this.TargetType,
			TargetCondition = this.TargetCondition,
			DataCount = (this.DataCount != null) ? int.Parse(this.DataCount) : (int?)null,
			DataCountDate = (this.DataCountDate != null) ? DateTime.Parse(this.DataCountDate) : (DateTime?)null,
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
			DelFlg = this.DelFlg,
			LastChanged = this.LastChanged,
		};
		return model;
	}

	/// <summary>
	/// スケジュール入力内容をセット
	/// </summary>
	/// <param name="input">入力内容</param>
	public void SetSchedule(Hashtable input)
	{
		this.ExecTiming = StringUtility.ToEmpty(input[Constants.FIELD_TARGETLIST_EXEC_TIMING]);
		this.ScheduleKbn = StringUtility.ToEmpty(input[Constants.FIELD_TARGETLIST_SCHEDULE_KBN]);
		this.ScheduleDayOfWeek = StringUtility.ToEmpty(input[Constants.FIELD_TARGETLIST_SCHEDULE_DAY_OF_WEEK]);
		var year =  StringUtility.ToEmpty(input[Constants.FIELD_TARGETLIST_SCHEDULE_YEAR]);
		this.ScheduleYear = (string.IsNullOrEmpty(year)) ? null : year;
		var mohth = StringUtility.ToEmpty(input[Constants.FIELD_TARGETLIST_SCHEDULE_MONTH]);
		this.ScheduleMonth = (string.IsNullOrEmpty(mohth)) ? null : mohth;
		var day = StringUtility.ToEmpty(input[Constants.FIELD_TARGETLIST_SCHEDULE_DAY]);
		this.ScheduleDay = (string.IsNullOrEmpty(day)) ? null : day;
		var hour = StringUtility.ToEmpty(input[Constants.FIELD_TARGETLIST_SCHEDULE_DAY]);
		this.ScheduleHour = (string.IsNullOrEmpty(hour)) ? null : hour;
		var minute = StringUtility.ToEmpty(input[Constants.FIELD_TARGETLIST_SCHEDULE_MINUTE]);
		this.ScheduleMinute = (string.IsNullOrEmpty(minute)) ? null : minute;
		var second = StringUtility.ToEmpty(input[Constants.FIELD_TARGETLIST_SCHEDULE_SECOND]);
		this.ScheduleSecond = (string.IsNullOrEmpty(second)) ? null : second;
	}
	#endregion

	#region プロパティ
	/// <summary>識別ID</summary>
	public string DeptId
	{
		get { return (string)this.DataSource[Constants.FIELD_TARGETLIST_DEPT_ID]; }
		set { this.DataSource[Constants.FIELD_TARGETLIST_DEPT_ID] = value; }
	}
	/// <summary>ターゲットリストID</summary>
	public string TargetId
	{
		get { return (string)this.DataSource[Constants.FIELD_TARGETLIST_TARGET_ID]; }
		set { this.DataSource[Constants.FIELD_TARGETLIST_TARGET_ID] = value; }
	}
	/// <summary>ターゲットリスト名</summary>
	public string TargetName
	{
		get { return (string)this.DataSource[Constants.FIELD_TARGETLIST_TARGET_NAME]; }
		set { this.DataSource[Constants.FIELD_TARGETLIST_TARGET_NAME] = value; }
	}
	/// <summary>ステータス</summary>
	public string Status
	{
		get { return (string)this.DataSource[Constants.FIELD_TARGETLIST_STATUS]; }
		set { this.DataSource[Constants.FIELD_TARGETLIST_STATUS] = value; }
	}
	/// <summary>ターゲット種別</summary>
	public string TargetType
	{
		get { return (string)this.DataSource[Constants.FIELD_TARGETLIST_TARGET_TYPE]; }
		set { this.DataSource[Constants.FIELD_TARGETLIST_TARGET_TYPE] = value; }
	}
	/// <summary>抽出条件</summary>
	public string TargetCondition
	{
		get { return (string)this.DataSource[Constants.FIELD_TARGETLIST_TARGET_CONDITION]; }
		set { this.DataSource[Constants.FIELD_TARGETLIST_TARGET_CONDITION] = value; }
	}
	/// <summary>前回抽出件数</summary>
	public string DataCount
	{
		get { return (string)this.DataSource[Constants.FIELD_TARGETLIST_DATA_COUNT]; }
		set { this.DataSource[Constants.FIELD_TARGETLIST_DATA_COUNT] = value; }
	}
	/// <summary>前回抽出日付</summary>
	public string DataCountDate
	{
		get { return (string)this.DataSource[Constants.FIELD_TARGETLIST_DATA_COUNT_DATE]; }
		set { this.DataSource[Constants.FIELD_TARGETLIST_DATA_COUNT_DATE] = value; }
	}
	/// <summary>実行タイミング</summary>
	public string ExecTiming
	{
		get { return (string)this.DataSource[Constants.FIELD_TARGETLIST_EXEC_TIMING]; }
		set { this.DataSource[Constants.FIELD_TARGETLIST_EXEC_TIMING] = value; }
	}
	/// <summary>スケジュール区分</summary>
	public string ScheduleKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_TARGETLIST_SCHEDULE_KBN]; }
		set { this.DataSource[Constants.FIELD_TARGETLIST_SCHEDULE_KBN] = value; }
	}
	/// <summary>スケジュール曜日</summary>
	public string ScheduleDayOfWeek
	{
		get { return (string)this.DataSource[Constants.FIELD_TARGETLIST_SCHEDULE_DAY_OF_WEEK]; }
		set { this.DataSource[Constants.FIELD_TARGETLIST_SCHEDULE_DAY_OF_WEEK] = value; }
	}
	/// <summary>スケジュール日程(年)</summary>
	public string ScheduleYear
	{
		get { return (string)this.DataSource[Constants.FIELD_TARGETLIST_SCHEDULE_YEAR]; }
		set { this.DataSource[Constants.FIELD_TARGETLIST_SCHEDULE_YEAR] = value; }
	}
	/// <summary>スケジュール日程(月)</summary>
	public string ScheduleMonth
	{
		get { return (string)this.DataSource[Constants.FIELD_TARGETLIST_SCHEDULE_MONTH]; }
		set { this.DataSource[Constants.FIELD_TARGETLIST_SCHEDULE_MONTH] = value; }
	}
	/// <summary>スケジュール日程(日)</summary>
	public string ScheduleDay
	{
		get { return (string)this.DataSource[Constants.FIELD_TARGETLIST_SCHEDULE_DAY]; }
		set { this.DataSource[Constants.FIELD_TARGETLIST_SCHEDULE_DAY] = value; }
	}
	/// <summary>スケジュール日程(時)</summary>
	public string ScheduleHour
	{
		get { return (string)this.DataSource[Constants.FIELD_TARGETLIST_SCHEDULE_HOUR]; }
		set { this.DataSource[Constants.FIELD_TARGETLIST_SCHEDULE_HOUR] = value; }
	}
	/// <summary>スケジュール日程(分)</summary>
	public string ScheduleMinute
	{
		get { return (string)this.DataSource[Constants.FIELD_TARGETLIST_SCHEDULE_MINUTE]; }
		set { this.DataSource[Constants.FIELD_TARGETLIST_SCHEDULE_MINUTE] = value; }
	}
	/// <summary>スケジュール日程(秒)</summary>
	public string ScheduleSecond
	{
		get { return (string)this.DataSource[Constants.FIELD_TARGETLIST_SCHEDULE_SECOND]; }
		set { this.DataSource[Constants.FIELD_TARGETLIST_SCHEDULE_SECOND] = value; }
	}
	/// <summary>有効フラグ</summary>
	public string ValidFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_TARGETLIST_VALID_FLG]; }
		set { this.DataSource[Constants.FIELD_TARGETLIST_VALID_FLG] = value; }
	}
	/// <summary>削除フラグ</summary>
	public string DelFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_TARGETLIST_DEL_FLG]; }
		set { this.DataSource[Constants.FIELD_TARGETLIST_DEL_FLG] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_TARGETLIST_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_TARGETLIST_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_TARGETLIST_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_TARGETLIST_DATE_CHANGED] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_TARGETLIST_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_TARGETLIST_LAST_CHANGED] = value; }
	}
	#endregion
}
