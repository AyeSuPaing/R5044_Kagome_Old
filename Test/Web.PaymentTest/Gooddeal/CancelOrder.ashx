<%@ WebHandler Language="C#" Class="CancelOrder" %>
/*
=========================================================================================================
  Module      : Cancel Order(CancelOrder.ashx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System.Web;

/// <summary>
/// Cancel order
/// </summary>
public class CancelOrder : IHttpHandler
{
	/// <summary>
	/// Process Request
	/// </summary>
	/// <param name="context"></param>
	public void ProcessRequest(HttpContext context)
	{
		context.Response.ContentType = "application/json; charset=utf-8";
		var data = new GooddealApi(context).ExecProcess("CancelOrder");
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