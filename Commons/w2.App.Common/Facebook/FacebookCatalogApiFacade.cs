/*
=========================================================================================================
  Module      : Facebook Catalog Api Facade (FacebookCatalogApiFacade.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Net.Http;
using w2.App.Common.Facebook.Logger;

namespace w2.App.Common.Facebook
{
	/// <summary>
	/// Facebook Catalog Api Facade
	/// </summary>
	public class FacebookCatalogApiFacade
	{
		/// <summary>Http for API connection</summary>
		private static readonly HttpClient s_httpClient;

		/// <summary>
		/// Constructor
		/// </summary>
		static FacebookCatalogApiFacade()
		{
			s_httpClient = new HttpClient();
		}

		/// <summary>
		/// Call Api facebook
		/// </summary>
		/// <param name="request">Facebook catalog request api</param>
		/// <param name="facebookCatalogId">Facebook catalog id</param>
		/// <returns>Facebook Catalog result</returns>
		public FacebookCatalogResultApi CallApiFacebook(FacebookCatalogRequestApi request, string facebookCatalogId)
		{
			var apiUrl = string.Format(
				"{0}/{1}/batch",
				Constants.FACEBOOK_CATALOG_API_URL + Constants.FACEBOOK_CATALOG_API_VERSION,
				facebookCatalogId);
			var result = CallApi<FacebookCatalogResultApi, FacebookCatalogRequestApi>(apiUrl, request);
			return result;
		}

		/// <summary>
		/// Call api
		/// </summary>
		/// <typeparam name="TResult">The type of result data</typeparam>
		/// <typeparam name="TBody">The type of request body data</typeparam>
		/// <param name="url">Api url</param>
		/// <param name="body">The request body data</param>
		/// <returns>The result data</returns>
		private TResult CallApi<TResult, TBody>(string url, TBody body)
			where TResult : FacebookCatalogResultApi, new()
			where TBody : FacebookCatalogRequestApi, new()
		{
			var result = new TResult();
			try
			{
				// Get request
				var requestString = JsonConvert.SerializeObject(
					body,
					new JsonSerializerSettings
					{
						Formatting = Formatting.Indented,
						NullValueHandling = NullValueHandling.Ignore
					});

				// Get responce
				var response = new FacebookCatalogApiRequest().GetHttpPostResponse(
					url,
					s_httpClient,
					requestString);
				var responseResult = JsonConvert.DeserializeObject<FacebookCatalogResponseApi>(response.Content);
				result.Response = responseResult;
				result.ReasonPhrase = response.ReasonPhrase;
				result.StatusCode = response.StatusCode;

				// Write log api
				WriteRequestLog(url, HttpMethod.Post.ToString(), requestString);
				WriteResponseLog(responseResult);
			}
			catch (Exception ex)
			{
				result.ReasonPhrase = ex.Message;
				FacebookCatalogApiLogger.Write(ex.ToString());
			}
			return result;
		}

		/// <summary>
		/// Write request log
		/// </summary>
		/// <param name="uri">URI</param>
		/// <param name="httpMethod">Http method</param>
		/// <param name="requestString">Request string</param>
		private void WriteRequestLog(string uri, string httpMethod, string requestString)
		{
			var message = string.Format(
				"URL : {0} [HttpMethod]: {1} [RequestParam]: {2}{3}",
				uri,
				httpMethod,
				requestString,
				Environment.NewLine);
			FacebookCatalogApiLogger.Write(message);
		}

		/// <summary>
		/// Write response log
		/// </summary>
		/// <param name="response">Response</param>
		private void WriteResponseLog(FacebookCatalogResponseApi response)
		{
			var responseString = JsonConvert.SerializeObject(
				response,
				new JsonSerializerSettings
				{
					Formatting = Formatting.Indented,
					NullValueHandling = NullValueHandling.Ignore
				});
			var message = string.Format(
				"ResponseParam: {0}{1}",
				responseString,
				Environment.NewLine);
			FacebookCatalogApiLogger.Write(message);
		}
	}
}