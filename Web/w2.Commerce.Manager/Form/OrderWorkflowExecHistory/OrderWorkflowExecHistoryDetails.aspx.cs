/*
=========================================================================================================
  Module      : 受注ワークフロー実行履歴詳細ページ(OrderWorkflowExecHistoryDatails.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Web;
using w2.Domain.OrderWorkflowExecHistory;

namespace Form.OrderWorkflowExecHistory
{
	/// <summary>
	/// 受注ワークフロー実行履歴ページ
	/// </summary>
	public partial class OrderWorkflowExecHistoryDetails : OrderWorkflowPage
	{
		/// <summary>
		/// ページロード
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				Initialize();
			}
		}

		/// <summary>
		/// 初期化
		/// </summary>
		private void Initialize()
		{
			var history = new OrderWorkflowExecHistoryService().Get(this.RequestHistoryId);
			if (history == null)
			{
				var historyListUrl = new UrlCreator(Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_ORDERWORKFLOW_EXEC_HISTORY_LIST)
					.CreateUrl();
				Response.Redirect(historyListUrl);
			}
			this.History = history;

			lExecHistoryId.Text = WebSanitizer.HtmlEncode(this.History.OrderWorkflowExecHistoryId.ToString().Trim());
			lDateBegin.Text = WebSanitizer.HtmlEncode(
				DateTimeUtility.ToStringForManager(
					this.History.DateBegin,
					DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter));
			lDateEnd.Text = WebSanitizer.HtmlEncode(
				DateTimeUtility.ToStringForManager(
					this.History.DateEnd,
					DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter));
			lDateCreateded.Text = WebSanitizer.HtmlEncode(
				DateTimeUtility.ToStringForManager(
					this.History.DateCreated,
					DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter));
			lExecStatus.Text = WebSanitizer.HtmlEncode(ConvertExecStatusForDisplay(this.History.ExecStatus));
			lSuccessRate.Text = WebSanitizer.HtmlEncode(this.History.SuccessRate);
			lWorkFlowPlace.Text = WebSanitizer.HtmlEncode(ConvertExecPlaceForDisplay(this.History.ExecPlace));
			lWorkFlowName.Text = WebSanitizer.HtmlEncode(this.History.WorkflowName);
			lScenarioName.Text = WebSanitizer.HtmlEncode(ConvertScenarioNameForDisplay(this.History.ScenarioName));
			lWorkFlowType.Text = WebSanitizer.HtmlEncode(ConvertWorkflowTypeForDisplay(this.History.WorkflowType));
			lExecTiming.Text = WebSanitizer.HtmlEncode(ConvertExecTimingForDisplay(this.History.ExecTiming));
			lMessage.Text = WebSanitizer.HtmlEncode(ConvertMessageForDisplay(this.History.Message));
			lLastChanged.Text = WebSanitizer.HtmlEncode(this.History.LastChanged);

			if (this.History.ExecStatus == Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_RUNNING
				|| this.History.ExecStatus == Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_HOLD)
			{
				btnCancelRunningBottom.Enabled = true;
			}
		}

		/// <summary>
		/// ExecStatusの表示用Textに変換
		/// </summary>
		/// <param name="execStatus">実行ステータス</param>
		/// <returns>ExecStatus表示用Text</returns>
		private string ConvertExecStatusForDisplay(string execStatus)
		{
			var valueExecStatus = WebSanitizer.HtmlEncode(ValueText.GetValueText(
				Constants.TABLE_ORDERWORKFLOWEXECHISTORY,
				Constants.FIELD_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS,
				execStatus));
			return valueExecStatus;
		}

		/// <summary>
		/// ExecPlaceの表示用Textに変換
		/// </summary>
		/// <param name="execPlace">実行ステータス</param>
		/// <returns>ExecPlace表示用Text</returns>
		private string ConvertExecPlaceForDisplay(string execPlace)
		{
			var valueExecStatus = WebSanitizer.HtmlEncode(ValueText.GetValueText(
				Constants.TABLE_ORDERWORKFLOWEXECHISTORY,
				Constants.FIELD_ORDERWORKFLOWEXECHISTORY_EXEC_PLACE,
				execPlace));
			return valueExecStatus;
		}

		/// <summary>
		/// 表示用のシナリオ名に変換
		/// </summary>
		/// <param name="scenarioName">シナリオ名</param>
		/// <returns>表示用のシナリオ名</returns>
		private string ConvertScenarioNameForDisplay(string scenarioName)
		{
			return WebSanitizer.HtmlEncode(!string.IsNullOrEmpty(scenarioName) ? "(" + scenarioName + ")" : "");
		}

		/// <summary>
		/// WorkflowTypeの表示用Textに変換
		/// </summary>
		/// <param name="workflowType">ワークフロータイプ</param>
		/// <returns>ExecPlace表示用Text</returns>
		private string ConvertWorkflowTypeForDisplay(string workflowType)
		{
			var valueExecStatus = WebSanitizer.HtmlEncode(ValueText.GetValueText(
				Constants.TABLE_ORDERWORKFLOWEXECHISTORY,
				Constants.FIELD_ORDERWORKFLOWEXECHISTORY_WORKFLOW_TYPE,
				workflowType));
			return valueExecStatus;
		}

		/// <summary>
		/// ExecTimingの表示用Textに変換
		/// </summary>
		/// <param name="execTiming">実行ステータス</param>
		/// <returns>ExecTiming表示用Text</returns>
		private string ConvertExecTimingForDisplay(string execTiming)
		{
			var valueExecStatus = WebSanitizer.HtmlEncode(ValueText.GetValueText(
				Constants.TABLE_ORDERWORKFLOWEXECHISTORY,
				Constants.FIELD_ORDERWORKFLOWEXECHISTORY_EXEC_TIMING,
				execTiming));
			return valueExecStatus;
		}

		/// <summary>
		/// Messageの表示用Textに変換
		/// </summary>
		/// <param name="message">メッセージ</param>
		/// <returns>Message表示用Text</returns>
		private string ConvertMessageForDisplay(string message)
		{
			var valueMessage = ValueText.GetValueText(
				Constants.TABLE_ORDERWORKFLOWEXECHISTORY,
				Constants.FIELD_ORDERWORKFLOWEXECHISTORY_MESSAGE,
				message);
			return valueMessage;
		}

		/// <summary>
		/// 戻るボタンクリック
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnBack_Click(object sender, EventArgs e)
		{
			var url = (SessionManager.OrderworkflowDetailsUrlOfPreviousPage == null)
				? new UrlCreator(Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_ORDERWORKFLOW_EXEC_HISTORY_LIST).CreateUrl()
				: SessionManager.OrderworkflowDetailsUrlOfPreviousPage;

			SessionManager.OrderworkflowDetailsUrlOfPreviousPage = null;
			Response.Redirect(new UrlCreator(url).CreateUrl());
		}

		/// <summary>
		/// 変更ボタンクリック
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnChangeExecStatus_Click(object sender, EventArgs e)
		{
			btnCancelRunningBottom.Enabled = true;
			new OrderWorkflowExecHistoryService().ModifyForChangeFromRunningOrHoldToNg(
				this.History.OrderWorkflowExecHistoryId,
				history =>
				{
					history.ExecStatus = Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_NG;
					history.Message = Constants.FLG_ORDERWORKFLOWEXECHISTORY_MESSAGE_FOR_CHANGE_FROM_RUNNING_TO_NG;
				});
			var url = new UrlCreator(Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_ORDERWORKFLOW_EXEC_HISTORY_DETAILS)
				.AddParam(Constants.FIELD_ORDERWORKFLOWEXECHISTORY_ORDER_WORKFLOW_EXEC_HISTORY_ID, this.History.OrderWorkflowExecHistoryId.ToString())
				.CreateUrl();
			Response.Redirect(url);
		}

		/// <summary>
		/// list_item_bg_?のCSSクラスを取得
		/// </summary>
		/// <returns>CSS Class</returns>
		protected string GetCssClassToListItemBackGround()
		{
			var cssClass = "";
			switch (this.History.ExecStatus)
			{
				case Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_OK:
					cssClass = "list_item_bg_success";
					break;

				case Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_STOPPED:
				case Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_NG:
					cssClass = "list_item_bg_error";
					break;
			}
			return WebSanitizer.HtmlEncode(cssClass);
		}

		/// <summary>リクエストされた受注ワークフロー履歴ID</summary>
		private int RequestHistoryId
		{
			get
			{
				int historyId;
				if (int.TryParse(
					Request.Params[Constants.FIELD_ORDERWORKFLOWEXECHISTORY_ORDER_WORKFLOW_EXEC_HISTORY_ID],
					out historyId) == false)
				{
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDERWORKFLOW_EXEC_HISTORY_LIST);
				}
				return historyId;
			}
		}
		/// <summary>実行ステータス</summary>
		protected OrderWorkflowExecHistoryModel History
		{
			get { return (OrderWorkflowExecHistoryModel)ViewState["History"]; }
			private set { ViewState["History"] = value; }
		}
	}
}