/*
=========================================================================================================
  Module      : Line Pay Confirm Payment Request (LinePayConfirmPaymentRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;

namespace w2.App.Common.Order.Payment.LinePay
{
	/// <summary>
	/// LINE Pay Confirm Payment Request
	/// </summary>
	[Serializable]
	public class LinePayConfirmPaymentRequest
	{
		/// <summary>Amount</summary>
		[JsonProperty(PropertyName = "amount")]
		public decimal Amount { get; set; }
		/// <summary>Currency</summary>
		[JsonProperty(PropertyName = "currency")]
		public string Currency { get; set; }
	}
}
