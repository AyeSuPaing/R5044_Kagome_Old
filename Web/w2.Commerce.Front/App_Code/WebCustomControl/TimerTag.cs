/*
=========================================================================================================
  Module      : タイマータグ(TimerTag.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace w2.Commerce.Front.WebCustomControl
{
	/// <summary>
	/// タイマータグの属性に日時、曜日を指定することでタグ内の表示、非表示を切り替える
	/// </summary>
	[ParseChildren(false)]
	[PersistChildren(true)]
	[ToolboxData("<{0}:TimerTag runat=server></{0}:TimerTag>")]
	public class TimerTag : WebControl
	{
		//曜日一覧
		private static readonly string[] s_WEEKDAY_LIST = { "sun", "mon", "tue", "wed", "thu", "fri", "sat" };
		// タイマータグ：変換元のフォーマット
		private const string TAG_TIMER_DATE_FORMAT = "yyyyMMddHHmmss";

		/// <summary>
		/// レンダー処理のオーバーライド
		/// </summary>
		/// <param name="writer">HTMLライタ</param>
		protected override void Render(HtmlTextWriter writer)
		{
			RenderContents(writer);
		}

		/// <summary>
		/// コントロール出力
		/// </summary>
		/// <param name="output">HTMLライタ</param>
		protected override void RenderContents(HtmlTextWriter output)
		{
			if (string.IsNullOrEmpty(this.DayOfWeek) == false)
			{
				if (DetermineIfDayOfWeekMatched())
				{
					output.Write(
						WebMessages.GetMessages(WebMessages.ERRMSG_TIMER_TAG_WEEKDAY_SPECIFICATION)
							.Replace("@@ 1 @@", this.DayOfWeek));
					return;
				}
			}

			if ((IsPeriodExact(this.Period) == false) && (string.IsNullOrEmpty(this.Period) == false))
			{
				var dates = StringUtility.ToHankaku(this.Period).Split('-');

				if ((IsDateExact(dates[0], TAG_TIMER_DATE_FORMAT, null) == false)
					|| (IsDateExact(dates[1], TAG_TIMER_DATE_FORMAT, null) == false))
				{
					output.Write(
						WebMessages.GetMessages(WebMessages.ERRMSG_TIMER_TAG_DATE_SPECIFICATION)
							.Replace("@@ 1 @@", this.Period));
					return;
				}
				if (string.IsNullOrEmpty(dates[1]) == false) return;
			}

			base.RenderContents(output);
		}

		/// <summary>
		/// 入力された日付の表示チェック
		/// </summary>
		/// <param name="period">日付</param>
		/// <returns>表示可能な日付か否か</returns>
		private bool IsPeriodExact(string period)
		{
			if (string.IsNullOrEmpty(this.Period)) return true;

			var dates = StringUtility.ToHankaku(period).ToLower().Split('-');
			//配列が1つ以下の場合はperiodにハイフンが設定されていないのでfalseを返す
			if (dates.Length <= 1) return false;
			var referenceDateTime = HttpContext.Current.Session[Constants.SESSION_KEY_REFERENCE_DATETIME];
			var viewTime = (DateTime)(referenceDateTime ?? DateTime.Now);

			if ((IsDateExact(dates[0], TAG_TIMER_DATE_FORMAT, null) == false)
				|| (IsDateExact(dates[1], TAG_TIMER_DATE_FORMAT, null) == false))
			{
				this.IsDisp = true;
				return false;
			}

			var fromDate = (string.IsNullOrEmpty(dates[0]) == false) ? DateTime.Parse(dates[0]) : DateTime.Now;
			var toDate = (string.IsNullOrEmpty(dates[1]) == false) ? DateTime.Parse(dates[1]) : DateTime.Now;

			if (string.IsNullOrEmpty(dates[1]) && (viewTime >= fromDate)) return true;
			if ((string.IsNullOrEmpty(dates[0]) == false) && string.IsNullOrEmpty(dates[1]))
			{
				if (fromDate <= viewTime) return true;
			}
			else if (string.IsNullOrEmpty(dates[0]) && (string.IsNullOrEmpty(dates[1]) == false))
			{
				if (toDate >= viewTime) return true;
			}
			else if ((string.IsNullOrEmpty(dates[0]) == false) && (string.IsNullOrEmpty(dates[1]) == false))
			{
				if ((fromDate <= viewTime) && (toDate >= viewTime)) return true;
			}
			return false;
		}

		/// <summary>
		/// 日付フォーマットチェック
		/// </summary>
		/// <param name="date">日付</param>
		/// <param name="format">日付フォーマット</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>有効なフォーマットか否か</returns>
		public static bool IsDateExact(object date, string format, string languageLocaleId = "")
		{
			if (string.IsNullOrEmpty((string)date)) return true;
			var regex = new Regex(@"[/: ]");
			var replacedDate = regex.Replace(date.ToString(), "");
			DateTime dtTmp;
			var result = DateTime.TryParseExact(
				StringUtility.ToEmpty(replacedDate),
				format,
				string.IsNullOrEmpty(languageLocaleId) ? null : new CultureInfo(languageLocaleId),
				DateTimeStyles.None,
				out dtTmp);
			return result;
		}

		/// <summary>
		/// 入力された曜日の表示チェック
		/// </summary>
		/// <param name="dayOfWeek">曜日</param>
		/// <returns>表示可能な曜日か否か</returns>
		private bool IsDayOfWeekExact(string dayOfWeek)
		{
			if (string.IsNullOrEmpty(this.DayOfWeek)) return true;

			var weeks = StringUtility.ToHankaku(dayOfWeek).ToLower().Split(',');

			var referenceDateTime = HttpContext.Current.Session[Constants.SESSION_KEY_REFERENCE_DATETIME];
			var viewTime = (DateTime)(referenceDateTime ?? DateTime.Now);
			var todaysWeekday = new CultureInfo("en-US");

			return weeks.Any(week => week.Trim() == viewTime.ToString("ddd", todaysWeekday).ToLower());
		}

		/// <summary>
		/// 入力された曜日の有効性チェック
		/// </summary>
		/// <returns>有効な曜日か否か</returns>
		private bool DetermineIfDayOfWeekMatched()
		{
			var dates = StringUtility.ToHankaku(this.DayOfWeek).ToLower().Split(',');
			if (dates.Any(date => s_WEEKDAY_LIST.Contains(date.Trim()) == false))
			{
				this.IsDisp = true;
				return true;
			}
			return false;
		}

		/// <summary>
		/// 表示の有効性チェック
		/// </summary>
		/// <returns>有効な表示か否か</returns>
		private bool ShouldShow()
		{
			if (IsPeriodExact(this.Period) && IsDayOfWeekExact(this.DayOfWeek)) return true;
			if (IsDayOfWeekExact(this.DayOfWeek) == false)
			{
				if (DetermineIfDayOfWeekMatched()) return false;
			}
			return false;
		}

		/// <summary>表示期間</summary>
		[Bindable(true)]
		[Category("T")]
		[DefaultValue("")]
		[Localizable(true)]
		public string Period
		{
			get { return (string)ViewState["Period"]; }
			set { ViewState["Period"] = value; }
		}
		/// <summary>表示曜日</summary>
		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue("")]
		[Localizable(true)]
		public string DayOfWeek
		{
			get { return (string)ViewState["DayOfWeek"]; }
			set { ViewState["DayOfWeek"] = value; }
		}
		/// <summary>表示制御</summary>
		public override bool Visible
		{
			get { return (base.Visible) && ShouldShow() || this.IsDisp; }
			set { base.Visible = value; }
		}
		/// <summary>表示条件</summary>
		public bool IsDisp
		{
			get { return (bool)(ViewState["IsDisp"] ?? false); }
			set { ViewState["IsDisp"] = value; }
		}
	}
}