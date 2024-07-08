/*
=========================================================================================================
  Module      : OPlux Api Facade(OPluxApiFacade.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Net;
using System.Text;
using w2.App.Common.Order.OPlux.RegisterEvent;
using w2.App.Common.Order.Payment;
using w2.Common.Helper;
using w2.Common.Web;

namespace w2.App.Common.Order.OPlux
{
	/// <summary>
	/// O-PLUX api facade
	/// </summary>
	public class OPluxApiFacade
	{
		#region +Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		public OPluxApiFacade()
		{
			this.ApiSetting = new OPluxApiSetting
			{
				ApiEncoding = Encoding.UTF8,
				OnAfterRequest = AfterRequestProc,
				OnBeforeRequest = BeforeRequestProc,
				OnExtendWebRequest = ExtendWebRequestProc,
				OnRequestError = RequestErrorProc,
			};
		}
		#endregion

		#region +Method
		/// <summary>
		/// Call api
		/// </summary>
		/// <typeparam name="T">Base O-PLUX response</typeparam>
		/// <param name="apiUrl">Api url</param>
		/// <param name="requestData">Request data</param>
		/// <returns>O-PLUX response</returns>
		private T CallApi<T>(string apiUrl, BaseOPluxRequest requestData)
			where T : BaseOPluxResponse
		{
			try
			{
				var response = string.Empty;
				var postString = requestData.CreatePostString();
				var postData = this.ApiSetting.ApiEncoding.GetBytes(postString);
				var request = CreateWebRequest(postData, apiUrl);

				if (this.ApiSetting.OnBeforeRequest != null)
				{
					this.ApiSetting.OnBeforeRequest(requestData);
				}

				using (var stream = request.GetRequestStream())
				{
					stream.Write(postData, 0, postData.Length);
				}

				response = GetResponse(request, this.ApiSetting.ApiEncoding);

				if (this.ApiSetting.OnAfterRequest != null)
				{
					this.ApiSetting.OnAfterRequest(requestData, response);
				}

				return SerializeHelper.Deserialize<T>(response);
			}
			catch (Exception ex)
			{
				if (this.ApiSetting.OnRequestError != null)
				{
					this.ApiSetting.OnRequestError(requestData, apiUrl, ex);
				}

				return null;
			}
		}

		/// <summary>
		/// Get response web exception
		/// </summary>
		/// <param name="webException">Web exception</param>
		/// <returns>Response web exception</returns>
		private T GetResponseWebException<T>(WebException webException)
		{
			if (webException.Status == WebExceptionStatus.ProtocolError)
			{
				using (var responseStream = webException.Response.GetResponseStream())
				using (var streamReader = new StreamReader(responseStream))
				{
					var response = streamReader.ReadToEnd();
					return SerializeHelper.Deserialize<T>(response);
				}
			}

			return default(T);
		}

		/// <summary>
		/// Register event
		/// </summary>
		/// <param name="registerEventRequest">Register event request</param>
		/// <returns>Register event response</returns>
		public RegisterEventResponse RegisterEvent(RegisterEventRequest registerEventRequest)
		{
			var response = this.CallApi<RegisterEventResponse>(
				Constants.OPLUX_REQUEST_EVENT_URL,
				registerEventRequest);
			return response;
		}

		/// <summary>
		/// Before request proc
		/// </summary>
		/// <param name="apiRequestData">Api request data</param>>
		private void BeforeRequestProc(IHttpApiRequestData apiRequestData)
		{
			var postdata = ((BaseOPluxRequest)apiRequestData).CreatePostString();

			OPluxLogger.WriteOPluxLog(
				OPluxLogger.OPluxProcessingType.ApiRequestBegin,
				LogCreator.CreateRequestMessage(
					apiRequestData.GetType().Name,
					postdata));
		}

		/// <summary>
		/// After request proc
		/// </summary>
		/// <param name="apiRequestData">Api request data</param>
		/// <param name="response">Response</param>
		private void AfterRequestProc(IHttpApiRequestData apiRequestData, string response)
		{
			var postdata = ((BaseOPluxRequest)apiRequestData).CreatePostString();

			OPluxLogger.WriteOPluxLog(
				OPluxLogger.OPluxProcessingType.ApiRequestEnd,
				LogCreator.CreateRequestMessageWithResult(
					apiRequestData.GetType().Name,
					postdata,
					result: response));
		}

		/// <summary>
		/// Request error proc
		/// </summary>
		/// <param name="apiRequestData">Api request data</param>
		/// <param name="url">Url</param>
		/// <param name="exception">Exception</param>
		private void RequestErrorProc(
			IHttpApiRequestData apiRequestData,
			string url,
			Exception exception)
		{
			var postdata = ((BaseOPluxRequest)apiRequestData).CreatePostString();
			var externalLog = LogCreator.CreateRequestMessage(
				apiRequestData.GetType().Name,
				postdata,
				errorMessage: exception.Message);

			if (exception is WebException)
			{
				var webException = (WebException)exception;

				if (apiRequestData is RegisterEventRequest)
				{
					var response = GetResponseWebException<RegisterEventResponse>(webException);
					if ((response != null)
						&& (response.Errors != null)
						&& (response.Errors.Error.Length > 0))
					{
						externalLog = LogCreator.CreateErrorMessage(
							response.Errors.Error[0].Code,
							response.Errors.Error[0].Message,
							url: url);
					}
				}
				else
				{
					var response = GetResponseWebException<string>(webException);
					externalLog = LogCreator.CreateRequestMessageWithResult(
						apiRequestData.GetType().Name,
						postdata,
						result: response);
				}
			}

			OPluxLogger.WriteOPluxLog(
				OPluxLogger.OPluxProcessingType.ApiRequestError,
				externalLog);
		}

		/// <summary>
		/// Extend web request proc
		/// </summary>
		/// <param name="webRequest">Web request</param>
		private void ExtendWebRequestProc(HttpWebRequest webRequest)
		{
			if (string.IsNullOrEmpty(this.ApiSetting.Method) == false)
			{
				webRequest.Method = this.ApiSetting.Method;
			}

			if (this.ApiSetting.HttpTimeOutMiriSecond > 0)
			{
				webRequest.Timeout = this.ApiSetting.HttpTimeOutMiriSecond;
			}

			if (string.IsNullOrEmpty(this.ApiSetting.ContentType) == false)
			{
				webRequest.ContentType = this.ApiSetting.ContentType;
			}
		}

		/// <summary>
		/// Get response
		/// </summary>
		/// <param name="webRequest">Web request</param>
		/// <param name="encoding">Encoding</param>
		/// <returns>Response</returns>
		private string GetResponse(HttpWebRequest webRequest, Encoding encoding)
		{
			using (var response = webRequest.GetResponse())
			using (var stream = response.GetResponseStream())
			using (var reader = new StreamReader(stream, encoding))
			{
				return reader.ReadToEnd();
			}
		}

		/// <summary>
		/// Create web request
		/// </summary>
		/// <param name="postData">Post data</param>
		/// <param name="url">Url</param>
		/// <param name="userId">User id</param>
		/// <param name="password">Password</param>
		/// <returns>Web request</returns>
		private HttpWebRequest CreateWebRequest(
			byte[] postData,
			string url,
			string userId = "",
			string password = "")
		{
			var webRequest = (HttpWebRequest)WebRequest.Create(url);
			webRequest.Method = OPluxConst.DEFAULT_WEB_REQUEST_METHOD;
			webRequest.Timeout = OPluxConst.DEFAULT_WEB_REQUEST_TIME_OUT;
			webRequest.ContentType = OPluxConst.DEFAULT_WEB_REQUEST_CONTENT_TYPE;
			webRequest.ContentLength = postData.Length;
			webRequest.PreAuthenticate = (string.IsNullOrEmpty(userId)
				&& string.IsNullOrEmpty(password));
			webRequest.Proxy = null;

			if (this.ApiSetting.OnExtendWebRequest != null)
			{
				this.ApiSetting.OnExtendWebRequest(webRequest);
			}

			if (string.IsNullOrEmpty(userId)
				&& string.IsNullOrEmpty(password))
			{
				return webRequest;
			}

			webRequest.Credentials = new NetworkCredential(userId, password);
			return webRequest;
		}
		#endregion

		#region +Properties
		/// <summary>Api setting</summary>
		protected OPluxApiSetting ApiSetting { get; set; }
		#endregion
	}
}
