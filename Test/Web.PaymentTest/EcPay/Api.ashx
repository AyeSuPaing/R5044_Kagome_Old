<%@ WebHandler Language="C#" Class="Api" %>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Web;
using w2.App.Common.Order.Payment.ECPay;

public class Api : IHttpHandler
{
	/// <summary>
	/// リクエスト処理
	/// </summary>
	/// <param name="context">Context</param>
	public void ProcessRequest(HttpContext context)
	{
		if (context.Request.HttpMethod != "POST") return;
		context.Response.ContentType = "text/plain";
		var requestObject = ECPayApiFacade.Receive(context.Request);

		var xdocPath = Path.Combine(
			AppDomain.CurrentDomain.BaseDirectory,
			"Xml",
			"EcPay",
			"EcPayApi.xml");
		var xdoc = XDocument.Load(xdocPath);
		var returnCode = xdoc.Root.Elements("RtnCode").First().Value;
		var returnMesage = xdoc.Root.Elements("RtnMsg").First().Value;

		var actions = new string[] { "C", "N", "R" };
		var isOK = (returnCode == "1")
			&& actions.Contains(requestObject.Action);
		var parameters = new List<KeyValuePair<string, string>>
		{
			new KeyValuePair<string, string>("MerchantID", requestObject.MerchantId),
			new KeyValuePair<string, string>("MerchantTradeNo", requestObject.MerchantTradeNo),
			new KeyValuePair<string, string>("TradeNo", requestObject.TradeNo + (isOK ? "OK" : "NG")),
			new KeyValuePair<string, string>("RtnCode", returnCode),
			new KeyValuePair<string, string>("RtnMsg", isOK ? string.Empty : returnMesage)
		};

		var requestString = CreateParameters(parameters);
		context.Response.Write(requestString);
	}

	/// <summary>
	/// Create parameters
	/// </summary>
	/// <param name="parameters">List key value parameter</param>
	/// <returns>Parameters</returns>
	public static string CreateParameters(List<KeyValuePair<string, string>> parameters)
	{
		return string.Join("&", parameters.Select(param => string.Format("{0}={1}", param.Key, param.Value)));
	}

	/// <summary>Is Reusable</summary>
	public bool IsReusable
	{
		get { return false; }
	}
}