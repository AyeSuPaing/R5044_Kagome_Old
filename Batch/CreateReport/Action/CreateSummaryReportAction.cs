/*
=========================================================================================================
  Module      : Create Summary Report Action(CreateSummaryReportAction.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.Domain.SummaryReport;

namespace w2.MarketingPlanner.Batch.CreateReport.Action
{
	/// <summary>
	/// Create Summary Report Action
	/// </summary>
	public class CreateSummaryReportAction : ActionBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="reportKbn">Report Kbn</param>
		public CreateSummaryReportAction(string reportKbn)
			: base(reportKbn)
		{
			var service = new SummaryReportService();
			this.InsertAction = service.InsertSummaryReport;
		}
	}
}
