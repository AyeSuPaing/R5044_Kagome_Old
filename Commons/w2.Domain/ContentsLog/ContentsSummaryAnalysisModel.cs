/*
=========================================================================================================
  Module      : コンテンツ解析モデル (ContentsSummaryAnalysisModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.ContentsSummaryAnalysis
{
	/// <summary>
	/// コンテンツ解析モデル
	/// </summary>
	[Serializable]
	public partial class ContentsSummaryAnalysisModel : ModelBase<ContentsSummaryAnalysisModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ContentsSummaryAnalysisModel()
		{
			// TODO:定数を利用するよう書き換えてください。
			this.ReportType = "";
			this.AccessKbn = "";
			this.ContentsType = "";
			this.ContentsId = "";
			this.Count = 0;
			this.Price = 0;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ContentsSummaryAnalysisModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ContentsSummaryAnalysisModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>データNO</summary>
		public long DataNo
		{
			get { return (long)this.DataSource[Constants.FIELD_CONTENTSSUMMARYANALYSIS_DATA_NO]; }
		}
		/// <summary>日付</summary>
		public DateTime Date
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CONTENTSSUMMARYANALYSIS_DATE]; }
			set { this.DataSource[Constants.FIELD_CONTENTSSUMMARYANALYSIS_DATE] = value; }
		}
		/// <summary>対象年</summary>
		public string TgtYear
		{
			get { return (string)this.DataSource[Constants.FIELD_CONTENTSSUMMARYANALYSIS_TGT_YEAR]; }
		}
		/// <summary>対象月</summary>
		public string TgtMonth
		{
			get { return (string)this.DataSource[Constants.FIELD_CONTENTSSUMMARYANALYSIS_TGT_MONTH]; }
		}
		/// <summary>対象日</summary>
		public string TgtDay
		{
			get { return (string)this.DataSource[Constants.FIELD_CONTENTSSUMMARYANALYSIS_TGT_DAY]; }
		}
		/// <summary>レポートタイプ</summary>
		public string ReportType
		{
			get { return (string)this.DataSource[Constants.FIELD_CONTENTSSUMMARYANALYSIS_REPORT_TYPE]; }
			set { this.DataSource[Constants.FIELD_CONTENTSSUMMARYANALYSIS_REPORT_TYPE] = value; }
		}
		/// <summary>アクセス区分</summary>
		public string AccessKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_CONTENTSSUMMARYANALYSIS_ACCESS_KBN]; }
			set { this.DataSource[Constants.FIELD_CONTENTSSUMMARYANALYSIS_ACCESS_KBN] = value; }
		}
		/// <summary>コンテンツタイプ</summary>
		public string ContentsType
		{
			get { return (string)this.DataSource[Constants.FIELD_CONTENTSSUMMARYANALYSIS_CONTENTS_TYPE]; }
			set { this.DataSource[Constants.FIELD_CONTENTSSUMMARYANALYSIS_CONTENTS_TYPE] = value; }
		}
		/// <summary>コンテンツID</summary>
		public string ContentsId
		{
			get { return (string)this.DataSource[Constants.FIELD_CONTENTSSUMMARYANALYSIS_CONTENTS_ID]; }
			set { this.DataSource[Constants.FIELD_CONTENTSSUMMARYANALYSIS_CONTENTS_ID] = value; }
		}
		/// <summary>カウント</summary>
		public long Count
		{
			get { return (long)this.DataSource[Constants.FIELD_CONTENTSSUMMARYANALYSIS_COUNT]; }
			set { this.DataSource[Constants.FIELD_CONTENTSSUMMARYANALYSIS_COUNT] = value; }
		}
		/// <summary>金額</summary>
		public decimal Price
		{
			get { return (decimal)this.DataSource[Constants.FIELD_CONTENTSSUMMARYANALYSIS_PRICE]; }
			set { this.DataSource[Constants.FIELD_CONTENTSSUMMARYANALYSIS_PRICE] = value; }
		}
		#endregion
	}
}
