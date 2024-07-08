using System;
using Moq;
using w2.App.Common.DataCacheController;
using w2.Domain.CountryLocation;

namespace w2.App.CommonTests._Helper.DataCacheConfigurator
{
	/// <summary>
	/// 国ISOコードデータキャッシュ設定クラス
	/// </summary>
	internal class CountryLocationDataCacheConfigurator : IDisposable
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal CountryLocationDataCacheConfigurator(CountryLocationModel[] data)
		{
			RefreshDataCache(data);
		}

		/// <summary>
		/// 国ISOコードデータキャッシュリフレッシュ
		/// </summary>
		/// <param name="data">国ISOコードデータ</param>
		private void RefreshDataCache(CountryLocationModel[] data)
		{
			var countryLocationServiceMock = new Mock<ICountryLocationService>();
			countryLocationServiceMock.Setup(s => s.GetCountryNames()).Returns(data);
			Domain.DomainFacade.Instance.CountryLocationService = countryLocationServiceMock.Object;

			DataCacheControllerFacade.GetCountryLocationCacheController().RefreshCacheData();
		}

		/// <summary>
		/// Dispose(キャッシュデータを空でリフレッシュ)
		/// </summary>
		public void Dispose()
		{
			RefreshDataCache(new CountryLocationModel[0]);
		}
	}
}