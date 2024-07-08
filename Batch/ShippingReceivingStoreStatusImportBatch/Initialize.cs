/*
=========================================================================================================
  Module      : 初期化 (Initialize.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using w2.App.Common;

namespace w2.Commerce.Batch.ShippingReceivingStoreStatusImportBatch
{
	/// <summary>
	/// Initialize
	/// </summary>
	public class Initialize
	{
		/// <summary>
		/// Read Setting
		/// </summary>
		public static void ReadCommonConfig()
		{
			// Application Name
			Constants.APPLICATION_NAME = Properties.Settings.Default.ApplicationName;

			// Read Customer Resource Setting
			var setting = new ConfigurationSetting(
				Properties.Settings.Default.ConfigFileDirPath,
				ConfigurationSetting.ReadKbn.C000_AppCommon,
				ConfigurationSetting.ReadKbn.C100_BatchCommon,
				ConfigurationSetting.ReadKbn.C200_ShippingReceivingStoreStatusImportBatch);

			// Get physical path value text
			Constants.PHYSICALFILEPATH_VALUETEXT = Constants.PHYSICALDIRPATH_COMMERCE_MANAGER + Constants.FILEPATH_XML_VALUE_TEXT.Replace("/", @"\");
			Constants.SHIPPINGRECEIVINGSTORESTATUSIMPORTBATCH_TEMP_DIR_PATH = setting.GetAppStringSetting("ShippingReceivingStoreStatusImportBatch_Temp_Dir_Path");
		}
	}
}