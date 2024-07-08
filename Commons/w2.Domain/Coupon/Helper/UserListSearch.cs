/*
=========================================================================================================
  Module      : ユーザー&クーポン情報リスト検索のためのヘルパクラス (UserListSearch.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;

namespace w2.Domain.Coupon.Helper
{
	/// <summary>
	/// ユーザー＆クーポン情報リスト検索結果情報
	/// </summary>
	[Serializable]
	public class UserListSearchResult : w2.Domain.User.UserModel
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="drv">データソース</param>
		public UserListSearchResult(DataRowView drv)
			: base(drv)
		{
		}
		#endregion

		#region プロパティ
		/// <summary>
		/// 利用可能クーポン数（未利用クーポン）
		/// </summary>
		public int UnusedCoupon
		{
			get { return (int)this.DataSource["unused_coupon"]; }
			set { this.DataSource["unused_coupon"] = value; }
		}

		/// <summary>
		/// 利用済みクーポン数
		/// </summary>
		public int UsedCoupon
		{
			get { return (int)this.DataSource["used_coupon"]; }
			set { this.DataSource["used_coupon"] = value; }
		}
		#endregion
	}
}
