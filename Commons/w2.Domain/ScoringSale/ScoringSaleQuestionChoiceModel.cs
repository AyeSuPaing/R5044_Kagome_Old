/*
=========================================================================================================
  Module      : Scoring Sale Question Choice Model (ScoringSaleQuestionChoiceModel.cs)
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
	/// Scoring sale question choice model
	/// </summary>
	[Serializable]
	public partial class ScoringSaleQuestionChoiceModel : ModelBase<ScoringSaleQuestionChoiceModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ScoringSaleQuestionChoiceModel()
		{
			this.QuestionChoiceStatementImgPath = null;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ScoringSaleQuestionChoiceModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ScoringSaleQuestionChoiceModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>質問ID</summary>
		public string QuestionId
		{
			get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_QUESTION_ID]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_QUESTION_ID] = value; }
		}
		/// <summary>枝番</summary>
		public int BranchNo
		{
			get { return (int)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_BRANCH_NO]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_BRANCH_NO] = value; }
		}
		/// <summary>選択肢文</summary>
		public string QuestionChoiceStatement
		{
			get { return (string)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_QUESTION_CHOICE_STATEMENT]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_QUESTION_CHOICE_STATEMENT] = value; }
		}
		/// <summary>選択肢画像</summary>
		public string QuestionChoiceStatementImgPath
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_QUESTION_CHOICE_STATEMENT_IMG_PATH] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_QUESTION_CHOICE_STATEMENT_IMG_PATH];
			}
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_QUESTION_CHOICE_STATEMENT_IMG_PATH] = value; }
		}
		/// <summary>軸加算値１</summary>
		public int AxisAdditional1
		{
			get { return (int)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL1]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL1] = value; }
		}
		/// <summary>軸加算値２</summary>
		public int AxisAdditional2
		{
			get { return (int)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL2]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL2] = value; }
		}
		/// <summary>軸加算値３</summary>
		public int AxisAdditional3
		{
			get { return (int)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL3]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL3] = value; }
		}
		/// <summary>軸加算値４</summary>
		public int AxisAdditional4
		{
			get { return (int)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL4]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL4] = value; }
		}
		/// <summary>軸加算値５</summary>
		public int AxisAdditional5
		{
			get { return (int)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL5]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL5] = value; }
		}
		/// <summary>軸加算値６</summary>
		public int AxisAdditional6
		{
			get { return (int)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL6]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL6] = value; }
		}
		/// <summary>軸加算値７</summary>
		public int AxisAdditional7
		{
			get { return (int)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL7]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL7] = value; }
		}
		/// <summary>軸加算値８</summary>
		public int AxisAdditional8
		{
			get { return (int)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL8]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL8] = value; }
		}
		/// <summary>軸加算値９</summary>
		public int AxisAdditional9
		{
			get { return (int)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL9]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL9] = value; }
		}
		/// <summary>軸加算値１０</summary>
		public int AxisAdditional10
		{
			get { return (int)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL10]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL10] = value; }
		}
		/// <summary>軸加算値１１</summary>
		public int AxisAdditional11
		{
			get { return (int)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL11]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL11] = value; }
		}
		/// <summary>軸加算値１２</summary>
		public int AxisAdditional12
		{
			get { return (int)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL12]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL12] = value; }
		}
		/// <summary>軸加算値１３</summary>
		public int AxisAdditional13
		{
			get { return (int)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL13]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL13] = value; }
		}
		/// <summary>軸加算値１４</summary>
		public int AxisAdditional14
		{
			get { return (int)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL14]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL14] = value; }
		}
		/// <summary>軸加算値１５</summary>
		public int AxisAdditional15
		{
			get { return (int)this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL15]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALEQUESTIONCHOICE_AXIS_ADDITIONAL15] = value; }
		}
		#endregion
	}
}
