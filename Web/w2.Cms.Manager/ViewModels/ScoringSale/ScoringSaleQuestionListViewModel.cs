/*
=========================================================================================================
  Module      : Scoring Sale Question List View Model (ScoringSaleQuestionListViewModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using w2.Cms.Manager.Codes;
using w2.Common.Util;

namespace w2.Cms.Manager.ViewModels.ScoringSale
{
	/// <summary>
	/// Scoring sale question list view model
	/// </summary>
	[Serializable]
	public class ScoringSaleQuestionListViewModel : ViewModelBase
	{
		#region Property
		/// <summary>Items</summary>
		public ScoringSaleQuestionListItemDetailViewModel[] Items { get; set; }

		/// <summary>Question type settings</summary>
		public KeyValuePair<string, string>[] AnswerTypeSettings
		{
			get
			{
				return ValueText.GetValueKvpArray(
					Constants.TABLE_SCORINGSALEQUESTION,
					Constants.FIELD_SCORINGSALEQUESTION_ANSWER_TYPE);
			}
		}
		#endregion
	}

	/// <summary>
	/// Scoring sale question list item detail view model
	/// </summary>
	[Serializable]
	public class ScoringSaleQuestionListItemDetailViewModel : ViewModelBase
	{
		#region Property
		/// <summary>質問ID</summary>
		public string QuestionId { get; set; }
		/// <summary>質問タイトル</summary>
		public string QuestionTitle { get; set; }
		/// <summary>スコア軸タイトル</summary>
		public string ScoreAxisTitle { get; set; }
		/// <summary>更新日(日付)</summary>
		public string DateChanged1 { get; set; }
		/// <summary>更新日(時間)</summary>
		public string DateChanged2 { get; set; }
		#endregion
	}
}
