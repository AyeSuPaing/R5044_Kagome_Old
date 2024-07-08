/*
=========================================================================================================
  Module      : Scoring Sale Question Worker Service (ScoringSaleQuestionWorkerService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Linq;
using w2.App.Common.Util;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ViewModels.ScoringSale;
using w2.Common.Util;
using w2.Domain;
using w2.Domain.ScoringSale.Helper;
using Constants = w2.Cms.Manager.Codes.Constants;

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// Scoring sale question worker service
	/// </summary>
	public class ScoringSaleQuestionWorkerService : BaseWorkerService
	{
		/// <summary>Scoring sale image root</summary>
		private string _scoringSaleImageRoot = Path.Combine(Constants.PHYSICALDIRPATH_CONTENTS_ROOT, Constants.PATH_SCORING_QUESTION_IMAGE);

		/// <summary>
		/// Get list view model
		/// </summary>
		/// <param name="searchWord">Search word</param>
		/// <returns>View model</returns>
		public ScoringSaleQuestionListViewModel GetListViewModel(string searchWord)
		{
			var searchParamModel = new ScoringSaleQuestionSearchParamModel
			{
				SearchWord = searchWord,
			};
			var scoringSaleQuestionList = DomainFacade.Instance.ScoringSaleService.SearchScoringSaleQuestion(searchParamModel);
			var listViewModel = new ScoringSaleQuestionListViewModel();

			listViewModel.Items = scoringSaleQuestionList.Select(scoringSaleQuestion =>
			{
				var viewModelItem = scoringSaleQuestion.ToViewModel<ScoringSaleQuestionListItemDetailViewModel>();
				var scoreAxis = DomainFacade.Instance.ScoreAxisService.Get(scoringSaleQuestion.ScoreAxisId);
				var dateChanged = DateTimeUtility
					.ToStringForManager(
						scoringSaleQuestion.DateChanged,
						DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter)
					.Split(
						new[] { " " },
						StringSplitOptions.None);

				viewModelItem.DateChanged1 = dateChanged[0];
				viewModelItem.DateChanged2 = dateChanged[1];
				viewModelItem.ScoreAxisTitle = ((scoreAxis != null)
					? scoreAxis.ScoreAxisTitle
					: string.Empty);

				return viewModelItem;
			}).ToArray();

			return listViewModel;
		}

		/// <summary>
		/// Get detail view model
		/// </summary>
		/// <param name="questionId">Question id</param>
		/// <returns>View model</returns>
		public ScoringSaleQuestionDetailViewModel GetDetailViewModel(string questionId)
		{
			var scoringSaleQuestion = DomainFacade.Instance.ScoringSaleService.GetScoringSaleQuestion(questionId);

			if (scoringSaleQuestion == null)
			{
				return new ScoringSaleQuestionDetailViewModel();
			}

			var scoreAxis = DomainFacade.Instance.ScoreAxisService.Get(scoringSaleQuestion.ScoreAxisId);
			var detailViewModel = scoringSaleQuestion.ToViewModel<ScoringSaleQuestionDetailViewModel>();
			var scoringSaleQuestionChoiceList = DomainFacade.Instance.ScoringSaleService.GetScoringSaleQuestionChoices(questionId);
			detailViewModel.ScoreAxis = scoreAxis;
			detailViewModel.ScoringSaleQuestionChoiceList = scoringSaleQuestionChoiceList
				.Select(item => item.ToViewModel<ScoringSaleQuestionChoiceViewModel>())
				.ToList();

			return detailViewModel;
		}

		/// <summary>
		/// Register
		/// </summary>
		/// <param name="input">Input</param>
		/// <returns>Error message</returns>
		public string Register(ScoringSaleQuestionInput input)
		{
			var errorMessage = input.Validate(true);
			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				return errorMessage;
			};

			var model = input.CreateModel();
			model.LastChanged = base.SessionWrapper.LoginOperatorName;
			model.QuestionId = StringUtility
				.ToEmpty(NumberingUtility.CreateNewNumber(
					base.SessionWrapper.LoginShopId,
					Constants.FIELD_SCORINGSALEQUESTION_QUESTION_ID))
				.PadLeft(10, '0');

			DomainFacade.Instance.ScoringSaleService.InsertScoringSaleQuestion(model);
			ImageUpload(input);
			input.QuestionId = model.QuestionId;

			return errorMessage;
		}

		/// <summary>
		/// Update
		/// </summary>
		/// <param name="input">Input</param>
		/// <returns>Error message</returns>
		public string Update(ScoringSaleQuestionInput input)
		{
			var errorMessage = input.Validate(false);
			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				return errorMessage;
			};

			var model = input.CreateModel();
			model.LastChanged = base.SessionWrapper.LoginOperatorName;

			DomainFacade.Instance.ScoringSaleService.UpdateScoringSaleQuestion(model);
			ImageUpload(input);

			return errorMessage;
		}

		/// <summary>
		/// Delete
		/// </summary>
		/// <param name="questionId">Question id</param>
		/// <returns>Error message</returns>
		public string Delete(string questionId)
		{
			DomainFacade.Instance.ScoringSaleService.DeleteScoringSaleQuestion(questionId);
			return string.Empty;
		}

		/// <summary>
		/// Image search
		/// </summary>
		/// <param name="searchWord">Search word</param>
		/// <returns>Array of view model</returns>
		public ScoringSaleImageViewModel[] ImageSearch(string searchWord)
		{
			var imageList = GetImageList(_scoringSaleImageRoot)
				.Where(image => (string.IsNullOrEmpty(searchWord) || image.FileName.Contains(searchWord)))
				.OrderBy(image => image.DataChanged)
				.ToArray();

			return imageList;
		}

		/// <summary>
		/// Image upload
		/// </summary>
		/// <param name="input">Input</param>
		private void ImageUpload(ScoringSaleQuestionInput input)
		{
			if (Directory.Exists(_scoringSaleImageRoot) == false)
			{
				Directory.CreateDirectory(_scoringSaleImageRoot);
			}

			input.ScoringSaleQuestionChoiceList
				.Where(item => ((item.IsCopyImage == false)
					&& (item.UploadFile != null)))
				.Select(item => item.UploadFile)
				.ToList()
				.ForEach(item =>
				{
					var pathScoringSaleImage = Path.Combine(
						_scoringSaleImageRoot,
						Path.GetFileName(item.FileName));

					if (File.Exists(pathScoringSaleImage) == false)
					{
						item.SaveAs(pathScoringSaleImage);
					}
				});
		}

		/// <summary>
		/// Get scoring sale question search hit count
		/// </summary>
		/// <param name="scoreAxisId">Score axis id</param>
		/// <returns>Number</returns>
		public int GetSearchHitCountScoringSaleQuestion(string scoreAxisId)
		{
			var totalCount = DomainFacade.Instance.ScoringSaleService.GetSearchHitCountScoringSaleQuestion(scoreAxisId);
			return totalCount;
		}

		/// <summary>
		/// Search scoring sale question
		/// </summary>
		/// <param name="pageNo">Page no</param>
		/// <param name="scoreAxisId">Score axis id</param>
		/// <returns>Scoring sale question search result list view model</returns>
		public ScoringSaleQuestionSearchResultListViewModel SearchScoringSaleQuestion(int pageNo, string scoreAxisId)
		{
			var searchParamModel = new ScoringSaleQuestionSearchParamModel
			{
				SearchWord = string.Empty,
				BeginRowNumber = (pageNo - 1) * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST + 1,
				EndRowNumber = pageNo * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST,
				ScoreAxisId = scoreAxisId,
			};
			var totalCount = GetSearchHitCountScoringSaleQuestion(scoreAxisId);
			var countHtml = searchParamModel.BeginRowNumber + "-"
				+ ((totalCount > searchParamModel.EndRowNumber) ? StringUtility.ToNumeric(searchParamModel.EndRowNumber) : StringUtility.ToNumeric(totalCount))
				+ "/" + StringUtility.ToNumeric(totalCount);

			var scoringSaleQuestions = DomainFacade.Instance.ScoringSaleService.SearchScoringSaleQuestion(searchParamModel);
			var results = scoringSaleQuestions
				.Select(item => new ScoringSaleQuestionSearchResultModel(item))
				.ToArray();

			var viewModels = new ScoringSaleQuestionSearchResultListViewModel
			{
				List = results,
				CountHtml = countHtml,
			};

			return viewModels;
		}
	}
}