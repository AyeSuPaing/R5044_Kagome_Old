/*
=========================================================================================================
  Module      : クーポンテーブルモデル (CouponModel.cs)
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
	/// クーポンテーブルモデル
	/// </summary>
	[Serializable]
	public partial class CouponModel : ModelBase<CouponModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CouponModel()
		{
			this.CouponCount = 0;
			this.DiscountPrice = null;
			this.DiscountRate = null;
			this.ExpireDay = null;
			this.ExpireDateBgn = null;
			this.ExpireDateEnd = null;
			this.ProductKbn = Constants.FLG_COUPON_PRODUCT_KBN_TARGET;
			this.ExceptionalIcon = 0;
			this.UsablePrice = null;
			this.UseTogetherFlg = Constants.FLG_COUPON_USE_TOGETHER_FLG_UNUSE;
			this.ValidFlg = Constants.FLG_COUPON_VALID_FLG_VALID;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public CouponModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public CouponModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_DEPT_ID]; }
			set { this.DataSource[Constants.FIELD_COUPON_DEPT_ID] = value; }
		}

		/// <summary>クーポンID</summary>
		[UpdateData(4, "coupon_id")]
		public string CouponId
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_COUPON_ID]; }
			set { this.DataSource[Constants.FIELD_COUPON_COUPON_ID] = value; }
		}

		/// <summary>クーポンコード</summary>
		[UpdateData(5, "coupon_code")]
		public string CouponCode
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_COUPON_CODE]; }
			set { this.DataSource[Constants.FIELD_COUPON_COUPON_CODE] = value; }
		}

		/// <summary>管理用クーポン名</summary>
		public string CouponName
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_COUPON_NAME]; }
			set { this.DataSource[Constants.FIELD_COUPON_COUPON_NAME] = value; }
		}

		/// <summary>表示用クーポン名</summary>
		[UpdateData(6, "coupon_disp_name")]
		public string CouponDispName
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_COUPON_DISP_NAME]; }
			set { this.DataSource[Constants.FIELD_COUPON_COUPON_DISP_NAME] = value; }
		}

		/// <summary>モバイル用表示用クーポン名</summary>
		public string CouponDispNameMobile
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_COUPON_DISP_NAME_MOBILE]; }
			set { this.DataSource[Constants.FIELD_COUPON_COUPON_DISP_NAME_MOBILE] = value; }
		}

		/// <summary>クーポン説明文</summary>
		public string CouponDiscription
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_COUPON_DISCRIPTION]; }
			set { this.DataSource[Constants.FIELD_COUPON_COUPON_DISCRIPTION] = value; }
		}

		/// <summary>モバイル用クーポン説明文</summary>
		public string CouponDiscriptionMobile
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_COUPON_DISCRIPTION_MOBILE]; }
			set { this.DataSource[Constants.FIELD_COUPON_COUPON_DISCRIPTION_MOBILE] = value; }
		}

		/// <summary>クーポン種別</summary>
		public string CouponType
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_COUPON_TYPE]; }
			set { this.DataSource[Constants.FIELD_COUPON_COUPON_TYPE] = value; }
		}

		/// <summary>クーポン利用可能回数</summary>
		public int CouponCount
		{
			get { return (int)this.DataSource[Constants.FIELD_COUPON_COUPON_COUNT]; }
			set { this.DataSource[Constants.FIELD_COUPON_COUPON_COUNT] = value; }
		}

		/// <summary>クーポン発行期間(開始)</summary>
		public DateTime PublishDateBgn
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_COUPON_PUBLISH_DATE_BGN]; }
			set { this.DataSource[Constants.FIELD_COUPON_PUBLISH_DATE_BGN] = value; }
		}

		/// <summary>クーポン発行期間(終了)</summary>
		public DateTime PublishDateEnd
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_COUPON_PUBLISH_DATE_END]; }
			set { this.DataSource[Constants.FIELD_COUPON_PUBLISH_DATE_END] = value; }
		}

		/// <summary>クーポン商品割引額</summary>
		public decimal? DiscountPrice
		{
			get
			{
				if (this.DataSource[Constants.FIELD_COUPON_DISCOUNT_PRICE] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_COUPON_DISCOUNT_PRICE];
			}
			set { this.DataSource[Constants.FIELD_COUPON_DISCOUNT_PRICE] = value; }
		}

		/// <summary>クーポン商品割引率</summary>
		public decimal? DiscountRate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_COUPON_DISCOUNT_RATE] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_COUPON_DISCOUNT_RATE];
			}
			set { this.DataSource[Constants.FIELD_COUPON_DISCOUNT_RATE] = value; }
		}

		/// <summary>クーポン有効期限(日)</summary>
		public int? ExpireDay
		{
			get
			{
				if (this.DataSource[Constants.FIELD_COUPON_EXPIRE_DAY] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_COUPON_EXPIRE_DAY];
			}
			set { this.DataSource[Constants.FIELD_COUPON_EXPIRE_DAY] = value; }
		}

		/// <summary>クーポン有効期間(開始)</summary>
		public DateTime? ExpireDateBgn
		{
			get
			{
				if (this.DataSource[Constants.FIELD_COUPON_EXPIRE_DATE_BGN] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_COUPON_EXPIRE_DATE_BGN];
			}
			set { this.DataSource[Constants.FIELD_COUPON_EXPIRE_DATE_BGN] = value; }
		}

		/// <summary>クーポン有効期間(終了)</summary>
		public DateTime? ExpireDateEnd
		{
			get
			{
				if (this.DataSource[Constants.FIELD_COUPON_EXPIRE_DATE_END] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_COUPON_EXPIRE_DATE_END];
			}
			set { this.DataSource[Constants.FIELD_COUPON_EXPIRE_DATE_END] = value; }
		}

		/// <summary>クーポン対象商品区分</summary>
		public string ProductKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_PRODUCT_KBN]; }
			set { this.DataSource[Constants.FIELD_COUPON_PRODUCT_KBN] = value; }
		}

		/// <summary>クーポン例外商品</summary>
		public string ExceptionalProduct
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_PRODUCT]; }
			set { this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_PRODUCT] = value; }
		}

		/// <summary>クーポン例外商品アイコン</summary>
		public int ExceptionalIcon
		{
			get { return (int)this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON]; }
			set { this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_ICON] = value; }
		}

		/// <summary>クーポン利用最低購入金額</summary>
		public decimal? UsablePrice
		{
			get
			{
				if (this.DataSource[Constants.FIELD_COUPON_USABLE_PRICE] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_COUPON_USABLE_PRICE];
			}
			set { this.DataSource[Constants.FIELD_COUPON_USABLE_PRICE] = value; }
		}

		/// <summary>クーポン併用フラグ</summary>
		public string UseTogetherFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_USE_TOGETHER_FLG]; }
			set { this.DataSource[Constants.FIELD_COUPON_USE_TOGETHER_FLG] = value; }
		}

		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_COUPON_VALID_FLG] = value; }
		}

		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_COUPON_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_COUPON_DATE_CREATED] = value; }
		}

		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_COUPON_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_COUPON_DATE_CHANGED] = value; }
		}

		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_COUPON_LAST_CHANGED] = value; }
		}

		/// <summary>フロント表示フラグ</summary>
		public string DispFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_DISP_FLG]; }
			set { this.DataSource[Constants.FIELD_COUPON_DISP_FLG] = value; }
		}

		/// <summary>クーポン説明文(ユーザ表示用)</summary>
		public string CouponDispDiscription
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_COUPON_DISP_DISCRIPTION]; }
			set { this.DataSource[Constants.FIELD_COUPON_COUPON_DISP_DISCRIPTION] = value; }
		}
		/// <summary>クーポン配送料無料フラグ</summary>
		public string FreeShippingFlg
		{
			get
			{
				if (this.DataSource[Constants.FIELD_COUPON_FREE_SHIPPING_FLG] == DBNull.Value) return Constants.FLG_COUPON_FREE_SHIPPING_INVALID;
				return (string)this.DataSource[Constants.FIELD_COUPON_FREE_SHIPPING_FLG];
			}
			set { this.DataSource[Constants.FIELD_COUPON_FREE_SHIPPING_FLG] = value; }
		}

		/// <summary>クーポン例外ブランドID</summary>
		public string ExceptionalBrandIds
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_BRAND_IDS]; }
			set { this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_BRAND_IDS] = value; }
		}

		/// <summary>クーポン例外商品カテゴリID</summary>
		public string ExceptionalProductCategoryIds
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_PRODUCT_CATEGORY_IDS]; }
			set { this.DataSource[Constants.FIELD_COUPON_EXCEPTIONAL_PRODUCT_CATEGORY_IDS] = value; }
		}
		#endregion
	}
}
