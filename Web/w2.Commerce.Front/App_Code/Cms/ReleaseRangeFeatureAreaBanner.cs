/*
=========================================================================================================
  Module      : 公開範囲設定 特集エリアバナークラス(ReleaseRangeFeatureAreaBanner.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using w2.App.Common.DataCacheController;
using w2.Domain.FeatureArea;

namespace Cms
{
	/// <summary>
	/// 公開範囲設定 特集エリアバナークラス
	/// </summary>
	public class ReleaseRangeFeatureAreaBanner : ReleaseRangeBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">特集エリアバナーモデル</param>
		public ReleaseRangeFeatureAreaBanner(FeatureAreaBannerModel model)
		{
			this.ReleaseRangeDefinition = new ReleaseRangeDefinition();
			this.ReleaseRangeDefinition.SetPublish(
				Constants.FLG_FEATUREAREABANNER_PUBLISH_PUBLIC,
				Constants.FLG_FEATUREAREABANNER_PUBLISH_PRIVATE);
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
		private void SetCondition(FeatureAreaBannerModel model)
		{
			this.ReleaseRangeCondition = new ReleaseRangeCondition
			{
				Publish = model.Publish,
				PublishDateFrom = model.ConditionPublishDateFrom,
				PublishDateTo = model.ConditionPublishDateTo,
				MemberOnlyType = model.ConditionMemberOnlyType,
				MemberRankInfo = (string.IsNullOrEmpty(model.ConditionMemberRankId))
					? null
					: DataCacheControllerFacade.GetMemberRankCacheController().CacheData
						.FirstOrDefault(m => m.MemberRankId == model.ConditionMemberRankId),
				TargetListType = model.ConditionTargetListType
			};
			this.ReleaseRangeCondition.SetTargetListIds(model.ConditionTargetListIds);
		}

		/// <summary>
		/// 条件判定
		/// </summary>
		/// <param name="accessUser">アクセスユーザ</param>
		/// <param name="publish">公開状態 チェック有無</param>
		/// <param name="publishDate">公開期間 チェック有無</param>
		/// <param name="memberRank">会員ランク チェック有無</param>
		/// <param name="targetList">ターゲットリスト チェック有無</param>
		/// <returns>公開対象か</returns>
		public bool IsPublish(
			ReleaseRangeAccessUser accessUser,
			CheckType publish = CheckType.Check,
			CheckType publishDate = CheckType.Check,
			CheckType memberRank = CheckType.Check,
			CheckType targetList = CheckType.Check)
		{
			var checkResult = base.Check(accessUser, publish, publishDate, memberRank, targetList);

			// 公開状態・公開期間・ターゲットリストの結果判定
			var result = ((checkResult.Publish == ReleaseRangeResult.RangeResult.In)
				&& (checkResult.PublishDate == ReleaseRangeResult.RangeResult.In)
				&& (checkResult.TargetList == ReleaseRangeResult.RangeResult.In));

			// ログイン状態による結果判定
			result &= (checkResult.RequiredLoginStatus == ReleaseRangeResult.RequiredLogin.NotRequired)
				|| ((checkResult.RequiredLoginStatus == ReleaseRangeResult.RequiredLogin.Required)
					&& accessUser.IsLoggedIn);

			// 会員ランクによる結果判定
			if (this.ReleaseRangeCondition.MemberOnlyType == Constants.FLG_PAGEDESIGN_MEMBER_ONLY_TYPE_PARTIAL_MEMBER_ONLY)
			{
				result &= (checkResult.MemberRank == ReleaseRangeResult.RangeResult.In);
			}

			return result;
		}
	}
}