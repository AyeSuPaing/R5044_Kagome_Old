/*
=========================================================================================================
  Module      : 初期化 (Initializer.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using w2.App.Common;

namespace w2.Commerce.Batch.FlapsIntegration
{
	/// <summary>
	/// Initialize
	/// </summary>
	public class Initializer
	{
		/// <summary>
		/// Read Common Setting
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
				ConfigurationSetting.ReadKbn.C200_TwInvoice);
		}
	}
}