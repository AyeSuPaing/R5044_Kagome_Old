/*
=========================================================================================================
  Module      : リクエスト・レスポンスのベースモデル(BaseParamModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using Newtonsoft.Json;

/// <summary>
/// リクエスト・レスポンスのベースモデル
/// </summary>
public class BaseParamModel
{
	/// <summary>要求情報総数</summary>
	[JsonProperty("total_request")]
	public int TotalRequest { get; set; }
}

/// <summary>
/// リクエスト要求・レスポンス要求のベースモデル
/// </summary>
public class BaseParam
{
	/// <summary>要求情報番号</summary>
	[JsonProperty("no")]
	public int Number { get; set; }
	/// <summary>モード</summary>
	[JsonProperty("mode")]
	public int Mode { get; set; }
	/// <summary>識別コード</summary>
	[JsonProperty("identification_code")]
	public string IdentificationCode { get; set; }
	/// <summary>店番</summary>
	[JsonProperty("spl_code")]
	public string SqlCode { get; set; }
	/// <summary>在庫利用種別</summary>
	[JsonProperty("use_type")]
	public string UseType { get; set; }
}

/// <summary>
/// リクエスト商品情報・レスポンス商品情報のベースモデル
/// </summary>
public class BaseItemize
{
	/// <summary>テナントコード</summary>
	[JsonProperty("corporation_code")]
	public string CorporationCode { get; set; }
	/// <summary>ショップコード</summary>
	[JsonProperty("base_store_code")]
	public string BaseStoreCode { get; set; }
	/// <summary>サイトコード</summary>
	[JsonProperty("site_code")]
	public string SiteCode { get; set; }
	/// <summary>新識別コード</summary>
	[JsonProperty("new_identification_code")]
	public string NewIdentificationCode { get; set; }
	/// <summary>商品コード</summary>
	[JsonProperty("product_code")]
	public string ProductCode { get; set; }
	/// <summary>SKUコード</summary>
	[JsonProperty("cs_code")]
	public string CsCode { get; set; }
	/// <summary>販売方式</summary>
	[JsonProperty("sales_type", DefaultValueHandling = DefaultValueHandling.Ignore)]
	public string SalesType { get; set; }
	/// <summary>数量</summary>
	[JsonProperty("quantity", DefaultValueHandling = 0)]
	public int Quantity { get; set; }
	/// <summary>追加項目</summary>
	[JsonProperty("extra_option", DefaultValueHandling = DefaultValueHandling.Ignore)]
	public List<RequestExtraOption> ExtraOptions { get; set; }
}

/// <summary>
/// リクエストオプション・レスポンスオプションのベースモデル
/// </summary>
public class BaseExtraOption
{
}