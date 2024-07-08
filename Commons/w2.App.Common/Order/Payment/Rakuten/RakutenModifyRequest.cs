/*
=========================================================================================================
  Module      : Rakuten Modify Request(RakutenModifyRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
namespace w2.App.Common.Order.Payment.Rakuten
{
	/// <summary>
	/// Rakuten Modify Request
	/// </summary>
	[Serializable]
	public class RakutenModifyRequest : RakutenRequestBase
	{
		/// <summary>Amount</summary>
		[JsonProperty(PropertyName = "amount")]
		public string Amount { get; set; }
	}
}
