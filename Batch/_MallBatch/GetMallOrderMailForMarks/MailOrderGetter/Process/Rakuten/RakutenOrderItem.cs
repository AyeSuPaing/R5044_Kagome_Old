/*
=========================================================================================================
  Module      : 楽天注文情報：注文商品クラス (RakutenOrderItem.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Mall.Rakuten;
using w2.App.Common.Option;
using w2.Domain.Order;
using w2.Domain.Product;
using w2.Domain.ProductTaxCategory;

namespace w2.Commerce.MallBatch.MailOrderGetter.Process.Rakuten
{
	///**************************************************************************************
	/// <summary>
	/// 楽天注文情報：注文商品クラス
	/// </summary>
	///**************************************************************************************
	public class RakutenOrderItem : OrderItemModel
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="orderItem">注文商品情報</param>
		public RakutenOrderItem(string shopId, string productId, itemModel orderItem)
		{
			this.ShopId = shopId;
			this.OrderId = "";
			this.ProductId = productId;
			this.VariationId = orderItem.itemNumber;
			this.ProductName = orderItem.itemName;
			this.ProductTaxIncludedFlg = TaxCalculationUtility.GetPrescribedProductTaxIncludedFlag();
			this.ProductTaxRate = ((decimal)orderItem.taxRate * 100);
			this.ProductPriceTax = TaxCalculationUtility.GetTaxPrice(
				orderItem.price,
				this.ProductTaxRate,
				Constants.TAX_ROUNDTYPE,
				orderItem.isIncludedTax);
			this.ProductPrice = TaxCalculationUtility.GetPrescribedPrice(
				orderItem.price,
				this.ProductPriceTax,
				orderItem.isIncludedTax);
			this.ProductPricePretax = orderItem.taxIncludedPrice;
			this.ItemQuantity = orderItem.units;
			this.ItemPriceTax = this.ProductPriceTax * this.ItemQuantity;
			this.ItemPrice = this.ProductPrice * this.ItemQuantity;
		}
		#endregion

		#region プロパティ
		/// <summary>単価税額</summary>
		public decimal ProductPriceTax { get; set; }
		#endregion
	}
}