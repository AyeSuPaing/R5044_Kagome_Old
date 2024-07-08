using System;
using Moq;
using w2.App.Common.DataCacheController;
using w2.Domain.ShopShipping;

namespace w2.App.CommonTests._Helper.DataCacheConfigurator
{
	/// <summary>
	/// 店舗配送種別データキャッシュ設定クラス
	/// </summary>
	internal class ShopShippingDataCacheConfigurator : IDisposable
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="data">店舗配送種別データ</param>
		internal ShopShippingDataCacheConfigurator(ShopShippingModel[] data)
		{
			RefreshDataCache(data);
		}

		/// <summary>
		/// 店舗配送種別データキャッシュリフレッシュ
		/// </summary>
		/// <param name="data">店舗配送種別データ</param>
		private void RefreshDataCache(ShopShippingModel[] data)
		{
			var shopShippingServiceMock = new Mock<IShopShippingService>();
			shopShippingServiceMock.Setup(s => s.GetAll(It.IsAny<string>())).Returns(data);
			Domain.DomainFacade.Instance.ShopShippingService = shopShippingServiceMock.Object;

			DataCacheControllerFacade.GetShopShippingCacheController().RefreshCacheData();
		}

		/// <summary>
		/// Dispose(キャッシュデータを空でリフレッシュ)
		/// </summary>
		public void Dispose()
		{
			RefreshDataCache(new ShopShippingModel[0]);
		}
	}
}