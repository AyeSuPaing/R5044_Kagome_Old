<%@ WebHandler Language="C#" Class="RakutenPaymentAuthorize" %>
/*
=========================================================================================================
  Module      : Rakuten Payment Authorize (RakutenPaymentAuthorize.ashx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Web;

/// <summary>
/// Rakuten payment authorize
/// </summary>
public class RakutenPaymentAuthorize : IHttpHandler
{
	/// <summary>
	/// Process request
	/// </summary>
	/// <param name="context">Context</param>
	public void ProcessRequest(HttpContext context)
	{
		context.Response.ContentType = "application/json; charset=utf-8";
		var data = new RakutenCvsApi(context, "Authorize")
			.ExecProcess();
		context.Response.Write(data);
	}

	/// <summary>Is reusable</summary>
	public bool IsReusable
	{
		get { return false; }
	}
}
