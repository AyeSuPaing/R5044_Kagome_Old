/*
=========================================================================================================
  Module      : Elogit Api Response (ElogitApiResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Net;

namespace w2.App.Common.Elogit
{
	/// <summary>
	/// Elogit api response
	/// </summary>
	public class ElogitApiResponse
	{
		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="statusCode">Status code</param>
		/// <param name="content">Content</param>
		/// <param name="reason">Reason</param>
		public ElogitApiResponse(HttpStatusCode statusCode, string content, string reason)
		{
			this.StatusCode = statusCode;
			this.Content = content;
			this.ReasonPhrase = reason;
		}
		#endregion

		#region Properties
		/// <summary>Status code</summary>
		internal HttpStatusCode StatusCode { get; set; }
		/// <summary>Reason phrase</summary>
		internal string ReasonPhrase { get; set; }
		/// <summary>Content</summary>
		internal string Content { get; set; }
		#endregion
	}
}
