/*
=========================================================================================================
  Module      : ユーザークーポン詳細情報クラス (UserCouponDetailInfo.cs)
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
	/// ユーザークーポン詳細情報
	/// </summary>
	[Serializable]
	public class UserCouponDetailInfo : CouponModel
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public UserCouponDetailInfo()
			: base()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserCouponDetailInfo(DataRowView source)
			: base(source)
		{
		}
		#endregion

		#region プロパティ
		/// <summary>ユーザID</summary>
		[UpdateData(1, "user_id")]
		public string UserId
		{
			get
			{
				if ((this.DataSource[Constants.FIELD_USERCOUPON_USER_ID] == DBNull.Value)
					|| (this.DataSource[Constants.FIELD_USERCOUPON_USER_ID] == null)) return null;
				return (string)this.DataSource[Constants.FIELD_USERCOUPON_USER_ID];
			}
			set { this.DataSource[Constants.FIELD_USERCOUPON_USER_ID] = value; }
		}

		/// <summary>枝番</summary>
		[UpdateData(2, "coupon_no")]
		public int? CouponNo
		{
			get
			{
				if ((this.DataSource[Constants.FIELD_USERCOUPON_COUPON_NO] == DBNull.Value)
					|| (this.DataSource[Constants.FIELD_USERCOUPON_COUPON_NO] == null)) return null;
				return (int)this.DataSource[Constants.FIELD_USERCOUPON_COUPON_NO];
			}
			set { this.DataSource[Constants.FIELD_USERCOUPON_COUPON_NO] = value; }
		}

		/// <summary>注文ID</summary>
		public string OrderId
		{
			get
			{
				if ((this.DataSource[Constants.FIELD_USERCOUPON_ORDER_ID] == DBNull.Value)
					|| (this.DataSource[Constants.FIELD_USERCOUPON_ORDER_ID] == null)) return null;
				return (string)this.DataSource[Constants.FIELD_USERCOUPON_ORDER_ID];
			}
			set { this.DataSource[Constants.FIELD_USERCOUPON_ORDER_ID] = value; }
		}

		/// <summary>利用フラグ</summary>
		[UpdateData(3, "use_flg")]
		public string UseFlg
		{
			get
			{
				if ((this.DataSource[Constants.FIELD_USERCOUPON_USE_FLG] == DBNull.Value)
					|| (this.DataSource[Constants.FIELD_USERCOUPON_USE_FLG] == null)) return null;
				return (string)this.DataSource[Constants.FIELD_USERCOUPON_USE_FLG];
			}
			set { this.DataSource[Constants.FIELD_USERCOUPON_USE_FLG] = value; }
		}

		/// <summary>クーポン有効期間(開始)</summary>
		public DateTime? ExpireBgn
		{
			get
			{
				if (this.DataSource["expire_bgn"] == DBNull.Value) return null;
				return (DateTime)this.DataSource["expire_bgn"];
			}
			set { this.DataSource["expire_bgn"] = value; }
		}

		/// <summary>クーポン有効期間(終了)</summary>
		public DateTime? ExpireEnd
		{
			get
			{
				if (this.DataSource["expire_end"] == DBNull.Value) return null;
				return (DateTime)this.DataSource["expire_end"];
			}
			set { this.DataSource["expire_end"] = value; }
		}

		/// <summary>クーポン表示順番</summary>
		public int? Sort
		{
			get
			{
				if (this.DataSource["sort"] == DBNull.Value) return null;
				return (int?)this.DataSource["sort"];
			}
			set { this.DataSource["sort"] = value; }
		}

		/// <summary>クーポン利用可能枚数</summary>
		public int? UserCouponCount
		{
			get
			{
				if (this.DataSource["user_coupon_count"] == DBNull.Value) return null;
				return (int?)this.DataSource["user_coupon_count"];
			}
			set { this.DataSource["user_coupon_count"] = value; }
		}

		/// <summary>
		/// 検索結果の全て件数
		/// </summary>
		public int RowCount
		{
			get { return (int)this.DataSource["row_count"]; }
			set { this.DataSource["row_count"] = value; }
		}
		/// <summary> 表示名（表示用の名称＞管理用の名称の優先度）</summary>
		public string DisplayName
		{
			get
			{
				return ((string.IsNullOrEmpty(this.CouponDispName) == false)
					? this.CouponDispName
					: this.CouponName);
			}
		}
		#endregion
	}
}