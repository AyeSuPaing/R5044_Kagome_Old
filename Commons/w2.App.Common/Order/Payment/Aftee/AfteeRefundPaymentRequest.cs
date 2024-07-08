/*
=========================================================================================================
  Module      : Aftee Refund Payment Request (AfteeRefundPaymentRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;

namespace w2.App.Common.Order.Payment.Aftee
{
	/// <summary>
	/// Aftee refund request object
	/// </summary>
	[Serializable]
	public class AfteeRefundPaymentRequest
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
