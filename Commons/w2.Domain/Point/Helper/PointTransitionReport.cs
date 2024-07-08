/*
=========================================================================================================
  Module      : ポイント推移レポートのためのヘルパクラス (PointTransitionReport.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Domain.Helper;
using w2.Domain.Helper.Attribute;

namespace w2.Domain.Point.Helper
{
	#region 列挙体
	/// <summary>
	/// レポート種別
	/// </summary>
	public enum ReportType
	{
		/// <summary>日別</summary>
		Day,
		/// <summary>月別</summary>
		Month
	}
	#endregion

	/// <summary>
	/// ポイント推移レポートのためのヘルパクラス
	/// </summary>
	/// <remarks>本クラスはinternalであり、外部への公開はServiceクラスを介す</remarks>
	internal class PointTransitionReport
	{
		#region ~Search ポイント推移レポート検索
		/// <summary>
		/// ポイント推移レポート検索
		/// </summary>
		/// <param name="cond">ポイント推移レポート検索条件クラス</param>
		/// <returns>検索結果</returns>
		internal PointTransitionReportResult[] Search(PointTransitionReportCondition cond)
		{
			using (var rep = new PointRepository())
			{
				return cond.ReportType == ReportType.Day ? rep.PointTransitionReportDay(cond) : rep.PointTransitionReportMonth(cond);
			}
		}
		#endregion
	}

	/// <summary>
	/// ポイント推移レポート検索条件クラス
	/// </summary>
	public class PointTransitionReportCondition : BaseDbMapModel
	{
		/*
		 * 検索条件となるものをプロパティで持つ
		 * 各プロパティはDbMapName属性を利用して検索クエリのバインドパラメータ名とマップ
		 */
		#region プロパティ
		/// <summary>
		/// 識別ID
		/// </summary>
		[DbMapName("dept_id")]
		public string DeptId { get; set; }

		/// <summary>
		/// 対象年
		/// </summary>
		[DbMapName("year")]
		public string Year { get; set; }

		/// <summary>
		/// 対象月
		/// </summary>
		[DbMapName("month")]
		public string Month { get; set; }

		/// <summary>
		/// レポート種別（月別or日別）
		/// </summary>
		public ReportType ReportType { get; set; }
		#endregion
	}

	/// <summary>
	/// レポート結果クラス(DBモデルではない！)
	/// </summary>
	[Serializable]
	public class PointTransitionReportResult : ModelBase<PointTransitionReportResult>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		private PointTransitionReportResult()
		{

		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PointTransitionReportResult(PointTransitionReportResult model, ReportType reportType)
			: this(model.DataSource, reportType)
		{

		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PointTransitionReportResult(DataRowView source, ReportType reportType)
			: this()
		{
			this.DataSource = source.ToHashtable();
			this.ReportType = reportType;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PointTransitionReportResult(Hashtable source, ReportType reportType)
			: this()
		{
			this.DataSource = source;
			this.ReportType = reportType;
		}
		#endregion

		#region プロパティ
		/// <summary>レポート種別</summary>
		public ReportType ReportType
		{
			get { return (ReportType)this.DataSource["reporttype"]; }
			private set { this.DataSource["reporttype"] = value; }
		}
		/// <summary>対象年</summary>
		public int TargetYear
		{
			get { return (int)this.DataSource["tgt_year"]; }
			set { this.DataSource["tgt_year"] = value; }
		}
		/// <summary>対象月</summary>
		public int TargetMonth
		{
			get { return (int)this.DataSource["tgt_month"]; }
			set { this.DataSource["tgt_month"] = value; }
		}
		/// <summary>対象日</summary>
		public int TargetDay
		{
			get { return (int)this.DataSource["tgt_day"]; }
			set { this.DataSource["tgt_day"] = value; }
		}
		/// <summary>ポイント発行数</summary>
		public decimal? AddPoint
		{
			//HACK 変換で怒られるのでちょっと工夫
			get { return this.DataSource["add_point"] == DBNull.Value ? null : (decimal?)decimal.Parse(this.DataSource["add_point"].ToString()); }
			set { this.DataSource["add_point"] = value; }
		}
		/// <summary>ポイント発行人数</summary>
		public decimal? AddPointCount
		{
			get { return this.DataSource["add_count"] == DBNull.Value ? null : (decimal?)decimal.Parse(this.DataSource["add_count"].ToString()); }
			set { this.DataSource["add_count"] = value; }
		}
		/// <summary>ポイント回収数</summary>
		public decimal? UsePoint
		{
			get { return this.DataSource["use_point"] == DBNull.Value ? null : (decimal?)decimal.Parse(this.DataSource["use_point"].ToString()); }
			set { this.DataSource["use_point"] = value; }
		}
		/// <summary>ポイント回収人数</summary>
		public decimal? UsePointCount
		{
			get { return this.DataSource["use_count"] == DBNull.Value ? null : (decimal?)decimal.Parse(this.DataSource["use_count"].ToString()); }
			set { this.DataSource["use_count"] = value; }
		}
		/// <summary>ポイント消滅数</summary>
		public decimal? ExpPoint
		{
			get { return this.DataSource["exp_point"] == DBNull.Value ? null : (decimal?)decimal.Parse(this.DataSource["exp_point"].ToString()); }
			set { this.DataSource["exp_point"] = value; }
		}
		/// <summary>ポイント消滅人数</summary>
		public decimal? ExpPointCount
		{
			get { return this.DataSource["exp_count"] == DBNull.Value ? null : (decimal?)decimal.Parse(this.DataSource["exp_count"].ToString()); }
			set { this.DataSource["exp_count"] = value; }
		}
		/// <summary>小計ポイント</summary>
		public decimal? SubtotalPoint
		{
			get { return this.DataSource["subtotal_point"] == DBNull.Value ? null : (decimal?)decimal.Parse(this.DataSource["subtotal_point"].ToString()); }
			set { this.DataSource["subtotal_point"] = value; }
		}
		/// <summary>未使用ポイント</summary>
		public decimal? UnusedPoint
		{
			get { return this.DataSource["unused_point"] == DBNull.Value ? null : (decimal?)decimal.Parse(this.DataSource["unused_point"].ToString()); }
			set { this.DataSource["unused_point"] = value; }
		}
		#endregion
	}
}
