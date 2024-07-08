/*
=========================================================================================================
  Module      : Paidy Token Object (PaidyTokenObject.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;

namespace w2.App.Common.Order.Payment.Paidy
{
	/// <summary>
	/// Paidy token object
	/// </summary>
	[Serializable]
	public class PaidyTokenObject
	{
		/// <summary>Token ID</summary>
		[JsonProperty(PropertyName = "id")]
		public string Id { get; set; }
		/// <summary>Status</summary>
		[JsonProperty(PropertyName = "status")]
		public string Status { get; set; }
	}
}
