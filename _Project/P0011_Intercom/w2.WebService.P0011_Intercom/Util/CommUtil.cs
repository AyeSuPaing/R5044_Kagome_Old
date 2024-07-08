/*
=========================================================================================================
  Module      : ユーティリティ共通クラスクラス(CommUtil.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2011 All Rights Reserved.
  MEMO        : 各種ユーティリティクラスのインスタンス生成・保持・公開を行う。
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Reflection;
using System.IO;
using System.Web.Caching;
using System.Web.Configuration;

namespace w2.Plugin.P0011_Intercom.WebService.Util
{
	class CommUtil
	{
	
		//キャッシュが使えないので変数に保持

		private static object m_dbutillockobj = new object();
		private static DBUtil m_dbutill = null;

		private static object m_logfileutillockobj = new object();
		private static LogWriter m_logfileutill = null;

		/// <summary>
		/// 静的コンストラクタ
		/// </summary>
		static CommUtil()
		{
			m_dbutill = new DBUtil(WebConfigurationManager.ConnectionStrings[WebServiceConst.CONNECTION_KEY].ToString(),
							WebConfigurationManager.AppSettings[WebServiceConst.APP_SETTINGS_KEY_SQL_FILEPATH].ToString());

			m_logfileutill = new LogWriter();
		}

		/// <summary>
		/// キャッシュしているDBUtilインスタンス返却
		/// キャッシュがない場合はインスタンス生成、保持
		/// </summary>
		/// <returns></returns>
		public static DBUtil _DBUtil()
		{
			lock (m_dbutillockobj)
			{
				if (m_dbutill == null)
				{
					m_dbutill =
						new DBUtil(WebConfigurationManager.ConnectionStrings[WebServiceConst.CONNECTION_KEY].ToString(),
							WebConfigurationManager.AppSettings[WebServiceConst.APP_SETTINGS_KEY_SQL_FILEPATH].ToString());

				}
			}

			return m_dbutill;
		}

		/// <summary>
		/// キャッシュしているDBUtilインスタンス返却
		/// キャッシュがない場合はインスタンス生成、保持
		/// </summary>
		/// <returns></returns>
		public static LogWriter _LogFileUtil()
		{
			lock (m_logfileutillockobj)
			{
				if (m_logfileutill == null)
				{
					m_logfileutill = new LogWriter();
				}
			}

			return m_logfileutill;
		}
	}
}
