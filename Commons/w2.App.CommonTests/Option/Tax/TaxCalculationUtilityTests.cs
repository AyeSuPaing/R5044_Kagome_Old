using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using w2.App.Common;
using w2.App.Common.Option;
using w2.App.CommonTests._Helper;
using w2.CommonTests._Helper;

namespace w2.App.CommonTests.Option.Tax
{
	/// <summary>
	/// TaxCalculationUtilityのテスト
	/// </summary>
	[TestClass()]
	[Ignore]
	public class TaxCalculationUtilityTests : AppTestClassBase
	{
		/// <summary>
		/// 消費税額計算テスト：国外配送時の免税判定
		/// ・引数「shippingCountryCode」とConstants.OPERATIONAL_BASE_ISO_CODEから海外配送判定を行い、海外配送の場合免税となること。
		///  ・「shippingCountryCode」がConstants.OPERATIONAL_BASE_ISO_CODEの国コードと同一な場合 → 計算された消費税額が返却される。
		///  ・「shippingCountryCode」がConstants.OPERATIONAL_BASE_ISO_CODEの国コードと異なる場合 → 0が返却される
		/// システム設定値は以下とする
		///  ・GLOBAL_TRANSACTION_INCLUDED_TAX_FLAG：TRUE
		///  ・OPERATIONAL_BASE_ISO_CODE：JP
		///  ・MANAGEMENT_INCLUDED_TAX_FLAG：FALSE
		/// </summary>
		[DataTestMethod()]
		[DynamicData("m_testCaseGetTaxPriceTest_CrossBorderJudge")]
		public void GetTaxPriceTest_CrossBorderJudge(
			string shippingCountryCode,
			string taxFractionRoundingMethod,
			decimal itemPrice,
			decimal taxRate,
			decimal taxPrice)
		{
			using (new TestConfigurator(Member.Of(() => Constants.GLOBAL_TRANSACTION_INCLUDED_TAX_FLAG), false))
			using (new TestConfigurator(Member.Of(() => Constants.OPERATIONAL_BASE_ISO_CODE), Constants.COUNTRY_ISO_CODE_JP))
			using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), false))
			{
				var result = TaxCalculationUtility.GetTaxPrice(
					itemPrice,
					taxRate,
					shippingCountryCode,
					"",
					taxFractionRoundingMethod);

				// 消費税額計算テスト：国外配送時の免税判定
				// ・引数「shippingCountryCode」とConstants.OPERATIONAL_BASE_ISO_CODEから海外配送判定を行い、海外配送の場合免税となること。
				result.Should().Be(taxPrice, "消費税額計算：越境配送判定");
			}
		}

		public static object[] m_testCaseGetTaxPriceTest_CrossBorderJudge = new[]
		{
			new object[] { Constants.COUNTRY_ISO_CODE_JP, TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN, 1000m, 10m, 100m },
			new object[] { Constants.COUNTRY_ISO_CODE_US, TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN, 1000m, 10m, 0m }
		};

		/// <summary>
		/// 消費税額計算テスト：元価格が税込みか税抜か
		///  ・元注文が税抜価格の場合 → 「taxPrice = price * taxRate / 100」で消費税額が計算されること
		///  ・元注文が税込価格の場合 → 「taxPrice = price - (price / (1 + taxRate / 100m))」で消費税額が計算されること
		/// システム設定値は以下とする
		///  ・MANAGEMENT_INCLUDED_TAX_FLAG：FALSE
		/// </summary>
		[DataTestMethod()]
		[DynamicData("m_testCaseGetTaxPriceTest_PriceIncludedTaxMethodDesignation")]
		public void GetTaxPriceTest_PriceIncludedTaxMethodDesignation(
			string taxFractionRoundingMethod,
			decimal itemPrice,
			decimal taxRate,
			bool? priceTaxIncludedFlg,
			decimal taxPrice)
		{
			using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), false))
			{
				var result = TaxCalculationUtility.GetTaxPrice(
					itemPrice,
					taxRate,
					taxFractionRoundingMethod,
					priceTaxIncludedFlg);

				// 消費税額計算テスト：元価格が税込みか税抜か
				result.Should().Be(taxPrice, "消費税額計算：価格「税込/税別」指定");
			}
		}

		public static object[] m_testCaseGetTaxPriceTest_PriceIncludedTaxMethodDesignation = new[]
		{
			new object[] { TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN, 1000m, 10m, true, 90m },
			new object[] { TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN, 1000m, 10m, false, 100m },
			new object[] { TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN, 1000m, 10m, null, 100m },
		};

		/// <summary>
		/// 消費税額端数丸め処理テスト
		///  ・消費税の絶対値に対して、引数「taxFractionRoundingMethod」で指定した形式で「切り上げ」「切捨て」「四捨五入」の小数点以下丸め処理を行う
		///  ・「taxFractionRoundingMethod」にNULLが指定された場合 → Constants.TAX_EXCLUDED_FRACTION_ROUNDINGに指定された形式で丸め処理が行われること
		/// システム設定値は以下とする
		///  ・TAX_EXCLUDED_FRACTION_ROUNDING：ROUND_DOWN
		/// </summary>
		[DataTestMethod()]
		[DynamicData("m_testCaseRoundTaxFractionTest")]
		public void RoundTaxFractionTest(
			decimal taxPrice,
			string taxFractionRoundingMethod,
			decimal roundedTaxPrice)
		{
			using (new TestConfigurator(Member.Of(() => Constants.TAX_EXCLUDED_FRACTION_ROUNDING), TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN))
			{
				var result = TaxCalculationUtility.RoundTaxFraction(taxPrice, taxFractionRoundingMethod);

				// 消費税額端数丸め処理テスト
				// ・消費税の絶対値に対して、引数「taxFractionRoundingMethod」で指定した形式で「切り上げ」「切捨て」「四捨五入」の小数点以下丸め処理を行う
				result.Should().Be(roundedTaxPrice, "消費税端数丸め処理");
			}
		}

		public static object[] m_testCaseRoundTaxFractionTest = new[]
		{
			new object[] { 10.4m, TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_UP, 11m },
			new object[] { 10.4m, TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_OFF, 10m },
			new object[] { 10.5m, TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_OFF, 11m },
			new object[] { 10.5m, TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN, 10m },
			new object[] { 10.5m, null, 10m },
			new object[] { -10.4m, TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_UP, -11m },
			new object[] { -10.4m, TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_OFF, -10m },
			new object[] { -10.5m, TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_OFF, -11m },
			new object[] { -10.5m, TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN, -10m },
			new object[] { -10.5m, null, -10m },
		};

		/// <summary>
		/// 税込価格取得処理テスト
		/// ・引数「isTaxIncluded」によって、引数「itemPrice」と引数「taxPrice」を用いて税込み価格が返却されること
		///  ・「isTaxIncluded」がtrueだった場合 → 「itemPrice」が返されること
		///  ・「isTaxIncluded」がfalseだった場合 → 「itemPrice + taxPrice」が返されること
		///  ・「isTaxIncluded」がnullだった場合 → Constants.MANAGEMENT_INCLUDED_TAX_FLAGがフラグとして用いられること
		/// システム設定値は以下とする
		///  ・MANAGEMENT_INCLUDED_TAX_FLAG：TRUE
		/// </summary>
		[DataTestMethod()]
		[DynamicData("m_testCaseGetPriceTaxIncludedTest")]
		public void GetPriceTaxIncludedTest(decimal itemPrice,
			decimal taxPrice,
			bool? isTaxIncluded,
			decimal priceIncludedTax)
		{
			var result = TaxCalculationUtility.GetPriceTaxIncluded(itemPrice, taxPrice, isTaxIncluded);

			// 税込価格取得処理テスト
			// ・引数「isTaxIncluded」によって、引数「itemPrice」と引数「taxPrice」を用いて税込み価格が返却されること
			result.Should().Be(priceIncludedTax, "税込価格計算");
		}

		public static object[] m_testCaseGetPriceTaxIncludedTest = new[]
		{
			new object[] { 100m, 10m, true, 100m },
			new object[] { 100m, 10m, false, 110m },
			new object[] { 100m, 10m, null, 100m },
		};

		/// <summary>
		/// 税抜価格取得処理テスト
		/// ・引数「isTaxIncluded」によって、引数「itemPrice」と引数「taxPrice」を用いて税抜価格が返却されること
		///  ・「isTaxIncluded」がtrueだった場合 → 「itemPrice - taxPrice」が返されること
		///  ・「isTaxIncluded」がfalseだった場合 → 「itemPrice」が返されること
		///  ・「isTaxIncluded」がnullだった場合 → Constants.MANAGEMENT_INCLUDED_TAX_FLAGがフラグとして用いられること
		/// システム設定値は以下とする
		///  ・MANAGEMENT_INCLUDED_TAX_FLAG：TRUE
		/// </summary>
		[DataTestMethod()]
		[DynamicData("m_testCaseGetPriceTaxExcludedTest")]
		public void GetPriceTaxExcludedTest(
			decimal itemPrice,
			decimal taxPrice,
			bool? isTaxIncluded,
			decimal priceIncludedTax)
		{
			var result = TaxCalculationUtility.GetPriceTaxExcluded(itemPrice, taxPrice, isTaxIncluded);

			// 税抜価格取得処理テスト
			// ・引数「isTaxIncluded」によって、引数「itemPrice」と引数「taxPrice」を用いて税抜価格が返却されること
			result.Should().Be(priceIncludedTax, "税抜価格計算");
		}

		public static object[] m_testCaseGetPriceTaxExcludedTest = new[]
		{
			new object[] { 100m, 10m, true, 90m },
			new object[] { 100m, 10m, false, 100m },
			new object[] { 100m, 10m, null, 90m },
		};

		/// <summary>
		/// 「税込/税抜」システム設定の価格取得処理テスト
		/// ・引数「isTaxIncluded」によって、引数「itemPrice」と引数「taxPrice」を用いて、Constants.MANAGEMENT_INCLUDED_TAX_FLAGで指定された形式の価格が返却されること
		///  ・「isTaxIncluded」がtrue、Constants.MANAGEMENT_INCLUDED_TAX_FLAGがtrueだった場合 → 「itemPrice」が返されること
		///  ・「isTaxIncluded」がtrue、Constants.MANAGEMENT_INCLUDED_TAX_FLAGがfalseだった場合 → 「itemPrice - taxPrice」が返されること
		///  ・「isTaxIncluded」がfalse、Constants.MANAGEMENT_INCLUDED_TAX_FLAGがtrueだった場合 → 「itemPrice + taxPrice」が返されること
		///  ・「isTaxIncluded」がfalse、Constants.MANAGEMENT_INCLUDED_TAX_FLAGがfalseだった場合 → 「itemPrice」が返されること
		/// </summary>
		[DataTestMethod()]
		[DynamicData("m_testCaseGetPrescribedPriceTest")]
		public void GetPrescribedPriceTest(
			decimal itemPrice,
			decimal taxPrice,
			bool isTaxIncluded,
			bool managementTaxIncludedFlg,
			decimal prescribedPrice)
		{
			using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), managementTaxIncludedFlg))
			{
				var result = TaxCalculationUtility.GetPrescribedPrice(itemPrice, taxPrice, isTaxIncluded);

				// 「税込/税抜」システム設定の価格取得処理テスト
				// ・引数「isTaxIncluded」によって、引数「itemPrice」と引数「taxPrice」を用いて、Constants.MANAGEMENT_INCLUDED_TAX_FLAGで指定された形式の価格が返却されること
				result.Should().Be(prescribedPrice, "「税込/税抜」システム設定の価格計算");
			}
		}

		public static object[] m_testCaseGetPrescribedPriceTest = new[]
		{
			new object[] { 100m, 10m, true, true, 100m },
			new object[] { 100m, 10m, true, false, 90m },
			new object[] { 100m, 10m, false, true, 110m },
			new object[] { 100m, 10m, false, false, 100m },
		};
	}
}
