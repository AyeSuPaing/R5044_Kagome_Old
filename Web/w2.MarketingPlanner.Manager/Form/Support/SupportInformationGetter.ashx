<%--
=========================================================================================================
  Module      : サポート情報取得ハンドラ(SupportInformationGetter.ashx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
--%>
<%@ WebHandler Language="C#" Class="SupportInformationGetter" %>

using System;
using System.Web;
using w2.App.Common.SupportSite;

public class SupportInformationGetter : IHttpHandler {

	/// <summary>
	/// プロセスリクエスト
	/// </summary>
	/// <param name="context">コンテキスト</param>
	public void ProcessRequest(HttpContext context)
	{
		context.Response.AddHeader("content-type", "application/json");
		context.Response.AddHeader("charset", "UTF-8");

		var data = new SupportSiteContentsGetter().Get(context.Request["j"]);
		context.Response.Write(data);
		context.Response.End();
	}

	/// <summary>
	/// 再利用可能か
	/// </summary>
	public bool IsReusable
	{
		get { return false; }
	}

}