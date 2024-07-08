/*
=========================================================================================================
  Module      : DSK後払い後払い請求書明細モデル (InvoiceDskDeferredDetailModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.InvoiceDskDeferredDetail
{
	/// <summary>
	/// DSK後払い後払い請求書明細モデル
	/// </summary>
	[Serializable]
	public class InvoiceDskDeferredDetailModel : ModelBase<InvoiceDskDeferredDetailModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public InvoiceDskDeferredDetailModel()
		{
			this.OrderId = "";
			this.DetailNo = 1;
			this.GoodsName = "";
			this.GoodsPrice = "";
			this.GoodsNum = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public InvoiceDskDeferredDetailModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public InvoiceDskDeferredDetailModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>注文ID</summary>
		public string OrderId
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERREDDETAIL_ORDER_ID]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERREDDETAIL_ORDER_ID] = value; }
		}
		/// <summary>明細番号</summary>
		public int DetailNo
		{
			get { return (int)this.DataSource[Constants.FIELD_INVOICEDSKDEFERREDDETAIL_DETAIL_NO]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERREDDETAIL_DETAIL_NO] = value; }
		}
		/// <summary>明細名</summary>
		public string GoodsName
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERREDDETAIL_GOODS_NAME]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERREDDETAIL_GOODS_NAME] = value; }
		}
		/// <summary>単価</summary>
		public string GoodsPrice
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERREDDETAIL_GOODS_PRICE]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERREDDETAIL_GOODS_PRICE] = value; }
		}
		/// <summary>数量</summary>
		public string GoodsNum
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICEDSKDEFERREDDETAIL_GOODS_NUM]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERREDDETAIL_GOODS_NUM] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_INVOICEDSKDEFERREDDETAIL_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_INVOICEDSKDEFERREDDETAIL_DATE_CREATED] = value; }
		}
		#endregion
	}
}
