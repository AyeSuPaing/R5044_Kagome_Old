/*
=========================================================================================================
  Module      : ワーカーサービス基底クラス(BaseWorkerService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using w2.App.Common.Util;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.ViewModels.ScoringSale;
using w2.Common.Logger;

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// ワーカーサービス基底クラス
	/// </summary>
	public class BaseWorkerService : IWorkerService
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="urlHelper">URLヘルパ（テストの場合ダミーがわたってくるが、その後呼び出し元で初期化）</param>
		protected BaseWorkerService(UrlHelper urlHelper = null)
		{
			this.SessionWrapper = new SessionWrapper();
			this.UrlHelper = urlHelper ?? new UrlHelper(
				(HttpContext.Current != null) ? HttpContext.Current.Request.RequestContext : new RequestContext());
		}

		/// <summary>
		/// ファイルを開いた時間を更新
		/// </summary>
		/// <param name="lpPageFilePath">LPページファイルパス</param>
		public void UpdateFileOpenTime(string lpPageFilePath)
		{
			this.SessionWrapper.UpdateFileOpenTime(lpPageFilePath);
		}

		/// <summary>
		/// リネーム
		/// </summary>
		/// <param name="current">現在選択中のディレクトリ</param>
		/// <param name="path">パス</param>
		/// <param name="rename">変更後のファイル/ディレクトリ名</param>
		/// <returns>エラーメッセージ</returns>
		protected string Rename(string current, string path, string rename)
		{
			var sourcePath = path;
			if (path.EndsWith(@"\"))
			{
				sourcePath = path.Substring(0, path.Length - 1);
			}

			// 親ディレクトリ取得
			var parentDirPath = Regex.Replace(sourcePath, @"([^\\]+?)?$", string.Empty);

			// ファイル名チェック
			var errorMessage = CheckFileName(rename);
			if (string.IsNullOrEmpty(errorMessage) == false) return errorMessage;

			try
			{
				var filePath = parentDirPath + rename;
				if (string.IsNullOrEmpty(current) == false)
				{
					if (current.EndsWith(@"\"))
					{
						Directory.Move(sourcePath, filePath);
					}
					else
					{
						File.Move(sourcePath, filePath);
					}
				}
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
				return WebMessages.ContentsManagerFileOperationError;
			}

			return string.Empty;
		}

		/// <summary>
		/// ファイル名チェック
		/// </summary>
		/// <param name="filePath">ファイルパス</param>
		/// <returns>エラーメッセージ</returns>
		public string CheckFileName(string filePath)
		{
			if (filePath.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
			{
				return WebMessages.FileNameError;
			}
			return string.Empty;
		}

		/// <summary>
		/// Get image list
		/// </summary>
		/// <param name="pathImageRoot">Path image root</param>
		/// <returns>List of view model</returns>
		public List<ScoringSaleImageViewModel> GetImageList(string pathImageRoot)
		{
			if (Directory.Exists(pathImageRoot) == false)
			{
				Directory.CreateDirectory(pathImageRoot);
			}

			var list = new List<ScoringSaleImageViewModel>();

			foreach (var filePath in Directory.GetFiles(pathImageRoot))
			{
				var file = new FileInfo(filePath);
				var imageScoringImage = pathImageRoot
					.Substring(Constants.PHYSICALDIRPATH_CONTENTS_ROOT.Length)
					.Replace("\\", "/");

				var model = new ScoringSaleImageViewModel(imageScoringImage)
				{
					ImageSrc = Path.Combine(
						Constants.PATH_ROOT_FRONT_PC,
						filePath
							.Substring(Constants.PHYSICALDIRPATH_CONTENTS_ROOT.Length)
							.Replace("\\", "/")),
					DataChanged = DateTimeUtility.ToStringForManager(
						file.LastWriteTime,
						DateTimeUtility.FormatType.LongDateHourMinuteSecond1Letter),
					FileSize = ConvertFileSize(file.Length),
				};

				var imageExtesion = new string[]
				{
					".jpg",
					".jpeg",
					".png",
					".gif",
					".bmp",
					".tiff"
				};

				if (imageExtesion.Contains(Path.GetExtension(filePath).ToLower()))
				{
					try
					{
						using (var img = Image.FromFile(filePath))
						{
							model.ImageSize = string.Format("{0} x {1}px", img.Width, img.Height);
						}
					}
					catch (Exception)
					{
						model.ImageSize = "-";
					}
				}

				if (Path.GetExtension(filePath).ToLower() == ".svg")
				{
					model.ImageSize = "-";
				}

				list.Add(model);
			}

			return list;
		}

		/// <summary>
		/// Convert file size
		/// </summary>
		/// <param name="size">Size</param>
		/// <returns>File size</returns>
		public string ConvertFileSize(long size)
		{
			var fileSize = string.Empty;

			if (size < 1024)
			{
				fileSize = "1KB";
			}
			else if (size <= 1048576)
			{
				fileSize = string.Format("{0}KB", (size / 1000));
			}
			else
			{
				var milibyte = size / 1048576f;
				fileSize = string.Format("{0}MB", milibyte.ToString("F1"));
			}

			return fileSize;
		}

		#region プロパティ
		/// <summary>セッションラッパー</summary>
		public SessionWrapper SessionWrapper { get; set; }
		/// <summary>URLヘルパ</summary>
		public UrlHelper UrlHelper { get; set; }
		#endregion
	}
}