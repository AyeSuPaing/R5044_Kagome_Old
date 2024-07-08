<%--
=========================================================================================================
  Module      : ヤマトKWCレスポンス取得ハンドラ(PaymentYamatoKwcResponseReceiver.ashx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
--%>
<%@ WebHandler Language="C#" Class="PaymentYamatoKwcCreditCardRegisterResponseReceiver" %>
using System;
using System.Web;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment;
using w2.Common.Logger;

public class PaymentYamatoKwcCreditCardRegisterResponseReceiver : IHttpHandler
{
	/// <summary>
	/// プロセスリクエスト
	/// </summary>
	/// <param name="context"></param>
	public void ProcessRequest(HttpContext context)
	{
		// メモ
		// タイムアウトは15秒。1回目のPOST送信でエラーだった場合は、1秒待機後、リトライを4回実施。（計５回）
		// 正常に処理できた場合は、200番台か、300番台のレスポンスを返すようにしてください。
		// それ以外（例えば500番台）が返ってきた場合に、リトライ対象となります。

		context.Response.ContentType = "text/plain";

		bool result;
		try
		{
			result = new ProvisionalCreditCardProcessor().UpdateProvisionalCreditCardRegisterd(context, Constants.FLG_LASTCHANGED_CGI);
		}
		catch (Exception ex)
		{
			PaymentFileLogger.WritePaymentLog(
				false,
				"",
				PaymentFileLogger.PaymentType.Yamatokwc,
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
				"",
				PaymentFileLogger.PaymentType.Yamatokwc,
				PaymentFileLogger.PaymentProcessingType.GetResponse,
				"");
		}
	}

	/// <summary>再利用可能か</summary>
	public bool IsReusable
	{
		get { return false; }
	}
}