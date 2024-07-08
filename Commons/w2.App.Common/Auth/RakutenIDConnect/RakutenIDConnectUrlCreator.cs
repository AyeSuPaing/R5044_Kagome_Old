/*
=========================================================================================================
  Module      : 楽天IDConnect用URL作成クラス(RakutenIDConnectUrlCreator.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Web;

namespace w2.App.Common.Auth.RakutenIDConnect
{
	/// <summary>
	/// 楽天IDConnect用URL作成クラス
	/// </summary>
	public class RakutenIDConnectUrlCreator
	{
		#region メソッド
		/// <summary>
		/// ログイン認証用URL作成インスタンス作成
		/// </summary>
		/// <param name="type">アクション種別</param>
		/// <param name="state">ステート</param>
		/// <param name="nonce">ノンス</param>
		/// <returns>ログイン認証用URL作成インスタンス</returns>
		public static UrlCreator CreateUrlCreatorForAuth(ActionType type, string state, string nonce)
		{
			// 楽天IDConnect連携用モック利用？
			if (string.IsNullOrEmpty(Constants.RAKUTEN_ID_CONNECT_MOCK_URL) == false)
			{
				// HttpWebRequestでアクセスするため絶対URL指定
				var url = new UrlCreator(Constants.RAKUTEN_ID_CONNECT_MOCK_URL)
					.AddParam("type", (type == ActionType.UserRegister) ? "register" : "authorize")
					.AddParam("state", state);
				return url;
			}

			// 新規登録の場合はOpenID以外も取得
			var scope = "openid";
			if (type == ActionType.UserRegister)
			{
				scope = "openid profile email address phone";
			}

			var redirectUri = CreateRedirectUri();
			var urlCreator = new UrlCreator("https://accounts.id.rakuten.co.jp/auth/oauth/authorize")
				.AddParam("response_type", "code")
				.AddParam("client_id", Constants.RAKUTEN_ID_CONNECT_CLIENT_ID)
				.AddParam("redirect_uri", redirectUri)
				.AddParam("scope", scope)
				.AddParam("state", state)
				.AddParam("nonce", nonce);

			return urlCreator;
		}

		/// <summary>
		/// トークン用URL作成インスタンス作成
		/// </summary>
		/// <param name="code">Authorization Code</param>
		/// <returns>トークン用URL作成インスタンス</returns>
		public static UrlCreator CreateUrlCreatorForToken(string code)
		{
			// 楽天IDConnect連携用モック利用？
			if (string.IsNullOrEmpty(Constants.RAKUTEN_ID_CONNECT_MOCK_URL) == false)
			{
				// HttpWebRequestでアクセスするため絶対URL指定
				return new UrlCreator(Constants.RAKUTEN_ID_CONNECT_MOCK_URL).AddParam("type", "tokens");
			}

			var redirectUri = CreateRedirectUri();
			var urlCreator = new UrlCreator("https://api.accounts.id.rakuten.co.jp/v1/oAuth/tokens")
				.AddParam("grant_type", "authorization_code")
				.AddParam("client_id", Constants.RAKUTEN_ID_CONNECT_CLIENT_ID)
				.AddParam("client_secret", Constants.RAKUTEN_ID_CONNECT_CLIENT_SECRET)
				.AddParam("code", code)
				.AddParam("redirect_uri", redirectUri);
			return urlCreator;
		}

		/// <summary>
		/// ユーザー取得用URL作成インスタンス作成
		/// </summary>
		/// <returns>ユーザー取得用URL作成インスタンス</returns>
		public static UrlCreator CreateUrlCreatorForUserInfo()
		{
			// 楽天IDConnect連携用モック利用？
			if (string.IsNullOrEmpty(Constants.RAKUTEN_ID_CONNECT_MOCK_URL) == false)
			{
				// HttpWebRequestでアクセスするため絶対URL指定
				return new UrlCreator(Constants.RAKUTEN_ID_CONNECT_MOCK_URL).AddParam("type", "userinfo");
			}

			var urlCreator = new UrlCreator("https://api.accounts.id.rakuten.co.jp/v1/openid/userinfo");
			return urlCreator;
		}

		/// <summary>
		/// 戻り先URL作成
		/// </summary>
		/// <returns>戻り先URL</returns>
		public static string CreateRedirectUri()
		{
			var redirectUri = Constants.PROTOCOL_HTTPS + Constants.SITE_DOMAIN + Constants.PATH_ROOT + Constants.PAGE_FRONT_AUTH_RAKUTEN_ID_CONNECT;
			return redirectUri;
		}
		#endregion
	}
}
