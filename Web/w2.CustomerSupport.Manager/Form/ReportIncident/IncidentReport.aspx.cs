/*
=========================================================================================================
  Module      : インシデント集計ページ処理(IncidentReport.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.Cs.Reports;
using w2.App.Common.Cs.SummarySetting;

public partial class Form_ReportIncident_IncidentReport : ReportPageCs
{
	#region #Page_Load ページロード
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			Initialize();

			DispTopReports();
		}
	}
	#endregion

	#region -Initialize 初期化
	/// <summary>
	/// 初期化
	/// </summary>
	private void Initialize()
	{
		// 集計区分ドロップダウン
		var summaryService = new CsSummarySettingService(new CsSummarySettingRepository());
		var summarySettings = summaryService.GetAll(this.LoginOperatorDeptId);
		ddlReportSummaryKbn.Items.AddRange(
			(from setting in summarySettings
			 select new ListItem(setting.SummarySettingTitle
				 + ((setting.ValidFlg == Constants.FLG_CSSUMMARYSETTING_VALID_FLG_VALID)
					 ? string.Empty
					 : GetDispTextInvalid(true)),
				 setting.SummarySettingNo.ToString()))
			.ToArray());

		// デフォルトセット
		rbReportTypeGroup.Checked = true;
		rblDateType.Items[0].Selected = true;
	}
	#endregion

	#region -DispTopReports トップレポート表示
	/// <summary>
	/// トップレポート表示
	/// </summary>
	private void DispTopReports()
	{
		DispTopReportStatusCount();
		DispTopReportActionCount();
	}
	#endregion

	#region -DispTopReportStatusCount トップレポート（ステータス毎の現在の件数）表示
	/// <summary>
	/// トップレポート（ステータス毎の現在の件数）表示
	/// </summary>
	private void DispTopReportStatusCount()
	{
		var service = new IncidentReportService(new IncidentReportRepository());
		var model = service.GetStatusCount(this.LoginOperatorDeptId);

		lIncidentNoneCount.Text = WebSanitizer.UrlAttrHtmlEncode(StringUtility.ToNumeric(model.None));
		lIncidentActiveCount.Text = WebSanitizer.UrlAttrHtmlEncode(StringUtility.ToNumeric(model.Active));
		lIncidentUrgentCount.Text = WebSanitizer.UrlAttrHtmlEncode(StringUtility.ToNumeric(model.Urgent));
		lIncidentSuspendCount.Text = WebSanitizer.UrlAttrHtmlEncode(StringUtility.ToNumeric(model.Suspend));
		lIncidentTotalCount.Text = WebSanitizer.UrlAttrHtmlEncode(StringUtility.ToNumeric(model.UncompleteTotal));

		this.IncidentNowCountByStatus = model;
	}
	#endregion

	#region -DispTopReportActionCount トップレポート（期間内のアクション件数）表示
	/// <summary>
	/// トップレポート（期間内のアクション件数）表示
	/// </summary>
	private void DispTopReportActionCount()
	{
		rIncidentCompleteRepot.DataSource = GetIncidentActionCounts();
		rIncidentCompleteRepot.DataBind();
	}
	#endregion

	#region -GetIncidentActionCounts 期間内のアクション件数リスト取得（必要があれば更新）
	/// <summary>
	/// 期間内のアクション件数オブジェクトリスト取得（必要があれば更新）
	/// </summary>
	private IncidentActionCountByTermModel[] GetIncidentActionCounts()
	{
		var targetDate = DateTime.Today;	// 日付（00:00:00)で指定

		var service = new IncidentReportService(new IncidentReportRepository());
		if (DateTime.Now.Date != this.IncidentActionCountModelsUpdateDate.Date)
		{
			var models = new List<IncidentActionCountByTermModel>();
			foreach (int span in new int[] { 1, 7, 30 })
			{
				var beginDate =  targetDate.AddDays(-1 * span);	// X日前
				var endDate = targetDate.AddDays(-1);			// 昨日
				var model = service.GetActionCountByTerm(this.LoginOperatorDeptId, beginDate, endDate);
				model.DaySpan = span;
				models.Add(model);
			}
			this.IncidentActionCountModels = models.ToArray();
			this.IncidentActionCountModelsUpdateDate = DateTime.Now;
		}
		return this.IncidentActionCountModels;
	}
	#endregion

	#region #btnSearch_Click 検索ボタンクリック
	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, EventArgs e)
	{
		DispReportList();
	}
	#endregion

	#region -DispReportList レポート一覧表示
	/// <summary>
	/// レポート一覧表示
	/// </summary>
	private void DispReportList()
	{
		// 日付が空ならデフォルト値セット
		ucCsWorkflowDateTimePickerPeriod.SetDefaultDateIfEmpty();

		// 日付取得
		DateTime begin;
		DateTime end;
		ucCsWorkflowDateTimePickerPeriod.GetDateInput(out begin, out end);

		ReportRowModel[] list = GetReportList(begin, end);
		rList.DataSource = list;
		rList.DataBind();

		tblResult.Visible = true;
		dispReportListNoInfo.Visible = (list.Length == 0);
	}
	#endregion

	#region -GetReportList レポートリスト取得
	/// <summary>
	/// レポートリスト取得
	/// </summary>
	/// <param name="begin">開始日</param>
	/// <param name="end">終了日</param>
	/// <returns>レポートリスト</returns>
	private ReportRowModel[] GetReportList(DateTime begin, DateTime end)
	{
		var service = new IncidentReportService(new IncidentReportRepository());

		Hashtable param = GetDateSqlParamsForReport(begin, end);

		ReportRowModel[] list = null;
		if (rbReportTypeGroup.Checked)
		{
			list = service.GetGroupReport(param);
		}
		else if (rbReportTypeOperator.Checked)
		{
			list = service.GetOperatorReport(param);
		}
		else if (rbReportTypeGroupOperator.Checked)
		{
			list = service.GetGroupOperatorReport(this.LoginOperatorDeptId, param);
		}
		else if (rbReportTypeCategory.Checked)
		{
			list = service.GetCategoryReport(param);
		}
		else if (rbReportTypeVoc.Checked)
		{
			list = service.GetVocReport(param);
		}
		else if (rbReportTypeSummary.Checked)
		{
			param.Add(Constants.FIELD_CSINCIDENTSUMMARYVALUE_SUMMARY_NO, ddlReportSummaryKbn.SelectedValue);
			list = service.GetSummaryKbnReport(param);
		}
		else if (rbReportTypeMonth.Checked)
		{
			var monthList = GetMonthList(begin, end);
			list = service.GetMonthReport(monthList, param);
			list.ToList().ForEach(
				month => month.Name = month.Month + (string.IsNullOrEmpty(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE)
					// 「月」
					? ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_INCIDENTREPORT,
						Constants.VALUETEXT_PARAM_AGGREGATE_CATEGORY,
						Constants.VALUETEXT_PARAM_MONTH)
					: string.Empty));
		}
		else if (rbReportTypeMonthDay.Checked)
		{
			var monthDayList = GetMonthDayList(begin, end);
			list = service.GetMonthDayReport(monthDayList, param);
			list.ToList().ForEach(
				m => m.Name = DateTimeUtility.ToStringForManager(
					m.MonthDay,
					DateTimeUtility.FormatType.LongDate1Letter));
		}
		else if (rbReportTypeWeekday.Checked)
		{
			var weekdayList = GetWeekdayList(begin, end);
			list = service.GetWeekdayReport(weekdayList, param);
			list.ToList().ForEach(
				m => m.Name = ValueText.GetValueText(CS_REPORT, WEEK_DAY_LIST, m.Weekday.Value));
		}
		else if (rbReportTypeTime.Checked)
		{
			var startHour = DateTime.Parse(ucCsWorkflowDateTimePickerPeriod.HfStartTime.Value).Hour;
			var endHour = DateTime.Parse(ucCsWorkflowDateTimePickerPeriod.HfEndTime.Value).Hour;
			var hourList = Enumerable.Range(startHour, endHour).ToArray();
			list = service.GetTimeReport(hourList, param);
			list.ToList().ForEach(m => m.Name = m.Hour.Value + ":00 ～ " + (m.Hour.Value + 1) + ":00");
		}
		else if (rbReportTypeWeekdayTime.Checked)
		{
			var weekdayList = GetWeekdayList(begin, end);
			var startHour = DateTime.Parse(ucCsWorkflowDateTimePickerPeriod.HfStartTime.Value).Hour;
			var endHour = DateTime.Parse(ucCsWorkflowDateTimePickerPeriod.HfEndTime.Value).Hour;
			var hourList = Enumerable.Range(startHour, endHour).ToArray();
			list = service.GetWeekdayTimeReport(weekdayList, hourList, param);
			list.ToList().ForEach(m => {
				m.IsIndent = (m.Count.HasValue);
				m.Name = (m.Count.HasValue)
					? m.Hour.Value + ":00 ～ " + (m.Hour.Value + 1) + ":00"
					: ValueText.GetValueText(CS_REPORT, WEEK_DAY_LIST, m.Weekday.Value);
			});
		}

		return list;
	}
	#endregion

	#region -GetDateSqlParamsForReport レポート用日付SQLパラメタ取得
	/// <summary>
	/// レポート用日付SQLパラメタ取得
	/// </summary>
	/// <param name="begin">開始日</param>
	/// <param name="end">終了日</param>
	/// <returns>レポート用日付SQLパラメタ</returns>
	private Hashtable GetDateSqlParamsForReport(DateTime begin, DateTime end)
	{
		Hashtable result = new Hashtable();
		result.Add(Constants.FIELD_CSINCIDENT_DEPT_ID, this.LoginOperatorDeptId);
		var dateTimeEnd = DateTimeUtility.ToStringForManager(
			end.AddMilliseconds(998),
			DateTimeUtility.FormatType.LongFullDateTimeNoneServerTime);

		result.Add("create_date_bgn", (rblDateType.SelectedValue == "CREATE") ? (DateTime?)begin : null);
		result.Add("create_date_end", (rblDateType.SelectedValue == "CREATE") ? dateTimeEnd : null);
		result.Add("comeplete_date_bgn", (rblDateType.SelectedValue == "COMPLETE") ? (DateTime?)begin : null);
		result.Add("comeplete_date_end", (rblDateType.SelectedValue == "COMPLETE") ? dateTimeEnd : null);

		return result;
	}
	#endregion

	#region #rblReportType_SelectedIndexChanged レポートタイプラジオボタンリスト変更
	/// <summary>
	/// レポートタイプラジオボタンリスト変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rReportType_CheckedChanged(object sender, EventArgs e)
	{
		ddlReportSummaryKbn.Enabled = rbReportTypeSummary.Checked;
	}
	#endregion

	#region #GetReportTypeString レポートタイプ文字列取得
	/// <summary>
	/// レポートタイプ文字列取得
	/// </summary>
	/// <returns>レポートタイプ文字列</returns>
	protected string GetReportTypeString()
	{
		if (rbReportTypeGroup.Checked) return rbReportTypeGroup.Text;
		if (rbReportTypeOperator.Checked) return rbReportTypeOperator.Text;
		if (rbReportTypeGroupOperator.Checked) return rbReportTypeGroupOperator.Text;
		if (rbReportTypeCategory.Checked) return rbReportTypeCategory.Text;
		if (rbReportTypeVoc.Checked) return rbReportTypeVoc.Text;
		if (rbReportTypeSummary.Checked) return rbReportTypeSummary.Text;
		if (rbReportTypeMonth.Checked) return rbReportTypeMonth.Text;
		if (rbReportTypeMonthDay.Checked) return rbReportTypeMonthDay.Text;
		if (rbReportTypeWeekday.Checked) return rbReportTypeWeekday.Text;
		if (rbReportTypeTime.Checked) return rbReportTypeTime.Text;
		if (rbReportTypeWeekdayTime.Checked) return rbReportTypeWeekdayTime.Text;
		return "";
	}
	#endregion

	#region #CheckValid 有効状態取得
	/// <summary>
	/// 有効状態取得
	/// </summary>
	/// <param name="row">レポート行モデル</param>
	/// <returns>有効</returns>
	protected bool CheckValid(ReportRowModel row)
	{
		//未設定の時にtrueを返すため
		if (string.IsNullOrEmpty(row.Name) && string.IsNullOrEmpty(row.Id)) return true;
		if (rbReportTypeCategory.Checked) return row.RankedValid;
		return row.Valid;
	}
	#endregion

	#region プロパティ
	/// <summary>ステータス毎の現在の件数</summary>
	protected IncidentCountByStatusModel IncidentNowCountByStatus
	{
		get { return (IncidentCountByStatusModel)ViewState["IncidentNowCountByStatus"]; }
		set { ViewState["IncidentNowCountByStatus"] = value; }
	}
	/// <summary>期間内のアクション件数オブジェクトリスト</summary>
	private IncidentActionCountByTermModel[] IncidentActionCountModels
	{
		get { return (IncidentActionCountByTermModel[])ViewState["IncidentActionCountModels"]; }
		set { ViewState["IncidentActionCountModels"] = value; }
	}
	/// <summary>期間内のアクション件数オブジェクトリスト更新日</summary>
	private DateTime IncidentActionCountModelsUpdateDate
	{
		get { return (DateTime)(ViewState["IncidentActionCountModelsUpdateDate"] ?? DateTime.MinValue); }
		set { ViewState["IncidentActionCountModelsUpdateDate"] = value; }
	}

	#endregion
}