/*
=========================================================================================================
  Module      : Initializer (Initializer.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common;

namespace w2.Commerce.Batch.GooddealGetShippingCheckNo
{
	/// <summary>
	/// Initializer
	/// </summary>
	public static class Initializer
	{
		/// <summary>
		/// Read common setting
		/// </summary>
		public static void ReadCommonConfig()
		{
			Constants.APPLICATION_NAME = Properties.Settings.Default.ApplicationName;
			// Read resource setting
			var setting = new ConfigurationSetting(
				Properties.Settings.Default.ConfigFileDirPath,
				ConfigurationSetting.ReadKbn.C000_AppCommon,
				ConfigurationSetting.ReadKbn.C100_BatchCommon,
				ConfigurationSetting.ReadKbn.C200_GooddealGetShippingCheckNo,
				ConfigurationSetting.ReadKbn.C200_CommonManager);
		}
	}
}
