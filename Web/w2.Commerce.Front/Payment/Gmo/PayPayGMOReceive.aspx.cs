/*
=========================================================================================================
  Module      : メルペイ決済 決済結果受信画面 (PayPayGMOReceive.aspx.cs)
･････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using SessionWrapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.Paypay;
using w2.Common.Extensions;
using w2.Common.Helper;
using w2.Common.Logger;
using w2.Common.Web;
using w2.Domain;
using w2.Domain.Order;
using w2.Domain.Order.Helper;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;

public partial class Form_Payment_Gmo_PayPayReceive : OrderCartPageExternalPayment
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (IsRequestedRequiredParameters() == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		var landingSessionKey = (string)this.Session[Constants.SESSION_KEY_LANDING_CART_SESSION_KEY];
		this.CartList = (landingSessionKey == null)
			? GetCartObjectList()
			: (CartObjectList)this.Session[landingSessionKey];

		var param = (Hashtable)this.Session[Constants.SESSION_KEY_PARAM];
		if (param == null)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PAYPAY_IRREGULAR_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		this.OrderRegister = new OrderRegisterFront(this.IsLoggedIn)
		{
			SuccessOrders = (List<Hashtable>)param["order"],
			PaypayOrders = (Dictionary<string, Hashtable>)param["paypay_order"],
			GoogleAnalyticsParams = (List<Hashtable>)param["googleanaytics_params"],
		};

		var order = new OrderService().Get(this.RequestOrderId);
		// 決済対象注文IDが仮注文のまま、注文対象ではなくなっている場合、決済取消処理
		if ((this.OrderRegister.PaypayOrders.ContainsKey(this.RequestOrderId) == false) && order.IsTempOrder)
		{
			var cancelResult = new PaypayGmoFacade().CancelPayment(order);
			PaymentFileLogger.WritePaymentLog(
				cancelResult.Result == Results.Success,
				order.PaymentName ?? string.Empty,
				PaymentFileLogger.PaymentType.PayPay,
				PaymentFileLogger.PaymentProcessingType.CancelPayment,
				WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PAYPAY_UNNORMAL_PAGE_REDIRECT_ERROR)
					.Replace("@@ 1 @@", LogCreator.CrateMessageWithCardTranId(order.CardTranId ?? order.CardTranId, string.Empty))
					.Replace("@@ 2 @@", PaymentFileLogger.PaymentType.PayPay.ToText()));

			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PAYPAY_IRREGULAR_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		// 決済処理中の注文情報
		this.PaymentOrder = this.OrderRegister.PaypayOrders[this.RequestOrderId];

		this.IsExecutionPayPayFromCartList = (order.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_TEMP);
		this.IsExecutionPayPayFromMyPage = !this.IsExecutionPayPayFromCartList;

		// 結果通知プログラムにより
		// 既に対象受注が仮注文ではなくなっている可能性を考慮
		if (order.IsTempOrder == false)
		{
			if (this.PaymentOrder != null)
			{
				this.OrderRegister.SuccessOrders.Add(this.PaymentOrder);
				this.OrderRegister.PaypayOrders.Remove(this.RequestOrderId);
			}

			foreach (var ht in this.OrderRegister.SuccessOrders)
			{
				var orderId = StringUtility.ToEmpty(ht[Constants.FIELD_ORDER_ORDER_ID]);
				var cartRemove = this.CartList.Items.FirstOrDefault(cart => (cart.OrderId == orderId));
				if (cartRemove == null) continue;
				cartRemove.Payment.CreditToken = null;

				this.OrderRegister.AddGoogleAnalyticsParams(order.OrderId, cartRemove);
				this.CartList.DeleteCartVurtual(cartRemove);
			}

			Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_SETTLEMENT;
			Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SETTLEMENT);
		}

		var success = false;
		try
		{
			success = this.IsExecutionPayPayFromCartList
				? RestoreSession(this.RequestOrderId)
				: SessionSecurityManager.RestoreSessionFromDatabaseForGoToOtherSite(Session, this.RequestOrderId, true);

			if (success == false)
			{
				FileLogger.WriteError("PaypayReceive DBからのセッション復元に失敗しました " + GetCurrentStateStringsForLog());
			}

			if (success)
			{
				success = this.IsExecutionPayPayFromCartList
					? ExecuteForCartList(order, UpdateHistoryAction.Insert)
					: ExecuteForMyPage(order, UpdateHistoryAction.DoNotInsert); // マイページ側で更新履歴は入る
			}

			if (success == false)
			{
				if (this.IsExecutionPayPayFromCartList)
				{
					Session[Constants.SESSION_KEY_ERROR_FOR_PAYPAY_PAYMENT] = this.ErrorMessage;
					var landingCartSessionKey = (string)Session[Constants.SESSION_KEY_LANDING_CART_SESSION_KEY];
					var nextPage = (string.IsNullOrEmpty(landingCartSessionKey))
						? Constants.PAGE_FRONT_ORDER_CONFIRM
						: Constants.PAGE_FRONT_LANDING_LANDING_CART_CONFIRM;

					if (string.IsNullOrEmpty(landingCartSessionKey))
					{
						Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = nextPage;
					}
					else
					{
						var landingCartNextPageForCheck = string.Format(
							"{0}{1}",
							Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK,
							Session[Constants.SESSION_KEY_LANDING_CART_INPUT_ABSOLUTE_PATH]);
						Session[landingCartNextPageForCheck] = nextPage;
					}

					this.CartList.Items.RemoveAll(item => item.IsOrderDone);
					Response.Redirect(
						new UrlCreator(Constants.PATH_ROOT + nextPage)
							.CreateUrl());
				}
				else
				{
					Session[Constants.SESSION_KEY_ERROR_FOR_PAYPAY_PAYMENT] = this.ErrorMessage;
					var nextPage = new UrlCreator(
						Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_HISTORY_DETAIL)
							.AddParam(Constants.REQUEST_KEY_ORDER_ID, this.RequestOrderId)
							.CreateUrl();
					Response.Redirect(nextPage);
				}
			}
		}
		catch (ThreadAbortException)
		{
			// この例外はResponse.Redirectで飛ぶので無視
		}
		catch (Exception ex)
		{
			FileLogger.WriteError("PaypayReveive 予期しない例外が発生 " + GetCurrentStateStringsForLog(), ex);

			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_AUTH_EXCEPTION);
			Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_CONFIRM;
			Response.Redirect(
				new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_CONFIRM)
					.CreateUrl());
		}
		if (this.IsExecutionPayPayFromCartList)
		{
			Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_SETTLEMENT;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SETTLEMENT);
		}
		else if (this.IsExecutionPayPayFromMyPage)
		{
			RedirectToOrderHistoryPage(success);
		}

	}

	/// <summary>
	/// 必須パラメータが指定されているか？
	/// </summary>
	/// <returns></returns>
	private bool IsRequestedRequiredParameters()
	{
		var orderId = this.Request[PaypayConstants.REQUEST_KEY_ORDERID];
		return (string.IsNullOrEmpty(orderId) == false);
	}

	/// <summary>
	/// 現在の状態を文字列で取得（ログ用）
	/// </summary>
	/// <returns>現在の状態</returns>
	private string GetCurrentStateStringsForLog()
	{
		var param = new List<KeyValuePair<string, string>>
		{
			new KeyValuePair<string, string>("ua", this.Request.UserAgent),
			new KeyValuePair<string, string>("addr", this.Request.ServerVariables["REMOTE_ADDR"]),
			new KeyValuePair<string, string>("oid", this.RequestOrderId),
			new KeyValuePair<string, string>("uid", this.LoginUserId),
			new KeyValuePair<string, string>("sid", this.Session.SessionID),
		};

		var result = param
			.Select(kvp => string.Format("{0}={1}", kvp.Key, StringUtility.ToValueIfNullOrEmpty(kvp.Value, "UNKNOWN")))
			.JoinToString(" ");
		return result;
	}

	/// <summary>
	/// 実行
	/// </summary>
	/// <param name="order">注文情報</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <returns>成功</returns>
	private bool ExecuteForCartList(OrderModel order, UpdateHistoryAction updateHistoryAction)
	{
		// セッション上に対象注文が存在しない場合は
		// 既に結果通知プログラムにより注文完了済みの新規注文と判定する
		var cart = GetTargetCart();
		if (cart == null)
		{
			RestoreSession(order.OrderId);
			this.PaypayOrders.Remove(order.OrderId);
			this.SuccessOrders.Add(order.DataSource);
			Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_SETTLEMENT;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SETTLEMENT);
		}

		var paymentResult = new PaypayGmoFacade().ReceiveExternalSitePaymentResult(this.Context, order);
		if (paymentResult.Result.HasFlag(Results.PreOrderRollbackIsRequired))
		{
			// 仮注文削除
			OrderCommon.RollbackPreOrder(
				order.DataSource,
				cart,
				(this.IsLoggedIn == false),
				0,
				this.IsLoggedIn,
				UpdateHistoryAction.Insert);
		}

		if (paymentResult.Result.HasFlag(Results.Failed))
		{
			this.PaypayOrders.Remove(order.OrderId);
			this.ErrorMessage = paymentResult.ErrorMessage;
			return false;
		}

		if (paymentResult.Status == Statuses.Canceled)
		{
			this.PaypayOrders.Remove(order.OrderId);
			this.CancelOrders.Add(order.DataSource);
			return true;
		}

		if (paymentResult.Result.HasFlag(Results.Success))
		{
			this.PaypayOrders.Remove(order.OrderId);
			this.SuccessOrders.Add(order.DataSource);

			if (order.IsFixedPurchaseOrder)
			{
				cart.FixedPurchase = DomainFacade.Instance.FixedPurchaseService.Get(order.FixedPurchaseId);
				order.PaymentOrderId = OrderCommon.CreatePaymentOrderId(cart.ShopId);
				var result = new PaypayGmoFacade().ExecPayment(cart, order, true);

				if (string.IsNullOrEmpty(result.ErrorMessage) == false)
				{
					this.ErrorMessage = result.ErrorMessage;
					return false;
				}

				order.OnlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;
			}

			// 注文確定
			var register = new OrderRegisterFront(this.IsLoggedIn)
			{
				PaypayOrders = this.PaypayOrders
			};

			order.AppendPaymentMemo(
				OrderCommon.CreateOrderPaymentMemo(
					order.PaymentOrderId,
					order.OrderPaymentKbn,
					order.CardTranId,
					"結果受信",
					order.LastBilledAmount));

			if (register.UpdateForOrderComplete(
				order.DataSource,
				cart,
				true,
				UpdateHistoryAction.DoNotInsert) == false)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = register.ErrorMessages;
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}

			register.OrderCompleteProcesses(order.DataSource, cart, UpdateHistoryAction.DoNotInsert);
			register.AfterOrderCompleteProcesses(order.DataSource, cart, UpdateHistoryAction.DoNotInsert);

			// Update external payment status
			if (order.IsFixedPurchaseOrder)
			{
				DomainFacade.Instance.OrderService.Modify(
					order.OrderId,
					model =>
					{
						model.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP;
					},
					UpdateHistoryAction.DoNotInsert);
			}

			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertAllForOrder(order.OrderId, Constants.FLG_LASTCHANGED_USER);
			}
		}
		return true;
	}

	/// <summary>
	/// 実行（マイページの決済変更）
	/// </summary>
	/// <param name="order">注文情報</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <returns>正常: true</returns>
	private bool ExecuteForMyPage(OrderModel order, UpdateHistoryAction updateHistoryAction)
	{
		var paypayOrder = GmoSessionWrapper.FindGmoMultiPendingOrder(this.RequestOrderId);
		if (paypayOrder == null)
		{
			FileLogger.WriteError(
				"PaypayReceive マイページで指定された注文情報をセッションから取得できませんでした。"
				+ GetCurrentStateStringsForLog());
			return false;
		}

		var response = new PaypayGmoFacade().ReceiveExternalSitePaymentResult(this.Context, paypayOrder);
		if (response.Result.HasFlag(Results.Failed))
		{
			// エンドユーザーによるキャンセルの場合はそのままマイページへ戻る
			if (response.Status == Statuses.Canceled)
			{
				GmoSessionWrapper.RemoveGmoMultiPendingOrder(paypayOrder.OrderId);
				RedirectToOrderHistoryPage(isSuccess: true);
				return true;
			}

			GmoSessionWrapper.RemoveGmoMultiPendingOrder(paypayOrder.OrderId);
			return false;
		}

		// 実際の更新はマイページ側
		paypayOrder.OnlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_NONE;
		paypayOrder.ExternalPaymentStatus = (order.IsFixedPurchaseOrder
				|| (response.Status != Statuses.Auth))
			? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP
			: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
		paypayOrder.ExternalPaymentAuthDate = DateTime.Now;
		var paymentMemo = OrderCommon.CreateOrderPaymentMemo(
			paypayOrder.PaymentOrderId,
			paypayOrder.OrderPaymentKbn,
			paypayOrder.CardTranId,
			"結果受信",
			paypayOrder.OrderPriceTotal);
		paypayOrder.PaymentMemo = OrderExternalPaymentUtility.SetExternalPaymentMemo(
			StringUtility.ToEmpty(paypayOrder.PaymentMemo),
			paymentMemo);
		paypayOrder.LastChanged = Constants.FLG_LASTCHANGED_USER;

		GmoSessionWrapper.AddGmoMultiPendingOrder(paypayOrder);

		if (updateHistoryAction == UpdateHistoryAction.Insert)
		{
			new UpdateHistoryService().InsertAllForOrder(paypayOrder.OrderId, Constants.FLG_LASTCHANGED_USER);
		}
		return true;
	}

	/// <summary>
	/// 注文履歴詳細画面にリダイレクト
	/// </summary>
	/// <param name="isSuccess">処理に成功したか</param>
	private void RedirectToOrderHistoryPage(bool isSuccess = false)
	{
		var nextUrl = new UrlCreator(
			Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_HISTORY_DETAIL)
				.AddParam(Constants.REQUEST_KEY_ORDER_ID, this.RequestOrderId)
				.AddParam(
					PaypayConstants.REQUEST_KEY_RECEIVE_RESULT,
					isSuccess
						? PaypayConstants.FLG_GMO_EXECUTE_RESULT_OK
						: PaypayConstants.FLG_GMO_EXECUTE_RESULT_NG)
				.CreateUrl();
		Response.Redirect(nextUrl);
	}

	/// <summary>
	/// 決済対象カートを取得
	/// </summary>
	/// <returns>カート</returns>
	private CartObject GetTargetCart()
	{
		var cart = this.CartList.Items.FirstOrDefault(c => (c.OrderId == this.RequestOrderId));
		// w2_TempDatasによるセッション復元直後はthis.CartListが取れない場合がある
		if (cart == null)
		{
			var landingCartKey = (string)Session[Constants.SESSION_KEY_LANDING_CART_SESSION_KEY];
			this.CartList = string.IsNullOrEmpty(landingCartKey)
				? GetCartObjectList()
				: (CartObjectList)Session[landingCartKey];
			cart = this.CartList.Items.FirstOrDefault(c => (c.OrderId == this.RequestOrderId));
		}
		return cart;
	}

	/// <summary>決済実行対象注文ID</summary>
	private string RequestOrderId
	{
		get { return Request[PaypayConstants.REQUEST_KEY_ORDERID]; }
	}
	/// <summary>マイページからの実行か？</summary>
	private bool IsExecutionPayPayFromMyPage { get; set; }
	/// <summary>カートページからの実行か？</summary>
	private bool IsExecutionPayPayFromCartList { get; set; }
	/// <summary>Error Message</summary>
	private string ErrorMessage { get; set; }
	/// <summary>注文登録情報</summary>
	private OrderRegisterFront OrderRegister { get; set; }
	/// <summary>決済処理中の注文情報</summary>
	private Hashtable PaymentOrder { get; set; }
}
