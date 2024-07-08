/*
=========================================================================================================
  Module      : 会員ランク更新履歴リポジトリ (UserMemberRankHistoryRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using w2.Common.Sql;

namespace w2.Domain.UserMemberRankHistory
{
	/// <summary>
	/// 会員ランク更新履歴リポジトリ
	/// </summary>
	internal class UserMemberRankHistoryRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "UserMemberRankHistory";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal UserMemberRankHistoryRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		internal UserMemberRankHistoryRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~GetByUserId 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <returns>モデル</returns>
		internal UserMemberRankHistoryModel GetByUserId(string userId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERMEMBERRANKHISTORY_USER_ID, userId},
			};
			var dv = Get(XML_KEY_NAME, "GetByUserId", ht);
			return (dv.Count != 0) ? new UserMemberRankHistoryModel(dv[0]) : null;
		}
		#endregion

	}
}