/*
=========================================================================================================
  Module      : ポイントルールのキャッシュデータ(PointRuleCacheData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.Domain.Point;

namespace w2.App.Common.DataCacheController.CacheData
{
	/// <summary>
	/// ポイントルールのキャッシュデータ
	/// </summary>
	public class PointRuleCacheData
	{
		public PointRuleCacheData()
		{
			BasicRule = new PointRuleModel[]{};
			HightPriorityCampaignRule = new PointRuleModel[]{};
			CacheUpdateTime = DateTime.Now;
		}

		/// <summary>
		/// 優先度の高いポイントキャンペーンルール
		/// w2_PointRule.priorityの昇順で並んでいます
		/// </summary>
		public PointRuleModel[] HightPriorityCampaignRule { get; internal set; }

		/// <summary>
		/// 基本ルール
		/// </summary>
		public PointRuleModel[] BasicRule { get; internal set; }

		/// <summary>キャッシュ更新日</summary>
		public DateTime CacheUpdateTime { get; internal set; }

	}
}
