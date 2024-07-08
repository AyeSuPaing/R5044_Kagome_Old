using System;
using Moq;
using w2.App.Common.DataCacheController;
using w2.Domain.MemberRank;

namespace w2.App.CommonTests._Helper.DataCacheConfigurator
{
	/// <summary>
	/// 会員ランクデータキャッシュ設定クラス
	/// </summary>
	internal class MemberRankDataCacheConfigurator : IDisposable
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal MemberRankDataCacheConfigurator(MemberRankModel[] data)
		{
			RefreshDataCache(data);
		}

		/// <summary>
		/// 会員ランクデータキャッシュリフレッシュ
		/// </summary>
		/// <param name="data">会員ランクデータ</param>
		private void RefreshDataCache(MemberRankModel[] data)
		{
			var memnberRankMock = new Mock<IMemberRankService>();
			memnberRankMock.Setup(s => s.GetMemberRankList()).Returns(data);
			Domain.DomainFacade.Instance.MemberRankService = memnberRankMock.Object;

			DataCacheControllerFacade.GetMemberRankCacheController().RefreshCacheData();
		}

		/// <summary>
		/// Dispose(キャッシュデータを空でリフレッシュ)
		/// </summary>
		public void Dispose()
		{
			RefreshDataCache(new MemberRankModel[0]);
		}
	}
}