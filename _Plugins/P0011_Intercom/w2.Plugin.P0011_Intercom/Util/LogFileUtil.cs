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

namespace w2.Plugin.P0011_Intercom.Util
{
	class LogFileUtil
	{
		private DataSetLogWriter m_dswriter = null;
		private DebugLogWriter m_debugwriter = null;
		private ErrorCsvLogWriter m_csv = null;
	
		internal LogFileUtil()
		{
			m_dswriter = new DataSetLogWriter();
			m_debugwriter = new DebugLogWriter(@"c:\WebServiceTest\debuglog.txt");
			m_csv = new ErrorCsvLogWriter();
		}

		public void writeDebugLog(string writeStr)
		{
			try
			{
				if (CommUtil._ConfigUtil().GetValue(PluginConst.SETTING_KEY_DEBUG_FLAG) == "1")
				{
					m_debugwriter.write(writeStr);
				}
			}
			catch
			{
			}
		}

		public void WriteErrorCSV(DataTable dt, string procType)
		{
			m_csv.write(dt, procType);
		}

		public void WriteSendDataSetLog(DataSet ds,string procType)
		{
			m_dswriter.write(ds, procType,"送信");
		}

		public void WriteReceiveDataSetLog(DataSet ds, string procType)
		{
			m_dswriter.write(ds, procType, "受信");
		}
	}
}
