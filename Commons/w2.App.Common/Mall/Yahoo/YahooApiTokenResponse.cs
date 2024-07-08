/*
=========================================================================================================
  Module      : YAHOO API TokenエンドポイントAPI結果クラス (YahooApiTokenResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Net;
using w2.App.Common.Mall.Yahoo.Dto;
using w2.Common.Logger;

namespace w2.App.Common.Mall.Yahoo
{
	/// <summary>
	/// YAHOO API TokenエンドポイントAPI結果クラス
	/// </summary>
	public class YahooApiTokenResponse
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="statusCode">HTTPステータスコード</param>
		/// <param name="reasonPhrase">理由語句</param>
		/// <param name="dto">データトランスファーオブジェクト</param>
		public YahooApiTokenResponse(HttpStatusCode statusCode, string reasonPhrase, YahooApiTokenResponseDto dto) :
			this(statusCode, reasonPhrase, dto, DateTime.Now)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="statusCode">HTTPステータスコード</param>
		/// <param name="reasonPhrase">理由語句</param>
		/// <param name="dto">データトランスファーオブジェクト</param>
		/// <param name="calledApiAt">API実行日時</param>
		/// <remarks>Dependency Injectionのため実行日時を受け取る</remarks>
		public YahooApiTokenResponse(HttpStatusCode statusCode, string reasonPhrase, YahooApiTokenResponseDto dto, DateTime calledApiAt)
		{
			this.StatusCode = statusCode;
			this.ReasonPhrase = reasonPhrase;
			this.AccessToken = dto.AccessToken;
			if (double.TryParse(dto.ExpiresIn, out var expiresIn) == false)
			{
				FileLogger.WriteWarn($"decimalへのパースに失敗しました。access_token={dto.AccessToken}expires_in={expiresIn}");
			}
			this.AccessTokenExpirationDateTime = calledApiAt.AddSeconds(expiresIn);
			this.TokenType = dto.TokenType;
			this.RefreshToken = dto.RefreshToken;
			this.RefreshTokenExpirationDateTime = calledApiAt.AddDays(value: 28);
			this.IdToken = dto.IdToken;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="statusCode">HTTPステータスコード</param>
		/// <param name="reasonPhrase">理由語句</param>
		/// <param name="dto">データトランスファーオブジェクト</param>
		public YahooApiTokenResponse(HttpStatusCode statusCode, string reasonPhrase, YahooApiTokenErrorResponseDto dto)
		{
			this.StatusCode = statusCode;
			this.ReasonPhrase = reasonPhrase;
			this.Error= dto.Error;
			this.ErrorDescription = dto.ErrorDescription;
			this.ErrorCode = dto.ErrorCode;
		}

		/// <summary>
		/// 成功したかどうか
		/// </summary>
		/// <returns>成功したかどうか</returns>
		public bool IsSuccessful() =>
			string.IsNullOrEmpty(this.AccessToken) == false && string.IsNullOrEmpty(this.Error);

		/// <summary>HTTPステータスコード</summary>
		public HttpStatusCode StatusCode { get; }
		/// <summary>理由語句</summary>
		public string ReasonPhrase { get; } = "";
		/// <summary>アクセストークン</summary>
		public string AccessToken { get; } = "";
		/// <summary>トークンタイプ</summary>
		public string TokenType { get; } = "";
		/// <summary>リフレッシュトークン</summary>
		public string RefreshToken { get; } = "";
		/// <summary>アクセストークン有効期限</summary>
		public DateTime AccessTokenExpirationDateTime { get; }
		/// <summary>リフレッシュトークン有効期限</summary>
		public DateTime RefreshTokenExpirationDateTime { get; }
		/// <summary>ID TOKEN</summary>
		public string IdToken { get; } = "";
		/// <summary>エラーコード</summary>
		public string Error { get; } = "";
		/// <summary>エラー詳細</summary>
		public string ErrorDescription { get; } = "";
		/// <summary>エラー判定用コード</summary>
		public string ErrorCode { get; } = "";
	}
}
