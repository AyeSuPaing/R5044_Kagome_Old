/*
=========================================================================================================
  Module      : YAHOO API  TokenエンドポイントAPI エラーDTO クラス(YahooApiTokenErrorResponseDto.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Mall.Yahoo.Dto
{
	/// <summary>
	/// YAHOO API  TokenエンドポイントAPI エラーDTO クラス
	/// </summary>
	public class YahooApiTokenErrorResponseDto
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public YahooApiTokenErrorResponseDto()
		{
		}
		
		/// <summary>エラー</summary>
		[JsonProperty("error")]
		public string Error { get; set; } = "";
		/// <summary>詳細</summary>
		[JsonProperty("error_description")]
		public string ErrorDescription { get; set; } = "";
		/// <summary>エラーコード</summary>
		[JsonProperty("error_code")]
		public string ErrorCode { get; set; } = "";
	}
}
