/*
=========================================================================================================
  Module      : Elogit Utility (ElogitUtility.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using w2.Common.Logger;

namespace w2.App.Common.Elogit
{
	/// <summary>
	/// Elogit utility
	/// </summary>
	public class ElogitUtility
	{
		/// <summary>Http for API connection</summary>
		private static readonly HttpClient m_httpClient;

		/// <summary>
		/// Elogit utility
		/// </summary>
		static ElogitUtility()
		{
			m_httpClient = new HttpClient();
		}

		/// <summary>
		/// File upload
		/// </summary>
		/// <param name="request">request</param>
		/// <param name="file">file</param>
		/// <returns>Elogit result</returns>
		public ElogitResult FileUpload(ElogitUploadRequest request, FileStream file = null)
		{
			var result = CallApi<ElogitResult, ElogitUploadRequest, ElogitResponse>(Constants.ELOGIT_WMS_UPLOAD_APIKEY, request, file);
			return result;
		}

		/// <summary>
		/// File download
		/// </summary>
		/// <param name="request">Request</param>
		/// <returns>Elogit result</returns>
		public ElogitResult FileDownload(ElogitDownloadRequest request)
		{
			var result = CallApi<ElogitResult, ElogitDownloadRequest, ElogitResponse>(Constants.ELOGIT_WMS_DOWNLOAD_APIKEY, request);
			return result;
		}

		/// <summary>
		/// Call api
		/// </summary>
		/// <typeparam name="TResult">The type of result data</typeparam>
		/// <typeparam name="TBody">The type of request body data</typeparam>
		/// <typeparam name="TResponse">The type of response data</typeparam>
		/// <param name="url">Api url</param>
		/// <param name="body">The request body data</param>
		/// <param name="file">File</param>
		/// <returns>The result data</returns>
		private static TResult CallApi<TResult, TBody, TResponse>(string url, TBody body, FileStream file = null)
			where TResult : ElogitResult, new()
			where TBody : BaseElogitRequest, new()
			where TResponse : ElogitResponse, new()
		{
			var result = new TResult();
			try
			{
				var paramaters = CreateRequestParamaters(body);
				var keyValues = string.Join(
					Environment.NewLine,
					paramaters.Select(param => string.Format("{0}={1}", param.Key, param.Value)));

				// Write log
				FileLogger.Write("WmsShipping", string.Format("{0}{1}", Environment.NewLine, keyValues));

				var response = new ElogitRequestHandler().GetHttpPostResponse(
					url,
					m_httpClient,
					file,
					paramaters);

				result.ReasonPhrase = response.ReasonPhrase;
				result.StatusCode = response.StatusCode;
				try
				{
					var responseResult = JsonConvert.DeserializeObject<TResponse>(response.Content);
					result.Response = responseResult;
				}
				catch
				{
					if (body.SyoriKbn == ElogitConstants.SYORI_KBN_FILE_DOWNLOAD)
					{
						result.DataFile = response.Content;
					}
				}
				FileLogger.Write("WmsShipping", string.Format("{0}{1}", Environment.NewLine, response.Content));
			}
			catch (Exception ex)
			{
				result.ReasonPhrase = ex.Message.ToString();
				FileLogger.WriteError(ex);
			}
			return result;
		}

		/// <summary>
		/// Create request paramaters
		/// </summary>
		/// <param name="request">Request</param>
		/// <returns>A dictionary</returns>
		private static Dictionary<string, string> CreateRequestParamaters(BaseElogitRequest request)
		{
			var jsonRequest = JsonConvert.SerializeObject(
				request,
				new JsonSerializerSettings
				{
					Formatting = Formatting.Indented,
					NullValueHandling = NullValueHandling.Ignore,
				});
			var resultRequest = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonRequest);
			return resultRequest;
		}
	}
}
