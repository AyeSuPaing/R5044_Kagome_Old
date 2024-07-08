/*
=========================================================================================================
  Module      : 会員ランク付与ルール入力クラス (MemberRankRuleInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using w2.App.Common.Input;
using w2.Domain.MemberRankRule;

/// <summary>
/// 会員ランク付与ルール入力クラス
/// </summary>
public class MemberRankRuleInput : InputBase<MemberRankRuleModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public MemberRankRuleInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public MemberRankRuleInput(MemberRankRuleModel model)
		: this()
	{
		this.MemberRankRuleId = model.MemberRankRuleId;
		this.MemberRankRuleName = model.MemberRankRuleName;
		this.Status = model.Status;
		this.LastCount = model.LastCount;
		this.LastExecDate = (model.LastExecDate != null) ? model.LastExecDate.ToString() : null;
		this.TargetExtractType = model.TargetExtractType;
		this.TargetExtractStart = (model.TargetExtractStart != null) ? model.TargetExtractStart.ToString() : null;
		this.TargetExtractEnd = (model.TargetExtractEnd != null) ? model.TargetExtractEnd.ToString() : null;
		this.TargetExtractDaysAgo = (model.TargetExtractDaysAgo != null) ? model.TargetExtractDaysAgo.ToString() : null;
		this.TargetExtractTotalPriceFrom = (model.TargetExtractTotalPriceFrom != null) ? model.TargetExtractTotalPriceFrom.ToString() : null;
		this.TargetExtractTotalPriceTo = (model.TargetExtractTotalPriceTo != null) ? model.TargetExtractTotalPriceTo.ToString() : null;
		this.TargetExtractTotalCountFrom = (model.TargetExtractTotalCountFrom != null) ? model.TargetExtractTotalCountFrom.ToString() : null;
		this.TargetExtractTotalCountTo = (model.TargetExtractTotalCountTo != null) ? model.TargetExtractTotalCountTo.ToString() : null;
		this.TargetExtractOldRankFlg = model.TargetExtractOldRankFlg;
		this.RankChangeType = model.RankChangeType;
		this.RankChangeRankId = model.RankChangeRankId;
		this.MailId = model.MailId;
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
		this.LastChanged = model.LastChanged;
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override MemberRankRuleModel CreateModel()
	{
		var model = new MemberRankRuleModel
		{
			MemberRankRuleId = this.MemberRankRuleId,
			MemberRankRuleName = this.MemberRankRuleName,
			Status = this.Status,
			LastCount = this.LastCount,
			LastExecDate = (this.LastExecDate != null) ? DateTime.Parse(this.LastExecDate) : (DateTime?)null,
			TargetExtractType = this.TargetExtractType,
			TargetExtractStart = (this.TargetExtractStart != null) ? DateTime.Parse(this.TargetExtractStart) : (DateTime?)null,
			TargetExtractEnd = (this.TargetExtractEnd != null) ? DateTime.Parse(this.TargetExtractEnd) : (DateTime?)null,
			TargetExtractDaysAgo = (this.TargetExtractDaysAgo != null) ? int.Parse(this.TargetExtractDaysAgo) : (int?)null,
			TargetExtractTotalPriceFrom = (this.TargetExtractTotalPriceFrom != null) ? decimal.Parse(this.TargetExtractTotalPriceFrom) : (decimal?)null,
			TargetExtractTotalPriceTo = (this.TargetExtractTotalPriceTo != null) ? decimal.Parse(this.TargetExtractTotalPriceTo) : (decimal?)null,
			TargetExtractTotalCountFrom = (this.TargetExtractTotalCountFrom != null) ? int.Parse(this.TargetExtractTotalCountFrom) : (int?)null,
			TargetExtractTotalCountTo = (this.TargetExtractTotalCountTo != null) ? int.Parse(this.TargetExtractTotalCountTo) : (int?)null,
			TargetExtractOldRankFlg = this.TargetExtractOldRankFlg,
			RankChangeType = this.RankChangeType,
			RankChangeRankId = this.RankChangeRankId,
			MailId = this.MailId,
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
		};
		return model;
	}

	/// <summary>
	/// スケジュール入力内容をセット
	/// </summary>
	/// <param name="input">入力内容</param>
	public void SetSchedule(Hashtable input)
	{
		this.ExecTiming = StringUtility.ToEmpty(input[Constants.FIELD_MEMBERRANKRULE_EXEC_TIMING]);
		this.ScheduleKbn = StringUtility.ToEmpty(input[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_KBN]);
		this.ScheduleDayOfWeek = StringUtility.ToEmpty(input[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_DAY_OF_WEEK]);
		var year = StringUtility.ToEmpty(input[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_YEAR]);
		this.ScheduleYear = (string.IsNullOrEmpty(year)) ? null : year;
		var mohth = StringUtility.ToEmpty(input[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_MONTH]);
		this.ScheduleMonth = (string.IsNullOrEmpty(mohth)) ? null : mohth;
		var day = StringUtility.ToEmpty(input[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_DAY]);
		this.ScheduleDay = (string.IsNullOrEmpty(day)) ? null : day;
		var hour = StringUtility.ToEmpty(input[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_HOUR]);
		this.ScheduleHour = (string.IsNullOrEmpty(hour)) ? null : hour;
		var minute = StringUtility.ToEmpty(input[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_MINUTE]);
		this.ScheduleMinute = (string.IsNullOrEmpty(minute)) ? null : minute;
		var second = StringUtility.ToEmpty(input[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_SECOND]);
		this.ScheduleSecond = (string.IsNullOrEmpty(second)) ? null : second;
	}
	#endregion

	#region プロパティ
	/// <summary>ランク付与ルールID</summary>
	public string MemberRankRuleId
	{
		get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_ID]; }
		set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_ID] = value; }
	}
	/// <summary>ランク付与ルール名</summary>
	public string MemberRankRuleName
	{
		get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_NAME]; }
		set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_NAME] = value; }
	}
	/// <summary>ステータス</summary>
	public string Status
	{
		get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_STATUS]; }
		set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_STATUS] = value; }
	}
	/// <summary>最終付与人数</summary>
	public string LastCount
	{
		get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_LAST_COUNT]; }
		set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_LAST_COUNT] = value; }
	}
	/// <summary>最終付与日時</summary>
	public string LastExecDate
	{
		get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_LAST_EXEC_DATE]; }
		set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_LAST_EXEC_DATE] = value; }
	}
	/// <summary>抽出条件集計期間指定</summary>
	public string TargetExtractType
	{
		get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TYPE]; }
		set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TYPE] = value; }
	}
	/// <summary>抽出条件集計期間開始日</summary>
	public string TargetExtractStart
	{
		get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_START]; }
		set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_START] = value; }
	}
	/// <summary>抽出条件集計期間終了日</summary>
	public string TargetExtractEnd
	{
		get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_END]; }
		set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_END] = value; }
	}
	/// <summary>抽出条件集計期間前日指定</summary>
	public string TargetExtractDaysAgo
	{
		get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_DAYS_AGO]; }
		set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_DAYS_AGO] = value; }
	}
	/// <summary>抽出条件合計購入金額範囲(From)</summary>
	public string TargetExtractTotalPriceFrom
	{
		get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_PRICE_FROM]; }
		set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_PRICE_FROM] = value; }
	}
	/// <summary>抽出条件合計購入金額範囲(To)</summary>
	public string TargetExtractTotalPriceTo
	{
		get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_PRICE_TO]; }
		set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_PRICE_TO] = value; }
	}
	/// <summary>抽出条件合計購入回数範囲(From)</summary>
	public string TargetExtractTotalCountFrom
	{
		get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_COUNT_FROM]; }
		set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_COUNT_FROM] = value; }
	}
	/// <summary>抽出条件合計購入回数範囲(To)</summary>
	public string TargetExtractTotalCountTo
	{
		get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_COUNT_TO]; }
		set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_COUNT_TO] = value; }
	}
	/// <summary>抽出時の旧ランク情報抽出判定</summary>
	public string TargetExtractOldRankFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_OLD_RANK_FLG]; }
		set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_OLD_RANK_FLG] = value; }
	}
	/// <summary>ランク付与方法</summary>
	public string RankChangeType
	{
		get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_RANK_CHANGE_TYPE]; }
		set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_RANK_CHANGE_TYPE] = value; }
	}
	/// <summary>指定付与ランクID</summary>
	public string RankChangeRankId
	{
		get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_RANK_CHANGE_RANK_ID]; }
		set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_RANK_CHANGE_RANK_ID] = value; }
	}
	/// <summary>メールテンプレートID</summary>
	public string MailId
	{
		get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_MAIL_ID]; }
		set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_MAIL_ID] = value; }
	}
	/// <summary>実行タイミング</summary>
	public string ExecTiming
	{
		get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_EXEC_TIMING]; }
		set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_EXEC_TIMING] = value; }
	}
	/// <summary>スケジュール区分</summary>
	public string ScheduleKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_KBN]; }
		set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_KBN] = value; }
	}
	/// <summary>スケジュール曜日</summary>
	public string ScheduleDayOfWeek
	{
		get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_DAY_OF_WEEK]; }
		set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_DAY_OF_WEEK] = value; }
	}
	/// <summary>スケジュール日程(年)</summary>
	public string ScheduleYear
	{
		get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_YEAR]; }
		set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_YEAR] = value; }
	}
	/// <summary>スケジュール日程(月)</summary>
	public string ScheduleMonth
	{
		get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_MONTH]; }
		set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_MONTH] = value; }
	}
	/// <summary>スケジュール日程(日)</summary>
	public string ScheduleDay
	{
		get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_DAY]; }
		set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_DAY] = value; }
	}
	/// <summary>スケジュール日程(時)</summary>
	public string ScheduleHour
	{
		get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_HOUR]; }
		set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_HOUR] = value; }
	}
	/// <summary>スケジュール日程(分)</summary>
	public string ScheduleMinute
	{
		get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_MINUTE]; }
		set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_MINUTE] = value; }
	}
	/// <summary>スケジュール日程(秒)</summary>
	public string ScheduleSecond
	{
		get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_SECOND]; }
		set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_SECOND] = value; }
	}
	/// <summary>有効フラグ</summary>
	public string ValidFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_VALID_FLG]; }
		set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_VALID_FLG] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_DATE_CHANGED] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_LAST_CHANGED] = value; }
	}
	#endregion
}
