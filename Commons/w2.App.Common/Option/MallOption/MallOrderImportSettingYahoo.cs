/*
=========================================================================================================
  Module      : Yahoo受注取込処理(MallOrderImportSettingYahoo)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace w2.App.Common.Option
{
	///*********************************************************************************************
	/// <summary>
	/// Yahooモール注文取込設定クラス
	/// </summary>
	///*********************************************************************************************
	public class MallOrderImportSettingYahoo
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="strXml"></param>
		public MallOrderImportSettingYahoo(string strSettingXml)
		{
			XmlDocument xdSettingXml = new XmlDocument();
			try
			{
				xdSettingXml.LoadXml(strSettingXml);
			}
			catch (Exception)
			{
				return;
			}

			foreach (XmlNode xnSetting in xdSettingXml.SelectNodes("OrderImportSetting/Setting"))
			{
				if (xnSetting.Attributes["type"].Value == "yahoo")
				{
					XmlNode xnTmp = null;

					xnTmp = xnSetting.SelectSingleNode("YahooStoreAccountName");
					this.YahooStoreAccountName = (xnTmp != null) ? xnTmp.InnerText : null;

					xnTmp = xnSetting.SelectSingleNode("YahooStoreLoginId");
					this.YahooStoreLoginId = (xnTmp != null) ? xnTmp.InnerText : null;

					xnTmp = xnSetting.SelectSingleNode("YahooStoreLoginPassword");
					this.YahooStoreLoginPassword = (xnTmp != null) ? xnTmp.InnerText : null;

					xnTmp = xnSetting.SelectSingleNode("YahooBusinessManagerLoginId");
					this.YahooBusinessManagerLoginId = (xnTmp != null) ? xnTmp.InnerText : null;

					xnTmp = xnSetting.SelectSingleNode("YahooBusinessManagerLoginPassword");
					this.YahooBusinessManagerLoginPassword = (xnTmp != null) ? xnTmp.InnerText : null;

					xnTmp = xnSetting.SelectSingleNode("YahooOrderCsvLinkName");
					this.YahooOrderCsvLinkName = (xnTmp != null) ? xnTmp.InnerText : null;

					xnTmp = xnSetting.SelectSingleNode("YahooCustomFieldCsvLinkName");
					this.YahooCustomFieldCsvLinkName = (xnTmp != null) ? xnTmp.InnerText : null;
				}
			}

		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="strYahooStoreAccountName"></param>
		/// <param name="strYahooStoreLoginId"></param>
		/// <param name="strYahooStoreLoginPassword"></param>
		/// <param name="strYahooBusinessManagerLoginId"></param>
		/// <param name="strYahooBusinessManagerLoginPassword"></param>
		/// <param name="strYahooOrderCsvLinkName"></param>
		/// <param name="strYahooCustomFieldCsvLinkName"></param>
		public MallOrderImportSettingYahoo(
			string strYahooStoreAccountName,
			string strYahooStoreLoginId,
			string strYahooStoreLoginPassword,
			string strYahooBusinessManagerLoginId,
			string strYahooBusinessManagerLoginPassword,
			string strYahooOrderCsvLinkName,
			string strYahooCustomFieldCsvLinkName
			)
		{
			this.YahooStoreAccountName = strYahooStoreAccountName;
			this.YahooStoreLoginId = strYahooStoreLoginId;
			this.YahooStoreLoginPassword = strYahooStoreLoginPassword;
			this.YahooBusinessManagerLoginId = strYahooBusinessManagerLoginId;
			this.YahooBusinessManagerLoginPassword = strYahooBusinessManagerLoginPassword;
			this.YahooOrderCsvLinkName = strYahooOrderCsvLinkName;
			this.YahooCustomFieldCsvLinkName = strYahooCustomFieldCsvLinkName;
		}

		/// <summary>
		/// XML生成
		/// </summary>
		/// <returns></returns>
		public string CreateXml()
		{
			string strResult = null;

			using (StringWriter sw = new StringWriter())
			using (XmlTextWriter xtw = new XmlTextWriter(sw))
			{
				xtw.WriteProcessingInstruction("xml", "version='1.0' encoding='utf-8'");	// 強制UTF-8
				xtw.WriteStartElement("OrderImportSetting");

				xtw.WriteStartElement("Setting");
				xtw.WriteAttributeString("type", "yahoo");


				xtw.WriteStartElement("YahooStoreAccountName");
				xtw.WriteString(this.YahooStoreAccountName);
				xtw.WriteEndElement();

				xtw.WriteStartElement("YahooStoreLoginId");
				xtw.WriteString(this.YahooStoreLoginId);
				xtw.WriteEndElement();

				xtw.WriteStartElement("YahooStoreLoginPassword");
				xtw.WriteString(this.YahooStoreLoginPassword);
				xtw.WriteEndElement();

				xtw.WriteStartElement("YahooBusinessManagerLoginId");
				xtw.WriteString(this.YahooBusinessManagerLoginId);
				xtw.WriteEndElement();

				xtw.WriteStartElement("YahooBusinessManagerLoginPassword");
				xtw.WriteString(this.YahooBusinessManagerLoginPassword);
				xtw.WriteEndElement();

				xtw.WriteStartElement("YahooOrderCsvLinkName");
				xtw.WriteString(this.YahooOrderCsvLinkName);
				xtw.WriteEndElement();

				xtw.WriteStartElement("YahooCustomFieldCsvLinkName");
				xtw.WriteString(this.YahooCustomFieldCsvLinkName);
				xtw.WriteEndElement();

				xtw.WriteEndElement();	// Setting

				xtw.WriteEndElement();	// OrderImportSetting

				strResult = sw.ToString();
			}

			return strResult;
		}

		/// <summary>ヤフーストアアカウント名</summary>
		public string YahooStoreAccountName { get; set; }
		/// <summary>ヤフーストアログインＩＤ</summary>
		public string YahooStoreLoginId { get; set; }
		/// <summary>ヤフーストアログインパスワード</summary>
		public string YahooStoreLoginPassword { get; set; }
		/// <summary>ヤフービジネスマネージャログインＩＤ</summary>
		public string YahooBusinessManagerLoginId { get; set; }
		/// <summary>ヤフフービジネスマネージャログインパスワード</summary>
		public string YahooBusinessManagerLoginPassword { get; set; }
		/// <summary>ヤフー受注CSVリンク名</summary>
		public string YahooOrderCsvLinkName { get; set; }
		/// <summary>ヤフーカスタムフィールドCSVリンク名</summary>
		public string YahooCustomFieldCsvLinkName { get; set; }
	}
}
