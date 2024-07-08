/*
=========================================================================================================
  Module      : サマリ分析結果テーブルリポジトリ (DispSummaryAnalysisRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.DispSummaryAnalysis
{
	/// <summary>
	/// サマリ分析結果テーブルリポジトリ
	/// </summary>
	public class DispSummaryAnalysisRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "DispSummaryAnalysis";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public DispSummaryAnalysisRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public DispSummaryAnalysisRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +GetDailyReport 日次レポート取得
		/// <summary>
		/// 日次レポート取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="summaryKbn">サマリ区分</param>
		/// <param name="tgtYear">対象年</param>
		/// <param name="tgtMonth">対象月</param>
		/// <returns>モデル</returns>
		public DispSummaryAnalysisModel[] GetDailyReport(string deptId, string summaryKbn, int tgtYear, int tgtMonth)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_DISPSUMMARYANALYSIS_DEPT_ID, deptId},
				{Constants.FIELD_DISPSUMMARYANALYSIS_SUMMARY_KBN, summaryKbn},
				{Constants.FIELD_DISPSUMMARYANALYSIS_TGT_YEAR, tgtYear.ToString("D4")},
				{Constants.FIELD_DISPSUMMARYANALYSIS_TGT_MONTH, tgtMonth.ToString("D2")},
			};
			var dv = Get(XML_KEY_NAME, "GetDailyReport", ht);
			return dv.Cast<DataRowView>().Select(drv => new DispSummaryAnalysisModel(drv)).ToArray();
		}
		#endregion

		#region +GetMonthlyReport 月次レポート取得（日レポートで最新のものを月のレポートとする）
		/// <summary>
		/// 月次レポート取得（日レポートで最新のものを月のレポートとする）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="summaryKbn">サマリ区分</param>
		/// <param name="tgtYear">対象年</param>
		/// <returns>モデル</returns>
		public DispSummaryAnalysisModel[] GetMonthlyReport(string deptId, string summaryKbn, int tgtYear)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_DISPSUMMARYANALYSIS_DEPT_ID, deptId},
				{Constants.FIELD_DISPSUMMARYANALYSIS_SUMMARY_KBN, summaryKbn},
				{Constants.FIELD_DISPSUMMARYANALYSIS_TGT_YEAR, tgtYear.ToString("D4")},
			};
			var dv = Get(XML_KEY_NAME, "GetMonthlyReport", ht);
			return dv.Cast<DataRowView>().Select(drv => new DispSummaryAnalysisModel(drv)).ToArray();
		}
		#endregion

		#region +GetReportDay レポート取得（日）
		/// <summary>
		/// レポート取得（日）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="summaryKbn">サマリ区分</param>
		/// <param name="tgtYear">対象年</param>
		/// <param name="tgtMonth">対象月</param>
		/// <param name="tgtDay">対象日</param>
		/// <returns>モデル</returns>
		public DispSummaryAnalysisModel[] GetReportDay(string deptId, string summaryKbn, int tgtYear, int tgtMonth, int tgtDay)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_DISPSUMMARYANALYSIS_DEPT_ID, deptId},
				{Constants.FIELD_DISPSUMMARYANALYSIS_SUMMARY_KBN, summaryKbn},
				{Constants.FIELD_DISPSUMMARYANALYSIS_TGT_YEAR, tgtYear.ToString("D4")},
				{Constants.FIELD_DISPSUMMARYANALYSIS_TGT_MONTH, tgtMonth.ToString("D2")},
				{Constants.FIELD_DISPSUMMARYANALYSIS_TGT_DAY, tgtDay.ToString("D2")},
			};
			var dv = Get(XML_KEY_NAME, "GetReportDay", ht);
			return dv.Cast<DataRowView>().Select(drv => new DispSummaryAnalysisModel(drv)).ToArray();
		}
		#endregion

		#region +GetReportMonth レポート取得（月）（日レポートで最新のものを月のレポートとする）
		/// <summary>
		/// レポート取得（月）（日レポートで最新のものを月のレポートとする）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="summaryKbn">サマリ区分</param>
		/// <param name="tgtYear">対象年</param>
		/// <param name="tgtMonth">対象月</param>
		/// <returns>モデル</returns>
		public DispSummaryAnalysisModel[] GetReportMonth(string deptId, string summaryKbn, int tgtYear, int tgtMonth)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_DISPSUMMARYANALYSIS_DEPT_ID, deptId},
				{Constants.FIELD_DISPSUMMARYANALYSIS_SUMMARY_KBN, summaryKbn},
				{Constants.FIELD_DISPSUMMARYANALYSIS_TGT_YEAR, tgtYear.ToString("D4")},
				{Constants.FIELD_DISPSUMMARYANALYSIS_TGT_MONTH, tgtMonth.ToString("D2")},
			};
			var dv = Get(XML_KEY_NAME, "GetReportMonth", ht);
			return dv.Cast<DataRowView>().Select(drv => new DispSummaryAnalysisModel(drv)).ToArray();
		}
		#endregion
		

		#region +Merge マージ
		/// <summary>
		/// マージ
		/// </summary>
		/// <param name="model">モデル</param>
		public void Merge(DispSummaryAnalysisModel model)
		{
			Exec(XML_KEY_NAME, "Merge", model.DataSource);
		}
		#endregion

		#region +InsertOrderCountForWorkflowSetting
		/// <summary>
		/// Insert order count for workflow setting
		/// </summary>
		/// <param name="deptId">Dept id</param>
		/// <param name="summaryKbn">Summary kbn</param>
		/// <param name="valueName">Value name</param>
		/// <param name="count">Count</param>
		internal void InsertOrderCountForWorkflowSetting(
			string deptId,
			string summaryKbn,
			string valueName,
			int count)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_DISPSUMMARYANALYSIS_DEPT_ID, deptId },
				{ Constants.FIELD_DISPSUMMARYANALYSIS_SUMMARY_KBN, summaryKbn },
				{ Constants.FIELD_DISPSUMMARYANALYSIS_VALUE_NAME, valueName },
				{ Constants.FIELD_DISPSUMMARYANALYSIS_COUNTS, count },
			};
			Exec(XML_KEY_NAME, "InsertOrderCountForWorkflowSetting", input);
		}
		#endregion

		#region +GetOrderCountForWorkflowSetting
		/// <summary>
		/// Get order count for workflow setting
		/// </summary>
		/// <param name="deptId">Dept id</param>
		/// <param name="summaryKbn">Summary kbn</param>
		/// <param name="valueName">Value name</param>
		/// <returns>Order count</returns>
		internal long GetOrderCountForWorkflowSetting(
			string deptId,
			string summaryKbn,
			string valueName)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_DISPSUMMARYANALYSIS_DEPT_ID, deptId },
				{ Constants.FIELD_DISPSUMMARYANALYSIS_VALUE_NAME, valueName },
				{ Constants.FIELD_DISPSUMMARYANALYSIS_SUMMARY_KBN, summaryKbn }
			};
			var dv = Get(XML_KEY_NAME, "GetOrderCountForWorkflowSetting", input);
			return (dv.Count != 0) ? (long)dv[0][0] : 0;
		}
		#endregion

		#region +GetCountPageScoringSales
		/// <summary>
		/// Get count page scoring sales
		/// </summary>
		/// <param name="deptId">Dept id</param>
		/// <param name="scoringSaleId">Scoring sale id</param>
		/// <param name="topUrl">Top url</param>
		/// <param name="questionUrl">Question url</param>
		/// <param name="resultUrl">Result url</param>
		/// <param name="subDirectoryCommon">Sub directory common</param>
		/// <param name="startDate">指定期間(開始)</param>
		/// <param name="numberOfDays">集計日数></param>
		/// <returns>Number of page scroring sales</returns>
		public Dictionary<string, int> GetCountPageScoringSales(
			string deptId,
			string scoringSaleId,
			string topUrl,
			string questionUrl,
			string resultUrl,
			string subDirectoryCommon,
			DateTime startDate,
			int numberOfDays)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_DISPSUMMARYANALYSIS_DEPT_ID, deptId },
				{ Constants.FIELD_SCORINGSALE_SCORING_SALE_ID, scoringSaleId },
				{ "top_url", topUrl },
				{ "question_url", questionUrl },
				{ "result_url", resultUrl },
				{ "sub_directory_common", subDirectoryCommon },
				{ "start_date", startDate },
				{ "number_of_days", numberOfDays },
			};

			var data = Get(XML_KEY_NAME, "GetCountPageScoringSales", input);
			var result = data.Cast<DataRowView>().ToDictionary(
				row => (string)row[Constants.FIELD_DISPSUMMARYANALYSIS_VALUE_NAME],
				row => Convert.ToInt32(row["number"]));
			return result;
		}
		#endregion
	}
}
