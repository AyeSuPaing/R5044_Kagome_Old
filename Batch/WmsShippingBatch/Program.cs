/*
=========================================================================================================
  Module      : Program (Program.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using w2.Commerce.Batch.WmsShippingBatch.Action;
using w2.Common.Logger;

namespace w2.Commerce.Batch.WmsShippingBatch
{
	/// <summary>
	/// Program
	/// </summary>
	class Program
	{
		/// <summary>
		/// Main
		/// </summary>
		/// <param name="args">Execute parameter</param>
		private static void Main(string[] args)
		{
			try
			{
				// Read common setting
				Initialize.ReadCommonConfig();

				FileLogger.WriteInfo("起動");

				var actionTpye = (args.Length != 0) ? args[0] : string.Empty;
				MainExecute(actionTpye);

				FileLogger.WriteInfo("正常終了");
			}
			catch (Exception ex)
			{
				// Write log error
				FileLogger.WriteError(ex);
			}
		}

		/// <summary>
		/// Main execute
		/// </summary>
		/// <param name="actionType">Action Type</param>
		private static void MainExecute(string actionType)
		{
			// Create actions
			var actions = CreateActions(actionType);

			foreach (var action in actions)
			{
				try
				{
					if (Constants.ELOGIT_WMS_ENABLED) action.Execute();
				}
				catch (Exception ex)
				{
					FileLogger.WriteError(ex);
				}
			}
		}

		/// <summary>
		/// Create actions
		/// </summary>
		/// <param name="action">Action</param>
		/// <returns>A collection of executable action</returns>
		private static IEnumerable<ActionBase> CreateActions(string action)
		{
			switch (action)
			{
				case Constants.FLG_TASKSCHEDULE_ACTION_KBN_ELOGIT_UPLOAD:
					yield return new FileUploadCooperationAction();
					break;

				case Constants.FLG_TASKSCHEDULE_ACTION_KBN_ELOGIT_DOWNLOAD:
					yield return new FileDownloadCooperationAction();
					break;

				default:
					yield return new FileUploadCooperationAction();
					yield return new FileDownloadCooperationAction();
					break;
			}
		}
	}
}
