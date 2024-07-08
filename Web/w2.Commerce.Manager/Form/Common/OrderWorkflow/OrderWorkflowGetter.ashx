<%--
=========================================================================================================
  Module      : Order Workflow Getter(OrderWorkflowGetter.ashx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>
<%@ WebHandler Language="C#" Class="OrderWorkflowGetter" %>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using w2.App.Common.Order.Workflow;
using w2.Common.Logger;
using w2.Domain.ShopOperator;

/// <summary>
/// Order Workflow Getter
/// </summary>
public class OrderWorkflowGetter : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{
	/// <summary>
	/// プロセスリクエスト
	/// </summary>
	/// <param name="context">コンテキスト</param>
	public void ProcessRequest(HttpContext context)
	{
		// Set context and content type
		this.Context = context;
		context.Response.AddHeader("content-type", "application/json");
		context.Response.AddHeader("charset", "UTF-8");

		// Execute process by action kbn and create the response
		var response = ExecuteProcess();
		context.Response.Write(response);
	}

	/// <summary>
	/// Execute process
	/// </summary>
	/// <returns>Response</returns>
	private string ExecuteProcess()
	{
		var shopId = this.LoginShopOperator.ShopId;
		switch (this.ActionKbn)
		{
			case Constants.ORDERWORKFLOW_ACTION_KBN_ORDER_COUNT:
				var orderCountResponses = new List<OrderWorkflowReport>();
				foreach (var request in this.Requests)
				{
					var orderCount = GetWorkflowTargetOrderCount(
						shopId,
						request.WorkflowType,
						request.WorkflowKbn,
						request.WorkflowNo);
					orderCountResponses.Add(
						new OrderWorkflowReport
						{
							WorkflowType = request.WorkflowType,
							WorkflowKbn = request.WorkflowKbn,
							WorkflowNo = request.WorkflowNo,
							OrderCount = StringUtility.ToNumeric(orderCount),
						});
				}
				return BasePageHelper.ConvertObjectToJsonString(orderCountResponses);

			case Constants.ORDERWORKFLOW_ACTION_KBN_ACTION_CONDITIONS:
				var workflowResponses = new List<OrderWorkflowReport>();
				foreach (var request in this.Requests)
				{
					var orderCount = GetWorkflowTargetOrderCount(
						shopId,
						request.WorkflowType,
						request.WorkflowKbn,
						request.WorkflowNo);
					var displayWorkflowActionAndConditions = GetDisplayWorkflowActionAndConditions(
						shopId,
						request.WorkflowType,
						request.WorkflowKbn,
						request.WorkflowNo);
					workflowResponses.Add(
						new OrderWorkflowReport
						{
							WorkflowType = request.WorkflowType,
							WorkflowKbn = request.WorkflowKbn,
							WorkflowNo = request.WorkflowNo,
							OrderCount = StringUtility.ToNumeric(orderCount),
							Actions = displayWorkflowActionAndConditions.Item1,
							Conditions = displayWorkflowActionAndConditions.Item2,
						});
				}
				return BasePageHelper.ConvertObjectToJsonString(workflowResponses);

			default:
				throw new Exception("未定義のActionKbn：" + this.ActionKbn);
		}
	}

	/// <summary>
	/// Get display workflow action and conditions
	/// </summary>
	/// <param name="shopId">Shop id</param>
	/// <param name="workflowType">Workflow type</param>
	/// <param name="workflowKbn">Workflow kbn</param>
	/// <param name="workflowNo">Workflow no</param>
	/// <returns>Two results: actions and conditions</returns>
	private Tuple<KeyValueItem[], KeyValueItem[]> GetDisplayWorkflowActionAndConditions(
		string shopId,
		string workflowType,
		string workflowKbn,
		string workflowNo)
	{
		var actions = new List<KeyValueItem>();
		var conditions = new List<KeyValueItem>();
		try
		{
			var displayWorkflowActionAndConditions = new WorkflowActionAndCondition(shopId)
				.GetDisplayWorkflowActionAndConditions(
					workflowKbn,
					workflowNo,
					workflowType);
			actions.AddRange(displayWorkflowActionAndConditions.Item1);
			conditions.AddRange(displayWorkflowActionAndConditions.Item2);
		}
		catch (Exception ex)
		{
			FileLogger.WriteError(
				CreateActionWorkflowFailedLogMessage(
					workflowType,
					workflowKbn,
					workflowNo),
				ex);
		}
		var results = new Tuple<KeyValueItem[], KeyValueItem[]>(actions.ToArray(), conditions.ToArray());
		return results;
	}

	/// <summary>
	/// Get workflow target order count
	/// </summary>
	/// <param name="shopId">Shop id</param>
	/// <param name="workflowType">Workflow type</param>
	/// <param name="workflowKbn">Workflow kbn</param>
	/// <param name="workflowNo">Workflow no</param>
	/// <returns>Workflow target order count</returns>
	private int GetWorkflowTargetOrderCount(
		string shopId,
		string workflowType,
		string workflowKbn,
		string workflowNo)
	{
		var orderCount = 0;
		try
		{
			orderCount = new WorkflowUtility().GetOrderCountForWorkflowSetting(
				shopId,
				workflowType,
				workflowKbn,
				workflowNo);
		}
		catch (Exception ex)
		{
			FileLogger.WriteError(
				CreateActionWorkflowFailedLogMessage(
					workflowType,
					workflowKbn,
					workflowNo),
				ex);
		}
		return orderCount;
	}

	/// <summary>
	/// Create action workflow failed log message
	/// </summary>
	/// <param name="workflowType">Workflow type</param>
	/// <param name="workflowKbn">Workflow kbn</param>
	/// <param name="workflowNo">Workflow no</param>
	/// <returns>Log message</returns>
	private string CreateActionWorkflowFailedLogMessage(
		string workflowType,
		string workflowKbn,
		string workflowNo)
	{
		var logMessage = string.Format(
			"受注ワークフロー失敗：workflow_type={0},workflow_kbn={1},workflow_no={2}",
			workflowType,
			workflowKbn,
			workflowNo);
		return logMessage;
	}

	/// <summary>コンテキスト</summary>
	private HttpContext Context { get; set; }
	/// <summary>Action kbn</summary>
	private string ActionKbn
	{
		get { return this.Context.Request.Form[Constants.PARAM_ORDERWORKFLOW_ACTION_KBN]; }
	}
	/// <summary>Login shop operator</summary>
	private ShopOperatorModel LoginShopOperator
	{
		get
		{
			return (ShopOperatorModel)this.Context.Session[Constants.SESSION_KEY_LOGIN_SHOP_OPERTOR] ?? new ShopOperatorModel();
		}
	}
	/// <summary>The requests</summary>
	private OrderWorkflowReport[] Requests
	{
		get
		{
			var requestString = this.Context.Request.Form[Constants.PARAM_ORDERWORKFLOW_REQUESTS];
			var requests = BasePageHelper.DeserializeJsonObject<List<OrderWorkflowReport>>(requestString);
			return requests.ToArray();
		}
	}
	/// <summary>再利用可能か</summary>
	public bool IsReusable
	{
		get { return false; }
	}
}
