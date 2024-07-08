<%@ WebHandler Language="C#" Class="NextEngineUpdateStock" %>
/*
=========================================================================================================
  Module      : ネクストエンジン在庫連携API ジェネリックハンドラー(NextEngineUpdateStock.ashx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System.Web;

/// <summary>
/// ネクストエンジン在庫連携API ジェネリックハンドラー
/// </summary>
public class NextEngineUpdateStock : IHttpHandler
{
	/// <summary>
	/// リクエスト処理
	/// </summary>
	/// <param name="context">HTTPコンテキスト</param>
	public void ProcessRequest(HttpContext context)
	{
		new NextEngineStockUpdateApi(context).ProcessRequestContext();
	}

	/// <summary>
	/// 別の要求は使用できない
	/// </summary>
	public bool IsReusable
	{
		get { return false; }
	}
}