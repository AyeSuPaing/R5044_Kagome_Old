/*
=========================================================================================================
  Module      : ベリトランス3Dセキュア送信画面 (PostCard3DSecureAuth.aspx.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Order.Payment.Veritrans;

/// <summary>
/// ベリトランス3Dセキュア送信画面
/// </summary>
public partial class PostCard3DSecureAuth : OrderCartPageExternalPayment
{
	/// <summary>
	/// Page load
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		var htmlStr = (string)Session[VeriTransConst.RESPONSE_CONTENTS];
		Response.Write(htmlStr);
	}
}
