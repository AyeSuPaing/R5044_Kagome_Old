using System;
using Moq;
using w2.App.Common.DataCacheController;
using w2.Domain.SubscriptionBox;

namespace w2.App.CommonTests._Helper.DataCacheConfigurator
{
	/// <summary>
	/// 頒布会情報キャッシュ設定クラス
	/// </summary>
	internal class SubscriptionBoxDataCacheConfigurator : IDisposable
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal SubscriptionBoxDataCacheConfigurator(SubscriptionBoxModel[] data)
		{
			RefreshDataCache(data);
		}

		/// <summary>
		/// 頒布会情報キャッシュリフレッシュ
		/// </summary>
		/// <param name="data">頒布会データ</param>
		private void RefreshDataCache(SubscriptionBoxModel[] data)
		{
			var subscriptionBoxServiceMock = new Mock<ISubscriptionBoxService>();
			subscriptionBoxServiceMock.Setup(s => s.GetValidOnesWithChildren()).Returns(data);
			Domain.DomainFacade.Instance.SubscriptionBoxService = subscriptionBoxServiceMock.Object;

			DataCacheControllerFacade.GetSubscriptionBoxCacheController().RefreshCacheData();
		}

		/// <summary>
		/// Dispose(キャッシュデータを空でリフレッシュ)
		/// </summary>
		public void Dispose()
		{
			RefreshDataCache(new SubscriptionBoxModel[0]);
		}
	}
}
