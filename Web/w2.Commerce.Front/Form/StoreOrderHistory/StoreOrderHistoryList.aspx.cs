/*
=========================================================================================================
  Module      : Store Order History List (StoreOrderHistoryList.aspx.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.App.Common.CrossPoint.OrderHistory;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Web;

public partial class Form_StoreOrderHistory_StoreOrderHistoryList : BasePage
{
	/// <summary>Wrapped repeater store order history list</summary>
	WrappedRepeater WrStoreOrderHistoryList { get { return GetWrappedControl<WrappedRepeater>("rStoreOrderHistoryList"); } }
	/// <summary>My page menu display</summary>
	public override bool DispMyPageMenu { get { return true; } }

	/// <summary>
	/// Page load
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			if (Constants.CROSS_POINT_OPTION_ENABLED == false)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_404_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}

			Display();
		}
	}

	/// <summary>
	/// Display
	/// </summary>
	private void Display()
	{
		// Get store order history data
		var orderHistoryList = new CrossPointOrderHistoryApiService()
			.GetList(new OrderHistoryApiInput(this.LoginUserId)
				.GetParam(OrderHistoryApiInput.RequestType.List));

		// Page error
		Session[Constants.SESSION_KEY_ERROR_MSG] = Session[w2.App.Common.Constants.SESSION_KEY_CROSSPOINT_ERROR_MESSAGE];
		if (string.IsNullOrEmpty(Session[Constants.SESSION_KEY_ERROR_MSG].ToString()) == false)
		{
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		// When the store order history has no item
		if ((orderHistoryList == null)
			|| (orderHistoryList.Any(item => (item.ShopName != Constants.CROSS_POINT_EC_SHOP_NAME)) == false))
		{
			this.ErrorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_STOREORDERHISTORY_NO_ITEM);

			return;
		}

		orderHistoryList = orderHistoryList.Where(item => (item.ShopName != Constants.CROSS_POINT_EC_SHOP_NAME)).ToArray();
		var totalCounts = orderHistoryList.Count();

		// Page setting
		var pageNo = 1;
		if (string.IsNullOrEmpty(Request[Constants.REQUEST_KEY_PAGE_NO]) == false)
		{
			int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out pageNo);
		}

		// Bind Store Order History Data
		this.WrStoreOrderHistoryList.DataSource = orderHistoryList
			.Skip((pageNo - 1) * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST)
			.Take(Constants.CONST_DISP_CONTENTS_DEFAULT_LIST);
		this.WrStoreOrderHistoryList.DataBind();

		this.PagerHtml = WebPager.CreateDefaultListPager(
			totalCounts,
			pageNo,
			Constants.PATH_ROOT + Constants.PAGE_FRONT_STORE_ORDER_HISTORY_LIST);
	}

	/// <summary>
	/// Create URL to detail
	/// </summary>
	/// <param name="slipNo">Slip no</param>
	/// <returns>URL</returns>
	public string CreateUrlToDetail(string slipNo)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_STORE_ORDER_HISTORY_DETAIL)
			.AddParam(w2.App.Common.Constants.CROSS_POINT_PARAM_POINT_SLIP_NO, slipNo)
			.CreateUrl();

		return url;
	}

	/// <summary>Pager HTML</summary>
	protected string PagerHtml
	{
		get { return (string)ViewState["PagerHtml"]; }
		private set { ViewState["PagerHtml"] = value; }
	}
	/// <summary>Error message</summary>
	protected string ErrorMessage
	{
		get { return (string)ViewState["ErrorMessage"]; }
		private set { ViewState["ErrorMessage"] = value; }
	}
}