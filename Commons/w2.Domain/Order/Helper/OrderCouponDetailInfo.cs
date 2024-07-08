/*
=========================================================================================================
  Module      : 注文クーポン詳細情報クラス (OrderCouponDetailInfo.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using w2.Domain.Coupon.Helper;

namespace w2.Domain.Order.Helper
{
	/// <summary>
	/// 注文クーポン詳細情報
	/// </summary>
	public class OrderCouponDetailInfo : UserCouponDetailInfo
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public OrderCouponDetailInfo()
			: base()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public OrderCouponDetailInfo(DataRowView source)
			: base(source)
		{
		}
		#endregion

		#region プロパティ
		/// <summary>注文クーポン枝番</summary>
		public int OrderCouponNo
		{
			get { return (int)this.DataSource[Constants.FIELD_ORDERCOUPON_ORDER_COUPON_NO]; }
			set { this.DataSource[Constants.FIELD_ORDERCOUPON_ORDER_COUPON_NO] = value; }
		}

		/// <summary>クーポン割引額</summary>
		public decimal? CouponDiscountPrice
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDERCOUPON_COUPON_DISCOUNT_PRICE] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_ORDERCOUPON_COUPON_DISCOUNT_PRICE];
			}
			set { this.DataSource[Constants.FIELD_ORDERCOUPON_COUPON_DISCOUNT_PRICE] = value; }
		}

		/// <summary>クーポン割引率</summary>
		public decimal? CouponDiscountRate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDERCOUPON_COUPON_DISCOUNT_RATE] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_ORDERCOUPON_COUPON_DISCOUNT_RATE];
			}
			set { this.DataSource[Constants.FIELD_ORDERCOUPON_COUPON_DISCOUNT_RATE] = value; }
		}
		#endregion
	}
}