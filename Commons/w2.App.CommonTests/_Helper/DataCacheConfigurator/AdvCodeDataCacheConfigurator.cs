using System;
using Moq;
using w2.App.Common.DataCacheController;
using w2.Domain.AdvCode;

namespace w2.App.CommonTests._Helper.DataCacheConfigurator
{
	/// <summary>
	/// 広告コード情報キャッシュ設定クラス
	/// </summary>
	internal class AdvCodeDataCacheConfigurator : IDisposable
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal AdvCodeDataCacheConfigurator(AdvCodeModel[] data)
		{
			RefreshDataCache(data);
		}

		/// <summary>
		/// 広告コード情報キャッシュリフレッシュ
		/// </summary>
		/// <param name="data">広告コードデータ</param>
		private void RefreshDataCache(AdvCodeModel[] data)
		{
			var advCodeServiceMock = new Mock<IAdvCodeService>();
			advCodeServiceMock.Setup(s => s.GetAll()).Returns(data);
			Domain.DomainFacade.Instance.AdvCodeService = advCodeServiceMock.Object;

			DataCacheControllerFacade.GetAdvCodeCacheController().RefreshCacheData();
		}

		/// <summary>
		/// Dispose(キャッシュデータを空でリフレッシュ)
		/// </summary>
		public void Dispose()
		{
			RefreshDataCache(new AdvCodeModel[0]);
		}
	}
}