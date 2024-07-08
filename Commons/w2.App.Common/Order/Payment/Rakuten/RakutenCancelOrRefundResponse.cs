/*
=========================================================================================================
  Module      : Rakuten CancelOrRefund Response(RakutenCancelOrRefundResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using Newtonsoft.Json;

namespace w2.App.Common.Order.Payment.Rakuten
{
	/// <summary>
	/// Rakuten CancelOrRefund Response
	/// </summary>
	[Serializable]
	public class RakutenCancelOrRefundResponse : RakutenResponseBase
	{
		/// <summary>Transaction Time</summary>
		[JsonProperty(PropertyName = "transactionTime")]
		public string TransactionTime { get; set; }
	}
}
