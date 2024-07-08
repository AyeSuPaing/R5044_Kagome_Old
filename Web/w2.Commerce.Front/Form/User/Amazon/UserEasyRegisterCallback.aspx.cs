/*
=========================================================================================================
  Module      : Amazon かんたん会員登録コールバック画面(UserEasyRegisterCallback.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Amazon;
using w2.App.Common.Amazon.Helper;
using w2.App.Common.Amazon.Util;

/// <summary>
/// Amazon かんたん会員登録コールバック画面
/// </summary>
public partial class Form_User_Amazon_UserEasyRegisterCallback : AmazonLoginPage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (IsPostBack)
		{
			// 初期処理
			InitPage();

			// 既に他のユーザと連携済のAmazonアカウントの場合はエラーとする
			var w2User = AmazonUtil.GetUserByAmazonUserId(this.AmazonModel.UserId);
			if (w2User != null)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_USER_COOPERATED_AMAZON_USER);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}

			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_EASY_REGIST_INPUT);
		}
	}
}