/*
=========================================================================================================
  Module      : メールマガジン登録完了画面処理(MailMagazineRegistComplete.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;

public partial class Form_User_MailMagazineRegistComplete : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
    {
		if (!IsPostBack)
		{
			//------------------------------------------------------
			// ユーザメールアドレス表示
			//------------------------------------------------------
			try
			{
				this.UserMailAddr = ((UserInput)Session[Constants.SESSION_KEY_PARAM]).MailAddr;
			}
			catch
			{
				// パラメタ取得失敗エラー
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_NO_PARAM);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}
		}

		DataBind();
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
	protected string UserMailAddr
	{
		get { return (string)ViewState["UserMailAddr"]; }
		set { ViewState["UserMailAddr"] = value; }
	}
}