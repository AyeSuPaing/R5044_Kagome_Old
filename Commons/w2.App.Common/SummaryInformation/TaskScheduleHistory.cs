/*
=========================================================================================================
  Module      : メール配信状況 (TaskScheduleHistory.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.Domain.SummaryReport;

namespace w2.App.Common.SummaryInformation
{
	/// <summary>
	/// メール配信数取得
	/// </summary>
	[Serializable]
	public class TaskScheduleHistory
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public TaskScheduleHistory()
		{
			InitializeProperties();
		}

		/// <summary>
		/// レポート作成
		/// </summary>
		/// <returns>Report</returns>
		public TaskScheduleHistory CreateReport()
		{
			var result = new Decimal[6];

			DateTime thismonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
			result[0] = new SummaryReportService().GetTaskScheduleHistory(thismonth, Constants.FLG_SUMMARYREPORT_DATA_KBN_SENT_MAIL_COUNT, Constants.FLG_SUMMARYREPORT_PERIOD_KBN_THIS_MONTH);
			result[1] = new SummaryReportService().GetTaskScheduleHistory(thismonth.AddMonths(-1), Constants.FLG_SUMMARYREPORT_DATA_KBN_SENT_MAIL_COUNT, Constants.FLG_SUMMARYREPORT_PERIOD_KBN_LAST_MONTH);
			result[2] = new SummaryReportService().GetMailClickCount(thismonth, Constants.FLG_SUMMARYREPORT_DATA_KBN_MAIL_CLICK_COUNT, Constants.FLG_SUMMARYREPORT_PERIOD_KBN_THIS_MONTH);
			result[3] = new SummaryReportService().GetMailClickCount(thismonth.AddMonths(-1), Constants.FLG_SUMMARYREPORT_DATA_KBN_MAIL_CLICK_COUNT, Constants.FLG_SUMMARYREPORT_PERIOD_KBN_LAST_MONTH);
			if(result[0] != 0) result[4] = Decimal.Round((result[2] / result[0] * 100.00m), 2);
			if (result[1] != 0) result[5] = Decimal.Round((result[3] / result[1] * 100.00m), 2);
			
			SetReports(result);
			return this;
		}
		
		/// <summary>
		/// プロパティ初期化
		/// </summary>
		private void InitializeProperties()
		{
			this.SendMailCount_ThisMonth = 0;
			this.SendMailCount_LastMonth = 0;
			this.MailClickCount_ThisMonth = 0;
			this.MailClickCount_LastMonth = 0;
			this.ClickRate_ThisMonth = 0.00m;
			this.ClickRate_LastMonth = 0.00m;
		}

		/// <summary>
		/// 取得データをセット
		/// </summary>
		/// <param name="reports">Reports</param>
		private void SetReports(Decimal[] reports)
		{
			if (reports == null) return;

			this.SendMailCount_ThisMonth = reports[0];
			this.SendMailCount_LastMonth = reports[1];
			this.MailClickCount_ThisMonth = reports[2];
			this.MailClickCount_LastMonth = reports[3];
			this.ClickRate_ThisMonth = reports[4];
			this.ClickRate_LastMonth = reports[5];
		}

		/// <summary> 配信済みメール当月 </summary>
		public Decimal SendMailCount_ThisMonth { get; set; }
		/// <summary> 配信済みメール前月 </summary>
		public Decimal SendMailCount_LastMonth { get; set; }
		/// <summary> クリック数当月 </summary>
		public Decimal MailClickCount_ThisMonth { get; set; }
		/// <summary> クリック数前月 </summary>
		public Decimal MailClickCount_LastMonth { get; set; }
		/// <summary> クリック率当月 </summary>
		public Decimal ClickRate_ThisMonth { get; set; }
		/// <summary> クリック率前月 </summary>
		public Decimal ClickRate_LastMonth { get; set; }
	}
}
