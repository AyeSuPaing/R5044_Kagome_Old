/*
=========================================================================================================
  Module      : Facebook Catalog Api Request (FacebookCatalogApiRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using w2.Common.Util;

namespace w2.App.Common.Facebook
{
	/// <summary>
	/// Facebook Catalog Api Request
	/// </summary>
	internal class FacebookCatalogApiRequest
	{
		/// <summary>Application/json</summary>
		private const string APPLICATION_MEDIA_TYPE_JSON = "application/json";

		/// <summary>
		/// Get http post response
		/// </summary>
		/// <param name="apiUrl">The api url</param>
		/// <param name="httpClient">The http client</param>
		/// <param name="body">Body</param>
		/// <returns>Api response</returns>
		public FacebookCatalogApiResponse GetHttpPostResponse(
			string apiUrl,
			HttpClient httpClient,
			string body)
		{
			var request = GetHttpPostRequest(apiUrl, body);
			var result = GetResponseAsync(httpClient, request).Result;
			return result;
		}

		/// <summary>
		/// Get http post request
		/// </summary>
		/// <param name="apiUrl">Api url</param>
		/// <param name="body">Body</param>
		/// <returns>Http request message</returns>
		private static HttpRequestMessage GetHttpPostRequest(
			string apiUrl,
			string body)
		{
			var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
			{
				Content = new StringContent(
					body,
					Encoding.UTF8,
					APPLICATION_MEDIA_TYPE_JSON)
			};
			return request;
		}

		/// <summary>
		/// Get response async
		/// </summary>
		/// <param name="httpClient">The http client</param>
		/// <param name="request">The request</param>
		/// <returns>Api response</returns>
		private static async Task<FacebookCatalogApiResponse> GetResponseAsync(
			HttpClient httpClient,
			HttpRequestMessage request)
		{
			var response = await httpClient.SendAsync(request);
			var content = await response.Content.ReadAsByteArrayAsync();
			var encoding = StringUtility.GetCode(content);
			var responseString = Encoding.GetEncoding(encoding.CodePage).GetString(content);
			var result = new FacebookCatalogApiResponse(
				response.StatusCode,
				responseString,
				response.ReasonPhrase);
			return result;
		}
	}
}