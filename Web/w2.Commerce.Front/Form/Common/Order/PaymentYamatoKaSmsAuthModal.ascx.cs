/*
=========================================================================================================
  Module      : ヤマト決済(後払い) SMS認証用モーダル(PaymentYamatoKaSmsAuthModal.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;

public partial class Form_Common_Order_PaymentYamatoKaSmsAuthModal : PaymentYamatoKaSmsAuthModalBase
{
	/// <summary>
	/// 認証するボタン
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbAuthorize_OnClick(object sender, EventArgs e)
	{
		var isNg = bool.Parse(hfIsNg.Value);
		if (isNg)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] =
				WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_AUTH_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		this.OnAuthorizeComplete(sender, e);
	}
}
