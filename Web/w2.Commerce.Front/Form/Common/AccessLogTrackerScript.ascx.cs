/*
=========================================================================================================
  Module      : トラッカー出力ユーザコントローラ処理(AccessLogTrackerScript.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;

public partial class Form_Common_AccessLogTrackerScript : BaseUserControl
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// アクセスログが有効な場合
		if ((Constants.W2MP_ACCESSLOG_ENABLED) && (DateTime.Now >= Constants.W2MP_ACCESSLOG_BEGIN_DATETIME))
		{
			// トラッカー表示
			divTracker.Visible = true;

			switch (StringUtility.ToEmpty(Session[Constants.SESSION_KEY_W2MP_ACCESSLOG_STATUS]))
			{
				// ログイン時用ログスクリプト表示
				case Constants.KBN_W2MP_ACCESSLOG_STATUS_LOGIN:
					// Login.aspx.csでステータスを格納
					// スクリプト出力と同時にステータス情報を初期化(初回のみログイン時スクリプトを出力したいため)
					divGetLogForLogin.Visible = true;
					Session[Constants.SESSION_KEY_W2MP_ACCESSLOG_STATUS] = null;
					break;

				// 退会時用ログスクリプト表示
				case Constants.KBN_W2MP_ACCESSLOG_STATUS_LEAVE:
					// UserWithdrawalInput.aspx.csでステータスを格納
					// スクリプト出力と同時にステータス情報を初期化(初回のみ退会時スクリプトを出力したいため)
					divGetLogForLeave.Visible = true;
					Session[Constants.SESSION_KEY_W2MP_ACCESSLOG_STATUS] = null;
					break;

				// ログスクリプト表示
				default:
					divGetLog.Visible = true;
					break;
			}
		}
	}
}
