/*
=========================================================================================================
  Module      : メール配信設定マスタモデル (MailDistSettingModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.MailDistSetting
{
	/// <summary>
	/// メール配信設定マスタモデル
	/// </summary>
	[Serializable]
	public partial class MailDistSettingModel : ModelBase<MailDistSettingModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public MailDistSettingModel()
		{
			this.DeptId = "";
			this.MaildistId = "";
			this.MaildistName = "";
			this.Status = "";
			this.LastCount = 0;
			this.LastErrorexceptCount = 0;
			this.LastMobilemailexceptCount = 0;
			this.LastDistDate = null;
			this.TargetId = "";
			this.TargetExtractFlg = Constants.FLG_MAILDISTSETTING_TARGET_EXTRACT_FLG_OFF;
			this.TargetId2 = "";
			this.TargetExtractFlg2 = Constants.FLG_MAILDISTSETTING_TARGET_EXTRACT_FLG_OFF;
			this.TargetId3 = "";
			this.TargetExtractFlg3 = Constants.FLG_MAILDISTSETTING_TARGET_EXTRACT_FLG_OFF;
			this.TargetId4 = "";
			this.TargetExtractFlg4 = Constants.FLG_MAILDISTSETTING_TARGET_EXTRACT_FLG_OFF;
			this.TargetId5 = "";
			this.TargetExtractFlg5 = Constants.FLG_MAILDISTSETTING_TARGET_EXTRACT_FLG_OFF;
			this.ExceptErrorPoint = 5;
			this.ExceptMobilemailFlg = Constants.FLG_MAILDISTSETTING_EXCEPT_MOBILEMAIL_FLG_OFF;
			this.MailtextId = "";
			this.ExecTiming = Constants.FLG_MAILDISTSETTING_EXEC_TIMING_MANUAL;
			this.ScheduleKbn = "";
			this.ScheduleDayOfWeek = "";
			this.ScheduleYear = null;
			this.ScheduleMonth = null;
			this.ScheduleDay = null;
			this.ScheduleHour = null;
			this.ScheduleMinute = null;
			this.ScheduleSecond = null;
			this.ValidFlg = Constants.FLG_MAILDISTSETTING_VALID_FLG_VALID;
			this.DelFlg = "0";
			this.LastChanged = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public MailDistSettingModel(DataRowView source) : this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public MailDistSettingModel(Hashtable source) : this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_DEPT_ID]; }
			set { this.DataSource[Constants.FIELD_MAILDISTSETTING_DEPT_ID] = value; }
		}
		/// <summary>メール配信設定ID</summary>
		public string MaildistId
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_MAILDIST_ID]; }
			set { this.DataSource[Constants.FIELD_MAILDISTSETTING_MAILDIST_ID] = value; }
		}
		/// <summary>メール配信設定名</summary>
		public string MaildistName
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_MAILDIST_NAME]; }
			set { this.DataSource[Constants.FIELD_MAILDISTSETTING_MAILDIST_NAME] = value; }
		}
		/// <summary>ステータス</summary>
		public string Status
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_STATUS]; }
			set { this.DataSource[Constants.FIELD_MAILDISTSETTING_STATUS] = value; }
		}
		/// <summary>最終集計人数</summary>
		public long LastCount
		{
			get { return (long)this.DataSource[Constants.FIELD_MAILDISTSETTING_LAST_COUNT]; }
			set { this.DataSource[Constants.FIELD_MAILDISTSETTING_LAST_COUNT] = value; }
		}
		/// <summary>最終エラー除外人数</summary>
		public long LastErrorexceptCount
		{
			get { return (long)this.DataSource[Constants.FIELD_MAILDISTSETTING_LAST_ERROREXCEPT_COUNT]; }
			set { this.DataSource[Constants.FIELD_MAILDISTSETTING_LAST_ERROREXCEPT_COUNT] = value; }
		}
		/// <summary>最終モバイル除外人数</summary>
		public long LastMobilemailexceptCount
		{
			get { return (long)this.DataSource[Constants.FIELD_MAILDISTSETTING_LAST_MOBILEMAILEXCEPT_COUNT]; }
			set { this.DataSource[Constants.FIELD_MAILDISTSETTING_LAST_MOBILEMAILEXCEPT_COUNT] = value; }
		}
		/// <summary>最終配信開始日時</summary>
		public DateTime? LastDistDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MAILDISTSETTING_LAST_DIST_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_MAILDISTSETTING_LAST_DIST_DATE];
			}
			set { this.DataSource[Constants.FIELD_MAILDISTSETTING_LAST_DIST_DATE] = value; }
		}
		/// <summary>ターゲットID</summary>
		public string TargetId
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_TARGET_ID]; }
			set { this.DataSource[Constants.FIELD_MAILDISTSETTING_TARGET_ID] = value; }
		}
		/// <summary>ターゲット抽出フラグ</summary>
		public string TargetExtractFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_TARGET_EXTRACT_FLG]; }
			set { this.DataSource[Constants.FIELD_MAILDISTSETTING_TARGET_EXTRACT_FLG] = value; }
		}
		/// <summary>ターゲットID2</summary>
		public string TargetId2
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_TARGET_ID2]; }
			set { this.DataSource[Constants.FIELD_MAILDISTSETTING_TARGET_ID2] = value; }
		}
		/// <summary>ターゲット2抽出フラグ</summary>
		public string TargetExtractFlg2
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_TARGET_EXTRACT_FLG2]; }
			set { this.DataSource[Constants.FIELD_MAILDISTSETTING_TARGET_EXTRACT_FLG2] = value; }
		}
		/// <summary>ターゲットID3</summary>
		public string TargetId3
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_TARGET_ID3]; }
			set { this.DataSource[Constants.FIELD_MAILDISTSETTING_TARGET_ID3] = value; }
		}
		/// <summary>ターゲット3抽出フラグ</summary>
		public string TargetExtractFlg3
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_TARGET_EXTRACT_FLG3]; }
			set { this.DataSource[Constants.FIELD_MAILDISTSETTING_TARGET_EXTRACT_FLG3] = value; }
		}
		/// <summary>ターゲットID4</summary>
		public string TargetId4
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_TARGET_ID4]; }
			set { this.DataSource[Constants.FIELD_MAILDISTSETTING_TARGET_ID4] = value; }
		}
		/// <summary>ターゲット4抽出フラグ</summary>
		public string TargetExtractFlg4
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_TARGET_EXTRACT_FLG4]; }
			set { this.DataSource[Constants.FIELD_MAILDISTSETTING_TARGET_EXTRACT_FLG4] = value; }
		}
		/// <summary>ターゲットID5</summary>
		public string TargetId5
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_TARGET_ID5]; }
			set { this.DataSource[Constants.FIELD_MAILDISTSETTING_TARGET_ID5] = value; }
		}
		/// <summary>ターゲット5抽出フラグ</summary>
		public string TargetExtractFlg5
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_TARGET_EXTRACT_FLG5]; }
			set { this.DataSource[Constants.FIELD_MAILDISTSETTING_TARGET_EXTRACT_FLG5] = value; }
		}
		/// <summary>配信除外エラーポイント</summary>
		public int ExceptErrorPoint
		{
			get { return (int)this.DataSource[Constants.FIELD_MAILDISTSETTING_EXCEPT_ERROR_POINT]; }
			set { this.DataSource[Constants.FIELD_MAILDISTSETTING_EXCEPT_ERROR_POINT] = value; }
		}
		/// <summary>モバイルメール排除フラグ</summary>
		public string ExceptMobilemailFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_EXCEPT_MOBILEMAIL_FLG]; }
			set { this.DataSource[Constants.FIELD_MAILDISTSETTING_EXCEPT_MOBILEMAIL_FLG] = value; }
		}
		/// <summary>メール文章ID</summary>
		public string MailtextId
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_MAILTEXT_ID]; }
			set { this.DataSource[Constants.FIELD_MAILDISTSETTING_MAILTEXT_ID] = value; }
		}
		/// <summary>実行タイミング</summary>
		public string ExecTiming
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_EXEC_TIMING]; }
			set { this.DataSource[Constants.FIELD_MAILDISTSETTING_EXEC_TIMING] = value; }
		}
		/// <summary>スケジュール区分</summary>
		public string ScheduleKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_SCHEDULE_KBN]; }
			set { this.DataSource[Constants.FIELD_MAILDISTSETTING_SCHEDULE_KBN] = value; }
		}
		/// <summary>スケジュール曜日</summary>
		public string ScheduleDayOfWeek
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_SCHEDULE_DAY_OF_WEEK]; }
			set { this.DataSource[Constants.FIELD_MAILDISTSETTING_SCHEDULE_DAY_OF_WEEK] = value; }
		}
		/// <summary>スケジュール日程(年)</summary>
		public int? ScheduleYear
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MAILDISTSETTING_SCHEDULE_YEAR] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_MAILDISTSETTING_SCHEDULE_YEAR];
			}
			set { this.DataSource[Constants.FIELD_MAILDISTSETTING_SCHEDULE_YEAR] = value; }
		}
		/// <summary>スケジュール日程(月)</summary>
		public int? ScheduleMonth
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MAILDISTSETTING_SCHEDULE_MONTH] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_MAILDISTSETTING_SCHEDULE_MONTH];
			}
			set { this.DataSource[Constants.FIELD_MAILDISTSETTING_SCHEDULE_MONTH] = value; }
		}
		/// <summary>スケジュール日程(日)</summary>
		public int? ScheduleDay
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MAILDISTSETTING_SCHEDULE_DAY] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_MAILDISTSETTING_SCHEDULE_DAY];
			}
			set { this.DataSource[Constants.FIELD_MAILDISTSETTING_SCHEDULE_DAY] = value; }
		}
		/// <summary>スケジュール日程(時)</summary>
		public int? ScheduleHour
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MAILDISTSETTING_SCHEDULE_HOUR] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_MAILDISTSETTING_SCHEDULE_HOUR];
			}
			set { this.DataSource[Constants.FIELD_MAILDISTSETTING_SCHEDULE_HOUR] = value; }
		}
		/// <summary>スケジュール日程(分)</summary>
		public int? ScheduleMinute
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MAILDISTSETTING_SCHEDULE_MINUTE] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_MAILDISTSETTING_SCHEDULE_MINUTE];
			}
			set { this.DataSource[Constants.FIELD_MAILDISTSETTING_SCHEDULE_MINUTE] = value; }
		}
		/// <summary>スケジュール日程(秒)</summary>
		public int? ScheduleSecond
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MAILDISTSETTING_SCHEDULE_SECOND] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_MAILDISTSETTING_SCHEDULE_SECOND];
			}
			set { this.DataSource[Constants.FIELD_MAILDISTSETTING_SCHEDULE_SECOND] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_MAILDISTSETTING_VALID_FLG] = value; }
		}
		/// <summary>削除フラグ</summary>
		public string DelFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_DEL_FLG]; }
			set { this.DataSource[Constants.FIELD_MAILDISTSETTING_DEL_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_MAILDISTSETTING_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_MAILDISTSETTING_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_MAILDISTSETTING_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_MAILDISTSETTING_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_MAILDISTSETTING_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_MAILDISTSETTING_LAST_CHANGED] = value; }
		}
		#endregion
	}
}