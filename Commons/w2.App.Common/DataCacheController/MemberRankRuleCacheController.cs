/*
=========================================================================================================
  Module      : 会員ランク変動ルールキャッシュコントローラ(MemberRankRuleCacheController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.RefreshFileManager;
using w2.Domain;
using w2.Domain.MemberRankRule;

namespace w2.App.Common.DataCacheController
{
	/// <summary>
	/// 会員ランクキャッシュコントローラ
	/// </summary>
	public class MemberRankRuleCacheController : DataCacheControllerBase<MemberRankRuleModel[]>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal MemberRankRuleCacheController()
			: base(RefreshFileType.MemberRank)
		{
		}

		/// <summary>
		/// キャッシュデータリフレッシュ
		/// </summary>
		internal override void RefreshCacheData()
		{
			this.CacheData = DomainFacade.Instance.MemberRankRuleService.GetMemberRankRuleList();
		}
	}
}
