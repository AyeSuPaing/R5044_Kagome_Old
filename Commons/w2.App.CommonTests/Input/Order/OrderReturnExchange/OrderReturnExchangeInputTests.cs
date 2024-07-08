using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using w2.App.Common;
using w2.App.Common.Input.Order.OrderReturnExchange;
using w2.App.Common.Order;
using w2.App.CommonTests._Helper;

namespace w2.App.CommonTests.Input.Order.OrderReturnExchange
{
	/// <summary>
	/// OrderReturnExchangeInputのテスト
	/// </summary>
	[TestClass()]
	[Ignore]
	public class OrderReturnExchangeInputTests : AppTestClassBase
	{
		private OrderReturnExchangeInput m_testClass;

		/// <summary>
		/// 初期化
		/// </summary>
		[TestInitialize]
		public void TestInitialize()
		{
			m_testClass = new OrderReturnExchangeInput();
		}

		/// <summary>
		/// 返品交換商品合計の計算テスト
		/// ・返品交換商品合計 = 返品商品合計 + 交換商品合計 となること
		/// </summary>
		[DataTestMethod()]
		[DynamicData("m_tdCalculateReturnExchangeOrderPriceSubTotalTest")]
		public void CalculateReturnExchangeOrderPriceSubTotalTest(
				decimal returnOrderPriceSubTotal,
				decimal exchangeOrderPriceSubTotal,
				decimal expected)
		{
			var result = m_testClass.CalculateReturnExchangeOrderPriceSubTotal(returnOrderPriceSubTotal, exchangeOrderPriceSubTotal);
			result.Should().Be(expected, "返品交換商品合計");
		}

		public static object[] m_tdCalculateReturnExchangeOrderPriceSubTotalTest = new[]
		{
			new object[] { 10m, 1m, 11m }
		};

		/// <summary>
		/// 返品交換商品消費税合計の計算テスト
		/// ・返品交換商品消費税合計 = 返品商品消費税合計 + 交換商品消費税合計 となること
		/// </summary>
		[DataTestMethod()]
		[DynamicData("m_tdCalculateReturnExchangeOrderPriceTaxSubTotalTest")]
		public void CalculateReturnExchangeOrderPriceTaxSubTotalTest(
			decimal returnOrderPriceTaxSubTotal,
			decimal exchangeOrderPriceTaxSubTotal,
			decimal expected)
		{
			var result = m_testClass.CalculateReturnExchangeOrderPriceTaxSubTotal(returnOrderPriceTaxSubTotal, exchangeOrderPriceTaxSubTotal);
			result.Should().Be(expected, "返品交換商品消費税合計");
		}

		public static object[] m_tdCalculateReturnExchangeOrderPriceTaxSubTotalTest = new[]
		{
			new object[] { 10m, 1m, 11m }
		};

		/// <summary>
		/// 最終合計金額の計算テスト
		/// ・最終合計金額 = 返品交換商品合計(税込額) - ポイント調整金額 + 返品用金額補正合計 となること
		/// </summary>
		[DataTestMethod()]
		[DynamicData("m_tdCalculateReturnExchangeOrderPriceTotalTest")]
		public void CalculateReturnExchangeOrderPriceTotalTest(
			decimal returnExchangeOrderPriceSubTotal,
			decimal returnExchangeOrderPriceTaxSubTotal,
			decimal adjustmentPointPrice,
			decimal returnPriceCorrectionTotal,
			decimal expected)
		{
			var result = m_testClass.CalculateReturnExchangeOrderPriceTotal(
				returnExchangeOrderPriceSubTotal,
				returnExchangeOrderPriceTaxSubTotal,
				adjustmentPointPrice,
				returnPriceCorrectionTotal);
			result.Should().Be(expected, "最終合計金額");
		}

		public static object[] m_tdCalculateReturnExchangeOrderPriceTotalTest = new[]
		{
			new object[] { 1000m, 100m, 10m, 1m, 991m }
		};

		/// <summary>
		/// 最終請求金額の計算テスト
		/// ・最終請求金額 = 前回の最終請求金額 + 最終合計金額 となること
		/// </summary>
		[DataTestMethod()]
		[DynamicData("m_tdCalculateReturnExchangeLastBilledAmountTest")]
		public void CalculateReturnExchangeLastBilledAmountTest(
			decimal returnExchangeOrderLastBilledAmount,
			decimal returnExchangeOrderPriceTotal,
			decimal expected)
		{
			var result = m_testClass.CalculateReturnExchangeLastBilledAmount(returnExchangeOrderLastBilledAmount, returnExchangeOrderPriceTotal);
			result.Should().Be(expected, "最終請求金額");
		}

		public static object[] m_tdCalculateReturnExchangeLastBilledAmountTest = new[]
		{
			new object[] { 10m, 1m, 11m }
		};

		/// <summary>
		/// 返品商品合計の計算テスト
		/// ・返品交換ステータス = 返品対象の商品合計 となること
		/// </summary>
		//[DataTestMethod()]
		//[DynamicData("m_tdCalculateReturnOrderPriceSubTotalTest")]
		//public void CalculateReturnOrderPriceSubTotalTest(
		//		ReturnOrderItem[] returnOrderItems,
		//		decimal expected)
		//{
		//	var result = m_testClass.CalculateReturnOrderPriceSubTotal(returnOrderItems);
		//	result.Should().Be(expected, "返品商品合計");
		//}

		public static object[] m_tdCalculateReturnOrderPriceSubTotalTest = new[]
		{
			new object[]
			{
				new[]
				{
					// 対象
					new ReturnOrderItem
					{
						ReturnStatus = Constants.FLG_ORDER_RETURN_STATUS_RETURN_TARGET,
						ProductPrice = 100m,
						ItemQuantity = 1,
					},
					// 対象外
					new ReturnOrderItem
					{
						ReturnStatus = Constants.FLG_ORDER_RETURN_STATUS_RETURN_UNTARGET,
						ProductPrice = 100m,
						ItemQuantity = 1,
					},
					// 対象外
					new ReturnOrderItem
					{
						ReturnStatus = Constants.FLG_ORDER_RETURN_STATUS_RETURN_COMPLETE,
						ProductPrice = 100m,
						ItemQuantity = 1,
					},
					// 対象外
					new ReturnOrderItem
					{
						ReturnStatus = Constants.FLG_ORDER_RETURN_STATUS_RETURN_EXCHANGED,
						ProductPrice = 100m,
						ItemQuantity = 1,
					},
					// 対象
					new ReturnOrderItem
					{
						ReturnStatus = Constants.FLG_ORDER_RETURN_STATUS_RETURN_TARGET,
						ProductPrice = 10m,
						ItemQuantity = 2,
					},
					// 対象外
					new ReturnOrderItem
					{
						ReturnStatus = Constants.FLG_ORDER_RETURN_STATUS_RETURN_UNTARGET,
						ProductPrice = 10m,
						ItemQuantity = 2,
					},
					// 対象外
					new ReturnOrderItem
					{
						ReturnStatus = Constants.FLG_ORDER_RETURN_STATUS_RETURN_COMPLETE,
						ProductPrice = 10m,
						ItemQuantity = 2,
					},
					// 対象外
					new ReturnOrderItem
					{
						ReturnStatus = Constants.FLG_ORDER_RETURN_STATUS_RETURN_EXCHANGED,
						ProductPrice = 10m,
						ItemQuantity = 2,
					},
				},
				120m
			}
		};
	}
}
