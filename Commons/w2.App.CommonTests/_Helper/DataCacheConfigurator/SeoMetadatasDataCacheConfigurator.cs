using System;
using Moq;
using w2.App.Common.DataCacheController;
using w2.Domain.SeoMetadatas;

namespace w2.App.CommonTests._Helper.DataCacheConfigurator
{
	/// <summary>
	/// SEOメタデータキャッシュ設定クラス
	/// </summary>
	internal class SeoMetadatasDataCacheConfigurator : IDisposable
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal SeoMetadatasDataCacheConfigurator(SeoMetadatasModel[] data)
		{
			RefreshDataCache(data);
		}

		/// <summary>
		/// SEOメタデータキャッシュリフレッシュ
		/// </summary>
		/// <param name="data">SEOメタデータ</param>
		private void RefreshDataCache(SeoMetadatasModel[] data)
		{
			var seoMetadatasServiceMock = new Mock<ISeoMetadatasService>();
			seoMetadatasServiceMock.Setup(s => s.GetAll()).Returns(data);
			Domain.DomainFacade.Instance.SeoMetadatasService = seoMetadatasServiceMock.Object;

			DataCacheControllerFacade.GetSeoMetadatasCacheController().RefreshCacheData();
		}

		/// <summary>
		/// Dispose(キャッシュデータを空でリフレッシュ)
		/// </summary>
		public void Dispose()
		{
			RefreshDataCache(new SeoMetadatasModel[0]);
		}
	}
}