/*
=========================================================================================================
  Module      : 日付入力コントロール(DateTimeInputControl.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using w2.App.Common.Global.Config;
using w2.App.Common.Util;
using w2.App.Common.Web.Page;
using w2.App.Common.Web.WrappedContols;

namespace w2.App.Common.Web.WebCustomControl
{
	/// <summary>
	/// 年月日のそれぞれの項目を分けて、日付を入力するためのコントロール
	/// </summary>
	public partial class DateTimeInputControl : CommonUserControl
	{
		// プルダウンの選択肢変更イベントハンドラー
		public event EventHandler DateTimeSelectedEvent;

		/// <summary>
		/// ページ初期化
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Init(object sender, EventArgs e)
		{
			if (!IsPostBack) DataBind();
		}

		/// <summary>
		/// ページロード
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e)
		{
		}

		/// <summary>
		/// 年リスト作成
		/// </summary>
		/// <returns>年リスト</returns>
		protected ListItem[] CreateYearList()
		{
			var list = new List<ListItem>();

			// ブランク値があれば、ブランクアイテムを追加
			if (this.HasBlankValue) list.Add(new ListItem(this.HasBlankSign ? "----" : "", ""));

			if (this.YearList != null) list.AddRange(this.YearList);

			// 渡した日付の年は年リスト範囲外の場合、この値を年リストに追加
			var selectedYear = m_selectedDate.HasValue ? m_selectedDate.Value.Year.ToString() : "";
			if ((selectedYear != "") && (this.YearList.Any(li => li.Value == selectedYear) == false))
			{
				list.Add(new ListItem(selectedYear, selectedYear));
			}
			return list.ToArray();
		}

		/// <summary>
		/// プルダウンの選択値セット
		/// </summary>
		/// <param name="control">プルダウンコントロール</param>
		/// <param name="value">値</param>
		private void SetSelectedValue(DropDownList control, string value)
		{
			if (control.Items.FindByValue(value) == null)
			{
				var insertIndex = this.HasBlankValue && (control.Items.Count > 0)
					&& (string.IsNullOrEmpty(control.Items[0].Value))
						? 1
						: 0;
				control.Items.Insert(insertIndex, new ListItem(value, value));
			}

			control.SelectedValue = value;
		}

		/// <summary>
		/// 日付時刻のプルダウンの有効有無セット
		/// </summary>
		/// <param name="flag">フラグ</param>
		public void SetEnable(bool flag)
		{
			this.DdlDay.Enabled = this.DdlMonth.Enabled = this.DdlYear.Enabled = flag;
			if (this.HasTime)
			{
				this.DdlHour.Enabled = this.DdlMinute.Enabled = this.DdlSecond.Enabled = flag;
			}
		}

		/// <summary>
		/// 日付セット
		/// </summary>
		/// <param name="date">日付</param>
		public void SetDate(DateTime date)
		{
			this.Year = date.Year.ToString();
			this.Month = date.Month.ToString(this.MonthFormat);
			this.Day = date.Day.ToString(this.DayFormat);
			if (this.HasTime)
			{
				this.Hour = date.Hour.ToString("00");
				this.Minute = date.Minute.ToString("00");
				this.Second = date.Second.ToString("00");
			}

			m_selectedDate = date;
		}
		/// <summary>
		/// 日付セット
		/// </summary>
		/// <param name="year">年</param>
		/// <param name="month">月</param>
		/// <param name="day">日付</param>
		/// <param name="hour">時</param>
		/// <param name="minute">分</param>
		/// <param name="second">秒</param>
		public void SetDate(int year, int month, int day, int hour, int minute, int second)
		{
			this.Year = year.ToString();
			this.Month = month.ToString(this.MonthFormat);
			this.Day = day.ToString(this.DayFormat);
			if (this.HasTime)
			{
				this.Hour = hour.ToString("00");
				this.Minute = minute.ToString("00");
				this.Second = second.ToString("00");
			}

			var checkDatetime = Validator.IsDate(
				string.Format(
					"{0}-{1}-{2} 00:00:00",
					this.Year,
					this.Month,
					this.Day));

			m_selectedDate = (checkDatetime)
				? new DateTime(year, month, day, hour, minute, second)
				: new DateTime(year, month, DateTime.DaysInMonth(year, month), hour, minute, second);
		}

		/// <summary>
		/// 日付選択のイベント
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnDateTimeSelected(EventArgs e)
		{
			if (DateTimeSelectedEvent != null) DateTimeSelectedEvent(this, e);
		}

		/// <summary>
		/// プルダウンの選択肢変更のイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void ddlDateTime_SelectedIndexChanged(object sender, EventArgs e)
		{
			DateTime date;
			this.AlertPastDate
				&= (DateTime.TryParse(this.DateString, out date) && (date < DateTime.Today));

			OnDateTimeSelected(EventArgs.Empty);
		}

		/// <summary>
		/// Set value
		/// </summary>
		/// <param name="selectedDateTime">Selected Date Time</param>
		private void SetValue(string selectedDateTime)
		{
			var date = selectedDateTime.Split(' ')[0];
			var time = ((selectedDateTime.Split(' ').Length > 1) ? selectedDateTime.Split(' ')[1] : string.Empty);
			if (date.Split('/').Length > 2)
			{
				this.Year = date.Split('/')[0];
				this.Month = date.Split('/')[1];
				this.Day = date.Split('/')[2];
			}
			if (this.HasTime && (time.Split(':').Length > 2))
			{
				this.Hour = time.Split(':')[0];
				this.Minute = time.Split(':')[1];
				this.Second = time.Split(':')[2];
			}

			if (w2.App.Common.Util.Validator.IsDate(selectedDateTime))
				m_selectedDate = DateTime.Parse(selectedDateTime);
		}

		#region プロパティ
		/// <summary> 日付形式タイプ </summary>
		private string DateFormatType
		{
			get
			{
				// グローバル対応なしの場合、空を戻す
				if (Constants.GLOBAL_OPTION_ENABLE == false) return string.Empty;
				return GlobalConfigUtil.GetDateTimeFormatText(
					Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE,
					DateTimeUtility.FormatType.General).ToLower();
			}
		}
		/// <summary> 空の選択肢があるか </summary>
		public bool HasBlankValue { get; set; }
		/// <summary> 空の選択肢のテキストがあるか </summary>
		public bool HasBlankSign { get; set; }
		/// <summary> 時刻部分があるか </summary>
		public bool HasTime { get; set; }
		/// <summary>月・日の0埋めをしない</summary>
		public bool ZeroSuppress { get; set; }
		/// <summary>月のフォーマット</summary>
		public string MonthFormat
		{
			get { return this.ZeroSuppress ? "0" : "00"; }
		}
		/// <summary>日のフォーマット</summary>
		public string DayFormat
		{
			get { return this.ZeroSuppress ? "0" : "00"; }
		}
		/// <summary> 過去日判定するか </summary>
		public bool AlertPastDate { get; set; }
		/// <summary> 選択日付セッター </summary>
		public DateTime? SelectedDate
		{
			set { m_selectedDate = value; }
		}
		private DateTime? m_selectedDate;
		/// <summary> コントロールが自動ポストバックか </summary>
		public bool ControlAutoPostBack
		{
			get { return m_autoPostBack; }
			set { m_autoPostBack = value; }
		}
		private bool m_autoPostBack = false;
		/// <summary> 年リスト </summary>
		public ListItem[] YearList { get; set; }
		/// <summary> 月リスト </summary>
		protected ListItem[] MonthList
		{
			get
			{
				var list = new List<ListItem>();
				if (this.HasBlankValue) list.Add(new ListItem(this.HasBlankSign ? "--" : "", ""));
				list.AddRange(DateTimeUtility.GetMonthListItem("00", this.MonthFormat));
				return list.ToArray();
			}
		}
		/// <summary> 日リスト </summary>
		protected ListItem[] DayList
		{
			get
			{
				var list = new List<ListItem>();
				if (this.HasBlankValue) list.Add(new ListItem(this.HasBlankSign ? "--" : "", ""));
				list.AddRange(DateTimeUtility.GetDayListItem("00", this.DayFormat));
				return list.ToArray();
			}
		}
		/// <summary> 時刻の時リスト </summary>
		protected ListItem[] HourList
		{
			get
			{
				var list = new List<ListItem>();
				if (this.HasBlankValue) list.Add(new ListItem(this.HasBlankSign ? "--" : "", ""));
				list.AddRange(DateTimeUtility.GetHourListItem("00"));
				return list.ToArray();
			}
		}
		/// <summary> 時刻の分リスト </summary>
		protected ListItem[] MinuteList
		{
			get
			{
				var list = new List<ListItem>();
				if (this.HasBlankValue) list.Add(new ListItem(this.HasBlankSign ? "--" : "", ""));
				list.AddRange(DateTimeUtility.GetMinuteListItem("00"));
				return list.ToArray();
			}
		}
		/// <summary> 時刻の秒リスト </summary>
		protected ListItem[] SecondList
		{
			get
			{
				var list = new List<ListItem>();
				if (this.HasBlankValue) list.Add(new ListItem(this.HasBlankSign ? "--" : "", ""));
				list.AddRange(DateTimeUtility.GetSecondListItem("00"));
				return list.ToArray();
			}
		}
		/// <summary> 日付の第1部分のデータソース </summary>
		protected ListItem[] DataSourceDatePart1
		{
			get { return (this.IsMdyFormat ? this.MonthList : (this.IsDmyFormat ? this.DayList : CreateYearList())); }
		}
		/// <summary> 日付の第２部分のデータソース </summary>
		protected ListItem[] DataSourceDatePart2
		{
			get { return (this.IsMdyFormat ? this.DayList : this.MonthList); }
		}
		/// <summary> 日付の第３部分のデータソース </summary>
		protected ListItem[] DataSourceDatePart3
		{
			get { return ((this.IsMdyFormat || this.IsDmyFormat) ? CreateYearList() : this.DayList); }
		}
		/// <summary> 日付の年 </summary>
		public string Year
		{
			get { return this.DdlYear.SelectedValue; }
			set { SetSelectedValue(this.DdlYear, value); }
		}
		/// <summary> 日付の月 </summary>
		public string Month
		{
			get { return this.DdlMonth.SelectedValue; }
			set { SetSelectedValue(this.DdlMonth, value); }
		}
		/// <summary> 日付の日 </summary>
		public string Day
		{
			get { return this.DdlDay.SelectedValue; }
			set { SetSelectedValue(this.DdlDay, value); }
		}
		/// <summary> 時刻の時 </summary>
		public string Hour
		{
			get { return this.DdlHour.SelectedValue; }
			set { this.DdlHour.SelectedValue = value; }
		}
		/// <summary> 時刻の分 </summary>
		public string Minute
		{
			get { return this.DdlMinute.SelectedValue; }
			set { this.DdlMinute.SelectedValue = value; }
		}
		/// <summary> 時刻の秒 </summary>
		public string Second
		{
			get { return this.DdlSecond.SelectedValue; }
			set { this.DdlSecond.SelectedValue = value; }
		}
		/// <summary> 「MDY」のフォーマットか </summary>
		private bool IsMdyFormat
		{
			get { return (this.DateFormatType == "mdy"); }
		}
		/// <summary> 「DMY」のフォーマットか </summary>
		private bool IsDmyFormat
		{
			get { return (this.DateFormatType == "dmy"); }
		}
		/// <summary> 日付の第1部分の選択値 </summary>
		protected string SelectedValueDatePart1
		{
			get
			{
				if (m_selectedDate.HasValue == false) return null;
				return (this.IsMdyFormat
					? m_selectedDate.Value.Month.ToString(this.MonthFormat)
					: (this.IsDmyFormat
						? m_selectedDate.Value.Day.ToString(this.DayFormat)
						: m_selectedDate.Value.Year.ToString()));
			}
		}
		/// <summary> 日付の第２部分の選択値 </summary>
		protected string SelectedValueDatePart2
		{
			get
			{
				if (m_selectedDate.HasValue == false) return null;
				return (this.IsMdyFormat
					? m_selectedDate.Value.Day.ToString(this.DayFormat)
					: m_selectedDate.Value.Month.ToString(this.MonthFormat));
			}
		}
		/// <summary> 日付の第３部分の選択値 </summary>
		protected string SelectedValueDatePart3
		{
			get
			{
				if (m_selectedDate.HasValue == false) return null;
				return ((this.IsMdyFormat || this.IsDmyFormat)
					? m_selectedDate.Value.Year.ToString()
					: m_selectedDate.Value.Day.ToString(this.DayFormat));
			}
		}
		/// <summary> 時項目の選択値 </summary>
		protected string SelectedValueHour
		{
			get { return (m_selectedDate.HasValue ? m_selectedDate.Value.Hour.ToString("00") : null); }
		}
		/// <summary> 分項目の選択値 </summary>
		protected string SelectedValueMinute
		{
			get { return (m_selectedDate.HasValue ? m_selectedDate.Value.Minute.ToString("00") : null); }
		}
		/// <summary> 秒項目の選択値 </summary>
		protected string SelectedValueSecond
		{
			get { return (m_selectedDate.HasValue ? m_selectedDate.Value.Second.ToString("00") : null); }
		}
		/// <summary> 「yyyy/MM/dd」の日付文字列 </summary>
		public string DateString
		{
			get
			{
				var date = string.Format("{0}/{1}/{2}", this.Year, this.Month, this.Day);
				return (Validator.IsDate(date)) ? date : string.Empty;
			}
		}
		/// <summary> 「yyyy/MM/dd HH:mm:ss」の日付時刻文字列 </summary>
		public string DateTimeString
		{
			get
			{
				var date = string.Format("{0} {1}:{2}:{3}", this.DateString, this.Hour, this.Minute, this.Second);
				return (date.Length > 3) ? date : string.Empty;
			}
		}
		/// <summary> 「yyyy/MM/dd HH:mm:ss.fff」の日付を最後時刻に設定（23:59:59.997） </summary>
		public string DateAndLastTimeString
		{
			get
			{
				var date = (string.IsNullOrEmpty(this.DateString) == false)
					? string.Format("{0} 23:59:59.997", this.DateString)
					: string.Empty;
				return date;
			}
		}
		/// <summary> 年のプルダウン </summary>
		public DropDownList DdlYear
		{
			get
			{
				return (this.IsMdyFormat || this.IsDmyFormat)
					? this.WddlDatePart3.InnerControl
					: this.WddlDatePart1.InnerControl;
			}
		}
		/// <summary> 月のプルダウン </summary>
		public DropDownList DdlMonth
		{
			get { return this.IsMdyFormat ? this.WddlDatePart1.InnerControl : this.WddlDatePart2.InnerControl; }
		}
		/// <summary> 日のプルダウン </summary>
		public DropDownList DdlDay
		{
			get
			{
				return this.IsMdyFormat
					? this.WddlDatePart2.InnerControl
					: (this.IsDmyFormat ? this.WddlDatePart1.InnerControl : this.WddlDatePart3.InnerControl);
			}
		}
		/// <summary> 時のプルダウン </summary>
		public DropDownList DdlHour
		{
			get { return this.WddlTimeHour.InnerControl; }
		}
		/// <summary> 分のプルダウン </summary>
		public DropDownList DdlMinute
		{
			get { return this.WddlTimeMinute.InnerControl; }
		}
		/// <summary> 秒のプルダウン </summary>
		public DropDownList DdlSecond
		{
			get { return this.WddlTimeSecond.InnerControl; }
		}
		/// <summary> 日のスラッシュ </summary>
		public HtmlGenericControl DaySeparator
		{
			get { return this.IsDmyFormat ? this.WhgcSeparator1.InnerControl : this.WhgcSeparator2.InnerControl; }
		}
		/// <summary> 年月のスラッシュ </summary>
		public HtmlGenericControl MonthYearSeparator
		{
			get { return this.IsDmyFormat ? this.WhgcSeparator2.InnerControl : this.WhgcSeparator1.InnerControl; }
		}
		/// <summary> 「日」文字 </summary>
		public HtmlGenericControl DayString
		{
			get { return this.WhgcDayString.InnerControl; }
		}
		#region ラップコントロール定義
		/// <summary> 日付第1部分 </summary>
		private WrappedDropDownList WddlDatePart1
		{
			get { return GetWrappedControl<WrappedDropDownList>("ddlDatePart1"); }
		}
		/// <summary> 日付第２部分 </summary>
		private WrappedDropDownList WddlDatePart2
		{
			get { return GetWrappedControl<WrappedDropDownList>("ddlDatePart2"); }
		}
		/// <summary> 日付第３部分 </summary>
		private WrappedDropDownList WddlDatePart3
		{
			get { return GetWrappedControl<WrappedDropDownList>("ddlDatePart3"); }
		}
		/// <summary> 時刻の時 </summary>
		private WrappedDropDownList WddlTimeHour
		{
			get { return GetWrappedControl<WrappedDropDownList>("ddlTimeHour"); }
		}
		/// <summary> 時刻の分 </summary>
		private WrappedDropDownList WddlTimeMinute	
		{
			get { return GetWrappedControl<WrappedDropDownList>("ddlTimeMinute"); }
		}
		/// <summary> 時刻の秒 </summary>
		private WrappedDropDownList WddlTimeSecond
		{
			get { return GetWrappedControl<WrappedDropDownList>("ddlTimeSecond"); }
		}
		/// <summary> スラッシュ１ </summary>
		private WrappedHtmlGenericControl WhgcSeparator1
		{
			get { return GetWrappedControl<WrappedHtmlGenericControl>("spSeparator1"); }
		}
		/// <summary> スラッシュ２ </summary>
		private WrappedHtmlGenericControl WhgcSeparator2
		{
			get { return GetWrappedControl<WrappedHtmlGenericControl>("spSeparator2"); }
		}
		/// <summary> 「日」の項目 </summary>
		private WrappedHtmlGenericControl WhgcDayString
		{
			get { return GetWrappedControl<WrappedHtmlGenericControl>("spDayString"); }
		}
		/// <summary> 「指定なし」更新の注意書き </summary>
		public string DateTimeInfoMessage
		{
			get { return (string)this.ViewState["DateTimeInfoMessage"]; }
			set { this.ViewState["DateTimeInfoMessage"] = value; }
		}
		#endregion
		#endregion
	}
}