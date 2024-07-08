/*
=========================================================================================================
  Module      : WmsShippingThread (WmsShippingThread.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Diagnostics;
using System.Threading;
using w2.Common.Logger;

namespace w2.MarketingPlanner.Win.ScheduleManager.WorkerThread
{
	/// <summary>
	/// Wms shipping thread
	/// </summary>
	class WmsShippingThread : BaseThread
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="scheduleDate">Schedule date</param>
		/// <param name="deptId">Dept id</param>
		/// <param name="actionType">Action type</param>
		/// <param name="masterId">Master id</param>
		/// <param name="actionNo">Action no</param>
		public WmsShippingThread(DateTime scheduleDate, string deptId, string actionType, string masterId, int actionNo)
			: base(scheduleDate, deptId, actionType, masterId, actionNo)
		{

		}

		/// <summary>
		/// Thread creation (task schedule execution)
		/// </summary>
		/// <param name="scheduleDate">Schedule date</param>
		/// <param name="deptId">Dept id</param>
		/// <param name="actionType">Action type</param>
		/// <param name="masterId">Master id</param>
		/// <param name="actionNo">Action number</param>
		/// <returns>Spawn thread</returns>
		public static WmsShippingThread CreateAndStart(
			DateTime scheduleDate,
			string deptId,
			string actionType,
			string masterId,
			int actionNo)
		{
			// Thread creation
			var wmsShippingThread = new WmsShippingThread(scheduleDate, deptId, actionType, masterId, actionNo);
			wmsShippingThread.Thread = new Thread(new ThreadStart(wmsShippingThread.Work));

			// Start thread
			wmsShippingThread.Thread.Start();
			return wmsShippingThread;
		}

		/// <summary>
		/// Work
		/// </summary>
		public void Work()
		{
			try
			{
				Form1.WriteInfoLogLine(string.Format(
					"Wms shipping execution: [{0} - {1}] Start execution",
					this.MasterId,
					this.ActionKbn));

				// Update task status to start
				UpdateTaskStatusBegin(
					Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_EXECUTE,
					Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_INIT,
					string.Empty);

				// Run batch
				Process.Start(Constants.PHYSICALDIRPATH_WMSSHIPPING_EXE, this.ActionKbn);

				// Update task status to end
				UpdateTaskStatusEnd(
					Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_DONE,
					Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_EXECUTE,
					string.Empty);

				Form1.WriteInfoLogLine(string.Format("{0} Complete", this.ActionKbn));
			}
			catch (Exception exception)
			{
				FileLogger.WriteError(exception);
				Form1.WriteErrorLogLine(exception.ToString());

				// Update task status error
				UpdateTaskStatusEnd(
					Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_ERROR,
					Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_EXECUTE,
					string.Empty);
			}
		}
	}
}
