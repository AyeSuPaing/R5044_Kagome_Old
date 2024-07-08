/*
=========================================================================================================
  Module      : パーツ管理ユーティリティ(PartsDesignUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using w2.App.Common.Design;
using w2.App.Common.RefreshFileManager;
using w2.Cms.Manager.Codes.Common;
using w2.Common.Util;
using w2.Domain.PartsDesign;

namespace w2.Cms.Manager.Codes.PageDesign
{
	/// <summary>
	/// パーツ管理ユーティリティ
	/// </summary>
	public class PartsDesignUtility
	{
		/// <summary>プレビューファイル拡張子</summary>
		public const string PREVIEW_PARTS_FILE_EXTENSION = ".Preview.ascx";
		/// <summary>パーツ最大数</summary>
		private const int CUSTOM_PARTS_MAX_COUNT = 999;
		/// <summary>特集エリア ファイル接頭</summary>
		public const string FEATUREAREA_TEMP_NAME = "900FAT_";
		/// <summary>特集エリア テンプレートファイルパス</summary>
		public const string FEATUREAREA_TEMP_FILE_PATH = "Form/PageTemplates/PartsBannerTemplate.ascx";
		/// <summary>新規追加ファイル 初期パーツID</summary>
		public const int NEW_PARTS_DEFAULT_PARTS_ID = -1;

		#region パーツファイル生成・更新
		/// <summary>
		/// 新規パーツのモデル作成
		/// </summary>
		/// <param name="partsPrefixStandardPartsName">パーツ種別文字 000TMPL_ など(ValueText管理)</param>
		/// <returns>パーツモデル</returns>
		public static PartsDesignModel CreateNewPartsFileModel(string partsPrefixStandardPartsName)
		{
			var newFileName = NewPartsFileName(partsPrefixStandardPartsName);

			var insertModel = new PartsDesignModel
			{
				PartsId = NEW_PARTS_DEFAULT_PARTS_ID,
				FileName = newFileName,
				FileDirPath = DesignCommon.CUSTOM_PARTS_DIR_PATH,
				Publish = Constants.FLG_PAGEDESIGN_PUBLISH_PUBLIC,
				PartsType = Constants.FLG_PAGEDESIGN_PAGE_TYPE_CUSTOM,
				ManagementTitle = ""
			};

			return insertModel;
		}

		/// <summary>
		/// パーツ新規作成
		/// </summary>
		/// <param name="errorMessage">エラーメッセージ</param>
		/// <param name="newFileName">新規ファイル名</param>
		/// <param name="standardPartsPath">標準パーツテンプレートパス</param>
		/// <param name="managementTitleParam">管理タイトル名(任意)</param>
		/// <param name="areaId">特集エリアID(任意)</param>
		/// <param name="useTypeParam">端末タイプ(任意)</param>
		/// <returns>パーツモデル</returns>
		public static PartsDesignModel CreateNewPartsFile(
			out string errorMessage,
			string newFileName,
			string standardPartsPath,
			string managementTitleParam = "",
			string areaId = "",
			string useTypeParam = "")
		{
			int partsId = 0;
			var newPartsPath = Path.Combine(DesignCommon.CUSTOM_PARTS_DIR_PATH, newFileName);

			var pcStandardPartsPath = Path.Combine(DesignCommon.PhysicalDirPathTargetSitePc, standardPartsPath);
			var pcNewPartsPath = Path.Combine(DesignCommon.PhysicalDirPathTargetSitePc, newPartsPath);

			if (File.Exists(pcStandardPartsPath))
			{
				DesignCommon.FileCopy(pcStandardPartsPath, pcNewPartsPath);
			}
			else
			{
				var defaultPartsTemplatePathPc = Path.Combine(DesignCommon.PhysicalDirPathTargetSitePc, Constants.PAGE_FRONT_CUSTOMPARTS_TEMPLATE.Replace("/", @"\"));
				DesignCommon.FileCopy(defaultPartsTemplatePathPc, pcNewPartsPath);
			}

			// レスポンシブ対応の場合はPCのみ生成
			var spStandardPartsPath = Path.Combine(DesignCommon.PhysicalDirPathTargetSiteSp, standardPartsPath);
			var spNewPartsPath = Path.Combine(DesignCommon.PhysicalDirPathTargetSiteSp, newPartsPath);

			if ((DesignCommon.UseResponsive == false) && File.Exists(spStandardPartsPath))
			{
				DesignCommon.FileCopy(spStandardPartsPath, spNewPartsPath);
			}
			else if ((DesignCommon.UseResponsive == false) && File.Exists(spStandardPartsPath) == false)
			{
				var defaultPartsTemplatePathSp = Path.Combine(DesignCommon.PhysicalDirPathTargetSiteSp, Constants.PAGE_FRONT_CUSTOMPARTS_TEMPLATE.Replace("/", @"\"));
				DesignCommon.FileCopy(defaultPartsTemplatePathSp, spNewPartsPath);
			}

			var managementTitle = (string.IsNullOrEmpty(managementTitleParam))
				? DesignCommon.GetAspxTitle(
					DesignCommon.GetFileTextAll(Path.Combine(DesignCommon.PhysicalDirPathTargetSitePc, standardPartsPath)))
				: managementTitleParam;

			var useType =
				(File.Exists(pcStandardPartsPath) && File.Exists(spStandardPartsPath))
					? Constants.FLG_PAGEDESIGN_USE_TYPE_PC_SP
					: ((File.Exists(pcStandardPartsPath) == false) && File.Exists(spStandardPartsPath))
						? Constants.FLG_PAGEDESIGN_USE_TYPE_SP
						: Constants.FLG_PAGEDESIGN_USE_TYPE_PC;

			var partsDesignModel = new PartsDesignService().GetPartsByFileName(newFileName);
			var error = string.Empty;
			if (partsDesignModel == null)
			{
				var insertModel = new PartsDesignModel
				{
					FileName = newFileName,
					FileDirPath = DesignCommon.CUSTOM_PARTS_DIR_PATH,
					Publish = Constants.FLG_PAGEDESIGN_PUBLISH_PUBLIC,
					PartsType = Constants.FLG_PAGEDESIGN_PAGE_TYPE_CUSTOM,
					ManagementTitle = managementTitle,
					AreaId = areaId,
					UseType = (string.IsNullOrEmpty(useTypeParam)) ? useType : useTypeParam
				};

				var partsDesignService = new PartsDesignService();
				try
				{
					partsId = partsDesignService.InsertParts(insertModel);
				}
				catch(Exception ex)
				{
					error = WebMessages.DataBaseError;
				}
				// フロントのキャッシュ更新
				RefreshFileManagerProvider.GetInstance(RefreshFileType.PartsDesign).CreateUpdateRefreshFile();

				try
				{
					partsDesignModel = partsDesignService.GetParts(partsId);
				}
				catch (Exception ex)
				{
					error = WebMessages.DataBaseError;
				}
			}
			errorMessage = error;

			return partsDesignModel;
		}

		/// <summary>
		/// パーツ複製
		/// </summary>
		/// <param name="sourcePartsId">複製元パーツID</param>
		/// <returns>新規作成パーツのパーツID</returns>
		public static long CreateCopyPartsFile(long sourcePartsId)
		{
			var pageDesignService = new PartsDesignService();
			var sourcePageModel = pageDesignService.GetParts(sourcePartsId);

			var partsPrefix = GetPartsPrefixStandardPartsName(sourcePageModel.FileName);
			var newFileName = NewPartsFileName(partsPrefix);

			// 複製パーツ末尾に追加
			var newManagementTitle = string.Format("{0}" + Constants.COPY_NEW_SUFFIX, sourcePageModel.ManagementTitle);
			var insertPageModel = sourcePageModel.Clone();
			insertPageModel.FileName = newFileName;
			insertPageModel.ManagementTitle = newManagementTitle;
			insertPageModel.Publish = Constants.FLG_PARTSDESIGN_PUBLISH_PRIVATE;

			var newPageId = pageDesignService.InsertParts(insertPageModel);
			var newPageModel = pageDesignService.GetParts(newPageId);


			var sourcePartsPhysicalPathPc = Path.Combine(
				DesignCommon.PhysicalDirPathTargetSitePc,
				sourcePageModel.FileDirPath,
				sourcePageModel.FileName);
			var fileTextAllPc = new StringBuilder(DesignCommon.GetFileTextAll(sourcePartsPhysicalPathPc));
			DesignCommon.ReplaceTitle(fileTextAllPc, newManagementTitle);

			var partsPhysicalPathPc = Path.Combine(
				DesignCommon.PhysicalDirPathTargetSitePc,
				sourcePageModel.FileDirPath,
				newFileName);
			DesignUtility.UpdateFile(partsPhysicalPathPc, fileTextAllPc.ToString());

			// レスポンシブ対応の場合はPCのみ生成
			if (DesignCommon.UseResponsive == false)
			{
				var sourcePartsPhysicalPathSp = Path.Combine(
					DesignCommon.PhysicalDirPathTargetSiteSp,
					sourcePageModel.FileDirPath,
					sourcePageModel.FileName);
				var fileTextAllSp = new StringBuilder(DesignCommon.GetFileTextAll(sourcePartsPhysicalPathSp));
				DesignCommon.ReplaceTitle(fileTextAllSp, newManagementTitle);

				var partsPhysicalPathSp = Path.Combine(
					DesignCommon.PhysicalDirPathTargetSiteSp,
					sourcePageModel.FileDirPath,
					newFileName);
				DesignUtility.UpdateFile(partsPhysicalPathSp, fileTextAllSp.ToString());
			}

			return newPageModel.PartsId;
		}

		/// <summary>
		/// 新規パーツファイル名の取得
		/// </summary>
		/// <param name="partsPrefixStandardPartsName">パーツ設定文字</param>
		/// <returns>新規パーツファイル名</returns>
		public static string NewPartsFileName(string partsPrefixStandardPartsName)
		{
			var allParts = new PartsDesignService().GetAllParts();
			for (var fileNumber = 1; fileNumber <= CUSTOM_PARTS_MAX_COUNT; fileNumber++)
			{
				var newFileName = PartsDesignCommon.PARTS_PREFIX_NAME + partsPrefixStandardPartsName + fileNumber.ToString("000")
					+ DesignCommon.PARTS_FILE_EXTENSION;
				if (File.Exists(Path.Combine(DesignCommon.PhysicalDirPathTargetSitePc, DesignCommon.CUSTOM_PARTS_DIR_PATH, newFileName))
					|| File.Exists(Path.Combine(DesignCommon.PhysicalDirPathTargetSiteSp, DesignCommon.CUSTOM_PARTS_DIR_PATH, newFileName))
					|| (allParts.Any(parts => parts.FileName == newFileName))) 
					continue;

				return newFileName;
			}
			return string.Empty;
		}

		/// <summary>
		/// パーツ削除
		/// </summary>
		/// <param name="partsId">削除対象のパーツID</param>
		/// <returns>エラーメッセージ</returns>
		public static string DeleteParts(long partsId)
		{
			var errorMessage = string.Empty;

			var pcAllPageList = PageDesignCommon.GetCustomPageList(DesignCommon.DeviceType.Pc);
			pcAllPageList.AddRange(PageDesignCommon.GetStandardPageSetting(DesignCommon.DeviceType.Pc));

			var spAllPageList = PageDesignCommon.GetCustomPageList(DesignCommon.DeviceType.Sp);
			spAllPageList.AddRange(PageDesignCommon.GetStandardPageSetting(DesignCommon.DeviceType.Sp));

			var partsDesignService = new PartsDesignService();
			var model = partsDesignService.GetParts(partsId);
			var pcRealParts = (model.PartsType == Constants.FLG_PARTSDESIGN_PARTS_TYPE_CUSTOM)
				? PartsDesignCommon.GetCustomPartsList(DesignCommon.DeviceType.Pc, model.FileName).FirstOrDefault()
				: PartsDesignCommon.GetStandardPartsList(DesignCommon.DeviceType.Pc).FirstOrDefault(p => p.FileName == model.FileName);
			var spRealParts = (model.PartsType == Constants.FLG_PARTSDESIGN_PARTS_TYPE_CUSTOM)
				? PartsDesignCommon.GetCustomPartsList(DesignCommon.DeviceType.Sp, model.FileName).FirstOrDefault()
				: PartsDesignCommon.GetStandardPartsList(DesignCommon.DeviceType.Sp).FirstOrDefault(p => p.FileName == model.FileName);

			var realParts = (pcRealParts != null) && (pcRealParts.Existence == RealPage.ExistStatus.Exist)
				? pcRealParts
				: spRealParts;
			var hitPcPageList = pcAllPageList.Where(
				p =>
				{
					var allFileText = DesignCommon.GetFileTextAll(p.PhysicalFullPath);
					var result = (realParts != null) && realParts.PartsTag.Any(rp => allFileText.Contains(rp.Value));
					return result;
				}).ToArray();

			var hitSpPageList = spAllPageList.Where(
				p =>
				{
					var allFileText = DesignCommon.GetFileTextAll(p.PhysicalFullPath);
					var result = (realParts != null) && realParts.PartsTag.Any(rp => allFileText.Contains(rp.Value));
					return result;
				}).ToArray();

			// ページ内で利用されている場合は削除不可
			if ((hitPcPageList.Length > 0) || (hitSpPageList.Length > 0))
			{
				var pageNameList = hitPcPageList.Select(p => "(PC)" + p.FileName).ToList();
				pageNameList.AddRange(hitSpPageList.Select(p => "(SP)" + p.FileName).ToList());
				return WebMessages.PartsDesignPartsUseError
					.Replace("@@ 1 @@", model.ManagementTitle)
					.Replace("@@ 2 @@", string.Join("<br>", pageNameList));
			}

			// 各ページ削除
			if ((pcRealParts != null) && (pcRealParts.Existence == RealPage.ExistStatus.Exist))
			{
				errorMessage += DesignUtility.DeleteFile(pcRealParts.PhysicalFullPath);
			}

			if ((spRealParts != null) && (spRealParts.Existence == RealPage.ExistStatus.Exist))
			{
				errorMessage += DesignUtility.DeleteFile(spRealParts.PhysicalFullPath);
			}

			if (string.IsNullOrEmpty(errorMessage))
			{
				partsDesignService.DeleteParts(partsId);
			}

			return errorMessage;
		}
		#endregion

		#region パーツリスト取得
		/// <summary>
		/// テンプレート標準パーツリスト
		/// </summary>
		/// <returns></returns>
		public static List<TemplateStandardParts> TemplateStandardPartsList()
		{
			var result = GetValueItemListStandardParts().Select(
				p =>
				{
					var temp = p.Value.Split(',');
					var partsPrefixStandardPartsName = temp[0];
					var standardPartsPath = temp[1];

					var parts = new TemplateStandardParts
					{
						Title = p.Text,
						PrefixPartsName = partsPrefixStandardPartsName,
						FilePath = standardPartsPath
					};

					return parts;
				}).ToList();

			return result;
		}
		#endregion

		/// <summary>
		/// 標準パーツ取得
		/// </summary>
		/// <returns>標準パーツ情報</returns>
		public static SelectListItem[] GetValueItemListStandardParts()
		{
			var standardPartsItems
				= PartsDesignSetting.GetInstance().DesignParts.UseTemplateStandardParts
					.Select(
					s => new SelectListItem
						{
							Value = s.Value,
							Text = s.Text
						}).ToArray();

			return standardPartsItems;
		}

		#region パーツ内 コンテンツ取得
		/// <summary>
		/// ファイル名よりパーツ種別文字 000TMPL_ など(ValueText管理) を取得
		/// </summary>
		/// <param name="fileName">ファイル名</param>
		/// <returns>パーツ設定文字</returns>
		public static string GetPartsPrefixStandardPartsName(string fileName)
		{
			var partsInitial = StringUtility.ToEmpty(Path.GetFileNameWithoutExtension(fileName))
				.Replace(PartsDesignCommon.PARTS_PREFIX_NAME, "");
			partsInitial = (partsInitial.Length >= 3) ? partsInitial.Substring(0, partsInitial.Length - 3) : "";
			return partsInitial;
		}
		#endregion

		#region プロパティ
		/// <summary>「その他」グループモデル</summary>
		public static PartsDesignGroupModel OtherPartsGroupModel
		{
			get
			{
				return new PartsDesignGroupModel
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
	/// テンプレート標準パーツ
	/// </summary>
	public class TemplateStandardParts
	{
		/// <summary>パーツタイトル</summary>
		public string Title { get; set; }
		/// <summary>パーツ種別名</summary>
		public string PrefixPartsName { get; set; }
		/// <summary>パーツファイルパス</summary>
		public string FilePath { get; set; }
	}
}
