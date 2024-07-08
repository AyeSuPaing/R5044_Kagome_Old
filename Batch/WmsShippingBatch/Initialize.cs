/*
=========================================================================================================
  Module      : Initialize (Initialize.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using w2.App.Common;

namespace w2.Commerce.Batch.WmsShippingBatch
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
			// Application name
			Constants.APPLICATION_NAME = Properties.Settings.Default.ApplicationName;

			// Read customer resource setting
			var setting = new ConfigurationSetting(
				Properties.Settings.Default.ConfigFileDirPath,
				ConfigurationSetting.ReadKbn.C000_AppCommon,
				ConfigurationSetting.ReadKbn.C100_BatchCommon,
				ConfigurationSetting.ReadKbn.C200_WmsShippingBatch);

			// Set directory path
			Constants.DIR_PATH_WAITING_FOR_PROCESSING = Path.Combine(
				AppDomain.CurrentDomain.BaseDirectory,
				@"処理待ち\");
			Constants.DIR_PATH_UPLOADING = Path.Combine(
				AppDomain.CurrentDomain.BaseDirectory,
				@"アップロード中");
			Constants.DIR_PATH_UPLOADED = Path.Combine(
				AppDomain.CurrentDomain.BaseDirectory,
				@"処理済\");
			Constants.DIR_PATH_WAIT_DOWNLOAD_FILE = Path.Combine(
				AppDomain.CurrentDomain.BaseDirectory,
				@"ダウンロードファイル作成待ち\");
			Constants.DIR_PATH_DOWNLOADED_FILE = Path.Combine(
				AppDomain.CurrentDomain.BaseDirectory,
				@"ダウンロード済\");
			Constants.PATH_XML_WMS_MESSAGES = Path.Combine(
				AppDomain.CurrentDomain.BaseDirectory,
				"Xml", "WmsMessages.xml");
		}
	}
}
