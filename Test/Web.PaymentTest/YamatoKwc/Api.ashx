<%@ WebHandler Language="C#" Class="Api" %>

using System;
using System.Web;
using w2.App.Common.Order.Payment.YamatoKwc.Helper;

public class Api : IHttpHandler {

    /// <summary>
    /// リクエスト処理
    /// </summary>
    /// <param name="context"></param>
	public void ProcessRequest(HttpContext context)
	{
		context.Response.ContentType = "text/plain";

        PaymentYamatoKwcFunctionDiv funtionDiv;
        //if (Enum.TryParse(context.Request["function_div"], out funtionDiv) == false)
        //{
        //    context.Response.Write("エラー 機能区分：" + context.Request["function_div"]);
        //    return;
        //}

	    string responseXml = new YamatoKwcApiReceiverFacade().Receive(context.Request);
		context.Response.Write(responseXml);
	}

	public bool IsReusable
	{
		get { return false; }
	}

}