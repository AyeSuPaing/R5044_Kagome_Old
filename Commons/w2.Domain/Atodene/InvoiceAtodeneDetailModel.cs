/*
=========================================================================================================
  Module      : Atodene後払い請求書明細モデル (InvoiceAtodeneDetailModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.Atodene
{
	/// <summary>
	/// Atodene後払い請求書明細モデル
	/// </summary>
	[Serializable]
	public partial class InvoiceAtodeneDetailModel : ModelBase<InvoiceAtodeneDetailModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public InvoiceAtodeneDetailModel()
		{
			this.DetailNo = 1;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public InvoiceAtodeneDetailModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public InvoiceAtodeneDetailModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>注文ID</summary>
		public string OrderId
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEATODENEDETAIL_ORDER_ID]; }
			set { this.DataSource[Constants.FIELD_INVOICEATODENEDETAIL_ORDER_ID] = value; }
		}
		/// <summary>明細番号</summary>
		public int DetailNo
		{
			get { return (int)this.DataSource[Constants.FIELD_INVOICEATODENEDETAIL_DETAIL_NO]; }
			set { this.DataSource[Constants.FIELD_INVOICEATODENEDETAIL_DETAIL_NO] = value; }
		}
		/// <summary>明細内容</summary>
		public string Goods
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENEDETAIL_GOODS] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENEDETAIL_GOODS];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENEDETAIL_GOODS] = value; }
		}
		/// <summary>注文数</summary>
		public string Goodsamount
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENEDETAIL_GOODSAMOUNT] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENEDETAIL_GOODSAMOUNT];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENEDETAIL_GOODSAMOUNT] = value; }
		}
		/// <summary>単価</summary>
		public string Goodsprice
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENEDETAIL_GOODSPRICE] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENEDETAIL_GOODSPRICE];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENEDETAIL_GOODSPRICE] = value; }
		}
		/// <summary>金額</summary>
		public string Goodssubtotal
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENEDETAIL_GOODSSUBTOTAL] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENEDETAIL_GOODSSUBTOTAL];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENEDETAIL_GOODSSUBTOTAL] = value; }
		}
		/// <summary>明細予備項目</summary>
		public string Goodsexpand
		{
			get
			{
				if (this.DataSource[Constants.FIELD_INVOICEATODENEDETAIL_GOODSEXPAND] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_INVOICEATODENEDETAIL_GOODSEXPAND];
			}
			set { this.DataSource[Constants.FIELD_INVOICEATODENEDETAIL_GOODSEXPAND] = value; }
		}
		#endregion
	}
}
