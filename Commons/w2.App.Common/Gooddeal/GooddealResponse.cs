/*
=========================================================================================================
  Module      : Gooddeal response (GooddealResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Gooddeal
{
	/// <summary>
	/// Gooddeal response
	/// </summary>
	public class GooddealResponse
	{
		/// <summary>Status</summary>
		[JsonProperty("Status")]
		public string Status { set; get; }
		/// <summary>Order no</summary>
		[JsonProperty("OrderNo")]
		public string OrderNo { set; get; }
		/// <summary>Error message</summary>
		[JsonProperty("Messenger")]
		public string ErrorMessage { get; set; }
		/// <summary>Register error code</summary>
		[JsonProperty("Errcode")]
		public string RegisterErrorCode { set; get; }
		/// <summary>Error code</summary>
		[JsonProperty("Ercode")]
		public string ErrorCode { set; get; }
		/// <summary> Date time</summary>
		[JsonProperty("DateTime")]
		public string DateTime { get; set; }
		/// <summary>Deliver no</summary>
		[JsonProperty("Deliver_no")]
		public string DeliverNo { get; set; }
		/// <summary>Deliver type</summary>
		[JsonProperty("DeliverType")]
		public string DeliverType { set; get; }
	}
}