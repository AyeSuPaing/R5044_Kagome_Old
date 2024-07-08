<!--
=========================================================================================================
  Module      : セッション維持用ジェネリックハンドラー(MaintainSession.ashx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
-->
<%@ WebHandler Language="C#" Class="MaintainSession" %>

using System.Web;

/// <summary>
/// セッション維持用ジェネリックハンドラー
/// </summary>
public class MaintainSession : IHttpHandler {
	/// <summary>
	/// リクエスト処理
	/// </summary>
	/// <param name="context">コンテキスト</param>
	public void ProcessRequest (HttpContext context) {
		context.Response.ContentType = "text/plain";
		context.Response.Write(string.Empty);
	}
	
	/// <summary>
	/// インスタンスが再利用できるか
	/// </summary>
	public bool IsReusable {
		get { return true; }
	}
}