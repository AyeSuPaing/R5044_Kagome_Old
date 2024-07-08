using System;
using Moq;
using w2.App.Common.DataCacheController;
using w2.Domain.Recommend;

namespace w2.App.CommonTests._Helper.DataCacheConfigurator
{
	/// <summary>
	/// レコメンド設定データキャッシュ設定クラス
	/// </summary>
	internal class RecommendDataCacheConfigurator : IDisposable
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal RecommendDataCacheConfigurator(RecommendModel[] data)
		{
			RefreshDataCache(data);
		}

		/// <summary>
		/// レコメンド設定データキャッシュリフレッシュ
		/// </summary>
		/// <param name="data">レコメンド設定データ</param>
		private void RefreshDataCache(RecommendModel[] data)
		{
			var recommendServiceMock = new Mock<IRecommendService>();
			recommendServiceMock.Setup(s => s.GetAll(It.IsAny<string>())).Returns(data);
			Domain.DomainFacade.Instance.RecommendService = recommendServiceMock.Object;

			DataCacheControllerFacade.GetRecommendCacheController().RefreshCacheData();
		}

		/// <summary>
		/// Dispose(キャッシュデータを空でリフレッシュ)
		/// </summary>
		public void Dispose()
		{
			RefreshDataCache(new RecommendModel[0]);
		}
	}
}