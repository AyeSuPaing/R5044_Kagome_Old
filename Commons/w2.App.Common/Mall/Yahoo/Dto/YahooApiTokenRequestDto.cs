/*
=========================================================================================================
  Module      : YAHOO API  TokenエンドポイントAPI リクエストDTO クラス(YahooApiTokenRequestDto.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System.Text;
using System;
using w2.Common.Helper;
using w2.Common.Helper.Attribute;

namespace w2.App.Common.Mall.Yahoo.Dto
{
	/// <summary>
	/// YAHOO API  TokenエンドポイントAPI リクエストDTO クラス
	/// </summary>
	public class YahooApiTokenRequestDto
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="grantType">取得方法</param>>
		/// <param name="clientId">クライアントID</param>
		/// <param name="clientSecret">クライアントシークレット</param>
		/// <param name="redirectUri">リダイレクトURI</param>
		/// <param name="code">認可コード</param>
		/// <param name="codeVerifier">認可コード横取り攻撃対策（PKCE）のパラメーター</param>
		private YahooApiTokenRequestDto(
			string grantType,
			string clientId,
			string clientSecret,
			string redirectUri,
			string code,
			string codeVerifier)
		{
			this.GrantType = grantType;
			this.ClientId = clientId;
			this.ClientSecret = clientSecret;
			this.RedirectUri = redirectUri;
			this.Code = code;
			this.CodeVerifier = codeVerifier;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="grantType">取得方法</param>>
		/// <param name="clientId">クライアントID</param>
		/// <param name="clientSecret">クライアントシークレット</param>
		/// <param name="refreshToken">リフレッシュトークン</param>
		private YahooApiTokenRequestDto(string grantType, string clientId, string clientSecret, string refreshToken)
		{
			this.GrantType = grantType;
			this.ClientId = clientId;
			this.ClientSecret = clientSecret;
			this.RefreshToken = refreshToken;
		}

		/// <summary>
		/// インスタンス化静的メソッド
		/// </summary>
		/// <param name="grantType">取得方法</param>>
		/// <param name="clientId">クライアントID</param>
		/// <param name="clientSecret">クライアントシークレット</param>
		/// <param name="redirectUri">リダイレクトURI</param>
		/// <param name="code">認可コード</param>
		/// <param name="codeVerifier">認可コード横取り攻撃対策（PKCE）のパラメーター</param>
		/// <returns>インスタンス</returns>
		public static YahooApiTokenRequestDto InstantiateToGetTokenSet(
			string grantType,
			string clientId,
			string clientSecret,
			string redirectUri,
			string code,
			string codeVerifier) =>
			new YahooApiTokenRequestDto(grantType, clientId, clientSecret, redirectUri, code, codeVerifier);
		/// <summary>
		/// インスタンス化静的メソッド
		/// </summary>
		/// <param name="grantType">取得方法</param>>
		/// <param name="clientId">クライアントID</param>
		/// <param name="clientSecret">クライアントシークレット</param>
		/// <param name="refreshToken">リフレッシュトークン</param>
		/// <returns>インスタンス</returns>
		public static YahooApiTokenRequestDto InstantiateToRefreshAccessToken(
			string grantType,
			string clientId,
			string clientSecret,
			string refreshToken) =>
			new YahooApiTokenRequestDto(grantType, clientId, clientSecret, refreshToken);

		/// <summary>
		/// クエリ文字列生成
		/// </summary>
		/// <returns>クエリ文字列</returns>
		public string GenerateQueryString()
		{
			var queryString = QueryStringHelper.GenerateQueryString(obj: this);
			return queryString;
		}
		
		/// <summary>
		/// TokenエンドポイントAPI用のクエリ文字列生成
		/// </summary>
		/// <returns>クエリ文字列</returns>
		public string GenerateQueryStringForTokenEndpoint()
		{
			var obj = new YahooApiTokenRequestDto(
				grantType: this.GrantType,
				clientId: "",
				clientSecret: "",
				redirectUri: this.RedirectUri,
				code: this.Code,
				codeVerifier: "");
			var queryString = QueryStringHelper.GenerateQueryString(obj: obj);
			return queryString;
		}

		/// <summary>
		/// Base64エンコード
		/// </summary>
		/// <returns>エンコード済み文字列</returns>
		public string Base64Encode() =>
			Convert.ToBase64String(Encoding.UTF8.GetBytes($"{this.ClientId}:{this.ClientSecret}"));

		/// <summary>取得方法</summary>
		[QueryStringVariableName("grant_type")]
		public string GrantType { get; set; } = "";
		/// <summary>クライアントID</summary>
		[QueryStringVariableName("client_id")]
		public string ClientId { get; set; } = "";
		/// <summary>クライアントシークレット</summary>
		[QueryStringVariableName("client_secret")]
		public string ClientSecret { get; set; } = "";
		/// <summary>リダイレクトURI</summary>
		[QueryStringVariableName("redirect_uri")]
		public string RedirectUri { get; set; } = "";
		/// <summary>認可コード</summary>
		[QueryStringVariableName("code")]
		public string Code { get; set; } = "";
		/// <summary>認可コード横取り攻撃対策（PKCE）のパラメーター</summary>
		[QueryStringVariableName("code_verifier")]
		public string CodeVerifier { get; set; } = "";
		/// <summary>リフレッシュトークン</summary>
		[QueryStringVariableName("refresh_token")]
		public string RefreshToken { get; set; } = "";
	}
}
