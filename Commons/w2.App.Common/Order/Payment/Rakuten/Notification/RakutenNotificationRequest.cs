/*
=========================================================================================================
  Module      : Rakuten Notification Request(RakutenNotificationRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Order.Payment.Rakuten.Notification
{
	/// <summary>
	/// Rakuten notification request
	/// </summary>
	public class RakutenNotificationRequest : RakutenRequestBase
	{
		/// <summary>Service reference id</summary>
		[JsonProperty(PropertyName = "serviceReferenceId")]
		public string ServiceReferenceId { get; set; }
		/// <summary>Agency code</summary>
		[JsonProperty(PropertyName = "agencyCode")]
		public string AgencyCode { get; set; }
		/// <summary>Agency request id</summary>
		[JsonProperty(PropertyName = "agencyRequestId")]
		public string AgencyRequestId { get; set; }
		/// <summary>Error code</summary>
		[JsonProperty(PropertyName = "errorCode")]
		public string ErrorCode { get; set; }
		/// <summary>Error message</summary>
		[JsonProperty(PropertyName = "errorMessage")]
		public string ErrorMessage { get; set; }
		/// <summary>Transaction time</summary>
		[JsonProperty(PropertyName = "transactionTime")]
		public string TransactionTime { get; set; }
		/// <summary>Payment status type</summary>
		[JsonProperty(PropertyName = "paymentStatusType")]
		public string PaymentStatusType { get; set; }
		/// <summary>Request type</summary>
		[JsonProperty(PropertyName = "requestType")]
		public string RequestType { get; set; }
		/// <summary>Gross amount</summary>
		[JsonProperty(PropertyName = "grossAmount")]
		public string GrossAmount { get; set; }
		/// <summary>Currency code</summary>
		[JsonProperty(PropertyName = "currencyCode")]
		public string CurrencyCode { get; set; }
		/// <summary>Custom</summary>
		[JsonProperty(PropertyName = "custom")]
		public string Custom { get; set; }
		/// <summary>Reference</summary>
		[JsonProperty(PropertyName = "reference")]
		public Reference Reference { get; set; }
		/// <summary>Result type</summary>
		[JsonProperty(PropertyName = "resultType")]
		public string ResultType { get; set; }
	}
}
