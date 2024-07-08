/*
=========================================================================================================
  Module      : 注文商品情報クラス(OrderItem.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Collections.Generic;
using w2.App.Common.Option;
using w2.Common.Util;

namespace w2.App.Common.Order
{
	/// <summary>
	/// OrderItem の概要の説明です
	/// </summary>
	[Serializable]
	public class OrderItem
	{
		//------------------------------------------------------
		// 定数
		//------------------------------------------------------
		public const string CONST_DELETE_TARGET_PRODUCT = "delete_target"; // 削除対象商品フラグ
		public const string CONST_DELETE_TARGET_PRODUCT_SET = "delete_target_set"; // 削除対象商品セットフラグ

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public OrderItem()
		{
			this.ProductPrice = "0";
			this.ItemRealStockReserved = "0";
			this.ItemRealStockShipped = "0";
			this.ItemQuantity = "1";
			this.ItemPrice = "0";
			this.ItemPriceTax = "0";
			this.ProductSetId = ""; // よく判定に利用されるので初期化
			this.ProductPricePretax = "0";
			this.ProductTaxIncludedFlg = TaxCalculationUtility.GetPrescribedOrderItemTaxIncludedFlag();
			this.ProductTaxRate = "0";
			this.ProductTaxRoundType = Constants.TAX_EXCLUDED_FRACTION_ROUNDING;
			this.OrderSetPromotionNo = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="item">注文商品情報</param>
		public OrderItem(DataRowView item)
		{
			//-----------------------------------------------------
			// 注文商品情報設定
			//-----------------------------------------------------
			this.OrderId = (string) item[Constants.FIELD_ORDERITEM_ORDER_ID];
			this.OrderItemNo = item[Constants.FIELD_ORDERITEM_ORDER_ITEM_NO].ToString();
			this.OrderShippingNo = item[Constants.FIELD_ORDERITEM_ORDER_SHIPPING_NO].ToString();
			this.ShopId = (string) item[Constants.FIELD_ORDERITEM_SHOP_ID];
			this.ProductId = (string) item[Constants.FIELD_ORDERITEM_PRODUCT_ID];
			this.VariationId = (string) item[Constants.FIELD_ORDERITEM_VARIATION_ID];
			this.SupplierId = (string) item["item_supplier_id"];
			this.BrandId = (string) item[Constants.FIELD_ORDERITEM_BRAND_ID];
			this.ProductName = (string) item[Constants.FIELD_ORDERITEM_PRODUCT_NAME];
			this.ProductNameKana = (string) item[Constants.FIELD_ORDERITEM_PRODUCT_NAME_KANA];
			this.ProductPrice = item[Constants.FIELD_ORDERITEM_PRODUCT_PRICE].ToString();
			this.ItemRealStockReserved = item[Constants.FIELD_ORDERITEM_ITEM_REALSTOCK_RESERVED].ToString();
			this.ItemRealStockShipped = item[Constants.FIELD_ORDERITEM_ITEM_REALSTOCK_SHIPPED].ToString();
			this.ItemQuantity = item[Constants.FIELD_ORDERITEM_ITEM_QUANTITY].ToString();
			this.ItemPrice = item[Constants.FIELD_ORDERITEM_ITEM_PRICE].ToString();
			this.ItemPriceTax = item[Constants.FIELD_ORDERITEM_ITEM_PRICE_TAX].ToString();
			this.DateCreated = item[Constants.FIELD_ORDERITEM_DATE_CREATED].ToString();
			this.DateChanged = item[Constants.FIELD_ORDERITEM_DATE_CHANGED].ToString();
			this.ProductOptionSettingSelectedTexts = (string) item[Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS];
			this.DigitalContentsFlg =
				(item[Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG].ToString() == Constants.FLG_ORDER_DIGITAL_CONTENTS_FLG_ON);
			this.DownloadUrl = (string) item[Constants.FIELD_ORDERITEM_DOWNLOAD_URL];
			this.ProductSaleId = (string) item[Constants.FIELD_ORDERITEM_PRODUCTSALE_ID];
			this.ProductPricePretax = item[Constants.FIELD_ORDERITEM_PRODUCT_PRICE_PRETAX].ToString();
			this.ProductTaxIncludedFlg = (string) item[Constants.FIELD_ORDERITEM_PRODUCT_TAX_INCLUDED_FLG];
			this.ProductTaxRate = item[Constants.FIELD_ORDERITEM_PRODUCT_TAX_RATE].ToString();
			this.ProductTaxRoundType = (string) item[Constants.FIELD_ORDERITEM_PRODUCT_TAX_ROUND_TYPE];
			this.CooperationId = new List<string>();
			for (int index = 1; index <= Constants.COOPERATION_ID_COLUMNS_COUNT; index++)
			{
				this.CooperationId.Add((string) item[Constants.HASH_KEY_COOPERATION_ID + index]);
			}

			// セット商品用
			this.ProductSetId = item[Constants.FIELD_ORDERITEM_PRODUCT_SET_ID].ToString();
			this.ProductSetNo = StringUtility.ToEmpty(item[Constants.FIELD_ORDERITEM_PRODUCT_SET_NO]); // nullの可能性
			this.ProductSetCount = StringUtility.ToEmpty(item[Constants.FIELD_ORDERITEM_PRODUCT_SET_COUNT]); // nullの可能性
			this.ProductPriceOrg = item[Constants.FIELD_ORDERITEM_PRODUCT_PRICE_ORG].ToString();
			this.ItemQuantitySingle = item[Constants.FIELD_ORDERITEM_ITEM_QUANTITY_SINGLE].ToString();
			this.ItemPriceSingle = item[Constants.FIELD_ORDERITEM_ITEM_PRICE_SINGLE].ToString();
			this.DeleteTargetSet = false;

			this.OrderSetPromotionNo = item[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO].ToString();
			this.OrderSetPromotionItemNo = item[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_ITEM_NO].ToString();

			// ノベルティID
			this.NoveltyId = item[Constants.FIELD_ORDERITEM_NOVELTY_ID].ToString();

			// レコメンドID
			this.RecommendId = item[Constants.FIELD_ORDERITEM_RECOMMEND_ID].ToString();

			this.VId = (string) item[Constants.FIELD_PRODUCTVARIATION_V_ID];
			this.DeleteTarget = false;

			this.ItemReturnExchangeKbn = item[Constants.FIELD_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN].ToString();

			// 返品交換在庫フラグ
			this.StockReturnedFlg = item[Constants.FIELD_ORDERITEM_STOCK_RETURNED_FLG].ToString();

			// 定期購入フラグ
			this.FixedPurchaseFlg =
				item.Row.Table.Columns.Contains(Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG + "_product")
					? StringUtility.ToEmpty(item[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG + "_product"])
					: string.Empty;

			// 定期商品フラグ
			this.FixedPurchaseProductFlg = item[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG].ToString();

			this.ProductBundleId = (string) item[Constants.FIELD_ORDERITEM_PRODUCT_BUNDLE_ID];
			this.BundleItemDisplayType = (string) item[Constants.FIELD_ORDERITEM_BUNDLE_ITEM_DISPLAY_TYPE];
			this.OrderHistoryDisplayType = (string) item[Constants.FIELD_ORDERITEM_ORDER_HISTORY_DISPLAY_TYPE];

			// 定期商品購入回数
			this.FixedPurchaseItemOrderCount = (item[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_ITEM_ORDER_COUNT] != DBNull.Value)
				? (int?)item[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_ITEM_ORDER_COUNT]
				: 0;
			this.FixedPurchaseItemShippedCount = (item[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_ITEM_SHIPPED_COUNT] != DBNull.Value)
				? (int?)item[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_ITEM_SHIPPED_COUNT]
				: 0;

			// 割引後商品価格
			this.DiscountedPrice = item[Constants.FIELD_ORDERITEM_DISCOUNTED_PRICE].ToString();
			this.SubscriptionBoxCourseId = (string)item[Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_COURSE_ID_WITH_PREFIX];
			this.SubscriptionBoxFixedAmount = item[Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_FIXED_AMOUNT_WITH_PREFIX] != DBNull.Value
				? (decimal?)item[Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_FIXED_AMOUNT_WITH_PREFIX]
				: null;
		}

		/// <summary>
		/// 実在庫が引当されているか？を返す
		/// </summary>
		/// <returns>実在庫引当済み？(true：1つ以上引当されている、false：未引当)</returns>
		public bool IsRealStockReserved()
		{
			bool blResult = false;

			// 実在庫利用が有効 AND 実在庫引当済み商品数が0より大きい
			if (Constants.REALSTOCK_OPTION_ENABLED && int.Parse(this.ItemRealStockReserved) > 0)
			{
				blResult = true;
			}

			return blResult;
		}

		#region "プロパティ"
		/// <summary>注文ID</summary>
		public string OrderId { get; set; }
		/// <summary>注文商品枝番</summary>
		public string OrderItemNo { get; set; }
		/// <summary>配送先枝番</summary>
		public string OrderShippingNo { get; set; }
		/// <summary>店舗ID</summary>
		public string ShopId { get; set; }
		/// <summary>商品ID</summary>
		public string ProductId { get; set; }
		/// <summary>商品バリエーションID</summary>
		public string VariationId { get; set; }
		/// <summary>サプライヤID</summary>
		public string SupplierId { get; set; }
		/// <summary>ブランドID</summary>
		public string BrandId { get; set; }
		/// <summary>商品名</summary>
		public string ProductName { get; set; }
		/// <summary>商品名かな</summary>
		public string ProductNameKana { get; set; }
		/// <summary>商品単価</summary>
		public string ProductPrice { get; set; }
		/// <summary>実在庫引当済み商品数</summary>
		public string ItemRealStockReserved { get; set; }
		/// <summary>実在庫出荷済み商品数</summary>
		public string ItemRealStockShipped { get; set; }
		/// <summary>注文数</summary>
		public string ItemQuantity { get; set; }
		/// <summary>明細金額（小計）</summary>
		public string ItemPrice { get; set; }
		/// <summary>消費税額</summary>
		public string ItemPriceTax { get; set; }
		/// <summary>作成日</summary>
		public string DateCreated { get; set; }
		/// <summary>更新日</summary>
		public string DateChanged { get; set; }
		/// <summary>商品単価（値引き前）</summary>
		public string ProductPriceOrg { get; set; }
		/// <summary>注文数（セット未考慮）</summary>
		public string ItemQuantitySingle { get; set; }
		/// <summary>明細金額（小計・セット未考慮）</summary>
		public string ItemPriceSingle { get; set; }
		/// <summary>商品セットID</summary>
		public string ProductSetId { get; set; }
		/// <summary>商品セット枝番</summary>
		public string ProductSetNo { get; set; }
		/// <summary>商品セット注文数</summary>
		public string ProductSetCount { get; set; }
		/// <summary>商品IDを含まないバリエーションID</summary>
		public string VId { get; set; }
		/// <summary>削除対象フラグ</summary>
		/// <remarks>True:削除対象 False:削除対象外</remarks>
		public bool DeleteTarget { get; set; }
		/// <summary>削除セット対象フラグ</summary>
		/// <remarks>True:削除対象 False:削除対象外</remarks>
		public bool DeleteTargetSet { get; set; }
		/// <summary>返品交換区分</summary>
		public string ItemReturnExchangeKbn { get; set; }
		/// <summary>商品付帯情報選択値</summary>
		public string ProductOptionSettingSelectedTexts { get; set; }
		/// <summary>デジタルコンテンツフラグ</summary>
		public bool DigitalContentsFlg { get; set; }
		/// <summary>ダウンロードURL</summary>
		public string DownloadUrl { get; set; }
		/// <summary>セールID</summary>
		public string ProductSaleId { get; set; }
		/// <summary>税抜販売価格</summary>
		public string ProductPricePretax { get; set; }
		/// <summary>税込フラグ</summary>
		public string ProductTaxIncludedFlg { get; set; }
		/// <summary>税率</summary>
		public string ProductTaxRate { get; set; }
		/// <summary>税計算方法</summary>
		public string ProductTaxRoundType { get; set; }
		/// <summary>商品連携ID</summary>
		public List<string> CooperationId { get; private set; }
		/// <summary>注文セットプロモーション枝番</summary>
		public string OrderSetPromotionNo { get; set; }
		/// <summary>注文セットプロモーションアイテム枝番</summary>
		public string OrderSetPromotionItemNo { get; set; }
		/// <summary>在庫戻し済みフラグ</summary>
		public string StockReturnedFlg { get; set; }
		/// <summary>在庫戻し済みかどうか</summary>
		public bool IsItemStockReturned
		{
			get { return (this.StockReturnedFlg == Constants.FLG_ORDERITEM_STOCK_RETURNED_FLG_RETURNED); }
		}
		/// <summary>在庫管理する商品かどうか</summary>
		public bool IsProductStockManaged
		{
			get
			{
				return (ProductCommon.GetProductInfo(this.ShopId, this.ProductId, "", string.Empty).Count > 0)
					? (ProductCommon
						   .GetKeyValue(ProductCommon.GetProductInfo(this.ShopId, this.ProductId, "", string.Empty)[0],
							   Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN).ToString() !=
					   Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED)
					: false;
			}
		}
		/// <summary>在庫戻し可能かどうか</summary>
		public bool IsAllowReturnStock
		{
			get
			{
				return ((this.IsProductStockManaged) && (this.IsItemStockReturned == false) &&
				        (int.Parse(this.ItemQuantity) < 0));
			}
		}
		/// <summary>ノベルティID</summary>
		public string NoveltyId { get; set; }
		/// <summary>レコメンドID</summary>
		public string RecommendId { get; set; }
		/// <summary>定期購入フラグ</summary>
		public string FixedPurchaseFlg { get; set; }
		/// <summary>定期商品フラグ</summary>
		public string FixedPurchaseProductFlg { get; set; }
		/// <summary>商品同梱ID</summary>
		public string ProductBundleId { get; set; }
		/// <summary>同梱商品明細表示フラグ</summary>
		public string BundleItemDisplayType { get; set; }
		/// <summary>明細表示フラグ</summary>
		public string OrderHistoryDisplayType { get; set; }
		/// <summary>定期商品購入回数(注文時点)</summary>
		public int? FixedPurchaseItemOrderCount { get; set; }
		/// <summary>定期商品購入回数(出荷時点)</summary>
		public int? FixedPurchaseItemShippedCount { get; set; }
		/// <summary> 定期購入商品が初回購入か</summary>
		public bool FixedPurchaseItemIsFirstOrder
		{
			get { return ((this.FixedPurchaseItemOrderCount.HasValue) && (this.FixedPurchaseItemOrderCount == 1)); }
		}
		/// <summary>割引後商品価格</summary>
		public string DiscountedPrice { get; set; }
		/// <summary>頒布会コースID</summary>
		public string SubscriptionBoxCourseId { get; set; }
		/// <summary>頒布会定額価格</summary>
		public decimal? SubscriptionBoxFixedAmount { get; set; }
		/// <summary>通常商品（返品・交換ではない）か</summary>
		public bool IsNotReturnExchange =>
			this.ItemReturnExchangeKbn == Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_UNKNOWN;
		/// <summary>頒布会商品か</summary>
		public bool IsSubscriptionBox => string.IsNullOrEmpty(this.SubscriptionBoxCourseId) == false;
		/// <summary>頒布会定額コース商品か</summary>
		public bool IsSubscriptionBoxFixedAmount => this.SubscriptionBoxFixedAmount.HasValue;
		#endregion
	}
}
