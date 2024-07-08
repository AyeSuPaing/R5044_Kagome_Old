/*
=========================================================================================================
  Module      : 定期購入情報モデル (FixedPurchaseModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using w2.Common.Util;
using w2.Domain.Coupon.Helper;
using w2.Domain.Payment;
using w2.Domain.TwFixedPurchaseInvoice;

namespace w2.Domain.FixedPurchase
{
	/// <summary>
	/// 定期購入情報モデル
	/// </summary>
	public partial class FixedPurchaseModel
	{
		/// <summary>次回購入利用ポイント数のデフォルト値</summary>
		public const string DEFAULT_NEXT_SHIPPING_USE_POINT = "0";
		/// <summary>次回購入利用クーポンIDのデフォルト値</summary>
		public const string DEFAULT_NEXT_SHIPPING_USE_COUPON_ID = "";
		/// <summary>次回購入利用クーポン番号のデフォルト値</summary>
		public const int DEFAULT_NEXT_SHIPPING_USE_COUPON_NO = 0;
		/// <summary>配送パターン 月間隔日付指定の設定数</summary>
		public const int FLG_FIXED_PURCHASE_SETTING_MONTHLY_DATE_COUNT = 2;
		/// <summary>配送パターン 月間隔・週・曜日指定の設定数</summary>
		public const int FLG_FIXED_PURCHASE_SETTING_MONTHLY_WEEKANDDAY_COUNT = 3;
		/// <summary>配送パターン 配送日間隔指定の設定数</summary>
		public const int FLG_FIXED_PURCHASE_SETTING_INTERVAL_BY_DAYS_COUNT = 1;
		/// <summary>配送パターン 週間隔・曜日指定の設定数</summary>
		public const int FLG_FIXED_PURCHASE_SETTING_WEEK_AND_DAY_COUNT = 2;

		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>配送先リスト</summary>
		public FixedPurchaseShippingModel[] Shippings
		{
			get { return (FixedPurchaseShippingModel[])this.DataSource["EX_Shippings"]; }
			set { this.DataSource["EX_Shippings"] = value; }
		}
		/// <summary>定期注文可能か</summary>
		public bool IsOrderRegister
		{
			get
			{
				// 定期購入ステータス：完了とキャンセル
				// 決済ステータス：決済失敗以外
				// 有効フラグ：有効
				return ((this.IsCompleteStatus == false)
						&& (this.IsCancelFixedPurchaseStatus == false)
						&& (this.PaymentStatus != Constants.FLG_FIXEDPURCHASE_PAYMENT_STATUS_ERROR)
						&& (this.IsValid)
						&& (this.OrderPaymentKbn != Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID));
			}
		}
		/// <summary>定期購入再開可能か</summary>
		public bool IsResumeOrder
		{
			get
			{
				// 定期購入ステータス：完了,キャンセル,仮登録,仮登録キャンセル
				// 有効フラグ：有効
				// 拡張ステータス36:拡張情報なし
				return ((this.IsCompleteStatus
						|| this.IsCancelFixedPurchaseStatus
						|| this.IsTemporaryRegistrationStatus
						|| this.IsCancelTemporaryRegistrationStatus
						|| this.IsSuspendFixedPurchaseStatus)
					&& this.IsValid
					&& (this.ExtendStatus36 == Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF));
			}
		}
		/// <summary>定期購入ステータスがキャンセルか</summary>
		public bool IsCancelFixedPurchaseStatus
		{
			get { return (this.FixedPurchaseStatus == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_CANCEL); }
		}
		/// <summary>定期購入ステータスがその他エラーか</summary>
		public bool IsOthersError
		{
			get { return (this.FixedPurchaseStatus == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_FAILED); }
		}
		/// <summary>定期購入ステータスが完了か</summary>
		public bool IsCompleteStatus
		{
			get { return (this.FixedPurchaseStatus == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_COMPLETE); }
		}
		/// <summary>定期購入ステータスが仮登録か</summary>
		public bool IsTemporaryRegistrationStatus
		{
			get { return (this.FixedPurchaseStatus == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_TEMP); }
		}
		/// <summary>定期購入ステータスが仮登録キャンセルか</summary>
		public bool IsCancelTemporaryRegistrationStatus
		{
			get { return (this.FixedPurchaseStatus == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_CANCEL_TEMP); }
		}
		/// <summary>定期購入ステータスが休止か</summary>
		public bool IsSuspendFixedPurchaseStatus
		{
			get { return (this.FixedPurchaseStatus == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_SUSPEND); }
		}
		/// <summary>クレジットカードを利用しているか?</summary>
		public bool UseUserCreditCard
		{
			get { return ((this.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT) && (this.CreditBranchNo != null)); }
		}
		/// <summary>
		/// 拡張項目_定期購入ステータステキスト
		/// </summary>
		public string FixedPurchaseStatusText
		{
			get
			{
				return ValueText.GetValueText(Constants.TABLE_FIXEDPURCHASE, Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_STATUS, this.FixedPurchaseStatus);
			}
		}
		/// <summary>
		/// 拡張項目_支払区分ステキスト
		/// </summary>
		public string OrderPaymentKbnText
		{
			get
			{
				if (m_orderPaymentKbnText == null)
				{
					var payment = new PaymentService().Get(this.ShopId, this.OrderPaymentKbn);
					m_orderPaymentKbnText = payment.PaymentName;
				}
				return m_orderPaymentKbnText;
			}
		}
		private string m_orderPaymentKbnText = null;
		/// <summary>
		/// 拡張項目_決済ステータステキスト
		/// </summary>
		public string PaymentStatusText
		{
			get
			{
				return ValueText.GetValueText(Constants.TABLE_FIXEDPURCHASE, Constants.FIELD_FIXEDPURCHASE_PAYMENT_STATUS, this.PaymentStatus);
			}
		}
		/// <summary>
		/// 拡張項目_注文区分テキスト
		/// </summary>
		public string OrderKbnText
		{
			get
			{
				return ValueText.GetValueText(Constants.TABLE_FIXEDPURCHASE, Constants.FIELD_FIXEDPURCHASE_ORDER_KBN, this.OrderKbn);
			}
		}
		/// <summary>
		/// 拡張項目_有効フラグテキスト
		/// </summary>
		public string ValidFlgText
		{
			get
			{
				return ValueText.GetValueText(Constants.TABLE_FIXEDPURCHASE, Constants.FIELD_FIXEDPURCHASE_VALID_FLG, this.ValidFlg);
			}
		}
		/// <summary>拡張項目_有効か</summary>
		public bool IsValid
		{
			get { return (this.ValidFlg == Constants.FLG_FIXEDPURCHASE_VALID_FLG_VALID); }
		}
		/// <summary>
		/// 拡張ステータス
		/// </summary>
		public ExtendStatusData ExtendStatus
		{
			get
			{
				// 既にデータが存在する場合、返却
				if (m_extendStatus != null) return m_extendStatus;
				m_extendStatus = new ExtendStatusData(this.DataSource);
				return m_extendStatus;
			}
		}
		private ExtendStatusData m_extendStatus = null;
		/// <summary>定期購入キャンセル期限（配送日の何日前までキャンセル可能か）</summary>
		public int? CancelDeadline
		{
			get { return (int?)this.DataSource[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_CANCEL_DEADLINE]; }
			set { this.DataSource[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_CANCEL_DEADLINE] = value; }
		}
		/// <summary>次回購入利用クーポン</summary>
		public UserCouponDetailInfo NextShippingUseCouponDetail
		{
			get { return (UserCouponDetailInfo)this.DataSource["EX_NextShippingUseCouponDetail"]; }
			set { this.DataSource["EX_NextShippingUseCouponDetail"] = value; }
		}
		/// <summary>Invoice</summary>
		public TwFixedPurchaseInvoiceModel Invoice
		{
			get { return (TwFixedPurchaseInvoiceModel)this.DataSource["EX_Invoice"]; }
			set { this.DataSource["EX_Invoice"] = value; }
		}
		/// <summary>配送種別ID※定期台帳から注文生成時に利用</summary>
		public string ShippingType
		{
			get { return StringUtility.ToEmpty(this.DataSource["shipping_type"]); }
			set { this.DataSource["shipping_type"] = value; }
		}
		/// <summary>GMO後払い_デバイス情報</summary>
		public string DeviceInfo { get; set; }
		/// <summary>頒布会定期台帳か</summary>
		public bool IsSubsctriptionBox
		{
			get { return (string.IsNullOrEmpty(this.SubscriptionBoxCourseId) == false); }
		}
		/// <summary>頒布会定額コースか</summary>
		public bool IsSubscriptionBoxFixedAmount
		{
			get { return ((this.SubscriptionBoxFixedAmount.HasValue) && (this.SubscriptionBoxFixedAmount.Value != 0)); }
		}
		/// <summary>全ポイント継続利用かどうか</summary>
		public bool IsUseAllPointFlg
		{
			get { return this.UseAllPointFlg == Constants.FLG_FIELD_FIXEDPURCHASE_USE_ALL_POINT_FLG_ON; }
		}
		#endregion

		#region 拡張ステータスデータクラス
		/// <summary>
		/// 拡張ステータスデータクラス
		/// </summary>
		[Serializable]
		public class ExtendStatusData
		{
			/// <summary>拡張ステータスデータ格納用</summary>
			private List<ExtendStatusDataInner> m_inners = new List<ExtendStatusDataInner>();

			#region コンストラクタ
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="source">ソース</param>
			public ExtendStatusData(Hashtable source)
			{
				this.DataSource = source;
			}
			#endregion

			#region インデクサ
			/// <summary>
			/// インデクサ
			/// </summary>
			/// <param name="index">インデックス</param>
			/// <returns>拡張ステータスデータインナー</returns>
			public ExtendStatusDataInner this[int index]
			{
				get
				{
					// 注文拡張ステータスNo範囲外の場合はエラー
					var extendStatusNo = index + 1;
					if (extendStatusNo < 1 || extendStatusNo > Constants.CONST_ORDER_EXTEND_STATUS_USE_MAX)
					{
						throw new ApplicationException(string.Format("拡張ステータスNo範囲外のためエラーが発生しました。（No.{0}）", extendStatusNo));
					}
					// 既にデータが存在する場合、返却
					var inner = m_inners.FirstOrDefault(i => i.ExtendStatusNo == extendStatusNo);
					if (inner != null) return inner;
					inner = new ExtendStatusDataInner(this.DataSource, extendStatusNo);
					m_inners.Add(inner);
					return inner;
				}
			}
			#endregion

			#region プロパティ
			/// <summary>ソース</summary>
			private Hashtable DataSource { get; set; }
			#endregion
		}
		#endregion

		#region 拡張ステータスデータインナークラス
		/// <summary>
		/// 拡張ステータスデータインナークラス
		/// </summary>
		[Serializable]
		public class ExtendStatusDataInner
		{
			#region コンストラクタ
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="source">ソース</param>
			/// <param name="extendStatusNo">注文拡張ステータスNo</param>
			public ExtendStatusDataInner(Hashtable source, int extendStatusNo)
			{
				this.DataSource = source;
				this.ExtendStatusNo = extendStatusNo;
			}
			#endregion

			#region プロパティ
			/// <summary>注文拡張ステータスNo</summary>
			public int ExtendStatusNo { get; set; }
			/// <summary>拡張ステータス値</summary>
			public string Value
			{
				get { return (string)this.DataSource[this.ValueKey]; }
				set { this.DataSource[this.ValueKey] = value; }
			}
			/// <summary>
			/// 拡張ステータス値がON?
			/// </summary>
			public bool IsOn
			{
				get { return (this.Value == Constants.FLG_ORDER_ORDER_EXTEND_STATUS_ON); }
			}
			/// <summary>拡張ステータス値キー</summary>
			private string ValueKey { get { return string.Format(Constants.FIELD_ORDER_EXTEND_STATUS_BASENAME + "{0}", this.ExtendStatusNo.ToString()); } }
			/// <summary>拡張ステータス更新日</summary>
			public DateTime ExtendStatusDate
			{
				get { return (DateTime)this.DataSource[this.DateKey]; }
				set { this.DataSource[this.DateKey] = value; }
			}
			private string DateKey { get { return string.Format(Constants.FIELD_ORDER_EXTEND_STATUS_DATE_BASENAME + "{0}", this.ExtendStatusNo.ToString()); } }
			/// <summary>ソース</summary>
			private Hashtable DataSource { get; set; }
			#endregion
		}
		#endregion
	}
}
