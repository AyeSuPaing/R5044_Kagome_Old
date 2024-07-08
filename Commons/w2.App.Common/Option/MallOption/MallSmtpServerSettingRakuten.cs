/*
=========================================================================================================
  Module      : RakutenSMTPサーバ設定処理(MallSmtpServerSettingRakuten)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
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
	/// 楽天SMTPサーバ設定クラス
	/// </summary>
	///*********************************************************************************************
	public class MallSmtpServerSettingRakuten
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="strXml"></param>
		public MallSmtpServerSettingRakuten(string strSettingXml)
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

			foreach (XmlNode xnSetting in xdSettingXml.SelectNodes("SmtpServerSetting/Setting"))
			{
				if (xnSetting.Attributes["type"].Value == "rakuten")
				{
					XmlNode xnTmp = null;

					xnTmp = xnSetting.SelectSingleNode("SmtpServerName");
					this.SmtpServerName = (xnTmp != null) ? xnTmp.InnerText : null;

					xnTmp = xnSetting.SelectSingleNode("SmtpServerPort");
					this.SmtpServerPort = (xnTmp != null) ? xnTmp.InnerText : null;

					xnTmp = xnSetting.SelectSingleNode("SmtpAuthId");
					this.SmtpAuthId = (xnTmp != null) ? xnTmp.InnerText : null;

					xnTmp = xnSetting.SelectSingleNode("SmtpAuthPassword");
					this.SmtpAuthPassword = (xnTmp != null) ? xnTmp.InnerText : null;

					xnTmp = xnSetting.SelectSingleNode("RakutenStoreMailAddress");
					this.RakutenStoreMailAddress = (xnTmp != null) ? xnTmp.InnerText : null;
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
		public MallSmtpServerSettingRakuten(
			string strSmtpServerName,
			string strSmtpServerPort,
			string strSmtpAuthId,
			string strSmtpAuthPassword,
			string strRakutenStoreMailAddress
			)
		{
			this.SmtpServerName = strSmtpServerName;
			this.SmtpServerPort = strSmtpServerPort;
			this.SmtpAuthId = strSmtpAuthId;
			this.SmtpAuthPassword = strSmtpAuthPassword;
			this.RakutenStoreMailAddress = strRakutenStoreMailAddress;
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
				xtw.WriteStartElement("SmtpServerSetting");

				xtw.WriteStartElement("Setting");
				xtw.WriteAttributeString("type", "rakuten");


				xtw.WriteStartElement("SmtpServerName");
				xtw.WriteString(this.SmtpServerName);
				xtw.WriteEndElement();

				xtw.WriteStartElement("SmtpServerPort");
				xtw.WriteString(this.SmtpServerPort.ToString());
				xtw.WriteEndElement();

				xtw.WriteStartElement("SmtpAuthId");
				xtw.WriteString(this.SmtpAuthId);
				xtw.WriteEndElement();

				xtw.WriteStartElement("SmtpAuthPassword");
				xtw.WriteString(this.SmtpAuthPassword);
				xtw.WriteEndElement();

				xtw.WriteStartElement("RakutenStoreMailAddress");
				xtw.WriteString(this.RakutenStoreMailAddress);
				xtw.WriteEndElement();

				xtw.WriteEndElement();	// Setting

				xtw.WriteEndElement();	// SmtpServerSetting

				strResult = sw.ToString();
			}

			return strResult;
		}

		/// <summary>SMTPサーバ名</summary>
		public string SmtpServerName { get; set; }
		/// <summary>SMTPサーバポート番号</summary>
		public string SmtpServerPort { get; set; }
		/// <summary>SMTP-AUTH ID</summary>
		public string SmtpAuthId { get; set; }
		/// <summary>SMTP-AUTH パスワード</summary>
		public string SmtpAuthPassword { get; set; }
		/// <summary>楽天店舗登録先メールアドレス</summary>
		public string RakutenStoreMailAddress { get; set; }
	}
}
