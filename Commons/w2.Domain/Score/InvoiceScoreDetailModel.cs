/*
=========================================================================================================
  Module      : スコア後払い後払い請求書明細モデル (InvoiceScoreDetailModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.Score
{
	/// <summary>
	/// スコア後払い後払い請求書明細モデル
	/// </summary>
	[Serializable]
	public class InvoiceScoreDetailModel : ModelBase<InvoiceScoreDetailModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public InvoiceScoreDetailModel()
		{
			this.OrderId = string.Empty;
			this.DetailNo = 1;
			this.GoodsName = string.Empty;
			this.GoodsPrice = string.Empty;
			this.GoodsNum = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public InvoiceScoreDetailModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public InvoiceScoreDetailModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>注文ID</summary>
		public string OrderId
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_DETAIL_ORDER_ID]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_DETAIL_ORDER_ID] = value; }
		}
		/// <summary>明細番号</summary>
		public int DetailNo
		{
			get { return (int)this.DataSource[Constants.FIELD_INVOICE_SCORE_DETAIL_DETAIL_NO]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_DETAIL_DETAIL_NO] = value; }
		}
		/// <summary>明細名</summary>
		public string GoodsName
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_DETAIL_GOODS_NAME]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_DETAIL_GOODS_NAME] = value; }
		}
		/// <summary>単価</summary>
		public string GoodsPrice
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_DETAIL_GOODS_PRICE]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_DETAIL_GOODS_PRICE] = value; }
		}
		/// <summary>数量</summary>
		public string GoodsNum
		{
			get { return (string)this.DataSource[Constants.FIELD_INVOICE_SCORE_DETAIL_GOODS_NUM]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_DETAIL_GOODS_NUM] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_INVOICE_SCORE_DETAIL_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_INVOICE_SCORE_DETAIL_DATE_CREATED] = value; }
		}
		#endregion
	}
}
