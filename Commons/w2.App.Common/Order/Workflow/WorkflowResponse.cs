/*
=========================================================================================================
  Module      : Workflow response(WorkflowResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Domain.OrderWorkflowExecHistory;

namespace w2.App.Common.Order.Workflow
{
	/// <summary>
	/// Workflow response
	/// </summary>
	[Serializable]
	public class WorkflowResponse
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public WorkflowResponse()
		{
			this.Orders = new OrderResponse[0];
			this.Pages = new string[0];
			this.TotalCase = "0";
			this.IsOver100 = false;
		}

		/// <summary>Orders</summary>
		public OrderResponse[] Orders { get; set; }
		/// <summary>Pages</summary>
		public string[] Pages { get; set; }
		/// <summary>Has value</summary>
		public bool HasValue { get; set; }
		/// <summary>Total case</summary>
		public string TotalCase { get; set; }
		/// <summary>Total page</summary>
		public int TotalPage { get; set; }
		/// <summary>Detail kbn</summary>
		public string DetailKbn { get; set; }
		/// <summary>Actions</summary>
		public KeyValueItem[] Actions { get; set; }
		/// <summary>Actions for confirm</summary>
		public KeyValueItem[] ActionsForConfirm { get; set; }
		/// <summary>Has search box</summary>
		public bool HasSearchBox { get; set; }
		/// <summary>Is over 100</summary>
		public bool IsOver100 { get; set; }
		/// <summary>Running workflow history</summary>
		public OrderWorkflowExecHistoryModel RunningWorkflowHistory { get; set; }
		/// <summary>Update status valid</summary>
		public bool UpdateStatusValid { get; set; }
	}
}
