/*
=========================================================================================================
  Module      : パフォーマンス計測ロガー(PerformanceLogger.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using w2.Common.Util;

namespace w2.Common.Logger
{
	/// <summary>
	/// パフォーマンス計測ロガー
	/// </summary>
	public class PerformanceLogger
	{
		/// <summary>コールバックメソッド定義</summary>
		private static readonly WaitCallback m_waitCallbackForRequest = WriteLogInnerForRequest;
		/// <summary>コールバックメソッド定義</summary>
		private static readonly WaitCallback m_waitCallbackForSql = WriteLogInnerForSql;

		/// <summary>
		/// ログ書き込み（リクエスト向け）
		/// </summary>
		/// <param name="begin">開始</param>
		/// <param name="end">終了</param>
		/// <param name="message">メッセージ</param>
		public static void WriteForRequest(DateTime begin, DateTime end, string message)
		{
			ThreadPool.QueueUserWorkItem(
				m_waitCallbackForRequest,
				CreateTimeSpanString(begin, end) + " " + message);
		}

		/// <summary>
		/// ログ書き込み（SQL向け）
		/// </summary>
		/// <param name="begin">開始</param>
		/// <param name="end">終了</param>
		/// <param name="message">メッセージ</param>
		public static void WriteForSql(DateTime begin, DateTime end, string message)
		{
			ThreadPool.QueueUserWorkItem(
				m_waitCallbackForSql,
				CreateTimeSpanString(begin, end) + " " + message);
		}

		/// <summary>
		/// ログ書き込み（リクエスト向け）
		/// </summary>
		/// <param name="message">メッセージ</param>
		private static void WriteLogInnerForRequest(object message)
		{
			WriteLogInner("PerformanceReq", StringUtility.ToEmpty(message));
		}

		/// <summary>
		/// ログ書き込み（SQL向け）
		/// </summary>
		/// <param name="message">メッセージ</param>
		private static void WriteLogInnerForSql(object message)
		{
			WriteLogInner("PerformanceSql", StringUtility.ToEmpty(message));
		}

		/// <summary>
		/// タイムスパン文字列取得
		/// </summary>
		/// <param name="begin">開始</param>
		/// <param name="end">終了</param>
		/// <returns>タイムスパン文字列</returns>
		private static string CreateTimeSpanString(DateTime begin, DateTime end)
		{
			return (DateTime.Now - begin).ToString().PadRight(16, ' ');
		}

		/// <summary>
		/// ログ書き込み
		/// </summary>
		/// <param name="fileHead">ファイルヘッダ</param>
		/// <param name="message">メッセージ</param>
		private static void WriteLogInner(string fileHead, string message)
		{
			Thread.Sleep(5);
			FileLogger.Write(fileHead, message);
		}
	}
}
