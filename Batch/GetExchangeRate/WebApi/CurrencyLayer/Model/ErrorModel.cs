/*
=========================================================================================================
  Module      : CurrencyLayer WebAPI エラーモデルクラス(ErrorModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.Commerce.GetExchangeRate.WebApi.CurrencyLayer.WebApi.Model
{
	/// <summary>
	/// エラーモデルクラス
	/// </summary>
	[JsonObject]
	public class ErrorModel
	{
		/// <summary>エラーコード</summary>
		[JsonProperty("code")]
		public long? Code { get; set; }
		/// <summary>種別</summary>
		[JsonProperty("type")]
		public string Type { get; set; }
		/// <summary>情報</summary>
		[JsonProperty("info")]
		public string Info { get; set; }
	}
}