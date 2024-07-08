/*
=========================================================================================================
  Module      : Action base (ActionBase.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Util;
using w2.Common.Logger;
using w2.Common.Sql;

namespace w2.MarketingPlanner.Batch.CreateReport.Action
{
	/// <summary>
	/// Action base
	/// </summary>
	public abstract class ActionBase : IAction
	{
		/// <summary>Workflow target count aggregate action</summary>
		public const string CONST_WORKFLOW_TARGET_COUNT_AGGREGATE_ACTION = "WF_TCAG";

		/// <summary>Success message</summary>
		private const string CONST_SUCCESS_MESSAGE = "成功";
		/// <summary>Failure message</summary>
		private const string CONST_FAILURE_MESSAGE = "失敗";
		/// <summary>Execution message</summary>
		private const string CONST_EXECUTION_MESSAGE = "実行";

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="reportKbn">Report Kbn</param>
		public ActionBase(string reportKbn)
		{
			this.ReportKbn = reportKbn;
			this.ActionName = GetCurrentActionNameExecution();
		}

		/// <summary>
		/// Execute
		/// </summary>
		public void Execute()
		{
			DoExecute();
		}

		/// <summary>
		/// Do Execute
		/// </summary>
		public virtual void DoExecute()
		{
			var isSuccess = false;
			try
			{
				using (var accessor = new SqlAccessor())
				{
					accessor.OpenConnection();
					accessor.BeginTransaction();
					if ((this.ReportKbn != Constants.FLG_SUMMARYREPORT_DATA_KBN_SENT_MAIL_COUNT)
						&& (this.ReportKbn != Constants.FLG_SUMMARYREPORT_DATA_KBN_MAIL_CLICK_COUNT))
					{
						// Insert data for 7 days
						this.InsertAction(
							Constants.FLG_SUMMARYREPORT_PERIOD_KBN_LAST_SEVEN_DAYS,
							this.ReportKbn,
							DateTimeUtility.GetDatePastByDay(7),
							DateTimeUtility.GetDatePastByDay(1),
							accessor);

						// Insert data for current year
						this.InsertAction(
							Constants.FLG_SUMMARYREPORT_PERIOD_KBN_THIS_YEAR,
							this.ReportKbn,
							DateTimeUtility.GetFirstDateOfCurrentYear(),
							new DateTime(DateTime.Today.Year, 12, 1),
							accessor);
					}
					// Insert data for current month
					this.InsertAction(
						Constants.FLG_SUMMARYREPORT_PERIOD_KBN_THIS_MONTH,
						this.ReportKbn,
						DateTimeUtility.GetFirstDateOfCurrentMonth(),
						DateTimeUtility.GetLastDateOfCurrentMonth(),
						accessor);

					// Insert data for last month
					this.InsertAction(
						Constants.FLG_SUMMARYREPORT_PERIOD_KBN_LAST_MONTH,
						this.ReportKbn,
						DateTimeUtility.GetFirstDateOfLastMonth(),
						DateTimeUtility.GetLastDateOfLastMonth(),
						accessor);

						accessor.CommitTransaction();
					isSuccess = true;
				}
			}
			catch (Exception ex)
			{
				WriteLogError(ex);
			}

			WriteResultLog(isSuccess);
		}

		/// <summary>
		/// Write result log
		/// </summary>
		/// <param name="isSuccess">Is success</param>
		/// <param name="expandedLogMessage">Expanded log message</param>
		protected void WriteResultLog(bool isSuccess, string expandedLogMessage = "")
		{
			var messageLog = string.Format("{0}：{1}　{2}　->　{3}",
				CONST_EXECUTION_MESSAGE,
				this.ActionName,
				expandedLogMessage,
				isSuccess
					? CONST_SUCCESS_MESSAGE
					: CONST_FAILURE_MESSAGE);
			FileLogger.WriteInfo(messageLog);
		}

		/// <summary>
		/// Write Log Error
		/// </summary>
		/// <param name="exception">Exception</param>
		/// <param name="expandedLogMessage">Expanded log message</param>
		protected void WriteLogError(Exception exception, string expandedLogMessage = "")
		{
			var messageLog = string.Format("{0}：{1}　{2}",
				CONST_EXECUTION_MESSAGE,
				this.ActionName,
				expandedLogMessage);
			FileLogger.WriteError(messageLog, exception);
		}

		/// <summary>
		/// Get current action name execution
		/// </summary>
		/// <returns>The current action name</returns>
		private string GetCurrentActionNameExecution()
		{
			switch (this.ReportKbn)
			{
				case Constants.FLG_SUMMARYREPORT_DATA_KBN_USER_ACCESS:
					return "ユーザーの訪問数レポート（ユニークユーザー数）";

				case Constants.FLG_SUMMARYREPORT_DATA_KBN_ORDER_COUNT:
					return "注文数";

				case Constants.FLG_SUMMARYREPORT_DATA_KBN_ORDER_AMOUNT:
					return "注文金額";

				case Constants.FLG_SUMMARYREPORT_DATA_KBN_CONVERSION:
					return "コンバージョン レポート（ユニークユーザーあたりの注文金額）";

				case Constants.FLG_SUMMARYREPORT_DATA_KBN_LTV:
					return "LTV レポート";

				case Constants.FLG_SUMMARYREPORT_DATA_KBN_FIXED_PURCHASE_REGISTER:
					return "定期申込数";

				case Constants.FLG_SUMMARYREPORT_DATA_KBN_FIXED_PURCHASE_CANCEL:
					return "定期解約数";

				case Constants.FLG_SUMMARYREPORT_DATA_KBN_USER_REGISTER:
					return "新規会員獲得数";

				case Constants.FLG_SUMMARYREPORT_DATA_KBN_USER_WITHDRAWAL:
					return "会員退会数";

				case Constants.FLG_SUMMARYREPORT_DATA_KBN_MEMBERSHIP_COUNT:
					return "会員数";

				case Constants.FLG_SUMMARYREPORT_DATA_KBN_FIXED_PURCHASE_COUNT:
					return "定期継続数";

				case CONST_WORKFLOW_TARGET_COUNT_AGGREGATE_ACTION:
					return "受注ワークフロー対象";

				case Constants.FLG_SUMMARYREPORT_DATA_KBN_SENT_MAIL_COUNT:
					return "MPからの送信メール数";

				case Constants.FLG_SUMMARYREPORT_DATA_KBN_MAIL_CLICK_COUNT:
					return "メールクリック数";

				default:
					throw new Exception("未定義のReportKbn：" + this.ReportKbn);
			}
		}

		/// <summary>Report Kbn</summary>
		protected string ReportKbn { get; set; }
		/// <summary>Action Name</summary>
		protected string ActionName { get; set; }
		/// <summary>The insert action</summary>
		protected Action<string, string, DateTime, DateTime, SqlAccessor> InsertAction { get; set; }
	}
}
