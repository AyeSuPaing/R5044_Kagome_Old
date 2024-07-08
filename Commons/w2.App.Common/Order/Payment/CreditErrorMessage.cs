/*
=========================================================================================================
  Module      : クレジットカードエラーメッセージモジュール(CreditErrorMessage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using w2.Common.Extensions;
using w2.Common.Util;

namespace w2.App.Common.Order.Payment
{
	/// <summary>
	/// クレジットカードエラー詳細取得クラス
	/// </summary>
	public class CreditErrorMessage
	{
		/*
		設定ファイル記述例：
		<?xml version="1.0" encoding="utf-8" ?>
		<GMOCreditErrorMessages>
		<Message code="G12"><![CDATA[42G020000:カード会社オーソリエラー/カード残高が不足しているために、決済を完了する事が出来ませんでした。]]></Value>
		</GMOCreditErrorMessages>
		*/

		/// <summary>エラーメッセージ格納タグ</summary>
		const string FIELD_MESSAGE = "Message";
		/// <summary>エラーコード属性</summary>
		const string FIELD_MESSAGE_ATTRIBUTE_CODE = "code";
		/// <summary>スレッドセーフ保つためのロックオブジェクト</summary>
		private readonly static object m_lockObject = new object();
		/// <summary>CreditErrorMessages設定XML</summary>
		private static XmlDocument m_xmlDocCache = null;
		/// <summary>CreditErrorMessagesファイル名</summary>
		private static string m_paymentFileName = "";

		/// <summary>
		/// CreditErrorMessagesファイル設定
		/// </summary>
		/// <param name="fileName">クレジットカードエラーXMLファイル名</param>
		public void SetCreditErrorMessages(string fileName)
		{
			var filePath = "";
			m_paymentFileName = fileName;
			switch (fileName)
			{
				case Constants.FILE_XML_ZEUS_CREDIT_ERROR_MESSAGE:
					filePath = Constants.FILEPATH_XML_ZEUS_CREDIT_ERROR_MESSAGE;
					break;

				case Constants.FILE_XML_SBPS_CREDIT_ERROR_MESSAGE:
					filePath = Constants.FILEPATH_XML_SBPS_CREDIT_ERROR_MESSAGE;
					break;

				case Constants.FILE_XML_GMO_CREDIT_ERROR_MESSAGE:
					filePath = Constants.FILEPATH_XML_GMO_CREDIT_ERROR_MESSAGE;
					break;

				case Constants.FILE_XML_YAMATOKWC_CREDIT_ERROR_MESSAGE:
					filePath = Constants.FILEPATH_XML_YAMATOKWC_CREDIT_ERROR_MESSAGE;
					break;
				
				case Constants.FILE_XML_ESCOTT_CREDIT_ERROR_MESSAGE:
					filePath = Constants.FILEPATH_XML_ESCOTT_CREDIT_ERROR_MESSAGE;
					break;

				case Constants.FILE_XML_VERITRANS_CREDIT_ERROR_MESSAGE:
					filePath = Constants.FILEPATH_XML_VERITRANS_CREDIT_ERROR_MESSAGE;
					break;

				case Constants.FILE_XML_RAKUTEN_CREDIT_ERROR_MESSAGE:
					filePath = Constants.FILEPATH_XML_RAKUTEN_CREDIT_ERROR_MESSAGE;
					break;

				case Constants.FILE_XML_PAYGENT_CREDIT_ERROR_MESSAGE:
					filePath = Constants.FILEPATH_XML_PAYGENT_CREDIT_ERROR_MESSAGE;
					break;
			}

			// CreditErrorMessagesパス設定
			Constants.PHYSICALFILEPATH_CREDITERRORMESSAGE = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);

			// 監視セット
			FileUpdateObserver.GetInstance().AddObservation(
				Path.GetDirectoryName(Constants.PHYSICALFILEPATH_CREDITERRORMESSAGE),
				Path.GetFileName(Constants.PHYSICALFILEPATH_CREDITERRORMESSAGE),
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
		/// リストアイテム配列取得
		/// </summary>
		/// <returns>対象クレジットのエラーコード・メッセージの配列</returns>
		public ListItem[] GetValueItemArray()
		{
			var result = GetValueKvpArray().Select(
				kvp => new ListItem(kvp.Value, kvp.Key)).ToArray();
			return result;
		}

		/// <summary>
		/// フィールド値の表示文字列KeyValuePair配列取得
		/// </summary>
		/// <returns>KeyValuePair配列</returns>
		public static KeyValuePair<string, string>[] GetValueKvpArray()
		{
			try
			{
				var array = XmlData.SelectSingleNode(string.Format("{0}", m_paymentFileName)).ChildNodes
					.Cast<XmlNode>().Where(fv => fv.Name == FIELD_MESSAGE).Select(
						fv => new KeyValuePair<string, string>(
							fv.Attributes[FIELD_MESSAGE_ATTRIBUTE_CODE].Value,
							fv.InnerText)).ToArray();
				return array;
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("{0}で例外が発生しました。", m_paymentFileName), ex);
			}
		}

		/// <summary>
		/// CreditErrorMessages.xmlを読み込む
		/// </summary>
		/// <returns>CreditErrorMessages</returns>
		private static XmlDocument LoadValueText()
		{
			XDocument xdoc = XDocument.Load(Constants.PHYSICALFILEPATH_CREDITERRORMESSAGE);
			return xdoc.ToXmlDocument();
		}

		/// <summary>XMLデータ</summary>
		private static XmlDocument XmlData
		{
			get
			{
				lock (m_lockObject)
				{
					if ((m_xmlDocCache == null)|| (m_xmlDocCache.DocumentElement.Name != m_paymentFileName)) UpdateCacheData();
					return m_xmlDocCache;
				}
			}
		}
	}
}

