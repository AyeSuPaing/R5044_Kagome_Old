/*
=========================================================================================================
  Module      : Rakuten Authorize Respone(RakutenAuthoireRespone.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Order.Payment.Rakuten.Authorize
{
	/// <summary>
	/// Rakuten Authorize Response
	/// </summary>
	public class RakutenAuthorizeResponse : RakutenResponseBase
	{
		/// <summary>Sub Service Id</summary>
		[JsonProperty(PropertyName = "subServiceId")]
		public string SubServiceId { get; set; }
		/// <summary>Agency Code</summary>
		[JsonProperty(PropertyName = "agencyCode")]
		public string AgencyCode { get; set; }
		/// <summary>Agency Request Id</summary>
		[JsonProperty(PropertyName = "agencyRequestId")]
		public string AgencyRequestId { get; set; }
		/// <summary>Card</summary>
		[JsonProperty(PropertyName = "card")]
		public Card Card { get; set; }
		/// <summary>Transaction time</summary>
		[JsonProperty(PropertyName = "transactionTime")]
		public string TransactionTime { get; set; }
	}

	public class Card : CardBase
	{
		/// <summary>Installments</summary>
		[JsonProperty(PropertyName = "installments")]
		public int Installments { get; set; }
		/// <summary>With Bonus</summary>
		[JsonProperty(PropertyName = "withBonus")]
		public bool WithBonus { get; set; }
		/// <summary>With Revolving</summary>
		[JsonProperty(PropertyName = "withRevolving")]
		public bool WithRevolving { get; set; }
		/// <summary>Is Rakuten Card</summary>
		[JsonProperty(PropertyName = "isRakutenCard")]
		public bool IsRakutenCard { get; set; }
	}
}