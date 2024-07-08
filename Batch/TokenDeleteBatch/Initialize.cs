/*
=========================================================================================================
  Module      : 初期化 (Initialize.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common;

namespace w2.Commerce.Batch.TokenDeleteBatch
{
	/// <summary>
	/// Initialize Class
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
				ConfigurationSetting.ReadKbn.C200_TokenDeleteBatch);

			// Payment Paidy Token Delete Limit Day
			Constants.PAYMENT_PAIDY_TOKEN_DELETE_LIMIT_DAY = setting.GetAppIntSetting("Payment_Paidy_TokenDeleteLimitDay");
		}
	}
}