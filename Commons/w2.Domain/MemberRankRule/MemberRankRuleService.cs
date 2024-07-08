/*
=========================================================================================================
  Module      : 会員ランク付与ルールサービス (MemberRankRuleService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.MemberRankRule
{
	/// <summary>
	/// 会員ランク付与ルールサービス
	/// </summary>
	public class MemberRankRuleService : ServiceBase, IMemberRankRuleService
	{
		#region +GetMemberRankList 会員ランクリスト取得
		/// <summary>
		/// 会員ランクリスト取得
		/// </summary>
		/// <returns>モデル</returns>
		public MemberRankRuleModel[] GetMemberRankRuleList()
		{
			using (var repository = new MemberRankRuleRepository())
			{
				var model = repository.GetMemberRankRuleList();
				return model;
			}
		}
		#endregion

		#region +GetMemberRankListFromTargetList ターゲットリストから会員ランク変動ルール設定取得
		/// <summary>
		/// ターゲットリストから会員ランク変動ルール設定取得
		/// </summary>
		/// <param name="targetId">ターゲットリストID</param>
		/// <returns>モデル</returns>
		public MemberRankRuleModel[] GetMemberRankRuleFromTargetList(string targetId)
		{
			using (var repository = new MemberRankRuleRepository())
			{
				var model = repository.GetMemberRankRuleFromTargetList(targetId);
				return model;
			}
		}
		#endregion

		#region ~DeleteAll
		/// <summary>
		/// Delete all
		/// </summary>
		/// <returns>Number of affected cases</returns>
		public int DeleteAll()
		{
			using (var repository = new MemberRankRuleRepository())
			{
				return repository.DeleteAll();
			}
		}
		#endregion
	}
}
