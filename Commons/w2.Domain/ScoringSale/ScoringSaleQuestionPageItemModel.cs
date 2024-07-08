/*
=========================================================================================================
  Module      : ScoringSaleQuestionPageItemモデル (ScoringSaleQuestionPageItemModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.ScoringSale
{
	/// <summary>
	/// ScoringSaleQuestionPageItemモデル
	/// </summary>
	[Serializable]
	public partial class ScoringSaleQuestionPageItemModel : ModelBase<ScoringSaleQuestionPageItemModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ScoringSaleQuestionPageItemModel()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ScoringSaleQuestionPageItemModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ScoringSaleQuestionPageItemModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>スコアリング販売ID</summary>
		/// <summary>質問ID</summary>
		public string QuestionId
		{
			get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGEITEM_QUESTION_ID]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGEITEM_QUESTION_ID] = value; }
		}
		/// <summary>スコアリング販売ID</summary>
		public string ScoringSaleId
		{
			get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGEITEM_SCORING_SALE_ID]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGEITEM_SCORING_SALE_ID] = value; }
		}
		/// <summary>ページNo</summary>
		public int PageNo
		{
			get { return (int)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGEITEM_PAGE_NO]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGEITEM_PAGE_NO] = value; }
		}
		/// <summary>枝番</summary>
		public int BranchNo
		{
			get { return (int)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGEITEM_BRANCH_NO]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONPAGEITEM_BRANCH_NO] = value; }
		}
		#endregion
	}
}
