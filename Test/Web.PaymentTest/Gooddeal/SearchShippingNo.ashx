<%@ WebHandler Language="C#" Class="SearchShippingNo" %>
/*
=========================================================================================================
  Module      : Search Shipping No (SearchShippingNo.ashx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System.Web;

/// <summary>
/// Search shipping no
/// </summary>
public class SearchShippingNo : IHttpHandler
{
	/// <summary>
	/// Process request
	/// </summary>
	/// <param name="context">Context</param>
	public void ProcessRequest(HttpContext context)
	{
		context.Response.ContentType = "application/json; charset=utf-8";
		var data = new GooddealApi(context).ExecProcess("SearchShippingNo");
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