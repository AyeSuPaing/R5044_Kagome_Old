/*
=================================================================================================================
  Module      : DataExporter for Resource of P0053_Haier_0(DataExporterSettingManager_P0053_Haier_0_Resource.cs)
 ････････････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=================================================================================================================
*/
using System;
using System.IO;
using w2.Common.Logger;
using w2.Common.Util;

namespace w2.App.Common.DataExporters.SettingManager
{
	public class DataExporterSettingManager_P0053_Haier_0_Resource
	{
		/// <summary>Resource</summary>
		private static string Resource = string.Empty;
		/// <summary>Resource file</summary>
		private static string RESOURCE_FILE = "P0053_Haier_0.xml";
		/// <summary>Resource dir</summary>
		private static string RESOURCE_DIR = Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE + @"/DataExport/";

		#region Initialize
		/// <summary>
		/// Constructor
		/// </summary>
		static DataExporterSettingManager_P0053_Haier_0_Resource()
		{
			// Initialize
			InitializeResource();

			// Update file observer
			FileUpdateObserver.GetInstance().AddObservation(
				RESOURCE_DIR,
				RESOURCE_FILE,
				InitializeResource);
		}

		/// <summary>
		/// Initialize resource
		/// </summary>
		private static void InitializeResource()
		{
			// Setting file path
			var filePath = RESOURCE_DIR + RESOURCE_FILE;

			if (File.Exists(filePath) == false) return;

			try
			{
				using (StreamReader reader = File.OpenText(filePath))
				{
					Resource = reader.ReadToEnd();
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
		/// Get resource
		/// </summary>
		/// <returns>Resource</returns>
		public static string GetResource()
		{
			return Resource;
		}
		#endregion
	}
}
