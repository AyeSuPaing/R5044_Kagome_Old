/*
=========================================================================================================
  Module      : TempoStar setting(TempoStarSetting.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.IO;
using System.Xml;
using w2.App.Common;
using w2.Common.Logger;

namespace TempoStar.w2.Commerce.ExternalAPI
{
	/// <summary>
	/// Tempo star setting class
	/// </summary>
	public static class TempoStarSetting
	{
		#region GetSettingFromXml
		/// <summary>
		/// Get setting from xml
		/// </summary>
		/// <param name="node">Node</param>
		/// <returns>Setting value</returns>
		public static string GetSettingFromXml(string node)
		{
			var valueSetting = string.Empty;
			try
			{
				var filePath = Path.Combine(Constants.PHYSICALDIRPATH_EXTERNALAPI_STORAGE_SETTING_LOCATION, "TempoStarSettings.xml");
				var xmlDoc = new XmlDocument();
				xmlDoc.Load(filePath);

				valueSetting = xmlDoc.SelectSingleNode("Settings/TempostarFtp/" + node).InnerText;
			}
			catch (Exception exception)
			{
				FileLogger.WriteError(exception);
			}

			return valueSetting;
		}
		#endregion
	}
}