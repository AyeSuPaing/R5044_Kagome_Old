/*
=========================================================================================================
  Module      : クーポン利用ユーザテーブルモデル (CouponUseUserModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.Coupon
{
	/// <summary>
	/// クーポン利用ユーザテーブルモデル
	/// </summary>
	[Serializable]
	public partial class CouponUseUserModel : ModelBase<CouponUseUserModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CouponUseUserModel()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public CouponUseUserModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public CouponUseUserModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>クーポンID</summary>
		public string CouponId
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPONUSEUSER_COUPON_ID]; }
			set { this.DataSource[Constants.FIELD_COUPONUSEUSER_COUPON_ID] = value; }
		}
		/// <summary>クーポン利用ユーザー</summary>
		public string CouponUseUser
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPONUSEUSER_COUPON_USE_USER]; }
			set { this.DataSource[Constants.FIELD_COUPONUSEUSER_COUPON_USE_USER] = value; }
		}
		/// <summary>注文ID</summary>
		public string OrderId
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPONUSEUSER_ORDER_ID]; }
			set { this.DataSource[Constants.FIELD_COUPONUSEUSER_ORDER_ID] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_COUPONUSEUSER_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_COUPONUSEUSER_DATE_CREATED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPONUSEUSER_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_COUPONUSEUSER_LAST_CHANGED] = value; }
		}
		/// <summary>定期購入ID</summary>
		public string FixedPurchaseId
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPONUSEUSER_FIXED_PURCHASE_ID]; }
			set { this.DataSource[Constants.FIELD_COUPONUSEUSER_FIXED_PURCHASE_ID] = value; }
		}
		#endregion
	}
}
