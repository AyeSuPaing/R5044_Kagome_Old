/*
=========================================================================================================
  Module      : 集計ページ向け日付選択ユーザーコントロール処理(ReportDateTimeSelector.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI.WebControls;
using w2.App.Common.Global.Config;

public partial class Form_ReportIncident_ReportDateTimeSelector : System.Web.UI.UserControl
{
	#region #Page_Init ページ初期化
	/// <summary>
	/// ページ初期化
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Init(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			Initialize();
		}
	}
	#endregion

	#region -Initialize 初期化
	/// <summary>
	/// 初期化
	/// </summary>
	private void Initialize()
	{
		ddlBeginDatePart1.Items.AddRange(this.DataSourceDatePart1);
		ddlBeginDatePart2.Items.AddRange(this.DataSourceDatePart2);
		ddlBeginDatePart3.Items.AddRange(this.DataSourceDatePart3);
		ddlEndDatePart1.Items.AddRange(this.DataSourceDatePart1);
		ddlEndDatePart2.Items.AddRange(this.DataSourceDatePart2);
		ddlEndDatePart3.Items.AddRange(this.DataSourceDatePart3);
	}
	#endregion

	#region +GetDateInput 入力日付取得（エラーであればエラーページ遷移）
	/// <summary>
	/// 入力日付取得（エラーであればエラーページ遷移）
	/// </summary>
	/// <param name="begin">開始日</param>
	/// <param name="end">終了日</param>
	public void GetDateInput(out DateTime begin, out DateTime end)
	{
		var errorMessages = new List<string>();
		if (DateTime.TryParse(
			(ddlBeginDatePart1.SelectedValue + "/" + ddlBeginDatePart2.SelectedValue + "/" + ddlBeginDatePart3.SelectedValue + " " + "00:00:00"),
			string.IsNullOrEmpty(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE) 
				? CultureInfo.CurrentCulture
				: new CultureInfo(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE),
			DateTimeStyles.None,
			out begin) == false)
		{
			errorMessages.Add(WebMessages.GetMessages(WebMessages.ERROR_MANAGER_START_DATE_CORRECT_DATE));
		}
		if (DateTime.TryParse(
			(ddlEndDatePart1.SelectedValue + "/" + ddlEndDatePart2.SelectedValue + "/" + ddlEndDatePart3.SelectedValue + " " + "23:59:59.997"),
			string.IsNullOrEmpty(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE)
				? CultureInfo.CurrentCulture
				: new CultureInfo(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE),
			DateTimeStyles.None,
			out end) == false)
		{
			errorMessages.Add(WebMessages.GetMessages(WebMessages.ERROR_MANAGER_END_DATE_CORRECT_DATE));
		}
		if ((errorMessages.Count == 0) && (begin > end))
		{
			errorMessages.Add(WebMessages.GetMessages(WebMessages.ERROR_MANAGER_START_DATE_BEFORE_END_DATE));
		}
		if ((errorMessages.Count == 0) && (end- begin).Days > 100)
		{
			errorMessages.Add(WebMessages.GetMessages(WebMessages.ERROR_MANAGER_PERIOD_WITHIN_100_DAYS));
		}
		if (errorMessages.Count != 0)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = string.Join("<br />", errorMessages.ToArray());
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
	}
	#endregion

	#region +SetDefaultDateIfEmpty 日付指定が空ならデフォルト値をセットする
	/// <summary>
	/// 日付指定が空ならデフォルト値をセットする
	/// </summary>
	public void SetDefaultDateIfEmpty()
	{
		if ((ddlBeginDatePart1.SelectedValue == "") && (ddlBeginDatePart2.SelectedValue == "") && (ddlBeginDatePart3.SelectedValue == "")
			&& (ddlEndDatePart1.SelectedValue == "") && (ddlEndDatePart2.SelectedValue == "") && (ddlEndDatePart3.SelectedValue == ""))
		{
			var yearBegin = DateTime.Now.AddDays(-100).Year.ToString();
			var monthBegin = DateTime.Now.AddDays(-100).Month.ToString("00");
			var dayBegin = DateTime.Now.AddDays(-100).Day.ToString("00");
			var yearEnd = DateTime.Now.Year.ToString();
			var monthEnd = DateTime.Now.Month.ToString("00");
			var dayEnd = DateTime.Now.Day.ToString("00");
			ddlBeginDatePart1.SelectedValue = this.IsMdyFormat ? monthBegin : (this.IsDmyFormat ? dayBegin : yearBegin);
			ddlBeginDatePart2.SelectedValue = this.IsMdyFormat ? dayBegin : monthBegin;
			ddlBeginDatePart3.SelectedValue = (this.IsMdyFormat || this.IsDmyFormat) ? yearBegin : dayBegin;
			ddlEndDatePart1.SelectedValue = this.IsMdyFormat ? monthEnd : (this.IsDmyFormat ? dayEnd : yearEnd);
			ddlEndDatePart2.SelectedValue = this.IsMdyFormat ? dayEnd : monthEnd;
			ddlEndDatePart3.SelectedValue = (this.IsMdyFormat || this.IsDmyFormat) ? yearEnd : dayEnd;
		}
	}
	#endregion

	/// <summary>
	/// 日付の第一部分取得
	/// </summary>
	/// <param name="date">日付</param>
	/// <returns>日付の第一部分</returns>
	protected string GetDatePart1(DateTime date)
	{
		return this.IsMdyFormat
			? date.Month.ToString("00")
			: (this.IsDmyFormat ? date.Day.ToString("00") : date.Year.ToString());
	}

	/// <summary>
	/// 日付の第二部分取得
	/// </summary>
	/// <param name="date">日付</param>
	/// <returns>日付の第二部分</returns>
	protected string GetDatePart2(DateTime date)
	{
		return this.IsMdyFormat ? date.Day.ToString("00") : date.Month.ToString("00");
	}

	/// <summary>
	/// 日付の第三部分取得
	/// </summary>
	/// <param name="date">日付</param>
	/// <returns>日付の第三部分</returns>
	protected string GetDatePart3(DateTime date)
	{
		return (this.IsMdyFormat || this.IsDmyFormat) ? date.Year.ToString() : date.Day.ToString("00");
	}

	/// <summary> 日付形式タイプ </summary>
	private string DateFormatType
	{
		get
		{
			if (Constants.GLOBAL_OPTION_ENABLE == false) return string.Empty;
			return GlobalConfigUtil.GetDateTimeFormatText(
					Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE,
					DateTimeUtility.FormatType.General)
				.ToLower();
		}
	}
	/// <summary> 「MDY」日付形式タイプか </summary>
	private bool IsMdyFormat { get { return (this.DateFormatType == "mdy"); } }
	/// <summary> 「DMY」日付形式タイプか </summary>
	private bool IsDmyFormat { get { return (this.DateFormatType == "dmy"); } }
	/// <summary> 年のリスト </summary>
	private ListItem[] YearList
	{
		get
		{
			var list = new List<ListItem>();
			list.Add(new ListItem("",""));
			list.AddRange(DateTimeUtility.GetYearListItem((DateTime.Now.Year - 10), DateTime.Now.Year));
			return list.ToArray();
		}
	}	
	/// <summary> 月のリスト </summary>
	private ListItem[] MonthList
	{
		get
		{
			var list = new List<ListItem>();
			list.Add(new ListItem("", ""));
			list.AddRange(DateTimeUtility.GetMonthListItem("00"));
			return list.ToArray();
		}
	}
	/// <summary> 日のリスト </summary>
	private ListItem[] DayList
	{
		get
		{
			var list = new List<ListItem>();
			list.Add(new ListItem("", ""));
			list.AddRange(DateTimeUtility.GetDayListItem("00"));
			return list.ToArray();
		}
	}
	/// <summary> 日付第一部分のデータソース </summary>
	protected ListItem[] DataSourceDatePart1
	{
		get
		{
			return (this.IsMdyFormat ? this.MonthList : (this.IsDmyFormat ? this.DayList : this.YearList));
		}
	}
	/// <summary> 日付第二部分のデータソース </summary>
	protected ListItem[] DataSourceDatePart2 { get { return (this.IsMdyFormat ? this.DayList : this.MonthList); } }
	/// <summary> 日付第三部分のデータソース </summary>
	protected ListItem[] DataSourceDatePart3 { get { return ((this.IsMdyFormat || this.IsDmyFormat) ? this.YearList : this.DayList); } }
}