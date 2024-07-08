/*
=========================================================================================================
  Module      : Invoice Setting(InvoiceSetting.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using w2.App.Common.ShopMessage;
using w2.Common.Util;

namespace w2.App.Common.Pdf.Settings.Invoice
{
	/// <summary>
	/// Invoice Setting
	/// </summary>
	internal class InvoiceSetting
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="settingFilePath">Setting file path</param>
		public InvoiceSetting(string settingFilePath)
		{
			this.BlockSettings = new InvoiceBlockSetting[0];
			this.TagShopMessages = ShopMessageUtil.GetMailTagArrayByShopMassage().ToList();

			if (File.Exists(settingFilePath))
			{
				LoadSettings(settingFilePath);
			}
		}

		/// <summary>
		/// Replace image data element by setting
		/// </summary>
		/// <param name="document">Document</param>
		/// <returns>Composition of image data had replaced new value</returns>
		public XElement ReplaceImageDataElementBySetting(XDocument document)
		{
			if (this.FooterBlockSetting == null) return document.Root;

			// Get first image data element
			var imageElement = document.Root.GetElements(InvoiceConstants.CONST_XML_ELEMENT_NAME_EMBEDDEDIMAGE)
				.FirstOrDefault(node => (node.Attribute(InvoiceConstants.CONST_XML_ATTRIBUTE_NAME_NAME).Value == InvoiceConstants.CONST_LOGO_IMAGE_NAME));
			var imageDataElement = (imageElement != null)
				? imageElement.GetFirstElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_IMAGEDATA)
				: null;

			// Convert image setting to base64 string, check and set value in the element
			var imageBase64String = ConvertImageToBase64String(this.FooterBlockSetting.LogoImagePath);
			if ((imageDataElement != null)
				&& (string.IsNullOrEmpty(imageBase64String) == false))
			{
				imageDataElement.SetValue(imageBase64String);
			}
			return document.Root;
		}

		/// <summary>
		/// Replace header and footer element by setting
		/// </summary>
		/// <param name="document">Document</param>
		/// <returns>Composition of header and footer had replaced new value</returns>
		public XElement ReplaceHeaderAndFooterElementBySetting(XDocument document)
		{
			// For case title block has setting
			// Replace header title default element by new setting
			if (this.TitleBlockSetting != null)
			{
				var header = document.Descendants(document.Root.GetDefaultNamespace() + InvoiceConstants.CONST_XML_ELEMENT_NAME_PAGEHEADER)
					.FirstOrDefault();

				// Get Textbox elements and replace new setting value
				var textboxes = GetTextboxElements(header).ToList();
				textboxes = ReplaceTextboxElementsBySettings(
					this.TitleBlockSetting,
					textboxes);

				// Modify all children element in parent footer element
				ModifyTextboxChildrenElementsByName(
					header,
					textboxes,
					InvoiceConstants.CONST_XML_ELEMENT_NAME_REPORTITEMS);
			}

			// For case footer block has setting
			// Replace footer default element by new setting
			if (this.FooterBlockSetting != null)
			{
				var footer = document.Descendants(document.Root.GetDefaultNamespace() + InvoiceConstants.CONST_XML_ELEMENT_NAME_PAGEFOOTER)
					.FirstOrDefault();

				// Get Textbox elements and replace new setting value
				var textboxes = GetTextboxElements(footer).ToList();
				textboxes = ReplaceTextboxElementsBySettings(
					this.FooterBlockSetting,
					textboxes);

				// Modify all children element in parent footer element
				ModifyTextboxChildrenElementsByName(
					footer,
					textboxes,
					InvoiceConstants.CONST_XML_ELEMENT_NAME_REPORTITEMS);

				// Get image element and set value is w2logo, use to replace logo image settings
				var imageElement = footer.GetElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_REPORTITEMS)
					?.GetElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_IMAGE);
				var imageBase64String = ConvertImageToBase64String(this.FooterBlockSetting.LogoImagePath);
				if ((imageElement != null)
					&& (string.IsNullOrEmpty(imageBase64String) == false))
				{
					imageElement.GetElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_VALUE)
						?.SetValue(InvoiceConstants.CONST_LOGO_IMAGE_NAME);
				}
			}

			return document.Root;
		}

		/// <summary>
		/// Replace body header element by setting
		/// </summary>
		/// <param name="document">Document</param>
		/// <returns>Composition of body header element</returns>
		public XElement ReplaceBodyHeaderElementBySetting(XDocument document)
		{
			if (this.BodyHeaderBlockSetting == null) return document.Root;

			// Get Textbox elements and replace new setting value
			var textboxes = GetTextboxElements(document.Root)
				.Where(node => (node.Attribute(InvoiceConstants.CONST_XML_ATTRIBUTE_NAME_NAME).Value.Contains("owner_zip") == false))
				.ToList();
			textboxes = ReplaceTextboxElementsBySettings(
				this.BodyHeaderBlockSetting,
				textboxes);

			// Modify all children element in parent footer element
			ModifyTextboxChildrenElementsByName(
				document.Root,
				textboxes,
				InvoiceConstants.CONST_XML_ELEMENT_NAME_REPORTITEMS,
				new[] { "owner_zip" });

			return document.Root;
		}

		/// <summary>
		/// Replace detail and price in body element by setting
		/// </summary>
		/// <param name="document">Document</param>
		/// <returns>Composition of body element</returns>
		public XElement ReplaceDetailAndPriceInBodyElementBySetting(XDocument document)
		{
			// For case detail block has setting
			// Replace detail default element by new setting
			if (this.DetailBlockSetting != null)
			{
				var bodyDetail = document.Descendants(document.Root.GetDefaultNamespace() + InvoiceConstants.CONST_XML_ELEMENT_NAME_RECTANGLE)
					.FirstOrDefault(node => (node.Attribute(InvoiceConstants.CONST_XML_ATTRIBUTE_NAME_NAME).Value == InvoiceConstants.CONST_BODY_DETAIL_NAME));
				if (bodyDetail != null)
				{
					// Get Textbox elements and replace new setting value
					var textboxes = GetTextboxElements(bodyDetail).ToList();
					textboxes = ReplaceTextboxElementsBySettings(
						this.DetailBlockSetting,
						textboxes);

					// Modify all children element in parent body detail element
					ModifyTextboxChildrenElementsByName(
						bodyDetail,
						textboxes,
						InvoiceConstants.CONST_XML_ELEMENT_NAME_REPORTITEMS,
						isAppend: true);
				}
			}

			// For case price block has setting
			// Replace price default element by new setting
			if (this.PriceBlockSetting != null)
			{
				AddPriceTextsInBodyElementBySetting(document, this.PriceBlockSetting);
			}

			return document.Root;
		}

		/// <summary>
		/// Replace max display items in code element by setting
		/// </summary>
		/// <param name="document">Document</param>
		/// <returns>Composition of code element</returns>
		public XElement ReplaceMaxDisplayItemsInCodeElementBySetting(XDocument document)
		{
			if (this.ProductBlockSetting == null) return document.Root;

			var code = document.Root.GetFirstElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_CODE);
			if (code == null) return document.Root;

			var value = code.Value;
			var oldMaxFirstPageRows = Regex.Match(value, InvoiceConstants.CONST_FORMAT_PATTERN_MAXFIRSTPAGEROWS).Value;
			var oldMaxFirstPageRowsNumber = Regex.Match(oldMaxFirstPageRows, InvoiceConstants.CONST_FORMAT_PATTERN_NUMBER).Value;
			var newMaxFirstPageRows = oldMaxFirstPageRows.Replace(
				oldMaxFirstPageRowsNumber,
				StringUtility.ToEmpty(this.ProductBlockSetting.MaxDisplayItems));
			value = value.Replace(oldMaxFirstPageRows, newMaxFirstPageRows);

			var oldMaxRows = Regex.Match(value, InvoiceConstants.CONST_FORMAT_PATTERN_MAXROWS).Value;
			var oldMaxRowsNumber = Regex.Match(oldMaxRows, InvoiceConstants.CONST_FORMAT_PATTERN_NUMBER).Value;
			var newMaxRows = oldMaxRows.Replace(
				oldMaxRowsNumber,
				StringUtility.ToEmpty(this.ProductBlockSetting.MaxDisplayItems));
			value = value.Replace(oldMaxRows, newMaxRows);

			var oldDefaultMaxFirstPageRows = Regex.Match(value, InvoiceConstants.CONST_FORMAT_PATTERN_DEFAULT_MAXFIRSTPAGEROWS).Value;
			var oldDefaultMaxFirstPageRowsNumber = Regex.Match(oldDefaultMaxFirstPageRows, InvoiceConstants.CONST_FORMAT_PATTERN_NUMBER).Value;
			var newDefaultMaxFirstPageRowsNumber = InvoiceHelper.TryParseInt(oldMaxFirstPageRowsNumber, 10)
				+ ((this.DetailBlockSetting != null) ? -1 : 0);
			var newDefaultMaxFirstPageRows = oldDefaultMaxFirstPageRows.Replace(
				oldDefaultMaxFirstPageRowsNumber,
				StringUtility.ToEmpty(newDefaultMaxFirstPageRowsNumber));
			value = value.Replace(oldDefaultMaxFirstPageRows, newDefaultMaxFirstPageRows);

			var oldDefaultMaxRows = Regex.Match(value, InvoiceConstants.CONST_FORMAT_PATTERN_DEFAULT_MAXROWS).Value;
			var oldDefaultMaxRowsNumber = Regex.Match(oldDefaultMaxRows, InvoiceConstants.CONST_FORMAT_PATTERN_NUMBER).Value;
			var newDefaultMaxRowsNumber = InvoiceHelper.TryParseInt(oldMaxRowsNumber, 19) + 1;
			var newDefaultMaxRows = oldDefaultMaxRows.Replace(
				oldDefaultMaxRowsNumber,
				StringUtility.ToEmpty(newDefaultMaxRowsNumber));
			value = value.Replace(oldDefaultMaxRows, newDefaultMaxRows);

			code.SetValue(value);

			return document.Root;
		}

		/// <summary>
		/// Replace price texts in body element by setting
		/// </summary>
		/// <param name="document">Document</param>
		/// <param name="blockSetting">Block setting</param>
		/// <returns>Composition of body element</returns>
		private XElement AddPriceTextsInBodyElementBySetting(XDocument document, InvoiceBlockSetting blockSetting)
		{
			// Get range of the list TablixRow, which has price setting (this TablixRow node from 9 to 23 in the Body file XML)
			var taxlixRows = document.Descendants(document.Root.GetDefaultNamespace() + InvoiceConstants.CONST_XML_ELEMENT_NAME_TABLIXROW)
				.Skip(8)
				.ToList();

			// Get range of the list TablixMember, which is the one corresponding to the TablixMember node from 9 to 23 in the Body file XML
			var taxlixMembersOfOrderIdGroup = document.Descendants(document.Root.GetDefaultNamespace() + InvoiceConstants.CONST_XML_ELEMENT_NAME_TABLIXMEMBER)
				.Where(node => (node.HasElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_GROUP)
					&& (node.GetElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_GROUP)
						.Attribute(InvoiceConstants.CONST_XML_ATTRIBUTE_NAME_NAME).Value == InvoiceConstants.CONST_GROUP_NAME_ORDER_ID)))
				.SelectMany(node => node.GetElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_TABLIXMEMBERS).Elements())
				.Skip(3)
				.ToList();

			var rectangles = document.Descendants(document.Root.GetDefaultNamespace() + InvoiceConstants.CONST_XML_ELEMENT_NAME_RECTANGLE)
			.Skip(8)
			.Select(items => items.Attribute(InvoiceConstants.CONST_XML_ATTRIBUTE_NAME_NAME).Value)
			.ToList();
			foreach (var setting in blockSetting.PriceTexts)
			{
				var priceTextIndex = rectangles.IndexOf(setting.ID);
				if (setting.ID == InvoiceConstants.CONST_XML_ATTRIBUTE_RECTANGLE_BODY_NAME_RELATION_MEMO) continue;
				if (priceTextIndex == -1) 
				{
					if (setting.HasValue == false) continue;

						var templateTablixrow = XElement.Parse(taxlixRows.ElementAtOrDefault(taxlixRows.Count - 2).ToString());
						// Get the display Textbox to set display name setting
						var textboxes = GetTextboxElements(templateTablixrow).ToArray();
						var displayTextbox = textboxes.FirstOrDefault(node => IsTextboxWithName(node, InvoiceConstants.CONST_TEMPLATE_FIELD_NAME_TEXT));
						ModifyValueOfTextRunInTextboxElementForPriceTextSetting(displayTextbox, setting.DisplayName);

						// Get the value Textbox to set value setting
						var valueTextbox = textboxes.FirstOrDefault(node => IsTextboxWithName(node, InvoiceConstants.CONST_TEMPLATE_FIELD_NAME_VALUE));
						ModifyValueOfTextRunInTextboxElementForPriceTextSetting(valueTextbox, setting.FieldValue, false);

						ModifyNameForNewTablixRow(templateTablixrow);
						taxlixRows[taxlixRows.Count - 4].AddAfterSelf(templateTablixrow);

						var templateTablixMember = new XElement(document.Root.GetDefaultNamespace() + InvoiceConstants.CONST_XML_ELEMENT_NAME_TABLIXMEMBER);

						if (setting.HasCondition) AddVisibilityConditionForTablixMember(templateTablixMember, setting.Condition);

						taxlixMembersOfOrderIdGroup[taxlixMembersOfOrderIdGroup.Count - 4].AddAfterSelf(templateTablixMember);
				}
				else if (setting.DisplayFreeAmountFlg == false)
				{
					var rectanglesValue = document.Descendants(document.Root.GetDefaultNamespace() + InvoiceConstants.CONST_XML_ELEMENT_NAME_RECTANGLE).Skip(8).ToList();
					var rectangleValue = rectanglesValue[priceTextIndex].Value;
					var priceValues = Regex.Matches(rectangleValue, InvoiceConstants.CONST_REGEXP_GET_PRICE_VALUE);
					setting.FieldValue = string.Join("+", priceValues.Cast<Match>().Select(item => item.Value));
					setting.Condition = string.Format(
					InvoiceConstants.CONST_FORMAT_EQUAL_TO_ZERO_CONDITION,
					setting.FieldValue);
					var tablixMember = taxlixMembersOfOrderIdGroup[priceTextIndex];
					AddVisibilityConditionForTablixMember(tablixMember, setting.Condition);
				}
			}

			return document.Root;
		}

		/// <summary>
		/// Modify name for new tablixrow
		/// </summary>
		/// <param name="templateElement">Template element</param>
		private void ModifyNameForNewTablixRow(XElement templateElement)
		{
			var rectangle = templateElement.GetFirstElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_RECTANGLE);
			if (rectangle != null) ModifyNameForNewElement(rectangle);

			var textboxes = templateElement.GetElements(InvoiceConstants.CONST_XML_ELEMENT_NAME_TEXTBOX).ToArray();
			if (textboxes.Length > 0)
			{
				foreach (var textbox in textboxes)
				{
					ModifyNameForNewElement(textbox);
				}
			}

			var lines = templateElement.GetElements(InvoiceConstants.CONST_XML_ELEMENT_NAME_LINE).ToArray();
			if (lines.Length > 0)
			{
				foreach (var line in lines)
				{
					ModifyNameForNewElement(line);
				}
			}
		}

		/// <summary>
		/// Add visibility condition for TablixMember
		/// </summary>
		/// <param name="tablixMember">TablixMember</param>
		/// <param name="condition">Condition</param>
		private void AddVisibilityConditionForTablixMember(XElement tablixMember, string condition)
		{
			if (tablixMember.HasElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_VISIBILITY))
			{
				var visibility = tablixMember.GetElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_VISIBILITY);
				if (visibility.HasElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_HIDDEN))
				{
					var hidden = visibility.GetElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_HIDDEN);
					var oldValue = hidden.Value.Trim();
					var newValue = condition;
					if ((oldValue.Length != 0) && oldValue.StartsWith("="))
					{
						newValue = string.Format(
							InvoiceConstants.CONST_FORMAT_OR_FOR_TWO_CONDITION,
							condition,
							oldValue.TrimStart('='));
					}

					hidden.SetValue(newValue);
				}
				else
				{
					var defaultNamespace = visibility.GetDefaultNamespace();
					var hidden = new XElement(
						defaultNamespace + InvoiceConstants.CONST_XML_ELEMENT_NAME_HIDDEN,
						condition);
					visibility.Add(hidden);
				}
			}
			else
			{
				var defaultNamespace = tablixMember.Name.Namespace;
				var visibility = new XElement(
					defaultNamespace + InvoiceConstants.CONST_XML_ELEMENT_NAME_VISIBILITY,
					new XElement(
						defaultNamespace + InvoiceConstants.CONST_XML_ELEMENT_NAME_HIDDEN,
						condition));
				tablixMember.Add(visibility);
			}
		}

		/// <summary>
		/// Modify value of TextRun in Textbox element for price text setting
		/// </summary>
		/// <param name="textbox">Textbox</param>
		/// <param name="newValue">New value</param>
		/// <param name="isDisplayTextbox">Is display Textbox</param>
		private void ModifyValueOfTextRunInTextboxElementForPriceTextSetting(
			XElement textbox,
			string newValue,
			bool isDisplayTextbox = true)
		{
			if (textbox == null) return;

			// Get TextRun element
			var textRun = textbox.GetFirstElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_TEXTRUN);
			if (textRun == null) return;

			// Get value element and set new value
			var defaultNamespace = textRun.GetDefaultNamespace();
			var value = textRun.GetFirstElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_VALUE);
			if (value != null)
			{
				// Get old value and check its a condition in this value
				var oldValue = value.Value.Trim();
				if (oldValue.StartsWith("=IIF(")
					&& (isDisplayTextbox == false))
				{
					// Get in
					var indexOfComma = oldValue.LastIndexOf(",");
					if ((indexOfComma == -1)
						|| ((indexOfComma + 1) > oldValue.Length)) return;

					// Get the last clause in the condition
					var valueForReplacer = oldValue.Substring(indexOfComma + 1);
					var operatorValue = InvoiceConstants.CONST_OPERATORS_FOR_CALCULATOR
						.FirstOrDefault(item => valueForReplacer.Trim().StartsWith(item));
					var adjustedValue = oldValue;
					if (valueForReplacer.EndsWith("))"))
					{
						adjustedValue = string.Format(
							" {0}({1}))",
							StringUtility.ToEmpty(operatorValue),
							newValue);
					}
					else if (valueForReplacer.EndsWith(")"))
					{
						adjustedValue = string.Format(
							" {0}{1})",
							StringUtility.ToEmpty(operatorValue),
							newValue);
					}

					// Replace and set new value
					adjustedValue = oldValue.Replace(valueForReplacer, adjustedValue.Replace("=", string.Empty));
					value.SetValue(adjustedValue);
				}
				else
				{
					value.SetValue(newValue);
				}
			}
			else
			{
				textRun.SetElementValue(defaultNamespace + InvoiceConstants.CONST_XML_ELEMENT_NAME_VALUE, newValue);
			}
		}

		/// <summary>
		/// Is Textbox with name
		/// </summary>
		/// <param name="textbox">Textbox</param>
		/// <param name="name">Name</param>
		/// <returns>True: If the element has Textbox with input name</returns>
		private bool IsTextboxWithName(XElement textbox, string name)
		{
			var result = (textbox.Attribute(InvoiceConstants.CONST_XML_ATTRIBUTE_NAME_NAME).Value == name);
			return result;
		}

		/// <summary>
		/// Replace Textbox elements by settings
		/// </summary>
		/// <param name="blockSetting">Block setting</param>
		/// <param name="parentElements">Parent elements</param>
		/// <returns>New Textbox elements</returns>
		private List<XElement> ReplaceTextboxElementsBySettings(
			InvoiceBlockSetting blockSetting,
			List<XElement> parentElements)
		{
			if (parentElements.Count == 0) return parentElements;

			var index = 0;
			var templateTextbox = parentElements.Last().ToString();
			var newElements = new List<XElement>();
			var addedZIndexCount = 0;
			foreach (var setting in blockSetting.Texts.Where(item => (string.IsNullOrEmpty(item.Value) == false)))
			{
				var isAddNewTextbox = ((index >= parentElements.Count) || blockSetting.IsDetail);
				var textbox = (isAddNewTextbox == false)
					? parentElements[index]
					: XElement.Parse(templateTextbox);

				// Get Paragraph elements and replace new setting value
				var paragraphs = textbox.GetElements(InvoiceConstants.CONST_XML_ELEMENT_NAME_PARAGRAPH).ToList();
				paragraphs = ReplaceParagraphElementsBySetting(
					blockSetting,
					paragraphs,
					setting);

				// Modify all children element in parent textbox element
				ModifyChildrenElementsByName(
					textbox,
					paragraphs,
					InvoiceConstants.CONST_XML_ELEMENT_NAME_PARAGRAPHS);

				// Modify parts-align|margin|width style setting in textbox
				ModifyStyleTextboxElementBySetting(textbox, setting);

				// For case the textbox is new creation, modify name attribute for this textbox
				if (isAddNewTextbox)
				{
					ModifyNameForNewElement(textbox);

					// Adjust the top of the Textbox according to the current top and height
					AdjustTopStyleForNewTextbox(textbox, setting);

					// Adjust the width of the Textbox according to the current left and width
					AdjustWidthStyleForNewTextbox(setting, textbox);

					// Adjust the height of the Textbox according to the Paragraphs
					AdjustHeightStyleForNewTextbox(textbox, setting);

					// Adjust the ZIndex of the Textbox according to the current max ZIndex
					AdjustZIndexStyleForNewTextbox(parentElements.Last(), textbox, addedZIndexCount);
					addedZIndexCount++;
				}

				newElements.Add(textbox);
				index++;
				
				if (setting.Value.Contains(InvoiceConstants.CONST_FIELD_VALUE_NEXT_SHIPPING_DATE)) this.IsUnderLocationNextShippingDate = true;
			}

			return (newElements.Count != 0) ? newElements : parentElements;
		}

		/// <summary>
		/// Replace Paragraph elements by settings
		/// </summary>
		/// <param name="blockSetting">Block setting</param>
		/// <param name="parentElements">Parent elements</param>
		/// <param name="setting">Setting</param>
		/// <returns>New Paragraph elements</returns>
		private List<XElement> ReplaceParagraphElementsBySetting(
			InvoiceBlockSetting blockSetting,
			List<XElement> parentElements,
			TextNodeSetting setting)
		{
			if ((parentElements.Count == 0) || string.IsNullOrEmpty(setting.Value)) return parentElements;

			var index = 0;
			var templateParagraph = parentElements.Last().ToString();
			var newElements = new List<XElement>();

			// Get TextRun objects for TextRun element creation
			var settingLines = GetTextRunInformationsFromSetting(blockSetting, setting.Value);
			foreach (var key in settingLines.Keys)
			{
				var paragraph = (index < parentElements.Count)
					? parentElements[index]
					: XElement.Parse(templateParagraph);

				// Get TextRun elements and replace new setting value
				var textRuns = paragraph.GetElements(InvoiceConstants.CONST_XML_ELEMENT_NAME_TEXTRUN).ToList();
				textRuns = ReplaceTextRunElementsBySetting(
					blockSetting,
					textRuns,
					setting,
					settingLines[key]);

				// Modify all children element in parent paragraph element
				ModifyChildrenElementsByName(
					paragraph,
					textRuns,
					InvoiceConstants.CONST_XML_ELEMENT_NAME_TEXTRUNS);

				// Modify text align style setting in paragraph
				ModifyStyleParagraphElementBySetting(paragraph, setting);

				newElements.Add(paragraph);
				index++;
			}

			return (newElements.Count != 0) ? newElements : parentElements;
		}

		/// <summary>
		/// Replace TextRun elements by settings
		/// </summary>
		/// <param name="blockSetting">Block setting</param>
		/// <param name="parentElements">Parent elements</param>
		/// <param name="setting">Setting</param>
		/// <param name="textRuns">TextRuns</param>
		/// <returns>New TextRun elements</returns>
		private List<XElement> ReplaceTextRunElementsBySetting(
			InvoiceBlockSetting blockSetting,
			List<XElement> parentElements,
			TextNodeSetting setting,
			List<TextRunObject> textRuns)
		{
			if ((parentElements.Count == 0) || (textRuns.Count == 0)) return parentElements;

			var index = 0;
			var templateTextRun = parentElements.Last().ToString();
			var newElements = new List<XElement>();
			foreach (var text in textRuns)
			{
				var textRun = (index < parentElements.Count)
					? parentElements[index]
					: XElement.Parse(templateTextRun);

				// Replace new setting value for TextRun
				textRun = ReplaceTextRunElementBySetting(
					blockSetting,
					textRun,
					text);

				newElements.Add(textRun);
				index++;
			}

			return (newElements.Count != 0) ? newElements : parentElements;
		}

		/// <summary>
		/// Replace TextRun element by settings
		/// </summary>
		/// <param name="blockSetting">Block setting</param>
		/// <param name="element">Element</param>
		/// <param name="textRun">TextRun</param>
		/// <returns>New TextRun element</returns>
		private XElement ReplaceTextRunElementBySetting(
			InvoiceBlockSetting blockSetting,
			XElement element,
			TextRunObject textRun)
		{
			var newElement = XElement.Parse(element.ToString());

			// Set new value
			var defaultNamespace = newElement.GetDefaultNamespace();
			var value = newElement.GetFirstElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_VALUE);
			if (value != null)
			{
				value.SetValue(textRun.Text);
			}
			else
			{
				newElement.SetElementValue(defaultNamespace + InvoiceConstants.CONST_XML_ELEMENT_NAME_VALUE, textRun.Text);
			}

			// Set new style
			var style = newElement.GetFirstElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_STYLE);
			var newStyle = textRun.CreateStyleElement(defaultNamespace, blockSetting);
			if (style != null)
			{
				style.ReplaceWith(newStyle);
			}
			else
			{
				newElement.Add(newStyle);
			}

			// Set new label for field setting
			if (textRun.IsField)
			{
				var newLabel = textRun.CreateLabelElement(defaultNamespace);
				if (newElement.HasElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_LABEL))
				{
					var label = newElement.GetFirstElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_LABEL);
					label.SetValue(newLabel.Value);
				}
				else
				{
					newElement.Add(newLabel);
				}
			}

			return newElement;
		}

		/// <summary>
		/// Modify style paragraph element by setting
		/// </summary>
		/// <param name="source">Source</param>
		/// <param name="setting">Setting</param>
		private void ModifyStyleParagraphElementBySetting(XElement source, TextNodeSetting setting)
		{
			// Modify text align style
			if (source.HasElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_STYLE))
			{
				var style = source.GetElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_STYLE);
				var newTextAlign = setting.CreateTextAlignElement(style.GetDefaultNamespace());
				if (style.HasElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_TEXTALIGN))
				{
					var textAlign = style.GetElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_TEXTALIGN);
					textAlign.SetValue(newTextAlign.Value);
				}
				else
				{
					style.Add(newTextAlign);
				}
			}
			else
			{
				source.Add(setting.CreateTextAlignElement(source.GetDefaultNamespace(), true));
			}

			// テキストの色をInvoiceSetting.xmlで設定されているものに変更する。
			var styles = source
				.Descendants(source.GetDefaultNamespace() + InvoiceConstants.CONST_XML_ELEMENT_NAME_STYLE)
				.ToList();
			var newTextColor = setting.CreateTextColorElement(source.GetDefaultNamespace());
			if( string.IsNullOrEmpty(newTextColor.Value) == false)
			{
				foreach (var style in styles)
				{
					if (style.HasElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_COLOR))
					{
						var color = style.GetElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_COLOR);
						color.SetValue(newTextColor.Value);
					}
					else
					{
						style.Add(newTextColor);
					}
				}
			}
		}

		/// <summary>
		/// Modify style textbox element by setting
		/// </summary>
		/// <param name="source">Source</param>
		/// <param name="setting">Setting</param>
		private void ModifyStyleTextboxElementBySetting(XElement source, TextNodeSetting setting = null)
		{
			// Modify left style (parts-align in file setting)
			var newLeft = setting.CreateLeftElement(
				source.GetDefaultNamespace(),
				CalculateLeftStyleValue(source, setting.Width));
			if (source.HasElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_LEFT))
			{
				var left = source.GetElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_LEFT);
				if (newLeft != null)
				{
					left.SetValue(newLeft.Value);
				}
				else
				{
					left.Remove();
				}
			}
			else if (newLeft != null)
			{
				source.Add(newLeft);
			}

			if (source.HasElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_STYLE))
			{
				var style = source.GetElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_STYLE);

				// Modify padding left style (margin-left in file setting)
				var newPaddingLeft = setting.CreatePaddingLeftElement(style.GetDefaultNamespace());
				if (style.HasElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_PADDINGLEFT))
				{
					var paddingLeft = style.GetElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_PADDINGLEFT);
					paddingLeft.SetValue(newPaddingLeft.Value);
				}
				else
				{
					style.Add(newPaddingLeft);
				}

				// Modify padding right style (margin-right in file setting)
				var newPaddingRight = setting.CreatePaddingRightElement(style.GetDefaultNamespace());
				if (style.HasElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_PADDINGRIGHT))
				{
					var paddingRight = style.GetElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_PADDINGRIGHT);
					paddingRight.SetValue(newPaddingRight.Value);
				}
				else
				{
					style.Add(newPaddingRight);
				}

				// Modify padding top style (margin-top in file setting)
				var newPaddingTop = setting.CreatePaddingTopElement(style.GetDefaultNamespace());
				// 「次回配送日」の下に配置する場合は、paddingを加算する。
				if ((setting.PartsAlignAttributes[InvoiceConstants.CONST_XML_ELEMENT_NAME_LEFT] == InvoiceConstants.CONST_NEXT_SHIPPING_DATE_PARTSALIGN)
					&& this.IsUnderLocationNextShippingDate)
				{
					var underNextShippingDatePaddingPoint =  string.Format("{0}pt", (int.Parse(Regex.Match(setting.MarginAttributes[InvoiceConstants.CONST_XML_ELEMENT_NAME_PADDINGTOP], @"\d+").ToString()) + InvoiceConstants.CONST_UNDER_NEXT_SHIPPING_DATE_PADDING_POINT).ToString());
					newPaddingTop.Value = string.Format(InvoiceConstants.CONDITIONAL_EXPRESSION_FOR_NEXT_SHIPPING_DATE, "\"0pt\"", string.Format("\"{0}\"", underNextShippingDatePaddingPoint));
				}
				if (style.HasElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_PADDINGTOP))
				{
					var paddingTop = style.GetElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_PADDINGTOP);
					paddingTop.SetValue(newPaddingTop.Value);
				}
				else
				{
					style.Add(newPaddingTop);
				}

				// Modify padding bottom style (margin-bottom in file setting)
				var newPaddingBottom = setting.CreatePaddingBottomElement(style.GetDefaultNamespace());
				if (style.HasElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_PADDINGBOTTOM))
				{
					var paddingBottom = style.GetElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_PADDINGBOTTOM);
					paddingBottom.SetValue(newPaddingBottom.Value);
				}
				else
				{
					style.Add(newPaddingBottom);
				}
			}
			else
			{
				source.Add(setting.CreatePaddingStyleElement(source.GetDefaultNamespace()));
			}

			// Modify with style (with in file setting)
			if (source.HasElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_WIDTH))
			{
				var width = source.GetElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_WIDTH);
				var newWidth = setting.CreateWidthElement(width.GetDefaultNamespace());
				if (newWidth != null) width.SetValue(newWidth.Value);
			}
			else
			{
				var newWidth = setting.CreateWidthElement(source.GetDefaultNamespace());
				if (newWidth != null) source.Add(newWidth);
			}
		}

		/// <summary>
		/// Adjust ZIndex style for new Textbox
		/// </summary>
		/// <param name="oldTextbox">Old Textbox</param>
		/// <param name="newTextbox">New Textbox</param>
		/// <param name="addedZIndexCount">Added ZIndex count</param>
		private void AdjustZIndexStyleForNewTextbox(XElement oldTextbox, XElement newTextbox, int addedZIndexCount)
		{
			var maxZIndex = oldTextbox.Parent.GetElements(InvoiceConstants.CONST_XML_ELEMENT_NAME_TEXTBOX)
				.Max(node => GetElementIntValueByName(node, InvoiceConstants.CONST_XML_ELEMENT_NAME_ZINDEX));
			var adjustedZIndex = maxZIndex + 1 + addedZIndexCount;
			if (newTextbox.HasElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_ZINDEX))
			{
				var zIndex = newTextbox.GetElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_ZINDEX);
				zIndex.SetValue(adjustedZIndex.ToString());
			}
			else
			{
				newTextbox.Add(
					new XElement(
						newTextbox.GetDefaultNamespace() + InvoiceConstants.CONST_XML_ELEMENT_NAME_ZINDEX),
						adjustedZIndex.ToString());
			}
		}

		/// <summary>
		/// Adjust height style for new Textbox
		/// </summary>
		/// <param name="textbox">Textbox</param>
		/// <param name="textSetting">InvoiceSetting.xmlにあるテキストの設定</param>
		private void AdjustHeightStyleForNewTextbox(XElement textbox, TextNodeSetting textSetting = null)
		{
			var paragrapCount = textbox.GetElements(InvoiceConstants.CONST_XML_ELEMENT_NAME_PARAGRAPH).Length;
			var adjustedHeight = paragrapCount * InvoiceConstants.CONST_DEFAULT_HEIGHT_PER_PARAGRAPH;
			// 「次回配送日」の下に配置する場合は、paddingを加算する。
			if ((textSetting.PartsAlignAttributes[InvoiceConstants.CONST_XML_ELEMENT_NAME_LEFT] == InvoiceConstants.CONST_NEXT_SHIPPING_DATE_PARTSALIGN)
				&& this.IsUnderLocationNextShippingDate) adjustedHeight = adjustedHeight + InvoiceConstants.CONST_DEFAULT_HEIGHT_PER_PARAGRAPH;
			var adjustedHeightValue = string.Format(
				InvoiceConstants.CONST_FORMAT_STYLE_CENTIMETR,
				adjustedHeight);
			if (textbox.HasElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_HEIGHT))
			{
				var height = textbox.GetElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_HEIGHT);
				height.SetValue(adjustedHeightValue);
			}
			else
			{
				textbox.Add(
					new XElement(
						textbox.GetDefaultNamespace() + InvoiceConstants.CONST_XML_ELEMENT_NAME_HEIGHT),
						adjustedHeightValue);
			}
		}

		/// <summary>
		/// Adjust width style for new Textbox
		/// </summary>
		/// <param name="textbox">Textbox</param>
		private void AdjustWidthStyleForNewTextbox(TextNodeSetting setting, XElement textbox)
		{
			var currentLeft = GetElementDecimalValueByName(textbox, InvoiceConstants.CONST_XML_ELEMENT_NAME_LEFT);
			var currentWidth = GetElementDecimalValueByName(textbox, InvoiceConstants.CONST_XML_ELEMENT_NAME_WIDTH);
			var maxWidthPaper = (InvoiceConstants.CONST_DEFAULT_CENTER_DISTANCE * 2m) - InvoiceConstants.CONST_DEFAULT_HEIGHT_PER_PARAGRAPH;
			var adjustedWidth = currentWidth;
			switch (setting.PartsAlignAttributes[InvoiceConstants.CONST_XML_ELEMENT_NAME_LEFT])
			{
				case InvoiceConstants.CONST_ALIGN_KBN_CENTER:
					if ((currentLeft + currentWidth) >= maxWidthPaper)
					{
						adjustedWidth = maxWidthPaper - currentLeft;
					}
					break;

				case InvoiceConstants.CONST_ALIGN_KBN_RIGHT:
					if ((currentLeft + currentWidth) >= maxWidthPaper)
					{
						adjustedWidth = maxWidthPaper - currentLeft - InvoiceConstants.CONST_DEFAULT_HEIGHT_PER_PARAGRAPH;
					}
					break;

				case InvoiceConstants.CONST_ALIGN_KBN_LEFT:
					if (currentWidth > InvoiceConstants.CONST_DEFAULT_CENTER_DISTANCE)
					{
						adjustedWidth = InvoiceConstants.CONST_DEFAULT_CENTER_DISTANCE;
					}
					break;
			}

			var adjustedWidthValue = string.Format(
				InvoiceConstants.CONST_FORMAT_STYLE_CENTIMETR,
				adjustedWidth);
			if (textbox.HasElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_WIDTH))
			{
				var width = textbox.GetElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_WIDTH);
				width.SetValue(adjustedWidthValue);
			}
			else
			{
				textbox.Add(
					new XElement(
						textbox.GetDefaultNamespace() + InvoiceConstants.CONST_XML_ELEMENT_NAME_WIDTH),
						adjustedWidthValue);
			}
		}

		/// <summary>
		/// Adjust top style for new Textbox
		/// </summary>
		/// <param name="textbox">Textbox</param>
		/// <param name="textSetting">InvoiceSetting.xmlにあるテキストの設定</param>
		private void AdjustTopStyleForNewTextbox(XElement textbox, TextNodeSetting textSetting = null)
		{
			var currentTop = GetElementDecimalValueByName(textbox, InvoiceConstants.CONST_XML_ELEMENT_NAME_TOP);
			var currentHeight = GetElementDecimalValueByName(textbox, InvoiceConstants.CONST_XML_ELEMENT_NAME_HEIGHT);
			var adjustedTop = currentTop + currentHeight;
			// 明細ブロックの「次回配送日」と同じポジションであれば位置を調整する
			if (textSetting.PartsAlignAttributes[InvoiceConstants.CONST_XML_ELEMENT_NAME_LEFT] == InvoiceConstants.CONST_NEXT_SHIPPING_DATE_PARTSALIGN)
			{
				adjustedTop = adjustedTop + InvoiceConstants.CONST_NEXT_SHIPPING_DATE_SAME_POSITION_TOP_VALUE * this.TopAdjustmentMagnification;
				if(textSetting.Value.Contains(InvoiceConstants.CONST_FIELD_VALUE_NEXT_SHIPPING_DATE) == false) this.TopAdjustmentMagnification = this.TopAdjustmentMagnification + 1;
			}
			var adjustedTopValue = string.Format(
				InvoiceConstants.CONST_FORMAT_STYLE_CENTIMETR,
				adjustedTop);
			if (textbox.HasElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_TOP))
			{
				var height = textbox.GetElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_TOP);
				height.SetValue(adjustedTopValue);
			}
			else
			{
				textbox.Add(
					new XElement(
						textbox.GetDefaultNamespace() + InvoiceConstants.CONST_XML_ELEMENT_NAME_TOP),
						adjustedTopValue);
			}
		}

		/// <summary>
		/// Modify children elements by name
		/// </summary>
		/// <param name="source">Source</param>
		/// <param name="newElements">New elements</param>
		/// <param name="name">Parent element name</param>
		private void ModifyChildrenElementsByName(
			XElement source,
			List<XElement> newElements,
			string name)
		{
			var oldElement = source.GetElement(name);
			oldElement.Elements().Remove();
			foreach (var item in newElements)
			{
				oldElement.Add(item);
			}
		}

		/// <summary>
		/// Modify Textbox children elements by name
		/// </summary>
		/// <param name="source">Source</param>
		/// <param name="newElements">New elements</param>
		/// <param name="name">Parent element name</param>
		/// <param name="exceptAttributeNames">Except attribute names</param>
		/// <param name="isAppend">Is append</param>
		private void ModifyTextboxChildrenElementsByName(
			XElement source,
			List<XElement> newElements,
			string name,
			string[] exceptAttributeNames = null,
			bool isAppend = false)
		{

			var oldElement = source.GetElement(name);
			if (isAppend)
			{
				foreach (var item in newElements)
				{
					oldElement.Add(item);
				}
			}
			else
			{
				oldElement.Elements()
					.Where(node => ((node.Name.LocalName == InvoiceConstants.CONST_XML_ELEMENT_NAME_TEXTBOX)
						&& (node.IsHidden() == false)
						&& ((exceptAttributeNames == null)
							|| (exceptAttributeNames != null)
								&& (exceptAttributeNames.Any(item => node.Attribute(InvoiceConstants.CONST_XML_ATTRIBUTE_NAME_NAME).Value.Contains(item)) == false))))
					.Remove();
				foreach (var item in newElements)
				{
					oldElement.Add(item);
				}
			}
		}

		/// <summary>
		/// Modify name for new element
		/// </summary>
		/// <param name="newElement">New element</param>
		private void ModifyNameForNewElement(XElement newElement)
		{
			// Get name attribute and set new value in element
			foreach (var attribute in newElement.Attributes())
			{
				if (attribute.Name.LocalName == InvoiceConstants.CONST_XML_ATTRIBUTE_NAME_NAME)
				{
					var newName = string.Format("{0}{1}", attribute.Value, Guid.NewGuid())
						.Replace("-", string.Empty);
					attribute.SetValue(newName);
					break;
				}
			}
		}

		/// <summary>
		/// Calculate left style value
		/// </summary>
		/// <param name="source">Source</param>
		/// <param name="settingWidth">Setting width</param>
		/// <returns>Left value</returns>
		private decimal CalculateLeftStyleValue(XElement source, decimal? settingWidth)
		{
			var currentWidth = GetElementDecimalValueByName(source, InvoiceConstants.CONST_XML_ELEMENT_NAME_WIDTH);
			if (settingWidth.HasValue) currentWidth = settingWidth.Value;

			var left = InvoiceConstants.CONST_DEFAULT_CENTER_DISTANCE - (currentWidth / 2m);
			if (left < 0) left = 0m;

			return left;
		}

		/// <summary>
		/// Get element decimal value by name
		/// </summary>
		/// <param name="source">Source</param>
		/// <param name="name">Element name</param>
		/// <returns>A value has decimal type</returns>
		private decimal GetElementDecimalValueByName(XElement source, string name)
		{
			var widthString = GetElementValueOrDefaultEmpty(source, name);
			if (string.IsNullOrEmpty(widthString)) return 0m;

			var numberString = Regex.Match(widthString, @"\d+(\.\d+)?").Value;
			return InvoiceHelper.TryParseDecimal(numberString, 0m);
		}

		/// <summary>
		/// Get element int value by name
		/// </summary>
		/// <param name="source">Source</param>
		/// <param name="name">Element name</param>
		/// <returns>A value has int type</returns>
		private int GetElementIntValueByName(XElement source, string name)
		{
			var widthString = GetElementValueOrDefaultEmpty(source, name);
			if (string.IsNullOrEmpty(widthString)) return 0;

			var numberString = Regex.Match(widthString, @"\d+").Value;
			return InvoiceHelper.TryParseInt(numberString, 0);
		}

		/// <summary>
		/// Get element value or default empty
		/// </summary>
		/// <param name="source">Source</param>
		/// <param name="name">Element name</param>
		/// <returns>Element value or default empty if element doesn't exist</returns>
		private string GetElementValueOrDefaultEmpty(XElement source, string name)
		{
			var element = source.GetElement(name);
			return (element != null) ? element.Value : string.Empty;
		}

		/// <summary>
		/// Get Textbox elements
		/// </summary>
		/// <param name="parentElement">Parent element</param>
		/// <returns>Textbox elements</returns>
		private XElement[] GetTextboxElements(XElement parentElement)
		{
			var textboxElements = parentElement.GetElements(InvoiceConstants.CONST_XML_ELEMENT_NAME_TEXTBOX)
				.Where(node => (node.IsHidden() == false))
				.ToArray();
			return textboxElements;
		}

		/// <summary>
		/// Get TextRun informations from setting
		/// </summary>
		/// <param name="blockSetting">Block setting</param>
		/// <param name="value">Value</param>
		/// <returns>Replaced shop site setting lines</returns>
		private Dictionary<int, List<TextRunObject>> GetTextRunInformationsFromSetting(
			InvoiceBlockSetting blockSetting,
			string value)
		{
			// Get lines
			var lines = value.Split(InvoiceConstants.CONST_NEWLINE_CHARACTERS, StringSplitOptions.None);

			var results = new Dictionary<int, List<TextRunObject>>();
			var count = 0;
			foreach (var line in lines)
			{
				var tempLine = (string)line.Trim().Clone();
				var tagCount = 0;

				// Get shop site tags from line
				var shopSiteTags = new Dictionary<string, string>();
				var matchShopMessageTagResults = Regex.Matches(line, ShopMessageUtil.FORMAT_SHOP_MESSAGE_TAG_WITH_SPACE);
				foreach (Match m in matchShopMessageTagResults)
				{
					var newTag = string.Format(InvoiceConstants.CONST_FORMAT_NEW_TAG, tagCount);
					var key = string.Format(InvoiceConstants.CONST_FORMAT_TAG, tagCount);
					tempLine = tempLine.Replace(m.Value, newTag);
					shopSiteTags.Add(key, m.Value);
					tagCount++;
				}

				// Get field tags from line
				var fieldTags = new Dictionary<string, string>();
				var matchFieldTagResults = Regex.Matches(line, InvoiceConstants.CONST_FORMAT_PATTERN_FIELD_TAG);
				foreach (Match m in matchFieldTagResults)
				{
					var newTag = string.Format(InvoiceConstants.CONST_FORMAT_NEW_TAG, tagCount);
					var key = string.Format(InvoiceConstants.CONST_FORMAT_TAG, tagCount);
					tempLine = tempLine.Replace(m.Value, newTag);
					fieldTags.Add(key, m.Value);
					tagCount++;
				}

				// Get urls from line
				var urlInformations = new Dictionary<string, string>();
				var matchUrlResults = Regex.Matches(line, InvoiceConstants.CONST_FORMAT_PATTERN_URL);
				foreach (Match m in matchUrlResults)
				{
					var newTag = string.Format(InvoiceConstants.CONST_FORMAT_NEW_TAG, tagCount);
					var key = string.Format(InvoiceConstants.CONST_FORMAT_TAG, tagCount);
					tempLine = tempLine.Replace(m.Value, newTag);
					urlInformations.Add(key, m.Value);
					tagCount++;
				}

				// Create TextRuns
				var textRuns = new List<TextRunObject>();
				foreach (var item in tempLine.Split(InvoiceConstants.CONST_TAB_DELIMITER))
				{
					if (string.IsNullOrEmpty(item.Trim())) continue;

					// For case the line has set shop site tag
					// Get texts to create TextRun for the shop site
					if (shopSiteTags.ContainsKey(item))
					{
						// Get replaced text shop site
						var replacedText = ShopMessageUtil.ConvertShopMessage(
							shopSiteTags[item],
							blockSetting.LanguageCode,
							blockSetting.LanguageLocaleId,
							false);

						// For case the replaced text has break line
						var replacedTexts = replacedText.Split(InvoiceConstants.CONST_NEWLINE_CHARACTERS, StringSplitOptions.RemoveEmptyEntries)
							.ToArray();
						var index = 0;
						foreach (var text in replacedTexts)
						{
							// For case this item is not last item, use this to create new line in file invoice
							if (index < (replacedTexts.Length - 1))
							{
								results.Add(
									count,
									GetTextRunsForShopSite(text));
								count++;
							}
							else
							{
								textRuns.AddRange(GetTextRunsForShopSite(text));
							}

							index++;
						}
					}
					// For case the line has set field tag
					// Get texts to create TextRun for the field
					else if (fieldTags.ContainsKey(item))
					{
						var fieldName = fieldTags[item].TrimAllSpaces().Replace("@@", string.Empty);
						var fieldValue = string.Format(
							InvoiceConstants.CONST_FORMAT_FIELD_VALUE,
							fieldName);
						textRuns.Add(
							new TextRunObject
							{
								Text = value.Contains(InvoiceConstants.CONST_FIELD_VALUE_NEXT_SHIPPING_DATE) ?
								string.Format(
									InvoiceConstants.CONDITIONAL_EXPRESSION_FOR_NEXT_SHIPPING_DATE,
									"\"\"",
									fieldValue.Substring(1)): fieldValue,
								FieldName = fieldName,
							});
					}
					// For case the line has url
					// Get texts to create TextRun for the url
					else if (urlInformations.ContainsKey(item))
					{
						var urlValue = urlInformations[item];
						textRuns.Add(
							new TextRunObject
							{
								Text = urlValue,
								IsUrl = true,
							});
					}
					else
					{
						textRuns.Add(
							new TextRunObject
							{
								Text = value.Contains(InvoiceConstants.CONST_FIELD_VALUE_NEXT_SHIPPING_DATE) ?
								string.Format(
									InvoiceConstants.CONDITIONAL_EXPRESSION_FOR_NEXT_SHIPPING_DATE,
									"\"\"",
									string.Format("\"{0}\"", item)): item,
							});
					}
				}

				results.Add(count, textRuns);
				count++;
			}

			return results;
		}

		/// <summary>
		/// Get TextRuns for shop site
		/// </summary>
		/// <param name="text">Text</param>
		/// <returns>TextRuns for shop site</returns>
		private List<TextRunObject> GetTextRunsForShopSite(string text)
		{
			var count = 0;
			var tempText = (string)text.Clone();
			var results = new List<TextRunObject>();

			// Get urls from text
			var urlInformations = new Dictionary<string, string>();
			var matchUrlResults = Regex.Matches(text, InvoiceConstants.CONST_FORMAT_PATTERN_URL);
			foreach (Match m in matchUrlResults)
			{
				var newTag = string.Format(InvoiceConstants.CONST_FORMAT_NEW_TAG, count);
				var key = string.Format(InvoiceConstants.CONST_FORMAT_TAG, count);
				tempText = tempText.Replace(m.Value, newTag);
				urlInformations.Add(key, m.Value);
				count++;
			}

			// Get texts to create TextRun
			foreach (var item in tempText.Split(InvoiceConstants.CONST_TAB_DELIMITER))
			{
				if (string.IsNullOrEmpty(item.Trim())) continue;

				if (urlInformations.ContainsKey(item))
				{
					var urlValue = urlInformations[item];
					results.Add(
						new TextRunObject
						{
							Text = urlValue,
							IsShopSiteTag = true,
							IsUrl = true,
						});
				}
				else
				{
					results.Add(
						new TextRunObject
						{
							Text = item,
							IsShopSiteTag = true,
							IsUrl = false,
						});
				}
			}

			return results;
		}

		/// <summary>
		/// Load settings
		/// </summary>
		/// <param name="settingFilePath">Setting file path</param>
		private void LoadSettings(string settingFilePath)
		{
			try
			{
				var document = XDocument.Load(settingFilePath);
				this.BlockSettings = document.Root.Elements(InvoiceConstants.CONST_XML_ELEMENT_NAME_BLOCK)
					.Select(elment => new InvoiceBlockSetting(elment))
					.ToArray();
				this.LanguageCode = string.Empty;
			}
			catch (Exception ex)
			{
				throw new ApplicationException("ファイル「" + settingFilePath + "」の読み込みに失敗しました。", ex);
			}
		}

		/// <summary>
		/// Convert image to base64 string
		/// </summary>
		/// <param name="imagePath">Image path</param>
		/// <returns>Inage as base64 string</returns>
		private string ConvertImageToBase64String(string imagePath)
		{
			if (string.IsNullOrEmpty(imagePath)
				|| (File.Exists(imagePath) == false)) return string.Empty;

			var imageBytes = File.ReadAllBytes(imagePath);
			var imageString = Convert.ToBase64String(imageBytes);
			return imageString;
		}

		/// <summary>
		/// Get block setting with language code
		/// </summary>
		/// <param name="blockSettings">Block settings</param>
		/// <returns>A block setting with language code</returns>
		private InvoiceBlockSetting GetBlockSettingWithLanguageCode(InvoiceBlockSetting[] blockSettings)
		{
			if (Constants.GLOBAL_OPTION_ENABLE && (string.IsNullOrEmpty(this.LanguageCode) == false))
			{
				var result = blockSettings.FirstOrDefault(blockSetting => (blockSetting.LanguageCode == this.LanguageCode));
				if (result != null) return result;
			}

			return blockSettings.FirstOrDefault(blockSetting => string.IsNullOrEmpty(blockSetting.LanguageCode));
		}

		/// <summary>Block settings</summary>
		private InvoiceBlockSetting[] BlockSettings { get; set; }
		/// <summary>Title block setting</summary>
		private InvoiceBlockSetting TitleBlockSetting
		{
			get { return GetBlockSettingWithLanguageCode(this.BlockSettings.Where(item => item.IsTitle).ToArray()); }
		}
		/// <summary>Body header block setting</summary>
		private InvoiceBlockSetting BodyHeaderBlockSetting
		{
			get { return GetBlockSettingWithLanguageCode(this.BlockSettings.Where(item => item.IsBodyHeader).ToArray()); }
		}
		/// <summary>Detail block setting</summary>
		private InvoiceBlockSetting DetailBlockSetting
		{
			get { return GetBlockSettingWithLanguageCode(this.BlockSettings.Where(item => item.IsDetail).ToArray()); }
		}
		/// <summary>Product block setting</summary>
		private InvoiceBlockSetting ProductBlockSetting
		{
			get { return GetBlockSettingWithLanguageCode(this.BlockSettings.Where(item => item.IsProduct).ToArray()); }
		}
		/// <summary>Price block setting</summary>
		private InvoiceBlockSetting PriceBlockSetting
		{
			get { return GetBlockSettingWithLanguageCode(this.BlockSettings.Where(item => item.IsPrice).ToArray()); }
		}
		/// <summary>Footer block setting</summary>
		private InvoiceBlockSetting FooterBlockSetting
		{
			get { return GetBlockSettingWithLanguageCode(this.BlockSettings.Where(item => item.IsFooter).ToArray()); }
		}
		/// <summary>Tag shop messages</summary>
		private List<KeyValuePair<string, string>> TagShopMessages { get; set; }
		/// <summary>Language code</summary>
		public string LanguageCode { get; set; }
		/// <summary>Top調整倍率</summary>
		public int TopAdjustmentMagnification { get; set; }
		/// <summary>明細ブロックの「次回配送予定日」の下に配置するか</summary>
		public bool IsUnderLocationNextShippingDate { get; set; }
	}
}
