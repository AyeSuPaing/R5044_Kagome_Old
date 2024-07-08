/*
=========================================================================================================
  Module      : タスクスケジュール履歴マスタモデル (TaskScheduleHistoryModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.TaskScheduleHistory
{
	/// <summary>
	/// タスクスケジュール履歴マスタモデル
	/// </summary>
	[Serializable]
	public partial class TaskScheduleHistoryModel : ModelBase<TaskScheduleHistoryModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public TaskScheduleHistoryModel()
		{
			this.DeptId = "";
			this.ActionKbn = "";
			this.ActionMasterId = "";
			this.ActionNo = 1;
			this.ActionStep = 0;
			this.ActionKbnDetail = "";
			this.ActionResult = "";
			this.TargetId = "";
			this.UserId = "";
			this.MailAddr = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public TaskScheduleHistoryModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public TaskScheduleHistoryModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>スケジュール履歴NO</summary>
		public long HistoryNo
		{
			get { return (long)this.DataSource[Constants.FIELD_TASKSCHEDULEHISTORY_HISTORY_NO]; }
		}
		/// <summary>スケジュール日付</summary>
		public DateTime ScheduleDate
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_TASKSCHEDULEHISTORY_SCHEDULE_DATE]; }
			set { this.DataSource[Constants.FIELD_TASKSCHEDULEHISTORY_SCHEDULE_DATE] = value; }
		}
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return (string)this.DataSource[Constants.FIELD_TASKSCHEDULEHISTORY_DEPT_ID]; }
			set { this.DataSource[Constants.FIELD_TASKSCHEDULEHISTORY_DEPT_ID] = value; }
		}
		/// <summary>実行区分</summary>
		public string ActionKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_KBN]; }
			set { this.DataSource[Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_KBN] = value; }
		}
		/// <summary>実行マスタID</summary>
		public string ActionMasterId
		{
			get { return (string)this.DataSource[Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_MASTER_ID]; }
			set { this.DataSource[Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_MASTER_ID] = value; }
		}
		/// <summary>実行履歴NO</summary>
		public int ActionNo
		{
			get { return (int)this.DataSource[Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_NO]; }
			set { this.DataSource[Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_NO] = value; }
		}
		/// <summary>実行ステップ</summary>
		public int ActionStep
		{
			get { return (int)this.DataSource[Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_STEP]; }
			set { this.DataSource[Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_STEP] = value; }
		}
		/// <summary>詳細実行区分</summary>
		public string ActionKbnDetail
		{
			get { return (string)this.DataSource[Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_KBN_DETAIL]; }
			set { this.DataSource[Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_KBN_DETAIL] = value; }
		}
		/// <summary>実行結果</summary>
		public string ActionResult
		{
			get { return (string)this.DataSource[Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_RESULT]; }
			set { this.DataSource[Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_RESULT] = value; }
		}
		/// <summary>対象ターゲットリストID</summary>
		public string TargetId
		{
			get { return (string)this.DataSource[Constants.FIELD_TASKSCHEDULEHISTORY_TARGET_ID]; }
			set { this.DataSource[Constants.FIELD_TASKSCHEDULEHISTORY_TARGET_ID] = value; }
		}
		/// <summary>対象ユーザID</summary>
		public string UserId
		{
			get { return (string)this.DataSource[Constants.FIELD_TASKSCHEDULEHISTORY_USER_ID]; }
			set { this.DataSource[Constants.FIELD_TASKSCHEDULEHISTORY_USER_ID] = value; }
		}
		/// <summary>対象メールアドレス</summary>
		public string MailAddr
		{
			get { return (string)this.DataSource[Constants.FIELD_TASKSCHEDULEHISTORY_MAIL_ADDR]; }
			set { this.DataSource[Constants.FIELD_TASKSCHEDULEHISTORY_MAIL_ADDR] = value; }
		}
		#endregion
	}
}
