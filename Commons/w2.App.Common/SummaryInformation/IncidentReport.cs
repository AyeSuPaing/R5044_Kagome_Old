/*
=========================================================================================================
  Module      : Incident Report (IncidentReport.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Domain.CsIncident;

namespace w2.App.Common.SummaryInformation
{
	/// <summary>
	/// Incident report
	/// </summary>
	[Serializable]
	public class IncidentReport
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="reports">Reports</param>
		/// <param name="operatorId">Operator Id</param>
		public IncidentReport(string deptId,string operatorId)
		{
			this.OperatorId = operatorId;
			this.DeptId = deptId;
			InitializeProperties();
		}

		/// <summary>
		/// Create report
		/// </summary>
		/// <returns>Report</returns>
		public IncidentReport CreateReport()
		{
			var includeUnassigned =
				(Constants.SETTING_CSTOP_GROUP_TASK_DISPLAY_MODE == Constants.GroupTaskDisplayModeType.IncludeUnassigned);

			var counts = new CsIncidentService().CountTask(
				this.DeptId,
				this.OperatorId,
				includeUnassigned);

			var incidents = includeUnassigned
				? counts.Second
				: counts.First;
			SetReports(incidents);
			return this;
		}

		/// <summary>
		/// Initialize properties
		/// </summary>
		private void InitializeProperties()
		{
			this.StatusNoneCount = 0;
			this.StatusActiveCount = 0;
			this.StatusSuspendCount = 0;
			this.StatusUrgentCount = 0;
			this.StatusCompleteCount = 0;
		}

		/// <summary>
		/// Set reports
		/// </summary>
		/// <param name="reports">Reports</param>
		private void SetReports(CsIncidentModel[] reports)
		{
			if (reports == null) return;

			foreach (var report in reports)
			{
				if (report.OperatorId != this.OperatorId) continue;

				switch (report.Status)
				{
					case Constants.FLG_CSINCIDENT_STATUS_NONE:
						this.StatusNoneCount = report.EX_SearchCount;
						break;

					case Constants.FLG_CSINCIDENT_STATUS_ACTIVE:
						this.StatusActiveCount = report.EX_SearchCount;
						break;

					case Constants.FLG_CSINCIDENT_STATUS_SUSPEND:
						this.StatusSuspendCount = report.EX_SearchCount;
						break;

					case Constants.FLG_CSINCIDENT_STATUS_URGENT:
						this.StatusUrgentCount = report.EX_SearchCount;
						break;

					case Constants.FLG_CSINCIDENT_STATUS_COMPLETE:
						this.StatusCompleteCount = report.EX_SearchCount;
						break;
				}
			}
		}

		/// <summary>Dept id</summary>
		private string DeptId { get; set; }
		/// <summary>Operator id</summary>
		private string OperatorId { get; set; }
		/// <summary>Status none count</summary>
		public int StatusNoneCount { get; set; }
		/// <summary>Status active count</summary>
		public int StatusActiveCount { get; set; }
		/// <summary>Status suspend count</summary>
		public int StatusSuspendCount { get; set; }
		/// <summary>Status urgent count</summary>
		public int StatusUrgentCount { get; set; }
		/// <summary>Status complete count</summary>
		public int StatusCompleteCount { get; set; }
		/// <summary>Has status urgent count</summary>
		public bool HasStatusUrgentCount
		{
			get { return (this.StatusUrgentCount > 0); }
		}
	}
}
