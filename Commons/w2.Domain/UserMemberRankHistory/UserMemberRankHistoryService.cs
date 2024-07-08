/*
=========================================================================================================
  Module      : 会員ランク更新履歴サービス (UserMemberRankHistoryService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.UserMemberRankHistory
{
	/// <summary>
	/// 会員ランク更新履歴サービス
	/// </summary>
	public class UserMemberRankHistoryService : ServiceBase
	{
		#region +GetByUserId 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <returns>モデル</returns>
		public UserMemberRankHistoryModel GetByUserId(string userId)
		{
			using (var repository = new UserMemberRankHistoryRepository())
			{
				var model = repository.GetByUserId(userId);
				return model;
			}
		}
		#endregion
	}
}