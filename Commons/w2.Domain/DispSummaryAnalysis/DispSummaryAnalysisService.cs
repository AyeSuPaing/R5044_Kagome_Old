/*
=========================================================================================================
  Module      : サマリ分析結果テーブルサービス (DispSummaryAnalysisService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using w2.Common.Sql;
using w2.Domain.User.Helper;

namespace w2.Domain.DispSummaryAnalysis
{
	/// <summary>
	/// サマリ分析結果テーブルサービス
	/// </summary>
	public class DispSummaryAnalysisService : ServiceBase, IDispSummaryAnalysisService
	{
		#region +GetDailyReportForCpmReport CPMレポート用日次レポート取得
		/// <summary>
		/// CPMレポート用日次レポート取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="tgtYear">対象年</param>
		/// <param name="tgtMonth">対象月</param>
		/// <returns>モデル</returns>
		public CpmClusterReport[] GetDailyReportForCpmReport(string deptId, int tgtYear, int tgtMonth)
		{
			var yesterday = new DateTime(tgtYear, tgtMonth, 1).AddDays(-1);
			var models = GetDailyReport(deptId, "cpm_report", tgtYear, tgtMonth);

			var reportList = new List<CpmClusterReport>();
			for (var day = 1; day <= new DateTime(tgtYear, tgtMonth, 1).AddMonths(1).AddDays(-1).Day; day++)
			{
				reportList.Add(new CpmClusterReport(
					models.Where(m => int.Parse(m.TgtDay) == day).Select(m => new CpmClusterReportItem(m)))
				{
					No = day
				});
			}

			// 伸び率セット
			var beforeModels = GetReportDay(deptId, "cpm_report", yesterday.Year, yesterday.Month, yesterday.Day);
			var before = new CpmClusterReport(beforeModels.Select((m => new CpmClusterReportItem(m))));
			SetGrowthRate(reportList, before);

			return reportList.ToArray();
		}
		#endregion

		#region +GetMonthlyReportForCpmReport CPMレポート用月次レポート取得
		/// <summary>
		/// CPMレポート用月次レポート取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="tgtYear">対象年</param>
		/// <returns>モデル</returns>
		public CpmClusterReport[] GetMonthlyReportForCpmReport(string deptId, int tgtYear)
		{
			var lastMonth = new DateTime(tgtYear, 1, 1).AddMonths(-1);
			var models = GetMonthlyReport(deptId, "cpm_report", tgtYear);

			var reportList = new List<CpmClusterReport>();
			for (var month = 1; month <= 12; month++)
			{
				reportList.Add(new CpmClusterReport(
					models.Where(m => int.Parse(m.TgtMonth) == month).Select(m => new CpmClusterReportItem(m)))
				{
					No = month
				});
			}

			// 伸び率セット
			var beforeModels = GetReportMonth(deptId, "cpm_report", lastMonth.Year, lastMonth.Month);
			var before = new CpmClusterReport(beforeModels.Select((m => new CpmClusterReportItem(m))));
			SetGrowthRate(reportList, before);

			return reportList.ToArray();
		}
		#endregion

		#region -SetGrowthRate 伸び率セット
		/// <summary>
		/// 伸び率セット
		/// </summary>
		/// <param name="reportList">レポートリスト</param>
		/// <param name="before">直前のレポート</param>
		private void SetGrowthRate(List<CpmClusterReport> reportList, CpmClusterReport before)
		{
			var itemsList = new List<List<CpmClusterReportItem>> { before.Items };
			itemsList.AddRange(reportList.Select(report => report.Items));

			List<CpmClusterReportItem> beforeItems = null;
			foreach (var items in itemsList)
			{
				if ((beforeItems != null)
					&& (beforeItems.Count != 0))
				{
					foreach (var item in items)
					{
						var beforeItem = beforeItems.FirstOrDefault(tmp => item.CpmClusterId == tmp.CpmClusterId);
						if (beforeItem == null) continue;
						item.GrowthCount = item.Count - beforeItem.Count;
						if (item.GrowthCount == 0)
						{
							item.GrowthRate = 0;
						}
						else if (beforeItem.Count != 0)
						{
							item.GrowthRate = (item.GrowthCount != 0) ? (int)(item.GrowthCount * 100 / beforeItem.Count) : 0;
						}
					}
				}
				beforeItems = items;
			}
		}
		#endregion

		#region -GetDailyReport 日次レポート取得
		/// <summary>
		/// 日次レポート取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="summaryKbn">サマリ区分</param>
		/// <param name="tgtYear">対象年</param>
		/// <param name="tgtMonth">対象月</param>
		/// <returns>モデル</returns>
		private DispSummaryAnalysisModel[] GetDailyReport(string deptId, string summaryKbn, int tgtYear, int tgtMonth)
		{
			using (var repository = new DispSummaryAnalysisRepository())
			{
				var models = repository.GetDailyReport(deptId, summaryKbn, tgtYear, tgtMonth);
				return models;
			}
		}
		#endregion

		#region -GetMonthlyReport 月次レポート取得（日レポートで最新のものを月のレポートとする）
		/// <summary>
		/// 月次レポート取得（日レポートで最新のものを月のレポートとする）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="summaryKbn">サマリ区分</param>
		/// <param name="tgtYear">対象年</param>
		/// <returns>モデル</returns>
		private DispSummaryAnalysisModel[] GetMonthlyReport(string deptId, string summaryKbn, int tgtYear)
		{
			using (var repository = new DispSummaryAnalysisRepository())
			{
				var models = repository.GetMonthlyReport(deptId, summaryKbn, tgtYear);
				return models;
			}
		}
		#endregion

		#region -GetReportDay レポート取得（日）
		/// <summary>
		/// レポート取得（日）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="summaryKbn">サマリ区分</param>
		/// <param name="tgtYear">対象年</param>
		/// <param name="tgtMonth">対象月</param>
		/// <param name="tgtDay">対象日</param>
		/// <returns>モデル</returns>
		private DispSummaryAnalysisModel[] GetReportDay(string deptId, string summaryKbn, int tgtYear, int tgtMonth, int tgtDay)
		{
			using (var repository = new DispSummaryAnalysisRepository())
			{
				var models = repository.GetReportDay(deptId, summaryKbn, tgtYear, tgtMonth, tgtDay);
				return models;
			}
		}
		#endregion

		#region -GetReportMonth レポート取得（月）（日レポートで最新のものを月のレポートとする）
		/// <summary>
		///レポート取得（月）（日レポートで最新のものを月のレポートとする）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="summaryKbn">サマリ区分</param>
		/// <param name="tgtYear">対象年</param>
		/// <param name="tgtMonth">対象月</param>
		/// <returns>モデル</returns>
		private DispSummaryAnalysisModel[] GetReportMonth(string deptId, string summaryKbn, int tgtYear, int tgtMonth)
		{
			using (var repository = new DispSummaryAnalysisRepository())
			{
				var models = repository.GetReportMonth(deptId, summaryKbn, tgtYear, tgtMonth);
				return models;
			}
		}
		#endregion

		#region +RegisterForCpmReport CPMレポート登録
		/// <summary>
		/// CPMレポート登録
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="targetDate">対象日</param>
		/// <param name="report">CPMクラスタレポートアイテム</param>
		public void RegisterForCpmReport(string deptId, DateTime targetDate, CpmClusterReport report)
		{
			foreach (var item in report.Items
				.Where(item => string.IsNullOrEmpty(item.CpmClusterId) == false))
			{
				var model = new DispSummaryAnalysisModel
				{
					DeptId = deptId,
					SummaryKbn = "cpm_report",
					TgtYear = targetDate.ToString("yyyy"),
					TgtMonth = targetDate.ToString("MM"),
					TgtDay = targetDate.ToString("dd"),
					ValueName = item.CpmClusterId,
					Counts = item.Count ?? 0,
				};
				Register(model);
			}
		}

		#endregion

		#region -Merge マージ
		/// <summary>
		/// マージ
		/// </summary>
		/// <param name="model">モデル</param>
		private void Register(DispSummaryAnalysisModel model)
		{
			using (var repository = new DispSummaryAnalysisRepository())
			{
				repository.Merge(model);
			}
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
		/// <param name="accessor">Sql Accessor</param>
		public void InsertOrderCountForWorkflowSetting(
			string deptId,
			string summaryKbn,
			string valueName,
			int count,
			SqlAccessor accessor)
		{
			using (var repository = new DispSummaryAnalysisRepository(accessor))
			{
				repository.InsertOrderCountForWorkflowSetting(
					deptId,
					summaryKbn,
					valueName,
					count);
			}
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
		public long GetOrderCountForWorkflowSetting(
			string deptId,
			string summaryKbn,
			string valueName)
		{
			using (var repository = new DispSummaryAnalysisRepository())
			{
				var count = repository.GetOrderCountForWorkflowSetting(
					deptId,
					summaryKbn,
					valueName);
				return count;
			}
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
		/// <param name="numberOfDays">集計日数</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Number of page scroring sales</returns>
		public Dictionary<string, int> GetCountPageScoringSales(
			string deptId,
			string scoringSaleId,
			string topUrl,
			string questionUrl,
			string resultUrl,
			string subDirectoryCommon,
			DateTime startDate,
			int numberOfDays,
			SqlAccessor accessor = null)
		{
			using (var repository = new DispSummaryAnalysisRepository(accessor))
			{
				return repository.GetCountPageScoringSales(
					deptId,
					scoringSaleId,
					topUrl,
					questionUrl,
					resultUrl,
					subDirectoryCommon,
					startDate,
					numberOfDays);
			}
		}
		#endregion
	}
}
