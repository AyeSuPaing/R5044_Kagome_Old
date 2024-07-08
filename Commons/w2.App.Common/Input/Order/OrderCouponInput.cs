/*
=========================================================================================================
  Module      : 注文クーポンテーブル入力クラス (OrderCouponInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.App.Common.Option;
using w2.Domain.Order;

namespace w2.App.Common.Input.Order
{
	/// <summary>
	/// 注文クーポンテーブル入力クラス
	/// </summary>
	[Serializable]
	public class OrderCouponInput : InputBase<OrderCouponModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public OrderCouponInput()
		{
			this.OrderId = string.Empty;
			this.OrderCouponNo = "1";
			this.DeptId = Constants.CONST_DEFAULT_DEPT_ID;
			this.CouponId = string.Empty;
			this.CouponNo = "1";
			this.CouponCode = string.Empty;
			this.CouponName = string.Empty;
			this.CouponDispName = string.Empty;
			this.CouponType = string.Empty;
			this.CouponDiscountPrice = null;
			this.CouponDiscountRate = null;
			this.DateCreated = DateTime.Now.ToString();
			this.DateChanged = DateTime.Now.ToString();
			this.LastChanged = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public OrderCouponInput(OrderCouponModel model)
			: this()
		{
			this.OrderId = model.OrderId;
			this.OrderCouponNo = model.OrderCouponNo.ToString();
			this.DeptId = model.DeptId;
			this.CouponId = model.CouponId;
			this.CouponNo = model.CouponNo.ToString();
			this.CouponCode = model.CouponCode;
			this.CouponName = model.CouponName;
			this.CouponDispName = model.CouponDispName;
			this.CouponType = model.CouponType;
			this.CouponDiscountPrice = (model.CouponDiscountPrice != null) ? model.CouponDiscountPrice.ToString() : null;
			this.CouponDiscountRate = (model.CouponDiscountRate != null) ? model.CouponDiscountRate.ToString() : null;
			this.DateCreated = model.DateCreated.ToString();
			this.DateChanged = model.DateChanged.ToString();
			this.LastChanged = model.LastChanged;
		}
		#endregion

		#region メソッド
		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns>モデル</returns>
		public override OrderCouponModel CreateModel()
		{
			var model = new OrderCouponModel
			{
				OrderId = this.OrderId,
				OrderCouponNo = int.Parse(this.OrderCouponNo),
				DeptId = this.DeptId,
				CouponId = this.CouponId,
				CouponNo = int.Parse(this.CouponNo),
				CouponCode = this.CouponCode,
				CouponName = this.CouponName,
				CouponDispName = this.CouponDispName,
				CouponType = this.CouponType,
				CouponDiscountPrice = (this.CouponDiscountPrice != null) ? decimal.Parse(this.CouponDiscountPrice) : (decimal?)null,
				CouponDiscountRate = (this.CouponDiscountRate != null) ? decimal.Parse(this.CouponDiscountRate) : (decimal?)null,
				DateCreated = DateTime.Parse(this.DateCreated),
				DateChanged = DateTime.Parse(this.DateChanged),
				LastChanged = this.LastChanged,
			};
			return model;
		}

		/// <summary>
		/// 検証 ※利用しない
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public string Validate()
		{
			return string.Empty;
		}
		#endregion

		#region プロパティ
		/// <summary>注文ID</summary>
		public string OrderId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERCOUPON_ORDER_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERCOUPON_ORDER_ID] = value; }
		}
		/// <summary>注文クーポン枝番</summary>
		public string OrderCouponNo
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERCOUPON_ORDER_COUPON_NO]; }
			set { this.DataSource[Constants.FIELD_ORDERCOUPON_ORDER_COUPON_NO] = value; }
		}
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERCOUPON_DEPT_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERCOUPON_DEPT_ID] = value; }
		}
		/// <summary>クーポンID</summary>
		public string CouponId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERCOUPON_COUPON_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERCOUPON_COUPON_ID] = value; }
		}
		/// <summary>枝番</summary>
		public string CouponNo
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERCOUPON_COUPON_NO]; }
			set { this.DataSource[Constants.FIELD_ORDERCOUPON_COUPON_NO] = value; }
		}
		/// <summary>クーポンコード</summary>
		public string CouponCode
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERCOUPON_COUPON_CODE]; }
			set { this.DataSource[Constants.FIELD_ORDERCOUPON_COUPON_CODE] = value; }
		}
		/// <summary>管理用クーポン名</summary>
		public string CouponName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERCOUPON_COUPON_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERCOUPON_COUPON_NAME] = value; }
		}
		/// <summary>表示用クーポン名</summary>
		public string CouponDispName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERCOUPON_COUPON_DISP_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERCOUPON_COUPON_DISP_NAME] = value; }
		}
		/// <summary>クーポン種別</summary>
		public string CouponType
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERCOUPON_COUPON_TYPE]; }
			set { this.DataSource[Constants.FIELD_ORDERCOUPON_COUPON_TYPE] = value; }
		}
		/// <summary>クーポン割引額</summary>
		public string CouponDiscountPrice
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERCOUPON_COUPON_DISCOUNT_PRICE]; }
			set { this.DataSource[Constants.FIELD_ORDERCOUPON_COUPON_DISCOUNT_PRICE] = value; }
		}
		/// <summary>クーポン割引率</summary>
		public string CouponDiscountRate
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERCOUPON_COUPON_DISCOUNT_RATE]; }
			set { this.DataSource[Constants.FIELD_ORDERCOUPON_COUPON_DISCOUNT_RATE] = value; }
		}
		/// <summary>作成日</summary>
		public string DateCreated
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERCOUPON_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_ORDERCOUPON_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public string DateChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERCOUPON_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_ORDERCOUPON_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERCOUPON_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_ORDERCOUPON_LAST_CHANGED] = value; }
		}
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
		/// <summary>会員限定回数制限付きクーポン？(汎用クーポン？)</summary>
		public bool IsCouponLimitedForRegisteredUser
		{
			get { return CouponOptionUtility.IsCouponLimitedForRegisteredUser(this.CouponType); }
		}
		#endregion
	}
}
