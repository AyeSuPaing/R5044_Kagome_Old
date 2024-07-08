/*
=========================================================================================================
  Module      : Facebook Conversion Api Request(FacebookConversionApiRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using w2.Common.Util;

namespace w2.App.Common.FacebookConversion
{
	/// <summary>
	/// Facebook conversion api request
	/// </summary>
	class FacebookConversionApiRequest
	{
		/// <summary>Application/json</summary>
		private const string APPLICATION_MEDIA_TYPE_JSON = "application/json";

		/// <summary>
		/// Get http post response
		/// </summary>
		/// <param name="apiUrl">The api url</param>
		/// <param name="httpClient">The http client</param>
		/// <param name="parameter">Parameter</param>
		/// <returns>Api response</returns>
		public FacebookConversionApiResponse GetHttpPostResponse(
			string apiUrl,
			HttpClient httpClient,
			string parameter)
		{
			var request = GetHttpPostRequest(apiUrl, parameter);
			var result = GetResponseAsync(httpClient, request).Result;
			return result;
		}

		/// <summary>
		/// Get http post request
		/// </summary>
		/// <param name="apiUrl">Api url</param>
		/// <param name="parameter">Parameter</param>
		/// <returns>Http request message</returns>
		private static HttpRequestMessage GetHttpPostRequest(
			string apiUrl,
			string parameter)
		{
			var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
			{
				Content = new StringContent(
					parameter,
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
		private static async Task<FacebookConversionApiResponse> GetResponseAsync(
			HttpClient httpClient,
			HttpRequestMessage request)
		{
			var response = await httpClient.SendAsync(request);
			var content = await response.Content.ReadAsByteArrayAsync();
			var encoding = StringUtility.GetCode(content);
			var responseString = Encoding.GetEncoding(encoding.CodePage).GetString(content);
			var result = new FacebookConversionApiResponse(
				response.StatusCode,
				responseString,
				response.ReasonPhrase);
			return result;
		}
	}
}
