/*
=========================================================================================================
  Module      : Setting Base(SettingBase.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using System.Xml.Linq;

namespace w2.App.Common.Pdf.Settings.Invoice
{
	/// <summary>
	/// Setting Base
	/// </summary>
	internal abstract class SettingBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="element">Element</param>
		protected SettingBase(XElement element)
		{
			InitializeProperties();

			SetProperties(element);
		}

		/// <summary>
		/// Initialize properties
		/// </summary>
		protected abstract void InitializeProperties();

		/// <summary>
		/// Set properties
		/// </summary>
		/// <param name="element">Element</param>
		protected abstract void SetProperties(XElement element);

		/// <summary>
		/// Get first element
		/// </summary>
		/// <param name="element">Element</param>
		/// <param name="name">Element name</param>
		/// <returns>Firt element</returns>
		protected XElement GetFirstElement(XElement element, string name)
		{
			var result = element.Elements(name).FirstOrDefault();
			return result;
		}

		/// <summary>
		/// Get attribute value
		/// </summary>
		/// <param name="element">Element</param>
		/// <param name="name">Attribute name</param>
		/// <returns>Attribute value</returns>
		protected string GetAttributeValue(XElement element, string name)
		{
			var attribute = element.Attribute(name);
			if (attribute == null) return string.Empty;

			return attribute.Value.Trim();
		}
	}
}
