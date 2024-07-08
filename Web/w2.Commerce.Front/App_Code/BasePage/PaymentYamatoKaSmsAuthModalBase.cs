/*
=========================================================================================================
  Module      : ヤマト決済(後払い) SMS認証用モーダル基底(PaymentYamatoKaSmsAuthModalBase.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;

/// <summary>
/// ヤマト決済(後払い) SMS認証用モーダル基底
/// </summary>
public class PaymentYamatoKaSmsAuthModalBase : BaseUserControl
{
	/// <summary>電話番号</summary>
	public string TelNum
	{
		get { return (string)ViewState["TelNum"]; }
		set { ViewState["TelNum"] = value; }
	}
	/// <summary>認証完了時処理</summary>
	public EventHandler OnAuthorizeComplete { get; set; }
}