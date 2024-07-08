/*
=========================================================================================================
  Module      : Summary Information Page(SummaryInformationPage.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Web;
using w2.Domain.SummaryReport;

/// <summary>
/// Summary Information Page
/// </summary>
public partial class SummaryInformationPage : OrderPage
{
	/// <summary>Default page no</summary>
	private const string CONST_DEFAULT_PAGE_NO = "1";

	/// <summary>
	/// Create order list url for search order shipped status
	/// </summary>
	/// <param name="orderShippedStatus">Order shipped status</param>
	/// <returns>Order list url</returns>
	public string CreateOrderListUrlForSearchOrderShippedStatus(string orderShippedStatus)
	{
		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDERWORKFLOW_LIST)
			.AddParam(
				Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_KBN,
				Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_KBN_SYSTEM)
			.AddParam(
				Constants.REQUEST_KEY_WORKFLOW_TYPE,
				Constants.FLG_ORDERWORKFLOWSETTING_TARGET_WORKFLOW_TYPE_ORDER)
			.AddParam(
				Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_NO,
				Constants.CONST_SUMMARY_REPORT_UNSHIPPED_WORKFLOW_NO);
		var currentDay = DateTime.Today;
		switch (orderShippedStatus)
		{
			case SummaryReportModel.ORDERSHIPPEDSTATUS_REPORT_UNSHIPPED:
				var yesterday = currentDay.AddDays(-1);
				urlCreator.AddParam(
					Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SCHEDULED_SHIPPINGDATE_TO,
					string.Format(
						"{0}/{1}/{2}",
						yesterday.Year.ToString(),
						yesterday.Month.ToString().PadLeft(2, '0'),
						yesterday.Day.ToString().PadLeft(2, '0')));
				break;

			case SummaryReportModel.ORDERSHIPPEDSTATUS_REPORT_TODAY:
				urlCreator.AddParam(
					Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SCHEDULED_SHIPPINGDATE_FROM,
					string.Format(
						"{0}/{1}/{2}",
						currentDay.Year.ToString(),
						currentDay.Month.ToString().PadLeft(2, '0'),
						currentDay.Day.ToString().PadLeft(2, '0'))).AddParam(
					Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SCHEDULED_SHIPPINGDATE_TO,
					string.Format(
						"{0}/{1}/{2}",
						currentDay.Year.ToString(),
						currentDay.Month.ToString().PadLeft(2, '0'),
						currentDay.Day.ToString().PadLeft(2, '0')));
				break;

			case SummaryReportModel.ORDERSHIPPEDSTATUS_REPORT_FUTURE:
				var futureDay = DateTime.Today.AddDays(1);
				urlCreator.AddParam(
					Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SCHEDULED_SHIPPINGDATE_FROM,
					string.Format(
						"{0}/{1}/{2}",
						futureDay.Year.ToString(),
						futureDay.Month.ToString().PadLeft(2, '0'),
						futureDay.Day.ToString().PadLeft(2, '0')));
				break;
		}

		urlCreator.AddParam(Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_SCHEDULED_SHIPPINGDATE, "FALSE");
		urlCreator.AddParam(Constants.REQUEST_KEY_PRODUCT_DISPLAY_KBN, Constants.FLG_FIXEDPURCHASEWORKFLOWSETTING_DISPLAY_KBN_LINE);
		urlCreator.AddParam(Constants.REQUEST_KEY_PAGE_NO, CONST_DEFAULT_PAGE_NO);
		urlCreator.AddParam(Constants.REQUEST_KEY_WINDOW_KBN, Constants.KBN_WINDOW_POPUP);
		return urlCreator.CreateUrl();
	}

	/// <summary>
	/// Create order list url for search order status
	/// </summary>
	/// <param name="status">Status</param>
	/// <returns>Order list url</returns>
	public string CreateOrderListUrlForSearchOrderStatus(string status)
	{
		var lastDateOfCurrentMonth = DateTimeUtility.GetLastDateOfCurrentMonth();
		var firstDateOfCurrentMonth = DateTimeUtility.GetFirstDateOfCurrentMonth();
		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDER_LIST)
			.AddParam(Constants.REQUEST_KEY_ORDER_ORDER_STATUS, status);

		switch (status)
		{
			case Constants.FLG_ORDER_ORDER_STATUS_TEMP:
			case Constants.FLG_ORDER_ORDER_STATUS_ORDERED:
				urlCreator.AddParam(
					Constants.REQUEST_KEY_ORDER_UPDATE_DATE_STATUS,
					Constants.FIELD_ORDER_ORDER_DATE);
				break;

			case Constants.FLG_ORDER_ORDER_STATUS_ORDER_RECOGNIZED:
				urlCreator.AddParam(
					Constants.REQUEST_KEY_ORDER_UPDATE_DATE_STATUS,
					Constants.FIELD_ORDER_ORDER_RECOGNITION_DATE);
				break;

			case Constants.FLG_ORDER_ORDER_STATUS_STOCK_RESERVED:
				urlCreator.AddParam(
					Constants.REQUEST_KEY_ORDER_UPDATE_DATE_STATUS,
					Constants.FIELD_ORDER_ORDER_STOCKRESERVED_DATE);
				break;

			case Constants.FLG_ORDER_ORDER_STATUS_SHIP_ARRANGED:
				urlCreator.AddParam(
					Constants.REQUEST_KEY_ORDER_UPDATE_DATE_STATUS,
					Constants.FIELD_ORDER_ORDER_SHIPPING_DATE);
				break;

			case Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP:
				urlCreator.AddParam(
						Constants.REQUEST_KEY_ORDER_UPDATE_DATE_STATUS,
						Constants.FIELD_ORDER_ORDER_SHIPPED_DATE)
					.AddParam(
						Constants.REQUEST_KEY_ORDER_UPDATE_DATE_FROM,
						string.Format("{0}/{1}/{2}",
							firstDateOfCurrentMonth.Year.ToString(),
							firstDateOfCurrentMonth.Month.ToString().PadLeft(2, '0'),
							firstDateOfCurrentMonth.Day.ToString().PadLeft(2, '0')))
					.AddParam(
						Constants.REQUEST_KEY_ORDER_UPDATE_TIME_FROM,
						"00:00:00")
					.AddParam(
						Constants.REQUEST_KEY_ORDER_UPDATE_DATE_TO,
						string.Format("{0}/{1}/{2}",
							lastDateOfCurrentMonth.Year.ToString(),
							lastDateOfCurrentMonth.Month.ToString().PadLeft(2, '0'),
							lastDateOfCurrentMonth.Day.ToString().PadLeft(2, '0')))
					.AddParam(
						Constants.REQUEST_KEY_ORDER_UPDATE_TIME_TO,
						"23:59:59");
				break;
		}

		urlCreator.AddParam(Constants.REQUEST_KEY_PAGE_NO, CONST_DEFAULT_PAGE_NO);
		urlCreator.AddParam(Constants.REQUEST_KEY_WINDOW_KBN, Constants.KBN_WINDOW_POPUP);
		return urlCreator.CreateUrl(); ;
	}

	/// <summary>
	/// Create workflow list url
	/// </summary>
	/// <param name="workflowType">Workflow type</param>
	/// <param name="workflowNo">Workflow no</param>
	/// <param name="workflowKbn">Workflow Kbn</param>
	/// <returns>Workflow list url</returns>
	protected static string CreateWorkflowListUrl(
		string workflowType,
		string workflowNo,
		string workflowKbn)
	{
		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDERWORKFLOW_LIST)
			.AddParam(Constants.REQUEST_KEY_WORKFLOW_TYPE, workflowType)
			.AddParam(Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_KBN, workflowKbn)
			.AddParam(Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_NO, workflowNo)
			.AddParam(Constants.REQUEST_KEY_DISPLAY_KBN, Constants.KBN_ORDER_DISPLAY_LIST)
			.AddParam(Constants.REQUEST_KEY_PAGE_NO, CONST_DEFAULT_PAGE_NO)
			.AddParam(Constants.REQUEST_KEY_WINDOW_KBN, Constants.KBN_WINDOW_POPUP);
		return urlCreator.CreateUrl();
	}

	/// <summary>
	/// Create Product Stock Detail Url
	/// </summary>
	/// <param name="alertKbn">Alert Kbn</param>
	/// <returns>Product stock detail url</returns>
	protected string CreateProductStockDetailUrl(string alertKbn)
	{
		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTSTOCK_LIST)
			.AddParam(Constants.REQUEST_KEY_STOCK_ALERT_KBN, alertKbn)
			.AddParam(Constants.REQUEST_KEY_WINDOW_KBN, Constants.KBN_WINDOW_POPUP);
		return urlCreator.CreateUrl();
	}

	/// <summary>
	/// Create Incident Detail Url
	/// </summary>
	/// <param name="status">Status</param>
	/// <returns>Incident detail url</returns>
	protected string CreateIncidentDetailUrl(string status)
	{
		var urlCreator = new UrlCreator(Constants.PATH_ROOT_CS + Constants.PAGE_W2CS_MANAGER_TOP_PAGE)
			.AddParam(Constants.REQUEST_KEY_CS_TASKSTATUS_MODE, status)
			.AddParam(Constants.REQUEST_KEY_WINDOW_KBN, Constants.KBN_WINDOW_POPUP);
		return urlCreator.CreateUrl();
	}

	/// <summary>
	/// Create adv code detail url
	/// </summary>
	/// <param name="advCode">Adv code</param>
	/// <returns>Adv code detail url</returns>
	protected string CreateAdvCodeDetailUrl(string advCode)
	{
		var urlCreator = new UrlCreator(Constants.PATH_ROOT_MP + Constants.PAGE_W2MP_MANAGER_ADVERTISMENT_CODE_REGISTER)
			.AddParam(Constants.REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE, advCode)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_UPDATE)
			.AddParam(Constants.REQUEST_KEY_WINDOW_KBN, Constants.KBN_WINDOW_POPUP);
		return urlCreator.CreateUrl();
	}
}