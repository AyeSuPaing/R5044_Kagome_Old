/*
=========================================================================================================
  Module      : コンテンツマネージャワーカーサービス(ContentsManagerWorkerService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Xml.Linq;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.Codes.Settings;
using w2.Cms.Manager.Codes.TreeView;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ViewModels.ContentsManager;
using w2.Common.Logger;
using w2.Common.Util.Archiver;

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// コンテンツマネージャワーカーサービス
	/// </summary>
	public class ContentsManagerWorkerService : BaseWorkerService
	{
		/// <summary>フォルダ作成時の接尾辞</summary>
		private const string CONST_NEWDIRECTORY_NAME = "_NewDirectory";
		/// <summary>ROOTアイコン</summary>
		private const string ROOT_IMAGE = "Images/Directory/root.jpg";
		/// <summary>ディレクトリアイコン</summary>
		private const string DIRECTORY_ICON = "Images/Directory/directory.gif";
		/// <summary>ASPXアイコン</summary>
		private const string ASPX_ICON = "Images/Directory/aspx.gif";
		/// <summary>Htmlアイコン</summary>
		private const string HTML_ICON = "Images/Directory/html.gif";
		/// <summary>イメージアイコン</summary>
		private const string IMAGE_ICON = "Images/Directory/image.gif";
		/// <summary>その他アイコン</summary>
		private const string OTHER_ICON = "Images/Directory/other.gif";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ContentsManagerWorkerService()
		{
		}

		/// <summary>
		/// ツリービュー作成
		/// </summary>
		/// <returns>ビューモデル</returns>
		internal ContentsViewModel CreateContentsViewModel()
		{
			return new ContentsViewModel();
		}

		/// <summary>
		/// コンテンツマネージャー設定
		/// </summary>
		/// <param name="sessionId">セッションID</param>
		public void SetContentsManagerSetting(string sessionId)
		{
			this.SessionWrapper.ContentsMnagerClickCurrent = string.Empty;
			this.SessionWrapper.ContentsMnagerTempDirPath = Path.Combine(Constants.PHYSICALDIRPATH_TEMP, sessionId);
		}

		/// <summary>
		/// ツリー作成
		/// </summary>
		/// <param name="openDirPathList">展開中のディレクトリパスリスト</param>
		/// <param name="clickPath">クリックしたパス</param>
		/// <returns>ツリー</returns>
		internal IEnumerable<TreeNode> CreateTreeView(string[] openDirPathList, string clickPath = "")
		{
			this.SessionWrapper.ContentsMnagerClickCurrent = clickPath;

			var rootNode = new TreeNode
			{
				Text = "ROOT",
				Value = string.Empty,
				ImageUrl = ROOT_IMAGE,
				IsDir = true,
				IsOpen = openDirPathList.Contains(string.Empty),
				IsSelected = string.IsNullOrEmpty(clickPath),
			};

			if(rootNode.IsOpen) CreateChildNode(openDirPathList, rootNode, clickPath);

			var tree = new List<TreeNode>
			{
				rootNode
			}.AsEnumerable();

			return tree;
		}

		/// <summary>
		/// 再帰的に子ノード作成
		/// </summary>
		/// <param name="openDirPathList">展開中のディレクトリパスリスト</param>
		/// <param name="parent">親ノード</param>
		/// <param name="clickPath">クリックしたパス</param>
		private void CreateChildNode(string[] openDirPathList, TreeNode parent, string clickPath)
		{
			// 物理ディレクトリパス作成
			var targetRealPath = Path.Combine(Constants.PHYSICALDIRPATH_CONTENTS_ROOT, parent.Value);

			// 各ディレクトリ作成
			var dirs = Directory.GetDirectories(targetRealPath);
			dirs.Where(
				d => ContentsManagerSetting.GetInstance().RefuseDirList.Any(
					// 拒否リストにあれば表示しない
					rdl => (rdl == d.Replace(Constants.PHYSICALDIRPATH_CONTENTS_ROOT, string.Empty).ToLower()))
					== false).ToList().ForEach(
				directory =>
				{
					// 親フォルダのパス文字列を切り捨てることでファイル名のみを取得
					var path = directory.Substring(targetRealPath.Length);

					var dir = new TreeNode
					{
						Text = path,
						Value = parent.Value + path + @"\",
						ImageUrl = DIRECTORY_ICON,
						IsDir = true,
						IsSelected = (parent.Value + path + @"\" == clickPath),
					};

					if (openDirPathList.Contains(dir.Value))
					{
						dir.IsOpen = true;
						CreateChildNode(openDirPathList, dir, clickPath);
					}

					parent.Childs.Add(dir);
				});

			// 各ファイル作成
			var files = TreeViewHelper.GetTreeViewFiles(targetRealPath);
			var errorMessage = TreeViewHelper.GetTreeViewErrorMessage(files);
			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				parent.ErrorMessage = errorMessage;
				parent.IsOpen = false;
				return;
			}

			files.Where(
				f => ContentsManagerSetting.GetInstance().RefuseFileList.Any(
					rfl => rfl == (f.Replace(Constants.PHYSICALDIRPATH_CONTENTS_ROOT, string.Empty).ToLower()))
					== false).ToList().ForEach(
				fileName =>
				{
					var path = fileName.Substring(targetRealPath.Length);

					var file = new TreeNode
					{
						Text = path,
						Value = parent.Value + path,
						IsSelected = (parent.Value + path == clickPath),
					};

					switch (Path.GetExtension(path).ToLower())
					{
						case ".aspx":
						case ".js":
						case ".css":
						case ".xml":
						case ".config":
						case ".asax":
							file.ImageUrl = ASPX_ICON;
							break;

						case ".html":
						case ".htm":
							file.ImageUrl = HTML_ICON;
							break;

						case ".jpg":
						case ".jpeg":
						case ".gif":
						case ".ico":
						case ".png":
						case ".bmp":
							file.ImageUrl = IMAGE_ICON;
							break;

						default:
							file.ImageUrl = OTHER_ICON;
							break;
					}

					parent.Childs.Add(file);
				});
		}

		#region クリック
		/// <summary>
		/// クリック
		/// </summary>
		/// <param name="clickPath">クリックしたパス</param>
		/// <param name="clickDir">ディレクトリ展開か</param>
		/// <param name="comeFromShortCut">ショートカットから来てるかどうか</param>
		/// <param name="openDirPathList">展開中のディレクトリパスリスト</param>
		/// <returns>ツリー</returns>
		internal IEnumerable<TreeNode> Click(string clickPath, bool clickDir, bool comeFromShortCut, string[] openDirPathList)
		{
			// ショットカートクリックした後、対象フォルダ存在を確認
			if (comeFromShortCut)
			{
				var treeView = (Directory.Exists(Path.Combine(Constants.PHYSICALDIRPATH_CONTENTS_ROOT, clickPath)) == false)
					? new ContentsManagerWorkerService().CreateTreeView(openDirPathList)
					: ExpandDirectoryByShortCut(clickPath);

				return treeView;
			}

			return ExpandDirectoryByManual(openDirPathList, clickPath, clickDir);
		}
		#endregion

		#region ダウンロード
		/// <summary>
		/// ダウンロード
		/// </summary>
		/// <param name="response">レスポンス</param>
		internal void Download(HttpResponseBase response)
		{
			var downloadTarget = Path.Combine(Constants.PHYSICALDIRPATH_CONTENTS_ROOT, this.SessionWrapper.ContentsMnagerClickCurrent);

			if (downloadTarget.EndsWith(@"\"))
			{
				downloadTarget = downloadTarget.Substring(0, downloadTarget.LastIndexOf(@"\"));	// 最後の\削除

				// ディレクトリダウンロード(Zip圧縮)
				if (Directory.Exists(downloadTarget) == false) return;
				// ZIP圧縮
				var tempRefuseList = ContentsManagerSetting.GetInstance().RefuseListIncludeContentsRoot.ToList();
				tempRefuseList.AddRange(ContentsManagerSetting.GetInstance().RefuseList
					.Select(x => Path.Combine(this.SessionWrapper.ContentsMnagerTempDirPath, x)));
				var zaZipArchiver = new ZipArchiver(tempRefuseList);

				response.HeaderEncoding = Encoding.GetEncoding("Shift_JIS");
				response.ContentType = "application/zip";
				response.AppendHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(downloadTarget) + ".zip");

				zaZipArchiver.CompressToStream(downloadTarget, Constants.PHYSICALDIRPATH_CONTENTS_ROOT, response.OutputStream);

				response.End();
			}
			else
			{
				if (File.Exists(downloadTarget) == false) return;
				using (var fs = new FileStream(downloadTarget, FileMode.Open, FileAccess.Read))
				using (var br = new BinaryReader(fs))
				{
					// ファイル出力
					var bBuffer = new byte[(int)fs.Length];

					response.HeaderEncoding = Encoding.GetEncoding("Shift_JIS");
					response.ContentType = "application/octet-stream";
					response.AppendHeader(
						"Content-Disposition",
						"attachment; filename=" + downloadTarget.Substring(downloadTarget.LastIndexOf(@"\") + 1));

					bBuffer = br.ReadBytes((int)fs.Length);
					response.BinaryWrite(bBuffer);
					response.End();
				}
			}
		}
		#endregion

		#region フォルダ作成
		/// <summary>
		/// フォルダ作成
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		internal string MakeDirectory()
		{
			// ファイルを選択中の場合
			if (this.SessionWrapper.ContentsMnagerClickCurrent.EndsWith(@"\") == false)
			{
				this.SessionWrapper.ContentsMnagerClickCurrent = Regex.Replace(this.SessionWrapper.ContentsMnagerClickCurrent, @"([^\\]+?)?$", string.Empty);
			}

			var newDirectoryFilePathBase = Path.Combine(
				Constants.PHYSICALDIRPATH_CONTENTS_ROOT,
				this.SessionWrapper.ContentsMnagerClickCurrent,
				CONST_NEWDIRECTORY_NAME);

			var suffixNum = 1;
			var newDirectoryFilePath = newDirectoryFilePathBase;
			while (Directory.Exists(newDirectoryFilePath))
			{
				newDirectoryFilePath = newDirectoryFilePathBase + suffixNum++;
			}

			try
			{
				Directory.CreateDirectory(newDirectoryFilePath);

				// 作成後ディレクトリを選択する
				this.SessionWrapper.ContentsMnagerClickCurrent = newDirectoryFilePath.Substring(Constants.PHYSICALDIRPATH_CONTENTS_ROOT.Length) + @"\";
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
				return WebMessages.ContentsManagerFileOperationError;
			}

			return string.Empty;
		}
		#endregion

		#region 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="current">現在選択中のディレクトリ</param>
		/// <returns>エラーメッセージ</returns>
		internal string Delete(string current)
		{
			var success = false;
			var deleteFilePath = Path.Combine(Constants.PHYSICALDIRPATH_CONTENTS_ROOT, this.SessionWrapper.ContentsMnagerClickCurrent);

			try
			{
				// ディレクトリ削除
				if (deleteFilePath.EndsWith(@"\"))
				{
					if ((this.SessionWrapper.ContentsMnagerClickCurrent == string.Empty) || ContentsManagerSetting.GetInstance().NotDeleteList.Any(ndl => ndl == current.ToLower()))
					{
						return WebMessages.ContentsManagerDeleteError;
					}

					if (Directory.Exists(deleteFilePath) == false) return string.Empty;
					// ディレクトリ削除
					Directory.Delete(deleteFilePath, true);
					
					success = true;
				}
				// ファイル削除
				else
				{
					if (File.Exists(deleteFilePath))
					{
						// 書き込み専用でなければ削除
						if ((File.GetAttributes(deleteFilePath) & FileAttributes.ReadOnly) != FileAttributes.ReadOnly)
						{
							// ファイル削除
							File.Delete(deleteFilePath);

							success = true;
						}
						else
						{
							// 読みとり専用になっている
							return WebMessages.ContentsManagerFileOperationError;
						}
					}
				}

				// ファイル削除ログ出力
				if (success)
				{
					var logMessage = new StringBuilder();
					logMessage.Append("オペレータ：").Append(this.SessionWrapper.LoginOperatorName).Append("　");
					logMessage.Append("ファイル：").Append(deleteFilePath);

					WriteCMSLog("DELETE", logMessage.ToString());
				}
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
				return WebMessages.ContentsManagerFileOperationError;
			}

			// 親を選択する
			var reg = (this.SessionWrapper.ContentsMnagerClickCurrent.EndsWith(@"\"))
				? new Regex(@"([^\\]+?)?\\$")
				: new Regex(@"([^\\]+?)?$");
			this.SessionWrapper.ContentsMnagerClickCurrent = reg.Replace(this.SessionWrapper.ContentsMnagerClickCurrent, string.Empty);

			return string.Empty;
		}
		#endregion

		#region リネーム
		/// <summary>
		/// リネーム
		/// </summary>
		/// <param name="rename">変更後のファイル/ディレクトリ名</param>
		/// <returns>エラーメッセージ</returns>
		internal string Rename(string rename)
		{
			var errorMessage = Rename(
				this.SessionWrapper.ContentsMnagerClickCurrent,
				Path.Combine(Constants.PHYSICALDIRPATH_CONTENTS_ROOT,
					this.SessionWrapper.ContentsMnagerClickCurrent),
				rename);
			return errorMessage;
		}
		#endregion

		#region ファイルアップロード
		/// <summary>
		/// ファイルアップロード
		/// </summary>
		/// <param name="input">入力</param>
		/// <returns>エラーメッセージか終了メッセージ</returns>
		internal string UploadFile(ContentsManagerInput input)
		{
			// ディレクトリ存在チェック（存在しない場合は作成）
			if (Directory.Exists(this.SessionWrapper.ContentsMnagerTempDirPath) == false) Directory.CreateDirectory(this.SessionWrapper.ContentsMnagerTempDirPath);

			var fileName = input.UploadContents.FileName;

			var targetDirPath = Path.GetDirectoryName(Path.Combine(Constants.PHYSICALDIRPATH_CONTENTS_ROOT, this.SessionWrapper.ContentsMnagerClickCurrent));
			var targetFilePath = Path.Combine(targetDirPath, fileName);

			try
			{
				// ファイルアップロード実行（一時保存場所へ保存）
				var tempUploadFilePath = "";
				// IEだとC:\から始まる
				tempUploadFilePath = Path.Combine(
					this.SessionWrapper.ContentsMnagerTempDirPath,
					fileName.Contains(@"\")
						? new Regex(@"([^\\]+?)?$").Match(fileName).Value
						: fileName);
				input.UploadContents.SaveAs(tempUploadFilePath);

				// zip解凍（zipファイルでない場合はリストにファイルパスを設定）
				var uploadTempFilePathList = new List<string>();
				if ((Path.GetExtension(targetFilePath).ToLower().Contains(".zip")) && (input.ZipDecompress))
				{
					var tempRefuseList = ContentsManagerSetting.GetInstance().RefuseListIncludeContentsRoot.ToList();
					tempRefuseList.AddRange(ContentsManagerSetting.GetInstance().RefuseList.Select(x => Path.Combine(this.SessionWrapper.ContentsMnagerTempDirPath, x)));
					var zipArchiver = new ZipArchiver(tempRefuseList);
					uploadTempFilePathList = zipArchiver.Decompress(
						tempUploadFilePath,
						this.SessionWrapper.ContentsMnagerTempDirPath,
						Regex.Replace(this.SessionWrapper.ContentsMnagerClickCurrent, @"([^\\]+?)?$", string.Empty));

					File.Delete(tempUploadFilePath);
				}
				else
				{
					uploadTempFilePathList.Add(tempUploadFilePath);
				}

				// ファイル属性更新
				foreach (var filePath in uploadTempFilePathList)
				{
					// 読取属性を削除（※ファイル削除エラー回避のため）
					// 更新日時を更新（※キャッシュの再フィッチのため）
					var file = new FileInfo(filePath)
					{
						IsReadOnly = false,
						LastWriteTime = DateTime.Now
					};
				}

				// XML構文チェック
				var errorMessages = new StringBuilder();

				uploadTempFilePathList.Where(utfp => utfp.EndsWith(".xml")).ToList().ForEach(
					xml =>
					{
						try
						{
							XDocument.Load(xml);
						}
						catch (Exception ex)
						{
							// エラーメッセージ作成
							if (errorMessages.Length != 0) errorMessages.Append("<br/>");

							var filePaths = xml.Split('\\');
							var exceptionMessages = ex.Message.Replace("\r\n", "\n").Split('\n');
							errorMessages.Append(
								WebMessages.ContentsManagerXmlFormatError
									.Replace("@@ 1 @@", filePaths[filePaths.Length - 1])
									.Replace("@@ 2 @@", "<br/>" + exceptionMessages[0]));

							FileLogger.WriteError(ex);
						}
						
					});

				// 画像リサイズ可能チェック
				if (input.AutoResize)
				{
					var filePathList = uploadTempFilePathList.Where(
						ufp => Path.GetExtension(ufp).ToLower().Contains(".jpg") == false
							&& Path.GetExtension(ufp).ToLower().Contains(".jpeg") == false
							&& Path.GetExtension(ufp).ToLower().Contains(".gif") == false
							&& Path.GetExtension(ufp).ToLower().Contains(".bmp") == false
							&& Path.GetExtension(ufp).ToLower().Contains(".png") == false).ToArray();

					if (filePathList.Any())
					{
						if (errorMessages.Length != 0) errorMessages.Append("<br/>");
						errorMessages.Append(WebMessages.ContentsManagerImageResizeError);
						foreach (var filePath in filePathList)
						{
							var uploadFileName = filePath.Split('\\');
							errorMessages.Append(uploadFileName.Last());
							errorMessages.Append("<br />");
						}
					}
				}

				// ファイルを正式な場所へコピー＆一時保存ファイルの削除
				// （zipファイルの場合、全てのファイルがアップ出来る状態のみコピー処理を行う）
				var uploadFilePathList = new List<string>();
				foreach (var filePath in uploadTempFilePathList)
				{
					if (errorMessages.Length == 0)
					{
						var uploadFilePath = ((Path.GetExtension(targetFilePath).ToLower().Contains(".zip")) && (input.ZipDecompress))
							? Path.Combine(Constants.PHYSICALDIRPATH_CONTENTS_ROOT, filePath.Replace(this.SessionWrapper.ContentsMnagerTempDirPath + @"\", string.Empty))
							: Path.Combine(targetDirPath, filePath.Replace(this.SessionWrapper.ContentsMnagerTempDirPath, string.Empty).Substring(1));

						// ディレクトリが存在しない場合は階層を再現
						if (Directory.Exists(Path.GetDirectoryName(uploadFilePath)) == false) Directory.CreateDirectory(Path.GetDirectoryName(uploadFilePath));

						File.Copy(filePath, uploadFilePath, true);
						uploadFilePathList.Add(uploadFilePath);
					}

					File.Delete(filePath);
				}

				// 一時保存ディレクトリをサブディレクトリ含めて削除
				Directory.Delete(this.SessionWrapper.ContentsMnagerTempDirPath, true);

				// xml構文エラーのエラーメッセージ設定（エラーが存在する場合はここで処理終了）
				if (errorMessages.Length != 0) return errorMessages.ToString();

				// 必要なら画像リサイズ
				if (input.AutoResize)
				{
					uploadFilePathList.Where(
						ufp => Path.GetExtension(ufp).ToLower().Contains(".jpg")
						|| Path.GetExtension(ufp).ToLower().Contains(".jpeg")
						|| Path.GetExtension(ufp).ToLower().Contains(".gif")
						|| Path.GetExtension(ufp).ToLower().Contains(".bmp")
						|| Path.GetExtension(ufp).ToLower().Contains(".png"))
						.ToList().ForEach(resizeFilePath =>
						{
							var resizeFilePathBase = resizeFilePath.Substring(0, resizeFilePath.LastIndexOf('.'));
							if (IsSelectImages(this.SessionWrapper.ContentsMnagerClickCurrent))
							{
								ImageConvert.ResizeImage(
									resizeFilePath,
									resizeFilePathBase + Constants.PRODUCTIMAGE_FOOTER_S,
									(input.ImageSizeWidthS == string.Empty) ? (int?)null : int.Parse(input.ImageSizeWidthS),
									(input.ImageSizeHeightS == string.Empty) ? (int?)null : int.Parse(input.ImageSizeHeightS));
							}

							// 中サイズへリサイズ
							ImageConvert.ResizeImage(
								resizeFilePath,
								resizeFilePathBase + Constants.PRODUCTIMAGE_FOOTER_M,
								(input.ImageSizeWidthM == string.Empty) ? (int?)null : int.Parse(input.ImageSizeWidthM),
								(input.ImageSizeHeightM == string.Empty) ? (int?)null : int.Parse(input.ImageSizeHeightM));

							// 大サイズへリサイズ
							ImageConvert.ResizeImage(
								resizeFilePath,
								resizeFilePathBase + Constants.PRODUCTIMAGE_FOOTER_L,
								(input.ImageSizeWidthL == string.Empty) ? (int?)null : int.Parse(input.ImageSizeWidthL),
								(input.ImageSizeHeightL == string.Empty) ? (int?)null : int.Parse(input.ImageSizeHeightL));

							// 拡大サイズへリサイズ
							ImageConvert.ResizeImage(
								resizeFilePath,
								resizeFilePathBase + Constants.PRODUCTIMAGE_FOOTER_LL,
								(input.ImageSizeWidthLL == string.Empty) ? (int?)null : int.Parse(input.ImageSizeWidthLL),
								(input.ImageSizeHeightLL == string.Empty) ? (int?)null : int.Parse(input.ImageSizeHeightLL));

							// 元画像ファイル削除
							File.Delete(resizeFilePath);
						});
				}

				// ファイルアップロードログ出力
				var logMessage = new StringBuilder();
				logMessage.Append("オペレータ：").Append(this.SessionWrapper.LoginOperatorName).Append("　");
				logMessage.Append("ファイル：").Append(targetFilePath);

				WriteCMSLog("UPLOAD", logMessage.ToString());

				return Constants.TAG_REPLACER_DATA_SCHEMA.GetValue(
					"@@DispText.file_upload.success@@",
					Constants.GLOBAL_OPTION_ENABLE
						? Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE
						: "");

			}
			catch (UnauthorizedAccessException ex)
			{
				FileLogger.WriteError(ex);
				return WebMessages.ContentsManagerFileOperationError;
			}
		}
		#endregion

		/// <summary>
		/// リサイズ
		/// </summary>
		/// <param name="imageType">イメージかサブイメージか</param>
		/// <returns>コンテンツマネージャ入力</returns>
		internal ContentsManagerInput ReSize(string imageType)
		{
			var input = new ContentsManagerInput();

			var isSelectSubImage = imageType == "productSubImage";
			var imageSizeS = new List<string>((isSelectSubImage) ? new [] { "0", "0" } : Constants.PRODUCTIMAGE_SIZE_S.Split(','));
			var imageSizeM = new List<string>((isSelectSubImage) ? Constants.PRODUCTSUBIMAGE_SIZE_M.Split(',') : Constants.PRODUCTIMAGE_SIZE_M.Split(','));
			var imageSizeL = new List<string>((isSelectSubImage) ? Constants.PRODUCTSUBIMAGE_SIZE_L.Split(',') : Constants.PRODUCTIMAGE_SIZE_L.Split(','));
			var imageSizeLL = new List<string>((isSelectSubImage) ? Constants.PRODUCTSUBIMAGE_SIZE_LL.Split(',') : Constants.PRODUCTIMAGE_SIZE_LL.Split(','));

			// 横幅設定
			input.ImageSizeWidthS = imageSizeS[0].Trim();
			input.ImageSizeWidthM = imageSizeM[0].Trim();
			input.ImageSizeWidthL = imageSizeL[0].Trim();
			input.ImageSizeWidthLL = imageSizeLL[0].Trim();

			// 縦幅設定
			input.ImageSizeHeightS = (imageSizeS.Count == 1) ? imageSizeS[0].Trim() : imageSizeS[1].Trim();
			input.ImageSizeHeightM = (imageSizeM.Count == 1) ? imageSizeM[0].Trim() : imageSizeM[1].Trim();
			input.ImageSizeHeightL = (imageSizeL.Count == 1) ? imageSizeL[0].Trim() : imageSizeL[1].Trim();
			input.ImageSizeHeightLL = (imageSizeLL.Count == 1) ? imageSizeLL[0].Trim() : imageSizeLL[1].Trim();

			return input;
		}



		/// <summary>
		/// CMSログ出力
		/// </summary>
		/// <param name="operationKbn">操作区分</param>
		/// <param name="message">メッセージ</param>
		private void WriteCMSLog(string operationKbn, string message)
		{
			// 書き込みログファイル名決定
			var now = DateTime.Now;
			var logFilePath = Path.Combine(
				Constants.PHYSICALDIRPATH_LOGFILE,
				string.Format("manager_cms_{0}.{1}", now.ToString("yyyyMM"), Constants.LOGFILE_EXTENSION));

			try
			{
				// Mutexで排他制御しながらファイル書き込み
				using (var mtx = new Mutex(false, logFilePath.Replace(@"\", "_") + ".FileWrite"))
				{
					try
					{
						mtx.WaitOne();

						using (var sw = new StreamWriter(logFilePath, true, Encoding.Default))
						{
							// ファイル書き込み
							sw.Write(
								"[{0}] {1} {2}\r\n",
								operationKbn,
								DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss"),
								message);
							sw.Close();
						}
					}
					finally
					{
						mtx.ReleaseMutex();
					}
				}
			}
			catch (Exception ex)
			{
				// 例外の場合はログ出力してやりすごす
				FileLogger.WriteError(ex);
			}
		}

		/// <summary>
		/// 商品画像一括削除
		/// </summary>
		/// <param name="fileNameWithoutSize">商品画像ファイル名(サイズ以降除く)</param>
		/// <returns>エラーメッセージ</returns>
		public string DeleteProductImages(string fileNameWithoutSize)
		{
			try
			{
				var targetDirPath = Path.GetDirectoryName(Path.Combine(Constants.PHYSICALDIRPATH_CONTENTS_ROOT, this.SessionWrapper.ContentsMnagerClickCurrent));
				var targetDirAndFileName = Path.Combine(targetDirPath, fileNameWithoutSize);

				var deleteImagePrefix = (IsSelectImages(this.SessionWrapper.ContentsMnagerClickCurrent))
					? targetDirAndFileName
					: targetDirAndFileName.Replace(@"Contents\ProductSubImages", @"Contents\ProductImages");

				var deleteSubImagePrefix = (IsSelectImages(this.SessionWrapper.ContentsMnagerClickCurrent))
					? targetDirAndFileName.Replace(@"Contents\ProductImages", @"Contents\ProductSubImages")
					: targetDirAndFileName;

				DeleteStoLlImages(deleteImagePrefix);
				DeleteMtoLlImages(deleteSubImagePrefix);
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
				return WebMessages.ContentsManagerFileOperationError;
			}

			// ファイルを選択中なら親を選択する
			if (this.SessionWrapper.ContentsMnagerClickCurrent.EndsWith(@"\") == false)
			{
				this.SessionWrapper.ContentsMnagerClickCurrent = Regex.Replace(
					this.SessionWrapper.ContentsMnagerClickCurrent, @"([^\\]+?)?$",
					string.Empty);
			}

			return string.Empty;
		}

		/// <summary>
		/// SからLLの画像削除
		/// </summary>
		/// <param name="deleteImagePrefix">ファイル名含むサイズ除くパス</param>
		public void DeleteStoLlImages(string deleteImagePrefix)
		{
			DeleteResizeImage(deleteImagePrefix + "_S.jpg");

			DeleteMtoLlImages(deleteImagePrefix);
		}

		/// <summary>
		/// MからLLの画像削除
		/// </summary>
		/// <param name="deleteImagePrefix">ファイル名含むサイズ除くパス</param>
		public void DeleteMtoLlImages(string deleteImagePrefix)
		{
			DeleteResizeImage(deleteImagePrefix + "_M.jpg");
			DeleteResizeImage(deleteImagePrefix + "_L.jpg");
			DeleteResizeImage(deleteImagePrefix + "_LL.jpg");
		}

		/// <summary>
		/// 商品画像削除とログ出力
		/// </summary>
		/// <param name="imagePath">画像ファイルパス</param>
		public void DeleteResizeImage(string imagePath)
		{
			if (File.Exists(imagePath) == false) return;
			File.Delete(imagePath);

			var logMessage = new StringBuilder();
			logMessage.Append("オペレータ：").Append(this.SessionWrapper.LoginOperatorName).Append("　");
			logMessage.Append("ファイル：").Append(imagePath);

			WriteCMSLog("DELETE", logMessage.ToString());
		}

		/// <summary>
		/// 手動でディレクトリを展開する
		/// </summary>
		/// <param name="openDirPathList">展開中のディレクトリパスリスト</param>
		/// <param name="clickPath">クリックしたパス</param>
		/// <param name="clickDir">ディレクトリをクリックしたかどうか</param>
		/// <returns></returns>
		private IEnumerable<TreeNode> ExpandDirectoryByManual(string[] openDirPathList, string clickPath, bool clickDir)
		{
			var temp = openDirPathList;
			if (clickDir)
			{
				temp = OpenAndCloseDirctory(openDirPathList, clickPath);
			}

			var treeView = CreateTreeView(temp, clickPath);
			if (Constants.CONTENTSMANAGER_CONTENTS_SHORTCUT_LIST.All(sl => (sl.Value != clickPath))) return treeView;
			SetCurrentPath(clickPath);

			return treeView;
		}

		/// <summary>
		/// ショートカットによってディレクトリを展開する
		/// </summary>
		/// <param name="clickPath"></param>
		/// <returns></returns>
		private IEnumerable<TreeNode> ExpandDirectoryByShortCut(string clickPath)
		{
			// ショートカットのみ開くために一度初期化
			var defaultTree = new [] { "" }; 

			// ディレクトリ展開
			IEnumerable<TreeNode> treeView;
			treeView = CreateTreeView(defaultTree);
			var shortcutPath = SetCurrentPath(clickPath);
			ExpandShortCut(defaultTree, treeView.ElementAt(0), shortcutPath);
			return treeView;
		}

		/// <summary>
		/// ディレクトリを開いてたら閉じ、閉じてたら開く
		/// </summary>
		/// <param name="openDirPathList">展開中のディレクトリパスリスト</param>
		/// <param name="path">操作するパス</param>
		private string[] OpenAndCloseDirctory(string[] openDirPathList, string path)
		{
			var temp = openDirPathList.ToList();
			if (openDirPathList.Contains(path))
			{
				temp.Remove(path);
			}
			else
			{
				temp.Add(path);
			}
			return temp.ToArray();
		}

		/// <summary>
		/// Currentパスを設定する
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		private string SetCurrentPath(string path)
		{
			var resultPath = path.Replace("/", @"\");
			this.SessionWrapper.ContentsMnagerClickCurrent = resultPath;
			return resultPath;
		}

		/// <summary>
		/// ショートカットのパスまで展開
		/// </summary>
		/// <param name="openDirPathList">展開中のディレクトリパスリスト</param>
		/// <param name="treeNode">ノード</param>
		/// <param name="selectPath">ショートカットのパス</param>
		private void ExpandShortCut(string[] openDirPathList, TreeNode treeNode, string selectPath)
		{
			if (treeNode.Value == selectPath)
			{
				treeNode.IsSelected = true;
				return;
			}

			// 展開先を探す
			var childNode = treeNode.Childs.First(cn => selectPath.StartsWith(cn.Value));

			// 性能等を考慮し、一部ディレクトリは展開しない
			var notExpandDirectory = new[]
			{
				Constants.CONTENTSMANAGER_CONTENTS_SHORTCUT_LIST[0].Key,
				Constants.CONTENTSMANAGER_CONTENTS_SHORTCUT_LIST[1].Key
			};
			if (notExpandDirectory.All(x => x != childNode.Value))
			{
				ExpandIncludingPathOnWay(openDirPathList, childNode, selectPath);
			}

			// 再帰
			ExpandShortCut(openDirPathList, childNode, selectPath);
		}

		/// <summary>
		/// 途中のパスも含めて展開する
		/// </summary>
		/// <param name="openDirPathList">展開中のディレクトリパスリスト</param>
		/// <param name="node">ノード</param>
		/// <param name="selectPath">展開するパス</param>
		private void ExpandIncludingPathOnWay(string[] openDirPathList, TreeNode node, string selectPath)
		{
			node.IsOpen = true;
			var temp = openDirPathList.ToList();
			// 途中まで展開済である場合を考慮
			while (node.Childs.Count != 0)
			{
				if (node.Value == selectPath)
				{
					node.IsSelected = true;
					return;
				}
				node = node.Childs.First(cn => selectPath.StartsWith(cn.Value));
			}
			// 開いてるパスに追加
			temp.Add(node.Value);
			// 展開
			CreateChildNode(temp.ToArray(), node, selectPath);
		}


		/// <summary>
		/// 商品Imageフォルダを選択しているか
		/// </summary>
		/// <param name="current">現在選択中のディレクトリ</param>
		/// <returns>判断結果</returns>
		public static bool IsSelectImages(string current)
		{
			var result = current.Contains(@"Contents\ProductImages");
			return result;
		}

		/// <summary>
		/// 商品サブImageフォルダを選択しているか
		/// </summary>
		/// <param name="current">現在選択中のディレクトリ</param>
		/// <returns>判断結果</returns>
		public static bool IsSelectSubImages(string current)
		{
			var result = current.Contains(@"Contents\ProductSubImages");
			return result;
		}
	}
}