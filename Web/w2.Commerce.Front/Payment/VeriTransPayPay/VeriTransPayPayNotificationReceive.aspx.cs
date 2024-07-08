/*
=========================================================================================================
  Module      : ベリトランスPaypay通知受信(VeriTransPayPayNotificationReceive.aspx.cs)
  ･････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.Paypay;
using w2.App.Common.Order.Payment.Veritrans;
using w2.Common.Logger;
using w2.Domain;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;

public partial class Payment_VeriTransPayPay_VeriTransPayPayResultNotificationReceive : OrderCartPageExternalPayment
{
	/// <summary>通知件数</summary>
	private const string RESULT_NUMBER_OF_NOTIFY = "numberOfNotify";
	/// <summary>送信時刻</summary>
	private const string RESULT_PUSH_TIME = "pushTime";
	/// <summary>取引ID</summary>
	private const string RESULT_ORDERID = "orderId";
	/// <summary>Paypay取引ID</summary>
	private const string RESULT_PAYPAY_ORDERID = "paypayOrderId";
	/// <summary>詳細結果コード</summary>
	private const string RESULT_VRESULT_CODE = "vResultCode";
	/// <summary>処理ステータス</summary>
	private const string RESULT_MSTATUS = "mstatus";
	/// <summary>トランザクションタイプ</summary>
	private const string RESULT_TRAN_TYPE = "txnType";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// 結果通知プログラム
		if (string.IsNullOrEmpty(Request.Form[RESULT_NUMBER_OF_NOTIFY]) == false)
		{
			var numberOfNotify = int.Parse(Request.Form[RESULT_NUMBER_OF_NOTIFY]);

			for (int index = 0; index < numberOfNotify; index++)
			{
				var serialNumber = index.ToString("D4");
				var paymentOrderId = StringUtility.ToEmpty(Request.Form[RESULT_ORDERID + serialNumber]);
				var paypayOrderId = StringUtility.ToEmpty(Request.Form[RESULT_PAYPAY_ORDERID + serialNumber]);
				var cardMstatus = StringUtility.ToEmpty(Request.Form[RESULT_MSTATUS + serialNumber]);
				var resultCode = this.Request.Form[RESULT_VRESULT_CODE + serialNumber];

				// 決済申込完了通知以外は処理しない
				if (StringUtility.ToEmpty(Request.Form[RESULT_TRAN_TYPE + serialNumber]) != PaypayConstants.FLG_PAYPAY_COMMAND_TYPE_AUTHORIZE)
				{
					continue;
				}

				if (cardMstatus == VeriTransConst.RESULT_STATUS_NG)
				{
					PaymentLogWrite(false, paymentOrderId, resultCode);
					continue;
				}

				ProcessPaypayOrder(paymentOrderId, paypayOrderId, resultCode);
			}
			Response.StatusCode = 200;
			Response.Write("0");
			Response.End();
		}
	}

	/// <summary>
	/// Paypay決済処理
	/// <param name="paymentOrderId">取引ID</param>
	/// <param name="paypayOrderId">Paypay取引ID</param>
	/// <param name="resultCode">詳細結果コード</param>
	/// </summary>
	private void ProcessPaypayOrder(string paymentOrderId, string paypayOrderId, string resultCode)
	{
		try
		{
			var orderRegister = new OrderRegisterFront(this.IsLoggedIn);
			var isSuccess = false;
			CartObject cart = null;
			var orderInfo = new Hashtable();

			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var order = new OrderService();
				var target = order.GetOrderByPaymentOrderId(paymentOrderId, accessor);
				target.PaymentName = DomainFacade.Instance.PaymentService.Get(target.ShopId, target.OrderPaymentKbn).PaymentName;
				target.CardTranId = paypayOrderId;
				var updateOrder = order.GetAllUpdateLock(target.OrderId, accessor);

				if (updateOrder.IsTempOrder)
				{
					cart = CartObject.CreateCartByOrder(target);
					orderInfo = CreateOrderHashtable(target);

					isSuccess = ExecOrderComplete(orderRegister, cart, orderInfo, accessor);
				}

				if (isSuccess) accessor.CommitTransaction();
			}

			if (isSuccess)
			{
				// 注文確定後処理
				ExecAfterProccess(orderRegister, orderInfo, cart);

				// 決済ログ出力
				PaymentLogWrite(isSuccess, paymentOrderId, resultCode);
			}
		}
		catch (Exception ex)
		{
			// ログ出力
			PaymentLogWrite(false, paymentOrderId, resultCode);
			FileLogger.WriteError(ex);

			Response.StatusCode = 500;
			Response.Write("1");
		}
	}

	/// <summary>
	/// 決済確定情報を元に新受注情報の作成
	/// </summary>
	/// <param name="order">結果受信前の受注情報</param>
	/// <returns>受注情報</returns>
	private Hashtable CreateOrderHashtable(OrderModel order)
	{
		order.LastChanged = Constants.FLG_LASTCHANGED_CGI;
		order.AppendPaymentMemo(
			OrderCommon.CreateOrderPaymentMemo(
				order.PaymentOrderId,
				order.OrderPaymentKbn,
				order.CardTranId,
				"結果通知受取",
				order.LastBilledAmount));

		return order.DataSource;
	}

	/// <summary>
	/// 注文確定処理
	/// </summary>
	/// <param name="orderRegister">注文登録処理状態</param>
	/// <param name="cart">カート情報</param>
	/// <param name="orderInfo"></param>
	/// <param name="accessor">アクセサー</param>
	/// <returns>成功したか</returns>
	private bool ExecOrderComplete(OrderRegisterFront orderRegister, CartObject cart, Hashtable orderInfo, SqlAccessor accessor)
	{
		var isSuccess = orderRegister.UpdateForOrderComplete(
			orderInfo,
			cart,
			true,
			UpdateHistoryAction.DoNotInsert,
			false,
			accessor);

		return isSuccess;
	}

	/// <summary>
	/// 注文確定後処理
	/// </summary>
	/// <param name="orderRegister">注文登録処理状況</param>
	/// <param name="order">注文情報</param>
	/// <param name="cart">カート情報</param>
	private void ExecAfterProccess(OrderRegisterFront orderRegister, Hashtable order, CartObject cart)
	{
		if (string.IsNullOrEmpty((string)order[Constants.FIELD_ORDER_COMBINED_ORG_ORDER_IDS]) == false)
		{
			cart.OrderCombineParentOrderId = (string)order[Constants.FIELD_ORDER_COMBINED_ORG_ORDER_IDS];

			// 非同期通知のため、受注情報から注文同梱カートを作成
			SessionManager.OrderCombineCartList =
			SessionManager.OrderCombineBeforeCartList =
				CartObjectList.GetUserCartList(cart.CartUserId, cart.OrderKbn);

			new OrderCartPage().SuccessCombinedOrderAdditionProcess(
				order,
				null,
				cart.CartUserId,
				cart.Coupon);
		}

		orderRegister.OrderCompleteProcesses(
			order,
			cart,
			UpdateHistoryAction.DoNotInsert);

		orderRegister.AfterOrderCompleteProcesses(
			order,
			cart,
			UpdateHistoryAction.Insert);
	}

	/// <summary>
	/// 決済ログ出力
	/// </summary>
	/// <param name="success">決済に成功したかどうか</param>
	/// <param name="paymentOrderId">取引ID</param>
	/// <param name="resultCode">詳細結果コード</param>
	private void PaymentLogWrite(bool success, string paymentOrderId, string resultCode)
	{
		DateTime orderPaymentDate;
		DateTime.TryParseExact(
			StringUtility.ToEmpty(this.Request.Form[RESULT_PUSH_TIME]),
			"yyyyMMddHHmmss", CultureInfo.CurrentCulture, DateTimeStyles.None,
			out orderPaymentDate);

		PaymentFileLogger.WritePaymentLog(
			success,
			Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY,
			PaymentFileLogger.PaymentType.PayPay,
			PaymentFileLogger.PaymentProcessingType.GetOrderConfirmationNotice,
			"結果通知受取",
			new Dictionary<string, string>
			{
				{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, paymentOrderId },
				{ "date", orderPaymentDate.ToString("yyyy/MM/dd HH:mm:ss") },
				{ VeriTransConst.VERITRANS_REQUEST_KEY_VRESULTCODE, resultCode },
			});
	}
}
