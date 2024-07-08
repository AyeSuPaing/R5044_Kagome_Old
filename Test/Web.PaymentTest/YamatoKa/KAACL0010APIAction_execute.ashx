<%@ WebHandler Language="C#" Class="KAACL0010APIAction_execute" %>

using System;
using System.Web;

public class KAACL0010APIAction_execute : IHttpHandler
{

	public void ProcessRequest(HttpContext context)
	{
		context.Response.ContentType = "text/plain";

		string responseXml = new YamatoKaApiReceiverFacade().Receive(context.Request, "KAACL0010");
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