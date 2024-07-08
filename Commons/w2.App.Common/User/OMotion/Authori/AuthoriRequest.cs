/*
=========================================================================================================
  Module      : Authori Request(AuthoriRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Net;

namespace w2.App.Common.User.OMotion.Authori
{
	/// <summary>
	/// Authori request
	/// </summary>
	public class AuthoriRequest : BaseOMotionRequest
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="authoriId">Authori id</param>
		/// <param name="loginId">Login id</param>
		public AuthoriRequest(string authoriId, string loginId)
			: base(authoriId, string.Format(Constants.OMOTION_REQUEST_AUTHORI_URL, authoriId), loginId)
		{
			this.MethodType = WebRequestMethods.Http.Get;

			SetPostString();
		}
	}
}
