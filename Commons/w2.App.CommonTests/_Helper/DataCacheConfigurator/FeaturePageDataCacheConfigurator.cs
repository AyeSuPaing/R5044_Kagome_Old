using System;
using Moq;
using w2.App.Common.DataCacheController;
using w2.Domain.FeaturePage;

namespace w2.App.CommonTests._Helper.DataCacheConfigurator
{
	/// <summary>
	/// 特集エリアページ情報キャッシュ設定クラス
	/// </summary>
	internal class FeaturePageDataCacheConfigurator : IDisposable
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal FeaturePageDataCacheConfigurator(FeaturePageModel[] data)
		{
			RefreshDataCache(data);
		}

		/// <summary>
		/// 特集エリアページ情報キャッシュリフレッシュ
		/// </summary>
		/// <param name="data">特集エリアページデータ</param>
		private void RefreshDataCache(FeaturePageModel[] data)
		{
			var featurePageServiceMock = new Mock<IFeaturePageService>();
			featurePageServiceMock.Setup(s => s.GetAllPageWithContents()).Returns(data);
			Domain.DomainFacade.Instance.FeaturePageService = featurePageServiceMock.Object;

			DataCacheControllerFacade.GetFeaturePageCacheController().RefreshCacheData();
		}

		/// <summary>
		/// Dispose(キャッシュデータを空でリフレッシュ)
		/// </summary>
		public void Dispose()
		{
			RefreshDataCache(new FeaturePageModel[0]);
		}
	}
}
