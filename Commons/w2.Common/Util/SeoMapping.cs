/*
=========================================================================================================
  Module    : SEOマッピングモジュール(SeoMapping.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Xml;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using w2.Common.Extensions;

namespace w2.Common.Util
{
	/// <summary>
	/// 値とSEO文字を管理するSEOマッピングXMLからデータを取得する
	/// </summary>
	public class SeoMapping
	{
		/*
		 設定ファイル記述例：
		 
		<?xml version="1.0" encoding="utf-8" ?> 
		<SeoMapping>
			<color>
				<Setting before="B" after="黒" />
				<Setting before="1" after="グレー" />
				<Setting before="0" after="白" />
				<Setting before="6" after="赤" />
			</color>
			...
		</SeoMapping>
		*/
		/// <summary>設定値</summary>
		private const string SETTING = "Setting";
		/// <summary>前属性</summary>
		private const string SETTING_ATTRIBUTE_BEFORE = "before";
		/// <summary>後属性</summary>
		private const string SETTING_ATTRIBUTE_AFTER = "after";

		/// <summary>スレッドセーフ保つためのロックオブジェクト</summary>
		private readonly static object m_lockObject = new object();
		/// <summary>ValueText設定XML</summary>
		private static XmlDocument m_xmlDocCache = null;

		/// <summary>
		/// スタティックコンストラクタ
		/// </summary>
		static SeoMapping()
		{
			// ValueTextパス設定
			if (string.IsNullOrEmpty(Constants.PHYSICALFILEPATH_SEOMAPPING))
			{
				Constants.PHYSICALFILEPATH_SEOMAPPING = AppDomain.CurrentDomain.BaseDirectory + Constants.FILEPATH_XML_SEO_MAPPING.Replace("/", @"\");
			}

			// 監視セット
			FileUpdateObserver.GetInstance().AddObservation(
				Path.GetDirectoryName(Constants.PHYSICALFILEPATH_SEOMAPPING),
				Path.GetFileName(Constants.PHYSICALFILEPATH_SEOMAPPING),
				UpdateCacheData);
		}

		/// <summary>
		/// キャッシュデータ更新
		/// </summary>
		private static void UpdateCacheData()
		{
			lock (m_lockObject)
			{
				m_xmlDocCache = LoadValueText();
			}
		}

		/// <summary>
		/// 設定存在判定
		/// </summary>
		/// <param name="mappingType">マッピングタイプ</param>
		/// <returns>存在状態</returns>
		public static bool Exists(string mappingType)
		{
			//------------------------------------------------------
			// ファイル存在チェック
			//------------------------------------------------------
			if (File.Exists(Constants.PHYSICALFILEPATH_SEOMAPPING) == false)
			{
				return false;
			}

			//------------------------------------------------------
			// 要素存在チェック
			//------------------------------------------------------
			return (XmlData.SelectNodes("SeoMapping/" + mappingType).Count != 0);
		}

		/// <summary>
		/// フィールド値の表示文字列取得
		/// </summary>
		/// <param name="mappingType">マッピングタイプ</param>
		/// <param name="objValue">値</param>
		/// <returns>値の名称</returns>
		public static string GetValueText(string mappingType, object objValue)
		{
			return GetValueText(mappingType, StringUtility.ToEmpty(objValue));
		}

		/// <summary>
		/// フィールド値の表示文字列取得
		/// </summary>
		/// <param name="mappingType">マッピングタイプ</param>
		/// <param name="value">値</param>
		/// <returns>値の名称</returns>
		public static string GetValueText(string mappingType, string value)
		{
			try
			{
				foreach (XmlNode xnFiledValues in XmlData.SelectSingleNode("SeoMapping/" + mappingType).ChildNodes)
				{
					if ((xnFiledValues.Name == SETTING)
						&& (xnFiledValues.Attributes[SETTING_ATTRIBUTE_BEFORE].Value == value))
					{
						return xnFiledValues.Attributes[SETTING_ATTRIBUTE_AFTER].Value;
					}
				}
			}
			catch (Exception ex)
			{
				throw new w2Exception("SeoMapping:" + mappingType + "で例外が発生しました。", ex);
			}

			return "";
		}

		/// <summary>
		/// フィールド値の表示文字列リストアイテムコレクション取得
		/// </summary>
		/// <param name="mappingType">マッピングタイプ</param>
		/// <returns>対象フィールドのリスト</returns>
		public static ListItemCollection GetValueItemList(string mappingType)
		{
			var licValueItemList = new ListItemCollection();

			try
			{
				// XML読み込み・目的のフィールド定義取得
				foreach (XmlNode xnFiledValues in XmlData.SelectSingleNode("SeoMapping/" + mappingType).ChildNodes)
				{
					if (xnFiledValues.Name == SETTING)
					{
						licValueItemList.Add(
							new ListItem(
								xnFiledValues.Attributes[SETTING_ATTRIBUTE_AFTER].Value,
								xnFiledValues.Attributes[SETTING_ATTRIBUTE_BEFORE].Value));
					}
				}
			}
			catch (Exception ex)
			{
				throw new w2Exception("SeoMapping:" + mappingType + "で例外が発生しました。", ex);
			}

			return licValueItemList;
		}

		/// <summary>
		/// フィールド値の表示文字列リストアイテム配列取得
		/// </summary>
		/// <param name="mappingType">マッピングタイプ</param>
		/// <returns>対象フィールドの配列</returns>
		public static ListItem[] GetValueItemArray(string mappingType)
		{
			var licValueItemList = GetValueItemList(mappingType);
			var myListItemArray = new ListItem[licValueItemList.Count];
			licValueItemList.CopyTo(myListItemArray, 0);
			return myListItemArray;
		}

		/// <summary>
		/// SeoMapping.xmlを読み込む
		/// </summary>
		/// <returns>ValueText（disable削除後）</returns>
		private static XmlDocument LoadValueText()
		{
			var xdoc = XDocument.Load(Constants.PHYSICALFILEPATH_SEOMAPPING);
			return xdoc.ToXmlDocument();
		}

		/// <summary>XMLデータ</summary>
		private static XmlDocument XmlData
		{
			get
			{
				lock (m_lockObject)
				{
					if (m_xmlDocCache == null) UpdateCacheData();
					return m_xmlDocCache;
				}
			}
		}
	}
}