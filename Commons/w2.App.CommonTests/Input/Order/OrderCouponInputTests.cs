using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using w2.App.Common.Input.Order;
using w2.App.Common.Input.Order.Helper;
using w2.App.Common.Order.Payment.Rakuten.Authorize;
using w2.App.CommonTests._Helper;
using w2.Domain.Order;

namespace w2.App.CommonTests.Input.Order
{
	/// <summary>
	/// OrderCouponInputのテスト
	/// </summary>
	[TestClass()]
	[Ignore]
	public class OrderCouponInputTests : AppTestClassBase
	{
		/// <summary>
		/// OrderCouponInputコンストラクタ実行
		/// ・インスタンスを生成した際に例外がスローされないこと
		/// </summary>
		[DataTestMethod()]
		public void OrderCouponInputTest()
		{
			// ・インスタンスを生成した際に例外をスローしないこと
			Action act = () => new OrderCouponInput();
			Assert.ThrowsException<AssertFailedException>(() => Assert.ThrowsException<Exception>(act, "OrderCouponInputインスタンスの生成"));
		}

		/// <summary>
		/// OrderCouponModelからのOrderCouponInputインスタンス生成
		/// ・モデルから生成したOrderCouponInputと、引数無しで生成したOrderCouponInputの各プロパティ値が異なること
		/// </summary>
		[DataTestMethod()]
		public void OrderCouponInputTest_CreatedByModel()
		{
			var val = 1;
			var model = new OrderCouponModel
			{
				OrderId = (val++).ToString(),
				OrderCouponNo = val++,
				DeptId = (val++).ToString(),
				CouponId = (val++).ToString(),
				CouponNo = val++,
				CouponCode = (val++).ToString(),
				CouponName = (val++).ToString(),
				CouponDispName = (val++).ToString(),
				CouponType = (val++).ToString(),
				CouponDiscountPrice = val++,
				CouponDiscountRate = val++,
				DateCreated = DateTime.Parse("2020/01/01"),
				DateChanged = DateTime.Parse("2020/01/01"),
				LastChanged = (val++).ToString(),
			};

			var orderCouponInputEmpty = new OrderCouponInput();
			var orderCouponInputByModel = new OrderCouponInput(model);

			// ・モデルから生成したOrderCouponInputと、引数無しで生成したOrderCouponInputの各プロパティ値が異なること
			foreach (var key in orderCouponInputByModel.DataSource.Keys)
			{
				orderCouponInputByModel.DataSource[key].Should().NotBe(orderCouponInputEmpty.DataSource[key], " 値チェック：" + key);
			}
		}

		/// <summary>
		/// Modelオブジェクト生成
		/// ・生成したOrderCouponModelの各プロパティ値が、コンストラクタから生成したModelと異なっていること
		/// </summary>
		[DataTestMethod()]
		public void CreateModelTest()
		{
			var val = 1;
			var input = new OrderCouponInput()
			{
				OrderId = (val++).ToString(),
				OrderCouponNo = (val++).ToString(),
				DeptId = (val++).ToString(),
				CouponId = (val++).ToString(),
				CouponNo = (val++).ToString(),
				CouponCode = (val++).ToString(),
				CouponName = (val++).ToString(),
				CouponDispName = (val++).ToString(),
				CouponType = (val++).ToString(),
				CouponDiscountPrice = (val++).ToString(),
				CouponDiscountRate = (val++).ToString(),
				DateCreated = "2020/01/01",
				DateChanged = "2020/01/01",
				LastChanged = (val++).ToString(),
			};

			var orderCouponModelEmpty = new OrderCouponModel();
			var orderCouponModelByInput = input.CreateModel();

			// ・生成したOrderModelの各プロパティ値が、コンストラクタから生成したModelと異なっていること
			foreach (var key in orderCouponModelByInput.DataSource.Keys)
			{
				orderCouponModelByInput.DataSource[key].Should().NotBe(orderCouponModelEmpty.DataSource[key], " 値チェック：" + key);
			}
		}
	}
}
