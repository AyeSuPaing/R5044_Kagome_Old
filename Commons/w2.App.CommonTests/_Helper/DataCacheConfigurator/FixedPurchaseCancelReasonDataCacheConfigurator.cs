using System;
using Moq;
using w2.App.Common.DataCacheController;
using w2.Domain.FixedPurchase;

namespace w2.App.CommonTests._Helper.DataCacheConfigurator
{
	/// <summary>
	/// 定期解約理由区分設定情報キャッシュ設定クラス
	/// </summary>
	internal class FixedPurchaseCancelReasonDataCacheConfigurator : IDisposable
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal FixedPurchaseCancelReasonDataCacheConfigurator(FixedPurchaseCancelReasonModel[] data)
		{
			RefreshDataCache(data);
		}

		/// <summary>
		/// 定期解約理由区分設定情報キャッシュリフレッシュ
		/// </summary>
		/// <param name="data">定期解約理由区分設定情報</param>
		private void RefreshDataCache(FixedPurchaseCancelReasonModel[] data)
		{
			var fixedPurchaseServiceMock = new Mock<IFixedPurchaseService>();
			fixedPurchaseServiceMock.Setup(s => s.GetCancelReasonAll()).Returns(data);
			Domain.DomainFacade.Instance.FixedPurchaseService = fixedPurchaseServiceMock.Object;

			DataCacheControllerFacade.GetFixedPurchaseCancelReasonCacheController().RefreshCacheData();
		}

		/// <summary>
		/// Dispose(キャッシュデータを空でリフレッシュ)
		/// </summary>
		public void Dispose()
		{
			RefreshDataCache(new FixedPurchaseCancelReasonModel[0]);
		}
	}
}