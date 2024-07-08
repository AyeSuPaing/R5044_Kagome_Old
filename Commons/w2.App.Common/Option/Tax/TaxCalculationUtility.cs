/*
=========================================================================================================
  Module      : 税計算共通処理クラス(TaxCalculationUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Global;
using w2.App.Common.Web.Page;
using w2.Common.Util;

namespace w2.App.Common.Option
{
	/// <summary>
	/// 税計算ユーティリティ
	/// </summary>
	public class TaxCalculationUtility
	{
		/// <summary>キー：課税対象価格</summary>
		public const string HASH_KEY_TAXABLE_ITEM_PRICE = "taxableItemPrice";
		/// <summary>税金額は数処理：切り上げ</summary>
		public const string FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_UP = "ROUNDUP";
		/// <summary>税金額は数処理：切り捨て</summary>
		public const string FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN = "ROUNDDOWN";
		/// <summary>税金額は数処理：四捨五入</summary>
		public const string FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_OFF = "ROUNDOFF";

		/// <summary>
		/// 税金額取得(端数処理済み・商品用：国内国外判定有り)
		/// </summary>
		/// <param name="price">金額</param>
		/// <param name="taxRate">税率</param>
		/// <param name="shippingCountryCode">国コード</param>
		/// <param name="shippingProvince">州</param>
		/// <param name="taxFractionRoundingMethod">税率丸め方法</param>
		/// <param name="priceTaxIncludedFlg">税込み金額フラグ</param>
		/// <returns>税金額</returns>
		public static decimal GetTaxPrice(
			decimal price,
			decimal taxRate,
			string shippingCountryCode,
			string shippingProvince,
			string taxFractionRoundingMethod,
			bool? priceTaxIncludedFlg = null)
		{
			var tax = CheckShippingPlace(shippingCountryCode, shippingProvince)
				? GetTaxPrice(price, taxRate, taxFractionRoundingMethod, priceTaxIncludedFlg)
				: 0m;
			return tax;
		}
		/// <summary>
		/// 税金額取得(端数処理済み)
		/// </summary>
		/// <param name="price">税込金額</param>
		/// <param name="taxRate">税率</param>
		/// <param name="taxFractionRoundingMethod">税率丸め方法</param>
		/// <param name="priceTaxIncludedFlag">税込み金額フラグ</param>
		/// <returns>税金額</returns>
		public static decimal GetTaxPrice(decimal price, decimal taxRate, string taxFractionRoundingMethod, bool? priceTaxIncludedFlag = null)
		{
			var taxPrice = (priceTaxIncludedFlag ?? Constants.MANAGEMENT_INCLUDED_TAX_FLAG)
				? price - (price / (1 + taxRate / 100m))
				: price * (taxRate / 100m);
			taxPrice = RoundTaxFraction(taxPrice, taxFractionRoundingMethod);
			return taxPrice;
		}

		/// <summary>
		/// 税額端数処理
		/// </summary>
		/// <param name="taxPrice">税金額</param>
		/// <param name="taxFractionRoundingMethod">税額丸め方法</param>
		/// <returns>税金額</returns>
		public static decimal RoundTaxFraction(decimal taxPrice, string taxFractionRoundingMethod = null)
		{
			var taxPriceAbs = Math.Abs(taxPrice);
			var roundedTaxPriceAbs = 0m;
			switch (taxFractionRoundingMethod ?? Constants.TAX_EXCLUDED_FRACTION_ROUNDING)
			{
				// 端数切り上げ
				case FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_UP:
					roundedTaxPriceAbs = taxPriceAbs.ToPriceDecimal(DecimalUtility.Format.RoundUp).Value;
					break;

				// 端数切り捨て
				case FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN:
					roundedTaxPriceAbs = taxPriceAbs.ToPriceDecimal(DecimalUtility.Format.RoundDown).Value;
					break;

				// 端数四捨五入
				case FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_OFF:
					roundedTaxPriceAbs = taxPriceAbs.ToPriceDecimal(DecimalUtility.Format.Round).Value;
					break;

				default:
					roundedTaxPriceAbs = taxPriceAbs;
					break;
			}

			return (taxPrice > 0) ? roundedTaxPriceAbs : (roundedTaxPriceAbs * -1);
		}

		/// <summary>
		/// 配送先の国・州から税額取得有無を判定
		/// </summary>
		/// <param name="shippingCountryCode">国コード</param>
		/// <param name="shippingProvince">州</param>
		/// <returns>true:税額取得 false:税額取得無し</returns>
		public static bool CheckShippingPlace(string shippingCountryCode, string shippingProvince)
		{
			if (string.IsNullOrEmpty(shippingCountryCode)) return true;
			if (Constants.GLOBAL_TRANSACTION_INCLUDED_TAX_FLAG) return true;
			if ((shippingCountryCode != Constants.OPERATIONAL_BASE_ISO_CODE)
				|| ((shippingCountryCode == Constants.OPERATIONAL_BASE_ISO_CODE)
					&& GlobalAddressUtil.IsCountryUs(shippingCountryCode)
					&& ((shippingProvince ?? string.Empty) != Constants.OPERATIONAL_BASE_PROVINCE))) return false;

			return true;
		}

		/// <summary>
		/// 商品金額小計と税額から、税込み金額を取得
		/// </summary>
		/// <param name="itemPrice">商品金額小計</param>
		/// <param name="itemPriceTax">税額</param>
		/// <param name="isTaxIncluded">税込みフラグ(true：税込 false：システム管理の税区分)</param>
		/// <returns>税込み金額</returns>
		public static decimal GetPriceTaxIncluded(decimal itemPrice, decimal itemPriceTax, bool? isTaxIncluded = null)
		{
			var isPriceTaxIncluded = isTaxIncluded ?? Constants.MANAGEMENT_INCLUDED_TAX_FLAG;
			var priceTaxIncluded = isPriceTaxIncluded
				? itemPrice
				: itemPrice + itemPriceTax;
			return priceTaxIncluded;
		}

		/// <summary>
		/// 商品金額小計と税額から、税抜き金額を取得
		/// </summary>
		/// <param name="itemPrice">商品金額小計</param>
		/// <param name="itemPriceTax">税額</param>
		/// <param name="isTaxIncluded">税込みフラグ(true：税込 false：システム管理の税区分)</param>
		/// <returns>税抜き金額</returns>
		public static decimal GetPriceTaxExcluded(decimal itemPrice, decimal itemPriceTax, bool? isTaxIncluded = null)
		{
			var isPriceTaxIncluded = isTaxIncluded ?? Constants.MANAGEMENT_INCLUDED_TAX_FLAG;
			var priceTaxExcluded = isPriceTaxIncluded
				? itemPrice - itemPriceTax
				: itemPrice;
			return priceTaxExcluded;
		}

		/// <summary>
		/// 外部システムの税込み/税別金額と税額から、システム規定の形式(税別/税込み)の金額を取得
		/// </summary>
		/// <param name="price">税処理前金額</param>
		/// <param name="taxPrice">税額</param>
		/// <param name="taxIncludedFlag">税込みフラグ</param>
		/// <returns>システム規定の形式(税別/税込み)の金額</returns>
		public static decimal GetPrescribedPrice(decimal price, decimal taxPrice, bool taxIncludedFlag)
		{
			var prescribedPrice = taxIncludedFlag
				? Constants.MANAGEMENT_INCLUDED_TAX_FLAG
					? price
					: price - taxPrice
				: Constants.MANAGEMENT_INCLUDED_TAX_FLAG
					? price + taxPrice
					: price;
			return prescribedPrice;
		}

		/// <summary>
		/// システム既定の商品税区分を取得
		/// </summary>
		/// <returns>既定の商品税区分</returns>
		public static string GetPrescribedProductTaxIncludedFlag()
		{
			return Constants.MANAGEMENT_INCLUDED_TAX_FLAG ? Constants.FLG_PRODUCT_TAX_INCLUDED_PRETAX : Constants.FLG_PRODUCT_TAX_EXCLUDE_PRETAX;
		}
		
		/// <summary>
		/// システム既定の注文税区分を取得
		/// </summary>
		/// <returns>既定の注文税区分</returns>
		public static string GetPrescribedOrderTaxIncludedFlag()
		{
			return Constants.MANAGEMENT_INCLUDED_TAX_FLAG ? Constants.FLG_ORDER_ORDER_TAX_INCLUDED_PRETAX : Constants.FLG_ORDER_ORDER_TAX_EXCLUDE_PRETAX;
		}
		
		/// <summary>
		/// システム既定の注文商品税区分を取得
		/// </summary>
		/// <returns>既定の注文商品税区分</returns>
		public static string GetPrescribedOrderItemTaxIncludedFlag()
		{
			return Constants.MANAGEMENT_INCLUDED_TAX_FLAG ? Constants.FLG_ORDERITEM_PRODUCT_TAX_INCLUDED_FLG_ON : Constants.FLG_ORDERITEM_PRODUCT_TAX_INCLUDED_FLG_OFF;
		}

		/// <summary>
		/// 税区分表示文言を取得
		/// </summary>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>税区分表示文言</returns>
		public static string GetTaxTypeText(string languageLocaleId = null)
		{
			var tag = Constants.MANAGEMENT_INCLUDED_TAX_FLAG
				? "@@DispText.tax_type.included@@"
				: "@@DispText.tax_type.excluded@@";
			var taxTypeText = (languageLocaleId != null)
				? CommonPage.ReplaceTagByLocaleId(tag, languageLocaleId)
				: CommonPage.ReplaceTag(tag);
			return taxTypeText;
		}

		/// <summary>
		/// 表示用の税率を取得
		/// </summary>
		/// <param name="taxRate">税率</param>
		/// <returns>税区分表示文言</returns>
		public static string GetTaxRateForDIsplay(object taxRate)
		{
			var taxRateText = StringUtility.ToEmpty(taxRate);

			decimal taxRateValue;
			if (decimal.TryParse(taxRateText, out taxRateValue) == false) return "";

			var rateForDisplay = Constants.GLOBAL_OPTION_ENABLE
				? taxRateValue.ToString()
				: decimal.ToInt32(taxRateValue).ToString();

			return rateForDisplay;
		}

		/// <summary>
		/// 税処理後の金額取得
		/// </summary>
		/// <param name="taxIncludedPrice">税込金額</param>
		/// <param name="taxExcludedFractionRounding">税率丸め方法</param>
		/// <param name="taxRate">税率</param>
		/// <returns>税込金額または税抜金額</returns>
		public static decimal GetTaxExcludedPrice(
			decimal taxIncludedPrice,
			string taxExcludedFractionRounding,
			decimal taxRate)
		{
			switch (taxExcludedFractionRounding)
			{
				// 端数切り上げ
				case FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_UP:
					return Math.Ceiling(taxIncludedPrice / (1 + taxRate / 100m));

				// 端数切り捨て
				case FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN:
					return Math.Floor(taxIncludedPrice / (1 + taxRate / 100m));

				// 端数四捨五入
				case FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_OFF:
					return Math.Floor(taxIncludedPrice / (1 + taxRate / 100m) + 0.5m);

				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
