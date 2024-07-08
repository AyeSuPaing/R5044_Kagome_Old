/*
=========================================================================================================
  Module      : 定期購入商品情報モデル (FixedPurchaseItemModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.FixedPurchase
{
	/// <summary>
	/// 定期購入商品情報モデル
	/// </summary>
	[Serializable]
	public partial class FixedPurchaseItemModel : ModelBase<FixedPurchaseItemModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public FixedPurchaseItemModel()
		{
			this.FixedPurchaseItemNo = 1;
			this.FixedPurchaseShippingNo = 1;
			this.ItemQuantity = 1;
			this.ItemQuantitySingle = 1;
			this.ProductSetId = string.Empty;
			this.ProductSetNo = null;
			this.ProductSetCount = null;
			this.ItemOrderCount = 0;
			this.ItemShippedCount = 0;
			this.SupplierId = string.Empty;
			this.ProductOptionTexts = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FixedPurchaseItemModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FixedPurchaseItemModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>定期購入ID</summary>
		[UpdateData(1, "fixed_purchase_id")]
		public string FixedPurchaseId
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_FIXED_PURCHASE_ID]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_FIXED_PURCHASE_ID] = value; }
		}
		/// <summary>定期購入注文商品枝番</summary>
		[UpdateData(2, "fixed_purchase_item_no")]
		public int FixedPurchaseItemNo
		{
			get { return (int)this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_FIXED_PURCHASE_ITEM_NO]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_FIXED_PURCHASE_ITEM_NO] = value; }
		}
		/// <summary>定期購入配送先枝番</summary>
		[UpdateData(3, "fixed_purchase_shipping_no")]
		public int FixedPurchaseShippingNo
		{
			get { return (int)this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_FIXED_PURCHASE_SHIPPING_NO]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_FIXED_PURCHASE_SHIPPING_NO] = value; }
		}
		/// <summary>店舗ID</summary>
		[UpdateData(4, "shop_id")]
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_SHOP_ID] = value; }
		}
		/// <summary>商品ID</summary>
		[UpdateData(5, "product_id")]
		public string ProductId
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_PRODUCT_ID]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_PRODUCT_ID] = value; }
		}
		/// <summary>商品バリエーションID</summary>
		[UpdateData(6, "variation_id")]
		public string VariationId
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_VARIATION_ID]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_VARIATION_ID] = value; }
		}
		/// <summary>サプライヤID</summary>
		[UpdateData(7, "supplier_id")]
		public string SupplierId
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_SUPPLIER_ID]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_SUPPLIER_ID] = value; }
		}
		/// <summary>注文数</summary>
		[UpdateData(8, "item_quantity")]
		public int ItemQuantity
		{
			get { return (int)this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_ITEM_QUANTITY]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_ITEM_QUANTITY] = value; }
		}
		/// <summary>注文数（セット未考慮）</summary>
		[UpdateData(9, "item_quantity_single")]
		public int ItemQuantitySingle
		{
			get { return (int)this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_ITEM_QUANTITY_SINGLE]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_ITEM_QUANTITY_SINGLE] = value; }
		}
		///// <summary>明細金額（小計）</summary>
		//[UpdateDataAttribute(10, "item_price")]
		//public decimal ItemPrice
		//{
		//	get { return (decimal)this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_ITEM_PRICE]; }
		//	set { this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_ITEM_PRICE] = value; }
		//}
		///// <summary>明細金額（小計・セット未考慮）</summary>
		//[UpdateDataAttribute(11, "item_price_single")]
		//public decimal ItemPriceSingle
		//{
		//	get { return (decimal)this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_ITEM_PRICE_SINGLE]; }
		//	set { this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_ITEM_PRICE_SINGLE] = value; }
		//}
		/// <summary>商品セットID</summary>
		[UpdateData(12, "product_set_id")]
		public string ProductSetId
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_PRODUCT_SET_ID]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_PRODUCT_SET_ID] = value; }
		}
		/// <summary>商品セット枝番</summary>
		[UpdateData(13, "product_set_no")]
		public int? ProductSetNo
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_PRODUCT_SET_NO] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_PRODUCT_SET_NO];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_PRODUCT_SET_NO] = value; }
		}
		/// <summary>商品セット注文数</summary>
		[UpdateData(14, "product_set_count")]
		public int? ProductSetCount
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_PRODUCT_SET_COUNT] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_PRODUCT_SET_COUNT];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_PRODUCT_SET_COUNT] = value; }
		}
		/// <summary>作成日</summary>
		[UpdateData(15, "date_created")]
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		[UpdateData(16, "date_changed")]
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_DATE_CHANGED] = value; }
		}
		/// <summary>商品付帯情報選択値</summary>
		[UpdateData(17, "product_option_texts")]
		public string ProductOptionTexts
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_PRODUCT_OPTION_TEXTS]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_PRODUCT_OPTION_TEXTS] = value; }
		}
		/// <summary>購入回数(注文基準)</summary>
		[UpdateData(18, "item_order_count")]
		public int ItemOrderCount
		{
			get { return (int)this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_ITEM_ORDER_COUNT]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_ITEM_ORDER_COUNT] = value; }
		}
		/// <summary>購入回数(出荷基準)</summary>
		[UpdateData(19, "item_shipped_count")]
		public int ItemShippedCount
		{
			get { return (int)this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_ITEM_SHIPPED_COUNT]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEITEM_ITEM_SHIPPED_COUNT] = value; }
		}
		#endregion
	}
}