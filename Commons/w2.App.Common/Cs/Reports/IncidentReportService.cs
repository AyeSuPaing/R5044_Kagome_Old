/*
=========================================================================================================
  Module      : インシデントレポートサービス(IncidentReportService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using w2.Common.Sql;
using w2.App.Common.Cs.CsOperator;

namespace w2.App.Common.Cs.Reports
{
	/// <summary>
	/// インシデントレポートサービス
	/// </summary>
	public class IncidentReportService : ReportServiceBase
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="repository">リポジトリ</param>
		public IncidentReportService(IncidentReportRepository repository)
			: base(repository)
		{
		}
		#endregion

		#region +GetStatusCount ステータス毎の現在の件数取得
		/// <summary>
		/// ステータス毎の現在の件数取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>モデル</returns>
		public IncidentCountByStatusModel GetStatusCount(string deptId)
		{
			var dv = ((IncidentReportRepository)this.Repository).GetStatusCount(deptId);
			var model = new IncidentCountByStatusModel(dv[0]);
			return model;
		}
		#endregion

		#region -GetActionCountByTerm 期間内のアクション件数取得
		/// <summary>
		/// 期間内のアクション件数取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="beginDate">開始日</param>
		/// <param name="endDate">終了日</param>
		/// <returns>結果</returns>
		public IncidentActionCountByTermModel GetActionCountByTerm(string deptId, DateTime beginDate, DateTime endDate)
		{
			var dv = ((IncidentReportRepository)this.Repository).GetActionCountByTerm(deptId, beginDate, DateTime.Parse(endDate.ToString("yyyy/MM/dd 23:59:59.997")));
			var model = new IncidentActionCountByTermModel(dv[0]);
			return model;
		}
		#endregion

		#region +GetVocReport VOC毎レポート取得
		/// <summary>
		/// VOC毎レポート取得
		/// </summary>
		/// <param name="ht">パラメタ</param>
		/// <returns>モデル</returns>
		public ReportRowModel[] GetVocReport(Hashtable ht)
		{
			var dv = ((IncidentReportRepository)this.Repository).GetVocReport(ht);
			var models = (from DataRowView drv in dv select new ReportRowModel(drv)).ToArray();
			return models;
		}
		#endregion

		#region +GetSummaryKbnReport 集計区分毎レポート取得
		/// <summary>
		/// 集計区分毎レポート取得
		/// </summary>
		/// <param name="ht">パラメタ</param>
		/// <returns>モデル</returns>
		public ReportRowModel[] GetSummaryKbnReport(Hashtable ht)
		{
			var dv = ((IncidentReportRepository)this.Repository).GetSummaryKbnReport(ht);
			var models = (from DataRowView drv in dv select new ReportRowModel(drv)).ToArray();
			return models;
		}
		#endregion
	}
}
