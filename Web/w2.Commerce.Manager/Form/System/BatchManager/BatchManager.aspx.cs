/*
=========================================================================================================
  Module     バッチ管理ページ処理 : (BatchManager.cs)
 ････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright  W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using w2.Common.Logger;

public partial class Form_System_BatchManager : BasePage
{
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// 許可されたIPアドレスか
			if (IsAllowedIpAddress(Constants.ALLOWED_IP_ADDRESS_FOR_SYSTEMSETTINGS) == false)
			{
				Response.Redirect(Constants.PAGE_MANAGER_ERROR);
			}
		}
		BindAllTasks();
	}

	/// <summary>
	/// タスクをバインド
	/// </summary>
	private void BindAllTasks()
	{
		var allTaskList = TaskSchedulerHelper.GetAllTasks();
		// タスク情報が取得できなかった場合、エラーページに遷移
		if (allTaskList == null)
		{
			this.Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SYSTEM_MANAGEMENT_BATCH_CAN_NOT_GET);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
		rTaskList.DataSource = allTaskList;
		rTaskList.DataBind();
	}

	/// <summary>
	/// バインド時にボタンの設定を行う
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rTaskList_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
		{
			var task = (Hashtable)e.Item.DataItem;
			if (task == null) return;

			// 各ボタンの取得
			var activation = (LinkButton)e.Item.FindControl("lbActivation");
			var disabling = (LinkButton)e.Item.FindControl("lbDisabling");
			var execution = (LinkButton)e.Item.FindControl("lbExecution");
			var stop = (LinkButton)e.Item.FindControl("lbStop");

			var isValid = task["Enabled"].ToString() == ReplaceTag("@@System.batch_manager.task_status.enabled@@");
			var isActive = task["State"].ToString() == ReplaceTag("@@System.batch_manager.task_status.active@@");

			if (isValid && isActive)
			{
				// タスクの状態が「有効」かつ「実行中」の場合、タスクの停止ボタンのみ押下可能とする
				activation.Enabled = false;
				disabling.Enabled = false;
				execution.Enabled = false;
				stop.Enabled = true;
			}
			else if (isValid && (isActive == false))
			{
				// タスクの状態が「有効」かつ「実行中ではない」場合、無効化ボタンと実行ボタンを押下可能とする
				activation.Enabled = false;
				disabling.Enabled = true;
				execution.Enabled = true;
				stop.Enabled = false;
			}
			else if (isValid == false)
			{
				// タスクの状態が「無効」の場合、有効化ボタンのみ押下可能とする
				activation.Enabled = true;
				disabling.Enabled = false;
				execution.Enabled = false;
				stop.Enabled = false;
			}

			// 各ボタン押下時のアラートウィンドの設定
			activation.OnClientClick = "return confirm('"
				+ WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SYSTEM_MANAGEMENT_BATCH_CONFIRM)
				.Replace("@@ 1 @@", ReplaceTag("@@System.batch_manager.confirm_message.enabled@@"))
				.Replace("@@ 2 @@", task["Name"].ToString())
				.Replace("@@ 3 @@", task["Arguments"].ToString())
				.Replace("@@ 4 @@", task["Path"].ToString())
				+ "');";

			disabling.OnClientClick = "return confirm('"
				+ WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SYSTEM_MANAGEMENT_BATCH_CONFIRM)
				.Replace("@@ 1 @@", ReplaceTag("@@System.batch_manager.confirm_message.disabled@@"))
				.Replace("@@ 2 @@", task["Name"].ToString())
				.Replace("@@ 3 @@", task["Arguments"].ToString())
				.Replace("@@ 4 @@", task["Path"].ToString())
				+ "');";

			execution.OnClientClick = "return confirm('"
				+ WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SYSTEM_MANAGEMENT_BATCH_CONFIRM)
				.Replace("@@ 1 @@", ReplaceTag("@@System.batch_manager.confirm_message.execution@@"))
				.Replace("@@ 2 @@", task["Name"].ToString())
				.Replace("@@ 3 @@", task["Arguments"].ToString())
				.Replace("@@ 4 @@", task["Path"].ToString())
				+ "');";

			stop.OnClientClick = "return confirm('"
				+ WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SYSTEM_MANAGEMENT_BATCH_CONFIRM)
				.Replace("@@ 1 @@", ReplaceTag("@@System.batch_manager.confirm_message.stop@@"))
				.Replace("@@ 2 @@", task["Name"].ToString())
				.Replace("@@ 3 @@", task["Arguments"].ToString())
				.Replace("@@ 4 @@", task["Path"].ToString())
				+ "');";
		}
	}

	/// <summary>
	/// タスクを操作するボタン押下時の処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rTaskList_ItemCommand(object sender, RepeaterCommandEventArgs e)
	{
		var taskName = e.CommandArgument.ToString();

		var operation = string.Empty;
		var errorMessageKbn = string.Empty;

		switch (e.CommandName)
		{
			case "active":
				TaskSchedulerHelper.EnableTask(taskName);
				operation = ReplaceTag("@@System.batch_manager.log_message.enabled@@");
				break;

			case "disable":
				TaskSchedulerHelper.DisableTask(taskName);
				operation = ReplaceTag("@@System.batch_manager.log_message.disabled@@");
				break;

			case "execution":
				errorMessageKbn = TaskSchedulerHelper.RunTask(taskName);
				operation = ReplaceTag("@@System.batch_manager.log_message.execution@@");
				break;

			case "stop":
				errorMessageKbn = TaskSchedulerHelper.StopTask(taskName);
				operation = ReplaceTag("@@System.batch_manager.log_message.stop@@");
				break;

			default:
				break;
		}
		WriteLog(operation, taskName);
		BindAllTasks();

		if (string.IsNullOrEmpty(errorMessageKbn)) Response.Redirect(Request.RawUrl);

		var errorMessage = string.Empty;
		if (errorMessageKbn == Constants.BATCH_MANAGER_ERROR_KBN_RUN_DOUBLE_EXECUTION)
		{
			errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SYSTEM_MANAGEMENT_BATCH_RUN_DOUBLE_EXECUTION)
				.Replace("@@ 1 @@", taskName);
		}
		else if (errorMessageKbn == Constants.BATCH_MANAGER_ERROR_KBN_RUN_INFO_MISMATCH)
		{
			errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SYSTEM_MANAGEMENT_BATCH_RUN)
				.Replace("@@ 1 @@", TaskSchedulerHelper.GetTaskPath(taskName));
		}
		else if (errorMessageKbn == Constants.BATCH_MANAGER_ERROR_KBN_STOP_NOT_RUNNING)
		{
			errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SYSTEM_MANAGEMENT_BATCH_STOP)
				.Replace("@@ 1 @@", taskName);
		}

		lErrorMessage.Text = errorMessage;
	}

	/// <summary>
	/// Infoログを記載
	/// </summary>
	/// <param name="operation">操作内容</param>
	private void WriteLog(string operation, string taskName)
	{
		var path = TaskSchedulerHelper.GetTaskPath(taskName);
		var arguments = TaskSchedulerHelper.GetTaskArguments(taskName);

		var escapedPath = path.Replace(@"\", @"/");

		var request = HttpContext.Current.Request;

		// 開発環境の場合、ローカルのIPアドレスを取得
		var externalIp = request.IsLocal
			? new WebClient().DownloadString("https://api.ipify.org")
			: request.ServerVariables["REMOTE_ADDR"];

		var logMessage = new StringBuilder();
		logMessage.Append("バッチ操作が行われました");
		logMessage.AppendFormat(" (OperatorId: {0}", this.LoginOperatorId);
		logMessage.AppendFormat(" IpAddress: {0}", externalIp);
		logMessage.AppendFormat(" SessionId: {0})", Session.SessionID).AppendLine();
		logMessage.AppendFormat(" 操作: {0},", operation);
		logMessage.AppendFormat(" タスク名: {0},", taskName);
		logMessage.AppendFormat(" コマンドライン引数: {0},", arguments);
		logMessage.AppendFormat(" 実行パス: {0}", escapedPath);
		FileLogger.WriteInfo(logMessage.ToString());
	}
}
