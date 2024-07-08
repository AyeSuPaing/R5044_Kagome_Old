using System;
using Moq;
using w2.App.Common.DataCacheController;
using w2.Domain.PartsDesign;

namespace w2.App.CommonTests._Helper.DataCacheConfigurator
{
	/// <summary>
	/// パーツデザインデータキャッシュ設定クラス
	/// </summary>
	internal class PartsDesignDataCacheConfigurator : IDisposable
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal PartsDesignDataCacheConfigurator(PartsDesignModel[] data)
		{
			RefreshDataCache(data);
		}

		/// <summary>
		/// パーツデザインデータキャッシュリフレッシュ
		/// </summary>
		/// <param name="data">パーツデザインデータ</param>
		private void RefreshDataCache(PartsDesignModel[] data)
		{
			var partsDesignServiceMock = new Mock<IPartsDesignService>();
			partsDesignServiceMock.Setup(s => s.GetAllParts()).Returns(data);
			Domain.DomainFacade.Instance.PartsDesignService = partsDesignServiceMock.Object;

			DataCacheControllerFacade.GetPartsDesignCacheController().RefreshCacheData();
		}

		/// <summary>
		/// Dispose(キャッシュデータを空でリフレッシュ)
		/// </summary>
		public void Dispose()
		{
			RefreshDataCache(new PartsDesignModel[0]);
		}
	}
}