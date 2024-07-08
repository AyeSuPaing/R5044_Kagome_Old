/*
=========================================================================================================
  Module      : コンテンツログモデル (ContentsLogModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.ContentsLog
{
	/// <summary>
	/// コンテンツログモデル
	/// </summary>
	[Serializable]
	public partial class ContentsLogModel : ModelBase<ContentsLogModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ContentsLogModel()
		{
			// TODO:定数を利用するよう書き換えてください。
			this.Date = DateTime.Now;
			this.ReportType = "";
			this.AccessKbn = "";
			this.ContentsType = "";
			this.ContentsId = "";
			this.Price = 0;
			this.OrderId = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ContentsLogModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ContentsLogModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>ログNO</summary>
		public long LogNo
		{
			get { return (long)this.DataSource[Constants.FIELD_CONTENTSLOG_LOG_NO]; }
			set { this.DataSource[Constants.FIELD_CONTENTSLOG_LOG_NO] = value; }
		}
		/// <summary>日付</summary>
		public DateTime? Date
		{
			get
			{
				if (this.DataSource[Constants.FIELD_CONTENTSLOG_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_CONTENTSLOG_DATE];
			}
			set { this.DataSource[Constants.FIELD_CONTENTSLOG_DATE] = value; }
		}
		/// <summary>レポートタイプ</summary>
		public string ReportType
		{
			get { return (string)this.DataSource[Constants.FIELD_CONTENTSLOG_REPORT_TYPE]; }
			set { this.DataSource[Constants.FIELD_CONTENTSLOG_REPORT_TYPE] = value; }
		}
		/// <summary>アクセス区分</summary>
		public string AccessKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_CONTENTSLOG_ACCESS_KBN]; }
			set { this.DataSource[Constants.FIELD_CONTENTSLOG_ACCESS_KBN] = value; }
		}
		/// <summary>コンテンツタイプ</summary>
		public string ContentsType
		{
			get { return (string)this.DataSource[Constants.FIELD_CONTENTSLOG_CONTENTS_TYPE]; }
			set { this.DataSource[Constants.FIELD_CONTENTSLOG_CONTENTS_TYPE] = value; }
		}
		/// <summary>コンテンツID</summary>
		public string ContentsId
		{
			get { return (string)this.DataSource[Constants.FIELD_CONTENTSLOG_CONTENTS_ID]; }
			set { this.DataSource[Constants.FIELD_CONTENTSLOG_CONTENTS_ID] = value; }
		}
		/// <summary>金額</summary>
		public decimal Price
		{
			get { return (decimal)this.DataSource[Constants.FIELD_CONTENTSLOG_PRICE]; }
			set { this.DataSource[Constants.FIELD_CONTENTSLOG_PRICE] = value; }
		}
		/// <summary>注文ID</summary>
		public string OrderId
		{
			get { return (string)this.DataSource[Constants.FIELD_CONTENTSLOG_ORDER_ID]; }
			set { this.DataSource[Constants.FIELD_CONTENTSLOG_ORDER_ID] = value; }
		}
		#endregion
	}
}
