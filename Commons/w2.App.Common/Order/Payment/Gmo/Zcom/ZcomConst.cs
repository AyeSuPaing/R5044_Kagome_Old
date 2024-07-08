/*
=========================================================================================================
  Module      : Zcom定数 (ZcomConst.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order.Payment.GMO.Zcom
{
	/// <summary>
	/// Zcom定数
	/// </summary>
	public static class ZcomConst
	{
		/// <summary>結果コード：NG</summary>
		public const string RESULT_CODE_NG = "0";
		/// <summary>結果コード：OK</summary>
		public const string RESULT_CODE_OK = "1";
		/// <summary>結果コード：別URLへ遷移</summary>
		public const string RESULT_CODE_REDIRECT_URL = "5";
		/// <summary>結果コード：システムエラー</summary>
		public const string RESULT_CODE_SYSTEM_ERROR = "9";

		/// <summary>取引ステータス：未課金（未オーソリ）</summary>
		public const string STATUS_UNBILLED = "0";
		/// <summary>取引ステータス：課金済み（売上確定済み）</summary>
		public const string STATUS_BILLED = "1";
		/// <summary>取引ステータス：仮売上（オーソリ済み）</summary>
		public const string STATUS_PROVISIONAL_SALES = "5";
		/// <summary>取引ステータス：キャンセル</summary>
		public const string STATUS_CANCEL = "9";
		/// <summary>Status successful payment</summary>
		public const string STATUS_SUCCESSFUL_PAYMENT = "successful_payment";
		/// <summary>Status error payment</summary>
		public const string STATUS_ERROR_PAYMENT = "error_payment";
		/// <summary>Status back from payment</summary>
		public const string STATUS_BACK_FROM_PAYMENT = "back_from_payment";

		/// <summary>接続先URL</summary>
		public const string PARAM_ACCESS_URL = "access_url";
		/// <summary>モード</summary>
		public const string PARAM_MODE = "mode";
		/// <summary>トランザクションコードハッシュ</summary>
		public const string PARAM_TRANS_CODE_HASH = "trans_code_hash";
		/// <summary>決済コード</summary>
		public const string PARAM_PAYMENT_CODE = "payment_code";
		/// <summary>契約番号</summary>
		public const string PARAM_CONTRACT_CODE = "contract_code";
		/// <summary>トランザクションコード</summary>
		public const string PARAM_TRANS_CODE = "trans_code";
		/// <summary>注文番号</summary>
		public const string PARAM_ORDER_NUMBER = "order_number";
		/// <summary>ユーザーID</summary>
		public const string PARAM_USER_ID = "user_id";
		/// <summary>決済状態</summary>
		public const string PARAM_STATE = "state";
		/// <summary>エラーメッセージ</summary>
		public const string PARAM_ERR_DETAIL = "err_detail";
		/// <summary>エラーコード</summary>
		public const string PARAM_ERR_CODE = "err_code";
		/// <summary>バージョン</summary>
		public const string PARAM_VERSION = "version";
		/// <summary>文字コード</summary>
		public const string PARAM_CHARACTER_CODE = "character_code";
		/// <summary>処理区分</summary>
		public const string PARAM_PROCESS_CODE = "process_code";
		/// <summary>ユーザー氏名</summary>
		public const string PARAM_USER_NAME = "user_name";
		/// <summary>メールアドレス</summary>
		public const string PARAM_USER_MAIL_ADD = "user_mail_add";
		/// <summary>利用言語</summary>
		public const string PARAM_LANG_ID = "lang_id";
		/// <summary>商品コード</summary>
		public const string PARAM_ITEM_CODE = "item_code";
		/// <summary>商品名</summary>
		public const string PARAM_ITEM_NAME = "item_name";
		/// <summary>決済区分</summary>
		public const string PARAM_ST_CODE = "st_code";
		/// <summary>課金区分</summary>
		public const string PARAM_MISSION_CODE = "mission_code";
		/// <summary>通貨コード</summary>
		public const string PARAM_CURRENCY_ID = "currency_id";
		/// <summary>価格</summary>
		public const string PARAM_ITEM_PRICE = "item_price";
		/// <summary>予備1</summary>
		public const string PARAM_MEMO1 = "memo1";
		/// <summary>予備2</summary>
		public const string PARAM_MEMO2 = "memo2";
		/// <summary>注文日時</summary>
		public const string PARAM_RECEIPT_DATE = "receipt_date";
		/// <summary>クレジットカード番号</summary>
		public const string PARAM_CARD_NUMBER = "card_number";
		/// <summary>戻りURL</summary>
		public const string PARAM_BACK_URL = "back_url";
		/// <summary>エラー時URL</summary>
		public const string PARAM_ERR_URL = "err_url";
		/// <summary>決済完了時URL</summary>
		public const string PARAM_SUCCESS_URL = "success_url";
		/// <summary>Constant payment method credit card settlement</summary>
		public const string CONST_PAYMENT_METHOD_CREDIT_CARD_SETTLEMENT = "11";

		/// <summary>Content type xml</summary>
		public static string CONTENT_TYPE_XML = "appilication/xml";
	}
}
