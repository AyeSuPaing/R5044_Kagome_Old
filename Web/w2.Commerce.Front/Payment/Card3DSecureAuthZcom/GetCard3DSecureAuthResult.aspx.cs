/*
=========================================================================================================
  Module      : Get Card 3DSecure Auth Result (GetCard3DSecureAuthResult.aspx.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.GMO.Zcom;
using w2.App.Common.Order.Payment.GMO.Zcom.CheckAuth;
using w2.Common.Logger;
using w2.Common.Web;
using w2.Domain.FixedPurchase;
using w2.Domain.Order;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;

/// <summary>
/// Payment card 3DSecure auth Zcom get card 3DSecure auth result
/// </summary>
public partial class Payment_Card3DSecureAuthZcom_GetCard3DSecureAuthResult : OrderCartPageExternalPayment
{
	/// <summary>
	/// Page load
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		this.OrderNumber = StringUtility.ToEmpty(Request[ZcomConst.PARAM_ORDER_NUMBER]);
		this.IsSuccess = (string.IsNullOrEmpty(this.OrderNumber) == false);

		if (this.IsSuccess == false) return;

		WritePaymentLog(
			PaymentFileLogger.PaymentProcessingType.Zcom3DSecureAuthResultNotification,
			LogCreator.CreateMessage(this.OrderId, string.Empty));

		ExecCard3DSecureAuthResult();
	}


	/// <summary>
	/// Exec card 3DSecure auth result
	/// </summary>
	private void ExecCard3DSecureAuthResult()
	{
		InitComponents();

		// When order information or cart information is null
		if (this.IsSuccess == false)
		{
			AppLogger.WriteError(string.Format("セッション情報復元エラー：注文ID={0}", this.OrderId));

			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_AUTH_EXCEPTION);
			var errorUrlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR)
				.AddParam(Constants.REQUEST_KEY_ERRORPAGE_KBN, Constants.KBN_REQUEST_ERRORPAGE_GOTOP)
				.CreateUrl();
			Response.Redirect(errorUrlCreator);
		}

		var statusPayment = GetStatusPayment();
		switch (statusPayment)
		{
			case ZcomConst.STATUS_SUCCESSFUL_PAYMENT:
				ExecPaymentSuccess();
				return;

			case ZcomConst.STATUS_BACK_FROM_PAYMENT:
				BackFromPayment();
				return;

			case ZcomConst.STATUS_ERROR_PAYMENT:
				ExecPaymentError();
				return;
		}
	}

	/// <summary>
	/// Init components
	/// </summary>
	private void InitComponents()
	{
		try
		{
			this.ContractCode = StringUtility.ToEmpty(Request[ZcomConst.PARAM_CONTRACT_CODE]);
			this.TransCode = StringUtility.ToEmpty(Request[ZcomConst.PARAM_TRANS_CODE]);
			this.UserId = HttpUtility.UrlDecode(StringUtility.ToEmpty(Request[ZcomConst.PARAM_USER_ID]));
			this.PaymentCode = StringUtility.ToEmpty(Request[ZcomConst.PARAM_PAYMENT_CODE]);
			this.State = StringUtility.ToEmpty(Request[ZcomConst.PARAM_STATE]);
			this.ErrorMessage = HttpUtility.UrlDecode(StringUtility.ToEmpty(Request[ZcomConst.PARAM_ERR_DETAIL]));
			this.ErrorCode = StringUtility.ToEmpty(Request[ZcomConst.PARAM_ERR_CODE]);
			this.OrderRegister = new OrderRegisterFront(this.IsLoggedIn);

			// Check order url session
			CheckOrderUrlSession();

			var landingCartSessionKey = StringUtility.ToEmpty(Session[Constants.SESSION_KEY_LANDING_CART_SESSION_KEY]);
			var paramSession = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
			this.DispErrorMessages = (List<string>)paramSession["error"];

			// Set order register information
			this.OrderRegister.SuccessOrders = (List<Hashtable>)paramSession["order"];
			this.OrderRegister.ZcomCard3DSecurePaymentOrders = (List<Hashtable>)paramSession[Constants.SESSION_KEY_PAYMENT_CREDIT_ZCOM_ORDER_3DSECURE];
			this.OrderInfo = this.OrderRegister.ZcomCard3DSecurePaymentOrders.Find(order =>
				StringUtility.ToEmpty(order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]) == this.OrderNumber);
			this.OrderId = StringUtility.ToEmpty(this.OrderInfo[Constants.FIELD_ORDER_ORDER_ID]);

			// Restore session
			RestoreSession(this.OrderId);

			this.CartList = string.IsNullOrEmpty(landingCartSessionKey)
				? GetCartObjectList()
				: (CartObjectList)Session[landingCartSessionKey];
			this.CartObject = this.CartList.Items.Find(item => (item.OrderId == this.OrderId));
			this.IsOrderPaymentCreditCardBranchNew = (this.CartObject.Payment.CreditCardBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW);

			this.IsSuccess = ((this.OrderInfo != null) && (this.CartObject != null));
		}
		catch (Exception exception)
		{
			this.IsSuccess = false;
			AppLogger.WriteError(exception);
		}
	}

	/// <summary>
	/// Exec payment error
	/// </summary>
	private void ExecPaymentError()
	{
		ExecCancelOrder();
		RemoveSuccessOrders();
		this.OrderRegister.ZcomCard3DSecurePaymentOrders.Remove(this.OrderInfo);

		AppLogger.WriteError(LogCreator.CreateErrorMessage(
			this.ErrorCode,
			this.ErrorMessage));
		Session[Constants.SESSION_KEY_ERROR_MSG] = this.ErrorMessage;

		var errorUrlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR)
			.AddParam(Constants.REQUEST_KEY_ERRORPAGE_KBN, Constants.KBN_REQUEST_ERRORPAGE_GOCART)
			.AddParam(Constants.REQUEST_KEY_BACK_URL, GetNextPageUrl())
			.CreateUrl();
		Response.Redirect(errorUrlCreator);
	}

	/// <summary>
	/// Back from payment
	/// </summary>
	private void BackFromPayment()
	{
		// Execute cancel order
		ExecCancelOrder();
		RemoveSuccessOrders();
		this.OrderRegister.ZcomCard3DSecurePaymentOrders.Remove(this.OrderInfo);

		Response.Redirect(GetNextPageUrl());
	}

	/// <summary>
	/// Next page url
	/// </summary>
	private string GetNextPageUrl()
	{
		var nextPageUrl = this.SecurePageProtocolAndHost;
		if (this.CartList.IsLandingCart == false)
		{
			Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_CONFIRM;
			nextPageUrl += string.Format("{0}{1}", Constants.PATH_ROOT, Constants.PAGE_FRONT_ORDER_CONFIRM);
			return nextPageUrl;
		}

		var nextPage = StringUtility.ToEmpty(this.Session[Constants.SESSION_KEY_LANDING_CART_INPUT_ABSOLUTE_PATH]);
		var keyNextPageForCheck = string.Format(
			"{0}{1}",
			Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK,
			string.IsNullOrEmpty(this.Request[Constants.REQUEST_KEY_RETURN_URL])
				? nextPage
				: this.Request[Constants.REQUEST_KEY_RETURN_URL]);

		Session[keyNextPageForCheck] = Constants.PAGE_FRONT_LANDING_LANDING_CART_CONFIRM;
		nextPageUrl += nextPage;
		return nextPageUrl;
	}

	/// <summary>
	/// Exec payment success
	/// </summary>
	private void ExecPaymentSuccess()
	{
		try
		{
			var orderService = new OrderService();
			PaymentZcomCreditCheckAuth();

			if (this.IsSuccess == false)
			{
				this.TransactionName = "2-X.注文ロールバック処理";
				var isUserDeleted = (((this.IsLoggedIn == false) || (this.RegisterUser != null))
					&& (this.SuccessOrders.Count == 0)
					&& (orderService.GetOrdersByUserId(this.CartObject.OrderUserId).Length == 1));

				if (Constants.LANDING_CART_USER_REGISTER_WHEN_ORDER_COMPLETE
					&& this.IsLoggedIn
					&& isUserDeleted)
				{
					this.LoginUserId = null;
				}

				WritePaymentLog(
					PaymentFileLogger.PaymentProcessingType.CreditPaymentWithThreeDSecure,
					OrderCommon.CreateOrderFailedLogMessage(
						this.TransactionName,
						this.OrderInfo,
						this.CartObject));
			}
			else
			{
				this.IsSuccess = this.OrderRegister.UpdateForOrderComplete(
					this.OrderInfo,
					this.CartObject,
					true,
					UpdateHistoryAction.DoNotInsert);

				if (this.IsSuccess)
				{
					var isSendMail = orderService.GetOrdersByUserId(this.CartObject.OrderUserId)
						.Where(userOrder => ((userOrder.OrderId != this.OrderId)
							&& (userOrder.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_TEMP)))
						.Any(userOrder => this.CartList.Items
							.Select(item => item.OrderId)
							.Contains(userOrder.OrderId));

					if (isSendMail) this.OrderInfo[Constants.ORDER_KEY_MAIL_FOR_USER_REGISTER_WHEN_ORDER_COMPLETE] = null;

					// Order complete processes
					this.OrderRegister.OrderCompleteProcesses(
						this.OrderInfo,
						this.CartObject,
						UpdateHistoryAction.DoNotInsert);

					// After order complete processes
					this.OrderRegister.AfterOrderCompleteProcesses(
						this.OrderInfo,
						this.CartObject,
						UpdateHistoryAction.Insert);

					// Add success orders
					this.OrderRegister.SuccessOrders.Add(this.OrderInfo);
				}
			}
		}
		catch (Exception exception)
		{
			this.DispErrorMessages.Add(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_AUTH_EXCEPTION));
			var logMessage = OrderCommon.CreateOrderFailedLogMessage(
				this.TransactionName,
				this.OrderInfo,
				this.CartObject);

			WritePaymentLog(
				PaymentFileLogger.PaymentProcessingType.CreditPaymentWithThreeDSecure,
				BaseLogger.CreateExceptionMessage(logMessage, exception));
		}

		// Removed from 3DSecure payment order list
		if (this.OrderInfo != null)
		{
			this.OrderRegister.ZcomCard3DSecurePaymentOrders.Remove(this.OrderInfo);
		}

		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_SETTLEMENT;
		Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SETTLEMENT);
	}

	/// <summary>
	/// Payment zcom credit check auth
	/// </summary>
	private void PaymentZcomCreditCheckAuth()
	{
		// Check auth
		var zcomCheckAuthResponse = new ZcomCheckAuthRequestAdapter(this.ContractCode, this.OrderNumber).Excute();

		if (zcomCheckAuthResponse.State == ZcomConst.STATUS_PROVISIONAL_SALES)
		{
			PaymentFileLogger.WritePaymentLog(
				true,
				Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
				PaymentFileLogger.PaymentType.Zcom,
				PaymentFileLogger.PaymentProcessingType.CreditPaymentWithThreeDSecure,
				"認証成功、オーソリ成功");

			var paymentStatusCompleteFlg = (Constants.DIGITAL_CONTENTS_OPTION_ENABLED
					&& this.CartObject.HasDigitalContents)
				? Constants.PAYMENT_CARD_PATMENT_STAUS_COMPLETE_FORDIGITALCONTENTS
				: Constants.PAYMENT_CARD_PATMENT_STAUS_COMPLETE;
			this.OrderInfo[Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS] = paymentStatusCompleteFlg
				? Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE
				: Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM;
			this.OrderInfo[Constants.FIELD_ORDER_CARD_TRAN_ID] = StringUtility.ToEmpty(zcomCheckAuthResponse.TransCode);
			this.OrderInfo[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = zcomCheckAuthResponse.OrderNumber;

			if (this.IsOrderPaymentCreditCardBranchNew)
			{
				this.CartObject.Payment.UserCreditCard.UpdateCooperationId(
					zcomCheckAuthResponse.UserId,
					Constants.FLG_LASTCHANGED_USER,
					UpdateHistoryAction.DoNotInsert);

				this.CartObject.Payment.UserCreditCard.UpdateDispFlg(
					this.CartObject.Payment.UserCreditCardRegistFlg,
					Constants.FLG_LASTCHANGED_USER,
					UpdateHistoryAction.DoNotInsert);
			}

			return;
		}

		ExecCancelOrder();
		RemoveSuccessOrders();
		this.OrderRegister.ZcomCard3DSecurePaymentOrders.Remove(this.OrderInfo);
		this.IsSuccess = false;
	}

	/// <summary>
	/// Exec cancel order
	/// </summary>
	private void ExecCancelOrder()
	{
		var order = OrderCommon.GetOrder(this.OrderId)[0];
		var orderId = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]);
		var fixedPurchaseId = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_FIXED_PURCHASE_ID]);
		var userId = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_USER_ID]);
		var updateHistoryService = new UpdateHistoryService();

		using (var accessor = new SqlAccessor())
		{
			accessor.OpenConnection();
			accessor.BeginTransaction();

			OrderCommon.CancelOrderSubProcess(
				order,
				false,
				"注文キャンセルバッチ",
				Constants.W2MP_DEPT_ID,
				Constants.FLG_LASTCHANGED_CGI,
				true,
				UpdateHistoryAction.DoNotInsert,
				accessor);

			new OrderService().Modify(
				orderId,
				(orderModel) =>
				{
					orderModel.OrderStatus = Constants.FLG_ORDER_ORDER_STATUS_TEMP_CANCELED;
					orderModel.LastChanged = Constants.FLG_LASTCHANGED_CGI;
				},
				UpdateHistoryAction.Insert,
				accessor);

			updateHistoryService.InsertForFixedPurchase(fixedPurchaseId, Constants.FLG_LASTCHANGED_CGI, accessor);
			updateHistoryService.InsertForOrder(orderId, Constants.FLG_LASTCHANGED_BATCH, accessor);
			updateHistoryService.InsertForUser(userId, Constants.FLG_LASTCHANGED_CGI, accessor);

			new FixedPurchaseService().CancelTemporaryRegistrationFixedPurchase(
				fixedPurchaseId,
				Constants.FLG_LASTCHANGED_CGI,
				UpdateHistoryAction.Insert,
				accessor,
				orderId);

			accessor.CommitTransaction();
		}

		FileLogger.WriteInfo(string.Format(
			"{0} を仮注文キャンセルしました。",
			StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID])));
	}

	/// <summary>
	/// Get status payment
	/// </summary>
	/// <returns>Status payment</returns>
	private string GetStatusPayment()
	{
		if (string.IsNullOrEmpty(this.ErrorMessage) == false) return ZcomConst.STATUS_ERROR_PAYMENT;

		if ((string.IsNullOrEmpty(this.PaymentCode) == false)
			&& (string.IsNullOrEmpty(this.State) == false))
		{
			return ZcomConst.STATUS_SUCCESSFUL_PAYMENT;
		}

		return ZcomConst.STATUS_BACK_FROM_PAYMENT;
	}

	/// <summary>
	/// Write payment log
	/// </summary>
	/// <param name="paymentProcessingType">Payment processing type</param>
	/// <param name="logMessage">Log message</param>
	private void WritePaymentLog(PaymentFileLogger.PaymentProcessingType paymentProcessingType, string logMessage)
	{
		PaymentFileLogger.WritePaymentLog(
			this.IsSuccess,
			Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
			PaymentFileLogger.PaymentType.Zcom,
			paymentProcessingType,
			logMessage,
			new Dictionary<string, string>
			{
				{ Constants.FIELD_ORDER_ORDER_ID, this.OrderId },
				{ Constants.FIELD_ORDER_USER_ID, this.CartList.UserId },
			});
	}

	/// <summary>
	/// Remove success orders
	/// </summary>
	private void RemoveSuccessOrders()
	{
		foreach (Hashtable order in this.OrderRegister.SuccessOrders)
		{
			var orderId = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]);
			var cartRemove = this.CartList.Items.FirstOrDefault(cart => (cart.OrderId == orderId));

			if (cartRemove == null) continue;

			this.CartList.DeleteCartVurtual(cartRemove);
		}
	}

	/// <summary>Order info</summary>
	private Hashtable OrderInfo { get; set; }
	/// <summary>Cart object</summary>
	private CartObject CartObject { get; set; }
	/// <summary>Order register</summary>
	private OrderRegisterFront OrderRegister { get; set; }
	/// <summary>Is order payment credit card branch new</summary>
	private bool IsOrderPaymentCreditCardBranchNew { get; set; }
	/// <summary>Is success</summary>
	private bool IsSuccess { get; set; }
	/// <summary>Order id</summary>
	private string OrderId { get; set; }
	/// <summary>Contract code</summary>
	private string ContractCode { get; set; }
	/// <summary>Trans code</summary>
	private string TransCode { get; set; }
	/// <summary>Order number</summary>
	protected string OrderNumber { get; set; }
	/// <summary>User id</summary>
	private string UserId { get; set; }
	/// <summary>Payment code</summary>
	private string PaymentCode { get; set; }
	/// <summary>State</summary>
	private string State { get; set; }
	/// <summary>Error message</summary>
	private new string ErrorMessage { get; set; }
	/// <summary>Error code</summary>
	private string ErrorCode { get; set; }
}
