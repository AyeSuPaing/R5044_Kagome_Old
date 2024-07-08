/*
=========================================================================================================
  Module      : OMotion Api Facade(OMotionApiFacade.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Net;
using System.Text;
using w2.App.Common.User.OMotion.Authori;
using w2.App.Common.User.OMotion.AuthoriFeedback;
using w2.App.Common.User.OMotion.AuthoriLoginSuccess;
using w2.App.Common.Order.Payment;
using w2.Common.Helper;

namespace w2.App.Common.User.OMotion
{
	/// <summary>
	/// O-MOTION api facade
	/// </summary>
	public class OMotionApiFacade
	{
		#region +Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		public OMotionApiFacade()
		{
			this.ApiSetting = new OMotionApiSetting
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
		/// <typeparam name="T">Base O-MOTION response</typeparam>
		/// <param name="requestData">Request data</param>
		/// <returns>O-MOTION response</returns>
		private T CallApi<T>(BaseOMotionRequest requestData)
			where T : BaseOMotionResponse
		{
			try
			{
				var postData = this.ApiSetting.ApiEncoding.GetBytes(requestData.PostString);
				var request = CreateWebRequest(postData, requestData);

				if (this.ApiSetting.OnBeforeRequest != null)
				{
					this.ApiSetting.OnBeforeRequest(requestData);
				}

				if (requestData.MethodType != WebRequestMethods.Http.Get)
				{
					using (var stream = request.GetRequestStream())
					{
						stream.Write(postData, 0, postData.Length);
					}
				}

				var response = GetResponse(request, this.ApiSetting.ApiEncoding);

				if (this.ApiSetting.OnAfterRequest != null)
				{
					this.ApiSetting.OnAfterRequest(requestData, response);
				}

				return SerializeHelper.DeserializeJson<T>(response);
			}
			catch (Exception ex)
			{
				if (this.ApiSetting.OnRequestError != null)
				{
					this.ApiSetting.OnRequestError(requestData, requestData.ApiUrl, ex);
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
					return SerializeHelper.DeserializeJson<T>(response);
				}
			}

			return default(T);
		}

		/// <summary>
		/// Authori
		/// </summary>
		/// <param name="authoriRequest">Authori request</param>
		/// <returns>Authori response</returns>
		public AuthoriResponse Authori(AuthoriRequest authoriRequest)
		{
			var response = this.CallApi<AuthoriResponse>(authoriRequest);
			return response;
		}

		/// <summary>
		/// Authori
		/// </summary>
		/// <param name="authoriId">Authori id</param>
		/// <param name="loginId">Login id</param>
		/// <returns>Authori response</returns>
		public AuthoriResponse Authori(string authoriId, string loginId)
		{
			var request = new AuthoriRequest(authoriId, loginId);
			var response = this.CallApi<AuthoriResponse>(request);
			return response;
		}

		/// <summary>
		/// Authori login success
		/// </summary>
		/// <param name="authoriId">Authori id</param>
		/// <param name="loginId">Login id</param>
		/// <param name="value">Value</param>
		/// <returns>Authori login success response</returns>
		public AuthoriLoginSuccessResponse AuthoriLoginSuccess(string authoriId, string loginId, bool value)
		{
			var request = new AuthoriLoginSuccessRequest(authoriId, loginId, value);
			var response = this.CallApi<AuthoriLoginSuccessResponse>(request);
			return response;
		}

		/// <summary>
		/// Authori feedback
		/// </summary>
		/// <param name="authoriId">Authori id</param>
		/// <param name="loginId">Login id</param>
		/// <param name="value">Value</param>
		/// <returns>Authori feedback response</returns>
		public AuthoriFeedbackResponse AuthoriFeedback(string authoriId, string loginId, string value)
		{
			var request = new AuthoriFeedbackRequest(authoriId, loginId, value);
			var response = this.CallApi<AuthoriFeedbackResponse>(request);
			return response;
		}

		/// <summary>
		/// Before request proc
		/// </summary>
		/// <param name="apiRequestData">Api request data</param>>
		private void BeforeRequestProc(BaseOMotionRequest apiRequestData)
		{
			OMotionLogger.WriteOMotionLog(
				OMotionLogger.OMotionProcessingType.ApiRequestBegin,
				LogCreator.CreateRequestMessage(
					apiRequestData.GetType().Name,
					apiRequestData.CreateLogMessage()));
		}

		/// <summary>
		/// After request proc
		/// </summary>
		/// <param name="apiRequestData">Api request data</param>
		/// <param name="response">Response</param>
		private void AfterRequestProc(BaseOMotionRequest apiRequestData, string response)
		{
			OMotionLogger.WriteOMotionLog(
				OMotionLogger.OMotionProcessingType.ApiRequestEnd,
				LogCreator.CreateRequestMessageWithResult(
					apiRequestData.GetType().Name,
					apiRequestData.CreateLogMessage(),
					result: response));
		}

		/// <summary>
		/// Request error proc
		/// </summary>
		/// <param name="apiRequestData">Api request data</param>
		/// <param name="url">Url</param>
		/// <param name="exception">Exception</param>
		private void RequestErrorProc(
			BaseOMotionRequest apiRequestData,
			string url,
			Exception exception)
		{
			var postString = apiRequestData.PostString;
			var externalLog = LogCreator.CreateRequestMessage(
				apiRequestData.GetType().Name,
				postString,
				errorMessage: exception.Message);

			if (exception is WebException)
			{
				var webException = (WebException)exception;
				BaseOMotionResponse response = null;

				if (apiRequestData is AuthoriRequest)
				{
					response = GetResponseWebException<AuthoriResponse>(webException);
				}
				else if (apiRequestData is AuthoriLoginSuccessRequest)
				{
					response = GetResponseWebException<AuthoriLoginSuccessResponse>(webException);
				}
				else if (apiRequestData is AuthoriFeedbackRequest)
				{
					response = GetResponseWebException<AuthoriFeedbackResponse>(webException);
				}
				if ((response != null)
					&& (response.Status > 0))
				{
					externalLog = LogCreator.CreateErrorMessage(
						response.Status.ToString(),
						response.Message,
						url: url);
				}
			}

			OMotionLogger.WriteOMotionLog(
				OMotionLogger.OMotionProcessingType.ApiRequestError,
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

			if (this.ApiSetting.HttpTimeOutMilliSecond > 0)
			{
				webRequest.Timeout = this.ApiSetting.HttpTimeOutMilliSecond;
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
		private string GetResponse(WebRequest webRequest, Encoding encoding)
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
		/// <param name="requestData">Request data</param>
		/// <param name="userId">User id</param>
		/// <param name="password">Password</param>
		/// <returns>Web request</returns>
		private HttpWebRequest CreateWebRequest(
			byte[] postData,
			BaseOMotionRequest requestData,
			string userId = "",
			string password = "")
		{
			var webRequest = (HttpWebRequest)WebRequest.Create(requestData.ApiUrl);
			webRequest.Method = requestData.MethodType;
			//webRequest.Timeout = OMotionConst.DEFAULT_WEB_REQUEST_TIME_OUT;
			webRequest.ContentType = OMotionConstants.DEFAULT_WEB_REQUEST_CONTENT_TYPE;
			webRequest.ContentLength = postData.Length;
			webRequest.PreAuthenticate = (string.IsNullOrEmpty(userId)
				&& string.IsNullOrEmpty(password));
			webRequest.Proxy = null;
			foreach (var header in requestData.Headers)
			{
				webRequest.Headers.Add(header.Key, header.Value);
			}
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
		protected OMotionApiSetting ApiSetting { get; set; }
		#endregion
	}
}
