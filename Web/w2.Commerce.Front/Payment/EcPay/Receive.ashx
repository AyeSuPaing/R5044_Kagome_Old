<%--
=========================================================================================================
  Module      : Receive(Receive.ashx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
--%>
<%@ WebHandler Language="C#" Class="Receive" %>
using System;
using System.Web;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment;
using w2.Common.Logger;
using w2.Domain.Order;
using w2.App.Common.Order.Payment.ECPay;
using w2.Domain.UpdateHistory.Helper;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Mail;
using w2.App.Common;

public class Receive : IHttpHandler
{
	/// <summary>
	/// プロセスリクエスト
	/// </summary>
	/// <param name="context"></param>
	public void ProcessRequest(HttpContext context)
	{
		context.Response.ContentType = "text/plain";
		this.Request = context.Request;
		var result = false;
		try
		{
			result = UpdateOrder();
		}
		catch (Exception ex)
		{
			PaymentFileLogger.WritePaymentLog(
				false,
				string.Empty,
				PaymentFileLogger.PaymentType.EcPay,
				PaymentFileLogger.PaymentProcessingType.GetResponse,
				BaseLogger.CreateExceptionMessage(ex));
			result = false;
		}

		if (result == false)
		{
			context.Response.StatusCode = 500;
		}
		else
		{
			PaymentFileLogger.WritePaymentLog(
				true,
				string.Empty,
				PaymentFileLogger.PaymentType.EcPay,
				PaymentFileLogger.PaymentProcessingType.GetResponse,
				string.Empty);

			// Notice to EC Pay
			context.Response.Write("1|OK");
		}
	}

	/// <summary>
	/// Update Order
	/// </summary>
	/// <returns>True: if update success</returns>
	public bool UpdateOrder()
	{
		var response = ECPayApiFacade.GetResponseFromRequest(this.Request);
		var result = response.IsSuccess;
		if (result == false) return false;
		var orderInfo = new OrderService().Get(response.MerchantTradeNo);
		if (orderInfo == null) return false;

		switch (this.Request[Constants.REQUEST_KEY_NO])
		{
			case ECPayConstants.CONST_RETURN_URL_PARAMETER_NO:
				result = UpdateOrderWhenCallReturnApi(response, orderInfo);
				break;

			case ECPayConstants.CONST_PAYMENT_URL_PARAMETER_NO:
				result = UpdateOrderWhenCallPaymentApi(response, orderInfo);
				break;
		}
		// Send Mail
		if (result)
		{
			SendMails(orderInfo);
		}

		// Export Log
		var message = string.Format(
			"OrderID:{0}\r\nTradeNo:{1}\r\nRtnCode:{2}\r\nRtnMsg:{3}",
			response.MerchantTradeNo,
			response.TradeNo,
			response.ReturnCode,
			response.ReturnMessage);
		ECPayUtility.ExportLog(
			message,
			response.MerchantTradeNo,
			orderInfo.PaymentOrderId,
			result,
			orderInfo.OrderPaymentKbn);
		return result;
	}

	/// <summary>
	/// Update Order When Call Payment Api
	/// </summary>
	/// <param name="response">Response</param>
	/// <param name="order">Order</param>
	/// <returns>True: if update success</returns>
	private static bool UpdateOrderWhenCallReturnApi(ECPayResponse response, OrderModel order)
	{
		if (order.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_TEMP)
			order.OrderStatus = Constants.FLG_ORDER_ORDER_STATUS_ORDERED;
		order.CardTranId = response.TradeNo;
		if (response.IsPaymentTypeCreditCard)
		{
			order.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
		}
		else
		{
			DateTime orderPaymentDate;
			if (DateTime.TryParse(response.PaymentDate, out orderPaymentDate))
			{
				order.OrderPaymentDate = orderPaymentDate;
			}
			order.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP;
			order.OrderPaymentStatus = Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE;
		}

		var sendingAmount = CurrencyManager.GetSendingAmount(
			order.OrderPriceTotal,
			order.SettlementAmount,
			order.SettlementCurrency);
		order.PaymentMemo = ECPayUtility.CreatePaymentMemo(order, "与信", sendingAmount);
		var update = new OrderService().Modify(
			order.OrderId,
			model =>
			{
				model.OrderStatus = order.OrderStatus;
				model.CardTranId = order.CardTranId;
				model.ExternalPaymentStatus = order.ExternalPaymentStatus;
				model.OrderPaymentDate = order.OrderPaymentDate;
				model.OrderPaymentStatus = order.OrderPaymentStatus;
				model.PaymentMemo = order.PaymentMemo;
			},
			UpdateHistoryAction.Insert);

		// ECPAYで入金後、ECPAY電子発票発行される
		if ((update > 0) && Constants.TWINVOICE_ECPAY_ENABLED)
		{
			for (var i = 0; i < order.Shippings.Length; i++)
			{
				var cartShipping = new CartShipping(null)
				{
					ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_TW,
					UniformInvoiceType = order.Invoices[i].TwUniformInvoice,
					CarryType = order.Invoices[i].TwCarryType
				};
				var errorMessage = OrderCommon.EcPayInvoiceReleased(
					w2.Common.Util.StringUtility.ToEmpty(order.OrderId),
					cartShipping,
					i + 1,
					w2.Common.Util.StringUtility.ToEmpty(order.LastChanged));
				if (string.IsNullOrEmpty(errorMessage) == false)
				{
					FileLogger.WriteError(errorMessage);
				}
			}
		}
		return (update > 0);
	}

	/// <summary>
	/// Update Order When Call Payment Api
	/// </summary>
	/// <param name="response">Response</param>
	/// <param name="order">Order</param>
	/// <returns>True: if update success</returns>
	public bool UpdateOrderWhenCallPaymentApi(ECPayResponse response, OrderModel order)
	{
		if (order.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_TEMP)
			order.OrderStatus = Constants.FLG_ORDER_ORDER_STATUS_ORDERED;
		order.CardTranId = response.TradeNo;

		var sendingAmount = CurrencyManager.GetSendingAmount(
			order.OrderPriceTotal,
			order.SettlementAmount,
			order.SettlementCurrency);
		var paymentMemo = string.Format(
			"与信({0})：{1}",
			order.ExternalPaymentType,
			response.PaymentMemo);
		order.PaymentMemo = ECPayUtility.CreatePaymentMemo(order, paymentMemo, sendingAmount);
		var updateOrder = new OrderService().Modify(
			order.OrderId,
			model =>
			{
				model.OrderStatus = order.OrderStatus;
				model.CardTranId = order.CardTranId;
				model.PaymentMemo = order.PaymentMemo;
			},
			UpdateHistoryAction.Insert);
		
		return (updateOrder > 0);
	}

	/// <summary>
	/// Send Mails
	/// </summary>
	/// <param name="orderInfo">Order Info</param>
	private void SendMails(OrderModel order)
	{
		var transactionName = string.Empty;
		try
		{
			var data = new MailTemplateDataCreaterForOrder(true).GetOrderMailDatas(order.OrderId);
			var languageCode = string.Empty;
			var languageLocaleId = string.Empty;

			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				languageCode = StringUtility.ToEmpty(data[Constants.FIELD_USER_DISP_LANGUAGE_CODE]);
				languageLocaleId = StringUtility.ToEmpty(data[Constants.FIELD_USER_DISP_LANGUAGE_LOCALE_ID]);
			}
			var isPc = (bool)data["is_pc"];
			var mailAddr = (isPc
				? order.Owner.OwnerMailAddr
				: order.Owner.OwnerMailAddr2);

			data[Constants.FIELD_ORDER_MEMO] = ((string)data[Constants.FIELD_ORDER_MEMO]).Trim();

			using (MailSendUtility mailSender = new MailSendUtility(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.CONST_MAIL_ID_ORDER_COMPLETE,
				order.UserId,
				data,
				isPc,
				Constants.MailSendMethod.Auto,
				languageCode,
				languageLocaleId,
                StringUtility.ToEmpty(mailAddr)))
			{
				mailSender.AddTo(StringUtility.ToEmpty(mailAddr));
				if (mailSender.SendMail() == false)
					throw new Exception("メール送信処理に失敗しました。", mailSender.MailSendException);
			}

			if (Constants.THANKSMAIL_FOR_OPERATOR_ENABLED)
			{
				transactionName = "4-2-2.メール送信処理(管理者向け)";

				using (MailSendUtility mailSender = new MailSendUtility(
					Constants.CONST_DEFAULT_SHOP_ID,
					Constants.CONST_MAIL_ID_ORDER_COMPLETE_FOR_OPERATOR,
					string.Empty,
					data,
					isPc,
					Constants.MailSendMethod.Auto))
				{
					if (string.IsNullOrEmpty(mailSender.TmpTo) == false)
					{
						if (mailSender.SendMail() == false)
							throw new Exception("メール送信処理に失敗しました。", mailSender.MailSendException);
					}
				}
			}
		}
		catch (Exception ex)
		{
			var errorMessage = string.Format(
				"注文成功→{0}失敗：[user_id={1},order_id={2},owner_kbn={3},cart_id={4},shop_id={5}]",
				transactionName,
				order.UserId,
				order.OrderId,
				order.Owner.OwnerKbn,
				this.Request[Constants.REQUEST_KEY_CART_ID],
				order.ShopId);
			AppLogger.WriteWarn(errorMessage, ex);
		}
	}

	/// <summary>Request</summary>
	private HttpRequest Request { get; set; }
	/// <summary>Is Reusable</summary>
	public bool IsReusable { get { return false; } }
}
