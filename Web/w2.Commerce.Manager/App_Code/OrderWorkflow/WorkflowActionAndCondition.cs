/*
=========================================================================================================
  Module      : Workflow Action And Condition (WorkflowActionAndCondition.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using w2.App.Common.DataCacheController;
using w2.App.Common.Global;
using w2.App.Common.Global.Config;
using w2.App.Common.Order;
using w2.App.Common.Order.Workflow;
using w2.App.Common.OrderExtend;
using w2.App.Common.Util;
using w2.Domain;

/// <summary>
/// Workflow Action And Condition
/// </summary>
public class WorkflowActionAndCondition
{
	/// <summary>Value text: Search condition title</summary>
	private const string VALUETEXT_FIELD_SEARCH_CONDITION_TITLE = "search_condition_title";
	/// <summary>Value text: Search condition format</summary>
	private const string VALUETEXT_FIELD_SEARCH_CONDITIONFORMAT = "search_condition_format";
	/// <summary>Value text: Fixed purchase</summary>
	private const string VALUETEXT_FIELD_FIXEDPURCHASE = "fixedpurchase";
	/// <summary>Value text: Field order extend status search</summary>
	private const string VALUETEXT_FIELD_ORDER_EXTEND_STATUS_SEARCH = "order_extend_status_search";
	/// <summary>Value text: Field actions title</summary>
	private const string VALUETEXT_FIELD_ACTIONS_TITLE = "actions_title";
	/// <summary>Value text: Field order extend status change</summary>
	private const string VALUETEXT_FIELD_ORDER_EXTEND_STATUS_CHANGE = "order_extend_status_change";
	/// <summary>Value text: Field date format</summary>
	private const string VALUETEXT_FIELD_DATE_FORMAT = "date_format";
	/// <summary>Value text: Field next shipping date format</summary>
	private const string VALUETEXT_FIELD_NEXT_SHIPPING_DATE_FORMAT = "next_shipping_date_format";
	/// <summary>Value text: Fixed purchase count</summary>
	private const string VALUETEXT_FIXEDPURCHASE_COUNT = "fixedpurchase_count";
	/// <summary>Value text: specified unspecified</summary>
	private const string VALUETEXT_SPECIFIED_UNSPECIFIED = "specified_unspecified";
	/// <summary>Value text: update status format</summary>
	private const string VALUETEXT_UPDATE_STATUS_FORMAT = "update_status_format";
	/// <summary>Value text: update status specified format</summary>
	private const string VALUETEXT_UPDATE_STATUS_SPECIFIED_FORMAT = "update_status_specified_format";
	/// <summary>Value text: return exchange update status format</summary>
	private const string VALUETEXT_RETURN_EXCHANGE_UPDATE_STATUS_FORMAT = "return_exchange_update_status_format";
	/// <summary>Value text: return exchange update status specified format</summary>
	private const string VALUETEXT_RETURN_EXCHANGE_UPDATE_STATUS_SPECIFIED_FORMAT = "return_exchange_update_status_specified_format";
	/// <summary>Value text: display order extend status format</summary>
	private const string VALUETEXT_DISPLAY_ORDER_EXTEND_STATUS_FORMAT = "display_order_extend_status_format";
	/// <summary>Value text: display order extend format</summary>
	private const string VALUETEXT_DISPLAY_ORDER_EXTEND_FORMAT = "display_order_extend_format";
	/// <summary>Cassette text</summary>
	private const string CASSETTE_TEXT = "cassette_";
	/// <summary>Extend status change</summary>
	private const string EXTEND_STATUS_CHANGE = "extend_status_change";
	/// <summary>返品交換：決済種別</summary>
	private const string ORDER_RETURN_PAYMENT_KBN = OrderWorkflowPage.ORDER_RETURN_PAYMENT_KBN;
	/// <summary>Sort line acions</summary>
	private static readonly List<string> SORT_LINE_ACTIONS = new List<string>
	{
		Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_STATUS_CHANGE,
		Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_ACTION,
		Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_REASON_KBN,
		Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_REASON_MEMO,
		Constants.FIELD_ORDERWORKFLOWSETTING_SHIPPING_DATE_ACTION,
		Constants.FIELD_ORDERWORKFLOWSETTING_SCHEDULED_SHIPPING_DATE_ACTION,
		Constants.FIELD_ORDERWORKFLOWSETTING_PRODUCT_REALSTOCK_CHANGE,
		Constants.FIELD_ORDERWORKFLOWSETTING_PAYMENT_STATUS_CHANGE,
		Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION,
		Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION,
		Constants.FIELD_ORDERWORKFLOWSETTING_DEMAND_STATUS_CHANGE,
		Constants.FIELD_ORDERWORKFLOWSETTING_RECEIPT_OUTPUT_FLG_CHANGE,
		Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_IS_ALIVE_CHANGE,
		Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CANCEL_REASON_ID,
		Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CANCEL_MEMO,
		Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_PAYMENT_STATUS_CHANGE,
		Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_NEXT_SHIPPING_DATE_CHANGE,
		Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_NEXT_NEXT_SHIPPING_DATE_CHANGE,
		Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_STOP_UNAVAILABLE_SHIPPING_AREA_CHANGE,
		Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_EXCHANGE_STATUS_CHANGE,
		Constants.FIELD_ORDERWORKFLOWSETTING_REPAYMENT_STATUS_CHANGE,
		Constants.FIELD_ORDERWORKFLOWSETTING_INVOICE_STATUS_API,
		Constants.FIELD_ORDERWORKFLOWSETTING_INVOICE_STATUS_CHANGE,
		Constants.FIELD_ORDERWORKFLOWSETTING_MAIL_ID
	};
	/// <summary>Sort cassette actions</summary>
	private static readonly List<string> SORT_CASSETTE_ACTIONS = new List<string>
	{
		Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_NO_UPDATE,
		Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_IS_ALIVE_CHANGE,
		Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CANCEL_REASON_ID,
		Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CANCEL_MEMO,
		Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_PAYMENT_STATUS_CHANGE,
		Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_STATUS_CHANGE,
		Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_RETURN_ACTION,
		Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_REASON_KBN,
		Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_REASON_MEMO,
		Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_PRODUCT_REALSTOCK_CHANGE,
		Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_PAYMENT_STATUS_CHANGE,
		Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_EXTERNAL_PAYMENT_ACTION,
		Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_STOP_UNAVAILABLE_SHIPPING_AREA_CHANGE,
		Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_EXTERNAL_ORDER_INFO_ACTION,
		Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_DEMAND_STATUS_CHANGE,
		Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_RECEIPT_OUTPUT_FLG_CHANGE,
		Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_RETURN_EXCHANGE_STATUS_CHANGE,
		Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_REPAYMENT_STATUS_CHANGE,
		Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_INVOICE_STATUS_API,
		Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_INVOICE_STATUS_CHANGE,
		Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_STOREPICKUP_STATUS_CHANGE
	};

	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="loginShopOperatorShopId">Login shop operator shop id</param>
	public WorkflowActionAndCondition(string loginShopOperatorShopId)
	{
		this.LoginShopOperatorShopId = loginShopOperatorShopId;
	}

	/// <summary>
	/// Get display workflow action and conditions
	/// </summary>
	/// <param name="workflowKbn">Workflow kbn</param>
	/// <param name="workflowNo">Workflow no</param>
	/// <param name="workflowType">Workflow type</param>
	/// <returns>Two results: actions and conditions</returns>
	public Tuple<KeyValueItem[], KeyValueItem[]> GetDisplayWorkflowActionAndConditions(
		string workflowKbn,
		string workflowNo,
		string workflowType)
	{
		var actions = new List<KeyValueItem>();
		var conditions = new List<KeyValueItem>();
		if (workflowType == WorkflowSetting.m_KBN_WORKFLOW_TYPE_ORDER)
		{
			var setting = DomainFacade.Instance.OrderWorkflowSettingService.GetOrderWorkflowSetting(
				workflowKbn,
				workflowNo);
			if (setting != null)
			{
				var hasReason = (setting.DisplayKbn == Constants.FLG_ORDERWORKFLOWSETTING_DISPLAY_KBN_LINE)
					? (setting.WorkflowReturnAction == Constants.FLG_ORDERWORKFLOWSETTING_RETURN_ACTION_ACCEPT_RETURN)
					: HasReason(setting.WorkflowCassetteReturnAction.Split(','));
				// Create actions
				actions.AddRange(GetActions(
					setting.DataSource,
					setting.DisplayKbn,
					setting.WorkflowDetailKbn,
					workflowType,
					hasReason));

				// Create conditions
				conditions.AddRange(GetSearchConditions(
					setting.SearchSetting,
					workflowType,
					setting.WorkflowDetailKbn,
					setting.WorkflowKbn));
			}
		}
		else
		{
			var setting = DomainFacade.Instance.FixedPurchaseWorkflowSettingService.GetFixedPurchaseWorkflowSetting(
				workflowKbn,
				workflowNo);
			if (setting != null)
			{
				var hasReason = (setting.DisplayKbn == Constants.FLG_ORDERWORKFLOWSETTING_DISPLAY_KBN_LINE)
					? (setting.FixedPurchaseIsAliveChange == Constants.FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_IS_ALIVE_CHANGE_ACTION_CANCEL)
					: HasReason(setting.CassetteFixedPurchaseIsAliveChange.Split(','));

				// Create actions
				actions.AddRange(GetActions(
					setting.DataSource,
					setting.DisplayKbn,
					setting.WorkflowDetailKbn,
					workflowType,
					hasReason));

				// Create conditions
				conditions.AddRange(GetSearchConditions(
					setting.SearchSetting,
					workflowType,
					setting.WorkflowDetailKbn,
					setting.WorkflowKbn));
			}
		}
		var results = new Tuple<KeyValueItem[], KeyValueItem[]>(actions.ToArray(), conditions.ToArray());
		return results;
	}

	/// <summary>
	/// Get actions for workflow execution
	/// </summary>
	/// <param name="workflowKbn">Workflow kbn</param>
	/// <param name="workflowNo">Workflow no</param>
	/// <param name="workflowType">Workflow type</param>
	/// <returns>Two results: actions for execution and actions for confirm (display)</returns>
	public Tuple<KeyValueItem[], KeyValueItem[]> GetActionsForWorkflowExecution(
		string workflowKbn,
		string workflowNo,
		string workflowType)
	{
		var displayKbn = string.Empty;
		var displayDetailKbn = string.Empty;
		var hasReason = false;
		var workflowSetting = new Hashtable();
		var actions = new List<KeyValueItem>();
		var actionsForConfirm = new List<KeyValueItem>();
		var results = new Tuple<KeyValueItem[], KeyValueItem[]>(actions.ToArray(), actionsForConfirm.ToArray());
		if (workflowType == WorkflowSetting.m_KBN_WORKFLOW_TYPE_ORDER)
		{
			var orderWorkflowSetting = DomainFacade.Instance.OrderWorkflowSettingService.GetOrderWorkflowSetting(
				workflowKbn,
				workflowNo);
			if (orderWorkflowSetting == null) return results;

			workflowSetting = orderWorkflowSetting.DataSource;
			displayKbn = orderWorkflowSetting.DisplayKbn;
			hasReason = (displayKbn == Constants.FLG_ORDERWORKFLOWSETTING_DISPLAY_KBN_LINE)
				? (orderWorkflowSetting.WorkflowReturnAction
					== Constants.FLG_ORDERWORKFLOWSETTING_RETURN_ACTION_ACCEPT_RETURN)
				: HasReason(orderWorkflowSetting.WorkflowCassetteReturnAction.Split(','));
			displayDetailKbn = orderWorkflowSetting.WorkflowDetailKbn;
		}
		else
		{
			var fixedPurchaseWorkflowSetting = DomainFacade.Instance.FixedPurchaseWorkflowSettingService.GetFixedPurchaseWorkflowSetting(
				workflowKbn,
				workflowNo);
			if (fixedPurchaseWorkflowSetting == null) return results;

			workflowSetting = fixedPurchaseWorkflowSetting.DataSource;
			displayKbn = fixedPurchaseWorkflowSetting.DisplayKbn;
			hasReason = (displayKbn == Constants.FLG_ORDERWORKFLOWSETTING_DISPLAY_KBN_LINE)
				? (fixedPurchaseWorkflowSetting.FixedPurchaseIsAliveChange
					== Constants.FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_IS_ALIVE_CHANGE_ACTION_CANCEL)
				: HasReason(fixedPurchaseWorkflowSetting.CassetteFixedPurchaseIsAliveChange.Split(','));
			displayDetailKbn = fixedPurchaseWorkflowSetting.WorkflowDetailKbn;
		}

		// Add actions for execution
		foreach (var action in GetActions(workflowSetting, displayKbn, displayDetailKbn, workflowType, hasReason, false))
		{
			var index = 0;
			var actionValues = action.Value.Split('|')[0].Split(',');
			var actionValuesTrans = action.Value.Split('|')[1].Split(',');
			foreach (var actionValue in actionValues)
			{
				if (CheckDisplayActions(action.Key) == false) continue;

				var itemData = new KeyValueItem();
				itemData.Key = string.Format("{0}&{1}", action.Key, actionValue);
				if (action.Key.Contains(EXTEND_STATUS_CHANGE))
				{
					var transKey = GetDisplayKeys(action.Key, VALUETEXT_FIELD_ACTIONS_TITLE);
					itemData.Value = string.Format(
						"{0} ({1})",
						actionValuesTrans[index].Split('&')[0],
						transKey);
				}
				else
				{
					itemData.Value = actionValuesTrans[index].Split('&')[0];
				}

				if (action.Key.Contains(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_DEFAULT_SELECT))
				{
					actions.Insert(0, itemData);
				}
				else
				{
					actions.Add(itemData);
				}
				index++;
			}
		}

		// Add actions for confirm (display)
		actionsForConfirm.AddRange(GetActions(
			workflowSetting,
			displayKbn,
			displayDetailKbn,
			workflowType,
			hasReason));

		results = new Tuple<KeyValueItem[], KeyValueItem[]>(actions.ToArray(), actionsForConfirm.ToArray());
		return results;
	}

	/// <summary>
	/// Get actions
	/// </summary>
	/// <param name="workflowKbn">A workflow kbn</param>
	/// <param name="workflowNo">A workflow no</param>
	/// <param name="workflowType">A workflow type</param>
	/// <param name="isTranslate">Has base value flag</param>
	/// <returns>A collection of actions</returns>
	private KeyValueItem[] GetActions(
		Hashtable workflowSetting,
		string displayKbn,
		string displayDetailKbn,
		string workflowType,
		bool hasReason,
		bool isTranslate = true)
	{
		var functions = GetConvertActionFunctions(
			displayKbn,
			hasReason,
			StringUtility.ToEmpty(workflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_NO_UPDATE]),
			displayDetailKbn,
			StringUtility.ToEmpty(workflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_KBN]),
			workflowType);

		var sortList = GetSortList(displayKbn);

		var results = new List<KeyValueItem>();
		var settings = workflowSetting.Cast<DictionaryEntry>()
			.Where(setting => ((string.IsNullOrEmpty(StringUtility.ToEmpty(setting.Value)) == false)
				&& functions.ContainsKey(StringUtility.ToEmpty(setting.Key))
				&& IsDisplaySetting(setting)))
			.OrderBy(setting => sortList.IndexOf(StringUtility.ToEmpty(setting.Key)))
			.ToArray();
		foreach (var setting in settings)
		{
			if (isTranslate)
			{
				var displayKey = GetDisplayKeys(StringUtility.ToEmpty(setting.Key), VALUETEXT_FIELD_ACTIONS_TITLE);
				if (string.IsNullOrEmpty(displayKey)) continue;

				results.Add(new KeyValueItem
				{
					Key = displayKey,
					Value = functions[StringUtility.ToEmpty(setting.Key)].Invoke(StringUtility.ToEmpty(setting.Value))
				});

				continue;
			}

			results.Add(new KeyValueItem
			{
				Key = StringUtility.ToEmpty(setting.Key),
				Value = string.Format(
					"{0}|{1}",
					setting.Value,
					functions[StringUtility.ToEmpty(setting.Key)].Invoke(StringUtility.ToEmpty(setting.Value)))
			});
		}
		return results.ToArray();
	}

	/// <summary>
	/// Get convert action condition functions
	/// </summary>
	/// <param name="displayKbn">Display type</param>
	/// <param name="hasReason">Has display reason</param>
	/// <param name="noUpdate">Update value</param>
	/// <param name="displayDetailKbn">Display details value</param>
	/// <param name="workflowKbn">Workflow kbn</param>
	/// <param name="workflowType">Workflow type</param>
	/// <returns>A dictionary of convert action condition functions</returns>
	private IDictionary<string, Func<string, string>> GetConvertActionFunctions(
		string displayKbn,
		bool hasReason,
		string noUpdate,
		string displayDetailKbn,
		string workflowKbn,
		string workflowType)
	{
		var functions = new Dictionary<string, Func<string, string>>();

		if (displayKbn == Constants.FLG_ORDERWORKFLOWSETTING_DISPLAY_KBN_LINE)
		{
			SetLineConvertedActionFuntions(
				displayDetailKbn,
				functions,
				workflowKbn,
				workflowType,
				hasReason);
		}
		else if (displayKbn == Constants.FLG_ORDERWORKFLOWSETTING_DISPLAY_KBN_CASSETTE)
		{
			SetCassetteConvertedActionFuntions(
				noUpdate,
				functions,
				workflowKbn,
				workflowType,
				hasReason,
				displayDetailKbn);
		}
		return functions;
	}

	/// <summary>
	/// Set cassette converted action functions
	/// </summary>
	/// <param name="noUpdate">No update</param>
	/// <param name="functions">The functions as dictionary</param>
	/// <param name="workflowKbn">Workflow kbn</param>
	/// <param name="workflowType">Workflow type</param>
	/// <param name="hasReason">If has setting have reason field</param>
	/// <param name="displayDetailKbn">Display detail kbn</param>
	private void SetCassetteConvertedActionFuntions(
		string noUpdate,
		Dictionary<string, Func<string, string>> functions,
		string workflowKbn,
		string workflowType,
		bool hasReason,
		string displayDetailKbn)
	{
		if (noUpdate == Constants.FLG_ORDERWORKFLOWSETTING_CASSETTE_NO_UPDATE_ON)
		{
			// 共通
			functions.Add(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_NO_UPDATE,
				(value) =>
					ValueText.GetValueText(
						Constants.TABLE_ORDERWORKFLOWSETTING,
						Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_NO_UPDATE,
						value));
		}

		switch (workflowKbn)
		{
			case Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_KBN_RETURN_EXCHANGE:
				// 返品交換ステータス
				functions.Add(
					Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_RETURN_EXCHANGE_STATUS_CHANGE,
					(value) =>
						GetDisplayCassetteValues(
							Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_RETURN_EXCHANGE_STATUS_CHANGE,
							value.Split(',')));

				// 返金ステータス
				functions.Add(
					Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_REPAYMENT_STATUS_CHANGE,
					(value) =>
						GetDisplayCassetteValues(
							Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_REPAYMENT_STATUS_CHANGE,
							value.Split(',')));
				break;

			case Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_KBN_DAY:
			case Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_KBN_MONTH:
			case Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_KBN_YEAR:
			case Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_KBN_OTHER:
				// Get extned status
				var getExtnedStatus = new Func<string, string>(
					(value) =>
						GetDisplayCassetteValues(VALUETEXT_FIELD_ORDER_EXTEND_STATUS_CHANGE, value.Split(',')));

				// For case workflow type is other
				if (workflowType == WorkflowSetting.m_KBN_WORKFLOW_TYPE_ORDER)
				{
					switch (displayDetailKbn)
					{
						case Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN_RETURN:
							// 返品
							functions.Add(
								Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_RETURN_ACTION,
								(value) =>
									GetDisplayCassetteValues(
										Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_RETURN_ACTION,
										value.Split(',')));

							if (hasReason)
							{
								// 返品区分
								functions.Add(
									Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_REASON_KBN,
									(value) =>
										ValueText.GetValueText(
											Constants.TABLE_ORDERWORKFLOWSETTING,
											Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_REASON_KBN,
											value));

								// 返品理由
								functions.Add(
									Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_REASON_MEMO,
									(value) => value.ToString());
							}
							break;

						case Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN_NORMAL:
							// 注文ステータス更新区分
							functions.Add(
								Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_STATUS_CHANGE,
								(value) =>
									GetDisplayCassetteValues(
										Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_STATUS_CHANGE,
										value.Split(',')));

							// 実在庫連動処理
							functions.Add(
								Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_PRODUCT_REALSTOCK_CHANGE,
								(value) =>
									GetDisplayCassetteValues(
										Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_PRODUCT_REALSTOCK_CHANGE,
										value.Split(',')));

							// 入金ステータス
							functions.Add(
								Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_PAYMENT_STATUS_CHANGE,
								(value) =>
									GetDisplayCassetteValues(
										Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_PAYMENT_STATUS_CHANGE,
										value.Split(',')));

							// 外部決済連携
							functions.Add(
								Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_EXTERNAL_PAYMENT_ACTION,
								(value) =>
									GetDisplayCassetteValues(
										Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_EXTERNAL_PAYMENT_ACTION,
										value.Split(',')));

							// 外部受注情報連携
							functions.Add(
								Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_EXTERNAL_ORDER_INFO_ACTION,
								(value) =>
									GetDisplayCassetteValues(
										Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_EXTERNAL_ORDER_INFO_ACTION,
										value.Split(',')));

							if (Constants.TWINVOICE_ENABLED)
							{
								// 電子発票連携
								functions.Add(
									Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_INVOICE_STATUS_API,
									(value) =>
										GetDisplayCassetteValues(
											Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_INVOICE_STATUS_API,
											value.Split(',')));
							}

							if (OrderCommon.DisplayTwInvoiceInfo())
							{
								// 発票ステータス
								functions.Add(
									Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_INVOICE_STATUS_CHANGE,
									(value) =>
										GetDisplayCassetteValues(
											Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_INVOICE_STATUS_CHANGE,
											value.Split(',')));
							}
							break;
					}

					// 拡張ステータス
					for (var index = 0; index < Constants.CONST_ORDER_EXTEND_STATUS_DBFIELDS_MAX; index++)
					{
						functions.Add(
							WorkflowSetting.m_FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE[index],
							getExtnedStatus);
					}

					// 督促ステータス
					functions.Add(
						Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_DEMAND_STATUS_CHANGE,
						(value) =>
							GetDisplayCassetteValues(
								Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_DEMAND_STATUS_CHANGE,
								value.Split(',')));

					if (Constants.RECEIPT_OPTION_ENABLED)
					{
						// 領収書出力フラグ
						functions.Add(
							Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_RECEIPT_OUTPUT_FLG_CHANGE,
							(value) =>
								GetDisplayCassetteValues(
									Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_RECEIPT_OUTPUT_FLG_CHANGE,
									value.Split(',')));
					}
					if (Constants.STORE_PICKUP_OPTION_ENABLED)
					{
						// Cassete order store pickup status change
						functions.Add(
							Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_STOREPICKUP_STATUS_CHANGE,
							(value) =>
								GetDisplayCassetteValues(
									Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_STOREPICKUP_STATUS_CHANGE,
									value.Split(',')));
					}
				}
				// For case workflow type is fixed purchase
				else
				{
					// 定期購入状態変更
					functions.Add(
						Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_IS_ALIVE_CHANGE,
						(value) =>
							GetDisplayCassetteValues(
								Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_IS_ALIVE_CHANGE,
								value.Split(',')));

					// 決済ステータス変更区分
					functions.Add(
						Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_PAYMENT_STATUS_CHANGE,
						(value) =>
							GetDisplayCassetteValues(
								Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_PAYMENT_STATUS_CHANGE,
								value.Split(',')));

					// 配送不可エリア停止変更区分
					functions.Add(
						Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_STOP_UNAVAILABLE_SHIPPING_AREA_CHANGE,
						(value) =>
							GetDisplayCassetteValues(
								Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_STOP_UNAVAILABLE_SHIPPING_AREA_CHANGE,
								value.Split(',')));

					if (hasReason)
					{
						// 解約理由
						functions.Add(
							Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CANCEL_REASON_ID,
							(value) =>
							{
								if (hasReason == false) return string.Empty;

								var fixedPurchaseCancelReason = DomainFacade.Instance.FixedPurchaseService.GetCancelReason(value);
								if (fixedPurchaseCancelReason == null) return string.Empty;
								return fixedPurchaseCancelReason.CancelReasonName;
							});

						// 解約メモ
						functions.Add(
							Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CANCEL_MEMO,
							(value) => value);
					}

					// 拡張ステータス
					for (var index = 0; index < Constants.CONST_ORDER_EXTEND_STATUS_DBFIELDS_MAX; index++)
					{
						functions.Add(
							WorkflowSetting.m_FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE[index],
							getExtnedStatus);
					}
				}
				break;
		}

		// Cassette default
		functions.Add(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_DEFAULT_SELECT,
			(value) => GetDisplayCassetteDefaultValues(value));
	}

	/// <summary>
	/// Set line converted action functions
	/// </summary>
	/// <param name="displayDetailKbn">Display details kbn</param>
	/// <param name="functions">The functions as dictionary</param>
	/// <param name="workflowKbn">Workflow kbn</param>
	/// <param name="workflowType">Workflow type</param>
	/// <param name="hasReason">If has setting have reason field</param>
	private void SetLineConvertedActionFuntions(
		string displayDetailKbn,
		Dictionary<string, Func<string, string>> functions,
		string workflowKbn,
		string workflowType,
		bool hasReason)
	{
		switch (workflowKbn)
		{
			case Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_KBN_DAY:
			case Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_KBN_MONTH:
			case Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_KBN_YEAR:
			case Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_KBN_OTHER:
				// Get extned status
				var getExtnedStatus = new Func<string, string>(
					(value) =>
						ValueText.GetValueText(
							Constants.TABLE_ORDERWORKFLOWSETTING,
							VALUETEXT_FIELD_ORDER_EXTEND_STATUS_CHANGE,
							value));

				// For case workflow type is order 
				if (workflowType == WorkflowSetting.m_KBN_WORKFLOW_TYPE_ORDER)
				{
					switch (displayDetailKbn)
					{
						case Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN_NORMAL:
							// 注文ステータス
							functions.Add(
								Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_STATUS_CHANGE,
								(value) =>
									ValueText.GetValueText(
										Constants.TABLE_ORDERWORKFLOWSETTING,
										Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_STATUS_CHANGE,
										value));

							// 配送希望日
							functions.Add(
								Constants.FIELD_ORDERWORKFLOWSETTING_SHIPPING_DATE_ACTION,
								(value) =>
									ValueText.GetValueText(
										Constants.TABLE_ORDERWORKFLOWSETTING,
										Constants.FIELD_ORDERWORKFLOWSETTING_SHIPPING_DATE_ACTION,
										value));

							if (GlobalConfigUtil.UseLeadTime())
							{
								// 出荷予定日
								functions.Add(
									Constants.FIELD_ORDERWORKFLOWSETTING_SCHEDULED_SHIPPING_DATE_ACTION,
									(value) =>
										ValueText.GetValueText(
											Constants.TABLE_ORDERWORKFLOWSETTING,
											Constants.FIELD_ORDERWORKFLOWSETTING_SCHEDULED_SHIPPING_DATE_ACTION,
											value));
							}

							// 実在庫連動処理
							functions.Add(
								Constants.FIELD_ORDERWORKFLOWSETTING_PRODUCT_REALSTOCK_CHANGE,
								(value) =>
									ValueText.GetValueText(
										Constants.TABLE_ORDERWORKFLOWSETTING,
										Constants.FIELD_ORDERWORKFLOWSETTING_PRODUCT_REALSTOCK_CHANGE,
										value));

							// 入金ステータス
							functions.Add(
								Constants.FIELD_ORDERWORKFLOWSETTING_PAYMENT_STATUS_CHANGE,
								(value) =>
									ValueText.GetValueText(
										Constants.TABLE_ORDERWORKFLOWSETTING,
										Constants.FIELD_ORDERWORKFLOWSETTING_PAYMENT_STATUS_CHANGE,
										value));

							// 外部決済連携
							functions.Add(
								Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION,
								(value) =>
									ValueText.GetValueText(
										Constants.TABLE_ORDERWORKFLOWSETTING,
										Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION,
										value));

							// 外部受注情報連携
							functions.Add(
								Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION,
								(value) =>
									ValueText.GetValueText(
										Constants.TABLE_ORDERWORKFLOWSETTING,
										Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION,
										value));

							if (Constants.TWINVOICE_ENABLED)
							{
								// 電子発票連携
								functions.Add(
									Constants.FIELD_ORDERWORKFLOWSETTING_INVOICE_STATUS_API,
									(value) =>
										ValueText.GetValueText(
											Constants.TABLE_ORDERWORKFLOWSETTING,
											Constants.FIELD_ORDERWORKFLOWSETTING_INVOICE_STATUS_API,
											value));
							}

							if (OrderCommon.DisplayTwInvoiceInfo())
							{
								// 発票ステータス
								functions.Add(
									Constants.FIELD_ORDERWORKFLOWSETTING_INVOICE_STATUS_CHANGE,
									(value) =>
										ValueText.GetValueText(
											Constants.TABLE_ORDERWORKFLOWSETTING,
											Constants.FIELD_ORDERWORKFLOWSETTING_INVOICE_STATUS_CHANGE,
											value));
							}
							break;

						case Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN_RETURN:
							// 返品
							functions.Add(
								Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_ACTION,
								(value) =>
									ValueText.GetValueText(
										Constants.TABLE_ORDERWORKFLOWSETTING,
										Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_ACTION,
										value));

							if (hasReason)
							{
								// 返品区分
								functions.Add(
									Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_REASON_KBN,
									(value) =>
										ValueText.GetValueText(
											Constants.TABLE_ORDERWORKFLOWSETTING,
											Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_REASON_KBN,
											value));

								// 返品理由
								functions.Add(
									Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_REASON_MEMO,
									(value) => value);
							}
							break;
					}

					// 拡張ステータス
					for (var index = 0; index < Constants.CONST_ORDER_EXTEND_STATUS_DBFIELDS_MAX; index++)
					{
						functions.Add(
							WorkflowSetting.m_FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE[index],
							getExtnedStatus);
					}

					// 督促ステータス
					functions.Add(
						Constants.FIELD_ORDERWORKFLOWSETTING_DEMAND_STATUS_CHANGE,
						(value) =>
							ValueText.GetValueText(
								Constants.TABLE_ORDERWORKFLOWSETTING,
								Constants.FIELD_ORDERWORKFLOWSETTING_DEMAND_STATUS_CHANGE,
								value));

					if (Constants.RECEIPT_OPTION_ENABLED)
					{
						// 領収書出力フラグ
						functions.Add(
							Constants.FIELD_ORDERWORKFLOWSETTING_RECEIPT_OUTPUT_FLG_CHANGE,
							(value) =>
								ValueText.GetValueText(
									Constants.TABLE_ORDERWORKFLOWSETTING,
									Constants.FIELD_ORDERWORKFLOWSETTING_RECEIPT_OUTPUT_FLG_CHANGE,
									value));
					}

					// メール送信設定
					functions.Add(
						Constants.FIELD_ORDERWORKFLOWSETTING_MAIL_ID,
						(value) => GetMailName(value));

					if (Constants.STORE_PICKUP_OPTION_ENABLED)
					{
						functions.Add(
							Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_STOREPICKUP_STATUS_CHANGE,
							(value) =>
								ValueText.GetValueText(
									Constants.TABLE_ORDERWORKFLOWSETTING,
									Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_STOREPICKUP_STATUS_CHANGE,
									value));
					}
				}
				// For case workflow type is fixed purchase 
				else
				{
					// 定期購入状態変更区分
					functions.Add(
						Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_IS_ALIVE_CHANGE,
						(value) =>
							ValueText.GetValueText(
								Constants.TABLE_ORDERWORKFLOWSETTING,
								Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_IS_ALIVE_CHANGE,
								value));

					// 決済ステータス変更区分
					functions.Add(
						Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_PAYMENT_STATUS_CHANGE,
						(value) =>
							ValueText.GetValueText(
								Constants.TABLE_ORDERWORKFLOWSETTING,
								Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_PAYMENT_STATUS_CHANGE,
								value));

					// 次回配送日変更区分
					functions.Add(
						Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_NEXT_SHIPPING_DATE_CHANGE,
						(value) =>
							ValueText.GetValueText(
								Constants.TABLE_ORDERWORKFLOWSETTING,
								Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_NEXT_SHIPPING_DATE_CHANGE,
								value));

					// 次々回配送日変更区分
					functions.Add(
						Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_NEXT_NEXT_SHIPPING_DATE_CHANGE,
						(value) =>
							ValueText.GetValueText(
								Constants.TABLE_ORDERWORKFLOWSETTING,
								Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_NEXT_NEXT_SHIPPING_DATE_CHANGE,
								value));

					// 配送不可エリア停止変更区分
					functions.Add(
						Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_STOP_UNAVAILABLE_SHIPPING_AREA_CHANGE,
						(value) =>
							ValueText.GetValueText(
								Constants.TABLE_ORDERWORKFLOWSETTING,
								Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_STOP_UNAVAILABLE_SHIPPING_AREA_CHANGE,
								value));

					if (hasReason)
					{
						// 解約理由
						functions.Add(
							Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CANCEL_REASON_ID,
							(value) =>
							{
								var fixedPurchaseCancelReason = DomainFacade.Instance.FixedPurchaseService.GetCancelReason(value);
								if (fixedPurchaseCancelReason == null) return string.Empty;
								return fixedPurchaseCancelReason.CancelReasonName;
							});

						// 解約メモ
						functions.Add(
							Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CANCEL_MEMO,
							(value) => value);
					}

					// 拡張ステータス
					for (var index = 0; index < Constants.CONST_ORDER_EXTEND_STATUS_DBFIELDS_MAX; index++)
					{
						functions.Add(
							WorkflowSetting.m_FIELD_FIXEDPURCHASEWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE[index],
							getExtnedStatus);
					}
				}
				break;

			case Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_KBN_RETURN_EXCHANGE:
				// 返品交換ステータス
				functions.Add(
					Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_EXCHANGE_STATUS_CHANGE,
					(value) =>
						ValueText.GetValueText(
							Constants.TABLE_ORDERWORKFLOWSETTING,
							Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_EXCHANGE_STATUS_CHANGE,
							value));

				// 返金ステータス
				functions.Add(
					Constants.FIELD_ORDERWORKFLOWSETTING_REPAYMENT_STATUS_CHANGE,
					(value) =>
						ValueText.GetValueText(
							Constants.TABLE_ORDERWORKFLOWSETTING,
							Constants.FIELD_ORDERWORKFLOWSETTING_REPAYMENT_STATUS_CHANGE,
							value));
				break;
		}
	}

	/// <summary>
	/// Get search conditions
	/// </summary>
	/// <param name="conditions">The order workflow search conditions</param>
	/// <param name="workflowType">The workflow type</param>
	/// <param name="displayDetailKbn">The display detail kbn</param>
	/// <param name="workflowKbn">The workflow kbn</param>
	/// <returns>A collection of search conditions</returns>
	private KeyValueItem[] GetSearchConditions(
		string conditions,
		string workflowType,
		string displayDetailKbn,
		string workflowKbn)
	{
		var queryString = HttpUtility.ParseQueryString(conditions);
		queryString.Remove(Constants.REQUEST_KEY_SORT_KBN);
		var defaultTableValueText = (workflowType == WorkflowSetting.m_KBN_WORKFLOW_TYPE_ORDER)
			? Constants.TABLE_ORDER
			: Constants.TABLE_FIXEDPURCHASE;

		var functions = GetConvertSearchConditionFunctions(workflowType, displayDetailKbn, workflowKbn);
		var keys = queryString.AllKeys
			.Where(key => ((string.IsNullOrEmpty(queryString[key]) == false) && (queryString[key] != ",")))
			.ToArray();
		var results = new List<KeyValueItem>();
		foreach (var key in keys)
		{
			var displayKey = GetDisplayKeys(key, VALUETEXT_FIELD_SEARCH_CONDITION_TITLE);
			if (string.IsNullOrEmpty(displayKey)) continue;

			switch (key)
			{
				case WorkflowSetting.m_FIELD_UPDATE_STATUS:
					// ステータス更新日
					results.Add(GetDisplaySearchConditionForUpdateStatus(
						false,
						queryString[key],
						queryString[WorkflowSetting.m_FIELD_UPDATE_STATUS_DAY],
						queryString[WorkflowSetting.m_FIELD_UPDATE_STATUS_TO],
						queryString[WorkflowSetting.m_FIELD_UPDATE_STATUS_FROM],
						queryString[WorkflowSetting.m_FIELD_UPDATE_STATUS_HOUR],
						queryString[WorkflowSetting.m_FIELD_UPDATE_STATUS_MINUTE],
						queryString[WorkflowSetting.m_FIELD_UPDATE_STATUS_SECOND]));
					break;

				case WorkflowSetting.m_FIELD_RETURN_EXCHANGE_UPDATE_STATUS:
					// 返品交換返金更新日
					results.Add(GetDisplaySearchConditionForUpdateStatus(
						true,
						queryString[key],
						queryString[WorkflowSetting.m_FIELD_RETURN_EXCHANGE_UPDATE_STATUS_DAY],
						queryString[WorkflowSetting.m_FIELD_RETURN_EXCHANGE_UPDATE_STATUS_TO],
						queryString[WorkflowSetting.m_FIELD_RETURN_EXCHANGE_UPDATE_STATUS_FROM],
						queryString[WorkflowSetting.m_FIELD_RETURN_EXCHANGE_UPDATE_STATUS_HOUR],
						queryString[WorkflowSetting.m_FIELD_RETURN_EXCHANGE_UPDATE_STATUS_MINUTE],
						queryString[WorkflowSetting.m_FIELD_RETURN_EXCHANGE_UPDATE_STATUS_SECOND]));
					break;

				case Constants.FIELD_ORDER_MEMO:
				case Constants.FIELD_ORDER_MANAGEMENT_MEMO:
				case Constants.FIELD_ORDER_SHIPPING_MEMO:
				case Constants.FIELD_ORDER_PAYMENT_MEMO:
				case Constants.FIELD_ORDER_RELATION_MEMO:
				case Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_MANAGEMENT_MEMO:
					if (functions.ContainsKey(key))
					{
						results.Add(new KeyValueItem
						{
							Key = displayKey,
							Value = string.Format(
								"{0}{1}",
								functions[key].Invoke(queryString[key], defaultTableValueText),
								(Constants.ORDERWORKFLOWSETTING_MEMO_TEXTBOX_ENABLED
										&& (string.IsNullOrEmpty(queryString[key + Constants.CONST_FIELD_EXTEND_SEARCH_TEXT]) == false))
									? string.Format(
										", {0}：{1}",
										GetDisplayKeys(key + Constants.CONST_FIELD_EXTEND_SEARCH_TEXT, VALUETEXT_FIELD_SEARCH_CONDITION_TITLE),
										queryString[key + Constants.CONST_FIELD_EXTEND_SEARCH_TEXT])
									: string.Empty)
						});
					}
					break;

				case Constants.FIELD_ORDER_ADVCODE_WORKFLOW:
					if (functions.ContainsKey(key))
					{
						results.Add(new KeyValueItem
						{
							Key = displayKey,
							Value = string.Format(
								"{0}{1}",
								functions[key].Invoke(queryString[key], defaultTableValueText),
								(string.IsNullOrEmpty(queryString[key + Constants.CONST_FIELD_EXTEND_SEARCH_TEXT]) == false)
									? string.Format(
										", {0}：{1}",
										displayKey,
										queryString[key + Constants.CONST_FIELD_EXTEND_SEARCH_TEXT])
									: string.Empty)
						});
					}
					break;

				default:
					if (Constants.ORDER_EXTEND_OPTION_ENABLED
						&& Constants.ORDER_EXTEND_ATTRIBUTE_FIELD_LIST.Contains(key))
					{
						results.Add(GetDisplayOrderExtend(key, displayKey, queryString[key].Split(',')));
					}

					if (functions.ContainsKey(key))
					{
						results.Add(new KeyValueItem
						{
							Key = displayKey,
							Value = functions[key].Invoke(queryString[key], defaultTableValueText)
						});
					}
					break;
			}
		}
		return results.ToArray();
	}

	/// <summary>
	/// Get display search condition for update status
	/// </summary>
	/// <param name="isWorkflowKbnReturnExchange">Is workflow kbn return exchange</param>
	/// <param name="updateStatus">Update status</param>
	/// <param name="updateStatusDay">Update status day</param>
	/// <param name="updateStatusFrom">Update status from</param>
	/// <param name="updateStatusTo">Update status to</param>
	/// <param name="updateStatusHour">Update status hour</param>
	/// <param name="updateStatusMinute">Update status minute</param>
	/// <param name="updateStatusSecond">Update status second</param>
	/// <returns>Display search condition for update status</returns>
	private KeyValueItem GetDisplaySearchConditionForUpdateStatus(
		bool isWorkflowKbnReturnExchange,
		string updateStatus,
		string updateStatusDay,
		string updateStatusFrom,
		string updateStatusTo,
		string updateStatusHour,
		string updateStatusMinute,
		string updateStatusSecond)
	{
		var displayKey = GetDisplayKeys(
			isWorkflowKbnReturnExchange
				? WorkflowSetting.m_FIELD_RETURN_EXCHANGE_UPDATE_STATUS
				: WorkflowSetting.m_FIELD_UPDATE_STATUS,
			VALUETEXT_FIELD_SEARCH_CONDITION_TITLE);
		var displaySpecifiedValue = string.Empty;
		if (updateStatusDay == WorkflowSetting.m_SEARCH_STATUS_DATE_FROMTO)
		{
			var specifiedFormat = ValueText.GetValueText(
				Constants.TABLE_ORDERWORKFLOWSETTING,
				VALUETEXT_FIELD_SEARCH_CONDITIONFORMAT,
				isWorkflowKbnReturnExchange
					? VALUETEXT_RETURN_EXCHANGE_UPDATE_STATUS_SPECIFIED_FORMAT
					: VALUETEXT_UPDATE_STATUS_SPECIFIED_FORMAT);
			displaySpecifiedValue = string.Format(
				specifiedFormat,
				ValueText.GetValueText(
					Constants.TABLE_ORDERWORKFLOWSETTING,
					WorkflowSetting.UPDATE_STATUS_DATE_FROM,
					updateStatusFrom),
				ValueText.GetValueText(
					Constants.TABLE_ORDERWORKFLOWSETTING,
					WorkflowSetting.UPDATE_STATUS_DATE_TO,
					updateStatusTo),
				updateStatusHour,
				updateStatusMinute,
				updateStatusSecond);
		}

		var format = ValueText.GetValueText(
			Constants.TABLE_ORDERWORKFLOWSETTING,
			VALUETEXT_FIELD_SEARCH_CONDITIONFORMAT,
			isWorkflowKbnReturnExchange
				? VALUETEXT_RETURN_EXCHANGE_UPDATE_STATUS_FORMAT
				: VALUETEXT_UPDATE_STATUS_FORMAT);

		var fieldName = WorkflowSetting.m_FIELD_UPDATE_STATUS;
		if (Constants.STORE_PICKUP_OPTION_ENABLED
			&& (isWorkflowKbnReturnExchange == false))
		{
			var checkValue = ValueText.GetValueText(
				Constants.TABLE_ORDER,
				fieldName,
				updateStatus);
			if (string.IsNullOrEmpty(checkValue))
			{
				fieldName = WorkflowSetting.REALSHOP_AND_STOREPICKUP;
			}
		}

		var displayValue = string.Format(
			format,
			ValueText.GetValueText(
				isWorkflowKbnReturnExchange
					? Constants.TABLE_ORDERWORKFLOWSETTING
					: Constants.TABLE_ORDER,
				isWorkflowKbnReturnExchange
					? WorkflowSetting.m_FIELD_RETURN_EXCHANGE_UPDATE_STATUS
					: fieldName,
				updateStatus),
			ValueText.GetValueText(
				Constants.TABLE_ORDERWORKFLOWSETTING,
				isWorkflowKbnReturnExchange
					? WorkflowSetting.m_FIELD_RETURN_EXCHANGE_UPDATE_STATUS_DAY
					: WorkflowSetting.m_FIELD_UPDATE_STATUS_DAY,
				updateStatus),
			displaySpecifiedValue);
		var result = new KeyValueItem
		{
			Key = displayKey,
			Value = displayValue
		};
		return result;
	}

	/// <summary>
	/// Get convert search condition functions
	/// </summary>
	/// <param name="type">Workflow type</param>
	/// <param name="displayDetailKbn">Display detail kbn</param>
	/// <param name="workflowKbn">Workflow kbn</param>
	/// <returns>A dictionary of convert search condition functions</returns>
	private IDictionary<string, Func<string, string, string>> GetConvertSearchConditionFunctions(
		string type,
		string displayDetailKbn,
		string workflowKbn)
	{
		var functions = new Dictionary<string, Func<string, string, string>>();

		// Get payment names
		var getPaymentNames = new Func<string, string, string>(
			(value, workflowType) =>
				 string.Join(
					", ",
					DomainFacade.Instance.PaymentService.GetPaymentNamesByPaymentIds(
						this.LoginShopOperatorShopId,
						value.Split(','))));

		// Get external payment status
		var getExternalPaymentStatus = new Func<string, string, string>(
			(value, workflowType) =>
				GetDisplaySearchValuesFromValueText(
					Constants.TABLE_ORDER,
					Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS,
					value.Split(',')));

		switch (workflowKbn)
		{
			case Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_KBN_DAY:
			case Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_KBN_MONTH:
			case Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_KBN_YEAR:
			case Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_KBN_OTHER:
				// Get shipping names
				var getShippingNames = new Func<string, string, string>(
					(value, workflowType) =>
						 string.Join(
							", ",
							DomainFacade.Instance.ShopShippingService.GetShippingNamesByShippingIds(
								this.LoginShopOperatorShopId,
								value.Split(','))));

				// Get order extend status value text
				var getOrderExtendStatusValueText = new Func<string, string, string>(
					(value, workflowType) =>
						GetDisplaySearchValuesFromValueText(
							Constants.TABLE_ORDERWORKFLOWSETTING,
							VALUETEXT_FIELD_ORDER_EXTEND_STATUS_SEARCH,
							value.Split(',')));

				// For case workflow type is order
				if (type == WorkflowSetting.m_KBN_WORKFLOW_TYPE_ORDER)
				{
					if (Constants.FIXEDPURCHASE_OPTION_ENABLED)
					{
						// 注文種別
						functions.Add(
							WorkflowSetting.m_TARGET_ORDER_TYPE,
							(value, workflowType) =>
								GetDisplaySearchValuesFromValueText(
									Constants.TABLE_ORDERWORKFLOWSETTING,
									WorkflowSetting.m_TARGET_ORDER_TYPE,
									value.Split(',')));
					}

					// 注文区分
					functions.Add(
						Constants.FIELD_ORDER_ORDER_KBN,
						(value, workflowType) =>
							GetDisplaySearchValuesFromValueText(
								Constants.TABLE_ORDER,
								Constants.FIELD_ORDER_ORDER_KBN,
								value.Split(',')));

					// 注文者区分
					functions.Add(
						Constants.FIELD_ORDEROWNER_OWNER_KBN,
						(value, workflowType) =>
							GetDisplaySearchValuesFromValueText(
								Constants.TABLE_ORDEROWNER,
								Constants.FIELD_ORDEROWNER_OWNER_KBN,
								value.Split(',')));

					// 注文ステータス
					functions.Add(
						Constants.FIELD_ORDER_ORDER_STATUS,
						(value, workflowType) =>
							GetDisplaySearchValuesFromValueText(
								Constants.TABLE_ORDERWORKFLOWSETTING,
								Constants.FIELD_ORDER_ORDER_STATUS,
								value.Split(',')));

					// 商品ID
					functions.Add(
						Constants.FIELD_ORDERITEM_PRODUCT_ID,
						(value, workflowType) =>
							string.Join(", ", value.Split(',')));

					if (Constants.SETPROMOTION_OPTION_ENABLED)
					{
						// セットプロモーションID
						functions.Add(
							Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_ID,
							(value, workflowType) => value);
					}

					if (Constants.NOVELTY_OPTION_ENABLED)
					{
						// ノベルティID
						functions.Add(
							Constants.FIELD_ORDERITEM_NOVELTY_ID,
							(value, workflowType) =>
								GetDisplayYesNoSearchValues(value));
					}

					if (Constants.RECOMMEND_OPTION_ENABLED)
					{
						// レコメンドID
						functions.Add(Constants.FIELD_ORDERITEM_RECOMMEND_ID,
							(value, workflowType) =>
								GetDisplayYesNoSearchValues(value));
					}

					// 合計金額
					functions.Add(
						Constants.FIELD_ORDER_ORDER_PRICE_TOTAL,
						(value, workflowType) => value);

					// 入金ステータス
					functions.Add(
						Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS,
						(value, workflowType) =>
							GetDisplaySearchValuesFromValueText(
								Constants.TABLE_ORDER,
								Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS,
								value.Split(',')));

					// 督促ステータス
					functions.Add(
						Constants.FIELD_ORDER_DEMAND_STATUS,
						(value, workflowType) =>
							GetDisplaySearchValuesFromValueText(
								Constants.TABLE_ORDER,
								Constants.FIELD_ORDER_DEMAND_STATUS,
								value.Split(',')));

					// 注文拡張ステータス1～50
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS1, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS2, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS3, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS4, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS5, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS6, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS7, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS8, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS9, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS10, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS11, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS12, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS13, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS14, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS15, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS16, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS17, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS18, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS19, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS20, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS21, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS22, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS23, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS24, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS25, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS26, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS27, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS28, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS29, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS30, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS31, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS32, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS33, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS34, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS35, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS36, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS37, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS38, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS39, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS40, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS41, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS42, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS43, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS44, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS45, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS46, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS47, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS48, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS49, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_ORDER_EXTEND_STATUS50, getOrderExtendStatusValueText);

					if (Constants.REALSTOCK_OPTION_ENABLED)
					{
						// 引当状況
						functions.Add(
							Constants.FIELD_ORDER_ORDER_STOCKRESERVED_STATUS,
							(value, workflowType) =>
								GetDisplaySearchValuesFromValueText(
									Constants.TABLE_ORDER,
									Constants.FIELD_ORDER_ORDER_STOCKRESERVED_STATUS,
									value.Split(',')));

						// 出荷状況
						functions.Add(
							Constants.FIELD_ORDER_ORDER_SHIPPED_STATUS,
							(value, workflowType) =>
								GetDisplaySearchValuesFromValueText(
									Constants.TABLE_ORDER,
									Constants.FIELD_ORDER_ORDER_SHIPPED_STATUS,
									value.Split(',')));
					}

					// 決済種別
					functions.Add(Constants.FIELD_ORDER_ORDER_PAYMENT_KBN, getPaymentNames);

					// 外部決済ステータス：クレジットカード
					functions.Add(WorkflowSetting.m_ORDER_EXTERNAL_PAYMENT_STATUS_CARD, getExternalPaymentStatus);

					// 外部決済ステータス：コンビニ（後払い）
					functions.Add(WorkflowSetting.m_ORDER_EXTERNAL_PAYMENT_STATUS_CVS, getExternalPaymentStatus);

					// 外部決済ステータス：後付款(TriLink後払い)
					functions.Add(WorkflowSetting.m_ORDER_EXTERNAL_PAYMENT_STATUS_TRYLINK_AFTERPAY, getExternalPaymentStatus);

					// ECPay決済方法
					functions.Add(
						WorkflowSetting.m_ORDER_EXTERNAL_PAYMENT_STATUS_ECPAY,
						(value, workflowType) =>
							GetDisplaySearchValuesFromValueText(
								Constants.TABLE_PAYMENT,
								Constants.VALUETEXT_PARAM_ECPAY_PAYMENT_TYPE,
								value.Split(',')));

					// 藍新Pay決済方法
					functions.Add(
						WorkflowSetting.m_ORDER_EXTERNAL_PAYMENT_STATUS_NEWEBPAY,
						(value, workflowType) =>
							GetDisplaySearchValuesFromValueText(
								Constants.TABLE_PAYMENT,
								Constants.VALUETEXT_PARAM_NEWEB_PAYMENT_TYPE,
								value.Split(',')));

					// 外部決済ステータス：全決済
					functions.Add(Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS, getExternalPaymentStatus);

					// 最終与信日時
					functions.Add(
						Constants.FIELD_ORDER_EXTERNAL_PAYMENT_AUTH_DATE,
						(value, workflowType) =>
							GetDisplayYesNoSearchValues(value, Constants.FIELD_ORDER_EXTERNAL_PAYMENT_AUTH_DATE));

					// 配送先：国
					if (Constants.GLOBAL_OPTION_ENABLE)
					{
						var countryList = DomainFacade.Instance.CountryLocationService.GetShippingAvailableCountry();
						functions.Add(
							Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE,
							(value, workflowType) =>
								GetDisplaySearchValues(value.Split(','), (searchValue) =>
								{
									var countryLocation = countryList.FirstOrDefault(
										shipping => (shipping.CountryIsoCode == searchValue));
									var hasCountryLocation = (countryLocation != null);
									return new Tuple<bool, string>(hasCountryLocation, hasCountryLocation ? countryLocation.CountryName : string.Empty);
								}));
					}

					// 配送種別
					functions.Add(Constants.FIELD_ORDER_SHIPPING_ID, getShippingNames);

					if (GlobalConfigUtil.UseLeadTime())
					{
						// 出荷予定日指定
						functions.Add(
							Constants.FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE,
							(value, workflowType) =>
								GetDisplayYesNoSearchValues(value, Constants.FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE));
					}

					// 配送希望日指定
					functions.Add(
						Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE,
						(value, workflowType) =>
							GetDisplayYesNoSearchValues(value, Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE));

					if (Constants.SHIPPINGPRICE_SEPARATE_ESTIMATE_ENABLED)
					{
						// 送料の別途見積
						functions.Add(
							Constants.FIELD_ORDER_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG,
							(value, workflowType) =>
								GetDisplaySearchValuesFromValueText(
									Constants.TABLE_ORDER,
									Constants.FIELD_ORDER_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG,
									value.Split(',')));
					}

					// 配送伝票番号
					functions.Add(
						Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO,
						(value, workflowType) =>
							GetDisplaySearchValuesFromValueText(
								Constants.TABLE_ORDERSHIPPING,
								Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO + (Constants.GIFTORDER_OPTION_ENABLED ? Constants.CONST_FIELD_EXTEND_FOR_GIFT : string.Empty),
								value.Split(',')));

					// 出荷後変更区分
					functions.Add(
						Constants.FIELD_ORDER_SHIPPED_CHANGED_KBN,
						(value, workflowType) =>
							GetDisplaySearchValuesFromValueText(
								Constants.TABLE_ORDERWORKFLOWSETTING,
								Constants.FIELD_ORDER_SHIPPED_CHANGED_KBN,
								value.Split(',')));

					if (Constants.MALLCOOPERATION_OPTION_ENABLED)
					{
						// サイト
						var mallCooperationSettings = DomainFacade.Instance.MallCooperationSettingService.GetAll(this.LoginShopOperatorShopId);
						functions.Add(
							Constants.FIELD_ORDER_MALL_ID,
							(value, workflowType) =>
								GetDisplaySearchValues(value.Split(','), (searchValue) =>
								{
									if (searchValue == Constants.FLG_ORDER_MALL_ID_OWN_SITE)
									{
										var convertedValue = ValueText.GetValueText(
											Constants.VALUETEXT_PARAM_SITENAME,
											Constants.VALUETEXT_PARAM_OWNSITENAME,
											searchValue);
										return new Tuple<bool, string>(true, convertedValue);
									}

									var mallCooperationSetting = mallCooperationSettings.FirstOrDefault(
										mall => (mall.MallId == searchValue));
									var hasMallCooperationSetting = (mallCooperationSetting != null);
									return new Tuple<bool, string>(hasMallCooperationSetting, hasMallCooperationSetting ? mallCooperationSetting.MallName : string.Empty);
								}));

						// モール連携ステータス
						functions.Add(
							WorkflowSetting.m_FIELD_MALL_LINK_STATUS,
							(value, workflowType) =>
								GetDisplaySearchValuesFromValueText(
									Constants.TABLE_ORDER,
									WorkflowSetting.m_FIELD_MALL_LINK_STATUS,
									value.Split(',')));

						// 楽天ポイント利用方法
						functions.Add(
							WorkflowSetting.m_FIELD_RAKUTEN_POINT_USE_TYPE,
							(value, workflowType) =>
								GetDisplaySearchValuesFromValueText(
									Constants.TABLE_ORDERWORKFLOWSETTING,
									WorkflowSetting.m_FIELD_RAKUTEN_POINT_USE_TYPE,
									value.Split(',')));
					}

					if (Constants.MALLCOOPERATION_OPTION_ENABLED
						|| Constants.URERU_AD_IMPORT_ENABLED)
					{
						// 外部連携ステータス
						functions.Add(
							WorkflowSetting.m_FIELD_EXTERNAL_IMPORT_STATUS,
							(value, workflowType) =>
								GetDisplaySearchValuesFromValueText(
									Constants.TABLE_ORDER,
									WorkflowSetting.m_FIELD_EXTERNAL_IMPORT_STATUS,
									value.Split(',')));
					}

					if (Constants.DIGITAL_CONTENTS_OPTION_ENABLED)
					{
						// デジタルコンテンツ商品
						functions.Add(
							Constants.FIELD_ORDER_DIGITAL_CONTENTS_FLG,
							(value, workflowType) =>
								GetDisplaySearchValuesFromValueText(
									Constants.TABLE_ORDERWORKFLOWSETTING,
									Constants.FIELD_ORDER_DIGITAL_CONTENTS_FLG,
									value.Split(',')));
					}

					if (Constants.FIXEDPURCHASE_OPTION_ENABLED)
					{
						// 定期購買注文
						functions.Add(Constants.FIELD_ORDER_FIXED_PURCHASE_ID,
							(value, workflowType) =>
								GetDisplaySearchValuesFromValueText(
									Constants.TABLE_ORDERWORKFLOWSETTING,
									Constants.VALUETEXT_PARAM_ORDERWORKFLOWSETTING_FIXED_PURCHASE,
									value.Split(',')));

						// 定期購入回数(注文時点)
						var getCount = new Func<string, string, string>(
							(value, workflowType) =>
							{
								var range = value.Split(',');
								var format = ValueText.GetValueText(
									Constants.TABLE_ORDERWORKFLOWSETTING,
									VALUETEXT_FIELD_SEARCH_CONDITIONFORMAT,
									VALUETEXT_FIXEDPURCHASE_COUNT);
								return string.Format(format, range[0], range[1]);
							});
						functions.Add(Constants.FIELD_ORDER_FIXED_PURCHASE_ORDER_COUNT, getCount);

						// 定期購入回数(出荷時点)
						functions.Add(Constants.FIELD_ORDER_FIXED_PURCHASE_SHIPPED_COUNT, getCount);
					}

					// 注文メモ
					functions.Add(
						Constants.FIELD_ORDER_MEMO,
						(value, workflowType) =>
							GetDisplaySearchValuesFromValueText(
								Constants.TABLE_ORDERWORKFLOWSETTING,
								Constants.FIELD_ORDER_MEMO,
								value.Split(',')));

					// 管理メモ
					functions.Add(
						Constants.FIELD_ORDER_MANAGEMENT_MEMO,
						(value, workflowType) =>
							GetDisplaySearchValuesFromValueText(
								Constants.TABLE_ORDERWORKFLOWSETTING,
								Constants.FIELD_ORDER_MANAGEMENT_MEMO,
								value.Split(',')));

					// 配送メモ
					functions.Add(
						Constants.FIELD_ORDER_SHIPPING_MEMO,
						(value, workflowType) =>
							GetDisplaySearchValuesFromValueText(
								Constants.TABLE_ORDERWORKFLOWSETTING,
								Constants.FIELD_ORDER_SHIPPING_MEMO,
								value.Split(',')));

					// 決済連携メモ
					functions.Add(
						Constants.FIELD_ORDER_PAYMENT_MEMO,
						(value, workflowType) =>
							GetDisplaySearchValuesFromValueText(
								Constants.TABLE_ORDERWORKFLOWSETTING,
								Constants.FIELD_ORDER_PAYMENT_MEMO,
								value.Split(',')));

					// 決済連携メモ
					functions.Add(
						Constants.FIELD_ORDER_RELATION_MEMO,
						(value, workflowType) =>
							GetDisplaySearchValuesFromValueText(
								Constants.TABLE_ORDERWORKFLOWSETTING,
								Constants.FIELD_ORDER_RELATION_MEMO,
								value.Split(',')));

					// ユーザー特記欄
					functions.Add(
						Constants.FIELD_USER_USER_MEMO,
						(value, workflowType) =>
							GetDisplaySearchValuesFromValueText(
								Constants.TABLE_USER,
								Constants.FIELD_USER_USER_MEMO,
								value.Split(',')));

					// 商品付帯情報
					functions.Add(
						Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS,
						(value, workflowType) =>
							GetDisplaySearchValuesFromValueText(
								Constants.TABLE_ORDERITEM,
								Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS,
								value.Split(',')));

					if (Constants.W2MP_AFFILIATE_OPTION_ENABLED)
					{
						// 広告コード
						functions.Add(
							Constants.FIELD_ORDER_ADVCODE_WORKFLOW,
							(value, workflowType) =>
								GetDisplaySearchValuesFromValueText(
									Constants.TABLE_ORDER,
									Constants.FIELD_ORDER_ADVCODE_WORKFLOW,
									value.Split(',')));
					}

					if (Constants.GIFTORDER_OPTION_ENABLED)
					{
						// ギフト購入フラグ
						functions.Add(
							Constants.FIELD_ORDER_GIFT_FLG,
							(value, workflowType) =>
								GetDisplaySearchValuesFromValueText(
									Constants.TABLE_ORDER,
									Constants.FIELD_ORDER_GIFT_FLG,
									value.Split(',')));
					}

					// ユーザー管理レベル
					functions.Add(
						Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID,
						(value, workflowType) =>
							string.Join(
								", ",
								DomainFacade.Instance.UserManagementLevelService.GetUserManagementLevelNamesByUserManagementLevelIds(value.Split(','))));

					// 配送先
					functions.Add(
						Constants.FIELD_ORDERSHIPPING_ANOTHER_SHIPPING_FLG,
						(value, workflowType) =>
							ValueText.GetValueText(
								Constants.TABLE_ORDERSHIPPING,
								Constants.FIELD_ORDERSHIPPING_ANOTHER_SHIPPING_FLG,
								value));

					if (Constants.SEARCHCONDITION_SHIPPINGADDR1_ENABLED
						&& ((Constants.GLOBAL_OPTION_ENABLE == false)
							|| ShippingCountryUtil.GetShippingCountryAvailableListAndCheck(Constants.COUNTRY_ISO_CODE_JP)))
					{
						// 配送先：都道府県
						functions.Add(
							Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_SHIPPING_PREFECTURES,
							(value, workflowType) => string.Join(", ", PrefectureUtility.GetPrefectures(value)));

						// 市区町村
						functions.Add(
								Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_SHIPPING_CITY,
								(value, workflowType) => string.Join(", ", value));
					}

					if (OrderCommon.CanDisplayInvoiceBundle())
					{
						// 請求書同梱フラグ
						functions.Add(
							Constants.FIELD_ORDER_INVOICE_BUNDLE_FLG,
							(value, workflowType) =>
								GetDisplaySearchValuesFromValueText(
									Constants.TABLE_ORDER,
									Constants.FIELD_ORDER_INVOICE_BUNDLE_FLG,
									value.Split(',')));
					}

					if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED)
					{
						// 配送状態
						functions.Add(
							Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS,
							(value, workflowType) =>
								GetDisplaySearchValuesFromValueText(
									Constants.TABLE_ORDERSHIPPING,
									Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS,
									value.Split(',')));
					}

					// 配送方法
					functions.Add(
						Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD,
						(value, workflowType) =>
							GetDisplaySearchValuesFromValueText(
								Constants.TABLE_ORDERSHIPPING,
								Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD,
								value.Split(',')));

					// 配送サービス
					functions.Add(
						Constants.FIELD_ORDERSHIPPING_DELIVERY_COMPANY_ID,
						(value, workflowType) =>
							string.Join(
								", ",
								DomainFacade.Instance.DeliveryCompanyService.GetDeliveryCompanyNamesByDeliveryCompanyIds(value.Split(','))));

					if (Constants.RECEIPT_OPTION_ENABLED)
					{
						// 領収書希望フラグ
						functions.Add(
							Constants.FIELD_ORDER_RECEIPT_FLG,
							(value, workflowType) =>
								GetDisplaySearchValuesFromValueText(
									Constants.TABLE_ORDER,
									Constants.FIELD_ORDER_RECEIPT_FLG,
									value.Split(',')));

						// 領収書出力フラグ
						functions.Add(
							Constants.FIELD_ORDER_RECEIPT_OUTPUT_FLG,
							(value, workflowType) =>
								GetDisplaySearchValuesFromValueText(
									Constants.TABLE_ORDER,
									Constants.FIELD_ORDER_RECEIPT_OUTPUT_FLG,
									value.Split(',')));
					}

					if (Constants.STORE_PICKUP_OPTION_ENABLED)
					{
						functions.Add(
							Constants.FIELD_ORDER_STOREPICKUP_STATUS,
							(value, workflowType) =>
								GetDisplaySearchValuesFromValueText(
									Constants.TABLE_ORDER,
									Constants.FIELD_ORDER_STOREPICKUP_STATUS,
									value.Split(',')));
					}
				}
				// For case workflow type is fixed purchase
				else
				{
					// 注文区分
					functions.Add(
						Constants.FIELD_FIXEDPURCHASE_ORDER_KBN,
						(value, workflowType) =>
							GetDisplaySearchValuesFromValueText(
								Constants.TABLE_ORDER,
								Constants.FIELD_ORDER_ORDER_KBN,
								value.Split(',')));

					// 定期購入ステータス
					functions.Add(
						Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_STATUS,
						(value, workflowType) =>
							GetDisplaySearchValuesFromValueText(
								Constants.TABLE_ORDERWORKFLOWSETTING,
								Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_STATUS,
								value.Split(',')));

					// 商品ID
					functions.Add(
						Constants.FIELD_FIXEDPURCHASEITEM_PRODUCT_ID,
						(value, workflowType) =>
							string.Join(", ", value.Split(',')));

					// 定期再開予定日
					functions.Add(
						Constants.FIELD_FIXEDPURCHASE_RESUME_DATE,
						(value, workflowType) =>
							GetDisplayYesNoSearchValues(value, Constants.FIELD_FIXEDPURCHASE_RESUME_DATE));

					// 定期購入区分
					functions.Add(
						Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_KBN,
						(value, workflowType) =>
							GetDisplaySearchValuesFromValueText(
								Constants.TABLE_ORDERWORKFLOWSETTING,
								Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_KBN,
								value.Split(',')));

					// 配送種別(定期)
					functions.Add(Constants.FIELD_PRODUCT_SHIPPING_TYPE, getShippingNames);

					// 決済種別(定期)
					functions.Add(Constants.FIELD_FIXEDPURCHASE_ORDER_PAYMENT_KBN, getPaymentNames);

					// 管理メモ(定期)
					functions.Add(
						Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_MANAGEMENT_MEMO,
						(value, workflowType) =>
							GetDisplaySearchValuesFromValueText(
								Constants.TABLE_ORDERWORKFLOWSETTING,
								Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_MANAGEMENT_MEMO,
								value.Split(',')));

					// 配送メモ(定期)
					functions.Add(
						Constants.FIELD_FIXEDPURCHASE_SHIPPING_MEMO,
						(value, workflowType) =>
							GetDisplaySearchValuesFromValueText(
								Constants.TABLE_ORDERWORKFLOWSETTING,
								Constants.FIELD_FIXEDPURCHASE_SHIPPING_MEMO,
								value.Split(',')));

					// 購入回数(注文基準from)
					functions.Add(
						WorkflowSetting.m_FIXEDPURCHASE_ORDER_COUNT_FROM,
						(value, workflowType) => value);

					// 購入回数(注文基準to)
					functions.Add(
						WorkflowSetting.m_FIXEDPURCHASE_ORDER_COUNT_TO,
						(value, workflowType) => value);

					// 購入回数(出荷基準from)
					functions.Add(
						WorkflowSetting.m_FIXEDPURCHASE_SHIPPED_COUNT_FROM,
						(value, workflowType) => value);

					// 購入回数(出荷基準to)
					functions.Add(
						WorkflowSetting.m_FIXEDPURCHASE_SHIPPED_COUNT_TO,
						(value, workflowType) => value);

					// 作成日
					functions.Add(
						Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_DATE_CREATED,
						(value, workflowType) =>
							GetDisplaySearchValuesFromValueText(value));

					// 更新日
					functions.Add(
						Constants.FIELD_ORDERWORKFLOWSCENARIOSETTING_DATE_CHANGED,
						(value, workflowType) =>
							GetDisplaySearchValuesFromValueText(value));

					// 最終購入日
					functions.Add(
						Constants.FIELD_FIXEDPURCHASE_LAST_ORDER_DATE,
						(value, workflowType) =>
							GetDisplaySearchValuesFromValueText(value));

					// 購入開始日
					functions.Add(
						Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_DATE_BGN,
						(value, workflowType) =>
							GetDisplaySearchValuesFromValueText(value));

					// 次回配送日
					functions.Add(
						Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_DATE,
						(value, workflowType) =>
							GetDisplayYesNoSearchValues(value, VALUETEXT_FIELD_NEXT_SHIPPING_DATE_FORMAT));

					// 次々回配送日
					functions.Add(
						Constants.FIELD_FIXEDPURCHASE_NEXT_NEXT_SHIPPING_DATE,
						(value, workflowType) =>
							GetDisplayYesNoSearchValues(value, VALUETEXT_FIELD_NEXT_SHIPPING_DATE_FORMAT));

					// 注文拡張ステータス1～40
					functions.Add(Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS1, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS2, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS3, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS4, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS5, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS6, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS7, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS8, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS9, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS10, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS11, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS12, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS13, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS14, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS15, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS16, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS17, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS18, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS19, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS20, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS21, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS22, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS23, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS24, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS25, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS26, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS27, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS28, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS29, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS30, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS31, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS32, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS33, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS34, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS35, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS36, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS37, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS38, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS39, getOrderExtendStatusValueText);
					functions.Add(Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS40, getOrderExtendStatusValueText);

					if (Constants.RECEIPT_OPTION_ENABLED)
					{
						// 領収書希望フラグ
						functions.Add(
							Constants.FIELD_FIXEDPURCHASE_RECEIPT_FLG,
							(value, workflowType) =>
								GetDisplaySearchValuesFromValueText(
									Constants.TABLE_FIXEDPURCHASE,
									Constants.FIELD_FIXEDPURCHASE_RECEIPT_FLG,
									value.Split(',')));
					}
				}
				break;

			case Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_KBN_RETURN_EXCHANGE:
				// 返品交換区分
				functions.Add(
					Constants.FIELD_ORDER_RETURN_EXCHANGE_KBN,
					(value, workflowType) =>
						GetDisplaySearchValuesFromValueText(
							Constants.TABLE_ORDERWORKFLOWSETTING,
							Constants.FIELD_ORDER_RETURN_EXCHANGE_KBN,
							value.Split(',')));

				// 返品交換都合区分
				functions.Add(
					Constants.FIELD_ORDER_RETURN_EXCHANGE_REASON_KBN,
					(value, workflowType) =>
						GetDisplaySearchValuesFromValueText(
							Constants.TABLE_ORDERWORKFLOWSETTING,
							Constants.FIELD_ORDER_RETURN_EXCHANGE_REASON_KBN,
							value.Split(',')));

				// 返品交換ステータス
				functions.Add(
					Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_STATUS,
					(value, workflowType) =>
						GetDisplaySearchValuesFromValueText(
							Constants.TABLE_ORDERWORKFLOWSETTING,
							Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_STATUS,
							value.Split(',')));

				// 返金ステータス
				functions.Add(Constants.FIELD_ORDER_ORDER_REPAYMENT_STATUS,
					(value, workflowType) =>
						GetDisplaySearchValuesFromValueText(
							Constants.TABLE_ORDERWORKFLOWSETTING,
							Constants.FIELD_ORDER_ORDER_REPAYMENT_STATUS,
							value.Split(',')));

				// 決済種別
				functions.Add(ORDER_RETURN_PAYMENT_KBN, getPaymentNames);

				// 外部決済ステータス：クレジットカード
				functions.Add(WorkflowSetting.m_ORDER_RETURN_EXTERNAL_PAYMENT_STATUS_CARD, getExternalPaymentStatus);

				// 外部決済ステータス：コンビニ（後払い）
				functions.Add(WorkflowSetting.m_ORDER_RETURN_EXTERNAL_PAYMENT_STATUS_CVS, getExternalPaymentStatus);

				// 外部決済ステータス：後付款(TriLink後払い)
				functions.Add(WorkflowSetting.m_ORDER_RETURN_EXTERNAL_PAYMENT_STATUS_TRYLINK_AFTERPAY, getExternalPaymentStatus);

				// 外部決済ステータス：全決済
				functions.Add(Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS, getExternalPaymentStatus);
				break;
		}

		if (OrderCommon.DisplayTwInvoiceInfo())
		{
			// 発票ステータス
			functions.Add(
				Constants.FIELD_TWORDERINVOICE_TW_INVOICE_STATUS,
				(value, workflowType) =>
					GetDisplaySearchValuesFromValueText(
						Constants.TABLE_TWORDERINVOICE,
						Constants.FIELD_TWORDERINVOICE_TW_INVOICE_STATUS,
						value.Split(',')));
		}
		return functions;
	}

	/// <summary>
	/// Get display order extend
	/// </summary>
	/// <param name="id">Setting id</param>
	/// <param name="displayKey">Display key</param>
	/// <param name="values">Values</param>
	/// <returns>Display order extend</returns>
	private KeyValueItem GetDisplayOrderExtend(string id, string displayKey, string[] values)
	{
		var model = DataCacheControllerFacade.GetOrderExtendSettingCacheController().CacheData
			.SettingModels.FirstOrDefault(m => m.SettingId == id);
		var result = new KeyValueItem();
		if (model == null) return result;

		switch (model.InputType)
		{
			case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_TEXT:
				result = new KeyValueItem
				{
					Key = displayKey,
					Value = GetDisplaySearchValuesFromValueText(
						Constants.TABLE_ORDER,
						Constants.SEARCH_FIELD_ORDER_EXTEND_FLG,
						values)
				};
				break;

			case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_DROPDOWN:
			case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_RADIO:
			case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_CHECKBOX:
				var settings = OrderExtendCommon.GetListItemForManager(model.InputDefault);
				var displayItems = new List<string>();
				foreach (var value in values)
				{
					var setting = settings.FirstOrDefault(item => item.Value == value);
					if (setting == null) continue;

					displayItems.Add(setting.Text);
				}
				result = new KeyValueItem
				{
					Key = displayKey,
					Value = string.Join(", ", displayItems)
				};
				break;
		}
		return result;
	}

	/// <summary>
	/// Get display cassette default values
	/// </summary>
	/// <param name="value">The action value</param>
	/// <returns>A display action value of string</returns>
	private string GetDisplayCassetteDefaultValues(string value)
	{
		var valueArray = value.Split('&');
		if (valueArray.All(item => string.IsNullOrEmpty(item))) return string.Empty;

		var field = GetDisplayKeys(valueArray[0], VALUETEXT_FIELD_ACTIONS_TITLE);
		if (string.IsNullOrEmpty(field)) return string.Empty;

		var textField = valueArray[0].Contains(EXTEND_STATUS_CHANGE)
			? VALUETEXT_FIELD_ORDER_EXTEND_STATUS_CHANGE
			: valueArray[0].Replace(CASSETTE_TEXT, string.Empty);
		var fieldValue = ValueText.GetValueText(
			Constants.TABLE_ORDERWORKFLOWSETTING,
			textField,
			valueArray[1]);
		return string.Format("{0}&{1}", field, fieldValue);
	}

	/// <summary>
	/// Get display cassette values
	/// </summary>
	/// <param name="field">Field name</param>
	/// <param name="values">The action values</param>
	/// <returns>A display action value of string</returns>
	private string GetDisplayCassetteValues(string field, string[] values)
	{
		var result = new List<string>();
		var newField = field.Replace(CASSETTE_TEXT, string.Empty);

		if (ValueText.Exists(Constants.TABLE_ORDERWORKFLOWSETTING, newField))
		{
			foreach (var value in values)
			{
				var settingValues = value.Split('&');
				if (settingValues.Length != 2) continue;

				var text = ValueText.GetValueText(
					Constants.TABLE_ORDERWORKFLOWSETTING,
					newField,
					settingValues[0]);
				var mail = GetMailName(settingValues[1]);
				if (string.IsNullOrEmpty(mail))
				{
					result.Add(text);
					continue;
				}

				result.Add(string.Format("{0}&{1}", text, mail));
			}
		}
		return string.Join(", ", result);
	}

	/// <summary>
	/// Get display search values from value text
	/// </summary>
	/// <param name="searchValue">The search value</param>
	/// <returns>A display search value as string</returns>
	private string GetDisplaySearchValuesFromValueText(string searchValue)
	{
		var format = ValueText.GetValueText(
			Constants.TABLE_ORDERWORKFLOWSETTING,
			VALUETEXT_FIELD_SEARCH_CONDITIONFORMAT,
			VALUETEXT_FIELD_DATE_FORMAT);
		return string.Format(format, searchValue);
	}

	/// <summary>
	/// Get display search values from value text
	/// </summary>
	/// <param name="tableName">The value text table name</param>
	/// <param name="fieldName">The value text field name</param>
	/// <param name="searchValues">The search values</param>
	/// <returns>A display search values as string</returns>
	private string GetDisplaySearchValuesFromValueText(
		string tableName,
		string fieldName,
		string[] searchValues)
	{
		var convertedValues = searchValues.Select(
			value => ValueText.GetValueText(tableName, fieldName, value));
		var result = string.Join(", ", convertedValues);
		return result;
	}

	/// <summary>
	/// Get display update status from to values
	/// </summary>
	/// <param name="tableName">The value text table name</param>
	/// <param name="fieldName">The value text field name</param>
	/// <param name="searchValues">The search values</param>
	/// <returns>A display update status values as string</returns>
	private string GetDisplayUpdateStatusFromToValues(
		string tableName,
		string fieldName,
		string searchValues)
	{
		if (searchValues == WorkflowSetting.m_SEARCH_STATUS_DATE_FROMTO)
		{
			return ValueText.GetValueText(tableName, fieldName, searchValues);
		}
		return searchValues;
	}

	/// <summary>
	/// Get display search values
	/// </summary>
	/// <param name="searchValues">The search values</param>
	/// <param name="convert">The convert value function</param>
	/// <returns>A display search values as string</returns>
	private string GetDisplaySearchValues(
		string[] searchValues,
		Func<string, Tuple<bool, string>> convert)
	{
		var convertedValues = new List<string>();
		foreach (var searchValue in searchValues)
		{
			var result = convert(searchValue);
			if ((searchValue == WorkflowSetting.m_SHIPPINGDATE_SPECIFIED) && result.Item1)
			{
				convertedValues.Insert(0, result.Item2);
				continue;
			}
			if (result.Item1) convertedValues.Add(result.Item2);
		}
		return string.Join(", ", convertedValues);
	}

	/// <summary>
	/// Get display yes no search values
	/// </summary>
	/// <param name="value">The search values</param>
	/// <returns>A display search values as string</returns>
	private string GetDisplayYesNoSearchValues(string value)
	{
		return GetDisplayYesNoSearchValues(value, (searchValue) => searchValue);
	}

	/// <summary>
	/// Get display yes no search values
	/// </summary>
	/// <param name="value">The search values</param>
	/// <param name="defaultFormatValue">The value text default format value</param>
	/// <returns>A display search values as string</returns>
	private string GetDisplayYesNoSearchValues(string value, string defaultFormatValue)
	{
		return GetDisplayYesNoSearchValues(value, (searchValue) =>
		{
			var values = searchValue.Split(WorkflowSetting.m_SHIPPINGDATE_SEPARATOR_CHARACTER);
			var from = (values.Length >= 1) ? values[0] : " ";
			var to = (values.Length >= 2) ? values[1] : " ";

			var format = ValueText.GetValueText(
				Constants.TABLE_ORDERWORKFLOWSETTING,
				VALUETEXT_FIELD_SEARCH_CONDITIONFORMAT,
				defaultFormatValue);

			var convertedValue = string.Format(format, from, to);
			return convertedValue;
		});
	}

	/// <summary>
	/// Get display yes no search values
	/// </summary>
	/// <param name="value">The search values</param>
	/// <param name="defaultConvert">The default convert function</param>
	/// <returns>A display search values as string</returns>
	private string GetDisplayYesNoSearchValues(string value, Func<string, string> defaultConvert)
	{
		return GetDisplaySearchValues(value.Split(','), (searchValue) =>
		{
			var convertedValue = string.Empty;
			switch (searchValue)
			{
				case WorkflowSetting.m_SHIPPINGDATE_SPECIFIED:
				case WorkflowSetting.m_SHIPPINGDATE_UNSPECIFIED:
					convertedValue = ValueText.GetValueText(
						Constants.TABLE_ORDERWORKFLOWSETTING,
						VALUETEXT_SPECIFIED_UNSPECIFIED,
						searchValue);
					break;

				default:
					convertedValue = defaultConvert(searchValue);
					break;
			}

			if (string.IsNullOrEmpty(convertedValue))
			{
				return new Tuple<bool, string>(false, string.Empty);
			}
			return new Tuple<bool, string>(true, convertedValue);
		});
	}

	/// <summary>
	/// Get display keys
	/// </summary>
	/// <param name="key">Field key</param>
	/// <param name="title">Title</param>
	/// <returns>A display string of key</returns>
	private string GetDisplayKeys(string key, string title)
	{
		if (string.IsNullOrEmpty(key)) return string.Empty;

		if (key.Contains(Constants.FIELD_ORDER_EXTEND_STATUS_BASENAME)
			|| key.Contains(Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_BASENAME))
		{
			var extendStatusNo = Regex.Match(key, @"\d+").Value;
			var orderExtendStatusSettingList = DomainFacade.Instance.OrderExtendStatusSettingService.GetOrderExtendStatusSetting();
			var model = orderExtendStatusSettingList.FirstOrDefault(item =>
				(extendStatusNo == item.ExtendStatusNo.ToString()));
			if (model == null) return string.Empty;

			var format = ValueText.GetValueText(Constants.TABLE_ORDERWORKFLOWSETTING, title, VALUETEXT_DISPLAY_ORDER_EXTEND_STATUS_FORMAT);
			return string.Format(format, model.ExtendStatusNo, " <br /> " + model.ExtendStatusName);
		}

		if (Constants.ORDER_EXTEND_OPTION_ENABLED
			&& Constants.ORDER_EXTEND_ATTRIBUTE_FIELD_LIST.Contains(key))
		{
			var model = DataCacheControllerFacade.GetOrderExtendSettingCacheController().CacheData
				.SettingModels.FirstOrDefault(m => m.SettingId == key);
			if (model == null) return string.Empty;

			var format = ValueText.GetValueText(Constants.TABLE_ORDERWORKFLOWSETTING, title, VALUETEXT_DISPLAY_ORDER_EXTEND_FORMAT);
			return string.Format(format, " <br /> " + model.SettingName, model.SettingId);
		}

		var result = ValueText.GetValueText(Constants.TABLE_ORDERWORKFLOWSETTING, title, key);
		return result;
	}

	/// <summary>
	/// Is display setting
	/// </summary>
	/// <param name="setting">workflow action setting</param>
	/// <returns>True: Display, False: Non-display</returns>
	private bool IsDisplaySetting(DictionaryEntry setting)
	{
		if ((StringUtility.ToEmpty(setting.Key) == Constants.FIELD_ORDERWORKFLOWSETTING_SCHEDULED_SHIPPING_DATE_ACTION)
			|| (StringUtility.ToEmpty(setting.Key) == Constants.FIELD_ORDERWORKFLOWSETTING_SHIPPING_DATE_ACTION)
			|| (StringUtility.ToEmpty(setting.Key) == Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_ACTION))
		{
			return (StringUtility.ToEmpty(setting.Value) != Constants.FLG_ORDERWORKFLOWSETTING_SCHEDULED_SHIPPING_DATE_ACTION_OFF);
		}

		return true;
	}

	/// <summary>
	/// Get sort list
	/// </summary>
	/// <param name="displayKbn">Display kbn</param>
	/// <returns>A list to sort</returns>
	private List<string> GetSortList(string displayKbn)
	{
		var sortList = (displayKbn == Constants.FLG_ORDERWORKFLOWSETTING_DISPLAY_KBN_LINE)
			? new List<string>(SORT_LINE_ACTIONS)
			: new List<string>(SORT_CASSETTE_ACTIONS);
		for (var index = 0; index < Constants.CONST_ORDER_EXTEND_STATUS_DBFIELDS_MAX; index++)
		{
			var invoiceIndex = 0;
			if (displayKbn == Constants.FLG_ORDERWORKFLOWSETTING_DISPLAY_KBN_LINE)
			{
				invoiceIndex = sortList.IndexOf(Constants.FIELD_ORDERWORKFLOWSETTING_INVOICE_STATUS_API);
				sortList.Insert(
					invoiceIndex,
					WorkflowSetting.m_FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE[index]);
				sortList.Insert(
					invoiceIndex,
					WorkflowSetting.m_FIELD_FIXEDPURCHASEWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE[index]);
			}
			else
			{
				invoiceIndex = sortList.IndexOf(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_INVOICE_STATUS_API);
				sortList.Insert(
					invoiceIndex,
					WorkflowSetting.m_FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE[index]);
				sortList.Insert(
					invoiceIndex,
					WorkflowSetting.m_FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE[index]);
			}
		}

		return sortList;
	}

	/// <summary>
	/// Check has reason
	/// </summary>
	/// <param name="values">The action values</param>
	/// <returns>True: If display reason</returns>
	private bool HasReason(string[] values)
	{
		var result = values.Any(value =>
			((value.Split('&')[0] == Constants.FLG_ORDERWORKFLOWSETTING_RETURN_ACTION_ACCEPT_RETURN)
				|| (value.Split('&')[0] == Constants.FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_IS_ALIVE_CHANGE_ACTION_CANCEL)));
		return result;
	}

	/// <summary>
	/// Check display actions
	/// </summary>
	/// <param name="actionKey">Action key</param>
	/// <returns>True: if display, otherwise: false</returns>
	private bool CheckDisplayActions(string actionKey)
	{
		switch (actionKey)
		{
			case Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_REASON_KBN:
			case Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_REASON_MEMO:
			case Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CANCEL_REASON_ID:
			case Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CANCEL_MEMO:
				return false;

			default:
				return true;
		}
	}

	/// <summary>
	/// Get mail name
	/// </summary>
	/// <param name="mailId">Mail id</param>
	/// <returns>Mail name</returns>
	private string GetMailName(string mailId)
	{
		var mailTemplate = DomainFacade.Instance.MailTemplateService.Get(
			this.LoginShopOperatorShopId,
			mailId);
		if (mailTemplate == null) return string.Empty;
		return mailTemplate.MailName;
	}

	/// <summary>Login shop operator shop id</summary>
	private string LoginShopOperatorShopId { get; set; }
}
