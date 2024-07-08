/*
=========================================================================================================
  Module      : ターゲットリスト設定マスタモデル (TargetListModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.TargetList
{
	/// <summary>
	/// ターゲットリスト設定マスタモデル
	/// </summary>
	[Serializable]
	public partial class TargetListModel : ModelBase<TargetListModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public TargetListModel()
		{
			// TODO:定数を利用するよう書き換えてください。
			this.Status = Constants.FLG_TARGETLIST_STATUS_NORMAL;
			this.DataCount = null;
			this.DataCountDate = null;
			this.ExecTiming = Constants.FLG_TARGETLIST_EXEC_TIMING_MANUAL;
			this.ScheduleYear = null;
			this.ScheduleMonth = null;
			this.ScheduleDay = null;
			this.ScheduleHour = null;
			this.ScheduleMinute = null;
			this.ScheduleSecond = null;
			this.ValidFlg = "1";
			this.DelFlg = "0";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public TargetListModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public TargetListModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return (string)this.DataSource[Constants.FIELD_TARGETLIST_DEPT_ID]; }
			set { this.DataSource[Constants.FIELD_TARGETLIST_DEPT_ID] = value; }
		}
		/// <summary>ターゲットリストID</summary>
		public string TargetId
		{
			get { return (string)this.DataSource[Constants.FIELD_TARGETLIST_TARGET_ID]; }
			set { this.DataSource[Constants.FIELD_TARGETLIST_TARGET_ID] = value; }
		}
		/// <summary>ターゲットリスト名</summary>
		public string TargetName
		{
			get { return (string)this.DataSource[Constants.FIELD_TARGETLIST_TARGET_NAME]; }
			set { this.DataSource[Constants.FIELD_TARGETLIST_TARGET_NAME] = value; }
		}
		/// <summary>ステータス</summary>
		public string Status
		{
			get { return (string)this.DataSource[Constants.FIELD_TARGETLIST_STATUS]; }
			set { this.DataSource[Constants.FIELD_TARGETLIST_STATUS] = value; }
		}
		/// <summary>ターゲット種別</summary>
		public string TargetType
		{
			get { return (string)this.DataSource[Constants.FIELD_TARGETLIST_TARGET_TYPE]; }
			set { this.DataSource[Constants.FIELD_TARGETLIST_TARGET_TYPE] = value; }
		}
		/// <summary>抽出条件</summary>
		public string TargetCondition
		{
			get { return (string)this.DataSource[Constants.FIELD_TARGETLIST_TARGET_CONDITION]; }
			set { this.DataSource[Constants.FIELD_TARGETLIST_TARGET_CONDITION] = value; }
		}
		/// <summary>前回抽出件数</summary>
		public int? DataCount
		{
			get
			{
				if (this.DataSource[Constants.FIELD_TARGETLIST_DATA_COUNT] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_TARGETLIST_DATA_COUNT];
			}
			set { this.DataSource[Constants.FIELD_TARGETLIST_DATA_COUNT] = value; }
		}
		/// <summary>前回抽出日付</summary>
		public DateTime? DataCountDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_TARGETLIST_DATA_COUNT_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_TARGETLIST_DATA_COUNT_DATE];
			}
			set { this.DataSource[Constants.FIELD_TARGETLIST_DATA_COUNT_DATE] = value; }
		}
		/// <summary>実行タイミング</summary>
		public string ExecTiming
		{
			get { return (string)this.DataSource[Constants.FIELD_TARGETLIST_EXEC_TIMING]; }
			set { this.DataSource[Constants.FIELD_TARGETLIST_EXEC_TIMING] = value; }
		}
		/// <summary>スケジュール区分</summary>
		public string ScheduleKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_TARGETLIST_SCHEDULE_KBN]; }
			set { this.DataSource[Constants.FIELD_TARGETLIST_SCHEDULE_KBN] = value; }
		}
		/// <summary>スケジュール曜日</summary>
		public string ScheduleDayOfWeek
		{
			get { return (string)this.DataSource[Constants.FIELD_TARGETLIST_SCHEDULE_DAY_OF_WEEK]; }
			set { this.DataSource[Constants.FIELD_TARGETLIST_SCHEDULE_DAY_OF_WEEK] = value; }
		}
		/// <summary>スケジュール日程(年)</summary>
		public int? ScheduleYear
		{
			get
			{
				if (this.DataSource[Constants.FIELD_TARGETLIST_SCHEDULE_YEAR] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_TARGETLIST_SCHEDULE_YEAR];
			}
			set { this.DataSource[Constants.FIELD_TARGETLIST_SCHEDULE_YEAR] = value; }
		}
		/// <summary>スケジュール日程(月)</summary>
		public int? ScheduleMonth
		{
			get
			{
				if (this.DataSource[Constants.FIELD_TARGETLIST_SCHEDULE_MONTH] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_TARGETLIST_SCHEDULE_MONTH];
			}
			set { this.DataSource[Constants.FIELD_TARGETLIST_SCHEDULE_MONTH] = value; }
		}
		/// <summary>スケジュール日程(日)</summary>
		public int? ScheduleDay
		{
			get
			{
				if (this.DataSource[Constants.FIELD_TARGETLIST_SCHEDULE_DAY] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_TARGETLIST_SCHEDULE_DAY];
			}
			set { this.DataSource[Constants.FIELD_TARGETLIST_SCHEDULE_DAY] = value; }
		}
		/// <summary>スケジュール日程(時)</summary>
		public int? ScheduleHour
		{
			get
			{
				if (this.DataSource[Constants.FIELD_TARGETLIST_SCHEDULE_HOUR] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_TARGETLIST_SCHEDULE_HOUR];
			}
			set { this.DataSource[Constants.FIELD_TARGETLIST_SCHEDULE_HOUR] = value; }
		}
		/// <summary>スケジュール日程(分)</summary>
		public int? ScheduleMinute
		{
			get
			{
				if (this.DataSource[Constants.FIELD_TARGETLIST_SCHEDULE_MINUTE] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_TARGETLIST_SCHEDULE_MINUTE];
			}
			set { this.DataSource[Constants.FIELD_TARGETLIST_SCHEDULE_MINUTE] = value; }
		}
		/// <summary>スケジュール日程(秒)</summary>
		public int? ScheduleSecond
		{
			get
			{
				if (this.DataSource[Constants.FIELD_TARGETLIST_SCHEDULE_SECOND] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_TARGETLIST_SCHEDULE_SECOND];
			}
			set { this.DataSource[Constants.FIELD_TARGETLIST_SCHEDULE_SECOND] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_TARGETLIST_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_TARGETLIST_VALID_FLG] = value; }
		}
		/// <summary>削除フラグ</summary>
		public string DelFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_TARGETLIST_DEL_FLG]; }
			set { this.DataSource[Constants.FIELD_TARGETLIST_DEL_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_TARGETLIST_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_TARGETLIST_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_TARGETLIST_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_TARGETLIST_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_TARGETLIST_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_TARGETLIST_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
