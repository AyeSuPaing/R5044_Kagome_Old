using System;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using w2.App.Common.Input.Order;
using w2.App.CommonTests._Helper;
using w2.Domain.Order;

namespace w2.App.CommonTests.Input.Order
{
	/// <summary>
	/// OrderItemInputのテスト
	/// </summary>
	[TestClass()]
	[Ignore]
	public class OrderItemInputTests : AppTestClassBase
	{
		/// <summary>
		/// OrderItemInputコンストラクタ実行テスト
		/// ・インスタンスを生成した際に例外がスローされないこと
		/// </summary>
		[DataTestMethod()]
		public void OrderItemInputTest()
		{
			// ・インスタンスを生成した際に例外がスローされないこと
			Action act = () => new OrderItemInput();
			Assert.ThrowsException<AssertFailedException>(() => Assert.ThrowsException<Exception>(act, "OrderItemInputインスタンスの生成"));
		}

		/// <summary>
		/// OrderItemModelからのOrderItemInputインスタンス生成テスト
		/// ・モデルから生成したOrderItemInputと、引数無しで生成したOrderItemInputの各プロパティ値が異なること
		/// </summary>
		[DataTestMethod()]
		public void OrderItemInputTest_CreatedByModel()
		{
			var val = 1;
			var excludedPropertyReplication = new [] { "DeleteTarget", "DeleteTargetSet", "ItemIndex" };
			var model = new OrderItemModel
			{
				OrderId = (val++).ToString(),
				OrderItemNo = val++,
				OrderShippingNo = val++,
				ShopId = (val++).ToString(),
				ProductId = (val++).ToString(),
				VariationId = (val++).ToString(),
				SupplierId = (val++).ToString(),
				ProductName = (val++).ToString(),
				ProductNameKana = (val++).ToString(),
				ProductPrice = val++,
				ProductPriceOrg = val++,
				ProductPoint = val++,
				ProductTaxIncludedFlg = (val++).ToString(),
				ProductTaxRate = val++,
				ProductTaxRoundType = (val++).ToString(),
				ProductPricePretax = val++,
				ProductPriceShip = val++,
				ProductPriceCost = val++,
				ProductPointKbn = (val++).ToString(),
				ItemRealstockReserved = val++,
				ItemRealstockShipped = val++,
				ItemQuantity = val++,
				ItemQuantitySingle = val++,
				ItemPrice = val++,
				ItemPriceSingle = val++,
				ItemPriceTax = val++,
				ProductSetId = (val++).ToString(),
				ProductSetNo = val++,
				ProductSetCount = val++,
				ItemReturnExchangeKbn = (val++).ToString(),
				ItemMemo = (val++).ToString(),
				ItemPoint = val++,
				ItemCancelFlg = (val++).ToString(),
				ItemCancelDate = DateTime.Parse("2020/01/01"),
				ItemReturnFlg = (val++).ToString(),
				ItemReturnDate = DateTime.Parse("2020/01/01"),
				DelFlg = (val++).ToString(),
				ProductOptionTexts = (val++).ToString(),
				BrandId = (val++).ToString(),
				DownloadUrl = (val++).ToString(),
				ProductsaleId = (val++).ToString(),
				CooperationId1 = (val++).ToString(),
				CooperationId2 = (val++).ToString(),
				CooperationId3 = (val++).ToString(),
				CooperationId4 = (val++).ToString(),
				CooperationId5 = (val++).ToString(),
				OrderSetpromotionNo = val++,
				OrderSetpromotionItemNo = val++,
				StockReturnedFlg = (val++).ToString(),
				NoveltyId = (val++).ToString(),
				RecommendId = (val++).ToString(),
				FixedPurchaseProductFlg = (val++).ToString(),
				ProductBundleId = (val++).ToString(),
				BundleItemDisplayType = (val++).ToString(),
				OrderHistoryDisplayType = (val++).ToString(),
				LimitedPaymentIds = (val++).ToString(),
				PluralShippingPriceFreeFlg = (val++).ToString(),
				ShippingSizeKbn = (val++).ToString(),
				ColumnForMallOrder = (val++).ToString(),
				CooperationId6 = (val++).ToString(),
				CooperationId7 = (val++).ToString(),
				CooperationId8 = (val++).ToString(),
				CooperationId9 = (val++).ToString(),
				CooperationId10 = (val++).ToString(),
				GiftWrappingId = (val++).ToString(),
				GiftMessage = (val++).ToString(),
				FixedPurchaseDiscountType = (val++).ToString(),
				FixedPurchaseDiscountValue = (val++),
				FixedPurchaseItemOrderCount = (val++),
				FixedPurchaseItemShippedCount = (val++),
				ItemDiscountedPrice = (val++),
				DateCreated = DateTime.Parse("2020/01/01"),
				DateChanged = DateTime.Parse("2020/01/01"),
			};
			var orderItemInputEmpty = new OrderItemInput();
			var orderItemInputByModel = new OrderItemInput(model, 0);

			// ・モデルから生成したOrderItemInputと、引数無しで生成したOrderItemInputの各プロパティ値が異なること
			foreach (var key in orderItemInputByModel.DataSource.Keys)
			{
				if (excludedPropertyReplication.Contains(key)) continue;
				orderItemInputByModel.DataSource[key].Should().NotBe(orderItemInputEmpty.DataSource[key], " 値チェック：" + key);
			}
		}

		/// <summary>
		/// Modelオブジェクト生成テスト
		/// ・生成したOrderItemModelの各プロパティ値が、コンストラクタから生成したModelと異なっていること
		/// </summary>
		[DataTestMethod()]
		public void CreateModelTest()
		{
			var val = 1;
			var input = new OrderItemInput()
			{
				OrderId = (val++).ToString(),
				OrderItemNo = (val++).ToString(),
				OrderShippingNo = (val++).ToString(),
				ShopId = (val++).ToString(),
				ProductId = (val++).ToString(),
				VariationId = (val++).ToString(),
				VId = (val++).ToString(),
				ProductName = (val++).ToString(),
				ProductNameKana = (val++).ToString(),
				ProductPrice = (val++).ToString(),
				ProductPriceOrg = (val++).ToString(),
				ProductPoint = (val++).ToString(),
				ProductTaxIncludedFlg = (val++).ToString(),
				ProductTaxRate = (val++).ToString(),
				ProductTaxRoundType = (val++).ToString(),
				ProductPricePretax = (val++).ToString(),
				ProductPriceShip = (val++).ToString(),
				ProductPriceCost = (val++).ToString(),
				ProductPointKbn = (val++).ToString(),
				ItemRealstockReserved = (val++).ToString(),
				ItemRealstockShipped = (val++).ToString(),
				ItemQuantity = (val++).ToString(),
				ItemQuantitySingle = (val++).ToString(),
				ItemPrice = (val++).ToString(),
				ItemPriceSingle = (val++).ToString(),
				ItemPriceTax = (val++).ToString(),
				ProductSetId = (val++).ToString(),
				ProductSetNo = (val++).ToString(),
				ProductSetCount = (val++).ToString(),
				ItemReturnExchangeKbn = (val++).ToString(),
				ItemMemo = (val++).ToString(),
				ItemPoint = (val++).ToString(),
				ItemCancelFlg = (val++).ToString(),
				ItemCancelDate = "2020/01/01",
				ItemReturnFlg = (val++).ToString(),
				ItemReturnDate = "2020/01/01",
				DelFlg = (val++).ToString(),
				DateCreated = "2020/01/01",
				DateChanged = "2020/01/01",
				ProductOptionTexts = (val++).ToString(),
				BrandId = (val++).ToString(),
				DownloadUrl = (val++).ToString(),
				ProductsaleId = (val++).ToString(),
				CooperationId1 = (val++).ToString(),
				CooperationId2 = (val++).ToString(),
				CooperationId3 = (val++).ToString(),
				CooperationId4 = (val++).ToString(),
				CooperationId5 = (val++).ToString(),
				CooperationId6 = (val++).ToString(),
				CooperationId7 = (val++).ToString(),
				CooperationId8 = (val++).ToString(),
				CooperationId9 = (val++).ToString(),
				CooperationId10 = (val++).ToString(),
				OrderSetpromotionNo = (val++).ToString(),
				OrderSetpromotionItemNo = (val++).ToString(),
				StockReturnedFlg = (val++).ToString(),
				NoveltyId = (val++).ToString(),
				RecommendId = (val++).ToString(),
				FixedPurchaseProductFlg = (val++).ToString(),
				ProductBundleId = (val++).ToString(),
				BundleItemDisplayType = (val++).ToString(),
				ItemIndex = (val++).ToString(),
				ShippingSizeKbn = (val++).ToString(),
				ColumnForMallOrder = (val++).ToString(),
				GiftWrappingId = (val++).ToString(),
				GiftMessage = (val++).ToString(),
				SupplierId = (val++).ToString(),
				FixedPurchaseDiscountType = (val++).ToString(),
				FixedPurchaseDiscountValue = (val++).ToString(),
			};

			var orderCouponModelEmpty = new OrderCouponModel();
			var orderCouponModelByInput = input.CreateModel();

			// ・生成したOrderModelの各プロパティ値が、コンストラクタから生成したModelと異なっていること
			foreach (var key in orderCouponModelByInput.DataSource.Keys)
			{
				orderCouponModelByInput.DataSource[key].Should().NotBe(
					orderCouponModelEmpty.DataSource[key],
					" 値チェック：" + key);
			}
		}
	}
}
