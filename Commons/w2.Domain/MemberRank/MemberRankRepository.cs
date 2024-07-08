/*
=========================================================================================================
  Module      : 会員ランクリポジトリ (MemberRankRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.MemberRank
{
	/// <summary>
	/// 会員ランクリポジトリ
	/// </summary>
	internal class MemberRankRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "MemberRank";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal MemberRankRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MemberRankRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="memberRankId">ランクID</param>
		/// <returns>モデル</returns>
		internal MemberRankModel Get(string memberRankId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID, memberRankId},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			return dv.Cast<DataRowView>().Select(drv => new MemberRankModel(drv)).FirstOrDefault();
		}
		#endregion

		#region ~GetMemberRankList 会員ランクリスト取得
		/// <summary>
		/// 会員ランクリスト取得
		/// </summary>
		/// <returns>モデル</returns>
		internal MemberRankModel[] GetMemberRankList()
		{
			var dv = Get(XML_KEY_NAME, "GetMemberRankList");
			return dv.Cast<DataRowView>().Select(drv => new MemberRankModel(drv)).ToArray();
		}
		#endregion

		#region ~GetDefaultMemberRank デフォルト会員ランク情報取得
		/// <summary>
		/// デフォルト会員ランク情報取得
		/// </summary>
		/// <returns>モデル</returns>
		internal MemberRankModel GetDefaultMemberRank()
		{
			var dv = Get(XML_KEY_NAME, "GetDefaultMemberRank");
			if (dv.Count == 0) return null;
			return new MemberRankModel(dv[0]);
		}
		#endregion

		#region ~CheckMemberRankUse 会員ランク利用判定
		/// <summary>
		/// 会員ランク利用判定
		/// </summary>
		/// <returns>0:利用有 1:利用無</returns>
		internal int CheckMemberRankUse(string memberRankId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID, memberRankId},
			};
			var dv = Get(XML_KEY_NAME, "CheckMemberRankUse", ht);
			return (int)dv[0]["check_count"];
		}
		#endregion
	}
}
