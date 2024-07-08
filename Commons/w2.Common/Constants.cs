/*
=========================================================================================================
  Module      : 共通定数定義(Constants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Net.Mime;
using System.Text;
using w2.Common.Util.TagReplacer;

namespace w2.Common
{
	///**************************************************************************************
	/// <summary>
	/// 定数/設定値を管理する
	/// </summary>
	///**************************************************************************************
	public partial class Constants
	{
		//=========================================================================================
		// 各種設定（設定ファイルより読み出しを行い、アプリケーション毎に設定）
		//=========================================================================================
		//------------------------------------------------------
		// アプリケーション設定
		//------------------------------------------------------
		/// <summary>アプリケーション名（イベントログで利用）</summary>
		public static string APPLICATION_NAME = "";		// イベントログなどで利用
		/// <summary>日本語アプリケーション名（常駐プログラムのアプリケーション名で利用）</summary>
		public static string APPLICATION_NAME_JP = "";	// 常駐プログラムのタイトルバーなどで利用

		/// <summary>プロジェクト名付きアプリケーション名</summary>
		public static string APPLICATION_NAME_WITH_PROJECT_NO = "";

		//------------------------------------------------------
		// WEBサイト設定
		//------------------------------------------------------
		/// <summary>サイトドメイン</summary>
		public static string SITE_DOMAIN = "";
		/// <summary>WEBサイトパスルート</summary>
		public static string PATH_ROOT = "";
		/// <summary>WEBPCサイトパスルート</summary>
		public static string PATH_ROOT_FRONT_PC = "";
		/// <summary> WEBPCサイトパスルート </summary>
		public static string PATH_ROOT_CMS = "";
		/// <summary>WEBPCサイトパスルート</summary>
		public static string URL_FRONT_PC = "";
		/// <summary>WEBPCサイトパスルート(https)</summary>
		public static string URL_FRONT_PC_SECURE = "";
		/// <summary>WEBMobileサイトパスルート</summary>
		public static string PATH_ROOT_FRONT_MOBILE = "";
		/// <summary>WEBMobileサイトパスルート</summary>
		public static string URL_FRONT_MOBILE = "";
		/// <summary>WEBサイトHTTPプロトコル文字列</summary>
		public static string PROTOCOL_HTTP = "";
		/// <summary>WEBサイトHTTPSプロトコル文字列</summary>
		public static string PROTOCOL_HTTPS = "";
		/// <summary>本番環境設定</summary>
		public static bool SETTING_PRODUCTION_ENVIRONMENT = false;

		//------------------------------------------------------
		// 送信メールサーバー設定
		//------------------------------------------------------
		/// <summary>SMTPサーバ名</summary>
		public static string SERVER_SMTP = "";
		/// <summary>SMTPサーバポート</summary>
		public static int SERVER_SMTP_PORT = 25;
		/// <summary>SMTP認証タイプ</summary>
		public static SmtpAuthType SERVER_SMTP_AUTH_TYPE = SmtpAuthType.Normal;
		/// <summary>SMTP認証用POPサーバー</summary>
		public static string SERVER_SMTP_AUTH_POP_SERVER = "";
		/// <summary>SMTP認証用POPサーバーポート</summary>
		public static string SERVER_SMTP_AUTH_POP_PORT = "";
		/// <summary>SMTP認証用POPサーバー認証タイプ(APOPか否か)</summary>
		public static PopType SERVER_SMTP_AUTH_POP_TYPE = PopType.Pop;
		/// <summary>SMTP認証用POPサーバー認証ユーザー名</summary>
		public static string SERVER_SMTP_AUTH_USER_NAME = "";
		/// <summary>SMTP認証用POPサーバー認証ユーザーパスワード</summary>
		public static string SERVER_SMTP_AUTH_PASSOWRD = "";

		//------------------------------------------------------
		// 受信メールサーバー設定
		//------------------------------------------------------
		/// <summary>POPサーバ名</summary>
		public static string SERVER_POP = "";
		/// <summary>POPサーバーポート</summary>
		public static int SERVER_POP_PORT = 110;
		/// <summary>POPサーバー認証タイプ(APOPか否か)</summary>
		public static PopType SERVER_POP_TYPE = PopType.Apop;
		/// <summary>POPサーバーユーザー名</summary>
		public static string SERVER_POP_AUTH_USER_NAME = "";
		/// <summary>POPサーバーユーザーパスワード</summary>
		public static string SERVER_POP_AUTH_PASSOWRD = "";

		/// <summary>メール受信エラー通知メールアドレスFROM</summary>
		public static string MAIL_RECV_ERROR_MAILADDR_FROM = "";
		/// <summary>メール受信エラー通知メールアドレスTO</summary>
		public static string[] MAIL_RECV_ERROR_MAILADDR_TO = new string[0];

		/// <summary>エラーメール送信先</summary>
		public static string ERROR_MAILADDRESS = null;

		//------------------------------------------------------
		// SQLServer設定
		//------------------------------------------------------
		/// <summary>標準SQL接続文字列</summary>
		public static string STRING_SQL_CONNECTION = "";
		/// <summary>転送先SQL接続文字列</summary>
		public static string STRING_SQL_CONNECTION_TRANSFER = "";

		//------------------------------------------------------
		// ログファイル設定
		//------------------------------------------------------
		/// <summary>ログ出力設定</summary>
		public static List<string> KBN_LOGOUTPUT_SETTINGS = new List<string>();

		/// <summary>ログファイル拡張子</summary>
		public static string LOGFILE_EXTENSION = "log";	// ログファイル拡張子

		/// <summary>ログファイル文字コード</summary>
		public static string LOGFILE_ENCODING = "shift_jis";

		/// <summary>SQLパフォーマンスログ出力設定</summary>
		public static bool LOGGING_PERFORMANCE_SQL_ENABLED = false;
		/// <summary>リクエストパフォーマンスログ出力設定</summary>
		public static bool LOGGING_PERFORMANCE_REQUEST_ENABLED = false;
		/// <summary>オペレーションログエラーファイル名</summary>
		public static string LOGFILE_NAME_OPERATIONLOG_NOT_SEND = "operationlog_not_sent";

		//------------------------------------------------------
		// アプリケーション物理パス設定
		//------------------------------------------------------
		/// <summary>ログファイル格納物理ディレクトリパス</summary>
		public static string PHYSICALDIRPATH_LOGFILE = "";
		/// <summary>オペレータ操作ログファイル格納物理ディレクトリパス</summary>
		public static string PHYSICALDIRPATH_OPERATION_LOGFILE = "";
		/// <summary>オペレータ操作送信できないログファイル格納物理ディレクトリパス</summary>
		public static string PHYSICALDIRPATH_OPERATION_NOTSEND_LOGFILE = "";
		/// <summary>ステートメントXML格納物理ディレクトリパス</summary>
		public static string PHYSICALDIRPATH_SQL_STATEMENT = "";
		/// <summary>ValuteText XML物理ファイルパス（デフォルトではないものを指定する場合はValueTextアクセス前に設定する必要があります）</summary>
		public static string PHYSICALFILEPATH_VALUETEXT = "";
		/// <summary>SeoMapping XML物理ファイルパス</summary>
		public static string PHYSICALFILEPATH_SEOMAPPING = "";
		/// <summary>ヴァリデータXML格納物理ディレクトリパス</summary>
		public static string PHYSICALDIRPATH_VALIDATOR = "";
		/// <summary>エラーメッセージXML物理ファイルディレクトリパス</summary>
		public static List<string> PHYSICALFILEPATH_ERROR_MESSAGE_XMLS = new List<string>();
		/// <summary>一時ファイル格納物理ディレクトリパス</summary>
		public static string PHYSICALDIRPATH_TEMP = "";

		//------------------------------------------------------
		// ユーザーパスワード暗号化設定
		//------------------------------------------------------
		/// <summary>ユーザーパスワード暗号化：秘密鍵</summary>
		public static byte[] ENCRYPTION_USER_PASSWORD_KEY = null;
		/// <summary>ユーザーパスワード暗号化：初期化ベクトル</summary>
		public static byte[] ENCRYPTION_USER_PASSWORD_IV = null;

		//========================================================================		
		// ユーザー情報設定
		//========================================================================		
		/// <summary>ユーザー情報設定：表示名</summary>
		public static string USER_NAME_COMPLETION_TEXT = null;
		/// <summary>Amazonログイン連携： ID格納用ユーザー拡張項目名</summary>
		public static string AMAZON_USER_ID_USEREXTEND_COLUMN_NAME = "";

		/// <summary>マスタ出力設定のエクセル形式（.xls：Excel2000/2002/2003、.xlsx：Excel2007以降）</summary>
		public static string MASTEREXPORT_EXCELFORMAT = "";
		/// <summary>CSVマスタ出力時の文字コード</summary>
		public static string MASTEREXPORT_CSV_ENCODE = "shift_jis";

		//------------------------------------------------------
		// デジタルコンテンツ系設定
		//------------------------------------------------------
		/// <summary>デジタルコンテンツ：シリアルキーフォーマット</summary>
		public static string DIGITAL_CONTENTS_SERIAL_KEY_FORMAT = "";
		/// <summary>デジタルコンテンツ：シリアルキー有効日数（0：無期限）</summary>
		public static int DIGITAL_CONTENTS_SERIAL_KEY_VALID_DAYS = 0;

		//=========================================================================================
		// 各種定数
		//=========================================================================================
		//------------------------------------------------------
		// メール送信設定
		//------------------------------------------------------
		/// <summary>ネットワーク読み取りタイムアウト（ミリ秒）</summary>
		public const int MAILSEND_NETWORK_STREAM_READ_TIMEOUT = 30000;
		/// <summary>メール送信最大リトライ回数</summary>
		public const int MAILSEND_RETRY_COUNT_MAX = 1;
		/// <summary>メール送信デフォルトエンコーディング（日本向けのメール：「ISO-2022-JP」）</summary>
		public const string MAIL_DEFAULT_ENCODING = "ISO-2022-JP";
		/// <summary>PCメール送信デフォルトエンコーディング</summary>
		public static Encoding PC_MAIL_DEFAULT_ENCODING = Encoding.GetEncoding(MAIL_DEFAULT_ENCODING);
		/// <summary>PCメール送信デフォルトエンコーディング表示名</summary>
		public static string PC_MAIL_DEFAULT_ENCODING_STRING = MAIL_DEFAULT_ENCODING;
		/// <summary>モバイルメール送信エンコーディング</summary>
		public readonly static Encoding MOBILE_MAIL_ENCODING = Encoding.GetEncoding(MAIL_DEFAULT_ENCODING);
		/// <summary>PCメール送信デフォルトエンコーディング(Content-Transfer-Encoding用)</summary>
		public static TransferEncoding PC_MAIL_DEFAULT_TRANSFER_ENCODING = TransferEncoding.SevenBit;
		/// <summary>モバイルメール送信エンコーディング(Content-Transfer-Encoding用)</summary>
		public readonly static TransferEncoding MOBILE_MAIL_TRANSFER_ENCODING = TransferEncoding.SevenBit;
		/// <summary>メールに表示する金額のフォーマット(基軸通貨 JPYの場合に限定)</summary>
		public static string SETTING_MAIL_PRICE_FORMAT = "";
		/// <summary>メール送信禁止文字列</summary>
		public static string[] MAIL_TRANSMISSION_DISABLED_STRINGS = { string.Empty };
		/// <summary>Maild send: sleep time (millisecond)</summary>
		public const int MAILSEND_SLEEP_TIME = 5000;

		//------------------------------------------------------
		// メール受信設定
		//------------------------------------------------------
		/// <summary>メール受信ネットワーク読み取りタイムアウト（ミリ秒）</summary>
		public const int MAILRECEIVE_NETWORK_STREAM_READ_TIMEOUT = 30000;	// 30秒
		/// <summary>メール受信ネットワーク読み取りムタイムアウト（ミリ秒）</summary>
		public const int MAILRECEIVE_NETWORK_STREAM_WRITE_TIMEOUT = 30000;	// 30秒

		//=========================================================================================
		// 各種固定ディレクトリ定数
		//=========================================================================================
		/// <summary>ステートメントXML格納ディレクトリ</summary>
		public const string DIRPATH_XML_STATEMENTS = @"Xml\Db\";
		/// <summary>ヴァリデータXML格納ディレクトリ</summary>
		public const string DIRPATH_XML_VALIDATORS = @"Xml\Validator\";

		/// <summary>ページタイトルファイルパス</summary>
		public const string FILEPATH_XML_PAGE_TITLES = @"Xml\PageTitles.xml";
		/// <summary>バリューテキストファイルパス</summary>
		public const string FILEPATH_XML_VALUE_TEXT = @"Xml\ValueText.xml";
		/// <summary>カラーマップ設定ファイルパス</summary>
		public const string FILEPATH_XML_COLORS = @"Xml\ProductColors.xml";
		/// <summary>SEOマッピングファイルパス</summary>
		public const string FILEPATH_XML_SEO_MAPPING = @"Xml\SeoMapping.xml";

		//=========================================================================================
		// 共通リクエストキー
		//=========================================================================================
		/// <summary>ページ番号</summary>
		public const string REQUEST_KEY_PAGE_NO = "pno";

		//========================================================================
		// ページネーションタグ
		//========================================================================
		public const string CONST_PAGINATION_PREVIOUS_TAG = "<link rel=\"prev\" href=\"{0}\" />";
		public const string CONST_PAGINATION_NEXT_TAG = "<link rel=\"next\" href=\"{0}\" />";

		/// <summary>
		/// 顧客別リソース格納ディレクトリパス設定
		/// </summary>
		public static string PHYSICALDIRPATH_CUSTOMER_RESOUERCE = "";

		//HACK:下記はValidatorに必要なのでここで定義するが、実際はValudatorの一部含めApp.Commonにあるのが正しいと思うのでどこかでリファクタリングする
		/// <summary>グローバルオプション</summary>
		public static bool GLOBAL_OPTION_ENABLE = false;
		/// <summary>タグリプレーサ</summary>
		public static TagReplacer TAG_REPLACER_DATA_SCHEMA = null;
		// 国ISOコード
		/// <summary>国ISOコード：US(アメリカ)</summary>
		public const string COUNTRY_ISO_CODE_US = "US";
		/// <summary>国ISOコード：JP(台湾)</summary>
		public const string COUNTRY_ISO_CODE_TW = "TW";
		/// <summary>国ISOコード：JP(日本)</summary>
		public const string COUNTRY_ISO_CODE_JP = "JP";
		// 言語ロケールID
		/// <summary>言語ロケールID：日本語・日本</summary>
		public const string LANGUAGE_LOCALE_ID_JAJP = "ja-JP";
		/// <summary>言語ロケールID：台湾語・台湾</summary>
		public const string LANGUAGE_LOCALE_ID_ZHTW = "zh-TW";
		// 言語コード
		/// <summary>言語コード：ja(日本語)</summary>
		public const string LANGUAGE_CODE_JAPANESE = "ja";

		/// <summary>管理側の言語ロケールコード</summary>
		public static string OPERATIONAL_LANGUAGE_LOCALE_CODE = "";
		/// <summary>納品書のデフォルト言語コード</summary>
		public static string DEFAULT_INVOICE_LANGUAGE_CODE = "";

		/// <summary>オプション：GLOBALSMS</summary>
		public static bool GLOBAL_SMS_OPTION_ENABLED = false;
		/// <summary>オプション：LINE連携</summary>
		public static bool LINE_COOPERATION_OPTION_ENABLED = false;

		/// <summary>列挙体：リピートラインオプション</summary>
		public enum RepeatLineOption
		{
			/// <summary>ユーザー連携とLINE送信両方使用</summary>
			CooperationAndMessaging,
			/// <summary>ユーザー連携のみ使用</summary>
			CooperationOnly,
			/// <summary>不使用</summary>
			Off,
		}
		/// <summary> オプション：リピートライン</summary>
		public static RepeatLineOption REPEATLINE_OPTION_ENABLED;

		/// <summary>GLOBALSMS種別</summary>
		public static string GLOBAL_SMS_TYPE = "";
		/// <summary>GLOBALSMS送信元</summary>
		public static string GLOBAL_SMS_FROM = "";
		/// <summary>GLOBALSMS完了ステータスが確認できずに経過した場合にエラーとする時間</summary>
		public static int GLOBAL_SMS_STATUS_TIME_OVER_HOURS = 0;
		/// <summary>GLOBALSMS送信停止とするエラーポイントの閾値</summary>
		public static int GLOBAL_SMS_STOP_ERROR_POINT = 0;
		/// <summary>GLOBALSMSステータスのクリーニング対象となる超過日数の閾値</summary>
		public static int GLOBAL_SMS_STATUS_CLEAING_DAYS = 0;
		/// <summary>MacroKiosk：APIのURL</summary>
		public static string MACROKIOSK_API_URL = "";
		/// <summary>MacroKiosk：APIのUSER</summary>
		public static string MACROKIOSK_API_USER = "";
		/// <summary>MacroKiosk：APIのPASS</summary>
		public static string MACROKIOSK_API_PASS = "";
		/// <summary>MacroKiosk：APIのサービスID</summary>
		public static string MACROKIOSK_API_SERVID = "";
		/// <summary>MacroKiosk：DN受信ファイルの出力先</summary>
		public static string MACROKIOSK_DN_OUTPUT_DIR_PATH = "";

		/// <summary>タグ機能:タグ機能の出力を本番環境以外でも出力を許可する TRUE:本番のみ FALSE:本番以外でも出力</summary>
		public static bool TAG_OUTPUT_PRODUCT_ENV_ONLY = false;

		/// <summary>GLOBALSMSの種別：MacroKiosk</summary>
		public const string GLOBAL_SMS_TYPE_MACROKIOSK = "MacroKiosk";

		/// <summary>WebキャプチャーAPI URL</summary>
		public static string WEB_BROWSER_CAPTURE_API_URL = "";

		/// <summary>カート投入URLでの定期購入配送パターンエリア非表示設定</summary>
		public static bool HIDE_FIXEDPURCHASE_SHIPPING_PATTERN_AREA_IN_ADD_CART_URL_ENABLED = false;
		/// <summary>商品詳細ページでSEOタグとOGPを使えるか</summary>
		public static bool SEOTAG_AND_OGPTAG_IN_PRODUCTDETAIL_ENABLED = false;
		/// <summary>商品一覧ページでSEOタグを使えるか</summary>
		public static bool SEOTAG_IN_PRODUCTLIST_ENABLED = false;

		/// <summary>PCサイトのLP用ディレクトリパス</summary>
		public static string CMS_LANDING_PAGE_DIR_PATH_PC = "";
		/// <summary>SPサイトのLP用ディレクトリパス</summary>
		public static string CMS_LANDING_PAGE_DIR_PATH_SP = "";
		/// <summary>PCサイトのLP用テンプレートファイルパス</summary>
		public static string CMS_LANDING_PAGE_TEMPLATE_FILE_PATH_PC = "";
		/// <summary>SPサイトのLP用テンプレートファイルパス</summary>
		public static string CMS_LANDING_PAGE_TEMPLATE_FILE_PATH_SP = "";
		/// <summary>PCサイトのLP用 カスタムデザイン テンプレートファイルパス</summary>
		public static string CMS_LANDING_PAGE_CUSTOM_TEMPLATE_FILE_PATH_PC = "";
		/// <summary>SPサイトのLP用 カスタムデザイン テンプレートファイルパス</summary>
		public static string CMS_LANDING_PAGE_CUSTOM_TEMPLATE_FILE_PATH_SP = "";
		/// <summary>PCサイトのLP用URL</summary>
		public static string CMS_LANDING_PAGE_DIR_URL_PC = "";
		/// <summary>SPサイトのLP用URL</summary>
		public static string CMS_LANDING_PAGE_DIR_URL_SP = "";
		/// <summary>デフォルトブロックデザインのパス</summary>
		public static string CMS_LANDING_PAGE_DEFAULT_BLOCK_DESIGN_FILE_PATH = "";
		/// <summary>更新時にデザインファイルを再作成するかどうかパス</summary>
		public static bool CMS_LANDING_PAGE_RECREATE_DESIGN_FILE_ON_UPDATE = true;
		/// <summary> LPメンテツールの有効化</summary>
		public static bool CMS_LANDING_PAGE_ENABLE_MAINTENANCE_TOOL = false;
		/// <summary>LPビルダー カスタムデザインモードを利用するか</summary>
		public static bool CMS_LANDING_PAGE_USE_CUSTOM_DESIGN_MODE = false;

		/// <summary>保持するフロントバックアップの世代数</summary>
		public static int NUMBER_OF_GENERATIONS_HOLODING_FRONT_BACKUP = 3;

		/// <summary>プレビュー公開範囲設定機能</summary>
		public static bool PREVIEW_RELEASE_RANGE_ENABLE = false;

		/// <summary>集計SQL タイムアウト(ミリ秒)</summary>
		public const int AGGREGATE_SQL_TIME_OUT = 600;

		/// <summary> 新規受注登録内項目 サジェスト用クエリタイムアウト時間(秒)</summary>
		public static int ORDERREGISTINPUT_SUGGEST_QUERY_TIMEOUT = 10;

		/// <summary>CookieのSameSiteをLaxとするUAの正規表現</summary>
		public static string DISALLOW_SAMESITE_NONE_USERAGENTSPATTERN = "";

		/// <summary>Cart list landing page option</summary>
		public static bool CART_LIST_LP_OPTION = false;

		/// <summary>Taiwan country shipping enable</summary>
		public static bool TW_COUNTRY_SHIPPING_ENABLE = false;

		/// <summary>当日出荷締め時間設定</summary>
		public static bool TODAY_SHIPPABLE_DEADLINE_TIME = false;

		/// <summary>Exchange Web Services URL</summary>
		public static string EXCHANGE_WEB_SERVICES_URL = string.Empty;
		/// <summary>Exchange Web Services scopes</summary>
		public static string[] EXCHANGE_WEB_SERVICES_SCOPES = new string[0];

		/// <summary>メール：ListUnsubscribeオプション</summary>
		public static bool MAIL_LISTUNSUBSCRIBE_OPTION_ENABLED = false;
		/// <summary>メール：ListUnsubscribe  POST URL</summary>
		public static string MAIL_LISTUNSUBSCRIBE_URL = "Form/Mail/Unsubscribe.aspx";
		/// <summary>メール：ListUnsubscribe  リクエストキー：ユーザーID</summary>
		public static string MAIL_LISTUNSUBSCRIBE_REQUEST_KEY_USER_ID = "uid";
		/// <summary>メール：ListUnsubscribe  リクエストキー：検証キー</summary>
		public static string MAIL_LISTUNSUBSCRIBE_REQUEST_KEY_VERIFICATION_KEY = "vkey";
		/// <summary>メール：ListUnsubscribe  メール送信先</summary>
		public static string MAIL_LISTUNSUBSCRIBE_MAILTO = "";
		/// <summary>メール：DKIMオプション</summary>
		public static bool MAIL_DKIM_OPTION_ENABLED = false;
		/// <summary>メール：DKIM  セレクタ</summary>
		public static string MAIL_DKIM_SELECTOR = "";
	}
}
