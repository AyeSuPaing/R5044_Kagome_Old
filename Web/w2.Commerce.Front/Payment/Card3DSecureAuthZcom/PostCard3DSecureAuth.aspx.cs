/*
=========================================================================================================
  Module      : Post Card 3DSecure Auth (PostCard3DSecureAuth.aspx.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Order.Payment.GMO.Zcom;

/// <summary>
/// Payment card 3DSecure auth Zcom post card 3DSecure auth
/// </summary>
public partial class Payment_Card3DSecureAuthZcom_PostCard3DSecureAuth : OrderCartPageExternalPayment
{
	/// <summary>
	/// Page load
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		this.AccessUrl = this.Request[ZcomConst.PARAM_ACCESS_URL];
		this.PaymentCode = this.Request[ZcomConst.PARAM_PAYMENT_CODE];
		this.TransCode = this.Request[ZcomConst.PARAM_TRANS_CODE_HASH];
		this.Mode = this.Request[ZcomConst.PARAM_MODE];
	}

	/// <summary>Access url</summary>
	protected string AccessUrl { get; set; }
	/// <summary>Payment code</summary>
	protected string PaymentCode { get; set; }
	/// <summary>Trans code</summary>
	protected string TransCode { get; set; }
	/// <summary>Mode</summary>
	protected string Mode { get; set; }
}
