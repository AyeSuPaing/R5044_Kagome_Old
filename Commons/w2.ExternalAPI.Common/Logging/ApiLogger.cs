/*
=========================================================================================================
  Module      : 外部連携用のログ書き込みを提供するクラス（ApiLogger.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.Common;

namespace w2.ExternalAPI.Common.Logging
{
	/// <summary>
	/// ログレベル列挙体
	/// </summary>
	public enum LogLevel
	{
		debug,
		info,
		warn,
		error,
		fatal
	}

	/// <summary>
	///	ログ書き込みのための抽象クラス
	/// </summary>
	/// <remarks>
	/// ログに書きこむ処理はこのクラスを実装すること
	/// 使うはかならずSetLogTargetメソッドを実行してTarget情報をセットすること
	/// 一回SetLogTargetを実行していれば以降はしなくて良いです
	/// </remarks>
	public static class ApiLogger
	{
		#region 定数
		private static ILogger m_logger = null;
		private static ExecuteTarget m_executeTarget;
		private static readonly object m_lockObject = new object();
		#endregion

		#region +SetLogTarget ターゲット情報セット
		/// <summary>
		/// ターゲット情報セット
		/// </summary>
		/// <param name="target">ログ書き込み時に記載するAPIID等の情報をもつターゲット情報</param>
		public static void SetLogTarget(ExecuteTarget target)
		{
			m_executeTarget = target;
		}
		#endregion

		#region +SetLogger ロガーセット
		/// <summary>
		/// ロガーセット
		/// </summary>
		/// <param name="logger">ロガー</param>
		public static void SetLogger(ILogger logger)
		{
			m_logger = logger;
		}
		#endregion

		#region +Write ログ書き込み
		/// <summary>
		/// ログ書き込み（Exception指定パターン）
		/// </summary>
		/// <param name="logLevel">書き込むログのログレベル</param>
		/// <param name="message">ログに書き込むメッセージ</param>
		/// <param name="data">ログに書き込むデータ内容</param>
		/// <param name="ex">ログに書き込むエラーの情報</param>
		public static void Write(LogLevel logLevel, string message, string data, Exception ex = null)
		{
			// APIログを出力しない場合、処理を抜ける
			// ※ただし、ターゲットがセットされていない場合はAPIログを出力
			if ((m_executeTarget != null) && (m_executeTarget.WriteLog == false)) return;

			lock (m_lockObject)
			{
				if (m_logger == null) m_logger = new DbLogger();
			}

			if (m_executeTarget == null)
			{
				WriteWithoutTarget(logLevel, Constants.APPLICATION_NAME, message, data, ex);
			}
			else
			{
				try
				{
					m_logger.Write(m_executeTarget.ExecutedTime, logLevel, Constants.APPLICATION_NAME + ", " + m_executeTarget.ToString(), message, data, ex);
				}
				catch (Exception ex2)
				{
					// ｗ２標準ログに出力
					w2.Common.Logger.AppLogger.Write(Enum.GetName(typeof(LogLevel), LogLevel.fatal), "DBログに書き込み失敗しました", ex2);
					throw new Exception("DBログに書き込み失敗しました");
				}

			}
		}
		#endregion

		#region -WriteWithoutTarget
		/// <summary>
		/// ログ書き込み（ターゲット情報未指定の場合）（Exception指定パターン）
		/// </summary>
		/// <param name="logLevel">書き込むログのログレベル</param>
		/// <param name="source">ログに書き込むソース情報</param>
		/// <param name="message">ログに書き込みメッセージ</param>
		/// <param name="data">ログに書き込むデータ内容</param>
		/// <param name="ex">例外</param>
		private static void WriteWithoutTarget(LogLevel logLevel, string source, string message, string data, Exception ex=null)
		{
			lock (m_lockObject)
			{
				if (m_logger == null) m_logger = new DbLogger();
			}

			try
			{
				m_logger.Write(DateTime.Now, logLevel, source, message, data, ex);
			}
			catch (Exception ex2)
			{
				// ｗ２標準ログに出力
				w2.Common.Logger.AppLogger.Write(Enum.GetName(typeof(LogLevel), LogLevel.fatal), "DBログに書き込み失敗しました", ex2);
				throw new Exception("DBログに書き込み失敗しました");
			}
		}
		#endregion
	}
}
