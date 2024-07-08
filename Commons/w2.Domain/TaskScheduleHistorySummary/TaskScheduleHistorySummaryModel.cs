/*
=========================================================================================================
  Module      : タスクスケジュール履歴集計テーブルモデル (TaskScheduleHistorySummaryModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.TaskScheduleHistorySummary
{
	/// <summary>
	/// タスクスケジュール履歴集計テーブルモデル
	/// </summary>
	[Serializable]
	public partial class TaskScheduleHistorySummaryModel : ModelBase<TaskScheduleHistorySummaryModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public TaskScheduleHistorySummaryModel()
		{
			this.DeptId = "";
			this.ActionKbn = "";
			this.ActionMasterId = "";
			this.ActionNo = 1;
			this.ActionResult = "";
			this.ActionKbnDetail = "";
			this.TargetId = "";
			this.HistoryCount = 0;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public TaskScheduleHistorySummaryModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public TaskScheduleHistorySummaryModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return (string)this.DataSource[Constants.FIELD_TASKSCHEDULEHISTORYSUMMARY_DEPT_ID]; }
			set { this.DataSource[Constants.FIELD_TASKSCHEDULEHISTORYSUMMARY_DEPT_ID] = value; }
		}
		/// <summary>実行区分</summary>
		public string ActionKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_TASKSCHEDULEHISTORYSUMMARY_ACTION_KBN]; }
			set { this.DataSource[Constants.FIELD_TASKSCHEDULEHISTORYSUMMARY_ACTION_KBN] = value; }
		}
		/// <summary>実行マスタID</summary>
		public string ActionMasterId
		{
			get { return (string)this.DataSource[Constants.FIELD_TASKSCHEDULEHISTORYSUMMARY_ACTION_MASTER_ID]; }
			set { this.DataSource[Constants.FIELD_TASKSCHEDULEHISTORYSUMMARY_ACTION_MASTER_ID] = value; }
		}
		/// <summary>実行履歴NO</summary>
		public int ActionNo
		{
			get { return (int)this.DataSource[Constants.FIELD_TASKSCHEDULEHISTORYSUMMARY_ACTION_NO]; }
			set { this.DataSource[Constants.FIELD_TASKSCHEDULEHISTORYSUMMARY_ACTION_NO] = value; }
		}
		/// <summary>実行結果</summary>
		public string ActionResult
		{
			get { return (string)this.DataSource[Constants.FIELD_TASKSCHEDULEHISTORYSUMMARY_ACTION_RESULT]; }
			set { this.DataSource[Constants.FIELD_TASKSCHEDULEHISTORYSUMMARY_ACTION_RESULT] = value; }
		}
		/// <summary>詳細実行区分</summary>
		public string ActionKbnDetail
		{
			get { return (string)this.DataSource[Constants.FIELD_TASKSCHEDULEHISTORYSUMMARY_ACTION_KBN_DETAIL]; }
			set { this.DataSource[Constants.FIELD_TASKSCHEDULEHISTORYSUMMARY_ACTION_KBN_DETAIL] = value; }
		}
		/// <summary>対象ターゲットリストID</summary>
		public string TargetId
		{
			get { return (string)this.DataSource[Constants.FIELD_TASKSCHEDULEHISTORYSUMMARY_TARGET_ID]; }
			set { this.DataSource[Constants.FIELD_TASKSCHEDULEHISTORYSUMMARY_TARGET_ID] = value; }
		}
		/// <summary>履歴件数</summary>
		public int HistoryCount
		{
			get { return (int)this.DataSource[Constants.FIELD_TASKSCHEDULEHISTORYSUMMARY_HISTORY_COUNT]; }
			set { this.DataSource[Constants.FIELD_TASKSCHEDULEHISTORYSUMMARY_HISTORY_COUNT] = value; }
		}
		#endregion
	}
}
