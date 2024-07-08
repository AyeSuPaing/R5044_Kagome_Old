/*
=========================================================================================================
  Module      : タスクスケジュールログマスタモデル(TaskScheduleLogModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.TaskScheduleLog
{
	/// <summary>
	/// タスクスケジュールログマスタモデル
	/// </summary>
	public class TaskScheduleLogModel : ModelBase<TaskScheduleLogModel>
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public TaskScheduleLogModel()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public TaskScheduleLogModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public TaskScheduleLogModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}

		/// <summary>
		/// 識別ID
		/// </summary>
		public string DeptId
		{
			get { return (string)this.DataSource[Constants.FIELD_TASKSCHEDULELOG_DEPT_ID]; }
			set { this.DataSource[Constants.FIELD_TASKSCHEDULELOG_DEPT_ID] = value; }
		}
		/// <summary>
		/// 実行区分
		/// </summary>
		public string ActionKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_TASKSCHEDULELOG_ACTION_KBN]; }
			set { this.DataSource[Constants.FIELD_TASKSCHEDULELOG_ACTION_KBN] = value; }
		}
		/// <summary>
		/// 実行マスタ区分
		/// </summary>
		public string ActionMasterId
		{
			get { return (string)this.DataSource[Constants.FIELD_TASKSCHEDULELOG_ACTION_MASTER_ID]; }
			set { this.DataSource[Constants.FIELD_TASKSCHEDULELOG_ACTION_MASTER_ID] = value; }
		}
		/// <summary>
		/// 実行履歴NO
		/// </summary>
		public int ActionNo
		{
			get { return (int)this.DataSource[Constants.FIELD_TASKSCHEDULELOG_ACTION_NO]; }
			set { this.DataSource[Constants.FIELD_TASKSCHEDULELOG_ACTION_NO] = value; }
		}
		/// <summary>
		/// メッセージアプリ区分
		/// </summary>
		public string MessagingAppKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_TASKSCHEDULELOG_MESSAGING_APP_KBN]; }
			set { this.DataSource[Constants.FIELD_TASKSCHEDULELOG_MESSAGING_APP_KBN] = value; }
		}
		/// <summary>
		/// 配信結果
		/// </summary>
		public string Result
		{
			get { return (string)this.DataSource[Constants.FIELD_TASKSCHEDULELOG_RESULT]; }
			set { this.DataSource[Constants.FIELD_TASKSCHEDULELOG_RESULT] = value; }
		}
	}
}
