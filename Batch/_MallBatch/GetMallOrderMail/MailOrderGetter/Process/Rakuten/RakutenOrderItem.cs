/*
=========================================================================================================
  Module      : 楽天注文情報：注文商品クラス (RakutenOrderItem.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Mall.RakutenApi;
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
		/// <param name="product">商品情報</param>
		/// <param name="orderId">注文番号</param>
		/// <param name="rakutenOrderItem">注文商品情報</param>
		/// <remarks>注文商品情報を構成します</remarks>
		public RakutenOrderItem(string shopId, ProductModel product, string orderId, RakutenApiItem rakutenOrderItem)
		{
			this.ShopId = shopId;
			this.OrderId = orderId;
			this.ProductId = product.ProductId;
			this.VariationId = Constants.MALLCOOPERATION_RAKUTEN_SKUMIGRATION
				? product.VariationId
				: rakutenOrderItem.ItemNumber;
			this.ProductName = rakutenOrderItem.ItemName;
			this.ProductTaxIncludedFlg = TaxCalculationUtility.GetPrescribedProductTaxIncludedFlag();
			this.ProductTaxRate = (rakutenOrderItem.TaxRate * 100);
			this.ProductPriceTax = TaxCalculationUtility.GetTaxPrice(
				rakutenOrderItem.Price,
				this.ProductTaxRate,
				Constants.TAX_ROUNDTYPE,
				(rakutenOrderItem.IncludeTaxFlag.ToString() == Constants.RAKUTEN_API_INCLUDE_TAX_FLG_ON));
			this.ProductPrice = TaxCalculationUtility.GetPrescribedPrice(
				rakutenOrderItem.Price,
				this.ProductPriceTax,
				(rakutenOrderItem.IncludeTaxFlag.ToString() == Constants.RAKUTEN_API_INCLUDE_TAX_FLG_ON));
			this.ProductPricePretax = rakutenOrderItem.PriceTaxIncl;
			this.ItemQuantity = rakutenOrderItem.Units;
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
