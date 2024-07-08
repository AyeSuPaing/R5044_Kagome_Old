using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using w2.App.CommonTests._Helper;
using w2.App.CommonTests.Order.Cart;
using w2.CommonTests._Helper;
using w2.Domain;
using w2.Domain.ProductTag;

namespace w2.App.Common.Order.Tests
{
	[TestClass()]
	[Ignore]
	public class CartShippingTests : AppTestClassBase
	{
		/// <summary>
		/// ユーザー配送先作成テスト（Global OFF）
		/// </summary>
		[DataTestMethod()]
		public void CreateUserShippingTest_GlobalOptionOff()
		{
			var cart = CartTestHelper.CreateCart();
			cart.Owner = CartTestHelper.CreateCartOwner();
			var cartShipping = new CartShipping(cart);
			cartShipping.UpdateShippingAddr(cart.Owner, blIsSameShippingAsCart1: true);

			var userShipping = cartShipping.CreateUserShipping(userId: "U001");
			userShipping.ShippingName.Should().Be(cart.Owner.Name);
			userShipping.ShippingName1.Should().Be(cart.Owner.Name1);
			userShipping.ShippingName2.Should().Be(cart.Owner.Name2);
			userShipping.ShippingNameKana.Should().Be(cart.Owner.NameKana);
			userShipping.ShippingNameKana1.Should().Be(cart.Owner.NameKana1);
			userShipping.ShippingNameKana2.Should().Be(cart.Owner.NameKana2);
			userShipping.ShippingZip.Should().Be(cart.Owner.Zip);
			userShipping.ShippingZip1.Should().Be(cart.Owner.Zip1);
			userShipping.ShippingZip2.Should().Be(cart.Owner.Zip2);
			userShipping.ShippingAddr1.Should().Be(cart.Owner.Addr1);
			userShipping.ShippingAddr2.Should().Be(cart.Owner.Addr2);
			userShipping.ShippingAddr3.Should().Be(cart.Owner.Addr3);
			userShipping.ShippingAddr4.Should().Be(cart.Owner.Addr4);
			userShipping.ShippingAddr5.Should().Be(cart.Owner.Addr5);
			userShipping.ShippingCountryName.Should().Be(cart.Owner.AddrCountryName);
			userShipping.ShippingCompanyName.Should().Be(cart.Owner.CompanyName);
			userShipping.ShippingCompanyPostName.Should().Be(cart.Owner.CompanyPostName);
			userShipping.ShippingTel1.Should().Be(cart.Owner.Tel1);
			userShipping.ShippingTel1_1.Should().Be(cart.Owner.Tel1_1);
			userShipping.ShippingTel1_2.Should().Be(cart.Owner.Tel1_2);
			userShipping.ShippingTel1_3.Should().Be(cart.Owner.Tel1_3);
		}

		/// <summary>
		/// 配送料金特殊の計算テスト
		/// ・テストケースごとに記載
		/// </summary>
		[DataTestMethod()]
		[DynamicData("m_tdSetSpecialShippingPriceTest")]
		public void SetSpecialShippingPriceTest(
			dynamic config,
			dynamic data,
			decimal expected,
			string msg)
		{
			// モックによるドメイン層偽装
			var mock = new Mock<IProductTagService>();
			mock.Setup(s => s.GetProductTagSetting()).Returns(new ProductTagSettingModel[0]);
			DomainFacade.Instance.ProductTagService = mock.Object;

			using (new TestConfigurator(Member.Of(() => Constants.SHIPPINGPRICE_SEPARATE_ESTIMATE_ENABLED), config.ShippingpriceSeparateEstimateEnabled))
			{
				var result = (decimal)CartShipping.SetSpecialShippingPrice(
					data.PreferableShippingPrice,
					data.ShippingPriceTotal,
					data.ShippingFreePriceFlg,
					data.ShippingFreePrice,
					data.ShippingZoneNo,
					data.PriceSubtotal,
					data.CalculationPluralKbn,
					data.PluralShippingPrice,
					data.ProductCount,
					data.ShippingPriceSeparateEstimateFlg,
					data.HasFixedPurchase,
					data.FixedPurchaseFreeShippingFlg,
					data.SetpromotionProductDiscountAmount,
					data.MemberRankDiscountPrice,
					data.FixedPurchaseMemberDiscountAmount,
					data.FixedPurchaseDiscountPrice,
					data.UseCouponPrice,
					string.Empty,
					null,
					null,
					data.UseGlobalShippingPrice);
				result.Should().Be(expected, string.Format("配送料金特殊：{0}", msg));
			}
		}

		public static object[] m_tdSetSpecialShippingPriceTest = new[]
		{
			// 配送料無料設定パターン（特別配送先ではない、海外配送料不使用、小計から各種割引を減算して判定）
			// ・配送料金特殊 = 0 となること
			new object[]
			{
				new
				{
					ShippingpriceSeparateEstimateEnabled = false
				},
				new
				{
					PreferableShippingPrice = 0m,
					ShippingPriceTotal = 11m,
					ShippingFreePriceFlg = Constants.FLG_SHOPSHIPPING_SHIPPING_FREE_PRICE_FLG_VALID,
					ShippingFreePrice = 100m,
					ShippingZoneNo = 1,	
					PriceSubtotal = 10000m,
					CalculationPluralKbn = string.Empty,
					PluralShippingPrice = 0m,
					ProductCount = 1,
					ShippingPriceSeparateEstimateFlg = false,
					HasFixedPurchase = false,
					FixedPurchaseFreeShippingFlg = Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_FREE_SHIPPING_FLG_INVALID,
					SetpromotionProductDiscountAmount = 10m,
					MemberRankDiscountPrice = 0m,
					FixedPurchaseMemberDiscountAmount = 0m,
					FixedPurchaseDiscountPrice = 0m,
					UseCouponPrice = 0m,
					UseGlobalShippingPrice = false,
				},
				0m,
				"配送料無料設定パターン（特別配送先ではない、海外配送料不使用、小計から各種割引を減算して判定）"
			},
			// 配送料無料設定パターン（特別配送先ではない、海外配送料使用、小計から各種割引を減算して判定）
			// ・配送料金特殊 = 0 となること
			new object[]
			{
				new
				{
					ShippingpriceSeparateEstimateEnabled = false
				},
				new
				{
					PreferableShippingPrice = 0m,
					ShippingPriceTotal = 11m,
					ShippingFreePriceFlg = Constants.FLG_SHOPSHIPPING_SHIPPING_FREE_PRICE_FLG_VALID,
					ShippingFreePrice = 100m,
					ShippingZoneNo = 47,
					PriceSubtotal = 10000m,
					CalculationPluralKbn = string.Empty,
					PluralShippingPrice = 0m,
					ProductCount = 1,
					ShippingPriceSeparateEstimateFlg = false,
					HasFixedPurchase = false,
					FixedPurchaseFreeShippingFlg = Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_FREE_SHIPPING_FLG_INVALID,
					SetpromotionProductDiscountAmount = 10m,
					MemberRankDiscountPrice = 0m,
					FixedPurchaseMemberDiscountAmount = 0m,
					FixedPurchaseDiscountPrice = 0m,
					UseCouponPrice = 0m,
					UseGlobalShippingPrice = true,
				},
				0m,
				"配送料無料設定パターン（特別配送先ではない、海外配送料使用、小計から各種割引を減算して判定）"
			},
			// 配送料無料設定パターン（特別配送先、海外配送料使用、小計から各種割引を減算して判定）
			// ・配送料金特殊 = 0 となること
			new object[]
			{
				new
				{
					ShippingpriceSeparateEstimateEnabled = false
				},
				new
				{
					PreferableShippingPrice = 0m,
					ShippingPriceTotal = 11m,
					ShippingFreePriceFlg = Constants.FLG_SHOPSHIPPING_SHIPPING_FREE_PRICE_FLG_VALID,
					ShippingFreePrice = 100m,
					ShippingZoneNo = 48,
					PriceSubtotal = 10000m,
					CalculationPluralKbn = string.Empty,
					PluralShippingPrice = 0m,
					ProductCount = 1,
					ShippingPriceSeparateEstimateFlg = false,
					HasFixedPurchase = false,
					FixedPurchaseFreeShippingFlg = Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_FREE_SHIPPING_FLG_INVALID,
					SetpromotionProductDiscountAmount = 10m,
					MemberRankDiscountPrice = 0m,
					FixedPurchaseMemberDiscountAmount = 0m,
					FixedPurchaseDiscountPrice = 0m,
					UseCouponPrice = 0m,
					UseGlobalShippingPrice = true,
				},
				0m,
				"配送料無料設定パターン（特別配送先、海外配送料使用、小計から各種割引を減算して判定）"
			},
			// 最も優先される送料１点＋x円／個の送料パターン
			// ・配送料金特殊 = 優先配送料金 + ((商品数 - 1) * 複数商品配送料) となること
			new object[]
			{
				new
				{
					ShippingpriceSeparateEstimateEnabled = false
				},
				new
				{
					PreferableShippingPrice = 100m,
					ShippingPriceTotal = 11m,
					ShippingFreePriceFlg = Constants.FLG_SHOPSHIPPING_SHIPPING_FREE_PRICE_FLG_INVALID,
					ShippingFreePrice = 100m,
					ShippingZoneNo = 48,
					PriceSubtotal = 10000m,
					CalculationPluralKbn = Constants.FLG_SHOPSHIPPING_CALCULATION_PLURAL_KBN_HIGHEST_SHIPPING_PRICE_PLUS_PLURAL_PRICE,
					PluralShippingPrice = 500m,
					ProductCount = 10,
					ShippingPriceSeparateEstimateFlg = false,
					HasFixedPurchase = false,
					FixedPurchaseFreeShippingFlg = Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_FREE_SHIPPING_FLG_INVALID,
					SetpromotionProductDiscountAmount = 10m,
					MemberRankDiscountPrice = 0m,
					FixedPurchaseMemberDiscountAmount = 0m,
					FixedPurchaseDiscountPrice = 0m,
					UseCouponPrice = 0m,
					UseGlobalShippingPrice = false,
				},
				4600m,
				"最も優先される送料１点＋x円／個の送料パターン"
			},
			// 配送料金別途見積もりパターン（配送料金別途見積もりオプション利用＆配送種別設定が配送料金別途見積もりフラグ有効）
			// ・配送料金特殊 = 0 となること
			new object[]
			{
				new
				{
					ShippingpriceSeparateEstimateEnabled = true
				},
				new
				{
					PreferableShippingPrice = 100m,
					ShippingPriceTotal = 11m,
					ShippingFreePriceFlg = Constants.FLG_SHOPSHIPPING_SHIPPING_FREE_PRICE_FLG_INVALID,
					ShippingFreePrice = 100m,
					ShippingZoneNo = 48,
					PriceSubtotal = 10000m,
					CalculationPluralKbn = Constants.FLG_SHOPSHIPPING_CALCULATION_PLURAL_KBN_HIGHEST_SHIPPING_PRICE_PLUS_PLURAL_PRICE,
					PluralShippingPrice = 500m,
					ProductCount = 10,
					ShippingPriceSeparateEstimateFlg = true,
					HasFixedPurchase = false,
					FixedPurchaseFreeShippingFlg = Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_FREE_SHIPPING_FLG_INVALID,
					SetpromotionProductDiscountAmount = 10m,
					MemberRankDiscountPrice = 0m,
					FixedPurchaseMemberDiscountAmount = 0m,
					FixedPurchaseDiscountPrice = 0m,
					UseCouponPrice = 0m,
					UseGlobalShippingPrice = false,
				},
				0m,
				"配送料金別途見積もりパターン（配送料金別途見積もりオプション利用＆配送種別設定が配送料金別途見積もりフラグ有効）"
			},
			// 定期配送料無料パターン
			// ・配送料金特殊 = 0 となること
			new object[]
			{
				new
				{
					ShippingpriceSeparateEstimateEnabled = true
				},
				new
				{
					PreferableShippingPrice = 100m,
					ShippingPriceTotal = 11m,
					ShippingFreePriceFlg = Constants.FLG_SHOPSHIPPING_SHIPPING_FREE_PRICE_FLG_INVALID,
					ShippingFreePrice = 100m,
					ShippingZoneNo = 48,
					PriceSubtotal = 10000m,
					CalculationPluralKbn = Constants.FLG_SHOPSHIPPING_CALCULATION_PLURAL_KBN_HIGHEST_SHIPPING_PRICE_PLUS_PLURAL_PRICE,
					PluralShippingPrice = 500m,
					ProductCount = 10,
					ShippingPriceSeparateEstimateFlg = true,
					HasFixedPurchase = true,
					FixedPurchaseFreeShippingFlg = Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_FREE_SHIPPING_FLG_VALID,
					SetpromotionProductDiscountAmount = 10m,
					MemberRankDiscountPrice = 0m,
					FixedPurchaseMemberDiscountAmount = 0m,
					FixedPurchaseDiscountPrice = 0m,
					UseCouponPrice = 0m,
					UseGlobalShippingPrice = false,
				},
				0m,
				"定期配送料無料パターン"
			},
			// 通常商品合計金額パターン
			// ・配送料金特殊 = 通常商品合計金額 となること
			new object[]
			{
				new
				{
					ShippingpriceSeparateEstimateEnabled = false
				},
				new
				{
					PreferableShippingPrice = 100m,
					ShippingPriceTotal = 11m,
					ShippingFreePriceFlg = Constants.FLG_SHOPSHIPPING_SHIPPING_FREE_PRICE_FLG_INVALID,
					ShippingFreePrice = 100m,
					ShippingZoneNo = 48,
					PriceSubtotal = 10000m,
					CalculationPluralKbn = Constants.FLG_SHOPSHIPPING_CALCULATION_PLURAL_KBN_SUM_OF_PRODUCT_SHIPPING_PRICE,
					PluralShippingPrice = 500m,
					ProductCount = 10,
					ShippingPriceSeparateEstimateFlg = true,
					HasFixedPurchase = false,
					FixedPurchaseFreeShippingFlg = Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_FREE_SHIPPING_FLG_VALID,
					SetpromotionProductDiscountAmount = 10m,
					MemberRankDiscountPrice = 0m,
					FixedPurchaseMemberDiscountAmount = 0m,
					FixedPurchaseDiscountPrice = 0m,
					UseCouponPrice = 0m,
					UseGlobalShippingPrice = false,
				},
				11m,
				"通常商品合計金額パターン"
			},
		};

		/// <summary>
		/// 配送先商品合計の計算テスト
		/// ・単一配送先の場合に配送先商品合計が正しく計算されること
		/// </summary>
		[DataTestMethod()]
		[DynamicData("m_tdPriceSubtotalTest_SingleShipping")]
		public void PriceSubtotalTest_SingleShipping(
			decimal productPrice,
			int productCount,
			int cartItemRepeatCount,
			decimal expected)
		{
			// モックによるドメイン層偽装
			var mock = new Mock<IProductTagService>();
			mock.Setup(s => s.GetProductTagSetting()).Returns(new ProductTagSettingModel[0]);
			DomainFacade.Instance.ProductTagService = mock.Object;

			var cart = CartTestHelper.CreateCart();
			cart.Owner = CartTestHelper.CreateCartOwner();

			foreach (var i in Enumerable.Range(0, cartItemRepeatCount))
			{
				cart.Items.Add(CartTestHelper.CreateCartProduct(productPrice, productCount));
			}
			var cartShipping = new CartShipping(cart);
			cartShipping.UpdateShippingAddr(cart.Owner, blIsSameShippingAsCart1: true);
			cart.SetShippingAddressAndShippingDateTime(cartShipping);

			var result = cartShipping.PriceSubtotal;
			result.Should().Be(expected, "配送先商品合計");
		}

		public static object[] m_tdPriceSubtotalTest_SingleShipping = new[]
		{
			new object[] { 10m, 1, 1, 10m },
			new object[] { 100m, 10, 2, 2000m },
		};

		/// <summary>
		/// 配送先商品合計の計算テスト
		/// ・複数配送先（ギフトオプション＝有効）の場合に配送先商品合計が正しく計算されること
		/// </summary>
		[DataTestMethod()]
		[DynamicData("m_tdPriceSubtotalTest_MultipleShipping")]
		public void PriceSubtotalTest_MultipleShipping(
			decimal productPrice,
			int productCount,
			int cartItemRepeatCount,
			decimal expected)
		{
			using (new TestConfigurator(Member.Of(() => Constants.GIFTORDER_OPTION_ENABLED), true))
			{
				// モックによるドメイン層偽装
				var mock = new Mock<IProductTagService>();
				mock.Setup(s => s.GetProductTagSetting()).Returns(new ProductTagSettingModel[0]);
				DomainFacade.Instance.ProductTagService = mock.Object;

				var cart = CartTestHelper.CreateCart();
				cart.Owner = CartTestHelper.CreateCartOwner();
				cart.Items.Add(CartTestHelper.CreateCartProduct(
					productPrice: productPrice,
					productCount: productCount,
					addCartKbn: Constants.AddCartKbn.GiftOrder));
				cart.Items.Add(CartTestHelper.CreateCartProduct(
					productPrice: productPrice,
					productCount: productCount,
					addCartKbn: Constants.AddCartKbn.GiftOrder));

				var cartShipping = new CartShipping(cart);
				cartShipping.UpdateShippingAddr(cart.Owner, blIsSameShippingAsCart1: true);

				foreach (var i in Enumerable.Range(0, cartItemRepeatCount))
				{
					cartShipping.ProductCounts.Add(
						new CartShipping.ProductCount(
							CartTestHelper.CreateCartProduct(
								productPrice: productPrice,
								productCount: productCount,
								addCartKbn: Constants.AddCartKbn.GiftOrder),
							productCount));
				}

				cart.Shippings.Add(cartShipping);
				cart.Shippings.Add(cartShipping);

				var result = cartShipping.PriceSubtotal;
				result.Should().Be(expected, "配送先商品合計");
			}
		}

		public static object[] m_tdPriceSubtotalTest_MultipleShipping = new[]
		{
			new object[] { 10m, 1, 1, 10m },
			new object[] { 100m, 10, 2, 2000m },
		};
	}
}
