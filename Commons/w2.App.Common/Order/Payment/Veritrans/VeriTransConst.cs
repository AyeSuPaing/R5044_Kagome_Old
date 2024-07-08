/*
=========================================================================================================
  Module      : ベリトランス定数 (VeriTransConst.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Helper.Attribute;

namespace w2.App.Common.Order.Payment.Veritrans
{
	/// <summary>
	/// ベリトランス定数
	/// </summary>
	public static class VeriTransConst
	{
		/// <summary>結果コード：NG</summary>
		public const string RESULT_STATUS_NG = "failure";
		/// <summary>結果コード：OK</summary>
		public const string RESULT_STATUS_OK = "success";
		/// <summary>結果コード：ペンディング</summary>
		public const string RESULT_STATUS_PENDING = "pending";
		/// <summary>3Dセキュア：レスポンスコンテンツ</summary>
		public const string RESPONSE_CONTENTS = "responseContents";
		/// <summary>決済サービスオプションタイプ：完全認証</summary>
		public const string SERVICE_OPTION_TYPE_FULL_AUTH = "mpi-complete";
		/// <summary>HTTPユーザエージェント</summary>
		public const string USER_AGENT = "UserAgent";
		/// <summary>HTTP アクセプト</summary>
		public const string HTTP_ACCEPT = "httpAccept";
		/// <summary>リダイレクション URI</summary>
		public const string REDIRECTION_URI_VERITRANS = "VeriTransReturnUrl";
		/// <summary>売上フラグ：与信のみ</summary>
		public const string WITH_CAPTURE_ONLY_AUTH = "false";
		/// <summary>売上フラグ：与信と売上確定</summary>
		public const string WITH_CAPTURE_AND_AUTH = "true";
		/// <summary>一括支払</summary>
		public const string CREDIT_INSTALLMENTS_ONE_TIME_PAYMENT = "01";
		/// <summary>リポ払い</summary>
		public const string CREDIT_INSTALLMENTS_REVOLVING_PAYMENT = "99";
		/// <summary>PayTgレスポンス：カード有効期限_月</summary>
		public const string PAYTG_CARD_EXPIRE_MONTH = "cardExpireMonth";
		/// <summary>PayTgレスポンス：カード有効期限_年</summary>
		public const string PAYTG_CARD_EXPIRE_YEAR = "cardExpireYear";
		/// <summary>PayTgレスポンス：カード番号</summary>
		public const string PAYTG_CARD_NUMBER = "cardNumber";
		/// <summary>ベリトランス連携：設定ファイル名</summary>
		public const string VERITRANS_CREDIT_CONFIG_FILE_NAME = "CREDIT_3GPSMDK.ini";
		/// <summary>ベリトランス連携：設定ファイル名</summary>
		public const string VERITRANS_CVS_DEF_CONFIG_FILE_NAME = "CVS_DEF_3GPSMDK.ini";
		/// <summary>ベリトランス連携：設定ファイル名：PAYPAY</summary>
		public const string VERITRANS_PAYPAY_CONFIG_FILE_NAME = "PAYPAY_3GPSMDK.ini";
		/// <summary>ベリトランス連携：ログ設定ファイル名</summary>
		public const string LOG_CONFIG_FILE_NAME = "log4net.config";
		/// <summary>ベリトランス連携：ベースディレクトリ</summary>
		public const string VERSTRIAN_CONFIG_FILE_NAME_BASE = "base\\3GPSMDK.ini";
		/// <summary>ベリトランス連携：ログ設定ファイル名</summary>
		public const string LOG_CONFIG_FILE_NAME_BASE = "base\\log4net.config";
		/// <summary>ベリトランス連携：ログ出力先環境変数</summary>
		public const string VERITRANS_LOG_OUT_DIR_ENV = "LOG_OUT_DIR";
		/// <summary>PayTgレスポンスエラーメッセージ</summary>
		public const string PAYTG_RESPONSE_ERROR = "paytg_error";
		/// <summary>改行コード</summary>
		public const string NEWLINE_CHARACTER = "\r\n";
		/// <summary>決済種別：請求書別送</summary>
		public const string VERITRANS_PAYMENT_TYPE_SEPARATE_SERVICE = "2";
		/// <summary>決済種別：請求書同梱</summary>
		public const string VERITRANS_PAYMENT_TYPE_INCLUDE_SERVICE = "3";
		/// <summary>デバイスチャンネル：3Dセキュア2.0</summary>
		public const string VERITRANS_3DS_DEVICE_CHANNEL = "02";

		/// <summary>課金種別: 都度決済</summary>
		public const string ACCOUNTING_TYPE_ONETIME = "0";
		/// <summary>課金種別: 随時決済</summary>
		public const string ACCOUNTING_TYPE_ANYTIME = "1";
		/// <summary>Veritrans request key: order id</summary>
		public const string VERITRANS_REQUEST_KEY_ORDERID = "orderId";
		/// <summary>ベリトランスPaypayOrderID</summary>
		public const string VERITRANS_REQUEST_KEY_PAYPAY_ORDERID = "paypayOrderId";
		/// <summary>Veritrans request key: 詳細結果コード</summary>
		public const string VERITRANS_REQUEST_KEY_VRESULTCODE = "vResultCode";
		/// <summary>Veritrans request key: mstatus/summary>
		public const string VERITRANS_REQUEST_KEY_MSTATUS = "mstatus";
		/// <summary>Veritrans payment service option type online</summary>
		public const string VERITRANS_PAYMENT_SERVICE_OPTION_TYPE_ONLINE = "online";

		/// <summary>審査結果</summary>
		public enum VeritransAuthorResult
		{
			/// <summary>OK</summary>
			[EnumTextName("OK")] Ok,
			/// <summary>NG</summary>
			[EnumTextName("NG")] Ng,
			/// <summary>審査中</summary>
			[EnumTextName("HD")] Hold,
			/// <summary>保留</summary>
			[EnumTextName("HR")] Hr,
		}

		/// <summary>ベリトランス決済種別</summary>
		public enum VeritransPaymentKbn
		{
			/// <summary>クレジット</summary>
			Credit,
			/// <summary>後払い</summary>
			CvsDef,
			/// <summary>Paypay</summary>
			Paypay,
		}
	}
}
