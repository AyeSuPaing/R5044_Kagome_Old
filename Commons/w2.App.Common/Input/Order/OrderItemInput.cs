/*
=========================================================================================================
  Module      : 注文商品情報入力クラス (OrderItemInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Linq;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.Product;
using w2.App.Common.Util;
using w2.Common.Util;
using w2.Domain.Order;

namespace w2.App.Common.Input.Order
{
	/// <summary>
	/// 注文商品情報入力クラス（登録、編集で利用）
	/// </summary>
	[Serializable]
	public class OrderItemInput : InputBase<OrderItemModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public OrderItemInput()
		{
			this.DeleteTarget = false;
			this.DeleteTargetSet = false;
			this.OrderId = string.Empty;
			this.OrderItemNo = "0";
			this.OrderShippingNo = "0";
			this.ShopId = Constants.CONST_DEFAULT_SHOP_ID;
			this.ProductId = string.Empty;
			this.VariationId = string.Empty;
			this.VId = string.Empty;
			this.ProductName = string.Empty;
			this.ProductNameKana = string.Empty;
			this.ProductPrice = "0";
			this.ProductPriceOrg = "0";
			this.ProductPoint = "0";
			this.ProductTaxIncludedFlg = TaxCalculationUtility.GetPrescribedOrderItemTaxIncludedFlag();
			this.ProductTaxRate = "0";
			this.ProductTaxRoundType = Constants.TAX_EXCLUDED_FRACTION_ROUNDING;
			this.ProductPricePretax = "0";
			this.ProductPriceShip = null;
			this.ProductPriceCost = null;
			this.ProductPointKbn = "0";
			this.ItemRealstockReserved = "0";
			this.ItemRealstockShipped = "0";
			this.ItemQuantity = "1";
			this.ItemQuantitySingle = "1";
			this.ItemPrice = "0";
			this.ItemPriceSingle = "0";
			this.ItemPriceTax = "0";
			this.ProductSetId = string.Empty;
			this.ProductSetNo = null;
			this.ProductSetCount = null;
			this.ItemReturnExchangeKbn = Constants.FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_UNKNOWN;
			this.ItemMemo = string.Empty;
			this.ItemPoint = "0";
			this.ItemCancelFlg = "0";
			this.ItemCancelDate = null;
			this.ItemReturnFlg = "0";
			this.ItemReturnDate = null;
			this.DelFlg = "0";
			this.DateCreated = DateTime.Now.ToString();
			this.DateChanged = DateTime.Now.ToString();
			this.ProductOptionTexts = string.Empty;
			this.BrandId = string.Empty;
			this.DownloadUrl = string.Empty;
			this.ProductsaleId = string.Empty;
			this.CooperationId1 = string.Empty;
			this.CooperationId2 = string.Empty;
			this.CooperationId3 = string.Empty;
			this.CooperationId4 = string.Empty;
			this.CooperationId5 = string.Empty;
			this.OrderSetpromotionNo = null;
			this.OrderSetpromotionItemNo = null;
			this.StockReturnedFlg = Constants.FLG_ORDERITEM_STOCK_RETURNED_FLG_NORETURN;
			this.NoveltyId = string.Empty;
			this.RecommendId = string.Empty;
			this.FixedPurchaseProductFlg = Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_OFF;
			this.ProductBundleId = string.Empty;
			this.BundleItemDisplayType = Constants.FLG_ORDERITEM_BUNDLE_ITEM_DISPLAY_TYPE_VALID;
			this.ItemIndex = "0";
			this.ShippingSizeKbn = string.Empty;
			this.ColumnForMallOrder = string.Empty;
			this.GiftWrappingId = string.Empty;
			this.GiftMessage = string.Empty;
			this.FixedPurchaseDiscountValue = null;
			this.FixedPurchaseDiscountType = null;
			this.FixedPurchaseItemOrderCount = null;
			this.FixedPurchaseItemShippedCount = null;
			this.ItemDiscountedPrice = "0";
			this.SubscriptionBoxCourseId = string.Empty;
			this.SubscriptionBoxFixedAmount = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="itemIndex">アイテムインデックス</param>
		public OrderItemInput(OrderItemModel model, int itemIndex)
			: this()
		{
			this.OrderId = model.OrderId;
			this.OrderItemNo = model.OrderItemNo.ToString();
			this.OrderShippingNo = model.OrderShippingNo.ToString();
			this.ShopId = model.ShopId;
			this.ProductId = model.ProductId;
			this.VariationId = model.VariationId;
			this.VId = model.VariationId.StartsWith(model.ProductId)
				? model.VariationId.Substring(model.ProductId.Length)
				: model.VariationId;
			this.SupplierId = model.SupplierId;
			this.ProductName = model.ProductName;
			this.ProductNameKana = model.ProductNameKana;
			this.ProductPrice = model.ProductPrice.ToPriceString();
			this.ProductPriceOrg = model.ProductPriceOrg.ToPriceString();
			this.ProductPoint = model.ProductPoint.ToString();
			this.ProductTaxIncludedFlg = model.ProductTaxIncludedFlg;
			this.ProductTaxRate = model.ProductTaxRate.ToString();
			this.ProductTaxRoundType = model.ProductTaxRoundType;
			this.ProductPricePretax = model.ProductPricePretax.ToPriceString();
			this.ProductPriceShip = (model.ProductPriceShip != null) ? model.ProductPriceShip.ToPriceString() : null;
			this.ProductPriceCost = (model.ProductPriceCost != null) ? model.ProductPriceCost.ToPriceString() : null;
			this.ProductPointKbn = model.ProductPointKbn;
			this.ItemRealstockReserved = model.ItemRealstockReserved.ToString();
			this.ItemRealstockShipped = model.ItemRealstockShipped.ToString();
			this.ItemQuantity = model.ItemQuantity.ToString();
			this.ItemQuantitySingle = model.ItemQuantitySingle.ToString();
			this.ItemPrice = model.ItemPrice.ToPriceString();
			this.ItemPriceSingle = model.ItemPriceSingle.ToPriceString();
			this.ItemPriceTax = model.ItemPriceTax.ToPriceString();
			this.ProductSetId = model.ProductSetId;
			this.ProductSetNo = (model.ProductSetNo != null) ? model.ProductSetNo.ToString() : null;
			this.ProductSetCount = (model.ProductSetCount != null) ? model.ProductSetCount.ToString() : null;
			this.ItemReturnExchangeKbn = model.ItemReturnExchangeKbn;
			this.ItemMemo = model.ItemMemo;
			this.ItemPoint = model.ItemPoint.ToString();
			this.ItemCancelFlg = model.ItemCancelFlg;
			this.ItemCancelDate = (model.ItemCancelDate != null) ? model.ItemCancelDate.ToString() : null;
			this.ItemReturnFlg = model.ItemReturnFlg;
			this.ItemReturnDate = (model.ItemReturnDate != null) ? model.ItemReturnDate.ToString() : null;
			this.DelFlg = model.DelFlg;
			this.DateCreated = model.DateCreated.ToString();
			this.DateChanged = model.DateChanged.ToString();
			this.ProductOptionTexts = model.ProductOptionTexts;
			this.BrandId = model.BrandId;
			this.DownloadUrl = model.DownloadUrl;
			this.ProductsaleId = model.ProductsaleId;
			this.CooperationId1 = model.CooperationId1;
			this.CooperationId2 = model.CooperationId2;
			this.CooperationId3 = model.CooperationId3;
			this.CooperationId4 = model.CooperationId4;
			this.CooperationId5 = model.CooperationId5;
			this.CooperationId6 = model.CooperationId6;
			this.CooperationId7 = model.CooperationId7;
			this.CooperationId8 = model.CooperationId8;
			this.CooperationId9 = model.CooperationId9;
			this.CooperationId10 = model.CooperationId10;
			this.OrderSetpromotionNo = (model.OrderSetpromotionNo != null) ? model.OrderSetpromotionNo.ToString() : null;
			this.OrderSetpromotionItemNo = (model.OrderSetpromotionItemNo != null) ? model.OrderSetpromotionItemNo.ToString() : null;
			this.StockReturnedFlg = model.StockReturnedFlg;
			this.NoveltyId = model.NoveltyId;
			this.RecommendId = model.RecommendId;
			this.FixedPurchaseProductFlg = model.FixedPurchaseProductFlg;
			this.ProductBundleId = model.ProductBundleId;
			this.BundleItemDisplayType = model.BundleItemDisplayType;
			this.ItemIndex = itemIndex.ToString();
			this.ShippingSizeKbn = model.ShippingSizeKbn;
			this.ColumnForMallOrder = model.ColumnForMallOrder;
			this.GiftWrappingId = model.GiftWrappingId;
			this.GiftMessage = model.GiftMessage;
			this.FixedPurchaseDiscountType = model.FixedPurchaseDiscountType;
			this.FixedPurchaseDiscountValue = (model.FixedPurchaseDiscountValue != null) ? model.FixedPurchaseDiscountValue.ToString() : null;
			this.FixedPurchaseItemOrderCount = (model.FixedPurchaseItemOrderCount != null) ? model.FixedPurchaseItemOrderCount.ToString() : null;
			this.FixedPurchaseItemShippedCount = (model.FixedPurchaseItemShippedCount != null) ? model.FixedPurchaseItemShippedCount.ToString() : null;
			this.ItemDiscountedPrice = (model.ItemDiscountedPrice != null ) ? model.ItemDiscountedPrice.ToString() : null;
			this.SubscriptionBoxCourseId = model.SubscriptionBoxCourseId;
			this.SubscriptionBoxFixedAmount = StringUtility.ToEmpty(model.SubscriptionBoxFixedAmount);
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="order">注文入力情報</param>
		/// <param name="cartProduct">カート商品情報</param>
		/// <param name="itemQuantity">商品数</param>
		/// <param name="itemQuantitySingle">商品単品数</param>
		/// <param name="orderShippingNo">配送先枝番</param>
		/// <param name="orderItemNo">注文商品枝番</param>
		/// <param name="orderSetPromotionNo">注文セットプロモーション枝番</param>
		/// <param name="orderSetpromotionItemNo">注文セットプロモーション商品枝番</param>
		/// <param name="isDutyFree">免税か</param>
		/// <returns>注文商品入力情報</returns>
		public OrderItemInput(
			OrderInput order,
			CartProduct cartProduct,
			int itemQuantity,
			int itemQuantitySingle,
			int orderShippingNo,
			int orderItemNo,
			int? orderSetPromotionNo,
			int? orderSetpromotionItemNo,
			bool isDutyFree)
			: this()
		{
			this.OrderId = order.OrderId;
			this.OrderItemNo = orderItemNo.ToString();
			this.OrderShippingNo = orderShippingNo.ToString();
			this.ShopId = order.ShopId;
			this.ProductId = cartProduct.ProductId;
			this.VariationId = cartProduct.VariationId;
			this.VId = cartProduct.VId;
			this.SupplierId = cartProduct.SupplierId;
			this.ProductName = cartProduct.ProductJointName;
			this.ProductNameKana = cartProduct.ProductNameKana;
			this.ProductPrice = cartProduct.Price.ToString();
			this.ProductPriceOrg = cartProduct.PriceOrg.ToString();
			this.ProductTaxIncludedFlg = cartProduct.TaxIncludedFlg;
			this.ProductTaxRate = cartProduct.TaxRate.ToString();
			this.ProductTaxRoundType = cartProduct.TaxRoundType;
			this.ProductPricePretax = cartProduct.PricePretax.ToString();
			this.ItemQuantity = itemQuantity.ToString();
			this.ItemQuantitySingle = itemQuantitySingle.ToString();
			this.ItemPrice = PriceCalculator.GetItemPrice(cartProduct.Price, itemQuantity).ToString();
			this.ItemPriceTax = PriceCalculator.GetItemPrice(cartProduct.PriceTax, itemQuantity).ToString();
			this.ItemPriceSingle = PriceCalculator.GetItemPrice(cartProduct.Price, itemQuantitySingle)
				.ToString();
			this.ProductSetId = cartProduct.IsSetItem
				? cartProduct.ProductSet.ProductSetId
				: string.Empty;
			this.ProductSetNo = cartProduct.IsSetItem
				? cartProduct.ProductSet.ProductSetNo.ToString()
				: null;
			this.ProductSetCount = cartProduct.IsSetItem
				? cartProduct.ProductSet.ProductSetCount.ToString()
				: null;
			this.DateCreated = order.DateCreated;
			this.DateChanged = order.DateChanged;
			this.ProductOptionTexts = (cartProduct.ProductOptionSettingList != null)
				? ProductOptionSettingHelper.GetSelectedOptionSettingForOrderItem(cartProduct.ProductOptionSettingList)
				: string.Empty;
			this.BrandId = cartProduct.BrandId;
			this.DownloadUrl = cartProduct.DownloadUrl;
			this.ProductsaleId = cartProduct.ProductSaleId;
			this.CooperationId1 = cartProduct.CooperationId[0];
			this.CooperationId2 = cartProduct.CooperationId[1];
			this.CooperationId3 = cartProduct.CooperationId[2];
			this.CooperationId4 = cartProduct.CooperationId[3];
			this.CooperationId5 = cartProduct.CooperationId[4];
			this.CooperationId6 = cartProduct.CooperationId[5];
			this.CooperationId7 = cartProduct.CooperationId[6];
			this.CooperationId8 = cartProduct.CooperationId[7];
			this.CooperationId9 = cartProduct.CooperationId[8];
			this.CooperationId10 = cartProduct.CooperationId[9];
			this.OrderSetpromotionNo = orderSetPromotionNo.HasValue
				? orderSetPromotionNo.ToString()
				: null;
			this.OrderSetpromotionItemNo = orderSetpromotionItemNo.HasValue
				? orderSetpromotionItemNo.ToString()
				: null;
			this.NoveltyId = cartProduct.NoveltyId;
			this.RecommendId = cartProduct.RecommendId;
			this.FixedPurchaseProductFlg = (cartProduct.AddCartKbn == (Constants.AddCartKbn.FixedPurchase)
				|| (cartProduct.AddCartKbn == Constants.AddCartKbn.SubscriptionBox))
				? Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON
				: Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_OFF;
			this.ProductBundleId = cartProduct.ProductBundleId;
			this.BundleItemDisplayType = cartProduct.BundleItemDisplayType;
			this.FixedPurchaseDiscountType = cartProduct.FixedPurchaseDiscountType;
			this.FixedPurchaseDiscountValue = cartProduct.FixedPurchaseDiscountValue.HasValue
				? cartProduct.FixedPurchaseDiscountValue.ToString()
				: null;
			this.ItemDiscountedPrice = (orderSetPromotionNo == null)
				? cartProduct.DiscountedPriceUnAllocatedToSet.ToString()
				: cartProduct.DiscountedPrice[int.Parse(orderSetPromotionNo.ToString())].ToString();
			this.ShippingSizeKbn = cartProduct.ShippingSizeKbn;
			var beforeProduct = order.Shippings[0].Items
				.Where(p => (p.ShopId == cartProduct.ShopId) && (p.ProductId == cartProduct.ProductId)
					&& (p.VariationId == cartProduct.VariationId)).ToArray();
			this.FixedPurchaseItemOrderCount =
				beforeProduct.Any() ? beforeProduct[0].FixedPurchaseItemOrderCount : "1";
			this.FixedPurchaseItemShippedCount =
				beforeProduct.Any() ? beforeProduct[0].FixedPurchaseItemShippedCount : "1";
			this.SubscriptionBoxCourseId = beforeProduct.Any()
				? beforeProduct[0].SubscriptionBoxCourseId
				: string.Empty;
			this.SubscriptionBoxFixedAmount = beforeProduct.Any()
				? beforeProduct[0].SubscriptionBoxFixedAmount
				: string.Empty;
		}
		#endregion

		#region メソッド
		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns>モデル</returns>
		public override OrderItemModel CreateModel()
		{
			var model = new OrderItemModel
			{
				OrderId = this.OrderId,
				OrderItemNo = int.Parse(this.OrderItemNo),
				OrderShippingNo = int.Parse(this.OrderShippingNo),
				ShopId = this.ShopId,
				ProductId = this.ProductId,
				VariationId = this.VariationId,
				SupplierId = this.SupplierId,
				ProductName = this.ProductName,
				ProductNameKana = this.ProductNameKana,
				ProductPrice = decimal.Parse(this.ProductPrice),
				ProductPriceOrg = decimal.Parse(this.ProductPriceOrg),
				ProductPoint = double.Parse(this.ProductPoint),
				ProductTaxIncludedFlg = this.ProductTaxIncludedFlg,
				ProductTaxRate = decimal.Parse(this.ProductTaxRate),
				ProductTaxRoundType = this.ProductTaxRoundType,
				ProductPricePretax = decimal.Parse(this.ProductPricePretax),
				ProductPriceShip = (this.ProductPriceShip != null) ? decimal.Parse(this.ProductPriceShip) : (decimal?)null,
				ProductPriceCost = (this.ProductPriceCost != null) ? decimal.Parse(this.ProductPriceCost) : (decimal?)null,
				ProductPointKbn = this.ProductPointKbn,
				ItemRealstockReserved = int.Parse(this.ItemRealstockReserved),
				ItemRealstockShipped = int.Parse(this.ItemRealstockShipped),
				ItemQuantity = int.Parse(this.ItemQuantity),
				ItemQuantitySingle = int.Parse(this.ItemQuantitySingle),
				ItemPrice = decimal.Parse(this.ItemPrice),
				ItemPriceSingle = decimal.Parse(this.ItemPriceSingle),
				ItemPriceTax = decimal.Parse(this.ItemPriceTax),
				ProductSetId = this.ProductSetId,
				ProductSetNo = (this.ProductSetNo != null) ? int.Parse(this.ProductSetNo) : (int?)null,
				ProductSetCount = (this.ProductSetCount != null) ? int.Parse(this.ProductSetCount) : (int?)null,
				ItemReturnExchangeKbn = this.ItemReturnExchangeKbn,
				ItemMemo = this.ItemMemo,
				ItemPoint = double.Parse(this.ItemPoint),
				ItemCancelFlg = this.ItemCancelFlg,
				ItemCancelDate = (this.ItemCancelDate != null) ? DateTime.Parse(this.ItemCancelDate) : (DateTime?)null,
				ItemReturnFlg = this.ItemReturnFlg,
				ItemReturnDate = (this.ItemReturnDate != null) ? DateTime.Parse(this.ItemReturnDate) : (DateTime?)null,
				DelFlg = this.DelFlg,
				DateCreated = DateTime.Parse(this.DateCreated),
				DateChanged = DateTime.Parse(this.DateChanged),
				ProductOptionTexts = this.ProductOptionTexts,
				BrandId = this.BrandId,
				DownloadUrl = this.DownloadUrl,
				ProductsaleId = this.ProductsaleId,
				CooperationId1 = this.CooperationId1,
				CooperationId2 = this.CooperationId2,
				CooperationId3 = this.CooperationId3,
				CooperationId4 = this.CooperationId4,
				CooperationId5 = this.CooperationId5,
				CooperationId6 = this.CooperationId6,
				CooperationId7 = this.CooperationId7,
				CooperationId8 = this.CooperationId8,
				CooperationId9 = this.CooperationId9,
				CooperationId10 = this.CooperationId10,
				OrderSetpromotionNo = (this.OrderSetpromotionNo != null) ? int.Parse(this.OrderSetpromotionNo) : (int?)null,
				OrderSetpromotionItemNo = (this.OrderSetpromotionItemNo != null) ? int.Parse(this.OrderSetpromotionItemNo) : (int?)null,
				StockReturnedFlg = this.StockReturnedFlg,
				NoveltyId = this.NoveltyId,
				RecommendId = this.RecommendId,
				FixedPurchaseProductFlg = this.FixedPurchaseProductFlg,
				ProductBundleId = this.ProductBundleId,
				BundleItemDisplayType = this.BundleItemDisplayType,
				ShippingSizeKbn = this.ShippingSizeKbn,
				ColumnForMallOrder = this.ColumnForMallOrder,
				FixedPurchaseDiscountType = this.FixedPurchaseDiscountType,
				FixedPurchaseDiscountValue = (this.FixedPurchaseDiscountValue != null)
					? decimal.Parse(this.FixedPurchaseDiscountValue)
					: (decimal?)null,
				FixedPurchaseItemOrderCount = string.IsNullOrEmpty(this.FixedPurchaseItemOrderCount)
					? 0
					: int.Parse(this.FixedPurchaseItemOrderCount),
				FixedPurchaseItemShippedCount = string.IsNullOrEmpty(this.FixedPurchaseItemShippedCount)
					? 0
					: int.Parse(this.FixedPurchaseItemShippedCount),
				ItemDiscountedPrice = string.IsNullOrEmpty(this.ItemDiscountedPrice)
					? (decimal?) null
					: decimal.Parse(this.ItemDiscountedPrice),
				SubscriptionBoxCourseId = this.SubscriptionBoxCourseId,
				SubscriptionBoxFixedAmount = string.IsNullOrEmpty(this.SubscriptionBoxFixedAmount) == false
					? decimal.Parse(this.SubscriptionBoxFixedAmount)
					: (decimal?)null
			};
			return model;
		}

		/// <summary>
		/// 検証
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public w2.Common.Util.Validator.ErrorMessageList Validate()
		{
			if (this.DeleteTarget) return new w2.Common.Util.Validator.ErrorMessageList();

			return w2.Common.Util.Validator.Validate("OrderItemModifyInput", this.DataSource);
		}

		/// <summary>
		/// 表示用の商品付帯情報選択値取得
		/// </summary>
		/// <returns>表示用の商品付帯情報選択値</returns>
		public string GetDisplayProductOptionTexts()
		{
			var displayProductOptionTexts = ProductOptionSettingHelper.GetDisplayProductOptionTexts(this.ProductOptionTexts);
			return displayProductOptionTexts;
		}

		/// <summary>
		/// 同一商品か
		/// </summary>
		/// <param name="orderItem">注文商品情報</param>
		/// <returns>結果</returns>
		public bool IsSameProduct(OrderItemInput orderItem)
		{
			var result = (this.ShopId == orderItem.ShopId)
				&& (this.ProductId == orderItem.ProductId)
				&& (this.VariationId == orderItem.VariationId)
				&& (this.OrderShippingNo == orderItem.OrderShippingNo)
				&& (this.ProductOptionTexts == orderItem.ProductOptionTexts)
				&& (this.IsFixedPurchaseItem == orderItem.IsFixedPurchaseItem)
				&& (this.IsSubscriptionBox == orderItem.IsSubscriptionBox);
			return result;
		}

		/// <summary>
		/// 商品価格を含む同一商品か
		/// </summary>
		/// <param name="orderItem">注文商品情報</param>
		/// <returns>結果</returns>
		public bool IsSameProductWithPrice(OrderItemInput orderItem)
		{
			var result = IsSameProduct(orderItem)
				&& (this.ProductPrice.ToPriceDecimal() == orderItem.ProductPrice.ToPriceDecimal());
			return result;
		}

		/// <summary>
		/// 複製商品判定（更新前情報との比較時）
		/// </summary>
		/// <param name="oldOrderItem">更新前注文商品情報</param>
		/// <returns>結果</returns>
		public bool CheckOrderHistoryModifyCopyTargetByOldOrderItem(OrderItemInput oldOrderItem)
		{
			var result = (IsSameProductWithPrice(oldOrderItem)
					&& (this.IsOrderHistoryModifyCopyTarget == false))
				|| ((IsSameProductWithPrice(oldOrderItem) == false)
					&& IsSameProduct(oldOrderItem)
					&& this.IsOrderHistoryModifyCopyTarget);
			return result;
		}
		#endregion

		#region プロパティ
		/// <summary>注文ID</summary>
		public string OrderId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_ORDER_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_ORDER_ID] = value; }
		}
		/// <summary>注文商品枝番</summary>
		public string OrderItemNo
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_ORDER_ITEM_NO]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_ORDER_ITEM_NO] = value; }
		}
		/// <summary>配送先枝番</summary>
		public string OrderShippingNo
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_ORDER_SHIPPING_NO]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_ORDER_SHIPPING_NO] = value; }
		}
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_SHOP_ID] = value; }
		}
		/// <summary>商品ID</summary>
		public string ProductId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_ID] = value; }
		}
		/// <summary>商品バリエーションID</summary>
		public string VariationId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_VARIATION_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_VARIATION_ID] = value; }
		}
		/// <summary>商品IDを含まないバリエーションID</summary>
		public string VId
		{
			get { return (string)this.DataSource["v_id"]; }
			set { this.DataSource["v_id"] = value; }
		}
		/// <summary>サプライヤID</summary>
		public string SupplierId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_SUPPLIER_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_SUPPLIER_ID] = value; }
		}
		/// <summary>商品名</summary>
		public string ProductName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_NAME] = value; }
		}
		/// <summary>商品名かな</summary>
		public string ProductNameKana
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_NAME_KANA]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_NAME_KANA] = value; }
		}
		/// <summary>商品単価</summary>
		public string ProductPrice
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_PRICE]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_PRICE] = value; }
		}
		/// <summary>商品単価（値引き前）</summary>
		public string ProductPriceOrg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_PRICE_ORG]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_PRICE_ORG] = value; }
		}
		/// <summary>商品ポイント</summary>
		public string ProductPoint
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_POINT]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_POINT] = value; }
		}
		/// <summary>税込フラグ</summary>
		public string ProductTaxIncludedFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_TAX_INCLUDED_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_TAX_INCLUDED_FLG] = value; }
		}
		/// <summary>税率</summary>
		public string ProductTaxRate
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_TAX_RATE]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_TAX_RATE] = value; }
		}
		/// <summary>税計算方法</summary>
		public string ProductTaxRoundType
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_TAX_ROUND_TYPE]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_TAX_ROUND_TYPE] = value; }
		}
		/// <summary>税込販売価格</summary>
		public string ProductPricePretax
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_PRICE_PRETAX]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_PRICE_PRETAX] = value; }
		}
		/// <summary>配送料</summary>
		public string ProductPriceShip
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_PRICE_SHIP]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_PRICE_SHIP] = value; }
		}
		/// <summary>商品原価</summary>
		public string ProductPriceCost
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_PRICE_COST]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_PRICE_COST] = value; }
		}
		/// <summary>付与ポイント区分</summary>
		public string ProductPointKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_POINT_KBN]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_POINT_KBN] = value; }
		}
		/// <summary>実在庫引当済み商品数</summary>
		public string ItemRealstockReserved
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_ITEM_REALSTOCK_RESERVED]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_ITEM_REALSTOCK_RESERVED] = value; }
		}
		/// <summary>実在庫出荷済み商品数</summary>
		public string ItemRealstockShipped
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_ITEM_REALSTOCK_SHIPPED]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_ITEM_REALSTOCK_SHIPPED] = value; }
		}
		/// <summary>注文数</summary>
		public string ItemQuantity
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_ITEM_QUANTITY]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_ITEM_QUANTITY] = value; }
		}
		/// <summary>注文数（セット未考慮）</summary>
		public string ItemQuantitySingle
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_ITEM_QUANTITY_SINGLE]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_ITEM_QUANTITY_SINGLE] = value; }
		}
		/// <summary>明細金額（小計）</summary>
		public string ItemPrice
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_ITEM_PRICE]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_ITEM_PRICE] = value; }
		}
		/// <summary>明細金額（小計・セット未考慮）</summary>
		public string ItemPriceSingle
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_ITEM_PRICE_SINGLE]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_ITEM_PRICE_SINGLE] = value; }
		}
		/// <summary>商品セットID</summary>
		public string ProductSetId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_SET_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_SET_ID] = value; }
		}
		/// <summary>商品セット枝番</summary>
		public string ProductSetNo
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_SET_NO]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_SET_NO] = value; }
		}
		/// <summary>商品セット注文数</summary>
		public string ProductSetCount
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_SET_COUNT]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_SET_COUNT] = value; }
		}
		/// <summary>商品セット？</summary>
		public bool IsProductSet
		{
			get { return (string.IsNullOrEmpty(this.ProductSetId) == false); }
		}
		/// <summary>返品交換区分</summary>
		public string ItemReturnExchangeKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN] = value; }
		}
		/// <summary>明細備考</summary>
		public string ItemMemo
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_ITEM_MEMO]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_ITEM_MEMO] = value; }
		}
		/// <summary>明細ポイント</summary>
		public string ItemPoint
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_ITEM_POINT]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_ITEM_POINT] = value; }
		}
		/// <summary>キャンセルフラグ</summary>
		public string ItemCancelFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_ITEM_CANCEL_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_ITEM_CANCEL_FLG] = value; }
		}
		/// <summary>キャンセル日時</summary>
		public string ItemCancelDate
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_ITEM_CANCEL_DATE]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_ITEM_CANCEL_DATE] = value; }
		}
		/// <summary>返品フラグ</summary>
		public string ItemReturnFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_ITEM_RETURN_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_ITEM_RETURN_FLG] = value; }
		}
		/// <summary>返品日時</summary>
		public string ItemReturnDate
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_ITEM_RETURN_DATE]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_ITEM_RETURN_DATE] = value; }
		}
		/// <summary>削除フラグ</summary>
		public string DelFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_DEL_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_DEL_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public string DateCreated
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public string DateChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_DATE_CHANGED] = value; }
		}
		/// <summary>商品付帯情報選択値</summary>
		public string ProductOptionTexts
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS] = value; }
		}
		/// <summary>ブランドID</summary>
		public string BrandId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_BRAND_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_BRAND_ID] = value; }
		}
		/// <summary>ダウンロードURL</summary>
		public string DownloadUrl
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_DOWNLOAD_URL]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_DOWNLOAD_URL] = value; }
		}
		/// <summary>商品セールID</summary>
		public string ProductsaleId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCTSALE_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCTSALE_ID] = value; }
		}
		/// <summary>商品連携ID1</summary>
		public string CooperationId1
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID1]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID1] = value; }
		}
		/// <summary>商品連携ID2</summary>
		public string CooperationId2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID2]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID2] = value; }
		}
		/// <summary>商品連携ID3</summary>
		public string CooperationId3
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID3]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID3] = value; }
		}
		/// <summary>商品連携ID4</summary>
		public string CooperationId4
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID4]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID4] = value; }
		}
		/// <summary>商品連携ID5</summary>
		public string CooperationId5
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID5]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID5] = value; }
		}
		/// <summary>商品連携ID6</summary>
		public string CooperationId6
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID6]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID6] = value; }
		}
		/// <summary>商品連携ID7</summary>
		public string CooperationId7
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID7]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID7] = value; }
		}
		/// <summary>商品連携ID8</summary>
		public string CooperationId8
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID8]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID8] = value; }
		}
		/// <summary>商品連携ID9</summary>
		public string CooperationId9
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID9]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID9] = value; }
		}
		/// <summary>商品連携ID10</summary>
		public string CooperationId10
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID10]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_COOPERATION_ID10] = value; }
		}
		/// <summary>注文セットプロモーション枝番</summary>
		public string OrderSetpromotionNo
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO] = value; }
		}
		/// <summary>注文セットプロモーション商品枝番</summary>
		public string OrderSetpromotionItemNo
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_ITEM_NO]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_ITEM_NO] = value; }
		}
		/// <summary>在庫戻し済みフラグ</summary>
		public string StockReturnedFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_STOCK_RETURNED_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_STOCK_RETURNED_FLG] = value; }
		}
		/// <summary>ノベルティID</summary>
		public string NoveltyId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_NOVELTY_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_NOVELTY_ID] = value; }
		}
		/// <summary>レコメンドID</summary>
		public string RecommendId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_RECOMMEND_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_RECOMMEND_ID] = value; }
		}
		/// <summary>定期購入フラグ</summary>
		public string FixedPurchaseProductFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG] = value; }
		}
		/// <summary>商品同梱ID</summary>
		public string ProductBundleId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_BUNDLE_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_BUNDLE_ID] = value; }
		}
		/// <summary>同梱商品明細表示フラグ</summary>
		public string BundleItemDisplayType
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_BUNDLE_ITEM_DISPLAY_TYPE]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_BUNDLE_ITEM_DISPLAY_TYPE] = value; }
		}
		/// <summary>配送サイズ区分</summary>
		public string ShippingSizeKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_SHIPPING_SIZE_KBN]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_SHIPPING_SIZE_KBN] = value; }
		}
		/// <summary>モール用項目</summary>
		public string ColumnForMallOrder
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_COLUMN_FOR_MALL_ORDER]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_COLUMN_FOR_MALL_ORDER] = value; }
		}
		/// <summary>ギフト包装ID</summary>
		public string GiftWrappingId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_GIFT_WRAPPING_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_GIFT_WRAPPING_ID] = value; }
		}
		/// <summary>ギフトメッセージ</summary>
		public string GiftMessage
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_GIFT_MESSAGE]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_GIFT_MESSAGE] = value; }
		}
		/// <summary>定期購入値引</summary>
		public string FixedPurchaseDiscountValue
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_DISCOUNT_VALUE]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_DISCOUNT_VALUE] = value; }
		}
		/// <summary>定期購入値引(種別)</summary>
		public string FixedPurchaseDiscountType
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_DISCOUNT_TYPE]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_DISCOUNT_TYPE] = value; }
		}
		/// <summary>削除対象</summary>
		public bool DeleteTarget
		{
			get { return (bool)this.DataSource["DeleteTarget"]; }
			set { this.DataSource["DeleteTarget"] = value; }
		}
		/// <summary>削除対象（セット商品）</summary>
		public bool DeleteTargetSet
		{
			get { return (bool)this.DataSource["DeleteTargetSet"]; }
			set { this.DataSource["DeleteTargetSet"] = value; }
		}
		/// <summary>画面操作時の商品通番</summary>
		public string ItemIndex
		{
			get { return (string)this.DataSource["ItemIndex"]; }
			set { this.DataSource["ItemIndex"] = value; }
		}
		/// <summary>明細金額（税金額）</summary>
		public string ItemPriceTax
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_ITEM_PRICE_TAX]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_ITEM_PRICE_TAX] = value; }
		}
		/// <summary>明細金額(割引後価格)</summary>
		public string ItemDiscountedPrice
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_DISCOUNTED_PRICE]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_DISCOUNTED_PRICE] = value; }
		}
		/// <summary>頒布会コースID</summary>
		public string SubscriptionBoxCourseId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_COURSE_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_COURSE_ID] = value; }
		}
		/// <summary>頒布会定額価格</summary>
		public string SubscriptionBoxFixedAmount
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_FIXED_AMOUNT]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_FIXED_AMOUNT] = value; }
		}
		/// <remarks>実在庫引当済み？(true：1つ以上引当されている、false：未引当)</remarks>
		public bool IsRealStockReserved
		{
			get
			{
				int itemRealstockReserved;
				int.TryParse(this.ItemRealstockReserved, out itemRealstockReserved);
				return (Constants.REALSTOCK_OPTION_ENABLED && (itemRealstockReserved > 0));
			}
		}
		/// <summary>在庫戻し済みかどうか</summary>
		public bool IsItemStockReturned { get { return (this.StockReturnedFlg == Constants.FLG_ORDERITEM_STOCK_RETURNED_FLG_RETURNED); } }
		/// <summary>在庫管理する商品かどうか</summary>
		public bool IsProductStockManaged
		{
			get
			{
				var product = ProductCommon.GetProductInfo(this.ShopId, this.ProductId, string.Empty, string.Empty);
				if (product.Count == 0) return false;
				var productStockManaged =
					ProductCommon.GetKeyValue(product[0], Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN).ToString();
				return (productStockManaged != Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED);
			}
		}
		/// <summary>在庫戻し可能かどうか</summary>
		public bool IsAllowReturnStock { get { return ((this.IsProductStockManaged) && (this.IsItemStockReturned == false) && (int.Parse(this.ItemQuantity) < 0)); } }
		/// <summary>返品商品かどうか</summary>
		public bool IsReturnItem
		{
			get
			{
				return (this.ItemReturnExchangeKbn == Constants.FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_RETURN);
			}
		}
		/// <summary>交換商品かどうか</summary>
		public bool IsExchangeItem
		{
			get
			{
				return (this.ItemReturnExchangeKbn == Constants.FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_EXCHANGE);
			}
		}
		/// <summary>定期商品かどうか</summary>
		public bool IsFixedPurchaseItem
		{
			get
			{
				return (this.FixedPurchaseProductFlg == Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON);
			}
		}
		/// <summary>商品小計(割引金額の按分処理適用後)</summary>
		public decimal PriceSubtotalAfterDistribution { get; set; }
		/// <summary>定期会員割引率の商品適用あり判定</summary>
		public bool IsApplyFixedPurchaseMemberDiscount
		{
			get
			{
				return ((string.IsNullOrEmpty(this.OrderSetpromotionNo))
					&& (this.IsFixedPurchaseItem == false)
					&& (string.IsNullOrEmpty(this.NoveltyId)));
			}
		}
		/// <summary>会員ランク割引対象</summary>
		public bool IsMemberRankDiscount
		{
			get
			{
				return (Constants.MEMBER_RANK_OPTION_ENABLED
					&& MemberRankOptionUtility.IsDiscountTarget(this.ShopId, this.ProductId));
			}
		}
		/// <summary>Can Delete</summary>
		public bool CanDelete { get; set; }
		/// <summary>定期商品購入回数(注文時点)</summary>
		public string FixedPurchaseItemOrderCount
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_ITEM_ORDER_COUNT]); }
			set { this.DataSource[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_ITEM_ORDER_COUNT] = value; }
		}
		/// <summary>定期商品購入回数(出荷時点)</summary>
		public string FixedPurchaseItemShippedCount
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_ITEM_SHIPPED_COUNT]); }
			set { this.DataSource[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_ITEM_SHIPPED_COUNT] = value; }
		}
		/// <summary> 定期購入商品が初回購入か </summary>
		public bool FixedPurchaseItemIsFirstOrder
		{
			get { return string.IsNullOrEmpty(this.FixedPurchaseItemOrderCount) || (this.FixedPurchaseItemOrderCount == "1"); }
		}
		/// <summary>選択値を反映した商品付帯情報設定</summary>
		public ProductOptionSettingList ProductOptionSettingsWithSelectedValues
		{
			get
			{
				return ProductOptionSettingHelper.GetProductOptionSettingList(
					this.ShopId,
					this.ProductId,
					GetDisplayProductOptionTexts());
			}
		}
		/// <summary>付帯価格</summary>
		public decimal OptionPrice
		{
			get { return ProductOptionSettingHelper.ExtractOptionPriceFromProductOptionTexts(GetDisplayProductOptionTexts()); }
		}
		/// <summary>頒布会商品か</summary>
		public bool IsSubscriptionBox => string.IsNullOrEmpty(this.SubscriptionBoxCourseId) == false;
		/// <summary>頒布会定額コース商品か</summary>
		public bool IsSubscriptionBoxFixedAmount => string.IsNullOrEmpty(this.SubscriptionBoxFixedAmount) == false;
		/// <summary>編集時削除対象か</summary>
		public bool ModifyDeleteTarget { get; set; } = false;
		/// <summary>複製商品か</summary>
		public bool IsOrderHistoryModifyCopyTarget { get; set; } = false;
		#endregion
	}
}
