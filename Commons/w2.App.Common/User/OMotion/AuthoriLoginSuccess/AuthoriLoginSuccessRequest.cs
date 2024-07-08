/*
=========================================================================================================
  Module      : Authori Login Success Request(AuthoriLoginSuccessRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Net;

namespace w2.App.Common.User.OMotion.AuthoriLoginSuccess
{
	/// <summary>
	/// Authori login success request
	/// </summary>
	public class AuthoriLoginSuccessRequest : BaseOMotionRequest
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="authoriId">Authori id</param>
		/// <param name="loginId">Login id</param>
		/// <param name="value">Value</param>
		public AuthoriLoginSuccessRequest(string authoriId, string loginId, bool value)
			: base(authoriId, string.Format(Constants.OMOTION_REQUEST_AUTHORI_LOGIN_SUCCESS_URL, authoriId), loginId)
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
			//this.Posts.Add("connected_id", this.ConnectedId);
		}

		/// <summary>Value</summary>
		public bool Value { get; set; }
		/// <summary>Connected id</summary>
		public string ConnectedId { get; set; }
	}
}
