/*
=========================================================================================================
  Module      : 受注ワークフロー実行履歴モデル (OrderWorkflowExecHistoryModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.OrderWorkflowExecHistory
{
	/// <summary>
	/// 受注ワークフロー実行履歴モデル
	/// </summary>
	[Serializable]
	public partial class OrderWorkflowExecHistoryModel : ModelBase<OrderWorkflowExecHistoryModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public OrderWorkflowExecHistoryModel()
		{
			this.ShopId = "";
			this.WorkflowKbn = "";
			this.ScenarioSettingId = "";
			this.WorkflowName = "";
			this.ScenarioName = "";
			this.ExecStatus = "";
			this.SuccessRate = "";
			this.WorkflowType = "";
			this.ExecPlace = "";
			this.ExecTiming = Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_TIMING_MANUAL;
			this.Message = "";
			this.DateBegin = null;
			this.DateEnd = null;
			this.LastChanged = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public OrderWorkflowExecHistoryModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public OrderWorkflowExecHistoryModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>受注ワークフロー実行履歴ID</summary>
		public long OrderWorkflowExecHistoryId
		{
			get { return (long)this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_ORDER_WORKFLOW_EXEC_HISTORY_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_ORDER_WORKFLOW_EXEC_HISTORY_ID] = value; }
		}
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_SHOP_ID] = value; }
		}
		/// <summary>ワークフロー区分</summary>
		public string WorkflowKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_WORKFLOW_KBN]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_WORKFLOW_KBN] = value; }
		}
		/// <summary>ワークフロー枝番</summary>
		public int WorkflowNo
		{
			get { return (int)this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_WORKFLOW_NO]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_WORKFLOW_NO] = value; }
		}
		/// <summary>シナリオ設定ID</summary>
		public string ScenarioSettingId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_SCENARIO_SETTING_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_SCENARIO_SETTING_ID] = value; }
		}
		/// <summary>ワークフロー名</summary>
		public string WorkflowName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_WORKFLOW_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_WORKFLOW_NAME] = value; }
		}
		/// <summary>シナリオ名</summary>
		public string ScenarioName
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_SCENARIO_NAME] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_SCENARIO_NAME];
			}
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_SCENARIO_NAME] = value; }
		}
		/// <summary>実行ステータス</summary>
		public string ExecStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS] = value; }
		}
		/// <summary>成功件率</summary>
		public string SuccessRate
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_SUCCESS_RATE]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_SUCCESS_RATE] = value; }
		}
		/// <summary>ワークフロー種別</summary>
		public string WorkflowType
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_WORKFLOW_TYPE]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_WORKFLOW_TYPE] = value; }
		}
		/// <summary>実行場所</summary>
		public string ExecPlace
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_EXEC_PLACE]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_EXEC_PLACE] = value; }
		}
		/// <summary>実行タイミング</summary>
		public string ExecTiming
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_EXEC_TIMING]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_EXEC_TIMING] = value; }
		}
		/// <summary>メッセージ</summary>
		public string Message
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_MESSAGE]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_MESSAGE] = value; }
		}
		/// <summary>開始日時</summary>
		public DateTime? DateBegin
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_DATE_BEGIN] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_DATE_BEGIN];
			}
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_DATE_BEGIN] = value; }
		}
		/// <summary>終了日時</summary>
		public DateTime? DateEnd
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_DATE_END] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_DATE_END];
			}
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_DATE_END] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_DATE_CREATED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
