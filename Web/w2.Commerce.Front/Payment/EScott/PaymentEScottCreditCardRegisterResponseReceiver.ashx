<%--
=========================================================================================================
  Module      : e-SCOTTクレジット登録レスポンス取得ハンドラ(PaymentEScottCreditCardRegisterResponseReceiver.ashx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>
<%@ WebHandler Language="C#" Class="PaymentEScottCreditCardRegisterResponseReceiver" %>
using System;
using System.Web;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment;
using w2.Common.Logger;

public class PaymentEScottCreditCardRegisterResponseReceiver : IHttpHandler
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

		context.Response.ContentType = "application/x-www-form-urlencoded";

		bool result;
		try
		{
			result = new ProvisionalCreditCardProcessor().UpdateProvisionalCreditCardRegisterd(context, Constants.FLG_LASTCHANGED_CGI);
			PaymentFileLogger.WritePaymentLog(
				result,
				Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
				PaymentFileLogger.PaymentType.EScott,
				PaymentFileLogger.PaymentProcessingType.CreditCardRegistResponse,
				string.Empty);
		}
		catch (Exception ex)
		{
			PaymentFileLogger.WritePaymentLog(
				false,
				Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
				PaymentFileLogger.PaymentType.EScott,
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