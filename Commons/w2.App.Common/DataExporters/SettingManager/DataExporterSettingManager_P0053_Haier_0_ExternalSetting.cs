using System;
/*
=============================================================================================================================
  Module      : DataExporter for ExternaSetting of P0053_Haier_0(DataExporterSettingManager_P0053_Haier_0_ExternalSetting.cs)
 ････････････････････････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=============================================================================================================================
*/
using System.Collections;
using System.IO;
using System.Xml;
using w2.Common.Logger;
using w2.Common.Util;

namespace w2.App.Common.DataExporters.SettingManager
{
	public class DataExporterSettingManager_P0053_Haier_0_ExternalSetting
	{
		/// <summary>External setting</summary>
		private static Hashtable ExternalSetting = new Hashtable();
		/// <summary>External setting file</summary>
		private static string YHC_EXTERNAL_SETTING_FILE = "YHCExternalSetting.xml";
		/// <summary>External setting dir</summary>
		private static string YHC_EXTERNAL_SETTING_DIR = string.Format("{0}{1}", Constants.PHYSICALDIRPATH_FRONT_PC, @"Contents\Settings\");

		#region Initialize
		/// <summary>
		/// Constructor
		/// </summary>
		static DataExporterSettingManager_P0053_Haier_0_ExternalSetting()
		{
			// Initialize
			InitializeExternalSetting();

			// Update file observer
			FileUpdateObserver.GetInstance().AddObservation(
				YHC_EXTERNAL_SETTING_DIR,
				YHC_EXTERNAL_SETTING_FILE,
				InitializeExternalSetting);
		}

		/// <summary>
		/// Initialize external setting
		/// </summary>
		private static void InitializeExternalSetting()
		{
			// Setting file path
			string filePath = YHC_EXTERNAL_SETTING_DIR + YHC_EXTERNAL_SETTING_FILE;

			if (File.Exists(filePath) == false) return;

			try
			{
				ExternalSetting = new Hashtable();

				XmlDocument setting = new XmlDocument();
				setting.Load(filePath);

				XmlNodeList items = setting.SelectSingleNode("YHCExternalSetting").ChildNodes;
				foreach (XmlNode xnItem in items)
				{
					ExternalSetting[xnItem.Name] = xnItem.InnerText;
				}
			}
			catch (Exception exception)
			{
				FileLogger.WriteError(exception);
			}
		}
		#endregion

		#region Public Function
		/// <summary>
		/// Get external setting
		/// </summary>
		/// <param name="key">Key</param>
		/// <returns>External setting</returns>
		public static string GetExternalSetting(string key)
		{
			return StringUtility.ToEmpty(ExternalSetting[key]);
		}
		#endregion
	}
}
