using System;
using Moq;
using w2.App.Common.DataCacheController;
using w2.Domain.RealShop;

namespace w2.App.CommonTests._Helper.DataCacheConfigurator
{
	/// <summary>
	/// リアル店舗情報キャッシュ設定クラス
	/// </summary>
	internal class RealShopDataCacheConfigurator : IDisposable
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal RealShopDataCacheConfigurator(RealShopModel[] data)
		{
			RefreshDataCache(data);
		}

		/// <summary>
		/// リアル店舗情報キャッシュリフレッシュ
		/// </summary>
		/// <param name="data">リアル店舗データ</param>
		private void RefreshDataCache(RealShopModel[] data)
		{
			var realShopServiceMock = new Mock<IRealShopService>();
			realShopServiceMock.Setup(s => s.GetAll()).Returns(data);
			Domain.DomainFacade.Instance.RealShopService = realShopServiceMock.Object;

			DataCacheControllerFacade.GetAdvCodeCacheController().RefreshCacheData();
		}

		/// <summary>
		/// Dispose(キャッシュデータを空でリフレッシュ)
		/// </summary>
		public void Dispose()
		{
			RefreshDataCache(new RealShopModel[0]);
		}
	}
}
