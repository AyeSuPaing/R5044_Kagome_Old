/*
=========================================================================================================
  Module      : シルバーエッグレポート レスポンスデータ(SilvereggAigentReportResponseData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

[JsonObject]
public class SilvereggAigentReportResponseData
{
	/// <summary>結果コード（complete / error）</summary>
	[JsonProperty(Constants.JSON_FIELD_RECOMMEND_REPORT_TYPE)]
	public string Type { get; set; }
	/// <summary>通貨単位（前）</summary>
	[JsonProperty(Constants.JSON_FIELD_RECOMMEND_REPORT_CURRENCY)]
	public string Currency { get; set; }
	/// <summary>通貨単位（後）</summary>
	[JsonProperty(Constants.JSON_FIELD_RECOMMEND_REPORT_CURRENCYPOSTFIX)]
	public string CurrencyPostfix { get; set; }
	/// <summary>結果リスト</summary>
	[JsonProperty(Constants.JSON_FIELD_RECOMMEND_REPORT_ROW)]
	public string[][] Row { get; set; }
	/// <summary>結果メッセージ</summary>
	[JsonProperty(Constants.JSON_FIELD_RECOMMEND_REPORT_MESSAGE)]
	public string Message { get; set; }
	/// <summary>稼働状況</summary>
	[JsonProperty(Constants.JSON_FIELD_RECOMMEND_REPORT_ISACTIVE)]
	public string IsActive { get; set; }
	/// <summary>マーチャントID</summary>
	[JsonProperty(Constants.JSON_FIELD_RECOMMEND_REPORT_MERCHANT)]
	public string Merchant { get; set; }
	/// <summary>集計開始日・月（YYYYMMDD / YYYYMM）</summary>
	[JsonProperty(Constants.JSON_FIELD_RECOMMEND_REPORT_FROM)]
	public string From { get; set; }
	/// <summary>集計終了日・月（YYYYMMDD / YYYYMM）</summary>
	[JsonProperty(Constants.JSON_FIELD_RECOMMEND_REPORT_TO)]
	public string To { get; set; }
	/// <summary>集計日・月（YYYYMMDD / YYYYMM）</summary>
	[JsonProperty(Constants.JSON_FIELD_RECOMMEND_REPORT_DATE)]
	public string Date { get; set; }
	/// <summary>レコメンドID</summary>
	[JsonProperty(Constants.JSON_FIELD_RECOMMEND_REPORT_SPEC)]
	public string Spec { get; set; }
}
