/*
=========================================================================================================
  Module      : Line Pay Receive (LinePayReceive.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.LinePay;
using w2.Common.Logger;
using w2.Common.Web;
using w2.Domain.Order;
using w2.Domain.Order.Helper;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

/// <summary>
/// Line pay receive
/// </summary>
public partial class Payment_LinePay_LinePayReceive : OrderCartPageExternalPayment
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		var orderId = Request[Constants.REQUEST_KEY_ORDER_ID];

		switch (Request["action"])
		{
			case LinePayUtility.API_CALLBACK_REQUEST_FOR_ORDER:
				RequestForOrder(orderId);
				break;

			case LinePayUtility.API_CALLBACK_REQUEST_FOR_MODIFY:
				RequestForModify(orderId);
				break;

			case LinePayUtility.API_CALLBACK_CONFIRM_FOR_ORDER:
				ConfirmForOrder(orderId);
				break;

			case LinePayUtility.API_CALLBACK_CANCEL_ORDER:
				ExecCancelAction(orderId);
				break;
		}
	}

	/// <summary>
	/// 注文向けRequest（Requestを送信して,LINE決済画面へ遷移）
	/// </summary>
	/// <param name="orderId">orderId</param>
	/// <returns>Payment Url</returns>
	public void RequestForOrder(string orderId)
	{
		var landingCartSessionKey = (string)Session[Constants.SESSION_KEY_LANDING_CART_SESSION_KEY];
		var isLanding = ((string.IsNullOrEmpty(landingCartSessionKey) == false) && (SessionManager.CartListLp == null));

		// セッション情報をDB保存
		SessionSecurityManager.SaveSesstionContetnsToDatabaseForGoToOtherSite(Session, orderId);

		var cartList = GetCartList(isLanding);
		if (cartList == null)
		{
			RedirectToNextPage(null, WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_AUTH_ERROR));
		}

		var cart = cartList.Items.FirstOrDefault(c => c.OrderId == orderId);
		var cartIndex = cartList.Items.IndexOf(cart);

		using (var productBundler = new ProductBundler(
			cartList.Items,
			cartList.UserId,
			SessionManager.AdvCodeFirst,
			SessionManager.AdvCodeNow,
			null,
			null,
			true))
		{
			cart = productBundler.CartList[cartIndex];

			// カート内容と注文内容の金額を比較
			var orderModel = new OrderService().Get(orderId);
			if (cart.PriceTotal != orderModel.OrderPriceTotal)
			{
				RedirectToNextPage(null, WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CART_CHANGED), Constants.PAGE_FRONT_CART_LIST);
			}

			// LINE PAY リクエスト
			var returnUrl = (Constants.PROTOCOL_HTTPS + Constants.SITE_DOMAIN + Constants.PATH_ROOT_FRONT_PC + Constants.PAGE_FRONT_PAYMENT_LINEPAY_RECEIVE);
			var request = LinePayUtility.CreateRequestPayment(
				cart,
				new RedirectUrls
				{
					ConfirmUrl = new UrlCreator(returnUrl).AddParam("action", LinePayUtility.API_CALLBACK_CONFIRM_FOR_ORDER).AddParam(Constants.REQUEST_KEY_ORDER_ID, orderId).CreateUrl(),
					CancelUrl = new UrlCreator(returnUrl).AddParam("action", LinePayUtility.API_CALLBACK_CANCEL_ORDER).AddParam(Constants.REQUEST_KEY_ORDER_ID, orderId).CreateUrl(),
					ConfirmUrlType = "CLIENT",
				});
			var paymentOrderId = OrderCommon.CreatePaymentOrderId(cart.ShopId);

			// 自動決済キーを取得
			var regKey = GetRegKey(orderId, paymentOrderId);

			// 自動決済キーが存在しない場合CheckOut画面に飛ばす
			var response = (string.IsNullOrEmpty(regKey))
				? LinePayApiFacade.RequestPayment(
					request,
					new LinePayApiFacade.LinePayLogInfo(cart.OrderId, request.PaymentOrderId, ""))
				: LinePayApiFacade.PreapprovedPayment(
					regKey,
					string.Join(",", cart.Items.Select(product => product.ProductJointName)),
					cart.SettlementAmount,
					cart.SettlementCurrency,
					paymentOrderId,
					Constants.PAYMENT_LINEPAY_PAYMENTCAPTURENOW,
					new LinePayApiFacade.LinePayLogInfo(orderId, paymentOrderId, ""));

			// Handle API response
			if (response.IsSuccess == false)
			{
				RedirectToNextPage(null, LogCreator.CreateErrorMessage(response.ReturnCode, response.ReturnMessage));
			}

			// LINE PAY画面表示
			if (string.IsNullOrEmpty(regKey))
			{
				SessionManager.IsRedirectFromLinePay = true;
				Response.Redirect(response.Info.PaymentUrl.WebUrl);
			}
			else
			{
				ExecOrderComplete(orderId, paymentOrderId, response.Info.TransactionId, UpdateHistoryAction.Insert);
				var param = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
				this.LinePayOrders = (List<KeyValuePair<string, Hashtable>>)param["order_linepay"];
				var order = this.LinePayOrders.Find(o => o.Key == orderId).Value;
				var successFlag = false;
				var register = new OrderRegisterFront(this.IsLoggedIn);
				try
				{
					successFlag = RestoreSession(orderId);
					if (successFlag)
					{
						// プロパティセット
						GetOrderPropertiesFromSession();
						register.GoogleAnalyticsParams = (List<Hashtable>)param["googleanaytics_params"];
						this.DispErrorMessages = (List<string>)param["error"];
					}
				}
				// 例外時
				catch (Exception ex)
				{
					// ログ出力
					AppLogger.WriteError(ex);
					throw new Exception("セッション情報復元エラー");
				}

				if (successFlag)
				{
					// 注文完了後処理
					register.AfterOrderCompleteProcesses(order, cart, UpdateHistoryAction.DoNotInsert);
					// 注文完了画面用に注文IDを格納
					this.SuccessOrders.Add(order);
				}

				if (order != null) this.LinePayOrders.RemoveAll(o => o.Key == orderId);

				// 次の画面へ
				RedirectToNextPage(order);
			}
		}
	}

	/// <summary>
	/// 自動決済キー(regKey)を取得
	/// </summary>
	/// <param name="orderId">注文ID</param>
	/// <param name="paymentOrderId">決済注文ID</param>
	/// <returns>自動決済キー(regKey)</returns>
	private string GetRegKey(string orderId, string paymentOrderId)
	{
		if (!this.IsLoggedIn) return string.Empty;
		var userService = new UserService();
		var userExtend = userService.GetUserExtend(this.LoginUser.UserId);
		if (userExtend != null)
		{
			var regKey = StringUtility.ToEmpty(userExtend.UserExtendDataValue[Constants.LINEPAY_USEREXRTEND_COLUMNNAME_REGKEY]);

			// 自動決済キーが存在する場合有効性をチェック
			if (string.IsNullOrEmpty(regKey) == false)
			{
				var status = LinePayApiFacade.ValidateRegKey(
					regKey,
					new LinePayApiFacade.LinePayLogInfo(orderId, paymentOrderId, ""));

				// ステータスが有効の場合regKeyを返す
				if (status.IsSuccess) return regKey;

				LogCreator.CreateErrorMessage(status.ReturnCode, status.ReturnMessage);
			}
		}
		return string.Empty;
	}

	/// <summary>
	/// 注文変更向けRequest（Requestを送信して,LINE決済画面へ遷移）
	/// </summary>
	/// <param name="orderId">orderId</param>
	/// <returns>Payment Url</returns>
	public void RequestForModify(string orderId)
	{
		var orderDetailUrlCreator =
			new UrlCreator(
				Constants.PROTOCOL_HTTPS + Constants.SITE_DOMAIN + Constants.PATH_ROOT_FRONT_PC
				+ Constants.PAGE_FRONT_ORDER_HISTORY_DETAIL).AddParam(Constants.REQUEST_KEY_ORDER_ID, orderId);

		var param = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
		if ((param == null)
			|| (param.ContainsKey("order_new") == false)
			|| ((OrderModel)param["order_new"]).OrderId != orderId)
		{
			RedirectToNextPage(null, WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_AUTH_ERROR));
		}
		var orderNew = (OrderModel)param["order_new"];

		var cart = CartObject.CreateCartByOrder(orderNew);
		cart.SettlementCurrency = orderNew.SettlementCurrency;
		cart.SettlementRate = orderNew.SettlementRate;
		cart.SettlementAmount = orderNew.SettlementAmount;

		var request = LinePayUtility.CreateRequestPayment(
			cart,
			new RedirectUrls
			{
				ConfirmUrl = orderDetailUrlCreator.AddParam("action", "linepay").CreateUrl(),
				CancelUrl = orderDetailUrlCreator.CreateUrl(),
			});
		var response = LinePayApiFacade.RequestPayment(
			request,
			new LinePayApiFacade.LinePayLogInfo(orderId, request.PaymentOrderId, ""));

		// Handle API response
		if (response.IsSuccess == false)
		{
			RedirectToErrorPage(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_AUTH_ERROR));
		}
		Response.Redirect(response.Info.PaymentUrl.WebUrl);
	}

	/// <summary>
	/// Confirm
	/// </summary>
	/// <param name="orderId">注文ID</param>
	private void ConfirmForOrder(string orderId)
	{
		var paymentOrderId = Request["orderId"];	// LinePayからは"orderId"の名称でPaymentOrderIdが届く
		var transactionId = Request["transactionId"];

		// セッション復元
		var param = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
		if (param == null)
		{
			RedirectToErrorPage(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_ANOTHER_BROWSER_ERROR));
		}
		var landingCartSessionKey = (string)Session[Constants.SESSION_KEY_LANDING_CART_SESSION_KEY];
		this.CartList = (landingCartSessionKey == null) ? (CartObjectList)Session[Constants.SESSION_KEY_CART_LIST] : (CartObjectList)Session[landingCartSessionKey];
		this.LinePayOrders = (List<KeyValuePair<string, Hashtable>>)param["order_linepay"];
		var order = this.LinePayOrders.Find(o => o.Key == orderId).Value;
		var cart = this.CartList.Items.Find(c => c.OrderId == orderId);
		this.SuccessOrders = (List<Hashtable>)param["order"];
		if ((order == null) || (cart == null))
		{
			FileLogger.WriteError("セッション情報復元エラー：注文ID=" + orderId);
			RedirectToNextPage(order, WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_AUTH_ERROR));
		}

		var orderModel = new OrderService().Get(orderId);
		if (orderModel == null)
		{
			FileLogger.WriteError("LinePayReceive.ExecSuccessActionでorderが取得できませんでした。paymentOrderId:" + paymentOrderId);
			RedirectToNextPage(order, WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_AUTH_ERROR));
		}

		// LINE Confirm
		var confirmResponse = LinePayApiFacade.ConfirmPayment(
			transactionId,
			LinePayUtility.CreateConfirmRequest(orderModel),
			new LinePayApiFacade.LinePayLogInfo(orderId, paymentOrderId, transactionId));
		OrderCommon.AppendExternalPaymentCooperationLog(
			confirmResponse.IsSuccess,
			orderId,
			LogCreator.CreateMessage(orderId, paymentOrderId),
			Constants.FLG_LASTCHANGED_USER,
			UpdateHistoryAction.DoNotInsert);
		if (confirmResponse.IsSuccess == false)
		{
			RedirectToNextPage(order, WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_AUTH_ERROR));
		}

		// ユーザー情報更新
		var userExtend = new UserService().GetUserExtend(orderModel.UserId);
		userExtend.UserExtendDataValue[Constants.LINEPAY_USEREXRTEND_COLUMNNAME_REGKEY] = confirmResponse.Info.RegKey;
		new UserService().UpdateUserExtend(
			userExtend,
			orderModel.UserId,
			Constants.FLG_LASTCHANGED_USER,
			UpdateHistoryAction.Insert);

		//------------------------------------------------------
		// ３．注文確定処理
		//------------------------------------------------------
		ExecOrderComplete(orderId, paymentOrderId, transactionId, UpdateHistoryAction.Insert);

		bool successFlag;
		var register = new OrderRegisterFront(this.IsLoggedIn);
		try
		{
			successFlag = RestoreSession(orderId);
			if (successFlag)
			{
				// プロパティセット
				GetOrderPropertiesFromSession();
				register.GoogleAnalyticsParams = (List<Hashtable>)param["googleanaytics_params"];
				this.DispErrorMessages = (List<string>)param["error"];
			}
		}
		// 例外時
		catch (Exception ex)
		{
			// ログ出力
			AppLogger.WriteError(ex);
			throw new Exception("セッション情報復元エラー");
		}

		//------------------------------------------------------
		// ４．注文後処理
		//------------------------------------------------------
		if (successFlag)
		{
			// 注文完了後処理
			register.AfterOrderCompleteProcesses(order, cart, UpdateHistoryAction.DoNotInsert);

			// 注文完了画面用に注文IDを格納
			this.SuccessOrders.Add(order);
		}

		if (order != null) this.LinePayOrders.RemoveAll(o => o.Key == orderId);

		// 次の画面へ
		RedirectToNextPage(order);
	}

	/// <summary>
	/// 注文完了処理実行
	/// </summary>
	/// <param name="orderId">注文ID</param>
	/// <param name="paymentOrderId">決済注文ID</param>
	/// <param name="transactionId">トランザクションID</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <returns>レスポンスメッセージ</returns>
	private void ExecOrderComplete(
		string orderId,
		string paymentOrderId,
		string transactionId,
		UpdateHistoryAction updateHistoryAction)
	{
		// セッションデータいったん復元（削除しない）
		var sessionData = SessionSecurityManager.RestoreSessionFromDatabaseForGoToOtherSite(orderId, false);

		var param = (Hashtable)sessionData[Constants.SESSION_KEY_PARAM];
		var linePayOrders = (List<KeyValuePair<string, Hashtable>>)param["order_linepay"];
		// 決済処理中の注文情報、カート情報を取得
		var landingCartSessionKey = (string)sessionData[Constants.SESSION_KEY_LANDING_CART_SESSION_KEY];
		this.CartList = (landingCartSessionKey == null) ? (CartObjectList)sessionData[Constants.SESSION_KEY_CART_LIST] : (CartObjectList)sessionData[landingCartSessionKey];

		var order = linePayOrders.First(o => (o.Key == orderId)).Value;
		var cart = this.CartList.Items.First(c => c.OrderId == orderId);

		order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = paymentOrderId;
		var paymentMemo = OrderCommon.CreateOrderPaymentMemoForAuth(
			orderId,
			paymentOrderId,
			Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
			transactionId,
			cart.PriceTotal);
		order[Constants.FIELD_ORDER_PAYMENT_MEMO] = OrderExternalPaymentUtility.SetExternalPaymentMemo(
			StringUtility.ToEmpty(order[Constants.FIELD_ORDER_PAYMENT_MEMO]),
			paymentMemo);

		// 定期会員フラグ更新
		if (Constants.FIXEDPURCHASE_MEMBER_CONDITION_INCLUDES_ORDER_PAYMENT_STATUS_COMPLETE
			&& ((cart.IsFixedPurchaseMember == false) && cart.HasFixedPurchase))
		{
			new UserService().UpdateFixedPurchaseMemberFlg(
				(string)order[Constants.FIELD_ORDER_USER_ID],
				Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_ON,
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.DoNotInsert);
		}

		// 注文ステータス更新
		var updateOrder = UpdateOrderStatus(
			order,
			cart,
			transactionId,
			Constants.PAYMENT_LINEPAY_PAYMENTSTATUSCOMPLETE,
			UpdateHistoryAction.DoNotInsert);

		// 更新できたらその他処理実行
		if (updateOrder != 0)
		{
			var user = new UserService().Get((string)order[Constants.FIELD_USER_USER_ID]);
			var isUser = UserService.IsUser(user.UserKbn);

			var isSendMail = new OrderService().GetOrdersByUserId(cart.OrderUserId)
				.Where(userOrder => ((userOrder.OrderId != orderId) && (userOrder.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_TEMP)))
				.Any(userOrder => this.CartList.Items.Select(item => item.OrderId).Contains(userOrder.OrderId));
			if (isSendMail) order[Constants.ORDER_KEY_MAIL_FOR_USER_REGISTER_WHEN_ORDER_COMPLETE] = null;

			// 注文完了時処理
			var register = new OrderRegisterFront(isUser);
			register.OrderCompleteProcesses(order, cart, UpdateHistoryAction.DoNotInsert);
			// 完了後処理
			register.AfterOrderCompleteProcesses(order, cart, updateHistoryAction);

			cart.IsOrderDone = true;
		}
	}

	/// <summary>
	/// キャンセルアクション
	/// </summary>
	/// <param name="orderId"></param>
	private void ExecCancelAction(string orderId)
	{
		// セッション復元＆注文ロールバック
		var order = RestoreSessionAndRollbackOrder(orderId, UpdateHistoryAction.Insert);
		if (order == null)
		{
			FileLogger.WriteError("LinePayReceive.ExecCancelActionでorderが取得できませんでした。");
			RedirectToNextPage(null, WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_AUTH_ERROR));
		}

		// 注文リストから削除＆キャンセルリストに追加
		RemoveFromOrderList(orderId);
		this.CancelOrders.Add(order);

		// 注文ID保持用セッションを削除
		this.Session.Remove(Constants.SESSION_KEY_ORDER_ID_CHECK_FOR_LINK_TYPE_PAYMENT);

		// 次の画面へ遷移
		RedirectToNextPage(order);
	}

	/// <summary>
	/// 注文リストから削除
	/// </summary>
	/// <param name="orderId">注文ID</param>
	private void RemoveFromOrderList(string orderId)
	{
		this.LinePayOrders.RemoveAll(o => o.Key == orderId);
	}

	/// <summary>
	/// エラーページへ遷移
	/// </summary>
	/// <param name="errorMessge">エラメッセージ</param>
	/// <param name="returnPage">戻りページ</param>
	private void RedirectToErrorPage(string errorMessge, string returnPage = null)
	{
		HttpContext.Current.Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessge;

		var errorUrlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		if (string.IsNullOrEmpty(returnPage) == false)
		{
			errorUrlCreator.AddParam(Constants.REQUEST_KEY_BACK_URL, returnPage);
		}
		Response.Redirect(errorUrlCreator.CreateUrl());
	}
}