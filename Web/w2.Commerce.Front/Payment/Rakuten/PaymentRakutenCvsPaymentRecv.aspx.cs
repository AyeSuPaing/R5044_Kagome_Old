/*
=========================================================================================================
  Module      : Payment Rakuten Cvs Payment Receive (PaymentRakutenCvsPaymentRecv.aspx.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.Rakuten;
using w2.App.Common.Order.Payment.Rakuten.Notification;
using w2.Domain;
using w2.Domain.UpdateHistory.Helper;

/// <summary>
/// Payment rakuten cvs payment recv
/// </summary>
public partial class Payment_Rakuten_PaymentRakutenCvsPaymentRecv : System.Web.UI.Page
{
	/// <summary>
	/// Page load
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		InitComponents();
		var responseString = ExecNotificationForPaymentResult();
		Response.Write(responseString);
	}

	/// <summary>
	/// Exec notification for payment result
	/// </summary>
	/// <returns>Result exec notification</returns>
	private string ExecNotificationForPaymentResult()
	{
		var isSuccess = false;
		try
		{
			switch (this.RakutenRequest.RequestType)
			{
				case RakutenConstants.REQUEST_TYPE_CAPTURE:
					var updated = UpdateOrderCapture();
					if (updated == 0)
					{
						throw new Exception("入金済に更新できませんでした。:" + this.RakutenRequest.ServiceReferenceId);
					}

					isSuccess = true;
					break;

				case RakutenConstants.REQUEST_TYPE_AUTHORIZE:
				case RakutenConstants.REQUEST_TYPE_CANCEL_OR_REFUND:
					isSuccess = true;
					break;

				default:
					isSuccess = false;
					break;
			}

			WritePaymentLog(isSuccess, string.Empty);
		}
		catch (Exception ex)
		{
			WritePaymentLog(false, ex.Message);
		}

		return CreateRakutenNotificationResponse(isSuccess);
	}

	/// <summary>
	/// Update order capture
	/// </summary>
	/// <returns>Number of order updated</returns>
	private int UpdateOrderCapture()
	{
		var updated = 0;
		var order = DomainFacade.Instance.OrderService.GetOrderByPaymentOrderId(
			this.RakutenRequest.PaymentId);

		if (order == null) return updated;

		if ((this.RakutenRequest.ResultType == RakutenConstants.RESULT_TYPE_SUCCESS)
			&& (order.OrderPaymentStatus == Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM))
		{
			updated = DomainFacade.Instance.OrderService.UpdatePaymentStatusForCvs(
				order.OrderId,
				Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE,
				DateTime.Parse(this.RakutenRequest.TransactionTime),
				order.CardTranId,
				Constants.FLG_LASTCHANGED_CGI,
				UpdateHistoryAction.Insert);
		}

		return updated;
	}

	/// <summary>
	/// Init components
	/// </summary>
	private void InitComponents()
	{
		var paymentInfo = StringUtility.ToEmpty(
			Request.Form[RakutenConstants.HTTP_PARAMETER_PAYMENT_INFO]);
		this.RakutenRequest = JsonConvert
			.DeserializeObject<RakutenNotificationRequest>(Base64UrlDecode(paymentInfo));
	}

	/// <summary>
	/// Create rakuten notification response
	/// </summary>
	/// <param name="isSuccess">Is success</param>
	/// <returns>String rakuten notification response</returns>
	private string CreateRakutenNotificationResponse(bool isSuccess)
	{
		var response = new RakutenResponseBase
		{
			ResultType = isSuccess
				? RakutenConstants.RESULT_TYPE_SUCCESS
				: RakutenConstants.RESULT_TYPE_FAILURE,
		};

		var result = JsonConvert.SerializeObject(
			response,
			new JsonSerializerSettings
			{
				Formatting = Formatting.Indented,
				NullValueHandling = NullValueHandling.Ignore,
			});

		return result;
	}

	/// <summary>
	/// Write payment log
	/// </summary>
	/// <param name="isSuccess">Is success</param>
	/// <param name="externalPaymentCooperationLog">External payment cooperation log</param>
	private void WritePaymentLog(
		bool? isSuccess,
		string externalPaymentCooperationLog)
	{
		PaymentFileLogger.WritePaymentLog(
			isSuccess,
			Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE,
			PaymentFileLogger.PaymentType.Rakuten,
			PaymentFileLogger.PaymentProcessingType.PaymentStatusUpForWebCvs,
			externalPaymentCooperationLog,
			new Dictionary<string, string>
			{
				{ Constants.FIELD_ORDER_ORDER_ID, this.RakutenRequest.ServiceReferenceId },
			});
	}

	/// <summary>
	/// Base64 url decode
	/// </summary>
	/// <param name="paymentInfo">Payment info</param>
	/// <returns>Converted data</returns>
	private string Base64UrlDecode(string paymentInfo)
	{
		var output = paymentInfo
			.Replace('-', '+')
			.Replace('_', '/');

		switch (output.Length % 4)
		{
			case 0:
				break;

			case 2:
				output += "==";
				break;

			case 3:
				output += "=";
				break;

			default:
				throw new Exception("Illegal base64url string!");
		}

		var converted = Encoding.UTF8.GetString(Convert.FromBase64String(output));
		return converted;
	}

	/// <summary>Rakuten request</summary>
	private RakutenNotificationRequest RakutenRequest { get; set; }
}
