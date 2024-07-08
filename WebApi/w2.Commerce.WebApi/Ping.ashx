<%--
=========================================================================================================
  Module      : WebAPI死活監視 (Ping.ashx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>
<%@ WebHandler Language="C#" Class="Ping" %>

using System;
using System.Net;
using System.Web;
using w2.Common.Logger;
using w2.Common.Sql;

public class Ping : IHttpHandler {
	
	/// <summary>
	/// プロセスリクエスト
	/// </summary>
	/// <param name="context">Context</param>
	public void ProcessRequest (HttpContext context) {

		var result = EnsureSqlConnection();

		context.Response.ContentType = "text/plain";
		context.Response.TrySkipIisCustomErrors = true;
		context.Response.StatusCode = result
			? (int)HttpStatusCode.OK
			: (int)HttpStatusCode.ServiceUnavailable;
		context.Response.Write(context.Response.StatusCode);
		context.Response.End();
	}

	public bool IsReusable
	{
		get
		{
			return false;
		}
	}

	/// <summary>
	/// SQL接続を確認
	/// </summary>
	/// <returns>結果</returns>
	public static bool EnsureSqlConnection()
	{
		try
		{
			using (var accessor = new SqlAccessor())
			{
				// ここで例外が発生しなければOK
				accessor.OpenConnection();
				return true;
			}
		}
		catch (Exception ex)
		{
			AppLogger.WriteError(ex);
			return false;
		}
	}
}