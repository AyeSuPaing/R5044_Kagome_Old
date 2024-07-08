/*
=========================================================================================================
  Module      : つくーるAPI連携 レスポンスデータ(UreruAdResponseData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace w2.Commerce.Batch.ExternalOrderImport.Entity
{
	/// <summary>
	/// つくーるAPI連携 レスポンスデータ
	/// </summary>
	[JsonObject]
	public class UreruAdResponseData
	{
		/// <summary>レスポンスデータ</summary>
		[JsonProperty("results")]
		public UreruAdResponseDataItem[] Results { get; set; }
	}

	/// <summary>
	/// つくーるAPI連携 レスポンスデータレコード
	/// </summary>
	[JsonObject]
	public partial class UreruAdResponseDataItem
	{
		/// <summary>氏名</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_NAME)]
		public string Name { get; set; }
		/// <summary>姓</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_FAMILY_NAME)]
		public string FamilyName { get; set; }
		/// <summary>名</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_GIVEN_NAME)]
		public string GivenName { get; set; }
		/// <summary>ふりがな</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_KANA)]
		public string Kana { get; set; }
		/// <summary>せい</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_FAMILY_KANA)]
		public string FamilyKana { get; set; }
		/// <summary>めい</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_GIVEN_KANA)]
		public string GivenKana { get; set; }
		/// <summary>カタカナ</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_KATAKANA)]
		public string Katakana { get; set; }
		/// <summary>セイ</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_FAMILY_KATAKANA)]
		public string FamilyKatakana { get; set; }
		/// <summary>メイ</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_GIVEN_KATAKANA)]
		public string GivenKatakana { get; set; }
		/// <summary>性別</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_SEX)]
		public string Sex { get; set; }
		/// <summary>生年月日</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_BIRTHDAY)]
		public DateTime? Birthday { get; set; }
		/// <summary>郵便番号</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_ZIP_FULL_HYPHEN)]
		public string ZipFullHyphen { get; set; }
		/// <summary>郵便番号（上3桁）</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_ZIP1)]
		public string Zip1 { get; set; }
		/// <summary>郵便番号（下4桁）</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_ZIP2)]
		public string Zip2 { get; set; }
		/// <summary>住所</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_ADDRESS_FULL)]
		public string AddressFull { get; set; }
		/// <summary>都道府県</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_PREFECTURE)]
		public string Prefecture { get; set; }
		/// <summary>住所1（全角変換）</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_ADDRESS1_ZENKAKU)]
		public string Address1Zenkaku { get; set; }
		/// <summary>住所2（全角変換）</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_ADDRESS2_ZENKAKU)]
		public string Address2Zenkaku { get; set; }
		/// <summary>住所3（全角変換）</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_ADDRESS3_ZENKAKU)]
		public string Address3Zenkaku { get; set; }
		/// <summary>電話番号</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_TEL_NO_FULL)]
		public string TelNoFull { get; set; }
		/// <summary>電話番号（ハイフン付き）</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_TEL_NO_FULL_HYPHEN)]
		public string TelNoFullHyphen { get; set; }
		/// <summary>電話番号（市外局番）</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_TEL_NO1)]
		public string TelNo1 { get; set; }
		/// <summary>電話番号（市内局番）</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_TEL_NO2)]
		public string TelNo2 { get; set; }
		/// <summary>電話番号（加入者番号）</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_TEL_NO3)]
		public string TelNo3 { get; set; }
		/// <summary>メールアドレス</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_EMAIL)]
		public string Email { get; set; }
		/// <summary>お支払方法</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_PAYMENT_METHOD)]
		public string PaymentMethod { get; set; }
		/// <summary>GMOペイメント：オーダーID</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_GMO_ORDER_ID)]
		public string CreditGmoOrderId { get; set; }
		/// <summary>GMOペイメント：取引ID</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_GMO_ACCESS_ID)]
		public string CreditGmoAccessId { get; set; }
		/// <summary>GMOペイメント：取引パスワード</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_GMO_ACCESS_PASS)]
		public string CreditGmoAccessPass { get; set; }
		/// <summary>GMOペイメント：会員ID</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_GMO_MEMBER_ID)]
		public string CreditGmoMemberId { get; set; }
		/// <summary>ZEUS：オーダNo</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_ZEUS_ORDD)]
		public string CreditZeusOrdd { get; set; }
		/// <summary>ZEUS：ユニークなID</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_ZEUS_SENDID)]
		public string CreditZeusSendId { get; set; }
		/// <summary>VeriTrans：取引ID</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_VERITRANS_ACCESS_ID)]
		public string CreditVeritransAccessId { get; set; }
		/// <summary>ソニーペイメントサービス（e-SCOTT）：プロセスID</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_SONY_PAYMENT_PROCESS_ID)]
		public string CreditSonyPaymentProcessId { get; set; }
		/// <summary>ソニーペイメントサービス（e-SCOTT）：プロセスパスワード</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_SONY_PAYMENT_PROCESS_PASS)]
		public string CreditSonyPaymentProcessPass { get; set; }
		/// <summary>ソニーペイメントサービス（e-SCOTT）：会員ID</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_SONY_PAYMENT_KAIIN_ID)]
		public string CreditSonyPaymentKaiinId { get; set; }
		/// <summary>ソニーペイメントサービス（e-SCOTT）：会員パスワード</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_SONY_PAYMENT_KAIIN_PASS)]
		public string CreditSonyPaymentKaiinPass { get; set; }
		/// <summary>クロネコwebコレクト：与信承認番号</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_KURONEKO_WEB_COLLECT_CRD_C_RES_CD)]
		public string CreditKuronekoWebCollectCrdCResCd { get; set; }
		/// <summary>クロネコwebコレクト：受付番号</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_KURONEKO_WEB_COLLECT_ORDER_NO)]
		public string CreditKuronekoWebCollectOrderNo { get; set; }
		/// <summary>クロネコwebコレクト：カード保有者ID</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_KURONEKO_WEB_COLLECT_MEMBER_ID)]
		public string CreditKuronekoWebCollectMemberId { get; set; }
		/// <summary>クロネコwebコレクト：認証キー</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_KURONEKO_WEB_COLLECT_AUTHENTICATION_KEY)]
		public string CreditKuronekoWebCollectAuthenticationKey { get; set; }
		/// <summary>SBペイメントサービス：顧客ID</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_SOFT_BANK_PAYMENT_CUSTOMER_ID)]
		public string CreditSoftBankPaymentCustomerId { get; set; }
		/// <summary>SBペイメントサービス：受注ID</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_SOFT_BANK_PAYMENT_ORDER_ID)]
		public string CreditSoftBankPaymentOrderId { get; set; }
		/// <summary>SBペイメントサービス：トラッキングID</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_SOFT_BANK_PAYMENT_TRACKING_ID)]
		public string CreditSoftBankPaymentTrackingId { get; set; }
		/// <summary>Amazon Pay：注文番号(BillingAgreementId)</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_AMAZON_PAYMENTS_ID)]
		public string AmazonPaymentsId { get; set; }
		/// <summary>Amazon Pay：AmazonリファレンスID(OrderReferenceId)</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_AMAZON_PAYMENTS_ORDER_ID)]
		public string AmazonPaymentsOrderId { get; set; }
		/// <summary>Amazon Pay：取引ID</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_AMAZON_PAYMENTS_AUTH_ID)]
		public string AmazonPaymentsAuthId { get; set; }
		/// <summary>Amazon Pay：販売事業者リファレンスID</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_AMAZON_PAYMENTS_AUTH_REF_ID)]
		public string AmazonPaymentsAuthRefId { get; set; }
		/// <summary>Amazon Pay：氏名</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_AMAZON_PAYMENTS_NAME)]
		public string AmazonPaymentsName { get; set; }
		/// <summary>Amazon Pay：郵便番号(ハイフン付き)</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_AMAZON_PAYMENTS_POSTAL_CODE)]
		public string AmazonPaymentsPostalCode { get; set; }
		/// <summary>Amazon Pay：住所1</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_AMAZON_PAYMENTS_ADDRESS_LINE1)]
		public string AmazonPaymentsAddressLine1 { get; set; }
		/// <summary>Amazon Pay：住所2</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_AMAZON_PAYMENTS_ADDRESS_LINE2)]
		public string AmazonPaymentsAddressLine2 { get; set; }
		/// <summary>Amazon Pay：住所3</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_AMAZON_PAYMENTS_ADDRESS_LINE3)]
		public string AmazonPaymentsAddressLine3 { get; set; }
		/// <summary>NP後払いリアルタイム与信：与信結果</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_NP_AUTHORIZE_RESULT)]
		public string NpAuthorizeResult { get; set; }
		/// <summary>NP後払いリアルタイム与信：加盟店取引ID</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_SHOP_TRANSACTION_ID)]
		public string ShopTransactionId { get; set; }
		/// <summary>NP後払いリアルタイム与信：NP取引ID</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_NP_TRANSACTION_ID)]
		public string NpTransactionId { get; set; }
		/// <summary>メールオプトイン</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_MAIL_OPTIN_FLG)]
		public string MailOptinFlg { get; set; }
		/// <summary>受注日時</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_CREATED)]
		public DateTime? Created { get; set; }
		/// <summary>注文ID</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_ID)]
		public string Id { get; set; }
		/// <summary>キャンペーンタイプ</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_TYPE)]
		public string Type { get; set; }
		/// <summary>購入金額合計</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_TOTAL_INC)]
		public decimal? TotalInc { get; set; }
		/// <summary>商品金額合計</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_PRODUCT_TOTAL_INC)]
		public decimal? ProductTotalInc { get; set; }
		/// <summary>割引</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_DISCOUNT)]
		public decimal? Discount { get; set; }
		/// <summary>決済手数料</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_COMMISSION)]
		public decimal? Commission { get; set; }
		/// <summary>送料</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_SHIPPING_COST)]
		public decimal? ShippingCost { get; set; }
		/// <summary>販売商品コード</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_LANDING_PRODUCT_CODE)]
		public string LandingProductCode { get; set; }
		/// <summary>販売商品名</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_LANDING_PRODUCT_NAME)]
		public string LandingProductName { get; set; }
		/// <summary>販売商品価格（税込）</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_LANDING_PRODUCT_PRICE_INC)]
		public decimal? LandingProductPriceInc { get; set; }
		/// <summary>販売商品購入数</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_LANDING_PRODUCT_QTY)]
		public int? LandingProductQty { get; set; }
		/// <summary>販売商品販売方式</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_LANDING_PRODUCT_RECURRING_FLG)]
		public string LandingProductRecurringFlg { get; set; }
		/// <summary>アップセル販売商品コード</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_UPSELL_PRODUCT_CODE)]
		public string UpsellProductCode { get; set; }
		/// <summary>アップセル販売商品名</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_UPSELL_PRODUCT_NAME)]
		public string UpsellProductName { get; set; }
		/// <summary>アップセル販売	商品価格（税込）</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_UPSELL_PRODUCT_PRICE_INC)]
		public decimal? UpsellProductPriceInc { get; set; }
		/// <summary>アップセル販売商品購入数</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_UPSELL_PRODUCT_QTY)]
		public int? UpsellProductQty { get; set; }
		/// <summary>アップセル販売商品販売方式</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_UPSELL_PRODUCT_RECURRING_FLG)]
		public string UpsellProductRecurringFlg { get; set; }
		/// <summary>注文時クエリ</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_QUERY_STRING)]
		public string QueryString { get; set; }
		/// <summary>IPアドレス</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_IP_ADDRESS)]
		public string IpAddress { get; set; }
		/// <summary>ユーザーエージェント</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_USER_AGENT)]
		public string UserAgent { get; set; }
		/// <summary>管理者メモ（改行あり）</summary>
		[JsonProperty(Constants.URERU_AD_IMPORT_REQUEST_FIELD_NOTE)]
		public string Note { get; set; }
	}
}