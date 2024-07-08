/*
=========================================================================================================
  Module      : Gooddeal api request (GooddealApiRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace w2.App.Common.Gooddeal
{
	/// <summary>
	/// Gooddeal api request
	/// </summary>
	public class GooddealApiRequest
	{
		/// <summary>Media type in JSON format</summary>
		private const string APPLICATION_MEDIA_TYPE_JSON = "application/json";
		/// <summary>Request header: x-api-key</summary>
		private const string REQUEST_HEADER_X_API_KEY = "X-API-KEY";

		/// <summary>
		/// Get http post response
		/// </summary>
		/// <param name="apiUrl">Api url</param>
		/// <param name="httpClient">Http client</param>
		/// <param name="content">Content</param>
		/// <param name="timestamp">timestamp</param>
		/// <returns>Gooddeal api response</returns>
		public GooddealApiResponse GetHttpPostResponse(
			string apiUrl,
			HttpClient httpClient,
			string content,
			string timestamp)
		{
			var request = GetHttpPostRequest(apiUrl, content, timestamp);
			var result = AsyncHelper.RunSync(() => GetResponseAsync(httpClient, request));
			return result;
		}

		/// <summary>
		/// Get http post request
		/// </summary>
		/// <param name="apiUrl">Api url</param>
		/// <param name="content">Content</param>
		/// <param name="timestamp">timestamp</param>
		/// <returns>Http request message</returns>
		private static HttpRequestMessage GetHttpPostRequest(
			string apiUrl,
			string content,
			string timestamp)
		{
			var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
			{
				Content = new StringContent(
					content,
					Encoding.UTF8,
					APPLICATION_MEDIA_TYPE_JSON)
			};
			request.Headers.Add(REQUEST_HEADER_X_API_KEY, CreateGooddealApiKey(timestamp));
			return request;
		}

		/// <summary>
		/// Get response async
		/// </summary>
		/// <param name="httpClient">Http client</param>
		/// <param name="request">Request</param>
		/// <returns>Gooddeal api response</returns>
		private static async Task<GooddealApiResponse> GetResponseAsync(
			HttpClient httpClient,
			HttpRequestMessage request)
		{
			var response = await httpClient.SendAsync(request);
			var content = await response.Content.ReadAsStringAsync();
			var result = new GooddealApiResponse(
				response.StatusCode,
				content,
				response.ReasonPhrase);
			return result;
		}

		/// <summary>
		/// MD5でハッシュ化したGooddealApiKeyを作成
		/// </summary>
		/// <param name="timestamp">timestamp</param>
		/// <returns>GooddealApiKey</returns>
		private static string CreateGooddealApiKey(string timestamp)
		{
			var apiKey = string.Format(
				"hashkey={0}&apikey={1}&checktime={2}",
				Constants.TWSHIPPING_GOODDEAL_HASHKEY,
				Constants.TWSHIPPING_GOODDEAL_APIKEY,
				timestamp);
			var encodedApiKey = HttpUtility.UrlEncode(apiKey).ToLower();

			// MD5でハッシュ化
			var result = new StringBuilder();
			using (var md5Hash = MD5.Create())
			{
				var hashBytes = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(encodedApiKey));
				foreach (var byteData in hashBytes)
				{
					result.Append(byteData.ToString("x2"));
				}
			}
			return result.ToString();
		}
	}
}