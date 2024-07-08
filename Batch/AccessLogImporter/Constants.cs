/*
=========================================================================================================
  Module      : 定数設定(Constants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.Common.Net.Mail;

namespace w2.MarketingPlanner.Batch.AccessLogImporter
{
	/// <summary>
	/// Constants の概要の説明です。
	/// </summary>
	public class Constants : w2.App.Common.Constants
	{
		//*****************************************************************************************
		// 設定系
		//*****************************************************************************************
		// global.asaで設定
		public static string TARGETURL_LOGIMPORT = "";	// 画像ファイル名など

		// W3Cログ取得元ディレクトリ
		public static List<string> W3CLOG_DIRECTORIES = new List<string>();
		public static string IMPORTLOG_HEADER = "";

		// W3Cログ取込ディレクトリ
		public static string PATH_W3CLOG_IMPORT = "";
		public static string PATH_W3CLOG_ACTIVE = "";
		public static string PATH_W3CLOG_COMPLETE = "";
		public static string PATH_W3CLOG_ERROR = "";

		// ログ保持月数
		public static int LOGSTOCK_MONTHS = 3;

		// SQLタイムアウト値
		public static int SQLTIMEOUT_SEC = 3600;

		public static List<string> PROCESSACCESSLOGSETTINGS = new List<string>();
		public static bool ACCESSLOG_TARGET_PC = false;

		//*****************************************************************************************
		// 定数系
		//*****************************************************************************************
		// W3C拡張ログカラム名
		public const string W3CLOG_COLUMN_DATE = "date";
		public const string W3CLOG_COLUMN_TIME = "time";
		public const string W3CLOG_COLUMN_CLIENTIP = "c-ip";
		public const string W3CLOG_COLUMN_SERVERNAME = "s-computername";
		public const string W3CLOG_COLUMN_SERVERIP = "s-ip";
		public const string W3CLOG_COLUMN_SERVER_PORT = "s-port";
		public const string W3CLOG_COLUMN_PROTOCOLSTAT = "sc-status";
		public const string W3CLOG_COLUMN_USERAGENT = "cs(User-Agent)";
		public const string W3CLOG_COLUMN_URL = "cs(Referer)";
		public const string W3CLOG_COLUMN_PARAMS = "cs-uri-query";

		// ログトラッカーパラメータ名
		public const string W3CLOG_PARAM_ACCOUNTID = "__account_id";
		public const string W3CLOG_PARAM_ACCESSUSERID = "__access_user_id";
		public const string W3CLOG_PARAM_SESSIONID = "__session_id";
		public const string W3CLOG_PARAM_REALUSERID = "__real_user_id";
		public const string W3CLOG_PARAM_ACS_INTERVAL = "__acs_interval";
		public const string W3CLOG_PARAM_URLDOMAIN = "__url_domain";
		public const string W3CLOG_PARAM_URLPAGE = "__url_page";
		public const string W3CLOG_PARAM_URLPARAM = "__url_param";
		public const string W3CLOG_PARAM_FIRSTLOGINFLG = "__first_login_flg";
		public const string W3CLOG_PARAM_REFERER = "__referrer";
		public const string W3CLOG_PARAM_SRCHENGN = "__srch_engn";
		public const string W3CLOG_PARAM_SRCHWORD = "__srch_word";
		public const string W3CLOG_PARAM_ACTIONKBN = "__action_kbn";

		public const string W3CLOG_PARAM_S_HEAD = "__s_";
		public const string W3CLOG_PARAM_P_HEAD = "__p_";

		// アクセスログテーブルインサートフィールド（チェック用）
		public static string[] ACCESSLOG_INSERT_FIELDS = {
													   Constants.FIELD_ACCESSLOG_ACCESS_DATE,
													   Constants.FIELD_ACCESSLOG_ACCESS_TIME,
													   Constants.FIELD_ACCESSLOG_CLIENT_IP,
													   Constants.FIELD_ACCESSLOG_SERVER_NAME,
													   Constants.FIELD_ACCESSLOG_SERVER_IP,
													   Constants.FIELD_ACCESSLOG_SERVER_PORT,
													   Constants.FIELD_ACCESSLOG_PROTOCOL_STATUS,
													   Constants.FIELD_ACCESSLOG_USER_AGENT,
													   Constants.FIELD_ACCESSLOG_URL_DOMAIN,
													   Constants.FIELD_ACCESSLOG_URL_PAGE,
													   Constants.FIELD_ACCESSLOG_URL_PARAM,
													   Constants.FIELD_ACCESSLOG_ACCOUNT_ID,
													   Constants.FIELD_ACCESSLOG_ACCESS_USER_ID,
													   Constants.FIELD_ACCESSLOG_SESSION_ID,
													   Constants.FIELD_ACCESSLOG_REAL_USER_ID,
													   Constants.FIELD_ACCESSLOG_ACCESS_INTERVAL,
													   Constants.FIELD_ACCESSLOG_FIRST_LOGIN_FLG,
													   Constants.FIELD_ACCESSLOG_REFERRER_DOMAIN,
													   Constants.FIELD_ACCESSLOG_REFERRER_PAGE,
													   Constants.FIELD_ACCESSLOG_REFERRER_PARAM,
													   Constants.FIELD_ACCESSLOG_SEARCH_ENGINE,
													   Constants.FIELD_ACCESSLOG_SEARCH_WORDS,
													   Constants.FIELD_ACCESSLOG_ACTION_KBN,
													   Constants.FIELD_ACCESSLOG_S1,
													   Constants.FIELD_ACCESSLOG_S2,
													   Constants.FIELD_ACCESSLOG_S3,
													   Constants.FIELD_ACCESSLOG_S4,
													   Constants.FIELD_ACCESSLOG_S5,
													   Constants.FIELD_ACCESSLOG_S6,
													   Constants.FIELD_ACCESSLOG_S7,
													   Constants.FIELD_ACCESSLOG_S8,
													   Constants.FIELD_ACCESSLOG_S9,
													   Constants.FIELD_ACCESSLOG_S10,
													   Constants.FIELD_ACCESSLOG_S11,
													   Constants.FIELD_ACCESSLOG_S12,
													   Constants.FIELD_ACCESSLOG_S13,
													   Constants.FIELD_ACCESSLOG_S14,
													   Constants.FIELD_ACCESSLOG_S15,
													   Constants.FIELD_ACCESSLOG_S16,
													   Constants.FIELD_ACCESSLOG_S17,
													   Constants.FIELD_ACCESSLOG_S18,
													   Constants.FIELD_ACCESSLOG_S19,
													   Constants.FIELD_ACCESSLOG_S20,
													   Constants.FIELD_ACCESSLOG_P1,
													   Constants.FIELD_ACCESSLOG_P2,
													   Constants.FIELD_ACCESSLOG_P3,
													   Constants.FIELD_ACCESSLOG_P4,
													   Constants.FIELD_ACCESSLOG_P5,
													   Constants.FIELD_ACCESSLOG_P6,
													   Constants.FIELD_ACCESSLOG_P7,
													   Constants.FIELD_ACCESSLOG_P8,
													   Constants.FIELD_ACCESSLOG_P9,
													   Constants.FIELD_ACCESSLOG_P10,
													   Constants.FIELD_ACCESSLOG_P11,
													   Constants.FIELD_ACCESSLOG_P12,
													   Constants.FIELD_ACCESSLOG_P13,
													   Constants.FIELD_ACCESSLOG_P14,
													   Constants.FIELD_ACCESSLOG_P15,
													   Constants.FIELD_ACCESSLOG_P16,
													   Constants.FIELD_ACCESSLOG_P17,
													   Constants.FIELD_ACCESSLOG_P18,
													   Constants.FIELD_ACCESSLOG_P19,
													   Constants.FIELD_ACCESSLOG_P20
		};
	}
}
