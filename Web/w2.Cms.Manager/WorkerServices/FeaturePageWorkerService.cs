/*
=========================================================================================================
  Module      : 特集ページワーカーサービス (FeaturePageWorkerService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
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
using w2.App.Common.RefreshFileManager;
using w2.App.Common.Util;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.Codes.PageDesign;
using w2.Cms.Manager.Controllers;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ParamModels.FeaturePage;
using w2.Cms.Manager.ViewModels.FeaturePage;
using w2.Cms.Manager.ViewModels.Shared;
using w2.Common.Logger;
using w2.Common.Util;
using w2.Domain.ContentsLog;
using w2.Domain.FeatureImage;
using w2.Domain.FeaturePage;
using w2.Domain.PartsDesign;
using w2.Domain.Product;
using w2.Domain.ProductGroup;
using w2.Domain.ProductStock;

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// 特集ページワーカーサービス
	/// </summary>
	public class FeaturePageWorkerService : BaseWorkerService
	{
		/// <summary>特集ページPCルート</summary>
		public static string m_featurePcRoot = Path.Combine(
			DesignCommon.PhysicalDirPathTargetSitePc,
			PageDesignUtility.FEATURE_PAGE_DIR_PATH);
		/// <summary>特集ページSPルート</summary>
		public static string m_featureSpRoot = Path.Combine(
			DesignCommon.PhysicalDirPathTargetSiteSp,
			PageDesignUtility.FEATURE_PAGE_DIR_PATH);
		/// <summary>商品価格で並び替え</summary>
		public const string SORT_PRICE = "1";
		/// <summary>商品IDで並び替え</summary>
		public const string SORT_ID = "2";
		/// <summary>カテゴリIDで並び替え</summary>
		public const string SORT_CATEGORY = "3";
		/// <summary>ブランドIDで並び替え</summary>
		public const string SORT_BRAND = "4";
		/// <summary>商品名で並び替え</summary>
		public const string SORT_NAME = "5";
		/// <summary>表示優先順で並び替え</summary>
		public const string SORT_DISPLAY_PRIORITY = "6";
		/// <summary>販売開始日で並び替え</summary>
		public const string SORT_SELL_FROM = "7";
		/// <summary>1日以内</summary>
		public const string DAY = "1";
		/// <summary>1週間以内</summary>
		public const string WEEK = "2";
		/// <summary>1か月以内</summary>
		public const string MONTH = "3";
		/// <summary>3か月以内</summary>
		public const string THREE_MONTH = "4";
		/// <summary>3か月以降</summary>
		public const string THREE_MONTH_OVER = "5";

		/// <summary>
		/// メインビューモデル作成
		/// </summary>
		/// <returns>メインビューモデル</returns>
		public MainViewModel CreateMainVm()
		{
			if (Directory.Exists(m_featurePcRoot) == false) Directory.CreateDirectory(m_featurePcRoot);
			if (Directory.Exists(m_featureSpRoot) == false) Directory.CreateDirectory(m_featureSpRoot);

			var mainViewModel = new MainViewModel();
			return mainViewModel;
		}

		/// <summary>
		/// リストビューモデル作成
		/// </summary>
		/// <param name="paramModel">検索パラメータモデル</param>
		/// <returns>ビューモデル</returns>
		public ListViewModel CreateListVm(FeaturePageListSearchParamModel paramModel)
		{
			var viewModel = new ListViewModel
			{
				PcRealPageList = PageDesignUtility.GetFeaturePageList(DesignCommon.DeviceType.Pc),
				SpRealPageList = DesignCommon.UseSmartPhone
					? PageDesignUtility.GetFeaturePageList(DesignCommon.DeviceType.Sp)
					: new List<RealPage>()
			};

			var pageAllList = viewModel.PcRealPageList
				.Select(
					item => new PageViewModel
					{
						PcRealPage = item
					})
				.ToList();

			// SPページの設定
			foreach (var spList in viewModel.SpRealPageList)
			{
				// PCに存在する場合はペアリング
				// 存在しない場合はPCページは存在しないものとして全ページリストへ登録
				if (pageAllList.Any(item => (item.PcRealPage != null)
					&& (item.PcRealPage.FileName == spList.FileName)))
				{
					var val = pageAllList.FirstOrDefault(item => (item.PcRealPage.FileName == spList.FileName));
					if (val == null) continue;

					var index = pageAllList.IndexOf(val);
					pageAllList.RemoveAt(index);
					val.SpRealPage = spList;
					pageAllList.Insert(index, val);
				}
				else
				{
					pageAllList.Add(
						new PageViewModel
						{
							SpRealPage = spList,
							PcRealPage = new RealPage(
									spList.PageTitle,
									DesignCommon.PhysicalDirPathTargetSitePc,
									spList.PageDirPath,
									spList.FileName,
									string.Empty)
								{
									LastChange = spList.LastChange
								}
						});
				}
			}

			var pageAllModel = new FeaturePageService().GetAllPage();

			// DBよりページ情報取得
			// 実ファイルに存在する場合は順序を設定
			pageAllList = pageAllList
				.Select(item =>
				{
					var model = pageAllModel.FirstOrDefault(
						pageModel => (pageModel.FileDirPath == item.PcDirPath)
							&& (pageModel.FileName == item.FileName));

					if (model != null)
					{
						item.PageSortNumber = model.PageSortNumber;
						item.Publish = model.Publish;
						item.UseType = model.UseType;
						item.PageId = model.FeaturePageId;
						item.PublishDateFrom = model.ConditionPublishDateFrom;
						item.PublishDateTo = model.ConditionPublishDateTo;
						item.IsSettingReleaseRange = (model.ConditionPublishDateFrom != null)
							|| (model.ConditionPublishDateTo != null)
							|| (model.ConditionMemberOnlyType != Constants.FLG_FEATUREPAGE_MEMBER_ONLY_TYPE_ALL)
							|| (string.IsNullOrEmpty(model.ConditionTargetListIds) == false);
						item.BrandIds = model.PermittedBrandIds;
						item.BrandIdList = model.BrandIdList;
						item.RootCateogryId = (string.IsNullOrEmpty(model.CategoryId) == false) ? model.CategoryId.Substring(0, 3) : string.Empty;
					}
					else
					{
						var insertModel = new FeaturePageModel
						{
							FileName = item.FileName,
							FileDirPath = item.PcDirPath,
							UseType = item.UseType,
							LastChanged = this.SessionWrapper.LoginOperatorName,
							CategoryId = string.Empty,
							PermittedBrandIds = string.Empty
						};
						var newPageId = new FeaturePageService().Insert(insertModel);
						model = new FeaturePageService().Get(newPageId);
					}

					item.PageId = model.FeaturePageId;
					item.ManagementTitle = model.ManagementTitle;

					return item;
				})
				.ToList();

			// SPオプション無効・レスポンシブ対応の場合、SPのみ設定されているページは除外
			if (DesignCommon.UseSmartPhone == false)
			{
				pageAllList = pageAllList.Where(p => p.UseType != Constants.FLG_PAGEDESIGN_USE_TYPE_SP).ToList();
			}

			pageAllList = SortBy(pageAllList.Where(l => Search(l, paramModel)).ToList(), paramModel);
			viewModel.HitCount = pageAllList.Count;

			var contentsLogService = new ContentsLogService();
			viewModel.ListPageViewModels = pageAllList
				.Skip((paramModel.PagerNo - 1) * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST)
				.Take(Constants.CONST_DISP_CONTENTS_DEFAULT_LIST)
				.Select(
					item =>
					{
						// サマリー追加
						var summary = contentsLogService.GetContentsSummaryData(
							Constants.FLG_CONTENTSLOG_CONTENTS_TYPE_FEATURE,
							item.PageId.ToString());
						var summaryToday = contentsLogService.GetContentsSummaryDataOfToday(
							Constants.FLG_CONTENTSLOG_CONTENTS_TYPE_FEATURE,
							item.PageId.ToString());
						item.ViewCount = ((summary.Any()) ? summary[0].PvCount : 0) + ((summaryToday.Any()) ? summaryToday[0].PvCount : 0);
						item.CvCount = ((summary.Any()) ? summary[0].CvCount : 0) + ((summaryToday.Any()) ? summaryToday[0].CvCount : 0);
						item.CvPrice = ((summary.Any()) ? summary[0].Price : 0) + ((summaryToday.Any()) ? summaryToday[0].Price : 0);

						return item;
					}).ToList();

			return viewModel;
		}

		/// <summary>
		/// 検索内容と一致するか?
		/// </summary>
		/// <param name="pvm">ページビューモデル</param>
		/// <param name="pm">検索パラメータ</param>
		/// <returns>一致 => true 不一致 => false</returns>
		private bool Search(PageViewModel pvm, FeaturePageListSearchParamModel pm)
		{
			var checkPageKeyWord = string.IsNullOrEmpty(pm.Keyword)
				|| (pvm.ManagementTitle.Contains(pm.Keyword) || pvm.FileName.Contains(pm.Keyword));

			var compareDateTime = DateTime.Now;
			switch (pm.DateChangedKbn)
			{
				case DAY:
					compareDateTime = DateTime.Now.AddDays(-1);
					break;

				case WEEK:
					compareDateTime = DateTime.Now.AddDays(-7);
					break;

				case MONTH:
					compareDateTime = DateTime.Now.AddMonths(-1);
					break;

				case THREE_MONTH:
				case THREE_MONTH_OVER:
					compareDateTime = DateTime.Now.AddMonths(-3);
					break;
			}

			var checkUpdateDate = (string.IsNullOrEmpty(pm.DateChangedKbn))
				|| ((pm.DateChangedKbn != THREE_MONTH_OVER)
					? ((pvm.PcRealPage.UpdateDate > compareDateTime)
						|| (pvm.SpRealPage.UpdateDate > compareDateTime))
					: ((pvm.PcRealPage.UpdateDate < compareDateTime)
						&& (pvm.SpRealPage.UpdateDate < compareDateTime)));

			var checkRootCategory = (string.IsNullOrEmpty(pm.ParentCategoryId) || (pvm.RootCateogryId == pm.ParentCategoryId));
			var checkBrandId = (string.IsNullOrEmpty(pm.BrandIds) || pvm.BrandIdList.Contains(pm.BrandIds));


			// 公開開始日条件
			var publishBeginFrom = true;
			var publishBeginTo = true;
			if (string.IsNullOrEmpty(pm.PublishBeginDateFromDate) == false)
			{
				publishBeginFrom = (DateTime.Parse(string.Format("{0} {1}", pm.PublishBeginDateFromDate, pm.PublishBeginDateFromTime))
					<= pvm.PublishDateFrom);
			}

			if (string.IsNullOrEmpty(pm.PublishBeginDateToDate) == false)
			{
				publishBeginTo = (DateTime.Parse(string.Format("{0} {1}", pm.PublishBeginDateToDate, pm.PublishBeginDateToTime))
					>= pvm.PublishDateFrom);
			}

			// 公開終了日条件
			var publishEndFrom = true;
			var publishEndTo = true;
			if (string.IsNullOrEmpty(pm.PublishEndDateFromDate) == false)
			{
				publishBeginFrom = (DateTime.Parse(string.Format("{0} {1}", pm.PublishEndDateFromDate, pm.PublishEndDateFromTime))
					<= pvm.PublishDateTo);
			}

			if (string.IsNullOrEmpty(pm.PublishEndDateToDate) == false)
			{
				publishBeginTo = (DateTime.Parse(string.Format("{0} {1}", pm.PublishEndDateToDate, pm.PublishEndDateToTime))
					>= pvm.PublishDateTo);
			}

			var checkType = pm.Types.Any(type => (type.IsSelected && (pvm.Publish == type.Value)));

			return (checkPageKeyWord && checkUpdateDate && publishBeginFrom && publishBeginTo && publishEndFrom && publishEndTo && checkType && checkBrandId && checkRootCategory);
		}

		/// <summary>
		/// 指定フィールドで並び替える
		/// </summary>
		/// <param name="pvm">ページビューモデルリスト</param>
		/// <param name="pm">検索パラメータ</param>
		/// <returns>並び替えるページビューモデルリスト</returns>
		private List<PageViewModel> SortBy(IEnumerable<PageViewModel> pvm, FeaturePageListSearchParamModel pm)
		{
			switch (pm.SortField)
			{
				case Constants.FIELD_FEATUREPAGE_DATE_CHANGED:
					return ((pm.SortType == Constants.FLG_SORT_TYPE_ASC)
						? pvm.OrderBy(m => m.UpdateDate)
						: pvm.OrderByDescending(m => m.UpdateDate)).ToList();

				case Constants.FIELD_FEATUREPAGE_MANAGEMENT_TITLE:
					var test = ((pm.SortType == Constants.FLG_SORT_TYPE_ASC)
						? pvm.OrderBy(m => m.ManagementTitle)
						: pvm.OrderByDescending(m => m.ManagementTitle)).ToList();

					return ((pm.SortType == Constants.FLG_SORT_TYPE_ASC)
						? pvm.OrderBy(m => m.ManagementTitle)
						: pvm.OrderByDescending(m => m.ManagementTitle)).ToList();

				case Constants.FIELD_FEATUREPAGE_PUBLISH:
					return ((pm.SortType == Constants.FLG_SORT_TYPE_ASC)
						? pvm.OrderBy(m => m.Publish)
						: pvm.OrderByDescending(m => m.Publish)).ToList();

				default:
					return pvm.ToList();
			}
		}

		/// <summary>
		/// 特集ページ詳細 ビューモデル作成
		/// </summary>
		/// <param name="pageId">ページID nullの場合は新規作成</param>
		/// <param name="parentCategory"></param>
		/// <returns>ビューモデル</returns>
		public FeaturePageDetailViewModel CreateFeaturePageDetailVm(long? pageId, string parentCategory = null)
		{
			try
			{
				var service = new FeaturePageService();

				var featurePageModel = (pageId == null)
					? PageDesignUtility.CreateNewFeatureFile()
					: service.Get(pageId.Value);

				this.PageId = featurePageModel.FeaturePageId;

				var pcPageList = PageDesignUtility.GetFeaturePageList(DesignCommon.DeviceType.Pc, featurePageModel.FileName);

				var pcRealPage = (pcPageList.Count == 1)
					? pcPageList[0]
					: new RealPage(
						featurePageModel.ManagementTitle,
						DesignCommon.PhysicalDirPathTargetSitePc,
						featurePageModel.FileDirPath,
						featurePageModel.FileName,
						string.Empty);

				// PCファイルが存在しない場合は新規作成
				RealPageIsNotExist(pcRealPage, featurePageModel, DesignCommon.DeviceType.Pc);

				// PC コンテンツ読み込み
				var pcFileTextAll = DesignCommon.GetFileTextAll(
					Path.Combine(
						DesignCommon.PhysicalDirPathTargetSitePc,
						pcRealPage.PageDirPath,
						pcRealPage.FileName));
				var pcContent = PageDesignUtility.GetEditableAreaForEdit(pcFileTextAll);

				var spPageList = PageDesignUtility.GetFeaturePageList(DesignCommon.DeviceType.Sp, featurePageModel.FileName);
				var spRealPage = (spPageList.Count == 1)
					? spPageList[0]
					: new RealPage(
						featurePageModel.ManagementTitle,
						DesignCommon.PhysicalDirPathTargetSiteSp,
						featurePageModel.FileDirPath,
						featurePageModel.FileName,
						string.Empty);

				// SPファイルが存在しない場合は新規作成
				RealPageIsNotExist(spRealPage, featurePageModel, DesignCommon.DeviceType.Sp);

				// SP コンテンツ読み込み
				var spFileTextAll = DesignCommon.GetFileTextAll(
					Path.Combine(
						DesignCommon.PhysicalDirPathTargetSiteSp,
						spRealPage.PageDirPath,
						spRealPage.FileName));
				var spContent = PageDesignUtility.GetEditableAreaForEdit(spFileTextAll);

				// もしPC、SPどちらかのファイルが存在しなかった場合
				if ((pcRealPage.Existence == RealPage.ExistStatus.NotExist)
					|| (spRealPage.Existence == RealPage.ExistStatus.NotExist))
				{
					new FeaturePageService().Update(featurePageModel);
				}

				// 入力箇所が無くなっていたらエラー
				var editor = ValueText.GetValueKvpArray(Constants.TABLE_FEATUREPAGE, "editor");
				if (((editor.All(e => pcContent.Any(pc => pc.Key == e.Value)))
					&& (editor.All(e => spContent.Any(sp => sp.Key == e.Value)))) == false)
					return null;

				var pcBannerFileName = PageDesignUtility.GetHeaderBannerFileName(pcFileTextAll);
				var spBannerFileName = PageDesignUtility.GetHeaderBannerFileName(spFileTextAll);
				var input = new FeaturePageInput(featurePageModel)
				{
					PcContentInput =
					{
						Content = pcContent,
						SeoTitle = pcRealPage.PageTitle,
						HeaderBanner = new ImageInput(ImageType.Page)
						{
							BaseName = "input.PcContentInput.HeaderBanner",
							FileName = pcBannerFileName,
							RealFileName = HttpUtility.UrlDecode(pcBannerFileName),
						},
						PageTitleDisp = PageDesignUtility.GetDispCheckBox(pcFileTextAll, Constants.FEATUREPAGE_PAGE_TITLE),
						HeaderBannerDisp = PageDesignUtility.GetDispCheckBox(pcFileTextAll, Constants.FEATUREPAGE_HEADER_BANNER),
						UpperContentsAreaDisp = PageDesignUtility.GetDispCheckBox(pcFileTextAll, Constants.FEATUREPAGE_UPPER_CONTENTS_AREA),
						LowerContentsAreaDisp = PageDesignUtility.GetDispCheckBox(pcFileTextAll, Constants.FEATUREPAGE_LOWER_CONTENTS_AREA),

					},
					SpContentInput =
					{
						Content = spContent,
						SeoTitle = spRealPage.PageTitle,
						HeaderBanner = new ImageInput(ImageType.Page)
						{
							BaseName = "input.SpContentInput.HeaderBanner",
							FileName = spBannerFileName,
							RealFileName = HttpUtility.UrlDecode(spBannerFileName),
						},
						PageTitleDisp = PageDesignUtility.GetDispCheckBox(spFileTextAll, Constants.FEATUREPAGE_PAGE_TITLE),
						HeaderBannerDisp = PageDesignUtility.GetDispCheckBox(spFileTextAll, Constants.FEATUREPAGE_HEADER_BANNER),
						UpperContentsAreaDisp = PageDesignUtility.GetDispCheckBox(spFileTextAll, Constants.FEATUREPAGE_UPPER_CONTENTS_AREA),
						LowerContentsAreaDisp = PageDesignUtility.GetDispCheckBox(spFileTextAll, Constants.FEATUREPAGE_LOWER_CONTENTS_AREA),
					}
				};

				input.PcContentInput.ProductList = input.PcContentInput.Sort.Where(list => list.StartsWith(Constants.FEATUREPAGE_PRODUCR_LIST)).ToArray();
				input.SpContentInput.ProductList = input.SpContentInput.Sort.Where(list => list.StartsWith(Constants.FEATUREPAGE_PRODUCR_LIST)).ToArray();
				var pcProductListDisp = new List<bool>();
				var spProductListDisp = new List<bool>();
				for (var i = 0; i < input.PcContentInput.ProductList.Length; i++)
				{
					pcProductListDisp.Add(PageDesignUtility.GetDispCheckBox(pcFileTextAll, Constants.FEATUREPAGE_PRODUCR_LIST + i));
				}
				input.PcContentInput.ProductListDisp = pcProductListDisp.ToArray();

				for (var i = 0; i < input.SpContentInput.ProductList.Length; i++)
				{
					spProductListDisp.Add(PageDesignUtility.GetDispCheckBox(spFileTextAll, Constants.FEATUREPAGE_PRODUCR_LIST + i));
				}
				input.SpContentInput.ProductListDisp = spProductListDisp.ToArray();

				var vm = new FeaturePageDetailViewModel(input)
				{
					PcDirPath = Path.Combine(Constants.PATH_ROOT_FRONT_PC, pcRealPage.PageDirPath).Replace(@"\", "/"),
					PcRealPage = pcRealPage,
					SpRealPage = spRealPage,
					Input = input,
				};

				// 選択した親カテゴリに紐づいた子カテゴリのドロップダウン作成
				if (string.IsNullOrEmpty(parentCategory) == false)
				{
					vm.Input.ParentCategoryId = parentCategory;
					vm.ParentCategoryItems = new[]
					{
						new SelectListItem
						{
							Value = string.Empty,
							Text = string.Empty
						}
					}.Concat(
						new FeaturePageService()
							.GetRootCategoryItem()
							.Select(
							s => new SelectListItem
							{
								Value = s.CategoryId,
								Text = s.CategoryName,
								Selected = (parentCategory == s.CategoryId.ToString())
							})).ToArray();

					vm.CategoryItems = new[]
					{
						new SelectListItem
						{
							Value = string.Empty,
							Text = string.Empty,
							Selected = true
						}
					}.Concat(
						new FeaturePageService().GetChildCategoryItem(parentCategory).Select(
							s => new SelectListItem
							{
								Value = s.CategoryId,
								Text = s.CategoryName
							})).ToArray();
				}

				vm.Input.ProductInput = SetProductInput(featurePageModel.Contents);

				var allPartsModel = new PartsDesignService().GetAllParts();
				// PCレイアウトパーツセット
				var allRealPartsListPc = PartsDesignCommon.GetCustomPartsList(DesignCommon.DeviceType.Pc);
				allRealPartsListPc.AddRange(PartsDesignCommon.GetStandardPartsList(DesignCommon.DeviceType.Pc));
				PageDesignUtility.SetContentInputLayoutParts(pcFileTextAll, allPartsModel, allRealPartsListPc, vm.LayoutEditViewModelPc.Input);
				var pcLayoutName = PageDesignUtility.GetLayoutName(pcFileTextAll);
				vm.LayoutEditViewModelPc.Input.LayoutType = (string.IsNullOrEmpty(pcLayoutName))
					? LayoutEditInput.LayoutTypeMaster.Default.ToString()
					: pcLayoutName;

				// SPレイアウトパーツセット
				var allRealPartsListSp = PartsDesignCommon.GetCustomPartsList(DesignCommon.DeviceType.Sp);
				allRealPartsListSp.AddRange(PartsDesignCommon.GetStandardPartsList(DesignCommon.DeviceType.Sp));
				PageDesignUtility.SetContentInputLayoutParts(spFileTextAll, allPartsModel, allRealPartsListSp, vm.LayoutEditViewModelSp.Input);
				var spLayoutName = PageDesignUtility.GetLayoutName(spFileTextAll);
				vm.LayoutEditViewModelSp.Input.LayoutType = (string.IsNullOrEmpty(spLayoutName))
					? LayoutEditInput.LayoutTypeMaster.Default.ToString()
					: spLayoutName;

				UpdateFileOpenTime(pcRealPage.PhysicalFullPath);
				UpdateFileOpenTime(spRealPage.PhysicalFullPath);
				return vm;
			}
			catch (Exception e)
			{
				FileLogger.WriteError(e);
				return null;
			}
		}

		/// <summary>
		/// ファイルが存在しない場合は新規作成
		/// </summary>
		/// <param name="realPage">実ページ</param>
		/// <param name="model">特集ページモデル</param>
		/// <param name="type">デバイスタイプ</param>
		public void RealPageIsNotExist(RealPage realPage, FeaturePageModel model, DesignCommon.DeviceType type)
		{
			if (realPage.Existence != RealPage.ExistStatus.NotExist) return;
			DesignCommon.FileCopy(
				Path.Combine(
					(type == DesignCommon.DeviceType.Pc)
						? DesignCommon.PhysicalDirPathTargetSitePc
						: DesignCommon.PhysicalDirPathTargetSiteSp,
					Constants.PAGE_FRONT_FEATUREPAGE_TEMPLATE.Replace("/", @"\")),
				realPage.PhysicalFullPath);

			PageTitleEdit(realPage.PhysicalFullPath, model.ManagementTitle);

			DesignUtility.UpdateLastChanged(realPage.PhysicalFullPath, this.SessionWrapper.LoginOperatorName);
			realPage.LastChange = this.SessionWrapper.LoginOperatorName;
		}

		/// <summary>
		/// 商品一覧の入力情報をセット
		/// </summary>
		/// <param name="contents">特集ページコンテンツ</param>
		/// <returns>商品一覧入力</returns>
		private ProductInputViewModel[] SetProductInput(FeaturePageContentsModel[] contents)
		{
			if (contents.Length == 0) return null;

			var productInputList = new List<ProductInputViewModel>();

			foreach (var c in contents)
			{
				if (c.ContentsType != Constants.FLG_FEATUREPAGECONTENTS_TYPE_PRODUCT_LIST
					|| c.ContentsKbn == Constants.FLG_FEATUREPAGECONTENTS_CONTENTS_KBN_SP
					|| (string.IsNullOrEmpty(c.ProductGroupId))) continue;

				var model = new ProductGroupService().Get(c.ProductGroupId);

				// 紐づいているのに商品グループ設定を削除してしまったときに有効になる
				if (model == null) continue;

				var productInput = new ProductInputViewModel
				{
					Title = c.ProductListTitle,
					GroupId = model.ProductGroupId,
					GroupName = model.ProductGroupName,
					DispNum = c.DispNum,
					Pagination = c.Pagination,
					ProductList = model.Items.Select(
						i => new ProductViewModel
						{
							Id = i.MasterId,
							ImagePath = GetImagePath(i.MasterId),
						}).ToArray(),
				};

				productInputList.Add(productInput);
			}
			return productInputList.ToArray();
		}

		/// <summary>
		/// 画像パス取得
		/// </summary>
		/// <param name="productId">商品ID</param>
		/// <returns>画像パス</returns>
		public string GetImagePath(string productId)
		{
			var product = new ProductService().Get(this.SessionWrapper.LoginShopId, productId);
			if (product == null) return String.Empty;
			var path = string.IsNullOrEmpty(product.ImageHead)
				? string.Format("{0}{1}", Constants.PATH_PRODUCTIMAGES, Constants.PRODUCTIMAGE_NOIMAGE_PC)
				: string.Format("{0}0/{1}{2}", Constants.PATH_PRODUCTIMAGES, product.ImageHead, Constants.PRODUCTIMAGE_FOOTER_M);
			return path;
		}

		/// <summary>
		/// ページ詳細更新 ページペアリング単位
		/// </summary>
		/// <param name="input">ページ詳細 入力内容</param>
		/// <returns>エラーメッセージ</returns>
		public string UpdateDetailPage(FeaturePageInput input)
		{
			var errorMessage = input.Validate();

			if (string.IsNullOrEmpty(errorMessage) == false) return errorMessage;

			// ページコンパイルチェック
			var pcPageErrorMessage = WebRequestCheck.Send(Preview(input, DesignCommon.DeviceType.Pc));
			var spPageErrorMessage = WebRequestCheck.Send(Preview(input, DesignCommon.DeviceType.Sp));
			PageDesignUtility.DeleteFeaturePreview();

			if (string.IsNullOrEmpty(pcPageErrorMessage) == false)
			{
				return pcPageErrorMessage;
			}

			if(string.IsNullOrEmpty(spPageErrorMessage) == false)
			{
				return spPageErrorMessage;
			}

			var service = new FeaturePageService();
			var beforeModel = service.Get(input.PageId);
			var model = input.CreateModel();
			model.LastChanged = this.SessionWrapper.LoginOperatorName;
			input.PcContentInput.SeoTitle = model.HtmlPageTitle;
			input.SpContentInput.SeoTitle = model.HtmlPageTitle;

			var contentsList = new List<FeaturePageContentsModel>();

			if (input.PageType != Constants.FLG_FEATUREPAGE_MULTI_GROUP)
			{
				var productInput = (input.ProductInput == null)
					? new ProductInputViewModel()
					: input.ProductInput[0];

				contentsList.AddRange(
					new[]
					{
						CreateProductContent(input, productInput, 0),
						CreateProductContent(input, productInput, 0, false)
					});
			}
			else if (input.ProductInput != null)
			{
				contentsList.AddRange(input.ProductInput.Select((pl, index) => CreateProductContent(input, pl, index)));
				contentsList.AddRange(
					input.ProductInput.Select((pl, index) => CreateProductContent(input, pl, index, false)));
			}

			AddExceptForProductToList(input.PageId, input.PcContentInput, contentsList);
			AddExceptForProductToList(input.PageId, input.SpContentInput, contentsList, false);

			errorMessage = UpdatePage(
				input,
				input.PcContentInput,
				DesignCommon.PhysicalDirPathTargetSitePc,
				beforeModel,
				DesignCommon.DeviceType.Pc);

			if (string.IsNullOrEmpty(errorMessage) == false) return errorMessage;

			if (DesignCommon.UseResponsive == false)
			{
				errorMessage = UpdatePage(
					input,
					input.SpContentInput,
					DesignCommon.PhysicalDirPathTargetSiteSp,
					beforeModel,
					DesignCommon.DeviceType.Sp);
			}

			if (string.IsNullOrEmpty(errorMessage) == false) return errorMessage;

			HeaderBannerUpload(input);

			model.Contents = contentsList.ToArray();
			service.Update(model);

			// フロントのキャッシュ更新
			RefreshFileManagerProvider.GetInstance(RefreshFileType.FeaturePage).CreateUpdateRefreshFile();

			WebBrowserCapture.Create(
				Constants.PHYSICALDIRPATH_CMS_MANAGER,
				Path.Combine(beforeModel.FileDirPath, (input.FileNameNoExtension + PageDesignUtility.PAGE_FILE_EXTENSION)),
				device: WebBrowserCapture.Device.Pc,
				delay: 100,
				iSizeH: 800,
				iSizeW: 800,
				bSizeH: 1280,
				bSizeW: 720);
			WebBrowserCapture.Create(
				Constants.PHYSICALDIRPATH_CMS_MANAGER,
				Path.Combine(
					DesignCommon.SiteSpRootPath,
					beforeModel.FileDirPath,
					(input.FileNameNoExtension + PageDesignUtility.PAGE_FILE_EXTENSION)),
				device: WebBrowserCapture.Device.Sp,
				delay: 100,
				iSizeH: 400,
				iSizeW: 400,
				bSizeH: 400,
				bSizeW: 800);

			return errorMessage;
		}

		/// <summary>
		/// ヘッダーバナーアップロード
		/// </summary>
		/// <param name="input">特集ページ詳細入力</param>
		private void HeaderBannerUpload(FeaturePageInput input)
		{
			var feature = Path.Combine(Constants.PHYSICALDIRPATH_CONTENTS_ROOT, Constants.PATH_FEATUREPAGE_IMAGE);
			var pcHeaderBanner = input.PcContentInput.HeaderBanner;

			// 画像リストから選択されていない且つアップロードファイルがある
			if ((pcHeaderBanner.ImageId == 0) && (pcHeaderBanner.UploadFile != null))
			{
				pcHeaderBanner.UploadFile.SaveAs(
					Path.Combine(feature, Path.GetFileName(pcHeaderBanner.UploadFile.FileName)));
			}

			var spHeaderBanner = input.SpContentInput.HeaderBanner;
			if ((spHeaderBanner.ImageId == 0) && (spHeaderBanner.UploadFile != null))
			{
				spHeaderBanner.UploadFile.SaveAs(
					Path.Combine(feature, Path.GetFileName(spHeaderBanner.UploadFile.FileName)));
			}
		}

		/// <summary>
		/// 商品以外のコンテンツをリストに追加
		/// </summary>
		/// <param name="pageId">特集ページID</param>
		/// <param name="input">特集ページコンテンツ入力</param>
		/// <param name="contentsList">コンテンツリスト</param>
		/// <param name="isPc">Pcか</param>
		private void AddExceptForProductToList(
			long pageId,
			FeaturePageContentInput input,
			List<FeaturePageContentsModel> contentsList,
			bool isPc = true)
		{
			var pageTitle = new FeaturePageContentsModel
			{
				FeaturePageId = pageId,
				ContentsType = Constants.FLG_FEATUREPAGECONTENTS_TYPE_PAGE_TITLE,
				ContentsSortNumber = input.Sort.ToList().FindIndex(s => (s == Constants.FEATUREPAGE_PAGE_TITLE)),
				PageTitle = input.PageTitle,
				ContentsKbn = isPc ? Constants.FLG_FEATUREPAGECONTENTS_CONTENTS_KBN_PC : Constants.FLG_FEATUREPAGECONTENTS_CONTENTS_KBN_SP,
				LastChanged = this.SessionWrapper.LoginOperatorName,
			};

			var headerBanner = new FeaturePageContentsModel
			{
				FeaturePageId = pageId,
				ContentsType = Constants.FLG_FEATUREPAGECONTENTS_TYPE_HEADER_BANNER,
				ContentsSortNumber = input.Sort.ToList().FindIndex(s => (s == Constants.FEATUREPAGE_HEADER_BANNER)),
				AltText = input.AltText,
				ContentsKbn = isPc ? Constants.FLG_FEATUREPAGECONTENTS_CONTENTS_KBN_PC : Constants.FLG_FEATUREPAGECONTENTS_CONTENTS_KBN_SP,
				LastChanged = this.SessionWrapper.LoginOperatorName,
			};

			var upperContentArea = new FeaturePageContentsModel
			{
				FeaturePageId = pageId,
				ContentsType = Constants.FLG_FEATUREPAGECONTENTS_TYPE_UPPER_CONTENTS_AREA,
				ContentsSortNumber = input.Sort.ToList().FindIndex(s => (s == Constants.FEATUREPAGE_UPPER_CONTENTS_AREA)),
				ContentsKbn = isPc ? Constants.FLG_FEATUREPAGECONTENTS_CONTENTS_KBN_PC : Constants.FLG_FEATUREPAGECONTENTS_CONTENTS_KBN_SP,
				LastChanged = this.SessionWrapper.LoginOperatorName,
			};

			var lowerContentArea = new FeaturePageContentsModel
			{
				FeaturePageId = pageId,
				ContentsType = Constants.FLG_FEATUREPAGECONTENTS_TYPE_LOWER_CONTENTS_AREA,
				ContentsSortNumber = input.Sort.ToList().FindIndex(s => (s == Constants.FEATUREPAGE_LOWER_CONTENTS_AREA)),
				ContentsKbn = isPc ? Constants.FLG_FEATUREPAGECONTENTS_CONTENTS_KBN_PC : Constants.FLG_FEATUREPAGECONTENTS_CONTENTS_KBN_SP,
				LastChanged = this.SessionWrapper.LoginOperatorName,
			};

			contentsList.Add(pageTitle);
			contentsList.Add(headerBanner);
			contentsList.Add(upperContentArea);
			contentsList.Add(lowerContentArea);
		}

		/// <summary>
		/// 商品の特集ページコンテンツ作成
		/// </summary>
		/// <param name="input">特集ページ入力</param>
		/// <param name="productInput">商品入力</param>
		/// <param name="index">商品入力配列のインデックス</param>
		/// <param name="isPc">PCか</param>
		/// <returns></returns>
		private FeaturePageContentsModel CreateProductContent(
			FeaturePageInput input,
			ProductInputViewModel productInput,
			int index,
			bool isPc = true)
		{
			var model = new FeaturePageContentsModel
			{
				FeaturePageId = input.PageId,
				ContentsType = Constants.FLG_FEATUREPAGECONTENTS_TYPE_PRODUCT_LIST,
				ContentsSortNumber = (isPc) ? input.PcContentInput.Sort.ToList().FindIndex(s => (s == "product-list-" + index))
					: input.SpContentInput.Sort.ToList().FindIndex(s => (s == "product-list-" + index)),
				ProductListTitle = productInput.Title,
				DispNum = productInput.DispNum,
				Pagination = productInput.Pagination,
				LastChanged = this.SessionWrapper.LoginOperatorName,
				ContentsKbn = isPc ? Constants.FLG_FEATUREPAGECONTENTS_CONTENTS_KBN_PC : Constants.FLG_FEATUREPAGECONTENTS_CONTENTS_KBN_SP,
			};

			if (isPc == false) return model;

			if (string.IsNullOrEmpty(productInput.GroupId) && (productInput.ProductList != null))
			{
				this.ProductGourpId = NumberingUtility
									.CreateNewNumber(
										this.SessionWrapper.LoginShopId,
										"ProductGroupId")
									.ToString()
									.PadLeft(10, '0');

				var productGroups = new ProductGroupService().GetAllProductGroup();
				CreateProductGroupId(productGroups);

				// 商品グループを作成
				var groupId = this.ProductGourpId;
				var productGroupModel = new ProductGroupModel
				{
					ProductGroupName = "特集ページから作成したグループ",
					ProductGroupId = groupId,
					BeginDate = DateTime.Now, // 公開範囲設定で制御するため
					LastChanged = this.SessionWrapper.LoginOperatorName,
					Items = productInput.ProductList.Select(
						(p, i) => new ProductGroupItemModel
						{
							ProductGroupId = groupId,
							ItemNo = i + 1,
							MasterId = p.Id,
							ShopId = this.SessionWrapper.LoginShopId,
							ItemType = Constants.FLG_PRODUCTGROUPITEM_ITEM_TYPE_PRODUCT,
							LastChanged = this.SessionWrapper.LoginOperatorName,
						}).ToArray()
				};

				new ProductGroupService().Insert(productGroupModel);

				model.ProductGroupId = groupId;
			}
			else
			{
				model.ProductGroupId = productInput.GroupId;
			}

			return model;
		}

		/// <summary>
		/// 特集ページ詳細更新 ページ単位
		/// </summary>
		/// <param name="input">特集ページ詳細 入力内容</param>
		/// <param name="contentInput">コンテンツ入力内容</param>
		/// <param name="physicaldirpathTargetSite">ページ物理パス</param>
		/// <param name="beforeModel">更新前ページモデル</param>
		/// <param name="deviceType">デバイスタイプ</param>
		/// <returns>エラーメッセージ</returns>
		private string UpdatePage(
			FeaturePageInput input,
			FeaturePageContentInput contentInput,
			string physicaldirpathTargetSite,
			FeaturePageModel beforeModel,
			DesignCommon.DeviceType deviceType)
		{
			var fileName = input.FileNameNoExtension + PageDesignUtility.PAGE_FILE_EXTENSION;
			var sourceFilePath = Path.Combine(
				physicaldirpathTargetSite,
				beforeModel.FileDirPath,
				beforeModel.FileName);

			var outputFilePath = Path.Combine(
				physicaldirpathTargetSite,
				beforeModel.FileDirPath,
				fileName);
			var targetUrl = deviceType == DesignCommon.DeviceType.Pc
				? beforeModel.FileDirPath
				: Path.Combine(DesignCommon.SiteSpRootPath, beforeModel.FileDirPath);
			var webUrl = PageDesignUtility.WebTargetPageUrl(
				targetUrl,
				fileName);

			if (string.IsNullOrEmpty(beforeModel.HtmlPageTitle)) beforeModel.HtmlPageTitle = input.ManagementTitle;

			// ファイル名変更があり、変更後ファイル名が既に存在する場合はエラー
			if ((sourceFilePath != outputFilePath)
				&& (File.Exists(outputFilePath)
					|| (PageDesignUtility.CheckFeaturePageFileName(fileName) == false)))
			{
				return WebMessages.PageDesignFileNameDuplicateError;
			}

			var errorMessage = UpdatePage(contentInput, sourceFilePath, outputFilePath, webUrl, deviceType);

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
		/// <param name="contentInput">コンテンツ内容</param>
		/// <param name="inputFilePath">元ページ物理ファイルパス</param>
		/// <param name="outputFilePath">生成ページ物理ファイルパス</param>
		/// <param name="webUrl">WebアクセスURL</param>
		/// <param name="deviceType">デバイスタイプ</param>
		private string UpdatePage(
			FeaturePageContentInput contentInput,
			string inputFilePath,
			string outputFilePath,
			string webUrl,
			DesignCommon.DeviceType deviceType)
		{
			// 選択したテンプレート取得
			var fileTextAll = new StringBuilder(DesignCommon.GetFileTextAll(inputFilePath));

			// レイアウト名 置換
			PageDesignUtility.ReplaceLayoutName(fileTextAll, contentInput.LayoutEditInput.LayoutType);

			contentInput.LayoutEditInput.SetMovePartsModel(deviceType);

			var allRealPartsList = PartsDesignCommon.GetCustomPartsList(deviceType);
			allRealPartsList.AddRange(PartsDesignCommon.GetStandardPartsList(deviceType));
			PageDesignUtility.ReplaceLayoutForUpdate(
				fileTextAll,
				allRealPartsList,
				deviceType,
				contentInput.LayoutEditInput);

			// ヘッダーバナー画像が非表示の場合headに非表示にするstyleタグを追加
			var headArea = ValueText.GetValueKvpArray(Constants.TABLE_FEATUREPAGE, "editor")[0].Value;
			var hiddenHeaderBannerStyle = "\r\n<style>#" + Constants.FEATUREPAGE_HEADER_BANNER + " { display: none; }</style>";
			var hiddenPageTitleStyle = "\r\n<style>#" + Constants.FEATUREPAGE_PAGE_TITLE + " { display: none; }</style>";
			var hiddenUpperContentsAreaStyle = "\r\n<style>#" + Constants.FEATUREPAGE_UPPER_CONTENTS_AREA + " { display: none; }</style>";
			var hiddenLowerContentsAreaStyle = "\r\n<style>#" + Constants.FEATUREPAGE_LOWER_CONTENTS_AREA + " { display: none; }</style>";

			var matchHederBunner = Regex.Match(
				contentInput.Content[headArea],
				"\r\n<style>#" + Constants.FEATUREPAGE_HEADER_BANNER + " {.*?display: none;.*?}</style>",
				RegexOptions.Singleline);
			var matchPageTitle = Regex.Match(
				contentInput.Content[headArea],
				"\r\n<style>#" + Constants.FEATUREPAGE_PAGE_TITLE + " {.*?display: none;.*?}</style>",
				RegexOptions.Singleline);
			var matchUpperContentsArea = Regex.Match(
				contentInput.Content[headArea],
				"\r\n<style>#" + Constants.FEATUREPAGE_UPPER_CONTENTS_AREA + " {.*?display: none;.*?}</style>",
				RegexOptions.Singleline);
			var matchLowerContentsArea = Regex.Match(
				contentInput.Content[headArea],
				"\r\n<style>#" + Constants.FEATUREPAGE_LOWER_CONTENTS_AREA + " {.*? display: none;.*?}</style>",
				RegexOptions.Singleline);

			if ((contentInput.HeaderBannerDisp == false) && (matchHederBunner == Match.Empty))
			{
				contentInput.Content[headArea] += hiddenHeaderBannerStyle;
			}
			if (contentInput.HeaderBannerDisp && (matchHederBunner != Match.Empty))
			{
				contentInput.Content[headArea] = contentInput.Content[headArea].Replace(matchHederBunner.Value, string.Empty);
			}

			if ((contentInput.PageTitleDisp == false) && (matchPageTitle == Match.Empty))
			{
				contentInput.Content[headArea] += hiddenPageTitleStyle;
			}
			if (contentInput.PageTitleDisp && (matchPageTitle != Match.Empty))
			{
				contentInput.Content[headArea] = contentInput.Content[headArea].Replace(matchPageTitle.Value, string.Empty);
			}

			if ((contentInput.UpperContentsAreaDisp == false) && (matchUpperContentsArea == Match.Empty))
			{
				contentInput.Content[headArea] += hiddenUpperContentsAreaStyle;
			}
			if (contentInput.UpperContentsAreaDisp && (matchUpperContentsArea != Match.Empty))
			{
				contentInput.Content[headArea] = contentInput.Content[headArea].Replace(matchUpperContentsArea.Value, string.Empty);
			}

			if ((contentInput.LowerContentsAreaDisp == false) && (matchLowerContentsArea == Match.Empty))
			{
				contentInput.Content[headArea] += hiddenLowerContentsAreaStyle;
			}
			if (contentInput.LowerContentsAreaDisp && (matchLowerContentsArea != Match.Empty))
			{
				contentInput.Content[headArea] = contentInput.Content[headArea].Replace(matchLowerContentsArea.Value, string.Empty);
			}

			var index = 0;
			foreach(var productListDisp in contentInput.ProductListDisp)
			{
				if ((index + 1) == contentInput.ProductListDisp.Length) break;
				var matchProductList = Regex.Match(
					contentInput.Content[headArea],
					"\r\n<style>#" + Constants.FEATUREPAGE_PRODUCR_LIST + index.ToString() + " {.*?display: none;.*?}</style>",
					RegexOptions.Singleline);
				var hiddenProductListStyle = "\r\n<style>#" + Constants.FEATUREPAGE_PRODUCR_LIST + index.ToString() + " { display: none; }</style>";

				if ((productListDisp == false) && (matchProductList == Match.Empty))
				{
					contentInput.Content[headArea] += hiddenProductListStyle;
				}
				if (productListDisp && (matchProductList != Match.Empty))
				{
					contentInput.Content[headArea] = contentInput.Content[headArea].Replace(matchProductList.Value, string.Empty);
				}
				index++;
			}

			// 元の表示処理が残っていたら削除
			PageDesignUtility.DeleteRemoveJquery(fileTextAll);

			// コンテンツ置換
			PageDesignUtility.ReplaceContentsTagForUpdate(fileTextAll, contentInput.Content);

			// タイトル置換
			DesignCommon.ReplaceTitle(fileTextAll, contentInput.SeoTitle);

			// 最終更新者 置換
			DesignCommon.ReplaceLastChanged(fileTextAll, this.SessionWrapper.LoginOperatorName);


			// レイアウトによるマスターファイルの更新
			var masterFileName =
				(contentInput.LayoutEditInput.LayoutType == LayoutEditInput.LayoutTypeMaster.Simple.ToString())
					? Path.GetFileNameWithoutExtension(Constants.PAGE_FRONT_SIMPLE_DEFAULT_MASTER)
					: Path.GetFileNameWithoutExtension(Constants.PAGE_FRONT_DEFAULT_MASTER);

			PageDesignUtility.ReplaceMasterFile(fileTextAll, masterFileName);

			// ヘッダーバナー置換
			if (contentInput.HeaderBanner.IsRemove == false)
			{
				var fileName = (contentInput.HeaderBanner.UploadFile != null)
					? HttpUtility.UrlEncode(contentInput.HeaderBanner.UploadFile.FileName).Replace("+", "%20")
					: (contentInput.HeaderBanner.ImageId != 0)
						? new FeatureImageService().Get(contentInput.HeaderBanner.ImageId).FileName
						: string.Empty;
				if (string.IsNullOrEmpty(fileName) == false)
				{
					var filePath = Path.Combine(Constants.PATH_ROOT_FRONT_PC, Constants.PATH_FEATUREPAGE_IMAGE, fileName);
					PageDesignUtility.ReplaceHeaderBanner(fileTextAll, filePath);
				}

				// プレビューソースが指定されている場合はこちらで置換
				if (string.IsNullOrEmpty(contentInput.HeaderBanner.PreviewBinary) == false)
				{
					PageDesignUtility.ReplaceHeaderBanner(fileTextAll, contentInput.HeaderBanner.PreviewBinary);
				}
			}
			else
			{
				PageDesignUtility.ReplaceHeaderBanner(fileTextAll, string.Empty);
			}

			// ファイル書き込み
			var errorMessage = DesignUtility.UpdateFile(
				outputFilePath,
				fileTextAll.ToString(),
				true,
				(inputFilePath.Equals(outputFilePath) == false));

			// Webリクエストチェック
			errorMessage += WebRequestCheck.Send(webUrl);

			return errorMessage;
		}

		/// <summary>
		/// ページ順序更新
		/// </summary>
		/// <param name="pageIds">ページ順序</param>
		public void PageSortUpdate(long[] pageIds)
		{
			if (pageIds == null) return;

			var models = pageIds
				.Select((item, index) => new FeaturePageModel
					{
						FeaturePageId = item,
						PageSortNumber = index + 1,
						LastChanged = this.SessionWrapper.LoginOperatorName
					})
				.ToArray();

			new FeaturePageService().UpdatePageSort(models);
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
		/// ページ削除
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <returns>エラーメッセージ</returns>
		public string DeletePage(long pageId)
		{
			var errorMessage = PageDesignUtility.DeleteFeaturePage(pageId);
			return errorMessage;
		}

		/// <summary>
		/// ページ複製
		/// </summary>
		/// <param name="sourcePageId">コピー元ページID</param>
		/// <returns>新規ページID</returns>
		public long CreateCopyFile(long sourcePageId)
		{
			var newPageId = PageDesignUtility.CopyFeatureFile(sourcePageId);
			return newPageId;
		}

		/// <summary>
		/// プレビュー表示
		/// </summary>
		/// <param name="input">特集ページ詳細 入力内容</param>
		/// <param name="deviceType">デバイスタイプ</param>
		/// <returns>プレビューURL</returns>
		public string Preview(FeaturePageInput input, DesignCommon.DeviceType deviceType)
		{
			var model = new FeaturePageService().Get(input.PageId);
			var physicalDirPathTargetSite = DesignCommon.GetPhysicalDirPathTargetSite(deviceType);
			var webUrlDirPath = (deviceType == DesignCommon.DeviceType.Pc)
				? model.FileDirPath
				: Path.Combine(DesignCommon.SiteSpRootPath, model.FileDirPath);
			var contentInput = (deviceType == DesignCommon.DeviceType.Pc)
				? input.PcContentInput
				: input.SpContentInput;
			var previewFileName = input.FileNameNoExtension + PageDesignUtility.PREVIEW_PAGE_FILE_EXTENSION;

			var sourceFilePath = Path.Combine(
				physicalDirPathTargetSite,
				model.FileDirPath,
				model.FileName);

			var outputFilePath = Path.Combine(
				physicalDirPathTargetSite,
				model.FileDirPath,
				previewFileName);

			var webUrl = PageDesignUtility.WebTargetPageUrl(
				webUrlDirPath,
				previewFileName);

			UpdatePage(contentInput, sourceFilePath, outputFilePath, webUrl, deviceType);

			if (model.Contents.Length == 0)
			{
				webUrl += string.Format("?{0}={1}", Constants.REQUEST_KEY_FEATURE_PAGE_TYPE, HttpUtility.UrlEncode(input.PageType).Replace("+", "%20"));
			}

			return webUrl;
		}

		/// <summary>
		/// 管理用タイトル更新
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <param name="title">管理用タイトル</param>
		/// <returns>エラーメッセージ</returns>
		public void ManagementTitleEdit(long pageId, string title)
		{
			new FeaturePageService().UpdateManagementTitle(pageId, title);
		}

		/// <summary>
		/// 商品の表示順を変更
		/// </summary>
		/// <param name="productIds">商品IDのリスト</param>
		/// <param name="sortType">並び順</param>
		/// <param name="baseName">バインド時の名前</param>
		/// <returns>ビューモデル</returns>
		public ProductSortViewModel UpdateProductSort(string[] productIds, string sortType, string baseName)
		{
			var service = new ProductService();
			var models = productIds.Select(id => service.Get(this.SessionWrapper.LoginShopId, id));

			switch (sortType)
			{
				case SORT_PRICE:
					models = models.OrderBy(m => m.DisplayPrice);
					break;

				case SORT_ID:
					models = models.OrderBy(m => m.ProductId);
					break;

				case SORT_CATEGORY:
					models = models.OrderBy(m => m.CategoryId1);
					break;

				case SORT_BRAND:
					models = models.OrderBy(m => m.BrandId1);
					break;

				case SORT_NAME:
					models = models.OrderBy(m => m.Name);
					break;

				case SORT_DISPLAY_PRIORITY:
					models = models.OrderBy(m => m.DisplayPriority);
					break;

				case SORT_SELL_FROM:
					models = models.OrderBy(m => m.SellFrom);
					break;
			}

			var stockService = new ProductStockService();
			var vm = new ProductSortViewModel
			{
				ProductList = models.Select((m, index) => new ProductViewModel
				{
					Name = m.Name,
					Id = m.ProductId,
					ImagePath = GetImagePath(m.ProductId),
					Stock = stockService.GetSum(this.SessionWrapper.LoginShopId, m.ProductId),
					SortNo = ++index,
				}).ToArray(),
				BaseName = baseName
			};

			return vm;
		}

		/// <summary>
		/// 子カテゴリリスト作成
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <param name="rootCategoryId">最上位カテゴリID</param>
		/// <returns>子カテゴリリスト</returns>
		public FeaturePageDetailViewModel CreateChildCategoryList(long? pageId, string rootCategoryId)
		{
			var vm = CreateFeaturePageDetailVm(pageId, rootCategoryId);
			return vm;
		}

		/// <summary>
		/// 商品グループID生成
		/// </summary>
		/// <param name="productGroupModels">商品グループモデル</param>
		private void CreateProductGroupId(ProductGroupModel[] productGroupModels)
		{
			var isProductGroupId = productGroupModels.Any(productGroup => productGroup.ProductGroupId == this.ProductGourpId);

			if (isProductGroupId == false) return;

			this.ProductGourpId =
				NumberingUtility
					.CreateNewNumber(
						this.SessionWrapper.LoginShopId,
						"ProductGroupId")
					.ToString()
					.PadLeft(10, '0');
			CreateProductGroupId(productGroupModels);
		}

		/// <summary>ページID</summary>
		public long? PageId
		{
			get { return this.SessionWrapper.PageId; }
			set { this.SessionWrapper.PageId = value; }
		}

		/// <summary>商品グループID</summary>
		private string ProductGourpId { get; set; }
	}
}
