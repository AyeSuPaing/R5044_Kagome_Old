/*
=========================================================================================================
  Module      : PayPalログインスクリプト処理 (PayPalScriptsFormBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Web.UI.WebControls;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment.PayPal;
using w2.App.Common.Web.WrappedContols;

/// <summary>
/// PayPalログインスクリプト処理
/// </summary>
public class PayPalScriptsFormBase : BaseUserControl
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
	}

	/// <summary>認証完了アクションコントロール</summary>
	public WebControl AuthCompleteActionControl { get; set; }
	/// <summary>クライアントトークン</summary>
	public string ClientToken
	{
		get { return PayPalUtility.Account.CreateClientToken(); }
	}
	/// <summary>PayPalナンス</summary>
	public string PayPalNonce { get { return GetWrappedControl<WrappedHiddenField>("hfPayPalNonce").Value; } }
	/// <summary>PayPal PayerId</summary>
	public string PayPalPayerId { get { return GetWrappedControl<WrappedHiddenField>("hfPayPalPayerId").Value; } }
	/// <summary>PayPal DeviceData</summary>
	public string PayPalDeviceData { get { return GetWrappedControl<WrappedHiddenField>("hfPayPalDeviceData").Value; } }
	/// <summary>PayPal 配送先</summary>
	public string PayPalShippingAddress { get { return GetWrappedControl<WrappedHiddenField>("hfPayPalShippingAddress").Value; } }
	/// <summary>カート</summary>
	public CartObject Cart { get; set; }
	/// <summary>ロゴデザイン</summary>
	public string LogoDesign { get; set; }
	/// <summary>配送先取得するか</summary>
	public bool GetShippingAddress { get; set; }
}