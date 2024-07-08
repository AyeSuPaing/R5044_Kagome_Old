/*
=========================================================================================================
  Module      : ポイントルールキャッシュコントローラ(PointRuleCacheController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.DataCacheController.CacheData;
using w2.App.Common.RefreshFileManager;
using w2.Domain;

namespace w2.App.Common.DataCacheController
{
	/// <summary>
	/// ポイントルールキャッシュコントローラ
	/// </summary>
	public class PointRuleCacheController : DataCacheControllerBase<PointRuleCacheData>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal PointRuleCacheController()
			: base(RefreshFileType.PointRules)
		{
		}

		/// <summary>
		/// キャッシュデータリフレッシュ
		/// </summary>
		internal override void RefreshCacheData()
		{
			var sv = DomainFacade.Instance.PointService;

			var currentDateTime = DateTime.Now;

			var cache = new PointRuleCacheData();

			// 優先度の高いキャンペーン
			cache.HightPriorityCampaignRule = sv.GetHightPriorityCampaignRule(Constants.W2MP_DEPT_ID, currentDateTime);

			// 基本ルール
			cache.BasicRule = sv.GetBasicRule(Constants.W2MP_DEPT_ID, currentDateTime);

			// キャッシュ更新日時
			cache.CacheUpdateTime = currentDateTime;

			base.CacheData = cache;
		}
	}
}