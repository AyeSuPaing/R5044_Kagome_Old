<%@ WebHandler Language="C#" Class="FacebookCatalog" %>
/*
=========================================================================================================
  Module      : Facebook Catalog (FacebookCatalog.ashx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Web;

/// <summary>
/// Facebook Catalog
/// </summary>
public class FacebookCatalog : IHttpHandler
{
	/// <summary>
	/// Process request
	/// </summary>
	/// <param name="context">Context</param>
	public void ProcessRequest(HttpContext context)
	{
		new GetFacebookCatalogApi().ProcessRequestContext(context);
	}

	/// <summary>
	/// Is reusable
	/// </summary>
	public bool IsReusable
	{
		get { return false; }
	}
}