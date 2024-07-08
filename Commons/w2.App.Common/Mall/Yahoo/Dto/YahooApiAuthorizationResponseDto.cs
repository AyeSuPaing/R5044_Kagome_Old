/*
=========================================================================================================
  Module      : YAHOO API AuthorizationエンドポイントAPI レスポンスDTO クラス(YahooApiAuthorizationResponseDto.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Mall.Yahoo.Dto
{
	/// <summary>
	/// YAHOO API AuthorizationエンドポイントAPI レスポンスDTO クラス
	/// </summary>
	public class YahooApiAuthorizationResponseDto
	{
		/// <summary>アクセストークン</summary>
		public string AccessToken { get; set; } = "";
		/// <summary>トークンタイプ</summary>
		public string TokenType { get; set; } = "";
		/// <summary>ID TOKEN</summary>
		public string IdToken { get; set; } = "";
		/// <summary>認可コード</summary>
		public string Code { get; set; } = "";
		/// <summary>ステート</summary>
		public string State { get; set; } = "";
	}
}
