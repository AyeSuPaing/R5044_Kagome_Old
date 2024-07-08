/*
=========================================================================================================
  Module      : コンソールロガー(ConsoleLogger.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Logger;

namespace w2.Commerce.Reauth
{
	/// <summary>
	/// コンソールロガー
	/// </summary>
	public class ConsoleLogger
	{
		/// <summary>
		/// 情報書き込み
		/// </summary>
		/// <param name="message">メッセージ</param>
		/// <param name="args">パラメタ</param>
		public static void WriteInfo(string message, params object[] args)
		{
			Console.WriteLine(message, args);
			AppLogger.Write("info", string.Format(message, args), true);
		}

		/// <summary>
		/// エラー書き込み
		/// </summary>
		/// <param name="message">メッセージ</param>
		/// <param name="args">パラメタ</param>
		public static void WriteError(string message, params object[] args)
		{
			Console.WriteLine(message, args);
			AppLogger.WriteError(string.Format(message, args));
		}
		/// <summary>
		/// エラー書き込み
		/// </summary>
		/// <param name="ex">例外</param>
		public static void WriteError(Exception ex)
		{
			Console.WriteLine(ex.ToString());
			AppLogger.WriteError(ex);
		}
	}
}
