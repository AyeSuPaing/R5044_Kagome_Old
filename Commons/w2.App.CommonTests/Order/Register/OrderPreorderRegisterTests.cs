using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Moq;
using w2.App.CommonTests._Helper;
using w2.App.CommonTests.Order.Cart;
using w2.Common.Sql;
using w2.Domain;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.UserShipping;
using w2.App.Common.Input.Order;
using w2.Domain.OrderWorkflowSetting;

namespace w2.App.Common.Order.Register.Tests
{
	[TestClass()]
	[Ignore]
	public class OrderPreorderRegisterTests : AppTestClassBase
	{
		/// <summary>
		/// ユーザー配送先登録テスト
		/// </summary>
		[DataTestMethod()]
		public void InsertUserShippingTest_ForService()
		{
			// カート作成
			var cart = CartTestHelper.CreateCart();
			cart.Owner = CartTestHelper.CreateCartOwner();
			var cartShipping = new CartShipping(cart);
			cartShipping.UpdateShippingAddr(cart.Owner, blIsSameShippingAsCart1: true);

			var mock = new Mock<IUserShippingService>();
			mock.Setup(
				s => s.Insert(
					It.IsAny<UserShippingModel>(),
					It.IsAny<string>(),
					It.IsAny<UpdateHistoryAction>(),
					It.IsAny<SqlAccessor>())).Returns(1);
			DomainFacade.Instance.UserShippingService = mock.Object;

			var orderPreorderRegister = new OrderPreorderRegister();

			// テスト
			((int)new PrivateType(typeof(OrderPreorderRegister)).InvokeStatic("InsertUserShipping", new object[] { "U001", cartShipping, (SqlAccessor)null })).Should().Be(1);
		}
	}
}
