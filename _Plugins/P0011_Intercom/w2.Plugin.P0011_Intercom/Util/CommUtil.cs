/*
=========================================================================================================
  Module      : 共通用ユーティリティクラス(CommUtil.cs)
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

namespace w2.Plugin.P0011_Intercom.Util
{
	class CommUtil
	{
		private static CooperationWebService m_linkutil = null;
		private static LogFileUtil m_fileutil = null;

		private static object m_configlockobj = new object();
		private static object m_dbutillockobj = new object();

		private static CacheDependency m_configDependency = null;
		private static CacheDependency m_dubtilDependency = null;

		private static string m_dbutilfilepath = null;
		private static string m_configfilepath = null;

		//private static Assembly m_asm = null;

		/// <summary>
		/// 静的コンストラクタ
		/// </summary>
		static CommUtil()
		{
			//codebase
			string codebase = Assembly.GetExecutingAssembly().CodeBase;
			Uri dllFilepath = new Uri(codebase);
			string dir = Path.GetDirectoryName(dllFilepath.LocalPath) + @"\xml";
			
			//SQLXMLファイル用のDependency
			m_dbutilfilepath = dir + @"\" + PluginConst.SQLXML_FILENAME;
			m_dubtilDependency = new CacheDependency(m_dbutilfilepath);

			//SETTINGXMLファイル用のDependency			
			m_configfilepath = dir + @"\" + PluginConst.SETTINGXM_FILENAME;
			m_configDependency = new CacheDependency(m_configfilepath);

			m_linkutil = new CooperationWebService();
			m_fileutil = new LogFileUtil();
			
		}

		/// <summary>
		/// キャッシュしているDBUtilインスタンス返却
		/// キャッシュがない場合はインスタンス生成、保持
		/// </summary>
		/// <returns></returns>
		public static DBUtil _DBUtil()
		{
			Cache cache = HttpContext.Current.Cache;

			lock (m_dbutillockobj)
			{

				DBUtil result = (DBUtil)cache[PluginConst.CACHE_KEY_DBUTIL];

				if ((result == null) || (m_dubtilDependency.HasChanged))
				// cacheに格納
				{
					m_dubtilDependency = new CacheDependency(m_dbutilfilepath);

					string codebase = Assembly.GetExecutingAssembly().CodeBase;
					Uri dllFilepath = new Uri(codebase);
					string filepath = Path.GetDirectoryName(dllFilepath.LocalPath) + @"\xml\" + PluginConst.SQLXML_FILENAME;

					result = new DBUtil(WebConfigurationManager.ConnectionStrings[PluginConst.CONNECTION_KEY].ToString()
						,filepath);

					cache.Add(PluginConst.CACHE_KEY_DBUTIL, 
						result,
						m_dubtilDependency, 
						Cache.NoAbsoluteExpiration, 
						new TimeSpan(12, 0, 0), 
						CacheItemPriority.High, null);

				}
				return result;
			}
		}

		/// <summary>
		/// キャッシュしているLinkUtilインスタンス返却
		/// </summary>
		/// <returns></returns>
		public static CooperationWebService _LinkUtil()
		{
			return m_linkutil;
		}

		public static LogFileUtil _FileUtil()
		{
			return m_fileutil;
		}

		/// <summary>
		/// キャッシュしているConfigUtilインスタンス返却
		/// キャッシュがない場合はインスタンス生成、保持
		/// </summary>
		/// <returns></returns>
		public static ConfigUtil _ConfigUtil()
		{
			
			lock (m_configlockobj)
			{
				Cache cache = HttpContext.Current.Cache;

				ConfigUtil result = (ConfigUtil)cache[PluginConst.CACHE_KEY_CONFIGUTIL];

				if ((result == null) || (m_configDependency.HasChanged))
				// cacheに格納
				{
					m_configDependency = new CacheDependency(m_configfilepath);

					string codebase = Assembly.GetExecutingAssembly().CodeBase;
					Uri dllFilepath = new Uri(codebase);
					string filepath = Path.GetDirectoryName(dllFilepath.LocalPath) + @"\xml\" + PluginConst.SETTINGXM_FILENAME;

					result = new ConfigUtil(filepath);

					cache.Add(PluginConst.CACHE_KEY_CONFIGUTIL, 
						result, m_configDependency, 
						Cache.NoAbsoluteExpiration, 
						new TimeSpan(12, 0, 0), 
						CacheItemPriority.High, null);

				}
				return result;
			}
		}
	}
}
