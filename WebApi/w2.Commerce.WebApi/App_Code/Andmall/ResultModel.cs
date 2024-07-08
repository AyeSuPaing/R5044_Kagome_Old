/*
=========================================================================================================
  Module      : レスポンス用モデル(ResultModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using Newtonsoft.Json;

/// <summary>
/// レスポンス用モデル
/// </summary>
public class ResultModel : BaseParamModel
{
	/// <summary>処理結果コード</summary>
	[JsonProperty("processed_code")]
	public string ProcessedCode { get; set; }
	/// <summary>エラーメッセージ</summary>
	[JsonProperty("processed_message", DefaultValueHandling = DefaultValueHandling.Ignore)]
	public string ProcessedMessage { get; set; }
	/// <summary>結果情報リスト</summary>
	[JsonProperty("result", DefaultValueHandling = DefaultValueHandling.Ignore)]
	public List<Result> Results { get; set; }
}

/// <summary>
/// 結果情報モデル
/// </summary>
public class Result : BaseParam
{
	/// <summary></summary>
	[JsonProperty("itemize")]
	public List<ResultItemize> Items { get; set; }
}

/// <summary>
/// 結果明細モデル
/// </summary>
public class ResultItemize : BaseItemize
{
	/// <summary>明細処理結果コード</summary>
	[JsonProperty("processed_code")]
	public string ProcessedCode { get; set; }
	/// <summary>明細エラーメッセージ</summary>
	[JsonProperty("processed_message", DefaultValueHandling = DefaultValueHandling.Ignore)]
	public string ProcessedMessage { get; set;}
	/// <summary>引当数量</summary>
	[JsonProperty("allocate_quantity", DefaultValueHandling = 0)]
	public int AllocateQuantity { get; set; }
	/// <summary>引当可能数</summary>
	[JsonProperty("allocatable_stock", DefaultValueHandling = 0)]
	public int AllocatableStock { get; set; }
	/// <summary>[ログ用]商品バリエーションID</summary>
	[JsonIgnore]
	public string VariationId { get; set; }
	/// <summary>[ログ用]識別コード</summary>
	[JsonIgnore]
	public string IdentificationCode { get; set; }
}

/// <summary>
/// 結果追加項目
/// </summary>
public class ResultExtraOption : BaseExtraOption
{
}