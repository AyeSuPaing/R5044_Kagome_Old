/*
=========================================================================================================
  Module      : OrderImporterレスポンス (OrderImporterResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/

using Newtonsoft.Json;
using System;

namespace w2.App.Common.Recustomer.OrderImporter
{
	/// <summary>
	/// OrderImporterレスポンス
	/// </summary>
	[Serializable]
	public class OrderImporterResponse
	{
		/// <summary>results</summary>
		[JsonProperty("results")]
		public string Results { get; set; }
		/// <summary>message</summary>
		[JsonProperty("message")]
		public string Message { get; set; }
	}
}
