<%--
=========================================================================================================
  Module      : Receive(NewebPayReceive.ashx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
--%>
<%@ WebHandler Language="C#" Class="Receive" %>
using System;
using System.IO;
using System.Web;
using System.Web.Services;
using w2.App.Common.Order.Payment;
using w2.Common.Logger;
using w2.App.Common.Order.Payment.NewebPay;
using w2.Domain.Order;
using w2.App.Common.Global.Region.Currency;
using w2.Domain.UpdateHistory.Helper;
using w2.App.Common.Mail;
using w2.App.Common;
using w2.Common.Web;
using w2.App.Common.Order;

/// <summary>
/// Receive
/// </summary>
public class Receive : IHttpHandler
{
	/// <summary>
	/// Process Request
	/// </summary>
	/// <param name="context">Http Context</param>
	[WebMethod]
	public void ProcessRequest(HttpContext context)
	{
		context.Response.ContentType = "text/html";
		this.Request = context.Request;

		var result = false;
		try
		{
			result = UpdateOrder();
		}
		catch (Exception ex)
		{
			result = false;
			PaymentFileLogger.WritePaymentLog(
				result,
				string.Empty,
				PaymentFileLogger.PaymentType.NewebPay,
				PaymentFileLogger.PaymentProcessingType.GetResponse,
				BaseLogger.CreateExceptionMessage(ex));
		}

		if (result == false)
		{
			context.Response.StatusCode = 500;
		}
		else
		{
			var baseUrl = string.Format(
				"{0}{1}{2}",
				Constants.PROTOCOL_HTTPS,
				string.IsNullOrEmpty(Constants.WEBHOOK_DOMAIN)
					? Constants.SITE_DOMAIN
					: Constants.WEBHOOK_DOMAIN,
				Constants.PATH_ROOT_FRONT_PC);
			var paramTemp = string.Join(",", NewebPayConstants.CONST_RETURN_URL, this.CartId);
			var url = new UrlCreator(baseUrl + Constants.PAGE_FRONT_PAYMENT_NEWEBPAY_ORDER_RESULT)
				.AddParam(Constants.REQUEST_KEY_NO, paramTemp)
				.AddParam(NewebPayConstants.PARAM_STATUS, this.Status)
				.AddParam(NewebPayConstants.PARAM_MESSAGE, this.Message)
				.CreateUrl();
			context.Response.Redirect(url);
		}
	}

	/// <summary>
	/// Update Order
	/// </summary>
	/// <returns>True: If Update Success</returns>
	public bool UpdateOrder()
	{
		var response = NewebPayApiFacade.GetResponseFromRequest(this.Request);
		var result = (response.Status
			== NewebPayConstants.CONST_STATUS_SUCCESS);
		if (result == false) return false;

		this.Status = response.Status;
		this.Message = response.Message;

		var orderInfo = new OrderService().Get(response.Result.MerchantOrderNo);
		if (orderInfo == null) return false;

		if (CheckOneTimeUpdate(orderInfo)) return true;

		orderInfo.OrderStatus = Constants.FLG_ORDER_ORDER_STATUS_ORDERED;
		if (string.IsNullOrEmpty(response.Result.StoreCode) == false)
		{
			for (var index = 0; index < orderInfo.Shippings.Length; index++)
			{
				orderInfo.Shippings[index].ShippingReceivingStoreId = response.Result.StoreCode;
				orderInfo.Shippings[index].ShippingName = response.Result.StoreName;
				orderInfo.Shippings[index].ShippingAddr4 = response.Result.StoreAddr;
				orderInfo.Shippings[index].ShippingReceivingStoreFlg = Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON;
				orderInfo.Shippings[index].ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_TW;
				orderInfo.Shippings[index].ShippingCountryName = "Taiwan";
			}
		}

		var paramNo =
			StringUtility.ToEmpty(this.Request[Constants.REQUEST_KEY_NO]).Split(',');

		this.CartId = paramNo[1];

		switch (paramNo[0])
		{
			case NewebPayConstants.CONST_NOTIFY_URL:
				result = UpdateOrderWhenCallNotifyUrl(response, orderInfo);
				break;

			case NewebPayConstants.CONST_CUSTOMER_URL:
				result = UpdateOrderWhenCallCustomerUrl(response, orderInfo);
				break;
		}

		// Send Mail
		if (result)
		{
			SendMails(orderInfo);
		}

		var jsonResult = NewebPayApiFacade.CreateResponseData(response);
		NewebPayApiFacade.WriteNewebPayLog(jsonResult);

		if (response.IsCredit
			|| (CheckOneTimeUpdate(orderInfo) == false))
		{
			// Export Log
			var messageForLog = string.Format(
				"OrderID:{0}\r\nTradeNo:{1}\r\nStatus:{2}\r\nMessage:{3}",
				response.Result.MerchantOrderNo,
				response.Result.TradeNo,
				response.Status,
				response.Message);
			NewebPayUtility.ExportLog(
				messageForLog,
				response.Result.MerchantOrderNo,
				orderInfo.PaymentOrderId,
				result,
				orderInfo.OrderPaymentKbn);
		}
		return result;
	}

	/// <summary>
	/// Update Order When Api Return No Is 1
	/// </summary>
	/// <param name="response">NewebPay Response</param>
	/// <param name="order">Order</param>
	/// <returns>True: If Update Success</returns>
	private static bool UpdateOrderWhenCallNotifyUrl(NewebPayResponse response, OrderModel order)
	{
		order.CardTranId = response.Result.TradeNo;
		if (response.IsCredit)
		{
			order.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
		}
		else
		{
			DateTime orderPaymentDate;
			if (DateTime.TryParse(response.Result.PayTime, out orderPaymentDate))
			{
				order.OrderPaymentDate = orderPaymentDate;
			}
			order.ExternalPaymentStatus = (response.IsCvsCom)
				? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_NONE
				: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP;
			order.OrderPaymentStatus = (response.IsCvsCom)
				? Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM
				: Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE;
		}

		var paymentMemo = string.Format(
			"与信 {0}",
			response.PaymentMemo);
		order.PaymentMemo = NewebPayUtility.CreatePaymentMemo(
			order,
			paymentMemo,
			order.SettlementAmount,
			CheckOneTimeUpdate(order));
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
				model.ExternalPaymentAuthDate = order.ExternalPaymentAuthDate;
			},
			UpdateHistoryAction.Insert);

		// Update order shipping
		using (var accessor = new SqlAccessor())
		{
			accessor.OpenConnection();
			accessor.BeginTransaction();

			var updateOrderShipping = new OrderService().UpdateOrderShipping(order.Shippings[0], accessor);
			accessor.CommitTransaction();
		}

		// NewebPayで入金後、ECPAY電子発票発行される
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
	/// Update Order When Api Return No Is 2
	/// </summary>
	/// <param name="response">NewebPay Response</param>
	/// <param name="order">Order</param>
	/// <returns>True: If Update Success</returns>
	public bool UpdateOrderWhenCallCustomerUrl(NewebPayResponse response, OrderModel order)
	{
		order.CardTranId = response.Result.TradeNo;

		var paymentMemo = (response.IsCvsCom)
			? string.Format(
				"与信 {0}",
				response.PaymentMemo)
			: string.Format(
				"与信({0})：{1}",
				order.ExternalPaymentType,
				response.PaymentMemo);
		order.PaymentMemo = NewebPayUtility.CreatePaymentMemo(
			order,
			paymentMemo,
			order.SettlementAmount);

		var updateOrder = new OrderService().Modify(
			order.OrderId,
			model =>
			{
				model.OrderStatus = order.OrderStatus;
				model.CardTranId = order.CardTranId;
				model.PaymentMemo = order.PaymentMemo;
			},
			UpdateHistoryAction.Insert);

		// Update order shipping
		using (var accessor = new SqlAccessor())
		{
			accessor.OpenConnection();
			accessor.BeginTransaction();

			var updateOrderShipping = new OrderService().UpdateOrderShipping(order.Shippings[0], accessor);
			accessor.CommitTransaction();
		}
		return (updateOrder > 0);
	}

	/// <summary>
	/// Send Mails
	/// </summary>
	/// <param name="order">Order Info</param>
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
			data[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4] = ((string)data[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4]).Trim();

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

	/// <summary>
	/// Check one time update
	/// </summary>
	/// <param name="orderInfo">Order</param>
	/// <returns>True: If match condition, otherwise: false</returns>
	private static bool CheckOneTimeUpdate(OrderModel orderInfo)
	{
		var result = ((orderInfo.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_TEMP)
			&& ((orderInfo.ExternalPaymentStatus != Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_NONE)
				|| (orderInfo.ExternalPaymentType == Constants.FLG_PAYMENT_TYPE_NEWEBPAY_CVSCOM)));
		return result;
	}

	/// <summary>Request</summary>
	private HttpRequest Request { get; set; }
	/// <summary>Status</summary>
	public string Status { get; set; }
	/// <summary>Cart Id</summary>
	public string CartId { get; set; }
	/// <summary>Message</summary>
	public string Message { get; set; }
	/// <summary>Is Reusable</summary>
	public bool IsReusable { get { return false; } }
}
