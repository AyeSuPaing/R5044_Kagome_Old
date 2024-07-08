/*
=========================================================================================================
  Module      : ワークフロー実行クラス(WorkflowExec.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using w2.App.Common.NextEngine;
using w2.App.Common.NextEngine.Helper;
using w2.App.Common.NextEngine.Response;
using w2.Common.Extensions;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain;
using w2.Domain.DeliveryCompany;
using w2.Domain.FixedPurchase;
using w2.Domain.FixedPurchase.Helper;
using w2.Domain.FixedPurchaseWorkflowSetting;
using w2.Domain.Order;
using w2.Domain.Order.Helper;
using w2.Domain.OrderWorkflowExecHistory;
using w2.Domain.OrderWorkflowScenarioSetting;
using w2.Domain.OrderWorkflowSetting;
using w2.Domain.TaskSchedule;
using w2.Domain.UpdateHistory.Helper;

namespace w2.App.Common.Order.Workflow
{
	/// <summary>
	/// ワークフロー実行クラス
	/// </summary>
	public class WorkflowExec
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public WorkflowExec()
		{
		}

		/// <summary>
		/// コンストラクタ(シナリオ実行の場合)
		/// </summary>
		/// <param name="deptId">デプトID</param>
		/// <param name="actionKbn">アクション区分</param>
		/// <param name="masterId">マスタ－ID</param>
		/// <param name="actionNo">アクションNo</param>
		/// <param name="scheduleDate">スケジュール</param>
		/// <param name="lastChanged">最終実行者</param>
		/// <param name="execStatus">実行ステータス</param>
		public WorkflowExec(
			string deptId,
			string actionKbn,
			string masterId,
			int actionNo,
			DateTime scheduleDate,
			string lastChanged,
			string execStatus)
		{
			var scenario = new OrderWorkflowScenarioSettingService().GetForDatails(masterId);

			scenario.Items = scenario.Items.Where(scenarioItem => scenarioItem.TargetWorkflowKbn == Constants.FLG_ORDERWORKFLOWSCENARIOSETTING_TARGETKBN_NORMAL
				? scenarioItem.OrderWorkflowSetting.IsValid
				: scenarioItem.FixedPurchaseWorkflowSetting.IsValid).ToArray();
			this.ScenarioExecResult = new ScenarioExecResult
			{
				DeptId = deptId,
				ActionKbn = actionKbn,
				MasterId = masterId,
				ActionNo = actionNo,
				ScheduleDate = scheduleDate,
				LastChanged = lastChanged,
				ExecTiming = execStatus,
				Scenario = scenario,
				WorkflowCountBeExec = scenario.Items.Length,
				WorkflowExecSuccessfulCount = 0,
				ActionCountBeExecOfRunningWorkflow = 0,
				ActionExecSuccessfulCountOfRunningWorkflow = 0,
				ActionExecCountOfRunningWorkflow = 0,
			};
		}

		/// <summary>
		/// シナリオの実行をする
		/// </summary>
		/// <returns>ScenarioExecResult</returns>
		public ScenarioExecResult ExecOrderWorkflowScenario()
		{
			UpdatePrepareTaskStatus(
				Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_EXECUTE,
				Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_INIT);

			this.ScenarioExecResult.Histories = InsertWorkflowExecHistories();

			WaitingForWorkflowExec();

			if (this.ScenarioExecResult.Histories.Any())
			{
				this.ScenarioExecResult.Histories[0] = UpdateOrderWorkflowExecHistory(
					this.ScenarioExecResult.Histories[0],
					GetHistoryStartAction(
						Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_RUNNING,
						Constants.FLG_ORDERWORKFLOWEXECHISTORY_MESSAGE_FOR_WORKFLOW_RUNNING,
						DateTime.Now));
			}

			this.ScenarioExecResult.ExecStartDatetime = DateTime.Now;

			SendStartMail();

			UpdateTaskStatusBegin(
				Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_EXECUTE,
				Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_INIT,
				"");

			for (var key = 0; this.ScenarioExecResult.Histories.Count > key; key++)
			{
				var workflowItemToExecute = this.ScenarioExecResult.Histories[key];

				// 実行中のワークフローがなくなって並列実行されないよう、次に回すワークフローを実行中に更新
				var nextWorkflowKey = (key + 1);
				if (this.ScenarioExecResult.Histories.Count > nextWorkflowKey)
				{
					this.ScenarioExecResult.Histories[nextWorkflowKey] = UpdateOrderWorkflowExecHistory(
						this.ScenarioExecResult.Histories[nextWorkflowKey],
						GetHistoryStartAction(
							Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_RUNNING,
							Constants.FLG_ORDERWORKFLOWEXECHISTORY_MESSAGE_FOR_WORKFLOW_RUNNING,
							DateTime.Now));
				}

				// 実行対象ワークフロー区分が受注のかどうか
				var isTargetNormal = this.ScenarioExecResult.Scenario.Items[key].TargetWorkflowKbn == Constants.FLG_ORDERWORKFLOWSCENARIOSETTING_TARGETKBN_NORMAL;

				var hasError = false;
				var errorMessage = string.Empty;
				// 実行対象ワークフロー区分が受注の処理
				if (isTargetNormal)
				{
					var workflowToExec = new OrderWorkflowSettingService().Get(
						workflowItemToExecute.ShopId,
						workflowItemToExecute.WorkflowKbn,
						workflowItemToExecute.WorkflowNo);

					// エラーがあるかどうかで履歴のアップデート内容を変更
					hasError = ExecOrderWorkflowForScenario(workflowToExec, out errorMessage) == false;
				}
				else
				{
					var workflowToExec = new FixedPurchaseWorkflowSettingService().Get(
						workflowItemToExecute.ShopId,
						workflowItemToExecute.WorkflowKbn,
						workflowItemToExecute.WorkflowNo);

					// エラーがあるかどうかで履歴のアップデート内容を変更
					hasError = ExecOrderWorkflowForScenario(workflowToExec, out errorMessage) == false;
				}

				this.ScenarioExecResult.Histories[key] = UpdateOrderWorkflowExecHistory(
					this.ScenarioExecResult.Histories[key],
					GetHistoryEndAction(
						hasError
							? Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_NG
							: Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_OK,
						CreateHistoryProgress(
							this.ScenarioExecResult.ActionExecSuccessfulCountOfRunningWorkflow,
							this.ScenarioExecResult.ActionCountBeExecOfRunningWorkflow),
						hasError
							? isTargetNormal
								? Constants.FLG_ORDERWORKFLOWEXECHISTORY_MESSAGE_FOR_WORKFLOW_FAILURE
								: Constants.FLG_ORDERWORKFLOWEXECHISTORY_MESSAGE_FOR_FIXEDPURCHASE_FAILURE
							: isTargetNormal
								? Constants.FLG_ORDERWORKFLOWEXECHISTORY_MESSAGE_FOR_WORKFLOW_SUCCESS
								: Constants.FLG_ORDERWORKFLOWEXECHISTORY_MESSAGE_FOR_FIXEDPURCHASE_SUCCESS,
						DateTime.Now));

				if (hasError)
				{
					// エラーログを落とす
					AppLogger.WriteError(string.Format(
						"「{0}」のワークフローで一部失敗がありました。{1}",
						this.ScenarioExecResult.Histories[key].WorkflowName,
						errorMessage));

					// 管理者へのメール送信のため、シナリオ実施結果にエラーメッセージを追加
					this.ScenarioExecResult.Histories[key].ErrorMessage = errorMessage;
				}

				this.ScenarioExecResult.ActionExecSuccessfulCountOfRunningWorkflow = 0;
				this.ScenarioExecResult.ActionExecCountOfRunningWorkflow = 0;

				// Update order count for workflow setting
				new WorkflowUtility().UpdateOrderCountForWorkflowSetting(
					workflowItemToExecute.ShopId,
					Constants.FLG_ORDERWORKFLOWSETTING_TARGET_WORKFLOW_TYPE_ORDER,
					workflowItemToExecute.WorkflowKbn,
					workflowItemToExecute.WorkflowNo.ToString());
			}

			this.ScenarioExecResult.ScenarioExecStatus = Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_DONE;
			this.ScenarioExecResult.ExecEndDatetime = DateTime.Now;

			return this.ScenarioExecResult;
		}

		/// <summary>
		/// シナリオ用の受注ワークフロー実行
		/// </summary>
		/// <param name="workflow">受注ワークフロー設定モデル</param>
		/// <param name="errorMessage">エラーメッセージ</param>
		/// <returns>成功か</returns>
		private bool ExecOrderWorkflowForScenario(OrderWorkflowSettingModel workflow, out string errorMessage)
		{
			var workflowSetting = GetWorkflowSetting(workflow);
			var sqlParam = GetWorkflowSearchParamOrderForScenarioExec(workflow);
			var orders = new OrderService().GetOrderWorkflowListNoPagination(sqlParam);

			var result = ExecOrderWorkflowForScenario(out errorMessage, workflowSetting, orders, Constants.FLG_ORDERWORKFLOWSCENARIOSETTING_TARGETKBN_NORMAL);
			return result;
		}

		/// <summary>
		/// シナリオ用の定期ワークフロー実行
		/// </summary>
		/// <param name="workflow">定期ワークフロー設定モデル</param>
		/// <param name="errorMessage">エラーメッセージ</param>
		/// <returns>成功か</returns>
		private bool ExecOrderWorkflowForScenario(FixedPurchaseWorkflowSettingModel workflow, out string errorMessage)
		{
			var workflowSetting = GetWorkflowSetting(workflow);
			var sqlParam = GetWorkflowSearchParamOrderForScenarioExec(workflow);
			var orders = new FixedPurchaseService().GetFixedPurchaseWorkflowListNoPagination(sqlParam);

			var result = ExecOrderWorkflowForScenario(out errorMessage, workflowSetting, orders, Constants.FLG_ORDERWORKFLOWSCENARIOSETTING_TARGETKBN_FIXEDPURCHASE);
			return result;
		}

		/// <summary>
		/// シナリオ用のワークフロー実行
		/// </summary>
		/// <param name="workflowSetting">ワークフロー設定</param>
		/// <param name="orders">受注情報</param>
		/// <param name="workflowType">実行対象ワークフロー区分</param>
		/// <returns>成功か</returns>
		private bool ExecOrderWorkflowForScenario(out string errorMessage, WorkflowSetting workflowSetting, DataView orders, string workflowType)
		{
			this.ScenarioExecResult.ActionCountBeExecOfRunningWorkflow = orders.Count;

			UpdatePrepareTaskStatus(
				Constants.FLG_TASKSCHEDULE_PREPARE_STATUS_DONE,
				Constants.FLG_TASKSCHEDULE_PREPARE_STATUS_EXECUTE);

			CheckScheduleStoppedAndUpdateProgress(CreateTaskSchedulerProgress());

			using (var accessor = new SqlAccessor())
			{
				var isAccessTokenValid = true;
				var isSuccessNextEngineUpload = true;
				var isSuccessNextEngineImport = true;

				var accessToken = string.Empty;
				var refreshToken = string.Empty;

				this.UpdateNextEngineOrders = new NEOrder[0];

				var nextEngineImportError = "";
				if (workflowSetting.ExternalOrderInfoActionValue
					== Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION_NEXTENGINE_IMPORT)
				{
					isSuccessNextEngineImport = isAccessTokenValid = NextEngineApi.IsExistsToken(out accessToken, out refreshToken);

					if (isAccessTokenValid)
					{
						var neOrderList = new List<NEOrder>();
						// 1000件ずつ処理
						foreach (var chunkOrders in orders.Cast<DataRowView>().Chunk(1000))
						{
							var orderIds = chunkOrders
								.Select(chunkOrder => workflowType == Constants.FLG_ORDERWORKFLOWSCENARIOSETTING_TARGETKBN_NORMAL
									? (string)chunkOrder[Constants.FIELD_ORDER_ORDER_ID]
									: (string)chunkOrder[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID])
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
									nextEngineImportError += (string.IsNullOrEmpty(nextEngineImportError) ? "" : "、") + searchOrderRsponse.Message;
									continue;

								case NextEngineConstants.FLG_RESULT_REDIRECT:
									isAccessTokenValid = isSuccessNextEngineImport = false;
									nextEngineImportError += (string.IsNullOrEmpty(nextEngineImportError) ? "" : "、") + searchOrderRsponse.Message;
									break;
							}
						}

						this.UpdateNextEngineOrders = neOrderList.ToArray();
					}
				}

				var deliveryCompanies = new DeliveryCompanyService().GetAll();
				accessor.OpenConnection();
				accessor.BeginTransaction();
				var actionResultUnits = orders.Cast<DataRowView>().Select(
					order =>
					{
						var resultUnits = ExecActionForScenario(workflowSetting, order,
							this.UpdateNextEngineOrders,
							deliveryCompanies,
							accessor,
							workflowType);
						if ((this.ScenarioExecResult.ActionExecCountOfRunningWorkflow % 10) == 0)
						{
							CheckScheduleStoppedAndUpdateProgress(CreateTaskSchedulerProgress());
						}
						return resultUnits;
					}).ToList();
				this.ScenarioExecResult.WorkflowExecSuccessfulCount++;
				CheckScheduleStoppedAndUpdateProgress(CreateTaskSchedulerProgress());

				if (isAccessTokenValid == false)
				{
					errorMessage = string.Format("{0}:{1}", Constants.FLG_ORDERWORKFLOWEXECHISTORY_MESSAGE_FOR_NEXTENGINE_INVALID_ACCESS_TOKEN, nextEngineImportError);
					return false;
				}
				if (isSuccessNextEngineImport == false)
				{
					errorMessage = string.Format("{0}:{1}", Constants.FLG_ORDERWORKFLOWEXECHISTORY_MESSAGE_FOR_WORKFLOW_FAILURE, nextEngineImportError);
					return false;
				}

				if (workflowSetting.ExternalOrderInfoActionValue
					!= Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION_NEXTENGINE)
				{
					errorMessage = CreateErrorResultMessage(actionResultUnits);
					var hasError = actionResultUnits.Any(actionResultUnit => actionResultUnit.HasError);
					return (hasError == false);
				}

				isAccessTokenValid &= NextEngineApi.IsExistsToken(out accessToken, out refreshToken);

				if (isAccessTokenValid == false)
				{
					errorMessage = Constants.FLG_ORDERWORKFLOWEXECHISTORY_MESSAGE_FOR_NEXTENGINE_INVALID_ACCESS_TOKEN;
					return false;
				}

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
						if (isSuccessNextEngineUpload == false)
						{
							errorMessage = string.Format("{0}:{1}", Constants.FLG_ORDERWORKFLOWEXECHISTORY_MESSAGE_FOR_WORKFLOW_FAILURE, uploadOrderResponse.Message);
							return false;
						}
						break;

					case NextEngineConstants.FLG_RESULT_ERROR:
						errorMessage = string.Format("{0}:{1}", Constants.FLG_ORDERWORKFLOWEXECHISTORY_MESSAGE_FOR_WORKFLOW_FAILURE, uploadPatternesponse.Message);
						return false;

					case NextEngineConstants.FLG_RESULT_REDIRECT:
						errorMessage = string.Format("{0}:{1}", Constants.FLG_ORDERWORKFLOWEXECHISTORY_MESSAGE_FOR_NEXTENGINE_INVALID_ACCESS_TOKEN, uploadPatternesponse.Message);
						return false;
				}

				errorMessage = CreateErrorResultMessage(actionResultUnits);
				var result = (actionResultUnits.Any(actionResultUnit => actionResultUnit.HasError) == false);
				return result;
			}
		}

		/// <summary>
		/// シナリオ用のアクション実行
		/// </summary>
		/// <param name="workflowSetting">ワークフロー設定</param>
		/// <param name="order">受注</param>
		/// <param name="updateNextengineOrders">更新ネクストエンジン受注情報</param>
		/// <param name="deliveryCompanies">配送会社配列</param>
		/// <param name="accessor">sqlアクセサー</param>
		/// <param name="workflowType">実行対象ワークフロー区分</param>
		/// <returns>アクションリザルトユニット</returns>
		private ActionResultUnit ExecActionForScenario(
			WorkflowSetting workflowSetting,
			DataRowView order,
			NEOrder[] updateNextengineOrders,
			DeliveryCompanyModel[] deliveryCompanies,
			SqlAccessor accessor,
			string workflowType)
		{

			var dateTimeNow = DateTime.Now.ToString("yyyy/MM/dd");
			if (workflowType == Constants.FLG_ORDERWORKFLOWSCENARIOSETTING_TARGETKBN_NORMAL)
			{
				// 金額取得のためDBから情報取得
				var orderOld = OrderCommon.GetOrder((string)order[Constants.FIELD_ORDER_ORDER_ID], accessor);
				if (orderOld.Count == 0) return null;

				var orderForUpdate = GetAnOrderToAction(
					orderOld,
					this.ScenarioExecResult.LastChanged,
					dateTimeNow,
					dateTimeNow,
					dateTimeNow);

				var actionResultUnit = workflowSetting.ActionForLine(
					orderForUpdate,
					orderOld,
					UpdateHistoryAction.Insert,
					updateNextengineOrders,
					deliveryCompanies,
					accessor);

				if (actionResultUnit.HasError == false) this.ScenarioExecResult.ActionExecSuccessfulCountOfRunningWorkflow++;
				this.ScenarioExecResult.ActionExecCountOfRunningWorkflow++;

				return actionResultUnit;
			}
			else
			{
				var fixedPurchase = DomainFacade.Instance.FixedPurchaseService.Get(order[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID].ToString(), accessor);
				if (fixedPurchase == null) return null;

				var orderForUpdate = new Hashtable
				{
					{ Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID, fixedPurchase.FixedPurchaseId },
					{ Constants.FIELD_ORDER_LAST_CHANGED, this.ScenarioExecResult.LastChanged },
					{ Constants.FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE, dateTimeNow },
					{ Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_DATE, dateTimeNow },
					{ Constants.FIELD_FIXEDPURCHASE_NEXT_NEXT_SHIPPING_DATE, dateTimeNow }
				};

				var actionResultUnit = workflowSetting.ActionForLineFixedPurchase(
					orderForUpdate,
					UpdateHistoryAction.Insert,
					accessor);

				if (actionResultUnit.HasError == false) this.ScenarioExecResult.ActionExecSuccessfulCountOfRunningWorkflow++;
				this.ScenarioExecResult.ActionExecCountOfRunningWorkflow++;

				return actionResultUnit;
			}
		}

		/// <summary>
		/// アクションを実行する
		/// </summary>
		/// <param name="workflowSetting">ワークフロー設定</param>
		/// <param name="workflowExecInfo">ワークフロー実行情報</param>
		/// <param name="loginOperatorName">ログインオペレーター名</param>
		/// <param name="isDisplayKbnLine">表示区分が一行表示か</param>
		/// <param name="updateDate">更新日</param>
		/// <param name="updateScheduledShippingDate">発送予定日</param>
		/// <param name="updateShippingDate">発送日</param>
		/// <param name="updateNextShippingDate">次回定期発送日</param>
		/// <param name="updateNextNextShippingDate">次々回定期発送日</param>
		/// <param name="updateNextEngineOrder">更新ネクストエンジン受注情報</param>
		/// <param name="deliveryCompanies">配送会社配列</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>アクションリザルトユニット</returns>
		public ActionResultUnit ExecAction(
			WorkflowSetting workflowSetting,
			WorkflowExecInfo workflowExecInfo,
			string loginOperatorName,
			bool isDisplayKbnLine,
			string updateDate,
			string updateScheduledShippingDate,
			string updateShippingDate,
			string updateNextShippingDate,
			string updateNextNextShippingDate,
			NEOrder[] updateNextEngineOrder,
			DeliveryCompanyModel[] deliveryCompanies,
			SqlAccessor accessor)
		{
			// 金額取得のためDBから情報取得
			var orderOld = OrderCommon.GetOrder(workflowExecInfo.MasterId);
			if (orderOld.Count == 0) return null;
			var orderForUpdate = GetAnOrderToAction(
				orderOld,
				loginOperatorName,
				updateDate,
				updateScheduledShippingDate,
				updateShippingDate,
				updateNextShippingDate,
				updateNextNextShippingDate);

			if (!workflowExecInfo.DoExec) return null;

			// アクション
			ActionResultUnit actionResultUnit;
			if (isDisplayKbnLine)
			{
				actionResultUnit = workflowSetting.ActionForLine(
					orderForUpdate,
					orderOld,
					UpdateHistoryAction.Insert,
					updateNextEngineOrder,
					deliveryCompanies,
					accessor);
				return actionResultUnit;
			}
			actionResultUnit = workflowSetting.ActionForCassette(
				orderForUpdate,
				orderOld,
				workflowExecInfo.UpdateCassetteStatus,
				UpdateHistoryAction.Insert,
				updateNextEngineOrder,
				deliveryCompanies,
				accessor);
			return actionResultUnit;
		}

		/// <summary>
		/// シナリオのワークフローを全て実行履歴にインサート
		/// </summary>
		/// <returns>インサートしたワークフロー(List)</returns>
		private List<OrderWorkflowExecHistoryModel> InsertWorkflowExecHistories()
		{
			var workflowExecHistoryModels = this.ScenarioExecResult.Scenario.Items.Select(
				(item, count) =>
				{
					var workflowName = string.Empty;
					if (item.TargetWorkflowKbn == Constants.FLG_ORDERWORKFLOWSCENARIOSETTING_TARGETKBN_NORMAL)
					{
						var workflowToExec = new OrderWorkflowSettingService().Get(
							item.ShopId,
							item.WorkflowKbn,
							item.WorkflowNo.Value);

						workflowName = workflowToExec.WorkflowName;
					}
					else
					{
						var workflowToExec = new FixedPurchaseWorkflowSettingService().Get(
							item.ShopId,
							item.WorkflowKbn,
							item.WorkflowNo.Value);

						workflowName = workflowToExec.WorkflowName;
					}
					var history = InsertOrderWorkflowExecHistory(
						item.ShopId,
						item.WorkflowKbn,
						item.WorkflowNo.Value,
						string.Format("{0}.{1}", item.ScenarioNo, workflowName),
						item.TargetWorkflowKbn);
					return history;
				}).ToList();

			return workflowExecHistoryModels;
		}

		/// <summary>
		/// 他のワークフローで実行中がある場合に順番を待つ
		/// </summary>
		private void WaitingForWorkflowExec()
		{
			while (true)
			{
				Delay(10000);

				var runningHistory = new OrderWorkflowExecHistoryService()
					.GetByExecStatus(Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_RUNNING);

				if (runningHistory == null) break;
				CheckScheduleStoppedAndUpdateProgress(Constants.FLG_TASK_SCHEDULE_WORKFLOW_EXEC_PROGRESS_WAITING_FOR_EXEC);
			}
		}

		/// <summary>
		/// 処理待ち
		/// </summary>
		/// <param name="millisecond">待たせるミリ秒</param>
		private void Delay(int millisecond)
		{
			Task.Delay(millisecond).Wait();
		}

		/// <summary>
		/// 開始メールを送信
		/// </summary>
		private void SendStartMail()
		{
			var execStatus = ValueText.GetValueText(
				Constants.TABLE_ORDERWORKFLOWEXECHISTORY,
				Constants.FIELD_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS,
				Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_RUNNING);
			var scenarioNameOfMessage = string.Format(
				"[{0}]",
				this.ScenarioExecResult.Scenario.ScenarioName);
			var execStatusOfMessage = string.Format(
				ValueText.GetValueText(
					Constants.VALUE_TEXT_KEY_ORDER_WORKFLOW_EXEC_THREAD,
					Constants.VALUE_TEXT_FIELD_ORDER_WORKFLOW_EXEC_THREAD_MAIL_FORMAT,
					Constants.VALUE_TEXT_ORDER_WORKFLOW_EXEC_THREAD_MAIL_FORMAT_EXEC_STATUS),
				execStatus);
			var execStartDatetimeOfMessage = string.Format(
				ValueText.GetValueText(
					Constants.VALUE_TEXT_KEY_ORDER_WORKFLOW_EXEC_THREAD,
					Constants.VALUE_TEXT_FIELD_ORDER_WORKFLOW_EXEC_THREAD_MAIL_FORMAT,
					Constants.VALUE_TEXT_ORDER_WORKFLOW_EXEC_THREAD_MAIL_FORMAT_START_TIME),
				this.ScenarioExecResult.ExecStartDatetime);
			var workflowsStateOfMessage = CreateWorkflowsStateForMail();

			var massage = string.Format(
				"{0}\r\n{1}\r\n{2}\r\n\r\n{3}",
				scenarioNameOfMessage,
				execStatusOfMessage,
				execStartDatetimeOfMessage,
				workflowsStateOfMessage);

			var mailInput = new Hashtable
			{
				{ Constants.MAILTEMPLATE_KEY_EXEC_STATUS, "" },
				{ Constants.MAILTEMPLATE_KEY_SCENARIO_NAME, this.ScenarioExecResult.Scenario.ScenarioName },
				{ Constants.MAILTEMPLATE_KEY_MAIL_TYPE,
					ValueText.GetValueText(
						Constants.VALUE_TEXT_KEY_ORDER_WORKFLOW_EXEC_THREAD,
						Constants.MAILTEMPLATE_KEY_MAIL_TYPE,
						Constants.VALUE_TEXT_IMPORT_MAILTEMPLATE_KEY_MAIL_TYPE_START) },
				{ Constants.MAILTEMPLATE_KEY_SCENARIO_MESSAGE, massage },
			};

			using (MailSendUtility msMailSend = new MailSendUtility(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.CONST_MAIL_ID_ORDER_WORKFLOW_SCENARIO_EXEC,
				"",
				mailInput, true, Constants.MailSendMethod.Auto))
			{
				if (msMailSend.SendMail() == false)
				{
					FileLogger.WriteError(string.Format("{0} : {1}", GetType().BaseType, msMailSend.MailSendException));
				}
			}
		}

		/// <summary>
		/// 開始メールに記載するワークフローの実行ステータスを作成
		/// </summary>
		/// <returns>メールに記載するワークフローの実行ステータス</returns>
		private string CreateWorkflowsStateForMail()
		{
			var workflowStateList = this.ScenarioExecResult.Histories.Select(
				history =>
				{
					var historyDetailsUrl = new UrlCreator(
						string.Format(
							"{0}{1}{2}{3}",
							Constants.PROTOCOL_HTTPS,
							Constants.SITE_DOMAIN,
							Constants.PATH_ROOT_EC,
							Constants.PAGE_MANAGER_ORDERWORKFLOW_EXEC_HISTORY_DETAILS))
						.AddParam(
							Constants.FIELD_ORDERWORKFLOWEXECHISTORY_ORDER_WORKFLOW_EXEC_HISTORY_ID,
							history.OrderWorkflowExecHistoryId.ToString())
						.CreateUrl();

					var workflowState = GetWorkflowStateStringForMail(
						history.WorkflowName,
						ValueText.GetValueText(
							Constants.TABLE_ORDERWORKFLOWEXECHISTORY,
							Constants.FIELD_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS,
							history.ExecStatus),
						historyDetailsUrl);
					return workflowState;
				}).ToList();

			return string.Join("\r\n\r\n", workflowStateList);
		}

		/// <summary>
		/// メール送信する時のワークフローのステータス文字列を取得
		/// </summary>
		/// <param name="workflowName">ワークフロー名</param>
		/// <param name="execStatus">実行ステータス</param>
		/// <param name="historyDetailsUrl">実行履歴詳細URL</param>
		/// <returns>ワークフローのステータス文字列</returns>
		private string GetWorkflowStateStringForMail(
			string workflowName,
			string execStatus,
			string historyDetailsUrl)
		{
			var execStatusForWriteMail = string.Format(
				ValueText.GetValueText(
					Constants.VALUE_TEXT_KEY_ORDER_WORKFLOW_EXEC_THREAD,
					Constants.VALUE_TEXT_FIELD_ORDER_WORKFLOW_EXEC_THREAD_MAIL_FORMAT,
					Constants.VALUE_TEXT_ORDER_WORKFLOW_EXEC_THREAD_MAIL_FORMAT_EXEC_STATUS),
				execStatus);

			var historyDetailsUrlForWriteMail = string.Format(
				ValueText.GetValueText(
					Constants.VALUE_TEXT_KEY_ORDER_WORKFLOW_EXEC_THREAD,
					Constants.VALUE_TEXT_FIELD_ORDER_WORKFLOW_EXEC_THREAD_MAIL_FORMAT,
					Constants.VALUE_TEXT_ORDER_WORKFLOW_EXEC_THREAD_MAIL_FORMAT_HISTORY_DETAILS_URL),
				historyDetailsUrl);

			var workflowState = string.Format(
				"{0}\r\n\t{1}\r\n\t{2}",
				workflowName,
				execStatusForWriteMail,
				historyDetailsUrlForWriteMail);
			return workflowState;
		}

		/// <summary>
		/// タスクステータス更新（開始）
		/// </summary>
		/// <param name="strStatus">ステータス</param>
		/// <param name="strTargetStatus">更新対象ステータス</param>
		/// <param name="strProgress">進捗</param>
		/// <returns>更新レコード数</returns>
		private void UpdateTaskStatusBegin(string strStatus, string strTargetStatus, string strProgress)
		{
			var taskSchedule = new TaskScheduleModel
			{
				DeptId = this.ScenarioExecResult.DeptId,
				ActionKbn = this.ScenarioExecResult.ActionKbn,
				ActionMasterId = this.ScenarioExecResult.MasterId,
				ActionNo = this.ScenarioExecResult.ActionNo,
				ExecuteStatus = strStatus,
				ExecuteStatusTarget = strTargetStatus,
				Progress = strProgress,
				ScheduleDate = this.ScenarioExecResult.ScheduleDate,
				LastChanged = this.ScenarioExecResult.LastChanged,
			};
			new TaskScheduleService().UpdateTaskStatusBegin(taskSchedule);
		}

		/// <summary>
		/// タスク準備ステータス更新
		/// </summary>
		/// <param name="strStatus">ステータス</param>
		/// <param name="strTargetStatus">更新対象ステータス</param>
		/// <returns>更新レコード数</returns>
		private void UpdatePrepareTaskStatus(string strStatus, string strTargetStatus)
		{
			var taskSchedule = new TaskScheduleModel
			{
				DeptId = this.ScenarioExecResult.DeptId,
				ActionKbn = this.ScenarioExecResult.ActionKbn,
				ActionMasterId = this.ScenarioExecResult.MasterId,
				ActionNo = this.ScenarioExecResult.ActionNo,
				PrepareStatus = strStatus,
				PrepareStatusTarget = strTargetStatus,
				ScheduleDate = this.ScenarioExecResult.ScheduleDate,
				LastChanged = this.ScenarioExecResult.LastChanged,
			};
			new TaskScheduleService().UpdatePrepareTaskStatus(taskSchedule);
		}

		/// <summary>
		/// 受注ワークフロー設定を取得
		/// </summary>
		/// <param name="workflow">受注ワークフロー設定モデル</param>
		/// <returns>ワークフロー設定</returns>
		private WorkflowSetting GetWorkflowSetting(OrderWorkflowSettingModel workflow)
		{
			var orderToSetting = new WorkflowSetting().GetOrderWorkflowSetting(
				workflow.ShopId,
				workflow.WorkflowKbn,
				workflow.WorkflowNo.Value.ToString(),
				WorkflowSetting.m_KBN_WORKFLOW_TYPE_ORDER)[0];

			var workflowSetting = new WorkflowSetting(
				orderToSetting,
				this.ScenarioExecResult.DeptId,
				this.ScenarioExecResult.LastChanged,
				WorkflowSetting.WorkflowTypes.Order);
			return workflowSetting;
		}

		/// <summary>
		/// 定期ワークフロー設定を取得
		/// </summary>
		/// <param name="workflow">定期ワークフロー設定モデル</param>
		/// <returns>ワークフロー設定</returns>
		private WorkflowSetting GetWorkflowSetting(FixedPurchaseWorkflowSettingModel workflow)
		{
			var orderToSetting = new WorkflowSetting().GetOrderWorkflowSetting(
				workflow.ShopId,
				workflow.WorkflowKbn,
				workflow.WorkflowNo.ToString(),
				WorkflowSetting.m_KBN_WORKFLOW_TYPE_FIXEDPURCHASE)[0];

			var workflowSetting = new WorkflowSetting(
				orderToSetting,
				this.ScenarioExecResult.DeptId,
				this.ScenarioExecResult.LastChanged,
				WorkflowSetting.WorkflowTypes.FixedPurchase);
			return workflowSetting;
		}

		/// <summary>
		/// シナリオ実行用のSQLパラメータを取得（受注）
		/// </summary>
		/// <param name="workflow">受注ワークフロー設定モデル</param>
		/// <returns>SQLパラメータ</returns>
		private Hashtable GetWorkflowSearchParamOrderForScenarioExec(OrderWorkflowSettingModel workflow)
		{
			var condition = new OrderListConditionForWorkflow { IsSelectedByWorkflow = true };
			var sqlParam = new OrderSearchParam().GetSearchParamOrder(
				Constants.FLG_ORDERWORKFLOWSETTING_TARGET_WORKFLOW_TYPE_ORDER,
				workflow.ShopId,
				workflow.WorkflowKbn,
				workflow.WorkflowNo.Value.ToString(),
				condition,
				new WorkflowSetting(),
				true
			);
			// シナリオ実行の場合は表示件数を増やして全てアクションが実行されるようにする
			sqlParam[Constants.FIELD_ORDERWORKFLOWSETTING_DISPLAY_COUNT] = 1000000;
			return sqlParam;
		}

		/// <summary>
		/// シナリオ実行用のSQLパラメータを取得（定期）
		/// </summary>
		/// <param name="workflow">定期ワークフロー設定モデル</param>
		/// <returns>SQLパラメータ</returns>
		private Hashtable GetWorkflowSearchParamOrderForScenarioExec(FixedPurchaseWorkflowSettingModel workflow)
		{
			var condition = new FixedPurchaseConditionForWorkflow { IsSelectedByWorkflow = true };
			var sqlParam = new OrderSearchParam().GetSearchParamFixedPurchase(
				Constants.FLG_ORDERWORKFLOWSETTING_TARGET_WORKFLOW_TYPE_FIXED_PURCHASE,
				workflow.ShopId,
				workflow.WorkflowKbn,
				workflow.WorkflowNo.ToString(),
				condition,
				new WorkflowSetting(),
				true
			);
			// シナリオ実行の場合は表示件数を増やして全てアクションが実行されるようにする
			sqlParam[Constants.FIELD_ORDERWORKFLOWSETTING_DISPLAY_COUNT] = 1000000;
			return sqlParam;
		}

		/// <summary>
		/// スレッドストップチェック
		/// </summary>
		/// <param name="progress">進捗</param>
		private void CheckScheduleStoppedAndUpdateProgress(string progress)
		{
			var taskSchedule = new TaskScheduleService().GetTaskScheduleAndUpdateProgress(
				this.ScenarioExecResult.DeptId,
				this.ScenarioExecResult.ActionKbn,
				this.ScenarioExecResult.MasterId,
				this.ScenarioExecResult.ActionNo,
				progress);

			// 停止命令がなければ
			if ((taskSchedule == null) || (taskSchedule.StopFlg != Constants.FLG_TASKSCHEDULE_STOP_FLG_ON)) return;

			this.ScenarioExecResult.ExecEndDatetime = DateTime.Now;
			this.ScenarioExecResult.ScenarioExecStatus = Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_STOP;
			UpdateHoldHistoryToStopped();

			throw new WorkflowStopException
			{
				ScenarioExecResult = this.ScenarioExecResult,
			};
		}

		/// <summary>
		/// 実行中と保留中のワークフロー履歴を全て停止済みに変更
		/// </summary>
		private void UpdateHoldHistoryToStopped()
		{
			this.ScenarioExecResult.Histories = this.ScenarioExecResult.Histories.Select(history =>
			{
				if (history.ExecStatus == Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_RUNNING
					|| history.ExecStatus == Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_HOLD)
				{
					var updateHistory = UpdateOrderWorkflowExecHistory(
						history,
						GetHistoryEndAction(
							Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_STOPPED,
							(history.ExecStatus == Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_RUNNING)
								? CreateHistoryProgress(
									this.ScenarioExecResult.ActionExecSuccessfulCountOfRunningWorkflow,
									this.ScenarioExecResult.ActionCountBeExecOfRunningWorkflow)
								: CreateHistoryProgress(
									0,
									0),
							Constants.FLG_ORDERWORKFLOWEXECHISTORY_MESSAGE_FOR_WORKFLOW_STOP,
							(history.ExecStatus == Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_RUNNING)
								? DateTime.Now
								: (DateTime?)null));
					return updateHistory;
				}
				return history;
			}).ToList();
		}

		/// <summary>
		/// 受注ワークフローの実行履歴を保留中で登録する
		/// </summary>
		/// <param name="shopId">ショップID</param>
		/// <param name="workflowKbn">ワークフロー区分</param>
		/// <param name="workflowNo">ワークフローNo</param>
		/// <param name="workflowName">ワークフロー名</param>
		/// <param name="workflowType">ワークフロー種別</param>
		/// <returns>受注ワークフロー実行履歴</returns>
		private OrderWorkflowExecHistoryModel InsertOrderWorkflowExecHistory(
			string shopId,
			string workflowKbn,
			int workflowNo,
			string workflowName,
			string workflowType)
		{
			var history = new OrderWorkflowExecHistoryService().Insert(
				new OrderWorkflowExecHistoryModel
				{
					ShopId = shopId,
					WorkflowKbn = workflowKbn,
					WorkflowNo = workflowNo,
					ScenarioSettingId = this.ScenarioExecResult.Scenario.ScenarioSettingId,
					WorkflowName = workflowName,
					ScenarioName = this.ScenarioExecResult.Scenario.ScenarioName,
					ExecStatus = Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_STATUS_HOLD,
					WorkflowType = workflowType == Constants.FLG_ORDERWORKFLOWSCENARIOSETTING_TARGETKBN_NORMAL
						? Constants.FLG_ORDERWORKFLOWEXECHISTORY_WORKFLOW_TYPE_ORDER_WORKFLOW
						: Constants.FLG_ORDERWORKFLOWEXECHISTORY_WORKFLOW_TYPE_FIXED_PURCHASE_WORKFLOW,
					ExecPlace = Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_PLACE_SCENARIO,
					ExecTiming = this.ScenarioExecResult.ExecTiming,
					Message = Constants.FLG_ORDERWORKFLOWEXECHISTORY_MESSAGE_FOR_WORKFLOW_HOLD,
					LastChanged = this.ScenarioExecResult.LastChanged,
					DateBegin = DateTime.Now,
					DateEnd = DateTime.Now,
				});

			return history;
		}

		/// <summary>
		/// 受注ワークフローの実行履歴を更新する
		/// </summary>
		/// <param name="history">ワークフロー実行履歴</param>
		/// <param name="updateAction">アップデートアクション</param>
		/// <returns>受注ワークフロー実行履歴</returns>
		private OrderWorkflowExecHistoryModel UpdateOrderWorkflowExecHistory(
			OrderWorkflowExecHistoryModel history,
			Action<OrderWorkflowExecHistoryModel> updateAction)
		{
			var historyId = history.OrderWorkflowExecHistoryId;
			var updatedHistory = new OrderWorkflowExecHistoryService().ModifyAndGetUpdatedRecords(
				historyId,
				updateAction);

			return updatedHistory;
		}

		/// <summary>
		/// 履歴の開始アクションを取得
		/// </summary>
		/// <param name="execStatus">実行ステータス</param>>
		/// <param name="message">メッセージ</param>
		/// <param name="dateBegin">開始時間</param>
		/// <returns>履歴の開始アクション</returns>
		private Action<OrderWorkflowExecHistoryModel> GetHistoryStartAction(
			string execStatus,
			string message,
			DateTime dateBegin)
		{
			var startAction = new Action<OrderWorkflowExecHistoryModel>(
				action =>
				{
					action.ExecStatus = execStatus;
					action.Message = message;
					action.DateBegin = dateBegin;
				});

			return startAction;
		}

		/// <summary>
		/// 履歴の終了アクションを取得
		/// </summary>
		/// <param name="execStatus">実行ステータス</param>
		/// <param name="successRate">成功率</param>
		/// <param name="message">メッセージ</param>
		/// <param name="dateEnd">履歴の終了時間</param>
		/// <returns>履歴の終了アクション</returns>
		private Action<OrderWorkflowExecHistoryModel> GetHistoryEndAction(
			string execStatus,
			string successRate,
			string message,
			DateTime? dateEnd)
		{
			var startAction = new Action<OrderWorkflowExecHistoryModel>(
				action =>
				{
					action.ExecStatus = execStatus;
					action.SuccessRate = successRate;
					action.Message = message;
					action.DateEnd = dateEnd;
				});

			return startAction;
		}

		/// <summary>
		/// 履歴テーブルに保存する進捗を取得
		/// </summary>
		/// <param name="successfulCount">成功件数</param>
		/// <param name="countBeExec">実行する件数</param>
		/// <returns>履歴テーブルに保存する進捗</returns>
		private string CreateHistoryProgress(int successfulCount, int countBeExec)
		{
			var progress = string.Format(
				"{0}/{1}",
				StringUtility.ToNumeric(successfulCount),
				StringUtility.ToNumeric(countBeExec));
			return progress;
		}

		/// <summary>
		/// タスクスケジューラに保存する進捗を取得
		/// </summary>
		/// <returns>タスクスケジューラに保存する進捗</returns>
		private string CreateTaskSchedulerProgress()
		{
			var progress = string.Format(
				"{0}/{1}\r\n{2}/{3}",
				this.ScenarioExecResult.WorkflowExecSuccessfulCount,
				this.ScenarioExecResult.WorkflowCountBeExec,
				StringUtility.ToNumeric(this.ScenarioExecResult.ActionExecCountOfRunningWorkflow),
				StringUtility.ToNumeric(this.ScenarioExecResult.ActionCountBeExecOfRunningWorkflow));
			return progress;
		}

		/// <summary>

		/// アクションする受注を取得
		/// </summary>
		/// <param name="orderOld">DataViewの受注</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateDate">更新日</param>
		/// <param name="updateScheduledShippingDate">発送予定日</param>
		/// <param name="updateNextShippingDate">次回定期発送日</param>
		/// <param name="updateNextNextShippingDate">次々回定期発送日</param>
		/// <param name="updateShippingDate">発送日</param>
		/// <returns>受注</returns>
		private Hashtable GetAnOrderToAction(
			DataView orderOld,
			string lastChanged,
			string updateDate,
			string updateScheduledShippingDate,
			string updateNextShippingDate,
			string updateNextNextShippingDate = "",
			string updateShippingDate = "")
		{
			var orderForUpdate = new Hashtable
			{
				{ Constants.FIELD_ORDER_ORDER_ID, (string)orderOld[0][Constants.FIELD_ORDER_ORDER_ID] },
				{ Constants.FIELD_ORDER_USER_ID, (string)orderOld[0][Constants.FIELD_ORDER_USER_ID] },
				{ Constants.FIELD_ORDER_SHOP_ID, (string)orderOld[0][Constants.FIELD_ORDER_SHOP_ID] },
				{ Constants.FIELD_ORDER_ORDER_PRICE_TOTAL,(decimal)orderOld[0][Constants.FIELD_ORDER_ORDER_PRICE_TOTAL] },
				{ Constants.FIELD_ORDER_LAST_BILLED_AMOUNT, (decimal)orderOld[0][Constants.FIELD_ORDER_LAST_BILLED_AMOUNT] },
				{ Constants.FIELD_ORDER_CARD_TRAN_ID, (string)orderOld[0][Constants.FIELD_ORDER_CARD_TRAN_ID] },
				{ Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO, (string)orderOld[0][Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO] },
				{ Constants.FIELD_ORDER_DATE_CREATED, orderOld[0][Constants.FIELD_ORDER_DATE_CREATED] },
				{ Constants.FIELD_ORDER_LAST_CHANGED, lastChanged },
				{ Constants.FIELD_ORDER_ORDER_STATUS, (string)orderOld[0][Constants.FIELD_ORDER_ORDER_STATUS] },
				{ Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS, (string)orderOld[0][Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS] },
				{ "update_date", updateDate },
				{ Constants.FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE, updateScheduledShippingDate },
				{ Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_DATE, updateNextShippingDate },
				{ Constants.FIELD_FIXEDPURCHASE_NEXT_NEXT_SHIPPING_DATE, updateNextNextShippingDate },
				{ Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE, updateShippingDate }
			};
			return orderForUpdate;
		}

		/// <summary>
		/// 結果エラーメッセージ作成
		/// </summary>
		/// <param name="results">アクション結果リスト</param>
		/// <returns>結果エラーメッセージ</returns>
		private string CreateErrorResultMessage(List<ActionResultUnit> results)
		{
			if (results.Any(r => r.HasError) == false) return string.Empty;

			var errorData = string.Join(
				"\r\n",
				results.Where(r => r.HasError)
					.Select(unit => string.Format("\t{0}（※{1}）", unit.OrderId, string.Join("、", unit.ErrorMessages)))
					.ToArray());
			var errorMsg = string.Format("失敗の対象ID：\r\n{0}",
				errorData);
			return errorMsg;
		}

		/// <summary>シナリオ実行リザルト</summary>
		private ScenarioExecResult ScenarioExecResult { get; set; }
		/// <summary>ネクストエンジン受注情報配列</summary>
		protected NEOrder[] UpdateNextEngineOrders { get; set; }
	}
}
