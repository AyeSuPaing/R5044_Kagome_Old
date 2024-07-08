/*
=========================================================================================================
  Module      : タスクスケジューラー操作クラス(TaskSchedulerHelper.cs)
 ････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright  W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// タスクスケジューラーの操作を行うクラス
/// </summary>
public static class TaskSchedulerHelper
{
	/// <summary>
	/// タスク(Batch)の取得
	/// </summary>
	/// <returns>環境ごとのタスク(Batch)</returns>
	public static List<Hashtable> GetAllTasks()
	{
		var allTask = new TaskService();
		// コンフィグの認証情報が正しくない、記載されていない場合のエラー処理
		try
		{
			allTask = new TaskService(Constants.BATCH_MANAGER_TASKSCHEDULER_TARGET_SERVER,
			Constants.BATCH_MANAGER_TASKSCHEDULER_USER_NAME,
			Constants.BATCH_MANAGER_TASKSCHEDULER_ACCOUNT_DOMAIN,
			Constants.BATCH_MANAGER_TASKSCHEDULER_PASSWORD);
		}
		catch(Exception e)
		{
			return null;
		}
		var allTaskList = new List<Hashtable>();

		var projectName = Constants.PROJECT_NO + Constants.ENVIRONMENT_NAME;

		foreach (var task in allTask.RootFolder.AllTasks)
		{
			var taskProjectName = task.Name.Split('.')[0];
			// プロジェクトNoが含まれないタスクの場合次のループへ
			if (projectName != taskProjectName) continue;
			var path = string.Empty;
			var arguments = string.Empty;

			foreach (Microsoft.Win32.TaskScheduler.Action action in task.Definition.Actions)
			{
				ExecAction execAction = action as ExecAction;
				if (execAction != null)
				{
					path = execAction.Path;
					arguments = execAction.Arguments;
				}
			}
			var escapedPath = path.Replace(@"\", @"/");

			if (string.IsNullOrEmpty(arguments)) arguments = string.Empty;

			var taskInfo = new Hashtable{
			{ "Name", task.Name },
			{ "Enabled", task.Enabled ? "有効" : "無効" },
			{ "State", task.State == TaskState.Running ? "実行中" : "―" },
			{ "NextRunTime",  task.NextRunTime.Year != 1 ? StringUtility.ToDateString(task.NextRunTime, "yyyy/MM/dd HH:mm:ss") : "―" },
			{ "LastRunTime", StringUtility.ToDateString(task.LastRunTime, "yyyy/MM/dd HH:mm:ss") },
			{ "Arguments" , arguments },
			{ "Path" , escapedPath }
		};
			allTaskList.Add(taskInfo);
		}

		return allTaskList;
	}

	/// <summary>
	/// パスの取得
	/// </summary>
	/// <param name="taskName">タスク名</param>
	/// <returns>タスクのパス</returns>
	public static string GetTaskPath(string taskName)
	{
		var allTask = new TaskService(Constants.BATCH_MANAGER_TASKSCHEDULER_TARGET_SERVER,
			Constants.BATCH_MANAGER_TASKSCHEDULER_USER_NAME,
			Constants.BATCH_MANAGER_TASKSCHEDULER_ACCOUNT_DOMAIN,
			Constants.BATCH_MANAGER_TASKSCHEDULER_PASSWORD);

		var taskPath = string.Empty;

		var task = allTask.GetTask(taskName);

		foreach (Microsoft.Win32.TaskScheduler.Action action in task.Definition.Actions)
		{
			ExecAction execAction = action as ExecAction;
			if (execAction != null)
			{
				taskPath = execAction.Path;
			}
		}

		return taskPath;
	}

	/// <summary>
	/// コマンドライン引数の取得
	/// </summary>
	/// <param name="taskName">タスク名</param>
	/// <returns>コマンドライン引数</returns>
	public static string GetTaskArguments(string taskName)
	{
		var allTask = new TaskService(Constants.BATCH_MANAGER_TASKSCHEDULER_TARGET_SERVER,
			Constants.BATCH_MANAGER_TASKSCHEDULER_USER_NAME,
			Constants.BATCH_MANAGER_TASKSCHEDULER_ACCOUNT_DOMAIN,
			Constants.BATCH_MANAGER_TASKSCHEDULER_PASSWORD);

		var taskArguments = string.Empty;

		var task = allTask.GetTask(taskName);

		foreach (Microsoft.Win32.TaskScheduler.Action action in task.Definition.Actions)
		{
			ExecAction execAction = action as ExecAction;
			if (execAction != null)
			{
				taskArguments = execAction.Arguments;
			}
		}

		return taskArguments;
	}

	/// <summary>
	/// タスクの有効化
	/// </summary>
	/// <param name="taskName">タスク名</param>
	public static void EnableTask(string taskName)
	{
		var allTask = new TaskService(Constants.BATCH_MANAGER_TASKSCHEDULER_TARGET_SERVER,
			Constants.BATCH_MANAGER_TASKSCHEDULER_USER_NAME,
			Constants.BATCH_MANAGER_TASKSCHEDULER_ACCOUNT_DOMAIN,
			Constants.BATCH_MANAGER_TASKSCHEDULER_PASSWORD);

		var task = allTask.GetTask(taskName);
		if (task == null) return;

		task.Enabled = true;
	}

	/// <summary>
	/// タスクの無効化
	/// </summary>
	/// <param name="taskName">タスク名</param>
	public static void DisableTask(string taskName)
	{
		var allTask = new TaskService(Constants.BATCH_MANAGER_TASKSCHEDULER_TARGET_SERVER,
			Constants.BATCH_MANAGER_TASKSCHEDULER_USER_NAME,
			Constants.BATCH_MANAGER_TASKSCHEDULER_ACCOUNT_DOMAIN,
			Constants.BATCH_MANAGER_TASKSCHEDULER_PASSWORD);

		var task = allTask.GetTask(taskName);
		if (task == null) return;

		task.Enabled = false;
	}

	/// <summary>
	/// タスクの実行
	/// </summary>
	/// <param name="taskName">タスク情報</param>
	/// <rerutn>エラーメッセージ区分</rerutn>
	public static string RunTask(string taskName)
	{
		var allTask = new TaskService(Constants.BATCH_MANAGER_TASKSCHEDULER_TARGET_SERVER,
			Constants.BATCH_MANAGER_TASKSCHEDULER_USER_NAME,
			Constants.BATCH_MANAGER_TASKSCHEDULER_ACCOUNT_DOMAIN,
			Constants.BATCH_MANAGER_TASKSCHEDULER_PASSWORD);

		var task = allTask.GetTask(taskName);

		var path = string.Empty;
		var errorMessageKey = string.Empty;

		foreach (Microsoft.Win32.TaskScheduler.Action action in task.Definition.Actions)
		{
			ExecAction execAction = action as ExecAction;
			if (execAction != null)
			{
				path = execAction.Path;
			}
		}
		var escapedPath = path.Replace(@"\", @"\\");

		var taskProjectName = task.Name.Split('.')[0];

		if (task.Enabled
			&& escapedPath.Contains(taskProjectName)
			&& task.State != TaskState.Running)
		{
			task.Run();
		}
		else if (task.State == TaskState.Running)
		{
			errorMessageKey = Constants.BATCH_MANAGER_ERROR_KBN_RUN_DOUBLE_EXECUTION;
		}
		else
		{
			errorMessageKey = Constants.BATCH_MANAGER_ERROR_KBN_RUN_INFO_MISMATCH;
		}

		return errorMessageKey;
	}

	/// <summary>
	/// タスクの停止
	/// </summary>
	/// <param name="taskName">タスク情報</param>
	/// <rerutn>エラーメッセージ区分</rerutn>
	public static string StopTask(string taskName)
	{
		var allTask = new TaskService(Constants.BATCH_MANAGER_TASKSCHEDULER_TARGET_SERVER,
			Constants.BATCH_MANAGER_TASKSCHEDULER_USER_NAME,
			Constants.BATCH_MANAGER_TASKSCHEDULER_ACCOUNT_DOMAIN,
			Constants.BATCH_MANAGER_TASKSCHEDULER_PASSWORD);


		var task = allTask.GetTask(taskName);

		var errorMessageKey = string.Empty;
		if (task.State == TaskState.Running)
		{
			task.Stop();
		}
		else
		{
			errorMessageKey = Constants.BATCH_MANAGER_ERROR_KBN_STOP_NOT_RUNNING;
		}

		return errorMessageKey;
	}
}
