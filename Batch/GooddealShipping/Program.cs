/*
=========================================================================================================
  Module      : Program (Program.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using w2.App.Common;
using w2.App.Common.Util;
using w2.Commerce.Batch.GooddealShipping.Action;
using w2.Common.Logger;

namespace w2.Commerce.Batch.GooddealShipping
{
	/// <summary>
	/// Program
	/// </summary>
	class Program
	{
		/// <summary>
		/// Main thread
		/// </summary>
		[STAThread]
		private static void Main()
		{
			try
			{
				Initializer.ReadCommonConfig();
				FileLogger.WriteInfo("起動");

				if (Constants.TWSHIPPING_GOODDEAL_OPTION_ENABLED == false) return;

				var executeSuccess = ProcessUtility.ExcecWithProcessMutex(MainExecute);
				if (executeSuccess == false)
				{
					throw new Exception("他プロセスが起動しているため、起動に失敗しました。二重起動は禁止されています。");
				}

				FileLogger.WriteInfo("正常終了");
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
			}
		}

		/// <summary>
		/// Main execute
		/// </summary>
		private static void MainExecute()
		{
			foreach (var action in CreateActions())
			{
				action.Execute();
			}
		}

		/// <summary>
		/// Create actions
		/// </summary>
		/// <returns>A collection of executable action</returns>
		private static IEnumerable<IAction> CreateActions()
		{
			yield return new GooddealShippingAction();
		}
	}
}