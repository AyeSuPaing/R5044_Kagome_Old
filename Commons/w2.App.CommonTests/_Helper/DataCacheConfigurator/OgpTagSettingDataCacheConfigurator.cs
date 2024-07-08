using System;
using Moq;
using w2.App.Common.DataCacheController;
using w2.Common.Sql;
using w2.Domain.OgpTagSetting;

namespace w2.App.CommonTests._Helper.DataCacheConfigurator
{
	/// <summary>
	/// OGPタグ設定データキャッシュ設定クラス
	/// </summary>
	internal class OgpTagSettingDataCacheConfigurator : IDisposable
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal OgpTagSettingDataCacheConfigurator(OgpTagSettingModel[] data)
		{
			RefreshDataCache(data);
		}

		/// <summary>
		/// OGPタグ設定データキャッシュリフレッシュ
		/// </summary>
		/// <param name="data">OGPタグ設定データ</param>
		private void RefreshDataCache(OgpTagSettingModel[] data)
		{
			var ogpTagSettingServiceMock = new Mock<IOgpTagSettingService>();
			ogpTagSettingServiceMock.Setup(s => s.GetAll(It.IsAny<SqlAccessor>())).Returns(data);
			Domain.DomainFacade.Instance.OgpTagSettingService = ogpTagSettingServiceMock.Object;

			DataCacheControllerFacade.GetOgpTagSettingCacheController().RefreshCacheData();
		}

		/// <summary>
		/// Dispose(キャッシュデータを空でリフレッシュ)
		/// </summary>
		public void Dispose()
		{
			RefreshDataCache(new OgpTagSettingModel[0]);
		}
	}
}