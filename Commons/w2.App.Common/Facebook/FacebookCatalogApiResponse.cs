/*
=========================================================================================================
  Module      : Facebook Catalog Api Response (FacebookCatalogApiResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Net;

namespace w2.App.Common.Facebook
{
	/// <summary>
	/// Facebook Catalog Api Response
	/// </summary>
	public class FacebookCatalogApiResponse
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="statusCode">Status code</param>
		/// <param name="content">Content</param>
		/// <param name="reason">Reason</param>
		public FacebookCatalogApiResponse(
			HttpStatusCode statusCode,
			string content,
			string reason)
		{
			this.StatusCode = statusCode;
			this.Content = content;
			this.ReasonPhrase = reason;
		}

		/// <summary>Status code</summary>
		internal HttpStatusCode StatusCode { get; set; }
		/// <summary>Reason phrase</summary>
		internal string ReasonPhrase { get; set; }
		/// <summary>Content</summary>
		internal string Content { get; set; }
	}
}