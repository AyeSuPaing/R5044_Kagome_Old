/*
=========================================================================================================
  Module      : Summary Report Service (SummaryReportService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using w2.Common.Sql;
using w2.Domain.AdvCode.Helper;
using w2.Domain.OrderWorkflowSetting;
using w2.Domain.Product.Helper;

namespace w2.Domain.SummaryReport
{
	/// <summary>
	/// Summary Report Service
	/// </summary>
	public class SummaryReportService : ServiceBase
	{
		#region +GetSummaryReportsByPeriodKbn
		/// <summary>
		/// Get summary reports by period kbn
		/// </summary>
		/// <param name="periodKbn">Period kbn</param>
		/// <param name="startDate">Start date</param>
		/// <param name="endDate">End date</param>
		/// <returns>Summary reports</returns>
		public SummaryReportModel[] GetSummaryReportsByPeriodKbn(
			string periodKbn,
			DateTime startDate,
			DateTime endDate)
		{
			using (var repository = new SummaryReportRepository())
			{
				var result = repository.GetSummaryReportsByPeriodKbn(
					periodKbn,
					startDate,
					endDate);
				return result;
			}
		}
		#endregion

		#region +InsertSummaryReport
		/// <summary>
		/// Insert summary report
		/// </summary>
		/// <param name="periodKbn">Period kbn</param>
		/// <param name="dataKbn">Data kbn</param>
		/// <param name="startDate">Start date</param>
		/// <param name="endDate">End date</param>
		/// <param name="accessor">SQL Accessor</param>
		public void InsertSummaryReport(
			string periodKbn,
			string dataKbn,
			DateTime startDate,
			DateTime endDate,
			SqlAccessor accessor)
		{
			using (var repository = new SummaryReportRepository(accessor))
			{
				repository.InsertSummaryReport(
					periodKbn,
					dataKbn,
					startDate,
					endDate);
			}
		}
		#endregion

		#region +GetProductStockForReport
		/// <summary>
		/// Get product stock for report
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <returns>Product stock reports</returns>
		public IDictionary<string, decimal> GetProductStockForReport(string shopId)
		{
			using (var repository = new SummaryReportRepository())
			{
				var result = repository.GetProductStockForReport(shopId);
				return result;
			}
		}
		#endregion

		#region +CountLatestUserRegisteredForReport
		/// <summary>
		/// Count latest user registered for report
		/// </summary>
		/// <param name="beginDate">Begin date</param>
		/// <param name="endDate">End date</param>
		/// <returns>Latest report of registered users</returns>
		public IDictionary<string, int> CountLatestUserRegisteredForReport(
			DateTime beginDate,
			DateTime endDate)
		{
			using (var repository = new SummaryReportRepository())
			{
				var result = repository.CountLatestUserRegisteredForReport(beginDate, endDate);
				return result;
			}
		}
		#endregion

		#region +CountLatestOrderAmountForReport
		/// <summary>
		/// Count latest order amount for report
		/// </summary>
		/// <param name="beginDate">Begin date</param>
		/// <param name="endDate">End date</param>
		/// <returns>Latest report of order amount</returns>
		public IDictionary<string, decimal> CountLatestOrderAmountForReport(
			DateTime beginDate,
			DateTime endDate)
		{
			using (var repository = new SummaryReportRepository())
			{
				var result = repository.CountLatestOrderAmountForReport(beginDate, endDate);
				return result;
			}
		}
		#endregion

		#region +CountLatestOrderForReport
		/// <summary>
		/// Count latest order for report
		/// </summary>
		/// <param name="beginDate">Begin date</param>
		/// <param name="endDate">End date</param>
		/// <returns>Latest report of order count</returns>
		public IDictionary<string, int> CountLatestOrderForReport(
			DateTime beginDate,
			DateTime endDate)
		{
			using (var repository = new SummaryReportRepository())
			{
				var result = repository.CountLatestOrderForReport(beginDate, endDate);
				return result;
			}
		}
		#endregion

		#region +CountOrderShippedStatusForDailyReport
		/// <summary>
		/// Count order shipped status for daily report
		/// </summary>
		/// <returns>Count order shipped status</returns>
		public IDictionary<string, int> CountOrderShippedStatusForDailyReport()
		{
			using (var repository = new SummaryReportRepository())
			{
				var result = repository.CountOrderShippedStatusForDailyReport();
				return result;
			}
		}
		#endregion

		#region +CountOrderStatusForMonthlyReport
		/// <summary>
		/// Count order status for monthly report
		/// </summary>
		/// <returns>Count order status</returns>
		public IDictionary<string, int> CountOrderStatusForMonthlyReport()
		{
			using (var repository = new SummaryReportRepository())
			{
				var result = repository.CountOrderStatusForMonthlyReport();
				return result;
			}
		}
		#endregion

		#region +GetTop10AdvCodeOrderRankingList
		/// <summary>
		/// Get top 10 adv code order ranking list
		/// </summary>
		/// <param name="shopId">Shop Id</param>
		/// <param name="orderByKbn">Order by kbn</param>
		/// <param name="beginDate">Begin date</param>
		/// <param name="endDate">End date</param>
		/// <returns>List of advcode models</returns>
		public AdvCodeListForReport[] GetTop10AdvCodeOrderRankingList(
			string shopId,
			string orderByKbn,
			DateTime beginDate,
			DateTime endDate)
		{
			using (var repository = new SummaryReportRepository())
			{
				var result = repository.GetTop10AdvCodeOrderRankingList(
					shopId,
					orderByKbn,
					beginDate,
					endDate);
				return result;
			}
		}
		#endregion

		#region +GetTop10ProductSalesRankingListForReport
		/// <summary>
		/// Get top 10 product sales ranking list for report
		/// </summary>
		/// <param name="shopId">Shop Id</param>
		/// <param name="orderByKbn">Order by kbn</param>
		/// <param name="beginDate">Begin date</param>
		/// <param name="endDate">End date</param>
		/// <returns>List of product models</returns>
		public ProductListForReport[] GetTop10ProductSalesRankingList(
			string shopId,
			string orderByKbn,
			DateTime beginDate,
			DateTime endDate)
		{
			using (var repository = new SummaryReportRepository())
			{
				var result = repository.GetTop10ProductSalesRankingList(
					shopId,
					orderByKbn,
					beginDate,
					endDate);
				return result;
			}
		}
		#endregion

		#region +GetTop10ProductVariationSalesRankingList
		/// <summary>
		/// Get top 10 product variation sales ranking list
		/// </summary>
		/// <param name="shopId">Shop Id</param>
		/// <param name="orderByKbn">Order by kbn</param>
		/// <param name="beginDate">Begin date</param>
		/// <param name="endDate">End date</param>
		/// <returns>List of product models</returns>
		public ProductListForReport[] GetTop10ProductVariationSalesRankingList(
			string shopId,
			string orderByKbn,
			DateTime beginDate,
			DateTime endDate)
		{
			using (var repository = new SummaryReportRepository())
			{
				var result = repository.GetTop10ProductVariationSalesRankingList(
					shopId,
					orderByKbn,
					beginDate,
					endDate);
				return result;
			}
		}
		#endregion

		#region +GetOrderWorkflowSettingAllListForReport
		/// <summary>
		/// Get order workflow setting all list for report
		/// </summary>
		/// <returns>Order workflow setting list</returns>
		public OrderWorkflowSettingModel[] GetOrderWorkflowSettingAllListForReport()
		{
			using (var repository = new SummaryReportRepository())
			{
				var result = repository.GetOrderWorkflowSettingAllListForReport();
				return result;
			}
		}
		#endregion

		#region GetTaskScheduleHistory
		/// <summary>
		/// メール配信件数取得
		/// </summary>
		/// <param name="thisMonth">今月の開始日</param>
		/// <param name="dataKbn">データ区分</param>
		/// <param name="periodKbn">期間</param>
		/// <returns>該当期間のメール件数</returns>
		public int GetTaskScheduleHistory(DateTime thisMonth, string dataKbn, string periodKbn)
		{
			using (var repository = new SummaryReportRepository())
			{
				var result = repository.GetTaskScheduleHistory(thisMonth, dataKbn, periodKbn);
				return result;
			}
		}
		#endregion

		#region GetMailClickCount
		/// <summary>
		/// メールクリック件数取得
		/// </summary>
		/// <param name="thisMonth">今月の開始日</param>
		/// <param name="dataKbn">データ区分</param>
		/// <param name="periodKbn">期間</param>
		/// <returns>該当期間のメールクリック件数</returns>
		public int GetMailClickCount(DateTime thisMonth, string dataKbn, string periodKbn)
		{
			using (var repository = new SummaryReportRepository())
			{
				var result = repository.GetMailClickCount(thisMonth, dataKbn, periodKbn);
				return result;
			}
		}
		#endregion
	}
}
