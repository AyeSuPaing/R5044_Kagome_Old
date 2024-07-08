/*
=========================================================================================================
  Module      : Paidy Checkout Base Control (PaidyCheckoutBaseControl.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;

/// <summary>
/// Paidy checkout base control
/// </summary>
public class PaidyCheckoutBaseControl : BaseUserControl
{
	/// <summary>Display user control</summary>
	private const string DISPLAY_USER_CONTROL = "DisplayUserControl";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
	}

	/// <summary>Display User Control</summary>
	public bool DisplayUserControl
	{
		get { return (ViewState[DISPLAY_USER_CONTROL] != null) ? (bool)ViewState[DISPLAY_USER_CONTROL] : false; }
		set { ViewState[DISPLAY_USER_CONTROL] = value; }
	}
}