/*
=========================================================================================================
  Module      : Authori Feedback Response(AuthoriFeedbackResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

using System;
using Newtonsoft.Json;

namespace w2.App.Common.User.OMotion.AuthoriFeedback
{
	/// <summary>
	/// Authori feedback response
	/// </summary>
	[Serializable]
	public class AuthoriFeedbackResponse : BaseOMotionResponse
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public AuthoriFeedbackResponse()
			: base()
		{
		}

		/// <summary>Status</summary>
		[JsonProperty("status")]
		public int Status{ get; set; }
		/// <summary>Message</summary>
		[JsonProperty("message")]
		public string Message { get; set; }
	}
}
