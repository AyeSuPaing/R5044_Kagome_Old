<%@ WebHandler Language="C#" Class="AndmallApi" %>
/*
=========================================================================================================
  Module      : ＆mall在庫引当API ジェネリックハンドラー(AndmallApi.ashx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System.Web;

/// <summary>
/// ＆mall在庫引当API ジェネリックハンドラー
/// </summary>
public class AndmallApi : IHttpHandler
{
	/// <summary>
	/// リクエスト処理
	/// </summary>
	/// <param name="context">HTTPコンテキスト</param>
	public void ProcessRequest(HttpContext context)
	{
		new AndmallStockAllocation().ProcessRequestContext(context);
	}

	/// <summary>
	/// 別の要求は使用できない
	/// </summary>
	public bool IsReusable
	{
		get
		{
			return false;
		}
	}
}