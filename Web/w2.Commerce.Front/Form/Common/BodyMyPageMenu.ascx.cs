/*
=========================================================================================================
  Module      :  MYページメニュー出力コントローラ処理(BodyMyPageMenu.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;

public partial class Form_Common_BodyMyPageMenu : BaseUserControl
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// なにもしない
	}

	/// <summary>受信メール履歴機能が有効か</summary>
	public bool DisplayMailSendLogs
	{
		get{ return Constants.MYPAGE_RECIEVEMAIL_HISTORY_DISPLAY && Constants.CS_OPTION_ENABLED; }
	}
}
