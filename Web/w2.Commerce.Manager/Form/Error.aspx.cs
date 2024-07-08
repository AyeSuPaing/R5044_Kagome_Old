/*
=========================================================================================================
  Module      : エラーページ処理(Error.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System.Web.Services;
using w2.Common.Logger;

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
			
			// その他エラー
			default:
				strErrorMessage = StringUtility.ToEmpty(Session[Constants.SESSION_KEY_ERROR_MSG]);
				break;
		}

		LBErrorMsg.Text = (strErrorMessage != "") ? StringUtility.ChangeToBrTag(strErrorMessage) : WebMessages.GetMessages(WebMessages.ERRMSG_SYSTEM_ERROR);

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
		trFrameTopLine.Visible = (this.IsPopUp == false);
	}

	/// <summary>
	/// Write log error for call ajax
	/// </summary>
	/// <param name="errorContent">Error content</param>
	[WebMethod]
	public static void WriteLogErrorForCallAjax(string errorContent)
	{
		if (string.IsNullOrEmpty(errorContent)) return;

		FileLogger.WriteError(errorContent);
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

