/*
=========================================================================================================
  Module      : Summary Report Repository (SummaryReportRepository.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;
using w2.Domain.AdvCode.Helper;
using w2.Domain.OrderWorkflowSetting;
using w2.Domain.Product.Helper;

namespace w2.Domain.SummaryReport
{
	/// <summary>
	/// Summary Report Repository
	/// </summary>
	internal class SummaryReportRepository : RepositoryBase
	{
		/// <summary>Xml key name</summary>
		private const string XML_KEY_NAME = "SummaryReport";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public SummaryReportRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">SQL Accessor</param>
		public SummaryReportRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +GetSummaryReportsByPeriodKbn
		/// <summary>
		/// Get summary reports by period kbn
		/// </summary>
		/// <param name="periodKbn">Period kbn</param>
		/// <param name="startDate">Start date</param>
		/// <param name="endDate">End date</param>
		/// <returns>Summary Reports</returns>
		internal SummaryReportModel[] GetSummaryReportsByPeriodKbn(
			string periodKbn,
			DateTime startDate,
			DateTime endDate)
		{
			var dv = Get(
				XML_KEY_NAME,
				"GetSummaryReportsByPeriodKbn",
				new Hashtable
				{
					{ Constants.FIELD_SUMMARYREPORT_PERIOD_KBN, periodKbn },
					{ "start_date", startDate },
					{ "end_date", endDate }
				});
			var summaryReports = dv.Cast<DataRowView>()
				.Select(drv => new SummaryReportModel(drv))
				.ToArray();
			return summaryReports;
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
		internal void InsertSummaryReport(
			string periodKbn,
			string dataKbn,
			DateTime startDate,
			DateTime endDate)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_SUMMARYREPORT_PERIOD_KBN, periodKbn },
				{ Constants.FIELD_SUMMARYREPORT_DATA_KBN, dataKbn },
				{ "start_date", startDate },
				{ "end_date", endDate }
			};
			var statementName = GetStatementNameForInsertSummaryReport(periodKbn, dataKbn);
			Exec(XML_KEY_NAME, statementName, input);
		}

		/// <summary>
		/// Get statement name for insert summary report
		/// </summary>
		/// <param name="periodKbn">Period kbn</param>
		/// <param name="dataKbn">Data kbn</param>
		/// <returns>Statement name</returns>
		private string GetStatementNameForInsertSummaryReport(string periodKbn, string dataKbn)
		{
			var isCurrentYear = (periodKbn == Constants.FLG_SUMMARYREPORT_PERIOD_KBN_THIS_YEAR);
			switch (dataKbn)
			{
				case Constants.FLG_SUMMARYREPORT_DATA_KBN_USER_ACCESS:
					return (isCurrentYear
						? "InsertUserAccessSummaryReportByMonth"
						: "InsertUserAccessSummaryReportByDay");

				case Constants.FLG_SUMMARYREPORT_DATA_KBN_ORDER_COUNT:
					return (isCurrentYear
						? "InsertOrderCountSummaryReportByMonth"
						: "InsertOrderCountSummaryReportByDay");

				case Constants.FLG_SUMMARYREPORT_DATA_KBN_ORDER_AMOUNT:
					return (isCurrentYear
						? "InsertOrderAmountSummaryReportByMonth"
						: "InsertOrderAmountSummaryReportByDay");

				case Constants.FLG_SUMMARYREPORT_DATA_KBN_CONVERSION:
					return (isCurrentYear
						? "InsertConversionSummaryReportByMonth"
						: "InsertConversionSummaryReportByDay");

				case Constants.FLG_SUMMARYREPORT_DATA_KBN_LTV:
					return (isCurrentYear
						? "InsertLtvSummaryReportByMonth"
						: "InsertLtvSummaryReportByDay");

				case Constants.FLG_SUMMARYREPORT_DATA_KBN_FIXED_PURCHASE_REGISTER:
					return (isCurrentYear
						? "InsertFixedPurchaseRegisterSummaryReportByMonth"
						: "InserFixedPurchaseRegisterSummaryReportByDay");

				case Constants.FLG_SUMMARYREPORT_DATA_KBN_FIXED_PURCHASE_CANCEL:
					return (isCurrentYear
						? "InsertFixedPurchaseCancelSummaryReportByMonth"
						: "InserFixedPurchaseCancelSummaryReportByDay");

				case Constants.FLG_SUMMARYREPORT_DATA_KBN_USER_REGISTER:
					return (isCurrentYear
						? "InsertUserRegisterSummaryReportByMonth"
						: "InsertUserRegisterSummaryReportByDay");

				case Constants.FLG_SUMMARYREPORT_DATA_KBN_USER_WITHDRAWAL:
					return (isCurrentYear
						? "InsertUserWithdrawalSummaryReportByMonth"
						: "InsertUserWithdrawalSummaryReportByDay");

				case Constants.FLG_SUMMARYREPORT_DATA_KBN_MEMBERSHIP_COUNT:
					return (isCurrentYear
						? "InsertMembershipCountSummaryReportByMonth"
						: "InsertMembershipCountSummaryReportByDay");

				case Constants.FLG_SUMMARYREPORT_DATA_KBN_FIXED_PURCHASE_COUNT:
					return (isCurrentYear
						? "InsertFixedPurchaseCountSummaryReportByMonth"
						: "InsertFixedPurchaseCountSummaryReportByDay");

				case Constants.FLG_SUMMARYREPORT_DATA_KBN_SENT_MAIL_COUNT:
					return (isCurrentYear
						? string.Empty
						: "InsertSentMailCountOneMonth");

				case Constants.FLG_SUMMARYREPORT_DATA_KBN_MAIL_CLICK_COUNT:
					return (isCurrentYear
						? string.Empty
						: "InsertMailClickCountOneMonth");

				default:
					return string.Empty;
			}
		}
		#endregion

		#region +GetProductStockForReport
		/// <summary>
		/// Get product stock for report
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <returns>Product stock reports</returns>
		internal IDictionary<string, decimal> GetProductStockForReport(string shopId)
		{
			var dv = Get(
				XML_KEY_NAME,
				"GetProductStockForReport",
				new Hashtable
				{
					{ Constants.FIELD_PRODUCTSTOCK_SHOP_ID, shopId }
				});
			var result = dv.Table.AsEnumerable()
				.ToDictionary<DataRow, string, decimal>(
					row => row.Field<string>(0),
					row => row.Field<decimal>(1));
			return result;
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
			var input = new Hashtable
			{
				{ "begin_date", beginDate },
				{ "end_date", endDate },
			};
			var dv = Get(XML_KEY_NAME, "CountLatestUserRegisteredForReport", input);
			var result = dv.Table.AsEnumerable()
				.ToDictionary<DataRow, string, int>(
					row => row.Field<string>(0),
					row => row.Field<int>(1));
			return result;
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
			var input = new Hashtable
			{
				{ "begin_date", beginDate },
				{ "end_date", endDate },
			};
			var dv = Get(XML_KEY_NAME, "CountLatestOrderAmountForReport", input);
			var result = dv.Table.AsEnumerable()
				.ToDictionary<DataRow, string, decimal>(
					row => row.Field<string>(0),
					row => row.Field<decimal>(1));
			return result;
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
			var input = new Hashtable
			{
				{ "begin_date", beginDate },
				{ "end_date", endDate },
			};
			var dv = Get(XML_KEY_NAME, "CountLatestOrderForReport", input);
			var result = dv.Table.AsEnumerable()
				.ToDictionary<DataRow, string, int>(
					row => row.Field<string>(0),
					row => row.Field<int>(1));
			return result;
		}
		#endregion

		#region +CountOrderShippedStatusForDailyReport
		/// <summary>
		/// Count order shipped status for daily report
		/// </summary>
		/// <returns>Count order shipped status</returns>
		internal IDictionary<string, int> CountOrderShippedStatusForDailyReport()
		{
			var dv = Get(XML_KEY_NAME, "CountOrderShippedStatusForDailyReport");
			var result = dv.Table.AsEnumerable()
				.ToDictionary<DataRow, string, int>(
					row => row.Field<string>(0),
					row => row.Field<int>(1));
			return result;
		}
		#endregion

		#region +CountOrderStatusForMonthlyReport
		/// <summary>
		/// Count order status monthly report
		/// </summary>
		/// <returns>Count order status</returns>
		internal IDictionary<string, int> CountOrderStatusForMonthlyReport()
		{
			var dv = Get(XML_KEY_NAME, "CountOrderStatusForMonthlyReport");
			var result = dv.Table.AsEnumerable()
				.ToDictionary<DataRow, string, int>(
					row => row.Field<string>(0),
					row => row.Field<int>(1));
			return result;
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
			var input = new Hashtable
			{
				{ Constants.FIELD_ORDER_SHOP_ID, shopId },
				{ (orderByKbn == SummaryReportModel.RANKING_REPORT_ORDER_BY_COUNT)
					? "orderby_number"
					: "orderby_amount", orderByKbn },
				{ "begin_date", beginDate },
				{ "end_date", endDate },
			};
			var dv = Get(
				XML_KEY_NAME,
				"GetTop10AdvCodeOrderRankingList",
				input);
			var results = dv.Cast<DataRowView>()
				.Select(drv => new AdvCodeListForReport(drv))
				.ToArray();
			return results;
		}
		#endregion

		#region ~GetTop10ProductSalesRankingList
		/// <summary>
		/// Get top 10 product sales ranking list
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
			var input = new Hashtable
			{
				{ Constants.FIELD_PRODUCT_SHOP_ID, shopId },
				{ (orderByKbn == SummaryReportModel.RANKING_REPORT_ORDER_BY_COUNT)
					? "orderby_number"
					: "orderby_amount", orderByKbn },
				{ "begin_date", beginDate },
				{ "end_date", endDate },
			};
			var dv = Get(
				XML_KEY_NAME,
				"GetTop10ProductSalesRankingList",
				input);
			var results = dv.Cast<DataRowView>()
				.Select(drv => new ProductListForReport(drv))
				.ToArray();
			return results;
		}
		#endregion

		#region ~GetTop10ProductVariationSalesRankingList
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
			var input = new Hashtable
			{
				{ Constants.FIELD_PRODUCT_SHOP_ID, shopId },
				{ (orderByKbn == SummaryReportModel.RANKING_REPORT_ORDER_BY_COUNT)
					? "orderby_number"
					: "orderby_amount", orderByKbn },
				{ "begin_date", beginDate },
				{ "end_date", endDate },
			};
			var dv = Get(
				XML_KEY_NAME,
				"GetTop10ProductVariationSalesRankingList",
				input);
			var results = dv.Cast<DataRowView>()
				.Select(drv => new ProductListForReport(drv))
				.ToArray();
			return results;
		}
		#endregion

		#region +GetOrderWorkflowSettingAllListForReport
		/// <summary>
		/// Get order workflow setting all list for report
		/// </summary>
		/// <returns>Order workflow setting list</returns>
		internal OrderWorkflowSettingModel[] GetOrderWorkflowSettingAllListForReport()
		{
			var dv = Get(
				XML_KEY_NAME,
				"GetOrderWorkflowSettingAllListForReport");
			var results = dv.Cast<DataRowView>()
				.Select(drv => new OrderWorkflowSettingModel(drv))
				.ToArray();
			return results;
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
		internal int GetTaskScheduleHistory(DateTime thisMonth, string dataKbn, string periodKbn)
		{
			var dv = Get(
				XML_KEY_NAME,
				"GetSummaryReport",
				new Hashtable
				{
					{ "first_date", thisMonth },
					{ "last_date", thisMonth.AddMonths(1).AddMilliseconds(-1) },
					{ Constants.FIELD_SUMMARYREPORT_DATA_KBN, dataKbn },
					{ Constants.FIELD_SUMMARYREPORT_PERIOD_KBN, periodKbn }
				}
			);
			
			var result = dv[0][0].ToString();
			return int.Parse(string.IsNullOrEmpty(result) ? "0" : result.Split('.')[0]);
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
		internal int GetMailClickCount(DateTime thisMonth, string dataKbn, string periodKbn)
		{
			var dv = Get(
				XML_KEY_NAME,
				"GetSummaryReport",
				new Hashtable
				{
					{ "first_date", thisMonth },
					{ "last_date", thisMonth.AddMonths(1).AddMilliseconds(-1) },
					{ Constants.FIELD_SUMMARYREPORT_DATA_KBN, dataKbn},
					{ Constants.FIELD_SUMMARYREPORT_PERIOD_KBN, periodKbn }
				}
			);

			var result = dv[0][0].ToString();
			return int.Parse(string.IsNullOrEmpty(result) ? "0" : result.Split('.')[0]);
		}
		#endregion
	}
}
