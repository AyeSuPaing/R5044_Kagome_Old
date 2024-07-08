/*
=========================================================================================================
  Module      : Workflow execute request(WorkflowExecRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.App.Common.Order.Workflow
{
	/// <summary>
	/// Workflow execute request
	/// </summary>
	[Serializable]
	public class WorkflowExecRequest
	{
		/// <summary>Index</summary>
		public int Index { get; set; }
		/// <summary>Order id</summary>
		public string OrderId { get; set; }
		/// <summary>Cassette action</summary>
		public string CassetteAction { get; set; }
	}
}
