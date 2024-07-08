/*
=========================================================================================================
  Module      : 注文ワークフロー情報一覧ページ処理(OrderWorkflowList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Services;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using w2.App.Common.DataCacheController;
using w2.App.Common.DataExporters;
using w2.App.Common.Order;
using w2.App.Common.Order.Import;
using w2.App.Common.Order.Payment.ECPay;
using w2.App.Common.Order.Workflow;
using w2.App.Common.OrderExtend;
using w2.App.Common.Pdf.PdfCreater;
using w2.App.Common.SendMail;
using w2.Common.Extensions;
using w2.Common.Logger;
using w2.Common.Web;
using w2.Domain;
using w2.Domain.ShopOperator;

/// <summary>
/// Order workflow list
/// </summary>
public partial class Form_OrderWorkflow_OrderWorkflowList : OrderWorkflowPage
{
	#region Ajax methods
	/// <summary>
	/// Get workflow kbn list
	/// </summary>
	/// <returns>A workflow kbns as json string</returns>
	[WebMethod]
	public static string GetWorkflowKbnList()
	{
		return HandleErrorForAction(() =>
		{
		return GetJsonReponseByValueText(
			Constants.TABLE_ORDERWORKFLOWSETTING,
			Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_KBN);
		});
	}

	/// <summary>
	/// Get external payment statuses
	/// </summary>
	/// <returns>An external payment statuses as json string</returns>
	[WebMethod]
	public static string GetExternalPaymentStatuses()
	{
		return HandleErrorForAction(() =>
		{
		return GetJsonReponseByValueText(
			Constants.TABLE_ORDER,
			Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS,
			true);
		});
	}

	/// <summary>
	/// Get taiwan invoice statuses
	/// </summary>
	/// <returns>A taiwan invoice statuses as json string</returns>
	[WebMethod]
	public static string GetTaiwanInvoiceStatuses()
	{
		return HandleErrorForAction(() =>
		{
		return GetJsonReponseByValueText(
			Constants.TABLE_TWORDERINVOICE,
			Constants.FIELD_TWORDERINVOICE_TW_INVOICE_STATUS,
			true);
		});
	}

	/// <summary>
	/// Get fixed purchase statuses
	/// </summary>
	/// <returns>A fixed purchase statuses as json string</returns>
	[WebMethod]
	public static string GetFixedPurchaseStatuses()
	{
		return HandleErrorForAction(() =>
		{
		return GetJsonReponseByValueText(
			Constants.TABLE_FIXEDPURCHASE,
			Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_STATUS,
			true);
		});
	}

	/// <summary>
	/// Get fixed purchase payment statuses
	/// </summary>
	/// <returns>A fixed purchase payment statuses as json string</returns>
	[WebMethod]
	public static string GetFixedPurchasePaymentStatuses()
	{
		return HandleErrorForAction(() =>
		{
		return GetJsonReponseByValueText(
			Constants.TABLE_FIXEDPURCHASE,
			Constants.FIELD_FIXEDPURCHASE_PAYMENT_STATUS,
			true);
		});
	}

	/// <summary>
	/// Get update date extend statuses
	/// </summary>
	/// <returns>An update date extend statuses as json string</returns>
	[WebMethod]
	public static string GetUpdateDateExtendStatuses()
	{
		return HandleErrorForAction(() =>
		{
		var extendStatusList = DomainFacade.Instance.OrderExtendStatusSettingService.GetOrderExtendStatusSetting();
		var extendStatusNos = extendStatusList.Select(item =>
			new KeyValueItem(
				string.Format("{0}：{1}", item.ExtendStatusNo, item.ExtendStatusName),
				item.ExtendStatusNo.ToString()
		)).ToArray();
		return GetJsonReponse(extendStatusNos, true, true);
		});
	}

	/// <summary>
	/// Get update date order statuses
	/// </summary>
	/// <returns>An update date order statuses as json string</returns>
	[WebMethod]
	public static string GetUpdateDateOrderStatuses()
	{
		return HandleErrorForAction(() =>
		{
		var updateStatuses = ValueText.GetValueKvpArray(Constants.TABLE_ORDER, FIELD_UPDATE_STATUS)
			.Where(item => (Constants.REALSTOCK_OPTION_ENABLED
				|| (item.Value != Constants.FIELD_ORDER_ORDER_STOCKRESERVED_DATE)))
			.Select(item => new KeyValueItem(item.Key, item.Value))
			.ToArray();
		return GetJsonReponse(updateStatuses, true);
		});
	}

	/// <summary>
	/// Get invoice bundle flags
	/// </summary>
	/// <returns>An invoice bundle flags as json string</returns>
	[WebMethod]
	public static string GetInvoiceBundleFlags()
	{
		return HandleErrorForAction(() =>
		{
		return GetJsonReponseByValueText(
			Constants.TABLE_ORDER,
			Constants.FIELD_ORDER_INVOICE_BUNDLE_FLG,
			true);
		});
	}

	/// <summary>
	/// Get another shipping flags
	/// </summary>
	/// <returns>An another shipping flg as json string</returns>
	[WebMethod]
	public static string GetAnotherShippingFlags()
	{
		return HandleErrorForAction(() =>
		{
		return GetJsonReponseByValueText(
			Constants.TABLE_ORDERSHIPPING,
			Constants.FIELD_ORDERSHIPPING_ANOTHER_SHIPPING_FLG,
			true);
		});
	}

	/// <summary>
	/// Get shipping statuses
	/// </summary>
	/// <returns>A shipping statuses as json string</returns>
	[WebMethod]
	public static string GetShippingStatuses()
	{
		return HandleErrorForAction(() =>
		{
		return GetJsonReponseByValueText(
			Constants.TABLE_ORDERSHIPPING,
			Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS,
			true);
		});
	}

	/// <summary>
	/// Get extend status nos
	/// </summary>
	/// <returns>An extend status no as json string</returns>
	[WebMethod]
	public static string GetExtendStatusNos()
	{
		return HandleErrorForAction(() =>
		{
		var extendStatusList = DomainFacade.Instance.OrderExtendStatusSettingService.GetOrderExtendStatusSetting();
		var extendStatusNos = extendStatusList.Select(item =>
			new KeyValueItem(
				string.Format("{0}：{1}", item.ExtendStatusNo, item.ExtendStatusName),
				item.ExtendStatusNo.ToString()
		)).ToArray();
		return GetJsonReponse(extendStatusNos, true, true);
		});
	}

	/// <summary>
	/// Get extend statuses
	/// </summary>
	/// <returns>An extend statuses as json string</returns>
	[WebMethod]
	public static string GetExtendStatuses()
	{
		return HandleErrorForAction(() =>
		{
		return GetJsonReponseByValueText(
			Constants.TABLE_ORDER,
			Constants.FIELD_ORDER_EXTEND_STATUS2,
			true);
		});
	}

	/// <summary>
	/// Get memo flags
	/// </summary>
	/// <returns>A memo flg as json string</returns>
	[WebMethod]
	public static string GetMemoFlags()
	{
		return HandleErrorForAction(() =>
		{
		return GetJsonReponseByValueText(
			Constants.TABLE_ORDER,
			Constants.FIELD_ORDER_MEMO,
			true);
		});
	}

	/// <summary>
	/// Get shipping prefectures
	/// </summary>
	/// <returns>Shipping prefectures</returns>
	[WebMethod]
	public static string GetShippingPrefectures()
	{
		return HandleErrorForAction(() =>
		{
		// 配送先 都道府県
		var prefectures = Constants.STR_PREFECTURES_LIST
			.Select(prefecture
				=> new KeyValueItem(prefecture, prefecture));
		return GetJsonReponse(prefectures.ToArray(), false, false);
		});
	}

	/// <summary>
	/// Get order workflow list
	/// </summary>
	/// <param name="workflowKbn">A workflow kbn</param>
	/// <param name="workflowName">A workflow name</param>
	/// <returns>A order workflow list as json string</returns>
	[WebMethod]
	public static string GetOrderWorkflowList(string workflowKbn, string workflowName)
	{
		return HandleErrorForAction(() =>
		{
		var orderWorkFlowSettingList =
			DomainFacade.Instance.OrderWorkflowSettingService.GetOrderWorkflowSettings(
				workflowKbn,
				StringUtility.SqlLikeStringSharpEscape(workflowName));

		var orderWorkflows = new List<OrderWorkflowReport>();
		foreach (var setting in orderWorkFlowSettingList)
		{
			var orderWorkflow = new OrderWorkflowReport
			{
				WorkflowName = setting.WorkflowName,
				Description = setting.Desc1,
				WorkflowDetailKbn = setting.WorkflowDetailKbn,
				DisplayKbn = setting.DisplayKbn,
				WorkflowType = setting.WorkflowType,
				WorkflowNo = setting.WorkflowNo.ToString(),
				WorkflowKbn = setting.WorkflowKbn
			};
			orderWorkflows.Add(orderWorkflow);
		}

		var result = CreateReportJsonString(orderWorkflows);
		return result;
		});
	}

	/// <summary>
	/// Get workflow disp setting
	/// </summary>
	/// <returns>Workflow disp setting json data</returns>
	[WebMethod]
	public static string GetWorkflowDispSetting()
	{
		return HandleErrorForAction(() =>
		{
		var data = DomainFacade.Instance.ManagerListDispSettingService.GetAllByDispSettingKbn(
			Constants.CONST_DEFAULT_SHOP_ID,
			Constants.FLG_MANAGERLISTDISPSETTING_DISPSETTINGKBN_ORDERWORKFLOW);
		return JsonConvert.SerializeObject(data);
		});
	}

	/// <summary>
	/// Get login shop operator
	/// </summary>
	/// <returns>Login shop operator model</returns>
	private static ShopOperatorModel GetLoginShopOperator()
	{
		var loginShopOperator =
			(ShopOperatorModel)HttpContext.Current.Session[Constants.SESSION_KEY_LOGIN_SHOP_OPERTOR]
				?? new ShopOperatorModel();
		return loginShopOperator;
	}

	/// <summary>
	/// Get order extends
	/// </summary>
	/// <returns>Order extends</returns>
	protected string GetOrderExtends()
	{
		var orderExtendList = DataCacheControllerFacade.GetOrderExtendSettingCacheController().CacheData.SettingModels;
		var orderExtends = new List<object>
		{
			new
			{
				setting_id = string.Empty,
				setting_name = string.Empty,
				setting_type = string.Empty,
				setting_default = string.Empty
			}
		};

		foreach (var item in orderExtendList)
		{
			var defaultInputs = new List<KeyValueItem>
			{
				new KeyValueItem(string.Empty, string.Empty)
			};
			if (item.InputType != Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_TEXT)
			{
				defaultInputs.AddRange(OrderExtendCommon.GetListItemForManager(item.InputDefault)
					.Select(input => new KeyValueItem(input.Value, input.Text)));
			}

			var orderExtend = new
			{
				setting_id = item.SettingId,
				setting_name = item.SettingName,
				setting_type = item.InputType,
				setting_default = defaultInputs.ToArray()
			};
			orderExtends.Add(orderExtend);
		}

		return CreateReportJsonString(orderExtends);
	}

	/// <summary>
	/// Get order extend flg
	/// </summary>
	/// <returns>Order extend flg</returns>
	protected string GetOrderExtendFlg()
	{
		return GetJsonReponseByValueText(Constants.TABLE_ORDER, Constants.SEARCH_FIELD_ORDER_EXTEND_FLG, true);
	}

	/// <summary>
	/// 完了状態コードのJSON文字列を取得
	/// </summary>
	/// <returns>完了状態コードのJSON文字列</returns>
	[WebMethod]
	public static string GetShippingStatusCodes()
	{
		return HandleErrorForAction(() =>
		{
		return GetJsonReponseByValueText(
			Constants.TABLE_ORDERSHIPPING,
			Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS_CODE,
			true);
		});
	}

	/// <summary>
	/// 現在の状態のJSON文字列を取得
	/// </summary>
	/// <returns>現在の状態のJSON文字列</returns>
	[WebMethod]
	public static string GetShippingCurrentStatuses()
	{
		return HandleErrorForAction(() =>
		{
			return GetJsonReponseByValueText(
				Constants.TABLE_ORDERSHIPPING,
				Constants.FIELD_ORDERSHIPPING_SHIPPING_CURRENT_STATUS,
				true);
		});
	}

	/// <summary>
	/// Get all status for store pickup
	/// </summary>
	/// <returns>Store pickup status list</returns>
	protected string GetStorePickupStatusList()
	{
		return GetJsonReponseByValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_STOREPICKUP_STATUS, true);
	}
	#endregion

	#region Step 2 methods
	/// <summary>
	/// Exec workflow
	/// </summary>
	/// <param name="data">Receive data</param>
	/// <returns>Exec result</returns>
	[WebMethod]
	public static string ExecWorkflow(WorkflowRequest data)
	{
		return HandleErrorForAction(() =>
		{
		var processId = Guid.NewGuid().ToString();
		var resultData = new WorkflowUtility(data).Exec(processId);
		var orderExtendStatusSettingList =
			DomainFacade.Instance.OrderExtendStatusSettingService.GetOrderExtendStatusSetting();
		resultData.DisplayExtendStatusList = orderExtendStatusSettingList.ToDictionary(
			item => item.ExtendStatusNo,
			item => item.ExtendStatusName);
		if (resultData.IsAsync == false) WorkflowProcessObserver.Instance.RemoveWorkflowProcess(processId);
		return CreateReportJsonString(resultData);
		});
	}

	/// <summary>
	/// Get workflow exec current process
	/// </summary>
	/// <param name="processId">Process Id</param>
	/// <returns>Result exec process</returns>
	[WebMethod]
	public static string GetWorkflowCurrentProcess(string processId)
	{
		return HandleErrorForAction(() =>
		{
		var resultData = WorkflowProcessObserver.Instance.GetWorkflowProcess(processId);
		var orderExtendStatusSettingList =
			DomainFacade.Instance.OrderExtendStatusSettingService.GetOrderExtendStatusSetting();
		resultData.DisplayExtendStatusList = orderExtendStatusSettingList.ToDictionary(
			item => item.ExtendStatusNo,
			item => item.ExtendStatusName);
		if (resultData.IsSuccess
			|| resultData.IsSystemError)
		{
			WorkflowProcessObserver.Instance.RemoveWorkflowProcess(processId);
		}

		return CreateReportJsonString(resultData);
		});
	}

	/// <summary>
	/// Get workflow exec data
	/// </summary>
	/// <param name="data">Receive data</param>
	/// <returns>Workflow exec data</returns>
	[WebMethod]
	public static string GetWorkflowExecData(WorkflowRequest data)
	{
		return HandleErrorForAction(() =>
		{
			var responData = new WorkflowUtility(data).CreateResponseData();
			if (responData == null) return string.Empty;
			HttpContext.Current.Session[Constants.SESSIONPARAM_KEY_ORDER_TOTAL_COUNT] = int.Parse(responData.TotalCase.Replace(",",""));
			var actions = new WorkflowActionAndCondition(GetLoginShopOperator().ShopId)
				.GetActionsForWorkflowExecution(
			data.WorkflowKbn,
					data.WorkflowNo,
			data.WorkflowType);
			responData.Actions = actions.Item1;
			responData.ActionsForConfirm = actions.Item2;
		return CreateReportJsonString(responData);
		});
	}

	/// <summary>
	/// Get order file import setting
	/// </summary>
	/// <returns>Json string of order file data</returns>
	[WebMethod]
	public static string GetOrderFileImportSetting()
	{
		return HandleErrorForAction(() =>
		{
		var orderFiles = new List<ListItem>();
		var orderInfos = new List<string>();
		var fileNamePatternList = new List<string>();
		var usedMailTemplateIdList = new List<string>();

		var mainElement = XElement.Load(
			Constants.PHYSICALDIRPATH_COMMERCE_MANAGER + Constants.FILE_XML_ORDERFILEIMPORT_SETTING);
		var index = 0;
		foreach (var settingNode in mainElement.Elements("OrderFile"))
		{
			// ドロップダウン設定
			orderFiles.Add(new ListItem(settingNode.Element("Name").Value, settingNode.Element("Value").Value));

			// ファイル種別に関する情報格納
			orderInfos.Add(settingNode.Element("Info").Value);

			// Dictionary型にしてから設定値をAddしていく
			var importSettings = settingNode.Elements("ImportFileSetting")
				.ToDictionary(
					node => node.Attribute("key").Value,
					node => node.Attribute("value").Value);
			fileNamePatternList.Add(
				importSettings.ContainsKey("FileNamePattern")
					? importSettings["FileNamePattern"]
					: string.Empty);

			if (importSettings.ContainsKey("PastMonths")
				&& (string.IsNullOrEmpty(importSettings["PastMonths"]) == false))
			{
				orderInfos[index] = string.Format(
					"{0}{1}",
					orderInfos[index],
					WebMessages.GetMessages(
						WebMessages.ERRMSG_MANAGER_ORDERFILEIMPORT_PAST_MONTHS_INFO).Replace("@@ 1 @@", importSettings["PastMonths"]));
			}

			var onSuccessElement = settingNode.Element("OnSuccess");
			// Used mail template id
			usedMailTemplateIdList.Add(
				(onSuccessElement != null)
					? onSuccessElement.Attribute("usedMailTemplateId").Value
					: string.Empty);
			index++;
		}

		var orders = new List<OrderFileResponse>();
		index = 0;
		foreach (var file in orderFiles)
		{
			var value = file.Value.Split(':')[0];
			var canShipmentEntry =
				OrderCommon.CanShipmentEntry()
				&& ((value == Constants.KBN_ORDERFILE_SHIPPING_NO_LINK)
					|| (value == Constants.KBN_ORDERFILE_ECAT2000LINK)
					|| (value == Constants.KBN_B2_RAKUTEN_INCL_LINK)
					|| (value == Constants.KBN_B2_RAKUTEN_INCL_LINK_CLOUD)
					|| (value == Constants.KBN_ORDERFILE_PELICAN_RESULT_REPORT_LINK)) 
					|| (value == Constants.KBN_ORDERFILE_SHIPPING_DATA);
			orders.Add(new OrderFileResponse(
				file.Value,
				file.Text,
				orderInfos[index],
				canShipmentEntry,
				fileNamePatternList[index],
				usedMailTemplateIdList[index]));
			index++;
		}
		return CreateReportJsonString(orders);
		});
	}

	/// <summary>
	/// ポップアップリンクを作成する
	/// </summary>
	/// <param name="orderIdList">注文ID</param>
	/// <returns>注文情報詳細URL</returns>
	private static string CreatePopUpUrl(List<string> orderIdList)
	{
		var popupUrlString = new StringBuilder();
		foreach (var orderId in orderIdList)
		{
			if (popupUrlString.Length != 0)
			{
				popupUrlString.Append(", ");
			}

			// 注文詳細URL作成
			var orderDetailUrl =
				OrderPage.CreateOrderDetailUrl(
					orderId,
					true,
					false,
					Constants.PAGE_MANAGER_ORDERFILEIMPORT_LIST);

			// javascript作成
			var orderDetailLink = new StringBuilder();
			orderDetailLink.Append("<a href=\"javascript:open_window('");
			orderDetailLink.Append(WebSanitizer.UrlAttrHtmlEncode(orderDetailUrl.ToString()));
			orderDetailLink.Append("','ordercontact','width=828,height=600,top=110,left=380,status=NO,scrollbars=yes');\">");
			orderDetailLink.Append(orderId);
			orderDetailLink.Append("</a>");

			popupUrlString.Append(orderDetailLink);
		}
		return popupUrlString.ToString();
	}

	/// <summary>
	/// Send the mail to user and update extend status.
	/// </summary>
	/// <param name="successInfos">The success infos</param>
	/// <param name="mailTemplateId">The mail template identifier.</param>
	/// <returns>The error messages</returns>
	private static string SendMail(List<ImportBase.SuccessInfo> successInfos, string mailTemplateId)
	{
		var errorMessages = new StringBuilder();

		foreach (var succesInfo in successInfos)
		{
			try
			{
				OrderCommon.SendOrderMail(succesInfo.OrderId, mailTemplateId);
			}
			catch (Exception exception)
			{
				AppLogger.WriteError(
					string.Format(
						"SHIPPING_NO_LINK 行:{0} 注文ID:{1}",
						succesInfo.LineNo,
						succesInfo.OrderId),
					exception);
				errorMessages.AppendFormat(
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERFILEIMPORT_SEND_MAIL_ERROR),
					succesInfo.LineNo,
					succesInfo.OrderId);
			}
		}
		return errorMessages.ToString();
	}

	/// <summary>
	/// 実行中のワークフロー実行履歴URLを作成する
	/// </summary>
	/// <param name="orderWorkflowExecHistoryId">Order workflow exec history id</param>
	/// <returns>実行中のワークフロー実行履歴URL</returns>
	[WebMethod]
	public static string CreateUrlAnOrderWorkflowExecHistoryDetails(string orderWorkflowExecHistoryId)
	{
		if (string.IsNullOrEmpty(orderWorkflowExecHistoryId)) return string.Empty;
		var url = new UrlCreator(Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_ORDERWORKFLOW_EXEC_HISTORY_DETAILS)
			.AddParam(
				Constants.FIELD_ORDERWORKFLOWEXECHISTORY_ORDER_WORKFLOW_EXEC_HISTORY_ID,
				orderWorkflowExecHistoryId)
			.CreateUrl();
		return url;
	}

	/// <summary>
	/// Validate exec
	/// </summary>
	/// <param name="data">Workflow exec data</param>
	/// <returns>Error message</returns>
	[WebMethod]
	public static string ValidateExecData(WorkflowValidateRequest data)
	{
		return HandleErrorForAction(() =>
		{
		var errorMessage = new WorkflowUtility(data).Validate();
		return errorMessage;
		});
	}

	/// <summary>
	/// Get shipping label export setting
	/// </summary>
	/// <returns>Shipping label export setting</returns>
	protected string GetShippingLabelExportSetting()
	{
		var settings = new OrderFileExportShippingLabel().GetShippingLabelExportSettingList();
		var result = settings
			.Select((setting, index) =>
				new
				{
					Index = index,
					Displayname = setting.DisplayName,
				})
			.ToArray();
		return CreateReportJsonString(result);
	}

	/// <summary>
	/// Get download anchor text setting
	/// </summary>
	/// <returns>Download anchor text setting</returns>
	protected string GetDownloadAnchorTextSetting()
	{
		var settings = DataExporterCreater.GetDownloadAnchorTextForCommerceManager(
			Constants.PROJECT_NO,
			Constants.ExportKbn.OrderList);
		var result = settings
			.Select(setting =>
				new KeyValueItem
				{
					Key = setting.Key.ToString(),
					Value = setting.Value,
				})
			.ToArray();
		return CreateReportJsonString(result);
	}
	#endregion

	#region Helper methods
	/// <summary>
	/// Get json response by value text
	/// </summary>
	/// <param name="tableName">The table name</param>
	/// <param name="fieldName">The field name</param>
	/// <param name="addEmptyValue">Add empty value</param>
	/// <returns>A json response</returns>
	public static string GetJsonReponseByValueText(
		string tableName,
		string fieldName,
		bool addEmptyValue = false)
	{
		var pairs = ValueText.GetValueKvpArray(tableName, fieldName);
		var result = new List<KeyValueItem>();

		if (addEmptyValue)
		{
			result.Add(new KeyValueItem(string.Empty, string.Empty));
		}

		foreach (var pair in pairs)
		{
			result.Add(new KeyValueItem(pair.Key, pair.Value));
		}
		return CreateReportJsonString(result);
	}

	/// <summary>
	/// Get json response
	/// </summary>
	/// <param name="keyValueItems">The key value items</param>
	/// <param name="addEmptyValue">Add empty value</param>
	/// <param name="isExtendStatus">Is extend status</param>
	/// <returns>A json response</returns>
	public static string GetJsonReponse(
		KeyValueItem[] keyValueItems,
		bool addEmptyValue = false,
		bool isExtendStatus = false)
	{
		var result = new List<KeyValueItem>();

		if (addEmptyValue)
		{
			result.Add(new KeyValueItem(string.Empty, string.Empty));
		}

		foreach (var item in keyValueItems)
		{
			if (isExtendStatus)
			{
				result.Add(new KeyValueItem(item.Value, item.Key));
				continue;
			}

			result.Add(new KeyValueItem(item.Key, item.Value));
		}
		return CreateReportJsonString(result.ToArray());
	}

	/// <summary>
	/// Handle error for action
	/// </summary>
	/// <typeparam name="TResult">Type of result action</typeparam>
	/// <param name="action">The action</param>
	/// <returns>Result action</returns>
	private static TResult HandleErrorForAction<TResult>(Func<TResult> action)
	{
		try
		{
			return action.Invoke();
		}
		catch (Exception ex)
		{
			FileLogger.WriteError(ex);
			throw;
		}
	}
	#endregion

	#region Export
	/// <summary>
	/// Create master export setting items
	/// </summary>
	/// <param name="type">Workflow type</param>
	/// <returns>A list of items</returns>
	[WebMethod]
	public static string CreateMasterExportSettingItems(string type)
	{
		return HandleErrorForAction(() =>
		{
		var download = new DownloadUtility().CreateItems(type);
		var result = download.Select(item => new KeyValueItem(item.Key, item.Value))
			.ToArray();
		return GetJsonReponse(result);
		});
	}

	/// <summary>
	/// Export
	/// </summary>
	/// <param name="data">Receice data</param>
	/// <returns>True: if set session success, otherwise: false</returns>
	[WebMethod(EnableSession = true)]
	public static bool Export(WorkflowRequest data)
	{
		return HandleErrorForAction(() =>
		{
		var workflowUtility = new WorkflowUtility(data);
		var download = new DownloadUtility();
		download.CreateSearchInputParams(data.WorkflowType, workflowUtility);
		var param = download.ExecuteExport(data.ExportKey);

		if (param == null)
		{
			HttpContext.Current.Session[Constants.SESSION_KEY_ERROR_MSG] =
				WebMessages.GetMessages(WebMessages.ERRMSG_MASTEREXPORTSETTING_SETTING_ID_NOT_SELECTED);
			return false;
		}

		HttpContext.Current.Session[Constants.SESSION_KEY_PARAM] = param;
		return true;
		});
	}

	/// <summary>
	/// Check and get param export action
	/// </summary>
	/// <param name="data">Data</param>
	/// <returns>Param export</returns>
	public static Hashtable CheckAndGetParamExportAction(WorkflowRequest data)
	{
		var workflowUtility = new WorkflowUtility(data);
		var param = workflowUtility.GetSearchParamsOrder();

		if (param == null)
		{
			HttpContext.Current.Session[Constants.SESSION_KEY_ERROR_MSG] =
				WebMessages.GetMessages(WebMessages.ERRMSG_MASTEREXPORTSETTING_SETTING_ID_NOT_SELECTED);
			return null;
		}

		HttpContext.Current.Session[Constants.SESSION_KEY_PARAM] = param;
		return param;
	}

	/// <summary>
	/// Export pdf output
	/// </summary>
	/// <param name="data">Receice data</param>
	/// <returns>True: if set session success, otherwise: false</returns>
	[WebMethod(EnableSession = true)]
	public static bool ExportPdfOutput(WorkflowRequest data)
	{
		return HandleErrorForAction(() =>
		{
		var searchParams = CheckAndGetParamExportAction(data);
		if (searchParams == null) return false;

		if (data.IsUnSysnc || Constants.GLOBAL_OPTION_ENABLE)
		{
			// ＰＤＦクリエータ起動
			Form_PdfOutput_PdfOutput.ExecPdfCreater(
				HttpContext.Current.Session.SessionID,
				Constants.KBN_PDF_OUTPUT_ORDER_WORKFLOW,
				Constants.KBN_PDF_OUTPUT_ORDER_INVOICE,
				searchParams);
			return true;
		}

		return true;
		});
	}

	/// <summary>
	/// Export total picking list output
	/// </summary>
	/// <param name="data">Receice data</param>
	/// <returns>True: if set session success, otherwise: false</returns>
	[WebMethod(EnableSession = true)]
	public static bool ExportTotalPickingListOutput(WorkflowRequest data)
	{
		return HandleErrorForAction(() =>
		{
		var searchParams = CheckAndGetParamExportAction(data);
		if (searchParams == null) return false;

		if (data.IsUnSysnc)
		{
			// ＰＤＦクリエータ起動
			Form_PdfOutput_PdfOutput.ExecPdfCreater(
				HttpContext.Current.Session.SessionID,
				Constants.KBN_PDF_OUTPUT_ORDER_WORKFLOW,
				Constants.KBN_PDF_OUTPUT_TOTAL_PICKING_LIST,
				searchParams);
			return true;
		}

		return true;
		});
	}

	/// <summary>
	/// Export pdf output receipt
	/// </summary>
	/// <param name="data">Receice data</param>
	/// <returns>True: if set session success, otherwise: false</returns>
	[WebMethod(EnableSession = true)]
	public static bool ExportPdfOutputReceipt(WorkflowRequest data)
	{
		return HandleErrorForAction(() =>
		{
		var searchParams = CheckAndGetParamExportAction(data);
		if (searchParams == null) return false;

		searchParams[Constants.FIELD_ORDER_RECEIPT_FLG] = Constants.FLG_ORDER_RECEIPT_FLG_ON;
		//領収書発行メール送信
		SendReceiptMails(searchParams);

		if (data.IsUnSysnc)
		{
			// ＰＤＦクリエータ起動
			Form_PdfOutput_PdfOutput.ExecPdfCreater(
				HttpContext.Current.Session.SessionID,
				Constants.KBN_PDF_OUTPUT_ORDER_WORKFLOW,
				Constants.KBN_PDF_OUTPUT_RECEIPT,
				searchParams);
			return true;
		}

		return true;
		});
	}

	/// <summary>
	/// 領収書発行メール送信
	/// </summary>
	/// <param name="param">検索パラメタ</param>
	public static void SendReceiptMails(Hashtable param)
	{
		var orders = new ReceiptCreater().GetOrders(param, Constants.KBN_PDF_OUTPUT_ORDER_WORKFLOW);
		var orderIdList = new List<string>();
		var runningTask = new List<Task>();
		if (Directory.Exists(ReceiptCreater.TempDirPath))
		{
			var dir = new DirectoryInfo(ReceiptCreater.TempDirPath);
			foreach (var file in dir.GetFiles())
			{
				if (file.LastWriteTime < DateTime.Now.AddMinutes(-10))
				{
					// 更新日時が10分たったものは削除
					file.Delete();
				}
			}
		}

		foreach (var drvOrder in orders)
		{
			var order = (DataRowView)drvOrder;
			if (orderIdList.Contains((string)order[Constants.REQUEST_KEY_ORDER_ID])) continue;
			orderIdList.Add((string)order[Constants.REQUEST_KEY_ORDER_ID]);

			var orderParam = new Hashtable
			{
				{ Constants.REQUEST_KEY_SHOP_ID, order[Constants.REQUEST_KEY_SHOP_ID] },
				{ Constants.HASH_KEY_ORDER_ID, order[Constants.REQUEST_KEY_ORDER_ID] },
				{ Constants.FIELD_ORDERITEM_DATE_CREATED, order[Constants.FIELD_ORDERITEM_DATE_CREATED] },
				{ Constants.FIELD_ORDER_ORDER_PRICE_TOTAL, order[Constants.FIELD_ORDER_ORDER_PRICE_TOTAL] },
			};
			runningTask.Add(Task.Run(() =>
			{
				var filePath = new ReceiptCreater().CreateMailFile(Constants.KBN_PDF_OUTPUT_ORDER, orderParam, (string)orderParam[Constants.HASH_KEY_ORDER_ID]);
				SendMailCommon.SendReceipFiletMail((string)orderParam[Constants.HASH_KEY_ORDER_ID], filePath);
				if (File.Exists(filePath))
				{
					File.Delete(filePath);
				}
			}));
		}

		foreach (var t in runningTask)
		{
			t.Wait();
		}
	}

	/// <summary>
	/// Export pdf output order statement
	/// </summary>
	/// <param name="data">Receice data</param>
	/// <returns>True: if set session success, otherwise: false</returns>
	[WebMethod(EnableSession = true)]
	public static bool ExportPdfOutputOrderStatement(WorkflowRequest data)
	{
		return HandleErrorForAction(() =>
		{
		var searchParams = CheckAndGetParamExportAction(data);
		if (searchParams == null) return false;

		if (data.IsUnSysnc)
		{
			// ＰＤＦクリエータ起動
			Form_PdfOutput_PdfOutput.ExecPdfCreater(
				HttpContext.Current.Session.SessionID,
				Constants.KBN_PDF_OUTPUT_ORDER_WORKFLOW,
				Constants.KBN_PDF_OUTPUT_ORDER_STATEMENT,
				searchParams);
			return true;
		}

		return true;
		});
	}

	/// <summary>
	/// Check shipping label
	/// </summary>
	/// <param name="data">Receice data</param>
	/// <returns>True: if set session success</returns>
	[WebMethod(EnableSession = true)]
	public static bool ExportShippingLabel(WorkflowRequest data)
	{
		return HandleErrorForAction(() =>
		{
		var searchParams = CheckAndGetParamExportAction(data);
		return (searchParams != null);
		});
	}

	/// <summary>
	/// Export interaction data
	/// </summary>
	/// <param name="data">Receice data</param>
	/// <returns>True: if set session success</returns>
	[WebMethod(EnableSession = true)]
	public static bool ExportInteractionData(WorkflowRequest data)
	{
		return HandleErrorForAction(() =>
		{
		var searchParams = CheckAndGetParamExportAction(data);
		return (searchParams != null);
		});
	}

	/// <summary>
	/// Export home delivery
	/// </summary>
	/// <param name="data">Receice data</param>
	/// <returns>True: if set session success, otherwise: false</returns>
	[WebMethod(EnableSession = true)]
	public static bool ExportHomeDelivery(WorkflowRequest data)
	{
		return HandleErrorForAction(() =>
		{
		var searchParams = CheckAndGetParamExportAction(data);
		return (searchParams != null);
		});
	}

	/// <summary>
	/// Get total count
	/// </summary>
	/// <param name="data">Receice data</param>
	/// <returns>a string of total count</returns>
	[WebMethod]
	public static string GetTotalCount(WorkflowRequest data)
	{
		return HandleErrorForAction(() =>
		{
		var workflowUtility = new WorkflowUtility(data);
		var param = workflowUtility.GetSearchParamsOrder();
		var hasOrderReceiptSearchParam = (Hashtable)param.Clone();

		// ピッキングリスト件数取得
		if (Constants.PDF_OUTPUT_PICKINGLIST_ENABLED
			&& (data.TotalName == Constants.KBN_PDF_OUTPUT_TOTAL_PICKING_LIST))
		{
			var pickingListItemCount = new TotalPickingListCreater().GetOrderItemCount(Constants.KBN_PDF_OUTPUT_ORDER_WORKFLOW, param);
			return pickingListItemCount.ToString();
		}

		// 領収書出力リンク設定
		if (Constants.RECEIPT_OPTION_ENABLED
			&& (data.TotalName == Constants.KBN_PDF_OUTPUT_RECEIPT))
		{
			var orderCount = OrderCommon.GetOrderListForWorkflowCount(param);
			if (orderCount.Count > 0)
			{
				var totalCount = (int)orderCount[0][Constants.FIELD_COMMON_ROW_COUNT];
				var totalHasReceiptOrderCounts = 0;
				// 検索条件と抽出条件にあわせて、領収書希望ありの注文件数を取得
				switch (StringUtility.ToEmpty(param[Constants.FIELD_ORDER_RECEIPT_FLG]))
				{
					case Constants.FLG_ORDER_RECEIPT_FLG_ON:
						totalHasReceiptOrderCounts = totalCount;
						break;

					case Constants.FLG_ORDER_RECEIPT_FLG_OFF:
						totalHasReceiptOrderCounts = 0;
						break;

					default:
						// 領収書希望ありの条件を設定
						hasOrderReceiptSearchParam[Constants.FIELD_ORDER_RECEIPT_FLG] = Constants.FLG_ORDER_RECEIPT_FLG_ON;
						var orders = OrderCommon.GetOrderListForWorkflowCount(hasOrderReceiptSearchParam);
						totalHasReceiptOrderCounts = (orders.Count > 0)
							? (int)orders[0][Constants.FIELD_COMMON_ROW_COUNT]
							: 0;
						break;
				}
				return totalHasReceiptOrderCounts.ToString();
			}
		}
		return string.Empty;
		});
	}

	/// <summary>
	/// Print invoice order for twECPay
	/// </summary>
	/// <param name="data">Receice data</param>
	/// <returns>a string of total count</returns>
	[WebMethod]
	public static string PrintInvoiceOrderForTwECPay(WorkflowRequest data)
	{
		return HandleErrorForAction(() =>
		{
		var workflowUtility = new WorkflowUtility(data);
		var deliveryTranIdLists =
			DomainFacade.Instance.OrderService.GetDeliveryTranIdListOrderWorkFlow(workflowUtility.GetSearchParamsOrder());

		// Create request data and get data from api
		var deliveryTranIdList = deliveryTranIdLists.Cast<DataRowView>()
			.Select(row => StringUtility.ToEmpty(row[Constants.FIELD_ORDER_DELIVERY_TRAN_ID]))
			.ToArray();
		var clientScript = ECPayUtility.CreateScriptForGetInvoiceOrder(deliveryTranIdList);
		return clientScript;
		});
	}

	/// <summary>
	/// Create error page url
	/// </summary>
	/// <returns>Error page url</returns>
	protected string CreateErrorPageUrl()
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR)
			.CreateUrl();
		return url;
	}

	/// <summary>
	/// Create order file export page url
	/// </summary>
	/// <param name="exportKbnKey">Export kbn key</param>
	/// <param name="exportKbnValue">Export kbn value</param>
	/// <returns>Order file export page url</returns>
	protected string CreateOrderFileExportPageUrl(string exportKbnKey, string exportKbnValue = "")
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDERFILEEXPORT_LIST)
			.AddParam(Constants.REQUEST_KEY_ORDERFILE_ORDERPAGE, Constants.PAGE_MANAGER_ORDERWORKFLOW_LIST)
			.AddParam(exportKbnKey, exportKbnValue)
			.CreateUrl();
		return url;
	}

	/// <summary>
	/// Create PDF export page url
	/// </summary>
	/// <param name="exportKbnValue">Export kbn value</param>
	/// <returns>PDF export page url</returns>
	protected string CreatePdfExportPageUrl(string exportKbnValue)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PDF_OUTPUT)
			.AddParam(Constants.REQUEST_KEY_PDF_OUTPUT, Constants.KBN_PDF_OUTPUT_ORDER_WORKFLOW)
			.AddParam(Constants.REQUEST_KEY_PDF_KBN, exportKbnValue)
			.CreateUrl();
		return url;
	}

	/// <summary>
	/// Create download wait export page url
	/// </summary>
	/// <returns>Download wait export page url</returns>
	protected string CreateDownloadWaitExportPageUrl()
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_DOWNLOAD_WAIT)
			.AddParam("targaturl", Constants.PATH_ROOT + Constants.PATH_CONTENTS + "Invoice/")
			.AddParam("sid", Session.SessionID)
			.CreateUrl();
		return url;
	}
	#endregion

	/// <summary>The order workflow base URL</summary>
	protected string OrderWorkflowBaseUrl
	{
		get { return (Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDERWORKFLOW_LIST); }
	}
	/// <summary>Order workflow getter URL</summary>
	protected string OrderWorkflowGetterUrl
	{
		get { return (Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDERWORKFLOW_GETTER); }
	}
}
