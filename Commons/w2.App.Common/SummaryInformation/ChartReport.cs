/*
=========================================================================================================
  Module      : Chart Report (ChartReport.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.Util;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain.SummaryReport;

namespace w2.App.Common.SummaryInformation
{
	/// <summary>
	/// Chart report
	/// </summary>
	[Serializable]
	public class ChartReport
	{
		/// <summary>Type chart type: bar</summary>
		private const string CONST_CHART_TYPE_BAR = "bar";
		/// <summary>Type chart type: line</summary>
		private const string CONST_CHART_TYPE_LINE = "line";
		/// <summary>Chart color: user</summary>
		private const string CONST_CHART_COLOR_USER = "color-user";
		/// <summary>Chart color: price</summary>
		private const string CONST_CHART_COLOR_PRICE = "color-price";
		/// <summary>Chart color: order</summary>
		private const string CONST_CHART_COLOR_ORDER = "color-order";
		/// <summary>Chart color: apply</summary>
		private const string CONST_CHART_COLOR_APPLY = "color-apply";
		/// <summary>Chart color: cancel</summary>
		private const string CONST_CHART_COLOR_CANCEL = "color-cancel";
		/// <summary>Chart color: signup</summary>
		private const string CONST_CHART_COLOR_SIGNUP = "color-signup";
		/// <summary>Chart color: unsubscribe</summary>
		private const string CONST_CHART_COLOR_UNSUBSCRIBE = "color-unsubscribe";
		/// <summary>Chart y axist id: 1</summary>
		private const string CONST_CHART_Y_AXIS_ID_1 = "1";
		/// <summary>Chart y axist id: 2</summary>
		private const string CONST_CHART_Y_AXIS_ID_2 = "2";

		/// <summary>Number chart data summary display default</summary>
		private const int CONST_NUMBER_CHART_DATA_SUMMARY_DISPLAY_DEFAULT = 7;

		/// <summary>Format date: short date (only month and year)</summary>
		private const string CONST_FORMATDATE_SHORTDATE_ONLY_MONTH_AND_YEAR = "yyyy/MM";
		/// <summary>Format date: short date</summary>
		private const string CONST_FORMATDATE_SHORTDATE = "yyyy/MM/dd";

		/// <summary>Chart title type</summary>
		private const string VALUETEXT_PARAM_SUMMARYREPORT_CHART_TITLE_KBN = "chart_title_kbn";
		/// <summary>Chart label type</summary>
		private const string VALUETEXT_PARAM_SUMMARYREPORT_CHART_LABEL_KBN = "chart_label_kbn";
		/// <summary>Chart label custom type</summary>
		private const string VALUETEXT_PARAM_SUMMARYREPORT_CHART_LABEL_CUSTOM_KBN = "chart_label_custom_kbn";

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="periodKbn">Period kbn</param>
		public ChartReport(string periodKbn = null)
		{
			this.PeriodKbn = periodKbn;
		}

		/// <summary>
		/// Create reports
		/// </summary>
		/// <returns>Reports</returns>
		public List<ChartReport> CreateReports()
		{
			var results = new List<ChartReport>();
			if (string.IsNullOrEmpty(this.PeriodKbn)) return results;

			var reportDatePeriod = GetReportDatePeriod();
			var reports = new SummaryReportService().GetSummaryReportsByPeriodKbn(
				this.PeriodKbn,
				reportDatePeriod.Item1,
				reportDatePeriod.Item2);
			if (reports.Any() == false) return results;

			// Need to add in order according to the order on the screen
			results.Add(CreateUserAccessChartReport(this.PeriodKbn, reports));
			results.Add(CreateOrderChartReport(this.PeriodKbn, reports));
			results.Add(CreateConversionChartReport(this.PeriodKbn, reports));
			results.Add(CreateLtvChartReport(this.PeriodKbn, reports));

			// For case fixed purchase option enabled
			if (Constants.FIXEDPURCHASE_OPTION_ENABLED)
			{
				results.Add(CreateFixedPurchaseChartReport(this.PeriodKbn, reports));
			}

			results.Add(CreateUserChartReport(this.PeriodKbn, reports));
			results.Add(CreateMembershipCountChartReport(this.PeriodKbn, reports));

			// For case fixed purchase option enabled
			if (Constants.FIXEDPURCHASE_OPTION_ENABLED)
			{
				results.Add(CreateFixedPurchaseCountChartReport(this.PeriodKbn, reports));
			}
			return results;
		}

		/// <summary>
		/// Get report date period
		/// </summary>
		/// <returns>Two result: start date and end date</returns>
		private Tuple<DateTime, DateTime> GetReportDatePeriod()
		{
			var today = DateTime.Today;
			switch (this.PeriodKbn)
			{
				case Constants.FLG_SUMMARYREPORT_PERIOD_KBN_LAST_SEVEN_DAYS:
					return new Tuple<DateTime, DateTime>(
						today.AddDays(-7),
						today);

				case Constants.FLG_SUMMARYREPORT_PERIOD_KBN_THIS_MONTH:
					return new Tuple<DateTime, DateTime>(
						DateTimeUtility.GetFirstDateOfCurrentMonth(),
						DateTimeUtility.GetFirstDateOfNextMonth());

				case Constants.FLG_SUMMARYREPORT_PERIOD_KBN_LAST_MONTH:
					return new Tuple<DateTime, DateTime>(
						DateTimeUtility.GetFirstDateOfLastMonth(),
						DateTimeUtility.GetFirstDateOfCurrentMonth());

				case Constants.FLG_SUMMARYREPORT_PERIOD_KBN_THIS_YEAR:
					return new Tuple<DateTime, DateTime>(
						DateTimeUtility.GetFirstDateOfCurrentYear(),
						DateTimeUtility.GetFirstDateOfNextYear());

				default:
					return new Tuple<DateTime, DateTime>(today, today);
			}
		}

		/// <summary>
		/// Create user access chart report
		/// </summary>
		/// <param name="periodKbn">Period kbn</param>
		/// <param name="reports">Reports</param>
		/// <returns>User access chart report</returns>
		private ChartReport CreateUserAccessChartReport(
			string periodKbn,
			SummaryReportModel[] reports)
		{
			var dateModalList = new List<string>();
			var accessCountModalList = new List<string>();
			var summaryChart = CreateChartDetailsForReport(
				periodKbn,
				Constants.FLG_SUMMARYREPORT_DATA_KBN_USER_ACCESS,
				reports,
				dateModalList,
				accessCountModalList,
				new List<string>());

			var accessCountList = new string[summaryChart.Item1];
			var dateList = new string[summaryChart.Item1];
			CreateSummaryForChart(
				summaryChart.Item2,
				summaryChart.Item1,
				dateModalList,
				dateList,
				accessCountModalList,
				accessCountList);

			var options = GetChartOptions(Constants.FLG_SUMMARYREPORT_DATA_KBN_USER_ACCESS);
			var dataSetsSummary = GetChartDataSetsObjectList(
				Constants.FLG_SUMMARYREPORT_DATA_KBN_USER_ACCESS,
				accessCountList);
			var dataSetsModal = GetChartDataSetsObjectList(
				Constants.FLG_SUMMARYREPORT_DATA_KBN_USER_ACCESS,
				accessCountModalList.ToArray());
			var dataSummary = CreateChartDataModalObjectForReport(
				dateList,
				dataSetsSummary);
			var dataModal = CreateChartDataModalObjectForReport(
				dateModalList.ToArray(),
				dataSetsModal);
			var result = CreateChartObjectForReport(
				options,
				dataSummary,
				dataModal);
			return result;
		}

		/// <summary>
		/// Create order chart report
		/// </summary>
		/// <param name="periodKbn">Period Kbn</param>
		/// <param name="reports">Reports</param>
		/// <returns>Order chart report</returns>
		private ChartReport CreateOrderChartReport(
			string periodKbn,
			SummaryReportModel[] reports)
		{
			var dateModalList = new List<string>();
			var orderCountModalList = new List<string>();
			var orderAmountModalList = new List<string>();
			var summaryChart = CreateChartDetailsForReport(
				periodKbn,
				Constants.FLG_SUMMARYREPORT_DATA_KBN_ORDER_COUNT,
				reports,
				dateModalList,
				orderCountModalList,
				orderAmountModalList);

			var orderCountList = new string[summaryChart.Item1];
			var orderAmountList = new string[summaryChart.Item1];
			var dateList = new string[summaryChart.Item1];
			CreateSummaryForChart(
				summaryChart.Item2,
				summaryChart.Item1,
				dateModalList,
				dateList,
				orderCountModalList,
				orderCountList,
				orderAmountModalList,
				orderAmountList);

			var options = GetChartOptions(Constants.FLG_SUMMARYREPORT_DATA_KBN_ORDER_COUNT);
			var dataSetsSummary = GetChartDataSetsObjectList(
				Constants.FLG_SUMMARYREPORT_DATA_KBN_ORDER_COUNT,
				orderAmountList,
				orderCountList);
			var dataSetsModal = GetChartDataSetsObjectList(
				Constants.FLG_SUMMARYREPORT_DATA_KBN_ORDER_COUNT,
				orderAmountModalList.ToArray(),
				orderCountModalList.ToArray());
			var dataSummary = CreateChartDataModalObjectForReport(
				dateList,
				dataSetsSummary);
			var dataModal = CreateChartDataModalObjectForReport(
				dateModalList.ToArray(),
				dataSetsModal);
			var result = CreateChartObjectForReport(
				options,
				dataSummary,
				dataModal);
			return result;
		}

		/// <summary>
		/// Create conversion chart report
		/// </summary>
		/// <param name="periodKbn">Period Kbn</param>
		/// <param name="reports">Reports</param>
		/// <returns>Conversion chart report</returns>
		private ChartReport CreateConversionChartReport(
			string periodKbn,
			SummaryReportModel[] reports)
		{
			var dateModalList = new List<string>();
			var conversionModalList = new List<string>();
			var summaryChart = CreateChartDetailsForReport(
				periodKbn,
				Constants.FLG_SUMMARYREPORT_DATA_KBN_CONVERSION,
				reports,
				dateModalList,
				conversionModalList,
				new List<string>());

			var conversionList = new string[summaryChart.Item1];
			var dateList = new string[summaryChart.Item1];
			CreateSummaryForChart(
				summaryChart.Item2,
				summaryChart.Item1,
				dateModalList,
				dateList,
				conversionModalList,
				conversionList);

			var options = GetChartOptions(Constants.FLG_SUMMARYREPORT_DATA_KBN_CONVERSION);
			var dataSetsSummary = GetChartDataSetsObjectList(
				Constants.FLG_SUMMARYREPORT_DATA_KBN_CONVERSION,
				conversionList);
			var dataSetsModal = GetChartDataSetsObjectList(
				Constants.FLG_SUMMARYREPORT_DATA_KBN_CONVERSION,
				conversionModalList.ToArray());
			var dataSummary = CreateChartDataModalObjectForReport(
				dateList,
				dataSetsSummary);
			var dataModal = CreateChartDataModalObjectForReport(
				dateModalList.ToArray(),
				dataSetsModal);
			var result = CreateChartObjectForReport(
				options,
				dataSummary,
				dataModal);
			return result;
		}

		/// <summary>
		/// Create Ltv chart report
		/// </summary>
		/// <param name="periodKbn">Period Kbn</param>
		/// <param name="reports">Reports</param>
		/// <returns>Ltv chart report</returns>
		private ChartReport CreateLtvChartReport(
			string periodKbn,
			SummaryReportModel[] reports)
		{
			var dateModalList = new List<string>();
			var ltvModalList = new List<string>();
			var summaryChart = CreateChartDetailsForReport(
				periodKbn,
				Constants.FLG_SUMMARYREPORT_DATA_KBN_LTV,
				reports,
				dateModalList,
				ltvModalList,
				new List<string>());

			var ltvList = new string[summaryChart.Item1];
			var dateList = new string[summaryChart.Item1];
			CreateSummaryForChart(
				summaryChart.Item2,
				summaryChart.Item1,
				dateModalList,
				dateList,
				ltvModalList,
				ltvList);

			var options = GetChartOptions(Constants.FLG_SUMMARYREPORT_DATA_KBN_LTV);
			var dataSetsSummary = GetChartDataSetsObjectList(
				Constants.FLG_SUMMARYREPORT_DATA_KBN_LTV,
				ltvList);
			var dataSetsModal = GetChartDataSetsObjectList(
				Constants.FLG_SUMMARYREPORT_DATA_KBN_LTV,
				ltvModalList.ToArray());
			var dataSummary = CreateChartDataModalObjectForReport(
				dateList,
				dataSetsSummary);
			var dataModal = CreateChartDataModalObjectForReport(
				dateModalList.ToArray(),
				dataSetsModal);
			var result = CreateChartObjectForReport(
				options,
				dataSummary,
				dataModal);
			return result;
		}

		/// <summary>
		/// Create fixed purchase chart report
		/// </summary>
		/// <param name="periodKbn">Period Kbn</param>
		/// <param name="reports">Reports</param>
		/// <returns>Fixed purchase chart report</returns>
		private ChartReport CreateFixedPurchaseChartReport(
			string periodKbn,
			SummaryReportModel[] reports)
		{
			var dateModalList = new List<string>();
			var orderFixedPurchaseRegisterModalList = new List<string>();
			var orderFixedPurchaseCancelModalList = new List<string>();
			var summaryChart = CreateChartDetailsForReport(
				periodKbn,
				Constants.FLG_SUMMARYREPORT_DATA_KBN_FIXED_PURCHASE_REGISTER,
				reports,
				dateModalList,
				orderFixedPurchaseRegisterModalList,
				orderFixedPurchaseCancelModalList);

			var orderFixedPurchaseRegisterList = new string[summaryChart.Item1];
			var orderFixedPurchaseCancelList = new string[summaryChart.Item1];
			var dateList = new string[summaryChart.Item1];
			CreateSummaryForChart(
				summaryChart.Item2,
				summaryChart.Item1,
				dateModalList,
				dateList,
				orderFixedPurchaseRegisterModalList,
				orderFixedPurchaseRegisterList,
				orderFixedPurchaseCancelModalList,
				orderFixedPurchaseCancelList);

			var options = GetChartOptions(Constants.FLG_SUMMARYREPORT_DATA_KBN_FIXED_PURCHASE_REGISTER);
			var dataSetsSummary = GetChartDataSetsObjectList(
				Constants.FLG_SUMMARYREPORT_DATA_KBN_FIXED_PURCHASE_REGISTER,
				orderFixedPurchaseRegisterList,
				orderFixedPurchaseCancelList);
			var dataSetsModal = GetChartDataSetsObjectList(
				Constants.FLG_SUMMARYREPORT_DATA_KBN_FIXED_PURCHASE_REGISTER,
				orderFixedPurchaseRegisterModalList.ToArray(),
				orderFixedPurchaseCancelModalList.ToArray());
			var dataSummary = CreateChartDataModalObjectForReport(
				dateList,
				dataSetsSummary);
			var dataModal = CreateChartDataModalObjectForReport(
				dateModalList.ToArray(),
				dataSetsModal);
			var result = CreateChartObjectForReport(
				options,
				dataSummary,
				dataModal);
			return result;
		}

		/// <summary>
		/// Create user chart report
		/// </summary>
		/// <param name="periodKbn">Period Kbn</param>
		/// <param name="reports">Reports</param>
		/// <returns>User chart report</returns>
		private ChartReport CreateUserChartReport(
			string periodKbn,
			SummaryReportModel[] reports)
		{
			var dateModalList = new List<string>();
			var userRegisterModalList = new List<string>();
			var userWithdrawalModalList = new List<string>();
			var summaryChart = CreateChartDetailsForReport(
				periodKbn,
				Constants.FLG_SUMMARYREPORT_DATA_KBN_USER_REGISTER,
				reports,
				dateModalList,
				userRegisterModalList,
				userWithdrawalModalList);

			var userRegisterList = new string[summaryChart.Item1];
			var userWithdrawalList = new string[summaryChart.Item1];
			var dateList = new string[summaryChart.Item1];
			CreateSummaryForChart(
				summaryChart.Item2,
				summaryChart.Item1,
				dateModalList,
				dateList,
				userRegisterModalList,
				userRegisterList,
				userWithdrawalModalList,
				userWithdrawalList);

			var options = GetChartOptions(Constants.FLG_SUMMARYREPORT_DATA_KBN_USER_REGISTER);
			var dataSetsSummary = GetChartDataSetsObjectList(
				Constants.FLG_SUMMARYREPORT_DATA_KBN_USER_REGISTER,
				userRegisterList,
				userWithdrawalList);
			var dataSetsModal = GetChartDataSetsObjectList(
				Constants.FLG_SUMMARYREPORT_DATA_KBN_USER_REGISTER,
				userRegisterModalList.ToArray(),
				userWithdrawalModalList.ToArray());
			var dataSummary = CreateChartDataModalObjectForReport(
				dateList,
				dataSetsSummary);
			var dataModal = CreateChartDataModalObjectForReport(
				dateModalList.ToArray(),
				dataSetsModal);
			var result = CreateChartObjectForReport(
				options,
				dataSummary,
				dataModal);
			return result;
		}

		/// <summary>
		/// Create membership count chart report
		/// </summary>
		/// <param name="periodKbn">Period Kbn</param>
		/// <param name="reports">Reports</param>
		/// <returns>Membership count chart report</returns>
		private ChartReport CreateMembershipCountChartReport(
			string periodKbn,
			SummaryReportModel[] reports)
		{
			var dateModalList = new List<string>();
			var memberCountModalList = new List<string>();
			var summaryChart = CreateChartDetailsForReport(
				periodKbn,
				Constants.FLG_SUMMARYREPORT_DATA_KBN_MEMBERSHIP_COUNT,
				reports,
				dateModalList,
				memberCountModalList,
				new List<string>());

			var memberCountList = new string[summaryChart.Item1];
			var dateList = new string[summaryChart.Item1];
			CreateSummaryForChart(
				summaryChart.Item2,
				summaryChart.Item1,
				dateModalList,
				dateList,
				memberCountModalList,
				memberCountList);

			var options = GetChartOptions(Constants.FLG_SUMMARYREPORT_DATA_KBN_MEMBERSHIP_COUNT);
			var dataSetsSummary = GetChartDataSetsObjectList(
				Constants.FLG_SUMMARYREPORT_DATA_KBN_MEMBERSHIP_COUNT,
				memberCountList);
			var dataSetsModal = GetChartDataSetsObjectList(
				Constants.FLG_SUMMARYREPORT_DATA_KBN_MEMBERSHIP_COUNT,
				memberCountModalList.ToArray());
			var dataSummary = CreateChartDataModalObjectForReport(
				dateList,
				dataSetsSummary);
			var dataModal = CreateChartDataModalObjectForReport(
				dateModalList.ToArray(),
				dataSetsModal);
			var result = CreateChartObjectForReport(
				options,
				dataSummary,
				dataModal);
			return result;
		}

		/// <summary>
		/// Create fixed purchase count access chart report
		/// </summary>
		/// <param name="periodKbn">Period Kbn</param>
		/// <param name="reports">Reports</param>
		/// <returns>Fixed purchase count chart report</returns>
		private ChartReport CreateFixedPurchaseCountChartReport(
			string periodKbn,
			SummaryReportModel[] reports)
		{
			var dateModalList = new List<string>();
			var fixedPurchaseCountModalList = new List<string>();
			var summaryChart = CreateChartDetailsForReport(
				periodKbn,
				Constants.FLG_SUMMARYREPORT_DATA_KBN_FIXED_PURCHASE_COUNT,
				reports,
				dateModalList,
				fixedPurchaseCountModalList,
				new List<string>());

			var fixedPurchaseCountList = new string[summaryChart.Item1];
			var dateList = new string[summaryChart.Item1];
			CreateSummaryForChart(
				summaryChart.Item2,
				summaryChart.Item1,
				dateModalList,
				dateList,
				fixedPurchaseCountModalList,
				fixedPurchaseCountList);

			var options = GetChartOptions(Constants.FLG_SUMMARYREPORT_DATA_KBN_FIXED_PURCHASE_COUNT);
			var dataSetsSummary = GetChartDataSetsObjectList(
				Constants.FLG_SUMMARYREPORT_DATA_KBN_FIXED_PURCHASE_COUNT,
				fixedPurchaseCountList);
			var dataSetsModal = GetChartDataSetsObjectList(
				Constants.FLG_SUMMARYREPORT_DATA_KBN_FIXED_PURCHASE_COUNT,
				fixedPurchaseCountModalList.ToArray());
			var dataSummary = CreateChartDataModalObjectForReport(
				dateList,
				dataSetsSummary);
			var dataModal = CreateChartDataModalObjectForReport(
				dateModalList.ToArray(),
				dataSetsModal);
			var result = CreateChartObjectForReport(
				options,
				dataSummary,
				dataModal);
			return result;
		}

		/// <summary>
		/// Create chart details for report
		/// </summary>
		/// <param name="periodKbn">Period kbn</param>
		/// <param name="dataKbn">Data kbn</param>
		/// <param name="reports">Reports</param>
		/// <param name="dateList">Date list</param>
		/// <param name="firstDetailList">First detail list</param>
		/// <param name="secondDetailList">Second detail list</param>
		/// <returns>Data for create summary chart report</returns>
		private Tuple<int, int> CreateChartDetailsForReport(
			string periodKbn,
			string dataKbn,
			SummaryReportModel[] reports,
			List<string> dateList,
			List<string> firstDetailList,
			List<string> secondDetailList)
		{
			var isCurrentMonth = (periodKbn == Constants.FLG_SUMMARYREPORT_PERIOD_KBN_THIS_MONTH);
			var isCurrentYear = (periodKbn == Constants.FLG_SUMMARYREPORT_PERIOD_KBN_THIS_YEAR);
			var currentDate = DateTime.Today;
			var totalPastDayOrMonth = 0;

			foreach (var report in reports.Where(item => (item.DataKbn == dataKbn)))
			{
				var reportDate = report.ReportDate;
				dateList.Add(reportDate.ToString(GetFormatDate(periodKbn)));
				if ((isCurrentYear && (reportDate.Month <= currentDate.Month))
					|| (isCurrentMonth && (reportDate.Day <= currentDate.Day)))
				{
					totalPastDayOrMonth++;
				}
			}

			// Add summary report list
			var dataKbnExtend = GetDataKbnExtend(dataKbn);
			if (dataKbn == dataKbnExtend)
			{
				firstDetailList.AddRange(reports
					.Where(item => (item.DataKbn == dataKbn))
					.Select(item => item.Data.ToString())
					.ToList());
			}
			else
			{
				firstDetailList.AddRange(reports
					.Where(item => (item.DataKbn == dataKbn))
					.Select(item => item.Data.ToString("0"))
					.ToList());
				secondDetailList.AddRange(reports
					.Where(item => (item.DataKbn == dataKbnExtend))
					.Select(item => item.Data.ToString("0"))
					.ToList());
			}

			var summarySize = GetSizeSummaryForChart(
				dateList.Count,
				totalPastDayOrMonth,
				isCurrentMonth,
				isCurrentYear);
			var startIndex = GetStartIndexForChart(
				dateList.Count,
				totalPastDayOrMonth,
				isCurrentMonth,
				isCurrentYear);
			var result = new Tuple<int, int>(summarySize, startIndex);
			return result;
		}

		/// <summary>
		/// Get chart options
		/// </summary>
		/// <param name="dataKbn">Data kbn</param>
		/// <returns>Chart options</returns>
		private OptionsObject GetChartOptions(string dataKbn)
		{
			switch (dataKbn)
			{
				case Constants.FLG_SUMMARYREPORT_DATA_KBN_USER_ACCESS:
					return CreateChartOptionsObjectForReport(
						ValueText.GetValueText(
							Constants.TABLE_SUMMARYREPORT,
							VALUETEXT_PARAM_SUMMARYREPORT_CHART_TITLE_KBN,
							Constants.FLG_SUMMARYREPORT_DATA_KBN_USER_ACCESS),
						CONST_CHART_TYPE_BAR);

				case Constants.FLG_SUMMARYREPORT_DATA_KBN_ORDER_AMOUNT:
				case Constants.FLG_SUMMARYREPORT_DATA_KBN_ORDER_COUNT:
					return CreateChartOptionsObjectForReport(
						ValueText.GetValueText(
							Constants.TABLE_SUMMARYREPORT,
							VALUETEXT_PARAM_SUMMARYREPORT_CHART_TITLE_KBN,
							string.Format("{0}_{1}",
								Constants.FLG_SUMMARYREPORT_DATA_KBN_ORDER_COUNT,
								Constants.FLG_SUMMARYREPORT_DATA_KBN_ORDER_AMOUNT)),
						CONST_CHART_TYPE_BAR);

				case Constants.FLG_SUMMARYREPORT_DATA_KBN_CONVERSION:
					return CreateChartOptionsObjectForReport(
						ValueText.GetValueText(
							Constants.TABLE_SUMMARYREPORT,
							VALUETEXT_PARAM_SUMMARYREPORT_CHART_TITLE_KBN,
							Constants.FLG_SUMMARYREPORT_DATA_KBN_CONVERSION),
						CONST_CHART_TYPE_LINE);

				case Constants.FLG_SUMMARYREPORT_DATA_KBN_LTV:
					return CreateChartOptionsObjectForReport(
						ValueText.GetValueText(
							Constants.TABLE_SUMMARYREPORT,
							VALUETEXT_PARAM_SUMMARYREPORT_CHART_TITLE_KBN,
							Constants.FLG_SUMMARYREPORT_DATA_KBN_LTV),
						CONST_CHART_TYPE_LINE);

				case Constants.FLG_SUMMARYREPORT_DATA_KBN_FIXED_PURCHASE_REGISTER:
				case Constants.FLG_SUMMARYREPORT_DATA_KBN_FIXED_PURCHASE_CANCEL:
					return CreateChartOptionsObjectForReport(
						ValueText.GetValueText(
							Constants.TABLE_SUMMARYREPORT,
							VALUETEXT_PARAM_SUMMARYREPORT_CHART_TITLE_KBN,
							string.Format("{0}_{1}",
								Constants.FLG_SUMMARYREPORT_DATA_KBN_FIXED_PURCHASE_REGISTER,
								Constants.FLG_SUMMARYREPORT_DATA_KBN_FIXED_PURCHASE_CANCEL)),
						CONST_CHART_TYPE_BAR);

				case Constants.FLG_SUMMARYREPORT_DATA_KBN_USER_REGISTER:
				case Constants.FLG_SUMMARYREPORT_DATA_KBN_USER_WITHDRAWAL:
					return CreateChartOptionsObjectForReport(
						ValueText.GetValueText(
							Constants.TABLE_SUMMARYREPORT,
							VALUETEXT_PARAM_SUMMARYREPORT_CHART_TITLE_KBN,
							string.Format("{0}_{1}",
								Constants.FLG_SUMMARYREPORT_DATA_KBN_USER_REGISTER,
								Constants.FLG_SUMMARYREPORT_DATA_KBN_USER_WITHDRAWAL)),
						CONST_CHART_TYPE_BAR);

				case Constants.FLG_SUMMARYREPORT_DATA_KBN_MEMBERSHIP_COUNT:
					return CreateChartOptionsObjectForReport(
						ValueText.GetValueText(
							Constants.TABLE_SUMMARYREPORT,
							VALUETEXT_PARAM_SUMMARYREPORT_CHART_TITLE_KBN,
							Constants.FLG_SUMMARYREPORT_DATA_KBN_MEMBERSHIP_COUNT),
						CONST_CHART_TYPE_BAR);

				case Constants.FLG_SUMMARYREPORT_DATA_KBN_FIXED_PURCHASE_COUNT:
					return CreateChartOptionsObjectForReport(
						ValueText.GetValueText(
							Constants.TABLE_SUMMARYREPORT,
							VALUETEXT_PARAM_SUMMARYREPORT_CHART_TITLE_KBN,
							Constants.FLG_SUMMARYREPORT_DATA_KBN_FIXED_PURCHASE_COUNT),
						CONST_CHART_TYPE_BAR);

				default:
					return new OptionsObject();
			}
		}

		/// <summary>
		/// Get chart data sets object list
		/// </summary>
		/// <param name="dataKbn">Data kbn</param>
		/// <param name="firstDataList">First data list</param>
		/// <param name="secondDataList">Second data list</param>
		/// <returns>Chart data sets object list</returns>
		private DataSetsObject[] GetChartDataSetsObjectList(
			string dataKbn,
			string[] firstDataList,
			string[] secondDataList = null)
		{
			switch (dataKbn)
			{
				case Constants.FLG_SUMMARYREPORT_DATA_KBN_USER_ACCESS:
					var userAccessChartDataSetsObject = new[]
					{
						CreateChartDataSetsObjectForReport(
							CONST_CHART_TYPE_BAR,
							ValueText.GetValueText(
								Constants.TABLE_SUMMARYREPORT,
								VALUETEXT_PARAM_SUMMARYREPORT_CHART_LABEL_KBN,
								Constants.FLG_SUMMARYREPORT_DATA_KBN_USER_ACCESS),
							CONST_CHART_COLOR_USER,
							null,
							firstDataList)
					};
					return userAccessChartDataSetsObject;

				case Constants.FLG_SUMMARYREPORT_DATA_KBN_ORDER_COUNT:
					var orderChartDataSetsObject = new[]
					{
						CreateChartDataSetsObjectForReport(
							CONST_CHART_TYPE_LINE,
							ValueText.GetValueText(
								Constants.TABLE_SUMMARYREPORT,
								VALUETEXT_PARAM_SUMMARYREPORT_CHART_LABEL_KBN,
								string.Format("{0}_{1}",
									Constants.FLG_SUMMARYREPORT_DATA_KBN_ORDER_COUNT,
									Constants.FLG_SUMMARYREPORT_DATA_KBN_ORDER_AMOUNT)),
							CONST_CHART_COLOR_PRICE,
							CONST_CHART_Y_AXIS_ID_1,
							firstDataList),
						CreateChartDataSetsObjectForReport(
							CONST_CHART_TYPE_BAR,
							ValueText.GetValueText(
								Constants.TABLE_SUMMARYREPORT,
								VALUETEXT_PARAM_SUMMARYREPORT_CHART_LABEL_CUSTOM_KBN,
								string.Format("{0}_{1}",
									Constants.FLG_SUMMARYREPORT_DATA_KBN_ORDER_COUNT,
									Constants.FLG_SUMMARYREPORT_DATA_KBN_ORDER_AMOUNT)),
							CONST_CHART_COLOR_ORDER,
							CONST_CHART_Y_AXIS_ID_2,
							secondDataList),
					};
					return orderChartDataSetsObject;

				case Constants.FLG_SUMMARYREPORT_DATA_KBN_CONVERSION:
					var conversionChartDataSetsObject = new[]
					{
						CreateChartDataSetsObjectForReport(
							CONST_CHART_TYPE_LINE,
							ValueText.GetValueText(
								Constants.TABLE_SUMMARYREPORT,
								VALUETEXT_PARAM_SUMMARYREPORT_CHART_TITLE_KBN,
								Constants.FLG_SUMMARYREPORT_DATA_KBN_CONVERSION),
							CONST_CHART_COLOR_PRICE,
							null,
							firstDataList)
					};
					return conversionChartDataSetsObject;

				case Constants.FLG_SUMMARYREPORT_DATA_KBN_LTV:
					var ltvChartDataSetsObject = new[]
					{
						CreateChartDataSetsObjectForReport(
							CONST_CHART_TYPE_LINE,
							ValueText.GetValueText(
								Constants.TABLE_SUMMARYREPORT,
								VALUETEXT_PARAM_SUMMARYREPORT_CHART_TITLE_KBN,
								Constants.FLG_SUMMARYREPORT_DATA_KBN_LTV),
							CONST_CHART_COLOR_PRICE,
							null,
							firstDataList)
					};
					return ltvChartDataSetsObject;

				case Constants.FLG_SUMMARYREPORT_DATA_KBN_FIXED_PURCHASE_REGISTER:
					var orderFixedPurchaseChartDataSetsObject = new[]
					{
						CreateChartDataSetsObjectForReport(
							CONST_CHART_TYPE_BAR,
							ValueText.GetValueText(
								Constants.TABLE_SUMMARYREPORT,
								VALUETEXT_PARAM_SUMMARYREPORT_CHART_LABEL_KBN,
								string.Format("{0}_{1}",
									Constants.FLG_SUMMARYREPORT_DATA_KBN_FIXED_PURCHASE_REGISTER,
									Constants.FLG_SUMMARYREPORT_DATA_KBN_FIXED_PURCHASE_CANCEL)),
							CONST_CHART_COLOR_APPLY,
							CONST_CHART_Y_AXIS_ID_1,
							firstDataList),
						CreateChartDataSetsObjectForReport(
							CONST_CHART_TYPE_BAR,
							ValueText.GetValueText(
								Constants.TABLE_SUMMARYREPORT,
								VALUETEXT_PARAM_SUMMARYREPORT_CHART_LABEL_CUSTOM_KBN,
								string.Format("{0}_{1}",
									Constants.FLG_SUMMARYREPORT_DATA_KBN_FIXED_PURCHASE_REGISTER,
									Constants.FLG_SUMMARYREPORT_DATA_KBN_FIXED_PURCHASE_CANCEL)),
							CONST_CHART_COLOR_CANCEL,
							CONST_CHART_Y_AXIS_ID_1,
							secondDataList),
					};
					return orderFixedPurchaseChartDataSetsObject;

				case Constants.FLG_SUMMARYREPORT_DATA_KBN_USER_REGISTER:
					var userChartDataSetsObject = new[]
					{
						CreateChartDataSetsObjectForReport(
							CONST_CHART_TYPE_BAR,
							ValueText.GetValueText(
								Constants.TABLE_SUMMARYREPORT,
								VALUETEXT_PARAM_SUMMARYREPORT_CHART_LABEL_KBN,
								string.Format("{0}_{1}",
									Constants.FLG_SUMMARYREPORT_DATA_KBN_USER_REGISTER,
									Constants.FLG_SUMMARYREPORT_DATA_KBN_USER_WITHDRAWAL)),
							CONST_CHART_COLOR_SIGNUP,
							CONST_CHART_Y_AXIS_ID_1,
							firstDataList),
						CreateChartDataSetsObjectForReport(
							CONST_CHART_TYPE_BAR,
							ValueText.GetValueText(
								Constants.TABLE_SUMMARYREPORT,
								VALUETEXT_PARAM_SUMMARYREPORT_CHART_LABEL_CUSTOM_KBN,
								string.Format("{0}_{1}",
									Constants.FLG_SUMMARYREPORT_DATA_KBN_USER_REGISTER,
									Constants.FLG_SUMMARYREPORT_DATA_KBN_USER_WITHDRAWAL)),
							CONST_CHART_COLOR_UNSUBSCRIBE,
							CONST_CHART_Y_AXIS_ID_1,
							secondDataList),
					};
					return userChartDataSetsObject;

				case Constants.FLG_SUMMARYREPORT_DATA_KBN_MEMBERSHIP_COUNT:
					var membershipChartDataSetsObject = new[]
					{
						CreateChartDataSetsObjectForReport(
							CONST_CHART_TYPE_BAR,
							ValueText.GetValueText(
								Constants.TABLE_SUMMARYREPORT,
								VALUETEXT_PARAM_SUMMARYREPORT_CHART_TITLE_KBN,
								Constants.FLG_SUMMARYREPORT_DATA_KBN_MEMBERSHIP_COUNT),
							CONST_CHART_COLOR_USER,
							null,
							firstDataList)
					};
					return membershipChartDataSetsObject;

				case Constants.FLG_SUMMARYREPORT_DATA_KBN_FIXED_PURCHASE_COUNT:
					var fixedPurchaseCountChartDataSetsObject = new[]
					{
						CreateChartDataSetsObjectForReport(
							CONST_CHART_TYPE_BAR,
							ValueText.GetValueText(
								Constants.TABLE_SUMMARYREPORT,
								VALUETEXT_PARAM_SUMMARYREPORT_CHART_TITLE_KBN,
								Constants.FLG_SUMMARYREPORT_DATA_KBN_FIXED_PURCHASE_COUNT),
							CONST_CHART_COLOR_USER,
							null,
							firstDataList)
					};
					return fixedPurchaseCountChartDataSetsObject;

				default:
					return new[] { new DataSetsObject() };
			}
		}

		/// <summary>
		/// Get start index for chart
		/// </summary>
		/// <param name="sizeOfReportData">Size of report data</param>
		/// <param name="totalPastDayOrMonth">Total past day or month</param>
		/// <param name="isCurrentMonth">Is current month</param>
		/// <param name="isCurrentYear">Is current year</param>
		/// <returns>Start index</returns>
		private int GetStartIndexForChart(
			int sizeOfReportData,
			int totalPastDayOrMonth,
			bool isCurrentMonth,
			bool isCurrentYear)
		{
			if (sizeOfReportData < CONST_NUMBER_CHART_DATA_SUMMARY_DISPLAY_DEFAULT) return 0;

			if ((isCurrentYear == false)
				&& (isCurrentMonth == false))
			{
				return (sizeOfReportData - CONST_NUMBER_CHART_DATA_SUMMARY_DISPLAY_DEFAULT);
			}

			if (totalPastDayOrMonth > CONST_NUMBER_CHART_DATA_SUMMARY_DISPLAY_DEFAULT)
			{
				return (totalPastDayOrMonth - CONST_NUMBER_CHART_DATA_SUMMARY_DISPLAY_DEFAULT);
			}
			return 0;
		}

		/// <summary>
		/// Get size summary for chart
		/// </summary>
		/// <param name="sizeOfReport">Size of report data</param>
		/// <param name="totalPastDayOrMonth">Total past day or month</param>
		/// <param name="isCurrentMonth">Is current month</param>
		/// <param name="isCurrentYear">Is current year</param>
		/// <returns>Size aummary</returns>
		private int GetSizeSummaryForChart(
			int sizeOfReport,
			int totalPastDayOrMonth,
			bool isCurrentMonth,
			bool isCurrentYear)
		{
			var size = (sizeOfReport < CONST_NUMBER_CHART_DATA_SUMMARY_DISPLAY_DEFAULT)
				? sizeOfReport
				: CONST_NUMBER_CHART_DATA_SUMMARY_DISPLAY_DEFAULT;
			if ((isCurrentYear == false) && (isCurrentMonth == false)) return size;

			size = (totalPastDayOrMonth < CONST_NUMBER_CHART_DATA_SUMMARY_DISPLAY_DEFAULT)
				? totalPastDayOrMonth
				: size;
			return size;
		}

		/// <summary>
		/// Create summary for chart
		/// </summary>
		/// <param name="startIndex">Start index</param>
		/// <param name="length">Length</param>
		/// <param name="firstSourceList">First data source list</param>
		/// <param name="firstDestinationList">First data destination list</param>
		/// <param name="secondSourceList">Second data source list</param>
		/// <param name="secondDestinationList">Second data destination list</param>
		/// <param name="thirdSourceList">Third data source list</param>
		/// <param name="thirdDestinationList">Third data destination list</param>
		private void CreateSummaryForChart(
			int startIndex,
			int length,
			List<string> firstSourceList,
			string[] firstDestinationList,
			List<string> secondSourceList,
			string[] secondDestinationList,
			List<string> thirdSourceList = null,
			string[] thirdDestinationList = null)
		{
			firstSourceList.CopyTo(startIndex, firstDestinationList, 0, length);
			secondSourceList.CopyTo(startIndex, secondDestinationList, 0, length);

			if ((thirdSourceList == null) || (thirdDestinationList == null)) return;

			thirdSourceList.CopyTo(startIndex, thirdDestinationList, 0, length);
		}

		/// <summary>
		/// Get format date
		/// </summary>
		/// <param name="periodKbn">Period kbn</param>
		/// <returns>Format date</returns>
		private string GetFormatDate(string periodKbn)
		{
			var formatDate = (periodKbn == Constants.FLG_SUMMARYREPORT_PERIOD_KBN_THIS_YEAR)
				? CONST_FORMATDATE_SHORTDATE_ONLY_MONTH_AND_YEAR
				: CONST_FORMATDATE_SHORTDATE;
			return formatDate;
		}

		/// <summary>
		/// Get data kbn extend
		/// </summary>
		/// <returns>Data kbn extend</returns>
		private string GetDataKbnExtend(string dataKbn)
		{
			switch (dataKbn)
			{
				case Constants.FLG_SUMMARYREPORT_DATA_KBN_ORDER_COUNT:
					return Constants.FLG_SUMMARYREPORT_DATA_KBN_ORDER_AMOUNT;

				case Constants.FLG_SUMMARYREPORT_DATA_KBN_FIXED_PURCHASE_REGISTER:
					return Constants.FLG_SUMMARYREPORT_DATA_KBN_FIXED_PURCHASE_CANCEL;

				case Constants.FLG_SUMMARYREPORT_DATA_KBN_USER_REGISTER:
					return Constants.FLG_SUMMARYREPORT_DATA_KBN_USER_WITHDRAWAL;

				default:
					return dataKbn;
			}
		}

		/// <summary>
		/// Create chart data sets object for report
		/// </summary>
		/// <param name="type">Type</param>
		/// <param name="label">Label</param>
		/// <param name="color">Color</param>
		/// <param name="yAxisId">Y axis id</param>
		/// <param name="modals">Modals</param>
		/// <returns>Data sets object</returns>
		private DataSetsObject CreateChartDataSetsObjectForReport(
			string type,
			string label,
			string color,
			string yAxisId,
			string[] modals)
		{
			var result = new DataSetsObject
			{
				Type = type,
				Label = HtmlSanitizer.HtmlEncode(label),
				Data = modals,
				YAxisId = yAxisId,
				Color = color,
			};
			return result;
		}

		/// <summary>
		/// Create chart options object for report
		/// </summary>
		/// <param name="title">Title</param>
		/// <param name="type">Type</param>
		/// <returns>Options object</returns>
		private OptionsObject CreateChartOptionsObjectForReport(string title, string type)
		{
			var result = new OptionsObject
			{
				Title = HtmlSanitizer.HtmlEncode(title),
				Type = type
			};
			return result;
		}

		/// <summary>
		/// Create chart data modal object for report
		/// </summary>
		/// <param name="lables">Lables</param>
		/// <param name="dataSets">Data sets</param>
		/// <returns>Data object</returns>
		private DataModalObject CreateChartDataModalObjectForReport(
			string[] lables,
			DataSetsObject[] dataSets)
		{
			var result = new DataModalObject
			{
				Lables = lables,
				DataSets = dataSets,
			};
			return result;
		}

		/// <summary>
		/// Create chart data object for report
		/// </summary>
		/// <param name="options">Options</param>
		/// <param name="summary">Summary</param>
		/// <param name="detail">Detail</param>
		/// <returns>Chart object</returns>
		private ChartReport CreateChartObjectForReport(
			OptionsObject options,
			DataModalObject summary,
			DataModalObject detail)
		{
			var chartObject = new ChartReport
			{
				Options = options,
				Data = summary,
				DataModal = detail
			};
			return chartObject;
		}

		/// <summary>Period kbn</summary>
		private string PeriodKbn { get; set; }
		/// <summary>Options</summary>
		[JsonProperty("options")]
		public OptionsObject Options { get; set; }
		/// <summary>Data</summary>
		[JsonProperty("data")]
		public DataModalObject Data { get; set; }
		/// <summary>Data modal</summary>
		[JsonProperty("data_modal")]
		public DataModalObject DataModal { get; set; }
	}

	/// <summary>
	/// Options object
	/// </summary>
	[Serializable]
	public class OptionsObject
	{
		/// <summary>Title</summary>
		[JsonProperty("title")]
		public string Title { get; set; }
		/// <summary>Type</summary>
		[JsonProperty("type")]
		public string Type { get; set; }
	}

	/// <summary>
	/// Data modal object
	/// </summary>
	[Serializable]
	public class DataModalObject
	{
		/// <summary>Lables</summary>
		[JsonProperty("labels")]
		public string[] Lables { get; set; }
		/// <summary>Data sets</summary>
		[JsonProperty("datasets")]
		public DataSetsObject[] DataSets { get; set; }
	}

	/// <summary>
	/// Data sets object
	/// </summary>
	[Serializable]
	public class DataSetsObject
	{
		/// <summary>Type</summary>
		[JsonProperty("type")]
		public string Type { get; set; }
		/// <summary>Label</summary>
		[JsonProperty("label")]
		public string Label { get; set; }
		/// <summary>Data</summary>
		[JsonProperty("data")]
		public string[] Data { get; set; }
		/// <summary>Y axis id</summary>
		[JsonProperty("yAxisID")]
		public string YAxisId { get; set; }
		/// <summary>Color</summary>
		[JsonProperty("color")]
		public string Color { get; set; }
	}
}
