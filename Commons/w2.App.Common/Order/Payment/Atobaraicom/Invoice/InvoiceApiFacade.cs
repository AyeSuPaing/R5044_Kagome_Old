/*
=========================================================================================================
  Module      : Invoice Api Facade (InvoiceApiFacade.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.IO;
using System.Net;
using System.Text;
using w2.Common.Helper;
using w2.Common.Logger;

namespace w2.App.Common.Order.Payment.Atobaraicom.Invoice
{
	/// <summary>
	/// Invoice api facade
	/// </summary>
	public static class InvoiceApiFacade
	{
		/// <summary>
		/// Call api
		/// </summary>
		/// <param name="url">Url</param>
		/// <param name="request">Request</param>
		/// <returns>Response</returns>
		public static T CallApi<T>(string url, object request)
		{
			var parameters = SerializeHelper.Serialize(request);
			var dataRequest = Encoding.UTF8.GetBytes(parameters);
			var webRequest = WebRequest.Create(url);
			webRequest.Method = WebRequestMethods.Http.Post;
			webRequest.ContentType = "text/xml";
			webRequest.ContentLength = dataRequest.Length;
			webRequest.Timeout = (Constants.PAYMENT_ATOBARAICOM_WEB_REQUEST_TIME_OUT_SECOND * 1000);

			FileLogger.Write("RequestInvoiceApi", parameters);
			using (var requestStream = webRequest.GetRequestStream())
			{
				requestStream.Write(dataRequest, 0, dataRequest.Length);
			}

			var httpResponse = (HttpWebResponse)webRequest.GetResponse();
			var response = string.Empty;
			using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
			{
				response = streamReader.ReadToEnd();
			}

			FileLogger.Write("ResponseInvoiceApi", response);
			var result = SerializeHelper.Deserialize<T>(response);
			return result;
		}
	}
}
