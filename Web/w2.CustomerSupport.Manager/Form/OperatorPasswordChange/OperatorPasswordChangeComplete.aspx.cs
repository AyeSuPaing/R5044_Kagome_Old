/*
=========================================================================================================
  Module      : パスワード変更完了ページ処理(OperatorPasswordChangeComplete.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

public partial class Form_OperatorPasswordChange_OperatorPasswordChangeComplete : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{
		// セッション初期化
		InitSession();
	}

	/// <summary>
	/// セッション初期化
	/// </summary>
	private void InitSession()
	{
		// オペレータログインオフ
		Session.Clear();
	}

	/// <summary>
	/// ログイン画面へボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnLogin_Click(object sender, System.EventArgs e)
	{
		// ログインページへ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_LOGIN);
	}
}

