/*
=========================================================================================================
  Module      : サマリー情報ページ処理(SummaryInformation.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.UI.WebControls;
using w2.App.Common.Order.Workflow;
using w2.App.Common.SummaryInformation;
using w2.Domain.DispSummaryAnalysis;
using w2.Domain.SaleGoal;
using w2.Domain.ShopOperator;
using w2.Domain.SummaryReport;

/// <summary>
/// Summary information
/// </summary>
public partial class Form_Summary_SummaryInformation : SummaryInformationPage
{
	#region ~Constants
	/// <summary>Format date: long date</summary>
	protected const string CONST_FORMATDATE_LONGDATE = "yyyy/MM/dd HH:mm";

	/// <summary>Button has current class</summary>
	private const string CONST_BUTTON_HAS_CURRENT_CLASS = "btn-group-btn btn-smartphone is-current";
	/// <summary>Button not has current class</summary>
	private const string CONST_BUTTON_NOT_HAS_CURRENT_CLASS = "btn-group-btn btn-smartphone";
	/// <summary>Highlight class</summary>
	private const string CONST_HIGHLIGHT_CLASS = "highlight";

	/// <summary>Ranking title type</summary>
	private const string VALUETEXT_PARAM_SUMMARYREPORT_RANKING_TITLE_KBN = "ranking_title_kbn";
	/// <summary>Ranking title type: Count (adv code)</summary>
	private const string VALUETEXT_PARAM_SUMMARYREPORT_RANKING_TITLE_KBN_COUNT_ADVCODE = "COUNT_ADVCODE";
	/// <summary>Ranking title type: Price (adv code)</summary>
	private const string VALUETEXT_PARAM_SUMMARYREPORT_RANKING_TITLE_KBN_PRICE_ADVCODE = "PRICE_ADVCODE";
	/// <summary>Ranking title type: Count (product sales)</summary>
	private const string VALUETEXT_PARAM_SUMMARYREPORT_RANKING_TITLE_KBN_COUNT_PRODUCTSALES = "COUNT_PRODUCTSALES";
	/// <summary>Ranking title type: Price (product sales)</summary>
	private const string VALUETEXT_PARAM_SUMMARYREPORT_RANKING_TITLE_KBN_PRICE_PRODUCTSALES = "PRICE_PRODUCTSALES";
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			InitializeComponent();
			Display();

		}
	}

	/// <summary>
	/// MPからのメール配信状況
	/// </summary>
	private void ShowTaskScheduleHistory()
	{
		this.TaskScheduleHistory = new TaskScheduleHistory().CreateReport();
	}

	/// <summary>
	/// Initialize component
	/// </summary>
	protected void InitializeComponent()
	{
		ddlApplicableMonth.Items.AddRange(DateTimeUtility.GetMonthListItem());
	}

	/// <summary>
	/// Display
	/// </summary>
	private void Display()
	{
		DisplayCurrentSaleGoal();
		DisplayLatestReport();
		DisplayRanking();
		DisplayOrderStatusReport();
		DisplayIncidentReport();
		DisplayProductStockReport();
		ShowTaskScheduleHistory();
	}

	/// <summary>
	/// Display current sale goal
	/// </summary>
	private void DisplayCurrentSaleGoal()
	{
		var saleGoalReportHelper = new SaleGoalReport(DateTime.Today);
		var saleGoalList = saleGoalReportHelper.CreateReports();
		if (saleGoalList.Any() == false)
		{
			// Set default data
			saleGoalList = saleGoalReportHelper.CreateDefaultReports();

			// Display warning when sale goal has not been set
			dvGuide.Attributes.Add("class", "dashboard-goal-progress-guide");
			dvGuideBackground.Attributes.Add("class", "dashboard-goal-progress-guide-bg");
			dvGuideLink.Visible = true;
		}

		// Convert current sale goal list to Json result
		this.CurrentSalesRevenue = CreateReportJsonString(saleGoalList);
	}

	/// <summary>
	/// Ranking display
	/// </summary>
	private void DisplayRanking()
	{
		this.RankingPeriodKbn = Constants.FLG_SUMMARYREPORT_PERIOD_KBN_YESTERDAY;
		this.AdvertisingCodeOrderByKbn = SummaryReportModel.RANKING_REPORT_ORDER_BY_COUNT;
		this.ProductSalesOrderByKbn = SummaryReportModel.RANKING_REPORT_ORDER_BY_COUNT;

		SetCommandArgumentForRankingLinkButtonPeriodKbn();
		SetCommandArgumentForRankingLinkButtonOrderByKbn();
		SetRankingDisplay(this.RankingPeriodKbn);
	}

	/// <summary>
	/// Latest report display
	/// </summary>
	private void DisplayLatestReport()
	{
		this.LatestReport = GetLatestReport(Constants.FLG_SUMMARYREPORT_PERIOD_KBN_YESTERDAY);
	}

	/// <summary>
	/// Display order status report
	/// </summary>
	private void DisplayOrderStatusReport()
	{
		this.OrderStatusReport = new OrderStatusReport().CreateReport();
	}

	/// <summary>
	/// Display incident report
	/// </summary>
	private void DisplayIncidentReport()
	{
		if (Constants.CS_OPTION_ENABLED == false) return;

		this.IncidentReport = new IncidentReport(this.LoginOperatorDeptId, this.LoginOperatorId)
			.CreateReport();
	}

	/// <summary>
	/// Display product stock report
	/// </summary>
	private void DisplayProductStockReport()
	{
		if (Constants.PRODUCT_STOCK_OPTION_ENABLE == false) return;

		this.ProductStockReport = new ProductStockReport(this.LoginOperatorShopId)
			.CreateReport();
	}

	/// <summary>
	/// MPからのメール配信レポート
	/// </summary>
	private void MpMailCounts()
	{
		if (Constants.MP_OPTION_ENABLED == false) return;
	}

	/// <summary>
	/// Get shipped report
	/// </summary>
	/// <returns>Shipped report</returns>
	[WebMethod]
	public static string GetShippedReport()
	{
		var orderShippedStatus = new OrderShippedStatusReport().CreateReport();
		var createReport = CreateReportJsonString(orderShippedStatus);
		return createReport;
	}

	#region ~Workflow status
	/// <summary>
	/// Display workflow status report
	/// </summary>
	/// <returns>Order workflow report</returns>
	[WebMethod]
	public static string GetOrderWorkflowReport()
	{
		var orderWorkflowSettingList = new SummaryReportService().GetOrderWorkflowSettingAllListForReport();
		var orderWorkflows = new List<OrderWorkflowReport>();
		var summaryService = new DispSummaryAnalysisService();
		foreach (var setting in orderWorkflowSettingList)
		{
			var urlWorkflowList = CreateWorkflowListUrl(
				setting.WorkflowType,
				setting.WorkflowNo.ToString(),
				setting.WorkflowKbn);
			var orderWorkflow = new OrderWorkflowReport
			{
				UrlWorkflow = string.Format(
					"javascript:open_window('{0}','orderworkflowlist','width=1200,height=850,top=110,left=380,status=NO,scrollbars=yes,resizable=yes');",
					urlWorkflowList),
				WorkflowName = setting.WorkflowName,
				WorkflowKbn = setting.WorkflowKbn,
				WorkflowType = setting.WorkflowType,
				WorkflowNo = setting.WorkflowNo.ToString(),
			};

			orderWorkflows.Add(orderWorkflow);
		}

		var result = CreateReportJsonString(orderWorkflows);
		return result;
	}
	#endregion

	#region ~Sale goal
	/// <summary>
	/// Update sale goal
	/// </summary>
	/// <param name="annualGoal">The annual goal</param>
	/// <param name="applicableMonth">The applicable month</param>
	/// <param name="monthlyGoal1">The monthly goal 1</param>
	/// <param name="monthlyGoal2">The monthly goal 2</param>
	/// <param name="monthlyGoal3">The monthly goal 3</param>
	/// <param name="monthlyGoal4">The monthly goal 4</param>
	/// <param name="monthlyGoal5">The monthly goal 5</param>
	/// <param name="monthlyGoal6">The monthly goal 6</param>
	/// <param name="monthlyGoal7">The monthly goal 7</param>
	/// <param name="monthlyGoal8">The monthly goal 8</param>
	/// <param name="monthlyGoal9">The monthly goal 9</param>
	/// <param name="monthlyGoal10">The monthly goal 10</param>
	/// <param name="monthlyGoal11">The monthly goal 11</param>
	/// <param name="monthlyGoal12">The monthly goal 12</param>
	/// <returns>Json result</returns>
	[WebMethod]
	public static string UpdateSaleGoal(
		string annualGoal,
		string applicableMonth,
		string monthlyGoal1,
		string monthlyGoal2,
		string monthlyGoal3,
		string monthlyGoal4,
		string monthlyGoal5,
		string monthlyGoal6,
		string monthlyGoal7,
		string monthlyGoal8,
		string monthlyGoal9,
		string monthlyGoal10,
		string monthlyGoal11,
		string monthlyGoal12)
	{
		// Get sale goal input
		var loginShopOperator =
			(ShopOperatorModel)HttpContext.Current.Session[Constants.SESSION_KEY_LOGIN_SHOP_OPERTOR]
				?? new ShopOperatorModel();
		var input = new SaleGoalInput
		{
			Year = DateTime.Now.Year.ToString(),
			AnnualGoal = annualGoal,
			ApplicableMonth = applicableMonth,
			MonthlyGoal1 = monthlyGoal1,
			MonthlyGoal2 = monthlyGoal2,
			MonthlyGoal3 = monthlyGoal3,
			MonthlyGoal4 = monthlyGoal4,
			MonthlyGoal5 = monthlyGoal5,
			MonthlyGoal6 = monthlyGoal6,
			MonthlyGoal7 = monthlyGoal7,
			MonthlyGoal8 = monthlyGoal8,
			MonthlyGoal9 = monthlyGoal9,
			MonthlyGoal10 = monthlyGoal10,
			MonthlyGoal11 = monthlyGoal11,
			MonthlyGoal12 = monthlyGoal12,
			LastChanged = loginShopOperator.Name
		};

		// Validate input
		var errorMessages = input.Validate();
		if (string.IsNullOrEmpty(errorMessages) == false)
		{
			var createReport = CreateReportJsonString(
				new
				{
					success = false,
					error = errorMessages.Replace("<br />", "\r\n")
				});
			return createReport;
		}

		// Insert or update sale goal
		using (var accessor = new SqlAccessor())
		{
			accessor.OpenConnection();
			accessor.BeginTransaction();

			var model = input.CreateModel();
			var saleGoalService = new SaleGoalService();
			if (saleGoalService.Get(int.Parse(model.Year), accessor) == null)
			{
				saleGoalService.Insert(model, accessor);
			}
			else
			{
				saleGoalService.Update(model, accessor);
			}

			accessor.CommitTransaction();
		}

		var result = GetUpdatedSaleGoal();
		return result;
	}

	/// <summary>
	/// Open sale goal setting
	/// </summary>
	/// <returns>A JSON string that represent sale goal model</returns>
	[WebMethod]
	public static string OpenSaleGoalSetting()
	{
		var toDay = DateTime.Now;
		var currentSaleGoal = new SaleGoalService().Get(toDay.Year);
		if (currentSaleGoal == null)
		{
			currentSaleGoal = new SaleGoalModel
			{
				ApplicableMonth = toDay.Month,
			};
		}

		var result = CreateReportJsonString(
			new
			{
				model = currentSaleGoal,
			});
		return result;
	}

	/// <summary>
	/// Get updated sale goal
	/// </summary>
	/// <returns>Json string that represent updated sale goal</returns>
	private static string GetUpdatedSaleGoal()
	{
		var saleGoalList = new SaleGoalReport(DateTime.Today).CreateReports();
		if (saleGoalList.Any() == false)
		{
			var createReport = CreateReportJsonString(
				new
				{
					success = false,
					error = string.Empty,
					goal = string.Empty,
				});
			return createReport;
		}

		var result = CreateReportJsonString(
			new
			{
				success = true,
				error = string.Empty,
				updatedData = CreateReportJsonString(saleGoalList)
			});
		return result;
	}
	#endregion

	#region ~Latest report
	/// <summary>
	/// Get latest reports by period kbn
	/// </summary>
	/// <param name="periodKbn">Period kbn</param>
	/// <returns>Latest report information</returns>
	[WebMethod]
	public static string GetLatestReportsByPeriodKbn(string periodKbn)
	{
		var report = GetLatestReport(periodKbn);
		var result = CreateReportJsonString(report);
		return result;
	}

	/// <summary>
	/// Get latest report
	/// </summary>
	/// <param name="periodKbn">Period kbn</param>
	/// <returns>Latest report</returns>
	private static LatestReport GetLatestReport(string periodKbn)
	{
		var periodDate = GetLatestReportPeriod(periodKbn);
		var report = new LatestReport(
			periodKbn,
			periodDate.Item1,
			periodDate.Item2);
		return report;
	}

	/// <summary>
	/// Get latest report period
	/// </summary>
	/// <param name="periodKbn">Period kbn</param>
	/// <returns>Two result (begin date, end date)</returns>
	private static Tuple<DateTime, DateTime> GetLatestReportPeriod(string periodKbn)
	{
		var today = DateTime.Today;
		DateTime beginDate;
		DateTime endDate;
		if (periodKbn == Constants.FLG_SUMMARYREPORT_PERIOD_KBN_YESTERDAY)
		{
			beginDate = DateTimeUtility.GetBeginTimeOfDay(today.AddDays(-1));
			endDate = DateTimeUtility.GetEndTimeOfDay(today.AddDays(-1));
			return new Tuple<DateTime, DateTime>(beginDate, endDate);
		}

		beginDate = DateTimeUtility.GetBeginTimeOfDay(today);
		endDate = DateTimeUtility.GetEndTimeOfDay(today);
		return new Tuple<DateTime, DateTime>(beginDate, endDate);
	}
	#endregion

	#region ~Charts
	/// <summary>
	/// Get chart reports by period kbn
	/// </summary>
	/// <param name="periodKbn">Period Kbn</param>
	/// <returns>Chart reports</returns>
	[WebMethod]
	public static string GetChartReportsByPeriodKbn(string periodKbn)
	{
		var charts = new ChartReport(periodKbn).CreateReports();
		var result = CreateReportJsonString(charts);
		return result;
	}
	#endregion

	#region ~Ranking
	/// <summary>
	/// Link button ranking period click event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbRankingPeriod_Click(object sender, EventArgs e)
	{
		var lbRankingPeriod = ((LinkButton)sender);
		this.RankingPeriodKbn = lbRankingPeriod.CommandArgument;

		hAdvertisingCodeOrderRankingTitle.InnerText = GetAdvCodeTitleHtmlEncoded(
			this.RankingPeriodKbn,
			this.AdvertisingCodeOrderByKbn);

		hProductSalesRankingTitle.InnerText = GetProductSalesTitleHtmlEncoded(
			this.RankingPeriodKbn,
			this.ProductSalesOrderByKbn);

		SetRankingDisplay(this.RankingPeriodKbn);
		ChangeRankingPeriodLinkButtonCssClass(this.RankingPeriodKbn);
	}

	/// <summary>
	/// Link button adv code order ranking order by kbn click event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbAdvCodeOrderRankingOrderByKbn_Click(object sender, EventArgs e)
	{
		this.AdvertisingCodeOrderByKbn = ((LinkButton)sender).CommandArgument;

		switch (this.AdvertisingCodeOrderByKbn)
		{
			case SummaryReportModel.RANKING_REPORT_ORDER_BY_COUNT:
				lbAdvCodeOrderRankingOrderByNumber.CssClass = CONST_BUTTON_HAS_CURRENT_CLASS;
				lbAdvCodeOrderRankingOrderByAmount.CssClass = CONST_BUTTON_NOT_HAS_CURRENT_CLASS;
				break;

			case SummaryReportModel.RANKING_REPORT_ORDER_BY_AMOUNT:
				lbAdvCodeOrderRankingOrderByNumber.CssClass = CONST_BUTTON_NOT_HAS_CURRENT_CLASS;
				lbAdvCodeOrderRankingOrderByAmount.CssClass = CONST_BUTTON_HAS_CURRENT_CLASS;
				break;
		}

		hAdvertisingCodeOrderRankingTitle.InnerText =
			GetAdvCodeTitleHtmlEncoded(
				this.RankingPeriodKbn,
				this.AdvertisingCodeOrderByKbn);

		SetAdvCodeOrderRankingList(
			this.RankingPeriodKbn,
			this.AdvertisingCodeOrderByKbn);
	}

	/// <summary>
	/// Link button product sales ranking order by kbn click event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbProductSalesRankingOrderByKbn_Click(object sender, EventArgs e)
	{
		this.ProductSalesOrderByKbn = ((LinkButton)sender).CommandArgument;

		switch (this.ProductSalesOrderByKbn)
		{
			case SummaryReportModel.RANKING_REPORT_ORDER_BY_COUNT:
				lbProductSalesRankingOrderByNumber.CssClass = CONST_BUTTON_HAS_CURRENT_CLASS;
				lbProductSalesRankingOrderByAmount.CssClass = CONST_BUTTON_NOT_HAS_CURRENT_CLASS;
				break;

			case SummaryReportModel.RANKING_REPORT_ORDER_BY_AMOUNT:
				lbProductSalesRankingOrderByNumber.CssClass = CONST_BUTTON_NOT_HAS_CURRENT_CLASS;
				lbProductSalesRankingOrderByAmount.CssClass = CONST_BUTTON_HAS_CURRENT_CLASS;
				break;
		}

		hProductSalesRankingTitle.InnerText =
			GetProductSalesTitleHtmlEncoded(
				this.RankingPeriodKbn,
				this.ProductSalesOrderByKbn);

		cbByVariation_CheckedChanged(sender, e);
	}

	/// <summary>
	/// Check box by variation checked changed event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbByVariation_CheckedChanged(object sender, EventArgs e)
	{
		rProductVariationSalesRankingList.Visible = cbByVariation.Checked;
		rProductSalesRankingList.Visible = (cbByVariation.Checked == false);

		SetProductSalesRankingList(
			this.RankingPeriodKbn,
			this.ProductSalesOrderByKbn);
		SetProductVariationSalesRankingList(
			this.RankingPeriodKbn,
			this.ProductSalesOrderByKbn);
	}

	/// <summary>
	/// Set ranking display
	/// </summary>
	/// <param name="periodKbn">Period kbn</param>
	private void SetRankingDisplay(string periodKbn)
	{
		SetAdvCodeOrderRankingList(
			periodKbn,
			this.AdvertisingCodeOrderByKbn);
		SetProductVariationSalesRankingList(
			periodKbn,
			this.ProductSalesOrderByKbn);
		SetProductSalesRankingList(
			periodKbn,
			this.ProductSalesOrderByKbn);
	}

	/// <summary>
	/// Set command argument for ranking link button period kbn
	/// </summary>
	private void SetCommandArgumentForRankingLinkButtonPeriodKbn()
	{
		lbRankingPeriodYesterday.CommandArgument = Constants.FLG_SUMMARYREPORT_PERIOD_KBN_YESTERDAY;
		lbRankingPeriodLast7Days.CommandArgument = Constants.FLG_SUMMARYREPORT_PERIOD_KBN_LAST_SEVEN_DAYS;
		lbRankingPeriodLastMonth.CommandArgument = Constants.FLG_SUMMARYREPORT_PERIOD_KBN_LAST_MONTH;
		lbRankingPeriodThisMonth.CommandArgument = Constants.FLG_SUMMARYREPORT_PERIOD_KBN_THIS_MONTH;
		lbRankingPeriodThisYear.CommandArgument = Constants.FLG_SUMMARYREPORT_PERIOD_KBN_THIS_YEAR;
	}

	/// <summary>
	/// Set command argument for ranking link button order by kbn
	/// </summary>
	private void SetCommandArgumentForRankingLinkButtonOrderByKbn()
	{
		lbAdvCodeOrderRankingOrderByNumber.CommandArgument = SummaryReportModel.RANKING_REPORT_ORDER_BY_COUNT;
		lbAdvCodeOrderRankingOrderByAmount.CommandArgument = SummaryReportModel.RANKING_REPORT_ORDER_BY_AMOUNT;
		lbProductSalesRankingOrderByNumber.CommandArgument = SummaryReportModel.RANKING_REPORT_ORDER_BY_COUNT;
		lbProductSalesRankingOrderByAmount.CommandArgument = SummaryReportModel.RANKING_REPORT_ORDER_BY_AMOUNT;
	}

	/// <summary>
	/// Set adv code order ranking list
	/// </summary>
	/// <param name="periodKbn">Period kbn</param>
	/// <param name="orderByKbn">Order by kbn</param>
	private void SetAdvCodeOrderRankingList(string periodKbn, string orderByKbn)
	{
		var advCodeOrderRankingList = new SummaryReportService().GetTop10AdvCodeOrderRankingList(
			this.LoginOperatorShopId,
			orderByKbn,
			GetBeginDateTime(periodKbn),
			GetEndDateTime(periodKbn));

		rAdvCodeOrderRankingList.DataSource = advCodeOrderRankingList;
		rAdvCodeOrderRankingList.DataBind();
	}

	/// <summary>
	/// Set product sales ranking list
	/// </summary>
	/// <param name="periodKbn">Period kbn</param>
	/// <param name="orderByKbn">Order by kbn</param>
	private void SetProductSalesRankingList(string periodKbn, string orderByKbn)
	{
		var productSalesRankingList = new SummaryReportService().GetTop10ProductSalesRankingList(
			this.LoginOperatorShopId,
			orderByKbn,
			GetBeginDateTime(periodKbn),
			GetEndDateTime(periodKbn));

		rProductSalesRankingList.DataSource = productSalesRankingList;
		rProductSalesRankingList.DataBind();
	}

	/// <summary>
	/// Get high light css
	/// </summary>
	/// <param name="index">An index</param>
	/// <returns>A high light css class or empty</returns>
	protected string GetHighLightCss(int index)
	{
		var result = (index < 3)
			? CONST_HIGHLIGHT_CLASS
			: string.Empty;
		return result;
	}

	/// <summary>
	/// Get order by number high light css
	/// </summary>
	/// <param name="index">An index</param>
	/// <returns>A high light css class or empty</returns>
	protected string GetOrderByNumberHighLightCss(int index, string orderByKbn)
	{
		var result = ((index < 3)
				&& (orderByKbn == SummaryReportModel.RANKING_REPORT_ORDER_BY_COUNT))
			? CONST_HIGHLIGHT_CLASS
			: string.Empty;
		return result;
	}

	/// <summary>
	/// Get order by amount high light css
	/// </summary>
	/// <param name="index">An index</param>
	/// <returns>A high light css class or empty</returns>
	protected string GetOrderByAmountHighLightCss(int index, string orderByKbn)
	{
		var result = ((index < 3)
				&& (orderByKbn == SummaryReportModel.RANKING_REPORT_ORDER_BY_AMOUNT))
			? CONST_HIGHLIGHT_CLASS
			: string.Empty;
		return result;
	}

	/// <summary>
	/// Set product variation sales ranking list
	/// </summary>
	/// <param name="periodKbn">Period kbn</param>
	/// <param name="orderByKbn">Order by kbn</param>
	private void SetProductVariationSalesRankingList(string periodKbn, string orderByKbn)
	{
		var productVariationSalesRankingList = new SummaryReportService().GetTop10ProductVariationSalesRankingList(
			this.LoginOperatorShopId,
			orderByKbn,
			GetBeginDateTime(periodKbn),
			GetEndDateTime(periodKbn));

		rProductVariationSalesRankingList.DataSource = productVariationSalesRankingList;
		rProductVariationSalesRankingList.DataBind();
	}

	/// <summary>
	/// Change ranking period link button css class
	/// </summary>
	/// <param name="periodKbn">Period kbn</param>
	private void ChangeRankingPeriodLinkButtonCssClass(string periodKbn)
	{
		lbRankingPeriodYesterday.CssClass = (periodKbn == Constants.FLG_SUMMARYREPORT_PERIOD_KBN_YESTERDAY)
			? CONST_BUTTON_HAS_CURRENT_CLASS
			: CONST_BUTTON_NOT_HAS_CURRENT_CLASS;
		lbRankingPeriodLast7Days.CssClass = (periodKbn == Constants.FLG_SUMMARYREPORT_PERIOD_KBN_LAST_SEVEN_DAYS)
			? CONST_BUTTON_HAS_CURRENT_CLASS
			: CONST_BUTTON_NOT_HAS_CURRENT_CLASS;
		lbRankingPeriodLastMonth.CssClass = (periodKbn == Constants.FLG_SUMMARYREPORT_PERIOD_KBN_LAST_MONTH)
			? CONST_BUTTON_HAS_CURRENT_CLASS
			: CONST_BUTTON_NOT_HAS_CURRENT_CLASS;
		lbRankingPeriodThisMonth.CssClass = (periodKbn == Constants.FLG_SUMMARYREPORT_PERIOD_KBN_THIS_MONTH)
			? CONST_BUTTON_HAS_CURRENT_CLASS
			: CONST_BUTTON_NOT_HAS_CURRENT_CLASS;
		lbRankingPeriodThisYear.CssClass = (periodKbn == Constants.FLG_SUMMARYREPORT_PERIOD_KBN_THIS_YEAR)
			? CONST_BUTTON_HAS_CURRENT_CLASS
			: CONST_BUTTON_NOT_HAS_CURRENT_CLASS;
	}

	/// <summary>
	/// Get adv code title html encoded
	/// </summary>
	/// <param name="periodKbn">The period kbn</param>
	/// <param name="orderByKbn">Order by kbn</param>
	/// <returns>A adv code title</returns>
	protected string GetAdvCodeTitleHtmlEncoded(string periodKbn, string orderByKbn)
	{
		var advertisingCodeTitle = ValueText.GetValueText(
			Constants.TABLE_SUMMARYREPORT,
			VALUETEXT_PARAM_SUMMARYREPORT_RANKING_TITLE_KBN,
			(orderByKbn == SummaryReportModel.RANKING_REPORT_ORDER_BY_COUNT)
				? VALUETEXT_PARAM_SUMMARYREPORT_RANKING_TITLE_KBN_COUNT_ADVCODE
				: VALUETEXT_PARAM_SUMMARYREPORT_RANKING_TITLE_KBN_PRICE_ADVCODE);

		var resutl = WebSanitizer.HtmlEncode(
			string.Format("{0}（{1}）",
				advertisingCodeTitle,
				GetRankingPeriodText(periodKbn)));
		return resutl;
	}

	/// <summary>
	/// Get product sales title html encoded
	/// </summary>
	/// <param name="periodKbn">The period kbn</param>
	/// <param name="orderByKbn">Order by kbn</param>
	/// <returns>A product sales title</returns>
	protected string GetProductSalesTitleHtmlEncoded(string periodKbn, string orderByKbn)
	{
		var productSalesTitle = ValueText.GetValueText(
			Constants.TABLE_SUMMARYREPORT,
			VALUETEXT_PARAM_SUMMARYREPORT_RANKING_TITLE_KBN,
			(orderByKbn == SummaryReportModel.RANKING_REPORT_ORDER_BY_COUNT)
				? VALUETEXT_PARAM_SUMMARYREPORT_RANKING_TITLE_KBN_COUNT_PRODUCTSALES
				: VALUETEXT_PARAM_SUMMARYREPORT_RANKING_TITLE_KBN_PRICE_PRODUCTSALES);

		var resutl = WebSanitizer.HtmlEncode(
			string.Format("{0}（{1}）",
				productSalesTitle,
				GetRankingPeriodText(periodKbn)));
		return resutl;
	}

	/// <summary>
	/// Get ranking period text
	/// </summary>
	/// <param name="periodKbn">The period kbn</param>
	/// <returns>A ranking period text</returns>
	protected static string GetRankingPeriodText(string periodKbn)
	{
		var result = ValueText.GetValueText(
			Constants.TABLE_SUMMARYREPORT,
			Constants.FIELD_SUMMARYREPORT_PERIOD_KBN,
			periodKbn);
		return result;
	}

	/// <summary>
	/// Get begin date time
	/// </summary>
	/// <param name="periodKbn">Period kbn</param>
	/// <returns>Begin date date time</returns>
	protected static DateTime GetBeginDateTime(string periodKbn)
	{
		switch (periodKbn)
		{
			case Constants.FLG_SUMMARYREPORT_PERIOD_KBN_YESTERDAY:
				return DateTime.Today.AddDays(-1);

			case Constants.FLG_SUMMARYREPORT_PERIOD_KBN_LAST_SEVEN_DAYS:
				return DateTimeUtility.GetDatePastByDay(7);

			case Constants.FLG_SUMMARYREPORT_PERIOD_KBN_THIS_MONTH:
				return DateTimeUtility.GetFirstDateOfCurrentMonth();

			case Constants.FLG_SUMMARYREPORT_PERIOD_KBN_LAST_MONTH:
				return DateTimeUtility.GetFirstDateOfLastMonth();

			case Constants.FLG_SUMMARYREPORT_PERIOD_KBN_THIS_YEAR:
				return DateTimeUtility.GetFirstDateOfCurrentYear();

			default:
				return DateTime.Today;
		}
	}

	/// <summary>
	/// Get end date time
	/// </summary>
	/// <param name="periodKbn">Period kbn</param>
	/// <returns>End date time</returns>
	protected static DateTime GetEndDateTime(string periodKbn)
	{
		switch (periodKbn)
		{
			case Constants.FLG_SUMMARYREPORT_PERIOD_KBN_YESTERDAY:
				return DateTime.Today;

			case Constants.FLG_SUMMARYREPORT_PERIOD_KBN_LAST_MONTH:
				return DateTimeUtility.GetFirstDateOfCurrentMonth();

			case Constants.FLG_SUMMARYREPORT_PERIOD_KBN_LAST_SEVEN_DAYS:
				return DateTime.Today;

			case Constants.FLG_SUMMARYREPORT_PERIOD_KBN_THIS_MONTH:
			case Constants.FLG_SUMMARYREPORT_PERIOD_KBN_THIS_YEAR:
				return DateTime.Now;

			default:
				return DateTime.Now;
		}
	}
	#endregion

	#region ~Properties
	/// <summary>Latest report</summary>
	protected LatestReport LatestReport
	{
		get { return (LatestReport)ViewState["LatestReport"]; }
		set { ViewState["LatestReport"] = value; }
	}
	/// <summary>Order status report</summary>
	protected OrderStatusReport OrderStatusReport
	{
		get { return (OrderStatusReport)ViewState["OrderStatusReport"]; }
		set { ViewState["OrderStatusReport"] = value; }
	}
	/// <summary>Incident report</summary>
	protected IncidentReport IncidentReport
	{
		get { return (IncidentReport)ViewState["IncidentReport"]; }
		set { ViewState["IncidentReport"] = value; }
	}
	/// <summary>Product stock report</summary>
	protected ProductStockReport ProductStockReport
	{
		get { return (ProductStockReport)ViewState["ProductStockReport"]; }
		set { ViewState["ProductStockReport"] = value; }
	}
	/// <summary> TaskScheduleHistory </summary>
	protected TaskScheduleHistory TaskScheduleHistory
	{
		get { return (TaskScheduleHistory)ViewState["TaskScheduleHistory"]; }
		set { ViewState["TaskScheduleHistory"] = value; }
	}
	/// <summary>The current sales revenue</summary>
	protected string CurrentSalesRevenue
	{
		get { return (string)ViewState["CurrentSalesRevenue"]; }
		set { ViewState["CurrentSalesRevenue"] = value; }
	}
	/// <summary>Ranking period kbn</summary>
	protected string RankingPeriodKbn
	{
		get { return (string)ViewState["RankingPeriodKbn"]; }
		set { ViewState["RankingPeriodKbn"] = value; }
	}
	/// <summary>Advertising code order by kbn</summary>
	protected string AdvertisingCodeOrderByKbn
	{
		get { return (string)ViewState["AdvertisingCodeOrderByKbn"]; }
		set { ViewState["AdvertisingCodeOrderByKbn"] = value; }
	}
	/// <summary>Product sales order by kbn</summary>
	protected string ProductSalesOrderByKbn
	{
		get { return (string)ViewState["ProductSalesOrderByKbn"]; }
		set { ViewState["ProductSalesOrderByKbn"] = value; }
	}
	/// <summary>Summary Root Url</summary>
	protected string SummaryRootUrl
	{
		get { return (Constants.PATH_ROOT + Constants.PAGE_MANAGER_SUMMARY_INFORMATION); }
	}
	/// <summary>Order workflow getter URL</summary>
	protected string OrderWorkflowGetterUrl
	{
		get { return (Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDERWORKFLOW_GETTER); }
	}
	#endregion
}