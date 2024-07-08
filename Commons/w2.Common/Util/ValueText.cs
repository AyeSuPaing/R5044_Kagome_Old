/*
=========================================================================================================
  Module      : 値テキストモジュール(ValueText.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Linq;
using System.Xml.Linq;
using w2.Common.Extensions;

namespace w2.Common.Util
{
	///**************************************************************************************
	/// <summary>
	/// 値と表示文字を管理するバリューテキストXMLからデータを取得する
	/// </summary>
	///**************************************************************************************
	public class ValueText
	{
		/*
		 設定ファイル記述例：
		 
		<?xml version="1.0" encoding="utf-8" ?> 
		<ValueText>
			<w2_User>
				<sex>
					<Value text="" value=""></Value>
					<Value text="男" value="1"></Value>
					<Value text="女" value="2"></Value>
					<Value text="不明" value="9"></Value>
				</sex>
			</w2_User>
			...
		</ValueText>
		*/

		const string FIELD_VALUE = "Value";
		const string FIELD_VALUE_ATTRIBUTE_VALUE = "value";
		const string FIELD_VALUE_ATTRIBUTE_TEXT = "text";
		const string FIELD_VALUE_ATTRIBUTE_DISABLE = "disable";

		/// <summary>スレッドセーフ保つためのロックオブジェクト</summary>
		private readonly static object m_lockObject = new object();
		/// <summary>ValueText設定XML</summary>
		private static XmlDocument m_xmlDocCache = null;

		/// <summary>
		/// スタティックコンストラクタ
		/// </summary>
		static ValueText()
		{
			// ValueTextパス設定
			if (string.IsNullOrEmpty(Constants.PHYSICALFILEPATH_VALUETEXT))
			{
				Constants.PHYSICALFILEPATH_VALUETEXT = AppDomain.CurrentDomain.BaseDirectory + Constants.FILEPATH_XML_VALUE_TEXT.Replace("/", @"\");
			}

			// 監視セット
			FileUpdateObserver.GetInstance().AddObservation(
				Path.GetDirectoryName(Constants.PHYSICALFILEPATH_VALUETEXT),
				Path.GetFileName(Constants.PHYSICALFILEPATH_VALUETEXT),
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
		/// <param name="strTableName">テーブル名</param>
		/// <param name="strFieldName">フィールド名</param>
		/// <returns>存在状態</returns>
		public static bool Exists(string strTableName, string strFieldName)
		{
			//------------------------------------------------------
			// ファイル存在チェック
			//------------------------------------------------------
			if (File.Exists(Constants.PHYSICALFILEPATH_VALUETEXT) == false)
			{
				return false;
			}

			//------------------------------------------------------
			// 要素存在チェック
			//------------------------------------------------------
			return (XmlData.SelectNodes("ValueText/" + strTableName + "/" + strFieldName).Count != 0);
		}

		/// <summary>
		/// フィールド値の表示文字列取得
		/// </summary>
		/// <param name="strTableName">テーブル名</param>
		/// <param name="strFieldName">フィールド名</param>
		/// <param name="objValue">値</param>
		/// <returns>値の名称</returns>
		public static string GetValueText(string strTableName, string strFieldName, object objValue)
		{
			return GetValueText(strTableName, strFieldName, StringUtility.ToEmpty(objValue));
		}

		/// <summary>
		/// フィールド値の表示文字列取得
		/// </summary>
		/// <param name="tableName">テーブル名</param>
		/// <param name="fieldName">フィールド名</param>
		/// <param name="value">値</param>
		/// <returns>値の名称</returns>
		public static string GetValueText(string tableName, string fieldName, string value)
		{
			var kvps = GetValueKvpArray(tableName, fieldName);
			foreach (var kvp in kvps)
			{
				if (kvp.Key == value) return kvp.Value;
			}
			return "";
		}

		/// <summary>
		/// フィールド値の表示文字列リストアイテムコレクション取得
		/// </summary>
		/// <param name="tableName">テーブル名</param>
		/// <param name="fieldName">フィールド名</param>
		/// <returns>対象フィールドのリスト</returns>
		/// <remarks>
		/// DropDownListのDataSourceへ設定した際、属性にに「DataValueField="Value" DataTextField="Text"」
		/// を加えておく必要がある。
		/// </remarks>
		public static ListItemCollection GetValueItemList(string tableName, string fieldName)
		{
			var licValueItemList = new ListItemCollection();
			licValueItemList.AddRange(GetValueItemArray(tableName, fieldName));
			return licValueItemList;
		}

		/// <summary>
		/// フィールド値の表示文字列リストアイテム配列取得
		/// </summary>
		/// <param name="tableName">テーブル名</param>
		/// <param name="fieldName">フィールド名</param>
		/// <returns>対象フィールドの配列</returns>
		/// <remarks>
		/// DropDownListのDataSourceへ設定した際、属性にに「DataValueField="Value" DataTextField="Text"」
		/// を加えておく必要がある。
		/// </remarks>
		public static ListItem[] GetValueItemArray(string tableName, string fieldName)
		{
			var result = GetValueKvpArray(tableName, fieldName).Select(
				kvp => new ListItem(kvp.Value, kvp.Key)).ToArray();
			return result;
		}

		/// <summary>
		/// フィールド値の表示文字列KeyValuePair配列取得(MVC向け)
		/// </summary>
		/// <param name="tableName">テーブル名</param>
		/// <param name="fieldName">フィールド名</param>
		/// <returns>KeyValuePair配列</returns>
		public static KeyValuePair<string, string>[] GetValueKvpArray(string tableName, string fieldName)
		{
			try
			{
				var array = XmlData.SelectSingleNode("ValueText/" + tableName + "/" + fieldName).ChildNodes
					.Cast<XmlNode>().Where(fv => fv.Name == FIELD_VALUE).Select(
						fv => new KeyValuePair<string, string>(
							fv.Attributes[FIELD_VALUE_ATTRIBUTE_VALUE].Value,
							fv.Attributes[FIELD_VALUE_ATTRIBUTE_TEXT].Value)).ToArray();
				return array;
			}
			catch (Exception ex)
			{
				throw new w2Exception("ValueText:" + tableName + "->" + fieldName + "で例外が発生しました。", ex);
			}
		}

		/// <summary>
		/// ValueText.xmlを読み込む
		/// </summary>
		/// <returns>ValueText（disable削除後）</returns>
		private static XmlDocument LoadValueText()
		{
			XDocument xdoc = XDocument.Load(Constants.PHYSICALFILEPATH_VALUETEXT);

			// disable属性が付与されているものを削除する
			var xeResult = from Value in xdoc.Descendants(FIELD_VALUE)
						   where Value.Attribute(FIELD_VALUE_ATTRIBUTE_DISABLE) != null
						   select Value;
			xeResult.Remove();

			// 削除後のオブジェクトをreturn
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
