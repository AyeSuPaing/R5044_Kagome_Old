using System;
using Moq;
using w2.App.Common.DataCacheController;
using w2.Domain.Point;

namespace w2.App.CommonTests._Helper.DataCacheConfigurator
{
	/// <summary>
	/// ポイントルールデータキャッシュ設定クラス
	/// </summary>
	internal class PointRuleDataCacheConfigurator : IDisposable
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="basicRuleData">基本ポイントルールデータ</param>
		/// <param name="hightPriorityCampaignRuleData">優先度が高いポイントキャンペーンデータ</param>
		internal PointRuleDataCacheConfigurator(PointRuleModel[] basicRuleData, PointRuleModel[] hightPriorityCampaignRuleData)
		{
			RefreshDataCache(basicRuleData, hightPriorityCampaignRuleData);
		}

		/// <summary>
		/// ポイントルールデータキャッシュリフレッシュ
		/// </summary>
		/// <param name="basicRuleData">基本ポイントルールデータ</param>
		/// <param name="hightPriorityCampaignRuleData">優先度が高いポイントキャンペーンデータ</param>
		private void RefreshDataCache(PointRuleModel[] basicRuleData, PointRuleModel[] hightPriorityCampaignRuleData)
		{
			var pointServiceMock = new Mock<IPointService>();
			pointServiceMock.Setup(s => s.GetBasicRule(It.IsAny<string>(), It.IsAny<DateTime>())).Returns(basicRuleData);
			pointServiceMock.Setup(s => s.GetHightPriorityCampaignRule(It.IsAny<string>(), It.IsAny<DateTime>())).Returns(hightPriorityCampaignRuleData);
			Domain.DomainFacade.Instance.PointService = pointServiceMock.Object;

			DataCacheControllerFacade.GetPointRuleCacheController().RefreshCacheData();
		}

		/// <summary>
		/// Dispose(キャッシュデータを空でリフレッシュ)
		/// </summary>
		public void Dispose()
		{
			RefreshDataCache(new PointRuleModel[0], new PointRuleModel[0]);
		}
	}
}