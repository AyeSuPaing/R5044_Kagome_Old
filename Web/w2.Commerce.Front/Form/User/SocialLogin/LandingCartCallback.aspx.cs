/*
=========================================================================================================
  Module      : ソーシャルログイン ランディングカートコールバック画面(LandingCartCallback.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;

/// <summary>
/// ソーシャルログイン ランディングカートコールバック画面
/// </summary>
public partial class Form_User_SocialLogin_LandingCartCallback : SocialLoginPage
{
	protected void Page_Load(object sender, EventArgs e)
	{
		var returnPath = Request[Constants.REQUEST_KEY_RETURN_URL] ?? Constants.PATH_ROOT;

		Redirect(returnPath, returnPath);
	}
}