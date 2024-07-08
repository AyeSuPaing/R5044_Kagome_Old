/*
=========================================================================================================
  Module      : Score Axis Worker Service (ScoreAxisWorkerService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.Util;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ViewModels.ScoreAxis;
using w2.Common.Util;
using w2.Domain;
using w2.Domain.ScoreAxis;
using w2.Domain.ScoreAxis.Helper;
using Constants = w2.Cms.Manager.Codes.Constants;

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// Score axis worker service
	/// </summary>
	public class ScoreAxisWorkerService : BaseWorkerService
	{
		/// <summary>
		/// Get list view
		/// </summary>
		/// <param name="SearchWord">Search word</param>
		/// <returns>Score axis list view model</returns>
		public ScoreAxisListViewModel GetListView(string searchWord)
		{
			var models = DomainFacade.Instance.ScoreAxisService.Search(searchWord);
			var result = new ScoreAxisListViewModel
			{
				Items = models.Select(CreateDetailView).ToArray()
			};

			return result;
		}

		/// <summary>
		/// Create detail view
		/// </summary>
		/// <param name="model">Score axis model</param>
		/// <returns>Detailed view</returns>
		private ScoreAxisItemDetailViewModel CreateDetailView(ScoreAxisModel model)
		{
			var dateChanged = DateTimeUtility.ToStringForManager(
					model.DateChanged,
					DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter)
				.Split(new[] { " " }, StringSplitOptions.None);

			var result = new ScoreAxisItemDetailViewModel
			{
				ScoreAxisId = model.ScoreAxisId,
				ScoreAxisTitle = model.ScoreAxisTitle,
				DateChanged1 = dateChanged[0],
				DateChanged2 = dateChanged[1],
			};

			return result;
		}

		/// <summary>
		/// Get score axis detail view model
		/// </summary>
		/// <param name="scoreAxisId">Score axis id</param>
		/// <returns>Score axis detail view model</returns>
		public ScoreAxisDetailViewModel GetScoreAxisDetailViewModel(string scoreAxisId)
		{
			var model = DomainFacade.Instance.ScoreAxisService.Get(scoreAxisId);
			var result = new ScoreAxisDetailViewModel
			{
				ScoreAxisId = model.ScoreAxisId,
				ScoreAxisTitle = model.ScoreAxisTitle,
				AxisNames = CreateListAxisName(model).ToArray(),
			};

			return result;
		}

		/// <summary>
		/// Create list axis name
		/// </summary>
		/// <param name="model">Score axis model</param>
		/// <returns>Axis names</returns>
		public List<string> CreateListAxisName(ScoreAxisModel model)
		{
			var axisNames = model.GetType().GetProperties()
				.Where(propertyInfo =>
				{
					 if (propertyInfo.Name.StartsWith("AxisName")
						&& propertyInfo.CanRead
						&& (propertyInfo.GetIndexParameters().Length == 0)) {
						var valueProperty = StringUtility.ToEmpty(propertyInfo.GetValue(model));
						return (string.IsNullOrEmpty(valueProperty) == false);
					 }

					return false;
				})
				.Select(propertyInfo =>
				{
					var valueProperty = StringUtility.ToEmpty(propertyInfo.GetValue(model));
					return valueProperty;
				})
				.ToList();

			return axisNames;
		}

		/// <summary>
		/// Register score axis data
		/// </summary>
		/// <param name="input">Score axis input</param>
		/// <returns>Error message</returns>
		public string RegisterScoreAxisData(ScoreAxisInput input)
		{
			SetValueForAxisName(input);
			var errorMessage = input.CreateErrorJoinMessage(input.Validate(true));
			if (string.IsNullOrEmpty(errorMessage) == false) return errorMessage;

			if (string.IsNullOrEmpty(input.ScoreAxisId))
			{
				input.ScoreAxisId = NumberingUtility
					.CreateNewNumber(base.SessionWrapper.LoginShopId, "ScoreAxisId")
					.ToString()
					.PadLeft(10, '0');
			}

			var model = input.CreateModel();
			model.LastChanged = this.SessionWrapper.LoginOperatorName;

			DomainFacade.Instance.ScoreAxisService.Insert(model);
			return string.Empty;
		}

		/// <summary>
		/// Update score axis data
		/// </summary>
		/// <param name="input">Score axis input</param>
		/// <returns>Error message</returns>
		public string UpdateScoreAxisData(ScoreAxisInput input)
		{
			SetValueForAxisName(input);
			var errorMessage = input.CreateErrorJoinMessage(input.Validate(false));
			if (string.IsNullOrEmpty(errorMessage) == false) return errorMessage;

			var model = input.CreateModel();
			model.LastChanged = base.SessionWrapper.LoginOperatorName;

			DomainFacade.Instance.ScoreAxisService.Update(model);
			return string.Empty;
		}

		/// <summary>
		/// Delete score axis data
		/// </summary>
		/// <param name="scoreAxisId">Score axis id</param>
		/// <returns>Error message</returns>
		public int DeleteScoreAxisData(string scoreAxisId)
		{
			var result = DomainFacade.Instance.ScoreAxisService.Delete(scoreAxisId);
			return result;
		}

		/// <summary>
		/// Set value for axis name
		/// </summary>
		/// <param name="input">Score axis input</param>
		public void SetValueForAxisName(ScoreAxisInput input)
		{
			var index = 0;
			foreach (var axisName in input.AxisNames)
			{
				var propertyInfo = input.GetType().GetProperty("AxisName" + (index + 1));
				propertyInfo.SetValue(input, input.AxisNames[index]);
				index++;
			}
		}

		/// <summary>
		/// Search score axis
		/// </summary>
		/// <param name="paramModel">Parameter model</param>
		/// <returns>Array of score axis result model</returns>
		public ScoreAxisResultViewModel[] SearchScoreAxis(ScoreAxisSearchParamModel paramModel)
		{
			paramModel.BeginRowNumber = (paramModel.PagerNo - 1) * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST + 1;
			paramModel.EndRowNumber = paramModel.PagerNo * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST;
			var scoreAxisList = DomainFacade.Instance.ScoreAxisService.SearchAtModal(paramModel);
			var totalCount = DomainFacade.Instance.ScoreAxisService.GetSearchHitCount();
			var beginRowNumber = paramModel.BeginRowNumber;
			var endRowNumber = ((totalCount > paramModel.EndRowNumber)
				? StringUtility.ToNumeric(paramModel.EndRowNumber)
				: StringUtility.ToNumeric(totalCount));

			var countHtml = string.Format(
				"{0}-{1}/{2}",
				beginRowNumber,
				endRowNumber,
				StringUtility.ToNumeric(totalCount));

			var result = scoreAxisList
				.Select(scoreAxis =>
				{
					var viewModel = scoreAxis.ToViewModel<ScoreAxisResultViewModel>();
					viewModel.CountHtml = countHtml;
					viewModel.DateChanged = DateTimeUtility.ToStringForManager(
						scoreAxis.DateChanged,
						DateTimeUtility.FormatType.ShortDate2Letter);
					return viewModel;
				})
				.ToArray();

			return result;
		}

		/// <summary>
		/// Get search hit count
		/// </summary>
		/// <returns>Count</returns>
		public int GetSearchHitCount()
		{
			var count = DomainFacade.Instance.ScoreAxisService.GetSearchHitCount();
			return count;
		}
	}
}
