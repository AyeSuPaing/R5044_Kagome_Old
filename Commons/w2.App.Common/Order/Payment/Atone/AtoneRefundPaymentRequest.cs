/*
=========================================================================================================
  Module      : Atone Refund Payment Request (AtoneRefundPaymentRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;

namespace w2.App.Common.Order.Payment.Atone
{
	/// <summary>
	/// Atone refund request object
	/// </summary>
	[Serializable]
	public class AtoneRefundPaymentRequest
	{
		/// <summary>Amount refund</summary>
		[JsonProperty(PropertyName = "amount_refund")]
		public string AmountRefund { get; set; }
		/// <summary>Refund reason</summary>
		[JsonProperty(PropertyName = "refund_reason")]
		public string RefundReason { get; set; }
		/// <summary>Description refund</summary>
		[JsonProperty(PropertyName = "description_refund")]
		public string DescriptionRefund { get; set; }
	}
}
