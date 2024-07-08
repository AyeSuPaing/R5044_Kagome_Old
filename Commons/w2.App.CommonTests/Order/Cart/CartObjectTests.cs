using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Moq;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.CommonTests._Helper;
using w2.App.CommonTests._Helper.DataCacheConfigurator;
using w2.Common.Sql;
using w2.Common.Wrapper;
using w2.CommonTests._Helper;
using w2.Domain;
using w2.Domain.Coupon;
using w2.Domain.Coupon.Helper;
using w2.Domain.FixedPurchase;
using w2.Domain.MemberRank;
using w2.Domain.Payment;
using w2.Domain.Product;
using w2.Domain.ProductFixedPurchaseDiscountSetting;
using w2.Domain.ProductTag;
using w2.Domain.SetPromotion;
using w2.Domain.ShopShipping;
using w2.Domain.User;
using w2.Domain.UserDefaultOrderSetting;
using Constants = w2.App.Common.Constants;
using System.Collections;
using w2.Domain.GlobalShipping;

namespace w2.App.CommonTests.Order.Cart
{
	/// <summary>
	/// CartObjectのテスト
	/// </summary>
	[TestClass()]
	public class CartObjectTests : AppTestClassBase
	{
		/// <summary>
		/// カート商品へのセットプロモーション割引金額按分計算：配送先一つのみ
		/// ・セットプロモーション割引金額を、セットプロモーション対象商品の金額でそれぞれ按分すること。
		/// ・按分後に端数余りが発生した場合、セットプロモーション対象商品内の最も価格が高い商品に端数が重み付けされること。
		/// ※セットプロモーション設定と購入商品は以下とする
		///   ・セットプロモーション設定・対象商品：商品A 1個、商品B 1個
		///   ・購入商品：商品A 1個、商品B 2個
		/// </summary>
		[DataTestMethod()]
		[DynamicData("TdCalculateSetPromotionProductDiscountPriceTest_SingleShipping")]
		public void CalculateSetPromotionProductDiscountPriceTest_SingleShipping(
			CreateSetPromotionParamsForProductDiscount setPromotionParams,
			CartProduct[] products,
			AssertParamsForProrastedProductPrice[] assertParams,
			string msg)
		{
			using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), true))
			using (new TestConfigurator(Member.Of(() => Constants.TAX_EXCLUDED_FRACTION_ROUNDING),
				TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN))
			using (new TestConfigurator(Member.Of(() => Constants.GIFTORDER_OPTION_ENABLED), false))
			{
				// モックによるドメイン偽装
				var mock = new Mock<IProductTagService>();
				mock.Setup(s => s.GetProductTagSetting()).Returns(new ProductTagSettingModel[0]);
				DomainFacade.Instance.ProductTagService = mock.Object;
				// カート作成
				var cart = CartTestHelper.CreateCart();
				cart.Owner = CartTestHelper.CreateCartOwner();
				var shipping = cart.GetShipping();
				shipping.UpdateShippingAddr(cart.Owner, true);
				shipping.UpdateCartPriceShipping(1000m);
				cart.Items.AddRange(products);
				cart.SetPromotions.AddSetPromotion(CartTestHelper.CreateCartSetPromotion(
					cart,
					cart.Items,
					1,
					setPromotionParams.ProductDiscountKbn,
					setPromotionParams.ProductDiscountAmount,
					setPromotionParams.IsProductDiscount,
					setPromotionParams.IsShippingPriceFree));
				cart.CalculatePriceSubTotal();

				// 実行
				new PrivateObject(cart).Invoke("CalculateSetPromotionProductDiscountPrice");

				// カート商品へのセットプロモーション割引金額按分計算：配送先一つのみ
				// ・セットプロモーション割引金額を、セットプロモーション対象商品の金額でそれぞれ按分すること。
				// ・按分後に端数余りが発生した場合、セットプロモーション対象商品内の最も価格が高い商品に端数が重み付けされること。
				var index = 0;
				foreach (var item in cart.Items)
				{
					var resultExpected = assertParams[index];
					item.PriceSubtotalAfterDistribution.Should().Be(
						resultExpected.PriceSubtotalAfterDistribution,
						string.Format("{0}:カート商品・割引按分適用後小計", msg));
					item.DiscountedPrice[1].Should().Be(
						resultExpected.DiscountedPriceForSetPromotionTarget,
						string.Format("{0}:カート商品・明細金額", msg));
					item.PriceSubtotalAfterDistributionForCampaign.Should().Be(
						resultExpected.PriceSubtotalAfterDistributionForCampaign,
						string.Format("{0}:カート商品・割引按分適用後小計(キャンペーン用)", msg));
					index++;
				}
			}
		}

		public static object[] TdCalculateSetPromotionProductDiscountPriceTest_SingleShipping => new[]
		{
			// 按分後小数点以下端数金額なし
			new object[]
			{
				new CreateSetPromotionParamsForProductDiscount
				{
					ProductDiscountKbn = Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_KBN_DISCOUNT_PRICE,
					ProductDiscountAmount = 300m,
					IsProductDiscount = true
				},
				new CartProduct[]
				{
					// 商品A
					CartTestHelper.CreateCartProduct(
						1000m,
						1),
					// 商品B(2個購入中、1個がセットプロモーション対象)
					CartTestHelper.CreateCartProduct(
						2000m,
						2),
				},
				new AssertParamsForProrastedProductPrice[]
				{
					// 割引金額 = 300 * 1000 / (1000 + 2000) = 100
					new AssertParamsForProrastedProductPrice
					{
						PriceSubtotalAfterDistribution = 900m,
						PriceSubtotalAfterDistributionForCampaign = 900m,
						DiscountedPriceForSetPromotionTarget = 900m
					},
					// 割引金額 = 300 * 2000 / (1000 + 2000) = 200
					new AssertParamsForProrastedProductPrice
					{
						PriceSubtotalAfterDistribution = 3800m,
						PriceSubtotalAfterDistributionForCampaign = 3800m,
						DiscountedPriceForSetPromotionTarget = 1800m
					}
				},
				"按分後小数点以下端数金額なし"
			},
			// 按分後小数点以下端数金額あり
			new object[]
			{
				new CreateSetPromotionParamsForProductDiscount
				{
					ProductDiscountKbn = Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_KBN_DISCOUNT_PRICE,
					ProductDiscountAmount = 98m,
					IsProductDiscount = true
				},
				new CartProduct[]
				{
					// 商品A
					CartTestHelper.CreateCartProduct(
						1000m,
						1),
					// 商品B(2個購入中、1個がセットプロモーション対象)
					CartTestHelper.CreateCartProduct(
						2000m,
						2),
				},
				// 按分後小数点以下端数金額あり
				new AssertParamsForProrastedProductPrice[]
				{
					new AssertParamsForProrastedProductPrice
					{
						PriceSubtotalAfterDistribution = 968m,
						PriceSubtotalAfterDistributionForCampaign = 968m,
						DiscountedPriceForSetPromotionTarget = 968m
					},
					new AssertParamsForProrastedProductPrice
					{
						PriceSubtotalAfterDistribution = 3934m,
						PriceSubtotalAfterDistributionForCampaign = 3934m,
						DiscountedPriceForSetPromotionTarget = 1934m
					}
				},
				"按分後小数点以下端数金額あり"
			},
			// 配送料割引フラグON
			new object[]
			{
				new CreateSetPromotionParamsForProductDiscount
				{
					IsShippingPriceFree = true
				},
				new CartProduct[]
				{
					// 商品A
					CartTestHelper.CreateCartProduct(
						1000m,
						1),
					// 商品B(2個購入中、1個がセットプロモーション対象)
					CartTestHelper.CreateCartProduct(
						2000m,
						2),
				},
				new AssertParamsForProrastedProductPrice[]
				{
					// 割引金額 = 300 * 1000 / (1000 + 2000) = 100
					new AssertParamsForProrastedProductPrice
					{
						PriceSubtotalAfterDistribution = 1000m,
						PriceSubtotalAfterDistributionForCampaign = 1000m,
						DiscountedPriceForSetPromotionTarget = 1000m,
					},
					// 割引金額 = 300 * 2000 / (1000 + 2000) = 200
					new AssertParamsForProrastedProductPrice
					{
						PriceSubtotalAfterDistribution = 4000m,
						PriceSubtotalAfterDistributionForCampaign = 4000m,
						DiscountedPriceForSetPromotionTarget = 2000m
					}
				},
				"配送料割引フラグON"
			},
		};

		/// <summary>
		/// カート商品へのセットプロモーション割引金額按分計算：複数配送先
		/// ・セットプロモーション割引金額を、セットプロモーション対象商品の金額でそれぞれ按分すること。
		/// ・按分後に端数余りが発生した場合、セットプロモーション対象商品内の最も価格が高い商品に端数が重み付けされること。
		/// ※セットプロモーション設定と購入商品は以下とする
		///   ・セットプロモーション設定・対象商品：商品A 1個、商品B 1個
		///   ・購入商品：商品A 1個、商品B 2個
		/// </summary>
		[DataTestMethod()]
		[DynamicData("TdCalculateSetPromotionProductDiscountPriceTest_MultipleShipping")]
		public void CalculateSetPromotionProductDiscountPriceTest_MultipleShipping(
			CreateSetPromotionParamsForProductDiscount setPromotionParams,
			CreateCartShippingParams[] createCartShippingParams,
			AssertParamsForProrastedProductPrice[] assertParams,
			AssertParamsForProrastedShippingProductCountPrice[] assertShippingProductParams,
			string msg)
		{
			using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), true))
			using (new TestConfigurator(Member.Of(() => Constants.TAX_EXCLUDED_FRACTION_ROUNDING),
				TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN))
			using (new TestConfigurator(Member.Of(() => Constants.GIFTORDER_OPTION_ENABLED), true))
			{

				// モックによるドメイン偽装
				var mock = new Mock<IProductTagService>();
				mock.Setup(s => s.GetProductTagSetting()).Returns(new ProductTagSettingModel[0]);
				DomainFacade.Instance.ProductTagService = mock.Object;
				var cart = CartTestHelper.CreateMultipleShippingCart(createCartShippingParams);
				cart.SetPromotions.AddSetPromotion(CartTestHelper.CreateCartSetPromotion(
					cart,
					cart.Items,
					1,
					setPromotionParams.ProductDiscountKbn,
					setPromotionParams.ProductDiscountAmount,
					setPromotionParams.IsProductDiscount,
					setPromotionParams.IsShippingPriceFree));
				cart.CalculatePriceSubTotal();

				// 実行
				new PrivateObject(cart).Invoke("CalculateSetPromotionProductDiscountPrice");

				// カート商品へのセットプロモーション割引金額按分計算：複数配送先
				// ・セットプロモーション割引金額を、セットプロモーション対象商品の金額でそれぞれ按分すること。
				// ・按分後に端数余りが発生した場合、セットプロモーション対象商品内の最も価格が高い商品に端数が重み付けされること。
				var index = 0;
				foreach (var item in cart.Items)
				{
					var resultExpected = assertParams[index];
					item.PriceSubtotalAfterDistribution.Should().Be(
						(decimal)resultExpected.PriceSubtotalAfterDistribution,
						string.Format("{0}:カート商品・割引按分適用後小計", msg));
					item.DiscountedPrice[1].Should().Be(
						resultExpected.DiscountedPriceForSetPromotionTarget,
						string.Format("{0}:カート商品・明細金額", msg));
					item.PriceSubtotalAfterDistributionForCampaign.Should().Be(
						(decimal)resultExpected.PriceSubtotalAfterDistributionForCampaign,
						string.Format("{0}:カート商品・割引按分適用後小計(キャンペーン用)", msg));
					index++;
				}

				var shippingIndex = 0;
				foreach (var shipping in cart.Shippings)
				{
					var pcIndex = 0;
					foreach (var pc in shipping.ProductCounts)
					{
						var productCountsResultExpected = assertShippingProductParams[shippingIndex].PriceSubtotalAfterDistributions[pcIndex];
						pc.PriceSubtotalAfterDistribution.Should().Be(
							productCountsResultExpected,
							string.Format("{0}:配送先商品情報・割引按分適用後小計", msg));
						pcIndex++;
					}

					shippingIndex++;
				}
			}
		}

		public static object[] TdCalculateSetPromotionProductDiscountPriceTest_MultipleShipping => new[]
		{
			// 按分後小数点以下端数金額なし
			new object[]
			{
				new CreateSetPromotionParamsForProductDiscount
				{
					ProductDiscountKbn = Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_KBN_DISCOUNT_PRICE,
					ProductDiscountAmount = 300m,
					IsProductDiscount = true
				},
				new CreateCartShippingParams[]
				{
					new CreateCartShippingParams
					{
						ProductParams = new CreateCartProductParams[]
						{
							new CreateCartProductParams
							{
								Id = "0",
								ProductPrice = 1000m,
								Count = 1
							},
							new CreateCartProductParams
							{
								Id = "1",
								ProductPrice = 2000m,
								Count = 1
							},
						}
					},
					new CreateCartShippingParams
					{
						ProductParams = new CreateCartProductParams[]
						{
							new CreateCartProductParams
							{
								Id = "1",
								ProductPrice = 2000m,
								Count = 1
							},
						}
					},
				},
				new AssertParamsForProrastedProductPrice[]
				{
					new AssertParamsForProrastedProductPrice
					{
						// 割引金額 = 300 * 1000 / (1000 + 2000) = 100
						PriceSubtotalAfterDistribution = 900m,
						PriceSubtotalAfterDistributionForCampaign = 900m,
						DiscountedPriceForSetPromotionTarget = 900m
					},
					new AssertParamsForProrastedProductPrice
					{
						// 割引金額 = 300 * 2000 / (1000 + 2000) = 200
						PriceSubtotalAfterDistribution = 3800m,
						PriceSubtotalAfterDistributionForCampaign = 3800m,
						DiscountedPriceForSetPromotionTarget = 1800m
					},
				},
				new AssertParamsForProrastedShippingProductCountPrice[]
				{
					new AssertParamsForProrastedShippingProductCountPrice
					{
						PriceSubtotalAfterDistributions = new decimal[]
						{
							900m,
							2000m,
						}
					},
					new AssertParamsForProrastedShippingProductCountPrice
					{
						PriceSubtotalAfterDistributions = new decimal[]
						{
							1800m
						}
					},
				},
				"按分後小数点以下端数金額なし"
			},

			// 按分後小数点以下端数金額あり
			new object[]
			{
				new CreateSetPromotionParamsForProductDiscount
				{
					ProductDiscountKbn = Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_KBN_DISCOUNT_PRICE,
					ProductDiscountAmount = 98,
					IsProductDiscount = true
				},
				new CreateCartShippingParams[]
				{
					new CreateCartShippingParams
					{
						ProductParams = new CreateCartProductParams[]
						{
							new CreateCartProductParams
							{
								Id = "0",
								ProductPrice = 1000m,
								Count = 1
							},
							new CreateCartProductParams
							{
								Id = "1",
								ProductPrice = 2000m,
								Count = 1
							},
						}
					},
					new CreateCartShippingParams
					{
						ProductParams = new CreateCartProductParams[]
						{
							new CreateCartProductParams
							{
								Id = "1",
								ProductPrice = 2000m,
								Count = 1
							},
						}
					},
				},
				new AssertParamsForProrastedProductPrice[]
				{
					new AssertParamsForProrastedProductPrice
					{
						PriceSubtotalAfterDistribution = 968m,
						PriceSubtotalAfterDistributionForCampaign = 968m,
						DiscountedPriceForSetPromotionTarget = 968m
					},
					new AssertParamsForProrastedProductPrice
					{
						PriceSubtotalAfterDistribution = 3934m,
						PriceSubtotalAfterDistributionForCampaign = 3934m,
						DiscountedPriceForSetPromotionTarget = 1934m
					},
				},
				new AssertParamsForProrastedShippingProductCountPrice[]
				{
					new AssertParamsForProrastedShippingProductCountPrice
					{
						PriceSubtotalAfterDistributions = new decimal[]
						{
							968m,
							2000m,
						}
					},
					new AssertParamsForProrastedShippingProductCountPrice
					{
						PriceSubtotalAfterDistributions = new decimal[]
						{
							1934m
						}
					},
				},
				"按分後小数点以下端数金額あり"
			},
		};

		/// <summary>
		/// 商品割引セットプロモーション作成用パラメータ
		/// </summary>
		public class CreateSetPromotionParamsForProductDiscount
		{
			/// <summary>割引区分</summary>
			public string ProductDiscountKbn { get; set; } = Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_KBN_DISCOUNT_PRICE;
			/// <summary>割引額</summary>
			public decimal ProductDiscountAmount { get; set; } = 0m;
			/// <summary>商品割引フラグ</summary>
			public bool IsProductDiscount { get; set; } = false;
			/// <summary>配送料無料フラグ</summary>
			public bool IsShippingPriceFree { get; set; } = false;
		}

		/// <summary>
		/// 価格計算用商品作成パラメータ
		/// </summary>
		public class CreateCartProductParams
		{
			/// <summary>識別ID</summary>
			public string Id { get; set; } = "";
			/// <summary>単価</summary>
			public decimal ProductPrice { get; set; } = 0m;
			/// <summary>数量</summary>
			public int Count { get; set; } = 0;
		}

		/// <summary>
		/// 価格計算用配送先作成パラメータ
		/// </summary>
		public class CreateCartShippingParams
		{
			/// <summary>識別ID</summary>
			public CreateCartProductParams[] ProductParams { get; set; } = Array.Empty<CreateCartProductParams>();
		}

		/// <summary>
		/// 割引後価格の検証用パラメータ
		/// </summary>
		public class AssertParamsForProrastedProductPrice
		{
			/// <summary>商品小計(割引金額の按分処理適用後)</summary>
			public decimal PriceSubtotalAfterDistribution { get; set; } = 0m;
			/// <summary>商品小計(調整金額・割引金額の按分処理適用後)</summary>
			public decimal PriceSubtotalAfterDistributionForCampaign { get; set; } = 0m;
			/// <summary>割引後明細金額(セットプロモーション対象)</summary>
			public decimal DiscountedPriceForSetPromotionTarget { get; set; } = 0m;
		}

		/// <summary>
		/// 割引後価格の検証用パラメータ(配送先毎)
		/// </summary>
		public class AssertParamsForProrastedShippingProductCountPrice
		{
			/// <summary>商品小計(割引金額の按分処理適用後)</summary>
			public decimal[] PriceSubtotalAfterDistributions { get; set; } = Array.Empty<decimal>();
		}

		/// <summary>
		/// 配送料無料時の請求料金計算のテスト
		/// </summary>
		/// <param name="config">コンフィグ設定</param>
		/// <param name="data">データ</param>
		/// <param name="expected">期待値</param>
		/// <param name="msg">メッセージ</param>
		[DataTestMethod]
		[DynamicData(nameof(TdCalculateFreeShippingFeeTest))]
		public void CalculateFreeShippingFeeTest(
			dynamic config,
			dynamic data,
			dynamic expected,
			string msg)
		{
			var memberRankModel = new MemberRankModel
			{
				MemberRankId = "Test",
				ShippingDiscountType = data.ShippingDiscountType,
			};
			var cart = CartTestHelper.CreateCart(
				memberRankId: memberRankModel.MemberRankId);

			var cartProductUseExcludeFree = CartTestHelper.CreateCartProduct(
				isPluralShippingPriceFree: Constants.FLG_PRODUCT_PLURAL_SHIPPING_PRICE_FREE_FLG_INVALID,
				excludeFreeShippingFlg: Constants.FLG_PRODUCT_PLURAL_SHIPPING_PRICE_FREE_FLG_VALID);

			var cartProductPluralShippingPriceFree = CartTestHelper.CreateCartProduct(
				isPluralShippingPriceFree: Constants.FLG_PRODUCT_PLURAL_SHIPPING_PRICE_FREE_FLG_VALID,
				excludeFreeShippingFlg: Constants.FLG_PRODUCT_PLURAL_SHIPPING_PRICE_FREE_FLG_INVALID);

			cart.Coupon = new CartCoupon()
			{
				FreeShippingFlg = data.CouponFreeShippingFlag,
			};
			cart.SetPromotions.AddSetPromotion(CartTestHelper.CreateCartSetPromotion(
				cart: cart,
				cartProductList: cart.Items,
				allocateToSetPromotionQuantity: 1,
				productDiscountKbn: Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_KBN_DISCOUNT_PRICE,
				discountAmount: 1,
				isDiscountTypeProductDiscount: true,
				isDiscountTypeShippingChargeFree: data.IsDiscountTypeShippingChargeFree
			));
			using (new TestConfigurator(Member.Of(() => Constants.FREE_SHIPPING_FEE_OPTION_ENABLED), config.FreeShippingFeeOption))
			using (new TestConfigurator(Member.Of(() => Constants.MEMBER_RANK_OPTION_ENABLED), config.MemberRankOption))
			using (new MemberRankDataCacheConfigurator(new MemberRankModel[] { memberRankModel }))
			// 計算用の配送地帯区分ShippingZoneNoと複数商品配送料を設定する、
			// ON・OFFにより配送料無料の請求料の計算分岐パターンはないため、グローバルオプションをOFFにする
			using (new TestConfigurator(Member.Of(() => Constants.GLOBAL_OPTION_ENABLE), false))
			{
				var cartShipping = new CartShipping(cart);
				cartShipping.DeliveryCompanyId = "Test";
				cartShipping.SetShippingPrice(new ShopShippingModel()
				{
					ZoneList = new ShopShippingZoneModel[]
					{
						new ShopShippingZoneModel()
						{
							DeliveryCompanyId = "Test",
							ShippingZoneNo = data.ShippingZoneNo,
							SizeMailShippingPrice = 0m
						}
					},
					CompanyPostageSettings = new ShippingDeliveryPostageModel[]
					{
						new ShippingDeliveryPostageModel()
						{
							DeliveryCompanyId = "Test",
							PluralShippingPrice = data.PluralShippingPrice,
							CalculationPluralKbn = data.CalculationPluralKbn,
						}
					}
				}); ;
				cartShipping.FreeShippingFee = data.FreeShippingFee;
				cartShipping.ProductCounts.Add(new CartShipping.ProductCount(cartProductUseExcludeFree, data.FreeShippingFeeProductCount));
				cartShipping.ProductCounts.Add(new CartShipping.ProductCount(cartProductPluralShippingPriceFree, data.PluralShippingPriceFreeProductCount));
				cartShipping.UpdateCartPriceShipping((decimal)data.PriceShipping);
				cart.Shippings[0] = cartShipping;

				new PrivateObject(cart).Invoke("CalculateFreeShippingFee");
				foreach (var shipping in cart.Shippings)
				{
					shipping.IsUseFreeShippingFee.Should().Be(expected.IsUseFreeShippingFee, msg);
					shipping.PriceShipping.Should().Be(expected.ShippingPrice, msg);
				}
			}
		}

		private static IEnumerable<object[]> TdCalculateFreeShippingFeeTest
		{
			get
			{
				// data
				// PriceShipping ->配送料
				// FreeShippingFee ->配送料無料の請求料
				// CalculationPluralKbn -> 複数商品計算区分
				// ShippingDiscountType -> 会員ランク：配送料割引方法
				// CouponFreeShippingFlag -> クーポン：配送料無料有効フラグ
				// IsDiscountTypeShippingChargeFree -> セットプロモーション:配送料無料フラグ
				// ShippingZoneNo -> 配送料地帯区分番号
				// PluralShippingPrice -> 複数商品配送料
				// PluralShippingPriceFreeProductCount -> 配送料無料複数個の商品数
				// FreeShippingFeeProductCount -> 配送料無料時の請求料金利用の商品数
				yield return new object[]
				{
					new
					{
						FreeShippingFeeOption = false,
						MemberRankOption = true,
					},
					new
					{
						PriceShipping = 0m,
						FreeShippingFee = 10m,
						CalculationPluralKbn = Constants.FLG_SHOPSHIPPING_CALCULATION_PLURAL_KBN_SUM_OF_PRODUCT_SHIPPING_PRICE,
						ShippingDiscountType = Constants.FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_NONE,
						CouponFreeShippingFlag = Constants.FLG_COUPON_FREE_SHIPPING_INVALID,
						IsDiscountTypeShippingChargeFree = false,
						ShippingZoneNo = 1,
						PluralShippingPrice = 100m,
						PluralShippingPriceFreeProductCount = 2,
						FreeShippingFeeProductCount = 2,
					},
					new
					{
						IsUseFreeShippingFee = false,
						ShippingPrice = 0m,
					},
					"配送料無料の請求料のオプション：OFF"
				};
				yield return new object[]
				{
					new
					{
						FreeShippingFeeOption = true,
						MemberRankOption = true,
					},
					new
					{
						PriceShipping = 100m,
						FreeShippingFee = 10m,
						CalculationPluralKbn = Constants.FLG_SHOPSHIPPING_CALCULATION_PLURAL_KBN_SUM_OF_PRODUCT_SHIPPING_PRICE,
						ShippingDiscountType = Constants.FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_NONE,
						CouponFreeShippingFlag = Constants.FLG_COUPON_FREE_SHIPPING_INVALID,
						IsDiscountTypeShippingChargeFree = false,
						ShippingZoneNo = 1,
						PluralShippingPrice = 100m,
						PluralShippingPriceFreeProductCount = 2,
						FreeShippingFeeProductCount = 2,
					},
					new
					{
						IsUseFreeShippingFee = false,
						ShippingPrice = 100m,
					},
					"配送料が0円ではない"
				};
				yield return new object[]
				{
					new
					{
						FreeShippingFeeOption = true,
						MemberRankOption = true,
					},
					new
					{
						PriceShipping = 0m,
						FreeShippingFee = 10m,
						CalculationPluralKbn = Constants.FLG_SHOPSHIPPING_CALCULATION_PLURAL_KBN_SUM_OF_PRODUCT_SHIPPING_PRICE,
						ShippingDiscountType = Constants.FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_NONE,
						CouponFreeShippingFlag = Constants.FLG_COUPON_FREE_SHIPPING_INVALID,
						IsDiscountTypeShippingChargeFree = false,
						ShippingZoneNo = 1,
						PluralShippingPrice = 100m,
						PluralShippingPriceFreeProductCount = 2,
						FreeShippingFeeProductCount = 0,
					},
					new
					{
						IsUseFreeShippingFee = false,
						ShippingPrice = 0m,
					},
					"配送料無料の請求料を利用する商品はなし"
				};
				yield return new object[]
				{
					new
					{
						FreeShippingFeeOption = true,
						MemberRankOption = true,
					},
					new
					{
						PriceShipping = 0m,
						FreeShippingFee = 10m,
						CalculationPluralKbn = Constants.FLG_SHOPSHIPPING_CALCULATION_PLURAL_KBN_SUM_OF_PRODUCT_SHIPPING_PRICE,
						ShippingDiscountType = Constants.FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_NONE,
						CouponFreeShippingFlag = Constants.FLG_COUPON_FREE_SHIPPING_INVALID,
						IsDiscountTypeShippingChargeFree = false,
						ShippingZoneNo = 48,
						PluralShippingPrice = 100m,
						PluralShippingPriceFreeProductCount = 2,
						FreeShippingFeeProductCount = 2,
					},
					new
					{
						IsUseFreeShippingFee = false,
						ShippingPrice = 0m,
					},
					"配送地帯区分番号は47を超えた"
				};
				yield return new object[]
				{
					new
					{
						FreeShippingFeeOption = true,
						MemberRankOption = true,
					},
					new
					{
						PriceShipping = 0m,
						FreeShippingFee = 10m,
						CalculationPluralKbn = Constants.FLG_SHOPSHIPPING_CALCULATION_PLURAL_KBN_SUM_OF_PRODUCT_SHIPPING_PRICE,
						ShippingDiscountType = Constants.FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_NONE,
						CouponFreeShippingFlag = Constants.FLG_COUPON_FREE_SHIPPING_INVALID,
						IsDiscountTypeShippingChargeFree = false,
						ShippingZoneNo = 1,
						PluralShippingPrice = 100m,
						PluralShippingPriceFreeProductCount = 2,
						FreeShippingFeeProductCount = 2,
					},
					new
					{
						IsUseFreeShippingFee = true,
						ShippingPrice = 10m,
					},
					"複数商品計算区分：商品１つ毎に送料を設定"
				};
				yield return new object[]
				{
					new
					{
						FreeShippingFeeOption = true,
						MemberRankOption = true,
					},
					new
					{
						PriceShipping = 0m,
						FreeShippingFee = 10m,
						CalculationPluralKbn = Constants.FLG_SHOPSHIPPING_CALCULATION_PLURAL_KBN_HIGHEST_SHIPPING_PRICE_PLUS_PLURAL_PRICE,
						ShippingDiscountType = Constants.FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_NONE,
						CouponFreeShippingFlag = Constants.FLG_COUPON_FREE_SHIPPING_INVALID,
						IsDiscountTypeShippingChargeFree = false,
						ShippingZoneNo = 1,
						PluralShippingPrice = 100m,
						PluralShippingPriceFreeProductCount = 2,
						FreeShippingFeeProductCount = 2,
					},
					new
					{
						IsUseFreeShippingFee = true,
						ShippingPrice = 110m,
					},
					"複数商品計算区分：最も高い送料１点＋x円／個の送料を設定"
				};
				yield return new object[]
				{
					new
					{
						FreeShippingFeeOption = true,
						MemberRankOption = true,
					},
					new
					{
						PriceShipping = 0m,
						FreeShippingFee = 10m,
						CalculationPluralKbn = string.Empty,
						ShippingDiscountType = Constants.FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_NONE,
						CouponFreeShippingFlag = Constants.FLG_COUPON_FREE_SHIPPING_INVALID,
						IsDiscountTypeShippingChargeFree = false,
						ShippingZoneNo = 1,
						PluralShippingPrice = 100m,
						PluralShippingPriceFreeProductCount = 2,
						FreeShippingFeeProductCount = 2,
					},
					new
					{
						IsUseFreeShippingFee = true,
						ShippingPrice = 10m,
					},
					"複数商品計算区分：設定なし"
				};
				yield return new object[]
				{
					new
					{
						FreeShippingFeeOption = true,
						MemberRankOption = true,
					},
					new
					{
						PriceShipping = 0m,
						FreeShippingFee = 10m,
						CalculationPluralKbn = string.Empty,
						ShippingDiscountType = Constants.FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_NONE,
						CouponFreeShippingFlag = Constants.FLG_COUPON_FREE_SHIPPING_VALID,
						IsDiscountTypeShippingChargeFree = false,
						ShippingZoneNo = 1,
						PluralShippingPrice = 100m,
						PluralShippingPriceFreeProductCount = 2,
						FreeShippingFeeProductCount = 2,
					},
					new
					{
						IsUseFreeShippingFee = true,
						ShippingPrice = 10m,
					},
					"クーポンの割引で配送料無料"
				};
				yield return new object[]
				{
					new
					{
						FreeShippingFeeOption = true,
						MemberRankOption = true,
					},
					new
					{
						PriceShipping = 0m,
						FreeShippingFee = 10m,
						CalculationPluralKbn = string.Empty,
						ShippingDiscountType = Constants.FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_FREE,
						CouponFreeShippingFlag = Constants.FLG_COUPON_FREE_SHIPPING_INVALID,
						IsDiscountTypeShippingChargeFree = false,
						ShippingZoneNo = 1,
						PluralShippingPrice = 100m,
						PluralShippingPriceFreeProductCount = 2,
						FreeShippingFeeProductCount = 2,
					},
					new
					{
						IsUseFreeShippingFee = true,
						ShippingPrice = 10m,
					},
					"会員ランクの割引で配送料無料"
				};
				yield return new object[]
				{
					new
					{
						FreeShippingFeeOption = true,
						MemberRankOption = true,
					},
					new
					{
						PriceShipping = 0m,
						FreeShippingFee = 10m,
						CalculationPluralKbn = string.Empty,
						ShippingDiscountType = Constants.FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_NONE,
						CouponFreeShippingFlag = Constants.FLG_COUPON_FREE_SHIPPING_INVALID,
						IsDiscountTypeShippingChargeFree = true,
						ShippingZoneNo = 1,
						PluralShippingPrice = 100m,
						PluralShippingPriceFreeProductCount = 2,
						FreeShippingFeeProductCount = 2,
					},
					new
					{
						IsUseFreeShippingFee = true,
						ShippingPrice = 10m,
					},
					"セットプロモーションの割引で配送料無料"
				};
				yield return new object[]
				{
					new
					{
						FreeShippingFeeOption = true,
						MemberRankOption = false,
					},
					new
					{
						PriceShipping = 0m,
						FreeShippingFee = 10m,
						CalculationPluralKbn = string.Empty,
						ShippingDiscountType = Constants.FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_NONE,
						CouponFreeShippingFlag = Constants.FLG_COUPON_FREE_SHIPPING_INVALID,
						IsDiscountTypeShippingChargeFree = true,
						ShippingZoneNo = 1,
						PluralShippingPrice = 100m,
						PluralShippingPriceFreeProductCount = 2,
						FreeShippingFeeProductCount = 2,
					},
					new
					{
						IsUseFreeShippingFee = true,
						ShippingPrice = 10m,
					},
					"会員ランクオプションはOFF"
				};
			}
		}

		/// <summary>
		/// 配送料金
		/// </summary>
		/// <param name="config">コンフィグ設定</param>
		/// <param name="data">データ</param>
		/// <param name="expected">期待値</param>
		/// <param name="msg">メッセージ</param>
		[DataTestMethod]
		[DynamicData(nameof(TdCalculateShippingPriceTest))]
		public void CalculateShippingPriceTest(
			dynamic config,
			dynamic data,
			dynamic expected,
			string msg)
		{
			var memberRankModel = new MemberRankModel()
			{
				MemberRankId = "Test",
			};
			var shopShippingModel = CartTestHelper.GetShopShippingModel();
			using (new MemberRankDataCacheConfigurator(new MemberRankModel[] { memberRankModel }))
			using (new ShopShippingDataCacheConfigurator(new ShopShippingModel[] { shopShippingModel }))
			using (new TestConfigurator(Member.Of(() => Constants.GLOBAL_OPTION_ENABLE), config.GlobalOpitonEnable))
			using (new TestConfigurator(Member.Of(() => Constants.TW_COUNTRY_SHIPPING_ENABLE), config.TwCountryShippingEnbale))
			using (new TestConfigurator(Member.Of(() => Constants.MEMBER_RANK_OPTION_ENABLED), config.MemberRankOptionEnable))
			using (new TestConfigurator(Member.Of(() => Constants.OPERATIONAL_BASE_ISO_CODE), config.OperationalBaseIsoCode))
			using (new TestConfigurator(Member.Of(() => Constants.MAIL_ESCALATION_COUNT), 3))
			{

				var cart = CartTestHelper.CreateCart(
					memberRankId: memberRankModel.MemberRankId);

				var cartShipping = new CartShipping(cart);
				cartShipping.UpdateCartPriceShipping(data.BeforePriceShipping);
				cartShipping.ShippingCountryIsoCode = data.ShippingCountryIsoCode;
				cartShipping.DeliveryCompanyId = data.DeliveryCompanyId;

				var cartProduct = CartTestHelper.CreateCartProduct(
					weight: "100",
					shippingSizeKbn: Constants.FLG_PRODUCT_SHIPPING_SIZE_KBN_L);
				cart.Items.Add(cartProduct);
				cart.Shippings[0] = cartShipping;

				var shopShippingServiceMock = new Mock<IShopShippingService>();
				shopShippingServiceMock.Setup(s => s.GetFromZipcode(
					It.IsAny<string>(),
					It.IsAny<string>(),
					It.IsAny<int>(),
					It.IsAny<string>(),
					It.IsAny<string>(),
					It.IsAny<int>())).Returns((ShopShippingModel)data.ShopShippingModelFromDb);
				DomainFacade.Instance.ShopShippingService = shopShippingServiceMock.Object;

				// グローバル配送 0-10000g 200円
				var globalShippingPostageModels = new GlobalShippingPostageModel[]
				{
					new GlobalShippingPostageModel(new Hashtable()
					{
						{ Constants.FIELD_GLOBALSHIPPINGPOSTAGE_SEQ, 1L},
						{ Constants.FIELD_GLOBALSHIPPINGPOSTAGE_WEIGHT_GRAM_GREATER_THAN_OR_EQUAL_TO, 0L},
						{ Constants.FIELD_GLOBALSHIPPINGPOSTAGE_WEIGHT_GRAM_LESS_THAN, 10000L},
						{ Constants.FIELD_GLOBALSHIPPINGPOSTAGE_GLOBAL_SHIPPING_POSTAGE, 200m},
					}),
				};
				var globalShippingServiceMock = new Mock<IGlobalShippingService>();
				globalShippingServiceMock.Setup(s => s.GetAreaPostageByShippingDeliveryCompany(
					It.IsAny<string>(),
					It.IsAny<string>(),
					It.IsAny<string>(),
					It.IsAny<string>())).Returns(globalShippingPostageModels);
				globalShippingServiceMock.Setup(s => s.DistributesShippingArea(
					It.IsAny<string>(),
					It.IsAny<string>(),
					It.IsAny<string>(),
					It.IsAny<string>(),
					It.IsAny<string>(),
					It.IsAny<string>())).Returns(new GlobalShippingAreaComponentModel());
				DomainFacade.Instance.GlobalShippingService = globalShippingServiceMock.Object;

				var productServiceMock = new Mock<IProductService>();
				productServiceMock.Setup(s => s.GetProducts(
					It.IsAny<string>(),
					It.IsAny<string[]>(),
					It.IsAny<SqlAccessor>())).Returns(new ProductModel[] { new ProductModel() });
				DomainFacade.Instance.ProductService = productServiceMock.Object;

				var result = new PrivateObject(cart).Invoke("CalculateShippingPrice", (bool)data.SetDefaultShipping);
				result.Should().Be(expected.Expected, msg);
				foreach (var shipping in cart.Shippings)
				{
					shipping.PriceShipping.Should().Be(expected.ShippingPrice, msg);
				}
			}
		}

		private static IEnumerable<object[]> TdCalculateShippingPriceTest
		{
			get
			{
				yield return new object[]
				{
					new
					{
						GlobalOpitonEnable = false,
						MemberRankOptionEnable = true,
						OperationalBaseIsoCode = Constants.COUNTRY_ISO_CODE_JP,
						TwCountryShippingEnbale = false,
					},
					new
					{
						SetDefaultShipping = false,
						BeforePriceShipping = 0m,
						DeliveryCompanyId = "TEST",
						ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP,
						ShopShippingModelFromDb = CartTestHelper.GetShopShippingModel(),
					},
					new
					{
						Expected = true,
						ShippingPrice = 100m,
					},
					"日本国内配送、配送料変更ある、Lサイズ100円"
				};
				yield return new object[]
				{
					new
					{
						GlobalOpitonEnable = false,
						MemberRankOptionEnable = true,
						OperationalBaseIsoCode = Constants.COUNTRY_ISO_CODE_JP,
						TwCountryShippingEnbale = false,
					},
					new
					{
						SetDefaultShipping = false,
						BeforePriceShipping = 100m,
						DeliveryCompanyId = "TEST",
						ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP,
						ShopShippingModelFromDb = CartTestHelper.GetShopShippingModel(),
					},
					new
					{
						Expected = false,
						ShippingPrice = 100m,
					},
					"日本国内配送、配送料変更なし、Lサイズ100円"
				};
				yield return new object[]
				{
					new
					{
						GlobalOpitonEnable = false,
						MemberRankOptionEnable = true,
						OperationalBaseIsoCode = Constants.COUNTRY_ISO_CODE_US,
						TwCountryShippingEnbale = true,
					},
					new
					{
						SetDefaultShipping = true,
						BeforePriceShipping = 0m,
						DeliveryCompanyId = "TEST",
						ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_US,
						ShopShippingModelFromDb = CartTestHelper.GetShopShippingModel(),
					},
					new
					{
						Expected = true,
						ShippingPrice = 100m,
					},
					"グローバルオプションOFF、ISOは日本以外も日本の配送料で計算する"
				};
				yield return new object[]
				{
					new
					{
						GlobalOpitonEnable = true,
						MemberRankOptionEnable = true,
						OperationalBaseIsoCode = Constants.COUNTRY_ISO_CODE_US,
						TwCountryShippingEnbale = true,
					},
					new
					{
						SetDefaultShipping = true,
						BeforePriceShipping = 0m,
						DeliveryCompanyId = "TEST",
						ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_US,
						ShopShippingModelFromDb = CartTestHelper.GetShopShippingModel(),
					},
					new
					{
						Expected = true,
						ShippingPrice = 200m,
					},
					"海外配送、配送料変更ある、100g200円"
				};
				yield return new object[]
				{
					new
					{
						GlobalOpitonEnable = true,
						MemberRankOptionEnable = true,
						OperationalBaseIsoCode = Constants.COUNTRY_ISO_CODE_TW,
						TwCountryShippingEnbale = true,
					},
					new
					{
						SetDefaultShipping = true,
						BeforePriceShipping = 0m,
						DeliveryCompanyId = "TEST",
						ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_TW,
						ShopShippingModelFromDb = CartTestHelper.GetShopShippingModel(),
					},
					new
					{
						Expected = true,
						ShippingPrice = 100m,
					},
					"海外配送、配送料変更ある、台湾配送オプションはON、日本の配送料計算ロジックと同じく"
				};
				yield return new object[]
				{
					new
					{
						GlobalOpitonEnable = true,
						MemberRankOptionEnable = true,
						OperationalBaseIsoCode = Constants.COUNTRY_ISO_CODE_TW,
						TwCountryShippingEnbale = false,
					},
					new
					{
						SetDefaultShipping = true,
						BeforePriceShipping = 0m,
						DeliveryCompanyId = "TEST",
						ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_TW,
						ShopShippingModelFromDb = CartTestHelper.GetShopShippingModel(),
					},
					new
					{
						Expected = true,
						ShippingPrice = 200m,
					},
					"海外配送、配送料変更ある、台湾配送オプションはOFF、100g200円"
				};
				// 下記のパターンは計算のための配送情報だけ影響するので、1パターンを確認する
				yield return new object[]
				{
					new
					{
						GlobalOpitonEnable = false,
						MemberRankOptionEnable = true,
						OperationalBaseIsoCode = Constants.COUNTRY_ISO_CODE_JP,
						TwCountryShippingEnbale = false,
					},
					new
					{
						SetDefaultShipping = true,
						BeforePriceShipping = 0m,
						DeliveryCompanyId = "TEST",
						ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP,
						ShopShippingModelFromDb = CartTestHelper.GetShopShippingModel(),
					},
					new
					{
						Expected = true,
						ShippingPrice = 100m,
					},
					"日本国内配送、配送料変更ある、デフォルト配送先利用する、Lサイズ100円"
				};
				yield return new object[]
				{
					new
					{
						GlobalOpitonEnable = false,
						MemberRankOptionEnable = true,
						OperationalBaseIsoCode = Constants.COUNTRY_ISO_CODE_JP,
						TwCountryShippingEnbale = false,
					},
					new
					{
						SetDefaultShipping = false,
						BeforePriceShipping = 0m,
						DeliveryCompanyId = "",
						ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP,
						ShopShippingModelFromDb = CartTestHelper.GetShopShippingModel(),
					},
					new
					{
						Expected = true,
						ShippingPrice = 100m,
					},
					"日本国内配送、配送料変更ある、配送会社IDはヌル、Lサイズ100円"
				};
				yield return new object[]
				{
					new
					{
						GlobalOpitonEnable = true,
						MemberRankOptionEnable = true,
						OperationalBaseIsoCode = Constants.COUNTRY_ISO_CODE_US,
						TwCountryShippingEnbale = false,
					},
					new
					{
						SetDefaultShipping = true,
						BeforePriceShipping = 0m,
						DeliveryCompanyId = "TEST",
						ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_US,
						ShopShippingModelFromDb = CartTestHelper.GetShopShippingModel(),
					},
					new
					{
						Expected = true,
						ShippingPrice = 200m,
					},
					"海外配送、配送料変更ある、台湾配送オプションはOFF、100g200円"
				};
				yield return new object[]
				{
					new
					{
						GlobalOpitonEnable = true,
						MemberRankOptionEnable = true,
						OperationalBaseIsoCode = Constants.COUNTRY_ISO_CODE_US,
						TwCountryShippingEnbale = false,
					},
					new
					{
						SetDefaultShipping = true,
						BeforePriceShipping = 0m,
						DeliveryCompanyId = "TEST",
						ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_US,
						ShopShippingModelFromDb = (ShopShippingModel) null,
					},
					new
					{
						Expected = true,
						ShippingPrice = 200m,
					},
					"海外配送、DBから取得した店舗配送情報はヌル、配送料変更ある、台湾配送オプションはOFF、100g200円"
				};
			}
		}
	}

	/// <summary>
	/// CartObjectのテスト
	/// </summary>
	/// <remarks>
	/// 古い形式なので一旦Ignoreとする
	/// 順次CartObjectTestsに以降する
	/// </remarks>
	[TestClass()]
	[Ignore]
	public class CartObjectTestsOLd : AppTestClassBase
	{
		/// <summary>
		/// 商品金額合計再計算：配送先一つのみ
		/// ・商品単価と購入個数からカート商品小計が計算されること
		/// ・「商品小計(割引金額の按分処理適用後)」が初期化されること
		/// ・再計算前と商品小計の値が異なっていればTRUE、同一ならばFALSEが戻り値で帰ってくること
		///	 ・「cartShipping.ShippingCountryIsoCode」がConstants.OPERATIONAL_BASE_ISO_CODEの国コードと同一な場合 → 「商品小計(割引金額の按分処理適用後)」に消費税額が含まれていること。
		///	 ・「cartShipping.ShippingCountryIsoCode」がConstants.OPERATIONAL_BASE_ISO_CODEの国コードと異なる場合 → 「商品小計(割引金額の按分処理適用後)」に消費税額が含まれていないこと。
		/// </summary>
		[DataTestMethod()]
		[DynamicData("m_tdCalculatePriceSubTotaTest_SingleShipping")]
		public void CalculatePriceSubTotaTest_SingleShipping(
			dynamic config,
			dynamic data,
			dynamic expected,
			string msg)
		{
			using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), false))
			using (new TestConfigurator(Member.Of(() => Constants.OPERATIONAL_BASE_ISO_CODE), Constants.COUNTRY_ISO_CODE_JP))
			using (new TestConfigurator(Member.Of(() => Constants.GLOBAL_TRANSACTION_INCLUDED_TAX_FLAG), false))
			using (new TestConfigurator(Member.Of(() => Constants.GIFTORDER_OPTION_ENABLED), false))
			using (new TestConfigurator(Member.Of(() => Constants.MEMBER_RANK_OPTION_ENABLED), false))
			using (new TestConfigurator(Member.Of(() => Constants.TAX_EXCLUDED_FRACTION_ROUNDING),
				TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN))
			{
				// モックによるドメイン偽装
				var mock = new Mock<IProductTagService>();
				mock.Setup(s => s.GetProductTagSetting()).Returns(new ProductTagSettingModel[0]);
				DomainFacade.Instance.ProductTagService = mock.Object;

				var cart = CartTestHelper.CreateCart();
				cart.Owner = CartTestHelper.CreateCartOwner();
				foreach (var productParam in data.ProductParams)
				{
					var cartProduct = CartTestHelper.CreateCartProduct(
						productPrice: productParam.ProductPrice,
						productCount: productParam.ItemQuantity);
					cart.Items.Add(cartProduct);
				}

				cart.GetShipping().UpdateShippingAddr(cart.Owner, true);
				cart.GetShipping().ShippingCountryIsoCode = data.ShippingCountryIsoCode;
				cart.SetPriceSubtotal(data.PriceSubtotalOld, data.PriceTaxOld);

				var isPriceChanged = cart.CalculatePriceSubTotal();

				// 商品金額合計再計算：配送先一つのみ
				// ・商品単価と購入個数からカート商品小計が計算されること
				// ・「商品小計(割引金額の按分処理適用後)」が再計算されること
				// ・再計算前とPriceSubtotalプロパティの値が異なっていればTRUE、同一ならばFALSEが戻り値で帰ってくること
				var index = 0;
				foreach (var item in cart.Items)
				{
					var resultProductExpected = expected.ProductsExpected[index];
					item.PriceSubtotalSingle.Should().Be((decimal)resultProductExpected.ItemPrice,
						msg + " : カート商品・商品小計(セット未考慮)");
					item.PriceSubtotalSinglePretax.Should().Be((decimal)resultProductExpected.ItemPricePretax,
						msg + " : カート商品・税込商品小計(セット未考慮)");
					item.PriceSubtotalSingleTax.Should().Be((decimal)resultProductExpected.TaxPrice,
						msg + " : カート商品・商品税額小計(セット未考慮)");
					item.Count.Should().Be((int)resultProductExpected.ProductCount, msg + " : カート商品・商品数");
					item.PriceSubtotal.Should().Be((decimal)resultProductExpected.ItemPrice, msg + " : カート商品・商品小計");
					item.PriceSubtotalPretax.Should().Be((decimal)resultProductExpected.ItemPricePretax,
						msg + " : カート商品・税込商品小計");
					item.PriceSubtotalTax.Should().Be((decimal)resultProductExpected.TaxPrice, msg + " : カート商品・商品税額小計");
					item.PriceSubtotalAfterDistribution.Should().Be(
						(decimal)resultProductExpected.PriceSubtotalAfterDistribution,
						msg + " : カート商品・割引按分適用後商品小計");
					item.PriceSubtotalAfterDistributionForCampaign.Should().Be(
						(decimal)resultProductExpected.PriceSubtotalAfterDistribution,
						msg + " : カート商品・割引按分適用後商品小計(キャンペーン適用対象)");
					index++;
				}

				isPriceChanged.Should().Be((bool)expected.IsPriceChangedExpected, msg + " : 商品金額合計再計算");
			}
		}

		public static object[] m_tdCalculatePriceSubTotaTest_SingleShipping = new[]
		{
			// 国内配送・商品小計変化あり
			new object[]
			{
				new {},
				new
				{
					ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP,
					PriceSubtotalOld = 10000m,
					PriceTaxOld = 1000m,
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 1000m,
							ItemQuantity = 1,
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 2,
						},
					},
				},
				new
				{
					ProductsExpected = new[]
					{
						new
						{
							ItemPrice = 1000m,
							TaxPrice = 100m,
							ProductCount = 1,
							ItemPricePretax = 1100m,
							PriceSubtotalAfterDistribution = 1100m,
						},
						new
						{
							ItemPrice = 4000m,
							TaxPrice = 400m,
							ProductCount = 2,
							ItemPricePretax = 4400m,
							PriceSubtotalAfterDistribution = 4400m,
						}
					},
					IsPriceChangedExpected = true,
				},
				"国内配送・商品小計変化あり"
			},
			// 国内配送・商品小計変化なし
			new object[]
			{
				new {},
				new
				{
					ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP,
					PriceSubtotalOld = 5000m,
					PriceTaxOld = 500m,
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 1000m,
							ItemQuantity = 1,
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 2,
						},
					},
				},
				new
				{
					ProductsExpected = new[]
					{
						new
						{
							ItemPrice = 1000m,
							TaxPrice = 100m,
							ProductCount = 1,
							ItemPricePretax = 1100m,
							PriceSubtotalAfterDistribution = 1100m,
						},
						new
						{
							ItemPrice = 4000m,
							TaxPrice = 400m,
							ProductCount = 2,
							ItemPricePretax = 4400m,
							PriceSubtotalAfterDistribution = 4400m,
						}
					},
					IsPriceChangedExpected = false,
				},
				"国内配送・商品小計変化なし"
			},
			// 海外配送・商品小計変化あり
			new object[]
			{
				new {},
				new
				{
					ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_US,
					PriceSubtotalOld = 10000m,
					PriceTaxOld = 1000m,
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 1000m,
							ItemQuantity = 1,
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 2,
						},
					},
				},
				new
				{
					ProductsExpected = new[]
					{
						new
						{
							ItemPrice = 1000m,
							TaxPrice = 100m,
							ProductCount = 1,
							ItemPricePretax = 1100m,
							PriceSubtotalAfterDistribution = 1000m,
						},
						new
						{
							ItemPrice = 4000m,
							TaxPrice = 400m,
							ProductCount = 2,
							ItemPricePretax = 4400m,
							PriceSubtotalAfterDistribution = 4000m,
						}
					},
					IsPriceChangedExpected = true,
				},
				"海外配送・商品小計変化あり"
			},
			// 海外配送・商品小計変化なし
			new object[]
			{
				new {},
				new
				{
					ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_US,
					PriceSubtotalOld = 5000m,
					PriceTaxOld = 500m,
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 1000m,
							ItemQuantity = 1,
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 2,
						},
					},
				},
				new
				{
					ProductsExpected = new[]
					{
						new
						{
							ItemPrice = 1000m,
							TaxPrice = 100m,
							ProductCount = 1,
							ItemPricePretax = 1100m,
							PriceSubtotalAfterDistribution = 1000m,
						},
						new
						{
							ItemPrice = 4000m,
							TaxPrice = 400m,
							ProductCount = 2,
							ItemPricePretax = 4400m,
							PriceSubtotalAfterDistribution = 4000m,
						}
					},
					IsPriceChangedExpected = false,
				},
				"海外配送・商品小計変化なし"
			},
		};

		/// <summary>
		/// 商品金額合計再計算：複数配送先対応
		/// ・商品単価と購入個数からカート商品小計が計算されること
		/// ・「商品小計(割引金額の按分処理適用後)」が再計算されること
		/// ・再計算前とPriceSubtotalプロパティの値が異なっていればTRUE、同一ならばFALSEが戻り値で帰ってくること
		///	 ・「CartShipping.ShippingCountryIsoCode」がConstants.OPERATIONAL_BASE_ISO_CODEの国コードと同一な場合 → 消費税額が含まれた金額が入っていること。
		///	 ・「CartShipping.ShippingCountryIsoCode」がConstants.OPERATIONAL_BASE_ISO_CODEの国コードと異なる場合 → 消費税額が含まれない金額が入っていること。
		/// </summary>
		[DataTestMethod()]
		[DynamicData("m_tdCalculatePriceSubTotaTest_MultipleShipping")]
		public void CalculatePriceSubTotaTest_MultipleShipping(
			dynamic config,
			dynamic data,
			dynamic expected,
			string msg)
		{
			using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), false))
			using (new TestConfigurator(Member.Of(() => Constants.OPERATIONAL_BASE_ISO_CODE), Constants.COUNTRY_ISO_CODE_JP))
			using (new TestConfigurator(Member.Of(() => Constants.GLOBAL_TRANSACTION_INCLUDED_TAX_FLAG), false))
			using (new TestConfigurator(Member.Of(() => Constants.GIFTORDER_OPTION_ENABLED), true))
			using (new TestConfigurator(Member.Of(() => Constants.MEMBER_RANK_OPTION_ENABLED), false))
			using (new TestConfigurator(Member.Of(() => Constants.TAX_EXCLUDED_FRACTION_ROUNDING),
				TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN))
			{

				// モックによるドメイン偽装
				var mock = new Mock<IProductTagService>();
				mock.Setup(s => s.GetProductTagSetting()).Returns(new ProductTagSettingModel[0]);
				DomainFacade.Instance.ProductTagService = mock.Object;

				var cart = CartTestHelper.CreateCart();
				cart.SetPriceSubtotal(data.PriceSubtotalOld, data.PriceTaxOld);
				cart.Owner = CartTestHelper.CreateCartOwner();
				cart.Shippings.Clear();
				foreach (var cartItemParam in data.CartItemParams)
				{
					var cartProduct = CartTestHelper.CreateCartProduct(
						productPrice: cartItemParam.ProductPrice,
						productCount: cartItemParam.ItemQuantity,
						addCartKbn: Constants.AddCartKbn.GiftOrder);
					cart.Items.Add(cartProduct);
				}

				foreach (var shippingParam in data.ShippingParams)
				{
					var cartShipping = new CartShipping(cart);
					cartShipping.UpdateShippingAddr(cart.Owner, true);
					cartShipping.ShippingCountryIsoCode = shippingParam.ShippingCountryIsoCode;
					foreach (var productCountsParam in shippingParam.ProductCountsParams)
					{
						var productCounts = new CartShipping.ProductCount(
							cart.Items[productCountsParam.IndexOfCartProduct],
							productCountsParam.ItemQuantity);
						cartShipping.ProductCounts.Add(productCounts);
					}

					cart.Shippings.Add(cartShipping);
				}

				var result = cart.CalculatePriceSubTotal();

				// 商品金額合計再計算：複数配送先対応
				// ・商品単価と購入個数からカート商品小計が計算されること
				// ・「商品小計(割引金額の按分処理適用後)」が再計算されること
				// ・再計算前とPriceSubtotalプロパティの値が異なっていればTRUE、同一ならばFALSEが戻り値で帰ってくること
				var index = 0;
				foreach (var item in cart.Items)
				{
					var resultProduct = expected.ProductsExpected[index];
					item.PriceSubtotalSingle.Should().Be((decimal)resultProduct.ItemPrice, msg + " : カート商品・商品小計(セット未考慮)");
					item.PriceSubtotalSinglePretax.Should().Be((decimal)resultProduct.ItemPricePretax,
						msg + " : カート商品・税込商品小計(セット未考慮)");
					item.PriceSubtotalSingleTax.Should().Be((decimal)resultProduct.TaxPrice,
						msg + " : カート商品・商品税額小計(セット未考慮)");
					item.Count.Should().Be((int)resultProduct.ProductCount, msg + " : カート商品・商品数");
					item.PriceSubtotal.Should().Be((decimal)resultProduct.ItemPrice, msg + " : カート商品・商品小計");
					item.PriceSubtotalPretax.Should().Be((decimal)resultProduct.ItemPricePretax, msg + " : カート商品・税込商品小計");
					item.PriceSubtotalTax.Should().Be((decimal)resultProduct.TaxPrice, msg + " : カート商品・商品税額小計");
					item.PriceSubtotalAfterDistribution.Should().Be((decimal)resultProduct.PriceSubtotalAfterDistribution,
						msg + " : カート商品・割引按分適用後商品小計");
					item.PriceSubtotalAfterDistributionForCampaign.Should().Be(
						(decimal)resultProduct.PriceSubtotalAfterDistribution, msg + " : カート商品・割引按分適用後商品小計");
					index++;
				}

				var shippingIndex = 0;
				foreach (var shipping in cart.Shippings)
				{
					var pcIndex = 0;
					foreach (var pc in shipping.ProductCounts)
					{
						var productCountsResultExpected = expected.ProductCountsExpected[shippingIndex][pcIndex];
						pc.PriceSubtotalAfterDistribution.Should().Be(
							(decimal)productCountsResultExpected.PriceSubtotalAfterDistribution,
							msg + " : 配送先商品情報・割引按分適用後小計");
						pcIndex++;
					}

					shippingIndex++;
				}

				result.Should().Be((bool)expected.IsPriceChangedExpected, msg + " : 小計再計算");
			}
		}

		public static object[] m_tdCalculatePriceSubTotaTest_MultipleShipping = new[]
		{
			// 国内配送・商品小計変化あり
			new object[]
			{
				new {},
				new
				{
					PriceSubtotalOld = 10000m,
					PriceTaxOld = 1000m,
					CartItemParams = new[]
					{
						new
						{
							ProductPrice = 100m,
							ItemQuantity = 1,
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 2,
						},
					},
					ShippingParams = new[]
					{
						new
						{
							ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP,
							ProductCountsParams =  new[]
							{
								new
								{
									IndexOfCartProduct = 0,
									ItemQuantity = 1,
								},
								new
								{
									IndexOfCartProduct = 1,
									ItemQuantity = 1,
								},
							},
						},
						new
						{
							ShippingCountryIsoCode =  Constants.COUNTRY_ISO_CODE_JP,
							ProductCountsParams = new[]
							{
								new
								{
									IndexOfCartProduct = 1,
									ItemQuantity = 1,
								},
							}
						}
					},
				},
				new
				{
					ProductsExpected = new[]
					{
						new
						{
							ItemPrice = 100m,
							TaxPrice = 10m,
							ProductCount = 1,
							ItemPricePretax = 110m,
							PriceSubtotalAfterDistribution = 110m,
						},
						new
						{
							ItemPrice = 4000m,
							TaxPrice = 400m,
							ProductCount = 2,
							ItemPricePretax = 4400m,
							PriceSubtotalAfterDistribution = 4400m,
						}
					},
					ProductCountsExpected = new[]
					{
						new[]
						{
							new
							{
								PriceSubtotalAfterDistribution = 110m,
							},
							new
							{
								PriceSubtotalAfterDistribution = 2200m,
							}
						},
						new[]
						{
							new
							{
								PriceSubtotalAfterDistribution = 2200m,
							}
						}
					},
					IsPriceChangedExpected = true,
				},
				"国内配送・商品小計変化あり"
			},
			// 国内配送・商品小計変化なし
			new object[]
			{
				new {},
				new
				{
					PriceSubtotalOld = 4100m,
					PriceTaxOld = 410m,
					CartItemParams = new[]
					{
						new
						{
							ProductPrice = 100m,
							ItemQuantity = 1,
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 2,
						},
					},
					ShippingParams = new[]
					{
						new
						{
							ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP,
							ProductCountsParams =  new[]
							{
								new
								{
									IndexOfCartProduct = 0,
									ItemQuantity = 1,
								},
								new
								{
									IndexOfCartProduct = 1,
									ItemQuantity = 1,
								},
							},
						},
						new
						{
							ShippingCountryIsoCode =  Constants.COUNTRY_ISO_CODE_JP,
							ProductCountsParams = new[]
							{
								new
								{
									IndexOfCartProduct = 1,
									ItemQuantity = 1,
								},
							}
						}
					},
				},
				new
				{
					ProductsExpected = new[]
					{
						new
						{
							ItemPrice = 100m,
							TaxPrice = 10m,
							ProductCount = 1,
							ItemPricePretax = 110m,
							PriceSubtotalAfterDistribution = 110m,
						},
						new
						{
							ItemPrice = 4000m,
							TaxPrice = 400m,
							ProductCount = 2,
							ItemPricePretax = 4400m,
							PriceSubtotalAfterDistribution = 4400m,
						}
					},
					ProductCountsExpected = new[]
					{
						new[]
						{
							new
							{
								PriceSubtotalAfterDistribution = 110m,
							},
							new
							{
								PriceSubtotalAfterDistribution = 2200m,
							}
						},
						new[]
						{
							new
							{
								PriceSubtotalAfterDistribution = 2200m,
							}
						}
					},
					IsPriceChangedExpected = false,
				},
				"国内配送・商品小計変化なし"
			},
			// 海外配送・商品小計変化あり
			new object[]
			{
				new {},
				new
				{
					PriceSubtotalOld = 10000m,
					PriceTaxOld = 2000m,
					CartItemParams = new[]
					{
						new
						{
							ProductPrice = 100m,
							ItemQuantity = 1,
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 2,
						},
					},
					ShippingParams = new[]
					{
						new
						{
							ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_US,
							ProductCountsParams =  new[]
							{
								new
								{
									IndexOfCartProduct = 0,
									ItemQuantity = 1,
								},
								new
								{
									IndexOfCartProduct = 1,
									ItemQuantity = 1,
								},
							},
						},
						new
						{
							ShippingCountryIsoCode =  Constants.COUNTRY_ISO_CODE_US,
							ProductCountsParams = new[]
							{
								new
								{
									IndexOfCartProduct = 1,
									ItemQuantity = 1,
								},
							}
						}
					},
				},
				new
				{
					ProductsExpected = new[]
					{
						new
						{
							ItemPrice = 100m,
							TaxPrice = 10m,
							ProductCount = 1,
							ItemPricePretax = 110m,
							PriceSubtotalAfterDistribution = 100m,
						},
						new
						{
							ItemPrice = 4000m,
							TaxPrice = 400m,
							ProductCount = 2,
							ItemPricePretax = 4400m,
							PriceSubtotalAfterDistribution = 4000m,
						}
					},
					ProductCountsExpected = new[]
					{
						new[]
						{
							new
							{
								PriceSubtotalAfterDistribution = 100m,
							},
							new
							{
								PriceSubtotalAfterDistribution = 2000m,
							}
						},
						new[]
						{
							new
							{
								PriceSubtotalAfterDistribution = 2000m,
							}
						}
					},
					IsPriceChangedExpected = true,
				},
				"海外配送・商品小計変化あり"
			},
			// 海外配送・商品小計変化なし
			new object[]
			{
				new {},
				new
				{
					PriceSubtotalOld = 4100m,
					PriceTaxOld = 410m,
					CartItemParams = new[]
					{
						new
						{
							ProductPrice = 100m,
							ItemQuantity = 1,
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 2,
						},
					},
					ShippingParams = new[]
					{
						new
						{
							ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_US,
							ProductCountsParams =  new[]
							{
								new
								{
									IndexOfCartProduct = 0,
									ItemQuantity = 1,
								},
								new
								{
									IndexOfCartProduct = 1,
									ItemQuantity = 1,
								},
							},
						},
						new
						{
							ShippingCountryIsoCode =  Constants.COUNTRY_ISO_CODE_US,
							ProductCountsParams = new[]
							{
								new
								{
									IndexOfCartProduct = 1,
									ItemQuantity = 1,
								},
							}
						}
					},
				},
				new
				{
					ProductsExpected = new[]
					{
						new
						{
							ItemPrice = 100m,
							TaxPrice = 10m,
							ProductCount = 1,
							ItemPricePretax = 110m,
							PriceSubtotalAfterDistribution = 100m,
						},
						new
						{
							ItemPrice = 4000m,
							TaxPrice = 400m,
							ProductCount = 2,
							ItemPricePretax = 4400m,
							PriceSubtotalAfterDistribution = 4000m,
						}
					},
					ProductCountsExpected = new[]
					{
						new[]
						{
							new
							{
								PriceSubtotalAfterDistribution = 100m,
							},
							new
							{
								PriceSubtotalAfterDistribution = 2000m,
							}
						},
						new[]
						{
							new
							{
								PriceSubtotalAfterDistribution = 2000m,
							}
						}
					},
					IsPriceChangedExpected = false,
				},
				"海外配送・商品小計変化なし"
			},
			// 一部国内配送、一部海外配送・商品小計変化あり
			new object[]
			{
				new {},
				new
				{
					PriceSubtotalOld = 10000m,
					PriceTaxOld = 1000m,
					CartItemParams = new[]
					{
						new
						{
							ProductPrice = 100m,
							ItemQuantity = 1,
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 2,
						},
					},
					ShippingParams = new[]
					{
						new
						{
							ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP,
							ProductCountsParams =  new[]
							{
								new
								{
									IndexOfCartProduct = 0,
									ItemQuantity = 1,
								},
								new
								{
									IndexOfCartProduct = 1,
									ItemQuantity = 1,
								},
							},
						},
						new
						{
							ShippingCountryIsoCode =  Constants.COUNTRY_ISO_CODE_US,
							ProductCountsParams = new[]
							{
								new
								{
									IndexOfCartProduct = 1,
									ItemQuantity = 1,
								},
							}
						}
					},
				},
				new
				{
					ProductsExpected = new[]
					{
						new
						{
							ItemPrice = 100m,
							TaxPrice = 10m,
							ProductCount = 1,
							ItemPricePretax = 110m,
							PriceSubtotalAfterDistribution = 110m,
						},
						new
						{
							ItemPrice = 4000m,
							TaxPrice = 400m,
							ProductCount = 2,
							ItemPricePretax = 4400m,
							PriceSubtotalAfterDistribution = 4200m,
						}
					},
					ProductCountsExpected = new[]
					{
						new[]
						{
							new
							{
								PriceSubtotalAfterDistribution = 110m,
							},
							new
							{
								PriceSubtotalAfterDistribution = 2200m,
							}
						},
						new[]
						{
							new
							{
								PriceSubtotalAfterDistribution = 2000m,
							}
						}
					},
					IsPriceChangedExpected = true,
				},
				"一部国内配送、一部海外配送・商品小計変化あり"
			},
			// 一部国内配送、一部海外配送・商品小計変化なし
			new object[]
			{
				new {},
				new
				{
					PriceSubtotalOld = 4100m,
					PriceTaxOld = 410m,
					CartItemParams = new[]
					{
						new
						{
							ProductPrice = 100m,
							ItemQuantity = 1,
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 2,
						},
					},
					ShippingParams = new[]
					{
						new
						{
							ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP,
							ProductCountsParams =  new[]
							{
								new
								{
									IndexOfCartProduct = 0,
									ItemQuantity = 1,
								},
								new
								{
									IndexOfCartProduct = 1,
									ItemQuantity = 1,
								},
							},
						},
						new
						{
							ShippingCountryIsoCode =  Constants.COUNTRY_ISO_CODE_US,
							ProductCountsParams = new[]
							{
								new
								{
									IndexOfCartProduct = 1,
									ItemQuantity = 1,
								},
							}
						}
					},
				},
				new
				{
					ProductsExpected = new[]
					{
						new
						{
							ItemPrice = 100m,
							TaxPrice = 10m,
							ProductCount = 1,
							ItemPricePretax = 110m,
							PriceSubtotalAfterDistribution = 110m,
						},
						new
						{
							ItemPrice = 4000m,
							TaxPrice = 400m,
							ProductCount = 2,
							ItemPricePretax = 4400m,
							PriceSubtotalAfterDistribution = 4200m,
						}
					},
					ProductCountsExpected = new[]
					{
						new[]
						{
							new
							{
								PriceSubtotalAfterDistribution = 110m,
							},
							new
							{
								PriceSubtotalAfterDistribution = 2200m,
							}
						},
						new[]
						{
							new
							{
								PriceSubtotalAfterDistribution = 2000m,
							}
						}
					},
					IsPriceChangedExpected = false,
				},
				"一部国内配送、一部海外配送・商品小計変化なし"
			},
		};

		/// <summary>
		/// 送料無料条件金額との差額取得
		/// ・差額 = 「送料無料条件金額」- (「商品小計」 - (「セップロ商品割引」 + 「会員ランク割引」 + 「定期会員割引」 + 「定期回数割引」))
		/// ・差額がマイナスの場合は0が取得されること
		/// </summary>
		[DataTestMethod()]
		[DynamicData("m_tdGetDifferenceToFreeShippingPriceTest")]
		public void GetDifferenceToFreeShippingPriceTest(
			dynamic config,
			dynamic data,
			dynamic expected,
			string msg)
		{
			using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), false))
			using (new TestConfigurator(Member.Of(() => Constants.OPERATIONAL_BASE_ISO_CODE), Constants.COUNTRY_ISO_CODE_JP))
			using (new TestConfigurator(Member.Of(() => Constants.GLOBAL_TRANSACTION_INCLUDED_TAX_FLAG), false))
			using (new TestConfigurator(Member.Of(() => Constants.GIFTORDER_OPTION_ENABLED), true))
			using (new TestConfigurator(Member.Of(() => Constants.MEMBER_RANK_OPTION_ENABLED), false))
			using (new TestConfigurator(Member.Of(() => Constants.TAX_EXCLUDED_FRACTION_ROUNDING),
				TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN))
			{

				var mock = new Mock<IProductTagService>();
				mock.Setup(s => s.GetProductTagSetting()).Returns(new ProductTagSettingModel[0]);
				DomainFacade.Instance.ProductTagService = mock.Object;

				var cart = CartTestHelper.CreateCart();
				// 送料無料条件金額を設定する
				foreach (var shippingFreePrice in (decimal[])data.ShippingFreePriceList)
				{
					var shippingInfo = new ShopShippingModel
					{
						CompanyPostageSettings = new[]
						{
								new ShippingDeliveryPostageModel
								{
									DeliveryCompanyId = "dc1",
									ShippingFreePrice = shippingFreePrice
								}
						},
						ZoneList = new[]
						{
								new ShopShippingZoneModel
								{
									DeliveryCompanyId = "dc1",
									SizeMailShippingPrice = 0
								}
							}
					};
					var cartShipping = new CartShipping(cart)
					{
						DeliveryCompanyId = "dc1"
					};
					cartShipping.SetShippingPrice(shippingInfo);
					cart.Shippings.Add(cartShipping);
				}

				cart.Items.Add(CartTestHelper.CreateCartProduct(data.PriceSubtotal, 1, 10m));
				cart.SetPriceSubtotal(data.PriceSubtotal, data.PriceSubtotalTax);
				cart.SetPriceShipping(data.PriceShipping);
				// 各種割引金額を設定する
				CartTestHelper.CreateCartSetPromotion(
					cart,
					cart.Items,
					1,
					Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_KBN_DISCOUNT_PRICE,
					data.ProductDiscountAmount);
				cart.PriceInfoByTaxRate.Add(
					new CartPriceInfoByTaxRate
					{
						ReturnPriceCorrection = data.PriceCorrection
					});
				cart.PriceRegulation = data.PriceRegulation;
				cart.MemberRankDiscount = data.MemberRankDiscount;
				cart.UseCouponPrice = data.UseCouponPrice;
				cart.UsePointPrice = data.UsePointPrice;
				cart.SetPromotions.ShippingChargeDiscountAmount = data.ShippingChargeDiscountAmount;
				cart.FixedPurchaseDiscount = data.FixedPurchaseDiscount;
				cart.FixedPurchaseMemberDiscountAmount = data.FixedPurchaseMemberDiscountAmount;

				var result = cart.GetDifferenceToFreeShippingPrice();

				// ・差額 = 「送料無料条件金額」- (「商品小計」 - (「セップロ商品割引」 + 「会員ランク割引」 + 「定期会員割引」 + 「定期回数割引」))
				// ・差額がマイナスの場合は0が取得されること
				result.Should().Be((decimal)expected.DifferencePriceExpected, msg + "：小計再計算");
			}
		}

		public static object[] m_tdGetDifferenceToFreeShippingPriceTest = new[]
		{
			// 差額がプラス
			new object[]
			{
				new {},
				new
				{
					ShippingFreePriceList = new []
					{
						1000000000000m,
						1000000000000m
					},
					PriceSubtotal = 100000000000m,
					ProductDiscountAmount = 10000000000m,
					MemberRankDiscount = 1000000000m,
					FixedPurchaseMemberDiscountAmount = 100000000m,
					FixedPurchaseDiscount = 10000000m,
					PriceSubtotalTax = 1000000m,
					PriceShipping = 100000m,
					PriceRegulation = 10000m,
					PriceCorrection = 1000m,
					UseCouponPrice = 100m,
					UsePointPrice = 10m,
					ShippingChargeDiscountAmount = 1m,
				},
				new
				{
					DifferencePriceExpected = 1911110000000m,
				},
				"差額がプラス"
			},
			// 差額がマイナス
			new object[]
			{
				new {},
				new
				{
					ShippingFreePriceList = new []
					{
						1m,
						1m
					},
					PriceSubtotal = 100000000000m,
					ProductDiscountAmount = 10000000000m,
					MemberRankDiscount = 1000000000m,
					FixedPurchaseMemberDiscountAmount = 100000000m,
					FixedPurchaseDiscount = 10000000m,
					PriceSubtotalTax = 1000000m,
					PriceShipping = 100000m,
					PriceRegulation = 10000m,
					PriceCorrection = 1000m,
					UseCouponPrice = 100m,
					UsePointPrice = 10m,
					ShippingChargeDiscountAmount = 1m,
				},
				new
				{
					DifferencePriceExpected = 0m,
				},
				"差額がマイナス"
			},
		};

		/// <summary>
		/// カート商品への割引金額按分計算：配送先一つのみ
		/// ・引数のdiscountPriceを、カート内の商品金額でそれぞれ按分すること。
		/// ・按分後に端数余りが発生した場合、最も価格が高い商品に端数が重み付けされること。
		/// ・引数のisDiscountForCampaignにtrueが指定された場合、対象カート商品のPriceSubtotalAfterDistributionForCampaignにも按分金額を反映すること。
		/// </summary>
		[DataTestMethod()]
		[DynamicData("m_tdProrateDiscountPriceTest_SingleShipping")]
		public void ProrateDiscountPriceTest_SingleShipping(
			dynamic config,
			dynamic data,
			dynamic expected,
			string msg)
		{
			// モックによるドメイン偽装
			var mock = new Mock<IProductTagService>();
			mock.Setup(s => s.GetProductTagSetting()).Returns(new ProductTagSettingModel[0]);
			DomainFacade.Instance.ProductTagService = mock.Object;

			using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), true))
			using (new TestConfigurator(Member.Of(() => Constants.TAX_EXCLUDED_FRACTION_ROUNDING),
				TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN))
			using (new TestConfigurator(Member.Of(() => Constants.MEMBER_RANK_OPTION_ENABLED), false))
			using (new TestConfigurator(Member.Of(() => Constants.GIFTORDER_OPTION_ENABLED), config.GiftOptionEnabled))
			{
				var cart = CartTestHelper.CreateCart();
				cart.Owner = CartTestHelper.CreateCartOwner();
				foreach (var productParam in data.ProductParams)
				{
					var cartProduct = CartTestHelper.CreateCartProduct(
						productParam.ProductPrice,
						productParam.ItemQuantity);
					cart.Items.Add(cartProduct);
				}

				cart.GetShipping().UpdateShippingAddr(cart.Owner, true);
				cart.CalculatePriceSubTotal();

				cart.ProrateDiscountPrice(cart.Items, data.DiscountPrice, data.IsDiscountForCampaign);

				// カート商品への割引金額按分計算：配送先一つのみ
				// ・引数のdiscountPriceを、カート商品金額でそれぞれ按分すること。
				// ・按分後に端数余りが発生した場合、最も価格が高い商品に端数が重み付けされること。
				// ・引数のisDiscountForCampaignにtrueが指定された場合、対象カート商品のPriceSubtotalAfterDistributionForCampaignにも按分金額を反映すること。
				var index = 0;
				foreach (var item in cart.Items)
				{
					var resultExpected = expected.ResultsExpected[index];
					item.PriceSubtotalAfterDistribution.Should().Be(
						(decimal)resultExpected.PriceSubtotalAfterDistribution,
						string.Format("{0}:カート商品・割引按分適用後小計", msg));
					item.PriceSubtotalAfterDistributionForCampaign.Should().Be(
						(decimal)resultExpected.PriceSubtotalAfterDistributionForCampaign,
						string.Format("{0}:カート商品・割引按分適用後小計(キャンペーン適用対象)", msg));
					index++;
				}
			}
		}

		public static object[] m_tdProrateDiscountPriceTest_SingleShipping = new[]
		{
			// キャンペーン適用比較対象割引ではない
			new object[]
			{
				new
				{
					GiftOptionEnabled = false
				},
				new
				{
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 1000m,
							ItemQuantity = 1
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 2
						}
					},
					DiscountPrice = 100m,
					IsDiscountForCampaign = false,
				},
				new
				{
					ResultsExpected = new[]
					{
						new
						{
							PriceSubtotalAfterDistribution = 980m,
							PriceSubtotalAfterDistributionForCampaign = 1000m
						},
						new
						{
							PriceSubtotalAfterDistribution = 3920m,
							PriceSubtotalAfterDistributionForCampaign = 4000m
						}
					},
				},
				"キャンペーン適用比較対象割引ではない"
			},
			// キャンペーン適用比較対象割引
			new object[]
			{
				new
				{
					GiftOptionEnabled = false
				},
				new
				{
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 1000m,
							ItemQuantity = 1
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 2
						}
					},
					DiscountPrice = 100m,
					IsDiscountForCampaign = true,
				},
				new
				{
					ResultsExpected = new[]
					{
						new
						{
							PriceSubtotalAfterDistribution = 980m,
							PriceSubtotalAfterDistributionForCampaign = 980m
						},
						new
						{
							PriceSubtotalAfterDistribution = 3920m,
							PriceSubtotalAfterDistributionForCampaign = 3920m
						}
					},
				},
				"キャンペーン適用比較対象割引"
			},
			// 按分後小数点以下端数金額あり
			new object[]
			{
				new
				{
					GiftOptionEnabled = false
				},
				new
				{
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 1000m,
							ItemQuantity = 1
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 2
						}
					},
					DiscountPrice = 111m,
					IsDiscountForCampaign = false,
				},
				new
				{
					ResultsExpected = new[]
					{
						new
						{
							PriceSubtotalAfterDistribution = 978m,
							PriceSubtotalAfterDistributionForCampaign = 1000m
						},
						new
						{
							PriceSubtotalAfterDistribution = 3911m,
							PriceSubtotalAfterDistributionForCampaign = 4000m
						}
					},
				},
				"按分後小数点以下端数金額あり"
			},
		};

		/// <summary>
		/// カート商品への割引金額按分計算：複数配送先
		/// ・引数のdiscountPriceを、カート商品金額でそれぞれ按分すること。
		/// ・按分後に端数余りが発生した場合、対象カート商品内の最も価格が高い商品に端数が重み付けされること。
		/// ・引数のisDiscountForCampaignにtrueが指定された場合、対象カート商品のPriceSubtotalAfterDistributionForCampaignにも按分金額を反映すること。
		/// </summary>
		[DataTestMethod()]
		[DynamicData("m_tdProrateDiscountPriceTest_MultipleShipping")]
		public void ProrateDiscountPriceTest_MultipleShipping(
			dynamic config,
			dynamic data,
			dynamic expected,
			string msg)
		{
			// モックによるドメイン偽装
			var mock = new Mock<IProductTagService>();
			mock.Setup(s => s.GetProductTagSetting()).Returns(new ProductTagSettingModel[0]);
			DomainFacade.Instance.ProductTagService = mock.Object;

			using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), true))
			using (new TestConfigurator(Member.Of(() => Constants.TAX_EXCLUDED_FRACTION_ROUNDING),
				TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN))
			using (new TestConfigurator(Member.Of(() => Constants.MEMBER_RANK_OPTION_ENABLED), false))
			using (new TestConfigurator(Member.Of(() => Constants.GIFTORDER_OPTION_ENABLED), config.GiftOptionEnabled))
			{
				var cart = CartTestHelper.CreateCart();
				cart.Owner = CartTestHelper.CreateCartOwner();
				cart.Shippings.Clear();
				foreach (var cartItemParam in (dynamic[])data.CartItemParams)
				{
					var cartProduct = CartTestHelper.CreateCartProduct(
						productPrice: cartItemParam.ProductPrice,
						productCount: cartItemParam.ItemQuantity,
						addCartKbn: Constants.AddCartKbn.GiftOrder);
					cart.Items.Add(cartProduct);
				}

				foreach (var shippingParam in (dynamic[])data.ShippingParams)
				{
					var cartShipping = new CartShipping(cart);
					cartShipping.UpdateShippingAddr(cart.Owner, true);
					foreach (var productCountsParam in shippingParam.ProductCountsParams)
					{
						var productCounts = new CartShipping.ProductCount(
							cart.Items[productCountsParam.IndexOfCartProduct],
							productCountsParam.ItemQuantity);
						cartShipping.ProductCounts.Add(productCounts);
					}

					cart.Shippings.Add(cartShipping);
				}

				cart.CalculatePriceSubTotal();

				cart.ProrateDiscountPrice(cart.Items, data.DiscountPrice, data.IsDiscountForCampaign);

				// カート商品への割引金額按分計算：配送先一つのみ
				// ・引数のdiscountPriceを、カート商品金額でそれぞれ按分すること。
				// ・按分後に端数余りが発生した場合、対象カート商品内の最も価格が高い商品に端数が重み付けされること。
				// ・引数のisDiscountForCampaignにtrueが指定された場合、対象カート商品のPriceSubtotalAfterDistributionForCampaignにも按分金額を反映すること。
				var itemIndex = 0;
				foreach (var item in cart.Items)
				{
					var itemResultExpected = expected.ProductsExpected[itemIndex];
					item.PriceSubtotalAfterDistribution.Should().Be(
						(decimal)itemResultExpected.PriceSubtotalAfterDistribution,
						string.Format("{0}:カート商品・割引按分適用後小計", msg));
					item.PriceSubtotalAfterDistributionForCampaign.Should().Be(
						(decimal)itemResultExpected.PriceSubtotalAfterDistributionForCampaign,
						string.Format("{0}:カート商品・割引按分適用後小計(キャンペーン適用対象)", msg));
					itemIndex++;
				}

				var shippingIndex = 0;
				foreach (var shipping in cart.Shippings)
				{
					var pcIndex = 0;
					foreach (var pc in shipping.ProductCounts)
					{
						var productCountsResultExpected = expected.ProductCountsExpected[shippingIndex][pcIndex];
						pc.PriceSubtotalAfterDistribution.Should().Be(
							(decimal)productCountsResultExpected.PriceSubtotalAfterDistribution,
							string.Format("{0}:カート配送先商品情報・割引按分適用後小計", msg));
						pcIndex++;
					}

					shippingIndex++;
				}
			}
		}

		public static object[] m_tdProrateDiscountPriceTest_MultipleShipping = new[]
		{
			// キャンペーン適用比較対象割引ではない
			new object[]
			{
				new
				{
					GiftOptionEnabled = true
				},
				new
				{
					CartItemParams = new[]
					{
						new
						{
							ProductPrice = 1000m,
							ItemQuantity = 1,
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 2,
						},
					},
					ShippingParams = new[]
					{
						new
						{
							ProductCountsParams =  new[]
							{
								new
								{
									IndexOfCartProduct = 0,
									ItemQuantity = 1,
								},
								new
								{
									IndexOfCartProduct = 1,
									ItemQuantity = 1,
								},
							},
						},
						new
						{
							ProductCountsParams = new[]
							{
								new
								{
									IndexOfCartProduct = 1,
									ItemQuantity = 1,
								},
							}
						}
					},
					DiscountPrice = 100m,
					IsDiscountForCampaign = false,
				},
				new
				{
					ProductsExpected = new[]
					{
						new
						{
							PriceSubtotalAfterDistribution = 980m,
							PriceSubtotalAfterDistributionForCampaign = 1000m
						},
						new
						{
							PriceSubtotalAfterDistribution = 3920m,
							PriceSubtotalAfterDistributionForCampaign = 4000m
						}
					},
					ProductCountsExpected = new[]
					{
						new[]
						{
							new
							{
								PriceSubtotalAfterDistribution = 980m,
							},
							new
							{
								PriceSubtotalAfterDistribution = 1960m,
							}
						},
						new[]
						{
							new
							{
								PriceSubtotalAfterDistribution = 1960m,
							}
						}
					},
				},
				"キャンペーン適用比較対象割引ではない"
			},
			// キャンペーン適用比較対象割引
			new object[]
			{
				new
				{
					GiftOptionEnabled = true
				},
				new
				{
					CartItemParams = new[]
					{
						new
						{
							ProductPrice = 1000m,
							ItemQuantity = 1,
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 2,
						},
					},
					ShippingParams = new[]
					{
						new
						{
							ProductCountsParams =  new[]
							{
								new
								{
									IndexOfCartProduct = 0,
									ItemQuantity = 1,
								},
								new
								{
									IndexOfCartProduct = 1,
									ItemQuantity = 1,
								},
							},
						},
						new
						{
							ProductCountsParams = new[]
							{
								new
								{
									IndexOfCartProduct = 1,
									ItemQuantity = 1,
								},
							}
						}
					},
					DiscountPrice = 100m,
					IsDiscountForCampaign = true,
				},
				new
				{
					ProductsExpected = new[]
					{
						new
						{
							PriceSubtotalAfterDistribution = 980m,
							PriceSubtotalAfterDistributionForCampaign = 980m,
						},
						new
						{
							PriceSubtotalAfterDistribution = 3920m,
							PriceSubtotalAfterDistributionForCampaign = 3920m,
						}
					},
					ProductCountsExpected = new[]
					{
						new[]
						{
							new
							{
								PriceSubtotalAfterDistribution = 980m,
							},
							new
							{
								PriceSubtotalAfterDistribution = 1960m,
							}
						},
						new[]
						{
							new
							{
								PriceSubtotalAfterDistribution = 1960m,
							}
						}
					},
				},
				"キャンペーン適用比較対象割引"
			},
			// 按分後小数点以下端数金額あり
			new object[]
			{

				new
				{
					GiftOptionEnabled = true
				},
				new
				{
					CartItemParams = new[]
					{
						new
						{
							ProductPrice = 1000m,
							ItemQuantity = 1,
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 2,
						},
					},
					ShippingParams = new[]
					{
						new
						{
							ProductCountsParams =  new[]
							{
								new
								{
									IndexOfCartProduct = 0,
									ItemQuantity = 1,
								},
								new
								{
									IndexOfCartProduct = 1,
									ItemQuantity = 1,
								},
							},
						},
						new
						{
							ProductCountsParams = new[]
							{
								new
								{
									IndexOfCartProduct = 1,
									ItemQuantity = 1,
								},
							}
						}
					},
					DiscountPrice = 111m,
					IsDiscountForCampaign = false,
				},
				new
				{
					ProductsExpected = new[]
					{
						new
						{
							PriceSubtotalAfterDistribution = 978m,
							PriceSubtotalAfterDistributionForCampaign = 1000m
						},
						new
						{


							PriceSubtotalAfterDistribution = 3911m,
							PriceSubtotalAfterDistributionForCampaign = 4000m
						}
					},
					ProductCountsExpected = new[]
					{
						new[]
						{
							new
							{
								PriceSubtotalAfterDistribution = 978m,
							},
							new
							{
								PriceSubtotalAfterDistribution = 1955m,
							}
						},
						new[]
						{
							new
							{
								PriceSubtotalAfterDistribution = 1956m,
							}
						}
					},
				},
				"按分後小数点以下端数金額あり"
			},
		};

		/// <summary>
		/// 会員ランク割引額の按分計算
		/// ・会員ランクOPがONの場合、会員ランク割引フラグがTRUEの商品に対して割引金額の按分計算を実施すること
		/// ・商品ごとに按分した割引額を、税込小計価格からマイナスした値が「PriceSubtotalAfterDistribution」「PriceSubtotalAfterDistributionForCampaign」に設定されること
		/// ・カートに会員ランクIDが設定されていた場合、IDから会員ランクマスタ情報を取得して割引金額を設定すること
		/// ・カートに会員ランクIDが設定されておらずユーザーIDが存在した場合、ユーザー情報から会員ランクマスタ情報を取得して割引金額を設定すること
		/// </summary>
		[DataTestMethod()]
		[DynamicData("m_tdCalculateMemberRankDiscountPriceTest")]
		public void CalculateMemberRankDiscountPriceTest(
			dynamic config,
			dynamic data,
			dynamic expected,
			string msg)
		{
			// モックによるドメイン偽装
			var productTagMock = new Mock<IProductTagService>();
			productTagMock.Setup(s => s.GetProductTagSetting()).Returns(new ProductTagSettingModel[0]);
			DomainFacade.Instance.ProductTagService = productTagMock.Object;

			using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), true))
			using (new TestConfigurator(Member.Of(() => Constants.GIFTORDER_OPTION_ENABLED), false))
			using (new TestConfigurator(Member.Of(() => Constants.TAX_EXCLUDED_FRACTION_ROUNDING),
				TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN))
			using (new TestConfigurator(Member.Of(() => Constants.MEMBER_RANK_OPTION_ENABLED), config.MemberRankOptionEnabled))
			{
				var cacheData = new[]
				{
						new MemberRankModel
						{
							MemberRankId = data.MemberRankId,
							OrderDiscountType = Constants.FLG_MEMBERRANK_ORDER_DISCOUNT_TYPE_FIXED,
							OrderDiscountValue = data.DiscountValue,
							OrderDiscountThresholdPrice = data.OrderDiscountThresholdPrice,
						}
					};
				using (new MemberRankDataCacheConfigurator(cacheData))
				{
					var userServiceMock = new Mock<IUserService>();
					userServiceMock.Setup(s => s.GetMemberRankId(It.IsAny<string>(), It.IsAny<SqlAccessor>()))
						.Returns((string)data.MemberRankId);
					DomainFacade.Instance.UserService = userServiceMock.Object;

					var cart = (CartObject)CartTestHelper.CreateCart(data.UserId, data.MemberRankId);
					cart.Owner = CartTestHelper.CreateCartOwner();
					foreach (var productParam in data.ProductParams)
					{
						var cartProduct = (CartProduct)CartTestHelper.CreateCartProduct(
							productPrice: productParam.ProductPrice,
							productCount: productParam.ItemQuantity,
							memberRankDiscountFlg: productParam.MemberRankDiscountFlg);
						cart.Items.Add(cartProduct);
					}

					cart.GetShipping().UpdateShippingAddr(cart.Owner, true);
					cart.CalculatePriceSubTotal();

					new PrivateObject(cart).Invoke("CalculateMemberRankDiscountPrice");

					// 会員ランク割引額の按分計算
					// ・会員ランクOPがONの場合、会員ランク割引フラグがTRUEの商品に対して割引金額の按分計算を実施すること
					// ・商品ごとに按分した割引額を、税込小計価格からマイナスした値が「PriceSubtotalAfterDistribution」「PriceSubtotalAfterDistributionForCampaign」に設定されること
					// ・カートに会員ランクIDが設定されていた場合、IDから会員ランクマスタ情報を取得して割引金額を設定すること
					// ・カートに会員ランクIDが設定されておらずユーザーIDが存在した場合、ユーザー情報から会員ランクマスタ情報を取得して割引金額を設定すること
					var index = 0;
					foreach (var item in cart.Items)
					{
						var resultExpected = expected.ResultsExpected[index];
						item.PriceSubtotalAfterDistribution.Should().Be(
							(decimal)resultExpected.PriceSubtotalAfterDistribution,
							string.Format("{0}:カート商品・割引按分適用後小計", msg));
						item.PriceSubtotalAfterDistributionForCampaign.Should().Be(
							(decimal)resultExpected.PriceSubtotalAfterDistributionForCampaign,
							string.Format("{0}:カート商品・割引按分適用後小計(キャンペーン適用対象)", msg));
						index++;
					}
				}
			}
		}

		public static object[] m_tdCalculateMemberRankDiscountPriceTest = new[]
		{
			// 会員ランクオプション；OFF
			new object[]
			{
				new
				{
					MemberRankOptionEnabled = false
				},
				new
				{
					UserId = "user001",
					MemberRankId = "rank001",
					DiscountValue = 0m,
					OrderDiscountThresholdPrice = 0m,
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 1000m,
							ItemQuantity = 1,
							MemberRankDiscountFlg = true,
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 1,
							MemberRankDiscountFlg = true,
						},
						// 会員ランク割引対象外商品
						new
						{
							ProductPrice = 3000m,
							ItemQuantity = 1,
							MemberRankDiscountFlg = false,
						}
					},
				},
				new
				{
					ResultsExpected = new[]
					{
						new
						{
							PriceSubtotalAfterDistribution = 1000m,
							PriceSubtotalAfterDistributionForCampaign = 1000m
						},
						new
						{
							PriceSubtotalAfterDistribution = 2000m,
							PriceSubtotalAfterDistributionForCampaign = 2000m
						},
						// 会員ランク割引対象外商品
						new
						{
							PriceSubtotalAfterDistribution = 3000m,
							PriceSubtotalAfterDistributionForCampaign = 3000m
						}
					},
				},
				"会員ランクオプション；OFF"
			},
			// ユーザーIDが空かつ、会員ランクIDが空
			new object[]
			{
				new
				{
					MemberRankOptionEnabled = true
				},
				new
				{
					UserId = "",
					MemberRankId = "",
					DiscountValue = 100m,
					OrderDiscountThresholdPrice = 100m,
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 1000m,
							ItemQuantity = 1,
							MemberRankDiscountFlg = true,
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 1,
							MemberRankDiscountFlg = true,
						},
						// 会員ランク割引対象外商品
						new
						{
							ProductPrice = 3000m,
							ItemQuantity = 1,
							MemberRankDiscountFlg = false,
						}
					},
				},
				new
				{
					ResultsExpected = new[]
					{
						new
						{
							PriceSubtotalAfterDistribution = 1000m,
							PriceSubtotalAfterDistributionForCampaign = 1000m
						},
						new
						{
							PriceSubtotalAfterDistribution = 2000m,
							PriceSubtotalAfterDistributionForCampaign = 2000m
						},
						// 会員ランク割引対象外商品
						new
						{
							PriceSubtotalAfterDistribution = 3000m,
							PriceSubtotalAfterDistributionForCampaign = 3000m
						}
					},
				},
				"ユーザーIDが空かつ、会員ランクIDが空"
			},
			// ユーザーIDが空
			new object[]
			{
				new
				{
					MemberRankOptionEnabled = true
				},
				new
				{
					UserId = "",
					MemberRankId = "rank001",
					DiscountValue = 100m,
					OrderDiscountThresholdPrice = 100m,
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 1000m,
							ItemQuantity = 1,
							MemberRankDiscountFlg = true,
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 1,
							MemberRankDiscountFlg = true,
						},
						// 会員ランク割引対象外商品
						new
						{
							ProductPrice = 3000m,
							ItemQuantity = 1,
							MemberRankDiscountFlg = false,
						}
					},
				},
				new
				{
					ResultsExpected = new[]
					{
						// 割引金額　= 100(割引総額) * 1000(商品価格) / 3000(割引対象商品小計の総額) = 33.3 →(端数切捨て) 33円
						new
						{
							PriceSubtotalAfterDistribution = 967m,
							PriceSubtotalAfterDistributionForCampaign = 967m
						},
						// 割引金額　= 100(割引総額) * 2000(商品価格) / 3000(割引対象商品小計の総額) = 66.6 →(端数切捨て) 66 + 1(端数(100 - 33 - 66)の重み付け) = 67円
						new
						{
							PriceSubtotalAfterDistribution = 1933m,
							PriceSubtotalAfterDistributionForCampaign = 1933m
						},
						// 会員ランク割引対象外商品
						new
						{
							PriceSubtotalAfterDistribution = 3000m,
							PriceSubtotalAfterDistributionForCampaign = 3000m
						}
					},
				},
				"ユーザーIDが空"
			},
			// 会員ランクIDが空
			new object[]
			{
				new
				{
					MemberRankOptionEnabled = true
				},
				new
				{
					UserId = "user001",
					MemberRankId = "",
					DiscountValue = 100m,
					OrderDiscountThresholdPrice = 100m,
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 1000m,
							ItemQuantity = 1,
							MemberRankDiscountFlg = true,
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 1,
							MemberRankDiscountFlg = true,
						},
						// 会員ランク割引対象外商品
						new
						{
							ProductPrice = 3000m,
							ItemQuantity = 1,
							MemberRankDiscountFlg = false,
						}
					},
				},
				new
				{
					ResultsExpected = new[]
					{
						// 割引金額　= 100(割引総額) * 1000(商品価格) / 3000(割引対象商品小計の総額) = 33.3 →(端数切捨て) 33円
						new
						{
							PriceSubtotalAfterDistribution = 967m,
							PriceSubtotalAfterDistributionForCampaign = 967m
						},
						// 割引金額　= 100(割引総額) * 2000(商品価格) / 3000(割引対象商品小計の総額) = 66.6 →(端数切捨て) 66 + 1(端数(100 - 33 - 66)の重み付け) = 67円
						new
						{
							PriceSubtotalAfterDistribution = 1933m,
							PriceSubtotalAfterDistributionForCampaign = 1933m
						},
						// 会員ランク割引対象外商品
						new
						{
							PriceSubtotalAfterDistribution = 3000m,
							PriceSubtotalAfterDistributionForCampaign = 3000m
						}
					},
				},
				"会員ランクIDが空"
			},
			// ユーザーID、会員ランクIDが存在
			new object[]
			{
				new
				{
					MemberRankOptionEnabled = true
				},
				new
				{
					UserId = "user001",
					MemberRankId = "rank001",
					DiscountValue = 100m,
					OrderDiscountThresholdPrice = 100m,
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 1000m,
							ItemQuantity = 1,
							MemberRankDiscountFlg = true,
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 1,
							MemberRankDiscountFlg = true,
						},
						// 会員ランク割引対象外商品
						new
						{
							ProductPrice = 3000m,
							ItemQuantity = 1,
							MemberRankDiscountFlg = false,
						}
					},
				},
				new
				{
					ResultsExpected = new[]
					{
						// 割引金額　= 100(割引総額) * 1000(商品価格) / 3000(割引対象商品小計の総額) = 33.3 →(端数切捨て) 33円
						new
						{
							PriceSubtotalAfterDistribution = 967m,
							PriceSubtotalAfterDistributionForCampaign = 967m
						},
						// 割引金額　= 100(割引総額) * 2000(商品価格) / 3000(割引対象商品小計の総額) = 66.6 →(端数切捨て) 66 + 1(端数(100 - 33 - 66)の重み付け) = 67円
						new
						{
							PriceSubtotalAfterDistribution = 1933m,
							PriceSubtotalAfterDistributionForCampaign = 1933m
						},
						// 会員ランク割引対象外商品
						new
						{
							PriceSubtotalAfterDistribution = 3000m,
							PriceSubtotalAfterDistributionForCampaign = 3000m
						}
					},
				},
				"ユーザーID、会員ランクIDが存在"
			},
		};

		/// <summary>
		/// クーポン割引額の按分計算
		/// ・クーポンOPがONの場合、クーポン対象の商品に対して割引金額の按分計算を実施すること
		/// ・商品価格割引クーポン：商品ごとに按分した割引額を、税込小計価格からマイナスした値が「PriceSubtotalAfterDistribution」に設定されること
		/// ・配送料割引クーポン　：配送料金分が「ShippingPriceDiscountAmount」に設定されること
		/// ・商品割引クーポンの割引額が対象商品の商品価格合計を超過した場合、割引額は対象商品の商品価格合計と同額になること
		/// </summary>
		[DataTestMethod()]
		[DynamicData("m_tdCalculateCouponFamilyTest")]
		public void CalculateCouponFamilyTest(
			dynamic config,
			dynamic data,
			dynamic expected,
			string msg)
		{
			// モックによるドメイン偽装
			var productTagMock = new Mock<IProductTagService>();
			productTagMock.Setup(s => s.GetProductTagSetting()).Returns(new ProductTagSettingModel[0]);
			DomainFacade.Instance.ProductTagService = productTagMock.Object;

			using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), true))
			using (new TestConfigurator(Member.Of(() => Constants.GIFTORDER_OPTION_ENABLED), false))
			using (new TestConfigurator(Member.Of(() => Constants.TAX_EXCLUDED_FRACTION_ROUNDING),
				TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN))
			using (new TestConfigurator(Member.Of(() => Constants.MEMBER_RANK_OPTION_ENABLED), false))
			using (new TestConfigurator(Member.Of(() => Constants.W2MP_COUPON_OPTION_ENABLED), config.CouponOptionEnabled))
			{
				var dateTimeWrapperMock = new Mock<DateTimeWrapper>();
				dateTimeWrapperMock.Setup(s => s.Now).Returns((DateTime)data.DateTimeNow);
				DateTimeWrapper.Instance = dateTimeWrapperMock.Object;

				var couponServiceMock = new Mock<ICouponService>();
				couponServiceMock.Setup(s => s.GetAllUserCouponsFromCouponId(
					It.IsAny<string>(),
					It.IsAny<string>(),
					It.IsAny<string>(),
					It.IsAny<int>())).Returns((UserCouponDetailInfo[])data.UserCouponDetailInfo);
				DomainFacade.Instance.CouponService = couponServiceMock.Object;

				var cart = (CartObject)CartTestHelper.CreateCart();
				cart.Owner = CartTestHelper.CreateCartOwner();
				foreach (var productParam in data.ProductParams)
				{
					var cartProduct = (CartProduct)CartTestHelper.CreateCartProduct(
						productPrice: productParam.ProductPrice,
						productCount: productParam.ItemQuantity,
						memberRankDiscountFlg: productParam.MemberRankDiscountFlg);
					cartProduct.IconFlg[0] = productParam.ProductIconFlg1;
					cartProduct.IconTermEnd[0] = productParam.ProductIconTerm1;
					cart.Items.Add(cartProduct);
				}

				cart.GetShipping().UpdateShippingAddr(cart.Owner, true);
				cart.SetPriceShipping(data.ShippingPrice);
				cart.CalculatePriceSubTotal();
				cart.Coupon = data.CartCoupon;

				new PrivateObject(cart).Invoke("CalculateCouponFamily");

				// クーポン割引額の按分計算
				// ・クーポンOPがONの場合、クーポン対象の商品に対して割引金額の按分計算を実施すること
				// ・商品価格割引クーポン：商品ごとに按分した割引額を、税込小計価格からマイナスした値が「PriceSubtotalAfterDistribution」に設定されること
				// ・配送料割引クーポン　：配送料金分が「ShippingPriceDiscountAmount」に設定されること
				// ・商品割引クーポンの割引額が対象商品の商品価格合計を超過した場合、割引額は対象商品の商品価格合計と同額になること
				var index = 0;
				foreach (var item in cart.Items)
				{
					var resultExpected = expected.ResultsExpected[index];
					item.PriceSubtotalAfterDistribution.Should().Be(
						(decimal)resultExpected.PriceSubtotalAfterDistribution,
						string.Format("{0}:カート商品・割引按分適用後小計", msg));
					item.PriceSubtotalAfterDistributionForCampaign.Should().Be(
						(decimal)resultExpected.PriceSubtotalAfterDistributionForCampaign,
						string.Format("{0}:カート商品・割引按分適用後小計(キャンペーン適用対象)", msg));
					index++;
				}

				cart.UseMaxCouponPrice.Should().Be(
					(decimal)expected.UseMaxCouponPrice,
					string.Format("{0}:クーポン割引額（最大）", msg));
				cart.UseCouponPrice.Should().Be(
					(decimal)expected.UseCouponPrice,
					string.Format("{0}:クーポン割引額", msg));
				cart.ShippingPriceDiscountAmount.Should().Be(
					(decimal)expected.ShippingPriceDiscountAmount,
					string.Format("{0}:配送料割引額", msg));
			}
		}

		public static object[] m_tdCalculateCouponFamilyTest = new[]
		{
			// クーポンオプション；OFF
			new object[]
			{
				new
				{
					CouponOptionEnabled = false
				},
				new
				{
					DateTimeNow = DateTime.Parse("2020/01/01"),
					ShippingPrice = 4000m,
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 1000m,
							ItemQuantity = 1,
							ProductIconFlg1 = Constants.FLG_PRODUCT_ICON_ON,
							ProductIconTerm1 = DateTime.Parse("2021/01/01"),
							MemberRankDiscountFlg = true,
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 1,
							ProductIconFlg1 = Constants.FLG_PRODUCT_ICON_ON,
							ProductIconTerm1 = DateTime.Parse("2021/01/01"),
							MemberRankDiscountFlg = true,
						},
						// クーポン割引対象外商品
						new
						{
							ProductPrice = 3000m,
							ItemQuantity = 1,
							ProductIconFlg1 = Constants.FLG_PRODUCT_ICON_OFF,
							ProductIconTerm1 = DateTime.Parse("2021/01/01"),
							MemberRankDiscountFlg = false,
						}
					},
					UserCouponDetailInfo = new[]
					{
						new UserCouponDetailInfo
						{
							DiscountPrice = 100m,
							CouponType = Constants.FLG_COUPONCOUPON_TYPE_ALL_UNLIMIT,
							PublishDateBgn = DateTime.Now,
							PublishDateEnd = DateTime.Now,
							DateCreated = DateTime.Now,
							DateChanged = DateTime.Now,
							ExceptionalIcon1 = Constants.FLG_COUPON_EXCEPTIONAL_ICON1,
							ExceptionalIcon2 = 0,
							ExceptionalIcon3 = 0,
							ExceptionalIcon4 = 0,
							ExceptionalIcon5 = 0,
							ExceptionalIcon6 = 0,
							ExceptionalIcon7 = 0,
							ExceptionalIcon8 = 0,
							ExceptionalIcon9 = 0,
							ExceptionalIcon10 = 0,
							ProductKbn = Constants.FLG_COUPON_PRODUCT_KBN_UNTARGET,
							ExceptionalProduct = "test",
						}
					},
					// 商品アイコン1がONの商品を対象とするクーポン設定
					CartCoupon = new CartCoupon(
						new UserCouponDetailInfo
						{
							DiscountPrice = 100m,
							CouponType = Constants.FLG_COUPONCOUPON_TYPE_ALL_UNLIMIT,
							PublishDateBgn = DateTime.Now,
							PublishDateEnd = DateTime.Now,
							DateCreated = DateTime.Now,
							DateChanged = DateTime.Now,
							ExceptionalIcon1 = Constants.FLG_COUPON_EXCEPTIONAL_ICON1,
							ExceptionalIcon2 = 0,
							ExceptionalIcon3 = 0,
							ExceptionalIcon4 = 0,
							ExceptionalIcon5 = 0,
							ExceptionalIcon6 = 0,
							ExceptionalIcon7 = 0,
							ExceptionalIcon8 = 0,
							ExceptionalIcon9 = 0,
							ExceptionalIcon10 = 0,
							ProductKbn = Constants.FLG_COUPON_PRODUCT_KBN_UNTARGET,
							ExceptionalProduct = "test",
						}),
				},
				new
				{
					UseMaxCouponPrice = 0m,
					UseCouponPrice = 0m,
					ShippingPriceDiscountAmount = 0m,
					ResultsExpected = new[]
					{
						new
						{
							PriceSubtotalAfterDistribution = 1000m,
							PriceSubtotalAfterDistributionForCampaign = 1000m
						},
						new
						{
							PriceSubtotalAfterDistribution = 2000m,
							PriceSubtotalAfterDistributionForCampaign = 2000m
						},
						// クーポン割引対象外商品
						new
						{
							PriceSubtotalAfterDistribution = 3000m,
							PriceSubtotalAfterDistributionForCampaign = 3000m
						}
					},
				},
				"クーポンオプション；OFF"
			},
			// クーポン情報がNULL
			new object[]
			{
				new
				{
					CouponOptionEnabled = true
				},
				new
				{
					DateTimeNow = DateTime.Parse("2020/01/01"),
					ShippingPrice = 4000m,
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 1000m,
							ItemQuantity = 1,
							ProductIconFlg1 = Constants.FLG_PRODUCT_ICON_ON,
							ProductIconTerm1 = DateTime.Parse("2021/01/01"),
							MemberRankDiscountFlg = true,
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 1,
							ProductIconFlg1 = Constants.FLG_PRODUCT_ICON_ON,
							ProductIconTerm1 = DateTime.Parse("2021/01/01"),
							MemberRankDiscountFlg = true,
						},
						// クーポン割引対象外商品
						new
						{
							ProductPrice = 3000m,
							ItemQuantity = 1,
							ProductIconFlg1 = Constants.FLG_PRODUCT_ICON_OFF,
							ProductIconTerm1 = DateTime.Parse("2021/01/01"),
							MemberRankDiscountFlg = false,
						}
					},
					UserCouponDetailInfo = new UserCouponDetailInfo[0],
					CartCoupon = (CartCoupon)null,
				},
				new
				{
					UseMaxCouponPrice = 0m,
					UseCouponPrice = 0m,
					ShippingPriceDiscountAmount = 0m,
					ResultsExpected = new[]
					{
						new
						{
							PriceSubtotalAfterDistribution = 1000m,
							PriceSubtotalAfterDistributionForCampaign = 1000m
						},
						new
						{
							PriceSubtotalAfterDistribution = 2000m,
							PriceSubtotalAfterDistributionForCampaign = 2000m
						},
						// クーポン割引対象外商品
						new
						{
							PriceSubtotalAfterDistribution = 3000m,
							PriceSubtotalAfterDistributionForCampaign = 3000m
						}
					},
				},
				"クーポン情報がNULL"
			},
			// ユーザークーポン情報が存在しない
			new object[]
			{
				new
				{
					CouponOptionEnabled = true
				},
				new
				{
					DateTimeNow = DateTime.Parse("2020/01/01"),
					ShippingPrice = 4000m,
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 1000m,
							ItemQuantity = 1,
							ProductIconFlg1 = Constants.FLG_PRODUCT_ICON_ON,
							ProductIconTerm1 = DateTime.Parse("2021/01/01"),
							MemberRankDiscountFlg = true,
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 1,
							ProductIconFlg1 = Constants.FLG_PRODUCT_ICON_ON,
							ProductIconTerm1 = DateTime.Parse("2021/01/01"),
							MemberRankDiscountFlg = true,
						},
						// クーポン割引対象外商品
						new
						{
							ProductPrice = 3000m,
							ItemQuantity = 1,
							ProductIconFlg1 = Constants.FLG_PRODUCT_ICON_OFF,
							ProductIconTerm1 = DateTime.Parse("2021/01/01"),
							MemberRankDiscountFlg = false,
						}
					},
					UserCouponDetailInfo = new UserCouponDetailInfo[0],
					// 商品アイコン1がONの商品を対象とするクーポン設定
					CartCoupon = new CartCoupon(
						new UserCouponDetailInfo
						{
							DiscountPrice = 100m,
							CouponType = Constants.FLG_COUPONCOUPON_TYPE_ALL_UNLIMIT,
							PublishDateBgn = DateTime.Now,
							PublishDateEnd = DateTime.Now,
							DateCreated = DateTime.Now,
							DateChanged = DateTime.Now,
							ExceptionalIcon1 = Constants.FLG_COUPON_EXCEPTIONAL_ICON1,
							ExceptionalIcon2 = 0,
							ExceptionalIcon3 = 0,
							ExceptionalIcon4 = 0,
							ExceptionalIcon5 = 0,
							ExceptionalIcon6 = 0,
							ExceptionalIcon7 = 0,
							ExceptionalIcon8 = 0,
							ExceptionalIcon9 = 0,
							ExceptionalIcon10 = 0,
							ProductKbn = Constants.FLG_COUPON_PRODUCT_KBN_UNTARGET,
							ExceptionalProduct = "test",
						}),
				},
				new
				{
					UseMaxCouponPrice = 100m,
					UseCouponPrice = 0m,
					ShippingPriceDiscountAmount = 0m,
					ResultsExpected = new[]
					{
						new
						{
							PriceSubtotalAfterDistribution = 1000m,
							PriceSubtotalAfterDistributionForCampaign = 1000m
						},
						new
						{
							PriceSubtotalAfterDistribution = 2000m,
							PriceSubtotalAfterDistributionForCampaign = 2000m
						},
						// クーポン割引対象外商品
						new
						{
							PriceSubtotalAfterDistribution = 3000m,
							PriceSubtotalAfterDistributionForCampaign = 3000m
						}
					},
				},
				"ユーザークーポン情報が存在しない"
			},
			// 商品割引金額指定：割引金額 < 対象商品価格小計
			new object[]
			{
				new
				{
					CouponOptionEnabled = true
				},
				new
				{
					DateTimeNow = DateTime.Parse("2020/01/01"),
					ShippingPrice = 4000m,
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 1000m,
							ItemQuantity = 1,
							ProductIconFlg1 = Constants.FLG_PRODUCT_ICON_ON,
							ProductIconTerm1 = DateTime.Parse("2021/01/01"),
							MemberRankDiscountFlg = true,
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 1,
							ProductIconFlg1 = Constants.FLG_PRODUCT_ICON_ON,
							ProductIconTerm1 = DateTime.Parse("2021/01/01"),
							MemberRankDiscountFlg = true,
						},
						// クーポン割引対象外商品
						new
						{
							ProductPrice = 3000m,
							ItemQuantity = 1,
							ProductIconFlg1 = Constants.FLG_PRODUCT_ICON_OFF,
							ProductIconTerm1 = DateTime.Parse("2021/01/01"),
							MemberRankDiscountFlg = false,
						}
					},
					UserCouponDetailInfo = new[]
					{
						new UserCouponDetailInfo
						{
							DiscountPrice = 100m,
							CouponType = Constants.FLG_COUPONCOUPON_TYPE_ALL_UNLIMIT,
							PublishDateBgn = DateTime.Now,
							PublishDateEnd = DateTime.Now,
							DateCreated = DateTime.Now,
							DateChanged = DateTime.Now,
							ExceptionalIcon1 = Constants.FLG_COUPON_EXCEPTIONAL_ICON1,
							ExceptionalIcon2 = 0,
							ExceptionalIcon3 = 0,
							ExceptionalIcon4 = 0,
							ExceptionalIcon5 = 0,
							ExceptionalIcon6 = 0,
							ExceptionalIcon7 = 0,
							ExceptionalIcon8 = 0,
							ExceptionalIcon9 = 0,
							ExceptionalIcon10 = 0,
							ProductKbn = Constants.FLG_COUPON_PRODUCT_KBN_UNTARGET,
							ExceptionalProduct = "test",
						}
					},
					// 商品アイコン1がONの商品を対象とするクーポン設定
					CartCoupon = new CartCoupon(
						new UserCouponDetailInfo
						{
							DiscountPrice = 100m,
							CouponType = Constants.FLG_COUPONCOUPON_TYPE_ALL_UNLIMIT,
							PublishDateBgn = DateTime.Now,
							PublishDateEnd = DateTime.Now,
							DateCreated = DateTime.Now,
							DateChanged = DateTime.Now,
							ExceptionalIcon1 = Constants.FLG_COUPON_EXCEPTIONAL_ICON1,
							ExceptionalIcon2 = 0,
							ExceptionalIcon3 = 0,
							ExceptionalIcon4 = 0,
							ExceptionalIcon5 = 0,
							ExceptionalIcon6 = 0,
							ExceptionalIcon7 = 0,
							ExceptionalIcon8 = 0,
							ExceptionalIcon9 = 0,
							ExceptionalIcon10 = 0,
							ProductKbn = Constants.FLG_COUPON_PRODUCT_KBN_UNTARGET,
							ExceptionalProduct = "test",
						}),
				},
				new
				{
					UseMaxCouponPrice = 100m,
					UseCouponPrice = 100m,
					ShippingPriceDiscountAmount = 0m,
					ResultsExpected = new[]
					{
						// 割引金額　= 100(割引総額) * 1000(商品価格) / 3000(割引対象商品小計の総額) = 33.3 →(端数切捨て) 33円
						new
						{
							PriceSubtotalAfterDistribution = 967m,
							PriceSubtotalAfterDistributionForCampaign = 1000m
						},
						// 割引金額　= 100(割引総額) * 2000(商品価格) / 3000(割引対象商品小計の総額) = 66.6 →(端数切捨て) 66 + 1(端数(100 - 33 - 66)の重み付け) = 67円
						new
						{
							PriceSubtotalAfterDistribution = 1933m,
							PriceSubtotalAfterDistributionForCampaign = 2000m
						},
						new
							// クーポン割引対象外商品
						{
							PriceSubtotalAfterDistribution = 3000m,
							PriceSubtotalAfterDistributionForCampaign = 3000m
						}
					},
				},
				"商品割引金額指定：割引金額 < 対象商品価格小計"
			},
			// 商品割引金額指定：割引金額 > 対象商品価格小計
			new object[]
			{
				new
				{
					CouponOptionEnabled = true
				},
				new
				{
					DateTimeNow = DateTime.Parse("2020/01/01"),
					ShippingPrice = 4000m,
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 1000m,
							ItemQuantity = 1,
							ProductIconFlg1 = Constants.FLG_PRODUCT_ICON_ON,
							ProductIconTerm1 = DateTime.Parse("2021/01/01"),
							MemberRankDiscountFlg = true,
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 1,
							ProductIconFlg1 = Constants.FLG_PRODUCT_ICON_ON,
							ProductIconTerm1 = DateTime.Parse("2021/01/01"),
							MemberRankDiscountFlg = true,
						},
						// クーポン割引対象外商品
						new
						{
							ProductPrice = 3000m,
							ItemQuantity = 1,
							ProductIconFlg1 = Constants.FLG_PRODUCT_ICON_OFF,
							ProductIconTerm1 = DateTime.Parse("2021/01/01"),
							MemberRankDiscountFlg = false,
						}
					},
					UserCouponDetailInfo = new[]
					{
						new UserCouponDetailInfo
						{
							DiscountPrice = 100m,
							CouponType = Constants.FLG_COUPONCOUPON_TYPE_ALL_UNLIMIT,
							PublishDateBgn = DateTime.Now,
							PublishDateEnd = DateTime.Now,
							DateCreated = DateTime.Now,
							DateChanged = DateTime.Now,
							ExceptionalIcon1 = Constants.FLG_COUPON_EXCEPTIONAL_ICON1,
							ExceptionalIcon2 = 0,
							ExceptionalIcon3 = 0,
							ExceptionalIcon4 = 0,
							ExceptionalIcon5 = 0,
							ExceptionalIcon6 = 0,
							ExceptionalIcon7 = 0,
							ExceptionalIcon8 = 0,
							ExceptionalIcon9 = 0,
							ExceptionalIcon10 = 0,
							ProductKbn = Constants.FLG_COUPON_PRODUCT_KBN_UNTARGET,
							ExceptionalProduct = "test",
						}
					},
					// 商品アイコン1がONの商品を対象とするクーポン設定
					CartCoupon = new CartCoupon(
						new UserCouponDetailInfo
						{
							DiscountPrice = 5000m,
							CouponType = Constants.FLG_COUPONCOUPON_TYPE_ALL_UNLIMIT,
							PublishDateBgn = DateTime.Now,
							PublishDateEnd = DateTime.Now,
							DateCreated = DateTime.Now,
							DateChanged = DateTime.Now,
							ExceptionalIcon1 = Constants.FLG_COUPON_EXCEPTIONAL_ICON1,
							ExceptionalIcon2 = 0,
							ExceptionalIcon3 = 0,
							ExceptionalIcon4 = 0,
							ExceptionalIcon5 = 0,
							ExceptionalIcon6 = 0,
							ExceptionalIcon7 = 0,
							ExceptionalIcon8 = 0,
							ExceptionalIcon9 = 0,
							ExceptionalIcon10 = 0,
							ProductKbn = Constants.FLG_COUPON_PRODUCT_KBN_UNTARGET,
							ExceptionalProduct = "test",
						}),
				},
				new
				{
					UseMaxCouponPrice = 5000m,
					UseCouponPrice = 3000m,
					ShippingPriceDiscountAmount = 0m,
					ResultsExpected = new[]
					{
						// 割引金額　= 3000(割引総額) * 1000(商品価格) / 3000(割引対象商品小計の総額) = 1000円
						new
						{
							PriceSubtotalAfterDistribution = 0m,
							PriceSubtotalAfterDistributionForCampaign = 1000m
						},
						// 割引金額　= 3000(割引総額) * 2000(商品価格) / 3000(割引対象商品小計の総額) = 1000円
						new
						{
							PriceSubtotalAfterDistribution = 0m,
							PriceSubtotalAfterDistributionForCampaign = 2000m
						},
						// クーポン割引対象外商品
						new
						{
							PriceSubtotalAfterDistribution = 3000m,
							PriceSubtotalAfterDistributionForCampaign = 3000m
						}
					},
				},
				"商品割引金額指定：割引金額 > 対象商品価格小計"
			},
			// 商品割引率指定
			new object[]
			{
				new
				{
					CouponOptionEnabled = true
				},
				new
				{
					DateTimeNow = DateTime.Parse("2020/01/01"),
					ShippingPrice = 4000m,
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 1000m,
							ItemQuantity = 1,
							ProductIconFlg1 = Constants.FLG_PRODUCT_ICON_ON,
							ProductIconTerm1 = DateTime.Parse("2021/01/01"),
							MemberRankDiscountFlg = true,
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 1,
							ProductIconFlg1 = Constants.FLG_PRODUCT_ICON_ON,
							ProductIconTerm1 = DateTime.Parse("2021/01/01"),
							MemberRankDiscountFlg = true,
						},
						// クーポン割引対象外商品
						new
						{
							ProductPrice = 3000m,
							ItemQuantity = 1,
							ProductIconFlg1 = Constants.FLG_PRODUCT_ICON_OFF,
							ProductIconTerm1 = DateTime.Parse("2021/01/01"),
							MemberRankDiscountFlg = false,
						}
					},
					UserCouponDetailInfo = new[]
					{
						new UserCouponDetailInfo
						{
							DiscountRate = 10m,
							CouponType = Constants.FLG_COUPONCOUPON_TYPE_ALL_UNLIMIT,
							PublishDateBgn = DateTime.Now,
							PublishDateEnd = DateTime.Now,
							DateCreated = DateTime.Now,
							DateChanged = DateTime.Now,
							ExceptionalIcon1 = Constants.FLG_COUPON_EXCEPTIONAL_ICON1,
							ExceptionalIcon2 = 0,
							ExceptionalIcon3 = 0,
							ExceptionalIcon4 = 0,
							ExceptionalIcon5 = 0,
							ExceptionalIcon6 = 0,
							ExceptionalIcon7 = 0,
							ExceptionalIcon8 = 0,
							ExceptionalIcon9 = 0,
							ExceptionalIcon10 = 0,
							ProductKbn = Constants.FLG_COUPON_PRODUCT_KBN_UNTARGET,
							ExceptionalProduct = "test",
						}
					},
					CartCoupon = new CartCoupon(
						new UserCouponDetailInfo
						{
							DiscountRate = 10m,
							CouponType = Constants.FLG_COUPONCOUPON_TYPE_ALL_UNLIMIT,
							PublishDateBgn = DateTime.Now,
							PublishDateEnd = DateTime.Now,
							DateCreated = DateTime.Now,
							DateChanged = DateTime.Now,
							ExceptionalIcon1 = Constants.FLG_COUPON_EXCEPTIONAL_ICON1,
							ExceptionalIcon2 = 0,
							ExceptionalIcon3 = 0,
							ExceptionalIcon4 = 0,
							ExceptionalIcon5 = 0,
							ExceptionalIcon6 = 0,
							ExceptionalIcon7 = 0,
							ExceptionalIcon8 = 0,
							ExceptionalIcon9 = 0,
							ExceptionalIcon10 = 0,
							ProductKbn = Constants.FLG_COUPON_PRODUCT_KBN_UNTARGET,
							ExceptionalProduct = "test",
						}),
				},
				new
				{
					UseMaxCouponPrice = 300m,
					UseCouponPrice = 300m,
					ShippingPriceDiscountAmount = 0m,
					ResultsExpected = new[]
					{
						// 割引金額　= 300(割引総額) * 1000(商品価格) / 3000(割引対象商品小計の総額) = 100円
						new
						{
							PriceSubtotalAfterDistribution = 900m,
							PriceSubtotalAfterDistributionForCampaign = 1000m
						},
						// 割引金額　= 300(割引総額) * 2000(商品価格) / 3000(割引対象商品小計の総額) = 200円
						new
						{
							PriceSubtotalAfterDistribution = 1800m,
							PriceSubtotalAfterDistributionForCampaign = 2000m
						},
						new
							// クーポン割引対象外商品
						{
							PriceSubtotalAfterDistribution = 3000m,
							PriceSubtotalAfterDistributionForCampaign = 3000m
						}
					},
				},
				"商品割引率指定"
			},
			// 配送料無料
			new object[]
			{
				new
				{
					CouponOptionEnabled = true
				},
				new
				{
					DateTimeNow = DateTime.Parse("2020/01/01"),
					ShippingPrice = 4000m,
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 1000m,
							ItemQuantity = 1,
							ProductIconFlg1 = Constants.FLG_PRODUCT_ICON_ON,
							ProductIconTerm1 = DateTime.Parse("2021/01/01"),
							MemberRankDiscountFlg = true,
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 1,
							ProductIconFlg1 = Constants.FLG_PRODUCT_ICON_ON,
							ProductIconTerm1 = DateTime.Parse("2021/01/01"),
							MemberRankDiscountFlg = true,
						},
						// クーポン割引対象外商品
						new
						{
							ProductPrice = 3000m,
							ItemQuantity = 1,
							ProductIconFlg1 = Constants.FLG_PRODUCT_ICON_OFF,
							ProductIconTerm1 = DateTime.Parse("2021/01/01"),
							MemberRankDiscountFlg = false,
						}
					},
					UserCouponDetailInfo = new[]
					{
						new UserCouponDetailInfo
						{
							CouponType = Constants.FLG_COUPONCOUPON_TYPE_LIMITED_FREESHIPPING_FOR_REGISTERED_USER,
							PublishDateBgn = DateTime.Now,
							PublishDateEnd = DateTime.Now,
							DateCreated = DateTime.Now,
							DateChanged = DateTime.Now,
							ExceptionalIcon1 = 0,
							ExceptionalIcon2 = 0,
							ExceptionalIcon3 = 0,
							ExceptionalIcon4 = 0,
							ExceptionalIcon5 = 0,
							ExceptionalIcon6 = 0,
							ExceptionalIcon7 = 0,
							ExceptionalIcon8 = 0,
							ExceptionalIcon9 = 0,
							ExceptionalIcon10 = 0,
							ProductKbn = Constants.FLG_COUPON_PRODUCT_KBN_UNTARGET,
							ExceptionalProduct = "test",
						}
					},
					CartCoupon = new CartCoupon(
						new UserCouponDetailInfo
						{
							CouponType = Constants.FLG_COUPONCOUPON_TYPE_LIMITED_FREESHIPPING_FOR_REGISTERED_USER,
							PublishDateBgn = DateTime.Now,
							PublishDateEnd = DateTime.Now,
							DateCreated = DateTime.Now,
							DateChanged = DateTime.Now,
							ExceptionalIcon1 = 0,
							ExceptionalIcon2 = 0,
							ExceptionalIcon3 = 0,
							ExceptionalIcon4 = 0,
							ExceptionalIcon5 = 0,
							ExceptionalIcon6 = 0,
							ExceptionalIcon7 = 0,
							ExceptionalIcon8 = 0,
							ExceptionalIcon9 = 0,
							ExceptionalIcon10 = 0,
							ProductKbn = Constants.FLG_COUPON_PRODUCT_KBN_UNTARGET,
							ExceptionalProduct = "test",
						}),
				},
				new
				{
					UseMaxCouponPrice = 4000m,
					UseCouponPrice = 4000m,
					ShippingPriceDiscountAmount = 4000m,
					ResultsExpected = new[]
					{
						new
						{
							PriceSubtotalAfterDistribution = 1000m,
							PriceSubtotalAfterDistributionForCampaign = 1000m
						},
						new
						{
							PriceSubtotalAfterDistribution = 2000m,
							PriceSubtotalAfterDistributionForCampaign = 2000m
						},
						new
							// クーポン割引対象外商品
						{
							PriceSubtotalAfterDistribution = 3000m,
							PriceSubtotalAfterDistributionForCampaign = 3000m
						}
					},
				},
				"配送料無料"
			},
		};

		/// <summary>
		/// ポイント割引額の按分計算
		/// ・ポイントOPがONの場合、カート内全ての商品に対して割引金額の按分計算を実施すること。
		/// ・商品ごとに按分した割引額を、税込小計価格からマイナスした値が「PriceSubtotalAfterDistribution」に設定されること
		/// </summary>
		[DataTestMethod()]
		[DynamicData("m_tdCalculatePointDisCountProductPriceTest")]
		public void CalculatePointDisCountProductPriceTest(
			dynamic config,
			dynamic data,
			dynamic expected,
			string msg)
		{
			// モックによるドメイン偽装
			var productTagMock = new Mock<IProductTagService>();
			productTagMock.Setup(s => s.GetProductTagSetting()).Returns(new ProductTagSettingModel[0]);
			DomainFacade.Instance.ProductTagService = productTagMock.Object;

			using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), true))
			using (new TestConfigurator(Member.Of(() => Constants.GIFTORDER_OPTION_ENABLED), false))
			using (new TestConfigurator(Member.Of(() => Constants.TAX_EXCLUDED_FRACTION_ROUNDING),
				TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN))
			using (new TestConfigurator(Member.Of(() => Constants.W2MP_POINT_OPTION_ENABLED), config.PointOptionEnabled))
			{
				var cart = CartTestHelper.CreateCart();
				cart.Owner = CartTestHelper.CreateCartOwner();
				foreach (var productParam in data.ProductParams)
				{
					var cartProduct = (CartProduct)CartTestHelper.CreateCartProduct(
						productPrice: productParam.ProductPrice,
						productCount: productParam.ItemQuantity);
					cart.Items.Add(cartProduct);
				}

				cart.GetShipping().UpdateShippingAddr(cart.Owner, true);
				cart.CalculatePriceSubTotal();
				cart.UsePointPrice = data.UsePointPrice;

				new PrivateObject(cart).Invoke("CalculatePointDisCountProductPrice");

				// ポイント割引額の按分計算
				// ・ポイントOPがONの場合、カート内全ての商品に対して割引金額の按分計算を実施すること。
				// ・商品ごとに按分した割引額を、税込小計価格からマイナスした値が「PriceSubtotalAfterDistribution」に設定されること
				var index = 0;
				foreach (var item in cart.Items)
				{
					var resultExpected = expected.ResultsExpected[index];
					item.PriceSubtotalAfterDistribution.Should().Be(
						(decimal)resultExpected.PriceSubtotalAfterDistribution,
						string.Format("{0}:カート商品・割引按分適用後小計", msg));
					item.PriceSubtotalAfterDistributionForCampaign.Should().Be(
						(decimal)resultExpected.PriceSubtotalAfterDistributionForCampaign,
						string.Format("{0}:カート商品・割引按分適用後小計(キャンペーン適用対象)", msg));
					index++;
				}
			}
		}

		public static object[] m_tdCalculatePointDisCountProductPriceTest = new[]
		{
			//　ポイントオプション；OFF
			new object[]
			{
				new
				{
					PointOptionEnabled = false
				},
				new
				{
					UsePointPrice = 100m,
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 1000m,
							ItemQuantity = 1,
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 1,
						},
						new
						{
							ProductPrice = 3000m,
							ItemQuantity = 1,
						}
					},
				},
				new
				{
					ResultsExpected = new[]
					{
						new
						{
							PriceSubtotalAfterDistribution = 1000m,
							PriceSubtotalAfterDistributionForCampaign = 1000m
						},
						new
						{
							PriceSubtotalAfterDistribution = 2000m,
							PriceSubtotalAfterDistributionForCampaign = 2000m
						},
						new
						{
							PriceSubtotalAfterDistribution = 3000m,
							PriceSubtotalAfterDistributionForCampaign = 3000m
						}
					},
				},
				"ポイントオプション；OFF"
			},
			// ポイントオプション；ON
			new object[]
			{
				new
				{
					PointOptionEnabled = true
				},
				new
				{
					UsePointPrice = 100m,
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 1000m,
							ItemQuantity = 1,
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 1,
						},
						new
						{
							ProductPrice = 3000m,
							ItemQuantity = 1,
						}
					},
				},
				new
				{
					ResultsExpected = new[]
					{
						// 割引金額　= 100(割引総額) * 1000(商品価格) / 6000(割引対象商品小計の総額) = 16.6 →(端数切捨て) 16円
						new
						{
							PriceSubtotalAfterDistribution = 984m,
							PriceSubtotalAfterDistributionForCampaign = 1000m
						},
						// 割引金額　= 100(割引総額) * 2000(商品価格) / 6000(割引対象商品小計の総額) = 33.3 →(端数切捨て) 33円
						new
						{
							PriceSubtotalAfterDistribution = 1967m,
							PriceSubtotalAfterDistributionForCampaign = 2000m
						},
						// 割引金額　= 100(割引総額) * 3000(商品価格) / 6000(割引対象商品小計の総額) = 50 + 1(端数(100 - 16 - 33 - 55)の重み付け) = 51円
						new
						{
							PriceSubtotalAfterDistribution = 2949m,
							PriceSubtotalAfterDistributionForCampaign = 3000m
						}
					},
				},
				"ポイントオプション；ON"
			},
		};

		/// <summary>
		/// 定期購入割引額の計算
		/// ・定期購入オプションがONの場合計算が行われること。
		/// ・割引額の計算には定期購入割引設定マスタの値を使用して、商品毎の定期購入割引価格を計算すること
		/// ・計算した割引額を、税込小計価格からマイナスした値が「PriceSubtotalAfterDistribution」「PriceSubtotalAfterDistributionForCampaign」に設定されること
		/// ・計算後、使用した定期購入割引設定マスタの値をCartProductの「FixedPurchaseDiscountType」「FixedPurchaseDiscountValue」に設定すること
		/// ■通常カートの場合
		///   ・カート投入区分が「定期」の商品に対して、定期購入回数から取得した定期購入割引設定マスタの値で商品毎の割引価格を計算すること。
		///     定期購入回数は以下の値を使用する
		///     1.カートに紐づく定期台帳情報が存在する場合、定期台帳に保持された購入回数を用いて割引額が計算されること
		///     2.カートに紐づく定期台帳情報が存在しない場合、1回目の購入として割引額が計算されること
		/// ■注文同梱カートの場合
		///   ・親注文、もしくは子注文に定期購入商品が存在している場合定期購入割引を計算する
		///   ・引数「CurrentFixedPurchaseOrderCount」が1の場合(同梱が行われた回)、元注文の定期購入金額を引継いで使用する
		///       1.定期購入割引の合計価格は、元Cartに設定された定期購入割引額「FixedPurchaseDiscount」を引き継いで適用されること
		///       2.商品個別の割引額は、CartProductの「FixedPurchaseDiscountType」「FixedPurchaseDiscountValue」を用いて計算されること。
		///       3.Cart.FixedPurchaseDiscountと、2で計算した割引額の合計に差異があった場合、差異の金額を「FixedPurchaseDiscountTypeが`空でない商品」に対して按分を行う
		///   ・引数「CurrentFixedPurchaseOrderCount」が2以上の場合(同梱後の定期台帳から生成された回)、通常注文カートと同様に設定から再計算を行うこと。
		/// </summary>
		[DataTestMethod()]
		[DynamicData("m_tdCalculateFixedPurchaseDiscountTest")]
		public void CalculateFixedPurchaseDiscountTest(
			dynamic config,
			dynamic data,
			dynamic expected,
			string msg)
		{
			// モックによるドメイン偽装
			var productTagMock = new Mock<IProductTagService>();
			productTagMock.Setup(s => s.GetProductTagSetting()).Returns(new ProductTagSettingModel[0]);
			DomainFacade.Instance.ProductTagService = productTagMock.Object;

			using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), true))
			using (new TestConfigurator(Member.Of(() => Constants.MEMBER_RANK_OPTION_ENABLED), false))
			using (new TestConfigurator(Member.Of(() => Constants.FIXEDPURCHASE_ORDER_DISCOUNT_METHOD), Constants.FLG_FIXEDPURCHASE_COUNT))
			using (new TestConfigurator(Member.Of(() => Constants.FIXEDPURCHASE_OPTION_ENABLED), config.FixedPurchaseOptionEnabled))
			{
				var fixedPurchaseOrderCount = (int)data.FixedPurchaseOrderCount;
				var productFixedPurchaseDiscountSettingServiceMock =
					new Mock<IProductFixedPurchaseDiscountSettingService>();
				// テストケースで想定される定期購入回数以外が引数に指定された場合、NULLを返すように偽装
				productFixedPurchaseDiscountSettingServiceMock.Setup(
					s => s.GetApplyFixedPurchaseDiscountSetting(
						It.IsAny<string>(),
						It.IsAny<string>(),
						It.IsAny<int>(),
						It.IsAny<int>())).Returns((ProductFixedPurchaseDiscountSettingModel)null);
				// テストケースで想定される定期購入回数が引数に指定された場合、テスト用の商品定期購入割引設定を返すように偽装
				productFixedPurchaseDiscountSettingServiceMock.Setup(
						s => s.GetApplyFixedPurchaseDiscountSetting(
							It.IsAny<string>(),
							It.IsAny<string>(),
							It.Is<int>(param => (param == fixedPurchaseOrderCount)),
							It.IsAny<int>()))
					.Returns((ProductFixedPurchaseDiscountSettingModel)data.ProductFixedPurchaseDiscountSetting);
				DomainFacade.Instance.ProductFixedPurchaseDiscountSettingService =
					productFixedPurchaseDiscountSettingServiceMock.Object;

				var cart = CartTestHelper.CreateCart();
				cart.Owner = CartTestHelper.CreateCartOwner();
				foreach (var productParam in data.ProductParams)
				{
					var cartProduct = (CartProduct)CartTestHelper.CreateCartProduct(
						productPrice: productParam.ProductPrice,
						productCount: productParam.ItemQuantity,
						addCartKbn: productParam.AddCartKbn);
					cartProduct.FixedPurchaseDiscountValue = productParam.FixedPurchaseDiscountValue;
					cartProduct.FixedPurchaseDiscountType = productParam.FixedPurchaseDiscountType;
					cart.Items.Add(cartProduct);
				}

				cart.GetShipping().UpdateShippingAddr(cart.Owner, true);
				cart.CalculatePriceSubTotal();
				cart.OrderCombineParentOrderId = data.OrderCombineParentOrderId;
				cart.IsCombineParentOrderHasFixedPurchase = data.IsCombineParentOrderHasFixedPurchase;
				cart.IsBeforeCombineCartHasFixedPurchase = data.IsBeforeCombineCartHasFixedPurchase;
				cart.CombineParentOrderFixedPurchaseOrderCount = data.CombineParentOrderFixedPurchaseOrderCount;
				cart.FixedPurchaseDiscount = data.OldFixedPurchaseDiscount;
				cart.FixedPurchase = data.CartFixedPurchase;

				new PrivateObject(cart).Invoke("CalculateFixedPurchaseDiscount",
					data.CurrentFixedPurchaseOrderCount);

				// 定期購入割引額の計算
				// ・定期購入オプションがONの場合、カート投入区分が「定期」の商品に対して割引金額の按分計算を実施すること。
				// ・商品ごとに按分した割引額を、税込小計価格からマイナスした値が「PriceSubtotalAfterDistribution」「PriceSubtotalAfterDistributionForCampaign」に設定されること
				var index = 0;
				foreach (var item in cart.Items)
				{
					var resultExpected = expected.ResultsExpected[index];
					item.PriceSubtotalAfterDistribution.Should().Be(
						(decimal)resultExpected.PriceSubtotalAfterDistribution,
						string.Format("{0}:カート商品・割引按分適用後小計", msg));
					item.PriceSubtotalAfterDistributionForCampaign.Should().Be(
						(decimal)resultExpected.PriceSubtotalAfterDistributionForCampaign,
						string.Format("{0}:カート商品・割引按分適用後小計(キャンペーン適用対象)", msg));
					item.FixedPurchaseDiscountValue.Should().Be(
						(decimal?)resultExpected.FixedPurchaseDiscountValue,
						string.Format("{0}:カート商品・定期購入割引設定値", msg));
					item.FixedPurchaseDiscountType.Should().Be(
						(string)resultExpected.FixedPurchaseDiscountType,
						string.Format("{0}:カート商品・定期購入割引設定種別", msg));
					index++;
				}

				cart.FixedPurchaseDiscount.Should().Be(
					(decimal)expected.FixedPurchaseDiscount,
					string.Format("{0}:定期購入割引額", msg));
			}
		}

		public static object[] m_tdCalculateFixedPurchaseDiscountTest = new[]
		{
			//　定期購入オプション；OFF
			new object[]
			{
				new
				{
					FixedPurchaseOptionEnabled = false
				},
				new
				{
					OrderCombineParentOrderId = "Order001",
					IsCombineParentOrderHasFixedPurchase = true,
					IsBeforeCombineCartHasFixedPurchase = false,
					CombineParentOrderFixedPurchaseOrderCount = 1,
					CurrentFixedPurchaseOrderCount = 0,
					OldFixedPurchaseDiscount = 0m,
					FixedPurchaseOrderCount = 1,
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 1000m,
							ItemQuantity = 1,
							AddCartKbn = Constants.AddCartKbn.FixedPurchase,
							FixedPurchaseDiscountValue = (decimal?)null,
							FixedPurchaseDiscountType = (string)null,
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 1,
							AddCartKbn = Constants.AddCartKbn.FixedPurchase,
							FixedPurchaseDiscountValue = (decimal?)null,
							FixedPurchaseDiscountType = (string)null,
						},
						// 定期購入割引対象外商品
						new
						{
							ProductPrice = 3000m,
							ItemQuantity = 1,
							AddCartKbn = Constants.AddCartKbn.Normal,
							FixedPurchaseDiscountValue = (decimal?)null,
							FixedPurchaseDiscountType = (string)null,
						}
					},
					ProductFixedPurchaseDiscountSetting = new ProductFixedPurchaseDiscountSettingModel
					{
						DiscountValue = 100m,
						DiscountType = Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_YEN
					},
					CartFixedPurchase = new FixedPurchaseModel
					{
						OrderCount = 1,
					},
				},
				new
				{
					//　定期購入オプションがOFFの場合、割引額は0円
					FixedPurchaseDiscount = 0m,
					ResultsExpected = new[]
					{
						new
						{
							PriceSubtotalAfterDistribution = 1000m,
							PriceSubtotalAfterDistributionForCampaign = 1000m,
							FixedPurchaseDiscountValue = (decimal?)null,
							FixedPurchaseDiscountType = (string)null,
						},
						new
						{
							PriceSubtotalAfterDistribution = 2000m,
							PriceSubtotalAfterDistributionForCampaign = 2000m,
							FixedPurchaseDiscountValue = (decimal?)null,
							FixedPurchaseDiscountType = (string)null,
						},
						// 定期購入割引対象外商品
						new
						{
							PriceSubtotalAfterDistribution = 3000m,
							PriceSubtotalAfterDistributionForCampaign = 3000m,
							FixedPurchaseDiscountValue = (decimal?)null,
							FixedPurchaseDiscountType = (string)null,
						}
					},
				},
				"定期購入オプション；OFF"
			},
			//　注文同梱カート・親子注文に定期購入なし
			new object[]
			{
				new
				{
					FixedPurchaseOptionEnabled = true
				},
				new
				{
					OrderCombineParentOrderId = "Order001",
					IsCombineParentOrderHasFixedPurchase = false,
					IsBeforeCombineCartHasFixedPurchase = false,
					CombineParentOrderFixedPurchaseOrderCount = 1,
					CurrentFixedPurchaseOrderCount = 0,
					OldFixedPurchaseDiscount = 10m,
					// 注文同梱カートの場合、計算対象の定期購入回数はCart.CombineParentOrderFixedPurchaseOrderCount + 引数のCurrentFixedPurchaseOrderCount
					FixedPurchaseOrderCount = 1,
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 1000m,
							ItemQuantity = 1,
							AddCartKbn = Constants.AddCartKbn.Normal,
							FixedPurchaseDiscountValue = (decimal?)null,
							FixedPurchaseDiscountType = (string)null,
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 1,
							AddCartKbn = Constants.AddCartKbn.Normal,
							FixedPurchaseDiscountValue = (decimal?)null,
							FixedPurchaseDiscountType = (string)null,
						},
						// 定期購入割引対象外商品
						new
						{
							ProductPrice = 3000m,
							ItemQuantity = 1,
							AddCartKbn = Constants.AddCartKbn.Normal,
							FixedPurchaseDiscountValue = (decimal?)null,
							FixedPurchaseDiscountType = (string)null,
						}
					},
					ProductFixedPurchaseDiscountSetting = new ProductFixedPurchaseDiscountSettingModel
					{
						DiscountValue = 100m,
						DiscountType = Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_YEN
					},
					CartFixedPurchase = new FixedPurchaseModel
					{
						OrderCount = 1,
					}
				},
				new
				{
					// 親注文・個注文に定期商品がない場合は割引額0円
					FixedPurchaseDiscount = 0m,
					ResultsExpected = new[]
					{
						new
						{
							PriceSubtotalAfterDistribution = 1000m,
							PriceSubtotalAfterDistributionForCampaign = 1000m,
							FixedPurchaseDiscountValue = (decimal?)null,
							FixedPurchaseDiscountType = (string)null,
						},
						new
						{
							PriceSubtotalAfterDistribution = 2000m,
							PriceSubtotalAfterDistributionForCampaign = 2000m,
							FixedPurchaseDiscountValue = (decimal?)null,
							FixedPurchaseDiscountType = (string)null,
						},
						new
						{
							PriceSubtotalAfterDistribution = 3000m,
							PriceSubtotalAfterDistributionForCampaign = 3000m,
							FixedPurchaseDiscountValue = (decimal?)null,
							FixedPurchaseDiscountType = (string)null,
						}
					},
				},
				"注文同梱カート・親子注文に定期購入なし"
			},
			//　注文同梱カート・同梱後1回目の注文・親注文に定期購入あり
			new object[]
			{
				new
				{
					FixedPurchaseOptionEnabled = true
				},
				new
				{
					OrderCombineParentOrderId = "Order001",
					IsCombineParentOrderHasFixedPurchase = true,
					IsBeforeCombineCartHasFixedPurchase = false,
					CombineParentOrderFixedPurchaseOrderCount = 1,
					CurrentFixedPurchaseOrderCount = 0,
					OldFixedPurchaseDiscount = 25m,
					// 注文同梱カートの場合、計算対象の定期購入回数はCart.CombineParentOrderFixedPurchaseOrderCount + 引数のCurrentFixedPurchaseOrderCount
					FixedPurchaseOrderCount = 1,
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 1000m,
							ItemQuantity = 1,
							AddCartKbn = Constants.AddCartKbn.FixedPurchase,
							FixedPurchaseDiscountValue = (decimal?)5m,
							FixedPurchaseDiscountType = Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_YEN,
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 1,
							AddCartKbn = Constants.AddCartKbn.FixedPurchase,
							FixedPurchaseDiscountValue = (decimal?)20m,
							FixedPurchaseDiscountType = Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_YEN,
						},
						new
						{
							ProductPrice = 3000m,
							ItemQuantity = 1,
							AddCartKbn = Constants.AddCartKbn.FixedPurchase,
							FixedPurchaseDiscountValue = (decimal?)null,
							FixedPurchaseDiscountType = (string)null,
						},
						// 定期購入割引対象外商品
						new
						{
							ProductPrice = 4000m,
							ItemQuantity = 1,
							AddCartKbn = Constants.AddCartKbn.Normal,
							FixedPurchaseDiscountValue = (decimal?)null,
							FixedPurchaseDiscountType = (string)null,
						}
					},
					ProductFixedPurchaseDiscountSetting = new ProductFixedPurchaseDiscountSettingModel
					{
						DiscountValue = 100m,
						DiscountType = Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_YEN
					},
					CartFixedPurchase = new FixedPurchaseModel
					{
						OrderCount = 1,
					}
				},
				new
				{
					// 注文同梱後1回目の購入の定期購入割引額は、元注文の定期購入割引額を引き継ぐ
					FixedPurchaseDiscount = 25m,
					ResultsExpected = new[]
					{
						new
						{
							// 割引金額 = 元商品の購入割引設定(種別「円」値「5」：5円)
							PriceSubtotalAfterDistribution = 995m,
							PriceSubtotalAfterDistributionForCampaign = 995m,
							FixedPurchaseDiscountValue = (decimal?)5m,
							FixedPurchaseDiscountType = Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_YEN,
						},
						new
						{
							// 割引金額 = 元商品の購入割引設定(種別「円」値「20」：20円)
							PriceSubtotalAfterDistribution = 1980m,
							PriceSubtotalAfterDistributionForCampaign = 1980m,
							FixedPurchaseDiscountValue = (decimal?)20m,
							FixedPurchaseDiscountType = Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_YEN,

						},
						// 元注文の商品情報に定期購入割引情報が存在しない
						new
						{
							PriceSubtotalAfterDistribution = 3000m,
							PriceSubtotalAfterDistributionForCampaign = 3000m,
							FixedPurchaseDiscountValue = (decimal?)null,
							FixedPurchaseDiscountType = (string)null,
						},
						// 定期購入割引対象外商品
						new
						{
							PriceSubtotalAfterDistribution = 4000m,
							PriceSubtotalAfterDistributionForCampaign = 4000m,
							FixedPurchaseDiscountValue = (decimal?)null,
							FixedPurchaseDiscountType = (string)null,
						}
					},
				},
				"注文同梱カート・同梱後1回目の注文・親注文に定期購入あり"
			},
			//　注文同梱カート・同梱後1回目の注文・子注文に定期購入あり
			new object[]
			{
				new
				{
					FixedPurchaseOptionEnabled = true
				},
				new
				{
					OrderCombineParentOrderId = "Order001",
					IsCombineParentOrderHasFixedPurchase = false,
					IsBeforeCombineCartHasFixedPurchase = true,
					CombineParentOrderFixedPurchaseOrderCount = 1,
					CurrentFixedPurchaseOrderCount = 0,
					OldFixedPurchaseDiscount = 450m,
					// 注文同梱カートの場合、計算対象の定期購入回数はCart.CombineParentOrderFixedPurchaseOrderCount + 引数のCurrentFixedPurchaseOrderCount
					FixedPurchaseOrderCount = 1,
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 1000m,
							ItemQuantity = 1,
							AddCartKbn = Constants.AddCartKbn.FixedPurchase,
							FixedPurchaseDiscountValue = (decimal?)5m,
							FixedPurchaseDiscountType = Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_PERCENT,
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 1,
							AddCartKbn = Constants.AddCartKbn.FixedPurchase,
							FixedPurchaseDiscountValue = (decimal?)20m,
							FixedPurchaseDiscountType = Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_PERCENT,
						},
						new
						{
							ProductPrice = 3000m,
							ItemQuantity = 1,
							AddCartKbn = Constants.AddCartKbn.FixedPurchase,
							FixedPurchaseDiscountValue = (decimal?)null,
							FixedPurchaseDiscountType = (string)null,
						},
						// 定期購入割引対象外商品
						new
						{
							ProductPrice = 4000m,
							ItemQuantity = 1,
							AddCartKbn = Constants.AddCartKbn.Normal,
							FixedPurchaseDiscountValue = (decimal?)null,
							FixedPurchaseDiscountType = (string)null,
						}
					},
					ProductFixedPurchaseDiscountSetting = new ProductFixedPurchaseDiscountSettingModel
					{
						DiscountValue = 100m,
						DiscountType = Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_YEN
					},
					CartFixedPurchase = new FixedPurchaseModel
					{
						OrderCount = 1,
					}
				},
				new
				{
					// 注文同梱後1回目の購入の定期購入割引額は、元注文の定期購入割引額を引き継ぐ
					FixedPurchaseDiscount = 450m,
					ResultsExpected = new[]
					{
						new
						{
							// 割引金額 = 元商品の購入割引設定(種別「%」値「5」：1000 * 0.05 = 50円)
							PriceSubtotalAfterDistribution = 950m,
							PriceSubtotalAfterDistributionForCampaign = 950m,
							FixedPurchaseDiscountValue = (decimal?)5m,
							FixedPurchaseDiscountType = Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_PERCENT,
						},
						new
						{
							// 割引金額 = 元商品の購入割引設定(種別「%」値「20」：2000 * 0.2 = 400円)
							PriceSubtotalAfterDistribution = 1600m,
							PriceSubtotalAfterDistributionForCampaign = 1600m,
							FixedPurchaseDiscountValue = (decimal?)20m,
							FixedPurchaseDiscountType = Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_PERCENT,

						},
						// 元注文の商品情報に定期購入割引情報が存在しない
						new
						{
							PriceSubtotalAfterDistribution = 3000m,
							PriceSubtotalAfterDistributionForCampaign = 3000m,
							FixedPurchaseDiscountValue = (decimal?)null,
							FixedPurchaseDiscountType = (string)null,
						},
						// 定期購入割引対象外商品
						new
						{
							PriceSubtotalAfterDistribution = 4000m,
							PriceSubtotalAfterDistributionForCampaign = 4000m,
							FixedPurchaseDiscountValue = (decimal?)null,
							FixedPurchaseDiscountType = (string)null,
						}
					},
				},
				"注文同梱カート・同梱後1回目の注文・子注文に定期購入あり"
			},
			//　注文同梱カート・同梱後1回目の注文・元注文定期購入価格と商品個別の割引額に差異が存在
			new object[]
			{
				new
				{
					FixedPurchaseOptionEnabled = true
				},
				new
				{
					OrderCombineParentOrderId = "Order001",
					IsCombineParentOrderHasFixedPurchase = true,
					IsBeforeCombineCartHasFixedPurchase = false,
					CombineParentOrderFixedPurchaseOrderCount = 1,
					CurrentFixedPurchaseOrderCount = 0,
					OldFixedPurchaseDiscount = 50m,
					// 注文同梱カートの場合、計算対象の定期購入回数はCart.CombineParentOrderFixedPurchaseOrderCount + 引数のCurrentFixedPurchaseOrderCount
					FixedPurchaseOrderCount = 1,
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 1000m,
							ItemQuantity = 1,
							AddCartKbn = Constants.AddCartKbn.FixedPurchase,
							FixedPurchaseDiscountValue = (decimal?)5m,
							FixedPurchaseDiscountType = Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_YEN,
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 1,
							AddCartKbn = Constants.AddCartKbn.FixedPurchase,
							FixedPurchaseDiscountValue = (decimal?)20m,
							FixedPurchaseDiscountType = Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_YEN,
						},
						new
						{
							ProductPrice = 3000m,
							ItemQuantity = 1,
							AddCartKbn = Constants.AddCartKbn.FixedPurchase,
							FixedPurchaseDiscountValue = (decimal?)null,
							FixedPurchaseDiscountType = (string)null,
						},
						// 定期購入割引対象外商品
						new
						{
							ProductPrice = 4000m,
							ItemQuantity = 1,
							AddCartKbn = Constants.AddCartKbn.Normal,
							FixedPurchaseDiscountValue = (decimal?)null,
							FixedPurchaseDiscountType = (string)null,
						}
					},
					ProductFixedPurchaseDiscountSetting = new ProductFixedPurchaseDiscountSettingModel
					{
						DiscountValue = 100m,
						DiscountType = Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_YEN
					},
					CartFixedPurchase = new FixedPurchaseModel
					{
						OrderCount = 1,
					}
				},
				new
				{
					// 注文同梱後1回目の購入の定期購入割引額は、元注文の定期購入割引額を引き継ぐ
					FixedPurchaseDiscount = 50m,
					// 1.元注文のOldFixedPurchaseDiscount「50」を引継ぐ
					// 2.CartProductの定期購入割引設定の合計値と1に差異がある場合、以下の方法で差異分の商品毎の割引額を計算する
					// 当ケースでは差異額は「50(元注文の割引額合計)」 - (5 + 20)(各商品の割引設定から算出した割引額) = 25」がある。
					// ・差異(25)を、FixedPurchaseDiscountTypeがNullでない商品に対して按分処理を行う
					ResultsExpected = new[]
					{
						new
						{
							// CartProductの設定値から計算された割引額 = 元商品の購入割引設定(種別「円」値「5」：5円)
							// 按分された差異額 = 25(元注文との割引差異) * 995(按分前の商品小計) / 2975(按分前の対象商品の合計) = 8.3 → 8円
							// 商品の割引額 = 5(元注文の設定から計算した割引額) + 8(按分された割引差異額) = 13円
							PriceSubtotalAfterDistribution = 987m,
							PriceSubtotalAfterDistributionForCampaign = 987m,
							FixedPurchaseDiscountValue = (decimal?)5m,
							FixedPurchaseDiscountType = Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_YEN,
						},
						new
						{
							// CartProductの設定値から計算された割引額 = 元商品の購入割引設定(種別「円」値「20」：20円)
							// 按分された差異額 = 25(元注文との割引差異) * 1980(按分前の商品小計) / 2975(按分前の対象商品の合計) = 16.6 → 17円
							// 商品の割引額 = 20(元注文の設定から計算した割引額) + 17(按分された割引差異額) = 37円
							PriceSubtotalAfterDistribution = 1963m,
							PriceSubtotalAfterDistributionForCampaign = 1963m,
							FixedPurchaseDiscountValue = (decimal?)20m,
							FixedPurchaseDiscountType = Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_YEN,

						},
						// 元注文の商品情報に定期購入割引情報が存在しないので割引対象外
						new
						{
							PriceSubtotalAfterDistribution = 3000m,
							PriceSubtotalAfterDistributionForCampaign = 3000m,
							FixedPurchaseDiscountValue = (decimal?)null,
							FixedPurchaseDiscountType = (string)null,
						},
						// 定期購入割引対象外商品
						new
						{
							PriceSubtotalAfterDistribution = 4000m,
							PriceSubtotalAfterDistributionForCampaign = 4000m,
							FixedPurchaseDiscountValue = (decimal?)null,
							FixedPurchaseDiscountType = (string)null,
						}
					},
				},
				"注文同梱カート・同梱後1回目の注文・元注文定期購入価格と商品個別の割引額に差異が存在"
			},
			//　注文同梱カート・同梱後2回目の購入時割引価格の計算
			new object[]
			{
				new
				{
					FixedPurchaseOptionEnabled = true
				},
				new
				{
					OrderCombineParentOrderId = "Order001",
					IsCombineParentOrderHasFixedPurchase = true,
					IsBeforeCombineCartHasFixedPurchase = false,
					CombineParentOrderFixedPurchaseOrderCount = 1,
					CurrentFixedPurchaseOrderCount = 1,
					OldFixedPurchaseDiscount = 25m,
					// 注文同梱カートの場合、計算対象の定期購入回数はCart.CombineParentOrderFixedPurchaseOrderCount + 引数のCurrentFixedPurchaseOrderCount
					FixedPurchaseOrderCount = 2,
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 1000m,
							ItemQuantity = 1,
							AddCartKbn = Constants.AddCartKbn.FixedPurchase,
							FixedPurchaseDiscountValue = (decimal?)5m,
							FixedPurchaseDiscountType = Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_YEN,
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 1,
							AddCartKbn = Constants.AddCartKbn.FixedPurchase,
							FixedPurchaseDiscountValue = (decimal?)20m,
							FixedPurchaseDiscountType = Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_YEN,
						},
						// 定期購入割引対象外商品
						new
						{
							ProductPrice = 3000m,
							ItemQuantity = 1,
							AddCartKbn = Constants.AddCartKbn.Normal,
							FixedPurchaseDiscountValue = (decimal?)null,
							FixedPurchaseDiscountType = (string)null,
						}
					},
					ProductFixedPurchaseDiscountSetting = new ProductFixedPurchaseDiscountSettingModel
					{
						DiscountValue = 100m,
						DiscountType = Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_YEN
					},
					CartFixedPurchase = new FixedPurchaseModel
					{
						OrderCount = 1,
					}
				},
				new
				{
					// 注文同梱2回目の購入の定期購入割引額は、商品定期購入割引設定から計算される
					FixedPurchaseDiscount = 200m,
					ResultsExpected = new[]
					{
						new
						{
							// 割引金額 = 定期購入割引設定マスタの購入割引設定(種別「円」値「100」：100円)
							// 定期購入割引設定マスタの値がCartProductに設定される
							PriceSubtotalAfterDistribution = 900m,
							PriceSubtotalAfterDistributionForCampaign = 900m,
							FixedPurchaseDiscountValue = (decimal?)100m,
							FixedPurchaseDiscountType = Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_YEN,
						},
						new
						{
							// 割引金額 = 定期購入割引設定マスタの購入割引設定(種別「円」値「100」：100円)
							// 定期購入割引設定マスタの値がCartProductに設定される
							PriceSubtotalAfterDistribution = 1900m,
							PriceSubtotalAfterDistributionForCampaign = 1900m,
							FixedPurchaseDiscountValue = (decimal?)100m,
							FixedPurchaseDiscountType = Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_YEN,
						},
						// 定期購入商品ではない
						new
						{
							PriceSubtotalAfterDistribution = 3000m,
							PriceSubtotalAfterDistributionForCampaign = 3000m,
							FixedPurchaseDiscountValue = (decimal?)null,
							FixedPurchaseDiscountType = (string)null,
						}
					},
				},
				"注文同梱カート・同梱後2回目の購入時割引価格の計算"
			},
			// 通常定期購入カート・カートに紐づく定期情報がNULLではない
			new object[]
			{
				new
				{
					FixedPurchaseOptionEnabled = true
				},
				new
				{
					OrderCombineParentOrderId = "",
					IsCombineParentOrderHasFixedPurchase = false,
					IsBeforeCombineCartHasFixedPurchase = false,
					CombineParentOrderFixedPurchaseOrderCount = 0,
					CurrentFixedPurchaseOrderCount = 0,
					OldFixedPurchaseDiscount = 0m,
					// 通常カートかつカートに紐づく定期台帳情報が存在する場合、計算対象の定期購入回数は、定期台帳のOrderCount + 1
					FixedPurchaseOrderCount = 2,
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 1000m,
							ItemQuantity = 1,
							AddCartKbn = Constants.AddCartKbn.FixedPurchase,
							FixedPurchaseDiscountValue = (decimal?)null,
							FixedPurchaseDiscountType = (string)null,
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 1,
							AddCartKbn = Constants.AddCartKbn.FixedPurchase,
							FixedPurchaseDiscountValue = (decimal?)null,
							FixedPurchaseDiscountType = (string)null,
						},
						// 定期購入商品ではない
						new
						{
							ProductPrice = 3000m,
							ItemQuantity = 1,
							AddCartKbn = Constants.AddCartKbn.Normal,
							FixedPurchaseDiscountValue = (decimal?)null,
							FixedPurchaseDiscountType = (string)null,
						}
					},
					ProductFixedPurchaseDiscountSetting = new ProductFixedPurchaseDiscountSettingModel
					{
						DiscountValue = 100m,
						DiscountType = Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_YEN
					},
					CartFixedPurchase = new FixedPurchaseModel
					{
						OrderCount = 1,
					}
				},
				new
				{
					FixedPurchaseDiscount = 200m,
					ResultsExpected = new[]
					{
						new
						{
							// 割引金額 = 定期購入割引設定マスタの購入割引設定(種別「円」値「100」：100円)
							// 定期購入割引設定マスタの値がCartProductに設定される
							PriceSubtotalAfterDistribution = 900m,
							PriceSubtotalAfterDistributionForCampaign = 900m,
							FixedPurchaseDiscountValue = (decimal?)100m,
							FixedPurchaseDiscountType = Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_YEN,
						},
						new
						{
							// 割引金額 = 定期購入割引設定マスタの購入割引設定(種別「円」値「100」：100円)
							// 定期購入割引設定マスタの値がCartProductに設定される
							PriceSubtotalAfterDistribution = 1900m,
							PriceSubtotalAfterDistributionForCampaign = 1900m,
							FixedPurchaseDiscountValue = (decimal?)100m,
							FixedPurchaseDiscountType = Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_YEN,
						},
						// 定期購入商品ではない
						new
						{
							PriceSubtotalAfterDistribution = 3000m,
							PriceSubtotalAfterDistributionForCampaign = 3000m,
							FixedPurchaseDiscountValue = (decimal?)null,
							FixedPurchaseDiscountType = (string)null,
						}
					},
				},
				"通常定期購入カート・カートに紐づく定期情報がNULLではない"
			},
			// 通常定期購入カート・カートに紐づく定期情報がNULL・定期購入商品あり
			new object[]
			{
				new
				{
					FixedPurchaseOptionEnabled = true
				},
				new
				{
					OrderCombineParentOrderId = "",
					IsCombineParentOrderHasFixedPurchase = false,
					IsBeforeCombineCartHasFixedPurchase = false,
					CombineParentOrderFixedPurchaseOrderCount = 0,
					CurrentFixedPurchaseOrderCount = 0,
					OldFixedPurchaseDiscount = 0m,
					// 通常カートかつカートに紐づく定期台帳情報がNULLの場合、計算対象の定期購入回数は、1 + 引数に設定されたCurrentFixedPurchaseOrderCount
					FixedPurchaseOrderCount = 1,
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 1000m,
							ItemQuantity = 1,
							AddCartKbn = Constants.AddCartKbn.FixedPurchase,
							FixedPurchaseDiscountValue = (decimal?)null,
							FixedPurchaseDiscountType = (string)null,
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 1,
							AddCartKbn = Constants.AddCartKbn.FixedPurchase,
							FixedPurchaseDiscountValue = (decimal?)null,
							FixedPurchaseDiscountType = (string)null,
						},
						// 定期購入割引対象外商品
						new
						{
							ProductPrice = 3000m,
							ItemQuantity = 1,
							AddCartKbn = Constants.AddCartKbn.Normal,
							FixedPurchaseDiscountValue = (decimal?)null,
							FixedPurchaseDiscountType = (string)null,
						}
					},
					ProductFixedPurchaseDiscountSetting = new ProductFixedPurchaseDiscountSettingModel
					{
						DiscountValue = 100m,
						DiscountType = Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_YEN
					},
					CartFixedPurchase = (FixedPurchaseModel)null,
				},
				new
				{
					FixedPurchaseDiscount = 200m,
					ResultsExpected = new[]
					{
						new
						{
							// 割引金額 = 定期購入割引設定マスタの購入割引設定(種別「円」値「100」：100円)
							// 定期購入割引設定マスタの値がCartProductに設定される
							PriceSubtotalAfterDistribution = 900m,
							PriceSubtotalAfterDistributionForCampaign = 900m,
							FixedPurchaseDiscountValue = (decimal?)100m,
							FixedPurchaseDiscountType = Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_YEN,
						},
						new
						{
							// 割引金額 = 定期購入割引設定マスタの購入割引設定(種別「円」値「100」：100円)
							// 定期購入割引設定マスタの値がCartProductに設定される
							PriceSubtotalAfterDistribution = 1900m,
							PriceSubtotalAfterDistributionForCampaign = 1900m,
							FixedPurchaseDiscountValue = (decimal?)100m,
							FixedPurchaseDiscountType = Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_YEN,
						},
						// 定期購入商品ではない
						new
						{
							PriceSubtotalAfterDistribution = 3000m,
							PriceSubtotalAfterDistributionForCampaign = 3000m,
							FixedPurchaseDiscountValue = (decimal?)null,
							FixedPurchaseDiscountType = (string)null,
						}
					},
				},
				"通常定期購入カート・カートに紐づく定期情報がNULL・定期購入商品あり"
			},
			// 通常定期購入カート・カートに紐づく定期情報がNULL・定期購入商品なし
			new object[]
			{
				new
				{
					FixedPurchaseOptionEnabled = true
				},
				new
				{
					OrderCombineParentOrderId = "",
					IsCombineParentOrderHasFixedPurchase = false,
					IsBeforeCombineCartHasFixedPurchase = false,
					CombineParentOrderFixedPurchaseOrderCount = 0,
					CurrentFixedPurchaseOrderCount = 0,
					OldFixedPurchaseDiscount = 0m,
					FixedPurchaseOrderCount = 1,
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 1000m,
							ItemQuantity = 1,
							AddCartKbn = Constants.AddCartKbn.Normal,
							FixedPurchaseDiscountValue = (decimal?)null,
							FixedPurchaseDiscountType = (string)null,
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 1,
							AddCartKbn = Constants.AddCartKbn.Normal,
							FixedPurchaseDiscountValue = (decimal?)null,
							FixedPurchaseDiscountType = (string)null,
						},
						// 定期購入割引対象外商品
						new
						{
							ProductPrice = 3000m,
							ItemQuantity = 1,
							AddCartKbn = Constants.AddCartKbn.Normal,
							FixedPurchaseDiscountValue = (decimal?)null,
							FixedPurchaseDiscountType = (string)null,
						}
					},
					ProductFixedPurchaseDiscountSetting = new ProductFixedPurchaseDiscountSettingModel
					{
						DiscountValue = 100m,
						DiscountType = Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_YEN
					},
					CartFixedPurchase = (FixedPurchaseModel)null,
				},
				new
				{
					// 定期購入商品無しの場合、定期購入割引額は0円
					FixedPurchaseDiscount = 0m,
					ResultsExpected = new[]
					{
						new
						{
							PriceSubtotalAfterDistribution = 1000m,
							PriceSubtotalAfterDistributionForCampaign = 1000m,
							FixedPurchaseDiscountValue = (decimal?)null,
							FixedPurchaseDiscountType = (string)null,
						},
						new
						{
							PriceSubtotalAfterDistribution = 2000m,
							PriceSubtotalAfterDistributionForCampaign = 2000m,
							FixedPurchaseDiscountValue = (decimal?)null,
							FixedPurchaseDiscountType = (string)null,
						},
						new
						{
							PriceSubtotalAfterDistribution = 3000m,
							PriceSubtotalAfterDistributionForCampaign = 3000m,
							FixedPurchaseDiscountValue = (decimal?)null,
							FixedPurchaseDiscountType = (string)null,
						}
					},
				},
				"通常定期購入カート・カートに紐づく定期情報がNULL・定期購入商品なし"
			},
		};

		/// <summary>
		/// 定期会員割引額の按分計算
		/// ・カート投入区分が「通常」の商品に対して割引金額の按分計算を実施すること
		/// ・商品ごとに按分した割引額を、税込小計価格からマイナスした値が「PriceSubtotalAfterDistribution」「PriceSubtotalAfterDistributionForCampaign」に設定されること
		/// </summary>
		[DataTestMethod()]
		[DynamicData("m_tdCalculateFixedPurchaseMemberDiscountTest")]
		public void CalculateFixedPurchaseMemberDiscountTest(
			dynamic config,
			dynamic data,
			dynamic expected,
			string msg)
		{
			// モックによるドメイン偽装
			var productTagMock = new Mock<IProductTagService>();
			productTagMock.Setup(s => s.GetProductTagSetting()).Returns(new ProductTagSettingModel[0]);
			DomainFacade.Instance.ProductTagService = productTagMock.Object;

			using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), true))
			using (new TestConfigurator(Member.Of(() => Constants.TAX_EXCLUDED_FRACTION_ROUNDING),
				TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN))
			using (new TestConfigurator(Member.Of(() => Constants.GIFTORDER_OPTION_ENABLED), false))
			using (new TestConfigurator(Member.Of(() => Constants.FIXEDPURCHASE_OPTION_ENABLED), true))
			using (new TestConfigurator(Member.Of(() => Constants.MEMBER_RANK_OPTION_ENABLED), config.MemberRankOptionEnabled))
			{
				var cacheData = new[]
				{
						new MemberRankModel
						{
							MemberRankId = data.MemberRankId,
							OrderDiscountType = Constants.FLG_MEMBERRANK_ORDER_DISCOUNT_TYPE_NONE,
							OrderDiscountValue = 0m,
							OrderDiscountThresholdPrice = 0m,
							FixedPurchaseDiscountRate = data.FixedPurchaseDiscountRate,
						}
					};
				using (new MemberRankDataCacheConfigurator(cacheData))
				{
					var cart = (CartObject)CartTestHelper.CreateCart(memberRankId: data.MemberRankId);
					cart.Owner = CartTestHelper.CreateCartOwner();
					foreach (var productParam in data.ProductParams)
					{
						var cartProduct = (CartProduct)CartTestHelper.CreateCartProduct(
							productPrice: productParam.ProductPrice,
							productCount: productParam.ItemQuantity,
							addCartKbn: productParam.AddCartKbn);
						cartProduct.NoveltyId = productParam.NoveltyId;
						cart.Items.Add(cartProduct);
					}

					cart.GetShipping().UpdateShippingAddr(cart.Owner, true);
					cart.CalculatePriceSubTotal();

					new PrivateObject(cart).Invoke("CalculateFixedPurchaseMemberDiscount");

					// 定期会員割引額の按分計算
					// ・カート投入区分が「通常」の商品に対して割引金額の按分計算を実施すること
					// ・商品ごとに按分した割引額を、税込小計価格からマイナスした値が「PriceSubtotalAfterDistribution」「PriceSubtotalAfterDistributionForCampaign」に設定されること
					var index = 0;
					foreach (var item in cart.Items)
					{
						var resultExpected = expected.ResultsExpected[index];
						item.PriceSubtotalAfterDistribution.Should().Be(
							(decimal)resultExpected.PriceSubtotalAfterDistribution,
							string.Format("{0}:カート商品・割引按分適用後小計", msg));
						item.PriceSubtotalAfterDistributionForCampaign.Should().Be(
							(decimal)resultExpected.PriceSubtotalAfterDistributionForCampaign,
							string.Format("{0}:カート商品・割引按分適用後小計(キャンペーン適用対象)", msg));
						index++;
					}

					cart.FixedPurchaseMemberDiscountAmount.Should().Be(
						(decimal)expected.FixedPurchaseMemberDiscountAmount,
						string.Format("{0}:定期会員割引額", msg));
				}
			}
		}

		public static object[] m_tdCalculateFixedPurchaseMemberDiscountTest = new[]
		{
			// 会員ランクOP：ON
			new object[]
			{
				new
				{
					MemberRankOptionEnabled = true,
				},
				new
				{
					MemberRankId = "rank001",
					FixedPurchaseDiscountRate = 10m,
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 1000m,
							ItemQuantity = 1,
							AddCartKbn = Constants.AddCartKbn.Normal,
							NoveltyId = "",
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 1,
							AddCartKbn = Constants.AddCartKbn.Normal,
							NoveltyId = "",
						},
						// 定期会員割引対象外商品
						new
						{
							ProductPrice = 3000m,
							ItemQuantity = 1,
							AddCartKbn = Constants.AddCartKbn.FixedPurchase,
							NoveltyId = "",
						}
					},
				},
				new
				{
					FixedPurchaseMemberDiscountAmount = 300m,
					ResultsExpected = new[]
					{
						// 割引金額　= 300(割引総額) * 1000(商品価格) / 3000(割引対象商品小計の総額) = 100円
						new
						{
							PriceSubtotalAfterDistribution = 900m,
							PriceSubtotalAfterDistributionForCampaign = 900m
						},
						// 割引金額　= 3000(割引総額) * 2000(商品価格) / 3000(割引対象商品小計の総額) = 200円
						new
						{
							PriceSubtotalAfterDistribution = 1800m,
							PriceSubtotalAfterDistributionForCampaign = 1800m
						},
						// 定期会員割引対象外商品
						new
						{
							PriceSubtotalAfterDistribution = 3000m,
							PriceSubtotalAfterDistributionForCampaign = 3000m
						}
					},
				},
				"会員ランクOP：ON"
			},
			// 会員ランクOP：OFF
			new object[]
			{
				new
				{
					MemberRankOptionEnabled = false,
				},
				new
				{
					MemberRankId = "rank001",
					FixedPurchaseDiscountRate = 10m,
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 1000m,
							ItemQuantity = 1,
							AddCartKbn = Constants.AddCartKbn.Normal,
							NoveltyId = "",
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 1,
							AddCartKbn = Constants.AddCartKbn.Normal,
							NoveltyId = "",
						},
						// 定期会員割引対象外商品
						new
						{
							ProductPrice = 3000m,
							ItemQuantity = 1,
							AddCartKbn = Constants.AddCartKbn.FixedPurchase,
							NoveltyId = "",
						}
					},
				},
				new
				{
					FixedPurchaseMemberDiscountAmount = 0m,
					ResultsExpected = new[]
					{
						new
						{
							PriceSubtotalAfterDistribution = 1000m,
							PriceSubtotalAfterDistributionForCampaign = 1000m
						},
						new
						{
							PriceSubtotalAfterDistribution = 2000m,
							PriceSubtotalAfterDistributionForCampaign = 2000m
						},
						new
						{
							PriceSubtotalAfterDistribution = 3000m,
							PriceSubtotalAfterDistributionForCampaign = 3000m
						}
					},
				},
				"会員ランクOP：OFF"
			},
		};

		/// <summary>
		/// 調整金額按分計算：配送先一つ
		/// ・調整金額を各商品・配送料・決済手数料に対してそれぞれ按分計算を行う。
		/// ・按分後に端数余りが発生した場合、下記の優先順位で端数が重み付けされること。
		///   ・「最も金額が高い商品」→「配送料」→「決済手数料」(金額が0以上の項目に端数重み付けがされる)
		/// </summary>
		[DataTestMethod()]
		[DynamicData("m_tdCalculatePriceRegulation_ShingleShipping")]
		public void CalculatePriceRegulation_ShingleShippings(
			dynamic config,
			dynamic data,
			dynamic expected,
			string msg)
		{
			using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), true))
			using (new TestConfigurator(Member.Of(() => Constants.TAX_EXCLUDED_FRACTION_ROUNDING),
				TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN))
			using (new TestConfigurator(Member.Of(() => Constants.GIFTORDER_OPTION_ENABLED), true))
			{

				// モックによるドメイン偽装
				var mock = new Mock<IProductTagService>();
				mock.Setup(s => s.GetProductTagSetting()).Returns(new ProductTagSettingModel[0]);
				DomainFacade.Instance.ProductTagService = mock.Object;

				var cart = CartTestHelper.CreateCart();
				cart.Owner = CartTestHelper.CreateCartOwner();
				cart.GetShipping().UpdateShippingAddr(cart.Owner, true);
				foreach (var productParam in data.ProductParams)
				{
					var cartProduct = CartTestHelper.CreateCartProduct(
						productParam.ProductPrice,
						productParam.ItemQuantity);
					cart.Items.Add(cartProduct);
				}

				cart.EnteredShippingPrice = data.ShippingPrice;
				cart.EnteredPaymentPrice = data.PaymentPrice;
				cart.PriceRegulation = data.RegulationPrice;
				cart.CalculatePriceSubTotal();
				new PrivateObject(cart).Invoke("CalculatePriceRegulation");

				// 調整金額按分計算：配送先一つ
				// ・調整金額を各商品・配送料・決済手数料に対してそれぞれ按分計算を行う。
				var itemIndex = 0;
				foreach (var item in cart.Items)
				{
					var itemPriceRegulationExpected = (decimal)expected.ItemPriceRegulationsExpected[itemIndex];
					item.ItemPriceRegulation.Should().Be(
						itemPriceRegulationExpected,
						string.Format("{0}:カート商品・按分後調整金額", msg));
					itemIndex++;
				}

				cart.ShippingPriceDiscountAmount.Should().Be(
					(decimal)expected.ShippingDiscountAmount,
					string.Format("{0}:配送料割引金額", msg));
				cart.PaymentPriceDiscountAmount.Should().Be(
					(decimal)expected.PaymentDiscountAmount,
					string.Format("{0}:決済手数料割引金額", msg));
			}
		}

		public static object[] m_tdCalculatePriceRegulation_ShingleShipping = new[]
		{
			// 調整金額0円
			new object[]
			{
				new {},
				new
				{
					RegulationPrice = 0m,
					ShippingPrice = 3000m,
					PaymentPrice = 4000m,
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 1000m,
							ItemQuantity = 1
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 1
						}
					},
				},
				new
				{
					ItemPriceRegulationsExpected = new[] { 0m, 0m, },
					ShippingDiscountAmount = 0m,
					PaymentDiscountAmount = 0m,
				},
				"調整金額0円"
			},
			// 調整金額マイナス
			new object[]
			{
				new {},
				new
				{
					RegulationPrice = -100m,
					ShippingPrice = 3000m,
					PaymentPrice = 4000m,
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 1000m,
							ItemQuantity = 1
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 1
						}
					},
				},
				new
				{
					ItemPriceRegulationsExpected = new[] { -10m, -20m, },
					ShippingDiscountAmount = 30m,
					PaymentDiscountAmount = 40m,
				},
				"調整金額マイナス"
			},
			// 注文合計金額0円
			new object[]
			{
				new {},
				new
				{
					RegulationPrice = 100m,
					ShippingPrice = 0m,
					PaymentPrice = 0m,
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 0m,
							ItemQuantity = 1
						},
						new
						{
							ProductPrice = 0m,
							ItemQuantity = 1
						}
					},
				},
				new
				{
					ItemPriceRegulationsExpected = new[] {0m, 0m, },
					ShippingDiscountAmount = 0m,
					PaymentDiscountAmount = -100m,
				},
				"注文合計金額0円"
			},
			// 按分後小数点以下端数金額無し
			new object[]
			{
				new {},
				new
				{
					RegulationPrice = 100m,
					ShippingPrice = 3000m,
					PaymentPrice = 4000m,
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 1000m,
							ItemQuantity = 1
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 1
						}
					},
				},
				new
				{
					ItemPriceRegulationsExpected = new[] { 10m, 20m, },
					ShippingDiscountAmount =-30m,
					PaymentDiscountAmount = -40m,
				},
				"按分後小数点以下端数金額無し"
			},
			// 按分後小数点以下端数金額あり：商品に重み付け
			new object[]
			{
				new {},
				new
				{
					RegulationPrice = 99m,
					ShippingPrice = 3000m,
					PaymentPrice = 4000m,
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 5000m,
							ItemQuantity = 1
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 1
						}
					},
				},
				new
				{
					ItemPriceRegulationsExpected = new[] { 36m, 14m, },
					ShippingDiscountAmount = -21m,
					PaymentDiscountAmount = -28m,
				},
				"按分後小数点以下端数金額あり：商品に重み付け"
			},
			// 按分後小数点以下端数金額あり：配送料に重み付け
			new object[]
			{
				new {},
				new
				{
					RegulationPrice = 99m,
					ShippingPrice = 3000m,
					PaymentPrice = 4000m,
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 0m,
							ItemQuantity = 1
						},
						new
						{
							ProductPrice = 0m,
							ItemQuantity = 1
						}
					},
				},
				new
				{
					ItemPriceRegulationsExpected = new[] { 0m, 0m, },
					ShippingDiscountAmount = -43m,
					PaymentDiscountAmount = -56m,
				},
				"按分後小数点以下端数金額あり：配送料に重み付け"
			},
		};

		/// <summary>
		/// 調整金額按分計算：複数配送先
		/// ・調整金額を各商品・配送料・決済手数料に対してそれぞれ按分計算を行う。
		/// ・按分後に端数余りが発生した場合、下記の優先順位で端数が重み付けされること。
		///   ・「最も金額が高い商品」→「配送料」→「決済手数料」(金額が0以上の項目に端数重み付けがされる)
		/// </summary>
		[DataTestMethod()]
		[DynamicData("m_tdCalculatePriceRegulation_MultipleShipping")]
		public void CalculatePriceRegulation_MultipleShipping(
			dynamic config,
			dynamic data,
			dynamic expected,
			string msg)
		{
			// モックによるドメイン偽装
			var mock = new Mock<IProductTagService>();
			mock.Setup(s => s.GetProductTagSetting()).Returns(new ProductTagSettingModel[0]);
			DomainFacade.Instance.ProductTagService = mock.Object;

			using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), true))
			using (new TestConfigurator(Member.Of(() => Constants.TAX_EXCLUDED_FRACTION_ROUNDING),
				TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN))
			using (new TestConfigurator(Member.Of(() => Constants.GIFTORDER_OPTION_ENABLED), config.GiftOptionEnabled))
			{
				var cart = CartTestHelper.CreateCart();
				cart.Owner = CartTestHelper.CreateCartOwner();
				cart.Shippings.Clear();
				foreach (var cartItemParam in data.CartItemParams)
				{
					var cartProduct = CartTestHelper.CreateCartProduct(
						productPrice: cartItemParam.ProductPrice,
						productCount: cartItemParam.ItemQuantity,
						addCartKbn: Constants.AddCartKbn.GiftOrder);
					cart.Items.Add(cartProduct);
				}

				foreach (var shippingParam in data.ShippingParams)
				{
					var cartShipping = new CartShipping(cart);
					cartShipping.UpdateShippingAddr(cart.Owner, true);
					foreach (var productCountsParam in shippingParam.ProductCountsParams)
					{
						var productCounts = new CartShipping.ProductCount(
							cart.Items[productCountsParam.IndexOfCartProduct],
							productCountsParam.ItemQuantity);
						cartShipping.ProductCounts.Add(productCounts);
					}

					cart.Shippings.Add(cartShipping);
				}

				cart.EnteredShippingPrice = data.ShippingPrice;
				cart.EnteredPaymentPrice = data.PaymentPrice;
				cart.PriceRegulation = data.RegulationPrice;
				cart.CalculatePriceSubTotal();
				new PrivateObject(cart).Invoke("CalculatePriceRegulation");

				// 調整金額按分計算：複数配送先
				// ・調整金額を各商品・配送料・決済手数料に対してそれぞれ按分計算を行う。
				var itemIndex = 0;
				foreach (var item in cart.Items)
				{
					var itemPriceRegulationExpected = expected.ItemPriceRegulationsExpected[itemIndex];
					item.ItemPriceRegulation.Should().Be(
						(decimal)itemPriceRegulationExpected,
						string.Format("{0}:カート商品・按分後調整金額", msg));
					itemIndex++;
				}

				var shippingIndex = 0;
				foreach (var shipping in cart.Shippings)
				{
					var pcIndex = 0;
					foreach (var pc in shipping.ProductCounts)
					{
						var productCountsResultExpected =
							expected.ProductCountsRegulationExpected[shippingIndex][pcIndex];
						pc.ItemPriceRegulation.Should().Be(
							(decimal)productCountsResultExpected,
							string.Format("{0}:配送先商品情報・按分後調整金額", msg));
						pcIndex++;
					}

					shippingIndex++;
				}

				cart.ShippingPriceDiscountAmount.Should().Be(
					(decimal)expected.ShippingDiscountAmount,
					string.Format("{0}:配送料割引金額", msg));
				cart.PaymentPriceDiscountAmount.Should().Be(
					(decimal)expected.PaymentDiscountAmount,
					string.Format("{0}:決済手数料割引金額", msg));
			}
		}

		public static object[] m_tdCalculatePriceRegulation_MultipleShipping = new[]
		{
			// 調整金額0円
			new object[]
			{
				new
				{
					GiftOptionEnabled = true
				},
				new
				{
					RegulationPrice = 0m,
					ShippingPrice = 3000m,
					PaymentPrice = 4000m,
					CartItemParams = new[]
					{
						new
						{
							ProductPrice = 1000m,
							ItemQuantity = 1,
						},
						new
						{
							ProductPrice = 2000m,
							ItemQuantity = 2,
						},
					},
					ShippingParams = new[]
					{
						new
						{
							ProductCountsParams =  new[]
							{
								new
								{
									IndexOfCartProduct = 0,
									ItemQuantity = 1,
								},
								new
								{
									IndexOfCartProduct = 1,
									ItemQuantity = 1,
								},
							},
						},
						new
						{
							ProductCountsParams = new[]
							{
								new
								{
									IndexOfCartProduct = 1,
									ItemQuantity = 1,
								},
							}
						}
					},
				},
				new
				{
					ItemPriceRegulationsExpected = new[] { 0m, 0m, },
					ProductCountsRegulationExpected = new[]
					{
						new[] { 0m,0m, },
						new[] { 0m }
					},
					ShippingDiscountAmount = 0m,
					PaymentDiscountAmount = 0m,
				},
				"調整金額0円"
			},
			// 注文合計金額0円
			new object[]
			{
				new
				{
					GiftOptionEnabled = true
				},
				new
				{
					RegulationPrice = 100m,
					ShippingPrice = 0m,
					PaymentPrice = 0m,
					CartItemParams = new[]
					{
						new
						{
							ProductPrice = 0m,
							ItemQuantity = 1,
						},
						new
						{
							ProductPrice = 0m,
							ItemQuantity = 2,
						},
					},
					ShippingParams = new[]
					{
						new
						{
							ProductCountsParams =  new[]
							{
								new
								{
									IndexOfCartProduct = 0,
									ItemQuantity = 1,
								},
								new
								{
									IndexOfCartProduct = 1,
									ItemQuantity = 1,
								},
							},
						},
						new
						{
							ProductCountsParams = new[]
							{
								new
								{
									IndexOfCartProduct = 1,
									ItemQuantity = 1,
								},
							}
						}
					},
				},
				new
				{
					ItemPriceRegulationsExpected = new [] { 0m, 0m, },
					ProductCountsRegulationExpected = new[]
					{
						new[] { 0m,0m, },
						new[] { 0m }
					},
					ShippingDiscountAmount = 0m,
					PaymentDiscountAmount = -100m,
				},
				"注文合計金額0円"
			},
			// 按分後小数点以下端数金額無し
			new object[]
			{
				new
				{
					GiftOptionEnabled = true
				},
				new
				{
					RegulationPrice = 100m,
					ShippingPrice = 3000m,
					PaymentPrice = 4000m,
					CartItemParams = new[]
					{
						new
						{
							ProductPrice = 1000m,
							ItemQuantity = 1,
						},
						new
						{
							ProductPrice = 1000m,
							ItemQuantity = 2,
						},
					},
					ShippingParams = new[]
					{
						new
						{
							ProductCountsParams =  new[]
							{
								new
								{
									IndexOfCartProduct = 0,
									ItemQuantity = 1,
								},
								new
								{
									IndexOfCartProduct = 1,
									ItemQuantity = 1,
								},
							},
						},
						new
						{
							ProductCountsParams = new[]
							{
								new
								{
									IndexOfCartProduct = 1,
									ItemQuantity = 1,
								},
							}
						}
					},
				},
				new
				{
					ItemPriceRegulationsExpected = new[] { 10m, 20m, },
					ProductCountsRegulationExpected = new[]
					{
						new[] { 10m,10m, },
						new[] { 10m }
					},
					ShippingDiscountAmount = -30m,
					PaymentDiscountAmount = -40m,
				},
				"按分後小数点以下端数金額無し"
			},
			// 按分後小数点以下端数金額あり：商品に重み付け
			new object[]
			{
				new
				{
					GiftOptionEnabled = true
				},
				new
				{
					RegulationPrice = 99m,
					ShippingPrice = 3000m,
					PaymentPrice = 4000m,
					CartItemParams = new[]
					{
						new
						{
							ProductPrice = 5000m,
							ItemQuantity = 1,
						},
						new
						{
							ProductPrice = 1000m,
							ItemQuantity = 2,
						},
					},
					ShippingParams = new[]
					{
						new
						{
							ProductCountsParams =  new[]
							{
								new
								{
									IndexOfCartProduct = 0,
									ItemQuantity = 1,
								},
								new
								{
									IndexOfCartProduct = 1,
									ItemQuantity = 1,
								},
							},
						},
						new
						{
							ProductCountsParams = new[]
							{
								new
								{
									IndexOfCartProduct = 1,
									ItemQuantity = 1,
								},
							}
						}
					},
				},
				new
				{
					ItemPriceRegulationsExpected = new[] { 36m, 14m, },
					ProductCountsRegulationExpected = new[]
					{
						new[] { 36m, 7m, },
						new[] { 7m }
					},
					ShippingDiscountAmount = -21m,
					PaymentDiscountAmount = -28m,
				},
				"按分後小数点以下端数金額あり：商品に重み付け"
			},
			// 按分後小数点以下端数金額無し：送料に重み付け
			new object[]
			{
				new
				{
					GiftOptionEnabled = true
				},
				new
				{
					RegulationPrice = 99m,
					ShippingPrice = 3000m,
					PaymentPrice = 4000m,
					CartItemParams = new[]
					{
						new
						{
							ProductPrice = 0m,
							ItemQuantity = 1,
						},
						new
						{
							ProductPrice = 0m,
							ItemQuantity = 2,
						},
					},
					ShippingParams = new[]
					{
						new
						{
							ProductCountsParams =  new[]
							{
								new
								{
									IndexOfCartProduct = 0,
									ItemQuantity = 1,
								},
								new
								{
									IndexOfCartProduct = 1,
									ItemQuantity = 1,
								},
							},
						},
						new
						{
							ProductCountsParams = new[]
							{
								new
								{
									IndexOfCartProduct = 1,
									ItemQuantity = 1,
								},
							}
						}
					},
				},
				new
				{
					ItemPriceRegulationsExpected = new[] { 0m, 0m, },
					ProductCountsRegulationExpected = new[]
					{
						new[] { 0m, 0m, },
						new[] { 0m }
					},
					ShippingDiscountAmount = -43m,
					PaymentDiscountAmount = -56m
				},
				"按分後小数点以下端数金額無し：送料に重み付け"
			},
		};

		/// <summary>
		/// セットプロモーション配送料・決済手数料割引額計算
		/// ・決済手数料無料フラグが設定されたセットプロモーションが存在する場合、決済手数料の料金分が決済手数料割引額に設定されること
		/// ・配送料無料フラグが設定されたセットプロモーションが存在する、もしくは、送料無料のクーポンが存在する場合、配送料の料金分が配送料割引額に設定されること
		/// </summary>
		[DataTestMethod()]
		[DynamicData("m_tdCalculateSetPromotionShippingaAndPaymentDiscountPriceTest")]
		public void CalculateSetPromotionShippingaAndPaymentDiscountPriceTest(
			dynamic config,
			dynamic data,
			dynamic expected,
			string msg)
		{
			using (new TestConfigurator(Member.Of(() => Constants.W2MP_COUPON_OPTION_ENABLED), true))
			{
				// モックによるドメイン偽装
				var productTagMock = new Mock<IProductTagService>();
				productTagMock.Setup(s => s.GetProductTagSetting()).Returns(new ProductTagSettingModel[0]);
				DomainFacade.Instance.ProductTagService = productTagMock.Object;
				var memnberRankMock = new Mock<IMemberRankService>();
				memnberRankMock.Setup(s => s.GetMemberRankList()).Returns(new MemberRankModel[0]);
				DomainFacade.Instance.MemberRankService = memnberRankMock.Object;

				var cart = CartTestHelper.CreateCart();
				cart.Owner = CartTestHelper.CreateCartOwner();
				cart.GetShipping().UpdateShippingAddr(cart.Owner, true);
				var cartProduct = CartTestHelper.CreateCartProduct(
					productPrice: data.ProductParams.ProductPrice,
					productCount: data.ProductParams.ItemQuantity,
					addCartKbn: Constants.AddCartKbn.GiftOrder);
				cart.Items.Add(cartProduct);
				cart.EnteredShippingPrice = data.ShippingPrice;
				cart.EnteredPaymentPrice = data.PaymentPrice;
				var setpromotionItemList = new List<CartSetPromotion.Item>();
				setpromotionItemList.Add(new CartSetPromotion.Item(cartProduct, data.ProductParams.ItemQuantity));
				cartProduct.QuantityAllocatedToSet.Add(1, data.ProductParams.ItemQuantity);
				var setpromotionmodel = new SetPromotionModel
				{
					IsDiscountTypeProductDiscount = true,
					IsDiscountTypeShippingChargeFree = data.IsDiscountTypeShippingChargeFree,
					IsDiscountTypePaymentChargeFree = data.IsDiscountTypePaymentChargeFree,
				};
				var cartsetPromotion = new CartSetPromotion(cart, setpromotionmodel, setpromotionItemList);
				cartsetPromotion.SetCount = 1;
				cart.SetPromotions.AddSetPromotion(cartsetPromotion);
				cart.Coupon = data.CartCoupon;

				new PrivateObject(cart).Invoke("CalculateSetPromotionShippingaAndPaymentDiscountPrice");

				// セットプロモーション配送料・決済手数料割引額計算
				// ・決済手数料無料フラグが設定されたセットプロモーションが存在する場合、決済手数料の料金分が決済手数料割引額に設定されること
				// ・配送料無料フラグが設定されたセットプロモーションが存在する、かつ、送料無料のクーポンが存在しない場合、配送料の料金分が配送料割引額に設定されること
				cart.PaymentPriceDiscountAmount.Should().Be(
					(decimal)expected.PaymentPriceDiscountAmount,
					string.Format("{0}:決済手数料割引額", msg));
				cart.ShippingPriceDiscountAmount.Should().Be(
					(decimal)expected.ShippingPriceDiscountAmount,
					string.Format("{0}:配送料割引額", msg));
			}
		}

		public static object[] m_tdCalculateSetPromotionShippingaAndPaymentDiscountPriceTest = new[]
		{
			// セットプロモーション設定「配送料無料：OFF、決済手数料割引：OFF」
			new object[]
			{
				new {},
				new
				{
					ShippingPrice = 10m,
					PaymentPrice = 100m,
					IsDiscountTypeShippingChargeFree = false,
					IsDiscountTypePaymentChargeFree = false,
					ProductParams = new
					{
						ProductPrice = 1000m,
						ItemQuantity = 1,
					},
					CartCoupon = new CartCoupon(
						new UserCouponDetailInfo
						{
							CouponType = Constants.FLG_COUPONCOUPON_TYPE_USERREGIST,
							PublishDateBgn = DateTime.Now,
							PublishDateEnd = DateTime.Now,
							DateCreated = DateTime.Now,
							DateChanged = DateTime.Now,
							ExceptionalIcon1 = 0,
							ExceptionalIcon2 = 0,
							ExceptionalIcon3 = 0,
							ExceptionalIcon4 = 0,
							ExceptionalIcon5 = 0,
							ExceptionalIcon6 = 0,
							ExceptionalIcon7 = 0,
							ExceptionalIcon8 = 0,
							ExceptionalIcon9 = 0,
							ExceptionalIcon10 = 0,
						}),
				},
				new
				{
					ShippingPriceDiscountAmount = 0m,
					PaymentPriceDiscountAmount = 0m,
				},
				"セットプロモーション設定「配送料無料：OFF、決済手数料割引：OFF」"
			},
			// セットプロモーション設定「配送料無料：ON、決済手数料割引：OFF」
			new object[]
			{
				new {},
				new
				{
					ShippingPrice = 10m,
					PaymentPrice = 100m,
					IsDiscountTypeShippingChargeFree = true,
					IsDiscountTypePaymentChargeFree = false,
					ProductParams = new
					{
						ProductPrice = 1000m,
						ItemQuantity = 1,
					},
					CartCoupon = new CartCoupon(
						new UserCouponDetailInfo
						{
							CouponType = Constants.FLG_COUPONCOUPON_TYPE_USERREGIST,
							PublishDateBgn = DateTime.Now,
							PublishDateEnd = DateTime.Now,
							DateCreated = DateTime.Now,
							DateChanged = DateTime.Now,
							ExceptionalIcon1 = 0,
							ExceptionalIcon2 = 0,
							ExceptionalIcon3 = 0,
							ExceptionalIcon4 = 0,
							ExceptionalIcon5 = 0,
							ExceptionalIcon6 = 0,
							ExceptionalIcon7 = 0,
							ExceptionalIcon8 = 0,
							ExceptionalIcon9 = 0,
							ExceptionalIcon10 = 0,
						}),
				},
				new
				{
					ShippingPriceDiscountAmount = 10m,
					PaymentPriceDiscountAmount = 0m,
				},
				"セットプロモーション設定「配送料無料：ON、決済手数料割引：OFF」"
			},
			// セットプロモーション設定「配送料無料：OFF、決済手数料割引：ON」
			new object[]
			{
				new {},
				new
				{
					ShippingPrice = 10m,
					PaymentPrice = 100m,
					IsDiscountTypeShippingChargeFree = false,
					IsDiscountTypePaymentChargeFree = true,
					ProductParams = new
					{
						ProductPrice = 1000m,
						ItemQuantity = 1,
					},
					CartCoupon = new CartCoupon(
						new UserCouponDetailInfo
						{
							CouponType = Constants.FLG_COUPONCOUPON_TYPE_USERREGIST,
							PublishDateBgn = DateTime.Now,
							PublishDateEnd = DateTime.Now,
							DateCreated = DateTime.Now,
							DateChanged = DateTime.Now,
							ExceptionalIcon1 = 0,
							ExceptionalIcon2 = 0,
							ExceptionalIcon3 = 0,
							ExceptionalIcon4 = 0,
							ExceptionalIcon5 = 0,
							ExceptionalIcon6 = 0,
							ExceptionalIcon7 = 0,
							ExceptionalIcon8 = 0,
							ExceptionalIcon9 = 0,
							ExceptionalIcon10 = 0,
						}),
				},
				new
				{
					ShippingPriceDiscountAmount = 0m,
					PaymentPriceDiscountAmount = 100m,
				},
				"セットプロモーション設定「配送料無料：OFF、決済手数料割引：ON」"
			},
			// セットプロモーション設定「配送料無料：ON、決済手数料割引：ON」
			new object[]
			{
				new {},
				new
				{
					ShippingPrice = 10m,
					PaymentPrice = 100m,
					IsDiscountTypeShippingChargeFree = true,
					IsDiscountTypePaymentChargeFree = true,
					ProductParams = new
					{
						ProductPrice = 1000m,
						ItemQuantity = 1,
					},
					CartCoupon = new CartCoupon(
						new UserCouponDetailInfo
						{
							CouponType = Constants.FLG_COUPONCOUPON_TYPE_USERREGIST,
							PublishDateBgn = DateTime.Now,
							PublishDateEnd = DateTime.Now,
							DateCreated = DateTime.Now,
							DateChanged = DateTime.Now,
							ExceptionalIcon1 = 0,
							ExceptionalIcon2 = 0,
							ExceptionalIcon3 = 0,
							ExceptionalIcon4 = 0,
							ExceptionalIcon5 = 0,
							ExceptionalIcon6 = 0,
							ExceptionalIcon7 = 0,
							ExceptionalIcon8 = 0,
							ExceptionalIcon9 = 0,
							ExceptionalIcon10 = 0,
						}),
				},
				new
				{
					ShippingPriceDiscountAmount = 10m,
					PaymentPriceDiscountAmount = 100m,
				},
				"セットプロモーション設定「配送料無料：ON、決済手数料割引：ON」"
			},
			// セットプロモーション設定「配送料無料：ON」、クーポン設定「クーポン種別：新規会員登録時発行クーポン」
			new object[]
			{
				new {},
				new
				{
					ShippingPrice = 10m,
					PaymentPrice = 100m,
					IsDiscountTypeShippingChargeFree = true,
					IsDiscountTypePaymentChargeFree = false,
					ProductParams = new
					{
						ProductPrice = 1000m,
						ItemQuantity = 1,
					},
					CartCoupon = new CartCoupon(
						new UserCouponDetailInfo
						{
							CouponType = Constants.FLG_COUPONCOUPON_TYPE_USERREGIST,
							PublishDateBgn = DateTime.Now,
							PublishDateEnd = DateTime.Now,
							DateCreated = DateTime.Now,
							DateChanged = DateTime.Now,
							ExceptionalIcon1 = 0,
							ExceptionalIcon2 = 0,
							ExceptionalIcon3 = 0,
							ExceptionalIcon4 = 0,
							ExceptionalIcon5 = 0,
							ExceptionalIcon6 = 0,
							ExceptionalIcon7 = 0,
							ExceptionalIcon8 = 0,
							ExceptionalIcon9 = 0,
							ExceptionalIcon10 = 0,
						}),
				},
				new
				{
					ShippingPriceDiscountAmount = 10m,
					PaymentPriceDiscountAmount = 0m,
				},
				"セットプロモーション設定「配送料無料：ON」、クーポン設定「クーポン種別：新規会員登録時発行クーポン」"
			},
			// セットプロモーション設定「配送料無料：OFF」、クーポン設定「クーポン種別：配送料無料クーポン」
			new object[]
			{
				new {},
				new
				{
					ShippingPrice = 10m,
					PaymentPrice = 100m,
					IsDiscountTypeShippingChargeFree = false,
					IsDiscountTypePaymentChargeFree = false,
					ProductParams = new
					{
						ProductPrice = 1000m,
						ItemQuantity = 1,
					},
					CartCoupon = new CartCoupon(
						new UserCouponDetailInfo
						{
							CouponType = Constants.FLG_COUPONCOUPON_TYPE_BLACKLIST_FREESHIPPING_FOR_REGISTERED_USER,
							PublishDateBgn = DateTime.Now,
							PublishDateEnd = DateTime.Now,
							DateCreated = DateTime.Now,
							DateChanged = DateTime.Now,
							ExceptionalIcon1 = 0,
							ExceptionalIcon2 = 0,
							ExceptionalIcon3 = 0,
							ExceptionalIcon4 = 0,
							ExceptionalIcon5 = 0,
							ExceptionalIcon6 = 0,
							ExceptionalIcon7 = 0,
							ExceptionalIcon8 = 0,
							ExceptionalIcon9 = 0,
							ExceptionalIcon10 = 0,
						}),
				},
				new
				{
					ShippingPriceDiscountAmount = 10m,
					PaymentPriceDiscountAmount = 0m,
				},
				"セットプロモーション設定「配送料無料：ON」、クーポン設定「クーポン種別：配送料無料クーポン」"
			},
		};

		/// <summary>
		/// 税率毎価格計算：配送先一つ
		/// ・税率毎に商品合計金額・配送料・決済手数料・消費税額を計算し、税率毎価格情報を生成する。
		/// ・免税の場合は商品消費税額が0円となること
		/// ・消費税の小数点以下端数処理(切捨て・切り上げ・四捨五入)は引数で指定された方法で実施する。引数に空が指定された場合は設定値TAX_EXCLUDED_FRACTION_ROUNDINGの方法で実施する
		/// ・各プロパティの計算方法は、適格請求書保存方式に対応できるように税率毎に税込金額を格納し、消費税額は税率毎の税込合計金額から算出されていること。
		///   ・CartPriceInfoByTaxRate.PriceSubtotal：税率毎の「CartProduct.PriceSubtotalAfterDistribution + CartProduct.ItemPriceRegulation」の合計
		///   ・CartPriceInfoByTaxRate.PricePayment ：税率が該当する「CartObject.PaymentPriceForCalculationDiscountAndTax - CartObject.PaymentPriceDiscountAmount」
		///   ・CartPriceInfoByTaxRate.PriceShipping：税率が該当する「CartObject.ShippingPriceForCalculationDiscountAndTax - CartObject.ShippingPriceDiscountAmount」
		///   ・CartPriceInfoByTaxRate.ReturnPriceCorrection：税率毎の返品用金額補正（返品交換時に設定され、CalculateTaxPriceメソッド実行前の値を引き継ぐ）
		///   ・CartPriceInfoByTaxRate.PriceTotal：CartPriceInfoByTaxRateオブジェクト自身の「PriceSubtotal + PricePayment + PriceShipping + ReturnPriceCorrection」
		///   ・CartPriceInfoByTaxRate.TaxPrice：CartPriceInfoByTaxRateオブジェクト自身の「CartPriceInfoByTaxRate.PriceTotal * CartPriceInfoByTaxRate.TaxRate / (100 + CartPriceInfoByTaxRate.TaxRate)」
		///   ・CartObject.PriceTax：CartObjectに紐づくCartPriceInfoByTaxRate.TaxPriceの合計
		///   ・CartObject.PriceSubtotalTax：CartObjectに紐づくCartProduct.ItemPriceTaxの合計(商品毎に計算した消費税額の合計 ※この値のみ、商品ごとに消費税の小数点以下端数処理が行われている)
		/// </summary>
		[DataTestMethod()]
		[DynamicData("m_tdCalculateTaxPrice_ShingleShippings")]
		public void CalculateTaxPrice_ShingleShippings(
			dynamic config,
			dynamic data,
			dynamic expected,
			string msg)
		{
			// モックによるドメイン偽装
			var mock = new Mock<IProductTagService>();
			mock.Setup(s => s.GetProductTagSetting()).Returns(new ProductTagSettingModel[0]);
			DomainFacade.Instance.ProductTagService = mock.Object;

			using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), true))
			using (new TestConfigurator(Member.Of(() => Constants.OPERATIONAL_BASE_ISO_CODE), Constants.COUNTRY_ISO_CODE_JP))
			using (new TestConfigurator(Member.Of(() => Constants.GLOBAL_TRANSACTION_INCLUDED_TAX_FLAG), false))
			using (new TestConfigurator(Member.Of(() => Constants.TAX_EXCLUDED_FRACTION_ROUNDING),
				TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN))
			using (new TestConfigurator(Member.Of(() => Constants.GIFTORDER_OPTION_ENABLED), false))
			using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), config.ManagementIncludedTaxFlag))
			{
				var cart = CartTestHelper.CreateCart();
				cart.Owner = CartTestHelper.CreateCartOwner();
				cart.GetShipping().UpdateShippingAddr(cart.Owner, true);
				cart.GetShipping().ShippingCountryIsoCode = data.ShippingCountryIsoCode;
				foreach (var productParam in data.ProductParams)
				{
					var cartProduct = CartTestHelper.CreateCartProduct(
						productParam.ProductPrice,
						productParam.ItemQuantity,
						productParam.TaxRate);
					cartProduct.ItemPriceRegulation = productParam.ItemPriceRegulation;
					cartProduct.PriceSubtotalAfterDistribution = productParam.PriceSubtotalAfterDistribution;
					cart.Items.Add(cartProduct);
				}

				cart.EnteredShippingPrice = data.ShippingPrice;
				cart.ShippingPriceDiscountAmount = data.ShippingPriceDiscountAmount;
				cart.ShippingTaxRate = data.ShippingTaxRate;
				cart.EnteredPaymentPrice = data.PaymentPrice;
				cart.PaymentPriceDiscountAmount = data.PaymentPriceDiscountAmount;
				cart.PaymentTaxRate = data.PaymentTaxRate;
				cart.PriceInfoByTaxRate.Clear();
				cart.PriceInfoByTaxRate.AddRange(((CartPriceInfoByTaxRate[])data.PriceInfoByTaxRate).ToList());

				cart.CalculateTaxPrice(data.TaxExcludedFractionRounding);

				// 税率毎価格計算：配送先一つ
				// ・税率毎に商品合計金額・配送料・決済手数料・消費税額を計算し、税率毎価格情報を生成する。
				var allExceptedPriceInfoCreated = ((CartPriceInfoByTaxRate[])expected.PriceInfoByTaxRateExpected)
					.FirstOrDefault(pibtye =>
						(cart.PriceInfoByTaxRate.Any(pibty => pibtye.TaxRate == pibty.TaxRate) == false));
				allExceptedPriceInfoCreated.Should().BeNull("予期された税率の金額情報が生成されていません。");
				foreach (var priceInfo in cart.PriceInfoByTaxRate)
				{
					var priceInfoExpected = ((CartPriceInfoByTaxRate[])expected.PriceInfoByTaxRateExpected)
						.FirstOrDefault(pibtye => (pibtye.TaxRate == priceInfo.TaxRate));
					priceInfoExpected.Should().NotBeNull(string.Format(
						"{0}:税率{1}%:予期されない税率の金額情報",
						priceInfo.TaxRate,
						msg));
					priceInfo.PriceSubtotal.Should().Be(priceInfoExpected.PriceSubtotal,
						string.Format("{0}:税率{1}%:商品小計", priceInfo.TaxRate, msg));
					priceInfo.PriceShipping.Should().Be(priceInfoExpected.PriceShipping,
						string.Format("{0}:税率{1}%:配送料", priceInfo.TaxRate, msg));
					priceInfo.PricePayment.Should().Be(priceInfoExpected.PricePayment,
						string.Format("{0}:税率{1}%:決済手数料", priceInfo.TaxRate, msg));
					priceInfo.PriceTotal.Should().Be(priceInfoExpected.PriceTotal,
						string.Format("{0}:税率{1}%:合計", priceInfo.TaxRate, msg));
					priceInfo.ReturnPriceCorrection.Should().Be(priceInfoExpected.ReturnPriceCorrection,
						string.Format("{0}:税率{1}%:返品用金額補正", priceInfo.TaxRate, msg));
					priceInfo.TaxPrice.Should().Be(priceInfoExpected.TaxPrice,
						string.Format("{0}:税率{1}%:消費税額", priceInfo.TaxRate, msg));
				}

				cart.PriceTax.Should().Be(
					(decimal)expected.PriceTaxExpected,
					string.Format("{0}:カート消費税額合計", msg));
				cart.PriceSubtotalTax.Should().Be(
					(decimal)expected.PriceSubtotalTaxExpected,
					string.Format("{0}:カート商品税額小計", msg));
			}
		}

		public static object[] m_tdCalculateTaxPrice_ShingleShippings = new[]
		{
			// 国内配送・端数処理方法「切捨て」・税抜設定
			new object[]
			{
				new
				{
					ManagementIncludedTaxFlag = false
				},
				new
				{
					TaxExcludedFractionRounding = TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN,
					ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP,
					ShippingPrice = 115m,
					ShippingPriceDiscountAmount = 10m,
					ShippingTaxRate = 10m,
					PaymentPrice = 1105m,
					PaymentPriceDiscountAmount = 100m,
					PaymentTaxRate = 10m,
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 11005m,
							ItemQuantity = 1,
							TaxRate = 10m,
							ItemPriceRegulation = -500m,
							PriceSubtotalAfterDistribution = 10505m
						},
						new
						{
							ProductPrice = 110001m,
							ItemQuantity = 2,
							TaxRate = 8m,
							ItemPriceRegulation = -10000m,
							PriceSubtotalAfterDistribution = 210001m
						},
						new
						{
							ProductPrice = 1100001m,
							ItemQuantity = 3,
							TaxRate = 8m,
							ItemPriceRegulation = -100000m,
							PriceSubtotalAfterDistribution = 3100001m
						},
					},
					PriceInfoByTaxRate = new[]
					{
						new CartPriceInfoByTaxRate
						{
							TaxRate = 8m,
							PriceSubtotal = 0m,
							PricePayment = 0m,
							PriceShipping = 0m,
							ReturnPriceCorrection = 0m,
							PriceTotal = 0m,
							TaxPrice = 0m,
						},
						new CartPriceInfoByTaxRate
						{
							TaxRate = 10m,
							PriceSubtotal = 0m,
							PricePayment = 0m,
							PriceShipping = 0m,
							ReturnPriceCorrection = 0m,
							PriceTotal = 0m,
							TaxPrice = 0m,
						},
					}
				},
				new
				{
					PriceInfoByTaxRateExpected = new[]
					{
						new CartPriceInfoByTaxRate
						{
							TaxRate = 8m,
							PriceSubtotal = 3200002,
							PricePayment = 0m,
							PriceShipping = 0m,
							ReturnPriceCorrection = 0m,
							PriceTotal = 3200002m,
							// TaxPrice = 3200002(税率8%の税込み合計金額) * 8 / 108(税率による消費税額割り戻し算出) = 237037.1 →(小数点以下端数処理) 237037
							TaxPrice = 237037,
						},
						new CartPriceInfoByTaxRate
						{
							TaxRate = 10m,
							PriceSubtotal = 10005,
							PricePayment = 1005m,
							PriceShipping = 105m,
							ReturnPriceCorrection = 0m,
							PriceTotal = 11115,
							// TaxPrice = 11115(税率10%の税込み合計金額) * 10 / 110(税率による消費税額割り戻し算出) = 1010.4 →(小数点以下端数処理) 1010
							TaxPrice = 1010,
						},
					},
					// PriceTaxExpected = 237037(8%消費税額) + 1010(10%消費税額) = 238047
					PriceTaxExpected = 238047m,
					// PriceSubtotalTaxExpectedは、全商品の単価から計算した消費税額の合計
					// 商品1：11005(単価) * 0.1(税率) = 1100.5 →(小数点以下端数処理) 1100(単価の消費税額) * 1(個数) = 1100(商品消費税合計)
					// 商品2：110001(単価) * 0.08(税率) = 8800.08 →(小数点以下端数処理) 8800(単価の消費税額) * 2(個数) = 17600(商品消費税合計)
					// 商品3：1100001(単価) * 0.08(税率) = 88000.08 →(小数点以下端数処理) 88000(単価の消費税額) * 3(個数) = 264000(商品消費税合計)
					// PriceSubtotalTaxExpected = 1100 + 17600 + 264000 = 282700
					PriceSubtotalTaxExpected = 282700m,
				},
				"国内配送・端数処理方法「切捨て」・税抜設定"
			},
			// 海外配送・端数処理方法「切捨て」・税抜設定
			new object[]
			{
				new
				{
					ManagementIncludedTaxFlag = false
				},
				new
				{
					TaxExcludedFractionRounding = TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN,
					ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_US,
					ShippingPrice = 115m,
					ShippingPriceDiscountAmount = 10m,
					ShippingTaxRate = 10m,
					PaymentPrice = 1105m,
					PaymentPriceDiscountAmount = 100m,
					PaymentTaxRate = 10m,
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 11005m,
							ItemQuantity = 1,
							TaxRate = 10m,
							ItemPriceRegulation = -500m,
							PriceSubtotalAfterDistribution = 10505m
						},
						new
						{
							ProductPrice = 110001m,
							ItemQuantity = 2,
							TaxRate = 8m,
							ItemPriceRegulation = -10000m,
							PriceSubtotalAfterDistribution = 210001m
						},
						new
						{
							ProductPrice = 1100001m,
							ItemQuantity = 3,
							TaxRate = 8m,
							ItemPriceRegulation = -100000m,
							PriceSubtotalAfterDistribution = 3100001m
						},
					},
					PriceInfoByTaxRate = new[]
					{
						new CartPriceInfoByTaxRate
						{
							TaxRate = 8m,
							PriceSubtotal = 0m,
							PricePayment = 0m,
							PriceShipping = 0m,
							ReturnPriceCorrection = 0m,
							PriceTotal = 0m,
							TaxPrice = 0m,
						},
						new CartPriceInfoByTaxRate
						{
							TaxRate = 10m,
							PriceSubtotal = 0m,
							PricePayment = 0m,
							PriceShipping = 0m,
							ReturnPriceCorrection = 0m,
							PriceTotal = 0m,
							TaxPrice = 0m,
						},
					}
				},
				new
				{
					PriceInfoByTaxRateExpected = new[]
					{
						new CartPriceInfoByTaxRate
						{
							TaxRate = 8m,
							PriceSubtotal = 3200002,
							PricePayment = 0m,
							PriceShipping = 0m,
							ReturnPriceCorrection = 0m,
							PriceTotal = 3200002m,
							// 海外配送のため、商品にかかる消費税額は0となる
							TaxPrice = 0,
						},
						new CartPriceInfoByTaxRate
						{
							TaxRate = 10m,
							PriceSubtotal = 10005,
							PricePayment = 1005m,
							PriceShipping = 105m,
							ReturnPriceCorrection = 0m,
							PriceTotal = 11115,
							// 海外配送のため、商品にかかる消費税額は0となるが、決済手数料・配送料は消費税額を計算する
							// TaxPrice = 1110(PriceShipping + PricePayment) * 10 / 110 = 100.9 →(端数処理) 100
							TaxPrice = 100,
						},
					},
					PriceTaxExpected = 100m,
					PriceSubtotalTaxExpected = 0m,
				},
				"海外配送・端数処理方法「切捨て」・税抜設定"
			},
			// 国内配送・端数処理方法「四捨五入」・税抜設定
			new object[]
			{
				new
				{
					ManagementIncludedTaxFlag = false
				},
				new
				{
					TaxExcludedFractionRounding = TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_OFF,
					ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP,
					ShippingPrice = 115m,
					ShippingPriceDiscountAmount = 10m,
					ShippingTaxRate = 10m,
					PaymentPrice = 1105m,
					PaymentPriceDiscountAmount = 100m,
					PaymentTaxRate = 10m,
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 11005m,
							ItemQuantity = 1,
							TaxRate = 10m,
							ItemPriceRegulation = -500m,
							PriceSubtotalAfterDistribution = 10505m
						},
						new
						{
							ProductPrice = 110001m,
							ItemQuantity = 2,
							TaxRate = 8m,
							ItemPriceRegulation = -10000m,
							PriceSubtotalAfterDistribution = 210001m
						},
						new
						{
							ProductPrice = 1100001m,
							ItemQuantity = 3,
							TaxRate = 8m,
							ItemPriceRegulation = -100000m,
							PriceSubtotalAfterDistribution = 3100001m
						},
					},
					PriceInfoByTaxRate = new[]
					{
						new CartPriceInfoByTaxRate
						{
							TaxRate = 8m,
							PriceSubtotal = 0m,
							PricePayment = 0m,
							PriceShipping = 0m,
							ReturnPriceCorrection = 0m,
							PriceTotal = 0m,
							TaxPrice = 0m,
						},
						new CartPriceInfoByTaxRate
						{
							TaxRate = 10m,
							PriceSubtotal = 0m,
							PricePayment = 0m,
							PriceShipping = 0m,
							ReturnPriceCorrection = 0m,
							PriceTotal = 0m,
							TaxPrice = 0m,
						},
					}
				},
				new
				{
					PriceInfoByTaxRateExpected = new[]
					{
						new CartPriceInfoByTaxRate
						{
							TaxRate = 8m,
							PriceSubtotal = 3200002,
							PricePayment = 0m,
							PriceShipping = 0m,
							ReturnPriceCorrection = 0m,
							PriceTotal = 3200002m,
							TaxPrice = 237037,
						},
						new CartPriceInfoByTaxRate
						{
							TaxRate = 10m,
							PriceSubtotal = 10005,
							PricePayment = 1005m,
							PriceShipping = 105m,
							ReturnPriceCorrection = 0m,
							PriceTotal = 11115,
							TaxPrice = 1010,
						},
					},
					PriceTaxExpected = 238047m,
					PriceSubtotalTaxExpected = 282700m,
				},
				"国内配送・端数処理方法「四捨五入」・税抜設定"
			},
			// 国内配送・端数処理方法「切り上げ」・税抜設定
			new object[]
			{
				new
				{
					ManagementIncludedTaxFlag = false
				},
				new
				{
					TaxExcludedFractionRounding = TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_UP,
					ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP,
					ShippingPrice = 115m,
					ShippingPriceDiscountAmount = 10m,
					ShippingTaxRate = 10m,
					PaymentPrice = 1105m,
					PaymentPriceDiscountAmount = 100m,
					PaymentTaxRate = 10m,
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 11005m,
							ItemQuantity = 1,
							TaxRate = 10m,
							ItemPriceRegulation = -500m,
							PriceSubtotalAfterDistribution = 10505m
						},
						new
						{
							ProductPrice = 110001m,
							ItemQuantity = 2,
							TaxRate = 8m,
							ItemPriceRegulation = -10000m,
							PriceSubtotalAfterDistribution = 210001m
						},
						new
						{
							ProductPrice = 1100001m,
							ItemQuantity = 3,
							TaxRate = 8m,
							ItemPriceRegulation = -100000m,
							PriceSubtotalAfterDistribution = 3100001m
						},
					},
					PriceInfoByTaxRate = new[]
					{
						new CartPriceInfoByTaxRate
						{
							TaxRate = 8m,
							PriceSubtotal = 0m,
							PricePayment = 0m,
							PriceShipping = 0m,
							ReturnPriceCorrection = 0m,
							PriceTotal = 0m,
							TaxPrice = 0m,
						},
						new CartPriceInfoByTaxRate
						{
							TaxRate = 10m,
							PriceSubtotal = 0m,
							PricePayment = 0m,
							PriceShipping = 0m,
							ReturnPriceCorrection = 0m,
							PriceTotal = 0m,
							TaxPrice = 0m,
						},
					}
				},
				new
				{
					PriceInfoByTaxRateExpected = new[]
					{
						new CartPriceInfoByTaxRate
						{
							TaxRate = 8m,
							PriceSubtotal = 3200002m,
							PricePayment = 0m,
							PriceShipping = 0m,
							ReturnPriceCorrection = 0m,
							PriceTotal = 3200002m,
							TaxPrice = 237038m,
						},
						new CartPriceInfoByTaxRate
						{
							TaxRate = 10m,
							PriceSubtotal = 10005m,
							PricePayment = 1005m,
							PriceShipping = 105m,
							ReturnPriceCorrection = 0m,
							PriceTotal = 11115m,
							TaxPrice = 1011m,
						},
					},
					PriceTaxExpected = 238049m,
					PriceSubtotalTaxExpected = 282700m,
				},
				"国内配送・端数処理方法「四捨五入」・税抜設定"
			},
			// 国内配送・端数処理方法「切捨て」・税込設定
			new object[]
			{
				new
				{
					ManagementIncludedTaxFlag = true
				},
				new
				{
					TaxExcludedFractionRounding = TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN,
					ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP,
					ShippingPrice = 115m,
					ShippingPriceDiscountAmount = 10m,
					ShippingTaxRate = 10m,
					PaymentPrice = 1105m,
					PaymentPriceDiscountAmount = 100m,
					PaymentTaxRate = 10m,
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 11005m,
							ItemQuantity = 1,
							TaxRate = 10m,
							ItemPriceRegulation = -500m,
							PriceSubtotalAfterDistribution = 10505m
						},
						new
						{
							ProductPrice = 110001m,
							ItemQuantity = 2,
							TaxRate = 8m,
							ItemPriceRegulation = -10000m,
							PriceSubtotalAfterDistribution = 210001m
						},
						new
						{
							ProductPrice = 1100001m,
							ItemQuantity = 3,
							TaxRate = 8m,
							ItemPriceRegulation = -100000m,
							PriceSubtotalAfterDistribution = 3100001m
						},
					},
					PriceInfoByTaxRate = new[]
					{
						new CartPriceInfoByTaxRate
						{
							TaxRate = 8m,
							PriceSubtotal = 0m,
							PricePayment = 0m,
							PriceShipping = 0m,
							ReturnPriceCorrection = 0m,
							PriceTotal = 0m,
							TaxPrice = 0m,
						},
						new CartPriceInfoByTaxRate
						{
							TaxRate = 10m,
							PriceSubtotal = 0m,
							PricePayment = 0m,
							PriceShipping = 0m,
							ReturnPriceCorrection = 0m,
							PriceTotal = 0m,
							TaxPrice = 0m,
						},
					}
				},
				new
				{
					PriceInfoByTaxRateExpected = new[]
					{
						new CartPriceInfoByTaxRate
						{
							TaxRate = 8m,
							PriceSubtotal = 3200002m,
							PricePayment = 0m,
							PriceShipping = 0m,
							ReturnPriceCorrection = 0m,
							PriceTotal = 3200002m,
							TaxPrice = 237037m,
						},
						new CartPriceInfoByTaxRate
						{
							TaxRate = 10m,
							PriceSubtotal = 10005m,
							PricePayment = 1005m,
							PriceShipping = 105m,
							ReturnPriceCorrection = 0m,
							PriceTotal = 11115m,
							TaxPrice = 1010m,
						},
					},
					PriceTaxExpected = 238047m,
					PriceSubtotalTaxExpected = 261739m,
				},
				"国内配送・端数処理方法「切捨て」・税込設定"
			},
			// 国内配送・端数処理方法「切捨て」・税抜設定・返品用金額補正あり
			new object[]
			{
				new
				{
					ManagementIncludedTaxFlag = false
				},
				new
				{
					TaxExcludedFractionRounding = TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN,
					ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP,
					ShippingPrice = 115m,
					ShippingPriceDiscountAmount = 10m,
					ShippingTaxRate = 10m,
					PaymentPrice = 1105m,
					PaymentPriceDiscountAmount = 100m,
					PaymentTaxRate = 10m,
					ProductParams = new[]
					{
						new
						{
							ProductPrice = 11005m,
							ItemQuantity = 1,
							TaxRate = 10m,
							ItemPriceRegulation = -500m,
							PriceSubtotalAfterDistribution = 10505m
						},
						new
						{
							ProductPrice = 110001m,
							ItemQuantity = 2,
							TaxRate = 8m,
							ItemPriceRegulation = -10000m,
							PriceSubtotalAfterDistribution = 210001m
						},
						new
						{
							ProductPrice = 1100001m,
							ItemQuantity = 3,
							TaxRate = 8m,
							ItemPriceRegulation = -100000m,
							PriceSubtotalAfterDistribution = 3100001m
						},
					},
					PriceInfoByTaxRate = new[]
					{
						new CartPriceInfoByTaxRate
						{
							TaxRate = 8m,
							PriceSubtotal = 0m,
							PricePayment = 0m,
							PriceShipping = 0m,
							ReturnPriceCorrection = -100m,
							PriceTotal = 0m,
							TaxPrice = 0m,
						},
						new CartPriceInfoByTaxRate
						{
							TaxRate = 10m,
							PriceSubtotal = 0m,
							PricePayment = 0m,
							PriceShipping = 0m,
							ReturnPriceCorrection = -100m,
							PriceTotal = 0m,
							TaxPrice = 0m,
						},
					}
				},
				new
				{
					PriceInfoByTaxRateExpected = new[]
					{
						new CartPriceInfoByTaxRate
						{
							TaxRate = 8m,
							PriceSubtotal = 3200002m,
							PricePayment = 0m,
							PriceShipping = 0m,
							ReturnPriceCorrection = -100m,
							PriceTotal = 3199902m,
							TaxPrice = 237029m,
						},
						new CartPriceInfoByTaxRate
						{
							TaxRate = 10m,
							PriceSubtotal = 10005,
							PricePayment = 1005m,
							PriceShipping = 105m,
							ReturnPriceCorrection = -100m,
							PriceTotal = 11015m,
							TaxPrice = 1001m,
						},
					},
					PriceTaxExpected = 238030m,
					PriceSubtotalTaxExpected = 282700m,
				},
				"国内配送・端数処理方法「切捨て」・税抜設定・返品用金額補正あり"
			},
		};

		/// <summary>
		/// 税率毎価格計算：複数配送先
		/// ・税率毎に商品合計金額・配送料・決済手数料・消費税額を計算し、税率毎価格情報を生成されること。
		/// ・配送先毎に、国コードから免税判定が行われること。
		/// </summary>
		[DataTestMethod()]
		[DynamicData("m_tdCalculateTaxPrice_MultipleShippings")]
		public void CalculateTaxPrice_MultipleShippings(
			dynamic config,
			dynamic data,
			dynamic expected,
			string msg)
		{
			using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), false))
			using (new TestConfigurator(Member.Of(() => Constants.OPERATIONAL_BASE_ISO_CODE), Constants.COUNTRY_ISO_CODE_JP))
			using (new TestConfigurator(Member.Of(() => Constants.GLOBAL_TRANSACTION_INCLUDED_TAX_FLAG), false))
			using (new TestConfigurator(Member.Of(() => Constants.TAX_EXCLUDED_FRACTION_ROUNDING),
				TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN))
			using (new TestConfigurator(Member.Of(() => Constants.GIFTORDER_OPTION_ENABLED), true))
			{
				// モックによるドメイン偽装
				var mock = new Mock<IProductTagService>();
				mock.Setup(s => s.GetProductTagSetting()).Returns(new ProductTagSettingModel[0]);
				DomainFacade.Instance.ProductTagService = mock.Object;

				var cart = CartTestHelper.CreateCart();
				cart.Owner = CartTestHelper.CreateCartOwner();
				cart.Shippings.Clear();
				foreach (var cartItemParam in data.CartItemParams)
				{
					var cartProduct = CartTestHelper.CreateCartProduct(
						productPrice: cartItemParam.ProductPrice,
						productCount: cartItemParam.ItemQuantity,
						taxRate: cartItemParam.TaxRate,
						addCartKbn: Constants.AddCartKbn.GiftOrder);
					cartProduct.ItemPriceRegulation = cartItemParam.ItemPriceRegulation;
					cartProduct.PriceSubtotalAfterDistribution = cartItemParam.PriceSubtotalAfterDistribution;
					cart.Items.Add(cartProduct);
				}

				foreach (var shippingParam in data.ShippingParams)
				{
					var cartShipping = new CartShipping(cart);
					cartShipping.UpdateShippingAddr(cart.Owner, true);
					cartShipping.ShippingCountryIsoCode = shippingParam.ShippingCountryIsoCode;
					foreach (var productCountsParam in shippingParam.ProductCountsParams)
					{
						var productCounts = new CartShipping.ProductCount(
							cart.Items[productCountsParam.IndexOfCartProduct],
							productCountsParam.ItemQuantity);
						productCounts.PriceSubtotalAfterDistribution =
							productCountsParam.PriceSubtotalAfterDistribution;
						productCounts.ItemPriceRegulation = productCountsParam.ItemPriceRegulation;
						cartShipping.ProductCounts.Add(productCounts);
					}

					cart.Shippings.Add(cartShipping);
				}

				cart.EnteredShippingPrice = data.ShippingPrice;
				cart.ShippingPriceDiscountAmount = data.ShippingPriceDiscountAmount;
				cart.ShippingTaxRate = data.ShippingTaxRate;
				cart.EnteredPaymentPrice = data.PaymentPrice;
				cart.PaymentPriceDiscountAmount = data.PaymentPriceDiscountAmount;
				cart.PaymentTaxRate = data.PaymentTaxRate;

				cart.CalculateTaxPrice(data.TaxExcludedFractionRounding);

				// 税率毎価格計算：複数配送先
				// ・税率毎に商品合計金額・配送料・決済手数料・消費税額を計算し、税率毎価格情報を生成されること。
				// ・配送先毎に、国コードから免税判定が行われること。
				var allExceptedPriceInfoCreated = ((CartPriceInfoByTaxRate[])expected.PriceInfoByTaxRateExpected)
					.FirstOrDefault(pibtye =>
						(cart.PriceInfoByTaxRate.Any(pibty => pibtye.TaxRate == pibty.TaxRate) == false));
				allExceptedPriceInfoCreated.Should().BeNull("予期された税率の金額情報が生成されていません。");
				foreach (var priceInfo in cart.PriceInfoByTaxRate)
				{
					var priceInfoExpected = ((CartPriceInfoByTaxRate[])expected.PriceInfoByTaxRateExpected)
						.FirstOrDefault(pibtye => (pibtye.TaxRate == priceInfo.TaxRate));
					priceInfoExpected.Should().NotBeNull(string.Format(
						"{0}:税率{1}%:予期されない税率の金額情報",
						priceInfo.TaxRate,
						msg));
					priceInfo.PriceSubtotal.Should().Be(priceInfoExpected.PriceSubtotal,
						string.Format("{0}:税率{1}%:商品小計", priceInfo.TaxRate, msg));
					priceInfo.PriceShipping.Should().Be(priceInfoExpected.PriceShipping,
						string.Format("{0}:税率{1}%:配送料", priceInfo.TaxRate, msg));
					priceInfo.PricePayment.Should().Be(priceInfoExpected.PricePayment,
						string.Format("{0}:税率{1}%:決済手数料", priceInfo.TaxRate, msg));
					priceInfo.PriceTotal.Should().Be(priceInfoExpected.PriceTotal,
						string.Format("{0}:税率{1}%:合計", priceInfo.TaxRate, msg));
					priceInfo.ReturnPriceCorrection.Should().Be(priceInfoExpected.ReturnPriceCorrection,
						string.Format("{0}:税率{1}%:返品用金額補正", priceInfo.TaxRate, msg));
					priceInfo.TaxPrice.Should().Be(priceInfoExpected.TaxPrice,
						string.Format("{0}:税率{1}%:消費税額", priceInfo.TaxRate, msg));
				}

				cart.PriceTax.Should().Be(
					(decimal)expected.PriceTaxExpected,
					string.Format("{0}:カート消費税額合計", msg));
				cart.PriceSubtotalTax.Should().Be(
					(decimal)expected.PriceSubtotalTaxExpected,
					string.Format("{0}:カート商品税額小計", msg));
			}
		}

		public static object[] m_tdCalculateTaxPrice_MultipleShippings = new[]
		{
			// 複数配送先・全て国内配送
			new object[]
			{
				new {},
				new
				{
					TaxExcludedFractionRounding = TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN,
					ShippingPrice = 115m,
					ShippingPriceDiscountAmount = 10m,
					ShippingTaxRate = 10m,
					PaymentPrice = 1105m,
					PaymentPriceDiscountAmount = 100m,
					PaymentTaxRate = 10m,
					CartItemParams = new[]
					{
						new
						{
							ProductPrice = 11005m,
							ItemQuantity = 2,
							TaxRate = 10m,
							ItemPriceRegulation = -500m,
							PriceSubtotalAfterDistribution = 20505m,
						},
						new
						{
							ProductPrice = 110001m,
							ItemQuantity = 4,
							TaxRate = 8m,
							ItemPriceRegulation = -10000m,
							PriceSubtotalAfterDistribution = 410001m,
						},
					},
					ShippingParams = new[]
					{
						new
						{
							ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP,
							ProductCountsParams =  new[]
							{
								new
								{
									IndexOfCartProduct = 0,
									ItemQuantity = 2,
									ItemPriceRegulation = -500m,
									PriceSubtotalAfterDistribution = 20505m,
								},
								new
								{
									IndexOfCartProduct = 1,
									ItemQuantity = 2,
									ItemPriceRegulation = -5000m,
									PriceSubtotalAfterDistribution = 205001m,
								},
							},
						},
						new
						{
							ShippingCountryIsoCode =  Constants.COUNTRY_ISO_CODE_JP,
							ProductCountsParams = new[]
							{
								new
								{
									IndexOfCartProduct = 1,
									ItemQuantity = 2,
									ItemPriceRegulation = -5000m,
									PriceSubtotalAfterDistribution = 205000m,
								},
							}
						}
					},
				},
				new
				{
					PriceInfoByTaxRateExpected = new[]
					{
						new CartPriceInfoByTaxRate
						{
							TaxRate = 8m,
							PriceSubtotal = 400001m,
							PricePayment = 0m,
							PriceShipping = 0m,
							ReturnPriceCorrection = 0m,
							PriceTotal = 400001m,
							TaxPrice = 29629m,
						},
						new CartPriceInfoByTaxRate
						{
							TaxRate = 10m,
							PriceSubtotal = 20005m,
							PricePayment = 1005m,
							PriceShipping = 105m,
							ReturnPriceCorrection = 0m,
							PriceTotal = 21115m,
							TaxPrice = 1919m,
						},
					},
					PriceTaxExpected = 31548m,
					PriceSubtotalTaxExpected = 37400m,
				},
				"複数配送先・全て国内配送"
			},
			// 複数配送先・全て海外配送
			new object[]
			{
				new {},
				new
				{
					TaxExcludedFractionRounding = TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN,
					ShippingPrice = 115m,
					ShippingPriceDiscountAmount = 10m,
					ShippingTaxRate = 10m,
					PaymentPrice = 1105m,
					PaymentPriceDiscountAmount = 100m,
					PaymentTaxRate = 10m,
					CartItemParams = new[]
					{
						new
						{
							ProductPrice = 11005m,
							ItemQuantity = 2,
							TaxRate = 10m,
							ItemPriceRegulation = -500m,
							PriceSubtotalAfterDistribution = 20505m,
						},
						new
						{
							ProductPrice = 110001m,
							ItemQuantity = 4,
							TaxRate = 8m,
							ItemPriceRegulation = -10000m,
							PriceSubtotalAfterDistribution = 410001m,
						},
					},
					ShippingParams = new[]
					{
						new
						{
							ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_US,
							ProductCountsParams =  new[]
							{
								new
								{
									IndexOfCartProduct = 0,
									ItemQuantity = 2,
									ItemPriceRegulation = -500m,
									PriceSubtotalAfterDistribution = 20505m,
								},
								new
								{
									IndexOfCartProduct = 1,
									ItemQuantity = 2,
									ItemPriceRegulation = -5000m,
									PriceSubtotalAfterDistribution = 205001m,
								},
							},
						},
						new
						{
							ShippingCountryIsoCode =  Constants.COUNTRY_ISO_CODE_US,
							ProductCountsParams = new[]
							{
								new
								{
									IndexOfCartProduct = 1,
									ItemQuantity = 2,
									ItemPriceRegulation = -5000m,
									PriceSubtotalAfterDistribution = 205000m,
								},
							}
						}
					},
				},
				new
				{
					PriceInfoByTaxRateExpected = new[]
					{
						new CartPriceInfoByTaxRate
						{
							TaxRate = 8m,
							PriceSubtotal = 400001m,
							PricePayment = 0m,
							PriceShipping = 0m,
							ReturnPriceCorrection = 0m,
							PriceTotal = 400001m,
							TaxPrice = 0m,
						},
						new CartPriceInfoByTaxRate
						{
							TaxRate = 10m,
							PriceSubtotal = 20005m,
							PricePayment = 1005m,
							PriceShipping = 105m,
							ReturnPriceCorrection = 0m,
							PriceTotal = 21115m,
							TaxPrice = 100m,
						},
					},
					PriceTaxExpected = 100m,
					PriceSubtotalTaxExpected = 0m,
				},
				"複数配送先・全て海外配送"
			},
			// 複数配送先・一部は国内配送、一部は海外配送
			new object[]
			{
				new {},
				new
				{
					TaxExcludedFractionRounding = TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN,
					ShippingPrice = 115m,
					ShippingPriceDiscountAmount = 10m,
					ShippingTaxRate = 10m,
					PaymentPrice = 1105m,
					PaymentPriceDiscountAmount = 100m,
					PaymentTaxRate = 10m,
					CartItemParams = new[]
					{
						new
						{
							ProductPrice = 11005m,
							ItemQuantity = 2,
							TaxRate = 10m,
							ItemPriceRegulation = -500m,
							PriceSubtotalAfterDistribution = 20505m,
						},
						new
						{
							ProductPrice = 110001m,
							ItemQuantity = 4,
							TaxRate = 8m,
							ItemPriceRegulation = -10000m,
							PriceSubtotalAfterDistribution = 410001m,
						},
					},
					ShippingParams = new[]
					{
						new
						{
							ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP,
							ProductCountsParams =  new[]
							{
								new
								{
									IndexOfCartProduct = 0,
									ItemQuantity = 2,
									ItemPriceRegulation = -500m,
									PriceSubtotalAfterDistribution = 20505m,
								},
								new
								{
									IndexOfCartProduct = 1,
									ItemQuantity = 2,
									ItemPriceRegulation = -5000m,
									PriceSubtotalAfterDistribution = 205001m,
								},
							},
						},
						new
						{
							ShippingCountryIsoCode =  Constants.COUNTRY_ISO_CODE_US,
							ProductCountsParams = new[]
							{
								new
								{
									IndexOfCartProduct = 1,
									ItemQuantity = 2,
									ItemPriceRegulation = -5000m,
									PriceSubtotalAfterDistribution = 205000m,
								},
							}
						}
					},
				},
				new
				{
					PriceInfoByTaxRateExpected = new[]
					{
						new CartPriceInfoByTaxRate
						{
							TaxRate = 8m,
							PriceSubtotal = 400001m,
							PricePayment = 0m,
							PriceShipping = 0m,
							ReturnPriceCorrection = 0m,
							PriceTotal = 400001m,
							// 一部商品が海外配送のため、海外配送商品の商品価格は考慮せずに消費税額を計算する
							// TaxPrice = 200001(「ShippingCountryIsoCode = JP」の割引後価格) * 8 / 108 = 14814.8 →(端数処理) 14814
							TaxPrice = 14814m,
						},
						new CartPriceInfoByTaxRate
						{
							TaxRate = 10m,
							PriceSubtotal = 20005m,
							PricePayment = 1005m,
							PriceShipping = 105m,
							ReturnPriceCorrection = 0m,
							PriceTotal = 21115m,
							TaxPrice = 1919m,
						},
					},
					PriceTaxExpected = 16733m,
					PriceSubtotalTaxExpected = 19800m,
				},
				"複数配送先・一部は国内配送、一部は海外配送"
			},
		};

		/// <summary>
		/// 合計割引額(決済手数料割引を除く)
		/// ・「会員ランク割引」+「セットプロモーション商品割引」+「セットプロモーション配送料割引」+「クーポン割引」+「ポイント割引」+「定期会員割引」+「定期購入割引」
		/// </summary>
		[DataTestMethod()]
		public void TotalPriceDiscountTest()
		{
			// モックによるドメイン偽装
			var mock = new Mock<IProductTagService>();
			mock.Setup(s => s.GetProductTagSetting()).Returns(new ProductTagSettingModel[0]);
			DomainFacade.Instance.ProductTagService = mock.Object;

			var cart = CartTestHelper.CreateCart();
			cart.Owner = CartTestHelper.CreateCartOwner();
			var cartProduct = CartTestHelper.CreateCartProduct();
			cart.Items.Add(cartProduct);
			cart.GetShipping().UpdateShippingAddr(cart.Owner, true);
			cart.CalculatePriceSubTotal();
			CartTestHelper.CreateCartSetPromotion(
				cart,
				cart.Items,
				1,
				Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_KBN_DISCOUNT_PRICE,
				1);

			cart.MemberRankDiscount = 10;
			cart.UseCouponPrice = 100;
			cart.UsePointPrice = 1000;
			cart.SetPromotions.ShippingChargeDiscountAmount = 10000;
			cart.FixedPurchaseDiscount = 100000;
			cart.FixedPurchaseMemberDiscountAmount = 1000000;
			var result = cart.TotalPriceDiscount;

			// 合計割引額(決済手数料割引を除く)
			// ・「会員ランク割引」+「セットプロモーション商品割引」+「セットプロモーション配送料割引」+「クーポン割引」+「ポイント割引」+「定期会員割引」+「定期購入割引」
			result.Should().Be(1111111, "合計割引額");
		}

		/// <summary>
		/// ポイント利用可能額取得テスト
		/// ・商品合計金額(税込み) - 会員ランク割引額 - クーポン割引額（商品割引額）- セットプロモーション商品割引額 - 定期購入割引額 - 定期会員割引額
		/// </summary>
		[DataTestMethod()]
		public void PointUsablePriceTest()
		{
			using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), false))
			using (new TestConfigurator(Member.Of(() => Constants.OPERATIONAL_BASE_ISO_CODE), Constants.COUNTRY_ISO_CODE_JP))
			using (new TestConfigurator(Member.Of(() => Constants.GLOBAL_TRANSACTION_INCLUDED_TAX_FLAG), false))
			using (new TestConfigurator(Member.Of(() => Constants.TAX_EXCLUDED_FRACTION_ROUNDING),
				TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN))
			using (new TestConfigurator(Member.Of(() => Constants.GIFTORDER_OPTION_ENABLED), false))
			{

				// モックによるドメイン偽装
				var mock = new Mock<IProductTagService>();
				mock.Setup(s => s.GetProductTagSetting()).Returns(new ProductTagSettingModel[0]);
				DomainFacade.Instance.ProductTagService = mock.Object;

				var cart = CartTestHelper.CreateCart();
				cart.Owner = CartTestHelper.CreateCartOwner();
				var cartProduct =
					CartTestHelper.CreateCartProduct(productPrice: 1000000, productCount: 1, taxRate: 10m);
				cart.Items.Add(cartProduct);
				cart.GetShipping().UpdateShippingAddr(cart.Owner, true);
				cart.GetShipping().ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP;
				cart.CalculatePriceSubTotal();
				cart.SetPriceSubtotal(1000000, 100000);
				CartTestHelper.CreateCartSetPromotion(
					cart,
					cart.Items,
					1,
					Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_KBN_DISCOUNT_PRICE,
					1);

				cart.MemberRankDiscount = 10;
				cart.UseCouponPrice = 100;
				cart.FixedPurchaseDiscount = 1000;
				cart.FixedPurchaseMemberDiscountAmount = 10000;
				var result = cart.PointUsablePrice;

				// ポイント利用可能額取得テスト
				// ・商品合計金額(税込み) - 会員ランク割引額 - クーポン割引額（商品割引額）- セットプロモーション商品割引額 - 定期購入割引額 - 定期会員割引額
				result.Should().Be(1088889, "ポイント利用可能額");
			}
		}

		/// <summary>
		/// カート支払い金額合計(決済手数料除く)取得テスト
		/// ・商品合計(税込み) + 配送料 + 調整金額 - クーポン割引額 - ポイント利用料 - セットプロモーション商品割引額 - セットプロモーション配送料割引額 - 定期購入割引額 - 定期会員割引額
		/// </summary>
		[DataTestMethod()]
		public void PriceCartTotalWithoutPaymentPriceTest()
		{
			using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), false))
			using (new TestConfigurator(Member.Of(() => Constants.OPERATIONAL_BASE_ISO_CODE), Constants.COUNTRY_ISO_CODE_JP))
			using (new TestConfigurator(Member.Of(() => Constants.TAX_EXCLUDED_FRACTION_ROUNDING),
				TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN))
			{
				// モックによるドメイン偽装
				var mock = new Mock<IProductTagService>();
				mock.Setup(s => s.GetProductTagSetting()).Returns(new ProductTagSettingModel[0]);
				DomainFacade.Instance.ProductTagService = mock.Object;

				var cart = CartTestHelper.CreateCart();
				cart.Owner = CartTestHelper.CreateCartOwner();
				var cartProduct =
					CartTestHelper.CreateCartProduct(productPrice: 1000000000, productCount: 1, taxRate: 10m);
				cart.Items.Add(cartProduct);
				cart.GetShipping().UpdateShippingAddr(cart.Owner, true);
				cart.GetShipping().ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP;
				cart.CalculatePriceSubTotal();
				cart.SetPriceShipping(10000000000);
				cart.SetPriceSubtotal(1000000000, 100000000);
				var setpromotionItemList = new List<CartSetPromotion.Item>();
				setpromotionItemList.Add(new CartSetPromotion.Item(cartProduct, 1));
				cartProduct.QuantityAllocatedToSet.Add(1, 1);
				var setpromotionmodel = new SetPromotionModel
				{
					ProductDiscountKbn = Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_KBN_DISCOUNT_PRICE,
					ProductDiscountSetting = 1,
					IsDiscountTypeProductDiscount = true,
					IsDiscountTypeShippingChargeFree = true,
				};
				var cartsetPromotion = new CartSetPromotion(cart, setpromotionmodel, setpromotionItemList);
				cartsetPromotion.SetCount = 1;
				cart.SetPromotions.AddSetPromotion(cartsetPromotion);

				cart.MemberRankDiscount = 10;
				cart.UseCouponPrice = 100;
				cart.FixedPurchaseDiscount = 1000;
				cart.FixedPurchaseMemberDiscountAmount = 10000;
				cart.PriceRegulation = -100000;
				cart.PriceInfoByTaxRate.Add(new CartPriceInfoByTaxRate
				{
					ReturnPriceCorrection = -1000000
				});
				cart.UsePointPrice = 10000000;

				var result = cart.PriceCartTotalWithoutPaymentPrice;

				// カート支払い金額合計(決済手数料除く)取得テスト
				// ・商品合計(税込み) + 配送料 + 調整金額 - クーポン割引額 - ポイント利用料 - セットプロモーション商品割引額 - セットプロモーション配送料割引額 - 定期購入割引額 - 定期会員割引額
				result.Should().Be(1088888889, "カート支払い金額合計(決済手数料除く)");
			}
		}

		/// <summary>
		/// 支払い金額総合計取得テスト
		/// ・商品合計(税込み) + 配送料 + 調整金額 - クーポン割引額 - ポイント利用料 - セットプロモーション商品割引額 - セットプロモーション配送料割引額 - 定期購入割引額 - 定期会員割引額 + 決済手数料 - セットプロモーション決済手数料割引額
		/// </summary>
		[DataTestMethod()]
		[DynamicData("m_tdPriceTotalTest")]
		public void PriceTotalTest(
			dynamic config,
			dynamic data,
			decimal expected,
			string msg)
		{
			using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), false))
			using (new TestConfigurator(Member.Of(() => Constants.OPERATIONAL_BASE_ISO_CODE), Constants.COUNTRY_ISO_CODE_JP))
			using (new TestConfigurator(Member.Of(() => Constants.TAX_EXCLUDED_FRACTION_ROUNDING),
				TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN))
			{

				// モックによるドメイン偽装
				var mock = new Mock<IProductTagService>();
				mock.Setup(s => s.GetProductTagSetting()).Returns(new ProductTagSettingModel[0]);
				DomainFacade.Instance.ProductTagService = mock.Object;

				var cart = CartTestHelper.CreateCart();
				cart.Owner = CartTestHelper.CreateCartOwner();
				foreach (var cartItemParam in data.CartItemParams)
				{
					var cartProduct = CartTestHelper.CreateCartProduct(
						productPrice: cartItemParam.ProductPrice,
						productCount: cartItemParam.ItemQuantity,
						taxRate: cartItemParam.TaxRate);
					cart.Items.Add(cartProduct);
				}

				cart.GetShipping().UpdateShippingAddr(cart.Owner, true);
				cart.GetShipping().ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP;
				cart.CalculatePriceSubTotal();
				cart.SetPriceTotal(data.FiexedPriceTotal);
				cart.Payment = data.Payment;
				cart.SetPriceShipping(data.ShippingPrice);
				cart.SetPriceSubtotal(data.ItemPriceSubtotal, data.ItemPriceSubtotalTax);
				var setpromotionItemList = new List<CartSetPromotion.Item>();
				setpromotionItemList.Add(new CartSetPromotion.Item(cart.Items[0], 1));
				cart.Items[0].QuantityAllocatedToSet.Add(1, 1);
				var setpromotionmodel = new SetPromotionModel
				{
					ProductDiscountKbn = Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_KBN_DISCOUNT_PRICE,
					ProductDiscountSetting = data.SetPromotionProductDiscountSetting,
					IsDiscountTypeProductDiscount = true,
					IsDiscountTypeShippingChargeFree = data.SetPromotionShippingDiscountFlg,
					IsDiscountTypePaymentChargeFree = data.SetPromotionPaymentDiscountFlg
				};
				var cartsetPromotion = new CartSetPromotion(cart, setpromotionmodel, setpromotionItemList);
				cartsetPromotion.SetCount = 1;
				cart.SetPromotions.AddSetPromotion(cartsetPromotion);

				cart.MemberRankDiscount = data.MemberRankDiscount;
				cart.UseCouponPrice = data.UseCouponPrice;
				cart.FixedPurchaseDiscount = data.FixedPurchaseDiscount;
				cart.FixedPurchaseMemberDiscountAmount = data.FixedPurchaseMemberDiscountAmount;
				cart.PriceRegulation = data.PriceRegulation;
				cart.PriceInfoByTaxRate.Add(data.PriceInfoByTaxRate);
				cart.UsePointPrice = data.UsePointPrice;

				var result = cart.PriceTotal;

				// カート支払い金額合計取得テスト
				// ・商品合計(税込み) + 配送料 + 調整金額 - クーポン割引額 - ポイント利用料 - セットプロモーション商品割引額 - セットプロモーション配送料割引額 - 定期購入割引額 - 定期会員割引額 + 決済手数料 - セットプロモーション決済手数料割引額
				result.Should().Be(expected, "支払い金額総合計:" + msg);
			}
		}

		public static object[] m_tdPriceTotalTest = new[]
		{
			// 支払い金額総合計(上書き用)あり
			new object[]
			{
				new { },
				new
				{
					FiexedPriceTotal = 1000000000000m,
					Payment = CartTestHelper.CreateCartPayment(
						Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
						100000000000m),
					ShippingPrice = 10000000000m,
					ItemPriceSubtotal = 1000000000m,
					ItemPriceSubtotalTax = 100000000m,
					SetPromotionProductDiscountSetting = 10000000m,
					SetPromotionShippingDiscountFlg = false,
					SetPromotionPaymentDiscountFlg = false,
					MemberRankDiscount = 1000000m,
					UseCouponPrice = 100000m,
					FixedPurchaseDiscount = 10000m,
					FixedPurchaseMemberDiscountAmount = 1000m,
					PriceRegulation = -100m,
					PriceInfoByTaxRate = new CartPriceInfoByTaxRate
					{
						ReturnPriceCorrection = -10m
					},
					UsePointPrice = 1m,
					CartItemParams = new[]
					{
						new
						{
							ProductPrice = 1000000000m,
							ItemQuantity = 1,
							TaxRate = 10m,
						},
					},
				},
				// 支払い金額総合計(上書き用)が存在する場合はそのまま結果として取得される
				1000000000000m,
				"支払い金額総合計(上書き用)あり"
			},
			// 支払い金額総合計(上書き用)なし・決済手数料割引なし
			new object[]
			{
				new { },
				new
				{
					FiexedPriceTotal = -1m,
					Payment = CartTestHelper.CreateCartPayment(
						Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
						100000000000m),
					ShippingPrice = 10000000000m,
					ItemPriceSubtotal = 1000000000m,
					ItemPriceSubtotalTax = 100000000m,
					SetPromotionProductDiscountSetting = 10000000m,
					SetPromotionShippingDiscountFlg = false,
					SetPromotionPaymentDiscountFlg = false,
					MemberRankDiscount = 1000000m,
					UseCouponPrice = 100000m,
					FixedPurchaseDiscount = 10000m,
					FixedPurchaseMemberDiscountAmount = 1000m,
					PriceRegulation = -100m,
					PriceInfoByTaxRate = new CartPriceInfoByTaxRate
					{
						ReturnPriceCorrection = -10m
					},
					UsePointPrice = 1m,
					CartItemParams = new[]
					{
						new
						{
							ProductPrice = 1000000000m,
							ItemQuantity = 1,
							TaxRate = 10m,
						},
					},
				},
				// 
				111088888889m,
				"支払い金額総合計(上書き用)なし・決済手数料割引なし"
			},
			// 支払い金額総合計(上書き用)なし・決済手数料割引あり
			new object[]
			{
				new { },
				new
				{
					FiexedPriceTotal = -1m,
					Payment = CartTestHelper.CreateCartPayment(
						Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
						100000000000m),
					ShippingPrice = 10000000000m,
					ItemPriceSubtotal = 1000000000m,
					ItemPriceSubtotalTax = 100000000m,
					SetPromotionProductDiscountSetting = 10000000m,
					SetPromotionShippingDiscountFlg = false,
					SetPromotionPaymentDiscountFlg = true,
					MemberRankDiscount = 1000000m,
					UseCouponPrice = 100000m,
					FixedPurchaseDiscount = 10000m,
					FixedPurchaseMemberDiscountAmount = 1000m,
					PriceRegulation = -100m,
					PriceInfoByTaxRate = new CartPriceInfoByTaxRate
					{
						ReturnPriceCorrection = -10m
					},
					UsePointPrice = 1m,
					CartItemParams = new[]
					{
						new
						{
							ProductPrice = 1000000000m,
							ItemQuantity = 1,
							TaxRate = 10m,
						},
					},
				},
				11088888889m,
				"支払い金額総合計(上書き用)なし・決済手数料割引あり"
			},
			// 支払い金額総合計(上書き用)なし・カート決済情報がNULL
			new object[]
			{
				new { },
				new
				{
					FiexedPriceTotal = -1m,
					Payment = (CartPayment)null,
					ShippingPrice = 10000000000m,
					ItemPriceSubtotal = 1000000000m,
					ItemPriceSubtotalTax = 100000000m,
					SetPromotionProductDiscountSetting = 10000000m,
					SetPromotionShippingDiscountFlg = false,
					SetPromotionPaymentDiscountFlg = true,
					MemberRankDiscount = 1000000m,
					UseCouponPrice = 100000m,
					FixedPurchaseDiscount = 10000m,
					FixedPurchaseMemberDiscountAmount = 1000m,
					PriceRegulation = -100m,
					PriceInfoByTaxRate = new CartPriceInfoByTaxRate
					{
						ReturnPriceCorrection = -10m
					},
					UsePointPrice = 1m,
					CartItemParams = new[]
					{
						new
						{
							ProductPrice = 1000000000m,
							ItemQuantity = 1,
							TaxRate = 10m,
						},
					},
				},
				11088888889m,
				"支払い金額総合計(上書き用)なし・カート決済情報がNULL"
			},
		};

		///// <summary>
		///// 総獲得ポイント取得テスト
		///// ・総獲得ポイント = 購入後獲得ポイント + 初回購入特別獲得ポイント
		///// </summary>
		//[DataTestMethod()]
		//public void AddPointTest()
		//{
		//	using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), true))
		//	using (new TestConfigurator(Member.Of(() => Constants.OPERATIONAL_BASE_ISO_CODE),
		//		Constants.COUNTRY_ISO_CODE_JP))
		//	using (new TestConfigurator(Member.Of(() => Constants.GLOBAL_TRANSACTION_INCLUDED_TAX_FLAG),
		//		false))
		//	using (new TestConfigurator(Member.Of(() => Constants.W2MP_POINT_OPTION_ENABLED), true))
		//	using (new TestConfigurator(Member.Of(() => Constants.POINT_OPTION_USE_TAX_EXCLUDED_POINT),
		//		false))
		//	{

		//		// モックによるドメイン偽装
		//		var productServiceMock = new Mock<IProductTagService>();
		//		productServiceMock.Setup(s => s.GetProductTagSetting()).Returns(new ProductTagSettingModel[0]);
		//		DomainFacade.Instance.ProductTagService = productServiceMock.Object;

		//		var deliveryCompanyServiceMock = new Mock<IDeliveryCompanyService>();
		//		deliveryCompanyServiceMock.Setup(s => s.Get(It.IsAny<string>())).Returns(new DeliveryCompanyModel());
		//		DomainFacade.Instance.DeliveryCompanyService = deliveryCompanyServiceMock.Object;

		//		var numberingUtilityMock = new Mock<NumberingUtilityWrapper>();
		//		numberingUtilityMock.Setup(s => s.CreateNewNumber(It.IsAny<string>(), It.IsAny<string>())).Returns(1);
		//		NumberingUtilityWrapper.Instance = numberingUtilityMock.Object;

		//		const string MEMBER_RANK_ID = "rank001";

		//		var memberRankCacheData = new[]
		//		{
		//			new MemberRankModel
		//			{
		//				MemberRankId = MEMBER_RANK_ID,
		//				PointAddType = Constants.FLG_MEMBERRANK_POINT_ADD_TYPE_RATE,
		//				PointAddValue = 10m,
		//			}
		//		};
		//		var pointRuleCacheData = new[]
		//		{
		//			new PointRuleModel
		//			{
		//				PointRuleId = "testPoint1",
		//				PointIncKbn = Constants.FLG_POINTRULE_POINT_INC_KBN_BUY,
		//				IncType = Constants.FLG_POINTRULE_INC_TYPE_RATE,
		//				IncRate = 10m
		//			}
		//		};
		//		using (new ShopShippingDataCacheConfigurator(new ShopShippingModel[0]))
		//		using (new PointRuleDataCacheConfigurator(pointRuleCacheData, new PointRuleModel[0]))
		//		using (new MemberRankDataCacheConfigurator(memberRankCacheData))
		//		{
		//			var shopShippingServiceMock = new Mock<IShopShippingService>();
		//			shopShippingServiceMock.Setup(s => s.GetFromZipcode(
		//				It.IsAny<string>(),
		//				It.IsAny<string>(),
		//				It.IsAny<int>(),
		//				It.IsAny<string>(),
		//				It.IsAny<string>(),
		//				It.IsAny<int>())).Returns((ShopShippingModel) null);
		//			DomainFacade.Instance.ShopShippingService = shopShippingServiceMock.Object;

		//			var orderServiceMock = new Mock<IOrderService>();
		//			orderServiceMock.Setup(s =>
		//					s.HasUncancelledOrders(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SqlAccessor>()))
		//				.Returns(false);
		//			DomainFacade.Instance.OrderService = orderServiceMock.Object;

		//			var cart = CartTestHelper.CreateCart(memberRankId: MEMBER_RANK_ID);
		//			cart.Owner = CartTestHelper.CreateCartOwner();
		//			var cartProduct =
		//				CartTestHelper.CreateCartProduct(productPrice: 100, productCount: 1, taxRate: 10m);
		//			cart.Items.Add(cartProduct);
		//			cart.GetShipping().UpdateShippingAddr(cart.Owner, true);
		//			cart.GetShipping().ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP;
		//			cart.GetShipping().DeliveryCompanyId = "test001";
		//			cart.Calculate(false);
		//			cart.FirstBuyPoint = 1;

		//			var result = cart.AddPoint;

		//			// 総獲得ポイント取得テスト
		//			// 購入後獲得ポイント「100(商品価格) * 0.2(基本ポイントルール付与倍率10% + 会員ランクポイント付与倍率10%) = 20」 + 初回購入特別獲得ポイント「1」
		//			result.Should().Be(21, "総獲得ポイント");
		//		}
		//	}
		//}

		/// <summary>
		/// 外部決済種別設定テスト
		/// ・引数に指定されたユーザーデフォルト注文設定の決済種別が「ECPAY」「NEWEBPAY」かつ、CartPayment.ExternalPaymentTypeが空だった場合、初期値「Credit」が設定されること。
		/// ・引数に指定されたユーザーデフォルト注文設定の決済種別とCartPayment.PaymentIdが異なる場合、上記処理がスキップされること
		/// </summary>
		[DataTestMethod()]
		[DynamicData("m_tdSetDefaultExternalPaymentTypeTest")]
		public void SetDefaultExternalPaymentTypeTest(
			dynamic config,
			dynamic data,
			string expected,
			string msg)
		{
			var productTagServiceMock = new Mock<IProductTagService>();
			productTagServiceMock.Setup(s => s.GetProductTagSetting()).Returns(new ProductTagSettingModel[0]);
			DomainFacade.Instance.ProductTagService = productTagServiceMock.Object;

			var cart = CartTestHelper.CreateCart();

			cart.Owner = CartTestHelper.CreateCartOwner();
			cart.GetShipping().UpdateShippingAddr(cart.Owner, true);
			cart.GetShipping().DeliveryCompanyId = "test001";
			cart.Items.Add(CartTestHelper.CreateCartProduct());
			cart.Payment = data.CreateCartPaymentFunc();

			cart.SetDefaultExternalPaymentType((UserDefaultOrderSettingModel)data.UserDefaultOrderSettingModel);

			// 商品金額合計再計算：配送先一つのみ
			// ・商品単価と購入個数からカート商品小計が計算されること
			// ・「商品小計(割引金額の按分処理適用後)」が再計算されること
			// ・再計算前とPriceSubtotalプロパティの値が異なっていればTRUE、同一ならばFALSEが戻り値で帰ってくること
			cart.Payment.ExternalPaymentType.Should().Be(expected, msg + " : デフォルト外部連携種別設定");
		}

		public static object[] m_tdSetDefaultExternalPaymentTypeTest = new[]
		{
			// デフォルト外部決済種別設定処理：ECPAY
			// 外部決済種別未設定
			new object[]
			{
				new { },
				new
				{
					UserDefaultOrderSettingModel = new UserDefaultOrderSettingModel
					{
						PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY
					},
					CreateCartPaymentFunc = new Func<CartPayment>(() =>
					{
						var cartPayment = CartTestHelper.CreateCartPayment(Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY);
						cartPayment.ExternalPaymentType = "";
						return cartPayment;
					})
				},
				Constants.FLG_PAYMENT_TYPE_ECPAY_CREDIT,
				"外部決済種別設定処理:外部決済種別未設定:ECPAY"
			},
			// デフォルト外部決済種別設定処理：ECPAY
			// 外部決済種別設定済み
			new object[]
			{
				new { },
				new
				{
					UserDefaultOrderSettingModel = new UserDefaultOrderSettingModel
					{
						PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY
					},
					CreateCartPaymentFunc = new Func<CartPayment>(() =>
					{
						var cartPayment = CartTestHelper.CreateCartPayment(Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY);
						cartPayment.ExternalPaymentType = Constants.FLG_PAYMENT_TYPE_ECPAY_WEBATM;
						return cartPayment;
					})
				},
				Constants.FLG_PAYMENT_TYPE_ECPAY_WEBATM,
				"外部決済種別設定処理:外部決済種別設定済み:ECPAY"
			},
			// デフォルト外部決済種別設定処理：ECPAY
			// デフォルト決済種別が利用されていない
			new object[]
			{
				new { },
				new
				{
					UserDefaultOrderSettingModel = new UserDefaultOrderSettingModel
					{
						PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY
					},
					CreateCartPaymentFunc = new Func<CartPayment>(() =>
					{
						var cartPayment = CartTestHelper.CreateCartPayment(Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT);
						cartPayment.ExternalPaymentType = "";
						return cartPayment;
					})
				},
				"",
				"外部決済種別設定処理:デフォルト決済種別が利用されていない:ECPAY"
			},
			// デフォルト外部決済種別設定処理：NEWEBPAY
			// 外部決済種別未設定
			new object[]
			{
				new { },
				new
				{
					UserDefaultOrderSettingModel = new UserDefaultOrderSettingModel
					{
						PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY
					},
					CreateCartPaymentFunc = new Func<CartPayment>(() =>
					{
						var cartPayment = CartTestHelper.CreateCartPayment(Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY);
						cartPayment.ExternalPaymentType = "";
						return cartPayment;
					})
				},
				Constants.FLG_PAYMENT_TYPE_NEWEBPAY_CREDIT,
				"外部決済種別設定処理:外部決済種別未設定:NEWEBPAY"
			},
			// デフォルト外部決済種別設定処理：NEWEBPAY
			// 外部決済種別設定済み
			new object[]
			{
				new { },
				new
				{
					UserDefaultOrderSettingModel = new UserDefaultOrderSettingModel
					{
						PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY
					},
					CreateCartPaymentFunc = new Func<CartPayment>(() =>
					{
						var cartPayment = CartTestHelper.CreateCartPayment(Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY);
						cartPayment.ExternalPaymentType = Constants.FLG_PAYMENT_TYPE_NEWEBPAY_WEBATM;
						return cartPayment;
					})
				},
				Constants.FLG_PAYMENT_TYPE_NEWEBPAY_WEBATM,
				"外部決済種別設定処理:外部決済種別設定済み:NEWEBPAY"
			},
			// デフォルト外部決済種別設定処理：NEWEBPAY
			// デフォルト決済種別が利用されていない
			new object[]
			{
				new { },
				new
				{
					UserDefaultOrderSettingModel = new UserDefaultOrderSettingModel
					{
						PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY
					},
					CreateCartPaymentFunc = new Func<CartPayment>(() =>
					{
						var cartPayment = CartTestHelper.CreateCartPayment(Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT);
						cartPayment.ExternalPaymentType = "";
						return cartPayment;
					})
				},
				"",
				"外部決済種別設定処理:デフォルト決済種別が利用されていない:NEWEBPAY"
			},
		};

		///// <summary>
		///// ユーザーデフォルト注文設定の決済種別利用可能チェックのテスト
		///// ・利用可能決済種別情報をDBから取得し、ユーザーデフォルト注文設定の決済種別が含まれていなければtrue、含まれていればfalseを返すこと
		///// ・ユーザーデフォルト注文設定の決済種別とCartPayment.PaymentIdが異なる場合、falseが返されること
		///// </summary>
		//[DataTestMethod()]
		//[DynamicData("m_tdCheckDefaultOrderSettingPaymentIsValidTest")]
		//public void CheckDefaultOrderSettingPaymentIsValid(
		//	dynamic config,
		//	dynamic data,
		//	bool expected,
		//	string msg)
		//{
		//		using (new ShopShippingDataCacheConfigurator(new ShopShippingModel[0]))
		//		using (new MemberRankDataCacheConfigurator(new MemberRankModel[0]))
		//		using (new PaymentDataCacheConfigurator((PaymentModel[])data.ValidPaymentCachData))
		//		{
		//			var userServiceMock = new Mock<IUserService>();
		//			userServiceMock
		//				.Setup(s => s.Get(
		//					It.IsAny<string>(),
		//					It.IsAny<SqlAccessor>()))
		//				.Returns(new UserModel());
		//			DomainFacade.Instance.UserService = userServiceMock.Object;

		//			var deliveryCompanyServiceMock = new Mock<IDeliveryCompanyService>();
		//			deliveryCompanyServiceMock
		//			.Setup(s => s.Get(It.IsAny<string>()))
		//			.Returns(new DeliveryCompanyModel());
		//			DomainFacade.Instance.DeliveryCompanyService = deliveryCompanyServiceMock.Object;

		//			var productServiceMock = new Mock<IProductService>();
		//			productServiceMock
		//				.Setup(s => s.GetCartProducts(It.IsAny<string>()))
		//				.Returns(new[]
		//				{
		//					new ProductModel
		//					{
		//						LimitedPaymentIds = ""
		//					}
		//				});
		//			DomainFacade.Instance.ProductService = productServiceMock.Object;

		//			var productTagServiceMock = new Mock<IProductTagService>();
		//			productTagServiceMock.Setup(s => s.GetProductTagSetting()).Returns(new ProductTagSettingModel[0]);
		//			DomainFacade.Instance.ProductTagService = productTagServiceMock.Object;

		//			var cart = CartTestHelper.CreateCart();

		//			cart.Owner = CartTestHelper.CreateCartOwner();
		//			cart.GetShipping().UpdateShippingAddr(cart.Owner, true);
		//			cart.GetShipping().DeliveryCompanyId = "test001";
		//			cart.Items.Add(CartTestHelper.CreateCartProduct());
		//			cart.Payment = data.CreateCartPaymentFunc();

		//			cart.Calculate(false);

		//			var result = cart.CheckDefaultOrderSettingPaymentIsValid(
		//				"test001",
		//				(UserDefaultOrderSettingModel)data.UserDefaultOrderSettingModel,
		//				true);

		//			result.Should().Be((bool)expected, msg + " : ユーザーデフォルト決済種別利用可否判定");
		//		}
		//}

		public static object[] m_tdCheckDefaultOrderSettingPaymentIsValidTest = new[]
		{
			// デフォルト決済種別利用可能判定
			// 決済種別利用可能〇
			new object[]
			{
				new { },
				new
				{
					UserDefaultOrderSettingModel = new UserDefaultOrderSettingModel
					{
						PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT
					},
					ValidPaymentCachData = new[]
					{
						new PaymentModel
						{
							PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT,
							OrderOwnerKbnNotUse = "",
							UserManagementLevelNotUse = "",
							PriceList = new []
							{
								new PaymentPriceModel
								{
									TgtPriceBgn = 0m,
									TgtPriceEnd = 1001m,
								}
							}

						}
					},
					CreateCartPaymentFunc = new Func<CartPayment>(() =>
					{
						var cartPayment = CartTestHelper.CreateCartPayment(Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT);
						cartPayment.ExternalPaymentType = "";
						return cartPayment;
					})
				},
				false,
				"デフォルト決済種別利用可能判定：〇"
			},
			// デフォルト決済種別利用可能判定
			// 決済種別利用可能✕
			new object[]
			{
				new { },
				new
				{
					UserDefaultOrderSettingModel = new UserDefaultOrderSettingModel
					{
						PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT
					},
					ValidPaymentCachData = new[]
					{
						new PaymentModel
						{
							PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
							OrderOwnerKbnNotUse = "",
							UserManagementLevelNotUse = "",
							PriceList = new []
							{
								new PaymentPriceModel
								{
									TgtPriceBgn = 0m,
									TgtPriceEnd = 1001m,
								}
							}

						}
					},
					CreateCartPaymentFunc = new Func<CartPayment>(() =>
					{
						var cartPayment = CartTestHelper.CreateCartPayment(Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT);
						cartPayment.ExternalPaymentType = "";
						return cartPayment;
					})
				},
				true,
				"デフォルト決済種別利用可能判定：×"
			},
			// デフォルト決済種別利用可能判定
			// デフォルト決済種別と、カート決済情報に設定された決済種別が異なる
			new object[]
			{
				new
				{
					TwoClickOptionEnable = true
				},
				new
				{
					UserDefaultOrderSettingModel = new UserDefaultOrderSettingModel
					{
						PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT
					},
					ValidPaymentCachData = new[]
					{
						new PaymentModel
						{
							PaymentId = Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT,
							OrderOwnerKbnNotUse = "",
							UserManagementLevelNotUse = "",
							PriceList = new []
							{
								new PaymentPriceModel
								{
									TgtPriceBgn = 0m,
									TgtPriceEnd = 1001m,
								}
							}

						}
					},
					CreateCartPaymentFunc = new Func<CartPayment>(() =>
					{
						var cartPayment = CartTestHelper.CreateCartPayment();
						cartPayment.ExternalPaymentType = "";
						return cartPayment;
					})
				},
				false,
				"デフォルト決済種別利用可能判定：デフォルト決済種別が利用されていない"
			},
		};
	}
}
