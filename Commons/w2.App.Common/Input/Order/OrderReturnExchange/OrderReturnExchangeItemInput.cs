/*
=========================================================================================================
  Module      : 注文返品交換商品入力クラス (OrderReturnExchangeItemInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Option;
using w2.App.Common.Util;

namespace w2.App.Common.Input.Order.OrderReturnExchange
{
	/// <summary>
	/// 注文返品交換商品入力クラス
	/// </summary>
	[Serializable]
	public class OrderReturnExchangeItemInput : InputBaseDto
	{
		#region プロパティ
		/// <summary>税率</summary>
		public string ProductTaxRate
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_TAX_RATE]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_TAX_RATE] = value; }
		}
		/// <summary>商品セールID</summary>
		public string ProductSaleId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCTSALE_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCTSALE_ID] = value; }
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
		/// <summary>レコメンドID2</summary>
		public string RecommendId2
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_RECOMMEND_ID + "_2"]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_RECOMMEND_ID + "_2"] = value; }
		}
		/// <summary>商品同梱ID</summary>
		public string ProductBundleId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_BUNDLE_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_BUNDLE_ID] = value; }
		}
		/// <summary>定期商品か？</summary>
		public bool IsFixedPurchaseItem
		{
			get { return (bool)this.DataSource[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG] = value; }
		}
		/// <summary>商品ID</summary>
		public string ProductId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_ID]; }
			set
			{
				this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_ID] = value;
				this.DataSource[Constants.FIELD_ORDERITEM_VARIATION_ID] = this.ProductId + this.VId;
			}
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
		/// <summary>注文セットプロモーション枝番</summary>
		public string OrderSetPromotionNo
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO] = value; }
		}
		/// <summary>商品単価</summary>
		public string ProductPrice
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_PRICE]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_PRICE] = value; }
		}
		/// <summary>注文数</summary>
		public string ItemQuantity
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_ITEM_QUANTITY]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_ITEM_QUANTITY] = value; }
		}
		/// <summary>商品IDを含まないバリエーションID</summary>
		public string VId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_V_ID]; }
			set
			{
				this.DataSource[Constants.FIELD_PRODUCTVARIATION_V_ID] = value;
				this.DataSource[Constants.FIELD_ORDERITEM_VARIATION_ID] = this.ProductId + this.VId;
			}
		}
		/// <summary>画面操作時の商品通番</summary>
		public string ItemIndex
		{
			get { return (string)this.DataSource[Constants.HASH_KEY_ORDER_ITEM_OPERATING_INDEX]; }
			set { this.DataSource[Constants.HASH_KEY_ORDER_ITEM_OPERATING_INDEX] = value; }
		}
		/// <summary>商品付帯情報</summary>
		public string ProductOptionValue
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS]; }
			set { this.DataSource[Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS] = value; }
		}
		/// <summary>バリエーションID</summary>
		public string VariationId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERITEM_VARIATION_ID]; }
		}
		/// <summary>定期購入フラグ</summary>
		public string FixedPurchaseProductFlg
		{
			get
			{
				return this.IsFixedPurchaseItem
					? Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON
					: Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_OFF;
			}
		}
		/// <summary>単価税額</summary>
		public decimal ProductPriceTax
		{
			get
			{
				decimal productPrice;
				decimal.TryParse(this.ProductPrice, out productPrice);
				decimal taxRate;
				decimal.TryParse(this.ProductTaxRate, out taxRate);

				var productPriceTax = TaxCalculationUtility.GetTaxPrice(
					productPrice,
					taxRate,
					Constants.TAX_EXCLUDED_FRACTION_ROUNDING);
				return productPriceTax;
			}
		}
		/// <summary>税込販売価格</summary>
		public string ProductPricePretax
		{
			get
			{
				decimal productPrice;
				decimal.TryParse(this.ProductPrice, out productPrice);

				var productPricePretax = TaxCalculationUtility.GetPriceTaxIncluded(productPrice, this.ProductPriceTax);
				return productPricePretax.ToString();
			}
		}
		/// <summary>明細金額（小計）</summary>
		public string ItemPrice
		{
			get
			{
				decimal productPrice;
				decimal.TryParse(this.ProductPrice, out productPrice);
				int itemQuantity;
				int.TryParse(this.ItemQuantity, out itemQuantity);
				return PriceCalculator.GetItemPrice(productPrice, itemQuantity).ToString();
			}
		}
		/// <summary>明細金額（税金額）</summary>
		public string ItemPriceTax
		{
			get
			{
				int itemQuantity;
				int.TryParse(this.ItemQuantity, out itemQuantity);
				return PriceCalculator.GetItemPrice(this.ProductPriceTax, itemQuantity).ToString();
			}
		}
		/// <summary>明細金額（割引後価格）</summary>
		public string DiscountedPrice
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
		#endregion
	}
}
