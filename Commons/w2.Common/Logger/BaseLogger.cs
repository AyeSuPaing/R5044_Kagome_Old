/*
=========================================================================================================
  Module      : 基底ロガー(BaseLogger.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.Common.Logger
{
	///*********************************************************************************************
	/// <summary>
	///	各ロガーが利用する基本となるメソッドを提供する
	/// </summary>
	///*********************************************************************************************
	public abstract class BaseLogger
	{
		//------------------------------------------------------
		// 書き込みログ区分
		//------------------------------------------------------
		/// <summary>ログ区分定数：ワイルドカード</summary>
		public const string LOGKBN_WILDCARD = "*";
		/// <summary>ログ区分定数：デバッグ</summary>
		public const string LOGKBN_DEBUG = "debug";
		/// <summary>ログ区分定数：情報</summary>
		public const string LOGKBN_INFO = "info";
		/// <summary>ログ区分定数：警告</summary>
		public const string LOGKBN_WARN = "warn";
		/// <summary>ログ区分定数：エラー</summary>
		public const string LOGKBN_ERROR = "error";
		/// <summary>ログ区分定数：致命的エラー</summary>
		public const string LOGKBN_FATAL = "fatal";

		/// <summary>書き込みログ設定リスト</summary>
		protected static List<string> KBN_LOGOUTPUT_SETTINGS = new List<string>();

		/// <summary>
		/// スタティックコンストラクタ
		/// </summary>
		static BaseLogger()
		{
			// 初期化
			KBN_LOGOUTPUT_SETTINGS.Add(BaseLogger.LOGKBN_WILDCARD);
		}

		/// <summary>
		/// アクセス許可設定更新
		/// </summary>
		/// <param name="strLogOutputKbns">ログ出力区分リスト</param>
		public static void UpdateOutputLogKbn(string strLogOutputKbns)
		{
			// クリア
			KBN_LOGOUTPUT_SETTINGS.Clear();

			// セット
			foreach (string strLogKbn in strLogOutputKbns.Replace(" ", "").Split(','))
			{
				AddOutputLogKbn(strLogKbn);
			}
		}
		/// <summary>
		/// アクセス許可設定追加
		/// </summary>
		/// <param name="strLogOutputKbn">ログ出力区分</param>
		public static void AddOutputLogKbn(string strLogOutputKbn)
		{
			if ((KBN_LOGOUTPUT_SETTINGS.Contains(strLogOutputKbn) == false)
				&& (strLogOutputKbn != "")
				&& (KBN_LOGOUTPUT_SETTINGS.Contains(BaseLogger.LOGKBN_WILDCARD) == false))
			{
				KBN_LOGOUTPUT_SETTINGS.Add(strLogOutputKbn);
			}
		}

		/// <summary>
		/// 例外メッセージ作成
		/// </summary>
		/// <param name="strMessage">メッセージ</param>
		/// <param name="ex">例外</param>
		/// <returns>例外メッセージ</returns>
		public static string CreateExceptionMessage(string strMessage, Exception ex)
		{
			return strMessage + Environment.NewLine + CreateExceptionMessage(ex);
		}
		/// <summary>
		/// 例外メッセージ生成
		/// </summary>
		/// <param name="ex">例外</param>
		/// <returns>例外メッセージ</returns>
		public static string CreateExceptionMessage(Exception ex)
		{
			StringBuilder sbErrorMessage = new StringBuilder();
			while (ex != null)
			{
				sbErrorMessage.Append("-> ").Append(ex.Message).Append(Environment.NewLine);
				sbErrorMessage.Append(ex.StackTrace).Append(Environment.NewLine);

				ex = ex.InnerException;
			}

			return sbErrorMessage.ToString();
		}
	}
}
