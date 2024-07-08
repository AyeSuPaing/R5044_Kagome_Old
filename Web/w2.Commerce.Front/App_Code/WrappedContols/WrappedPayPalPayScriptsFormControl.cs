/*
=========================================================================================================
  Module      : ラップ済みペイパルスクリプトフォームクラス(WrappedPayPalPayScriptsFormControl.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using w2.App.Common.Web.WrappedContols;

/// <summary>
/// ラップ済みペイパルスクリプトフォームクラス
/// </summary>
public class WrappedPayPalPayScriptsFormControl : WrappedControl
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="parentControlID">親コントロールID</param>
	/// <param name="controlID">コントロールID</param>
	/// <param name="hfForm">コントロールが属するForm</param>
	/// <param name="stateBagViewState">ViewState</param>
	public WrappedPayPalPayScriptsFormControl(
		string parentControlID,
		string controlID,
		HtmlForm hfForm,
		StateBag stateBagViewState)
		: base(
			parentControlID,
			controlID,
			hfForm,
			stateBagViewState)
	{
		// なにもしない //
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="parentControlID">親コントロールID</param>
	/// <param name="controlID">コントロールID</param>
	/// <param name="hfForm">コントロールが属するForm</param>
	/// <param name="stateBagViewState">ViewState</param>
	/// <param name="defaultValue">デフォルト値</param>
	public WrappedPayPalPayScriptsFormControl(
		string parentControlID,
		string controlID,
		HtmlForm hfForm,
		StateBag stateBagViewState,
		string defaultValue)
		: base(
			parentControlID,
			controlID,
			hfForm,
			stateBagViewState,
			defaultValue)
	{
		// なにもしない //
	}

	/// <summary>PayPalNonce</summary>
	public string PayPalNonce { get { return this.HasInnerControl ? this.InnerControl.PayPalNonce : ""; } }
	/// <summary>PayPalPayerId</summary>
	public string PayPalPayerId { get { return this.HasInnerControl ? this.InnerControl.PayPalPayerId : ""; } }
	/// <summary>PayPalDeviceData</summary>
	public string PayPalDeviceData { get { return this.HasInnerControl ? this.InnerControl.PayPalDeviceData : ""; } }
	/// <summary>PayPalShippingAddress</summary>
	public string PayPalShippingAddress { get { return this.HasInnerControl ? this.InnerControl.PayPalShippingAddress : ""; } }
	/// <summary>内部コントロール取得</summary>
	public new PayPalScriptsFormBase InnerControl { get { return (PayPalScriptsFormBase)base.InnerControl; } }
}