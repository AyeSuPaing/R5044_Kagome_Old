/*
=========================================================================================================
  Module      : Program (Program.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using w2.App.Common;
using w2.App.Common.Util;
using w2.Commerce.Batch.CrossPointCooperation.Commands;
using w2.Common.Logger;

namespace w2.Commerce.Batch.CrossPointCooperation
{
	class Program
	{
		/// <summary>
		/// Constructor
		/// </summary>
		private Program()
		{
			// Read settings
			ReadCommonConfig();
		}

		/// <summary>
		/// Main thread
		/// </summary>
		[STAThread]
		private static void Main()
		{
			try
			{
				var program = new Program();
				FileLogger.WriteInfo("起動");

				var executeSuccess = ProcessUtility.ExcecWithProcessMutex(program.Start);
				if (executeSuccess == false)
				{
					throw new Exception("他プロセスが起動しているため、起動に失敗しました。二重起動は禁止されています。");
				}

				FileLogger.WriteInfo("正常終了");
			}
			catch (Exception exception)
			{
				FileLogger.WriteError(exception);
			}
		}

		/// <summary>
		/// Start
		/// </summary>
		private void Start()
		{
			foreach (var command in CreateCommands())
			{
				command.Execute();
			}
		}

		/// <summary>
		/// Create commands
		/// </summary>
		/// <returns>Commands</returns>
		private IEnumerable<MemberMasterImportCommand> CreateCommands()
		{
			yield return new MemberMasterImportCommand();
		}

		/// <summary>
		/// Read common setting
		/// </summary>
		public static void ReadCommonConfig()
		{
			Constants.APPLICATION_NAME = Properties.Settings.Default.ApplicationName;

			// Read Resource Setting
			var setting = new ConfigurationSetting(
				Properties.Settings.Default.ConfigFileDirPath,
				ConfigurationSetting.ReadKbn.C000_AppCommon,
				ConfigurationSetting.ReadKbn.C100_BatchCommon);
		}
	}
}