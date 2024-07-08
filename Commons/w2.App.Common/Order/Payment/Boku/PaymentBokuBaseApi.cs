/*
=========================================================================================================
  Module      : Payment Boku Base API(PaymentBokuBaseApi.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Net;
using System.Text;
using w2.App.Common.Order.Payment.Boku.Utils;
using w2.Common.Helper;

namespace w2.App.Common.Order.Payment.Boku
{
	/// <summary>
	/// Payment Boku Base API
	/// </summary>
	public abstract class PaymentBokuBaseApi : PaymentBokuBase
	{
		/// <summary>エンコーディング</summary>
		protected Encoding _encodingPost = Encoding.GetEncoding("utf-8");

		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="settings">Boku settings</param>
		public PaymentBokuBaseApi(PaymentBokuSetting settings)
			: base(settings)
		{
		}
		#endregion

		#region Methods
		/// <summary>
		/// XML POST送信
		/// </summary>
		/// <param name="requestXml">リクエストXML</param>
		/// <param name="apiUrl">API url</param>
		/// <param name="paymentType">Payment type</param>
		/// <returns>Response</returns>
		protected T Post<T>(
			string requestXml,
			string apiUrl,
			string paymentType)
			where T : PaymentBokuBaseResponse
		{
			// 接続・レスポンス取得
			var responseXml = PostHttpRequest(requestXml, apiUrl);

			// Handle error
			if (string.IsNullOrEmpty(responseXml))
			{
				WriteLog(
					false,
					(paymentType != null) ? paymentType.ToString() : string.Empty,
					PaymentFileLogger.PaymentProcessingType.ApiRequest,
					this.ErrorMessage,
					requestXml.ToString());
				return null;
			}

			var response = SerializeHelper.Deserialize<T>(responseXml);
			var messageDetail = requestXml.ToString() + "\n" + responseXml.ToString();

			WriteLog(
				response.IsSuccess,
				(paymentType != null) ? paymentType.ToString() : string.Empty,
				PaymentFileLogger.PaymentProcessingType.ApiRequest,
				response.Result.Message,
				messageDetail);

			return response;
		}

		/// <summary>
		/// POST送信
		/// </summary>
		/// <param name="requestString">リクエスト文字列</param>
		/// <param name="apiUrl">API url</param>
		/// <returns>戻り文字列</returns>
		private string PostHttpRequest(string requestString, string apiUrl)
		{
			var postData = Encoding.UTF8.GetBytes(requestString);

			// Create post request
			var webRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
			webRequest.Method = WebRequestMethods.Http.Post;
			webRequest.ContentType = "application/xml; charset=UTF-8";
			webRequest.ContentLength = postData.Length;

			// Add authorization
			webRequest.Headers["Authorization"] = CreateAuthorization(webRequest, requestString);

			try
			{
				// Writing of transmission data
				var postStream = webRequest.GetRequestStream();
				postStream.Write(postData, 0, postData.Length);
				postStream.Close();

				var responseText = GetResponseString(webRequest.GetResponse());
				return responseText;
			}
			catch (WebException ex)
			{
				this.ErrorMessage = GetResponseString(ex.Response);
				return string.Empty;
			}
		}

		/// <summary>
		/// Get response string
		/// </summary>
		/// <param name="response">The web response</param>
		/// <returns>A response as string</returns>
		private string GetResponseString(WebResponse response)
		{
			using (var responseStream = response.GetResponseStream())
			using (var sr = new StreamReader(responseStream, _encodingPost))
			{
				return sr.ReadToEnd();
			}
		}

		/// <summary>
		/// Create authorization
		/// </summary>
		/// <param name="webRequest">The web request</param>
		/// <param name="requestString">The request string</param>
		/// <returns>Authorization as string</returns>
		private string CreateAuthorization(
			HttpWebRequest webRequest,
			string requestString)
		{
			var timestamp = PaymentBokuUtils.ConvertToUnixTimestamp(DateTime.Now);
			var messageToSign = string.Format(
				"{0} {1}\nContent-Type: {2}\n{3}\n{4}",
				webRequest.Method,
				webRequest.RequestUri.PathAndQuery,
				webRequest.ContentType,
				(string.IsNullOrEmpty(requestString) == false)
					? PaymentBokuUtils.CreateHashString(requestString)
					: string.Empty,
				timestamp);
			var authorization = string.Format(
				"2/HMAC_SHA256(H+SHA256(E)) partner-id={0}, key-id={1}, timestamp={2}, signed-headers=Content-Type, signature={3}",
				this.Settings.MerchantId,
				this.Settings.KeyId,
				timestamp,
				PaymentBokuUtils.CreateSignature(
					this.Settings.APIKey,
					messageToSign));
			return authorization;
		}
		#endregion
	}
}
