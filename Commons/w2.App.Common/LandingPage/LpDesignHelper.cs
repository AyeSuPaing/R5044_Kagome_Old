/*
=========================================================================================================
  Module      : LPページデザイン用のヘルパクラス(LpDesignHelper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using w2.App.Common.LandingPage.LandingPageDesignData;
using w2.App.Common.Util;
using w2.Domain.LandingPage;
using w2.Domain.OgpTagSetting;

namespace w2.App.Common.LandingPage
{
	/// <summary>
	/// LPページデザイン用のヘルパクラス
	/// </summary>
	public static class LpDesignHelper
	{
		/// <summary>カスタムデザイン:Metaタグ開始</summary>
		private const string CUSTOM_META_TAG_BGN = "<%-- ▽Meta Tag▽ --%>";
		/// <summary>カスタムデザイン:Metaタグ終了</summary>
		private const string CUSTOM_META_TAG_END = "<%-- △Meta Tag△ --%>";
		/// <summary>カスタムデザイン:システムコード開始</summary>
		private const string CUSTOM_SYSTEM_CODE_TAG_BGN = "// ▽System Code▽";
		/// <summary>カスタムデザイン:システムコード終了</summary>
		private const string CUSTOM_SYSTEM_CODE_TAG_END = "// △System Code△";

		/// <summary>
		/// デフォルトブロックデザインJSONファイルのパスを取得
		/// </summary>
		/// <returns>ファイルパス</returns>
		public static string GetDefaultBlockJsonFilePath()
		{
			return Constants.CMS_LANDING_PAGE_DEFAULT_BLOCK_DESIGN_FILE_PATH;
		}

		/// <summary>
		/// デフォルトブロックデザインのINPUTデータ取得
		/// </summary>
		/// <returns>JSON</returns>
		public static List<PageDesignInput> _GetDefaultBlockJsonData()
		{
			var j = GetDefaultBlockJsonString();
			return JsonConvert.DeserializeObject<List<PageDesignInput>>(j);
		}

		/// <summary>
		/// デフォルトブロックデザインのJSON取得
		/// </summary>
		/// <returns></returns>
		public static string GetDefaultBlockJsonString()
		{
			string j;
			using (var f = new StreamReader(GetDefaultBlockJsonFilePath()))
			{
				j = f.ReadToEnd();
			}

			// ルート部分のパスをきれいにする
			j = j.Replace("@@ rep_pc_site_root @@", Constants.PROTOCOL_HTTPS + Constants.SITE_DOMAIN + Constants.PATH_ROOT_FRONT_PC);

			return j;
		}

		/// <summary>
		/// PCサイト用のLPテンプレートファイルのパスを取得
		/// </summary>
		/// <param name="model">LPページモデル</param>
		/// <returns>ファイルパス</returns>
		public static string GetLpPageTemplateFilePathPc(LandingPageDesignModel model)
		{
			var templatePath = Constants.CMS_LANDING_PAGE_TEMPLATE_FILE_PATH_PC;
			switch (model.DesignMode)
			{
				case Constants.FLG_LANDINGPAGEDESIGN_DESIGN_MODE_DEFAULT:
					templatePath = Constants.CMS_LANDING_PAGE_TEMPLATE_FILE_PATH_PC;
					break;

				case Constants.FLG_LANDINGPAGEDESIGN_DESIGN_MODE_CUSTOM:
					templatePath = Constants.CMS_LANDING_PAGE_CUSTOM_TEMPLATE_FILE_PATH_PC;
					break;
			}

			return Path.Combine(Constants.PHYSICALDIRPATH_FRONT_PC, templatePath);
		}

		/// <summary>
		/// SPサイト用のLPテンプレートファイルのパスを取得
		/// </summary>
		/// <param name="model">LPページモデル</param>
		/// <returns>ファイルパス</returns>
		public static string GetLpPageTemplateFilePathSp(LandingPageDesignModel model)
		{
			var templatePath = Constants.CMS_LANDING_PAGE_TEMPLATE_FILE_PATH_SP;
			switch (model.DesignMode)
			{
				case Constants.FLG_LANDINGPAGEDESIGN_DESIGN_MODE_DEFAULT:
					templatePath = Constants.CMS_LANDING_PAGE_TEMPLATE_FILE_PATH_SP;
					break;

				case Constants.FLG_LANDINGPAGEDESIGN_DESIGN_MODE_CUSTOM:
					templatePath = Constants.CMS_LANDING_PAGE_CUSTOM_TEMPLATE_FILE_PATH_SP;
					break;
			}
			return Path.Combine(Constants.PHYSICALDIRPATH_FRONT_PC, templatePath);
		}

		/// <summary>
		/// PCサイト用のLPデザインファイルのパスを取得
		/// </summary>
		/// <param name="fileName">ファイル名</param>
		/// <returns>ファイルパス</returns>
		public static string GetLpPageFilePathPc(string fileName)
		{
			return GetLpPageFilePath(fileName, Constants.CMS_LANDING_PAGE_DIR_PATH_PC);
		}

		/// <summary>
		/// SPサイト用のLPデザインファイルのパスを取得
		/// </summary>
		/// <param name="fileName">ファイル名</param>
		/// <returns>ファイルパス</returns>
		public static string GetLpPageFilePathSp(string fileName)
		{
			return GetLpPageFilePath(fileName, Constants.CMS_LANDING_PAGE_DIR_PATH_SP);
		}

		/// <summary>
		/// LPデザインファイルのパスを取得
		/// </summary>
		/// <param name="fileName">ファイル名</param>
		/// <param name="dirPath">ディレクトリパス</param>
		/// <returns>ファイルパス</returns>
		public static string GetLpPageFilePath(string fileName, string dirPath)
		{
			return Path.Combine(Constants.PHYSICALDIRPATH_FRONT_PC, dirPath, string.Format("{0}.aspx", fileName));
		}

		/// <summary>
		/// PCサイトのLPページURLを取得
		/// </summary>
		/// <param name="fileName">ファイル名</param>
		/// <returns>URL</returns>
		public static string GetLpPageUrlPc(string fileName)
		{
			return Constants.PROTOCOL_HTTPS + Constants.SITE_DOMAIN + Constants.PATH_ROOT_FRONT_PC + Constants.CMS_LANDING_PAGE_DIR_URL_PC + string.Format("{0}.aspx", fileName);
		}

		/// <summary>
		/// SPサイトのLPページURLを取得
		/// </summary>
		/// <param name="fileName">ファイル名</param>
		/// <returns>URL</returns>
		public static string GetLpPageUrlSp(string fileName)
		{
			return Constants.PROTOCOL_HTTPS + Constants.SITE_DOMAIN + Constants.PATH_ROOT_FRONT_PC + Constants.CMS_LANDING_PAGE_DIR_URL_SP + string.Format("{0}.aspx", fileName);
		}

		/// <summary>
		/// PCサイト用のLPデザインファイル書き込み
		/// </summary>
		/// <param name="aftModel">更新後モデル</param>
		/// <param name="befModel">更新前モデル</param>
		/// <param name="copySourceModel">コピー元モデル</param>
		public static void WriteLpPageFilePc(
			LandingPageDesignModel aftModel,
			LandingPageDesignModel befModel = null,
			LandingPageDesignModel copySourceModel = null)
		{
			// PCの場合はテンプレート読込なければエラー
			if (File.Exists(GetLpPageTemplateFilePathPc(aftModel)) == false)
			{
				throw new Exception("LP用のテンプレートファイルが存在しないため、エラーが発生しました。");
			}

			// テンプレート読込
			string template;
			using (var sr = new StreamReader(GetLpPageTemplateFilePathPc(aftModel)))
			{
				template = sr.ReadToEnd();
			}

			var pagedata = GetLpPageData(
				aftModel,
				template,
				Constants.CMS_LANDING_PAGE_DIR_PATH_PC,
				befModel,
				copySourceModel);

			// ファイル出力（BOMつき）
			using (var sw = new StreamWriter(GetLpPageFilePathPc(aftModel.PageFileName), false, Encoding.UTF8))
			{
				sw.Write(pagedata);
			}
		}

		/// <summary>
		/// SPサイト用のLPデザインファイル書き込み
		/// </summary>
		/// <param name="aftModel">更新後モデル</param>
		/// <param name="befModel">更新前モデル</param>
		/// <param name="copySourceModel">コピー元モデル</param>
		public static void WriteLpPageFileSp(
			LandingPageDesignModel aftModel,
			LandingPageDesignModel befModel = null,
			LandingPageDesignModel copySourceModel = null)
		{
			// SP使わない場合はファイル作成しない
			if (Constants.SMARTPHONE_OPTION_ENABLED == false) return;
			if (SmartPhoneUtility.SmartPhoneSiteSettings.Any(setting => setting.SmartPhonePageEnabled == false)) return;

			// テンプレート読込
			string template;
			using (var sr = new StreamReader(GetLpPageTemplateFilePathSp(aftModel)))
			{
				template = sr.ReadToEnd();
			}

			var pagedata = GetLpPageData(
				aftModel,
				template,
				Constants.CMS_LANDING_PAGE_DIR_PATH_SP,
				befModel,
				copySourceModel);

			// ファイル出力（BOMつき）
			using (var sw = new StreamWriter(GetLpPageFilePathSp(aftModel.PageFileName), false, Encoding.UTF8))
			{
				sw.Write(pagedata);
			}
		}

		/// <summary>
		/// LP更新後のファイル内容を取得
		/// </summary>
		/// <param name="aftModel">更新後モデル</param>
		/// <param name="template">テンプレート内容</param>
		/// <param name="dirPath">ディレクトリパス</param>
		/// <param name="befModel">更新後モデル</param>
		/// <param name="copySourceModel">コピー元モデル</param>
		/// <returns></returns>
		public static string GetLpPageData(
			LandingPageDesignModel aftModel,
			string template,
			string dirPath,
			LandingPageDesignModel befModel = null,
			LandingPageDesignModel copySourceModel = null)
		{
			// テンプレート置換
			var pagedata = string.Empty;
			switch (aftModel.DesignMode)
			{
				case Constants.FLG_LANDINGPAGEDESIGN_DESIGN_MODE_DEFAULT:
					pagedata = ReplacePageDataTemlate(template, aftModel);
					break;

				case Constants.FLG_LANDINGPAGEDESIGN_DESIGN_MODE_CUSTOM:
					var filePath = (copySourceModel != null)
						? GetLpPageFilePath(copySourceModel.PageFileName, dirPath)
						: (befModel != null)
							? GetLpPageFilePath(befModel.PageFileName, dirPath)
							: GetLpPageFilePath(aftModel.PageFileName, dirPath);
					var originalContent = string.Empty;
					if (File.Exists(filePath))
					{
						using (var sr = new StreamReader(filePath))
						{
							originalContent = sr.ReadToEnd();
						}
					}
					pagedata = ReplacePageDataOriginal(template, originalContent, aftModel);
					break;
			}
			return pagedata;
		}

		/// <summary>
		/// w2標準デザイン ページデータを置換
		/// </summary>
		/// <param name="template">テンプレート</param>
		/// <param name="model">モデル</param>
		/// <returns>ページデータ</returns>
		public static string ReplacePageDataTemlate(
			string template,
			LandingPageDesignModel model)
		{
			var defaultOgpSetting = new OgpTagSettingService().Get(Constants.FLG_OGPTAGSETTING_DATA_KBN_DEFAULT_SETTING);
			defaultOgpSetting.SiteTitle = defaultOgpSetting.SiteTitle.Replace(Constants.SEOSETTING_KEY_SEO_TITLE, model.PageTitle)
				.Replace(Constants.SEOSETTING_KEY_SEO_DESCRIPTION, model.MetadataDesc);

			var imageFilePath = Path.Combine(
				Constants.PHYSICALDIRPATH_FRONT_PC,
				Constants.PATH_LANDINGPAGE,
				model.PageId,
				model.PageId + Constants.CONTENTS_IMAGE_FIRST);

			var imageUrl = (File.Exists(imageFilePath))
				? Constants.PROTOCOL_HTTPS
					+ Constants.SITE_DOMAIN 
					+ Constants.PATH_ROOT_FRONT_PC
					+ Path.Combine(Constants.PATH_LANDINGPAGE, model.PageId, model.PageId + Constants.CONTENTS_IMAGE_FIRST)
				: defaultOgpSetting.ImageUrl;
			var pagedata = template;
			pagedata = ReplaceMetaTag(
				pagedata,
				defaultOgpSetting.SiteTitle,
				model.PageTitle,
				GetLpPageUrlPc(model.PageFileName),
				model.MetadataDesc,
				imageUrl);
			pagedata = ReplaceSystemCode(pagedata, model.PageId);

			return pagedata;
		}

		/// <summary>
		/// カスタムデザイン ページデータを置換
		/// </summary>
		/// <param name="templateContent">テンプレート内容</param>
		/// <param name="sourceContent">更新元の内容</param>
		/// <param name="model">モデル</param>
		/// <returns>ページデータ</returns>
		public static string ReplacePageDataOriginal(
			string templateContent,
			string sourceContent,
			LandingPageDesignModel model)
		{
			var defaultOgpSetting = new OgpTagSettingService().Get(Constants.FLG_OGPTAGSETTING_DATA_KBN_DEFAULT_SETTING);
			defaultOgpSetting.SiteTitle = defaultOgpSetting.SiteTitle.Replace(Constants.SEOSETTING_KEY_SEO_TITLE, model.PageTitle)
				.Replace(Constants.SEOSETTING_KEY_SEO_DESCRIPTION, model.MetadataDesc);

			var imageFilePath = Path.Combine(
				Constants.PHYSICALDIRPATH_FRONT_PC,
				Constants.PATH_LANDINGPAGE,
				model.PageId,
				model.PageId + Constants.CONTENTS_IMAGE_FIRST);

			var imageUrl = (File.Exists(imageFilePath))
				? Constants.PROTOCOL_HTTPS
				+ Constants.SITE_DOMAIN
				+ Constants.PATH_ROOT_FRONT_PC
				+ Path.Combine(Constants.PATH_LANDINGPAGE, model.PageId, model.PageId + Constants.CONTENTS_IMAGE_FIRST)
				: defaultOgpSetting.ImageUrl;

				string fileContent;
				if (string.IsNullOrEmpty(sourceContent))
				{
					fileContent = templateContent;
					fileContent = ReplaceMetaTag(
						fileContent,
						defaultOgpSetting.SiteTitle,
						model.PageTitle,
						GetLpPageUrlPc(model.PageFileName),
						model.MetadataDesc,
						imageUrl);
					fileContent = ReplaceSystemCode(fileContent, model.PageId);
				}
				else
				{
					fileContent = sourceContent;
					// Metaタグ置換
					var metaContent = GetTagContent(templateContent, CUSTOM_META_TAG_BGN, CUSTOM_META_TAG_END);
					var newMetaTag = ReplaceMetaTag(
						metaContent,
						defaultOgpSetting.SiteTitle,
						model.PageTitle,
						GetLpPageUrlPc(model.PageFileName),
						model.MetadataDesc,
						imageUrl);

					newMetaTag = CUSTOM_META_TAG_BGN + "\r\n" + newMetaTag + CUSTOM_META_TAG_END;
					var oldMetaTagMatch = Regex.Match(
						fileContent,
						CUSTOM_META_TAG_BGN + ".*?" + CUSTOM_META_TAG_END,
						RegexOptions.Singleline | RegexOptions.IgnoreCase);
					if (oldMetaTagMatch.Length > 0)
					{
						fileContent = fileContent.Replace(oldMetaTagMatch.Value, newMetaTag);
					}

					// システムコード置換
					var systemCodeContent = GetTagContent(templateContent, CUSTOM_SYSTEM_CODE_TAG_BGN, CUSTOM_SYSTEM_CODE_TAG_END);
					var newSystemCode = ReplaceSystemCode(
						systemCodeContent,
						model.PageId);

					newSystemCode = CUSTOM_SYSTEM_CODE_TAG_BGN + "\r\n" + newSystemCode + CUSTOM_SYSTEM_CODE_TAG_END;
					var oldSystemCode = Regex.Match(
						fileContent,
						CUSTOM_SYSTEM_CODE_TAG_BGN + ".*?" + CUSTOM_SYSTEM_CODE_TAG_END,
						RegexOptions.Singleline | RegexOptions.IgnoreCase);
					if (oldSystemCode.Length > 0)
					{
						fileContent = fileContent.Replace(oldSystemCode.Value, newSystemCode);
					}
				}
				return fileContent;
		}

		/// <summary>
		/// タグ内容の取得
		/// </summary>
		/// <param name="content">テキスト内容</param>
		/// <param name="tagBgn">開始タグ</param>
		/// <param name="tagEnd">終了タグ</param>
		/// <returns>内容</returns>
		private static string GetTagContent(string content, string tagBgn, string tagEnd)
		{
			var match = Regex.Matches(content, tagBgn + ".*?" + tagEnd, RegexOptions.Singleline).Cast<Match>()
				.FirstOrDefault();

			var result = (match != null)
				? match.Value.Replace(
					match.Value.Substring(
						0,
						match.Value.IndexOf("\r\n", StringComparison.Ordinal))
					+ "\r\n", "")
					.Replace(tagEnd, "")
				: string.Empty;

			return result;
		}

		/// <summary>
		/// システムコードの置換
		/// </summary>
		/// <param name="content">テキスト内容</param>
		/// <param name="pageId">ページID</param>
		/// <returns>内容</returns>
		private static string ReplaceSystemCode(
			string content,
			string pageId)
		{
			var newSystemCode = content.Replace("[[@@page_id@@]]", pageId);
			return newSystemCode;
		}

		/// <summary>
		/// Metaタグ内容の置換
		/// </summary>
		/// <param name="content">テキスト内容</param>
		/// <param name="siteName">サイト名</param>
		/// <param name="ogTitle">OGタグ タイトル</param>
		/// <param name="ogUrl">OGタグ URL</param>
		/// <param name="ogDescription">OGタグ 説明</param>
		/// <param name="ogImageUrl">OGタグ 画像URL</param>
		/// <returns>内容</returns>
		private static string ReplaceMetaTag(
			string content,
			string siteName,
			string ogTitle,
			string ogUrl,
			string ogDescription,
			string ogImageUrl)
		{
			var newMetaTag = content.Replace("[[@@og_site_name@@]]", siteName)
				.Replace("[[@@og_title@@]]", ogTitle)
				.Replace("[[@@og_type@@]]", Constants.FLG_OGPTAGSETTING_TYPE_ARTICLE)
				.Replace("[[@@og_url@@]]", ogUrl)
				.Replace("[[@@og_description@@]]", ogDescription)
				.Replace("[[@@og_image@@]]", ogImageUrl);
			return newMetaTag;
		}

		/// <summary>
		/// PCサイトのLPデザインファイル削除
		/// </summary>
		/// <param name="model">モデル</param>
		public static void DeletePageFilePc(LandingPageDesignModel model)
		{
			var filePath = GetLpPageFilePathPc(model.PageFileName);
			if (File.Exists(filePath)) File.Delete(filePath);
		}

		/// <summary>
		/// SPサイトのLPデザインファイル削除
		/// </summary>
		/// <param name="model">モデル</param>
		public static void DeletePageFileSp(LandingPageDesignModel model)
		{
			// SP使わない場合はファイル削除しない
			if (Constants.SMARTPHONE_OPTION_ENABLED == false) return;
			if (SmartPhoneUtility.SmartPhoneSiteSettings.Any(setting => setting.SmartPhonePageEnabled == false)) return;
			var filePath = GetLpPageFilePathSp(model.PageFileName);
			if (File.Exists(filePath)) File.Delete(filePath);
		}

		/// <summary>
		/// プレビューファイルのパスを取得
		/// </summary>
		/// <param name="previewKey">プレビュー用キー</param>
		/// <param name="designType">デザインタイプ</param>
		/// <returns>ファイルパス</returns>
		public static string GetPreviewFilePath(string previewKey, string designType)
		{
			var filePath = Path.Combine(GetPreviewDirPath(designType), previewKey);
			return filePath;
		}

		/// <summary>
		/// プレビューファイル用のディレクトリパスを取得
		/// </summary>
		/// <param name="designType"></param>
		/// <returns></returns>
		public static string GetPreviewDirPath(string designType)
		{
			var dirPath = Path.Combine(Constants.PHYSICALDIRPATH_FRONT_PC,
				(designType == LandingPageConst.PAGE_DESIGN_TYPE_PC) ? Constants.CMS_LANDING_PAGE_DIR_PATH_PC : Constants.CMS_LANDING_PAGE_DIR_PATH_SP,
				"_preview");
			return dirPath;
		}

		/// <summary>
		/// プレビュー用のURLを取得
		/// </summary>
		/// <param name="previewKey">プレビュー用のキー</param>
		/// <param name="designType">デザインタイプ</param>
		/// <returns>URL</returns>
		public static string GetPreviewUrl(string previewKey, string designType)
		{
			var url = Constants.PROTOCOL_HTTPS + Constants.SITE_DOMAIN + Constants.PATH_ROOT_FRONT_PC +
				((designType == LandingPageConst.PAGE_DESIGN_TYPE_PC) ? Constants.CMS_LANDING_PAGE_DIR_URL_PC : Constants.CMS_LANDING_PAGE_DIR_URL_SP)
				+ "LpPreview.aspx?previewkey=" + previewKey;
			return url;
		}

		/// <summary>
		/// プレビュー用ファイルが存在するかどうか
		/// </summary>
		/// <param name="previewKey">プレビュー用のキー</param>
		/// <param name="designType">デザインタイプ</param>
		/// <returns>
		/// TRUE：存在する
		/// FALSE：存在しない
		/// </returns>
		public static bool ExistsPreviewFile(string previewKey, string designType)
		{
			var filePath = GetPreviewFilePath(previewKey, designType);
			return File.Exists(filePath);
		}

		/// <summary>
		/// プレビュー用のモデル取得
		/// </summary>
		/// <param name="previewKey">プレビュー用のキー</param>
		/// <param name="designType">デザインタイプ</param>
		/// <returns>モデル</returns>
		public static LandingPageDesignModel GetPreviewModel(string previewKey, string designType)
		{
			// ファイルが存在しない場合はNull
			if (ExistsPreviewFile(previewKey, designType) == false) return null;

			var filePath = GetPreviewFilePath(previewKey, designType);
			string j;
			using (var f = new StreamReader(filePath))
			{
				j = f.ReadToEnd();
			}
			return JsonConvert.DeserializeObject<LandingPageDesignModel>(j);
		}

		/// <summary>
		/// プレビューファイル削除
		/// </summary>
		/// <param name="previewKey">プレビュー用のキー</param>
		/// <param name="designType">デザインタイプ</param>
		public static void DeletePreviewFile(string previewKey, string designType)
		{
			// ファイルが存在しない場合は何もしない
			if (ExistsPreviewFile(previewKey, designType) == false) return;

			var filePath = GetPreviewFilePath(previewKey, designType);
			File.Delete(filePath);
		}

		/// <summary>
		/// プレビューファイル書き込み
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="designType">デザインタイプ</param>
		/// <returns>プレビュー用のキー</returns>
		public static string WritePreviewFile(LandingPageDesignModel model, string designType)
		{
			var previewKey = Guid.NewGuid().ToString().Replace("-", "");
			var filePath = GetPreviewFilePath(previewKey, designType);
			// ディレクトリがなければ作成
			var fi = new FileInfo(filePath);
			if (fi.DirectoryName != null && Directory.Exists(fi.DirectoryName) == false)
			{
				new DirectoryInfo(fi.DirectoryName).Create();
			}
			// プレビューファイルクリーニング
			CleaningPreviewFile(designType);

			var j = JsonConvert.SerializeObject(model);

			using (var sw = new StreamWriter(filePath, false, Encoding.UTF8))
			{
				sw.Write(j);
			}

			return previewKey;
		}

		/// <summary>
		/// プレビューファイルのクリーニング
		/// </summary>
		/// <param name="designType">デザインタイプ</param>
		/// <remarks>更新日時が1時間以上経過したものは削除</remarks>
		public static void CleaningPreviewFile(string designType)
		{
			var dirPath = GetPreviewDirPath(designType);
			// プレビュー用ディレクトリが見つからない場合は無視
			if (Directory.Exists(dirPath) == false) return;

			var dir = new DirectoryInfo(dirPath);
			foreach (var file in dir.GetFiles())
			{
				if (file.LastWriteTime < DateTime.Now.AddHours(-1))
				{
					// 更新日時が1時間たったものは削除
					file.Delete();
				}
			}
		}

		/// <summary>
		/// メンテツール有効化
		/// </summary>
		public static void EnableToMaintenaceTool()
		{
			// メンテツールの有効化
			HttpContext.Current.Session["enable_maintenance_tool"] = true;
		}

		/// <summary>
		/// メンテツールが有効かどうか
		/// </summary>
		/// <returns>
		/// TRUE：有効
		/// FALSE：無効
		/// </returns>
		public static bool EnableMaintenaceTool()
		{
			if (Constants.CMS_LANDING_PAGE_ENABLE_MAINTENANCE_TOOL)
			{
				return true;
			}

			if (HttpContext.Current.Session["enable_maintenance_tool"] != null
				&& (bool)HttpContext.Current.Session["enable_maintenance_tool"])
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// LPデザインファイル再作成
		/// </summary>
		public static void RecreateAllFile()
		{
			// デザインファイルの再作成
			var sv = new LandingPageService();
			var pages = sv.GetAllPage();
			foreach (var page in pages)
			{
				WriteLpPageFilePc(page);
				WriteLpPageFileSp(page);
			}
		}
	}
}
