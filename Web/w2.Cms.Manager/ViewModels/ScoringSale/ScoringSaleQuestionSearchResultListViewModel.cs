/*
=========================================================================================================
  Module      : Scoring Sale Question Search Result Model (ScoringSaleQuestionSearchResultModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Util;
using w2.Cms.Manager.Codes;
using w2.Common.Util;
using w2.Domain.ScoringSale;

namespace w2.Cms.Manager.ViewModels.ScoringSale
{
	/// <summary>
	/// Scoring sale question search result list view model
	/// </summary>
	[Serializable]
	public class ScoringSaleQuestionSearchResultListViewModel
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ScoringSaleQuestionSearchResultListViewModel()
		{
			this.List = new ScoringSaleQuestionSearchResultModel[0];
			this.CountHtml = string.Empty;
		}

		/// <summary>Search results list view model</summary>
		public ScoringSaleQuestionSearchResultModel[] List { get; set; }
		/// <summary>Total number HTML</summary>
		public string CountHtml { get; set; }
	}

	/// <summary>
	/// Scoring sale question search result model
	/// </summary>
	public class ScoringSaleQuestionSearchResultModel
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ScoringSaleQuestionSearchResultModel()
		{
			this.QuestionId = string.Empty;
			this.Name = string.Empty;
			this.Statement = string.Empty;
			this.Type = string.Empty;
			this.UpdateDate = string.Empty;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="Scoring sale question model">model</param>
		public ScoringSaleQuestionSearchResultModel(ScoringSaleQuestionModel model)
			: this()
		{
			this.QuestionId = model.QuestionId;
			this.Name = model.QuestionTitle;
			this.Statement = model.QuestionStatement;
			this.Type = model.AnswerType;
			this.UpdateDate = DateTimeUtility.ToStringForManager(
				model.DateChanged,
				DateTimeUtility.FormatType.ShortDate2Letter);
		}

		/// <summary>Question id</summary>
		public string QuestionId { get; set; }
		/// <summary>Name</summary>
		public string Name { get; set; }
		/// <summary>Statement</summary>
		public string Statement { get; set; }
		/// <summary>Type</summary>
		public string Type { get; set; }
		/// <summary>Type text</summary>
		public string TypeText
		{
			get
			{
				var result = ValueText.GetValueText(Constants.TABLE_SCORINGSALEQUESTION, Constants.FIELD_SCORINGSALEQUESTION_ANSWER_TYPE, this.Type);
				return result;
			}
		}
		/// <summary>Update date</summary>
		public string UpdateDate { get; set; }
	}
}