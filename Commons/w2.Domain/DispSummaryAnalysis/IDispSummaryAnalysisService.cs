/*
=========================================================================================================
  Module      : サマリ分析結果テーブルサービスのインターフェース (IDispSummaryAnalysisService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using w2.Common.Sql;
using w2.Domain.User.Helper;

namespace w2.Domain.DispSummaryAnalysis
{
	/// <summary>
	/// サマリ分析結果テーブルサービスのインターフェース
	/// </summary>
	public interface IDispSummaryAnalysisService : IService
	{
		/// <summary>
		/// CPMレポート用日次レポート取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="tgtYear">対象年</param>
		/// <param name="tgtMonth">対象月</param>
		/// <returns>モデル</returns>
		CpmClusterReport[] GetDailyReportForCpmReport(string deptId, int tgtYear, int tgtMonth);

		/// <summary>
		/// CPMレポート用月次レポート取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="tgtYear">対象年</param>
		/// <returns>モデル</returns>
		CpmClusterReport[] GetMonthlyReportForCpmReport(string deptId, int tgtYear);

		/// <summary>
		/// CPMレポート登録
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="targetDate">対象日</param>
		/// <param name="report">CPMクラスタレポートアイテム</param>
		void RegisterForCpmReport(string deptId, DateTime targetDate, CpmClusterReport report);

		/// <summary>
		/// Insert order count for workflow setting
		/// </summary>
		/// <param name="deptId">Dept id</param>
		/// <param name="summaryKbn">Summary kbn</param>
		/// <param name="valueName">Value name</param>
		/// <param name="count">Count</param>
		/// <param name="accessor">Sql Accessor</param>
		void InsertOrderCountForWorkflowSetting(
			string deptId,
			string summaryKbn,
			string valueName,
			int count,
			SqlAccessor accessor);

		/// <summary>
		/// Get order count for workflow setting
		/// </summary>
		/// <param name="deptId">Dept id</param>
		/// <param name="summaryKbn">Summary kbn</param>
		/// <param name="valueName">Value name</param>
		/// <returns>Order count</returns>
		long GetOrderCountForWorkflowSetting(
			string deptId,
			string summaryKbn,
			string valueName);

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
		/// <param name="numberOfDays">集計日数</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Number of page scroring sales</returns>
		Dictionary<string, int> GetCountPageScoringSales(
			string deptId,
			string scoringSaleId,
			string topUrl,
			string questionUrl,
			string resultUrl,
			string subDirectoryCommon,
			DateTime startDate,
			int numberOfDays,
			SqlAccessor accessor = null);
	}
}
