/*
=========================================================================================================
  Module    : 都道府県マッピングモジュール(PrefectureMapping.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Xml;
using System.Xml.Linq;
using w2.Common.Extensions;

namespace w2.Common.Util
{
	/// <summary>
	/// 値とコードを管理する都道府県マッピングXMLからデータを取得する
	/// </summary>
	public static class PrefectureMapping
	{
		/*
		 設定ファイル記述例：
		 
		<?xml version="1.0" encoding="utf-8" ?> 
		<PrefectureMapping>
			<prefecture>
				<Setting value="北海道" code="01" />
				<Setting value="青森県" code="02" />
				<Setting value="岩手県" code="03" />
				...
		</PrefectureMapping>
		*/
		/// <summary>設定値</summary>
		private const string SETTING = "Setting";
		/// <summary>バリュー属性</summary>
		private const string SETTING_ATTRIBUTE_VALUE = "value";
		/// <summary>コード属性</summary>
		private const string SETTING_ATTRIBUTE_CODE = "code";

		/// <summary>スレッドセーフ保つためのロックオブジェクト</summary>
		private static readonly object m_lockObject = new object();
		/// <summary>ValueText設定XML</summary>
		private static XmlDocument m_xmlDocCache = null;

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
		/// フィールド値の表示文字列取得
		/// </summary>
		/// <param name="mappingType">マッピングタイプ</param>
		/// <param name="value">値</param>
		/// <returns>値の名称</returns>
		public static string GetSubdivisionCode(string mappingType, string value)
		{
			try
			{
				var xnFiledValueses = XmlData.SelectSingleNode("PrefectureMapping/" + mappingType)?.ChildNodes;
				if (xnFiledValueses != null)
					foreach (XmlNode xnFiledValues in xnFiledValueses)
					{
						if (xnFiledValues.Attributes != null
							&& (xnFiledValues.Name == SETTING)
							&& (xnFiledValues.Attributes[SETTING_ATTRIBUTE_VALUE].Value == value))
						{
							return xnFiledValues.Attributes[SETTING_ATTRIBUTE_CODE].Value;
						}
					}
			}
			catch (Exception ex)
			{
				throw new w2Exception("PrefectureMapping:" + mappingType + "で例外が発生しました。", ex);
			}

			return string.Empty;
		}

		/// <summary>
		/// PrefectureMapping.xmlを読み込む
		/// </summary>
		/// <returns>ValueText（disable削除後）</returns>
		private static XmlDocument LoadValueText()
		{
			XmlDocument xdoc = new XmlDocument();
			xdoc.LoadXml(App.Common.Properties.Resources.PrefectureMapping);
			return xdoc;
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