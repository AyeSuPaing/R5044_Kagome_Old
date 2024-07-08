/*
=========================================================================================================
  Module      : 初期化 (Initialize.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common;

namespace w2.Commerce.Batch.PageDesignConsistency
{
	/// <summary>
	/// 初期化
	/// </summary>
	public class Initialize
	{
		/// <summary>
		/// 設定読み込み
		/// </summary>
		public static void ReadCommonConfig()
		{
			Constants.APPLICATION_NAME = Properties.Settings.Default.ApplicationName;

			new ConfigurationSetting(
				Properties.Settings.Default.ConfigFileDirPath,
				ConfigurationSetting.ReadKbn.C000_AppCommon,
				ConfigurationSetting.ReadKbn.C100_BatchCommon,
				ConfigurationSetting.ReadKbn.C200_PageDesignConsistency);
		}
	}
}