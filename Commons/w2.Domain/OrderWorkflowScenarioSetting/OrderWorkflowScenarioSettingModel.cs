/*
=========================================================================================================
  Module      : 受注ワークフローシナリオ設定モデル (OrderWorkflowScenarioSettingModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.OrderWorkflowScenarioSetting
{
	/// <summary>
	/// 受注ワークフローシナリオ設定モデル
	/// </summary>
	[Serializable]
	public partial class OrderWorkflowScenarioSettingModel : ModelBase<OrderWorkflowScenarioSettingModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public OrderWorkflowScenarioSettingModel()
		{
			this.ScenarioSettingId = "";
			this.ScenarioName = "";
			this.ExecTiming = Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_TIMING_MANUAL;
			this.ScheduleKbn = "";
			this.ScheduleDayOfWeek = null;
			this.ScheduleYear = null;
			this.ScheduleMonth = null;
			this.ScheduleDay = null;
			this.ScheduleHour = null;
			this.ScheduleMinute = null;
			this.ScheduleSecond = null;
			this.ValidFlg = Constants.FLG_ORDERWORKFLOWSCENARIOSETTING_VALID_FLG_VALID;
			this.LastChanged = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public OrderWorkflowScenarioSettingModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public OrderWorkflowScenarioSettingModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>シナリオ設定ID</summary>
		public string ScenarioSettingId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCENARIO_SETTING_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCENARIO_SETTING_ID] = value; }
		}
		/// <summary>シナリオ名</summary>
		public string ScenarioName
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCENARIO_NAME]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCENARIO_NAME] = value; }
		}
		/// <summary>実行タイミング</summary>
		public string ExecTiming
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_EXEC_TIMING]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_EXEC_TIMING] = value; }
		}
		/// <summary>スケジュール区分</summary>
		public string ScheduleKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_KBN]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_KBN] = value; }
		}
		/// <summary>スケジュール曜日</summary>
		public string ScheduleDayOfWeek
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_DAY_OF_WEEK] == DBNull.Value) return null;
				return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_DAY_OF_WEEK];
			}
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_DAY_OF_WEEK] = value; }
		}
		/// <summary>スケジュール日程(年)</summary>
		public int? ScheduleYear
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_YEAR] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_YEAR];
			}
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_YEAR] = value; }
		}
		/// <summary>スケジュール日程(月)</summary>
		public int? ScheduleMonth
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_MONTH] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_MONTH];
			}
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_MONTH] = value; }
		}
		/// <summary>スケジュール日程(日)</summary>
		public int? ScheduleDay
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_DAY] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_DAY];
			}
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_DAY] = value; }
		}
		/// <summary>スケジュール日程(時)</summary>
		public int? ScheduleHour
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_HOUR] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_HOUR];
			}
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_HOUR] = value; }
		}
		/// <summary>スケジュール日程(分)</summary>
		public int? ScheduleMinute
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_MINUTE] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_MINUTE];
			}
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_MINUTE] = value; }
		}
		/// <summary>スケジュール日程(秒)</summary>
		public int? ScheduleSecond
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_SECOND] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_SECOND];
			}
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_SCHEDULE_SECOND] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_VALID_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
