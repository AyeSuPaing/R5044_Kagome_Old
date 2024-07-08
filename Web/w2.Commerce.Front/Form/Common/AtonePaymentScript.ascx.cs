/*
=========================================================================================================
  Module      : Atone Payment Script(AtonePaymentScript.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using w2.App.Common.Order.Payment.Atone;
public partial class Form_Common_AtonePaymentScript : AtoneAfteeControl
{
	#region Properties
	/// <summary>Java Script Code</summary>
	protected string JavaScriptCode
	{
		get
		{
			var result = AtonePaymentApiFacade.GetJavascriptPaymentAtone();
			return result;
		}
	}
	#endregion
}