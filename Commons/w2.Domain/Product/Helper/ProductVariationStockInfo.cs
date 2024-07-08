/*
=========================================================================================================
  Module      : 商品バリエーション在庫情報 (ProductVariationStockInfo.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Common.Util;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.Product
{
	/// <summary>
	/// 商品バリエーション在庫情報
	/// </summary>
	[Serializable]
	public partial class ProductVariationStockInfo : ModelBase<ProductModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ProductVariationStockInfo()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductVariationStockInfo(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductVariationStockInfo(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_SHOP_ID] = value; }
		}
		/// <summary>商品ID</summary>
		public string ProductId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID] = value; }
		}
		/// <summary>バリエーションID</summary>
		public string VariationId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID] = value; }
		}
		/// <summary>商品名</summary>
		[DbMapName(Constants.FIELD_PRODUCT_NAME)]
		public string Name
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_NAME]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_NAME] = value; }
		}
		/// <summary>商品バリエーション名1</summary>
		[DbMapName(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1)]
		public string VariationName1
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1] = value; }
		}
		/// <summary>商品バリエーション名2</summary>
		[DbMapName(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2)]
		public string VariationName2
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2] = value; }
		}
		/// <summary>商品バリエーション名3</summary>
		[DbMapName(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3)]
		public string VariationName3
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3] = value; }
		}
		/// <summary>税込みフラグ</summary>
		public string TaxIncludedFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_TAX_INCLUDED_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_TAX_INCLUDED_FLG] = value; }
		}
		/// <summary>商品表示価格</summary>
		public string DisplayPrice
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_PRICE]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_PRICE] = value; }
		}
		/// <summary>商品表示特別価格</summary>
		public string DisplaSpecialyPrice
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE] = value; }
		}
		/// <summary>商品定期初回購入価格</summary>
		public string FixedPurchaseFirsttimePrice
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FIRSTTIME_PRICE]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FIRSTTIME_PRICE] = value; }
		}
		/// <summary>商品定期購入価格</summary>
		public string FixedPurchasePrice
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_PRICE]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_PRICE] = value; }
		}
		/// <summary>商品バリエーション価格</summary>
		public string VariationPrice
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_PRICE]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_PRICE] = value; }
		}
		/// <summary>商品バリエーション表示特別価格</summary>
		public string VariationSpecialyPrice
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE] = value; }
		}
		/// <summary>商品バリエーション定期初回購入価格</summary>
		public string VariationFixedPurchaseFirsttimePrice
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE] = value; }
		}
		/// <summary>商品バリエーション定期購入価格</summary>
		public string VariationFixedPurchasePrice
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE] = value; }
		}
		/// <summary>商品セール価格</summary>
		public string SalePrice
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSALEPRICE_SALE_PRICE]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSALEPRICE_SALE_PRICE] = value; }
		}
		/// <summary>定期購入フラグ</summary>
		public string FixedPurchaseFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG] = value; }
		}
		/// <summary>在庫管理方法</summary>
		public string StockManagementKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN] = value; }
		}
		/// <summary>標準在庫文言</summary>
		public string StockMessageDef
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_DEF]); }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_DEF] = value; }
		}
		/// <summary>在庫文言</summary>
		public string StockMessage
		{
			get { return (string)this.DataSource["stock_message"]; }
			set { this.DataSource["stock_message"] = value; }
		}
		/// <summary>商品在庫文言ID</summary>
		public string StockMessageId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_STOCK_MESSAGE_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_STOCK_MESSAGE_ID] = value; }
		}
		/// <summary>在庫文言表示カラム名称</summary>
		/// <remarks>
		/// 在庫文言の表示に使用しているカラム名称
		/// stock_datum1～7、stock_message_defのいずれかが設定される
		/// </remarks>
		public string StockMessageDisplayColumnName
		{
			get { return (string)this.DataSource["stock_message_display_column_name"]; }
			set { this.DataSource["stock_message_display_column_name"] = value; }
		}
		/// <summary>複数バリエーション使用フラグ</summary>
		public string UseVariationFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_USE_VARIATION_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_USE_VARIATION_FLG] = value; }
		}
		#endregion
	}
}
