/*
=========================================================================================================
  Module      : EC Pay Api Facade(ECPayApiFacade.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using w2.Common.Logger;

namespace w2.App.Common.Order.Payment.ECPay
{
	/// <summary>
	/// EC Pay Api Facade Class
	/// </summary>
	public static class ECPayApiFacade
	{
		#region +Method Convenience Store
		/// <summary>
		/// Send Order Information
		/// </summary>
		/// <param name="requestData">Request Data</param>
		/// <returns>EC Pay Response Result</returns>
		public static ECPayResponseResult SendOrderInfo(ECPayConvenienceStoreRequest requestData)
		{
			var result = CallApiConvenienceStore(
				string.Format("{0}Express/Create", Constants.RECEIVINGSTORE_TWECPAY_APIURL),
				requestData);
			return result;
		}

		/// <summary>
		/// Update Order Information
		/// </summary>
		/// <param name="requestData">Request Data</param>
		/// <returns>EC Pay Response Result</returns>
		public static ECPayResponseResult UpdateOrderInfo(ECPayConvenienceStoreRequest requestData)
		{
			var result = CallApiConvenienceStore(
				string.Format("{0}Express/UpdateStoreInfo", Constants.RECEIVINGSTORE_TWECPAY_APIURL),
				requestData);
			return result;
		}

		/// <summary>
		/// Send Return At Home Information
		/// </summary>
		/// <param name="requestData">Request Data</param>
		/// <returns>EC Pay Response Result</returns>
		public static ECPayResponseResult SendReturnAtHomeInfo(ECPayConvenienceStoreRequest requestData)
		{
			var result = CallApiConvenienceStore(
				string.Format("{0}Express/ReturnHome", Constants.RECEIVINGSTORE_TWECPAY_APIURL),
				requestData);
			return result;
		}

		/// <summary>
		/// Send Return At Convenience Store Information
		/// </summary>
		/// <param name="requestData">Request Data</param>
		/// <param name="isFamilyMart">Is Family Mart</param>
		/// <returns>EC Pay Response Result</returns>
		public static ECPayResponseResult SendReturnAtConvenienceStoreInfo(
			ECPayConvenienceStoreRequest requestData,
			bool isFamilyMart)
		{
			var result = CallApiConvenienceStore(
				string.Format("{0}Express/{1}",
					Constants.RECEIVINGSTORE_TWECPAY_APIURL,
					isFamilyMart
						? "ReturnCVS"
						: "ReturnUniMartCVS"),
				requestData);
			return result;
		}

		/// <summary>
		/// Call Api Convenience Store
		/// </summary>
		/// <param name="uri">The request URL</param>
		/// <param name="requestData">Request Data</param>
		/// <returns>Response data</returns>
		private static ECPayResponseResult CallApiConvenienceStore(string uri, ECPayConvenienceStoreRequest requestData)
		{
			var result = new ECPayResponseResult();
			try
			{
				using (var client = new HttpClient())
				{
					var httpContent = new FormUrlEncodedContent(requestData.GetParameters());

					WriteRequestDataLog(uri, httpContent.ReadAsStringAsync().Result);

					client.BaseAddress = new Uri(uri);
					client.DefaultRequestHeaders.Accept.Clear();

					var httpResponse = client.PostAsync(string.Empty, httpContent).Result;
					if (httpResponse.IsSuccessStatusCode == false)
					{
						var errorString = httpResponse.Content.ReadAsStringAsync().Result;
						result.IsSuccess = false;
						result.ErrorMessage = errorString;
						WriteEcPayPaymentLog(string.Format("{0}\r\n{1}\r\n", uri, errorString));
						return result;
					}

					var responseString = httpResponse.Content.ReadAsStringAsync().Result;
					result.SetResponseData(responseString);
					result.IsSuccess = true;
					WriteEcPayPaymentLog(responseString);
				}
			}
			catch (Exception exception)
			{
				result.IsSuccess = false;
				result.ErrorMessage = exception.Message;
				WriteEcPayPaymentLog(string.Format("{0}\r\n{1}\r\n", uri, exception));
			}
			return result;
		}
		#endregion

		#region +Method Payment
		/// <summary>
		/// Create Form Payment
		/// </summary>
		/// <param name="requestData">Request Data</param>
		/// <returns>Form Ec Pay</returns>
		public static string CreateFormPayment(ECPayRequest requestData)
		{
			var requestDataObject = CreateRequestData(requestData);
			var url = Constants.ECPAY_PAYMENT_API_URL.TrimEnd('/') + "/Cashier/AioCheckOut/V5";
			WriteRequestDataLog(url, requestDataObject);

			var parameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(requestDataObject);
			var format = "<input type='hidden' name='{0}' value='{1}'>";
			var formContent = new StringBuilder();
			formContent.Append("<html>")
				.Append("<body onload='document.forms[0].submit()'>")
				.AppendFormat("<form action='{0}' method='post'>", url);
			foreach (var parameter in parameters)
			{
				formContent.AppendFormat(format, parameter.Key, parameter.Value);
			}
			formContent.Append("</form>")
				.Append("</body>")
				.Append("</html>");
			return formContent.ToString();
		}

		/// <summary>
		/// Cancel Refund And Capture Payment
		/// </summary>
		/// <param name="requestData">Request Data</param>
		/// <returns>Ec Pay Payment</returns>
		public static ECPayResponse CancelRefundAndCapturePayment(ECPayRequest requestData)
		{
			var url = string.Format("{0}CreditDetail/DoAction", Constants.ECPAY_PAYMENT_API_URL);
			var requestDataObject = CreateRequestData(requestData);
			var dataObject = (JObject)JsonConvert.DeserializeObject(requestDataObject);
			var dataObjectChild = dataObject.Children().Cast<JProperty>()
				.Select(item => string.Format(
					"{0}={1}",
					item.Name,
					HttpUtility.UrlEncode(item.Value.ToString())));
			var parameter = string.Join(
				"&",
				dataObjectChild);
			var result = CallApiPayment<ECPayResponse>(
				url,
				ECPayConstants.HTTP_METHOD_POST,
				parameter,
				ECPayConstants.HTTP_HEADER_CONTENT_TYPE_APPLICATION_FORM);
			return result;
		}

		/// <summary>
		/// Create Request Data
		/// </summary>
		/// <param name="requestData">Request Data</param>
		/// <returns>Request Data</returns>
		private static string CreateRequestData(ECPayRequest requestData)
		{
			var dataObject = JsonConvert.SerializeObject(
				requestData,
				new JsonSerializerSettings
				{
					Formatting = Formatting.Indented,
					NullValueHandling = NullValueHandling.Ignore
				});
			return dataObject;
		}

		/// <summary>
		/// Create Respone Data
		/// </summary>
		/// <param name="responeData">Respone Data</param>
		/// <returns>Respone Data</returns>
		private static string CreateResponeData(object responeData)
		{
			var responeDataObject = JsonConvert.SerializeObject(
				responeData,
				new JsonSerializerSettings
				{
					Formatting = Formatting.Indented,
					NullValueHandling = NullValueHandling.Ignore
				});
			return responeDataObject;
		}

		/// <summary>
		/// Call Api
		/// </summary>
		/// <typeparam name="TResponse">Type of response data</typeparam>
		/// <param name="uri">The request URL</param>
		/// <param name="method">Method</param>
		/// <param name="requestData">The request data</param>
		/// <param name="contentType">Content Type</param>
		/// <returns>The response data</returns>
		private static TResponse CallApiPayment<TResponse>(
			string uri,
			string method,
			string requestData,
			string contentType = ECPayConstants.HTTP_HEADER_CONTENT_TYPE_APPLICATION_JSON)
		{
			using (var client = new WebClient())
			{
				try
				{
					WriteRequestDataLog(uri, requestData);

					// Set HTTP headers
					client.Headers.Add(ECPayConstants.HTTP_HEADER_CONTENT_TYPE, contentType);

					// Call API
					var requestBytes = Encoding.UTF8.GetBytes(requestData);
					var responseBytes = client.UploadData(uri, method, requestBytes);

					// Handle success response
					var responseBody = Encoding.UTF8.GetString(responseBytes);
					var queryData = HttpUtility.ParseQueryString(responseBody);
					if (queryData.AllKeys.All(item => string.IsNullOrEmpty(item)) == false)
					{
						var dictionary = queryData.AllKeys.ToDictionary(item => item, item => queryData[item]);
						responseBody = CreateResponeData(dictionary);
					}
					var result = JsonConvert.DeserializeObject<TResponse>(responseBody);
					WriteEcPayPaymentLog(responseBody);
					return result;
				}
				catch (WebException exception)
				{
					// Handle error response
					var errorString = GetResponseError(exception);
					WriteEcPayPaymentLog(string.Format("{0}\r\n{1}\r\n{2}", uri, errorString, exception));
					return JsonConvert.DeserializeObject<TResponse>(errorString);
				}
			}
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
		/// Receive
		/// </summary>
		/// <param name="request">request</param>
		/// <returns>request</returns>
		public static ECPayRequest Receive(HttpRequest request)
		{
			var urlFormEncode = Encoding.UTF8.GetString(request.BinaryRead(request.ContentLength));
			var json = string.Format(
				"{{'{0}'}}",
				urlFormEncode.Replace("&", "','").Replace("=", "':'"));
			var result = JsonConvert.DeserializeObject<ECPayRequest>(json);
			return result;
		}

		/// <summary>
		/// Get Response From Request
		/// </summary>
		/// <param name="request">Request</param>
		/// <returns>Response</returns>
		public static ECPayResponse GetResponseFromRequest(HttpRequest request)
		{
			var jsonBody = string.Empty;
			if (request.Form.AllKeys.All(item => string.IsNullOrEmpty(item)) == false)
			{
				var dictionary = request.Form.AllKeys.ToDictionary(item => item, item => request.Form[item]);
				dictionary[Constants.REQUEST_KEY_NO] = request[Constants.REQUEST_KEY_NO];
				jsonBody = CreateResponeData(dictionary);
				FileLogger.WriteInfo(jsonBody);
			}
			var result = JsonConvert.DeserializeObject<ECPayResponse>(jsonBody);
			return result;
		}
		#endregion

		/// <summary>
		/// Write Ec Pay Payment Log
		/// </summary>
		/// <param name="message">A Message</param>
		private static void WriteEcPayPaymentLog(string message)
		{
			FileLogger.Write("EcPay", message);
		}

		/// <summary>
		/// Write Request Data Log
		/// </summary>
		/// <param name="uri">Uri</param>
		/// <param name="requestData">A Request Data</param>
		private static void WriteRequestDataLog(string uri, string requestData)
		{
			WriteEcPayPaymentLog(string.Format("{0}\r\n{1}", uri, requestData));
		}
	}
}
