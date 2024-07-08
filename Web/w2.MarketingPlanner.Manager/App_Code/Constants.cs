/*
=========================================================================================================
  Module      : マネージャ定数定義(Constants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/

using w2.Domain.MenuAuthority.Helper;

public partial class Constants : w2.App.Common.Constants
{
	//*****************************************************************************************
	// パス・設定ファイル系
	//*****************************************************************************************
	//=========================================================================================
	// パス（global.asaxで設定）
	//=========================================================================================
	// メニューXML物理パス
	public static string PHYSICALDIRPATH_MENU_XML = "";
	// 機能一覧XML物理パス
	public static string PHYSICALDIRPATH_PAGE_INDEX_LIST_XML = "";
	// 区分分析XML物理パス
	public static string PHYSICALDIRPATH_KBN_ANALYSIS_XML = "";
	/// マスタファイル取込実行ＥＸＥ
	public static string PHYSICALDIRPATH_MASTERUPLOAD_EXE = "";

	// モバイルサイトのみ設定
	public static bool CONST_IS_MOBILE_SITE_ONLY = false;

	// ポイントオプション：ポイントオプション利用有無
	public static bool MARKETINGPLANNER_POINT_OPTION_ENABLE = false;

	// メール配信オプション：メール配信オプション利用有無
	public static bool MARKETINGPLANNER_MAIL_OPTION_ENABLE = false;

	// メール配信オプション：絵文字画像ファイルパス
	public static string MARKETINGPLANNER_EMOJI_IMAGE_DIRPATH = "";

	// メール配信オプション：絵文字画像ファイルURL
	public static string MARKETINGPLANNER_EMOJI_IMAGE_URL = "";

	// デコメ配信オプション：デコメ配信オプション利用有無
	public static bool MARKETINGPLANNER_DECOME_OPTION_ENABLE = false;

	// ターゲットリストSQL抽出利用有無
	public static bool MARKETINGPLANNER_TARGETLIST_SQL_CONDITION_ENABLE = false;

	// クーポンオプション：クーポンオプション利用有無
	public static bool MARKETINGPLANNER_COUPON_OPTION_ENABLE = false;

	// アフィリエイトオプション：アフィリエイトオプション利用有無
	public static bool MARKETINGPLANNER_AFFILIATE_OPTION_ENABLE = false;

	// 汎用アフィリエイトオプション：汎用アフィリエイト利用有無
	public static bool MARKETINGPLANNER_MULTIPURPOSE_AFFILIATE_OPTION_ENABLE = false;

	// モバイルマイメニュー利用有無
	public static bool USE_MOBILE_MYMENU = false;

	//*****************************************************************************************
	// パッケージ固有設定
	//*****************************************************************************************
	public const string APPLICATION_NAME_DISP = "w2MarketingPlanner";

	/// <summary>マネージャサイトタイプ</summary>
	public static readonly MenuAuthorityHelper.ManagerSiteType ManagerSiteType = MenuAuthorityHelper.ManagerSiteType.Mp;
	/// <summary>メニュー権限フィールド名（店舗オペレータマスタの利用フィールド名）</summary>
	public static string FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL_PKG = MenuAuthorityHelper.GetOperatorMenuAccessLevelFieldName(ManagerSiteType);

	//*****************************************************************************************
	// 定数値・マネージャサイド
	//*****************************************************************************************
	// パッケージ固有
	public static int CONST_DISP_CONTENTS_ACSPAGERANKING_LIST = 40;     // アクセスページランキングに表示する件数
	/// <summary>売上予測レポート算出区分のデフォルト値</summary>
	public static string FIXEDPURCHASEFORECASTREPORT_CALCULATION_DEFAULT_VALUE = "SHIPPED_DATE";

	//*****************************************************************************************
	// メニューパス設定
	//*****************************************************************************************
	public static string MENU_PATH_LARGE_COUPON = "Form/Coupon/"; // 受注情報メニューパス

	//*****************************************************************************************
	// 区分・マネージャサイド
	//*****************************************************************************************
	// メニュー権限機能レベル：会員情報
	public const int KBN_MENU_FUNCTION_USER_DL = 1;		// ユーザマスタダウンロード
	public const int KBN_MENU_FUNCTION_USER_UPDATE = 2;			// ユーザ情報更新

	// メニュー権限機能レベル：ターゲットリスト
	public const int KBN_MENU_FUNCTION_TARGETLIST_DL = 1;	// ターゲットリストダウンロード

	// メニュー権限機能レベル：広告コード
	public const int KBN_MENU_FUNCTION_ADVCODE_DL = 1;				// 広告コードダウンロード
	public const int KBN_MENU_FUNCTION_ADVCODE_MEDIA_TYPE_DL = 2;	// AdvCode Media Type download

	// メニュー権限機能レベル：ユーザーポイント
	public const int KBN_MENU_FUNCTION_USERPOINT_DL = 1;	// ユーザーポイントダウンロード
	public const int KBN_MENU_FUNCTION_USERPOINT_DETAIL_DL = 2;	// ユーザーポイント(詳細)ダウンロード

	// メニュー権限機能レベル：クーポン
	public const int KBN_MENU_FUNCTION_COUPON_DL = 1;	// クーポンダウンロード
	public const int KBN_MENU_FUNCTION_USERCOUPON_DL = 1;	// ユーザクーポンダウンロード

	// ユーザポイント一覧検索キー区分
	public const string KBN_SEARCHKEY_USERPOINT_LIST_USER_ID = "0";				// ユーザーID
	public const string KBN_SEARCHKEY_USERPOINT_LIST_NAME = "1";				// 氏名
	public const string KBN_SEARCHKEY_USERPOINT_LIST_NAME_KANA = "2";			// フリガナ
	public const string KBN_SEARCHKEY_USERPOINT_LIST_TEL1 = "3";				// 電話番号
	public const string KBN_SEARCHKEY_USERPOINT_LIST_MAIL_ADDR = "4";			// メールアドレス
	public const string KBN_SEARCHKEY_USERPOINT_LIST_ZIP1 = "5";				// 郵便番号
	public const string KBN_SEARCHKEY_USERPOINT_LIST_ADDR = "6";				// 住所
	public const string KBN_SEARCHKEY_USERPOINT_LIST_COMPANY_NAME = "7";		// 企業名
	public const string KBN_SEARCHKEY_USERPOINT_LIST_COMPANY_POST_NAME = "8";	// 部署名
	public const string KBN_SEARCHKEY_USERPOINT_LIST_DEFAULT = KBN_SEARCHKEY_USERPOINT_LIST_USER_ID;// ユーザーIDがデフォルト

	// ユーザーポイント一覧ソート区分
	public const string KBN_SORT_USERPOINT_LIST_POINT_ASC = "0";		// ポイント/昇順
	public const string KBN_SORT_USERPOINT_LIST_POINT_DESC = "1";		// ポイント/降順
	public const string KBN_SORT_USERPOINT_LIST_EXP_DATETIME_ASC = "2";		// 有効期限/昇順
	public const string KBN_SORT_USERPOINT_LIST_EXP_DATETIME_DESC = "3";		// 有効期限/降順
	public const string KBN_SORT_USERPOINT_LIST_USER_ID_ASC = "8";				// ユーザID/昇順
	public const string KBN_SORT_USERPOINT_LIST_USER_ID_DESC = "9";				// ユーザID/降順
	public static string KBN_SORT_USERPOINT_LIST_DEFAULT = KBN_SORT_USERPOINT_LIST_USER_ID_DESC;	// ユーザID/降順がデフォルト

	// ユーザ一覧ソート区分
	public const string KBN_SORT_USER_LIST_NAME_ASC = "0";		// 氏名/昇順
	public const string KBN_SORT_USER_LIST_NAME_DESC = "1";		// 氏名/降順
	public const string KBN_SORT_USER_LIST_NAME_KANA_ASC = "2";		// 氏名(かな)/昇順
	public const string KBN_SORT_USER_LIST_NAME_KANA_DESC = "3";		// 氏名(かな)/降順
	public const string KBN_SORT_USER_LIST_DATE_CREATED_ASC = "4";		// 作成日/昇順
	public const string KBN_SORT_USER_LIST_DATE_CREATED_DESC = "5";		// 作成日/降順
	public const string KBN_SORT_USER_LIST_DATE_CHANGED_ASC = "6";		// 更新日/昇順
	public const string KBN_SORT_USER_LIST_DATE_CHANGED_DESC = "7";		// 更新日/降順
	public const string KBN_SORT_USER_LIST_USER_ID_ASC = "8";			// ユーザID/昇順
	public const string KBN_SORT_USER_LIST_USER_ID_DESC = "9";			// ユーザID/降順
	public const string KBN_SORT_USER_LIST_DEFAULT = KBN_SORT_USER_LIST_USER_ID_DESC;	// ユーザID/降順がデフォルト

	// クーポン一覧ソート区分
	public const string KBN_SORT_COUPON_LIST_DATE_CREATED_ASC = "0";		// 作成日/昇順
	public const string KBN_SORT_COUPON_LIST_DATE_CREATED_DESC = "1";		// 作成日/降順
	public const string KBN_SORT_COUPON_LIST_DATE_CHANGED_ASC = "2";		// 更新日/昇順
	public const string KBN_SORT_COUPON_LIST_DATE_CHANGED_DESC = "3";		// 更新日/降順
	public static string KBN_SORT_COUPON_LIST_DEFAULT = KBN_SORT_COUPON_LIST_DATE_CHANGED_DESC;	// 更新日/昇順がデフォルト

	// ユーザクーポン一覧検索キー区分
	public const string KBN_SEARCHKEY_USERCOUPON_LIST_USER_ID = "0";			// ユーザーID
	public const string KBN_SEARCHKEY_USERCOUPON_LIST_NAME = "1";				// 氏名
	public const string KBN_SEARCHKEY_USERCOUPON_LIST_NAME_KANA = "2";			// フリガナ
	public const string KBN_SEARCHKEY_USERCOUPON_LIST_TEL1 = "3";				// 電話番号
	public const string KBN_SEARCHKEY_USERCOUPON_LIST_MAIL_ADDR = "4";			// メールアドレス
	public const string KBN_SEARCHKEY_USERCOUPON_LIST_ZIP1 = "5";				// 郵便番号
	public const string KBN_SEARCHKEY_USERCOUPON_LIST_ADDR = "6";				// 住所
	public const string KBN_SEARCHKEY_USERCOUPON_LIST_COMPANY_NAME = "7";		// 企業名
	public const string KBN_SEARCHKEY_USERCOUPON_LIST_COMPANY_POST_NAME = "8";	// 部署名
	public const string KBN_SEARCHKEY_USERCOUPON_LIST_DEFAULT = KBN_SEARCHKEY_USERCOUPON_LIST_USER_ID;// ユーザーIDがデフォルト

	// 絵文字一覧ソート区分
	public const string KBN_SORT_MOBILEPICTORIALSYMBOL_LIST_SYMBOL_ID_ASC = "0";	// 絵文字ID/昇順
	public const string KBN_SORT_MOBILEPICTORIALSYMBOL_LIST_SYMBOL_ID_DESC = "1";	// 絵文字ID/降順
	public const string KBN_SORT_MOBILEPICTORIALSYMBOL_LIST_SYMBOL_NAME_ASC = "2";	// 絵文字名/昇順
	public const string KBN_SORT_MOBILEPICTORIALSYMBOL_LIST_SYMBOL_NAME_DESC = "3";	// 絵文字名/降順
	public const string KBN_SORT_MOBILEPICTORIALSYMBOL_LIST_SYMBOL_TAG_ASC = "4";	// 絵文字タグ/昇順
	public const string KBN_SORT_MOBILEPICTORIALSYMBOL_LIST_SYMBOL_TAG_DESC = "5";	// 絵文字タグ/降順
	public const string KBN_SORT_MOBILEPICTORIALSYMBOL_LIST_DEFAULT = KBN_SORT_MOBILEPICTORIALSYMBOL_LIST_SYMBOL_ID_ASC;	// 絵文字ID/昇順がデフォルト

	// マスタ出力定義セッティング
	public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_AFFILIATECOOPLOG = "AffiliateCoopLog";		// アフィリエイト連携ログ
	public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_TARGETLISTDATA_USERATTRIBUTE = "TargetListDataUserAttribute";	// ターゲットリスト情報（ユーザー属性）
	/// <summary>広告コード</summary>
	public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_ADVCODE_MEDIA_TYPE = "AdvCodeMediaType";
	/// <summary>ユーザーポイント</summary>
	public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_USERPOINT_DETAIL = "UserPointDetail";
	/// <summary>クーポン</summary>
	public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_USERCOUPON = "UserCoupon";
	/// <summary>クーポン利用ユーザー情報</summary>
	public const string FLG_MASTEREXPORTSETTING_MASTER_KBN_COUPON_USE_USER = "CouponUseUser";

	// ポイントキャンペーンルール一覧検索キー区分
	public const string KBN_SEARCHKEY_POINTRULE_LIST_POINT_INC_KBN = "0";		// ポイント加算区分

	// ポイントキャンペーンルール一覧ソート区分
	public const string KBN_SORT_POINTRULE_LIST_PRIORITY_ASC = "0";		// 優先度/昇順
	public const string KBN_SORT_POINTRULE_LIST_PRIORITY_DESC = "1";		// 優先度/降順
	public static string KBN_SORT_POINTRULE_LIST_DEFAULT = KBN_SORT_POINTRULE_LIST_PRIORITY_ASC;		// 優先度/がデフォルト

	// ポイント区分
	public const string KBN_USERPOINT_LIST_ALL = "ALL";		// 全て
	public const string KBN_USERPOINT_LIST_BASE = "01";		// 通常ポイント
	public const string KBN_USERPOINT_LIST_LIMITED_TERM_POINT = "02";		// 期間限定ポイント
	public const string KBN_USERPOINT_LIST_DEFAULT = KBN_USERPOINT_LIST_ALL;		// 「全て」がデフォルト

	// ユーザーポイント一覧表示区分
	public const string KBN_USERPOINT_DISPLAY_EDIT = "1";				// 編集表示
	public const string KBN_USERPOINT_DISPLAY_COMPLETE = "2";				// 完了表示
	public const string KBN_USERPOINT_DISPLAY_DEFAULT = KBN_USERPOINT_DISPLAY_EDIT;	// 編集表示がデフォルト

	// 商品ランキング区分
	public const string KBN_RANKING_PRODUCT_SEARCH_WORD = "1";	// 商品検索ワードランキング
	public const string KBN_RANKING_PRODUCT_BUY_COUNT = "2";	// 商品販売個数ランキング
	public const string KBN_RANKING_PRODUCT_BUY_PRICE = "3";	// 商品販売金額ランキング

	// 広告コード分析詳細区分
	public const string KBN_ADVCODE_DISP_RANKING_PRODUCT_BUY = "1";		// 商品別売れ行きランキング
	public const string KBN_ADVCODE_DISP_ORDER = "2"; // 購入者詳細

	// 親ページリロード区分
	public const string KBN_RELOAD_PARENT_WINDOW_ON = "1"; // リロードする
	public const string KBN_RELOAD_PARENT_WINDOW_OFF = ""; // リロードしない

	// 消費税 ※売上状況レポートでのみ使用
	public const string KBN_ORDERCONDITIONREPORT_TAX_INCLUDED = "included"; // 税込
	public const string KBN_ORDERCONDITIONREPORT_TAX_EXCLUDE = "exclude"; // 税抜

	// Advertisement Code Media Type sort classification
	public const string KBN_SORT_ADVERTISEMENTCODEMEDIATYPE_DATE_CREATED_ASC = "0";					// 登録日/昇順
	public const string KBN_SORT_ADVERTISEMENTCODEMEDIATYPE_DATE_CREATED_DESC = "1";				// 登録日/降順
	public const string KBN_SORT_ADVERTISEMENTCODEMEDIATYPE_ADVCODE_MEDIA_TYPE_ID_ASC = "2";		// 区分ID/昇順
	public const string KBN_SORT_ADVERTISEMENTCODEMEDIATYPE_ADVCODE_MEDIA_TYPE_ID_DESC = "3";		// 区分ID/降順
	public static string KBN_SORT_ADVERTISEMENTCODEMEDIATYPE_DEFAULT = KBN_SORT_ADVERTISEMENTCODEMEDIATYPE_DATE_CREATED_DESC;	// 登録日/降順がデフォルト

	/// <summary></summary>
	public const string KBN_SORT_ADVCODE_DATE_CREATED_ASC = "0";	//作成日/昇順
	public const string KBN_SORT_ADVCODE_DATE_CREATED_DESC = "1";	//作成日/降順
	public const string KBN_SORT_ADVCODE_ADVERTISEMENT_CODE_ASC = "2";	//広告コード/昇順
	public const string KBN_SORT_ADVCODE_ADVERTISEMENT_CODE_DESC = "3";	//広告コード/降順
	public const string KBN_SORT_ADVCODE_ADVCODE_MEDIA_TYPE_ID_ASC = "4";	// 広告媒体区分ID/昇順
	public const string KBN_SORT_ADVCODE_ADVCODE_MEDIA_TYPE_ID_DESC = "5";	// 広告媒体区分ID/降順
	public const string KBN_SORT_ADVCODE_MEDIA_TYPE_DISPLAY_ORDER_ASC = "6";	// 広告媒体区分表示順/昇順
	public const string KBN_SORT_ADVCODE_MEDIA_TYPE_DISPLAY_ORDER_DESC = "7";	// 広告媒体区分表示順/降順
	public const string KBN_SORT_ADVCODE_DATE_CHANGED_ASC = "8";	//更新日/昇順
	public const string KBN_SORT_ADVCODE_DATE_CHANGED_DESC = "9";	//更新日/降順
	public static string KBN_SORT_ADVCODE_DEFAULT = KBN_SORT_ADVCODE_DATE_CREATED_ASC;

	// クーポン利用ユーザー一覧 表示区分
	public const string KBN_COUPONUSEUSER_DISPLAY_LIST = "LIST";	// 一覧表示
	public const string KBN_COUPONUSEUSER_DISPLAY_COMPLETE = "COMPLETE";	// 完了表示
	public const string KBN_COUPONUSEUSER_DISPLAY_DEFAULT = KBN_COUPONUSEUSER_DISPLAY_LIST; // 一覧表示がデフォルト

	//商品検索区分
	public const string KBN_PRODUCT_SEARCH_PRODUCT = "product";				// 商品検索

	//*****************************************************************************************
	// パス・設定ファイル系
	//*****************************************************************************************
	//=========================================================================================
	// 設定XML系
	//=========================================================================================
	// 設定Xmlルート（外部からアクセスできないようにします。）
	public const string PATH_SETTING_XML = "Xml/";

	// 区分分析セッティングXML格納パス
	public const string PATH_MANAGER_KBN_ANALYSIS = "Xml/KbnAnalysis/";
	// ランキングセッティングXML格納パス
	public const string PATH_MANAGER_RANKING = "Xml/Ranking/";

	// アクセスレポートセッティング
	public const string FILE_XML_CHART_ACCESS_REPORT = "Xml/Chart/AccessReport.xml";
	// 管理メニューセッティング
	public const string FILE_XML_MENU_SETTING = "Xml/Menu.xml";
	// 機能一覧メニューセッティング
	public const string FILE_XML_PAGE_INDEX_LIST = "Xml/PageIndexList.xml";
	// アフィリエイトタグセッティング
	public const string PATH_AFFILIATET_TAG_SETTING = "Xml/AffiliateTagSetting.xml";

	// メール配信文章タグマニュアル
	public const string FILE_XML_MAILDISTTEXTTAG_MANUAL = "Xml/Manual/MailDistTextTagManual.xml";

	//*****************************************************************************************
	// キー系
	//*****************************************************************************************
	//=========================================================================================
	// セッション変数キー
	//=========================================================================================
	public const string SESSION_KEY_LOGIN_OPERTOR_MENU = "w2mpMng_loggedin_operator_menu";
	public const string SESSION_KEY_LOGIN_OPERTOR_MENU_ACCESS_LEVEL = "w2cmpMng_loggedin_operator_menu_access_level";

	public const string SESSION_KEY_ACTION_STATUS = "w2mpMng_action_status";
	public const string SESSION_KEY_LOGIN_ERROR_INFO = "w2mpMng_loggedin_error_info";	// ログイン失敗回数保持用
	public const string SESSION_KEY_ORIGIN_PAGE = "w2mpMng_origin_page";	// 元ページ名

	public const string SESSION_KEY_MENUAUTHORITY_LMENUS = "menu_auth_lmenus";		// メニュー権限情報（大メニュー）
	public const string SESSION_KEY_MENUAUTHORITY_INFO = "menu_auth_info";		// メニュー権限情報（メニュー権限情報）

	/// <summary>ターゲットリストセット</summary>
	public const string SESSION_KEY_TARGET_SET = "target_set";

	//=========================================================================================
	// セッションパラメタキー
	//=========================================================================================
	public const string SESSIONPARAM_KEY_MEMBERRANK_INFO = "member_rank_info";			//会員ランク情報
	public const string SESSIONPARAM_KEY_MEMBERRANKRULE_INFO = "member_rank_rulu_info";	//会員ランク変動ルール情報
	public const string SESSIONPARAM_KEY_USERPOINT_INFO = "user_point_info";			// ユーザーポイント情報
	public const string SESSIONPARAM_KEY_POINTRULE_CAMPAIGN_CALENDAR_INFO = "point_rule_calendar_info";	// ポイントキャンペーンルール情報(カレンダー)
	public const string SESSIONPARAM_KEY_OPERATOR_INFO = "operator_info";		// オペレータ情報
	public const string SESSIONPARAM_KEY_COUPON_INFO = "coupon_info";		// クーポン情報
	public const string SESSIONPARAM_KEY_COUPON_SEARCH_INFO = "coupon_search_info";		// クーポン検索情報
	public const string SESSIONPARAM_KEY_USER_SEARCH_INFO = "user_search_info";	// ユーザ検索情報
	public const string SESSIONPARAM_KEY_USERPOINT_SEARCH_INFO = "userpoint_search_info";	// ユーザポイント検索情報
	public const string SESSIONPARAM_KEY_ADVCODE_INFO = "advertisement_code_info";			// Advertisement code info
	public const string SESSIONPARAM_KEY_ADVCODE_SEARCH_INFO = "advertisement_code_search_info";			// Advertisement code search info
	public const string SESSIONPARAM_KEY_AFFILIATE_TAG_INFO = "affiliate_tag_info";					// アフィリエイトタグ設定
	public const string SESSIONPARAM_KEY_AFFILIATE_TAG_SEARCH_INFO = "affiliate_tag_search_info";	// アフィリエイトタグ検索情報
	public const string SESSIONPARAM_KEY_TARGETLIST_SEARCH_INFO = "targetlist_search_info";			// ターゲットリスト検索情報
	public const string SESSIONPARAM_KEY_POINTRULESCHEDULE_INFO = "pointruleschedule_info"; // ポイントルールスケジュール情報
	public const string SESSIONPARAM_KEY_COUPONSCHEDULE_INFO = "couponschedule_info"; // クーポン発行スケジュール情報
	public const string SESSIONPARAM_KEY_POINTRULE_CAMPAIGN_SEARCH_INFO = "pointrule_campaign_searchi_info";	// キャンペーン設定検索情報
	/// <summary>定期売上予測レポート情報</summary>
	public const string SESSIONPARAM_KEY_FIXED_PURCHASE_FORECAST_INFO = "FixedPurchaseForecastInfo";
	/// <summary>定期継続分析レポート情報</summary>
	public const string SESSIONPARAM_KEY_FIXED_PURCHASE_REPEAT_ANALYSIS_REPORT_SEARCH_INFO = "fixedpurchaserepeatanalysisreport_search_info";

	//=========================================================================================
	// リクエストキー
	//=========================================================================================
	// 共通
	new public const string REQUEST_KEY_SHOP_ID = "shop_id";

	// 共通：商品系
	new public const string REQUEST_KEY_SEARCH_WORD = "swrd";					// 検索ワード

	public const string REQUEST_KEY_MENUAUTHORITY_LEVEL = "mauth_level";			// メニュー権限
	public const string REQUEST_KEY_USERID = "user_id";				// ユーザーID
	public const string REQUEST_KEY_MEMBERRANK_ID = "memberrank_id";				// 会員ランクID
	public const string REQUEST_KEY_MEMBERRANKRULE_ID = "memberrankrule_id";		// 会員ランク変動ルールID
	public const string REQUEST_KEY_MEMBERRANKRULE_NAME = "memberrankrule_name";	// 会員ランク変動ルール名
	public const string REQUEST_KEY_POINTRULE_ID = "point_rule_id";			// ポイントルールID
	public const string REQUEST_KEY_MAILTEXT_ID = "mailtext_id";			// メール文章ID
	public const string REQUEST_KEY_MAILTEXT_NAME = "mailtext_name";			// メール文章名
	public const string REQUEST_KEY_MAILDIST_ID = "maildist_id";			// メール配信ID
	public const string REQUEST_KEY_MAILDIST_NAME = "maildist_name";			// メール配信名
	public const string REQUEST_KEY_ACTION_NO = "action_id";			// アクションNO
	public const string REQUEST_KEY_MAILCLICK_URL = "mailclick_url";			// メールクリックURL
	public const string REQUEST_KEY_MAILCLICK_KEY = "mailclick_key";			// メールクリックキー
	public const string REQUEST_KEY_MAILCLICK_ID = "mailclick_id";			// メールクリックID
	public const string REQUEST_KEY_TARGET_ID = "tlid";			// ターゲットリストID
	public const string REQUEST_KEY_TARGET_NAME = "tlname";			// ターゲットリスト名
	public const string REQUEST_KEY_ANSWER_ID = "answer_id";				// 回答ID
	public const string REQUEST_KEY_CURRENT_YEAR = "cur_year";
	public const string REQUEST_KEY_CURRENT_MONTH = "cur_month";
	public const string REQUEST_KEY_CURRENT_DAY = "cur_day";
	public const string REQUEST_KEY_TARGET_YEAR = "tgt_year";
	public const string REQUEST_KEY_TARGET_MONTH = "tgt_month";
	public const string REQUEST_KEY_TARGET_DAY = "tgt_day";
	public const string REQUEST_KEY_NUMBER_OF_DAYS = "number_of_days";
	public const string REQUEST_KEY_ERROR_STATUS = "error_status";
	public const string REQUEST_KEY_DISPLAY_KBN = "dsiplay_kbn";			// 表示区分
	public const string REQUEST_KEY_ACCESS_KBN = "access_kbn";			// アクセス区分
	public const string REQUEST_KEY_SEARCH_KEY = "skey";					// 検索フィールド
	public const string REQUEST_KEY_TAX_DISPLAY_TYPE = "taxdisptype"; // 税込・税抜の表示タイプ
	public const string REQUEST_KEY_PRODUCT_UNIT_TYPE = "product_unit_type";
	public const string REQUEST_KEY_ORDER_ID = "order_id";	// 注文ID
	public const string REQUEST_KEY_POINTRULESCHEDULE_ID = "prs_id"; // ポイントルールスケジュールID
	public const string REQUEST_KEY_POINTRULESCHEDULE_NAME = "prs_name"; // ポイントルールスケジュール名
	public const string REQUEST_KEY_ACTION_KBN = "action_kbn"; // アクション区分
	public const string REQUEST_KEY_MASTER_ID = "master_id"; // マスターID
	public const string REQUEST_KEY_CURRENT_TIME = "cur_time";				// Current Time
	public const string REQUEST_KEY_TARGET_TIME = "tgt_time";				// Target Time

	// 会員情報
	public const string REQUEST_KEY_USER_USER_ID = "ui";						// ユーザーID
	public const string REQUEST_KEY_USER_USER_KBN = "uk";						// 顧客区分
	public const string REQUEST_KEY_USER_NAME = "un";						// 氏名
	public const string REQUEST_KEY_USER_NAME_KANA = "unk";					// 氏名(かな)
	public const string REQUEST_KEY_USER_MAIL_FLG = "umf";					// メール配信希望
	public const string REQUEST_KEY_USER_TEL1 = "ut";						// 電話番号
	public const string REQUEST_KEY_USER_MAIL_ADDR = "uma";					// メールアドレス
	public const string REQUEST_KEY_USER_ZIP = "uz";						// 郵便番号
	public const string REQUEST_KEY_USER_ADDR = "ua";						// 住所
	public const string REQUEST_KEY_USER_COMPANY_NAME = "ucn"; // 企業名
	public const string REQUEST_KEY_USER_COMPANY_POST_NAME = "ucpn"; // 部署名
	public const string REQUEST_KEY_USER_DEL_FLG = "udf";					// 退会者
	public const string REQUEST_KEY_MAIL_ERROR_POINT = "mep";						// メール配信エラーポイント
	public const string REQUEST_KEY_USER_MALL_ID = "umi";					// モールID
	public const string REQUEST_KEY_USER_PROPERTY_NAME = "upn";						// プロパティ名
	public const string REQUEST_KEY_USER_PROPERTY_VALUE = "upv";						// プロパティ
	public const string REQUEST_KEY_USER_PROPERTY_AND_OR = "ao";						// プロパティ
	public const string REQUEST_KEY_USER_USER_MANAGEMENT_LEVEL_ID = "umli"; // ユーザー管理レベルID
	public const string REQUEST_KEY_USER_USER_MANAGEMENT_LEVEL_EXCLUDE = "umle"; // 選択したユーザー管理レベルを除いて検索するか、しないか

	// 会員ランク更新履歴
	public const string REQUEST_KEY_USERMEMBERRANKHIS_USER_ID = "usid";				// ユーザーID
	public const string REQUEST_KEY_USERMEMBERRANKHIS_BEFORE_RANK_ID = "brid";		// 更新前ランクID
	public const string REQUEST_KEY_USERMEMBERRANKHIS_AFTER_RANK_ID = "arid";		// 更新後ランクID
	public const string REQUEST_KEY_USERMEMBERRANKHIS_DATE_FROM = "datef";			// Member rank date from
	public const string REQUEST_KEY_USERMEMBERRANKHIS_TIME_FROM = "timef";			// Member rank time from
	public const string REQUEST_KEY_USERMEMBERRANKHIS_DATE_TO = "datet";			// Member rank date to
	public const string REQUEST_KEY_USERMEMBERRANKHIS_TIME_TO = "timet";			// Member rank time to

	// 商品ランキング
	public const string REQUEST_KEY_RANKING_PRODUCT_TYPE = "rpt";			// 商品ランキング区分
	public const string REQUEST_KEY_RANKING_PRODUCT_VALUE = "rpv";			// 商品ランキング値
	public const string REQUEST_KEY_RANKING_PRODUCT_ACCESS_KBN = "rpak";	// 商品ランキングアクセス区分

	// エラーページ
	public const string REQUEST_KEY_ERRORPAGE_ERRORKBN = "errkbn";			// エラー区分

	// ユーザポイント情報
	public const string REQUEST_KEY_POINT_KBN = "pk";						// ポイント区分

	// ユーザーポイント履歴
	public const string REQUEST_KEY_USERPOINTHISTORY_HISTORY_GROUP_NO = "phgn"; // ポイント履歴グループ番号

	// クーポン情報
	public const string REQUEST_KEY_COUPON_COUPON_ID = "ci";						// クーポンID
	public const string REQUEST_KEY_COUPON_COUPON_CODE = "cc";						// クーポンコード
	public const string REQUEST_KEY_COUPON_COUPON_NAME = "cn";						// 管理用クーポン名
	public const string REQUEST_KEY_COUPON_COUPON_TYPE = "ct";						// クーポン種別
	public const string REQUEST_KEY_COUPON_PUBLISH_DATE = "cpd";						// 発行ステータス
	public const string REQUEST_KEY_COUPON_VALID_FLG = "cvf";						// 有効フラグ

	// クーポン利用ユーザー一覧
	public const string REQUEST_KEY_COUPONUSEUSER_USER_ID = "cui";	// ユーザーID
	public const string REQUEST_KEY_COUPONUSEUSER_USER_NAME = "cun";	// 氏名
	public const string REQUEST_KEY_COUPONUSEUSER_MAIL_ADDRESS = "cma";	// メールアドレス
	public const string REQUEST_KEY_COUPONUSEUSER_COUPON_ID = "cci";	// クーポンID
	public const string REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_YEAR_FROM = "cdcyf";	// 作成年 From
	public const string REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_MONTH_FROM = "cdcmf";	// 作成月 From
	public const string REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_DAY_FROM = "cdcdf";	// 作成日 From
	public const string REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_YEAR_TO = "cdcyt";	// 作成年 To
	public const string REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_MONTH_TO = "cdcmt";	// 作成月 To
	public const string REQUEST_KEY_COUPONUSEUSER_DATE_CREATED_DAY_TO = "cdcdt";	// 作成日 To

	// クーポン発行スケジュール情報
	public const string REQUEST_KEY_COUPONSCHEDULE_ID = "cps_id"; // クーポン発行スケジュールID
	public const string REQUEST_KEY_COUPONSCHEDULE_NAME = "cps_name"; // クーポン発行スケジュール名

	// アフィリエイトタグ設定
	public const string REQUEST_KEY_AFFILIATETAGSETTING_AFFILIATE_ID = "afid";		// アフィリエイトID
	public const string REQUEST_KEY_AFFILIATETAGSETTING_AFFILIATE_NAME = "afname";	// アフィリエイト名
	public const string REQUEST_KEY_AFFILIATETAGSETTING_AFFILIATE_KBN = "afkbn";	// アフィリエイト区分（PC,Mobile）
	public const string REQUEST_KEY_AFFILIATETAGSETTING_VALID_FLG = "afvf";			// 有効フラグ
	public const string REQUEST_KEY_AFFILIATETAGSETTING_PAGE = "afp";				// 表示画面

	// アフィリエイト商品タグ設定
	public const string REQUEST_KEY_AFFILIATEPRODUCTTAGSETTING_TAG_NAME = "afptg";	// アフィリエイト商品タグ名

	/// <summary>タグ閲覧権限：タグID</summary>
	public const string REQUEST_KEY_TAG_AUTHORITY_TAG_ID = "tati";
	/// <summary>タグ閲覧権限：広告媒体区分</summary>
	public const string REQUEST_KEY_TAG_AUTHORITY_MEDIA_TYPE = "tamt";
	/// <summary>タグ閲覧権限：設定区分</summary>
	public const string REQUEST_KEY_TAG_AUTHORITY_AUTHORITY_KBN = "taak";

	// アフィリエイトレポート
	public const string REQUEST_KEY_AFFILIATET_REPORT_LOG_NO = "lgno";				// ログNo
	public const string REQUEST_KEY_AFFILIATET_REPORT_AFFILIATE_KBN = "afkbn";		// アフィリエイト区分
	public const string REQUEST_KEY_AFFILIATET_REPORT_MASTER_ID = "msid";			// マスタID
	public const string REQUEST_KEY_AFFILIATET_REPORT_DATE_FROM = "afrdf";			// Requets Key Affiliatet Report Date From
	public const string REQUEST_KEY_AFFILIATET_REPORT_TIME_FROM = "afrtf";			// Requets Key Affiliatet Report Time From
	public const string REQUEST_KEY_AFFILIATET_REPORT_DATE_TO = "afrdt";				// Requets Key Affiliatet Report Date To
	public const string REQUEST_KEY_AFFILIATET_REPORT_TIME_TO = "afrtt";				// Requets Key Affiliatet Report Time To
	public const string REQUEST_KEY_AFFILIATET_REPORT_TAG_ID = "tgid";		//タグID

	// モバイル絵文字
	public const string REQUEST_KEY_MOBILEPICTORIALSYMBOL_SYMBOL_ID = "sblid";          // 絵文字ID
	public const string REQUEST_KEY_MOBILEPICTORIALSYMBOL_SYMBOL_TAG = "sbltg";          // 絵文字タグ
	public const string REQUEST_KEY_MOBILEPICTORIALSYMBOL_SYMBOL_NAME = "sblnm";          // 絵文字名
	public const string REQUEST_KEY_MOBILEPICTORIALSYMBOL_INSERTTO = "insto";          // 絵文字タグインサート対象

	// 広告コード分析
	public const string REQUEST_KEY_ADVCODE_ADVCODE_NO = "aacn";					// Advertisement Code NO
	public const string REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE = "aac";				// 広告コード
	public const string REQUEST_KEY_ADVCODE_CAREER_ID = "aamci";				// キャリアID
	public const string REQUEST_KEY_ADVCODE_TERM_FROM = "aatf";						// 日付(FROM)
	public const string REQUEST_KEY_ADVCODE_TERM_TO = "aatt";						// 日付(TO)
	public const string REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE_TARGET = "aact";		// 広告コード対象
	public const string REQUEST_KEY_ADVCODE_DISP_KBN = "adk";						// 表示区分
	public const string REQUEST_KEY_ADVCODE_MEDIA_NAME = "amn";						// Media Name
	public const string REQUEST_KEY_ADVCODE_MEDIA_TYPE_ID = "amtid";				// Media Type Id
	public const string REQUEST_KEY_ADVCODE_MEMBERRANK_ID = "amrid";	// 会員ランクID
	public const string REQUEST_KEY_ADVCODE_USER_MANAGEMENT_LEVEL_ID = "aumlid";	// ユーザー管理レベルID

	/// <summary>オペレータID</summary>
	public const string REQUEST_KEY_SHOP_OPERATOR_ID = "opid";					// オペレータID

	// Advertisement Code Media type name
	public const string REQUEST_KEY_ADVCODEMEDIATYPE_MEDIA_TYPE_NAME = "amtn";		// Advertisement code media type name

	// ポップアップマスター
	public const string REQUEST_KEY_RELOAD_PARENT_WINDOW = "pnt_win_rld";			// 親画面リロード

	// 商品情報
	public const string REQUEST_KEY_PRODUCT_PRODUCT_ID = "pi";						// 商品ID
	public const string REQUEST_KEY_PRODUCT_NAME = "pn";						// 商品名
	public const string REQUEST_KEY_PRODUCT_BRAND_ID = "bid";					// ブランドID
	public const string REQUEST_KEY_PRODUCT_VALID_FLG = "pvf";					// 有効フラグ
	public const string REQUEST_KEY_PRODUCT_SEARCH_KBN = "sk";			// 商品検索区分

	/// <summary>定期売上予測レポート(商品ID)</summary>
	public const string REQUEST_KEY_FIXEDPURCHASE_FORECAST_PRODUCT_ID = "pi";
	/// <summary>定期売上予測レポート(商品名)</summary>
	public const string REQUEST_KEY_FIXEDPURCHASE_FORECAST_PRODUCT_NAME = "pn";
	/// <summary>定期売上予測レポート(配送種別)</summary>
	public const string REQUEST_KEY_FIXEDPURCHASE_FORECAST_SHIPPING_NAME = "pst";
	/// <summary>定期売上予測レポート(カテゴリID)</summary>
	public const string REQUEST_KEY_FIXEDPURCHASE_FORECAST_CATEGORY_ID = "pci";
	/// <summary>定期売上予測レポート(表示区分)</summary>
	public const string REQUEST_KEY_FIXEDPURCHASE_FORECAST_DISPLAY_KBN = "dkbn";

	/// <summary>オペレータ名</summary>
	public const string REQUEST_KEY_OPERATOR_NAME = "operator_name";
	/// <summary>メニュー権限</summary>
	public const string REQUEST_KEY_OPERATOR_MENUACCESS_LEVEL = "mal";
	/// <summary>オペレータ有効フラグ</summary>
	public const string REQUEST_KEY_OPERATOR_VALID_FLG = "ovf";

	/// <summary>Request key order condition report time from</summary>
	public const string REQUEST_KEY_REPORT_TIME_FROM = "krtf";
	/// <summary>Request key order condition report time to</summary>
	public const string REQUEST_KEY_REPORT_TIME_TO = "krtt";

	//*****************************************************************************************
	// シルバーエッグアイジェントレコメンドレポート
	//*****************************************************************************************
	public const string JSON_FIELD_RECOMMEND_REPORT_TYPE = "type";			// 結果コード（complete / error）
	public const string JSON_FIELD_RECOMMEND_REPORT_CURRENCY = "currency";			// 通貨単位（前）
	public const string JSON_FIELD_RECOMMEND_REPORT_CURRENCYPOSTFIX = "currency_postfix";			// 通貨単位（後）
	public const string JSON_FIELD_RECOMMEND_REPORT_ROW = "rows";			// 結果リスト
	public const string JSON_FIELD_RECOMMEND_REPORT_MESSAGE = "message";			// 結果メッセージ
	public const string JSON_FIELD_RECOMMEND_REPORT_ISACTIVE = "is_active";			// 稼働状況
	public const string JSON_FIELD_RECOMMEND_REPORT_MERCHANT = "merchant";			// マーチャントID
	public const string JSON_FIELD_RECOMMEND_REPORT_FROM = "from";			// 集計開始日・月（YYYYMMDD / YYYYMM）
	public const string JSON_FIELD_RECOMMEND_REPORT_TO = "to";			// 集計終了日・月（YYYYMMDD / YYYYMM）
	public const string JSON_FIELD_RECOMMEND_REPORT_DATE = "date";			// 集計日・月（YYYYMMDD / YYYYMM）
	public const string JSON_FIELD_RECOMMEND_REPORT_SPEC = "spec";			// レコメンドID

	//*****************************************************************************************
	// その他
	//*****************************************************************************************
	public const string ACTION_STATUS_UPDATE = "update";		// 変更
	public const string ACTION_STATUS_INSERT = "insert";		// 登録
	public const string ACTION_STATUS_COPY_INSERT = "copyinsert";	// コピー登録
	public const string ACTION_STATUS_DELETE = "delete";		// 削除
	public const string ACTION_STATUS_CONFIRM = "confirm";	// 確認
	public const string ACTION_STATUS_COMPLETE = "complete";	// 完了
	public const string ACTION_STATUS_UPLOAD = "upload";					// アップロード
	public const string ACTION_STATUS_UPLOAD_CONFIRM = "upload_confirm";	//アップロード確認
	public const string ACTION_STATUS_GLOBAL_SETTING_INSERT = "globalsettinginsert";	// グローバル設定登録
	public const string ERROR_REQUEST_PRAMETER = "err_parameter";	// 不正パラメータ
	public static string STRING_SUPERUSER_NAME = "";	// スーパーユーザー名
	public static string STRING_UNACCESSABLEUSER_NAME = "";	// アクセス権限なしユーザ名

	public const string SETTING_TABLE_NAME = "setting_table_name";  // 設定テーブル名(メール配信文章のグローバル設定登録に使用)

	/// <summary>クーポンスケジュール配信用メール配信ID接頭句</summary>
	public const string MAILDIST_ID_PREFIX = "c";
	/// <summary>メール配信ID接頭句格納用</summary>
	public const string MAILDIST_ID_PREFIX_NAME = "maildist_id_prefix";

	// ValueText
	/// <summary>アクセスレポート</summary>
	public const string VALUETEXT_PARAM_ACCESS_REPORT = "access_report";
	/// <summary>アクセスレポート 表示種別</summary>
	public const string VALUETEXT_PARAM_ACCESS_REPORT_REPORT_TYPE = "report_type";
	/// <summary>広告コードレポート</summary>
	public const string VALUETEXT_PARAM_ADVC_REPORT = "advc_report";
	/// <summary>広告コードレポート レポート区分</summary>
	public const string VALUETEXT_PARAM_ADVC_REPORT_KBN = "kbn";
	/// <summary>アクセスランキングレポート</summary>
	public const string VALUETEXT_PARAM_ACCESS_RANKING = "access_ranking";
	/// <summary>アクセスランキングレポート レポート表示種別</summary>
	public const string VALUETEXT_PARAM_ACCESS_RANKING_REPORT_TYPE = "report_type";
	/// <summary>顧客状況レポート</summary>
	public const string VALUETEXT_PARAM_USER_CONDITION_REPORT = "user_condition_report";
	/// <summary>顧客状況レポート レポート表示種別</summary>
	public const string VALUETEXT_PARAM_USER_CONDITION_REPORT_DISPLAY_KBN = "display_kbn";
	/// <summary>商品ランキングレポート</summary>
	public const string VALUETEXT_PARAM_PRODUCT_RANKING_REPORT = "product_ranking_report";
	/// <summary>商品ランキングレポート レポート表示種別</summary>
	public const string VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_RANKING_TYPE = "ranking_type";
	/// <summary>定期継続分析レポート</summary>
	public const string VALUETEXT_PARAM_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT = "fixed_purchase_repeate_analysis_report";
	/// <summary>定期継続分析レポート: レポート表示種別(種別)</summary>
	public const string VALUETEXT_PARAM_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT_DISPLAY_KBN_TYPE = "display_kbn_type";
	/// <summary>定期継続分析レポート: レポート表示種別(種別): LTVレポート</summary>
	public const string VALUETEXT_PARAM_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT_DISPLAY_KBN_TYPE_LTV = "ltv";
	/// <summary>定期継続分析レポート: レポート表示種別(種別): 回数別レポート</summary>
	public const string VALUETEXT_PARAM_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT_DISPLAY_KBN_TYPE_FREQUENCY = "frequency";
	/// <summary>定期継続分析レポート: タイトル: LTV</summary>
	public const string VALUETEXT_PARAM_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT_TITLE_LTV = "ltv_title";
	/// <summary>定期継続分析レポート: タイトル: 回数別</summary>
	public const string VALUETEXT_PARAM_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT_TITLE_FREQUENCY = "frequency_title";
	/// <summary>定期継続分析レポート: target list</summary>
	public const string VALUETEXT_PARAM_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT_TARGET_LIST = "target_list";
	/// <summary>定期継続分析レポート: target list: LTV report</summary>
	public const string VALUETEXT_PARAM_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT_TARGET_LIST_LTV_REPORT = "ltv_report";
	/// <summary>売上状況レポート</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_REPORT = "order_condition_report";
	/// <summary>売上状況レポート レポート表示種別(期間)</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_REPORT_DISPLAY_KBN_PERIOD = "display_kbn_period";
	/// <summary>売上状況レポート レポート表示種別(種別)</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_REPORT_DISPLAY_KBN_TYPE = "display_kbn_type";
	/// <summary>売上状況レポート レポート表示種別(種別) 注文基準</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_REPORT_DISPLAY_KBN_TYPE_ORDER = "order";
	/// <summary>売上状況レポート レポート表示種別(種別) 商品基準</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_REPORT_DISPLAY_KBN_TYPE_PRODUCT = "product";
	/// <summary>売上状況レポート レポート表示種別(税区分)</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_REPORT_DISPLAY_KBN_TAX = "display_kbn_tax";
	/// <summary>売上状況レポート レポート表示種別(税区分) 税込</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_REPORT_DISPLAY_KBN_TAX_INCLUDED = "included";
	/// <summary>売上状況レポート レポート表示種別(税区分) 税抜</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_REPORT_DISPLAY_KBN_TAX_EXCLUDED = "excluded";
	/// <summary>ターゲットリスト設定</summary>
	public const string VALUETEXT_PARAM_TARGET_LIST_CONDITION = "condition";
	/// <summary>ターゲットリスト設定 ターゲット名</summary>
	public const string VALUETEXT_PARAM_TARGET_LIST_CONDITION_TARGET_LIST_NAME = "target_list_name";
	/// <summary>ターゲットリスト設定 データ区分</summary>
	public const string VALUETEXT_PARAM_TARGET_LIST_CONDITION_DATA_KBN = "data_kbn";
	/// <summary>ターゲットリスト設定 データフィールド</summary>
	public const string VALUETEXT_PARAM_TARGET_LIST_CONDITION_DATA_FIELD = "data_field";
	/// <summary>ターゲットリスト設定 評価式フィールド</summary>
	public const string VALUETEXT_PARAM_TARGET_LIST_CONDITION_EQUAL_SIGN = "equal_sign";
	/// <summary>ターゲットリスト設定 メッセージフォーマット</summary>
	public const string VALUETEXT_PARAM_TARGET_LIST_CONDITION_MESSAGE_TITLE_FORMAT = "message_title_format";
	/// <summary>定期購入回数別レポート</summary>
	public const string VALUETEXT_PARAM_ORDER_REPEAT_REPORT = "order_repeat_report";
	/// <summary>定期購入回数別レポート 注文種別No</summary>
	public const string VALUETEXT_PARAM_ORDER_REPEAT_REPORT_ORDER_TYPE_NUMBER = "order_type_number";
	/// <summary>定期購入回数別レポート 注文種別名</summary>
	public const string VALUETEXT_PARAM_ORDER_REPEAT_REPORT_ORDER_TYPE_TEXT = "order_type_text";
	/// <summary>定期購入回数別レポート ToolTip</summary>
	public const string VALUETEXT_PARAM_ORDER_REPEAT_REPORT_TOOL_TIP = "tool_tip";

	/// <summary>Value text param access ranking list</summary>
	public const string VALUETEXT_PARAM_ACCESS_RANKING_LIST = "access_ranking_list";
	/// <summary>Value text param access ranking list title</summary>
	public const string VALUETEXT_PARAM_ACCESS_RANKING_LIST_TITLE = "access_ranking_list_title";
	/// <summary>Value text param access ranking list rank</summary>
	public const string VALUETEXT_PARAM_ACCESS_RANKING_LIST_RANK = "rank";
	/// <summary>Value text param access ranking list composition raito</summary>
	public const string VALUETEXT_PARAM_ACCESS_RANKING_LIST_COMPOSITION_RATIO = "composition_ratio";

	/// <summary>Value text param access report list title</summary>
	public const string VALUETEXT_PARAM_ACCESS_REPORT_TITLE = "access_report_list_title";
	/// <summary>Value text param access report list date</summary>
	public const string VALUETEXT_PARAM_ACCESS_REPORT_LIST_DATE = "date";
	/// <summary>Value text param access report list number of page view</summary>
	public const string VALUETEXT_PARAM_ACCESS_REPORT_LIST_NUMBER_PAGE_VIEW = "number_page_view";
	/// <summary>Value text param access report list number of user</summary>
	public const string VALUETEXT_PARAM_ACCESS_REPORT_LIST_NUMBER_USER = "number_user";
	/// <summary>Value text param access report list man</summary>
	public const string VALUETEXT_PARAM_ACCESS_REPORT_LIST_MAN = "man";
	/// <summary>Value text param access report list number of visit</summary>
	public const string VALUETEXT_PARAM_ACCESS_REPORT_LIST_NUMBER_VISIT = "number_visit";
	/// <summary>Value text param access report list time</summary>
	public const string VALUETEXT_PARAM_ACCESS_REPORT_LIST_TIME = "time";
	/// <summary>Value text param access report table list title</summary>
	public const string VALUETEXT_PARAM_ACCESS_REPORT_TABLE_LIST_TITLE = "access_report_table_list_title";
	/// <summary>Value text param access report table list report classification</summary>
	public const string VALUETEXT_PARAM_ACCESS_REPORT_TABLE_LIST_REPORT_CLASSIFICATION = "report_classification";
	/// <summary>Value text param access report table list total</summary>
	public const string VALUETEXT_PARAM_ACCESS_REPORT_TABLE_LIST_TOTAL = "total";
	/// <summary>Value text param access report table list pc/smartphone</summary>
	public const string VALUETEXT_PARAM_ACCESS_REPORT_TABLE_LIST_PC_SMARTPHONE = "pc_smartphone";
	/// <summary>Value text param access report table list number of visiting user</summary>
	public const string VALUETEXT_PARAM_ACCESS_REPORT_TABLE_LIST_NUMBER_VISITING_USER = "number_visiting_user";
	/// <summary>Value text param access report table list number of new visitor</summary>
	public const string VALUETEXT_PARAM_ACCESS_REPORT_TABLE_LIST_NUMBER_NEW_VISITOR = "number_new_visitor";
	/// <summary>Value text param access report table list number of repeat visiting user</summary>
	public const string VALUETEXT_PARAM_ACCESS_REPORT_TABLE_LIST_NUMBER_REPEAT_VISITING_USER = "number_repeat_visiting_user";
	/// <summary>Value text param access report table list average number of page view per visit</summary>
	public const string VALUETEXT_PARAM_ACCESS_REPORT_TABLE_LIST_AVERAGE_PAGE_VIEW_PER_VISIT = "average_number_page_view_per_visit";

	/// <summary>Value text param advertisement code report</summary>
	public const string VALUETEXT_PARAM_ADVERTISEMENT_CODE_REPORT = "advertisement_code_report";
	/// <summary>Value text param advertisement code report detail sale display</summary>
	public const string VALUETEXT_PARAM_ADVERTISEMENT_CODE_REPORT_DETAIL_SALE_DISPLAY = "sale_display";
	/// <summary>Value text param advertisement code report detail sale display detail by product</summary>
	public const string VALUETEXT_PARAM_ADVERTISEMENT_CODE_REPORT_DETAIL_SALE_DISPLAY_DETAIL_PRODUCT = "detail_product";
	/// <summary>Value text param advertisement code report detail sale display purchaser detail</summary>
	public const string VALUETEXT_PARAM_ADVERTISEMENT_CODE_REPORT_DETAIL_SALE_DISPLAY_PURCHASER_DETAIL = "purchaser_detail";
	/// <summary>Value text param advertisement code report list title</summary>
	public const string VALUETEXT_PARAM_ADVERTISEMENT_CODE_REPORT_LIST_TITLE = "advertisement_code_report_list_title";
	/// <summary>Value text param advertisement code report list total</summary>
	public const string VALUETEXT_PARAM_ADVERTISEMENT_CODE_REPORT_LIST_TOTAL = "total";

	/// <summary>Value text param coupon confirm</summary>
	public const string VALUETEXT_PARAM_COUPON_CONFIRM = "coupon_confirm";
	/// <summary>Value text param coupon confirm title</summary>
	public const string VALUETEXT_PARAM_COUPON_CONFIRM_TITLE = "coupon_confirm_title";
	/// <summary>Value text param coupon confirm expire date</summary>
	public const string VALUETEXT_PARAM_COUPON_EXPIRE_DATE = "expire_date";

	/// <summary>Value text param coupon list</summary>
	public const string VALUETEXT_PARAM_COUPON_LIST = "coupon_list";
	/// <summary>Value text param coupon list title</summary>
	public const string VALUETEXT_PARAM_COUPON_LIST_TITLE = "coupon_list_title";
	/// <summary>Value text param coupon list before issue period</summary>
	public const string VALUETEXT_PARAM_COUPON_LIST_BEFORE_ISSUE_PERIOD = "before_issue_period";
	/// <summary>Value text param coupon list during issue period</summary>
	public const string VALUETEXT_PARAM_COUPON_LIST_DURING_ISSUE_PERIOD = "during_issue_period";
	/// <summary>Value text param coupon list after issue period</summary>
	public const string VALUETEXT_PARAM_COUPON_LIST_AFTER_ISSUE_PERIOD = "after_issue_period";
	/// <summary>Value text param coupon list free shipping</summary>
	public const string VALUETEXT_PARAM_COUPON_LIST_FREE_SHIPPING = "free_shipping";

	/// <summary>Value text param cpm report list</summary>
	public const string VALUETEXT_PARAM_CPM_REPORT_LIST = "cpm_report_list";
	/// <summary>Value text param cpm report list title</summary>
	public const string VALUETEXT_PARAM_CPM_REPORT_LIST_TITLE = "cpm_report_list_title";
	/// <summary>Value text param cpm report list the day before yesterday</summary>
	public const string VALUETEXT_PARAM_CPM_REPORT_LIST_DAY_BEFORE = "day_before_yesterday";
	/// <summary>Value text param cpm report list date</summary>
	public const string VALUETEXT_PARAM_CPM_REPORT_LIST_DATE = "date";
	/// <summary>Value text param cpm report list man</summary>
	public const string VALUETEXT_PARAM_CPM_REPORT_LIST_MAN = "man";
	/// <summary>Value text param cpm report list month to month basis</summary>
	public const string VALUETEXT_PARAM_CPM_REPORT_LIST_MONHT_TO_MONTH_BASIS = "month_to_month_basis";

	/// <summary>Value text param fixed purchase repeat analysis report</summary>
	public const string VALUETEXT_PARAM_FIXED_PURCHASE_REPEAT_ANALYSIS_REPORT = "fixed_purchase_repeat_analysis_report";
	/// <summary>Value text param fixed purchase repeat analysis report title</summary>
	public const string VALUETEXT_PARAM_FIXED_PURCHASE_REPEAT_ANALYSIS_REPORT_TITLE = "fixed_purchase_repeat_analysis_report_title";
	/// <summary>Value text param fixed purchase repeat analysis report number purchase</summary>
	public const string VALUETEXT_PARAM_FIXED_PURCHASE_REPEAT_ANALYSIS_REPORT_NUMBER_PURCHASE = "number_purchase";
	/// <summary>Value text param fixed purchase repeat analysis report time</summary>
	public const string VALUETEXT_PARAM_FIXED_PURCHASE_REPEAT_ANALYSIS_REPORT_TIME = "time";

	/// <summary>Value text param mail click report detail 2</summary>
	public const string VALUETEXT_PARAM_MAIL_CLICK_REPORT_DETAIL_2 = "mail_click_report_detail_2";
	/// <summary>Value text param mail click report detail 2 report kbn</summary>
	public const string VALUETEXT_PARAM_MAIL_CLICK_REPORT_DETAIL_2_REPORT_KBN = "report_kbn";
	/// <summary>Value text param mail click report detail 2 report kbn daily report</summary>
	public const string VALUETEXT_PARAM_MAIL_CLICK_REPORT_DETAIL_2_REPORT_KBN_DAILY_REPORT = "daily_report";
	/// <summary>Value text param mail click report detail 2 report kbn yearly monthly report</summary>
	public const string VALUETEXT_PARAM_MAIL_CLICK_REPORT_DETAIL_2_REPORT_KBN_YEARLY_MONTHLY_REPORT = "yearly_monthly_report";
	/// <summary>Value text param mail click report detail 2 report kbn minutes</summary>
	public const string VALUETEXT_PARAM_MAIL_CLICK_REPORT_DETAIL_2_REPORT_KBN_MINUTES = "minutes";
	/// <summary>Value text param mail click report detail 2 report kbn year</summary>
	public const string VALUETEXT_PARAM_MAIL_CLICK_REPORT_DETAIL_2_REPORT_KBN_YEAR = "year";
	/// <summary>Value text param mail click report detail 2 time kbn</summary>
	public const string VALUETEXT_PARAM_MAIL_CLICK_REPORT_DETAIL_2_TIME_KBN = "time_kbn";
	/// <summary>Value text param mail click report detail 2 report kbn day</summary>
	public const string VALUETEXT_PARAM_MAIL_CLICK_REPORT_DETAIL_2_TIME_KBN_DAY = "day";
	/// <summary>Value text param mail click report detail 2 report kbn month</summary>
	public const string VALUETEXT_PARAM_MAIL_CLICK_REPORT_DETAIL_2_TIME_KBN_MONTH = "month";

	/// <summary>Value text param order condition report list</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST = "order_condition_report_list";
	/// <summary>Value text param order condition report list title</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_TITLE = "order_condition_report_list_title";
	/// <summary>Value text param order condition time report list</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_TIME_REPORT_LIST = "order_condition_time_report_list";
	/// <summary>Value text param order condition time report list title</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_TIME_REPORT_LIST_TITLE = "order_condition_time_report_list_title";
	/// <summary>Value text param order condition report list time kbn</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_TIME_KBN = "time_kbn";
	/// <summary>Value text param order condition report list time kbn day</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_TIME_KBN_DAY = "day";
	/// <summary>Value text param order condition report list time kbn month</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_TIME_KBN_MONTH = "month";
	/// <summary>Value text param order condition report list time kbn year</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_TIME_KBN_YEAR = "year";
	/// <summary>Value text param order condition report list count kbn</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_COUNT_KBN = "count_kbn";
	/// <summary>Value text param order condition report list count kbn result</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_COUNT_KBN_RESULT = "result";
	/// <summary>Value text param order condition report list count kbn item</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_COUNT_KBN_ITEM = "item";
	/// <summary>Value text param order condition report list count kbn result quantity</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_COUNT_KBN_RESULT_QUANTITY = "result_quantity";
	/// <summary>Value text param order condition report list count kbn item quantity</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_COUNT_KBN_ITEM_QUANTITY = "item_quantity";
	/// <summary>Value text param order condition report list average kbn</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_AVERAGE_KBN = "average_kbn";
	/// <summary>Value text param order condition report list average kbn purchase</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_AVERAGE_KBN_PURCHASE = "purchase";
	/// <summary>Value text param order condition report list average kbn product</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_AVERAGE_KBN_PRODUCT = "product";
	/// <summary>Value text param order condition report list sale kbn</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN = "sale_kbn";
	/// <summary>Value text param order condition report list sale kbn</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN_ORDER_BASIS_AMOUNT = "order_basis_amount";
	/// <summary>Value text param order condition report list sale kbn</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN_ORDER_BASIS = "order_basis";
	/// <summary>Value text param order condition report list sale kbn</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN_ORDER_BASIS_AVERAGE = "order_basis_average";
	/// <summary>Value text param order condition report list sale kbn</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN_SHIPPING_STANDARD_AMOUNT = "shipping_standard_amount";
	/// <summary>Value text param order condition report list sale kbn</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN_SHIPPING_STANDARD = "shipping_standard";
	/// <summary>Value text param order condition report list sale kbn</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN_SHIPPING_STANDARD_AVERAGE = "shipping_standard_average";
	/// <summary>Value text param order condition report list sale kbn</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN_CANCEL_AMOUNT = "cancel_amount";
	/// <summary>Value text param order condition report list sale kbn</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN_CANCEL = "cancel";
	/// <summary>Value text param order condition report list sale kbn</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN_SUBTOTAL_AMOUNT = "subtotal_amount";
	/// <summary>Value text param order condition report list sale kbn</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN_SUBTOTAL = "subtotal";
	/// <summary>Value text param order condition report list sale kbn</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN_RETURN_AMOUNT = "return_amount";
	/// <summary>Value text param order condition report list sale kbn</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN_RETURN = "return";
	/// <summary>Value text param order condition report list sale status report</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_STATUS_REPORT = "sale_status_report";
	/// <summary>Value text param order condition report list total</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_TOTAL = "total";
	/// <summary>Value text param order condition report time from</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_REPORT_TIME_FROM = "time_from";
	/// <summary>Value text param order condition report time to</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_REPORT_TIME_TO = "time_to";
	/// <summary>Value text param order condition report default start time</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_REPORT_DEFAULT_START_TIME = "00:00:00.000";
	/// <summary>Value text param order condition report default end time</summary>
	public const string VALUETEXT_PARAM_ORDER_CONDITION_REPORT_DEFAULT_END_TIME = "23:59:59.998";

	/// <summary>Value text param product sale ranking report</summary>
	public const string VALUETEXT_PARAM_PRODUCT_SALE_RANKING_REPORT = "product_sale_ranking_report";
	/// <summary>Value text param product sale ranking report title</summary>
	public const string VALUETEXT_PARAM_PRODUCT_SALE_RANKING_REPORT_TITLE = "product_sale_ranking_report_title";
	/// <summary>Value text param product sale ranking report during the period</summary>
	public const string VALUETEXT_PARAM_PRODUCT_SALE_RANKING_REPORT_DURING_THE_PERIOD = "during_the_period";
	/// <summary>Value text param product sale ranking report during the period number sale</summary>
	public const string VALUETEXT_PARAM_PRODUCT_SALE_RANKING_REPORT_DURING_THE_PERIOD_NUMBER_SALE = "number_sale";
	/// <summary>Value text param product sale ranking report during the period total sale</summary>
	public const string VALUETEXT_PARAM_PRODUCT_SALE_RANKING_REPORT_DURING_THE_PERIOD_TOTAL_SALE = "total_sale";
	/// <summary>Value text param product sale ranking report during the period digestion rate</summary>
	public const string VALUETEXT_PARAM_PRODUCT_SALE_RANKING_REPORT_DURING_THE_PERIOD_DIGESTION_SALE = "digestion_rate";
	/// <summary>Value text param product sale ranking report total kbn</summary>
	public const string VALUETEXT_PARAM_PRODUCT_SALE_RANKING_REPORT_TOTAL_KBN = "total_kbn";
	/// <summary>Value text param product sale ranking report total kbn input</summary>
	public const string VALUETEXT_PARAM_PRODUCT_SALE_RANKING_REPORT_TOTAL_KBN_INPUT = "input";
	/// <summary>Value text param product sale ranking report total kbn sale</summary>
	public const string VALUETEXT_PARAM_PRODUCT_SALE_RANKING_REPORT_TOTAL_KBN_SALE = "sale";
	/// <summary>Value text param product sale ranking report total kbn digestibility</summary>
	public const string VALUETEXT_PARAM_PRODUCT_SALE_RANKING_REPORT_TOTAL_KBN_DIGESTIBILITY = "digestibility";
	/// <summary>Value text param product sale ranking reportaverage selling price</summary>
	public const string VALUETEXT_PARAM_PRODUCT_SALE_RANKING_REPORT_AVERAGE_SELLING_PRICE = "average_selling_price";
	/// <summary>Value text param product sale ranking report out of stock</summary>
	public const string VALUETEXT_PARAM_PRODUCT_SALE_RANKING_REPORT_OUT_OF_STOCK = "out_of_stock";

	/// <summary>Value text param order kbn report list</summary>
	public const string VALUETEXT_PARAM_ORDER_KBN_REPORT_LIST = "order_kbn_report_list";
	/// <summary>Value text param order kbn report list title</summary>
	public const string VALUETEXT_PARAM_ORDER_KBN_REPORT_LIST_TITLE = "order_kbn_report_list_title";
	/// <summary>Value text param order kbn report list purchasing category report</summary>
	public const string VALUETEXT_PARAM_ORDER_KBN_REPORT_LIST_PURCHASING_CATEGORY_REPORT = "purchasing_category_report";
	/// <summary>Value text param order kbn report list item</summary>
	public const string VALUETEXT_PARAM_ORDER_KBN_REPORT_LIST_ITEM = "item";
	/// <summary>Value text param order kbn report list composition ratio</summary>
	public const string VALUETEXT_PARAM_ORDER_KBN_REPORT_LIST_COMPOSITION_RATIO = "composition_ratio";
	/// <summary>Value text param order kbn report list number kbn</summary>
	public const string VALUETEXT_PARAM_ORDER_KBN_REPORT_LIST_NUMBER_KBN = "number_kbn";
	/// <summary>Value text param order kbn report list number kbn people</summary>
	public const string VALUETEXT_PARAM_ORDER_KBN_REPORT_LIST_NUMBER_KBN_PEOPLE = "people";
	/// <summary>Value text param order kbn report list number kbn result</summary>
	public const string VALUETEXT_PARAM_ORDER_KBN_REPORT_LIST_NUMBER_KBN_RESULT = "result";

	/// <summary>Value text param order repeat report title</summary>
	public const string VALUETEXT_PARAM_ORDER_REPEAT_REPORT_TITLE = "order_repeat_report_title";
	/// <summary>Value text param order repeat report report regular frequency</summary>
	public const string VALUETEXT_PARAM_ORDER_REPEAT_REPORT_REPORT_REGULAR_FREQUENCY = "report_regular_frequency";

	/// <summary>Value text param point rule campaign register</summary>
	public const string VALUETEXT_PARAM_POINT_RULE_CAMPAIGN_REGISTER = "point_rule_campaign_register";
	/// <summary>Value text param point rule campaign register time kbn</summary>
	public const string VALUETEXT_PARAM_POINT_RULE_CAMPAIGN_REGISTER_TIME_KBN = "time_kbn";
	/// <summary>Value text param point rule campaign register time kbn day</summary>
	public const string VALUETEXT_PARAM_POINT_RULE_CAMPAIGN_REGISTER_TIME_KBN_DAY = "day";

	/// <summary>Value text param product ranking list</summary>
	public const string VALUETEXT_PARAM_PRODUCT_RANKING_LIST = "product_ranking_list";
	/// <summary>Value text param product ranking list title</summary>
	public const string VALUETEXT_PARAM_PRODUCT_RANKING_LIST_TITLE = "product_ranking_list_title";
	/// <summary>Value text param product ranking list product ranking</summary>
	public const string VALUETEXT_PARAM_PRODUCT_RANKING_LIST_PRODUCT_RANKING = "product_ranking";
	/// <summary>Value text param product ranking list target</summary>
	public const string VALUETEXT_PARAM_PRODUCT_RANKING_LIST_TARGET = "target";
	/// <summary>Value text param product ranking list rank</summary>
	public const string VALUETEXT_PARAM_PRODUCT_RANKING_LIST_RANK = "rank";
	/// <summary>Value text param product ranking list composition raito</summary>
	public const string VALUETEXT_PARAM_PRODUCT_RANKING_LIST_COMPOSITION_RATIO = "composition_ratio";

	/// <summary>Value text param user condition report detail</summary>
	public const string VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL = "user_condition_report_detail";
	/// <summary>Value text param user condition report detail title</summary>
	public const string VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_TITLE = "user_condition_report_detail_title";
	/// <summary>Value text param user condition report detail number rate increase</summary>
	public const string VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_NUMBER_RATE_INCREASE = "number_rate_increase";
	/// <summary>Value text param user condition report detail new acquisition</summary>
	public const string VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_NEW_ACQUISITION = "new_acquisition";
	/// <summary>Value text param user condition report detail total number</summary>
	public const string VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_TOTAL_NUMBER = "total_number";
	/// <summary>Value text param user condition report detail number people</summary>
	public const string VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_NUMBER_PEOPLE = "number_people";
	/// <summary>Value text param user condition report detail rate change</summary>
	public const string VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_RATE_CHANGE = "rate_change";
	/// <summary>Value text param user condition report detail user</summary>
	public const string VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_USER = "user";
	/// <summary>Value text param user condition report detail user status report</summary>
	public const string VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_USER_STATUS_REPORT = "user_status_report";
	/// <summary>Value text param user condition report detail user kbn</summary>
	public const string VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_USER_KBN = "user_kbn";
	/// <summary>Value text param user condition report detail user stauts</summary>
	public const string VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_USER_STATUS = "user_status";
	/// <summary>Value text param user condition report detail potential user</summary>
	public const string VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_POTENTIAL_USER = "potential_user";
	/// <summary>Value text param user condition report detail active user</summary>
	public const string VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_ACTIVE_USER = "active_user";
	/// <summary>Value text param user condition report detail vacation user</summary>
	public const string VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_VACATION_USER = "vacation_user";
	/// <summary>Value text param user condition report detail customer</summary>
	public const string VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_CUSTOMER = "customer";
	/// <summary>Value text param user condition report detail cognitive customer</summary>
	public const string VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_COGNITIVE_CUSTOMER = "cognitive_customer";
	/// <summary>Value text param user condition report detail active customer</summary>
	public const string VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_ACTIVE_CUSTOMER = "active_customer";
	/// <summary>Value text param user condition report detail dormant customer</summary>
	public const string VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_DORMANT_CUSTOMER = "dormant_customer";
	/// <summary>Value text param user condition report detail withdrawal customer</summary>
	public const string VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_WITHDRAWAL_CUSTOMER = "withdrawal_customer";

	/// <summary>Value text param user condition report list</summary>
	public const string VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST = "user_condition_report_list";
	/// <summary>Value text param user condition report list title</summary>
	public const string VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_TITLE = "user_condition_report_list_title";
	/// <summary>Value text param user condition report list</summary>
	public const string VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_MAN = "man";
	/// <summary>Value text param user condition report list increase/decrease</summary>
	public const string VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_INCREASE_DECREASE = "increase_decrease";
	/// <summary>Value text param user condition report detail number of new acquisition</summary>
	public const string VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_NUMBER_NEW_ACQUISITION = "number_new_acquisition";
	/// <summary>Value text param user condition report detail total number</summary>
	public const string VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_TOTAL_NUMBER = "total_number";
	/// <summary>Value text param user condition report list number of withdrawal this month</summary>
	public const string VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_NUMBER_WITHDRAWAL_THIS_MONTH = "number_withdrawal_this_month";
	/// <summary>Value text param user condition report list user</summary>
	public const string VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_USER = "user";
	/// <summary>Value text param user condition report list user status report</summary>
	public const string VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_USER_STATUS_REPORT = "user_status_report";
	/// <summary>Value text param user condition report list user kbn</summary>
	public const string VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_USER_KBN = "user_kbn";
	/// <summary>Value text param user condition report list user stauts</summary>
	public const string VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_USER_STATUS = "user_status";
	/// <summary>Value text param user condition report list potential user</summary>
	public const string VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_POTENTIAL_USER = "potential_user";
	/// <summary>Value text param user condition report list active user</summary>
	public const string VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_ACTIVE_USER = "active_user";
	/// <summary>Value text param user condition report list dormant user</summary>
	public const string VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_DORMANT_USER = "dormant_user";
	/// <summary>Value text param user condition report list vacation user 1</summary>
	public const string VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_VACATION_USER1 = "vacation_user1";
	/// <summary>Value text param user condition report list vacation user 2</summary>
	public const string VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_VACATION_USER2 = "vacation_user2";
	/// <summary>Value text param user condition report list customer</summary>
	public const string VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_CUSTOMER = "customer";
	/// <summary>Value text param user condition report list number of withdrawal</summary>
	public const string VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_NUMBER_WITHDRAWAL = "number_withdrawal";
	/// <summary>Value text param user condition report list cognitive customer</summary>
	public const string VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_COGNITIVE_CUSTOMER = "cognitive_customer";
	/// <summary>Value text param user condition report list withdrawal customer</summary>
	public const string VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_WITHDRAWAL_CUSTOMER = "withdrawal_customer";

	/// <summary>Value text param user extend report list</summary>
	public const string VALUETEXT_PARAM_USER_EXTEND_REPORT_LIST = "user_extend_report_list";
	/// <summary>Value text param user extend report list title</summary>
	public const string VALUETEXT_PARAM_USER_EXTEND_REPORT_LIST_TITLE = "user_extend_report_list_title";
	/// <summary>Value text param user extend report list user kbn report</summary>
	public const string VALUETEXT_PARAM_USER_EXTEND_REPORT_LIST_USER_KBN_REPORT = "user_kbn_report";
	/// <summary>Value text param user extend report list item</summary>
	public const string VALUETEXT_PARAM_USER_EXTEND_REPORT_LIST_ITEM = "item";
	/// <summary>Value text param user extend report list composition ratio</summary>
	public const string VALUETEXT_PARAM_USER_EXTEND_REPORT_LIST_COMPOSITION_RATIO = "composition_ratio";
	/// <summary>Value text param user extend report list number of people</summary>
	public const string VALUETEXT_PARAM_USER_EXTEND_REPORT_LIST_NUMBER_PEOPLE = "number_people";

	/// <summary>Value text param user kbn report list</summary>
	public const string VALUETEXT_PARAM_USER_KBN_REPORT_LIST = "user_kbn_report_list";
	/// <summary>Value text param user kbn report list title</summary>
	public const string VALUETEXT_PARAM_USER_KBN_REPORT_LIST_TITLE = "user_kbn_report_list_title";
	/// <summary>Value text param user kbn report list user kbn report</summary>
	public const string VALUETEXT_PARAM_USER_KBN_REPORT_LIST_USER_KBN_REPORT = "user_kbn_report";
	/// <summary>Value text param user kbn report list item</summary>
	public const string VALUETEXT_PARAM_USER_KBN_REPORT_LIST_ITEM = "item";
	/// <summary>Value text param user kbn report list composition ratio</summary>
	public const string VALUETEXT_PARAM_USER_KBN_REPORT_LIST_COMPOSITION_RATIO = "composition_ratio";
	/// <summary>Value text param user kbn report list number of people</summary>
	public const string VALUETEXT_PARAM_USER_KBN_REPORT_LIST_NUMBER_PEOPLE = "number_people";

	/// <summary>Value text param fixed purchase kbn report list</summary>
	public const string VALUETEXT_PARAM_FIXED_PURCHASE_KBN_REPORT_LIST = "fixed_purchase_kbn_report_list";
	/// <summary>Value text param fixed purchase kbn report list title</summary>
	public const string VALUETEXT_PARAM_FIXED_PURCHASE_KBN_REPORT_LIST_TITLE = "fixed_purchase_kbn_report_list_title";
	/// <summary>Value text param fixed purchase kbn report list user kbn report</summary>
	public const string VALUETEXT_PARAM_FIXED_PURCHASE_KBN_REPORT_LIST_FIXED_PURCHASE_KBN_REPORT = "fixed_purchase_kbn_report";
	/// <summary>Value text param fixed purchase kbn report list item</summary>
	public const string VALUETEXT_PARAM_FIXED_PURCHASE_KBN_REPORT_LIST_ITEM = "item";
	/// <summary>Value text param fixed purchase kbn report list composition ratio</summary>
	public const string VALUETEXT_PARAM_FIXED_PURCHASE_KBN_REPORT_LIST_COMPOSITION_RATIO = "composition_ratio";
	/// <summary>Value text param fixed purchase kbn report list number</summary>
	public const string VALUETEXT_PARAM_FIXED_PURCHASE_KBN_REPORT_LIST_NUMBER = "number";
	/// <summary>Value text param product ranking report access kbn</summary>
	public const string VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_ACCESS_KBN = "access_kbn";
	/// <summary>Value text param product ranking report access kbn mobile</summary>
	public const string VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_ACCESS_KBN_MOBILE = "access_kbn_mobile";
	/// <summary>Value text param product ranking report access kbn smartphone</summary>
	public const string VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_ACCESS_KBN_SMARTPHONE = "access_kbn_smartphone";
	/// <summary>Value text param product ranking report access kbn all</summary>
	public const string VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_ACCESS_KBN_ALL = "access_kbn_all";
	/// <summary>Value text param product ranking report ranking info</summary>
	public const string VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_RANKING_INFO = "ranking_info";
	/// <summary>Value text param product ranking report product search word</summary>
	public const string VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_PRODUTC_SEARCH_WORD_RANKING = "product_search_word_ranking";
	/// <summary>Value text param product ranking report search word</summary>
	public const string VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_SEARCH_WORD = "search_word";
	/// <summary>Value text param product ranking report target</summary>
	public const string VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_TARGET = "target";
	/// <summary>Value text param product ranking report product sales quantity</summary>
	public const string VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_PRODUCT_SALES_QUANTITY = "product_sales_quantity_ranking";
	/// <summary>Value text param product ranking report commodity id</summary>
	public const string VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_COMMODITY_ID = "commodity_id";
	/// <summary>Value text param product ranking report product sales amount</summary>
	public const string VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_PRODUCT_AMOUNT_QUANTITY = "product_sales_amount_ranking";
	/// <summary>Value text param product ranking report tax included</summary>
	public const string VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_TAX_INCLUDED = "tax_included";
	/// <summary>Value text param product ranking report tax excluded</summary>
	public const string VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_TAX_EXCLUDED = "tax_excluded";
	/// <summary>Value text param product ranking report report info</summary>
	public const string VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_REPORT_INFO = "report_info";
	/// <summary>Value text param product ranking report date</summary>
	public const string VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_DATE = "date";
	/// <summary>Value text param product ranking report number search</summary>
	public const string VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_NUMBER_SEARCH = "number_search";
	/// <summary>Value text param product ranking report number units</summary>
	public const string VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_NUMBER_UNITS = "number_units";
	/// <summary>Value text param product ranking report number sales</summary>
	public const string VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_NUMBER_SALES = "number_sales";
	/// <summary>Value text param product ranking report times</summary>
	public const string VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_TIMES = "times";
	/// <summary>Value text param product ranking report result</summary>
	public const string VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_RESULT = "result";
	/// <summary>Value text param target list register</summary>
	public const string VALUETEXT_PARAM_TARGET_LIST_REGISTER = "target_list_register";
	/// <summary>Value text param target name</summary>
	public const string VALUETEXT_PARAM_TARGET_NAME = "target_name";
	/// <summary>Value text param template</summary>
	public const string VALUETEXT_PARAM_TEAMPLATE = "template";
	/// <summary>定期売上予測レポート</summary>
	public const string VALUETEXT_PARAM_FIXED_PURCHASE_FORECAST_REPORT = "fixed_purchase_forecast_report";
	/// <summary>定期売上予測レポート 売上予測算出基準</summary>
	public const string VALUETEXT_PARAM_FIXED_PURCHASE_FORECAST_REPORT_SALES_FORECAST_CALCULATION_CRITERIA = "sales_forecast_calculation_criteria";
	/// <summary>定期売上予測レポート 売上予測算出基準 配送日基準</summary>
	public const string VALUETEXT_PARAM_FIXED_PURCHASE_FORECAST_REPORT_SALES_FORECAST_CALCULATION_CRITERIA_SHIPPED_DATE = "shipped_date";
	/// <summary>定期売上予測レポート 売上予測算出基準 出荷予定日基準</summary>
	public const string VALUETEXT_PARAM_FIXED_PURCHASE_FORECAST_REPORT_SALES_FORECAST_CALCULATION_CRITERIA_SCHEDULED_SHIPPING_DATE = "scheduled_shipping_date";
	/// <summary>定期売上予測レポート 表示区分</summary>
	public const string VALUETEXT_PARAM_FIXED_PURCHASE_FORECAST_REPORT_DISPLAY_KBN = "display_kbn";
	/// <summary>定期売上予測レポート 表示区分</summary>
	public const string VALUETEXT_PARAM_FIXED_PURCHASE_FORECAST_REPORT_DISPLAY_KBN_PRODUCT = "product";
	/// <summary>定期売上予測レポート 表示区分</summary>
	public const string VALUETEXT_PARAM_FIXED_PURCHASE_FORECAST_REPORT_DISPLAY_KBN_PRODUCT_VARIATION = "productVariation";
	/// <summary>定期売上予測レポート 表示区分</summary>
	public const string VALUETEXT_PARAM_FIXED_PURCHASE_FORECAST_REPORT_DISPLAY_KBN_MONTHLY = "monthly";

	/// <summary> 定期購入ステータス(頒布会を除く)</summary>
	public const string VALUETEXT_PARAM_FIXED_PURCHASE_STATUS_INVALID_SUBSCRIPTION_BOX = "fixed_purchase_status_invalid_subscription_box";

	// レコメンドレポート
	/// <summary>レコメンドレポート レポート対象選択</summary>
	public const string PAGE_W2MP_MANAGER_RECOMMEND_TABLE_REPORT = "Form/RecommendReport/RecommendReportTableList.aspx";
	/// <summary>レコメンドレポート 詳細レポート対象選択</summary>
	public const string PAGE_W2MP_MANAGER_RECOMMEND_REPORT = "Form/RecommendReport/RecommendReportList.aspx";

	/// <summary>レコメンドレポート レコメンドID</summary>
	public const string REQUEST_KEY_RECOMMEND_ID = "rcdid";
	/// <summary>レコメンドレポート レコメンド名(管理用)</summary>
	public const string REQUEST_KEY_RECOMMEND_NAME = "rcdn";
	/// <summary>レコメンドレポート レコメンド区分</summary>
	public const string REQUEST_KEY_RECOMMEND_KBN = "rckbn";
	/// <summary>レコメンドレポート 開催状態</summary>
	public const string REQUEST_KEY_RECOMMEND_STATUS = "status";
	/// <summary>レコメンドレポート 有効フラグ</summary>
	public const string REQUEST_KEY_RECOMMEND_VALID_FLG = "valid_flg";
	/// <summary>レコメンドレポート レコメンド検索情報</summary>
	public const string SESSIONPARAM_KEY_RECOMMEND_SEARCH_INFO = "recommend_search_info";

	/// <summary>レコメンドレポート 詳細レポート対象選択 タイトル</summary>
	public const string VALUETEXT_PARAM_RECOMMEND_REPORT = "recommend_report";
	/// <summary>レコメンドレポート 詳細レポート対象選択</summary>
	public const string VALUETEXT_PARAM_RECOMMEND_REPORT_LIST = "recommend_report_list";
	/// <summary>レコメンドレポート 詳細レポート対象選択 レコメンド詳細レポート</summary>
	public const string VALUETEXT_PARAM_RECOMMEND_REPORT_LIST_TITLE = "report_title";
	/// <summary>レコメンドレポート 詳細レポート対象選択 日付</summary>
	public const string VALUETEXT_PARAM_RECOMMEND_REPORT_LIST_DATE = "date";
	/// <summary>レコメンドレポート 詳細レポート対象選択 PV数</summary>
	public const string VALUETEXT_PARAM_RECOMMEND_REPORT_LIST_NUMBER_PAGE_VIEW = "number_page_view";
	/// <summary>レコメンドレポート 詳細レポート対象選択 CV数</summary>
	public const string VALUETEXT_PARAM_RECOMMEND_REPORT_LIST_NUMBER_CONVERSION = "number_conversion";
	/// <summary>レコメンドレポート 詳細レポート対象選択 CV率</summary>
	public const string VALUETEXT_PARAM_RECOMMEND_REPORT_LIST_CONVERSION_RATE = "conversion_rate";
	/// <summary>レコメンドレポート 詳細レポート対象選択 回</summary>
	public const string VALUETEXT_PARAM_RECOMMEND_REPORT_LIST_TIME = "time";
	/// <summary>レコメンドレポート 詳細レポート対象選択 %</summary>
	public const string VALUETEXT_PARAM_RECOMMEND_REPORT_LIST_PERCENT = "percent";
	/// <summary>レコメンドレポート 詳細レポート対象選択 期間指定</summary>
	public const string VALUETEXT_PARAM_RECOMMEND_REPORT_LIST_PERIOD = "period";

	/// <summary>レコメンドレポート レポート対象選択</summary>
	public const string VALUETEXT_PARAM_RECOMMEND_REPORT_TABLE_LIST = "recommend_report_table_list";
	/// <summary>レコメンドレポート レポート対象選択 レコメンドレポート</summary>
	public const string VALUETEXT_PARAM_RECOMMEND_REPORT_TABLE_LIST_TITLE = "report_title";
	/// <summary>レコメンドレポート レポート対象選択 合計</summary>
	public const string VALUETEXT_PARAM_RECOMMEND_REPORT_TABLE_LIST_TOTAL = "total";
	/// <summary>レコメンドレポート レポート対象選択 レコメンドID</summary>
	public const string VALUETEXT_PARAM_RECOMMEND_REPORT_TABLE_LIST_RECOMMEND_ID = "recommend_id";
	/// <summary>レコメンドレポート レポート対象選択 レコメンド名(管理用)</summary>
	public const string VALUETEXT_PARAM_RECOMMEND_REPORT_TABLE_LIST_RECOMMEND_NAME = "recommend_name";
	/// <summary>レコメンドレポート レポート対象選択 表示ページ</summary>
	public const string VALUETEXT_PARAM_RECOMMEND_REPORT_TABLE_LIST_RECOMMEND_DISPLAYNAME = "recommend_displayname";
	/// <summary>レコメンドレポート レポート対象選択 レコメンド区分</summary>
	public const string VALUETEXT_PARAM_RECOMMEND_REPORT_TABLE_LIST_RECOMMEND_KBN = "recommend_kbn";
	/// <summary>レコメンドレポート レポート対象選択 PV数</summary>
	public const string VALUETEXT_PARAM_RECOMMEND_REPORT_TABLE_LIST_NUMBER_PAGE_VIEW = "number_page_view";
	/// <summary>レコメンドレポート レポート対象選択 CV数</summary>
	public const string VALUETEXT_PARAM_RECOMMEND_REPORT_TABLE_LIST_NUMBER_CONVERSION = "number_conversion";
	/// <summary>レコメンドレポート レポート対象選択 CV率(%)</summary>
	public const string VALUETEXT_PARAM_RECOMMEND_REPORT_TABLE_LIST_CONVERSION_RATE = "conversion_rate";
	/// <summary>レコメンドレポート レポート対象選択 開催状態</summary>
	public const string VALUETEXT_PARAM_RECOMMEND_REPORT_TABLE_LIST_RECOMMEND_STATUS = "recommend_status";
	/// <summary>レコメンドレポート レポート対象選択 有効フラグ</summary>
	public const string VALUETEXT_PARAM_RECOMMEND_REPORT_TABLE_LIST_RECOMMEND_VALIDFLG = "recommend_validflg";
	/// <summary>レコメンドレポート レポート対象選択 期間指定</summary>
	public const string VALUETEXT_PARAM_RECOMMEND_REPORT_TABLE_LIST_PERIOD = "period";
	/// <summary>レコメンドレポート レポート対象選択 平均</summary>
	public const string VALUETEXT_PARAM_RECOMMEND_REPORT_TABLE_LIST_AVERAGE = "average";

	/// <summary>レコメンドレポート レポート対象選択 並び順 開催状態順
	public const string KBN_REPORT_SORT_RECOMMEND_LIST_STATUS = "0";
	/// <summary>レコメンドレポート レポート対象選択 並び順 レコメンド名(管理用)/昇順
	public const string KBN_REPORT_SORT_RECOMMEND_LIST_RECOMMEND_NAME_ASC = "1";
	/// <summary>レコメンドレポート レポート対象選択 並び順 レコメンド名(管理用)/降順
	public const string KBN_REPORT_SORT_RECOMMEND_LIST_RECOMMEND_NAME_DESC = "2";
	/// <summary>レコメンドレポート レポート対象選択 並び順 PV数/昇順
	public const string KBN_REPORT_SORT_RECOMMEND_LIST_PAGE_VIEW_ASC = "3";
	/// <summary>レコメンドレポート レポート対象選択 並び順 PV数/降順
	public const string KBN_REPORT_SORT_RECOMMEND_LIST_PAGE_VIEW_DESC = "4";
	/// <summary>レコメンドレポート レポート対象選択 並び順 CV数/昇順
	public const string KBN_REPORT_SORT_RECOMMEND_LIST_CONVERSION_ASC = "5";
	/// <summary>レコメンドレポート レポート対象選択 並び順 CV数/降順
	public const string KBN_REPORT_SORT_RECOMMEND_LIST_CONVERSION_DESC = "6";
	/// <summary>レコメンドレポート レポート対象選択 並び順 CV率(%)/昇順
	public const string KBN_REPORT_SORT_RECOMMEND_LIST_CONVERSION_RATE_ASC = "7";
	/// <summary>レコメンドレポート レポート対象選択 並び順 CV率(%)/降順
	public const string KBN_REPORT_SORT_RECOMMEND_LIST_CONVERSION_RATE_DESC = "8";

	/// <summary>Request param: order month</summary>
	public const string REQUEST_KEY_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT_ORDER_MONTH = "om";
	/// <summary>Request param: page number</summary>
	public const string REQUEST_KEY_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT_PAGE_NUMBER = "page_number";
	/// <summary>Request param: title</summary>
	public const string REQUEST_KEY_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT_TITLE = "tt";
	/// <summary>Request param: fixed purchase order year</summary>
	public const string REQUEST_KEY_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT_FIXEDPURCHASE_ORDER_YEAR = "fpy";
	/// <summary>Request param: fixed purchase order month</summary>
	public const string REQUEST_KEY_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT_FIXEDPURCHASE_ORDER_MONTH = "fpm";

	/// <summary>Search param: date to</summary>
	public const string SEARCH_PARAM_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT_DATE_TO = "date_to";
	/// <summary>Search param: date from</summary>
	public const string SEARCH_PARAM_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT_DATE_FROM = "date_from";
	/// <summary>Search param: user extend flag</summary>
	public const string SEARCH_PARAM_USER_EXTEND_FLG = "user_extend_flg";
	/// <summary>Search param: user extend name</summary>
	public const string SEARCH_PARAM_USER_EXTEND_NAME = "user_extend_name";
	/// <summary>Search param: user extend like escaped</summary>
	public const string SEARCH_PARAM_USER_EXTEND_LIKE_ESCAPED = "user_extend_like_escaped";
	/// <summary>Search param: user extend type</summary>
	public const string SEARCH_PARAM_USER_EXTEND_TYPE = "user_extend_type";
}
