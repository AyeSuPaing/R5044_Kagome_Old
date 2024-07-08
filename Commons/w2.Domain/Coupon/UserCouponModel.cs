/*
=========================================================================================================
  Module      : ユーザクーポンテーブルモデル (UserCouponModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.Coupon
{
	/// <summary>
	/// ユーザクーポンテーブルモデル
	/// </summary>
	[Serializable]
	public partial class UserCouponModel : ModelBase<UserCouponModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public UserCouponModel()
		{
			this.CouponNo = 1;
			this.UseFlg = Constants.FLG_USERCOUPON_USE_FLG_UNUSE;
			this.UserCouponCount = null;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserCouponModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserCouponModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>ユーザID</summary>
		[UpdateDataAttribute(1, "user_id")]
		public string UserId
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCOUPON_USER_ID]; }
			set { this.DataSource[Constants.FIELD_USERCOUPON_USER_ID] = value; }
		}
		/// <summary>識別ID</summary>
		[UpdateDataAttribute(2, "dept_id")]
		public string DeptId
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCOUPON_DEPT_ID]; }
			set { this.DataSource[Constants.FIELD_USERCOUPON_DEPT_ID] = value; }
		}
		/// <summary>クーポンID</summary>
		[UpdateDataAttribute(3, "coupon_id")]
		public string CouponId
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCOUPON_COUPON_ID]; }
			set { this.DataSource[Constants.FIELD_USERCOUPON_COUPON_ID] = value; }
		}
		/// <summary>枝番</summary>
		[UpdateDataAttribute(4, "coupon_no")]
		public int CouponNo
		{
			get { return (int)this.DataSource[Constants.FIELD_USERCOUPON_COUPON_NO]; }
			set { this.DataSource[Constants.FIELD_USERCOUPON_COUPON_NO] = value; }
		}
		/// <summary>注文ID</summary>
		[UpdateDataAttribute(5, "order_id")]
		public string OrderId
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCOUPON_ORDER_ID]; }
			set { this.DataSource[Constants.FIELD_USERCOUPON_ORDER_ID] = value; }
		}
		/// <summary>利用フラグ</summary>
		[UpdateDataAttribute(6, "use_flg")]
		public string UseFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCOUPON_USE_FLG]; }
			set { this.DataSource[Constants.FIELD_USERCOUPON_USE_FLG] = value; }
		}
		/// <summary>作成日</summary>
		[UpdateDataAttribute(7, "date_created")]
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_USERCOUPON_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_USERCOUPON_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		[UpdateDataAttribute(8, "date_changed")]
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_USERCOUPON_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_USERCOUPON_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		[UpdateDataAttribute(9, "last_changed")]
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCOUPON_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_USERCOUPON_LAST_CHANGED] = value; }
		}
		/// <summary>ユーザークーポン利用可能回数</summary>
		[UpdateDataAttribute(10, "user_coupon_count")]
		public int? UserCouponCount
		{
			get
			{
				if (this.DataSource[Constants.FIELD_USERCOUPON_USER_COUPON_COUNT] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_USERCOUPON_USER_COUPON_COUNT];
			}
			set { this.DataSource[Constants.FIELD_USERCOUPON_USER_COUPON_COUNT] = value; }
		}
		#endregion
	}
}
