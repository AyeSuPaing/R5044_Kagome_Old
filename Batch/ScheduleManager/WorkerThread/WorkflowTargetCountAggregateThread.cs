/*
=========================================================================================================
  Module      : Workflow target count aggregate thread (WorkflowTargetCountAggregateThread.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Threading;
using w2.App.Common.Order.Workflow;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Domain.SummaryReport;
using w2.Domain.TaskSchedule;

namespace w2.MarketingPlanner.Win.ScheduleManager.WorkerThread
{
	/// <summary>
	/// Workflow target count aggregate thread
	/// </summary>
	class WorkflowTargetCountAggregateThread : BaseThread
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="scheduleDate">Schedule date</param>
		/// <param name="deptId">Dept id</param>
		/// <param name="masterId">Master id</param>
		/// <param name="actionNo">Action no</param>
		public WorkflowTargetCountAggregateThread(
			DateTime scheduleDate,
			string deptId,
			string masterId,
			int actionNo)
			: base(
				scheduleDate,
				deptId,
				Constants.FLG_TASKSCHEDULE_ACTION_KBN_WORKFLOW_TARGET_COUNT_AGGREGATE,
				masterId,
				actionNo)
		{
		}

		/// <summary>
		/// Create and start (task schedule execution)
		/// </summary>
		/// <param name="scheduleDate">Schedule date</param>
		/// <param name="deptId">Dept id</param>
		/// <param name="masterId">Master id</param>
		/// <param name="actionNo">Action no</param>
		/// <returns>Workflow target count aggregate thread</returns>
		public static WorkflowTargetCountAggregateThread CreateAndStart(
			DateTime scheduleDate,
			string deptId,
			string masterId,
			int actionNo)
		{
			// Thread creation
			var workflowTargetCountAggregateThread = new WorkflowTargetCountAggregateThread(
				scheduleDate,
				deptId,
				masterId,
				actionNo);
			workflowTargetCountAggregateThread.Thread = new Thread(workflowTargetCountAggregateThread.Work);

			// Thread start
			workflowTargetCountAggregateThread.Thread.Start();
			return workflowTargetCountAggregateThread;
		}

		/// <summary>
		/// Workflow target count aggregate execution
		/// </summary>
		public void Work()
		{
			Form1.WriteInfoLogLine(
				string.Format(
					"受注ワークフロー対象件数集計処理開始 : ID : {0} ",
					this.MasterId));

			// Get target data list
			var workflowSettingList = new SummaryReportService()
				.GetOrderWorkflowSettingAllListForReport();
			foreach (var workflowSetting in workflowSettingList)
			{
				// Create log message
				var expandedLogMessage = string.Empty;
				switch (workflowSetting.WorkflowType)
				{
					case WorkflowSetting.m_KBN_WORKFLOW_TYPE_ORDER:
						expandedLogMessage = string.Format(
							"ワークフロー種別：受注ワークフロー（ワークフロー区分：{0}、枝番：{1}）",
							workflowSetting.WorkflowKbn,
							workflowSetting.WorkflowNo);
						break;

					case WorkflowSetting.m_KBN_WORKFLOW_TYPE_FIXEDPURCHASE:
						expandedLogMessage = string.Format(
							"ワークフロー種別：定期台帳ワークフロー（ワークフロー区分：{0}、枝番：{1}）",
							workflowSetting.WorkflowKbn,
							workflowSetting.WorkflowNo);
						break;
				}

				try
				{
					using (var accessor = new SqlAccessor())
					{
						accessor.OpenConnection();
						accessor.BeginTransaction();

						// Update workflow target count
						var orderCount = new WorkflowUtility().UpdateOrderCountForWorkflowSetting(
							workflowSetting.ShopId,
							workflowSetting.WorkflowType,
							workflowSetting.WorkflowKbn,
							workflowSetting.WorkflowNo.ToString(),
							accessor);

						accessor.CommitTransaction();
						expandedLogMessage += string.Format("　{0}件", orderCount);
						FileLogger.WriteInfo(expandedLogMessage);
					}
				}
				catch (Exception ex)
				{
					var message = string.Format("受注ワークフロー対象件数集計失敗。 {0}", expandedLogMessage);
					FileLogger.WriteError(message, ex);
					Form1.WriteErrorLogLine(message);
				}
			}

			// Update task information for end process
			UpdateWorkflowTargetCountAggregateEndProcess();

			Form1.WriteInfoLogLine("受注ワークフロー対象件数集計処理完了。");
		}

		/// <summary>
		/// Update workflow target count aggregate end process
		/// </summary>
		private void UpdateWorkflowTargetCountAggregateEndProcess()
		{
			var service = new TaskScheduleService();
			var taskSchedule = service.Get(
				this.DeptId,
				this.ActionKbn,
				this.MasterId,
				this.ActionNo);
			if (taskSchedule == null) return;

			// Update task information
			taskSchedule.DateEnd = DateTime.Now;
			taskSchedule.ScheduleDate = this.ScheduleDate.AddHours(Constants.WORKFLOW_TARGET_COUNT_AGGREGATE_INTERVAL_HOUR);
			taskSchedule.ExecuteStatus = Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_INIT;
			taskSchedule.LastChanged = Constants.FLG_LASTCHANGED_BATCH;
			service.Update(taskSchedule);
		}
	}
}
