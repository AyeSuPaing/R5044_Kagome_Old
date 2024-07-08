/*
=========================================================================================================
  Module      : 決済種別マスタモデル (PaymentModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Util;

namespace w2.Domain.Payment
{
	/// <summary>
	/// 決済種別マスタモデル
	/// </summary>
	public partial class PaymentModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>価格リスト</summary>
		public PaymentPriceModel[] PriceList
		{
			get { return (PaymentPriceModel[])this.DataSource["EX_PriceList"]; }
			set { this.DataSource["EX_PriceList"] = value; }
		}
		/// <summary>
		/// 拡張項目_決済手数料区分テキスト
		/// </summary>
		public string PaymentPriceKbnText
		{
			get
			{
				return ValueText.GetValueText(Constants.TABLE_PAYMENT, Constants.FIELD_PAYMENT_PAYMENT_PRICE_KBN, this.PaymentPriceKbn);
			}
		}
		/// <summary>
		/// 拡張項目_モバイル表示フラグテキスト
		/// </summary>
		public string MobileDispFlgText
		{
			get
			{
				return ValueText.GetValueText(Constants.TABLE_PAYMENT, Constants.FIELD_PAYMENT_MOBILE_DISP_FLG, this.MobileDispFlg);
			}
		}
		/// <summary>
		/// 拡張項目_有効フラグフラグテキスト
		/// </summary>
		public string ValidFlgText
		{
			get
			{
				return ValueText.GetValueText(Constants.TABLE_PAYMENT, Constants.FIELD_PAYMENT_VALID_FLG, this.ValidFlg);
			}
		}
		/// <summary>拡張項目_有効か</summary>
		public bool IsValid
		{
			get { return (this.ValidFlg == Constants.FLG_PAYMENT_VALID_FLG_VALID); }
		}
		/// <summary>拡張項目_定期購入で有効か</summary>
		public bool IsValidFixedPurchase
		{
			// 有効 AND (クレジットカード OR 外部連携がない決済種別)
			get
			{
				return (this.IsValid
					&& ((this.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
						|| (this.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT)
						|| (this.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_BANK_PRE)
						|| (this.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_BANK_DEF)
						|| (this.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_POST_PRE)
						|| (this.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_POST_DEF)
						|| (this.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
						|| (this.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
						|| (this.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
						|| (this.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
						|| (this.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_DSK_DEF)));
			}
		}
		/// <summary>拡張項目_既定のお支払方法で有効か</summary>
		public bool IsValidUserDefaultPayment
		{
			get
			{
				if (this.IsValid == false) return false;

				if ((this.MobileDispFlg != Constants.FLG_PAYMENT_MOBILE_DISP_FLG_BOTH_PC_AND_MOBILE)
					&& (this.MobileDispFlg != Constants.FLG_PAYMENT_MOBILE_DISP_FLG_PC))
				{
					// PC表示でなければ無効
					return false;
				}

				if ((this.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT)
					|| (this.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT)
					|| (this.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2))
				{
					// 決済なし OR Amazon Payであれば無効
					return false;
				}

				return true;
			}
		}
		/// <summary>支払区分がAmazonPay(CV2)か？</summary>
		public bool IsPaymentIdAmazonPayCv2
		{
			get
			{
				return (this.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2);
			}
		}
		#endregion
	}
}
