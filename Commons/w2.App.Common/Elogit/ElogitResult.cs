/*
=========================================================================================================
  Module      : Elogit Result (ElogitResult.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Net;

namespace w2.App.Common.Elogit
{
	/// <summary>
	/// Elogit result
	/// </summary>
	public class ElogitResult
	{
		#region Contructor
		/// <summary>
		/// Constructor
		/// </summary>
		public ElogitResult()
		{
			this.ReasonPhrase = string.Empty;
			this.StatusCode = HttpStatusCode.InternalServerError;
		}
		#endregion

		#region Properties
		/// <summary>Response</summary>
		public ElogitResponse Response { get; set; }
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
					&& (this.Response.ErrorInfo.Message == ElogitConstants.RESPONSE_MESSAGE_PROCESSING));
				return isInProcess;
			}
		}
		/// <summary>Is error</summary>
		public bool IsError
		{
			get
			{
				if (this.Response == null) return true;
				if (this.Response.IsError) return true;
				if (this.Response.ErrorInfo != null)
				{
					if (this.Response.ErrorInfo.IsError.ToUpper() == "TRUE") return true;
					return (string.IsNullOrEmpty(this.GetLogText) == false);
				}
				return this.Response.IsError;
			}
		}
		/// <summary>Get status code and error message</summary>
		public string GetStatusCodeAndErrorMessage
		{
			get
			{
				var message = (this.StatusCode != HttpStatusCode.OK)
					? string.Format("{0} : {1}", (int)this.StatusCode, this.ReasonPhrase)
					: string.Empty;

				if (this.Response == null) return message;
				var result = (this.IsSuccess == false)
					? (this.Response.ErrorInfo != null)
						? string.Format("{0} : {1}", this.Response.ErrorInfo.StatusCode, this.GetErrorMessage)
						: string.Empty
					: string.Empty;

				if (string.IsNullOrEmpty(result)) result = message;
				return result;
			}
		}
		/// <summary>Get log text</summary>
		public string GetLogText
		{
			get
			{
				if ((this.Response == null)
					|| (this.Response.ErrorInfo == null)) return string.Empty;

				var result = this.Response.ErrorInfo.LogText ?? string.Empty;
				return result.Trim();
			}
		}
		/// <summary>Get error message</summary>
		public string GetErrorMessage
		{
			get
			{
				if (this.Response == null) return string.Empty;
				var result = (this.IsSuccess == false)
					? this.Response.ErrorInfo.Message
					: string.Empty;
				return result;
			}
		}
		/// <summary>Data file</summary>
		public string DataFile { get; set; }
		#endregion
	}
}
