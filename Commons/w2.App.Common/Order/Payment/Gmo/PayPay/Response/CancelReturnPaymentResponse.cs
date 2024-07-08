/*
=========================================================================================================
  Module      : キャンセル結果 (CancelReturnPaymentResponse.cs)
･････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Order.Payment.Paypay.Response
{
	/// <summary>
	/// キャンセル結果
	/// </summary>
	public class CancelReturnPaymentResponse : PaypayGmoResponse
	{
		/// <summary>決済注文ID</summary>
		[JsonProperty("orderID")]
		public string PaymentOrderId { get; set; }
		/// <summary>現状態</summary>
		[JsonProperty("Status")]
		public string Status { get; set; }
		/// <summary>Cancel Amount</summary>
		[JsonProperty("CancelAmount")]
		public string CancelAmount { get; set; }
		/// <summary>Cancel Tax</summary>
		[JsonProperty("CancelTax")]
		public string CancelTax { get; set; }
	}
}
