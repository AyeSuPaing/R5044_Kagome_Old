/*
=========================================================================================================
  Module      : 初期化クラス(Initializer.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

using w2.App.Common;

namespace UpdateSMSState
{
	/// <summary>
	/// 初期化クラス
	/// </summary>
	class Initializer
	{
		/// <summary>
		/// 基本設定初期化
		/// </summary>
		public static void Initialize()
		{
			Constants.APPLICATION_NAME = Properties.Settings.Default.ApplicationName;

			// Read Customer Resource Setting
			var csSetting = new ConfigurationSetting(
				Properties.Settings.Default.ConfigFileDirPath,
				ConfigurationSetting.ReadKbn.C000_AppCommon,
				ConfigurationSetting.ReadKbn.C100_BatchCommon,
				ConfigurationSetting.ReadKbn.C200_UpdateSMSState);
		}
	}
}
