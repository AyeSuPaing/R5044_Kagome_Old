/*
=========================================================================================================
  Module      : VeriTrans PayPay Auth (VeriTransPayPayAuth.aspx.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Order.Payment.Veritrans;

public partial class VeriTransPayPayAuth : OrderCartPageExternalPayment
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		var html = (string)Session[VeriTransConst.RESPONSE_CONTENTS];
		Response.Write(html);
	}
}
