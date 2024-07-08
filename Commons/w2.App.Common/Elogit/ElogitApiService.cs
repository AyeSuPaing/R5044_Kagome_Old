/*
=========================================================================================================
  Module      : Elogit Api Service(ElogitApiService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.IO;

namespace w2.App.Common.Elogit
{
	/// <summary>
	/// Elogit api service
	/// </summary>
	public class ElogitApiService
	{
		/// <summary>
		/// Upload file
		/// </summary>
		/// <param name="filePath">File path</param>
		/// <returns>A elogit result</returns>
		public ElogitResult UploadFile(string filePath)
		{
			var fileData = File.OpenRead(filePath);
			var uploadRequest = new ElogitRequestHandler()
				.CreateRequestUpload(ElogitConstants.SYORI_KBN_FILE_CREATE_INSTRUCTIONS);
			var result = new ElogitUtility().FileUpload(uploadRequest, fileData);
			return result;
		}

		/// <summary>
		/// Get if history key upload
		/// </summary>
		/// <param name="ifHistoryKey">If history key</param>
		/// <returns>A elogit result</returns>
		public ElogitResult GetIfHistoryKeyUpload(string ifHistoryKey)
		{
			var uploadRequest = new ElogitRequestHandler()
				.CreateRequestUpload(ElogitConstants.SYORI_KBN_GET_STATUS_IF_HISTORY, ifHistoryKey);
			var result = new ElogitUtility().FileUpload(uploadRequest);
			return result;
		}

		/// <summary>
		/// File create instructions
		/// </summary>
		/// <returns>A elogit result</returns>
		public ElogitResult FileCreateInstructions()
		{
			var downloadRequest = new ElogitRequestHandler()
				.CreateRequestFileDownLoad(ElogitConstants.SYORI_KBN_FILE_CREATE_INSTRUCTIONS);
			var result = new ElogitUtility().FileDownload(downloadRequest);
			return result;
		}

		/// <summary>
		/// Get if history key download
		/// </summary>
		/// <param name="ifHistoryKey">If hostory key</param>
		/// <returns>A elogit result</returns>
		public ElogitResult GetIfHistoryKeyDownload(string ifHistoryKey)
		{
			var downloadRequest = new ElogitRequestHandler()
				.CreateRequestFileDownLoad(ElogitConstants.SYORI_KBN_GET_STATUS_IF_HISTORY, ifHistoryKey);
			var result = new ElogitUtility().FileDownload(downloadRequest);
			return result;
		}

		/// <summary>
		/// File download
		/// </summary>
		/// <param name="ifHistoryKey">If hostory key</param>
		/// <param name="fileName">File name</param>
		/// <returns>A elogit result</returns>
		public ElogitResult FileDownload(string ifHistoryKey, string fileName)
		{
			var downloadRequest = new ElogitRequestHandler()
				.CreateRequestFileDownLoad(ElogitConstants.SYORI_KBN_FILE_DOWNLOAD, ifHistoryKey, fileName);
			var result = new ElogitUtility().FileDownload(downloadRequest);
			return result;
		}
	}
}
