/*
=========================================================================================================
  Module      : Scoring Sale Question Search Param Model (ScoringSaleQuestionSearchParamModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Domain.Helper;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.ScoringSale.Helper
{
	/// <summary>
	/// Scoring sale question search param model
	/// </summary>
	[Serializable]
	public class ScoringSaleQuestionSearchParamModel : BaseDbMapModel
	{
		/// <summary>Search word</summary>
		[DbMapName("search_word")]
		public string SearchWord { get; set; }
		/// <summary>Begin row number</summary>
		[DbMapName(Constants.FIELD_COMMON_BEGIN_NUM)]
		public int? BeginRowNumber { get; set; }
		/// <summary>End row number</summary>
		[DbMapName(Constants.FIELD_COMMON_END_NUM)]
		public int? EndRowNumber { get; set; }
		/// <summary>Score axis id</summary>
		[DbMapName(Constants.FIELD_SCORINGSALEQUESTION_SCORE_AXIS_ID)]
		public string ScoreAxisId { get; set; }
	}
}
