/*
=========================================================================================================
  Module      : Invoice Helper(InvoiceHelper.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using System.Xml.Linq;

namespace w2.App.Common.Pdf.Settings.Invoice
{
	/// <summary>
	/// Invoice Helper
	/// </summary>
	internal static class InvoiceHelper
	{
		/// <summary>
		/// Is hidden element
		/// </summary>
		/// <param name="parentElement">Parent element</param>
		/// <param name="name">Name</param>
		/// <returns>True if element exists</returns>
		public static bool HasElement(this XElement parentElement, string name)
		{
			var element = parentElement.GetElement(name);
			return (element != null);
		}

		/// <summary>
		/// Is hidden
		/// </summary>
		/// <param name="parentElement">Parent element</param>
		/// <returns>True if this element contains the hidden element</returns>
		public static bool IsHidden(this XElement parentElement)
		{
			var visibilityElement =
				parentElement.Element(parentElement.GetDefaultNamespace() + InvoiceConstants.CONST_XML_ELEMENT_NAME_VISIBILITY);
			if (visibilityElement == null) return false;

			var hiddenElement = visibilityElement.GetFirstElement(InvoiceConstants.CONST_XML_ELEMENT_NAME_HIDDEN);
			var isHidden = ((hiddenElement != null)
				&& TryParseBool(hiddenElement.Value));
			return isHidden;
		}

		/// <summary>
		/// Get first element
		/// </summary>
		/// <param name="parentElement">Parent element</param>
		/// <param name="name">Element name</param>
		/// <returns>Element</returns>
		public static XElement GetFirstElement(this XElement parentElement, string name)
		{
			var firstElement = parentElement.GetElements(name)
				.FirstOrDefault();
			return firstElement;
		}

		/// <summary>
		/// Get element
		/// </summary>
		/// <param name="parentElement">Parent element</param>
		/// <param name="name">Element name</param>
		/// <returns>Element</returns>
		public static XElement GetElement(this XElement parentElement, string name)
		{
			var element = parentElement.Element(parentElement.GetDefaultNamespace() + name);
			return element;
		}

		/// <summary>
		/// Get elements
		/// </summary>
		/// <param name="parentElement">Parent element</param>
		/// <param name="name">Element name</param>
		/// <returns>Elements</returns>
		public static XElement[] GetElements(this XElement parentElement, string name)
		{
			if (parentElement == null) return new XElement[0];

			var elements = parentElement
				.Descendants(parentElement.GetDefaultNamespace() + name)
				.ToArray();
			return elements;
		}

		/// <summary>
		/// Try parse string to boolean
		/// </summary>
		/// <param name="value">The string value</param>
		/// <returns>A boolean value</returns>
		public static bool TryParseBool(string value)
		{
			bool result;
			if (bool.TryParse(value, out result) == false) return false;

			return result;
		}

		/// <summary>
		/// Try parse string to int
		/// </summary>
		/// <param name="value">The string value</param>
		/// <param name="defaultValue">The default value</param>
		/// <returns>An int value</returns>
		public static int TryParseInt(string value, int defaultValue)
		{
			int result;
			if (int.TryParse(value, out result) == false) return defaultValue;

			return result;
		}

		/// <summary>
		/// Try parse string to decimal
		/// </summary>
		/// <param name="value">The string value</param>
		/// <param name="defaultValue">The default value</param>
		/// <returns>A decimal value</returns>
		public static decimal TryParseDecimal(string value, decimal defaultValue)
		{
			decimal result;
			if (decimal.TryParse(value, out result) == false) return defaultValue;

			return result;
		}
	}
}
