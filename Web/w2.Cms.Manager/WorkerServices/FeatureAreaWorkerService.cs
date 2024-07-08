/*
=========================================================================================================
  Module      : 特集エリアワーカーサービス(FeatureAreaWorkerService.cs)
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
using System.Web.Mvc;
using w2.App.Common.Design;
using w2.App.Common.Preview;
using w2.App.Common.RefreshFileManager;
using w2.App.Common.Util;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.Codes.PageDesign;
using w2.Cms.Manager.Controllers;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ParamModels.FeatureArea;
using w2.Cms.Manager.ViewModels.FeatureArea;
using w2.Common.Web;
using w2.Domain.FeatureArea;
using w2.Domain.PartsDesign;

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// 特集エリアワーカーサービス
	/// </summary>
	public class FeatureAreaWorkerService : BaseWorkerService
	{
		/// <summary>プレビューファイル名</summary>
		private const string PREVIEW_FILE_NAME = "Form/Parts.Preview.aspx";

		/// <summary>
		/// リストビューモデル作成
		/// </summary>
		/// <param name="pm">パラメタモデル</param>
		/// <returns>ビューモデル</returns>
		public ListViewModel CreateListVm(FeatureAreaListParamModel pm)
		{
			var service = new FeatureAreaService();
			var list = service.GetFeatureAreaSearchAll().Select(data => new ListViewRow(data.DataSource)).ToArray();
			var areaTypes = service.GetFeatureAreaTypeList();
			var partsList = new PartsDesignService().GetAllParts();
			var bannerAllList = service.GetAllFeatureAreaBanner();

			foreach (var areaType in areaTypes)
			{
				foreach (var area in list.Where(area => area.AreaTypeId == areaType.AreaTypeId))
				{
					area.AreaTypeName = areaType.AreaTypeName;
				}
			}

			foreach (var area in list)
			{
				var parts = partsList.FirstOrDefault(p => p.AreaId == area.AreaId);
				area.PartsModel = parts ?? new PartsDesignModel();
			}

			if (pm.PublicDateKbn != Constants.DateSelectType.Unselected)
			{
				list = list.Where(data => CheckPublicDate(data, pm, bannerAllList)).ToArray();
			}

			if (string.IsNullOrEmpty(pm.UseType) == false)
			{
				list = list.Where(data => (data.PartsModel.UseType == pm.UseType)).ToArray();
			}

			if (string.IsNullOrEmpty(pm.AreaType) == false)
			{
				list = list.Where(data => (data.AreaTypeId == pm.AreaType)).ToArray();
			}

			if (string.IsNullOrEmpty(pm.FreeWord) == false)
			{
				list = list.Where(
					data => (data.AreaId.Contains(pm.FreeWord) || (data.AreaName.Contains(pm.FreeWord))
						|| (data.InternalMemo.Contains(pm.FreeWord))))
					.ToArray();
			}

			foreach (var data in list)
			{
				data.ThumbFileNames = bannerAllList
					.Where(b => b.AreaId == data.AreaId)
					.Select(banner => HttpUtility.UrlEncode(banner.FilePath).Replace("+", "%20"))
					.ToArray();
			}

			var result = new ListViewModel
			{
				AreaTypes = areaTypes.Select(
					areaType => new SelectListItem()
					{
						Text = areaType.AreaTypeName,
						Value = areaType.AreaTypeId
					}).ToArray(),
				ParamModel = pm,
				List = list
					.OrderByDescending(data => data.DateChanged)
					.Skip((pm.PagerNo - 1) * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST)
					.Take(Constants.CONST_DISP_CONTENTS_DEFAULT_LIST)
					.ToArray(),
				ThumbFileName = PREVIEW_FILE_NAME,
				HitCount = list.Count(),
			};

			return result;
		}

		/// <summary>
		/// 公開日チェック
		/// </summary>
		/// <param name="data">特集エリア</param>
		/// <param name="pm">パラメタモデル</param>
		/// <param name="allFeatureAreaBannerModels">全特集エリアバナーモデル</param>
		/// <returns>公開日検索対象か</returns>
		private bool CheckPublicDate(
			FeatureAreaModel data,
			FeatureAreaListParamModel pm,
			FeatureAreaBannerModel[] allFeatureAreaBannerModels)
		{
			var compareDateTime = DateTime.Now;
			switch (pm.PublicDateKbn)
			{
				case Constants.DateSelectType.Day:
					compareDateTime = DateTime.Now.AddDays(-1);
					break;

				case Constants.DateSelectType.Week:
					compareDateTime = DateTime.Now.AddDays(-7);
					break;

				case Constants.DateSelectType.Month:
					compareDateTime = DateTime.Now.AddMonths(-1);
					break;

				case Constants.DateSelectType.ThreeMonth:
					compareDateTime = DateTime.Now.AddMonths(-3);
					break;

				case Constants.DateSelectType.AfterThreeMonth:
					compareDateTime = DateTime.Now.AddMonths(-3);
					break;
			}

			foreach(var banner in allFeatureAreaBannerModels.Where(m => m.AreaId == data.AreaId))
			{
				var checkCreatedDate = ((pm.PublicDateKbn != Constants.DateSelectType.AfterThreeMonth)
					? ((banner.ConditionPublishDateFrom > compareDateTime) || (banner.ConditionPublishDateFrom > compareDateTime))
					: ((banner.ConditionPublishDateFrom < compareDateTime) && (banner.ConditionPublishDateFrom < compareDateTime)));
				if (checkCreatedDate) return true;
			}

			return false;
		}

		/// <summary>
		/// 詳細ビューモデル作成
		/// </summary>
		/// <param name="areaId">特集エリアID</param>
		/// <returns>ビューモデル</returns>
		public DetailViewModel CreateDetailVm(string areaId)
		{
			var featureArea = new FeatureAreaService().GetFeatureArea(areaId);
			var model = new DetailViewModel(featureArea);
			return model;
		}

		/// <summary>
		/// 詳細画面更新
		/// </summary>
		/// <param name="input">特集エリア</param>
		/// <param name="actionStatus">アクションステータス</param>
		/// <returns>エラー内容</returns>
		public string InsertUpdateFeatureArea(FeatureAreaInput input, ActionStatus actionStatus)
		{
			// バナーの順番を並び替えた際に画像が消えるためここで保持させる
			var index = 0;

			// banner.BannerImageInput.BaseNameで入っている番号と一致する番号を探して画像と紐づける
			var uploadImageList = input.BannerInput
				.Select(
					banner => new KeyValuePair<int, HttpPostedFileBase>(
						int.Parse(Regex.Match(banner.BannerImageInput.BaseName, @"(?<=\[)[0-9]+(?=\])").ToString()),
						banner.BannerImageInput.UploadFile))
				.OrderBy(kvp => kvp.Key)
				.Select(kvp => kvp.Value)
				.ToArray();
			foreach (var banner in input.BannerInput.OrderBy(data => int.Parse(data.Index)))
			{
				banner.BannerImageInput.UploadFile = uploadImageList[index];
				banner.BannerNo = index++;
				banner.AreaId = input.AreaId;
			}
			var errorMessage = input.Validate(actionStatus == ActionStatus.Update);
			if (string.IsNullOrEmpty(errorMessage) == false) return errorMessage;

			var model = input.CreateModel();
			model.LastChanged = this.SessionWrapper.LoginOperatorName;
			if ((actionStatus == ActionStatus.Insert) || (actionStatus == ActionStatus.CopyInsert))
			{
				try
				{
					new FeatureAreaService().InsertFeatureArea(model);
				}
				catch(Exception ex)
				{
					errorMessage = WebMessages.DataBaseError;
					return errorMessage;
				}
				var newFileName = PartsDesignUtility.NewPartsFileName(PartsDesignUtility.FEATUREAREA_TEMP_NAME);
				var partsModel = PartsDesignUtility.CreateNewPartsFile(
					out errorMessage,
					newFileName,
					PartsDesignUtility.FEATUREAREA_TEMP_FILE_PATH,
					input.AreaName,
					input.AreaId,
					input.UseType);

				if (string.IsNullOrEmpty(errorMessage) == false) return errorMessage;

				var pcPhysicalFullPath = Path.Combine(
					DesignCommon.PhysicalDirPathTargetSitePc,
					partsModel.FileDirPath,
					partsModel.FileName);
				var pcFileTextAllForCreate = ReplaceContentsTagForUpdate(
					DesignCommon.GetFileTextAll(pcPhysicalFullPath),
					input.AreaId,
					input.AreaTypeId,
					DesignCommon.DeviceType.Pc);
				DesignUtility.UpdateFile(pcPhysicalFullPath, pcFileTextAllForCreate);

				// レスポンシブ対応の場合はPCのみ生成
				if (DesignCommon.UseResponsive == false)
				{
					var spPhysicalFullPath = Path.Combine(
						DesignCommon.PhysicalDirPathTargetSiteSp,
						partsModel.FileDirPath,
						partsModel.FileName);
					var spFileTextAllForCreate = new FeatureAreaController().ReplaceContentsTagForUpdate(
						DesignCommon.GetFileTextAll(spPhysicalFullPath),
						input.AreaId,
						input.AreaTypeId,
						DesignCommon.DeviceType.Sp);
					DesignUtility.UpdateFile(spPhysicalFullPath, spFileTextAllForCreate);
				}
			}
			else
			{
				try
				{
					new FeatureAreaService().UpdateFeatureArea(model);
				}
				catch
				{
					errorMessage = WebMessages.DataBaseError;
					return errorMessage;
				}
			}
			ImageUpload(input);
			Preview(input, DesignCommon.DeviceType.Pc, true);

			WebBrowserCapture.Create(
				Constants.PHYSICALDIRPATH_CMS_MANAGER,
				PREVIEW_FILE_NAME,
				new List<KeyValuePair<string, string>>()
				{
					new KeyValuePair<string, string>(Constants.FIELD_FEATUREAREA_AREA_ID, input.AreaId)
				},
				device: WebBrowserCapture.Device.Pc,
				delay: 100,
				iSizeH: 800,
				iSizeW: 800,
				bSizeH: 1280,
				bSizeW: 720);

			// フロント系サイトを最新情報へ更新
			RefreshFileManagerProvider.GetInstance(RefreshFileType.FeatureAreaBanner).CreateUpdateRefreshFile();

			return "";
		}

		/// <summary>
		/// 詳細画面削除
		/// </summary>
		/// <param name="areaId">特集エリアID</param>
		/// <returns>エラーメッセージ</returns>
		public string DeleteFeatureArea(string areaId)
		{
			var errorMessage = "";
			var model = new PartsDesignService().GetPartsByAreaId(areaId);
			if (model != null)
			{
				errorMessage = PartsDesignUtility.DeleteParts(model.PartsId);
			}
			if (string.IsNullOrEmpty(errorMessage))
			{
				new FeatureAreaService().DeleteFeatureArea(areaId);
			}
			return errorMessage;
		}

		/// <summary>
		/// プレビュー表示
		/// </summary>
		/// <param name="input">入力内容</param>
		/// <param name="deviceType">デバイスタイプ</param>
		/// <param name="isThumb">サムネイル用か</param>
		/// <param name="typeInput">特集エリアタイプ情報</param>
		/// <returns>プレビューURL</returns>
		public string SorBannerAndPreview(
			FeatureAreaInput input,
			DesignCommon.DeviceType deviceType = DesignCommon.DeviceType.Pc,
			bool isThumb = false,
			FeatureAreaTypeInput typeInput = null)
		{
			// 画面での並びに並べる
			var index = 0;
			foreach (var banner in input.BannerInput)
			{
				banner.BannerNo = index++;
				banner.AreaId = input.AreaId;
			}

			// プレビュー画面生成 URL返却
			return Preview(input, deviceType, isThumb, typeInput);
		}

		/// <summary>
		/// プレビュー表示
		/// </summary>
		/// <param name="input">入力内容</param>
		/// <param name="deviceType">デバイスタイプ</param>
		/// <param name="isThumb">サムネイル用か</param>
		/// <param name="typeInput">特集エリアタイプ情報</param>
		/// <returns>プレビューURL</returns>
		public string Preview(
			FeatureAreaInput input,
			DesignCommon.DeviceType deviceType = DesignCommon.DeviceType.Pc,
			bool isThumb = false,
			FeatureAreaTypeInput typeInput = null)
		{
			var frontPath = Constants.PHYSICALDIRPATH_FRONT_PC + ((deviceType == DesignCommon.DeviceType.Pc)
				? ""
				: DesignCommon.SiteSpRootPath);
			var targetFile = frontPath + "Form/PageTemplates/PartsBannerTemplate.ascx";
			UpdateParts(input, targetFile, targetFile + ".Preview.ascx", typeInput, deviceType);
			UpdateFile(
				frontPath + PREVIEW_FILE_NAME,
				FeatureAreaPreview.CreatePreviewPage(isThumb, (deviceType == DesignCommon.DeviceType.Pc)));

			// 情報一時保存
			var model = input.CreateModel();
			FeatureAreaPreview.InsertFeatureAreaPreview(input.AreaId, model.DataSource);
			FeatureAreaPreview.InsertFeatureAreaBannerPreview(
				input.AreaId,
				input.BannerInput.Select(banner => banner.CreateModel().DataSource).ToArray());

			var url = new UrlCreator(
				Path.Combine(
					((Constants.PATH_ROOT_FRONT_PC.StartsWith(Uri.UriSchemeHttp))
						? Constants.PATH_ROOT_FRONT_PC
						: (Constants.PROTOCOL_HTTPS + Constants.SITE_DOMAIN + Constants.PATH_ROOT_FRONT_PC))
						+ ((deviceType == DesignCommon.DeviceType.Pc) ? "" : DesignCommon.SiteSpRootPath),
					PREVIEW_FILE_NAME));
			url.AddParam(Constants.REQUEST_KEY_FRONT_PREVIEW_HASH, FeatureAreaPreview.CreateFeatureAreaHash());

			return url.CreateUrl();
		}

		/// <summary>
		/// 更新処理
		/// </summary>
		/// <param name="input">入力情報</param>
		/// <param name="inputFilePath">置換元対象ファイルパス</param>
		/// <param name="outputFilePath">出力ファイルパス</param>
		/// <param name="typeInput">特集エリアタイプ情報</param>
		/// <param name="deviceType">デバイスタイプ</param>
		private void UpdateParts(
			FeatureAreaInput input,
			string inputFilePath,
			string outputFilePath,
			FeatureAreaTypeInput typeInput = null,
			DesignCommon.DeviceType deviceType = DesignCommon.DeviceType.Pc)
		{
			// ファイル読み込み＆編集領域取得
			var fileTextAll = ReplaceContentsTagForUpdate(input, GetFileTextAll(inputFilePath), typeInput, deviceType);
			UpdateFile(outputFilePath, fileTextAll);
		}

		/// <summary>
		/// ファイル更新
		/// </summary>
		/// <param name="targetFilePath">更新対象ファイルパス</param>
		/// <param name="fileTextAll">ファイルに書き込む内容</param>
		protected void UpdateFile(string targetFilePath, string fileTextAll)
		{
			//------------------------------------------------------
			// ファイル書き込み処理
			//------------------------------------------------------
			if ((File.Exists(targetFilePath) == false)
				|| ((File.GetAttributes(targetFilePath) & FileAttributes.ReadOnly) != 0) == false)
			{
				using (var sw = new StreamWriter(targetFilePath, false, Encoding.UTF8))
				{
					sw.Write(fileTextAll);
				}
			}
		}

		/// <summary>
		/// ファイルテキスト取得
		/// </summary>
		/// <param name="targetFilePath">対象ファイル</param>
		/// <returns>ファイルの内容</returns>
		public static string GetFileTextAll(string targetFilePath)
		{
			using (var srReader = new StreamReader(targetFilePath, Encoding.UTF8))
			{
				var fileTextAll = srReader.ReadToEnd();

				// 改行コードはWindown環境(CRLF)ではない場合、CRLFに変換する
				fileTextAll = fileTextAll.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");

				return fileTextAll;
			}
		}

		/// <summary>
		/// 画像アップロード
		/// </summary>
		/// <param name="input">特集エリア情報</param>
		private void ImageUpload(FeatureAreaInput input)
		{
			var feature = Path.Combine(Constants.PHYSICALDIRPATH_CONTENTS_ROOT, Constants.PATH_FEATURE_IMAGE);
			foreach (var bannerInput in input.BannerInput)
			{
				var image = bannerInput.BannerImageInput;

				// 画像リストから選択されていない且つアップロードファイルがある
				if ((image.ImageId == 0) && (image.UploadFile != null))
				{
					image.UploadFile.SaveAs(
						Path.Combine(
							feature,
							Path.GetFileName(
								(image.UploadFile.FileName == "blob"
									? bannerInput.FileName
									: image.UploadFile.FileName))));
				}
			}
		}

		/// <summary>
		/// 更新用コンテンツタグ置換（特集エリアタイプ指定）
		/// </summary>
		/// <param name="fileTextAll">テンプレートファイル内容</param>
		/// <param name="areaId">エリアID</param>
		/// <param name="areaTypeId">特集エリアタイプID</param>
		/// <param name="deviceType"></param>
		/// <returns>更新後コンテンツ</returns>
		public string ReplaceContentsTagForUpdate(
			string fileTextAll,
			string areaId,
			string areaTypeId,
			DesignCommon.DeviceType deviceType)
		{
			var model = new FeatureAreaService().GetFeatureAreaType(areaTypeId);
			var isPc = (DesignCommon.DeviceType.Pc == deviceType);

			var result = ReplaceContentsTagForUpdate(
				fileTextAll,
				areaId,
				isPc ? model.PcStartTag : model.SpStartTag,
				isPc ? model.PcRepeatTag : model.SpRepeatTag,
				isPc ? model.PcEndTag : model.SpEndTag,
				isPc ? model.PcScriptTag : model.SpScriptTag);
			return result;
		}

		/// <summary>
		/// 更新用コンテンツタグ置換（プレビュー情報指定）
		/// </summary>
		/// <param name="input">特集エリア情報</param>
		/// <param name="fileTextAll">テンプレートファイル内容</param>
		/// <param name="typeInput">特集エリアタイプ情報</param>
		/// <param name="deviceType">デバイスタイプ</param>
		/// <returns>更新後コンテンツ</returns>
		protected string ReplaceContentsTagForUpdate(
			FeatureAreaInput input,
			string fileTextAll,
			FeatureAreaTypeInput typeInput,
			DesignCommon.DeviceType deviceType)
		{
			var model = (typeInput == null)
				? new FeatureAreaService().GetFeatureAreaType(input.AreaTypeId)
				: typeInput.CreateModel();

			var isPc = (DesignCommon.DeviceType.Pc == deviceType);
			var resultString = ReplaceContentsTagForUpdate(
				fileTextAll,
				input.AreaId,
				isPc ? model.PcStartTag : model.SpStartTag,
				isPc ? model.PcRepeatTag : model.SpRepeatTag,
				isPc ? model.PcEndTag : model.SpEndTag,
				isPc ? model.PcScriptTag : model.SpScriptTag);
			return resultString;
		}

		/// <summary>
		/// 更新用コンテンツタグ置換（更新情報指定）
		/// </summary>
		/// <param name="fileTextAll">テンプレートファイル内容</param>
		/// <param name="areaId">特集エリアID</param>
		/// <param name="startTag">開始タグ</param>
		/// <param name="repeatTag">繰り返しタグ</param>
		/// <param name="endTag">終了タグ</param>
		/// <param name="scriptTag">スクリプトタグ</param>
		/// <returns>変換後コンテンツ</returns>
		public string ReplaceContentsTagForUpdate(
			string fileTextAll,
			string areaId,
			string startTag,
			string repeatTag,
			string endTag,
			string scriptTag)
		{
			var replaceSettings = new[]
			{
				new ReplaceSetting(
					ReplaceSetting.ReplaceType.Prop,
					string.Format("this.FeatureAreaId = \"{0}\";", areaId)),
				new ReplaceSetting(ReplaceSetting.ReplaceType.Start, startTag),
				new ReplaceSetting(ReplaceSetting.ReplaceType.Repeat, repeatTag),
				new ReplaceSetting(ReplaceSetting.ReplaceType.End, endTag),
				new ReplaceSetting(ReplaceSetting.ReplaceType.Script, scriptTag),
			};

			foreach (var replaceSetting in replaceSettings)
			{
				fileTextAll = replaceSetting.Replace(fileTextAll);
			}

			return fileTextAll;
		}
	}

	/// <summary>
	/// 置換用設定
	/// </summary>
	class ReplaceSetting
	{
		/// <summary>置換用開始タグフォーマット</summary>
		public const string REPLACE_START_TAG_FORMAT = "<%-- ▽{0}▽ --%>";
		/// <summary>置換用終了タグフォーマット</summary>
		public const string REPLACE_END_TAG_FORMAT = "<%-- △{0}△ --%>";
		/// <summary>プロパティ開始タグ</summary>
		public const string PROP_START_TAG = "編集可能領域：プロパティ設定";
		/// <summary>プロパティ終了タグ</summary>
		public const string PROP_END_TAG = "編集可能領域";
		/// <summary>開始タグ</summary>
		public const string START_TAG = "開始タグ";
		/// <summary>繰り返しタグ</summary>
		public const string REPEAT_TAG = "繰り返しタグ";
		/// <summary>終了タグ</summary>
		public const string END_TAG = "終了タグ";
		/// <summary>スクリプトタグ</summary>
		public const string SCRIPT_TAG = "スクリプトタグ";

		/// <summary>置換タイプ</summary>
		public enum ReplaceType
		{
			/// <summary>プロパティ</summary>
			Prop,
			/// <summary>開始タグ</summary>
			Start,
			/// <summary>繰り返しタグ</summary>
			Repeat,
			/// <summary>完了タグ</summary>
			End,
			/// <summary>スクリプトタグ</summary>
			Script,
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="type">置換タイプ</param>
		/// <param name="contents">内容</param>
		public ReplaceSetting(ReplaceType type, string contents)
		{
			switch (type)
			{
				case ReplaceType.Prop:
					this.ReplaceStartTag = GetReplaceStartTag(PROP_START_TAG);
					this.ReplaceEndTag = GetReplaceEndTag(PROP_END_TAG);
					this.ReplaceContents = string.Join(
						"\r\n",
						GetReplaceStartTag(PROP_START_TAG),
						contents,
						GetReplaceEndTag(PROP_END_TAG));
					break;

				case ReplaceType.Start:
					this.ReplaceStartTag = GetReplaceStartTag(START_TAG);
					this.ReplaceEndTag = GetReplaceEndTag(START_TAG);
					this.ReplaceContents = contents;
					break;

				case ReplaceType.Repeat:
					this.ReplaceStartTag = GetReplaceStartTag(REPEAT_TAG);
					this.ReplaceEndTag = GetReplaceEndTag(REPEAT_TAG);
					this.ReplaceContents = contents;
					break;

				case ReplaceType.End:
					this.ReplaceStartTag = GetReplaceStartTag(END_TAG);
					this.ReplaceEndTag = GetReplaceEndTag(END_TAG);
					this.ReplaceContents = contents;
					break;

				case ReplaceType.Script:
					this.ReplaceStartTag = GetReplaceStartTag(SCRIPT_TAG);
					this.ReplaceEndTag = GetReplaceEndTag(SCRIPT_TAG);
					this.ReplaceContents = contents;
					break;
			}
		}

		/// <summary>
		/// 置換開始タグ取得
		/// </summary>
		/// <param name="startTag">開始タグ</param>
		/// <returns>置換開始タグ</returns>
		public static string GetReplaceStartTag(string startTag)
		{
			return string.Format(REPLACE_START_TAG_FORMAT, startTag);
		}

		/// <summary>
		/// 置換終了タグ取得
		/// </summary>
		/// <param name="endTag">終了タグ</param>
		/// <returns>置換終了タグ</returns>
		public static string GetReplaceEndTag(string endTag)
		{
			return string.Format(REPLACE_END_TAG_FORMAT, endTag);
		}

		/// <summary>
		/// 置換処理
		/// </summary>
		/// <param name="fileTextAll">全体文字列</param>
		/// <returns>置換後の文字列</returns>
		public string Replace(string fileTextAll)
		{
			return fileTextAll.Replace(GetOldValue(fileTextAll), this.ReplaceContents);
		}

		/// <summary>
		/// 置換対象文字列取得
		/// </summary>
		/// <param name="fileTextAll">全体文字列</param>
		/// <returns>置換対象文字列</returns>
		private string GetOldValue(string fileTextAll)
		{
			return Regex.Match(
				fileTextAll,
				this.ReplacePattern,
				RegexOptions.Singleline | RegexOptions.IgnoreCase).Value;
		}

		/// <summary>置換開始タグ</summary>
		public string ReplaceStartTag { get; set; }
		/// <summary>置換終了タグ</summary>
		public string ReplaceEndTag { get; set; }
		/// <summary>置換要文字列</summary>
		public string ReplaceContents { get; set; }
		/// <summary>置換対象取得</summary>
		public string ReplacePattern
		{
			get
			{
				return string.Format("{0}.*?{1}", this.ReplaceStartTag, this.ReplaceEndTag);
			}
		}
	}
}
