/*
=========================================================================================================
  Module      : ペイジェント定数リスト(PaygentConstants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Helper.Attribute;
namespace w2.App.Common.Order.Payment.Paygent
{
	/// <summary>
	/// ペイジェント定数リスト
	/// </summary>
	public class PaygentConstants
	{
		#region 電文種別ID telegram_kind
		///<summary> カード決済オーソリ電文</summary>
		public const string PAYGENT_APITYPE_CARD_AUTH = "020";
		///<summary> カード決済オーソリキャンセル電文</summary>
		public const string PAYGENT_APITYPE_CARD_AUTH_CANCEL = "021";
		///<summary> カード決済売上電文</summary>
		public const string PAYGENT_APITYPE_CARD_REALSALE = "022";
		///<summary> カード決済売上キャンセル電文</summary>
		public const string PAYGENT_APITYPE_CARD_REALSALE_CANCEL = "023";
		///<summary> カード情報設定電文</summary>
		public const string PAYGENT_APITYPE_CARD_REGISTER = "025";
		///<summary> カード情報削除電文</summary>
		public const string PAYGENT_APITYPE_CARD_DELETE = "026";
		///<summary> EMV3Dセキュア認証電文</summary>
		public const string PAYGENT_APITYPE_CARD_3DSECURE_AUTH = "450";
		#endregion

		#region ペイジェントクレカ API応答パラメータ名
		/// <summary> API応答：実行結果</summary>
		public const string RESPONSE_STATUS = "resultStatus";
		/// <summary> API応答：異常終了時コード</summary>
		public const string RESPONSE_CODE = "responseCode";
		/// <summary> API応答：異常終了時詳細）</summary>
		public const string RESPONSE_DETAIL = "responseDetail";
		#endregion

		#region ペイジェントクレカ API実行結果
		/// <summary> API実行結果：成功</summary>
		public const string PAYGENT_RESPONSE_STATUS_SUCCESS = "0";
		/// <summary> API実行結果：失敗</summary>
		public const string PAYGENT_RESPONSE_STATUS_FAILURE = "1";
		#endregion

		#region ペイジェントクレカ 3Dセキュア認証後処理可否
		/// <summary> 3Dセキュア認証結果：OK</summary>
		public const string PAYGENT_3DSECURE_RESULT_OK = "3DSecureOK";
		/// <summary> 3Dセキュア認証結果：NG</summary>
		public const string PAYGENT_3DSECURE_RESULT_NG = "3DSecureNG";
		/// <summary> 3Dセキュア認証結果：タイムアウト</summary>
		public const string PAYGENT_3DSECURE_RESULT_TIMEOUT = "3DSecureTimeOut";
		#endregion

		#region ペイジェントクレカ APIリクエストパラメータ
		/// <summary> 支払い方法：一括払い</summary>
		public const string PAYGENT_PAYMENT_CLASS_FULL = "10";
		/// <summary> 支払い方法：分割払い</summary>
		public const string PAYGENT_PAYMENT_CLASS_INSTALLMENTS = "61";
		/// <summary> ログイン方法：加盟店システム固有の方法でログイン</summary>
		public const string PAYGENT_PAYMENT_LOGIN_TYPE_LOGGED_IN = "02";
		/// <summary> ログイン方法：未ログイン</summary>
		public const string PAYGENT_PAYMENT_LOGIN_TYPE_NOT_LOGGED_IN = "01";
		#endregion

		#region ペイジェントクレカ オーソリ実施区分
		/// <summary> attemptオーソリ実施区分：低</summary>
		public const string AUTHORIZATION_CONTROLKBN_LOW = "low";
		/// <summary> attemptオーソリ実施区分：中</summary>
		public const string AUTHORIZATION_CONTROLKBN_MIDDLE = "middle";
		/// <summary> attemptオーソリ実施区分：高</summary>
		public const string AUTHORIZATION_CONTROLKBN_HIGH = "high";
		#endregion

		#region レスポンスパラメータ名
		/// <summary> レスポンス：顧客カードID</summary>
		public const string CUSTOMER_CARD_ID = "customer_card_id";
		#endregion

		#region ペイジェント用セッションキー
		/// <summary> 注文フロー受注情報セッションキー</summary>
		public const string PAYGENT_SESSION_ORDER = "PaygentOrderInOrderProcess";
		/// <summary> 3Dセキュア与信結果セッションキー</summary>
		public const string PAYGENT_SESSION_AUTH_RESULT = "PaygentAuthResults";
		#endregion

		#region エラーコード
		/// <summary> ペイジェントAPIエラーコード</summary>
		public enum PaygentResponseErrorCode
		{
			/// <summary>認証パラメータ存在エラー</summary>
			[EnumTextName("P001")]
			P001,
			/// <summary>認証エラー</summary>
			[EnumTextName("P002")]
			P002,
			/// <summary>IFバージョンエラー</summary>
			[EnumTextName("P003")]
			P003,
			/// <summary>決済要求相違エラー</summary>
			[EnumTextName("P004")]
			P004,
			/// <summary>パラメータ存在エラー</summary>
			[EnumTextName("P005")]
			P005,
			/// <summary>必須エラー</summary>
			[EnumTextName("P006")]
			P006,
			/// <summary>未契約エラー</summary>
			[EnumTextName("P007")]
			P007,
			/// <summary>型エラー</summary>
			[EnumTextName("P008")]
			P008,
			/// <summary>桁数エラー</summary>
			[EnumTextName("P009")]
			P009,
			/// <summary>入力値エラー</summary>
			[EnumTextName("P010")]
			P010,
			/// <summary>利用限度額オーバー</summary>
			[EnumTextName("P011")]
			P011,
			/// <summary>決済金額下限エラー</summary>
			[EnumTextName("P014")]
			P014,
			/// <summary>不正接続エラー</summary>
			[EnumTextName("P015")]
			P015,
			/// <summary>クライアント証明書エラー</summary>
			[EnumTextName("P016")]
			P016,
			/// <summary>サービス停止中エラー</summary>
			[EnumTextName("P017")]
			P017,
			/// <summary>利用不可エラー</summary>
			[EnumTextName("P018")]
			P018,
			/// <summary>利用許可エラー</summary>
			[EnumTextName("P021")]
			P021,
			/// <summary>カード情報なしエラー</summary>
			[EnumTextName("P022")]
			P022,
			/// <summary>カード情報数上限エラー</summary>
			[EnumTextName("P023")]
			P023,
			/// <summary>利用許可エラー</summary>
			[EnumTextName("P024")]
			P024,
			/// <summary>カード情報なしエラー</summary>
			[EnumTextName("P025")]
			P026,
			/// <summary>支払区分エラー</summary>
			[EnumTextName("P030")]
			P030,
			/// <summary>利用許可エラー</summary>
			[EnumTextName("P063")]
			P063,
			/// <summary>ペイジェントシステムエラー</summary>
			[EnumTextName("E001")]
			E001,
			/// <summary>決済ベンダーアクセス数制限エラー</summary>
			[EnumTextName("E002")]
			E002,
			/// <summary>DBロックエラー</summary>
			[EnumTextName("9005")]
			N9005,
			/// <summary>オーソリエラー</summary>
			[EnumTextName("2001")]
			N2001,
			/// <summary>カード決済ベンダー側システムエラー</summary>
			[EnumTextName("2002")]
			N2002,
			/// <summary>カード情報入力エラー</summary>
			[EnumTextName("2003")]
			N2003,
			/// <summary>ステータス矛盾エラー</summary>
			[EnumTextName("2004")]
			N2004,
			/// <summary>取引ID重複エラー</summary>
			[EnumTextName("2005")]
			N2005,
			/// <summary>該当データなしエラー</summary>
			[EnumTextName("2006")]
			N2006,
			/// <summary>期限切れエラー</summary>
			[EnumTextName("2007")]
			N2007,
			/// <summary>タイムアウトエラー</summary>
			[EnumTextName("2008")]
			N2008,
			/// <summary>参照マーチャント取引IDエラー</summary>
			[EnumTextName("2009")]
			N2009,
			/// <summary>Attemptオーソリ実施拒否</summary>
			[EnumTextName("2015")]
			N2015,
			/// <summary>仕向カード会社判定エラー</summary>
			[EnumTextName("2016")]
			N2016,
			/// <summary>トークン有効期限切れエラー</summary>
			[EnumTextName("2020")]
			N2020,
			/// <summary>トークンなしエラー</summary>
			[EnumTextName("2021")]
			N2021,
			/// <summary>トークンワンタイムエラー</summary>
			[EnumTextName("2022")]
			N2022,
			/// <summary>トークン指定時他項目指定不可エラー</summary>
			[EnumTextName("2024")]
			N2024,
			/// <summary>該当データなしエラー</summary>
			[EnumTextName("13001")]
			N13001,
			/// <summary>取引ID重複エラー</summary>
			[EnumTextName("13002")]
			N13002,
			/// <summary>3Dセキュア認証失敗エラー</summary>
			[EnumTextName("31007")]
			N31007,
			/// <summary>3Dセキュア2.0認証結果指定エラー</summary>
			[EnumTextName("31008")]
			N31008,
			/// <summary>3Dセキュア2.0認証結果組み合わせエラー</summary>
			[EnumTextName("31009")]
			N31009,
			/// <summary>3Dセキュア2.0認証結果利用不可エラー</summary>
			[EnumTextName("31010")]
			N31010,
			/// <summary>3Dセキュア2.0認証情報なしエラー</summary>
			[EnumTextName("31011")]
			N31011,
			/// <summary>3Dセキュア未対応ブランドエラー</summary>
			[EnumTextName("31012")]
			N31012,
			/// <summary>認証タイムアウトエラー</summary>
			[EnumTextName("31013")]
			N31013,
		}
		#endregion

		/// <summary>処理結果: 正常</summary>
		public const string FLG_PAYGENT_API_RESPONSE_RESULT_NORMAL = "0";
		/// <summary>Paygent api hashcode error</summary>
		public const string PAYGENT_API_HASHCODE_ERROR = "00";

		/// <summary>Paygent telegram kind: Convenience store order register</summary>
		public const string PAYGENT_TELEGRAM_KIND_CONVENIENCE_STORE_ORDER_REGISTER = "030";

		/// <summary>キャンセル時のメッセージ</summary>
		public const string PAYGENT_PAIDY_CANCEL_LOG_MESSAGE = "Paidy決済キャンセル";
		/// <summary>返金時のメッセージ</summary>
		public const string PAYGENT_PAIDY_REFUND_LOG_MESSAGE = "Paidy決済返金";

		/// <summary>Content type</summary>
		public const string PAYGENT_API_REQUEST_CONTENT_TYPE = "Content-Type";
		/// <summary>Content type: form urlencoded</summary>
		public const string PAYGENT_API_REQUEST_CONTENT_TYPE_FORM_URLENCODED = "application/x-www-form-urlencoded";

		/// <summary>Flag paygent api response payment type: ATM</summary>
		public const string FLG_PAYGENT_API_RESPONSE_PAYMENT_TYPE_ATM = "01";
		/// <summary>Flag paygent api response payment type: Convenience store</summary>
		public const string FLG_PAYGENT_API_RESPONSE_PAYMENT_TYPE_CONVENIENCE_STORE = "03";
		/// <summary>Flag paygent api response payment type: Banknet</summary>
		public const string FLG_PAYGENT_API_RESPONSE_PAYMENT_TYPE_BANKNET = "05";
		/// <summary>Flag paygent api response payment type: Paidy</summary>
		public const string FLG_PAYGENT_API_RESPONSE_PAYMENT_TYPE_PAIDY = "22";
		/// <summary>コンビニ決済(番号方式)ステータス:申込済</summary>
		public const string PAYGENT_API_RESPONSE_PAYMENT_STATUS_APPLIED = "10";
		/// <summary>コンビニ決済(番号方式)ステータス:支払期限切</summary>
		public const string PAYGENT_API_RESPONSE_PAYMENT_STATUS_PERIOD_EXPIRED = "12";
		/// <summary>コンビニ決済(番号方式)ステータス:申込中断</summary>
		public const string PAYGENT_API_RESPONSE_PAYMENT_STATUS_APPLICATION_SUSPENDED = "15";
		/// <summary>コンビニ決済(番号方式)ステータス:オーソリOK</summary>
		public const string PAYGENT_API_RESPONSE_PAYMENT_STATUS_AUTH_COMP = "20";
		/// <summary>コンビニ決済(番号方式)ステータス:オーソリ取消済</summary>
		public const string PAYGENT_API_RESPONSE_PAYMENT_STATUS_AUTH_CANCEL = "32";
		/// <summary>コンビニ決済(番号方式)ステータス:オーソリ期限切</summary>
		public const string PAYGENT_API_RESPONSE_PAYMENT_STATUS_AUTH_EXPIRED = "33";
		/// <<summary>コンビニ決済(番号方式)ステータス:消込済</summary>
		public const string PAYGENT_API_RESPONSE_PAYMENT_STATUS_DELETED = "40";
		/// <<summary>コンビニ決済(番号方式)ステータス:消込済（売上取消期限切）</summary>
		public const string PAYGENT_API_RESPONSE_PAYMENT_STATUS_SALES_CANCEL_EXPIRED = "41";
		/// <summary>コンビニ決済(番号方式)ステータス:速報検知済</summary>
		public const string PAYGENT_API_RESPONSE_PAYMENT_STATUS_NOTICE_DETECTED = "43";
		/// <summary>コンビニ決済(番号方式)ステータス:売上取消済</summary>
		public const string PAYGENT_API_RESPONSE_PAYMENT_STATUS_SALES_CANCELED = "60";
		/// <summary>コンビニ決済(番号方式)ステータス:速報取消済</summary>
		public const string PAYGENT_API_RESPONSE_PAYMENT_STATUS_NOTICE_CANCELED = "61";

		/// <summary>エラーコード一覧:該当する決済情報が存在しません</summary>
		public const string PAYGENT_API_RESPONSE_STATUS_CODE_NO_MATCHING_PAYMENT_INFORMATION = "13001";

		/// <summary>Paygent telegram kind: ATM order register</summary>
		public const string PAYGENT_TELEGRAM_KIND_ATM_ORDER_REGISTER = "010";
		/// <summary>Paygent telegram kind: Banknet order register</summary>
		public const string PAYGENT_TELEGRAM_KIND_BANKNET_ORDER_REGISTER = "060";
		/// <summary>Paygent telegram kind: Paidy authorization cancellation</summary>
		public const string PAYGENT_TELEGRAM_KIND_PAIDY_AUTHORIZATION_CANCELLATION = "340";
		/// <summary>Paygent telegram kind: Paidy settlement</summary>
		public const string PAYGENT_TELEGRAM_KIND_PAIDY_SETTLEMENT = "341";
		/// <summary>Paygent telegram kind: Paidy refund</summary>
		public const string PAYGENT_TELEGRAM_KIND_PAIDY_REFUND = "342";
		/// <summary>Paygent telegram kind: Paidy authorization</summary>
		public const string PAYGENT_TELEGRAM_KIND_PAIDY_AUTHORIZATION = "343";

		/// <summary>マーチャントID</summary>
		public const string PAYGENT_API_REQUEST_MERCHANT_ID = "merchant_id";
		/// <summary>接続ID</summary>
		public const string PAYGENT_API_REQUEST_CONNECT_ID = "connect_id";
		/// <summary>接続パスワード</summary>
		public const string PAYGENT_API_REQUEST_CONNECT_PASSWORD = "connect_password";
		/// <summary>電文種別ID</summary>
		public const string PAYGENT_API_REQUEST_TELEGRAM_KIND = "telegram_kind";
		/// <summary>電文バージョン番号</summary>
		public const string PAYGENT_API_REQUEST_TELEGRAM_VERSION = "telegram_version";
		/// <summary>マーチャント取引ID</summary>
		public const string PAYGENT_API_REQUEST_TRADING_ID = "trading_id";
		/// <summary>決済ID</summary>
		public const string PAYGENT_API_REQUEST_PAYMENT_ID = "payment_id";
		/// <summary>決済金額</summary>
		public const string PAYGENT_API_REQUEST_PAYMENT_AMOUNT = "payment_amount";
		/// <summary>CVSタイプ</summary>
		public const string PAYGENT_API_REQUEST_CVS_TYPE = "cvs_type";
		/// <summary>利用者姓</summary>
		public const string PAYGENT_API_REQUEST_CUSTOMER_FAMILY_NAME = "customer_family_name";
		/// <summary>利用者名</summary>
		public const string PAYGENT_API_REQUEST_CUSTOMER_NAME = "customer_name";
		/// <summary>利用者姓カナ</summary>
		public const string PAYGENT_API_REQUEST_CUSTOMER_FAMILY_NAME_KANA = "customer_family_name_kana";
		/// <summary>利用者名カナ</summary>
		public const string PAYGENT_API_REQUEST_CUSTOMER_NAME_KANA = "customer_name_kana";
		/// <summary>利用者電話番号</summary>
		public const string PAYGENT_API_REQUEST_CUSTOMER_TEL = "customer_tel";
		/// <summary>支払期限日</summary>
		public const string PAYGENT_API_REQUEST_PAYMENT_LIMIT_DATE = "payment_limit_date";
		/// <summary>申込コンビニ企業CD</summary>
		public const string PAYGENT_API_REQUEST_CVS_COMPANY_ID = "cvs_company_id";
		/// <summary>支払種別</summary>
		public const string PAYGENT_API_REQUEST_SALES_TYPE = "sales_type";
		/// <summary>決済通知ID</summary>
		public const string PAYGENT_API_REQUEST_PAYMENT_NOTICE_ID = "payment_notice_id";
		/// <summary>変更日時</summary>
		public const string PAYGENT_API_REQUEST_CHANGE_DATE = "change_date";
		/// <summary>決済種別CD</summary>
		public const string PAYGENT_API_REQUEST_PAYMENT_TYPE = "payment_type";
		/// <summary>決済ステータス</summary>
		public const string PAYGENT_API_REQUEST_PAYMENT_STATUS = "payment_status";
		/// <summary>Paidy決済ID</summary>
		public const string PAYGENT_API_REQUEST_PAIDY_PAYMENT_ID = "paidy_payment_id";
		/// <summary>取引発生日時</summary>
		public const string PAYGENT_API_REQUEST_PAYMENT_INIT_DATE = "payment_init_date";
		/// <summary>オーソリ日時</summary>
		public const string PAYGENT_API_REQUEST_AUTHORIZED_DATE = "authorized_date";
		/// <summary>キャンセル日時</summary>
		public const string PAYGENT_API_REQUEST_CANCEL_DATE = "cancel_date";
		/// <summary>イベント</summary>
		public const string PAYGENT_API_REQUEST_EVENT = "event";
		/// <summary>イベント結果</summary>
		public const string PAYGENT_API_REQUEST_EVENT_RESULT = "event_result";
		/// <summary>エラーコード</summary>
		public const string PAYGENT_API_REQUEST_EVENT_ERROR_CODE = "event_error_code";
		/// <summary>ハッシュ値</summary>
		public const string PAYGENT_API_REQUEST_HC = "hc";
		/// <summary>返金額</summary>
		public const string PAYGENT_API_REQUEST_AMOUNT = "amount";
		/// <summary>サイトID</summary>
		public const string PAYGENT_API_REQUEST_SITE_ID = "site_id";
		/// <summary>購入日時</summary>
		public const string PAYGENT_API_REQUEST_PAYMENT_DATE = "payment_date";
		/// <summary>請求内容カナ</summary>
		public const string PAYGENT_API_REQUEST_CLAIM_KANA = "claim_kana";
		/// <summary>請求内容漢字</summary>
		public const string PAYGENT_API_REQUEST_CLAIM_KANJI = "claim_kanji";
		/// <summary>支払期間</summary>
		public const string PAYGENT_API_REQUEST_ASP_PAYMENT_TERM = "asp_payment_term";
		/// <summary>決済内容</summary>
		public const string PAYGENT_API_REQUEST_PAYMENT_DETAIL = "payment_detail";
		/// <summary>決済内容カナ</summary>
		public const string PAYGENT_API_REQUEST_PAYMENT_DETAIL_KANA = "payment_detail_kana";

		/// <summary>処理結果</summary>
		public const string PAYGENT_API_RESPONSE_RESULT = "result";
		/// <summary>レスポンスコード</summary>
		public const string PAYGENT_API_RESPONSE_CODE = "response_code";
		/// <summary>レスポンス詳細</summary>
		public const string PAYGENT_API_RESPONSE_DETAIL = "response_detail";
		/// <summary>決済ID</summary>
		public const string PAYGENT_API_RESPONSE_PAYMENT_ID = "payment_id";
		/// <summary>マーチャント取引ID</summary>
		public const string PAYGENT_API_RESPONSE_TRADING_ID = "trading_id";
		/// <summary>申込コンビニ企業CD</summary>
		public const string PAYGENT_API_RESPONSE_CVS_COMPANY_ID = "cvs_company_id";
		/// <summary>決済金額</summary>
		public const string PAYGENT_API_RESPONSE_PAYMENT_AMOUNT = "payment_amount";
		/// <summary>利用者電話番号</summary>
		public const string PAYGENT_API_RESPONSE_CUSTOMER_TEL = "customer_tel";
		/// <summary>サービス区分</summary>
		public const string PAYGENT_API_RESPONSE_SERVICE_TYPE = "service_type";
		/// <summary>キャンセル受信日時</summary>
		public const string PAYGENT_API_RESPONSE_CANCEL_DATE = "cancel_date";
		/// <summary>支払日時</summary>
		public const string PAYGENT_API_RESPONSE_PAYMENT_DATE = "payment_date";
		/// <summary>確定受信日時</summary>
		public const string PAYGENT_API_RESPONSE_CONFIRM_NOTICE_DATE = "confirm_notice_date";
		/// <summary>支払期限日</summary>
		public const string PAYGENT_API_RESPONSE_PAYMENT_LIMIT_DATE = "payment_limit_date";
		/// <summary>主券枚数</summary>
		public const string PAYGENT_API_RESPONSE_MAIN_TICKET_NUM = "main_ticket_num";
		/// <summary>取引発生日時</summary>
		public const string PAYGENT_API_RESPONSE_PAYMENT_INIT_DATE = "payment_init_date";
		/// <summary>現地支払日時</summary>
		public const string PAYGENT_API_RESPONSE_USER_PAYMENT_DATE = "user_payment_date";
		/// <summary>速報受信日時</summary>
		public const string PAYGENT_API_RESPONSE_EARLY_NOTICE_DATE = "early_notice_date";
		/// <summary>利用者姓ｶﾅ</summary>
		public const string PAYGENT_API_RESPONSE_CUSTOMER_FAMILY_NAME_KANA = "customer_family_name_kana";
		/// <summary>利用者名ｶﾅ</summary>
		public const string PAYGENT_API_RESPONSE_CUSTOMER_NAME_KANA = "customer_name_kana";
		/// <summary>利用者姓</summary>
		public const string PAYGENT_API_RESPONSE_CUSTOMER_FAMILY_NAME = "customer_family_name";
		/// <summary>利用者名</summary>
		public const string PAYGENT_API_RESPONSE_CUSTOMER_NAME = "customer_name";
		/// <summary>決済ステータス</summary>
		public const string PAYGENT_API_RESPONSE_PAYMENT_STATUS = "payment_status";
		/// <summary>決済種別CD</summary>
		public const string PAYGENT_API_RESPONSE_PAYMENT_TYPE = "payment_type";
		/// <summary>発券開始日</summary>
		public const string PAYGENT_API_RESPONSE_TICKET_START_DATE = "ticket_start_date";
		/// <summary>発券終了日</summary>
		public const string PAYGENT_API_RESPONSE_TICKET_END_DATE = "ticket_end_date";
		/// <summary>決済ベンダ受付番号</summary>
		public const string PAYGENT_API_RESPONSE_RECEIPT_NUMBER = "receipt_number";
		/// <summary>結果URL情報</summary>
		public const string PAYGENT_API_RESPONSE_RECEIPT_PRINT_URL = "receipt_print_url";
		/// <summary>チケット枚数</summary>
		public const string PAYGENT_API_RESPONSE_TICKET_NUM = "ticket_num";
		/// <summary>サイトID</summary>
		public const string PAYGENT_API_RESPONSE_SITE_ID = "site_id";
		/// <summary>副券枚数</summary>
		public const string PAYGENT_API_RESPONSE_SUB_TICKET_NUM = "sub_ticket_num";
		/// <summary>ASP画面有効期限</summary>
		public const string PAYGENT_API_RESPONSE_ASP_LIMIT_DATE = "asp_limit_date";
		/// <summary>支払発生期限</summary>
		public const string PAYGENT_API_RESPONSE_ASP_PAYMENT_LIMIT_DATE = "asp_payment_limit_date";
		/// <summary>銀行ネット決済ASP画面URL</summary>
		public const string PAYGENT_API_RESPONSE_ASP_URL = "asp_url";
		/// <summary>請求金額</summary>
		public const string PAYGENT_API_RESPONSE_AMOUNT = "amount";
		/// <summary>請求内容カナ</summary>
		public const string PAYGENT_API_RESPONSE_CLAIM_KANA = "claim_kana";
		/// <summary>請求内容漢字</summary>
		public const string PAYGENT_API_RESPONSE_CLAIM_KANJI = "claim_kanji";
		/// <summary>取引発生日時</summary>
		public const string PAYGENT_API_RESPONSE_TRADE_GENERATION_DATE = "trade_generation_date";
		/// <summary>決済照会結果</summary>
		public const string PAYGENT_API_RESPONSE_RETRIEVE_RESULT = "retrieve_result";
		/// <summary>通貨</summary>
		public const string PAYGENT_API_RESPONSE_CURRENCY = "currency";
		/// <summary>決済の説明</summary>
		public const string PAYGENT_API_RESPONSE_DESCRIPTION = "description";
		/// <summary>店舗名</summary>
		public const string PAYGENT_API_RESPONSE_STORE_NAME = "store_name";
		/// <summary>購入者_メールアドレス</summary>
		public const string PAYGENT_API_RESPONSE_BUYER_EMAIL = "buyer_email";
		/// <summary>購入者_氏名(感じ)</summary>
		public const string PAYGENT_API_RESPONSE_BUYER_NAME_KANJI = "buyer_name_kanji";
		/// <summary>購入者_氏名(カナ)</summary>
		public const string PAYGENT_API_RESPONSE_BUYER_NAME_KANA = "buyer_name_kana";
		/// <summary>購入者_電話番号</summary>
		public const string PAYGENT_API_RESPONSE_BUYER_PHONE = "buyer_phone";
		/// <summary>配送先_郵便番号</summary>
		public const string PAYGENT_API_RESPONSE_SHIPPING_ADDRESS_ZIP = "shipping_address_zip";
		/// <summary>配送先_都道府県</summary>
		public const string PAYGENT_API_RESPONSE_SHIPPING_ADDRESS_STATE = "shipping_address_state";
		/// <summary>配送先_市区町村</summary>
		public const string PAYGENT_API_RESPONSE_SHIPPING_ADDRESS_CITY = "shipping_address_city";
		/// <summary>配送先_番地</summary>
		public const string PAYGENT_API_RESPONSE_SHIPPING_ADDRESS_LINE2 = "shipping_address_line2";
		/// <summary> 配送先_建物名・部屋番号</summary>
		public const string PAYGENT_API_RESPONSE_SHIPPING_ADDRESS_LINE1 = "shipping_address_line1";
		/// <summary>注文_消費税</summary>
		public const string PAYGENT_API_RESPONSE_ORDER_TAX = "order_tax";
		/// <summary>注文_配送料</summary>
		public const string PAYGENT_API_RESPONSE_ORDER_SHIPPING = "order_shipping";
		/// <summary>注文商品_ID</summary>
		public const string PAYGENT_API_RESPONSE_ORDER_ITEMS_ID = "order_items_id";
		/// <summary>注文商品_商品名</summary>
		public const string PAYGENT_API_RESPONSE_ORDER_ITEMS_TITLE = "order_items_title";
		/// <summary>文商品_説明</summary>
		public const string PAYGENT_API_RESPONSE_ORDER_ITEMS_DESCRIPTION = "order_items_description";
		/// <summary>注文商品_単価</summary>
		public const string PAYGENT_API_RESPONSE_ORDER_ITEMS_UNIT_PRICE = "order_items_unit_price";
		/// <summary>注文商品_個数</summary>
		public const string PAYGENT_API_RESPONSE_ORDER_ITEMS_QUANTITY = "order_items_quantity";
		/// <summary>収納機関番号</summary>
		public const string PAYGENT_API_RESPONSE_PAY_CENTER_NUMBER = "pay_center_number";
		/// <summary>お客様番号</summary>
		public const string PAYGENT_API_RESPONSE_CUSTOMER_NUMBER = "customer_number";
		/// <summary>確認番号</summary>
		public const string PAYGENT_API_RESPONSE_CONF_NUMBER = "conf_number";

		#region + PaidyCheckoutパラメータ名
		/// <summary>決済総額</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_AMOUNT = "amount";
		/// <summary>通貨コード</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_CURRENCY = "currency";
		/// <summary>店舗名</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_STORE_NAME = "store_name";
		/// <summary>購入者情報</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_BUYER = "buyer";
		/// <summary>購入者履歴情報</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_BUYER_DATA = "buyer_data";
		/// <summary>注文情報</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_ORDER = "order";
		/// <summary>配送先情報</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_SHIPPING_ADDRESS = "shipping_address";
		/// <summary>注文概要</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_DESCRIPTION = "description";
		/// <summary>メールアドレス</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_EMAIL = "email";
		/// <summary>氏名(漢字)</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_NAME1 = "name1";
		/// <summary>氏名(カタカナ)</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_NAME2 = "name2";
		/// <summary>電話番号(ハイフン抜き)</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_PHONE = "phone";
		/// <summary>生年月日(YYY-MM-DD)</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_DOB = "dob";
		/// <summary>ユーザーID</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_USER_ID = "user_id";
		/// <summary>年齢</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_AGE = "age";
		/// <summary>作成経過日数</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_AGE_PLATFORM = "age_platform";
		/// <summary>アカウント登録日(YYY-MM-DD)</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_ACCOUNT_REGISTRATION_DATE = "account_registration_date";
		/// <summary>初回購入日(キャンセル・Paidy決済を除く)</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_DAYS_SINCE_FIRST_TRANSACTION = "days_since_first_transaction";
		/// <summary>LTV(キャンセル・Paidy決済を除く)</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_LTV = "ltv";
		/// <summary>注文回数(キャンセル・Paidy決済を除く)</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_ORDER_COUNT = "order_count";
		/// <summary>最終購入時の金額(キャンセル・Paidy決済を除く)</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_LAST_ORDER_AMOUNT = "last_order_amount";
		/// <summary>最終購入からの経過日数(キャンセル・Paidy決済を除く)</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_LAST_ORDER_AT = "last_order_at";
		/// <summary>最終購入日(YYY-MM-DD)</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_LAST_ORDER_DATE = "last_order_date";
		/// <summary>過去3ヶ月の合計購入金額</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_ORDER_AMOUNT_LAST3MONTHS = "order_amount_last3months";
		/// <summary>過去3ヶ月の合計注文数</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_ORDER_COUNT_LAST3MONTHS = "order_count_last3months";
		/// <summary>配送先住所</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_ADDITIONAL_SHIPPING_ADDRESSES = "additional_shipping_addresses";
		/// <summary>請求先住所</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_BILLING_ADDRESSES = "billing_address";
		/// <summary>配送先種別</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_DELIVERY_LOCN_TYPE = "delivery_locn_type";
		/// <summary>性別</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_GENDER = "gender";
		/// <summary>定期購入時の決済回数</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_SUBSCRIPTION_COUNTER = "subscription_counter";
		/// <summary>過去ユーザーが利用したことのある決済種別</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_PREVIOUS_PAYMENT_METHODS = "previous_payment_methods";
		/// <summary>ユーザーポイント</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_NUMBER_OF_POINTS = "number_of_points";
		/// <summary>商品カテゴリ</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_ORDER_ITEM_CATEGORIES = "order_item_categories";
		/// <summary>購入商品</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_ITEMS = "items";
		/// <summary>カートID or 注文ID</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_ORDER_REF = "order_ref";
		/// <summary>配送料</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_SHIPPING = "shipping";
		/// <summary>消費税額</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_TAX = "tax";
		/// <summary>建物名・部屋番号</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_LINE1 = "line1";
		/// <summary>番地</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_LINE2 = "line2";
		/// <summary>市区町村</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_CITY = "city";
		/// <summary>都道府県</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_STATE = "state";
		/// <summary>郵便番号(NNN-NNNN形式)</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_ZIP = "zip";
		/// <summary>商品ID</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_ID = "id";
		/// <summary>個数</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_QUANTITY = "quantity";
		/// <summary>商品名</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_TITLE = "title";
		/// <summary>商品単価</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_UNIT_PRICE = "unit_price";
		/// <summary>クレジット決済を利用したことがあるか</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_CREDIT_CARD_USED = "credit_card_used";
		/// <summary>代金引換を利用したことがあるか</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_CASH_ON_DELIVERY_USED = "cash_on_delivery_used";
		/// <summary>コンビニ決済を利用したことがあるか</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_CONVENIENCE_STORE_PREPAYMENT_USED = "convenience_store_prepayment_used";
		/// <summary>キャリア決済を利用したことがあるか</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_CARRIER_PAYMENT_USED = "carrier_payment_used";
		/// <summary>銀行振り込みを利用したことがあるか</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_BANK_TRANSFER_USED = "bank_transfer_used";
		/// <summary>楽天Payを利用したことがあるか</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_RAKUTEN_PAY_USED = "rakuten_pay_used";
		/// <summary>LINEPayを利用したことがあるか</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_LINE_PAY_USED = "line_pay_used";
		/// <summary>AmazonPayを利用したことがあるか</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_AMAZON_PAY_USED = "amazon_pay_used";
		/// <summary>NP後払いを利用したことがあるか</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_NP_POSTPAY_USED = "np_postpay_used";
		/// <summary>その他後払い決済を利用したことがあるか</summary>
		public const string PAIDY_CHECKOUT_RESPONSE_OTHER_POSTPAY_USED = "other_postpay_used";
		#endregion
	}
}
