/*
=========================================================================================================
  Module      : 受注ワークフロー実行履歴の検索条件クラス(OrderWorkflowExecHistoryListSearchCondition.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 20019 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.Domain.Helper;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.OrderWorkflowExecHistory.Helper
{
	/// <summary>
	/// 受注ワークフロー実行履歴の検索条件クラス
	/// </summary>
	public class OrderWorkflowExecHistoryListSearchCondition : BaseDbMapModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public OrderWorkflowExecHistoryListSearchCondition()
		{
			this.ShopId = "";
			this.WorkflowKbn = "";
			this.WorkflowNo = null;

			this.ScenarioSettingId = "";

			this.ExecStatus = string.Format("{0},{1},{2},{3}",
				Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_NG,
				Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_STOPPED,
				Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_RUNNING,
				Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_HOLD);
			this.ExecStatusSuccess = "";
			this.ExecStatusError = Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_NG;
			this.ExecStatusStopped = Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_STOPPED;
			this.ExecStatusRunning = Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_RUNNING;
			this.ExecStatusHold = Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_HOLD;
			this.ExecPlace = "";
			this.ExecPlaceWorkflow = "";
			this.ExecPlaceScenario = "";
			this.ExecTiming = "";
			this.ExecTimingManual = "";
			this.ExecTimingScedule = "";
			this.WorkflowType = "";
			this.WorkflowTypeOrderWorkflow = "";
			this.WorkflowTypeFixedPurchaseWorkflow = "";
			this.DateFrom = DateTime.Now.Date;
			this.DateTo = DateTime.Now.Date;
			this.BgnRowNum = 1;
			this.EndRowNum = 20;
		}

		#region プロパティ
		/// <summary>店舗ID</summary>
		[DbMapName("shop_id")]
		public string ShopId { get; set; }
		/// <summary>ワークフロー区分</summary>
		[DbMapName("workflow_kbn")]
		public string WorkflowKbn { get; set; }
		/// <summary>ワークフロー枝番</summary>
		[DbMapName("workflow_no")]
		public int? WorkflowNo { get; set; }
		/// <summary>シナリオ設定ID</summary>
		[DbMapName("scenario_setting_id")]
		public string ScenarioSettingId { get; set; }
		/// <summary>実行ステータス</summary>
		[DbMapName("exec_status")]
		public string ExecStatus { get; set; }
		/// <summary>実行ステータス(成功)</summary>
		[DbMapName("exec_status_success")]
		public string ExecStatusSuccess { get; set; }
		/// <summary>実行ステータス(失敗あり)</summary>
		[DbMapName("exec_status_error")]
		public string ExecStatusError { get; set; }
		/// <summary>実行ステータス(失敗あり)</summary>
		[DbMapName("exec_status_stopped")]
		public string ExecStatusStopped { get; set; }
		/// <summary>実行ステータス(実行中)</summary>
		[DbMapName("exec_status_running")]
		public string ExecStatusRunning { get; set; }
		/// <summary>実行ステータス(保留中)</summary>
		[DbMapName("exec_status_hold")]
		public string ExecStatusHold { get; set; }
		/// <summary>実行場所</summary>
		[DbMapName("exec_place")]
		public string ExecPlace { get; set; }
		/// <summary>実行場所(ワークフロー)</summary>
		[DbMapName("exec_place_workflow")]
		public string ExecPlaceWorkflow { get; set; }
		/// <summary>実行場所(シナリオ)</summary>
		[DbMapName("exec_place_scenario")]
		public string ExecPlaceScenario { get; set; }
		/// <summary>実行タイミング</summary>
		[DbMapName("exec_timing")]
		public string ExecTiming { get; set; }
		/// <summary>実行タイミング(手動)</summary>
		[DbMapName("exec_timing_manual")]
		public string ExecTimingManual { get; set; }
		/// <summary>実行タイミング(スケジュール)</summary>
		[DbMapName("exec_timing_scedule")]
		public string ExecTimingScedule { get; set; }
		/// <summary>ワークフロー種別</summary>
		[DbMapName("workflow_type")]
		public string WorkflowType { get; set; }
		/// <summary>ワークフロー種別(受注ワークフロー)</summary>
		[DbMapName("workflow_type_order_workflow")]
		public string WorkflowTypeOrderWorkflow { get; set; }
		/// <summary>ワークフロー種別(定期台帳ワークフロー)</summary>
		[DbMapName("workflow_type_fixed_purchase_workflow")]
		public string WorkflowTypeFixedPurchaseWorkflow { get; set; }
		/// <summary>検索開始日</summary>
		[DbMapName("date_from")]
		public DateTime? DateFrom { get; set; }
		/// <summary>検索終了日</summary>
		[DbMapName("date_to")]
		public DateTime? DateTo { get; set; }
		/// <summary>開始行番号</summary>
		[DbMapName("bgn_row_num")]
		public int BgnRowNum { get; set; }
		/// <summary>終了行番号</summary>
		[DbMapName("end_row_num")]
		public int EndRowNum { get; set; }
		#endregion
	}
}
