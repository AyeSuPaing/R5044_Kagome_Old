using System;
using Moq;
using w2.App.Common.DataCacheController;
using w2.Domain.MallCooperationSetting;

namespace w2.App.CommonTests._Helper.DataCacheConfigurator
{
	/// <summary>
	/// モール連携設定情報キャッシュ設定クラス
	/// </summary>
	internal class MallCooperationSettingDataCacheConfigurator : IDisposable
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal MallCooperationSettingDataCacheConfigurator(MallCooperationSettingModel[] data)
		{
			RefreshDataCache(data);
		}

		/// <summary>
		/// モール連携設定情報キャッシュリフレッシュ
		/// </summary>
		/// <param name="data">モール連携設定情報</param>
		private void RefreshDataCache(MallCooperationSettingModel[] data)
		{
			var mallCooperationSettingServiceMock = new Mock<IMallCooperationSettingService>();
			mallCooperationSettingServiceMock.Setup(s => s.GetAll(It.IsAny<string>())).Returns(data);
			Domain.DomainFacade.Instance.MallCooperationSettingService = mallCooperationSettingServiceMock.Object;

			DataCacheControllerFacade.GetMemberRankCacheController().RefreshCacheData();
		}

		/// <summary>
		/// Dispose(キャッシュデータを空でリフレッシュ)
		/// </summary>
		public void Dispose()
		{
			RefreshDataCache(new MallCooperationSettingModel[0]);
		}
	}
}