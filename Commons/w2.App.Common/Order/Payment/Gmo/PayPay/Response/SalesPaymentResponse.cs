/*
=========================================================================================================
  Module      : 実売上結果 (SalesPaymentResponse.cs)
･････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Order.Payment.Paypay.Response
{
	/// <summary>
	/// 実売上結果
	/// </summary>
	public class SalesPaymentResponse : PaypayGmoResponse
	{
		/// <summary>決済注文ID</summary>
		[JsonProperty("orderID")]
		public string PaymentOrderId { get; set; }
		/// <summary>現状態</summary>
		[JsonProperty("Status")]
		public string Status { get; set; }
		/// <summary>Amount</summary>
		[JsonProperty("Amount")]
		public string Amount { get; set; }
		/// <summary>Tax</summary>
		[JsonProperty("Tax")]
		public string Tax { get; set; }
	}
}
