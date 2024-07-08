/*
=========================================================================================================
  Module      : Workflow Utility (WorkflowUtility.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using w2.App.Common.Global.Config;
using w2.App.Common.NextEngine;
using w2.App.Common.NextEngine.Helper;
using w2.App.Common.NextEngine.Response;
using w2.App.Common.Order.Payment.ECPay;
using w2.App.Common.Util;
using w2.Common.Extensions;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain;
using w2.Domain.FixedPurchase.Helper;
using w2.Domain.Order.Helper;
using w2.Domain.OrderWorkflowExecHistory;
using w2.Domain.ShopOperator;
using w2.Domain.UpdateHistory.Helper;

namespace w2.App.Common.Order.Workflow
{
	/// <summary>
	/// WorkFlowHistoryHelper
	/// </summary>
	public static class WorkFlowHistoryHelper
	{
		/// <summary>
		/// 受注ワークフロー実行履歴
		/// </summary>
		/// <param name="inputId"></param>
		/// <param name="workflowName"></param>
		/// <param name="workflowType"></param>
		/// <param name="workflowKbn"></param>
		/// <param name="workflowNo"></param>
		/// <param name="operatorName"></param>
		/// <returns></returns>
		public static OrderWorkflowExecHistoryModel InsertFunction(string inputId, string workflowName, string workflowType, string workflowKbn, int workflowNo, string operatorName)
		{
			var history = DomainFacade.Instance.OrderWorkflowExecHistoryService.Insert(
				new OrderWorkflowExecHistoryModel
				{
					ShopId = inputId,
					WorkflowKbn = workflowKbn,
					WorkflowNo = workflowNo,
					WorkflowName = workflowName,
					ExecStatus = Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_RUNNING,
					WorkflowType = workflowType,
					ExecPlace = Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_PLACE_WORKFLOW,
					ExecTiming = Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_TIMING_MANUAL,
					Message = Constants.FLG_ORDERWORKFLOWEXECHISTORY_MESSAGE_FOR_WORKFLOW_RUNNING,
					DateBegin = DateTime.Now,
					LastChanged = operatorName,
				});
			return history;
		}
		/// <summary>
		/// 履歴更新処理
		/// </summary>
		/// <param name="execStatus"></param>
		/// <param name="successRate"></param>
		/// <param name="message"></param>
		/// <param name="historyId"></param>
		public static void UpdateHistoryFunction(string execStatus, string successRate, string message, long historyId)
		{
			DomainFacade.Instance.OrderWorkflowExecHistoryService.Modify(
				historyId,
				action =>
				{
					action.ExecStatus = execStatus;
					action.SuccessRate = successRate;
					action.DateEnd = DateTime.Now;
					action.Message = message;
				});
		}
	}
	/// <summary>
	/// Workflow utility
	/// </summary>
	public class WorkflowUtility
	{
		#region Constants
		/// <summary>Order list validate name</summary>
		private const string CONST_VALIDATE_ORDER_LIST_NAME = "OrderList";
		/// <summary>Update date</summary>
		private const string CONST_FIELD_UPDATE_DATE = "update_date";
		#endregion

		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		public WorkflowUtility()
		{
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="request">The request</param>
		public WorkflowUtility(WorkflowRequest request)
		{
			WorkflowSetting.WorkflowTypes workflowTypes;

			this.Request = request;

			var dvOrderWorkflowSetting = new WorkflowSetting().GetOrderWorkflowSetting(
				this.LoginOperator.ShopId,
				this.Request.WorkflowKbn,
				this.Request.WorkflowNo,
				this.Request.WorkflowType);

			if (this.Request.WorkflowType == WorkflowSetting.m_KBN_WORKFLOW_TYPE_ORDER)
			{
				workflowTypes = WorkflowSetting.WorkflowTypes.Order;
				this.OrderListCondition = new OrderListConditionForWorkflow(request.SearchCondition);
				if (string.IsNullOrEmpty(this.OrderListCondition.ShippingPrefectures) == false)
				{
					this.OrderListCondition.ShippingPrefectures = PrefectureUtility.GetHashString(
						this.OrderListCondition.ShippingPrefectures.Split(','));
				}
			}
			else
			{
				workflowTypes = WorkflowSetting.WorkflowTypes.FixedPurchase;
				this.FixedPurchaseListCondition = new FixedPurchaseConditionForWorkflow(request.SearchCondition);
			}

			this.OrderWorkflow = new WorkflowSetting(
				dvOrderWorkflowSetting[0],
				this.LoginOperator.ShopId,
				this.LoginOperator.Name,
				workflowTypes);
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="request">The request</param>
		public WorkflowUtility(WorkflowValidateRequest request)
		{
			WorkflowSetting.WorkflowTypes workflowTypes;

			this.ValidateRequest = request;

			var dvOrderWorkflowSetting = new WorkflowSetting().GetOrderWorkflowSetting(
				this.LoginOperator.ShopId,
				this.ValidateRequest.WorkflowKbn,
				this.ValidateRequest.WorkflowNo,
				this.ValidateRequest.WorkflowType);

			if (this.ValidateRequest.WorkflowType == WorkflowSetting.m_KBN_WORKFLOW_TYPE_ORDER)
			{
				workflowTypes = WorkflowSetting.WorkflowTypes.Order;
			}
			else
			{
				workflowTypes = WorkflowSetting.WorkflowTypes.FixedPurchase;
			}

			this.OrderWorkflow = new WorkflowSetting(
				dvOrderWorkflowSetting[0],
				this.LoginOperator.ShopId,
				this.LoginOperator.Name,
				workflowTypes);
		}
		#endregion

		#region Methods
		/// <summary>
		/// Validate
		/// </summary>
		/// <returns>Error message</returns>
		public string Validate()
		{
			var errorMessage = string.Empty;

			// ステータス更新日指定入力チェック
			if (this.OrderWorkflow.UpdateStatusValid)
			{
				var date = ConvertToDateFormatString(this.ValidateRequest.ExtendStatusDate);
				var extendStatusDate = new Hashtable
				{
					{ CONST_FIELD_UPDATE_DATE, date }
				};

				// エラーチェック
				errorMessage = w2.App.Common.Util.Validator.Validate(CONST_VALIDATE_ORDER_LIST_NAME, extendStatusDate);
				if (string.IsNullOrEmpty(errorMessage) == false) return errorMessage;
			}

			// Check schedule shipping date input
			if (this.OrderWorkflow.NeedsUpdateScheduledShippingDate)
			{
				var date = string.IsNullOrEmpty(this.ValidateRequest.ScheduledShippingDateUpdate)
					? string.Empty
					: ConvertToDateFormatString(this.ValidateRequest.ScheduledShippingDateUpdate);
				var scheduleShippingDate = new Hashtable
				{
					{ Constants.FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE, date }
				};

				// エラーチェック
				errorMessage = w2.App.Common.Util.Validator.Validate(CONST_VALIDATE_ORDER_LIST_NAME, scheduleShippingDate);
				if (string.IsNullOrEmpty(errorMessage) == false) return errorMessage;
			}

			// 配送希望日入力チェック
			if (this.OrderWorkflow.NeedsUpdateShippingDate)
			{
				var date = string.IsNullOrEmpty(this.ValidateRequest.ShippingDateUpdate)
					? string.Empty
					: ConvertToDateFormatString(this.ValidateRequest.ShippingDateUpdate);
				var shippingDate = new Hashtable
				{
					{ Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE, date }
				};

				// エラーチェック
				errorMessage = w2.App.Common.Util.Validator.Validate(CONST_VALIDATE_ORDER_LIST_NAME, shippingDate);
				if (string.IsNullOrEmpty(errorMessage) == false) return errorMessage;
			}

			// Check next shipping date input
			if (this.OrderWorkflow.NeedsUpdateNextShippingDate)
			{
				var date = ConvertToDateFormatString(this.ValidateRequest.NextShippingDateUpdate);
				var nextShippingDate = new Hashtable
				{
					{ Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_DATE, date }
				};

				errorMessage = w2.App.Common.Util.Validator.Validate(CONST_VALIDATE_ORDER_LIST_NAME, nextShippingDate);
				if (string.IsNullOrEmpty(errorMessage) == false) return errorMessage;
			}

			// Check next next shipping date input
			if (this.OrderWorkflow.NeedsUpdateNextNextShippingDate)
			{
				var date = ConvertToDateFormatString(this.ValidateRequest.NextNextShippingDateUpdate);
				var nextNextShippingDate = new Hashtable
				{
					{ Constants.FIELD_FIXEDPURCHASE_NEXT_NEXT_SHIPPING_DATE, date }
				};

				errorMessage = w2.App.Common.Util.Validator.Validate(CONST_VALIDATE_ORDER_LIST_NAME, nextNextShippingDate);
				if (string.IsNullOrEmpty(errorMessage) == false) return errorMessage;

				if (this.OrderWorkflow.NeedsUpdateNextShippingDate
					&& (DateTime.Parse(this.ValidateRequest.NextShippingDateUpdate) >= DateTime.Parse(this.ValidateRequest.NextNextShippingDateUpdate)))
				{
					errorMessage = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_MANAGER_FIXED_PURCHASE_SHIPPING_DATE_RANGE_ERROR);
					return errorMessage;
				}
			}

			// 実行中または保留中のワークフローがある場合にエラーページに遷移
			var hasExecStatusForRunningOrHold = DomainFacade.Instance.OrderWorkflowExecHistoryService.HasExecStatusForRunningOrHold();
			if (hasExecStatusForRunningOrHold)
			{
				errorMessage = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_MANAGER_ORDERWORKFLOW_OTHER_RUNNING_OR_HOLD);
				return errorMessage;
			}

			return errorMessage;
		}

		/// <summary>
		/// Convert to date format string
		/// </summary>
		/// <param name="input">Date input</param>
		/// <returns>Date format string</returns>
		private string ConvertToDateFormatString(string input)
		{
			if (input.Count(c => (c == '/')) == 0)
			{
				return input += "//";
			}

			if (input.Count(c => (c == '/')) == 1)
			{
				return input += "/";
			}

			return input;
		}

		/// <summary>
		/// Execute
		/// </summary>
		/// <param name="processId">Process id</param>
		/// <returns>Exec workflow</returns>
		public ProcessInfoType Exec(string processId)
		{
			if (this.Request.ExecType == WorkflowSetting.m_KBN_EXEC_TYPE_ALL)
			{
				this.Request.Orders = GetAllOrderList();
			}

			if (this.OrderWorkflow.UpdateStatusValid == false)
			{
				this.Request.ExtendStatusDate = null;
			}

			if (this.OrderWorkflow.NeedsUpdateScheduledShippingDate == false)
			{
				this.Request.ScheduledShippingDateUpdate = null;
			}

			if (this.OrderWorkflow.NeedsUpdateShippingDate == false)
			{
				this.Request.ShippingDateUpdate = null;
			}

			if (this.OrderWorkflow.NeedsUpdateNextShippingDate == false)
			{
				this.Request.NextShippingDateUpdate = null;
			}

			if (this.OrderWorkflow.NeedsUpdateNextNextShippingDate == false)
			{
				this.Request.NextNextShippingDateUpdate = null;
			}

			if (this.OrderWorkflow.NeedsUpdateFixedPurchaseStopUnavailableShippingArea == false)
			{
				this.Request.FixedPurchaseStopUnavailableShippingAreaUpdate = null;
			}

			if (this.Request.WorkflowType == WorkflowSetting.m_KBN_WORKFLOW_TYPE_ORDER)
			{
				ExecOrderWorkflow(processId);
			}
			else
			{
				ExecFixedPurchaseWorkflow(processId);
			}

			return WorkflowProcessObserver.Instance.GetWorkflowProcess(processId);
		}

		/// <summary>
		/// Exec order workflow
		/// </summary>
		/// <param name="processId">Process id</param>
		/// <returns>List action result unit</returns>
		protected List<ActionResultUnit> ExecOrderWorkflow(string processId)
		{
			var orderWorkflowSettingModel = DomainFacade.Instance.OrderWorkflowSettingService.Get(
				this.LoginOperator.ShopId,
				this.Request.WorkflowKbn,
				int.Parse(this.Request.WorkflowNo));
			var workflowName = orderWorkflowSettingModel.WorkflowName;

			this.RunningWorkflowHistory = InsertOrderWorkflowExecHistory(
				workflowName,
				Constants.FLG_ORDERWORKFLOWEXECHISTORY_WORKFLOW_TYPE_ORDER_WORKFLOW);

			var workflowExecInfos = new List<WorkflowExecInfo>();
			foreach (var order in this.Request.Orders)
			{
				var workflowExecInfo = new WorkflowExecInfo
				{
					MasterId = order.OrderId
				};

				var execExternalPaymentAction = false;
				var externalPaymentActionValue = string.Empty;

				// カセット表示
				if (this.OrderWorkflow.IsDisplayKbnCassette)
				{
					// 画面上で選択している状態を取得し、外部決携かを判断
					workflowExecInfo.ItemIndex = order.Index;
					workflowExecInfo.DoExec = true;
					workflowExecInfo.UpdateCassetteStatus = order.CassetteAction;

					if (workflowExecInfo.DoExec)
					{
						var cassetteStatuses = order.CassetteAction.Split('&');
						var cassetteStatusFieldName = cassetteStatuses[0];
						externalPaymentActionValue = cassetteStatuses[1];
						execExternalPaymentAction =
							(cassetteStatusFieldName == Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_EXTERNAL_PAYMENT_ACTION);
					}
				}
				// 1行表示
				else if (this.OrderWorkflow.IsDisplayKbnLine)
				{
					// チェックボックスの状態により更新リストの初期値をセット
					workflowExecInfo.DoExec = true;

					// ワークフロー設定から決済連携の設定値を取得
					if (workflowExecInfo.DoExec)
					{
						execExternalPaymentAction = this.OrderWorkflow.NeedsExecExternalPaymentAction;
						externalPaymentActionValue = this.OrderWorkflow.ExternalPaymentActionValue;
					}
				}

				// 外部決済連携なら決済取引IDに仮取引IDを格納
				if (execExternalPaymentAction)
				{
					var dvOrder = OrderCommon.GetOrder(order.OrderId);
					if (dvOrder.Count == 0) continue;

					var orderId = StringUtility.ToEmpty(dvOrder[0][Constants.FIELD_ORDER_ORDER_ID]);
					var cardTrandId = StringUtility.ToEmpty(dvOrder[0][Constants.FIELD_ORDER_CARD_TRAN_ID]);
					var cardTranIdUpdated = this.OrderWorkflow.UpdateCardTranIdBeforeRealSales(
						orderId,
						cardTrandId,
						HasDigitalContents(dvOrder),
						externalPaymentActionValue,
						UpdateHistoryAction.DoNotInsert);
					workflowExecInfo.DoExec = cardTranIdUpdated;
				}
				workflowExecInfos.Add(workflowExecInfo);
			}

			// 実行処理トランザクション
			var process = new ProcessInfoType
			{
				TotalCount = workflowExecInfos.Count,
				DoneCount = 0,
				IsAsync = (this.OrderWorkflow.DisplayExecExternalPaymentActionResult
					|| this.OrderWorkflow.DisplayExecExternalOrderInfoActionResult
					|| (workflowExecInfos.Count > 100)),
				ProcessId = processId
			};

			// Add new workflow process
			WorkflowProcessObserver.Instance.AddWorkflowProcess(processId, process);

			// 100件を超える場合または外部決済連携を行う場合は非同期で実行（リダイレクトはタイマ内で行う）
			var actionResultUnits = new List<ActionResultUnit>();
			if (process.IsAsync)
			{
				var context = HttpContext.Current;
				Task.Run(() =>
				{
					CallContext.HostContext = context;
					var begin = DateTime.Now;
					try
					{
						actionResultUnits = ExecWorkflowInner(workflowExecInfos, this.OrderWorkflow, processId);
						FileLogger.WriteInfo(
							string.Format(
								"OrderWorkflow実行({0}件：{1})",
								StringUtility.ToNumeric(WorkflowProcessObserver.Instance.GetWorkflowProcess(processId).TotalCount),
								(DateTime.Now - begin).ToString().PadRight(16, ' ')));
					}
					catch (Exception ex)
					{
						WorkflowProcessObserver.Instance.GetWorkflowProcess(processId).IsSystemError = true;
						FileLogger.WriteError(ex);
					}
				});
				Thread.Sleep(2000);
			}
			// 100件以内は同期で実行
			else
			{
				actionResultUnits = ExecWorkflowInner(workflowExecInfos, this.OrderWorkflow, processId);
			}

			return actionResultUnits;
		}

		/// <summary>
		/// Get search settings
		/// </summary>
		/// <param name="searchSetting">Search setting</param>
		/// <param name="key">Key</param>
		/// <returns>Values by key</returns>
		private string GetSearchSetting(string searchSetting, string key)
		{
			if (string.IsNullOrEmpty(searchSetting)) return string.Empty;

			var settingKeyValues = searchSetting.Split('&');
			var findSettingKeyValue = settingKeyValues.FirstOrDefault(settingKeyValue =>
				Regex.IsMatch(settingKeyValue, string.Format(@"^{0}=", key)));

			if (string.IsNullOrEmpty(findSettingKeyValue)) return string.Empty;

			var keyAndValue = findSettingKeyValue.Split('=');

			if (keyAndValue.Length < 2) return string.Empty;

			return keyAndValue[1];
		}

		/// <summary>
		/// ワークフロー実行処理（非同期でも実行される可能性がある。非同期の場合、セッションの書き込みは無効）
		/// </summary>
		/// <param name="workflowExecInfos">更新用リスト</param>
		/// <param name="orderWorkflowSetting">ワークフロー設定</param>
		/// <param name="processId">Process id</param>
		/// <returns>実行処理結果</returns>
		private List<ActionResultUnit> ExecWorkflowInner(
			List<WorkflowExecInfo> workflowExecInfos,
			WorkflowSetting orderWorkflowSetting,
			string processId)
		{
			var actionResultUnits = new List<ActionResultUnit>();
			var context = (HttpContext)CallContext.HostContext;
			var operatorName = this.LoginOperator.Name;

			// 2重押し防止
			lock (context.Session)
			{
				using (var accessor = new SqlAccessor())
				{
					// トランザクション開始
					accessor.OpenConnection();
					accessor.BeginTransaction();

					var isAccessTokenValid = true;
					var isSuccessNextEngineUpload = true;
					var isSuccessNextEngineImport = true;

					var accessToken = string.Empty;
					var refreshToken = string.Empty;

					this.UpdateNextEngineOrders = new NEOrder[0];

					if (orderWorkflowSetting.ExternalOrderInfoActionValue
						== Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION_NEXTENGINE_IMPORT)
					{
						isSuccessNextEngineImport = isAccessTokenValid = NextEngineApi.IsExistsToken(out accessToken, out refreshToken);

						if (isAccessTokenValid)
						{
							var neOrderList = new List<NEOrder>();
							for (var i = 0; i <= (workflowExecInfos.Count(w => w.DoExec) / 1000); i++)
							{
								var orderIds = workflowExecInfos
									.Where(w => w.DoExec)
									.Select(w => w.MasterId)
									.Skip(i * 1000)
									.Take(1000)
									.ToArray();

								if (Constants.GIFTORDER_OPTION_ENABLED && Constants.NE_OPTION_ENABLED)
								{
									orderIds = NextEngineOrderModel.CreateNeGiftOrderId(orderIds, accessor);
								}

								var searchOrderRsponse = NextEngineApi.CallSearchOrderApi(
									accessToken,
									refreshToken,
									NextEngineConstants.FLG_WAIT_FLG_IMMEDIATELY,
									NextEngineConstants.FIELDS_SEARCH_ORDER_RESPONSE,
									orderIds);

								switch (searchOrderRsponse.Result)
								{
									case NextEngineConstants.FLG_RESULT_SUCCESS:
										neOrderList.AddRange(searchOrderRsponse.Data);
										break;

									case NextEngineConstants.FLG_RESULT_ERROR:
										isSuccessNextEngineImport = false;
										continue;

									case NextEngineConstants.FLG_RESULT_REDIRECT:
										isAccessTokenValid = isSuccessNextEngineImport = false;
										break;
								}
							}

							this.UpdateNextEngineOrders = neOrderList.ToArray();
						}
					}

					var deliveryCompanies = DomainFacade.Instance.DeliveryCompanyService.GetAll();

					foreach (var workflowExecInfo in workflowExecInfos.Where(info => info.DoExec))
					{
						var actionResultUnit = new WorkflowExec().ExecAction(
							orderWorkflowSetting,
							workflowExecInfo,
							operatorName,
							this.OrderWorkflow.IsDisplayKbnLine,
							this.Request.ExtendStatusDate.ToString(),
							this.Request.ScheduledShippingDateUpdate.ToString(),
							this.Request.NextShippingDateUpdate.ToString(),
							this.Request.NextNextShippingDateUpdate.ToString(),
							this.Request.ShippingDateUpdate.ToString(),
							this.UpdateNextEngineOrders,
							deliveryCompanies,
							accessor);

						if (actionResultUnit != null) actionResultUnits.Add(actionResultUnit);

						this.HasSelected = actionResultUnits.Any();
						WorkflowProcessObserver.Instance.GetWorkflowProcess(processId).DoneCount++;
					}

					this.ListOrderId = new List<string>();
					if (Constants.STORE_PICKUP_OPTION_ENABLED)
					{
						foreach (var workflowExecInfo in workflowExecInfos.Where(info => info.DoExec))
						{
							this.ListOrderId.Add(workflowExecInfo.MasterId);
						}
					}

					var workFlowSettings = orderWorkflowSetting.Setting.ToHashtable();
					var realShopId = GetSearchSetting(
						workFlowSettings[Constants.FIELD_ORDERWORKFLOWSETTING_SEARCH_SETTING].ToString(),
						Constants.FIELD_REALSHOP_REAL_SHOP_ID);
					if ((this.ListOrderId.Count != 0)
						&& ((orderWorkflowSetting.MailIdValue == Constants.CONST_MAIL_ID_TO_REAL_SHOP)
							|| (string.IsNullOrEmpty(realShopId) == false)))
					{
						// Sent all orders to real shop mail
						SendMailAllOrderToRealShop();
					}

					if (orderWorkflowSetting.ExternalOrderInfoActionValue
						== Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION_NEXTENGINE)
					{
						isSuccessNextEngineUpload = isAccessTokenValid = NextEngineApi.IsExistsToken(out accessToken, out refreshToken);

						if (isAccessTokenValid)
						{
							var uploadPatternesponse = NextEngineApi.CallGetUploadPatternApi(
								accessToken,
								refreshToken,
								NextEngineConstants.FLG_WAIT_FLG_IMMEDIATELY);

							switch (uploadPatternesponse.Result)
							{
								case NextEngineConstants.FLG_RESULT_SUCCESS:
									var patterns = uploadPatternesponse.Data;
									var patternId = patterns
										.First(d =>
											((d.FormatId == NextEngineConstants.FLG_UPLOAD_PATTERN_FORMAT_ID_GENERAL)
												&& (d.ShopId == Constants.NE_SHOP_ID)))
										.PatternId;

									// CallGetUploadPatternApi実行によりアクセストークンが更新されている可能性があるため再取得
									NextEngineApi.IsExistsToken(out accessToken, out refreshToken);

									var uploadOrderResponse = NextEngineApi.CallUploadOrderApi(
										accessToken,
										refreshToken,
										patternId,
										NextEngineOrderModel.GetCsvForUpload(),
										NextEngineConstants.FLG_WAIT_FLG_AVOID_FAILURE);
									isSuccessNextEngineUpload = (uploadOrderResponse.Result == NextEngineConstants.FLG_RESULT_SUCCESS);
									break;

								case NextEngineConstants.FLG_RESULT_ERROR:
									isSuccessNextEngineUpload = false;
									break;

								case NextEngineConstants.FLG_RESULT_REDIRECT:
									isAccessTokenValid = isSuccessNextEngineUpload = false;
									break;
							}
						}
						else
						{
							// アクセストークンが無効な場合はアップロード用ファイルを削除
							NextEngineOrderModel.DeleteCsv();
						}
					}

					if ((isSuccessNextEngineUpload == false)
						|| (isSuccessNextEngineImport == false))
					{
						foreach (var actionResultUnit in actionResultUnits)
						{
							actionResultUnit.ResultExternalOrderInfoAction = OrderCommon.ResultKbn.UpdateNG;
						}
					}

					var orderStatusChange = StringUtility.ToEmpty(
						orderWorkflowSetting.Setting[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_STATUS_CHANGE]);
					if (Constants.NE_OPTION_ENABLED
						&& Constants.NE_COOPERATION_CANCEL_ENABLED
						&& (orderStatusChange == Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED))
					{
						var orderIdsForNextEngineBulkCancel = actionResultUnits
							.Where(actionResultUnit =>
								actionResultUnit.ResultOrderStatusChange == OrderCommon.ResultKbn.UpdateOK)
							.Select(actionResultUnit =>
								actionResultUnit.OrderId)
							.ToArray();

						if ((orderIdsForNextEngineBulkCancel.Length > 0)
							&& NextEngineApi.IsExistsToken(out accessToken, out refreshToken))
						{
							var orderIdsForNextEngineBulkCancelChunks = orderIdsForNextEngineBulkCancel.Chunk(1000);
							foreach (var orderIdsForNextEngineBulkCancelChunk in orderIdsForNextEngineBulkCancelChunks)
							{
								var failureCancelOrderIds = OrderCommon.UpdateNextEngineOrderForBulkCancel(
									orderIdsForNextEngineBulkCancelChunk);

								if (failureCancelOrderIds.Any())
								{
									foreach (var orderApiUpdateFail in failureCancelOrderIds)
									{
										var orderFail = actionResultUnits
											.Where(actionResultUnit => actionResultUnit.OrderId == orderApiUpdateFail)
											.FirstOrDefault();
										if (orderFail == null) continue;

										orderFail.ResultOrderStatusChange = OrderCommon.ResultKbn.UpdateNG;
									}
								}
							}
						}
					}

					var execSuccessCount = actionResultUnits.Count(r => (r.HasError == false));
					var isExecSuccess = ((execSuccessCount == actionResultUnits.Count)
						&& isSuccessNextEngineUpload
						&& isSuccessNextEngineImport);

					var historyMessage = isAccessTokenValid
						? (isExecSuccess
							? Constants.FLG_ORDERWORKFLOWEXECHISTORY_MESSAGE_FOR_WORKFLOW_SUCCESS
							: Constants.FLG_ORDERWORKFLOWEXECHISTORY_MESSAGE_FOR_WORKFLOW_FAILURE)
						: Constants.FLG_ORDERWORKFLOWEXECHISTORY_MESSAGE_FOR_NEXTENGINE_INVALID_ACCESS_TOKEN;

					WorkFlowHistoryHelper.UpdateHistoryFunction(
						isExecSuccess
							? Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_OK
							: Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_NG,
						string.Format(
							"{0}/{1}",
							execSuccessCount,
							actionResultUnits.Count),
						historyMessage,
					this.RunningWorkflowHistory.OrderWorkflowExecHistoryId);
				}
			}

			SetUpdatedInfo(actionResultUnits);
			var process = WorkflowProcessObserver.Instance.GetWorkflowProcess(processId);
			process.Results = this.Results;

			// Update remain order count by workflow setting and set this value using for display
			process.RemainCount = UpdateOrderCountForWorkflowSetting(
				this.OrderWorkflow.GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_SHOP_ID),
				this.Request.WorkflowType,
				this.Request.WorkflowKbn,
				this.Request.WorkflowNo);
			process.WorkflowType = this.Request.WorkflowType;
			process.WorkflowKbn = this.Request.WorkflowKbn;
			process.WorkflowNo = this.Request.WorkflowNo;
			process.IsSuccess = true;
			WorkflowProcessObserver.Instance.UpdateWorkflowProcess(processId, process);
			return actionResultUnits;
		}

		/// <summary>
		/// 更新した注文情報をWorkflowActionResult.ActionResultsに格納
		/// </summary>
		/// <param name="actionResultUnits">結果一覧</param>
		private void SetUpdatedInfo(List<ActionResultUnit> actionResultUnits)
		{
			this.Results = new ActionResults(
				this.OrderWorkflow.GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_NAME),
				actionResultUnits,
				this.OrderWorkflow.DisplayUpdateOrderStatusResult,
				this.OrderWorkflow.DisplayUpdateProductRealStockResult,
				this.OrderWorkflow.DisplayUpdatePaymentStatusResult,
				this.OrderWorkflow.DisplayExecExternalPaymentActionResult,
				this.OrderWorkflow.DisplayUpdateDemandStatusResult,
				this.OrderWorkflow.DisplayUpdateReturnExchangeStatusResult,
				this.OrderWorkflow.DisplayUpdateRepaymentStatusResult,
				this.OrderWorkflow.DisplayUpdateOrderExtendStatusStatementResult,
				this.OrderWorkflow.DisplayMailSendResult,
				this.OrderWorkflow.DisplayUpdateScheduledShippingDateStatusResult,
				this.OrderWorkflow.DisplayUpdateShippingDateStatusResult,
				this.OrderWorkflow.DisplayUpdateFixedPurchaseIsAliveResult,
				this.OrderWorkflow.DisplayUpdateFixedPurchasePaymentStatusResult,
				this.OrderWorkflow.DisplayUpdateNextShippingDateResult,
				this.OrderWorkflow.DisplayUpdateNextNextShippingDateResult,
				this.OrderWorkflow.DisplayUpdateFixedPurchaseStopUnavailableShippingAreaChangeResult,
				this.OrderWorkflow.DisplayUpdateFixedPurchaseExtendStatusStatementResult,
				this.OrderWorkflow.DisplayUpdateOrderReturnResult,
				this.OrderWorkflow.DisplayUpdateOrderInvoiceStatusResult,
				this.OrderWorkflow.DisplayUpdateOrderInvoiceApiResult,
				this.OrderWorkflow.DisplayExecExternalOrderInfoActionResult,
				this.OrderWorkflow.DisplayUpdateStorePicupStatusResult);

			// For case has error when execute external order information
			if (this.Results.DisplayExecExternalOrderInfoActionResult
				&& this.Results.Results.Any(item
					=> (string.IsNullOrEmpty(item.ExternalOrderInfoErrorMessage) == false)))
			{
				var message = new StringBuilder();
				foreach (var item in this.Results.Results.Where(item
					=> string.IsNullOrEmpty(item.ExternalOrderInfoErrorMessage) == false))
				{
					message.AppendLine(item.ExternalOrderInfoErrorMessage);
				}
				ECPayUtility.SendMailError(message.ToString().Trim());
			}
		}

		/// <summary>
		/// Execute fixed purchase workflow
		/// </summary>
		/// <param name="processId">Process id</param>
		/// <returns>List action result unit</returns>
		protected List<ActionResultUnit> ExecFixedPurchaseWorkflow(string processId)
		{
			var fixedPurchaseWorkflowSettingModel =
				DomainFacade.Instance.FixedPurchaseWorkflowSettingService.Get(
					this.LoginOperator.ShopId,
					this.Request.WorkflowKbn,
					int.Parse(this.Request.WorkflowNo));
			var workflowName = fixedPurchaseWorkflowSettingModel.WorkflowName;

			this.RunningWorkflowHistory = InsertOrderWorkflowExecHistory(
				workflowName,
				Constants.FLG_ORDERWORKFLOWEXECHISTORY_WORKFLOW_TYPE_FIXED_PURCHASE_WORKFLOW);

			// 更新リスト作成
			var updateList = new Dictionary<int, bool>();
			foreach (var order in this.Request.Orders)
			{
				// カセット表示
				if (this.OrderWorkflow.IsDisplayKbnCassette)
				{
					// 画面上で選択している状態を取得し、外部決携かを判断
					var selectedCassetteAction = order.CassetteAction;
					updateList.Add(
						order.Index,
						(string.IsNullOrEmpty(selectedCassetteAction) == false));
				}
				// 1行表示
				else if (this.OrderWorkflow.IsDisplayKbnLine)
				{
					updateList.Add(order.Index, true);
				}
			}

			// 実行処理トランザクション
			var process = new ProcessInfoType
			{
				TotalCount = updateList.Count,
				DoneCount = 0,
				IsAsync = (updateList.Count > 100),
				ProcessId = processId
			};

			// Add new workflow process
			WorkflowProcessObserver.Instance.AddWorkflowProcess(processId, process);

			// 100件を超える場合は非同期で実行（リダイレクトはタイマ内で行う）
			var actionResultUnits = new List<ActionResultUnit>();
			if (process.IsAsync)
			{
				var context = HttpContext.Current;
				Task.Run(() =>
				{
					CallContext.HostContext = context;
					var begin = DateTime.Now;
					try
					{
						actionResultUnits = ExecFixedPurchaseWorkflow(updateList, processId);
						FileLogger.WriteInfo(
							string.Format(
								"OrderWorkflow実行({0}件：{1})",
								StringUtility.ToNumeric(WorkflowProcessObserver.Instance.GetWorkflowProcess(processId).TotalCount),
								(DateTime.Now - begin).ToString().PadRight(16, ' ')));
					}
					catch (Exception ex)
					{
						WorkflowProcessObserver.Instance.GetWorkflowProcess(processId).IsSystemError = true;
						FileLogger.WriteError(ex);
					}
				});
				Thread.Sleep(2000);
			}
			// 100件以内は同期で実行
			else
			{
				actionResultUnits = ExecFixedPurchaseWorkflow(updateList, processId);
			}
			return actionResultUnits;
		}

		/// <summary>
		/// 定期ワークフロー実行処理（非同期でも実行される可能性がある。非同期の場合、セッションの書き込みは無効）
		/// </summary>
		/// <param name="updateList">更新用リスト</param>
		/// <param name="processId">Process id</param>
		/// <returns>実行処理結果</returns>
		private List<ActionResultUnit> ExecFixedPurchaseWorkflow(Dictionary<int, bool> updateList, string processId)
		{
			var actionResultUnits = new List<ActionResultUnit>();
			var context = (HttpContext)CallContext.HostContext;
			var loginOperatorName = this.LoginOperator.Name;
			lock (context.Session)
			{
				using (var accessor = new SqlAccessor())
				{
					// トランザクション開始
					accessor.OpenConnection();
					accessor.BeginTransaction();

					// 注文情報をループさせアクション
					foreach (var item in this.Request.Orders)
					{
						// 金額取得のためDBから情報取得
						var fixedPurchase = DomainFacade.Instance.FixedPurchaseService.Get(item.OrderId);
						if (fixedPurchase == null) continue;

						var order = new Hashtable
						{
							{ Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID, fixedPurchase.FixedPurchaseId },
							{ Constants.FIELD_ORDER_LAST_CHANGED, loginOperatorName },
							{ Constants.FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE, this.Request.ScheduledShippingDateUpdate },
							{ Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_DATE, this.Request.NextShippingDateUpdate },
							{ Constants.FIELD_FIXEDPURCHASE_NEXT_NEXT_SHIPPING_DATE, this.Request.NextNextShippingDateUpdate }
						};

						// アクション
						if (this.OrderWorkflow.IsDisplayKbnLine)
						{
							var result = this.OrderWorkflow.ActionForLineFixedPurchase(
								order,
								UpdateHistoryAction.Insert,
								accessor);
							actionResultUnits.Add(result);
						}
						else
						{
							actionResultUnits.Add(
								this.OrderWorkflow.ActionForCassetteFixedPurchase(
									order,
									item.CassetteAction,
									UpdateHistoryAction.Insert,
									accessor));
						}
						this.HasSelected = true;
						WorkflowProcessObserver.Instance.GetWorkflowProcess(processId).DoneCount++;
					}
				}

				var execSuccessCount = actionResultUnits.Count(r => (r.HasError == false));
				var isExecSuccess = (execSuccessCount == actionResultUnits.Count);
				WorkFlowHistoryHelper.UpdateHistoryFunction(
					isExecSuccess
						? Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_OK
						: Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_NG,
					string.Format(
						"{0}/{1}",
						execSuccessCount,
						actionResultUnits.Count),
					isExecSuccess
						? Constants.FLG_ORDERWORKFLOWEXECHISTORY_MESSAGE_FOR_FIXEDPURCHASE_SUCCESS
						: Constants.FLG_ORDERWORKFLOWEXECHISTORY_MESSAGE_FOR_FIXEDPURCHASE_FAILURE,
					this.RunningWorkflowHistory.OrderWorkflowExecHistoryId);
			}

			SetUpdatedInfo(actionResultUnits);
			var process = WorkflowProcessObserver.Instance.GetWorkflowProcess(processId);
			process.Results = this.Results;

			// Update remain order count by workflow setting and set this value using for display
			process.RemainCount = UpdateOrderCountForWorkflowSetting(
				this.OrderWorkflow.GetValue(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_SHOP_ID),
				this.Request.WorkflowType,
				this.Request.WorkflowKbn,
				this.Request.WorkflowNo);
			process.WorkflowType = this.Request.WorkflowType;
			process.WorkflowKbn = this.Request.WorkflowKbn;
			process.WorkflowNo = this.Request.WorkflowNo;
			process.IsSuccess = true;
			WorkflowProcessObserver.Instance.UpdateWorkflowProcess(processId, process);
			return actionResultUnits;
		}

		/// <summary>
		/// 受注ワークフローの実行履歴を登録する
		/// </summary>
		/// <param name="workflowName">ワークフロー名</param>
		/// <param name="workflowType">ワークフロータイプ</param>
		/// <returns>受注ワークフロー実行履歴</returns>
		private OrderWorkflowExecHistoryModel InsertOrderWorkflowExecHistory(string workflowName, string workflowType)
		{
			var history = WorkFlowHistoryHelper.InsertFunction(this.LoginOperator.ShopId, workflowName, workflowType, this.Request.WorkflowKbn, int.Parse(this.Request.WorkflowNo), this.LoginOperator.Name);
			return history;
		}

		/// <summary>
		/// Has digital contents
		/// </summary>
		/// <param name="orders">Orders</param>
		/// <returns>true : デジタルコンテンツ商品あり / false : デジタルコンテンツ商品なし</returns>
		protected bool HasDigitalContents(DataView orders)
		{
			if ((Constants.DIGITAL_CONTENTS_OPTION_ENABLED == false)
				|| (orders.Count == 0)) return false;

			var result = orders.Cast<DataRowView>()
				.Any(drv
					=> (StringUtility.ToEmpty(drv[Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG])
						== Constants.FLG_PRODUCT_DIGITAL_CONTENTS_FLG_VALID));
			return result;
		}

		/// <summary>
		/// Crearte response data
		/// </summary>
		/// <returns>Response data</returns>
		public WorkflowResponse CreateResponseData()
		{
			Hashtable searchParam;
			DataView orderList;
			if (this.Request.WorkflowType == WorkflowSetting.m_KBN_WORKFLOW_TYPE_ORDER)
			{
				searchParam = GetSearchParamsOrder();
				orderList = OrderCommon.GetOrderListForWorkflow(
					searchParam,
					this.Request.CurrentPage);
			}
			else
			{
				searchParam = GetSearchParamsFixedPurchase();
				orderList = DomainFacade.Instance.FixedPurchaseService.GetFixedPurchaseWorkflowList(
					searchParam,
					this.Request.CurrentPage);
			}

			var response = new WorkflowResponse();
			response.HasValue = (orderList.Count > 0);
			response.RunningWorkflowHistory = GetRunningWorkflowHistory();
			if (response.RunningWorkflowHistory != null)
			{
				response.RunningWorkflowHistory.ErrorMessage =
					CommerceMessages.GetMessages(CommerceMessages.ERRMSG_MANAGER_ORDERWORKFLOW_OTHER_RUNNING)
						.Replace("@@ 1 @@", response.RunningWorkflowHistory.WorkflowName);
			}

			var orders = new List<OrderResponse>();
			if (orderList.Count <= 100)
			{
				foreach (DataRowView item in orderList)
				{
					var order = new OrderResponse(item, this.Request.WorkflowType);
					if (this.OrderWorkflow.DisplayKbn != Constants.FLG_ORDERWORKFLOWSETTING_DISPLAY_KBN_LINE)
					{
						var orderItems = (this.Request.WorkflowType == WorkflowSetting.m_KBN_WORKFLOW_TYPE_ORDER)
							? DomainFacade.Instance.OrderService.GetItemAll(order.OrderId)
								.Select(orderItem => orderItem.DataSource)
								.ToArray()
							: DomainFacade.Instance.FixedPurchaseService.GetContainer(order.FixedPurchaseId)
								.Shippings[0]
								.Items
								.Select(fixedPurchaseItem
									=> fixedPurchaseItem.DataSource)
								.ToArray();
						order.OrderItems = orderItems
							.Select(orderitem => new OrderItemResponse(orderitem, this.Request.WorkflowType))
							.ToArray();
					}
					orders.Add(order);
				}
			}
			else
			{
				response.IsOver100 = true;
			}

			var totalPage = (orderList.Count != 0)
				? Math.Ceiling(
					(decimal.Parse(orderList[0][Constants.FIELD_COMMON_ROW_COUNT].ToString())
					/ decimal.Parse(searchParam[Constants.FIELD_ORDERWORKFLOWSETTING_DISPLAY_COUNT].ToString())))
				: 0;

			if (response.HasValue)
			{
				// 検索情報取得
				var sqlParam = (this.Request.WorkflowType == WorkflowSetting.m_KBN_WORKFLOW_TYPE_ORDER)
					? GetSearchParamsOrder(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDER_WORKFLOW)
					: GetSearchParamsFixedPurchase(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_FIXEDPURCHASE_WORKFLOW);
				var sqlParams = new Hashtable
				{
					{
						Constants.FIELD_TARGETLIST_TARGET_TYPE, (this.Request.WorkflowType == WorkflowSetting.m_KBN_WORKFLOW_TYPE_ORDER)
							? Constants.FLG_TARGETLIST_TARGET_TYPE_ORDERWORKFLOW_LIST
							: Constants.FLG_TARGETLIST_TARGET_TYPE_FIXEDPURCHASE_WORKFLOW_LIST
					},
					{ Constants.TABLE_USER, sqlParam }
				};
				HttpContext.Current.Session[Constants.SESSION_KEY_PARAM + "EC"] = sqlParams;
			}

			response.Orders = orders.ToArray();
			response.Pages = CalculatePager(totalPage).ToArray();
			response.TotalCase = (orderList.Count != 0)
				? StringUtility.ToNumeric(orderList[0][Constants.FIELD_COMMON_ROW_COUNT])
				: "0";
			response.TotalPage = (int)totalPage;
			response.HasSearchBox = this.OrderWorkflow.IsAdditionalSearchFlgOn;
			response.DetailKbn = this.OrderWorkflow.DetailKbn;
			response.UpdateStatusValid = this.OrderWorkflow.UpdateStatusValid;
			return response;
		}

		/// <summary>
		/// 実行中のワークフロー実行履歴を取得
		/// </summary>
		/// <returns>実行中のワークフロー実行履歴</returns>
		private OrderWorkflowExecHistoryModel GetRunningWorkflowHistory()
		{
			var history = DomainFacade.Instance.OrderWorkflowExecHistoryService
				.GetByExecStatus(Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_RUNNING);
			return history;
		}

		/// <summary>
		/// Calculate pager
		/// </summary>
		/// <param name="totalPage">Total page</param>
		/// <returns>A list string</returns>
		private List<string> CalculatePager(decimal totalPage)
		{
			var pageList = new List<string>();
			if (totalPage != 0)
			{
				for (var index = 0; index < totalPage; index++)
				{
					var page = index + 1;
					if (((page <= (this.Request.CurrentPage + 4))
							&& (page >= (this.Request.CurrentPage - 4)))
						|| ((page == totalPage)
						|| (page == 1)))
					{
						pageList.Add(page.ToString());
					}
					else if ((page == (this.Request.CurrentPage + 5))
						|| (page == (this.Request.CurrentPage - 5)))
					{
						pageList.Add("...");
					}
				}
			}
			return pageList;
		}

		/// <summary>
		/// Get search params order
		/// </summary>
		/// <param name="masterKbn">Master kbn for case export</param>
		/// <returns>Search params</returns>
		public Hashtable GetSearchParamsOrder(string masterKbn = null)
		{
			var isExport = (string.IsNullOrEmpty(masterKbn) == false);
			var orderSearchParam = new OrderSearchParam().GetSearchParamOrder(
				this.Request.WorkflowType,
				this.LoginOperator.ShopId,
				this.Request.WorkflowKbn,
				this.Request.WorkflowNo,
				this.OrderListCondition,
				this.OrderWorkflow,
				GlobalConfigUtil.UseLeadTime(),
				isExport
					? masterKbn
					: null);
			return orderSearchParam;
		}

		/// <summary>
		/// Get search params fixed purchase
		/// </summary>
		/// <param name="masterKbn">Master kbn for case export</param>
		/// <returns>Search params fixed purchase</returns>
		public Hashtable GetSearchParamsFixedPurchase(string masterKbn = null)
		{
			var fixedPurchaseSearchParam = new OrderSearchParam().GetSearchParamFixedPurchase(
				this.Request.WorkflowType,
				this.LoginOperator.ShopId,
				this.Request.WorkflowKbn,
				this.Request.WorkflowNo,
				this.FixedPurchaseListCondition,
				this.OrderWorkflow,
				GlobalConfigUtil.UseLeadTime(),
				masterKbn);
			return fixedPurchaseSearchParam;
		}

		/// <summary>
		/// Get all order list
		/// </summary>
		/// <returns>A list of order</returns>
		protected WorkflowExecRequest[] GetAllOrderList()
		{
			Hashtable searchParam;
			DataView orderList;

			if (this.Request.WorkflowType == WorkflowSetting.m_KBN_WORKFLOW_TYPE_ORDER)
			{
				searchParam = GetSearchParamsOrder();
				orderList = DomainFacade.Instance.OrderService.GetOrderWorkflowListNoPagination(searchParam);
			}
			else
			{
				searchParam = GetSearchParamsFixedPurchase();
				orderList = DomainFacade.Instance.FixedPurchaseService.GetFixedPurchaseWorkflowListNoPagination(searchParam);
			}

			var result = orderList.Cast<DataRowView>()
				.Select((drv, index) =>
					new WorkflowExecRequest
					{
						Index = index,
						OrderId = (this.Request.WorkflowType == WorkflowSetting.m_KBN_WORKFLOW_TYPE_ORDER)
							? StringUtility.ToEmpty(drv[Constants.FIELD_ORDER_ORDER_ID])
							: StringUtility.ToEmpty(drv[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID]),
						CassetteAction = string.Empty
					})
				.ToArray();
			return result;
		}

		/// <summary>
		/// Update order count for workflow setting
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="workflowType">Workflow type</param>
		/// <param name="workflowKbn">Workflow kbn</param>
		/// <param name="workflowNo">Workflow no</param>
		/// <returns>Order count</returns>
		public int UpdateOrderCountForWorkflowSetting(
			string shopId,
			string workflowType,
			string workflowKbn,
			string workflowNo)
		{
			var orderCount = 0;
			try
			{
				using (var accessor = new SqlAccessor())
				{
					accessor.OpenConnection();
					accessor.BeginTransaction();

					orderCount = UpdateOrderCountForWorkflowSetting(
						shopId,
						workflowType,
						workflowKbn,
						workflowNo,
						accessor);

					accessor.CommitTransaction();
				}
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
			}
			return orderCount;
		}

		/// <summary>
		/// Update order count for workflow setting
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="workflowType">Workflow type</param>
		/// <param name="workflowKbn">Workflow kbn</param>
		/// <param name="workflowNo">Workflow no</param>
		/// <param name="accessor">SQL Accessor</param>
		/// <returns>Order count</returns>
		public int UpdateOrderCountForWorkflowSetting(
			string shopId,
			string workflowType,
			string workflowKbn,
			string workflowNo,
			SqlAccessor accessor)
		{
			var dispSummaryAnalysisService = DomainFacade.Instance.DispSummaryAnalysisService;
			var orderCount = GetOrderCountForWorkflowSetting(shopId, workflowType, workflowKbn, workflowNo);
			switch (workflowType)
			{
				case WorkflowSetting.m_KBN_WORKFLOW_TYPE_ORDER:
					// Update order count for order workflow setting
					dispSummaryAnalysisService.InsertOrderCountForWorkflowSetting(
						Constants.CONST_DEFAULT_DEPT_ID,
						Constants.FLG_DISPSUMMARYANALYSYS_SUMMARY_KBN_ORDER_WORKFLOW_COUNT,
						string.Format("{0}_{1}", workflowKbn, workflowNo),
						orderCount,
						accessor);
					break;

				case WorkflowSetting.m_KBN_WORKFLOW_TYPE_FIXEDPURCHASE:
					// Update order count for fixed purchase workflow setting
					dispSummaryAnalysisService.InsertOrderCountForWorkflowSetting(
						Constants.CONST_DEFAULT_DEPT_ID,
						Constants.FLG_DISPSUMMARYANALYSYS_SUMMARY_KBN_FIXED_PURCHASE_WORKFLOW_COUNT,
						string.Format("{0}_{1}", workflowKbn, workflowNo),
						orderCount,
						accessor);
					break;
			}
			return orderCount;
		}

		/// <summary>
		/// Get order count for workflow setting
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="workflowType">Workflow type</param>
		/// <param name="workflowKbn">Workflow kbn</param>
		/// <param name="workflowNo">Workflow no</param>
		/// <returns>Order count</returns>
		public int GetOrderCountForWorkflowSetting(
			string shopId,
			string workflowType,
			string workflowKbn,
			string workflowNo)
		{
			var orderSearchParamHelper = new OrderSearchParam();
			var orderCount = 0;
			switch (workflowType)
			{
				case WorkflowSetting.m_KBN_WORKFLOW_TYPE_ORDER:
					// Create search param for order workflow
					var searchParamOrder = orderSearchParamHelper.GetSearchParamOrder(
						workflowType,
						shopId,
						workflowKbn,
						workflowNo,
						new OrderListConditionForWorkflow
						{
							IsSelectedByWorkflow = true
						},
						new WorkflowSetting(),
						GlobalConfigUtil.UseLeadTime());
					orderCount = DomainFacade.Instance.OrderService.GetOrderCountByOrderWorkflowSetting(searchParamOrder);
					break;

				case WorkflowSetting.m_KBN_WORKFLOW_TYPE_FIXEDPURCHASE:
					// Create search param for fixed purchase workflow
					var searchParamFixedPurchase = orderSearchParamHelper.GetSearchParamFixedPurchase(
						workflowType,
						shopId,
						workflowKbn,
						workflowNo,
						new FixedPurchaseConditionForWorkflow
						{
							IsSelectedByWorkflow = true
						},
						new WorkflowSetting(),
						GlobalConfigUtil.UseLeadTime());
					orderCount = DomainFacade.Instance.FixedPurchaseService.GetOrderCountByFixedPurchaseWorkflowSetting(searchParamFixedPurchase);
					break;
			}
			return orderCount;
		}

		/// <summary>
		/// Send mail all orders to real shop
		/// </summary>
		private void SendMailAllOrderToRealShop()
		{
			var mailCategories = DomainFacade.Instance.MailTemplateService.GetAll(Constants.CONST_DEFAULT_SHOP_ID);
			var isMailCategoryToRealShop = mailCategories
				.Select(mailCategory =>
					(mailCategory.MailCategory.Contains(Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_TOREALSHOP))
					&& (mailCategory.DelFlg == Constants.FLG_MAILTEMPLATE_DELFLG_UNDELETED))
				.Any();

			if (isMailCategoryToRealShop)
			{
				var orderWorkflowSettingModel = DomainFacade.Instance.OrderWorkflowSettingService.Get(
					this.LoginOperator.ShopId,
					this.Request.WorkflowKbn,
					int.Parse(this.Request.WorkflowNo));

				var executeTargetOrders = GetListOrderIdSendMailToRealShop(this.ListOrderId);
				if (executeTargetOrders == null) return;

				foreach (var executeTargetOrder in executeTargetOrders)
				{
					OrderCommon.SendMailAllOrderToRealShop(
						orderIds: executeTargetOrder.Item2,
						mailId: Constants.CONST_MAIL_ID_TO_REAL_SHOP,
						realShopId: executeTargetOrder.Item1,
						mailSendMethod: Constants.MailSendMethod.Manual);
				}
			}
		}

		/// <summary>
		/// Get list order id send mail to real shop
		/// </summary>
		/// <param name="orderIds">List order id</param>
		/// <returns>Target orders</returns>
		private List<Tuple<string, List<string>>> GetListOrderIdSendMailToRealShop(
			List<string> orderIds)
		{
			var orderRealShops = new List<Tuple<string, string>>();
			foreach (var orderId in orderIds)
			{
				var orderShipping = DomainFacade.Instance.OrderService.GetOrderShippingInDataView(orderId);
				if (orderShipping.Count == 0) continue;

				var realShopId = StringUtility.ToEmpty(orderShipping[0][Constants.FIELD_ORDERSHIPPING_STOREPICKUP_REAL_SHOP_ID]);
				if (string.IsNullOrEmpty(realShopId)) continue;

				orderRealShops.Add(new Tuple<string, string>(realShopId, orderId));
			}
			if (orderRealShops.Count == 0) return null;

			var realShopIds = orderRealShops.Select(orderRealShop => orderRealShop.Item1).Distinct().ToList();
			var result = new List<Tuple<string, List<string>>>();

			foreach (var realShopId in realShopIds)
			{
				var orders = orderRealShops
					.Where(orderRealShop => orderRealShop.Item1 == realShopId)
					.Select(orderRealShop => orderRealShop.Item2)
					.ToList();
				result.Add(new Tuple<string, List<string>>(realShopId, orders));
			}
			return result;
		}
		#endregion

		#region Properties
		/// <summary>Running workflow history</summary>
		private OrderWorkflowExecHistoryModel RunningWorkflowHistory { get; set; }
		/// <summary>The request</summary>
		public WorkflowRequest Request { get; set; }
		/// <summary>The request for validate</summary>
		public WorkflowValidateRequest ValidateRequest { get; set; }
		/// <summary>Order workflow</summary>
		protected WorkflowSetting OrderWorkflow { get; set; }
		/// <summary>Login operator</summary>
		private ShopOperatorModel LoginOperator
		{
			get
			{
				var shopOperatorModel = (ShopOperatorModel)HttpContext.Current.Session[Constants.SESSION_KEY_LOGIN_SHOP_OPERTOR]
					?? new ShopOperatorModel();
				return shopOperatorModel;
			}
		}
		/// <summary>Has selected</summary>
		private bool HasSelected { get; set; }
		/// <summary>Result</summary>
		protected ActionResults Results { get; set; }
		/// <summary>Order list condition</summary>
		private OrderListConditionForWorkflow OrderListCondition { get; set; }
		/// <summary>Fixed purchase list condition</summary>
		private FixedPurchaseConditionForWorkflow FixedPurchaseListCondition { get; set; }
		/// <summary>ネクストエンジン受注情報配列</summary>
		protected NEOrder[] UpdateNextEngineOrders { get; set; }
		/// <summary>List order ids</summary>
		public List<string> ListOrderId { get; set; }
		#endregion
	}
}
