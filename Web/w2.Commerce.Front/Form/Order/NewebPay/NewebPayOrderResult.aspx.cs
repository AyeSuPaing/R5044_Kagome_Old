/*
=========================================================================================================
  Module      : Order Result(NewebPayOrderResult.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using w2.App.Common.Order;
using w2.App.Common.Order.OrderCombine;
using w2.App.Common.Order.Payment.NewebPay;
using w2.Common.Web;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

/// <summary>
/// Order Result Class
/// </summary>
public partial class Form_Order_NewebPay_NewebPayOrderResult : ProductPage
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
				var status = StringUtility.ToEmpty(this.Request[NewebPayConstants.PARAM_STATUS]);
				var message = string.Empty;
				if (this.Request[NewebPayConstants.PARAM_TRADE_INFO] != null)
				{
					var response = NewebPayApiFacade.GetResponseFromRequest(this.Request);
					message = response.Message;
				}
				else
				{
					message = StringUtility.ToEmpty(this.Request[NewebPayConstants.PARAM_MESSAGE]);
				}
				var formattedUrl = string.Format(
					"{0}{1}{2}",
					Constants.PROTOCOL_HTTPS,
					Constants.SITE_DOMAIN,
					this.RawUrl);
				var newUrl = new UrlCreator(formattedUrl)
					.AddParam(NewebPayConstants.PARAM_STATUS, status)
					.AddParam(NewebPayConstants.PARAM_MESSAGE, message)
					.CreateUrl();
				baseUrl = (this.RawUrl.Contains(NewebPayConstants.PARAM_STATUS))
					? formattedUrl
					: newUrl;
				Response.Redirect(baseUrl);
			}

			var paramData = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_NO]).Split(',');

			switch (paramData[0])
			{
				case NewebPayConstants.CONST_CLIENT_BACK_URL:
					ExecuteRollBackOrder(baseUrl, paramData);
					break;

				case NewebPayConstants.CONST_RETURN_URL:
					ExecuteCompleteOrder(baseUrl);
					break;
			}
		}
	}

	/// <summary>
	/// Execute Roll Back Order
	/// </summary>
	/// <param name="baseUrl">Base Url</param>
	/// <param name="paramData">Array Of Param Data</param>
	private void ExecuteRollBackOrder(string baseUrl, string[] paramData)
	{
		var oldOrder = new OrderService().Get(paramData[1]);

		if (oldOrder.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_TEMP)
			Response.Redirect(Constants.PATH_ROOT);

		oldOrder.PaymentCartId = paramData[2];
		CartObject cart = null;
		var cartList = SessionManager.CartList;

		if (this.IsOrderCombined)
		{
			var combineCartList = SessionManager.OrderCombineCartList;
			combineCartList.Items[0].IsOrderDone = false;
			SessionManager.OrderCombineCartList = combineCartList;
			cart = combineCartList.Items[0];
		}
		else
		{
			var cartTemp = CartObject.CreateCartByOrder(oldOrder);
			cartList.AddCartVirtural(cartTemp);
			cart = cartList.Items[0];
		}

		cart.IsLandingUseNewebPay = bool.Parse(paramData[3].ToLower());

		if (cart.IsLandingUseNewebPay)
		{
			Session[NewebPayConstants.CONST_SESSION_IS_USE_NEWEBPAY] = cart.IsLandingUseNewebPay;
			Session[this.LadingCartSessionKey] = cartList;
		}

		var orderForRollback = OrderCommon.CreateOrderInfo(
			cart,
			paramData[1],
			cart.OrderUserId,
			"00pc",
			string.Empty,
			Request.ServerVariables["REMOTE_ADDR"],
			oldOrder.AdvcodeFirst,
			oldOrder.AdvcodeNew,
			oldOrder.LastChanged);

		var isUser = ((oldOrder.Owner.OwnerKbn == Constants.FLG_ORDEROWNER_OWNER_KBN_OFFLINE_USER)
			|| (oldOrder.Owner.OwnerKbn == Constants.FLG_ORDEROWNER_OWNER_KBN_SMARTPHONE_USER)
			|| (oldOrder.Owner.OwnerKbn == Constants.FLG_ORDEROWNER_OWNER_KBN_PC_USER));

		OrderCommon.RollbackPreOrder(
			orderForRollback,
			cart,
			false,
			oldOrder.Shippings[0].OrderShippingNo,
			isUser,
			UpdateHistoryAction.DoNotInsert);

		var nextPageUrl = cart.IsLandingUseNewebPay
			? Constants.PAGE_FRONT_LANDING_LANDING_CART_CONFIRM
			: Constants.PAGE_FRONT_ORDER_CONFIRM;

		SessionManager.NextPageForCheck = nextPageUrl;
		var url = new UrlCreator(baseUrl + nextPageUrl)
			.CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// Execute Complete Order
	/// </summary>
	/// <param name="baseUrl">Base Url</param>
	private void ExecuteCompleteOrder(string baseUrl)
	{
		var status = StringUtility.ToEmpty(this.Request[NewebPayConstants.PARAM_STATUS]);
		var paramData = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_NO]).Split(',');

		if ((paramData.Length > 1)
			&& (this.IsOrderCombined == false))
		{
			OrderCommon.DeleteCart((paramData.Length == 2) ? paramData[1] : paramData[2]);
		}
		else
		{
			var beforeCombineCartList = (CartObjectList)Session[Constants.SESSION_KEY_CART_LIST];
			beforeCombineCartList.Items
				.ForEach(beforeCombineCart => { OrderCommon.DeleteCart(beforeCombineCart.CartId); });

			var cartList = (CartObjectList)Session[Constants.SESSION_KEY_ORDERCOMBINE_CART_LIST];
			var cart = cartList.Items[0];

			CancelOrderCombined(cart);
		}

		SessionManager.CartList = null;
		SessionManager.OrderCombineCartList = null;
		SessionManager.OrderCombineBeforeCartList = null;

		// Go to Top Page If Status Is Null
		if (status == null) Response.Redirect(Constants.PATH_ROOT);

		if (status != NewebPayConstants.CONST_STATUS_SUCCESS)
		{
			divError.Visible = true;
			divButton.Visible = true;
			lError.Text = StringUtility.ToEmpty(this.Request[NewebPayConstants.PARAM_MESSAGE]);
		}
		else
		{
			SessionManager.NextPageForCheck = Constants.PAGE_FRONT_ORDER_COMPLETE;
			var url = new UrlCreator(baseUrl + Constants.PAGE_FRONT_ORDER_COMPLETE)
				.CreateUrl();
			Response.Redirect(url);
		}
	}

	/// <summary>
	/// Cancel Order Combined
	/// </summary>
	/// <param name="cart">Cart Object</param>
	private void CancelOrderCombined(CartObject cart)
	{
		var parentOrderId = cart.OrderCombineParentOrderId;
		using (var accessor = new SqlAccessor())
		{
			accessor.OpenConnection();
			accessor.BeginTransaction();

			var combinedNewOrderid = string.Join(",", cart.OrderId);
			var errMessage = OrderCombineUtility.OrderCancelForOrderCombine(
				this.ShopId,
				parentOrderId,
				this.LoginUserId,
				true,
				"user",
				UpdateHistoryAction.Insert,
				accessor,
				cart.Coupon);

			// 注文同梱元親注文の購入回数更新
			var orderCount = new UserService().Get(cart.OrderUserId, accessor).OrderCountOrderRealtime;
			var ht = new Hashtable
				{
					{ Constants.FIELD_ORDER_ORDER_ID, combinedNewOrderid },
					{ Constants.FIELD_ORDER_ORDER_COUNT_ORDER, (orderCount - 1) },
					{ Constants.FIELD_ORDER_LAST_CHANGED, "user" },
					{ Constants.FIELD_ORDER_USER_ID, cart.OrderUserId },
					{ Constants.FIELD_USER_ORDER_COUNT_ORDER_REALTIME, orderCount},
					{ "cancelCount", 1}
				};

			// 注文同梱元親注文の購入回数更新
			using (var statement = new SqlStatement("Order", "UpdateUserOrderCount"))
			{
				statement.ExecStatement(accessor, ht);
			}

			// ユーザーリアルタイム更新
			OrderCommon.UpdateRealTimeOrderCount(ht, Constants.FLG_REAL_TIME_ORDER_COUNT_ACTION_COMBINE, accessor);

			// 注文同梱元の親注文のキャンセルに失敗した場合、管理者へメール送信、ただしフロント画面が正常処理として処理を進める
			if (string.IsNullOrEmpty(errMessage) == false)
			{
				AppLogger.WriteError("フロント注文同梱の同梱元親注文のキャンセルに失敗しました。親注文ID：" + parentOrderId);
				OrderCombineUtility.SendOrderCombineParentOrderCancelErrorMail(
					cart.OrderUserId,
					parentOrderId,
					combinedNewOrderid,
					errMessage);
				accessor.RollbackTransaction();
			}
			accessor.CommitTransaction();
		}
	}

	/// <summary>ランディングカート保持用セッションキー</summary>
	public string LadingCartSessionKey
	{
		get
		{
			return string.Format(
				"{0}{1}{2}",
				Constants.SESSION_KEY_CART_LIST_LANDING,
				(string)this.Session["landing_cart_input_absolutePath"],
				this.LandingCartInputLastWriteTime);
		}
	}
	/// <summary>ランディング入力ページ最後に書き込み時刻</summary>
	public string LandingCartInputLastWriteTime
	{
		get { return (string)this.Session[Constants.SESSION_KEY_LANDING_CART_INPUT_LAST_WRITE_TIME]; }
		set { this.Session[Constants.SESSION_KEY_LANDING_CART_INPUT_LAST_WRITE_TIME] = value; }
	}
	/// <summary>Is Order Combined</summary>
	private bool IsOrderCombined { get { return (SessionManager.OrderCombineCartList != null); } }
}
