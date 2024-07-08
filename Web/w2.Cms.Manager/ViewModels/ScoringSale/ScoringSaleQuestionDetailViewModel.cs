/*
=========================================================================================================
  Module      : Scoring Sale Question Detail View Model (ScoringSaleQuestionDetailViewModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using w2.Domain.ScoreAxis;

namespace w2.Cms.Manager.ViewModels.ScoringSale
{
	/// <summary>
	/// Scoring sale question detail view model
	/// </summary>
	public class ScoringSaleQuestionDetailViewModel : ViewModelBase
	{
		#region Contructor
		/// <summary>
		/// Constructor
		/// </summary>
		public ScoringSaleQuestionDetailViewModel()
		{
			this.ScoringSaleQuestionChoiceList = new List<ScoringSaleQuestionChoiceViewModel>();
		}
		#endregion

		#region Property
		/// <summary>質問ID</summary>
		public string QuestionId { get; set; }
		/// <summary>質問タイトル</summary>
		public string QuestionTitle { get; set; }
		/// <summary>スコア軸ID</summary>
		public string ScoreAxisId { get; set; }
		/// <summary>回答タイプ</summary>
		public string AnswerType { get; set; }
		/// <summary>質問文</summary>
		public string QuestionStatement { get; set; }
		/// <summary>Score axis</summary>
		public ScoreAxisModel ScoreAxis { get; set; }
		/// <summary>Scoring sale question choice list</summary>
		public List<ScoringSaleQuestionChoiceViewModel> ScoringSaleQuestionChoiceList { get; set; }
		#endregion
	}
}
