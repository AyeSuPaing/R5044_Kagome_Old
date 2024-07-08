/*
=========================================================================================================
  Module      : セッション復元ページ処理(RestoreSession.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Text;
using System.Web;

public partial class Form_RestoreSession : System.Web.UI.Page
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// セッション情報復元
		SessionSecurityManager.RestoreSessionFromDatabaseForChangeSessionId(Context);

		// 画面遷移
		string nextUrl = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_NEXT_URL]);
		if (nextUrl == "")
		{
			nextUrl = Constants.PATH_ROOT;
		}
		
		// 他サイトへ飛ぼうとしていたらURLをルートへ書き換える（踏み台対策）
		nextUrl = this.Process.NextUrlValidation(Request, nextUrl);

		// Encode '#' character.
		nextUrl = nextUrl.Replace("#", HttpUtility.UrlEncode("#", Encoding.UTF8));

		Response.Redirect(nextUrl);
	}
	/// <summary>プロセス</summary>
	protected BasePageProcess Process
	{
		get
		{
			if (m_process == null) m_process = new BasePageProcess(this, this.ViewState, this.Context);
			return m_process;
		}
	}
	private BasePageProcess m_process = null;

}
