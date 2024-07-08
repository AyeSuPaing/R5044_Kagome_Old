/*
=========================================================================================================
  Module      : LINEミニアプリ共通ヘッダー(BodyMiniAppHeader.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;

public partial class MiniApp_Form_Common_BodyMiniAppHeader : BaseUserControl
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
	}

	/// <summary>会員証を表示可能か</summary>
	protected bool CanDisplayMemberCard
	{
		get { return Constants.CROSS_POINT_OPTION_ENABLED && (this.IsMemberCardPage == false); }
	}
	/// <summary>会員証画面か</summary>
	protected bool IsMemberCardPage
	{
		get { return this.Request.Path == (Constants.PATH_ROOT + Constants.PAGE_LINE_MINIAPP_MEMBER_CARD); }
	}
}