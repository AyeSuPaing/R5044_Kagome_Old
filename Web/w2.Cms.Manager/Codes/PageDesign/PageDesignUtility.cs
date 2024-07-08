/*
=========================================================================================================
  Module      : ページ管理ユーティリティ(PageDesignUtility.cs)
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
using System.Web;
using w2.App.Common.Design;
using w2.App.Common.RefreshFileManager;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.Input;
using w2.Common.Util;
using w2.Common.Util.Archiver;
using w2.Common.Web;
using w2.Domain.Coordinate;
using w2.Domain.FeaturePage;
using w2.Domain.PageDesign;
using w2.Domain.PartsDesign;
using w2.Domain.Product;
using w2.Domain.RealShop;

namespace w2.Cms.Manager.Codes.PageDesign
{
	/// <summary>
	/// ページ管理ユーティリティ
	/// </summary>
	public class PageDesignUtility
	{
		#region 定義値
		/// <summary>プレビューファイル拡張子</summary>
		public const string PREVIEW_PAGE_FILE_EXTENSION = ".aspx.Preview.aspx";
		/// <summary>プレビューディレクトリパス</summary>
		public const string PREVIEW_DIR_PATH = @"Page\Preview\";
		/// <summary>編集可能領域定義（開始タグ）</summary>
		private static string[] TAG_EDIT_TABLE_TAG_BGN = { "<%-- ▽編集可能領域：", "▽ --%>" };
		/// <summary>編集可能領域定義（終了タグ）</summary>
		private const string TAG_EDIT_TABLE_TAG_END = "<%-- △編集可能領域△ --%>";
		/// <summary>レイアウト名 開始タグ</summary>
		private const string TAG_FILEINFO_LAYOUT_NAME_BGN = "<%@ FileInfo LayoutName=\"";
		/// <summary>レイアウト名 終了タグ</summary>
		private const string TAG_FILEINFO_LAYOUT_NAME_END = "\" %>";
		/// <summary>マスターファイル指定 開始位置</summary>
		private const string MASTER_FILE_NAME_SET_BGN = "/Form/Common/";
		/// <summary>マスターファイル指定 終了位置</summary>
		private const string MASTER_FILE_NAME_SET_END = ".master";
		/// <summary>カスタムページ採番 最大値</summary>
		private const int MAX_CUSTOM_PAGE_NUMBER = 200;
		/// <summary>ユーザーコントロール宣言領域（開始タグ）</summary>
		private const string USER_CONTROL_DECLARATION_BGN = "<%-- ▽ユーザーコントロール宣言領域▽ --%>";
		/// <summary>ユーザーコントロール宣言領域（終了タグ）</summary>
		private const string USER_CONTROL_DECLARATION_END = "<%-- △ユーザーコントロール宣言領域△ --%>";
		/// <summary>レイアウト領域定義（開始タグ）</summary>
		private static readonly string[] LAYOUT_TAG_BGNS = { "<%-- ▽レイアウト領域：", "▽ --%>" };
		/// <summary>レイアウト領域定義（終了タグ）</summary>
		private const string LAYOUT_TAG_END = "<%-- △レイアウト領域△ --%>";
		/// <summary>レイアウト領域名:トップエリア</summary>
		public const string LAYOUT_AREA_NAME_TOP = "トップエリア";
		/// <summary>レイアウト領域名:ライトエリア</summary>
		public const string LAYOUT_AREA_NAME_RIGHT = "ライトエリア";
		/// <summary>レイアウト領域名:ボトムエリア</summary>
		public const string LAYOUT_AREA_NAME_BOTTOM = "ボトムエリア";
		/// <summary>レイアウト領域名:レフトエリア</summary>
		public const string LAYOUT_AREA_NAME_LEFT = "レフトエリア";
		/// <summary>新規追加ファイル 初期ページID</summary>
		public const int NEW_PAGE_DEFAULT_PAGE_ID = -1;
		/// <summary>ファイル拡張子</summary>
		public const string PAGE_FILE_EXTENSION = ".aspx";
		/// <summary>カスタムページディレクトリパス</summary>
		public const string CUSTOM_PAGE_DIR_PATH = @"Page\";
		/// <summary>特集ページディレクトリパス</summary>
		public const string FEATURE_PAGE_DIR_PATH = @"Page\Feature\";
		/// <summary>最終更新者 開始タグ</summary>
		private const string TAG_FILEINFO_LASTCHANGED_BGN = "<%@ FileInfo LastChanged=\"";
		/// <summary>最終更新者 終了タグ</summary>
		private const string TAG_FILEINFO_LASTCHANGED_END = "\" %>";
		#endregion

		#region ファイル生成・更新
		/// <summary>
		/// 新規ファイルのモデル生成
		/// </summary>
		/// <returns>新規ファイルページモデル</returns>
		public static PageDesignModel CreateNewFileModel()
		{
			var newFileName = GetNewFileName();

			var defaultCustomPageTemplatePath = Constants.PAGE_FRONT_CUSTOMPAGE_TEMPLATE.Replace("/", @"\");
			var managementTitle = DesignCommon.GetAspxTitle(
				DesignCommon.GetFileTextAll(Path.Combine(DesignCommon.PhysicalDirPathTargetSitePc, defaultCustomPageTemplatePath)));

			var pageModel = new PageDesignModel
			{
				PageId = NEW_PAGE_DEFAULT_PAGE_ID,
				FileName = newFileName,
				FileDirPath = DesignCommon.CUSTOM_PAGE_DIR_PATH,
				Publish = Constants.FLG_PAGEDESIGN_PUBLISH_PRIVATE,
				ManagementTitle = managementTitle
			};
			return pageModel;
		}

		/// <summary>
		/// 新規ファイル生成
		/// </summary>
		/// <param name="errorMessage">エラーメッセージ</param>
		/// <param name="newFileName">新規ファイル名</param>
		/// <param name="sourceFileName">コピー元ファイル名</param>
		/// <returns>新規ファイルページモデル</returns>
		public static PageDesignModel CreateNewFile(out string errorMessage, string newFileName, string sourceFileName = "")
		{
			var pageId = 0;
			var defaultCustomPageTemplatePath = Constants.PAGE_FRONT_CUSTOMPAGE_TEMPLATE.Replace("/", @"\");

			var sourcePathPc = (string.IsNullOrEmpty(sourceFileName) == false)
					? Path.Combine(
						DesignCommon.PhysicalDirPathTargetSitePc, 
						DesignCommon.CUSTOM_PAGE_DIR_PATH, 
						sourceFileName)
					: string.Empty;

			var copyPathPc = ((string.IsNullOrEmpty(sourcePathPc) == false) && File.Exists(sourcePathPc))
				? sourcePathPc
				: Path.Combine(DesignCommon.PhysicalDirPathTargetSitePc, defaultCustomPageTemplatePath);

			DesignCommon.FileCopy(
				copyPathPc,
				Path.Combine(DesignCommon.PhysicalDirPathTargetSitePc, DesignCommon.CUSTOM_PAGE_DIR_PATH, newFileName));

			// レスポンシブ対応の場合はPCのみ生成
			if (DesignCommon.UseResponsive == false)
			{
				var sourcePathSp = (string.IsNullOrEmpty(sourceFileName) == false)
					? Path.Combine(
						DesignCommon.PhysicalDirPathTargetSiteSp,
						DesignCommon.CUSTOM_PAGE_DIR_PATH,
						sourceFileName)
					: string.Empty;

				var copyPathSp = ((string.IsNullOrEmpty(sourcePathSp) == false) && File.Exists(sourcePathSp))
					? sourcePathSp
					: Path.Combine(DesignCommon.PhysicalDirPathTargetSiteSp, defaultCustomPageTemplatePath);

				DesignCommon.FileCopy(
					copyPathSp,
					Path.Combine(DesignCommon.PhysicalDirPathTargetSiteSp, DesignCommon.CUSTOM_PAGE_DIR_PATH, newFileName));
			}

			var managementTitle = DesignCommon.GetAspxTitle(
				DesignCommon.GetFileTextAll(Path.Combine(DesignCommon.PhysicalDirPathTargetSitePc, defaultCustomPageTemplatePath)));

			var insertModel = new PageDesignModel
			{
				FileName = newFileName,
				FileDirPath = DesignCommon.CUSTOM_PAGE_DIR_PATH,
				Publish = Constants.FLG_PAGEDESIGN_PUBLISH_PRIVATE,
				ManagementTitle = managementTitle
			};
			var pageDesignService = new PageDesignService();

			var pageModel = pageDesignService.GetPageByFileName(newFileName);
			var error = string.Empty;
			if (pageModel == null)
			{
				try
				{
					pageId = pageDesignService.InsertPage(insertModel);
				}
				catch (Exception ex)
				{
					error = WebMessages.DataBaseError;
				}

				// フロントのキャッシュ更新
				RefreshFileManagerProvider.GetInstance(RefreshFileType.PageDesign).CreateUpdateRefreshFile();

				pageModel = pageDesignService.GetPage(pageId);
			}
			errorMessage = error;

			return pageModel;
		}

		/// <summary>
		/// ページ削除
		/// </summary>
		/// <param name="pageId">削除対象のページID</param>
		/// <returns>エラーメッセージ</returns>
		public static string DeletePage(long pageId)
		{
			var pageDesignService = new PageDesignService();
			var model = pageDesignService.GetPage(pageId);

			var errorMessage = string.Empty;

			// 各ページ削除
			var pcPath = Path.Combine(
				DesignCommon.PhysicalDirPathTargetSitePc,
				model.FileDirPath,
				model.FileName);
			if (File.Exists(pcPath))
			{
				errorMessage += DesignUtility.DeleteFile(pcPath);
			}

			var spPath = Path.Combine(
				DesignCommon.PhysicalDirPathTargetSiteSp,
				model.FileDirPath,
				model.FileName);
			if (File.Exists(spPath))
			{
				errorMessage += DesignUtility.DeleteFile(spPath);
			}

			if (string.IsNullOrEmpty(errorMessage))
			{
				// テーブル削除
				pageDesignService.DeletePage(pageId);
			}

			return errorMessage;
		}
		#endregion

		#region ファイル内 コンテンツ取得
		/// <summary>
		/// 新規ファイル名の取得
		/// </summary>
		/// <returns>新規ファイル名</returns>
		private static string GetNewFileName()
		{
			for (var fileNumber = 1; fileNumber < MAX_CUSTOM_PAGE_NUMBER; fileNumber++)
			{
				var fileName = "Page" + fileNumber.ToString("000") + DesignCommon.PAGE_FILE_EXTENSION;
				if (File.Exists(DesignCommon.PhysicalDirPathTargetSitePc + DesignCommon.CUSTOM_PAGE_DIR_PATH + fileName)
					|| File.Exists(DesignCommon.PhysicalDirPathTargetSiteSp + DesignCommon.CUSTOM_PAGE_DIR_PATH + fileName)) continue;
				return fileName;
			}
			return string.Empty;
		}

		/// <summary>
		/// レイアウト名の取得
		/// </summary>
		/// <param name="fileTextAll">テキスト全文</param>
		/// <returns>レイアウト名</returns>
		public static string GetLayoutName(string fileTextAll)
		{
			var layoutName = DesignCommon.GetFileInfo(fileTextAll, TAG_FILEINFO_LAYOUT_NAME_BGN, TAG_FILEINFO_LAYOUT_NAME_END);
			return layoutName;
		}

		/// <summary>
		/// マスターファイル名の取得
		/// </summary>
		/// <param name="fileTextAll">テキスト全文</param>
		/// <returns>マスターファイル名</returns>
		public static string GetMasterFileName(string fileTextAll)
		{
			var masterFileName = DesignCommon.GetFileInfo(fileTextAll, MASTER_FILE_NAME_SET_BGN, MASTER_FILE_NAME_SET_END);
			return masterFileName;
		}

		/// <summary>
		/// ページ管理に必要なパーツがあるかチェックする
		/// </summary>
		/// <param name="pcFilePath">ファイルパス（PC）</param>
		/// <param name="spFilePath">ファイルパス（SP）</param>
		/// <param name="pcNoUse">PCサイトを使用しないかどうか</param>
		/// <param name="spNoUse">SPサイトを使用しないかどうか</param>
		/// <returns>チェック結果</returns>
		public static string CheckTags(string pcFilePath, string spFilePath, bool pcNoUse, bool spNoUse)
		{
			var errorMessage = "";
			var pcFileTextAll = DesignCommon.GetFileTextAll(pcFilePath);
			var spFileTextAll = DesignCommon.GetFileTextAll(spFilePath);

			if ((pcNoUse == false))
			{
				errorMessage += CheckHaveLayoutAreaTags(pcFileTextAll, true);
				errorMessage += CheckHaveEditableAreaTag(pcFileTextAll, true);
			}

			if ((spNoUse == false))
			{
				errorMessage += CheckHaveLayoutAreaTags(spFileTextAll, false);
				errorMessage += CheckHaveEditableAreaTag(spFileTextAll, false);
			}

			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				errorMessage += WebMessages.PageDesignFileEditRangeSettingError;
			}
			return errorMessage;
		}

		/// <summary>
		/// レイアウト領域タグ、ユーザーコントロール領域タグががあるか確認
		/// </summary>
		/// <param name="fileTextAll">テキスト全文</param>
		/// <param name="isPc">PCか</param>
		/// <returns>エラーメッセージ内容</returns>
		private static string CheckHaveLayoutAreaTags(string fileTextAll, bool isPc)
		{
			var errorMessage = string.Empty;

			var checkNeedUserControlDeclation = (Regex.Matches(
				fileTextAll,
				USER_CONTROL_DECLARATION_BGN + ".*?" + USER_CONTROL_DECLARATION_END,
				RegexOptions.Singleline).Count == 1);

			var headList = new List<string>();
			foreach (Match match in Regex.Matches(
				fileTextAll,
				LAYOUT_TAG_BGNS[0] + ".*?" + LAYOUT_TAG_BGNS[1] + "\r\n" + ".*?" + LAYOUT_TAG_END,
				RegexOptions.Singleline))
			{
				headList.Add(match.Value
					.Substring(0, match.Value.IndexOf("\r\n", StringComparison.Ordinal))
					.Replace(LAYOUT_TAG_BGNS[0], "")
					.Replace(LAYOUT_TAG_BGNS[1], "")
				);
			}

			var checkNeedLayoutArea = isPc
				? (headList.Contains(LAYOUT_AREA_NAME_TOP)
					&& headList.Contains(LAYOUT_AREA_NAME_LEFT)
					&& headList.Contains(LAYOUT_AREA_NAME_RIGHT)
					&& headList.Contains(LAYOUT_AREA_NAME_BOTTOM))
				: (headList.Contains(LAYOUT_AREA_NAME_TOP)
					&& headList.Contains(LAYOUT_AREA_NAME_BOTTOM));

			var duplicateLayoutAreaList = headList
				.GroupBy(layoutAreaName => layoutAreaName)
				.Where(layoutAreaName => layoutAreaName.Count() > 1)
				.Select(group => group.Key).ToList();

			var pageName = ValueText.GetValueText(
				Constants.VALUE_TEXT_KEY_CMS_COMMON,
				Constants.VALUE_TEXT_FIELD_PAGE_DESIGN,
				(isPc) ? "PC" : "SP");
			var editAreaName = ValueText.GetValueText(
				Constants.VALUE_TEXT_KEY_CMS_COMMON,
				Constants.VALUE_TEXT_FIELD_PAGE_DESIGN,
				"EditLayout");

			if ((checkNeedUserControlDeclation == false) || (checkNeedLayoutArea == false))
			{
				errorMessage += WebMessages.PageDesignTagCheckError
					.Replace("@@ 1 @@", pageName)
					.Replace("@@ 2 @@",editAreaName);
			}

			if (duplicateLayoutAreaList.Any())
			{
				errorMessage += WebMessages.PageDesignTagCheckDuplicateError
					.Replace("@@ 1 @@", pageName)
					.Replace("@@ 2 @@", editAreaName)
					.Replace("@@ 3 @@", string.Join(", ", duplicateLayoutAreaList));
			}
			return errorMessage;
		}

		/// <summary>
		/// 編集可能レイアウトタグがあるか確認
		/// </summary>
		/// <param name="fileTextAll">テキスト全文</param>
		/// <param name="isPc">PCか</param>
		/// <returns>エラーメッセージ内容</returns>
		private static string CheckHaveEditableAreaTag(string fileTextAll, bool isPc)
		{
			var errorMessage = string.Empty;

			var headList =
				(from Match match in Regex.Matches(
						fileTextAll,
						TAG_EDIT_TABLE_TAG_BGN[0] + ".*?" + TAG_EDIT_TABLE_TAG_BGN[1] + "\r\n" + ".*?"
						+ TAG_EDIT_TABLE_TAG_END,
						RegexOptions.Singleline)
					select match.Value.Substring(0, match.Value.IndexOf("\r\n", StringComparison.Ordinal))
						.Replace(TAG_EDIT_TABLE_TAG_BGN[0], "").Replace(TAG_EDIT_TABLE_TAG_BGN[1], "")).ToList();

			var duplicateEditAreaList = headList
				.GroupBy(editArea => editArea)
				.Where(editArea => editArea.Count() > 1)
				.Select(group => group.Key).ToList();

			var pageName = ValueText.GetValueText(
				Constants.VALUE_TEXT_KEY_CMS_COMMON,
				Constants.VALUE_TEXT_FIELD_PAGE_DESIGN,
				(isPc) ? "PC" : "SP");
			var editAreaName = ValueText.GetValueText(
				Constants.VALUE_TEXT_KEY_CMS_COMMON,
				Constants.VALUE_TEXT_FIELD_PAGE_DESIGN,
				"EditContent");

			if (headList.Any() == false)
			{
				errorMessage += WebMessages.PageDesignTagCheckError
					.Replace("@@ 1 @@", pageName)
					.Replace("@@ 2 @@", editAreaName);
			}

			if (duplicateEditAreaList.Any())
			{
				errorMessage += WebMessages.PageDesignTagCheckDuplicateError
					.Replace("@@ 1 @@", pageName)
					.Replace("@@ 2 @@", editAreaName)
					.Replace("@@ 3 @@", string.Join(", ", duplicateEditAreaList));
			}
			return errorMessage;
		}

		/// <summary>
		/// 編集可能エリア取得
		/// </summary>
		/// <param name="fileTextAll">テキスト全文</param>
		/// <returns>編集可能エリア key=> エリア名 value=> 内容</returns>
		public static Dictionary<string, string> GetEditableAreaForEdit(string fileTextAll)
		{
			// 編集領域可能エリアの取得
			var editableText = GetAreas(
				fileTextAll,
				TAG_EDIT_TABLE_TAG_BGN,
				TAG_EDIT_TABLE_TAG_END);

			return editableText;
		}

		/// <summary>
		/// 編集可能レイアウト領域取得
		/// </summary>
		/// <param name="fileTextAll">テキスト全文</param>
		/// <returns>編集可能レイアウト領域 key=> 領域名 value=> 内容</returns>
		public static Dictionary<string, string> GetLayoutAreaForEdit(string fileTextAll)
		{
			// ユーザーコントロール宣言領域の取得
			if (string.IsNullOrEmpty(
				GetArea(
				fileTextAll,
				USER_CONTROL_DECLARATION_BGN,
				USER_CONTROL_DECLARATION_END)))return new Dictionary<string, string>();

			// 編集領域可能エリアの取得
			var editableLayout = GetAreas(
				fileTextAll,
				LAYOUT_TAG_BGNS,
				LAYOUT_TAG_END);

			return editableLayout;
		}

		/// <summary>
		/// 各種エリア取得
		/// </summary>
		/// <param name="fileTextAll">テキスト全文</param>
		/// <param name="tagBgns">開始位置</param>
		/// <param name="tagEnd">終了位置</param>
		/// <returns>編集可能エリア key=> エリア名 value=> 内容</returns>
		private static Dictionary<string, string> GetAreas(string fileTextAll, string[] tagBgns, string tagEnd)
		{
			var editableText = new Dictionary<string, string>();
			foreach (Match match in Regex.Matches(
				fileTextAll,
				tagBgns[0] + ".*?" + tagBgns[1] + "\r\n" + ".*?" + tagEnd,
				RegexOptions.Singleline))
			{
				var strHeader = match.Value.Substring(0, match.Value.IndexOf("\r\n", StringComparison.Ordinal));

				var key = strHeader.Replace(tagBgns[0], "").Replace(tagBgns[1], "");

				if (editableText.ContainsKey(key) == false)
				{
					editableText.Add(
						key,
						new StringBuilder(
							match.Value.Replace(strHeader + "\r\n", "").Replace("\r\n" + tagEnd, "")
								.Replace(tagEnd, "")).ToString()); // 中身が空の時とそうでない時とでEndタグは２度置換
				}
			}
			return editableText;
		}

		/// <summary>
		/// 対象エリア取得
		/// </summary>
		/// <param name="fileTextAll">テキスト全文</param>
		/// <param name="tagBgn">開始位置</param>
		/// <param name="tagEnd">終了位置</param>
		/// <returns>編集可能エリア key=> エリア名 value=> 内容</returns>
		private static string GetArea(string fileTextAll, string tagBgn, string tagEnd)
		{
			foreach (Match match in Regex.Matches(fileTextAll, tagBgn + ".*?" + tagEnd, RegexOptions.Singleline))
			{
				var header = match.Value.Substring(0, match.Value.IndexOf("\r\n", StringComparison.Ordinal));

				return match.Value.Replace(header + "\r\n", "").Replace(tagEnd, "");
			}
			return "";
		}
		#endregion

		#region ファイル内 コンテンツ置換
		/// <summary>
		/// 更新用コンテンツタグ置換
		/// </summary>
		/// <param name="fileTextAll">置換対象テキスト全文</param>
		/// <param name="contents">編集可能エリア key=> エリア名 value=> 内容</param>
		public static void ReplaceContentsTagForUpdate(StringBuilder fileTextAll, Dictionary<string, string> contents)
		{
			if (contents == null) return;

			foreach (var content in contents)
			{
				// タグの内部取得
				var newText = TAG_EDIT_TABLE_TAG_BGN[0] + content.Key + TAG_EDIT_TABLE_TAG_BGN[1] + "\r\n" + content.Value
					+ "\r\n" + TAG_EDIT_TABLE_TAG_END;
				fileTextAll.Replace(
					Regex.Match(
						fileTextAll.ToString(),
						TAG_EDIT_TABLE_TAG_BGN[0] + content.Key + TAG_EDIT_TABLE_TAG_BGN[1] + ".*?" + TAG_EDIT_TABLE_TAG_END,
						RegexOptions.Singleline | RegexOptions.IgnoreCase).Value,
					newText);
			}
		}

		/// <summary>
		/// レイアウト名の置換
		/// </summary>
		/// <param name="fileTextAll">置換対象テキスト全文</param>
		/// <param name="layoutType">レイアウト名</param>
		public static void ReplaceLayoutName(StringBuilder fileTextAll, string layoutType)
		{
			foreach (Match mPageTag in Regex.Matches(
				fileTextAll.ToString(),
				TAG_FILEINFO_LAYOUT_NAME_BGN + ".*?" + TAG_FILEINFO_LAYOUT_NAME_END))
			{
				fileTextAll.Replace(
					mPageTag.Value,
					TAG_FILEINFO_LAYOUT_NAME_BGN + HtmlSanitizer.HtmlEncode(layoutType) + TAG_FILEINFO_LAYOUT_NAME_END);
				break;
			}
		}

		/// <summary>
		/// 更新用レイアウト置換
		/// </summary>
		/// <param name="fileTextAll">対象テキスト内容</param>
		/// <param name="allRealParts">全ての実パーツ</param>
		/// <param name="deviceType">デバイスタイプ</param>
		/// <param name="layoutEditInput">コンテンツ内容</param>
		/// <returns>置換したパーツ数</returns>
		public static int ReplaceLayoutForUpdate(
			StringBuilder fileTextAll,
			List<RealParts> allRealParts,
			DesignCommon.DeviceType deviceType,
			LayoutEditInput layoutEditInput)
		{
			var userControlDeclarationsBefore = GetArea(
				fileTextAll.ToString(),
				USER_CONTROL_DECLARATION_BGN,
				USER_CONTROL_DECLARATION_END);

			var layoutText = GetAreas(fileTextAll.ToString(), LAYOUT_TAG_BGNS, LAYOUT_TAG_END);
			var replacedPartsCount = 0;

			foreach (var kvpLayout in layoutText)
			{
				var movePartsModels = new MovePartsModel[]{};
				switch (kvpLayout.Key)
				{
					case LAYOUT_AREA_NAME_TOP:
						movePartsModels = layoutEditInput.MovePartsCenterTop;
						break;

					case LAYOUT_AREA_NAME_RIGHT:
						movePartsModels = layoutEditInput.MovePartsRight;
						break;

					case LAYOUT_AREA_NAME_BOTTOM:
						movePartsModels = layoutEditInput.MovePartsCenterBottom;
						break;

					case LAYOUT_AREA_NAME_LEFT:
						movePartsModels = layoutEditInput.MovePartsLeft;
						break;
				}

				var partsTags = movePartsModels
					.SelectMany(model => model.RealParts.PartsTag)
					.Select(model => model.Value);
				var layoutInner = string.Join("\r\n", partsTags);
				// レイアウトリプレース
				fileTextAll.Replace(
					LAYOUT_TAG_BGNS[0] + kvpLayout.Key + LAYOUT_TAG_BGNS[1] + "\r\n"
					+ kvpLayout.Value + ((kvpLayout.Value.Replace("\t", "").Length != 0) ? "\r\n" : "")
					+ LAYOUT_TAG_END,
					LAYOUT_TAG_BGNS[0] + kvpLayout.Key + LAYOUT_TAG_BGNS[1] + "\r\n"
					+ layoutInner + ((string.IsNullOrEmpty(layoutInner) == false) ? "\r\n" : "")
					+ LAYOUT_TAG_END);

				// 置換したパーツをカウントする
				replacedPartsCount += movePartsModels.Length;
			}

			// ユーザーコントロール宣言リプレース
			fileTextAll.Replace(
				USER_CONTROL_DECLARATION_BGN + "\r\n"
				+ userControlDeclarationsBefore
				+ (((userControlDeclarationsBefore != "") && (userControlDeclarationsBefore.Contains("\r\n") == false))
					? "\r\n"
					: "")
				+ USER_CONTROL_DECLARATION_END,
				USER_CONTROL_DECLARATION_BGN + "\r\n" 
				+ GetUserControlDeclarations(fileTextAll, allRealParts, deviceType, userControlDeclarationsBefore)
				+ USER_CONTROL_DECLARATION_END);

			return replacedPartsCount;
		}

		/// <summary>
		/// コンテンツ内容にパーツ配置内容をセット
		/// </summary>
		/// <param name="fileTextAll">対象テキスト内容</param>
		/// <param name="allPartsDesignModels">全パーツモデル</param>
		/// <param name="allRealParts">全ての実パーツ</param>
		/// <param name="input">コンテンツ内容</param>
		public static void SetContentInputLayoutParts(
			string fileTextAll,
			PartsDesignModel[] allPartsDesignModels,
			List<RealParts> allRealParts,
			LayoutEditInput input)
		{
			var layoutText = GetAreas(fileTextAll, LAYOUT_TAG_BGNS, LAYOUT_TAG_END);
			foreach (var kvpLayout in layoutText)
			{
				// 複数行にわたるコメントアウト排除
				var commentEscaped = Regex.Replace(kvpLayout.Value, "<%--.*?--%>", string.Empty, RegexOptions.Singleline);

				var partsNames = Regex.Matches(commentEscaped, "<.*?>").Cast<Match>()
					.Where(m => allRealParts.Any(p => p.PartsTag.Any(pt => pt.Value == m.Value)))
					.Select(
					m =>
					{
						var realParts = allRealParts.FirstOrDefault(
							arp => arp.PartsTag.Any(pt => pt.Value == m.Value));
						var partsDesignModel =
							allPartsDesignModels.FirstOrDefault(model =>
								(realParts != null) && (model.FileName == realParts.FileName));

						var result = new MovePartsModel()
						{
							RealParts = realParts,
							PartsDesignModel = partsDesignModel
						};
						return result;
					}).ToArray();

				switch (kvpLayout.Key)
				{
					case LAYOUT_AREA_NAME_TOP:
						input.MovePartsCenterTop = partsNames;
						break;

					case LAYOUT_AREA_NAME_RIGHT:
						input.MovePartsRight = partsNames;
						break;

					case LAYOUT_AREA_NAME_BOTTOM:
						input.MovePartsCenterBottom = partsNames;
						break;

					case LAYOUT_AREA_NAME_LEFT:
						input.MovePartsLeft = partsNames;
						break;
				}
			}
		}

		/// <summary>
		/// 対象テキスト内のユーザコントロール宣言を取得
		/// </summary>
		/// <param name="fileTextAll">対象テキスト内容</param>
		/// <param name="allRealParts">全ての実パーツ</param>
		/// <param name="deviceType">デバイスタイプ</param>
		/// <param name="currentUserControlDefinition">ユーザコントロール領域内容</param>
		private static string GetUserControlDeclarations(
			StringBuilder fileTextAll,
			List<RealParts> allRealParts,
			DesignCommon.DeviceType deviceType,
			string currentUserControlDefinition)
		{
			var result = new StringBuilder();

			// 対象テキスト内のユーザコントロール宣言を除外した内容
			var userControlDefinitionExtendFileText = fileTextAll.ToString().Replace(
				(USER_CONTROL_DECLARATION_BGN + "\r\n"
				+ currentUserControlDefinition
				+ ((currentUserControlDefinition.EndsWith("\r\n") == false)
					? "\r\n"
					: string.Empty)
				+ USER_CONTROL_DECLARATION_END),
				string.Empty);

			foreach (var parts in allRealParts)
			{
				foreach (var tag in parts.PartsTag)
				{
					if (fileTextAll.ToString().Contains(tag.Value) == false) continue;

					var partsDeclaration = parts.Declaration;
					var tagName = GetUserControlDefinitionTagName(partsDeclaration);

					// 既にページ内に宣言エリア外で宣言が存在する場合はスキップ
					// ※ 宣言エリア外にある場合はエリア外を優先
					if (userControlDefinitionExtendFileText.Contains(tagName)) continue;

					var currentPart = GetCurrentUserControlDefinition(currentUserControlDefinition, tagName);
					if ((string.IsNullOrEmpty(currentPart) == false)
						&& (result.ToString().Contains(currentPart) == false))
					{
						result.Append(currentPart).Append("\r\n");
					}
					else if (result.ToString().Contains(partsDeclaration) == false)
					{
						// ユーザコントロール宣言追加
						result.Append(partsDeclaration.Replace("~/", "~/" + ((deviceType == DesignCommon.DeviceType.Sp) ? DesignCommon.SiteSpRootPath : ""))).Append("\r\n");
					}
				}
			}
			return result.ToString();
		}

		/// <summary>
		/// ユーザコントロール領域内から対象パーツタグ宣言を取得
		/// </summary>
		/// <param name="currentUserControlDefinition">ユーザコントロール領域内容</param>
		/// <param name="tagName">TagName要素</param>
		/// <returns>パーツタグ宣言</returns>
		private static string GetCurrentUserControlDefinition(string currentUserControlDefinition, string tagName)
		{
			var result = string.IsNullOrEmpty(tagName) 
				? string.Empty 
				: currentUserControlDefinition.Split(new []{ "\r\n", "\n" }, StringSplitOptions.None)
					.FirstOrDefault(x => x.Contains(tagName));

			return result;
		}

		/// <summary>
		/// パーツタグ宣言からTagName要素を取得
		/// </summary>
		/// <param name="partsDeclaration">パーツタグ宣言</param>
		/// <returns>TagName要素</returns>
		private static string GetUserControlDefinitionTagName(string partsDeclaration)
		{
			var tagName = "";
			foreach (Match match in Regex.Matches(partsDeclaration, "TagName=\".*?\"", RegexOptions.Singleline))
			{
				tagName = match.Value;
				break;
			}
			return tagName;
		}

		/// <summary>
		/// デフォルトのマスターページを利用しているか判定
		/// </summary>
		/// <param name="fileTextAll">テキスト全文</param>
		/// <returns>結果</returns>
		public static bool CheckUseDefultMasterFile(string fileTextAll)
		{
			var result = Regex.Matches(fileTextAll, MASTER_FILE_NAME_SET_BGN + ".*?" + MASTER_FILE_NAME_SET_END)
				.Cast<Match>()
				.Any(
					tag => (tag.Value.Contains(Constants.PAGE_FRONT_DEFAULT_MASTER))
						|| (tag.Value.Contains(Constants.PAGE_FRONT_SIMPLE_DEFAULT_MASTER)));

			return result;
		}

		/// <summary>
		/// マスターファイル更新
		/// </summary>
		/// <param name="fileTextAll">置換対象テキスト全文</param>
		/// <param name="masterFileNameWithOutExtension">マスターファイル名(拡張子なし)</param>
		public static void ReplaceMasterFile(StringBuilder fileTextAll, string masterFileNameWithOutExtension)
		{
			foreach (Match mPageTag in Regex.Matches(
				fileTextAll.ToString(),
				MASTER_FILE_NAME_SET_BGN + ".*?" + MASTER_FILE_NAME_SET_END))
			{
				fileTextAll.Replace(
					mPageTag.Value,
					MASTER_FILE_NAME_SET_BGN + HtmlSanitizer.HtmlEncode(masterFileNameWithOutExtension) + MASTER_FILE_NAME_SET_END);
				break;
			}
		}
		#endregion

		#region プレビュー
		/// <summary>
		/// プレビューファイルの全削除
		/// </summary>
		public static void DeletePreviewFile()
		{
			DeletePreviewFile(DesignCommon.DeviceType.Pc);
			DeletePreviewFile(DesignCommon.DeviceType.Sp);
		}
		/// <summary>
		/// プレビューファイルの全削除(デバイス指定)
		/// </summary>
		/// <param name="deviceType">デバイスタイプ</param>
		private static void DeletePreviewFile(DesignCommon.DeviceType deviceType)
		{
			var previewDirPath = Path.Combine(GetPhysicalDirPathTargetSite(deviceType), PREVIEW_DIR_PATH);
			if (Directory.Exists(previewDirPath) == false)
			{
				Directory.CreateDirectory(previewDirPath);
			}

			var previewFileRemoveAfterDay = -3;

			foreach (var previewFile in Directory.GetFiles(
				previewDirPath,
				"*.Preview.*",
				SearchOption.AllDirectories))
			{
				var file = new FileInfo(previewFile);
				if (file.CreationTime < DateTime.Now.AddDays(previewFileRemoveAfterDay))
				{
					File.Delete(previewFile);
				}
			}
		}
		#endregion

		#region WebURL
		/// <summary>
		/// WebURLの取得
		/// </summary>
		/// <param name="dirPath">ディレクトリパス</param>
		/// <param name="fileName">ファイル名</param>
		/// <param name="usePreviewUrl">設定ファイルにあるプレビュー用URLを利用するか？(ELB対応)</param>
		/// <returns>WebURL</returns>
		public static string WebTargetPageUrl(string dirPath, string fileName, bool usePreviewUrl = false)
		{
			// ページURL作成
			var targetPageUrl = new StringBuilder();
			if ((string.IsNullOrEmpty(Constants.PREVIEW_URL) == false) && usePreviewUrl)
			{
				targetPageUrl.Append(Constants.PREVIEW_URL);
			}
			else if (Constants.PATH_ROOT_FRONT_PC.StartsWith("http"))
			{
				targetPageUrl.Append(Constants.PATH_ROOT_FRONT_PC);
			}
			else
			{
				targetPageUrl.Append(Constants.PROTOCOL_HTTPS).Append(Constants.SITE_DOMAIN).Append(Constants.PATH_ROOT_FRONT_PC);
			}

			targetPageUrl.Append(dirPath.Replace(@"\", "/"));

			targetPageUrl.Append(fileName);

			// 商品詳細であればパラメタ付加
			if (targetPageUrl.ToString().Contains("ProductDetail.aspx"))
			{
				var model = new ProductService().GetProductTopForPreview(Constants.CONST_DEFAULT_SHOP_ID);

				if (model != null)
				{
					var uc = new UrlCreator(targetPageUrl.ToString());
					uc.AddParam(
						Constants.REQUEST_KEY_FRONT_SHOP_ID,
						HttpUtility.UrlEncode(Constants.CONST_DEFAULT_SHOP_ID));

					uc.AddParam(
						Constants.REQUEST_KEY_FRONT_PRODUCT_ID,
						HttpUtility.UrlEncode(model.ProductId));

					targetPageUrl = new StringBuilder(uc.CreateUrl());
				}
			}

			// コーディネート詳細であればパラメタ付加
			if (targetPageUrl.ToString().Contains("CoordinateDetail.aspx"))
			{
				var model = new CoordinateService().GetCoordinateTopForPreview();

				if (model != null)
				{
					targetPageUrl = new StringBuilder(new UrlCreator(targetPageUrl.ToString())
							.AddParam(Constants.REQUEST_KEY_COORDINATE_ID, HttpUtility.UrlEncode(model.CoordinateId))
							.CreateUrl());
				}
			}

			if (targetPageUrl.ToString().ToLower().Contains("BodyRealShopProductStockList.ascx".ToLower()) || targetPageUrl.ToString().ToLower().Contains("100RSPSL_".ToLower()))
			{
				var realShopProductStockModel = new RealShopService().GetRealShopProductStockTopForPreview(Constants.CONST_DEFAULT_SHOP_ID);
				if (realShopProductStockModel != null)
				{
					var uc = new UrlCreator(targetPageUrl.ToString());
					uc.AddParam(Constants.REQUEST_KEY_FRONT_PRODUCT_ID, realShopProductStockModel.ProductId);
					uc.AddParam(Constants.REQUEST_KEY_FRONT_VARIATION_ID, realShopProductStockModel.VariationId);
					targetPageUrl = new StringBuilder(uc.CreateUrl());
				}
			}

			return targetPageUrl.ToString();
		}
		#endregion

		#region ファイルパス
		/// <summary>
		/// 物理パスの取得
		/// </summary>
		/// <param name="deviceType">デバイスタイプ</param>
		/// <returns>物理パス</returns>
		public static string GetPhysicalDirPathTargetSite(DesignCommon.DeviceType deviceType)
		{
			var physicalDirPathTargetSite = (deviceType == DesignCommon.DeviceType.Pc) ? DesignCommon.PhysicalDirPathTargetSitePc : DesignCommon.PhysicalDirPathTargetSiteSp;
			return physicalDirPathTargetSite;
		}

		/// <summary>
		///  全ページ・パーツファイルパス取得（標準・カスタム）
		/// </summary>
		/// <param name="deviceType">デバイスタイプ</param>
		/// <returns>全ページ・パーツファイルパス取得</returns>
		private static List<string> GetAllPagePartsList(DesignCommon.DeviceType deviceType)
		{
			var physicalDirPathTargetSite = GetPhysicalDirPathTargetSite(deviceType);

			// ページ・パーツリスト取得（標準・カスタム）
			var allPagePartsList = new List<List<string>>
			{
				PageDesignCommon.GetStandardPageSetting(DesignCommon.DeviceType.Pc).Select(p => Path.Combine(p.PageDirPath, p.FileName)).ToList(),
				PageDesignCommon.GetCustomPageList(DesignCommon.DeviceType.Pc).Select(p => Path.Combine(p.PageDirPath, p.FileName)).ToList(),
				PartsDesignCommon.GetStandardPartsList(DesignCommon.DeviceType.Pc)
					.Select(p => Path.Combine(p.PageDirPath, p.FileName)).ToList(),
				PartsDesignCommon.GetCustomPartsList(DesignCommon.DeviceType.Pc)
					.Select(p => Path.Combine(p.PageDirPath, p.FileName)).ToList()
			};

			var result = new List<string>();
			// 全ページファイルパス取得
			foreach (var pagePartsList in allPagePartsList)
			{
				foreach (var pageParts in pagePartsList)
				{
					var filePath = Path.Combine(physicalDirPathTargetSite, pageParts);
					if (File.Exists(filePath))
					{
						result.Add(filePath);
					}
				}
			}

			// Cssファイルパス取得(編集サイト毎のCssPath)
			foreach (string filePath in GetFilePathList(physicalDirPathTargetSite + @"Css\"))
			{
				// .sccファイルとthumbs.dbファイルは追加しない
				if ((Path.GetExtension(filePath) == ".scc")
					|| (Path.GetFileName(filePath) == "thumbs.db"))
				{
					continue;
				}

				result.Add(filePath);
			}

			// JavaScriptファイルパス取得(編集サイト毎のJavaScriptPath)
			result.AddRange(GetFilePathList(physicalDirPathTargetSite + @"Js\"));

			return result;
		}

		/// <summary>
		/// ファイルパス一覧取得（再帰）
		/// </summary>
		/// <param name="dirPath">ディレクトリパス</param>
		/// <returns>ディレクトリ配下のファイルパス一覧</returns>
		private static List<string> GetFilePathList(string dirPath)
		{
			var result = Directory.GetFiles(dirPath).ToList();
			result.AddRange(Directory.GetDirectories(dirPath).SelectMany(GetFilePathList));

			return result;
		}
		#endregion

		#region ページ・パーツダウンロード
		/// <summary>
		/// ページ・パーツダウンロード 全対象
		/// </summary>
		/// <param name="response">HTTPレスポンス</param>
		public static void Download(HttpResponseBase response)
		{
			var pcPagePartsList = GetAllPagePartsList(DesignCommon.DeviceType.Pc).ToArray();
			var spPagePartsList = (DesignCommon.UseSmartPhone)
				? GetAllPagePartsList(DesignCommon.DeviceType.Sp).Select(p => Path.Combine(DesignCommon.SiteSpRootPath, p)).ToArray()
				: new string[]{};

			Download(
				response,
				pcPagePartsList.Concat(spPagePartsList).ToArray(),
				GetPhysicalDirPathTargetSite(DesignCommon.DeviceType.Pc),
				"Design_" + DateTime.Now.ToString("yyyyMMdd") + ".zip");
		}
		/// <summary>
		/// ページ・パーツダウンロード デバイス毎
		/// </summary>
		/// <param name="response">HTTPレスポンス</param>
		/// <param name="deviceType">デバイスタイプ</param>
		public static void Download(HttpResponseBase response, DesignCommon.DeviceType deviceType)
		{
			Download(
				response,
				GetAllPagePartsList(deviceType).ToArray(),
				GetPhysicalDirPathTargetSite(deviceType),
				"Design_" + ((deviceType == DesignCommon.DeviceType.Pc) ? "Pc_" : "Sp_") + DateTime.Now.ToString("yyyyMMdd") + ".zip");
		}
		/// <summary>
		/// ページ・パーツダウンロード
		/// </summary>
		/// <param name="response">HTTPレスポンス</param>
		/// <param name="targetPaths">ダウンロード対象パスリスト</param>
		/// <param name="targetRootPath">ルートパス</param>
		/// <param name="zipFileName">Zipファイルネーム</param>
		private static void Download(HttpResponseBase response, string[] targetPaths, string targetRootPath, string zipFileName)
		{
			response.ContentType = "application/zip";
			response.AppendHeader("Content-Disposition", "attachment; filename=" + Path.GetFileNameWithoutExtension(zipFileName) + ".zip");

			new ZipArchiver().CompressToStream(
				targetPaths,
				targetRootPath,
				response.OutputStream);

			response.End();
		}
		#endregion

		#region 特集ページ
		/// <summary>
		/// 特集ページリスト取得
		/// </summary>
		/// <param name="deviceType">デバイスタイプ</param>
		/// <param name="targetPage">ターゲットファイル条件</param>
		/// <returns>特集ページリスト</returns>
		public static List<RealPage> GetFeaturePageList(
			DesignCommon.DeviceType deviceType,
			string targetPage = "*" + PAGE_FILE_EXTENSION)
		{
			var physicalDirPathTargetSite = GetPhysicalDirPathTargetSite(deviceType);

			// 特集ページ読み取り
			var list = new List<RealPage>();
			if (string.IsNullOrEmpty(targetPage)) return list;

			foreach (var strFilePath in Directory.GetFiles(physicalDirPathTargetSite + FEATURE_PAGE_DIR_PATH, targetPage))
			{
				var fileName = Path.GetFileName(strFilePath);
				var filePath = strFilePath
					.Replace(physicalDirPathTargetSite, string.Empty)
					.Replace(fileName, string.Empty);

				list.Add(
					new RealPage(
						StringUtility.ToEmpty(DesignCommon.GetAspxTitle(strFilePath)),
						physicalDirPathTargetSite,
						filePath,
						fileName,
						string.Empty));
			}

			return list;
		}

		/// <summary>
		/// 特集ページ削除
		/// </summary>
		/// <param name="pageId">特集ページID</param>
		/// <returns>エラーメッセージ</returns>
		public static string DeleteFeaturePage(long pageId)
		{
			var service = new FeaturePageService();
			var model = service.Get(pageId);
			var errorMessage = string.Empty;

			// 各ページ削除
			var pcPath = Path.Combine(
				DesignCommon.PhysicalDirPathTargetSitePc,
				model.FileDirPath,
				model.FileName);
			if (File.Exists(pcPath))
			{
				errorMessage += DesignUtility.DeleteFile(pcPath);
			}

			var spPath = Path.Combine(
				DesignCommon.PhysicalDirPathTargetSiteSp,
				model.FileDirPath,
				model.FileName);
			if (File.Exists(spPath))
			{
				errorMessage += DesignUtility.DeleteFile(spPath);
			}

			if (string.IsNullOrEmpty(errorMessage))
			{
				service.Delete(pageId);
				// フロントのキャッシュ更新
				RefreshFileManagerProvider.GetInstance(RefreshFileType.FeaturePage).CreateUpdateRefreshFile();
			}

			return errorMessage;
		}

		/// <summary>
		/// メタタグ取得
		/// </summary>
		/// <param name="fileTextAll">テキスト全文</param>
		/// <param name="name">name属性名</param>
		/// <returns>content属性値</returns>
		public static string GetMetaTag(string fileTextAll, string name)
		{
			foreach (Match match in Regex.Matches(fileTextAll, "<meta .*?/>"))
			{
				if (!match.Value.Contains(name)) continue;
				foreach (Match content in Regex.Matches(match.Value, @"(?<=(content="")).+?(?=(""))"))
				{
					return content.Value;
				}
			}
			return string.Empty;
		}

		/// <summary>
		/// メタタグ置換
		/// </summary>
		/// <param name="fileTextAll">テキスト全文</param>
		/// <param name="name">name属性名</param>
		/// <param name="content">content属性値</param>
		public static void ReplaceMetaTag(
			StringBuilder fileTextAll,
			string name,
			string content)
		{
			foreach (Match metaTag in Regex.Matches(fileTextAll.ToString(), "<meta .*? />"))
			{
				var pattern = string.Format("name=\"{0}\" content=\".*?\"", name);
				foreach (Match mName in Regex.Matches(metaTag.Value, pattern))
				{
					var replacedMetaTag = mName.Value.Replace(
						mName.Value,
						string.Format(
							"name=\"{0}\" content=\"{1}\"",
							name,
							HtmlSanitizer.HtmlEncode(content)));
					fileTextAll.Replace(mName.Value, replacedMetaTag);
					break;
				}
			}
		}

		/// <summary>
		/// 特集ページコピー新規
		/// </summary>
		/// <param name="sourcePageId">コピー元特集ページID</param>
		/// <returns>新規特集ページID</returns>
		public static long CopyFeatureFile(long sourcePageId)
		{
			var service = new FeaturePageService();
			var sourcePageModel = service.Get(sourcePageId);
			var newFileName = GetNewFeatureFileName();
			var insertPageModel = sourcePageModel.Clone();
			insertPageModel.FileName = newFileName;
			insertPageModel.ManagementTitle = string.Format(
				"{0}" + Constants.COPY_NEW_SUFFIX,
				insertPageModel.ManagementTitle);

			var newPageId = service.Insert(insertPageModel);
			var newModel = service.Get(newPageId);

			// 特集ページコンテンツも複製
			newModel.Contents = sourcePageModel.Contents
				.Select(
					content =>
					{
						var newContents = content.Clone();
						newContents.FeaturePageId = newPageId;
						return newContents;
					})
				.ToArray();

			service.Update(newModel);

			var dirPathSitePc = Path.Combine(
				DesignCommon.PhysicalDirPathTargetSitePc,
				sourcePageModel.FileDirPath,
				sourcePageModel.FileName);
			var newDirPathSitePc = Path.Combine(
				DesignCommon.PhysicalDirPathTargetSitePc,
				sourcePageModel.FileDirPath,
				newFileName);
			DesignCommon.FileCopy(dirPathSitePc, newDirPathSitePc);

			// レスポンシブ対応の場合はPCのみ生成
			if (DesignCommon.UseResponsive == false)
			{
				var dirPathSiteSp = Path.Combine(
					DesignCommon.PhysicalDirPathTargetSiteSp,
					sourcePageModel.FileDirPath,
					sourcePageModel.FileName);
				var newDirPathSiteSp = Path.Combine(
					DesignCommon.PhysicalDirPathTargetSiteSp,
					sourcePageModel.FileDirPath,
					newFileName);
				DesignCommon.FileCopy(dirPathSiteSp, newDirPathSiteSp);
			}

			return newModel.FeaturePageId;
		}

		/// <summary>
		/// 特集ページ新規ファイル名の取得
		/// </summary>
		/// <returns>新規ファイル名</returns>
		public static string GetNewFeatureFileName()
		{
			var fileNumber = 1;

			while (true)
			{
				var fileName = "FeaturePage" + fileNumber.ToString("000") + PAGE_FILE_EXTENSION;
				var dirPathSitePc = Path.Combine(
					DesignCommon.PhysicalDirPathTargetSitePc,
					FEATURE_PAGE_DIR_PATH,
					fileName);
				var dirPathSiteSp = Path.Combine(
					DesignCommon.PhysicalDirPathTargetSiteSp,
					FEATURE_PAGE_DIR_PATH,
					fileName);

				if (File.Exists(dirPathSitePc)
					|| File.Exists(dirPathSiteSp)
					|| (CheckFeaturePageFileName(fileName) == false))
				{
					fileNumber++;
					continue;
				}

				return fileName;
			}
		}

		/// <summary>
		/// 新規特集ファイル生成
		/// </summary>
		/// <returns>新規特集ページモデル</returns>
		public static FeaturePageModel CreateNewFeatureFile()
		{
			var defaultFeaturePageTemplatePath = Constants.PAGE_FRONT_FEATUREPAGE_TEMPLATE.Replace("/", @"\");
			var newFileName = GetNewFeatureFileName();
			var dirPathSitePc = Path.Combine(
				DesignCommon.PhysicalDirPathTargetSitePc,
				defaultFeaturePageTemplatePath);
			var newDirPathSitePc = Path.Combine(
				DesignCommon.PhysicalDirPathTargetSitePc,
				FEATURE_PAGE_DIR_PATH,
				newFileName);
			DesignCommon.FileCopy(dirPathSitePc, newDirPathSitePc);
			File.SetLastWriteTime(newDirPathSitePc, DateTime.Now);

			// レスポンシブ対応の場合はPCのみ生成
			if (DesignCommon.UseResponsive == false)
			{
				var dirPathSiteSp = Path.Combine(
					DesignCommon.PhysicalDirPathTargetSiteSp,
					defaultFeaturePageTemplatePath);
				var newDirPathSiteSp = Path.Combine(
					DesignCommon.PhysicalDirPathTargetSiteSp,
					FEATURE_PAGE_DIR_PATH,
					newFileName);
				DesignCommon.FileCopy(dirPathSiteSp, newDirPathSiteSp);
				File.SetLastWriteTime(newDirPathSiteSp, DateTime.Now);
			}

			var managementTitle = DesignCommon.GetAspxTitle(Path.Combine(
				DesignCommon.PhysicalDirPathTargetSitePc,
				defaultFeaturePageTemplatePath));

			var insertModel = new FeaturePageModel
			{
				FileName = newFileName,
				FileDirPath = FEATURE_PAGE_DIR_PATH,
				Publish = Constants.FLG_PAGEDESIGN_PUBLISH_PRIVATE,
				ManagementTitle = managementTitle,
				CategoryId = string.Empty,
				PermittedBrandIds = string.Empty
			};
			var service = new FeaturePageService();
			var pageId = service.Insert(insertModel);
			var pageModel = service.Get(pageId);
			return pageModel;
		}

		/// <summary>
		/// 特集ページのプレビューファイルの全削除
		/// </summary>
		public static void DeleteFeaturePreview()
		{
			DeleteFeaturePreview(DesignCommon.DeviceType.Pc);
			DeleteFeaturePreview(DesignCommon.DeviceType.Sp);
		}
		/// <summary>
		/// 特集ページのプレビューファイルの全削除(デバイス指定)
		/// </summary>
		/// <param name="deviceType">デバイスタイプ</param>
		private static void DeleteFeaturePreview(DesignCommon.DeviceType deviceType)
		{
			var physicalDirPathTargetSite = GetPhysicalDirPathTargetSite(deviceType);
			var previewFiles = Directory.GetFiles(
				Path.Combine(physicalDirPathTargetSite, FEATURE_PAGE_DIR_PATH),
				"*.Preview.*",
				SearchOption.AllDirectories);

			foreach (var previewFile in previewFiles)
			{
				File.Delete(previewFile);
			}
		}

		/// <summary>
		/// ヘッダーバナー置換
		/// </summary>
		/// <param name="fileTextAll">テキスト全文</param>
		/// <param name="filePath">ファイルパス ※空の場合は画像は表示されません</param>
		public static void ReplaceHeaderBanner(StringBuilder fileTextAll, string filePath)
		{
			var divTags = Regex.Matches(
				fileTextAll.ToString(),
				"<div id=\"" + Constants.FEATUREPAGE_HEADER_BANNER + "\".*?</div>", RegexOptions.Singleline);
			foreach (Match divTag in divTags)
			{
				foreach (Match src in Regex.Matches(divTag.Value, "src=\".*?\""))
				{
					var replacedSrcAttribute = src.Value.Replace(
						src.Value,
						string.Format("src=\"{0}\"", filePath));

					fileTextAll.Replace(src.Value, replacedSrcAttribute);
				}
			}
		}

		/// <summary>
		/// ヘッダーバナー取得
		/// </summary>
		/// <param name="fileTextAll">テキスト全文</param>
		public static string GetHeaderBannerFileName(string fileTextAll)
		{
			var result = Path.GetFileName(DesignCommon.GetHeaderBannerSrc(Constants.FEATUREPAGE_HEADER_BANNER, fileTextAll));
			return result;
		}

		/// <summary>
		/// 元の表示処理を削除
		/// </summary>
		/// <param name="fileTextAll">テキスト全文</param>
		public static void DeleteRemoveJquery(StringBuilder fileTextAll)
		{
			var jqueryStatements = Regex.Matches(
				fileTextAll.ToString(),
				"<%-- ▽非表示処理▽ --%>.*?<%-- △非表示処理△ --%>",
				RegexOptions.Singleline);
			foreach (Match jquery in jqueryStatements)
			{
				fileTextAll.Replace(jquery.Value, "");
			}
		}

		/// <summary>
		/// 表示チェックボックスの値を取得
		/// </summary>
		/// <param name="fileTextAll">テキスト全文</param>
		/// <param name="id">divID</param>
		/// <returns>true:表示 false:非表示</returns>
		public static bool GetDispCheckBox(string fileTextAll, string id)
		{
			var jqueryStatements = Regex.Matches(
				fileTextAll,
				"<%-- ▽非表示処理▽ --%>.*?<%-- △非表示処理△ --%>",
				RegexOptions.Singleline);
			if(fileTextAll.Contains("$('#"+ id + "').remove();"))
			{
				foreach (Match jquery in jqueryStatements)
				{
					return jquery.Value.Contains(id) == false;
				}
			}

			var cssStatements = Regex.Matches(
				fileTextAll,
				"<%-- ▽編集可能領域：HEAD追加部分▽ --%>.*?<%-- △編集可能領域△ --%>",
				RegexOptions.Singleline);
			var cssCheckboxFlg = (cssStatements.Cast<Match>().FirstOrDefault()?.Value.Contains("<style>#" + id + " { display: none; }</style>") ?? true) == false;
			return cssCheckboxFlg;
		}

		/// <summary>
		/// Check feature page file name
		/// </summary>
		/// <param name="fileName">File name</param>
		/// <returns>True if file name not exist</returns>
		public static bool CheckFeaturePageFileName(string fileName)
		{
			var featurePage = new FeaturePageService().GetByFileName(fileName);
			return string.IsNullOrEmpty(featurePage.FileName);
		}
		#endregion

		#region プロパティ
		/// <summary>「その他」グループモデル</summary>
		public static PageDesignGroupModel OtherPageGroupModel
		{
			get
			{
				return new PageDesignGroupModel
				{
					GroupId = Constants.FLG_PAGEDESIGNGROUP_GROUP_ID_OTHER_ID,
					GroupName = ValueText.GetValueText(
						Constants.TABLE_PAGEDESIGNGROUP,
						Constants.FIELD_PAGEDESIGNGROUP_GROUP_NAME,
						Constants.FLG_PAGEDESIGNGROUP_GROUP_ID_OTHER_ID),
					GroupSortNumber = 9999999
				};
			}
		}
		#endregion
	}

	/// <summary>
	/// レイアウト配置パーツモデル
	/// </summary>
	public class MovePartsModel
	{
		/// <summary>実パーツ</summary>
		public RealParts RealParts { get; set; }
		/// <summary>パーツモデル</summary>
		public PartsDesignModel PartsDesignModel { get; set; }
		public bool IsSettingReleaseRange
		{
			get
			{
				return (this.PartsDesignModel.ConditionPublishDateFrom != null)
					|| (this.PartsDesignModel.ConditionPublishDateTo != null)
					|| (this.PartsDesignModel.ConditionMemberOnlyType != Constants.FLG_PARTSDESIGN_MEMBER_ONLY_TYPE_ALL)
					|| (string.IsNullOrEmpty(this.PartsDesignModel.ConditionTargetListIds) == false);
			}
		}
	}
}
