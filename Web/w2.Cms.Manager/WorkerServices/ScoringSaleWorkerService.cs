/*
=========================================================================================================
  Module      : Scoring Sale Worker Service (ScoringSaleWorkerService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.WebPages.Html;
using w2.App.Common.Design;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Order;
using w2.App.Common.RefreshFileManager;
using w2.App.Common.ScoringSale;
using w2.App.Common.Util;
using w2.App.Common.Web.Page;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.ParamModels.ScoringSale;
using w2.Cms.Manager.ViewModels.ScoringSale;
using w2.Cms.Manager.ViewModels.Shared;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain;
using w2.Domain.ContentsLog;
using w2.Domain.Product.Helper;
using w2.Domain.ProductCategory;
using w2.Domain.ScoringSale;
using Constants = w2.Cms.Manager.Codes.Constants;

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// Scoring sale worker service
	/// </summary>
	public class ScoringSaleWorkerService : BaseWorkerService
	{
		/// <summary>Scoring sale page path</summary>
		private static string s_scoringSalePath = @"Contents\Scoring\";
		/// <summary>Scoring sale root</summary>
		private string _scoringSaleRoot = Path.Combine(Constants.PHYSICALDIRPATH_CONTENTS_ROOT, s_scoringSalePath);

		/// <summary>
		/// Get list view
		/// </summary>
		/// <param name="paramModel">Scoring sale list param model</param>
		/// <returns>Scoring sale list view model</returns>
		public ScoringSaleListViewModel GetListView(ScoringSaleListParamModel param)
		{
			var models = DomainFacade.Instance.ScoringSaleService.SearchScoringSale(
				param.SearchWord,
				param.SearchPublicDateKbn,
				param.SearchPublishStatus);

			var listViewModel = new ScoringSaleListViewModel();
			listViewModel.ActionStatus = ActionStatus.List;
			listViewModel.Items = models.Select(x => CreateDetailViewModel(x , 0)).ToArray();
			return listViewModel;
		}

		/// <summary>
		/// 分析レポート作成
		/// </summary>
		/// <param name="scoringSale">スコアリング情報</param>
		/// <param name="targetPeriodMonths">対象期間</param>
		/// <param name="isCreateChart">チャートを作成するか</param>
		/// <returns>スコアリング分析情報</returns>
		private ScoringSaleListItemDetailViewModel CreateDetailViewModel(ScoringSaleModel scoringSale, int targetPeriodMonths, bool isCreateChart = false)
		{
			var dateChanged = DateTimeUtility.ToStringForManager(
				scoringSale.DateChanged,
				DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter).Split(
					new[] { " " },
					StringSplitOptions.None);
			var isUseTopPage = (scoringSale.TopPageUseFlg == Constants.FLG_SCORINGSALE_TOP_PAGE_USE_FLG_ON);
			var pageFirstName = isUseTopPage
				? Constants.PAGE_FRONT_SCORINGSALE_TOP_PAGE_NAME
				: Constants.PAGE_FRONT_SCORINGSALE_QUESTION_PAGE_NAME;

			// Setting image for PC
			var thumbnailUrlPc = WebBrowserCapture.GetImageFilePath(
				CreateScoringSaleFilePath(
					scoringSale.ScoringSaleId,
					isUseTopPage,
					false),
				device: WebBrowserCapture.Device.Pc,
				delay: 100,
				iSizeH: 800,
				iSizeW: 800,
				bSizeH: 1280,
				bSizeW: 720);

			// Setting image for smart phone
			var thumbnailUrlSp = WebBrowserCapture.GetImageFilePath(
				CreateScoringSaleFilePath(
					scoringSale.ScoringSaleId,
					isUseTopPage,
					true),
				device: WebBrowserCapture.Device.Sp,
				delay: 100,
				iSizeH: 400,
				iSizeW: 400,
				bSizeH: 400,
				bSizeW: 800);

			// 集計日数計算
			var startDate = (targetPeriodMonths == 0) ? DateTime.Now.AddMonths(-3) : DateTime.Now.AddMonths(targetPeriodMonths * -1);
			var numberOfDays = (DateTime.Now - startDate).Days;

			var values = (isCreateChart)
				? DomainFacade.Instance.DispSummaryAnalysisService.GetCountPageScoringSales(
					Constants.CONST_DEFAULT_DEPT_ID,
					scoringSale.ScoringSaleId,
					Constants.PAGE_FRONT_SCORINGSALE_TOP_PAGE,
					Constants.PAGE_FRONT_SCORINGSALE_QUESTION_PAGE,
					Constants.PAGE_FRONT_SCORINGSALE_RESULT_PAGE,
					Constants.CONTROLLER_W2CMS_MANAGER_SCORING_SALE,
					startDate,
					numberOfDays)
				: new Dictionary<string, int>();

			var pages = DomainFacade.Instance.ScoringSaleService.GetScoringSaleQuestionPageName(scoringSale.ScoringSaleId);
			var contentsLogService = new ContentsLogService();
			var summary = contentsLogService.GetContentsSummaryData(
				Constants.FLG_CONTENTSLOG_CONTENTS_TYPE_SCORINGSALE,
				CreateContentsId(scoringSale.ScoringSaleId, pages.Length));
			var summaryToday = contentsLogService.GetContentsSummaryDataOfToday(
				Constants.FLG_CONTENTSLOG_CONTENTS_TYPE_SCORINGSALE,
				CreateContentsId(scoringSale.ScoringSaleId, pages.Length));

			var pageNames = new List<string>();
			if (isUseTopPage) pageNames.Add("Top");
			pageNames.AddRange(pages.Select(item => item.PageNo.ToString()));
			pageNames.Add("ResultPage");

			var valuePageUrl = CreateValuePageUrl(values, scoringSale.TopPageUseFlg);
			var valueForChart = SetValueForChart(valuePageUrl, pageNames.ToArray(), scoringSale);
			var count = ((summary.Any())
				? summary[0].CvCount : 0) + ((summaryToday.Any())
				? summaryToday[0].CvCount : 0);
			var viewCount = ((summary.Any())
				? summary[0].PvCount : 0) + ((summaryToday.Any())
				? summaryToday[0].PvCount : 0);

			var directoryPath = isUseTopPage
				? Constants.PAGE_FRONT_SCORINGSALE_TOP_PAGE
				: Constants.PAGE_FRONT_SCORINGSALE_QUESTION_PAGE;

			var urlPath = Path.Combine(
				Constants.PROTOCOL_HTTPS,
				Constants.SITE_DOMAIN,
				Constants.PATH_ROOT_FRONT_PC,
				directoryPath);

			var url = new UrlCreator(urlPath)
				.AddParam(Constants.REQUEST_KEY_SCORINGSALE_ID, scoringSale.ScoringSaleId)
				.CreateUrl();

			var listItemViewModel = new ScoringSaleListItemDetailViewModel
			{
				Count = count,
				Price = ((summary.Any()) ? summary[0].Price : 0) + ((summaryToday.Any()) ? summaryToday[0].Price : 0),
				DateChanged1 = dateChanged[0],
				DateChanged2 = dateChanged[1],
				ScoringSaleId = scoringSale.ScoringSaleId,
				PublishStatus = scoringSale.PublishStatus,
				ScoringSaleTitle = scoringSale.ScoringSaleTitle,
				ViewCount = viewCount,
				CVR = (viewCount == 0) ? "0" : RoundValue(((double)count / (double)viewCount) * 100).ToString(),
				UsePublicRange = ((scoringSale.PublicEndDatetime.HasValue) || (scoringSale.PublicStartDatetime.HasValue)),
				ThumbnailUrlPc = (File.Exists(Constants.PHYSICALDIRPATH_CMS_MANAGER + thumbnailUrlPc))
					? (Constants.PATH_ROOT_CMS + thumbnailUrlPc).Replace(@"\", "/")
					: Constants.PATH_ROOT_CMS + Constants.IMG_MANAGER_NO_IMAGE,
				ThumbnailUrlSp = (File.Exists(Constants.PHYSICALDIRPATH_CMS_MANAGER + thumbnailUrlSp))
					? (Constants.PATH_ROOT_CMS + thumbnailUrlSp).Replace(@"\", "/")
					: Constants.PATH_ROOT_CMS + Constants.IMG_MANAGER_NO_IMAGE,
				PageUrl = url,
				ScoringSaleChartViewModel = valueForChart,
			};

			return listItemViewModel;
		}

		/// <summary>
		/// Get default scoring sale detail view model
		/// </summary>
		/// <returns>Scoring sale detail view model</returns>
		public ScoringSaleDetailViewModel GetDefaultScoringSaleDetailViewModel()
		{
			var detail = new ScoringSaleDetailViewModel();
			return detail;
		}

		/// <summary>
		/// Register scoring sale data
		/// </summary>
		/// <param name="input">Scoring sale input</param>
		/// <returns>Check message empty on success</returns>
		public string RegisterScoringSaleData(ScoringSaleInput input)
		{
			// Check the input
			input.TopPageTitle = string.IsNullOrEmpty(input.TopPageTitle) ? string.Empty : input.TopPageTitle;
			input.TopPageSubTitle = string.IsNullOrEmpty(input.TopPageSubTitle) ? string.Empty : input.TopPageSubTitle;
			input.TopPageBtnCaption = string.IsNullOrEmpty(input.TopPageBtnCaption) ? string.Empty : input.TopPageBtnCaption;
			input.TopPageBody = string.IsNullOrEmpty(input.TopPageBody) ? string.Empty : input.TopPageBody;
			if (input.TopPageUseFlg == false)
			{
				input.TopPageTitle = null;
				input.TopPageSubTitle = null;
				input.TopPageBtnCaption = null;
				input.TopPageBody = null;
			}

			input.RadarChartTitle = string.IsNullOrEmpty(input.RadarChartTitle) ? string.Empty : input.RadarChartTitle;
			if (input.RadarChartUseFlg == false) input.RadarChartTitle = null;

			foreach (var questionPage in input.ScoringSaleQuestionPages)
			{
				if (questionPage.IsSameAsFirstPage == false) continue;

				questionPage.NextPageBtnCaption = null;
				questionPage.PreviousPageBtnCaption = null;
			}

			// Get error messages
			var errorMessages = input.Validate(true);
			if (errorMessages.Any())
			{
				return input.CreateErrorJoinMessage(errorMessages);
			}

			// File name check
			var scoringSaleService = DomainFacade.Instance.ScoringSaleService;

			// Set button wording setting is same as first page
			var previousPageBtnText = input.ScoringSaleQuestionPages[0].PreviousPageBtnCaption;
			var nextPageBtn = input.ScoringSaleQuestionPages[0].NextPageBtnCaption;
			foreach (var item in input.ScoringSaleQuestionPages)
			{
				if (item.IsSameAsFirstPage)
				{
					item.PreviousPageBtnCaption = previousPageBtnText;
					item.NextPageBtnCaption = nextPageBtn;
				}
			}

			var model = input.CreateModel();
			model.LastChanged = base.SessionWrapper.LoginOperatorName;
			model.ScoringSaleId = string.IsNullOrEmpty(this.SessionWrapper.ScoringSaleId)
				? NumberingUtility
					.CreateNewNumber(
						base.SessionWrapper.LoginShopId,
						Constants.NUMBER_KEY_CMS_SCORING_SALE_ID)
					.ToString()
					.PadLeft(10, '0')
				: this.SessionWrapper.ScoringSaleId;

			// Insert scoring sale
			scoringSaleService.InsertScoringSale(model);

			// Insert scoring sale question pages
			if (model.ScoringSaleQuestionPages.Length <= Constants.FLG_SCORINGSALE_QUESTION_PAGE_MAX_NUMBER_QUESTIONS_DISPLAY)
			{
				foreach (var scoringSaleQuestionPage in model.ScoringSaleQuestionPages)
				{
					scoringSaleQuestionPage.ScoringSaleId = model.ScoringSaleId;
					scoringSaleQuestionPage.LastChanged = base.SessionWrapper.LoginOperatorName;
					scoringSaleService.InsertScoringSaleQuestionPage(scoringSaleQuestionPage);
					var index = 1;
					if (scoringSaleQuestionPage.ScoringSaleQuestionPageItems.Length <= Constants.FLG_SCORINGSALE_QUESTION_PAGE_ITEM_MAX_NUMBER_QUESTIONS_DISPLAY)
					{
						foreach (var item in scoringSaleQuestionPage.ScoringSaleQuestionPageItems)
						{
							item.ScoringSaleId = model.ScoringSaleId;
							item.BranchNo = index;
							scoringSaleService.InsertScoringSaleQuestionPageItem(item);
							index++;
						}
					}
				}
			}

			// Insert scoring sale products
			foreach (var scoringSaleProduct in model.ScoringSaleProducts)
			{
				scoringSaleProduct.ScoringSaleId = model.ScoringSaleId;
				scoringSaleService.InsertScoringSaleProduct(scoringSaleProduct);
				var index = 1;
				foreach (var scoringSaleResultCondition in scoringSaleProduct.ScoringSaleResultConditions)
				{
					scoringSaleResultCondition.ScoringSaleId = model.ScoringSaleId;
					scoringSaleResultCondition.BranchNo = scoringSaleProduct.BranchNo;
					scoringSaleResultCondition.ConditionBranchNo = index;
					scoringSaleService.InsertScoringSaleResultCondition(scoringSaleResultCondition);
					index++;
				}
			}

			input.ScoringSaleId = model.ScoringSaleId;
			ImageUpload(input);
			this.SessionWrapper.ScoringSaleId = string.Empty;
			var pageFirstName = (model.TopPageUseFlg == Constants.FLG_SCORINGSALE_TOP_PAGE_USE_FLG_ON)
				? Constants.PAGE_FRONT_SCORINGSALE_TOP_PAGE_NAME
				: Constants.PAGE_FRONT_SCORINGSALE_QUESTION_PAGE_NAME;

			// Setting image for PC
			WebBrowserCapture.Create(
				Constants.PHYSICALDIRPATH_CMS_MANAGER,
				CreateScoringSaleFilePath(
					model.ScoringSaleId,
					input.TopPageUseFlg,
					false),
				device: WebBrowserCapture.Device.Pc,
				delay: 100,
				iSizeH: 800,
				iSizeW: 800,
				bSizeH: 1280,
				bSizeW: 720);

			// Setting image for smart phone
			if (DesignCommon.UseSmartPhone)
			{
				WebBrowserCapture.Create(
					Constants.PHYSICALDIRPATH_CMS_MANAGER,
					CreateScoringSaleFilePath(
						model.ScoringSaleId,
						input.TopPageUseFlg,
						DesignCommon.UseSmartPhone),
					device: WebBrowserCapture.Device.Sp,
					delay: 100,
					iSizeH: 400,
					iSizeW: 400,
					bSizeH: 400,
					bSizeW: 800);
			}

			return string.Empty;
		}

		/// <summary>
		/// Update scoring sale data
		/// </summary>
		/// <param name="input">Scoring sale input</param>
		/// <returns>Check message empty on success</returns>
		public string UpdateScoringSaleData(ScoringSaleInput input)
		{
			// Check the input
			input.TopPageTitle = string.IsNullOrEmpty(input.TopPageTitle) ? string.Empty : input.TopPageTitle;
			input.TopPageSubTitle = string.IsNullOrEmpty(input.TopPageSubTitle) ? string.Empty : input.TopPageSubTitle;
			input.TopPageBtnCaption = string.IsNullOrEmpty(input.TopPageBtnCaption) ? string.Empty : input.TopPageBtnCaption;
			input.TopPageBody = string.IsNullOrEmpty(input.TopPageBody) ? string.Empty : input.TopPageBody;
			if (input.TopPageUseFlg == false)
			{
				input.TopPageTitle = null;
				input.TopPageSubTitle = null;
				input.TopPageBtnCaption = null;
				input.TopPageBody = null;
			}

			input.RadarChartTitle = string.IsNullOrEmpty(input.RadarChartTitle) ? string.Empty : input.RadarChartTitle;
			if (input.RadarChartUseFlg == false) input.RadarChartTitle = null;

			foreach (var questionPage in input.ScoringSaleQuestionPages)
			{
				if (questionPage.IsSameAsFirstPage == false) continue;

				questionPage.NextPageBtnCaption = null;
				questionPage.PreviousPageBtnCaption = null;
			}

			// Get error messages
			var errorMessages = input.Validate(false);
			if (errorMessages.Any())
			{
				return input.CreateErrorJoinMessage(errorMessages);
			}

			// Directory name check
			var scoringSaleService = DomainFacade.Instance.ScoringSaleService;

			// Set button wording setting is same as first page
			var previousPageBtnText = input.ScoringSaleQuestionPages[0].PreviousPageBtnCaption;
			var nextPageBtn = input.ScoringSaleQuestionPages[0].NextPageBtnCaption;
			foreach (var item in input.ScoringSaleQuestionPages)
			{
				if (item.IsSameAsFirstPage)
				{
					item.PreviousPageBtnCaption = previousPageBtnText;
					item.NextPageBtnCaption = nextPageBtn;
				}
			}

			var model = input.CreateModel();
			model.LastChanged = base.SessionWrapper.LoginOperatorName;
			foreach (var questionPage in model.ScoringSaleQuestionPages)
			{
				questionPage.LastChanged = model.LastChanged;
				var questionBranchNo = 1;
				foreach (var scoringSaleQuestionPageItem in questionPage.ScoringSaleQuestionPageItems)
				{
					scoringSaleQuestionPageItem.BranchNo = questionBranchNo;
					questionBranchNo++;
				}
			}

			var productBranchNo = 1;
			foreach (var scoringSaleProduct in model.ScoringSaleProducts)
			{
				scoringSaleProduct.BranchNo = productBranchNo;
				var conditionBranchNo = 1;
				foreach (var scoringSaleResultCondition in scoringSaleProduct.ScoringSaleResultConditions)
				{
					scoringSaleResultCondition.BranchNo = productBranchNo;
					scoringSaleResultCondition.ConditionBranchNo = conditionBranchNo;
					conditionBranchNo++;
				}
				productBranchNo++;
			}

			// Update
			if (scoringSaleService.UpdateScoringSale(model) > 0)
			{
				// Cache update
				RefreshFileManagerProvider.GetInstance(RefreshFileType.ScoringSale).CreateUpdateRefreshFile();
			};

			ImageUpload(input);
			this.SessionWrapper.ScoringSaleId = string.Empty;

			var pageFirstName = (model.TopPageUseFlg == Constants.FLG_SCORINGSALE_TOP_PAGE_USE_FLG_ON)
				? Constants.PAGE_FRONT_SCORINGSALE_TOP_PAGE_NAME
				: Constants.PAGE_FRONT_SCORINGSALE_QUESTION_PAGE_NAME;

			WebBrowserCapture.Create(
				Constants.PHYSICALDIRPATH_CMS_MANAGER,
				CreateScoringSaleFilePath(
					model.ScoringSaleId,
					input.TopPageUseFlg,
					false),
				device: WebBrowserCapture.Device.Pc,
				delay: 100,
				iSizeH: 800,
				iSizeW: 800,
				bSizeH: 1280,
				bSizeW: 720);

			if (DesignCommon.UseSmartPhone)
			{
				WebBrowserCapture.Create(
					Constants.PHYSICALDIRPATH_CMS_MANAGER,
					CreateScoringSaleFilePath(
						model.ScoringSaleId,
						input.TopPageUseFlg,
						DesignCommon.UseSmartPhone),
					device: WebBrowserCapture.Device.Sp,
					delay: 100,
					iSizeH: 400,
					iSizeW: 400,
					bSizeH: 400,
					bSizeW: 800);
			}

			return string.Empty;
		}

		/// <summary>
		/// Get couseling detail
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <returns>A scoring sale detail view model</returns>
		public ScoringSaleDetailViewModel GetScoringSale(string scoringSaleId)
		{
			var scoringSaleService = DomainFacade.Instance.ScoringSaleService;
			var scoringSale = scoringSaleService.GetScoringSale(scoringSaleId);
			var scoreAxis = DomainFacade.Instance.ScoreAxisService.Get(scoringSale.ScoreAxisId);
			scoringSale.ScoreAxisTitle = scoreAxis.ScoreAxisTitle;

			// Set scoring sale question pages
			scoringSale.ScoringSaleQuestionPages = scoringSaleService.GetScoringSaleQuestionPages(scoringSaleId);
			var scoringSaleQuestionFirstPage = scoringSale.ScoringSaleQuestionPages.First();
			var previousPageBtnText = scoringSaleQuestionFirstPage.PreviousPageBtnCaption;
			var nextPageBtn = scoringSaleQuestionFirstPage.NextPageBtnCaption;
			foreach (var scoringSaleQuestionPage in scoringSale.ScoringSaleQuestionPages)
			{
				scoringSaleQuestionPage.ScoringSaleQuestionPageItems = scoringSaleService.GetScoringSaleQuestionPageItems(scoringSaleId);
				scoringSaleQuestionPage.ScoringSaleQuestionPageItems = scoringSaleQuestionPage.ScoringSaleQuestionPageItems
					.Where(scoringSaleQuestionItem => (scoringSaleQuestionItem.PageNo == scoringSaleQuestionPage.PageNo))
					.OrderBy(item => item.BranchNo)
					.ToArray();

				// Get question id
				var questionIds = string.Empty;
				foreach (var scoringSaleQuestionPageItem in scoringSaleQuestionPage.ScoringSaleQuestionPageItems)
				{
					questionIds = (string.IsNullOrEmpty(questionIds)
						? scoringSaleQuestionPageItem.QuestionId
						: string.Format("{0},{1}", questionIds, scoringSaleQuestionPageItem.QuestionId));
				}

				// Set scoring sale question items
				var questionsIdArr = questionIds.Replace("\"", string.Empty).Split(',');
				var questions = scoringSaleService.GetScoringSaleQuestions(questionsIdArr);
				foreach (var scoringSaleQuestionPageItem in scoringSaleQuestionPage.ScoringSaleQuestionPageItems)
				{
					if (questions.Length != 0)
					{
						var question = questions.Where(item => (item.QuestionId == scoringSaleQuestionPageItem.QuestionId)).FirstOrDefault();
						scoringSaleQuestionPageItem.Name = question.QuestionTitle;
						scoringSaleQuestionPageItem.Statement = question.QuestionStatement;
						scoringSaleQuestionPageItem.Type = question.AnswerType;
						scoringSaleQuestionPageItem.UpdateDate = DateTimeUtility.ToStringForManager(
							question.DateChanged,
							DateTimeUtility.FormatType.ShortDate2Letter);
					}
				}

				// Check button wording setting
				scoringSaleQuestionPage.IsSameAsFirstPage = ((scoringSaleQuestionPage.PageNo != 1)
					&& (scoringSaleQuestionPage.PreviousPageBtnCaption == previousPageBtnText)
					&& (scoringSaleQuestionPage.NextPageBtnCaption == nextPageBtn));
			}

			// Set scoring sale page result
			scoringSale.ScoringSaleProducts = scoringSaleService.GetScoringSaleProducts(scoringSaleId, Constants.CONST_DEFAULT_SHOP_ID);
			foreach (var scoringSaleProduct in scoringSale.ScoringSaleProducts)
			{
				var product = ProductCommon.GetProductInfo(scoringSaleProduct.ShopId, scoringSaleProduct.ProductId, null);
				if (product.Count != 0)
				{
					scoringSaleProduct.ProductImage = CreateProductImageUrl(product[0]);
					scoringSaleProduct.ProductName = ProductCommon.CreateProductJointName(product[0]);
				}

				var resultConditions = scoringSaleService.GetScoringSaleResultConditions(
					scoringSaleProduct.ScoringSaleId,
					scoringSaleProduct.BranchNo);
				var getGroupsNo = resultConditions.Select(condition => condition.GroupNo);
				// Check is group
				var groupBrandNo = 1;
				foreach (var resultCondition in resultConditions)
				{
					var index = resultConditions.ToList().IndexOf(resultCondition);
					var groupNo = resultCondition.GroupNo;
					resultCondition.IsGroup = (resultConditions.Count(item => (item.GroupNo == groupNo)) > 1);
					if (getGroupsNo.Count(item => (item == groupNo)) > 1)
					{
						if ((index == 0)
							|| ((index > 0)
								&& (resultCondition.GroupNo == resultConditions[index - 1].GroupNo)))
						{
							resultCondition.GroupBrandNo = groupBrandNo;
						}
						else
						{
							resultCondition.GroupBrandNo = 1;
							groupBrandNo = 1;
						}

						groupBrandNo++;
					}
					else
					{
						groupBrandNo = 1;
					}
				}

				scoringSaleProduct.ScoringSaleResultConditions = resultConditions;
			}

			var dataDetail = new ScoringSaleDetailViewModel(scoringSale);
			dataDetail.ScoreAxisAxisNoList = GetScoreAxisAxisNoList(scoringSale.ScoreAxisId);
			return dataDetail;
		}

		/// <summary>
		/// Get search hit count on cms
		/// </summary>
		/// <param name="paramModel">Product search param model</param>
		/// <returns>The total number of cases</returns>
		public int GetSearchHitCountOnCms(ProductSearchParamModel paramModel)
		{
			paramModel.BeginRowNumber = (paramModel.PagerNo - 1) * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST + 1;
			paramModel.EndRowNumber = paramModel.PagerNo * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST;

			var count = DomainFacade.Instance.ProductService.GetSearchVariationHitCountOnCms(paramModel, this.SessionWrapper.LoginShopId);
			return count;
		}

		/// <summary>
		/// Product search
		/// </summary>
		/// <param name="paramModel">Product search param model</param>
		/// <returns>Array product search result model</returns>
		public ProductSearchResultModel[] ProductSearch(ProductSearchParamModel paramModel)
		{
			paramModel.BeginRowNumber = (paramModel.PagerNo - 1) * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST + 1;
			paramModel.EndRowNumber = paramModel.PagerNo * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST;
			var totalCount = DomainFacade.Instance.ProductService.GetSearchVariationHitCountOnCms(paramModel, this.SessionWrapper.LoginShopId);
			var countHtml = string.Format(
				"{0}-{1}/{2}",
				paramModel.BeginRowNumber,
				((totalCount > paramModel.EndRowNumber)
					? StringUtility.ToNumeric(paramModel.EndRowNumber)
					: StringUtility.ToNumeric(totalCount)),
				StringUtility.ToNumeric(totalCount));

			var productList = DomainFacade.Instance.ProductService.SearchVariationOnCms(paramModel, this.SessionWrapper.LoginShopId);
			var service = new ProductCategoryService();
			var viewModel = productList.Select(
				product => new ProductSearchResultModel
				{
					ProductName = product.Name,
					VariationName = product.VariationName1,
					ProductId = product.ProductId,
					VariationId = product.VariationId,
					CategoryName = string.IsNullOrEmpty(product.CategoryId1) == false
						? service.Get(product.CategoryId1) != null ? service.Get(product.CategoryId1).Name : string.Empty
						: string.Empty,
					Price = product.Price.ToPriceString(true),
					SellStartDate = DateTimeUtility.ToStringForManager(
						product.SellFrom,
						DateTimeUtility.FormatType.ShortDate2Letter),
					CountHtml = countHtml,
					Controller = Constants.CONTROLLER_W2CMS_MANAGER_SCORING_SALE
				}).ToArray();
			return viewModel;
		}

		/// <summary>
		/// Create product image url
		/// </summary>
		/// <param name="data">Data row view</param>
		/// <returns>Image url</returns>
		private string CreateProductImageUrl(object data)
		{
			var imgHead = StringUtility.ToEmpty(ProductCommon.GetKeyValue(
				data,
				Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD));
			var imageUrl = Constants.PROTOCOL_HTTPS + Constants.SITE_DOMAIN + Constants.PATH_ROOT_FRONT_PC
				+ (string.IsNullOrEmpty(imgHead)
					? string.Format("{0}{1}", Constants.PATH_PRODUCTIMAGES, Constants.PRODUCTIMAGE_NOIMAGE_PC)
					: string.Format("{0}0/{1}{2}", Constants.PATH_PRODUCTIMAGES, imgHead, Constants.PRODUCTIMAGE_FOOTER_M));
			return imageUrl;
		}

		/// <summary>
		/// Get scoring sale product
		/// </summary>
		/// <param name="selectedProducts">Select product</param>
		/// <returns>A scoring sale product view model</returns>
		public ScoringSaleProductViewModel GetScoringSaleProduct(ProductSearchResultModel[] selectedProducts)
		{
			var product = selectedProducts[0];
			var model = DomainFacade.Instance.ProductService.GetProductVariation(
				this.SessionWrapper.LoginShopId,
				product.ProductId,
				product.VariationId,
				string.Empty);

			var result = new ScoringSaleProductViewModel();
			result.ImageUrl = CreateProductImageUrl(model.DataSource);
			result.ProductName = ProductCommon.CreateProductJointName(model.DataSource);
			result.ProductId = model.ProductId;
			result.ShopId = model.ShopId;
			result.VariationId = model.VariationId;

			return result;
		}

		/// <summary>
		/// Create value page url
		/// </summary>
		/// <param name="values">Values</param>
		/// <param name="topPageUseFlag">Top page use flag</param>
		/// <returns>Array decimal</returns>
		public Dictionary<string, double> CreateValuePageUrl(
			Dictionary<string, int> values,
			string topPageUseFlag)
		{
			var listPageExitRate = new Dictionary<string, double>();

			var page_exit = 0d;

			if (values.Keys.Count != 0)
			{
				var pageExitRateKey = (topPageUseFlag != Constants.FLG_SCORINGSALE_TOP_PAGE_USE_FLG_ON)
					? Constants.PAGE_FRONT_SCORINGSALE_QUESTION_PAGE_NAME.Split('.')[0]
					: Constants.PAGE_FRONT_SCORINGSALE_TOP_PAGE_NAME.Split('.')[0];
				listPageExitRate[pageExitRateKey] = 0;
			}

			var sortValues = SortDispSummaryAnalysisOfValueNames(values);

			for (int index = 0; index < (sortValues.Count - 1); index++)
			{
				page_exit = (Math.Abs(((double)sortValues.Values.ElementAt(index + 1) / (double)sortValues.Values.ElementAt(index)) - 1)) * 100;
				page_exit = Math.Round(page_exit * 100) / 100;
				listPageExitRate.Add(SetNameForPage(sortValues.Keys.ElementAt(index + 1)), page_exit);
			}

			return listPageExitRate;
		}

		/// <summary>
		/// Set name for page
		/// </summary>
		/// <param name="pageUrl">Page url</param>
		/// <returns>Name of page</returns>
		public string SetNameForPage(string pageUrl)
		{
			if (pageUrl.Contains(Constants.PAGE_FRONT_SCORINGSALE_RESULT_PAGE_NAME))
			{
				var indexResultPage = pageUrl.LastIndexOf("/");
				var nameResultPage = pageUrl.Substring(indexResultPage + 1, pageUrl.Length - indexResultPage - 6);
				return nameResultPage;
			}

			var index = pageUrl.LastIndexOf("=");
			var name = pageUrl.Substring(index + 1, pageUrl.Length - index - 1);
			return name;
		}

		/// <summary>
		/// Set name for page
		/// </summary>
		/// <param name="model">Scoring sale question page model</param>
		/// <returns>Array item of name</returns>
		public string[] SetNameForPage(ScoringSaleQuestionPageModel[] model)
		{
			var listName = model
				.Select(item => item.PageNo)
				.ToList();
			listName.Sort();
			var listNamePage = listName
				.Select(item => string.Format("Question{0}", item))
				.ToArray();
			return listNamePage;
		}

		/// <summary>
		/// Set value for chart
		/// </summary>
		/// <param name="values">Values</param>
		/// <param name="pages">Pages</param>
		/// <param name="model">Scoring sale model</param>
		/// <returns>Value for chart</returns>
		public ScoringSaleChartViewModel[] SetValueForChart(
			Dictionary<string, double> values,
			IEnumerable<string> pages,
			ScoringSaleModel model)
		{
			var list = pages.Select(item => new ScoringSaleChartViewModel
			{
				PageName = item,
				PageExitRate = 0,
			}).ToList();

			foreach (var item in list)
			{
				if (values.Keys.Any(x => (x == item.PageName)))
				{
					item.PageExitRate = values[item.PageName];
				}
				item.PageName = CreateNameReplaceTag(item.PageName);
			}

			return list.ToArray();
		}

		/// <summary>
		/// Delete scoring sale data
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		public void DeleteScoringSaleData(string scoringSaleId)
		{
			var service = DomainFacade.Instance.ScoringSaleService;
			var model = service.GetPageData(scoringSaleId);

			service.DeletePageData(scoringSaleId);
		}

		/// <summary>
		/// Create name replace tag
		/// </summary>
		/// <param name="name">Name</param>
		/// <returns>Name with replace tag</returns>
		public string CreateNameReplaceTag(string name)
		{
			switch (name)
			{
				case "Top":
					return CommonPage.ReplaceTag("@@ScoringSale.chart.top_page_title.name@@");

				case "ResultPage":
					return CommonPage.ReplaceTag("@@ScoringSale.chart.page_result_title.name@@");

				default:
					var nameQuestion = string.Format("{0} {1}", CommonPage.ReplaceTag("@@ScoringSale.chart.page_title.name@@"), name);
					return nameQuestion;
			}
		}

		/// <summary>
		/// Round value
		/// </summary>
		/// <param name="value">Value</param>
		/// <returns>Round number</returns>
		public double RoundValue(double value)
		{
			return (Math.Round(value, MidpointRounding.AwayFromZero));
		}

		/// <summary>
		/// Create contents id
		/// </summary>
		/// <param name="contentId">Content id</param>
		/// <param name="number">Number of page</param>
		/// <returns>Array item of content id</returns>
		public string[] CreateContentsId(string contentId, int number)
		{
			var listContentId = new List<string>()
			{
				contentId
			};

			for (int index = 1; index <= number; index++)
			{
				listContentId.Add(string.Format("{0}_{1}", contentId, index));
			}

			return listContentId.ToArray();
		}

		/// <summary>
		/// Set name replace tag
		/// </summary>
		/// <returns>Names</returns>
		public Dictionary<string, string> CreateNameReplaceTag()
		{
			var names = new Dictionary<string, string>();
			names.Add("Top", CommonPage.ReplaceTag("@@ScoringSale.chart.top_page_title.name@@"));
			for (int index = 1; index <= 10; index++)
			{
				names.Add(
					string.Format("Question{0}", index),
					string.Format("{0} {1}", CommonPage.ReplaceTag("@@ScoringSale.chart.page_title.name@@"), index));
			}
			names.Add("PageResult", CommonPage.ReplaceTag("@@ScoringSale.chart.page_result_title.name@@"));
			return names;
		}

		/// <summary>
		/// Set name replace tag
		/// </summary>
		/// <param name="name">Name</param>
		/// <param name="values">Values</param>
		/// <returns>Name replace tag</returns>
		public Dictionary<string, double> SetNameReplaceTag(Dictionary<string, string> name, Dictionary<string, double> values)
		{
			var nameReplaceTag = new Dictionary<string, double>();
			foreach (var item1 in name)
			{
				foreach (var item2 in values)
				{
					if (item2.Key == item1.Key)
					{
						nameReplaceTag.Add(item1.Value, item2.Value);
					}
				}
			}

			return nameReplaceTag;
		}

		/// <summary>
		/// Create preview file
		/// </summary>
		/// <param name="input">Scoring sale input</param>
		/// <param name="designType">Design type</param>
		/// <returns>Key</returns>
		public string CreatePreviewFile(ScoringSaleInput input, string designType)
		{
			var previewModel = DomainFacade.Instance.ScoringSaleService.GetPageData(input.ScoringSaleId);
			var key = ScoringSaleDesignHelper.CreatePreviewFileData(previewModel, designType);
			return key;
		}

		/// <summary>
		/// Create preview view model
		/// </summary>
		/// <param name="topUseFlg">Top use flag</param>
		/// <param name="previewKey">Preview key</param>
		/// <param name="designType">Design type</param>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <returns>Preview url</returns>
		public PreviewViewModel CreatePreviewViewModel(
			string topUseFlg,
			string previewKey,
			string designType,
			string scoringSaleId)
		{
			var url = ScoringSaleDesignHelper.GetPreviewUrl(
				topUseFlg,
				previewKey,
				designType,
				scoringSaleId);
			var preview = new PreviewViewModel
			{
				PreviewUrl = url,
			};
			return preview;
		}

		/// <summary>
		/// Get score axis axis no list
		/// </summary>
		/// <param name="scoreAxisId">Score axis id</param>
		/// <returns>Scoring sale detail view model</returns>
		public SelectListItem[] GetScoreAxisAxisNoList(string scoreAxisId)
		{
			var scoreAxis = DomainFacade.Instance.ScoreAxisService.Get(scoreAxisId);
			var scoreAxisAxisNoList = Enumerable.Range(1, 15)
				.Select(no =>
					{
						var nameNo = "AxisName" + no;
						var value = scoreAxis.GetType().GetProperty(nameNo).GetValue(scoreAxis, null).ToString();
						var item = new SelectListItem()
						{
							Text = value,
							Value = no.ToString()
						};
						return item;
					})
				.Where(item => (string.IsNullOrEmpty(item.Text) == false));
			return scoreAxisAxisNoList.ToArray();
		}

		/// <summary>
		/// Get replacement tag
		/// </summary>
		/// <returns>Replacement tag list</returns>
		public KeyValuePair<string, string>[] GetReplacementTag()
		{
			return ValueText.GetValueKvpArray(
				Constants.TABLE_SCORINGSALE,
				Constants.VALUETEXT_SCORINGSALE_REPLACEMENT_TAG);
		}

		/// <summary>
		/// Sort disp summary analysis of value names
		/// </summary>
		/// <param name="valueNames">Value names</param>
		/// <returns>Sort for Value names</returns>
		private IDictionary<string, int> SortDispSummaryAnalysisOfValueNames(IDictionary<string, int> valueNames)
		{
			if (valueNames.Count == 0) return valueNames;

			var maxValue = valueNames.Max(item =>
			{
				var pageNo1 = 0;

				if (int.TryParse(item.Key, out pageNo1))
				{
					return pageNo1;
				}

				return 1;
			});

			var result = valueNames
				.OrderBy(item =>
				{
					var pageNo = 0;

					if (int.TryParse(item.Key, out pageNo))
					{
						return pageNo;
					}

					var other = (Constants.PAGE_FRONT_SCORINGSALE_TOP_PAGE_NAME.StartsWith(item.Key))
						? -1
						: maxValue + 1;

					return other;
				})
				.ToDictionary(item => item.Key, item => item.Value);

			return result;
		}

		/// <summary>
		/// Create scoring sale file path
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <param name="isUseTopPage">Is use top page</param>
		/// <param name="isSmartPhone">Is smart phone</param>
		/// <returns>File path</returns>
		public static string CreateScoringSaleFilePath(
			string scoringSaleId,
			bool isUseTopPage,
			bool isSmartPhone)
		{
			var targetPageUrl = new StringBuilder();

			var page = isUseTopPage
				? Constants.PAGE_FRONT_SCORINGSALE_TOP_PAGE
				: Constants.PAGE_FRONT_SCORINGSALE_QUESTION_PAGE;

			if (isSmartPhone) targetPageUrl.Append("SmartPhone/");

			targetPageUrl.Append(page);

			var url = new UrlCreator(targetPageUrl.ToString());
			url.AddParam(Constants.REQUEST_KEY_SCORINGSALE_ID, scoringSaleId);

			return url.CreateUrl();
		}

		/// <summary>
		/// Image search
		/// </summary>
		/// <param name="searchWord">Search word</param>
		/// <returns>Array of view model</returns>
		public ScoringSaleImageViewModel[] ImageSearch(string searchWord)
		{
			var imageList = GetImageList(_scoringSaleRoot)
				.Where(image => (string.IsNullOrEmpty(searchWord) || image.FileName.Contains(searchWord)))
				.OrderBy(image => image.DataChanged)
				.ToArray();

			return imageList;
		}

		/// <summary>
		/// Image upload
		/// </summary>
		/// <param name="input">Input</param>
		private void ImageUpload(ScoringSaleInput input)
		{
			if (Directory.Exists(_scoringSaleRoot) == false)
			{
				Directory.CreateDirectory(_scoringSaleRoot);
			}

			if (input.TopPageUseFlg
				&& (input.IsCopyImage == false)
				&& (input.UploadFile != null))
			{
				var pathScoringSaleImage = Path.Combine(
					_scoringSaleRoot,
					Path.GetFileName(input.UploadFile.FileName));

				if (File.Exists(pathScoringSaleImage) == false)
				{
					input.UploadFile.SaveAs(pathScoringSaleImage);
				}
			}
		}

		/// <summary>
		/// スコアリング分析レポート取得
		/// </summary>
		/// <param name="scoringSaleId">スコアリングID</param>
		/// <param name="targetPeriodMonths">対象期間</param>
		/// <param name="isCreateChart">チャートを作成するか</param>
		/// <returns>スコアリング情報</returns>
		public ScoringSaleListItemDetailViewModel GetScoringSaleReport(string scoringSaleId, int targetPeriodMonths = 0, bool isCreateChart = false)
		{
			var scoringSale = DomainFacade.Instance.ScoringSaleService.GetScoringSale(scoringSaleId);
			var scoringSaleListItemDetailViewModel = CreateDetailViewModel(scoringSale, targetPeriodMonths, isCreateChart);
			return scoringSaleListItemDetailViewModel;
		}
	}
}