using System;
using Moq;
using w2.App.Common.DataCacheController;
using w2.Domain.ProductListDispSetting;

namespace w2.App.CommonTests._Helper.DataCacheConfigurator
{
	/// <summary>
	/// 商品一覧設定データキャッシュ設定クラス
	/// </summary>
	internal class ProductListDispSettingDataCacheConfigurator : IDisposable
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal ProductListDispSettingDataCacheConfigurator(ProductListDispSettingModel[] data)
		{
			RefreshDataCache(data);
		}

		/// <summary>
		/// 商品一覧設定データキャッシュリフレッシュ
		/// </summary>
		/// <param name="data">商品一覧設定データ</param>
		private void RefreshDataCache(ProductListDispSettingModel[] data)
		{
			var productListDispSettingServiceMock = new Mock<IProductListDispSettingService>();
			productListDispSettingServiceMock.Setup(s => s.GetUsable()).Returns(data);
			Domain.DomainFacade.Instance.ProductListDispSettingService = productListDispSettingServiceMock.Object;

			DataCacheControllerFacade.GetProductListDispSettingCacheController().RefreshCacheData();
		}

		/// <summary>
		/// Dispose(キャッシュデータを空でリフレッシュ)
		/// </summary>
		public void Dispose()
		{
			RefreshDataCache(new ProductListDispSettingModel[0]);
		}
	}
}