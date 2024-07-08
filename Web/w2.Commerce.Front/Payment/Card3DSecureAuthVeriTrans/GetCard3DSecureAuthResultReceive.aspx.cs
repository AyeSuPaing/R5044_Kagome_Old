/*
=========================================================================================================
  Module      : ベリトランス3Dセキュア2.0認証結果取得ページ処理(GetCard3DSecureAuthResultReceive.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using w2.Domain.UpdateHistory.Helper;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.Veritrans;
using w2.Common.Logger;
using w2.Domain.Order;
using w2.Domain;
using System.Globalization;

public partial class Payment_Card3DSecureAuthVeriTrans_GetCard3DSecureAuthResultReceive_aspx : OrderCartPageExternalPayment
{
	// Payment notification parameter
	private const string RESULT_NUMBER_OF_NOTIFY = "numberOfNotify";    // 通知件数
	private const string RESULT_PUSH_TIME = "pushTime";                 // 送信時刻
	private const string RESULT_PUSHID = "pushId";                      // 識別ID
	private const string RESULT_ORDERID = "orderId";                    // 取引ID
	private const string RESULT_VRESULT_CODE = "vResultCode";           // 詳細結果コード
	private const string RESULT_TXNTYPE = "txnType";                    // トランザクションタイプ
	private const string RESULT_MPI_MSTATUS = "mpiMstatus";             // MPI結果コード
	private const string RESULT_CARD_MSTATUS = "cardMstatus";           // カード結果コード
	private const string RESULT_DUMMY = "dummy";                        // ダミー決済フラグ

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

			for (int i = 0; i < numberOfNotify; i++)
			{
				string orderIdField = RESULT_ORDERID + i.ToString("D4");
				string paymentOrderId = StringUtility.ToEmpty(Request.Form[orderIdField]);

				string cardMstatusField = RESULT_CARD_MSTATUS + i.ToString("D4");
				string cardMstatus = StringUtility.ToEmpty(Request.Form[cardMstatusField]);

				string mpiMstatusField = RESULT_MPI_MSTATUS + i.ToString("D4");
				string mpiMstatus = StringUtility.ToEmpty(Request.Form[mpiMstatusField]);

				string vResultCodeField = RESULT_VRESULT_CODE + i.ToString("D4");
				string vResultCode = StringUtility.ToEmpty(Request.Form[vResultCodeField]);

				// カード結果コード・MPI結果コードが両方共OKではない場合、ログを落として次の注文へ進む
				if ((cardMstatus != VeriTransConst.RESULT_STATUS_OK) || (mpiMstatus != VeriTransConst.RESULT_STATUS_OK))
				{
					PaymentLogWrite(StringUtility.ToEmpty(this.Request.Form[RESULT_PUSH_TIME]), Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT, false, orderIdField, vResultCode);
					continue;
				}

				ProcessCreditCardOrder(paymentOrderId, orderIdField);
				FileLogger.WriteDebug("クレジット通知" + i + "=>payment_order_Id：" + paymentOrderId);
			}
			Response.StatusCode = 200;
			Response.Write("0");
			Response.End();
		}
	}

	/// <summary>
	/// クレジットカード決済処理
	/// </summary>
	private void ProcessCreditCardOrder(string paymentOrderId, string orderIdField)
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
				var target = order.GetOrderByPaymentOrderId(
					StringUtility.ToEmpty(paymentOrderId),
					accessor);
				target.PaymentName = DomainFacade.Instance.PaymentService.Get(target.ShopId, target.OrderPaymentKbn).PaymentName;
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
				PaymentLogWrite(StringUtility.ToEmpty(this.Request.Form[RESULT_PUSH_TIME]), Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT, true, orderIdField);
			}
		}
		catch (Exception ex)
		{
			// ログ出力
			PaymentLogWrite(StringUtility.ToEmpty(this.Request.Form[RESULT_PUSH_TIME]), Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT, false, orderIdField);
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
	/// <param name="paymentDate">決済処理日時</param>
	/// <param name="paymentType">決済種別</param>
	/// <param name="success">決済に成功したかどうか</param>
	/// <param name="orderIdField">決済注文ID</param>
	/// <param name="vResultCode">詳細結果コード</param>
	private void PaymentLogWrite(string paymentDate, string paymentType, bool success, string orderIdField, string vResultCode = null)
	{
		DateTime orderPaymentDate;
		DateTime.TryParseExact(paymentDate, "yyyyMMddHHmmss", CultureInfo.CurrentCulture, DateTimeStyles.None, out orderPaymentDate);

		PaymentFileLogger.WritePaymentLog(
			success,
			paymentType,
			PaymentFileLogger.PaymentType.VeriTrans,
			PaymentFileLogger.PaymentProcessingType.GetOrderConfirmationNotice,
			success ? "結果通知受取" : "与信NG　詳細結果コード：" + StringUtility.ToEmpty(vResultCode),
			new Dictionary<string, string>
			{
				{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, StringUtility.ToEmpty(Request.Form[orderIdField]) },
				{ "date", orderPaymentDate.ToString("yyyy/MM/dd HH:mm:ss") },
			});
	}
}
