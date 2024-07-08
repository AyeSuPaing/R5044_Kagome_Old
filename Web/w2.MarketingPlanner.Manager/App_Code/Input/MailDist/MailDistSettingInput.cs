/*
=========================================================================================================
  Module      : メール配信設定入力クラス (MailDistSettingInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using w2.App.Common.Input;
using w2.Domain.MailDistSetting;

/// <summary>
/// メール配信設定マスタ入力クラス
/// </summary>
public class MailDistSettingInput : InputBase<MailDistSettingModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public MailDistSettingInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public MailDistSettingInput(MailDistSettingModel model) : this()
	{
		this.DeptId = model.DeptId;
		this.MaildistId = model.MaildistId;
		this.MaildistName = model.MaildistName;
		this.Status = model.Status;
		this.LastCount = model.LastCount.ToString();
		this.LastErrorexceptCount = model.LastErrorexceptCount.ToString();
		this.LastMobilemailexceptCount = model.LastMobilemailexceptCount.ToString();
		this.LastDistDate = (model.LastDistDate != null) ? model.LastDistDate.ToString() : null;
		this.TargetId = model.TargetId;
		this.TargetExtractFlg = model.TargetExtractFlg;
		this.TargetId2 = model.TargetId2;
		this.TargetExtractFlg2 = model.TargetExtractFlg2;
		this.TargetId3 = model.TargetId3;
		this.TargetExtractFlg3 = model.TargetExtractFlg3;
		this.TargetId4 = model.TargetId4;
		this.TargetExtractFlg4 = model.TargetExtractFlg4;
		this.TargetId5 = model.TargetId5;
		this.TargetExtractFlg5 = model.TargetExtractFlg5;
		this.ExceptErrorPoint = model.ExceptErrorPoint.ToString();
		this.ExceptMobilemailFlg = model.ExceptMobilemailFlg;
		this.MailtextId = model.MailtextId;
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
	public override MailDistSettingModel CreateModel()
	{
		var model = new MailDistSettingModel
		{
			DeptId = this.DeptId,
			MaildistId = this.MaildistId,
			MaildistName = this.MaildistName,
			Status = this.Status,
			LastCount = long.Parse(this.LastCount),
			LastErrorexceptCount = long.Parse(this.LastErrorexceptCount),
			LastMobilemailexceptCount = long.Parse(this.LastMobilemailexceptCount),
			LastDistDate = (this.LastDistDate != null) ? DateTime.Parse(this.LastDistDate) : (DateTime?)null,
			TargetId = this.TargetId,
			TargetExtractFlg = this.TargetExtractFlg,
			TargetId2 = this.TargetId2,
			TargetExtractFlg2 = this.TargetExtractFlg2,
			TargetId3 = this.TargetId3,
			TargetExtractFlg3 = this.TargetExtractFlg3,
			TargetId4 = this.TargetId4,
			TargetExtractFlg4 = this.TargetExtractFlg4,
			TargetId5 = this.TargetId5,
			TargetExtractFlg5 = this.TargetExtractFlg5,
			ExceptErrorPoint = int.Parse(this.ExceptErrorPoint),
			ExceptMobilemailFlg = this.ExceptMobilemailFlg,
			MailtextId = this.MailtextId,
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
		this.ExecTiming = StringUtility.ToEmpty(input[Constants.FIELD_MAILDISTSETTING_EXEC_TIMING]);
		this.ScheduleKbn = StringUtility.ToEmpty(input[Constants.FIELD_MAILDISTSETTING_SCHEDULE_KBN]);
		this.ScheduleDayOfWeek = StringUtility.ToEmpty(input[Constants.FIELD_MAILDISTSETTING_SCHEDULE_DAY_OF_WEEK]);
		var year = StringUtility.ToEmpty(input[Constants.FIELD_MAILDISTSETTING_SCHEDULE_YEAR]);
		this.ScheduleYear = (string.IsNullOrEmpty(year)) ? null : year;
		var mohth = StringUtility.ToEmpty(input[Constants.FIELD_MAILDISTSETTING_SCHEDULE_MONTH]);
		this.ScheduleMonth = (string.IsNullOrEmpty(mohth)) ? null : mohth;
		var day = StringUtility.ToEmpty(input[Constants.FIELD_MAILDISTSETTING_SCHEDULE_DAY]);
		this.ScheduleDay = (string.IsNullOrEmpty(day)) ? null : day;
		var hour = StringUtility.ToEmpty(input[Constants.FIELD_MAILDISTSETTING_SCHEDULE_HOUR]);
		this.ScheduleHour = (string.IsNullOrEmpty(hour)) ? null : hour;
		var minute = StringUtility.ToEmpty(input[Constants.FIELD_MAILDISTSETTING_SCHEDULE_MINUTE]);
		this.ScheduleMinute = (string.IsNullOrEmpty(minute)) ? null : minute;
		var second = StringUtility.ToEmpty(input[Constants.FIELD_MAILDISTSETTING_SCHEDULE_SECOND]);
		this.ScheduleSecond = (string.IsNullOrEmpty(second)) ? null : second;
	}
	#endregion

	#region プロパティ
	/// <summary>識別ID</summary>
	public string DeptId
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_DEPT_ID]; }
		set { this.DataSource[Constants.FIELD_MAILDISTSETTING_DEPT_ID] = value; }
	}
	/// <summary>メール配信設定ID</summary>
	public string MaildistId
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_MAILDIST_ID]; }
		set { this.DataSource[Constants.FIELD_MAILDISTSETTING_MAILDIST_ID] = value; }
	}
	/// <summary>メール配信設定名</summary>
	public string MaildistName
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_MAILDIST_NAME]; }
		set { this.DataSource[Constants.FIELD_MAILDISTSETTING_MAILDIST_NAME] = value; }
	}
	/// <summary>ステータス</summary>
	public string Status
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_STATUS]; }
		set { this.DataSource[Constants.FIELD_MAILDISTSETTING_STATUS] = value; }
	}
	/// <summary>最終集計人数</summary>
	public string LastCount
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_LAST_COUNT]; }
		set { this.DataSource[Constants.FIELD_MAILDISTSETTING_LAST_COUNT] = value; }
	}
	/// <summary>最終エラー除外人数</summary>
	public string LastErrorexceptCount
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_LAST_ERROREXCEPT_COUNT]; }
		set { this.DataSource[Constants.FIELD_MAILDISTSETTING_LAST_ERROREXCEPT_COUNT] = value; }
	}
	/// <summary>最終モバイル除外人数</summary>
	public string LastMobilemailexceptCount
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_LAST_MOBILEMAILEXCEPT_COUNT]; }
		set { this.DataSource[Constants.FIELD_MAILDISTSETTING_LAST_MOBILEMAILEXCEPT_COUNT] = value; }
	}
	/// <summary>最終配信開始日時</summary>
	public string LastDistDate
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_LAST_DIST_DATE]; }
		set { this.DataSource[Constants.FIELD_MAILDISTSETTING_LAST_DIST_DATE] = value; }
	}
	/// <summary>ターゲットID</summary>
	public string TargetId
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_TARGET_ID]; }
		set { this.DataSource[Constants.FIELD_MAILDISTSETTING_TARGET_ID] = value; }
	}
	/// <summary>ターゲット抽出フラグ</summary>
	public string TargetExtractFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_TARGET_EXTRACT_FLG]; }
		set { this.DataSource[Constants.FIELD_MAILDISTSETTING_TARGET_EXTRACT_FLG] = value; }
	}
	/// <summary>ターゲットID2</summary>
	public string TargetId2
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_TARGET_ID2]; }
		set { this.DataSource[Constants.FIELD_MAILDISTSETTING_TARGET_ID2] = value; }
	}
	/// <summary>ターゲット2抽出フラグ</summary>
	public string TargetExtractFlg2
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_TARGET_EXTRACT_FLG2]; }
		set { this.DataSource[Constants.FIELD_MAILDISTSETTING_TARGET_EXTRACT_FLG2] = value; }
	}
	/// <summary>ターゲットID3</summary>
	public string TargetId3
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_TARGET_ID3]; }
		set { this.DataSource[Constants.FIELD_MAILDISTSETTING_TARGET_ID3] = value; }
	}
	/// <summary>ターゲット3抽出フラグ</summary>
	public string TargetExtractFlg3
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_TARGET_EXTRACT_FLG3]; }
		set { this.DataSource[Constants.FIELD_MAILDISTSETTING_TARGET_EXTRACT_FLG3] = value; }
	}
	/// <summary>ターゲットID4</summary>
	public string TargetId4
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_TARGET_ID4]; }
		set { this.DataSource[Constants.FIELD_MAILDISTSETTING_TARGET_ID4] = value; }
	}
	/// <summary>ターゲット4抽出フラグ</summary>
	public string TargetExtractFlg4
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_TARGET_EXTRACT_FLG4]; }
		set { this.DataSource[Constants.FIELD_MAILDISTSETTING_TARGET_EXTRACT_FLG4] = value; }
	}
	/// <summary>ターゲットID5</summary>
	public string TargetId5
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_TARGET_ID5]; }
		set { this.DataSource[Constants.FIELD_MAILDISTSETTING_TARGET_ID5] = value; }
	}
	/// <summary>ターゲット5抽出フラグ</summary>
	public string TargetExtractFlg5
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_TARGET_EXTRACT_FLG5]; }
		set { this.DataSource[Constants.FIELD_MAILDISTSETTING_TARGET_EXTRACT_FLG5] = value; }
	}
	/// <summary>配信除外エラーポイント</summary>
	public string ExceptErrorPoint
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_EXCEPT_ERROR_POINT]; }
		set { this.DataSource[Constants.FIELD_MAILDISTSETTING_EXCEPT_ERROR_POINT] = value; }
	}
	/// <summary>モバイルメール排除フラグ</summary>
	public string ExceptMobilemailFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_EXCEPT_MOBILEMAIL_FLG]; }
		set { this.DataSource[Constants.FIELD_MAILDISTSETTING_EXCEPT_MOBILEMAIL_FLG] = value; }
	}
	/// <summary>メール文章ID</summary>
	public string MailtextId
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_MAILTEXT_ID]; }
		set { this.DataSource[Constants.FIELD_MAILDISTSETTING_MAILTEXT_ID] = value; }
	}
	/// <summary>実行タイミング</summary>
	public string ExecTiming
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_EXEC_TIMING]; }
		set { this.DataSource[Constants.FIELD_MAILDISTSETTING_EXEC_TIMING] = value; }
	}
	/// <summary>スケジュール区分</summary>
	public string ScheduleKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_SCHEDULE_KBN]; }
		set { this.DataSource[Constants.FIELD_MAILDISTSETTING_SCHEDULE_KBN] = value; }
	}
	/// <summary>スケジュール曜日</summary>
	public string ScheduleDayOfWeek
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_SCHEDULE_DAY_OF_WEEK]; }
		set { this.DataSource[Constants.FIELD_MAILDISTSETTING_SCHEDULE_DAY_OF_WEEK] = value; }
	}
	/// <summary>スケジュール日程(年)</summary>
	public string ScheduleYear
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_SCHEDULE_YEAR]; }
		set { this.DataSource[Constants.FIELD_MAILDISTSETTING_SCHEDULE_YEAR] = value; }
	}
	/// <summary>スケジュール日程(月)</summary>
	public string ScheduleMonth
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_SCHEDULE_MONTH]; }
		set { this.DataSource[Constants.FIELD_MAILDISTSETTING_SCHEDULE_MONTH] = value; }
	}
	/// <summary>スケジュール日程(日)</summary>
	public string ScheduleDay
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_SCHEDULE_DAY]; }
		set { this.DataSource[Constants.FIELD_MAILDISTSETTING_SCHEDULE_DAY] = value; }
	}
	/// <summary>スケジュール日程(時)</summary>
	public string ScheduleHour
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_SCHEDULE_HOUR]; }
		set { this.DataSource[Constants.FIELD_MAILDISTSETTING_SCHEDULE_HOUR] = value; }
	}
	/// <summary>スケジュール日程(分)</summary>
	public string ScheduleMinute
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_SCHEDULE_MINUTE]; }
		set { this.DataSource[Constants.FIELD_MAILDISTSETTING_SCHEDULE_MINUTE] = value; }
	}
	/// <summary>スケジュール日程(秒)</summary>
	public string ScheduleSecond
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_SCHEDULE_SECOND]; }
		set { this.DataSource[Constants.FIELD_MAILDISTSETTING_SCHEDULE_SECOND] = value; }
	}
	/// <summary>有効フラグ</summary>
	public string ValidFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_VALID_FLG]; }
		set { this.DataSource[Constants.FIELD_MAILDISTSETTING_VALID_FLG] = value; }
	}
	/// <summary>削除フラグ</summary>
	public string DelFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_DEL_FLG]; }
		set { this.DataSource[Constants.FIELD_MAILDISTSETTING_DEL_FLG] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_MAILDISTSETTING_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_MAILDISTSETTING_DATE_CHANGED] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_MAILDISTSETTING_LAST_CHANGED] = value; }
	}
	#endregion
}