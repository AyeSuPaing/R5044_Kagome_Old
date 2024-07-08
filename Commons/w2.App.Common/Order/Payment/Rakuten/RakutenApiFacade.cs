/*
=========================================================================================================
  Module      : Rakuten Api Facade(RakutenApiFacade.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using w2.Common.Logger;
using w2.Common.Extensions;
using w2.App.Common.Order.Payment.Rakuten.Authorize;

namespace w2.App.Common.Order.Payment.Rakuten
{
	/// <summary>
	/// Rakuten Api Facade Class
	/// </summary>
	public static class RakutenApiFacade
	{
		#region + Method Payment
		/// <summary>
		/// Authorize API
		/// </summary>
		/// <param name="requestData">Request Data</param>
		/// <returns>Rakuten Payment</returns>
		public static RakutenAuthorizeResponse AuthorizeAPI(RakutenAuthorizeRequest requestData)
		{
			RakutenAuthorizeResponse response = null;

			switch (Constants.PAYMENT_RAKUTEN_CREDIT_PAYMENT_METHOD)
			{
				case Constants.RakutenPaymentType.AUTH:
					response = Authorize(requestData);
					break;

				case Constants.RakutenPaymentType.CAPTURE:
					response = AuthorizeAndCapture(requestData);
					break;
			}

			return response;
		}
		
		/// <summary>
		/// Authorize
		/// </summary>
		/// <param name="requestData">Request Data</param>
		/// <returns>Rakuten Payment</returns>
		public static RakutenAuthorizeResponse Authorize(RakutenAuthorizeRequest requestData)
		{
			var result = CallApiPayment<RakutenAuthorizeResponse>(
				string.Format(RakutenConstants.URL_PAYMENT_AUTHORIZE_3D_OFF, Constants.PAYMENT_RAKUTEN_API_URL),
				RakutenConstants.HTTP_METHOD_POST,
				CreateRequestData(requestData),
				RakutenConstants.HTTP_HEADER_CONTENT_TYPE_APPLICATION_FORM);
			return result;
		}

		/// <summary>
		/// AuthorizeAndCapture
		/// </summary>
		/// <param name="requestData">Request Data</param>
		/// <returns>Rakuten Payment</returns>
		public static RakutenAuthorizeResponse AuthorizeAndCapture(RakutenAuthorizeRequest requestData)
		{
			var result = CallApiPayment<RakutenAuthorizeResponse>(
				string.Format(RakutenConstants.URL_PAYMENT_AUTHORIZE_AND_CAPTURE_3D_OFF, Constants.PAYMENT_RAKUTEN_API_URL),
				RakutenConstants.HTTP_METHOD_POST,
				CreateRequestData(requestData),
				RakutenConstants.HTTP_HEADER_CONTENT_TYPE_APPLICATION_FORM);
			return result;
		}

		/// <summary>
		/// Modify
		/// </summary>
		/// <param name="requestData">Request Data</param>
		/// <returns>Rakuten Payment</returns>
		public static RakutenModifyResponse ModifyPaymentAmount(RakutenModifyRequest requestData)
		{
			var result = CallApiPayment<RakutenModifyResponse>(
				string.Format(RakutenConstants.URL_PAYMENT_MODIFY, Constants.PAYMENT_RAKUTEN_API_URL),
				RakutenConstants.HTTP_METHOD_POST,
				CreateRequestData(requestData),
				RakutenConstants.HTTP_HEADER_CONTENT_TYPE_APPLICATION_FORM);
			return result;
		}

		/// <summary>
		/// Cancel Or Refund
		/// </summary>
		/// <param name="requestData">Request Data</param>
		/// <returns>Rakuten Payment</returns>
		public static RakutenCancelOrRefundResponse CancelOrRefund(RakutenCancelOrRefundRequest requestData)
		{
			var result = CallApiPayment<RakutenCancelOrRefundResponse>(
				string.Format(RakutenConstants.URL_PAYMENT_CANCEL_OR_REFUND, Constants.PAYMENT_RAKUTEN_API_URL),
				RakutenConstants.HTTP_METHOD_POST,
				CreateRequestData(requestData),
				RakutenConstants.HTTP_HEADER_CONTENT_TYPE_APPLICATION_FORM);
			return result;
		}

		/// <summary>
		/// Capture 
		/// </summary>
		/// <param name="requestData">Request Data</param>
		/// <returns>Rakuten Payment</returns>
		public static RakutenCaptureResponse Capture(RakutenCaptureRequest requestData)
		{
			var result = CallApiPayment<RakutenCaptureResponse>(
				string.Format(RakutenConstants.URL_PAYMENT_CAPTURE, Constants.PAYMENT_RAKUTEN_API_URL),
				RakutenConstants.HTTP_METHOD_POST,
				CreateRequestData(requestData),
				RakutenConstants.HTTP_HEADER_CONTENT_TYPE_APPLICATION_FORM);
			return result;
		}

		/// <summary>
		/// Authorize cvs
		/// </summary>
		/// <param name="requestData">Request data</param>
		/// <returns>Rakuten authorize response</returns>
		public static RakutenAuthorizeResponse AuthorizeCvs(RakutenAuthorizeRequest requestData)
		{
			var urlApi = Constants.PAYMENT_RAKUTEN_CVS_MOCK_OPTION_ENABLED
				? Constants.PAYMENT_RAKUTEN_CVS_APIURL_AUTH_MOCK
				: string.Format(
					RakutenConstants.URL_PAYMENT_AUTHORIZE_3D_OFF,
					Constants.PAYMENT_RAKUTEN_API_URL);

			var result = CallApiPayment<RakutenAuthorizeResponse>(
				urlApi,
				RakutenConstants.HTTP_METHOD_POST,
				CreateUrlParameterFromRequestData(requestData),
				RakutenConstants.HTTP_HEADER_CONTENT_TYPE_APPLICATION_FORM,
				true);
			return result;
		}

		/// <summary>
		/// Create Signature key
		/// </summary>
		/// <param name="data">data json</param>
		/// <returns>Signature hashed</returns>
		public static string CreateSignature(string data)
		{
			using (var hmac = new HMACSHA256(Encoding.ASCII.GetBytes(Constants.PAYMENT_RAKUTEN_CREDIT_SIGNATURE_KEY)))
			{
				var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
				var result = hash.JoinToString(b => String.Format("{0:x2}", b));
				return result;
			}
		}

		/// <summary>
		/// Encode string to base 64
		/// </summary>
		/// <param name="bytes">Bytes array</param>
		/// <returns>String data</returns>
		public static string EncodeToBase64Url(byte[] bytes)
		{
			var result = Convert.ToBase64String(bytes)
				.TrimEnd('=')
				.Replace('+', '-')
				.Replace('/', '_');
			return result;
		}
		#endregion +Method Payment

		#region -Rakuten function

		/// <summary>
		/// Write Request Data Log
		/// </summary>
		/// <param name="uri">Uri</param>
		/// <param name="requestData">A Request Data</param>
		/// <param name="isPaymentCvsPre">Is payment cvs pre</param>
		private static void WriteRequestDataLog(string uri, string requestData, bool isPaymentCvsPre)
		{
			WriteRakutenPaymentLog(string.Format("{0}\r\n{1}", uri, requestData), isPaymentCvsPre);
		}

		/// <summary>
		/// Write Rakuten Payment Log
		/// </summary>
		/// <param name="message">A Message</param>
		/// <param name="isPaymentCvsPre">Is payment cvs pre</param>
		private static void WriteRakutenPaymentLog(string message, bool isPaymentCvsPre)
		{
			FileLogger.Write(isPaymentCvsPre ? "RakutenCvs" : "Rakuten", message);
		}

		/// <summary>
		/// Get Response Error
		/// </summary>
		/// <param name="exception">Web Exception</param>
		/// <returns>An Error Message</returns>
		private static string GetResponseError(WebException exception)
		{
			if (exception.Response == null) return string.Empty;

			using (var reader = new StreamReader(exception.Response.GetResponseStream()))
			{
				return reader.ReadToEnd();
			}
		}

		/// <summary>
		/// Create Request Data
		/// </summary>
		/// <param name="requestData">Request Data</param>
		/// <returns>Request Data</returns>
		private static string CreateRequestData(object requestData)
		{
			var jsonData = JsonConvert.SerializeObject(
				requestData,
				Formatting.None,
				new JsonSerializerSettings
				{
					NullValueHandling = NullValueHandling.Ignore,
					Converters = new List<JsonConverter> { new RakutenApiUtility.DecimalJsonConverter() },
				});
			var dataObject = RakutenConstants.HTTP_PARAMETER_PAYMENT_INFO + "=" + EncodeToBase64Url(Encoding.UTF8.GetBytes(jsonData))
				+ "&" + RakutenConstants.HTTP_PARAMETER_SIGNATURE + "=" + CreateSignature(jsonData)
				+ "&" + RakutenConstants.HTTP_PARAMETER_KEY_VERSION + "=" + RakutenConstants.KEY_VERSION;

			return dataObject;
		}

		/// <summary>
		/// Create Response Data
		/// </summary>
		/// <param name="responseData">Response Data</param>
		/// <returns>Response Data</returns>
		private static string CreateResponseData(object responseData)
		{
			var responseDataObject = JsonConvert.SerializeObject(
				responseData,
				new JsonSerializerSettings
				{
					Formatting = Formatting.Indented,
					NullValueHandling = NullValueHandling.Ignore
				});
			return responseDataObject;
		}

		/// <summary>
		/// Call Api
		/// </summary>
		/// <typeparam name="TResponse">Type of response data</typeparam>
		/// <param name="uri">The request URL</param>
		/// <param name="method">Method</param>
		/// <param name="requestData">The request data</param>
		/// <param name="contentType">Content Type</param>
		/// <param name="isPaymentCvsPre">Is payment cvs pre</param>
		/// <returns>The response data</returns>
		private static TResponse CallApiPayment<TResponse>(
			string uri,
			string method,
			string requestData,
			string contentType = RakutenConstants.HTTP_HEADER_CONTENT_TYPE_APPLICATION_JSON,
			bool isPaymentCvsPre = false)
		{
			using (var client = new WebClient())
			{
				try
				{
					WriteRequestDataLog(uri, requestData, isPaymentCvsPre);

					// Set HTTP headers
					client.Headers.Add(RakutenConstants.HTTP_HEADER_CONTENT_TYPE, contentType);

					// Call API
					var responseBytes = client.UploadString(uri, method, requestData);

					// Handle success response
					var responseBody = responseBytes;
					var queryData = HttpUtility.ParseQueryString(responseBody);
					if (queryData.AllKeys.All(item => string.IsNullOrEmpty(item)) == false)
					{
						var dictionary = queryData.AllKeys.ToDictionary(item => item, item => queryData[item]);
						responseBody = CreateResponseData(dictionary);
					}
					var result = JsonConvert.DeserializeObject<TResponse>(responseBody);
					WriteRakutenPaymentLog(responseBody, isPaymentCvsPre);
					return result;
				}
				catch (WebException exception)
				{
					// Handle error response
					var errorString = GetResponseError(exception);
					WriteRakutenPaymentLog(string.Format("{0}\r\n{1}\r\n{2}", uri, errorString, exception), isPaymentCvsPre);
					return JsonConvert.DeserializeObject<TResponse>(errorString);
				}
			}
		}

		/// <summary>
		/// レスポンスのクレジットカード会社名を、会社コードに変換
		/// </summary>
		/// <param name="companyName">クレジットカード会社名</param>
		/// <returns>クレジットカード会社コード</returns>
		public static string ConvertCompanyCode(string companyName)
		{
			var companyCode = "";
			switch (companyName)
			{
				case "Visa":
					companyCode = Constants.FLG_USERCREDITCARD_VISA;
					break;
				case "MasterCard":
					companyCode = Constants.FLG_USERCREDITCARD_MASTER;
					break;
				case "American Express":
					companyCode = Constants.FLG_USERCREDITCARD_AMEX;
					break;
				case "Diners Club":
					companyCode = Constants.FLG_USERCREDITCARD_DINERS;
					break;
				case "JCB":
					companyCode = Constants.FLG_USERCREDITCARD_JCB;
					break;
				default:
					companyCode = companyName;
					break;
			}
			return companyCode;
		}

		/// <summary>
		/// Create url parameter from request data
		/// </summary>
		/// <param name="requestData">Request data</param>
		/// <returns>Url parameter</returns>
		private static string CreateUrlParameterFromRequestData(object requestData)
		{
			var jsonData = JsonConvert.SerializeObject(
				requestData,
				Formatting.None,
				new JsonSerializerSettings
				{
					NullValueHandling = NullValueHandling.Ignore,
					Converters = new List<JsonConverter> { new RakutenApiUtility.DecimalJsonConverter() },
				});

			var result = string.Format(
				"{0}={1}&{2}={3}&{4}={5}",
				RakutenConstants.HTTP_PARAMETER_PAYMENT_INFO,
				RakutenApiFacade.EncodeToBase64Url(Encoding.UTF8.GetBytes(jsonData)),
				RakutenConstants.HTTP_PARAMETER_SIGNATURE,
				RakutenApiFacade.CreateSignature(jsonData),
				RakutenConstants.PARAMETER_KEY_VERSION,
				RakutenConstants.KEY_VERSION);
			return result;
		}

		#endregion -Rakuten function
	}
}

