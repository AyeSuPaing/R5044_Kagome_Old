/*
=========================================================================================================
  Module      : Base OMotion Response(BaseOMotionResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using w2.Common.Helper;
using w2.Common.Web;

namespace w2.App.Common.User.OMotion
{
	/// <summary>
	/// Base O-MOTION response
	/// </summary>
	public abstract class BaseOMotionResponse : IHttpApiResponseData
	{
		/// <summary>
		/// Create response string
		/// </summary>
		/// <returns>Response string</returns>
		public string CreateResponseString()
		{
			return SerializeHelper.Serialize(this);
		}

		/// <summary>Status</summary>
		[JsonProperty("status")]
		public int Status { get; set; }
		/// <summary>Message</summary>
		[JsonProperty("message")]
		public string Message { get; set; }
	}
}
