/*
=========================================================================================================
  Module      : PaidyPaymentAPIのファサード(PaidyPaymentApiFacade.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using w2.Common.Logger;

namespace w2.App.Common.Order.Payment.Paidy
{
	/// <summary>
	/// PaidyPaymentAPIのファサード
	/// </summary>
	public static class PaidyPaymentApiFacade
	{
		#region Constants
		/// <summary>HTTP method: GET</summary>
		private const string HTTP_METHOD_GET = "GET";
		/// <summary>HTTP method: POST</summary>
		private const string HTTP_METHOD_POST = "POST";
		/// <summary>HTTP header: Content-Type</summary>
		private const string HTTP_HEADER_CONTENT_TYPE = "Content-Type";
		/// <summary>HTTP header: Content-Type: application/json</summary>
		private const string HTTP_HEADER_CONTENT_TYPE_APPLICATION_JSON = "application/json";
		/// <summary>HTTP header: Paidy-Version</summary>
		private const string HTTP_HEADER_PAIDY_VERSION = "Paidy-Version";
		/// <summary>HTTP header: Authorization</summary>
		private const string HTTP_HEADER_AUTHORIZATION = "Authorization";
		#endregion

		#region Call API Methods
		/// <summary>
		/// Create a token payment
		/// </summary>
		/// <param name="paymentObject">Paidy payment object</param>
		/// <returns>Paidy response object</returns>
		/// <remarks>
		/// For more information, please visit: <seealso cref="https://paidy.com/docs/jp/payments.html#create_subs"/>
		/// </remarks>
		public static PaidyResultObject CreateTokenPayment(PaidyPaymentObject paymentObject)
		{
			using (var client = new WebClient())
			{
				var uri = "https://api.paidy.com/payments";
				var result = new PaidyResultObject();

				try
				{
					SetHttpHeaders(client);

					var requestData = JsonConvert.SerializeObject(
						paymentObject,
						new JsonSerializerSettings
						{
							Formatting = Formatting.Indented,
							NullValueHandling = NullValueHandling.Ignore
						});

					WriteRequestDataLog(uri, requestData);

					// Call API
					var requestBytes = Encoding.UTF8.GetBytes(requestData);
					var responseBytes = client.UploadData(uri, HTTP_METHOD_POST, requestBytes);

					// Handle response
					var responseBody = Encoding.UTF8.GetString(responseBytes);
					result.Payment = JsonConvert.DeserializeObject<PaidyPaymentObject>(responseBody);
				}
				catch (WebException exception)
				{
					HandleWebException(uri, result, exception);
				}
				catch (Exception exception)
				{
					HandleException(uri, result, exception);
				}

				return result;
			}
		}

		/// <summary>
		/// Capturing a payment
		/// </summary>
		/// <param name="paymentId">Paidy payment ID</param>
		/// <returns>Paidy response object</returns>
		/// <remarks>
		/// For more information, please visit: <seealso cref="https://paidy.com/docs/jp/payments.html#captures"/>
		/// </remarks>
		public static PaidyResultObject CapturePayment(string paymentId)
		{
			using (var client = new WebClient())
			{
				var uri = string.Format("https://api.paidy.com/payments/{0}/captures", paymentId);
				var result = new PaidyResultObject();

				try
				{
					SetHttpHeaders(client);

					var requestData = "{}";
					WriteRequestDataLog(uri, requestData);

					// Call API
					var requestBytes = Encoding.UTF8.GetBytes(requestData);
					var responseBytes = client.UploadData(uri, HTTP_METHOD_POST, requestBytes);

					// Handle response
					var responseBody = Encoding.UTF8.GetString(responseBytes);
					result.Payment = JsonConvert.DeserializeObject<PaidyPaymentObject>(responseBody);
				}
				catch (WebException exception)
				{
					HandleWebException(uri, result, exception);
				}
				catch (Exception exception)
				{
					HandleException(uri, result, exception);
				}

				return result;
			}
		}

		/// <summary>
		/// Refunding a payment
		/// </summary>
		/// <param name="captureId">Paidy capture ID</param>
		/// <param name="paymentId">Paidy payment ID</param>
		/// <param name="amount">A refund amount</param>
		/// <returns>Paidy response object</returns>
		/// <remarks>
		/// For more information, please visit: <seealso cref="https://paidy.com/docs/jp/payments.html#refunds"/>
		/// </remarks>
		public static PaidyResultObject RefundPayment(string captureId, string paymentId, decimal amount)
		{
			using (var client = new WebClient())
			{
				var uri = string.Format("https://api.paidy.com/payments/{0}/refunds", paymentId);
				var result = new PaidyResultObject();

				try
				{
					SetHttpHeaders(client);

					var requestData = string.Format("{{\"capture_id\":\"{0}\",\"amount\":{1}}}", captureId, amount);
					WriteRequestDataLog(uri, requestData);

					// Call API
					var requestBytes = Encoding.UTF8.GetBytes(requestData);
					var responseBytes = client.UploadData(uri, HTTP_METHOD_POST, requestBytes);

					// Handle response
					var responseBody = Encoding.UTF8.GetString(responseBytes);
					result.Payment = JsonConvert.DeserializeObject<PaidyPaymentObject>(responseBody);
				}
				catch (WebException exception)
				{
					HandleWebException(uri, result, exception);
				}
				catch (Exception exception)
				{
					HandleException(uri, result, exception);
				}

				return result;
			}
		}

		/// <summary>
		/// Retrieving a payment
		/// </summary>
		/// <param name="paymentId">Paidy payment ID</param>
		/// <returns>Paidy response object</returns>
		/// <remarks>
		/// For more information, please visit: <seealso cref="https://paidy.com/docs/jp/payments.html#status"/>
		/// </remarks>
		public static PaidyResultObject GetPayment(string paymentId)
		{
			using (var client = new WebClient())
			{
				var uri = string.Format("https://api.paidy.com/payments/{0}", paymentId);
				var result = new PaidyResultObject();

				try
				{
					SetHttpHeaders(client);

					WriteRequestDataLog(uri, string.Empty);

					// Call API
					var responseBytes = client.DownloadData(uri);

					// Handle response
					var responseBody = Encoding.UTF8.GetString(responseBytes);
					result.Payment = JsonConvert.DeserializeObject<PaidyPaymentObject>(responseBody);
				}
				catch (WebException exception)
				{
					HandleWebException(uri, result, exception);
				}
				catch (Exception exception)
				{
					HandleException(uri, result, exception);
				}

				return result;
			}
		}

		/// <summary>
		/// Closing a payment
		/// </summary>
		/// <param name="paymentId">Paidy payment ID</param>
		/// <returns>Paidy response object</returns>
		/// <remarks>
		/// For more information, please visit: <seealso cref="https://paidy.com/docs/jp/payments.html#close"/>
		/// </remarks>
		public static PaidyResultObject ClosePayment(string paymentId)
		{
			using (var client = new WebClient())
			{
				var uri = string.Format("https://api.paidy.com/payments/{0}/close", paymentId);
				var result = new PaidyResultObject();

				try
				{
					SetHttpHeaders(client);

					var requestData = "{}";
					WriteRequestDataLog(uri, requestData);

					// Call API
					var requestBytes = Encoding.UTF8.GetBytes(requestData);
					var responseBytes = client.UploadData(uri, HTTP_METHOD_POST, requestBytes);

					// Handle response
					var responseBody = Encoding.UTF8.GetString(responseBytes);
					result.Payment = JsonConvert.DeserializeObject<PaidyPaymentObject>(responseBody);
				}
				catch (WebException exception)
				{
					HandleWebException(uri, result, exception);
				}
				catch (Exception exception)
				{
					HandleException(uri, result, exception);
				}

				return result;
			}
		}

		/// <summary>
		/// Deleting a token
		/// </summary>
		/// <param name="tokenId">Paidy token ID</param>
		/// <returns>Paidy response object</returns>
		/// <remarks>
		/// For more information, please visit: <seealso cref="https://paidy.com/docs/jp/tokens.html#delete"/>
		/// </remarks>
		public static PaidyResultObject DeleteToken(string tokenId)
		{
			using (var client = new WebClient())
			{
				var uri = string.Format("https://api.paidy.com/tokens/{0}/delete", tokenId);
				var result = new PaidyResultObject();

				try
				{
					SetHttpHeaders(client);

					var requestData = string.Format(
						"{{\"wallet_id\":\"default\",\"reason\":{{\"code\":\"merchant.requested\",\"description\":\"deleted token\"}}}}");
					WriteRequestDataLog(uri, requestData);

					// Call API
					var requestBytes = Encoding.UTF8.GetBytes(requestData);
					var responseBytes = client.UploadData(uri, HTTP_METHOD_POST, requestBytes);

					// Handle response
					var responseBody = Encoding.UTF8.GetString(responseBytes);
					result.Token = JsonConvert.DeserializeObject<PaidyTokenObject>(responseBody);
				}
				catch (WebException exception)
				{
					HandleWebException(uri, result, exception);
				}
				catch (Exception exception)
				{
					HandleException(uri, result, exception);
				}

				return result;
			}
		}
		#endregion

		#region Helper methods
		/// <summary>
		/// Create HTTP header authorization data
		/// </summary>
		/// <returns>HTTP header authorization data</returns>
		private static string CreateHttpHeaderAuthorizationData()
		{
			return string.Format("Bearer {0}", Constants.PAYMENT_PAIDY_SECRET_KEY);
		}

		/// <summary>
		/// Set Paidy HTTP headers
		/// </summary>
		/// <param name="client">The web client object</param>
		private static void SetHttpHeaders(WebClient client)
		{
			client.Headers.Add(HTTP_HEADER_CONTENT_TYPE, HTTP_HEADER_CONTENT_TYPE_APPLICATION_JSON);
			client.Headers.Add(HTTP_HEADER_PAIDY_VERSION, Constants.PAYMENT_PAIDY_API_VERSION);
			client.Headers.Add(HTTP_HEADER_AUTHORIZATION, CreateHttpHeaderAuthorizationData());
		}

		/// <summary>
		/// Write request data log
		/// </summary>
		/// <param name="uri">URI</param>
		/// <param name="requestData">A request data</param>
		private static void WriteRequestDataLog(string uri, string requestData)
		{
			AppLogger.WriteInfo(string.Format("{0}\r\n{1}", uri, requestData));
		}

		/// <summary>
		/// Get Paidy error
		/// </summary>
		/// <param name="exception">A web exception</param>
		/// <returns>An error message</returns>
		private static string GetPaidyError(WebException exception)
		{
			if (exception.Response == null) return null;

			using (var reader = new StreamReader(exception.Response.GetResponseStream()))
			{
				return reader.ReadToEnd();
			}
		}

		/// <summary>
		/// Handle web exception
		/// </summary>
		/// <param name="uri">URI</param>
		/// <param name="result">Paidy result object</param>
		/// <param name="exception">A web exception</param>
		private static void HandleWebException(string uri, PaidyResultObject result, WebException exception)
		{
			var errorString = GetPaidyError(exception);
			result.InnerException = exception;
			result.Error = JsonConvert.DeserializeObject<PaidyErrorObject>(errorString);

			AppLogger.WriteError(string.Format("{0}\r\n{1}\r\n{2}", uri, errorString, exception));
		}

		/// <summary>
		/// Handle web exception
		/// </summary>
		/// <param name="uri">URI</param>
		/// <param name="result">Paidy result object</param>
		/// <param name="exception">An exception</param>
		private static void HandleException(string uri, PaidyResultObject result, Exception exception)
		{
			result.InnerException = exception;
			AppLogger.WriteError(string.Format("{0}\r\n{1}", uri, exception));
		}
		#endregion
	}
}
