/*
=========================================================================================================
  Module      : Member rank rule service interface(IMemberRankRuleService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.MemberRankRule
{
	/// <summary>
	/// Member rank rule service interface
	/// </summary>
	public interface IMemberRankRuleService : IService
	{
		/// <summary>
		/// 会員ランクリスト取得
		/// </summary>
		/// <returns>モデル</returns>
		MemberRankRuleModel[] GetMemberRankRuleList();

		/// <summary>
		/// ターゲットリストから会員ランク変動ルール設定取得
		/// </summary>
		/// <param name="targetId">ターゲットリストID</param>
		/// <returns>モデル</returns>
		MemberRankRuleModel[] GetMemberRankRuleFromTargetList(string targetId);

		/// <summary>
		/// Delete all
		/// </summary>
		/// <returns>Number of affected cases</returns>
		int DeleteAll();
	}
}
