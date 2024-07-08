/*
=========================================================================================================
  Module      : Latest Report (LatestReport.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using w2.Common.Util;
using w2.Domain.SummaryReport;

namespace w2.App.Common.SummaryInformation
{
	/// <summary>
	/// The latest report object
	/// </summary>
	[Serializable]
	public class LatestReport
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="periodKbn">Period kbn</param>
		/// <param name="beginDate">Begin Date</param>
		/// <param name="endDate">End Date</param>
		public LatestReport(string periodKbn, DateTime beginDate, DateTime endDate)
		{
			InitializeProperties();

			this.PeriodKbn = periodKbn;
			this.BeginDate = beginDate;
			this.EndDate = endDate;

			CreateReports();
		}

		/// <summary>
		/// Initialize properties
		/// </summary>
		private void InitializeProperties()
		{
			this.RegisteredUser = new RegisteredUserReport();
			this.OrderAmount = new OrderAmountReport();
			this.OrderCount = new OrderCountReport();
		}

		/// <summary>
		/// Create reports
		/// </summary>
		private void CreateReports()
		{
			if (string.IsNullOrEmpty(this.PeriodKbn)
				|| (this.BeginDate == null)
				|| (this.EndDate == null)) return;

			this.RegisteredUser = CreateRegisteredUserReport();
			this.OrderAmount = CreateOrderAmountReport();
			this.OrderCount = CreateOrderCountReport();
		}

		/// <summary>
		/// Create registered user report
		/// </summary>
		/// <returns>The registered user object</returns>
		private RegisteredUserReport CreateRegisteredUserReport()
		{
			if (this.PeriodKbn == Constants.FLG_SUMMARYREPORT_PERIOD_KBN_TODAY)
			{
				return new RegisteredUserReport();
			}

			var registeredUserReport = new SummaryReportService().CountLatestUserRegisteredForReport(
				this.BeginDate,
				this.EndDate);
			var result = new RegisteredUserReport(registeredUserReport);
			return result;
		}

		/// <summary>
		/// Create order amount report
		/// </summary>
		/// <returns>The order amount report object</returns>
		private OrderAmountReport CreateOrderAmountReport()
		{
			var orderAmountReport = new SummaryReportService().CountLatestOrderAmountForReport(
				this.BeginDate,
				this.EndDate);
			var result = new OrderAmountReport(orderAmountReport);
			return result;
		}

		/// <summary>
		/// Create order count report
		/// </summary>
		/// <returns>The order count report object</returns>
		private OrderCountReport CreateOrderCountReport()
		{
			var orderCountReport = new SummaryReportService().CountLatestOrderForReport(
				this.BeginDate,
				this.EndDate);
			var result = new OrderCountReport(orderCountReport);
			return result;
		}

		/// <summary>Period kbn</summary>
		private string PeriodKbn { get; set; }
		/// <summary>Begin date</summary>
		private DateTime BeginDate { get; set; }
		/// <summary>End date</summary>
		private DateTime EndDate { get; set; }
		/// <summary>The registered user object</summary>
		[JsonProperty("registeredUser")]
		public RegisteredUserReport RegisteredUser { get; set; }
		/// <summary>The order amount object</summary>
		[JsonProperty("orderAmount")]
		public OrderAmountReport OrderAmount { get; set; }
		/// <summary>The order count object</summary>
		[JsonProperty("orderCount")]
		public OrderCountReport OrderCount { get; set; }
	}

	/// <summary>
	/// The registered user report
	/// </summary>
	[Serializable]
	public class RegisteredUserReport
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="reports">Reports</param>
		public RegisteredUserReport(IDictionary<string, int> reports = null)
		{
			InitializeProperties();
			SetReports(reports);
		}

		/// <summary>
		/// Initialize properties
		/// </summary>
		private void InitializeProperties()
		{
			this.PcCount = 0;
			this.SpCount = 0;
		}

		/// <summary>
		/// Set reports
		/// </summary>
		/// <param name="reports">Reports</param>
		private void SetReports(IDictionary<string, int> reports)
		{
			if (reports == null) return;

			this.PcCount = reports.ContainsKey(SummaryReportModel.CONST_REPORT_SITE_KBN_PC)
				? reports[SummaryReportModel.CONST_REPORT_SITE_KBN_PC]
				: 0;
			this.SpCount = reports.ContainsKey(SummaryReportModel.CONST_REPORT_SITE_KBN_SP)
				? reports[SummaryReportModel.CONST_REPORT_SITE_KBN_SP]
				: 0;
		}

		/// <summary>Total registered user</summary>
		[JsonProperty("totalCount")]
		public int TotalCount
		{
			get
			{
				return (this.PcCount + this.SpCount);
			}
		}
		/// <summary>Display PC registered user</summary>
		[JsonProperty("pcCount")]
		public string DispPcCount
		{
			get { return StringUtility.ToNumeric(this.PcCount); }
		}
		/// <summary>PC registered user</summary>
		public int PcCount { get; set; }
		/// <summary>Display SP registered user</summary>
		[JsonProperty("spCount")]
		public string DispSpCount
		{
			get { return StringUtility.ToNumeric(this.SpCount); }
		}
		/// <summary>SP registered user</summary>
		public int SpCount { get; set; }
		/// <summary>The percent of PC registered user</summary>
		[JsonProperty("pcRate")]
		public string PcRate
		{
			get
			{
				return SummaryInformationUtility.GetPercentFromNumber(
					(decimal)this.PcCount,
					(decimal)this.TotalCount);
			}
		}
		/// <summary>The percent of SP registered user</summary>
		[JsonProperty("spRate")]
		public string SpRate
		{
			get
			{
				return SummaryInformationUtility.GetPercentFromNumber(
					(decimal)this.SpCount,
					(decimal)this.TotalCount);
			}
		}
	}

	/// <summary>
	/// The order amount object
	/// </summary>
	[Serializable]
	public class OrderAmountReport
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="reports">Reports</param>
		public OrderAmountReport(IDictionary<string, decimal> reports = null)
		{
			InitializeProperties();
			SetReports(reports);
		}

		/// <summary>
		/// Initialize properties
		/// </summary>
		private void InitializeProperties()
		{
			this.PcAmount = 0m;
			this.SpAmount = 0m;
			this.OtherAmount = 0m;
		}

		/// <summary>
		/// Set reports
		/// </summary>
		/// <param name="reports">Reports</param>
		private void SetReports(IDictionary<string, decimal> reports)
		{
			if (reports == null) return;

			this.PcAmount = reports.ContainsKey(SummaryReportModel.CONST_REPORT_SITE_KBN_PC)
				? reports[SummaryReportModel.CONST_REPORT_SITE_KBN_PC]
				: 0m;
			this.SpAmount = reports.ContainsKey(SummaryReportModel.CONST_REPORT_SITE_KBN_SP)
				? reports[SummaryReportModel.CONST_REPORT_SITE_KBN_SP]
				: 0m;
			this.OtherAmount = reports.ContainsKey(SummaryReportModel.CONST_REPORT_SITE_KBN_OTHER)
				? reports[SummaryReportModel.CONST_REPORT_SITE_KBN_OTHER]
				: 0m;
		}

		/// <summary>Total order amount</summary>
		[JsonProperty("totalAmount")]
		public decimal TotalAmount
		{
			get
			{
				return (this.PcAmount + this.SpAmount + this.OtherAmount);
			}
		}
		/// <summary>Display PC order amount</summary>
		[JsonProperty("pcAmount")]
		public string DispPcAmount
		{
			get { return StringUtility.ToNumeric(this.PcAmount.ToString("0")); }
		}
		/// <summary>PC order amount</summary>
		public decimal PcAmount { get; set; }
		/// <summary>Display SP order amount</summary>
		[JsonProperty("spAmount")]
		public string DispSpAmount
		{
			get { return StringUtility.ToNumeric(this.SpAmount.ToString("0")); }
		}
		/// <summary>SP order amount</summary>
		public decimal SpAmount { get; set; }
		/// <summary>Display other order amount</summary>
		[JsonProperty("otherAmount")]
		public string DispOtherAmount
		{
			get { return StringUtility.ToNumeric(this.OtherAmount.ToString("0")); }
		}
		/// <summary>Other order amount</summary>
		public decimal OtherAmount { get; set; }
		/// <summary>The percent of PC order amount</summary>
		[JsonProperty("pcRate")]
		public string PcRate
		{
			get
			{
				return SummaryInformationUtility.GetPercentFromNumber(
					this.PcAmount,
					this.TotalAmount);
			}
		}
		/// <summary>The percent of SP order amount</summary>
		[JsonProperty("spRate")]
		public string SpRate
		{
			get
			{
				return SummaryInformationUtility.GetPercentFromNumber(
					this.SpAmount,
					this.TotalAmount);
			}
		}
		/// <summary>The percent of other order amount</summary>
		[JsonProperty("otherRate")]
		public string OtherRate
		{
			get
			{
				return SummaryInformationUtility.GetPercentFromNumber(
					this.OtherAmount,
					this.TotalAmount);
			}
		}
	}

	/// <summary>
	/// The order count object
	/// </summary>
	[Serializable]
	public class OrderCountReport
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="reports">Reports</param>
		public OrderCountReport(IDictionary<string, int> reports = null)
		{
			InitializeProperties();
			SetReports(reports);
		}

		/// <summary>
		/// Initialize properties
		/// </summary>
		private void InitializeProperties()
		{
			this.PcCount = 0;
			this.SpCount = 0;
			this.OtherCount = 0;
			this.NormalCount = 0;
			this.FixedPurchaseCount = 0;
		}

		/// <summary>
		/// Set reports
		/// </summary>
		/// <param name="reports">Reports</param>
		private void SetReports(IDictionary<string, int> reports)
		{
			if (reports == null) return;

			this.PcCount = reports.ContainsKey(SummaryReportModel.CONST_REPORT_SITE_KBN_PC)
				? reports[SummaryReportModel.CONST_REPORT_SITE_KBN_PC]
				: 0;
			this.SpCount = reports.ContainsKey(SummaryReportModel.CONST_REPORT_SITE_KBN_SP)
				? reports[SummaryReportModel.CONST_REPORT_SITE_KBN_SP]
				: 0;
			this.OtherCount = reports.ContainsKey(SummaryReportModel.CONST_REPORT_SITE_KBN_OTHER)
				? reports[SummaryReportModel.CONST_REPORT_SITE_KBN_OTHER]
				: 0;
			this.NormalCount = reports.ContainsKey(SummaryReportModel.CONST_REPORT_ORDER_KBN_NORMAL)
				? reports[SummaryReportModel.CONST_REPORT_ORDER_KBN_NORMAL]
				: 0;
			this.FixedPurchaseCount = (Constants.FIXEDPURCHASE_OPTION_ENABLED
					&& reports.ContainsKey(SummaryReportModel.CONST_REPORT_ORDER_KBN_FIXED_PURCHASE))
				? reports[SummaryReportModel.CONST_REPORT_ORDER_KBN_FIXED_PURCHASE]
				: 0;
		}

		/// <summary>Total order count</summary>
		[JsonProperty("totalCount")]
		public int TotalCount
		{
			get
			{
				return (this.PcCount + this.SpCount + this.OtherCount);
			}
		}
		/// <summary>Display PC order count</summary>
		[JsonProperty("pcCount")]
		public string DispPcCount
		{
			get { return StringUtility.ToNumeric(this.PcCount); }
		}
		/// <summary>PC order count</summary>
		public int PcCount { get; set; }
		/// <summary>Display SP order count</summary>
		[JsonProperty("spCount")]
		public string DispSpCount
		{
			get { return StringUtility.ToNumeric(this.SpCount); }
		}
		/// <summary>SP order count</summary>
		public int SpCount { get; set; }
		/// <summary>Display other order count</summary>
		[JsonProperty("otherCount")]
		public string DispOtherCount
		{
			get { return StringUtility.ToNumeric(this.OtherCount); }
		}
		/// <summary>Other order count</summary>
		public int OtherCount { get; set; }
		/// <summary>Display normal order count</summary>
		[JsonProperty("normalCount")]
		public string DispNormalCount
		{
			get { return StringUtility.ToNumeric(this.NormalCount); }
		}
		/// <summary>Normal order count</summary>
		public int NormalCount { get; set; }
		/// <summary>Display fixed purchase order count</summary>
		[JsonProperty("fixedPurchaseCount")]
		public string DispFixedPurchaseCount
		{
			get { return StringUtility.ToNumeric(this.FixedPurchaseCount); }
		}
		/// <summary>Fixed purchase order count</summary>
		public int FixedPurchaseCount { get; set; }
		/// <summary>The percent of PC order count</summary>
		[JsonProperty("pcRate")]
		public string PcRate
		{
			get
			{
				return SummaryInformationUtility.GetPercentFromNumber(
					(decimal)this.PcCount,
					(decimal)this.TotalCount);
			}
		}
		/// <summary>The percent of SP order count</summary>
		[JsonProperty("spRate")]
		public string SpRate
		{
			get
			{
				return SummaryInformationUtility.GetPercentFromNumber(
					(decimal)this.SpCount,
					(decimal)this.TotalCount);
			}
		}
		/// <summary>The percent of other order count</summary>
		[JsonProperty("otherRate")]
		public string OtherRate
		{
			get
			{
				return SummaryInformationUtility.GetPercentFromNumber(
					(decimal)this.OtherCount,
					(decimal)this.TotalCount);
			}
		}
		/// <summary>The percent of normal order count</summary>
		[JsonProperty("normalRate")]
		public string NormalRate
		{
			get
			{
				return SummaryInformationUtility.GetPercentFromNumber(
					(decimal)this.NormalCount,
					(decimal)this.TotalCount);
			}
		}
		/// <summary>The percent of fixed purchase order count</summary>
		[JsonProperty("fixedPurchaseRate")]
		public string FixedPurchaseRate
		{
			get
			{
				return SummaryInformationUtility.GetPercentFromNumber(
					(decimal)this.FixedPurchaseCount,
					(decimal)this.TotalCount);
			}
		}
	}
}
