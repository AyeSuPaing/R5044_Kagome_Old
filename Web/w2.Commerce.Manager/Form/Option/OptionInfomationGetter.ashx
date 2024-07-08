<%--
=========================================================================================================
  Module      : オプション情報取得ハンドラ(OptionInfomationGetter.ashx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>
<%@ WebHandler Language="C#" Class="OptionInfomationGetter" %>

using System.Web;
using w2.App.Common.OptionAppeal;

/// <summary>
/// オプション情報取得ハンドラ
/// </summary>
public class OptionInfomationGetter : IHttpHandler {
	
	/// <summary>
	/// プロセスリクエスト
	/// </summary>
	/// <param name="context">コンテキスト</param>
	public void ProcessRequest (HttpContext context) {
		context.Response.AddHeader("content-type", "application/json");
		context.Response.AddHeader("charset", "UTF-8");

		var data = new OptionContentsGetter().Get();
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