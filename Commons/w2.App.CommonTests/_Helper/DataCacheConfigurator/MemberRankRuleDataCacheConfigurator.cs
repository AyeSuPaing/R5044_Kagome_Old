using System;
using Moq;
using w2.App.Common.DataCacheController;
using w2.Domain.MemberRankRule;

namespace w2.App.CommonTests._Helper.DataCacheConfigurator
{
	/// <summary>
	/// 会員ランクルール情報キャッシュ設定クラス
	/// </summary>
	internal class MemberRankRuleDataCacheConfigurator : IDisposable
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal MemberRankRuleDataCacheConfigurator(MemberRankRuleModel[] data)
		{
			RefreshDataCache(data);
		}

		/// <summary>
		/// 会員ランクルール情報キャッシュリフレッシュ
		/// </summary>
		/// <param name="data">会員ランクルールデータ</param>
		private void RefreshDataCache(MemberRankRuleModel[] data)
		{
			var memberRankRuleServiceMock = new Mock<IMemberRankRuleService>();
			memberRankRuleServiceMock.Setup(s => s.GetMemberRankRuleList()).Returns(data);
			Domain.DomainFacade.Instance.MemberRankRuleService = memberRankRuleServiceMock.Object;

			DataCacheControllerFacade.GetMemberRankRuleCacheController().RefreshCacheData();
		}

		/// <summary>
		/// Dispose(キャッシュデータを空でリフレッシュ)
		/// </summary>
		public void Dispose()
		{
			RefreshDataCache(new MemberRankRuleModel[0]);
		}
	}
}
