/*
=========================================================================================================
  Module      : Date Time Picker Period Input Control (DateTimePickerPeriodInputControl.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Globalization;
using System.Web.UI.HtmlControls;
using w2.App.Common.Util;
using w2.App.Common.Web.Page;
using w2.App.Common.Web.WrappedContols;

namespace w2.App.Common.Web.WebCustomControl
{
	/// <summary>
	/// Date time picker period input control
	/// </summary>
	public partial class DateTimePickerPeriodInputControl : CommonUserControl
	{
		// プルダウンの選択肢変更イベントハンドラー
		public event EventHandler DateTimeSelectedEvent;

		/// <summary>Format: short date</summary>
		private const string FORMAT_SHORT_DATE = "yyyy/MM/dd";
		/// <summary>Format: short time</summary>
		private const string FORMAT_SHORT_TIME = "HH:mm:ss";

		/// <summary>
		/// ページ初期化
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Init(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				DataBind();
			}
		}

		/// <summary>
		/// ページロード
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e)
		{
			this.HfStartDate.Attributes.Add("class", this.StartDateDataInputHiddenSelector);
			this.HfStartTime.Attributes.Add("class", this.StartTimeDataInputHiddenSelector);
			this.HfEndDate.Attributes.Add("class", this.EndDateDataInputHiddenSelector);
			this.HfEndTime.Attributes.Add("class", this.EndTimeDataInputHiddenSelector);

			if (!IsPostBack)
			{
				SetStartDate(this.StartDate);
				SetEndDate(this.EndDate);

				if (this.IsSetDefaultToday
					&& (this.IsNullStartDateTime == false)
					&& (this.IsNullEndDateTime == false)
					&& (this.StartDate.HasValue == false)
					&& (this.EndDate.HasValue == false))
				{
					SetPeriodDateToday();
				}
			}
		}

		/// <summary>
		/// Set period date
		/// </summary>
		/// <param name="startDateString">The start date is of type string</param>
		/// <param name="endDateString">The end date is of type string</param>
		/// <param name="format">Format used to parse date and time</param>
		public void SetPeriodDate(
			string startDateString,
			string endDateString,
			string format = "yyyyMMddHHmmss")
		{
			// Start date
			DateTime startDate;
			if (DateTime.TryParseExact(startDateString, format, null, DateTimeStyles.None, out startDate))
			{
				SetStartDate(startDate);
			}

			// End date
			DateTime endDate;
			if (DateTime.TryParseExact(endDateString, format, null, DateTimeStyles.None, out endDate))
			{
				SetEndDate(endDate);
			}
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
		/// Set period date
		/// </summary>
		/// <param name="startDate">The start date is of type DateTime</param>
		/// <param name="endDate">The end date is of type DateTime</param>
		public void SetPeriodDate(DateTime startDate, DateTime endDate)
		{
			if (startDate != null)
			{
				SetStartDate(startDate);
			}

			SetEndDate(endDate);
		}

		/// <summary>
		/// Set start date
		/// </summary>
		/// <param name="date">Date</param>
		public void SetStartDate(DateTime? date)
		{
			if (date != null)
			{
				this.HfStartDate.Value = ((DateTime)date).ToString(FORMAT_SHORT_DATE);
				this.HfStartTime.Value = ((DateTime)date).ToString(FORMAT_SHORT_TIME);
				this.StartDate = date;
			}
			else
			{
				this.HfStartDate.Value = null;
				this.HfStartTime.Value = null;
			}
		}

		/// <summary>
		/// Set end date
		/// </summary>
		/// <param name="date">Date</param>
		public void SetEndDate(DateTime? date)
		{
			if (date != null)
			{
				this.HfEndDate.Value = ((DateTime)date).ToString(FORMAT_SHORT_DATE);
				this.HfEndTime.Value = ((DateTime)date).ToString(FORMAT_SHORT_TIME);
				this.EndDate = date;
			}
			else
			{
				this.HfEndDate.Value = null;
				this.HfEndTime.Value = null;
			}
		}

		/// <summary>
		/// Set Period Date Today
		/// </summary>
		/// <param name="date">Date</param>
		public void SetPeriodDateToday()
		{
			SetPeriodDate(DateTime.Today, DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59));
		}

		#region プロパティ
		/// <summary>「yyyy/MM/dd」の日付文字列 (Start)</summary>
		public string StartDateString
		{
			get
			{
				return (Validator.IsDate(this.HfStartDate.Value) ? this.HfStartDate.Value : string.Empty);
			}
		}
		/// <summary>「yyyy/MM/dd HH:mm:ss」の日付時刻文字列 (Start)</summary>
		public string StartDateTimeString
		{
			get
			{
				var date = string.Format("{0} {1}", this.StartDateString, this.HfStartTime.Value);
				return date.Trim();
			}
		}
		/// <summary>「yyyy/MM/dd」の日付文字列 (End)</summary>
		public string EndDateString
		{
			get
			{
				return (Validator.IsDate(this.HfEndDate.Value) ? this.HfEndDate.Value : string.Empty);
			}
		}
		/// <summary>「yyyy/MM/dd HH:mm:ss」の日付時刻文字列 (End)</summary>
		public string EndDateTimeString
		{
			get
			{
				var date = string.Format("{0} {1}", this.EndDateString, this.HfEndTime.Value);
				return date.Trim();
			}
		}
		/// <summary>Start date data input hidden selector</summary>
		public string StartDateDataInputHiddenSelector
		{
			get { return string.Format("{0}-start-date", this.HfStartDate.ClientID); }
		}
		/// <summary>Start time data input hidden selector</summary>
		public string StartTimeDataInputHiddenSelector
		{
			get { return string.Format("{0}-start-time", this.HfStartTime.ClientID); }
		}
		/// <summary>End date data input hidden selector</summary>
		public string EndDateDataInputHiddenSelector
		{
			get { return string.Format("{0}-end-date", this.HfEndDate.ClientID); }
		}
		/// <summary>End time data input hidden selector</summary>
		public string EndTimeDataInputHiddenSelector
		{
			get { return string.Format("{0}-end-time", this.HfEndTime.ClientID); }
		}
		/// <summary>Start date (control)</summary>
		public HtmlInputHidden HfStartDate
		{
			get { return this.WhfStartDate.InnerControl; }
		}
		/// <summary>Start date (control)</summary>
		public HtmlInputHidden HfStartTime
		{
			get { return this.WhfStartTime.InnerControl; }
		}
		/// <summary>End date (control)</summary>
		public HtmlInputHidden HfEndDate
		{
			get { return this.WhfEndDate.InnerControl; }
		}
		/// <summary>End time (control)</summary>
		public HtmlInputHidden HfEndTime
		{
			get { return this.WhfEndTime.InnerControl; }
		}
		/// <summary>Disabled</summary>
		public bool Disabled
		{
			get { return (bool)(ViewState[this.ClientID + "IsDisabled"] ?? false); }
			set { ViewState[this.ClientID + "IsDisabled"] = value; }
		}
		/// <summary>Can show end date picker</summary>
		private bool _canShowEndDatePicker = true;
		public bool CanShowEndDatePicker
		{
			get { return _canShowEndDatePicker; }
			set { _canShowEndDatePicker = value; }
		}
		/// <summary> Is Null Start Date Time </summary>
		private bool _isNullStartDateTime = false;
		public bool IsNullStartDateTime
		{
			get { return _isNullStartDateTime; }
			set { _isNullStartDateTime = value; }
		}
		/// <summary> Is Null End Date Time </summary>
		private bool _isNullEndDateTime = false;
		public bool IsNullEndDateTime
		{
			get { return _isNullEndDateTime; }
			set { _isNullEndDateTime = value; }
		}
		/// <summary> Is Load Page </summary>
		private bool _isLoadPage = false;
		public bool IsLoadPage
		{
			get { return _isLoadPage; }
			set { _isLoadPage = value; }
		}
		/// <summary>Update panel control id</summary>
		public string UpdatePanelControlId { get; set; }
		/// <summary>Start time string has format「HH:mm:ss.fff」</summary>
		public string StartTimeString
		{
			get
			{
				var result = string.Format("{0}.000", this.HfStartTime.Value);
				return result.Trim();
			}
		}
		/// <summary>End time string has format「HH:mm:ss.fff」</summary>
		public string EndTimeString
		{
			get
			{
				var result = string.Format("{0}.998", this.HfEndTime.Value);
				return result.Trim();
			}
		}
		/// <summary> 時間表示を隠す </summary>
		private bool _isHideTime = false;
		public bool IsHideTime
		{
			get { return _isHideTime; }
			set { _isHideTime = value; }
		}
		/// <summary>デフォルト日付を設定するか（今日日付）</summary>
		public bool IsSetDefaultToday { get; set; }
		/// <summary>開始日（初期設定用）</summary>
		public DateTime? StartDate
		{
			get { return _startDate; }
			set { _startDate = value; }
		}
		private DateTime? _startDate;
		/// <summary>終了日（初期設定用）</summary>
		public DateTime? EndDate
		{
			get { return _endDate; }
			set { _endDate = value; }
		}
		private DateTime? _endDate;

		#region ラップコントロール定義
		/// <summary>Start date (wrapped control)</summary>
		private WrappedHtmlInputHidden WhfStartDate
		{
			get { return GetWrappedControl<WrappedHtmlInputHidden>("hfStartDate", string.Empty); }
		}
		/// <summary>Start time (wrapped control)</summary>
		private WrappedHtmlInputHidden WhfStartTime
		{
			get { return GetWrappedControl<WrappedHtmlInputHidden>("hfStartTime", string.Empty); }
		}
		/// <summary>End date (wrapped control)</summary>
		private WrappedHtmlInputHidden WhfEndDate
		{
			get { return GetWrappedControl<WrappedHtmlInputHidden>("hfEndDate", string.Empty); }
		}
		/// <summary>End time (wrapped control)</summary>
		private WrappedHtmlInputHidden WhfEndTime
		{
			get { return GetWrappedControl<WrappedHtmlInputHidden>("hfEndTime", string.Empty); }
		}
		#endregion
		#endregion
	}
}