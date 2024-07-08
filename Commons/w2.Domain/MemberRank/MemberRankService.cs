/*
=========================================================================================================
  Module      : 会員ランクサービス (MemberRankService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Transactions;

namespace w2.Domain.MemberRank
{
	/// <summary>
	/// 会員ランクサービス
	/// </summary>
	public class MemberRankService : ServiceBase, IMemberRankService
	{
		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="memberRankId">ランクID</param>
		/// <returns>モデル</returns>
		public static MemberRankModel Get(string memberRankId)
		{
			using (var repository = new MemberRankRepository())
			{
				var model = repository.Get(memberRankId);
				return model;
			}
		}
		#endregion

		#region +GetValid 有効な会員ランク情報を取得
		/// <summary>
		/// 有効な会員ランク情報を取得
		/// </summary>
		/// <param name="memberRankId">ランクID</param>
		/// <returns>モデル</returns>
		public static MemberRankModel GetValid(string memberRankId)
		{
			using (var repository = new MemberRankRepository())
			{
				var model = repository.Get(memberRankId);
				return model.IsValid ? model : null;
			}
		}
		#endregion
		
		#region +GetMemberRankList 会員ランクリスト取得
		/// <summary>
		/// 会員ランクリスト取得
		/// </summary>
		/// <returns>モデル</returns>
		public MemberRankModel[] GetMemberRankList()
		{
			using (var repository = new MemberRankRepository())
			{
				var model = repository.GetMemberRankList();
				return model;
			}
		}
		#endregion

		#region +GetDefaultMemberRank デフォルト会員ランク情報取得
		/// <summary>
		/// デフォルト会員ランク情報取得
		/// </summary>
		/// <returns>モデル</returns>
		public static MemberRankModel GetDefaultMemberRank()
		{
			using (var repository = new MemberRankRepository())
			{
				var model = repository.GetDefaultMemberRank();
				return model;
			}
		}
		#endregion

		#region +CheckMemberRankDelete 会員ランク削除可否判定
		/// <summary>
		/// 会員ランク削除可否判定
		/// </summary>
		/// <returns>True:削除可 False:削除不可</returns>
		public static bool CheckMemberRankDelete(string memberRankId)
		{
			using (var repository = new MemberRankRepository())
			{
				var count = repository.CheckMemberRankUse(memberRankId);
				return (count == 1);
			}
		}
		#endregion
	}
}
