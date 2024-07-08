/*
=========================================================================================================
  Module      : Workflow target count aggregate action(WorkflowTargetCountAggregateAction.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Order.Workflow;
using w2.Common.Sql;
using w2.Domain.SummaryReport;

namespace w2.MarketingPlanner.Batch.CreateReport.Action
{
	/// <summary>
	/// Workflow target count aggregate action
	/// </summary>
	public class WorkflowTargetCountAggregateAction : ActionBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="reportKbn">Report Kbn</param>
		public WorkflowTargetCountAggregateAction(string reportKbn)
			: base(reportKbn)
		{
		}

		/// <summary>
		/// Do execute
		/// </summary>
		public override void DoExecute()
		{
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

				var isSuccess = false;
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
						isSuccess = true;
					}
				}
				catch (Exception ex)
				{
					var message = string.Format("受注ワークフロー対象件数集計失敗。 {0}", expandedLogMessage);
					WriteLogError(ex, message);
				}

				WriteResultLog(isSuccess, expandedLogMessage);
			}
		}
	}
}
