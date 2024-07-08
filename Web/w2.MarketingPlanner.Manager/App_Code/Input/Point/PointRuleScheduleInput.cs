/*
=========================================================================================================
  Module      : ポイントルールスケジュールテーブル入力クラス (PointRuleScheduleInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.App.Common.Input;
using w2.Domain.Point;

namespace Input.Point
{
	/// <summary>
	/// ポイントルールスケジュールテーブル入力クラス
	/// </summary>
	[Serializable]
	public class PointRuleScheduleInput : InputBase<PointRuleScheduleModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public PointRuleScheduleInput()
		{
			this.Status = Constants.FLG_POINTRULESCHEDULE_STATUS_NORMAL;
			this.LastCount = "";
			this.ScheduleDayOfWeek = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public PointRuleScheduleInput(PointRuleScheduleModel model)
			: this()
		{
			this.PointRuleScheduleId = model.PointRuleScheduleId;
			this.PointRuleScheduleName = model.PointRuleScheduleName;
			this.Status = model.Status;
			this.LastCount = model.LastCount;
			this.LastExecDate = (model.LastExecDate != null) ? model.LastExecDate.ToString() : null;
			this.TargetId = model.TargetId;
			this.TargetExtractFlg = model.TargetExtractFlg;
			this.PointRuleId = model.PointRuleId;
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
		public override PointRuleScheduleModel CreateModel()
		{
			var model = new PointRuleScheduleModel
			{
				PointRuleScheduleId = this.PointRuleScheduleId,
				PointRuleScheduleName = this.PointRuleScheduleName,
				Status = this.Status,
				LastCount = this.LastCount,
				LastExecDate = (string.IsNullOrEmpty(this.LastExecDate) == false) ? DateTime.Parse(this.LastExecDate) : (DateTime?)null,
				TargetId = this.TargetId,
				TargetExtractFlg = this.TargetExtractFlg,
				PointRuleId = this.PointRuleId,
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
		/// 検証
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public string Validate()
		{
			var errorMessage = Validator.Validate("PointRuleSchedule", this.DataSource);
			// 実行タイミングが一回のみの場合に日付入力チェック
			if (this.ScheduleKbn == Constants.FLG_POINTRULESCHEDULE_SCHEDULE_KBN_ONCE)
			{
				errorMessage += Validator.CheckDateError("スケジュール日付", this.ScheduleDate);
			}
			return errorMessage;
		}
		#endregion

		#region プロパティ
		/// <summary>ポイントルールスケジュールID</summary>
		public string PointRuleScheduleId
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULESCHEDULE_POINT_RULE_SCHEDULE_ID]; }
			set { this.DataSource[Constants.FIELD_POINTRULESCHEDULE_POINT_RULE_SCHEDULE_ID] = value; }
		}
		/// <summary>ポイントルールスケジュール名</summary>
		public string PointRuleScheduleName
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULESCHEDULE_POINT_RULE_SCHEDULE_NAME]; }
			set { this.DataSource[Constants.FIELD_POINTRULESCHEDULE_POINT_RULE_SCHEDULE_NAME] = value; }
		}
		/// <summary>ステータス</summary>
		public string Status
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULESCHEDULE_STATUS]; }
			set { this.DataSource[Constants.FIELD_POINTRULESCHEDULE_STATUS] = value; }
		}
		/// <summary>最終付与人数</summary>
		public string LastCount
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULESCHEDULE_LAST_COUNT]; }
			set { this.DataSource[Constants.FIELD_POINTRULESCHEDULE_LAST_COUNT] = value; }
		}
		/// <summary>最終付与日時</summary>
		public string LastExecDate
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULESCHEDULE_LAST_EXEC_DATE]; }
			set { this.DataSource[Constants.FIELD_POINTRULESCHEDULE_LAST_EXEC_DATE] = value; }
		}
		/// <summary>ターゲットID</summary>
		public string TargetId
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULESCHEDULE_TARGET_ID]; }
			set { this.DataSource[Constants.FIELD_POINTRULESCHEDULE_TARGET_ID] = value; }
		}
		/// <summary>ターゲット抽出フラグ</summary>
		public string TargetExtractFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULESCHEDULE_TARGET_EXTRACT_FLG]; }
			set { this.DataSource[Constants.FIELD_POINTRULESCHEDULE_TARGET_EXTRACT_FLG] = value; }
		}
		/// <summary>ポイントルールID</summary>
		public string PointRuleId
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULESCHEDULE_POINT_RULE_ID]; }
			set { this.DataSource[Constants.FIELD_POINTRULESCHEDULE_POINT_RULE_ID] = value; }
		}
		/// <summary>メールテンプレートID</summary>
		public string MailId
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULESCHEDULE_MAIL_ID]; }
			set { this.DataSource[Constants.FIELD_POINTRULESCHEDULE_MAIL_ID] = value; }
		}
		/// <summary>実行タイミング</summary>
		public string ExecTiming
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULESCHEDULE_EXEC_TIMING]; }
			set { this.DataSource[Constants.FIELD_POINTRULESCHEDULE_EXEC_TIMING] = value; }
		}
		/// <summary>スケジュール区分</summary>
		public string ScheduleKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULESCHEDULE_SCHEDULE_KBN]; }
			set { this.DataSource[Constants.FIELD_POINTRULESCHEDULE_SCHEDULE_KBN] = value; }
		}
		/// <summary>スケジュール曜日</summary>
		public string ScheduleDayOfWeek
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULESCHEDULE_SCHEDULE_DAY_OF_WEEK]; }
			set { this.DataSource[Constants.FIELD_POINTRULESCHEDULE_SCHEDULE_DAY_OF_WEEK] = value; }
		}
		/// <summary>スケジュール日程(年)</summary>
		public string ScheduleYear
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULESCHEDULE_SCHEDULE_YEAR]; }
			set { this.DataSource[Constants.FIELD_POINTRULESCHEDULE_SCHEDULE_YEAR] = value; }
		}
		/// <summary>スケジュール日程(月)</summary>
		public string ScheduleMonth
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULESCHEDULE_SCHEDULE_MONTH]; }
			set { this.DataSource[Constants.FIELD_POINTRULESCHEDULE_SCHEDULE_MONTH] = value; }
		}
		/// <summary>スケジュール日程(日)</summary>
		public string ScheduleDay
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULESCHEDULE_SCHEDULE_DAY]; }
			set { this.DataSource[Constants.FIELD_POINTRULESCHEDULE_SCHEDULE_DAY] = value; }
		}
		/// <summary>スケジュール日程(時)</summary>
		public string ScheduleHour
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULESCHEDULE_SCHEDULE_HOUR]; }
			set { this.DataSource[Constants.FIELD_POINTRULESCHEDULE_SCHEDULE_HOUR] = value; }
		}
		/// <summary>スケジュール日程(分)</summary>
		public string ScheduleMinute
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULESCHEDULE_SCHEDULE_MINUTE]; }
			set { this.DataSource[Constants.FIELD_POINTRULESCHEDULE_SCHEDULE_MINUTE] = value; }
		}
		/// <summary>スケジュール日程(秒)</summary>
		public string ScheduleSecond
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULESCHEDULE_SCHEDULE_SECOND]; }
			set { this.DataSource[Constants.FIELD_POINTRULESCHEDULE_SCHEDULE_SECOND] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULESCHEDULE_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_POINTRULESCHEDULE_VALID_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public string DateCreated
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULESCHEDULE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_POINTRULESCHEDULE_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public string DateChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULESCHEDULE_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_POINTRULESCHEDULE_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_POINTRULESCHEDULE_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_POINTRULESCHEDULE_LAST_CHANGED] = value; }
		}
		/// <summary>スケジュール日付（チェック用）</summary>
		public string ScheduleDate
		{
			get { return string.Format("{0}/{1}/{2}", this.ScheduleYear, this.ScheduleMonth, this.ScheduleDay); }
		}
		#endregion
	}
}