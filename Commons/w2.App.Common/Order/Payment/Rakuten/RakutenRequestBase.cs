/*
=========================================================================================================
  Module      : Rakuten Request Base(RakutenRequestBase.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using Newtonsoft.Json;
using System;

namespace w2.App.Common.Order.Payment.Rakuten
{
	/// <summary>
	/// Rakuten request base
	/// </summary>
	[Serializable]
	public class RakutenRequestBase
	{
		/// <summary>
		/// Construction
		/// </summary>
		public RakutenRequestBase()
		{ 
			this.ServiceId = Constants.PAYMENT_RAKUTEN_CREDIT_SERVICE_ID;
			this.SubServiceId = Constants.PAYMENT_RAKUTEN_CREDIT_SERVICE_ID;
			this.Timestamp = DateTime.UtcNow.ToString(Constants.RAKUTEN_DATE_FORMAT);
		}

		/// <summary>Service Id</summary>
		[JsonProperty(PropertyName = "serviceId")]
		public string ServiceId { get; set; }
		/// <summary>Sub Service Id</summary>
		[JsonProperty(PropertyName = "subServiceId")]
		public string SubServiceId { get; set; }
		/// <summary>Payment Id</summary>
		[JsonProperty(PropertyName = "paymentId")]
		public string PaymentId { get; set; }
		/// <summary>Timestamp</summary>
		[JsonProperty(PropertyName = "timestamp")]
		public string Timestamp { get; set; }
	}
}
