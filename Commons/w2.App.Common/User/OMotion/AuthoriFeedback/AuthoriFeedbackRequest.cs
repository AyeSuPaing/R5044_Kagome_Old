/*
=========================================================================================================
  Module      : Authori Feedback Request(AuthoriFeedbackRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Net;

namespace w2.App.Common.User.OMotion.AuthoriFeedback
{
	/// <summary>
	/// Authori feedback request
	/// </summary>
	public class AuthoriFeedbackRequest : BaseOMotionRequest
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="authoriId">Authori id</param>
		/// <param name="loginId">Login id</param>
		/// <param name="value">Value</param>
		public AuthoriFeedbackRequest(string authoriId, string loginId, string value)
			: base(authoriId, string.Format(Constants.OMOTION_REQUEST_AUTHORI_FEEDBACK_URL, authoriId), loginId)
		{
			this.MethodType = WebRequestMethods.Http.Post;

			this.Value = value;
			SetPostString();
		}

		/// <summary>
		/// Create headers
		/// </summary>
		public override void CreateHeaders()
		{
			base.CreateHeaders();

			this.Headers.Add(OMotionConstants.REQUEST_HEADER_KEY_METHODOVERRIDE, "PATCH");
		}

		/// <summary>
		/// Create posts
		/// </summary>
		public override void CreatePosts()
		{
			this.Posts.Add("value", this.Value);
		}

		/// <summary>Value</summary>
		public string Value { get; set; }
	}
}
