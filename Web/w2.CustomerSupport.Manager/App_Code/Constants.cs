/*
=========================================================================================================
  Module      : マネージャ定数定義(Constants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/

using w2.Domain.MenuAuthority.Helper;

public class Constants : w2.App.Common.Constants
{
	//*****************************************************************************************
	// パス・設定ファイル系
	//*****************************************************************************************
	//=========================================================================================
	// パス（global.asaxで設定）
	//=========================================================================================
	/// <summary>メニューXML物理パス</summary>
	public static string PHYSICALDIRPATH_MENU_XML = "";
	/// <summary>機能一覧XML物理パス</summary>
	public static string PHYSICALDIRPATH_PAGE_INDEX_LIST_XML = "";
	// メール配信オプション：絵文字画像ファイルパス
	public static string EMOJI_IMAGE_DIRPATH = "";
	// メール配信オプション：絵文字画像ファイルURL
	public static string EMOJI_IMAGE_URL = "";

	/// <summary>承認依頼時の承認方法のデフォルト選択</summary>
	public static string MESSAGEREQUEST_APPROVAL_TYPE_DEFAULT;
	/// <summary>メール署名挿入モード</summary>
	public enum MailSignatureInsertModeType
	{
		/// <summary>署名を先頭に追加</summary>
		Top,
		/// <summary>署名を終端に追加</summary>
		Bottom
	}
	/// <summary>メッセージ作成画面 メール署名挿入モード</summary>
	public static MailSignatureInsertModeType SETTING_MAILINPUT_MAIL_SIGNATURE_INSERT_MODE = MailSignatureInsertModeType.Top;
	/// <summary>集計区分：最大登録件数</summary>
	public static int MAX_SUMMARY_SETTING_COUNT = 10;
	/// <summary>エラーポイント表示閾値</summary>
	public static int DISPLAY_ERROR_POINT;
	/// <summary>検索画面デフォルト期間設定</summary>
	public static string SEARCH_PAGE_FIRSTVIEW_TIMESPAN_SETTING;
	/// <summary>CSTOP一覧：1ページあたりのデフォルト表示件数</summary>
	public static int CONST_DISP_CONTENTS_CSTOP_LIST = 100;
	/// <summary>CS TOPグループ選択ドロップダウンリスト表示項目 マイグループ</summary>
	public static string TEXT_DDLGROUPLIST_MYGROUP = string.Empty;
	/// <summary>CS TOPグループ選択ドロップダウンリスト表示項目 未設定</summary>
	public static string TEXT_DDLGROUPLIST_NOT_SET = string.Empty;

	//*****************************************************************************************
	// パッケージ固有設定
	//*****************************************************************************************
	// アプリケーション表示名
	public const string APPLICATION_NAME_DISP = "w2CustomerSupport";
	
	/// <summary>マネージャサイトタイプ</summary>
	public static readonly MenuAuthorityHelper.ManagerSiteType ManagerSiteType = MenuAuthorityHelper.ManagerSiteType.Cs;
	/// <summary>メニュー権限フィールド名（店舗オペレータマスタの利用フィールド名）</summary>
	public static string FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL_PKG = MenuAuthorityHelper.GetOperatorMenuAccessLevelFieldName(ManagerSiteType);

	//*****************************************************************************************
	// 区分・マネージャサイド
	//*****************************************************************************************

	// Incident message download
	public const int KBN_MENU_FUNCTION_INCIDENT_MESSAGE_DL = 1;

	// オペレータ一覧ソート区分
	public const string KBN_SORT_OPERATOR_LIST_NAME_ASC = "0";				// オペレータ名/昇順
	public const string KBN_SORT_OPERATOR_LIST_NAME_DESC = "1";				// オペレータ名/降順
	public const string KBN_SORT_OPERATOR_LIST_ID_ASC = "8";				// オペレータID/昇順
	public const string KBN_SORT_OPERATOR_LIST_ID_DESC = "9";				// オペレータID/降順
	public const string KBN_SORT_OPERATOR_LIST_DISPLAY_ORDER_ASC = "2";		// CSオペレータ表示順/昇順
	public const string KBN_SORT_OPERATOR_LIST_DISPLAY_ORDER_DESC = "3";	// CSオペレータ表示順/降順

	// 共有情報一覧既読区分
	public const string KBN_READ_SHAREINFO_LIST_READING = "reading";		// 未確認/ピン止め
	public const string KBN_READ_SHAREINFO_LIST_UNREAD = "unread";			// 未確認
	public const string KBN_READ_SHAREINFO_LIST_ALL = "all";				// すべて表示
	public const string KBN_READ_SHAREINFO_LIST_DEFAULT = KBN_READ_SHAREINFO_LIST_READING;

	// 共有情報一覧ソート区分
	public const string KBN_SORT_SHAREINFO_LIST_DATE_CREATED_ASC = "0";		// 作成日/昇順
	public const string KBN_SORT_SHAREINFO_LIST_DATE_CREATED_DESC = "1";	// 作成日/降順
	public const string KBN_SORT_SHAREINFO_LIST_IMPORTANCE_ASC = "2";		// 重要度/昇順
	public const string KBN_SORT_SHAREINFO_LIST_IMPORTANCE_DESC = "3";		// 重要度/降順
	public const string KBN_SORT_SHAREINFO_LIST_DEFAULT = KBN_SORT_SHAREINFO_LIST_DATE_CREATED_DESC;

	// CSトップ：メッセージ媒体区分
	public const string KBN_MESSAGE_MEDIA_MODE_TEL = "tel";				// メッセージ媒体区分：電話
	public const string KBN_MESSAGE_MEDIA_MODE_MAIL = "mail";			// メッセージ媒体区分：メール

	// CSトップ：編集区分
	public const string KBN_MESSAGE_EDIT_MODE_NEW = "new";						// 編集区分：新規
	public const string KBN_MESSAGE_EDIT_MODE_REPLY = "reply";					// 編集区分：返信
	public const string KBN_MESSAGE_EDIT_MODE_EDIT_DRAFT = "editdraft";			// 編集区分：下書き編集
	public const string KBN_MESSAGE_EDIT_MODE_EDIT_FOR_SEND = "editforsend";	// 編集区分：代理送信編集
	public const string KBN_MESSAGE_EDIT_MODE_EDIT_DONE = "editdone";			// 編集区分：完了メッセージ編集

	// CSトップ：メールアクション
	public const string KBN_MAILACTION_APPROVE_CANCEL = "apprcsl";	// メールアクション：承認依頼取り下げ
	public const string KBN_MAILACTION_APPROVE_OK = "approk";		// メールアクション：承認OK
	public const string KBN_MAILACTION_APPROVE_OK_SEND = "approksend";		// メールアクション：承認OKからの送信
	public const string KBN_MAILACTION_APPROVE_NG = "apprng";		// メールアクション：承認NG
	public const string KBN_MAILACTION_SEND_CANCEL = "sendcsl";		// メールアクション：送信依頼取り下げ
	public const string KBN_MAILACTION_SEND_OK = "sendok";			// メールアクション：送信OK
	public const string KBN_MAILACTION_SEND_NG = "sendng";			// メールアクション：送信NG
	public const string KBN_MAILACTION_SEND = "send";				// メールアクション：送信
	public const string KBN_MAILACTION_APPROVE_REQ = "apprreq";		// メールアクション：承認依頼
	public const string KBN_MAILACTION_SEND_REQ = "sendreq";		// メールアクション：送信依頼

	// 親ページリロード区分
	public const string KBN_RELOAD_PARENT_WINDOW_ON = "1"; // リロードする
	public const string KBN_RELOAD_PARENT_WINDOW_OFF = ""; // リロードしない

	//*****************************************************************************************
	// パス・設定ファイル系
	//*****************************************************************************************
	//=========================================================================================
	// 設定XML系
	//=========================================================================================
	// 設定Xmlルート（外部からアクセスできないようにします。）
	public const string PATH_SETTING_XML = "Xml/";

	// 管理メニューセッティング
	public const string FILE_XML_MENU = "Xml/Menu.xml";
	/// <summary>機能一覧セッティング</summary>
	public const string FILE_XML_PAGE_INDEX_LIST = "Xml/PageIndexList.xml";

	//*****************************************************************************************
	// キー系
	//*****************************************************************************************
	//=========================================================================================
	// セッション変数キー
	//=========================================================================================
	public const string SESSION_KEY_LOGIN_OPERTOR_MENU = "w2csMng_loggedin_operator_menu";
	public const string SESSION_KEY_LOGIN_OPERTOR_MENU_ACCESS_LEVEL = "w2csMng_loggedin_operator_menu_access_level";
	public const string SESSION_KEY_LOGIN_OPERTOR_CS_GROUP_IDS = "w2csMng_loggedin_operator_cs_grouop_ids";
	public const string SESSION_KEY_LOGIN_OPERTOR_CS_GINFO = "w2csMng_loggedin_operator_cs_info";
	public const string SESSION_KEY_REDICRECT_NEXT_URL = "w2csMng_redirect_next_url";

	public const string SESSION_KEY_ACTION_STATUS = "w2csMng_action_status";
	public const string SESSION_KEY_LOGIN_ERROR_INFO = "w2csMng_loggedin_error_info";	// ログイン失敗回数保持用
	public const string SESSION_KEY_ORDER_REGIST_INPUT_ERROR = "w2csMng_order_regist_input_error";	// 連続書き込みエラー

	public const string SESSION_KEY_MENUAUTHORITY_LMENUS = "menu_auth_lmenus";		// メニュー権限情報（大メニュー）
	public const string SESSION_KEY_MENUAUTHORITY_INFO = "menu_auth_info";		// メニュー権限情報（メニュー権限情報）
	public const string SESSION_KEY_OPERATOR_INFO = "operator_info";			// オペレータ情報
	public const string SESSION_KEY_CSOPERATOR_INFO = "cs_operator_info";		// CSオペレータ情報

	// CS系
	public const string SESSION_KEY_CS_SEARCH_PARAMETER = "w2csMng_SearchParameter";	// 検索パラメタ
	public const string SESSION_KEY_CS_SEARCH_TOP_PARAMETER = "w2csMng_SearchTopParameter";				// Search top parameter
	public const string SESSION_KEY_INCIDENTCATEGORY_INFO = "incidentcategory_info";					// インシデントカテゴリ情報
	public const string SESSION_KEY_INCIDENTVOC_INFO = "incidentvoc_info";								// インシデントVOC情報
	/// <summary>インシデント警告アイコン</summary>
	public const string SESSION_KEY_INCIDENT_WARNING_ICON = "incident_warning_icon";
	public const string SESSION_KEY_SUMMARYSETTING_INFO = "summarysetting_info";						// 集計区分情報
	public const string SESSION_KEY_SUMMARYSETTINGITEM_INFO = "summarysettingitem_info";				// 集計区分アイテム情報
	public const string SESSION_KEY_ANSWERTEMPLATE_INFO = "answertemplate_info";						// 回答例情報
	public const string SESSION_KEY_EXTERNALLINKPREFERENCE_INFO = "externallinkpreference_info";		// 外部リンク情報
	public const string SESSION_KEY_ANSWERTEMPLATECATEGORY_INFO = "answertemplatecategory_info";		// 回答例カテゴリ情報
	public const string SESSION_KEY_SHAREINFO_INFO = "shareinfo_info";									// 共有情報
	public const string SESSION_KEY_SHAREINFO_SEARCH_INFO = "shareinfo_search_info";					// 共有情報検索情報
	public const string SESSION_KEY_SHAREINFOREAD_INFO = "shareinforead_info";							// 共有情報既読管理情報
	public const string SESSION_KEY_MAILASSIGNSETTING_INFO = "mailassignsetting_info";					// メール振分設定情報
	public const string SESSION_KEY_MAILSIGNATURE_INFO = "mailsignature_info";							// メール署名情報
	public const string SESSION_KEY_MAILFROM_INFO = "mailfrom_info";									// メール送信元情報
	public const string SESSION_KEY_OPERATORAUTHORITY_INFO = "operator_auth_info";						// オペレータ権限情報
	public const string SESSION_KEY_DDLOPERATORLIST_SELECTED = "ddloperatorlist_selected";				// オペレータドロップダウンリスト選択値
	public const string SESSION_KEY_DDLGROUPLIST_SELECTED = "ddlgrouplist_selected";					// グループドロップダウンリスト選択値

	//=========================================================================================
	// リクエストキー
	//=========================================================================================
	// 共通
	public const string REQUEST_KEY_RELOAD_PARENT_WINDOW = "pnt_win_rld";			// 親画面リロード
	public const string REQUEST_KEY_MENUAUTHORITY_LEVEL = "mauth_level";			// メニュー権限
	public const string REQUEST_KEY_ERROR_STATUS = "error_status";
	public const string REQUEST_KEY_ERRORPAGE_MANAGER_ERRORKBN = "errkbn";			// エラー区分

	// w2Commerce
	public const string REQUEST_KEY_ORDER_ID = "order_id";
	public const string REQUEST_KEY_FIXED_PURCHASE_ID = "fpid";
	public const string REQUEST_KEY_USER_KBN = "user_kbn";

	//------------------------------------------------------
	// CS系
	//------------------------------------------------------
	// CSトップ
	public const string REQUEST_KEY_MESSAGE_MEDIA_MODE = "msg_mode";	// メッセージ媒体区分
	public const string REQUEST_KEY_MESSAGE_EDIT_MODE = "editmode";		// 編集区分
	public const string REQUEST_KEY_MAILACTION = "mailaction";			// メールアクション

	public const string REQUEST_KEY_CS_TOPPAGE_KBN = "pagekbn";			// CSページ区分
	public const string REQUEST_KEY_CS_TASKSTATUS_MODE = "statusmode";	// CSステータスモード

	public const string REQUEST_KEY_INCIDENT_ID = "incid";	// インシデントID
	public const string REQUEST_KEY_MESSAGE_NO = "msgno";	// メッセージNO

	public const string REQUEST_KEY_REQUEST_NO = "reqno";	// メッセージ依頼NO
	public const string REQUEST_KEY_BRANCH_NO = "brchno";	// メッセージ依頼枝番

	public const string REQUEST_KEY_MAIL_ID = "mailid";	// メールID
	public const string REQUEST_KEY_FILE_NO = "fileno";	// ファイルNO

	public const string REQUEST_KEY_MAILSENDLOG_LOG_NO = "msllno";	// メール送信ログNo

	public const string REQUEST_KEY_CS_SEARCH = "search";			// Search (Top)
	public const string REQUEST_KEY_CS_SEARCH_KEYWORD = "skeyword";	// Keyword

	// 回答例文章管理
	public const string REQUEST_KEY_ANSWERTEMPLATE_TITLE = "ati";		// タイトル
	/// <summary>件名</summary>
	public const string REQUEST_KEY_ANSWERTEMPLATE_MAIL_TITLE = "amti";
	public const string REQUEST_KEY_ANSWERTEMPLATE_TEXT = "ate";		// 本文
	public const string REQUEST_KEY_ANSWERTEMPLATE_VALID_FLG = "avf";	// 有効フラグ

	// 外部リンク設定用
	public const string REQUEST_KEY_EXTERNAL_LINK_PERFERENCE_TITLE = "eti";		// タイトル
	/// <summary>件名</summary>
	public const string REQUEST_KEY_EXTERNAL_LINK_PERFERENCE_VALID_FLG = "evf";			// 有効フラグ

	// 共有情報管理
	public const string REQUEST_KEY_SHAREINFO_TEXT = "it";				// テキスト
	public const string REQUEST_KEY_SHAREINFO_KBN = "ik";				// 区分
	public const string REQUEST_KEY_SHAREINFO_IMPORTANCE = "im";		// 重要度
	public const string REQUEST_KEY_SHAREINFO_SENDER = "oi";			// 送信元オペレータID
	public const string REQUEST_KEY_SHAREINFO_READKBN = "rt";			// 既読区分

	// オペレータ所属設定
	public const string REQUEST_KEY_CS_GROUP_ID = "csgid";				// CSグループID

	// 
	public const string REQUEST_KEY_INCIDENT_CATEGORY_ID = "catid";						// インシデントカテゴリID
	public const string REQUEST_KEY_VOC_ID = "voc_id";									// VOC ID
	public const string REQUEST_KEY_SUMMARY_SETTING_NO = "summary_setting_no";			// 集計区分番号
	public const string REQUEST_KEY_MAIL_SIGNATURE_ID = "signature_id";					// メール署名番号
	public const string REQUEST_KEY_MAIL_FROM_ID = "from_id";							// メール送信元番号
	public const string REQUEST_KEY_ANSWER_TEMPLATE_ID = "answer_id";					// 回答例番号
	public const string REQUEST_KEY_EXTERNAL_LINK_PERFERENCE_LINK_ID = "link_id";		// リンクID
	public const string REQUEST_KEY_INFO_NO = "info_no";								// 共有情報No
	public const string REQUEST_KEY_MAIL_ASSIGN_ID = "assign_id";						// メール振分設定ID
	public const string REQUEST_KEY_CSGROUP_ID = "csgroup_id";							// CSグループID
	public const string REQUEST_KEY_OPERATOR_AUTHORITY_ID = "ope_auth_id";				// オペレータ権限ID

	public const string REQUEST_KEY_OPERATOR_NAME = "operator_name";			// オペレータ名
	public const string REQUEST_KEY_OPERATOR_MAIL_ADDR = "mail_addr";			// オペレータメールアドレス
	public const string REQUEST_KEY_OPERATOR_IS_CS_OPERATOR = "is_cs";			// オペレータ有効フラグ
	public const string REQUEST_KEY_OPERATOR_VALID_FLG = "ovf";			// オペレータ有効フラグ
	public const string REQUEST_KEY_OPERATOR_MENUACCESS_LEVEL = "mal";			// メニュー権限

	public const string REQUEST_KEY_WINDOW_TYPE = "wt";							// ウィンドウタイプ(通常,ポップアップ)

	public const string REQUEST_KEY_CUSTOMER_TELNO = "tel";						// CTI連携用のパラメータ

	//*****************************************************************************************
	// その他
	//*****************************************************************************************
	public const int VIEW_LIST_COUNT = 200;			// オペレータ管理一覧取得レコード数
	public const string ACTION_STATUS_DEFAULTSETTING = "defaultsetting";	// 商品初期設定
	public const string ACTION_STATUS_UPDATE = "update";		// 変更
	public const string ACTION_STATUS_INSERT = "insert";		// 登録
	public const string ACTION_STATUS_COPY_INSERT = "copyinsert";	// コピー登録
	public const string ACTION_STATUS_DELETE = "delete";		// 削除
	public const string ACTION_STATUS_IMPORT = "import";		// 取込（マスタ取込）
	public const string ACTION_STATUS_PARENT_INSERT = "parentinsert";	// 親カテゴリ登録
	public const string ACTION_STATUS_CHILD_INSERT = "childinsert";	// 子カテゴリ登録
	public const string ACTION_STATUS_CONFIRM = "confirm";	// 確認
	public const string ACTION_STATUS_COMPLETE = "complete";	// 完了
	public const string ACTION_STATUS_EXPORT = "export";		// エクスポート
	public const string ERROR_REQUEST_PRAMETER = "err_parameter";	// 不正パラメータ
	public const string DEFAULT_SHIPPING_PRICE = "1000";			// 初期表示配送料金
	public static string STRING_SUPERUSER_NAME = "";	// スーパーユーザー名
	public static string STRING_UNACCESSABLEUSER_NAME = "";	// アクセス権限なしユーザ名
	/// <summary>リターンフラグtrue</summary>
	public const string RETURN_FLAG_TRUE = "true";

	/// <summary>トップ画面 インシデント一覧表示枠高さ</summary>
	public const string COOKIE_KEY_TOPPAGE_INCIDENT_LIST_HEIGHT = "toppage_incident_list_height";
	/// <summary>トップ画面 メッセージタブ表示枠高さ</summary>
	public const string COOKIE_KEY_TOPPAGE_MESSAGE_HEIGHT = "toppage_message_height";
	/// <summary>トップ画面 インシデントタブ一覧表示枠高さ</summary>
	public const string COOKIE_KEY_TOPPAGE_INCIDENT_HEIGHT = "toppage_incident_height";
	/// <summary>メール作成画面 顧客検索結果表示枠高さ</summary>
	public const string COOKIE_KEY_MAIL_USER_SEARCH_HEIGHT = "mail_search_height";
	/// <summary>メール作成画面 表示枠高さ</summary>
	public const string COOKIE_KEY_MESSAGE_INPUT_HEIGHT = "message_input_height";
	/// <summary>受信時振分けルール設定画面 ルール表示枠高さ</summary>
	public const string COOKIE_KEY_MAIL_ASSIGN_SETTING_LIST_HEIGHT = "mail_assign_setting_list_height";
	/// <summary>電話問合せ画面 顧客検索結果表示枠高さ</summary>
	public const string COOKIE_KEY_USER_SEARCH_HEIGHT = "user_search_height";

	/// <summary>トップ画面 インシデント一覧表示枠高さデフォルトサイズ</summary>
	public const string TOPPAGE_INCIDENT_LIST_DEFAULT_HEIGHT_SIZE = "209";
	/// <summary>トップ画面 メッセージタブ表示枠高さデフォルトサイズ</summary>
	public const string TOPPAGE_MESSAGE_DEFAULT_HEIGHT_SIZE = "220";
	/// <summary>トップ画面 インシデントタブ一覧表示枠高さデフォルトサイズ</summary>
	public const string TOPPAGE_INCIDENT_DEFAULT_HEIGHT_SIZE = "160";
	/// <summary>メール作成画面 顧客検索結果表示枠高さデフォルトサイズ</summary>
	public const string MAIL_USER_SEARCH_DEFAULT_HEIGHT_SIZE = "302";
	/// <summary>メール作成画面 表示枠高さデフォルトサイズ</summary>
	public const string MESSAGE_INPUT_DEFAULT_HEIGHT_SIZE = "460";
	/// <summary>受信時振分けルール設定画面 ルール表示枠高さデフォルトサイズ</summary>
	public const string MAIL_ASSIGN_SETTING_LIST_DEFAULT_HEIGHT_SIZE = "300";
	/// <summary>電話問合せ画面 顧客検索結果表示枠高さデフォルトサイズ</summary>
	public const string USER_SEARCH_DEFAULT_HEIGHT_SIZE = "160";
	/// <summary>サイズ変更要素最大サイズ</summary>
	public const string VARIABLE_MAXIMUM_SIZE = "1000";

	/// <summary>Value text param message input</summary>
	public const string VALUETEXT_PARAM_MESSAGE_INPUT = "MessageInput";
	/// <summary>Value text param user list count max</summary>
	public const string VALUETEXT_PARAM_USER_LIST_COUNT_MAX = "user_list_count_max";
	/// <summary>Value text param by inside</summary>
	public const string VALUETEXT_PARAM_BY_INSIDE = "by_inside";
	/// <summary>Value text param approval request</summary>
	public const string VALUETEXT_PARAM_APPROVAL_REQUEST = "approval_request";
	/// <summary>Value text param send request</summary>
	public const string VALUETEXT_PARAM_SEND_REQUEST = "send_request";
	/// <summary>Value text param send preview</summary>
	public const string VALUETEXT_PARAM_SEND_PREVIEW = "send_preview";
	/// <summary>Value text param recognizer</summary>
	public const string VALUETEXT_PARAM_RECOGNIZER = "recognizer";
	/// <summary>Value text param sender</summary>
	public const string VALUETEXT_PARAM_SENDER = "sender";

	/// <summary>Value text param user search</summary>
	public const string VALUETEXT_PARAM_USER_SEARCH = "UserSearch";
	/// <summary>Value text param user</summary>
	public const string VALUETEXT_PARAM_USER = "user";
	/// <summary>Value text param customer</summary>
	public const string VALUETEXT_PARAM_CUSTOMER = "customer";
	/// <summary>Value text param guest</summary>
	public const string VALUETEXT_PARAM_GUEST = "guest";
	/// <summary>Value text param unaggregated</summary>
	public const string VALUETEXT_PARAM_UNAGGREGATED = "unaggregated";
	/// <summary>Value text param details</summary>
	public const string VALUETEXT_PARAM_DETAILS = "details";
	/// <summary>Value text param total</summary>
	public const string VALUETEXT_PARAM_TOTAL = "total";
	/// <summary>Value text param number of purchases final next time</summary>
	public const string VALUETEXT_PARAM_NUMBER_OF_PURCHASES_FINAL_NEXT_TIME = "number_of_purchases_final_next_time";
	/// <summary>Value text param other products</summary>
	public const string VALUETEXT_PARAM_OTHER_PRODUCTS = "other_products";
	/// <summary>Value text param author</summary>
	public const string VALUETEXT_PARAM_AUTHOR = "author";
	/// <summary>Value text param dm code name validity period</summary>
	public const string VALUETEXT_PARAM_DM_CODE_NAME_VALIDITY_PERIOD = "dm_code_name_validity_period";
	/// <summary>出荷予定日</summary>
	public const string VALUETEXT_PARAM_SCHEDULED_SHIPPING_DATE = "scheduled_shipping_date";
	/// <summary>指定なし</summary>
	public const string VALUETEXT_PARAM_UNSPECIFIED = "unspecified";

	/// <summary>Value text param operator confirm</summary>
	public const string VALUETEXT_PARAM_OPERATOR_CONFIRM = "OperatorConfirm";
	/// <summary>Value text param time string</summary>
	public const string VALUETEXT_PARAM_TIME_STRING = "time_string";
	/// <summary>Value text param hours minutes</summary>
	public const string VALUETEXT_PARAM_HOURS_MINUTES = "hours_minutes";
	/// <summary>Value text param marks</summary>
	public const string VALUETEXT_PARAM_MARKS = "marks";

	/// <summary>Value text param incident report</summary>
	public const string VALUETEXT_PARAM_INCIDENTREPORT = "IncidentReport";
	/// <summary>Value text param aggregate category</summary>
	public const string VALUETEXT_PARAM_AGGREGATE_CATEGORY = "aggregate_category";
	/// <summary>Value text param month</summary>
	public const string VALUETEXT_PARAM_MONTH = "month";

	/// <summary>Value text param summary setting register</summary>
	public const string VALUETEXT_PARAM_SUMMARY_SETTING_REGISTER = "SummarySettingRegister";
	/// <summary>Value text param item input check</summary>
	public const string VALUETEXT_PARAM_ITEM_INPUT_CHECK = "item_input_check";
	/// <summary>Value text param value save</summary>
	public const string VALUETEXT_PARAM_VALUE_SAVE = "value_save";

	/// <summary>Value text param top page</summary>
	public const string VALUETEXT_PARAM_TOP_PAGE = "TopPage";
	/// <summary>Value text param group list</summary>
	public const string VALUETEXT_PARAM_GROUP_LIST = "group_list";
	/// <summary>Value text param my group</summary>
	public const string VALUETEXT_PARAM_MY_GROUP = "my_group";
	/// <summary>Value text param not set</summary>
	public const string VALUETEXT_PARAM_NOT_SET = "not_set";

	/// <summary>Value text param operator left menu</summary>
	public const string VALUETEXT_PARAM_OPERATOR_LEFT_MENU = "left_menu";
	/// <summary>Value text param operator menu large</summary>
	public const string VALUETEXT_PARAM_OPERATOR_MENU_LARGE = "menu_large";
	/// <summary>Value text param operator menu small</summary>
	public const string VALUETEXT_PARAM_OPERATOR_MENU_SMALL = "menu_small";
	/// <summary>Value text param operator menu large incident</summary>
	public const string VALUETEXT_PARAM_OPERATOR_MENU_LARGE_INCIDENT = "menu_large_incident";
	/// <summary>Value text param operator menu large message</summary>
	public const string VALUETEXT_PARAM_OPERATOR_MENU_LARGE_MESSAGE = "menu_large_message";
	/// <summary>Value text param operator menu large approval</summary>
	public const string VALUETEXT_PARAM_OPERATOR_MENU_LARGE_APPROVAL = "menu_large_approval";
	/// <summary>Value text param operator menu large send</summary>
	public const string VALUETEXT_PARAM_OPERATOR_MENU_LARGE_SEND = "menu_large_send";
	/// <summary>Value text param operator menu large search</summary>
	public const string VALUETEXT_PARAM_OPERATOR_MENU_LARGE_SEARCH = "menu_large_search";
	/// <summary>Value text param operator menu large trash</summary>
	public const string VALUETEXT_PARAM_OPERATOR_MENU_LARGE_TRASH = "menu_large_trash";
	/// <summary>Value text param operator menu small incident none</summary>
	public const string VALUETEXT_PARAM_OPERATOR_MENU_SMALL_INCIDENT_NONE = "menu_small_incident_none";
	/// <summary>Value text param operator menu small incident active</summary>
	public const string VALUETEXT_PARAM_OPERATOR_MENU_SMALL_INCIDENT_ACTIVE = "menu_small_incident_active";
	/// <summary>Value text param operator menu small incident suspend</summary>
	public const string VALUETEXT_PARAM_OPERATOR_MENU_SMALL_INCIDENT_SUSPEND = "menu_small_incident_suspend";
	/// <summary>Value text param operator menu small incident urgent</summary>
	public const string VALUETEXT_PARAM_OPERATOR_MENU_SMALL_INCIDENT_URGENT = "menu_small_incident_urgent";
	/// <summary>Value text param operator menu small draft</summary>
	public const string VALUETEXT_PARAM_OPERATOR_MENU_SMALL_DRAFT = "menu_small_draft";
	/// <summary>Value text param operator menu small message reply</summary>
	public const string VALUETEXT_PARAM_OPERATOR_MENU_SMALL_MESSAGE_REPLY = "menu_small_message_reply";
	/// <summary>Value text param operator menu small approval</summary>
	public const string VALUETEXT_PARAM_OPERATOR_MENU_SMALL_APPROVAL = "menu_small_approval";
	/// <summary>Value text param operator menu small request</summary>
	public const string VALUETEXT_PARAM_OPERATOR_MENU_SMALL_REQUEST = "menu_small_request";
	/// <summary>Value text param operator menu small result</summary>
	public const string VALUETEXT_PARAM_OPERATOR_MENU_SMALL_RESULT = "menu_small_result";
	/// <summary>Value text param operator menu small send</summary>
	public const string VALUETEXT_PARAM_OPERATOR_MENU_SMALL_SEND = "menu_small_send";
	/// <summary>Value text param operator menu small search message</summary>
	public const string VALUETEXT_PARAM_OPERATOR_MENU_SMALL_SEARCH_MESSAGE = "menu_small_search_message";
	/// <summary>Value text param operator menu small trash incident</summary>
	public const string VALUETEXT_PARAM_OPERATOR_MENU_SMALL_TRASH_INCIDENT = "menu_small_trash_incident";
	/// <summary>Value text param operator menu small trash message</summary>
	public const string VALUETEXT_PARAM_OPERATOR_MENU_SMALL_TRASH_MESSAGE = "menu_small_trash_message";
	/// <summary>Value text param operator menu small trash request</summary>
	public const string VALUETEXT_PARAM_OPERATOR_MENU_SMALL_TRASH_REQUEST = "menu_small_trash_request";
	/// <summary>Value text param message print</summary>
	public const string VALUETEXT_PARAM_MESSAGE_PRINT = "message_print";
	/// <summary>Value text param message print media direction</summary>
	public const string VALUETEXT_PARAM_MESSAGE_PRINT_MEDIA_DIRECTION = "media_direction";
	/// <summary>Value text param message print inquiry reply date</summary>
	public const string VALUETEXT_PARAM_MESSAGE_PRINT_INQUIRY_REPLY_DATE = "inquiry_reply_date";
	/// <summary>Value text param message print message status name</summary>
	public const string VALUETEXT_PARAM_MESSAGE_PRINT_MESSAGE_STATUS_NAME = "message_status_name";
	/// <summary>Value text param message print file name</summary>
	public const string VALUETEXT_PARAM_MESSAGE_FILE_NAME = "file_name";
	/// <summary>Value text param message print user name</summary>
	public const string VALUETEXT_PARAM_MESSAGE_USER_NAME = "user_name";
	/// <summary>Value text param message print name kana</summary>
	public const string VALUETEXT_PARAM_MESSAGE_NAME_KANA = "name_kana";
	/// <summary>Value text param message print mail address</summary>
	public const string VALUETEXT_PARAM_MESSAGE_MAIL_ADDRESS = "mail_address";
	/// <summary>Value text param message print phone number</summary>
	public const string VALUETEXT_PARAM_MESSAGE_PHONE_NUMBER = "phone_number";
	/// <summary>Value text param message print inquiry title</summary>
	public const string VALUETEXT_PARAM_MESSAGE_INQUIRY_TITLE = "inquiry_title";
	/// <summary>Value text param message print inquiry text</summary>
	public const string VALUETEXT_PARAM_MESSAGE_INQUIRY_TEXT = "inquiry_text";
	/// <summary>Value text param message print reply text</summary>
	public const string VALUETEXT_PARAM_MESSAGE_REPLY_TEXT = "reply_text";

	/// <summary>Value text param message right mail</summary>
	public const string VALUETEXT_PARAM_MESSAGE_RIGHT_MAIL = "MessageRightMail";
	/// <summary>Value text param mail from</summary>
	public const string VALUETEXT_PARAM_MAIL_FROM = "mail_from";
	/// <summary>Value text param source unavailable</summary>
	public const string VALUETEXT_PARAM_SOURCE_UNAVAILABLE = "source_unavailable";

	/// <summary>Value text param mail signature</summary>
	public const string VALUETEXT_PARAM_MAIL_SIGNATURE = "MailSignature";
	/// <summary>Value text param owner id</summary>
	public const string VALUETEXT_PARAM_OWNER_ID = "owner_id";
	/// <summary>Value text param common</summary>
	public const string VALUETEXT_PARAM_COMMON = "common";

	/// <summary>Value text param search part</summary>
	public const string VALUETEXT_PARAM_SEARCH_PART = "SearchPart";
	/// <summary>Value text param search model list</summary>
	public const string VALUETEXT_PARAM_SEARCH_MODEL_LIST = "search_model_list";
	/// <summary>Value text param message unit</summary>
	public const string VALUETEXT_PARAM_MESSAGE_UNIT = "message_unit";
	/// <summary>Value text param incident unit</summary>
	public const string VALUETEXT_PARAM_INCIDENT_UNIT = "incident_unit";
	/// <summary>Value text param including all</summary>
	public const string VALUETEXT_PARAM_INCLUDING_ALL = "including_all";
	/// <summary>Value text param including any</summary>
	public const string VALUETEXT_PARAM_INCLUDING_ANY = "including_any";
	/// <summary>Value text param perfect matching</summary>
	public const string VALUETEXT_PARAM_PERFECT_MATCHING = "perfect_matching";
	/// <summary>Value text param subject</summary>
	public const string VALUETEXT_PARAM_SUBJECT = "subject";
	/// <summary>Value text param title</summary>
	public const string VALUETEXT_PARAM_TITLE = "title";
	/// <summary>Value text param memo</summary>
	public const string VALUETEXT_PARAM_MEMO = "memo";
	/// <summary>Value text param internal memo</summary>
	public const string VALUETEXT_PARAM_INTERNAL_MEMO = "internal_memo";
	/// <summary>Value text param inquirer name contact information</summary>
	public const string VALUETEXT_PARAM_INQUIRER_NAME_CONTACT_INFORMATION = "inquirer_name_contact_information";
	/// <summary>Value text param incoming mail</summary>
	public const string VALUETEXT_PARAM_INCOMING_MAIL = "incoming_mail";
	/// <summary>Value text param send mail</summary>
	public const string VALUETEXT_PARAM_SEND_MAIL = "send_mail";
	/// <summary>Value text param receive call</summary>
	public const string VALUETEXT_PARAM_RECEIVE_CALL = "receive_call";
	/// <summary>Value text param make call</summary>
	public const string VALUETEXT_PARAM_MAKE_CALL = "make_call";
	/// <summary>Value text param other reception</summary>
	public const string VALUETEXT_PARAM_OTHER_RECEPTION = "other_reception";
	/// <summary>Value text param other outgoing</summary>
	public const string VALUETEXT_PARAM_OTHER_OUTGOING = "other_outgoing";

	/// <summary>Value text param incident</summary>
	public const string VALUETEXT_PARAM_INCIDENT = "Incident";
	/// <summary>Value text param search update text</summary>
	public const string VALUETEXT_PARAM_UPDATE_TEXT = "update_text";
	/// <summary>Value text param not update</summary>
	public const string VALUETEXT_PARAM_NOT_UPDATE = "not_update";
	/// <summary>Value text param update empty</summary>
	public const string VALUETEXT_PARAM_UPDATE_EMPTY = "update_empty";

	/// <summary>Value text param data migration manager</summary>
	public const string VALUETEXT_PARAM_DATA_MIGRATION_MANAGER = "DataMigrationManager";
	/// <summary>Value text param credit mode text</summary>
	public const string VALUETEXT_PARAM_CREDIT_MODE_TEXT = "credit_mode";
}

