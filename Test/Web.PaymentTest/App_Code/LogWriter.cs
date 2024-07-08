using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

/// <summary>
/// LogWriter の概要の説明です
/// </summary>
public class LogWriter
{
	/// <summary>ログ区分</summary>
	public enum LogKbnType
	{
		/// <summary>ソフトバンクペイメント</summary>
		SBPS
	}

	/// <summary>
	/// ログ書き込み
	/// </summary>
	/// <param name="logKbn">ログ区分(例：SBPS)</param>
	/// <param name="logType">ログタイプ（Error.aspxなど）</param>
	/// <param name="form">フォーム</param>
	public static void Write(LogKbnType logKbn, string logType, System.Collections.Specialized.NameValueCollection form)
	{
		StringBuilder formString = new StringBuilder();
		formString.Append("[").Append(logType).Append("]\r\n");
		formString.Append("---------------------------\r\n");
		foreach (string key in form.Keys)
		{
			formString.Append("").Append(key).Append("\t\t").Append(form[key]).Append("\r\n");
		}
		formString.Append("---------------------------\r\n\r\n");
		w2.Common.Logger.FileLogger.Write(logKbn.ToString(), formString.ToString(), true);
	}
}