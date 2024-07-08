/*
=========================================================================================================
  Module      : 定数定義(Constants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Commerce.Batch.ExternalOrderImport
{
	/// <summary>
	/// 定数定義
	/// </summary>
	public class Constants : w2.App.Common.Constants
	{
		/// <summary>最終実行ファイル名プリフィックス</summary>
		public const string FILENAME_LASTEXEC_PREFIX = "_LastExec";
		/// <summary>最終実行ファイル名サフィックス日付フォーマット</summary>
		public const string FILENAME_LASTEXEC_SUFFIX_DATEFORMAT = "yyyyMMddHHmmss";

		/// <summary>つくーるAPI連携：受注連携API URL</summary>
		public static string URERU_AD_IMPORT_API_URL = "";
		/// <summary>つくーるAPI連携：受注連携API用アカウントID</summary>
		public static string URERU_AD_IMPORT_ACCOUNT = "";
		/// <summary>つくーるAPI連携：受注連携API用パスワード</summary>
		public static string URERU_AD_IMPORT_PASS = "";
		/// <summary>つくーるAPI連携：受注連携API用アクセスキー</summary>
		public static string URERU_AD_IMPORT_KEY = "";
		/// <summary>つくーるAPI連携：新規ユーザー登録 顧客区分設定</summary>
		public static string URERU_AD_IMPORT_DEFAULT_USER_KBN = "";
		/// <summary>つくーるAPI連携：リクエストパラメーター日付フォーマット</summary>
		public const string URERU_AD_IMPORT_REQUEST_DATEFORMAT = "yyyy-MM-dd HH:mm";
		/// <summary>つくーるAPI連携：ユーザークレジットカード情報登録値 カード表示名</summary>
		public const string URERU_AD_IMPORT_USER_CREDIT_CARD_CARD_DISP_NAME = "クレジットカード情報";
		/// <summary>つくーるAPI連携：ユーザークレジットカード情報登録値 カード番号下4桁</summary>
		public const string URERU_AD_IMPORT_USER_CREDIT_CARD_LAST_FOUR_DIGIT = "XXXX";
		/// <summary>つくーるAPI連携：ユーザークレジットカード情報登録値 有効期限（月）</summary>
		public const string URERU_AD_IMPORT_USER_CREDIT_CARD_EXPIRATION_MONTH = "XX";
		/// <summary>つくーるAPI連携：ユーザークレジットカード情報登録値 有効期限（年）</summary>
		public const string URERU_AD_IMPORT_USER_CREDIT_CARD_EXPIRATION_YEAR = "XX";
		/// <summary>つくーるAPI連携：ユーザークレジットカード情報登録値 カード名義人</summary>
		public const string URERU_AD_IMPORT_USER_CREDIT_CARD_AUTHOR_NAME = "XXXXXXXX";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 氏名</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_NAME = "name";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 姓</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_FAMILY_NAME = "family_name";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 名</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_GIVEN_NAME = "given_name";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 ふりがな</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_KANA = "kana";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 せい</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_FAMILY_KANA = "family_kana";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 めい</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_GIVEN_KANA = "given_kana";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 フリガナ</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_KATAKANA = "katakana";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 セイ</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_FAMILY_KATAKANA = "family_katakana";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 メイ</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_GIVEN_KATAKANA = "given_katakana";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 性別</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_SEX = "sex";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 生年月日</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_BIRTHDAY = "birthday";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 郵便番号</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_ZIP_FULL_HYPHEN = "zip_full_hyphen";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 郵便番号（上3桁）</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_ZIP1 = "zip1";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 郵便番号（下4桁）</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_ZIP2 = "zip2";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 住所</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_ADDRESS_FULL = "address_full";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 都道府県</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_PREFECTURE = "prefecture";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 住所1（全角変換）</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_ADDRESS1_ZENKAKU = "address1_zenkaku";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 住所2（全角変換）</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_ADDRESS2_ZENKAKU = "address2_zenkaku";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 住所3（全角変換）</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_ADDRESS3_ZENKAKU = "address3_zenkaku";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 電話番号</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_TEL_NO_FULL = "tel_no_full";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 電話番号（ハイフン付き）</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_TEL_NO_FULL_HYPHEN = "tel_no_full_hyphen";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 電話番号（市外局番）</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_TEL_NO1 = "tel_no1";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 電話番号（市内局番）</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_TEL_NO2 = "tel_no2";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 電話番号（加入者番号）</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_TEL_NO3 = "tel_no3";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 メールアドレス</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_EMAIL = "email";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 お支払方法</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_PAYMENT_METHOD = "payment_method";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 GMOペイメント：オーダーID</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_GMO_ORDER_ID = "credit_gmo_order_id";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 GMOペイメント：取引ID</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_GMO_ACCESS_ID = "credit_gmo_access_id";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 GMOペイメント：取引パスワード</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_GMO_ACCESS_PASS = "credit_gmo_access_pass";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 GMOペイメント会員ID</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_GMO_MEMBER_ID = "credit_gmo_member_id";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 ZEUS：オーダNo</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_ZEUS_ORDD = "credit_zeus_ordd";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 ZEUS：ユニークなID</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_ZEUS_SENDID = "credit_zeus_sendid";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 VeriTrans：取引ID</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_VERITRANS_ACCESS_ID = "credit_veritrans_order_id";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 ソニーペイメントサービス（e-SCOTT）：プロセスID</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_SONY_PAYMENT_PROCESS_ID = "credit_sony_payment_process_id";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 ソニーペイメントサービス（e-SCOTT）：プロセスパスワード</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_SONY_PAYMENT_PROCESS_PASS = "credit_sony_payment_process_pass";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 ソニーペイメントサービス（e-SCOTT）：会員ID</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_SONY_PAYMENT_KAIIN_ID = "credit_sony_payment_kaiin_id";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 ソニーペイメントサービス（e-SCOTT）：会員パスワード</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_SONY_PAYMENT_KAIIN_PASS = "credit_sony_payment_kaiin_pass";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 クロネコwebコレクト：与信承認番号</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_KURONEKO_WEB_COLLECT_CRD_C_RES_CD = "credit_kuroneko_web_collect_crd_c_res_cd";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 クロネコwebコレクト：受付番号</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_KURONEKO_WEB_COLLECT_ORDER_NO = "credit_kuroneko_web_collect_order_no";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 クロネコwebコレクト：カード保有者ID</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_KURONEKO_WEB_COLLECT_MEMBER_ID = "credit_kuroneko_web_collect_member_id";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 クロネコwebコレクト：認証キー</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_KURONEKO_WEB_COLLECT_AUTHENTICATION_KEY = "credit_kuroneko_web_collect_authentication_key";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 SBペイメントサービス：顧客ID</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_SOFT_BANK_PAYMENT_CUSTOMER_ID = "credit_soft_bank_payment_customer_id";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 SBペイメントサービス：受注ID</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_SOFT_BANK_PAYMENT_ORDER_ID = "credit_soft_bank_payment_order_id";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 SBペイメントサービス：トラッキングID</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_CREDIT_SOFT_BANK_PAYMENT_TRACKING_ID = "credit_soft_bank_payment_tracking_id";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 Amazon Pay：注文番号(BillingAgreementId)</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_AMAZON_PAYMENTS_ID = "amazon_payments_id";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 Amazon Pay：AmazonリファレンスID(OrderReferenceId)</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_AMAZON_PAYMENTS_ORDER_ID = "amazon_payments_order_id";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 Amazon Pay：取引ID</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_AMAZON_PAYMENTS_AUTH_ID = "amazon_payments_auth_id";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 Amazon Pay：販売事業者リファレンスID</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_AMAZON_PAYMENTS_AUTH_REF_ID = "amazon_payments_auth_ref_id";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 Amazon Pay：氏名</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_AMAZON_PAYMENTS_NAME = "amazon_payments_name";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 Amazon Pay：郵便番号(ハイフン付き)</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_AMAZON_PAYMENTS_POSTAL_CODE = "amazon_payments_postal_code";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 Amazon Pay：住所1</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_AMAZON_PAYMENTS_ADDRESS_LINE1 = "amazon_payments_address_line1";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 Amazon Pay：住所2</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_AMAZON_PAYMENTS_ADDRESS_LINE2 = "amazon_payments_address_line2";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 Amazon Pay：住所3</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_AMAZON_PAYMENTS_ADDRESS_LINE3 = "amazon_payments_address_line3";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 NP後払いリアルタイム与信：与信結果</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_NP_AUTHORIZE_RESULT = "np_authorize_result";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 NP後払いリアルタイム与信：加盟店取引ID</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_SHOP_TRANSACTION_ID = "shop_transaction_id";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 NP後払いリアルタイム与信：NP取引ID</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_NP_TRANSACTION_ID = "np_transaction_id";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 メールオプトイン</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_MAIL_OPTIN_FLG = "mail_optin_flg";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 受注日時（YYYY-MM-DD HH:MM:SS）</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_CREATED = "created";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 注文ID</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_ID = "id";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 キャンペーンタイプ</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_TYPE = "type";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 購入金額合計</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_TOTAL_INC = "total_inc";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 商品金額合計</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_PRODUCT_TOTAL_INC = "product_total_inc";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 割引</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_DISCOUNT = "discount";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 決済手数料</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_COMMISSION = "commission";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 送料</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_SHIPPING_COST = "shipping_cost";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 販売商品コード</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_LANDING_PRODUCT_CODE = "landing_product_code";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 販売商品名</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_LANDING_PRODUCT_NAME = "landing_product_name";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 販売商品価格（税込）</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_LANDING_PRODUCT_PRICE_INC = "landing_product_price_inc";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 販売商品購入数</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_LANDING_PRODUCT_QTY = "landing_product_qty";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 販売商品販売方式</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_LANDING_PRODUCT_RECURRING_FLG = "landing_product_recurring_flg";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 アップセル販売商品コード</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_UPSELL_PRODUCT_CODE = "upsell_product_code";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 アップセル販売商品名</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_UPSELL_PRODUCT_NAME = "upsell_product_name";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 アップセル販売商品価格（税込）</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_UPSELL_PRODUCT_PRICE_INC = "upsell_product_price_inc";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 アップセル販売商品購入数</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_UPSELL_PRODUCT_QTY = "upsell_product_qty";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 アップセル販売商品販売方式</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_UPSELL_PRODUCT_RECURRING_FLG = "upsell_product_recurring_flg";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 注文時クエリ</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_QUERY_STRING = "query_string";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 IPアドレス</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_IP_ADDRESS = "ip_address";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 ユーザーエージェント</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_USER_AGENT = "user_agent";
		/// <summary>つくーるAPI連携：受注連携API用取得フィールド名 管理者メモ（改行あり）</summary>
		public const string URERU_AD_IMPORT_REQUEST_FIELD_NOTE = "note";
		/// <summary>つくーるAPI連携：受注連携API 注文時クエリパラメータ：広告コード</summary>
		public const string URERU_AD_IMPORT_QUERY_STRING_PARAMETER_UTM_CAMPAIGN = "utm_campaign";
		/// <summary>つくーるAPI連携：受注連携API 注文時クエリパラメータ：頒布会コースID</summary>
		public const string URERU_AD_IMPORT_QUERY_STRING_PARAMETER_SUBSCRIPTION_BOX_COURSE_ID = "tsid";

		/// <summary>つくーるAPI連携：新規ユーザー登録 顧客区分設定 会員</summary>
		public const string FLG_URERU_AD_IMPORT_DEFAULT_USER_KBN_USER = "USER";
		/// <summary>つくーるAPI連携：新規ユーザー登録 顧客区分設定 ゲスト</summary>
		public const string FLG_URERU_AD_IMPORT_DEFAULT_USER_KBN_GUEST = "GUEST";
		/// <summary>つくーるAPI連携：お支払方法 クレジットカード</summary>
		public const string FLG_URERU_AD_IMPORT_PAYMENT_METHOD_CREDIT = "credit";
		/// <summary>つくーるAPI連携：お支払方法 代引き</summary>
		public const string FLG_URERU_AD_IMPORT_PAYMENT_METHOD_COLLECT = "collect";
		/// <summary>つくーるAPI連携：お支払方法 クロネコ代金後払いサービス</summary>
		public const string FLG_URERU_AD_IMPORT_PAYMENT_METHOD_KURONEKO_PS = "kuroneko_ps";
		/// <summary>つくーるAPI連携：お支払方法 GMO後払い</summary>
		public const string FLG_URERU_AD_IMPORT_PAYMENT_METHOD_GMO_PS = "gmo_ps";
		/// <summary>つくーるAPI連携：お支払方法 Amazon Pay</summary>
		public const string FLG_URERU_AD_IMPORT_PAYMENT_METHOD_AMAZON_PAYMENTS = "amazon_payments";
		/// <summary>つくーるAPI連携：お支払方法 決済なし</summary>
		public const string FLG_URERU_AD_IMPORT_PAYMENT_METHOD_NONE = "none";
		/// <summary>つくーるAPI連携：お支払方法 NP後払い</summary>
		public const string FLG_URERU_AD_IMPORT_PAYMENT_METHOD_NP = "np";
		/// <summary>つくーるAPI連携：お支払方法 NP後払いwiz</summary>
		public const string FLG_URERU_AD_IMPORT_PAYMENT_METHOD_NP_WIZ = "np_wiz";
		/// <summary>つくーるAPI連携：お支払方法 ATODENE</summary>
		public const string FLG_URERU_AD_IMPORT_PAYMENT_METHOD_ATODENE = "atodene";
		/// <summary>つくーるAPI連携：お支払方法 後払い.com</summary>
		public const string FLG_URERU_AD_IMPORT_PAYMENT_METHOD_ATOBARAI_COM = "atobarai_com";
		/// <summary>つくーるAPI連携：販売商品販売方式 通常商品</summary>
		public const string FLG_URERU_AD_IMPORT_PRODUCT_RECURRING_FLG_PRODUCT = "0";
		/// <summary>つくーるAPI連携：販売商品販売方式 定期商品</summary>
		public const string FLG_URERU_AD_IMPORT_PRODUCT_RECURRING_FLG_FIXED_PURCHASE = "1";

		/// <summary>メールテンプレート：処理名</summary>
		public const string EXTERNAL_ORDER_IMPORT_MAILTEMPLATE_KEY_APP_NAME = "app_name";
		/// <summary>メールテンプレート：処理結果</summary>
		public const string EXTERNAL_ORDER_IMPORT_MAILTEMPLATE_KEY_RESULT = "result";
		/// <summary>メールテンプレート：処理開始時間</summary>
		public const string EXTERNAL_ORDER_IMPORT_MAILTEMPLATE_KEY_TIME_BEGIN = "time_begin";
		/// <summary>メールテンプレート：処理終了時間</summary>
		public const string EXTERNAL_ORDER_IMPORT_MAILTEMPLATE_KEY_TIME_END = "time_end";
		/// <summary>メールテンプレート：処理総件数</summary>
		public const string EXTERNAL_ORDER_IMPORT_MAILTEMPLATE_KEY_EXECUTE_COUNT = "execute_count";
		/// <summary>メールテンプレート：成功件数</summary>
		public const string EXTERNAL_ORDER_IMPORT_MAILTEMPLATE_KEY_SUCCESS_COUNT = "success_count";
		/// <summary>メールテンプレート：失敗件数</summary>
		public const string EXTERNAL_ORDER_IMPORT_MAILTEMPLATE_KEY_FAILURE_COUNT = "failure_count";
		/// <summary>メールテンプレート：処理スキップ件数</summary>
		public const string EXTERNAL_ORDER_IMPORT_MAILTEMPLATE_KEY_SKIP_COUNT = "skip_count";
		/// <summary>メールテンプレート：メッセージ</summary>
		public const string EXTERNAL_ORDER_IMPORT_MAILTEMPLATE_KEY_MESSAGE = "message";

		/// <summary>つくーるAPI連携：不正な広告コードエラーメッセージ</summary>
		public const string ERRMSG_MANAGER_ADVCODE_NO_EXIST_ERROR = "ERRMSG_MANAGER_ADVCODE_NO_EXIST_ERROR";
		/// <summary>つくーるAPI連携：ユーザー特定不可警告メッセージ</summary>
		public const string ERRMSG_MANAGER_USER_INVALID_BATCH = "ERRMSG_MANAGER_USER_INVALID_BATCH";
		/// <summary>つくーるAPI連携：不正な商品情報エラーメッセージ</summary>
		public const string ERRMSG_MANAGER_PRODUCT_NOT_EXIST = "ERRMSG_MANAGER_PRODUCT_NOT_EXIST";
		/// <summary>つくーるAPI連携：不正な頒布会コース情報エラーメッセージ</summary>
		public const string ERRMSG_MANAGER_SUBSCRIPTION_BOX_NOT_EXIST = "ERRMSG_MANAGER_SUBSCRIPTION_BOX_NOT_EXIST";
		/// <summary>つくーるAPI連携：不正な配送種別エラーメッセージ</summary>
		public const string ERRMSG_MANAGER_SHOPSHIPPING_NOT_EXIST = "ERRMSG_MANAGER_SHOPSHIPPING_NOT_EXIST";
		/// <summary>つくーるAPI連携：不正な決済種別エラーメッセージ</summary>
		public const string ERRMSG_MANAGER_PAYMENT_NO_DATA = "ERRMSG_MANAGER_PAYMENT_NO_DATA";
		/// <summary>つくーるAPI連携：必須入力エラーメッセージ</summary>
		public const string ERRMSG_MANAGER_NECESSARY = "ERRMSG_MANAGER_NECESSARY";
		/// <summary>つくーるAPI連携：サイズ超過エラーメッセージ</summary>
		public const string ERRMSG_MANAGER_LENGTH_MAX = "ERRMSG_MANAGER_LENGTH_MAX";
		/// <summary>つくーるAPI連携：決済代行会社設定エラーメッセージ</summary>
		public const string ERRMSG_MANAGER_INCOMPATIBLE_CREDIT_CARD = "ERRMSG_MANAGER_INCOMPATIBLE_CREDIT_CARD";
		/// <summary>つくーるAPI連携：重複注文エラーメッセージ</summary>
		public const string ERRMSG_MANAGER_DUPLICATED_ORDER = "ERRMSG_MANAGER_DUPLICATED_ORDER";
		/// <summary>つくーるAPI連携：不正な決済方法エラーメッセージ</summary>
		public const string ERRMSG_MANAGER_UNUSABLE_PAYMENT_METHOD = "ERRMSG_MANAGER_UNUSABLE_PAYMENT_METHOD";
		/// <summary>つくーるAPI連携：決済方法該当なしエラーメッセージ</summary>
		public const string ERRMSG_MANAGER_NOT_EXIST_PAYMENT_METHOD = "ERRMSG_MANAGER_NOT_EXIST_PAYMENT_METHOD";
		/// <summary>つくーるAPI連携：クレジットカード情報連携ID取得エラーメッセージ</summary>
		public const string ERRMSG_MANAGER_INVALID_COOPERATION_ID = "ERRMSG_MANAGER_INVALID_COOPERATION_ID";
		/// <summary>つくーるAPI連携：決済カード取引ID取得エラーメッセージ</summary>
		public const string ERRMSG_MANAGER_INVALID_CARD_TRAN_ID = "ERRMSG_MANAGER_INVALID_CARD_TRAN_ID";
		/// <summary>つくーるAPI連携：決済注文ID取得エラーメッセージ</summary>
		public const string ERRMSG_MANAGER_INVALID_PAYMENT_ORDER_ID = "ERRMSG_MANAGER_INVALID_PAYMENT_ORDER_ID";
		/// <summary>つくーるAPI連携：ポイント発行時エラーメッセージ</summary>
		public const string ERRMSG_MANAGER_FAILED_TO_ADD_USER_POINT = "ERRMSG_MANAGER_FAILED_TO_ADD_USER_POINT";
		/// <summary>つくーるAPI連携：与信エラーメール送信エラーメッセージ</summary>
		public const string ERRMSG_MANAGER_AUTH_ERROR_MAIL_SEND_FAILED = "ERRMSG_MANAGER_AUTH_ERROR_MAIL_SEND_FAILED";
		/// <summary>つくーるAPI連携：定期購入時に定期フラグがOFFのエラーメッセージ</summary>
		public const string ERRMSG_MANAGER_PRODUCT_NOT_FIXEDPURCHASE_FLG = "ERRMSG_MANAGER_PRODUCT_NOT_FIXEDPURCHASE_FLG";
	}
}
