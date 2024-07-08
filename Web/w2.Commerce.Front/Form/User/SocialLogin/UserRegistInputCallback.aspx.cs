/*
=========================================================================================================
  Module      : ソーシャルログイン 会員情報入力コールバック画面(UserRegistInputCallback.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;

/// <summary>
/// ソーシャルログイン 会員情報入力コールバック画面
/// </summary>
public partial class Form_User_SocialLogin_LoginCallback : SocialLoginPage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_REGIST_INPUT, this.NextUrl);
	}
}