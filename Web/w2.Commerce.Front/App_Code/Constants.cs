/*
=========================================================================================================
  Module      : 定数/設定値を管理する
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/

/// <summary>
/// 定数/設定値を管理する
/// </summary>
public class Constants : w2.App.Common.Constants
{
	//========================================================================
	// 各種設定（設定ファイルより読み出しを行い、アプリケーション毎に設定）
	//========================================================================
	//------------------------------------------------------
	// アプリケーション設定
	//------------------------------------------------------
	/// <summary>モバイルサイトパス</summary>
	public static string PATH_MOBILESITE = null;
	/// <summary>セッションクッキー名</summary>
	public static string SESSION_COOKIE_NAME = null;

	//------------------------------------------------------
	// セキュリティ設定
	//------------------------------------------------------
	// ログイン後HTTP許可設定
	public static bool ALLOW_HTTP_AFTER_LOGGEDIN = false;

	// セッションクッキーセキュア設定
	public static bool SESSIONCOOKIE_SECURE_ENABLED = false;

	//------------------------------------------------------
	// パフォーマンス設定
	//------------------------------------------------------
	/// <summary>パフォーマンス：商品一覧キャッシング時間（分。「0」はキャッシングしない）</summary>
	public static int PRODUCTLIST_CACHE_EXPIRE_MINUTES = 0;
	/// <summary>パフォーマンス：コーディネート系一覧キャッシング時間（分。「0」はキャッシングしない）</summary>
	public static int COORDINATELIST_CACHE_EXPIRE_MINUTES = 0;

	/// <summary>パフォーマンス：商品ランキングキャッシング時間（分。「0」はキャッシングしない）</summary>
	public static int PRODUCT_RANKING_CACHE_EXPIRE_MINUTES = 0;

	/// <summary>パフォーマンス：おすすめ商品キャッシング時間（分。「0」はキャッシングしない）</summary>
	public static int PRODUCT_RECOMMEND_CACHE_EXPIRE_MINUTES = 0;

	/// <summary>パフォーマンス：おすすめ商品(詳細)キャッシング時間（分。「0」はキャッシングしない））</summary>
	public static int PRODUCT_RECOMMEND_ADVANCED_CACHE_EXPIRE_MINUTES = 0;

	//------------------------------------------------------
	// マイページ表示設定
	//------------------------------------------------------
	/// <summary>受信メール履歴表示</summary>
	public static bool MYPAGE_RECIEVEMAIL_HISTORY_DISPLAY = false;
	/// <summary>マイページ：会員ランクアップ表示</summary>
	public static bool MYPAGE_MEMBERRANKUP_DISPLAY = false;
	/// <summary>マイページ：スケジュール実行が１回のみの会員ランクアップ表示</summary>
	public static bool MYPAGE_SCHEDULE_KBN_ONCE_MEMBERRANKUP_DISPLAY = false;

	//========================================================================
	// 各種定数
	//========================================================================
	// 商品：アイコン表示フラグ（配列）
	public static string[] IMG_FRONT_PRODUCT_ICON = { IMG_FRONT_PRODUCT_ICON1, IMG_FRONT_PRODUCT_ICON2, IMG_FRONT_PRODUCT_ICON3, IMG_FRONT_PRODUCT_ICON4, IMG_FRONT_PRODUCT_ICON5, IMG_FRONT_PRODUCT_ICON6, IMG_FRONT_PRODUCT_ICON7, IMG_FRONT_PRODUCT_ICON8, IMG_FRONT_PRODUCT_ICON9, IMG_FRONT_PRODUCT_ICON10 };

	// エラーメッセージ用CSSクラス
	public const string CONST_INPUT_ERROR_CSS_CLASS_STRING = " error_input ";	//（先頭にスペースが必要）
	// エラーメッセージ用XMLヘッダ（
	public const string CONST_INPUT_ERROR_XML_HEADER = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>";

	//========================================================================
	// 各区分値
	//========================================================================
	// サイト遷移（スマートフォン用）
	public const string KBN_REQUEST_CHANGESITE_PC = "pc";				// PCサイト遷移
	public const string KBN_REQUEST_CHANGESITE_SMARTPHONE = "sp";				// SPサイト遷移

	// ログインページ表示区分
	public const string KBN_REQUEST_LOGINPAGE_LOGINACTION = "1";		// ログインページにてログインを行う
	// エラーページ表示区分(REQUEST_KEY_ERRORPAGE_KBNの値)
	public const string KBN_REQUEST_ERRORPAGE_GOTOP = "2";				// 「トップページへ」ボタンのエラーページ
	public const string KBN_REQUEST_ERRORPAGE_GOCART = "3";				// 「買い物かご」ボタンのエラーページ
	// 入荷通知メール処理区分
	public const string KBN_REQUEST_USERPRODUCTARRIVALMAIL_REGIST = "1";	// 入荷通知メール登録
	public const string KBN_REQUEST_USERPRODUCTARRIVALMAIL_DELETE = "2";	// 入荷通知メール削除
	// ポイントオプション区分
	public const string KBN_W2MP_POINT_OPTION_ENABLED_VALID = "1";		// ポイントオプション有効
	public const string KBN_W2MP_POINT_OPTION_ENABLED_INVALID = "0";	// ポイントオプション無効
	// アクセスログ区分
	public const string KBN_W2MP_ACCESSLOG_ENABLED_VALID = "1";			// アクセスログ有効
	public const string KBN_W2MP_ACCESSLOG_ENABLED_INVALID = "0";		// アクセスログ無効
	// アクセスログステータス
	public const string KBN_W2MP_ACCESSLOG_STATUS_LOGIN = "1";			// ログイン
	public const string KBN_W2MP_ACCESSLOG_STATUS_LEAVE = "2";			// 退会
	// 商品特殊一覧
	public const string KBN_DISP_PRODUCTLISTPARTICULAR_RANKING = "01";		// ランキング
	public const string KBN_DISP_PRODUCTLISTPARTICULAR_RECOMMEND1 = "02";	// おすすめ１
	public const string KBN_DISP_PRODUCTLISTPARTICULAR_RECOMMEND2 = "03";	// おすすめ２
	// 商品セール一覧
	public const string KBN_PRODUCTSALE_KBN_TIMESALES = "TS";			// タイムセールス
	public const string KBN_PRODUCTSALE_KBN_CLOSEDMARKET = "CM";		// 闇市

	// カート投入後画面遷移
	/// <summary>カート投入後画面遷移：カート一覧画面へ</summary>
	public const string KBN_REDIRECT_AFTER_ADDPRODUCT_CARTLIST = "CART";

	// 定期遷移画面区分
	/// <summary>定期解約画面へ</summary>
	public const string KBN_FIXED_PURCHASE_NEXT_PAGE_KBN_CANCEL = "cancle";	// 解約画面
	/// <summary>定期休止画面へ</summary>
	public const string KBN_FIXED_PURCHASE_NEXT_PAGE_KBN_SUSPEND = "suspend";	// 休止画面

	//========================================================================
	// パス定数
	//========================================================================
	//------------------------------------------------------
	// 設定XML系
	//------------------------------------------------------
	// 設定Xmlルート（外部からアクセスできないようにします。）
	public const string PATH_SETTING_XML = "Xml/";

	//------------------------------------------------------
	// コンテンツ系
	//------------------------------------------------------
	public const string PATH_FRONT_CATEGORY_HTML = "Contents/Html/Category/";
	public const string PAGE_FRONT_CATEGORY_DEFAULT_HTML = PATH_FRONT_CATEGORY_HTML + "Default.html";
	public const string PATH_FRONT_BRAND_HTML = "Contents/Html/Brand/";
	public const string PAGE_FRONT_BRAND_DEFAULT_HTML_FILE = "Default.html";

	//------------------------------------------------------
	// 画像パス系
	//------------------------------------------------------
	public const string IMG_FRONT_PRODUCT_ICON1 = "Contents/ProductImages/ProductIcon1.gif";
	public const string IMG_FRONT_PRODUCT_ICON2 = "Contents/ProductImages/ProductIcon2.gif";
	public const string IMG_FRONT_PRODUCT_ICON3 = "Contents/ProductImages/ProductIcon3.gif";
	public const string IMG_FRONT_PRODUCT_ICON4 = "Contents/ProductImages/ProductIcon4.gif";
	public const string IMG_FRONT_PRODUCT_ICON5 = "Contents/ProductImages/ProductIcon5.gif";
	public const string IMG_FRONT_PRODUCT_ICON6 = "Contents/ProductImages/ProductIcon6.gif";
	public const string IMG_FRONT_PRODUCT_ICON7 = "Contents/ProductImages/ProductIcon7.gif";
	public const string IMG_FRONT_PRODUCT_ICON8 = "Contents/ProductImages/ProductIcon8.gif";
	public const string IMG_FRONT_PRODUCT_ICON9 = "Contents/ProductImages/ProductIcon9.gif";
	public const string IMG_FRONT_PRODUCT_ICON10 = "Contents/ProductImages/ProductIcon10.gif";

	//========================================================================
	// リクエストキー
	//========================================================================
	// 共通
	new public static string REQUEST_KEY_SHOP_ID = "shop";				// 店舗ID
	public const string REQUEST_KEY_CHANGESITE = "changesite";			// サイト遷移（スマートフォン用）

	// 注文ページ
	public const string REQUEST_KEY_ADD_RECOMMEND_ITEM_FLG = "ariflg";		// レコメンド商品投入フラグ

	// エラーページ
	public const string REQUEST_KEY_ERRORPAGE_KBN = "errpage";		// エラーページ区分(1:トップページへ 2:閉じる OTHER:戻る)
	public const string REQUEST_KEY_ERRORPAGE_FRONT_ERRORKBN = "errkbn";		// エラー区分

	// ログインページ
	public const string REQUEST_KEY_NEXT_URL = "nurl";				// 遷移後URL
	public const string REQUEST_KEY_LOGIN_FLG = "lginflg";			// ログインフラグ（新セッション移行時のSecureフラグ制御用）
	/// <summary>リクエストキー：仮ユーザーID</summary>
	public const string REQUEST_KEY_TEMPORARY_USER_ID = "tmp_user_id";

	// アドレス帳ページ
	public const string REQUEST_KEY_SHIPPING_NO = "sno";			// 配送先枝番

	// 共通：商品系
	new public const string REQUEST_KEY_CATEGORY_ID = "cat";			// カテゴリID
	new public const string REQUEST_KEY_SEARCH_WORD = "swrd";			// 検索ワード
	new public const string REQUEST_KEY_PRODUCT_ID = "pid";				// 商品ID
	new public const string REQUEST_KEY_VARIATION_ID = "vid";			// 商品バリエーションID
	/// <summary>注文数</summary>
	public const string REQUEST_KEY_ITEM_QUANTITY = "iq";
	/// <summary>頒布会検索ワード</summary>
	public const string REQUEST_KEY_SUBSCRIPTION_BOX_SEARCH_WORD = "sbswrd";
	/// <summary>コースリストから遷移したかポストバックか判定用</summary>
	public const string REQUEST_KEY_FROM_LIST_OR_POSTBACK = "flop";
	public const string REQUEST_KEY_FAV_TOKEN = "fav_tkn";				// お気に入りトークン
	public const string REQUEST_KEY_LIKE_TOKEN = "like_tkn";				// いいねトークン
	public const string REQUEST_KEY_FOLLOW_TOKEN = "fol_tkn";				// フォロートークン

	// 商品一覧・詳細など
	public const string REQUEST_KEY_FAVORITE_KBN = "fkbn";			// お気に入り区分(1:追加 2:削除 OTHER:処理なし)
	public const string REQUEST_KEY_DATA_KBN = "dkbn";				// 商品特殊一覧データ区分
	public const string REQUEST_KEY_PRODUCTSALE_ID = "pslid";		// 商品セールID
	public const string REQUEST_KEY_PRODUCTSALE_KBN = "pslkbn";		// 商品セール区分
	public const string REQUEST_KEY_PRODUCT_SUBIMAGE_NO = "subimgno";	// 商品サブ画像番号

	// CMS
	public const string REQUEST_KEY_REAL_SHOP_ID = "rsid";						// リアル店舗ID
	public const string REQUEST_KEY_CONTENTS_TAG_ID = "ctid";					// コンテンツタグID

	// コーディネート系
	public const string REQUEST_KEY_COORDINATE_KEYWORD = "ckw";					// キーワード
	public const string REQUEST_KEY_COORDINATE_STAFF_ID = "csid";				// スタッフID
	public const string REQUEST_KEY_COORDINATE_UPPER_LIMIT = "cul";				// 身長上限
	public const string REQUEST_KEY_COORDINATE_LOWER_LIMIT = "cll";				// 身長下限

	// レコメンド
	public const string REQUEST_KEY_RECOMMEND_CODE = "rc";			// レコメンドコード
	public const string REQUEST_KEY_ITEM_CODE = "ic";				// アイテムコード
	public const string REQUEST_KEY_MAX_DISP_COUNT = "mdc";			// 商品最大表示数
	public const string REQUEST_KEY_IMAGE_SIZE = "is";				// 商品画像サイズ
	public const string REQUEST_KEY_DISP_KBN = "dk";				// 表示区分

	// 購入履歴一覧・詳細など
	public const string REQUEST_KEY_ORDER_ID = "odid";				// オーダーID
	// 定期購入
	public const string REQUEST_KEY_FIXED_PURCHASE_ID = "fxpchsid";	// 定期購入ID
	// 定期遷移画面区分
	public const string REQUEST_KEY_FIXED_PURCHASE_NEXT_PAGE_KBN = "fxpchnpk";	// 定期遷移画面区分
	// 入荷通知メール情報
	public const string REQUEST_KEY_USERPRODUCTARRIVALMAIL_ACTION_KBN = "act";		// 入荷通知メール処理区分(1:追加 2:削除 OTHER:処理なし)
	public const string REQUEST_KEY_USERPRODUCTARRIVALMAIL_MAIL_NO = "mno";				// 枝番
	public const string REQUEST_KEY_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN = "amkbn";	// 入荷通知メール区分
	public const string REQUEST_KEY_USERPRODUCTARRIVALMAIL_BEFORE_PRODUCT_URL = "bpurl";	// 登録元の商品ページURL

	// //頒布会系
	/// <summary>頒布会コースID</summary>
	public const string REQUEST_KEY_SUBSCRIPTION_BOX_COURSE_ID = "sbcid";
	/// <summary>コース一覧からの遷移か</summary>
	public const string REQUEST_KEY_SUBSCRIPTION_BOX_FOR_COURSE_LIST = "sbfcl";
	public const string REQUEST_KEY_SUBSCRIPTION_BOX_PRESERVE_SELECTION = "sbpsf";

	// 受信メール履歴
	public const string REQUEST_KEY_RECIEVEMAIL_NO = "rcvno";	// メール送信ログNo

	// メール自動返信ユーザ用認証キー
	public const string REQUEST_KEY_USER_AUTHKEY = "authkey";

	// フロントプレビュー用キー
	public const string REQUEST_KEY_PREVIEW_HASH = "prvw";

	// エラー画面
	public const string REQUEST_KEY_BACK_URL = "burl";				// 戻り用URL

	// W2MP:広告コード
	public const string REQUEST_KEY_ADVCODE = "advc";				// 広告コード

	/// <summary>楽天IDConnectによる自動ログイン</summary>
	public const string REQUEST_KEY_RAKUTEN_ID_CONNECT_LOGIN = "rlgin";
	/// <summary>楽天IDConnectによる紐づけ</summary>
	public const string REQUEST_KEY_RAKUTEN_ID_CONNECT_LINK = "rlink";
	/// <summary>楽天IDConnectで新規会員登録</summary>
	public const string REQUEST_KEY_RAKUTEN_REGIST = "rakutenRegist";

	/// <summary>特集ページタイプ</summary>
	public const string REQUEST_KEY_FEATURE_PAGE_TYPE = "fpagetype";

	/// <summary>CMSランディングページ:商品セット選択肢の枝番</summary>
	public const string REQUEST_KEY_LANDING_PAGE_BRANCH_NO= "bn";

	/// <summary>Landing page cart product id</summary>
	public const string REQUEST_KEY_LPCART_PRODUCT_ID = "productId";
	/// <summary>Landing page cart variation id</summary>
	public const string REQUEST_KEY_LPCART_VARIATION_ID = "variationId";
	/// <summary>Landing page cart add cart kbn</summary>
	public const string REQUEST_KEY_LPCART_ADD_CART_KBN = "addCartKbn";
	/// <summary>Landing page cart product count</summary>
	public const string REQUEST_KEY_LPCART_PRODUCT_COUNT = "productCount";

	/// <summary>戻る遷移などで、フォーカスを当てるコントロール名</summary>
	public const string REQUEST_KEY_FOCUS_ON = "focuson";

	/// <summary>ABテスト：ABテストID</summary>
	public const string REQUEST_KEY_AB_TEST_ID = "abUrl";

	/// <summary>Request referral code</summary>
	public const string REQUEST_KEY_REFERRAL_CODE = "ficd";

	/// <summary>リクエストキー定数: Amazon Pay自動連携解除アラート</summary>
	public const string REQUEST_KEY_AMAZON_PAY_AUTO_UNLINK_ALERT = "amazonpaymentautounlink";

	/// <summary>Request key front preview guid string</summary>
	public const string REQUEST_KEY_PREVIEW_GUID_STRING = "prvgs";

	//========================================================================
	// セッション変数キー
	//========================================================================
	// セキュア情報保護用認証キー
	public const string SESSION_KEY_AUTH_KEY_FOR_SECURE_SESSION = "w2cFront_AuthKeyForSecureSession";

	// ログイン系
	public const string SESSION_KEY_LOGIN_USER = "w2cFront_login_user";
	public const string SESSION_KEY_LOGIN_USER_ID = "w2cFront_login_user_id";
	public const string SESSION_KEY_LOGIN_USER_NAME = "w2cFront_login_user_name";
	public const string SESSION_KEY_LOGIN_USER_NICK_NAME = "w2cFront_login_user_nick_name";
	public const string SESSION_KEY_LOGIN_USER_MAIL = "w2cFront_login_user_mail";
	public const string SESSION_KEY_LOGIN_USER_MAIL2 = "w2cFront_login_user_mail2";
	public const string SESSION_KEY_LOGIN_USER_BIRTH = "w2cFront_login_user_birth";
	public const string SESSION_KEY_LOGIN_USER_POINT = "w2cFront_login_user_point";
	public const string SESSION_KEY_LOGIN_USER_MEMBER_RANK_ID = "w2cFront_login_member_rank_id";
	public const string SESSION_KEY_LOGIN_USER_RAKUTEN_OPEN_ID = "w2cFront_login_rakuten_open_id";
	public const string SESSION_KEY_LOGIN_USER_RAKUTEN_ID_CONNECT_REGISTER_USER = "w2cFront_login_rakuten_id_connect_register_user";
	public const string SESSION_KEY_LOGIN_USER_RAKUTEN_ID_CONNECT_LOGGEDIN = "w2cFront_login_rakuten_id_connect_loggedin";
	public const string SESSION_KEY_LOGIN_USER_LAST_LOGGEDIN_DATE = "w2cFront_login_last_loggedin_date";
	public const string SESSION_KEY_LOGIN_USER_CPM_CLUSTER_NAME = "w2cFront_login_user_cpm_cluster_name";
	public const string SESSION_KEY_LOGIN_USER_FIXED_PURCHASE_MEMBER_FLG = "w2cFront_login_user_fixed_purchase_member_flg";
	public const string SESSION_KEY_LOGIN_USER_EASY_REGISTER_FLG = "w2cFront_login_user_easy_register_flg";
	public const string SESSION_KEY_LOGIN_USER_MEMBER_RANK = "w2cFront_login_member_rank";
	public const string SESSION_KEY_LOGIN_USER_MANAGEMENT_LEVEL_ID = "w2cFront_login_user_management_level_id";
	public const string SESSION_KEY_LOGIN_USER_HIT_TARGET_LIST_IDS = "w2cFront_login_user_hit_target_list_ids";
	public const string SESSION_KEY_LOGIN_USER_LINE_ID = "w2cFront_login_line_user_id";

	/// <summary>注文完了時会員登録入力情報</summary>
	public const string SESSION_KEY_REGISTER_USER_INPUT = "w2cFront_register_user_input";
	/// <summary>注文完了時会員登録用メール情報</summary>
	public const string SESSION_KEY_MAIL_FOR_USER_REGISTER_WHEN_ORDER_COMPLETE = "w2Front_mail_for_user_register_when_order_complete";
	/// <summary>LPユーザー拡張項目入力情報</summary>
	public const string SESSION_KEY_USER_EXTEND_INPUT = "w2cFront_landing_user_extend_input";

	// 画面遷移正当性チェック用
	public const string SESSION_KEY_NEXT_PAGE_FOR_CHECK = "next_page_for_check";
	public const string SESSION_KEY_FROM_NEXT_BUTTON = "from_next_button";
	// ルートカテゴリ保持用
	public const string SESSION_KEY_ROOT_CATEGORIES = "w2cFront_root_categories";
	// カート保持用
	public const string SESSION_KEY_CART_LIST = "w2cFront_cart_list";
	public const string SESSION_KEY_CART_LIST_LANDING = "w2cFront_cart_list_landing";
	public const string SESSION_KEY_FIXED_PURCHASE_CART_LIST_LANDING = "w2cFront_fixed_purchase_cart_list_landing";
	public const string SESSION_KEY_LANDING_CART_SESSION_KEY = "w2cFront_landing_cart_session_key";
	public const string SESSION_KEY_CURRENT_CART_INDEX = "w2cFront_current_cart_index";
	public const string SESSION_KEY_PRODUCT_LIST_LANDING = "w2cFront_product_list_landing";
	public const string SESSION_KEY_LANDING_CART_INPUT_LAST_WRITE_TIME = "landing_cart_input_last_write_time";
	public const string SESSION_KEY_LANDING_CART_INPUT_ABSOLUTEPATH = "landing_cart_input_absolutePath";
	/// <summary>通常配送先設定のチェック状況保持用</summary>
	public const string SESSION_KEY_DEFAULT_SHIPPING_CHECK = "w2cFront_default_shipping_check_status";
	/// <summary>通常支払方法設定のチェック状況保持用</summary>
	public const string SESSION_KEY_DEFAULT_PAYMENT_CHECK = "w2cFront_default_payment_check_status";
	/// <summary>Session Key Default Invoice Check</summary>
	public const string SESSION_KEY_DEFAULT_INVOICE_CHECK = "w2cFront_default_invoice_check_status";
	// 処理後遷移URL
	public const string SESSION_KEY_NEXT_URL = "w2cFront_next_url";
	// 検索ワード取得用
	public const string SESSION_KEY_PRODUCT_SEARCH_FLG = "w2cFront_product_search_flg";
	// 商品表示履歴用
	public const string SESSION_KEY_PRODUCTHISTORY_LIST = "w2cFront_producthistory_list";
	// メール自動返信ユーザメールアドレス保持用
	public const string SESSION_KEY_AUTH_MAILADDR = "w2cFront_authmailaddr";
	// 商品セールID（闇市用）
	public const string SESSION_KEY_CLOSEDMARKET_ID = "w2cFront_closedmarket_";
	// ターゲットページ識別用
	public const string SESSION_KEY_TARGET_PAGE = "w2cFront_target_page";
	// リンク式決済向け注文IDチェック用（SBPSで利用）
	public const string SESSION_KEY_ORDER_ID_CHECK_FOR_LINK_TYPE_PAYMENT = "w2cFront_order_id_check_for_link_type_payment";
	// 注文同梱処理用
	public const string SESSION_KEY_ORDERCOMBINE_CART_LIST = "w2cFront_ordercombine_cart_list"; // 注文同梱後のカート情報
	public const string SESSION_KEY_ORDERCOMBINE_BEFORE_CART_LIST = "w2cFront_ordercombine_cart_list_before"; // 注文同梱前のカート情報
	public const string SESSION_KEY_ORDERCOMBINE_PAYMENT_RESELECT = "w2Front_ordercombine_payment_reselect"; // 注文同梱後に決済情報の変更があったか
	/// <summary>「Amazonアカウントでお支払」ボタンを押して注文同梱選択画面への選択か</summary>
	public const string SESSION_KEY_ORDERCOMBINE_FROM_AMAZON_PAY_BUTTON = "w2Front_ordercombine_from_amazon_pay_button";
	// デフォルト注文方法系メッセージ格納
	public const string SESSION_KEY_ORDER_ERROR_MESSAGE = "w2cFront_order_error_message";
	public const string SESSION_KEY_IS_DISP_USER_DEFAULT_ORDER_SETTING_COMPLETE_MESSAGE = "w2cFront_is_disp_user_default_order_setting_complete_message";
	// ワンタイムオファー用
	public const string SESSION_KEY_ORDER_ORDERED_CART_LIST = "w2cFront_order_ordered_cart_list"; // 注文済カート情報
	/// <summary>ペイパルログイン結果(PayPal認証すると格納される)</summary>
	public const string SESSION_KEY_PAYPAL_LOGIN_RESULT = "w2Front_paypal_login_result";
	/// <summary>ペイパル連携情報(ユーザーがPayPalに紐づいている場合格納される)</summary>
	public const string SESSION_KEY_PAYPAL_COOPERATION_INFO = "w2Front_paypal_cooperation_info";
	/// <summary>ペイパル注文失敗したか</summary>
	public const string SESSION_KEY_IS_PAYPAL_ORDER_FAILED = "w2Front_is_paypal_order_failed";
	/// <summary>お気に入りトークン</summary>
	public const string SESSION_KEY_FAV_TOKEN = "w2Front_fav_token";
	/// <summary>いいねトークン</summary>
	public const string SESSION_KEY_LIKE_TOKEN = "w2Front_like_token";
	/// <summary>フォロートークン</summary>
	public const string SESSION_KEY_FOLLOW_TOKEN = "w2Front_fol_token";
	/// <summary>配送方法調整後の遷移先</summary>
	public const string SESSION_KEY_NEXT_PAGE_AFTER_ADJUST_SHIPPING_METHOD = "w2Front_next_page_after_adjust_shipping_method";
	/// <summary>特集カテゴリID</summary>
	public const string SESSION_KEY_FEATURE_PARENT_CATEGORY_ID = "feature_parent_category_id";
	/// <summary>配送不可エリアエラー</summary>
	public const string SESSION_KEY_UNAVAILABLE_SHIPPING_AREA_ERROR = "w2Front_unavailable_shipping_area_error";
	/// <summary>仮ユーザーID</summary>
	public const string SESSION_KEY_TEMPORARY_USER_ID = "w2Front_temporary_user_id";

	/// <summary>頒布会用</summary>
	/// <summary>商品情報</summary>
	public const string SESSION_KEY_PRODUCT_OPTION_SETTING_LIST = "w2cFront_product_option_setting_list";
	/// <summary>頒布会コースID</summary>
	public const string SESSION_KEY_SUBSCRIPTION_BOX_COURSE_ID = "w2cFront_subscription_box_course_id";
	/// <summary>頒布会商品バリエーションID</summary>
	public const string SESSION_KEY_SUBSCRIPTION_BOX_VARIATION_ID = "w2cFront_subscription_box_variation_id";

	// W2MP：アクセスログ用セッション変数キー
	public const string SESSION_KEY_W2MP_ACCESSLOG_STATUS = "w2cFront_accesslog_status";				// ログイン/退会用ステータス
	public const string SESSION_KEY_W2MP_ACCESSLOG_LOGIN_USER_ID = "w2cFront_accesslog_login_user_id";	// ログインユーザID

	public const string SESSION_KEY_DISPPLAY_POPUP_ADD_FAVORITE = "w2cFront_display_popup_add_favorite"; // お気に入り遷移フラグ

	/// <summary> W2MP：商品検索ワード分析</summary>
	public const string SESSION_KEY_DO_REGISTER_PRODUCT_SEARCHWORD = "pswflg";

	/// <summary> 楽天IDConnectアクション情報 </summary>
	public const string SESSION_KEY_RAKUTEN_ID_CONNECT_ACTION_INFO = "w2cFront_rakuten_id_connect_action_info";

	/// <summary>ブランドID</summary>
	public const string SESSION_KEY_BRAND_ID = "w2cFront_brand_id";
	/// <summary>最後に表示していたブランドID</summary>
	public const string SESSION_KEY_LAST_DISPLAYED_BRAND_ID = "w2cFront_last_displayed_brand_id";

	/// <summary>参照日</summary>
	public const string SESSION_KEY_REFERENCE_DATETIME = "w2cFront_ReferenceDateTime";
	/// <summary>参照会員ランク</summary>
	public const string SESSION_KEY_REFERENCE_MEMBER_RANK = "w2cFront_ReferenceMemberRank";
	/// <summary>参照会員ターゲットリストsummary>
	public const string SESSION_KEY_REFERENCE_TARGET_LIST = "w2cFront_ReferenceTargetList";

	/// <summary>CMSランディングページ用 確認画面スキップフラグ</summary>
	public const string SESSION_KEY_CMS_LANDING_CART_CONFIRM_SKIP_FLG = "w2Front_cms_landing_cart_confirm_skip_flg";
	/// <summary>CMSランディングページ用 商品セット選択</summary>
	public const string SESSION_KEY_CMS_LANDING_PRODUCT_SET_SELECT = "w2Front_cms_landing_cart_product_set_select";
	/// <summary>CMSランディングページ用 定期購入判定用</summary>
	public const string SESSION_KEY_CMS_LANDING_IS_FIXEDPURCHASE = "w2Front_cms_landing_is_fixedpurchase_buytype";

	/// <summary>Is only add cart first time</summary>
	public const string SESSION_KEY_IS_ONLY_ADD_CART_FIRST_TIME = "w2Front_is_only_add_cart_first_time";
	/// <summary>Is only add product set cart landing page first time</summary>
	public const string SESSION_KEY_IS_ONLY_ADD_PRODUCT_SET_CARTLP_FIRST_TIME = "w2Front_is_only_add_product_set_cartlp_first_time";
	/// <summary>Is cart list landing page</summary>
	public const string SESSION_KEY_IS_CARTLIST_LP = "w2Front_is_cartlist_lp";
	/// <summary>新LPカート確認画面からの遷移か</summary>
	public const string SESSION_KEY_IS_REDIRECT_FROM_LANDINGCART_CONFIRM = "w2Front_is_redirect_from_landing_confirm";
	/// <summary>LINE PAYの画面からの遷移か</summary>
	public const string SESSION_KEY_IS_REDIRECT_FROM_LINEPAY = "w2Front_is_redirect_from_linepay";
	/// <summary>エラー時の遷移先要素のクライアントID</summary>
	public const string SESSION_KEY_LP_VAILEDATE_ERROR_ELEMENT_CLIENT_ID = "session_key_lp_validate_error_element_client_id";
	/// <summary>LPページでのパスワード</summary>
	public const string SESSION_KEY_LP_PASSWORD = "session_key_lp_password";
	public const string SESSION_KEY_LP_PASSWORDCONF = "session_key_lp_passwordconf";
	/// <summary>AmazonPayアカウントで新規登録するか（通常カート）</summary>
	public const string SESSION_KEY_IS_AMAZON_PAY_REGISTER_FOR_ORDER = "w2Front_is_amazon_pay_register_for_order";
	/// <summary>カート種別が変わったか（レコメンドで通常→定期 or 定期→通常）</summary>
	public const string SESSION_KEY_IS_CHANGED_AMAZON_PAY_FOR_FIXED_OR_NORMAL = "w2Front_is_changed_amazon_pay_for_fixed_or_normal";
	/// <summary>AmazonPayで定期の同意をとっているか</summary>
	public const string SESSION_KEY_IS_AMAZON_PAY_GOT_RECURRING_CONSENT = "w2Front_is_amazon_pay_got_recurring_consent";
	/// <summary>ABテスト振り分けからの遷移か</summary>
	public const string SESSION_KEY_IS_REDIRECT_FROM_ABTEST_ALLOTOMENT = "w2Front_is_redirect_from_AbTest_Alloutoment";

	/// <summary>Session key next page for Paypay SBPS</summary>
	public const string SESSION_KEY_NEXT_PAGE_FOR_PAYPAY_SBPS = "next_page_for_paypay_sbps";
	/// <summary>Session key cart list for Paypay</summary>
	public const string SESSION_KEY_CART_LIST_FOR_PAYPAY = "w2cFront_cart_list_for_paypay";

	/// <summary>Session key referral code</summary>
	public const string SESSION_KEY_REFERRAL_CODE = "w2Front_key_referral_code";

	/// <summary>Session key has authentication code</summary>
	public const string SESSION_KEY_HAS_AUTHENTICATION_CODE = "has_authentication_code";
	/// <summary>Session key authentication code</summary>
	public const string SESSION_KEY_AUTHENTICATION_CODE = "authentication_code";

	/// <summary>会員番号（クロスポイント用）</summary>
	public const string SESSION_KEY_CROSSPOINT_APP_KEY = "appKeyForCrossPoint";
	/// <summary>会員番号（クロスポイント用）</summary>
	public const string SESSION_KEY_CROSSPOINT_MEMBER_ID = "memberIdForCrossPoint";
	/// <summary>PINコード（クロスポイント用）</summary>
	public const string SESSION_KEY_CROSSPOINT_PIN_CODE = "pinCodeForCrossPoint";
	/// <summary>店舗カードNO/PIN更新フラグ</summary>
	public const string SESSION_KEY_CROSS_POINT_UPDATED_SHOP_CARD_NO_AND_PIN_FLG = "updated_shop_card_and_no_pin_flg";
	/// <summary>戻るボタン判定（クロスポイント用）</summary>
	public const string SESSION_KEY_IS_BACK_CROSS_POINT = "is_back_for_cross_point";

	/// <summary>Open index for order shipping select page</summary>
	public const string SESSION_KEY_ORDER_SHIPPING_SELECT_OPEN_INDEX = "w2Front_order_shipping_select_open_index";

	/// <summary>Session key real shop selection info</summary>
	public const string SESSION_KEY_REALSHOP_SELECTION_INFO = "w2Front_realshop_selection_storage";
	/// <summary>Session key store pickup order combine</summary>
	public const string SESSION_KEY_STORE_PICKUP_ORDER_COMBINE = "w2Front_store_pickup_order_combine";

	/// <summary>２クリックボタンが押下されたかどうか</summary>
	public const string SESSION_KEY_IS_TWO_CLICK_BUTTON = "isTwoClickButton";

	/// <summary>定期情報詳細画面からの遷移か</summary>
	public const string SESSION_KEY_IS_REDIRECT_FROM_FIXED_PURCHASE_DETAIL = "w2Front_is_redirect_from_fixed_purchase_detail";
	/// <summary>定期購入ID保持（Front定期商品追加用）</summary>
	public const string SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_FIXED_PURCHASE_ID = "fixed_purchase_product_add_fixed_purchase_id";
	/// <summary>配送種別保持（Front定期商品追加用）</summary>
	public const string SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_SHIPPING_ID = "fixed_purchase_product_add_shipping_id";
	/// <summary>決済種別保持（Front定期商品追加用）</summary>
	public const string SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_PAYMENT_ID = "fixed_purchase_product_add_payment_id";
	/// <summary>商品ID一覧保持（Front定期商品追加用）</summary>
	public const string SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_PRODUCT_IDS = "fixed_purchase_product_add_product_ids";
	/// <summary>商品バリエーションID一覧保持（Front定期商品追加用）</summary>
	public const string SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_VARIATION_IDS = "fixed_purchase_product_add_variation_ids";
	/// <summary>商品バリエーションID一覧保持（Front定期商品追加用）</summary>
	public const string SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_PRODUCT_AND_VARIATION_IDS = "fixed_purchase_product_addproduct_and__variation_ids";
	/// <summary>商品ID保持（Front定期商品追加用）</summary>
	public const string SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_PRODUCT_ID = "fixed_purchase_product_add_product_id";
	/// <summary>商品バリエーションID保持（Front定期商品追加用）</summary>
	public const string SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_VARIATION_ID = "fixed_purchase_product_add_variation_id";
	/// <summary>商品付帯情報保持（Front定期商品追加用）</summary>
	public const string SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_PRODUCT_PRODUCT_OPTION = "fixed_purchase_product_add_product_option";
	/// <summary>定期入力情報を復元するか（Front定期商品追加用）</summary>
	public const string SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_BACK = "fixed_purchase_product_add_back";
	/// <summary>不正なページ読み込みではないか</summary>
	public const string SESSION_KEY_PRODUCT_LIST_IS_VALID_PAGE_REQUEST = "product_list_is_valid_page_request";
	/// <summary>モーダル表示用商品ID</summary>
	public const string SESSION_KEY_PRODUCT_LIST_PRODUCT_ID_FOR_MODAL = "SESSION_KEY_PRODUCT_LIST_PRODUCT_ID_FOR_MODAL";
	/// <summary>定期商品変更設定ID保持（Front定期商品変更用）</summary>
	public const string SESSION_KEY_FIXED_PURCHASE_PRODUCT_CHANGE_SETTING_ID = "fixed_purchase_product_change_setting_id";
	/// <summary>変更前商品ID保持（Front定期商品変更用）</summary>
	public const string SESSION_KEY_FIXED_PURCHASE_PRODUCT_CHANGE_BEFORE_PRODUCT_ID = "fixed_purchase_product_change_before_product_id";
	/// <summary>変更後商品ID保持（Front定期商品変更用）</summary>
	public const string SESSION_KEY_FIXED_PURCHASE_PRODUCT_CHANGE_AFTER_PRODUCT_ID = "fixed_purchase_product_change_after_product_id";
	/// <summary>商品バリエーションID保持（Front定期商品変更用）</summary>
	public const string SESSION_KEY_FIXED_PURCHASE_PRODUCT_CHANGE_VARIATION_ID = "fixed_purchase_product_change_variation_id";
	/// <summary>定期入力情報を復元するか（Front定期商品変更用）</summary>
	public const string SESSION_KEY_FIXED_PURCHASE_PRODUCT_CHANGE_BACK = "fixed_purchase_product_change_back";
	/// <summary>注文確認画面へ遷移済みか</summary>
	public const string SESSION_KEY_IS_MOVED_ON_ORDER_CONFIRM = "w2Front_is_moved_on_order_confirm";

	//========================================================================
	// クッキー変数キー
	//========================================================================
	// セキュア情報保護用認証キー
	public const string COOKIE_KEY_AUTH_KEY = "authkey";
	// セッションデータ格納キー
	public const string COOKIE_KEY_SESSION_DATA_KEY = "Session_Data_Key";
	// スマートフォンＰＣサイト利用設定（pcならPCサイト利用）
	public const string COOKIE_KEY_SMARTPHONE_SITE = "SmartPhoneUsePcSite";
	// リンクシェアアフィリエイト
	public const string COOKIE_KEY_AFFILIATE_LINKSHARE = "Affiliate_LinkShare";
	/// <summary>GoogleAnalyticsタグ制御用注文ID</summary>
	public const string COOKIE_KEY_GOOGLEANALYTICS_ORDER_ID = "GoogleAnalytics_OrderId_";

	//========================================================================
	// 注文処理用変数キー
	//========================================================================
	public const string ORDER_KEY_ORDER_OWNER = "order_owner";
	public const string ORDER_KEY_ORDER_SHIPPING = "order_shipping";
	public const string ORDER_KEY_ORDER_PAYMENT = "order_payment";
	/// <summary>注文完了時会員登録用メール情報</summary>
	public const string ORDER_KEY_MAIL_FOR_USER_REGISTER_WHEN_ORDER_COMPLETE = "order_key_mail_for_user_register_when_order_complete";

	//========================================================================
	// 問い合わせ処理用変数キー
	//========================================================================
	public const string INQUIRY_KEY_INQUIRY_TITLE = "inquiry_title";
	public const string INQUIRY_KEY_INQUIRY_TEXT = "inquiry_text";
	public const string INQUIRY_KEY_PRODUCT_URL = "product_url";

	//========================================================================
	// クライアント検証用キー
	//========================================================================
	/// <summary>クライアント検証用キー：言語ロケールID</summary>
	public const string CLIENTVALIDATE_CSS_HEAD_LANGUAGE_LOCALE_ID = @"languageLocaleId";
	/// <summary>クライアント検証用キー：国ISOコード</summary>
	public const string CLIENTVALIDATE_CSS_HEAD_COUNTRY_ISO_CODE = @"countyIsoCode";

	//========================================================================
	// Async postback
	//========================================================================
	public const string ASYNC_POSTBACK_ERROR = "async_postback_error";

	// Webキャプチャー時の許可IPアドレス
	public static string ALLOWED_IP_ADDRESS_FOR_WEBCAPTURE = "";

	// SeoUtility用 クラス名
	public const string CLASS_NAME_PRODUCT_DETAIL = "Form_Product_ProductDetail";
	public const string CLASS_NAME_PRODUCT_LIST = "Form_Product_ProductList";
	public const string CLASS_NAME_COORDINATE_TOP = "Form_Coordinate_CoordinateTop";
	public const string CLASS_NAME_COORDINATE_DETAIL = "Form_Coordinate_CoordinateDetail";
	public const string CLASS_NAME_COORDINATE_LIST = "Form_Coordinate_CoordinateList";
	public const string CLASS_NAME_CONTENTS_PAGE = "ContentsPage";
	/// <summary>Class name shop list</summary>
	public const string CLASS_NAME_SHOPLIST = "Form_RealShop_ShopList";
	/// <summary>Class name shop detail</summary>
	public const string CLASS_NAME_SHOPDETAIL = "Form_RealShop_ShopDetail";
	/// <summary>Class name feature template</summary>
	public const string CLASS_NAME_FEATURE_TEMPLATE = "Form_PageTemplates_FeaturePageTemplate";

	/// <summary>Reqest key disptitile type</summary>
	public const string REQUEST_KEY_DISPTITILE_KBN = "disptitile";
	/// <summary>Disptitile type: none</summary>
	public const string KBN_DISPTITILE_NONE = "none";

	/// <summary>確認画面で決済エリアにフォーカスするためのコントロール名</summary>
	public const string CONST_CONTROLL_NAME_FOR_PAYMENT_AREA = "rbgPayment";
	/// <summary>確認画面で決済エリアにフォーカスするためのコントロール名</summary>
	public const string CONST_CONTROLL_NAME_FOR_PAYMENT_AREA_AMAZON_PAY = "#walletWidgetDiv";
	/// <summary>外部連携メモが書かれているか判断用</summary>
	public const string CONST_RELATIONMEMO_AMAZON_PAY = "【Amazon注文者情報】";

	/// <summary>Value text param fixed purchase detail</summary>
	public const string VALUETEXT_PARAM_FIXED_PURCHASE_DETAIL = "fixed_purchase_detail";
	/// <summary>Value text param month day interval invalid</summary>
	public const string VALUETEXT_PARAM_MONTH_DAY_INTERVAL_INVALID = "month_day_interval_invalid";
	/// <summary>Value text param day</summary>
	public const string VALUETEXT_PARAM_DAY = "day";
	/// <summary>Value text param week</summary>
	public const string VALUETEXT_PARAM_WEEK = "week";
	/// <summary>Value text param month</summary>
	public const string VALUETEXT_PARAM_MONTH = "month";
	/// <summary>Value text param product detail</summary>
	public const string VALUETEXT_PARAM_PRODUCT_DETAIL = "product_detail";
	/// <summary>Value text param product data setting message</summary>
	public const string VALUETEXT_PARAM_PRODUCT_DATA_SETTING_MESSAGE = "product_data_setting_message";
	/// <summary>Value text param product data main image</summary>
	public const string VALUETEXT_PARAM_PRODUCT_DATA_MAIN_IMAGE = "product_data_main_image";
	/// <summary>おすすめタグ・商品一覧画面</summary>
	public const string VALUETEXT_PARAM_PRODUCT_RECOMMEND_PRODUCT_LIST = "recommend_product_list";
	/// <summary>おすすめタグ・商品一覧画面 並び替え区分リスト</summary>
	public const string VALUETEXT_PARAM_PRODUCT_RECOMMEND_PRODUCT_LIST_SORT_KBN = "sort_kbn";

	/// <summary>セールのみ検索に利用するSQLのパラメータ</summary>
	public const string SQL_PARAMETR_PRODUCT_SALE_ONLY = "sale_only";
	/// <summary>現在時刻をSQLに渡すときに使用するSQLのパラメータ</summary>
	public const string SQL_PARAMETR_CURRENT_TIME = "current_time";
	
	/// <summary> Datetime format yyyy-MM-dd</summary>
	public const string DATETIME_YYYYMMDD = "yyyy-MM-dd";

	/// <summary>Value text 頒布会</summary>
	public const string VALUETEXT_PARAM_SUBSCRIPTIONBOX = "subscription_box";
	/// <summary>Value text 頒布会引継ぎ商品</summary>
	public const string VALUETEXT_PARAM_SUBSCRIPTION_BOX_TAKE_OVER = "subscription_box_take_over";
	/// <summary>頒布会更新商品リスト</summary>
	public const string SESSION_KEY_SUBSCRIPTION_BOX_OPTIONAL_LIST = "SubscriputionBoxProductListModify";
	/// <summary>頒布会任意商品リスト</summary>
	public const string SESSION_KEY_SUBSCRIPTION_BOX_LIST = "SubscriputionBoxProductList";
	/// <summary>頒布会更新商品リスト保持用</summary>
	public const string SESSION_KEY_SUBSCRIPTION_BOX_OPTIONAL_LIST_FOR_TEMP = "SubscriputionBoxProductListModifyForTemp";
	/// <summary>頒布会任意商品リスト保持用</summary>
	public const string SESSION_KEY_SUBSCRIPTION_BOX_LIST_FOR_TEMP = "SubscriputionBoxProductListForTemp";
	/// <summary>カート投入ボタン表示情報 頒布会</summary>
	public const string ADD_CART_INFO_CAN_SUBSCRIPTION_BOX = "CanSubscriptionBox";

	/// <summary>リピータコマンド:通知メール申し込みボタン表示（バリエーション毎のカート)</summary>
	public const string COMMAND_NAME_SMART_ARRIVALMAIL = "SmartArrivalMail";
}
