/*
=========================================================================================================
  Module      : ExchangeRate WebAPI ExchangeRateレスポンスモデルクラス(ExchangeRateResponseModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using Newtonsoft.Json;

namespace w2.Commerce.GetExchangeRate.WebApi.ExchangeRate.WebApi.Model
{
	/// <summary>
	/// ExchangeRateApiレスポンスモデルクラス
	/// </summary>
	[JsonObject]
	public class ExchangeRateResponseModel
	{
		/// <summary>成功/失敗</summary>
		[JsonProperty("result")]
		public string Success { get; set; }
		/// <summary>ドキュメント</summary>
		[JsonProperty("documentation")]
		public string Documentation { get; set; }
		/// <summary>利用規約</summary>
		[JsonProperty("terms_of_use")]
		public string TermsOfUse { get; set; }
		/// <summary>最終更新日時(UNIXタイム)</summary>
		[JsonProperty("time_last_update_unix")]
		public long? TimeLastUpdateUnix { get; set; }
		/// <summary>最終更新日時(UTC)</summary>
		[JsonProperty("time_last_update_utc")]
		public string TimeLastUpdateUtc { get; set; }
		/// <summary>次回更新日時(UNIXタイム)</summary>
		[JsonProperty("time_next_update_unix")]
		public long? TimeNextUpdateUnix { get; set; }
		/// <summary>次回更新日時(UTC)</summary>
		[JsonProperty("time_next_update_utc")]
		public string TimeNextUpdateUtc { get; set; }
		/// <summary>通貨コード（元）</summary>
		[JsonProperty("base_code")]
		public string BaseCode { get; set; }
		/// <summary>為替レートデータ</summary>
		[JsonProperty("conversion_rates")]
		public Dictionary<string, string> ConversionRates { get; set; }
		/// <summary>エラーモデル</summary>
		[JsonProperty("error-type")]
		public string ErrorType { get; set; }
	}
}