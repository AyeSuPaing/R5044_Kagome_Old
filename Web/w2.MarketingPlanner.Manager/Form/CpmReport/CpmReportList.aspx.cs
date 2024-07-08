/*
=========================================================================================================
  Module	  : CPMレポート一覧ページ処理(CpmReportList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using w2.Domain.DispSummaryAnalysis;
using w2.Domain.User.Helper;

public partial class Form_CpmReport_CpmReportList : BasePage
{
	/// <summary>リクエストキー：ターゲット種別</summary>
	protected const string REQUEST_KEY_TARGET_TYPE = "ttype";

	/// <summary>ターゲット</summary>
	protected enum TargetType
	{
		/// <summary>日別</summary>
		Daily,
		/// <summary>月別</summary>
		Monthly
	}

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

			Diplay();
		}
	}

	/// <summary>
	/// 初期化
	/// </summary>
	private void Initialize()
	{
		TargetType targetType;
		if (Enum.TryParse(Request[REQUEST_KEY_TARGET_TYPE], out targetType) == false) targetType = TargetType.Monthly;	// 月別がデフォルト

		rblTargetType.SelectedValue = targetType.ToString();

		// 日付取得
		this.CurrentYear = DateTime.Now.Year;
		int year;
		if (int.TryParse(Request[Constants.REQUEST_KEY_CURRENT_YEAR], out year)) this.CurrentYear = year;
		this.CurrentMonth = DateTime.Now.Month;
		int month;
		if (int.TryParse(Request[Constants.REQUEST_KEY_CURRENT_MONTH], out month)) this.CurrentMonth = month;
	}

	/// <summary>
	/// ターゲットタイプラジオボタンリストクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblTargetType_SelectedIndexChanged(object sender, EventArgs e)
	{
		Diplay();
	}

	/// <summary>
	/// 表示
	/// </summary>
	private void Diplay()
	{
		// カレンダ作成
		lblCurrentCalendar.Text = CalendarUtility.CreateHtmlYMCalendar(
			this.CurrentYear,
			this.CurrentMonth,
			Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_CPM_REPORT_LIST,
			REQUEST_KEY_TARGET_TYPE + "=" + HttpUtility.UrlEncode(rblTargetType.SelectedValue),
			Constants.REQUEST_KEY_CURRENT_YEAR,
			Constants.REQUEST_KEY_CURRENT_MONTH,
			this.IsDailyReport);

		// レポート取得・セット
		var reports = GetReport();
		rReport.DataSource = reports;
		rReport.DataBind();
	}

	/// <summary>
	/// CSVダウンロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbReportExport_Click(object sender, EventArgs e)
	{
		var fileName = "CpmReport" + DateTime.Now.ToString("yyyyMMdd") + rblTargetType.SelectedValue;
		Response.ContentEncoding = Encoding.GetEncoding("Shift_JIS");
		Response.ContentType = "application/csv";
		Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName + ".csv");

		var head = new List<string>();
		head.Add(
			// 「日付」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_CPM_REPORT_LIST,
				Constants.VALUETEXT_PARAM_CPM_REPORT_LIST_TITLE,
				Constants.VALUETEXT_PARAM_CPM_REPORT_LIST_DATE));
		head.AddRange(Constants.CPM_CLUSTER_SETTINGS.ClusterNames.SelectMany(kvp =>
			new[] {
				string.Format(
					"{0}{1}",
					kvp.Value,
					// 「(人)」
					ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_CPM_REPORT_LIST,
						Constants.VALUETEXT_PARAM_CPM_REPORT_LIST_TITLE,
						Constants.VALUETEXT_PARAM_CPM_REPORT_LIST_MAN)),
				kvp.Value + "(%)"
			}));
		WriteCsvLine(head);

		// データ作成
		var reports = GetReport();
		foreach (var report in reports)
		{
			var line = new List<string>();
			var datetimeString = this.IsDailyReport
				? DateTimeUtility.ToStringForManager(
					new DateTime(this.CurrentYear, this.CurrentMonth, report.No),
					DateTimeUtility.FormatType.LongDate2Letter)
				: DateTimeUtility.ToStringForManager(
					new DateTime(this.CurrentYear, report.No, 1),
					DateTimeUtility.FormatType.LongYearMonth);
			line.Add(datetimeString);
			line.AddRange(report.Items.SelectMany(item => new[]
			{
				(item.Count.HasValue ? item.Count.Value.ToString() : ""),
				(item.Percentage.HasValue ? item.Percentage.Value.ToString() : "")
			}).ToArray());
			WriteCsvLine(line);

			line = new List<string>();
			line.Add(datetimeString
				+ (this.IsDailyReport
					// 「前日比」
					? ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_CPM_REPORT_LIST,
						Constants.VALUETEXT_PARAM_CPM_REPORT_LIST_TITLE,
						Constants.VALUETEXT_PARAM_CPM_REPORT_LIST_DAY_BEFORE)
					// 「前月比」
					: ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_CPM_REPORT_LIST,
						Constants.VALUETEXT_PARAM_CPM_REPORT_LIST_TITLE,
						Constants.VALUETEXT_PARAM_CPM_REPORT_LIST_MONHT_TO_MONTH_BASIS)));
			line.AddRange(report.Items.SelectMany(item => new[]
			{
				(item.GrowthCount.HasValue ? item.GrowthCount.Value.ToString() : ""),
				(item.GrowthRate.HasValue ? item.GrowthRate.Value.ToString() : ""),
			}).ToArray());

			WriteCsvLine(line);
		}
		Response.End();
	}

	/// <summary>
	///レポート取得
	/// </summary>
	/// <returns></returns>
	private CpmClusterReport[] GetReport()
	{
		var reports = (this.IsDailyReport)
			? new DispSummaryAnalysisService().GetDailyReportForCpmReport(this.LoginOperatorDeptId, this.CurrentYear,
				this.CurrentMonth)
			: new DispSummaryAnalysisService().GetMonthlyReportForCpmReport(this.LoginOperatorDeptId, this.CurrentYear);

		foreach (var report in reports)
		{
			// レポートデータ並び替え・無いものは削除
			report.Items = Constants.CPM_CLUSTER_SETTINGS.ClusterIdList.Select(clusterId =>
			report.Items.FirstOrDefault(i => i.CpmClusterId == clusterId)).Select(item => item ?? new CpmClusterReportItem()).ToList();

			// パーセンテージ計算
			report.CalculateItemPercentage();
		}

		// 計算されていない日の空データ作成
		foreach (var reportData in reports.Where(d => d.Items.Count == 0))
		{
			for (var i = 0;
				i < Constants.CPM_CLUSTER_SETTINGS.Settings1.Length * Constants.CPM_CLUSTER_SETTINGS.Settings2.Length;
				i++)
			{
				reportData.Items.Add(new CpmClusterReportItem());
			}
		}

		return reports;
	}

	/// <summary>
	/// CSV行出力
	/// </summary>
	/// <param name="columns">カラム配列</param>
	private void WriteCsvLine(IEnumerable<string> columns)
	{
		Response.Write(StringUtility.CreateEscapedCsvString(columns) + "\r\n");
	}

	/// <summary>対象年</summary>
	public int CurrentYear
	{
		get { return (int)ViewState["CurrentYear"]; }
		set { ViewState["CurrentYear"] = value; }
	}
	/// <summary>対象月</summary>
	public int CurrentMonth
	{
		get { return (int)ViewState["CurrentMonth"]; }
		set { ViewState["CurrentMonth"] = value; }
	}
	/// <summary>日次レポートか</summary>
	protected bool IsDailyReport
	{
		get { return rblTargetType.SelectedValue == TargetType.Daily.ToString(); }
	}
	/// <summary>月次レポートか</summary>
	protected bool IsMonthlyReport
	{
		get { return rblTargetType.SelectedValue == TargetType.Monthly.ToString(); }
	}
}