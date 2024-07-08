/*
=========================================================================================================
  Module      : YAHOO API TokenエンドポイントAPI レスポンスDTO クラス(YahooApiTokenResponseDto.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Mall.Yahoo.Dto
{
	/// <summary>
	/// YAHOO API TokenエンドポイントAPI レスポンスDTO クラス
	/// </summary>
	public class YahooApiTokenResponseDto
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public YahooApiTokenResponseDto()
		{
		}

		/// <summary>アクセストークン</summary>
		[JsonProperty("access_token")]
		public string AccessToken { get; set; } = "";
		/// <summary>トークンタイプ</summary>
		[JsonProperty("token_type")]
		public string TokenType { get; set; } = "";
		/// <summary>リフレッシュトークン</summary>
		[JsonProperty("refresh_token")]
		public string RefreshToken { get; set; } = "";
		/// <summary>Access Tokenの有効期限を表す秒数</summary>
		[JsonProperty("expires_in")]
		public string ExpiresIn { get; set; } = "";
		/// <summary>ID TOKEN</summary>
		[JsonProperty("id_token")]
		public string IdToken { get; set; } = "";
	}
}
