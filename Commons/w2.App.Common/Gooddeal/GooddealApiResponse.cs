/*
=========================================================================================================
  Module      : Gooddeal api response (GooddealApiResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System.Net;

namespace w2.App.Common.Gooddeal
{
	/// <summary>
	/// Gooddeal api response
	/// </summary>
	public class GooddealApiResponse
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="statusCode">Status code</param>
		/// <param name="content">Content</param>
		/// <param name="reason">Reason</param>
		internal GooddealApiResponse(
			HttpStatusCode statusCode,
			string content,
			string reason)
		{
			this.StatusCode = statusCode;
			this.ReasonPhrase = reason ?? string.Empty;
			this.Content = content;
		}

		/// <summary>Status code</summary>
		internal HttpStatusCode StatusCode { get; set; }
		/// <summary>Reason phrase</summary>
		internal string ReasonPhrase { get; set; }
		/// <summary>Content</summary>
		internal string Content { get; set; }
	}
}