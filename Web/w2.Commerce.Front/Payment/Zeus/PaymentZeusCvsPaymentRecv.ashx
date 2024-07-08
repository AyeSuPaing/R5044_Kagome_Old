<%--
=========================================================================================================
  Module      : Payment Zeus Cvs Payment Receiver (PaymentZeusCvsPaymentRecv.ashx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
--%>
<%@ WebHandler Language="C#" Class="PaymentZeusCvsPaymentRecv" %>

using System;
using System.Web;
using w2.App.Common.Order.Payment.Zeus;

/// <summary>
/// Payment Zeus Cvs Payment Receiver
/// </summary>
public class PaymentZeusCvsPaymentRecv : IHttpHandler
{
	/// <summary>
	/// リクエスト処理
	/// </summary>
	/// <param name="context">Context</param>
	public void ProcessRequest(HttpContext context)
	{
		context.Response.ContentType = "text/plain";

		var result = new PaymentZeusCvsResultReceiver(context.Request).Exec();

		context.Response.Write(result ? "OK" : "NG");
	}

	/// <summary>再利用可能か</summary>
	public bool IsReusable
	{
		get { return false; }
	}
}