/*
===============================================================================================================
  Module      : DataExporter for ZipCodes of P0053_Haier_0(DataExporterSettingManager_P0053_Haier_0_ZipCode.cs)
 ･･････････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
===============================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using w2.Common.Logger;
using w2.Common.Util;

namespace w2.App.Common.DataExporters.SettingManager
{
	public class DataExporterSettingManager_P0053_Haier_0_ZipCode
	{
		/// <summary>ZipCode</summary>
		private static List<string> ZipCodes = new List<string>();
		/// <summary>ZipCode file</summary>
		private static string YHC_DELIVERY_ZIPCODE_FILE = "YHCDeliveryZipcode.xml";
		/// <summary>ZipCode dir</summary>
		private static string YHC_DELIVERY_ZIPCODE_DIR = string.Format("{0}{1}", Constants.PHYSICALDIRPATH_FRONT_PC, @"Contents\Settings\");

		#region Initialize
		/// <summary>
		/// Constructor
		/// </summary>
		static DataExporterSettingManager_P0053_Haier_0_ZipCode()
		{
			// Initialize
			InitializeZipCode();

			// Update file observer
			FileUpdateObserver.GetInstance().AddObservation(
				YHC_DELIVERY_ZIPCODE_DIR,
				YHC_DELIVERY_ZIPCODE_FILE,
				InitializeZipCode);
		}

		/// <summary>
		/// Initialize ZipCode
		/// </summary>
		private static void InitializeZipCode()
		{
			// setting file Path
			var filePath = YHC_DELIVERY_ZIPCODE_DIR + YHC_DELIVERY_ZIPCODE_FILE;

			if (File.Exists(filePath) == false) return;

			try
			{
				ZipCodes = new List<string>();

				XmlDocument setting = new XmlDocument();
				setting.Load(filePath);

				XmlNodeList items = setting.SelectSingleNode("CheckZipcode").SelectNodes("zipcode");
				foreach (XmlNode xnItem in items)
				{
					ZipCodes.Add(xnItem.InnerText);
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
		/// Get ZipCodes
		/// </summary>
		/// <returns>ZipCodes</returns>
		public static List<string> GetZipCodes()
		{
			return ZipCodes;
		}
		#endregion
	}
}
