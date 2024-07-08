/*
=========================================================================================================
  Module      : リマインダー完了画面処理(PasswordReminderComplete.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Form_User_PasswordReminderComplete : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		//------------------------------------------------------
		// ログインチェック（ログイン済みの場合、トップ画面へ）
		//------------------------------------------------------
		if (this.IsLoggedIn)
		{
			Response.Redirect(this.UnsecurePageProtocolAndHost + Constants.PATH_ROOT);
		}

		if (!IsPostBack)
		{
			//------------------------------------------------------
			// ユーザメールアドレス表示
			//------------------------------------------------------
			bool blExistMailAddr = false;
			// セッションキーを使いまわしているので、念のためtry-catchを利用
			try
			{
				if (Session[Constants.SESSION_KEY_PARAM] != null)
				{
					this.MailAddress = WebSanitizer.HtmlEncode(Session[Constants.SESSION_KEY_PARAM]);
					blExistMailAddr = true;

					// セッションクリア
					Session[Constants.SESSION_KEY_PARAM] = null;
				}
			}
			catch
			{
				// 処理なし
			}

			//------------------------------------------------------
			// メールアドレス存在チェック（メールアドレスが存在しない場合、トップ画面へ）
			//------------------------------------------------------
			if (blExistMailAddr == false)
			{
				Response.Redirect(this.UnsecurePageProtocolAndHost + Constants.PATH_ROOT);
			}
		}
	}

	/// <summary>
	/// 「戻る」リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbReturn_Click(object sender, EventArgs e)
	{
		// 「戻る」ボタン
		Response.Redirect(this.UnsecurePageProtocolAndHost + this.NextUrl);
	}

	/// <summary>
	/// 「トップページへ」リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbTopPage_Click(object sender, EventArgs e)
	{
		// トップページへ
		Response.Redirect(this.UnsecurePageProtocolAndHost + Constants.PATH_ROOT);
	}

	/// <summary>メールアドレス</summary>
	protected string MailAddress
	{
		get { return (string)ViewState["MailAddress"]; }
		private set { ViewState["MailAddress"] = value; }
	}
	/// <summary>次ページURL</summary>
	protected string NextUrl
	{
		get { return (string)Request[Constants.REQUEST_KEY_NEXT_URL] ?? ""; }
	}
}