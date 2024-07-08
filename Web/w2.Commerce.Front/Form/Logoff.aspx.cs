/*
=========================================================================================================
  Module      : ログオフ画面処理(Logoff.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Web;
using w2.Common.Web;

public partial class Form_Logoff : BasePage
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }	// httpsアクセス

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		// Amazon連携時はクライアント側でAmazonログオフ（JS）実行後、サーバー側でログオフする
		var execLogoff = this.IsAmazonLoggedIn
			? this.IsPostBack
			: true;

		if (execLogoff)
		{
			//------------------------------------------------------
			// ユーザログオフ
			//------------------------------------------------------
			// セッションクリア
			Session.Contents.RemoveAll();

			// ログオフ時はセッションにあわせてクッキーの認証キーも削除
			CookieManager.RemoveCookie(Constants.COOKIE_KEY_AUTH_KEY, Constants.PATH_ROOT);

			//------------------------------------------------------
			// 次のページの制御
			//------------------------------------------------------
			string nextUrl = NextUrlValidation(StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_NEXT_URL]));
			if (nextUrl.Length == 0)
			{
				nextUrl = this.UnsecurePageProtocolAndHost + Constants.PATH_ROOT;
			}

			//------------------------------------------------------
			// セッション張り直しのためのデータ格納（セッションハイジャック対策）
			//------------------------------------------------------
			SessionSecurityManager.SaveSesstionContetnsToDatabaseForChangeSessionId(Request, Response, Session);

			//------------------------------------------------------
			// 元のページへリダイレクト（セッション復元）
			//------------------------------------------------------
			Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_RESTORE_SESSION
				+ "?" + Constants.REQUEST_KEY_NEXT_URL + "=" + Server.UrlEncode(nextUrl));
		}
	}
}

