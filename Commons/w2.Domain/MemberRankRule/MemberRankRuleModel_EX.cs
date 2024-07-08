/*
=========================================================================================================
  Module      : 会員ランク付与ルールモデル (MemberRankRuleModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Domain.MemberRankRule
{
	/// <summary>
	/// 会員ランク付与ルールモデル
	/// </summary>
	public partial class MemberRankRuleModel
	{
		#region メソッド

		/// <summary>
		/// スケジュール実行日を取得
		/// </summary>
		/// <returns></returns>
		public DateTime? GetExecScheduleTime()
		{
			SynthesizeExecSchedule();
			return this.ExacScheduleDate;
		}

		/// <summary>
		/// スケジュール実行日時を合成
		/// </summary>
		public void SynthesizeExecSchedule()
		{
			if (this.ExecTiming == Constants.FLG_MEMBERRANKRULE_EXEC_TIMING_SCHEDULE)
			{
				var basisDate = DateTime.Today;
				if ((this.TargetExtractStart != null) && IsFutureDate(this.TargetExtractStart))
				{
					basisDate = (DateTime)this.TargetExtractStart;
				}

				DateTime time;
				switch (this.ScheduleKbn)
				{
					case Constants.FLG_MEMBERRANKRULE_SCHEDULE_KBN_DAY:

						time = new DateTime(
							int.Parse(basisDate.Year.ToString()),
							int.Parse(basisDate.Month.ToString()),
							int.Parse(basisDate.Day.ToString()),
							int.Parse(this.ScheduleHour.ToString()),
							int.Parse(this.ScheduleMinute.ToString()),
							int.Parse(this.ScheduleSecond.ToString()));
						this.ExacScheduleDate = IsFutureDate(time) ? time : time.AddDays(1);
						break;

					case Constants.FLG_MEMBERRANKRULE_SCHEDULE_KBN_WEEK:
						var day = CalcDayOfWeekToDate(basisDate, ConvertDayOfWeekToInt(this.ScheduleDayOfWeek));
						time = new DateTime(
							int.Parse(day.Year.ToString()),
							int.Parse(day.Month.ToString()),
							int.Parse(day.Day.ToString()),
							int.Parse(this.ScheduleHour.ToString()),
							int.Parse(this.ScheduleMinute.ToString()),
							int.Parse(this.ScheduleSecond.ToString()));
						this.ExacScheduleDate = time;
						break;

					case Constants.FLG_MEMBERRANKRULE_SCHEDULE_KBN_MONTH:
						time = new DateTime(
							int.Parse(basisDate.Year.ToString()),
							int.Parse(basisDate.Month.ToString()),
							int.Parse(this.ScheduleDay.ToString()),
							int.Parse(this.ScheduleHour.ToString()),
							int.Parse(this.ScheduleMinute.ToString()),
							int.Parse(this.ScheduleSecond.ToString()));
						this.ExacScheduleDate = IsFutureDate(time) ? time : time.AddMonths(1);
						break;

					case Constants.FLG_MEMBERRANKRULE_SCHEDULE_KBN__ONCE:
						time = new DateTime(
							int.Parse(this.ScheduleYear.ToString()),
							int.Parse(this.ScheduleMonth.ToString()),
							int.Parse(this.ScheduleDay.ToString()),
							int.Parse(this.ScheduleHour.ToString()),
							int.Parse(this.ScheduleMinute.ToString()),
							int.Parse(this.ScheduleSecond.ToString()));
						this.ExacScheduleDate = time;
						break;
				}
			}
		}

		/// <summary>
		/// 曜日をint型に変換
		/// </summary>
		/// <returns>int型の曜日</returns>
		public int ConvertDayOfWeekToInt(string dayOfWeek)
		{
			DayOfWeek tmpDayOfWeek;
			var tmpDayOfWeekValue = 0;
			if (Enum.TryParse(dayOfWeek, out tmpDayOfWeek))
			{
				tmpDayOfWeekValue = (int)tmpDayOfWeek;
			}
			return tmpDayOfWeekValue;
		}

		/// <summary>
		/// 曜日から日時を計算
		/// </summary>
		/// <param name="basisDate">日付データ</param>
		/// <param name="weekOfDate">int型曜日</param>
		/// <returns>日時</returns>
		public DateTime CalcDayOfWeekToDate(DateTime basisDate, int weekOfDate)
		{
			var result = (weekOfDate - (int)basisDate.DayOfWeek);
			var dates = result >= 0 ? basisDate.AddDays(result) : basisDate.AddDays(7 + result);

			return dates;
		}

		/// <summary>
		/// 集計期間の日付を計算
		/// </summary>
		public void CalcTargetExtract()
		{
			var exacDay = this.ExacScheduleDate ?? DateTime.Today.Date;
			var defaultStartDay = new DateTime(2000, 1, 1);

			switch (this.TargetExtractOldRankFlg)
			{
				case Constants.FLG_MEMBERRANKRULE_TARGET_EXTRACT_OLD_RANK_FLG_ON:
				
					switch (this.TargetExtractType)
					{
						case Constants.FLG_MEMBERRANKRULE_TARGET_EXTRACT_TYPE_DURING:
							this.AdjustedTargetExtractStart = this.TargetExtractStart ?? defaultStartDay;
							this.AdjustedTargetExtractEnd = this.TargetExtractEnd ?? exacDay;
							break;
						
						case Constants.FLG_MEMBERRANKRULE_TARGET_EXTRACT_TYPE_DAYSAGO:
							if (this.TargetExtractDaysAgo != null)
							{
								this.AdjustedTargetExtractStart =
									exacDay.AddDays((int)((-1) * this.TargetExtractDaysAgo));
							}
							this.AdjustedTargetExtractEnd = exacDay;
							break;
					}
					break;
			
				case Constants.FLG_MEMBERRANKRULE_TARGET_EXTRACT_OLD_RANK_FLG_OFF:
			
					switch (this.TargetExtractType)
					{
						case Constants.FLG_MEMBERRANKRULE_TARGET_EXTRACT_TYPE_DURING:
							this.AdjustedTargetExtractStart =
								((this.TargetExtractStart ?? defaultStartDay).CompareTo(this.ChangeRankDate) < 0)
									? this.ChangeRankDate
									: this.TargetExtractStart ?? defaultStartDay;
							this.AdjustedTargetExtractEnd = this.TargetExtractEnd ?? exacDay;
							break;
			
						case Constants.FLG_MEMBERRANKRULE_TARGET_EXTRACT_TYPE_DAYSAGO:
							if (this.TargetExtractDaysAgo != null)
							{
								var agoDay = exacDay.AddDays((int)((-1) * this.TargetExtractDaysAgo));
								this.AdjustedTargetExtractStart = (agoDay.CompareTo(this.ChangeRankDate) < 0)
									? this.ChangeRankDate
									: agoDay;
							}
							this.AdjustedTargetExtractEnd = exacDay;
							break;
					}
					break;
			}
		}

		/// <summary>
		/// 日時が未来かどうか
		/// </summary>
		/// <param name="date">日時</param>
		/// <returns>真偽</returns>
		public bool IsFutureDate(DateTime? date)
		{
			var result = DateTime.Now.CompareTo(date) < 0;
			return result;
		}

		#endregion

		#region プロパティ
		/// <summary>現在の会員ランクになった日付</summary>
		public DateTime ChangeRankDate { get; set; }
		/// <summary>スケジュール実行日</summary>
		public DateTime? ExacScheduleDate { get; set; }
		/// <summary>調整後の抽出条件集計期間開始日</summary>
		public DateTime AdjustedTargetExtractStart { get; set; }
		/// <summary>調整後の抽出条件集計期間終了日</summary>
		public DateTime AdjustedTargetExtractEnd { get; set; }
		#endregion
	}
}
