/*
=========================================================================================================
  Module      : Order Register Response(OrderRegisterResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;

namespace w2.App.Common.User.Botchan
{
	/// <summary>
	/// Order register response
	/// </summary>
	[Serializable]
	public class RegisterResponse
	{
		/// <summary>Result</summary>
		[JsonProperty("result")]
		public bool Result { get; set; }
		/// <summary>Message id</summary>
		[JsonProperty("message_id")]
		public string MessageId { set; get; }
		/// <summary>Message</summary>
		[JsonProperty("message")]
		public string Message { set; get; }
		/// <summary>Data</summary>
		[JsonProperty("data")]
		public RegisterResponseData Data { get; set; }
	}

	/// <summary>
	/// Data
	/// </summary>
	[Serializable]
	public class RegisterResponseData
	{
		/// <summary>Order id</summary>
		[JsonProperty("user_id")]
		public string UserId { get; set; }
	}
}
