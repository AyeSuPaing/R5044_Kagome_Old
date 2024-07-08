/*
=========================================================================================================
  Module      : DB定数定義MarketingPlanner部分(Constants_MarketingPlanner.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.Database.Common
{
	///*********************************************************************************************
	/// <summary>
	/// DB定数定義MarketingPlanner部分
	/// </summary>
	///*********************************************************************************************
	public partial class Constants : w2.Common.Constants
	{
		#region テーブル・フィールド定数

		// アクセスログテーブル
		public const string TABLE_ACCESSLOG = "w2_AccessLog";                                       // アクセスログテーブル
		public const string FIELD_ACCESSLOG_LOG_NO = "log_no";                                      // ログno
		public const string FIELD_ACCESSLOG_ACCESS_DATE = "access_date";                            // 日付
		public const string FIELD_ACCESSLOG_ACCESS_TIME = "access_time";                            // 時間
		public const string FIELD_ACCESSLOG_CLIENT_IP = "client_ip";                                // クライアント IP アドレス
		public const string FIELD_ACCESSLOG_SERVER_NAME = "server_name";                            // サーバー名
		public const string FIELD_ACCESSLOG_SERVER_IP = "server_ip";                                // サーバー IP アドレス
		public const string FIELD_ACCESSLOG_SERVER_PORT = "server_port";                            // サーバー ポート
		public const string FIELD_ACCESSLOG_PROTOCOL_STATUS = "protocol_status";                    // プロトコルの状態
		public const string FIELD_ACCESSLOG_USER_AGENT = "user_agent";                              // ユーザー エージェント
		public const string FIELD_ACCESSLOG_URL_DOMAIN = "url_domain";                              // アクセスURL・ドメイン
		public const string FIELD_ACCESSLOG_URL_PAGE = "url_page";                                  // アクセスURL・ページパス
		public const string FIELD_ACCESSLOG_URL_PARAM = "url_param";                                // アクセスURL・パラメタ
		public const string FIELD_ACCESSLOG_DEPT_ID = "dept_id";                                    // 識別ID
		public const string FIELD_ACCESSLOG_ACCOUNT_ID = "account_id";                              // アカウントID
		public const string FIELD_ACCESSLOG_ACCESS_USER_ID = "access_user_id";                      // アクセスユーザID
		public const string FIELD_ACCESSLOG_SESSION_ID = "session_id";                              // セッションID
		public const string FIELD_ACCESSLOG_REAL_USER_ID = "real_user_id";                          // ユーザID
		public const string FIELD_ACCESSLOG_ACCESS_INTERVAL = "access_interval";                    // アクセス頻度
		public const string FIELD_ACCESSLOG_FIRST_LOGIN_FLG = "first_login_flg";                    // 初回ログインフラグ
		public const string FIELD_ACCESSLOG_REFERRER_DOMAIN = "referrer_domain";                    // リファラー・ドメイン
		public const string FIELD_ACCESSLOG_REFERRER_PAGE = "referrer_page";                        // リファラー・ページパス
		public const string FIELD_ACCESSLOG_REFERRER_PARAM = "referrer_param";                      // リファラー・パラメタ
		public const string FIELD_ACCESSLOG_SEARCH_ENGINE = "search_engine";                        // 検索エンジン
		public const string FIELD_ACCESSLOG_SEARCH_WORDS = "search_words";                          // 検索ワード
		public const string FIELD_ACCESSLOG_ACTION_KBN = "action_kbn";                              // アクション区分
		public const string FIELD_ACCESSLOG_S1 = "s1";                                              // システム予約パラメタ1
		public const string FIELD_ACCESSLOG_S2 = "s2";                                              // システム予約パラメタ2
		public const string FIELD_ACCESSLOG_S3 = "s3";                                              // システム予約パラメタ3
		public const string FIELD_ACCESSLOG_S4 = "s4";                                              // システム予約パラメタ4
		public const string FIELD_ACCESSLOG_S5 = "s5";                                              // システム予約パラメタ5
		public const string FIELD_ACCESSLOG_S6 = "s6";                                              // システム予約パラメタ6
		public const string FIELD_ACCESSLOG_S7 = "s7";                                              // システム予約パラメタ7
		public const string FIELD_ACCESSLOG_S8 = "s8";                                              // システム予約パラメタ8
		public const string FIELD_ACCESSLOG_S9 = "s9";                                              // システム予約パラメタ9
		public const string FIELD_ACCESSLOG_S10 = "s10";                                            // システム予約パラメタ10
		public const string FIELD_ACCESSLOG_S11 = "s11";                                            // システム予約パラメタ11
		public const string FIELD_ACCESSLOG_S12 = "s12";                                            // システム予約パラメタ12
		public const string FIELD_ACCESSLOG_S13 = "s13";                                            // システム予約パラメタ13
		public const string FIELD_ACCESSLOG_S14 = "s14";                                            // システム予約パラメタ14
		public const string FIELD_ACCESSLOG_S15 = "s15";                                            // システム予約パラメタ15
		public const string FIELD_ACCESSLOG_S16 = "s16";                                            // システム予約パラメタ16
		public const string FIELD_ACCESSLOG_S17 = "s17";                                            // システム予約パラメタ17
		public const string FIELD_ACCESSLOG_S18 = "s18";                                            // システム予約パラメタ18
		public const string FIELD_ACCESSLOG_S19 = "s19";                                            // システム予約パラメタ19
		public const string FIELD_ACCESSLOG_S20 = "s20";                                            // システム予約パラメタ20
		public const string FIELD_ACCESSLOG_P1 = "p1";                                              // ユーザ定義パラメタ1
		public const string FIELD_ACCESSLOG_P2 = "p2";                                              // ユーザ定義パラメタ2
		public const string FIELD_ACCESSLOG_P3 = "p3";                                              // ユーザ定義パラメタ3
		public const string FIELD_ACCESSLOG_P4 = "p4";                                              // ユーザ定義パラメタ4
		public const string FIELD_ACCESSLOG_P5 = "p5";                                              // ユーザ定義パラメタ5
		public const string FIELD_ACCESSLOG_P6 = "p6";                                              // ユーザ定義パラメタ6
		public const string FIELD_ACCESSLOG_P7 = "p7";                                              // ユーザ定義パラメタ7
		public const string FIELD_ACCESSLOG_P8 = "p8";                                              // ユーザ定義パラメタ8
		public const string FIELD_ACCESSLOG_P9 = "p9";                                              // ユーザ定義パラメタ9
		public const string FIELD_ACCESSLOG_P10 = "p10";                                            // ユーザ定義パラメタ10
		public const string FIELD_ACCESSLOG_P11 = "p11";                                            // ユーザ定義パラメタ11
		public const string FIELD_ACCESSLOG_P12 = "p12";                                            // ユーザ定義パラメタ12
		public const string FIELD_ACCESSLOG_P13 = "p13";                                            // ユーザ定義パラメタ13
		public const string FIELD_ACCESSLOG_P14 = "p14";                                            // ユーザ定義パラメタ14
		public const string FIELD_ACCESSLOG_P15 = "p15";                                            // ユーザ定義パラメタ15
		public const string FIELD_ACCESSLOG_P16 = "p16";                                            // ユーザ定義パラメタ16
		public const string FIELD_ACCESSLOG_P17 = "p17";                                            // ユーザ定義パラメタ17
		public const string FIELD_ACCESSLOG_P18 = "p18";                                            // ユーザ定義パラメタ18
		public const string FIELD_ACCESSLOG_P19 = "p19";                                            // ユーザ定義パラメタ19
		public const string FIELD_ACCESSLOG_P20 = "p20";                                            // ユーザ定義パラメタ20
		public const string FIELD_ACCESSLOG_USER_AGENT_KBN = "user_agent_kbn";                      // ユーザー エージェント区分





		// モバイルアクセスログテーブル
		public const string TABLE_ACCESSLOGMOBILE = "w2_AccessLogMobile";                           // モバイルアクセスログテーブル
		public const string FIELD_ACCESSLOGMOBILE_LOG_NO = "log_no";                                // ログno
		public const string FIELD_ACCESSLOGMOBILE_ACCESS_DATE = "access_date";                      // 日付
		public const string FIELD_ACCESSLOGMOBILE_ACCESS_TIME = "access_time";                      // 時間
		public const string FIELD_ACCESSLOGMOBILE_CLIENT_IP = "client_ip";                          // クライアント IP アドレス
		public const string FIELD_ACCESSLOGMOBILE_SERVER_NAME = "server_name";                      // サーバー名
		public const string FIELD_ACCESSLOGMOBILE_SERVER_IP = "server_ip";                          // サーバー IP アドレス
		public const string FIELD_ACCESSLOGMOBILE_SERVER_PORT = "server_port";                      // サーバー ポート
		public const string FIELD_ACCESSLOGMOBILE_PROTOCOL_STATUS = "protocol_status";              // プロトコルの状態
		public const string FIELD_ACCESSLOGMOBILE_MOBILE_CAREER_CODE = "mobile_career_code";        // モバイルキャリアコード
		public const string FIELD_ACCESSLOGMOBILE_MOBILE_MODEL_NAME = "mobile_model_name";          // モバイル機種名
		public const string FIELD_ACCESSLOGMOBILE_MOBILE_MODEL_CODE = "mobile_model_code";          // モバイル機種コード
		public const string FIELD_ACCESSLOGMOBILE_USER_AGENT = "user_agent";                        // ユーザー エージェント
		public const string FIELD_ACCESSLOGMOBILE_URL_DOMAIN = "url_domain";                        // アクセスURL・ドメイン
		public const string FIELD_ACCESSLOGMOBILE_URL_PAGE = "url_page";                            // アクセスURL・ページパス
		public const string FIELD_ACCESSLOGMOBILE_URL_PARAM = "url_param";                          // アクセスURL・パラメタ
		public const string FIELD_ACCESSLOGMOBILE_DEPT_ID = "dept_id";                              // 識別ID
		public const string FIELD_ACCESSLOGMOBILE_SESSION_ID = "session_id";                        // セッションID
		public const string FIELD_ACCESSLOGMOBILE_REAL_USER_ID = "real_user_id";                    // ユーザID
		public const string FIELD_ACCESSLOGMOBILE_REFERRER_DOMAIN = "referrer_domain";              // リファラー・ドメイン
		public const string FIELD_ACCESSLOGMOBILE_REFERRER_PAGE = "referrer_page";                  // リファラー・ページパス
		public const string FIELD_ACCESSLOGMOBILE_REFERRER_PARAM = "referrer_param";                // リファラー・パラメタ
		public const string FIELD_ACCESSLOGMOBILE_SEARCH_ENGINE = "search_engine";                  // 検索エンジン
		public const string FIELD_ACCESSLOGMOBILE_SEARCH_WORDS = "search_words";                    // 検索ワード
		public const string FIELD_ACCESSLOGMOBILE_ACTION_KBN = "action_kbn";                        // アクション区分
		public const string FIELD_ACCESSLOGMOBILE_S1 = "s1";                                        // システム予約パラメタ1
		public const string FIELD_ACCESSLOGMOBILE_S2 = "s2";                                        // システム予約パラメタ2
		public const string FIELD_ACCESSLOGMOBILE_S3 = "s3";                                        // システム予約パラメタ3
		public const string FIELD_ACCESSLOGMOBILE_S4 = "s4";                                        // システム予約パラメタ4
		public const string FIELD_ACCESSLOGMOBILE_S5 = "s5";                                        // システム予約パラメタ5
		public const string FIELD_ACCESSLOGMOBILE_S6 = "s6";                                        // システム予約パラメタ6
		public const string FIELD_ACCESSLOGMOBILE_S7 = "s7";                                        // システム予約パラメタ7
		public const string FIELD_ACCESSLOGMOBILE_S8 = "s8";                                        // システム予約パラメタ8
		public const string FIELD_ACCESSLOGMOBILE_S9 = "s9";                                        // システム予約パラメタ9
		public const string FIELD_ACCESSLOGMOBILE_S10 = "s10";                                      // システム予約パラメタ10
		public const string FIELD_ACCESSLOGMOBILE_S11 = "s11";                                      // システム予約パラメタ11
		public const string FIELD_ACCESSLOGMOBILE_S12 = "s12";                                      // システム予約パラメタ12
		public const string FIELD_ACCESSLOGMOBILE_S13 = "s13";                                      // システム予約パラメタ13
		public const string FIELD_ACCESSLOGMOBILE_S14 = "s14";                                      // システム予約パラメタ14
		public const string FIELD_ACCESSLOGMOBILE_S15 = "s15";                                      // システム予約パラメタ15
		public const string FIELD_ACCESSLOGMOBILE_S16 = "s16";                                      // システム予約パラメタ16
		public const string FIELD_ACCESSLOGMOBILE_S17 = "s17";                                      // システム予約パラメタ17
		public const string FIELD_ACCESSLOGMOBILE_S18 = "s18";                                      // システム予約パラメタ18
		public const string FIELD_ACCESSLOGMOBILE_S19 = "s19";                                      // システム予約パラメタ19
		public const string FIELD_ACCESSLOGMOBILE_S20 = "s20";                                      // システム予約パラメタ20
		public const string FIELD_ACCESSLOGMOBILE_P1 = "p1";                                        // ユーザ定義パラメタ1
		public const string FIELD_ACCESSLOGMOBILE_P2 = "p2";                                        // ユーザ定義パラメタ2
		public const string FIELD_ACCESSLOGMOBILE_P3 = "p3";                                        // ユーザ定義パラメタ3
		public const string FIELD_ACCESSLOGMOBILE_P4 = "p4";                                        // ユーザ定義パラメタ4
		public const string FIELD_ACCESSLOGMOBILE_P5 = "p5";                                        // ユーザ定義パラメタ5
		public const string FIELD_ACCESSLOGMOBILE_P6 = "p6";                                        // ユーザ定義パラメタ6
		public const string FIELD_ACCESSLOGMOBILE_P7 = "p7";                                        // ユーザ定義パラメタ7
		public const string FIELD_ACCESSLOGMOBILE_P8 = "p8";                                        // ユーザ定義パラメタ8
		public const string FIELD_ACCESSLOGMOBILE_P9 = "p9";                                        // ユーザ定義パラメタ9
		public const string FIELD_ACCESSLOGMOBILE_P10 = "p10";                                      // ユーザ定義パラメタ10
		public const string FIELD_ACCESSLOGMOBILE_P11 = "p11";                                      // ユーザ定義パラメタ11
		public const string FIELD_ACCESSLOGMOBILE_P12 = "p12";                                      // ユーザ定義パラメタ12
		public const string FIELD_ACCESSLOGMOBILE_P13 = "p13";                                      // ユーザ定義パラメタ13
		public const string FIELD_ACCESSLOGMOBILE_P14 = "p14";                                      // ユーザ定義パラメタ14
		public const string FIELD_ACCESSLOGMOBILE_P15 = "p15";                                      // ユーザ定義パラメタ15
		public const string FIELD_ACCESSLOGMOBILE_P16 = "p16";                                      // ユーザ定義パラメタ16
		public const string FIELD_ACCESSLOGMOBILE_P17 = "p17";                                      // ユーザ定義パラメタ17
		public const string FIELD_ACCESSLOGMOBILE_P18 = "p18";                                      // ユーザ定義パラメタ18
		public const string FIELD_ACCESSLOGMOBILE_P19 = "p19";                                      // ユーザ定義パラメタ19
		public const string FIELD_ACCESSLOGMOBILE_P20 = "p20";                                      // ユーザ定義パラメタ20

		// アクセスログアカウントテーブル
		public const string TABLE_ACCESSLOGACCOUNT = "w2_AccessLogAccount";                         // アクセスログアカウントテーブル
		public const string FIELD_ACCESSLOGACCOUNT_ACCOUNT_ID = "account_id";                       // アカウントID
		public const string FIELD_ACCESSLOGACCOUNT_DEPT_ID = "dept_id";                             // 識別ID
		public const string FIELD_ACCESSLOGACCOUNT_DATE_CREATED = "date_created";                   // 作成日
		public const string FIELD_ACCESSLOGACCOUNT_DATE_CHANGED = "date_changed";                   // 更新日

		// アクセスログ処理ステータス
		public const string TABLE_ACCESSLOGSTATUS = "w2_AccessLogStatus";                           // アクセスログ処理ステータス
		public const string FIELD_ACCESSLOGSTATUS_TARGET_DATE = "target_date";                      // 対象日付
		public const string FIELD_ACCESSLOGSTATUS_DAY_STATUS = "day_status";                        // 日次ステータス
		public const string FIELD_ACCESSLOGSTATUS_MONTH_STATUS = "month_status";                    // 月次ステータス
		public const string FIELD_ACCESSLOGSTATUS_IMPORT_FILES = "import_files";                    // 取込完了ファイル数
		public const string FIELD_ACCESSLOGSTATUS_IMPORT_FILES_TOTAL = "import_files_total";        // 取込予定ファイル数
		public const string FIELD_ACCESSLOGSTATUS_TARGET_FILENAME = "target_filename";              // 最終取込ファイル名
		public const string FIELD_ACCESSLOGSTATUS_DATE_CHANGED = "date_changed";                    // 更新日

		// アクセスユーザマスタ
		public const string TABLE_ACCESSUSERMASTER = "w2_AccessUserMaster";                         // アクセスユーザマスタ
		public const string FIELD_ACCESSUSERMASTER_DEPT_ID = "dept_id";                             // 識別ID
		public const string FIELD_ACCESSUSERMASTER_ACCESS_USER_ID = "access_user_id";               // アクセスユーザID
		public const string FIELD_ACCESSUSERMASTER_USER_ID = "user_id";                             // ユーザID
		public const string FIELD_ACCESSUSERMASTER_FIRST_ACC_DATE = "first_acc_date";               // 初回アクセス日
		public const string FIELD_ACCESSUSERMASTER_LAST_ACC_DATE = "last_acc_date";                 // 最終アクセス日
		public const string FIELD_ACCESSUSERMASTER_RECOGNIZED_DATE = "recognized_date";             // 認知日

		// 認知ユーザマスタ
		public const string TABLE_ACCESSRECUSERMASTER = "w2_AccessRecUserMaster";                   // 認知ユーザマスタ
		public const string FIELD_ACCESSRECUSERMASTER_DEPT_ID = "dept_id";                          // 識別ID
		public const string FIELD_ACCESSRECUSERMASTER_USER_ID = "user_id";                          // ユーザID
		public const string FIELD_ACCESSRECUSERMASTER_RECOGNIZED_DATE = "recognized_date";          // 認知日
		public const string FIELD_ACCESSRECUSERMASTER_LAST_ACC_DATE = "last_acc_date";              // 最終アクセス日
		public const string FIELD_ACCESSRECUSERMASTER_LAST_LOGIN_DATE = "last_login_date";          // 最終ログイン日
		public const string FIELD_ACCESSRECUSERMASTER_LEAVE_DATE = "leave_date";                    // 退会日

		// 検索ワード履歴
		public const string TABLE_PRODUCTSEARCHWORDHISTORY = "w2_ProductSearchWordHistory";         // 検索ワード履歴
		public const string FIELD_PRODUCTSEARCHWORDHISTORY_HISTORY_NO = "history_no";               // 検索ワード履歴NO
		public const string FIELD_PRODUCTSEARCHWORDHISTORY_DEPT_ID = "dept_id";                     // 店舗ID
		public const string FIELD_PRODUCTSEARCHWORDHISTORY_ACCESS_KBN = "access_kbn";               // アクセス区分
		public const string FIELD_PRODUCTSEARCHWORDHISTORY_SEARCH_WORD = "search_word";             // 検索ワード
		public const string FIELD_PRODUCTSEARCHWORDHISTORY_HITS = "hits";                           // ヒット数
		public const string FIELD_PRODUCTSEARCHWORDHISTORY_DATE_CREATED = "date_created";           // 作成日

		// アクセス解析結果テーブル
		public const string TABLE_DISPACCESSANALYSIS = "w2_DispAccessAnalysis";                     // アクセス解析結果テーブル
		public const string FIELD_DISPACCESSANALYSIS_DEPT_ID = "dept_id";                           // 識別ID
		public const string FIELD_DISPACCESSANALYSIS_TGT_YEAR = "tgt_year";                         // 対象年
		public const string FIELD_DISPACCESSANALYSIS_TGT_MONTH = "tgt_month";                       // 対象月
		public const string FIELD_DISPACCESSANALYSIS_TGT_DAY = "tgt_day";                           // 対象日
		public const string FIELD_DISPACCESSANALYSIS_TOTAL_PAGE_VIEWS = "total_page_views";         // ページビュー数
		public const string FIELD_DISPACCESSANALYSIS_TOTAL_USERS = "total_users";                   // 訪問ユーザ数
		public const string FIELD_DISPACCESSANALYSIS_TOTAL_SESSIONS = "total_sessions";             // 訪問セッション数
		public const string FIELD_DISPACCESSANALYSIS_TOTAL_NEW_USERS = "total_new_users";           // 新規訪問ユーザ数
		public const string FIELD_DISPACCESSANALYSIS_TOTAL_NEW_SESSIONS = "total_new_sessions";     // 新規訪問数
		public const string FIELD_DISPACCESSANALYSIS_RESERVED1 = "reserved1";                       // 予備値1
		public const string FIELD_DISPACCESSANALYSIS_RESERVED2 = "reserved2";                       // 予備値2
		public const string FIELD_DISPACCESSANALYSIS_RESERVED3 = "reserved3";                       // 予備値3
		public const string FIELD_DISPACCESSANALYSIS_RESERVED4 = "reserved4";                       // 予備値4
		public const string FIELD_DISPACCESSANALYSIS_RESERVED5 = "reserved5";                       // 予備値5

		// ユーザ解析結果テーブル
		public const string TABLE_DISPUSERANALYSIS = "w2_DispUserAnalysis";                         // ユーザ解析結果テーブル
		public const string FIELD_DISPUSERANALYSIS_DEPT_ID = "dept_id";                             // 識別ID
		public const string FIELD_DISPUSERANALYSIS_TGT_YEAR = "tgt_year";                           // 対象年
		public const string FIELD_DISPUSERANALYSIS_TGT_MONTH = "tgt_month";                         // 対象月
		public const string FIELD_DISPUSERANALYSIS_TGT_DAY = "tgt_day";                             // 対象日
		public const string FIELD_DISPUSERANALYSIS_POTENTIAL_NEW = "potential_new";                 // 潜在ユーザ新規獲得数
		public const string FIELD_DISPUSERANALYSIS_POTENTIAL_ALL = "potential_all";                 // 潜在ユーザ全体数
		public const string FIELD_DISPUSERANALYSIS_POTENTIAL_ACTIVE = "potential_active";           // 潜在アクティブユーザ数
		public const string FIELD_DISPUSERANALYSIS_POTENTIAL_UNACTIVE1 = "potential_unactive1";     // 潜在休眠ユーザ数１
		public const string FIELD_DISPUSERANALYSIS_POTENTIAL_UNACTIVE2 = "potential_unactive2";     // 潜在休眠ユーザ数２
		public const string FIELD_DISPUSERANALYSIS_RECOGNIZE_NEW = "recognize_new";                 // 認知顧客新規獲得数
		public const string FIELD_DISPUSERANALYSIS_RECOGNIZE_ALL = "recognize_all";                 // 認知顧客全体数
		public const string FIELD_DISPUSERANALYSIS_RECOGNIZE_ACTIVE = "recognize_active";           // 認知アクティブ顧客数
		public const string FIELD_DISPUSERANALYSIS_RECOGNIZE_UNACTIVE1 = "recognize_unactive1";     // 認知休眠顧客数１
		public const string FIELD_DISPUSERANALYSIS_RECOGNIZE_UNACTIVE2 = "recognize_unactive2";     // 認知休眠顧客数２
		public const string FIELD_DISPUSERANALYSIS_LEAVE_NEW = "leave_new";                         // 退会顧客新規獲得数
		public const string FIELD_DISPUSERANALYSIS_LEAVE_ALL = "leave_all";                         // 退会顧客全体
		public const string FIELD_DISPUSERANALYSIS_RESERVED1 = "reserved1";                         // 予備値1
		public const string FIELD_DISPUSERANALYSIS_RESERVED2 = "reserved2";                         // 予備値2
		public const string FIELD_DISPUSERANALYSIS_RESERVED3 = "reserved3";                         // 予備値3
		public const string FIELD_DISPUSERANALYSIS_RESERVED4 = "reserved4";                         // 予備値4
		public const string FIELD_DISPUSERANALYSIS_RESERVED5 = "reserved5";                         // 予備値5

		// サマリ分析結果テーブル
		public const string TABLE_DISPSUMMARYANALYSIS = "w2_DispSummaryAnalysis";                   // サマリ分析結果テーブル
		public const string FIELD_DISPSUMMARYANALYSIS_DATA_NO = "data_no";                          // データ番号
		public const string FIELD_DISPSUMMARYANALYSIS_DEPT_ID = "dept_id";                          // 識別ID
		public const string FIELD_DISPSUMMARYANALYSIS_SUMMARY_KBN = "summary_kbn";                  // 集計区分
		public const string FIELD_DISPSUMMARYANALYSIS_TGT_YEAR = "tgt_year";                        // 対象年
		public const string FIELD_DISPSUMMARYANALYSIS_TGT_MONTH = "tgt_month";                      // 対象月
		public const string FIELD_DISPSUMMARYANALYSIS_TGT_DAY = "tgt_day";                          // 対象日
		public const string FIELD_DISPSUMMARYANALYSIS_VALUE_NAME = "value_name";                    // 項目名
		public const string FIELD_DISPSUMMARYANALYSIS_COUNTS = "counts";                            // カウント数
		public const string FIELD_DISPSUMMARYANALYSIS_RESERVED1 = "reserved1";                      // 予備値1
		public const string FIELD_DISPSUMMARYANALYSIS_RESERVED2 = "reserved2";                      // 予備値2
		public const string FIELD_DISPSUMMARYANALYSIS_RESERVED3 = "reserved3";                      // 予備値3
		public const string FIELD_DISPSUMMARYANALYSIS_RESERVED4 = "reserved4";                      // 予備値4
		public const string FIELD_DISPSUMMARYANALYSIS_RESERVED5 = "reserved5";                      // 予備値5
		public const string FIELD_DISPSUMMARYANALYSIS_RESERVED6 = "reserved6";                      // 予備値6
		public const string FIELD_DISPSUMMARYANALYSIS_RESERVED7 = "reserved7";                      // 予備値7
		public const string FIELD_DISPSUMMARYANALYSIS_RESERVED8 = "reserved8";                      // 予備値8
		public const string FIELD_DISPSUMMARYANALYSIS_RESERVED9 = "reserved9";                      // 予備値9
		public const string FIELD_DISPSUMMARYANALYSIS_RESERVED10 = "reserved10";                    // 予備値10
		public const string FIELD_DISPSUMMARYANALYSIS_PRICE = "price";                              // 金額
		public const string FIELD_DISPSUMMARYANALYSIS_PRICE_TAX = "price_tax";                      // 税金

		// ポイントマスタ
		public const string TABLE_POINT = "w2_Point";                                               // ポイントマスタ
		public const string FIELD_POINT_DEPT_ID = "dept_id";                                        // 識別ID
		public const string FIELD_POINT_POINT_KBN = "point_kbn";                                    // ポイント区分
		public const string FIELD_POINT_POINT_EXP_KBN = "point_exp_kbn";                            // ポイント有効期限設定
		public const string FIELD_POINT_USABLE_UNIT = "usable_unit";                                // ポイント利用可能単位
		public const string FIELD_POINT_EXCHANGE_RATE = "exchange_rate";                            // ポイント換算率
		public const string FIELD_POINT_KBN1 = "kbn1";                                              // 予備区分1
		public const string FIELD_POINT_KBN2 = "kbn2";                                              // 予備区分2
		public const string FIELD_POINT_KBN3 = "kbn3";                                              // 予備区分3
		public const string FIELD_POINT_KBN4 = "kbn4";                                              // 予備区分4
		public const string FIELD_POINT_KBN5 = "kbn5";                                              // 予備区分5
		public const string FIELD_POINT_DATE_CREATED = "date_created";                              // 作成日
		public const string FIELD_POINT_DATE_CHANGED = "date_changed";                              // 更新日
		public const string FIELD_POINT_LAST_CHANGED = "last_changed";                              // 最終更新者

		// ポイントルールマスタ
		public const string TABLE_POINTRULE = "w2_PointRule";                                       // ポイントルールマスタ
		public const string FIELD_POINTRULE_DEPT_ID = "dept_id";                                    // 識別ID
		public const string FIELD_POINTRULE_POINT_RULE_ID = "point_rule_id";                        // ポイントルールID
		public const string FIELD_POINTRULE_POINT_RULE_NAME = "point_rule_name";                    // ポイントルール名
		public const string FIELD_POINTRULE_POINT_RULE_KBN = "point_rule_kbn";                      // ポイントルール区分
		public const string FIELD_POINTRULE_POINT_KBN = "point_kbn";                                // 対象ポイント区分
		public const string FIELD_POINTRULE_USE_TEMP_FLG = "use_temp_flg";                          // 仮ポイント使用フラグ
		public const string FIELD_POINTRULE_POINT_INC_KBN = "point_inc_kbn";                        // ポイント加算区分
		public const string FIELD_POINTRULE_INC_TYPE = "inc_type";                                  // ポイント加算方法
		public const string FIELD_POINTRULE_INC_NUM = "inc_num";                                    // ポイント加算数
		public const string FIELD_POINTRULE_INC_RATE = "inc_rate";                                  // ポイント加算率
		public const string FIELD_POINTRULE_POINT_EXP_EXTEND = "point_exp_extend";                  // ポイント有効期限延長
		public const string FIELD_POINTRULE_EXP_BGN = "exp_bgn";                                    // 有効期間（開始）
		public const string FIELD_POINTRULE_EXP_END = "exp_end";                                    // 有効期間（終了）
		public const string FIELD_POINTRULE_CAMPAIGN_TERM_KBN = "campaign_term_kbn";                // キャンペーン期間区分
		public const string FIELD_POINTRULE_CAMPAIGN_TERM_VALUE = "campaign_term_value";            // キャンペーン期間値
		public const string FIELD_POINTRULE_PRIORITY = "priority";                                  // 優先順位
		public const string FIELD_POINTRULE_VALID_FLG = "valid_flg";                                // 有効フラグ
		public const string FIELD_POINTRULE_KBN1 = "kbn1";                                          // 予備区分1
		public const string FIELD_POINTRULE_KBN2 = "kbn2";                                          // 予備区分2
		public const string FIELD_POINTRULE_KBN3 = "kbn3";                                          // 予備区分3
		public const string FIELD_POINTRULE_KBN4 = "kbn4";                                          // 予備区分4
		public const string FIELD_POINTRULE_KBN5 = "kbn5";                                          // 予備区分5
		public const string FIELD_POINTRULE_DATE_CREATED = "date_created";                          // 作成日
		public const string FIELD_POINTRULE_DATE_CHANGED = "date_changed";                          // 更新日
		public const string FIELD_POINTRULE_LAST_CHANGED = "last_changed";                          // 最終更新者
		public const string FIELD_POINTRULE_EFFECTIVE_OFFSET = "effective_offset";                  // 期間限定ポイント発効オフセット
		public const string FIELD_POINTRULE_EFFECTIVE_OFFSET_TYPE = "effective_offset_type";        // 期間限定ポイント発効オフセット種別
		public const string FIELD_POINTRULE_TERM = "term";                                          // 期間限定ポイント有効期間（相対）
		public const string FIELD_POINTRULE_TERM_TYPE = "term_type";                                // 期間限定ポイント有効期間（相対）種別
		public const string FIELD_POINTRULE_PERIOD_BEGIN = "period_begin";                          // 期間限定ポイント有効期間（絶対）開始日
		public const string FIELD_POINTRULE_PERIOD_END = "period_end";                              // 期間限定ポイント有効期間（絶対）終了日
		public const string FIELD_POINTRULE_ALLOW_DUPLICATE_APPLY_FLG = "allow_duplicate_apply_flg";// 基本ルールとの二重適用を許可するか
		public const string FIELD_POINTRULE_FIXED_PURCHASE_INC_TYPE = "fixed_purchase_inc_type";    // 定期ポイント加算方法
		public const string FIELD_POINTRULE_FIXED_PURCHASE_INC_NUM = "fixed_purchase_inc_num";      // 定期ポイント加算数
		public const string FIELD_POINTRULE_FIXED_PURCHASE_INC_RATE = "fixed_purchase_inc_rate";    // 定期ポイント加算率

		// ポイントルール日付マスタ
		public const string TABLE_POINTRULEDATE = "w2_PointRuleDate";                               // ポイントルール日付マスタ
		public const string FIELD_POINTRULEDATE_DEPT_ID = "dept_id";                                // 識別ID
		public const string FIELD_POINTRULEDATE_POINT_RULE_ID = "point_rule_id";                    // ポイントルールID
		public const string FIELD_POINTRULEDATE_TGT_DATE = "tgt_date";                              // 対象日付

		// ポイントルールスケジュールテーブル
		public const string TABLE_POINTRULESCHEDULE = "w2_PointRuleSchedule";                       // ポイントルールスケジュールテーブル
		public const string FIELD_POINTRULESCHEDULE_POINT_RULE_SCHEDULE_ID = "point_rule_schedule_id";// ポイントルールスケジュールID
		public const string FIELD_POINTRULESCHEDULE_POINT_RULE_SCHEDULE_NAME = "point_rule_schedule_name";// ポイントルールスケジュール名
		public const string FIELD_POINTRULESCHEDULE_STATUS = "status";                              // ステータス
		public const string FIELD_POINTRULESCHEDULE_LAST_COUNT = "last_count";                      // 最終付与人数
		public const string FIELD_POINTRULESCHEDULE_LAST_EXEC_DATE = "last_exec_date";              // 最終付与日時
		public const string FIELD_POINTRULESCHEDULE_TARGET_ID = "target_id";                        // ターゲットID
		public const string FIELD_POINTRULESCHEDULE_TARGET_EXTRACT_FLG = "target_extract_flg";      // ターゲット抽出フラグ
		public const string FIELD_POINTRULESCHEDULE_POINT_RULE_ID = "point_rule_id";                // ポイントルールID
		public const string FIELD_POINTRULESCHEDULE_MAIL_ID = "mail_id";                            // メールテンプレートID
		public const string FIELD_POINTRULESCHEDULE_EXEC_TIMING = "exec_timing";                    // 実行タイミング
		public const string FIELD_POINTRULESCHEDULE_SCHEDULE_KBN = "schedule_kbn";                  // スケジュール区分
		public const string FIELD_POINTRULESCHEDULE_SCHEDULE_DAY_OF_WEEK = "schedule_day_of_week";  // スケジュール曜日
		public const string FIELD_POINTRULESCHEDULE_SCHEDULE_YEAR = "schedule_year";                // スケジュール日程(年)
		public const string FIELD_POINTRULESCHEDULE_SCHEDULE_MONTH = "schedule_month";              // スケジュール日程(月)
		public const string FIELD_POINTRULESCHEDULE_SCHEDULE_DAY = "schedule_day";                  // スケジュール日程(日)
		public const string FIELD_POINTRULESCHEDULE_SCHEDULE_HOUR = "schedule_hour";                // スケジュール日程(時)
		public const string FIELD_POINTRULESCHEDULE_SCHEDULE_MINUTE = "schedule_minute";            // スケジュール日程(分)
		public const string FIELD_POINTRULESCHEDULE_SCHEDULE_SECOND = "schedule_second";            // スケジュール日程(秒)
		public const string FIELD_POINTRULESCHEDULE_VALID_FLG = "valid_flg";                        // 有効フラグ
		public const string FIELD_POINTRULESCHEDULE_DATE_CREATED = "date_created";                  // 作成日
		public const string FIELD_POINTRULESCHEDULE_DATE_CHANGED = "date_changed";                  // 更新日
		public const string FIELD_POINTRULESCHEDULE_LAST_CHANGED = "last_changed";                  // 最終更新者

		// ユーザポイントマスタ
		public const string TABLE_USERPOINT = "w2_UserPoint";                                       // ユーザポイントマスタ
		public const string FIELD_USERPOINT_USER_ID = "user_id";                                    // ユーザID
		public const string FIELD_USERPOINT_POINT_KBN = "point_kbn";                                // ポイント区分
		public const string FIELD_USERPOINT_POINT_KBN_NO = "point_kbn_no";                          // 枝番
		public const string FIELD_USERPOINT_DEPT_ID = "dept_id";                                    // 識別ID
		public const string FIELD_USERPOINT_POINT_RULE_ID = "point_rule_id";                        // ポイントルールID
		public const string FIELD_USERPOINT_POINT_RULE_KBN = "point_rule_kbn";                      // ポイントルール区分
		public const string FIELD_USERPOINT_POINT_TYPE = "point_type";                              // ポイント種別
		public const string FIELD_USERPOINT_POINT_INC_KBN = "point_inc_kbn";                        // ポイント加算区分
		public const string FIELD_USERPOINT_POINT = "point";                                        // ポイント数(更新)
		public const string FIELD_USERPOINT_ADD_POINT = "add_point";                                // ポイント数(追加)
		public const string FIELD_USERPOINT_POINT_EXP = "point_exp";                                // 有効期限
		public const string FIELD_USERPOINT_HISTORY_NO = "history_no";                              // 枝番（ユーザポイント履歴用）
		public const string FIELD_USERPOINT_KBN1 = "kbn1";                                          // 予備区分1
		public const string FIELD_USERPOINT_KBN2 = "kbn2";                                          // 予備区分2
		public const string FIELD_USERPOINT_KBN3 = "kbn3";                                          // 予備区分3
		public const string FIELD_USERPOINT_KBN4 = "kbn4";                                          // 予備区分4
		public const string FIELD_USERPOINT_KBN5 = "kbn5";                                          // 予備区分5
		public const string FIELD_USERPOINT_DATE_CREATED = "date_created";                          // 作成日
		public const string FIELD_USERPOINT_DATE_CHANGED = "date_changed";                          // 更新日
		public const string FIELD_USERPOINT_LAST_CHANGED = "last_changed";                          // 最終更新者
		public const string FIELD_USERPOINT_EFFECTIVE_DATE = "effective_date";                      // ポイント発効日

		// ユーザポイント履歴
		public const string TABLE_USERPOINTHISTORY = "w2_UserPointHistory";                         // ユーザポイント履歴
		public const string FIELD_USERPOINTHISTORY_USER_ID = "user_id";                             // ユーザID
		public const string FIELD_USERPOINTHISTORY_HISTORY_NO = "history_no";                       // 枝番
		public const string FIELD_USERPOINTHISTORY_TGT_YEAR = "tgt_year";                           // 対象年
		public const string FIELD_USERPOINTHISTORY_TGT_MONTH = "tgt_month";                         // 対象月
		public const string FIELD_USERPOINTHISTORY_TGT_DAY = "tgt_day";                             // 対象日
		public const string FIELD_USERPOINTHISTORY_DEPT_ID = "dept_id";                             // 識別ID
		public const string FIELD_USERPOINTHISTORY_POINT_RULE_ID = "point_rule_id";                 // ポイントルールID
		public const string FIELD_USERPOINTHISTORY_POINT_RULE_KBN = "point_rule_kbn";               // ポイントルール区分
		public const string FIELD_USERPOINTHISTORY_POINT_KBN = "point_kbn";                         // ポイント区分
		public const string FIELD_USERPOINTHISTORY_POINT_TYPE = "point_type";                       // ポイント種別
		public const string FIELD_USERPOINTHISTORY_POINT_INC_KBN = "point_inc_kbn";                 // ポイント加算区分
		public const string FIELD_USERPOINTHISTORY_POINT_INC = "point_inc";                         // ポイント加算数
		public const string FIELD_USERPOINTHISTORY_POINT_EXP_EXTEND = "point_exp_extend";           // ポイント有効期限延長
		public const string FIELD_USERPOINTHISTORY_USER_POINT_EXP = "user_point_exp";               // ユーザ最新有効期限
		public const string FIELD_USERPOINTHISTORY_KBN1 = "kbn1";                                   // 予備区分1
		public const string FIELD_USERPOINTHISTORY_KBN2 = "kbn2";                                   // 予備区分2
		public const string FIELD_USERPOINTHISTORY_KBN3 = "kbn3";                                   // 予備区分3
		public const string FIELD_USERPOINTHISTORY_KBN4 = "kbn4";                                   // 予備区分4
		public const string FIELD_USERPOINTHISTORY_KBN5 = "kbn5";                                   // 予備区分5
		public const string FIELD_USERPOINTHISTORY_MEMO = "memo";                                   // メモ
		public const string FIELD_USERPOINTHISTORY_DATE_CREATED = "date_created";                   // 作成日
		public const string FIELD_USERPOINTHISTORY_LAST_CHANGED = "last_changed";                   // 最終更新者
		public const string FIELD_USERPOINTHISTORY_EFFECTIVE_DATE = "effective_date";               // ポイント発効日
		public const string FIELD_USERPOINTHISTORY_RESTORED_FLG = "restored_flg";                   // 復元処理済フラグ
		public const string FIELD_USERPOINTHISTORY_HISTORY_GROUP_NO = "history_group_no";           // 履歴グループ番号
		/// <summary>カートID</summary>
		public const string FIELD_USERPOINTHISTORY_CART_ID = "cart_id";

		// クーポンテーブル
		public const string TABLE_COUPON = "w2_Coupon";                                             // クーポンテーブル
		public const string FIELD_COUPON_DEPT_ID = "dept_id";                                       // 識別ID
		public const string FIELD_COUPON_COUPON_ID = "coupon_id";                                   // クーポンID
		public const string FIELD_COUPON_COUPON_CODE = "coupon_code";                               // クーポンコード
		public const string FIELD_COUPON_COUPON_NAME = "coupon_name";                               // 管理用クーポン名
		public const string FIELD_COUPON_COUPON_DISP_NAME = "coupon_disp_name";                     // 表示用クーポン名
		public const string FIELD_COUPON_COUPON_DISP_NAME_MOBILE = "coupon_disp_name_mobile";       // モバイル用表示用クーポン名
		public const string FIELD_COUPON_COUPON_DISCRIPTION = "coupon_discription";                 // クーポン説明文
		public const string FIELD_COUPON_COUPON_DISP_DISCRIPTION = "coupon_disp_discription";       // クーポン説明文(ユーザ表示用)
		public const string FIELD_COUPON_COUPON_DISCRIPTION_MOBILE = "coupon_discription_mobile";   // モバイル用クーポン説明文
		public const string FIELD_COUPON_COUPON_TYPE = "coupon_type";                               // クーポン種別
		public const string FIELD_COUPON_COUPON_COUNT = "coupon_count";                             // クーポン利用可能回数
		public const string FIELD_COUPON_PUBLISH_DATE_BGN = "publish_date_bgn";                     // クーポン発行期間(開始)
		public const string FIELD_COUPON_PUBLISH_DATE_END = "publish_date_end";                     // クーポン発行期間(終了)
		public const string FIELD_COUPON_DISCOUNT_PRICE = "discount_price";                         // クーポン商品割引額
		public const string FIELD_COUPON_DISCOUNT_RATE = "discount_rate";                           // クーポン商品割引率
		public const string FIELD_COUPON_EXPIRE_DAY = "expire_day";                                 // クーポン有効期限(日)
		public const string FIELD_COUPON_EXPIRE_DATE_BGN = "expire_date_bgn";                       // クーポン有効期間(開始)
		public const string FIELD_COUPON_EXPIRE_DATE_END = "expire_date_end";                       // クーポン有効期間(終了)
		public const string FIELD_COUPON_PRODUCT_KBN = "product_kbn";                               // クーポン対象商品区分
		public const string FIELD_COUPON_EXCEPTIONAL_PRODUCT = "exceptional_product";               // クーポン例外商品
		public const string FIELD_COUPON_EXCEPTIONAL_ICON = "exceptional_icon";                     // クーポン例外商品アイコン
		public const string FIELD_COUPON_USABLE_PRICE = "usable_price";                             // クーポン利用最低購入金額
		public const string FIELD_COUPON_USE_TOGETHER_FLG = "use_together_flg";                     // クーポン併用フラグ
		public const string FIELD_COUPON_VALID_FLG = "valid_flg";                                   // 有効フラグ
		public const string FIELD_COUPON_DISP_FLG = "disp_flg";                                     // ユーザ表示フラグ
		public const string FIELD_COUPON_DATE_CREATED = "date_created";                             // 作成日
		public const string FIELD_COUPON_DATE_CHANGED = "date_changed";                             // 更新日
		public const string FIELD_COUPON_LAST_CHANGED = "last_changed";                             // 最終更新者
		public const string FIELD_COUPON_FREE_SHIPPING_FLG = "free_shipping_flg";                   // クーポン配送料無料フラグ
		public const string FIELD_COUPON_EXCEPTIONAL_BRAND_IDS = "exceptional_brand_ids";           // クーポン例外ブランドID
		public const string FIELD_COUPON_EXCEPTIONAL_PRODUCT_CATEGORY_IDS = "exceptional_product_category_ids";// クーポン例外商品カテゴリID

		// ユーザクーポンテーブル
		public const string TABLE_USERCOUPON = "w2_UserCoupon";                                     // ユーザクーポンテーブル
		public const string FIELD_USERCOUPON_USER_ID = "user_id";                                   // ユーザID
		public const string FIELD_USERCOUPON_DEPT_ID = "dept_id";                                   // 識別ID
		public const string FIELD_USERCOUPON_COUPON_ID = "coupon_id";                               // クーポンID
		public const string FIELD_USERCOUPON_COUPON_NO = "coupon_no";                               // 枝番
		public const string FIELD_USERCOUPON_ORDER_ID = "order_id";                                 // 注文ID
		public const string FIELD_USERCOUPON_USE_FLG = "use_flg";                                   // 利用フラグ
		public const string FIELD_USERCOUPON_DATE_CREATED = "date_created";                         // 作成日
		public const string FIELD_USERCOUPON_DATE_CHANGED = "date_changed";                         // 更新日
		public const string FIELD_USERCOUPON_LAST_CHANGED = "last_changed";                         // 最終更新者
		public const string FIELD_USERCOUPON_USER_COUPON_COUNT = "user_coupon_count";               // ユーザークーポン利用可能回数

		// クーポン発行スケジュールテーブル
		public const string TABLE_COUPONSCHEDULE = "w2_CouponSchedule";                             // クーポン発行スケジュールテーブル
		public const string FIELD_COUPONSCHEDULE_COUPON_SCHEDULE_ID = "coupon_schedule_id";         // クーポン発行スケジュールID
		public const string FIELD_COUPONSCHEDULE_COUPON_SCHEDULE_NAME = "coupon_schedule_name";     // クーポン発行スケジュール名
		public const string FIELD_COUPONSCHEDULE_STATUS = "status";                                 // ステータス
		public const string FIELD_COUPONSCHEDULE_LAST_COUNT = "last_count";                         // 最終付与人数
		public const string FIELD_COUPONSCHEDULE_LAST_EXEC_DATE = "last_exec_date";                 // 最終付与日時
		public const string FIELD_COUPONSCHEDULE_TARGET_ID = "target_id";                           // ターゲットID
		public const string FIELD_COUPONSCHEDULE_TARGET_EXTRACT_FLG = "target_extract_flg";         // ターゲット抽出フラグ
		public const string FIELD_COUPONSCHEDULE_COUPON_ID = "coupon_id";                           // クーポン発行ID
		public const string FIELD_COUPONSCHEDULE_PUBLISH_QUANTITY = "publish_quantity";             // クーポン発行枚数
		public const string FIELD_COUPONSCHEDULE_MAIL_ID = "mail_id";                               // メールテンプレートID
		public const string FIELD_COUPONSCHEDULE_EXEC_TIMING = "exec_timing";                       // 実行タイミング
		public const string FIELD_COUPONSCHEDULE_SCHEDULE_KBN = "schedule_kbn";                     // スケジュール区分
		public const string FIELD_COUPONSCHEDULE_SCHEDULE_DAY_OF_WEEK = "schedule_day_of_week";     // スケジュール曜日
		public const string FIELD_COUPONSCHEDULE_SCHEDULE_YEAR = "schedule_year";                   // スケジュール日程(年)
		public const string FIELD_COUPONSCHEDULE_SCHEDULE_MONTH = "schedule_month";                 // スケジュール日程(月)
		public const string FIELD_COUPONSCHEDULE_SCHEDULE_DAY = "schedule_day";                     // スケジュール日程(日)
		public const string FIELD_COUPONSCHEDULE_SCHEDULE_HOUR = "schedule_hour";                   // スケジュール日程(時)
		public const string FIELD_COUPONSCHEDULE_SCHEDULE_MINUTE = "schedule_minute";               // スケジュール日程(分)
		public const string FIELD_COUPONSCHEDULE_SCHEDULE_SECOND = "schedule_second";               // スケジュール日程(秒)
		public const string FIELD_COUPONSCHEDULE_VALID_FLG = "valid_flg";                           // 有効フラグ
		public const string FIELD_COUPONSCHEDULE_DATE_CREATED = "date_created";                     // 作成日
		public const string FIELD_COUPONSCHEDULE_DATE_CHANGED = "date_changed";                     // 更新日
		public const string FIELD_COUPONSCHEDULE_LAST_CHANGED = "last_changed";                     // 最終更新者

		// ユーザクーポン履歴テーブル
		public const string TABLE_USERCOUPONHISTORY = "w2_UserCouponHistory";                       // ユーザクーポン履歴テーブル
		public const string FIELD_USERCOUPONHISTORY_USER_ID = "user_id";                            // ユーザID
		public const string FIELD_USERCOUPONHISTORY_HISTORY_NO = "history_no";                      // 枝番
		public const string FIELD_USERCOUPONHISTORY_DEPT_ID = "dept_id";                            // 識別ID
		public const string FIELD_USERCOUPONHISTORY_COUPON_ID = "coupon_id";                        // クーポンID
		public const string FIELD_USERCOUPONHISTORY_COUPON_CODE = "coupon_code";                    // クーポンコード
		public const string FIELD_USERCOUPONHISTORY_ORDER_ID = "order_id";                          // 注文ID
		public const string FIELD_USERCOUPONHISTORY_HISTORY_KBN = "history_kbn";                    // クーポン履歴区分
		public const string FIELD_USERCOUPONHISTORY_ACTION_KBN = "action_kbn";                      // 操作区分
		public const string FIELD_USERCOUPONHISTORY_COUPON_INC = "coupon_inc";                      // 加算数
		public const string FIELD_USERCOUPONHISTORY_COUPON_PRICE = "coupon_price";                  // クーポン金額
		public const string FIELD_USERCOUPONHISTORY_MEMO = "memo";                                  // メモ
		public const string FIELD_USERCOUPONHISTORY_DATE_CREATED = "date_created";                  // 作成日
		public const string FIELD_USERCOUPONHISTORY_LAST_CHANGED = "last_changed";                  // 最終更新者
		public const string FIELD_USERCOUPONHISTORY_USER_COUPON_COUNT = "user_coupon_count";        // ユーザークーポン利用可能回数
		public const string FIELD_USERCOUPONHISTORY_FIXED_PURCHASE_ID = "fixed_purchase_id";        // 定期購入ID

		// クーポン利用ユーザーテーブル
		public const string TABLE_COUPONUSEUSER = "w2_CouponUseUser";                               // クーポン利用ユーザーテーブル
		public const string FIELD_COUPONUSEUSER_COUPON_ID = "coupon_id";                            // クーポンID
		public const string FIELD_COUPONUSEUSER_COUPON_USE_USER = "coupon_use_user";                 // クーポン利用ユーザー(メールアドレスorユーザーID)
		public const string FIELD_COUPONUSEUSER_ORDER_ID = "order_id";                              // 注文ID
		public const string FIELD_COUPONUSEUSER_DATE_CREATED = "date_created";                      // 作成日
		public const string FIELD_COUPONUSEUSER_LAST_CHANGED = "last_changed";                      // 最終更新者
		public const string FIELD_COUPONUSEUSER_FIXED_PURCHASE_ID = "fixed_purchase_id";            // 定期購入ID

		// クーポン利用ユーザーワークテーブル
		public const string TABLE_WORKCOUPONUSEUSER = "w2_WorkCouponUseUser";                       // クーポン利用ユーザーワークテーブル
		public const string FIELD_WORKCOUPONUSEUSER_COUPON_ID = "coupon_id";                        // クーポンID
		public const string FIELD_WORKCOUPONUSEUSER_COUPON_USE_USER = "coupon_use_user";            // クーポン利用ユーザー(メールアドレスorユーザーID)
		public const string FIELD_WORKCOUPONUSEUSER_ORDER_ID = "order_id";                          // 注文ID
		public const string FIELD_WORKCOUPONUSEUSER_DATE_CREATED = "date_created";                  // 作成日
		public const string FIELD_WORKCOUPONUSEUSER_LAST_CHANGED = "last_changed";                  // 最終更新者
		public const string FIELD_WORKCOUPONUSEUSER_FIXED_PURCHASE_ID = "fixed_purchase_id";        // 定期購入ID

		// ターゲットリスト設定マスタ
		public const string TABLE_TARGETLIST = "w2_TargetList";                                     // ターゲットリスト設定マスタ
		public const string FIELD_TARGETLIST_DEPT_ID = "dept_id";                                   // 識別ID
		public const string FIELD_TARGETLIST_TARGET_ID = "target_id";                               // ターゲットリストID
		public const string FIELD_TARGETLIST_TARGET_NAME = "target_name";                           // ターゲットリスト名
		public const string FIELD_TARGETLIST_STATUS = "status";                                     // ステータス
		public const string FIELD_TARGETLIST_TARGET_TYPE = "target_type";                           // ターゲット種別
		public const string FIELD_TARGETLIST_TARGET_CONDITION = "target_condition";                 // 抽出条件
		public const string FIELD_TARGETLIST_DATA_COUNT = "data_count";                             // 前回抽出件数
		public const string FIELD_TARGETLIST_DATA_COUNT_DATE = "data_count_date";                   // 前回抽出日付
		public const string FIELD_TARGETLIST_EXEC_TIMING = "exec_timing";                           // 実行タイミング
		public const string FIELD_TARGETLIST_SCHEDULE_KBN = "schedule_kbn";                         // スケジュール区分
		public const string FIELD_TARGETLIST_SCHEDULE_DAY_OF_WEEK = "schedule_day_of_week";         // スケジュール曜日
		public const string FIELD_TARGETLIST_SCHEDULE_YEAR = "schedule_year";                       // スケジュール日程(年)
		public const string FIELD_TARGETLIST_SCHEDULE_MONTH = "schedule_month";                     // スケジュール日程(月)
		public const string FIELD_TARGETLIST_SCHEDULE_DAY = "schedule_day";                         // スケジュール日程(日)
		public const string FIELD_TARGETLIST_SCHEDULE_HOUR = "schedule_hour";                       // スケジュール日程(時)
		public const string FIELD_TARGETLIST_SCHEDULE_MINUTE = "schedule_minute";                   // スケジュール日程(分)
		public const string FIELD_TARGETLIST_SCHEDULE_SECOND = "schedule_second";                   // スケジュール日程(秒)
		public const string FIELD_TARGETLIST_VALID_FLG = "valid_flg";                               // 有効フラグ
		public const string FIELD_TARGETLIST_DEL_FLG = "del_flg";                                   // 削除フラグ
		public const string FIELD_TARGETLIST_DATE_CREATED = "date_created";                         // 作成日
		public const string FIELD_TARGETLIST_DATE_CHANGED = "date_changed";                         // 更新日
		public const string FIELD_TARGETLIST_LAST_CHANGED = "last_changed";                         // 最終更新者

		// ターゲットリストデータ
		public const string TABLE_TARGETLISTDATA = "w2_TargetListData";                             // ターゲットリストデータ
		public const string FIELD_TARGETLISTDATA_DEPT_ID = "dept_id";                               // 識別ID
		public const string FIELD_TARGETLISTDATA_TARGET_KBN = "target_kbn";                         // ターゲットデータ区分
		public const string FIELD_TARGETLISTDATA_MASTER_ID = "master_id";                           // マスタID
		public const string FIELD_TARGETLISTDATA_DATA_NO = "data_no";                               // 枝番
		public const string FIELD_TARGETLISTDATA_USER_ID = "user_id";                               // ユーザID
		public const string FIELD_TARGETLISTDATA_MAIL_ADDR = "mail_addr";                           // メールアドレス
		public const string FIELD_TARGETLISTDATA_MAIL_ADDR_KBN = "mail_addr_kbn";                   // メールアドレス区分
		public const string FIELD_TARGETLISTDATA_DATE_CREATED = "date_created";                     // 作成日

		// メール配信　配信先テンポラリテーブル
		public const string TABLE_MAILSENDTEMP = "w2_MailSendTemp";                                 // メール配信　配信先テンポラリテーブル
		public const string FIELD_MAILSENDTEMP_DEPT_ID = "dept_id";                                 // 識別ID
		public const string FIELD_MAILSENDTEMP_MASTER_ID = "master_id";                             // マスタID
		public const string FIELD_MAILSENDTEMP_DATA_NO = "data_no";                                 // 枝番
		public const string FIELD_MAILSENDTEMP_USER_ID = "user_id";                                 // ユーザID
		public const string FIELD_MAILSENDTEMP_MAIL_ADDR = "mail_addr";                             // メールアドレス
		public const string FIELD_MAILSENDTEMP_MAIL_ADDR_KBN = "mail_addr_kbn";                     // メールアドレス区分
		public const string FIELD_MAILSENDTEMP_DATE_CREATED = "date_created";                       // 作成日

		// メール配信設定マスタ
		public const string TABLE_MAILDISTSETTING = "w2_MailDistSetting";                           // メール配信設定マスタ
		public const string FIELD_MAILDISTSETTING_DEPT_ID = "dept_id";                              // 識別ID
		public const string FIELD_MAILDISTSETTING_MAILDIST_ID = "maildist_id";                      // メール配信設定ID
		public const string FIELD_MAILDISTSETTING_MAILDIST_NAME = "maildist_name";                  // メール配信設定名
		public const string FIELD_MAILDISTSETTING_STATUS = "status";                                // ステータス
		public const string FIELD_MAILDISTSETTING_LAST_COUNT = "last_count";                        // 最終集計人数
		public const string FIELD_MAILDISTSETTING_LAST_ERROREXCEPT_COUNT = "last_errorexcept_count";// 最終エラー除外人数
		public const string FIELD_MAILDISTSETTING_LAST_MOBILEMAILEXCEPT_COUNT = "last_mobilemailexcept_count";// 最終モバイル除外人数
		public const string FIELD_MAILDISTSETTING_LAST_DIST_DATE = "last_dist_date";                // 最終配信開始日時
		public const string FIELD_MAILDISTSETTING_TARGET_ID = "target_id";                          // ターゲットID
		public const string FIELD_MAILDISTSETTING_TARGET_EXTRACT_FLG = "target_extract_flg";        // ターゲット抽出フラグ
		public const string FIELD_MAILDISTSETTING_TARGET_ID2 = "target_id2";                        // ターゲットID2
		public const string FIELD_MAILDISTSETTING_TARGET_EXTRACT_FLG2 = "target_extract_flg2";      // ターゲット2抽出フラグ
		public const string FIELD_MAILDISTSETTING_TARGET_ID3 = "target_id3";                        // ターゲットID3
		public const string FIELD_MAILDISTSETTING_TARGET_EXTRACT_FLG3 = "target_extract_flg3";      // ターゲット3抽出フラグ
		public const string FIELD_MAILDISTSETTING_TARGET_ID4 = "target_id4";                        // ターゲットID4
		public const string FIELD_MAILDISTSETTING_TARGET_EXTRACT_FLG4 = "target_extract_flg4";      // ターゲット4抽出フラグ
		public const string FIELD_MAILDISTSETTING_TARGET_ID5 = "target_id5";                        // ターゲットID5
		public const string FIELD_MAILDISTSETTING_TARGET_EXTRACT_FLG5 = "target_extract_flg5";      // ターゲット5抽出フラグ
		public const string FIELD_MAILDISTSETTING_EXCEPT_ERROR_POINT = "except_error_point";        // 配信除外エラーポイント
		public const string FIELD_MAILDISTSETTING_EXCEPT_MOBILEMAIL_FLG = "except_mobilemail_flg";  // モバイルメール排除フラグ
		public const string FIELD_MAILDISTSETTING_MAILTEXT_ID = "mailtext_id";                      // メール文章ID
		public const string FIELD_MAILDISTSETTING_EXEC_TIMING = "exec_timing";                      // 実行タイミング
		public const string FIELD_MAILDISTSETTING_SCHEDULE_KBN = "schedule_kbn";                    // スケジュール区分
		public const string FIELD_MAILDISTSETTING_SCHEDULE_DAY_OF_WEEK = "schedule_day_of_week";    // スケジュール曜日
		public const string FIELD_MAILDISTSETTING_SCHEDULE_YEAR = "schedule_year";                  // スケジュール日程(年)
		public const string FIELD_MAILDISTSETTING_SCHEDULE_MONTH = "schedule_month";                // スケジュール日程(月)
		public const string FIELD_MAILDISTSETTING_SCHEDULE_DAY = "schedule_day";                    // スケジュール日程(日)
		public const string FIELD_MAILDISTSETTING_SCHEDULE_HOUR = "schedule_hour";                  // スケジュール日程(時)
		public const string FIELD_MAILDISTSETTING_SCHEDULE_MINUTE = "schedule_minute";              // スケジュール日程(分)
		public const string FIELD_MAILDISTSETTING_SCHEDULE_SECOND = "schedule_second";              // スケジュール日程(秒)
		public const string FIELD_MAILDISTSETTING_VALID_FLG = "valid_flg";                          // 有効フラグ
		public const string FIELD_MAILDISTSETTING_DEL_FLG = "del_flg";                              // 削除フラグ
		public const string FIELD_MAILDISTSETTING_DATE_CREATED = "date_created";                    // 作成日
		public const string FIELD_MAILDISTSETTING_DATE_CHANGED = "date_changed";                    // 更新日
		public const string FIELD_MAILDISTSETTING_LAST_CHANGED = "last_changed";                    // 最終更新者
		public const string FIELD_MAILDISTSETTING_LAST_DUPLICATE_EXCEPT_COUNT = "last_duplicate_except_count";// 最終重複配信除外人数
		public const string FIELD_MAILDISTSETTING_ENABLE_DEDUPLICATION = "enable_deduplication";    // 重複配信除外設定

		// メール配信排除アドレス
		public const string TABLE_MAILDISTEXCEPTLIST = "w2_MailDistExceptList";                     // メール配信排除アドレス
		public const string FIELD_MAILDISTEXCEPTLIST_DEPT_ID = "dept_id";                           // 識別ID
		public const string FIELD_MAILDISTEXCEPTLIST_MAILDIST_ID = "maildist_id";                   // メール配信設定ID
		public const string FIELD_MAILDISTEXCEPTLIST_MAIL_ADDR = "mail_addr";                       // メールアドレス
		public const string FIELD_MAILDISTEXCEPTLIST_DATE_CREATED = "date_created";                 // 作成日

		// メール配信送信済ユーザ
		public const string TABLE_MAILDISTSENTUSER = "w2_MailDistSentUser";                         // メール配信送信済ユーザ
		public const string FIELD_MAILDISTSENTUSER_MAILDIST_ID = "maildist_id";                     // メール配信設定ID
		public const string FIELD_MAILDISTSENTUSER_USER_ID = "user_id";                             // ユーザID
		public const string FIELD_MAILDISTSENTUSER_DATE_CREATED = "date_created";                   // 作成日
		public const string FIELD_MAILDISTSENTUSER_DATE_CHANGED = "date_changed";                   // 更新日
		public const string FIELD_MAILDISTSENTUSER_LAST_CHANGED = "last_changed";                   // 最終更新者

		// メール配信文章マスタ
		public const string TABLE_MAILDISTTEXT = "w2_MailDistText";                                 // メール配信文章マスタ
		public const string FIELD_MAILDISTTEXT_DEPT_ID = "dept_id";                                 // 識別ID
		public const string FIELD_MAILDISTTEXT_MAILTEXT_ID = "mailtext_id";                         // メール文章ID
		public const string FIELD_MAILDISTTEXT_MAILTEXT_NAME = "mailtext_name";                     // メール文章名
		public const string FIELD_MAILDISTTEXT_MAILTEXT_SUBJECT = "mailtext_subject";               // メールタイトル
		public const string FIELD_MAILDISTTEXT_MAILTEXT_SUBJECT_MOBILE = "mailtext_subject_mobile"; // メールタイトルモバイル
		public const string FIELD_MAILDISTTEXT_MAILTEXT_BODY = "mailtext_body";                     // メール文章テキスト
		public const string FIELD_MAILDISTTEXT_MAILTEXT_HTML = "mailtext_html";                     // メール文章HTML
		public const string FIELD_MAILDISTTEXT_MAILTEXT_BODY_MOBILE = "mailtext_body_mobile";       // メール文章モバイル
		public const string FIELD_MAILDISTTEXT_MAILTEXT_DECOME = "mailtext_decome";                 // メール文章デコメ
		public const string FIELD_MAILDISTTEXT_SENDHTML_FLG = "sendhtml_flg";                       // HTMLメール送信フラグ
		public const string FIELD_MAILDISTTEXT_SENDDECOME_FLG = "senddecome_flg";                   // デコメ送信フラグ
		public const string FIELD_MAILDISTTEXT_MAIL_FROM = "mail_from";                             // メールFROM
		public const string FIELD_MAILDISTTEXT_MAIL_FROM_NAME = "mail_from_name";                   // メールFROM名
		public const string FIELD_MAILDISTTEXT_MAIL_CC = "mail_cc";                                 // メールCC
		public const string FIELD_MAILDISTTEXT_MAIL_BCC = "mail_bcc";                               // メールBCC
		public const string FIELD_MAILDISTTEXT_MAIL_ATTACHMENTFILE_PATH = "mail_attachmentfile_path";// 添付ファイル
		public const string FIELD_MAILDISTTEXT_DEL_FLG = "del_flg";                                 // 削除フラグ
		public const string FIELD_MAILDISTTEXT_DATE_CREATED = "date_created";                       // 作成日
		public const string FIELD_MAILDISTTEXT_DATE_CHANGED = "date_changed";                       // 更新日
		public const string FIELD_MAILDISTTEXT_LAST_CHANGED = "last_changed";                       // 最終更新者
		public const string FIELD_MAILDISTTEXT_LANGUAGE_CODE = "language_code";                     // 言語コード
		public const string FIELD_MAILDISTTEXT_LANGUAGE_LOCALE_ID = "language_locale_id";           // 言語ロケールID
		public const string FIELD_MAILDISTTEXT_SMS_USE_FLG = "sms_use_flg";                         // SMS利用フラグ
		public const string FIELD_MAILDISTTEXT_LINE_USE_FLG = "line_use_flg";                       // LINE直接連携利用フラグ

		// メールクリックマスタ
		public const string TABLE_MAILCLICK = "w2_MailClick";                                       // メールクリックマスタ
		public const string FIELD_MAILCLICK_DEPT_ID = "dept_id";                                    // 識別ID
		public const string FIELD_MAILCLICK_MAILTEXT_ID = "mailtext_id";                            // メール文章ID
		public const string FIELD_MAILCLICK_MAILDIST_ID = "maildist_id";                            // メール配信設定ID
		public const string FIELD_MAILCLICK_ACTION_NO = "action_no";                                // 実行履歴NO
		public const string FIELD_MAILCLICK_MAILCLICK_ID = "mailclick_id";                          // メールクリックID
		public const string FIELD_MAILCLICK_MAILCLICK_URL = "mailclick_url";                        // メールクリックURL
		public const string FIELD_MAILCLICK_MAILCLICK_KEY = "mailclick_key";                        // メールクリック識別キー
		public const string FIELD_MAILCLICK_PCMOBILE_KBN = "pcmobile_kbn";                          // PCモバイル区分
		public const string FIELD_MAILCLICK_VALID_FLG = "valid_flg";                                // 有効フラグ
		public const string FIELD_MAILCLICK_DEL_FLG = "del_flg";                                    // 削除フラグ
		public const string FIELD_MAILCLICK_DATE_CREATED = "date_created";                          // 作成日
		public const string FIELD_MAILCLICK_DATE_CHANGED = "date_changed";                          // 更新日
		public const string FIELD_MAILCLICK_LAST_CHANGED = "last_changed";                          // 最終更新者

		// メールクリックログ
		public const string TABLE_MAILCLICKLOG = "w2_MailClickLog";                                 // メールクリックログ
		public const string FIELD_MAILCLICKLOG_LOG_NO = "log_no";                                   // ログNO
		public const string FIELD_MAILCLICKLOG_DEPT_ID = "dept_id";                                 // 識別ID
		public const string FIELD_MAILCLICKLOG_MAILTEXT_ID = "mailtext_id";                         // メール文章ID
		public const string FIELD_MAILCLICKLOG_MAILDIST_ID = "maildist_id";                         // メール配信設定ID
		public const string FIELD_MAILCLICKLOG_ACTION_NO = "action_no";                             // 実行履歴NO
		public const string FIELD_MAILCLICKLOG_MAILCLICK_ID = "mailclick_id";                       // メールクリックID
		public const string FIELD_MAILCLICKLOG_USER_ID = "user_id";                                 // ユーザーID
		public const string FIELD_MAILCLICKLOG_DATE_CREATED = "date_created";                       // 作成日

		// メールエラーアドレスマスタ
		public const string TABLE_MAILERRORADDR = "w2_MailErrorAddr";                               // メールエラーアドレスマスタ
		public const string FIELD_MAILERRORADDR_MAIL_ADDR = "mail_addr";                            // 対象メールアドレス
		public const string FIELD_MAILERRORADDR_ERROR_POINT = "error_point";                        // エラーポイント
		public const string FIELD_MAILERRORADDR_DATE_CREATED = "date_created";                      // 作成日
		public const string FIELD_MAILERRORADDR_DATE_CHANGED = "date_changed";                      // 更新日
		public const string FIELD_MAILERRORADDR_LAST_CHANGED = "last_changed";                      // 最終更新者

		// タスクスケジュールマスタ
		public const string TABLE_TASKSCHEDULE = "w2_TaskSchedule";                                 // タスクスケジュールマスタ
		public const string FIELD_TASKSCHEDULE_SCHEDULE_DATE = "schedule_date";                     // スケジュール日付
		public const string FIELD_TASKSCHEDULE_DEPT_ID = "dept_id";                                 // 識別ID
		public const string FIELD_TASKSCHEDULE_ACTION_KBN = "action_kbn";                           // 実行区分
		public const string FIELD_TASKSCHEDULE_ACTION_MASTER_ID = "action_master_id";               // 実行マスタID
		public const string FIELD_TASKSCHEDULE_ACTION_NO = "action_no";                             // 実行履歴NO
		public const string FIELD_TASKSCHEDULE_PREPARE_STATUS = "prepare_status";                   // 準備ステータス
		public const string FIELD_TASKSCHEDULE_EXECUTE_STATUS = "execute_status";                   // 実行ステータス
		public const string FIELD_TASKSCHEDULE_PROGRESS = "progress";                               // 進捗
		public const string FIELD_TASKSCHEDULE_STOP_FLG = "stop_flg";                               // 停止フラグ
		public const string FIELD_TASKSCHEDULE_DEL_FLG = "del_flg";                                 // 削除フラグ
		public const string FIELD_TASKSCHEDULE_DATE_BEGIN = "date_begin";                           // 開始日時
		public const string FIELD_TASKSCHEDULE_DATE_END = "date_end";                               // 終了日時
		public const string FIELD_TASKSCHEDULE_DATE_CREATED = "date_created";                       // 作成日
		public const string FIELD_TASKSCHEDULE_DATE_CHANGED = "date_changed";                       // 更新日
		public const string FIELD_TASKSCHEDULE_LAST_CHANGED = "last_changed";                       // 最終更新者

		// タスクスケジュールログマスタ
		public const string TABLE_TASKSCHEDULELOG = "w2_TaskScheduleLog";                           // タスクスケジュールログマスタ
		public const string FIELD_TASKSCHEDULELOG_DEPT_ID = "dept_id";                              // 識別ID
		public const string FIELD_TASKSCHEDULELOG_ACTION_KBN = "action_kbn";                        // 実行区分
		public const string FIELD_TASKSCHEDULELOG_ACTION_MASTER_ID = "action_master_id";            // 実行マスタID
		public const string FIELD_TASKSCHEDULELOG_ACTION_NO = "action_no";                          // 実行履歴NO
		public const string FIELD_TASKSCHEDULELOG_MESSAGING_APP_KBN = "messaging_app_kbn";          // メッセージアプリ区分
		public const string FIELD_TASKSCHEDULELOG_RESULT = "result";                                // 配信結果

		// タスクスケジュール履歴マスタ
		public const string TABLE_TASKSCHEDULEHISTORY = "w2_TaskScheduleHistory";                   // タスクスケジュール履歴マスタ
		public const string FIELD_TASKSCHEDULEHISTORY_HISTORY_NO = "history_no";                    // スケジュール履歴NO
		public const string FIELD_TASKSCHEDULEHISTORY_SCHEDULE_DATE = "schedule_date";              // スケジュール日付
		public const string FIELD_TASKSCHEDULEHISTORY_DEPT_ID = "dept_id";                          // 識別ID
		public const string FIELD_TASKSCHEDULEHISTORY_ACTION_KBN = "action_kbn";                    // 実行区分
		public const string FIELD_TASKSCHEDULEHISTORY_ACTION_MASTER_ID = "action_master_id";        // 実行マスタID
		public const string FIELD_TASKSCHEDULEHISTORY_ACTION_NO = "action_no";                      // 実行履歴NO
		public const string FIELD_TASKSCHEDULEHISTORY_ACTION_STEP = "action_step";                  // 実行ステップ
		public const string FIELD_TASKSCHEDULEHISTORY_ACTION_KBN_DETAIL = "action_kbn_detail";      // 詳細実行区分
		public const string FIELD_TASKSCHEDULEHISTORY_ACTION_RESULT = "action_result";              // 実行結果
		public const string FIELD_TASKSCHEDULEHISTORY_TARGET_ID = "target_id";                      // 対象ターゲットリストID
		public const string FIELD_TASKSCHEDULEHISTORY_USER_ID = "user_id";                          // 対象ユーザID
		public const string FIELD_TASKSCHEDULEHISTORY_MAIL_ADDR = "mail_addr";                      // 対象メールアドレス

		// タスクスケジュール履歴集計テーブル
		public const string TABLE_TASKSCHEDULEHISTORYSUMMARY = "w2_TaskScheduleHistorySummary";     // タスクスケジュール履歴集計テーブル
		public const string FIELD_TASKSCHEDULEHISTORYSUMMARY_DEPT_ID = "dept_id";                   // 識別ID
		public const string FIELD_TASKSCHEDULEHISTORYSUMMARY_ACTION_KBN = "action_kbn";             // 実行区分
		public const string FIELD_TASKSCHEDULEHISTORYSUMMARY_ACTION_MASTER_ID = "action_master_id"; // 実行マスタID
		public const string FIELD_TASKSCHEDULEHISTORYSUMMARY_ACTION_NO = "action_no";               // 実行履歴NO
		public const string FIELD_TASKSCHEDULEHISTORYSUMMARY_ACTION_RESULT = "action_result";       // 実行結果
		public const string FIELD_TASKSCHEDULEHISTORYSUMMARY_ACTION_KBN_DETAIL = "action_kbn_detail";// 詳細実行区分
		public const string FIELD_TASKSCHEDULEHISTORYSUMMARY_TARGET_ID = "target_id";               // 対象ターゲットリストID
		public const string FIELD_TASKSCHEDULEHISTORYSUMMARY_HISTORY_COUNT = "history_count";       // 履歴件数

		// 広告コードマスタ
		public const string TABLE_ADVCODE = "w2_AdvCode";                                           // 広告コードマスタ
		public const string FIELD_ADVCODE_ADVCODE_NO = "advcode_no";                                // 広告コードNO
		public const string FIELD_ADVCODE_DEPT_ID = "dept_id";                                      // 識別ID
		public const string FIELD_ADVCODE_ADVERTISEMENT_CODE = "advertisement_code";                // 広告コード
		public const string FIELD_ADVCODE_MEDIA_NAME = "media_name";                                // 媒体名
		public const string FIELD_ADVCODE_VALID_FLG = "valid_flg";                                  // 有効フラグ
		public const string FIELD_ADVCODE_DATE_CREATED = "date_created";                            // 作成日
		public const string FIELD_ADVCODE_DATE_CHANGED = "date_changed";                            // 更新日
		public const string FIELD_ADVCODE_LAST_CHANGED = "last_changed";                            // 最終更新者
		public const string FIELD_ADVCODE_ADVERTISEMENT_DATE = "advertisement_date";                // 出稿日
		public const string FIELD_ADVCODE_MEDIA_COST = "media_cost";                                // 媒体費
		public const string FIELD_ADVCODE_MEMO = "memo";                                            // 備考
		public const string FIELD_ADVCODE_PUBLICATION_DATE_FROM = "publication_date_from";          // 媒体掲載期間(From)
		public const string FIELD_ADVCODE_PUBLICATION_DATE_TO = "publication_date_to";              // 媒体掲載期間(To)
		public const string FIELD_ADVCODE_ADVCODE_MEDIA_TYPE_ID = "advcode_media_type_id";          // 広告媒体区分
		public const string FIELD_ADVCODE_MEMBER_RANK_ID_GRANTED_AT_ACCOUNT_REGISTRATION = "member_rank_id_granted_at_account_registration";	// 会員登録時紐づけ会員ランクID
		public const string FIELD_ADVCODE_USER_MANAGEMENT_LEVEL_ID_GRANTED_AT_ACCOUNT_REGISTRATION = "user_management_level_id_granted_at_account_registration";	// 会員登録時紐づけユーザー管理レベルID

		// 広告媒体区分マスタ
		public const string TABLE_ADVCODEMEDIATYPE = "w2_AdvCodeMediaType";                         // 広告媒体区分マスタ
		public const string FIELD_ADVCODEMEDIATYPE_ADVCODE_MEDIA_TYPE_ID = "advcode_media_type_id"; // 区分ID
		public const string FIELD_ADVCODEMEDIATYPE_ADVCODE_MEDIA_TYPE_NAME = "advcode_media_type_name";// 媒体区分名
		public const string FIELD_ADVCODEMEDIATYPE_DISPLAY_ORDER = "display_order";                 // 表示順
		public const string FIELD_ADVCODEMEDIATYPE_DATE_CREATED = "date_created";                   // 作成日
		public const string FIELD_ADVCODEMEDIATYPE_DATE_CHANGED = "date_changed";                   // 更新日
		public const string FIELD_ADVCODEMEDIATYPE_LAST_CHANGED = "last_changed";                   // 最終更新者
		/// <summary>広告媒体区分マスタ：検索キーワード</summary>
		public const string FIELD_ADVCODEMEDIATYPE_SEARCH_WORD = "search_word";

		// 広告コード流入ログ
		public const string TABLE_ADVCODELOG = "w2_AdvCodeLog";                                     // 広告コード流入ログ
		public const string FIELD_ADVCODELOG_ADVCODELOG_NO = "advcodelog_no";                       // 広告コードログNO
		public const string FIELD_ADVCODELOG_ACCESS_DATE = "access_date";                           // 日付
		public const string FIELD_ADVCODELOG_ACCESS_TIME = "access_time";                           // 時間
		public const string FIELD_ADVCODELOG_ADVERTISEMENT_CODE = "advertisement_code";             // 広告コード
		public const string FIELD_ADVCODELOG_CAREER_ID = "career_id";                               // キャリアID
		public const string FIELD_ADVCODELOG_MOBILE_UID = "mobile_uid";                             // モバイルユーザID
		public const string FIELD_ADVCODELOG_ACCESS_USER_ID = "access_user_id";                     // アクセスユーザID

		// 会員ランクマスタ
		public const string TABLE_MEMBERRANK = "w2_MemberRank";                                     // 会員ランクマスタ
		public const string FIELD_MEMBERRANK_MEMBER_RANK_ID = "member_rank_id";                     // ランクID
		public const string FIELD_MEMBERRANK_MEMBER_RANK_ORDER = "member_rank_order";               // ランク順位
		public const string FIELD_MEMBERRANK_MEMBER_RANK_NAME = "member_rank_name";                 // ランク名
		public const string FIELD_MEMBERRANK_ORDER_DISCOUNT_TYPE = "order_discount_type";           // 注文割引方法
		public const string FIELD_MEMBERRANK_ORDER_DISCOUNT_VALUE = "order_discount_value";         // 注文割引数
		public const string FIELD_MEMBERRANK_ORDER_DISCOUNT_THRESHOLD_PRICE = "order_discount_threshold_price";// 注文金額割引き閾値
		public const string FIELD_MEMBERRANK_POINT_ADD_TYPE = "point_add_type";                     // ポイント加算方法
		public const string FIELD_MEMBERRANK_POINT_ADD_VALUE = "point_add_value";                   // ポイント加算数
		public const string FIELD_MEMBERRANK_SHIPPING_DISCOUNT_TYPE = "shipping_discount_type";     // 配送料割引方法
		public const string FIELD_MEMBERRANK_SHIPPING_DISCOUNT_VALUE = "shipping_discount_value";   // 配送料割引数
		public const string FIELD_MEMBERRANK_DEFAULT_MEMBER_RANK_SETTING_FLG = "default_member_rank_setting_flg";// デフォルト会員ランク設定フラグ
		public const string FIELD_MEMBERRANK_VALID_FLG = "valid_flg";                               // 有効フラグ
		public const string FIELD_MEMBERRANK_DATE_CREATED = "date_created";                         // 作成日
		public const string FIELD_MEMBERRANK_DATE_CHANGED = "date_changed";                         // 更新日
		public const string FIELD_MEMBERRANK_LAST_CHANGED = "last_changed";                         // 最終更新者
		public const string FIELD_MEMBERRANK_MEMBER_RANK_MEMO = "member_rank_memo";                 // ランクメモ
		public const string FIELD_MEMBERRANK_FIXED_PURCHASE_DISCOUNT_RATE = "fixed_purchase_discount_rate";		// 定期会員割引率

		// 会員ランク付与ルール
		public const string TABLE_MEMBERRANKRULE = "w2_MemberRankRule";                             // 会員ランク付与ルール
		public const string FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_ID = "member_rank_rule_id";       // ランク付与ルールID
		public const string FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_NAME = "member_rank_rule_name";   // ランク付与ルール名
		public const string FIELD_MEMBERRANKRULE_STATUS = "status";                                 // ステータス
		public const string FIELD_MEMBERRANKRULE_LAST_COUNT = "last_count";                         // 最終付与人数
		public const string FIELD_MEMBERRANKRULE_LAST_EXEC_DATE = "last_exec_date";                 // 最終付与日時
		public const string FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TYPE = "target_extract_type";       // 抽出条件集計期間指定
		public const string FIELD_MEMBERRANKRULE_TARGET_EXTRACT_START = "target_extract_start";     // 抽出条件集計期間開始日
		public const string FIELD_MEMBERRANKRULE_TARGET_EXTRACT_END = "target_extract_end";         // 抽出条件集計期間終了日
		public const string FIELD_MEMBERRANKRULE_TARGET_EXTRACT_DAYS_AGO = "target_extract_days_ago";// 抽出条件集計期間前日指定
		public const string FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_PRICE_FROM = "target_extract_total_price_from";// 抽出条件合計購入金額範囲(From)
		public const string FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_PRICE_TO = "target_extract_total_price_to";// 抽出条件合計購入金額範囲(To)
		public const string FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_COUNT_FROM = "target_extract_total_count_from";// 抽出条件合計購入回数範囲(From)
		public const string FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_COUNT_TO = "target_extract_total_count_to";// 抽出条件合計購入回数範囲(To)
		public const string FIELD_MEMBERRANKRULE_TARGET_EXTRACT_OLD_RANK_FLG = "target_extract_old_rank_flg";// 抽出時の旧ランク情報抽出判定
		public const string FIELD_MEMBERRANKRULE_RANK_CHANGE_TYPE = "rank_change_type";             // ランク付与方法
		public const string FIELD_MEMBERRANKRULE_RANK_CHANGE_RANK_ID = "rank_change_rank_id";       // 指定付与ランクID
		public const string FIELD_MEMBERRANKRULE_MAIL_ID = "mail_id";                               // メールテンプレートID
		public const string FIELD_MEMBERRANKRULE_EXEC_TIMING = "exec_timing";                       // 実行タイミング
		public const string FIELD_MEMBERRANKRULE_SCHEDULE_KBN = "schedule_kbn";                     // スケジュール区分
		public const string FIELD_MEMBERRANKRULE_SCHEDULE_DAY_OF_WEEK = "schedule_day_of_week";     // スケジュール曜日
		public const string FIELD_MEMBERRANKRULE_SCHEDULE_YEAR = "schedule_year";                   // スケジュール日程(年)
		public const string FIELD_MEMBERRANKRULE_SCHEDULE_MONTH = "schedule_month";                 // スケジュール日程(月)
		public const string FIELD_MEMBERRANKRULE_SCHEDULE_DAY = "schedule_day";                     // スケジュール日程(日)
		public const string FIELD_MEMBERRANKRULE_SCHEDULE_HOUR = "schedule_hour";                   // スケジュール日程(時)
		public const string FIELD_MEMBERRANKRULE_SCHEDULE_MINUTE = "schedule_minute";               // スケジュール日程(分)
		public const string FIELD_MEMBERRANKRULE_SCHEDULE_SECOND = "schedule_second";               // スケジュール日程(秒)
		public const string FIELD_MEMBERRANKRULE_VALID_FLG = "valid_flg";                           // 有効フラグ
		public const string FIELD_MEMBERRANKRULE_DATE_CREATED = "date_created";                     // 作成日
		public const string FIELD_MEMBERRANKRULE_DATE_CHANGED = "date_changed";                     // 更新日
		public const string FIELD_MEMBERRANKRULE_LAST_CHANGED = "last_changed";                     // 最終更新者
		public const string FIELD_MEMBERRANKRULE_TARGET_ID = "target_id";                          // ターゲットID
		public const string FIELD_MEMBERRANKRULE_TARGET_EXTRACT_FLG = "target_extract_flg";          // ターゲット抽出フラグ
		public const string FIELD_MEMBERRANKRULE_TARGET_ID2 = "target_id2";                        // ターゲットID2
		public const string FIELD_MEMBERRANKRULE_TARGET_EXTRACT_FLG2 = "target_extract_flg2";        // ターゲット2抽出フラグ
		public const string FIELD_MEMBERRANKRULE_TARGET_ID3 = "target_id3";                        // ターゲットID3
		public const string FIELD_MEMBERRANKRULE_TARGET_EXTRACT_FLG3 = "target_extract_flg3";        // ターゲット3抽出フラグ
		public const string FIELD_MEMBERRANKRULE_TARGET_ID4 = "target_id4";                        // ターゲットID4
		public const string FIELD_MEMBERRANKRULE_TARGET_EXTRACT_FLG4 = "target_extract_flg4";        // ターゲット4抽出フラグ
		public const string FIELD_MEMBERRANKRULE_TARGET_ID5 = "target_id5";                        // ターゲットID5
		public const string FIELD_MEMBERRANKRULE_TARGET_EXTRACT_FLG5 = "target_extract_flg5";        // ターゲット5抽出フラグ

		// 会員ランク更新履歴
		public const string TABLE_USERMEMBERRANKHISTORY = "w2_UserMemberRankHistory";               // 会員ランク更新履歴
		public const string FIELD_USERMEMBERRANKHISTORY_HISTORY_NO = "history_no";                  // 履歴No
		public const string FIELD_USERMEMBERRANKHISTORY_USER_ID = "user_id";                        // ユーザID
		public const string FIELD_USERMEMBERRANKHISTORY_BEFORE_RANK_ID = "before_rank_id";          // 更新前ランクID
		public const string FIELD_USERMEMBERRANKHISTORY_AFTER_RANK_ID = "after_rank_id";            // 更新後ランクID
		public const string FIELD_USERMEMBERRANKHISTORY_MAIL_ID = "mail_id";                        // メールテンプレートID
		public const string FIELD_USERMEMBERRANKHISTORY_CHANGED_BY = "changed_by";                  // 変更者
		public const string FIELD_USERMEMBERRANKHISTORY_DATE_CREATED = "date_created";              // 作成日

		// アフィリエイト連携ログ
		public const string TABLE_AFFILIATECOOPLOG = "w2_AffiliateCoopLog";                         // アフィリエイト連携ログ
		public const string FIELD_AFFILIATECOOPLOG_LOG_NO = "log_no";                               // ログNo
		public const string FIELD_AFFILIATECOOPLOG_AFFILIATE_KBN = "affiliate_kbn";                 // アフィリエイト区分
		public const string FIELD_AFFILIATECOOPLOG_MASTER_ID = "master_id";                         // マスタID
		public const string FIELD_AFFILIATECOOPLOG_COOP_STATUS = "coop_status";                     // 連携ステータス
		public const string FIELD_AFFILIATECOOPLOG_COOP_DATA1 = "coop_data1";                       // 連携データ1
		public const string FIELD_AFFILIATECOOPLOG_COOP_DATA2 = "coop_data2";                       // 連携データ2
		public const string FIELD_AFFILIATECOOPLOG_COOP_DATA3 = "coop_data3";                       // 連携データ3
		public const string FIELD_AFFILIATECOOPLOG_COOP_DATA4 = "coop_data4";                       // 連携データ4
		public const string FIELD_AFFILIATECOOPLOG_COOP_DATA5 = "coop_data5";                       // 連携データ5
		public const string FIELD_AFFILIATECOOPLOG_COOP_DATA6 = "coop_data6";                       // 連携データ6
		public const string FIELD_AFFILIATECOOPLOG_COOP_DATA7 = "coop_data7";                       // 連携データ7
		public const string FIELD_AFFILIATECOOPLOG_COOP_DATA8 = "coop_data8";                       // 連携データ8
		public const string FIELD_AFFILIATECOOPLOG_COOP_DATA9 = "coop_data9";                       // 連携データ9
		public const string FIELD_AFFILIATECOOPLOG_COOP_DATA10 = "coop_data10";                     // 連携データ10
		public const string FIELD_AFFILIATECOOPLOG_DATE_CREATED = "date_created";                   // 作成日
		public const string FIELD_AFFILIATECOOPLOG_DATE_CHANGED = "date_changed";                   // 更新日
		public const string FIELD_AFFILIATECOOPLOG_LAST_CHANGED = "last_changed";                   // 最終更新者
		public const string FIELD_AFFILIATECOOPLOG_COOP_DATA11 = "coop_data11";                     // 連携データ11

		// アフィリエイトタグ設定マスタ
		public const string TABLE_AFFILIATETAGSETTING = "w2_AffiliateTagSetting";                   // アフィリエイトタグ設定マスタ
		public const string FIELD_AFFILIATETAGSETTING_AFFILIATE_ID = "affiliate_id";                // アフィリエイトID
		public const string FIELD_AFFILIATETAGSETTING_AFFILIATE_NAME = "affiliate_name";            // アフィリエイト名
		public const string FIELD_AFFILIATETAGSETTING_AFFILIATE_KBN = "affiliate_kbn";              // アフィリエイト区分
		public const string FIELD_AFFILIATETAGSETTING_SESSION_NAME1 = "session_name1";              // セッション変数名1
		public const string FIELD_AFFILIATETAGSETTING_SESSION_NAME2 = "session_name2";              // セッション変数名2
		public const string FIELD_AFFILIATETAGSETTING_USER_AGENT_COOP_KBN = "user_agent_coop_kbn";  // ユーザーエージェント連携区分
		public const string FIELD_AFFILIATETAGSETTING_DISPLAY_ORDER = "display_order";              // 表示順
		public const string FIELD_AFFILIATETAGSETTING_AFFILIATE_TAG1 = "affiliate_tag1";            // アフィリエイトタグ１
		public const string FIELD_AFFILIATETAGSETTING_AFFILIATE_TAG2 = "affiliate_tag2";            // アフィリエイトタグ２
		public const string FIELD_AFFILIATETAGSETTING_AFFILIATE_TAG3 = "affiliate_tag3";            // アフィリエイトタグ３
		public const string FIELD_AFFILIATETAGSETTING_AFFILIATE_TAG4 = "affiliate_tag4";            // アフィリエイトタグ４
		public const string FIELD_AFFILIATETAGSETTING_AFFILIATE_TAG5 = "affiliate_tag5";            // アフィリエイトタグ５
		public const string FIELD_AFFILIATETAGSETTING_AFFILIATE_TAG6 = "affiliate_tag6";            // アフィリエイトタグ６
		public const string FIELD_AFFILIATETAGSETTING_AFFILIATE_TAG7 = "affiliate_tag7";            // アフィリエイトタグ７
		public const string FIELD_AFFILIATETAGSETTING_AFFILIATE_TAG8 = "affiliate_tag8";            // アフィリエイトタグ８
		public const string FIELD_AFFILIATETAGSETTING_AFFILIATE_TAG9 = "affiliate_tag9";            // アフィリエイトタグ９
		public const string FIELD_AFFILIATETAGSETTING_AFFILIATE_TAG10 = "affiliate_tag10";          // アフィリエイトタグ１０
		public const string FIELD_AFFILIATETAGSETTING_VALID_FLG = "valid_flg";                      // 有効フラグ
		public const string FIELD_AFFILIATETAGSETTING_DATE_CREATED = "date_created";                // 作成日
		public const string FIELD_AFFILIATETAGSETTING_DATE_CHANGED = "date_changed";                // 更新日
		public const string FIELD_AFFILIATETAGSETTING_LAST_CHANGED = "last_changed";                // 最終更新者
		public const string FIELD_AFFILIATETAGSETTING_AFFILIATE_PRODUCT_TAG_ID = "affiliate_product_tag_id";// アフィリエイト商品タグID
		public const string FIELD_AFFILIATETAGSETTING_OUTPUT_LOCATION = "output_location";          // 出力箇所
		/// <summary>アフィリエイトタグ設定マスタ：キーワード検索フィールド</summary>
		public const string FIELD_AFFILIATETAGSETTING_SEARCH_WORD = "search_word";

		// アフィリエイト商品タグ設定マスタ
		public const string TABLE_AFFILIATEPRODUCTTAGSETTING = "w2_AffiliateProductTagSetting";     // アフィリエイト商品タグ設定マスタ
		public const string FIELD_AFFILIATEPRODUCTTAGSETTING_AFFILIATE_PRODUCT_TAG_ID = "affiliate_product_tag_id";// アフィリエイト商品タグID
		public const string FIELD_AFFILIATEPRODUCTTAGSETTING_TAG_NAME = "tag_name";                 // タグ名称
		public const string FIELD_AFFILIATEPRODUCTTAGSETTING_TAG_CONTENT = "tag_content";           // タグ内容
		public const string FIELD_AFFILIATEPRODUCTTAGSETTING_TAG_DELIMITER = "tag_delimiter";       // 区切り文字
		public const string FIELD_AFFILIATEPRODUCTTAGSETTING_DATE_CREATED = "date_created";         // 作成日
		public const string FIELD_AFFILIATEPRODUCTTAGSETTING_DATE_CHANGED = "date_changed";         // 更新日
		public const string FIELD_AFFILIATEPRODUCTTAGSETTING_LAST_CHANGED = "last_changed";         // 最終更新者

		// アフィリエイトタグの出力条件管理
		public const string TABLE_AFFILIATETAGCONDITION = "w2_AffiliateTagCondition";               // アフィリエイトタグの出力条件管理
		public const string FIELD_AFFILIATETAGCONDITION_AFFILIATE_ID = "affiliate_id";              // アフィリエイトID
		public const string FIELD_AFFILIATETAGCONDITION_CONDITION_TYPE = "condition_type";          // 条件タイプ
		public const string FIELD_AFFILIATETAGCONDITION_CONDITION_SORT_NO = "condition_sort_no";    // 登録順序
		public const string FIELD_AFFILIATETAGCONDITION_CONDITION_VALUE = "condition_value";        // 内容
		public const string FIELD_AFFILIATETAGCONDITION_MATCH_TYPE = "match_type";                  // 一致条件タイプ

		// 広告コードマスタワークテーブル
		public const string TABLE_WORKADVCODE = "w2_WorkAdvCode";                                   // 広告コードマスタワークテーブル
		public const string FIELD_WORKADVCODE_ADVCODE_NO = "advcode_no";                            // 広告コードNO
		public const string FIELD_WORKADVCODE_DEPT_ID = "dept_id";                                  // 識別ID
		public const string FIELD_WORKADVCODE_ADVERTISEMENT_CODE = "advertisement_code";            // 広告コード
		public const string FIELD_WORKADVCODE_MEDIA_NAME = "media_name";                            // 媒体名
		public const string FIELD_WORKADVCODE_VALID_FLG = "valid_flg";                              // 有効フラグ
		public const string FIELD_WORKADVCODE_DATE_CREATED = "date_created";                        // 作成日
		public const string FIELD_WORKADVCODE_DATE_CHANGED = "date_changed";                        // 更新日
		public const string FIELD_WORKADVCODE_LAST_CHANGED = "last_changed";                        // 最終更新者
		public const string FIELD_WORKADVCODE_ADVERTISEMENT_DATE = "advertisement_date";            // 出稿日
		public const string FIELD_WORKADVCODE_MEDIA_COST = "media_cost";                            // 媒体費
		public const string FIELD_WORKADVCODE_MEMO = "memo";                                        // 備考
		public const string FIELD_WORKADVCODE_PUBLICATION_DATE_FROM = "publication_date_from";      // 媒体掲載期間(From)
		public const string FIELD_WORKADVCODE_PUBLICATION_DATE_TO = "publication_date_to";          // 媒体掲載期間(To)
		public const string FIELD_WORKADVCODE_ADVCODE_MEDIA_TYPE_ID = "advcode_media_type_id";      // 広告媒体区分

		// 広告媒体区分マスタワークテーブル
		public const string TABLE_WORKADVCODEMEDIATYPE = "w2_WorkAdvCodeMediaType";                 // 広告媒体区分マスタワークテーブル
		public const string FIELD_WORKADVCODEMEDIATYPE_ADVCODE_MEDIA_TYPE_ID = "advcode_media_type_id";// 区分ID
		public const string FIELD_WORKADVCODEMEDIATYPE_ADVCODE_MEDIA_TYPE_NAME = "advcode_media_type_name";// 媒体区分名
		public const string FIELD_WORKADVCODEMEDIATYPE_DISPLAY_ORDER = "display_order";             // 表示順
		public const string FIELD_WORKADVCODEMEDIATYPE_DATE_CREATED = "date_created";               // 作成日
		public const string FIELD_WORKADVCODEMEDIATYPE_DATE_CHANGED = "date_changed";               // 更新日
		public const string FIELD_WORKADVCODEMEDIATYPE_LAST_CHANGED = "last_changed";               // 最終更新者

		// クーポンワークテーブル
		public const string TABLE_WORKCOUPON = "w2_WorkCoupon";                                     // クーポンワークテーブル
		public const string FIELD_WORKCOUPON_DEPT_ID = "dept_id";                                   // 識別ID
		public const string FIELD_WORKCOUPON_COUPON_ID = "coupon_id";                               // クーポンID
		public const string FIELD_WORKCOUPON_COUPON_CODE = "coupon_code";                           // クーポンコード
		public const string FIELD_WORKCOUPON_COUPON_NAME = "coupon_name";                           // 管理用クーポン名
		public const string FIELD_WORKCOUPON_COUPON_DISP_NAME = "coupon_disp_name";                 // 表示用クーポン名
		public const string FIELD_WORKCOUPON_COUPON_DISP_NAME_MOBILE = "coupon_disp_name_mobile";   // モバイル用表示用クーポン名
		public const string FIELD_WORKCOUPON_COUPON_DISCRIPTION = "coupon_discription";             // クーポン説明文
		public const string FIELD_WORKCOUPON_COUPON_DISP_DISCRIPTION = "coupon_disp_discription";   // クーポン説明文(ユーザ表示用)
		public const string FIELD_WORKCOUPON_COUPON_DISCRIPTION_MOBILE = "coupon_discription_mobile";// モバイル用クーポン説明文
		public const string FIELD_WORKCOUPON_COUPON_TYPE = "coupon_type";                           // クーポン種別
		public const string FIELD_WORKCOUPON_COUPON_COUNT = "coupon_count";                         // クーポン利用可能回数
		public const string FIELD_WORKCOUPON_PUBLISH_DATE_BGN = "publish_date_bgn";                 // クーポン発行期間(開始)
		public const string FIELD_WORKCOUPON_PUBLISH_DATE_END = "publish_date_end";                 // クーポン発行期間(終了)
		public const string FIELD_WORKCOUPON_DISCOUNT_PRICE = "discount_price";                     // クーポン商品割引額
		public const string FIELD_WORKCOUPON_DISCOUNT_RATE = "discount_rate";                       // クーポン商品割引率
		public const string FIELD_WORKCOUPON_EXPIRE_DAY = "expire_day";                             // クーポン有効期限(日)
		public const string FIELD_WORKCOUPON_EXPIRE_DATE_BGN = "expire_date_bgn";                   // クーポン有効期間(開始)
		public const string FIELD_WORKCOUPON_EXPIRE_DATE_END = "expire_date_end";                   // クーポン有効期間(終了)
		public const string FIELD_WORKCOUPON_PRODUCT_KBN = "product_kbn";                           // クーポン対象商品区分
		public const string FIELD_WORKCOUPON_EXCEPTIONAL_PRODUCT = "exceptional_product";           // クーポン例外商品
		public const string FIELD_WORKCOUPON_EXCEPTIONAL_ICON = "exceptional_icon";                 // クーポン例外商品アイコン
		public const string FIELD_WORKCOUPON_USABLE_PRICE = "usable_price";                         // クーポン利用最低購入金額
		public const string FIELD_WORKCOUPON_USE_TOGETHER_FLG = "use_together_flg";                 // クーポン併用フラグ
		public const string FIELD_WORKCOUPON_VALID_FLG = "valid_flg";                               // 有効フラグ
		public const string FIELD_WORKCOUPON_DISP_FLG = "disp_flg";                                 // ユーザ表示フラグ
		public const string FIELD_WORKCOUPON_DATE_CREATED = "date_created";                         // 作成日
		public const string FIELD_WORKCOUPON_DATE_CHANGED = "date_changed";                         // 更新日
		public const string FIELD_WORKCOUPON_LAST_CHANGED = "last_changed";                         // 最終更新者
		public const string FIELD_WORKCOUPON_FREE_SHIPPING_FLG = "free_shipping_flg";               // クーポン配送料無料フラグ

		// ユーザクーポンワークテーブル
		public const string TABLE_WORKUSERCOUPON = "w2_WorkUserCoupon";                             // ユーザクーポンワークテーブル
		public const string FIELD_WORKUSERCOUPON_USER_ID = "user_id";                               // ユーザID
		public const string FIELD_WORKUSERCOUPON_DEPT_ID = "dept_id";                               // 識別ID
		public const string FIELD_WORKUSERCOUPON_COUPON_ID = "coupon_id";                           // クーポンID
		public const string FIELD_WORKUSERCOUPON_COUPON_NO = "coupon_no";                           // 枝番
		public const string FIELD_WORKUSERCOUPON_ORDER_ID = "order_id";                             // 注文ID
		public const string FIELD_WORKUSERCOUPON_USE_FLG = "use_flg";                               // 利用フラグ
		public const string FIELD_WORKUSERCOUPON_DATE_CREATED = "date_created";                     // 作成日
		public const string FIELD_WORKUSERCOUPON_DATE_CHANGED = "date_changed";                     // 更新日
		public const string FIELD_WORKUSERCOUPON_LAST_CHANGED = "last_changed";                     // 最終更新者

		// メール配信文章グローバルマスタ
		public const string TABLE_MAILDISTTEXTGLOBAL = "w2_MailDistTextGlobal";                     // メール配信文章グローバルマスタ
		public const string FIELD_MAILDISTTEXTGLOBAL_DEPT_ID = "dept_id";                           // 識別ID
		public const string FIELD_MAILDISTTEXTGLOBAL_MAILTEXT_ID = "mailtext_id";                   // メール文章ID
		public const string FIELD_MAILDISTTEXTGLOBAL_LANGUAGE_CODE = "language_code";               // 言語コード
		public const string FIELD_MAILDISTTEXTGLOBAL_LANGUAGE_LOCALE_ID = "language_locale_id";     // 言語ロケールID
		public const string FIELD_MAILDISTTEXTGLOBAL_MAILTEXT_NAME = "mailtext_name";               // メール文章名
		public const string FIELD_MAILDISTTEXTGLOBAL_MAILTEXT_SUBJECT = "mailtext_subject";         // メールタイトル
		public const string FIELD_MAILDISTTEXTGLOBAL_MAILTEXT_BODY = "mailtext_body";               // メール文章テキスト
		public const string FIELD_MAILDISTTEXTGLOBAL_MAILTEXT_HTML = "mailtext_html";               // メール文章HTML
		public const string FIELD_MAILDISTTEXTGLOBAL_SENDHTML_FLG = "sendhtml_flg";                 // HTMLメール送信フラグ
		public const string FIELD_MAILDISTTEXTGLOBAL_MAIL_FROM_NAME = "mail_from_name";             // メールFROM名
		public const string FIELD_MAILDISTTEXTGLOBAL_MAIL_FROM = "mail_from";                       // メールFROM
		public const string FIELD_MAILDISTTEXTGLOBAL_DATE_CREATED = "date_created";                 // 作成日
		public const string FIELD_MAILDISTTEXTGLOBAL_DATE_CHANGED = "date_changed";                 // 更新日
		public const string FIELD_MAILDISTTEXTGLOBAL_LAST_CHANGED = "last_changed";                 // 最終更新者
		public const string FIELD_MAILDISTTEXTGLOBAL_SMS_USE_FLG = "sms_use_flg";                   // SMS利用フラグ

		// SMS配信文言
		public const string TABLE_GLOBALSMSDISTTEXT = "w2_GlobalSMSDistText";                       // SMS配信文言
		public const string FIELD_GLOBALSMSDISTTEXT_DEPT_ID = "dept_id";                            // 識別ID
		public const string FIELD_GLOBALSMSDISTTEXT_MAILTEXT_ID = "mailtext_id";                    // メール文章ID
		public const string FIELD_GLOBALSMSDISTTEXT_PHONE_CARRIER = "phone_carrier";                // キャリア
		public const string FIELD_GLOBALSMSDISTTEXT_SMS_TEXT = "sms_text";                          // SMS本文
		public const string FIELD_GLOBALSMSDISTTEXT_DATE_CREATED = "date_created";                  // 作成日
		public const string FIELD_GLOBALSMSDISTTEXT_DATE_CHANGED = "date_changed";                  // 更新日
		public const string FIELD_GLOBALSMSDISTTEXT_LAST_CHANGED = "last_changed";                  // 最終更新者

		// 定期出荷予測
		public const string TABLE_FIXEDPURCHASEFORECAST = "w2_FixedPurchaseForecast";               // 定期出荷予測
		public const string FIELD_FIXEDPURCHASEFORECAST_TARGET_MONTH = "target_month";              // 対象年月
		public const string FIELD_FIXEDPURCHASEFORECAST_FIXED_PURCHASE_ID = "fixed_purchase_id";    // 定期購入ID
		public const string FIELD_FIXEDPURCHASEFORECAST_SHOP_ID = "shop_id";                        // 店舗ID
		public const string FIELD_FIXEDPURCHASEFORECAST_PRODUCT_ID = "product_id";                  // 商品ID
		public const string FIELD_FIXEDPURCHASEFORECAST_VARIATION_ID = "variation_id";              // 商品バリエーションID
		public const string FIELD_FIXEDPURCHASEFORECAST_PRODUCT_PRICE = "product_price";            // 商品金額
		public const string FIELD_FIXEDPURCHASEFORECAST_ITEM_QUANTITY = "item_quantity";            // 商品数
		public const string FIELD_FIXEDPURCHASEFORECAST_DELIVERY_FREQUENCY = "delivery_frequency";  // 配送頻度(配送日基準)
		public const string FIELD_FIXEDPURCHASEFORECAST_DATE_CREATED = "date_created";              // 作成日
		public const string FIELD_FIXEDPURCHASEFORECAST_DELIVERY_FREQUENCY_BY_SCHEDULED_SHIPPING_DATE = "delivery_frequency_by_scheduled_shipping_date";    // 配送頻度(出荷予定日基準)

		#endregion

		#region フィールドフラグ定数

		// アクセスログ処理ステータス：ステータス
		public const string FLG_ACCESSLOGPROCSTAT_DAY_STATUS_INIT = "00";	// 初期状態
		public const string FLG_ACCESSLOGPROCSTAT_DAY_STATUS_IMPORT_RUN = "01";	// ログファイル取込処理中
		public const string FLG_ACCESSLOGPROCSTAT_DAY_STATUS_IMPORT_PAUSE = "02";	// ログファイル取込一時完了
		public const string FLG_ACCESSLOGPROCSTAT_DAY_STATUS_PROC_WAIT = "10";	// 加工待ち
		public const string FLG_ACCESSLOGPROCSTAT_DAY_STATUS_PROC_RUN = "11";	// 加工処理中
		public const string FLG_ACCESSLOGPROCSTAT_DAY_STATUS_END = "20";	// 取込・加工処理完了

		// ポイント：ポイント有効期限設定
		public const string FLG_POINT_POINT_EXP_KBN_VALID = "01";	// 有効期限あり
		public const string FLG_POINT_POINT_EXP_KBN_INVALID = "00";	// 有効期限なし

		// ポイント：ポイント区分
		public const string FLG_POINT_POINT_KBN_BASE = "01";	// 通常ポイント

		// ポイントルール：ポイントルール区分
		public const string FLG_POINTRULE_POINT_RULE_KBN_BASE = "01";	// 基本
		public const string FLG_POINTRULE_POINT_RULE_KBN_CAMPAIGN = "02";	// キャンペーン

		// ポイントルール：仮ポイント使用フラグ
		public const string FLG_POINTRULE_USE_TEMP_FLG_INVALID = "0";	// 仮ポイントを使用しない
		public const string FLG_POINTRULE_USE_TEMP_FLG_VALID = "1"; // 仮ポイントを使用する

		// ポイントルール：ポイント加算区分
		/// <summary>購入時ポイント発行</summary>
		public const string FLG_POINTRULE_POINT_INC_KBN_BUY = "01";
		/// <summary>新規登録ポイント発行</summary>
		public const string FLG_POINTRULE_POINT_INC_KBN_USER_REGISTER = "02";
		/// <summary>ログイン毎ポイント発行</summary>
		public const string FLG_POINTRULE_POINT_INC_KBN_LOGIN = "03";
		/// <summary>初回購入ポイント発行</summary>
		public const string FLG_POINTRULE_POINT_INC_KBN_FIRST_BUY = "04";
		/// <summary>クリックポイント</summary>
		public const string FLG_POINTRULE_POITN_INC_KBN_CLICK = "05";
		/// <summary>汎用ポイントルール</summary>
		public const string FLG_POINTRULE_POINT_INC_KBN_VERSATILE_POINT_RULE = "90";
		/// <summary>誕生日ポイント</summary>
		public const string FLG_POINTRULE_POINT_INC_KBN_BIRTHDAY_POINT = "91";
		/// <summary>レビューポイント発行</summary>
		public const string FLG_POINTRULE_POINT_INC_KBN_REWARD_POINT = "92";

		// レビュー投稿ポイント付与制限区分
		/// <summary>ユーザが各商品ごとに１回まで付与が可能</summary>
		public const string FLG_REWARD_POINT_GRANT_LIMIT_KBN_PRODUCT = "Product";
		/// <summary>ユーザごとに１回まで付与が可能</summary>
		public const string FLG_REWARD_POINT_GRANT_LIMIT_KBN_USER = "User";

		// ポイントルール：ポイント加算方法
		public const string FLG_POINTRULE_INC_TYPE_NUM = "01";	// ポイント加算数を使用
		public const string FLG_POINTRULE_INC_TYPE_RATE = "02";	// ポイント加算率を使用
		public const string FLG_POINTRULE_INC_TYPE_PRODUCT = "03";	// 商品マスタのポイント数を使用

		// ポイントルール：キャンペーン期間区分
		public const string FLG_POINTRULE_CAMPAIGN_TERM_KBN_MONTH = "01";	// 毎月
		public const string FLG_POINTRULE_CAMPAIGN_TERM_WEEK = "02";	// 毎週
		public const string FLG_POINTRULE_CAMPAIGN_TERM_KBN_EVERY_OTHER_WEEK = "03";	// 隔週

		// ポイントルール：期間限定ポイント発効オフセット種別
		public const string FLG_POINTRULE_EFFECTIVE_OFFSET_TYPE_DAY = "01"; // Ｎ日後
		public const string FLG_POINTRULE_EFFECTIVE_OFFSET_TYPE_MONTH = "02"; // Ｎヶ月後
		public const string FLG_POINTRULE_EFFECTIVE_OFFSET_TYPE_MONTH_FIRST_DAY = "03"; // Ｎヶ月後の月初

		// ポイントルール：期間限定ポイント有効期間（相対）種別
		public const string FLG_POINTRULE_TERM_TYPE_DAY = "01"; // Ｎ日間
		public const string FLG_POINTRULE_TERM_TYPE_MONTH = "02"; // Ｎヶ月間

		// ポイントルール：基本ルールとの二重適用を許可
		public const string FLG_POINTRULE_DUPLICATE_APPLY_DISALLOW = "0";	// 許可しない
		public const string FLG_POINTRULE_DUPLICATE_APPLY_ALLOW = "1";	// 許可する

		// ポイントルール：有効フラグ
		public const string FLG_POINTRULE_VALID_FLG_VALID = "1";	// 有効
		public const string FLG_POINTRULE_VALID_FLG_INVALID = "0";	// 無効

		// ユーザーポイント：ポイント区分
		public const string FLG_USERPOINT_POINT_KBN_BASE = "01";	// 通常ポイント
		public const string FLG_USERPOINT_POINT_KBN_LIMITED_TERM_POINT = "02"; // 期間限定ポイント

		// ユーザーポイント：ポイントルール区分
		public const string FLG_USERPOINT_POINT_RULE_KBN_BASIC = "01";
		public const string FLG_USERPOINT_POINT_RULE_KBN_CAMPAIGN = "02";

		// ユーザポイント：ポイント種別
		public const string FLG_USERPOINT_POINT_TYPE_TEMP = "00";	// 仮ポイント
		public const string FLG_USERPOINT_POINT_TYPE_COMP = "01";	// 本ポイント

		// ユーザポイント：ポイント付与されているか
		public const string FLG_USERPOINT_POINT_NOT_PUBLISHED = "0";	// 付与されていない
		public const string FLG_USERPOINT_POINT_PUBLISHED = "1";	// 付与されいる
		
		// ポイントルールスケジュール：ステータス
		public const string FLG_POINTRULESCHEDULE_STATUS_NORMAL = "00"; // 通常
		public const string FLG_POINTRULESCHEDULE_STATUS_EXTRACT = "01"; // 抽出中
		public const string FLG_POINTRULESCHEDULE_STATUS_UPDATE = "02"; // 更新中
		public const string FLG_POINTRULESCHEDULE_STATUS_ERROR = "09"; // 更新エラー

		// ポイントルールスケジュール：ターゲット抽出フラグ
		public const string FLG_POINTRULESCHEDULE_TARGET_EXTRACT_FLG_ON = "1";
		public const string FLG_POINTRULESCHEDULE_TARGET_EXTRACT_FLG_OFF = "0";

		// ポイントルールスケジュール：実行タイミング
		public const string FLG_POINTRULESCHEDULE_EXEC_TIMING_MANUAL = "01"; // 手動実行
		public const string FLG_POINTRULESCHEDULE_EXEC_TIMING_SCHEDULE = "02"; // スケジュール実行

		// ポイントルールスケジュール：スケジュール区分
		public const string FLG_POINTRULESCHEDULE_SCHEDULE_KBN_DAY = "01"; // 日単位（毎日HH:mm:ssに実行）
		public const string FLG_POINTRULESCHEDULE_SCHEDULE_KBN_WEEK = "02"; // 週単位（毎週ddd曜日HH:mm:ssに実行）
		public const string FLG_POINTRULESCHEDULE_SCHEDULE_KBN_MONTH = "03"; // 月単位（毎月dd日HH:mm:ssに実行）
		//public const string FLG_POINTRULESCHEDULE_SCHEDULE_KBN_YEAR = "04"; // 年単位（毎年MM月dd日HH:mm:ssに実行）
		public const string FLG_POINTRULESCHEDULE_SCHEDULE_KBN_ONCE = "05"; // 一回のみ（一回のみyyyy年MM月dd日HH:mm:ssに実行）

		// ポイントルールスケジュール：有効フラグ
		public const string FLG_POINTRULESCHEDULE_VALID_FLG_VALID = "ON"; // 有効
		public const string FLG_POINTRULESCHEDULE_VALID_FLG_INVALID = "OFF"; // 無効

		// ユーザーポイント履歴：ポイント区分
		public const string FLG_USERPOINTHISTORY_POINT_KBN_BASE = "01";	// 通常ポイント
		public const string FLG_USERPOINTHISTORY_POINT_KBN_LIMITED_TERM_POINT = "02";	// 期間限定ポイント

		// ユーザポイント履歴：ポイント種別
		public const string FLG_USERPOINTHISTORY_POINT_TYPE_TEMP = "00";	// 仮ポイント
		public const string FLG_USERPOINTHISTORY_POINT_TYPE_COMP = "01";	// 本ポイント

		// ユーザポイント履歴：ポイントルール区分
		public const string FLG_USERPOINTHISTORY_POINT_RULE_KBN_BASE = "01";		// 基本
		public const string FLG_USERPOINTHISTORY_POINT_RULE_KBN_CAMPAIGN = "02";    // キャンペーン

		// ユーザポイント履歴：ポイント加算区分
		/// <summary>購入時ポイント発行</summary>
		public const string FLG_USERPOINTHISTORY_POINT_INC_KBN_BUY = "01";
		/// <summary>新規登録ポイント発行</summary>
		public const string FLG_USERPOINTHISTORY_POINT_INC_KBN_USER_REGISTER = "02";
		/// <summary>ログイン毎ポイント発行</summary>
		public const string FLG_USERPOINTHISTORY_POINT_INC_KBN_LOGIN = "03";
		/// <summary>初回購入ポイント発行</summary>
		public const string FLG_USERPOINTHISTORY_POINT_INC_KBN_FIRST_BUY = "04";
		/// <summary>クリックポイント</summary>
		public const string FLG_USERPOINTHISTORY_POINT_INC_KBN_CLICK = "05";
		/// <summary>ポイント利用</summary>
		public const string FLG_USERPOINTHISTORY_POINT_INC_KBN_USE_POINT = "10";
		/// <summary>ポイント有効期限切れ削除</summary>
		public const string FLG_USERPOINTHISTORY_POINT_INC_KBN_POINT_EXPIRED = "11";
		/// <summary>ポイント利用変更（次回定期購入）</summary>
		public const string FLG_USERPOINTHISTORY_POINT_INC_KBN_UPDATE_NEXT_SHIPPING_USE_POINT = "12";
		/// <summary>ポイント利用調整（次回定期購入分）</summary>
		public const string FLG_USERPOINTHISTORY_POINT_INC_KBN_ADJUST_USE_POINT_FROM_FIXED_PURCHASE_TO_ORDER = "21";
		/// <summary>ポイント利用戻し（次回定期購入分調整）</summary>
		public const string FLG_USERPOINTHISTORY_POINT_INC_KBN_RETURN_NOT_USE_POINT = "22";
		/// <summary>ポイント利用キャンセル</summary>
		public const string FLG_USERPOINTHISTORY_POINT_INC_KBN_CANCEL_USED_POINT = "30";
		/// <summary>購入時ポイントキャンセル</summary>
		public const string FLG_USERPOINTHISTORY_POINT_INC_KBN_CANCEL_ADDED_POINT = "31";
		/// <summary>初回購入時ポイントキャンセル</summary>
		public const string FLG_USERPOINTHISTORY_POINT_INC_KBN_CANCEL_FIRST_BUY_ADDED_POINT = "34";
		/// <summary>ポイント利用キャンセル（次回定期購入）</summary>
		public const string FLG_USERPOINTHISTORY_POINT_INC_KBN_CANCEL_NEXT_SHIPPING_USE_POINT = "35";
		/// <summary>返品時オペレータ調整</summary>
		public const string FLG_USERPOINTHISTORY_POINT_INC_KBN_RETURN_ITEM = "40";
		/// <summary>交換時オペレータ発行</summary>
		public const string FLG_USERPOINTHISTORY_POINT_INC_KBN_EXCHANGE_ITEM = "41";
		/// <summary>返品交換付与ポイント調整</summary>
		public const string FLG_USERPOINTHISTORY_POINT_INC_KBN_RETURNEXCHANGE_ADJUST_ORDER_POINT_ADD = "42";
		/// <summary>返品交換利用ポイント調整</summary>
		public const string FLG_USERPOINTHISTORY_POINT_INC_KBN_RETURNEXCHANGE_ADJUST_ORDER_POINT_USE = "43";
		/// <summary>店舗利用</summary>
		public const string FLG_USERPOINTHISTORY_POINT_INC_KBN_USE_POINT_AT_REAL_SHOP = "95";
		/// <summary>店舗発行</summary>
		public const string FLG_USERPOINTHISTORY_POINT_INC_KBN_REAL_SHOP_POINT = "96";
		/// <summary>マスタアップロード</summary>
		public const string FLG_USERPOINTHISTORY_POINT_INC_KBN_KBN_OPERATOR_MASTER_UPLOAD = "97";
		/// <summary>オペレータ注文変更</summary>
		public const string FLG_USERPOINTHISTORY_POINT_INC_KBN_KBN_ORDER = "98";
		/// <summary>オペレータ調整</summary>
		public const string FLG_USERPOINTHISTORY_POINT_INC_KBN_KBN_OPERATOR = "99";
		/// <summary>ユーザー統合</summary>
		public const string FLG_USERPOINTHISTORY_POINT_INC_KBN_KBN_USERINTEGRATION = "100";
		/// <summary>再計算時の復元</summary>
		public const string FLG_USERPOINTHISTORY_POINT_INC_KBN_RECALCULATION_RESTORE = "101";
		/// <summary>CROSS POINT連携調整</summary>
		public const string FLG_USERPOINTHISTORY_POINT_INC_KBN_CROSS_POINT = "999";

		// ユーザーポイント履歴：復元処理済フラグ
		public const string FLG_USERPOINTHISTORY_POINT_RESTORED_FLG_NOT_RESTORED = "0";	// 復元処理未実施
		public const string FLG_USERPOINTHISTORY_POINT_RESTORED_FLG_RESTORED = "1";	// 復元処理実施済
		public const string FLG_USERPOINTHISTORY_POINT_RESTORED_FLG_NA = "2";	// V5.13以前の履歴情報

		// メール配信文章マスタ：HTMLメール送信フラグ
		public const string FLG_MAILDISTTEXT_SENDHTML_FLG_VALID = "1";	// 有効
		public const string FLG_MAILDISTTEXT_SENDHTML_FLG_INVALID = "0";	// 無効

		// クーポン：クーポン種別
		public const string FLG_COUPONCOUPON_TYPE_USERREGIST = "01";	// 新規登録で発行する会員用クーポン
		public const string FLG_COUPONCOUPON_TYPE_BUY = "02";	// 商品購入で発行する会員用クーポン
		public const string FLG_COUPONCOUPON_TYPE_FIRSTBUY = "03";	// 初回購入した人に発行する会員用クーポン
		public const string FLG_COUPONCOUPON_TYPE_LIMITED_FOR_REGISTERED_USER = "04";	// 利用回数制限つき会員用クーポン
		public const string FLG_COUPONCOUPON_TYPE_LIMITED_FREESHIPPING_FOR_REGISTERED_USER = "06";	// 利用回数制限つき配送料無料会員用クーポン
		public const string FLG_COUPONCOUPON_TYPE_LIMITED_BIRTHDAY_FOR_REGISTERED_USER = "07";	// 利用回数制限つき誕生日クーポン
		public const string FLG_COUPONCOUPON_TYPE_LIMITED_BIRTHDAY_FREESHIPPING_FOR_REGISTERED_USER = "08";	// 利用回数制限つき配送料無料誕生日クーポン
		public const string FLG_COUPONCOUPON_TYPE_UNLIMIT = "11";	// 無制限利用可能会員用クーポン
		public const string FLG_COUPONCOUPON_TYPE_ALL_UNLIMIT = "12";	// 無制限利用可能クーポン
		public const string FLG_COUPONCOUPON_TYPE_ALL_LIMIT = "21";	// 回数制限ありクーポン
		public const string FLG_COUPONCOUPON_TYPE_LIMITED_FREESHIPPING = "31";	// 回数制限あり配送無料クーポン
		public const string FLG_COUPONCOUPON_TYPE_BLACKLIST_FOR_REGISTERED_USER = "41";	// 会員用ブラックリスト型クーポン
		public const string FLG_COUPONCOUPON_TYPE_BLACKLIST_FREESHIPPING_FOR_REGISTERED_USER = "42";	// 会員用ブラックリスト型配送無料クーポン
		public const string FLG_COUPONCOUPON_TYPE_BLACKLIST_FOR_ALL = "43";	// ブラックリスト型クーポン
		public const string FLG_COUPONCOUPON_TYPE_BLACKLIST_FREESHIPPING_FOR_ALL = "44";	// ブラックリスト型配送無料クーポン
		/// <summary>紹介クーポン：紹介された人に発行</summary>
		public const string FLG_COUPONCOUPON_TYPE_ISSUED_TO_INTRODUCED_PERSON = "45";
		/// <summary>紹介クーポン：紹介された人が購入後に紹介した人に発行</summary>
		public const string FLG_COUPONCOUPON_TYPE_ISSUED_TO_PERSON_INTRODUCED_AFTER_PURCHASE_BY_INTRODUCED_PERSON = "46";
		/// <summary>紹介クーポン：紹介された人が会員登録後に紹介した人に発行</summary>
		public const string FLG_COUPONCOUPON_TYPE_ISSUED_TO_PERSON_INTRODUCED_BY_INTRODUCED_PERSON_AFTER_MEMBERSHIP_REGISTRATION = "47";

		// クーポン：クーポン対象商品区分
		public const string FLG_COUPON_PRODUCT_KBN_TARGET = "01";	// 全ての商品を対象
		public const string FLG_COUPON_PRODUCT_KBN_UNTARGET = "02";	// 全ての商品を対象としない、対象商品を限定する（または）
		public const string FLG_COUPON_PRODUCT_KBN_UNTARGET_BY_LOGICAL_AND = "03";	// 全ての商品を対象としない、対象商品を限定する（かつ）

		// クーポン：クーポン例外商品アイコン
		public const int FLG_COUPON_EXCEPTIONAL_ICON1 = 2;
		public const int FLG_COUPON_EXCEPTIONAL_ICON2 = 4;
		public const int FLG_COUPON_EXCEPTIONAL_ICON3 = 8;
		public const int FLG_COUPON_EXCEPTIONAL_ICON4 = 16;
		public const int FLG_COUPON_EXCEPTIONAL_ICON5 = 32;
		public const int FLG_COUPON_EXCEPTIONAL_ICON6 = 64;
		public const int FLG_COUPON_EXCEPTIONAL_ICON7 = 128;
		public const int FLG_COUPON_EXCEPTIONAL_ICON8 = 256;
		public const int FLG_COUPON_EXCEPTIONAL_ICON9 = 512;
		public const int FLG_COUPON_EXCEPTIONAL_ICON10 = 1024;

		// クーポン：クーポン併用フラグ
		public const string FLG_COUPON_USE_TOGETHER_FLG_UNUSE = "0";	// 併用不可
		public const string FLG_COUPON_USE_TOGETHER_FLG_USE = "1";	// 併用可能

		// クーポン：有効フラグ
		public const string FLG_COUPON_VALID_FLG_VALID = "1";	// 有効
		public const string FLG_COUPON_VALID_FLG_INVALID = "0";	// 無効

		// クーポン：ユーザ表示フラグ
		public const string FLG_COUPON_DISP_FLG_ON = "1";	// 表示
		public const string FLG_COUPON_DISP_FLG_OFF = "0";	// 非表示

		// クーポン：配送料無料有効フラグ
		public const string FLG_COUPON_FREE_SHIPPING_VALID = "1";	// 有効
		public const string FLG_COUPON_FREE_SHIPPING_INVALID = "0";	// 無効

		// ユーザクーポン：利用フラグ
		public const string FLG_USERCOUPON_USE_FLG_UNUSE = "0";	// 未利用
		public const string FLG_USERCOUPON_USE_FLG_USE = "1";	// 利用済

		// ユーザクーポン履歴：クーポン履歴区分
		public const string FLG_USERCOUPONHISTORY_HISTORY_KBN_PUBLISH = "01";	// 発行
		public const string FLG_USERCOUPONHISTORY_HISTORY_KBN_USE = "10";		// 利用
		public const string FLG_USERCOUPONHISTORY_HISTORY_KBN_EXPIRE = "11";	// 有効期限切れ削除
		public const string FLG_USERCOUPONHISTORY_HISTORY_KBN_FIXEDPURCHASE_USE = "20";			// 利用（次回定期購入）
		public const string FLG_USERCOUPONHISTORY_HISTORY_KBN_FIXEDPURCHASE_ADJUSTMENT = "21";	// 利用調整（次回定期購入）
		public const string FLG_USERCOUPONHISTORY_HISTORY_KBN_FIXEDPURCHASE_USE_CANCEL = "22";	// 利用キャンセル（次回定期購入）
		public const string FLG_USERCOUPONHISTORY_HISTORY_KBN_USE_CANCEL = "30";	// 利用キャンセル
		public const string FLG_USERCOUPONHISTORY_HISTORY_KBN_PUBLISH_CANCEL = "31";	// 発行キャンセル

		// ユーザクーポン履歴：操作区分
		public const string FLG_USERCOUPONHISTORY_ACTION_KBN_BASE = "01";		// 基本
		public const string FLG_USERCOUPONHISTORY_ACTION_KBN_MASTER_UPLOAD = "97";	// マスタアップロード
		public const string FLG_USERCOUPONHISTORY_ACTION_KBN_ORDER = "98";		// オペレータ注文変更
		public const string FLG_USERCOUPONHISTORY_ACTION_KBN_OPERATOR = "99";	// オペレータ調整

		// クーポン利用ユーザー：クーポン利用済みユーザー判定方法パラメタ名
		public const string FLG_COUPONUSEUSER_USED_USER_JUDGE_TYPE = "used_user_judge_type";

		// クーポン発行スケジュール：ステータス
		public const string FLG_COUPONSCHEDULE_STATUS_NORMAL = "00"; // 通常
		public const string FLG_COUPONSCHEDULE_STATUS_EXTRACT = "01"; // 抽出中
		public const string FLG_COUPONSCHEDULE_STATUS_UPDATE = "02"; // 更新中
		public const string FLG_COUPONSCHEDULE_STATUS_ERROR = "09"; // 更新エラー

		// クーポン発行スケジュール：ターゲット抽出フラグ
		public const string FLG_COUPONSCHEDULE_TARGET_EXTRACT_FLG_ON = "1";
		public const string FLG_COUPONSCHEDULE_TARGET_EXTRACT_FLG_OFF = "0";

		// クーポン発行スケジュール：実行タイミング
		public const string FLG_COUPONSCHEDULE_EXEC_TIMING_MANUAL = "01"; // 手動実行
		public const string FLG_COUPONSCHEDULE_EXEC_TIMING_SCHEDULE = "02"; // スケジュール実行

		// クーポン発行スケジュール：スケジュール区分
		public const string FLG_COUPONSCHEDULE_SCHEDULE_KBN_DAY = "01"; // 日単位（毎日HH:mm:ssに実行）
		public const string FLG_COUPONSCHEDULE_SCHEDULE_KBN_WEEK = "02"; // 週単位（毎週ddd曜日HH:mm:ssに実行）
		public const string FLG_COUPONSCHEDULE_SCHEDULE_KBN_MONTH = "03"; // 月単位（毎月dd日HH:mm:ssに実行）
		//public const string FLG_COUPONSCHEDULE_SCHEDULE_KBN_YEAR = "04"; // 年単位（毎年MM月dd日HH:mm:ssに実行）
		public const string FLG_COUPONSCHEDULE_SCHEDULE_KBN_ONCE = "05"; // 一回のみ（一回のみyyyy年MM月dd日HH:mm:ssに実行）

		// クーポン発行スケジュール：有効フラグ
		public const string FLG_COUPONSCHEDULE_VALID_FLG_VALID = "ON"; // 有効
		public const string FLG_COUPONSCHEDULE_VALID_FLG_INVALID = "OFF"; // 無効

		// ターゲットリスト設定マスタ：ステータス
		/// <summary>通常</summary>
		public const string FLG_TARGETLIST_STATUS_NORMAL = "00";
		/// <summary>抽出中</summary>
		public const string FLG_TARGETLIST_STATUS_EXTRACT = "01";
		/// <summary>利用中</summary>
		public const string FLG_TARGETLIST_STATUS_USING = "02";
		/// <summary>抽出エラー</summary>
		public const string FLG_TARGETLIST_STATUS_ERROR = "09";

		// ターゲットリスト 有効フラグ
		/// <summary>ターゲットリスト有効フラグ:有効</summary>
		public const string FLG_TARGETLIST_VALID_FLG_VALID = "1";
		/// <summary>ターゲットリスト有効フラグ:無効</summary>
		public const string FLG_TARGETLIST_VALID_FLG_INVALID = "0";

		/// <summary>ターゲットリスト削除フラグ:削除済み</summary>
		public const string FLG_TARGETLIST_DEL_FLG_VALID = "1";
		/// <summary>ターゲットリスト削除フラグ:削除されていない</summary>
		public const string FLG_TARGETLIST_DEL_FLG_INVALID = "0";

		// ターゲットリスト設定マスタ：ターゲット種別
		public const string FLG_TARGETLIST_TARGET_TYPE_MANUAL = "Manual";
		public const string FLG_TARGETLIST_TARGET_TYPE_CSV = "UploadCsv";							// アップロードターゲットリスト
		public const string FLG_TARGETLIST_TARGET_TYPE_MERGE = "MergeTargetList";					// ターゲットリストマージ
		public const string FLG_TARGETLIST_TARGET_TYPE_USER_LIST = "UserList";						// ユーザー情報一覧
		public const string FLG_TARGETLIST_TARGET_TYPE_ORDER_LIST = "OrderList";					// 受注情報一覧
		public const string FLG_TARGETLIST_TARGET_TYPE_ORDERWORKFLOW_LIST = "OrderWorkflowList";	// 受注ワークフロー一覧
		public const string FLG_TARGETLIST_TARGET_TYPE_FIXEDPURCHASE_LIST = "FixedPurchaseList";	// 定期購入情報一覧
		public const string FLG_TARGETLIST_TARGET_TYPE_FIXEDPURCHASE_WORKFLOW_LIST = "FpWorkflowList";	// 定期ワークフロー一覧
		/// <summary>Target type: fixed purchase repeat analysis report</summary>
		public const string FLG_TARGETLIST_TARGET_TYPE_FIXEDPURCHASE_REPEAT_ANALYSIS_REPORT = "FpAnalysisLtvReport";

		// ターゲットリスト設定マスタ：実行タイミング
		public const string FLG_TARGETLIST_EXEC_TIMING_MANUAL = "01";
		public const string FLG_TARGETLIST_EXEC_TIMING_SCHEDULE = "02";

		// ターゲットリスト設定マスタ：スケジュール区分
		public const string FLG_TARGETLIST_SCHEDULE_KBN_DAY = "01";
		public const string FLG_TARGETLIST_SCHEDULE_KBN_WEEK = "02";
		public const string FLG_TARGETLIST_SCHEDULE_KBN_MONTH = "03";
		//public const string FLG_TARGETLIST_SCHEDULE_KBN_YEAR = "04";
		public const string FLG_TARGETLIST_SCHEDULE_KBN_ONCE = "05";

		// ターゲットリストデータ：ターゲットデータ区分
		public const string FLG_TARGETLISTDATA_TARGET_KBN_TARGETLIST = "TargetList";
		public const string FLG_TARGETLISTDATA_TARGET_KBN_MAILDIST = "MailDist";
		public const string FLG_TARGETLISTDATA_TARGET_KBN_MEMBERRANKRULE = "MemberRankRule";		// 会員ランク付与ルール

		// ターゲットリストデータ：メールアドレス区分
		public const string FLG_TARGETLISTDATA_MAIL_ADDR_KBN_PC = "PC";
		public const string FLG_TARGETLISTDATA_MAIL_ADDR_KBN_MOBILE = "MB";

		// メール配信設定マスタ：ステータス
		public const string FLG_MAILDISTSETTING_STATUS_NORMAL = "00";
		public const string FLG_MAILDISTSETTING_STATUS_SEND = "01";
		public const string FLG_MAILDISTSETTING_STATUS_ERROR = "09";

		// メール配信設定マスタ：モバイルメール排除フラグ
		public const string FLG_MAILDISTSETTING_EXCEPT_MOBILEMAIL_FLG_ON = "1";
		public const string FLG_MAILDISTSETTING_EXCEPT_MOBILEMAIL_FLG_OFF = "0";

		// メール配信設定マスタ：ターゲット抽出フラグ
		public const string FLG_MAILDISTSETTING_TARGET_EXTRACT_FLG_ON = "1";
		public const string FLG_MAILDISTSETTING_TARGET_EXTRACT_FLG_OFF = "0";

		// メール配信設定マスタ：実行タイミング
		public const string FLG_MAILDISTSETTING_EXEC_TIMING_MANUAL = "01";
		public const string FLG_MAILDISTSETTING_EXEC_TIMING_SCHEDULE = "02";

		// メール配信設定マスタ：スケジュール区分
		public const string FLG_MAILDISTSETTING_SCHEDULE_KBN_DAY = "01";
		public const string FLG_MAILDISTSETTING_SCHEDULE_KBN_WEEK = "02";
		public const string FLG_MAILDISTSETTING_SCHEDULE_KBN_MONTH = "03";
		//public const string FLG_MAILDISTSETTING_SCHEDULE_KBN_YEAR = "04";
		public const string FLG_MAILDISTSETTING_SCHEDULE_KBN_ONCE = "05";

		// メール配信設定マスタ：有効フラグ
		public const string FLG_MAILDISTSETTING_VALID_FLG_VALID = "1";
		public const string FLG_MAILDISTSETTING_VALID_FLG_INVALID = "0";

		// メール配信設定マスタ：重複配信除外設定
		public const string FLG_MAILDISTSETTING_ENABLE_DEDUPLICATION_ENABLED = "1";
		public const string FLG_MAILDISTSETTING_ENABLE_DEDUPLICATION_DISABLED = "0";

		// メールクリックマスタ：PCモバイル区分
		public const string FLG_MAILCLICK_PCMOBILE_KBN_PC = "PC";
		public const string FLG_MAILCLICK_PCMOBILE_KBN_MOBILE = "MB";

		// メールクリックマスタ：有効フラグ
		public const string FLG_MAILCLICK_VALID_FLG_VALID = "1";
		public const string FLG_MAILCLICK_VALID_FLG_INVALID = "0";

		// タスクスケジュールマスタ：実行区分
		public const string FLG_TASKSCHEDULE_ACTION_KBN_CREATE_TARGETLIST = "CreateTargetList";
		public const string FLG_TASKSCHEDULE_ACTION_KBN_MAIL_DIST = "MailDist";
		public const string FLG_TASKSCHEDULE_ACTION_KBN_MEMBER_RANK_USER_EXTRACT = "MemberRankUserExtract";// 会員ランク付与ユーザー抽出
		public const string FLG_TASKSCHEDULE_ACTION_KBN_CHANGE_MEMBER_RANK = "ChangeMemberRank";	// 会員ランク付与
		public const string FLG_TACKSCHEDULE_ACTION_KBN_FIXED_PURCHASE_MAIL = "FixedPurchaseMail";// 定期購入メール送信
		public const string FLG_TACKSCHEDULE_ACTION_KBN_FIXED_PURCHASE_RESUME = "FixedPurchaseResume";	// 定期購入再開
		/// <summary>定期購入変更期限案内メール送信</summary>
		public const string FLG_TACKSCHEDULE_ACTION_KBN_FIXED_PURCHASE_CHANGE_DEADLINE_MAIL = "FPChangeDeadlineMail";
		public const string FLG_TASKSCHEDULE_ACTION_KBN_ADD_POINT = "AddPoint"; // ポイント付与
		public const string FLG_TASKSCHEDULE_ACTION_KBN_PUBLISH_COUPON = "PublishCoupon"; // クーポン発行
		/// <summary>Elogit wms upload</summary>
		public const string FLG_TASKSCHEDULE_ACTION_KBN_ELOGIT_UPLOAD = "upload";
		/// <summary>Elogit wms download</summary>
		public const string FLG_TASKSCHEDULE_ACTION_KBN_ELOGIT_DOWNLOAD = "download";
		/// <summary>Workflow target count aggregate</summary>
		public const string FLG_TASKSCHEDULE_ACTION_KBN_WORKFLOW_TARGET_COUNT_AGGREGATE = "WorkflowTargetCountAggregate";

		// タスクスケジュールマスタ：準備ステータス
		public const string FLG_TASKSCHEDULE_EXECUTE_STATUS_INIT = "00";
		public const string FLG_TASKSCHEDULE_EXECUTE_STATUS_EXECUTE = "01";
		public const string FLG_TASKSCHEDULE_EXECUTE_STATUS_STOP = "05";
		public const string FLG_TASKSCHEDULE_EXECUTE_STATUS_SKIP = "08";
		public const string FLG_TASKSCHEDULE_EXECUTE_STATUS_ERROR = "09";
		public const string FLG_TASKSCHEDULE_EXECUTE_STATUS_DONE = "10";

		// タスクスケジュールマスタ：実行ステータス
		public const string FLG_TASKSCHEDULE_PREPARE_STATUS_INIT = "00";
		public const string FLG_TASKSCHEDULE_PREPARE_STATUS_EXECUTE = "01";
		public const string FLG_TASKSCHEDULE_PREPARE_STATUS_STOP = "05";
		public const string FLG_TASKSCHEDULE_PREPARE_STATUS_SKIP = "08";
		public const string FLG_TASKSCHEDULE_PREPARE_STATUS_ERROR = "09";
		public const string FLG_TASKSCHEDULE_PREPARE_STATUS_DONE = "10";

		// タスクスケジュールマスタ：停止フラグ
		public const string FLG_TASKSCHEDULE_STOP_FLG_ON = "1";
		public const string FLG_TASKSCHEDULE_STOP_FLG_OFF = "0";

		// タスクスケジュール履歴マスタ：実行結果
		public const string FLG_TASKSCHEDULEHISTORY_ACTION_RESULT_MAILSEND_OK = "MLOK";
		public const string FLG_TASKSCHEDULEHISTORY_ACTION_RESULT_MAILSEND_NG = "MLNG";
		public const string FLG_TASKSCHEDULEHISTORY_ACTION_RESULT_MAILSEND_NOBODY = "MLNB";
		public const string FLG_TASKSCHEDULEHISTORY_ACTION_RESULT_MAILSEND_NOTARGET = "MLNT";

		// タスクスケジュール履歴マスタ：実行区分
		public const string FLG_TASKSCHEDULEHISTORY_ACTION_KBN_MAIL_DIST = "MailDist";
		public const string FLG_TASKSCHEDULEHISTORY_ACTION_KBN_PUBLISH_COUPON = "PublishCoupon";

		// タスクスケジュール履歴マスタ：詳細実行区分
		public const string FLG_TASKSCHEDULEHISTORY_ACTION_KBN_DETAIL_MAIL_DIST = "MailDist";

		// 広告コードマスタ：有効フラグ
		public const string FLG_ADVCODE_VALID_FLG_VALID = "1";		// 有効
		public const string FLG_ADVCODE_VALID_FLG_INVALID = "0";	// 無効

		// 会員ランク：注文割引方法
		public const string FLG_MEMBERRANK_ORDER_DISCOUNT_TYPE_NONE = "NONE";						// 割引きしない
		public const string FLG_MEMBERRANK_ORDER_DISCOUNT_TYPE_RATE = "RATE";						// 割引率指定
		public const string FLG_MEMBERRANK_ORDER_DISCOUNT_TYPE_FIXED = "FIXED";						// 割引金額指定

		// 会員ランク：ポイント加算方法
		public const string FLG_MEMBERRANK_POINT_ADD_TYPE_NONE = "NONE";							// ポイントを加算しない
		public const string FLG_MEMBERRANK_POINT_ADD_TYPE_RATE = "RATE";							// 加算率指定

		// 会員ランク：配送料割引方法
		public const string FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_NONE = "NONE";					// 配送手数料割引しない
		public const string FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_FIXED = "FIXED";					// 割引金額指定
		public const string FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_FREE = "FREE";					// 配送手数料無料
		public const string FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_FRESHP_TSD = "FRESHP_TSD";		// 配送料無料最低金額設定

		// 会員ランク：デフォルト会員ランク設定フラグ
		public const string FLG_MEMBERRANK_DEFAULT_MEMBER_RANK_SETTING_FLG_ON = "ON";				// デフォルトの会員ランク
		public const string FLG_MEMBERRANK_DEFAULT_MEMBER_RANK_SETTING_FLG_OFF = "OFF";				// デフォルトの会員ランクではない

		// 会員ランク：有効フラグ
		public const string FLG_MEMBERRANK_VALID_FLG_VALID = "ON";									// 有効
		public const string FLG_MEMBERRANK_VALID_FLG_INVALID = "OFF";								// 無効

		// 会員ランク付与ルール：ステータス
		public const string FLG_MEMBERRANKRULE_STATUS_NORMAL = "00";								// 通常
		public const string FLG_MEMBERRANKRULE_STATUS_EXTRACT = "01";								// 抽出中
		public const string FLG_MEMBERRANKRULE_STATUS_UPDATE = "02";								// 更新中
		public const string FLG_MEMBERRANKRULE_STATUS_ERROR = "09";									// 更新エラー

		// 会員ランク付与ルール：抽出条件集計期間指定
		public const string FLG_MEMBERRANKRULE_TARGET_EXTRACT_TYPE_DURING = "DURING";				// 期間指定
		public const string FLG_MEMBERRANKRULE_TARGET_EXTRACT_TYPE_DAYSAGO = "DAYSAGO";				// 前日指定

		// 会員ランク付与ルール：抽出時の旧ランク情報抽出判定
		public const string FLG_MEMBERRANKRULE_TARGET_EXTRACT_OLD_RANK_FLG_ON = "ON";				// 旧ランク情報を含む
		public const string FLG_MEMBERRANKRULE_TARGET_EXTRACT_OLD_RANK_FLG_OFF = "OFF";				// 旧ランク情報を含まない

		// 会員ランク付与ルール：ランク付与方法
		public const string FLG_MEMBERRANKRULE_RANK_CHANGE_TYPE_UP = "UP";							// ランクアップ用
		public const string FLG_MEMBERRANKRULE_RANK_CHANGE_TYPE_DOWN = "DOWN";						// ランクダウン用

		// 会員ランク付与ルール：実行タイミング
		public const string FLG_MEMBERRANKRULE_EXEC_TIMING_MANUAL = "01";							// 手動実行
		public const string FLG_MEMBERRANKRULE_EXEC_TIMING_SCHEDULE = "02";							// スケジュール実行

		// 会員ランク付与ルール：スケジュール区分
		public const string FLG_MEMBERRANKRULE_SCHEDULE_KBN_DAY = "01";								// 日単位（毎日HH:mm:ssに実行）
		public const string FLG_MEMBERRANKRULE_SCHEDULE_KBN_WEEK = "02";							// 週単位（毎週ddd曜日HH:mm:ssに実行）
		public const string FLG_MEMBERRANKRULE_SCHEDULE_KBN_MONTH = "03";							// 月単位（毎月dd日HH:mm:ssに実行）
		//public const string FLG_MEMBERRANKRULE_SCHEDULE_KBN_YEAR = "04";							// 年単位（毎年MM月dd日HH:mm:ssに実行）
		public const string FLG_MEMBERRANKRULE_SCHEDULE_KBN__ONCE = "05";							// 一回のみ（一回のみyyyy年MM月dd日HH:mm:ssに実行）

		// 会員ランク付与ルール：有効フラグ
		public const string FLG_MEMBERRANKRULE_VALID_FLG_VALID = "ON";								// 有効
		public const string FLG_MEMBERRANKRULE_VALID_FLG_INVALID = "OFF";							// 無効

		// 会員ランク付与ルール：ターゲット抽出フラグ
		/// <summary> 1：配信時に抽出する </summary>
		public const string FLG_MEMBERRANKRULE_TARGET_EXTRACT_FLG_ON = "1";
		/// <summary> 0：配信時に抽出しない </summary>
		public const string FLG_MEMBERRANKRULE_TARGET_EXTRACT_FLG_OFF = "0";

		// アフィリエイト連携ログ：アフィリエイト区分
		public const string FLG_AFFILIATECOOPLOG_AFFILIATE_KBN_LINKSHARE_REP = "LINKSHARE_REP";		// リンクシェアアフィリエイト成果報告用
		public const string FLG_AFFILIATECOOPLOG_AFFILIATE_KBN_PC = "PC";							// 汎用アフィリエイト(PC)
		public const string FLG_AFFILIATECOOPLOG_AFFILIATE_KBN_MOBILE = "MOBILE";					// 汎用アフィリエイト(モバイル)

		// アフィリエイト連携ログ：連携ステータス
		public const string FLG_AFFILIATECOOPLOG_COOP_STATUS_WAIT = "WAIT";							// 連携待ち
		public const string FLG_AFFILIATECOOPLOG_COOP_STATUS_DONE = "DONE";							// 連携済
		public const string FLG_AFFILIATECOOPLOG_COOP_STATUS_NONE = "NONE";							// 連携なし

		// アフィリエイトタグ設定：アフィリエイト区分
		public const string FLG_AFFILIATETAGSETTING_AFFILIATE_KBN_PC_SP = "PC/SP";					// PC/SP
		public const string FLG_AFFILIATETAGSETTING_AFFILIATE_KBN_PC = "PC";						// PC
		public const string FLG_AFFILIATETAGSETTING_AFFILIATE_KBN_SP = "SP";						// SP
		public const string FLG_AFFILIATETAGSETTING_AFFILIATE_KBN_MOBILE = "Mobile";				// モバイル

		// アフィリエイトタグ設定：ユーザーエージェント連携区分
		public const string FLG_AFFILIATETAGSETTING_USER_AGENT_COOP_KBN_SERVER = "SERVER";			// サーバー
		public const string FLG_AFFILIATETAGSETTING_USER_AGENT_COOP_KBN_USER = "USER";				// ユーザー端末

		// アフィリエイトタグ設定：有効フラグ
		public const string FLG_AFFILIATETAGSETTING_VALID_FLG_UNVALID = "0";						// 無効
		public const string FLG_AFFILIATETAGSETTING_VALID_FLG_VALID = "1";							// 有効

		// アフィリエイトタグ設定 : 出力箇所
		public const string FLG_AFFILIATETAGSETTING_OUTPUT_LOCATION_BODY_TOP = "body_top";			// body 上部
		public const string FLG_AFFILIATETAGSETTING_OUTPUT_LOCATION_BODY_BOTTOM = "body_bottom";	// body 下部
		public const string FLG_AFFILIATETAGSETTING_OUTPUT_LOCATION_HEAD = "head";					// head

		// アフィリエイトタグ条件設定 : 条件タイプ
		public const string FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_PAGE = "PAGE";									// 画面
		public const string FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_ADCODE_MEDIA_TYPE = "ADCODE_MEDIA_TYPE";		// 広告媒体区分
		public const string FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_ADVERTISEMENT_CODE = "ADVERTISEMENT_CODE";		// 広告コード
		public const string FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_PRODUCT_ID = "PRODUCT_ID";						// 商品ID

		// アフィリエイトタグ条件設定 : 一致条件タイプ
		public const string FLG_AFFILIATETAGCONDITION_MATCH_TYPE_PERFECT = "PERFECT";								// 完全一致
		public const string FLG_AFFILIATETAGCONDITION_MATCH_TYPE_FORWARD = "FORWARD";								// 前方一致

		/// <summary>タグ閲覧権限設定区分：タグID</summary>
		public const string FLG_TAG_AUTHORITY_KBN_TAG = "TAG";
		/// <summary>タグ閲覧権限設定区分：広告媒体区分</summary>
		public const string FLG_TAG_AUTHORITY_KBN_MEDIA_TYPE = "MEDIATYPE";
		/// <summary>タグ閲覧権限設定区分：設置個所</summary>
		public const string FLG_TAG_AUTHORITY_KBN_LOCATION = "LOCATION";
		/// <summary>タグ閲覧権限設定区分表示文言：タグID</summary>
		public const string FLG_TAG_AUTHORITY_TEXT_TAG_ID = "タグID";
		/// <summary>タグ閲覧権限設定区分表示文言：広告媒体区分</summary>
		public const string FLG_TAG_AUTHORITY_TEXT_MEDIA_TYPE = "広告媒体区分";
		/// <summary>タグ閲覧権限設定区分表示文言：設置個所</summary>
		public const string FLG_TAG_AUTHORITY_TEXT_LOCATION = "設置箇所";
		/// <summary>タグ閲覧権限設定 一括操作フラグ：一括選択</summary>
		public const string FLG_TAG_AUTHORITY_ISCHECKED_VALID = "1";
		/// <summary>タグ閲覧権限設定 一括操作フラグ：一括解除</summary>
		public const string FLG_TAG_AUTHORITY_ISCHECKED_INVALID = "0";
		#endregion
	}
}
