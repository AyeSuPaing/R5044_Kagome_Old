/*
=========================================================================================================
  Module      : Text Run Object(TextRunObject.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Linq;
using w2.App.Common.Global.Config;
using w2.App.Common.Util;

namespace w2.App.Common.Pdf.Settings.Invoice
{
	/// <summary>
	/// Text Run Object
	/// </summary>
	internal class TextRunObject
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public TextRunObject()
		{
			this.Text = string.Empty;
			this.IsShopSiteTag = false;
			this.IsUrl = false;
			this.FieldName = string.Empty;
		}

		/// <summary>
		/// Create style element
		/// </summary>
		/// <param name="defaultNamespace">Default namespace</param>
		/// <param name="blockSetting">Block setting</param>
		/// <returns>Style element for TextRun</returns>
		public XElement CreateStyleElement(
			XNamespace defaultNamespace,
			InvoiceBlockSetting blockSetting)
		{
			var newStyleElement = new XElement(
				defaultNamespace + InvoiceConstants.CONST_XML_ELEMENT_NAME_STYLE,
				new XElement(
					defaultNamespace + InvoiceConstants.CONST_XML_ELEMENT_NAME_FONTFAMILY,
					InvoiceConstants.CONST_DEFAULT_FONTFAMILY));

			// For case the elemnent can add font-size style
			switch (blockSetting.Type)
			{
				case InvoiceConstants.BlockType.Title:
					newStyleElement.Add(new XElement(
						defaultNamespace + InvoiceConstants.CONST_XML_ELEMENT_NAME_FONTSIZE,
						InvoiceConstants.CONST_DEFAULT_HEADER_FONTSIZE));
					break;

				case InvoiceConstants.BlockType.Body:
				case InvoiceConstants.BlockType.Detail:
					newStyleElement.Add(new XElement(
						defaultNamespace + InvoiceConstants.CONST_XML_ELEMENT_NAME_FONTSIZE,
						InvoiceConstants.CONST_DEFAULT_BODYHEADER_AND_DETAIL_FONTSIZE));
					break;

				case InvoiceConstants.BlockType.Price:
					newStyleElement.Add(new XElement(
						defaultNamespace + InvoiceConstants.CONST_XML_ELEMENT_NAME_FONTSIZE,
						InvoiceConstants.CONST_DEFAULT_PRICE_FONTSIZE));
					break;

				case InvoiceConstants.BlockType.Footer:
					newStyleElement.Add(new XElement(
						defaultNamespace + InvoiceConstants.CONST_XML_ELEMENT_NAME_FONTSIZE,
						InvoiceConstants.CONST_DEFAULT_FOOTER_FONTSIZE));
					break;
			}

			// For case the elemnent can add color style
			if (this.IsShopSiteTag || this.IsUrl)
			{
				newStyleElement.Add(new XElement(
					defaultNamespace + InvoiceConstants.CONST_XML_ELEMENT_NAME_COLOR,
					InvoiceConstants.CONST_BLUE_COLOR));
			}

			// For case the elemnent can add text-decoration style
			if (this.IsUrl)
			{
				newStyleElement.Add(new XElement(
					defaultNamespace + InvoiceConstants.CONST_XML_ELEMENT_NAME_TEXTDECORATION,
					InvoiceConstants.CONST_TEXTDECORATION_UNDERLINE));
			}

			// For case the elemnent can add font-weight style
			if (this.IsShopSiteTag
				|| this.IsUrl
				|| (blockSetting.Type == InvoiceConstants.BlockType.Title))
			{
				newStyleElement.Add(new XElement(
					defaultNamespace + InvoiceConstants.CONST_XML_ELEMENT_NAME_FONTWEIGHT,
					InvoiceConstants.CONST_FONTWEIGHT_NORMAL));
			}

			// For case the elemnent can add format style
			var formatValue = string.Empty;
			var localeId = string.Empty;
			if (Constants.GLOBAL_OPTION_ENABLE
				&& (string.IsNullOrEmpty(blockSetting.LanguageLocaleId) == false))
			{
				localeId = blockSetting.LanguageLocaleId;
			}
			switch (this.FieldName)
			{
				case Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_DATE:
					formatValue = GlobalConfigUtil.GetDateTimeFormatText(
						localeId,
						DateTimeUtility.FormatType.ShortDateWeekOfDay2Letter);
					break;

				case Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE:
					formatValue = GlobalConfigUtil.GetDateTimeFormatText(
						localeId,
						DateTimeUtility.FormatType.LongDateWeekOfDay2Letter);
					break;
			}
			if (string.IsNullOrEmpty(formatValue) == false)
			{
				newStyleElement.Add(new XElement(
					defaultNamespace + InvoiceConstants.CONST_XML_ELEMENT_NAME_FORMAT,
					formatValue));
			}

			return newStyleElement;
		}

		/// <summary>
		/// Create Label element
		/// </summary>
		/// <param name="defaultNamespace">Default namespace</param>
		/// <returns>Label element</returns>
		public XElement CreateLabelElement(XNamespace defaultNamespace)
		{
			var label = new XElement(
				defaultNamespace + InvoiceConstants.CONST_XML_ELEMENT_NAME_LABEL,
				this.FieldName);
			return label;
		}

		/// <summary>Text</summary>
		public string Text { get; set; }
		/// <summary>Is shop site tag</summary>
		public bool IsShopSiteTag { get; set; }
		/// <summary>Is url</summary>
		public bool IsUrl { get; set; }
		/// <summary>Field name</summary>
		public string FieldName { get; set; }
		/// <summary>Is field</summary>
		public bool IsField
		{
			get { return (string.IsNullOrEmpty(this.FieldName) == false); }
		}
	}
}
