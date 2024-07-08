/*
=========================================================================================================
  Module      : 注文商品情報モデル (OrderItemModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.Order
{
	/// <summary>
	/// 注文商品情報モデル
	/// </summary>
	[Serializable]
	public partial class OrderItemModel : ModelBase<OrderItemModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public OrderItemModel()
		{
			this.OrderId = "";
			this.OrderItemNo = 1;
			this.OrderShippingNo = 1;
			this.ShopId = "";
			this.ProductId = "";
			this.VariationId = "";
			this.SupplierId = "";
			this.ProductName = "";
			this.ProductNameKana = "";
			this.ProductPrice = 0;
			this.ProductPriceOrg = 0;
			this.ProductPoint = 0;
			this.ProductTaxIncludedFlg = Constants.FLG_ORDERITEM_PRODUCT_TAX_INCLUDED_FLG_OFF;
			this.ProductTaxRate = 0;
			this.ProductTaxRoundType = Constants.FLG_ORDERITEM_PRODUCT_TAX_ROUND_TYPE_ROUNDDOWN;
			this.ProductPricePretax = 0;
			this.ProductPriceShip = null;
			this.ProductPriceCost = null;
			this.ProductPointKbn = "0";
			this.ItemRealstockReserved = 0;
			this.ItemRealstockShipped = 0;
			this.ItemQuantity = 1;
			this.ItemQuantitySingle = 1;
			this.ItemPrice = 0;
			this.ItemPriceSingle = 0;
			this.ItemPriceTax = 0;
			this.ProductSetId = "";
			this.ProductSetNo = null;
			this.ProductSetCount = null;
			this.ItemReturnExchangeKbn = Constants.FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_UNKNOWN;
			this.ItemMemo = "";
			this.ItemPoint = 0;
			this.ItemCancelFlg = "0";
			this.ItemCancelDate = null;
			this.ItemReturnFlg = "0";
			this.ItemReturnDate = null;
			this.DelFlg = "0";
			this.ProductOptionTexts = "";
			this.BrandId = "";
			this.DownloadUrl = "";
			this.ProductsaleId = "";
			this.CooperationId1 = "";
			this.CooperationId2 = "";
			this.CooperationId3 = "";
			this.CooperationId4 = "";
			this.CooperationId5 = "";
			this.OrderSetpromotionNo = null;
			this.OrderSetpromotionItemNo = null;
			this.StockReturnedFlg = Constants.FLG_ORDERITEM_STOCK_RETURNED_FLG_NORETURN;
			this.NoveltyId = "";
			this.RecommendId = "";
			this.FixedPurchaseProductFlg = Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_OFF;
			this.ProductBundleId = "";
			this.BundleItemDisplayType = Constants.FLG_ORDERITEM_BUNDLE_ITEM_DISPLAY_TYPE_VALID;
			this.OrderHistoryDisplayType = Constants.FLG_ORDERITEM_ORDER_HISTORY_DISPLAY_TYPE_VALID;
			this.LimitedPaymentIds = "";
			this.PluralShippingPriceFreeFlg = "0";
			this.ShippingSizeKbn = null;
			this.ColumnForMallOrder = string.Empty;
			this.CooperationId6 = string.Empty;
			this.CooperationId7 = string.Empty;
			this.CooperationId8 = string.Empty;
			this.CooperationId9 = string.Empty;
			this.CooperationId10 = string.Empty;
			this.GiftWrappingId = string.Empty;
			this.GiftMessage = string.Empty;
			this.FixedPurchaseDiscountType = null;
			this.FixedPurchaseDiscountValue = null;
			this.FixedPurchaseItemOrderCount = null;
			this.FixedPurchaseItemShippedCount = null;
			this.ItemDiscountedPrice = 0;
			this.SubscriptionBoxCourseId = string.Empty;
			this.SubscriptionBoxFixedAmount = null;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public OrderItemModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public OrderItemModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>注文ID</summary>
		[UpdateData(1, "order_id")]
		public string OrderId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_ORDER_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_ORDER_ID] = value; }
		}
		/// <summary>注文商品枝番</summary>
		[UpdateData(2, "order_item_no")]
		public int OrderItemNo
		{
			get { return (int)this.DataSource[Constants.FIELD_ORDERITEM_ORDER_ITEM_NO]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_ORDER_ITEM_NO] = value; }
		}
		/// <summary>配送先枝番</summary>
		[UpdateData(3, "order_shipping_no")]
		public int OrderShippingNo
		{
			get { return (int)this.DataSource[Constants.FIELD_ORDERITEM_ORDER_SHIPPING_NO]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_ORDER_SHIPPING_NO] = value; }
		}
		/// <summary>店舗ID</summary>
		[UpdateData(4, "shop_id")]
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_SHOP_ID] = value; }
		}
		/// <summary>商品ID</summary>
		[UpdateData(5, "product_id")]
		public string ProductId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_ID] = value; }
		}
		/// <summary>商品バリエーションID</summary>
		[UpdateData(6, "variation_id")]
		public string VariationId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_VARIATION_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_VARIATION_ID] = value; }
		}
		/// <summary>サプライヤID</summary>
		[UpdateData(7, "supplier_id")]
		public string SupplierId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_SUPPLIER_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_SUPPLIER_ID] = value; }
		}
		/// <summary>商品名</summary>
		[UpdateData(8, "product_name")]
		public string ProductName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_NAME] = value; }
		}
		/// <summary>商品名かな</summary>
		[UpdateData(9, "product_name_kana")]
		public string ProductNameKana
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_NAME_KANA]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_NAME_KANA] = value; }
		}
		/// <summary>商品単価</summary>
		[UpdateData(10, "product_price")]
		public decimal ProductPrice
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_PRICE]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_PRICE] = value; }
		}
		/// <summary>商品単価（値引き前）</summary>
		[UpdateData(11, "product_price_org")]
		public decimal ProductPriceOrg
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_PRICE_ORG]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_PRICE_ORG] = value; }
		}
		/// <summary>商品ポイント</summary>
		[UpdateData(12, "product_point")]
		public double ProductPoint
		{
			get { return (double)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_POINT]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_POINT] = value; }
		}
		/// <summary>税込フラグ</summary>
		[UpdateData(13, "product_tax_included_flg")]
		public string ProductTaxIncludedFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_TAX_INCLUDED_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_TAX_INCLUDED_FLG] = value; }
		}
		/// <summary>税率</summary>
		[UpdateData(14, "product_tax_rate")]
		public decimal ProductTaxRate
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_TAX_RATE]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_TAX_RATE] = value; }
		}
		/// <summary>税計算方法</summary>
		[UpdateData(15, "product_tax_round_type")]
		public string ProductTaxRoundType
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_TAX_ROUND_TYPE]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_TAX_ROUND_TYPE] = value; }
		}
		/// <summary>税込販売価格</summary>
		[UpdateData(16, "product_price_pretax")]
		public decimal ProductPricePretax
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_PRICE_PRETAX]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_PRICE_PRETAX] = value; }
		}
		/// <summary>配送料</summary>
		[UpdateData(17, "product_price_ship")]
		public decimal? ProductPriceShip
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_PRICE_SHIP] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_PRICE_SHIP];
			}
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_PRICE_SHIP] = value; }
		}
		/// <summary>商品原価</summary>
		[UpdateData(18, "product_price_cost")]
		public decimal? ProductPriceCost
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_PRICE_COST] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_PRICE_COST];
			}
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_PRICE_COST] = value; }
		}
		/// <summary>付与ポイント区分</summary>
		[UpdateData(19, "product_point_kbn")]
		public string ProductPointKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_POINT_KBN]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_POINT_KBN] = value; }
		}
		/// <summary>実在庫引当済み商品数</summary>
		[UpdateData(20, "item_realstock_reserved")]
		public int ItemRealstockReserved
		{
			get { return (int)this.DataSource[Constants.FIELD_ORDERITEM_ITEM_REALSTOCK_RESERVED]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_ITEM_REALSTOCK_RESERVED] = value; }
		}
		/// <summary>実在庫出荷済み商品数</summary>
		[UpdateData(21, "item_realstock_shipped")]
		public int ItemRealstockShipped
		{
			get { return (int)this.DataSource[Constants.FIELD_ORDERITEM_ITEM_REALSTOCK_SHIPPED]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_ITEM_REALSTOCK_SHIPPED] = value; }
		}
		/// <summary>注文数</summary>
		[UpdateData(22, "item_quantity")]
		public int ItemQuantity
		{
			get { return (int)this.DataSource[Constants.FIELD_ORDERITEM_ITEM_QUANTITY]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_ITEM_QUANTITY] = value; }
		}
		/// <summary>注文数（セット未考慮）</summary>
		[UpdateData(23, "item_quantity_single")]
		public int ItemQuantitySingle
		{
			get { return (int)this.DataSource[Constants.FIELD_ORDERITEM_ITEM_QUANTITY_SINGLE]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_ITEM_QUANTITY_SINGLE] = value; }
		}
		/// <summary>明細金額（小計）</summary>
		[UpdateData(24, "item_price")]
		public decimal ItemPrice
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDERITEM_ITEM_PRICE]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_ITEM_PRICE] = value; }
		}
		/// <summary>明細金額（小計・セット未考慮）</summary>
		[UpdateData(25, "item_price_single")]
		public decimal ItemPriceSingle
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDERITEM_ITEM_PRICE_SINGLE]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_ITEM_PRICE_SINGLE] = value; }
		}
		/// <summary>商品セットID</summary>
		[UpdateData(26, "product_set_id")]
		public string ProductSetId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_SET_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_SET_ID] = value; }
		}
		/// <summary>商品セット枝番</summary>
		[UpdateData(27, "product_set_no")]
		public int? ProductSetNo
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_SET_NO] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_SET_NO];
			}
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_SET_NO] = value; }
		}
		/// <summary>商品セット注文数</summary>
		[UpdateData(28, "product_set_count")]
		public int? ProductSetCount
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_SET_COUNT] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_SET_COUNT];
			}
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_SET_COUNT] = value; }
		}
		/// <summary>返品交換区分</summary>
		[UpdateData(29, "item_return_exchange_kbn")]
		public string ItemReturnExchangeKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN] = value; }
		}
		/// <summary>明細備考</summary>
		[UpdateData(30, "item_memo")]
		public string ItemMemo
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_ITEM_MEMO]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_ITEM_MEMO] = value; }
		}
		/// <summary>明細ポイント</summary>
		[UpdateData(31, "item_point")]
		public double ItemPoint
		{
			get { return (double)this.DataSource[Constants.FIELD_ORDERITEM_ITEM_POINT]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_ITEM_POINT] = value; }
		}
		/// <summary>キャンセルフラグ</summary>
		[UpdateData(32, "item_cancel_flg")]
		public string ItemCancelFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_ITEM_CANCEL_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_ITEM_CANCEL_FLG] = value; }
		}
		/// <summary>キャンセル日時</summary>
		[UpdateData(33, "item_cancel_date")]
		public DateTime? ItemCancelDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDERITEM_ITEM_CANCEL_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDERITEM_ITEM_CANCEL_DATE];
			}
			set { this.DataSource[Constants.FIELD_ORDERITEM_ITEM_CANCEL_DATE] = value; }
		}
		/// <summary>返品フラグ</summary>
		[UpdateData(34, "item_return_flg")]
		public string ItemReturnFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_ITEM_RETURN_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_ITEM_RETURN_FLG] = value; }
		}
		/// <summary>返品日時</summary>
		[UpdateData(35, "item_return_date")]
		public DateTime? ItemReturnDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDERITEM_ITEM_RETURN_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDERITEM_ITEM_RETURN_DATE];
			}
			set { this.DataSource[Constants.FIELD_ORDERITEM_ITEM_RETURN_DATE] = value; }
		}
		/// <summary>削除フラグ</summary>
		[UpdateData(36, "del_flg")]
		public string DelFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_DEL_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_DEL_FLG] = value; }
		}
		/// <summary>作成日</summary>
		[UpdateData(37, "date_created")]
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_ORDERITEM_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		[UpdateData(38, "date_changed")]
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_ORDERITEM_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_DATE_CHANGED] = value; }
		}
		/// <summary>商品付帯情報選択値</summary>
		[UpdateData(39, "product_option_texts")]
		public string ProductOptionTexts
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS] = value; }
		}
		/// <summary>ブランドID</summary>
		[UpdateData(40, "brand_id")]
		public string BrandId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_BRAND_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_BRAND_ID] = value; }
		}
		/// <summary>ダウンロードURL</summary>
		[UpdateData(41, "download_url")]
		public string DownloadUrl
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_DOWNLOAD_URL]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_DOWNLOAD_URL] = value; }
		}
		/// <summary>商品セールID</summary>
		[UpdateData(42, "productsale_id")]
		public string ProductsaleId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCTSALE_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCTSALE_ID] = value; }
		}
		/// <summary>商品連携ID1</summary>
		[UpdateData(43, "cooperation_id1")]
		public string CooperationId1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID1]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID1] = value; }
		}
		/// <summary>商品連携ID2</summary>
		[UpdateData(44, "cooperation_id2")]
		public string CooperationId2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID2]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID2] = value; }
		}
		/// <summary>商品連携ID3</summary>
		[UpdateData(45, "cooperation_id3")]
		public string CooperationId3
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID3]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID3] = value; }
		}
		/// <summary>商品連携ID4</summary>
		[UpdateData(46, "cooperation_id4")]
		public string CooperationId4
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID4]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID4] = value; }
		}
		/// <summary>商品連携ID5</summary>
		[UpdateData(47, "cooperation_id5")]
		public string CooperationId5
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID5]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID5] = value; }
		}
		/// <summary>注文セットプロモーション枝番</summary>
		[UpdateData(48, "order_setpromotion_no")]
		public int? OrderSetpromotionNo
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO];
			}
			set { this.DataSource[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO] = value; }
		}
		/// <summary>注文セットプロモーション商品枝番</summary>
		[UpdateData(49, "order_setpromotion_item_no")]
		public int? OrderSetpromotionItemNo
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_ITEM_NO] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_ITEM_NO];
			}
			set { this.DataSource[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_ITEM_NO] = value; }
		}
		/// <summary>在庫戻し済みフラグ</summary>
		[UpdateData(50, "stock_returned_flg")]
		public string StockReturnedFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_STOCK_RETURNED_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_STOCK_RETURNED_FLG] = value; }
		}
		/// <summary>ノベルティID</summary>
		[UpdateData(51, "novelty_id")]
		public string NoveltyId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_NOVELTY_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_NOVELTY_ID] = value; }
		}
		/// <summary>レコメンドID</summary>
		[UpdateData(52, "recommend_id")]
		public string RecommendId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_RECOMMEND_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_RECOMMEND_ID] = value; }
		}
		/// <summary>定期購入フラグ</summary>
		[UpdateData(53, "fixed_purchase_product_flg")]
		public string FixedPurchaseProductFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG] = value; }
		}
		/// <summary>商品同梱ID</summary>
		[UpdateData(54, "product_bundle_id")]
		public string ProductBundleId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_BUNDLE_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_BUNDLE_ID] = value; }
		}
		/// <summary>同梱商品明細表示フラグ</summary>
		[UpdateData(55, "bundle_item_display_type")]
		public string BundleItemDisplayType
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_BUNDLE_ITEM_DISPLAY_TYPE]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_BUNDLE_ITEM_DISPLAY_TYPE] = value; }
		}
		/// <summary>明細表示フラグ</summary>
		[UpdateData(56, "order_history_display_type")]
		public string OrderHistoryDisplayType
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_ORDER_HISTORY_DISPLAY_TYPE]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_ORDER_HISTORY_DISPLAY_TYPE] = value; }
		}
		/// <summary>明細金額（税金額）</summary>
		[UpdateData(57, "item_price_tax")]
		public decimal ItemPriceTax
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDERITEM_ITEM_PRICE_TAX]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_ITEM_PRICE_TAX] = value; }
		}
		/// <summary>決済利用不可</summary>
		public string LimitedPaymentIds
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_LIMITED_PAYMENT_IDS]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_LIMITED_PAYMENT_IDS] = value; }
		}
		/// <summary>配送料金複数個無料フラグ</summary>
		public string PluralShippingPriceFreeFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PLURAL_SHIPPING_PRICE_FREE_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PLURAL_SHIPPING_PRICE_FREE_FLG] = value; }
		}
		/// <summary>配送サイズ区分</summary>
		public string ShippingSizeKbn
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDERITEM_SHIPPING_SIZE_KBN] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_ORDERITEM_SHIPPING_SIZE_KBN];
			}
			set { this.DataSource[Constants.FIELD_ORDERITEM_SHIPPING_SIZE_KBN] = value; }
		}
		/// <summary>モール用項目</summary>
		public string ColumnForMallOrder
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_COLUMN_FOR_MALL_ORDER]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_COLUMN_FOR_MALL_ORDER] = value; }
		}
		/// <summary>商品連携ID6</summary>
		[UpdateData(58, "cooperation_id6")]
		public string CooperationId6
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID6]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID6] = value; }
		}
		/// <summary>商品連携ID7</summary>
		[UpdateData(59, "cooperation_id7")]
		public string CooperationId7
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID7]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID7] = value; }
		}
		/// <summary>商品連携ID8</summary>
		[UpdateData(60, "cooperation_id8")]
		public string CooperationId8
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID8]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID8] = value; }
		}
		/// <summary>商品連携ID9</summary>
		[UpdateData(61, "cooperation_id9")]
		public string CooperationId9
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID9]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID9] = value; }
		}
		/// <summary>商品連携ID10</summary>
		[UpdateData(62, "cooperation_id10")]
		public string CooperationId10
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID10]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID10] = value; }
		}
		/// <summary>ギフト包装ID</summary>
		[UpdateDataAttribute(63, "gift_wrapping_id")]
		public string GiftWrappingId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_GIFT_WRAPPING_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_GIFT_WRAPPING_ID] = value; }
		}
		/// <summary>ギフトメッセージ</summary>
		[UpdateDataAttribute(64, "gift_message")]
		public string GiftMessage
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_GIFT_MESSAGE]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_GIFT_MESSAGE] = value; }
		}
		/// <summary>定期購入値引</summary>
		[UpdateDataAttribute(65, "fixed_purchase_discount_value")]
		public decimal? FixedPurchaseDiscountValue
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_DISCOUNT_VALUE] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_DISCOUNT_VALUE];
			}
			set { this.DataSource[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_DISCOUNT_VALUE] = value; }
		}
		/// <summary>定期購入値引タイプ</summary>
		[UpdateDataAttribute(66, "fixed_purchase_discount_type")]
		public string FixedPurchaseDiscountType
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_DISCOUNT_TYPE] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_DISCOUNT_TYPE];
			}
			set { this.DataSource[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_DISCOUNT_TYPE] = value; }
		}
		/// <summary>定期商品購入回数(注文時点)</summary>
		[UpdateDataAttribute(67, "fixed_purchase_item_order_count")]
		public int? FixedPurchaseItemOrderCount
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_ITEM_ORDER_COUNT] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_ITEM_ORDER_COUNT];
			}
			set { this.DataSource[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_ITEM_ORDER_COUNT] = value; }
		}
		/// <summary>定期商品購入回数(出荷時点)</summary>
		[UpdateDataAttribute(68, "fixed_purchase_item_shipped_count")]
		public int? FixedPurchaseItemShippedCount
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_ITEM_SHIPPED_COUNT] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_ITEM_SHIPPED_COUNT];
			}
			set { this.DataSource[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_ITEM_SHIPPED_COUNT] = value; }
		}
		/// <summary>明細金額（割引後価格）</summary>
		[UpdateData(69, "item_discounted_price")]
		public decimal? ItemDiscountedPrice
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDERITEM_DISCOUNTED_PRICE] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_ORDERITEM_DISCOUNTED_PRICE];
			}
			set { this.DataSource[Constants.FIELD_ORDERITEM_DISCOUNTED_PRICE] = value; }
		}
		/// <summary>頒布会コースID</summary>
		[UpdateData(70, "subscription_box_course_id")]
		public string SubscriptionBoxCourseId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_COURSE_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_COURSE_ID] = value; }
		}
		/// <summary>頒布会定額価格</summary>
		[UpdateData(71, "subscription_box_fixed_amount")]
		public decimal? SubscriptionBoxFixedAmount
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_FIXED_AMOUNT] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_FIXED_AMOUNT];
			}
			set { this.DataSource[Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_FIXED_AMOUNT] = value; }
		}
		#endregion
	}
}
