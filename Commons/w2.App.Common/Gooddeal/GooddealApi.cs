/*
=========================================================================================================
  Module      : Gooddeal api(GooddealApi.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using w2.Common.Logger;

namespace w2.App.Common.Gooddeal
{
	/// <summary>
	/// Gooddeal api
	/// </summary>
	public class GooddealApi
	{
		/// <summary>Http for API connection</summary>
		private static readonly HttpClient m_httpClient;

		/// <summary>
		/// Constructor
		/// </summary>
		static GooddealApi()
		{
			m_httpClient = new HttpClient();
		}

		/// <summary>
		/// Register shipping delivery
		/// </summary>
		/// <param name="request">The request data</param>
		/// <param name="timestamp">timestamp</param>
		/// <returns>Gooddeal result</returns>
		public static GooddealResult RegisterShippingDelivery(GooddealRequest request, string timestamp)
		{
			var result = CallApi<GooddealResult, GooddealRequest, GooddealResponse>(
				Constants.TWSHIPPING_GOODDEAL_DELIVER_APIURL,
				request,
				timestamp);
			return result;
		}

		/// <summary>
		/// Cancel shipping delivery
		/// </summary>
		/// <param name="request">The request data</param>
		/// <param name="timestamp">timestamp</param>
		/// <returns>Gooddeal result</returns>
		public static GooddealResult CancelShippingDelivery(GooddealRequest request, string timestamp)
		{
			var result = CallApi<GooddealResult, GooddealRequest, GooddealResponse>(
				Constants.TWSHIPPING_GOODDEAL_CANCEL_APIURL,
				request,
				timestamp);
			return result;
		}

		/// <summary>
		/// Get shipping delivery
		/// </summary>
		/// <param name="request">The request data</param>
		/// <param name="timestamp">timestamp</param>
		/// <returns>Gooddeal result</returns>
		public static GooddealResult GetShippingDeliverySlip(GooddealRequest request, string timestamp)
		{
			var result = CallApi<GooddealResult, GooddealRequest, GooddealResponse>(
				Constants.TWSHIPPING_GOODDEAL_GET_SHIPPINGNO_APIURL,
				request,
				timestamp);
			return result;
		}

		/// <summary>
		/// Call api
		/// </summary>
		/// <typeparam name="TResult">The type of result data</typeparam>
		/// <typeparam name="TBody">The type of request body data</typeparam>
		/// <typeparam name="TResponse">The type of response data</typeparam>
		/// <param name="apiUrl">Api url</param>
		/// <param name="body">The request body data</param>
		/// <param name="timestamp">timestamp</param>
		/// <returns>The result data</returns>
		private static TResult CallApi<TResult, TBody, TResponse>(string apiUrl, TBody body, string timestamp)
			where TResult : GooddealResult, new()
			where TBody : GooddealRequest, new()
			where TResponse : GooddealResponse, new()
		{
			var result = new TResult();
			try
			{
				var request = body.ToJsonRequest();
				WriteRequestLog(apiUrl, request);

				var response = new GooddealApiRequest()
					.GetHttpPostResponse(
						apiUrl,
						m_httpClient,
						request,
						timestamp);

				WriteResponseLog(
					apiUrl,
					response.StatusCode,
					response.Content,
					response.ReasonPhrase);

				var responseObject = JsonConvert.DeserializeObject<TResponse>(response.Content);
				result.SetResponse(responseObject);
				return result;
			}
			catch (Exception ex)
			{
				var responseObject = new TResponse
				{
					Status = GooddealConstants.GOODDEAL_STATUS_FAILURE,
					ErrorMessage = ex.Message
				};
				result.SetResponse(responseObject);
				WriteGooddealLog(string.Format("{0}\r\n{1}\r\n", apiUrl, ex.ToString()));
				return result;
			}
		}

		/// <summary>
		/// Write response log
		/// </summary>
		/// <param name="apiUrl">Api url</param>
		/// <param name="code">Http status code</param>
		/// <param name="response">A response</param>
		/// <param name="message">A message</param>
		private static void WriteResponseLog(
			string apiUrl,
			HttpStatusCode code,
			string response,
			string message)
		{
			var status = string.Format("[{0}]{1}", code, message);
			WriteGooddealLog(string.Format("{0}\r\n{1}\r\n{2}", apiUrl, status, response));
		}

		/// <summary>
		/// Write request log
		/// </summary>
		/// <param name="apiUrl">Api url</param>
		/// <param name="request">A request</param>
		private static void WriteRequestLog(string apiUrl, string request)
		{
			WriteGooddealLog(string.Format("{0}\r\n{1}", apiUrl, request));
		}

		/// <summary>
		/// Write gooddeal log
		/// </summary>
		/// <param name="message">A message</param>
		private static void WriteGooddealLog(string message)
		{
			FileLogger.Write("Gooddeal", message);
		}
	}
}