using System;
using Moq;
using w2.App.Common.DataCacheController;
using w2.Domain.SetPromotion;

namespace w2.App.CommonTests._Helper.DataCacheConfigurator
{
	/// <summary>
	/// セットプロモーションデータキャッシュ設定クラス
	/// </summary>
	internal class SetPromotionDataCacheConfigurator : IDisposable
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal SetPromotionDataCacheConfigurator(SetPromotionModel[] data)
		{
			RefreshDataCache(data);
		}

		/// <summary>
		/// セットプロモーションデータキャッシュリフレッシュ
		/// </summary>
		/// <param name="data">セットプロモーションデータ</param>
		private void RefreshDataCache(SetPromotionModel[] data)
		{
			var setPromotionServiceMock = new Mock<ISetPromotionService>();
			setPromotionServiceMock.Setup(s => s.GetUsable()).Returns(data);
			Domain.DomainFacade.Instance.SetPromotionService = setPromotionServiceMock.Object;

			DataCacheControllerFacade.GetSetPromotionCacheController().RefreshCacheData();
		}

		/// <summary>
		/// Dispose(キャッシュデータを空でリフレッシュ)
		/// </summary>
		public void Dispose()
		{
			RefreshDataCache(new SetPromotionModel[0]);
		}
	}
}