/*
=========================================================================================================
  Module      : Sale Goal Report (SaleGoalReport.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using w2.App.Common.Util;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain.SaleGoal;

namespace w2.App.Common.SummaryInformation
{
	/// <summary>
	/// Sale goal report
	/// </summary>
	[Serializable]
	public class SaleGoalReport
	{
		/// <summary>Currency symbol text</summary>
		private const string CONST_SUMMARYREPORT_CURRENCY_SYMBOL_TEXT = "currency_symbol_text";
		/// <summary>Currency symbol text: Jp</summary>
		private const string CONST_SUMMARYREPORT_CURRENCY_SYMBOL_TEXT_JP = "JP";

		/// <summary>Sale goal display type</summary>
		private const string CONST_SUMMARYREPORT_SALE_GOAL_DISP_KBN = "sale_goal_disp_kbn";
		/// <summary>Sale goal display type: Monthly</summary>
		private const string CONST_SUMMARYREPORT_SALE_GOAL_DISP_KBN_MONTHLY = "MONTHLY";
		/// <summary>Sale goal display type: AnnuaL</summary>
		private const string CONST_SUMMARYREPORT_SALE_GOAL_DISP_KBN_ANNUAL = "ANNUAL";

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="reportDate">Report date</param>
		public SaleGoalReport(DateTime reportDate)
		{
			this.ReportDate = reportDate;
			this.Title = string.Empty;
			this.Current = new CurrentObject();
			this.Goal = new GoalObject();
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="title">Title</param>
		/// <param name="current">Current object</param>
		/// <param name="gold">Gold object</param>
		public SaleGoalReport(string title, CurrentObject current, GoalObject gold)
		{
			this.Title = title;
			this.Goal = gold;
			this.Current = current;
		}

		/// <summary>
		/// Create current reports
		/// </summary>
		/// <returns>Reports</returns>
		public List<SaleGoalReport> CreateReports()
		{
			var saleGoalService = new SaleGoalService();
			var currentSaleGoal = saleGoalService.Get(this.ReportDate.Year);
			if (currentSaleGoal == null) return new List<SaleGoalReport>();

			// Monthly sale goal
			var startMonthDate = DateTimeUtility.GetFirstDateOfCurrentMonth();
			var currentMonthlySalesRevenue = saleGoalService.GetSalesRevenue(startMonthDate, this.ReportDate);

			// Annual sale goal
			var applicableDate = new DateTime(this.ReportDate.Year, currentSaleGoal.ApplicableMonth, 1);
			var annualStartMonth = DateTime.Compare(startMonthDate, applicableDate) < 0 ? applicableDate.AddYears(-1) : applicableDate;
			var currentAnnualSalesRevenue = saleGoalService.GetSalesRevenue(
				annualStartMonth,
				DateTimeUtility.GetEndTimeOfDay(annualStartMonth.AddYears(1).AddDays(-1))
			);

			// Create current sale goal list
			var saleGoalList = new List<SaleGoalReport>
			{
				CreateMonthlySaleGoal(
					currentMonthlySalesRevenue,
					currentSaleGoal.GetMonthlySaleGoal(this.ReportDate.Month)),
				CreateAnnualSaleGoal(
					currentAnnualSalesRevenue,
					currentSaleGoal.AnnualGoal)
			};
			return saleGoalList;
		}

		/// <summary>
		/// Create default reports (For the case that there is no record in this year's database)
		/// </summary>
		/// <returns>Default reports</returns>
		public List<SaleGoalReport> CreateDefaultReports()
		{
			// Create default sale goal list
			var saleGoalList = new List<SaleGoalReport>
			{
				CreateMonthlySaleGoal(
					0,
					new SaleGoalModel().GetMonthlySaleGoal(this.ReportDate.Month)),
				CreateAnnualSaleGoal(0, 0)
			};
			return saleGoalList;
		}

		/// <summary>
		/// Create current object
		/// </summary>
		/// <param name="par">The par value</param>
		/// <param name="value">The value</param>
		/// <returns>A current object</returns>
		private CurrentObject CreateCurrentObject(int par, decimal value)
		{
			var current = new CurrentObject
			{
				Par = par,
				Value = StringUtility.ToNumeric(value.ToString("0")),
				Unit = GetUnitHtmlEncodedForSaleGoalReport(),
				Outlook = par
			};
			return current;
		}

		/// <summary>
		/// Get title html encoded for sale goal report
		/// </summary>
		/// <param name="isMonthly">Is monthly</param>
		/// <returns>Title</returns>
		private string GetTitleHtmlEncodedForSaleGoalReport(bool isMonthly = false)
		{
			var result = HtmlSanitizer.HtmlEncode(
				ValueText.GetValueText(
					Constants.TABLE_SUMMARYREPORT,
					CONST_SUMMARYREPORT_SALE_GOAL_DISP_KBN,
					isMonthly
						? CONST_SUMMARYREPORT_SALE_GOAL_DISP_KBN_MONTHLY
						: CONST_SUMMARYREPORT_SALE_GOAL_DISP_KBN_ANNUAL));
			return result;
		}

		/// <summary>
		/// Create monthly sale goal
		/// </summary>
		/// <param name="monthlySalesRevenue">Monthly sales revenue</param>
		/// <param name="monthlySaleGoal">Monthly sale goal</param>
		/// <returns>Sale goal object</returns>
		private SaleGoalReport CreateMonthlySaleGoal(
			decimal monthlySalesRevenue,
			decimal monthlySaleGoal)
		{
			var saleGoal = CreateSaleGoalObject(
				GetTitleHtmlEncodedForSaleGoalReport(true),
				monthlySalesRevenue,
				monthlySaleGoal);
			return saleGoal;
		}

		/// <summary>
		/// Create annual sale goal
		/// </summary>
		/// <param name="annualSalesRevenue">Annual sales revenue</param>
		/// <param name="annualGoal">Annual goal</param>
		/// <returns>Sale goal object</returns>
		private SaleGoalReport CreateAnnualSaleGoal(
			decimal annualSalesRevenue,
			decimal annualGoal)
		{
			var saleGoal = CreateSaleGoalObject(
				GetTitleHtmlEncodedForSaleGoalReport(),
				annualSalesRevenue,
				annualGoal);
			return saleGoal;
		}

		/// <summary>
		/// Create sale goal object
		/// </summary>
		/// <param name="title">The title</param>
		/// <param name="currentObjectValue">The current object value</param>
		/// <param name="goalObjectValue">The goal object value</param>
		/// <returns>A sale goal object</returns>
		private SaleGoalReport CreateSaleGoalObject(
			string title,
			decimal currentObjectValue,
			decimal goalObjectValue)
		{
			var par = SummaryInformationUtility.CalculatePercent(currentObjectValue, goalObjectValue);
			var current = CreateCurrentObject((int)par, currentObjectValue);
			var goal = CreateGoalObject(goalObjectValue);
			var saleGoal = new SaleGoalReport(title, current, goal);
			return saleGoal;
		}

		/// <summary>
		/// Create goal object
		/// </summary>
		/// <param name="value">The value</param>
		/// <returns>A goal object</returns>
		private GoalObject CreateGoalObject(decimal value)
		{
			var goal = new GoalObject
			{
				Value = StringUtility.ToNumeric(value.ToString("0")),
				Unit = GetUnitHtmlEncodedForSaleGoalReport(),
			};
			return goal;
		}

		/// <summary>
		/// Get unit html encoded for sale goal report
		/// </summary>
		/// <returns>Unit</returns>
		private string GetUnitHtmlEncodedForSaleGoalReport()
		{
			var result = HtmlSanitizer.HtmlEncode(
				ValueText.GetValueText(
					Constants.TABLE_SUMMARYREPORT,
					CONST_SUMMARYREPORT_CURRENCY_SYMBOL_TEXT,
					CONST_SUMMARYREPORT_CURRENCY_SYMBOL_TEXT_JP));
			return result;
		}

		/// <summary>Report date</summary>
		private DateTime ReportDate { get; set; }
		/// <summary>Title</summary>
		[JsonProperty("title")]
		public string Title { get; set; }
		/// <summary>Current</summary>
		[JsonProperty("current")]
		public CurrentObject Current { get; set; }
		/// <summary>Goal</summary>
		[JsonProperty("goal")]
		public GoalObject Goal { get; set; }
	}

	/// <summary>
	/// Current object
	/// </summary>
	[Serializable]
	public class CurrentObject
	{
		public CurrentObject()
		{
			this.Par = 0;
			this.Value = string.Empty;
			this.Unit = string.Empty;
			this.Outlook = 0;
		}

		/// <summary>Par</summary>
		[JsonProperty("par")]
		public int Par { get; set; }
		/// <summary>Value</summary>
		[JsonProperty("value")]
		public string Value { get; set; }
		/// <summary>Unit</summary>
		[JsonProperty("unit")]
		public string Unit { get; set; }
		/// <summary>Outlook</summary>
		[JsonProperty("outlook")]
		public int Outlook { get; set; }
	}

	/// <summary>
	/// Goal object
	/// </summary>
	[Serializable]
	public class GoalObject
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public GoalObject()
		{
			this.Value = string.Empty;
			this.Unit = string.Empty;
		}

		/// <summary>Value</summary>
		[JsonProperty("value")]
		public string Value { get; set; }
		/// <summary>Unit</summary>
		[JsonProperty("unit")]
		public string Unit { get; set; }
	}
}
