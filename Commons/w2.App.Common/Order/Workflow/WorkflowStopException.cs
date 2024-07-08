/*
=========================================================================================================
  Module      : ワークフロー停止エクセプションクラス(WorkflowStopException.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order.Workflow
{
	/// <summary>
	/// ワークフロー停止エクセプションクラス
	/// </summary>
	public class WorkflowStopException : System.Exception
	{
		/// <summary>シナリオ実行リザルト</summary>
		public ScenarioExecResult ScenarioExecResult { get; set; }
	}
}
