/*
=========================================================================================================
  Module      : Yahoo API モジュールインターフェース (IYahooApiFacade.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Mall.Yahoo.Foundation;
using w2.App.Common.Mall.Yahoo.YahooMallOrders;

namespace w2.App.Common.Mall.Yahoo.Interfaces
{
	/// <summary>
	/// Yahoo API モジュールインターフェース
	/// </summary>
	public interface IYahooApiFacade
	{
		/// <summary>
		/// AuthorizationエンドポイントAPIのためのURL生成
		/// </summary>
		/// <param name="clientId">クライアントID</param>
		/// <param name="state">ステート</param>
		/// <returns>URL</returns>
		string GenerateUrlToGetApiTokens(string clientId, string state);

		/// <summary>
		/// 認可コードを使用して、アクセストークンとリフレッシュトークンを取得
		/// </summary>
		/// <param name="clientId">クライアントID</param>
		/// <param name="clientSecret">クライアントシークレット</param>
		/// <param name="authCode">認可コード</param>
		/// <param name="redirectUri">リダイレクトURI</param>
		/// <returns>TokenエンドポイントAPIの実行結果</returns>
		YahooApiTokenResponse GetAccessTokensWithAuthCode(
			string clientId,
			string clientSecret,
			string authCode,
			string redirectUri);

		/// <summary>
		/// 認可コードを使用して、アクセストークンとリフレッシュトークンを取得
		/// </summary>
		/// <param name="clientId">クライアントID</param>
		/// <param name="clientSecret">クライアントシークレット</param>
		/// <param name="refreshToken">リフレッシュトークン</param>
		/// <returns>TokenエンドポイントAPIの実行結果</returns>
		YahooApiTokenResponse GetAccessTokenWithRefreshToken(string clientId, string clientSecret, string refreshToken);

		/// <summary>
		/// 注文詳細APIでYahoo注文詳細を取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessToken">アクセストークン</param>
		/// <param name="sellerId">セラーID</param>
		/// <param name="authValue">暗号化した認証情報</param>
		/// <param name="publicKeyVersion">公開鍵バージョン</param>
		/// <returns>Yahoo注文詳細</returns>
		YahooMallOrder GetYahooMallOrder(
			string orderId,
			string accessToken,
			string sellerId,
			string authValue = "",
			string publicKeyVersion = "");
	}
}
