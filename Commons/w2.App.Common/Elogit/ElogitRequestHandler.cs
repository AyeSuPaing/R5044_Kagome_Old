/*
=========================================================================================================
  Module      : Elogit Request Handler (ElogitRequestHandler.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using w2.App.Common.Util;
using w2.Common.Util;

namespace w2.App.Common.Elogit
{
	/// <summary>
	/// Elogit request handler
	/// </summary>
	public class ElogitRequestHandler
	{
		/// <summary>
		/// Create request upload
		/// </summary>
		/// <param name="syoriKbn">Syori kbn</param>
		/// <param name="ifHistoryKey">If history key</param>
		/// <returns>Elogit upload request</returns>
		public ElogitUploadRequest CreateRequestUpload(string syoriKbn, string ifHistoryKey = null)
		{
			var request = new ElogitUploadRequest
			{
				SyoriKbn = syoriKbn,
				IfCategoryCd = ElogitConstants.IF_CATEGORY_CD_UPLOAD,
				ApiMode = ElogitConstants.API_MODE,
				TargetType = ElogitConstants.TARGET_TYPE,
				Code = Constants.ELOGIT_WMS_CODE,
				UserId = Constants.ELOGIT_WMS_USER_ID,
				Password = Constants.ELOGIT_WMS_PASSWORD,
				IfHistoryKey = ifHistoryKey,
			};
			return request;
		}

		/// <summary>
		/// Create request file download
		/// </summary>
		/// <param name="syoriKbn">Syori kbn</param>
		/// <param name="ifHistorykey">If history key</param>
		/// <param name="downloadFileName">Download file name</param>
		/// <returns>Elogit download request</returns>
		public ElogitDownloadRequest CreateRequestFileDownLoad(
			string syoriKbn,
			string ifHistorykey = null,
			string downloadFileName = null)
		{
			var request = new ElogitDownloadRequest
			{
				SyoriKbn = syoriKbn,
				ApiMode = ElogitConstants.API_MODE,
				TargetType = ElogitConstants.TARGET_TYPE,
				Code = Constants.ELOGIT_WMS_CODE,
				UserId = Constants.ELOGIT_WMS_USER_ID,
				Password = Constants.ELOGIT_WMS_PASSWORD,
				IfHistoryKey = ifHistorykey,
				DlFileNm = downloadFileName,
			};

			if (syoriKbn == ElogitConstants.SYORI_KBN_FILE_CREATE_INSTRUCTIONS)
			{
				var dateCurrent = DateTimeUtility.ToString(
					DateTime.Now,
					DateTimeUtility.FormatType.ShortDate2Letter,
					string.Empty);
				var whereCondition = ElogitConstants.WHERE_CONDITION
					.Replace("@@ shiping_start_date @@", dateCurrent)
					.Replace("@@ shipping_end_date @@", dateCurrent);

				request.WhereCondition = whereCondition;
				request.IfCategoryCd = ElogitConstants.IF_CATEGORY_CD_DOWNLOAD;
			}
			return request;
		}

		/// <summary>
		/// Get http post response
		/// </summary>
		/// <param name="apiUrl">The api url</param>
		/// <param name="httpClient">The http client</param>
		/// <param name="file">File</param>
		/// <param name="parameters">Parameters</param>
		/// <returns>Api response</returns>
		public ElogitApiResponse GetHttpPostResponse(
			string apiUrl,
			HttpClient httpClient,
			FileStream file,
			Dictionary<string, string> parameters)
		{
			var request = GetHttpPostRequest(apiUrl, file, parameters);
			var result = GetResponseAsync(httpClient, request).Result;
			return result;
		}

		/// <summary>
		/// Get http post request
		/// </summary>
		/// <param name="apiUrl">Api url</param>
		/// <param name="file">File</param>
		/// <returns>Http request message</returns>
		private static HttpRequestMessage GetHttpPostRequest(
			string apiUrl,
			FileStream file,
			Dictionary<string, string> paramaters)
		{
			var boundary = string.Format("---------------------------{0}", DateTime.Now.ToString("MMddyyyyHmmss"));
			var content = new MultipartFormDataContent(boundary);

			if (file != null)
			{
				var streamContent = new StreamContent(file);
				streamContent.Headers.Add("Content-Type", "application/octet-stream");
				streamContent.Headers.Add(
					"Content-Disposition",
					string.Format(
						"form-data; name=\"UploadFile\"; filename=\"{0}\"",
						Path.GetFileNameWithoutExtension(file.Name)));

				content.Add(streamContent, "UploadFile");
			}

			foreach (var paramater in paramaters)
			{
				content.Add(new StringContent(paramater.Value), paramater.Key);
			}

			var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
			{
				Content = content,
			};
			return request;
		}

		/// <summary>
		/// Get response async
		/// </summary>
		/// <param name="httpClient">The Http client</param>
		/// <param name="request">The request</param>
		/// <returns>Api response</returns>
		private static async Task<ElogitApiResponse> GetResponseAsync(
			HttpClient httpClient,
			HttpRequestMessage request)
		{
			var response = await httpClient.SendAsync(request);
			var content = await response.Content.ReadAsByteArrayAsync();
			var encoding = StringUtility.GetCode(content);
			var responseString = Encoding.GetEncoding(encoding.CodePage).GetString(content);
			var result = new ElogitApiResponse(
				response.StatusCode,
				responseString,
				response.ReasonPhrase);
			return result;
		}
	}
}
