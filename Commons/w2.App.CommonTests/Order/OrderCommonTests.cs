using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using w2.App.Common;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.CommonTests._Helper;
using w2.App.CommonTests._Helper.DataCacheConfigurator;
using w2.App.CommonTests.Order.Cart;
using w2.Common.Wrapper;
using w2.CommonTests._Helper;
using w2.Domain;
using w2.Domain.Coupon;
using w2.Domain.MemberRank;
using w2.Domain.Order;
using w2.Domain.Payment;
using w2.Domain.Product;
using w2.Domain.ProductTag;
using w2.Domain.ProductTaxCategory;
using w2.Domain.SetPromotion;
using w2.Domain.SubscriptionBox;
using Constants = w2.App.Common.Constants;

namespace w2.App.CommonTests.Order
{
	/// <summary>
	/// OrderCommonのテスト
	/// </summary>
	[TestClass()]
	public class OrderCommonTests : AppTestClassBase
	{
		/// <summary>
		/// セットプロモーション割引を注文情報の商品小計に按分する計算テスト
		/// </summary>
		[Ignore]
		[DataTestMethod()]
		[DynamicData("m_tdCalculateSetPromotionTest")]
		public void CalculateSetPromotionTest(
			dynamic config,
			dynamic data,
			dynamic expected,
			string msg)
		{
			using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), true))
			using (new TestConfigurator(Member.Of(() => Constants.OPERATIONAL_BASE_ISO_CODE), Constants.COUNTRY_ISO_CODE_JP))
			using (new TestConfigurator(Member.Of(() => Constants.GLOBAL_TRANSACTION_INCLUDED_TAX_FLAG), true))
			{
				var order = (OrderModel)data.Order;
				var result = new PrivateType(typeof(OrderCommon))
					.InvokeStatic(
						"CalculateSetPromotion",
						new object[]
						{
								order,
						});

				result.Should().Be((string)expected.ErrorMessage, string.Format("エラーメッセージ：{0}", msg));

				order.PaymentPriceDiscountAmount.Should().Be((decimal)expected.PaymentPriceDiscountAmount, string.Format("決済手数料割引金額：{0}", msg));
				order.ShippingPriceDiscountAmount.Should().Be((decimal)expected.ShippingPriceDiscountAmount, string.Format("配送料割引金額：{0}", msg));

				var targetTestItems = order.Shippings.SelectMany(shipping => shipping.Items).ToArray();
				for (var i = 0; i < targetTestItems.Length; i++)
				{
					targetTestItems[i].PriceSubtotalAfterDistribution.Should().Be((decimal)expected.PriceSubtotalAfterDistributions[i], string.Format("商品合計：{0}", msg));
				}
			}
		}

		public static object[] m_tdCalculateSetPromotionTest = new[]
		{
			new object[]
			{
				new { },
				new
				{
					Order = new OrderModel
					{
						OrderPriceRegulation = 0m,
						Shippings = new[]
						{
							new OrderShippingModel
							{
								Items = new[]
								{
									new OrderItemModel
									{
										ProductPricePretax = 100m,
										ItemQuantity = 1,
										OrderSetpromotionNo = 1,
									},
									new OrderItemModel
									{
										ProductPricePretax = 100m,
										ItemQuantity = 2,
										OrderSetpromotionNo = 2,
									},
								},
							}, 
						},
						SetPromotions = new[]
						{
							// 対象
							new OrderSetPromotionModel
							{
								OrderSetpromotionNo = 1,
								ProductDiscountFlg = Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_FLG_ON,
								UndiscountedProductSubtotal = 110m,
								ProductDiscountAmount = 10m,
							},
							// 対象外
							new OrderSetPromotionModel
							{
								OrderSetpromotionNo = 2,
								ProductDiscountFlg = Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_FLG_OFF,
								UndiscountedProductSubtotal = 2000m,
								ProductDiscountAmount = 20m,
							},
						}
					},
				},
				new
				{
					ErrorMessage = string.Empty,
					PaymentPriceDiscountAmount = 0m,
					ShippingPriceDiscountAmount = 0m,
					PriceSubtotalAfterDistributions = new[]
					{
						// 商品合計(割引按分後) = -(10(商品割引額) * (100(小計)/110(割引前商品合計)) + 1(商品割引額)) = -10
						-10m,
						0m,
					},
				},
				"商品割引あり、配送料割引なし、決済手数料割引なしパターン"
			},
			new object[]
			{
				new { },
				new
				{
					Order = new OrderModel
					{
						OrderPriceRegulation = 0m,
						OrderPriceExchange = 100m,
						OrderPriceShipping = 1000m,
						Shippings = new[]
						{
							new OrderShippingModel
							{
								Items = new[]
								{
									new OrderItemModel
									{
										ProductPricePretax = 100m,
										ItemQuantity = 1,
										OrderSetpromotionNo = 1,
									},
									new OrderItemModel
									{
										ProductPricePretax = 100m,
										ItemQuantity = 2,
										OrderSetpromotionNo = 2,
									},
								},
							},
						},
						SetPromotions = new[]
						{
							// 対象
							new OrderSetPromotionModel
							{
								OrderSetpromotionNo = 1,
								PaymentChargeFreeFlg = Constants.FLG_SETPROMOTION_PAYMENT_CHARGE_FREE_FLG_ON
							},
							// 対象
							new OrderSetPromotionModel
							{
								OrderSetpromotionNo = 2,
								ShippingChargeFreeFlg = Constants.FLG_SETPROMOTION_SHIPPING_CHARGE_FREE_FLG_ON
							},
						}
					},
				},
				new
				{
					ErrorMessage = string.Empty,
					PaymentPriceDiscountAmount = 100m,
					ShippingPriceDiscountAmount = 1000m,
					PriceSubtotalAfterDistributions = new[]
					{
						0m,
						0m,
					},
				},
				"商品割引なし、配送料割引あり、決済手数料割引ありパターン"
			},
			new object[]
			{
				new { },
				new
				{
					Order = new OrderModel
					{
						OrderPriceRegulation = 0m,
						OrderPriceExchange = 100m,
						OrderPriceShipping = 1000m,
						Shippings = new[]
						{
							new OrderShippingModel
							{
								Items = new[]
								{
									new OrderItemModel
									{
										ProductPricePretax = 100m,
										ItemQuantity = 1,
										OrderSetpromotionNo = 1,
									},
									new OrderItemModel
									{
										ProductPricePretax = 100m,
										ItemQuantity = 2,
										OrderSetpromotionNo = 2,
									},
								},
							},
						},
						SetPromotions = new[]
						{
							// 対象
							new OrderSetPromotionModel
							{
								OrderSetpromotionNo = 1,
								ProductDiscountFlg = Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_FLG_ON,
								UndiscountedProductSubtotal = 1000m,
								ProductDiscountAmount = 10m,
							},
							// 対象外
							new OrderSetPromotionModel
							{
								OrderSetpromotionNo = 2,
								ProductDiscountFlg = Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_FLG_OFF,
								UndiscountedProductSubtotal = 2000m,
								ProductDiscountAmount = 20m,
							},
							// 対象
							new OrderSetPromotionModel
							{
								OrderSetpromotionNo = 3,
								PaymentChargeFreeFlg = Constants.FLG_SETPROMOTION_PAYMENT_CHARGE_FREE_FLG_ON
							},
							// 対象
							new OrderSetPromotionModel
							{
								OrderSetpromotionNo = 4,
								ShippingChargeFreeFlg = Constants.FLG_SETPROMOTION_SHIPPING_CHARGE_FREE_FLG_ON
							},
						}
					},
				},
				new
				{
					ErrorMessage = string.Empty,
					PaymentPriceDiscountAmount = 100m,
					ShippingPriceDiscountAmount = 1000m,
					PriceSubtotalAfterDistributions = new[]
					{
						// 商品合計(割引按分後) = -(10(商品割引額) * (100(小計)/1000(割引前商品合計)) + 1(商品割引額)) = -10
						-10m,
						0m,
					},
				},
				"商品割引あり、配送料割引あり、決済手数料割引ありパターン"
			},
			new object[]
			{
				new { },
				new
				{
					Order = new OrderModel
					{
						OrderPriceRegulation = 0m,
						OrderPriceExchange = 100m,
						OrderPriceShipping = 1000m,
						Shippings = new[]
						{
							new OrderShippingModel
							{
								Items = new[]
								{
									new OrderItemModel
									{
										ProductPricePretax = 100m,
										ItemQuantity = 1,
									},
									new OrderItemModel
									{
										ProductPricePretax = 100m,
										ItemQuantity = 2,
									},
								},
							},
						},
						SetPromotions = new[]
						{
							// 対象外
							new OrderSetPromotionModel
							{
								OrderSetpromotionNo = 1,
								ProductDiscountFlg = Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_FLG_ON,
								UndiscountedProductSubtotal = 1000m,
								ProductDiscountAmount = 10m,
							},
							// 対象外
							new OrderSetPromotionModel
							{
								OrderSetpromotionNo = 2,
								ProductDiscountFlg = Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_FLG_OFF,
								UndiscountedProductSubtotal = 2000m,
								ProductDiscountAmount = 20m,
							},
						}
					},
				},
				new
				{
					ErrorMessage = string.Empty,
					PaymentPriceDiscountAmount = 0m,
					ShippingPriceDiscountAmount = 0m,
					PriceSubtotalAfterDistributions = new[]
					{
						0m,
						0m,
					},
				},
				"割引なしパターン"
			},
			new object[]
			{
				new { },
				new
				{
					Order = new OrderModel
					{
						OrderPriceRegulation = 0m,
						Shippings = new[]
						{
							new OrderShippingModel
							{
								Items = new[]
								{
									new OrderItemModel
									{
										ProductPricePretax = 100m,
										ItemQuantity = 1,
										OrderSetpromotionNo = 1,
									},
									new OrderItemModel
									{
										ProductPricePretax = 100m,
										ItemQuantity = 2,
										OrderSetpromotionNo = 2,
									},
								},
							},
						},
						SetPromotions = new[]
						{
							// 対象
							new OrderSetPromotionModel
							{
								OrderSetpromotionNo = 1,
								ProductDiscountFlg = Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_FLG_ON,
								UndiscountedProductSubtotal = 10m,
								ProductDiscountAmount = 100m,
							},
						}
					},
				},
				new
				{
					ErrorMessage = CommerceMessages.ERRMSG_MANAGER_DISCOUNT_LIMIT_ERROR,
					PaymentPriceDiscountAmount = 0m,
					ShippingPriceDiscountAmount = 0m,
					PriceSubtotalAfterDistributions = new[]
					{
						-100m,
						0m,
					},
				},
				"エラーパターン"
			},
		};

		/// <summary>
		/// 調整金額を商品・配送料・決済手数料で按分する計算テスト
		/// </summary>
		[Ignore]
		[DataTestMethod()]
		[DynamicData("m_tdCalculatePriceRegulationTest")]
		public void CalculatePriceRegulationTest(
			dynamic config,
			dynamic data,
			dynamic expected,
			string msg)
		{
			using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), true))
			using (new TestConfigurator(Member.Of(() => Constants.OPERATIONAL_BASE_ISO_CODE), Constants.COUNTRY_ISO_CODE_JP))
			using (new TestConfigurator(Member.Of(() => Constants.GLOBAL_TRANSACTION_INCLUDED_TAX_FLAG), true))
			{
				var order = (OrderModel)data.Order;
				new PrivateType(typeof(OrderCommon))
					.InvokeStatic(
						"CalculatePriceRegulation",
						new object[]
						{
								order,
						});
				order.ShippingPriceDiscountAmount.Should().Be((decimal)expected.ShippingPriceDiscountAmount,
					string.Format("配送料割引金額：{0}", msg));
				order.PaymentPriceDiscountAmount.Should().Be((decimal)expected.PaymentPriceDiscountAmount,
					string.Format("決済手数料割引金額：{0}", msg));

				var targetTestItems = order.Shippings.SelectMany(shipping => shipping.Items).ToArray();
				for (var i = 0; i < targetTestItems.Length; i++)
				{
					targetTestItems[i].ItemPriceRegulation.Should().Be((decimal)expected.Items[i].ItemPriceRegulation,
						string.Format("調整金額：{0}", msg));
				}
			}
		}

		public static object[] m_tdCalculatePriceRegulationTest = new[]
		{
			new object[]
			{
				new { },
				new
				{
					Order = new OrderModel
					{
						OrderPriceRegulation = 0m,
						Shippings = new OrderShippingModel[0]
					},
				},
				new
				{
					ShippingPriceDiscountAmount = 0m,
					PaymentPriceDiscountAmount = 0m,
				},
				"注文調整金額なしパターン"
			},
			new object[]
			{
				new { },
				new
				{
					Order = new OrderModel
					{
						OrderPriceExchange = 10000m,
						PaymentPriceDiscountAmount = 1000m,
						OrderPriceShipping = 100m,
						ShippingPriceDiscountAmount = 10m,
						OrderPriceRegulation = 2500m,
						Shippings = new[]
						{
							new OrderShippingModel
							{
								Items = new[]
								{
									new OrderItemModel
									{
										PriceSubtotalAfterDistribution = 100m,
									},
									new OrderItemModel
									{
										PriceSubtotalAfterDistribution = 200m,
									},
								}
							},
						}
					},
				},
				new
				{
					/*
					【計算式】
					   A = 100(商品小計1) + 200(商品小計2) = 300
					   B = 10000(決済手数料) - 1000(決済手数料割引金額) = 9000
					   C = 100(配送料) - 10(配送料割引金額) = 90
					   商品合計 = 300(A) + 9000(B) + 90(C) = 9390
					
					   D = 2500(調整金額) * 100(商品小計1) / 9390(商品合計) = 26
					   E = 2500(調整金額) * 200(商品小計2) / 9390(商品合計) = 53 ※金額が大きい方に寄せる
					   
					   F = 2500(調整金額) * 9000(B) / 9390(商品合計) = 2396
					   G = 2500(調整金額) * 90(C) / 9390(商品合計) = 23

					   E = 2500(調整金額) - (26(D) + 53(E) + 2396(F) + 23(G)) = 55
					   
					   H = 1000(決済手数料割引金額) - 2396(F) = -1396
					   I = 10(配送料割引金額) - 23(G) = -13
					 */

					// Iの計算結果
					ShippingPriceDiscountAmount = -13m,
					// Hの計算結果
					PaymentPriceDiscountAmount = -1396m,
					Items = new[]
					{
						// Dの計算結果
						new
						{
							ItemPriceRegulation = 26m
						},
						// Eの計算結果
						new
						{
							ItemPriceRegulation = 55m
						}
					}
				},
				"商品の調整金額に重み付けしたパターン"
			},
			new object[]
			{
				new { },
				new
				{
					Order = new OrderModel
					{
						OrderPriceShipping = 1000m,
						OrderPriceRegulation = 10m,
						Shippings = new[]
						{
							new OrderShippingModel
							{
								Items = new[]
								{
									new OrderItemModel
									{
										PriceSubtotalAfterDistribution = 0m,
									},
								}
							},
						}
					},
				},
				new
				{
					ShippingPriceDiscountAmount = -10m,
					PaymentPriceDiscountAmount = 0m,
					Items = new[]
					{
						new
						{
							PriceSubtotalAfterDistribution = 0m,
							ItemPriceRegulation = 0m
						},
						new
						{
							PriceSubtotalAfterDistribution = 0m,
							ItemPriceRegulation = 0m
						}
					}
				},
				"配送手数料割引に重み付けしたパターン"
			},
			new object[]
			{
				new { },
				new
				{
					Order = new OrderModel
					{
						OrderPriceShipping = 0m,
						OrderPriceRegulation = 1000m,
						Shippings = new[]
						{
							new OrderShippingModel
							{
								Items = new[]
								{
									new OrderItemModel
									{
										ProductPrice = 10m,
										ProductPricePretax = 10m,
										ItemQuantity = 1,
									},
									new OrderItemModel
									{
										ProductPrice = 10m,
										ProductPricePretax = 10m,
										ItemQuantity = 2,
									},
								}
							},
						}
					},
				},
				new
				{
					ShippingPriceDiscountAmount = 0m,
					PaymentPriceDiscountAmount = -1000m,
					Items = new[]
					{
						new
						{
							PriceSubtotalAfterDistribution = 10m,
							ItemPriceRegulation = 0m
						},
						new
						{
							PriceSubtotalAfterDistribution = 20m,
							ItemPriceRegulation = 0m
						}
					}
				},
				"決済手数料割引に重み付けしたパターン"
			},
		};

		///// <summary>
		///// 税率毎価格情報計算テスト(返品交換用)
		///// </summary>
		//[DataTestMethod()]
		//[DynamicData("m_tdCalculateReturnPriceInfoByTaxRateTest")]
		//public void CalculateReturnPriceInfoByTaxRateTest(
		//	dynamic config,
		//	dynamic data,
		//	List<OrderPriceByTaxRateModel> expected,
		//	string msg)
		//{
		//		using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), true))
		//		using (new TestConfigurator(Member.Of(() => Constants.OPERATIONAL_BASE_ISO_CODE), Constants.COUNTRY_ISO_CODE_JP))
		//		using (new TestConfigurator(Member.Of(() => Constants.GLOBAL_TRANSACTION_INCLUDED_TAX_FLAG), true))
		//		using (new TestConfigurator(Member.Of(() => Constants.GLOBAL_TRANSACTION_INCLUDED_TAX_FLAG), config.GlobalTransactionIncludedTaxFlag))
		//		{
		//			var result = (List<OrderPriceByTaxRateModel>)OrderCommon.CalculateReturnPriceInfoByTaxRate(
		//				data.ReturnAndExchangeItems,
		//				data.PriceCorrectionByTax,
		//				data.PointPriceByTax);
		//			result.Should().Be(expected, OrderTestHelper.EqualOrderPriceByTaxRateModel(),
		//				string.Format("税率毎価格情報：{0}", msg));
		//		}
		//}

		public static object[] m_tdCalculateReturnPriceInfoByTaxRateTest = new[]
		{
			new object[]
			{
				new
				{
					GlobalTransactionIncludedTaxFlag = true
				},
				new
				{
					ReturnAndExchangeItems = new List<ReturnOrderItem>
					{
						new ReturnOrderItem
						{
							ProductTaxRate = 8m,
							ProductPrice = 10m,
							ProductPricePretax = 100m,
						},
						new ReturnOrderItem
						{
							ProductTaxRate = 10m,
							ProductPrice = 1000m,
							ProductPricePretax = 10000m,
						}
					},
					PriceCorrectionByTax = new List<OrderPriceByTaxRateModel>
					{
						new OrderPriceByTaxRateModel
						{
							KeyTaxRate = 8m,
							PriceSubtotalByRate = 1000000m,
							PricePaymentByRate = 100000m,
							PriceShippingByRate = 10000m,
							ReturnPriceCorrectionByRate = 1000m,
							PriceTotalByRate = 100m,
							TaxPriceByRate = 10m
						},
						new OrderPriceByTaxRateModel
						{
							KeyTaxRate = 10m,
							PriceSubtotalByRate = 1000000m,
							PricePaymentByRate = 100000m,
							PriceShippingByRate = 10000m,
							ReturnPriceCorrectionByRate = 1000m,
							PriceTotalByRate = 100m,
							TaxPriceByRate = 10m
						},
					},
					PointPriceByTax = new List<OrderPriceByTaxRateModel>
					{
						new OrderPriceByTaxRateModel
						{
							KeyTaxRate = 8m,
							PriceSubtotalByRate = 79900m,
							PricePaymentByRate = 100000m,
							PriceShippingByRate = 10000m,
							ReturnPriceCorrectionByRate = 1000m,
							PriceTotalByRate = 100m,
							TaxPriceByRate = 10m
						},
						new OrderPriceByTaxRateModel
						{
							KeyTaxRate = 10m,
							PriceSubtotalByRate = 99000m,
							PricePaymentByRate = 100000m,
							PriceShippingByRate = 10000m,
							ReturnPriceCorrectionByRate = 1000m,
							PriceTotalByRate = 100m,
							TaxPriceByRate = 10m
						},
					},
				},
				new List<OrderPriceByTaxRateModel>
				{
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = 8,
						PriceSubtotalByRate = 1080000m,
						PricePaymentByRate = 0m,
						PriceShippingByRate = 0m,
						ReturnPriceCorrectionByRate = 1000m,
						PriceTotalByRate = 1081000m,
						TaxPriceByRate = 6000m
					},
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = 10,
						PriceSubtotalByRate = 1109000m,
						PricePaymentByRate = 0m,
						PriceShippingByRate = 0m,
						ReturnPriceCorrectionByRate = 1000,
						PriceTotalByRate = 1110000m,
						TaxPriceByRate = 10000m
					},
				},
				"課税パターン"
			},
			new object[]
			{
				new
				{
					GlobalTransactionIncludedTaxFlag = false
				},
				new
				{
					ReturnAndExchangeItems = new List<ReturnOrderItem>
					{
						new ReturnOrderItem
						{
							ProductTaxRate = 8m,
							ProductPrice = 10m,
							ProductPricePretax = 100m,

							// 免税
							ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_US,
							ShippingAddr5 = "州"
						},
						new ReturnOrderItem
						{
							ProductTaxRate = 10m,
							ProductPrice = 1000m,
							ProductPricePretax = 10000m,

							// 免税
							ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_US,
							ShippingAddr5 = "州"
						}
					},
					PriceCorrectionByTax = new List<OrderPriceByTaxRateModel>
					{
						new OrderPriceByTaxRateModel
						{
							KeyTaxRate = 8m,
							PriceSubtotalByRate = 1000000m,
							PricePaymentByRate = 100000m,
							PriceShippingByRate = 10000m,
							ReturnPriceCorrectionByRate = 1000m,
							PriceTotalByRate = 100m,
							TaxPriceByRate = 10m
						},
						new OrderPriceByTaxRateModel
						{
							KeyTaxRate = 10m,
							PriceSubtotalByRate = 1000000m,
							PricePaymentByRate = 100000m,
							PriceShippingByRate = 10000m,
							ReturnPriceCorrectionByRate = 1000m,
							PriceTotalByRate = 100m,
							TaxPriceByRate = 10m
						},
					},
					PointPriceByTax = new List<OrderPriceByTaxRateModel>
					{
						new OrderPriceByTaxRateModel
						{
							KeyTaxRate = 8m,
							PriceSubtotalByRate = 79900m,
							PricePaymentByRate = 100000m,
							PriceShippingByRate = 10000m,
							ReturnPriceCorrectionByRate = 1000m,
							PriceTotalByRate = 100m,
							TaxPriceByRate = 10m
						},
						new OrderPriceByTaxRateModel
						{
							KeyTaxRate = 10m,
							PriceSubtotalByRate = 99000m,
							PricePaymentByRate = 100000m,
							PriceShippingByRate = 10000m,
							ReturnPriceCorrectionByRate = 1000m,
							PriceTotalByRate = 100m,
							TaxPriceByRate = 10m
						},
					},
				},
				new List<OrderPriceByTaxRateModel>
				{
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = 8,
						PriceSubtotalByRate = 1079910m,
						PricePaymentByRate = 0m,
						PriceShippingByRate = 0m,
						ReturnPriceCorrectionByRate = 1000m,
						PriceTotalByRate = 1080910m,
						TaxPriceByRate = 0m
					},
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = 10,
						PriceSubtotalByRate = 1100000m,
						PricePaymentByRate = 0m,
						PriceShippingByRate = 0m,
						ReturnPriceCorrectionByRate = 1000,
						PriceTotalByRate = 1101000m,
						TaxPriceByRate = 0m
					},
				},
				"免税パターン"
			}
		};

		/// <summary>
		/// 税率毎金額補正情報計算テスト(返品交換用)
		/// </summary>
		[DataTestMethod()]
		[DynamicData("TdCalculatePriceCorrectionByTaxRateTest")]
		public void CalculatePriceCorrectionByTaxRateTest(
			Func<CartObject> createCartFunc,
			List<OrderPriceByTaxRateModel> lastBilledAmountByTaxRate,
			List<OrderPriceByTaxRateModel> pointPriceByTaxRate,
			List<ReturnOrderItem> returnAndExchangeItems,
			string[] allReturnFixedAmountCourseIds,
			List<OrderPriceByTaxRateModel> expected,
			string msg)
		{
			// キャッシュ生成
			var cacheData = new[]
			{
				new SubscriptionBoxModel
				{
					TaxCategoryId = "Dummy",
				}
			};

			// モックによるドメイン偽装
			var productTaxCategoryMock = new Mock<IProductTaxCategoryService>();
			productTaxCategoryMock.Setup(s => s.Get(It.IsAny<string>())).Returns(new ProductTaxCategoryModel() { TaxRate = 8m });
			DomainFacade.Instance.ProductTaxCategoryService = productTaxCategoryMock.Object;

			using (new SubscriptionBoxDataCacheConfigurator(cacheData))
			{
				var result = OrderCommon.CalculatePriceCorrectionByTaxRate(
					createCartFunc(),
					lastBilledAmountByTaxRate,
					pointPriceByTaxRate,
					returnAndExchangeItems,
					allReturnFixedAmountCourseIds);

				result.Should().BeEquivalentTo(expected, options => options.Excluding(x => x.DateCreated).Excluding(x => x.DateChanged), msg);
			}
		}
		public static IEnumerable<object[]> TdCalculatePriceCorrectionByTaxRateTest => new List<object[]>()
		{
			new object[]
			{
				new Func<CartObject>(() =>
				{
					var cart = CartTestHelper.CreateCart();
					cart.Payment = CartTestHelper.CreateCartPayment(Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE);

					cart.IsReturnAllItems = true;
					cart.OnlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;

					return cart;
				}),
				new List<OrderPriceByTaxRateModel>
				{
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = 8m,
						PriceSubtotalByRate = 1000000m,
						PricePaymentByRate = 100000m,
						PriceShippingByRate = 10000m,
						ReturnPriceCorrectionByRate = 1000m,
						PriceTotalByRate = 100m,
						TaxPriceByRate = 10m
					},
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = 10m,
						PriceSubtotalByRate = 1000000m,
						PricePaymentByRate = 100000m,
						PriceShippingByRate = 10000m,
						ReturnPriceCorrectionByRate = 1000m,
						PriceTotalByRate = 100m,
						TaxPriceByRate = 10m
					},
				},
				new List<OrderPriceByTaxRateModel>
				{
					new OrderPriceByTaxRateModel
					{
						ReturnPriceCorrectionByRate = 100m
					},
					new OrderPriceByTaxRateModel
					{
						ReturnPriceCorrectionByRate = 10m
					},
				},
				new List<ReturnOrderItem>
				{
					new ReturnOrderItem()
				},
				new string[0],
				new List<OrderPriceByTaxRateModel>
				{
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = 8m,
						PriceSubtotalByRate = 0m,
						PricePaymentByRate = 0m,
						PriceShippingByRate = 0m,
						ReturnPriceCorrectionByRate = -1111500m,
						PriceTotalByRate = 0m,
						TaxPriceByRate = 0m,
					},
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = 10m,
						PriceSubtotalByRate = 0m,
						PricePaymentByRate = 0m,
						PriceShippingByRate = 0m,
						ReturnPriceCorrectionByRate = -1111500m,
						PriceTotalByRate = 0m,
						TaxPriceByRate = 0m
					},
				},
				"商品価格調整金額が割り切れるパターン"
			},
			new object[]
			{
				new Func<CartObject>(() =>
				{
					var cart = CartTestHelper.CreateCart();
					cart.Payment = CartTestHelper.CreateCartPayment(Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE);

					cart.IsReturnAllItems = true;
					cart.OnlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;

					return cart;
				}),
				new List<OrderPriceByTaxRateModel>
				{
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = 5m,
						PriceSubtotalByRate = 1000000m,
						PricePaymentByRate = 100000m,
						PriceShippingByRate = 10000m,
						ReturnPriceCorrectionByRate = 1000m,
						PriceTotalByRate = 100m,
						TaxPriceByRate = 10m
					},
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = 8m,
						PriceSubtotalByRate = 1000000m,
						PricePaymentByRate = 100000m,
						PriceShippingByRate = 10000m,
						ReturnPriceCorrectionByRate = 1000m,
						PriceTotalByRate = 100m,
						TaxPriceByRate = 10m
					},
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = 10m,
						PriceSubtotalByRate = 1000000m,
						PricePaymentByRate = 100000m,
						PriceShippingByRate = 10000m,
						ReturnPriceCorrectionByRate = 1000m,
						PriceTotalByRate = 100m,
						TaxPriceByRate = 10m
					},
				},
				new List<OrderPriceByTaxRateModel>
				{
					new OrderPriceByTaxRateModel
					{
						ReturnPriceCorrectionByRate = 100m
					},
					new OrderPriceByTaxRateModel
					{
						ReturnPriceCorrectionByRate = 10m
					},
				},
				new List<ReturnOrderItem>
				{
					new ReturnOrderItem()
				},
				new string[0],
				new List<OrderPriceByTaxRateModel>
				{
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = 5m,
						PriceSubtotalByRate = 0m,
						PricePaymentByRate = 0m,
						PriceShippingByRate = 0m,
						ReturnPriceCorrectionByRate = -1111334m,
						PriceTotalByRate = 0m,
						TaxPriceByRate = 0m
					},
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = 8m,
						PriceSubtotalByRate = 0m,
						PricePaymentByRate = 0m,
						PriceShippingByRate = 0m,
						ReturnPriceCorrectionByRate = -1111333m,
						PriceTotalByRate = 0m,
						TaxPriceByRate = 0m
					},
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = 10m,
						PriceSubtotalByRate = 0m,
						PricePaymentByRate = 0m,
						PriceShippingByRate = 0m,
						ReturnPriceCorrectionByRate = -1111333m,
						PriceTotalByRate = 0m,
						TaxPriceByRate = 0m
					},
				},
				"商品価格調整金額が割り切れないパターン"
			},
			new object[]
			{
				new Func<CartObject>(() =>
				{
					var cart = CartTestHelper.CreateCart();
					cart.Payment = CartTestHelper.CreateCartPayment(Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE);

					cart.IsReturnAllItems = true;
					cart.OnlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;

					return cart;
				}),
				new List<OrderPriceByTaxRateModel>
				{
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = 5m,
						PriceSubtotalByRate = 1000000m,
						PricePaymentByRate = 100000m,
						PriceShippingByRate = 10000m,
						ReturnPriceCorrectionByRate = 1000m,
						PriceTotalByRate = 100m,
						TaxPriceByRate = 10m
					},
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = 8m,
						PriceSubtotalByRate = 1000000m,
						PricePaymentByRate = 100000m,
						PriceShippingByRate = 10000m,
						ReturnPriceCorrectionByRate = 1000m,
						PriceTotalByRate = 100m,
						TaxPriceByRate = 10m
					},
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = 10m,
						PriceSubtotalByRate = 1000000m,
						PricePaymentByRate = 100000m,
						PriceShippingByRate = 10000m,
						ReturnPriceCorrectionByRate = 1000m,
						PriceTotalByRate = 100m,
						TaxPriceByRate = 10m
					},
				},
				new List<OrderPriceByTaxRateModel>
				{
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = 8m,
						ReturnPriceCorrectionByRate = 100m
					},
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = 10m,
						ReturnPriceCorrectionByRate = 10m
					},
				},
				new List<ReturnOrderItem>
				{
					new ReturnOrderItem()
				},
				new string[0],
				new List<OrderPriceByTaxRateModel>
				{
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = 5m,
						PriceSubtotalByRate = 0m,
						PricePaymentByRate = 0m,
						PriceShippingByRate = 0m,
						ReturnPriceCorrectionByRate = -1111334m,
						PriceTotalByRate = 0m,
						TaxPriceByRate = 0m
					},
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = 8m,
						PriceSubtotalByRate = 0m,
						PricePaymentByRate = 0m,
						PriceShippingByRate = 0m,
						ReturnPriceCorrectionByRate = -1111333m,
						PriceTotalByRate = 0m,
						TaxPriceByRate = 0m
					},
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = 10m,
						PriceSubtotalByRate = 0m,
						PricePaymentByRate = 0m,
						PriceShippingByRate = 0m,
						ReturnPriceCorrectionByRate = -1111333m,
						PriceTotalByRate = 0m,
						TaxPriceByRate = 0m
					},
				},
				"ポイント情報が存在しないパターン"
			},
			new object[]
			{
				new Func<CartObject>(() =>
				{
					var cart = CartTestHelper.CreateCart();
		
					return cart;
				}),
				 new List<OrderPriceByTaxRateModel>
				{
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = 8m,
						PriceSubtotalByRate = 1000000m,
						PricePaymentByRate = 100000m,
						PriceShippingByRate = 10000m,
						ReturnPriceCorrectionByRate = 1000m,
						PriceTotalByRate = 100m,
						TaxPriceByRate = 10m
					},
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = 10m,
						PriceSubtotalByRate = 1000000m,
						PricePaymentByRate = 100000m,
						PriceShippingByRate = 10000m,
						ReturnPriceCorrectionByRate = 1000m,
						PriceTotalByRate = 100m,
						TaxPriceByRate = 10m
					},
				},
				new List<OrderPriceByTaxRateModel>
				{
					new OrderPriceByTaxRateModel
					{
						ReturnPriceCorrectionByRate = 100m
					},
					new OrderPriceByTaxRateModel
					{
						ReturnPriceCorrectionByRate = 10m
					},
				},
				new List<ReturnOrderItem>
				{
					new ReturnOrderItem
					{
						ProductTaxRate = 8m,
						SubscriptionBoxFixedAmount = 5000m,
						SubscriptionBoxCourseId = "SubscriptionBox1",
					}
				},
				new string[1]{ "SubscriptionBox1" },
				new List<OrderPriceByTaxRateModel>
				{
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = 8m,
						PriceSubtotalByRate = 0m,
						PricePaymentByRate = 0m,
						PriceShippingByRate = 0m,
						ReturnPriceCorrectionByRate = -1106000m,
						PriceTotalByRate = 0m,
						TaxPriceByRate = 0m
					},
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = 10m,
						PriceSubtotalByRate = 0m,
						PricePaymentByRate = 0m,
						PriceShippingByRate = 0m,
						ReturnPriceCorrectionByRate = -1111000m,
						PriceTotalByRate = 0m,
						TaxPriceByRate = 0m
					},
				},
				"定額頒布会が存在するパターン"
			},
		};

		/// <summary>
		/// 税率毎ポイント調整情報計算テスト(返品交換用)
		/// </summary>
		//[DataTestMethod()]
		//[DynamicData("m_tdCalculateAdjustmentPointPriceByTaxRateTest")]
		//public void CalculateAdjustmentPointPriceByTaxRateTest(
		//	dynamic config,
		//	dynamic data,
		//	List<OrderPriceByTaxRateModel> expected,
		//	string msg)
		//{
		//	// モックによるドメイン層偽装
		//	var mockProductTag = new Mock<IProductTagService>();
		//	mockProductTag.Setup(s => s.GetProductTagSetting()).Returns(new ProductTagSettingModel[0]);
		//	DomainFacade.Instance.ProductTagService = mockProductTag.Object;

		//	// モックによるユーティリティ偽装
		//	var mockNumberingUtility = new Mock<NumberingUtilityWrapper>();
		//	mockNumberingUtility.Setup(s => s.CreateNewNumber(It.IsAny<string>(), It.IsAny<string>()))
		//		.Returns(1);
		//	NumberingUtilityWrapper.Instance = mockNumberingUtility.Object;

		//	using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), true))
		//	using (new TestConfigurator(Member.Of(() => Constants.OPERATIONAL_BASE_ISO_CODE), Constants.COUNTRY_ISO_CODE_JP))
		//	using (new TestConfigurator(Member.Of(() => Constants.GLOBAL_TRANSACTION_INCLUDED_TAX_FLAG), true))
		//	using (new TestConfigurator(Member.Of(() => Constants.GLOBAL_TRANSACTION_INCLUDED_TAX_FLAG), config.GlobalTransactionIncludedTaxFlag))
		//	{
		//		var result = (List<OrderPriceByTaxRateModel>)OrderCommon.CalculateAdjustmentPointPriceByTaxRate(
		//			data.AdjustmentPointPrice,
		//			data.ReturnAndExchangeItems,
		//			data.CreateCartFunc());
		//		result.Should().BeEquivalentTo(expected, OrderTestHelper.EqualOrderPriceByTaxRateModel(),
		//			string.Format("税率毎ポイント調整金額情報：{0}", msg));
		//	}
		//}

		public static object[] m_tdCalculateAdjustmentPointPriceByTaxRateTest = new[]
		{
			new object[]
			{
				new
				{
					GlobalTransactionIncludedTaxFlag = false
				},
				new
				{
					AdjustmentPointPrice = 0m,
					ReturnAndExchangeItems = new List<ReturnOrderItem>()
					{
						new ReturnOrderItem(),
					},
					CreateCartFunc = new Func<CartObject>(() =>
					{
						var cart = CartTestHelper.CreateCart();
						return cart;
					}),
				},
				new List<OrderPriceByTaxRateModel> { },
				"ポイント調整金額なしパターン"
			},
			new object[]
			{
				new
				{
					GlobalTransactionIncludedTaxFlag = false
				},
				new
				{
					AdjustmentPointPrice = 1000m,
					ReturnAndExchangeItems = new List<ReturnOrderItem>()
					{
						new ReturnOrderItem(),
					},
					CreateCartFunc = new Func<CartObject>(() =>
					{
						var cart = CartTestHelper.CreateCart();
						cart.Owner = CartTestHelper.CreateCartOwner();
						cart.Items.Add(CartTestHelper.CreateCartProduct(1000m, 100));
						return cart;
					}),
				},
				new List<OrderPriceByTaxRateModel>
				{
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = 10m,
						PriceSubtotalByRate = -1000m,
						PricePaymentByRate = 0m,
						PriceShippingByRate = 0m,
						ReturnPriceCorrectionByRate = 0m,
						PriceTotalByRate = 0m,
						TaxPriceByRate = 0m
					}
				},
				"カート商品あり・課税のパターン"
			},
			new object[]
			{
				new
				{
					GlobalTransactionIncludedTaxFlag = false
				},
				new
				{
					AdjustmentPointPrice = 1000m,
					ReturnAndExchangeItems = new List<ReturnOrderItem>()
					{
						new ReturnOrderItem(),
					},
					CreateCartFunc = new Func<CartObject>(() =>
					{
						var cart = CartTestHelper.CreateCart();
						cart.Owner = CartTestHelper.CreateCartOwner();
						cart.Items.Add(CartTestHelper.CreateCartProduct(1000m, 100));

						var cartShipping = new CartShipping(cart);
						cartShipping.UpdateShippingAddr(cart.Owner, blIsSameShippingAsCart1: true);
						cartShipping.ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_US;
						cartShipping.Addr5 = "州";
						cart.SetShippingAddressAndShippingDateTime(cartShipping);

						return cart;
					}),
				},
				new List<OrderPriceByTaxRateModel>
				{
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = 10m,
						PriceSubtotalByRate = -1000m,
						PricePaymentByRate = 0m,
						PriceShippingByRate = 0m,
						ReturnPriceCorrectionByRate = 0m,
						PriceTotalByRate = 0m,
						TaxPriceByRate = 0m
					}
				},
				"カート商品あり・免税のパターン"
			},
		};

		/// <summary>
		/// ポイント利用可能額の計算テスト
		/// ・ポイント利用可能額 = 商品合計金額(税込み) + 調整金額 - 会員ランク割引額 - クーポン商品割引額 - セットプロモーション商品割引額 - 定期会員割引 - 定期購入割引 となること
		/// </summary>
		[Ignore]
		[DataTestMethod()]
		[DynamicData("m_tdGetOrderPointUsableTest")]
		public void GetOrderPointUsableTest(
			decimal orderPriceSubtotal,
			decimal orderPriceRegulationTotal,
			decimal memberRankDiscount,
			decimal useCouponPriceForProduct,
			decimal setPromotionProductDiscountAmount,
			decimal orderFixedPurchaseMemberDiscountAmount,
			decimal orderFixedPurchaseDiscountPrice,
			decimal expected)
		{
			using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), true))
			using (new TestConfigurator(Member.Of(() => Constants.OPERATIONAL_BASE_ISO_CODE), Constants.COUNTRY_ISO_CODE_JP))
			using (new TestConfigurator(Member.Of(() => Constants.GLOBAL_TRANSACTION_INCLUDED_TAX_FLAG), true))
			{
				var result = OrderCommon.GetOrderPointUsable(
					orderPriceSubtotal,
					orderPriceRegulationTotal,
					memberRankDiscount,
					useCouponPriceForProduct,
					setPromotionProductDiscountAmount,
					orderFixedPurchaseMemberDiscountAmount,
					orderFixedPurchaseDiscountPrice);
				result.Should().Be(expected, "ポイント利用可能額");
			}
		}

		public static object[] m_tdGetOrderPointUsableTest = new[]
		{
			new object[] { 1000000m, 100000m, 10000m, 1000m, 100m, 10m, 1m, 1088889m }
		};

		/// <summary>
		/// カート支払い金額合計の計算テスト
		/// ・カート支払い金額合計 = 商品合計(税込) + 配送料(税込) + 調整金額 - 会員ランク割引額 - クーポン割引額 - ポイント利用料 - セットプロモーション商品割引額 - セットプロモーション配送料割引額 - 定期会員割引額 - 定期購入割引額 となること
		/// </summary>
		[Ignore]
		[DataTestMethod()]
		[DynamicData("m_tdGetPriceCartTotalWithoutPaymentPriceTest")]
		public void GetPriceCartTotalWithoutPaymentPriceTest(
			OrderModel order,
			decimal expected)
		{
			using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), true))
			using (new TestConfigurator(Member.Of(() => Constants.OPERATIONAL_BASE_ISO_CODE), Constants.COUNTRY_ISO_CODE_JP))
			using (new TestConfigurator(Member.Of(() => Constants.GLOBAL_TRANSACTION_INCLUDED_TAX_FLAG), true))
			{
				var result = OrderCommon.GetPriceCartTotalWithoutPaymentPrice(order);
				result.Should().Be(expected, "カート支払い金額合計");
			}
		}

		public static object[] m_tdGetPriceCartTotalWithoutPaymentPriceTest = new[]
		{
			new object[]
			{
				new OrderModel
				{
					OrderPriceSubtotal = 1000000000m,
					OrderPriceSubtotalTax = 100000000m,
					OrderPriceShipping = 10000000m,
					MemberRankDiscountPrice = 1000000m,
					OrderCouponUse = 100000m,
					OrderPointUseYen = 10000m,
					SetpromotionProductDiscountAmount = 1000m,
					SetpromotionShippingChargeDiscountAmount = 100m,
					FixedPurchaseMemberDiscountAmount = 10m,
					FixedPurchaseDiscountPrice = 1m,
					OrderPriceRegulation = 0m,
					OrderPriceByTaxRates = new[]
					{
						new OrderPriceByTaxRateModel
						{
							ReturnPriceCorrectionByRate = 100m
						},
						new OrderPriceByTaxRateModel
						{
							ReturnPriceCorrectionByRate = 10m
						},
					}
				},
				1008888999m
			}
		};

		/// <summary>
		/// 割引額合計の計算テスト
		/// </summary>
		[Ignore]
		[DataTestMethod()]
		[DynamicData("m_tdCalculateDiscountPriceByCartTest")]
		public void CalculateDiscountPriceByCartTest(
			dynamic config,
			dynamic data,
			decimal expected,
			string msg)
		{
			using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), true))
			using (new TestConfigurator(Member.Of(() => Constants.OPERATIONAL_BASE_ISO_CODE), Constants.COUNTRY_ISO_CODE_JP))
			using (new TestConfigurator(Member.Of(() => Constants.GLOBAL_TRANSACTION_INCLUDED_TAX_FLAG), true))
			{
				var result = (decimal)new PrivateType(typeof(OrderCommon))
					.InvokeStatic(
						"CalculateDiscountPriceByCart",
						new object[]
						{
								data.CreateCartFunc()
						});
				result.Should().Be(expected, string.Format("割引額合計：{0}", msg));
			}
		}

		public static object[] m_tdCalculateDiscountPriceByCartTest = new[]
		{
			// ・セットプロモーションを除いた割引金額が計算されること
			new object[]
			{
				new { },
				new
				{
					CreateCartFunc = new Func<CartObject>(() =>
					{
						var cart = CartTestHelper.CreateCart();
						cart.MemberRankDiscount = 1m;
						cart.UseCouponPrice = 10m;
						cart.FixedPurchaseMemberDiscountAmount = 100m;
						cart.FixedPurchaseDiscount = 1000m;
						cart.SetPromotions = null;
						return cart;
					}),
				},
				1111m,
				"セットプロモーションなしパターン"
			},
			// ・セットプロモーションを含んだ割引金額が計算されること
			new object[]
			{
				new { },
				new
				{
					CreateCartFunc = new Func<CartObject>(() =>
					{
						// モックによるドメイン層偽装
						var mock = new Mock<IProductTagService>();
						mock.Setup(s => s.GetProductTagSetting()).Returns(new ProductTagSettingModel[0]);
						DomainFacade.Instance.ProductTagService = mock.Object;

						var cart = CartTestHelper.CreateCart();
						cart.Owner = CartTestHelper.CreateCartOwner();
						cart.Items.Add(CartTestHelper.CreateCartProduct(1000m, 100));
						var cartShipping = new CartShipping(cart);
						cartShipping.UpdateShippingAddr(cart.Owner, blIsSameShippingAsCart1: true);
						cart.SetShippingAddressAndShippingDateTime(cartShipping);

						cart.MemberRankDiscount = 1m;
						cart.UseCouponPrice = 10m;
						cart.FixedPurchaseMemberDiscountAmount = 100m;
						cart.FixedPurchaseDiscount = 1000m;

						var setPromotion = new CartSetPromotion(
							cart,
							new SetPromotionModel
							{
								IsDiscountTypeProductDiscount = true,
								ProductDiscountKbn = Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_KBN_DISCOUNT_PRICE,
								ProductDiscountSetting = 10000m,
							},
							new List<CartSetPromotion.Item>
							{
								new CartSetPromotion.Item(CartTestHelper.CreateCartProduct(10100m, 100), 1)
							});
						setPromotion.SetCount = 1;
						setPromotion.ShippingChargeDiscountAmount = 100000m;

						cart.SetPromotions.AddSetPromotion(setPromotion);
						return cart;
					}),
				},
				111111m,
				"セットプロモーションありパターン"
			},
		};

		/// <summary>
		/// カート合計金額が利用可能金額の範囲内であるかのテスト
		/// </summary>
		[Ignore]
		[DataTestMethod()]
		[DynamicData("m_tdIsPaymentPriceInRangeTest")]
		public void IsPaymentPriceInRangeTest(
			dynamic config,
			dynamic data,
			bool expected,
			string msg)
		{
			var result = (bool)new PrivateType(typeof(OrderCommon))
				.InvokeStatic(
					"IsPaymentPriceInRange",
					new object[]
					{
							data.CartPriceTotalWithoutPayment,
							data.IsPaymentChargeFree,
							data.IsCheckUsablePriceMin,
							data.PaymentPriceInfo,
							data.PaymentInfo
					});
			result.Should().Be(expected, string.Format("利用可能金額の範囲内：{0}", msg));
		}

		public static object[] m_tdIsPaymentPriceInRangeTest = new[]
		{
			new object[]
			{
				new { },
				new
				{
					CartPriceTotalWithoutPayment = 100m,
					IsPaymentChargeFree = true,
					IsCheckUsablePriceMin = true,
					PaymentPriceInfo = new PaymentPriceModel
					{
						PaymentPrice = 10m
					},
					PaymentInfo = new PaymentModel
					{
						UsablePriceMin = 100m,
						UsablePriceMax = 300m,
					},
				},
				true,
				"利用可能金額の下限との境界値パターン（範囲内）"
			},
			new object[]
			{
				new { },
				new
				{
					CartPriceTotalWithoutPayment = 99m,
					IsPaymentChargeFree = true,
					IsCheckUsablePriceMin = true,
					PaymentPriceInfo = new PaymentPriceModel
					{
						PaymentPrice = 10m
					},
					PaymentInfo = new PaymentModel
					{
						UsablePriceMin = 100m,
						UsablePriceMax = 300m,
					},
				},
				false,
				"利用可能金額の下限との境界値パターン（範囲外）"
			},
			new object[]
			{
				new { },
				new
				{
					CartPriceTotalWithoutPayment = 300m,
					IsPaymentChargeFree = true,
					IsCheckUsablePriceMin = true,
					PaymentPriceInfo = new PaymentPriceModel
					{
						PaymentPrice = 10m
					},
					PaymentInfo = new PaymentModel
					{
						UsablePriceMin = 100m,
						UsablePriceMax = 300m,
					},
				},
				true,
				"利用可能金額の上限との境界値パターン（範囲内）"
			},
			new object[]
			{
				new { },
				new
				{
					CartPriceTotalWithoutPayment = 301m,
					IsPaymentChargeFree = true,
					IsCheckUsablePriceMin = true,
					PaymentPriceInfo = new PaymentPriceModel
					{
						PaymentPrice = 10m
					},
					PaymentInfo = new PaymentModel
					{
						UsablePriceMin = 100m,
						UsablePriceMax = 300m,
					},
				},
				false,
				"利用可能金額の上限との境界値パターン（範囲外）"
			},
			new object[]
			{
				new { },
				new
				{
					CartPriceTotalWithoutPayment = 100m,
					IsPaymentChargeFree = false,
					IsCheckUsablePriceMin = true,
					PaymentPriceInfo = new PaymentPriceModel
					{
						PaymentPrice = 10m
					},
					PaymentInfo = new PaymentModel
					{
						UsablePriceMin = 100m,
						UsablePriceMax = 300m,
					},
				},
				true,
				"決済手数料がかかる場合の利用可能金額の上限との境界値パターン（範囲内）"
			},
			new object[]
			{
				new { },
				new
				{
					CartPriceTotalWithoutPayment = 300m,
					IsPaymentChargeFree = false,
					IsCheckUsablePriceMin = true,
					PaymentPriceInfo = new PaymentPriceModel
					{
						PaymentPrice = 10m
					},
					PaymentInfo = new PaymentModel
					{
						UsablePriceMin = 100m,
						UsablePriceMax = 300m,
					},
				},
				false,
				"決済手数料がかかる場合の利用可能金額の上限との境界値パターン（範囲外）"
			},
			new object[]
			{
				new { },
				new
				{
					CartPriceTotalWithoutPayment = 99m,
					IsPaymentChargeFree = false,
					IsCheckUsablePriceMin = true,
					PaymentPriceInfo = new PaymentPriceModel
					{
						PaymentPrice = 1m
					},
					PaymentInfo = new PaymentModel
					{
						UsablePriceMin = 100m,
						UsablePriceMax = 300m,
					},
				},
				true,
				"決済手数料がかかる場合の利用可能金額の下限との境界値パターン（範囲内）"
			},
			new object[]
			{
				new { },
				new
				{
					CartPriceTotalWithoutPayment = 98m,
					IsPaymentChargeFree = false,
					IsCheckUsablePriceMin = true,
					PaymentPriceInfo = new PaymentPriceModel
					{
						PaymentPrice = 1m
					},
					PaymentInfo = new PaymentModel
					{
						UsablePriceMin = 100m,
						UsablePriceMax = 300m,
					},
				},
				false,
				"決済手数料がかかる場合の利用可能金額の下限との境界値パターン（範囲外）"
			},
			new object[]
			{
				new { },
				new
				{
					CartPriceTotalWithoutPayment = 100m,
					IsPaymentChargeFree = false,
					IsCheckUsablePriceMin = false,
					PaymentPriceInfo = new PaymentPriceModel
					{
						PaymentPrice = 1m
					},
					PaymentInfo = new PaymentModel
					{
						UsablePriceMin = 100m,
						UsablePriceMax = 300m,
					},
				},
				true,
				"決済手数料がかかり、かつ、下限チェックがない場合の利用可能金額の下限との境界値パターン（範囲内）"
			},
		};

		/// <summary>
		/// 決済手数料情報取得のテスト
		/// </summary>
		//[DataTestMethod()]
		//[DynamicData("m_tdGetPaymentPriceInfoTest")]
		//public void GetPaymentPriceInfoTest(
		//	dynamic config,
		//	PaymentModel mockReturns,
		//	string shopId,
		//	string paymentId,
		//	decimal priceSubtotal,
		//	decimal priceTotalWithoutPaymentPrice,
		//	PaymentPriceModel expected)
		//{
		//	using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), true))
		//	using (new TestConfigurator(Member.Of(() => Constants.OPERATIONAL_BASE_ISO_CODE), Constants.COUNTRY_ISO_CODE_JP))
		//	using (new TestConfigurator(Member.Of(() => Constants.GLOBAL_TRANSACTION_INCLUDED_TAX_FLAG), true))
		//	using (new TestConfigurator(Member.Of(() => Constants.CALCULATE_PAYMENT_PRICE_ONLY_SUBTOTAL), config.CalculatePaymentPriceOnlySubtotal))
		//	{
		//		// モックによるドメイン層偽装
		//		var mock = new Mock<IPaymentService>();
		//		mock.Setup(s => s.Get(
		//			It.IsAny<string>(),
		//			It.IsAny<string>())).Returns(mockReturns);
		//		DomainFacade.Instance.PaymentService = mock.Object;

		//		var result = OrderCommon.GetPaymentPriceInfo(
		//			shopId,
		//			paymentId,
		//			priceSubtotal,
		//			priceTotalWithoutPaymentPrice);
		//		result.Should().ShouldBeEquivalentTo(expected, "決済手数料情報");
		//	}
		//}

		public static object[] m_tdGetPaymentPriceInfoTest = new[]
		{
			new object[]
			{
				new
				{
					CalculatePaymentPriceOnlySubtotal = true,
				},
				null,
				string.Empty,
				string.Empty,
				10m,
				1m,
				null,
				"NULLパターン"
			},
			new object[]
			{
				new
				{
					CalculatePaymentPriceOnlySubtotal = true,
				},
				new PaymentModel
				{
					PaymentPriceKbn = Constants.FLG_PAYMENT_PAYMENT_PRICE_KBN_SINGULAR,
					PriceList =  new[]
					{
						new PaymentPriceModel
						{
							PaymentId = "1",
							DateCreated = DateTime.Parse("2020/01/01"),
							DateChanged = DateTime.Parse("2020/01/01"),
						},
						new PaymentPriceModel
						{
							PaymentId = "2",
							DateCreated = DateTime.Parse("2020/01/01"),
							DateChanged = DateTime.Parse("2020/01/01"),
						},
					}
				},
				string.Empty,
				string.Empty,
				10m,
				1m,
				new PaymentPriceModel
				{
					PaymentId = "1",
					DateCreated = DateTime.Parse("2020/01/01"),
					DateChanged = DateTime.Parse("2020/01/01"),
				},
				"金額によって決済手数料を分けないパターン"
			},
			new object[]
			{
				new
				{
					CalculatePaymentPriceOnlySubtotal = true,
				},
				new PaymentModel
				{
					PaymentPriceKbn = Constants.FLG_PAYMENT_PAYMENT_PRICE_KBN_PLURAL,
					PriceList = new[]
					{
						new PaymentPriceModel
						{
							PaymentId = "1",
							TgtPriceBgn = 100m,
							TgtPriceEnd = 199m,
							DateCreated = DateTime.Parse("2020/01/01"),
							DateChanged = DateTime.Parse("2020/01/01"),
						},
						new PaymentPriceModel
						{
							PaymentId = "2",
							TgtPriceBgn = 200m,
							TgtPriceEnd = 299m,
							DateCreated = DateTime.Parse("2020/01/01"),
							DateChanged = DateTime.Parse("2020/01/01"),
						},
					}
				},
				string.Empty,
				string.Empty,
				199m,
				200m,
				new PaymentPriceModel
				{
					PaymentId = "1",
					TgtPriceBgn = 100m,
					TgtPriceEnd = 199m,
					DateCreated = DateTime.Parse("2020/01/01"),
					DateChanged = DateTime.Parse("2020/01/01"),
				},
				"割引を含まない商品小計のみを使用するパターン"
			},
			new object[]
			{
				new
				{
					CalculatePaymentPriceOnlySubtotal = false,
				},
				new PaymentModel
				{
					PaymentPriceKbn = Constants.FLG_PAYMENT_PAYMENT_PRICE_KBN_PLURAL,
					PriceList = new[]
					{
						new PaymentPriceModel
						{
							PaymentId = "1",
							TgtPriceBgn = 100m,
							TgtPriceEnd = 199m,
							DateCreated = DateTime.Parse("2020/01/01"),
							DateChanged = DateTime.Parse("2020/01/01"),
						},
						new PaymentPriceModel
						{
							PaymentId = "2",
							TgtPriceBgn = 200m,
							TgtPriceEnd = 299m,
							DateCreated = DateTime.Parse("2020/01/01"),
							DateChanged = DateTime.Parse("2020/01/01"),
						},
					}
				},
				string.Empty,
				string.Empty,
				199m,
				200m,
				new PaymentPriceModel
				{
					PaymentId = "2",
					TgtPriceBgn = 200m,
					TgtPriceEnd = 299m,
					DateCreated = DateTime.Parse("2020/01/01"),
					DateChanged = DateTime.Parse("2020/01/01"),
				},
				"合計金額を使用するパターン"
			},
		};

		/// <summary>
		/// 変更前と変更後の税率毎価格情報の差額を計算するテスト
		/// </summary>
		//[DataTestMethod()]
		//[DynamicData("m_tdCalculateDifferencePriceByTaxRateTest")]
		//public void CalculateDifferencePriceByTaxRateTest(
		//	List<OrderPriceByTaxRateModel> orderPriceByTaxRateBefore,
		//	List<OrderPriceByTaxRateModel> orderPriceByTaxRateAfter,
		//	List<OrderPriceByTaxRateModel> expected)
		//{
		//	var result =
		//		OrderCommon.CalculateDifferencePriceByTaxRate(orderPriceByTaxRateBefore, orderPriceByTaxRateAfter);

		//	result.Should().BeEquivalentTo(expected, OrderTestHelper.EqualOrderPriceByTaxRateModel(), "税率毎差額情報");
		//}

		public static object[] m_tdCalculateDifferencePriceByTaxRateTest = new[]
		{
			new object[]
			{
				new List<OrderPriceByTaxRateModel>
				{
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = 8m,
						PriceSubtotalByRate = 1000000m,
						PricePaymentByRate = 100000m,
						PriceShippingByRate = 10000m,
						ReturnPriceCorrectionByRate = 1000m,
						PriceTotalByRate = 100m,
						TaxPriceByRate = 10m
					},
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = 10m,
						PriceSubtotalByRate = 1000000m,
						PricePaymentByRate = 100000m,
						PriceShippingByRate = 10000m,
						ReturnPriceCorrectionByRate = 1000m,
						PriceTotalByRate = 100m,
						TaxPriceByRate = 10m
					},
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = 10m,
						PriceSubtotalByRate = 1000000m,
						PricePaymentByRate = 100000m,
						PriceShippingByRate = 10000m,
						ReturnPriceCorrectionByRate = 1000m,
						PriceTotalByRate = 100m,
						TaxPriceByRate = 10m
					}
				},
				new List<OrderPriceByTaxRateModel>
				{
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = 8m,
						PriceSubtotalByRate = 100000m,
						PricePaymentByRate = 10000m,
						PriceShippingByRate = 1000m,
						ReturnPriceCorrectionByRate = 100m,
						PriceTotalByRate = 10m,
						TaxPriceByRate = 1m
					},
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = 10m,
						PriceSubtotalByRate = 100000m,
						PricePaymentByRate = 10000m,
						PriceShippingByRate = 1000m,
						ReturnPriceCorrectionByRate = 100m,
						PriceTotalByRate = 10m,
						TaxPriceByRate = 1m
					},
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = 10m,
						PriceSubtotalByRate = 100000m,
						PricePaymentByRate = 10000m,
						PriceShippingByRate = 1000m,
						ReturnPriceCorrectionByRate = 100m,
						PriceTotalByRate = 10m,
						TaxPriceByRate = 1m
					}
				},
				new List<OrderPriceByTaxRateModel>
				{
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = 8m,
						PriceSubtotalByRate = 900000m,
						PricePaymentByRate = 90000m,
						PriceShippingByRate = 9000m,
						ReturnPriceCorrectionByRate = 900m,
						PriceTotalByRate = 90m,
						TaxPriceByRate = 9m	
					},
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = 10m,
						PriceSubtotalByRate = 1800000m,
						PricePaymentByRate = 180000m,
						PriceShippingByRate = 18000m,
						ReturnPriceCorrectionByRate = 1800m,
						PriceTotalByRate = 180m,
						TaxPriceByRate = 18m
					}
				}
			}
		};

		/// <summary>
		/// 割引額合計の計算テスト
		/// </summary>
		//[DataTestMethod()]
		//[DynamicData("m_tdCreateOrderPriceByTaxRateTest")]
		//public void CreateOrderPriceByTaxRateTest(
		//	dynamic config,
		//	dynamic data,
		//	OrderPriceByTaxRateModel[] expected,
		//	string msg)
		//{
		//	using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), false))
		//	using (new TestConfigurator(Member.Of(() => Constants.OPERATIONAL_BASE_ISO_CODE), Constants.COUNTRY_ISO_CODE_JP))
		//	using (new TestConfigurator(Member.Of(() => Constants.GLOBAL_TRANSACTION_INCLUDED_TAX_FLAG), true))
		//	using (new TestConfigurator(Member.Of(() => Constants.TAX_EXCLUDED_FRACTION_ROUNDING),
		//		TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN))
		//	{
		//		var result = (OrderPriceByTaxRateModel[]) OrderCommon.CreateOrderPriceByTaxRate(data.Order);
		//		result.Should().BeEquivalentTo(expected, OrderTestHelper.EqualOrderPriceByTaxRateModel(),
		//		string.Format("税率毎価格情報：{0}", msg));
		//	}
		//}

		public static object[] m_tdCreateOrderPriceByTaxRateTest = new[]
		{
			new object[]
			{
				new { },
				new
				{
					Order = new OrderModel
					{
						OrderId = "ODID001",
						OrderPriceExchange = 10m,
						OrderPriceRegulation = 200m,
						OrderPriceShipping = 1000m,
						PaymentTaxRate = 8m,
						ShippingTaxRate = 10m,
						Items = new[]
						{
							new OrderItemModel
							{
								ProductTaxRate = 10m,
								ProductPrice = 100m,
								ProductPricePretax = 1000m,
								ItemPriceTax = 10000m,
								ItemQuantity = 1,
							},
						}
					},
				},
				new[]
				{
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = 10m,
						OrderId = "ODID001",
						PricePaymentByRate = 0m,
						PriceShippingByRate = 1099,
						PriceSubtotalByRate = 1101m,
						PriceTotalByRate = 2200m,
						ReturnPriceCorrectionByRate = 0m,
						TaxPriceByRate = 200m,
					},
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = 8m,
						OrderId = "ODID001",
						PricePaymentByRate = 10m,
						PriceShippingByRate = 0m,
						PriceSubtotalByRate = 0m,
						PriceTotalByRate = 10m,
						ReturnPriceCorrectionByRate = 0m,
						TaxPriceByRate = 0m,
					},
				},
				"按分を商品に寄せたパターン"
			},
			new object[]
			{
				new { },
				new
				{
					Order = new OrderModel
					{
						OrderId = "ODID002",
						OrderPriceExchange = 10m,
						OrderPriceRegulation = 200m,
						OrderPriceShipping = 1000m,
						PaymentTaxRate = 8m,
						ShippingTaxRate = 10m,
						Items = new OrderItemModel[0]
					},
				},
				new[]
				{
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = 10m,
						OrderId = "ODID002",
						PricePaymentByRate = 0m,
						PriceShippingByRate = 1199m,
						PriceSubtotalByRate = 0m,
						PriceTotalByRate = 1199m,
						ReturnPriceCorrectionByRate = 0m,
						TaxPriceByRate = 109m,
					},
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = 8m,
						OrderId = "ODID002",
						PricePaymentByRate = 11m,
						PriceShippingByRate = 0m,
						PriceSubtotalByRate = 0m,
						PriceTotalByRate = 11m,
						ReturnPriceCorrectionByRate = 0m,
						TaxPriceByRate = 0m,
					},
				},
				"按分を配送手数料に寄せたパターン"
			},
			new object[]
			{
				new { },
				new
				{
					Order = new OrderModel
					{
						OrderId = "ODID003",
						OrderPriceExchange = 1080m,
						OrderPriceRegulation = 0m,
						OrderPriceShipping = 0m,
						PaymentTaxRate = 8m,
						ShippingTaxRate = 10m,
						Items = new OrderItemModel[0]
					},
				},
				new[]
				{
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = 10m,
						OrderId = "ODID003",
						PricePaymentByRate = 0m,
						PriceShippingByRate = 0m,
						PriceSubtotalByRate = 0m,
						PriceTotalByRate = 0m,
						ReturnPriceCorrectionByRate = 0m,
						TaxPriceByRate = 0m,
					},
					new OrderPriceByTaxRateModel
					{
						KeyTaxRate = 8m,
						OrderId = "ODID003",
						PricePaymentByRate = 1080m,
						PriceShippingByRate = 0m,
						PriceSubtotalByRate = 0m,
						PriceTotalByRate = 1080m,
						ReturnPriceCorrectionByRate = 0m,
						TaxPriceByRate = 80m,
					},
				},
				"按分を決済手数料に寄せたパターン"
			},
		};

		/// <summary>
		/// 受注商品に割引金額を按分する計算テスト
		/// </summary>
		[Ignore]
		[DataTestMethod()]
		[DynamicData("m_tdProrateDiscountPriceTest")]
		public void ProrateDiscountPriceTest(
			dynamic config,
			dynamic data,
			dynamic expected,
			string msg)
		{
			using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), true))
			using (new TestConfigurator(Member.Of(() => Constants.OPERATIONAL_BASE_ISO_CODE), Constants.COUNTRY_ISO_CODE_JP))
			using (new TestConfigurator(Member.Of(() => Constants.GLOBAL_TRANSACTION_INCLUDED_TAX_FLAG), true))
			{
				var order = (OrderModel)data.Order;
				var items = (OrderItemModel[])data.Items;

				var result = (string)new PrivateType(typeof(OrderCommon))
					.InvokeStatic(
						"ProrateDiscountPrice",
						new object[]
						{
								order,
								items,
								(decimal) data.DiscountTotal,
								(string) data.DiscountName
						});

				result.Should().Be((string)expected.ErrorMessage, string.Format("エラーメッセージ：{0}", msg));

				var targetTestItems = items.Any()
					? items
					: order.Shippings.SelectMany(shipping => shipping.Items).ToArray();
				for (var i = 0; i < targetTestItems.Length; i++)
				{
					targetTestItems[i].PriceSubtotalAfterDistribution.Should().Be(
						(decimal)expected.PriceSubtotalAfterDistributions[i], string.Format("商品小計：{0}", msg));
				}
			}
		}

		public static object[] m_tdProrateDiscountPriceTest = new[]
		{
			new object[]
			{
				new { },
				new
				{
					Order = new OrderModel
					{
						Shippings = new[]
						{
							new OrderShippingModel
							{
								Items = new OrderItemModel[0]
							}, 
						}
					},
					Items = new OrderItemModel[0],
					DiscountTotal = 0m,
					DiscountName = "割引名"
				},
				new
				{
					ErrorMessage = string.Empty,
					PriceSubtotalAfterDistributions = new decimal[0]
				},
				"割引金額なしパターン"
			},
			new object[]
			{
				new { },
				new
				{
					Order = new OrderModel
					{
						Shippings = new[]
						{
							new OrderShippingModel
							{
								Items = new OrderItemModel[0]
							},
						}
					},
					Items = new[]
					{
						new OrderItemModel
						{
							PriceSubtotalAfterDistribution = 2000m,
						},
						new OrderItemModel
						{
							PriceSubtotalAfterDistribution = 3000m,
						},
					},
					DiscountTotal = 1000m,
					DiscountName = "割引名"
				},
				new
				{
					ErrorMessage = string.Empty,
					PriceSubtotalAfterDistributions = new[]
					{
						// 2000(割引前商品小計) - 1000(商品割引額) * (2000(割引前商品小計)/5000(割引前商品合計) = 1600
						1600m,
						// 3000(割引前商品小計) - 1000(商品割引額) * (3000(割引前商品小計)/5000(割引前商品合計) = 2400
						2400m,
					}
				},
				"単一配送先：正常系パターン"
			},
			new object[]
			{
				new { },
				new
				{
					Order = new OrderModel
					{
						Shippings = new[]
						{
							new OrderShippingModel
							{
								Items = new OrderItemModel[0]
							},
						}
					},
					Items = new[]
					{
						new OrderItemModel
						{
							PriceSubtotalAfterDistribution = 100m,
						},
						new OrderItemModel
						{
							PriceSubtotalAfterDistribution = 200m,
						},
					},
					DiscountTotal = 1000m,
					DiscountName = "割引名"
				},
				new
				{
					ErrorMessage = CommerceMessages.ERRMSG_MANAGER_DISCOUNT_LIMIT_ERROR,
					PriceSubtotalAfterDistributions = new[]
					{
						-234m,
						-466m,
					}
				},
				"単一配送先：エラーパターン"
			},
			new object[]
			{
				new { },
				new
				{
					Order = new OrderModel
					{
						Shippings = new[]
						{
							new OrderShippingModel
							{
								Items = new[]
								{
									new OrderItemModel
									{
										PriceSubtotalAfterDistribution = 10m,
									},
									new OrderItemModel
									{
										PriceSubtotalAfterDistribution = 100m,
									},
								},
							},
							new OrderShippingModel
							{
								Items = new[]
								{
									new OrderItemModel
									{
										PriceSubtotalAfterDistribution = 1000m,
									},
									new OrderItemModel
									{
										PriceSubtotalAfterDistribution = 10000m,
									},
								},
							},
						}
					},
					Items = new OrderItemModel[0],
					DiscountTotal = 1000m,
					DiscountName = "割引名"
				},
				new
				{
					ErrorMessage = string.Empty,
					PriceSubtotalAfterDistributions = new[]
					{
						// 10(割引前商品小計) - 1000(商品割引額) * (10(割引前商品小計)/11110(割引前商品合計) = 10
						10m,
						// 100(割引前商品小計) - 1000(商品割引額) * (100(割引前商品小計)/11110(割引前商品合計) = 91
						91m,
						// 1000(割引前商品小計) - 1000(商品割引額) * (1000(割引前商品小計)/11110(割引前商品合計) = 910
						910m,
						// 10000(割引前商品小計) - 1000(商品割引額) * (10000(割引前商品小計)/11110(割引前商品合計) = 9099
						9099m,
					}
				},
				"複数配送先：正常系パターン"
			},
			new object[]
			{
				new { },
				new
				{
					Order = new OrderModel
					{
						Shippings = new[]
						{
							new OrderShippingModel
							{
								Items = new[]
								{
									new OrderItemModel
									{
										PriceSubtotalAfterDistribution = 10m,
									},
									new OrderItemModel
									{
										PriceSubtotalAfterDistribution = 100m,
									},
								},
							},
							new OrderShippingModel
							{
								Items = new[]
								{
									new OrderItemModel
									{
										PriceSubtotalAfterDistribution = 1000m,
									},
									new OrderItemModel
									{
										PriceSubtotalAfterDistribution = 10000m,
									},
								},
							},
						}
					},
					Items = new OrderItemModel[0],
					DiscountTotal = 100000m,
					DiscountName = "割引名"
				},
				new
				{
					ErrorMessage = CommerceMessages.ERRMSG_MANAGER_DISCOUNT_LIMIT_ERROR,
					PriceSubtotalAfterDistributions = new[]
					{
						-81m,
						-800m,
						-8000m,
						-80009m,
					}
				},
				"複数配送先：エラーパターン"
			},
		};

		/// <summary>
		/// 受注商品に割引金額を按分する計算テスト
		/// </summary>
		[Ignore]
		[DataTestMethod()]
		[DynamicData("m_tdInitializeSubtotalTest")]
		public void InitializeSubtotalTest(
			dynamic config,
			dynamic data,
			dynamic expected,
			string msg)
		{
			using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), true))
			using (new TestConfigurator(Member.Of(() => Constants.OPERATIONAL_BASE_ISO_CODE), Constants.COUNTRY_ISO_CODE_JP))
			using (new TestConfigurator(Member.Of(() => Constants.GLOBAL_TRANSACTION_INCLUDED_TAX_FLAG), config.GlobalTransactionIncludedTaxFlag))
			{
				var order = (OrderModel)data.Order;
				OrderCommon.InitializeSubtotal(order);

				order.ShippingPriceDiscountAmount.Should().Be((decimal)expected.ShippingPriceDiscountAmount,
					string.Format("配送料割引金額：{0}", msg));
				order.PaymentPriceDiscountAmount.Should().Be((decimal)expected.PaymentPriceDiscountAmount,
					string.Format("決済手数料割引金額：{0}", msg));

				var targetTestItems = order.Shippings.SelectMany(shipping => shipping.Items).ToArray();
				for (var i = 0; i < targetTestItems.Length; i++)
				{
					targetTestItems[i].PriceSubtotalAfterDistribution.Should().Be(
						(decimal)expected.Items[i].PriceSubtotalAfterDistribution, string.Format("商品小計：{0}", msg));
					targetTestItems[i].ItemPriceRegulation.Should().Be((decimal)expected.Items[i].ItemPriceRegulation,
						string.Format("調整金額：{0}", msg));
				}
			}
		}

		public static object[] m_tdInitializeSubtotalTest = new[]
		{
			new object[]
			{
				new
				{
					GlobalTransactionIncludedTaxFlag = false,
				},
				new
				{
					Order = new OrderModel
					{
						Shippings = new[]
						{
							new OrderShippingModel
							{
								ShippingCountryIsoCode = "",
								ShippingAddr5 = "",
								Items = new[]
								{
									new OrderItemModel
									{
										ProductPrice = 10m,
										ProductPricePretax = 100m,
										ItemQuantity = 1,
									},
									new OrderItemModel
									{
										ProductPrice = 10m,
										ProductPricePretax = 100m,
										ItemQuantity = 2,
									},
								}
							},
						}
					},
				},
				new
				{
					ShippingPriceDiscountAmount = 0m,
					PaymentPriceDiscountAmount = 0m,
					Items = new[]
					{
						new
						{
							// 100(税込み単価) * 1(数量) = 100
							PriceSubtotalAfterDistribution = 100m,
							ItemPriceRegulation = 0m
						},
						new
						{
							// 100(税込み単価) * 2(数量) = 200
							PriceSubtotalAfterDistribution = 200m,
							ItemPriceRegulation = 0m
						}
					}
				},
				"課税パターン"
			},
			new object[]
			{
				new
				{
					GlobalTransactionIncludedTaxFlag = false,
				},
				new
				{
					Order = new OrderModel
					{
						Shippings = new[]
						{
							new OrderShippingModel
							{
								ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_US,
								ShippingAddr5 = "州",
								Items = new[]
								{
									new OrderItemModel
									{
										ProductPrice = 10m,
										ProductPricePretax = 10m,
										ItemQuantity = 1,
									},
									new OrderItemModel
									{
										ProductPrice = 10m,
										ProductPricePretax = 10m,
										ItemQuantity = 2,
									},
								}
							},
						}
					},
				},
				new
				{
					ShippingPriceDiscountAmount = 0m,
					PaymentPriceDiscountAmount = 0m,
					Items = new[]
					{
						new
						{
							PriceSubtotalAfterDistribution = 10m,
							ItemPriceRegulation = 0m
						},
						new
						{
							PriceSubtotalAfterDistribution = 20m,
							ItemPriceRegulation = 0m
						}
					}
				},
				"免税パターン"
			},
		};

		/// <summary>
		/// 定期購入割引額の計算テスト
		/// </summary>
		[Ignore]
		[DataTestMethod()]
		[DynamicData("m_tdCalculateFixedPurchaseDiscountPriceTest")]
		public void CalculateFixedPurchaseDiscountPriceTest(
			dynamic config,
			dynamic data,
			dynamic expected,
			string msg)
		{
			var order = (OrderModel)data.Order;

			var result = (string)new PrivateType(typeof(OrderCommon))
				.InvokeStatic(
					"CalculateFixedPurchaseDiscountPrice",
					new object[]
					{
							order,
					});

			result.Should().Be((string)expected.ErrorMessage, string.Format("エラーメッセージ：{0}", msg));

			var targetTestItems = order.Shippings.SelectMany(shipping => shipping.Items).ToArray();
			for (var i = 0; i < targetTestItems.Length; i++)
			{
				targetTestItems[i].PriceSubtotalAfterDistribution.Should().Be((decimal)expected.PriceSubtotalAfterDistributions[i], string.Format("商品小計：{0}", msg));
			}
		}

		public static object[] m_tdCalculateFixedPurchaseDiscountPriceTest = new[]
		{
			new object[]
			{
				new { },
				new
				{
					Order = new OrderModel
					{
						Shippings = new[]
						{
							new OrderShippingModel
							{
								Items = new OrderItemModel[0]
							},
						},
						FixedPurchaseDiscountPrice = 0m
					},
				},
				new
				{
					ErrorMessage = string.Empty,
					PriceSubtotalAfterDistributions = new decimal[0]
				},
				"割引金額なしパターン"
			},
			new object[]
			{
				new { },
				new
				{
					Order = new OrderModel
					{
						Shippings = new[]
						{
							new OrderShippingModel
							{
								Items = new[]
								{
									new OrderItemModel
									{
										// 対象（割引金額1円）
										PriceSubtotalAfterDistribution = 11m,
										FixedPurchaseProductFlg = Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON,
										FixedPurchaseDiscountType = Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_YEN,
										FixedPurchaseDiscountValue = 1m,
									},
									new OrderItemModel
									{
										// 対象（割引金額11円）
										PriceSubtotalAfterDistribution = 111m,
										FixedPurchaseProductFlg = Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON,
										FixedPurchaseDiscountType = Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_PERCENT,
										FixedPurchaseDiscountValue = 10m,
									},
									new OrderItemModel
									{
										// 対象外(定期商品・定期割引設定なし)
										PriceSubtotalAfterDistribution = 200m,
										FixedPurchaseProductFlg = Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON,
										FixedPurchaseDiscountType = null,
										FixedPurchaseDiscountValue = null,
									},
									new OrderItemModel
									{
										// 対象外(定期商品ではない)
										PriceSubtotalAfterDistribution = 2000m,
										FixedPurchaseProductFlg = Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_OFF,
										FixedPurchaseDiscountType = null,
										FixedPurchaseDiscountValue = null,
									},
								},
							},
							new OrderShippingModel
							{
								Items = new[]
								{
									// 対象（割引金額100円）
									new OrderItemModel
									{
										PriceSubtotalAfterDistribution = 1100m,
										FixedPurchaseProductFlg = Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON,
										FixedPurchaseDiscountType = Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_YEN,
										FixedPurchaseDiscountValue = 100m,
									},
									// 対象（割引金額1111円）
									new OrderItemModel
									{
										PriceSubtotalAfterDistribution = 11111m,
										FixedPurchaseProductFlg = Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON,
										FixedPurchaseDiscountType = Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_PERCENT,
										FixedPurchaseDiscountValue = 10m,
									},
									new OrderItemModel
									{
										// 対象外(定期商品・定期割引設定なし)
										PriceSubtotalAfterDistribution = 20000m,
										FixedPurchaseProductFlg = Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON,
										FixedPurchaseDiscountType = null,
										FixedPurchaseDiscountValue = null,
									},
									new OrderItemModel
									{
										// 対象外(定期商品ではない)
										PriceSubtotalAfterDistribution = 200000m,
										FixedPurchaseProductFlg = Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_OFF,
										FixedPurchaseDiscountType = null,
										FixedPurchaseDiscountValue = null,
									},
								},
							},
						},
						FixedPurchaseDiscountPrice = 2223m,
					},
				},
				new
				{
					ErrorMessage = string.Empty,
					// 各商品の設定値から計算した割引額の合計は「1 + 11 + 100 + 1111 = 1223円」
					// FixedPurchaseDiscountPrice「2223」円との差異、「1000」円は定期購入割引対象商品の各割引後金額に対して按分を行う
					// 按分前の商品金額合計は「11 + 111 + 1100 + 11111」(定期購入割引対象商品の小計)-「1 + 11 + 100 + 1111」(定期購入割引の小計) = 11110
					PriceSubtotalAfterDistributions = new[]
					{
						// 10(按分前商品小計) - 1000(商品割引額) * (10(按分前商品小計)/11110(按分前商品合計) = 10
						10m,
						// 100(按分前商品小計) - 1000(商品割引額) * (100(按分前商品小計)/11110(按分前商品合計) = 91
						91m,
						// 割引対象外
						200m,
						// 割引対象外
						2000m,
						// 1000(按分前商品小計) - 1000(商品割引額) * (1000(按分前商品小計)/11110(按分前商品合計) = 910
						910m,
						// 10000(按分前商品小計) - 1000(商品割引額) * (10000(按分前商品小計)/11110(按分前商品合計) = 9099
						9099m,
						// 割引対象外
						20000m,
						// 割引対象外
						200000m,
					}
				},
				"正常系パターン"
			},
			new object[]
			{
				new { },
				new
				{
					Order = new OrderModel
					{
						Shippings = new[]
						{
							new OrderShippingModel
							{
								Items = new[]
								{
									new OrderItemModel
									{
										// 対象
										PriceSubtotalAfterDistribution = 11m,
										FixedPurchaseProductFlg = Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON,
										FixedPurchaseDiscountType = Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_YEN,
										FixedPurchaseDiscountValue = 1m,
									},
									new OrderItemModel
									{
										// 対象
										PriceSubtotalAfterDistribution = 111m,
										FixedPurchaseProductFlg = Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON,
										FixedPurchaseDiscountType = Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_PERCENT,
										FixedPurchaseDiscountValue = 10m,
									},
								},
							},
							new OrderShippingModel
							{
								Items = new[]
								{
									new OrderItemModel
									{
										// 対象外(定期商品・定期割引設定なし)
										PriceSubtotalAfterDistribution = 1000m,
										FixedPurchaseProductFlg = Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON,
										FixedPurchaseDiscountType = null,
										FixedPurchaseDiscountValue = null,
									},
									new OrderItemModel
									{
										// 対象外(定期商品ではない)
										PriceSubtotalAfterDistribution = 10000m,
										FixedPurchaseProductFlg = Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_OFF,
										FixedPurchaseDiscountType = null,
										FixedPurchaseDiscountValue = null,
									},
								},
							},
						},
						FixedPurchaseDiscountPrice = 1000m,
					},
				},
				new
				{
					ErrorMessage = CommerceMessages.ERRMSG_MANAGER_DISCOUNT_LIMIT_ERROR,
					PriceSubtotalAfterDistributions = new[]
					{
						-80m,
						-798m,
						1000m,
						10000m,
					}
				},
				"エラーパターン"
			},
		};

		/// <summary>
		/// 会員ランク割引額の按分計算テスト
		/// </summary>
		[Ignore]
		[DataTestMethod()]
		[DynamicData("m_tdCalculateMemberRankDiscountPriceTest")]
		public void CalculateMemberRankDiscountPriceTest(
			dynamic config,
			dynamic data,
			dynamic expected,
			string msg)
		{
			// モックによるユーティリティ偽装
			var mockNumberingUtility = new Mock<MemberRankOptionUtilityWrapper>();
			mockNumberingUtility.Setup(s => s.IsDiscountTarget(It.IsAny<string>(), It.IsAny<string>()))
				.Returns(true);
			MemberRankOptionUtilityWrapper.Instance = mockNumberingUtility.Object;

			using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), true))
			using (new TestConfigurator(Member.Of(() => Constants.OPERATIONAL_BASE_ISO_CODE), Constants.COUNTRY_ISO_CODE_JP))
			using (new TestConfigurator(Member.Of(() => Constants.GLOBAL_TRANSACTION_INCLUDED_TAX_FLAG), true))
			using (new TestConfigurator(Member.Of(() => Constants.MEMBER_RANK_OPTION_ENABLED), config.MemberRankOptionEnabled))
			{
				var order = (OrderModel)data.Order;

				var result = (string)new PrivateType(typeof(OrderCommon))
					.InvokeStatic(
						"CalculateMemberRankDiscountPrice",
						new object[]
						{
							order,
						});

				result.Should().Be((string)expected.ErrorMessage, string.Format("エラーメッセージ：{0}", msg));

				var targetTestItems = order.Shippings.SelectMany(shipping => shipping.Items)
					.Where(item =>
						(Constants.MEMBER_RANK_OPTION_ENABLED &&
							MemberRankOptionUtilityWrapper.Instance.IsDiscountTarget(item.ShopId, item.ProductId)))
					.ToArray();
				for (var i = 0; i < targetTestItems.Length; i++)
				{
					targetTestItems[i].PriceSubtotalAfterDistribution.Should().Be(
						(decimal)expected.PriceSubtotalAfterDistributions[i], string.Format("商品小計：{0}", msg));
				}
			}
		}

		public static object[] m_tdCalculateMemberRankDiscountPriceTest = new[]
		{
			new object[]
			{
				new 
				{
					MemberRankOptionEnabled = false,
				},
				new
				{
					Order = new OrderModel
					{
						Shippings = new[]
						{
							new OrderShippingModel
							{
								Items = new OrderItemModel[0]
							},
						},
						MemberRankDiscountPrice = 0m
					},
				},
				new
				{
					ErrorMessage = string.Empty,
					PriceSubtotalAfterDistributions = new decimal[0]
				},
				"会員ランクオプションOFFパターン"
			},
			new object[]
			{
				new 
				{
					MemberRankOptionEnabled = true,
				},
				new
				{
					Order = new OrderModel
					{
						Shippings = new[]
						{
							new OrderShippingModel
							{
								Items = new OrderItemModel[0]
							},
						},
						MemberRankDiscountPrice = 0m
					},
				},
				new
				{
					ErrorMessage = string.Empty,
					PriceSubtotalAfterDistributions = new decimal[0]
				},
				"割引金額なしパターン"
			},
			new object[]
			{
				new 
				{
					MemberRankOptionEnabled = true,
				},
				new
				{
					Order = new OrderModel
					{
						Shippings = new[]
						{
							new OrderShippingModel
							{
								Items = new[]
								{
									new OrderItemModel
									{
										PriceSubtotalAfterDistribution = 10m,
									},
									new OrderItemModel
									{
										PriceSubtotalAfterDistribution = 100m,
									},
								},
							},
							new OrderShippingModel
							{
								Items = new[]
								{
									new OrderItemModel
									{
										PriceSubtotalAfterDistribution = 1000m,
									},
									new OrderItemModel
									{
										PriceSubtotalAfterDistribution = 10000m,
									},
								},
							},
						},
						MemberRankDiscountPrice = 1000m,
					},
				},
				new
				{
					ErrorMessage = string.Empty,
					PriceSubtotalAfterDistributions = new[]
					{
						// 10(割引前商品小計) - 1000(商品割引額) * (10(割引前商品小計)/11110(割引前商品合計) = 10
						10m,
						// 100(割引前商品小計) - 1000(商品割引額) * (100(割引前商品小計)/11110(割引前商品合計) = 91
						91m,
						// 1000(割引前商品小計) - 1000(商品割引額) * (1000(割引前商品小計)/11110(割引前商品合計) = 910
						910m,
						// 10000(割引前商品小計) - 1000(商品割引額) * (10000(割引前商品小計)/11110(割引前商品合計) = 9099
						9099m,
					}
				},
				"正常系パターン"
			},
			new object[]
			{
				new 
				{
					MemberRankOptionEnabled = true,
				},
				new
				{
					Order = new OrderModel
					{
						Shippings = new[]
						{
							new OrderShippingModel
							{
								Items = new[]
								{
									new OrderItemModel
									{
										PriceSubtotalAfterDistribution = 10m,
									},
									new OrderItemModel
									{
										PriceSubtotalAfterDistribution = 100m,
									},
								},
							},
							new OrderShippingModel
							{
								Items = new[]
								{
									new OrderItemModel
									{
										PriceSubtotalAfterDistribution = 1000m,
									},
									new OrderItemModel
									{
										PriceSubtotalAfterDistribution = 10000m,
									},
								},
							},
						},
						MemberRankDiscountPrice = 100000m,
					},
				},
				new
				{
					ErrorMessage = CommerceMessages.ERRMSG_MANAGER_DISCOUNT_LIMIT_ERROR,
					PriceSubtotalAfterDistributions = new[]
					{
						-81m,
						-800m,
						-8000m,
						-80009m,
					}
				},
				"エラーパターン"
			},
		};

		/// <summary>
		/// ポイント割引額の按分計算テスト
		/// </summary>
		[Ignore]
		[DataTestMethod()]
		[DynamicData("m_tdCalculatePointDisCountProductPrice")]
		public void CalculatePointDisCountProductPrice(
			dynamic config,
			dynamic data,
			dynamic expected,
			string msg)
		{
			using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), true))
			using (new TestConfigurator(Member.Of(() => Constants.OPERATIONAL_BASE_ISO_CODE), Constants.COUNTRY_ISO_CODE_JP))
			using (new TestConfigurator(Member.Of(() => Constants.GLOBAL_TRANSACTION_INCLUDED_TAX_FLAG), true))
			using (new TestConfigurator(Member.Of(() => Constants.W2MP_POINT_OPTION_ENABLED), config.PointOptionEnabled))
			{
				var order = (OrderModel)data.Order;

				var result = (string)new PrivateType(typeof(OrderCommon))
					.InvokeStatic(
						"CalculatePointDisCountProductPrice",
						new object[]
						{
							order,
						});

				result.Should().Be((string)expected.ErrorMessage, string.Format("エラーメッセージ：{0}", msg));

				var targetTestItems = order.Shippings.SelectMany(shipping => shipping.Items).ToArray();
				for (var i = 0; i < targetTestItems.Length; i++)
				{
					targetTestItems[i].PriceSubtotalAfterDistribution.Should().Be(
						(decimal)expected.PriceSubtotalAfterDistributions[i], string.Format("商品小計：{0}", msg));
				}
			}
		}

		public static object[] m_tdCalculatePointDisCountProductPrice = new[]
		{
			new object[]
			{
				new
				{
					PointOptionEnabled = false,
				},
				new
				{
					Order = new OrderModel
					{
						Shippings = new[]
						{
							new OrderShippingModel
							{
								Items = new OrderItemModel[0]
							},
						},
						OrderPointUse = 0m
					},
				},
				new
				{
					ErrorMessage = string.Empty,
					PriceSubtotalAfterDistributions = new decimal[0]
				},
				"ポイントオプションOFFパターン"
			},
			new object[]
			{
				new
				{
					PointOptionEnabled = true,
				},
				new
				{
					Order = new OrderModel
					{
						Shippings = new[]
						{
							new OrderShippingModel
							{
								Items = new OrderItemModel[0]
							},
						},
						OrderPointUse = 0m
					},
				},
				new
				{
					ErrorMessage = string.Empty,
					PriceSubtotalAfterDistributions = new decimal[0]
				},
				"割引金額なしパターン"
			},
			new object[]
			{
				new
				{
					PointOptionEnabled = true,
				},
				new
				{
					Order = new OrderModel
					{
						Shippings = new[]
						{
							new OrderShippingModel
							{
								Items = new[]
								{
									new OrderItemModel
									{
										PriceSubtotalAfterDistribution = 10m,
									},
									new OrderItemModel
									{
										PriceSubtotalAfterDistribution = 100m,
									},
								},
							},
							new OrderShippingModel
							{
								Items = new[]
								{
									new OrderItemModel
									{
										PriceSubtotalAfterDistribution = 1000m,
									},
									new OrderItemModel
									{
										PriceSubtotalAfterDistribution = 10000m,
									},
								},
							},
						},
						OrderPointUse = 1000m,
					},
				},
				new
				{
					ErrorMessage = string.Empty,
					PriceSubtotalAfterDistributions = new[]
					{
						// 10(割引前商品小計) - 1000(商品割引額) * (10(割引前商品小計)/11110(割引前商品合計) = 10
						10m,
						// 100(割引前商品小計) - 1000(商品割引額) * (100(割引前商品小計)/11110(割引前商品合計) = 91
						91m,
						// 1000(割引前商品小計) - 1000(商品割引額) * (1000(割引前商品小計)/11110(割引前商品合計) = 910
						910m,
						// 10000(割引前商品小計) - 1000(商品割引額) * (10000(割引前商品小計)/11110(割引前商品合計) = 9099
						9099m,
					}
				},
				"正常系パターン"
			},
			new object[]
			{
				new
				{
					PointOptionEnabled = true,
				},
				new
				{
					Order = new OrderModel
					{
						Shippings = new[]
						{
							new OrderShippingModel
							{
								Items = new[]
								{
									new OrderItemModel
									{
										PriceSubtotalAfterDistribution = 10m,
									},
									new OrderItemModel
									{
										PriceSubtotalAfterDistribution = 100m,
									},
								},
							},
							new OrderShippingModel
							{
								Items = new[]
								{
									new OrderItemModel
									{
										PriceSubtotalAfterDistribution = 1000m,
									},
									new OrderItemModel
									{
										PriceSubtotalAfterDistribution = 10000m,
									},
								},
							},
						},
						OrderPointUse = 100000m,
					},
				},
				new
				{
					ErrorMessage = CommerceMessages.ERRMSG_MANAGER_DISCOUNT_LIMIT_ERROR,
					PriceSubtotalAfterDistributions = new[]
					{
						-81m,
						-800m,
						-8000m,
						-80009m,
					}
				},
				"エラーパターン"
			},
		};

		/// <summary>
		/// クーポン系計算テスト
		/// </summary>
		[Ignore]
		[DataTestMethod()]
		[DynamicData("m_tdCalculateCouponFamilyTest")]
		public void CalculateCouponFamilyTest(
			dynamic config,
			dynamic data,
			dynamic expected,
			string msg)
		{
			// モックによるドメイン層偽装
			var couponServiceMock = new Mock<ICouponService>();
			couponServiceMock.Setup(s => s.GetCoupon(It.IsAny<string>(), It.IsAny<string>()))
				.Returns(new CouponModel());
			DomainFacade.Instance.CouponService = couponServiceMock.Object;

			var productServiceMock = new Mock<IProductService>();
			productServiceMock.Setup(s => s.GetProductVariation(
				It.IsAny<string>(),
				It.IsAny<string>(),
				It.IsAny<string>(),
				It.IsAny<string>(),
				null
			)).Returns(new ProductModel());
			DomainFacade.Instance.ProductService = productServiceMock.Object;

			// モックによるユーティリティ偽装
			var mockCouponOptionUtility = new Mock<CouponOptionUtilityWrapper>();
			mockCouponOptionUtility.Setup(s => s.IsCouponApplyProduct(It.IsAny<CouponModel>(), It.IsAny<object>()))
				.Returns(true);
			CouponOptionUtilityWrapper.Instance = mockCouponOptionUtility.Object;

			using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), true))
			using (new TestConfigurator(Member.Of(() => Constants.OPERATIONAL_BASE_ISO_CODE), Constants.COUNTRY_ISO_CODE_JP))
			using (new TestConfigurator(Member.Of(() => Constants.GLOBAL_TRANSACTION_INCLUDED_TAX_FLAG), true))
			using (new TestConfigurator(Member.Of(() => Constants.W2MP_COUPON_OPTION_ENABLED), config.CouponOptionEnabled))
			{
				var order = (OrderModel)data.Order;

				var result = (string)new PrivateType(typeof(OrderCommon))
					.InvokeStatic(
						"CalculateCouponFamily",
						new object[]
						{
							order,
						});

				result.Should().Be((string)expected.ErrorMessage, string.Format("エラーメッセージ：{0}", msg));
				order.ShippingPriceDiscountAmount.Should().Be((decimal)expected.ShippingPriceDiscountAmount,
					string.Format("配送料割引金額：{0}", msg));

				var targetTestItems = order.Shippings.SelectMany(shipping => shipping.Items).ToArray();
				for (var i = 0; i < targetTestItems.Length; i++)
				{
					targetTestItems[i].PriceSubtotalAfterDistribution.Should().Be(
						(decimal)expected.PriceSubtotalAfterDistributions[i], string.Format("商品小計：{0}", msg));
				}
			}
		}

		public static object[] m_tdCalculateCouponFamilyTest = new[]
		{
			new object[]
			{
				new
				{
					CouponOptionEnabled = false,
				},
				new
				{
					Order = new OrderModel
					{
						Shippings = new[]
						{
							new OrderShippingModel
							{
								Items = new OrderItemModel[0]
							},
						},
						OrderCouponUse = 0m
					},
				},
				new
				{
					ErrorMessage = string.Empty,
					ShippingPriceDiscountAmount = 0m,
					PriceSubtotalAfterDistributions = new decimal[0]
				},
				"クーポンオプションOFFパターン"
			},
			new object[]
			{
				new
				{
					CouponOptionEnabled = true,
				},
				new
				{
					Order = new OrderModel
					{
						Shippings = new[]
						{
							new OrderShippingModel
							{
								Items = new OrderItemModel[0]
							},
						},
						Coupons = new OrderCouponModel[0],
						OrderCouponUse = 0m
					},
				},
				new
				{
					ErrorMessage = string.Empty,
					ShippingPriceDiscountAmount = 0m,
					PriceSubtotalAfterDistributions = new decimal[0]
				},
				"割引金額なしパターン"
			},
			new object[]
			{
				new
				{
					CouponOptionEnabled = true,
				},
				new
				{
					Order = new OrderModel
					{
						Shippings = new[]
						{
							new OrderShippingModel
							{
								Items = new[]
								{
									new OrderItemModel
									{
										PriceSubtotalAfterDistribution = 10m,
									},
									new OrderItemModel
									{
										PriceSubtotalAfterDistribution = 100m,
									},
								},
							},
							new OrderShippingModel
							{
								Items = new[]
								{
									new OrderItemModel
									{
										PriceSubtotalAfterDistribution = 1000m,
									},
									new OrderItemModel
									{
										PriceSubtotalAfterDistribution = 10000m,
									},
								},
							},
						},
						OrderCouponUse = 1000m,
						Coupons = new []
						{
							new OrderCouponModel
							{
								CouponId = "CPN01",
							},
						} 
					},
				},
				new
				{
					ErrorMessage = string.Empty,
					ShippingPriceDiscountAmount = 0m,
					PriceSubtotalAfterDistributions = new[]
					{
						// 10(割引前商品小計) - 1000(商品割引額) * (10(割引前商品小計)/11110(割引前商品合計) = 10
						10m,
						// 100(割引前商品小計) - 1000(商品割引額) * (100(割引前商品小計)/11110(割引前商品合計) = 91
						91m,
						// 1000(割引前商品小計) - 1000(商品割引額) * (1000(割引前商品小計)/11110(割引前商品合計) = 910
						910m,
						// 10000(割引前商品小計) - 1000(商品割引額) * (10000(割引前商品小計)/11110(割引前商品合計) = 9099
						9099m,
					}
				},
				"正常系パターン"
			},
			new object[]
			{
				new
				{
					CouponOptionEnabled = true,
				},
				new
				{
					Order = new OrderModel
					{
						Shippings = new[]
						{
							new OrderShippingModel
							{
								Items = new[]
								{
									new OrderItemModel
									{
										PriceSubtotalAfterDistribution = 10m,
									},
									new OrderItemModel	
									{
										PriceSubtotalAfterDistribution = 100m,
									},
								},
							},
							new OrderShippingModel
							{
								Items = new[]
								{
									new OrderItemModel
									{
										PriceSubtotalAfterDistribution = 1000m,
									},
									new OrderItemModel
									{
										PriceSubtotalAfterDistribution = 10000m,
									},
								},
							},
						},
						OrderCouponUse = 100000m,
						Coupons = new []
						{
							new OrderCouponModel
							{
								CouponId = "CPN01",
							},
						}
					},
				},
				new
				{
					ErrorMessage = CommerceMessages.ERRMSG_MANAGER_DISCOUNT_LIMIT_ERROR,
					ShippingPriceDiscountAmount = 0m,
					PriceSubtotalAfterDistributions = new[]
					{
						-81m,
						-800m,
						-8000m,
						-80009m,
					}
				},
				"エラーパターン"
			},
		};

		/// <summary>
		/// 定期会員割引額の按分計算テスト
		/// </summary>
		[Ignore]
		[DataTestMethod()]
		[DynamicData("m_tdCalculateFixedPurchaseMemberDiscountTest")]
		public void CalculateFixedPurchaseMemberDiscountTest(
			dynamic config,
			dynamic data,
			dynamic expected,
			string msg)
		{
			using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), true))
			using (new TestConfigurator(Member.Of(() => Constants.OPERATIONAL_BASE_ISO_CODE), Constants.COUNTRY_ISO_CODE_JP))
			using (new TestConfigurator(Member.Of(() => Constants.GLOBAL_TRANSACTION_INCLUDED_TAX_FLAG), true))
			{
				var order = (OrderModel)data.Order;

				var result = (string)new PrivateType(typeof(OrderCommon))
					.InvokeStatic(
						"CalculateFixedPurchaseMemberDiscount",
						new object[]
						{
								order,
						});

				result.Should().Be((string)expected.ErrorMessage, string.Format("エラーメッセージ：{0}", msg));

				var targetTestItems = order.Shippings
					.SelectMany(shipping => shipping.Items)
					.Where(product => (product.OrderSetpromotionNo == null)
									  && (product.IsFixedPurchaseItem == false)
									  && (string.IsNullOrEmpty(product.NoveltyId))).ToArray();
				for (var i = 0; i < targetTestItems.Length; i++)
				{
					targetTestItems[i].PriceSubtotalAfterDistribution.Should().Be(
						(decimal)expected.PriceSubtotalAfterDistributions[i], string.Format("商品小計：{0}", msg));
				}
			}
		}

		public static object[] m_tdCalculateFixedPurchaseMemberDiscountTest = new[]
		{
			new object[]
			{
				new { },
				new
				{
					Order = new OrderModel
					{
						Shippings = new[]
						{
							new OrderShippingModel
							{
								Items = new OrderItemModel[0]
							},
						},
						FixedPurchaseMemberDiscountAmount = 0m
					},
				},
				new
				{
					ErrorMessage = string.Empty,
					PriceSubtotalAfterDistributions = new decimal[0]
				},
				"割引金額なしパターン"
			},
			new object[]
			{
				new { },
				new
				{
					Order = new OrderModel
					{
						Shippings = new[]
						{
							new OrderShippingModel
							{
								Items = new[]
								{
									new OrderItemModel
									{
										// 対象
										PriceSubtotalAfterDistribution = 10m,
										FixedPurchaseProductFlg = Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_OFF,
									},
									new OrderItemModel
									{
										// 対象
										PriceSubtotalAfterDistribution = 100m,
										FixedPurchaseProductFlg = Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_OFF,
									},
									new OrderItemModel
									{
										// 対象外
										PriceSubtotalAfterDistribution = 200m,
										FixedPurchaseProductFlg = Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON,
									},
								},
							},
							new OrderShippingModel
							{
								Items = new[]
								{
									// 対象
									new OrderItemModel
									{
										PriceSubtotalAfterDistribution = 1000m,
										FixedPurchaseProductFlg = Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_OFF,
									},
									// 対象
									new OrderItemModel
									{
										PriceSubtotalAfterDistribution = 10000m,
										FixedPurchaseProductFlg = Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_OFF,
									},
								},
							},
						},
						FixedPurchaseMemberDiscountAmount = 1000m,
					},
				},
				new
				{
					ErrorMessage = string.Empty,
					PriceSubtotalAfterDistributions = new[]
					{
						// 10(割引前商品小計) - 1000(商品割引額) * (10(割引前商品小計)/11110(割引前商品合計) = 10
						10m,
						// 100(割引前商品小計) - 1000(商品割引額) * (100(割引前商品小計)/11110(割引前商品合計) = 91
						91m,
						// 1000(割引前商品小計) - 1000(商品割引額) * (1000(割引前商品小計)/11110(割引前商品合計) = 910
						910m,
						// 10000(割引前商品小計) - 1000(商品割引額) * (10000(割引前商品小計)/11110(割引前商品合計) = 9099
						9099m,
					}
				},
				"正常系パターン"
			},
			new object[]
			{
				new { },
				new
				{
					Order = new OrderModel
					{
						Shippings = new[]
						{
							new OrderShippingModel
							{
								Items = new[]
								{
									new OrderItemModel
									{
										// 対象
										PriceSubtotalAfterDistribution = 10m,
										FixedPurchaseProductFlg = Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_OFF,
									},
									new OrderItemModel
									{
										// 対象
										PriceSubtotalAfterDistribution = 100m,
										FixedPurchaseProductFlg = Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_OFF,
									},
									new OrderItemModel
									{
										// 対象外
										PriceSubtotalAfterDistribution = 200m,
										FixedPurchaseProductFlg = Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON,
									},
								},
							},
							new OrderShippingModel
							{
								Items = new[]
								{
									new OrderItemModel
									{
										// 対象
										PriceSubtotalAfterDistribution = 1000m,
										FixedPurchaseProductFlg = Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_OFF,
									},
									new OrderItemModel
									{
										// 対象
										PriceSubtotalAfterDistribution = 10000m,
										FixedPurchaseProductFlg = Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_OFF,
									},
								},
							},
						},
						FixedPurchaseMemberDiscountAmount = 100000m,
					},
				},
				new
				{
					ErrorMessage = CommerceMessages.ERRMSG_MANAGER_DISCOUNT_LIMIT_ERROR,
					PriceSubtotalAfterDistributions = new[]
					{
						-81m,
						-800m,
						-8000m,
						-80009m,
					}
				},
				"エラーパターン"
			},
		};


		/// <summary>
		/// 税額の計算テスト
		/// </summary>
		[Ignore]
		[DataTestMethod()]
		[DynamicData("m_tdCalculateTaxPriceTest")]
		public void CalculateTaxPriceTest(
			dynamic config,
			dynamic data,
			dynamic expected,
			string msg)
		{
			using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), false))
			using (new TestConfigurator(Member.Of(() => Constants.OPERATIONAL_BASE_ISO_CODE), Constants.COUNTRY_ISO_CODE_JP))
			using (new TestConfigurator(Member.Of(() => Constants.GLOBAL_TRANSACTION_INCLUDED_TAX_FLAG), false))
			{
				var order = (OrderModel)data.Order;

				new PrivateType(typeof(OrderCommon))
					.InvokeStatic(
						"CalculateTaxPrice",
						new object[]
						{
								order,
								data.TaxExcludedFractionRounding,
						});

				var allExceptedPriceInfoCreated =
					((OrderPriceByTaxRateModel[])expected.OrderPriceByTaxRateExpected)
					.FirstOrDefault(pibtye =>
						(order.OrderPriceByTaxRates.Any(pibty => pibtye.KeyTaxRate == pibty.KeyTaxRate) == false));
				allExceptedPriceInfoCreated.Should().BeNull("予期された税率の金額情報が生成されていません。");
				foreach (var priceInfo in order.OrderPriceByTaxRates)
				{
					var priceInfoExpected = ((OrderPriceByTaxRateModel[])expected.OrderPriceByTaxRateExpected)
						.FirstOrDefault(pibtye => (pibtye.KeyTaxRate == priceInfo.KeyTaxRate));
					priceInfoExpected.Should().NotBeNull(string.Format(
						"{0}:税率{1}%:予期されない税率の金額情報",
						priceInfo.KeyTaxRate,
						msg));
					priceInfo.PriceSubtotalByRate.Should().Be(priceInfoExpected.PriceSubtotalByRate,
						string.Format("{0}:税率{1}%:商品小計", priceInfo.KeyTaxRate, msg));
					priceInfo.PriceShippingByRate.Should().Be(priceInfoExpected.PriceShippingByRate,
						string.Format("{0}:税率{1}%:配送料", priceInfo.KeyTaxRate, msg));
					priceInfo.PricePaymentByRate.Should().Be(priceInfoExpected.PricePaymentByRate,
						string.Format("{0}:税率{1}%:決済手数料", priceInfo.KeyTaxRate, msg));
					priceInfo.PriceTotalByRate.Should().Be(priceInfoExpected.PriceTotalByRate,
						string.Format("{0}:税率{1}%:合計", priceInfo.KeyTaxRate, msg));
					priceInfo.ReturnPriceCorrectionByRate.Should().Be(priceInfoExpected.ReturnPriceCorrectionByRate,
						string.Format("{0}:税率{1}%:返品用金額補正", priceInfo.KeyTaxRate, msg));
					priceInfo.TaxPriceByRate.Should().Be(priceInfoExpected.TaxPriceByRate,
						string.Format("{0}:税率{1}%:消費税額", priceInfo.KeyTaxRate, msg));
				}

				order.OrderPriceTax.Should().Be(
					(decimal)expected.OrderPriceTaxExpected,
					string.Format("{0}:カート消費税額合計", msg));
				order.OrderPriceSubtotalTax.Should().Be(
					(decimal)expected.OrderPriceSubtotalTaxExpected,
					string.Format("{0}:カート商品税額小計", msg));
			}
		}

		public static object[] m_tdCalculateTaxPriceTest = new[]
		{
			new object[]
			{
				new { },
				new
				{
					TaxExcludedFractionRounding = TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN,
					Order = new OrderModel
					{
						Shippings = new[]
						{
							new OrderShippingModel
							{
								ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP,
								Items = new []
								{
									new OrderItemModel
									{
										ProductPrice = 11005m,
										ItemQuantity = 2,
										ProductTaxRate = 10m,
										ItemPriceRegulation = -500m,
										PriceSubtotalAfterDistribution = 20505m,
										ItemPriceTax = 2200m,
									},
									new OrderItemModel
									{
										ProductPrice = 110001m,
										ItemQuantity = 4,
										ProductTaxRate = 8m,
										ItemPriceRegulation = -10000m,
										PriceSubtotalAfterDistribution = 410001m,
										ItemPriceTax = 32000m,
									},
								}
							},
						},
						OrderPriceByTaxRates = new OrderPriceByTaxRateModel[0],
						OrderPriceShipping = 115m,
						ShippingPriceDiscountAmount = 10m,
						ShippingTaxRate = 10m,
						OrderPriceExchange = 1105m,
						PaymentPriceDiscountAmount = 100m,
						PaymentTaxRate = 10m,
					},
				},
				new
				{
					OrderPriceByTaxRateExpected = new[]
					{
						new OrderPriceByTaxRateModel
						{
							KeyTaxRate = 8m,
							PriceSubtotalByRate = 400001m,
							PricePaymentByRate = 0m,
							PriceShippingByRate = 0m,
							ReturnPriceCorrectionByRate = 0m,
							PriceTotalByRate = 400001m,
							TaxPriceByRate = 29629m,
						},
						new OrderPriceByTaxRateModel
						{
							KeyTaxRate = 10m,
							PriceSubtotalByRate = 20005m,
							PricePaymentByRate = 1005m,
							PriceShippingByRate = 105m,
							ReturnPriceCorrectionByRate = 0m,
							PriceTotalByRate = 21115m,
							TaxPriceByRate = 1919m,
						},
					},
					
					// OrderPriceTaxExpected = 29629(8%消費税額) + 1919m(10%消費税額) = 31548
					OrderPriceTaxExpected = 31548m,
					// OrderPriceSubtotalTaxExpected、全商品の単価から計算した消費税額の合計
					// 商品1：11005(単価) * 0.1(税率) = 1100.5 →(小数点以下端数処理) 1100(単価の消費税額) * 2(個数) = 2200(商品消費税合計)
					// 商品2：110001(単価) * 0.08(税率) = 8800.08 →(小数点以下端数処理) 8800(単価の消費税額) * 4(個数) = 32000(商品消費税合計)
					// OrderPriceSubtotalTaxExpected = 2200 + 32000 = 342000
					OrderPriceSubtotalTaxExpected = 34200m,
				},
				"全て国内配送パターン"
			},
			new object[]
			{
				new { },
				new
				{
					TaxExcludedFractionRounding = TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN,
					Order = new OrderModel
					{
						Shippings = new[]
						{
							new OrderShippingModel
							{
								ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_US,
								Items = new []
								{
									new OrderItemModel
									{
										ProductPrice = 11005m,
										ItemQuantity = 2,
										ProductTaxRate = 10m,
										ItemPriceRegulation = -500m,
										PriceSubtotalAfterDistribution = 20505m,
										ItemPriceTax = 2200m,
									},
									new OrderItemModel
									{
										ProductPrice = 110001m,
										ItemQuantity = 2,
										ProductTaxRate = 8m,
										ItemPriceRegulation = -5000m,
										PriceSubtotalAfterDistribution = 205001m,
										ItemPriceTax = 16000m,
									},
								}
							},
							new OrderShippingModel
							{
								ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_US,
								Items = new []
								{
									new OrderItemModel
									{
										ProductPrice = 110001m,
										ItemQuantity = 2,
										ProductTaxRate = 8m,
										ItemPriceRegulation = -5000m,
										PriceSubtotalAfterDistribution = 205000m,
										ItemPriceTax = 16000m,
									},
								}
							},
						},
						OrderPriceByTaxRates = new OrderPriceByTaxRateModel[0],
						OrderPriceShipping = 115m,
						ShippingPriceDiscountAmount = 10m,
						ShippingTaxRate = 10m,
						OrderPriceExchange = 1105m,
						PaymentPriceDiscountAmount = 100m,
						PaymentTaxRate = 10m,
					},
				},
				new
				{
					OrderPriceByTaxRateExpected = new[]
					{
						new OrderPriceByTaxRateModel
						{
							KeyTaxRate = 8m,
							PriceSubtotalByRate = 400001m,
							PricePaymentByRate = 0m,
							PriceShippingByRate = 0m,
							ReturnPriceCorrectionByRate = 0m,
							PriceTotalByRate = 400001m,
							TaxPriceByRate = 0m,
						},
						new OrderPriceByTaxRateModel
						{
							KeyTaxRate = 10m,
							PriceSubtotalByRate = 20005m,
							PricePaymentByRate = 1005m,
							PriceShippingByRate = 105m,
							ReturnPriceCorrectionByRate = 0m,
							PriceTotalByRate = 21115m,
							TaxPriceByRate = 100m,
						},
					},
					OrderPriceTaxExpected = 100m,
					OrderPriceSubtotalTaxExpected = 0m,
				},
				"全て海外配送パターン"
			},
			new object[]
			{
				new { },
				new
				{
					TaxExcludedFractionRounding = TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN,
					Order = new OrderModel
					{
						Shippings = new[]
						{
							new OrderShippingModel
							{
								ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP,
								Items = new []
								{
									new OrderItemModel
									{
										ProductPrice = 11005m,
										ItemQuantity = 2,
										ProductTaxRate = 10m,
										ItemPriceRegulation = -500m,
										PriceSubtotalAfterDistribution = 20505m,
										ItemPriceTax = 2200m,
									},
									new OrderItemModel
									{
										ProductPrice = 110001m,
										ItemQuantity = 2,
										ProductTaxRate = 8m,
										ItemPriceRegulation = -5000m,
										PriceSubtotalAfterDistribution = 205001m,
										ItemPriceTax = 16000m,
									},
								}
							},
							new OrderShippingModel
							{
								ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_US,
								Items = new []
								{
									new OrderItemModel
									{
										ProductPrice = 110001m,
										ItemQuantity = 2,
										ProductTaxRate = 8m,
										ItemPriceRegulation = -5000m,
										PriceSubtotalAfterDistribution = 205000m,
										ItemPriceTax = 16000m,
									},
								}
							},
						},
						OrderPriceByTaxRates = new OrderPriceByTaxRateModel[0],
						OrderPriceShipping = 115m,
						ShippingPriceDiscountAmount = 10m,
						ShippingTaxRate = 10m,
						OrderPriceExchange = 1105m,
						PaymentPriceDiscountAmount = 100m,
						PaymentTaxRate = 10m,
					},
				},
				new
				{
					OrderPriceByTaxRateExpected = new[]
					{
						new OrderPriceByTaxRateModel
						{
							KeyTaxRate = 8m,
							PriceSubtotalByRate = 400001m,
							PricePaymentByRate = 0m,
							PriceShippingByRate = 0m,
							ReturnPriceCorrectionByRate = 0m,
							PriceTotalByRate = 400001m,
							// 一部商品が海外配送のため、海外配送商品の商品価格は考慮せずに消費税額を計算する
							// TaxPrice = 200001(「ShippingCountryIsoCode = JP」の割引後価格) * 8 / 108 = 14814.8 →(端数処理) 14814
							TaxPriceByRate = 14814m,
						},
						new OrderPriceByTaxRateModel
						{
							KeyTaxRate = 10m,
							PriceSubtotalByRate = 20005m,
							PricePaymentByRate = 1005m,
							PriceShippingByRate = 105m,
							ReturnPriceCorrectionByRate = 0m,
							PriceTotalByRate = 21115m,
							TaxPriceByRate = 1919m,
						},
					},
					OrderPriceTaxExpected = 16733m,
					OrderPriceSubtotalTaxExpected = 18200m,
				},
				"一部は国内配送、一部は海外配送パターン"
			},
		};
	}
}
