/*
=========================================================================================================
  Module      : NP After Pay Response(NPAfterPayResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Order.Payment.NPAfterPay
{
	/// <summary>
	/// NP After Pay Response
	/// </summary>
	public class NPAfterPayResponse
	{
		/// <summary>Results</summary>
		[JsonProperty(PropertyName = "results")]
		public Result[] Results { get; set; }
		/// <summary>Errors</summary>
		[JsonProperty(PropertyName = "errors")]
		public Error[] Errors { get; set; }
	}

	/// <summary>
	/// Result
	/// </summary>
	public class Result
	{
		/// <summary>NP取引ID</summary>
		[JsonProperty(PropertyName = "np_transaction_id")]
		public string NpTransactionId { get; set; }
		/// <summary>支払ステータス</summary>
		[JsonProperty(PropertyName = "payment_status")]
		public string PaymentStatus { get; set; }
		/// <summary>購入者支払日</summary>
		[JsonProperty(PropertyName = "customer_payment_date")]
		public string CustomerPaymentDate { get; set; }
		/// <summary>加盟店取引ID</summary>
		[JsonProperty(PropertyName = "shop_transaction_id")]
		public string ShopTransactionId { get; set; }
		/// <summary>利用照会結果</summary>
		[JsonProperty(PropertyName = "authori_result")]
		public string AuthoriResult { get; set; }
		/// <summary>与信NG事由</summary>
		[JsonProperty(PropertyName = "authori_ng")]
		public string AuthoriNg { get; set; }
		/// <summary>与信保留事由</summary>
		[JsonProperty(PropertyName = "authori_hold")]
		public string[] AuthoriHold { get; set; }
	}

	/// <summary>
	/// Error
	/// </summary>
	public class Error
	{
		/// <summary>エラーコード</summary>
		[JsonProperty(PropertyName = "codes")]
		public string[] Codes { get; set; }
		/// <summary>対象リソースID</summary>
		[JsonProperty(PropertyName = "id")]
		public string Id { get; set; }
	}
}
