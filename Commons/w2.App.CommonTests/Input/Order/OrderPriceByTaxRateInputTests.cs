using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using w2.App.Common.Input.Order;
using w2.App.CommonTests._Helper;
using w2.Domain.Order;

namespace w2.App.CommonTests.Input.Order
{
	/// <summary>
	/// OrderPriceByTaxRateInputのテスト
	/// </summary>
	[TestClass()]
	[Ignore]
	public class OrderPriceByTaxRateInputTests : AppTestClassBase
	{
		/// <summary>
		/// OrderPriceByTaxRateInputコンストラクタ実行テスト
		/// ・インスタンスを生成した際に例外がスローされないこと
		/// </summary>
		[DataTestMethod()]
		public void OrderPriceByTaxRateInputTest()
		{
			// ・インスタンスを生成した際に例外をスローしないこと
			Action act = () => new OrderPriceByTaxRateInput();
			Assert.ThrowsException<AssertFailedException>(() => Assert.ThrowsException<Exception>(act, "OrderPriceByTaxRateInputインスタンスの生成"));
		}

		/// <summary>
		/// OrderPriceByTaxRateModelからのOrderPriceByTaxRateInputインスタンス生成テスト
		/// ・モデルから生成したOrderPriceByTaxRateInputと、引数無しで生成したOrderPriceByTaxRateInputの各プロパティ値が異なること
		/// </summary>
		[DataTestMethod()]
		public void OrderPriceByTaxRateInputByModelTest()
		{
			var val = 1;
			var model = new OrderPriceByTaxRateModel
			{
				OrderId = (val++).ToString(),
				KeyTaxRate = val++,
				PriceSubtotalByRate = val++,
				PriceShippingByRate = val++,
				PricePaymentByRate = val++,
				PriceTotalByRate = val++,
				TaxPriceByRate = val++,
				ReturnPriceCorrectionByRate = val++,
				DateCreated = DateTime.Parse("2020/01/01"),
				DateChanged = DateTime.Parse("2020/01/01"),
			};
			var orderPriceByTaxRateInputEmpty = new OrderPriceByTaxRateInput();
			var orderPriceByTaxRateInputByModel = new OrderPriceByTaxRateInput(model);

			// ・モデルから生成したOrderPriceByTaxRateInputと、引数無しで生成したOrderPriceByTaxRateInputの各プロパティ値が異なること
			foreach (var key in orderPriceByTaxRateInputByModel.DataSource.Keys)
			{
				orderPriceByTaxRateInputByModel.DataSource[key].Should().NotBe(orderPriceByTaxRateInputEmpty.DataSource[key], " 値チェック：" + key);
			}
		}

		/// <summary>
		/// Modelオブジェクト生成テスト
		/// ・生成したOrderPriceByTaxRateModelの各プロパティ値が、コンストラクタから生成したModelと異なっていること
		/// </summary>
		[DataTestMethod()]
		public void CreateModelTest()
		{
			var val = 1;
			var input = new OrderPriceByTaxRateInput
			{
				OrderId = (val++).ToString(),
				KeyTaxRate = (val++).ToString(),
				PriceSubtotalByRate = (val++).ToString(),
				PriceShippingByRate = (val++).ToString(),
				PricePaymentByRate = (val++).ToString(),
				PriceTotalByRate = (val++).ToString(),
				TaxPriceByRate = (val++).ToString(),
				PriceCorrectionByRate = (val++).ToString(),
				DateCreated = "2020/01/01",
				DateChanged = "2020/01/01",
			};

			var orderPriceByTaxRateModelEmpty = new OrderPriceByTaxRateModel();
			var orderPriceByTaxRateModelByInput = input.CreateModel();

			// ・生成したOrderPriceByTaxRateModelの各プロパティ値が、コンストラクタから生成したModelと異なっていること
			foreach (var key in orderPriceByTaxRateModelByInput.DataSource.Keys)
			{
				orderPriceByTaxRateModelByInput.DataSource[key].Should().NotBe(orderPriceByTaxRateModelEmpty.DataSource[key], " 値チェック：" + key);
			}
		}
	}
}
