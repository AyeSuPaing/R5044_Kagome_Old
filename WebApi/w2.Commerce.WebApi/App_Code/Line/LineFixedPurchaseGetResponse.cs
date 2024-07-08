/*
=========================================================================================================
  Module      : LINE連携の定期台帳レスポンス (LineFixedPurchaseGetResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;

/// <summary>
/// LINE連携の定期台帳レスポンス
/// </summary>
[Serializable]
public class LineFixedPurchaseGetResponse
{
	/// <summary>検索結果の数</summary>
	[JsonProperty(PropertyName = "count")]
	public int Count
	{
		get { return (this.FixedPurchases != null) ? this.FixedPurchases.Length : 0; }
	}
	/// <summary>リクエストで指定したoffset</summary>
	[JsonProperty(PropertyName = "offset")]
	public int Offset { get; set; }
	/// <summary>リクエストで指定したlimit</summary>
	[JsonProperty(PropertyName = "limit")]
	public int Limit { get; set; }
	/// <summary>定期情報の配列</summary>
	[JsonProperty(PropertyName = "fixedPurchases")]
	public LineFixedPurchase[] FixedPurchases { get; set; }
	/// <summary>ステータス</summary>
	[JsonProperty(PropertyName = "status")]
	public int Status { get; set; }
}