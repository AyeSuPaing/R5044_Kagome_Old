/*
=========================================================================================================
  Module      : 商品同梱テーブルモデル (ProductBundleModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.ProductBundle
{
	/// <summary>
	/// 商品同梱テーブルモデル
	/// </summary>
	[Serializable]
	public partial class ProductBundleModel : ModelBase<ProductBundleModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ProductBundleModel()
		{
			this.TargetOrderType = Constants.FLG_PRODUCTBUNDLE_TARGET_ORDER_TYPE_ALL;
			this.EndDatetime = null;
			this.TargetProductKbn = Constants.FLG_PRODUCTBUNDLE_TARGET_PRODUCT_KBN_ALL;
			this.TargetOrderFixedPurchaseCountFrom = Constants.FLG_PRODUCTBUNDLE_TARGET_ORDER_FIXED_PURCHASE_COUNT_FROM_DEFAULT;
			this.TargetOrderFixedPurchaseCountTo = Constants.FLG_PRODUCTBUNDLE_TARGET_ORDER_FIXED_PURCHASE_COUNT_TO_DEFAULT;
			this.UsableTimesKbn = Constants.FLG_PRODUCTBUNDLE_USABLE_TIMES_KBN_NOLIMIT;
			this.ApplyType = Constants.FLG_PRODUCTBUNDLE_APPLY_TYPE_FOR_ORDER;
			this.ValidFlg = Constants.FLG_PRODUCTBUNDLE_VALID_FLG_VALID;
			this.MultipleApplyFlg = Constants.FLG_PRODUCTBUNDLE_MULTIPLE_APPLY_FLG_INVALID;
			this.ApplyOrder = Constants.FLG_PRODUCTBUNDLE_APPLY_ORDER_DEFAULT;
			this.UsableTimes = null;
			this.TargetProductCategoryIds = string.Empty;
			this.ExceptProductIds = string.Empty;
			this.ExceptProductCategoryIds = string.Empty;
			this.TargetId = string.Empty;
			this.TargetIdExceptFlg = Constants.FLG_PRODUCTBUNDLE_TARGET_ID_EXCEPT_FLG_TARGET;
			this.TargetOrderPriceSubtotalMin = null;
			this.TargetProductCountMin = null;
			this.TargetAdvCodesFirst = string.Empty;
			this.TargetAdvCodesNew = string.Empty;
			this.OrderedCount = 0;
			this.TargetPaymentIds = string.Empty;
			this.TargetCouponCodes = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductBundleModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductBundleModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>商品同梱ID</summary>
		public string ProductBundleId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_PRODUCT_BUNDLE_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_PRODUCT_BUNDLE_ID] = value; }
		}
		/// <summary>商品同梱名</summary>
		public string ProductBundleName
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_PRODUCT_BUNDLE_NAME]; }
			set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_PRODUCT_BUNDLE_NAME] = value; }
		}
		/// <summary>対象注文種別</summary>
		public string TargetOrderType
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_ORDER_TYPE]; }
			set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_ORDER_TYPE] = value; }
		}
		/// <summary>説明文</summary>
		public string Description
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_DESCRIPTION]; }
			set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_DESCRIPTION] = value; }
		}
		/// <summary>開始日時</summary>
		public DateTime StartDatetime
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_START_DATETIME]; }
			set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_START_DATETIME] = value; }
		}
		/// <summary>終了日時</summary>
		public DateTime? EndDatetime
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTBUNDLE_END_DATETIME] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_END_DATETIME];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_END_DATETIME] = value; }
		}
		/// <summary>対象商品指定方法</summary>
		public string TargetProductKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_PRODUCT_KBN]; }
			set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_PRODUCT_KBN] = value; }
		}
		/// <summary>対象商品ID</summary>
		public string TargetProductIds
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_PRODUCT_IDS]; }
			set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_PRODUCT_IDS] = value; }
		}
		/// <summary>対象定期注文回数_FROM</summary>
		public int? TargetOrderFixedPurchaseCountFrom
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_ORDER_FIXED_PURCHASE_COUNT_FROM] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_ORDER_FIXED_PURCHASE_COUNT_FROM];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_ORDER_FIXED_PURCHASE_COUNT_FROM] = value; }
		}
		/// <summary>対象定期注文回数_TO</summary>
		public int? TargetOrderFixedPurchaseCountTo
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_ORDER_FIXED_PURCHASE_COUNT_TO] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_ORDER_FIXED_PURCHASE_COUNT_TO];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_ORDER_FIXED_PURCHASE_COUNT_TO] = value; }
		}
		/// <summary>ユーザ利用可能回数</summary>
		public string UsableTimesKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_USABLE_TIMES_KBN]; }
			set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_USABLE_TIMES_KBN] = value; }
		}
		/// <summary>商品同梱設定適用種別</summary>
		public string ApplyType
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_APPLY_TYPE]; }
			set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_APPLY_TYPE] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_VALID_FLG] = value; }
		}
		/// <summary>重複適用フラグ</summary>
		public string MultipleApplyFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_MULTIPLE_APPLY_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_MULTIPLE_APPLY_FLG] = value; }
		}
		/// <summary>適用優先順</summary>
		public int ApplyOrder
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_APPLY_ORDER]; }
			set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_APPLY_ORDER] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_LAST_CHANGED] = value; }
		}
		/// <summary>ユーザ利用可能回数</summary>
		public int? UsableTimes
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTBUNDLE_USABLE_TIMES] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_USABLE_TIMES];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_USABLE_TIMES] = value; }
		}
		/// <summary>対象商品カテゴリID</summary>
		public string TargetProductCategoryIds
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_PRODUCT_CATEGORY_IDS]; }
			set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_PRODUCT_CATEGORY_IDS] = value; }
		}
		/// <summary>対象外商品ID</summary>
		public string ExceptProductIds
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_EXCEPT_PRODUCT_IDS]; }
			set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_EXCEPT_PRODUCT_IDS] = value; }
		}
		/// <summary>対象外商品カテゴリID</summary>
		public string ExceptProductCategoryIds
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_EXCEPT_PRODUCT_CATEGORY_IDS]; }
			set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_EXCEPT_PRODUCT_CATEGORY_IDS] = value; }
		}
		/// <summary>ターゲットリストID</summary>
		public string TargetId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_ID] = value; }
		}
		/// <summary>ターゲットリストID除外フラグ</summary>
		public string TargetIdExceptFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_ID_EXCEPT_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_ID_EXCEPT_FLG] = value; }
		}
		/// <summary>対象注文商品合計金額適用下限</summary>
		public decimal? TargetOrderPriceSubtotalMin
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_ORDER_PRICE_SUBTOTAL_MIN] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_ORDER_PRICE_SUBTOTAL_MIN];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_ORDER_PRICE_SUBTOTAL_MIN] = value; }
		}
		/// <summary>対象商品個数適用下限</summary>
		public int? TargetProductCountMin
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_PRODUCT_COUNT_MIN] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_PRODUCT_COUNT_MIN];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_PRODUCT_COUNT_MIN] = value; }
		}
		/// <summary>初回広告コード</summary>
		public string TargetAdvCodesFirst
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_ADVCODES_FIRST]; }
			set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_ADVCODES_FIRST] = value; }
		}
		/// <summary>最新広告コード</summary>
		public string TargetAdvCodesNew
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_ADVCODES_NEW]; }
			set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_ADVCODES_NEW] = value; }
		}
		/// <summary>決済種別</summary>
		public string TargetPaymentIds
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_PAYMENT_IDS]; }
			set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_PAYMENT_IDS] = value; }
		}
		/// <summary>クーポンコード</summary>
		public string TargetCouponCodes
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_COUPON_CODES]; }
			set { this.DataSource[Constants.FIELD_PRODUCTBUNDLE_TARGET_COUPON_CODES] = value; }
		}
		#endregion
	}
}
