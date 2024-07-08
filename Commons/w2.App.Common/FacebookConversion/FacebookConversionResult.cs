/*
=========================================================================================================
  Module      : Facebook Conversion Result(FacebookConversionResult.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Net;

namespace w2.App.Common.FacebookConversion
{
	/// <summary>
	/// Facebook conversion result
	/// </summary>
	public class FacebookConversionResult
	{
		#region Constructor
		/// <summary>
		/// Facebook conversion result
		/// </summary>
		public FacebookConversionResult()
		{
			this.ReasonPhrase = string.Empty;
			this.StatusCode = HttpStatusCode.InternalServerError;
		}
		#endregion

		#region Properties
		/// <summary>Response</summary>
		public FacebookConversionResponse Response { get; set; }
		/// <summary>Reason phrase</summary>
		public string ReasonPhrase { get; set; }
		/// <summary>Status code</summary>
		public HttpStatusCode StatusCode { get; set; }
		/// <summary>Is success</summary>
		public bool IsSuccess
		{
			get
			{
				var result = (this.IsError == false) && (this.IsInProcess == false);
				return result;
			}
		}
		/// <summary>Is in process</summary>
		public bool IsInProcess
		{
			get
			{
				if (this.Response == null) return false;
				var isInProcess = ((this.Response.ErrorInfo != null)
					&& (this.Response.ErrorInfo.Message == FacebookConversionConstants.RESPONSE_MESSAGE_PROCESSING));
				return isInProcess;
			}
		}
		/// <summary>Is error</summary>
		public bool IsError
		{
			get
			{
				if ((this.Response == null) || (this.Response.ErrorInfo != null)) return true;
				return false;
			}
		}
		#endregion
	}
}
