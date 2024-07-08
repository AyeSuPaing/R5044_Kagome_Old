/*
=========================================================================================================
  Module      : Facebook Conversion Response(FacebookConversionResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;

namespace w2.App.Common.FacebookConversion
{
	/// <summary>
	/// Facebook conversion response
	/// </summary>
	[Serializable]
	public class FacebookConversionResponse
	{
		/// <summary>Events received</summary>
		[JsonProperty("events_received")]
		public int? EventsReceived { get; set; }
		/// <summary>Messages success</summary>
		[JsonProperty("messages")]
		public string[] Messages { get; set; }
		/// <summary>Fb trace id</summary>
		[JsonProperty("fbtrace_id")]
		public string FbTraceId { get; set; }
		/// <summary>Convert facebook respone error</summary>
		[JsonProperty("error")]
		public FacebookConversionResponseError ErrorInfo { get; set; }

		/// <summary>
		/// Facebook conversion response error
		/// </summary>
		[Serializable]
		public class FacebookConversionResponseError
		{
			/// <summary>Message</summary>
			[JsonProperty("message")]
			public string Message { get; set; }
			/// <summary>Type Error</summary>
			[JsonProperty("type")]
			public string Type { get; set; }
			/// <summary>Code error</summary>
			[JsonProperty("code")]
			public int Code { get; set; }
			/// <summary>Error sub code</summary>
			[JsonProperty("error_subcode")]
			public int ErrorSubCode { get; set; }
			/// <summary>Error user title</summary>
			[JsonProperty("error_user_title")]
			public string ErrorUserTitle { get; set; }
			/// <summary>Error user message</summary>
			[JsonProperty("error_user_msg")]
			public string ErrorUserMsg { get; set; }
			/// <summary>Fb trace id</summary>
			[JsonProperty("fbtrace_id")]
			public string FbTraceId { get; set; }
		}
	}
}
