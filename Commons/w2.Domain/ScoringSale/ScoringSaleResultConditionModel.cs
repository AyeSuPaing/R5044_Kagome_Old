/*
=========================================================================================================
  Module      : ScoringSaleResultConditionモデル (ScoringSaleResultConditionModel_EX.cs)
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
	/// Scoring sale result conditionモデル
	/// </summary>
	[Serializable]
	public partial class ScoringSaleResultConditionModel : ModelBase<ScoringSaleResultConditionModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ScoringSaleResultConditionModel()
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ScoringSaleResultConditionModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ScoringSaleResultConditionModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>スコアリング販売ID</summary>
		public string ScoringSaleId
		{
			get { return (string)this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_SCORING_SALE_ID]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_SCORING_SALE_ID] = value; }
		}
		/// <summary>枝番</summary>
		public int BranchNo
		{
			get { return (int)this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_BRANCH_NO]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_BRANCH_NO] = value; }
		}
		/// <summary>条件枝番</summary>
		public int ConditionBranchNo
		{
			get { return (int)this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_CONDITION_BRANCH_NO]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_CONDITION_BRANCH_NO] = value; }
		}
		/// <summary>スコア軸番号</summary>
		public int ScoreAxisAxisNo
		{
			get { return (int)this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_SCORE_AXIS_AXIS_NO]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_SCORE_AXIS_AXIS_NO] = value; }
		}
		/// <summary>スコア軸加算値(From)</summary>
		public int ScoreAxisAxisValueFrom
		{
			get { return (int)this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_SCORE_AXIS_AXIS_VALUE_FROM]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_SCORE_AXIS_AXIS_VALUE_FROM] = value; }
		}
		/// <summary>スコア軸加算値(To)</summary>
		public int ScoreAxisAxisValueTo
		{
			get { return (int)this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_SCORE_AXIS_AXIS_VALUE_TO]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_SCORE_AXIS_AXIS_VALUE_TO] = value; }
		}
		/// <summary>条件</summary>
		public string Condition
		{
			get { return (string)this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_CONDITION]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_CONDITION] = value; }
		}
		/// <summary>グループ番号</summary>
		public int GroupNo
		{
			get { return (int)this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_GROUP_NO]; }
			set { this.DataSource[Constants.FIELD_SCORINGSALERESULTCONDITION_GROUP_NO] = value; }
		}
		#endregion
	}
}
