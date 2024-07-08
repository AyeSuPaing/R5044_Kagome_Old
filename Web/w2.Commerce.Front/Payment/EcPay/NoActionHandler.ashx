<%@ WebHandler Language="C#" Class="NoActionHandler" %>

/*
=========================================================================================================
  Module      : ダミー通知ハンドラ (NoActionHandler.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

using System.Web;

/// <summary>
/// ダミー通知ハンドラ
/// </summary>
public class NoActionHandler : IHttpHandler
{
	/// <summary>
	/// リクエスト処理
	/// </summary>
	/// <param name="context">HTTPコンテキスト</param>
	public void ProcessRequest(HttpContext context)
	{
		context.Response.ContentType = "text/plain";
		context.Response.Write("1|OK");
		context.Response.StatusCode = 200;
		context.Response.End();
	}

	/// <summary>ハンドラの再利用可能フラグ</summary>
	public bool IsReusable
	{
		get { return true; }
	}
}
