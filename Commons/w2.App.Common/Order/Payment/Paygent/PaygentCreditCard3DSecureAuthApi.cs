/*
=========================================================================================================
  Module      : ペイジェントカード3Dセキュア認証API (PaygentCreditCard3DSecureAuthApi.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
namespace w2.App.Common.Order.Payment.Paygent
{
	/// <summary>
	/// ペイジェントカード情報設定API
	/// </summary>
	public class PaygentCreditCard3DSecureAuthApi : PaygentApiHeader
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="apiType">電文種別ID</param>
		public PaygentCreditCard3DSecureAuthApi() : base(PaygentConstants.PAYGENT_APITYPE_CARD_3DSECURE_AUTH)
		{
			this.SiteId = string.Empty;
			this.TermUrl = string.Empty;
			this.MerchantName = string.Empty;
			this.AuthenticationType = string.Empty;
			this.CardSetMethod = string.Empty;
			this.CardToken = string.Empty;
			this.CustomerId = string.Empty;
			this.CustomerCardId = string.Empty;
			this.CardNumber = string.Empty;
			this.CardValidTerm = string.Empty;
			this.GooglePaymentMethodToken = string.Empty;
			this.PaymentDate = string.Empty;
			this.CurrencyCode = string.Empty;
			this.PaymentAmount = string.Empty;
			this.TransactionType = string.Empty;
			this.CardholderName = string.Empty;
			this.LoginType = string.Empty;
			this.LoginDate = string.Empty;
			this.AccountIndicator = string.Empty;
			this.AccountChangeDate = string.Empty;
			this.AccountChangeIndicator = string.Empty;
			this.AccountCreateDate = string.Empty;
			this.PasswordChangeDate = string.Empty;
			this.PasswordChangeIndicator = string.Empty;
			this.PurchaseCount = string.Empty;
			this.CardRegistrationCount = string.Empty;
			this.ActivityCountDay = string.Empty;
			this.ActivityCountYear = string.Empty;
			this.CardRegistrationDate = string.Empty;
			this.CardRegistrationIndicator = string.Empty;
			this.ShipAddressUseDate = string.Empty;
			this.ShipAddressUseIndicator = string.Empty;
			this.ShipNameIndicator = string.Empty;
			this.SuspiciousAccActivity = string.Empty;
			this.AddressMatch = string.Empty;
			this.BillAddressCity = string.Empty;
			this.BillAddressCountry = string.Empty;
			this.BillAddressLine1 = string.Empty;
			this.BillAddressLine2 = string.Empty;
			this.BillAddressLine3 = string.Empty;
			this.BillAddressPostCode = string.Empty;
			this.BillAddressState = string.Empty;
			this.EmailAddress = string.Empty;
			this.HomePhoneCc = string.Empty;
			this.HomePhoneSubscriber = string.Empty;
			this.MobilePhoneCc = string.Empty;
			this.MobilePhoneSubscriber = string.Empty;
			this.ShipAddressCity = string.Empty;
			this.ShipAddressCountry = string.Empty;
			this.ShipAddressLine1 = string.Empty;
			this.ShipAddressLine2 = string.Empty;
			this.ShipAddressLine3 = string.Empty;
			this.ShipAddressPostCode = string.Empty;
			this.ShipAddressState = string.Empty;
			this.WorkPhoneCc = string.Empty;
			this.WorkPhoneSubscriber = string.Empty;
			this.DeliveryEmailAddress = string.Empty;
			this.DeliveryTimeframe = string.Empty;
			this.GiftCardAmount = string.Empty;
			this.GiftCardCount = string.Empty;
			this.GiftCardCurrencyCode = string.Empty;
			this.PreOrderDate = string.Empty;
			this.PreOrderPurchaseIndicator = string.Empty;
			this.ReorderIndicator = string.Empty;
			this.ShippingIndicator = string.Empty;
		}

		///<summary> サイトID</summary>
		public string SiteId
		{
			get { return this.RequestParams["site_id"]; }
			set { this.RequestParams["site_id"] = value; }
		}
		///<summary> 3Dセキュア戻りURL</summary>
		public string TermUrl
		{
			get { return this.RequestParams["term_url"]; }
			set { this.RequestParams["term_url"] = value; }
		}
		///<summary> 加盟店名</summary>
		public string MerchantName
		{
			get { return this.RequestParams["merchant_name"]; }
			set { this.RequestParams["merchant_name"] = value; }
		}
		///<summary> 認証用途</summary>
		public string AuthenticationType
		{
			get { return this.RequestParams["authentication_type"]; }
			set { this.RequestParams["authentication_type"] = value; }
		}
		///<summary> カード情報の指定方法</summary>
		public string CardSetMethod
		{
			get { return this.RequestParams["card_set_method"]; }
			set { this.RequestParams["card_set_method"] = value; }
		}
		///<summary> カード情報トークン</summary>
		public string CardToken
		{
			get { return this.RequestParams["card_token"]; }
			set { this.RequestParams["card_token"] = value; }
		}
		///<summary> 顧客ID</summary>
		public string CustomerId
		{
			get { return this.RequestParams["customer_id"]; }
			set { this.RequestParams["customer_id"] = value; }
		}
		///<summary> 顧客カードID</summary>
		public string CustomerCardId
		{
			get { return this.RequestParams["customer_card_id"]; }
			set { this.RequestParams["customer_card_id"] = value; }
		}
		///<summary> カード番号</summary>
		public string CardNumber
		{
			get { return this.RequestParams["card_number"]; }
			set { this.RequestParams["card_number"] = value; }
		}
		///<summary> カード有効期限</summary>
		public string CardValidTerm
		{
			get { return this.RequestParams["card_valid_term"]; }
			set { this.RequestParams["card_valid_term"] = value; }
		}
		///<summary> Google PaymentMethodToken</summary>
		public string GooglePaymentMethodToken
		{
			get { return this.RequestParams["google_payment_method_token"]; }
			set { this.RequestParams["google_payment_method_token"] = value; }
		}
		///<summary> 購入日時</summary>
		public string PaymentDate
		{
			get { return this.RequestParams["payment_date"]; }
			set { this.RequestParams["payment_date"] = value; }
		}
		///<summary> 通貨コード</summary>
		public string CurrencyCode
		{
			get { return this.RequestParams["currency_code"]; }
			set { this.RequestParams["currency_code"] = value; }
		}
		///<summary> 決済金額</summary>
		public string PaymentAmount
		{
			get { return this.RequestParams["payment_amount"]; }
			set { this.RequestParams["payment_amount"] = value; }
		}
		///<summary> 取引タイプ</summary>
		public string TransactionType
		{
			get { return this.RequestParams["transaction_type"]; }
			set { this.RequestParams["transaction_type"] = value; }
		}
		///<summary> カード名義人</summary>
		public string CardholderName
		{
			get { return this.RequestParams["cardholder_name"]; }
			set { this.RequestParams["cardholder_name"] = value; }
		}
		///<summary> ログイン方法</summary>
		public string LoginType
		{
			get { return this.RequestParams["login_type"]; }
			set { this.RequestParams["login_type"] = value; }
		}
		///<summary> ログイン日時</summary>
		public string LoginDate
		{
			get { return this.RequestParams["login_date"]; }
			set { this.RequestParams["login_date"] = value; }
		}
		///<summary> アカウントの保有時間</summary>
		public string AccountIndicator
		{
			get { return this.RequestParams["account_indicator"]; }
			set { this.RequestParams["account_indicator"] = value; }
		}
		///<summary> アカウントの保有時間更新日</summary>
		public string AccountChangeDate
		{
			get { return this.RequestParams["account_change_date"]; }
			set { this.RequestParams["account_change_date"] = value; }
		}
		///<summary> アカウント更新後の経過時間</summary>
		public string AccountChangeIndicator
		{
			get { return this.RequestParams["account_change_indicator"]; }
			set { this.RequestParams["account_change_indicator"] = value; }
		}
		///<summary> アカウント作成日</summary>
		public string AccountCreateDate
		{
			get { return this.RequestParams["account_create_date"]; }
			set { this.RequestParams["account_create_date"] = value; }
		}
		///<summary> パスワード更新日</summary>
		public string PasswordChangeDate
		{
			get { return this.RequestParams["password_change_date"]; }
			set { this.RequestParams["password_change_date"] = value; }
		}
		///<summary> パスワード更新日更新後の経過時間</summary>
		public string PasswordChangeIndicator
		{
			get { return this.RequestParams["password_change_indicator"]; }
			set { this.RequestParams["password_change_indicator"] = value; }
		}
		///<summary> 購入回数（全決済手段）</summary>
		public string PurchaseCount
		{
			get { return this.RequestParams["purchase_count"]; }
			set { this.RequestParams["purchase_count"] = value; }
		}
		///<summary> カード追加回数</summary>
		public string CardRegistrationCount
		{
			get { return this.RequestParams["card_registration_count"]; }
			set { this.RequestParams["card_registration_count"] = value; }
		}
		///<summary> 取引回数（過去24時間）</summary>
		public string ActivityCountDay
		{
			get { return this.RequestParams["activity_count_day"]; }
			set { this.RequestParams["activity_count_day"] = value; }
		}
		///<summary> 取引回数（過去1年間）</summary>
		public string ActivityCountYear
		{
			get { return this.RequestParams["activity_count_year"]; }
			set { this.RequestParams["activity_count_year"] = value; }
		}
		///<summary> カード登録日時</summary>
		public string CardRegistrationDate
		{
			get { return this.RequestParams["card_registration_date"]; }
			set { this.RequestParams["card_registration_date"] = value; }
		}
		///<summary> カード登録後の経過時間</summary>
		public string CardRegistrationIndicator
		{
			get { return this.RequestParams["card_registration_indicator"]; }
			set { this.RequestParams["card_registration_indicator"] = value; }
		}
		///<summary> 配送先住所の初回利用日</summary>
		public string ShipAddressUseDate
		{
			get { return this.RequestParams["ship_address_use_date"]; }
			set { this.RequestParams["ship_address_use_date"] = value; }
		}
		///<summary> 配送先住所を初回利用してからの経過時間</summary>
		public string ShipAddressUseIndicator
		{
			get { return this.RequestParams["ship_address_use_indicator"]; }
			set { this.RequestParams["ship_address_use_indicator"] = value; }
		}
		///<summary> 配送先氏名の確認</summary>
		public string ShipNameIndicator
		{
			get { return this.RequestParams["ship_name_indicator"]; }
			set { this.RequestParams["ship_name_indicator"] = value; }
		}
		///<summary> 不正行為の疑い</summary>
		public string SuspiciousAccActivity
		{
			get { return this.RequestParams["suspicious_acc_activity"]; }
			set { this.RequestParams["suspicious_acc_activity"] = value; }
		}
		///<summary> 住所の確認</summary>
		public string AddressMatch
		{
			get { return this.RequestParams["address_match"]; }
			set { this.RequestParams["address_match"] = value; }
		}
		///<summary> 請求先情報（都市）</summary>
		public string BillAddressCity
		{
			get { return this.RequestParams["bill_address_city"]; }
			set { this.RequestParams["bill_address_city"] = value; }
		}
		///<summary> 請求先情報（国番）</summary>
		public string BillAddressCountry
		{
			get { return this.RequestParams["bill_address_country"]; }
			set { this.RequestParams["bill_address_country"] = value; }
		}
		///<summary> 請求先情報（住所1）</summary>
		public string BillAddressLine1
		{
			get { return this.RequestParams["bill_address_line1"]; }
			set { this.RequestParams["bill_address_line1"] = value; }
		}
		///<summary> 請求先情報（住所2）</summary>
		public string BillAddressLine2
		{
			get { return this.RequestParams["bill_address_line2"]; }
			set { this.RequestParams["bill_address_line2"] = value; }
		}
		///<summary> 請求先情報（住所3）</summary>
		public string BillAddressLine3
		{
			get { return this.RequestParams["bill_address_line3"]; }
			set { this.RequestParams["bill_address_line3"] = value; }
		}
		///<summary> 請求先情報（郵便番号）</summary>
		public string BillAddressPostCode
		{
			get { return this.RequestParams["bill_address_post_code"]; }
			set { this.RequestParams["bill_address_post_code"] = value; }
		}
		///<summary> 請求先情報（州・都道府県）</summary>
		public string BillAddressState
		{
			get { return this.RequestParams["bill_address_state"]; }
			set { this.RequestParams["bill_address_state"] = value; }
		}
		///<summary> メールアドレス</summary>
		public string EmailAddress
		{
			get { return this.RequestParams["email_address "]; }
			set { this.RequestParams["email_address "] = value; }
		}
		///<summary> 自宅電話番号（国コード）</summary>
		public string HomePhoneCc
		{
			get { return this.RequestParams["home_phone_cc"]; }
			set { this.RequestParams["home_phone_cc"] = value; }
		}
		///<summary> 自宅電話番号</summary>
		public string HomePhoneSubscriber
		{
			get { return this.RequestParams["home_phone_subs criber"]; }
			set { RequestParams["home_phone_subs criber"] = value; }
		}
		///<summary> 携帯電話番号（国コード）</summary>
		public string MobilePhoneCc
		{
			get { return this.RequestParams["mobile_phone_cc"]; }
			set { this.RequestParams["mobile_phone_cc"] = value; }
		}
		///<summary> 携帯電話番号</summary>
		public string MobilePhoneSubscriber
		{
			get { return this.RequestParams["mobile_phone_subscriber"]; }
			set { this.RequestParams["mobile_phone_subscriber"] = value; }
		}
		///<summary> 配送先情報（都市）</summary>
		public string ShipAddressCity
		{
			get { return this.RequestParams["ship_address_city"]; }
			set { this.RequestParams["ship_address_city"] = value; }
		}
		///<summary> 配送先情報（国番号）</summary>
		public string ShipAddressCountry
		{
			get { return this.RequestParams["ship_address_country"]; }
			set { this.RequestParams["ship_address_country"] = value; }
		}
		///<summary> 配送先情報（住所1）</summary>
		public string ShipAddressLine1
		{
			get { return this.RequestParams["ship_address_line1"]; }
			set { this.RequestParams["ship_address_line1"] = value; }
		}
		///<summary> 配送先情報（住所2）</summary>
		public string ShipAddressLine2
		{
			get { return this.RequestParams["ship_address_line2"]; }
			set { this.RequestParams["ship_address_line2"] = value; }
		}
		///<summary> 配送先情報（住所3）</summary>
		public string ShipAddressLine3
		{
			get { return this.RequestParams["ship_address_line3"]; }
			set { this.RequestParams["ship_address_line3"] = value; }
		}
		///<summary> 配送先情報（郵便番号）</summary>
		public string ShipAddressPostCode
		{
			get { return this.RequestParams["ship_address_post_code"]; }
			set { this.RequestParams["ship_address_post_code"] = value; }
		}
		///<summary> 配送先情報（州・都道府県）</summary>
		public string ShipAddressState
		{
			get { return this.RequestParams["ship_address_state"]; }
			set { this.RequestParams["ship_address_state"] = value; }
		}
		///<summary> 職場電話番号（国コード）</summary>
		public string WorkPhoneCc
		{
			get { return this.RequestParams["work_phone_cc"]; }
			set { this.RequestParams["work_phone_cc"] = value; }
		}
		///<summary> 職場電話番号</summary>
		public string WorkPhoneSubscriber
		{
			get { return this.RequestParams["work_phone_subscriber"]; }
			set { this.RequestParams["work_phone_subscriber"] = value; }
		}
		///<summary> 納品先電子メールアドレス</summary>
		public string DeliveryEmailAddress
		{
			get { return this.RequestParams["delivery_email_address"]; }
			set { this.RequestParams["delivery_email_address"] = value; }
		}
		///<summary> 商品納品時間枠</summary>
		public string DeliveryTimeframe
		{
			get { return this.RequestParams["delivery_timeframe"]; }
			set { this.RequestParams["delivery_timeframe"] = value; }
		}
		///<summary> ギフトカード購入金額</summary>
		public string GiftCardAmount
		{
			get { return this.RequestParams["gift_card_amount"]; }
			set { this.RequestParams["gift_card_amount"] = value; }
		}
		///<summary> ギフトカード購入枚数</summary>
		public string GiftCardCount
		{
			get { return this.RequestParams["gift_card_count"]; }
			set { this.RequestParams["gift_card_count"] = value; }
		}
		///<summary> ギフトカードの通貨コード</summary>
		public string GiftCardCurrencyCode
		{
			get { return this.RequestParams["gift_card_currency_code"]; }
			set { this.RequestParams["gift_card_currency_code"] = value; }
		}
		///<summary> 予約商品の発売予定日</summary>
		public string PreOrderDate
		{
			get { return this.RequestParams["pre_order_date"]; }
			set { this.RequestParams["pre_order_date"] = value; }
		}
		///<summary> 商品の発送状態</summary>
		public string PreOrderPurchaseIndicator
		{
			get { return this.RequestParams["pre_order_purchas e_indicator"]; }
			set { RequestParams["pre_order_purchas e_indicator"] = value; }
		}
		///<summary> 再注文区分</summary>
		public string ReorderIndicator
		{
			get { return this.RequestParams["reorder_indicator"]; }
			set { this.RequestParams["reorder_indicator"] = value; }
		}
		///<summary> 出荷方法</summary>
		public string ShippingIndicator
		{
			get { return this.RequestParams["shipping_indicator"]; }
			set { this.RequestParams["shipping_indicator"] = value; }
		}
	}
}
