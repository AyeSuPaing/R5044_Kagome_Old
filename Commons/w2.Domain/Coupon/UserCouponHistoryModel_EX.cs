/*
=========================================================================================================
  Module      : ユーザクーポン履歴テーブルモデル (UserCouponHistoryModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.Coupon
{
	/// <summary>
	/// ユーザクーポン履歴テーブルモデル
	/// </summary>
	public partial class UserCouponHistoryModel
	{
		#region プロパティ
		/// <summary>枝番</summary>
		public int CouponNo
		{
			get { return (int)this.DataSource[Constants.FIELD_USERCOUPON_COUPON_NO]; }
			set { this.DataSource[Constants.FIELD_USERCOUPON_COUPON_NO] = value; }
		}
		#endregion
	}
}
