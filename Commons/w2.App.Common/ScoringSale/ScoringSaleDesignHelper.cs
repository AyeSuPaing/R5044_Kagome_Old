/*
=========================================================================================================
  Module      : Scoring Sale Design Helper (ScoringSaleDesignHelper.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using w2.Common.Web;
using w2.Domain.ScoringSale;

namespace w2.App.Common.ScoringSale
{
	/// <summary>
	/// Scoring sale design helper
	/// </summary>
	public static class ScoringSaleDesignHelper
	{
		/// <summary>Default an hour ago</summary>
		private const int CONST_DEFAULT_AN_HOUR_AGO = -1;

		/// <summary>
		/// Create preview file data
		/// </summary>
		/// <param name="model">Scoring sale</param>
		/// <param name="designType">Design type</param>
		/// <returns>Preview key</returns>
		public static string CreatePreviewFileData(ScoringSaleModel model, string designType)
		{
			var previewKey = Guid.NewGuid().ToString().Replace("-", string.Empty);
			var pathDirectory = GetPreviewDirectory(designType);

			if (Directory.Exists(pathDirectory) == false)
			{
				new DirectoryInfo(pathDirectory).Create();
			}

			DeletePreviewFile(designType);

			var content = JsonConvert.SerializeObject(model);
			var filePath = GetPreviewFilePath(previewKey, designType);

			WriteFile(filePath, content);

			return previewKey;
		}

		/// <summary>
		/// Delete preview file
		/// </summary>
		/// <param name="designType">Design type</param>
		public static void DeletePreviewFile(string designType)
		{
			var path = GetPreviewDirectory(designType);

			if (Directory.Exists(path) == false) return;

			var directory = new DirectoryInfo(path);
			foreach (var file in directory.GetFiles())
			{
				if (file.LastWriteTime < DateTime.Now.AddHours(CONST_DEFAULT_AN_HOUR_AGO))
				{
					file.Delete();
				}
			}
		}

		/// <summary>
		/// Get preview file path
		/// </summary>
		/// <param name="previewKey">Preview key</param>
		/// <param name="designType">Design type</param>
		/// <returns>File path</returns>
		public static string GetPreviewFilePath(string previewKey, string designType)
		{
			var filePath = Path.Combine(GetPreviewDirectory(designType), previewKey);
			return filePath;
		}

		/// <summary>
		/// Get preview directory
		/// </summary>
		/// <param name="designType">Design type</param>
		/// <returns>Path</returns>
		public static string GetPreviewDirectory(string designType)
		{
			var path = Path.Combine(
				Constants.PHYSICALDIRPATH_FRONT_PC,
				(designType == Constants.SCORINGSALE_DESIGN_TYPE_PC)
					? Constants.CMS_SCORINGSALE_PAGE_DIR_PATH_PC
					: Constants.CMS_SCORINGSALE_PAGE_DIR_PATH_SP,
				"_preview");
			return path;
		}

		/// <summary>
		/// Get preivew url
		/// </summary>
		/// <param name="directory">Directory</param>
		/// <param name="topUseFlg">Top use flg</param>
		/// <param name="previewKey">Preview key</param>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <returns>Url</returns>
		public static string GetPreviewUrl(
			string topUseFlg,
			string previewKey,
			string designType,
			string scoringSaleId)
		{
			var isTopPage = Convert.ToBoolean(topUseFlg);

			var directoryPath = (designType == Constants.SCORINGSALE_DESIGN_TYPE_PC)
				? Constants.CMS_SCORINGSALE_PAGE_DIR_PATH_PC
				: Constants.CMS_SCORINGSALE_PAGE_DIR_PATH_SP;
			var page = isTopPage
				? Constants.PAGE_FRONT_SCORINGSALE_TOP_PAGE_NAME
				: Constants.PAGE_FRONT_SCORINGSALE_QUESTION_PAGE_NAME;
			var urlPath = Path.Combine(
				Constants.PROTOCOL_HTTPS,
				Constants.SITE_DOMAIN,
				Constants.PATH_ROOT_FRONT_PC,
				directoryPath,
				page);

			var url = new UrlCreator(urlPath)
				.AddParam(Constants.REQUEST_KEY_SCORINGSALE_ID, scoringSaleId)
				.AddParam(Constants.REQUEST_KEY_PREVIEW_KEY, previewKey)
				.CreateUrl();
			return url;
		}

		/// <summary>
		/// Get preview model
		/// </summary>
		/// <param name="previewKey">Preview key</param>
		/// <param name="designType">Design type</param>
		/// <returns>Data</returns>
		public static ScoringSaleModel GetPreviewModel(string previewKey, string designType)
		{
			// Null if the file does not exist
			if (ExistsPreviewFile(previewKey, designType) == false) return null;

			var filePath = GetPreviewFilePath(previewKey, designType);

			return JsonConvert.DeserializeObject<ScoringSaleModel>(ReadFile(filePath, string.Empty));
		}

		/// <summary>
		/// Exists preview file
		/// </summary>
		/// <param name="previewKey">Preview key</param>
		/// <param name="designType">Design type</param>
		/// <returns>True：Exists, False：Not exists </returns>
		public static bool ExistsPreviewFile(string previewKey, string designType)
		{
			var filePath = GetPreviewFilePath(previewKey, designType);
			return File.Exists(filePath);
		}

		/// <summary>
		/// Write file
		/// </summary>
		/// <param name="path">Path</param>
		/// <param name="content">Content</param>
		private static void WriteFile(string path, string content)
		{
			using (var streamWriter = new StreamWriter(path, false, Encoding.UTF8))
			{
				streamWriter.Write(content);
			}
		}

		/// <summary>
		/// Read file
		/// </summary>
		/// <param name="path">Path</param>
		/// <param name="content">Content</param>
		/// <returns>Content data</returns>
		private static string ReadFile(string path, string content)
		{
			using (var streamReader = new StreamReader(path))
			{
				content = streamReader.ReadToEnd();
			}

			return content;
		}
	}
}
