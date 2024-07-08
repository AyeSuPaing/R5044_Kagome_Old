/*
=========================================================================================================
  Module      : YAHOO API トークンセット (YahooApiTokenSet.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.App.Common.Mall.Yahoo
{
	/// <summary>
	/// YAHOO API トークンセット
	/// </summary>
	public class YahooApiTokenSet
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessToken">アクセストークン</param>
		/// <param name="accessTokenExpirationDateTime">アクセストークン有効期限</param>
		/// <param name="refreshToken">リフレッシュトークン</param>
		/// <param name="refreshTokenExpirationDateTime">リフレッシュトークン有効期限</param>
		public YahooApiTokenSet(
			string accessToken,
			DateTime? accessTokenExpirationDateTime,
			string refreshToken,
			DateTime? refreshTokenExpirationDateTime)
		{
			this.AccessToken = accessToken;
			this.AccessTokenExpirationDateTime = accessTokenExpirationDateTime;
			this.RefreshToken = refreshToken;
			this.RefreshTokenExpirationDateTime= refreshTokenExpirationDateTime;
		}

		/// <summary>
		/// 認可コードを使用してAccess Tokenを取得する必要がある
		/// </summary>
		/// <returns>認可コードを使用してAccess Tokenを取得する必要がある</returns>
		/// <remarks>
		/// True: AuthorizationエンドポイントAPIの実行が必要な時 <br />
		/// AuthCodeというのは、取得にブラウザ操作が必要な "認可コード" のこと。
		/// </remarks>
		public bool RequiresToAcquireNewAccessTokenWithAuthCode() =>
			RequiresToAcquireNewAccessTokenWithAuthCode(DateTime.Now);
		/// <summary>
		/// 認可コードを使用してAccess Tokenを取得する必要がある
		/// </summary>
		/// <param name="dateTimeToCompare">処理実行日時</param>
		/// <remarks>比較対象の日時のことで、基本は現在日時を指す。ユニットテストのために引数を持たせる。</remarks>
		/// <returns>認可コードを使用してAccess Tokenを取得する必要がある</returns>
		/// <remarks>
		/// True: AuthorizationエンドポイントAPIの実行が必要な時 <br />
		/// AuthCodeというのは、取得にブラウザ操作が必要な "認可コード" のこと。
		/// </remarks>
		public bool RequiresToAcquireNewAccessTokenWithAuthCode(DateTime dateTimeToCompare) =>
			this.HasAccessTokenAvailable(dateTimeToCompare) == false
			&& this.HasRefreshTokenAvailable(dateTimeToCompare) == false;

		/// <summary>
		/// 有効なアクセストークンがあるかどうか
		/// </summary>
		/// <param name="dateTimeToCompare">有効期限が切れているかどうかを比較するための時間</param>
		/// <remarks>Dependency Injectionのため</remarks>
		/// <returns>有効なアクセストークンがあるかどうか</returns>
		public bool HasAccessTokenAvailable(DateTime dateTimeToCompare) =>
			HasAccessToken()
			&& (this.AccessTokenExpirationDateTime != null)
			&& (this.AccessTokenExpirationDateTime >= dateTimeToCompare);

		/// <summary>
		/// Access Tokenが存在する
		/// </summary>
		/// <returns>アクセストークンが存在するのかどうか</returns>
		private bool HasAccessToken() => string.IsNullOrEmpty(this.AccessToken) == false;

		/// <summary>
		/// 有効なリフレッシュトークンがあるかどうか
		/// </summary>
		/// <param name="dateTimeToCompare">有効期限が切れているかどうかを比較するための時間</param>
		/// <returns>有効なリフレッシュトークンがあるかどうか</returns>
		private bool HasRefreshTokenAvailable(DateTime dateTimeToCompare) =>
			HasRefreshToken() && (this.RefreshTokenExpirationDateTime != null)
			&& (this.RefreshTokenExpirationDateTime >= dateTimeToCompare);

		/// <summary>
		/// リフレッシュトークンが存在するか
		/// </summary>
		/// <returns>リフレッシュトークンが存在するか</returns>
		private bool HasRefreshToken() => string.IsNullOrEmpty(this.RefreshToken) == false;

		/// <summary>アクセストークン</summary>
		public string AccessToken { get; }
		/// <summary>アクセストークン有効期限</summary>
		public DateTime? AccessTokenExpirationDateTime { get; }
		/// <summary>リフレッシュトークン</summary>
		public string RefreshToken { get; }
		/// <summary>リフレッシュトークン有効期限</summary>
		public DateTime? RefreshTokenExpirationDateTime { get; }
	}
}
