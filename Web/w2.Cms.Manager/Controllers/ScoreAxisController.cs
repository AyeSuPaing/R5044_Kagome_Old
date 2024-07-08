/*
=========================================================================================================
  Module      : Score Axis Controller (ScoreAxisController.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Web.Mvc;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ViewModels.ScoreAxis;
using w2.Cms.Manager.WorkerServices;
using w2.Domain.ScoreAxis.Helper;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// Score axis controller
	/// </summary>
	public class ScoreAxisController : BaseController
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
			var viewModel = this.Service.GetListView(searchWord);
			return View(viewModel);
		}

		/// <summary>
		/// Get score axis list view model
		/// </summary>
		/// <param name="searchWord">Search word</param>
		/// <returns>Json of score axis list view model</returns>
		public ActionResult GetScoreAxisListViewModel(string searchWord)
		{
			var listView = this.Service.GetListView(searchWord);
			return Json(listView);
		}

		/// <summary>
		/// Get score axis default detail view model
		/// </summary>
		/// <returns>JSON of score axis detail view model</returns>
		public ActionResult GetScoreAxisDefaultDetailViewModel()
		{
			return Json(new ScoreAxisDetailViewModel());
		}

		/// <summary>
		/// Get score axis detail view model
		/// </summary>
		/// <param name="scoreAxisId">Score axis id</param>
		/// <returns>JSON of score axis detail view model</returns>
		public ActionResult GetScoreAxisDetailViewModel(string scoreAxisId)
		{
			var scoreAxisDetailViewModel = this.Service.GetScoreAxisDetailViewModel(scoreAxisId);
			return Json(scoreAxisDetailViewModel);
		}

		/// <summary>
		/// Register
		/// </summary>
		/// <param name="input">Score axis input</param>
		/// <returns>JSON</returns>
		public ActionResult Register(ScoreAxisInput input)
		{
			var errorMessage = this.Service.RegisterScoreAxisData(input);
			var result = string.IsNullOrEmpty(errorMessage)
				? Constants.CONST_RESPONSE_KEY_RESULT_OK
				: Constants.CONST_RESPONSE_KEY_RESULT_NG;

			var data = new Dictionary<string, string>
			{
				{ Constants.CONST_RESPONSE_KEY_RESULT, result },
				{ Constants.CONST_RESPONSE_KEY_MESSAGE, errorMessage },
				{ Constants.CONST_RESPONSE_KEY_ID, input.ScoreAxisId },
			};

			return Json(data);
		}

		/// <summary>
		/// Modify
		/// </summary>
		/// <param name="input">Score axis input</param>
		/// <returns>JSON</returns>
		public ActionResult Modify(ScoreAxisInput input)
		{
			var errorMessage = this.Service.UpdateScoreAxisData(input);
			var result = string.IsNullOrEmpty(errorMessage)
				? Constants.CONST_RESPONSE_KEY_RESULT_OK
				: Constants.CONST_RESPONSE_KEY_RESULT_NG;

			var data = new Dictionary<string, string>
			{
				{ Constants.CONST_RESPONSE_KEY_RESULT, result },
				{ Constants.CONST_RESPONSE_KEY_MESSAGE, errorMessage },
				{ Constants.CONST_RESPONSE_KEY_ID, input.ScoreAxisId },
			};

			return Json(data);
		}

		/// <summary>
		/// Delete
		/// </summary>
		/// <param name="scoreAxisId">Score axis id</param>
		/// <returns>JSON</returns>
		public ActionResult Delete(string scoreAxisId)
		{
			var number = this.Service.DeleteScoreAxisData(scoreAxisId);
			var result = (number > 0)
				? Constants.CONST_RESPONSE_KEY_RESULT_OK
				: Constants.CONST_RESPONSE_KEY_RESULT_NG;

			var data = new Dictionary<string, string>
			{
				{ Constants.CONST_RESPONSE_KEY_RESULT, result },
				{ Constants.CONST_RESPONSE_KEY_ID, scoreAxisId },
			};

			return Json(data);
		}

		/// <summary>
		/// Score axis search
		/// </summary>
		/// <param name="paramModel">Parameter model</param>
		/// <returns>Partial view</returns>
		public ActionResult SearchScoreAxis(ScoreAxisSearchParamModel paramModel)
		{
			var viewModel = this.Service.SearchScoreAxis(paramModel);
			return Json(viewModel);
		}

		/// <summary>
		/// Get search hit count
		/// </summary>
		/// <returns>Count</returns>
		public int GetSearchHitCount()
		{
			var count = this.Service.GetSearchHitCount();
			return count;
		}

		/// <summary>Service</summary>
		private ScoreAxisWorkerService Service { get { return GetDefaultService<ScoreAxisWorkerService>(); } }
	}
}
