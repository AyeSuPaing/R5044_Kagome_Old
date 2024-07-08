/*
=========================================================================================================
  Module      : リクエストモデル(RequestModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using Newtonsoft.Json;

/// <summary>
/// リクエストモデル
/// </summary>
public class RequestModel : BaseParamModel
{
	/// <summary>要求情報リスト</summary>
	[JsonProperty("request")]
	public List<Request> Requests { get; set; }
}

/// <summary>
/// リクエスト要求モデル
/// </summary>
public class Request : BaseParam
{
	/// <summary>リクエスト商品情報リスト</summary>
	[JsonProperty("itemize")]
	public List<RequestItemize> Items { get; set; }
}

/// <summary>
/// リクエスト商品情報モデル
/// </summary>
public class RequestItemize : BaseItemize
{
	/// <summary>リクエスト追加情報リスト</summary>
	[JsonProperty("extra_option")]
	public new List<RequestExtraOption> ExtraOptions { get; set; }
}

/// <summary>
/// リクエスト追加情報モデル
/// </summary>
public class RequestExtraOption : BaseExtraOption
{
}