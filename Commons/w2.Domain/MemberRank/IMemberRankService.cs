/*
=========================================================================================================
  Module      : 会員ランクサービスのインタフェース(IMemberRankService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.MemberRank
{
	/// <summary>
	/// 会員ランクサービスのインタフェース
	/// </summary>
	public interface IMemberRankService : IService
	{
		/// <summary>
		/// 会員ランクリスト取得
		/// </summary>
		/// <returns>モデル</returns>
		MemberRankModel[] GetMemberRankList();
	}
}