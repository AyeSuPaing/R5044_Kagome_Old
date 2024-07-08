/*
=========================================================================================================
  Module      : 注文フロー（注文カート表示）プロセス(OrderFlowProcess_OrderPageDisp.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Linq;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment.Zeus;
using w2.Domain.Payment;

/// <summary>
/// OrderLpInputs の概要の説明です
/// </summary>
public partial class OrderFlowProcess
{
	#region 配送先表示関連

	/// <summary>
	/// 注文者へ配送するか取得
	/// </summary>
	/// <param name="csShipping">配送先情報</param>
	/// <returns>チェック値</returns>
	public bool GetShipToOwner(CartShipping csShipping)
	{
		return (csShipping.ShippingAddrKbn == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER);
	}

	/// <summary>
	/// 注文者へ配送するか取得
	/// </summary>
	/// <param name="csShipping">配送先情報</param>
	/// <returns>チェック値</returns>
	public bool GetShipToNew(CartShipping csShipping)
	{
		return (csShipping.ShippingAddrKbn == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW);
	}

	/// <summary>
	/// 注文者から配送するか取得
	/// </summary>
	/// <param name="csShipping">配送先情報</param>
	/// <returns>チェック値</returns>
	public bool GetSendFromOwner(CartShipping csShipping)
	{
		return (csShipping.SenderAddrKbn == CartShipping.AddrKbn.Owner);
	}

	/// <summary>
	/// 配送先情報取得（新規入力の場合のみ値を返す）
	/// </summary>
	/// <param name="coCartObject">カート情報</param>
	/// <param name="strFieldName">フィールド名</param>
	/// <returns>配送先氏名</returns>
	public string GetShippingValue(CartObject coCartObject, string strFieldName)
	{
		return GetShippingValue(coCartObject.Shippings[0], strFieldName);
	}
	/// <summary>
	/// 配送先情報取得（新規入力の場合のみ値を返す）
	/// </summary>
	/// <param name="csShipping">配送先情報</param>
	/// <param name="strFieldName">フィールド名</param>
	/// <returns>配送先氏名</returns>
	public string GetShippingValue(CartShipping csShipping, string strFieldName)
	{
		// 新規入力の場合
		if (csShipping.ShippingAddrKbn == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW)
		{
			switch (strFieldName)
			{
				case Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1:
					return csShipping.Name1;

				case Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME2:
					return csShipping.Name2;

				case Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME:
					return csShipping.Name1 + csShipping.Name2;

				case Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA:
					return csShipping.NameKana1 + csShipping.NameKana2;

				case Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA1:
					return csShipping.NameKana1;

				case Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA2:
					return csShipping.NameKana2;

				case Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_NAME:
					return csShipping.ShippingCountryName;

				case Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP:
					return csShipping.Zip;

				case CartShipping.FIELD_ORDERSHIPPING_SHIPPING_ZIP_1:
					return csShipping.Zip1;

				case CartShipping.FIELD_ORDERSHIPPING_SHIPPING_ZIP_2:
					return csShipping.Zip2;

				case Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1:
					return csShipping.Addr1;

				case Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR2:
					return csShipping.Addr2;

				case Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR3:
					return csShipping.Addr3;

				case Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4:
					return csShipping.Addr4;

				case Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR5:
					return csShipping.Addr5;

				case CartShipping.FIELD_ORDERSHIPPING_SHIPPING_ADDR5_US:
					return IsCountryUs(csShipping.ShippingCountryIsoCode)
						? csShipping.Addr5
						: Constants.US_STATES_LIST[0];

				case Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_NAME:
					return csShipping.CompanyName;

				case Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_POST_NAME:
					return csShipping.CompanyPostName;

				case Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1:
					return csShipping.Tel1;

				case CartShipping.FIELD_ORDERSHIPPING_SHIPPING_TEL1_1:
					return csShipping.Tel1_1;

				case CartShipping.FIELD_ORDERSHIPPING_SHIPPING_TEL1_2:
					return csShipping.Tel1_2;

				case CartShipping.FIELD_ORDERSHIPPING_SHIPPING_TEL1_3:
					return csShipping.Tel1_3;

				case Constants.FIELD_USERSHIPPING_SHIPPING_COUNTRY_ISO_CODE:
					return csShipping.ShippingCountryIsoCode;
			}
		}
		else if (csShipping.ShippingAddrKbn == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE)
		{
			switch (strFieldName)
			{
				case Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1:
					return csShipping.Name1;

				case Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME2:
					return csShipping.Name2;

				case Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME:
					return csShipping.Name1 + csShipping.Name2;

				case Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA:
					return csShipping.NameKana1 + csShipping.NameKana2;

				case Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA1:
					return csShipping.NameKana1;

				case Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA2:
					return csShipping.NameKana2;

				case Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_NAME:
					return csShipping.ShippingCountryName;

				case Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP:
					return csShipping.Zip;

				case CartShipping.FIELD_ORDERSHIPPING_SHIPPING_ZIP_1:
					return csShipping.Zip1;

				case CartShipping.FIELD_ORDERSHIPPING_SHIPPING_ZIP_2:
					return csShipping.Zip2;

				case Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1:
					return csShipping.Addr1;

				case Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR2:
					return csShipping.Addr2;

				case Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR3:
					return csShipping.Addr3;

				case Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4:
					return csShipping.Addr4;

				case Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR5:
					return csShipping.Addr5;

				case CartShipping.FIELD_ORDERSHIPPING_SHIPPING_ADDR5_US:
					return IsCountryUs(csShipping.ShippingCountryIsoCode)
						? csShipping.Addr5
						: Constants.US_STATES_LIST[0];

				case Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_NAME:
					return csShipping.CompanyName;

				case Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_POST_NAME:
					return csShipping.CompanyPostName;

				case Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1:
					return csShipping.Tel1;

				case CartShipping.FIELD_ORDERSHIPPING_SHIPPING_TEL1_1:
					return csShipping.Tel1_1;

				case CartShipping.FIELD_ORDERSHIPPING_SHIPPING_TEL1_2:
					return csShipping.Tel1_2;

				case CartShipping.FIELD_ORDERSHIPPING_SHIPPING_TEL1_3:
					return csShipping.Tel1_3;

				case Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_ID:
					return csShipping.ConvenienceStoreId;

				case Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG:
					return csShipping.ConvenienceStoreFlg;
			}
		}

		return null;
	}

	/// <summary>
	/// 送り主情報取得（新規入力の場合のみ値を返す）
	/// </summary>
	/// <param name="csShipping">配送先情報</param>
	/// <param name="strFieldName">フィールド名</param>
	/// <returns>配送先氏名</returns>
	public string GetSenderValue(CartShipping csShipping, string strFieldName)
	{
		switch (strFieldName)
		{
			case Constants.FIELD_ORDERSHIPPING_SENDER_NAME1:
				return csShipping.SenderName1;

			case Constants.FIELD_ORDERSHIPPING_SENDER_NAME2:
				return csShipping.SenderName2;

			case Constants.FIELD_ORDERSHIPPING_SENDER_NAME:
				return csShipping.SenderName1 + csShipping.SenderName2;

			case Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA:
				return csShipping.SenderNameKana1 + csShipping.SenderNameKana2;

			case Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA1:
				return csShipping.SenderNameKana1;

			case Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA2:
				return csShipping.SenderNameKana2;

			case Constants.FIELD_ORDERSHIPPING_SENDER_COUNTRY_NAME:
				return csShipping.SenderCountryName;

			case Constants.FIELD_ORDERSHIPPING_SENDER_ZIP:
				return csShipping.SenderZip;

			case CartShipping.FIELD_ORDERSHIPPING_SENDER_ZIP_1:
				return csShipping.SenderZip1;

			case CartShipping.FIELD_ORDERSHIPPING_SENDER_ZIP_2:
				return csShipping.SenderZip2;

			case Constants.FIELD_ORDERSHIPPING_SENDER_ADDR1:
				return csShipping.SenderAddr1;

			case Constants.FIELD_ORDERSHIPPING_SENDER_ADDR2:
				return csShipping.SenderAddr2;

			case Constants.FIELD_ORDERSHIPPING_SENDER_ADDR3:
				return csShipping.SenderAddr3;

			case Constants.FIELD_ORDERSHIPPING_SENDER_ADDR4:
				return csShipping.SenderAddr4;

			case Constants.FIELD_ORDERSHIPPING_SENDER_ADDR5:
				return csShipping.SenderAddr5;

			case CartShipping.FIELD_ORDERSHIPPING_SENDER_ADDR5_US:
				return IsCountryUs(csShipping.SenderCountryIsoCode)
					? csShipping.SenderAddr5
					: (Constants.GLOBAL_OPTION_ENABLE)
						? Constants.US_STATES_LIST[0]
						: string.Empty;

			case Constants.FIELD_ORDERSHIPPING_SENDER_COMPANY_NAME:
				return csShipping.SenderCompanyName;

			case Constants.FIELD_ORDERSHIPPING_SENDER_COMPANY_POST_NAME:
				return csShipping.SenderCompanyPostName;

			case Constants.FIELD_ORDERSHIPPING_SENDER_TEL1:
				return csShipping.SenderTel1;

			case CartShipping.FIELD_ORDERSHIPPING_SENDER_TEL1_1:
				return csShipping.SenderTel1_1;

			case CartShipping.FIELD_ORDERSHIPPING_SENDER_TEL1_2:
				return csShipping.SenderTel1_2;

			case CartShipping.FIELD_ORDERSHIPPING_SENDER_TEL1_3:
				return csShipping.SenderTel1_3;

			case Constants.FIELD_ORDERSHIPPING_SENDER_COUNTRY_ISO_CODE:
				return csShipping.SenderCountryIsoCode;
		}

		return null;
	}

	/// <summary>
	/// 配送先表示設定値取得
	/// </summary>
	/// <param name="csShipping">配送先情報</param>
	/// <returns>チェック値</returns>
	public bool IsSameShippingAsCart1(CartShipping csShipping)
	{
		if (csShipping != null)
		{
			return csShipping.IsSameShippingAsCart1;
		}

		return true;
	}

	/// <summary>
	/// 配送希望日取得
	/// </summary>
	/// <param name="csShipping">配送方法情報</param>
	/// <returns>配送希望日</returns>
	public string GetShippingDate(CartShipping csShipping)
	{
		return csShipping.GetShippingDate();
	}

	/// <summary>
	/// 配送希望時間帯取得
	/// </summary>
	/// <param name="csShipping">配送方法情報</param>
	/// <returns>配送希望時間帯</returns>
	public string GetShippingTime(CartShipping csShipping)
	{
		return csShipping.GetShippingTime();
	}

	/// <summary>
	/// アドレス帳保存フラグ取得
	/// </summary>
	/// <param name="csShipping">配送先情報</param>
	/// <returns>チェック値</returns>
	public bool GetUserShippingRegiest(CartShipping csShipping)
	{
		if (csShipping != null)
		{
			// アドレス帳保存フラグ(保存する：true、保存しない：false)
			return csShipping.UserShippingRegistFlg;
		}

		return false;	// デフォルトは保存しない
	}

	/// <summary>
	/// のし利用フラグ有効判定
	/// </summary>
	/// <param name="htShopShipping">配送種別情報</param>
	/// <returns>のし利用フラグ有効状態</returns>
	public bool GetWrappingPaperFlgValid(Hashtable htShopShipping)
	{
		return (string)htShopShipping[Constants.FIELD_SHOPSHIPPING_WRAPPING_PAPER_FLG] == Constants.FLG_SHOPSHIPPING_WRAPPING_PAPER_FLG_VALID;

	}
	/// <summary>
	/// 包装利用フラグ有効判定
	/// </summary>
	/// <param name="htShopShipping">配送種別情報</param>
	/// <returns>包装利用フラグ有効状態</returns>
	public bool GetWrappingBagFlgValid(Hashtable htShopShipping)
	{
		return (string)htShopShipping[Constants.FIELD_SHOPSHIPPING_WRAPPING_BAG_FLG] == Constants.FLG_SHOPSHIPPING_WRAPPING_BAG_FLG_VALID;
	}

	#endregion

	#region お支払い方法表示関連

	/// <summary>
	/// カートお支払い情報から決済種別ID取得
	/// </summary>
	/// <param name="cpPayment">支払方法情報</param>
	/// <returns>チェック状態</returns>
	public string GetPaymentId(CartPayment cpPayment)
	{
		// カート決済情報なし or 決済種別無しの場合はデフォルト
		if ((cpPayment != null)
			&& (cpPayment.PaymentId != null))
		{
			return cpPayment.PaymentId;
		}

		return null;
	}

	/// <summary>
	/// カード決済フィールドデフォルト値取得
	/// </summary>
	/// <param name="payment">カート支払方法</param>
	/// <param name="field">フィールド名</param>
	/// <returns>カード決済フィールドデフォルト値</returns>
	public string GetCreditValue(CartPayment payment, string field)
	{
		if (payment.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT) return null;
		if (payment.CreditCardBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
		{
			if (string.IsNullOrEmpty(payment.CreditExpireMonth)
				&& (field == "credit_expire_month"))
			{
				payment.CreditExpireMonth = DateTime.Now.Month.ToString("00");
			}

			if (string.IsNullOrEmpty(payment.CreditExpireYear)
				&& (field == "credit_expire_year"))
			{
				var yearNow = DateTime.Now.Year.ToString();
				payment.CreditExpireYear = yearNow.Substring(yearNow.Length - 2);
			}

			// 新規カードの場合
			switch (field)
			{
				case CartPayment.FIELD_CREDIT_CARD_BRANCH_NO:
					return payment.CreditCardBranchNo;

				case CartPayment.FIELD_CREDIT_COMPANY:
					if (OrderCommon.CreditCompanySelectable) return payment.CreditCardCompany;
					return null;

				case CartPayment.FIELD_CREDIT_CARD_NO:
					return payment.CreditCardNo;

				case CartPayment.FIELD_CREDIT_CARD_NO_1:
					return payment.CreditCardNo1;

				case CartPayment.FIELD_CREDIT_CARD_NO_2:
					return payment.CreditCardNo2;

				case CartPayment.FIELD_CREDIT_CARD_NO_3:
					return payment.CreditCardNo3;

				case CartPayment.FIELD_CREDIT_CARD_NO_4:
					return payment.CreditCardNo4;

				case CartPayment.FIELD_CREDIT_EXPIRE_YEAR:
					return payment.CreditExpireYear;

				case CartPayment.FIELD_CREDIT_EXPIRE_MONTH:
					return payment.CreditExpireMonth;

				case CartPayment.FIELD_CREDIT_INSTALLMENTS_CODE:
					return payment.CreditInstallmentsCode;

				case CartPayment.FIELD_CREDIT_SECURITY_CODE:
					return payment.CreditSecurityCode;

				case CartPayment.FIELD_CREDIT_AUTHOR_NAME:
					return payment.CreditAuthorName;

				case CartPayment.FIELD_CREDIT_BINCODE:
					return payment.CreditBincode;
			}
		}
		else
		{
			// 登録済みカードの場合
			switch (field)
			{
				case CartPayment.FIELD_CREDIT_CARD_BRANCH_NO:
					return payment.CreditCardBranchNo;

				case CartPayment.FIELD_CREDIT_INSTALLMENTS_CODE:
					return payment.CreditInstallmentsCode;
			}
		}

		return null;
	}

	/// <summary>
	/// 電算システムコンビニ決済支払先取得
	/// </summary>
	/// <param name="cpPayment">カート支払方法</param>
	/// <returns>電算システムコンビニ決済支払先値</returns>
	public string GetDskConveniType(CartPayment cpPayment)
	{
		if ((cpPayment != null) && (cpPayment.PaymentObject is PaymentDskCvs))
		{
			PaymentDskCvs pdc = (PaymentDskCvs)cpPayment.PaymentObject;
			if ((pdc.ConveniType != null))
			{
				return pdc.ConveniType;
			}
		}

		return null;
	}

	/// <summary>
	/// SBPSコンビニ決済支払先取得
	/// </summary>
	/// <param name="cpPayment">カート支払方法</param>
	/// <returns>コンビニタイプ</returns>
	public string GetSBPSConveniType(CartPayment cpPayment)
	{
		if ((Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.SBPS)
			&& (cpPayment != null))
		{
			return cpPayment.SBPSWebCvsType.ToString();
		}

		return null;
	}

	/// <summary>
	/// ヤマトKWCコンビニ決済支払先取得
	/// </summary>
	/// <param name="cpPayment">カート支払方法</param>
	/// <returns>コンビニタイプ</returns>
	public string GetYamatoKwcConveniType(CartPayment cpPayment)
	{
		if ((Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.YamatoKwc)
			&& (cpPayment != null))
		{
			return cpPayment.YamatoKwcCvsType.ToString();
		}
		return null;
	}

	/// <summary>
	/// Gmo convenience store
	/// </summary>
	/// <param name="payment">Cart payment</param>
	/// <returns>Convenience store type</returns>
	public string GetGmoConveniType(CartPayment payment)
	{
		if ((Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Gmo)
			&& (payment != null)
			&& (string.IsNullOrEmpty(payment.GmoCvsType) == false))
		{
			return payment.GmoCvsType;
		}

		return null;
	}

	/// <summary>
	/// 登録済みクレジットカードを利用するか新規カードか
	/// </summary>
	/// <param name="cpPayment">カート支払方法</param>
	/// <returns>電算システムコンビニ決済支払先値</returns>
	public bool IsNewCreditCard(CartPayment cpPayment)
	{
		return (cpPayment.CreditCardBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW);
	}

	/// <summary>
	/// クレジットカード会社名取得
	/// </summary>
	/// <param name="value">会社コード</param>
	/// <returns>会社名</returns>
	public static string GetCreditCardCompanyName(object value)
	{
		return OrderCommon.GetCreditCardCompanyName(value);
	}

	/// <summary>
	/// 楽天IDConnectログイン時の決済種別ソート（楽天ペイを先頭にする)
	/// </summary>
	/// <param name="validPaymentList">有効な決済種別リスト</param>
	public PaymentModel[] SortPaymentListForRakutenIdConnectLoggedIn(PaymentModel[] validPaymentList)
	{
		// 楽天IDConnectでログインしている場合は楽天ペイを先頭にする
		if (SessionManager.IsRakutenIdConnectLoggedIn && validPaymentList
			.Any(p => p.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_RAKUTEN_ID_SBPS))
		{
			var sortedPaymentList = validPaymentList.OrderBy(
				payment => (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_RAKUTEN_ID_SBPS) ? 0 : 1).ToArray();
			return sortedPaymentList;
		}

		return validPaymentList;
	}

	/// <summary>
	/// Get selected payment id
	/// </summary>
	/// <param name="validPaymentList">有効な決済種別リスト</param>
	/// <param name="payment">カート支払方法</param>
	/// <returns>Payment id is selected or null value</returns>
	public string GetSelectedPaymentId(PaymentModel[] validPaymentList, CartPayment payment)
	{
		if (payment == null) return null;

		var paymentInformation = validPaymentList.FirstOrDefault(p => p.PaymentId == payment.PaymentId);
		return (paymentInformation != null) ? paymentInformation.PaymentId : null;
	}

	/// <summary>
	/// Get rakuten convenience type
	/// </summary>
	/// <param name="payment">Payment</param>
	/// <returns>Convenience store type</returns>
	public string GetRakutenConvenienceType(CartPayment payment)
	{
		if ((Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Rakuten)
			&& (payment != null)
			&& (string.IsNullOrEmpty(payment.RakutenCvsType) == false))
		{
			return payment.RakutenCvsType;
		}

		return null;
	}

	/// <summary>
	/// Get Zeus convenience type
	/// </summary>
	/// <param name="payment">Payment</param>
	/// <returns>Convenience store type</returns>
	public string GetZeusConvenienceType(CartPayment payment)
	{
		if (OrderCommon.IsPaymentCvsTypeZeus
			&& (payment != null))
		{
			return payment.GetZeusCvsType();
		}

		return null;
	}

	/// <summary>
	/// Get Paygent convenience type
	/// </summary>
	/// <param name="payment">Payment</param>
	/// <returns>Convenience store type</returns>
	public string GetPaygentConvenienceType(CartPayment payment)
	{
		if (OrderCommon.IsPaymentCvsTypePaygent
			&& (payment != null))
		{
			return payment.GetPaygentCvsType();
		}

		return null;
	}
	#endregion
}