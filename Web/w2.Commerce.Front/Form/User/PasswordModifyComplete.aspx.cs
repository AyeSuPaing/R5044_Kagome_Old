/*
=========================================================================================================
  Module      : パスワード変更完了画面処理(PasswordModifyComplete.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Form_User_PasswordModifyComplete : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{

		//------------------------------------------------------
		// HTTPS通信チェック（HTTPのとき、トップ画面へ）
		//------------------------------------------------------
		CheckHttps();

		//------------------------------------------------------
		// ログインチェック（ログイン済みの場合、トップ画面へ）
		//------------------------------------------------------
		if (this.IsLoggedIn)
		{
			Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT);
		}

		if (!IsPostBack)
		{
			// 何もしない //
		}
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
}