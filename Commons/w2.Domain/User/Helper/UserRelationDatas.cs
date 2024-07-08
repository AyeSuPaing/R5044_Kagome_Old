/*
=========================================================================================================
  Module      : ユーザー関連情報クラス (UserRelationDatas.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using w2.Domain.Coupon.Helper;
using w2.Domain.Point;
using w2.Domain.TwUserInvoice;
using w2.Domain.UserShipping;

namespace w2.Domain.User.Helper
{
	/// <summary>
	/// ユーザー関連データ取得
	/// </summary>
	public class UserRelationDatas
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UserRelationDatas()
		{
			this.User = new UserModel();
			this.UserPoint = new List<UserPointModel>();
			this.UserCouponDetail = new List<UserCouponDetailInfo>();
			this.UserShipping = new List<UserShippingModel>();
			this.UserInvoice = new List<TwUserInvoiceModel>();
		}

		#region プロパティ
		/// <summary>ユーザー情報</summary>
		public UserModel User { get; set; }
		/// <summary>ユーザーポイント</summary>
		public List<UserPointModel> UserPoint { get; set; }
		/// <summary>ユーザークーポン詳細情報</summary>
		public List<UserCouponDetailInfo> UserCouponDetail { get; set; }
		/// <summary>ユーザー配送先情報</summary>
		public List<UserShippingModel> UserShipping { get; set; }
		/// <summary>User Invoice</summary>
		public List<TwUserInvoiceModel> UserInvoice { get; set; }
		#endregion
	}
}
