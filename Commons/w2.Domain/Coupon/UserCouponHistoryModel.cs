/*
=========================================================================================================
  Module      : ユーザクーポン履歴テーブルモデル (UserCouponHistoryModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.Coupon
{
	/// <summary>
	/// ユーザクーポン履歴テーブルモデル
	/// </summary>
	[Serializable]
	public partial class UserCouponHistoryModel : ModelBase<UserCouponHistoryModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public UserCouponHistoryModel()
		{
			this.HistoryNo = 1;
			this.CouponInc = 0;
			this.CouponPrice = 0;
			this.Memo = string.Empty;
			this.FixedPurchaseId = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserCouponHistoryModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public UserCouponHistoryModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>ユーザID</summary>
		public string UserId
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCOUPONHISTORY_USER_ID]; }
			set { this.DataSource[Constants.FIELD_USERCOUPONHISTORY_USER_ID] = value; }
		}

		/// <summary>枝番</summary>
		public int HistoryNo
		{
			get { return (int)this.DataSource[Constants.FIELD_USERCOUPONHISTORY_HISTORY_NO]; }
			set { this.DataSource[Constants.FIELD_USERCOUPONHISTORY_HISTORY_NO] = value; }
		}

		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCOUPONHISTORY_DEPT_ID]; }
			set { this.DataSource[Constants.FIELD_USERCOUPONHISTORY_DEPT_ID] = value; }
		}

		/// <summary>クーポンID</summary>
		public string CouponId
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCOUPONHISTORY_COUPON_ID]; }
			set { this.DataSource[Constants.FIELD_USERCOUPONHISTORY_COUPON_ID] = value; }
		}

		/// <summary>クーポンコード</summary>
		public string CouponCode
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCOUPONHISTORY_COUPON_CODE]; }
			set { this.DataSource[Constants.FIELD_USERCOUPONHISTORY_COUPON_CODE] = value; }
		}

		/// <summary>注文ID</summary>
		public string OrderId
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCOUPONHISTORY_ORDER_ID]; }
			set { this.DataSource[Constants.FIELD_USERCOUPONHISTORY_ORDER_ID] = value; }
		}

		/// <summary>クーポン履歴区分</summary>
		public string HistoryKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCOUPONHISTORY_HISTORY_KBN]; }
			set { this.DataSource[Constants.FIELD_USERCOUPONHISTORY_HISTORY_KBN] = value; }
		}

		/// <summary>操作区分</summary>
		public string ActionKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCOUPONHISTORY_ACTION_KBN]; }
			set { this.DataSource[Constants.FIELD_USERCOUPONHISTORY_ACTION_KBN] = value; }
		}

		/// <summary>加算数</summary>
		public decimal CouponInc
		{
			get { return (decimal)this.DataSource[Constants.FIELD_USERCOUPONHISTORY_COUPON_INC]; }
			set { this.DataSource[Constants.FIELD_USERCOUPONHISTORY_COUPON_INC] = value; }
		}

		/// <summary>クーポン金額</summary>
		public decimal CouponPrice
		{
			get { return (decimal)this.DataSource[Constants.FIELD_USERCOUPONHISTORY_COUPON_PRICE]; }
			set { this.DataSource[Constants.FIELD_USERCOUPONHISTORY_COUPON_PRICE] = value; }
		}

		/// <summary>メモ</summary>
		public string Memo
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCOUPONHISTORY_MEMO]; }
			set { this.DataSource[Constants.FIELD_USERCOUPONHISTORY_MEMO] = value; }
		}

		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_USERCOUPONHISTORY_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_USERCOUPONHISTORY_DATE_CREATED] = value; }
		}

		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCOUPONHISTORY_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_USERCOUPONHISTORY_LAST_CHANGED] = value; }
		}
		/// <summary>ユーザークーポン利用可能回数</summary>
		public int? UserCouponCount
		{
			get { return (int?)this.DataSource[Constants.FIELD_USERCOUPONHISTORY_USER_COUPON_COUNT]; }
			set { this.DataSource[Constants.FIELD_USERCOUPONHISTORY_USER_COUPON_COUNT] = value; }
		}
		/// <summary>定期購入ID</summary>
		public string FixedPurchaseId
		{
			get { return (string)this.DataSource[Constants.FIELD_USERCOUPONHISTORY_FIXED_PURCHASE_ID]; }
			set { this.DataSource[Constants.FIELD_USERCOUPONHISTORY_FIXED_PURCHASE_ID] = value; }
		}
		#endregion
	}
}
