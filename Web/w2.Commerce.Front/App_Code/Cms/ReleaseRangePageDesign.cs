/*
=========================================================================================================
  Module      : 公開範囲設定 ページ管理クラス(ReleaseRangePageDesign.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using w2.App.Common.DataCacheController;
using w2.Domain.PageDesign;

/// <summary>
/// 公開範囲設定 ページ管理クラス
/// </summary>
public class ReleaseRangePageDesign : ReleaseRangeBase
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">ページ管理モデル</param>
	public ReleaseRangePageDesign(PageDesignModel model)
	{
		this.ReleaseRangeDefinition = new ReleaseRangeDefinition();
		this.ReleaseRangeDefinition.SetPublish(
			Constants.FLG_PAGEDESIGN_PUBLISH_PUBLIC,
			Constants.FLG_PAGEDESIGN_PUBLISH_PRIVATE);
		this.ReleaseRangeDefinition.SetMember(
			Constants.FLG_PAGEDESIGN_MEMBER_ONLY_TYPE_ALL,
			Constants.FLG_PAGEDESIGN_MEMBER_ONLY_TYPE_MEMBER_ONLY,
			Constants.FLG_PAGEDESIGN_MEMBER_ONLY_TYPE_PARTIAL_MEMBER_ONLY);
		this.ReleaseRangeDefinition.SetTargetListJudgment(
			Constants.FLG_PAGEDESIGN_CONDITION_TARGET_LIST_TYPE_OR,
			Constants.FLG_PAGEDESIGN_CONDITION_TARGET_LIST_TYPE_AND);

		SetCondition(model);
	}

	/// <summary>
	/// 条件セット
	/// </summary>
	/// <param name="model">ページ管理モデル</param>
	private void SetCondition(PageDesignModel model)
	{
		this.ReleaseRangeCondition = new ReleaseRangeCondition
		{
			Publish = model.Publish,
			PublishDateFrom = model.ConditionPublishDateFrom,
			PublishDateTo = model.ConditionPublishDateTo,
			MemberOnlyType = model.ConditionMemberOnlyType,
			MemberRankInfo = (string.IsNullOrEmpty(model.ConditionMemberRankId))
				? null
				: DataCacheControllerFacade.GetMemberRankCacheController().CacheData.FirstOrDefault(m => m.MemberRankId == model.ConditionMemberRankId),
			TargetListType = model.ConditionTargetListType
		};
		this.ReleaseRangeCondition.SetTargetListIds(model.ConditionTargetListIds);
	}
}