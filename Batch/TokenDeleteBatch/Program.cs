/*
=========================================================================================================
  Module      : Program (Program.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.App.Common.Util;
using w2.Common.Logger;

namespace w2.Commerce.Batch.TokenDeleteBatch
{
	/// <summary>
	/// Program Class
	/// </summary>
	class Program
	{
		#region +MainProcess
		/// <summary>
		/// Main Thread
		/// </summary>
		private static void Main()
		{
			try
			{
				var program = new Program();

				FileLogger.WriteInfo("起動");

				// Excute Single Instance
				var executeSuccess = ProcessUtility.ExcecWithProcessMutex(program.Start);
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
		#endregion

		#region +Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		private Program()
		{
			// Read Common Setting
			Initialize.ReadCommonConfig();
		}
		#endregion

		#region +StartProcess
		/// <summary>
		/// Start Process
		/// </summary>
		private void Start()
		{
			new TokenDeleteCommand(Constants.PAYMENT_PAIDY_TOKEN_DELETE_LIMIT_DAY).Exec();
		}
		#endregion
	}
}