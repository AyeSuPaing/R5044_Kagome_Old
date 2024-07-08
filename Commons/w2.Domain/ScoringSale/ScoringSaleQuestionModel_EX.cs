/*
=========================================================================================================
  Module      : Scoring Sale Question Model (ScoringSaleQuestionModel_EX.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;

namespace w2.Domain.ScoringSale
{
	/// <summary>
	/// Scoring sale question model
	/// </summary>
	public partial class ScoringSaleQuestionModel
	{
		#region プロパティ
		/// <summary>Scoring sale question choice list</summary>
		public List<ScoringSaleQuestionChoiceModel> ScoringSaleQuestionChoiceList { get; set; }
		#endregion
	}
}
