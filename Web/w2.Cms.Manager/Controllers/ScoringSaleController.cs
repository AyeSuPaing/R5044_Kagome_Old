/*
=========================================================================================================
  Module      : Scoring Sale Controller (ScoringSaleController.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Web.Mvc;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.ParamModels.ScoringSale;
using w2.Cms.Manager.ViewModels.Shared;
using w2.Cms.Manager.WorkerServices;
using w2.Domain.Product.Helper;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// Get default scoring sale detail view model
	/// </summary>
	public class ScoringSaleController : BaseController
	{
		/// <summary>Result</summary>
		public const string RESULT = "result";
		/// <summary>Result OK</summary>
		public const string RESULT_OK = "ok";
		/// <summary>Result NG</summary>
		public const string RESULT_NG = "ng";
		/// <summary>Message</summary>
		public const string MESSAGE = "msg";
		/// <summary>Id</summary>
		public const string ID = "id";

		/// <summary>
		/// Main
		/// </summary>
		/// <returns>Action result</returns>
		public ActionResult Main()
		{
			return View();
		}

		/// <summary>
		/// Get default scoring sale detail view model
		/// </summary>
		/// <param name="paramModel">Scoring sale list param model</param>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <returns>Action result</returns>
		public ActionResult List(ScoringSaleListParamModel paramModel, string scoringSaleId)
		{
			var viewModel = this.Service.GetListView(paramModel);
			if (string.IsNullOrEmpty(scoringSaleId) == false)
			{
				viewModel.OpenDetailScoringSaleId = scoringSaleId;
			}
			return View(viewModel);
		}

		/// <summary>
		/// Get scoring sale list view model
		/// </summary>
		/// <param name="paramModel">Scoring sale list param model</param>
		/// <returns>Action result</returns>
		public ActionResult GetScoringSaleListViewModel(ScoringSaleListParamModel paramModel)
		{
			var viewModel = this.Service.GetListView(paramModel);
			return Json(viewModel);
		}

		/// <summary>
		/// Get default scoring sale detail view model
		/// </summary>
		/// <returns>Action result</returns>
		public ActionResult GetDefaultScoringSaleDetailViewModel()
		{
			var data = this.Service.GetDefaultScoringSaleDetailViewModel();
			return Json(data);
		}

		/// <summary>
		/// Register
		/// </summary>
		/// <param name="input">Input</param>
		/// <returns>Action result</returns>
		public ActionResult Register(ScoringSaleInput input)
		{
			var errorMessage = this.Service.RegisterScoringSaleData(input);
			var data = DataResult(
				string.IsNullOrEmpty(errorMessage) ? RESULT_OK : RESULT_NG,
				errorMessage,
				input.ScoringSaleId);
			return Json(data);
		}

		/// <summary>
		/// Update
		/// </summary>
		/// <param name="input">Input</param>
		/// <returns>Action result</returns>
		public ActionResult Update(ScoringSaleInput input)
		{
			var errorMessage = this.Service.UpdateScoringSaleData(input);
			var data = DataResult(
				string.IsNullOrEmpty(errorMessage) ? RESULT_OK : RESULT_NG,
				errorMessage,
				input.ScoringSaleId);
			return Json(data);
		}

		/// <summary>
		/// Get scoring sale detail view model
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <returns>Scoring sale detail view model</returns>
		public ActionResult GetScoringSaleDetailViewModel(string scoringSaleId)
		{
			var data = this.Service.GetScoringSale(scoringSaleId);
			return Json(data);
		}

		/// <summary>
		/// Get search hit count on cms
		/// </summary>
		/// <param name="paramModel">Search conditions</param>
		/// <returns>Result</returns>
		public override int GetSearchHitCountOnCms(ProductSearchParamModel paramModel)
		{
			var count = this.Service.GetSearchHitCountOnCms(paramModel);
			return count;
		}

		/// <summary>
		/// Product search
		/// </summary>
		/// <param name="paramModel">Search conditions</param>
		/// <returns>Result</returns>
		public override ActionResult ProductSearch(ProductSearchParamModel paramModel)
		{
			var viewModel = this.Service.ProductSearch(paramModel);
			return PartialView("_ProductVariationSearchResult", viewModel);
		}

		/// <summary>
		/// Set product variation
		/// </summary>
		/// <param name="products">Product information</param>
		/// <returns>Action result</returns>
		public ActionResult SetProductVariation(ProductSearchResultModel[] products)
		{
			var data = this.Service.GetScoringSaleProduct(products);
			return Json(data);
		}

		/// <summary>
		/// Delete
		/// </summary>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <returns>Data</returns>
		public ActionResult Delete(string scoringSaleId)
		{
			this.Service.DeleteScoringSaleData(scoringSaleId);
			var data = new Dictionary<string, string>
			{
				{ ID, scoringSaleId },
			};
			return Json(data);
		}

		/// <summary>
		/// Data result
		/// </summary>
		/// <param name="result">Result</param>
		/// <param name="message">Message</param>
		/// <param name="id">Id</param>
		/// <returns>Data</returns>
		public Dictionary<string, string> DataResult(string result, string message, string id)
		{
			var data = new Dictionary<string, string>
			{
				{ RESULT, result },
				{ MESSAGE, message },
				{ ID, id }
			};

			return data;
		}

		/// <summary>
		/// Create preview file list page
		/// </summary>
		/// <param name="input">Scoring sale input</param>
		/// <param name="designType">Design type</param>
		/// <returns>Data</returns>
		public ActionResult CreatePreviewFileListPage(ScoringSaleInput input, string designType)
		{
			var previewKey = this.Service.CreatePreviewFile(input, designType);
			var data = new Dictionary<string, string>
			{
				{ "topUseFlg", input.TopPageUseFlg.ToString() },
				{ Constants.REQUEST_KEY_PREVIEW_KEY, previewKey },
				{ "scoringSaleId", input.ScoringSaleId }
			};
			return Json(data);
		}

		/// <summary>
		/// Preview
		/// </summary>
		/// <param name="directory">Directory</param>
		/// <param name="topUseFlg">Top use flg</param>
		/// <param name="previewKey">Preview key</param>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <returns>Data</returns>
		public ActionResult Preview(
			string topUseFlg,
			string previewKey,
			string designType,
			string scoringSaleId)
		{
			var data = this.Service.CreatePreviewViewModel(
				topUseFlg,
				previewKey,
				designType,
				scoringSaleId);
			return View(data);
		}

		/// <summary>
		/// Get score axis axis no list
		/// </summary>
		/// <param name="scoreAxisId">Score axis id</param>
		/// <returns>Scoring sale detail view model</returns>
		public ActionResult GetScoreAxisAxisNoList(string scoreAxisId)
		{
			var data = this.Service.GetScoreAxisAxisNoList(scoreAxisId);
			return Json(data);
		}

		/// <summary>
		/// Get replacement tag
		/// </summary>
		/// <returns>Replacement tag list</returns>
		public ActionResult GetReplacementTag()
		{
			var data = this.Service.GetReplacementTag();
			return Json(data);
		}

		/// <summary>
		/// Image search
		/// </summary>
		/// <param name="searchWord">Search word</param>
		/// <returns>Partial view</returns>
		public ActionResult ImageSearch(string searchWord)
		{
			var scoringSaleImageViewModel = this.Service.ImageSearch(searchWord);
			return PartialView("_ImageDetail", scoringSaleImageViewModel);
		}

		/// <summary>
		/// スコアリング分析レポートモデル取得
		/// </summary>
		/// <param name="scoringSaleid">スコアリングID</param>
		/// <returns>Partial view</returns>
		public ActionResult ScoringSaleAnalysisReportModal(string scoringSaleid)
		{
			return PartialView("ScoringSaleAnalysisReportModal", scoringSaleid);
		}

		/// <summary>
		/// スコアリング分析レポート取得
		/// </summary>
		/// <param name="scoringSaleId">スコアリングID</param>
		/// <param name="targetPeriod">対象期間</param>
		/// <returns>スコアリングモデル情報</returns>
		public ActionResult GetScoringSaleReport(string scoringSaleId, int targetPeriod)
		{
			var scoringSaleListItemDetailViewModel = this.Service.GetScoringSaleReport(scoringSaleId, targetPeriod, true);
			return Json(scoringSaleListItemDetailViewModel);
		}

		/// <summary>Service</summary>
		private ScoringSaleWorkerService Service { get { return GetDefaultService<ScoringSaleWorkerService>(); } }
	}
}
