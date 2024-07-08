/*
=========================================================================================================
  Module      : Default Setting (DefaultSetting.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using w2.Common.Util;
using w2.Domain.DefaultSetting;

namespace w2.App.Common.DefaultSetting
{
	/// <summary>
	/// Default setting
	/// </summary>
	[Serializable]
	public class DefaultSetting
	{
		#region +Constants
		/// <summary>Default setting node</summary>
		private const string DEFAULTSETTING_XML_NODE = "DefaultSetting";
		/// <summary>Field</summary>
		private const string DEFAULTSETTING_XML_FIELD = "Field";
		/// <summary>Field default</summary>
		private const string DEFAULTSETTING_XML_FIELD_DEFAULT = "Default";
		/// <summary>Field comment</summary>
		private const string DEFAULTSETTING_XML_FIELD_COMMENT = "Comment";
		/// <summary>Field display</summary>
		private const string DEFAULTSETTING_XML_FIELD_DISPLAY = "Display";
		/// <summary>Attribute name</summary>
		private const string DEFAULTSETTING_XML_ATTRIBUTE_NAME = "Name";
		/// <summary>Display flag on</summary>
		private const string DEFAULTSETTING_DISPLAY_FLG_ON = "1";
		/// <summary>Display flag off</summary>
		private const string DEFAULTSETTING_DISPLAY_FLG_OFF = "0";
		#endregion

		#region +Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		public DefaultSetting()
		{
			this.DefaultSettingTables = new Dictionary<string, DefaultSettingTable>();
		}
		#endregion

		#region +Method
		/// <summary>
		/// Load default setting
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="classification">Classification</param>
		public void LoadDefaultSetting(string shopId, string classification)
		{
			// Get default setting information
			var defaultSetting = new DefaultSettingService().Get(shopId, classification);

			// XML structure storage
			if (defaultSetting != null)
			{
				// Store data
				var document = new XmlDocument();
				document.LoadXml(StringUtility.ToEmpty(defaultSetting.InitData));

				var childNodes = document.SelectSingleNode(DEFAULTSETTING_XML_NODE).ChildNodes;
				foreach (XmlNode childNode in childNodes)
				{
					if (childNode.NodeType == XmlNodeType.Comment) continue;

					var setting = new DefaultSettingTable(childNode.Name);
					foreach (XmlNode node in childNode.SelectNodes(DEFAULTSETTING_XML_FIELD))
					{
						var fieldName = node.Attributes[DEFAULTSETTING_XML_ATTRIBUTE_NAME].Value;
						var fieldDefault = (node[DEFAULTSETTING_XML_FIELD_DEFAULT] != null)
							? node[DEFAULTSETTING_XML_FIELD_DEFAULT].InnerText
							: (string)null;
						var fieldComment = node[DEFAULTSETTING_XML_FIELD_COMMENT].InnerText;
						var fieldDisplay = (node[DEFAULTSETTING_XML_FIELD_DISPLAY].InnerText == DEFAULTSETTING_DISPLAY_FLG_ON);

						var defaultSettingField = new DefaultSettingField(
							fieldName,
							fieldDefault,
							fieldComment,
							fieldDisplay);

						setting.Fields[node.Attributes[DEFAULTSETTING_XML_ATTRIBUTE_NAME].Value] = defaultSettingField;
					}

					this.DefaultSettingTables[setting.Name] = setting;
				}
			}

			// Set default setting
			var defaultSettingTable = this.DefaultSettingTables.Values
				.FirstOrDefault(item => (item.Name == classification));

			if (defaultSettingTable != null) this.DefaultSettingTable = defaultSettingTable;
		}

		/// <summary>
		/// Update default setting
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="classification">Classification</param>
		/// <param name="operatorName">Operator name</param>
		public void UpdateDefaultSetting(string shopId, string classification, string operatorName)
		{
			var writerString = new StringWriter();
			var writerXmlText = new XmlTextWriter(writerString);

			writerXmlText.WriteProcessingInstruction("xml", "version='1.0' encoding='utf-8'");
			writerXmlText.WriteStartElement(DEFAULTSETTING_XML_NODE);

			foreach (DefaultSettingTable defaultSettingTable in this.DefaultSettingTables.Values)
			{
				// Default setting XML creation
				writerXmlText.WriteStartElement(defaultSettingTable.Name);
				{
					foreach (DefaultSettingField defaultSettingField in defaultSettingTable.Fields.Values)
					{
						writerXmlText.WriteStartElement(DEFAULTSETTING_XML_FIELD);
						{
							writerXmlText.WriteStartAttribute(DEFAULTSETTING_XML_ATTRIBUTE_NAME);
							writerXmlText.WriteString(defaultSettingField.Name);
							writerXmlText.WriteEndAttribute();
						}

						if (defaultSettingField.Default != null)
						{
							writerXmlText.WriteStartElement(DEFAULTSETTING_XML_FIELD_DEFAULT);
							writerXmlText.WriteCData(defaultSettingField.Default);
							writerXmlText.WriteEndElement();
						}

						writerXmlText.WriteStartElement(DEFAULTSETTING_XML_FIELD_COMMENT);
						writerXmlText.WriteCData(defaultSettingField.Comment);
						writerXmlText.WriteEndElement();

						writerXmlText.WriteStartElement(DEFAULTSETTING_XML_FIELD_DISPLAY);
						writerXmlText.WriteString(
							defaultSettingField.Display
								? DEFAULTSETTING_DISPLAY_FLG_ON
								: DEFAULTSETTING_DISPLAY_FLG_OFF);
						writerXmlText.WriteEndElement();

						writerXmlText.WriteEndElement();
					}
				}
				writerXmlText.WriteEndElement();
			}
			writerXmlText.WriteEndElement();
			
			// Create model for default setting
			var model = CreateModel(
				shopId,
				classification,
				writerString,
				operatorName);

			// Insert/Update default setting
			new DefaultSettingService().UpsertDefaultSetting(model);
		}

		/// <summary>
		/// Create default setting model
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="classification">Classification</param>
		/// <param name="initData">Init data</param>
		/// <param name="operatorName">Operator name</param>
		/// <returns>Default setting model</returns>
		private DefaultSettingModel CreateModel(
			string shopId,
			string classification,
			object initData,
			string operatorName)
		{
			var model = new DefaultSettingModel
			{
				ShopId = shopId,
				Classification = classification,
				InitData = StringUtility.ToEmpty(initData),
				LastChanged = operatorName,
			};
			return model;
		}
		#endregion

		#region +Properties
		/// <summary>Default setting tables</summary>
		public Dictionary<string, DefaultSettingTable> DefaultSettingTables { get; set; }
		/// <summary>Default setting table</summary>
		public DefaultSettingTable DefaultSettingTable { get; set; }
		#endregion
	}
}
