<%@ WebHandler Language="C#" Class="UploadOrder" %>
/*
=========================================================================================================
  Module      : Upload Order(UploadOrder.ashx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System.Web;

/// <summary>
/// Upload order
/// </summary>
public class UploadOrder : IHttpHandler
{
	/// <summary>
	/// Process Request
	/// </summary>
	/// <param name="context"></param>
	public void ProcessRequest(HttpContext context)
	{
		context.Response.ContentType = "application/json; charset=utf-8";
		var data = new GooddealApi(context).ExecProcess("UploadOrder");
		context.Response.Write(data);
	}

	/// <summary>Is reusable</summary>
	public bool IsReusable
	{
		get
		{
			return false;
		}
	}
}