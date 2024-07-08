/*
=========================================================================================================
  Module      : CurrencyLayer WebAPI liveレスポンスモデルクラス(LiveResponseModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using Newtonsoft.Json;

namespace w2.Commerce.GetExchangeRate.WebApi.CurrencyLayer.WebApi.Model
{
	/// <summary>
	/// CurrencyLayer WebAPI liveレスポンスモデルクラス
	/// </summary>
	[JsonObject]
	public class LiveResponseModel
	{
		/// <summary>成功/失敗</summary>
		[JsonProperty("success")]
		public bool Success { get; set; }
		/// <summary>利用規約</summary>
		[JsonProperty("terms")]
		public string Terms { get; set; }
		/// <summary>プライバシー</summary>
		[JsonProperty("privacy")]
		public string Privacy { get; set; }
		/// <summary>タイムスタンプ</summary>
		[JsonProperty("timestamp")]
		public long? Timestamp { get; set; }
		/// <summary>通貨コード（元）</summary>
		[JsonProperty("source")]
		public string Source { get; set; }
		/// <summary>為替レートデータ</summary>
		[JsonProperty("quotes")]
		public Dictionary<string, string> Quotes { get; set; }
		/// <summary>エラーモデル</summary>
		[JsonProperty("error")]
		public ErrorModel Error { get; set; }
	}
}