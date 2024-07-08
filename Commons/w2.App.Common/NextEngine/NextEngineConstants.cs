/*
=========================================================================================================
  Module      : ネクストエンジン用定数定義クラス(NextEngineConstants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
namespace w2.App.Common.NextEngine
{
	/// <summary>
	///ネクストエンジン用定数定義クラス
	/// </summary>
	public static class NextEngineConstants
	{
		/// <summary>ログイン画面URL</summary>
		public const string LOGIN_URL = "https://base.next-engine.org/users/sign_in/";
		/// <summary>APIホスト</summary>
		public const string API_URL = "https://api.next-engine.org";

		/// <summary>アクセストークン取得APIエンドポイント</summary>
		public const string PATH_ACCESS_TOKEN_ENDPOINT = API_URL + "/api_neauth";
		/// <summary>ログインユーザー情報取得APIエンドポイント</summary>
		public const string PATH_LOGIN_USER_ENDPOINT = API_URL + "/api_v1_login_user/info";
		/// <summary>受注伝票検索APIエンドポイント</summary>
		public const string PATH_SEARCH_ORDER_ENDPOINT = API_URL + "/api_v1_receiveorder_base/search";
		/// <summary>受注一括登録パターン情報取得エンドポイント</summary>
		public const string PATH_GET_UPLOAD_PATTERN_ENDPOINT = API_URL + "/api_v1_receiveorder_uploadpattern/info";
		/// <summary>受注伝票アップロードエンドポイント</summary>
		public const string PATH_UPLOAD_ORDER_ENDPOINT = API_URL + "/api_v1_receiveorder_base/upload";
		/// <summary>受注伝票更新エンドポイント</summary>
		public const string PATH_UPDATE_ORDER_ENDPOINT = API_URL + "/api_v1_receiveorder_base/update";
		/// <summary>Bulk update endpoint</summary>
		public const string PATH_BULK_UPDATE_ORDER_ENDPOINT = API_URL + "/api_v1_receiveorder_base/bulkupdate";

		/// <summary>受注伝票アップロード用一時保存CSVファイルパス</summary>
		public static readonly string PATH_CSV_TMP_FOR_UPLOAD_ORDER_DATA = Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE + "/NextEngine/upload.csv";

		// リクエストパラメタキー
		/// <summary>UID</summary>
		public const string PARAM_UID = "uid";
		/// <summary>ステート</summary>
		public const string PARAM_STATE = "state";
		/// <summary>リダイレクトURI</summary>
		public const string PARAM_REDIRECT_URI = "redirect_uri";
		/// <summary>クライアントID</summary>
		public const string PARAM_CLIENT_ID = "client_id";
		/// <summary>クライアントシークレット</summary>
		public const string PARAM_CLIENT_SECRET = "client_secret";
		/// <summary>アクセストークン</summary>
		public const string PARAM_ACCESS_TOKEN = "access_token";
		/// <summary>リフレッシュトークン</summary>
		public const string PARAM_REFRESH_TOKEN = "refresh_token";
		/// <summary>待機フラグ</summary>
		public const string PARAM_WAIT_FLG = "wait_flag";
		/// <summary>フィールド</summary>
		public const string PARAM_FIELDS = "fields";
		/// <summary>受注一括登録パターンID</summary>
		public const string PARAM_PATTERN_ID = "receive_order_upload_pattern_id";
		/// <summary>受注データタイプ</summary>
		public const string PARAM_DATA_TYPE = "data_type_1";
		/// <summary>受注データ</summary>
		public const string PARAM_UPLOAD_ORDER_DATA = "data_1";
		/// <summary>受注番号IN検索</summary>
		public const string PARAM_SEARCH_ORDER_ORDER_ID_IN = "receive_order_shop_cut_form_id-in";
		/// <summary>受注番号</summary>
		public const string PARAM_SEARCH_ORDER_ORDER_ID= "receive_order_id";
		/// <summary>最終更新日</summary>
		public const string PARAM_SEARCH_ORDER_ORDER_LAST_MODIFIED_DATE = "receive_order_last_modified_date";
		/// <summary>更新データ</summary>
		public const string PARAM_SEARCH_ORDER_UPDATE_DATA = "data";
		/// <summary>受注キャンセルフラグ</summary>
		public const string PARAM_SEARCH_ORDER_CANCEL_UPDATE_FLG = "receive_order_row_cancel_update_flag";
		/// <summary>データファイル種類</summary>
		public const string PARAM_DATA_TYPE_FOR_BULK_UPDATE_ORDER = "data_type";
		/// <summary>Param data: Receive order cancel type id</summary>
		public const string PARAM_SEARCH_ORDER_CANCEL_TYPE_ID = "receive_order_cancel_type_id";
		/// <summary>Xml element: Receive order</summary>
		public const string PARAM_SEARCH_XML_ELEMENT_RECEIVEORDER = "receiveorder";
		/// <summary>Xml element: Receive order base</summary>
		public const string PARAM_SEARCH_XML_ELEMENT_RECEIVEORDER_BASE = "receiveorder_base";

		// レスポンスパラメタキー
		/// <summary>結果</summary>
		public const string RESPONSE_PARAM_RESULT = "result";
		/// <summary>メッセージコード</summary>
		public const string RESPONSE_PARAM_CODE = "code";
		/// <summary>メッセージ</summary>
		public const string RESPONSE_PARAM_MESSAGE = "message";
		/// <summary>検索結果件数</summary>
		public const string RESPONSE_PARAM_COUNT = "count";
		/// <summary>検索結果連想配列（JSONオブジェクト）</summary>
		public const string RESPONSE_PARAM_DATA = "data";
		/// <summary>アップロードキューID</summary>
		public const string RESPONSE_PARAM_QUE_ID = "que_id";
		/// <summary>アクセストークン有効期限切れ日時</summary>
		public const string RESPONSE_PARAM_ACCESS_TOKEN_END_DATE = "access_token_end_date";
		/// <summary>リフレッシュトークン有効期限切れ日時</summary>
		public const string RESPONSE_PARAM_REFRESH_TOKEN_END_DATE = "refresh_token_end_date";

		// リクエスト待機フラグ
		/// <summary>待機フラグ：:メイン機能過負荷でも可能な限りエラーにせず実行</summary>
		public const string FLG_WAIT_FLG_AVOID_FAILURE = "1";
		/// <summary>待機フラグ：メイン機能過負荷の場合、003002のエラーを返却</summary>
		public const string FLG_WAIT_FLG_IMMEDIATELY = "999";

		// リクエストデータタイプ
		/// <summary>受注データタイプ：csv</summary>
		public const string FLG_DATA_TYPE_CSV = "csv";
		/// <summary>受注データタイプ：GZIP</summary>
		public const string FLG_DATA_TYPE_GZIP = "gz";
		/// <summary>受注データタイプ：XML</summary>
		public const string FLG_DATA_TYPE_XML = "xml";

		// リクエスト受注データ：ギフトフラグ：無し（0）
		public const string FLG_UPLOAD_ORDER_DATA_IS_GIFT_INVALID = "0";
		// リクエスト受注データ：ギフトフラグ：有り（1）
		public const string FLG_UPLOAD_ORDER_DATA_IS_GIFT_VALID = "1";

		// レスポンス結果コード
		/// <summary>結果：成功</summary>
		public const string FLG_RESULT_SUCCESS = "success";
		/// <summary>結果：エラー</summary>
		public const string FLG_RESULT_ERROR = "error";
		/// <summary>結果：リダイレクト</summary>
		public const string FLG_RESULT_REDIRECT = "redirect";

		/// レスポンス受注一括登パターンID：フォーマットID：汎用
		public const string FLG_UPLOAD_PATTERN_FORMAT_ID_GENERAL = "90";

		/// レスポンス受注伝票検索：受注キャンセル区分ID：有効な受注(キャンセルは0以外)
		public const string FLG_SEARCH_ORDER_ORDER_CANCEL_TYPE_ID_VALID_ORDER = "0";
		/// <summary>レスポンス受注伝票更新:受注キャンセル区分ID：都合によるキャンセル</summary>
		public const string FLG_SEARCH_ORDER_ORDER_CANCEL_TYPE_ID_DUE_TO_CIRCUMSTANCES  = "5";

		/// レスポンス受注伝票検索：受注状態区分：出荷確定済（完了）
		public const string FLG_SEARCH_ORDER_ORDER_STATUS_ID_COMPLETED = "50";

		/// <summary>受注キャンセルフラグ:ON</summary>
		public const string FLG_SEARCH_ORDER_CANCEL_UPDATE_ON = "1";

		/// <summary>ネクストエンジン仮注文キャンセル失敗文言</summary>
		public const string ERROR_MESSAGE_FAIL_TMP_CNSL = "ネクストエンジン管理画面で注文が見つかりませんでした。";
		/// <summary>ネクストエンジン仮注文キャンセル失敗文言(管理者用)</summary>
		public const string ERROR_MESSAGE_FAIL_TMP_CNSL_FOR_ADMIN = "在庫にずれが生じている可能性があるため、ネクストエンジン管理画面より確認をお願いします。";
		/// <summary>ネクストエンジン仮注文キャンセル失敗ユーザー情報</summary>
		public const string ERROR_MESSAGE_FORMAT_TARGET = "注文ID：{0}　ユーザーID: {1}";

		/// フィールドリスト
		/// <summary>受注伝票アップロードリクエストCSVフィールドリスト</summary>
		public static readonly string[] FIELDS_UPLOAD_ORDER_DATA = {
			"店舗伝票番号","受注日","受注郵便番号","受注住所１","受注住所２","受注名","受注名カナ","受注電話番号","受注メールアドレス","発送郵便番号",
			"発送先住所１","発送先住所２","発送先名","発送先カナ","発送電話番号","支払方法","発送方法","商品計","税金","発送料",
			"手数料","ポイント","その他費用","合計金額","ギフトフラグ","時間帯指定","日付指定","作業者欄","備考","商品名",
			"商品コード","商品価格","受注数量","商品オプション","出荷済フラグ","顧客区分","顧客コード","消費税率（%）"
		};
		/// <summary>受注伝票検索取得フィールドリスト</summary>
		public static readonly string[] FIELDS_SEARCH_ORDER_RESPONSE =
		{
			"receive_order_id","receive_order_shop_cut_form_id","receive_order_cancel_type_id","receive_order_cancel_date",
			"receive_order_order_status_id","receive_order_order_status_name","receive_order_delivery_id",
			"receive_order_delivery_name","receive_order_delivery_cut_form_id","receive_order_send_date","receive_order_last_modified_date"
		};

		/// <summary>ネクストエンジン受注伝票検索API 検索可能最大受注件数</summary>
		public const int MAX_SEARCHABLE_ORDERS = 1000;
	}
}
