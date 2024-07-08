/*
=========================================================================================================
  Module      : アプリケーションロガー(AppLogger.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace w2.Common.Logger
{
	///*********************************************************************************************
	/// <summary>
	/// アプリケーションで利用する各ログを出力する
	/// </summary>
	///*********************************************************************************************
	public partial class AppLogger : BaseLogger
	{
		/// <summary>
		/// ログ書き込みを行う
		/// </summary>
		/// <param name="logKbn">ログ区分</param>
		/// <param name="message">エラーメッセージ</param>
		/// <param name="monthly">月ごとローテーション（指定なければ日ごと）</param>
		public static void Write(string logKbn, string message, bool monthly = false)
		{
			EventLogger.Write(logKbn, message);
			FileLogger.Write(logKbn, message, monthly);
		}
	}
}
