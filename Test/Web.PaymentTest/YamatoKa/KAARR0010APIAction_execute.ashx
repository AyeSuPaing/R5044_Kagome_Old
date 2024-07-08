<%@ WebHandler Language="C#" Class="KAARR0010APIAction_execute" %>

using System;
using System.Web;

public class KAARR0010APIAction_execute : IHttpHandler
{

	public void ProcessRequest(HttpContext context)
	{
		context.Response.ContentType = "text/plain";

		string responseXml = new YamatoKaApiReceiverFacade().Receive(context.Request, "KAARR0010");
		context.Response.Write(responseXml);
	}

	public bool IsReusable
	{
		get
		{
			return false;
		}
	}

}