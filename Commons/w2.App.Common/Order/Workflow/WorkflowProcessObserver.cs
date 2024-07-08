/*
=========================================================================================================
  Module      : Workflow Process Observer (WorkflowProcessObserver.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;

namespace w2.App.Common.Order.Workflow
{
	/// <summary>
	/// Workflow Process Observer
	/// </summary>
	public class WorkflowProcessObserver
	{
		/// <summary>インスタンス</summary>
		private static readonly WorkflowProcessObserver s_instance = new WorkflowProcessObserver();
		/// <summary>Process actions</summary>
		private readonly Dictionary<string, ProcessInfoType> _processActions = new Dictionary<string, ProcessInfoType>();
		/// <summary>ロックオブジェクト</summary>
		private readonly object _lockObject = new object();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		private WorkflowProcessObserver()
		{
		}

		/// <summary>
		/// Get workflow process
		/// </summary>
		/// <param name="processId">Process Id</param>
		/// <returns>Workflow process</returns>
		public ProcessInfoType GetWorkflowProcess(string processId)
		{
			lock (this._lockObject)
			{
				return this._processActions.ContainsKey(processId)
					? this._processActions[processId]
					: null;
			}
		}

		/// <summary>
		/// Add workflow process
		/// </summary>
		/// <param name="processId">Process id</param>
		/// <param name="process">Process</param>
		public void AddWorkflowProcess(string processId, ProcessInfoType process)
		{
			lock (this._lockObject)
			{
				if (this._processActions.ContainsKey(processId)) return;

				this._processActions.Add(processId, process);
			}
		}

		/// <summary>
		/// Update workflow process
		/// </summary>
		/// <param name="processId">Process id</param>
		/// <param name="process">Process</param>
		public void UpdateWorkflowProcess(string processId, ProcessInfoType process)
		{
			lock (this._lockObject)
			{
				if (this._processActions.ContainsKey(processId))
				{
					this._processActions[processId] = process;
				}
			}
		}

		/// <summary>
		/// Remove workflow process
		/// </summary>
		/// <param name="processId">Process id</param>
		public void RemoveWorkflowProcess(string processId)
		{
			lock (this._lockObject)
			{
				if (this._processActions.ContainsKey(processId))
				{
					this._processActions.Remove(processId);
				}
			}
		}

		/// <summary>Instance</summary>
		public static WorkflowProcessObserver Instance { get { return s_instance; } }
	}
}