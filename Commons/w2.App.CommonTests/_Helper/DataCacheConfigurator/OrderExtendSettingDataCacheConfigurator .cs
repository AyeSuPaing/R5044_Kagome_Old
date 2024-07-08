using System;
using Moq;
using w2.App.Common.DataCacheController;
using w2.Domain.OrderExtendSetting;

namespace w2.App.CommonTests._Helper.DataCacheConfigurator
{
	/// <summary>
	/// 注文拡張設定情報キャッシュ設定クラス
	/// </summary>
	internal class OrderExtendSettingCacheConfigurator : IDisposable
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal OrderExtendSettingCacheConfigurator(OrderExtendSettingModel[] data)
		{
			RefreshDataCache(data);
		}

		/// <summary>
		/// 注文拡張設定情報キャッシュリフレッシュ
		/// </summary>
		/// <param name="data">注文拡張設定データ</param>
		private void RefreshDataCache(OrderExtendSettingModel[] data)
		{
			var orderExtendSettingService = new Mock<IOrderExtendSettingService>();
			orderExtendSettingService.Setup(s => s.GetAll()).Returns(data);
			Domain.DomainFacade.Instance.OrderExtendSettingService = orderExtendSettingService.Object;

			DataCacheControllerFacade.GetOrderExtendSettingCacheController().RefreshCacheData();
		}

		/// <summary>
		/// Dispose(キャッシュデータを空でリフレッシュ)
		/// </summary>
		public void Dispose()
		{
			RefreshDataCache(new OrderExtendSettingModel[0]);
		}
	}
}
