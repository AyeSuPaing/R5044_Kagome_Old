/*
=========================================================================================================
  Module      : ソーシャルログイン 注文者決定コールバック画面(OrderCallback.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Web;

/// <summary>
/// ソーシャルログイン 注文者決定コールバック画面
/// </summary>
public partial class Form_User_SocialLogin_OrderCallback : SocialLoginPage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_LOGIN)
			.AddParam(Constants.REQUEST_KEY_NEXT_URL, Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SHIPPING)
			.CreateUrl();
		Redirect(url, Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SHIPPING);
	}
}