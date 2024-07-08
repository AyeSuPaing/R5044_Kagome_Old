/*
=========================================================================================================
  Module      : 定数定義クラス(Constants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Text;
using w2.Common;
using w2.Common.Net.Mail;

namespace w2.MarketingPlanner.Win.ScheduleManager
{
	class Constants : w2.App.Common.Constants
	{
		/// <summary>メッセージデバッグ</summary>
		public static bool MESSAGE_DEBUG = false;

		/// <summary>ターゲットリスト抽出インターバル(分)</summary>
		public static int SEND_ALERTMAIL_INTERVAL_MINUTES = 0;

		/// <summary>スレッド実行の最大数</summary>
		public static int THREADS_MAX = 10;

		/// <summary>メールクリックURL（PC）</summary>
		public static string MAILCLICK_URL_PC = null;
		/// <summary>メールクリックURL（MB）</summary>
		public static string MAILCLICK_URL_MOBILE = null;

		/// <summary>デコメ画像ディレクトリ物理パス</summary>
		public static string PHYSICALDIRPATH_DECOMEIMAGE = null;

		/// <summary>メール送信エラーポイント（定期注文＆会員ランク変更メールは指定以上エラーポイントの場合送信しない）</summary>
		public static int SEND_MAIL_ERROR_POINT = 0;

		/// <summary>ログ区分</summary>
		public const string INTERIM = "interim";
		/// <summary>無効なEmailアドレスによるSMTPエラーレスポンス</summary>
		public static string PATTERN_SMTP_ERROR_RESPONSE_FOR_INVALID_EMAIL_ADDRESS;

		/// <summary>エラーメール送信用SMTPサーバ名</summary>
		public static string SERVER_SMTP_FOR_ERROR = "";
		/// <summary>エラーメール送信用SMTPサーバポート</summary>
		public static int SERVER_SMTP_PORT_FOR_ERROR = 25;
		/// <summary>エラーメール送信用SMTP認証タイプ</summary>
		public static SmtpAuthType SERVER_SMTP_AUTH_TYPE_FOR_ERROR = SmtpAuthType.Normal;
		/// <summary>エラーメール送信用SMTP認証用POPサーバー</summary>
		public static string SERVER_SMTP_AUTH_POP_SERVER_FOR_ERROR = "";
		/// <summary>エラーメール送信用SMTP認証用POPサーバーポート</summary>
		public static string SERVER_SMTP_AUTH_POP_PORT_FOR_ERROR = "";
		/// <summary>エラーメール送信用SMTP認証用POPサーバー認証タイプ(APOPか否か)</summary>
		public static PopType SERVER_SMTP_AUTH_POP_TYPE_FOR_ERROR = PopType.Pop;
		/// <summary>エラーメール送信用SMTP認証用POPサーバー認証ユーザー名</summary>
		public static string SERVER_SMTP_AUTH_USER_NAME_FOR_ERROR = "";
		/// <summary>エラーメール送信用SMTP認証用POPサーバー認証ユーザーパスワード</summary>
		public static string SERVER_SMTP_AUTH_PASSOWRD_FOR_ERROR = "";
		/// <summary>Workflow target count aggregate interval count</summary>
		public static int WORKFLOW_TARGET_COUNT_AGGREGATE_INTERVAL_HOUR = 0;
	}
}