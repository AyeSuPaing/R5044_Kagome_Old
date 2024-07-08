using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using w2.App.Common.Option;
using w2.App.CommonTests._Helper;
using w2.CommonTests._Helper;
using w2.Domain;
using w2.Domain.ProductTag;
using Constants = w2.App.Common.Constants;

namespace w2.App.CommonTests.Order.Cart
{
	/// <summary>
	/// CartProductのテスト
	/// </summary>
	[TestClass()]
	[Ignore]
	public class CartProductTests : AppTestClassBase
	{
		/// <summary>
		/// 商品小計再計算テスト：セット商品ではない
		/// ・CartProductに設定された商品価格、個数から「商品小計」「商品税込み小計」「商品小計税額」「個数」が設定されること。
		/// ・PriceSubtotalSingle = CartProduct.ProductPrice * CartProduct.ItemQuantity
		/// ・PriceSubtotalSingleTax = 「消費税小数点以下切捨て(CartProduct.ProductPrice * CartProduct.TaxRate / 100)」 * CartProduct.ItemQuantity
		/// ・PriceSubtotalSinglePretax = PriceSubtotalSingle + PriceSubtotalSingleTax
		/// ・CountSingle = CartProduct.ItemQuantity
		/// ・PriceSubtotal = PriceSubtotalSingle
		/// ・PriceSubtotalTax = PriceSubtotalSingleTax
		/// ・PriceSubtotalPretax = PriceSubtotalSinglePretax
		/// ・Count = CountSingle
		/// </summary>
		[DataTestMethod()]
		public void CalculateTest_IsNotSetProduct()
		{
			using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), false))
			using (new TestConfigurator(Member.Of(() => Constants.TAX_EXCLUDED_FRACTION_ROUNDING),
				TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN))
			{
				// モックによるドメイン層偽装
				var mock = new Mock<IProductTagService>();
				mock.Setup(s => s.GetProductTagSetting()).Returns(new ProductTagSettingModel[0]);
				DomainFacade.Instance.ProductTagService = mock.Object;

				var cartProduct = CartTestHelper.CreateCartProduct(
					productPrice: 1001,
					productCount: 2,
					taxRate: 10);

				cartProduct.Calculate();

				// 商品小計再計算テスト：セット商品ではない
				// ・CartProductに設定された商品価格、個数から「商品小計」「商品税込み小計」「商品小計税額」「個数」が設定されること。
				cartProduct.PriceSubtotalSingle.Should().Be(2002, "カート商品小計計算：商品小計（セット未考慮）");
				cartProduct.PriceSubtotalSinglePretax.Should().Be(2202, "カート商品小計計算：税込商品小計価格（セット未考慮）");
				cartProduct.PriceSubtotalSingleTax.Should().Be(200, "カート商品小計計算：商品小計税額（セット未考慮）");
				cartProduct.CountSingle.Should().Be(2, "カート商品小計計算：数量（セット未考慮）");
				cartProduct.PriceSubtotal.Should().Be(2002, "カート商品小計計算：商品小計（セット考慮済み）");
				cartProduct.PriceSubtotalPretax.Should().Be(2202, "カート商品小計計算：税込商品小計価格（セット考慮済み）");
				cartProduct.PriceSubtotalTax.Should().Be(200, "カート商品小計計算：商品小計税額（セット考慮済み）");
				cartProduct.Count.Should().Be(2, "カート商品小計計算：数量（セット考慮済み）");
			}
		}

		/// <summary>
		/// 商品小計再計算テスト：セット商品
		/// ・CartProductに設定された個数と、CartProductSetに設定されたセット商品価格から「商品小計」「商品税込み小計」「商品小計税額」「個数」が設定されること。
		/// ・「セット考慮済み」プロパティの値は、「セット未考慮 * CartProductSet.ProductSetCount」となること
		/// ・PriceSubtotalSingle = CartProduct.ProductPrice * CartProduct.ItemQuantity
		/// ・PriceSubtotalSingleTax = 「消費税小数点以下切捨て(CartProduct.ProductPrice * CartProduct.TaxRate / 100)」 * CartProduct.ItemQuantity
		/// ・PriceSubtotalSinglePretax = PriceSubtotalSingle + PriceSubtotalSingleTax
		/// ・CountSingle = CartProduct.ItemQuantity
		/// ・PriceSubtotal = PriceSubtotalSingle * CartProductSet.ProductSetCount
		/// ・PriceSubtotalTax = PriceSubtotalSingleTax * CartProductSet.ProductSetCount
		/// ・PriceSubtotalPretax = PriceSubtotalSinglePretax * CartProductSet.ProductSetCount
		/// ・Count = CountSingle * CartProductSet.ProductSetCount
		/// </summary>
		[DataTestMethod()]
		public void CalculateTest_IsSetProduct()
		{
			using (new TestConfigurator(Member.Of(() => Constants.MANAGEMENT_INCLUDED_TAX_FLAG), false))
			using (new TestConfigurator(Member.Of(() => Constants.TAX_EXCLUDED_FRACTION_ROUNDING),
				TaxCalculationUtility.FLG_DISPLAY_TAX_TYPE_FRACTION_ROUNDING_ROUND_DOWN))
			{
				// モックによるドメイン層偽装
				var mock = new Mock<IProductTagService>();
				mock.Setup(s => s.GetProductTagSetting()).Returns(new ProductTagSettingModel[0]);
				DomainFacade.Instance.ProductTagService = mock.Object;

				var cartProductSet = CartTestHelper.CreateCartProductSet(
					maxSellQuantity: 10,
					productSetCount: 2,
					productSetNo: 1);
				CartTestHelper.CreateAndAddProductToProductSet(
					cartProductSet,
					setItemPrice: 1001,
					productCount: 2);

				var cartProduct = cartProductSet.Items[0];

				cartProduct.Calculate();

				// 商品小計再計算テスト：セット商品
				// ・CartProductに設定された個数と、CartProductSetに設定されたセット商品価格から「商品小計」「商品税込み小計」「商品小計税額」「個数」が設定されること。
				// ・「セット考慮済み」プロパティの値は、「セット未考慮 * CartProductSet.ProductSetCount」となること
				cartProduct.PriceSubtotalSingle.Should().Be(2002, "カート商品小計計算：商品小計（セット未考慮）");
				cartProduct.PriceSubtotalSinglePretax.Should().Be(2202, "カート商品小計計算：税込商品小計価格（セット未考慮）");
				cartProduct.PriceSubtotalSingleTax.Should().Be(200, "カート商品小計計算：商品小計税額（セット未考慮）");
				cartProduct.CountSingle.Should().Be(2, "カート商品小計計算：数量（セット未考慮）");
				cartProduct.PriceSubtotal.Should().Be(4004, "カート商品小計計算：商品小計（セット考慮済み）");
				cartProduct.PriceSubtotalPretax.Should().Be(4404, "カート商品小計計算：税込商品小計価格（セット考慮済み）");
				cartProduct.PriceSubtotalTax.Should().Be(400, "カート商品小計計算：商品小計税額（セット考慮済み）");
				cartProduct.Count.Should().Be(4, "カート商品小計計算：数量（セット考慮済み）");
			}
		}
	}
}
