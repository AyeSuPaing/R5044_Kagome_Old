<%--
=========================================================================================================
  Module      : GMOクレジット登録レスポンス取得ハンドラ(PaymentGmoCreditCardRegisterResponseReceiver.ashx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
--%>
<%@ WebHandler Language="C#" Class="PaymentGmoCreditCardRegisterResponseReceiver" %>
using System;
using System.Collections.Specialized;
using System.Web;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment;
using w2.Common.Logger;

public class PaymentGmoCreditCardRegisterResponseReceiver : IHttpHandler
{
	/// <summary>レスポンス：成功</summary>
	private const string RESPONSE_SUCCESS = "0";
	/// <summary>レスポンス：エラー</summary>
	private const string RESPONSE_ERROR = "1";

	/// <summary>
	/// プロセスリクエスト
	/// </summary>
	/// <param name="context"></param>
	public void ProcessRequest(HttpContext context)
	{
		// メモ
		// タイムアウトは15秒。リトライは約60分毎に5回。

		context.Response.ContentType = "text/plain";

		bool result;
		try
		{
			result = new ProvisionalCreditCardProcessor().UpdateProvisionalCreditCardRegisterd(context, Constants.FLG_LASTCHANGED_CGI);
			PaymentFileLogger.WritePaymentLog(
				result,
				Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
				PaymentFileLogger.PaymentType.Gmo,
				PaymentFileLogger.PaymentProcessingType.CreditCardRegistResponse,
				"");
		}
		catch (Exception ex)
		{
			PaymentFileLogger.WritePaymentLog(
				false,
				Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
				PaymentFileLogger.PaymentType.Gmo,
				PaymentFileLogger.PaymentProcessingType.CreditCardRegistResponse,
				BaseLogger.CreateExceptionMessage(ex));
			result = false;
		}
		context.Response.Write(result ? RESPONSE_SUCCESS : RESPONSE_ERROR);
	}

	/// <summary>再利用可能か</summary>
	public bool IsReusable
	{
		get { return false; }
	}
}