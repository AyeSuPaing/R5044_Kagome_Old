/*
=========================================================================================================
  Module      : タスクスケジュールマスタモデル (TaskScheduleModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.TaskSchedule
{
	/// <summary>
	/// タスクスケジュールマスタモデル
	/// </summary>
	[Serializable]
	public partial class TaskScheduleModel : ModelBase<TaskScheduleModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public TaskScheduleModel()
		{
			this.ActionNo = 1;
			this.PrepareStatus = Constants.FLG_TASKSCHEDULE_PREPARE_STATUS_INIT;
			this.ExecuteStatus = Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_INIT;
			this.Progress = string.Empty;
			this.StopFlg = string.Empty;
			this.DelFlg = "0";
			this.DateBegin = null;
			this.DateEnd = null;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public TaskScheduleModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public TaskScheduleModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>スケジュール日付</summary>
		public DateTime ScheduleDate
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_TASKSCHEDULE_SCHEDULE_DATE]; }
			set { this.DataSource[Constants.FIELD_TASKSCHEDULE_SCHEDULE_DATE] = value; }
		}
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return (string)this.DataSource[Constants.FIELD_TASKSCHEDULE_DEPT_ID]; }
			set { this.DataSource[Constants.FIELD_TASKSCHEDULE_DEPT_ID] = value; }
		}
		/// <summary>実行区分</summary>
		public string ActionKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_TASKSCHEDULE_ACTION_KBN]; }
			set { this.DataSource[Constants.FIELD_TASKSCHEDULE_ACTION_KBN] = value; }
		}
		/// <summary>実行マスタID</summary>
		public string ActionMasterId
		{
			get { return (string)this.DataSource[Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID]; }
			set { this.DataSource[Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID] = value; }
		}
		/// <summary>実行履歴NO</summary>
		public int ActionNo
		{
			get { return (int)this.DataSource[Constants.FIELD_TASKSCHEDULE_ACTION_NO]; }
			set { this.DataSource[Constants.FIELD_TASKSCHEDULE_ACTION_NO] = value; }
		}
		/// <summary>準備ステータス</summary>
		public string PrepareStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_TASKSCHEDULE_PREPARE_STATUS]; }
			set { this.DataSource[Constants.FIELD_TASKSCHEDULE_PREPARE_STATUS] = value; }
		}
		/// <summary>実行ステータス</summary>
		public string ExecuteStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_TASKSCHEDULE_EXECUTE_STATUS]; }
			set { this.DataSource[Constants.FIELD_TASKSCHEDULE_EXECUTE_STATUS] = value; }
		}
		/// <summary>進捗</summary>
		public string Progress
		{
			get { return (string)this.DataSource[Constants.FIELD_TASKSCHEDULE_PROGRESS]; }
			set { this.DataSource[Constants.FIELD_TASKSCHEDULE_PROGRESS] = value; }
		}
		/// <summary>停止フラグ</summary>
		public string StopFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_TASKSCHEDULE_STOP_FLG]; }
			set { this.DataSource[Constants.FIELD_TASKSCHEDULE_STOP_FLG] = value; }
		}
		/// <summary>削除フラグ</summary>
		public string DelFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_TASKSCHEDULE_DEL_FLG]; }
			set { this.DataSource[Constants.FIELD_TASKSCHEDULE_DEL_FLG] = value; }
		}
		/// <summary>開始日時</summary>
		public DateTime? DateBegin
		{
			get
			{
				if (this.DataSource[Constants.FIELD_TASKSCHEDULE_DATE_BEGIN] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_TASKSCHEDULE_DATE_BEGIN];
			}
			set { this.DataSource[Constants.FIELD_TASKSCHEDULE_DATE_BEGIN] = value; }
		}
		/// <summary>終了日時</summary>
		public DateTime? DateEnd
		{
			get
			{
				if (this.DataSource[Constants.FIELD_TASKSCHEDULE_DATE_END] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_TASKSCHEDULE_DATE_END];
			}
			set { this.DataSource[Constants.FIELD_TASKSCHEDULE_DATE_END] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_TASKSCHEDULE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_TASKSCHEDULE_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_TASKSCHEDULE_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_TASKSCHEDULE_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_TASKSCHEDULE_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_TASKSCHEDULE_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
