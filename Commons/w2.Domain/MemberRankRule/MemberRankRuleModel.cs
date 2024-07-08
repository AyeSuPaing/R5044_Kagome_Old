/*
=========================================================================================================
  Module      : 会員ランク付与ルールモデル (MemberRankRuleModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.MemberRankRule
{
	/// <summary>
	/// 会員ランク付与ルールモデル
	/// </summary>
	[Serializable]
	public partial class MemberRankRuleModel : ModelBase<MemberRankRuleModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public MemberRankRuleModel()
		{
			this.MemberRankRuleId = "";
			this.MemberRankRuleName = "";
			this.Status = "";
			this.LastCount = "";
			this.LastExecDate = null;
			this.TargetExtractType = Constants.FLG_MEMBERRANKRULE_TARGET_EXTRACT_TYPE_DURING;
			this.TargetExtractStart = null;
			this.TargetExtractEnd = null;
			this.TargetExtractDaysAgo = null;
			this.TargetExtractTotalPriceFrom = null;
			this.TargetExtractTotalPriceTo = null;
			this.TargetExtractTotalCountFrom = null;
			this.TargetExtractTotalCountTo = null;
			this.TargetExtractOldRankFlg = Constants.FLG_MEMBERRANKRULE_TARGET_EXTRACT_OLD_RANK_FLG_OFF;
			this.RankChangeType = Constants.FLG_MEMBERRANKRULE_RANK_CHANGE_TYPE_UP;
			this.RankChangeRankId = "";
			this.MailId = "";
			this.ExecTiming = Constants.FLG_MEMBERRANKRULE_EXEC_TIMING_MANUAL;
			this.ScheduleKbn = "";
			this.ScheduleDayOfWeek = "";
			this.ScheduleYear = null;
			this.ScheduleMonth = null;
			this.ScheduleDay = null;
			this.ScheduleHour = null;
			this.ScheduleMinute = null;
			this.ScheduleSecond = null;
			this.ValidFlg = Constants.FLG_MEMBERRANKRULE_VALID_FLG_VALID;
			this.LastChanged = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public MemberRankRuleModel(DataRowView source) : this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public MemberRankRuleModel(Hashtable source) : this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>ランク付与ルールID</summary>
		public string MemberRankRuleId
		{
			get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_ID]; }
			set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_ID] = value; }
		}
		/// <summary>ランク付与ルール名</summary>
		public string MemberRankRuleName
		{
			get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_NAME]; }
			set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_NAME] = value; }
		}
		/// <summary>ステータス</summary>
		public string Status
		{
			get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_STATUS]; }
			set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_STATUS] = value; }
		}
		/// <summary>最終付与人数</summary>
		public string LastCount
		{
			get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_LAST_COUNT]; }
			set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_LAST_COUNT] = value; }
		}
		/// <summary>最終付与日時</summary>
		public DateTime? LastExecDate
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MEMBERRANKRULE_LAST_EXEC_DATE] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_MEMBERRANKRULE_LAST_EXEC_DATE];
			}
			set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_LAST_EXEC_DATE] = value; }
		}
		/// <summary>抽出条件集計期間指定</summary>
		public string TargetExtractType
		{
			get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TYPE]; }
			set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TYPE] = value; }
		}
		/// <summary>抽出条件集計期間開始日</summary>
		public DateTime? TargetExtractStart
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_START] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_START];
			}
			set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_START] = value; }
		}
		/// <summary>抽出条件集計期間終了日</summary>
		public DateTime? TargetExtractEnd
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_END] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_END];
			}
			set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_END] = value; }
		}
		/// <summary>抽出条件集計期間前日指定</summary>
		public int? TargetExtractDaysAgo
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_DAYS_AGO] == DBNull.Value)
					return null;
				return (int?)this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_DAYS_AGO];
			}
			set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_DAYS_AGO] = value; }
		}
		/// <summary>抽出条件合計購入金額範囲(From)</summary>
		public decimal? TargetExtractTotalPriceFrom
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_PRICE_FROM]
					== DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_PRICE_FROM];
			}
			set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_PRICE_FROM] = value; }
		}
		/// <summary>抽出条件合計購入金額範囲(To)</summary>
		public decimal? TargetExtractTotalPriceTo
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_PRICE_TO]
					== DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_PRICE_TO];
			}
			set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_PRICE_TO] = value; }
		}
		/// <summary>抽出条件合計購入回数範囲(From)</summary>
		public int? TargetExtractTotalCountFrom
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_COUNT_FROM]
					== DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_COUNT_FROM];
			}
			set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_COUNT_FROM] = value; }
		}
		/// <summary>抽出条件合計購入回数範囲(To)</summary>
		public int? TargetExtractTotalCountTo
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_COUNT_TO]
					== DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_COUNT_TO];
			}
			set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_COUNT_TO] = value; }
		}
		/// <summary>抽出時の旧ランク情報抽出判定</summary>
		public string TargetExtractOldRankFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_OLD_RANK_FLG]; }
			set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_OLD_RANK_FLG] = value; }
		}
		/// <summary>ランク付与方法</summary>
		public string RankChangeType
		{
			get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_RANK_CHANGE_TYPE]; }
			set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_RANK_CHANGE_TYPE] = value; }
		}
		/// <summary>指定付与ランクID</summary>
		public string RankChangeRankId
		{
			get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_RANK_CHANGE_RANK_ID]; }
			set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_RANK_CHANGE_RANK_ID] = value; }
		}
		/// <summary>メールテンプレートID</summary>
		public string MailId
		{
			get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_MAIL_ID]; }
			set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_MAIL_ID] = value; }
		}
		/// <summary>実行タイミング</summary>
		public string ExecTiming
		{
			get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_EXEC_TIMING]; }
			set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_EXEC_TIMING] = value; }
		}
		/// <summary>スケジュール区分</summary>
		public string ScheduleKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_KBN]; }
			set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_KBN] = value; }
		}
		/// <summary>スケジュール曜日</summary>
		public string ScheduleDayOfWeek
		{
			get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_DAY_OF_WEEK]; }
			set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_DAY_OF_WEEK] = value; }
		}
		/// <summary>スケジュール日程(年)</summary>
		public int? ScheduleYear
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_YEAR] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_YEAR];
			}
			set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_YEAR] = value; }
		}
		/// <summary>スケジュール日程(月)</summary>
		public int? ScheduleMonth
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_MONTH] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_MONTH];
			}
			set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_MONTH] = value; }
		}
		/// <summary>スケジュール日程(日)</summary>
		public int? ScheduleDay
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_DAY] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_DAY];
			}
			set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_DAY] = value; }
		}
		/// <summary>スケジュール日程(時)</summary>
		public int? ScheduleHour
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_HOUR] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_HOUR];
			}
			set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_HOUR] = value; }
		}
		/// <summary>スケジュール日程(分)</summary>
		public int? ScheduleMinute
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_MINUTE] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_MINUTE];
			}
			set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_MINUTE] = value; }
		}
		/// <summary>スケジュール日程(秒)</summary>
		public int? ScheduleSecond
		{
			get
			{
				if (this.DataSource[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_SECOND] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_SECOND];
			}
			set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_SCHEDULE_SECOND] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_VALID_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_MEMBERRANKRULE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_MEMBERRANKRULE_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_MEMBERRANKRULE_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_MEMBERRANKRULE_LAST_CHANGED] = value; }
		}
		#endregion
	}
}