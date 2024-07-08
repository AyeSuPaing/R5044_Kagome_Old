/*
=========================================================================================================
  Module      : ページデザイン管理 ワーカーサービス(PageDesignWorkerService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using w2.App.Common.Design;
using w2.App.Common.RefreshFileManager;
using w2.App.Common.Util;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.Codes.PageDesign;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ParamModels.PageDesign;
using w2.Cms.Manager.ViewModels.PageDesign;
using w2.Cms.Manager.ViewModels.Shared;
using w2.Common.Util;
using w2.Domain.Coordinate;
using w2.Domain.PageDesign;
using w2.Domain.PageDesign.Helper;
using w2.Domain.PartsDesign;

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// ページデザイン管理 ワーカーサービス
	/// </summary>
	public class PageDesignWorkerService : BaseWorkerService
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
		/// <returns>ビューモデル</returns>
		public GroupListViewModel CreateGroupListVm(PageDesignListSearchParamModel paramModel)
		{
			var pageDesignListSearchCondition = new PageDesignListSearch()
			{
				GroupId = (string.IsNullOrEmpty(paramModel.GroupId)) ? (long?)null : long.Parse(paramModel.GroupId),
				UseType = new[] { paramModel.UseType },
				Keyword = paramModel.Keyword,
				PageTypes = paramModel.Types.Where(type => type.IsSelected).Select(type => type.Value).ToArray()
			};

			var pageDesignService = new PageDesignService();
			var count = pageDesignService.GetSearchHitCount(pageDesignListSearchCondition);

			if (count >= Constants.PAGE_PARTS_MAX_VIEW_CONTENT_COUNT)
			{
				var temp = new GroupListViewModel()
				{
					GroupPageViewModels = new List<GroupViewModel>()
				};
				temp.ErrorMessage = WebMessages.PageDesignPartsDesignMaxViewOrverError
					.Replace("@@ 1 @@", Constants.PAGE_PARTS_MAX_VIEW_CONTENT_COUNT.ToString())
					.Replace("@@ 2 @@", count.ToString());
				return temp;
			}

			var searchResult = pageDesignService.SearchGroup(
				pageDesignListSearchCondition,
				PageDesignUtility.OtherPageGroupModel);

			var reslt = new GroupListViewModel()
			{
				GroupPageViewModels = searchResult.Select(
					s => new GroupViewModel(new PageDesignGroupModel(s.DataSource))
					{
						ListPageViewModels = s.PageList.Select(
							p => new PageViewModel()
							{
								GroupId = p.GroupId,
								PageSortNumber = p.PageSortNumber,
								Publish = p.Publish,
								UseType = p.UseType,
								PageId = p.PageId,
								PcRealPage = new RealPage(
									p.ManagementTitle,
									DesignCommon.PhysicalDirPathTargetSitePc,
									p.FileDirPath,
									p.FileName,
									p.PageType),
								SpRealPage = new RealPage(
									p.ManagementTitle,
									DesignCommon.PhysicalDirPathTargetSiteSp,
									p.FileDirPath,
									p.FileName,
									p.PageType),
								ManagementTitle = p.ManagementTitle,
								MetadataDesc = p.MetadataDesc,
								IsSettingReleaseRange = (p.ConditionPublishDateFrom != null)
									|| (p.ConditionPublishDateTo != null)
									|| (p.ConditionMemberOnlyType != Constants.FLG_PAGEDESIGN_MEMBER_ONLY_TYPE_ALL)
									|| (string.IsNullOrEmpty(p.ConditionTargetListIds) == false),
								DateChanged = p.DateChanged
							}).ToList()
					}).ToList()
			};
			return reslt;
		}

		/// <summary>
		/// ページ詳細 新規登録 ビューモデル作成
		/// </summary>
		/// <returns>ビューモデル</returns>
		public PageDetailViewModel CreatePageDetailRegisterVm()
		{
			var vm = CreatePageDetailRegisterVm(PageDesignUtility.NEW_PAGE_DEFAULT_PAGE_ID);
			return vm;
		}
		/// <summary>
		/// ページ詳細 新規作成 ビューモデル作成
		/// </summary>
		/// <param name="pageId">複製元ページID</param>
		/// <returns>ビューモデル</returns>
		public PageDetailViewModel CreatePageDetailRegisterVm(long pageId)
		{
			var pageModel = (pageId == PageDesignUtility.NEW_PAGE_DEFAULT_PAGE_ID)
				? PageDesignUtility.CreateNewFileModel()
				: new PageDesignService().GetPage(pageId);

			pageModel.PageId = PageDesignUtility.NEW_PAGE_DEFAULT_PAGE_ID;

			// PC コンテンツ読み込み(デフォルトページ)
			var pagePath = (pageId == PageDesignUtility.NEW_PAGE_DEFAULT_PAGE_ID)
				? Constants.PAGE_FRONT_CUSTOMPAGE_TEMPLATE.Replace("/", @"\")
				: Path.Combine(pageModel.FileDirPath, pageModel.FileName);
			var pcFileTextAll = DesignCommon.GetFileTextAll(Path.Combine(DesignCommon.PhysicalDirPathTargetSitePc, pagePath));
			var pcContent = PageDesignUtility.GetEditableAreaForEdit(pcFileTextAll);

			// SP コンテンツ読み込み(デフォルトページ)
			var spFileTextAll = (File.Exists(Path.Combine(DesignCommon.PhysicalDirPathTargetSiteSp, pagePath)))
				? DesignCommon.GetFileTextAll(Path.Combine(DesignCommon.PhysicalDirPathTargetSiteSp, pagePath))
				: DesignCommon.GetFileTextAll(Path.Combine(DesignCommon.PhysicalDirPathTargetSiteSp, Constants.PAGE_FRONT_CUSTOMPAGE_TEMPLATE.Replace("/", @"\")));

			var spContent = PageDesignUtility.GetEditableAreaForEdit(spFileTextAll);

			var pcRealPage = new RealPage(
				pageModel.ManagementTitle,
				DesignCommon.PhysicalDirPathTargetSitePc,
				pageModel.FileDirPath,
				pageModel.FileName,
				pageModel.PageType);

			var spRealPage = new RealPage(
				pageModel.ManagementTitle,
				DesignCommon.PhysicalDirPathTargetSiteSp,
				pageModel.FileDirPath,
				pageModel.FileName,
				pageModel.PageType);

			var input = new PageDesignPageInput(pageModel)
			{
				PcContentInput =
				{
					Content = pcContent,
				},
				SpContentInput =
				{
					Content = spContent,
				}
			};

			if (pageId != PageDesignUtility.NEW_PAGE_DEFAULT_PAGE_ID)
			{
				input.ManagementTitle = pageModel.ManagementTitle + "_Copy";
				input.PcContentInput.PageTitle = input.SpContentInput.PageTitle = pcRealPage.PageTitle + "_Copy";
				input.FileNameNoExtension = input.FileNameNoExtension + "_Copy";
				input.SourcePageId = pageId;
			}

			var vm = new PageDetailViewModel
			{
				PcDirPath = Path.Combine(Constants.PATH_ROOT_FRONT_PC, DesignCommon.CUSTOM_PAGE_DIR_PATH).Replace(@"\", "/"),
				Input = input,
				PcRealPage = pcRealPage,
				SpRealPage = spRealPage
			};

			var allPartsModel = new PartsDesignService().GetAllParts();
			// PCレイアウトパーツセット
			var allRealPartsListPc = PartsDesignCommon.GetCustomPartsList(DesignCommon.DeviceType.Pc);
			allRealPartsListPc.AddRange(PartsDesignCommon.GetStandardPartsList(DesignCommon.DeviceType.Pc));
			PageDesignUtility.SetContentInputLayoutParts(pcFileTextAll, allPartsModel, allRealPartsListPc, vm.LayoutEditViewModelPc.Input);
			var pcLayoutName = PageDesignUtility.GetLayoutName(pcFileTextAll);
			vm.LayoutEditViewModelPc.Input.LayoutType = (string.IsNullOrEmpty(pcLayoutName))
				? LayoutEditInput.LayoutTypeMaster.Default.ToString()
				: pcLayoutName;
			vm.LayoutEditViewModelPc.IsCustomPage = input.IsCustomPage;
			vm.LayoutEditViewModelPc.UseDefaultMaster = PageDesignUtility.CheckUseDefultMasterFile(pcFileTextAll);

			// SPレイアウトパーツセット
			var allRealPartsListSp = PartsDesignCommon.GetCustomPartsList(DesignCommon.DeviceType.Sp);
			allRealPartsListSp.AddRange(PartsDesignCommon.GetStandardPartsList(DesignCommon.DeviceType.Sp));
			PageDesignUtility.SetContentInputLayoutParts(spFileTextAll, allPartsModel, allRealPartsListSp, vm.LayoutEditViewModelSp.Input);
			var spLayoutName = PageDesignUtility.GetLayoutName(spFileTextAll);
			vm.LayoutEditViewModelSp.Input.LayoutType = (string.IsNullOrEmpty(spLayoutName))
				? LayoutEditInput.LayoutTypeMaster.Default.ToString()
				: spLayoutName;
			vm.LayoutEditViewModelSp.IsCustomPage = input.IsCustomPage;
			vm.LayoutEditViewModelSp.UseDefaultMaster = PageDesignUtility.CheckUseDefultMasterFile(spFileTextAll);

			return vm;
		}

		/// <summary>
		/// ページ詳細 ビューモデル作成
		/// </summary>
		/// <param name="pageId">ページID nullの場合は新規作成</param>
		/// <param name="buckUpFileName">バックアップするファイル名(バックアップから復元する場合に指定)</param>
		/// <returns>ビューモデル</returns>
		public PageDetailViewModel CreatePageDetailVm(long pageId, string buckUpFileName = "")
		{
			var pageModel = new PageDesignService().GetPage(pageId);

			var pcRealPage = new RealPage();
			switch (pageModel.PageType)
			{
				case Constants.FLG_PAGEDESIGN_PAGE_TYPE_CUSTOM:
					var pcCustomPageList = PageDesignCommon.GetCustomPageList(DesignCommon.DeviceType.Pc, pageModel.FileName);
					pcRealPage = ((pcCustomPageList.Count == 1)
						? pcCustomPageList[0]
						: new RealPage(
							pageModel.ManagementTitle,
							DesignCommon.PhysicalDirPathTargetSitePc,
							pageModel.FileDirPath,
							pageModel.FileName,
							pageModel.PageType));
					break;

				case Constants.FLG_PAGEDESIGN_PAGE_TYPE_HTML:
					var pcHtmlPageList = PageDesignCommon.GetHtmlPageList(DesignCommon.DeviceType.Pc, pageModel.FileName);
					pcRealPage = ((pcHtmlPageList.Count == 1)
						? pcHtmlPageList[0]
						: new RealPage(
							pageModel.ManagementTitle,
							DesignCommon.PhysicalDirPathTargetSitePc,
							pageModel.FileDirPath,
							pageModel.FileName,
							pageModel.PageType));
					break;

				case Constants.FLG_PAGEDESIGN_PAGE_TYPE_NORMAL:
					pcRealPage = new RealPage(
						pageModel.ManagementTitle,
						DesignCommon.PhysicalDirPathTargetSitePc,
						pageModel.FileDirPath,
						pageModel.FileName,
						pageModel.PageType);
					break;
			}

			var defaultCustomPageTemplatePath = Constants.PAGE_FRONT_CUSTOMPAGE_TEMPLATE.Replace("/", @"\");

			// PCファイルが存在しない場合は新規作成
			if (pcRealPage.Existence == RealPage.ExistStatus.NotExist
				&& (pcRealPage.PageType != Constants.FLG_PAGEDESIGN_PAGE_TYPE_HTML))
			{
				DesignCommon.FileCopy(
					Path.Combine(DesignCommon.PhysicalDirPathTargetSitePc, defaultCustomPageTemplatePath),
					pcRealPage.PhysicalFullPath);

				PageTitleEdit(pcRealPage.PhysicalFullPath, pageModel.ManagementTitle);

				DesignUtility.UpdateLastChanged(pcRealPage.PhysicalFullPath, this.SessionWrapper.LoginOperatorName);
				pcRealPage.LastChange = this.SessionWrapper.LoginOperatorName;
			}

			// PC コンテンツ読み込み
			var pcFilePath = string.IsNullOrEmpty(buckUpFileName)
				? pcRealPage.PhysicalFullPath
				: Path.Combine(
					RestoreUtility.GetBuckUpDirectoryPath(
						pcRealPage.PhysicalFullPath,
						DesignCommon.DeviceType.Pc,
						RestoreType.Page),
					buckUpFileName);
			var pcFileTextAll = DesignCommon.GetFileTextAll(pcFilePath);
			var pcContent = (pcRealPage.PageType == Constants.FLG_PAGEDESIGN_PAGE_TYPE_HTML)
				? new Dictionary<string, string> { { "全文", pcFileTextAll } }
				: PageDesignUtility.GetEditableAreaForEdit(pcFileTextAll);
			pcRealPage.PageTitle = DesignCommon.GetAspxTitle(pcFileTextAll);
			pcRealPage.LastChange = DesignCommon.GetLastChanged(pcFileTextAll);

			var spRealPage = new RealPage();
			switch (pageModel.PageType)
			{
				case Constants.FLG_PAGEDESIGN_PAGE_TYPE_CUSTOM:
					var spCustomPageList = PageDesignCommon.GetCustomPageList(DesignCommon.DeviceType.Sp, pageModel.FileName);
					if (spCustomPageList.Count == 1)
					{
						spRealPage = spCustomPageList[0];
					}
					else
					{
						spRealPage = pcRealPage;
						spRealPage.Existence = RealPage.ExistStatus.NotExist;
					}
					break;

				case Constants.FLG_PAGEDESIGN_PAGE_TYPE_HTML:
					spRealPage = new RealPage(
						pageModel.ManagementTitle,
						DesignCommon.PhysicalDirPathTargetSiteSp,
						pageModel.FileDirPath,
						pageModel.FileName,
						pageModel.PageType);
					break;

				case Constants.FLG_PAGEDESIGN_PAGE_TYPE_NORMAL:
					spRealPage = new RealPage(
						pageModel.ManagementTitle,
						DesignCommon.PhysicalDirPathTargetSiteSp,
						pageModel.FileDirPath,
						pageModel.FileName,
						pageModel.PageType);
					if (spRealPage.Existence == RealPage.ExistStatus.NotExist)
					{
						spRealPage = pcRealPage;
						spRealPage.Existence = RealPage.ExistStatus.NotExist;
					}
					break;
			}

			var input = new PageDesignPageInput(pageModel)
			{
				PcContentInput =
				{
					Content = pcContent,
					PageTitle = pcRealPage.PageTitle,
				}
			};

			var vm = new PageDetailViewModel
			{
				PcDirPath = Path.Combine(Constants.PATH_ROOT_FRONT_PC, pcRealPage.PageDirPath).Replace(@"\", "/"),
				Input = input,
				PcRealPage = pcRealPage
			};

			if (spRealPage.PageType != Constants.FLG_PAGEDESIGN_PAGE_TYPE_HTML)
			{
				// SP コンテンツ読み込み
				var spFilePath = string.IsNullOrEmpty(buckUpFileName)
					? spRealPage.PhysicalFullPath
					: Path.Combine(
						RestoreUtility.GetBuckUpDirectoryPath(
							spRealPage.PhysicalFullPath,
							DesignCommon.DeviceType.Sp,
							RestoreType.Page),
						buckUpFileName);
				var spFileTextAll = DesignCommon.GetFileTextAll(spFilePath);
				var spContent = PageDesignUtility.GetEditableAreaForEdit(spFileTextAll);
				spRealPage.PageTitle = DesignCommon.GetAspxTitle(spFileTextAll);
				spRealPage.LastChange = DesignCommon.GetLastChanged(spFileTextAll);

				input = new PageDesignPageInput(pageModel)
				{
					PcContentInput =
					{
						Content = pcContent,
						PageTitle = pcRealPage.PageTitle,
					},
					SpContentInput =
					{
						Content = spContent,
						PageTitle = spRealPage.PageTitle,
						NoUse = (pageModel.UseType == Constants.FLG_PAGEDESIGN_USE_TYPE_PC)
							|| (spRealPage.Existence == RealPage.ExistStatus.NotExist),
					}
				};

				vm = new PageDetailViewModel
				{
					PcDirPath = Path.Combine(Constants.PATH_ROOT_FRONT_PC, pcRealPage.PageDirPath).Replace(@"\", "/"),
					Input = input,
					PcRealPage = pcRealPage,
					SpRealPage = spRealPage
				};

				var allPartsModel = new PartsDesignService().GetAllParts();
				// PCレイアウトパーツセット
				var allRealPartsListPc = PartsDesignCommon.GetCustomPartsList(DesignCommon.DeviceType.Pc);
				allRealPartsListPc.AddRange(PartsDesignCommon.GetStandardPartsList(DesignCommon.DeviceType.Pc));
				PageDesignUtility.SetContentInputLayoutParts(pcFileTextAll, allPartsModel, allRealPartsListPc, vm.LayoutEditViewModelPc.Input);
				var pcLayoutName = PageDesignUtility.GetLayoutName(pcFileTextAll);
				vm.LayoutEditViewModelPc.Input.LayoutType = (string.IsNullOrEmpty(pcLayoutName))
					? LayoutEditInput.LayoutTypeMaster.Default.ToString()
					: pcLayoutName;
				vm.LayoutEditViewModelPc.IsCustomPage = input.IsCustomPage;
				vm.LayoutEditViewModelPc.UseDefaultMaster = PageDesignUtility.CheckUseDefultMasterFile(pcFileTextAll);

				// SPレイアウトパーツセット
				var allRealPartsListSp = PartsDesignCommon.GetCustomPartsList(DesignCommon.DeviceType.Sp);
				allRealPartsListSp.AddRange(PartsDesignCommon.GetStandardPartsList(DesignCommon.DeviceType.Sp));
				PageDesignUtility.SetContentInputLayoutParts(spFileTextAll, allPartsModel, allRealPartsListSp, vm.LayoutEditViewModelSp.Input);
				var spLayoutName = PageDesignUtility.GetLayoutName(spFileTextAll);
				vm.LayoutEditViewModelSp.Input.LayoutType = (string.IsNullOrEmpty(spLayoutName))
					? LayoutEditInput.LayoutTypeMaster.Default.ToString()
					: spLayoutName;
				vm.LayoutEditViewModelSp.IsCustomPage = input.IsCustomPage;
				vm.LayoutEditViewModelSp.UseDefaultMaster = PageDesignUtility.CheckUseDefultMasterFile(spFileTextAll);

				UpdateFileOpenTime(pcFilePath);
				UpdateFileOpenTime(spFilePath);
			}

			return vm;
		}

		/// <summary>
		/// ページ選択によるグループ移動 ビューモデル作成
		/// </summary>
		/// <param name="pageId">グループID</param>
		/// <returns>ビューモデル</returns>
		public GroupMoveViewModel CreateGroupMoveViewModel(long pageId)
		{
			var pageModel = new PageDesignService().GetPage(pageId);
			var otherModel = PageDesignUtility.OtherPageGroupModel;
			var groupModel = (pageModel.GroupId == otherModel.GroupId)
				? otherModel
				: new PageDesignService().GetGroup(pageModel.GroupId);

			var viewModel = new GroupMoveViewModel
			{
				SelectGroupModel = groupModel,
				PageId = pageId
			};

			return viewModel;
		}

		/// <summary>
		/// グループ追加
		/// </summary>
		/// <param name="name">グループ名</param>
		/// <returns>グループID</returns>
		public int GroupAdd(string name)
		{
			var model = new PageDesignGroupModel
			{
				GroupName = name,
				LastChanged = this.SessionWrapper.LoginOperatorName
			};
			var id = new PageDesignService().InsertGroup(model);
			return id;
		}

		/// <summary>
		/// グループ名変更
		/// </summary>
		/// <param name="pageId">グループID</param>
		/// <param name="name">グループ名</param>
		public void GroupNameEdit(long pageId, string name)
		{
			var model = new PageDesignService().GetGroup(pageId);
			if (model == null) return;

			model.GroupName = name;
			model.LastChanged = this.SessionWrapper.LoginOperatorName;

			new PageDesignService().UpdateGroup(model);
		}

		/// <summary>
		/// グループ削除
		/// </summary>
		/// <param name="groupId">グループID</param>
		public void GroupDelete(long groupId)
		{
			new PageDesignService().DeleteGroup(groupId, this.SessionWrapper.LoginOperatorName);
		}

		/// <summary>
		/// グループ順更新
		/// </summary>
		/// <param name="groupIds">グループ順</param>
		public void GroupSortUpdate(long[] groupIds)
		{
			new PageDesignService().UpdateGroupSort(groupIds);
		}

		/// <summary>
		/// ページ選択によるグループ移動
		/// </summary>
		/// <param name="groupId">グループID</param>
		/// <param name="pageId">ページID</param>
		public void GroupMoveEdit(long groupId, long pageId)
		{
			var model = new PageDesignService().GetPage(pageId);
			model.GroupId = groupId;
			model.PageSortNumber = 0;
			model.LastChanged = this.SessionWrapper.LoginOperatorName;
			new PageDesignService().UpdatePage(model);
		}

		/// <summary>
		/// ページ順序更新
		/// </summary>
		/// <param name="groupId">グループID</param>
		/// <param name="pageIds">ページ順序</param>
		public void PageSortUpdate(long groupId, long[] pageIds)
		{
			if (pageIds == null) return;

			var models = pageIds.Select(
				(p, index) => new PageDesignModel
				{
					GroupId = groupId,
					PageId = p,
					PageSortNumber = index + 1,
					LastChanged = this.SessionWrapper.LoginOperatorName
				}).ToArray();
			new PageDesignService().UpdatePageSort(models);
		}

		/// <summary>
		/// 管理用タイトル更新
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <param name="title">管理用タイトル</param>
		/// <returns>エラーメッセージ</returns>
		public void ManagementTitleEdit(long pageId, string title)
		{
			new PageDesignService().UpdateManagementTitle(pageId, title);
		}

		/// <summary>
		/// ページタイトル更新
		/// </summary>
		/// <param name="pageFullPath">ページパス</param>
		/// <param name="title">タイトル</param>
		/// <returns>エラーメッセージ</returns>
		private string PageTitleEdit(string pageFullPath, string title)
		{
			if (File.Exists(pageFullPath) == false) return WebMessages.FileUnFindError.Replace("@@ 1 @@", Path.GetFileName(pageFullPath));

			var textAll = new StringBuilder(DesignCommon.GetFileTextAll(pageFullPath));

			DesignCommon.ReplaceTitle(textAll, title);

			DesignCommon.ReplaceLastChanged(textAll, this.SessionWrapper.LoginOperatorName);

			var errorMessage = DesignUtility.UpdateFile(pageFullPath, textAll.ToString());
			return errorMessage;
		}

		/// <summary>
		/// ページ詳細更新 ページペアリング単位
		/// </summary>
		/// <param name="input">ページ詳細 入力内容</param>
		/// <returns>エラーメッセージ</returns>
		public string UpdatePage(PageDesignPageInput input)
		{
			// 未利用の場合は入力チェックをスキップさせる
			if (input.PcContentInput.NoUse) input.PcContentInput.PageTitle = null;
			if ((input.SpContentInput.NoUse) || DesignCommon.UseResponsive) input.SpContentInput.PageTitle = null;

			var errorMessage = input.Validate();

			if (string.IsNullOrEmpty(errorMessage) == false) return errorMessage;

			// ページコンパイルチェック
			var pageErrorMessage = "";
			if (input.IsRegister)
			{
				pageErrorMessage = WebRequestCheck.Send(Preview(input, DesignCommon.DeviceType.Pc));
				if (input.IsHtmlPage == false)
				{
					pageErrorMessage += WebRequestCheck.Send(Preview(input, DesignCommon.DeviceType.Sp));
				}
			}
			else
			{
				pageErrorMessage = WebRequestCheck.Send(
					Preview(input,
						(input.IsPc
							? DesignCommon.DeviceType.Pc
							: DesignCommon.DeviceType.Sp)));
			}

			PageDesignUtility.DeletePreviewFile();

			if (string.IsNullOrEmpty(pageErrorMessage) == false)
			{
				return pageErrorMessage;
			}

			if (input.IsRegister)
			{
				var newFileName = input.FileNameNoExtension + DesignCommon.PAGE_FILE_EXTENSION;

				var outputPcFilePath = Path.Combine(
					DesignCommon.PhysicalDirPathTargetSitePc,
					DesignCommon.CUSTOM_PAGE_DIR_PATH,
					newFileName);

				var outputSpFilePath = Path.Combine(
					DesignCommon.PhysicalDirPathTargetSiteSp,
					DesignCommon.CUSTOM_PAGE_DIR_PATH,
					newFileName);

				if (File.Exists(outputPcFilePath) || File.Exists(outputSpFilePath))
				{
					return WebMessages.PageDesignFileNameDuplicateError;
				}

				var sourceFileName = (input.SourcePageId != null)
					? new PageDesignService().GetPage(input.SourcePageId.Value).FileName
					: string.Empty;
				var newPageModel = PageDesignUtility.CreateNewFile(out errorMessage, newFileName, sourceFileName);
				if (string.IsNullOrEmpty(errorMessage) == false)
				{
					return errorMessage;
				}
				input.PageId = newPageModel.PageId;
			}

			var pageDesignService = new PageDesignService();
			var beforeModel = pageDesignService.GetPage(input.PageId);
			var newModel = input.CreateModel();

			// グループに更新がある場合は先頭に移動
			if (beforeModel.GroupId != newModel.GroupId)
			{
				newModel.PageSortNumber = 0;
			}

			// 初回作成時はバックアップを取らない
			if (string.IsNullOrEmpty(beforeModel.LastChanged) == false)
			{
				// バックアップを作成
				var sourcePcFilePath = Path.Combine(
					DesignCommon.PhysicalDirPathTargetSitePc,
					beforeModel.FileDirPath,
					beforeModel.FileName);
				var sourceSpFilePath = Path.Combine(
					DesignCommon.PhysicalDirPathTargetSiteSp,
					beforeModel.FileDirPath,
					beforeModel.FileName);

				var bkFileName = File.GetLastWriteTime(sourcePcFilePath).ToString("yyyyMMdd_HHmmss") + "_" + Path.GetFileName(sourcePcFilePath) + ".bk";
				RestoreUtility.CreateBuckUpFile(sourcePcFilePath, bkFileName, DesignCommon.DeviceType.Pc, RestoreType.Page);
				RestoreUtility.CreateBuckUpFile(sourceSpFilePath, bkFileName, DesignCommon.DeviceType.Sp, RestoreType.Page);
			}

			if (input.IsRegister)
			{
				var pcUpdateErrorMessage = UpdatePage(input, input.PcContentInput, DesignCommon.PhysicalDirPathTargetSitePc, beforeModel, DesignCommon.DeviceType.Pc);
				if (string.IsNullOrEmpty(pcUpdateErrorMessage) == false)
				{
					errorMessage += ValueText.GetValueText(
						Constants.VALUE_TEXT_KEY_CMS_COMMON,
						Constants.VALUE_TEXT_FIELD_PAGE_DESIGN,
						"PC") + " : " + pcUpdateErrorMessage;
				}

				if ((DesignCommon.UseResponsive == false) && (input.IsHtmlPage == false))
				{
					var spUpdateErrorMessage = UpdatePage(input, input.SpContentInput, DesignCommon.PhysicalDirPathTargetSiteSp, beforeModel, DesignCommon.DeviceType.Sp);
					if (string.IsNullOrEmpty(spUpdateErrorMessage) == false)
					{
						errorMessage += ValueText.GetValueText(
							Constants.VALUE_TEXT_KEY_CMS_COMMON,
							Constants.VALUE_TEXT_FIELD_PAGE_DESIGN,
							"SP") + " : " + spUpdateErrorMessage;
					}
				}
			}
			else
			{
				var updateErrorMessage = UpdatePage(
					input,
					(input.IsPc ? input.PcContentInput : input.SpContentInput),
					(input.IsPc ? DesignCommon.PhysicalDirPathTargetSitePc : DesignCommon.PhysicalDirPathTargetSiteSp),
					beforeModel,
					(input.IsPc ? DesignCommon.DeviceType.Pc : DesignCommon.DeviceType.Sp));
				if (string.IsNullOrEmpty(updateErrorMessage) == false)
				{
					errorMessage += ValueText.GetValueText(
						Constants.VALUE_TEXT_KEY_CMS_COMMON,
						Constants.VALUE_TEXT_FIELD_PAGE_DESIGN,
					(input.IsPc ? "PC" : "SP")) + " : " + updateErrorMessage;
				}

				// ページ名変更に合わせて、変更対象外のページも名前だけは変更しておく
				if ((beforeModel.FileName != newModel.FileName)
					&& (input.IsNormalPage == false)
					&& string.IsNullOrEmpty(updateErrorMessage)
					&& (DesignCommon.PhysicalDirPathTargetSiteSp != DesignCommon.PhysicalDirPathTargetSitePc))
				{
					var replaceErrorMessage = MovePage(
						input,
						(input.IsPc ? DesignCommon.PhysicalDirPathTargetSiteSp : DesignCommon.PhysicalDirPathTargetSitePc),
						beforeModel,
						(input.IsPc ? DesignCommon.DeviceType.Sp : DesignCommon.DeviceType.Pc));
					if (string.IsNullOrEmpty(replaceErrorMessage) == false)
					{
						var pc = ValueText.GetValueText(
							Constants.VALUE_TEXT_KEY_CMS_COMMON,
							Constants.VALUE_TEXT_FIELD_PAGE_DESIGN,
							"PC");
						var sp = ValueText.GetValueText(
							Constants.VALUE_TEXT_KEY_CMS_COMMON,
							Constants.VALUE_TEXT_FIELD_PAGE_DESIGN,
							"SP");
						var replaceHeadError = WebMessages.PageDesignMoveFileError
							.Replace("@@ 1 @@", (input.IsPc ? pc : sp))
							.Replace("@@ 2 @@", (input.IsPc ? sp : pc));
						errorMessage += replaceHeadError + replaceErrorMessage;
					}
				}
			}

			if (string.IsNullOrEmpty(errorMessage))
			{
				newModel.LastChanged = this.SessionWrapper.LoginOperatorName;
				try
				{
					pageDesignService.UpdatePage(newModel);
				}
				catch (Exception ex)
				{
					errorMessage = WebMessages.DataBaseError;
				}

				// フロントのキャッシュ更新
				RefreshFileManagerProvider.GetInstance(RefreshFileType.PageDesign).CreateUpdateRefreshFile();

				var webUrlParams = new List<KeyValuePair<string, string>>();
				// コーディネート詳細であればパラメタ付加
				if (input.FileNameNoExtension.Contains("CoordinateDetail"))
				{
					var model = new CoordinateService().GetCoordinateTopForPreview();
					if (model != null)
					{
						webUrlParams.Add(new KeyValuePair<string, string>(Constants.REQUEST_KEY_COORDINATE_ID, HttpUtility.UrlEncode(model.CoordinateId)));
					}
				}

				var filePath = input.IsPc
					? Path.Combine(
						beforeModel.FileDirPath,
						(input.FileNameNoExtension + DesignCommon.PAGE_FILE_EXTENSION))
					: Path.Combine(
						DesignCommon.SiteSpRootPath,
						beforeModel.FileDirPath,
						(input.FileNameNoExtension + DesignCommon.PAGE_FILE_EXTENSION));

				WebBrowserCapture.Create(
					Constants.PHYSICALDIRPATH_CMS_MANAGER,
					filePath,
					webUrlParams,
					device: (input.IsPc ? WebBrowserCapture.Device.Pc : WebBrowserCapture.Device.Sp),
					delay: 100,
					iSizeH: (input.IsPc ? 800 : 400),
					iSizeW: (input.IsPc ? 800 : 400),
					bSizeH: (input.IsPc ? 1280 : 400),
					bSizeW: (input.IsPc ? 720 : 800));

			}

			return errorMessage;
		}
		/// <summary>
		/// ページ詳細更新 ページ単位
		/// </summary>
		/// <param name="input">ページ詳細 入力内容</param>
		/// <param name="contentInput">コンテンツ内容</param>
		/// <param name="physicalDirPathTargetSite">ページ物理パス</param>
		/// <param name="beforeModel">更新前ページモデル</param>
		/// <param name="deviceType">デバイスタイプ</param>
		/// <returns>エラーメッセージ</returns>
		private string UpdatePage(
			PageDesignPageInput input,
			PageDesignContentInput contentInput,
			string physicalDirPathTargetSite,
			PageDesignModel beforeModel,
			DesignCommon.DeviceType deviceType)
		{
			var sourceFilePath = Path.Combine(
				physicalDirPathTargetSite,
				beforeModel.FileDirPath,
				beforeModel.FileName);

			var outputFileName = input.FileNameNoExtension;
			outputFileName += (input.PageType == Constants.FLG_PAGEDESIGN_PAGE_TYPE_HTML)
				? DesignCommon.HTML_FILE_EXTENSION
				: DesignCommon.PAGE_FILE_EXTENSION;
			var outputFilePath = Path.Combine(
				physicalDirPathTargetSite,
				beforeModel.FileDirPath,
				outputFileName);

			var webUrl = PageDesignUtility.WebTargetPageUrl(
				Path.Combine(DesignCommon.SiteSpRootPath, beforeModel.FileDirPath),
				outputFileName,
				true);

			// ファイル名変更があり、変更後ファイル名が既に存在する場合はエラー
			if ((sourceFilePath != outputFilePath)
				&& File.Exists(outputFilePath))
			{
				return WebMessages.PageDesignFileNameDuplicateError;
			}

			var errorMessage = UpdatePage(input, contentInput, sourceFilePath, outputFilePath, webUrl, deviceType, beforeModel);

			// 更新成功後、ファイル名が異なる場合は元ファイルを削除
			if ((sourceFilePath != outputFilePath)
				&& File.Exists(outputFilePath)
				&& (string.IsNullOrEmpty(errorMessage)))
			{
				errorMessage = DesignUtility.DeleteFile(sourceFilePath);
			}

			return errorMessage;
		}
		/// <summary>
		/// ページ詳細更新 ページ更新のみ
		/// </summary>
		/// <param name="input">ページ詳細 入力内容</param>
		/// <param name="contentInput">コンテンツ内容</param>
		/// <param name="inputFilePath">元ページ物理ファイルパス</param>
		/// <param name="outputFilePath">生成ページ物理ファイルパス</param>
		/// <param name="webUrl">WebアクセスURL</param>
		/// <param name="deviceType">デバイスタイプ</param>
		/// <param name="beforeModel">更新前ページモデル</param>
		/// <param name="isPreview">プレビューか</param>
		/// <returns>エラーメッセージ</returns>
		private string UpdatePage(
			PageDesignPageInput input,
			PageDesignContentInput contentInput,
			string inputFilePath,
			string outputFilePath,
			string webUrl,
			DesignCommon.DeviceType deviceType,
			PageDesignModel beforeModel,
			bool isPreview = false)
		{
			var fileTextAll = new StringBuilder(DesignCommon.GetFileTextAll(inputFilePath));
			var errorMessage = "";

			if ((deviceType != DesignCommon.DeviceType.Pc || input.PcContentInput.NoUse != false)
				&& (deviceType != DesignCommon.DeviceType.Sp || input.SpContentInput.NoUse != false))
				return errorMessage;
			if (input.IsHtmlPage)
			{
				// ファイル書き込み
				errorMessage = DesignUtility.UpdateFile(outputFilePath, contentInput.Content["全文"], isPageRename : (inputFilePath.Equals(outputFilePath) == false));
			}
			//編集可能なレイアウト領域・編集可能領域確認
			else if ((PageDesignUtility.GetLayoutAreaForEdit(fileTextAll.ToString()).Any() == false)
				&& (PageDesignUtility.GetEditableAreaForEdit(fileTextAll.ToString()).Any() == false))
			{
				errorMessage = WebMessages.PageDesignFileEditLayoutRangeSettingError;
			}
			else
			{
				// レイアウト名 置換
				PageDesignUtility.ReplaceLayoutName(fileTextAll, contentInput.LayoutEditInput.LayoutType);

				// コンテンツ 置換
				// コンテンツ内に直書きされているパーツも宣言に含めるためにパーツ配置の前にコンテンツ内容を置換する
				PageDesignUtility.ReplaceContentsTagForUpdate(fileTextAll, contentInput.Content);

				contentInput.LayoutEditInput.SetMovePartsModel(deviceType);

				var allRealPartsList = PartsDesignCommon.GetCustomPartsList(deviceType);
				allRealPartsList.AddRange(PartsDesignCommon.GetStandardPartsList(deviceType));

				// レイアウト置換＆置換されたパーツ数取得
				var replacedPartsCount = PageDesignUtility.ReplaceLayoutForUpdate(
					fileTextAll,
					allRealPartsList,
					deviceType,
					contentInput.LayoutEditInput);

				if (beforeModel != null)
				{
					var beforeSourceFilePath = Path.Combine(
						(deviceType == DesignCommon.DeviceType.Pc)
							? DesignCommon.PhysicalDirPathTargetSitePc
							: DesignCommon.PhysicalDirPathTargetSiteSp,
						beforeModel.FileDirPath,
						beforeModel.FileName);
					var beforeFileTextAll = DesignCommon.GetFileTextAll(beforeSourceFilePath);
					var beforeInput = new PageDesignPageInput(beforeModel);
					if (deviceType == DesignCommon.DeviceType.Pc)
					{
						beforeInput.PcContentInput.Content = PageDesignUtility.GetEditableAreaForEdit(beforeFileTextAll);
						beforeInput.PcContentInput.PageTitle = DesignCommon.GetAspxTitle(beforeFileTextAll);
					}
					else
					{
						beforeInput.SpContentInput.Content = PageDesignUtility.GetEditableAreaForEdit(beforeFileTextAll);
						beforeInput.SpContentInput.PageTitle = DesignCommon.GetAspxTitle(beforeFileTextAll);
					}

					// 全レイアウト領域に配置されたパーツのカウント（置換前）
					var layoutPartsCountBeforeReplace = contentInput.LayoutEditInput.MovePartsLeft.Length
						+ contentInput.LayoutEditInput.MovePartsCenterTop.Length
						+ contentInput.LayoutEditInput.MovePartsCenterBottom.Length
						+ contentInput.LayoutEditInput.MovePartsRight.Length;

					// 置換前と置換後のパーツ数を比べて数が同じでないかつ、レイアウト編集以外に変更がなければ、
					// 編集不可のレイアウト領域にパーツが配置されているエラーを出す
					if ((replacedPartsCount != layoutPartsCountBeforeReplace)
						&& CheckSameInputValueExceptLayoutEdit(input, beforeInput, deviceType))
					{
						errorMessage = WebMessages.PageDesignFileEditLayoutRangeUpdateError;
					}
				}

				// 最終更新者 置換
				DesignCommon.ReplaceLastChanged(fileTextAll, this.SessionWrapper.LoginOperatorName);

				if (input.IsCustomPage)
				{
					// タイトル置換
					DesignCommon.ReplaceTitle(fileTextAll, contentInput.PageTitle);

					if (PageDesignUtility.CheckUseDefultMasterFile(fileTextAll.ToString()))
					{
						// レイアウトによるマスターファイルの更新
						var masterFileName =
							(contentInput.LayoutEditInput.LayoutType == LayoutEditInput.LayoutTypeMaster.Simple.ToString())
								? Path.GetFileNameWithoutExtension(Constants.PAGE_FRONT_SIMPLE_DEFAULT_MASTER)
								: Path.GetFileNameWithoutExtension(Constants.PAGE_FRONT_DEFAULT_MASTER);

						PageDesignUtility.ReplaceMasterFile(fileTextAll, masterFileName);
					}
				}

				if (isPreview)
				{
					// レイアウトによるマスターファイルの更新
					var sourceMasterFile = PageDesignUtility.GetMasterFileName(fileTextAll.ToString());

					string masterFileName;

					if (sourceMasterFile == "OrderPage")
					{
						masterFileName = Path.GetFileNameWithoutExtension(Constants.PAGE_FRONT_ORDER_PREVIEW);
					}
					else if (sourceMasterFile == "LandingOrderPage")
					{
						masterFileName =
							Path.GetFileNameWithoutExtension(Constants.PAGE_FRONT_LANDING_ORDER_PREVIEW_MASTER);
					}
					else
					{
						masterFileName =
							(contentInput.LayoutEditInput.LayoutType == LayoutEditInput.LayoutTypeMaster.Simple.ToString())
								? Path.GetFileNameWithoutExtension(Constants.PAGE_FRONT_SIMPLE_DEFAULT_PREVIEW_MASTER)
								: Path.GetFileNameWithoutExtension(Constants.PAGE_FRONT_DEFAULT_PREVIEW_MASTER);
					}

					PageDesignUtility.ReplaceMasterFile(fileTextAll, masterFileName);
				}

				// ファイル書き込み
				errorMessage += DesignUtility.UpdateFile(outputFilePath, fileTextAll.ToString(), true, isPageRename: (inputFilePath.Equals(outputFilePath) == false));
			}

			// Webリクエストチェック
			errorMessage += WebRequestCheck.Send(webUrl);

			return errorMessage;
		}

		/// <summary>
		/// ページ移動
		/// </summary>
		/// <param name="input">ページ詳細 入力内容</param>
		/// <param name="physicalDirPathTargetSite">ページ物理パス</param>
		/// <param name="beforeModel">更新前ページモデル</param>
		/// <param name="deviceType">デバイスタイプ</param>
		/// <returns>エラーメッセージ</returns>
		private string MovePage(
			PageDesignPageInput input,
			string physicalDirPathTargetSite,
			PageDesignModel beforeModel,
			DesignCommon.DeviceType deviceType)
		{
			var sourceFilePath = Path.Combine(
				physicalDirPathTargetSite,
				beforeModel.FileDirPath,
				beforeModel.FileName);

			var outputFileName = input.FileNameNoExtension;
			outputFileName += (input.PageType == Constants.FLG_PAGEDESIGN_PAGE_TYPE_HTML)
				? DesignCommon.HTML_FILE_EXTENSION
				: DesignCommon.PAGE_FILE_EXTENSION;
			var outputFilePath = Path.Combine(
				physicalDirPathTargetSite,
				beforeModel.FileDirPath,
				outputFileName);

			var webUrl = PageDesignUtility.WebTargetPageUrl(
				Path.Combine(DesignCommon.SiteSpRootPath, beforeModel.FileDirPath),
				outputFileName,
				true);

			// 変更後ファイル名が既に存在する場合はエラー
			if (File.Exists(outputFilePath))
			{
				return WebMessages.PageDesignFileNameDuplicateError;
			}

			var errorMessage = WritePage(input, sourceFilePath, outputFilePath, webUrl, deviceType);

			// 更新成功後、元ファイルを削除
			if (File.Exists(outputFilePath)
				&& (string.IsNullOrEmpty(errorMessage)))
			{
				errorMessage = DesignUtility.DeleteFile(sourceFilePath);
			}

			return errorMessage;
		}

		/// <summary>
		/// ページの書き込み
		/// </summary>
		/// <param name="input">ページ詳細 入力内容</param>
		/// <param name="inputFilePath">元ページ物理ファイルパス</param>
		/// <param name="outputFilePath">生成ページ物理ファイルパス</param>
		/// <param name="webUrl">WebアクセスURL</param>
		/// <param name="deviceType">デバイスタイプ</param>
		/// <returns>エラーメッセージ</returns>
		private string WritePage(
			PageDesignPageInput input,
			string inputFilePath,
			string outputFilePath,
			string webUrl,
			DesignCommon.DeviceType deviceType)
		{
			var fileTextAll = new StringBuilder(DesignCommon.GetFileTextAll(inputFilePath));
			var errorMessage = "";

			if ((deviceType != DesignCommon.DeviceType.Pc || input.PcContentInput.NoUse != false)
				&& (deviceType != DesignCommon.DeviceType.Sp || input.SpContentInput.NoUse != false))
				return errorMessage;

			//編集可能なレイアウト領域・編集可能領域確認
			if ((PageDesignUtility.GetLayoutAreaForEdit(fileTextAll.ToString()).Any() == false)
				&& (PageDesignUtility.GetEditableAreaForEdit(fileTextAll.ToString()).Any() == false))
			{
				errorMessage = WebMessages.PageDesignFileEditLayoutRangeSettingError;
			}
			else
			{
				// ファイル書き込み
				errorMessage = DesignUtility.UpdateFile(outputFilePath, fileTextAll.ToString(), true, isPageRename: (inputFilePath.Equals(outputFilePath) == false));
			}

			// Webリクエストチェック
			errorMessage += WebRequestCheck.Send(webUrl);

			return errorMessage;
		}

		/// <summary>
		/// ページ削除
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <returns>エラーメッセージ</returns>
		public string DeletePage(long pageId)
		{
			var errorMessage = PageDesignUtility.DeletePage(pageId);
			return errorMessage;
		}

		/// <summary>
		/// プレビュー表示
		/// </summary>
		/// <param name="input">ページ詳細 入力内容</param>
		/// <param name="deviceType">デバイスタイプ</param>
		/// <returns>プレビューURL</returns>
		public string Preview(PageDesignPageInput input, DesignCommon.DeviceType deviceType)
		{
			var physicalDirPathTargetSite = PageDesignUtility.GetPhysicalDirPathTargetSite(deviceType);
			var previewFileName = input.FileNameNoExtension + PageDesignUtility.PREVIEW_PAGE_FILE_EXTENSION;

			var sourceFilePath = "";

			if (input.PageId == PageDesignUtility.NEW_PAGE_DEFAULT_PAGE_ID)
			{
				sourceFilePath = Path.Combine(
					physicalDirPathTargetSite,
					Constants.PAGE_FRONT_CUSTOMPAGE_TEMPLATE);
			}
			else
			{
				var model = new PageDesignService().GetPage(input.PageId);
				sourceFilePath =
					File.Exists(Path.Combine(physicalDirPathTargetSite, model.FileDirPath, model.FileName))
						? Path.Combine(physicalDirPathTargetSite, model.FileDirPath, model.FileName)
						: Path.Combine(
							PageDesignUtility.GetPhysicalDirPathTargetSite(DesignCommon.DeviceType.Pc),
							model.FileDirPath,
							model.FileName);
			}

			var outputFilePath = Path.Combine(
				physicalDirPathTargetSite,
				PageDesignUtility.PREVIEW_DIR_PATH,
				previewFileName);

			var webUrlDirPath = (deviceType == DesignCommon.DeviceType.Pc)
				? PageDesignUtility.PREVIEW_DIR_PATH
				: Path.Combine(DesignCommon.SiteSpRootPath, PageDesignUtility.PREVIEW_DIR_PATH);
			var webUrl = PageDesignUtility.WebTargetPageUrl(
				webUrlDirPath,
				previewFileName,
				true);

			var contentInput = (deviceType == DesignCommon.DeviceType.Pc)
				? input.PcContentInput
				: input.SpContentInput;
			UpdatePage(input, contentInput, sourceFilePath, outputFilePath, webUrl, deviceType, null, true);

			return webUrl;
		}

		/// <summary>
		/// ページ自動バックアップリスト取得
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <returns>ページバックアップリスト</returns>
		public AutoBackedUpDesignFileViewModel GetPageBackupList(long pageId)
		{
			var autoBackedUpDesignFileViewModel = new AutoBackedUpDesignFileViewModel(pageId, RestoreType.Page);
			return autoBackedUpDesignFileViewModel;
		}

		/// <summary>
		/// ページ・パーツダウンロード 全対象
		/// </summary>
		/// <param name="response">レスポンス</param>
		public void Download(HttpResponseBase response)
		{
			PageDesignUtility.Download(response);
		}
		/// <summary>
		/// ページ・パーツダウンロード デバイス指定
		/// </summary>
		/// <param name="response">レスポンス</param>
		/// <param name="deviceType">デバイスタイプ</param>
		public void Download(HttpResponseBase response, DesignCommon.DeviceType deviceType)
		{
			PageDesignUtility.Download(response, deviceType);
		}

		/// <summary>
		/// 整合性バッチの起動
		/// </summary>
		public void PageDesignConsistencyAction()
		{
			Process.Start(Constants.PHYSICALDIRPATH_PAGEDESIGN_CONSISTENCY_EXE);
		}

		/// <summary>
		/// レイアウト編集以外の入力値に変更がないかチェック
		/// </summary>
		/// <param name="input">入力値</param>
		/// <param name="beforeInput">編集前の入力値</param>
		/// <param name="deviceType">デバイスタイプ</param>
		/// <returns>結果</returns>
		private bool CheckSameInputValueExceptLayoutEdit(
			PageDesignPageInput input,
			PageDesignPageInput beforeInput,
			DesignCommon.DeviceType deviceType)
		{
			if (input.ManagementTitle != beforeInput.ManagementTitle) return false;
			if (input.Publish != beforeInput.Publish) return false;
			if (input.GroupId != beforeInput.GroupId) return false;
			if (input.FileNameNoExtension != beforeInput.FileNameNoExtension) return false;
			if (input.MetadataDesc != beforeInput.MetadataDesc) return false;
			if (input.ReleaseRangeSettingInput.ConditionMemberOnlyType
				!= beforeInput.ReleaseRangeSettingInput.ConditionMemberOnlyType)
			{
				return false;
			}

			if (input.ReleaseRangeSettingInput.ConditionMemberRankId
				!= beforeInput.ReleaseRangeSettingInput.ConditionMemberRankId)
			{
				return false;
			}

			if (input.ReleaseRangeSettingInput.ConditionPublishDateFromDate
				!= beforeInput.ReleaseRangeSettingInput.ConditionPublishDateFromDate)
			{
				return false;
			}

			if (input.ReleaseRangeSettingInput.ConditionPublishDateFromTime
				!= beforeInput.ReleaseRangeSettingInput.ConditionPublishDateFromTime)
			{
				return false;
			}

			if (input.ReleaseRangeSettingInput.ConditionPublishDateToDate
				!= beforeInput.ReleaseRangeSettingInput.ConditionPublishDateToDate)
			{
				return false;
			}

			if (input.ReleaseRangeSettingInput.ConditionPublishDateToTime
				!= beforeInput.ReleaseRangeSettingInput.ConditionPublishDateToTime)
			{
				return false;
			}

			if (input.ReleaseRangeSettingInput.ConditionTargetListIds
				!= beforeInput.ReleaseRangeSettingInput.ConditionTargetListIds)
			{
				return false;
			}

			if (input.ReleaseRangeSettingInput.ConditionTargetListType
				!= beforeInput.ReleaseRangeSettingInput.ConditionTargetListType)
			{
				return false;
			}

			if (deviceType == DesignCommon.DeviceType.Pc)
			{
				if (input.PcContentInput.PageTitle.SequenceEqual(beforeInput.PcContentInput.PageTitle) == false)
				{
					return false;
				}

				if (input.PcContentInput.Content.SequenceEqual(beforeInput.PcContentInput.Content) == false)
				{
					return false;
				}
			}
			else
			{
				if (input.SpContentInput.PageTitle.SequenceEqual(beforeInput.SpContentInput.PageTitle) == false)
				{
					return false;
				}

				if (input.SpContentInput.Content.SequenceEqual(beforeInput.SpContentInput.Content) == false)
				{
					return false;
				}
			}

			return true;
		}
	}
}
