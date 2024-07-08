/*
=========================================================================================================
  Module      : パーツデザイン管理 ワーカーサービス(PartsDesignWorkerService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using w2.App.Common.Design;
using w2.App.Common.RefreshFileManager;
using w2.App.Common.Util;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.Codes.PageDesign;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ParamModels.PartsDesign;
using w2.Cms.Manager.ViewModels.PartsDesign;
using w2.Cms.Manager.ViewModels.Shared;
using w2.Common.Sql;
using w2.Domain.PartsDesign;
using w2.Domain.PartsDesign.Helper;

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// パーツデザイン管理 ワーカーサービス
	/// </summary>
	public class PartsDesignWorkerService : BaseWorkerService
	{
		/// <summary>
		/// メインビューモデル作成
		/// </summary>
		/// <returns>メインビューモデル</returns>
		public MainViewModel CreateMainVm()
		{
			var mainViewModel = new MainViewModel();
			return mainViewModel;
		}

		/// <summary>
		/// グループリストビューモデル作成
		/// </summary>
		/// <param name="paramModel">検索パラメータモデル</param>
		/// <param name="maxViewCount">最大表示件数</param>
		/// <param name="useType">利用デバイスタイプ</param>
		/// <returns>ビューモデル</returns>
		public GroupListViewModel CreateGroupListVm(
			PartsDesignListSearchParamModel paramModel,
			int maxViewCount,
			string useType = "")
		{
			var pageDesignListSearchCondition = new PartsDesignListSearch()
			{
				GroupId = (string.IsNullOrEmpty(paramModel.GroupId)) ? (long?)null : long.Parse(paramModel.GroupId),
				UseType = (string.IsNullOrEmpty(useType))
					? new [] { paramModel.UseType}
					: (useType == Constants.FLG_PARTSDESIGN_USE_TYPE_PC)
						? new []
						{
							Constants.FLG_PARTSDESIGN_USE_TYPE_PC,
							Constants.FLG_PARTSDESIGN_USE_TYPE_PC_SP
						}
						: new []
						{
							Constants.FLG_PARTSDESIGN_USE_TYPE_SP,
							Constants.FLG_PARTSDESIGN_USE_TYPE_PC_SP
						},
				Keyword = paramModel.Keyword,
				PartsTypes = paramModel.Types.Where(type => type.IsSelected).Select(type => type.Value).ToArray()
			};

			var partsDesignService = new PartsDesignService();
			var count = partsDesignService.GetSearchHitCount(pageDesignListSearchCondition);

			if (count >= maxViewCount)
			{
				var temp = new GroupListViewModel()
				{
					GroupPartsViewModels = new List<GroupViewModel>()
				};
				temp.ErrorMessage = WebMessages.PageDesignPartsDesignMaxViewOrverError
					.Replace("@@ 1 @@", maxViewCount.ToString())
					.Replace("@@ 2 @@", count.ToString());
				return temp;
			}

			var searchResult = partsDesignService.SearchGroup(
				pageDesignListSearchCondition,
				PartsDesignUtility.OtherPartsGroupModel);

			if (searchResult.Length >= maxViewCount)
			{
				return new GroupListViewModel()
				{
					GroupPartsViewModels = new List<GroupViewModel>()
				};
			}
			var result = new GroupListViewModel()
			{
				GroupPartsViewModels = searchResult
				  .Select(
					groupParts => new GroupViewModel(new PartsDesignGroupModel(groupParts.DataSource))
					{
						ListPartsViewModels = groupParts.PartsList
						  .Select(
							parts =>
							{
								var standardPartsSetting = (parts.PartsType == Constants.FLG_PARTSDESIGN_PARTS_TYPE_NORMAL)
								  ? PartsDesignSetting.GetInstance().DesignParts.PartsSetting.FirstOrDefault(
									partsSetting => partsSetting.Path.Contains(parts.FileName))
								  : null;

								return new PartsViewModel
								{
									GroupId = parts.GroupId,
									PartsSortNumber = parts.PartsSortNumber,
									Publish = parts.Publish,
									UseType = parts.UseType,
									PartsId = parts.PartsId,
									PcRealParts = new RealParts(
									  parts.ManagementTitle,
									  DesignCommon.PhysicalDirPathTargetSitePc,
									  parts.FileDirPath,
									  parts.FileName,
									  parts.PartsType,
									  string.Empty,
									  string.Empty,
									  string.Empty)
									{
										StandardPartsSetting = standardPartsSetting
									},
									SpRealParts = new RealParts(
									  parts.ManagementTitle,
									  DesignCommon.PhysicalDirPathTargetSiteSp,
									  parts.FileDirPath,
									  parts.FileName,
									  parts.PartsType,
									  string.Empty,
									  string.Empty,
									  string.Empty)
									{
										StandardPartsSetting = standardPartsSetting
									},
									ManagementTitle = parts.ManagementTitle,
									IsSettingReleaseRange = ((parts.ConditionPublishDateFrom != null)
									  || (parts.ConditionPublishDateTo != null)
									  || (parts.ConditionMemberOnlyType != Constants.FLG_PAGEDESIGN_MEMBER_ONLY_TYPE_ALL)
									  || (string.IsNullOrEmpty(parts.ConditionTargetListIds) == false))
								};
							})
						  .Where(parts => PartsDesignCommon.ValidParts(parts.FileName))
						  .ToList()
					})
				  .ToList()
			};
			return result;
		}

		/// <summary>
		/// パーツ詳細 新規登録 ビューモデル作成
		/// </summary>
		/// <param name="selectStandardParts">新規作成時の選択テンプレート</param>
		/// <returns>ビューモデル</returns>
		public PartsDetailViewModel CreatePartsDetailRegisterVm(string selectStandardParts)
		{
			var vm = CreatePartsDetailRegisterVm(
				PartsDesignUtility.NEW_PARTS_DEFAULT_PARTS_ID,
				selectStandardParts);
			return vm;
		}
		/// <summary>
		/// パーツ詳細 新規登録 ビューモデル作成
		/// </summary>
		/// <param name="partsId">複製元 Parts</param>
		/// <param name="selectStandardParts">新規作成時の選択テンプレート</param>
		/// <returns>ビューモデル</returns>
		public PartsDetailViewModel CreatePartsDetailRegisterVm(long partsId, string selectStandardParts)
		{
			PartsDesignModel partsModel;
			var standardPartsPath = "";
			if (partsId == PartsDesignUtility.NEW_PARTS_DEFAULT_PARTS_ID)
			{
				if (string.IsNullOrEmpty(selectStandardParts)) return new PartsDetailViewModel();
				var temp = selectStandardParts.Split(',');
				if (temp.Length < 2) return new PartsDetailViewModel();

				var partsPrefixStandardPartsName = temp[0];
				standardPartsPath = temp[1];

				partsModel = PartsDesignUtility.CreateNewPartsFileModel(partsPrefixStandardPartsName);
			}
			else
			{
				partsModel = new PartsDesignService().GetParts(partsId);
				standardPartsPath = Path.Combine(partsModel.FileDirPath, partsModel.FileName);
			}
			partsModel.PartsId = PartsDesignUtility.NEW_PARTS_DEFAULT_PARTS_ID;

			// PC コンテンツ読み込み
			var pcFilePath = (partsId == PartsDesignUtility.NEW_PARTS_DEFAULT_PARTS_ID)
				? Path.Combine(DesignCommon.PhysicalDirPathTargetSitePc, standardPartsPath)
				: Path.Combine(
					DesignCommon.PhysicalDirPathTargetSitePc,
					partsModel.FileDirPath,
					partsModel.FileName);
			var pcFileTextAll = DesignCommon.GetFileTextAll(pcFilePath);
			var pcContent = PageDesignUtility.GetEditableAreaForEdit(pcFileTextAll);

			// SP コンテンツ読み込み
			var spFilePath = (partsId == PartsDesignUtility.NEW_PARTS_DEFAULT_PARTS_ID)
				? Path.Combine(DesignCommon.PhysicalDirPathTargetSiteSp, standardPartsPath)
				: Path.Combine(
					DesignCommon.PhysicalDirPathTargetSiteSp,
					partsModel.FileDirPath,
					partsModel.FileName);
			var spFileTextAll = DesignCommon.GetFileTextAll(spFilePath);
			var spContent = PageDesignUtility.GetEditableAreaForEdit(spFileTextAll);

			var partsList = PartsDesignUtility.TemplateStandardPartsList();
			var templateParts = partsList.FirstOrDefault(p => partsModel.FileName.Contains(p.PrefixPartsName));

			var pcRealPage = new RealParts(
				partsModel.ManagementTitle,
				DesignCommon.PhysicalDirPathTargetSitePc,
				partsModel.FileDirPath,
				partsModel.FileName,
				partsModel.PartsType,
				templateParts.Title,
				Path.GetFileName(templateParts.FilePath),
				Path.GetDirectoryName(templateParts.FilePath));

			var spRealPage = new RealParts(
				partsModel.ManagementTitle,
				DesignCommon.PhysicalDirPathTargetSiteSp,
				partsModel.FileDirPath,
				partsModel.FileName,
				partsModel.PartsType,
				templateParts.Title,
				Path.GetFileName(templateParts.FilePath),
				Path.GetDirectoryName(templateParts.FilePath));

			var input = new PartsDesignPartsInput(partsModel)
			{
				PcContentInput =
				{
					Content = pcContent,
				},
				SpContentInput =
				{
					Content = spContent,
				},
				CreateNewFileName = partsModel.FileName,
				TemplateFilePath = standardPartsPath,
				FileDirPath = partsModel.FileDirPath
			};

			if (partsId != PartsDesignUtility.NEW_PARTS_DEFAULT_PARTS_ID)
			{
				input.ManagementTitle = partsModel.ManagementTitle + "_Copy";
				input.CreateNewFileName = PartsDesignUtility.NewPartsFileName(PartsDesignUtility.GetPartsPrefixStandardPartsName(partsModel.FileName));
			}

			var vm = new PartsDetailViewModel
			{
				Input = input,
				PartsModel = partsModel,
				PcRealParts = pcRealPage,
				SpRealParts = spRealPage
			};

			return vm;
		}

		/// <summary>
		/// パーツ詳細 ビューモデル作成
		/// </summary>
		/// <param name="partsId">パーツID nullの場合は新規作成</param>
		/// <param name="buckUpFileName">バックアップファイル名</param>
		/// <returns>ビューモデル</returns>
		public PartsDetailViewModel CreatePartsDetailVm(long partsId, string buckUpFileName = "")
		{
			var partsModel = new PartsDesignService().GetParts(partsId);

			RealParts pcRealParts;
			if (partsModel.PartsType == Constants.FLG_PARTSDESIGN_PARTS_TYPE_CUSTOM)
			{
				if (File.Exists(
					Path.Combine(
						DesignCommon.PhysicalDirPathTargetSitePc,
						partsModel.FileDirPath,
						partsModel.FileName)))
				{
					pcRealParts = PartsDesignCommon.GetCustomPartsList(
						DesignCommon.DeviceType.Pc,
						partsModel.FileName).FirstOrDefault();
				}
				else
				{
					var partsList = PartsDesignUtility.TemplateStandardPartsList();
					var templateParts = partsList.FirstOrDefault(p => partsModel.FileName.Contains(p.PrefixPartsName));

					DesignCommon.FileCopy(
						Path.Combine(DesignCommon.PhysicalDirPathTargetSitePc, templateParts.FilePath),
						Path.Combine(
							DesignCommon.PhysicalDirPathTargetSitePc,
							partsModel.FileDirPath,
							partsModel.FileName));

					pcRealParts = new RealParts(
						partsModel.ManagementTitle,
						DesignCommon.PhysicalDirPathTargetSitePc,
						partsModel.FileDirPath,
						partsModel.FileName,
						partsModel.PartsType,
						templateParts.Title,
						Path.GetFileName(templateParts.FilePath),
						Path.GetDirectoryName(templateParts.FilePath));
				}
			}
			else
			{
				var pcStandardPartsList = PartsDesignCommon.GetStandardPartsList(DesignCommon.DeviceType.Pc);
				pcRealParts = pcStandardPartsList.FirstOrDefault(p => p.FileName == partsModel.FileName);
			}

			// PC コンテンツ読み込み
			var pcFileTextAll = string.Empty;
			if (pcRealParts != null)
			{
				var pcFilePath = string.IsNullOrEmpty(buckUpFileName)
					? pcRealParts.PhysicalFullPath
					: Path.Combine(
						RestoreUtility.GetBuckUpDirectoryPath(
							pcRealParts.PhysicalFullPath,
							DesignCommon.DeviceType.Pc,
							RestoreType.Parts),
						buckUpFileName);
				pcFileTextAll = DesignCommon.GetFileTextAll(pcFilePath);
				pcRealParts.PageTitle = DesignCommon.GetAspxTitle(pcFileTextAll);
				pcRealParts.LastChange = DesignCommon.GetLastChanged(pcFileTextAll);
				UpdateFileOpenTime(pcFilePath);
			}
			else
			{
				pcRealParts = new RealParts();
			}

			RealParts spRealParts;
			if (partsModel.PartsType == Constants.FLG_PARTSDESIGN_PARTS_TYPE_CUSTOM)
			{
				if (File.Exists(
					Path.Combine(
						DesignCommon.PhysicalDirPathTargetSiteSp,
						partsModel.FileDirPath,
						partsModel.FileName)))
				{
					spRealParts = PartsDesignCommon.GetCustomPartsList(
						DesignCommon.DeviceType.Sp,
						partsModel.FileName).FirstOrDefault();
				}
				else
				{
					var partsList = PartsDesignUtility.TemplateStandardPartsList();
					var templateParts = partsList.FirstOrDefault(p => partsModel.FileName.Contains(p.PrefixPartsName));

					DesignCommon.FileCopy(
						Path.Combine(DesignCommon.PhysicalDirPathTargetSiteSp, templateParts.FilePath),
						Path.Combine(
							DesignCommon.PhysicalDirPathTargetSiteSp,
							partsModel.FileDirPath,
							partsModel.FileName));

					spRealParts = new RealParts(
						partsModel.ManagementTitle,
						DesignCommon.PhysicalDirPathTargetSiteSp,
						partsModel.FileDirPath,
						partsModel.FileName,
						partsModel.PartsType,
						templateParts.Title,
						Path.GetFileName(templateParts.FilePath),
						Path.GetDirectoryName(templateParts.FilePath));
				}
			}
			else
			{
				var spStandardPartsList = PartsDesignCommon.GetStandardPartsList(DesignCommon.DeviceType.Sp);
				spRealParts = spStandardPartsList.FirstOrDefault(p => p.FileName == partsModel.FileName);
			}

			// SP コンテンツ読み込み
			var spFileTextAll = string.Empty;
			if (spRealParts != null)
			{
				var spFilePath = string.IsNullOrEmpty(buckUpFileName)
					? spRealParts.PhysicalFullPath
					: Path.Combine(
						RestoreUtility.GetBuckUpDirectoryPath(
							spRealParts.PhysicalFullPath,
							DesignCommon.DeviceType.Sp,
							RestoreType.Parts),
						buckUpFileName);
				spFileTextAll = DesignCommon.GetFileTextAll(spFilePath);
				spRealParts.PageTitle = DesignCommon.GetAspxTitle(spFileTextAll);
				spRealParts.LastChange = DesignCommon.GetLastChanged(spFileTextAll);
				UpdateFileOpenTime(spFilePath);
			}
			else
			{
				spRealParts = new RealParts();
			}

			var input = new PartsDesignPartsInput(partsModel)
			{
				PcContentInput =
				{
					Content = PageDesignUtility.GetEditableAreaForEdit(pcFileTextAll),
				},
				SpContentInput =
				{
					Content = PageDesignUtility.GetEditableAreaForEdit(spFileTextAll),
				}
			};

			var vm = new PartsDetailViewModel
			{
				Input = input,
				PartsModel = partsModel,
				PcRealParts = pcRealParts,
				SpRealParts = spRealParts
			};
			return vm;
		}

		/// <summary>
		/// パーツ選択によるグループ移動 ビューモデル作成
		/// </summary>
		/// <param name="partsId">パーツID</param>
		/// <returns>ビューモデル</returns>
		public GroupMoveViewModel CreateGroupMoveViewModel(long partsId)
		{
			var pageModel = new PartsDesignService().GetParts(partsId);
			var otherModel = PartsDesignUtility.OtherPartsGroupModel;
			var groupModel = (pageModel.GroupId == otherModel.GroupId)
				? otherModel
				: new PartsDesignService().GetGroup(pageModel.GroupId);

			var viewModel = new GroupMoveViewModel
			{
				SelectGroupModel = groupModel,
				PartsId = partsId
			};

			return viewModel;
		}

		/// <summary>
		/// パーツ詳細更新 ページペアリング単位
		/// </summary>
		/// <param name="input">パーツ詳細 入力内容</param>
		/// <returns>エラーメッセージ</returns>
		public string UpdateParts(PartsDesignPartsInput input)
		{
			var errorMessage = input.Validate();

			if (string.IsNullOrEmpty(errorMessage) == false) return errorMessage;

			var pcPartsErrorMessage = WebRequestCheck.Send(Preview(input, DesignCommon.DeviceType.Pc));
			var spPartsErrorMessage = WebRequestCheck.Send(Preview(input, DesignCommon.DeviceType.Sp));
			PageDesignUtility.DeletePreviewFile();

			if (string.IsNullOrEmpty(pcPartsErrorMessage) == false)
			{
				return pcPartsErrorMessage;
			}

			if (string.IsNullOrEmpty(spPartsErrorMessage) == false)
			{
				return spPartsErrorMessage;
			}
			
			if (input.PartsId == PartsDesignUtility.NEW_PARTS_DEFAULT_PARTS_ID)
			{
				var newPageModel = PartsDesignUtility.CreateNewPartsFile(out errorMessage ,input.CreateNewFileName, input.TemplateFilePath);
				if (string.IsNullOrEmpty(errorMessage) == false)
				{
					return errorMessage;
				}

				input.PartsId = newPageModel.PartsId;
			}

			var beforeModel = new PartsDesignService().GetParts(input.PartsId);
			var newModel = input.CreateModel();

			// グループに更新がある場合は先頭に移動
			if (beforeModel.GroupId != newModel.GroupId)
			{
				newModel.PartsSortNumber = 0;
			}

			var pcFilePath = Path.Combine(
				DesignCommon.PhysicalDirPathTargetSitePc,
				beforeModel.FileDirPath,
				beforeModel.FileName);
			var spFilePath = Path.Combine(
				DesignCommon.PhysicalDirPathTargetSiteSp,
				beforeModel.FileDirPath,
				beforeModel.FileName);

			// 初回作成時はバックアップを取らない
			if (string.IsNullOrEmpty(beforeModel.LastChanged) == false)
			{
				// バックアップを作成
				var bkFileName = File.GetLastWriteTime(pcFilePath).ToString("yyyyMMdd_HHmmss") + "_" + Path.GetFileName(pcFilePath) + ".bk";
				RestoreUtility.CreateBuckUpFile(pcFilePath, bkFileName, DesignCommon.DeviceType.Pc, RestoreType.Parts);
				RestoreUtility.CreateBuckUpFile(spFilePath, bkFileName, DesignCommon.DeviceType.Sp, RestoreType.Parts);
			}

			errorMessage += UpdateParts(input, input.PcContentInput, pcFilePath, pcFilePath);
			if (DesignCommon.UseResponsive == false)
			{
				errorMessage += UpdateParts(input, input.SpContentInput, spFilePath, spFilePath);
			}

			if (string.IsNullOrEmpty(errorMessage))
			{
				newModel.LastChanged = this.SessionWrapper.LoginOperatorName;
				try
				{
					new PartsDesignService().UpdateParts(newModel);
				}
				catch (Exception ex)
				{
					errorMessage = WebMessages.DataBaseError;
				}

				// フロントのキャッシュ更新
				RefreshFileManagerProvider.GetInstance(RefreshFileType.PartsDesign).CreateUpdateRefreshFile();

				var param = new List<KeyValuePair<string, string>>
				{
					new KeyValuePair<string, string>(Constants.REQUEST_KEY_PARTS_PREVIEW_PARTS_ID, newModel.PartsId.ToString())
				};

				WebBrowserCapture.Create(
					Constants.PHYSICALDIRPATH_CMS_MANAGER,
					Constants.PAGE_FRONT_PARTS_PREVIEW,
					webUrlParams:param,
					device: WebBrowserCapture.Device.Pc,
					delay: 100,
					iSizeH: 800,
					iSizeW: 800,
					bSizeH: 1280,
					bSizeW: 720);
				WebBrowserCapture.Create(
					Constants.PHYSICALDIRPATH_CMS_MANAGER,
					Path.Combine(DesignCommon.SiteSpRootPath, Constants.PAGE_FRONT_PARTS_PREVIEW),
					webUrlParams: param,
					device: WebBrowserCapture.Device.Sp,
					delay: 100,
					iSizeH: 400,
					iSizeW: 400,
					bSizeH: 400,
					bSizeW: 800);
			}

			return errorMessage;
		}

		/// <summary>
		/// パーツ詳細更新 ページ更新のみ
		/// </summary>
		/// <param name="input">パーツ詳細 入力内容</param>
		/// <param name="contentInput">コンテンツ内容</param>
		/// <param name="inputFilePath">元ページ物理ファイルパス</param>
		/// <param name="outputFilePath">生成ページ物理ファイルパス</param>
		/// <returns>エラーメッセージ</returns>
		private string UpdateParts(
			PartsDesignPartsInput input,
			PartsDesignContentInput contentInput,
			string inputFilePath,
			string outputFilePath)
		{
			var fileTextAll = new StringBuilder(DesignCommon.GetFileTextAll(inputFilePath));

			// コンテンツ 置換
			PageDesignUtility.ReplaceContentsTagForUpdate(fileTextAll, contentInput.Content);

			// 最終更新者 置換
			DesignCommon.ReplaceLastChanged(fileTextAll, this.SessionWrapper.LoginOperatorName);

			if (input.IsCustomPage)
			{
				// タイトル置換
				DesignCommon.ReplaceTitle(fileTextAll, input.ManagementTitle);
			}

			// ファイル書き込み
			var errorMessage = DesignUtility.UpdateFile(outputFilePath, fileTextAll.ToString(), true);

			return errorMessage;
		}

		/// <summary>
		/// プレビュー表示(入力内容による)
		/// </summary>
		/// <param name="input">ページ詳細 入力内容</param>
		/// <param name="deviceType">デバイスタイプ</param>
		/// <returns>プレビューURL</returns>
		public string Preview(PartsDesignPartsInput input, DesignCommon.DeviceType deviceType)
		{

			var model = (input.PartsId == PartsDesignUtility.NEW_PARTS_DEFAULT_PARTS_ID)
				? new PartsDesignModel()
				{
					FileName = input.CreateNewFileName,
					FileDirPath = input.FileDirPath
				}
				: new PartsDesignService().GetParts(input.PartsId);

			//Pageディレクトリにプレビュー用の仮パーツ生成
			var contentInput = (deviceType == DesignCommon.DeviceType.Pc)
				? input.PcContentInput
				: input.SpContentInput;
			var previewPartsFileName = model.FileName + PartsDesignUtility.PREVIEW_PARTS_FILE_EXTENSION;
			var physicalDirPathTargetSite = PageDesignUtility.GetPhysicalDirPathTargetSite(deviceType);

			var sourceFilePath = (input.PartsId == PartsDesignUtility.NEW_PARTS_DEFAULT_PARTS_ID)
				? Path.Combine(physicalDirPathTargetSite, input.TemplateFilePath)
				: Path.Combine(physicalDirPathTargetSite, model.FileDirPath, model.FileName) ;
			var outputFilePath = Path.Combine(physicalDirPathTargetSite, PageDesignUtility.PREVIEW_DIR_PATH, previewPartsFileName);
			UpdateParts(input, contentInput, sourceFilePath, outputFilePath);

			var webUrl = CreatePartsPreviewPage(PageDesignUtility.PREVIEW_DIR_PATH, previewPartsFileName, model.FileName, deviceType);
			return webUrl;
		}

		/// <summary>
		/// プレビュー表示(パーツIDより)
		/// </summary>
		/// <param name="partsId">パーツID</param>
		/// <param name="deviceType">デバイスタイプ</param>
		/// <returns>プレビューURL</returns>
		public string Preview(long partsId, DesignCommon.DeviceType deviceType)
		{
			var model = new PartsDesignService().GetParts(partsId);
			var webUrl = CreatePartsPreviewPage(model.FileDirPath, model.FileName, model.FileName, deviceType);
			return webUrl;
		}

		/// <summary>
		/// プレビューページ生成
		/// </summary>
		/// <param name="pcPartsDirPath">パーツの存在するディレクトリ</param>
		/// <param name="previewPartsFileName">パーツの実ファイル名</param>
		/// <param name="previewPageFileName">プレビューページの実ファイル名</param>
		/// <param name="deviceType">デバイスタイプ</param>
		/// <returns>プレビューURL</returns>
		private string CreatePartsPreviewPage(string pcPartsDirPath, string previewPartsFileName, string previewPageFileName, DesignCommon.DeviceType deviceType)
		{
			var rootDir = (deviceType == DesignCommon.DeviceType.Sp) ? DesignCommon.SiteSpRootPath : "";
			// プレビューページを作成
			var page = new StringBuilder();
			page.Append("<%@ Register TagPrefix=\"uc\" TagName=\"Preview\" Src=\"~/" 
				+ Path.Combine(rootDir, pcPartsDirPath.Replace("\\", "/"), previewPartsFileName).Replace("\\", "/") 
				+ "\" %>");
			page.Append("<%@ Page Language=\"C#\" MasterPageFile=\"~/" + Path.Combine(rootDir, Constants.PAGE_FRONT_SIMPLE_DEFAULT_PREVIEW) + "\" %>\r\n");
			page.Append("<asp:Content ContentPlaceHolderID=\"ContentPlaceHolder1\" Runat=\"Server\">\r\n");
			page.Append("<uc:Preview runat=\"server\" IsPreview=\"True\" />\r\n");
			page.Append("</asp:Content>");
			var previewPageFileNameIncludeExtension = "preview_" + previewPageFileName + PageDesignUtility.PREVIEW_PAGE_FILE_EXTENSION;
			var previewPage = Path.Combine(
				PageDesignUtility.GetPhysicalDirPathTargetSite(deviceType),
				PageDesignUtility.PREVIEW_DIR_PATH,
				previewPageFileNameIncludeExtension);
			DesignUtility.UpdateFile(previewPage, page.ToString());

			var webUrl = PageDesignUtility.WebTargetPageUrl(
				Path.Combine(rootDir, PageDesignUtility.PREVIEW_DIR_PATH),
				previewPageFileName,
				true);
			webUrl = webUrl.Replace(previewPageFileName, previewPageFileNameIncludeExtension);
			return webUrl;
		}

		/// <summary>
		/// グループ追加
		/// </summary>
		/// <param name="name">グループ名</param>
		/// <returns>グループID</returns>
		public int GroupAdd(string name)
		{
			var model = new PartsDesignGroupModel
			{
				GroupName = name,
				LastChanged = this.SessionWrapper.LoginOperatorName
			};
			var id = new PartsDesignService().InsertGroup(model);
			return id;
		}

		/// <summary>
		/// グループ名変更
		/// </summary>
		/// <param name="pageId">グループID</param>
		/// <param name="name">グループ名</param>
		public void GroupNameEdit(long pageId, string name)
		{
			var model = new PartsDesignService().GetGroup(pageId);
			if (model == null) return;

			model.GroupName = name;
			model.LastChanged = this.SessionWrapper.LoginOperatorName;

			new PartsDesignService().UpdateGroup(model);
		}

		/// <summary>
		/// グループ削除
		/// </summary>
		/// <param name="groupId">グループID</param>
		public void GroupDelete(long groupId)
		{
			new PartsDesignService().DeleteGroup(groupId, this.SessionWrapper.LoginOperatorName);
		}

		/// <summary>
		/// グループ順更新
		/// </summary>
		/// <param name="groupIds">グループ順</param>
		public void GroupSortUpdate(long[] groupIds)
		{
			new PartsDesignService().UpdateGroupSort(groupIds);
		}

		/// <summary>
		/// パーツ選択によるグループ移動
		/// </summary>
		/// <param name="groupId">グループID</param>
		/// <param name="partsId">パーツID</param>
		public void GroupMoveEdit(long groupId, long partsId)
		{
			var model = new PartsDesignService().GetParts(partsId);
			model.GroupId = groupId;
			model.PartsSortNumber = 0;
			model.LastChanged = this.SessionWrapper.LoginOperatorName;
			new PartsDesignService().UpdateParts(model);
		}

		/// <summary>
		/// パーツ順序更新
		/// </summary>
		/// <param name="groupId">グループID</param>
		/// <param name="partsIds">パーツ順序</param>
		public void PartsSortUpdate(long groupId, long[] partsIds)
		{
			if (partsIds == null) return;

			var models = partsIds.Select(
				(p, index) => new PartsDesignModel
				{
					GroupId = groupId,
					PartsId = p,
					PartsSortNumber = index + 1,
					LastChanged = this.SessionWrapper.LoginOperatorName
				}).ToArray();
			new PartsDesignService().UpdatePartsSort(models);
		}

		/// <summary>
		/// 管理用タイトル更新
		/// </summary>
		/// <param name="partsId">パーツID</param>
		/// <param name="title">管理用タイトル</param>
		/// <returns>エラーメッセージ</returns>
		public void ManagementTitleEdit(long partsId, string title)
		{
			new PartsDesignService().UpdateManagementTitle(partsId, title);
		}

		/// <summary>
		/// パーツの削除
		/// </summary>
		/// <param name="partsId">パーツID</param>
		/// <returns>エラーメッセージ</returns>
		public string DeleteParts(long partsId)
		{
			var errorMessage = PartsDesignUtility.DeleteParts(partsId);
			return errorMessage;
		}

		/// <summary>
		/// パーツ複製
		/// </summary>
		/// <param name="sourcePartsId">複製元パーツID</param>
		/// <returns>新規パーツID</returns>
		public long CreateCopyFile(long sourcePartsId)
		{
			var newPartsId = PartsDesignUtility.CreateCopyPartsFile(sourcePartsId);
			return newPartsId;
		}

		/// <summary>
		/// パーツ自動バックアップリスト取得
		/// </summary>
		/// <param name="partsId">パーツID</param>
		/// <returns>ページバックアップリスト</returns>
		public AutoBackedUpDesignFileViewModel GetPageBackupList(long partsId)
		{
			var autoBackedUpDesignFileViewModel = new AutoBackedUpDesignFileViewModel(partsId, RestoreType.Parts);
			return autoBackedUpDesignFileViewModel;
		}
	}
}
