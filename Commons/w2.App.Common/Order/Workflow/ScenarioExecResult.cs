/*
=========================================================================================================
  Module      : シナリオ実行リザルトクラス(ScenarioExecResult.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using w2.Domain.OrderWorkflowExecHistory;
using w2.Domain.OrderWorkflowScenarioSetting;

namespace w2.App.Common.Order.Workflow
{
	/// <summary>
	/// シナリオ実行リザルトクラス
	/// </summary>
	public class ScenarioExecResult
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ScenarioExecResult()
		{
			this.Histories = new List<OrderWorkflowExecHistoryModel>();
		}

		/// <summary>デプトID</summary>
		public string DeptId { get; set; }
		/// <summary>アクション区分</summary>
		public string ActionKbn { get; set; }
		/// <summary>マスターID</summary>
		public string MasterId { get; set; }
		/// <summary>アクションNo</summary>
		public int ActionNo { get; set; }
		/// <summary>スケジュール日時</summary>
		public DateTime ScheduleDate { get; set; }
		/// <summary>実行者名</summary>
		public string LastChanged { get; set; }
		/// <summary>実行ステータス</summary>
		public string ExecTiming { get; set; }
		/// <summary>シナリオモデル</summary>
		public OrderWorkflowScenarioSettingModel Scenario { get; set; }
		/// <summary>シナリオの実行ステータス</summary>
		public string ScenarioExecStatus { get; set; }
		/// <summary>実行されるワークフロー件数</summary>
		public int WorkflowCountBeExec { get; set; }
		/// <summary>正常に実行されたワークフロー件数</summary>
		public int WorkflowExecSuccessfulCount { get; set; }
		/// <summary>実行中のワークフローの全てのアクション件数</summary>
		public int ActionCountBeExecOfRunningWorkflow { get; set; }
		/// <summary>実行中のワークフローで正常に実行されたアクション件数</summary>
		public int ActionExecSuccessfulCountOfRunningWorkflow { get; set; }
		/// <summary>実行中のワークフローで実行されたアクション件数</summary>
		public int ActionExecCountOfRunningWorkflow { get; set; }
		/// <summary>実行したワークフローの履歴</summary>
		public List<OrderWorkflowExecHistoryModel> Histories { get; set; }
		/// <summary>実行開始時間</summary>
		public DateTime ExecStartDatetime { get; set; }
		/// <summary>実行終了時間</summary>
		public DateTime ExecEndDatetime { get; set; }
	}
}