/*
=========================================================================================================
  Module      : Workflow request(WorkflowRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;

namespace w2.App.Common.Order.Workflow
{
	/// <summary>
	/// Workflow request
	/// </summary>
	[Serializable]
	public class WorkflowRequest
	{
		/// <summary>Current page</summary>
		public int CurrentPage { get; set; }
		/// <summary>Workflow no</summary>
		public string WorkflowNo { get; set; }
		/// <summary>Workflow kbn</summary>
		public string WorkflowKbn { get; set; }
		/// <summary>Workflow type</summary>
		public string WorkflowType { get; set; }
		/// <summary>Workflow display kbn</summary>
		public string WorkflowDisplayKbn { get; set; }
		/// <summary>Search condition</summary>
		public Hashtable SearchCondition { get; set; }
		/// <summary>Extend status date</summary>
		public DateTime? ExtendStatusDate { get; set; }
		/// <summary>Shipping date update</summary>
		public DateTime? ShippingDateUpdate { get; set; }
		/// <summary>Scheduled shipping date update</summary>
		public DateTime? ScheduledShippingDateUpdate { get; set; }
		/// <summary>Next shipping date update</summary>
		public DateTime? NextShippingDateUpdate { get; set; }
		/// <summary>Next next shipping date update</summary>
		public DateTime? NextNextShippingDateUpdate { get; set; }
		/// <summary>配送不可エリア変更</summary>
		public string FixedPurchaseStopUnavailableShippingAreaUpdate { get; set; }
		/// <summary>Orders</summary>
		public WorkflowExecRequest[] Orders { get; set; }
		/// <summary>Exec type</summary>
		public string ExecType { get; set; }
		/// <summary>Export key</summary>
		public string ExportKey { get; set; }
		/// <summary>Total name</summary>
		public string TotalName { get; set; }
		/// <summary>Is unsysnc</summary>
		public bool IsUnSysnc { get; set; }
	}
}
