/*
=========================================================================================================
  Module      : 会員ランク付与ルールリポジトリ (MemberRankRuleRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.MemberRankRule
{
	/// <summary>
	/// 会員ランク付与ルールリポジトリ
	/// </summary>
	internal class MemberRankRuleRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "MemberRankRule";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal MemberRankRuleRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		internal MemberRankRuleRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~GetMemberRankRuleList 会員ランク変動ルールリスト取得
		/// <summary>
		/// 会員ランクリスト取得
		/// </summary>
		/// <returns>モデル</returns>
		internal MemberRankRuleModel[] GetMemberRankRuleList()
		{
			var dv = Get(XML_KEY_NAME, "GetMemberRankRuleList");
			return dv.Cast<DataRowView>().Select(drv => new MemberRankRuleModel(drv)).ToArray();
		}
		#endregion

		#region ~GetMemberRankListFromTargetList ターゲットリストから会員ランク変動ルール設定取得
		/// <summary>
		/// ターゲットリストから会員ランク変動ルール設定取得
		/// </summary>
		/// <param name="targetId">ターゲットリストID</param>
		/// <returns>モデル</returns>
		internal MemberRankRuleModel[] GetMemberRankRuleFromTargetList(string targetId)
		{
			var input = new Hashtable
			{
				{"target_id", targetId}
			};
			var dv = Get(XML_KEY_NAME, "GetMemberRankRuleFromTargetList", input);
			return dv.Cast<DataRowView>().Select(drv => new MemberRankRuleModel(drv)).ToArray();
		}
		#endregion
		
		#region ~DeleteAll
		/// <summary>
		/// Delete all
		/// </summary>
		/// <returns>Number of affected cases</returns>
		internal int DeleteAll()
		{
			return Exec(XML_KEY_NAME, "DeleteAll");
		}
		#endregion
	}
}