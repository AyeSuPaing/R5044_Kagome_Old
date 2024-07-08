/*
=========================================================================================================
  Module      : デザイン管理 共通処理 (DesignCommon.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using w2.App.Common.Util;
using w2.Common.Logger;
using w2.Common.Util;
using w2.Common.Web;

namespace w2.App.Common.Design
{
	/// <summary>
	/// デザイン管理 共通処理
	/// </summary>
	public class DesignCommon
	{
		/// <summary>デバイスタイプ</summary>
		public enum DeviceType
		{
			/// <summary>PCデバイス</summary>
			Pc,
			/// <summary>SPデバイス</summary>
			Sp
		}

		#region 定義値
		/// <summary>ファイル拡張子</summary>
		public const string PAGE_FILE_EXTENSION = ".aspx";
		/// <summary>HTMLファイル拡張子</summary>
		public const string HTML_FILE_EXTENSION = ".html";
		/// <summary>ファイル拡張子</summary>
		public const string PARTS_FILE_EXTENSION = ".ascx";
		/// <summary>カスタムページディレクトリパス</summary>
		public const string CUSTOM_PAGE_DIR_PATH = @"Page\";
		/// <summary>カスタムパーツディレクトリパス</summary>
		public const string CUSTOM_PARTS_DIR_PATH = @"Page\Parts\";
		/// <summary>最終更新者 開始タグ</summary>
		private const string TAG_FILEINFO_LASTCHANGED_BGN = "<%@ FileInfo LastChanged=\"";
		/// <summary>最終更新者 終了タグ</summary>
		private const string TAG_FILEINFO_LASTCHANGED_END = "\" %>";
		#endregion

		/// <summary>
		/// ファイルコピー 読み取り専用の場合は解除
		/// </summary>
		/// <param name="sourceFilePath">コピー元</param>
		/// <param name="outputFilePath">コピー先</param>
		public static void FileCopy(string sourceFilePath, string outputFilePath)
		{
			if (File.Exists(sourceFilePath) == false) return;
			File.Copy(sourceFilePath, outputFilePath);
			var attr = File.GetAttributes(outputFilePath);
			//ファイルが読み取り専用の場合は解除
			if ((attr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
			{
				attr = attr & ~FileAttributes.ReadOnly;
				File.SetAttributes(outputFilePath, attr);
			}
		}

		/// <summary>
		/// ファイル 物理削除
		/// </summary>
		/// <param name="filePath">ファイルパス</param>
		/// <returns>エラーメッセージ</returns>
		public static bool DeleteFile(string filePath)
		{
			try
			{
				File.Delete(filePath);
			}
			catch (Exception ex)
			{
				FileLogger.WriteError("ファイルが削除できませんでした:" + filePath, ex);
				return false;
			}

			return true;
		}

		/// <summary>
		/// ファイルのテキスト全文を取得
		/// </summary>
		/// <param name="filePath">ファイルパス</param>
		/// <param name="encodingParam">文字コード</param>
		/// <returns>テキスト全文</returns>
		public static string GetFileTextAll(string filePath, Encoding encodingParam = null)
		{
			if (File.Exists(filePath) == false) return string.Empty;

			var content = File.ReadAllBytes(filePath);

			var encoding = encodingParam ?? StringUtility.GetCode(content);
			using (var reader = new StreamReader(filePath, encoding))
			{
				var fileTextAll = reader.ReadToEnd();

				// 改行コードはWindown環境(CRLF)ではない場合、CRLFに変換する
				fileTextAll = fileTextAll.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");

				return fileTextAll;
			}
		}

		/// <summary>
		/// ASPXタイトル取得
		/// </summary>
		/// <param name="fileTextAll">テキスト全文</param>
		/// <returns>タイトル</returns>
		public static string GetAspxTitle(string fileTextAll)
		{
			foreach (Match match in Regex.Matches(fileTextAll, "<%@ (P|p)age " + ".*?" + "%>"))
			{
				if (match.Value.Contains(" Title=", true))
				{
					var bgn = match.Value.IndexOf(" Title=\"", StringComparison.OrdinalIgnoreCase) + " Title=\"".Length;

					return HttpUtility.HtmlDecode(
						match.Value.Substring(bgn, match.Value.IndexOf("\"", bgn, StringComparison.Ordinal) - bgn));
				}
			}

			return string.Empty;
		}


		/// <summary>
		/// ファイルタイトル置換
		/// </summary>
		/// <param name="fileTextAll">置換対象テキスト全文</param>
		/// <param name="title">タイトル</param>
		public static void ReplaceTitle(StringBuilder fileTextAll, string title)
		{
			foreach (Match matchPageTag in Regex.Matches(fileTextAll.ToString(), "<%@ (P|p)age " + ".*?" + "%>"))
			{
				string replacedPageTag = null;
				foreach (Match matchTitle in Regex.Matches(matchPageTag.Value, " (T|t)itle=" + "\".*?\" "))
				{
					replacedPageTag = matchPageTag.Value.Replace(
						matchTitle.Value,
						" Title=\"" + HtmlSanitizer.HtmlEncode(title) + "\" ");
					break;
				}

				if (replacedPageTag != null)
				{
					fileTextAll.Replace(matchPageTag.Value, replacedPageTag);
				}

				break;
			}
		}

		/// <summary>
		/// ファイル内情報の取得
		/// </summary>
		/// <param name="fileTextAll">テキスト全文</param>
		/// <param name="tagBgn">タグ開始位置</param>
		/// <param name="tagEnd">タグ終了位置</param>
		/// <returns>ファイル内情報</returns>
		public static string GetFileInfo(string fileTextAll, string tagBgn, string tagEnd)
		{
			foreach (Match mPageTag in Regex.Matches(fileTextAll, tagBgn + ".*?" + tagEnd))
			{
				return mPageTag.Value.Replace(tagBgn, "").Replace(tagEnd, "");
			}

			return "";
		}

		/// <summary>
		/// 最終更新者の取得
		/// </summary>
		/// <param name="fileTextAll">テキスト全文</param>
		/// <returns>最終更新者</returns>
		public static string GetLastChanged(string fileTextAll)
		{
			var lastChanged = GetFileInfo(fileTextAll, TAG_FILEINFO_LASTCHANGED_BGN, TAG_FILEINFO_LASTCHANGED_END);
			return lastChanged;
		}

		/// <summary>
		/// 最終更新者の置換
		/// </summary>
		/// <param name="fileTextAll">置換対象テキスト全文</param>
		/// <param name="changeOperator">オペレータ名</param>
		public static void ReplaceLastChanged(StringBuilder fileTextAll, string changeOperator)
		{
			foreach (Match mPageTag in Regex.Matches(
				fileTextAll.ToString(),
				TAG_FILEINFO_LASTCHANGED_BGN + ".*?" + TAG_FILEINFO_LASTCHANGED_END))
			{
				fileTextAll.Replace(
					mPageTag.Value,
					TAG_FILEINFO_LASTCHANGED_BGN + HtmlSanitizer.HtmlEncode(changeOperator)
					+ TAG_FILEINFO_LASTCHANGED_END);
				break;
			}
		}

		/// <summary>
		/// 物理パスの取得
		/// </summary>
		/// <param name="deviceType">デバイスタイプ</param>
		/// <returns>物理パス</returns>
		public static string GetPhysicalDirPathTargetSite(DeviceType deviceType)
		{
			var physicalDirPathTargetSite = (deviceType == DeviceType.Pc)
				? PhysicalDirPathTargetSitePc
				: PhysicalDirPathTargetSiteSp;
			return physicalDirPathTargetSite;
		}

		/// <summary>
		/// ヘッダーバナー取得
		/// </summary>
		/// <param name="targetHtmlId">ターゲットHTMLID</param>
		/// <param name="fileTextAll">テキスト全文</param>
		public static string GetHeaderBannerSrc(string targetHtmlId, string fileTextAll)
		{
			var divTags = Regex.Matches(
				fileTextAll,
				"<div id=\"" + targetHtmlId + "\".*?</div>", RegexOptions.Singleline);
			foreach (Match divTag in divTags)
			{
				foreach (Match src in Regex.Matches(divTag.Value, "src=\".*?\""))
				{
					foreach (Match content in Regex.Matches(src.Value, @"(?<=(src="")).+?(?=(""))"))
					{
						return content.Value;
					}
				}
			}

			return string.Empty;
		}

		/// <summary>SP利用可能か?</summary>
		public static bool UseSmartPhone
		{
			get { return Constants.SMARTPHONE_OPTION_ENABLED && (UseResponsive == false); }
		}
		/// <summary>レスポンシブ利用か？</summary>
		public static bool UseResponsive
		{
			get { return string.IsNullOrEmpty(SiteSpRootPath); }
		}
		/// <summary>対象PCサイト物理パス</summary>
		public static string PhysicalDirPathTargetSitePc
		{
			get
			{
				return Constants.PHYSICALDIRPATH_FRONT_PC.Substring(
						0,
						Constants.PHYSICALDIRPATH_FRONT_PC.Length - Constants.PATH_ROOT_FRONT_PC.Length)
					+ Constants.PATH_ROOT_FRONT_PC.Replace("/", @"\");
			}
		}
		/// <summary>対象SPサイト物理パス</summary>
		public static string PhysicalDirPathTargetSiteSp
		{
			get
			{
				return Constants.PHYSICALDIRPATH_FRONT_PC.Substring(
						0,
						Constants.PHYSICALDIRPATH_FRONT_PC.Length - Constants.PATH_ROOT_FRONT_PC.Length)
					+ Path.Combine(Constants.PATH_ROOT_FRONT_PC, SiteSpRootPath).Replace("/", @"\");
			}
		}
		/// <summary>SPルートパス</summary>
		public static string SiteSpRootPath
		{
			get
			{
				var spRoot = string.Empty;
				foreach (var psst in SmartPhoneUtility.SmartPhoneSiteSettings)
				{
					spRoot = psst.RootPath.Replace("~/", "");
				}

				return spRoot;
			}
		}
	}
}
