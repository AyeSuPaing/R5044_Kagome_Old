/*
=========================================================================================================
  Module      : 会員ランクマスタモデル (MemberRankModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Domain.MemberRank
{
	/// <summary>
	/// 会員ランクマスタモデル
	/// </summary>
	public partial class MemberRankModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>有効か</summary>
		public bool IsValid { get { return this.ValidFlg == Constants.FLG_MEMBERRANK_VALID_FLG_VALID; } }
		#endregion
	}
}
