/*
=========================================================================================================
  Module      : ソーシャルログイン かんたん会員登録コールバック画面(UserEasyRegisterCallback.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;

/// <summary>
/// ソーシャルログイン かんたん会員登録コールバック画面
/// </summary>
public partial class Form_User_SocialLogin_UserEasyRegisterCallback : SocialLoginPage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_EASY_REGIST_INPUT, Constants.PATH_ROOT);
	}
}