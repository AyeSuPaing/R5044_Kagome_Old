/*
=========================================================================================================
  Module      : Neweb Pay Api Facade(NewebPayApiFacade.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using w2.Common.Logger;
using w2.Common.Util;

namespace w2.App.Common.Order.Payment.NewebPay
{
	/// <summary>
	/// NewebPay Api Facade Class
	/// </summary>
	public class NewebPayApiFacade
	{
		/// <summary>Http client</summary>
		private static readonly HttpClient m_httpClient;

		/// <summary>
		/// Constructor
		/// </summary>
		static NewebPayApiFacade()
		{
			m_httpClient = new HttpClient();
		}

		/// <summary>
		/// Send Cancel Refund And Capture Order
		/// </summary>
		/// <param name="requestData">Request Data</param>
		/// <param name="isCancel">Is Cancel Order</param>
		/// <param name="isRefund">Is Refund Order</param>
		/// <returns>NewebPay Response Result</returns>
		public static NewebPayResponseResult SendCancelRefundAndCaptureOrder(
			NewebPayRequest requestData,
			bool isCancel,
			bool isRefund = false)
		{
			var apiUrl = (isCancel) ? "API/CreditCard/Cancel" : "API/CreditCard/Close";
			var requestDataObject = CreateRequestData(requestData);
			var dataObject = (JObject)JsonConvert.DeserializeObject(requestDataObject);
			var dataObjectChild = dataObject.Children().Cast<JProperty>()
				.Select(item => string.Format(
					"{0}={1}",
					item.Name,
					HttpUtility.UrlEncode(item.Value.ToString())));

			var url = string.Format("{0}{1}", Constants.NEWEBPAY_PAYMENT_APIURL, apiUrl);
			var parameter = string.Join("&", dataObjectChild);
			var result = CallApi<NewebPayResponseResult, NewebPayRequest>(url, parameter, isRefund);
			return result;
		}

		/// <summary>
		/// Call Api
		/// </summary>
		/// <typeparam name="TResult">The Type Of Result Data</typeparam>
		/// <typeparam name="TBody">The Type Of Request Body Data</typeparam>
		/// <param name="apiUrl">Api Url</param>
		/// <param name="body">The Request Body Data</param>
		/// <param name="isRefund">Is Refund Order</param>
		/// <returns>The Result Data</returns>
		protected static TResult CallApi<TResult, TBody>(
			string apiUrl,
			string body,
			bool isRefund)
			where TResult : NewebPayResponseResult, new()
		{
			WriteRequestLog(apiUrl, body);

			var response = new NewebPayApiRequest()
				.GetHttpPostResponse(
					apiUrl,
					m_httpClient,
					body,
					isRefund);

			WriteResponseLog(
				apiUrl,
				response.StatusCode,
				response.Content,
				response.ReasonPhrase);

			var result = GetResult<TResult>(response, isRefund);
			return result;
		}

		/// <summary>
		/// Get Result
		/// </summary>
		/// <typeparam name="TResult">The Type Of Result Data</typeparam>
		/// <param name="response">The Response Data</param>
		/// <param name="isRefund">Is Refund Order</param>
		/// <returns>The Result Data</returns>
		private static TResult GetResult<TResult>(NewebPayApiResponse response, bool isRefund)
			where TResult : NewebPayResponseResult, new()
		{
			var result = new TResult();
			try
			{
				var jsonBody = string.Empty;
				NewebPayResponse responseObject = null;
				if (isRefund)
				{
					responseObject = JsonConvert.DeserializeObject<NewebPayResponse>(response.Content.Replace("Result", "ResultRefund"));
				}
				else
				{
					var requests = HttpUtility.ParseQueryString(response.Content);
					var dictionary = requests.AllKeys.ToDictionary(item => item, item => requests[item]);
					jsonBody = CreateResponseData(dictionary);
					responseObject = JsonConvert.DeserializeObject<NewebPayResponse>(jsonBody);
				}
				result.SetResponse(responseObject);
				return result;
			}
			catch
			{
				return result;
			}
		}

		/// <summary>
		/// Create Form Payment
		/// </summary>
		/// <param name="requestData">Request Data</param>
		/// <returns>Form NewebPay</returns>
		public static string CreateFormPayment(NewebPayRequest requestData)
		{
			var requestDataObject = CreateRequestData(requestData);
			var url = string.Format("{0}MPG/mpg_gateway", Constants.NEWEBPAY_PAYMENT_APIURL);
			WriteRequestLog(url, requestDataObject);

			var parameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(requestDataObject);
			var format = "<input type='hidden' name='{0}' value='{1}'>";
			var formContent = new StringBuilder();
			formContent.Append("<html>")
				.Append("<body onload='document.forms[0].submit()'>")
				.AppendFormat("<form name='Newebpay' action='{0}' method='post'>", url);
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
		/// Create Request Data
		/// </summary>
		/// <param name="requestData">Request Data</param>
		/// <returns>Request Data</returns>
		private static string CreateRequestData(NewebPayRequest requestData)
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
		/// Get Response From Request
		/// </summary>
		/// <param name="request">Request</param>
		/// <returns>NewebPay Response</returns>
		public static NewebPayResponse GetResponseFromRequest(HttpRequest request)
		{
			var tradeInfo = string.Empty;
			if (request.Form.AllKeys.All(item => string.IsNullOrEmpty(item)) == false)
			{
				var dictionary = request.Form.AllKeys.ToDictionary(item => item, item => request.Form[item]);
				tradeInfo = dictionary[NewebPayConstants.PARAM_TRADE_INFO];
			}
			var resultDecode = NewebPayUtility.DecryptAES256(tradeInfo);
			var result = JsonConvert.DeserializeObject<NewebPayResponse>(resultDecode);
			var paramNo = StringUtility.ToEmpty(request[Constants.REQUEST_KEY_NO]);
			var no = paramNo.Split(',')[0];
			if (no == NewebPayConstants.CONST_CLIENT_BACK_URL)
			{
				result.CartId = paramNo.Split(',')[1];
			}
			return result;
		}

		/// <summary>
		/// Create Response Data
		/// </summary>
		/// <param name="responseData">Response Data</param>
		/// <returns>Response Data</returns>
		public static string CreateResponseData(object responseData)
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
		/// Write NewebPay Log
		/// </summary>
		/// <param name="message">A Message</param>
		public static void WriteNewebPayLog(string message)
		{
			FileLogger.Write("NewebPay", message, false, Encoding.UTF8);
		}

		/// <summary>
		/// Write Request Log
		/// </summary>
		/// <param name="apiUrl">Api Url</param>
		/// <param name="request">A Request</param>
		private static void WriteRequestLog(string apiUrl, string request)
		{
			WriteNewebPayLog(string.Format("{0}\r\n{1}", apiUrl, request));
		}

		/// <summary>
		/// Write Response Log
		/// </summary>
		/// <param name="apiUrl">Api Url</param>
		/// <param name="code">Http Status Code</param>
		/// <param name="response">A Response</param>
		/// <param name="message">A Message</param>
		private static void WriteResponseLog(
			string apiUrl,
			HttpStatusCode code,
			string response,
			string message)
		{
			WriteNewebPayLog(
				string.Format("{0}\r\n[{1}]{2}\r\n{3}", apiUrl, code, message, response));
		}
	}
}
