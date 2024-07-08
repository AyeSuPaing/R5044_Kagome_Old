/*
=========================================================================================================
  Module      : Invoice Block Setting(InvoiceBlockSetting.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using w2.Common.Util;

namespace w2.App.Common.Pdf.Settings.Invoice
{
	/// <summary>
	/// Invoice Block Setting
	/// </summary>
	internal class InvoiceBlockSetting : SettingBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="element">Element</param>
		public InvoiceBlockSetting(XElement element)
			: base(element)
		{
		}

		/// <summary>
		/// Initialize properties
		/// </summary>
		protected override void InitializeProperties()
		{
			this.Type = null;
			this.Texts = new TextNodeSetting[0];
			this.PriceTexts = new PriceTextNodeSetting[0];
			this.LanguageCode = null;
			this.MaxDisplayItems = InvoiceConstants.CONST_DEFAULT_MAX_DISPLAY_ITEMS;
			this.LogoImagePath = GetPhysicalImagePath();
		}

		/// <summary>
		/// Set properties
		/// </summary>
		/// <param name="element">Element</param>
		protected override void SetProperties(XElement element)
		{
			// Get block type setting
			var typeString = GetAttributeValue(element, InvoiceConstants.CONST_XML_ATTRIBUTE_NAME_TYPE);
			InvoiceConstants.BlockType blockType;
			if (Enum.TryParse(typeString, true, out blockType))
			{
				this.Type = blockType;
			}

			// Get language code setting
			var languageLocaleId = GetAttributeValue(element, InvoiceConstants.CONST_XML_ATTRIBUTE_LANGUAGELOCALEID);
			if (string.IsNullOrEmpty(languageLocaleId) == false)
			{
				this.LanguageLocaleId = languageLocaleId;
				this.LanguageCode = languageLocaleId.Split('-')
					.FirstOrDefault();
			}

			// Get text settings
			this.Texts = element.Elements(InvoiceConstants.CONST_XML_ELEMENT_NAME_TEXT)
				.Select(item => new TextNodeSetting(item))
				.ToArray();

			// Get price text settings
			this.PriceTexts = element.Elements(InvoiceConstants.CONST_XML_ELEMENT_NAME_PRICETEXT)
				.Select(item => new PriceTextNodeSetting(item))
				.ToArray();

			// Get max display items setting
			var maxDisplayItemElement = GetFirstElement(element, InvoiceConstants.CONST_XML_ELEMENT_NAME_MAXDISPLAYITEMS);
			if (maxDisplayItemElement != null)
			{
				var numberString = GetAttributeValue(maxDisplayItemElement, InvoiceConstants.CONST_XML_ATTRIBUTE_NAME_VALUE);
				int number;
				if (int.TryParse(numberString, out number) && (number > 0))
				{
					this.MaxDisplayItems = number;
				}
			}

			// Get logo image path setting
			var logoImageElement = GetFirstElement(element, InvoiceConstants.CONST_XML_ELEMENT_NAME_LOGOIMAGE);
			if (logoImageElement != null)
			{
				var path = GetAttributeValue(logoImageElement, InvoiceConstants.CONST_XML_ATTRIBUTE_NAME_PATH);
				if (string.IsNullOrEmpty(path) == false)
				{
					this.LogoImagePath = GetPhysicalImagePath(path);
				}
			}
		}

		/// <summary>
		/// Get physical image path
		/// </summary>
		/// <param name="path">Path</param>
		/// <returns>physical image path</returns>
		private string GetPhysicalImagePath(string path = Constants.IMG_FRONT_LOGO_INVOICE_DEFAULT)
		{
			if (string.IsNullOrEmpty(path)) return string.Empty;

			// For the case where the path has a replacement tag,
			// the execution of the substitution installs the tag substitution
			if (path.Contains(InvoiceConstants.CONST_ROOT_PATH_TAG))
			{
				return StringUtility.ConvertSlachToBackSlash(
					path.Replace(InvoiceConstants.CONST_ROOT_PATH_TAG, Constants.PHYSICALDIRPATH_FRONT_PC));
			}

			// Return default image path
			return Path.Combine(
				Constants.PHYSICALDIRPATH_FRONT_PC,
				path);
		}

		/// <summary>Block type</summary>
		public InvoiceConstants.BlockType? Type { get; set; }
		/// <summary>Language locale code</summary>
		public string LanguageLocaleId { get; set; }
		/// <summary>Language code</summary>
		public string LanguageCode { get; set; }
		/// <summary>Text elements</summary>
		public TextNodeSetting[] Texts { get; set; }
		/// <summary>Price text elements</summary>
		public PriceTextNodeSetting[] PriceTexts { get; set; }
		/// <summary>Max display items</summary>
		public int? MaxDisplayItems { get; set; }
		/// <summary>Logo image path</summary>
		public string LogoImagePath { get; set; }
		/// <summary>Is title</summary>
		public bool IsTitle
		{
			get { return (this.Type == InvoiceConstants.BlockType.Title); }
		}
		/// <summary>Is body header</summary>
		public bool IsBodyHeader
		{
			get { return (this.Type == InvoiceConstants.BlockType.Body); }
		}
		/// <summary>Is detail</summary>
		public bool IsDetail
		{
			get { return (this.Type == InvoiceConstants.BlockType.Detail); }
		}
		/// <summary>Is product</summary>
		public bool IsProduct
		{
			get { return (this.Type == InvoiceConstants.BlockType.Product); }
		}
		/// <summary>Is price</summary>
		public bool IsPrice
		{
			get { return (this.Type == InvoiceConstants.BlockType.Price); }
		}
		/// <summary>Is footer</summary>
		public bool IsFooter
		{
			get { return (this.Type == InvoiceConstants.BlockType.Footer); }
		}
	}
}
