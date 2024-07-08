/*
=========================================================================================================
  Module      : 定期商品価格計算(FixedPurchaseProductCalculator.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Globalization;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.Product;
using w2.Domain.ProductSale;

namespace w2.Domain.FixedPurchaseForecast.Helper
{
	/// <summary>
	/// 定期商品価格計算
	/// </summary>
	internal class FixedPurchaseProductCalculator
	{
		/// <summary>税金額は数処理：切り上げ</summary>
		private const string FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_UP = "ROUNDUP";
		/// <summary>税金額は数処理：切り捨て</summary>
		private const string FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN = "ROUNDDOWN";
		/// <summary>税金額は数処理：四捨五入</summary>
		private const string FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_OFF = "ROUNDOFF";

		/// <summary>
		/// 商品の税込み販売価格を取得
		/// </summary>
		/// <param name="product">商品</param>
		/// <param name="strProductSaleId">商品セールID</param>
		/// <param name="productCount">商品数</param>
		/// <param name="taxExcludedFractionRounding">税処理金額端数処理方法</param>
		/// <param name="managementIncludedTaxFlag">税込み管理フラグ</param>
		/// <param name="currencyLocaleId">通貨ロケールID</param>
		/// <param name="currencyDecimalDigits">通貨の小数点以下の有効桁数</param>
		/// <param name="accessor">SQLアクセサ</param>
		internal FixedPurchaseProductCalculator(
			ProductModel product,
			string strProductSaleId,
			int productCount,
			string taxExcludedFractionRounding,
			bool managementIncludedTaxFlag,
			string currencyLocaleId,
			int? currencyDecimalDigits,
			SqlAccessor accessor = null)
		{
			this.ProductSaleId = strProductSaleId;
			this.Count = productCount;
			TaxExcludedFractionRounding = taxExcludedFractionRounding;
			ManagementIncludedTaxFlag = managementIncludedTaxFlag;
			CurrencyLocaleId = currencyLocaleId;
			CurrencyDecimalDigits = currencyDecimalDigits;

			SetPrice(product, accessor);
		}

		/// <summary>
		/// 商品価格更新
		/// </summary>
		/// <param name="product">商品情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		private void SetPrice(ProductModel product, SqlAccessor accessor = null)
		{
			this.PriceOrg = product.Price;
			if (product.SpecialPrice != null)
			{
				this.Price = (decimal)product.SpecialPrice;
			}
			else
			{
				this.Price = product.Price;
			}

			if (product.MemberRankPrice != null)
			{
				this.PriceMemberRank = product.MemberRankPrice;
				this.Price = this.PriceMemberRank.Value;
			}
			else if (string.IsNullOrEmpty(this.ProductSaleId) == false)
			{
				var dProductSalePrice = new ProductSaleService().GetProductSalePrice(product.ShopId, this.ProductSaleId, product.ProductId, product.VariationId, accessor);
				if (dProductSalePrice != null)
				{
					this.Price = dProductSalePrice.Value;
				}
			}

			if (product.VariationFixedPurchasePrice != null)
			{
				this.Price = (decimal)product.VariationFixedPurchasePrice;
			}

			this.PriceTax = GetTaxPrice(this.Price, this.TaxRate);
			this.PricePretax = GetPriceTaxIncluded(this.Price, this.PriceTax);
		}

		/// <summary>
		/// 税金額取得(端数処理済み)
		/// </summary>
		/// <param name="price">税込金額</param>
		/// <param name="taxRate">税率</param>
		/// <returns>税金額</returns>
		private static decimal GetTaxPrice(decimal price, decimal taxRate)
		{
			var taxPrice = ManagementIncludedTaxFlag
				? price - (price / (1 + taxRate / 100m))
				: price * (taxRate / 100m);
			taxPrice = RoundTaxFraction(taxPrice, TaxExcludedFractionRounding);
			return taxPrice;
		}

		/// <summary>
		/// 税額端数処理
		/// </summary>
		/// <param name="taxPrice">税金額</param>
		/// <param name="taxExcludedFractionRounding">税額丸め方法</param>
		/// <returns>税金額</returns>
		private static decimal RoundTaxFraction(decimal taxPrice, string taxExcludedFractionRounding)
		{
			var taxPriceAbs = Math.Abs(taxPrice);
			var roundedTaxPriceAbs = 0m;
			switch (taxExcludedFractionRounding)
			{
				case FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_UP:
					roundedTaxPriceAbs = ConvertPriceByKeyCurrency(taxPriceAbs, DecimalUtility.Format.RoundUp).Value;
					break;

				case FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN:
					roundedTaxPriceAbs = ConvertPriceByKeyCurrency(taxPriceAbs, DecimalUtility.Format.RoundUp).Value;
					break;

				case FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_OFF:
					roundedTaxPriceAbs = ConvertPriceByKeyCurrency(taxPriceAbs, DecimalUtility.Format.RoundUp).Value;
					break;

				default:
					roundedTaxPriceAbs = taxPriceAbs;
					break;
			}

			return (taxPrice > 0) ? roundedTaxPriceAbs : (roundedTaxPriceAbs * -1);
		}

		/// <summary>
		/// 基軸通貨価格取得 Decimal型
		/// </summary>
		/// <param name="price">基軸通貨価格</param>
		/// <param name="format">Round→四捨五入、RoundDown→切り捨て、RoundUp→切り上げ</param>
		/// <returns>基軸通貨価格</returns>
		private static decimal? ConvertPriceByKeyCurrency(Object price, DecimalUtility.Format format = DecimalUtility.Format.RoundDown)
		{
			var priceText = StringUtility.ToEmpty(price).Replace(
				CultureInfo.CreateSpecificCulture(CurrencyLocaleId).NumberFormat.CurrencyDecimalSeparator,
				".");

			decimal dec;
			if (decimal.TryParse(priceText, out dec) == false) return null;

			var digitsByKeyCurrency = CurrencyDecimalDigits
				?? CultureInfo.CreateSpecificCulture(CurrencyLocaleId).NumberFormat.CurrencyDecimalDigits;
			dec = DecimalUtility.DecimalRound(dec, format, digitsByKeyCurrency);
			return dec;
		}

		/// <summary>
		/// 商品金額小計と税額から、税込み金額を取得
		/// </summary>
		/// <param name="itemPrice">商品金額小計</param>
		/// <param name="itemPriceTax">税額</param>
		/// <returns>税込み金額</returns>
		private static decimal GetPriceTaxIncluded(decimal itemPrice, decimal itemPriceTax)
		{
			var priceTaxIncluded = ManagementIncludedTaxFlag
				? itemPrice
				: itemPrice + itemPriceTax;
			return priceTaxIncluded;
		}

		/// <summary>価格</summary>
		private decimal Price { get; set; }
		/// <summary>価格（値引き前）</summary>
		private decimal PriceOrg { get; set; }
		/// <summary>会員ランク価格</summary>
		private decimal? PriceMemberRank { get; set; }
		/// <summary>商品セールID</summary>
		private string ProductSaleId { get; set; }
		/// <summary>商品税額</summary>
		private decimal PriceTax { get; set; }
		/// <summary>税率</summary>
		private decimal TaxRate { get; set; }
		/// <summary>商品数</summary>
		internal int Count { get; private set; }
		/// <summary>税込販売価格</summary>
		internal decimal PricePretax { get; private set; }
		/// <summary>税処理金額端数処理方法</summary>
		private static string TaxExcludedFractionRounding { get; set; }
		/// <summary>税込み管理フラグ</summary>
		private static bool ManagementIncludedTaxFlag { get; set; }
		/// <summary>通貨ロケールID</summary>
		private static string CurrencyLocaleId { get; set; }
		/// <summary>補助単位 小数点以下の有効桁数</summary>
		private static int? CurrencyDecimalDigits { get; set; }
	}
}
