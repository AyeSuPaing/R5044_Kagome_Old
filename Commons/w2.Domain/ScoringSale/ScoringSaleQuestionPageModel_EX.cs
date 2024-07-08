/*
=========================================================================================================
  Module      : ScoringSaleQuestionPageモデル (ScoringSaleQuestionPageModel_EX.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.ScoringSale
{
	/// <summary>
	/// ScoringSaleQuestionPageモデル
	/// </summary>
	public partial class ScoringSaleQuestionPageModel
	{
		#region プロパティ
		/// <summary>Scoring sale question page items</summary>
		public ScoringSaleQuestionPageItemModel[] ScoringSaleQuestionPageItems { get; set; }
		/// <summary>Is same as first page</summary>
		public bool IsSameAsFirstPage { get; set; }
		#endregion
	}
}
