using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using w2.App.Common;
using w2.App.Common.Util;
using w2.App.CommonTests._Helper;
using w2.Domain.ProductFixedPurchaseDiscountSetting;

namespace w2.App.CommonTests.Util
{
	/// <summary>
	/// PriceCalculatorのテスト
	/// </summary>
	[TestClass()]
	[Ignore]
	public class PriceCalculatorTests : AppTestClassBase
	{
		/// <summary>
		/// 明細金額計算テスト
		/// ・商品単価と個数から小計金額が計算されること
		/// </summary>
		[DataTestMethod()]
		public void GetItemPriceTest()
		{
			var result = PriceCalculator.GetItemPrice(
				unitPrice:100,
				itemQuantity:2);

			// ・商品単価と個数から小計金額が計算されること
			result.Should().Be(200, "小計金額の計算");
		}

		/// <summary>
		/// 受注編集時の最終利用ポイント計算テスト
		/// ・前回の最終利用ポイント数に、利用ポイント変更の差額をプラスして最終利用ポイント数が計算されること
		/// </summary>
		[DataTestMethod()]
		public void GetLastOrderPointUseTest()
		{
			var result = PriceCalculator.GetLastOrderPointUse(
				lastOrderPointUseOld:100,
				orderPointUseOld:100,
				orderPointUse:200);

			// ・前回の最終利用ポイント数に、利用ポイント変更の差額をプラスして最終利用ポイント数が計算されること
			result.Should().Be(200, "最終利用ポイント数の計算");
		}

		/// <summary>
		/// 受注編集時の利用可能ポイント数計算テスト
		/// ・現在の利用可能ポイント数に、変更前の利用ポイント数がプラスされた値が最終利用ポイント数となること
		/// ・付与ポイント種別が「本ポイント」かつ、付与ポイント数が変更前より減っていた場合は減算分を利用可能ポイント数から減らされていること
		/// ※引数として以下の値が渡された場合、正常に計算された戻り値が返却される
		/// ・第一(現在の利用可能ポイント数)：100 第二(利用ポイント数(変更前))：200 第三(ポイント付与数(変更後)):200  第四(ポイント付与数(変更前)): 300 第五(付与ポイント種別): 本ポイント → 戻り値(利用可能ポイント数)：200
		/// ・第一(現在の利用可能ポイント数)：100 第二(利用ポイント数(変更前))：200 第三(ポイント付与数(変更後)):300  第四(ポイント付与数(変更前)): 200 第五(付与ポイント種別): 本ポイント → 戻り値(利用可能ポイント数)：300
		/// ・第一(現在の利用可能ポイント数)：100 第二(利用ポイント数(変更前))：200 第三(ポイント付与数(変更後)):200  第四(ポイント付与数(変更前)): 300 第五(付与ポイント種別): 仮ポイント → 戻り値(利用可能ポイント数)：300
		/// </summary>
		[DataTestMethod()]
		[DataRow(Constants.FLG_USERPOINT_POINT_TYPE_COMP, 200, 100, 200, 300, 200, "本ポイント付与 付与ポイント減算あり")]
		[DataRow(Constants.FLG_USERPOINT_POINT_TYPE_COMP, 200, 100, 300, 200, 300, "本ポイント付与 付与ポイント減算なし")]
		[DataRow(Constants.FLG_USERPOINT_POINT_TYPE_TEMP, 200, 100, 200, 300, 300, "仮ポイント付与")]
		public void GetUserPointUsableTest(
			string pointType,
			int userPointUsablNow,
			int orderPointUseOld,
			int orderPointAddNew,
			int orderPointAddOld,
			int userPointUsableResult,
			string msg)
		{
			var result = PriceCalculator.GetUserPointUsable(
				userPointUsablNow,
				orderPointUseOld,
				orderPointAddNew,
				orderPointAddOld,
				pointType);

			// 受注編集時の利用可能ポイント数計算テスト
			// ・現在の利用可能ポイント数に、変更前の利用ポイント数がプラスされた値が最終利用ポイント数となること
			// ・付与ポイント種別が「本ポイント」かつ、付与ポイント数が変更前より減っていた場合は減算分を利用可能ポイント数から減らされていること
			result.Should().Be(userPointUsableResult, msg);
		}

		/// <summary>
		/// 割引額合計計算テスト
		/// ・割引額合計 = 会員ランク割引 + セットプロモーション商品割引額 + セットプロモーション配送料割引額 + セットプロモーション決済手数料割引額 + ポイント利用額 + クーポン割引額 + 定期会員割引額 + 定期購入割引額 となること
		/// </summary>
		[DataTestMethod()]
		[DynamicData("m_getOrderPriceDiscountTotalTest")]
		public void GetOrderPriceDiscountTotalTest(
			decimal memberRankDiscountPrice,
			decimal setpromotionProductDiscountAmount,
			decimal setpromotionShippingChargeDiscountAmount,
			decimal setpromotionPaymentChargeDiscountAmount,
			decimal orderPointUseYen,
			decimal orderCouponUse,
			decimal fixedPurchaseMemberDiscountAmount,
			decimal fixedPurchaseDiscountPrice,
			decimal expected)
		{
			var result = PriceCalculator.GetOrderPriceDiscountTotal(memberRankDiscountPrice,
				setpromotionProductDiscountAmount,
				setpromotionShippingChargeDiscountAmount,
				setpromotionPaymentChargeDiscountAmount,
				orderPointUseYen,
				orderCouponUse,
				fixedPurchaseMemberDiscountAmount,
				fixedPurchaseDiscountPrice);
			result.Should().Be(expected, "割引額合計");
		}

		public static object[] m_getOrderPriceDiscountTotalTest = new []
		{
			new object[] { 1m, 10m, 100m, 1000m, 10000m, 100000m, 1000000m, 10000000m, 11111111m }
		};

		/// <summary>
		/// 価格の按分計算テスト
		/// ・按分後の価格 = 按分対象の価格 * (割合の分子となる価格 / 割合の分母となる価格) となること
		/// ・ゼロ除算の場合は0を返却すること
		/// </summary>
		[DataTestMethod()]
		[DynamicData("m_getDistributedPriceTest")]
		public void GetDistributedPriceTest(
			decimal targetPrice,
			decimal denominatorPrice,
			decimal numeratorPrice,
			decimal expected,
			string msg)
		{
			var result = PriceCalculator.GetDistributedPrice(targetPrice, denominatorPrice, numeratorPrice);
			result.Should().Be(expected, string.Format("按分後の価格：{0}", msg));
		}

		public static object[] m_getDistributedPriceTest = new []
		{
			new object[] { 1234m, 10m, 100m, 123.4m, "正常系"},
			new object[] { 1000m, 10m, 0m, 0m, "異常系（0除算）"},
		};

		/// <summary>
		/// 商品定期購入割引設定からの割引金額取得テスト
		/// ・DiscountTypeが空もしくはDiscountValueがNULLの場合 → 値引き額 = 0
		/// ・DiscountTypeが「YEN」の場合 → 値引き額 = 「discountValue * itemQuantity」
		/// ・DiscountTypeが「PERCENT」の場合 → 値引き額 = 「itemPrice * discountValue / 100」
		/// ・「計算された値引き額 > itemPrice」の場合 → 値引き額 = 「itemPrice」
		/// </summary>
		[DataTestMethod()]
		[DynamicData("m_tdGetFixedPurchaseDiscountPriceTest")]
		public void GetFixedPurchaseDiscountPriceTest(
			dynamic config,
			dynamic data,
			dynamic expected,
			string msg)
		{
			var result = PriceCalculator.GetFixedPurchaseDiscountPrice(
				(string)data.DiscountType,
				(decimal?)data.DiscountValue,
				(decimal)data.ItemPrice,
				(int)data.ItemQuantity);

			// 商品定期購入割引設定からの割引金額取得テスト
			result.Should().Be((decimal)expected.DiscountPrice, string.Format("商品定期購入割引の価格：{0}", msg));
		}

		public static object[] m_tdGetFixedPurchaseDiscountPriceTest = new[]
		{
			new object[]
			{
				new	{},
				new
				{
					DiscountValue = (decimal?)null,
					DiscountType = Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_YEN,
					ItemPrice = 0m,
					ItemQuantity = 1,
				},
				new
				{
					DiscountPrice = 0m
				},
				"設定値「値引」：NULL"
			},
			new object[]
			{
				new {},
				new
				{
					DiscountValue = 1m,
					DiscountType = "",
					ItemPrice = 0m,
					ItemQuantity = 1,
				},
				new
				{
					DiscountPrice = 0m
				},
				"設定値「値引タイプ」：空"
			},
			new object[]
			{
				new {},
				new
				{
					DiscountValue = 10m,
					DiscountType = Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_YEN,
					ItemPrice = 100m,
					ItemQuantity = 2,
				},
				new
				{
					DiscountPrice = 20m
				},
				"正常系：固定金額値引き"
			},
			new object[]
			{
				new {},
				new
				{
					DiscountValue = 20m,
					DiscountType = Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_PERCENT,
					ItemPrice = 100m,
					ItemQuantity = 1,
				},
				new
				{
					DiscountPrice = 20m
				},
				"正常系：割合金額値引き"
			},
			new object[]
			{
				new {},
				new
				{
					DiscountValue = 110m,
					DiscountType = Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_YEN,
					ItemPrice = 100m,
					ItemQuantity = 1,
				},
				new
				{
					DiscountPrice = 100m
				},
				"正常系：値引き額が上限金額以上"
			},
		};
	}
}
