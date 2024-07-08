/*
=========================================================================================================
  Module      : Name Normalization Api Facade(NameNormalizationApiFacade.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Text;

namespace w2.App.Common.Order.OPlux.NameNormalization
{
	/// <summary>
	/// Name normalization api facade
	/// </summary>
	public static class NameNormalizationApiFacade
	{
		#region +Methods
		/// <summary>
		/// Call api
		/// </summary>
		/// <param name="request">Request</param>
		/// <returns>Name normalization response</returns>
		public static NameNormalizationResponse CallApi(string request)
		{
			using (var webClient = new WebClient())
			{
				var url = string.Format(
					"{0}?{1}",
					Constants.OPLUX_NORMALIZE_NAME_URL,
					request);

				try
				{
					OPluxLogger.WriteOPluxLog(OPluxLogger.OPluxProcessingType.ApiRequestBegin, url);

					// Call API
					var responseBytes = webClient.DownloadData(url);

					// Handle response
					var responseBody = Encoding.UTF8.GetString(responseBytes);
					var nameNormalizationResponse = JsonConvert.DeserializeObject<NameNormalizationResponse>(
						responseBody,
						new JsonSerializerSettings
						{
							NullValueHandling = NullValueHandling.Ignore,
						});

					OPluxLogger.WriteOPluxLog(
						OPluxLogger.OPluxProcessingType.ApiRequestEnd,
						ConvertObjectToString(nameNormalizationResponse));

					return nameNormalizationResponse;
				}
				catch (WebException ex)
				{
					// Handle error response
					var responseError = GetResponseError(ex);
					var errorMessage = ConvertObjectToString(string.Format(
						"{0}\r\n{1}\r\n{2}",
						url,
						responseError,
						ex));
					OPluxLogger.WriteOPluxLog(
						OPluxLogger.OPluxProcessingType.ApiRequestError,
						errorMessage);

					return JsonConvert.DeserializeObject<NameNormalizationResponse>(responseError);
				}
			}
		}

		/// <summary>
		/// Get response error
		/// </summary>
		/// <param name="webException">Web exception</param>
		/// <returns>Response error</returns>
		private static string GetResponseError(WebException webException)
		{
			if (webException.Response == null) return string.Empty;

			using (var streamReader = new StreamReader(webException.Response.GetResponseStream()))
			{
				return streamReader.ReadToEnd();
			}
		}

		/// <summary>
		/// Convert object to string
		/// </summary>
		/// <param name="content">Content</param>
		/// <returns>Content has been converted to string</returns>
		private static string ConvertObjectToString(object content)
		{
			var result = JsonConvert.SerializeObject(
				content,
				new JsonSerializerSettings
				{
					Formatting = Formatting.Indented,
					NullValueHandling = NullValueHandling.Ignore,
				});

			return result;
		}
		#endregion
	}
}
