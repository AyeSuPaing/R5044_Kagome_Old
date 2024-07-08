/*
=========================================================================================================
  Module      : Score Axis Model (ScoreAxisModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.ScoreAxis
{
	/// <summary>
	/// Score axis model
	/// </summary>
	[Serializable]
	public partial class ScoreAxisModel : ModelBase<ScoreAxisModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ScoreAxisModel()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ScoreAxisModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ScoreAxisModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>スコア軸ID</summary>
		public string ScoreAxisId
		{
			get { return (string)this.DataSource[Constants.FIELD_SCOREAXIS_SCORE_AXIS_ID]; }
			set { this.DataSource[Constants.FIELD_SCOREAXIS_SCORE_AXIS_ID] = value; }
		}
		/// <summary>スコア軸タイトル</summary>
		public string ScoreAxisTitle
		{
			get { return (string)this.DataSource[Constants.FIELD_SCOREAXIS_SCORE_AXIS_TITLE]; }
			set { this.DataSource[Constants.FIELD_SCOREAXIS_SCORE_AXIS_TITLE] = value; }
		}
		/// <summary>軸名１</summary>
		public string AxisName1
		{
			get { return (string)this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME1]; }
			set { this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME1] = value; }
		}
		/// <summary>軸名２</summary>
		public string AxisName2
		{
			get { return (string)this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME2]; }
			set { this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME2] = value; }
		}
		/// <summary>軸名３</summary>
		public string AxisName3
		{
			get { return (string)this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME3]; }
			set { this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME3] = value; }
		}
		/// <summary>軸名４</summary>
		public string AxisName4
		{
			get { return (string)this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME4]; }
			set { this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME4] = value; }
		}
		/// <summary>軸名５</summary>
		public string AxisName5
		{
			get { return (string)this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME5]; }
			set { this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME5] = value; }
		}
		/// <summary>軸名６</summary>
		public string AxisName6
		{
			get { return (string)this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME6]; }
			set { this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME6] = value; }
		}
		/// <summary>軸名７</summary>
		public string AxisName7
		{
			get { return (string)this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME7]; }
			set { this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME7] = value; }
		}
		/// <summary>軸名８</summary>
		public string AxisName8
		{
			get { return (string)this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME8]; }
			set { this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME8] = value; }
		}
		/// <summary>軸名９</summary>
		public string AxisName9
		{
			get { return (string)this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME9]; }
			set { this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME9] = value; }
		}
		/// <summary>軸名１０</summary>
		public string AxisName10
		{
			get { return (string)this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME10]; }
			set { this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME10] = value; }
		}
		/// <summary>軸名１１</summary>
		public string AxisName11
		{
			get { return (string)this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME11]; }
			set { this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME11] = value; }
		}
		/// <summary>軸名１２</summary>
		public string AxisName12
		{
			get { return (string)this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME12]; }
			set { this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME12] = value; }
		}
		/// <summary>軸名１３</summary>
		public string AxisName13
		{
			get { return (string)this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME13]; }
			set { this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME13] = value; }
		}
		/// <summary>軸名１４</summary>
		public string AxisName14
		{
			get { return (string)this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME14]; }
			set { this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME14] = value; }
		}
		/// <summary>軸名１５</summary>
		public string AxisName15
		{
			get { return (string)this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME15]; }
			set { this.DataSource[Constants.FIELD_SCOREAXIS_AXIS_NAME15] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_SCOREAXIS_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_SCOREAXIS_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_SCOREAXIS_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_SCOREAXIS_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_SCOREAXIS_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_SCOREAXIS_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
