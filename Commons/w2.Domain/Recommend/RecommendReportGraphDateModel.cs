/*
=========================================================================================================
  Module      : レコメンドレポートグラフモデル (RecommendReportGraphDateModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.Recommend
{
	/// <summary>
	/// レコメンドレポートグラフモデル
	/// </summary>
	[Serializable]
	public partial class RecommendReportGraphDateModel : ModelBase<RecommendReportGraphDateModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public RecommendReportGraphDateModel()
		{
			this.DateYear = string.Empty;
			this.DateMonth = string.Empty;
			this.DateDay = string.Empty;
			this.DateValue = 0;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public RecommendReportGraphDateModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public RecommendReportGraphDateModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>年</summary>
		public string DateYear
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMEND_REPORT_GRAPH_DATE_YEAR]; }
			set { this.DataSource[Constants.FIELD_RECOMMEND_REPORT_GRAPH_DATE_YEAR] = value; }
		}
		/// <summary>月</summary>
		public string DateMonth
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMEND_REPORT_GRAPH_DATE_MONTH]; }
			set { this.DataSource[Constants.FIELD_RECOMMEND_REPORT_GRAPH_DATE_MONTH] = value; }
		}
		/// <summary>日</summary>
		public string DateDay
		{
			get { return (string)this.DataSource[Constants.FIELD_RECOMMEND_REPORT_GRAPH_DATE_DAY]; }
			set { this.DataSource[Constants.FIELD_RECOMMEND_REPORT_GRAPH_DATE_DAY] = value; }
		}
		/// <summary>値</summary>
		public int DateValue
		{
			get { return (int)this.DataSource[Constants.FIELD_RECOMMEND_REPORT_GRAPH_DATE_VALUE]; }
			set { this.DataSource[Constants.FIELD_RECOMMEND_REPORT_GRAPH_DATE_VALUE] = value; }
		}
		#endregion
	}
}
