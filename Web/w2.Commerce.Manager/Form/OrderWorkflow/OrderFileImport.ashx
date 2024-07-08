<%@ WebHandler Language="C#" Class="OrderFileImport" %>
/*
=========================================================================================================
  Module      : Order file import(OrderFileImport.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Web;
using w2.Domain.ShopOperator;
using Newtonsoft.Json;
using w2.App.Common.Order.Import;
using w2.App.Common.Order.Workflow;
using w2.Common.Logger;
using w2.Domain;
using w2.Domain.OrderWorkflowExecHistory;

/// <summary>
/// Order file import
/// </summary>
public class OrderFileImport : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{
	/// <summary>
	/// Process request
	/// </summary>
	/// <param name="context">Httpcontext</param>
	public void ProcessRequest(HttpContext context)
	{
		context.Response.ContentType = "text/plain";
		var uploadedFile = context.Request.Files[0];
		var result = ImportOrderFile(
			uploadedFile,
			context.Request.Form["workflowName"],
			int.Parse(context.Request.Form["workflowNo"].ToString()),
			context.Request.Form["selectedValue"],
			int.Parse(context.Request.Form["selectedIndex"].ToString()),
			bool.Parse(context.Request.Form["isShipmentEntry"].ToString()),
			context.Request.Form["fileNamePattern"],
			context.Request.Form["mailTemplateId"],
			context.Request.Form["workflowKbn"],
			context);
		context.Response.Write(result);
	}

	/// <summary>
	/// Import order file
	/// </summary>
	/// <param name="file">File upload</param>
	/// <param name="workflowName">work flow name</param>
	/// <param name="workflowNo">work flow no</param>
	/// <param name="selectedValue">Selected value</param>
	/// <param name="selectedIndex">Selected index</param>
	/// <param name="isShipmentEntry">Is shipment entry</param>
	/// <param name="fileNamePattern">File name pattern</param>
	/// <param name="mailTemplateId">Mail template id</param>
	/// <param name="workflowKbn">work flow kbn</param>
	/// <param name="context">Http context</param>
	/// <returns>Import result</returns>
	public string ImportOrderFile(
		HttpPostedFile file,
		string workflowName,
		int workflowNo,
		string selectedValue,
		int selectedIndex,
		bool isShipmentEntry,
		string fileNamePattern,
		string mailTemplateId,
		string workflowKbn,
		HttpContext context)
	{
		var operatorName = ((ShopOperatorModel)context.Session[Constants.SESSION_KEY_LOGIN_SHOP_OPERTOR]).Name;

		var history = InsertOrderWorkflowExecHistory(
			workflowName,
			Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_PLACE_WORKFLOW,
			Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_KBN_DAY,
			workflowNo);

		var importResponse = OrderFileImportProcess.Start(
			selectedValue,
			file,
			fileNamePattern,
			mailTemplateId,
			isShipmentEntry,
			operatorName);

		var isSuccess = importResponse.ImportSuccess;
		
		// 実行結果メッセージをログ出力
		if (isSuccess)
		{
			FileLogger.WriteInfo(importResponse.ResultMessage + "　取り込みファイル：" + importResponse.FileName);
		}
		else
		{
			// 失敗の場合はエラーログを落とす（受注ワークフロー失敗のログも一緒にエラーログに落ちるため）
			FileLogger.WriteError(importResponse.ResultMessage + "　取り込みファイル：" + importResponse.FileName);
		}

		WorkFlowHistoryHelper.UpdateHistoryFunction(
			isSuccess
				? w2.App.Common.Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_OK
				: w2.App.Common.Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_NG,
			string.Format(
				"{0}/{1}",
				importResponse.SuccessCase,
				importResponse.TotalCase),
			isSuccess
				? w2.App.Common.Constants.FLG_ORDERWORKFLOWEXECHISTORY_MESSAGE_FOR_WORKFLOW_SUCCESS
				: w2.App.Common.Constants.FLG_ORDERWORKFLOWEXECHISTORY_MESSAGE_FOR_WORKFLOW_FAILURE,
			history.OrderWorkflowExecHistoryId);
		
		return JsonConvert.SerializeObject(importResponse);
	}

	/// <summary>
	/// 受注ワークフローの実行履歴を登録する
	/// </summary>
	/// <param name="workflowName">ワークフロー名</param>
	/// <param name="workflowType">ワークフロータイプ</param>
	/// <param name="workflowKbn"></param>
	/// <param name="workflowNo"></param>
	/// <returns>受注ワークフロー実行履歴</returns>
	private OrderWorkflowExecHistoryModel InsertOrderWorkflowExecHistory(string workflowName, string workflowType, string workflowKbn, int workflowNo)
	{
		var shopOperatorModel
			= (ShopOperatorModel)HttpContext.Current.Session[Constants.SESSION_KEY_LOGIN_SHOP_OPERTOR]
			  ?? new ShopOperatorModel();
		var history = WorkFlowHistoryHelper.InsertFunction(shopOperatorModel.ShopId, workflowName, workflowType, workflowKbn, workflowNo, shopOperatorModel.Name);
		return history;
	}

	/// <summary>Is reusable</summary>
	public bool IsReusable { get { return false; } }
}