/*
=========================================================================================================
  Module      : Scoring Sale Question Model (ScoringSaleQuestionModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.ScoringSale
{
	/// <summary>
	/// Scoring sale question model
	/// </summary>
	[Serializable]
	public partial class ScoringSaleQuestionModel : ModelBase<ScoringSaleQuestionModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ScoringSaleQuestionModel()
		{
			this.ScoringSaleQuestionChoiceList = new List<ScoringSaleQuestionChoiceModel>();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ScoringSaleQuestionModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ScoringSaleQuestionModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>質問ID</summary>
		public string QuestionId
		{
			get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEQUESTION_QUESTION_ID]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTION_QUESTION_ID] = value; }
		}
		/// <summary>質問タイトル</summary>
		public string QuestionTitle
		{
			get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEQUESTION_QUESTION_TITLE]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTION_QUESTION_TITLE] = value; }
		}
		/// <summary>スコア軸ID</summary>
		public string ScoreAxisId
		{
			get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEQUESTION_SCORE_AXIS_ID]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTION_SCORE_AXIS_ID] = value; }
		}
		/// <summary>回答タイプ</summary>
		public string AnswerType
		{
			get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEQUESTION_ANSWER_TYPE]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTION_ANSWER_TYPE] = value; }
		}
		/// <summary>質問文</summary>
		public string QuestionStatement
		{
			get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEQUESTION_QUESTION_STATEMENT]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTION_QUESTION_STATEMENT] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_SCORINGSALEQUESTION_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTION_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_SCORINGSALEQUESTION_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTION_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEQUESTION_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTION_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
