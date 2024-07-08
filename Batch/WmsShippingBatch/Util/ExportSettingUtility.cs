/*
=========================================================================================================
  Module      : Export Setting Utility (ExportSettingUtility.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using w2.Common;
using w2.Common.Extensions;
using w2.Common.Util;

namespace w2.Commerce.Batch.WmsShippingBatch.Util
{
	/// <summary>
	/// The export setting Util
	/// </summary>
	public class ExportSettingUtility
	{
		#region Fields
		/// <summary>The XML setting file name node</summary>
		private const string XML_SETTING_FILENAME_NODE = "filename";
		/// <summary>The XML setting fields node</summary>
		private const string XML_SETTING_FIELDS_NODE = "fields";
		/// <summary>The XML field node</summary>
		private const string XML_FIELD_NODE = "Field";
		/// <summary>The XML header attribute</summary>
		private const string XML_HEADER_ATTRIBUTE = "header";
		/// <summary>The XML name attribute</summary>
		private const string XML_NAME_ATTRIBUTE = "name";
		/// <summary>The XML format attribute</summary>
		private const string XML_FORMAT_ATTRIBUTE = "format";
		/// <summary>The XML default value attribute</summary>
		private const string XML_DEFAULT_VALUE = "default_value";
		/// <summary>The XML max length attribute</summary>
		private const string XML_MAX_LENGTH = "max_length";
		/// <summary>The XML pad left attribute</summary>
		private const string XML_PAD_LEFT = "pad_left";
		/// <summary>The XML setting Elogit node</summary>
		private const string XML_SETTING_ELOGIT_NODE = "Elogit";
		/// <summary>The XML setting Payment node</summary>
		private const string XML_SETTING_PAYMENT_NODE = "Payment";
		/// <summary>The XML paymentId attribute</summary>
		private const string XML_KEY_ATTRIBUTE = "paymentId";

		/// <summary>The XML document</summary>
		private static XmlDocument m_document;
		#endregion

		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		static ExportSettingUtility()
		{
			var filePath = Path.Combine(
				AppDomain.CurrentDomain.BaseDirectory,
				@"Xml\\ExportSettings.xml");
			var doc = XDocument.Load(filePath);

			m_document = doc.ToXmlDocument();
		}
		#endregion

		#region Methods
		/// <summary>
		/// Get field settings
		/// </summary>
		/// <returns>The export setting</returns>
		public static ExportSetting GetExportSetting()
		{
			var settingName = "WmsShippingExport";
			var exportSetting = new ExportSetting
			{
				FileNameFormat = string.Empty,
				FieldSettings = new List<ExportFieldSetting>(),
			};

			try
			{
				var settingNodePath = string.Format(
					"ExportSettings/{0}/{1}",
					settingName,
					XML_SETTING_FILENAME_NODE);
				var node = m_document.SelectSingleNode(settingNodePath);
				exportSetting.FileNameFormat = GetAttributeValue(node, XML_FORMAT_ATTRIBUTE);
				var fieldSettingNodePath = string.Format(
					"ExportSettings/{0}/{1}",
					settingName,
					XML_SETTING_FIELDS_NODE);
				var childNodes = m_document.SelectSingleNode(fieldSettingNodePath).ChildNodes;

				// Set value for export field setting
				foreach (XmlNode xmlNode in childNodes)
				{
					if (xmlNode.Name != XML_FIELD_NODE) continue;

					var exportFieldSetting = new ExportFieldSetting
					{
						ExportHeaderName = GetAttributeValue(xmlNode, XML_HEADER_ATTRIBUTE),
						ExportName = GetAttributeValue(xmlNode, XML_NAME_ATTRIBUTE),
						ExportFormat = GetAttributeValue(xmlNode, XML_FORMAT_ATTRIBUTE),
						ExportDefaultValue = GetAttributeValue(xmlNode, XML_DEFAULT_VALUE),
						ExportMaxLength = GetAttributeIntValue(xmlNode, XML_MAX_LENGTH),
						PaddingChar = GetAttributeCharValue(xmlNode, XML_PAD_LEFT),
					};

					exportSetting.FieldSettings.Add(exportFieldSetting);
				}
			}
			catch (Exception ex)
			{
				// Exception
				throw new w2Exception(
					string.Format(
						"ExportSettings: {0}->{1}で例外が発生しました。",
						settingName,
						XML_SETTING_FIELDS_NODE),
					ex);
			}
			return exportSetting;
		}

		/// <summary>
		/// 後払い決済区分IDの取得
		/// </summary>
		/// <returns>後払い決済区分IDのリスト</returns>
		public static List<string> GetDeferredPayment()
		{
			var settingName = "DeferredPayments";
			var deferredPayments=new List<string>();

			try
			{
				var fieldSettingNodePath = string.Format(
					"ExportSettings/{0}/{1}/{2}",
					settingName,
					XML_SETTING_ELOGIT_NODE,
					XML_SETTING_PAYMENT_NODE);
				var nodes = m_document.SelectNodes(fieldSettingNodePath);

				foreach (XmlNode xmlNode in nodes)
				{
					if (xmlNode.Name != XML_SETTING_PAYMENT_NODE) continue;
					var paymentId = GetAttributeValue(xmlNode, XML_KEY_ATTRIBUTE);
					deferredPayments = paymentId.Split(',').ToList();
				}
			}
			catch (Exception ex)
			{
				throw new w2Exception(
					string.Format(
						"ExportSettings: {0}->{1}->{2}で例外が発生しました。",
						settingName,
						XML_SETTING_ELOGIT_NODE,
						XML_SETTING_PAYMENT_NODE),
					ex);
			}

			return deferredPayments;
		}

		/// <summary>
		/// Get attribute value
		/// </summary>
		/// <param name="xmlNode">The XML node</param>
		/// <param name="attribute">The attribute name</param>
		/// <returns>The attribute value</returns>
		private static string GetAttributeValue(XmlNode xmlNode, string attribute)
		{
			var value = (xmlNode.Attributes[attribute] != null)
				? StringUtility.ToEmpty(xmlNode.Attributes[attribute].Value)
				: string.Empty;
			return value;
		}

		/// <summary>
		/// Get attribute char value
		/// </summary>
		/// <param name="xmlNode">The XML node</param>
		/// <param name="attribute">The attribute name</param>
		/// <returns>The attribute value</returns>
		private static char? GetAttributeCharValue(XmlNode xmlNode, string attribute)
		{
			var value = GetAttributeValue(xmlNode, attribute);
			char paddingChar;
			if (char.TryParse(value, out paddingChar) == false) return null;

			return paddingChar;
		}

		/// <summary>
		/// Get attribute int value
		/// </summary>
		/// <param name="xmlNode">The XML node</param>
		/// <param name="attribute">The attribute name</param>
		/// <returns>The attribute value</returns>
		private static int? GetAttributeIntValue(XmlNode xmlNode, string attribute)
		{
			var value = GetAttributeValue(xmlNode, attribute);
			var number = 0;
			var result = int.TryParse(value, out number)
				? number
				: (int?)null;
			return result;
		}
		#endregion
	}

	/// <summary>
	/// The export setting
	/// </summary>
	public class ExportSetting
	{
		/// <summary>An export file name format</summary>
		public string FileNameFormat { get; set; }
		/// <summary>A list of export field settings</summary>
		public List<ExportFieldSetting> FieldSettings { get; set; }
	}

	/// <summary>
	/// The export field setting
	/// </summary>
	public class ExportFieldSetting
	{
		/// <summary>The export header name</summary>
		public string ExportHeaderName { get; set; }
		/// <summary>The export name</summary>
		public string ExportName { get; set; }
		/// <summary>The export format</summary>
		public string ExportFormat { get; set; }
		/// <summary>The export default value</summary>
		public string ExportDefaultValue { get; set; }
		/// <summary>The export max length</summary>
		public int? ExportMaxLength { get; set; }
		/// <summary>The export padding char</summary>
		public char? PaddingChar { get; set; }
	}
}
