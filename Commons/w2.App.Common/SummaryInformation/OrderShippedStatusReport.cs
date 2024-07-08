/*
=========================================================================================================
  Module      : Order Shipped Status Report (OrderShippedStatusReport.cs)
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
	/// Order shipped status report
	/// </summary>
	[Serializable]
	public class OrderShippedStatusReport
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public OrderShippedStatusReport()
		{
			InitializeProperties();
		}

		/// <summary>
		/// Create report
		/// </summary>
		/// <returns>Report</returns>
		public OrderShippedStatusReport CreateReport()
		{
			var reports = new SummaryReportService().CountOrderShippedStatusForDailyReport();
			SetReports(reports);
			return this;
		}

		/// <summary>
		/// Initialize properties
		/// </summary>
		private void InitializeProperties()
		{
			this.UnshippedCount = 0;
			this.ShippedTodayCount = 0;
			this.ShippedFutureCount = 0;
		}

		/// <summary>
		/// Set reports
		/// </summary>
		/// <param name="reports">Reports</param>
		private void SetReports(IDictionary<string, int> reports)
		{
			if (reports == null) return;

			this.UnshippedCount = reports.ContainsKey(SummaryReportModel.ORDERSHIPPEDSTATUS_REPORT_UNSHIPPED)
				? reports[SummaryReportModel.ORDERSHIPPEDSTATUS_REPORT_UNSHIPPED]
				: 0;
			this.ShippedTodayCount = reports.ContainsKey(SummaryReportModel.ORDERSHIPPEDSTATUS_REPORT_TODAY)
				? reports[SummaryReportModel.ORDERSHIPPEDSTATUS_REPORT_TODAY]
				: 0;
			this.ShippedFutureCount = reports.ContainsKey(SummaryReportModel.ORDERSHIPPEDSTATUS_REPORT_FUTURE)
				? reports[SummaryReportModel.ORDERSHIPPEDSTATUS_REPORT_FUTURE]
				: 0;
		}

		/// <summary>Display unshipped count</summary>
		[JsonProperty("unshippedCount")]
		public string DispUnshippedCount
		{
			get { return StringUtility.ToNumeric(this.UnshippedCount); }
		}
		/// <summary>Unshipped count</summary>
		public int UnshippedCount { get; set; }
		/// <summary>Display shipped today count</summary>
		[JsonProperty("shippedTodayCount")]
		public string DispShippedTodayCount
		{
			get { return StringUtility.ToNumeric(this.ShippedTodayCount); }
		}
		/// <summary>Shipped today count</summary>
		public int ShippedTodayCount { get; set; }
		/// <summary>Display shipped future count</summary>
		[JsonProperty("shippedFutureCount")]
		public string DispShippedFutureCount
		{
			get { return StringUtility.ToNumeric(this.ShippedFutureCount); }
		}
		/// <summary>Shipped future count</summary>
		public int ShippedFutureCount { get; set; }
		/// <summary>Has unshipped count</summary>
		[JsonProperty("hasUnshippedCount")]
		public bool HasUnshippedCount
		{
			get { return (this.UnshippedCount > 0); }
		}
	}
}
