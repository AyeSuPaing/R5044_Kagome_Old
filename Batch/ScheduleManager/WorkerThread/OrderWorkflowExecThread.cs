/*
=========================================================================================================
  Module      : 受注ワークフロー実行スレッドクラス(OrderWorkflowExecThread.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using System.Threading;
using w2.App.Common;
using w2.App.Common.Order.Workflow;
using w2.Common.Logger;
using w2.Common.Util;
using w2.Common.Web;

namespace w2.MarketingPlanner.Win.ScheduleManager.WorkerThread
{
	/// <summary>
	/// 受注ワークフロー実行スレッドクラス
	/// </summary>
	class OrderWorkflowExecThread : BaseThread
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="scheduleDate">スケジュール日付</param>
		/// <param name="deptId">識別ID</param>
		/// <param name="actionKbn">アクション区分</param>
		/// <param name="workflowScenarioId">ワークフローシナリオID</param>
		/// <param name="actionNo">アクションNO</param>
		/// <param name="execTiming">実行タイミング</param>
		/// <param name="lastChanged">スレッドが呼び出された時点での最終更新者</param>
		public OrderWorkflowExecThread(
			DateTime scheduleDate,
			string deptId,
			string actionKbn,
			string workflowScenarioId,
			int actionNo,
			string execTiming,
			string lastChanged)
			: base(scheduleDate, deptId, actionKbn, workflowScenarioId, actionNo)
		{
			this.ExecTiming = execTiming;
			this.LastChanged = lastChanged;
		}

		/// <summary>
		/// スレッド作成（タスクスケジュール実行）
		/// </summary>
		/// <param name="scheduleDate">スケジュール日付</param>
		/// <param name="deptId">識別ID</param>
		/// <param name="actionKbn">アクション区分</param>
		/// <param name="workflowScenarioId">ワークフローシナリオID</param>
		/// <param name="actionNo">アクションNo</param>
		/// <param name="execTiming">実行タイミング</param>
		/// <param name="lastChanged">スレッドが呼び出された時点での最終更新者</param>
		/// <returns>生成スレッド</returns>
		public static OrderWorkflowExecThread CreateAndStart(
			DateTime scheduleDate,
			string deptId,
			string actionKbn,
			string workflowScenarioId,
			int actionNo,
			string execTiming,
			string lastChanged)
		{
			// スレッド作成
			var orderWorkflowExecThread = new OrderWorkflowExecThread(
				scheduleDate,
				deptId,
				actionKbn,
				workflowScenarioId,
				actionNo,
				execTiming,
				lastChanged);
			orderWorkflowExecThread.Thread = new Thread(orderWorkflowExecThread.Work);

			// スレッドスタート
			orderWorkflowExecThread.Thread.Start();

			return orderWorkflowExecThread;
		}

		/// <summary>
		/// 受注ワークフロー実行
		/// </summary>
		public void Work()
		{
			ScenarioExecResult scenarioExecResult;
			try
			{
				lock (GetLockObject(this.DeptId, this.ActionKbn, this.MasterId))
				{
					Form1.WriteInfoLogLine(
						string.Format(
							ValueText.GetValueText(
								Constants.VALUE_TEXT_KEY_ORDER_WORKFLOW_EXEC_THREAD,
								Constants.VALUE_TEXT_FIELD_ORDER_WORKFLOW_EXEC_THREAD_LOG_FORMAT,
								Constants.VALUE_TEXT_ORDER_WORKFLOW_EXEC_THREAD_LOG_FORMAT_EXEC_START),
							this.MasterId));

					scenarioExecResult = new WorkflowExec(
						this.DeptId,
						this.ActionKbn,
						this.MasterId,
						this.ActionNo,
						this.ScheduleDate,
						this.LastChanged,
						this.ExecTiming).ExecOrderWorkflowScenario();
					SendEndMail(scenarioExecResult);
					EndProcess(scenarioExecResult);
				}
			}
			catch (WorkflowStopException ws)
			{
				scenarioExecResult = ws.ScenarioExecResult;
				SendEndMail(scenarioExecResult);
				EndProcess(scenarioExecResult);
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
				Form1.WriteErrorLogLine(ex.ToString());
			}
		}

		/// <summary>
		/// メール送信
		/// </summary>
		/// <param name="scenarioExecResult">シナリオ実行リザルト</param>
		private void SendEndMail(ScenarioExecResult scenarioExecResult)
		{
			var mailInput = CreateMailInput(scenarioExecResult);

			using (MailSendUtility msMailSend = new MailSendUtility(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.CONST_MAIL_ID_ORDER_WORKFLOW_SCENARIO_EXEC,
				"",
				mailInput, true, Constants.MailSendMethod.Auto))
			{
				if (msMailSend.SendMail() == false)
				{
					FileLogger.WriteError(string.Format("{0} : {1}", GetType().BaseType, msMailSend.MailSendException));
				}
			}
		}

		/// <summary>
		/// メールのInputを作成
		/// </summary>
		/// <param name="scenarioExecResult">シナリオ実行リザルト</param>
		/// <returns>メールのインプット</returns>
		private Hashtable CreateMailInput(ScenarioExecResult scenarioExecResult)
		{
			var execStatus = (Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_DONE == scenarioExecResult.ScenarioExecStatus)
				? ValueText.GetValueText(
					Constants.TABLE_ORDERWORKFLOWEXECHISTORY,
					Constants.FIELD_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS,
					Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_OK)
				: ValueText.GetValueText(
					Constants.TABLE_ORDERWORKFLOWEXECHISTORY,
					Constants.FIELD_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS,
					Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_NG);

			var execTiming = ValueText.GetValueText(
				Constants.TABLE_ORDERWORKFLOWEXECHISTORY,
				Constants.FIELD_ORDERWORKFLOWEXECHISTORY_EXEC_TIMING,
				this.ExecTiming);
			var scenarioNameOfMessage = string.Format(
				"[{0}]",
				scenarioExecResult.Scenario.ScenarioName);
			var execStatusOfMessage = string.Format(
				ValueText.GetValueText(
					Constants.VALUE_TEXT_KEY_ORDER_WORKFLOW_EXEC_THREAD,
					Constants.VALUE_TEXT_FIELD_ORDER_WORKFLOW_EXEC_THREAD_MAIL_FORMAT,
					Constants.VALUE_TEXT_ORDER_WORKFLOW_EXEC_THREAD_MAIL_FORMAT_EXEC_STATUS),
				execStatus);
			var execTimingOfMessage = string.Format(
				ValueText.GetValueText(
					Constants.VALUE_TEXT_KEY_ORDER_WORKFLOW_EXEC_THREAD,
					Constants.VALUE_TEXT_FIELD_ORDER_WORKFLOW_EXEC_THREAD_MAIL_FORMAT,
					Constants.VALUE_TEXT_ORDER_WORKFLOW_EXEC_THREAD_MAIL_FORMAT_EXEC_TIMING),
				execTiming);
			var execStartDatetimeOfMessage = string.Format(
				ValueText.GetValueText(
					Constants.VALUE_TEXT_KEY_ORDER_WORKFLOW_EXEC_THREAD,
					Constants.VALUE_TEXT_FIELD_ORDER_WORKFLOW_EXEC_THREAD_MAIL_FORMAT,
					Constants.VALUE_TEXT_ORDER_WORKFLOW_EXEC_THREAD_MAIL_FORMAT_START_TIME),
				scenarioExecResult.ExecStartDatetime);
			var execEndDatetimeOfMessage = string.Format(
				ValueText.GetValueText(
					Constants.VALUE_TEXT_KEY_ORDER_WORKFLOW_EXEC_THREAD,
					Constants.VALUE_TEXT_FIELD_ORDER_WORKFLOW_EXEC_THREAD_MAIL_FORMAT,
					Constants.VALUE_TEXT_ORDER_WORKFLOW_EXEC_THREAD_MAIL_FORMAT_END_TIME),
				scenarioExecResult.ExecEndDatetime);
			var workflowsStateOfMessage = CreateWorkflowsStateForMail(scenarioExecResult);
			var massage = string.Format(
				"{0}\r\n{1}\r\n{2}\r\n{3}\r\n{4}\r\n\r\n{5}\r\n",
				scenarioNameOfMessage,
				execStatusOfMessage,
				execTimingOfMessage,
				execStartDatetimeOfMessage,
				execEndDatetimeOfMessage,
				workflowsStateOfMessage);

			var mailInput = new Hashtable
			{
				{ Constants.MAILTEMPLATE_KEY_EXEC_STATUS, string.Format("[{0}]", execStatus) },
				{ Constants.MAILTEMPLATE_KEY_SCENARIO_NAME, scenarioExecResult.Scenario.ScenarioName },
				{ Constants.MAILTEMPLATE_KEY_MAIL_TYPE, ValueText.GetValueText(
					Constants.VALUE_TEXT_KEY_ORDER_WORKFLOW_EXEC_THREAD,
					Constants.MAILTEMPLATE_KEY_MAIL_TYPE,
					Constants.VALUE_TEXT_IMPORT_MAILTEMPLATE_KEY_MAIL_TYPE_END) },
				{ Constants.MAILTEMPLATE_KEY_SCENARIO_MESSAGE, massage },
			};

			return mailInput;
		}

		/// <summary>
		/// メールに記載するワークフローの実行ステータスを作成
		/// </summary>
		/// <param name="scenarioExecResult">シナリオ実行リザルト</param>
		/// <returns>メールに記載するワークフローの実行ステータス</returns>
		private string CreateWorkflowsStateForMail(ScenarioExecResult scenarioExecResult)
		{
			// 実行したワークフローを追加
			var workflowsState = scenarioExecResult.Histories.Select(
				(history, index) =>
				{
					var historyDetailsUrl = new UrlCreator(
						string.Format(
							"{0}{1}{2}{3}",
							Constants.PROTOCOL_HTTPS,
							Constants.SITE_DOMAIN,
							Constants.PATH_ROOT_EC,
							Constants.PAGE_MANAGER_ORDERWORKFLOW_EXEC_HISTORY_DETAILS))
						.AddParam(
							Constants.FIELD_ORDERWORKFLOWEXECHISTORY_ORDER_WORKFLOW_EXEC_HISTORY_ID,
							history.OrderWorkflowExecHistoryId.ToString())
						.CreateUrl();

					var workflowState = GetWorkflowStateStringForMail(
						history.WorkflowName,
						ValueText.GetValueText(
							Constants.TABLE_ORDERWORKFLOWEXECHISTORY,
							Constants.FIELD_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS,
							history.ExecStatus),
						history.SuccessRate,
						historyDetailsUrl);

					if (string.IsNullOrEmpty(history.ErrorMessage) == false)
					{
						workflowState += "\r\n\t" + history.ErrorMessage;
					}

					return workflowState;
				}).ToArray();

			return string.Join("\r\n\r\n", workflowsState);
		}

		/// <summary>
		/// メール送信する時のワークフローのステータス文字列を取得
		/// </summary>
		/// <param name="workflowName">ワークフロー名</param>
		/// <param name="execStatus">実行ステータス</param>
		/// <param name="successRate">成功率</param>
		/// <param name="historyDetailsUrl">実行履歴詳細URL</param>
		/// <returns>ワークフローのステータス文字列</returns>
		private string GetWorkflowStateStringForMail(
			string workflowName,
			string execStatus,
			string successRate,
			string historyDetailsUrl)
		{
			var execStatusForWriteMail = string.Format(
				ValueText.GetValueText(
					Constants.VALUE_TEXT_KEY_ORDER_WORKFLOW_EXEC_THREAD,
					Constants.VALUE_TEXT_FIELD_ORDER_WORKFLOW_EXEC_THREAD_MAIL_FORMAT,
					Constants.VALUE_TEXT_ORDER_WORKFLOW_EXEC_THREAD_MAIL_FORMAT_EXEC_STATUS),
				execStatus);

			var progressForWriteMail = string.Format(
				ValueText.GetValueText(
					Constants.VALUE_TEXT_KEY_ORDER_WORKFLOW_EXEC_THREAD,
					Constants.VALUE_TEXT_FIELD_ORDER_WORKFLOW_EXEC_THREAD_MAIL_FORMAT,
					Constants.VALUE_TEXT_ORDER_WORKFLOW_EXEC_THREAD_MAIL_FORMAT_PROGRESS),
				successRate);

			var historyDetailsUrlForWriteMail = string.Format(
				ValueText.GetValueText(
					Constants.VALUE_TEXT_KEY_ORDER_WORKFLOW_EXEC_THREAD,
					Constants.VALUE_TEXT_FIELD_ORDER_WORKFLOW_EXEC_THREAD_MAIL_FORMAT,
					Constants.VALUE_TEXT_ORDER_WORKFLOW_EXEC_THREAD_MAIL_FORMAT_HISTORY_DETAILS_URL),
				historyDetailsUrl);

			var workflowState = string.Format(
				"{0}\r\n\t{1}\r\n\t{2}\r\n\t{3}",
				workflowName,
				execStatusForWriteMail,
				progressForWriteMail,
				historyDetailsUrlForWriteMail);
			return workflowState;
		}

		/// <summary>
		/// 終了時のプロセス
		/// </summary>
		/// <param name="scenarioExecResult">シナリオ実行リザルト</param>
		private void EndProcess(ScenarioExecResult scenarioExecResult)
		{
			var title = "";
			var status = "";
			switch (scenarioExecResult.ScenarioExecStatus)
			{
				case Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_DONE:
					title = ValueText.GetValueText(
						Constants.VALUE_TEXT_KEY_ORDER_WORKFLOW_EXEC_THREAD,
						Constants.VALUE_TEXT_FIELD_ORDER_WORKFLOW_EXEC_THREAD_LOG_TITLE,
						Constants.VALUE_TEXT_WORKFLOW_THREAD_WRITE_LOG_TITLE_SCENARIO_EXEC);
					status = ValueText.GetValueText(
						Constants.VALUE_TEXT_KEY_ORDER_WORKFLOW_EXEC_THREAD,
						Constants.VALUE_TEXT_FIELD_ORDER_WORKFLOW_EXEC_THREAD_LOG_STATUS,
						Constants.VALUE_TEXT_WORKFLOW_THREAD_WRITE_LOG_STATUS_SCENARIO_EXEC);
					break;

				case Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_STOP:
					title = ValueText.GetValueText(
						Constants.VALUE_TEXT_KEY_ORDER_WORKFLOW_EXEC_THREAD,
						Constants.VALUE_TEXT_FIELD_ORDER_WORKFLOW_EXEC_THREAD_LOG_TITLE,
						Constants.VALUE_TEXT_WORKFLOW_THREAD_WRITE_LOG_TITLE_SCENARIO_STOP);
					status = ValueText.GetValueText(
						Constants.VALUE_TEXT_KEY_ORDER_WORKFLOW_EXEC_THREAD,
						Constants.VALUE_TEXT_FIELD_ORDER_WORKFLOW_EXEC_THREAD_LOG_STATUS,
						Constants.VALUE_TEXT_WORKFLOW_THREAD_WRITE_LOG_STATUS_SCENARIO_STOP);
					break;

				case Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_ERROR:
					title = ValueText.GetValueText(
						Constants.VALUE_TEXT_KEY_ORDER_WORKFLOW_EXEC_THREAD,
						Constants.VALUE_TEXT_FIELD_ORDER_WORKFLOW_EXEC_THREAD_LOG_TITLE,
						Constants.VALUE_TEXT_WORKFLOW_THREAD_WRITE_LOG_TITLE_SCENARIO_ERROR);
					status = ValueText.GetValueText(
						Constants.VALUE_TEXT_KEY_ORDER_WORKFLOW_EXEC_THREAD,
						Constants.VALUE_TEXT_FIELD_ORDER_WORKFLOW_EXEC_THREAD_LOG_STATUS,
						Constants.VALUE_TEXT_WORKFLOW_THREAD_WRITE_LOG_STATUS_SCENARIO_ERROR);
					break;
			}

			var progress = (scenarioExecResult.ScenarioExecStatus == Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_DONE)
				? string.Format(
					"{0}/{1}",
					scenarioExecResult.WorkflowExecSuccessfulCount,
					scenarioExecResult.WorkflowCountBeExec)
				: string.Format(
					"{0}/{1}\r\n{2}/{3}",
					scenarioExecResult.WorkflowExecSuccessfulCount,
					scenarioExecResult.WorkflowCountBeExec,
					StringUtility.ToNumeric(scenarioExecResult.ActionExecCountOfRunningWorkflow),
					StringUtility.ToNumeric(scenarioExecResult.ActionCountBeExecOfRunningWorkflow));

			UpdateTaskStatusEnd(
				scenarioExecResult.ScenarioExecStatus,
				Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_EXECUTE,
				progress);

			var log = string.Format("{0}: [{1}]{2}", title, this.MasterId, status);
			if (scenarioExecResult.ScenarioExecStatus == Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_DONE)
			{
				Form1.WriteInfoLogLine(log);
			}
			else
			{
				Form1.WriteErrorLogLine(log);
				FileLogger.WriteError(log);
			}
		}

		/// <summary>実行タイミング</summary>
		private string ExecTiming { get; set; }
		/// <summary>最終更新者</summary>
		private string LastChanged { get; set; }
	}
}
