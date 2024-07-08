using System;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using w2.App.Common;
using w2.App.Common.Input.Order;
using w2.App.CommonTests._Helper;
using w2.Domain.Order;

namespace w2.App.CommonTests.Input.Order
{
	/// <summary>
	/// OrderShippingInputのテスト
	/// </summary>
	[TestClass()]
	[Ignore]
	public class OrderShippingInputTests : AppTestClassBase
	{
		/// <summary>
		/// OrderShippingInputコンストラクタ実行テスト
		/// ・インスタンスを生成した際に例外がスローされないこと
		/// </summary>
		[DataTestMethod()]
		public void OrderShippingInputTest()
		{
			// ・インスタンスを生成した際に例外をスローしないこと
			Action act = () => new OrderShippingInput();
			Assert.ThrowsException<AssertFailedException>(() => Assert.ThrowsException<Exception>(act, "OrderShippingInputインスタンスの生成"));
		}

		/// <summary>
		/// OrderShippingModelからのOrderShippingInputインスタンス生成テスト
		/// ・モデルから生成したOrderShippingInputと、引数無しで生成したOrderShippingInputの各プロパティ値が異なること
		/// </summary>
		[DataTestMethod()]
		public void OrderShippingInputByModelTest()
		{
			var val = 1;
			const string ORDER_ID = "DEV001";
			var excludedPropertyReplication = new[] { "shipping_address_kbn" };
			var model = new OrderShippingModel
			{
				OrderId = ORDER_ID,
				OrderShippingNo = val++,
				ShippingName = (val++).ToString(),
				ShippingName1 = (val++).ToString(),
				ShippingName2 = (val++).ToString(),
				ShippingNameKana = (val++).ToString(),
				ShippingNameKana1 = (val++).ToString(),
				ShippingNameKana2 = (val++).ToString(),
				ShippingZip = val++ + "-" + val++,
				ShippingAddr1 = (val++).ToString(),
				ShippingAddr2 = (val++).ToString(),
				ShippingAddr3 = (val++).ToString(),
				ShippingAddr4 = (val++).ToString(),
				ShippingTel1 = val++ + "-" + val++ + "-" + val++,
				ShippingTel2 = (val++).ToString(),
				ShippingTel3 = (val++).ToString(),
				ShippingFax = (val++).ToString(),
				ShippingCompany = (val++).ToString(),
				ShippingDate = DateTime.Parse("2020/01/01"),
				ShippingTime = (val++).ToString(),
				ShippingCheckNo = (val++).ToString(),
				DelFlg = (val++).ToString(),
				DateCreated = DateTime.Parse("2020/01/01"),
				DateChanged = DateTime.Parse("2020/01/01"),
				SenderName = (val++).ToString(),
				SenderName1 = (val++).ToString(),
				SenderName2 = (val++).ToString(),
				SenderNameKana = (val++).ToString(),
				SenderNameKana1 = (val++).ToString(),
				SenderNameKana2 = (val++).ToString(),
				SenderZip = val++ + "-" + val++,
				SenderAddr1 = (val++).ToString(),
				SenderAddr2 = (val++).ToString(),
				SenderAddr3 = (val++).ToString(),
				SenderAddr4 = (val++).ToString(),
				SenderTel1 = val++ + "-" + val++ + "-" + val++,
				WrappingPaperType = (val++).ToString(),
				WrappingPaperName = (val++).ToString(),
				WrappingBagType = (val++).ToString(),
				ShippingCompanyName = (val++).ToString(),
				ShippingCompanyPostName = (val++).ToString(),
				SenderCompanyName = (val++).ToString(),
				SenderCompanyPostName = (val++).ToString(),
				AnotherShippingFlg = (val++).ToString(),
				ShippingMethod = (val++).ToString(),
				DeliveryCompanyId = (val++).ToString(),
				ShippingCountryIsoCode = (val++).ToString(),
				ShippingCountryName = (val++).ToString(),
				ShippingAddr5 = (val++).ToString(),
				SenderCountryIsoCode = (val++).ToString(),
				SenderCountryName = (val++).ToString(),
				SenderAddr5 = (val++).ToString(),
				ScheduledShippingDate = DateTime.Parse("2020/01/01"),
				ShippingReceivingStoreFlg = (val++).ToString(),
				ShippingReceivingStoreId = (val++).ToString(),
				ShippingExternalDelivertyStatus = (val++).ToString(),
				ShippingStatus = (val++).ToString(),
				ShippingStatusUpdateDate = DateTime.Parse("2020/01/01"),
				ShippingReceivingMailDate = DateTime.Parse("2020/01/01"),
				ShippingReceivingStoreType = (val++).ToString(),
				Items = new[]
				{
					new OrderItemModel
					{
						OrderId = ORDER_ID,
						DateCreated = DateTime.Parse("2020/01/01"),
						DateChanged = DateTime.Parse("2020/01/01"),
					}
				},
			};

			var orderShippingInputEmpty = new OrderShippingInput();
			var orderShippingInputByModel = new OrderShippingInput(model);

			// ・モデルから生成したOrderShippingInputと、引数無しで生成したOrderShippingInputの各プロパティ値が異なること
			foreach (var key in orderShippingInputByModel.DataSource.Keys)
			{
				if (excludedPropertyReplication.Contains(key)
					|| orderShippingInputByModel.DataSource[key] is string == false) continue;
				orderShippingInputByModel.DataSource[key].Should().NotBe(
					orderShippingInputEmpty.DataSource[key],
					" 値チェック：" + key);
			}

			// string以外のプロパティ値はオブジェクトが作られていることをチェックする
			orderShippingInputByModel.Items[0].OrderId.Should().Be(ORDER_ID, " 値チェック：" + Constants.TABLE_ORDERITEM);
		}

		/// <summary>
		/// Modelオブジェクト生成テスト
		/// ・生成したOrderShippingModelの各プロパティ値が、コンストラクタから生成したModelと異なっていること
		/// </summary>
		[DataTestMethod()]
		public void CreateModelTest()
		{
			var val = 2;
			const string ORDER_ID = "DEV001";
			var excludedPropertyReplication = new[] { Constants.FIELD_ORDERSHIPPING_EXTERNAL_SHIPPING_COOPERATION_ID };
			var input = new OrderShippingInput
			{
				OrderId = ORDER_ID,
				OrderShippingNo = (val++).ToString(),
				ShippingName = (val++).ToString(),
				ShippingName1 = (val++).ToString(),
				ShippingName2 = (val++).ToString(),
				ShippingNameKana = (val++).ToString(),
				ShippingNameKana1 = (val++).ToString(),
				ShippingNameKana2 = (val++).ToString(),
				ShippingZip = (val++).ToString(),
				ShippingZip1 = (val++).ToString(),
				ShippingZip2 = (val++).ToString(),
				ShippingAddr1 = (val++).ToString(),
				ShippingAddr2 = (val++).ToString(),
				ShippingAddr3 = (val++).ToString(),
				ShippingAddr4 = (val++).ToString(),
				ShippingTel1 = (val++).ToString(),
				ShippingTel1_1 = (val++).ToString(),
				ShippingTel1_2 = (val++).ToString(),
				ShippingTel1_3 = (val++).ToString(),
				ShippingTel2 = (val++).ToString(),
				ShippingTel3 = (val++).ToString(),
				ShippingFax = (val++).ToString(),
				ShippingCompany = (val++).ToString(),
				ShippingDate = "2020/01/01",
				ShippingTime = (val++).ToString(),
				ShippingCheckNo = (val++).ToString(),
				DelFlg = (val++).ToString(),
				DateCreated = "2020/01/01",
				DateChanged = "2020/01/01",
				SenderName = (val++).ToString(),
				SenderName1 = (val++).ToString(),
				SenderName2 = (val++).ToString(),
				SenderNameKana = (val++).ToString(),
				SenderNameKana1 = (val++).ToString(),
				SenderNameKana2 = (val++).ToString(),
				SenderZip = (val++).ToString(),
				SenderZip1 = (val++).ToString(),
				SenderZip2 = (val++).ToString(),
				SenderAddr1 = (val++).ToString(),
				SenderAddr2 = (val++).ToString(),
				SenderAddr3 = (val++).ToString(),
				SenderAddr4 = (val++).ToString(),
				SenderTel1 = (val++).ToString(),
				SenderTel1_1 = (val++).ToString(),
				SenderTel1_2 = (val++).ToString(),
				SenderTel1_3 = (val++).ToString(),
				WrappingPaperType = (val++).ToString(),
				WrappingPaperName = (val++).ToString(),
				WrappingBagType = (val++).ToString(),
				ShippingCompanyName = (val++).ToString(),
				ShippingCompanyPostName = (val++).ToString(),
				SenderCompanyName = (val++).ToString(),
				SenderCompanyPostName = (val++).ToString(),
				AnotherShippingFlg = (val++).ToString(),
				ShippingMethod = (val++).ToString(),
				DeliveryCompanyId = (val++).ToString(),
				ShippingCountryIsoCode = (val++).ToString(),
				ShippingCountryName = (val++).ToString(),
				ShippingAddr5 = (val++).ToString(),
				SenderCountryIsoCode = (val++).ToString(),
				SenderCountryName = (val++).ToString(),
				SenderAddr5 = (val++).ToString(),
				ScheduledShippingDate = "2020/01/01",
				ShippingReceivingStoreFlg = (val++).ToString(),
				ShippingReceivingStoreId = (val++).ToString(),
				ShippingExternalDelivertyStatus = (val++).ToString(),
				ShippingStatus = (val++).ToString(),
				ShippingStatusUpdateDate = "2020/01/01",
				ShippingReceivingMailDate = "2020/01/01",
				ShippingReceivingStoreType = (val++).ToString(),
				Items = new []
				{
					new OrderItemInput
					{
						OrderId = ORDER_ID,
					}
				},
			};

			var orderShippingModelEmpty = new OrderShippingModel();
			var orderShippingModelByInput = input.CreateModel();

			// ・生成したOrderShippingModelの各プロパティ値が、コンストラクタから生成したModelと異なっていること
			foreach (var key in orderShippingModelByInput.DataSource.Keys)
			{
				if (excludedPropertyReplication.Contains(key)
					|| ((string)key == Constants.TABLE_ORDERITEM)) continue;
				orderShippingModelByInput.DataSource[key].Should().NotBe(orderShippingModelEmpty.DataSource[key], " 値チェック：" + key);
			}
			// string以外のプロパティ値はオブジェクトが作られていることをチェックする
			orderShippingModelByInput.Items[0].OrderId.Should().Be(ORDER_ID, " 値チェック：" + Constants.TABLE_ORDERITEM);
		}
	}
}
