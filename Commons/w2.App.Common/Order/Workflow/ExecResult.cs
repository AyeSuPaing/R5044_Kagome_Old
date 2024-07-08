/*
=========================================================================================================
  Module      : Exec result(ExecResult.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;

namespace w2.App.Common.Order.Workflow
{
	/// <summary>
	/// Exec result
	/// </summary>
	[Serializable]
	public class ExecResult
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ExecResult()
		{
			this.ImportSuccess = true;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="results">Action results</param>
		/// <param name="displayExtendStatusList">Display extend status list</param>
		public ExecResult(ActionResults results, Dictionary<int, string> displayExtendStatusList)
		{
			this.Results = results;
			this.DisplayExtendStatusList = displayExtendStatusList;
		}

		/// <summary>Action results</summary>
		public ActionResults Results { get; set; }
		/// <summary>Display extend status list</summary>
		public Dictionary<int, string> DisplayExtendStatusList { get; set; }
		/// <summary>Exclude message</summary>
		public string ExcludeMessage { get; set; }
		/// <summary>Result message</summary>
		public string ResultMessage { get; set; }
		/// <summary>Error data</summary>
		public List<Dictionary<string, string>> ErrorData { get; set; }
		/// <summary>Import success</summary>
		public bool ImportSuccess { get; set; }
		/// <summary>Total case</summary>
		public int TotalCase { get; set; }
		/// <summary>Success case</summary>
		public int SuccessCase { get; set; }
		/// <summary>Error case</summary>
		public int ErrorCase { get; set; }
		/// <summary>File name</summary>
		public string FileName { get; set; }
		/// <summary>Import type</summary>
		public string ImportType { get; set; }
		/// <summary>非同期実行か</summary>
		public bool IsAsyncExec { get; set; }
	}
}
