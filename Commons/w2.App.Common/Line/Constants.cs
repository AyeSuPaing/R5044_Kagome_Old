/*
=========================================================================================================
  Module      : 定数定義(Constants.cs)
 ････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Line
{
	/// <summary>
	/// 定数定義
	/// </summary>
	public class Constants
	{
		/// <summary>API接続URL</summary>
		public static string LINE_API_URL_ROOT_PATH = "";
		/// <summary>認証鍵</summary>
		public static string LINE_API_AUTHENTICATION_KEY = "";
		/// <summary>ログファイル出力先フォルダパス</summary>
		public static string DIRECTORY_LINE_API_LOGFILEPATH = "";
		/// <summary>ログファイル拡張子</summary>
		public static string LOGFILE_EXTENSION = "log";
		/// <summary>APIログファイル名 接頭辞</summary>
		public static string LINE_API_LOGFILE_NAME_PREFIX = "LineApi";
		/// <summary>ログファイル文字コード</summary>
		public static string LOGFILE_ENCODING = "shift_jis";
		/// <summary>APIログファイル サイズ閾値（MB）</summary>
		public static int LINE_API_LOGFILE_THRESHOLD = 100;
		/// <summary>APIログレベル（INFO:開始終了のみ、ERROR：エラー時詳細出力、TRACE:常に詳細出力）</summary>
		public static string LINE_API_LOG_LEVEL = "TRACE";
		/// <summary>ログレベル 実行ログのみ</summary>
		public const string LINE_API_FLG_LOG_LEVEL_INFO = "INFO";
		/// <summary>ログレベル エラー時詳細出力</summary>
		public const string LINE_API_FLG_LOG_LEVEL_ERROR = "ERROR";
		/// <summary>ログレベル 常に詳細出力</summary>
		public const string LINE_API_FLG_LOG_LEVEL_TRACE = "TRACE";


		/// <summary>オプション：LINE直接連携</summary>
		public static bool LINE_DIRECT_OPTION_ENABLED = false;
		/// <summary>LINE直接連携：直接連携時URL</summary>
		public const string LINE_DIRECT_CONNECT_PATH = "https://access.line.me/oauth2/v2.1/authorize";
		/// <summary>LINE直接連携：直接連携時アクセストークン取得URL</summary>
		public const string LINE_DIRECT_CONNECT_REQUEST_ACCESS_TOKEN_PATH = "https://api.line.me/oauth2/v2.1/token";
		/// <summary>LINE直接連携：直接連携時LINEユーザーID取得URL</summary>
		public const string LINE_DIRECT_CONNECT_REQUEST_LINE_PROVIDER_ID_PATH = "https://api.line.me/v2/profile";
		/// <summary>LINE直接連携：LINE Messaginga API URL</summary>
		public const string LINE_DIRECT_MESSAGING_API_URL = "https://api.line.me/v2/bot/message/";

		/// <summary>LINE直接連携：スコープ範囲(リクエストキー)</summary>
		public const string LINE_DIRECT_CONNECT_REQUEST_KEY_SCOPE = "scope";
		/// <summary>LINE直接連携：認可コード(リクエストキー)</summary>
		public const string LINE_DIRECT_CONNECT_REQUEST_KEY_CODE = "code";
		/// <summary>LINE直接連携：レスポンスタイプ(リクエストキー)</summary>
		public const string LINE_DIRECT_CONNECT_REQUEST_KEY_RESPONSE_TYPE = "response_type";
		/// <summary>LINE直接連携：認可コード(リクエストキー)</summary>
		public const string LINE_DIRECT_CONNECT_REQUEST_KEY_STATE = "state";
		/// <summary>LINE直接連携：友達承認形式(リクエストキー)</summary>
		public const string LINE_DIRECT_CONNECT_REQUEST_KEY_BOT_PROMPT = "bot_prompt";
		/// <summary>LINE直接連携：承諾型(リクエストキー)</summary>
		public const string LINE_DIRECT_CONNECT_REQUEST_KEY_GRANT_TYPE = "grant_type";
		/// <summary>LINE直接連携：コールバックURL(リクエストキー)</summary>
		public const string LINE_DIRECT_CONNECT_REQUEST_KEY_REDIRECT_URI = "redirect_uri";
		/// <summary>LINE直接連携：チャネルID(リクエストキー)</summary>
		public const string LINE_DIRECT_CONNECT_REQUEST_KEY_CLIENT_ID = "client_id";
		/// <summary>LINE直接連携：チャネルシークレットID(リクエストキー)</summary>
		public const string LINE_DIRECT_CONNECT_REQUEST_KEY_CLIENT_SECRET = "client_secret";
		/// <summary>LINE直接連携：アクセストークン(リクエストキー)</summary>
		public const string LINE_DIRECT_CONNECT_JSON_REQUEST_KEY_ACCESS_TOKEN = "access_token";
		/// <summary>LINE直接連携：ユーザーID(リクエストキー)</summary>
		public const string LINE_DIRECT_CONNECT_JSON_REQUEST_KEY_USER_ID = "userId";

		/// <summary>LINE直接連携：スコープ範囲</summary>
		public const string LINE_DIRECT_CONNECT_REQUEST_VALUE_SCOPE = "profile";
		/// <summary>LINE直接連携：友達承認形式</summary>
		public static string LINE_DIRECT_CONNECT_REQUEST_VALUE_BOT_PROMPT = "normal";
		/// <summary>LINE直接連携：承諾型</summary>
		public const string LINE_DIRECT_CONNECT_REQUEST_VALUE_GRANT_TYPE = "authorization_code";
		/// <summary>LINE直接連携：コールバックURL</summary>
		public static string LINE_DIRECT_CONNECT_REQUEST_VALUE_REDIRECT_URI = "";
		/// <summary>LINE直接連携：チャネルID</summary>
		public static string LINE_DIRECT_CONNECT_CLIENT_ID = "";
		/// <summary>LINE直接連携：チャネルシークレットID</summary>
		public static string LINE_DIRECT_CONNECT_CLIENT_SECRET = "";

		/// <summary>LINE直接連携：チャネルアクセストークン</summary>
		public static string LINE_DIRECT_CHANNEL_ACCESS_TOKEN = "";
		/// <summary>LINE直接連携：最大メッセージ作成数</summary>
		public static int LINE_DIRECT_MAX_MESSAGE_COUNT = 0;
		/// <summary>LINE直接連携：自動ログイン</summary>
		public static bool LINE_DIRECT_AUTO_LOGIN_OPTION = false;
		/// <summary>LINE直接連携：自動ログイン時のユーザ名</summary>
		public static string LINE_DIRECT_AUTO_LOGIN_USER_NAME = "LINE会員";

		/// <summary>LINE連携：テンプレートID</summary>
		public static int LINE_API_TEMPLATE_ID = 1;

		/// <summary>オプション：LINEミニアプリ</summary>
		public static bool LINE_MINIAPP_OPTION_ENABLED = false;
		/// <summary>LINEミニアプリ：LiffアプリID</summary>
		public static string LINE_MINIAPP_LIFF_ID = string.Empty;
	}
}
