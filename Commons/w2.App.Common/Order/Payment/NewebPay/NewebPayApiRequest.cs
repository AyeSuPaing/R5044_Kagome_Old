/*
=========================================================================================================
  Module      : NewebPay Api Request(NewebPayApiRequest.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using w2.App.Common.Mall.RakutenApi.Helper;

namespace w2.App.Common.Order.Payment.NewebPay
{
	/// <summary>
	/// NewebPay Api Request
	/// </summary>
	public class NewebPayApiRequest
	{
		/// <summary>
		/// Get Http Post Response
		/// </summary>
		/// <param name="apiUrl">Api Url</param>
		/// <param name="httpClient">HTTP Client</param>
		/// <param name="content">Content</param>
		/// <param name="isRefund">Is Refund</param>
		/// <returns>NewebPay Api Response</returns>
		public NewebPayApiResponse GetHttpPostResponse(
			string apiUrl,
			HttpClient httpClient,
			string content,
			bool isRefund)
		{
			var request = GetHttpPostRequest(apiUrl, content, isRefund);
			var result = AsyncHelper.RunSync(() => GetResponseAsync(httpClient, request));
			return result;
		}

		/// <summary>
		/// Get Http Post Request
		/// </summary>
		/// <param name="apiUrl">Api Url</param>
		/// <param name="content">Content</param>
		/// <param name="isRefund">Is Refund</param>
		/// <returns>Http Request Message</returns>
		private static HttpRequestMessage GetHttpPostRequest(
			string apiUrl,
			string content,
			bool isRefund)
		{
			var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
			{
				Content = new StringContent(
					content,
					Encoding.UTF8,
					isRefund
						? "application/json"
						: "application/x-www-form-urlencoded"),
			};
			return request;
		}

		/// <summary>
		/// Get Response Async
		/// </summary>
		/// <param name="httpClient">Http Client</param>
		/// <param name="request">Request</param>
		/// <returns>NewebPay Api Response</returns>
		private static async Task<NewebPayApiResponse> GetResponseAsync(
			HttpClient httpClient,
			HttpRequestMessage request)
		{
			var response = await httpClient.SendAsync(request);
			var content = await response.Content.ReadAsStringAsync();
			var result = new NewebPayApiResponse(
				response.StatusCode,
				content,
				response.ReasonPhrase);
			return result;
		}
	}
}
