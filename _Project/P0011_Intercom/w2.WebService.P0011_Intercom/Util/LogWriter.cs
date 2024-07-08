/*
=========================================================================================================
  Module      : ログ用ファイル用ユーティリティクラス(LogFileUtil.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2011 All Rights Reserved.
  MEMO        : 各種ログファイルユーティリティ。
=========================================================================================================
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Data;
using System.IO;
using System.Text;

namespace w2.Plugin.P0011_Intercom.WebService.Util
{
	public class LogWriter : FileWriterBase 
	{
		private ReceiveDataWriter m_receive = null;
		private ErrorLogWriter m_error = null;
		private ErrorCsvLogWriter m_csv = null;
	
		internal LogWriter()
		{
			m_receive = new ReceiveDataWriter();
			m_error = new ErrorLogWriter();
			m_csv = new ErrorCsvLogWriter();
		}

		public void writedebuglog(string target)
		{
			WriteFile(@"C:\WebServiceTest\deblog.txt",target);
		}

		public void WriteReceiveDataLog(DataSet ds)
		{
			m_receive.write(ds);
		}

		public void WriteErrorLog(DataSet ds,Exception ex)
		{
			m_error.write(ds, ex);
		}

		public void WriteErrorCSV(DataTable dt, string proc)
		{
			m_csv.write(dt, proc);
		}
	}
}