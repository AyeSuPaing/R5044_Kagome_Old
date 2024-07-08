/*
=========================================================================================================
  Module      : Program (Program.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.App.Common;
using w2.App.Common.Util;
using w2.Commerce.Batch.ShippingReceivingStoreSettingImportBatch.Action;
using w2.Common.Logger;
using w2.ExternalAPI.Common.Ftp;

namespace w2.Commerce.Batch.ShippingReceivingStoreSettingImportBatch
{
	/// <summary>
	/// Program
	/// </summary>
	class Program
	{
		/// <summary>
		/// Main Thread
		/// </summary>
		[STAThread]
		private static void Main()
		{
			try
			{
				// Read Common Setting
				Initialize.ReadCommonConfig();

				FileLogger.WriteInfo("起動");

				// Excute Single Instance
				var executeSuccess = ProcessUtility.ExcecWithProcessMutex(() => MainExecute());
				if (executeSuccess == false)
				{
					throw new Exception("他プロセスが起動しているため、起動に失敗しました。二重起動は禁止されています。");
				}

				FileLogger.WriteInfo("正常終了");
			}
			catch (Exception ex)
			{
				// Write Log
				FileLogger.WriteError(ex);
			}
		}

		/// <summary>
		/// Main Execute
		/// </summary>
		private static void MainExecute()
		{
			// Setting dependencies
			var ftp = new FluentFtpUtility(
				Constants.TWPELICANEXPRESS_FTP_HOST,
				Constants.TWPELICANEXPRESS_FTP_ID,
				Constants.TWPELICANEXPRESS_FTP_PW,
				Constants.TWPELICANEXPRESS_FTP_ENABLE_SSL,
				true,
				Constants.TWPELICANEXPRESS_FTP_PORT);

			// On Execute
			new ShippingReceivingStoreSettingImportAction(ftp).OnExecute();
		}
	}
}