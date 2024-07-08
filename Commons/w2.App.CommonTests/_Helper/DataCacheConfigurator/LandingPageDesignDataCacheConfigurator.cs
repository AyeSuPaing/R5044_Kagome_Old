using System;
using Moq;
using w2.App.Common.DataCacheController;
using w2.Domain.LandingPage;

namespace w2.App.CommonTests._Helper.DataCacheConfigurator
{
	/// <summary>
	/// ランディングページデザインデータキャッシュ設定クラス
	/// </summary>
	internal class LandingPageDesignDataCacheConfigurator : IDisposable
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal LandingPageDesignDataCacheConfigurator(LandingPageDesignModel[] data)
		{
			RefreshDataCache(data);
		}

		/// <summary>
		/// ランディングページデザインデータキャッシュリフレッシュ
		/// </summary>
		/// <param name="data">ランディングページデザインデータ</param>
		private void RefreshDataCache(LandingPageDesignModel[] data)
		{
			var landingPageServiceMock = new Mock<ILandingPageService>();
			landingPageServiceMock.Setup(s => s.GetAllPage()).Returns(data);
			Domain.DomainFacade.Instance.LandingPageService = landingPageServiceMock.Object;

			DataCacheControllerFacade.GetLandingPageDeCacheController().RefreshCacheData();
		}

		/// <summary>
		/// Dispose(キャッシュデータを空でリフレッシュ)
		/// </summary>
		public void Dispose()
		{
			RefreshDataCache(new LandingPageDesignModel[0]);
		}
	}
}