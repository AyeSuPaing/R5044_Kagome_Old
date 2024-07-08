using System;
using Moq;
using w2.App.Common.DataCacheController;
using w2.Domain.Novelty;

namespace w2.App.CommonTests._Helper.DataCacheConfigurator
{
	/// <summary>
	/// ノベルティデータキャッシュ設定クラス
	/// </summary>
	internal class NoveltyDataCacheConfigurator : IDisposable
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal NoveltyDataCacheConfigurator(NoveltyModel[] data)
		{
			RefreshDataCache(data);
		}

		/// <summary>
		/// ノベルティデータキャッシュリフレッシュ
		/// </summary>
		/// <param name="data">ノベルティデータ</param>
		private void RefreshDataCache(NoveltyModel[] data)
		{
			var noveltyServiceMock = new Mock<INoveltyService>();
			noveltyServiceMock.Setup(s => s.GetAll(It.IsAny<string>())).Returns(data);
			Domain.DomainFacade.Instance.NoveltyService = noveltyServiceMock.Object;

			DataCacheControllerFacade.GetNoveltyCacheController().RefreshCacheData();
		}

		/// <summary>
		/// Dispose(キャッシュデータを空でリフレッシュ)
		/// </summary>
		public void Dispose()
		{
			RefreshDataCache(new NoveltyModel[0]);
		}
	}
}