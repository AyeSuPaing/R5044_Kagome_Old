using System;
using Moq;
using w2.App.Common.DataCacheController;
using w2.Domain.FeatureArea;

namespace w2.App.CommonTests._Helper.DataCacheConfigurator
{
	/// <summary>
	/// 特集エリアバナーデータキャッシュ設定クラス
	/// </summary>
	internal class FeatureAreaBannerDataCacheConfigurator : IDisposable
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal FeatureAreaBannerDataCacheConfigurator(FeatureAreaModel[] data)
		{
			RefreshDataCache(data);
		}

		/// <summary>
		/// 特集エリアバナークデータキャッシュリフレッシュ
		/// </summary>
		/// <param name="data">特集エリアバナーデータ</param>
		private void RefreshDataCache(FeatureAreaModel[] data)
		{
			var featureAreaServiceMock = new Mock<IFeatureAreaService>();
			featureAreaServiceMock.Setup(s => s.GetFeatureAreaAll()).Returns(data);
			Domain.DomainFacade.Instance.FeatureAreaService = featureAreaServiceMock.Object;

			DataCacheControllerFacade.GetFeatureAreaBannerCacheController().RefreshCacheData();
		}

		/// <summary>
		/// Dispose(キャッシュデータを空でリフレッシュ)
		/// </summary>
		public void Dispose()
		{
			RefreshDataCache(new FeatureAreaModel[0]);
		}
	}
}