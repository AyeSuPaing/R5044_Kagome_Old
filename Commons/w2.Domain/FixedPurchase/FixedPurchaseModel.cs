/*
=========================================================================================================
  Module      : 定期購入情報モデル (FixedPurchaseModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.FixedPurchase
{
	/// <summary>
	/// 定期購入情報モデル
	/// </summary>
	[Serializable]
	public partial class FixedPurchaseModel : ModelBase<FixedPurchaseModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public FixedPurchaseModel()
		{
			this.FixedPurchaseKbn = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE;
			this.BaseTelNo = "";
			this.FixedPurchaseStatus = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_NORMAL;
			this.PaymentStatus = Constants.FLG_FIXEDPURCHASE_PAYMENT_STATUS_NORMAL;
			this.LastOrderDate = null;
			this.OrderCount = 0;
			this.SupplierId = "";
			this.OrderKbn = Constants.FLG_FIXEDPURCHASE_ORDER_KBN_PC;
			this.OrderPaymentKbn = "00";
			this.CardKbn = "";
			this.ValidFlg = Constants.FLG_FIXEDPURCHASE_VALID_FLG_VALID;
			this.DelFlg = "0";
			this.CreditBranchNo = null;
			this.NextShippingDate = null;
			this.NextNextShippingDate = null;
			this.ExtendStatus1 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus2 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus3 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus4 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus5 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus6 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus7 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus8 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus9 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus10 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus11 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus12 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus13 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus14 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus15 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus16 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus17 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus18 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus19 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus20 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus21 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus22 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus23 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus24 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus25 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus26 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus27 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus28 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus29 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus30 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus31 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus32 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus33 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus34 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus35 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus36 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus37 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus38 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus39 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus40 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus41 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus42 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus43 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus44 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus45 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus46 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus47 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus48 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus49 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatus50 = Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF;
			this.ExtendStatusDate1 = null;
			this.ExtendStatusDate2 = null;
			this.ExtendStatusDate3 = null;
			this.ExtendStatusDate4 = null;
			this.ExtendStatusDate5 = null;
			this.ExtendStatusDate6 = null;
			this.ExtendStatusDate7 = null;
			this.ExtendStatusDate8 = null;
			this.ExtendStatusDate9 = null;
			this.ExtendStatusDate10 = null;
			this.ExtendStatusDate11 = null;
			this.ExtendStatusDate12 = null;
			this.ExtendStatusDate13 = null;
			this.ExtendStatusDate14 = null;
			this.ExtendStatusDate15 = null;
			this.ExtendStatusDate16 = null;
			this.ExtendStatusDate17 = null;
			this.ExtendStatusDate18 = null;
			this.ExtendStatusDate19 = null;
			this.ExtendStatusDate20 = null;
			this.ExtendStatusDate21 = null;
			this.ExtendStatusDate22 = null;
			this.ExtendStatusDate23 = null;
			this.ExtendStatusDate24 = null;
			this.ExtendStatusDate25 = null;
			this.ExtendStatusDate26 = null;
			this.ExtendStatusDate27 = null;
			this.ExtendStatusDate28 = null;
			this.ExtendStatusDate29 = null;
			this.ExtendStatusDate30 = null;
			this.ExtendStatusDate31 = null;
			this.ExtendStatusDate32 = null;
			this.ExtendStatusDate33 = null;
			this.ExtendStatusDate34 = null;
			this.ExtendStatusDate35 = null;
			this.ExtendStatusDate36 = null;
			this.ExtendStatusDate37 = null;
			this.ExtendStatusDate38 = null;
			this.ExtendStatusDate39 = null;
			this.ExtendStatusDate40 = null;
			this.ExtendStatusDate41 = null;
			this.ExtendStatusDate42 = null;
			this.ExtendStatusDate43 = null;
			this.ExtendStatusDate44 = null;
			this.ExtendStatusDate45 = null;
			this.ExtendStatusDate46 = null;
			this.ExtendStatusDate47 = null;
			this.ExtendStatusDate48 = null;
			this.ExtendStatusDate49 = null;
			this.ExtendStatusDate50 = null;
			this.ShippedCount = 0;
			this.CancelReasonId = "";
			this.CancelMemo = "";
			this.NextShippingUsePoint = 0;
			this.AccessCountryIsoCode = "";
			this.DispLanguageCode = "";
			this.DispLanguageLocaleId = "";
			this.DispCurrencyCode = "";
			this.DispCurrencyLocaleId = "";
			this.ExternalPaymentAgreementId = "";
			this.CancelDate = null;
			this.RestartDate = null;
			this.SuspendReason = "";
			this.ResumeDate = null;
			this.ShippingMemo = "";

			this.Shippings = new FixedPurchaseShippingModel[0];
			this.Memo = string.Empty;
			this.NextShippingUseCouponId = "";
			this.NextShippingUseCouponNo = 0;
			this.ReceiptFlg = Constants.FLG_ORDER_RECEIPT_FLG_OFF;
			this.ReceiptAddress = "";
			this.ReceiptProviso = "";
			this.SkippedCount = 0;
			this.Attribute1 = "";
			this.Attribute2 = "";
			this.Attribute3 = "";
			this.Attribute4 = "";
			this.Attribute5 = "";
			this.Attribute6 = "";
			this.Attribute7 = "";
			this.Attribute8 = "";
			this.Attribute9 = "";
			this.Attribute10 = "";
			this.SubscriptionBoxCourseId = string.Empty;
			this.SubscriptionBoxOrderCount = 0;
			this.SubscriptionBoxFixedAmount = null;
			this.UseAllPointFlg = Constants.FLG_FIELD_FIXEDPURCHASE_USE_ALL_POINT_FLG_OFF;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FixedPurchaseModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FixedPurchaseModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>定期購入ID</summary>
		[UpdateDataAttribute(1, "fixed_purchase_id")]
		public string FixedPurchaseId
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID] = value; }
		}
		/// <summary>定期購入区分</summary>
		[UpdateDataAttribute(2, "fixed_purchase_kbn")]
		public string FixedPurchaseKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_KBN]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_KBN] = value; }
		}
		/// <summary>定期購入設定１</summary>
		[UpdateDataAttribute(3, "fixed_purchase_setting1")]
		public string FixedPurchaseSetting1
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_SETTING1]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_SETTING1] = value; }
		}
		/// <summary>元電話番号</summary>
		[UpdateDataAttribute(4, "base_tel_no")]
		public string BaseTelNo
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_BASE_TEL_NO]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_BASE_TEL_NO] = value; }
		}
		/// <summary>定期購入ステータス</summary>
		[UpdateDataAttribute(5, "fixed_purchase_status")]
		public string FixedPurchaseStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_STATUS]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_STATUS] = value; }
		}
		/// <summary>決済ステータス</summary>
		[UpdateDataAttribute(6, "payment_status")]
		public string PaymentStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_PAYMENT_STATUS]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_PAYMENT_STATUS] = value; }
		}
		/// <summary>最終購入日</summary>
		[UpdateDataAttribute(7, "last_order_date")]
		public DateTime? LastOrderDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_LAST_ORDER_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_LAST_ORDER_DATE];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_LAST_ORDER_DATE] = value; }
		}
		/// <summary>購入回数(注文基準)</summary>
		[UpdateDataAttribute(8, "order_count")]
		public int OrderCount
		{
			get { return (int)this.DataSource[Constants.FIELD_FIXEDPURCHASE_ORDER_COUNT]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_ORDER_COUNT] = value; }
		}
		/// <summary>ユーザID</summary>
		[UpdateDataAttribute(9, "user_id")]
		public string UserId
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_USER_ID]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_USER_ID] = value; }
		}
		/// <summary>店舗ID</summary>
		[UpdateDataAttribute(10, "shop_id")]
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_SHOP_ID] = value; }
		}
		/// <summary>サプライヤID</summary>
		[UpdateDataAttribute(11, "supplier_id")]
		public string SupplierId
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_SUPPLIER_ID]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_SUPPLIER_ID] = value; }
		}
		/// <summary>注文区分</summary>
		[UpdateDataAttribute(12, "order_kbn")]
		public string OrderKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_ORDER_KBN]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_ORDER_KBN] = value; }
		}
		/// <summary>支払区分</summary>
		[UpdateDataAttribute(13, "order_payment_kbn")]
		public string OrderPaymentKbn
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_ORDER_PAYMENT_KBN] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_ORDER_PAYMENT_KBN];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_ORDER_PAYMENT_KBN] = value; }
		}
		/// <summary>定期購入開始日時</summary>
		[UpdateDataAttribute(14, "fixed_purchase_date_bgn")]
		public DateTime FixedPurchaseDateBgn
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_DATE_BGN]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_DATE_BGN] = value; }
		}
		/// <summary>決済カード区分</summary>
		[UpdateDataAttribute(15, "card_kbn")]
		public string CardKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_CARD_KBN]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_CARD_KBN] = value; }
		}
		/// <summary>有効フラグ</summary>
		[UpdateDataAttribute(16, "valid_flg")]
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_VALID_FLG] = value; }
		}
		/// <summary>削除フラグ</summary>
		[UpdateDataAttribute(17, "del_flg")]
		public string DelFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_DEL_FLG]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_DEL_FLG] = value; }
		}
		/// <summary>作成日</summary>
		[UpdateDataAttribute(18, "date_created")]
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_FIXEDPURCHASE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		[UpdateDataAttribute(19, "date_changed")]
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_FIXEDPURCHASE_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		[UpdateDataAttribute(20, "last_changed")]
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_LAST_CHANGED] = value; }
		}
		/// <summary>クレジットカード枝番</summary>
		[UpdateDataAttribute(21, "credit_branch_no")]
		public int? CreditBranchNo
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_CREDIT_BRANCH_NO] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_CREDIT_BRANCH_NO];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_CREDIT_BRANCH_NO] = value; }
		}
		/// <summary>次回配送日</summary>
		[UpdateDataAttribute(22, "next_shipping_date")]
		public DateTime? NextShippingDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_DATE];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_DATE] = value; }
		}
		/// <summary>次々回配送日</summary>
		[UpdateDataAttribute(23, "next_next_shipping_date")]
		public DateTime? NextNextShippingDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_NEXT_NEXT_SHIPPING_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_NEXT_NEXT_SHIPPING_DATE];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_NEXT_NEXT_SHIPPING_DATE] = value; }
		}
		/// <summary>定期購入管理メモ</summary>
		[UpdateDataAttribute(24, "fixed_purchase_management_memo")]
		public string FixedPurchaseManagementMemo
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_MANAGEMENT_MEMO]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_MANAGEMENT_MEMO] = value; }
		}
		/// <summary>カード支払い回数コード</summary>
		[UpdateDataAttribute(25, "card_installments_code")]
		public string CardInstallmentsCode
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_CARD_INSTALLMENTS_CODE]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_CARD_INSTALLMENTS_CODE] = value; }
		}
		/// <summary>拡張ステータス1</summary>
		[UpdateDataAttribute(26, "extend_status1")]
		public string ExtendStatus1
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS1]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS1] = value; }
		}
		/// <summary>拡張ステータス2</summary>
		[UpdateDataAttribute(27, "extend_status2")]
		public string ExtendStatus2
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS2]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS2] = value; }
		}
		/// <summary>拡張ステータス3</summary>
		[UpdateDataAttribute(28, "extend_status3")]
		public string ExtendStatus3
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS3]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS3] = value; }
		}
		/// <summary>拡張ステータス4</summary>
		[UpdateDataAttribute(29, "extend_status4")]
		public string ExtendStatus4
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS4]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS4] = value; }
		}
		/// <summary>拡張ステータス5</summary>
		[UpdateDataAttribute(30, "extend_status5")]
		public string ExtendStatus5
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS5]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS5] = value; }
		}
		/// <summary>拡張ステータス6</summary>
		[UpdateDataAttribute(31, "extend_status6")]
		public string ExtendStatus6
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS6]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS6] = value; }
		}
		/// <summary>拡張ステータス7</summary>
		[UpdateDataAttribute(32, "extend_status7")]
		public string ExtendStatus7
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS7]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS7] = value; }
		}
		/// <summary>拡張ステータス8</summary>
		[UpdateDataAttribute(33, "extend_status8")]
		public string ExtendStatus8
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS8]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS8] = value; }
		}
		/// <summary>拡張ステータス9</summary>
		[UpdateDataAttribute(34, "extend_status9")]
		public string ExtendStatus9
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS9]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS9] = value; }
		}
		/// <summary>拡張ステータス10</summary>
		[UpdateDataAttribute(35, "extend_status10")]
		public string ExtendStatus10
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS10]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS10] = value; }
		}
		/// <summary>拡張ステータス11</summary>
		[UpdateDataAttribute(36, "extend_status11")]
		public string ExtendStatus11
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS11]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS11] = value; }
		}
		/// <summary>拡張ステータス12</summary>
		[UpdateDataAttribute(37, "extend_status12")]
		public string ExtendStatus12
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS12]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS12] = value; }
		}
		/// <summary>拡張ステータス13</summary>
		[UpdateDataAttribute(38, "extend_status13")]
		public string ExtendStatus13
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS13]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS13] = value; }
		}
		/// <summary>拡張ステータス14</summary>
		[UpdateDataAttribute(39, "extend_status14")]
		public string ExtendStatus14
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS14]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS14] = value; }
		}
		/// <summary>拡張ステータス15</summary>
		[UpdateDataAttribute(40, "extend_status15")]
		public string ExtendStatus15
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS15]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS15] = value; }
		}
		/// <summary>拡張ステータス16</summary>
		[UpdateDataAttribute(41, "extend_status16")]
		public string ExtendStatus16
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS16]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS16] = value; }
		}
		/// <summary>拡張ステータス17</summary>
		[UpdateDataAttribute(42, "extend_status17")]
		public string ExtendStatus17
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS17]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS17] = value; }
		}
		/// <summary>拡張ステータス18</summary>
		[UpdateDataAttribute(43, "extend_status18")]
		public string ExtendStatus18
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS18]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS18] = value; }
		}
		/// <summary>拡張ステータス19</summary>
		[UpdateDataAttribute(44, "extend_status19")]
		public string ExtendStatus19
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS19]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS19] = value; }
		}
		/// <summary>拡張ステータス20</summary>
		[UpdateDataAttribute(45, "extend_status20")]
		public string ExtendStatus20
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS20]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS20] = value; }
		}
		/// <summary>拡張ステータス21</summary>
		[UpdateDataAttribute(46, "extend_status21")]
		public string ExtendStatus21
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS21]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS21] = value; }
		}
		/// <summary>拡張ステータス22</summary>
		[UpdateDataAttribute(47, "extend_status22")]
		public string ExtendStatus22
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS22]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS22] = value; }
		}
		/// <summary>拡張ステータス23</summary>
		[UpdateDataAttribute(48, "extend_status23")]
		public string ExtendStatus23
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS23]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS23] = value; }
		}
		/// <summary>拡張ステータス24</summary>
		[UpdateDataAttribute(49, "extend_status24")]
		public string ExtendStatus24
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS24]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS24] = value; }
		}
		/// <summary>拡張ステータス25</summary>
		[UpdateDataAttribute(50, "extend_status25")]
		public string ExtendStatus25
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS25]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS25] = value; }
		}
		/// <summary>拡張ステータス26</summary>
		[UpdateDataAttribute(51, "extend_status26")]
		public string ExtendStatus26
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS26]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS26] = value; }
		}
		/// <summary>拡張ステータス27</summary>
		[UpdateDataAttribute(52, "extend_status27")]
		public string ExtendStatus27
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS27]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS27] = value; }
		}
		/// <summary>拡張ステータス28</summary>
		[UpdateDataAttribute(53, "extend_status28")]
		public string ExtendStatus28
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS28]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS28] = value; }
		}
		/// <summary>拡張ステータス29</summary>
		[UpdateDataAttribute(54, "extend_status29")]
		public string ExtendStatus29
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS29]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS29] = value; }
		}
		/// <summary>拡張ステータス30</summary>
		[UpdateDataAttribute(55, "extend_status30")]
		public string ExtendStatus30
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS30]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS30] = value; }
		}
		/// <summary>購入回数(出荷基準)</summary>
		[UpdateDataAttribute(56, "shipped_count")]
		public int ShippedCount
		{
			get { return (int)this.DataSource[Constants.FIELD_FIXEDPURCHASE_SHIPPED_COUNT]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_SHIPPED_COUNT] = value; }
		}
		/// <summary>解約理由区分ID</summary>
		[UpdateDataAttribute(57, "cancel_reason_id")]
		public string CancelReasonId
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_CANCEL_REASON_ID]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_CANCEL_REASON_ID] = value; }
		}
		/// <summary>解約メモ</summary>
		[UpdateDataAttribute(58, "cancel_memo")]
		public string CancelMemo
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_CANCEL_MEMO]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_CANCEL_MEMO] = value; }
		}
		/// <summary>次回購入の利用ポイント数</summary>
		[UpdateDataAttribute(59, "next_shipping_use_point")]
		public decimal NextShippingUsePoint
		{
			get { return (decimal)this.DataSource[Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_USE_POINT]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_USE_POINT] = value; }
		}
		/// <summary>定期購入同梱元定期購入ID</summary>
		[UpdateDataAttribute(60, "combined_org_fixedpurchase_ids")]
		public string CombinedOrgFixedpurchaseIds
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_COMBINED_ORG_FIXEDPURCHASE_IDS]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_COMBINED_ORG_FIXEDPURCHASE_IDS] = value; }
		}
		/// <summary>拡張ステータス31</summary>
		[UpdateDataAttribute(61, "extend_status31")]
		public string ExtendStatus31
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS31]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS31] = value; }
		}
		/// <summary>拡張ステータス32</summary>
		[UpdateDataAttribute(62, "extend_status32")]
		public string ExtendStatus32
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS32]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS32] = value; }
		}
		/// <summary>拡張ステータス33</summary>
		[UpdateDataAttribute(63, "extend_status33")]
		public string ExtendStatus33
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS33]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS33] = value; }
		}
		/// <summary>拡張ステータス34</summary>
		[UpdateDataAttribute(64, "extend_status34")]
		public string ExtendStatus34
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS34]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS34] = value; }
		}
		/// <summary>拡張ステータス35</summary>
		[UpdateDataAttribute(65, "extend_status35")]
		public string ExtendStatus35
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS35]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS35] = value; }
		}
		/// <summary>拡張ステータス36</summary>
		[UpdateDataAttribute(66, "extend_status36")]
		public string ExtendStatus36
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS36]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS36] = value; }
		}
		/// <summary>拡張ステータス37</summary>
		[UpdateDataAttribute(67, "extend_status37")]
		public string ExtendStatus37
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS37]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS37] = value; }
		}
		/// <summary>拡張ステータス38</summary>
		[UpdateDataAttribute(68, "extend_status38")]
		public string ExtendStatus38
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS38]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS38] = value; }
		}
		/// <summary>拡張ステータス39</summary>
		[UpdateDataAttribute(69, "extend_status39")]
		public string ExtendStatus39
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS39]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS39] = value; }
		}
		/// <summary>拡張ステータス40</summary>
		[UpdateDataAttribute(70, "extend_status40")]
		public string ExtendStatus40
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS40]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS40] = value; }
		}
		/// <summary>アクセス国ISOコード</summary>
		[UpdateDataAttribute(71, "access_country_iso_code")]
		public string AccessCountryIsoCode
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_ACCESS_COUNTRY_ISO_CODE]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_ACCESS_COUNTRY_ISO_CODE] = value; }
		}
		/// <summary>表示言語コード</summary>
		[UpdateDataAttribute(72, "disp_language_code")]
		public string DispLanguageCode
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_DISP_LANGUAGE_CODE]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_DISP_LANGUAGE_CODE] = value; }
		}
		/// <summary>表示言語ロケールID</summary>
		[UpdateDataAttribute(73, "disp_language_locale_id")]
		public string DispLanguageLocaleId
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_DISP_LANGUAGE_LOCALE_ID]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_DISP_LANGUAGE_LOCALE_ID] = value; }
		}
		/// <summary>表示通貨コード</summary>
		[UpdateDataAttribute(74, "disp_currency_code")]
		public string DispCurrencyCode
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_DISP_CURRENCY_CODE]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_DISP_CURRENCY_CODE] = value; }
		}
		/// <summary>表示通貨ロケールID</summary>
		[UpdateDataAttribute(75, "disp_currency_locale_id")]
		public string DispCurrencyLocaleId
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_DISP_CURRENCY_LOCALE_ID]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_DISP_CURRENCY_LOCALE_ID] = value; }
		}
		/// <summary>外部支払契約ID</summary>
		[UpdateDataAttribute(76, "external_payment_agreement_id")]
		public string ExternalPaymentAgreementId
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTERNAL_PAYMENT_AGREEMENT_ID]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTERNAL_PAYMENT_AGREEMENT_ID] = value; }
		}
		/// <summary>定期再開予定日</summary>
		[UpdateDataAttribute(77, "resume_date")]
		public DateTime? ResumeDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_RESUME_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_RESUME_DATE];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_RESUME_DATE] = value; }
		}
		/// <summary>休止理由</summary>
		[UpdateDataAttribute(78, "suspend_reason")]
		public string SuspendReason
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_SUSPEND_REASON]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_SUSPEND_REASON] = value; }
		}
		/// <summary>配送メモ</summary>
		[UpdateDataAttribute(79, "shipping_memo")]
		public string ShippingMemo
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_SHIPPING_MEMO]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_SHIPPING_MEMO] = value; }
		}
		/// <summary>拡張ステータス更新日1</summary>
		[UpdateDataAttribute(80, "extend_status_date1")]
		public DateTime? ExtendStatusDate1
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE1] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE1];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE1] = value; }
		}
		/// <summary>拡張ステータス更新日2</summary>
		[UpdateDataAttribute(81, "extend_status_date2")]
		public DateTime? ExtendStatusDate2
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE2] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE2];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE2] = value; }
		}
		/// <summary>拡張ステータス更新日3</summary>
		[UpdateDataAttribute(82, "extend_status_date3")]
		public DateTime? ExtendStatusDate3
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE3] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE3];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE3] = value; }
		}
		/// <summary>拡張ステータス更新日4</summary>
		[UpdateDataAttribute(83, "extend_status_date4")]
		public DateTime? ExtendStatusDate4
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE4] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE4];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE4] = value; }
		}
		/// <summary>拡張ステータス更新日5</summary>
		[UpdateDataAttribute(84, "extend_status_date5")]
		public DateTime? ExtendStatusDate5
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE5] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE5];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE5] = value; }
		}
		/// <summary>拡張ステータス更新日6</summary>
		[UpdateDataAttribute(85, "extend_status_date6")]
		public DateTime? ExtendStatusDate6
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE6] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE6];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE6] = value; }
		}
		/// <summary>拡張ステータス更新日7</summary>
		[UpdateDataAttribute(86, "extend_status_date7")]
		public DateTime? ExtendStatusDate7
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE7] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE7];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE7] = value; }
		}
		/// <summary>拡張ステータス更新日8</summary>
		[UpdateDataAttribute(87, "extend_status_date8")]
		public DateTime? ExtendStatusDate8
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE8] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE8];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE8] = value; }
		}
		/// <summary>拡張ステータス更新日9</summary>
		[UpdateDataAttribute(88, "extend_status_date9")]
		public DateTime? ExtendStatusDate9
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE9] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE9];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE9] = value; }
		}
		/// <summary>拡張ステータス更新日10</summary>
		[UpdateDataAttribute(89, "extend_status_date10")]
		public DateTime? ExtendStatusDate10
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE10] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE10];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE10] = value; }
		}
		/// <summary>拡張ステータス更新日11</summary>
		[UpdateDataAttribute(90, "extend_status_date11")]
		public DateTime? ExtendStatusDate11
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE11] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE11];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE11] = value; }
		}
		/// <summary>拡張ステータス更新日12</summary>
		[UpdateDataAttribute(91, "extend_status_date12")]
		public DateTime? ExtendStatusDate12
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE12] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE12];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE12] = value; }
		}
		/// <summary>拡張ステータス更新日13</summary>
		[UpdateDataAttribute(92, "extend_status_date13")]
		public DateTime? ExtendStatusDate13
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE13] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE13];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE13] = value; }
		}
		/// <summary>拡張ステータス更新日14</summary>
		[UpdateDataAttribute(93, "extend_status_date14")]
		public DateTime? ExtendStatusDate14
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE14] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE14];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE14] = value; }
		}
		/// <summary>拡張ステータス更新日15</summary>
		[UpdateDataAttribute(94, "extend_status_date15")]
		public DateTime? ExtendStatusDate15
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE15] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE15];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE15] = value; }
		}
		/// <summary>拡張ステータス更新日16</summary>
		[UpdateDataAttribute(95, "extend_status_date16")]
		public DateTime? ExtendStatusDate16
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE16] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE16];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE16] = value; }
		}
		/// <summary>拡張ステータス更新日17</summary>
		[UpdateDataAttribute(96, "extend_status_date17")]
		public DateTime? ExtendStatusDate17
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE17] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE17];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE17] = value; }
		}
		/// <summary>拡張ステータス更新日18</summary>
		[UpdateDataAttribute(97, "extend_status_date18")]
		public DateTime? ExtendStatusDate18
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE18] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE18];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE18] = value; }
		}
		/// <summary>拡張ステータス更新日19</summary>
		[UpdateDataAttribute(98, "extend_status_date19")]
		public DateTime? ExtendStatusDate19
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE19] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE19];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE19] = value; }
		}
		/// <summary>拡張ステータス更新日20</summary>
		[UpdateDataAttribute(99, "extend_status_date20")]
		public DateTime? ExtendStatusDate20
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE20] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE20];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE20] = value; }
		}
		/// <summary>拡張ステータス更新日21</summary>
		[UpdateDataAttribute(100, "extend_status_date21")]
		public DateTime? ExtendStatusDate21
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE21] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE21];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE21] = value; }
		}
		/// <summary>拡張ステータス更新日22</summary>
		[UpdateDataAttribute(101, "extend_status_date22")]
		public DateTime? ExtendStatusDate22
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE22] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE22];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE22] = value; }
		}
		/// <summary>拡張ステータス更新日23</summary>
		[UpdateDataAttribute(102, "extend_status_date23")]
		public DateTime? ExtendStatusDate23
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE23] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE23];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE23] = value; }
		}
		/// <summary>拡張ステータス更新日24</summary>
		[UpdateDataAttribute(103, "extend_status_date24")]
		public DateTime? ExtendStatusDate24
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE24] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE24];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE24] = value; }
		}
		/// <summary>拡張ステータス更新日25</summary>
		[UpdateDataAttribute(104, "extend_status_date25")]
		public DateTime? ExtendStatusDate25
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE25] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE25];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE25] = value; }
		}
		/// <summary>拡張ステータス更新日26</summary>
		[UpdateDataAttribute(105, "extend_status_date26")]
		public DateTime? ExtendStatusDate26
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE26] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE26];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE26] = value; }
		}
		/// <summary>拡張ステータス更新日27</summary>
		[UpdateDataAttribute(106, "extend_status_date27")]
		public DateTime? ExtendStatusDate27
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE27] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE27];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE27] = value; }
		}
		/// <summary>拡張ステータス更新日28</summary>
		[UpdateDataAttribute(107, "extend_status_date28")]
		public DateTime? ExtendStatusDate28
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE28] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE28];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE28] = value; }
		}
		/// <summary>拡張ステータス更新日29</summary>
		[UpdateDataAttribute(108, "extend_status_date29")]
		public DateTime? ExtendStatusDate29
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE29] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE29];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE29] = value; }
		}
		/// <summary>拡張ステータス更新日30</summary>
		[UpdateDataAttribute(109, "extend_status_date30")]
		public DateTime? ExtendStatusDate30
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE30] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE30];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE30] = value; }
		}
		/// <summary>拡張ステータス更新日31</summary>
		[UpdateDataAttribute(110, "extend_status_date31")]
		public DateTime? ExtendStatusDate31
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE31] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE31];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE31] = value; }
		}
		/// <summary>拡張ステータス更新日32</summary>
		[UpdateDataAttribute(111, "extend_status_date32")]
		public DateTime? ExtendStatusDate32
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE32] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE32];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE32] = value; }
		}
		/// <summary>拡張ステータス更新日33</summary>
		[UpdateDataAttribute(112, "extend_status_date33")]
		public DateTime? ExtendStatusDate33
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE33] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE33];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE33] = value; }
		}
		/// <summary>拡張ステータス更新日34</summary>
		[UpdateDataAttribute(113, "extend_status_date34")]
		public DateTime? ExtendStatusDate34
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE34] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE34];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE34] = value; }
		}
		/// <summary>拡張ステータス更新日35</summary>
		[UpdateDataAttribute(114, "extend_status_date35")]
		public DateTime? ExtendStatusDate35
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE35] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE35];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE35] = value; }
		}
		/// <summary>拡張ステータス更新日36</summary>
		[UpdateDataAttribute(115, "extend_status_date36")]
		public DateTime? ExtendStatusDate36
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE36] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE36];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE36] = value; }
		}
		/// <summary>拡張ステータス更新日37</summary>
		[UpdateDataAttribute(116, "extend_status_date37")]
		public DateTime? ExtendStatusDate37
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE37] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE37];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE37] = value; }
		}
		/// <summary>拡張ステータス更新日38</summary>
		[UpdateDataAttribute(117, "extend_status_date38")]
		public DateTime? ExtendStatusDate38
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE38] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE38];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE38] = value; }
		}
		/// <summary>拡張ステータス更新日39</summary>
		[UpdateDataAttribute(118, "extend_status_date39")]
		public DateTime? ExtendStatusDate39
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE39] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE39];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE39] = value; }
		}
		/// <summary>拡張ステータス更新日40</summary>
		[UpdateDataAttribute(119, "extend_status_date40")]
		public DateTime? ExtendStatusDate40
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE40] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE40];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE40] = value; }
		}
		/// <summary>解約日</summary>
		[UpdateDataAttribute(120, "cancel_date")]
		public DateTime? CancelDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_CANCEL_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_CANCEL_DATE];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_CANCEL_DATE] = value; }
		}
		/// <summary>再開日</summary>
		[UpdateDataAttribute(121, "restart_date")]
		public DateTime? RestartDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_RESTART_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_RESTART_DATE];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_RESTART_DATE] = value; }
		}
		/// <summary>メモ</summary>
		[UpdateDataAttribute(122, "memo")]
		public string Memo
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_MEMO]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_MEMO] = value; }
		}
		/// <summary>次回購入の利用クーポンID</summary>
		[UpdateDataAttribute(123, "next_shipping_use_coupon_id")]
		public string NextShippingUseCouponId
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_USE_COUPON_ID]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_USE_COUPON_ID] = value; }
		}
		/// <summary>次回購入の利用クーポン枝番</summary>
		[UpdateDataAttribute(124, "next_shipping_use_coupon_no")]
		public int NextShippingUseCouponNo
		{
			get { return (int)this.DataSource[Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_USE_COUPON_NO]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_USE_COUPON_NO] = value; }
		}
		/// <summary>領収書希望フラグ</summary>
		[UpdateDataAttribute(125, "receipt_flg")]
		public string ReceiptFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_RECEIPT_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDER_RECEIPT_FLG] = value; }
		}
		/// <summary>宛名</summary>
		[UpdateDataAttribute(126, "receipt_address")]
		public string ReceiptAddress
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_RECEIPT_ADDRESS]; }
			set { this.DataSource[Constants.FIELD_ORDER_RECEIPT_ADDRESS] = value; }
		}
		/// <summary>但し書き</summary>
		[UpdateDataAttribute(127, "receipt_proviso")]
		public string ReceiptProviso
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDER_RECEIPT_PROVISO]; }
			set { this.DataSource[Constants.FIELD_ORDER_RECEIPT_PROVISO] = value; }
		}
		/// <summary>スキップ回数</summary>
		[UpdateDataAttribute(128, "skipped_count")]
		public int SkippedCount
		{
			get { return (int)this.DataSource[Constants.FIELD_FIXEDPURCHASE_SKIPPED_COUNT]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_SKIPPED_COUNT] = value; }
		}
		/// <summary>拡張ステータス41</summary>
		[UpdateDataAttribute(129, "extend_status41")]
		public string ExtendStatus41
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS41]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS41] = value; }
		}
		/// <summary>拡張ステータス42</summary>
		[UpdateDataAttribute(130, "extend_status42")]
		public string ExtendStatus42
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS42]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS42] = value; }
		}
		/// <summary>拡張ステータス43</summary>
		[UpdateDataAttribute(131, "extend_status43")]
		public string ExtendStatus43
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS43]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS43] = value; }
		}
		/// <summary>拡張ステータス44</summary>
		[UpdateDataAttribute(132, "extend_status44")]
		public string ExtendStatus44
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS44]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS44] = value; }
		}
		/// <summary>拡張ステータス45</summary>
		[UpdateDataAttribute(133, "extend_status45")]
		public string ExtendStatus45
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS45]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS45] = value; }
		}
		/// <summary>拡張ステータス46</summary>
		[UpdateDataAttribute(134, "extend_status46")]
		public string ExtendStatus46
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS46]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS46] = value; }
		}
		/// <summary>拡張ステータス47</summary>
		[UpdateDataAttribute(135, "extend_status47")]
		public string ExtendStatus47
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS47]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS47] = value; }
		}
		/// <summary>拡張ステータス48</summary>
		[UpdateDataAttribute(136, "extend_status48")]
		public string ExtendStatus48
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS48]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS48] = value; }
		}
		/// <summary>拡張ステータス49</summary>
		[UpdateDataAttribute(137, "extend_status49")]
		public string ExtendStatus49
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS49]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS49] = value; }
		}
		/// <summary>拡張ステータス50</summary>
		[UpdateDataAttribute(138, "extend_status50")]
		public string ExtendStatus50
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS50]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS50] = value; }
		}
		/// <summary>拡張ステータス更新日41</summary>
		[UpdateDataAttribute(139, "extend_status_date41")]
		public DateTime? ExtendStatusDate41
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE41] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE41];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE41] = value; }
		}
		/// <summary>拡張ステータス更新日42</summary>
		[UpdateDataAttribute(140, "extend_status_date42")]
		public DateTime? ExtendStatusDate42
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE42] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE42];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE42] = value; }
		}
		/// <summary>拡張ステータス更新日43</summary>
		[UpdateDataAttribute(141, "extend_status_date43")]
		public DateTime? ExtendStatusDate43
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE43] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE43];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE43] = value; }
		}
		/// <summary>拡張ステータス更新日44</summary>
		[UpdateDataAttribute(142, "extend_status_date44")]
		public DateTime? ExtendStatusDate44
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE44] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE44];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE44] = value; }
		}
		/// <summary>拡張ステータス更新日45</summary>
		[UpdateDataAttribute(143, "extend_status_date45")]
		public DateTime? ExtendStatusDate45
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE45] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE45];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE45] = value; }
		}
		/// <summary>拡張ステータス更新日46</summary>
		[UpdateDataAttribute(144, "extend_status_date46")]
		public DateTime? ExtendStatusDate46
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE46] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE46];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE46] = value; }
		}
		/// <summary>拡張ステータス更新日47</summary>
		[UpdateDataAttribute(145, "extend_status_date47")]
		public DateTime? ExtendStatusDate47
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE47] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE47];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE47] = value; }
		}
		/// <summary>拡張ステータス更新日48</summary>
		[UpdateDataAttribute(146, "extend_status_date48")]
		public DateTime? ExtendStatusDate48
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE48] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE48];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE48] = value; }
		}
		/// <summary>拡張ステータス更新日49</summary>
		[UpdateDataAttribute(147, "extend_status_date49")]
		public DateTime? ExtendStatusDate49
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE49] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE49];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE49] = value; }
		}
		/// <summary>拡張ステータス更新日50</summary>
		[UpdateDataAttribute(148, "extend_status_date50")]
		public DateTime? ExtendStatusDate50
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE50] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE50];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_DATE50] = value; }
		}
		/// <summary>注文拡張項目1</summary>
		[UpdateDataAttribute(149, "attribute1")]
		public string Attribute1
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_ATTRIBUTE1]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_ATTRIBUTE1] = value; }
		}
		/// <summary>注文拡張項目2</summary>
		[UpdateDataAttribute(150, "attribute2")]
		public string Attribute2
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_ATTRIBUTE2]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_ATTRIBUTE2] = value; }
		}
		/// <summary>注文拡張項目3</summary>
		[UpdateDataAttribute(151, "attribute3")]
		public string Attribute3
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_ATTRIBUTE3]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_ATTRIBUTE3] = value; }
		}
		/// <summary>注文拡張項目4</summary>
		[UpdateDataAttribute(152, "attribute4")]
		public string Attribute4
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_ATTRIBUTE4]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_ATTRIBUTE4] = value; }
		}
		/// <summary>注文拡張項目5</summary>
		[UpdateDataAttribute(153, "attribute5")]
		public string Attribute5
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_ATTRIBUTE5]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_ATTRIBUTE5] = value; }
		}
		/// <summary>注文拡張項目6</summary>
		[UpdateDataAttribute(154, "attribute6")]
		public string Attribute6
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_ATTRIBUTE6]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_ATTRIBUTE6] = value; }
		}
		/// <summary>注文拡張項目7</summary>
		[UpdateDataAttribute(155, "attribute7")]
		public string Attribute7
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_ATTRIBUTE7]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_ATTRIBUTE7] = value; }
		}
		/// <summary>注文拡張項目8</summary>
		[UpdateDataAttribute(156, "attribute8")]
		public string Attribute8
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_ATTRIBUTE8]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_ATTRIBUTE8] = value; }
		}
		/// <summary>注文拡張項目9</summary>
		[UpdateDataAttribute(157, "attribute9")]
		public string Attribute9
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_ATTRIBUTE9]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_ATTRIBUTE9] = value; }
		}
		/// <summary>注文拡張項目10</summary>
		[UpdateDataAttribute(158, "attribute10")]
		public string Attribute10
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_ATTRIBUTE10]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_ATTRIBUTE10] = value; }
		}
		/// <summary>Subscription Box Course Id</summary>
		[UpdateDataAttribute(159, "subscription_box_course_id")]
		public string SubscriptionBoxCourseId
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_SUBSCRIPTION_BOX_COURSE_ID]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_SUBSCRIPTION_BOX_COURSE_ID] = value; }
		}
		/// <summary>Subscription Box Course Order Count</summary>
		[UpdateDataAttribute(160, "subscription_box_order_count")]
		public int SubscriptionBoxOrderCount
		{
			get { return (int)this.DataSource[Constants.FIELD_FIXEDPURCHASE_SUBSCRIPTION_BOX_ORDER_COUNT]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_SUBSCRIPTION_BOX_ORDER_COUNT] = value; }
		}
		/// <summary>頒布会コース定額価格</summary>
		[UpdateDataAttribute(161, "subscription_box_fixed_amount")]
		public decimal? SubscriptionBoxFixedAmount
		{
			get
			{
				if (this.DataSource[Constants.FIELD_FIXEDPURCHASE_SUBSCRIPTION_BOX_FIXED_AMOUNT] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_FIXEDPURCHASE_SUBSCRIPTION_BOX_FIXED_AMOUNT];
			}
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_SUBSCRIPTION_BOX_FIXED_AMOUNT] = value; }
		}
		/// <summary>次回購入の利用ポイントの全適用フラグ</summary>
		[UpdateDataAttribute(162, "use_all_point_flg")]
		public string UseAllPointFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASE_USE_ALL_POINT_FLG]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASE_USE_ALL_POINT_FLG] = value; }
		}
		#endregion
	}
}