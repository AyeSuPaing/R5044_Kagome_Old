/*
=========================================================================================================
  Module      : Order Status Report (OrderStatusReport.cs)
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
	/// Order status report
	/// </summary>
	[Serializable]
	public class OrderStatusReport
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public OrderStatusReport()
		{
			InitializeProperties();
		}

		/// <summary>
		/// Create report
		/// </summary>
		/// <returns>Report</returns>
		public OrderStatusReport CreateReport()
		{
			var reports = new SummaryReportService().CountOrderStatusForMonthlyReport();
			SetReports(reports);
			return this;
		}

		/// <summary>
		/// Initialize properties
		/// </summary>
		private void InitializeProperties()
		{
			this.OrderStatusTempCount = 0;
			this.OrderStatusOrderedCount = 0;
			this.OrderStatusRecognizedCount = 0;
			this.OrderStatusStockReservedCount = 0;
			this.OrderStatusShipArrangedCount = 0;
			this.OrderStatusShipCompleteCount = 0;
		}

		/// <summary>
		/// Set reports
		/// </summary>
		/// <param name="reports">Reports</param>
		private void SetReports(IDictionary<string, int> reports = null)
		{
			if (reports == null) return;

			this.OrderStatusTempCount = reports.ContainsKey(Constants.FLG_ORDER_ORDER_STATUS_TEMP)
				? reports[Constants.FLG_ORDER_ORDER_STATUS_TEMP]
				: 0;
			this.OrderStatusOrderedCount = reports.ContainsKey(Constants.FLG_ORDER_ORDER_STATUS_ORDERED)
				? reports[Constants.FLG_ORDER_ORDER_STATUS_ORDERED]
				: 0;
			this.OrderStatusRecognizedCount = reports.ContainsKey(Constants.FLG_ORDER_ORDER_STATUS_ORDER_RECOGNIZED)
				? reports[Constants.FLG_ORDER_ORDER_STATUS_ORDER_RECOGNIZED]
				: 0;
			this.OrderStatusStockReservedCount = reports.ContainsKey(Constants.FLG_ORDER_ORDER_STATUS_STOCK_RESERVED)
				? reports[Constants.FLG_ORDER_ORDER_STATUS_STOCK_RESERVED]
				: 0;
			this.OrderStatusShipArrangedCount = reports.ContainsKey(Constants.FLG_ORDER_ORDER_STATUS_SHIP_ARRANGED)
				? reports[Constants.FLG_ORDER_ORDER_STATUS_SHIP_ARRANGED]
				: 0;
			this.OrderStatusShipCompleteCount = reports.ContainsKey(Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP)
				? reports[Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP]
				: 0;
		}

		/// <summary>Order status temp count</summary>
		public int OrderStatusTempCount { get; set; }
		/// <summary>Order status ordered count</summary>
		public int OrderStatusOrderedCount { get; set; }
		/// <summary>Order status recognized count</summary>
		public int OrderStatusRecognizedCount { get; set; }
		/// <summary>Order status stock reserved count</summary>
		public int OrderStatusStockReservedCount { get; set; }
		/// <summary>Order status ship arranged count</summary>
		public int OrderStatusShipArrangedCount { get; set; }
		/// <summary>Order status ship complete count</summary>
		public int OrderStatusShipCompleteCount { get; set; }
		/// <summary>Has order status temp count</summary>
		public bool HasOrderStatusTempCount
		{
			get { return (this.OrderStatusTempCount > 0); }
		}
	}
}