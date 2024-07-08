/*
=========================================================================================================
  Module      : Price Text Node Setting(PriceTextNodeSetting.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using w2.Common.Util;

namespace w2.App.Common.Pdf.Settings.Invoice
{
	/// <summary>
	/// Price Text Node Setting
	/// </summary>
	internal class PriceTextNodeSetting : SettingBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="element">Element</param>
		public PriceTextNodeSetting(XElement element)
			: base(element)
		{
		}

		/// <summary>
		/// Initialize properties
		/// </summary>
		protected override void InitializeProperties()
		{
			this.Value = string.Empty;
			this.DisplayName = string.Empty;
			this.ID = string.Empty;
			this.DisplayFreeAmountFlg = true;
		}

		/// <summary>
		/// Set properties
		/// </summary>
		/// <param name="element">Element</param>
		protected override void SetProperties(XElement element)
		{
			// Get text settings
			this.Value = element.Value.Trim();

			// Get display name settings
			this.DisplayName = GetAttributeValue(element, InvoiceConstants.CONST_XML_ATTRIBUTE_NAME_DISPLAYNAME);

			this.ID = GetAttributeValue(element, InvoiceConstants.CONST_XML_ATTRIBUTE_NAME_ID);

			// Get display free amount flag settings
			var displayFreeAmountFlgString = GetAttributeValue(element, InvoiceConstants.CONST_XML_ATTRIBUTE_NAME_DISPLAYFREEAMOUNTFLG);
			bool displayFreeAmountFlg;
			if (bool.TryParse(displayFreeAmountFlgString, out displayFreeAmountFlg))
			{
				this.DisplayFreeAmountFlg = displayFreeAmountFlg;
			}

			CreateFieldInformations();
		}

		/// <summary>
		/// Create field informations
		/// </summary>
		private void CreateFieldInformations()
		{
			if (this.HasValue == false) return;

			var tempValue = (string)this.Value.Clone();
			var tagCount = 0;

			// For case the price text set many fields,
			// current use for set promotion discount
			if (InvoiceConstants.CONST_OPERATORS_FOR_CALCULATOR.Any(item => this.Value.Contains(item))
				&& this.Value.Contains(InvoiceConstants.CONST_PREFIX_FIELD_SETPROMOTION))
			{
				this.FieldNameText = string.Format(
					InvoiceConstants.CONST_FORMAT_FIELD_NAME_TEXT,
					InvoiceConstants.CONST_FIELD_SETPROMOTION_DISCOUNT);
				this.FieldNameValue = string.Format(
					InvoiceConstants.CONST_FORMAT_FIELD_NAME_VALUE,
					InvoiceConstants.CONST_FIELD_SETPROMOTION_DISCOUNT);

				// Get field tags from value setting
				var fieldTags = new Dictionary<string, string>();
				var matchFieldTagResults = Regex.Matches(this.Value, InvoiceConstants.CONST_FORMAT_PATTERN_FIELD_TAG);
				foreach (Match m in matchFieldTagResults)
				{
					var newTag = string.Format(InvoiceConstants.CONST_FORMAT_NEW_TAG, tagCount);
					var key = string.Format(InvoiceConstants.CONST_FORMAT_TAG, tagCount);
					tempValue = tempValue.Replace(m.Value, newTag);
					fieldTags.Add(key, m.Value);
					tagCount++;
				}

				var fieldValue = string.Empty;
				foreach (var item in tempValue.Split(InvoiceConstants.CONST_TAB_DELIMITER))
				{
					if (string.IsNullOrEmpty(item.Trim())) continue;

					if (InvoiceConstants.CONST_OPERATORS_FOR_CALCULATOR.Contains(item.Trim()))
					{
						fieldValue += item;
						continue;
					}

					if (fieldTags.ContainsKey(item))
					{
						// Get field name setting
						var fieldName = fieldTags[item].TrimAllSpaces().Replace("@@", string.Empty);
						fieldValue += string.Format(
							InvoiceConstants.CONST_FORMAT_FIELD_VALUE,
							fieldName);
					}
				}
				this.FieldValue = fieldValue;
			}
			else
			{
				// Get field name setting
				var fieldName = this.Value.TrimAllSpaces().Replace("@@", string.Empty);

				// For case the field name setting is set promotion discount
				if (fieldName.Contains(InvoiceConstants.CONST_PREFIX_FIELD_SETPROMOTION))
				{
					fieldName = InvoiceConstants.CONST_FIELD_SETPROMOTION_DISCOUNT;
				}

				this.FieldNameText = string.Format(
					InvoiceConstants.CONST_FORMAT_FIELD_NAME_TEXT,
					fieldName);
				this.FieldNameValue = string.Format(
					InvoiceConstants.CONST_FORMAT_FIELD_NAME_VALUE,
					fieldName);
				this.FieldValue = string.Format(
					InvoiceConstants.CONST_FORMAT_FIELD_VALUE,
					fieldName);
			}

			// Create condition string
			if ((this.DisplayFreeAmountFlg == false)
				&& (string.IsNullOrEmpty(this.FieldValue) == false))
			{
				this.Condition = string.Format(
					InvoiceConstants.CONST_FORMAT_EQUAL_TO_ZERO_CONDITION,
					this.FieldValue.Replace("=", string.Empty));
			}
		}

		/// <summary>Value</summary>
		public string Value { get; set; }
		/// <summary>Display name</summary>
		public string DisplayName { get; set; }
		/// <summary>ID</summary>
		public string ID { get; set; }
		/// <summary>Display free amount flag</summary>
		public bool DisplayFreeAmountFlg { get; set; }
		/// <summary>Has value</summary>
		public bool HasValue
		{
			get { return (string.IsNullOrEmpty(this.Value) == false); }
		}
		/// <summary>Field name</summary>
		public string FieldName { get; set; }
		/// <summary>Field name text</summary>
		public string FieldNameText { get; set; }
		/// <summary>Field value</summary>
		public string FieldValue { get; set; }
		/// <summary>Field name value</summary>
		public string FieldNameValue { get; set; }
		/// <summary>Condition</summary>
		public string Condition { get; set; }
		/// <summary>Has condition</summary>
		public bool HasCondition
		{
			get { return (string.IsNullOrEmpty(this.Condition) == false); }
		}
	}
}
