/*
=========================================================================================================
  Module      : ScoringSaleQuestionPageItemモデル (ScoringSaleQuestionPageItemModel_EX.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.ScoringSale
{
	/// <summary>
	/// ScoringSaleQuestionPageItemモデル
	/// </summary>
	public partial class ScoringSaleQuestionPageItemModel
	{
		#region プロパティ
		/// <summary>Question</summary>
		public ScoringSaleQuestionModel Question
		{
			get { return (ScoringSaleQuestionModel)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGEITEM_QUESTION]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGEITEM_QUESTION] = value; }
		}
		/// <summary>Name</summary>
		public string Name { get; set; }
		/// <summary>Statement</summary>
		public string Statement { get; set; }
		/// <summary>Type</summary>
		public string Type { get; set; }
		/// <summary>Update date</summary>
		public string UpdateDate { get; set; }
		#endregion
	}
}
