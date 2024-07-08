/*
=========================================================================================================
  Module      : レコメンド表示履歴モデル (RecommendHistoryModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Domain.Recommend
{
	/// <summary>
	/// レコメンド表示履歴モデル
	/// </summary>
	public partial class RecommendHistoryModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>購入済？</summary>
		public bool IsBuy
		{
			get { return (this.OrderedFlg == Constants.FLG_RECOMMENDHISTORY_ORDERED_FLG_BUY); }
		}
		#endregion
	}
}
