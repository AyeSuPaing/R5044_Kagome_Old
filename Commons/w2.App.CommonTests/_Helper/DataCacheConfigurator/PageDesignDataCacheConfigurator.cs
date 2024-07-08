using System;
using Moq;
using w2.App.Common.DataCacheController;
using w2.Domain.PageDesign;

namespace w2.App.CommonTests._Helper.DataCacheConfigurator
{
	/// <summary>
	/// ページデザインデータキャッシュ設定クラス
	/// </summary>
	internal class PageDesignDataCacheConfigurator : IDisposable
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal PageDesignDataCacheConfigurator(PageDesignModel[] data)
		{
			RefreshDataCache(data);
		}

		/// <summary>
		/// ページデザインデータキャッシュリフレッシュ
		/// </summary>
		/// <param name="data">ページデザインデータ</param>
		private void RefreshDataCache(PageDesignModel[] data)
		{
			var pageDesignServiceMock = new Mock<IPageDesignService>();
			pageDesignServiceMock.Setup(s => s.GetAllPage()).Returns(data);
			Domain.DomainFacade.Instance.PageDesignService = pageDesignServiceMock.Object;

			DataCacheControllerFacade.GetPageDesignCacheController().RefreshCacheData();
		}

		/// <summary>
		/// Dispose(キャッシュデータを空でリフレッシュ)
		/// </summary>
		public void Dispose()
		{
			RefreshDataCache(new PageDesignModel[0]);
		}
	}
}