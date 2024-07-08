using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace w2.Plugin.P0011_Intercom.ReSync.Util
{
	public class ErrorLogWriter : FileWriterBase
	{
		private const string FileName = "ErrorLog.txt";
		private string m_fileDir = "";

		public ErrorLogWriter(string logDir)
		{
			m_fileDir = logDir;
		}

		public void write(string writeStr)
		{
			string writewilepath = m_fileDir + DateTime.Now.ToString("yyyyMMdd") + "_" + FileName;
			base.WriteFile(writewilepath, writeStr);

		}

		#region NullまたはDBNullかどうかをチェック

		/// <summary>
		/// NullまたはDBNullの場合にはEmptyに
		/// </summary>
		/// <param name="targetValue"></param>
		/// <returns></returns>
		private string ConvNullToEmpty(object targetValue)
		{
			if (targetValue == null)
			{
				return "";
			}

			if (targetValue == DBNull.Value)
			{
				return "";
			}

			return targetValue.ToString();

		}

		#endregion
	}

}
