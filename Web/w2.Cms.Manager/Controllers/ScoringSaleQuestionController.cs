/*
=========================================================================================================
  Module      : Scoring Sale Question Controller (ScoringSaleQuestionController.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Web.Mvc;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.WorkerServices;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// Scoring sale question controller
	/// </summary>
	public class ScoringSaleQuestionController : BaseController
	{
		/// <summary>
		/// Main
		/// </summary>
		/// <returns>View</returns>
		public ActionResult Main()
		{
			return View();
		}

		/// <summary>
		/// List
		/// </summary>
		/// <param name="searchWord">Search word</param>
		/// <returns>View</returns>
		public ActionResult List(string searchWord)
		{
			var scoringSaleQuestionListViewModel = this.Service.GetListViewModel(searchWord);
			return View(scoringSaleQuestionListViewModel);
		}

		/// <summary>
		/// Get scoring sale question list view model
		/// </summary>
		/// <param name="searchWord">Search word</param>
		/// <returns>JSON of scoring sale question list view model</returns>
		public ActionResult GetScoringSaleQuestionListViewModel(string searchWord)
		{
			var scoringSaleQuestionListViewModel = this.Service.GetListViewModel(searchWord);
			return Json(scoringSaleQuestionListViewModel);
		}

		/// <summary>
		/// Get scoring sale question detail view model
		/// </summary>
		/// <param name="questionId">Question id</param>
		/// <returns>JSON of scoring sale question detail view model</returns>
		public ActionResult GetScoringSaleQuestionDetailViewModel(string questionId)
		{
			var scoringSaleQuestionDetailViewModel = this.Service.GetDetailViewModel(questionId);
			return Json(scoringSaleQuestionDetailViewModel);
		}

		/// <summary>
		/// Register
		/// </summary>
		/// <param name="input">Input</param>
		/// <returns>Action result</returns>
		public ActionResult Register(ScoringSaleQuestionInput input)
		{
			var errorMessage = this.Service.Register(input);
			var result = string.IsNullOrEmpty(errorMessage)
				? Constants.CONST_RESPONSE_KEY_RESULT_OK
				: Constants.CONST_RESPONSE_KEY_RESULT_NG;

			var data = new Dictionary<string, string>
			{
				{ Constants.CONST_RESPONSE_KEY_RESULT, result },
				{ Constants.CONST_RESPONSE_KEY_MESSAGE, errorMessage },
				{ Constants.CONST_RESPONSE_KEY_ID, input.QuestionId },
			};

			return Json(data);
		}

		/// <summary>
		/// Update
		/// </summary>
		/// <param name="input">Input</param>
		/// <returns>JSON</returns>
		public ActionResult Update(ScoringSaleQuestionInput input)
		{
			var errorMessage = this.Service.Update(input);
			var result = string.IsNullOrEmpty(errorMessage)
				? Constants.CONST_RESPONSE_KEY_RESULT_OK
				: Constants.CONST_RESPONSE_KEY_RESULT_NG;

			var data = new Dictionary<string, string>
			{
				{ Constants.CONST_RESPONSE_KEY_RESULT, result },
				{ Constants.CONST_RESPONSE_KEY_MESSAGE, errorMessage },
				{ Constants.CONST_RESPONSE_KEY_ID, input.QuestionId },
			};

			return Json(data);
		}

		/// <summary>
		/// Delete
		/// </summary>
		/// <param name="questionId">Question id</param>
		/// <returns>JSON</returns>
		public ActionResult Delete(string questionId)
		{
			var errorMessage = this.Service.Delete(questionId);
			var result = string.IsNullOrEmpty(errorMessage)
				? Constants.CONST_RESPONSE_KEY_RESULT_OK
				: Constants.CONST_RESPONSE_KEY_RESULT_NG;

			var data = new Dictionary<string, string>
			{
				{ Constants.CONST_RESPONSE_KEY_RESULT, result },
				{ Constants.CONST_RESPONSE_KEY_MESSAGE, errorMessage },
				{ Constants.CONST_RESPONSE_KEY_ID, questionId },
			};

			return Json(data);
		}

		/// <summary>
		/// Image search
		/// </summary>
		/// <param name="searchWord">Search word</param>
		/// <returns>Partial view</returns>
		public ActionResult ImageSearch(string searchWord)
		{
			var scoringSaleQuestionChoiceImageViewModel = this.Service.ImageSearch(searchWord);
			return PartialView("_ImageDetail", scoringSaleQuestionChoiceImageViewModel);
		}

		/// <summary>
		/// Get search hit count
		/// </summary>
		/// <param name="scoreAxisId">Score axis id</param>
		/// <returns>Count</returns>
		public int GetSearchHitCount(string scoreAxisId)
		{
			var count = this.Service.GetSearchHitCountScoringSaleQuestion(scoreAxisId);
			return count;
		}

		/// <summary>
		/// Search scoring sale question
		/// </summary>
		/// <param name="pageNo">Page no</param>
		/// <param name="scoreAxisId">Score axis id</param>
		/// <returns>Action result</returns>
		public ActionResult SearchScoringSaleQuestion(int pageNo, string scoreAxisId)
		{
			var viewModel = this.Service.SearchScoringSaleQuestion(pageNo, scoreAxisId);
			return Json(viewModel);
		}

		/// <summary>Service</summary>
		private ScoringSaleQuestionWorkerService Service { get { return GetDefaultService<ScoringSaleQuestionWorkerService>(); } }
	}
}
