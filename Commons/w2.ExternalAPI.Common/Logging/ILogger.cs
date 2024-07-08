/*
=========================================================================================================
  Module      : ロガーのインターフェース(ILogger.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.ExternalAPI.Common.Logging
{
	public interface ILogger
	{
		/// <summary>
		/// ログ書き込み
		/// </summary>
		/// <param name="executedTime"></param>
		/// <param name="level"></param>
		/// <param name="source"></param>
		/// <param name="message"></param>
		/// <param name="data"></param>
		/// <param name="ex"></param>
		void Write(
			DateTime executedTime,
			LogLevel level,
			string source,
			string message,
			string data,
			Exception ex);
	}
}
