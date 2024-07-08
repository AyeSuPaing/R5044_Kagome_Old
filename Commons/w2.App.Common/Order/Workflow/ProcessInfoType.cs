/*
=========================================================================================================
  Module      : 処理情報（非同時処理とのやり取りに利用する）(ProcessInfoType.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;

namespace w2.App.Common.Order.Workflow
{
	/// <summary>
	/// 処理情報（非同時処理とのやり取りに利用する）
	/// </summary>
	[Serializable]
	public class ProcessInfoType
	{
		/// <summary>総件数</summary>
		public int TotalCount { get; set; }
		/// <summary>完了件数</summary>
		public int DoneCount { get; set; }
		/// <summary>システムエラー発生したか</summary>
		public bool IsSystemError { get; set; }
		/// <summary>実行結果</summary>
		public ActionResults Results { get; set; }
		/// <summary>Display extend status list</summary>
		public Dictionary<int, string> DisplayExtendStatusList { get; set; }
		/// <summary>Is async</summary>
		public bool IsAsync { get; set; }
		/// <summary>Remain count</summary>
		public int RemainCount { get; set; }
		/// <summary>Workflow no</summary>
		public string WorkflowNo { get; set; }
		/// <summary>Workflow kbn</summary>
		public string WorkflowKbn { get; set; }
		/// <summary>Workflow type</summary>
		public string WorkflowType { get; set; }
		/// <summary>Process id</summary>
		public string ProcessId { get; set; }
		/// <summary>Is success</summary>
		public bool IsSuccess { get; set; }
	}
}
