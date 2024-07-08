/*
=========================================================================================================
  Module      : Wrapped Paidy Checkout Control (WrappedPaidyCheckoutControl.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Web.UI;
using System.Web.UI.HtmlControls;
using w2.App.Common.Web.WrappedContols;

/// <summary>
/// Wrapped Paidy checkout control
/// </summary>
public class WrappedPaidyCheckoutControl : WrappedControl
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="parentControlId">親コントロールID</param>
	/// <param name="controlId">コントロールID</param>
	/// <param name="hfForm">コントロールが属するForm</param>
	/// <param name="viewState">ViewState</param>
	/// <param name="defaultValue">デフォルト値</param>
	public WrappedPaidyCheckoutControl(
		string parentControlId,
		string controlId,
		HtmlForm htmlForm,
		StateBag viewState)
		: base(parentControlId, controlId, htmlForm, viewState, string.Empty)
	{
	}

	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="parentControlId">親コントロールID</param>
	/// <param name="controlId">コントロールID</param>
	/// <param name="hfForm">コントロールが属するForm</param>
	/// <param name="viewState">ViewState</param>
	/// <param name="defaultValue">デフォルト値</param>
	public WrappedPaidyCheckoutControl(
		string parentControlId,
		string controlId,
		HtmlForm htmlForm,
		StateBag viewState,
		string defaultValue)
		: base(parentControlId, controlId, htmlForm, viewState, defaultValue)
	{
	}

	/// <summary>Display User Control</summary>
	public bool DisplayUserControl
	{
		get { return (this.InnerControl != null) ? this.InnerControl.DisplayUserControl : false; }
		set { if (this.InnerControl != null) this.InnerControl.DisplayUserControl = value; }
	}

	/// <summary>Paidy inner control</summary>
	public new PaidyCheckoutBaseControl InnerControl { get { return (PaidyCheckoutBaseControl)base.InnerControl; } }
}