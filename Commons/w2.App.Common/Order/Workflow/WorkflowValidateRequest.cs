/*
=========================================================================================================
  Module      : Workflow validate request(WorkflowValidateRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.App.Common.Order.Workflow
{
	/// <summary>
	/// Workflow validate request
	/// </summary>
	[Serializable]
	public class WorkflowValidateRequest
	{
		/// <summary>Workflow no</summary>
		public string WorkflowNo { get; set; }
		/// <summary>Workflow kbn</summary>
		public string WorkflowKbn { get; set; }
		/// <summary>Workflow type</summary>
		public string WorkflowType { get; set; }
		/// <summary>Extend status date</summary>
		public string ExtendStatusDate { get; set; }
		/// <summary>Shipping date update</summary>
		public string ShippingDateUpdate { get; set; }
		/// <summary>Scheduled shipping date update</summary>
		public string ScheduledShippingDateUpdate { get; set; }
		/// <summary>Next shipping date update</summary>
		public string NextShippingDateUpdate { get; set; }
		/// <summary>Next next shipping date update</summary>
		public string NextNextShippingDateUpdate { get; set; }
	}
}