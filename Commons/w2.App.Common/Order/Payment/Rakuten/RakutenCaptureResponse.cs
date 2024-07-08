/*
=========================================================================================================
  Module      : Rakuten Capture Response(RakutenCaptureResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Order.Payment.Rakuten
{
	/// <summary>
	/// Rakuten Capture Response
	/// </summary>
	public class RakutenCaptureResponse : RakutenResponseBase
	{
		/// <summary>transactionTime</summary>
		[JsonProperty(PropertyName = "transactionTime")]
		public string transactionTime { get; set; }
	}
}