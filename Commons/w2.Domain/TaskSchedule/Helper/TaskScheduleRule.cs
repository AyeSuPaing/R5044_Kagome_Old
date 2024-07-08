/*
=========================================================================================================
  Module      : タスクスレジュールルール(TaskScheduleRule.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.Common.Util;
using w2.Domain.Point;
using w2.Domain.Coupon;
using w2.Domain.MailDistSetting;
using w2.Domain.MemberRankRule;
using w2.Domain.OrderWorkflowScenarioSetting;
using w2.Domain.TargetList;

namespace w2.Domain.TaskSchedule.Helper
{
	/// <summary>
	/// タスクスケジュールルール
	/// </summary>
	public class TaskScheduleRule
	{
		private const string FLG_EXEC_TIMING_SCHEDULE = Constants.FLG_POINTRULESCHEDULE_EXEC_TIMING_SCHEDULE;
		private const string FLG_SCHEDULE_KBN_DAY = Constants.FLG_POINTRULESCHEDULE_SCHEDULE_KBN_DAY;
		private const string FLG_SCHEDULE_KBN_WEEK = Constants.FLG_POINTRULESCHEDULE_SCHEDULE_KBN_WEEK;
		private const string FLG_SCHEDULE_KBN_MONTH = Constants.FLG_POINTRULESCHEDULE_SCHEDULE_KBN_MONTH;
		private const string FLG_SCHEDULE_KBN_ONCE = Constants.FLG_POINTRULESCHEDULE_SCHEDULE_KBN_ONCE;

		/// <summary>
		/// コンストラクタ(ポイントルール)
		/// </summary>
		/// <param name="model">ポイントルールスケジュールモデル</param>
		public TaskScheduleRule(PointRuleScheduleModel model)
		{
			this.ExecTiming = model.ExecTiming;
			this.ScheduleKbn = model.ScheduleKbn;
			this.ScheduleDayOfWeek = model.ScheduleDayOfWeek;
			this.ScheduleYear = model.ScheduleYear;
			this.ScheduleMonth = model.ScheduleMonth;
			this.ScheduleDay = model.ScheduleDay;
			this.ScheduleHour = model.ScheduleHour;
			this.ScheduleMinute = model.ScheduleMinute;
			this.ScheduleSecond = model.ScheduleSecond;
		}

		/// <summary>
		/// コンストラクタ（クーポン）
		/// </summary>
		/// <param name="model">クーポンスケジュールモデル</param>
		public TaskScheduleRule(CouponScheduleModel model)
		{
			this.ExecTiming = model.ExecTiming;
			this.ScheduleKbn = model.ScheduleKbn;
			this.ScheduleDayOfWeek = model.ScheduleDayOfWeek;
			this.ScheduleYear = model.ScheduleYear;
			this.ScheduleMonth = model.ScheduleMonth;
			this.ScheduleDay = model.ScheduleDay;
			this.ScheduleHour = model.ScheduleHour;
			this.ScheduleMinute = model.ScheduleMinute;
			this.ScheduleSecond = model.ScheduleSecond;
		}

		/// <summary>
		/// コンストラクタ（受注ワークフローシナリオ）
		/// </summary>
		/// <param name="model">受注ワークフローシナリオ設定モデル</param>
		public TaskScheduleRule(OrderWorkflowScenarioSettingModel model)
		{
			this.ExecTiming = model.ExecTiming;
			this.ScheduleKbn = model.ScheduleKbn;
			this.ScheduleDayOfWeek = model.ScheduleDayOfWeek;
			this.ScheduleYear = model.ScheduleYear;
			this.ScheduleMonth = model.ScheduleMonth;
			this.ScheduleDay = model.ScheduleDay;
			this.ScheduleHour = model.ScheduleHour;
			this.ScheduleMinute = model.ScheduleMinute;
			this.ScheduleSecond = model.ScheduleSecond;
		}

		/// <summary>
		/// コンストラクタ（ターゲットリスト）
		/// </summary>
		/// <param name="model">ターゲットリストスケジュールモデル</param>
		public TaskScheduleRule(TargetListModel model)
		{
			this.ExecTiming = model.ExecTiming;
			this.ScheduleKbn = model.ScheduleKbn;
			this.ScheduleDayOfWeek = model.ScheduleDayOfWeek;
			this.ScheduleYear = model.ScheduleYear;
			this.ScheduleMonth = model.ScheduleMonth;
			this.ScheduleDay = model.ScheduleDay;
			this.ScheduleHour = model.ScheduleHour;
			this.ScheduleMinute = model.ScheduleMinute;
			this.ScheduleSecond = model.ScheduleSecond;
		}

		/// <summary>
		/// コンストラクタ（メール配信）
		/// </summary>
		/// <param name="model">メール配信スケジュールモデル</param>
		public TaskScheduleRule(MailDistSettingModel model)
		{
			this.ExecTiming = model.ExecTiming;
			this.ScheduleKbn = model.ScheduleKbn;
			this.ScheduleDayOfWeek = model.ScheduleDayOfWeek;
			this.ScheduleYear = model.ScheduleYear;
			this.ScheduleMonth = model.ScheduleMonth;
			this.ScheduleDay = model.ScheduleDay;
			this.ScheduleHour = model.ScheduleHour;
			this.ScheduleMinute = model.ScheduleMinute;
			this.ScheduleSecond = model.ScheduleSecond;
		}

		/// <summary>
		/// コンストラクタ（会員ランクルール）
		/// </summary>
		/// <param name="model">会員ランクルールスケジュールモデル</param>
		public TaskScheduleRule(MemberRankRuleModel model)
		{
			this.ExecTiming = model.ExecTiming;
			this.ScheduleKbn = model.ScheduleKbn;
			this.ScheduleDayOfWeek = model.ScheduleDayOfWeek;
			this.ScheduleYear = model.ScheduleYear;
			this.ScheduleMonth = model.ScheduleMonth;
			this.ScheduleDay = model.ScheduleDay;
			this.ScheduleHour = model.ScheduleHour;
			this.ScheduleMinute = model.ScheduleMinute;
			this.ScheduleSecond = model.ScheduleSecond;
		}

		/// <summary>
		/// スケジュール実行 月単位 を設定時 初回タスクスケジュールが作成可能かチェック 本日日付
		/// </summary>
		/// <returns>タスクスケジュールを作成可能かどうか</returns>
		public bool CheckCanCreateFirstTaskScheduleForScheduleMonth()
		{
			var result = CheckCanCreateFirstTaskScheduleForScheduleMonth(DateTime.Now);
			return result;
		}
		/// <summary>
		/// スケジュール実行 月単位 を設定時 初回タスクスケジュールが作成可能かチェック
		/// </summary>
		/// <param name="dateTime">チェック対象日時</param>
		/// <returns>タスクスケジュールを作成可能かどうか</returns>
		public bool CheckCanCreateFirstTaskScheduleForScheduleMonth(DateTime dateTime)
		{
			// スケジュール実行 月単位ではない場合は作成可能
			if ((this.IsSchedule == false) || (this.IsEveryMonth == false)) return true;

			var result = Validator.IsDate(
				string.Format(
					"{0}-{1}-{2} 00:00:00",
					dateTime.Year.ToString(),
					dateTime.Month.ToString(),
					StringUtility.ToEmpty(this.ScheduleDay)));
			return result;
		}

		/// <summary>実行タイミング</summary>
		public string ExecTiming { get; private set; }
		/// <summary>スケジュール区分</summary>
		public string ScheduleKbn { get; private set; }
		/// <summary>スケジュール曜日</summary>
		public string ScheduleDayOfWeek { get; private set; }
		/// <summary>スケジュール日程(年)</summary>
		public int? ScheduleYear { get; private set; }
		/// <summary>スケジュール日程(月)</summary>
		public int? ScheduleMonth { get; private set; }
		/// <summary>スケジュール日程(日)</summary>
		public int? ScheduleDay { get; private set; }
		/// <summary>スケジュール日程(時)</summary>
		public int? ScheduleHour { get; private set; }
		/// <summary>スケジュール日程(分)</summary>
		public int? ScheduleMinute { get; private set; }
		/// <summary>スケジュール日程(秒)</summary>
		public int? ScheduleSecond { get; private set; }
		/// <summary>スケジュール実行か？</summary>
		public bool IsSchedule { get { return (this.ExecTiming == FLG_EXEC_TIMING_SCHEDULE); } }
		/// <summary>毎日実行？</summary>
		public bool IsEveryDay { get { return (this.ScheduleKbn == FLG_SCHEDULE_KBN_DAY); } }
		/// <summary>毎週実行？</summary>
		public bool IsEveryWeek { get { return (this.ScheduleKbn == FLG_SCHEDULE_KBN_WEEK); } }
		/// <summary>毎月実行？</summary>
		public bool IsEveryMonth { get { return (this.ScheduleKbn == FLG_SCHEDULE_KBN_MONTH); } }
		/// <summary>一度だけ実行？</summary>
		public bool IsOnce { get { return (this.ScheduleKbn == FLG_SCHEDULE_KBN_ONCE); } }
	}
}
