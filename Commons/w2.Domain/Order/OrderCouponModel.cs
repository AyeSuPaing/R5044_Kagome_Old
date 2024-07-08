/*
=========================================================================================================
  Module      : 注文クーポンテーブルモデル (OrderCouponModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.Order
{
	/// <summary>
	/// 注文クーポンテーブルモデル
	/// </summary>
	[Serializable]
	public partial class OrderCouponModel : ModelBase<OrderCouponModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public OrderCouponModel()
		{
			this.OrderCouponNo = 1;
			this.CouponNo = 1;
			this.CouponDiscountPrice = null;
			this.CouponDiscountRate = null;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public OrderCouponModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public OrderCouponModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>注文ID</summary>
		[UpdateData(1, "order_id")]
		public string OrderId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERCOUPON_ORDER_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERCOUPON_ORDER_ID] = value; }
		}
		/// <summary>注文クーポン枝番</summary>
		[UpdateData(2, "order_coupon_no")]
		public int OrderCouponNo
		{
			get { return (int)this.DataSource[Constants.FIELD_ORDERCOUPON_ORDER_COUPON_NO]; }
			set { this.DataSource[Constants.FIELD_ORDERCOUPON_ORDER_COUPON_NO] = value; }
		}
		/// <summary>識別ID</summary>
		[UpdateData(3, "dept_id")]
		public string DeptId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERCOUPON_DEPT_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERCOUPON_DEPT_ID] = value; }
		}
		/// <summary>クーポンID</summary>
		[UpdateData(4, "coupon_id")]
		public string CouponId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERCOUPON_COUPON_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERCOUPON_COUPON_ID] = value; }
		}
		/// <summary>枝番</summary>
		[UpdateData(5, "coupon_no")]
		public int CouponNo
		{
			get { return (int)this.DataSource[Constants.FIELD_ORDERCOUPON_COUPON_NO]; }
			set { this.DataSource[Constants.FIELD_ORDERCOUPON_COUPON_NO] = value; }
		}
		/// <summary>クーポンコード</summary>
		[UpdateData(6, "coupon_code")]
		public string CouponCode
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERCOUPON_COUPON_CODE]; }
			set { this.DataSource[Constants.FIELD_ORDERCOUPON_COUPON_CODE] = value; }
		}
		/// <summary>管理用クーポン名</summary>
		[UpdateData(7, "coupon_name")]
		public string CouponName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERCOUPON_COUPON_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERCOUPON_COUPON_NAME] = value; }
		}
		/// <summary>表示用クーポン名</summary>
		[UpdateData(8, "coupon_disp_name")]
		public string CouponDispName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERCOUPON_COUPON_DISP_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERCOUPON_COUPON_DISP_NAME] = value; }
		}
		/// <summary>クーポン種別</summary>
		[UpdateData(9, "coupon_type")]
		public string CouponType
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERCOUPON_COUPON_TYPE]; }
			set { this.DataSource[Constants.FIELD_ORDERCOUPON_COUPON_TYPE] = value; }
		}
		/// <summary>クーポン割引額</summary>
		[UpdateData(10, "coupon_discount_price")]
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
		[UpdateData(11, "coupon_discount_rate")]
		public decimal? CouponDiscountRate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDERCOUPON_COUPON_DISCOUNT_RATE] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_ORDERCOUPON_COUPON_DISCOUNT_RATE];
			}
			set { this.DataSource[Constants.FIELD_ORDERCOUPON_COUPON_DISCOUNT_RATE] = value; }
		}
		/// <summary>作成日</summary>
		[UpdateData(12, "date_created")]
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_ORDERCOUPON_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_ORDERCOUPON_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		[UpdateData(13, "date_changed")]
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_ORDERCOUPON_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_ORDERCOUPON_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		[UpdateData(14, "last_changed")]
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERCOUPON_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_ORDERCOUPON_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
