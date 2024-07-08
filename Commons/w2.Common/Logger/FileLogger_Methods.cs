/*
=========================================================================================================
  Module      : ファイルロガー共通メソッド部分(FileLogger_Methods.cs)
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
	/// ファイルロガークラス
	/// パーシャルクラスなのでDocumentXml生成上コメントは空としておく。
	/// 「xxxxxx_Methods.cs」は各ロガーのパーシャルクラスで内容は同じものとする。
	///*********************************************************************************************
	public partial class FileLogger : BaseLogger
	{
		/// <summary>
		/// デバッグ出力
		/// </summary>
		/// <param name="ex">例外</param>
		public static void WriteDebug(Exception ex)
		{
			WriteDebug("", ex);
		}
		/// <summary>
		/// デバッグ出力
		/// </summary>
		/// <param name="strMessage">メッセージ</param>
		/// <param name="ex">例外</param>
		public static void WriteDebug(string strMessage, Exception ex)
		{
			Write(LOGKBN_DEBUG, strMessage, ex);
		}
		/// <summary>
		/// デバッグ出力
		/// </summary>
		/// <param name="strMessage">メッセージ</param>
		public static void WriteDebug(string strMessage)
		{
			Write(LOGKBN_DEBUG, strMessage);
		}

		/// <summary>
		/// 情報出力
		/// </summary>
		/// <param name="ex">例外</param>
		public static void WriteInfo(Exception ex)
		{
			WriteInfo("", ex);
		}
		/// <summary>
		/// 情報出力
		/// </summary>
		/// <param name="strMessage">メッセージ</param>
		/// <param name="ex">例外</param>
		public static void WriteInfo(string strMessage, Exception ex)
		{
			Write(LOGKBN_INFO, strMessage, ex);
		}
		/// <summary>
		/// 情報出力
		/// </summary>
		/// <param name="strMessage">メッセージ</param>
		public static void WriteInfo(string strMessage)
		{
			Write(LOGKBN_INFO, strMessage);
		}

		/// <summary>
		/// 警告出力
		/// </summary>
		/// <param name="ex">例外</param>
		public static void WriteWarn(Exception ex)
		{
			WriteWarn("", ex);
		}
		/// <summary>
		/// 警告出力
		/// </summary>
		/// <param name="strMessage">メッセージ</param>
		/// <param name="ex">例外</param>
		public static void WriteWarn(string strMessage, Exception ex)
		{
			Write(LOGKBN_WARN, strMessage, ex);
		}
		/// <summary>
		/// 警告出力
		/// </summary>
		/// <param name="strMessage">メッセージ</param>
		public static void WriteWarn(string strMessage)
		{
			Write(LOGKBN_WARN, strMessage);
		}

		/// <summary>
		/// エラー出力
		/// </summary>
		/// <param name="ex">例外</param>
		public static void WriteError(Exception ex)
		{
			WriteError("", ex);
		}
		/// <summary>
		/// エラー出力
		/// </summary>
		/// <param name="strMessage">メッセージ</param>
		/// <param name="ex">例外</param>
		public static void WriteError(string strMessage, Exception ex)
		{
			Write(LOGKBN_ERROR, strMessage, ex);
		}
		/// <summary>
		/// エラー出力
		/// </summary>
		/// <param name="strMessage">メッセージ</param>
		public static void WriteError(string strMessage)
		{
			Write(LOGKBN_ERROR, strMessage);
		}

		/// <summary>
		/// 致命的エラー出力
		/// </summary>
		/// <param name="ex">例外</param>
		public static void WriteFatal(Exception ex)
		{
			WriteFatal("", ex);
		}
		/// <summary>
		/// 致命的エラー出力
		/// </summary>
		/// <param name="strMessage">メッセージ</param>
		/// <param name="ex">例外</param>
		public static void WriteFatal(string strMessage, Exception ex)
		{
			Write(LOGKBN_FATAL, strMessage, ex);
		}
		/// <summary>
		/// 致命的エラー出力
		/// </summary>
		/// <param name="strMessage">メッセージ</param>
		public static void WriteFatal(string strMessage)
		{
			Write(LOGKBN_FATAL, strMessage);
		}

		/// <summary>
		/// ログ書き込み処理
		/// </summary>
		/// <param name="strLogKbn">ログ区分</param>
		/// <param name="ex">例外</param>
		public static void Write(string strLogKbn, Exception ex)
		{
			Write(strLogKbn, "", ex);
		}
		/// <summary>
		/// ログ書き込み処理
		/// </summary>
		/// <param name="strLogKbn">ログ区分</param>
		/// <param name="strMessage">メッセージ</param>
		/// <param name="ex">例外</param>
		public static void Write(string strLogKbn, string strMessage, Exception ex)
		{
			Write(strLogKbn, CreateExceptionMessage(strMessage, ex));
		}
	}
}
