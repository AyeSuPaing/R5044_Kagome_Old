/*
=========================================================================================================
  Module      : Text Node Setting(TextNodeSetting.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace w2.App.Common.Pdf.Settings.Invoice
{
	/// <summary>
	/// Text Node Setting
	/// </summary>
	internal class TextNodeSetting : SettingBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="element">Element</param>
		public TextNodeSetting(XElement element)
			: base(element)
		{
		}

		/// <summary>
		/// Create left element
		/// </summary>
		/// <returns>Left element</returns>
		public XElement CreateLeftElement(XNamespace defaultNamespace, decimal? with)
		{
			var partsAlign = this.PartsAlignAttributes[InvoiceConstants.CONST_XML_ELEMENT_NAME_LEFT];
			switch (partsAlign)
			{
				case InvoiceConstants.CONST_ALIGN_KBN_CENTER:
					return new XElement(
						defaultNamespace + InvoiceConstants.CONST_XML_ELEMENT_NAME_LEFT,
						string.Format(InvoiceConstants.CONST_FORMAT_STYLE_CENTIMETR, with ?? 0));

				case InvoiceConstants.CONST_ALIGN_KBN_RIGHT:
					return new XElement(
						defaultNamespace + InvoiceConstants.CONST_XML_ELEMENT_NAME_LEFT,
						string.Format(InvoiceConstants.CONST_FORMAT_STYLE_CENTIMETR, InvoiceConstants.CONST_DEFAULT_RIGHT_DISTANCE));

				case InvoiceConstants.CONST_ALIGN_KBN_LEFT:
				default:
					return null;
			}
		}

		/// <summary>
		/// Create text align element
		/// </summary>
		/// <param name="defaultNamespace">Default namespace</param>
		/// <param name="isAddStyle">Is add style element</param>
		/// <returns>Text align element</returns>
		public XElement CreateTextAlignElement(XNamespace defaultNamespace, bool isAddStyle = false)
		{
			if (isAddStyle)
			{
				var styleElement = new XElement(
					defaultNamespace + InvoiceConstants.CONST_XML_ELEMENT_NAME_STYLE,
					new XElement(
						defaultNamespace + InvoiceConstants.CONST_XML_ELEMENT_NAME_TEXTALIGN,
						this.TextAlignAttributes[InvoiceConstants.CONST_XML_ELEMENT_NAME_TEXTALIGN]));
				return styleElement;
			}

			var element = new XElement(
				defaultNamespace + InvoiceConstants.CONST_XML_ELEMENT_NAME_TEXTALIGN,
				this.TextAlignAttributes[InvoiceConstants.CONST_XML_ELEMENT_NAME_TEXTALIGN]);
			return element;
		}

		/// <summary>
		/// Create text color element
		/// </summary>
		/// <param name="defaultNamespace">Default namespace</param>
		/// <returns>Text color element</returns>
		public XElement CreateTextColorElement(XNamespace defaultNamespace)
		{
			var element = new XElement(
				defaultNamespace + InvoiceConstants.CONST_XML_ELEMENT_NAME_COLOR,
				this.TextColorAttributes[InvoiceConstants.CONST_XML_ELEMENT_NAME_COLOR]);
			return element;
		}

		/// <summary>
		/// Create width element
		/// </summary>
		/// <param name="defaultNamespace">Default namespace</param>
		/// <returns>Width element</returns>
		public XElement CreateWidthElement(XNamespace defaultNamespace)
		{
			if (this.Width.HasValue == false) return null;

			var element = new XElement(
				defaultNamespace + InvoiceConstants.CONST_XML_ELEMENT_NAME_WIDTH,
				string.Format(InvoiceConstants.CONST_FORMAT_STYLE_CENTIMETR, this.Width));
			return element;
		}

		/// <summary>
		/// Create padding style element
		/// </summary>
		/// <param name="defaultNamespace">Default namespace</param>
		/// <returns>Padding stype element</returns>
		public XElement CreatePaddingStyleElement(XNamespace defaultNamespace)
		{
			var element = new XElement(
				defaultNamespace + InvoiceConstants.CONST_XML_ELEMENT_NAME_STYLE,
				CreatePaddingLeftElement(defaultNamespace),
				CreatePaddingRightElement(defaultNamespace),
				CreatePaddingTopElement(defaultNamespace),
				CreatePaddingBottomElement(defaultNamespace));
			return element;
		}

		/// <summary>
		/// Create padding left element
		/// </summary>
		/// <param name="defaultNamespace">Default namespace</param>
		/// <returns>Padding left element</returns>
		public XElement CreatePaddingLeftElement(XNamespace defaultNamespace)
		{
			var element = new XElement(
				defaultNamespace + InvoiceConstants.CONST_XML_ELEMENT_NAME_PADDINGLEFT,
				this.MarginAttributes[InvoiceConstants.CONST_XML_ELEMENT_NAME_PADDINGLEFT]);
			return element;
		}

		/// <summary>
		/// Create padding right element
		/// </summary>
		/// <param name="defaultNamespace">Default namespace</param>
		/// <returns>Padding right element</returns>
		public XElement CreatePaddingRightElement(XNamespace defaultNamespace)
		{
			var element = new XElement(
				defaultNamespace + InvoiceConstants.CONST_XML_ELEMENT_NAME_PADDINGRIGHT,
				this.MarginAttributes[InvoiceConstants.CONST_XML_ELEMENT_NAME_PADDINGRIGHT]);
			return element;
		}

		/// <summary>
		/// Create padding top element
		/// </summary>
		/// <param name="defaultNamespace">Default namespace</param>
		/// <returns>Padding top element</returns>
		public XElement CreatePaddingTopElement(XNamespace defaultNamespace)
		{
			var element = new XElement(
				defaultNamespace + InvoiceConstants.CONST_XML_ELEMENT_NAME_PADDINGTOP,
				this.MarginAttributes[InvoiceConstants.CONST_XML_ELEMENT_NAME_PADDINGTOP]);
			return element;
		}

		/// <summary>
		/// Create padding bottom element
		/// </summary>
		/// <param name="defaultNamespace">Default namespace</param>
		/// <returns>Padding bottom element</returns>
		public XElement CreatePaddingBottomElement(XNamespace defaultNamespace)
		{
			var element = new XElement(
				defaultNamespace + InvoiceConstants.CONST_XML_ELEMENT_NAME_PADDINGBOTTOM,
				this.MarginAttributes[InvoiceConstants.CONST_XML_ELEMENT_NAME_PADDINGBOTTOM]);
			return element;
		}

		/// <summary>
		/// Initialize properties
		/// </summary>
		protected override void InitializeProperties()
		{
			this.Value = string.Empty;
			this.Width = null;
			this.PartsAlignAttributes = new Dictionary<string, string>();
			this.TextAlignAttributes = new Dictionary<string, string>();
			this.TextColorAttributes = new Dictionary<string, string>();
			this.MarginAttributes = new Dictionary<string, string>();
		}

		/// <summary>
		/// Set properties
		/// </summary>
		/// <param name="element">Element</param>
		protected override void SetProperties(XElement element)
		{
			// Get text settings
			this.Value = element.Value.Trim();

			// Get width settings
			var numberString = GetAttributeValue(element, InvoiceConstants.CONST_XML_ATTRIBUTE_NAME_WIDTH);
			decimal number;
			if (decimal.TryParse(numberString, out number) && (number > 0m))
			{
				this.Width = number;
			}

			// Get parts align settings
			var partsAlign = GetAttributeValue(element, InvoiceConstants.CONST_XML_ATTRIBUTE_NAME_PARTSALIGN);
			this.PartsAlignAttributes.Add(
				InvoiceConstants.CONST_XML_ELEMENT_NAME_LEFT,
				MapToRdlcReportTextAlignValue(partsAlign));

			// Get text align settings
			var textAlign = GetAttributeValue(element, InvoiceConstants.CONST_XML_ATTRIBUTE_NAME_TEXTALIGN);
			this.TextAlignAttributes.Add(
				InvoiceConstants.CONST_XML_ELEMENT_NAME_TEXTALIGN,
				MapToRdlcReportTextAlignValue(textAlign));

			// Get margin left settings
			var marginLeft = GetAttributeValue(element, InvoiceConstants.CONST_XML_ATTRIBUTE_NAME_MARGINLEFT);
			this.MarginAttributes.Add(
				InvoiceConstants.CONST_XML_ELEMENT_NAME_PADDINGLEFT,
				(string.IsNullOrEmpty(marginLeft) == false)
					? marginLeft
					: InvoiceConstants.CONST_NO_PADDING_VALUE);

			// Get margin right settings
			var marginRight = GetAttributeValue(element, InvoiceConstants.CONST_XML_ATTRIBUTE_NAME_MARGINRIGHT);
			this.MarginAttributes.Add(
				InvoiceConstants.CONST_XML_ELEMENT_NAME_PADDINGRIGHT,
				(string.IsNullOrEmpty(marginRight) == false)
					? marginRight
					: InvoiceConstants.CONST_NO_PADDING_VALUE);

			// Get margin top settings
			var marginTop = GetAttributeValue(element, InvoiceConstants.CONST_XML_ATTRIBUTE_NAME_MARGINTOP);
			this.MarginAttributes.Add(
				InvoiceConstants.CONST_XML_ELEMENT_NAME_PADDINGTOP,
				(string.IsNullOrEmpty(marginTop) == false)
					? marginTop
					: InvoiceConstants.CONST_NO_PADDING_VALUE);

			// Get margin bottom settings
			var marginBottom = GetAttributeValue(element, InvoiceConstants.CONST_XML_ATTRIBUTE_NAME_MARGINBOTTOM);
			this.MarginAttributes.Add(
				InvoiceConstants.CONST_XML_ELEMENT_NAME_PADDINGBOTTOM,
				(string.IsNullOrEmpty(marginBottom) == false)
					? marginBottom
					: InvoiceConstants.CONST_NO_PADDING_VALUE);

			// 色の設定取得
			var textColor = Regex.Match(
				GetAttributeValue(element, InvoiceConstants.CONST_XML_ATTRIBUTE_NAME_TEXTCOLOR),
				InvoiceConstants.CONST_REGEXP_GET_COLOR_CODE).Value;
			this.TextColorAttributes.Add(
				InvoiceConstants.CONST_XML_ELEMENT_NAME_COLOR,
				textColor);

			// 次回配送日のポジションを定数に設定する。
			if(this.Value.Contains(InvoiceConstants.CONST_FIELD_VALUE_NEXT_SHIPPING_DATE))InvoiceConstants.CONST_NEXT_SHIPPING_DATE_PARTSALIGN = this.PartsAlignAttributes[InvoiceConstants.CONST_XML_ELEMENT_NAME_LEFT];
		}

		/// <summary>
		/// Map to RDLC TextAlign value
		/// </summary>
		/// <param name="value">Value</param>
		/// <returns>RDLC TextAlign value</returns>
		private string MapToRdlcReportTextAlignValue(string value)
		{
			switch (value)
			{
				case InvoiceConstants.CONST_XML_ATTRIBUTE_VALUE_LEFT:
					return InvoiceConstants.CONST_ALIGN_KBN_LEFT;

				case InvoiceConstants.CONST_XML_ATTRIBUTE_VALUE_CENTER:
					return InvoiceConstants.CONST_ALIGN_KBN_CENTER;

				case InvoiceConstants.CONST_XML_ATTRIBUTE_VALUE_RIGHT:
					return InvoiceConstants.CONST_ALIGN_KBN_RIGHT;

				default:
					return InvoiceConstants.CONST_ALIGN_KBN_LEFT;
			}
		}

		/// <summary>Value</summary>
		public string Value { get; set; }
		/// <summary>Width</summary>
		public decimal? Width { get; set; }
		/// <summary>Parts align attributes</summary>
		public Dictionary<string, string> PartsAlignAttributes { get; set; }
		/// <summary>Text align attributes</summary>
		public Dictionary<string, string> TextAlignAttributes { get; set; }
		/// <summary>Margin attributes</summary>
		public Dictionary<string, string> MarginAttributes { get; set; }
		/// <summary>Text color attributes</summary>
		public Dictionary<string, string> TextColorAttributes { get; set; }
	}
}
