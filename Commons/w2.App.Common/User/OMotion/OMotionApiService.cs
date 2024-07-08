/*
=========================================================================================================
  Module      : OMotion Api Service (OMotionApiService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.User.OMotion
{
	/// <summary>
	/// OMotion api service
	/// </summary>
	public class OMotionApiService
	{
		#region +Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="authoriId">Authori id</param>
		/// <param name="loginId">Login id</param>
		public OMotionApiService(string authoriId, string loginId)
		{
			this.AuthoriId = authoriId;
			this.LoginId = loginId;
		}
		#endregion

		#region +Method
		/// <summary>
		/// Authori
		/// </summary>
		/// <returns>result</returns>
		public bool Authori()
		{
			var response = new OMotionApiFacade().Authori(this.AuthoriId, this.LoginId);

			var result = false;

			if (response == null)
			{
				// ログインできなくなると困るのでtrueで返す
				result = true;
			}
			else
			{
				result = (response.Result == "OK");
			}
			return result;
		}

		/// <summary>
		/// Authori feedback
		/// </summary>
		/// <param name="value">value</param>
		/// <returns>result</returns>
		public bool AuthoriFeedback(string value)
		{
			var response = new OMotionApiFacade().AuthoriFeedback(this.AuthoriId, this.LoginId, value);

			return ((response != null) && (response.Status == 200));
		}

		/// <summary>
		/// Authori login success
		/// </summary>
		/// <param name="value">value</param>
		/// <returns>result</returns>
		public bool AuthoriLoginSuccess(bool value)
		{
			var response = new OMotionApiFacade().AuthoriLoginSuccess(this.AuthoriId, this.LoginId, value);

			return ((response != null) && (response.Status == 200));
		}
		#endregion

		#region +Properties
		/// <summary>Authori id</summary>
		protected string AuthoriId { get; set; }
		/// <summary>Login id</summary>
		protected string LoginId { get; set; }
		#endregion
	}
}
