/*
=========================================================================================================
  Module      : ユーザークーポン履歴リスト検索のためのヘルパクラス (UserCouponHistoryListSearch.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.Coupon.Helper
{
	/// <summary>
	/// ユーザークーポン履歴一覧検索結果情報
	/// </summary>
	[Serializable]
	public class UserCouponHistoryListSearchResult : w2.Domain.Coupon.UserCouponHistoryModel
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="drv">データソース</param>
		public UserCouponHistoryListSearchResult(DataRowView drv)
			: base(drv)
		{
		}
		#endregion

		#region プロパティ
		/// <summary>
		/// クーポン名
		/// </summary>
		public string CouponName
		{
			get { return (string)this.DataSource["coupon_name"]; }
			set { this.DataSource["coupon_name"] = value; }
		}

		/// <summary>
		/// 行番号
		/// </summary>
		public long RowNum
		{
			get { return (long)this.DataSource["row_num"]; }
			set { this.DataSource["row_num"] = value; }
		}

		/// <summary>
		/// 検索結果の全て件数
		/// </summary>
		public int RowCount
		{
			get { return (int)this.DataSource["row_count"]; }
			set { this.DataSource["row_count"] = value; }
		}
		#endregion
	}
}
