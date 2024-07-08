/*
=========================================================================================================
  Module      : Rakuten Response Base(RakutenResponseBase.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;

namespace w2.App.Common.Order.Payment.Rakuten
{
	/// <summary>
	/// Rakuten response base
	/// </summary>
	[Serializable]
	public class RakutenResponseBase
	{
		/// <summary>ResultType</summary>
		[JsonProperty(PropertyName = "resultType")]
		public string ResultType { get; set; }
		/// <summary>Service Id</summary>
		[JsonProperty(PropertyName = "serviceId")]
		public string ServiceId { get; set; }
		/// <summary>Payment Id</summary>
		[JsonProperty(PropertyName = "paymentId")]
		public string PaymentId { get; set; }
		/// <summary>Error Code</summary>
		[JsonProperty(PropertyName = "errorCode")]
		public string ErrorCode { get; set; }
		/// <summary>Error Message</summary>
		[JsonProperty(PropertyName = "errorMessage")]
		public string ErrorMessage { get; set; }
		/// <summary>Reference</summary>
		[JsonProperty(PropertyName = "reference")]
		public Reference Reference { get; set; }
		/// <summary>Service reference id</summary>
		[JsonProperty(PropertyName = "serviceReferenceId")]
		public string ServiceReferenceId { get; set; }
	}

	/// <summary>
	/// Card
	/// </summary>
	[JsonObject("Card")]
	public class CardBase
	{
		/// <summary>Card Token</summary>
		[JsonProperty(PropertyName = "cardToken")]
		public string CardToken { get; set; }
		/// <summary>Iin</summary>
		[JsonProperty(PropertyName = "iin")]
		public int Iin { get; set; }
		/// <summary>Last 4digits</summary>
		[JsonProperty(PropertyName = "last4digits")]
		public int Last4digits { get; set; }
		/// <summary>Expiration Month</summary>
		[JsonProperty(PropertyName = "expirationMonth")]
		public int ExpirationMonth { get; set; }
		/// <summary>Expiration Year</summary>
		[JsonProperty(PropertyName = "expirationYear")]
		public int ExpirationYear { get; set; }
		/// <summary>Brand Code</summary>
		[JsonProperty(PropertyName = "brandCode")]
		public string BrandCode { get; set; }
	}

	/// <summary>
	/// Reference
	/// </summary>
	[JsonObject("reference")]
	public class Reference
	{
		/// <summary>Card Token</summary>
		[JsonProperty(PropertyName = "hasMemberId")]
		public bool CardToken { get; set; }
		/// <summary>Rakuten card result</summary>
		[JsonProperty(PropertyName = "rakutenCardResult")]
		public RakutenCardResult RakutenCardResult { get; set; }
	}

	/// <summary>
	/// Rakuten card result
	/// </summary>
	[JsonObject("rakutenCardResult")]
	public class RakutenCardResult
	{
		/// <summary>Cvs info list</summary>
		[JsonProperty(PropertyName = "cvsInfoList")]
		public CvsInfoList[] CvsInfoList { get; set; }
	}

	/// <summary>
	/// Cvs info list
	/// </summary>
	[JsonObject("cvsInfoList")]
	public class CvsInfoList
	{
		/// <summary>Cvs code</summary>
		[JsonProperty(PropertyName = "cvsCode")]
		public string CvsCode { get; set; }
		/// <summary>Reference</summary>
		[JsonProperty(PropertyName = "reference")]
		public string Reference { get; set; }
	}
}
