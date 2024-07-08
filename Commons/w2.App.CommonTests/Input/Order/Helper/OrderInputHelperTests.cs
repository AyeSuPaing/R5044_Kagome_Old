using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using w2.App.Common.Input.Order;
using w2.App.Common.Input.Order.Helper;

namespace w2.App.CommonTests.Input.Order.Helper
{
	/// <summary>
	/// OrderInputHelperTestsのテスト
	/// </summary>
	[TestClass()]
	[Ignore]
	public class OrderInputHelperTests
	{
		/// <summary>
		/// OrderItemへのCanDeleteフラグセット：注文に紐づいた商品が存在しない場合
		/// ・例外がスローされないこと
		/// </summary>
		[DataTestMethod()]
		public void SetCanDeletePropertyToOrderItemInputTest_OrderItemListIsEmpty()
		{
			var items = new OrderItemInput[0];

			// ・例外がスローされないこと
			Action act = () => OrderInputHelper.SetCanDeletePropertyToOrderItemInput(items);
			Assert.ThrowsException<AssertFailedException>(() => Assert.ThrowsException<Exception>(act, "明細が1件もないとき"));
		}

		/// <summary>
		/// OrderItemへのCanDeleteフラグセット：商品明細が一つのみ存在する場合
		/// ・OrderItemのCanDeleteフラグにfalseがセットされていること
		/// </summary>
		[DataTestMethod()]
		public void SetCanDeletePropertyToOrderItemInputTest_OrderHasOnlyOneItem()
		{
			var items = new[] { new OrderItemInput(), };
			OrderInputHelper.SetCanDeletePropertyToOrderItemInput(items);

			// ・OrderItemのCanDeleteフラグにfalseがセットされていること
			items.All(i => i.CanDelete == false).Should().Be(true, "通常1件は削除できない");
		}

		/// <summary>
		/// OrderItemへのCanDeleteフラグセット：商品明細が二つ存在する場合
		/// ・OrderItemのCanDeleteフラグにtrueがセットされていること
		/// </summary>
		[DataTestMethod()]
		public void SetCanDeletePropertyToOrderItemInputTest_OrderHasTwoItems()
		{
			var items = new[]
			{
				new OrderItemInput(),
				new OrderItemInput(),
			};
			OrderInputHelper.SetCanDeletePropertyToOrderItemInput(items);

			// ・OrderItemのCanDeleteフラグにtrueがセットされていること
			items.All(i => i.CanDelete).Should().Be(true, "通常2件は削除できる");
		}

		/// <summary>
		/// OrderItemへのCanDeleteフラグセット：商品セットが一つ存在する場合
		/// ・OrderItemのCanDeleteフラグにfalseがセットされていること
		/// </summary>
		[DataTestMethod()]
		public void SetCanDeletePropertyToOrderItemInputTest_OrderHasOneProductSet()
		{
			var items = new[]
			{
				new OrderItemInput { ProductSetId = "001", ProductSetNo = "1", },
				new OrderItemInput { ProductSetId = "001", ProductSetNo = "1", },
			};
			OrderInputHelper.SetCanDeletePropertyToOrderItemInput(items);

			// ・OrderItemのCanDeleteフラグにfalseがセットされていること
			items.All(i => i.CanDelete == false).Should().Be(true, "セット1種類は削除できない");
		}

		/// <summary>
		/// OrderItemへのCanDeleteフラグセット：商品セットが二つ存在する場合
		/// ・OrderItemのCanDeleteフラグにtrueがセットされていること
		/// </summary>
		[DataTestMethod()]
		public void SetCanDeletePropertyToOrderItemInputTest_OrderHasTwoProductSet()
		{
			var items = new[]
			{
				new OrderItemInput { ProductSetId = "001", ProductSetNo = "1", },
				new OrderItemInput { ProductSetId = "001", ProductSetNo = "2", },
			};

			OrderInputHelper.SetCanDeletePropertyToOrderItemInput(items);

			// ・OrderItemのCanDeleteフラグにtrueがセットされていること
			items.All(i => i.CanDelete).Should().Be(true, "セット2種類は削除できる");
		}

		/// <summary>
		/// OrderItemへのCanDeleteフラグセット：商品が一つ、商品セットが一つずつ存在する場合
		/// ・OrderItemのCanDeleteフラグにtrueがセットされていること
		/// </summary>
		[DataTestMethod()]
		public void SetCanDeletePropertyToOrderItemInputTest_OrderHasOneItemAndOneProductSet()
		{
			var items = new[]
			{
				new OrderItemInput(),
				new OrderItemInput { ProductSetId = "001", ProductSetNo = "1", },
				new OrderItemInput { ProductSetId = "001", ProductSetNo = "1", },
			};

			OrderInputHelper.SetCanDeletePropertyToOrderItemInput(items);

			// ・OrderItemのCanDeleteフラグにtrueがセットされていること
			items.All(i => i.CanDelete).Should().Be(true, "通常とセット混在は削除できる");
		}
	}
}
