/*
=========================================================================================================
  Module      : サマリ分析結果テーブルモデル (DispSummaryAnalysisModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.Coupon
{
	/// <summary>
	/// サマリ分析結果テーブルモデル
	/// </summary>
	[Serializable]
	public partial class DispSummaryAnalysisModel : ModelBase<DispSummaryAnalysisModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public DispSummaryAnalysisModel()
		{
			this.Counts = 0;
			this.Reserved1 = 0;
			this.Reserved2 = 0;
			this.Reserved3 = 0;
			this.Reserved4 = 0;
			this.Reserved5 = 0;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public DispSummaryAnalysisModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public DispSummaryAnalysisModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>データ番号</summary>
		public long DataNo
		{
			get { return (long)this.DataSource[Constants.FIELD_DISPSUMMARYANALYSIS_DATA_NO]; }
		}
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return (string)this.DataSource[Constants.FIELD_DISPSUMMARYANALYSIS_DEPT_ID]; }
			set { this.DataSource[Constants.FIELD_DISPSUMMARYANALYSIS_DEPT_ID] = value; }
		}
		/// <summary>集計区分</summary>
		public string SummaryKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_DISPSUMMARYANALYSIS_SUMMARY_KBN]; }
			set { this.DataSource[Constants.FIELD_DISPSUMMARYANALYSIS_SUMMARY_KBN] = value; }
		}
		/// <summary>対象年</summary>
		public string TgtYear
		{
			get { return (string)this.DataSource[Constants.FIELD_DISPSUMMARYANALYSIS_TGT_YEAR]; }
			set { this.DataSource[Constants.FIELD_DISPSUMMARYANALYSIS_TGT_YEAR] = value; }
		}
		/// <summary>対象月</summary>
		public string TgtMonth
		{
			get { return (string)this.DataSource[Constants.FIELD_DISPSUMMARYANALYSIS_TGT_MONTH]; }
			set { this.DataSource[Constants.FIELD_DISPSUMMARYANALYSIS_TGT_MONTH] = value; }
		}
		/// <summary>対象日</summary>
		public string TgtDay
		{
			get { return (string)this.DataSource[Constants.FIELD_DISPSUMMARYANALYSIS_TGT_DAY]; }
			set { this.DataSource[Constants.FIELD_DISPSUMMARYANALYSIS_TGT_DAY] = value; }
		}
		/// <summary>項目名</summary>
		public string ValueName
		{
			get { return (string)this.DataSource[Constants.FIELD_DISPSUMMARYANALYSIS_VALUE_NAME]; }
			set { this.DataSource[Constants.FIELD_DISPSUMMARYANALYSIS_VALUE_NAME] = value; }
		}
		/// <summary>カウント数</summary>
		public long Counts
		{
			get { return (long)this.DataSource[Constants.FIELD_DISPSUMMARYANALYSIS_COUNTS]; }
			set { this.DataSource[Constants.FIELD_DISPSUMMARYANALYSIS_COUNTS] = value; }
		}
		/// <summary>予備値1</summary>
		public long Reserved1
		{
			get { return (long)this.DataSource[Constants.FIELD_DISPSUMMARYANALYSIS_RESERVED1]; }
			set { this.DataSource[Constants.FIELD_DISPSUMMARYANALYSIS_RESERVED1] = value; }
		}
		/// <summary>予備値2</summary>
		public long Reserved2
		{
			get { return (long)this.DataSource[Constants.FIELD_DISPSUMMARYANALYSIS_RESERVED2]; }
			set { this.DataSource[Constants.FIELD_DISPSUMMARYANALYSIS_RESERVED2] = value; }
		}
		/// <summary>予備値3</summary>
		public long Reserved3
		{
			get { return (long)this.DataSource[Constants.FIELD_DISPSUMMARYANALYSIS_RESERVED3]; }
			set { this.DataSource[Constants.FIELD_DISPSUMMARYANALYSIS_RESERVED3] = value; }
		}
		/// <summary>予備値4</summary>
		public long Reserved4
		{
			get { return (long)this.DataSource[Constants.FIELD_DISPSUMMARYANALYSIS_RESERVED4]; }
			set { this.DataSource[Constants.FIELD_DISPSUMMARYANALYSIS_RESERVED4] = value; }
		}
		/// <summary>予備値5</summary>
		public long Reserved5
		{
			get { return (long)this.DataSource[Constants.FIELD_DISPSUMMARYANALYSIS_RESERVED5]; }
			set { this.DataSource[Constants.FIELD_DISPSUMMARYANALYSIS_RESERVED5] = value; }
		}
		/// <summary>予備値6</summary>
		public string Reserved6
		{
			get { return (string)this.DataSource[Constants.FIELD_DISPSUMMARYANALYSIS_RESERVED6]; }
			set { this.DataSource[Constants.FIELD_DISPSUMMARYANALYSIS_RESERVED6] = value; }
		}
		/// <summary>予備値7</summary>
		public string Reserved7
		{
			get { return (string)this.DataSource[Constants.FIELD_DISPSUMMARYANALYSIS_RESERVED7]; }
			set { this.DataSource[Constants.FIELD_DISPSUMMARYANALYSIS_RESERVED7] = value; }
		}
		/// <summary>予備値8</summary>
		public string Reserved8
		{
			get { return (string)this.DataSource[Constants.FIELD_DISPSUMMARYANALYSIS_RESERVED8]; }
			set { this.DataSource[Constants.FIELD_DISPSUMMARYANALYSIS_RESERVED8] = value; }
		}
		/// <summary>予備値9</summary>
		public string Reserved9
		{
			get { return (string)this.DataSource[Constants.FIELD_DISPSUMMARYANALYSIS_RESERVED9]; }
			set { this.DataSource[Constants.FIELD_DISPSUMMARYANALYSIS_RESERVED9] = value; }
		}
		/// <summary>予備値10</summary>
		public string Reserved10
		{
			get { return (string)this.DataSource[Constants.FIELD_DISPSUMMARYANALYSIS_RESERVED10]; }
			set { this.DataSource[Constants.FIELD_DISPSUMMARYANALYSIS_RESERVED10] = value; }
		}
		#endregion
	}
}
