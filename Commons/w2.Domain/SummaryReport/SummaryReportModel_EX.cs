/*
=========================================================================================================
  Module      : Summary Report Model Extend (SummaryReportModel_EX.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.SummaryReport
{
	/// <summary>
	/// Summary Report Model (Extend)
	/// </summary>
	public partial class SummaryReportModel
	{
		/// <summary>Ranking report order by: count</summary>
		public const string RANKING_REPORT_ORDER_BY_COUNT = "0";
		/// <summary>Ranking report order by: amount</summary>
		public const string RANKING_REPORT_ORDER_BY_AMOUNT = "1";

		/// <summary>Order shipped status report: Unshipped</summary>
		public const string ORDERSHIPPEDSTATUS_REPORT_UNSHIPPED = "UNSHIPPED";
		/// <summary>Order shipped status report: Shipped today</summary>
		public const string ORDERSHIPPEDSTATUS_REPORT_TODAY = "SHIPPEDTODAY";
		/// <summary>Order shipped status report: Shipped future</summary>
		public const string ORDERSHIPPEDSTATUS_REPORT_FUTURE = "SHIPPEDFUTURE";

		/// <summary>Product stock report alert kbn: All</summary>
		public const string PRODUCTSTOCK_REPORT_ALERT_KBN_ALL = "0";
		/// <summary>Product stock report alert kbn: Safety stock alert</summary>
		public const string PRODUCTSTOCK_REPORT_ALERT_KBN_SAFETY_STOCK_ALERT = "1";
		/// <summary>Product stock report alert kbn: Out of stock</summary>
		public const string PRODUCTSTOCK_REPORT_ALERT_KBN_OUT_OF_STOCK = "2";

		/// <summary>TascScheduleHistory当月分データ</summary>
		public const string TASKSCHEDULEHISTORY_SCHEDULE_DATE_THIS_MONTH = "0";
		/// <summary>TascScheduleHistory先月分データ</summary>
		public const string TASKSCHEDULEHISTORY_SCHEDULE_DATE_LAST_MONTH = "1";
		/// <summary>MailClickLogデータ作成日</summary>
		public const string MAILCLICK_DATE_CREATED = "0";

		/// <summary>Report site type: PC</summary>
		public const string CONST_REPORT_SITE_KBN_PC = "PC";
		/// <summary>Report site type: SP</summary>
		public const string CONST_REPORT_SITE_KBN_SP = "SP";
		/// <summary>Report site type: Other</summary>
		public const string CONST_REPORT_SITE_KBN_OTHER = "Other";
		/// <summary>Report order type: Normal</summary>
		public const string CONST_REPORT_ORDER_KBN_NORMAL = "Normal";
		/// <summary>Report order type: Fixed purchase</summary>
		public const string CONST_REPORT_ORDER_KBN_FIXED_PURCHASE = "FixedPurchase";

		#region プロパティ
		#endregion
	}
}
