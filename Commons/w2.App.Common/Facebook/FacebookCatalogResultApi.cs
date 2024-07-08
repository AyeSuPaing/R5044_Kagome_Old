/*
=========================================================================================================
  Module      : Facebook Catalog Result Api (FacebookCatalogResultApi.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Net;

namespace w2.App.Common.Facebook
{
	/// <summary>
	/// Facebook Catalog Result Api
	/// </summary>
	public class FacebookCatalogResultApi
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public FacebookCatalogResultApi()
		{
			this.ReasonPhrase = string.Empty;
			this.StatusCode = HttpStatusCode.InternalServerError;
		}

		/// <summary>Response</summary>
		public FacebookCatalogResponseApi Response { get; set; }
		/// <summary>Reason phrase</summary>
		public string ReasonPhrase { get; set; }
		/// <summary>Status code</summary>
		public HttpStatusCode StatusCode { get; set; }
		/// <summary>Is success</summary>
		public bool IsSuccess
		{
			get
			{
				var result = (this.IsError == false);
				return result;
			}
		}
		/// <summary>Is error</summary>
		public bool IsError
		{
			get
			{
				if ((this.Response == null) || (this.Response.Error != null)) return true;
				return false;
			}
		}
	}
}