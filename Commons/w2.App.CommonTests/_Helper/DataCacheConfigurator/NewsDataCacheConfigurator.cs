using System;
using Moq;
using w2.App.Common.DataCacheController;
using w2.Domain.News;

namespace w2.App.CommonTests._Helper.DataCacheConfigurator
{
	/// <summary>
	/// 新着情報データキャッシュ設定クラス
	/// </summary>
	internal class NewsDataCacheConfigurator : IDisposable
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal NewsDataCacheConfigurator(NewsModel[] data)
		{
			RefreshDataCache(data);
		}

		/// <summary>
		/// 新着情報データキャッシュリフレッシュ
		/// </summary>
		/// <param name="data">新着情報データ</param>
		private void RefreshDataCache(NewsModel[] data)
		{
			var newsServiceMock = new Mock<INewsService>();
			newsServiceMock.Setup(s => s.GetTopNewsList()).Returns(data);
			Domain.DomainFacade.Instance.NewsService = newsServiceMock.Object;

			DataCacheControllerFacade.GetNewsCacheController().RefreshCacheData();
		}

		/// <summary>
		/// Dispose(キャッシュデータを空でリフレッシュ)
		/// </summary>
		public void Dispose()
		{
			RefreshDataCache(new NewsModel[0]);
		}
	}
}