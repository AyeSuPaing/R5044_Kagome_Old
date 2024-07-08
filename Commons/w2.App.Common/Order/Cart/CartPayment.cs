/*
=========================================================================================================
  Module      : カート決済情報クラス(CartPayment.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.DataCacheController;
using w2.App.Common.Global.Translation;
using w2.App.Common.Order.Payment.Paygent;
using w2.App.Common.Order.Payment.YamatoKwc.Helper;
using w2.App.Common.Order.Payment.Zeus;
using w2.Common.Util;

namespace w2.App.Common.Order
{
	///*********************************************************************************************
	/// <summary>
	/// カート決済情報クラス
	/// </summary>
	///*********************************************************************************************
	[Serializable]
	public partial class CartPayment
	{
		// ■下記フィールドはモバイルページのinputタグ名にも対応してます■

		// 決済種別名(注文確認画面表示用)
		public const string FIELD_PAYMENT_NAME = "payment_name";
		// クレジット会社(コード) w2_Order.card_kbnを利用
		public const string FIELD_CREDIT_COMPANY = "credit_company";
		// クレジットカード枝番
		public const string FIELD_CREDIT_CARD_BRANCH_NO = "credit_card_branch_no";
		// クレジットカード番号
		public const string FIELD_CREDIT_CARD_NO = "credit_card_no";
		// クレジットカート番号1
		public const string FIELD_CREDIT_CARD_NO_1 = "credit_card_no1";
		// クレジットカート番号2
		public const string FIELD_CREDIT_CARD_NO_2 = "credit_card_no2";
		// クレジットカート番号3
		public const string FIELD_CREDIT_CARD_NO_3 = "credit_card_no3";
		// クレジットカート番号4
		public const string FIELD_CREDIT_CARD_NO_4 = "credit_card_no4";
		// 有効期限(月)
		public const string FIELD_CREDIT_EXPIRE_MONTH = "credit_expire_month";
		// 有効期限(年)
		public const string FIELD_CREDIT_EXPIRE_YEAR = "credit_expire_year";
		// 支払回数（コード）
		public const string FIELD_CREDIT_INSTALLMENTS_CODE = "credit_installments_code";
		// セキュリティコード
		public const string FIELD_CREDIT_SECURITY_CODE = "credit_security_code";
		// カード名義人
		public const string FIELD_CREDIT_AUTHOR_NAME = "credit_author_name";
		// クレジットトークンhidden値
		public const string FIELD_CREDIT_TOKEN_HIDDEN_VALUE = "credit_token_hidden";
		// クレジットcvvトークンhidden値
		public const string FIELD_CREDIT_CVV_TOKEN_HIDDEN_VALUE = "credit_cvv_token_hidden";
		// クレジットカード登録フラグ
		public const string FIELD_CREDIT_CARD_REGIST_FLG = "credit_card_regist_flg";
		// 登録クレジットカード名
		public const string FIELD_REGIST_CREDIT_CARD_NAME = "regist_credit_card_name";
		/// <summary>Credit bincode</summary>
		public const string FIELD_CREDIT_BINCODE = "credit_bincode";
		// セキュリティコード
		public const string FIELD_CREDIT_RAKUTEN_CVV_TOKEN = "rakuten_cvv_token";
		// クレジットカード連携ID
		public const string FIELD_CREDIT_COOPERATION_ID = "cooperation_id";
		// 新規クレジットカード枝番
		public const string FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW = "NEW";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CartPayment()
		{
			this.CreditCardBranchNo = FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW;
			this.IsSamePaymentAsCart1 = true;
			this.LanguageCode = null;
			this.LanguageLocaleId = null;
			this.IsBackFromConfirm = false;
			this.HasUpdateDefaultCard = false;
		}

		/// <summary>
		/// 決済情報更新
		/// </summary>
		/// <param name="paymentId">決済種別ID</param>
		/// <param name="paymentName">決済種別名</param>
		/// <param name="creditCardBranchNo">クレジットカード枝番</param>
		/// <param name="creditCardCompanyCode">クレジットカード会社コード</param>
		/// <param name="creditCardNo1">クレジットカードNO1</param>
		/// <param name="creditCardNo2">クレジットカードNO2</param>
		/// <param name="creditCardNo3">クレジットカードNO3</param>
		/// <param name="creditCardNo4">クレジットカードNO4</param>
		/// <param name="creditExpireMonth">クレジットカード有効期限(月)</param>
		/// <param name="creditExpireYear">クレジットカード有効期限(年)</param>
		/// <param name="creditInstallmentsCode">クレジットカード支払い回数</param>
		/// <param name="creditSecurityCode">クレジットカードセキュリティコード</param>
		/// <param name="creditAuthorName">クレジットカード名義人</param>
		/// <param name="paymentObject">決済オブジェクト</param>
		/// <param name="isSamePaymentAsCart1">カート1と同じ決済か</param>
		/// <param name="rakutenCvvToken">楽天クレカ連携用CvvToken</param>
		/// <param name="newebPayCreditInstallmentsCode">NewebPay Credit Installments Code</param>
		/// <param name="creditBincode">Credit bincode</param>
		/// <param name="rakutenCvsType">楽天前払い支払いコンビニ</param>
		/// <param name="cooperationId">クレジットカード連携ID</param>
		public void UpdateCartPayment(
			string paymentId,
			string paymentName,
			string creditCardBranchNo,
			string creditCardCompanyCode,
			string creditCardNo1,
			string creditCardNo2,
			string creditCardNo3,
			string creditCardNo4,
			string creditExpireMonth,
			string creditExpireYear,
			string creditInstallmentsCode,
			string creditSecurityCode,
			string creditAuthorName,
			IPayment paymentObject,
			bool isSamePaymentAsCart1,
			string rakutenCvvToken,
			string newebPayCreditInstallmentsCode = "",
			string creditBincode = null,
			string rakutenCvsType = null,
			string cooperationId = "")
		{
			this.PaymentId = paymentId;
			this.PaymentName = paymentName;
			this.CreditCardBranchNo = (StringUtility.ToEmpty(creditCardBranchNo) == "") ? FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW : creditCardBranchNo;
			this.CreditCardCompany = creditCardCompanyCode;
			this.CreditCardNo1 = creditCardNo1;
			this.CreditCardNo2 = creditCardNo2;
			this.CreditCardNo3 = creditCardNo3;
			this.CreditCardNo4 = creditCardNo4;
			this.CreditExpireMonth = creditExpireMonth;
			this.CreditExpireYear = creditExpireYear;
			this.CreditInstallmentsCode = creditInstallmentsCode;
			this.CreditSecurityCode = creditSecurityCode;
			this.CreditAuthorName = creditAuthorName;
			// 決済手数料は確認画面にて取得
			// m_dPriceExchange = (decimal)htPayment[Constants.FIELD_ORDER_ORDER_PRICE_EXCHANGE];
			// 決済オブジェクト
			this.PaymentObject = paymentObject;
			// 決済選択表示
			this.IsSamePaymentAsCart1 = isSamePaymentAsCart1;

			this.UserCreditCard = null;
			this.UserCreditCardRegistFlg = false;
			this.UserCreditCardRegistable = false;

			this.NewebPayCreditInstallmentsCode = newebPayCreditInstallmentsCode;

			this.CreditBincode = creditBincode;
			this.RakutenCvvToken = rakutenCvvToken;
			if (string.IsNullOrEmpty(rakutenCvsType) == false)
			{
				this.RakutenCvsType = rakutenCvsType;
			}
			this.CooperationId = cooperationId;
		}

		/// <summary>
		/// ユーザクレジットカード保存設定更新
		/// </summary>
		/// <param name="blUserCreditCardRegistFlg">クレジットカード保持フラグ</param>
		/// <param name="strUserCreditCardName">クレジットカード名（保存用）</param>
		public void UpdateUserCreditCardRegistSetting(
			bool? blUserCreditCardRegistFlg,
			string strUserCreditCardName)
		{
			// ユーザクレジットカード保存フラグ
			this.UserCreditCardRegistFlg = (blUserCreditCardRegistFlg.HasValue) ? blUserCreditCardRegistFlg.Value : false;

			// ユーザクレジットカード名称
			this.UserCreditCardName = StringUtility.ToEmpty(strUserCreditCardName);
		}

		/// <summary>
		/// クレジットカード情報初期化
		/// </summary>
		public void ClearCreditCardInfo()
		{
			this.CreditCardBranchNo = FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW;
			this.CreditCardCompany = null;
			this.CreditCardNo1 = null;
			this.CreditCardNo2 = null;
			this.CreditCardNo3 = null;
			this.CreditCardNo4 = null;
			this.CreditExpireYear = null;
			this.CreditExpireMonth = null;
			this.CreditInstallmentsCode = null;
			this.CreditAuthorName = null;
			this.CreditSecurityCode = null;
			this.CreditToken = null;
		}

		/// <summary>
		/// カート決済情報オブジェクト複製
		/// </summary>
		/// <returns>複製したカート決済情報オブジェクト</returns>
		public CartPayment Clone()
		{
			var clone = (CartPayment)MemberwiseClone();
			return clone;
		}

		/// <summary>
		/// 決済種別名翻訳名取得
		/// </summary>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>決済種別名翻訳名</returns>
		/// <remarks>バッチなど、フロント以外から翻訳名称取得するときに使用</remarks>
		public string GetPaymentTranslationName(string languageCode, string languageLocaleId)
		{
			if (Constants.GLOBAL_OPTION_ENABLE == false) return m_paymentName;

			this.LanguageCode = languageCode;
			this.LanguageLocaleId = languageLocaleId;

			return GetPaymentNameTranslationName();
		}
		/// <summary>
		/// 決済種別名翻訳名取得
		/// </summary>
		/// <returns>決済種別名翻訳名</returns>
		private string GetPaymentNameTranslationName()
		{
			var beforeTranslationPaymentName = DataCacheControllerFacade.GetPaymentCacheController()
				.GetPaymentName(this.PaymentId);
			var paymentNameTranslationName = NameTranslationCommon.GetTranslationName(
				this.PaymentId,
				Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PAYMENT,
				Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PAYMENT_PAYMENT_NAME,
				beforeTranslationPaymentName,
				this.LanguageCode,
				this.LanguageLocaleId);

			return paymentNameTranslationName;
		}
		/// <summary>
		/// 決済種別名翻訳名取得
		/// </summary>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>決済種別名翻訳名</returns>
		/// <remarks>バッチなど、フロント以外から翻訳名称取得するときに使用</remarks>
		public string GetPaymentNameTranslationName(string languageCode, string languageLocaleId)
		{
			var beforeTranslationPaymentName = DataCacheControllerFacade.GetPaymentCacheController()
				.GetPaymentName(this.PaymentId);
			var paymentNameTranslationName = NameTranslationCommon.GetTranslationName(
				this.PaymentId,
				Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PAYMENT,
				Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PAYMENT_PAYMENT_NAME,
				beforeTranslationPaymentName,
				languageCode,
				languageLocaleId);

			return paymentNameTranslationName;
		}

		/// <summary>
		/// Get Zeus cvs type
		/// </summary>
		/// <returns>Zeus cvs type</returns>
		public string GetZeusCvsType()
		{
			if ((this.PaymentObject != null)
				&& (this.PaymentObject is PaymentZeusCvs))
			{
				return ((PaymentZeusCvs)this.PaymentObject).ConveniType;
			}

			return null;
		}

		/// <summary>
		/// トークンがあるかどうか
		/// </summary>
		/// <returns>トークンがあるかどうか</returns>
		public bool HasAnyToken()
		{
			var result = ((this.CreditToken != null)
				&& (string.IsNullOrEmpty(this.CreditToken.Token) == false));
			return result;
		}

		/// <summary>
		/// Get Paygent cvs type
		/// </summary>
		/// <returns>Paygent cvs type</returns>
		public string GetPaygentCvsType()
		{
			if ((this.PaymentObject != null)
				&& (this.PaymentObject is PaymentPaygentCvs payment))
			{
				return payment.ConveniType;
			}

			return null;
		}

		/// <summary>決済種別ID</summary>
		public string PaymentId { get; set; }
		/// <summary>決済種別名(注文確認画面表示用)</summary>
		public string PaymentName
		{
			get
			{
				if (Constants.GLOBAL_OPTION_ENABLE == false) return m_paymentName;
				return GetPaymentNameTranslationName();
			}
			set { m_paymentName = value; }
		}
		private string m_paymentName;
		/// <summary>クレジットカード枝番</summary>
		public string CreditCardBranchNo { get; set; }
		/// <summary>決済カード会社コード</summary>
		public string CreditCardCompany { get; set; }
		/// <summary>決済カード会社名</summary>
		public string CreditCardCompanyName
		{
			get { return OrderCommon.GetCreditCardCompanyName(this.CreditCardCompany); }
		}
		/// <summary>クレジットカード番号</summary>
		public string CreditCardNo { get { return this.CreditCardNo1 + this.CreditCardNo2 + this.CreditCardNo3 + this.CreditCardNo4; } }
		/// <summary>クレジットカード番号1</summary>
		public string CreditCardNo1 { get; set; }
		/// <summary>クレジットカード番号2</summary>
		public string CreditCardNo2 { get; set; }
		/// <summary>クレジットカード番号3</summary>
		public string CreditCardNo3 { get; set; }
		/// <summary>クレジットカード番号4</summary>
		public string CreditCardNo4 { get; set; }
		/// <summary>有効期限(月)</summary>
		public string CreditExpireMonth { get; set; }
		/// <summary>有効期限(年)</summary>
		public string CreditExpireYear { get; set; }
		/// <summary>支払回数(コード)</summary>
		public string CreditInstallmentsCode { get; set; }
		/// <summary>セキュリティコード</summary>
		public string CreditSecurityCode { get; set; }
		/// <summary>カード名義人</summary>
		public string CreditAuthorName { get; set; }
		/// <summary>決済手数料</summary>
		public decimal PriceExchange { get; set; }
		/// <summary>決済オブジェクト</summary>
		public IPayment PaymentObject { get; set; }
		/// <summary>お支払い方法選択画面の決済選択</summary>
		public bool IsSamePaymentAsCart1 { get; set; }
		/// <summary>クレジットカード登録可能フラグ</summary>
		public bool UserCreditCardRegistable { get; set; }
		/// <summary>クレジットカード保存フラグ</summary>
		public bool UserCreditCardRegistFlg { get; set; }
		/// <summary>クレジットカード名称（保存用）</summary>
		public string UserCreditCardName { get; set; }
		/// <summary>クレジットカード登録名補完フラグ</summary>
		public bool UserCreditNameComplementFlg { get; set; }
		/// <summary>ユーザークレジットカード（カード保存後、生成される）</summary>
		public UserCreditCard UserCreditCard { get; set; }
		/// <summary>SBPS WEBコンビニ区分</summary>
		public PaymentSBPSTypes.WebCvsTypes SBPSWebCvsType { get; set; }
		/// <summary>ヤマトKWC コンビニタイプ（決済区分）</summary>
		public YamatoKwcFunctionDivCvs YamatoKwcCvsType { get; set; }
		/// <summary>GMO convenience store type (settlement category)</summary>
		public string GmoCvsType { get; set; }
		/// <summary>Rakuten convenience store type</summary>
		public string RakutenCvsType { get; set; }
		/// <summary>クレジットトークン</summary>
		public CreditTokenInfoBase CreditToken { get; set; }
		/// <summary>クレジットトークン（カート1と同じ）</summary>
		public CreditTokenInfoBase CreditTokenSameAs1 { get; set; }
		/// <summary>クレジット払い？</summary>
		public bool IsCredit
		{
			get { return (this.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT); }
		}
		/// <summary>ヤマト後払いSMS認証連携？</summary>
		public bool IsPaymentYamatoKaSms
		{
			get { return OrderCommon.CheckPaymentYamatoKaSms(this.PaymentId); }
		}
		/// <summary>言語コード</summary>
		public string LanguageCode { get; set; }
		/// <summary>言語ロケールID</summary>
		public string LanguageLocaleId { get; set; }
		/// <summary>Paidyトークン</summary>
		public string PaidyToken { get; set; }
		/// <summary>Is Rejected Payment</summary>
		public bool IsRejectedPayment { get; set; }
		/// <summary>Card Tran Id</summary>
		public string CardTranId { get; set; }
		/// <summary>External Payment Type</summary>
		public string ExternalPaymentType { get; set; }
		/// <summary>Is Payment Ec Pay With Credit Installment</summary>
		public bool IsPaymentEcPayWithCreditInstallment { get; set; }
		/// <summary>Is back from confirm</summary>
		public bool IsBackFromConfirm { get; set; }
		/// <summary>Has update default card</summary>
		public bool HasUpdateDefaultCard { get; set; }
		/// <summary>藍新Pay支払回数(コード)</summary>
		public string NewebPayCreditInstallmentsCode { get; set; }
		/// <summary>Credit bincode</summary>
		public string CreditBincode { get; set; }
		/// <summary>楽天クレカ連携用CvvToken</summary>
		public string RakutenCvvToken { get; set; }
		/// <summary>Is payment ec pay</summary>
		public bool IsPaymentEcPay
		{
			get { return (this.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY); }
		}
		/// <summary>Is payment neweb pay</summary>
		public bool IsPaymentNewebPay
		{
			get { return (this.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY); }
		}
		/// <summary>Is payment cvs pre</summary>
		public bool IsPaymentCvsPre
		{
			get { return (this.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE); }
		}
		/// <summary>Is payment cvs def</summary>
		public bool IsPaymentCvsDef
		{
			get { return (this.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF); }
		}
		/// <summary>Is not payment amazon</summary>
		public bool IsNotPaymentAmazon
		{
			get { return (this.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT); }
		}
		/// <summary>Has credit card company</summary>
		public bool HasCreditCardCompany
		{
			get
			{
				var result = (string.IsNullOrEmpty(StringUtility.ToEmpty(this.CreditCardCompany)) == false);
				return result;
			}
		}
		/// <summary>Has credit card no</summary>
		public bool HasCreditCardNo
		{
			get
			{
				var result = (string.IsNullOrEmpty(StringUtility.ToEmpty(this.CreditCardNo)) == false);
				return result;
			}
		}
		/// <summary>クレジットカード連携ID</summary>
		public string CooperationId { get; set; } = string.Empty;
		#region 決済方法確認
		/// <summary>ヤマトKWCクレジットでの決済か</summary>
		public bool IsYamatoKwcCredit
		{
			get
			{
				var result = (this.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
					&& (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.YamatoKwc);
				return result;
			}
		}
		/// <summary>ヤマトKWCクレジット3Dセキュア2.0での決済か</summary>
		public bool IsYamatoKwcCredit3dSecure2
		{
			get
			{
				var result = this.IsYamatoKwcCredit && Constants.PAYMENT_SETTING_YAMATO_KWC_3DSECURE;
				return result;
			}
		}
		#endregion
		/// <summary>支払い方法がpaidy(Direct)か</summary>
		public bool IsPaymentDirectPaidy
		{
			get
			{
				return ((Constants.PAYMENT_PAIDY_KBN == Constants.PaymentPaidyKbn.Direct)
					&& (this.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY));
			}
		}
		/// <summary>支払い方法がpaidy(Paygent)か</summary>
		public bool IsPaymentPaygentPaidy
		{
			get
			{
				return ((Constants.PAYMENT_PAIDY_KBN == Constants.PaymentPaidyKbn.Paygent)
					&& (this.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY));
			}
		}
	}
}
