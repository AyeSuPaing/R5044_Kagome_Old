/*
=========================================================================================================
  Module      : 店舗配送種別マスタモデル (ShopShippingModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.ShopShipping
{
	/// <summary>
	/// 店舗配送種別マスタモデル
	/// </summary>
	[Serializable]
	public partial class ShopShippingModel : ModelBase<ShopShippingModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ShopShippingModel()
		{
			this.PaymentRelationId = "";
			this.ShippingDateSetFlg = Constants.FLG_SHOPSHIPPING_SHIPPING_DATE_SET_FLG_INVALID;
			this.DelFlg = "0";
			this.FixedPurchaseFlg = Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_FLG_VALID;
			this.FixedPurchaseKbn1Flg = Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN1_FLG_VALID;
			this.FixedPurchaseKbn1Setting = "";
			this.FixedPurchaseKbn2Flg = Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN2_FLG_INVALID;
			this.FixedPurchaseKbn2Setting = "";
			this.FixedPurchaseKbn3Flg = Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN3_FLG_INVALID;
			this.FixedPurchaseKbn3Setting = "";
			this.FixedPurchaseOrderEntryTiming = 0;
			this.FixedPurchaseCancelDeadline = 0;
			this.WrappingPaperFlg = Constants.FLG_SHOPSHIPPING_WRAPPING_PAPER_FLG_INVALID;
			this.WrappingBagFlg = Constants.FLG_SHOPSHIPPING_WRAPPING_BAG_FLG_INVALID;
			this.PaymentSelectionFlg = Constants.FLG_SHOPSHIPPING_PAYMENT_SELECTION_FLG_INVALID;
			this.PermittedPaymentIds = "";
			this.ShippingPriceSeparateEstimatesFlg = Constants.FLG_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG_INVALID;
			this.FixedPurchaseShippingDaysRequired = 0;
			this.FixedPurchaseMinimumShippingSpan = 1;
			this.BusinessDaysForShipping = 0;
			this.NextShippingMaxChangeDays = Constants.FLG_SHOPSHIPPING_NEXT_SHIPPING_MAX_CHANGE_DAYS_DEFAULT;
			this.FixedPurchaseKbn4Flg = Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_KBN4_FLG_INVALID;
			this.FixedPurchaseKbn4Setting1 = "";
			this.FixedPurchaseKbn4Setting2 = "";
			this.FixedPurchaseKbn1Setting2 = "";
			this.FixedPurchaseFirstShippingNextMonthFlg = Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_FIRST_SHIPPING_NEXT_MONTH_FLG_INVALID;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ShopShippingModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ShopShippingModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPING_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_SHOP_ID] = value; }
		}
		/// <summary>配送種別ID</summary>
		public string ShippingId
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPING_SHIPPING_ID]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_SHIPPING_ID] = value; }
		}
		/// <summary>配送拠点ID</summary>
		public string ShippingBaseId
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPING_SHIPPING_BASE_ID]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_SHIPPING_BASE_ID] = value; }
		}
		/// <summary>配送種別名</summary>
		public string ShopShippingName
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPING_SHOP_SHIPPING_NAME]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_SHOP_SHIPPING_NAME] = value; }
		}
		/// <summary>決済連携ID</summary>
		public string PaymentRelationId
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPING_PAYMENT_RELATION_ID]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_PAYMENT_RELATION_ID] = value; }
		}
		/// <summary>配送日設定可能フラグ</summary>
		public string ShippingDateSetFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPING_SHIPPING_DATE_SET_FLG]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_SHIPPING_DATE_SET_FLG] = value; }
		}
		/// <summary>配送日設定可能範囲(開始)</summary>
		public int? ShippingDateSetBegin
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SHOPSHIPPING_SHIPPING_DATE_SET_BEGIN] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_SHOPSHIPPING_SHIPPING_DATE_SET_BEGIN];
			}
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_SHIPPING_DATE_SET_BEGIN] = value; }
		}
		/// <summary>配送日設定可能範囲（終了）</summary>
		public int? ShippingDateSetEnd
		{
			get
			{
				if (this.DataSource[Constants.FIELD_SHOPSHIPPING_SHIPPING_DATE_SET_END] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_SHOPSHIPPING_SHIPPING_DATE_SET_END];
			}
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_SHIPPING_DATE_SET_END] = value; }
		}
		/// <summary>削除フラグ</summary>
		public string DelFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPING_DEL_FLG]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_DEL_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_SHOPSHIPPING_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_SHOPSHIPPING_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPING_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_LAST_CHANGED] = value; }
		}
		/// <summary>定期購入設定可能フラグ</summary>
		public string FixedPurchaseFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FLG]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FLG] = value; }
		}
		/// <summary>定期購入区分1設定可能フラグ</summary>
		public string FixedPurchaseKbn1Flg
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN1_FLG]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN1_FLG] = value; }
		}
		/// <summary>定期購入区分1設定値</summary>
		public string FixedPurchaseKbn1Setting
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN1_SETTING]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN1_SETTING] = value; }
		}
		/// <summary>定期購入区分2設定可能フラグ</summary>
		public string FixedPurchaseKbn2Flg
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN2_FLG]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN2_FLG] = value; }
		}
		/// <summary>定期購入区分2設定値</summary>
		public string FixedPurchaseKbn2Setting
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN2_SETTING]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN2_SETTING] = value; }
		}
		/// <summary>定期購入区分3設定可能フラグ</summary>
		public string FixedPurchaseKbn3Flg
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN3_FLG]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN3_FLG] = value; }
		}
		/// <summary>定期購入区分3設定値</summary>
		public string FixedPurchaseKbn3Setting
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN3_SETTING]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN3_SETTING] = value; }
		}
		/// <summary>定期購入区分4設定可能フラグ</summary>
		public string FixedPurchaseKbn4Flg
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN4_FLG]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN4_FLG] = value; }
		}
		/// <summary>定期購入区分4設定値1</summary>
		public string FixedPurchaseKbn4Setting1
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN4_SETTING1]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN4_SETTING1] = value; }
		}
		/// <summary>定期購入区分4設定値2</summary>
		public string FixedPurchaseKbn4Setting2
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN4_SETTING2]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN4_SETTING2] = value; }
		}
		/// <summary>定期購入受注タイミング</summary>
		public int FixedPurchaseOrderEntryTiming
		{
			get { return (int)this.DataSource[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_ORDER_ENTRY_TIMING]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_ORDER_ENTRY_TIMING] = value; }
		}
		/// <summary>定期購入キャンセル期限</summary>
		public int FixedPurchaseCancelDeadline
		{
			get { return (int)this.DataSource[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_CANCEL_DEADLINE]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_CANCEL_DEADLINE] = value; }
		}
		/// <summary>のし利用フラグ</summary>
		public string WrappingPaperFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPING_WRAPPING_PAPER_FLG]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_WRAPPING_PAPER_FLG] = value; }
		}
		/// <summary>のし種類</summary>
		public string WrappingPaperTypes
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPING_WRAPPING_PAPER_TYPES]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_WRAPPING_PAPER_TYPES] = value; }
		}
		/// <summary>包装利用フラグ</summary>
		public string WrappingBagFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPING_WRAPPING_BAG_FLG]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_WRAPPING_BAG_FLG] = value; }
		}
		/// <summary>包装種類</summary>
		public string WrappingBagTypes
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPING_WRAPPING_BAG_TYPES]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_WRAPPING_BAG_TYPES] = value; }
		}
		/// <summary>決済選択の任意利用フラグ</summary>
		public string PaymentSelectionFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPING_PAYMENT_SELECTION_FLG]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_PAYMENT_SELECTION_FLG] = value; }
		}
		/// <summary>決済選択の可能リスト</summary>
		public string PermittedPaymentIds
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPING_PERMITTED_PAYMENT_IDS]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_PERMITTED_PAYMENT_IDS] = value; }
		}
		/// <summary>配送料の別見積もりの利用フラグ</summary>
		public string ShippingPriceSeparateEstimatesFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG] = value; }
		}
		/// <summary>配送料の別見積もりの文言</summary>
		public string ShippingPriceSeparateEstimatesMessage
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_MESSAGE]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_MESSAGE] = value; }
		}
		/// <summary>配送料の別見積もりの文言モバイル</summary>
		public string ShippingPriceSeparateEstimatesMessageMobile
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_MESSAGE_MOBILE]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_MESSAGE_MOBILE] = value; }
		}
		/// <summary>定期購入配送所要日数</summary>
		public int FixedPurchaseShippingDaysRequired
		{
			get { return (int)this.DataSource[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SHIPPING_DAYS_REQUIRED]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SHIPPING_DAYS_REQUIRED] = value; }
		}
		/// <summary>定期購入最低配送間隔（所定日数）</summary>
		public int FixedPurchaseMinimumShippingSpan
		{
			get { return (int)this.DataSource[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_MINIMUM_SHIPPING_SPAN]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_MINIMUM_SHIPPING_SPAN] = value; }
		}
		/// <summary>定期配送料無料フラグ</summary>
		public string FixedPurchaseFreeShippingFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FREE_SHIPPING_FLG]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FREE_SHIPPING_FLG] = value; }
		}
		/// <summary>出荷所要営業日数</summary>
		public int BusinessDaysForShipping
		{
			get { return (int)this.DataSource[Constants.FIELD_SHOPSHIPPING_BUSINESS_DAYS_FOR_SHIPPING]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_BUSINESS_DAYS_FOR_SHIPPING] = value; }
		}
		/// <summary>次回配送日の選択可能最大日数</summary>
		public int NextShippingMaxChangeDays
		{
			get { return (int)this.DataSource[Constants.FIELD_SHOPSHIPPING_NEXT_SHIPPING_MAX_CHANGE_DAYS]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_NEXT_SHIPPING_MAX_CHANGE_DAYS] = value; }
		}
		/// <summary>フロント側の配送パターン非表示フラグ</summary>
		public string FixedPurchaseShippingNotDisplayFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SHIPPING_NOTDISPLAY_FLG]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SHIPPING_NOTDISPLAY_FLG] = value; }
		}
		/// <summary>定期購入区分1設定値2</summary>
		public string FixedPurchaseKbn1Setting2
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN1_SETTING2]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN1_SETTING2] = value; }
		}
		/// <summary>定期購入初回配送翌月フラグ</summary>
		public string FixedPurchaseFirstShippingNextMonthFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FIRST_SHIPPING_NEXT_MONTH_FLG]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FIRST_SHIPPING_NEXT_MONTH_FLG] = value; }
		}
		#endregion
	}
}
