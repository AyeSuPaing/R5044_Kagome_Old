/*
=========================================================================================================
  Module      : PayTg定数(PayTgConstants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
namespace w2.App.Common.Order.Payment.PayTg
{
	/// <summary>
	/// PayTg定数クラス
	/// </summary>
	public class PayTgConstants
	{
		/// <summary>PayTg端末連動結果</summary>
		public const string PARAM_COMRESULT = "comResult";
		/// <summary>処理結果ステータス</summary>
		public const string PARAM_MSTATUS = "mstatus";
		/// <summary>詳細結果コード</summary>
		public const string PARAM_VRESULTCODE = "vResultCode";
		/// <summary>決済エラーメッセージ</summary>
		public const string PARAM_ERRORMSG = "merrMsg";
		/// <summary>カスタム</summary>
		public const string PARAM_MERCHANTFREE1 = "merchantFree1";
		/// <summary>金額</summary>
		public const string PARAM_AMOUNT = "amount";
		/// <summary>支払区分</summary>
		public const string PARAM_JPO = "jpo";
		/// <summary>支払回数</summary>
		public const string PARAM_COUNT = "count";
		/// <summary>決済 GW エラーコード</summary>
		public const string PARAM_GWERRORCODE = "gwErrCd";
		/// <summary>決済 GW エラーメッセージ</summary>
		public const string PARAM_GWERRORMSG = "gwErrMsg";
		/// <summary>処理時間（トークン化）</summary>
		public const string PARAM_ORDERDATE = "orderDate";
		/// <summary>デジタル署名</summary>
		public const string PARAM_LINE1 = "line1";
		/// <summary>カードトークン</summary>
		public const string PARAM_TOKEN = "token";
		/// <summary>発行者識別番号</summary>
		public const string PARAM_TOP6 = "top6";
		/// <summary>カード番号下4桁</summary>
		public const string PARAM_LAST4 = "last4";
		/// <summary>有効期限月</summary>
		public const string PARAM_MCACNTNO1 = "mcAcntNo1";
		/// <summary>有効期限年</summary>
		public const string PARAM_EXPIRE = "expire";
		/// <summary>国際ブランド</summary>
		public const string PARAM_BRAND = "brand";
		/// <summary>イシュアコード</summary>
		public const string PARAM_ISSUERNAME = "issurName";
		/// <summary>カードタイプ</summary>
		public const string PARAM_KANACARDNAME = "kanaCardName";
		/// <summary>セキュリティコードトークン</summary>
		public const string PARAM_PROCESSPASS = "processPass";
		/// <summary>サービスID</summary>
		public const string PARAM_PROCESSID = "processId";
		/// <summary>顧客ID</summary>
		public const string PARAM_CUSTOMERID = "customerId";
		/// <summary>取引ID</summary>
		public const string PARAM_ORDERID = "orderId";
		/// <summary>会社コード</summary>
		public const string PARAM_ACQNAME = "acqName";
		/// <summary>リクエストID</summary>
		public const string PARAM_RESAUTHCODE = "resAuthCode";
		/// <summary>処理時間（決済リクエスト）</summary>
		public const string PARAM_TRANDATE = "tranDate";
		/// <summary>リファレンス</summary>
		public const string PARAM_LINE2 = "line2";
		/// <summary>カード分割払い</summary>
		public const string PARAM_PAYTIMES = "payTimes";
		/// <summary>ボーナス払い</summary>
		public const string PARAM_NAMEKANJI = "nameKanji";
		/// <summary>リボ払い</summary>
		public const string PARAM_NAMEKANA = "nameKana";
		/// <summary>楽天カード判定</summary>
		public const string PARAM_FORWARD = "forward";

		/// レスポンス
		/// <summary>電文受信日時</summary>
		public const string PARAM_REQUESTDATE = "requestDate";
		/// <summary>電文送信日時</summary>
		public const string PARAM_RESPONSEDATE = "responseDate";
		/// <summary>キーバージョン</summary>
		public const string PARAM_MCSECCD = "mcSecCd";
		/// <summary>カード有効期限_月</summary>
		public const string PAYTG_CARD_EXPIRE_MONTH = "cardExpireMonth";
		/// <summary>カード有効期限_年</summary>
		public const string PAYTG_CARD_EXPIRE_YEAR = "cardExpireYear";
		/// <summary>カード番号</summary>
		public const string PAYTG_CARD_NUMBER = "cardNumber";
		/// <summary>PayTgレスポンスエラーメッセージ</summary>
		public const string PAYTG_RESPONSE_ERROR = "paytg_error";
		/// <summary>カード番号</summary>
		public const string PARAM_CARD_NUMBER = "reqCardNumber";
		/// <summary>カード有効期限</summary>
		public const string PARAM_CARD_EXPIRE = "expire";
		/// <summary>値テキスト:注文</summary>
		public const string VALUETEXT_PARAM_ORDER = "order";
		/// <summary>値テキスト:ベリトランス結果メッセージ</summary>
		public const string VALUETEXT_PARAM_VERITRANS_COM_RESULT = "veritrans_com_result";
		/// <summary>値テキスト:PayTg結果メッセージ</summary>
		public const string VALUETEXT_PARAM_PAYTG_COM_RESULT = "paytg_com_result";
		/// <summary>実行結果ステータス</summary>
		public const string PAYTG_STATUS_SUCCESS = "success";
		/// <summary>新規カードか</summary>
		public const string FLG_PAYTG_CREDITCARD_BRANCH_NEW = "paytg_creditcard_branch_new";

		/// <summary>PayTg端末は利用可能か</summary>
		public const string REQUEST_KEY_CAN_USE_PAYTG_DEVICE = "cupd";
		/// <summary>PayTg端末は利用可能か：利用可能</summary>
		public const string REQUEST_VALUE_CAN_USE_PAYTG_DEVICE_VALID = "1";
		/// <summary>PayTg端末は利用可能か：利用不可能</summary>
		public const string REQUEST_VALUE_CAN_USE_PAYTG_DEVICE_INVALID = "0";
		/// <summary>レスポンス値：PayTgデバイス状態：利用可能</summary>
		public const string RESPONSE_CAN_USE_PAYTG_DEVICE_VALID = "true";
		/// <summary>レスポンス値：PayTgデバイス状態：利用不可能</summary>
		public const string RESPONSE_CAN_USE_PAYTG_DEVICE_INVALID = "false";
		/// <summary>レスポンス値：PayTgメッセージ：操作可能</summary>
		public const string RESPONSE_STATE_MESSAGE_PAYTG_DEVICE_VALID = "操作可能";
		/// <summary>レスポンス値：PayTgメッセージ：未接続</summary>
		public const string RESPONSE_STATE_MESSAGE_PAYTG_DEVICE_INVALID = "未接続";
		// <summary>エラーメッセージ定数：PayTg端末の処理失敗
		public const string ERRMSG_PAYTG_UNAVAILABLE = "paytg_unavailable";

		/// <summary>
		/// PayTg Constants 処理カテゴリ
		/// </summary>
		public class DealingTypes
		{
			/// <summary>処理カテゴリ：オーソリ</summary>
			public const string URL_AUTHORIZATION = "101";
			/// <summary>処理カテゴリ：オーソリ売上</summary>
			public const string URL_AUTHORIZATIONSALE = "102";
			/// <summary>処理カテゴリ：カードチェック</summary>
			public const string URL_CHECKCARD = "103";
			/// <summary>処理カテゴリ：カード情報/継続課金登録</summary>
			public const string URL_REGISTERCARD = "104";
			/// <summary>処理カテゴリ：カード情報更新</summary>
			public const string URL_MODIFYCARD = "105";
		}

		/// <summary>
		/// Rakuten Constants クレジット分割情報
		/// </summary>
		public class Jpo
		{
			/// <summary>取引区分：一括</summary>
			public const string ONCE = "10";
			/// <summary>取引区分：ボーナス一括</summary>
			public const string BONUS1 = "21";
			/// <summary>取引区分：リボ</summary>
			public const string REVO = "80";
			/// <summary>取引区分：分割払い</summary>
			public const string INSTALLMENTS = "61";
		}

		/// <summary>
		/// PayTg Constants 決済代行事業者略称
		/// </summary>
		public class PspShortName
		{
			/// <summary>楽天</summary>
			public const string RAKUTEN = "rakuten/";
		}
	}
}
