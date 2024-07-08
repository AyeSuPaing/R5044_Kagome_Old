/*
=========================================================================================================
  Module      : デバッグログ用クラス(DebugLogWriter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2011 All Rights Reserved.
  MEMO        : デバッグログへの処理を司るクラス。
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.Plugin.P0011_Intercom.Util
{
	class DebugLogWriter : FileWriterBase
	{
		private string m_writeFilePath = "";

		internal DebugLogWriter(string filePath)
		{
			m_writeFilePath = filePath;
		}

		internal void write(string writeStr)
		{
			base.WriteFile(m_writeFilePath, writeStr);
		}

	}
}
