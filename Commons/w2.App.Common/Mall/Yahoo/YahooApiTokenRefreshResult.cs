/*
=========================================================================================================
  Module      : YAHOO API トークン更新結果クラス (YahooApiTokenRefreshResult.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.App.Common.Mall.Yahoo
{
	/// <summary>
	/// YAHOO API トークン更新結果クラス
	/// </summary>
	public class YahooApiTokenRefreshResult
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="result">結果</param>
		/// <param name="accessToken">アクセストークン</param>
		/// <param name="accessTokenExpirationDateTime">アクセストークン有効期限</param>
		public YahooApiTokenRefreshResult(
			YahooApiTokenRefreshResultCode result,
			string accessToken,
			DateTime? accessTokenExpirationDateTime = null)
		{
			this.Result = result;
			this.AccessToken = accessToken;
			this.AccessTokenExpirationDateTime = accessTokenExpirationDateTime;
		}

		/// <summary>
		/// ブラウザ上(EC管理画面)での操作が必要かどうか
		/// </summary>
		/// <returns>ブラウザ上(EC管理画面)での操作が必要かどうか</returns>
		public bool RequiresRefreshOnBrowser() =>
			this.Result == YahooApiTokenRefreshResultCode.RefreshOnBrowserRequired;

		/// <summary>
		/// 成功したかどうか
		/// </summary>
		/// <returns>成功したかどうか</returns>
		public bool IsSuccessful() =>
			string.IsNullOrEmpty(this.AccessToken) == false
			|| this.Result == YahooApiTokenRefreshResultCode.SuccessfullyRefreshed
			|| this.Result == YahooApiTokenRefreshResultCode.NoNeedToRefreshAccessToken;

		/// <summary>結果</summary>
		public YahooApiTokenRefreshResultCode Result { get; }
		/// <summary>アクセストークン</summary>
		public string AccessToken { get; }
		/// <summary>アクセストークン有効期限</summary>
		public DateTime? AccessTokenExpirationDateTime { get; }
	}

	/// <summary>
	/// YAHOO API トークン更新結果コード
	/// </summary>
	public enum YahooApiTokenRefreshResultCode
	{
		/// <summary>リフレッシュトークンを更新する必要がない</summary>
		NoNeedToRefreshAccessToken,
		/// <summary>ブラウザ上(EC管理画面)での操作が必要かどうか</summary>
		RefreshOnBrowserRequired,
		/// <summary>更新成功</summary>
		SuccessfullyRefreshed,
		/// <summary>更新失敗</summary>
		FailedToRefresh,
	}
}
