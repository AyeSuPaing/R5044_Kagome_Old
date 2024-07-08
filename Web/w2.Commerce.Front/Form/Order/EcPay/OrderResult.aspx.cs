/*
=========================================================================================================
  Module      : Order Result(OrderResult.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Web;
using w2.Domain.Order;

/// <summary>
/// Order Result Class
/// </summary>
public partial class EcPay_OrderResult : BasePage
{
	/// <summary>
	/// Page Load
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			var baseUrl = this.SecurePageProtocolAndHost + Constants.PATH_ROOT;
			if (baseUrl.ToLower().Contains(Constants.SITE_DOMAIN.ToLower()) == false)
			{
				baseUrl = string.Format(
					"{0}{1}{2}",
					Constants.PROTOCOL_HTTPS,
					Constants.SITE_DOMAIN,
					this.RawUrl);
				Response.Redirect(baseUrl);
			}

			var orderId = StringUtility.ToEmpty(Request[Constants.FIELD_ORDER_ORDER_ID]);
			var orderInfo = new OrderService().Get(orderId);

			// Goto Top Page if Order is null
			if (orderInfo == null) Response.Redirect(Constants.PATH_ROOT);

			if (orderInfo.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_TEMP)
			{
				divError.Visible = true;
				divButton.Visible = true;
			}
			else
			{
				SessionManager.NextPageForCheck = Constants.PAGE_FRONT_ORDER_COMPLETE;
				var url = new UrlCreator(baseUrl + Constants.PAGE_FRONT_ORDER_COMPLETE)
					.AddParam(Constants.FIELD_ORDER_ORDER_ID, orderId)
					.CreateUrl();
				Response.Redirect(url);
			}
		}
	}
}