/*
=========================================================================================================
  Module      : メールマガジン解除完了画面処理(MailMagazineCancelComplete.aspx.cs)
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

public partial class Form_User_MailMagazineCancelComplete : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		// 何もしない //
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
	protected string MailAddress { get { return (string)Session[Constants.SESSION_KEY_PARAM]; } }

}
