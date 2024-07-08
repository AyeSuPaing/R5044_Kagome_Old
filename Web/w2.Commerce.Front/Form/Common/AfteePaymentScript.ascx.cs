/*
=========================================================================================================
  Module      : Aftee Payment Script(AfteePaymentScript.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using w2.App.Common.Order.Payment.Aftee;
public partial class Form_Common_AfteePaymentScript : AtoneAfteeControl
{
	#region Properties
	/// <summary>Java Script Code</summary>
	protected string JavaScriptCode
	{
		get
		{
			var result = AfteePaymentApiFacade.GetJavascriptPaymentAftee();
			return result;
		}
	}
	#endregion
}