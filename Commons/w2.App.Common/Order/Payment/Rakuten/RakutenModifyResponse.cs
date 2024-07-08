/*
=========================================================================================================
  Module      : Rakuten Modify Response(RakutenModifyResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;

namespace w2.App.Common.Order.Payment.Rakuten
{
	/// <summary>
	/// Rakuten Modify Response
	/// </summary>
	[Serializable]
	public class RakutenModifyResponse : RakutenResponseBase
	{
		/// <summary>Sub Service Id</summary>
		[JsonProperty(PropertyName = "subServiceId")]
		public string SubServiceId { get; set; }
		/// <summary>Service Reference Id</summary>
		[JsonProperty(PropertyName = "serviceReferenceId")]
		public string ServiceReferenceId { get; set; }
		/// <summary>Agency Code</summary>
		[JsonProperty(PropertyName = "agencyCode")]
		public string AgencyCode { get; set; }
		/// <summary>Payment Method Code</summary>
		[JsonProperty(PropertyName = "paymentMethodCode")]
		public string PaymentMethodCode { get; set; }
		/// <summary>Payment Status Type</summary>
		[JsonProperty(PropertyName = "paymentStatusType")]
		public string PaymentStatusType { get; set; }
		/// <summary>Request Type</summary>
		[JsonProperty(PropertyName = "requestType")]
		public string RequestType { get; set; }
		/// <summary>Gross Amount</summary>
		[JsonProperty(PropertyName = "grossAmount")]
		public string GrossAmount { get; set; }
		/// <summary>Currency Code</summary>
		[JsonProperty(PropertyName = "currencyCode")]
		public string CurrencyCode { get; set; }
		// <summary>Custom</summary>
		[JsonProperty(PropertyName = "custom")]
		public string Custom { get; set; }
		/// <summary>Transaction Time</summary>
		[JsonProperty(PropertyName = "transactionTime")]
		public string TransactionTime { get; set; }
		/// <summary>Agency Request Id</summary>
		[JsonProperty(PropertyName = "agencyRequestId")]
		public string AgencyRequestId { get; set; }
		/// <summary>Card</summary>
		[JsonProperty(PropertyName = "card")]
		public CardBase Card { get; set; }
	}
}
