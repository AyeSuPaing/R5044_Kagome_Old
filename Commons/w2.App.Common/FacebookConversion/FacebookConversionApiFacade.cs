/*
=========================================================================================================
  Module      : Facebook Conversion Api Facade(FacebookConversionApiFacade.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using w2.App.Common.Facebook.Logger;

namespace w2.App.Common.FacebookConversion
{
	/// <summary>
	/// Facebook conversion api facade
	/// </summary>
	public class FacebookConversionApiFacade
	{
		#region Constants
		/// <summary>Http for API connection</summary>
		private static readonly HttpClient s_httpClient;
		#endregion

		#region Call API Methods
		/// <summary>
		/// Facebook Conversion Api Facade
		/// </summary>
		static FacebookConversionApiFacade()
		{
			s_httpClient = new HttpClient();
		}

		/// <summary>
		/// Call API Facebook
		/// </summary>
		/// <param name="convertFaceBookObject">Convert facebook object</param>
		public void CallAPIFacebook(FacebookConversionRequest convertFacebookRequest)
		{
			var uri = string.Format(
				"{0}{1}/{2}/events?access_token={3}",
				Constants.MARKETING_FACEBOOK_CAPI_API_URL,
				Constants.MARKETING_FACEBOOK_CAPI_API_VERSION,
				Constants.MARKETING_FACEBOOK_CAPI_PIXEL_ID,
				Constants.MARKETING_FACEBOOK_CAPI_ACCESS_TOKEN);
			var result = CallApi<FacebookConversionResult, FacebookConversionRequest>(uri, convertFacebookRequest);
		}

		/// <summary>
		/// Call api
		/// </summary>
		/// <typeparam name="TResult">The type of result data</typeparam>
		/// <typeparam name="TBody">The type of request body data</typeparam>
		/// <param name="url">Api url</param>
		/// <param name="body">The request body data</param>
		/// <returns>The result data</returns>
		private static TResult CallApi<TResult, TBody>(string url, TBody body)
			where TResult : FacebookConversionResult, new()
			where TBody : FacebookConversionRequest, new()
		{
			var result = new TResult();
			try
			{
				var request = JsonConvert.SerializeObject(
					body,
					new JsonSerializerSettings
					{
						Formatting = Formatting.Indented,
						NullValueHandling = NullValueHandling.Ignore
					});

				var response = new FacebookConversionApiRequest().GetHttpPostResponse(
					url,
					s_httpClient,
					request);

				result.ReasonPhrase = response.ReasonPhrase;
				result.StatusCode = response.StatusCode;

				var responseResult = JsonConvert.DeserializeObject<FacebookConversionResponse>(response.Content);
				result.Response = responseResult;

				WriteRequestLog(url, HttpMethod.Post.ToString(), request);
				WriteResponseLog(url, responseResult);
			}
			catch (Exception ex)
			{
				result.ReasonPhrase = ex.Message.ToString();
				FacebookApiLogger.Write(ex.ToString());
			}
			return result;
		}

		/// <summary>
		/// Write request log
		/// </summary>
		/// <param name="uri">URI</param>
		/// <param name="httpMethod">Http method</param>
		/// <param name="requestData">Request data</param>
		private static void WriteRequestLog(string uri, string httpMethod, string requestData)
		{
			FacebookApiLogger.Write(string.Format("URL:{0}", uri));
			FacebookApiLogger.Write(string.Format("[HttpMethod]: {0}", httpMethod));
			FacebookApiLogger.Write(string.Format("Request Param \r\n {0}", requestData));
		}

		/// <summary>
		/// Write response log
		/// </summary>
		/// <param name="uri">URI</param>
		/// <param name="response">Response</param>
		private static void WriteResponseLog(string uri, FacebookConversionResponse response)
		{
			var responseString = JsonConvert.SerializeObject(
				response,
				new JsonSerializerSettings
				{
					Formatting = Formatting.Indented,
					NullValueHandling = NullValueHandling.Ignore
				});
			FacebookApiLogger.Write(string.Format("URL : {0}\r\n Response Param\r\n{1}", uri, responseString));
		}
		#endregion
	}
}
