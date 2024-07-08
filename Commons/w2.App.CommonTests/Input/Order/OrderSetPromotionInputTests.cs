using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using w2.App.Common.Input.Order;
using w2.App.CommonTests._Helper;
using w2.Domain.Order;

namespace w2.App.CommonTests.Input.Order
{
	/// <summary>
	/// OrderSetPromotionInputのテスト
	/// </summary>
	[TestClass()]
	[Ignore]
	public class OrderSetPromotionInputTests : AppTestClassBase
	{
		/// <summary>
		/// OrderSetPromotionInputコンストラクタ実行テスト
		/// ・インスタンスを生成した際に例外がスローされないこと
		/// </summary>
		[DataTestMethod()]
		public void OrderSetPromotionInputTest()
		{
			// ・インスタンスを生成した際に例外をスローしないこと
			Action act = () => new OrderSetPromotionInput();
			Assert.ThrowsException<AssertFailedException>(() => Assert.ThrowsException<Exception>(act, "OrderSetPromotionInputインスタンスの生成"));
		}

		/// <summary>
		/// OrderSetPromotionModelからのOrderSetPromotionInputインスタンス生成テスト
		/// ・モデルから生成したOrderSetPromotionInputと、引数無しで生成したOrderSetPromotionInputの各プロパティ値が異なること
		/// </summary>
		[DataTestMethod()]
		public void OrderSetPromotionInputByModelTest()
		{
			var val = 1;
			var model = new OrderSetPromotionModel
			{
				OrderId = (val++).ToString(),
				OrderSetpromotionNo = val++,
				UndiscountedProductSubtotal = val++,
				ProductDiscountFlg = (val++).ToString(),
				ProductDiscountAmount = val++,
				ShippingChargeFreeFlg = (val++).ToString(),
				ShippingChargeDiscountAmount = val++,
				PaymentChargeFreeFlg = (val++).ToString(),
				PaymentChargeDiscountAmount = val++,
			};
			var orderSetPromotionInputEmpty = new OrderSetPromotionInput();
			var orderSetPromotionInputByModel = new OrderSetPromotionInput(model);

			// ・モデルから生成したOrderSetPromotionInputと、引数無しで生成したOrderSetPromotionInputの各プロパティ値が異なること
			foreach (var key in orderSetPromotionInputByModel.DataSource.Keys)
			{
				orderSetPromotionInputByModel.DataSource[key].Should().NotBe(orderSetPromotionInputEmpty.DataSource[key], " 値チェック：" + key);
			}
		}

		/// <summary>
		/// Modelオブジェクト生成テスト
		/// ・生成したOrderSetPromotionModelの各プロパティ値が、コンストラクタから生成したModelと異なっていること
		/// </summary>
		[DataTestMethod()]
		public void CreateModelTest()
		{
			var val = 1;
			var input = new OrderSetPromotionInput
			{
				OrderId = (val++).ToString(),
				OrderSetpromotionNo = (val++).ToString(),
				UndiscountedProductSubtotal = (val++).ToString(),
				ProductDiscountFlg = (val++).ToString(),
				ProductDiscountAmount = (val++).ToString(),
				ShippingChargeFreeFlg = (val++).ToString(),
				ShippingChargeDiscountAmount = (val++).ToString(),
				PaymentChargeFreeFlg = (val++).ToString(),
				PaymentChargeDiscountAmount = (val++).ToString(),
			};

			var orderSetPromotionModelEmpty = new OrderSetPromotionModel();
			var orderSetPromotionModelByInput = input.CreateModel();

			// ・生成したOrderSetPromotionModelの各プロパティ値が、コンストラクタから生成したModelと異なっていること
			foreach (var key in orderSetPromotionModelByInput.DataSource.Keys)
			{
				orderSetPromotionModelByInput.DataSource[key].Should().NotBe(orderSetPromotionModelEmpty.DataSource[key], " 値チェック：" + key);
			}
		}
	}
}
