/*
=========================================================================================================
  Module      : Product Stock Report (ProductStockReport.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using w2.Domain.SummaryReport;

namespace w2.App.Common.SummaryInformation
{
	/// <summary>
	/// Product stock report
	/// </summary>
	[Serializable]
	public class ProductStockReport
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="shopId">Shop id</param>
		public ProductStockReport(string shopId)
		{
			this.ShopId = shopId;
			InitializeProperties();
		}

		/// <summary>
		/// Create report
		/// </summary>
		/// <returns>Report</returns>
		public ProductStockReport CreateReport()
		{
			var reports = new SummaryReportService().GetProductStockForReport(this.ShopId);
			SetReports(reports);
			return this;
		}

		/// <summary>
		/// Initialize properties
		/// </summary>
		private void InitializeProperties()
		{
			this.StockPriceTotal = 0;
			this.StockOutOfStockCount = 0;
			this.StockSafetyAlertCount = 0;
		}

		/// <summary>
		/// Set reports
		/// </summary>
		/// <param name="reports">Reports</param>
		private void SetReports(IDictionary<string, decimal> reports)
		{
			if (reports == null) return;

			this.StockPriceTotal = reports.ContainsKey(SummaryReportModel.PRODUCTSTOCK_REPORT_ALERT_KBN_ALL)
				? reports[SummaryReportModel.PRODUCTSTOCK_REPORT_ALERT_KBN_ALL]
				: 0m;
			this.StockSafetyAlertCount = reports.ContainsKey(SummaryReportModel.PRODUCTSTOCK_REPORT_ALERT_KBN_SAFETY_STOCK_ALERT)
				? (int)reports[SummaryReportModel.PRODUCTSTOCK_REPORT_ALERT_KBN_SAFETY_STOCK_ALERT]
				: 0;
			this.StockOutOfStockCount = reports.ContainsKey(SummaryReportModel.PRODUCTSTOCK_REPORT_ALERT_KBN_OUT_OF_STOCK)
				? (int)reports[SummaryReportModel.PRODUCTSTOCK_REPORT_ALERT_KBN_OUT_OF_STOCK]
				: 0;
		}

		/// <summary>Shop id</summary>
		private string ShopId { get; set; }
		/// <summary>Stock safety alert count</summary>
		public int StockSafetyAlertCount { get; set; }
		/// <summary>Stock out of stock count</summary>
		public int StockOutOfStockCount { get; set; }
		/// <summary>Stock price total</summary>
		public decimal StockPriceTotal { get; set; }
		/// <summary>Has stock safety alert count</summary>
		public bool HasStockSafetyAlertCount
		{
			get { return (this.StockSafetyAlertCount > 0); }
		}
	}
}
