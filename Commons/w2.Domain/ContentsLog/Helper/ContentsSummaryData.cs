/*
=========================================================================================================
  Module      : コンテンツ解析データモデル (ContentsSummaryData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.ContentsLog.Helper
{
	/// <summary>
	/// コンテンツ解析データモデル
	/// </summary>
	[Serializable]
	public partial class ContentsSummaryData : ModelBase<ContentsSummaryData>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ContentsSummaryData()
		{
			this.ContentsType = "";
			this.ContentsId = "";
			this.PvCount = 0;
			this.CvCount = 0;
			this.Price = 0;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ContentsSummaryData(DataRowView source)
			: this(source.ToHashtable())
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ContentsSummaryData(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
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
		/// <summary>PVカウント</summary>
		public long PvCount
		{
			get { return (long)this.DataSource["pv_count"]; }
			set { this.DataSource["pv_count"] = value; }
		}
		/// <summary>CVカウント</summary>
		public long CvCount
		{
			get { return (long)this.DataSource["cv_count"]; }
			set { this.DataSource["cv_count"] = value; }
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
