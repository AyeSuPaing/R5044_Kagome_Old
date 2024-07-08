/*
=========================================================================================================
  Module      : コンテンツマネージャワーカーサービス(DesignSettingWorkerService.cs)
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
using Castle.Core.Internal;
using w2.App.Common;
using w2.App.Common.Design;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.Codes.PageDesign;
using w2.Cms.Manager.Codes.TreeView;
using w2.Cms.Manager.ViewModels.ProgramFileDesignSetting;
using w2.Common.Logger;
using TreeNode = w2.Cms.Manager.Codes.TreeView.TreeNode;

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// コンテンツマネージャワーカーサービス
	/// </summary>
	public abstract class ProgramFileDesignSettingWorkerService : BaseWorkerService
	{
		/// <summary>フォルダ作成時の接尾辞</summary>
		private const string CONST_NEWDIRECTORY_NAME = "_NewDirectory";
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
		/// <summary>新規ファイルのベース名(JavaScript)</summary>
		private const string CONST_NEW_JAVASCRIPT_FILE_BASENAME = "_NewJavaScriptFile";
		/// <summary>新規ファイルのベース名(CSS)</summary>
		private const string CONST_NEW_CSS_FILE_BASENAME = "_NewCssScriptFile";
		/// <summary>新規ファイルの拡張子(JavaScript)</summary>
		private const string CONST_NEW_JAVASCRIPT_FILE_EXTENSION = ".js";
		/// <summary>新規ファイルの拡張子(CSS)</summary>
		private const string CONST_NEW_CSS_FILE_EXTENSION = ".css";
		/// <summary>ファイルタイプ列挙体</summary>
		public enum FileTypes
		{
			Css,
			JavaScript
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ProgramFileDesignSettingWorkerService(FileTypes fileType)
		{
			this.FileType = fileType;
		}

		/// <summary>
		/// ツリービュー作成
		/// </summary>
		/// <returns>ビューモデル</returns>
		internal ProgramFileDesignSettingViewModel CreateContentsViewModel()
		{
			var designSettingViewModel = new ProgramFileDesignSettingViewModel(CreateTreeView(new []{ "" }));
			return designSettingViewModel;
		}

		/// <summary>
		/// コンテンツマネージャー設定
		/// </summary>
		public void SetDesignSetting()
		{
			this.ClickCurrentPathSession = string.Empty;
			this.SelectPathSession = string.Empty;
			this.PathRootFrontSession = Constants.PATH_ROOT_FRONT_PC;
		}

		/// <summary>
		/// ツリー作成
		/// </summary>
		/// <param name="openDirPathList">展開中のディレクトリパスリスト</param>
		/// <param name="clickPath">クリックしたパス</param>
		/// <returns>ツリー</returns>
		internal IEnumerable<TreeNode> CreateTreeView(string[] openDirPathList, string clickPath = "")
		{
			this.ClickCurrentPathSession = clickPath;

			var rootNode = new TreeNode
			{
				Text = GetFileTypeText(),
				Value = string.Empty,
				ImageUrl = DIRECTORY_ICON,
				IsDir = true,
				IsSelected = string.IsNullOrEmpty(clickPath),
			};

			CreateChildNode(openDirPathList, rootNode, clickPath, 0);

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
		/// <param name="roopCount">再起呼び出しカウント</param>
		private void CreateChildNode(string[] openDirPathList, TreeNode parent, string clickPath, int roopCount)
		{
			// 物理ディレクトリパス作成
			var targetRealPath = Path.Combine(
				Constants.PHYSICALDIRPATH_FRONT_PC,
				this.PathRootFrontSession.Substring(Constants.PATH_ROOT_FRONT_PC.Length).Replace("/", @"\"),
				GetFileTypeText() + @"\");
			targetRealPath = Path.Combine(targetRealPath + parent.Value);
			if (roopCount == 0) this.SelectPathSession = Path.Combine(targetRealPath, clickPath);

			// 各ディレクトリ作成
			var dirs = Directory.GetDirectories(targetRealPath);
			parent.Childs.AddRange(
				dirs.Select(
					dir =>
					{
						{
							// 親フォルダのパス文字列を切り捨てることでファイル名のみを取得
							var path = dir.Substring(targetRealPath.Length);
							var treeNode = new TreeNode
							{
								Text = path,
								Value = parent.Value + path + @"\",
								ImageUrl = DIRECTORY_ICON,
								IsDir = true,
								IsSelected = (parent.Value + path + @"\" == clickPath),
							};

							if (openDirPathList.Contains(treeNode.Value))
							{
								treeNode.IsOpen = true;
								CreateChildNode(openDirPathList, treeNode, clickPath, (roopCount + 1));
							}
							return treeNode;
						}
					}));

			// 各ファイル作成
			var files = TreeViewHelper.GetTreeViewFiles(targetRealPath);
			var errorMessage = TreeViewHelper.GetTreeViewErrorMessage(files);
			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				parent.ErrorMessage = errorMessage;
				parent.IsOpen = false;
				return;
			}

			parent.Childs.AddRange(
				files.Select(
					file =>
					{
						{
							var path = file.Substring(targetRealPath.Length);

							var treeNode = new TreeNode
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
									treeNode.ImageUrl = ASPX_ICON;
									break;

								case ".html":
								case ".htm":
									treeNode.ImageUrl = HTML_ICON;
									break;

								case ".jpg":
								case ".jpeg":
								case ".gif":
								case ".ico":
								case ".png":
								case ".bmp":
									treeNode.ImageUrl = IMAGE_ICON;
									break;

								default:
									treeNode.ImageUrl = OTHER_ICON;
									break;
							}
							return treeNode;
						}
					}));
		}

		/// <summary>
		/// クリック
		/// </summary>
		/// <param name="openDirPathList">展開中のディレクトリパスリスト</param>
		/// <param name="clickPath">クリックしたパス</param>
		/// <param name="clickDir">ディレクトリ展開か</param>
		/// <returns>ツリー</returns>
		internal IEnumerable<TreeNode> Click(string[] openDirPathList, string clickPath, bool clickDir)
		{
			var temp = openDirPathList.ToList();
			if (clickDir)
			{
				if (temp.Contains(clickPath))
				{
					temp.Remove(clickPath);
				}
				else
				{
					temp.Add(clickPath);
				}
			}

			var treeView = CreateTreeView(temp.ToArray(), clickPath);
			return treeView;
		}

		/// <summary>
		/// フォルダ作成
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		internal string MakeDirectory()
		{
			var parentPath = this.SelectPathSession;
			// ファイルを選択中の場合
			if (parentPath.EndsWith(@"\") == false) parentPath = Path.GetDirectoryName(this.SelectPathSession);
			var newDirectoryFilePathBase = Path.Combine(
				parentPath,
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
				var targetRealPath = Path.Combine(
					Constants.PHYSICALDIRPATH_FRONT_PC,
					this.PathRootFrontSession.Substring(Constants.PATH_ROOT_FRONT_PC.Length).Replace("/", @"\"),
					GetFileTypeText() + @"\");
				// 作成後ディレクトリを選択する
				this.ClickCurrentPathSession = newDirectoryFilePath.Substring((targetRealPath).Length) + "\\";
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
				return WebMessages.ContentsManagerFileOperationError;
			}

			return string.Empty;
		}

		/// <summary>
		/// 新しいファイルを生成
		/// </summary>
		/// <param name="isNew">isNew=Trueの場合は、ファイルを作成</param>
		internal string CreateFile(bool isNew)
		{
			// 作成ファイルパス取得（重複時は、連番付与する）
			var newFile = CreateFileName();

			// ファイル作成
			try
			{
				if (isNew)
				{
					// ファイル作成
					File.Create(newFile).Close();
				}
				else
				{
					File.Copy(this.SelectPathSession, newFile);
				}

				var targetRealPath = Path.Combine(
					Constants.PHYSICALDIRPATH_FRONT_PC,
					this.PathRootFrontSession.Substring(Constants.PATH_ROOT_FRONT_PC.Length).Replace("/", @"\"),
					GetFileTypeText() + @"\");
				// 作成後ディレクトリを選択する
				this.ClickCurrentPathSession = newFile.Substring((targetRealPath).Length);
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
				return WebMessages.ContentsManagerFileOperationError;
			}

			return string.Empty;
		}

		/// <summary>
		/// ファイルの新しい名前を生成
		/// </summary>
		/// <returns>新しいファイルの物理パス</returns>
		private string CreateFileName()
		{
			var parentPath = this.SelectPathSession;
			// ファイルを選択中の場合
			if (parentPath.EndsWith(@"\") == false) parentPath = Path.GetDirectoryName(this.SelectPathSession) + @"\";

			var fileNumber = 1;

			var newFile = "";
			switch (this.FileType)
			{
				case FileTypes.Css:
					newFile = parentPath + CONST_NEW_CSS_FILE_BASENAME + CONST_NEW_CSS_FILE_EXTENSION;
					break;

				case FileTypes.JavaScript:
					newFile = parentPath + CONST_NEW_JAVASCRIPT_FILE_BASENAME + CONST_NEW_JAVASCRIPT_FILE_EXTENSION;
					break;
			}

			while (File.Exists(newFile))
			{
				switch (this.FileType)
				{
					case FileTypes.Css:
						newFile = parentPath + CONST_NEW_CSS_FILE_BASENAME + fileNumber + CONST_NEW_CSS_FILE_EXTENSION;
						break;

					case FileTypes.JavaScript:
						newFile = parentPath + CONST_NEW_JAVASCRIPT_FILE_BASENAME + fileNumber + CONST_NEW_JAVASCRIPT_FILE_EXTENSION;
						break;
				}
				fileNumber++;
			}

			return newFile;
		}

		/// <summary>
		/// ファイルタイプのテキストを取得
		/// </summary>
		/// <returns></returns>
		private string GetFileTypeText()
		{
			var fileTypeText = "";
			switch (this.FileType)
			{
				case FileTypes.Css:
					fileTypeText = "Css";
					break;

				case FileTypes.JavaScript:
					fileTypeText = "Js";
					break;
			}

			return fileTypeText;
		}

		/// <summary>
		/// 削除
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		internal string Delete()
		{
			var success = false;
			var deleteFilePath = this.SelectPathSession;

			try
			{
				// ディレクトリ削除
				if (deleteFilePath.EndsWith(@"\"))
				{
					if (this.ClickCurrentPathSession == string.Empty)
					{
						return WebMessages.ContentsManagerDeleteError;
					}

					if (Directory.Exists(deleteFilePath) == false) return string.Empty;
					Directory.Delete(deleteFilePath, true);

					success = true;
				}
				// ファイル削除
				else
				{
					if (File.Exists(deleteFilePath))
					{
						// 書き込み専用でなければ削除
						if (((File.GetAttributes(deleteFilePath) & FileAttributes.ReadOnly) != FileAttributes.ReadOnly))
						{
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
			var reg = (this.ClickCurrentPathSession.EndsWith(@"\")) ? new Regex(@"([^\\]+?)?\\$") : new Regex(@"([^\\]+?)?$");
			this.SelectPathSession = this.SelectPathSession.Replace(this.ClickCurrentPathSession, string.Empty);
			this.SelectPathSession = "";
			this.ClickCurrentPathSession = reg.Replace(this.ClickCurrentPathSession, "");

			return string.Empty;
		}

		/// <summary>
		/// リネーム
		/// </summary>
		/// <param name="rename">変更後のファイル/ディレクトリ名</param>
		/// <returns>エラーメッセージ</returns>
		internal string Rename(string rename)
		{
			var errorMessage = Rename(this.ClickCurrentPathSession, this.SelectPathSession, rename);
			return errorMessage;
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
		/// 対象パス更新処理
		/// </summary>
		/// <returns>表示パス</returns>
		public string UpdateTargetPath()
		{
			return "(ROOT)/" + this.SelectPathSession.Replace(Constants.PHYSICALDIRPATH_FRONT_PC, "").Replace(@"\", "/");
		}

		/// <summary>
		/// ファイルテキスト取得処理
		/// </summary>
		/// <returns>ファイルかどうか</returns>
		public string GetFileText()
		{
			if (File.Exists(this.SelectPathSession)
				&& (File.GetAttributes(this.SelectPathSession).HasFlag(FileAttributes.Directory) == false)
				&& (this.SelectPathSession != "")
				&& (File.ReadAllText(this.SelectPathSession).Contains("\0") == false))
			{
				var text = DesignCommon.GetFileTextAll(this.SelectPathSession);
				UpdateFileOpenTime(this.SelectPathSession);
				return text;
			}
			return "NG";
		}

		/// <summary>
		/// ファイル更新
		/// </summary>
		/// <param name="text">テキスト</param>
		/// <returns>エラーテキスト</returns>
		public string UpdateFile(string text)
		{
			var errorText = DesignUtility.UpdateFile(this.SelectPathSession, text, true);
			if (errorText.IsNullOrEmpty()) UpdateFileOpenTime(this.SelectPathSession);
			return errorText;
		}

		/// <summary>
		/// パスルート変更処理
		/// </summary>
		/// <param name="path">パス</param>
		/// <returns>空文字</returns>
		internal object ChangeDirectory(string path)
		{
			this.PathRootFrontSession = path;
			return "";
		}

		#region プロパティ
		/// <summary>現在選択中のディレクトリ</summary>
		protected abstract string ClickCurrentPathSession { get; set; }
		/// <summary>現在選択中のパス</summary>
		protected abstract string SelectPathSession { get; set; }
		/// <summary>現在のフロントアプリケーションのパスルート</summary>
		protected abstract string PathRootFrontSession { get; set; }
		/// <summary>ファイルタイプ</summary>
		private FileTypes FileType { get; set; }
		#endregion
	}
}