/*
=========================================================================================================
  Module      : 定数定義(Constants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
using w2.Domain.MenuAuthority.Helper;

namespace w2.Cms.Manager.Codes
{
	/// <summary>
	/// 定数定義
	/// </summary>
	public class Constants : w2.App.Common.Constants
	{
		/// <summary>アプリケーション表示名</summary>
		public const string APPLICATION_NAME_DISP = "w2CmsManager";

		/// <summary>マネージャサイトタイプ</summary>
		public static readonly MenuAuthorityHelper.ManagerSiteType ManagerSiteType = MenuAuthorityHelper.ManagerSiteType.Cms;

		/// <summary>デフォルトレイアウトパス</summary>
		public const string LAYOUT_PATH_DEFAULT = "~/Views/Common/_LayoutDefault.cshtml";
		/// <summary>ポップアップウィンドウ用デフォルトレイアウトパス</summary>
		public const string POPUP_LAYOUT_PATH_DEFAULT = "~/Views/Common/_PopupLayoutDefault.cshtml";
		/// <summary>デザイナ用デフォルトレイアウトパス</summary>
		public const string DESIGNER_LAYOUT_PATH_DEFAULT = "~/Views/LandingPage/_DesignerLayoutDefault.cshtml";

		/// <summary>エラー区分</summary>
		public const string REQUEST_KEY_ERROR_KBN = "ErrorKbn";
		/// <summary>エラー区分：システムエラー</summary>
		public const string REQUEST_KBN_ERROR_KBN_SYSTEM_ERROR = "serr";
		/// <summary>エラー区分：システム検証エラー</summary>
		public const string REQUEST_KBN_ERROR_KBN_SYTEM_VALIDATION_ERROR = "svlderr";
		/// <summary>エラー区分：404エラー</summary>
		public const string REQUEST_KBN_ERROR_KBN_404_ERROR = "404";
		/// <summary>エラー区分：未ログインエラー</summary>
		public const string REQUEST_KBN_ERROR_KBN_UNLOGGEDIN_ERROR = "unlgdinerr";
		/// <summary>エラー区分：CMSオプションOFFエラー</summary>
		public const string REQUEST_KBN_ERROR_KBN_UNCMS_OPTION_DISABLED_ERROR = "uncmsoptionerr";

		/// <summary>エラーページタイプ</summary>
		public const string REQUEST_KEY_ERRORPAGE_TYPE = "ErrorPageType";
		/// <summary>エラーページタイプ：ログインページへボタン表示</summary>
		public const string REQUEST_KBN_ERRORPAGE_TYPE_DISP_GOTOLOGIN = "gotologin";

		/// <summary>商品検索区分</summary>
		public const string REQUEST_KEY_PRODUCT_SEARCH_KBN = "sk";
		/// <summary>有効フラグ</summary>
		public const string REQUEST_KEY_PRODUCT_VALID_FLG = "pvf";
		/// <summary>表示商品作成バッチ実行ＥＸＥ</summary>
		public static string PHYSICALDIRPATH_CREATEDISPPRODUCT_EXE = "";

		/// <summary>商品検索</summary>
		public const string KBN_PRODUCT_SEARCH_PRODUCT = "product";
		/// <summary>バリエーション検索</summary>
		public const string KBN_PRODUCT_SEARCH_VARIATION = "variation";
		/// <summary>注文商品検索</summary>
		public const string KBN_PRODUCT_SEARCH_ORDERPRODUCT = "orderproduct";

		/// <summary>スーパーユーザー名</summary>
		public const string STRING_SUPERUSER_NAME = "スーパーユーザー";
		/// <summary>アクセス権限なしユーザ名</summary>
		public const string STRING_UNACCESSABLEUSER_NAME = "アクセス権限なし";

		/// <summary>スーパーユーザーレベル区分</summary>
		public static  readonly int KBN_OPERATOR_LEVEL_SUPERUSER = 0;
		/// <summary>権限なしユーザーレベル区分</summary>
		public static readonly int? KBN_OPERATOR_LEVEL_UNACCESSABLEUSER = null;

		/// <summary>サイト情報XMLパス</summary>
		public const string FILE_XML_SHOP_MESSAGE = "Contents\\ShopMessage.xml";

		/// <summary>不正パラメータ</summary>
		public const string ERROR_REQUEST_PRAMETER = "err_parameter";

		/// <summary>コピー新規時の接尾語</summary>
		public const string COPY_NEW_SUFFIX = "-copy";

		// パス（global.asaxで設定）
		/// <summary>マスタファイル取込実行ＥＸＥ</summary>
		public static string PHYSICALDIRPATH_MASTERUPLOAD_EXE = "";

		// PCフロント リクエストパラメータ名
		/// <summary>PCフロント リクエストパラメータ名 : 店舗ID</summary>
		public const string REQUEST_KEY_FRONT_SHOP_ID = "shop";
		/// <summary>PCフロント リクエストパラメータ名 : 商品ID</summary>
		public const string REQUEST_KEY_FRONT_PRODUCT_ID = "pid";
		/// <summary>PCフロント リクエストパラメータ名：商品バリエーションID</summary>
		public const string REQUEST_KEY_FRONT_VARIATION_ID = "vid";

		/// <summary>特集ページタイプ</summary>
		public const string REQUEST_KEY_FEATURE_PAGE_TYPE = "fpagetype";

		// サイトマップ 更新頻度
		/// <summary>表示毎</summary>
		public const int SITEMAP_CHANGE_FREQ_ALWAYS = 1;
		/// <summary>１時間毎</summary>
		public const int SITEMAP_CHANGE_FREQ_HOURLY = 2;
		/// <summary>１日毎</summary>
		public const int SITEMAP_CHANGE_FREQ_DAILY = 3;
		/// <summary>１週間毎</summary>
		public const int SITEMAP_CHANGE_FREQ_WEEKLY = 4;
		/// <summary>１ヶ月毎</summary>
		public const int SITEMAP_CHANGE_FREQ_MONTHLY = 5;
		/// <summary>１年毎</summary>
		public const int SITEMAP_CHANGE_FREQ_YEARLY = 6;
		/// <summary>更新しない</summary>
		public const int SITEMAP_CHANGE_FREQ_NEVER = 7;

		// サイトマップ ページ種別
		/// <summary>指定なし</summary>
		public const int SITEMAP_PAGE_TYPE_NONE = 1;
		/// <summary>標準ページ</summary>
		public const int SITEMAP_PAGE_TYPE_STANDARD = 2;
		/// <summary>カスタムページ</summary>
		public const int SITEMAP_PAGE_TYPE_CUSTOM = 3;
		/// <summary>ランディングページ</summary>
		public const int SITEMAP_PAGE_TYPE_LANDING = 4;
		/// <summary>コーディネートページ</summary>
		public const int SITEMAP_PAGE_TYPE_COORDINATE = 5;
		/// <summary>特集ページ</summary>
		public const int SITEMAP_PAGE_TYPE_FEATURE = 6;

		//特集ページ
		/// <summary>PCフロント リクエストパラメータ名 : プレビューハッシュ</summary>
		public const string REQUEST_KEY_FRONT_PREVIEW_HASH = "prvw";

		/// <summary>CMS共通ValueTextキー</summary>
		public const string VALUE_TEXT_KEY_CMS_COMMON = "CMS";
		/// <summary>ページの利用状況ValueTextフィールド名</summary>
		public const string VALUE_TEXT_FIELD_USE_TYPE = "UseType";
		/// <summary>標準パーツValueTextフィールド名</summary>
		public const string VALUE_TEXT_FIELD_STANDARD_PARTS = "StandardParts";
		/// <summary>ページ管理ValueTextフィールド名</summary>
		public const string VALUE_TEXT_FIELD_PAGE_DESIGN = "PageDesign";
		/// <summary>実行EXE：ページ管理・パーツ管理の整合性調整バッチ</summary>
		public static string PHYSICALDIRPATH_PAGEDESIGN_CONSISTENCY_EXE = "";
		/// <summary>同時編集回避除外オペレータIDリスト</summary>
		public static List<string> CONCURRENT_EDIT_EXCLUSION_LOGIN_OPERATOR_ID_LIST = null;

		/// <summary>Value text param master export setting</summary>
		public const string VALUETEXT_PARAM_MASTER_EXPORT_SETTING = "MasterExportSetting";
		/// <summary>Value text param master export setting register</summary>
		public const string VALUETEXT_PARAM_MASTER_EXPORT_SETTING_REGISTER = "MasterExportSettingRegister";
		/// <summary>Value text param undeliteable</summary>
		public const string VALUETEXT_PARAM_UNDELITEABLE = "Undeliteable";

		/// <summary>コンテンツマネージャーショットカット 「名前」と「パス」のペア</summary>
		public static List<KeyValuePair<string, string>> CONTENTSMANAGER_CONTENTS_SHORTCUT_LIST = new List<KeyValuePair<string, string>>();
	}
}