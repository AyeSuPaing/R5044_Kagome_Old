/*
=========================================================================================================
  Module      : Order workflow report(OrderWorkflowReport.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;

namespace w2.App.Common.Order.Workflow
{
	/// <summary>
	/// Order workflow report
	/// </summary>
	[Serializable]
	public class OrderWorkflowReport
	{
		/// <summary>Workflow name</summary>
		[JsonProperty("workflowName")]
		public string WorkflowName { get; set; }
		/// <summary>Description</summary>
		[JsonProperty("description")]
		public string Description { get; set; }
		/// <summary>Order count</summary>
		[JsonProperty("orderCount")]
		public string OrderCount { get; set; }
		/// <summary>Conditions</summary>
		[JsonProperty("conditions")]
		public KeyValueItem[] Conditions { get; set; }
		/// <summary>Actions</summary>
		[JsonProperty("actions")]
		public KeyValueItem[] Actions { get; set; }
		/// <summary>Workflow detail kbn</summary>
		[JsonProperty("workflowDetailKbn")]
		public string WorkflowDetailKbn { get; set; }
		/// <summary>Display kbn</summary>
		[JsonProperty("displayKbn")]
		public string DisplayKbn { get; set; }
		/// <summary>Workflow type</summary>
		[JsonProperty("workflowType")]
		public string WorkflowType { get; set; }
		/// <summary>Workflow no</summary>
		[JsonProperty("workflowNo")]
		public string WorkflowNo { get; set; }
		/// <summary>Workflow kbn</summary>
		[JsonProperty("workflowKbn")]
		public string WorkflowKbn { get; set; }
		/// <summary>Url</summary>
		[JsonProperty("url")]
		public string UrlWorkflow { get; set; }
	}
}
