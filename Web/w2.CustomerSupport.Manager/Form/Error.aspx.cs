/*
=========================================================================================================
  Module      : エラーページ処理(Error.aspx.cs)
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

public partial class Form_Error : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{
		// ブラウザーキャッシュ削除
		ClearBrowserCache();

		// エラー文言設定
		string strErrorMessage = null;
		switch (StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ERRORPAGE_MANAGER_ERRORKBN]))
		{
			// 制御文字入力エラー
			case WebMessages.ERRMSG_SYSTEM_VALIDATION_ERROR:
				strErrorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_SYSTEM_VALIDATION_ERROR);
				break;

			// システムエラー（集約エラーハンドラ内ではセッションが使えないこともあるので）
			case WebMessages.ERRMSG_SYSTEM_ERROR:
				strErrorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_SYSTEM_ERROR);
				// UNDONE:IISでStatusCodeが補足されWeb側のエラーページが出力されてしまうのでコメント化（WindowsServer2008 IE7.5で再現）
				// Response.StatusCode = 500;
				break;

			// 404エラー
			case WebMessages.ERRMSG_404_ERROR:
				strErrorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_404_ERROR);
				// UNDONE:IISでStatusCodeが補足されWeb側のエラーページが出力されてしまうのでコメント化（WindowsServer2008 IE7.5で再現）
				// Response.StatusCode = 404;
				break;

			case WebMessages.ERRMSG_MANAGER_URL_FORMAT_ERROR:
				strErrorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_URL_FORMAT_ERROR);
				break;

			// 受信時振分けのときメールがないエラー
			case WebMessages.MSG_MANAGER_MAIL_ASSIGN_NO_MAIL:
				strErrorMessage = WebMessages.GetMessages(WebMessages.MSG_MANAGER_MAIL_ASSIGN_NO_MAIL);
				break;

			// その他エラー
			default:
				strErrorMessage = StringUtility.ToEmpty(Session[Constants.SESSION_KEY_ERROR_MSG]);
				break;
		}

		LBErrorMsg.Text = (strErrorMessage != "") ? strErrorMessage : WebMessages.GetMessages(WebMessages.ERRMSG_SYSTEM_ERROR);

		string strErrorStatus = Request.QueryString[Constants.REQUEST_KEY_ERROR_STATUS];
		switch (strErrorStatus)
		{
			case "notlogin":
				BTNLogin.Visible = true;
				break;
			case "close":
				break;
			default:
				DIVHistoryBack.Visible = true;
				break;
		}

		// ポップアップ表示制御（タイトルを非表示へ）
		trFrameTopLine.Visible = (Request[Constants.REQUEST_KEY_WINDOW_KBN] != Constants.KBN_WINDOW_POPUP);
	}

	/// <summary>
	/// ログイン画面へボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void BTNLogin_Click(object sender, System.EventArgs e)
	{
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_LOGIN,false);
	}
}