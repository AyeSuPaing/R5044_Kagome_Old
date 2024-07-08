using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using w2.App.Common;
using w2.App.Common.Order.Payment.GMO.Zcom;
using w2.App.Common.Order.Payment.GMO.Zcom.Cancel;

namespace w2.App.CommonTests.Order.Payment.GMO.Zcom.Cancel
{
	[TestClass()]
	public class ZcomCancelRequestAdapterTests
	{
		[TestMethod]
		public void ZcomCancelRequestAdapterTest()
		{
			// 期待結果
			var expectedResponse = new ZcomCancelResponse
			{
				Results = new[]
				{
					new ZcomCancelResponse.Result
					{
						result = "unit-test-success"
					}
				}
			};
			// 期待結果を返すモックAPIFacade
			var mockZcomApiFacade = new Mock<IZcomApiFacade>();
			mockZcomApiFacade
				.Setup(s => s.CancelPayment(It.IsAny<ZcomCancelRequest>()))
				.Returns(expectedResponse);
			// モックAPIFacadeを返すモックFactory
			var mockZcomApiFacadeFactory = new Mock<IZcomApiFacadeFactory>();
			mockZcomApiFacadeFactory
				.Setup(s => s.CreateFacade(It.IsAny<ZcomApiSetting>()))
				.Returns(mockZcomApiFacade.Object);
			// モックFactoryをExternakApiFacadeに注入
			ExternalApiFacade.Instance.ZcomApiFacadeFactory = mockZcomApiFacadeFactory.Object;

			var sut = new ZcomCancelRequestAdapter("test001");
			var result = sut.Execute();

			result.Results[0].result.Should().Be("unit-test-success", "Zcomキャンセルリクエスト");
		}
	}
}
