using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using w2.App.Common.Input.Order;
using w2.App.CommonTests._Helper;
using w2.Domain.Order;

namespace w2.App.CommonTests.Input.Order
{
	/// <summary>
	/// OrderOwnerInputのテスト
	/// </summary>
	[TestClass()]
	[Ignore]
	public class OrderOwnerInputTests : AppTestClassBase
	{
		/// <summary>
		/// OrderOwnerInputコンストラクタ実行テスト
		/// ・インスタンスを生成した際に例外がスローされないこと
		/// </summary>
		[DataTestMethod()]
		public void OrderOwnerInputTest()
		{
			// ・インスタンスを生成した際に例外をスローしないこと
			Action act = () => new OrderOwnerInput();
			Assert.ThrowsException<AssertFailedException>(() => Assert.ThrowsException<Exception>(act, "OrderOwnerInputインスタンスの生成"));
		}

		/// <summary>
		/// OrderOwnerModelからのOrderOwnerInputインスタンス生成テスト
		/// ・モデルから生成したOrderOwnerInputと、引数無しで生成したOrderOwnerInputの各プロパティ値が異なること
		/// </summary>
		[DataTestMethod()]
		public void OrderOwnerInputByModelTest()
		{
			var val = 1;
			var model = new OrderOwnerModel
			{
				OrderId = (val++).ToString(),
				OwnerKbn = (val++).ToString(),
				OwnerName = (val++).ToString(),
				OwnerName1 = (val++).ToString(),
				OwnerName2 = (val++).ToString(),
				OwnerNameKana = (val++).ToString(),
				OwnerNameKana1 = (val++).ToString(),
				OwnerNameKana2 = (val++).ToString(),
				OwnerZip = (val++) + "-" + (val++) + "-" + (val++),
				OwnerAddr1 = (val++).ToString(),
				OwnerAddr2 = (val++).ToString(),
				OwnerAddr3 = (val++).ToString(),
				OwnerAddr4 = (val++).ToString(),
				OwnerTel1 = (val++) + "-" + (val++) + "-" + (val++),
				OwnerTel2 = (val++).ToString(),
				OwnerTel3 = (val++).ToString(),
				OwnerSex = (val++).ToString(),
				OwnerBirth = DateTime.Parse("2020/01/01"),
				OwnerCompanyName = (val++).ToString(),
				OwnerCompanyPostName = (val++).ToString(),
				OwnerCompanyExectiveName = (val++).ToString(),
				DelFlg = (val++).ToString(),
				AccessCountryIsoCode = (val++).ToString(),
				DispLanguageCode = (val++).ToString(),
				DispLanguageLocaleId = (val++).ToString(),
				DispCurrencyCode = (val++).ToString(),
				DispCurrencyLocaleId = (val++).ToString(),
				OwnerAddrCountryIsoCode = (val++).ToString(),
				OwnerAddrCountryName = (val++).ToString(),
				OwnerAddr5 = (val++).ToString(),
				OwnerMailAddr = (val++).ToString(),
				OwnerMailAddr2 = (val++).ToString(),
				OwnerFax = (val++).ToString(),
				DateCreated = DateTime.Parse("2020/01/01"),
				DateChanged = DateTime.Parse("2020/01/01"),
			};
			var orderOwnerInputEmpty = new OrderOwnerInput();
			var orderOwnerInputByModel = new OrderOwnerInput(model);

			// ・モデルから生成したOrderOwnerInputと、引数無しで生成したOrderOwnerInputの各プロパティ値が異なること
			foreach (var key in orderOwnerInputByModel.DataSource.Keys)
			{
				orderOwnerInputByModel.DataSource[key].Should().NotBe(orderOwnerInputEmpty.DataSource[key], " 値チェック：" + key);
			}
		}

		/// <summary>
		/// Modelオブジェクト生成テスト
		/// ・生成したOrderOwnerModelの各プロパティ値が、コンストラクタから生成したModelと異なっていること
		/// </summary>
		[DataTestMethod()]
		public void CreateModelTest()
		{
			var val = 1;
			var input = new OrderOwnerInput
			{
				OrderId = (val++).ToString(),
				OwnerKbn = (val++).ToString(),
				OwnerName = (val++).ToString(),
				OwnerName1 = (val++).ToString(),
				OwnerName2 = (val++).ToString(),
				OwnerNameKana = (val++).ToString(),
				OwnerNameKana1 = (val++).ToString(),
				OwnerNameKana2 = (val++).ToString(),
				OwnerZip = (val++).ToString(),
				OwnerZip1 = (val++).ToString(),
				OwnerZip2 = (val++).ToString(),
				OwnerAddr1 = (val++).ToString(),
				OwnerAddr2 = (val++).ToString(),
				OwnerAddr3 = (val++).ToString(),
				OwnerAddr4 = (val++).ToString(),
				OwnerTel1 = (val++).ToString(),
				OwnerTel1_1 = (val++).ToString(),
				OwnerTel1_2 = (val++).ToString(),
				OwnerTel1_3 = (val++).ToString(),
				OwnerTel2 = (val++).ToString(),
				OwnerTel3 = (val++).ToString(),
				OwnerSex = (val++).ToString(),
				OwnerBirth = "2020/01/01",
				OwnerCompanyName = (val++).ToString(),
				OwnerCompanyPostName = (val++).ToString(),
				OwnerCompanyExectiveName = (val++).ToString(),
				DelFlg = (val++).ToString(),
				AccessCountryIsoCode = (val++).ToString(),
				DispLanguageCode = (val++).ToString(),
				DispLanguageLocaleId = (val++).ToString(),
				DispCurrencyCode = (val++).ToString(),
				DispCurrencyLocaleId = (val++).ToString(),
				OwnerAddrCountryIsoCode = (val++).ToString(),
				OwnerAddrCountryName = (val++).ToString(),
				OwnerAddr5 = (val++).ToString(),
				OwnerMailAddr = (val++).ToString(),
				OwnerMailAddr2 = (val++).ToString(),
				OwnerFax = (val++).ToString(),
				DateCreated = "2020/01/01",
				DateChanged = "2020/01/01",
			};

			var orderOwnerModelEmpty = new OrderOwnerModel();
			var orderOwnerModelByInput = input.CreateModel();

			// ・生成したOrderOwnerModelの各プロパティ値が、コンストラクタから生成したModelと異なっていること
			foreach (var key in orderOwnerModelByInput.DataSource.Keys)
			{
				orderOwnerModelByInput.DataSource[key].Should().NotBe(orderOwnerModelEmpty.DataSource[key], " 値チェック：" + key);
			}
		}

	}
}
