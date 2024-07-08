/*
=========================================================================================================
  Module      : シングルサインオンリンク（Webカスタムコントロール）(SingleSignOnLink.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.Cryptography;
using w2.Domain.User;

namespace w2.App.Common.Web.WebCustomControl
{
	/// <summary>
	/// SingleSignOnLink の概要の説明です
	/// </summary>
	[ToolboxData("<{0}:SingleSignOnLink runat=server></{0}:SingleSignOnLink>")]
	public class SingleSignOnLink : LinkButton
	{
		/// <summary>
		/// リダイレクト処理
		/// </summary>
		/// <param name="userId">ユーザー</param>
		protected void Redirect(string userId)
		{
			HttpResponse response = Page.Response;
			var userData = new UserService().Get(userId);

			// 会員では無い場合、設定されたURLへリダイレクト
			if (IsMember(userData) == false) response.Redirect(this.RedirectUrl, true);

			// ログインに必要な情報を取得
			string loginId = userData.LoginId;
			string password = userData.PasswordDecrypted;

			string redirectUrl = AppendSingleSignOnParam(this.RedirectUrl, loginId, password);
			response.Redirect(redirectUrl);
		}

		/// <summary>
		/// SingleSignOn用パラメタ付与
		/// </summary>
		/// <param name="url">URL</param>
		/// <param name="loginId">ログインID</param>
		/// <param name="password">パスワード</param>
		/// <returns>パラメタ付与されたURL</returns>
		private string AppendSingleSignOnParam(string url, string loginId, string password)
		{
			string param = CreateSingleSignOnParam(loginId, password);
			return url + (HasQuery(url) ? "&" : "?") + param;
		}

		/// <summary>
		/// SingleSignOn用のパラメタ生成
		/// </summary>
		/// <param name="loginId">ログインID</param>
		/// <param name="password">パスワード</param>
		/// <returns>SingleSignOn用パラメタ</returns>
		private string CreateSingleSignOnParam(string loginId, string password)
		{
			Cryptographer crypt = new Cryptographer(Constants.ENCRYPTION_SINGLE_SIGN_ON_KEY, Constants.ENCRYPTION_SINGLE_SIGN_ON_IV);
			string encryptedLoginId = crypt.Encrypt(loginId);
			string encryptedPassword = crypt.Encrypt(password);

			return Constants.REQUEST_KEY_SINGLE_SIGN_ON_LOGINID + "=" + HttpUtility.UrlEncode(encryptedLoginId)
				+ "&" + Constants.REQUEST_KEY_SINGLE_SIGN_ON_PASSWORD + "=" + HttpUtility.UrlEncode(encryptedPassword);
		}

		/// <summary>
		/// クエリが付与されているか
		/// </summary>
		/// <param name="url">URL</param>
		/// <returns>クエリ付与フラグ</returns>
		private bool HasQuery(string url)
		{
			Uri uri = new Uri(url);
			return uri.Query != "";
		}

		/// <summary>
		/// 会員かどうか
		/// </summary>
		/// <param name="userData">ユーザデータ</param>
		/// <returns>会員フラグ</returns>
		private bool IsMember(UserModel userData)
		{
			if (userData == null) return false;
	
			if (userData.LoginId == "" || userData.Password == "") return false;

			return true;
		}

		/// <summary>リダイレクト先URL</summary>
		public string RedirectUrl
		{
			get { return m_RedirectUrl.Replace("http://", Constants.PROTOCOL_HTTPS);}
			set { m_RedirectUrl = value;}
		}
		private string m_RedirectUrl;
	}
}