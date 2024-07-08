/*
=========================================================================================================
  Module      : 会員退会完了画面処理(UserWithdrawalComplete.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Form_User_UserWithdrawalComplete : BasePage
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }	// httpsアクセス
	/// <summary>マイページメニュー表示判定</summary>
	public override bool DispMyPageMenu { get { return true; } }	// マイページメニュー表示

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// ログイン時のみ退会時用のスクリプトを出力（※2重出力しないようにするため）
		if (this.IsLoggedIn)
		{
			//------------------------------------------------------
			// アクセスログ用にユーザIDを格納
			//------------------------------------------------------
			// 退会後、セッションクリアを行うため一時的に変数にユーザIDを格納
			string strUserId = this.LoginUserId;

			//------------------------------------------------------
			// セッションクリア
			//------------------------------------------------------
			Session.RemoveAll();

			//------------------------------------------------------
			// アクセスログ用に退会時ステータス、ユーザIDを格納
			//------------------------------------------------------
			// AccessLogTrackerScript.ascx.cs内でステータスを参照し、退会時用のスクリプトを出力している
			// スクリプト出力と同時にSession[Constants.SESSION_KEY_W2MP_ACCESSLOG_STATUS]にnullを格納
			Session[Constants.SESSION_KEY_W2MP_ACCESSLOG_STATUS] = Constants.KBN_W2MP_ACCESSLOG_STATUS_LEAVE;
			Session[Constants.SESSION_KEY_W2MP_ACCESSLOG_LOGIN_USER_ID] = strUserId;
		}
	}

	/// <summary>
	/// トップページへボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbTopPage_Click(object sender, EventArgs e)
	{
		// トップページへ
		Response.Redirect(this.UnsecurePageProtocolAndHost + Constants.PATH_ROOT);
	}
}
