/*
=========================================================================================================
  Module      : 注文クーポン情報クラス(OrderCoupon.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.App.Common.Option;
using w2.Common.Util;

namespace w2.App.Common.Order
{
	/// <summary>
	/// OrderCoupon の概要の説明です
	/// </summary>
	[Serializable]
	public class OrderCoupon
	{
		/// <summary>割引区分</summary>
		public enum DiscountKbnType
		{
			/// <summary>割引金額</summary>
			Price,

			/// <summary>割引率</summary>
			Rate,

			/// <summary>配送無料</summary>
			FreeShipping
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public OrderCoupon()
		{
			// なにもしない //
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="drvCoupon">注文クーポン情報</param>
		public OrderCoupon(DataRowView drvCoupon)
			: this()
		{
			this.OrderId = (string) drvCoupon[Constants.FIELD_ORDERCOUPON_ORDER_ID];
			this.OrderCouponNo = drvCoupon[Constants.FIELD_ORDERCOUPON_ORDER_COUPON_NO].ToString();
			this.DeptId = (string) drvCoupon[Constants.FIELD_ORDERCOUPON_DEPT_ID];
			this.CouponId = (string) drvCoupon[Constants.FIELD_ORDERCOUPON_COUPON_ID];
			this.CouponNo = drvCoupon[Constants.FIELD_ORDERCOUPON_COUPON_NO].ToString();
			this.CouponCode = (string) drvCoupon[Constants.FIELD_ORDERCOUPON_COUPON_CODE];
			this.CouponName = (string) drvCoupon[Constants.FIELD_ORDERCOUPON_COUPON_NAME];
			this.CouponDispName = (string) drvCoupon[Constants.FIELD_ORDERCOUPON_COUPON_DISP_NAME];
			this.CouponType = (string) drvCoupon[Constants.FIELD_ORDERCOUPON_COUPON_TYPE];
			// 配送無料クーポン
			if (CouponOptionUtility.IsFreeShipping(this.CouponType))
			{
				this.DiscountKbn = DiscountKbnType.FreeShipping;
				this.CouponDiscountPrice = null;
				this.CouponDiscountRate = null;
			}
			// 割引金額クーポン
			else if (drvCoupon[Constants.FIELD_ORDERCOUPON_COUPON_DISCOUNT_PRICE] != DBNull.Value)
			{
				this.DiscountKbn = DiscountKbnType.Price;
				this.CouponDiscountPrice = drvCoupon[Constants.FIELD_ORDERCOUPON_COUPON_DISCOUNT_PRICE].ToString();
				this.CouponDiscountRate = null;
			}
			// 割引率クーポン
			else if (drvCoupon[Constants.FIELD_ORDERCOUPON_COUPON_DISCOUNT_RATE] != DBNull.Value)
			{
				this.DiscountKbn = DiscountKbnType.Rate;
				this.CouponDiscountPrice = null;
				this.CouponDiscountRate = drvCoupon[Constants.FIELD_ORDERCOUPON_COUPON_DISCOUNT_RATE].ToString();
			}

			this.DateCreated = drvCoupon[Constants.FIELD_ORDERCOUPON_DATE_CREATED].ToString();
			this.DateChanged = drvCoupon[Constants.FIELD_ORDERCOUPON_DATE_CHANGED].ToString();
			this.LastChanged = (string) drvCoupon[Constants.FIELD_ORDERCOUPON_LAST_CHANGED];
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="soucre">Data soucre</param>
		public OrderCoupon(Hashtable soucre)
			: this()
		{
			this.OrderId = StringUtility.ToEmpty(soucre[Constants.FIELD_ORDERCOUPON_ORDER_ID]);
			this.OrderCouponNo = StringUtility.ToEmpty(soucre[Constants.FIELD_ORDERCOUPON_ORDER_COUPON_NO]);
			this.DeptId = StringUtility.ToEmpty(soucre[Constants.FIELD_ORDERCOUPON_DEPT_ID]);
			this.CouponId = StringUtility.ToEmpty(soucre[Constants.FIELD_ORDERCOUPON_COUPON_ID]);
			this.CouponNo = StringUtility.ToEmpty(soucre[Constants.FIELD_ORDERCOUPON_COUPON_NO]);
			this.CouponCode = StringUtility.ToEmpty(soucre[Constants.FIELD_ORDERCOUPON_COUPON_CODE]);
			this.CouponName = StringUtility.ToEmpty(soucre[Constants.FIELD_ORDERCOUPON_COUPON_NAME]);
			this.CouponDispName = StringUtility.ToEmpty(soucre[Constants.FIELD_ORDERCOUPON_COUPON_DISP_NAME]);
			this.CouponType = StringUtility.ToEmpty(soucre[Constants.FIELD_ORDERCOUPON_COUPON_TYPE]);
			// 配送無料クーポン
			if (CouponOptionUtility.IsFreeShipping(this.CouponType))
			{
				this.DiscountKbn = DiscountKbnType.FreeShipping;
				this.CouponDiscountPrice = null;
				this.CouponDiscountRate = null;
			}
			// 割引金額クーポン
			else if (soucre[Constants.FIELD_ORDERCOUPON_COUPON_DISCOUNT_PRICE] != DBNull.Value)
			{
				this.DiscountKbn = DiscountKbnType.Price;
				this.CouponDiscountPrice =
					StringUtility.ToEmpty(soucre[Constants.FIELD_ORDERCOUPON_COUPON_DISCOUNT_PRICE]);
				this.CouponDiscountRate = null;
			}
			// 割引率クーポン
			else if (soucre[Constants.FIELD_ORDERCOUPON_COUPON_DISCOUNT_RATE] != DBNull.Value)
			{
				this.DiscountKbn = DiscountKbnType.Rate;
				this.CouponDiscountPrice = null;
				this.CouponDiscountRate =
					StringUtility.ToEmpty(soucre[Constants.FIELD_ORDERCOUPON_COUPON_DISCOUNT_RATE]);
			}

			this.DateCreated = StringUtility.ToEmpty(soucre[Constants.FIELD_ORDERCOUPON_DATE_CREATED]);
			this.DateChanged = StringUtility.ToEmpty(soucre[Constants.FIELD_ORDERCOUPON_DATE_CHANGED]);
			this.LastChanged = StringUtility.ToEmpty(soucre[Constants.FIELD_ORDERCOUPON_LAST_CHANGED]);
		}

		#region "プロパティ"

		/// <summary>注文ID</summary>
		public string OrderId { get; set; }
		/// <summary>注文クーポン枝番</summary>
		public string OrderCouponNo { get; set; }
		/// <summary>識別ID</summary>
		public string DeptId { get; set; }
		/// <summary>クーポンID</summary>
		public string CouponId { get; set; }
		/// <summary>枝番</summary>
		public string CouponNo { get; set; }
		/// <summary>クーポンコード</summary>
		public string CouponCode { get; set; }
		/// <summary>管理用クーポン名</summary>
		public string CouponName { get; set; }
		/// <summary>表示用クーポン名</summary>
		public string CouponDispName { get; set; }
		/// <summary>クーポン種別</summary>
		public string CouponType { get; set; }
		/// <summary>クーポン割引額</summary>
		public string CouponDiscountPrice { get; set; }
		/// <summary>クーポン割引率</summary>
		public string CouponDiscountRate { get; set; }
		/// <summary>作成日</summary>
		public string DateCreated { get; set; }
		/// <summary>更新日</summary>
		public string DateChanged { get; set; }
		/// <summary>最終更新者</summary>
		public string LastChanged { get; set; }
		/// <summary>クーポン割引区分</summary>
		public DiscountKbnType DiscountKbn { get; set; }
		/// <summary>発行者向け回数制限ありクーポン？</summary>
		public bool IsCouponLimit
		{
			get { return CouponOptionUtility.IsCouponLimit(this.CouponType); }
		}
		/// <summary>全員向け回数制限ありクーポン？</summary>
		public bool IsCouponAllLimit
		{
			get { return CouponOptionUtility.IsCouponAllLimit(this.CouponType); }
		}
		/// <summary>ブラックリスト型クーポン？</summary>
		public bool IsBlacklistCoupon
		{
			get { return CouponOptionUtility.IsBlacklistCoupon(this.CouponType); }
		}
		#endregion
	}
}