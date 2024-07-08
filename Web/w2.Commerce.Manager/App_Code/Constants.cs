/*
=========================================================================================================
  Module      : マネージャ定数定義(Constants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
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
	/// <summary>マスタファイル取込実行ＥＸＥ</summary>
	public static string PHYSICALDIRPATH_MASTERUPLOAD_EXE = "";
	/// <summary>外部ファイルアップロードディレクトリ</summary>
	public static string PHYSICALDIRPATH_EXTERNAL_DIR = "";
	/// <summary>注文ＰＤＦクリエータ実行ＥＸＥ</summary>
	public static string PHYSICALDIRPATH_ORDER_PDF_CREATER_EXE = "";
	/// <summary>サービスキック用</summary>
	public static string PHYSICALDIRPATH_KICKUPDATE_SERVICE_EXE = "";
	/// <summary>表示商品作成バッチ実行ＥＸＥ</summary>
	public static string PHYSICALDIRPATH_CREATEDISPPRODUCT_EXE = "";
	/// <summary>注文関連ファイル取込バッチ実行ＥＸＥ</summary>
	public static string PHYSICALDIRPATH_ORDERFILEIMPORT_EXE = "";
	/// <summary>注文関連ファイル取込バッチ実行ＥＸＥ</summary>
	public static string DIRECTORY_IMPORTORDERFILE_UPLOAD_PATH = "";
	/// <summary>FLAPS連携バッチ実行EXEファイルパス</summary>
	public static string PHYSICALDIRPATH_FLAPS_INTEGRATION_EXE = "";

	/// <summary>注文一覧デフォルト注文区分</summary>
	public static string ORDERLIST_FIRSTVIEW_ORDERSTATUS = null;
	/// <summary>注文一覧デフォルト期間</summary>
	public static string ORDERLIST_FIRSTVIEW_ABSTIMESPAN = null;

	/// <summary>注文登録時デフォルト注文区分</summary>
	public static string ORDER_DEFALUT_ORDER_KBN = Constants.FLG_ORDER_ORDER_KBN_TEL;
	/// <summary>注文登録時デフォルト注文者区分</summary>
	public static string ORDER_DEFALUT_OWNER_KBN = Constants.FLG_ORDEROWNER_OWNER_KBN_OFFLINE_USER;
	/// <summary>新規注文登録会員ランクメモ表示</summary>
	public static bool ORDER_MEMBERRANK_MEMO_DISPLAY = false;

	/// <summary>仮クレジットカード表示用アンカー名</summary>
	public const string ANCHOR_NAME_FOR_DISP_PROVISIONAL_CREDITCARD = @"aNameForDispProvisionalCreditCardOrder";

	/// <summary>
	/// 注文情報登録完了後遷移先画面
	/// </summary>
	public static OrderRegisterCompletePageType? ORDER_REGISTER_COMPLETE_PAGE = null;
	/// <summary>
	/// 注文情報登録完了後遷移先画面種別
	/// </summary>
	public enum OrderRegisterCompletePageType
	{
		/// <summary>受注情報詳細</summary>
		OrderDetail,
		/// <summary>受注情報一覧</summary>
		OrderList
	}

	/// <summary>カード実売上連動設定</summary>
	public static bool PAYMENT_CARD_REALSALES_ENABLED = false;
	/// <summary>カードキャンセル連動設定</summary>
	public static bool PAYMENT_CARD_CANCEL_ENABLED = false;
	/// <summary>AmazonPay実売上連動設定</summary>
	public static bool PAYMENT_AMAZONPAY_REALSALES_ENABLED = false;
	/// <summary>AmazonPayキャンセル連動設定</summary>
	public static bool PAYMENT_AMAZONPAY_CANCEL_ENABLED = false;
	/// <summary>マスタアップロードのユーザー移行利用有無</summary>
	public static bool MASTERUPLOAD_USER_ENABLED = false;
	// ワークフロー設定画面のテキストボックスを表示/非表示
	public static bool ORDERWORKFLOWSETTING_MEMO_TEXTBOX_ENABLED = false;
	// 検索条件/ワークフロー設定に「配送先：都道府県」を表示/非表示
	public static bool SEARCHCONDITION_SHIPPINGADDR1_ENABLED = false;

	/// <summary>定期注文登録時にユーザーに注文メール＆定期購入エラーメール送信するか</summary>
	public static bool SEND_FIXEDPURCHASE_MAIL_TO_USER = false;

	// Cssルート
	public static string PHYSICALDIRPATH_CSS_ROOT = "";

	/// <summary>商品タグ設定：タグ設定を追加できる最大件数</summary>
	public static int PRODUCTTAGSETTING_MAXCOUNT = 0;

	/// <summary>ユーザ拡張項目を追加できる最大件数</summary>
	public static int USEREXTENDSETTING_MAXCOUNT = 0;

	/// <summary>受注ワークフローを追加できる最大件数</summary>
	public static int? ORDERWORKFLOWSETTING_MAXCOUNT = 0;

	/// <summary>システムで利用しているユーザー拡張項目</summary>
	public static string[] USEREXTENDSETTING_SYSTEM_USED_ITEMS = new string[]
	{
		SOCIAL_PROVIDER_ID_FACEBOOK,
		SOCIAL_PROVIDER_ID_LINE,
		SOCIAL_PROVIDER_ID_TWITTER,
		SOCIAL_PROVIDER_ID_YAHOO,
		SOCIAL_PROVIDER_ID_GOOGLE,
		SOCIAL_PROVIDER_ID_APPLE,
		AMAZON_USER_ID_USEREXTEND_COLUMN_NAME,
		FLG_USEREXTEND_USREX_ATONE_TOKEN_ID,
		FLG_USEREXTEND_USREX_AFTEE_TOKEN_ID,
		LINEPAY_USEREXRTEND_COLUMNNAME_REGKEY,
	};

	/// <summary>システムで利用しているユーザー拡張項目(一部編集制御)</summary>
	public static string[] USEREXTENDSETTING_SYSTEM_USED_NOT_DELETE_ITEMS = new string[]
	{
		FAVORITE_PRODUCT_DECREASE_MAILSENDFLG_USEREXRTEND_COLUMNNAME,
	};

	/// <summary> The number of item history first display</summary>
	public static int ITEMS_HISTORY_FIRST_DISPLAY = 0;

	// w2.App.Common.Constantsへ移動
	// カード決済区分(SBPS、ゼウス、、、)
	//public static string PAYMENT_CARD_KBN = "";
	// カードオーソリ後入金ステータスを「入金済」にするか
	//public static bool PAYMENT_CARD_PATMENT_STAUS_COMPLETE = true;
	// w2mp識別ID
	//public static string W2MP_DEPT_ID = "";
	// w2mpポイントオプション利用有無
	//public static bool W2MP_POINT_OPTION_ENABLED = false;
	// 定期購入有無
	//public static bool FIXEDPURCHASE_OPTION_ENABLED						= false;

	//*****************************************************************************************
	// パッケージ固有設定
	//*****************************************************************************************
	// アプリケーション表示名
	public const string APPLICATION_NAME_DISP = "W2Unified";

	/// <summary>マネージャサイトタイプ</summary>
	public static readonly MenuAuthorityHelper.ManagerSiteType ManagerSiteType = MenuAuthorityHelper.ManagerSiteType.Ec;
	/// <summary>メニュー権限フィールド名（店舗オペレータマスタの利用フィールド名）</summary>
	public static string FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL_PKG = MenuAuthorityHelper.GetOperatorMenuAccessLevelFieldName(ManagerSiteType);

	//*****************************************************************************************
	// 定数値・マネージャサイド
	//*****************************************************************************************
	public static int CONST_DISP_CONTENTS_PAYMENT_LIST = 30;		// 決済種別の一覧表示件数
	public static int CONST_DISP_BUNDLE_CONTENTS_OVER_HIT_LIST = 5000;		// 同梱情報を一覧に表示する上限数
	public static int CONST_COUNT_PDF_DIRECTDOUNLOAD = 200;		// PDFを直にダウンロードできる件数

	// 注文同梱
	public static int CONST_DISP_LIST_CONTENTS_COUNT_ORDERCOMBINE = 10; // 注文同梱で1ページに表示する親注文の件数
	public static int CONST_DISP_LIST_CONTENTS_COUNT_FIXEDPURCHASECOMBINE = 10; // 管理画面の定期購入同梱で1ページに表示する親定期購入の件数
	public static int CONST_DISP_FIXEDPURCHASECOMBINE_INITIAL_NEXT_SHIPPING_DATE_INTERVAL = 7; // 管理画面の定期購入同梱で初期表示で設定する次回配送日間隔

	// オプション訴求
	/// <summary> オプション訴求機能オプション </summary>
	public static bool OPTIONAPPEAL_ENABLED = true;
	/// <summary>オプション訴求プラン</summary>
	public static string OPTIONAPPEAL_PROJECT_PLAN = "";
	/// <summary>オプション訴求機能：案件利用済みオプションID※カンマ区切りでconfigで利用有無の判定していない利用済みのオプションIDを設定する</summary>
	public static string OPTIONAPPEAL_PROJECT_USED_OPTIONS = "";
	/// <summary>オプション訴求機能：案件オプション価格表バージョン(例：v1.1)</summary>
	public static string OPTIONAPPEAL_PROJECT_OPTION_VERSION = "";
	/// <summary> オプション訴求機能：案件番号 </summary>
	public static string OPTIONAPPEAL_PROJECT_NO = "";
	/// <summary> オプション訴求機能：問い合わせメール送信元 </summary>
	public static string OPTIONAPPEAL_INQUIRY_MAIL_FROM = "";
	/// <summary> オプション訴求機能：問い合わせメール送信先 </summary>
	public static string OPTIONAPPEAL_INQUIRY_MAIL_TO = "";
	/// <summary>カテゴリーID</summary>
	public const string OPTIONAPPEAL_CATEGORY_ID = "CategoryId";
	/// <summary>カテゴリー名</summary>
	public const string OPTIONAPPEAL_CATEGORY_NAME = "Name";
	/// <summary>親カテゴリー</summary>
	public const string OPTIONAPPEAL_CATEGORY_PARENT = "ParentCategory";
	/// <summary>カテゴリーアイコンパス</summary>
	public const string OPTIONAPPEAL_CATEGORY_ICON = "IconPath";

	/// <summary>オプションID</summary>
	public const string OPTIONAPPEAL_OPTION_ID = "OptionId";
	/// <summary>オプション名</summary>
	public const string OPTIONAPPEAL_OPTION_NAME = "Name";
	/// <summary>オプション説明</summary>
	public const string OPTIONAPPEAL_OPTION_SUMMARY = "Summary";
	/// <summary>オプション詳細</summary>
	public const string OPTIONAPPEAL_OPTION_DETAILS = "Details";
	/// <summary>オプションアイコン</summary>
	public const string OPTIONAPPEAL_OPTION_ICON_PATH = "IconPath";
	/// <summary>サポートサイトURL</summary>
	public const string OPTIONAPPEAL_OPTION_SUPPORTSITE_URL = "SupportSiteURL";
	/// <summary>金額</summary>
	public const string OPTIONAPPEAL_OPTION_PRICE = "Price";
	/// <summary>プラン</summary>
	public const string OPTIONAPPEAL_OPTION_PLAN = "Plan";
	/// <summary>有効性</summary>
	public const string OPTIONAPPEAL_OPTION_AVAILABLE = "IsAvailable";
	/// <summary>初期費用</summary>
	public const string OPTIONAPPEAL_OPTION_INITIAL = "Initial";
	/// <summary>月額</summary>
	public const string OPTIONAPPEAL_OPTION_MONTHLY = "Monthly";
	/// <summary>付属詳報</summary>
	public const string OPTIONAPPEAL_OPTION_ANCILLARYINFORMATION = "AncillaryInformation";
	/// <summary>導入済み判定</summary>
	public const string OPTIONAPPEAL_OPTION_ENABLE = "option_enable";

	/// <summary>アイコンパス</summary>
	public const string OPTIONAPPEAL_SLIDER_IMAGE_PATH = "ImagePath";
	/// <summary>オプションID</summary>
	public const string OPTIONAPPEAL_SLIDER_OPTION_ID = "OptionId";
	/// <summary>表示</summary>
	public const string OPTIONAPPEAL_SLIDER_VISIBLE = "Visible";

	/// <summary>人気アイコンパス</summary>
	public const string OPTIONAPPEAL_POPULAR_SLIDER_IMAGE_PATH = "ImagePath";
	/// <summary>人気オプションID</summary>
	public const string OPTIONAPPEAL_POPULAR_SLIDER_OPTION_ID = "OptionId";
	/// <summary>表示</summary>
	public const string OPTIONAPPEAL_POPULAR_SLIDER_VISIBLE = "Visible";

	//*****************************************************************************************
	// メニューパス設定
	//*****************************************************************************************
	public static string MENU_PATH_LARGE_ORDERREGIST = "Form/OrderRegist/"; // 注文情報登録メニューパス
	public static string MENU_PATH_LARGE_PRODUCT = "Form/Product/"; // 商品情報メニューパス
	public static string MENU_PATH_LARGE_USER = "Form/User/"; // ユーザー情報メニューパス
	//*****************************************************************************************
	// 区分・マネージャサイド
	//*****************************************************************************************
	// メニュー権限機能レベル：ユーザー情報
	public const int KBN_MENU_FUNCTION_USER_DL = 1;				// ユーザマスタダウンロード
	public const int KBN_MENU_FUNCTION_USER_UPDATE = 2;			// ユーザ情報更新
	public const int KBN_MENU_FUNCTION_USER_UPDATE_POINT_ERROR_MAIL = 4;		// エラーメールポイント更新
	public const int KBN_MENU_FUNCTION_USER_SHIPPING_DL = 8;	// 配送先マスタダウンロード
	public const int KBN_MENU_FUNCTION_USER_PASSWORD_DISPLAY = 16;		// ユーザパスワード欄表示
	public const int KBN_MENU_FUNCTION_USER_TAIWAN_INVOICE_DL = 32;		// User Taiwan Invoice Display
	/// <summary>e-SCOTT削除会員ダウンロード</summary>
	public const int KBN_MENU_FUNCTION_USER_ESCOTT_DELETE_MEMBER_DL = 64;
	/// <summary>ユーザ情報初期設定編集</summary>
	public const int KBN_MENU_FUNCTION_USER_DEFAULT_SETTINGS_EDIT = 128;

	// メニュー権限機能レベル：受注情報
	public const int KBN_MENU_FUNCTION_ORDER_DL = 1;			// 受注マスタダウンロード
	public const int KBN_MENU_FUNCTION_ORDER_ITEM_DL = 2;		// 受注商品マスタダウンロード
	public const int KBN_MENU_FUNCTION_ORDER_PDF_OUTPUT_DL = 4;		// 納品書ダウンロード
	public const int KBN_MENU_FUNCTION_ORDER_PICKING_LIST_DL = 8;	// ピッキングリストダウンロード
	public const int KBN_MENU_FUNCTION_ORDER_FILE_EXPORT_DL = 16;	// 送り状発行CSVダウンロード
	public const int KBN_MENU_FUNCTION_ORDER_STATEMENT_DL = 32;		// 受注明細書ダウンロード
	public const int KBN_MENU_FUNCTION_ORDER_SETPROMOTION_DL = 64;		// 受注セットプロモーションダウンロード
	public const int KBN_MENU_FUNCTION_ORDER_RECEIPT_DL = 128;		// 領収書ダウロード

	// メニュー権限機能レベル：受注ワークフロー
	public const int KBN_MENU_FUNCTION_ORDERWF_DL = 1;			// 受注マスタダウンロード
	public const int KBN_MENU_FUNCTION_ORDERWF_ITEM_DL = 2;		// 受注商品マスタダウンロード
	public const int KBN_MENU_FUNCTION_ORDERWF_PDF_OUTPUT_DL = 4;	// 納品書ダウンロード
	public const int KBN_MENU_FUNCTION_ORDERWF_PICKING_LIST_DL = 8;	// ピッキングリストダウンロード
	public const int KBN_MENU_FUNCTION_ORDERWF_FILE_EXPORT_DL = 16;	// 送り状発行CSVダウンロード
	public const int KBN_MENU_FUNCTION_ORDERWF_STATEMENT_DL = 32;	// 受注明細書ダウンロード
	public const int KBN_MENU_FUNCTION_ORDERWF_SETPROMOTION_DL = 64;// 受注セットプロモーションダウンロード
	public const int KBN_MENU_FUNCTION_ORDERWF_RECEIPT_DL = 128;	// 領収書ダウロード

	// メニュー権限機能レベル：定期購入情報
	public const int KBN_MENU_FUNCTION_FIXEDPURCHASE_DL = 1;			// 定期購入マスタダウンロード
	public const int KBN_MENU_FUNCTION_FIXEDPURCHASE_ITEM_DL = 2;		// 定期購入商品マスタダウンロード

	// メニュー権限機能レベル：定期ワークフロー情報
	public const int KBN_MENU_FUNCTION_FIXEDPURCHASEWF_DL = 1;			// 定期購入マスタダウンロード
	public const int KBN_MENU_FUNCTION_FIXEDPURCHASEWF_ITEM_DL = 2;		// 定期購入商品マスタダウンロード

	// メニュー権限機能レベル：商品情報
	public const int KBN_MENU_FUNCTION_PRODUCT_DL = 1;			// 商品ダウンロード
	public const int KBN_MENU_FUNCTION_PRODUCT_VAL_DL = 2;		// 商品バリエーションダウンロード
	public const int KBN_MENU_FUNCTION_PRODUCT_EXD_DL = 4;		// 商品拡張項目ダウンロード
	public const int KBN_MENU_FUNCTION_PRODUCT_PRICE_DL = 8;	// 会員ランク価格ダウンロード
	public const int KBN_MENU_FUNCTION_PRODUCT_TAG_DL = 16;		// 商品タグダウンロード
	public const int KBN_MENU_FUNCTION_PRODUCT_DETAILURL_DL = 32; // 商品詳細ページURLダウンロード
	/// <summary> 商品初期設定編集 </summary>
	public const int KBN_MENU_FUNCTION_PRODUCT_DEFAULT_SETTINGS_EDIT = 64;

	// メニュー権限機能レベル：商品カテゴリ情報
	public const int KBN_MENU_FUNCTION_PRODUCTCAT_DL = 1;		// 商品カテゴリマスタダウンロード
	public const int KBN_MENU_FUNCTION_PRODUCTCAT_PRODUCTLIST_URL_DL = 2; // 商品一覧ページURLダウンロード
	// メニュー権限機能レベル：商品在庫情報
	public const int KBN_MENU_FUNCTION_PRODUCTSTK_DL = 1;		// 商品在庫マスタダウンロード
	// メニュー権限機能レベル：発注入庫情報
	public const int KBN_MENU_FUNCTION_STOCKORDER_DL = 1;		// 発注マスタダウンロード
	public const int KBN_MENU_FUNCTION_STOCKORDER_ITEM_DL = 2;	// 発注商品マスタダウンロード
	// メニュー権限機能レベル：商品レビュー情報
	public const int KBN_MENU_FUNCTION_PRODUCTREVIEW_DL = 1;	// 商品レビューダウンロード
	// メニュー権限機能レベル：商品セール情報
	public const int KBN_MENU_FUNCTION_PRODUCTSALEPRICE_DL = 1;	// 商品セール価格ダウンロード
	// メニュー権限機能レベル：入荷通知メール情報
	public const int KBN_MENU_FUNCTION_ARRIVALMAIL_DL = 1;		// 入荷通知ダウンロード
	// メニュー権限機能レベル：シリアルキー情報
	public const int KBN_MENU_FUNCTION_SERIALKEY_DL = 1;		// シリアルキーマスタダウンロード

	// メニュー権限機能レベル： リアル店舗
	public const int KBN_MENU_FUNCTION_REALSHOP_DL = 1;				// リアル店舗情報マスタダウンロード
	// メニュー権限機能レベル： リアル店舗商品在庫
	public const int KBN_MENU_FUNCTION_REALSHOPPRODUCTSTOCK_DL = 1;				// リアル店舗商品在庫情報マスタダウンロード
	// メニュー権限機能レベル： データ結合マスタ
	public const int KBN_MENU_FUNCTION_DATABINDING_DL = 256;

	// メニュー権限機能レベル：モバイルページ設定
	public const int KBN_MENU_FUNCTION_MOBILEPAGE_DL = 1;	// XMLダウンロード

	// メニュー権限機能レベル：ショートURL情報
	public const int KBN_MENU_FUNCTION_SHORTURL_DL = 1;		// ショートURLダウンロード

	// ユーザ一覧ソート区分
	public const string KBN_SORT_USER_LIST_NAME_ASC = "0";		// 氏名/昇順
	public const string KBN_SORT_USER_LIST_NAME_DESC = "1";		// 氏名/降順
	public const string KBN_SORT_USER_LIST_NAME_KANA_ASC = "2";		// 氏名(かな)/昇順
	public const string KBN_SORT_USER_LIST_NAME_KANA_DESC = "3";		// 氏名(かな)/降順
	public const string KBN_SORT_USER_LIST_DATE_CREATED_ASC = "4";		// 作成日/昇順
	public const string KBN_SORT_USER_LIST_DATE_CREATED_DESC = "5";		// 作成日/降順
	public const string KBN_SORT_USER_LIST_DATE_CHANGED_ASC = "6";		// 更新日/昇順
	public const string KBN_SORT_USER_LIST_DATE_CHANGED_DESC = "7";		// 更新日/降順
	public const string KBN_SORT_USER_LIST_USER_ID_ASC = "8";		// ユーザID/昇順
	public const string KBN_SORT_USER_LIST_USER_ID_DESC = "9";		// ユーザID/降順
	public static string KBN_SORT_USER_LIST_DEFAULT = KBN_SORT_USER_LIST_USER_ID_DESC;	// 作成日/降順 がデフォルト

	// ユーザー統合一覧ソート区分
	public const string KBN_SORT_USERINTEGRATION_LIST_USER_INTEGRATION_NO_ASC = "0"; // ユーザー統合No/昇順
	public const string KBN_SORT_USERINTEGRATION_LIST_USER_INTEGRATION_NO_DESC = "1"; // ユーザー統合No/降順
	public const string KBN_SORT_USERINTEGRATION_LIST_DATE_CREATED_ASC = "2"; // 作成日/昇順
	public const string KBN_SORT_USERINTEGRATION_LIST_DATE_CREATED_DESC = "3"; // 作成日/降順
	public const string KBN_SORT_USERINTEGRATION_LIST_DATE_CHANGED_ASC = "4"; // 更新日/昇順
	public const string KBN_SORT_USERINTEGRATION_LIST_DATE_CHANGED_DESC = "5"; // 更新日/降順
	public static string KBN_SORT_USERINTEGRATION_LIST_DEFAULT = KBN_SORT_USERINTEGRATION_LIST_USER_INTEGRATION_NO_DESC; // No/降順がデフォルト

	// ワークフロー設定一覧ソート区分
	public const string KBN_SORT_ORDERWORKFLOWSETTING_LIST_WORKFLOW_KBN_ASC = "0";		// ワークフロー区分
	public const string KBN_SORT_ORDERWORKFLOWSETTING_LIST_DISPLAY_ORDER_ASC = "1";		// 実行順/昇順
	public const string KBN_SORT_ORDERWORKFLOWSETTING_LIST_DISPLAY_ORDER_DESC = "2";	// 実行順/降順
	public const string KBN_SORT_ORDERWORKFLOWSETTING_LIST_DATE_CREATED_ASC = "3";	// 作成日/昇順
	public const string KBN_SORT_ORDERWORKFLOWSETTING_LIST_DATE_CREATED_DESC = "4";	// 作成日/降順
	public const string KBN_SORT_ORDERWORKFLOWSETTING_LIST_DATE_CHANGED_ASC = "5";	// 更新日/昇順
	public const string KBN_SORT_ORDERWORKFLOWSETTING_LIST_DATE_CHANGED_DESC = "6";	// 更新日/降順

	// 商品レビューソート区分
	public const string KBN_SORT_PRODUCTREVIEW_LIST_DATE_CREATED_ASC = "0";		// 投稿日/昇順
	public const string KBN_SORT_PRODUCTREVIEW_LIST_DATE_CREATED_DESC = "1";		// 投稿日/降順
	public const string KBN_SORT_PRODUCTREVIEW_LIST_DATE_OPENED_ASC = "2";		// 公開日/昇順
	public const string KBN_SORT_PRODUCTREVIEW_LIST_DATE_OPENED_DESC = "3";		// 公開日/降順
	public const string KBN_SORT_PRODUCTREVIEW_LIST_DATE_CHECKED_ASC = "4";		// チェック日/昇順
	public const string KBN_SORT_PRODUCTREVIEW_LIST_DATE_CHECKED_DESC = "5";		// チェック日/降順
	public static string KBN_SORT_PRODUCTREVIEW_LIST_DEFAULT = KBN_SORT_PRODUCTREVIEW_LIST_DATE_CREATED_DESC;	// 投稿日/降順がデフォルト

	// セットプロモーション一覧ソート区分
	public const string KBN_SORT_SETPROMOTION_LIST_STATUS_ASC = "0";				// 開催状態順
	public const string KBN_SORT_SETPROMOTION_LIST_SETPROMOTION_ID_ASC = "1";		// セットプロモーションID/昇順
	public const string KBN_SORT_SETPROMOTION_LIST_SETPROMOTION_ID_DESC = "2";		// セットプロモーションID/降順
	public const string KBN_SORT_SETPROMOTION_LIST_SETPROMOTION_NAME_ASC = "3";		// セットプロモーション名(管理用)/昇順
	public const string KBN_SORT_SETPROMOTION_LIST_SETPROMOTION_NAME_DESC = "4";	// セットプロモーション名(管理用)/降順
	public const string KBN_SORT_SETPROMOTION_LIST_BEGIN_DATE_ASC = "5";			// 開始日時/昇順
	public const string KBN_SORT_SETPROMOTION_LIST_BEGIN_DATE_DESC = "6";			// 開始日時/降順
	public const string KBN_SORT_SETPROMOTION_LIST_END_DATE_ASC = "7";				// 終了日時/昇順
	public const string KBN_SORT_SETPROMOTION_LIST_END_DATE_DESC = "8";				// 終了日時/降順
	public static string KBN_SORT_SETPROMOTION_LIST_DEFAULT = KBN_SORT_SETPROMOTION_LIST_STATUS_ASC;	// 開催状態順がデフォルト

	// 商品在庫一覧検索キー区分
	public const string KBN_SEARCHKEY_PRODUCTSTOCK_LIST_PRODUCT_ID = "0";		// 商品ID
	public const string KBN_SEARCHKEY_PRODUCTSTOCK_LIST_NAME = "1";		// 商品名
	public const string KBN_SEARCHKEY_PRODUCTSTOCK_LIST_NAME_KANA = "2";		// 商品名(フリガナ)
	public const string KBN_SEARCHKEY_PRODUCTSTOCK_LIST_CATEGORY_ID = "3";		// カテゴリID
	public const string KBN_SEARCHKEY_PRODUCTSTOCK_LIST_DEFAULT = KBN_SEARCHKEY_PRODUCTSTOCK_LIST_PRODUCT_ID;// 商品IDがデフォルト

	// 商品在庫一覧ソート区分
	public const string KBN_SORT_PRODUCTSTOCK_LIST_PRODUCT_ID_ASC = "0";		// 商品ID/昇順
	public const string KBN_SORT_PRODUCTSTOCK_LIST_PRODUCT_ID_DESC = "1";		// 商品ID/降順
	public const string KBN_SORT_PRODUCTSTOCK_LIST_NAME_ASC = "2";		// 商品名/昇順
	public const string KBN_SORT_PRODUCTSTOCK_LIST_NAME_DESC = "3";		// 商品名/降順
	public const string KBN_SORT_PRODUCTSTOCK_LIST_NAME_KANA_ASC = "4";		// 商品フリガナ/昇順
	public const string KBN_SORT_PRODUCTSTOCK_LIST_NAME_KANA_DESC = "5";		// 商品フリガナ/降順
	public const string KBN_SORT_PRODUCTSTOCK_LIST_DATE_CREATED_ASC = "6";		// 作成日/昇順
	public const string KBN_SORT_PRODUCTSTOCK_LIST_DATE_CREATED_DESC = "7";		// 作成日/降順
	public const string KBN_SORT_PRODUCTSTOCK_LIST_DATE_CHANGED_ASC = "8";		// 更新日/昇順
	public const string KBN_SORT_PRODUCTSTOCK_LIST_DATE_CHANGED_DESC = "9";		// 更新日/降順
	public static string KBN_SORT_PRODUCTSTOCK_LIST_DEFAULT = KBN_SORT_PRODUCTSTOCK_LIST_PRODUCT_ID_ASC;	// 商品ID/昇順がデフォルト

	// 商品在庫一覧商品在庫区分
	public const string KBN_PRODUCTSTOCK_ALERT_PRODUCTSTOCK_LIST_ALL = "0";		// 在庫管理商品全て
	public const string KBN_PRODUCTSTOCK_ALERT_PRODUCTSTOCK_LIST_ALERT = "1";		// 安全在庫アラート
	public const string KBN_PRODUCTSTOCK_ALERT_PRODUCTSTOCK_LIST_SUSPENSION = "2";	// 在庫切れ販売停止
	public const string KBN_PRODUCTSTOCK_ALERT_PRODUCTSTOCK_LIST_DEFAULT = KBN_PRODUCTSTOCK_ALERT_PRODUCTSTOCK_LIST_ALL;		// 安全在庫アラート

	// 商品在庫一覧表示区分
	public const string KBN_PRODUCTSTOCK_DISPLAY_LIST = "0";				// 一覧表示
	public const string KBN_PRODUCTSTOCK_DISPLAY_EDIT = "1";				// 編集表示
	public const string KBN_PRODUCTSTOCK_DISPLAY_COMPLETE = "2";				// 完了表示
	public const string KBN_PRODUCTSTOCK_DISPLAY_DEFAULT = KBN_PRODUCTSTOCK_DISPLAY_LIST;	// 一覧表示がデフォルト

	// 在庫一覧商品表示在庫数検索区分
	public const string KBN_PRODUCTSTOCK_SEARCH_STOCK_KEY_STOCK = "stock";										// 論理在庫
	public const string KBN_PRODUCTSTOCK_SEARCH_STOCK_KEY_REALSTOCK = "realstock";								// 実在庫 A品
	public const string KBN_PRODUCTSTOCK_SEARCH_STOCK_KEY_REALSTOCK_B = "realstock_b";							// 実在庫 B品
	public const string KBN_PRODUCTSTOCK_SEARCH_STOCK_KEY_REALSTOCK_C = "realstock_c";							// 実在庫 C品

	public const string KBN_PRODUCTSTOCK_SEARCH_STOCK_COUNT_TYPE_EQUAL = "0";		// 等しい
	public const string KBN_PRODUCTSTOCK_SEARCH_STOCK_COUNT_TYPE_OVER = "1";		// 以上
	public const string KBN_PRODUCTSTOCK_SEARCH_STOCK_COUNT_TYPE_UNDER = "2";		// 以下
	public const string KBN_PRODUCTSTOCK_SEARCH_STOCK_COUNT_TYPE_DEFAULT = KBN_PRODUCTSTOCK_SEARCH_STOCK_COUNT_TYPE_OVER;	// 等しいがデフォルト

	// 商品セール一覧ソート区分
	public const string KBN_SORT_PRODUCTSALE_DATE_BGN_ASC = "0";			// 開始日時/昇順
	public const string KBN_SORT_PRODUCTSALE_DATE_BGN_DESC = "1";			// 開始日時/降順
	public const string KBN_SORT_PRODUCTSALE_ID_ASC = "2";					// 商品セールID/昇順
	public const string KBN_SORT_PRODUCTSALE_ID_DESC = "3";					// 商品セールID/降順
	public static string KBN_SORT_PRODUCTSALE_DEFAULT = KBN_SORT_PRODUCTSALE_DATE_BGN_DESC;	// 開始日時がデフォルト

	// 商品ランキング一覧ソート区分
	public const string KBN_SORT_PRODUCTRANKING_LIST_RANKING_ID_ASC = "0";		// 商品ランキングID/昇順
	public const string KBN_SORT_PRODUCTRANKING_LIST_RANKING_ID_DESC = "1";	// 商品ランキングID/降順
	public const string KBN_SORT_PRODUCTRANKING_LIST_DATE_CREATED_ASC = "2";	// 作成日/昇順
	public const string KBN_SORT_PRODUCTRANKING_LIST_DATE_CREATED_DESC = "3";	// 作成日/降順
	public const string KBN_SORT_PRODUCTRANKING_LIST_DATE_CHANGED_ASC = "4";	// 更新日/昇順
	public const string KBN_SORT_PRODUCTRANKING_LIST_DATE_CHANGED_DESC = "5";	// 更新日/降順
	public static string KBN_SORT_PRODUCTRANKING_DEFAULT = KBN_SORT_PRODUCTRANKING_LIST_RANKING_ID_ASC;	// 商品ランキングID/昇順がデフォルト

	// 入荷通知メール情報一覧ソート区分
	public const string KBN_SORT_USERPRODUCTARRIVALMAIL_PRODUCT_ID_ASC = "0";			// 商品ID/昇順
	public const string KBN_SORT_USERPRODUCTARRIVALMAIL_PRODUCT_ID_DESC = "1";			// 商品ID/降順
	public const string KBN_SORT_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_ASC = "2";	// 販売開始日/昇順
	public const string KBN_SORT_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DESC = "3";	// 販売開始日/降順
	public const string KBN_SORT_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_COUNT_ASC = "4";	// 再入荷通知件数/昇順
	public const string KBN_SORT_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_COUNT_DESC = "5";	// 再入荷通知件数/降順
	public const string KBN_SORT_USERPRODUCTARRIVALMAIL_RELEASE_MAIL_COUNT_ASC = "6";	// 販売開始通知件数/昇順
	public const string KBN_SORT_USERPRODUCTARRIVALMAIL_RELEASE_MAIL_COUNT_DESC = "7";	// 販売開始通知件数/降順
	public const string KBN_SORT_USERPRODUCTARRIVALMAIL_RESALE_MAIL_COUNT_ASC = "8";	// 再販売通知件数/昇順
	public const string KBN_SORT_USERPRODUCTARRIVALMAIL_RESALE_MAIL_COUNT_DESC = "9";	// 再販売通知件数/降順
	public const string KBN_SORT_USERPRODUCTARRIVALMAIL_DEFAULT = KBN_SORT_USERPRODUCTARRIVALMAIL_PRODUCT_ID_ASC;	// 商品ID/昇順がデフォルト

	// 商品検索区分
	public const string KBN_PRODUCT_SEARCH_PRODUCT = "product";				// 商品検索
	public const string KBN_PRODUCT_SEARCH_VARIATION = "variation";			// バリエーション検索
	public const string KBN_PRODUCT_SEARCH_ORDERPRODUCT = "orderproduct";	// 注文商品検索
	public const string KBN_PRODUCT_SEARCH_SUBSCRIPTION_BOX = "subscriptionbox";			// 頒布会

	// リアル店舗一覧ソート区分
	public const string KBN_SORT_REALSHOP_LIST_REAL_SHOP_ID_ASC = "1";			// リアル店舗ID/昇順
	public const string KBN_SORT_REALSHOP_LIST_REAL_SHOP_ID_DESC = "2";			// リアル店舗ID/降順
	public const string KBN_SORT_REALSHOP_LIST_REAL_SHOP_NAME_ASC = "3";		// リアル店舗名/昇順
	public const string KBN_SORT_REALSHOP_LIST_REAL_SHOP_NAME_DESC = "4";		// リアル店舗名/降順
	public const string KBN_SORT_REALSHOP_LIST_REAL_SHOP_NAME_KANA_ASC = "5";	// リアル店舗名/昇順
	public const string KBN_SORT_REALSHOP_LIST_REAL_SHOP_NAME_KANA_DESC = "6";	// リアル店舗名/降順
	public const string KBN_SORT_REALSHOP_LIST_DISPLAY_ORDER_ASC = "7";			// 表示順/昇順
	public const string KBN_SORT_REALSHOP_LIST_DISPLAY_ORDER_DESC = "8";		// 表示順/降順
	public static string KBN_SORT_REALSHOP_LIST_DEFAULT = KBN_SORT_REALSHOP_LIST_DISPLAY_ORDER_ASC;	// 表示順/昇順がデフォルト

	// リアル店舗商品在庫一覧ソート区分
	public const string KBN_SORT_REALSHOPPRODUCTSTOCK_LIST_PRODUCT_ID_ASC = "1";		// 商品ID/昇順
	public const string KBN_SORT_REALSHOPPRODUCTSTOCK_LIST_PRODUCT_ID_DESC = "2";		// 商品ID/降順
	public const string KBN_SORT_REALSHOPPRODUCTSTOCK_LIST_NAME_ASC = "3";				// 商品名/昇順
	public const string KBN_SORT_REALSHOPPRODUCTSTOCK_LIST_NAME_DESC = "4";				// 商品名/降順
	public const string KBN_SORT_REALSHOPPRODUCTSTOCK_LIST_NAME_KANA_ASC = "5";			// 商品フリガナ/昇順
	public const string KBN_SORT_REALSHOPPRODUCTSTOCK_LIST_NAME_KANA_DESC = "6";		// 商品フリガナ/降順
	public const string KBN_SORT_REALSHOPPRODUCTSTOCK_LIST_DATE_CREATED_ASC = "7";		// 作成日/昇順
	public const string KBN_SORT_REALSHOPPRODUCTSTOCK_LIST_DATE_CREATED_DESC = "8";		// 作成日/降順
	public const string KBN_SORT_REALSHOPPRODUCTSTOCK_LIST_DATE_CHANGED_ASC = "9";		// 更新日/昇順
	public const string KBN_SORT_REALSHOPPRODUCTSTOCK_LIST_DATE_CHANGED_DESC = "10";		// 更新日/降順
	public static string KBN_SORT_REALSHOPPRODUCTSTOCK_LIST_DEFAULT = KBN_SORT_REALSHOPPRODUCTSTOCK_LIST_PRODUCT_ID_ASC;	// 商品ID/昇順がデフォルト

	// 定期商品変更設定ソート区分
	/// <summary>定期商品変更設定ID/昇順</summary>
	public const string KBN_SORT_FIXEDPURCHASEPRODUCTCHANGESETTING_LIST_FIXED_PURCHASE_PRODUCT_CHANGE_ID_ASC = "0";
	/// <summary>定期商品変更設定ID/降順</summary>
	public const string KBN_SORT_FIXEDPURCHASEPRODUCTCHANGESETTING_LIST_FIXED_PURCHASE_PRODUCT_CHANGE_ID_DESC = "1";
	/// <summary>適用優先順</summary>
	public const string KBN_SORT_FIXEDPURCHASEPRODUCTCHANGESETTING_LIST_PRIORITY_DESC = "2";
	/// <summary>作成日/昇順</summary>
	public const string KBN_SORT_FIXEDPURCHASEPRODUCTCHANGESETTING_LIST_DATE_CREATED_ASC = "3";
	/// <summary>作成日/降順</summary>
	public const string KBN_SORT_FIXEDPURCHASEPRODUCTCHANGESETTING_LIST_DATE_CREATED_DESC = "4";
	/// <summary>更新日/昇順</summary>
	public const string KBN_SORT_FIXEDPURCHASEPRODUCTCHANGESETTING_LIST_DATE_CHANGED_ASC = "5";
	/// <summary>更新日/降順</summary>
	public const string KBN_SORT_FIXEDPURCHASEPRODUCTCHANGESETTING_LIST_DATE_CHANGED_DESC = "6";
	/// <summary>定期商品変更設定ID/昇順がデフォルト</summary>
	public static string KBN_SORT_FIXEDPURCHASEPRODUCTCHANGESETTING_LIST_DEFAULT = KBN_SORT_FIXEDPURCHASEPRODUCTCHANGESETTING_LIST_FIXED_PURCHASE_PRODUCT_CHANGE_ID_ASC;

	// リアル店舗商品在庫数一覧在庫数検索区分
	public const string KBN_SORT_REALSHOPPRODUCTSTOCK_REAL_SHOP_STOCK_LESS = "2";		// 以下
	public const string KBN_SORT_REALSHOPPRODUCTSTOCK_REAL_SHOP_STOCK_GREATER = "1";	// 以上
	public const string KBN_SORT_REALSHOPPRODUCTSTOCK_REAL_SHOP_STOCK_EQUAL = "0";		// と等しい
	public static string KBN_SORT_REALSHOPPRODUCTSTOCK_REAL_SHOP_STOCK_DEFAULT = KBN_SORT_REALSHOPPRODUCTSTOCK_REAL_SHOP_STOCK_EQUAL;	// と等しい/昇順がデフォルト

	// モール出品設定一覧検索キー区分
	public const string KBN_SEARCHKEY_MALLEXHIBITSCONFIG_LIST_PRODUCT_ID = "0";		// 商品ID
	public const string KBN_SEARCHKEY_MALLEXHIBITSCONFIG_LIST_NAME = "1";		// 商品名
	public const string KBN_SEARCHKEY_MALLEXHIBITSCONFIG_LIST_NAME_KANA = "2";			// 商品名(フリガナ)
	public const string KBN_SEARCHKEY_MALLEXHIBITSCONFIG_LIST_CATEGORY_ID = "3";		// カテゴリID
	public const string KBN_SEARCHKEY_MALLEXHIBITSCONFIG_LIST_DEFAULT = KBN_SEARCHKEY_MALLEXHIBITSCONFIG_LIST_PRODUCT_ID;// 商品IDがデフォルト

	// モール出品設定一覧ソート区分
	public const string KBN_SORT_MALLEXHIBITSCONFIG_LIST_PRODUCT_ID_ASC = "0";		// 商品ID/昇順
	public const string KBN_SORT_MALLEXHIBITSCONFIG_LIST_PRODUCT_ID_DESC = "1";		// 商品ID/降順
	public const string KBN_SORT_MALLEXHIBITSCONFIG_LIST_NAME_ASC = "2";		// 商品名/昇順
	public const string KBN_SORT_MALLEXHIBITSCONFIG_LIST_NAME_DESC = "3";		// 商品名/降順
	public const string KBN_SORT_MALLEXHIBITSCONFIG_LIST_NAME_KANA_ASC = "4";		// 商品フリガナ/昇順
	public const string KBN_SORT_MALLEXHIBITSCONFIG_LIST_NAME_KANA_DESC = "5";		// 商品フリガナ/降順
	public const string KBN_SORT_MALLEXHIBITSCONFIG_LIST_DATE_CREATED_ASC = "6";		// 作成日/昇順
	public const string KBN_SORT_MALLEXHIBITSCONFIG_LIST_DATE_CREATED_DESC = "7";		// 作成日/降順
	public const string KBN_SORT_MALLEXHIBITSCONFIG_LIST_DATE_CHANGED_ASC = "8";		// 更新日/昇順
	public const string KBN_SORT_MALLEXHIBITSCONFIG_LIST_DATE_CHANGED_DESC = "9";		// 更新日/降順
	public static string KBN_SORT_MALLEXHIBITSCONFIG_LIST_DEFAULT = KBN_SORT_MALLEXHIBITSCONFIG_LIST_PRODUCT_ID_ASC;	// 商品ID/昇順がデフォルト

	// モール商品アップロード商品一覧表示区分
	public const string KBN_MALLPRODUCTUPLOAD_DIPLAY_KBN_INSERT = "0";	// 登録商品表示
	public const string KBN_MALLPRODUCTUPLOAD_DIPLAY_KBN_UPDATE = "1";	// 更新商品表示

	// メールテンプレート一覧検索キー区分
	public const string KBN_SEARCHKEY_MAILTEMPLATE_LIST_MAIL_ID = "0";		// メールテンプレートID
	public const string KBN_SEARCHKEY_MAILTEMPLATE_LIST_MAIL_NAME = "1";		// メールテンプレート名
	public const string KBN_SEARCHKEY_MAILTEMPLATE_LIST_DEFAULT = KBN_SEARCHKEY_MAILTEMPLATE_LIST_MAIL_ID;// メールテンプレートID

	// メールテンプレート一覧ソート区分
	public const string KBN_SORT_MAILTEMPLATE_LIST_MAIL_ID_ASC = "0";		// メールテンプレートID/昇順
	public const string KBN_SORT_MAILTEMPLATE_LIST_MAIL_ID_DESC = "1";		// メールテンプレートID/降順
	public const string KBN_SORT_MAILTEMPLATE_LIST_MAIL_NAME_ASC = "2";		// メールテンプレート名/昇順
	public const string KBN_SORT_MAILTEMPLATE_LIST_MAIL_NAME_DESC = "3";		// メールテンプレート名/降順
	public const string KBN_SORT_MAILTEMPLATE_LIST_DATE_CREATED_ASC = "4";		// 作成日/昇順
	public const string KBN_SORT_MAILTEMPLATE_LIST_DATE_CREATED_DESC = "5";		// 作成日/降順
	public const string KBN_SORT_MAILTEMPLATE_LIST_DATE_CHANGED_ASC = "6";		// 更新日/昇順
	public const string KBN_SORT_MAILTEMPLATE_LIST_DATE_CHANGED_DESC = "7";		// 更新日/降順
	public static string KBN_SORT_MAILTEMPLATE_LIST_DEFAULT = KBN_SORT_MAILTEMPLATE_LIST_MAIL_ID_ASC;	// メールテンプレートID/昇順がデフォルト

	// 注文一覧ソート区分
	public const string KBN_SORT_ORDER_ID_ASC = "0";						// 注文ID/昇順
	public const string KBN_SORT_ORDER_ID_DESC = "1";						// 注文ID/降順
	public const string KBN_SORT_ORDER_DATE_ASC = "2";						// 注文日/昇順
	public const string KBN_SORT_ORDER_DATE_DESC = "3";						// 注文日/降順
	public const string KBN_SORT_ORDER_DATE_CREATED_ASC = "4";						// 作成日/昇順
	public const string KBN_SORT_ORDER_DATE_CREATED_DESC = "5";						// 作成日/降順
	public const string KBN_SORT_ORDER_DATE_CHANGED_ASC = "6";						// 更新日/昇順
	public const string KBN_SORT_ORDER_DATE_CHANGED_DESC = "7";						// 更新日/降順
	public static string KBN_SORT_ORDER_DATE_DEFAULT = KBN_SORT_ORDER_DATE_DESC;	// 注文日/降順がデフォルト

	// 注文一覧表示区分
	public const string KBN_ORDER_DISPLAY_LIST = "0";				// 一覧表示
	public const string KBN_ORDER_DISPLAY_EDIT = "1";				// 編集表示
	public const string KBN_ORDER_DISPLAY_COMPLETE = "2";				// 完了表示
	public const string KBN_ORDER_DISPLAY_DEFAULT = KBN_ORDER_DISPLAY_LIST;	// 一覧表示がデフォルト

	// 定期購入一覧ソート区分
	public const string KBN_SORT_FIXEDPURCHASE_DATE_CREATED_ASC = "0"; // 作成日/昇順
	public const string KBN_SORT_FIXEDPURCHASE_DATE_CREATED_DESC = "1"; // 作成日/降順
	public const string KBN_SORT_FIXEDPURCHASE_DATE_CHANGED_ASC = "2"; // 更新日/昇順
	public const string KBN_SORT_FIXEDPURCHASE_DATE_CHANGED_DESC = "3"; // 更新日/降順
	public const string KBN_SORT_FIXEDPURCHASE_ID_ASC = "4"; // 定期購入ID/昇順
	public const string KBN_SORT_FIXEDPURCHASE_ID_DESC = "5"; // 定期購入ID/降順
	public const string KBN_SORT_FIXEDPURCHASE_NEXT_SHIPPING_DATE_ASC = "6"; //次回配送日/昇順
	public const string KBN_SORT_FIXEDPURCHASE_NEXT_SHIPPING_DATE_DESC = "7"; //次回配送日/降順
	public const string KBN_SORT_FIXEDPURCHASE_NEXT_NEXT_SHIPPING_DATE_ASC = "8"; //次々回配送日/昇順
	public const string KBN_SORT_FIXEDPURCHASE_NEXT_NEXT_SHIPPING_DATE_DESC = "9"; //次々回配送日/降順
	public const string KBN_SORT_FIXEDPURCHASE_FIXED_PURCHASE_DATE_BGN_ASC = "10"; //定期購入開始日時/昇順
	public const string KBN_SORT_FIXEDPURCHASE_FIXED_PURCHASE_DATE_BGN_DESC = "11"; //定期購入開始日時/降順
	public const string KBN_SORT_FIXEDPURCHASE_LAST_ORDER_DATE_ASC = "12"; //最終購入日/昇順
	public const string KBN_SORT_FIXEDPURCHASE_LAST_ORDER_DATE_DESC = "13"; //最終購入日/降順
	public static string KBN_SORT_FIXEDPURCHASE_DATE_DEFAULT = KBN_SORT_FIXEDPURCHASE_DATE_CREATED_DESC; // 作成日/降順がデフォルト

	// モバイルページ一覧ソート区分
	public const string KBN_SORT_MOBILEPAGE_LIST_MOBILE_PAGE_ID_ASC = "0";		// ページID/昇順
	public const string KBN_SORT_MOBILEPAGE_LIST_MOBILE_PAGE_ID_DESC = "1";		// ページID/降順
	public const string KBN_SORT_MOBILEPAGE_LIST_MOBILE_PAGE_NAME_ASC = "2";	// ページ名/昇順
	public const string KBN_SORT_MOBILEPAGE_LIST_MOBILE_PAGE_NAME_DESC = "3";	// ページ名/降順
	public static string KBN_SORT_MOBILEPAGE_LIST_DEFAULT = KBN_SORT_MOBILEPAGE_LIST_MOBILE_PAGE_ID_ASC;	// ページID/昇順がデフォルト

	// 機種グループ一覧ソート区分
	public const string KBN_SORT_MOBILEGROUP_LIST_MOBILE_GROUP_ID_ASC = "0";		// 機種グループID/昇順
	public const string KBN_SORT_MOBILEGROUP_LIST_MOBILE_GROUP_ID_DESC = "1";		// 機種グループID/降順
	public const string KBN_SORT_MOBILEGROUP_LIST_MOBILE_GROUP_NAME_ASC = "2";		// 機種グループ名/昇順
	public const string KBN_SORT_MOBILEGROUP_LIST_MOBILE_GROUP_NAME_DESC = "3";		// 機種グループ名/降順
	public static string KBN_SORT_MOBILEGROUP_LIST_DEFAULT = KBN_SORT_MOBILEGROUP_LIST_MOBILE_GROUP_ID_ASC;	// タグID/昇順がデフォルト

	// モバイルオリジナルタグ一覧ソート区分
	public const string KBN_SORT_MOBILEORIGINALTAG_LIST_ORGTAG_ID_ASC = "0";		// タグID/昇順
	public const string KBN_SORT_MOBILEORIGINALTAG_LIST_ORGTAG_ID_DESC = "1";		// タグID/降順
	public const string KBN_SORT_MOBILEORIGINALTAG_LIST_ORGTAG_NAME_ASC = "2";		// 表示名/昇順
	public const string KBN_SORT_MOBILEORIGINALTAG_LIST_ORGTAG_NAME_DESC = "3";		// 表示名/降順
	public static string KBN_SORT_MOBILEORIGINALTAG_LIST_DEFAULT = KBN_SORT_MOBILEORIGINALTAG_LIST_ORGTAG_ID_ASC;	// タグID/昇順がデフォルト

	// 絵文字一覧ソート区分
	public const string KBN_SORT_MOBILEPICTORIALSYMBOL_LIST_SYMBOL_ID_ASC = "0";	// 絵文字ID/昇順
	public const string KBN_SORT_MOBILEPICTORIALSYMBOL_LIST_SYMBOL_ID_DESC = "1";	// 絵文字ID/降順
	public const string KBN_SORT_MOBILEPICTORIALSYMBOL_LIST_SYMBOL_NAME_ASC = "2";	// 絵文字名/昇順
	public const string KBN_SORT_MOBILEPICTORIALSYMBOL_LIST_SYMBOL_NAME_DESC = "3";	// 絵文字名/降順
	public const string KBN_SORT_MOBILEPICTORIALSYMBOL_LIST_SYMBOL_TAG_ASC = "4";	// 絵文字タグ/昇順
	public const string KBN_SORT_MOBILEPICTORIALSYMBOL_LIST_SYMBOL_TAG_DESC = "5";	// 絵文字タグ/降順
	public static string KBN_SORT_MOBILEPICTORIALSYMBOL_LIST_DEFAULT = KBN_SORT_MOBILEPICTORIALSYMBOL_LIST_SYMBOL_ID_ASC;	// 絵文字ID/昇順がデフォルト

	// 新着情報一覧ソート区分
	public const string KBN_SORT_NEWS_LIST_DISPLAY_DATE_ASC = "0";							// 開始日時/昇順
	public const string KBN_SORT_NEWS_LIST_DISPLAY_DATE_DESC = "1";							// 開始日時/降順
	public const string KBN_SORT_NEWS_LIST_DISPLAY_DATE_TO_ASC = "4";						// 終了日時/昇順
	public const string KBN_SORT_NEWS_LIST_DISPLAY_DATE_TO_DESC = "5";						// 終了日時/降順
	public const string KBN_SORT_NEWS_LIST_DISPLAY_ORDER_ASC = "2";							// 並び順/昇順
	public const string KBN_SORT_NEWS_LIST_DISPLAY_ORDER_DESC = "3";						// 並び順/降順
	public static string KBN_SORT_NEWS_LIST_DEFAULT = KBN_SORT_NEWS_LIST_DISPLAY_ORDER_ASC;	// 並び順/昇順がデフォルト

	// ショートURL一覧ソート区分
	public const string KBN_SORT_SHORTURL_LIST_DATE_CREATED_ASC = "0";		// 登録日/昇順
	public const string KBN_SORT_SHORTURL_LIST_DATE_CREATED_DESC = "1";		// 登録日/降順
	public const string KBN_SORT_SHORTURL_LIST_SHORTURL_ASC = "2";				// ショートURL/昇順
	public static string KBN_SORT_SHORTURL_LIST_DEFAULT = KBN_SORT_SHORTURL_LIST_DATE_CREATED_DESC;	// 登録日/降順がデフォルト

	// シリアルキー一覧ソート区分
	public const string KBN_SORT_SERIALKEY_LIST_SERIAL_KEY_ASC = "00"; // シリアルキー/昇順
	public const string KBN_SORT_SERIALKEY_LIST_SERIAL_KEY_DESC = "01"; // シリアルキー/降順
	public const string KBN_SORT_SERIALKEY_LIST_PRODUCT_ID_ASC = "02"; // 商品ID/昇順
	public const string KBN_SORT_SERIALKEY_LIST_PRODUCT_ID_DESC = "03"; // 商品ID/降順
	public const string KBN_SORT_SERIALKEY_LIST_USER_ID_ASC = "04"; // ユーザーID/昇順
	public const string KBN_SORT_SERIALKEY_LIST_USER_ID_DESC = "05"; // ユーザーID/降順
	public const string KBN_SORT_SERIALKEY_LIST_ORDER_ID_ASC = "06"; // 注文ID/昇順
	public const string KBN_SORT_SERIALKEY_LIST_ORDER_ID_DESC = "07"; // 注文ID/降順
	public const string KBN_SORT_SERIALKEY_LIST_DATE_CREATED_ASC = "08"; // 作成日/昇順
	public const string KBN_SORT_SERIALKEY_LIST_DATE_CREATED_DESC = "09"; // 作成日/降順
	public const string KBN_SORT_SERIALKEY_LIST_DATE_CHANGED_ASC = "10"; // 更新日/昇順
	public const string KBN_SORT_SERIALKEY_LIST_DATE_CHANGED_DESC = "11"; // 更新日/降順
	public const string KBN_SORT_SERIALKEY_LIST_DEFAULT = KBN_SORT_SERIALKEY_LIST_PRODUCT_ID_ASC; // デフォルト

	// ノベルティ設定
	public const string KBN_SORT_NOVELTY_LIST_STATUS = "0"; 								// 開催状態順
	public const string KBN_SORT_NOVELTY_LIST_NOVELTY_ID_ASC = "1"; 						// ノベルティID/昇順
	public const string KBN_SORT_NOVELTY_LIST_NOVELTY_ID_DESC = "2"; 						// ノベルティID/降順
	public const string KBN_SORT_NOVELTY_LIST_NOVELTY_DISP_NAME_ASC = "3"; 					// ノベルティ名(表示用)/昇順
	public const string KBN_SORT_NOVELTY_LIST_NOVELTY_DISP_NAME_DESC = "4";					// ノベルティ名(表示用)/降順
	public const string KBN_SORT_NOVELTY_LIST_NOVELTY_NAME_ASC = "5"; 						// ノベルティ名(管理用)/昇順
	public const string KBN_SORT_NOVELTY_LIST_NOVELTY_NAME_DESC = "6"; 						// ノベルティ名(管理用)/降順
	public const string KBN_SORT_NOVELTY_LIST_DATE_BEGIN_ASC = "7"; 						// 開始日時/昇順
	public const string KBN_SORT_NOVELTY_LIST_DATE_BEGIN_DESC = "8"; 						// 開始日時/降順
	public const string KBN_SORT_NOVELTY_LIST_DATE_END_ASC = "9"; 							// 終了日時/昇順
	public const string KBN_SORT_NOVELTY_LIST_DATE_END_DESC = "10"; 						// 終了日時/降順

	// レコメンド設定
	public const string KBN_SORT_RECOMMEND_LIST_STATUS = "0"; 					// 開催状態順
	public const string KBN_SORT_RECOMMEND_LIST_RECOMMEND_ID_ASC = "1"; 		// レコメンドID/昇順
	public const string KBN_SORT_RECOMMEND_LIST_RECOMMEND_ID_DESC = "2"; 		// レコメンドID/降順
	public const string KBN_SORT_RECOMMEND_LIST_RECOMMEND_NAME_ASC = "3"; 		// レコメンド名(管理用)/昇順
	public const string KBN_SORT_RECOMMEND_LIST_RECOMMEND_NAME_DESC = "4";		// レコメンド名(管理用)/降順
	public const string KBN_SORT_RECOMMEND_LIST_PRIORITY_ASC = "5"; 	// 適用優先順/昇順
	public const string KBN_SORT_RECOMMEND_LIST_PRIORITY_DESC = "6";	// 適用優先順/降順
	public const string KBN_SORT_RECOMMEND_LIST_DATE_BEGIN_ASC = "7"; 			// 開始日時/昇順
	public const string KBN_SORT_RECOMMEND_LIST_DATE_BEGIN_DESC = "8"; 			// 開始日時/降順
	public const string KBN_SORT_RECOMMEND_LIST_DATE_END_ASC = "9"; 			// 終了日時/昇順
	public const string KBN_SORT_RECOMMEND_LIST_DATE_END_DESC = "10"; 			// 終了日時/降順

	// 商品同梱設定ソート区分
	///<Summary>商品同梱ID/昇順</Summary>
	public const string KBN_SORT_PRODUCTBUNDLE_LIST_PRODUCT_BUNDLE_ID_ASC = "0";
	///<Summary>商品同梱ID/降順</Summary>
	public const string KBN_SORT_PRODUCTBUNDLE_LIST_PRODUCT_BUNDLE_ID_DESC = "1";
	///<Summary>商品同梱名/昇順</Summary>
	public const string KBN_SORT_PRODUCTBUNDLE_LIST_PRODUCT_BUNDLE_NAME_ASC = "2";
	///<Summary>商品同梱名/降順</Summary>
	public const string KBN_SORT_PRODUCTBUNDLE_LIST_PRODUCT_BUNDLE_NAME_DESC = "3";
	///<Summary>開始日時/昇順</Summary>
	public const string KBN_SORT_PRODUCTBUNDLE_LIST_START_DATE_TIME_ASC = "4";
	///<Summary>開始日時/降順</Summary>
	public const string KBN_SORT_PRODUCTBUNDLE_LIST_START_DATE_TIME_DESC = "5";
	///<Summary>終了日時/昇順</Summary>
	public const string KBN_SORT_PRODUCTBUNDLE_LIST_END_DATE_TIME_ASC = "6";
	///<Summary>終了日時/降順</Summary>
	public const string KBN_SORT_PRODUCTBUNDLE_LIST_END_DATE_TIME_DESC = "7";
	///<Summary>適用優先順/昇順</Summary>
	public const string KBN_SORT_PRODUCTBUNDLE_LIST_APPLY_ORDER_ASC = "8";
	///<Summary>適用優先順/降順</Summary>
	public const string KBN_SORT_PRODUCTBUNDLE_LIST_APPLY_ORDER_DESC = "9";
	/// <summary>適用優先順/既定値</summary>
	public const string KBN_SORT_PRODUCTBUNDLE_LIST_DEFAULT = KBN_SORT_PRODUCTBUNDLE_LIST_APPLY_ORDER_ASC;

	// クーポンリスト値テキスト
	/// <summary>クーポン：発行前</summary>
	public const string VALUETEXT_PARAM_COUPON_LIST_BEFORE_ISSUE_PERIOD = "before_issue_period";
	/// <summary>クーポン：発行期間中</summary>
	public const string VALUETEXT_PARAM_COUPON_LIST_DURING_ISSUE_PERIOD = "during_issue_period";
	/// <summary>クーポン：発行期間後</summary>
	public const string VALUETEXT_PARAM_COUPON_LIST_AFTER_ISSUE_PERIOD = "after_issue_period";

	// クーポンリストソート区分
	/// <summary>作成日/昇順</summary>
	public const string KBN_SORT_COUPON_LIST_DATE_CREATED_ASC = "0";
	/// <summary>作成日/降順</summary>
	public const string KBN_SORT_COUPON_LIST_DATE_CREATED_DESC = "1";
	/// <summary>更新日/昇順</summary>
	public const string KBN_SORT_COUPON_LIST_DATE_CHANGED_ASC = "2";
	/// <summary>更新日/降順</summary>
	public const string KBN_SORT_COUPON_LIST_DATE_CHANGED_DESC = "3";
	/// <summary>既定値：更新日/降順</summary>
	public const string KBN_SORT_COUPON_LIST_DEFAULT = KBN_SORT_COUPON_LIST_DATE_CHANGED_DESC;


	// ポイントオプション利用有無区分
	//public static string KBN_MARKETINGPLANNER_POINT_OPTION_VALID			= "1";		// ポイントオプション有効
	//public static string KBN_MARKETINGPLANNER_POINT_OPTION_INVALID			= "0";		// ポイントオプション無効

	// 親ページリロード区分
	public const string KBN_RELOAD_PARENT_WINDOW_ON = "1"; // リロードする
	public const string KBN_RELOAD_PARENT_WINDOW_OFF = ""; // リロードしない

	// w2CustomerSupport
	public const string KBN_MESSAGE_MEDIA_MODE_TEL = "tel";		// メッセージ媒体区分：電話
	public const string KBN_MESSAGE_EDIT_MODE_NEW = "new";		// 編集区分：新規

	// メールカテゴリー区分
	/// <summary>注文系メール</summary>
	public const int KBN_EMAIL_USAGE_CATEGORY_ORDER = 1;
	/// <summary>定期購入系メール</summary>
	public const int KBN_EMAIL_USAGE_CATEGORY_FIXEDPURCHASE = 2;
	/// <summary>会員系メール</summary>
	public const int KBN_EMAIL_USAGE_CATEGORY_USER = 3;
	/// <summary>EC事業部→各店舗</summary>
	public const int KBN_EMAIL_USAGE_CATEGORY_TOREALSHOP = 4;
	/// <summary>各店舗→お客様</summary>
	public const int KBN_EMAIL_USAGE_CATEGORY_FROMREALSHOP = 5;

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

	// 外部ファイル取込セッティング
	public const string FILE_XML_EXTERNAL_IMPORT_SETTING = "Xml/Setting/ExternalImportSetting.xml";

	// コンテンツ管理定義セッティング
	public const string FILE_XML_CONTENTSMANAGEMENT_SETTING = "Xml/Setting/ContentsManagerSetting.xml";

	// モバイルページテンプレート
	public const string FILE_XML_MOBILEPAGE_TEMPLATE = "Xml/MobilePageTemplate.xml";

	// モバイルタグマニュアル
	public const string FILE_XML_MOBILETAG_MANUAL = "Xml/Manual/MobileTagManual.xml";

	// 商品コンバータセッティング
	public const string FILE_XML_PRODUCTCONV_SETTING = "Xml/Setting/ProductConvSetting.xml";

	/// <summary>メールテンプレート置換タグファイルパス</summary>
	public const string FILEPATH_XML_MAIL_REPLACEMENT_TAG = @"Xml\Setting\MailReplacementTag.xml";

	//*****************************************************************************************
	// キー系
	//*****************************************************************************************
	//=========================================================================================
	// セッション変数キー
	//=========================================================================================
	public const string SESSION_KEY_LOGIN_OPERTOR_MENU = "w2cMng_loggedin_operator_menu";
	public const string SESSION_KEY_LOGIN_OPERTOR_MENU_ACCESS_LEVEL = "w2cMng_loggedin_operator_menu_access_level";

	public const string SESSION_KEY_ACTION_STATUS = "w2cMng_action_status";
	public const string SESSION_KEY_LOGIN_ERROR_INFO = "w2cMng_loggedin_error_info";	// ログイン失敗回数保持用
	public const string SESSION_KEY_ORDER_REGIST_INPUT_ERROR = "w2cMng_order_regist_input_error";	// 連続書き込みエラー
	public const string SESSION_KEY_PRODUCTCATEGORY_CURRENT_CATEGORY_ID = "w2cMng_productcategory_current_category_id";
	public const string SESSION_KEY_PRODUCT_ID = "w2cMng_product_id";			// 商品ID
	public const string SESSION_KEY_USERPOINT_EXPIRED_ALEAT_MESSAGE = "UserPointExpiredAlertMessage"; //ユーザーポイント戻し時の有効期限切れ警告メッセージ

	public const string SESSION_KEY_ORDERWORKFLOWDETAILS_URL_OF_PREVIOUS_PAGE = "WorkflowScenarioUrlOfPreviousPage"; //ワークフローシナリオ詳細の前ページURL

	public const string SESSION_KEY_MENUAUTHORITY_LMENUS = "menu_auth_lmenus";		// メニュー権限情報（大メニュー）
	public const string SESSION_KEY_MENUAUTHORITY_INFO = "menu_auth_info";		// メニュー権限情報（メニュー権限情報）
	/// <summary>コピー元メールID</summary>
	public const string SESSION_KEY_KEY_BEFORE_COPY_MAIL_ID = "before_copy_mail_id";
	/// <summary>メール送信エラーメッセージ</summary>
	public const string SESSION_KEY_SEND_MAIL_ERROR_MESSAGE = "SendMailErrorMessage";
	/// <summary>Show continue register button</summary>
	public const string SESSION_KEY_SHOW_CONTINUE_REGISTER_BUTTON = "ShowContinueRegisterButton";

	/// <summary>会員番号（クロスポイント用）</summary>
	public const string SESSION_KEY_CROSSPOINT_MEMBER_ID = "memberIdForCrossPoint";
	/// <summary>PINコード（クロスポイント用）</summary>
	public const string SESSION_KEY_CROSSPOINT_PIN_CODE = "pinCodeForCrossPoint";
	/// <summary>店舗カードNO/PIN更新フラグ</summary>
	public const string SESSION_KEY_CROSS_POINT_UPDATED_SHOP_CARD_NO_AND_PIN_FLG = "updated_shop_card_and_no_pin_flg";
	/// <summary>Yahoo API でアクセストークンを更新するモール設定のモールID</summary>
	public const string SESSION_KEY_YAHOO_API_MALL_ID = "yahoo_api_mall_id";
	/// <summary>Yahoo API "State" コード (CSRF対策)</summary>
	public const string SESSION_KEY_YAHOO_API_ANTI_FORGERY_STATE_CODE = "yahoo_api_anti_forgery_state_code";

	//=========================================================================================
	// セッションパラメタキー
	//=========================================================================================
	public const string SESSIONPARAM_KEY_USER_SEARCH_INFO = "user_search_info";	// 顧客検索情報
	public const string SESSIONPARAM_KEY_USERINTEGRATION_SEARCH_INFO = "user_integration_search_info";	// ユーザー統合検索情報
	public const string SESSIONPARAM_KEY_UPLODED_MASTER = "uploded_master";		// マスタアップロード
	public const string SESSIONPARAM_KEY_OPERATOR_INFO = "operator_info";		// オペレータ情報
	public const string SESSIONPARAM_KEY_PRODUCT_INFO = "product_info";		// 商品情報
	public const string SESSIONPARAM_KEY_PRODUCT_SEARCH_INFO = "product_search_info";	// 商品検索情報
	public const string SESSIONPARAM_KEY_PRODUCTVARIATION_INFO = "productvariation_info";	// 商品バリエーション情報
	public const string SESSIONPARAM_KEY_PRODUCTMALLEXHIBITS_INFO = "productmallexhibits_info";	// モール出品設定情報
	public const string SESSIONPARAM_KEY_PRODUCTEXTEND_INFO = "productextend_info";	// 商品拡張項目情報
	public const string SESSIONPARAM_KEY_PRODUCTTAG_INFO = "producttag_info";	// 商品タグ情報
	public const string SESSIONPARAM_KEY_PRODUCTCATEGORY_INFO = "productcategory_info";// 商品カテゴリ情報
	public const string SESSIONPARAM_KEY_MEMBERRANKPRICE_INFO = "memberrankprice_info";		// 商品価格情報
	public const string SESSIONPARAM_KEY_FIXEDPURCHASE_DISCOUNT_INFO = "fixedpurchase_discount_info";		// 定期購入割引情報
	/// <summary>セッションパラメタキー：商品付帯情報</summary>
	public const string SESSIONPARAM_KEY_PRODUCTOPTIONVALUE_INTO = "productoptionvalue_info";
	public const string SESSIONPARAM_KEY_SETPROMOTION_SEARCH_INFO = "setpromotion_search_info";	// セットプロモーション検索情報
	public const string SESSIONPARAM_KEY_PRODUCTSTOCK_INFO = "productstock_info";	// 商品在庫情報
	public const string SESSIONPARAM_KEY_PRODUCTSTOCK_DATUM_INFO = "productstock_datum_info";	// 商品在庫情報(在庫基準)
	public const string SESSIONPARAM_KEY_PRODUCTREVIEW_INFO = "productreview_info";// 商品レビュー情報
	public const string SESSIONPARAM_KEY_PRODUCTSALE_INFO = "product_sale_search_info";	// 商品セール検索情報
	public const string SESSIONPARAM_KEY_PRODUCTRANKING_INFO = "productranking_info";	// 商品ランキング検索情報
	public const string SESSIONPARAM_KEY_PRODUCTRANKINGITEM_INFO = "productrankingitem_info";	// 商品ランキングアイテム検索情報
	public const string SESSIONPARAM_KEY_USERPRODUCTARRIVALMAIL_INFO = "userproductarrivalmail_search_info";	// 入荷通知メール検索情報
	public const string SESSIONPARAM_KEY_MAILTEMPLATE_INFO = "mailtemplate_info";	// メールテンプレート
	public const string SESSIONPARAM_KEY_SHIPPING_INFO = "shipping_info";		// 配送種別情報
	public const string SESSIONPARAM_KEY_SHIPPINGZONE_INFO = "shippingzone_info";	// 特別配送先情報
	public const string SESSIONPARAM_KEY_DELIVERYCOMPANY_INFO = "deliverycompany_info";	// 配送会社情報
	public const string SESSIONPARAM_KEY_DELIVERYCOMPANY_INFO_BEFORE = "deliverycompany_info_before";	// 旧配送会社情報
	public const string SESSIONPARAM_KEY_SHIPPINGDELIVERYPOSTAGE_INFO = "shippingdeliverypostage_info"; // 配送料マスタ
	public const string SESSIONPARAM_KEY_GLOBALSHIPPING_AREA_POSTAGE = "globalshipping_area_postage"; // 海外配送エリア配送料表
	public const string SESSIONPARAM_KEY_PAYMENT_INFO = "payment_info";		// 決済種別情報
	public const string SESSIONPARAM_KEY_PAYMENT_PRICE_INFO = "payment_price_info";	// 決済手数料情報
	/// <summary> 注文情報 </summary>
	public const string SESSIONPARAM_KEY_ORDER_INFO = "order_info";
	/// <summary> 注文検索情報 </summary>
	public const string SESSIONPARAM_KEY_ORDER_SEARCH_INFO = "order_search_info";
	/// <summary> 注文数 </summary>
	public const string SESSIONPARAM_KEY_ORDER_TOTAL_COUNT = "order_total_count";
	public const string SESSIONPARAM_KEY_ORDER_MODIFY_INFO = "order_modify_info";	// 注文編集情報
	public const string SESSIONPARAM_KEY_FIXEDPURCHASE_SEARCH_INFO = "fixed_search_info";	// 定期購入検索情報
	public const string SESSIONPARAM_KEY_FIXEDPURCHASE_MODIFY_INFO = "fixed_modify_info";	// 定期購入編集情報
	public const string SESSIONPARAM_KEY_MOBILEGROUP_INFO = "mobilegroup_info";		    // 機種グループ情報
	public const string SESSIONPARAM_KEY_MOBILEORIGINALTAG_INFO = "mobileoriginaltag_info";		// 機種グループ情報
	public const string SESSIONPARAM_KEY_MALLPRODUCTUPLOAD_SEARCH_INFO = "mallproductupload_search_info";	// モール商品アップロード検索情報
	public const string SESSIONPARAM_KEY_NEWS_SEARCH_INFO = "news_search_info";	// 新着情報検索情報
	public const string SESSIONPARAM_KEY_SHORTURL_SEARCH_INFO = "shorturl_search_info";	// ショートURL検索情報
	public const string SESSIONPARAM_KEY_ORDER_MEMO_INFO = "order_memo_info";	// 注文メモ情報
	public const string SESSIONPARAM_KEY_ORDER_MEMO_SEARCH_INFO = "order_memo_search_info";	// 注文メモ情報検索情報
	public const string SESSIONPARAM_KEY_ORDER_WORKFLOW_SEARCH_INFO = "order_workflow_search_info";	// ワークフロー設定検索情報
	public const string SESSIONPARAM_KEY_PRODUCTBRAND_INFO = "productbrand_info";	// ブランド情報
	public const string SESSIONPARAM_KEY_SERIALKEY_INFO = "serialkey_info";			// シリアルキー情報
	public const string SESSIONPARAM_KEY_SERIALKEY_SEARCH_INFO = "serialkey_search_info";	// シリアルキー情報検索情報
	public const string SESSIONPARAM_KEY_ORDERDETAIL_POPUP_PARENT_NAME = "orderdatail_popup_parent_name";	// 受注詳細ポップアップ呼び出し元
	public const string SESSIONPARAM_KEY_ORDERDETAIL_RELOAD_PARENT_WINDOW = "orderdetail_reload_parent_window"; // 受注詳細ポップアップが親ウインドウをリロードするかのステータス
	public const string SESSIONPARAM_KEY_REALSHOP_SEARCH_INFO = "realshop_search_info";							// リアル店舗検索情報
	public const string SESSIONPARAM_KEY_REALSHOPPRODUCTSTOCK_SEARCH_INFO = "realshopproductstock_search_info";	// リアル店舗商品在庫検索情報
	public const string SESSIONPARAM_KEY_NOVELTY_SEARCH_INFO = "novelty_search_info";							// ノベルティ検索情報
	public const string SESSIONPARAM_KEY_RECOMMEND_SEARCH_INFO = "recommend_search_info";						// レコメンド検索情報
	public const string SESSIONPARAM_KEY_AUTOTRANSLATION_WORD_SEARCH_INFO = "autotranslation_word_search_info";							// 自動翻訳検索情報
	public const string SESSIONPARAM_KEY_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_INFO = "productfixedpurchasediscountsetting_info"; // 定期購入割引設定情報
	public const string SESSIONPARAM_KEY_SHOW_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_FLG = "show_productfixedpurchasediscountsetting_flg"; // 定期購入割引設定有無フラグ
	public const string SESSIONPARAM_KEY_PRODUCTBUNDLE_SEARCH_INFO = "productbundle_search_info";	//商品同梱検索情報
	public const string SESSIONPARAM_KEY_PRODUCT_TAX_CATEGORY_INFO = "product_tax_category_info";	//商品税率カテゴリ情報
	public const string SESSIONPARAM_KEY_NAME_TRANSLATION_SETTING_SEARCH_INFO = "name_translation_setting_search_info";	// 名称翻訳設定検索情報
	public const string SESSIONPARAM_KEY_SMSTEMPLATE_INFO = "smstemplate_info";	// SMSテンプレート
	public const string SESSIONPARAM_KEY_LINETEMPLATE_INFO = "linetemplate_info";	// LINE送信内容テンプレート
	public const string SESSIONPARAM_KEY_DELIVERYLEADTIME_INFO = "deliveryleadtime_info";	// 配送リードタイム情報
	public const string SESSIONPARAM_KEY_DELIVERYCOMPANY_CHECED_EXPRESS_INFO = "deliverycompany_checed_express_info";	// 配送会社選択情報(宅配便)
	public const string SESSIONPARAM_KEY_DELIVERYCOMPANY_CHECED_MAIL_INFO = "deliverycompany_checed_mail_info";	// 配送会社選択情報(メール便)
	public const string SESSIONPARAM_KEY_TWORDERINVOICE_MODIFY_INFO = "tworderinvoice_modify_info";	// TwOrder Invoice
	public const string SESSIONPARAM_KEY_TWFIXEDPURCHASEINVOICE_MODIFY_INFO = "twfixedpurchaseinvoice_modify_info";	// TwFixedPurchase Invoice
	public const string SESSIONPARAM_KEY_ALL_CONFIGRATION = "session_configlation";
	public const string SESSIONPARAM_KEY_ALL_CONFIGRATION_LAST_UPDATED_TIMES = "session_configlation_last_update_times";
	public const string SESSIONPARAM_KEY_ALL_CONFIGRATION_SEARCH_PARAM = "session_configlation_search_param";
	/// <summary>定期商品変更設定</summary>
	public const string SESSIONPARAM_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_INFO = "fixedpurchaseproductchangesetting_info";
	//=========================================================================================
	// リクエストキー
	//=========================================================================================
	// 共通
	new public const string REQUEST_KEY_SHOP_ID = "shop_id";

	public const string REQUEST_KEY_FIRSTVIEW = "firstview";

	// w2CustomerSupport
	public const string REQUEST_KEY_MESSAGE_MEDIA_MODE = "msg_mode";	// メッセージ媒体区分
	public const string REQUEST_KEY_MESSAGE_EDIT_MODE = "editmode";		// 編集区分
	public const string REQUEST_KEY_INCIDENT_ID = "incid";	// インシデントID
	public const string REQUEST_KEY_MESSAGE_NO = "msgno";	// メッセージNO
	public const string REQUEST_KEY_MAILSENDLOG_LOG_NO = "msllno";	// メール送信ログNo

	// 共通：商品系
	new public const string REQUEST_KEY_PRODUCT_ID = "product_id";
	new public const string REQUEST_KEY_VARIATION_ID = "variation_id";
	new public const string REQUEST_KEY_SEARCH_WORD = "swrd";					// 検索ワード

	public const string REQUEST_KEY_MENUAUTHORITY_LEVEL = "mauth_level";			// メニュー権限
	public const string REQUEST_KEY_USER_KBN = "user_kbn";
	public const string REQUEST_KEY_MAIL_TEMPLATE_ID = "mail_id";
	public const string REQUEST_KEY_PRODUCT_SET_ID = "productset_id";
	public const string REQUEST_KEY_PRODUCT_SET_NAME = "productset_name";
	public const string REQUEST_KEY_STOCK_MESSAGE_ID = "stock_message_id";
	public const string REQUEST_KEY_PRODUCTCATEGORY_ID = "category_id";
	public const string REQUEST_KEY_STOCKORDER_STOCK_ORDER_ID = "psoid";
	public const string REQUEST_KEY_SHIPPING_ID = "shipping_id";
	public const string REQUEST_KEY_SHIPPING_NO = "sno";
	public const string REQUEST_KEY_SHIPPING_ZONE_NO = "shipping_zone_no";
	public const string REQUEST_KEY_DELIVERY_COMPANY_ID = "delivery_company_id";
	public const string REQUEST_KEY_PAYMENT_ID = "payment_id";
	public const string REQUEST_KEY_ORDER_ID = "order_id";
	public const string REQUEST_KEY_REORDER_ID = "reorder_id";
	public const string REQUEST_KEY_REORDER_FLG = "reorder_flg";
	public const string REQUEST_KEY_ERROR_STATUS = "error_status";
	public const string REQUEST_KEY_DISPLAY_KBN = "display_kbn";			// 表示区分
	public const string REQUEST_KEY_SELECTED_FILENAME = "selected_filename";		// (マスタアップロード、マスタ取込)
	public const string REQUEST_KEY_DELETE_OPERATOR_ID = "delete_file";			// (オペレータ管理)
	public const string REQUEST_KEY_SEARCH_KEY = "skey";					// 検索フィールド
	public const string REQUEST_KEY_ADTO_ID = "adto_id";                // 広告先ＩＤ
	public const string REQUEST_KEY_RELOAD_PARENT_WINDOW = "pnt_win_rld";			// 親画面リロード
	/// <summary>Re-order fixed purchase id</summary>
	public const string REQUEST_KEY_REORDER_FIXEDPURCHASE_ID = "reorder_fixedpurchase_id";
	/// <summary>頒布会コースIDt</summary>
	public const string REQUEST_KEY_SUBSCRIPTION_BOX_COURSE_ID = "subscription_box_course_id";
	/// <summary>頒布会注文回数FROM</summary>
	public const string REQUEST_KEY_SUBSCRIPTION_BOX_ORDER_COUNT_FROM = "subscription_box_order_count_from";
	/// <summary>頒布会注文回数TO</summary>
	public const string REQUEST_KEY_SUBSCRIPTION_BOX_ORDER_COUNT_TO = "subscription_box_order_count_to";
	/// <summary>店舗受取ステータス</summary>
	public const string REQUEST_KEY_STORE_PICKUP_STATUS = "store_pickup_status";


	public const string REQUEST_KEY_PRODUCT_SEARCH_KBN = "sk";			// 商品検索区分
	public const string REQUEST_KEY_COUPON_CODE = "couponcode";			// クーポンコード
	public const string REQUEST_KEY_COUPON_NAME = "couponname";			// クーポン名

	// ユーザー情報
	public const string REQUEST_KEY_USER_USER_ID = "ui";						// ユーザーID
	public const string REQUEST_KEY_USER_LOGIN_ID = "uli";						// ユーザーログインID
	public const string REQUEST_KEY_USER_USER_KBN = "uk";						// 顧客区分
	public const string REQUEST_KEY_USER_EASY_REGISTER_FLG = "uerf";			// かんたん会員フラグ
	public const string REQUEST_KEY_USER_NAME = "un";							// 氏名
	public const string REQUEST_KEY_USER_NAME_KANA = "unk";						// 氏名(かな)
	public const string REQUEST_KEY_USER_MAIL_FLG = "umf";						// メール配信希望
	public const string REQUEST_KEY_USER_TEL1 = "ut";							// 電話番号
	public const string REQUEST_KEY_USER_MAIL_ADDR = "uma";						// メールアドレス
	public const string REQUEST_KEY_USER_ZIP = "uz";							// 郵便番号
	public const string REQUEST_KEY_USER_ADDR = "ua";							// 住所
	public const string REQUEST_KEY_USER_COMPANY_NAME = "ucn";					// 企業名
	public const string REQUEST_KEY_USER_COMPANY_POST_NAME = "ucpn";			// 部署名
	public const string REQUEST_KEY_USER_DEL_FLG = "udf";						// 退会者
	public const string REQUEST_KEY_USER_MALL_ID = "umi";						// モールID
	public const string REQUEST_KEY_USER_USER_MANAGEMENT_LEVEL_ID = "umli";			// ユーザー管理レベルID
	public const string REQUEST_KEY_USER_USER_MANAGEMENT_LEVEL_EXCLUDE = "umle";	// 選択したユーザー管理レベルを除いて検索するか、しないか
	public const string REQUEST_KEY_USER_FIXED_PURCHASE_MEMBER_FLG = "member";		// 定期会員フラグ
	public const string REQUEST_KEY_USER_AMAZON_COOPERATED_FLG = "uacf";			// Amazonログイン連携フラグ
	public const string REQUEST_KEY_USER_AMAZON_USER_ID = "uaui";					// AmazonユーザーID
	public const string REQUEST_KEY_USER_USER_MEMO = "um";							// ユーザー メモ
	public const string REQUEST_KEY_USER_USER_MEMO_TEXT = "umt";					// ユーザー メモのテキスト
	public const string REQUEST_KEY_USER_USER_EXTEND_NAME = "uen";					// ユーザー拡張項目名
	public const string REQUEST_KEY_USER_USER_EXTEND_FLG = "uef";					// ユーザー拡張項目ありなしフラグ
	public const string REQUEST_KEY_USER_USER_EXTEND_TYPE = "uetype";				// ユーザー拡張項目タイプ
	public const string REQUEST_KEY_USER_USER_EXTEND_TEXT = "uet";					// ユーザー拡張項目の検索テキスト
	public const string REQUEST_KEY_USER_USER_MEMO_FLG = "umf";						// User Memo Flag
	public const string REQUEST_KEY_USER_INTEGRATED_FLG = "uif";						// 統合ユーザー
	public const string REQUEST_KEY_USER_DATE_CREATED_DATE_FROM = "udcdf";			// Date Create Date From
	public const string REQUEST_KEY_USER_DATE_CREATED_TIME_FROM = "udctf";			// Date Create Time From
	public const string REQUEST_KEY_USER_DATE_CREATED_DATE_TO = "udcdt";			// Date Create Date To
	public const string REQUEST_KEY_USER_DATE_CREATED_TIME_TO = "udctt";			// Date Create Time To
	public const string REQUEST_KEY_USER_DATE_CHANGED_DATE_FROM = "udedf";			// Date Changed Date From
	public const string REQUEST_KEY_USER_DATE_CHANGED_TIME_FROM = "udetf";			// Date Changed tIME From
	public const string REQUEST_KEY_USER_DATE_CHANGED_DATE_TO = "udedt";			// Date change Date To
	public const string REQUEST_KEY_USER_DATE_CHANGED_TIME_TO = "udett";			// Date change Time To
	public const string REQUEST_KEY_USER_COUNTRY_ISO_CODE = "cic";					// グローバル対応:国ISOコード

	// ユーザ拡張項目設定
	public const string REQUEST_KEY_USEREXTENDSETTING_SETTING_ID = "uexsid";	// ユーザ拡張項目設定ID

	// ユーザー統合情報
	public const string REQUEST_KEY_USERINTEGRATION_USER_INTEGRATION_NO = "uiuin";	// ユーザー統合No
	public const string REQUEST_KEY_USERINTEGRATION_STATUS = "uist";								// ステータス
	public const string REQUEST_KEY_USERINTEGRATION_USER_ID = "uiuid";							// ユーザID
	public const string REQUEST_KEY_USERINTEGRATION_NAME = "uin";								// 氏名
	public const string REQUEST_KEY_USERINTEGRATION_NAME_KANA = "uink";						// 氏名(かな)
	public const string REQUEST_KEY_USERINTEGRATION_TEL = "uit";									// 電話番号
	public const string REQUEST_KEY_USERINTEGRATION_MAIL_ADDR = "uima";						// メールアドレス
	public const string REQUEST_KEY_USERINTEGRATION_ZIP = "uiz";									// 郵便番号
	public const string REQUEST_KEY_USERINTEGRATION_ADDR = "uia";									// 住所
	public const string REQUEST_KEY_USERINTEGRATIONHISTORY_TABLE_NAME = "uihtn";		// テーブル名

	// 商品情報
	public const string REQUEST_KEY_PRODUCT_PRODUCT_ID = "pi";						// 商品ID
	public const string REQUEST_KEY_PRODUCT_NAME = "pn";						// 商品名
	public const string REQUEST_KEY_PRODUCT_SUPPLIER_ID = "psi";					// サプライヤID
	public const string REQUEST_KEY_PRODUCT_COOPERATION_ID_HEAD = "pci";	// 商品連携ID頭文字
	public const string REQUEST_KEY_PRODUCT_COOPERATION_ID1 = "pci1";					// 商品連携ID1
	public const string REQUEST_KEY_PRODUCT_COOPERATION_ID2 = "pci2";					// 商品連携ID2
	public const string REQUEST_KEY_PRODUCT_COOPERATION_ID3 = "pci3";					// 商品連携ID3
	public const string REQUEST_KEY_PRODUCT_COOPERATION_ID4 = "pci4";					// 商品連携ID4
	public const string REQUEST_KEY_PRODUCT_COOPERATION_ID5 = "pci5";					// 商品連携ID5
	public const string REQUEST_KEY_PRODUCT_SHIPPING_TYPE = "pst";					// 配送種別
	public const string REQUEST_KEY_PRODUCT_SHIPPING_SIZE_KBN = "pssk";					// 配送サイズ区分
	public const string REQUEST_KEY_PRODUCT_DISPLAY_KBN = "pdk";					// 表示期間
	public const string REQUEST_KEY_PRODUCT_SELL_KBN = "psk";					// 販売期間
	public const string REQUEST_KEY_PRODUCT_SEARCH_DISPLAY_KBN = "sdk";	// 商品表示区分（検索時表示区分）
	public const string REQUEST_KEY_PRODUCT_VALID_FLG = "pvf";					// 有効フラグ
	public const string REQUEST_KEY_PRODUCT_CATEGORY_ID = "pci";					// カテゴリID
	public const string REQUEST_KEY_PRODUCT_MOBILE_DISP_FLG = "mdf";					// モバイル表示フラグ
	public const string REQUEST_KEY_PRODUCT_ICON_FLG1 = "pif1";					// アイコン1
	public const string REQUEST_KEY_PRODUCT_ICON_FLG2 = "pif2";					// アイコン2
	public const string REQUEST_KEY_PRODUCT_ICON_FLG3 = "pif3";					// アイコン3
	public const string REQUEST_KEY_PRODUCT_ICON_FLG4 = "pif4";					// アイコン4
	public const string REQUEST_KEY_PRODUCT_ICON_FLG5 = "pif5";					// アイコン5
	public const string REQUEST_KEY_PRODUCT_ICON_FLG6 = "pif6";					// アイコン6
	public const string REQUEST_KEY_PRODUCT_ICON_FLG7 = "pif7";					// アイコン7
	public const string REQUEST_KEY_PRODUCT_ICON_FLG8 = "pif8";					// アイコン8
	public const string REQUEST_KEY_PRODUCT_ICON_FLG9 = "pif9";					// アイコン9
	public const string REQUEST_KEY_PRODUCT_ICON_FLG10 = "pif10";				// アイコン10
	public const string REQUEST_KEY_PRODUCT_MEMBER_RANK_DISCOUNT_FLG = "mrdcf";	// 会員ランク割引対象フラグ
	public const string REQUEST_KEY_PRODUCT_DISP_MEMBER_RANK = "mrdmr";			// 閲覧可能会員ランク
	public const string REQUEST_KEY_PRODUCT_BUYABLE_MEMBER_RANK = "mrbmr";		// 購入可能会員ランク
	public const string REQUEST_KEY_PRODUCT_BRAND_ID = "bid";					// ブランドID
	public const string REQUEST_KEY_PRODUCT_DISPLAY_PRIORITY = "dp";					// 表示優先順
	public const string REQUEST_KEY_PRODUCT_SELL_FROM_DATE_YEAR_FROM = "psfdyf";		// 販売開始日・年(From)
	public const string REQUEST_KEY_PRODUCT_SELL_FROM_DATE_MONTH_FROM = "psfdmf";		// 販売開始日・月(From)
	public const string REQUEST_KEY_PRODUCT_SELL_FROM_DATE_DAY_FROM = "psfddf";			// 販売開始日・日(From)
	public const string REQUEST_KEY_PRODUCT_SELL_FROM_DATE_YEAR_TO = "psfdyt";			// 販売開始日・年(To)
	public const string REQUEST_KEY_PRODUCT_SELL_FROM_DATE_MONTH_TO = "psfdmt";			// 販売時期日・月(To)
	public const string REQUEST_KEY_PRODUCT_SELL_FROM_DATE_DAY_TO = "psfddt";			// 販売開始日・日(To)
	public const string REQUEST_KEY_PRODUCT_SELL_TO_DATE_YEAR_FROM = "pstdyf";			// 販売終了日・年(From)
	public const string REQUEST_KEY_PRODUCT_SELL_TO_DATE_MONTH_FROM = "pstdmf";			// 販売終了日・月(From)
	public const string REQUEST_KEY_PRODUCT_SELL_TO_DATE_DAY_FROM = "pstddf";			// 販売終了日・日(From)
	public const string REQUEST_KEY_PRODUCT_SELL_TO_DATE_YEAR_TO = "pstdyt";			// 販売終了日・年(To)
	public const string REQUEST_KEY_PRODUCT_SELL_TO_DATE_MONTH_TO = "pstdmt";			// 販売終了日・月(To)
	public const string REQUEST_KEY_PRODUCT_SELL_TO_DATE_DAY_TO = "pstddt";				// 販売終了日・日(To)
	public const string REQUEST_KEY_PRODUCT_SELL_FROM_DATE_HOUR_FROM = "psfdhof";		// 販売開始日・時(From)
	public const string REQUEST_KEY_PRODUCT_SELL_FROM_DATE_MINUTE_FROM = "psfdmif";		// 販売開始日・分(From)
	public const string REQUEST_KEY_PRODUCT_SELL_FROM_DATE_SECOND_FROM = "psfdsef";		// 販売開始日・秒(From)
	public const string REQUEST_KEY_PRODUCT_SELL_FROM_DATE_HOUR_TO = "psfdhot";			// 販売開始日・時(To)
	public const string REQUEST_KEY_PRODUCT_SELL_FROM_DATE_MINUTE_TO = "psfdmit";		// 販売時期日・分(To)
	public const string REQUEST_KEY_PRODUCT_SELL_FROM_DATE_SECOND_TO = "psfdset";		// 販売開始日・秒(To)
	public const string REQUEST_KEY_PRODUCT_SELL_TO_DATE_HOUR_FROM = "pstdhof";			// 販売終了日・時(From)
	public const string REQUEST_KEY_PRODUCT_SELL_TO_DATE_MINUTE_FROM = "pstdmif";		// 販売終了日・分(From)
	public const string REQUEST_KEY_PRODUCT_SELL_TO_DATE_SECOND_FROM = "pstdsef";		// 販売終了日・秒(From)
	public const string REQUEST_KEY_PRODUCT_SELL_TO_DATE_HOUR_TO = "pstdhot";			// 販売終了日・時(To)
	public const string REQUEST_KEY_PRODUCT_SELL_TO_DATE_MINUTE_TO = "pstdmit";			// 販売終了日・分(To)
	public const string REQUEST_KEY_PRODUCT_SELL_TO_DATE_SECOND_TO = "pstdset";			// 販売終了日・秒(To)
	public const string REQUEST_KEY_PRODUCT_FIXEDPURCHASE_PRODUCT = "pfpp";		// 定期購入商品
	public const string REQUEST_KEY_PRODUCT_SHIPPING_TYPE_PRODUCT_IDS = "pstkpids";		// 配送料種別取得用商品IDリスト
	public const string REQUEST_KEY_PRODUCT_LIMITED_PAYMENT = "plp";					// Product Limited Payment
	public const string REQUEST_KEY_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE = "pbidt";				// 同梱商品明細表示フラグ
	public const string REQUEST_KEY_PRODUCT_PRODUCT_TYPE = "ppt";				// 商品区分
	public const string REQUEST_KEY_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG = "pdofpmf";	// 定期会員限定フラグ（閲覧）
	public const string REQUEST_KEY_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG = "psofpmf";	// 定期会員限定フラグ（販売）
	public const string REQUEST_KEY_PRODUCT_SELL_FROM_DATE_FROM = "prosfdf";			// Product sell date from (From)
	public const string REQUEST_KEY_PRODUCT_SELL_FROM_DATE_TO = "prosfdt";				// Product sell date to (From)
	public const string REQUEST_KEY_PRODUCT_SELL_FROM_TIME_FROM = "prosftf";			// Product sell time from (From)
	public const string REQUEST_KEY_PRODUCT_SELL_FROM_TIME_TO = "prosftt";				// Product sell time to (From)
	public const string REQUEST_KEY_PRODUCT_SELL_TO_DATE_FROM = "prostdf";				// Product sell date from (To)
	public const string REQUEST_KEY_PRODUCT_SELL_TO_DATE_TO = "prostdt";				// Product sell date to (To)
	public const string REQUEST_KEY_PRODUCT_SELL_TO_TIME_FROM = "prosttf";				// Product sell time from (To)
	public const string REQUEST_KEY_PRODUCT_SELL_TO_TIME_TO = "prosttt";				// Product sell time to (To)
	/// <summary>Product fixed purchase flag</summary>
	public const string REQUEST_KEY_PRODUCT_FIXED_PURCHASE_FLG = "pfpf";
	/// <summary>Product subcription box</summary>
	public const string REQUEST_KEY_PRODUCT_SUBSCRIPTION_BOX_FLG = "psbf";
	/// <summary>Product subscription box id</summary>
	public const string REQUEST_KEY_PRODUCT_SUBSCRIPTION_BOX_ID = "psbid";
	// 商品カテゴリ情報
	public const string REQUEST_KEY_PRODUCTCATEGORY_CATEGORY_ID = "pci";	// カテゴリID
	public const string REQUEST_KEY_PRODUCTCATEGORY_NAME = "pcn";			// カテゴリ名

	// 商品レビュー情報
	public const string REQUEST_KEY_PRODUCTREVIEW_SHOP_ID = "prsi";				// 店舗ID
	public const string REQUEST_KEY_PRODUCTREVIEW_PRODUCT_ID = "prpi";				// 商品ID
	public const string REQUEST_KEY_PRODUCTREVIEW_REVIEW_NO = "prno";				// 番号
	public const string REQUEST_KEY_PRODUCTREVIEW_USER_ID = "prui";				// ユーザID
	public const string REQUEST_KEY_PRODUCTREVIEW_NICK_NAME = "prnn";				// ニックネーム
	public const string REQUEST_KEY_PRODUCTREVIEW_REVIEW_TITLE = "prrt";				// タイトル
	public const string REQUEST_KEY_PRODUCTREVIEW_REVIEW_COMMENT = "prrc";				// コメント
	public const string REQUEST_KEY_PRODUCTREVIEW_OPEN_FLG = "prof";				// 公開フラグ
	public const string REQUEST_KEY_PRODUCTREVIEW_CHECKED_FLG = "prcf";				// 管理者チェックフラグ

	// 商品在庫情報
	public const string REQUEST_KEY_STOCK_ALERT_KBN = "alk";					// 商品在庫区分
	public const string REQUEST_KEY_SEARCH_STOCK_KEY = "sskey";					// 在庫数検索(○在庫)
	public const string REQUEST_KEY_SEARCH_STOCK_COUNT = "sscnt";				// 在庫数検索(在庫数)
	public const string REQUEST_KEY_SEARCH_STOCK_COUNT_TYPE = "sscntt";			// 以上、等しい、以下

	// 商品セール系
	public const string REQUEST_KEY_PRODUCTSALE_PRODUCTSALE_KBN = "pskbn";			// 商品セール区分
	public const string REQUEST_KEY_PRODUCTSALE_PRODUCTSALE_ID = "psid";			// 商品セール区分
	public const string REQUEST_KEY_PRODUCTSALE_PRODUCTSALE_INFO = "psinfo";			// セール情報
	public const string REQUEST_KEY_PRODUCTSALE_PRODUCTSALE_OPENED = "psopn";			// セール開催状態

	// セットプロモーション系
	public const string REQUEST_KEY_SETPROMOTION_SETPROMOTION_ID = "spid";			// セットプロモーションID
	public const string REQUEST_KEY_SETPROMOTION_SETPROMOTION_NAME = "spn";			// セットプロモーション名
	public const string REQUEST_KEY_SETPROMOTION_PRODUCT_ID = "sppid";				// 商品ID
	public const string REQUEST_KEY_SETPROMOTION_CATEGORY_ID = "spcid";				// カテゴリID
	public const string REQUEST_KEY_SETPROMOTION_STATUS = "spst";					// 開催状態
	public const string REQUEST_KEY_SETPROMOTION_BEGIN_DATE_FROM = "spbdf";			// Request key setpromotion begin date from
	public const string REQUEST_KEY_SETPROMOTION_BEGIN_DATE_TO = "spbdt";			// Request key setpromotion begin date to
	public const string REQUEST_KEY_SETPROMOTION_BEGIN_TIME_FROM = "spbtf";			// Request key setpromotion begin time from
	public const string REQUEST_KEY_SETPROMOTION_BEGIN_TIME_TO = "spbtt";			// Request key setpromotion begin time to
	public const string REQUEST_KEY_SETPROMOTION_END_DATE_FROM = "spedf";			// Request key setpromotion end date from
	public const string REQUEST_KEY_SETPROMOTION_END_DATE_TO = "spedt";				// Request key setpromotion end date to
	public const string REQUEST_KEY_SETPROMOTION_END_TIME_FROM = "spetf";			// Request key setpromotion end time from
	public const string REQUEST_KEY_SETPROMOTION_END_TIME_TO = "spett";				// Request key setpromotion end time to

	// 商品ランキング系
	public const string REQUEST_KEY_PRODUCTRANKING_RANKING_ID = "pri";					// 商品ID
	public const string REQUEST_KEY_PRODUCTRANKING_TOTAL_TYPE = "ptt";					// 集計タイプ
	public const string REQUEST_KEY_PRODUCTRANKING_VALID_FLG = "pvf";					// 有効フラグ

	// ブランド系
	public const string REQUEST_KEY_PRODUCTBRAND_BRAND_ID = "bid";						// ブランドID
	public const string REQUEST_KEY_PRODUCTBRAND_BRAND_NAME = "bn";					// ブランド名

	// シリアルキー系
	public const string REQUEST_KEY_SERIALKEY_SERIAL_KEY = "skey"; // シリアルキー
	public const string REQUEST_KEY_SERIALKEY_PRODUCT_ID = "spi"; // 商品ID
	public const string REQUEST_KEY_SERIALKEY_USER_ID = "sui"; // ユーザID
	public const string REQUEST_KEY_SERIALKEY_ORDER_ID = "soi"; // 注文ID
	public const string REQUEST_KEY_SERIALKEY_STATUS = "stat"; // 状態
	public const string REQUEST_KEY_SERIALKEY_VALID_FLG = "svf"; // 有効フラグ

	// 入荷通知メール系
	public const string REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_ID = "upampi";						// 商品ID
	public const string REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_NAME = "upampn";						// 商品名
	public const string REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_STOCK_COUNT = "upampsc";				// 在庫数
	public const string REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DATE_DATE_FROM = "psfddf";	// Product Sell From Date Date From
	public const string REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DATE_TIME_FROM = "psfdtf";	// Product Sell From Date Time From
	public const string REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DATE_DATE_TO = "psfddt";	// Product Sell From Date Date To
	public const string REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_FROM_DATE_TIME_TO = "psfdtt";	// Product Sell From Date Time To
	public const string REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_TO_DATE_DATE_FROM = "pstddf";	// Product Sell From Date Date From
	public const string REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_TO_DATE_TIME_FROM = "pstdtf";	// Product Sell From Date Time From
	public const string REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_TO_DATE_DATE_TO = "pstddt";		// Product Sell From Date Date To
	public const string REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SELL_TO_DATE_TIME_TO = "pstdtt";		// Product Sell From Date Time To
	public const string REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_SALES_PERIOD = "psp";				// Product Sales Period
	public const string REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_DISPLAY_PERIOD = "pdp";				// Product Display Period
	public const string REQUEST_KEY_USERPRODUCTARRIVALMAIL_PRODUCT_VALID_FLG = "pvf";					// 商品有効フラグ
	public const string REQUEST_KEY_USERPRODUCTARRIVALMAIL_SEARCH_ARRIVAL_MAIL = "upamarv";				// 再入荷通知
	public const string REQUEST_KEY_USERPRODUCTARRIVALMAIL_SEARCH_RELEASE_MAIL = "upamrel";				// 販売開始通知
	public const string REQUEST_KEY_USERPRODUCTARRIVALMAIL_SEARCH_RESALE_MAIL = "upamres";				// 再販売通知
	public const string REQUEST_KEY_USERPRODUCTARRIVALMAIL_MAIL_SEND = "send";							// 送信処理

	// ノベルティ設定
	public const string REQUEST_KEY_NOVELTY_ID = "novid";						// ノベルティID
	public const string REQUEST_KEY_NOVELTY_DISP_NAME = "novdn";			// ノベルティ名(表示用)
	public const string REQUEST_KEY_NOVELTY_NAME = "novn";					// ノベルティ名(管理用)
	public const string REQUEST_KEY_NOVELTY_STATUS = "status";				// 開催状態

	// レコメンド設定
	public const string REQUEST_KEY_RECOMMEND_ID = "rcdid";					// レコメンドID
	public const string REQUEST_KEY_RECOMMEND_NAME = "rcdn";				// レコメンド名(管理用)
	public const string REQUEST_KEY_RECOMMEND_KBN = "rckbn";				// レコメンド区分
	public const string REQUEST_KEY_RECOMMEND_STATUS = "status";			// 開催状態
	public const string REQUEST_KEY_TEMP_RECOMMEND_ID = "trcdid";			// レコメンドID（※ボタン画像保存用）
	public const string REQUEST_KEY_RECOMMEND_BUTTONIMAGE_TYPE = "rcimgtp";	// ボタン画像種別
	public const string REQUEST_KEY_RECOMMENDITEM_FIXED_PURCHASE_KBN = "rifpk"; // 定期購入区分
	public const string REQUEST_KEY_RECOMMENDITEM_FIXED_PURCHASE_SETTING1 = "rifps1"; // 定期購入設定1

	// 注文情報
	public const string REQUEST_KEY_ORDER_USER_ID = "oui";			// ユーザーID
	public const string REQUEST_KEY_ORDER_OWNER_NAME = "on";				// 注文者名
	public const string REQUEST_KEY_ORDER_OWNER_NAME_KANA = "onk";			// 注文者名（かな）
	public const string REQUEST_KEY_ORDER_OWNER_MAIL_ADDR = "oma";			// メールアドレス
	public const string REQUEST_KEY_ORDER_MEMBER_RANK_ID = "omri";			// 会員ランクID
	public const string REQUEST_KEY_ORDER_OWNER_ZIP = "oz";			// 注文者郵便番号
	public const string REQUEST_KEY_ORDER_OWNER_ADDR = "oa";			// 注文者住所
	public const string REQUEST_KEY_ORDER_OWNER_TEL = "ot";			// 注文者電話番号
	public const string REQUEST_KEY_ORDER_OWNER_KBN = "ok";				// 注文者区分
	public const string REQUEST_KEY_ORDER_UPDATE_DATE_YEAR_FROM = "oduyf";			// 注文年(From)
	public const string REQUEST_KEY_ORDER_UPDATE_DATE_MONTH_FROM = "odumf";			// 注文月(From)
	public const string REQUEST_KEY_ORDER_UPDATE_DATE_DAY_FROM = "odudf";			// 注文日(From)
	public const string REQUEST_KEY_ORDER_UPDATE_DATE_YEAR_TO = "oduyt";			// 注文年(To)
	public const string REQUEST_KEY_ORDER_UPDATE_DATE_MONTH_TO = "odumt";			// 注文月(To)
	public const string REQUEST_KEY_ORDER_UPDATE_DATE_DAY_TO = "odudt";			// 注文日(To)
	public const string REQUEST_KEY_ORDER_ORDER_PAYMENT_STATUS = "ops";			// 入金ステータス
	public const string REQUEST_KEY_ORDER_DEMAND_STATUS = "ods";			// 督促ステータス
	public const string REQUEST_KEY_ORDER_EXTEND_STATUS_NO = "oesn";			// 拡張ステータス枝番
	public const string REQUEST_KEY_ORDER_EXTEND_STATUS = "oes";			// 拡張ステータス
	public const string REQUEST_KEY_ORDER_UPDATE_DATE_EXTEND_STATUS_NO = "oduesn";			// 拡張ステータス更新日・ステータス
	public const string REQUEST_KEY_ORDER_EXTEND_STATUS_UPDATE_DATE_YEAR_FROM = "oesuyf";			// 更新年(From)
	public const string REQUEST_KEY_ORDER_EXTEND_STATUS_UPDATE_DATE_MONTH_FROM = "oesumf";			// 更新月(From)
	public const string REQUEST_KEY_ORDER_EXTEND_STATUS_UPDATE_DATE_DAY_FROM = "oesudf";			// 更新日(From)
	public const string REQUEST_KEY_ORDER_EXTEND_STATUS_UPDATE_DATE_YEAR_TO = "oesuyt";				// 更新年(To)
	public const string REQUEST_KEY_ORDER_EXTEND_STATUS_UPDATE_DATE_MONTH_TO = "oesumt";			// 更新月(To)
	public const string REQUEST_KEY_ORDER_EXTEND_STATUS_UPDATE_DATE_DAY_TO = "oesudt";				// 更新日(To)
	public const string REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_STATUS = "odexps";		// 外部決済ステータス
	public const string REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_DATE_YEAR_FROM = "odexpayf";	// 最終与信年(From)
	public const string REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_DATE_MONTH_FROM = "odexpamf";	// 最終与信月(From)
	public const string REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_DATE_DAY_FROM = "odexpadf";		// 最終与信日(From)
	public const string REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_DATE_YEAR_TO = "odexpayt";		// 最終与信年(To)
	public const string REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_DATE_MONTH_TO = "odexpamt";		// 最終与信月(To)
	public const string REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_DATE_DAY_TO = "odexpadt";		// 最終与信日(To)
	public const string REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_DATE_NONE = "odexpadn";			// 最終与信日指定無し
	public const string REQUEST_KEY_ORDER_ORDER_SHIPPING_KBN = "oskn";			// 配送区分
	public const string REQUEST_KEY_ORDER_ORDER_STOCKRESERVED_STATUS = "osrs";			// 在庫引当ステータス
	public const string REQUEST_KEY_ORDER_ORDER_SHIPPED_STATUS = "oss";			// 出荷ステータス
	public const string REQUEST_KEY_ORDER_ORDER_SHIPPING_CHECK_NO = "oscno";			// 注文伝票番号
	public const string REQUEST_KEY_ORDER_SHIPPED_CHANGED_KBN = "osck";			// 出荷後変更区分
	public const string REQUEST_KEY_ORDER_RETURN_EXCHANGE = "ore";			// 返品交換注文
	public const string REQUEST_KEY_ORDER_RETURN_EXCHANGE_KBN = "orek";			// 返品交換区分
	public const string REQUEST_KEY_ORDER_RETURN_EXCHANGE_REASON_KBN = "orerk";			// 返品交換都合区分
	public const string REQUEST_KEY_ORDER_ORDER_RETURN_EXCHANGE_STATUS = "ores";			// 返品交換ステータス
	public const string REQUEST_KEY_ORDER_ORDER_REPAYMENT_STATUS = "orps";			// 返金ステータス
	public const string REQUEST_KEY_ORDER_FIXEDPURCHASE_ID = "ofid";				// 定期購買注文
	public const string REQUEST_KEY_ORDER_FIXEDPURCHASE_ORDER_COUNT_FROM = "ofocf";		// 定期購入回数(注文時点)From
	public const string REQUEST_KEY_ORDER_FIXEDPURCHASE_ORDER_COUNT_TO = "ofoct";			// 定期購入回数(注文時点)To
	public const string REQUEST_KEY_ORDER_FIXEDPURCHASE_SHIPPED_COUNT_FROM = "ofscf";		// 定期購入回数(出荷時点)From
	public const string REQUEST_KEY_ORDER_FIXEDPURCHASE_SHIPPED_COUNT_TO = "ofsct";			// 定期購入回数(出荷時点)To
	public const string REQUEST_KEY_ORDER_MEMO = "omm";								// 注文メモ
	public const string REQUEST_KEY_ORDER_PAYMENT_MEMO = "opmm";					// 決済連携メモ
	public const string REQUEST_KEY_ORDER_MANAGEMENT_MEMO = "ommm";					// 管理メモ
	public const string REQUEST_KEY_ORDER_SHIPPING_MEMO = "osmm";					// 配送メモ
	public const string REQUEST_KEY_ORDER_RELATION_MEMO = "ormm";					// 外部連携メモ
	public const string REQUEST_KEY_ORDER_MEMO_FLG = "ommf";							// 注文メモ
	public const string REQUEST_KEY_ORDER_MANAGEMENT_MEMO_FLG = "ommmf";				// 管理メモ
	public const string REQUEST_KEY_ORDER_SHIPPING_MEMO_FLG = "osmmf";					// 配送メモフラグ
	public const string REQUEST_KEY_ORDER_PAYMENT_MEMO_FLG = "opmmf";					// 決済連携メモ
	public const string REQUEST_KEY_ORDER_RELATION_MEMO_FLG = "ormmf";					// 外部連携メモ
	public const string REQUEST_KEY_ORDERITEM_PRODUCT_OPTION_FLG = "oppf"; //商品付帯情報フラグ
	public const string REQUEST_KEY_ORDERITEM_PRODUCT_OPTION_TEXTS = "oppt"; //商品付帯情報フラグ
	public const string REQUEST_KEY_ORDER_GIFT_FLG = "gftf";						// ギフト購入フラグ
	public const string REQUEST_KEY_ORDER_DIGITAL_CONTENTS_FLG = "dcntf"; // デジタルコンテンツ商品フラグ
	public const string REQUEST_KEY_ORDER_SETPROMOTION_ID = "ospid";		// セットプロモーションID
	public const string REQUEST_KEY_ORDER_NOVELTY_ID = "onovid";				// ノベルティID
	public const string REQUEST_KEY_ORDER_RECOMMEND_ID = "orcdid";				// レコメンドID
	/// <summary>注文ステータス更新済み</summary>
	public const string REQUEST_KEY_ORDER_STATUS_UPDATED = "osu";

	/// <summary>決済注文ID</summary>
	public const string REQUEST_KEY_ORDER_PAYMENT_ORDER_ID = "poid";
	/// <summary>決済取引ID</summary>
	public const string REQUEST_KEY_ORDER_CARD_TRAN_ID = "ctid";

	public const string REQUEST_KEY_ORDER_COMPANY_NAME = "cn"; // 企業名
	public const string REQUEST_KEY_ORDER_SEPARATE_ESTIMATES_FLG = "sef"; // 配送料の別見積もりフラグ
	public const string REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_DATE_YEAR_FROM = "ossdyf";		// 出荷予定日・年(From)
	public const string REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_DATE_MONTH_FROM = "ossdmf";	// 出荷予定日・月(From)
	public const string REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_DATE_DAY_FROM = "ossddf";		// 出荷予定日・日(From)
	public const string REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_DATE_YEAR_TO = "ossdyt";		// 出荷予定日・年(To)
	public const string REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_DATE_MONTH_TO = "ossdmt";		// 出荷予定日・月(To)
	public const string REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_DATE_DAY_TO = "ossddt";		// 出荷予定日・日(To)
	public const string REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_DATE_NONE = "ossdn";	// 指定なし含む
	public const string REQUEST_KEY_ORDER_SHIPPING_DATE_YEAR_FROM = "osdyf";		// 配送希望日・年(From)
	public const string REQUEST_KEY_ORDER_SHIPPING_DATE_MONTH_FROM = "osdmf";		// 配送希望日・月(From)
	public const string REQUEST_KEY_ORDER_SHIPPING_DATE_DAY_FROM = "osddf";			// 配送希望日・日(From)
	public const string REQUEST_KEY_ORDER_SHIPPING_DATE_YEAR_TO = "osdyt";			// 配送希望日・年(To)
	public const string REQUEST_KEY_ORDER_SHIPPING_DATE_MONTH_TO = "osdmt";			// 配送希望日・月(To)
	public const string REQUEST_KEY_ORDER_SHIPPING_DATE_DAY_TO = "osddt";			// 配送希望日・日(To)
	public const string REQUEST_KEY_ORDER_SHIPPING_DATE_NONE = "osdn";			// 指定なし含む
	public const string REQUEST_KEY_ORDER_ANOTHER_SHIPPING_FLAG = "osasf";		// 別出荷フラグ
	public const string REQUEST_KEY_ORDER_SHIPPING_NAME = "ospn";				// 配送者名
	public const string REQUEST_KEY_ORDER_SHIPPING_NAME_KANA = "ospnk";			// 配送者名（かな）
	public const string REQUEST_KEY_ORDER_SHIPPING_ZIP = "ospz";				// 配送先郵便番号
	public const string REQUEST_KEY_ORDER_SHIPPING_ADDR = "ospa";				// 配送先住所
	public const string REQUEST_KEY_ORDER_SHIPPING_TEL1 = "ospt1";				// 配送者電話番号
	public const string REQUEST_KEY_ORDER_ADVCODE_FLG = "oacf";					// Order AdvCode Flag
	public const string REQUEST_KEY_ORDER_ADVCODE = "oac";						// Order AdvCode
	public const string REQUEST_KEY_ORDERITEM_PRODUCT_BUNDLE_ID = "pbid";	// 商品同梱ID
	public const string REQUEST_KEY_ORDER_EXTERNAL_ORDER_ID = "oeoi";	// 外部連携受注ID
	public const string REQUEST_KEY_ORDER_EXTERNAL_IMPORT_STATUS = "oeis";	// 外部連携取込ステータス
	public const string REQUEST_KEY_ORDER_MALL_LINK_STATUS = "mlks";	// モール連携ステータス
	public const string REQUEST_KEY_ORDER_SHIPPING_METHOD = "osm";		// 配送方法
	public const string REQUEST_KEY_ORDER_ORDER_COUNT_FROM = "oocf";		// 購入回数(注文時点)From
	public const string REQUEST_KEY_ORDER_ORDER_COUNT_TO = "ooct";			// 購入回数(注文時点)To
	public const string REQUEST_KEY_ORDER_RECEIPT_FLG = "orf";    // 領収書希望フラグ
	public const string REQUEST_KEY_ORDER_INVOICE_BUNDLE_FLG = "oibf";	// 請求書同梱フラグ
	public const string REQUEST_KEY_ORDER_SHIPPING_STATUS = "osss";		// 配送状態
	public const string REQUEST_KEY_ORDER_SHIPPING_PREFECTURE = "osp";	// 配送先都道府県
	public const string REQUEST_KEY_ORDER_ORDER_EXTEND_NAME = "oen";	// 注文拡張項目 項目名
	public const string REQUEST_KEY_ORDER_ORDER_EXTEND_FLG = "oef";		// 注文拡張項目 内容有無
	public const string REQUEST_KEY_ORDER_ORDER_EXTEND_TYPE = "oety";	// 注文拡張項目 選択方法
	public const string REQUEST_KEY_ORDER_ORDER_EXTEND_TEXT = "oet";	// 注文拡張項目 内容
	public const string REQUEST_KEY_ORDER_SHIPPING_DATE_FROM = "osdf";	// Request key order shipping date from
	public const string REQUEST_KEY_ORDER_SHIPPING_DATE_TO = "osdt";	// Request key order shipping date to
	public const string REQUEST_KEY_ORDER_SHIPPING_TIME_FROM = "ostf";	// Request key order shipping time from
	public const string REQUEST_KEY_ORDER_SHIPPING_TIME_TO = "ostt";	// Request key order shipping time to
	public const string REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_DATE_FROM = "ossdf";	// Request key order schedule shipping date from
	public const string REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_DATE_TO = "ossdt";		// Request key order schedule shipping date to
	public const string REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_TIME_FROM = "osstf";	// Request key order schedule shipping time from
	public const string REQUEST_KEY_ORDER_SCHEDULED_SHIPPING_TIME_TO = "osstt";		// Request key order schedule shipping time to
	public const string REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_DATE_FROM = "odexpadf";	// Request key order external payment auth date from
	public const string REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_DATE_TO = "odexpadt";	// Request key order external payment auth date to
	public const string REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_TIME_FROM = "odexpatf";	// Request key order external payment auth time from
	public const string REQUEST_KEY_ORDER_EXTERNAL_PAYMENT_AUTH_TIME_TO = "odexpatt";	// Request key order external payment auth time to
	public const string REQUEST_KEY_ORDER_EXTEND_STATUS_UPDATE_DATE_FROM = "oesudf";	// Request key order extend status update date from
	public const string REQUEST_KEY_ORDER_EXTEND_STATUS_UPDATE_DATE_TO = "oesudt";		// Request key order extend status update date to
	public const string REQUEST_KEY_ORDER_EXTEND_STATUS_UPDATE_TIME_FROM = "oesutf";	// Request key order extend status update time from
	public const string REQUEST_KEY_ORDER_EXTEND_STATUS_UPDATE_TIME_TO = "oesutt";		// Request key order extend status update time to
	public const string REQUEST_KEY_ORDER_SHIPPING_STATUS_CODE = "ossc";	// 完了状態コード
	public const string REQUEST_KEY_ORDER_SHIPPING_CURRENT_STATUS = "oscs";	// 現在の状態

	public const string REQUEST_KEY_REATURN_EXCHANGE_REPAYMENT_UPDATE_DATE_STATUS = "rerdus";				// 返品交換返金更新日
	public const string REQUEST_KEY_RETURN_EXCHANGE_REPAYMENT_UPDATE_DATE_DATE_FROM = "rerdudf";			// Request Key Return Exchange RePayment Update Date Date From
	public const string REQUEST_KEY_RETURN_EXCHANGE_REPAYMENT_UPDATE_DATE_TIME_FROM = "rerdutf";			// Request Key Return Exchange RePayment Update Date Time From
	public const string REQUEST_KEY_RETURN_EXCHANGE_REPAYMENT_UPDATE_DATE_DATE_TO = "rerdudt";				// Request Key Return Exchange RePayment Update Date Date To
	public const string REQUEST_KEY_RETURN_EXCHANGE_REPAYMENT_UPDATE_DATE_TIME_TO = "rerdutt";				// Request Key Return Exchange RePayment Update Date Time to

	// Taiwan Order Invoice
	public const string REQUEST_KEY_TW_INVOICE_NO = "twin";			// TwInvoice No
	public const string REQUEST_KEY_TW_INVOICE_STATUS = "twis";		// TwInvoice Status

	public const string REQUEST_KEY_ORDERWORKFLOWSCENARIOSETTING_ORDERWORKFLOW_SCENARIOSETTING_ID = "owssssid";	// シナリオ設定ID

	public const string REQUEST_KEY_ORDERWORKFLOWEXECHISTORY_SHOP_ID = "owehsid";	// ショップID
	public const string REQUEST_KEY_ORDERWORKFLOWEXECHISTORY_WORKFLOW_KBN = "owehwkbn";	// ワークフロー区分
	public const string REQUEST_KEY_ORDERWORKFLOWEXECHISTORY_WORKFLOW_NO = "owehwn";	//ワークフローNo
	public const string REQUEST_KEY_ORDERWORKFLOWEXECHISTORY_SCENARIO_SETTING_ID = "owehssid";	//シナリオ設定ID
	public const string REQUEST_KEY_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS = "owehes";	// 実行ステータス
	public const string REQUEST_KEY_ORDERWORKFLOWEXECHISTORY_EXEC_PLACE = "owehep";	// 実行起点
	public const string REQUEST_KEY_ORDERWORKFLOWEXECHISTORY_EXEC_TIMING = "owehet";	// 実行タイミング
	public const string REQUEST_KEY_ORDERWORKFLOWEXECHISTORY_WORKFLOW_TYPE = "owehwt";	// ワークフロータイプ
	public const string REQUEST_KEY_ORDERWORKFLOWEXECHISTORY_DATE_FROM = "owehdf";	// Request key order workflow exec history date from
	public const string REQUEST_KEY_ORDERWORKFLOWEXECHISTORY_TIME_FROM = "owehdtf";	// Request key order workflow exec history time from
	public const string REQUEST_KEY_ORDERWORKFLOWEXECHISTORY_DATE_TO = "owehdt";	// Request key order workflow exec history date to
	public const string REQUEST_KEY_ORDERWORKFLOWEXECHISTORY_TIME_TO = "owehtt";	// Request key order workflow exec history time from

	// 定期購入系
	public const string REQUEST_KEY_FIXEDPURCHASE_USER_NAME = "fpunm"; // ユーザー名
	public const string REQUEST_KEY_FIXEDPURCHASE_USER_ID = "fpuid"; // ユーザーID
	public const string REQUEST_KEY_FIXEDPURCHASE_SHOP_ID = "fpspid"; // 店舗ID
	public const string REQUEST_KEY_FIXEDPURCHASE_FIXED_PURCHASE_STATUS = "fpstat"; // ステータス
	public const string REQUEST_KEY_FIXEDPURCHASE_MANAGEMENT_MEMO_FLG = "fpmmflg"; // 管理メモありのみ
	public const string REQUEST_KEY_FIXEDPURCHASE_MANAGEMENT_MEMO = "fpmms"; // 管理メモ
	public const string REQUEST_KEY_FIXEDPURCHASE_FIXED_PURCHASE_KBN = "fpfpk"; //定期購入区分
	public const string REQUEST_KEY_FIXEDPURCHASE_PAYMENT_STATUS = "fpps"; //決済ステータス
	public const string REQUEST_KEY_FIXEDPURCHASE_ORDER_COUNT_FROM = "fpocf"; //購入回数(注文基準)From
	public const string REQUEST_KEY_FIXEDPURCHASE_ORDER_COUNT_TO = "fpoct"; //購入回数(注文基準)To
	public const string REQUEST_KEY_FIXEDPURCHASE_SHIPPED_COUNT_FROM = "fpscf"; //購入回数(出荷基準)From
	public const string REQUEST_KEY_FIXEDPURCHASE_SHIPPED_COUNT_TO = "fpsct"; //購入回数(出荷基準)To
	public const string REQUEST_KEY_FIXEDPURCHASE_ORDER_KBN = "fpok"; //注文区分
	public const string REQUEST_KEY_FIXEDPURCHASE_ORDER_PAYMENT_KBN = "fpopk"; //支払区分
	public const string REQUEST_KEY_FIXEDPURCHASE_PRODUCT_SEARCH_VALUE = "fppsv"; //商品情報検索値
	public const string REQUEST_KEY_FIXEDPURCHASE_SHIPPING = "fpsi"; //配送先
	public const string REQUEST_KEY_FIXEDPURCHASE_DATE_TYPE = "fpdt"; //日付種類
	public const string REQUEST_KEY_FIXEDPURCHASE_EXTEND_STATUS_NO = "fpesn";	// Extend status number
	public const string REQUEST_KEY_FIXEDPURCHASE_EXTEND_STATUS = "fpes";		// Extend status
	public const string REQUEST_KEY_FIXEDPURCHASE_EXTEND_STATUS_NO_UPDATE_DATE = "fesnud";			// 拡張ステータス更新日・ステータス
	public const string REQUEST_KEY_FIXEDPURCHASE_SHIPPING_METHOD = "fpsm";		// 配送方法
	public const string REQUEST_KEY_FIXEDPURCHASE_SHIPPING_MEMO_FLG = "fpsmmf";	// 配送メモフラグ
	public const string REQUEST_KEY_FIXEDPURCHASE_SHIPPING_MEMO = "fpsmm";		// 配送メモ
	public const string REQUEST_KEY_FIXEDPURCHASE_RECEIPT_FLG = "fprf";			// 領収書希望フラグ
	public const string REQUEST_KEY_FIXEDPURCHASE_ORDER_EXTEND_NAME = "foen";	// 注文拡張項目 項目名
	public const string REQUEST_KEY_FIXEDPURCHASE_ORDER_EXTEND_FLG = "foef";	// 注文拡張項目 内容有無
	public const string REQUEST_KEY_FIXEDPURCHASE_ORDER_EXTEND_TYPE = "foety";	// 注文拡張項目 選択方法
	public const string REQUEST_KEY_FIXEDPURCHASE_ORDER_EXTEND_TEXT = "foet";	// 注文拡張項目 内容
	public const string REQUEST_KEY_FIXEDPURCHASE_DATE_FROM = "fdf";	// Request key fixedpurchase date from
	public const string REQUEST_KEY_FIXEDPURCHASE_DATE_TO = "fdt";		// Request key fixedpurchase date to
	public const string REQUEST_KEY_FIXEDPURCHASE_TIME_FROM = "ftf";	// Request key fixedpurchase time from
	public const string REQUEST_KEY_FIXEDPURCHASE_TIME_TO = "ftt";		// Request key fixedpurchase time to
	public const string REQUEST_KEY_FIXEDPURCHASE_EXTEND_STATUS_UPDATE_DATE_FROM = "fesudf";	// Request key fixedpurchase exted status update date from
	public const string REQUEST_KEY_FIXEDPURCHASE_EXTEND_STATUS_UPDATE_DATE_TO = "fesudt";	// Request key fixedpurchase exted status update date to
	public const string REQUEST_KEY_FIXEDPURCHASE_EXTEND_STATUS_UPDATE_TIME_FROM = "fesutf";	// Request key fixedpurchase exted status update time from
	public const string REQUEST_KEY_FIXEDPURCHASE_EXTEND_STATUS_UPDATE_TIME_TO = "fesutt";	// Request key fixedpurchase exted status update time from

	// 注文系ファイル出力
	public const string REQUEST_KEY_ORDERFILE_ORDERPAGE = "opage";			// 注文系ページ区分(受注一覧 or ワークフロー)
	public const string REQUEST_KEY_SHIPPING_LABEL_LINK = "olink";			// 送り状区分
	/// <summary>Interaction data link</summary>
	public const string REQUEST_KEY_INTERACTION_DATA_LINK = "idlink";

	// Export Kbn
	public const string REQUEST_KEY_EXPORT_KBN = "exkbn";

	// PDF出力
	public const string REQUEST_KEY_PDF_OUTPUT = "pdf";			// PDF出力区分
	public const string REQUEST_KEY_PDF_KBN = "pdfkbn";			// PDF区分

	// モバイルページ
	public const string REQUEST_KEY_MOBILEPAGE_CAREER_ID = "mcid";			// モバイルキャリアID
	public const string REQUEST_KEY_MOBILEPAGE_BRAND_ID = "mbid";			// ブランドID
	public const string REQUEST_KEY_MOBILEPAGE_PAGE_ID = "mpid";			// ページID
	public const string REQUEST_KEY_MOBILEPAGE_PAGE_NAME = "mpnm";			// ページ名

	// モバイルオリジナルタグ
	public const string REQUEST_KEY_MOBILEORIGINALTAG_ORGTAG_ID = "orgid";	        // オリジナルタグID
	public const string REQUEST_KEY_MOBILEORIGINALTAG_ORGTAG_NAME = "orgnm";	        // オリジナルタグ名

	// モバイル機種グループ
	public const string REQUEST_KEY_MOBILEGROUP_MOBILE_GROUP_ID = "mgid";			// 機種グループID
	public const string REQUEST_KEY_MOBILEGROUP_MOBILE_GROUP_NAME = "mgnm";			// 機種グループ名

	// モバイルブランド
	public const string REQUEST_KEY_MOBILEGROUP_MOBILE_BRAND_ID = "mbid";			// 機種グループID
	public const string REQUEST_KEY_MOBILEGROUP_MOBILE_BRAND_NAME = "mbnm";			// 機種グループ名

	// モバイル絵文字
	public const string REQUEST_KEY_MOBILEPICTORIALSYMBOL_SYMBOL_ID = "sblid";          // 絵文字ID
	public const string REQUEST_KEY_MOBILEPICTORIALSYMBOL_SYMBOL_TAG = "sbltg";          // 絵文字タグ
	public const string REQUEST_KEY_MOBILEPICTORIALSYMBOL_SYMBOL_NAME = "sblnm";          // 絵文字名

	// 広告コード分析
	public const string REQUEST_KEY_ADVCODE_ADVCODE_NO = "aacn";					// Advertisement Code NO
	public const string REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE = "aac";				// 広告コード
	public const string REQUEST_KEY_ADVCODE_CAREER_ID = "aamci";					// キャリアID
	public const string REQUEST_KEY_ADVCODE_TERM_FROM = "aatf";						// 日付(FROM)
	public const string REQUEST_KEY_ADVCODE_TERM_TO = "aatt";						// 日付(TO)
	public const string REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE_TARGET = "aact";		// 広告コード対象
	public const string REQUEST_KEY_ADVCODE_DISP_KBN = "adk";						// 表示区分
	public const string REQUEST_KEY_ADVCODE_MEDIA_NAME = "amn";						// Media Name
	public const string REQUEST_KEY_ADVCODE_MEDIA_TYPE_ID = "amtid";				// Media Type Id

	// Advertisement Code Media type name
	public const string REQUEST_KEY_ADVCODEMEDIATYPE_MEDIA_TYPE_NAME = "amtn";		// Advertisement code media type name

	// モール連携設定
	public const string REQUEST_KEY_MALL_ID = "mallid";         // モールＩＤ

	// モール商品アップロード
	public const string REQUEST_KEY_MALLPRODUCTUPLOAD_DIPLAY_KBN = "pudk";					// モール商品アップロード商品一覧表示区分

	// モール監視ログ
	public const string REQUEST_KEY_MALLWATCHINGLOG_LOG_NO = "lgno";			// ログNo
	public const string REQUEST_KEY_MALLWATCHINGLOG_MALL_ID = "mlid";			// モールＩＤ
	public const string REQUEST_KEY_MALLWATCHINGLOG_BATCH_ID = "btid";			// バッチＩＤ
	public const string REQUEST_KEY_MALLWATCHINGLOG_LOG_KBN = "lgkn";			// ログ区分
	public const string REQUEST_KEY_MALLWATCHINGLOG_DATE_FROM = "mwldf";		// Request key mall watching log date from
	public const string REQUEST_KEY_MALLWATCHINGLOG_DATE_TO = "mwldt";			// Request key mall watching log date to
	public const string REQUEST_KEY_MALLWATCHINGLOG_TIME_FROM = "mwltf";		// Request key mall watching log time from
	public const string REQUEST_KEY_MALLWATCHINGLOG_TIME_TO = "mwltt";			// Request key mall watching log time to
	public const string REQUEST_KEY_MALLWATCHINGLOG_LOG_MESSAGE = "lgms";			// ログメッセージ

	// エラーページ
	public const string REQUEST_KEY_ERRORPAGE_MANAGER_ERRORKBN = "errkbn";			// エラー区分

	// PCフロント
	public const string REQUEST_KEY_FRONT_SHOP_ID = "shop";									// 店舗ID
	public const string REQUEST_KEY_FRONT_PRODUCT_ID = "pid";								// 商品ID
	public const string REQUEST_KEY_FRONT_VARIATION_ID = "vid";								// 商品バリエーションID

	// モバイルフロント
	public static string REQUEST_KEY_MOBILEFRONT_PAGE_ID = "pgid";							// ページID
	public static string REQUEST_KEY_MOBILEFRONT_SHOP_ID = "spid";							// 店舗ID
	public static string REQUEST_KEY_MOBILEFRONT_PRODUCT_ID = "prid";						// 商品ID

	// フロントプレビュー
	public const string REQUEST_KEY_FRONT_PREVIEW_HASH = "prvw";							// プレビューハッシュ
	/// <summary>Request key front preview guid string</summary>
	public const string REQUEST_KEY_FRONT_PREVIEW_GUID_STRING = "prvgs";

	// 新着情報
	public const string REQUEST_KEY_NEWS_NEWS_ID = "nsid";			// 新着ID
	public const string REQUEST_KEY_NEWS_NEWS_TEXT = "nstx";		// 本文
	public const string REQUEST_KEY_NEWS_DISP_FLG = "nsdf";			// 表示フラグ
	public const string REQUEST_KEY_NEWS_MOBILE_DISP_FLG = "nsmdf";	// モバイル表示フラグ
	public const string REQUEST_KEY_NEWS_VALID_FLG = "nsvf";		// 有効フラグ
	public const string REQUEST_KEY_NEWS_DISPLAY_DATE_FROM_YEAR_FROM = "nddfyf";	// 開始日時（From）(年)
	public const string REQUEST_KEY_NEWS_DISPLAY_DATE_FROM_MONTH_FROM = "nddfmf";	// 開始日時（From）(月)
	public const string REQUEST_KEY_NEWS_DISPLAY_DATE_FROM_DAY_FROM = "nddfdf";		// 開始日時（From）(日)
	public const string REQUEST_KEY_NEWS_DISPLAY_DATE_FROM_YEAR_TO = "nddfyt";		// 開始日時（To）(年)
	public const string REQUEST_KEY_NEWS_DISPLAY_DATE_FROM_MONTH_TO = "nddfmt";		// 開始日時（To）(月)
	public const string REQUEST_KEY_NEWS_DISPLAY_DATE_FROM_DAY_TO = "nddfdt";		// 開始日時（To）(日)
	public const string REQUEST_KEY_NEWS_DISPLAY_DATE_TO_YEAR_FROM = "nddtyf";	// 終了日時（From）(年)
	public const string REQUEST_KEY_NEWS_DISPLAY_DATE_TO_MONTH_FROM = "nddtmf";	// 終了日時（From）(月)
	public const string REQUEST_KEY_NEWS_DISPLAY_DATE_TO_DAY_FROM = "nddtdf";	// 終了日時（From）(日)
	public const string REQUEST_KEY_NEWS_DISPLAY_DATE_TO_YEAR_TO = "nddtyt";	// 終了日時（To）(年)
	public const string REQUEST_KEY_NEWS_DISPLAY_DATE_TO_MONTH_TO = "nddtmt";	// 終了日時（To）(月)
	public const string REQUEST_KEY_NEWS_DISPLAY_DATE_TO_DAY_TO = "nddtdt";	// 終了日時（To）(日)

	// ショートURL情報
	public const string REQUEST_KEY_SHORTURL_SHORT_URL = "surl"; // ショートURL
	public const string REQUEST_KEY_SHORTURL_LONG_URL = "lurl"; // ロングURL

	// マスタアップロードエラーログ情報
	public const string REQUEST_KEY_MASTERIMPORTOUTPUTLOG_SHOP_ID = "sid";		// 店舗ID
	public const string REQUEST_KEY_MASTERIMPORTOUTPUTLOG_MASTER = "ms";		// マスタ種別
	public const string REQUEST_KEY_MASTERIMPORTOUTPUTLOG_FILE_NAME = "fn";		// ファイル名

	// 注文メモ情報
	public const string REQUEST_KEY_ORDER_MEMO_ID = "omid";			// 注文メモID
	public const string REQUEST_KEY_ORDER_MEMO_DISP_KBN = "omdk";	// 表示区分
	public const string REQUEST_KEY_ORDER_MEMO_VALID_FLG = "omvf";		// 有効フラグ

	// デザイン
	/// <summary>デザイン：ターゲットサイト</summary>
	public const string REQUEST_KEY_DESIGN_TARGET_SITE = "tsite";
	public const string REQUEST_KEY_DESIGN_SELECTED_STANDARD_PAGE = "pcck";		// 標準ページ選択
	public const string REQUEST_KEY_DESIGN_SELECTED_CUSTOM_PAGE = "csck";		// カスタムページ選択

	// リアル店舗
	public const string REQUEST_KEY_REALSHOP_REAL_SHOP_ID = "rsid";	// リアル店舗ID
	public const string REQUEST_KEY_REALSHOP_NAME = "rsn";			// リアル店舗者名
	public const string REQUEST_KEY_REALSHOP_NAME_KANA = "rsnk";	// リアル店舗者名（かな）
	public const string REQUEST_KEY_REALSHOP_TEL = "tel";			// 電話番号
	public const string REQUEST_KEY_REALSHOP_MAIL_ADDR = "mlid";	// メールアドレス
	public const string REQUEST_KEY_REALSHOP_ZIP = "zip";			// 郵便番号
	public const string REQUEST_KEY_REALSHOP_ADDR = "add";			// 住所
	public const string REQUEST_KEY_REALSHOP_FAX = "fax";			// FAX
	public const string REQUEST_KEY_REALSHOP_VALID_FLG = "rsvf";	// 有効フラグ

	// リアル店舗
	public const string REQUEST_KEY_REALSHOPPRODUCTSTOCK_REAL_SHOP_ID = "rspid";	// リアル店舗ID
	public const string REQUEST_KEY_REALSHOPPRODUCTSTOCK_PRODUCT_ID = "rpid";	// リアル店舗者名（かな）
	public const string REQUEST_KEY_REALSHOPPRODUCTSTOCK_VARIATION_ID = "pvid";			// 電話番号
	public const string REQUEST_KEY_REALSHOPPRODUCTSTOCK_REAL_SHOP_STOCK = "rss";	// メールアドレス
	public const string REQUEST_KEY_REALSHOPPRODUCTSTOCK_REAL_SHOP_STOCK_TYPE = "rsst";	// メールアドレス

	// ユーザポイント情報
	public const string REQUEST_KEY_POINT_KBN = "pk";						// ポイント区分

	// 商品グループ設定
	public const string REQUEST_KEY_PRODUCTGROUP_PRODUCT_GROUP_ID = "pgid";	// 商品グループID
	public const string REQUEST_KEY_PRODUCTGROUP_PRODUCT_GROUP_NAME = "pgnm";	// 商品グループ名
	public const string REQUEST_KEY_PRODUCTGROUPITEM_PRODUCT_ID = "pgitem";	// 商品グループアイテム商品ID
	public const string REQUEST_KEY_PRODUCTGROUP_BEGIN_DATE_YEAR_FROM = "pgbdyf";	// 開始日時・年(From)
	public const string REQUEST_KEY_PRODUCTGROUP_BEGIN_DATE_MONTH_FROM = "pgbdmf";	// 開始日時・月(From)
	public const string REQUEST_KEY_PRODUCTGROUP_BEGIN_DATE_DAY_FROM = "pgbddf";	// 開始日時・日(From)
	public const string REQUEST_KEY_PRODUCTGROUP_BEGIN_DATE_YEAR_TO = "pgbdyt";	// 開始日時・年(To)
	public const string REQUEST_KEY_PRODUCTGROUP_BEGIN_DATE_MONTH_TO = "pgbdmt";	// 開始日時・月(To)
	public const string REQUEST_KEY_PRODUCTGROUP_BEGIN_DATE_DAY_TO = "pgbddt";	// 開始日時・日(To)
	public const string REQUEST_KEY_PRODUCTGROUP_END_DATE_YEAR_FROM = "pgedyf";	// 終了日時・年(From)
	public const string REQUEST_KEY_PRODUCTGROUP_END_DATE_MONTH_FROM = "pgedmf";	// 終了日時・月(From)
	public const string REQUEST_KEY_PRODUCTGROUP_END_DATE_DAY_FROM = "pgeddf";	// 終了日時・日(From)
	public const string REQUEST_KEY_PRODUCTGROUP_END_DATE_YEAR_TO = "pgedyt";	// 終了日時・年(To)
	public const string REQUEST_KEY_PRODUCTGROUP_END_DATE_MONTH_TO = "pgedmt";	// 終了日時・月(To)
	public const string REQUEST_KEY_PRODUCTGROUP_END_DATE_DAY_TO = "pgeddt";	// 終了日時・日(To)
	public const string REQUEST_KEY_PRODUCTGROUP_VALID_FLG = "pgvf";	// 有効フラグ

	// 商品同梱設定
	///<Summary>商品同梱ID</Summary>
	public const string REQUEST_KEY_PRODUCTBUNDLE_PRODUCT_BUNDLE_ID = "pbid";
	///<Summary>商品同梱名</Summary>
	public const string REQUEST_KEY_PRODUCTBUNDLE_PRODUCT_BUNDLE_NAME = "pbn";
	///<Summary>対象注文種別</Summary>
	public const string REQUEST_KEY_PRODUCTBUNDLE_TARGET_ORDER_TYPE = "tot";
	///<Summary>対象購入商品ID</Summary>
	public const string REQUEST_KEY_PRODUCTBUNDLE_TARGET_PRODUCT_ID = "tpid";
	///<Summary>同梱商品ID</Summary>
	public const string REQUEST_KEY_PRODUCTBUNDLE_BUNDLE_PRODUCT_ID = "bpid";

	// クーポンリストポップアップ
	///<Summary>クーポンID</Summary>
	public const string REQUEST_KEY_COUPONLISTPOPUP_COUPON_ID = "cci";
	/// <summary>クーポンコード</summary>
	public const string REQUEST_KEY_COUPONLISTPOPUP_COUPON_CODE = "cc";
	/// <summary>管理用クーポン名</summary>
	public const string REQUEST_KEY_COUPONLISTPOPUP_COUPON_NAME = "cn";
	/// <summary>発行ステータス</summary>
	public const string REQUEST_KEY_COUPONLISTPOPUP_PUBLISH_DATE = "cpd";
	/// <summary>有効フラグ</summary>
	public const string REQUEST_KEY_COUPONLISTPOPUP_VALID_FLG = "cvf";

	// 休日設定
	///<Summary>休日設定の対象（年）</Summary>
	public const string REQUEST_KEY_HOLIDAY_YEAR = "hy";

	// 自動翻訳
	///<Summary>自動翻訳元ワードの対象</Summary>
	public const string REQUEST_KEY_AUTOTRANSLATION_WORD_HASH = "awh";
	public const string REQUEST_KEY_AUTOTRANSLATION_LANGUAGE_CODE = "alc";
	public const string REQUEST_KEY_AUTOTRANSLATION_WORD_BEFORE = "awb";

	// 注文同梱
	///<Summary>注文同梱親注文ID</Summary>
	public const string REQUEST_KEY_ORDERCOMBINE_PARENT_ORDER_ID = "ordercombine_parent_id";
	///<Summary>注文同梱子注文IDs</Summary>
	public const string REQUEST_KEY_ORDERCOMBINE_CHILD_ORDER_IDs = "ordercombine_child_id";

	// 更新履歴
	/// <summary>更新履歴区分</summary>
	public const string REQUEST_KEY_UPDATEHISTORY_UPDATE_HISTORY_KBN = "ruhuhk";
	/// <summary>ユーザーID</summary>
	public const string REQUEST_KEY_UPDATEHISTORY_USER_ID = "ruhuid";
	/// <summary>マスタID</summary>
	public const string REQUEST_KEY_UPDATEHISTORY_MASTER_ID = "ruhmid";

	/// <summary>海外配送先エリアID</summary>
	public const string REQUEST_KEY_GLOBAL_SHIPPING_AREA_ID = "gsaid";

	// 名称翻訳設定
	/// <summary>翻訳対象項目</summary>
	public const string REQUEST_KEY_NAME_TRANSLATION_SETTING_TRANSLATION_TARGET_COLUMN = "ntsttc";
	/// <summary>マスタID1</summary>
	public const string REQUEST_KEY_NAME_TRANSLATION_SETTING_MASTER_ID1 = "ntsmi1";
	/// <summary>マスタID2</summary>
	public const string REQUEST_KEY_NAME_TRANSLATION_SETTING_MASTER_ID2 = "ntsmi2";
	/// <summary>言語</summary>
	public const string REQUEST_KEY_NAME_TRANSLATION_SETTING_LANGUAGE = "ntsl";

	// 定期商品変更設定
	/// <summary>定期商品変更ID</summary>
	public const string REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_FIXED_PURCHASE_PRODUCT_CHANGE_ID = "fixed_purchase_product_change_id";
	/// <summary>定期商品変更名</summary>
	public const string REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_FIXED_PURCHASE_PRODUCT_CHANGE_NAME = "fixed_purchase_product_change_name";
	/// <summary>変更元商品ID</summary>
	public const string REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_BEFORE_CHANGE_PRODUCT_ID = "before_change_product_id";
	/// <summary>変更後商品ID</summary>
	public const string REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_AFTER_CHANGE_PRODUCT_ID = "after_change_product_id";
	/// <summary>有効フラグ</summary>
	public const string REQUEST_KEY_FIXEDPURCHASEPRODUCTCHANGESETTING_VALID_FLG = "valid_flg";

	public const string REQUEST_KEY_SUBSCRIPTION_BOX_NAME = "sbcn";						//Subscription box Name
	public const string REQUEST_KEY_SUBSCRIPTION_BOX_COOPERATION_ID_HEAD = "sbccih";	//Subscription box Cooperation Id Head
	public const string REQUEST_KEY_SUBSCRIPTION_BOX_SEARCH_DISPLAY_KBN = "sbcsdk";		//Subscription box Search Display Kbn
	public const string REQUEST_KEY_SUBSCRIPTION_BOX_MOBILE_DISP_FLG = "sbcmdf";		//Subscription box Display Flag
	public const string KBN_SORT_SUBSCRIPTION_BOX_NAME_ASC = "02";						//Subscription box Name Sort Asc
	public const string KBN_SORT_SUBSCRIPTION_BOX_NAME_DESC = "03";						//Subscription box Name Sort Desc
	public const string KBN_SORT_SUBSCRIPTION_BOX_ID_ASC = "00";						//Subscription box Id Sort Asc
	public const string KBN_SORT_SUBSCRIPTION_BOX_ID_DESC = "01";						//Subscription box Id Sort Desc
	public static string KBN_SORT_SUBSCRIPTION_BOX_DEFAULT = KBN_SORT_SUBSCRIPTION_BOX_ID_ASC;	//Subscription box Display Default Sort
	public const string REQUEST_KEY_SUBSCRIPTION_BOX_DISPLAY_KBN = "sbcdk";				//Subscription box Display Kbn

	/// <summary>オペレータ名</summary>
	public const string REQUEST_KEY_OPERATOR_NAME = "operator_name";
	/// <summary>メニュー権限</summary>
	public const string REQUEST_KEY_OPERATOR_MENUACCESS_LEVEL = "mal";
	/// <summary>オペレータ有効フラグ</summary>
	public const string REQUEST_KEY_OPERATOR_VALID_FLG = "ovf";

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
	public const string ACTION_STATUS_ORDERCOMBINE = "ordercombine";	// 注文同梱
	public const string ACTION_STATUS_GLOBAL_SETTING_INSERT = "globalsettinginsert";	// グローバル設定登録
	/// <summary>Update Shipping</summary>
	public const string ACTION_STATUS_UPDATE_SHIPPING = "updateshipping";
	public const string ERROR_REQUEST_PRAMETER = "err_parameter";	// 不正パラメータ
	public const string DEFAULT_SHIPPING_PRICE = "1000";			// 初期表示配送料金
	public static string STRING_SUPERUSER_NAME = "";	// スーパーユーザー名
	public static string STRING_UNACCESSABLEUSER_NAME = "";	// アクセス権限なしユーザ名

	public const string DEFAULT_MAILTEMPLATE_ARRIVAL = "00000110";	// 再入荷通知のデフォルトメールテンプレート
	public const string DEFAULT_MAILTEMPLATE_RELEASE = "00000111";	// 販売開始通知のデフォルトメールテンプレート
	public const string DEFAULT_MAILTEMPLATE_RESALE = "00000112";	// 再販売通知のデフォルトメールテンプレート

	public const string SETTING_TABLE_NAME = "setting_table_name";	// 設定テーブル名(メールテンプレート設定／在庫文言設定のグローバル設定登録に使用)

	// ValueTextの全体で使われるSchedule
	public const string VALUETEXT_COMMON_SCHEDULE = "CommonSchedule"; // ValueTextのタグ
	public const string VALUETEXT_COMMON_SCHEDULE_EXEC_TIMING = "exec_timing";      // 実行タイミング
	public const string VALUETEXT_COMMON_SCHEDULE_KBN = "schedule_kbn";       // スケジュール区分
	public const string VALUETEXT_COMMON_SCHEDULE_DAY_OF_WEEK = "schedule_day_of_week";// スケジュール曜日
	public const string VALUETEXT_COMMON_SCHEDULE_VALID_FLG = "valid_flg";      // 有効フラグ
	public const string VALUETEXT_COMMON_SCHEDULE_DISPLAY_FORMAT = "display_format";      // 表示用フォーマット
	// 実行タイミング
	public const string VALUETEXT_COMMON_SCHEDULE_EXEC_TIMING_MANUAL = "01";	// 手動実行
	public const string VALUETEXT_COMMON_SCHEDULE_EXEC_TIMING_SCHEDULE = "02";	// スケジュール実行
	// スケジュール区分
	public const string VALUETEXT_COMMON_SCHEDULE_KBN_DAY = "01";	// 日単位
	public const string VALUETEXT_COMMON_SCHEDULE_KBN_WEEK = "02";	// 週単位
	public const string VALUETEXT_COMMON_SCHEDULE_KBN_MONTH = "03";	// 月単位
	public const string VALUETEXT_COMMON_SCHEDULE_KBN_ONCE = "05";	// 一回のみ
	// 表示用フォーマット
	public const string VALUETEXT_COMMON_DISPLAY_FORMAT_DAY = "01";	// 日単位
	public const string VALUETEXT_COMMON_DISPLAY_FORMAT_WEEK = "02";	// 週単位
	public const string VALUETEXT_COMMON_DISPLAY_FORMAT_MONTH = "03";	// 月単位
	public const string VALUETEXT_COMMON_DISPLAY_FORMAT_ONCE = "05";	// 一回のみ

	/// <summary>更新履歴情報表示フォーマット(ValueText)</summary>
	/// <summary>商品値引き</summary>
	public const string UPDATE_HISTORY_DISPLAY_FORMAT_SET_PROMOTION_DISCOUNT_TYPE_PRODUCT = "SET_PROMOTION_DISCOUNT_TYPE_PRODUCT";
	/// <summary>配送料値引き</summary>
	public const string UPDATE_HISTORY_DISPLAY_FORMAT_SET_PROMOTION_DISCOUNT_TYPE_SHIPPING_FREE = "SET_PROMOTION_DISCOUNT_TYPE_SHIPPING_FREE";
	/// <summary>決済手数料料値引き</summary>
	public const string UPDATE_HISTORY_DISPLAY_FORMAT_SET_PROMOTION_DISCOUNT_TYPE_PAYMENT_FREE = "SET_PROMOTION_DISCOUNT_TYPE_PAYMENT_FREE";
	/// <summary>ユーザーポイント</summary>
	public const string UPDATE_HISTORY_DISPLAY_FORMAT_USER_POINT = "USER_POINT";
	/// <summary>ユーザークーポン</summary>
	public const string UPDATE_HISTORY_DISPLAY_FORMAT_USER_COUPON = "USER_COUPON";
	/// <summary>ユーザー住所リスト</summary>
	public const string UPDATE_HISTORY_DISPLAY_FORMAT_USER_SHIPPING_LIST = "USER_SHIPPING_LIST";
	/// <summary>ユーザーInvoiceリスト</summary>
	public const string UPDATE_HISTORY_DISPLAY_FORMAT_USER_INVOICE_LIST = "USER_INVOICE_LIST";
	/// <summary>注文拡張ステータス</summary>
	public const string UPDATE_HISTORY_DISPLAY_FORMAT_ORDER_EXTEND_STATUS = "ORDER_EXTEND_STATUS";
	/// <summary>注文決済区分</summary>
	public const string UPDATE_HISTORY_DISPLAY_FORMAT_ORDER_PAYMENT_KBN = "ORDER_PAYMENT_KBN";
	/// <summary>注文配送種別(不明)</summary>
	public const string UPDATE_HISTORY_DISPLAY_FORMAT_ORDER_SHIPPING_TYPE_UNKNOWN = "ORDER_SHIPPING_TYPE_UNKNOWN";
	/// <summary>注文者情報</summary>
	public const string UPDATE_HISTORY_DISPLAY_FORMAT_ORDER_OWNER = "ORDER_OWNER";
	/// <summary>注文配送先</summary>
	public const string UPDATE_HISTORY_DISPLAY_FORMAT_ORDER_SHIPPING = "ORDER_SHIPPING";
	/// <summary>注文拡項目</summary>
	public const string UPDATE_HISTORY_DISPLAY_FORMAT_ORDER_EXTEND = "ORDER_EXTEND";
	/// <summary>受け取り店舗情報</summary>
	public const string UPDATE_HISTORY_DISPLAY_FORMAT_ORDER_RECEIVING_STORE = "ORDER_RECEIVING_STORE";
	/// <summary>注文Invoice</summary>
	public const string UPDATE_HISTORY_DISPLAY_FORMAT_ORDER_INVOICE = "ORDER_INVOICE";
	/// <summary>注文商品</summary>
	public const string UPDATE_HISTORY_DISPLAY_FORMAT_ORDER_ITEM = "ORDER_ITEM";
	/// <summary>セール商品</summary>
	public const string UPDATE_HISTORY_DISPLAY_FORMAT_ORDER_ITEM_SALE = "ORDER_ITEM_SALE";
	/// <summary>ノベルティ商品</summary>
	public const string UPDATE_HISTORY_DISPLAY_FORMAT_ORDER_ITEM_NOVELTY = "ORDER_ITEM_NOVELTY";
	/// <summary>更新履歴情報表示フォーマットキー：頒布会商品</summary>
	public const string UPDATE_HISTORY_DISPLAY_FORMAT_ORDER_ITEM_SUBSCRIPTION_BOX = "ORDER_ITEM_SUBSCRIPTION_BOX";
	/// <summary>注文クーポン情報</summary>
	public const string UPDATE_HISTORY_DISPLAY_FORMAT_ORDER_COUPON = "ORDER_COUPON";
	/// <summary>注文セットプロモーション情報</summary>
	public const string UPDATE_HISTORY_DISPLAY_FORMAT_ORDER_SET_PROMOTION = "ORDER_SET_PROMOTION";
	/// <summary>定期配送先</summary>
	public const string UPDATE_HISTORY_DISPLAY_FORMAT_FIXED_PURCHASE_SHIPPING = "FIXED_PURCHASE_SHIPPING";
	/// <summary>定期配送先</summary>
	public const string UPDATE_HISTORY_DISPLAY_FORMAT_FIXED_PURCHASE_ITEM = "FIXED_PURCHASE_ITEM";
	/// <summary>宅配通配送実績情報</summary>
	public const string UPDATE_HISTORY_DISPLAY_FORMAT_ORDER_SHIPPING_PELICAN_REPORT = "ORDER_SHIPPING_PELICAN_REPORT";
	/// <summary>宅配通配送実績情報（コンビニ受取の場合）</summary>
	public const string UPDATE_HISTORY_DISPLAY_FORMAT_ORDER_SHIPPING_CVS_PELICAN_REPORT = "ORDER_SHIPPING_CVS_PELICAN_REPORT";
	/// <summary>返金メモ（返金先銀行口座情報）</summary>
	public const string UPDATE_HISTORY_DISPLAY_FORMAT_REPAYMENT_MEMO = "REPAYMENT_MEMO";

	// 商品セール商品登録上限数
	public const int PRODUCTSALE_REGISTER_UPPER_LIMIT = 1000;

	/// <summary>Order payment status message kbn key</summary>
	public const string VALUETEXT_ORDER_PAYMENT_STATUS_MESSAGE_KBN = "order_payment_status_message_kbn";
	/// <summary>Order payment status message kbn key: message not payment</summary>
	public const string VALUETEXT_ORDER_PAYMENT_STATUS_MESSAGE_KBN_MSG_NOT_PAYMENT = "MSG_NOT_PAYMENT";
	/// <summary>Order payment status message kbn key: message not deposited</summary>
	public const string VALUETEXT_ORDER_PAYMENT_STATUS_MESSAGE_KBN_MSG_DEPOSITED = "MSG_DEPOSITED";
	/// <summary>Order payment message kbn key</summary>
	public const string VALUETEXT_ORDER_PAYMENT_MESSAGE_KBN = "order_payment_message_kbn";
	/// <summary>Order payment message kbn key: message payment day</summary>
	public const string VALUETEXT_ORDER_PAYMENT_MESSAGE_KBN_MESSAGE_KBN_MSG_PAYMENT_DAY = "MSG_PAYMENT_DAY";
	/// <summary>Order payment message kbn key: message error</summary>
	public const string VALUETEXT_ORDER_PAYMENT_MESSAGE_KBN_MESSAGE_KBN_MSG_ERROR = "MSG_ERROR";

	/// <summary>受注検索時の受注IDを置換するパラメーター</summary>
	public const string PARAM_REPLACEMENT_ORDER_ID_LIKE_ESCAPED = "replacement_order_id_like_escaped";

	/// <summary>CS status mode</summary>
	public const string REQUEST_KEY_CS_TASKSTATUS_MODE = "statusmode";

	/// <summary>CS incident: Status none</summary>
	public const string CONST_CSINCIDENT_STATUS_NONE = "None";
	/// <summary>CS incident: Status active</summary>
	public const string CONST_CSINCIDENT_STATUS_ACTIVE = "Active";
	/// <summary>CS incident: Status suspend</summary>
	public const string CONST_CSINCIDENT_STATUS_SUSPEND = "Suspend";
	/// <summary>CS incident: Status urgent</summary>
	public const string CONST_CSINCIDENT_STATUS_URGENT = "Urgent";
	/// <summary>CS incident: Status complete</summary>
	public const string CONST_CSINCIDENT_STATUS_COMPLETE = "Complete";

	/// <summary>Value text param transaction name</summary>
	public const string VALUETEXT_PARAM_TRANSACTION_NAME = "transaction_name";
	/// <summary>Value text param payment status log</summary>
	public const string VALUETEXT_PARAM_PAYMENT_STATUS_LOG = "payment_status_log";
	/// <summary>Value text param text mall cooperation setting</summary>
	public const string VALUETEXT_PARAM_TEXT_MALL_COOPERATION_SETTING = "text_mall_cooperation_setting";
	/// <summary>Value text param payment status regular order</summary>
	public const string VALUETEXT_PARAM_PAYMENT_STATUS_REGULAR_ORDER = "payment_status_regular_order";
	/// <summary>Value text param result</summary>
	public const string VALUETEXT_PARAM_RESULT = "result";
	/// <summary>Value text param different</summary>
	public const string VALUETEXT_PARAM_DIFFERENT = "different";
	/// <summary>Value text param all product</summary>
	public const string VALUETEXT_PARAM_ALL_PRODUCT = "all_product";
	/// <summary>Value text param master export setting</summary>
	public const string VALUETEXT_PARAM_MASTER_EXPORT_SETTING = "master_export_setting";
	/// <summary>Value text param mall watching log list</summary>
	public const string VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER = "master_export_setting_register";
	/// <summary>Value text param master export setting register extended item</summary>
	public const string VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER_EXTENDED_ITEM = "extended_item";
	/// <summary>Value text param master export setting register price shipping</summary>
	public const string VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER_PRICE_SUBTOTAL = "price_subtotal";
	/// <summary>Value text param master export setting register price shipping</summary>
	public const string VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER_PRICE_SHIPPING = "price_shipping";
	/// <summary>Value text param payment master export setting register payment price</summary>
	public const string VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER_PAYMENT_PRICE = "payment_price";
	/// <summary>Value text param master export setting register return price</summary>
	public const string VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER_RETURN_PRICE = "return_price";
	/// <summary>Value text param master export setting register return price</summary>
	public const string VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER_PRICE_TOTAL = "price_total";
	/// <summary>Value text param master export settingregister tax price</summary>
	public const string VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER_TAX_PRICE = "tax_price";
	/// <summary>Value text param master export setting register extended status</summary>
	public const string VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER_EXTENDED_STATUS = "extended_status";
	/// <summary>Value text param master export setting register extended status date</summary>
	public const string VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER_EXTENDED_STATUS_DATE = "extended_status_date";
	/// <summary>注文拡張項目</summary>
	public const string VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER_ORDER_EXTENDED_ITEM = "order_extended_item";
	/// <summary>Value text param novelty</summary>
	public const string VALUETEXT_PARAM_NOVELTY = "novelty";
	/// <summary>Value text param novelty register</summary>
	public const string VALUETEXT_PARAM_NOVELTY_REGISTER = "novelty_register";
	/// <summary>Value text param novelty register condition</summary>
	public const string VALUETEXT_PARAM_NOVELTY_REGISTER_CONDITION = "condition";
	/// <summary>Value text param order</summary>
	public const string VALUETEXT_PARAM_ORDER = "order";
	/// <summary>Value text param order modify confirm</summary>
	public const string VALUETEXT_PARAM_ORDER_MODIFY_CONFIRM = "order_modify_confirm";
	/// <summary>Value text param order modify input</summary>
	public const string VALUETEXT_PARAM_ORDER_MODIFY_INPUT = "order_modify_input";
	/// <summary>Value text param order return exchange input</summary>
	public const string VALUETEXT_PARAM_ORDER_RETURN_EXCHANGE_INPUT = "return_exchange_input";
	/// <summary>Value text param order memo info</summary>
	public const string VALUETEXT_PARAM_ORDER_MEMO_INFO = "order_memo_info";
	/// <summary>Value text param order regist</summary>
	public const string VALUETEXT_PARAM_ORDER_REGIST = "order_regist";
	/// <summary>Value text param order users shipping items</summary>
	public const string VALUETEXT_PARAM_ORDER_USERS_SHIPPING_ITEMS = "users_shipping_items";
	/// <summary>Value text param transaction name update</summary>
	public const string VALUETEXT_PARAM_TRANSACTION_NAME_UPDATE = "update";
	/// <summary>Value text param transaction name update credit card</summary>
	public const string VALUETEXT_PARAM_TRANSACTION_NAME_UPDATE_CREDIT_CARD = "update_credit_card";
	/// <summary>Value text param transaction name update relate order</summary>
	public const string VALUETEXT_PARAM_TRANSACTION_NAME_UPDATE_RELATE_ORDER = "update_related_orders";
	/// <summary>Value text param transaction name cross point integration</summary>
	public const string VALUETEXT_PARAM_TRANSACTION_NAME_CROSS_POINT_INTEGRATION = "cross_point_integration";
	/// <summary>Value text param order modify payment GMO</summary>
	public const string VALUETEXT_PARAM_ORDER_MODIFY_PAYMENT_GMO = "payment_gmo";
	/// <summary>Value text param order modify payment interlooking</summary>
	public const string VALUETEXT_PARAM_ORDER_MODIFY_PAYMENT_INTERLOOKING = "payment_interlooking";
	/// <summary>Value text param order modify change before</summary>
	public const string VALUETEXT_PARAM_ORDER_MODIFY_CHANGE_BEFORE = "change_before";
	/// <summary>Value text param order modify after change</summary>
	public const string VALUETEXT_PARAM_ORDER_MODIFY_AFTER_CHANGE = "after_change";
	/// <summary>Value text param order modify change credit card before</summary>
	public const string VALUETEXT_PARAM_ORDER_MODIFY_CHANGE_CREDIT_CARD_BEFORE = "change_credit_card_before";
	/// <summary>Value text param order modify change credit card after other</summary>
	public const string VALUETEXT_PARAM_ORDER_MODIFY_CHANGE_CREDIT_CARD_AFTER_OTHER = "change_credit_card_after_other";
	/// <summary>Value text param order modify change number credit card after</summary>
	public const string VALUETEXT_PARAM_ORDER_MODIFY_CHANGE_NUMBER_CREDIT_CARD_AFTER = "change_number_credit_card_after";
	/// <summary>Value text param order payment cooperation</summary>
	public const string VALUETEXT_PARAM_ORDER_PAYMENT_COOPERATION = "payment_cooperation";
	/// <summary>Value text param order cooperation</summary>
	public const string VALUETEXT_PARAM_ORDER_COOPERATION = "cooperation";
	/// <summary>Value text param order memo info validity period</summary>
	public const string VALUETEXT_PARAM_ORDER_MEMO_INFO_VALIDITY_PERIOD = "validity_period";
	/// <summary>Value text param order memo info default</summary>
	public const string VALUETEXT_PARAM_ORDER_MEMO_INFO_DEFAULT = "default";
	/// <summary>Value text param order regist order token expire date</summary>
	public const string VALUETEXT_PARAM_ORDER_REGIST_ORDER_TOKEN_EXPIRE_DATE = "token_expire_date";
	/// <summary>Value text param member rank</summary>
	public const string VALUETEXT_PARAM_MEMBER_RANK = "member_rank";
	/// <summary>Value text param benefit wording</summary>
	public const string VALUETEXT_PARAM_BENEFIT_WORDING = "benefit_wording";
	/// <summary>Value text param member rank at purchase</summary>
	public const string VALUETEXT_PARAM_MEMBER_RANK_AT_PURCHASE = "at_purchase";
	/// <summary>Value text param member rank order discount</summary>
	public const string VALUETEXT_PARAM_MEMBER_RANK_ORDER_DISCOUNT = "order_discount";
	/// <summary>Value text param member rank add point</summary>
	public const string VALUETEXT_PARAM_MEMBER_RANK_ADD_POINT = "add_point";
	/// <summary>Value text param member rank shipping discount</summary>
	public const string VALUETEXT_PARAM_MEMBER_RANK_SHIPPING_DISCOUNT = "shipping_discount";
	/// <summary>Value text param member rank discount rate</summary>
	public const string VALUETEXT_PARAM_MEMBER_RANK_DISCOUNT_RATE = "discount_rate";
	/// <summary>Value text param payment</summary>
	public const string VALUETEXT_PARAM_PAYMENT = "payment";
	/// <summary>Value text param product</summary>
	public const string VALUETEXT_PARAM_PRODUCT = "product";
	/// <summary>Value text param product confirm</summary>
	public const string VALUETEXT_PARAM_PRODUCT_CONFIRM = "product_confirm";
	/// <summary>Value text param product confirm data inconsistency</summary>
	public const string VALUETEXT_PARAM_PRODUCT_CONFIRM_DATA_INCONSISTENCY = "data_inconsistency";
	/// <summary>Value text param product confirm required input</summary>
	public const string VALUETEXT_PARAM_PRODUCT_CONFIRM_REQUIRED_INPUT = "required_input";
	/// <summary>Value text param product confirm arbitrary input</summary>
	public const string VALUETEXT_PARAM_PRODUCT_CONFIRM_ARBITRARY_INPUT = "arbitrary_input";
	/// <summary>Value text param order workflow</summary>
	public const string VALUETEXT_PARAM_ORDER_WORKFLOW = "order_workflow";
	/// <summary>Value text param order workflow message</summary>
	public const string VALUETEXT_PARAM_ORDER_WORKFLOW_MESSAGE = "message";
	/// <summary>Value text param order workflow message processing</summary>
	public const string VALUETEXT_PARAM_ORDER_WORKFLOW_MESSAGE_PROCESSING = "processing";
	/// <summary>Value text param order workflow message process selected taget</summary>
	public const string VALUETEXT_PARAM_ORDER_WORKFLOW_MESSAGE_PROCESS_SELECTED_TAGET = "process_selected_taget";
	/// <summary>Value text param product converter</summary>
	public const string VALUETEXT_PARAM_PRODUCT_CONVERTER = "product_converter";
	/// <summary>Value text param product converter targets</summary>
	public const string VALUETEXT_PARAM_PRODUCT_CONVERTER_TARGETS = "targets";
	/// <summary>Value text param product stock</summary>
	public const string VALUETEXT_PARAM_PRODUCT_STOCK = "product_stock";
	/// <summary>Value text param product stock history list</summary>
	public const string VALUETEXT_PARAM_PRODUCT_STOCK_HISTORY_LIST = "product_stock_history_list";
	/// <summary>Value text param product stock message register</summary>
	public const string VALUETEXT_PARAM_PRODUCT_STOCK_MESSAGE_REGISTER = "product_stock_message_register";
	/// <summary>Value text param product stock update</summary>
	public const string VALUETEXT_PARAM_PRODUCT_STOCK_UPDATE = "update";
	/// <summary>Value text param product stock language code</summary>
	public const string VALUETEXT_PARAM_PRODUCT_STOCK_LANGUAGE_CODE = "language_code";
	/// <summary>Value text param user</summary>
	public const string VALUETEXT_PARAM_USER = "user";
	/// <summary>Value text param user target list</summary>
	public const string VALUETEXT_PARAM_USER_TARGET_LIST = "target_list";
	/// <summary>Value text param user integration</summary>
	public const string VALUETEXT_PARAM_USER_INTEGRATION = "user_integration";
	/// <summary>Value text param user target type user</summary>
	public const string VALUETEXT_PARAM_USER_TARGET_TYPE_USER = "target_type_user";
	/// <summary>Value text param user target type order</summary>
	public const string VALUETEXT_PARAM_USER_TARGET_TYPE_ORDER = "target_type_order";
	/// <summary>Value text param user target type orderworkflow</summary>
	public const string VALUETEXT_PARAM_USER_TARGET_TYPE_ORDERWORKFLOW = "target_type_orderworkflow";
	/// <summary>Value text param user target type fixedpurchase</summary>
	public const string VALUETEXT_PARAM_USER_TARGET_TYPE_FIXEDPURCHASE = "target_type_fixedpurchase";
	/// <summary>Value text param user target type fixedpurchase workflow</summary>
	public const string VALUETEXT_PARAM_USER_TARGET_TYPE_FIXEDPURCHASE_WORKFLOW = "target_type_fixedpurchase_workflow";
	/// <summary>Value text param user integration unintegrate</summary>
	public const string VALUETEXT_PARAM_USER_INTEGRATION_UNINTEGRATE = "unintegrate";
	/// <summary>Value text param user integration unintegrate no</summary>
	public const string VALUETEXT_PARAM_USER_INTEGRATION_UNINTEGRATE_NO = "unintegrate_no";
	/// <summary>Value text param user integration regist button</summary>
	public const string VALUETEXT_PARAM_USER_INTEGRATION_REGIST_BUTTON = "regist_button";
	/// <summary>Value text param user integration undo button</summary>
	public const string VALUETEXT_PARAM_USER_INTEGRATION_UNDO_BUTTON = "undo_button";
	/// <summary>Value text param name translation setting</summary>
	public const string VALUETEXT_PARAM_NAME_TRANSLATION_SETTING = "name_translation_setting";
	/// <summary>Value text param translation setting register</summary>
	public const string VALUETEXT_PARAM_NAME_TRANSLATION_SETTING_REGISTER = "register";
	/// <summary>Value text param name translation setting register product tag setting name</summary>
	public const string VALUETEXT_PARAM_NAME_TRANSLATION_SETTING_REGISTER_PRODUCT_TAG_SETTING_NAME = "ProductTagSettingName";
	/// <summary>Value text param body order confirm</summary>
	public const string VALUETEXT_PARAM_BODY_ORDER_CONFIRM = "body_order_confirm";
	/// <summary>Value text param shipping date text</summary>
	public const string VALUETEXT_PARAM_SHIPPING_DATE_TEXT = "shipping_date_text";
	/// <summary>Value text param designation</summary>
	public const string VALUETEXT_PARAM_DESIGNATION = "designation";
	/// <summary>Value text param body user confirm</summary>
	public const string VALUETEXT_PARAM_BODY_USER_CONFIRM = "body_user_confirm";
	/// <summary>Value text param cmp cluster</summary>
	public const string VALUETEXT_PARAM_CMP_CLUSTER = "cmp_cluster";
	/// <summary>Value text param customer</summary>
	public const string VALUETEXT_PARAM_CUSTOMER = "customer";
	/// <summary>Value text param before</summary>
	public const string VALUETEXT_PARAM_BEFORE = "before";
	/// <summary>Value text param order confirm</summary>
	public const string VALUETEXT_PARAM_ORDER_CONFIRM = "order_confirm";
	/// <summary>Value text param payment memo</summary>
	public const string VALUETEXT_PARAM_PAYMENT_MEMO = "payment_memo";
	/// <summary>Value text param sales confirmed</summary>
	public const string VALUETEXT_PARAM_SALES_CONFIRMED = "sales_confirmed";
	/// <summary>Value text param order regist input</summary>
	public const string VALUETEXT_PARAM_ORDER_REGIST_INPUT = "order_regist_input";
	/// <summary>Value text param order regist input message</summary>
	public const string VALUETEXT_PARAM_ORDER_REGIST_INPUT_MESSAGE = "order_regist_input_message";
	/// <summary>Value text param order regist input when</summary>
	public const string VALUETEXT_PARAM_ORDER_REGIST_INPUT_WHEN = "when";
	/// <summary>Value text param order regist input ordinal</summary>
	public const string VALUETEXT_PARAM_ORDER_REGIST_INPUT_ORDINAL = "ordinal";
	/// <summary>Value text param order regist input: other message</summary>
	public const string VALUETEXT_PARAM_ORDER_REGIST_INPUT_OTHER_MESSAGE = "other_message";
	/// <summary>Value text param order regist input other message: available</summary>
	public const string VALUETEXT_PARAM_ORDER_REGIST_INPUT_AVAILABLE = "available";
	/// <summary>Value text param user extend setting list</summary>
	public const string VALUETEXT_PARAM_USER_EXTEND_SETTING_LIST = "user_extend_setting_list";
	/// <summary>Value text param inputcheck duplication</summary>
	public const string VALUETEXT_PARAM_INUTCHECK_DUPLICATION = "inputcheck_duplication";
	/// <summary>Value text param project id</summary>
	public const string VALUETEXT_PARAM_PROJECT_ID = "project_id";
	/// <summary>Value text param project name</summary>
	public const string VALUETEXT_PARAM_PROJECT_NAME = "project_name";
	/// <summary>Value text param undeliteable</summary>
	public const string VALUETEXT_PARAM_UNDELITEABLE = "undeliteable";
	/// <summary>Value text param fixed purchase message</summary>
	public const string VALUETEXT_PARAM_FIXED_PURCHASE_MESSAGE = "message";
	/// <summary>Value text param fixed purchase new branch number</summary>
	public const string VALUETEXT_PARAM_FIXED_PURCHASE_NEW_BRANCH_NUMBER = "new_branch_number";
	/// <summary>新規作成メールの置換タグ</summary>
	public const string VALUETEXT_PARAM_MAIL_TEMPLATE_SETTING_LIST = "mail_template_setting_list";
	/// <summary>新規作成メールの使用可能置換タグのカテゴリ</summary>
	public const string VALUETEXT_PARAM_USE_APPLICATIONS = "use_applications";

	/// <summary>Value text param: EcPay payment type</summary>
	public const string VALUETEXT_PARAM_ECPAY_PAYMENT_TYPE = "ec_payment_type";
	/// <summary>Value text param: Neweb payment type</summary>
	public const string VALUETEXT_PARAM_NEWEB_PAYMENT_TYPE = "neweb_payment_type";
	/// <summary>Value text param: Order workflow setting fixed purchase</summary>
	public const string VALUETEXT_PARAM_ORDERWORKFLOWSETTING_FIXED_PURCHASE = "fixedpurchase";

	/// <summary>Field extend for gift</summary>
	public const string CONST_FIELD_EXTEND_FOR_GIFT = "_for_gift";
	/// <summary>Field extend search text</summary>
	public const string CONST_FIELD_EXTEND_SEARCH_TEXT = "_search_text";

	/// <summary>Value text param data migration manager</summary>
	public const string VALUETEXT_PARAM_DATA_MIGRATION_MANAGER = "data_migration_manager";
	/// <summary>Value text param credit mode text</summary>
	public const string VALUETEXT_PARAM_CREDIT_MODE_TEXT = "credit_mode";

	/// <summary>XML master upload setting master element</summary>
	public const string XML_MASTERUPLOADSETTING_MASTER_ELEMENT = "Master";
	/// <summary>XML master upload setting name element</summary>
	public const string XML_MASTERUPLOADSETTING_NAME_ELEMENT = "Name";
	/// <summary>XML master upload setting directory element</summary>
	public const string XML_MASTERUPLOADSETTING_DIRECTORY_ELEMENT = "Directory";
	/// <summary>Master upload setting action kbn delete product image</summary>
	public const string MASTERUPLOADSETTING_ACTION_KBN_DELETEPRODUCTIMAGE = "DeleteProductImage";

	/// <summary>Param product process action kbn</summary>
	public const string PARAM_PRODUCT_PROCESS_ACTION_KBN = "product_process_action_kbn";
	/// <summary>Product process action kbn: Regist preview</summary>
	public const string PRODUCT_PROCESS_ACTION_KBN_REGIST_PREVIEW = "registpreview";
	/// <summary>Product process action kbn: Update preview</summary>
	public const string PRODUCT_PROCESS_ACTION_KBN_UPDATE_PREVIEW = "updatepreview";
	/// <summary>Product process action kbn: Import</summary>
	public const string PRODUCT_PROCESS_ACTION_KBN_IMPORT = "import";
	/// <summary>Product process action kbn: Upload image insert</summary>
	public const string PRODUCT_PROCESS_ACTION_KBN_UPLOAD_IMAGE_INSERT = "uploadimageinsert";
	/// <summary>Product process action kbn: Upload image update or copy insert</summary>
	public const string PRODUCT_PROCESS_ACTION_KBN_UPLOAD_IMAGE_UPDATE_OR_COPY_INSERT = "uploadimageupdateorcopyinsert";
	/// <summary>Param product import master type</summary>
	public const string PARAM_PRODUCT_IMPORT_MASTER_TYPE = "import_master_type";
	/// <summary>Param product input</summary>
	public const string PARAM_PRODUCT_INPUT = "product_input";
	/// <summary>Jpg file extension</summary>
	public const string JPG_FILE_EXTENSION = ".jpg";
	/// <summary>Param product preview site</summary>
	public const string PARAM_PRODUCT_PREVIEW_SITE = "product_preview_site";
	/// <summary>Param guid string</summary>
	public const string PARAM_GUID_STRING = "guid_string";
	/// <summary>Param is back from confirm</summary>
	public const string PARAM_IS_BACK_FROM_CONFIRM = "is_back_from_confirm";
	/// <summary>Param product upload image input</summary>
	public const string PARAM_PRODUCT_UPLOAD_IMAGE_INPUT = "upload_image_input";
	/// <summary>Param reference product id</summary>
	public const string PARAM_REFERENCE_PRODUCT_ID = "ref_product_id";

	/// <summary>Value text param: Product fixed purchase discount setting: Number disp text</summary>
	public const string VALUETEXT_PARAM_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_NUMBER_DISP_TEXT = "number_disp_text";
	/// <summary>Value text param: Product fixed purchase discount setting: Number disp text: Line</summary>
	public const string VALUETEXT_PARAM_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_NUMBER_DISP_TEXT_LINE = "LINE";
	/// <summary>Value text param: Product fixed purchase discount setting: Number disp text: Column</summary>
	public const string VALUETEXT_PARAM_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_NUMBER_DISP_TEXT_COLUMN = "COLUMN";

	/// <summary>Param order workflow requests</summary>
	public const string PARAM_ORDERWORKFLOW_REQUESTS = "requests";
	/// <summary>Param order workflow action kbn</summary>
	public const string PARAM_ORDERWORKFLOW_ACTION_KBN = "action_kbn";
	/// <summary>Order workflow action kbn: order count</summary>
	public const string ORDERWORKFLOW_ACTION_KBN_ORDER_COUNT = "order_count";
	/// <summary>Order workflow action kbn: action and conditions</summary>
	public const string ORDERWORKFLOW_ACTION_KBN_ACTION_CONDITIONS = "action_conditions";

	/// <summary>ユーザー検索サジェスト：最大表示件数</summary>
	public static int AUTO_SUGGEST_MAX_COUNT_FOR_DISPLAY = 0;

	/// <summary>Get exchange rate execution path</summary>
	public static string PHYSICALDIRPATH_GET_EXCHANGERATE_EXE = string.Empty;

	/// <summary>商品検索ページ呼び出し元 </summary>
	public const string REQUEST_KEY_PRODUCT_SEARCH_CALLER = "prosc";
	/// <summary>呼び出し元：商品セール設定</summary>
	public const string REQUEST_KEY_NAME_PRODUCT_SALE = "prsa";

	/// <summary>NP後払い：返品時差額上限金額</summary>
	public const int NPAFTERPAY_DIFFERENCE_LIMIT = 3000;

	/// <summary>Flag list real shop id</summary>
	public const string FLG_LIST_REAL_SHOP_ID = "list_real_shop_id";
	/// <summary>Flag area real shop name</summary>
	public const string FLG_AREA_REAL_SHOP_NAME = "area_real_shop_name";
	/// <summary>Flag mail addr store pick up option valid</summary>
	public const string FLG_MAIL_ADDR_STORE_PICK_UP_OPTION_VALID = "mail_addr_store_pick_up_valid";
	/// <summary>Same shipping address as owner</summary>
	public const string SHIPPING_KBN_LIST_SAME_AS_OWNER = "SAME_AS_OWNER";
	/// <summary>User input shipping address</summary>
	public const string SHIPPING_KBN_LIST_USER_INPUT = "USER_INPUT";
	/// <summary>Real store pickup</summary>
	public const string SHIPPING_KBN_LIST_STORE_PICKUP = "STORE_PICKUP";
}
