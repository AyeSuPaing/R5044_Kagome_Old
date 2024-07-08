using System;
using Moq;
using w2.App.Common.DataCacheController;
using w2.Domain.Payment;

namespace w2.App.CommonTests._Helper.DataCacheConfigurator
{
	/// <summary>
	/// 決済種別データキャッシュ設定クラス
	/// </summary>
	internal class PaymentDataCacheConfigurator : IDisposable
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal PaymentDataCacheConfigurator(PaymentModel[] data)
		{
			RefreshDataCache(data);
		}

		/// <summary>
		/// 決済種別データキャッシュリフレッシュ
		/// </summary>
		/// <param name="data">決済種別データ</param>
		private void RefreshDataCache(PaymentModel[] data)
		{
			var paymentServiceMock = new Mock<IPaymentService>();
			paymentServiceMock.Setup(s => s.GetAllWithPrice(It.IsAny<string>())).Returns(data);
			Domain.DomainFacade.Instance.PaymentService = paymentServiceMock.Object;

			DataCacheControllerFacade.GetPaymentCacheController().RefreshCacheData();
		}

		/// <summary>
		/// Dispose(キャッシュデータを空でリフレッシュ)
		/// </summary>
		public void Dispose()
		{
			RefreshDataCache(new PaymentModel[0]);
		}
	}
}