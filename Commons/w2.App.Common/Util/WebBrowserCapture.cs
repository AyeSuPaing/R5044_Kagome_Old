/*
=========================================================================================================
  Module      : Webブラウザキャプチャー ユーティリティ(WebBrowserCapture.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using w2.Common.Logger;
using w2.Common.Web;

namespace w2.App.Common.Util
{
	/// <summary>
	/// Webブラウザキャプチャー ユーティリティ
	/// </summary>
	public static class WebBrowserCapture
	{
		/// <summary>
		/// デバイス
		/// </summary>
		public enum Device
		{
			/// <summary>PC</summary>
			Pc,
			/// <summary>SP</summary>
			Sp
		}

		/// <summary>
		/// 画像拡張子
		/// </summary>
		public enum Format
		{
			/// <summary>png形式</summary>
			Png,
			/// <summary>jpg形式</summary>
			Jpg
		}

		/// <summary>サムネイル画像 保存ディレクトリ</summary>
		private const string WEB_BROWSER_CAPTURE_DIR_PATH = @"Images\WebBrowserCapture";
		/// <summary>リクエストパラメータ:url 対象URL</summary>
		private const string PARAM_URL = "url";
		/// <summary>リクエストパラメータ:format 画像拡張子</summary>
		private const string PARAM_FORMAT = "format";
		/// <summary>リクエストパラメータ:delay キャプチャータイミング</summary>
		private const string PARAM_DELAY = "delay";
		/// <summary>リクエストパラメータ:device デバイス</summary>
		private const string PARAM_DEVICE = "device";
		/// <summary>リクエストパラメータ:bsize 縦横比</summary>
		private const string PARAM_BSIZE = "bsize";
		/// <summary>リクエストパラメータ:isize 取得時の画像サイズ</summary>
		private const string PARAM_ISIZE = "isize";

		/// <summary>
		/// Webブラウザキャプチャー画像生成
		/// </summary>
		/// <param name="managerSitePhysicalDirPath">管理環境 物理パス</param>
		/// <param name="filePath">対象ファイルパス(ルート以下のディレクトリパス ファイル名・拡張子含む)</param>
		/// <param name="webUrlParams">キャプチャー生成時のURLパラメータ一覧</param>
		/// <param name="format">拡張子</param>
		/// <param name="device">デバイス</param>
		/// <param name="delay">キャプチャータイミング</param>
		/// <param name="bSizeH">縦横比 縦</param>
		/// <param name="bSizeW">縦横比 横</param>
		/// <param name="iSizeH">画像サイズ 縦</param>
		/// <param name="iSizeW">画像サイズ 横</param>
		/// <returns>Webブラウザキャプチャーリクエストデータ</returns>
		public static void Create(
			string managerSitePhysicalDirPath,
			string filePath,
			List<KeyValuePair<string, string>> webUrlParams = null,
			Format format = Format.Png,
			Device device = Device.Pc,
			int delay = 0,
			int bSizeH = 1900,
			int bSizeW = 1000,
			int iSizeH = 200,
			int iSizeW = 300)
		{
			// Webブラウザキャプチャー保持用のディレクトリがなければ生成
			var dir = Path.Combine(managerSitePhysicalDirPath, WEB_BROWSER_CAPTURE_DIR_PATH);
			if (Directory.Exists(dir) == false)
			{
				Directory.CreateDirectory(dir);
			}

			var webUrl = GetWebUrl(filePath, webUrlParams);
			var requestUrl = GetApiRequestUrl(
				webUrl,
				format,
				device,
				delay,
				bSizeH,
				bSizeW,
				iSizeH,
				iSizeW);
			var imageFileName = GetImageFileName(requestUrl);
			var imagePhysicalFilePath = Path.Combine(dir, imageFileName + "." + format.ToString().ToLower());

			var temporaryPhysicalFilePath = Path.Combine(dir, imageFileName);
			if (File.Exists(temporaryPhysicalFilePath) == false)
			{
				using (File.Create(temporaryPhysicalFilePath)) { };

				Task.Run(
					() =>
					{
						var result = CreateImageFileAsync(
							requestUrl,
							imagePhysicalFilePath,
							temporaryPhysicalFilePath);
					});
			}

			//Tempファイルをクリアする
			try
			{
				//WebBrowserCaptureAPIのTmpファイルデイレクトリ
				var sourceDir = @"C:\Windows\Temp\WebBrowserCaptureAPI";
				DirectoryInfo directoryInfo = new DirectoryInfo(sourceDir);

				long totalSize = 0;
				var deleteFlag = false;

				// 一時フォルダ内のすべてのファイルの合計サイズを計算
				foreach (FileInfo fileInfo in directoryInfo.GetFiles())
				{
					totalSize += fileInfo.Length;
					if (totalSize > 100 * 1024 * 1024)
					{
						deleteFlag = true;
						break;
					}
				}

				// 100MB以上の場合、本日以外のファイルを削除
				if (deleteFlag)
				{
					foreach (FileInfo fileInfo in directoryInfo.GetFiles())
					{
						// 本日以外のファイルを削除
						if (fileInfo.LastWriteTime.Date < DateTime.Now.Date)
						{
							fileInfo.Delete();
						}
					}
				}
			}
			catch (DirectoryNotFoundException dirNotFound)
			{
				FileLogger.WriteError(string.Format("{0}デイレクトリ存在しません。", dirNotFound));
			}
		}

		/// <summary>
		/// 画像削除
		/// </summary>
		/// <param name="managerSitePhysicalDirPath">管理環境 物理パス</param>
		/// <param name="filePath">対象ファイルパス(ルート以下のディレクトリパス ファイル名・拡張子含む)</param>
		/// <param name="webUrlParams">キャプチャー生成時のURLパラメータ一覧</param>
		/// <param name="format">拡張子</param>
		/// <param name="device">デバイス</param>
		/// <param name="delay">キャプチャータイミング</param>
		/// <param name="bSizeH">縦横比 縦</param>
		/// <param name="bSizeW">縦横比 横</param>
		/// <param name="iSizeH">画像サイズ 縦</param>
		/// <param name="iSizeW">画像サイズ 横</param>
		public static void Delete(
			string managerSitePhysicalDirPath,
			string filePath,
			List<KeyValuePair<string, string>> webUrlParams = null,
			Format format = Format.Png,
			Device device = Device.Pc,
			int delay = 0,
			int bSizeH = 1900,
			int bSizeW = 1000,
			int iSizeH = 200,
			int iSizeW = 300)
		{
			var webUrl = GetWebUrl(filePath, webUrlParams);
			var requestUrl = GetApiRequestUrl(
				webUrl,
				format,
				device,
				delay,
				bSizeH,
				bSizeW,
				iSizeH,
				iSizeW);
			var imageFileName = GetImageFileName(requestUrl);
			var imagePhysicalFilePath = Path.Combine(managerSitePhysicalDirPath, WEB_BROWSER_CAPTURE_DIR_PATH, imageFileName + "." + format.ToString().ToLower());

			if (File.Exists(imagePhysicalFilePath))
			{
				File.Delete(imagePhysicalFilePath);
			}
			else
			{
				throw new Exception(string.Format("{0}が存在しません", imagePhysicalFilePath));
			}
		}

		/// <summary>
		/// 画像のファイルパス取得
		/// </summary>
		/// <param name="filePath">対象ファイルパス(ルート以下のディレクトリパス ファイル名・拡張子含む)</param>
		/// <param name="webUrlParams">キャプチャー生成時のURLパラメータ一覧</param>
		/// <param name="format">拡張子</param>
		/// <param name="device">デバイス</param>
		/// <param name="delay">キャプチャータイミング</param>
		/// <param name="bSizeH">縦横比 縦</param>
		/// <param name="bSizeW">縦横比 横</param>
		/// <param name="iSizeH">画像サイズ 縦</param>
		/// <param name="iSizeW">画像サイズ 横</param>
		/// <returns>画像パス</returns>
		public static string GetImageFilePath(
			string filePath,
			List<KeyValuePair<string, string>> webUrlParams = null,
			Format format = Format.Png,
			Device device = Device.Pc,
			int delay = 0,
			int bSizeH = 1900,
			int bSizeW = 1000,
			int iSizeH = 200,
			int iSizeW = 300)
		{
			var webUrl = GetWebUrl(filePath, webUrlParams);
			var requestUrl = GetApiRequestUrl(
				webUrl,
				format,
				device,
				delay,
				bSizeH,
				bSizeW,
				iSizeH,
				iSizeW);
			var imageFileName = GetImageFileName(requestUrl);
			var imageDisplayFilePath = Path.Combine(WEB_BROWSER_CAPTURE_DIR_PATH, imageFileName + "." + format.ToString().ToLower());
			return imageDisplayFilePath;
		}

		/// <summary>
		/// WebURL取得
		/// </summary>
		/// <param name="filePath">対象ファイルパス(ルート以下のディレクトリパス ファイル名・拡張子含む)</param>
		/// <param name="webUrlParams">キャプチャー生成時のURLパラメータ一覧</param>
		/// <returns>WebURL</returns>
		private static string GetWebUrl(string filePath, List<KeyValuePair<string, string>> webUrlParams = null)
		{
			// WebURL取得
			var webUrl = Constants.PROTOCOL_HTTP + Constants.SITE_DOMAIN + Constants.PATH_ROOT_FRONT_PC + filePath.Replace(@"\", "/");

			if (webUrlParams != null)
			{
				var webUrlCreator = new UrlCreator(webUrl);
				webUrlParams.ForEach(p => webUrlCreator.AddParam(p.Key, p.Value));
				webUrl = webUrlCreator.CreateUrl();
			}

			return webUrl;
		}

		/// <summary>
		/// WebブラウザキャプチャーAPIリクエストURLの取得
		/// </summary>
		/// <param name="webUrl">取得先WebURL</param>
		/// <param name="format">拡張子</param>
		/// <param name="device">デバイス</param>
		/// <param name="delay">キャプチャータイミング</param>
		/// <param name="bSizeH">縦横比 縦</param>
		/// <param name="bSizeW">縦横比 横</param>
		/// <param name="iSizeH">画像サイズ 縦</param>
		/// <param name="iSizeW">画像サイズ 横</param>
		/// <returns>WebブラウザキャプチャーAPIリクエストURL</returns>
		private static string GetApiRequestUrl(
			string webUrl,
			Format format = Format.Png,
			Device device = Device.Pc,
			int delay = 0,
			int bSizeH = 1900,
			int bSizeW = 1000,
			int iSizeH = 200,
			int iSizeW = 300)
		{
			// ブラウザキャプチャーURL生成
			var urlCreator = new UrlCreator(Constants.WEB_BROWSER_CAPTURE_API_URL);
			urlCreator.AddParam(PARAM_URL, webUrl);
			urlCreator.AddParam(PARAM_FORMAT, format.ToString().ToLower());
			urlCreator.AddParam(PARAM_DEVICE, device.ToString().ToLower());
			urlCreator.AddParam(PARAM_DELAY, delay.ToString());
			urlCreator.AddParam(PARAM_BSIZE, string.Format("{0}:{1}", bSizeH.ToString(), bSizeW.ToString()));
			urlCreator.AddParam(PARAM_ISIZE, string.Format("{0}:{1}", iSizeH.ToString(), iSizeW.ToString()));
			var requestUrl = urlCreator.CreateUrl();

			return requestUrl;
		}

		/// <summary>
		/// 画像ファイル名の取得 APIリクエストをSHA256でハッシュ
		/// </summary>
		/// <param name="requestUrl">WebブラウザキャプチャーAPIリクエストURL</param>
		/// <returns>画像ファイル名の取得</returns>
		private static string GetImageFileName(string requestUrl)
		{
			var hash = new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(requestUrl));
			var imageFileName = string.Join("", hash.Select(b => b.ToString("x2")));
			return imageFileName;
		}

		/// <summary>
		/// Webブラウザキャプチャー取得(非同期)
		/// </summary>
		/// <param name="requestUrl">WebブラウザキャプチャーAPIリクエストURL</param>
		/// <param name="imageFilePath">生成先 画像パス</param>
		/// <param name="temporaryPhysicalFilePath"></param>
		/// <returns>タスク</returns>
		private static async Task CreateImageFileAsync(string requestUrl, string imageFilePath, string temporaryPhysicalFilePath)
		{
			using (var httpClient = new HttpClient())
			{
				try
				{
					var response = await httpClient.GetAsync(requestUrl);
					using (var fileStream = new FileStream(imageFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
					using (var httpStream = await response.Content.ReadAsStreamAsync())
					{
						if (response.Content.Headers.ContentType.MediaType.Contains("image"))
						{
							httpStream.CopyTo(fileStream);
						}
						else
						{
							FileLogger.WriteError(string.Format("{0}のキャプチャー取得に失敗しました。", requestUrl));
						}
					}
				}
				catch (Exception ex)
				{
					FileLogger.WriteError(string.Format("{0}のキャプチャー取得に失敗しました。", requestUrl), ex);
				}

				if (File.Exists(temporaryPhysicalFilePath)) File.Delete(temporaryPhysicalFilePath);
			}
		}
	}
}
