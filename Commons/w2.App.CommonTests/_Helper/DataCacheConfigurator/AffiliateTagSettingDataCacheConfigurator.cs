using System;
using Moq;
using w2.App.Common.DataCacheController;
using w2.Domain.Affiliate;

namespace w2.App.CommonTests._Helper.DataCacheConfigurator
{
	/// <summary>
	/// アフィリエイトタグデータキャッシュ設定クラス
	/// </summary>
	internal class AffiliateTagSettingDataCacheConfigurator : IDisposable
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal AffiliateTagSettingDataCacheConfigurator(AffiliateTagSettingModel[] data)
		{
			RefreshDataCache(data);
		}

		/// <summary>
		/// アフィリエイトタグデータキャッシュリフレッシュ
		/// </summary>
		/// <param name="data">アフィリエイトタグデータ</param>
		private void RefreshDataCache(AffiliateTagSettingModel[] data)
		{
			var affiliateTagSettingServiceMock = new Mock<IAffiliateTagSettingService>();
			affiliateTagSettingServiceMock.Setup(s => s.GetAllIncludeConditionModels()).Returns(data);
			Domain.DomainFacade.Instance.AffiliateTagSettingService = affiliateTagSettingServiceMock.Object;

			DataCacheControllerFacade.GetAffiliateTagSettingCacheController().RefreshCacheData();
		}

		/// <summary>
		/// Dispose(キャッシュデータを空でリフレッシュ)
		/// </summary>
		public void Dispose()
		{
			RefreshDataCache(new AffiliateTagSettingModel[0]);
		}
	}
}