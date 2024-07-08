/*
=========================================================================================================
  Module      : クーポン発行スケジュールテーブルモデル (CouponScheduleModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.Coupon
{
	/// <summary>
	/// クーポン発行スケジュールテーブルモデル
	/// </summary>
	[Serializable]
	public partial class CouponScheduleModel : ModelBase<CouponScheduleModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CouponScheduleModel()
		{
			this.LastExecDate = null;
			this.TargetExtractFlg = "0";
			this.PublishQuantity = null;
			this.ExecTiming = "01";
			this.ScheduleYear = null;
			this.ScheduleMonth = null;
			this.ScheduleDay = null;
			this.ScheduleHour = null;
			this.ScheduleMinute = null;
			this.ScheduleSecond = null;
			this.ValidFlg = "ON";
			this.DateCreated = DateTime.Now;
			this.DateChanged = DateTime.Now;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public CouponScheduleModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public CouponScheduleModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>クーポン発行スケジュールID</summary>
		public string CouponScheduleId
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPONSCHEDULE_COUPON_SCHEDULE_ID]; }
			set { this.DataSource[Constants.FIELD_COUPONSCHEDULE_COUPON_SCHEDULE_ID] = value; }
		}
		/// <summary>クーポン発行スケジュール名</summary>
		public string CouponScheduleName
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPONSCHEDULE_COUPON_SCHEDULE_NAME]; }
			set { this.DataSource[Constants.FIELD_COUPONSCHEDULE_COUPON_SCHEDULE_NAME] = value; }
		}
		/// <summary>ステータス</summary>
		public string Status
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPONSCHEDULE_STATUS]; }
			set { this.DataSource[Constants.FIELD_COUPONSCHEDULE_STATUS] = value; }
		}
		/// <summary>最終付与人数</summary>
		public string LastCount
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPONSCHEDULE_LAST_COUNT]; }
			set { this.DataSource[Constants.FIELD_COUPONSCHEDULE_LAST_COUNT] = value; }
		}
		/// <summary>最終付与日時</summary>
		public DateTime? LastExecDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_COUPONSCHEDULE_LAST_EXEC_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_COUPONSCHEDULE_LAST_EXEC_DATE];
			}
			set { this.DataSource[Constants.FIELD_COUPONSCHEDULE_LAST_EXEC_DATE] = value; }
		}
		/// <summary>ターゲットID</summary>
		public string TargetId
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPONSCHEDULE_TARGET_ID]; }
			set { this.DataSource[Constants.FIELD_COUPONSCHEDULE_TARGET_ID] = value; }
		}
		/// <summary>ターゲット抽出フラグ</summary>
		public string TargetExtractFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPONSCHEDULE_TARGET_EXTRACT_FLG]; }
			set { this.DataSource[Constants.FIELD_COUPONSCHEDULE_TARGET_EXTRACT_FLG] = value; }
		}
		/// <summary>クーポン発行ID</summary>
		public string CouponId
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPONSCHEDULE_COUPON_ID]; }
			set { this.DataSource[Constants.FIELD_COUPONSCHEDULE_COUPON_ID] = value; }
		}
		/// <summary>クーポン発行枚数</summary>
		public int? PublishQuantity
		{
			get
			{
				if (this.DataSource[Constants.FIELD_COUPONSCHEDULE_PUBLISH_QUANTITY] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_COUPONSCHEDULE_PUBLISH_QUANTITY];
			}
			set { this.DataSource[Constants.FIELD_COUPONSCHEDULE_PUBLISH_QUANTITY] = value; }
		}
		/// <summary>メールテンプレートID</summary>
		public string MailId
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPONSCHEDULE_MAIL_ID]; }
			set { this.DataSource[Constants.FIELD_COUPONSCHEDULE_MAIL_ID] = value; }
		}
		/// <summary>実行タイミング</summary>
		public string ExecTiming
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPONSCHEDULE_EXEC_TIMING]; }
			set { this.DataSource[Constants.FIELD_COUPONSCHEDULE_EXEC_TIMING] = value; }
		}
		/// <summary>スケジュール区分</summary>
		public string ScheduleKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPONSCHEDULE_SCHEDULE_KBN]; }
			set { this.DataSource[Constants.FIELD_COUPONSCHEDULE_SCHEDULE_KBN] = value; }
		}
		/// <summary>スケジュール曜日</summary>
		public string ScheduleDayOfWeek
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPONSCHEDULE_SCHEDULE_DAY_OF_WEEK]; }
			set { this.DataSource[Constants.FIELD_COUPONSCHEDULE_SCHEDULE_DAY_OF_WEEK] = value; }
		}
		/// <summary>スケジュール日程(年)</summary>
		public int? ScheduleYear
		{
			get
			{
				if (this.DataSource[Constants.FIELD_COUPONSCHEDULE_SCHEDULE_YEAR] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_COUPONSCHEDULE_SCHEDULE_YEAR];
			}
			set { this.DataSource[Constants.FIELD_COUPONSCHEDULE_SCHEDULE_YEAR] = value; }
		}
		/// <summary>スケジュール日程(月)</summary>
		public int? ScheduleMonth
		{
			get
			{
				if (this.DataSource[Constants.FIELD_COUPONSCHEDULE_SCHEDULE_MONTH] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_COUPONSCHEDULE_SCHEDULE_MONTH];
			}
			set { this.DataSource[Constants.FIELD_COUPONSCHEDULE_SCHEDULE_MONTH] = value; }
		}
		/// <summary>スケジュール日程(日)</summary>
		public int? ScheduleDay
		{
			get
			{
				if (this.DataSource[Constants.FIELD_COUPONSCHEDULE_SCHEDULE_DAY] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_COUPONSCHEDULE_SCHEDULE_DAY];
			}
			set { this.DataSource[Constants.FIELD_COUPONSCHEDULE_SCHEDULE_DAY] = value; }
		}
		/// <summary>スケジュール日程(時)</summary>
		public int? ScheduleHour
		{
			get
			{
				if (this.DataSource[Constants.FIELD_COUPONSCHEDULE_SCHEDULE_HOUR] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_COUPONSCHEDULE_SCHEDULE_HOUR];
			}
			set { this.DataSource[Constants.FIELD_COUPONSCHEDULE_SCHEDULE_HOUR] = value; }
		}
		/// <summary>スケジュール日程(分)</summary>
		public int? ScheduleMinute
		{
			get
			{
				if (this.DataSource[Constants.FIELD_COUPONSCHEDULE_SCHEDULE_MINUTE] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_COUPONSCHEDULE_SCHEDULE_MINUTE];
			}
			set { this.DataSource[Constants.FIELD_COUPONSCHEDULE_SCHEDULE_MINUTE] = value; }
		}
		/// <summary>スケジュール日程(秒)</summary>
		public int? ScheduleSecond
		{
			get
			{
				if (this.DataSource[Constants.FIELD_COUPONSCHEDULE_SCHEDULE_SECOND] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_COUPONSCHEDULE_SCHEDULE_SECOND];
			}
			set { this.DataSource[Constants.FIELD_COUPONSCHEDULE_SCHEDULE_SECOND] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPONSCHEDULE_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_COUPONSCHEDULE_VALID_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_COUPONSCHEDULE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_COUPONSCHEDULE_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_COUPONSCHEDULE_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_COUPONSCHEDULE_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_COUPONSCHEDULE_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_COUPONSCHEDULE_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
