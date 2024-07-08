/*
=========================================================================================================
  Module      : NP After Pay Api Facade(NPAfterPayApiFacade.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using w2.Common.Logger;

namespace w2.App.Common.Order.Payment.NPAfterPay
{
	/// <summary>
	/// NP After Pay Api Facade
	/// </summary>
	public static class NPAfterPayApiFacade
	{
		#region +Constants
		/// <summary>HTTP method: POST</summary>
		private const string HTTP_METHOD_POST = "POST";
		/// <summary>HTTP method: PATCH</summary>
		private const string HTTP_METHOD_PATCH = "PATCH";
		/// <summary>HTTP header: Content-Type</summary>
		private const string HTTP_HEADER_CONTENT_TYPE = "Content-Type";
		/// <summary>HTTP header: Authorization</summary>
		private const string HTTP_HEADER_AUTHORIZATION = "Authorization";
		/// <summary>HTTP header: X NP Terminal Id</summary>
		private const string HTTP_HEADER_X_NP_TERMINAL_ID = "X-NP-Terminal-Id";
		/// <summary>HTTP header: Content-Type: application/json</summary>
		private const string HTTP_HEADER_CONTENT_TYPE_APPLICATION_JSON = "application/json";
		#endregion

		#region +Call API Methods
		/// <summary>
		/// Regist Order
		/// </summary>
		/// <param name="requestData">Request Data</param>
		/// <returns>NP After Pay Result</returns>
		public static NPAfterPayResult RegistOrder(NPAfterPayRequest requestData)
		{
			var requestDataObject = CreateRequestData(requestData);
			var response = CallApi<NPAfterPayResponse>(
				string.Format("{0}transactions", Constants.PAYMENT_NP_AFTERPAY_APIURL),
				HTTP_METHOD_POST,
				requestDataObject);
			var result = new NPAfterPayResult(response);
			return result;
		}

		/// <summary>
		/// Update Order
		/// </summary>
		/// <param name="requestData">Request Data</param>
		/// <returns>NP After Pay Result</returns>
		public static NPAfterPayResult UpdateOrder(NPAfterPayRequest requestData)
		{
			var requestDataObject = CreateRequestData(requestData);
			var response = CallApi<NPAfterPayResponse>(
				string.Format("{0}transactions/update", Constants.PAYMENT_NP_AFTERPAY_APIURL),
				HTTP_METHOD_PATCH,
				requestDataObject);
			var result = new NPAfterPayResult(response);
			return result;
		}

		/// <summary>
		/// Cancel Order
		/// </summary>
		/// <param name="requestData">Request Data</param>
		/// <returns>NP After Pay Result</returns>
		public static NPAfterPayResult CancelOrder(NPAfterPayRequest requestData)
		{
			var requestDataObject = CreateRequestData(requestData);
			var response = CallApi<NPAfterPayResponse>(
				string.Format("{0}transactions/cancel", Constants.PAYMENT_NP_AFTERPAY_APIURL),
				HTTP_METHOD_PATCH,
				requestDataObject);
			var result = new NPAfterPayResult(response);
			return result;
		}

		/// <summary>
		/// Shipment Order
		/// </summary>
		/// <param name="requestData">Request Data</param>
		/// <returns>NP After Pay Result</returns>
		public static NPAfterPayResult ShipmentOrder(NPAfterPayRequest requestData)
		{
			var requestDataObject = CreateRequestData(requestData);
			var response = CallApi<NPAfterPayResponse>(
				string.Format("{0}shipments", Constants.PAYMENT_NP_AFTERPAY_APIURL),
				HTTP_METHOD_POST,
				requestDataObject);
			var result = new NPAfterPayResult(response);
			return result;
		}

		/// <summary>
		/// Get Payment Order
		/// </summary>
		/// <param name="requestData">Request Data</param>
		/// <returns>NP After Pay Result</returns>
		public static NPAfterPayResult GetPaymentOrder(NPAfterPayRequest requestData)
		{
			var requestDataObject = CreateRequestData(requestData);
			var response = CallApi<NPAfterPayResponse>(
				string.Format("{0}transactions/payments/find", Constants.PAYMENT_NP_AFTERPAY_APIURL),
				HTTP_METHOD_POST,
				requestDataObject);
			var result = new NPAfterPayResult(response);
			return result;
		}

		/// <summary>
		/// Create Request Data
		/// </summary>
		/// <param name="requestData">Request Data</param>
		/// <returns>Request Data</returns>
		private static string CreateRequestData(NPAfterPayRequest requestData)
		{
			var requestDataObject = JsonConvert.SerializeObject(
				requestData,
				Formatting.Indented,
				new JsonSerializerSettings
				{
					NullValueHandling = NullValueHandling.Ignore
				});
			return requestDataObject;
		}

		/// <summary>
		/// Call Api
		/// </summary>
		/// <typeparam name="TResponse">Type of response data</typeparam>
		/// <param name="uri">The request URL</param>
		/// <param name="method">Method</param>
		/// <param name="requestData">The request data</param>
		/// <returns>The response data</returns>
		private static TResponse CallApi<TResponse>(string uri, string method, string requestData)
		{
			using (var client = new WebClient())
			{
				try
				{
					WriteRequestDataLog(uri, requestData);

					// Set HTTP headers
					client.Headers.Add(HTTP_HEADER_CONTENT_TYPE, HTTP_HEADER_CONTENT_TYPE_APPLICATION_JSON);
					client.Headers.Add(HTTP_HEADER_AUTHORIZATION, CreateHeaderAuthorization());
					client.Headers.Add(HTTP_HEADER_X_NP_TERMINAL_ID, Constants.PAYMENT_NP_AFTERPAY_TERMINALID);

					// Call API
					var requestBytes = Encoding.UTF8.GetBytes(requestData);
					var responseBytes = client.UploadData(uri, method, requestBytes);

					// Handle success response
					var responseBody = Encoding.UTF8.GetString(responseBytes);
					var result = JsonConvert.DeserializeObject<TResponse>(responseBody);
					WriteNPAfterPayLog(responseBody);
					return result;
				}
				catch (WebException exception)
				{
					// Handle error response
					var errorString = GetResponseError(exception);
					WriteNPAfterPayLog(string.Format("{0}\r\n{1}\r\n{2}", uri, errorString, exception));
					return JsonConvert.DeserializeObject<TResponse>(errorString);
				}
			}
		}
		#endregion

		#region +Helper Methods
		/// <summary>
		/// Create Header Authorization
		/// </summary>
		/// <returns>Header Authorization</returns>
		private static string CreateHeaderAuthorization()
		{
			var auth = string.Format("{0}:{1}",
				Constants.PAYMENT_NP_AFTERPAY_MERCHANTCODE,
				Constants.PAYMENT_NP_AFTERPAY_SPCODE);
			var result = string.Format("Basic {0}", Convert.ToBase64String(Encoding.UTF8.GetBytes(auth)));
			return result;
		}

		/// <summary>
		/// Get response error
		/// </summary>
		/// <param name="exception">A web exception</param>
		/// <returns>An error message</returns>
		private static string GetResponseError(WebException exception)
		{
			if (exception.Response == null) return string.Empty;

			using (var reader = new StreamReader(exception.Response.GetResponseStream()))
			{
				return reader.ReadToEnd();
			}
		}

		/// <summary>
		/// Write NP After Pay Log
		/// </summary>
		/// <param name="message">A message</param>
		private static void WriteNPAfterPayLog(string message)
		{
			FileLogger.Write("NPAfterPay", message);
		}

		/// <summary>
		/// Write request data log
		/// </summary>
		/// <param name="uri">URI</param>
		/// <param name="requestData">A request data</param>
		private static void WriteRequestDataLog(string uri, string requestData)
		{
			WriteNPAfterPayLog(string.Format("{0}\r\n{1}", uri, requestData));
		}
		#endregion
	}
}
