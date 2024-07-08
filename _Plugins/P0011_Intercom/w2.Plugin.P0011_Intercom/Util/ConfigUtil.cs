/*
=========================================================================================================
  Module      : 設定ファイル用ユーティリティクラス(ConfigUtil.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2011 All Rights Reserved.
  MEMO        : プラグイン用の設定ファイルに関する処理をつかさどるクラス。
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace w2.Plugin.P0011_Intercom.Util
{
	class ConfigUtil
	{
		private Dictionary<string, string> m_confData = null;

		internal ConfigUtil(string confXMLPath)
		{
			m_confData = LoadConfigXML(confXMLPath);
		}

		public string GetValue(string key)
		{
			//ヒットしない場合はから文字
			if (m_confData.Keys.Contains(key) == false)
			{
				return "";
			}

			return m_confData[key];
		}
		
		private Dictionary<string, string> LoadConfigXML(string confXMLPath)
		{
			Dictionary<string, string> confDic = new Dictionary<string, string>();

			XElement confXml = null;
			confXml = XElement.Load(confXMLPath);
			XElement confsettings = confXml.Element("PluginSettings");

			IEnumerable<XElement> list1 = confsettings.Elements();

			foreach (XElement el in list1)
			{
				confDic.Add(el.Name.ToString(), el.Value.ToString());
			}

			return confDic; 
		}
	}
}
