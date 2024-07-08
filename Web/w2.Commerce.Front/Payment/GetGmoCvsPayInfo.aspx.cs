/*
=========================================================================================================
  Module      : GetGmoCvsPayInfo(GetGmoCvsPayInfo.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.GMO;
using w2.Common.Logger;
using w2.Domain;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;

public partial class Payment_GetGmoCvsPayInfo : BasePage
{
	// Payment notification parameter
	private const string RESULT_ORDERID = "OrderId";		// Order id
	private const string RESULT_STATUS = "Status";			// Status
	private const string RESULT_PAYTYPE = "PayType";		// PayType
	private const string RESULT_FINISH_DATE = "FinishDate";	// Finish date
	private const string RESULT_TRAN_DATE = "TranDate";		// Finish date
	private const string RESULT_ACCESS_ID = "AccessID";		// Access id
	private const string RESULT_ACCESS_PASS = "AccessPass";	// Access pass
	private const string RESULT_ERRCODE = "ErrCode";		// Error code
	private const string RESULT_ERRINFO = "ErrInfo";		// Error info
	private const string PAY_TYPE_CREDIT = "0";				// クレジット
	private const string PAY_TYPE_CVS = "3";				// コンビニ前払い
	private const string PAY_TYPE_PAYPAY = "45";			// PayPay
	private const string FLG_RESULT_STATUS_SUCCESS = "PAYSUCCESS";	// Status cuccess

	/// <summary>
	/// Page load
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		var payType = StringUtility.ToEmpty(Request.Form[RESULT_PAYTYPE]);

		switch (payType)
		{
			// クレジットカード
			case PAY_TYPE_CREDIT:
				FileLogger.WriteDebug("クレジット通知");
				ProcessCreditCardOrder();
				break;

			// コンビニ
			case PAY_TYPE_CVS:
				FileLogger.WriteDebug("コンビニ前払い通知");
				ProcessCvsOrder();
				break;

			// PayPay
			case PAY_TYPE_PAYPAY:
				FileLogger.WriteDebug("PayPay通知");
				ProcessPayPayOrder();
				break;

			// 該当なしの場合
			default:
				FileLogger.WriteDebug("対応していない決済で通知が来ました:" + payType);
				Response.Write("0");
				Response.End();
				break;
		}
	}

	/// <summary>
	/// クレジットカード決済処理
	/// </summary>
	private void ProcessCreditCardOrder()
	{
		// ステータスが「即時売上」、「仮売上」以外
		if ((StringUtility.ToEmpty(Request.Form[RESULT_STATUS]) != "CAPTURE")
			&& (StringUtility.ToEmpty(Request.Form[RESULT_STATUS]) != "AUTH"))
		{
			Response.Write("0");
			Response.End();
		}

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
					StringUtility.ToEmpty(Request.Form[RESULT_ORDERID]),
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
				PaymentLogWrite(StringUtility.ToEmpty(this.Request.Form[RESULT_TRAN_DATE]), Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT, true);
			}
		}
		catch (Exception ex)
		{
			// ログ出力
			PaymentLogWrite(StringUtility.ToEmpty(this.Request.Form[RESULT_TRAN_DATE]), Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT, false);
			FileLogger.WriteError(ex);

			Response.Write("1");
			Response.StatusCode = 500;
			Response.End();
		}

		Response.Write("0");
		Response.End();
	}

	/// <summary>
	/// PayPay決済処理
	/// </summary>
	private void ProcessPayPayOrder()
	{
		// ステータスが「即時売上」、「仮売上」以外
		if ((StringUtility.ToEmpty(Request.Form[RESULT_STATUS]) != "CAPTURE")
			&& (StringUtility.ToEmpty(Request.Form[RESULT_STATUS]) != "AUTH"))
		{
			Response.Write("0");
			Response.End();
		}

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
					StringUtility.ToEmpty(Request.Form[RESULT_ORDERID]),
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
				PaymentLogWrite(StringUtility.ToEmpty(this.Request.Form[RESULT_TRAN_DATE]), Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY, true);
			}
		}
		catch (Exception ex)
		{
			// ログ出力
			PaymentLogWrite(StringUtility.ToEmpty(this.Request.Form[RESULT_TRAN_DATE]), Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY, false);
			FileLogger.WriteError(ex);

			Response.Write("1");
			Response.StatusCode = 500;
			Response.End();
		}

		Response.Write("0");
		Response.End();
	}

	/// <summary>
	/// コンビニ決済処理
	/// </summary>
	private void ProcessCvsOrder()
	{
		// catch句で利用したいためここで値を取得
		var payDate = StringUtility.ToEmpty(Request.Form[RESULT_FINISH_DATE]);
		var orderPaymentDate = DateTime.Parse(string.Format("{0}/{1}/{2} {3}:{4}:{5}", payDate.Substring(0, 4), payDate.Substring(4, 2), payDate.Substring(6, 2), payDate.Substring(8, 2), payDate.Substring(10, 2), payDate.Substring(12, 2)));

		try
		{
			if (StringUtility.ToEmpty(Request.Form[RESULT_STATUS]) != FLG_RESULT_STATUS_SUCCESS)
			{
				Response.Write("0");
			}
			else
			{
				var errorInfo = StringUtility.ToEmpty(Request.Form[RESULT_ERRINFO]);
				var errorCode = StringUtility.ToEmpty(Request.Form[RESULT_ERRCODE]);
				var orderPaymentId = StringUtility.ToEmpty(Request.Form[RESULT_ORDERID]);
				var response = new ResponseResult(Request.Url.GetComponents(UriComponents.Query, UriFormat.UriEscaped));
				response.Parameters[RESULT_ERRCODE] = Request.Form[RESULT_ERRCODE];

				// Get order by payment order id
				var order = new OrderService().GetOrderByPaymentOrderId(orderPaymentId);
				if (order == null) throw new ApplicationException("注文IDが存在しません");
				order.PaymentName = DomainFacade.Instance.PaymentService.Get(order.ShopId, order.OrderPaymentKbn).PaymentName;

				if (response.IsSuccess)
				{
					var updated = new OrderService().UpdatePaymentStatusForCvs(
						order.OrderId,
						Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE,
						orderPaymentDate,
						StringUtility.ToEmpty(Request.Form[RESULT_ACCESS_ID]) + " " + StringUtility.ToEmpty(Request.Form[RESULT_ACCESS_PASS]),
						Constants.FLG_LASTCHANGED_CGI,
						UpdateHistoryAction.Insert);

					if (updated == 0) throw new ApplicationException("更新に失敗しました");

					// Write log
					var log = new StringBuilder();
					log.Append("\t").Append("「入金通知成功」");
					log.Append("\t").Append(orderPaymentId);

					PaymentFileLogger.WritePaymentLog(
						true,
						Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE,
						PaymentFileLogger.PaymentType.Gmo,
						PaymentFileLogger.PaymentProcessingType.PaymentNotification,
						"入金通知成功",
						new Dictionary<string, string>
						{
							{Constants.FIELD_ORDER_ORDER_ID, orderPaymentId},
							{"date", orderPaymentDate.ToString()}
						});

					Response.Write("0");
				}
				else
				{
					// Log error
					var log = new StringBuilder();
					log.Append("\t").Append(errorCode);
					log.Append("\t").Append(orderPaymentId);
					log.Append("\t").Append(errorInfo);
					log.Append("\t").Append(response.GetErrorMessages(errorInfo));

					PaymentFileLogger.WritePaymentLog(
						false,
						Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE,
						PaymentFileLogger.PaymentType.Gmo,
						PaymentFileLogger.PaymentProcessingType.PaymentNotification,
						log.ToString(),
						new Dictionary<string, string>
						{
							{Constants.FIELD_ORDER_ORDER_ID, orderPaymentId},
							{"date", orderPaymentDate.ToString()}
						});

					if (order != null)
					{
						// Create log info message
						new PaymentGmoCvs().CreateLogInfoForPaymmentGmo("「入金通知」", order);
					}

					Response.Write("1");
				}
			}
		}
		catch (Exception ex)
		{
			FileLogger.WriteError(ex);

			Response.Write("1");

			// ログファイル格納処理
			PaymentFileLogger.WritePaymentLog(
				false,
				Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE,
				PaymentFileLogger.PaymentType.Gmo,
				PaymentFileLogger.PaymentProcessingType.PaymentNotification,
				"",
				new Dictionary<string, string>
				{
					{Constants.FIELD_ORDER_ORDER_ID, StringUtility.ToEmpty(Request.Form[RESULT_ORDERID])},
					{"date", orderPaymentDate.ToString()}
				});
			Response.StatusCode = 500;
		}

		// HTTP response output
		Response.End();
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
	/// <returns></returns>
	private bool ExecOrderComplete(OrderRegisterFront orderRegister,　CartObject cart, Hashtable orderInfo, SqlAccessor accessor)
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
	private void PaymentLogWrite(string paymentDate, string paymentType, bool success)
	{
		DateTime orderPaymentDate;
		DateTime.TryParseExact(paymentDate, "yyyyMMddHHmmss", CultureInfo.CurrentCulture, DateTimeStyles.None, out orderPaymentDate);
		
		PaymentFileLogger.WritePaymentLog(
			success,
			paymentType,
			PaymentFileLogger.PaymentType.Gmo,
			PaymentFileLogger.PaymentProcessingType.GetOrderConfirmationNotice,
			"結果通知受取",
			new Dictionary<string, string>
			{
				{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, StringUtility.ToEmpty(Request.Form[RESULT_ORDERID]) },
				{ "date", orderPaymentDate.ToString("yyyy/MM/dd HH:mm:ss") },
			});
	}
}
