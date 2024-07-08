using System;
using Moq;
using w2.App.Common.DataCacheController;
using w2.Domain.ShortUrl;

namespace w2.App.CommonTests._Helper.DataCacheConfigurator
{
	/// <summary>
	/// ショートURLデータキャッシュ設定クラス
	/// </summary>
	internal class ShortUrlDataCacheConfigurator : IDisposable
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal ShortUrlDataCacheConfigurator(ShortUrlModel[] data)
		{
			RefreshDataCache(data);
		}

		/// <summary>
		/// ショートURLデータキャッシュリフレッシュ
		/// </summary>
		/// <param name="data">ショートURLデータ</param>
		private void RefreshDataCache(ShortUrlModel[] data)
		{
			var shortUrlServiceMock = new Mock<IShortUrlService>();
			shortUrlServiceMock.Setup(s => s.GetAll(It.IsAny<string>())).Returns(data);
			Domain.DomainFacade.Instance.ShortUrlService = shortUrlServiceMock.Object;

			DataCacheControllerFacade.GetShortUrlCacheController().RefreshCacheData();
		}

		/// <summary>
		/// Dispose(キャッシュデータを空でリフレッシュ)
		/// </summary>
		public void Dispose()
		{
			RefreshDataCache(new ShortUrlModel[0]);
		}
	}
}