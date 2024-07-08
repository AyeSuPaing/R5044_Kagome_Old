/*
=========================================================================================================
  Module      : LINEミニアプリ会員証画面(MemberCard.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Web;

public partial class MiniApp_User_MemberCard : LineMiniAppPage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
	}

	/// <summary>
	/// 翌遷移URL取得
	/// </summary>
	/// <returns>遷移URL</returns>
	protected string GetBackUrl()
	{
		var url = this.IsTempLoggedIn
			? Constants.PATH_ROOT + Constants.PAGE_LINE_MINIAPP_LOGIN_COOPERATION
			: Constants.PATH_ROOT + Constants.PAGE_LINE_MINIAPP_TOP;

		return HtmlSanitizer.UrlAttrHtmlEncode(url);
	}
}