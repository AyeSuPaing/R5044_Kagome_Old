/*
=========================================================================================================
  Module      : タスクスケジュールマスタモデル (TaskScheduleModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.TaskSchedule
{
	/// <summary>
	/// タスクスケジュールマスタモデル
	/// </summary>
	public partial class TaskScheduleModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>実行ステータス</summary>
		public string ExecuteStatusTarget
		{
			get { return (string)this.DataSource["execute_status_target"]; }
			set { this.DataSource["execute_status_target"] = value; }
		}
		/// <summary>準備ステータス</summary>
		public string PrepareStatusTarget
		{
			get { return (string)this.DataSource["prepare_status_target"]; }
			set { this.DataSource["prepare_status_target"] = value; }
		}
		#endregion
	}
}
