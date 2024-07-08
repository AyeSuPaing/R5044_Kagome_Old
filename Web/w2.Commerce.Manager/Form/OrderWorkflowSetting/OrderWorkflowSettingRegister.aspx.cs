/*
=========================================================================================================
  Module      : 注文ワークフロー設定情報登録ページ処理(OrderWorkflowSettingRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common.DataCacheController;
using w2.App.Common.Mail;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment.TriLinkAfterPay.Request;
using w2.App.Common.Order.Workflow;
using w2.App.Common.OrderExtend;
using w2.App.Common.User;
using w2.App.Common.Util;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Extensions;
using w2.Common.Util.TagReplacer;
using w2.Common.Web;
using w2.Domain;
using w2.Domain.CountryLocation;
using w2.Domain.FixedPurchase;
using w2.Domain.OrderWorkflowScenarioSetting;
using w2.Domain.RealShop;
using w2.Domain.UserManagementLevel;

public partial class Form_OrderWorkflowSetting_OrderWorkflowSettingRegister : OrderWorkflowPage
{
	private Dictionary<string, string> m_dicExtendStatus = new Dictionary<string, string>();		// 注文拡張ステータス1～30データバインド用
	protected ListItemCollection m_licExtendStatus = new ListItemCollection();						// 注文拡張ステータス1～30選択肢データバインド用
	protected ListItemCollection m_licExtendStatusChange = new ListItemCollection();				// 注文拡張ステータス1～30更新区分選択肢データバインド用
	protected ListItemCollection m_licMailIds = new ListItemCollection();							// 複数設定時メール送信IDデータバインド用
	protected Dictionary<string, string> m_dicCassetteExtendStatus = new Dictionary<string, string>();	// 複数設定時注文拡張ステータス選択肢データバインド用

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{
		// Must clear Browser Cache because Browser don't display value which has input.
		// Problem is caused when this Form has an error in input validate and come back from Error Page with history.back()
		ClearBrowserCache();

		if ((this.RequestWorkflowType == WorkflowSetting.m_KBN_WORKFLOW_TYPE_FIXEDPURCHASE)
			&& (Constants.FIXEDPURCHASE_OPTION_ENABLED == false))
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FIXEDPURCHASEWORKFLOW_DISABLED);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		//------------------------------------------------------
		// 受注ワークフロー設定取得（更新・コピー新規登録のみ）
		//------------------------------------------------------
		// 受注ワークフロー設定取得
		if ((this.ActionStatus == Constants.ACTION_STATUS_UPDATE)
			|| (this.ActionStatus == Constants.ACTION_STATUS_COPY_INSERT)
			|| (this.ActionStatus == Constants.ACTION_STATUS_COMPLETE))
		{
			DataView dvWorkFlowSetting = new WorkflowSetting().GetOrderWorkflowSetting(
				this.LoginOperatorShopId,
				Request[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_KBN],
				Request[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_NO],
				this.RequestWorkflowType);

			if (dvWorkFlowSetting.Count == 0)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}
			this.OrderWorkflowSetting = new WorkflowSetting(
				dvWorkFlowSetting[0],
				this.LoginOperatorDeptId,
				this.LoginOperatorName,
				(this.RequestWorkflowType == WorkflowSetting.m_KBN_WORKFLOW_TYPE_ORDER)
					? WorkflowSetting.WorkflowTypes.Order : WorkflowSetting.WorkflowTypes.FixedPurchase);
		}
		else
		{
			this.OrderWorkflowSetting = new WorkflowSetting();
		}

		if (!IsPostBack)
		{
			//------------------------------------------------------
			// 画面制御
			//------------------------------------------------------
			InitializeComponents(this.ActionStatus);

			//------------------------------------------------------
			// 表示用値設定処理
			//------------------------------------------------------
			switch (this.ActionStatus)
			{
				// 新規？
				case Constants.ACTION_STATUS_INSERT:

					// データバインド
					DataBind();
					break;

				// 編集？コピー新規？
				case Constants.ACTION_STATUS_UPDATE:
				case Constants.ACTION_STATUS_COPY_INSERT:
				case Constants.ACTION_STATUS_COMPLETE:

					// 抽出検索データ設定
					SetSearchSetting((string)this.OrderWorkflowSetting.GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_SEARCH_SETTING, this.OrderWorkflowSetting.WorkflowType));

					// データバインド
					DataBind();

					// メールテンプレート選択（データバインドのあとに行う必要あり）
					foreach (ListItem li in ddlMailId.Items)
					{
						li.Selected = (li.Value == (string)this.OrderWorkflowSetting.GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_MAIL_ID, WorkflowSetting.WorkflowTypes.Order));
					}

					// Set value fixedpurchase cancel reason
					if (ddlCancelReason.Items.Count > 0)
					{
						ddlCancelReason.SelectedValue = StringUtility.ToEmpty(this.OrderWorkflowSetting.GetValue(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CANCEL_REASON_ID, WorkflowSetting.WorkflowTypes.FixedPurchase));
					}
					break;

				// その他はエラー
				default:
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
					break;
			}

			// 表示制御（データバインドの後に行う必要あり）
			rblUpdateStatusDate_SelectedIndexChanged(sender, e);
			rblReturnExchangeUpdateStatusDate_SelectedIndexChanged(sender, e);
			ddlWorkflowKbn_SelectedIndexChanged(sender, e);
			rblDisplayKbn_SelectedIndexChanged(sender, e);
			cbLastAuthDateOn_OnCheckedChanged(sender, e);
			cbShippingDateOn_OnCheckedChanged(sender, e);
			cbAdvCode_OnCheckedChanged(sender, e);
			cbScheduledShippingDateOn_OnCheckedChanged(sender, e);
			rblEnabledKbn_SelectedIndexChanged(sender, e);
			rblTargetWorkflowType_OnSelectedIndexChanged(sender, e);
			rblFixedPurchaseNextShippingDate_SelectedIndexChanged(sender, e);
			cbResumeDateOn_OnCheckedChanged(sender, e);
			ddlDisplayCount_SelectedIndexChanged(sender, e);
			rblReturnOrderAction_SelectedIndexChanged(sender, e);
			ddlAnotherShippingFlag_SelectedIndexChanged(sender, e);
			// データバインド
			DataBind();

			// 後から全体データバインドされるとリピータ内チェックボックスバインドが正常に動作しないためこのタイミング
			SetOrderExtend();
		}

		// スクリプトを起動して表示区分セット
		RunScriptSetVisibility();

		// Retore form data if come back from error page
		KeepFormData.AutoRestoreFormData = this.IsBackFromErrorPage;
	}

	/// <summary>
	/// 表示コンポーネント初期化
	/// </summary>
	/// <param name="actionStatus">アクションステータス</param>
	private void InitializeComponents(string actionStatus)
	{
		// 編集表示？
		if ((actionStatus == Constants.ACTION_STATUS_UPDATE)
			|| (actionStatus == Constants.ACTION_STATUS_COMPLETE))
		{
			// 更新ボタン表示
			trEdit.Visible = true;
			btnUpdateTop.Visible
				= btnUpdateBottom.Visible = true;

			// 削除ボタン表示
			// ダッシュボード未出荷注文表示用ワークフローの場合、削除ボタンを非表示にする
			var workflowNo = this.OrderWorkflowSetting.GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_NO);
			var workflowKbn = this.OrderWorkflowSetting.GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_KBN);
			var blDeletable = ((workflowKbn != Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_KBN_SYSTEM)
				|| (workflowNo != Constants.CONST_SUMMARY_REPORT_UNSHIPPED_WORKFLOW_NO));

			btnDeleteTop.Visible
				= btnDeleteBottom.Visible = blDeletable;

			// コピー新規登録ボタン表示
			btnCopyInsertTop.Visible
				= btnCopyInsertBottom.Visible = true;

			// 作成日表示
			lDateCreated.Text = DateTimeUtility.ToStringForManager(
				this.OrderWorkflowSetting.GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_DATE_CREATED),
				DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter);
			// 更新日表示
			lDateChanged.Text = DateTimeUtility.ToStringForManager(
				this.OrderWorkflowSetting.GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_DATE_CHANGED),
				DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter);
			// 最終更新者表示
			lLastChanged.Text = this.OrderWorkflowSetting.GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_LAST_CHANGED).ToString();

			// 最大件数に達していたらコピー新規登録ボタンを非表示にする
			if (IsLessOrderWorkflowSettingThenMaxCount() == false)
			{
				btnCopyInsertTop.Visible = btnCopyInsertBottom.Visible = false;
			}
		}
		// 新規登録？
		else if ((actionStatus == Constants.ACTION_STATUS_INSERT)
			|| (actionStatus == Constants.ACTION_STATUS_COPY_INSERT))
		{
			// 登録ボタン表示
			trRegister.Visible = true;
			btnInsertTop.Visible
				= btnInsertBottom.Visible = true;

			// 最大件数に達していたら通知を表示して登録ボタンを無効にする
			if (IsLessOrderWorkflowSettingThenMaxCount() == false)
			{
				lMessage.Text = string.Format(
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERWORKFLOWSETTING_REGISTER_OVER_MAX_COUNT),
					Constants.ORDERWORKFLOWSETTING_MAXCOUNT);
				divComp.Visible = true;
				btnInsertTop.Enabled = btnInsertBottom.Enabled = false;
			}
		}

		// 登録/更新完了?
		if (actionStatus == Constants.ACTION_STATUS_COMPLETE)
		{
			lMessage.Text = (string)Session[Constants.SESSION_KEY_ERROR_MSG];
			divComp.Visible = true;
		}

		//------------------------------------------------------
		// バインド用データ作成
		//------------------------------------------------------
		m_licMailIds.Add(
			new ListItem(
				Constants.TAG_REPLACER_DATA_SCHEMA.GetValue(
					"@@DispText.auto_text.unspecified@@",
					Constants.GLOBAL_OPTION_ENABLE
						? Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE
						: "")
				, ""));
		m_licMailIds.AddRange(
			GetMailTemplateUtility.GetMailTemplateForOrder(this.LoginOperatorShopId).Select(mail => new ListItem(mail.MailName, mail.MailId)).ToArray());

		//------------------------------------------------------
		// 基本設定
		//------------------------------------------------------
		// ワークフロー区分
		ddlWorkflowKbn.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_KBN))
		{
			ddlWorkflowKbn.Items.Add(li);
		}

		// 表示件数
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDERWORKFLOWSETTING_DISPLAY_COUNT))
		{
			ddlDisplayCount.Items.Add(li);
		}

		// 一覧表示区分
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDERWORKFLOWSETTING_DISPLAY_KBN + "_setting"))
		{
			rblDisplayKbn.Items.Add(li);
		}

		// 追加検索フラグ
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDERWORKFLOWSETTING_ADDITIONAL_SEARCH_FLG))
		{
			if (this.OrderWorkflowSetting.WorkflowType == WorkflowSetting.WorkflowTypes.FixedPurchase)
			{
				if (li.Value != Constants.FLG_ORDERWORKFLOWSETTING_ADDITIONAL_SEARCH_FLG_ON)
				{
					rblAdditionalSearchFlg.Items.Add(li);
				}
			}
			else
			{
				if (li.Value != Constants.FLG_ORDERWORKFLOWSETTING_ADDITIONAL_SEARCH_FIXEDPURCHASE_FLG_ON)
				{
					rblAdditionalSearchFlg.Items.Add(li);
				}

			}
		}
		// ワークフロー対象
		rblTargetWorkflowType.Items.AddRange(
			ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDERWORKFLOWSETTING_TARGET_TYPE)
				.Cast<ListItem>()
				.ToArray());
		rblTargetWorkflowType.SelectedValue = Constants.FLG_ORDERWORKFLOWSETTING_TARGET_WORKFLOW_TYPE_ORDER;

		//------------------------------------------------------
		// 抽出検索条件設定
		//------------------------------------------------------
		// 注文区分(受注)
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_KBN))
		{
			// モバイルデータの表示OPがOFFの場合は注文区分がモバイル注文を追加しない
			if ((Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED == false)
				&& (li.Value == Constants.FLG_ORDER_ORDER_KBN_MOBILE)) continue;
			cblOrderKbn.Items.Add(li);
		}

		// 注文者区分
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDEROWNER, Constants.FIELD_ORDEROWNER_OWNER_KBN))
		{
			// モバイルデータの表示と非表示OFF時はMB_USER(モバイル会員一般)とMB_GEST(モバイルゲスト)区分を追加しない
			if ((Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED == false)
				&& ((li.Value == Constants.FLG_ORDEROWNER_OWNER_KBN_MOBILE_USER)
					|| (li.Value == Constants.FLG_ORDEROWNER_OWNER_KBN_MOBILE_GUEST))) continue;
			cblOwnerKbn.Items.Add(li);
		}

		// 並び順(受注)
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.REQUEST_KEY_SORT_KBN))
		{
			ddlSortKbn.Items.Add(li);
		}

		// 注文ステータス
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDER_ORDER_STATUS))
		{
			// 在庫引き当済みは実在庫利用の時のみ、また、未指定は追加しない
			if ((Constants.REALSTOCK_OPTION_ENABLED || (li.Value != Constants.FLG_ORDER_ORDER_STATUS_STOCK_RESERVED))
				&& (li.Value != Constants.FLG_ORDER_ORDER_STATUS_UNKNOWN))
			{
				cblOrderStatus.Items.Add(li);
			}
		}

		// ステータス更新日
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, WorkflowSetting.m_FIELD_UPDATE_STATUS))
		{
			// 在庫引き当済みは実在庫利用の時のみ
			if (Constants.REALSTOCK_OPTION_ENABLED
				|| (li.Value != Constants.FIELD_ORDER_ORDER_STOCKRESERVED_DATE))
			{
				ddlUpdateStatusDate.Items.Add(li);
			}
		}

		if (Constants.STORE_PICKUP_OPTION_ENABLED)
		{
			foreach (var li in ValueText.GetValueItemArray(Constants.TABLE_ORDER, WorkflowSetting.REALSHOP_AND_STOREPICKUP))
			{
				ddlUpdateStatusDate.Items.Add(li);
			}
		}

		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, WorkflowSetting.m_FIELD_UPDATE_STATUS_DAY))
		{
			rblUpdateStatusDate.Items.Add(li);
		}
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, WorkflowSetting.UPDATE_STATUS_DATE_FROM))
		{
			ddlUpdateStatusDateFrom.Items.Add(li);
		}
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, WorkflowSetting.UPDATE_STATUS_DATE_TO))
		{
			ddlUpdateStatusDateTo.Items.Add(li);
		}
		for (int i = 1; i <= 60; i++)
		{
			ddlUpdateStatusDateFrom.Items.Add(new ListItem(i.ToString("0"), i.ToString()));
			ddlUpdateStatusDateTo.Items.Add(new ListItem(i.ToString("0"), i.ToString()));
		}

		// 期間(時)に設定
		for (int i = 0; i <= 23; i++)
		{
			ddlUpdateStatusHourTo.Items.Add(new ListItem(i.ToString("00")));
			ddlReturnExchangeUpdateStatusHourTo.Items.Add(new ListItem(i.ToString("00")));
		}
		// 期間(分)に設定
		for (int i = 0; i <= 59; i++)
		{
			ddlUpdateStatusMinuteTo.Items.Add(new ListItem(i.ToString("00")));
			ddlReturnExchangeUpdateStatusMinuteTo.Items.Add(new ListItem(i.ToString("00")));
		}
		// 期間(秒)に設定
		for (int i = 0; i <= 59; i++)
		{
			ddlUpdateStatusSecondTo.Items.Add(new ListItem(i.ToString("00")));
			ddlReturnExchangeUpdateStatusSecondTo.Items.Add(new ListItem(i.ToString("00")));
		}

		// 入金ステータス
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS))
		{
			if (li.Value != Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_UNKNOWN)	// 指定無し以外を追加
			{
				cblOrderPaymentStatus.Items.Add(li);
			}
		}

		// 督促ステータス
		if(Constants.DEMAND_OPTION_ENABLE)
		{
			trDemandStatus.Visible = true;
			foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_DEMAND_STATUS))
			{
				if (li.Value != Constants.FLG_ORDER_DEMAND_STATUS_UNKNOWN)	// 指定無し以外を追加
				{
					cblDemandStatus.Items.Add(li);
				}
			}
		}

		// 領収書希望フラグ（注文）
		cblOrderReceiptFlg.Items.AddRange(
			ValueText.GetValueItemArray(Constants.TABLE_ORDER, Constants.FIELD_ORDER_RECEIPT_FLG));
		// 領収書希望フラグ（定期）
		cblFixedPurchaseReceiptFlg.Items.AddRange(
			ValueText.GetValueItemArray(Constants.TABLE_FIXEDPURCHASE, Constants.FIELD_FIXEDPURCHASE_RECEIPT_FLG));

		// 領収書出力フラグ
		cblOrderReceiptOutputFlg.Items.AddRange(
			ValueText.GetValueItemArray(Constants.TABLE_ORDER, Constants.FIELD_ORDER_RECEIPT_OUTPUT_FLG));

		// 拡張ステータス
		DataView orderExtendStatusSettingList = GetOrderExtendStatusSettingList(this.LoginOperatorShopId);
		rOrderExtendStatusForSearch.DataSource
			= rFixedPurchaseExtendStatusForSearch.DataSource = orderExtendStatusSettingList;

		// 在庫引当ステータス
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_STOCKRESERVED_STATUS))
		{
			cblOrderStockRservedStatus.Items.Add(li);
		}

		// 出荷ステータス
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_SHIPPED_STATUS))
		{
			cblOrderShippedStatus.Items.Add(li);
		}

		// 決済種別（受注）
		foreach (DataRowView payment in GetPaymentValidList(this.LoginOperatorShopId))
		{
			cblOrderPaymentKbn.Items.Add(new ListItem(WebSanitizer.HtmlEncode(payment[Constants.FIELD_PAYMENT_PAYMENT_NAME]), (string)payment[Constants.FIELD_PAYMENT_PAYMENT_ID]));

			// 返品交換
			cblReturnOrderPaymentKbn.Items.Add(new ListItem(WebSanitizer.HtmlEncode(payment[Constants.FIELD_PAYMENT_PAYMENT_NAME]), (string)payment[Constants.FIELD_PAYMENT_PAYMENT_ID]));
		}

		// 外部決済ステータス：クレジットカードチェックボックスリスト作成
		var extendStatusListForCreditCard = CreateExtendStatusListForCreditCard(
			ValueText.GetValueItemArray(Constants.TABLE_ORDER, Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS));
		cblOrderExternalPaymentStatusByCard.Items.AddRange(extendStatusListForCreditCard);

		// 返品交換：外部決済ステータス：クレジットカードチェックボックスリスト作成
		var returnExtendStatusListForCreditCard = CreateExtendStatusListForCreditCard(
			ValueText.GetValueItemArray(Constants.TABLE_ORDER, Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS));
		cblReturnOrderExternalPaymentStatusByCard.Items.AddRange(returnExtendStatusListForCreditCard);

		// 外部決済ステータス：コンビニ(後払い)チェックボックスリスト作成
		var extendStatusListForCVS = CreateExtendStatusListForCVS(
			ValueText.GetValueItemArray(Constants.TABLE_ORDER, Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS));
		cblOrderExternalPaymentStatusByCVS.Items.AddRange(extendStatusListForCVS);

		// 返品交換：外部決済ステータス：コンビニ(後払い)チェックボックスリスト作成
		var returnExtendStatusListForCVS = CreateExtendStatusListForCVS(
			ValueText.GetValueItemArray(Constants.TABLE_ORDER, Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS));
		cblReturnOrderExternalPaymentStatusByCVS.Items.AddRange(returnExtendStatusListForCVS);

		// 外部決済ステータス：台湾後払いチェックボックスリスト作成
		var extendStatusListForTryLinkAfterPay = CreateExtendStatusListForTryLinkAfterPay(
			ValueText.GetValueItemArray(Constants.TABLE_ORDER, Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS));
		cblOrderExternalPaymentStatusByTryLinkAfterPay.Items.AddRange(extendStatusListForTryLinkAfterPay);

		// 返品交換：外部決済ステータス：台湾後払いチェックボックスリスト作成
		var returnExtendStatusListForTryLinkAfterPay = CreateExtendStatusListForTryLinkAfterPay(
			ValueText.GetValueItemArray(Constants.TABLE_ORDER, Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS));
		cblReturnOrderExternalPaymentStatusByTryLinkAfterPay.Items.AddRange(returnExtendStatusListForTryLinkAfterPay);

		// EcPay
		var extendStatusListForEcPay = ValueText.GetValueItemArray(Constants.TABLE_PAYMENT, Constants.VALUETEXT_PARAM_ECPAY_PAYMENT_TYPE);
		cblOrderExternalPaymentStatusByEcPay.Items.AddRange(extendStatusListForEcPay);

		// NewebPay
		var extendStatusListForNewebPay = ValueText.GetValueItemArray(Constants.TABLE_PAYMENT, Constants.VALUETEXT_PARAM_NEWEB_PAYMENT_TYPE);
		cblOrderExternalPaymentStatusByNewebPay.Items.AddRange(extendStatusListForNewebPay);

		// 配送先：国
		var shippingAvailableCountry = new CountryLocationService().GetShippingAvailableCountry();
		cblShippingCountry.Items.AddRange(shippingAvailableCountry.Select(c => new ListItem(c.CountryName, c.CountryIsoCode)).ToArray());

		// Get external payment status list for all payment
		var externalPaymentStatusList = ValueText.GetValueItemArray(Constants.TABLE_ORDER, Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS);
		cblReturnOrderExternalPaymentStatusForAllPayment.Items.AddRange(externalPaymentStatusList);
		cblOrderExternalPaymentStatusForAllPayment.Items.AddRange(externalPaymentStatusList);

		// 配送区分（受注）
		foreach (DataRowView shipping in GetShopShippingsAll(this.LoginOperatorShopId))
		{
			cblOrderShippingKbn.Items.Add(new ListItem(WebSanitizer.HtmlEncode(shipping[Constants.FIELD_SHOPSHIPPING_SHOP_SHIPPING_NAME]), (string)shipping[Constants.FIELD_SHOPSHIPPING_SHIPPING_ID]));
		}

		// 配送料の別途見積もりフラグ
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG))
		{
			cblSeparateEstimatesFlg.Items.Add(li);
		}

		// 配送伝票番号
		cblShippingCheckNo.Items.AddRange(
			ValueText.GetValueItemArray(
				Constants.TABLE_ORDERSHIPPING,
				Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO + (Constants.GIFTORDER_OPTION_ENABLED ? Constants.CONST_FIELD_EXTEND_FOR_GIFT : string.Empty)));

		// 出荷後変更区分
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDER_SHIPPED_CHANGED_KBN))
		{
			cblShippedChangedKbn.Items.Add(li);
		}

		// サイト
		cblSiteName.Items.Add(
			new ListItem(
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_SITENAME,
					Constants.VALUETEXT_PARAM_OWNSITENAME,
					Constants.FLG_ORDER_MALL_ID_OWN_SITE),
				Constants.FLG_ORDER_MALL_ID_OWN_SITE));
		foreach (DataRowView drvMallCooperationSetting in GetMallCooperationSettingList(this.LoginOperatorShopId))
		{
			cblSiteName.Items.Add(
				new ListItem(
					CreateSiteNameOnly(
						(string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID],
						(string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_NAME]),
					(string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID]));
		}

		// モール連携ステータス
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_MALL_LINK_STATUS))
		{
			cblMallLinkStatus.Items.Add(li);
		}

		// 楽天ポイント利用方法
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, WorkflowSetting.m_FIELD_RAKUTEN_POINT_USE_TYPE))
		{
			cblRakutenPointUseType.Items.Add(li);
		}

		// 外部連携ステータス
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_EXTERNAL_IMPORT_STATUS))
		{
			cblExternalImportStatus.Items.Add(li);
		}

		// 返品交換区分
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDER_RETURN_EXCHANGE_KBN))
		{
			cblReturnExchangeKbn.Items.Add(li);
		}

		// 返品交換都合区分
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDER_RETURN_EXCHANGE_REASON_KBN))
		{
			cblReturnExchangeReasonKbn.Items.Add(li);
		}

		// 並び順(返品交換用)
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.REQUEST_KEY_RETURN_EXCHANGE_SORT_KBN))
		{
			ddlReturnExchangeSortKbn.Items.Add(li);
		}

		// 返品交換ステータス
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_STATUS))
		{
			cblOrderReturnExchangeStatus.Items.Add(li);
		}

		// 返金ステータス
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDER_ORDER_REPAYMENT_STATUS))
		{
			cblOrderRepaymentStatus.Items.Add(li);
		}

		// 返品交換返金更新日
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, WorkflowSetting.m_FIELD_RETURN_EXCHANGE_UPDATE_STATUS))
		{
			ddlReturnExchangeUpdateStatusDate.Items.Add(li);
		}
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, WorkflowSetting.m_FIELD_RETURN_EXCHANGE_UPDATE_STATUS_DAY))
		{
			rblReturnExchangeUpdateStatusDate.Items.Add(li);
		}
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, WorkflowSetting.UPDATE_STATUS_DATE_FROM))
		{
			ddlReturnExchangeUpdateStatusDateFrom.Items.Add(li);
		}
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, WorkflowSetting.UPDATE_STATUS_DATE_TO))
		{
			ddlReturnExchangeUpdateStatusDateTo.Items.Add(li);
		}
		for (int i = 1; i <= 60; i++)
		{
			ddlReturnExchangeUpdateStatusDateFrom.Items.Add(new ListItem(i.ToString("0"), i.ToString()));
			ddlReturnExchangeUpdateStatusDateTo.Items.Add(new ListItem(i.ToString("0"), i.ToString()));
		}

		// デジタルコンテンツ商品
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDER_DIGITAL_CONTENTS_FLG))
		{
			cblDigitalContentsFlg.Items.Add(li);
		}

		// 定期購買注文
		cblFixedPurchase.Items.AddRange(
			ValueText.GetValueItemArray(
				Constants.TABLE_ORDERWORKFLOWSETTING,
				Constants.VALUETEXT_PARAM_ORDERWORKFLOWSETTING_FIXED_PURCHASE));

		// 注文メモ
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_MEMO))
		{
			cblOrderMemoFlg.Items.Add(li);
		}
		// 管理メモ（受注）
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_MANAGEMENT_MEMO))
		{
			cblOrderManagementMemoFlg.Items.Add(li);
		}
		// 配送メモ
		cblShippingMemoFlg.Items.AddRange(
			ValueText.GetValueItemArray(Constants.TABLE_ORDER, Constants.FIELD_ORDER_SHIPPING_MEMO));
		// 決済連携メモ
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_PAYMENT_MEMO))
		{
			cblOrderPaymentMemoFlg.Items.Add(li);
		}
		// 外部連携メモ
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_RELATION_MEMO))
		{
			cblOrderRelationMemoFlg.Items.Add(li);
		}
		// 商品付帯情報
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERITEM, Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS))
		{
			cblProductOptionFlg.Items.Add(li);
		}
		// ギフト購入フラグ
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_GIFT_FLG))
		{
			cblGiftFlg.Items.Add(li);
		}
		// 別出荷フラグ
		ddlAnotherShippingFlag.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_ANOTHER_SHIPPING_FLG))
		{
			if ((li.Value == Constants.FLG_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG)
				&& (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED == false)) continue;

			if ((Constants.STORE_PICKUP_OPTION_ENABLED == false)
				&& (li.Value == Constants.FLG_ORDERSHIPPING_SHIPPING_STORE_PICKUP_FLG)) continue;

			ddlAnotherShippingFlag.Items.Add(li);
		}

		// 請求書同梱フラグ
		cblInvoiceBundleFlg.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ORDER, Constants.FIELD_ORDER_INVOICE_BUNDLE_FLG));
		// 配送方法
		cblShippingMethod.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD));
		// 配送会社
		foreach (var deliveryCompany in this.DeliveryCompanyList)
		{
			cblDeliveryCompany.Items.Add(new ListItem(this.DeliveryCompanyList.First(i => i.DeliveryCompanyId == deliveryCompany.DeliveryCompanyId).DeliveryCompanyName, deliveryCompany.DeliveryCompanyId));
		}
		// ユーザー管理レベル
		var models = new UserManagementLevelService().GetAllList();
		cblUserManagementLevel.Items.AddRange(
			models.Select(m => new ListItem(m.UserManagementLevelName, m.UserManagementLevelId)).ToArray());
		// 注文種別
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, WorkflowSetting.m_TARGET_ORDER_TYPE))
		{
			cblOrder.Items.Add(li);
		}

		// 注文区分（定期）
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_KBN))
		{
			// モバイルデータの表示OPがOFFの場合は注文区分がモバイル注文を追加しない
			if ((Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED == false)
				&& (li.Value == Constants.FLG_ORDER_ORDER_KBN_MOBILE)) continue;
			cblFixedPurchaseOrderKbn.Items.Add(li);
		}
		// 並び順(定期)
		ddlFixedPurchaseSortKbn.Items.AddRange(
			ValueText.GetValueItemArray(Constants.TABLE_FIXEDPURCHASE, Constants.REQUEST_KEY_SORT_KBN));
		// 定期購入ステータス
		cblFixedPurchaseStatus.Items.AddRange(
			ValueText.GetValueItemArray(Constants.TABLE_ORDERWORKFLOWSETTING, "fixed_purchase_status"));
		// 定期購入区分
		cblFixedPurchaseKbn.Items.AddRange(
			ValueText.GetValueItemArray(Constants.TABLE_ORDERWORKFLOWSETTING, "fixed_purchase_kbn"));
		// 決済種別（定期）
		cblFixedPurchasePaymentKbn.Items.AddRange(
			GetPaymentValidList(this.LoginOperatorShopId)
				.Cast<DataRowView>()
				.Select(payment => new ListItem(WebSanitizer.HtmlEncode(payment[Constants.FIELD_PAYMENT_PAYMENT_NAME]), (string)payment[Constants.FIELD_PAYMENT_PAYMENT_ID]))
				.Where(item => (item.Value != Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY))
				.ToArray());
		// 配送区分（定期）
		cblFixedPurchaseShippingKbn.Items.AddRange(
			GetShopShippingsAll(this.LoginOperatorShopId)
				.Cast<DataRowView>()
				.Select(shipping => new ListItem(WebSanitizer.HtmlEncode(shipping[Constants.FIELD_SHOPSHIPPING_SHOP_SHIPPING_NAME]), (string)shipping[Constants.FIELD_SHOPSHIPPING_SHIPPING_ID]))
				.ToArray());
		// 管理メモ（定期）
		cblFixedPurchaseManagementMemoFlg.Items.AddRange(
			ValueText.GetValueItemArray(Constants.TABLE_ORDER, Constants.FIELD_ORDER_MANAGEMENT_MEMO));
		// 配送メモ（定期）
		cblFpShippingMemoFlg.Items.AddRange(
			ValueText.GetValueItemArray(Constants.TABLE_FIXEDPURCHASE, Constants.FIELD_FIXEDPURCHASE_SHIPPING_MEMO));
		//------------------------------------------------------
		// アクション設定（一行表示用）
		//------------------------------------------------------
		// 注文ステータス（一行表示用）
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_STATUS_CHANGE))
		{
			// 在庫引き当済みは実在庫利用の時のみ
			if (Constants.REALSTOCK_OPTION_ENABLED
				|| (li.Value != Constants.FLG_ORDERWORKFLOWSETTING_STATUS_CHANGE_STCOK_RESERVED))
			{
				rblOrderStatusChange.Items.Add(li);
			}
		}

		// 商品在庫変更（一行表示用）
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDERWORKFLOWSETTING_PRODUCT_REALSTOCK_CHANGE))
		{
			rblProductRealStockChange.Items.Add(li);
		}

		// 入金ステータス（一行表示用）
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDERWORKFLOWSETTING_PAYMENT_STATUS_CHANGE))
		{
			rblPaymentStatusChange.Items.Add(li);
		}

		// 督促ステータス（一行表示用）
		rblDemandStatusChange.Items.Add("");
		if (Constants.DEMAND_OPTION_ENABLE)
		{
			trDemandStatusChange.Visible = true;
			rblDemandStatusChange.Items.Clear();
			foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDERWORKFLOWSETTING_DEMAND_STATUS_CHANGE))
			{
				rblDemandStatusChange.Items.Add(li);
			}
		}

		// 拡張ステータス（一行表示用）
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, "order_extend_status_change"))
		{
			m_licExtendStatusChange.Add(li);
		}
		rOrderExtendStatusForAction.DataSource
			= rFixedPurchaseExtendStatusForAction.DataSource = orderExtendStatusSettingList;

		// 返品交換ステータス（一行表示用）
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_EXCHANGE_STATUS_CHANGE))
		{
			rblReturnExchangeStatusChange.Items.Add(li);
		}

		//返金ステータス（一行表示用）
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDERWORKFLOWSETTING_REPAYMENT_STATUS_CHANGE))
		{
			rblRepaymentStatusChange.Items.Add(li);
		}

		// メールテンプレート情報（一行表示用）
		foreach (ListItem li in m_licMailIds)
		{
			ddlMailId.Items.Add(new ListItem(li.Text, li.Value));
		}

		// 出荷予定日（一行表示用）
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDERWORKFLOWSETTING_SCHEDULED_SHIPPING_DATE_ACTION))
		{
			rblScheduledShippingDateAction.Items.Add(li);
		}

		// 配送希望日（一行表示用）
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDERWORKFLOWSETTING_SHIPPING_DATE_ACTION))
		{
			rblShippingDateAction.Items.Add(li);
		}

		// Order Invoice Status Change
		var invoiceStatusItems = ValueText.GetValueItemArray(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDERWORKFLOWSETTING_INVOICE_STATUS_CHANGE);
		rblInvoiceStatusChange.Items.AddRange(invoiceStatusItems);
		rUpdateOrderInvoiceStatusSettingList.DataSource = invoiceStatusItems
			.Where(item => string.IsNullOrEmpty(item.Value) == false)
			.ToDictionary(item => item.Value, item => item.Text);

		// Order Invoice Status Api
		var invoiceStatusApiItems = ValueText.GetValueItemArray(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDERWORKFLOWSETTING_INVOICE_STATUS_API);
		rblInvoiceStatusApi.Items.AddRange(invoiceStatusApiItems);
		rUpdateOrderInvoiceStatusCallApiSettingList.DataSource = invoiceStatusApiItems
			.Where(item => string.IsNullOrEmpty(item.Value) == false)
			.ToDictionary(item => item.Value, item => item.Text);

		//定期状態変更（一行表示用）
		rblFixedPurchaseIsAliveChange.Items.AddRange(
			ValueText.GetValueItemList(
				Constants.TABLE_ORDERWORKFLOWSETTING,
				Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_IS_ALIVE_CHANGE)
				.Cast<ListItem>()
				.ToArray());
		//定期決済ステータス（一行表示用）
		rblFixedPurchasePaymentStatusChange.Items.AddRange(
			ValueText.GetValueItemList(
				Constants.TABLE_ORDERWORKFLOWSETTING,
				Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_PAYMENT_STATUS_CHANGE)
				.Cast<ListItem>()
				.ToArray());

		//定期次回配送日（一行表示用）
		rblFixedPurchaseNextShippingDate.Items.AddRange(
			ValueText.GetValueItemList(
				Constants.TABLE_ORDERWORKFLOWSETTING,
				Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_NEXT_SHIPPING_DATE_CHANGE)
				.Cast<ListItem>()
				.ToArray());
		//定期次々回配送日（一行表示用）
		rblFixedPurchaseNextNextShippingDate.Items.AddRange(
			ValueText.GetValueItemList(
				Constants.TABLE_ORDERWORKFLOWSETTING,
				Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_NEXT_NEXT_SHIPPING_DATE_CHANGE)
				.Cast<ListItem>()
				.ToArray());

		//定期配送不可エリア停止（一行表示用）
		rblFixedPurchaseStopUnavailableShippingAreaChange.Items.AddRange(
			ValueText.GetValueItemList(
				Constants.TABLE_ORDERWORKFLOWSETTING,
				Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_STOP_UNAVAILABLE_SHIPPING_AREA_CHANGE)
				.Cast<ListItem>()
				.ToArray());

		// 領収書出力フラグ（一行表示用）
		rblOrderReceiptOutputFlgChange.Items.AddRange(
			ValueText.GetValueItemArray(
				Constants.TABLE_ORDERWORKFLOWSETTING,
				Constants.FIELD_ORDERWORKFLOWSETTING_RECEIPT_OUTPUT_FLG_CHANGE));

		//------------------------------------------------------
		// アクション設定（カセット表示用）
		//------------------------------------------------------
		// 注文ステータス（カセット表示用）
		var cassetteOrderStatusChange = new Dictionary<string, string>();
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_STATUS_CHANGE))
		{
			// 在庫引き当済みは実在庫利用の時のみ
			if (Constants.REALSTOCK_OPTION_ENABLED
				|| (li.Value != Constants.FLG_ORDERWORKFLOWSETTING_STATUS_CHANGE_STCOK_RESERVED))
			{
				if (li.Value != "")	// 「指定しない」を対象外とする
				{
					cassetteOrderStatusChange.Add(li.Value, li.Text);
				}
			}
		}
		rUpdateOrderStatusSettingList.DataSource = cassetteOrderStatusChange;

		if (Constants.STORE_PICKUP_OPTION_ENABLED)
		{
			var cassetteStorePickStatusChange = new Dictionary<string, string>();
			foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING,
				Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_STOREPICKUP_STATUS_CHANGE))
			{
				if (string.IsNullOrEmpty(li.Value) == false)
				{
					cassetteStorePickStatusChange.Add(li.Value, li.Text);
				}
			}
			rUpdateStorePickupStatusSettingList.DataSource = cassetteStorePickStatusChange;
		}

		// 実在庫連動処理（カセット表示用）
		Dictionary<string, string> cassetteProductRealStockChange = new Dictionary<string, string>();
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDERWORKFLOWSETTING_PRODUCT_REALSTOCK_CHANGE))
		{
			if (li.Value != "")	// 「指定しない」を対象外とする
			{
				cassetteProductRealStockChange.Add(li.Value, li.Text);
			}
		}
		rUpdateProductRealStockSettingList.DataSource = cassetteProductRealStockChange;

		// 入金ステータス（カセット表示用）
		Dictionary<string, string> cassettePaymentStatusChange = new Dictionary<string, string>();
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDERWORKFLOWSETTING_PAYMENT_STATUS_CHANGE))
		{
			if (li.Value != "")	// 「指定しない」を対象外とする
			{
				cassettePaymentStatusChange.Add(li.Value, li.Text);
			}
		}
		rUpdatePaymentStatusSettingList.DataSource = cassettePaymentStatusChange;

		// 外部決済連携（カセット表示用）
		Dictionary<string, string> cassetteExternalPaymentAction = new Dictionary<string, string>();
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION))
		{
			if (li.Value != "")	// 「指定しない」を対象外とする
			{
				// 実売上処理が無効な場合は設定しない
				if ((Constants.PAYMENT_CARD_REALSALES_ENABLED == false)
					&& ((li.Value == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_ZEUS_CREDITCARD_PAYMENT)
						|| (li.Value == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_SBPS_CREDITCARD_SALES)
						|| (li.Value == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_ZCOM_CREDITCARD_SALES)
						|| (li.Value == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_ESCOTT_CREDITCARD_SALES)
						|| (li.Value == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_VERITRANS_CREDITCARD_SALES)))
				{
					continue;
				}
				// ドコモケータイ払いの売上連動が無効な場合は設定しない
				if ((Constants.PAYMENT_SETTING_DOCOMOKETAI_REALSALES_ENABLED == false)
					&& (li.Value == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_DOCOMO_PAYMENT))
				{
					continue;
				}
				// S!まとめて支払い決済の売上連動が無効な場合は設定しない
				if ((Constants.PAYMENT_SETTING_SMATOMETE_REALSALES_ENABLED == false)
					&& (li.Value == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_SOFTBANK_PAYMENT))
				{
					continue;
				}
				// SBPSキャリア決済の売上連動が無効な場合は設定しない
				if (((Constants.PAYMENT_SETTING_SBPS_SOFTBANKKETAI_REALSALES_ENABLED == false)
						&& (li.Value == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_SBPS_SOFTBANKKETAI_PAYMENT))
					|| ((Constants.PAYMENT_SETTING_SBPS_DOCOMOKETAI_REALSALES_ENABLED == false)
						&& (li.Value == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_SBPS_DOCOMOKETAI_PAYMENT))
					|| ((Constants.PAYMENT_SETTING_SBPS_AUKANTAN_REALSALES_ENABLED == false)
						&& (li.Value == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_SBPS_AUKANTAN_PAYMENT))
					|| ((Constants.PAYMENT_SETTING_SBPS_RECRUIT_REALSALES_ENABLED == false)
						&& (li.Value == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_SBPS_RECRUIT_PAYMENT))
					|| ((Constants.PAYMENT_SETTING_SBPS_RAKUTEN_ID_REALSALES_ENABLED == false)
						&& (li.Value == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_SBPS_RAKUTEN_ID_PAYMENT)))
				{
					continue;
				}
				// 後払いがGMOじゃない場合は設定しない
				if ((Constants.PAYMENT_CVS_DEF_KBN != Constants.PaymentCvsDef.Gmo) && (li.Value == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_GMO_CVS_DEF_SHIP))
				{
					continue;
				}
				// 後払いがAtodeneじゃない場合は設定しない
				if ((Constants.PAYMENT_CVS_DEF_KBN != Constants.PaymentCvsDef.Atodene) && (li.Value == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_ATODENE_CVS_DEF_SHIP))
				{
					continue;
				}

				// Amazon Payのオプションが無効の場合は設定しない
				if ((Constants.AMAZON_PAYMENT_OPTION_ENABLED == false)
					&& (li.Value == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_AMAZON_PAYMENT))
				{
					continue;
				}
				// PayPalオプションが無効の場合は設定しない
				if ((Constants.PAYPAL_LOGINPAYMENT_ENABLED == false)
					&& (li.Value == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_PAYPAL_PAYMENT))
				{
					continue;
				}
				// LINE PAY 翌月払い
				if ((Constants.PAYMENT_LINEPAY_OPTION_ENABLED == false)
					&& (li.Value == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_LINE_PAYMENT))
				{
					continue;
				}

				// atone翌月払い
				if ((Constants.PAYMENT_ATONEOPTION_ENABLED == false)
					&& (li.Value == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_ATONE_PAYMENT))
				{
					continue;
				}

				// aftee翌月払い
				if ((Constants.PAYMENT_AFTEEOPTION_ENABLED == false)
					&& (li.Value == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_AFTEE_PAYMENT))
				{
					continue;
				}
				// NP後払い出荷報告 オプションが無効の場合は設定しない
				if ((Constants.PAYMENT_NP_AFTERPAY_OPTION_ENABLED == false)
					&& (li.Value == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_NP_AFTERPAY_SHIP))
				{
					continue;
				}
				// EcPay
				if ((Constants.ECPAY_PAYMENT_OPTION_ENABLED == false)
					&& (li.Value == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_EC_PAYMENT))
				{
					continue;
				}
				// GMO deffered payment reissue invoice
				if ((Constants.PAYMENT_CVS_DEF_KBN != Constants.PaymentCvsDef.Gmo)
					&& (li.Value == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_GMO_CVS_DEF_INVOICE_REISSUE))
				{
					continue;
				}
				// NewebPay
				if ((Constants.NEWEBPAY_PAYMENT_OPTION_ENABLED == false)
					&& (li.Value == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_NEWEB_PAYMENT))
				{
					continue;
				}
				// PayPay
				if ((Constants.PAYMENT_PAYPAYOPTION_ENABLED == false)
					&& (li.Value == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_PAYPAY_PAYMENT))
				{
					continue;
				}
				// Rakuten
				if ((Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.Rakuten)
					&& (li.Value == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_RAKUTEN_CREDITCARD_PAYMENT))
				{
					continue;
				}
				// Boku payment
				if ((Constants.PAYMENT_BOKU_OPTION_ENABLED == false)
					&& (li.Value == Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_CARRIERBILLING_BOKU))
				{
					continue;
				}

				cassetteExternalPaymentAction.Add(li.Value, li.Text);
			}
		}
		rExternalPaymentActionSettingList.DataSource = cassetteExternalPaymentAction;

		// Get External Order Action Information Setting List
		var cassetteExternalOrderInfoAction = ValueText.GetValueItemList(
			Constants.TABLE_ORDERWORKFLOWSETTING,
			Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION)
				.Cast<ListItem>()
				.Where(item => (string.IsNullOrEmpty(item.Value) == false)
					&& (item.Value != Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION_CROSSMALL_UPDATE_STATUS))
				.ToDictionary(item => item.Value, item => item.Text);
		if (Constants.RECUSTOMER_API_OPTION_ENABLED == false) cassetteExternalOrderInfoAction.Remove(Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION_RECUSTOMER);

		rExternalOrderInfoActionSettingList.DataSource = cassetteExternalOrderInfoAction;

		// 督促ステータス（カセット表示用）
		if (Constants.DEMAND_OPTION_ENABLE)
		{
			Dictionary<string, string> cassetteDemandStatusChange = new Dictionary<string, string>();
			foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDERWORKFLOWSETTING_DEMAND_STATUS_CHANGE))
			{
				if (li.Value != "")	// 「指定しない」を対象外とする
				{
					cassetteDemandStatusChange.Add(li.Value, li.Text);
				}
			}

			rUpdateDemandStatusSettingList.Visible = true;
			rUpdateDemandStatusSettingList.DataSource = cassetteDemandStatusChange;
		}

		// 領収書出力フラグ（カセット表示用）
		var cassetteReceiptOutputFlgChange = new Dictionary<string, string>();
		ValueText.GetValueItemArray(
				Constants.TABLE_ORDERWORKFLOWSETTING,
				Constants.FIELD_ORDERWORKFLOWSETTING_RECEIPT_OUTPUT_FLG_CHANGE)
			.Where(li => (li.Value != ""))
			.ToList()
			.ForEach(li => cassetteReceiptOutputFlgChange.Add(li.Value, li.Text));
		rReceiptOutputFlgSettingList.DataSource = cassetteReceiptOutputFlgChange;

		// 拡張ステータス（カセット表示用）
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, "order_extend_status_change"))
		{
			if (li.Value != "")	// 「指定しない」を対象外とする
			{
				m_dicCassetteExtendStatus.Add(li.Value, li.Text);
			}
		}
		rOrderExtendStatusSettingList.DataSource
			= rFixedPurchaseExtendStatusSettingList.DataSource = orderExtendStatusSettingList;

		// 返品交換ステータス（カセット表示用）
		Dictionary<string, string> cassetteReturnExchangeStatusChange = new Dictionary<string, string>();
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_EXCHANGE_STATUS_CHANGE))
		{
			if (li.Value != "")	// 「指定しない」を対象外とする
			{
				cassetteReturnExchangeStatusChange.Add(li.Value, li.Text);
			}
		}
		rCassetteReturnExchangeStatusChangeList.DataSource = cassetteReturnExchangeStatusChange;

		// 返金ステータス（カセット表示用）
		Dictionary<string, string> cassetteRepaymentStatusChange = new Dictionary<string, string>();
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDERWORKFLOWSETTING_REPAYMENT_STATUS_CHANGE))
		{
			if (li.Value != "")	// 「指定しない」を対象外とする
			{
				cassetteRepaymentStatusChange.Add(li.Value, li.Text);
			}
		}
		rCassetteRepaymentStatusChangeList.DataSource = cassetteRepaymentStatusChange;

		// 定期購入状態変更（カセット表示用）
		var cassetteFixedPurchaseIsAliveChange = ValueText
			.GetValueItemList(
				Constants.TABLE_ORDERWORKFLOWSETTING,
				Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_IS_ALIVE_CHANGE)
			.Cast<ListItem>()
			.Where(item => item.Value != "")
			.ToDictionary(item => item.Value, item => item.Text);
		rUpdateFixedPurchaseIsAliveList.DataSource = cassetteFixedPurchaseIsAliveChange;

		// 定期購入状態変更（カセット表示用）
		var cassetteFixedPurchasePaymentStatusChange = ValueText
			.GetValueItemList(
				Constants.TABLE_ORDERWORKFLOWSETTING,
				Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_PAYMENT_STATUS_CHANGE)
			.Cast<ListItem>()
			.Where(item => item.Value != "")
			.ToDictionary(item => item.Value, item => item.Text);
		rUpdatePaymentStatusList.DataSource = cassetteFixedPurchasePaymentStatusChange;

		// 配送不可エリア停止変更（カセット表示用）
		var cassetteFixedPurchaseStopUnavailableShippingAreaChange = ValueText
			.GetValueItemList(
				Constants.TABLE_ORDERWORKFLOWSETTING,
				Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_STOP_UNAVAILABLE_SHIPPING_AREA_CHANGE)
			.Cast<ListItem>()
			.Where(item => item.Value != "")
			.ToDictionary(item => item.Value, item => item.Text);
		rUpdateStopUnavailableShippingAreaList.DataSource = cassetteFixedPurchaseStopUnavailableShippingAreaChange;

		// User Memo
		cblUserUserMemoFlg.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_USER, Constants.FIELD_USER_USER_MEMO));

		// AdvCode
		cblAdvCodeFlg.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ORDER, OrderSearchParam.KEY_ORDER_ADVCODE_WORKFLOW));

		// Display kbn Return (basic setting)
		rblDisplayKbnReturn.Items.AddRange(
			ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDERWORKFLOWSETTING_DISPLAY_KBN + "_setting")
				.Cast<ListItem>()
				.ToArray());

		// Return action reason kbn (action setting)
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_REASON_KBN))
		{
			rblReturnActionReasonKbn.Items.Add(li);
		}

		// Cassette return action (action setting)
		var cassetteReturnAction = new Dictionary<string, string>();
		cassetteReturnAction.Add(Constants.FLG_ORDERWORKFLOWSETTING_RETURN_ACTION_ACCEPT_RETURN,
			ValueText.GetValueText(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_ACTION, Constants.FLG_ORDERWORKFLOWSETTING_RETURN_ACTION_ACCEPT_RETURN));
		rReturnActionSettingList.DataSource = cassetteReturnAction;

		// Return Order Action
		rblReturnOrderAction.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_ACTION));

		// 発票ステータス
		cbInvoiceStatus.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_TWORDERINVOICE, Constants.FIELD_TWORDERINVOICE_TW_INVOICE_STATUS));

		// 配送状態
		cblShippingStatus.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS));

		// 配送先 都道府県
		foreach (var prefecture in Constants.STR_PREFECTURES_LIST)
		{
			cblShippingPrefectures.Items.Add(new ListItem(prefecture, prefecture));
		}

		// 完了状態コード
		cblShippingStatusCode.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS_CODE));

		// 現在の状態
		cblShippingCurrentStatus.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_CURRENT_STATUS));

		// 店舗受取ステータス
		if (Constants.STORE_PICKUP_OPTION_ENABLED)
		{
			trStorePickupStatusCondition.Visible = true;
			trStorePickupStatusAction.Visible = true;

			cblStorePickupStatus.Items.AddRange(ValueText.GetValueItemArray(
				Constants.TABLE_ORDER,
				Constants.FIELD_ORDER_STOREPICKUP_STATUS));

			rblStorePickupStatus.Items.AddRange(ValueText.GetValueItemArray(
				Constants.TABLE_ORDERWORKFLOWSETTING,
				Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_STOREPICKUP_STATUS_CHANGE));

			rblStorePickupStatus.SelectedValue = GetOrderWorkflowSettingValue(
				Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_STOREPICKUP_STATUS_CHANGE,
				WorkflowSetting.WorkflowTypes.Order,
				Constants.FLG_ORDERWORKFLOWSETTING_ORDER_STORE_PICKUP_STATUS_DEFAULT);
		}

		//------------------------------------------------------
		// データバインド
		//------------------------------------------------------
		DataBind();
	}

	/// <summary>
	/// 注文拡張項目 設定
	/// </summary>
	private void SetOrderExtend()
	{
		var values = new Dictionary<string, string[]>();
		if ((this.ActionStatus == Constants.ACTION_STATUS_UPDATE)
			|| (this.ActionStatus == Constants.ACTION_STATUS_COPY_INSERT)
			|| (this.ActionStatus == Constants.ACTION_STATUS_COMPLETE))
		{
			foreach (var strSettingUnit in this.OrderWorkflowSetting.GetValue(
				Constants.FIELD_ORDERWORKFLOWSETTING_SEARCH_SETTING,
				this.OrderWorkflowSetting.WorkflowType).Split('&'))
			{
				var settingUnitNaveValues = strSettingUnit.Split('=');
				var searchValues = settingUnitNaveValues[1].Split(',');
				if (Constants.ORDER_EXTEND_ATTRIBUTE_FIELD_LIST.Contains(settingUnitNaveValues[0]))
				{
					values.Add(settingUnitNaveValues[0], searchValues);
				}
			}
		}

		rOrderExtend.DataSource =
			DataCacheControllerFacade.GetOrderExtendSettingCacheController().CacheData.SettingModels;
		rOrderExtend.DataBind();
		foreach (var item in rOrderExtend.Items.Cast<RepeaterItem>())
		{
			var whfSettingId = GetWrappedControl<WrappedHiddenField>(item, "hfSettingId").Value;
			var wcbOrderExtend = GetWrappedControl<WrappedCheckBoxList>(item, "cblOrderExtendAttribute");
			wcbOrderExtend.Items.AddRange(SetOrderExtendItemList(whfSettingId).Cast<ListItem>().ToArray());

			if (values.ContainsKey(whfSettingId))
			{
				SetSearchCheckBoxValue((CheckBoxList)wcbOrderExtend.InnerControl, values[whfSettingId]);
			}
		}

		rFixedPurchaseOrderExtend.DataSource =
			DataCacheControllerFacade.GetOrderExtendSettingCacheController().CacheData.SettingModels;
		rFixedPurchaseOrderExtend.DataBind();
		foreach (var item in rFixedPurchaseOrderExtend.Items.Cast<RepeaterItem>())
		{
			var whfSettingId = GetWrappedControl<WrappedHiddenField>(item, "hfSettingId").Value;
			var wcbOrderExtend = GetWrappedControl<WrappedCheckBoxList>(item, "cblOrderExtendAttribute");
			wcbOrderExtend.Items.AddRange(SetOrderExtendItemList(whfSettingId).Cast<ListItem>().ToArray());

			if (values.ContainsKey(whfSettingId))
			{
				SetSearchCheckBoxValue((CheckBoxList)wcbOrderExtend.InnerControl, values[whfSettingId]);
			}
		}
	}

	/// <summary>
	/// Get Cancel Reason
	/// </summary>
	/// <returns>List Item</returns>
	public ListItemCollection GetCancelReason()
	{
		var result = new ListItemCollection();
		var fixedPurchasecancelReasonList = new FixedPurchaseService().GetCancelReasonForEC();
		if (fixedPurchasecancelReasonList.Length > 0)
		{
			result.Add(new ListItem(string.Empty, string.Empty));
			result.AddRange(fixedPurchasecancelReasonList.Select(item => new ListItem(item.CancelReasonName, item.CancelReasonId)).ToArray());

			return result;
		}

		return null;
	}

	/// <summary>
	/// 外部決済ステータス:クレジットカードリスト作成
	/// </summary>
	/// <param name="orderExternalPaymentStatusList">全外部決済ステータスリスト</param>
	/// <returns>外部決済ステータス:クレジットカードリスト</returns>
	private ListItem[] CreateExtendStatusListForCreditCard(ListItem[] orderExternalPaymentStatusList)
	{
		return orderExternalPaymentStatusList
			.Where(x =>
				(x.Value == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED)
				|| (x.Value == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP)
				|| (x.Value == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP)
				|| (x.Value == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL)
				|| (x.Value == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_REFUND)
				|| (x.Value == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_ERROR)
				|| (x.Value == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_ERROR)
				|| (x.Value == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL_ERROR)
				|| (x.Value == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_ERROR)
			).ToArray();
	}

	/// <summary>
	/// 外部決済ステータス：コンビニ(後払い)リスト作成
	/// </summary>
	/// <param name="orderExternalPaymentStatusList">全外部決済ステータスリスト</param>
	/// <returns>外部決済ステータス：コンビニ(後払い)リスト</returns>
	private ListItem[] CreateExtendStatusListForCVS(ListItem[] orderExternalPaymentStatusList)
	{
		return orderExternalPaymentStatusList
			.Where(x =>
				(x.Value == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED)
				|| (x.Value == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_WAIT)
				|| (x.Value == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_MIDST)
				|| (x.Value == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP)
				|| (x.Value == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_INV_MIDST)
				|| (x.Value == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_INV_COMP)
				|| (x.Value == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SHIP_WAIT)
				|| (x.Value == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SHIP_COMP)
				|| (x.Value == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_DELI_COMP)
				|| (x.Value == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP)
				|| (x.Value == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL)
				|| (x.Value == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_ERROR)
				|| (x.Value == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL_ERROR)
				|| (x.Value == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_INV_ERROR)
				|| (x.Value == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SHIP_ERROR)
				|| (x.Value == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_ERROR)
			).ToArray();
	}

	/// <summary>
	/// 外部決済ステータス：台湾後払いリスト作成
	/// </summary>
	/// <param name="orderExternalPaymentStatusList">全外部決済ステータスリスト</param>
	/// <returns>外部決済ステータス：コンビニ(後払い)リスト</returns>
	private ListItem[] CreateExtendStatusListForTryLinkAfterPay(ListItem[] orderExternalPaymentStatusList)
	{
		return orderExternalPaymentStatusList
			.Where(x =>
				(x.Value == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED)
				|| (x.Value == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_WAIT)
				|| (x.Value == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_MIDST)
				|| (x.Value == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP)
				|| (x.Value == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SHIP_WAIT)
				|| (x.Value == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SHIP_COMP)
				|| (x.Value == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_DELI_COMP)
				|| (x.Value == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL)
				|| (x.Value == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_ERROR)
				|| (x.Value == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL_ERROR)
				|| (x.Value == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SHIP_ERROR)
				|| (x.Value == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_ERROR)
			).ToArray();
	}

	/// <summary>
	/// 抽出検索条件情報を各コントロールに設定
	/// </summary>
	/// <param name="searchSettingFieldValue">抽出検索条件</param>
	/// <remarks>
	///	抽出検索条件の格納フォーマットは以下
	///	「項目名=値,値,値&項目名2=値,値&項目名3=値....」
	/// </remarks>
	private void SetSearchSetting(string searchSettingFieldValue)
	{
		foreach (string strSettingUnit in searchSettingFieldValue.Split('&'))
		{
			string[] settingUnitNaveValues = strSettingUnit.Split('=');
			string[] searchValues = settingUnitNaveValues[1].Split(',');	// 値(カンマ区切りの連続した値)

			switch (settingUnitNaveValues[0])
			{
				// 注文区分
				case Constants.FIELD_ORDER_ORDER_KBN:
					if (this.OrderWorkflowSetting.WorkflowType == WorkflowSetting.WorkflowTypes.Order) SetSearchCheckBoxValue(cblOrderKbn, searchValues);	// チェックボックスリスト設定
					else SetSearchCheckBoxValue(cblFixedPurchaseOrderKbn, searchValues);	// チェックボックスリスト設定
					break;

				// 注文者区分
				case Constants.FIELD_ORDEROWNER_OWNER_KBN:
					SetSearchCheckBoxValue(cblOwnerKbn, searchValues);
					break;

				// 並び順
				case Constants.REQUEST_KEY_SORT_KBN:
					var target = (this.OrderWorkflowSetting.WorkflowType == WorkflowSetting.WorkflowTypes.Order)
						? ddlSortKbn
						: ddlFixedPurchaseSortKbn;
					foreach (ListItem li in target.Items)
					{
						li.Selected = (li.Value == searchValues[0]);
					}
					break;

				// 注文ステータス
				case Constants.FIELD_ORDER_ORDER_STATUS:
					SetSearchCheckBoxValue(cblOrderStatus, searchValues);
					break;

				// ステータス
				case WorkflowSetting.m_FIELD_UPDATE_STATUS:
					foreach (ListItem li in ddlUpdateStatusDate.Items)
					{
						li.Selected = (li.Value == searchValues[0]);
					}
					break;

				// ステータス更新日
				case WorkflowSetting.m_FIELD_UPDATE_STATUS_DAY:
					foreach (ListItem li in rblUpdateStatusDate.Items)
					{
						li.Selected = (li.Value == searchValues[0]);
					}
					break;

				// ステータス更新日期間指定(From)
				case WorkflowSetting.m_FIELD_UPDATE_STATUS_FROM:
					foreach (ListItem li in ddlUpdateStatusDateFrom.Items)
					{
						li.Selected = (li.Value == searchValues[0]);
					}
					break;

				// ステータス更新日期間指定(To)
				case WorkflowSetting.m_FIELD_UPDATE_STATUS_TO:
					foreach (ListItem li in ddlUpdateStatusDateTo.Items)
					{
						li.Selected = (li.Value == searchValues[0]);
					}
					break;

				// ステータス更新日期間指定(時)
				case WorkflowSetting.m_FIELD_UPDATE_STATUS_HOUR:
					foreach (ListItem li in ddlUpdateStatusHourTo.Items)
					{
						li.Selected = (li.Value == searchValues[0]);
					}
					break;

				// ステータス更新日期間指定(分)
				case WorkflowSetting.m_FIELD_UPDATE_STATUS_MINUTE:
					foreach (ListItem li in ddlUpdateStatusMinuteTo.Items)
					{
						li.Selected = (li.Value == searchValues[0]);
					}
					break;

				// ステータス更新日期間指定(秒)
				case WorkflowSetting.m_FIELD_UPDATE_STATUS_SECOND:
					foreach (ListItem li in ddlUpdateStatusSecondTo.Items)
					{
						li.Selected = (li.Value == searchValues[0]);
					}
					break;

				// 入金ステータス
				case Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS:
					SetSearchCheckBoxValue(cblOrderPaymentStatus, searchValues);
					break;

				// 督促ステータス
				case Constants.FIELD_ORDER_DEMAND_STATUS:
					SetSearchCheckBoxValue(cblDemandStatus, searchValues);
					break;

				// 在庫引当ステータス
				case Constants.FIELD_ORDER_ORDER_STOCKRESERVED_STATUS:
					SetSearchCheckBoxValue(cblOrderStockRservedStatus, searchValues);
					break;

				// 出荷ステータス
				case Constants.FIELD_ORDER_ORDER_SHIPPED_STATUS:
					SetSearchCheckBoxValue(cblOrderShippedStatus, searchValues);
					break;

				// 決済種別
				case Constants.FIELD_ORDER_ORDER_PAYMENT_KBN:
					if (this.OrderWorkflowSetting.WorkflowType == WorkflowSetting.WorkflowTypes.Order) SetSearchCheckBoxValue(cblOrderPaymentKbn, searchValues);
					else SetSearchCheckBoxValue(cblFixedPurchasePaymentKbn, searchValues);
					break;

				// 返品交換：決済種別
				case ORDER_RETURN_PAYMENT_KBN:
					SetSearchCheckBoxValue(cblReturnOrderPaymentKbn, searchValues);
					break;

				// 配送先：国
				case Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE:
					SetSearchCheckBoxValue(cblShippingCountry, searchValues);
					break;

				// 配送種別
				case Constants.FIELD_ORDER_SHIPPING_ID:
					SetSearchCheckBoxValue(cblOrderShippingKbn, searchValues);
					break;

				// 配送希望日
				case Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE:
					foreach (string searchValue in searchValues)
					{
						switch (searchValue)
						{
							case WorkflowSetting.m_SHIPPINGDATE_SPECIFIED:
								cbShippingDateOn.Checked = true;
								break;

							case WorkflowSetting.m_SHIPPINGDATE_UNSPECIFIED:
								cbShippingDateOff.Checked = true;
								break;

							default:
								string[] values = searchValue.Split(WorkflowSetting.m_SHIPPINGDATE_SEPARATOR_CHARACTER);
								if (values.Length > 1)
								{
									tbShippingDateFrom.Text = values[0];
									tbShippingDateTo.Text = values[1];
								}
								break;
						}
					}
					break;

				// 配送料の別途見積もりフラグ
				case Constants.FIELD_ORDER_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG:
					SetSearchCheckBoxValue(cblSeparateEstimatesFlg, searchValues);
					break;

				// 配送伝票番号
				case Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO:
					SetSearchCheckBoxValue(cblShippingCheckNo, searchValues);
					break;

				// 出荷後変更区分
				case Constants.FIELD_ORDER_SHIPPED_CHANGED_KBN:
					SetSearchCheckBoxValue(cblShippedChangedKbn, searchValues);
					break;

				// サイト
				case Constants.FIELD_ORDER_MALL_ID:
					SetSearchCheckBoxValue(cblSiteName, searchValues);
					break;

				// 楽天ポイント利用方法
				case WorkflowSetting.m_FIELD_RAKUTEN_POINT_USE_TYPE:
					SetSearchCheckBoxValue(cblRakutenPointUseType, searchValues);
					break;

				// モール連携ステータス
				case WorkflowSetting.m_FIELD_MALL_LINK_STATUS:
					SetSearchCheckBoxValue(cblMallLinkStatus, searchValues);
					break;

				// 外部連携ステータス
				case WorkflowSetting.m_FIELD_EXTERNAL_IMPORT_STATUS:
					SetSearchCheckBoxValue(cblExternalImportStatus, searchValues);
					break;

				// 返品交換区分
				case Constants.FIELD_ORDER_RETURN_EXCHANGE_KBN:
					SetSearchCheckBoxValue(cblReturnExchangeKbn, searchValues);
					break;

				// 返品交換都合区分
				case Constants.FIELD_ORDER_RETURN_EXCHANGE_REASON_KBN:
					SetSearchCheckBoxValue(cblReturnExchangeReasonKbn, searchValues);
					break;

				// 並び順
				case Constants.REQUEST_KEY_RETURN_EXCHANGE_SORT_KBN:
					foreach (ListItem li in ddlReturnExchangeSortKbn.Items)
					{
						li.Selected = (li.Value == searchValues[0]);
					}
					break;

				// 返品交換ステータス
				case Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_STATUS:
					SetSearchCheckBoxValue(cblOrderReturnExchangeStatus, searchValues);
					break;

				// 返金ステータス
				case Constants.FIELD_ORDER_ORDER_REPAYMENT_STATUS:
					SetSearchCheckBoxValue(cblOrderRepaymentStatus, searchValues);
					break;

				// 返品交換返金更新日ステータス
				case WorkflowSetting.m_FIELD_RETURN_EXCHANGE_UPDATE_STATUS:
					foreach (ListItem li in ddlReturnExchangeUpdateStatusDate.Items)
					{
						li.Selected = (li.Value == searchValues[0]);
					}
					break;

				// 返品交換返金更新日ステータス更新日
				case WorkflowSetting.m_FIELD_RETURN_EXCHANGE_UPDATE_STATUS_DAY:
					foreach (ListItem li in rblReturnExchangeUpdateStatusDate.Items)
					{
						li.Selected = (li.Value == searchValues[0]);
					}
					break;

				// 返品交換返金更新日ステータス更新日期間指定(From)
				case WorkflowSetting.m_FIELD_RETURN_EXCHANGE_UPDATE_STATUS_FROM:
					foreach (ListItem li in ddlReturnExchangeUpdateStatusDateFrom.Items)
					{
						li.Selected = (li.Value == searchValues[0]);
					}
					break;

				// 返品交換返金更新日ステータス更新日期間指定(To)
				case WorkflowSetting.m_FIELD_RETURN_EXCHANGE_UPDATE_STATUS_TO:
					foreach (ListItem li in ddlReturnExchangeUpdateStatusDateTo.Items)
					{
						li.Selected = (li.Value == searchValues[0]);
					}
					break;

				// 返品交換返金更新日ステータス更新日期間指定(時)
				case WorkflowSetting.m_FIELD_RETURN_EXCHANGE_UPDATE_STATUS_HOUR:
					foreach (ListItem li in ddlReturnExchangeUpdateStatusHourTo.Items)
					{
						li.Selected = (li.Value == searchValues[0]);
					}
					break;

				// 返品交換返金更新日ステータス更新日期間指定(分)
				case WorkflowSetting.m_FIELD_RETURN_EXCHANGE_UPDATE_STATUS_MINUTE:
					foreach (ListItem li in ddlReturnExchangeUpdateStatusMinuteTo.Items)
					{
						li.Selected = (li.Value == searchValues[0]);
					}
					break;

				// 返品交換返金更新日ステータス更新日期間指定(秒)
				case WorkflowSetting.m_FIELD_RETURN_EXCHANGE_UPDATE_STATUS_SECOND:
					foreach (ListItem li in ddlReturnExchangeUpdateStatusSecondTo.Items)
					{
						li.Selected = (li.Value == searchValues[0]);
					}
					break;

				// 注文拡張ステータス1～50
				case Constants.FIELD_ORDER_EXTEND_STATUS1:
				case Constants.FIELD_ORDER_EXTEND_STATUS2:
				case Constants.FIELD_ORDER_EXTEND_STATUS3:
				case Constants.FIELD_ORDER_EXTEND_STATUS4:
				case Constants.FIELD_ORDER_EXTEND_STATUS5:
				case Constants.FIELD_ORDER_EXTEND_STATUS6:
				case Constants.FIELD_ORDER_EXTEND_STATUS7:
				case Constants.FIELD_ORDER_EXTEND_STATUS8:
				case Constants.FIELD_ORDER_EXTEND_STATUS9:
				case Constants.FIELD_ORDER_EXTEND_STATUS10:
				case Constants.FIELD_ORDER_EXTEND_STATUS11:
				case Constants.FIELD_ORDER_EXTEND_STATUS12:
				case Constants.FIELD_ORDER_EXTEND_STATUS13:
				case Constants.FIELD_ORDER_EXTEND_STATUS14:
				case Constants.FIELD_ORDER_EXTEND_STATUS15:
				case Constants.FIELD_ORDER_EXTEND_STATUS16:
				case Constants.FIELD_ORDER_EXTEND_STATUS17:
				case Constants.FIELD_ORDER_EXTEND_STATUS18:
				case Constants.FIELD_ORDER_EXTEND_STATUS19:
				case Constants.FIELD_ORDER_EXTEND_STATUS20:
				case Constants.FIELD_ORDER_EXTEND_STATUS21:
				case Constants.FIELD_ORDER_EXTEND_STATUS22:
				case Constants.FIELD_ORDER_EXTEND_STATUS23:
				case Constants.FIELD_ORDER_EXTEND_STATUS24:
				case Constants.FIELD_ORDER_EXTEND_STATUS25:
				case Constants.FIELD_ORDER_EXTEND_STATUS26:
				case Constants.FIELD_ORDER_EXTEND_STATUS27:
				case Constants.FIELD_ORDER_EXTEND_STATUS28:
				case Constants.FIELD_ORDER_EXTEND_STATUS29:
				case Constants.FIELD_ORDER_EXTEND_STATUS30:
				case Constants.FIELD_ORDER_EXTEND_STATUS31:
				case Constants.FIELD_ORDER_EXTEND_STATUS32:
				case Constants.FIELD_ORDER_EXTEND_STATUS33:
				case Constants.FIELD_ORDER_EXTEND_STATUS34:
				case Constants.FIELD_ORDER_EXTEND_STATUS35:
				case Constants.FIELD_ORDER_EXTEND_STATUS36:
				case Constants.FIELD_ORDER_EXTEND_STATUS37:
				case Constants.FIELD_ORDER_EXTEND_STATUS38:
				case Constants.FIELD_ORDER_EXTEND_STATUS39:
				case Constants.FIELD_ORDER_EXTEND_STATUS40:
				case Constants.FIELD_ORDER_EXTEND_STATUS41:
				case Constants.FIELD_ORDER_EXTEND_STATUS42:
				case Constants.FIELD_ORDER_EXTEND_STATUS43:
				case Constants.FIELD_ORDER_EXTEND_STATUS44:
				case Constants.FIELD_ORDER_EXTEND_STATUS45:
				case Constants.FIELD_ORDER_EXTEND_STATUS46:
				case Constants.FIELD_ORDER_EXTEND_STATUS47:
				case Constants.FIELD_ORDER_EXTEND_STATUS48:
				case Constants.FIELD_ORDER_EXTEND_STATUS49:
				case Constants.FIELD_ORDER_EXTEND_STATUS50:
					m_dicExtendStatus.Add(settingUnitNaveValues[0], settingUnitNaveValues[1]);
					break;

				// デジタルコンテンツ商品
				case Constants.FIELD_ORDER_DIGITAL_CONTENTS_FLG:
					SetSearchCheckBoxValue(cblDigitalContentsFlg, searchValues);
					break;

				// 定期購買注文
				case Constants.FIELD_ORDER_FIXED_PURCHASE_ID:
					SetSearchCheckBoxValue(cblFixedPurchase, searchValues);
					break;

				// 定期購入回数(注文時点)
				case Constants.FIELD_ORDER_FIXED_PURCHASE_ORDER_COUNT:
					tbOrderCountFrom.Text = searchValues[0];
					tbOrderCountTo.Text = searchValues[1];
					break;

				// 定期購入回数(出荷時点)
				case Constants.FIELD_ORDER_FIXED_PURCHASE_SHIPPED_COUNT:
					tbShippedCountFrom.Text = searchValues[0];
					tbShippedCountTo.Text = searchValues[1];
					break;

				// 注文メモ
				case Constants.FIELD_ORDER_MEMO:
					SetSearchCheckBoxValue(cblOrderMemoFlg, searchValues);
					break;

				// 管理メモ
				case Constants.FIELD_ORDER_MANAGEMENT_MEMO:
					SetSearchCheckBoxValue(cblOrderManagementMemoFlg, searchValues);
					break;

				// 配送メモ
				case Constants.FIELD_ORDER_SHIPPING_MEMO:
					SetSearchCheckBoxValue(
						(this.RequestWorkflowType == WorkflowSetting.m_KBN_WORKFLOW_TYPE_ORDER)
							? cblShippingMemoFlg
							: cblFpShippingMemoFlg,
						searchValues);
					break;

				// 決済連携メモ
				case Constants.FIELD_ORDER_PAYMENT_MEMO:
					SetSearchCheckBoxValue(cblOrderPaymentMemoFlg, searchValues);
					break;

				// 外部連携メモ
				case Constants.FIELD_ORDER_RELATION_MEMO:
					SetSearchCheckBoxValue(cblOrderRelationMemoFlg, searchValues);
					break;

				// 商品付帯情報
				case Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS:
					SetSearchCheckBoxValue(cblProductOptionFlg, searchValues);
					break;

				// ギフト購入フラグ
				case Constants.FIELD_ORDER_GIFT_FLG:
					SetSearchCheckBoxValue(cblGiftFlg, searchValues);
					break;

				// ユーザー管理レベル
				case Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID:
					SetSearchCheckBoxValue(cblUserManagementLevel, searchValues);
					break;

				// 別出荷フラグ
				case Constants.FIELD_ORDERSHIPPING_ANOTHER_SHIPPING_FLG:
					foreach (ListItem li in ddlAnotherShippingFlag.Items)
					{
						li.Selected = (li.Value == searchValues[0]);
					}
					break;

				// 請求書同梱フラグ
				case Constants.FIELD_ORDER_INVOICE_BUNDLE_FLG:
					SetSearchCheckBoxValue(cblInvoiceBundleFlg, searchValues);
					break;

				// 配送方法
				case Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD:
					SetSearchCheckBoxValue(cblShippingMethod, searchValues);
					break;

				// 配送会社
				case Constants.FIELD_ORDERSHIPPING_DELIVERY_COMPANY_ID:
					SetSearchCheckBoxValue(cblDeliveryCompany, searchValues);
					break;

				// 合計金額
				case Constants.FIELD_ORDER_ORDER_PRICE_TOTAL:
					tbOrderPriceTotal.Text = searchValues[0];
					break;

				// 商品ID
				case Constants.FIELD_ORDERITEM_PRODUCT_ID:
					if (this.OrderWorkflowSetting.WorkflowType == WorkflowSetting.WorkflowTypes.Order)
					{
						tbProductId.Text = string.Join(",", searchValues);
					}
					else
					{
						tbfixedPurchaseItemProductId.Text = string.Join(",", searchValues);
					}
					break;

				// 頒布会コースID
				case Constants.FIELD_ORDER_SUBSCRIPTION_BOX_COURSE_ID:
					if (this.WorkflowType == WorkflowSetting.m_KBN_WORKFLOW_TYPE_ORDER)
					{
						tbSubscriptionBoxCourseId.Text = string.Join(",", searchValues);
					}
					else
					{
						tbFixedPurchaseSubscriptionBoxCourseId.Text = string.Join(",", searchValues);
					}

					break;

				// セットプロモーションID
				case Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_ID:
					tbOrderSetPromotionId.Text = searchValues[0];
					break;

				// ノベルティID
				case Constants.FIELD_ORDERITEM_NOVELTY_ID:
					var noveltyIds = new List<string>();
					searchValues.ToList()
						.ForEach(value =>
						{
							switch (value)
							{
								case WorkflowSetting.m_NOVELTY_ID_SPECIFIED:
									cbNoveltyOn.Checked = true;
									break;
								case WorkflowSetting.m_NOVELTY_ID_UNSPECIFIED:
									cbNoveltyOff.Checked = true;
									break;
								default:
									if (string.IsNullOrEmpty(value) == false)
									{
										cbNoveltyOn.Checked = true;
										noveltyIds.Add(value);
									}
									break;
							}
						});
					tbNoveltyId.Enabled = cbNoveltyOn.Checked;
					tbNoveltyId.Text = string.Join(",", noveltyIds);
					break;

				// レコメンドID
				case Constants.FIELD_ORDERITEM_RECOMMEND_ID:
					var recommendIds = new List<string>();
					searchValues.ToList()
						.ForEach(value =>
						{
							switch (value)
							{
								case WorkflowSetting.m_NOVELTY_ID_SPECIFIED:
									cbRecommendOn.Checked = true;
									break;
								case WorkflowSetting.m_NOVELTY_ID_UNSPECIFIED:
									cbRecommendOff.Checked = true;
									break;
								default:
									if (string.IsNullOrEmpty(value) == false)
									{
										cbRecommendOn.Checked = true;
										recommendIds.Add(value);
									}
									break;
							}
						});
					tbRecommendId.Enabled = cbRecommendOn.Checked;
					tbRecommendId.Text = string.Join(",", recommendIds);
					break;

				// 外部決済ステータス：クレジットカード
				case WorkflowSetting.m_ORDER_EXTERNAL_PAYMENT_STATUS_CARD:
					SetSearchCheckBoxValue(cblOrderExternalPaymentStatusByCard, searchValues);
					break;

				// 返品交換：外部決済ステータス：クレジットカード
				case WorkflowSetting.m_ORDER_RETURN_EXTERNAL_PAYMENT_STATUS_CARD:
					SetSearchCheckBoxValue(cblReturnOrderExternalPaymentStatusByCard, searchValues);
					break;

				// 外部決済ステータス：コンビニ(後払い)
				case WorkflowSetting.m_ORDER_EXTERNAL_PAYMENT_STATUS_CVS:
					SetSearchCheckBoxValue(cblOrderExternalPaymentStatusByCVS, searchValues);
					break;

				// 返品交換：外部決済ステータス：コンビニ(後払い)
				case WorkflowSetting.m_ORDER_RETURN_EXTERNAL_PAYMENT_STATUS_CVS:
					SetSearchCheckBoxValue(cblReturnOrderExternalPaymentStatusByCVS, searchValues);
					break;

				// 外部決済ステータス：台湾後払い
				case WorkflowSetting.m_ORDER_EXTERNAL_PAYMENT_STATUS_TRYLINK_AFTERPAY:
					SetSearchCheckBoxValue(cblOrderExternalPaymentStatusByTryLinkAfterPay, searchValues);
					break;

				// 返品交換：外部決済ステータス：台湾後払い
				case WorkflowSetting.m_ORDER_RETURN_EXTERNAL_PAYMENT_STATUS_TRYLINK_AFTERPAY:
					SetSearchCheckBoxValue(cblReturnOrderExternalPaymentStatusByTryLinkAfterPay, searchValues);
					break;

				// Order external payment status for all payment
				case Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS:
					SetSearchCheckBoxValue(cblOrderExternalPaymentStatusForAllPayment, searchValues);
					SetSearchCheckBoxValue(cblReturnOrderExternalPaymentStatusForAllPayment, searchValues);
					break;

				// External Payment Status EcPay
				case WorkflowSetting.m_ORDER_EXTERNAL_PAYMENT_STATUS_ECPAY:
					SetSearchCheckBoxValue(cblOrderExternalPaymentStatusByEcPay, searchValues);
					break;

				// External Payment Status NewebPay
				case WorkflowSetting.m_ORDER_EXTERNAL_PAYMENT_STATUS_NEWEBPAY:
					SetSearchCheckBoxValue(cblOrderExternalPaymentStatusByNewebPay, searchValues);
					break;

				// 最終与信日指定
				case Constants.FIELD_ORDER_EXTERNAL_PAYMENT_AUTH_DATE:
					foreach (var searchValue in searchValues)
					{
						switch (searchValue)
						{
							case WorkflowSetting.m_LAST_AUTH_DATE_SPECIFIED:
								cbLastAuthDateOn.Checked = true;
								break;

							case WorkflowSetting.m_LAST_AUTH_DATE_UNSPECIFIED:
								cbLastAuthDateOff.Checked = true;
								break;

							default:
								var values = searchValue.Split(WorkflowSetting.m_LAST_AUTH_DATE_SEPARATOR_CHARACTER);
								if (values.Length > 1)
								{
									tbLastAuthDateFrom.Text = values[0];
									tbLastAuthDateTo.Text = values[1];
								}
								break;
						}
					}
					break;

				// user memo
				case Constants.FIELD_USER_USER_MEMO:
					SetSearchCheckBoxValue(cblUserUserMemoFlg, searchValues);
					break;

				// Advcode
				case OrderSearchParam.KEY_ORDER_ADVCODE_WORKFLOW:
					SetSearchCheckBoxValue(cblAdvCodeFlg, searchValues);
					break;

				// 出荷予定日
				case Constants.FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE:
					foreach (var searchValue in searchValues)
					{
						switch (searchValue)
						{
							case WorkflowSetting.m_SCHEDULED_SHIPPINGDATE_SPECIFIED:
								cbScheduledShippingDateOn.Checked = true;
								break;

							case WorkflowSetting.m_SCHEDULED_SHIPPINGDATE_UNSPECIFIED:
								cbScheduledShippingDateOff.Checked = true;
								break;

							default:
								var values = searchValue.Split(WorkflowSetting.m_SCHEDULED_SHIPPINGDATE_SEPARATOR_CHARACTER);
								if (values.Length > 1)
								{
									tbcbScheduledShippingDateFrom.Text = values[0];
									tbcbScheduledShippingDateTo.Text = values[1];
								}
								break;
						}
					}
					break;

				// 注文種別
				case WorkflowSetting.m_TARGET_ORDER_TYPE:
					SetSearchCheckBoxValue(cblOrder, searchValues);
					break;

				// 定期購入ステータス
				case Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_STATUS:
					SetSearchCheckBoxValue(cblFixedPurchaseStatus, searchValues);
					break;

				// 定期購入区分
				case Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_KBN:
					SetSearchCheckBoxValue(cblFixedPurchaseKbn, searchValues);
					break;

				// 配送種別(定期)
				case Constants.FIELD_PRODUCT_SHIPPING_TYPE:
					SetSearchCheckBoxValue(cblFixedPurchaseShippingKbn, searchValues);
					break;

				// 購入回数(注文基準[From])
				case WorkflowSetting.m_FIXEDPURCHASE_ORDER_COUNT_FROM:
					tbFixedPurchaseCountFrom.Text = searchValues[0];
					break;

				// 購入回数(注文基準[To])
				case WorkflowSetting.m_FIXEDPURCHASE_ORDER_COUNT_TO:
					tbFixedPurchaseCountTo.Text = searchValues[0];
					break;

				// 購入回数(出荷基準[From])
				case WorkflowSetting.m_FIXEDPURCHASE_SHIPPED_COUNT_FROM:
					tbFixedPurchaseShippedCountFrom.Text = searchValues[0];
					break;

				// 購入回数(出荷基準[To])
				case WorkflowSetting.m_FIXEDPURCHASE_SHIPPED_COUNT_TO:
					tbFixedPurchaseShippedCountTo.Text = searchValues[0];
					break;

				// 頒布会注文回数[FROM]
				case WorkflowSetting.SUBSCRIPTIONBOX_ORDER_COUNT_FROM:
					if (this.WorkflowType == WorkflowSetting.m_KBN_WORKFLOW_TYPE_ORDER)
					{
						tbSubscriptionBoxOrderCountFrom.Text = searchValues[0];
					}
					else
					{
						tbFixedPurchaseSubscriptionBoxOrderCountFrom.Text = searchValues[0];
					}

					break;

				// 頒布会注文回数[TO]
				case WorkflowSetting.SUBSCRIPTIONBOX_ORDER_COUNT_TO:
					if (this.WorkflowType == WorkflowSetting.m_KBN_WORKFLOW_TYPE_ORDER)
					{
						tbSubscriptionBoxOrderCountTo.Text = searchValues[0];
					}
					else
					{
						tbFixedPurchaseSubscriptionBoxOrderCountTo.Text = searchValues[0];
					}
					break;

				// 注文メモ
				case Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_MANAGEMENT_MEMO:
					SetSearchCheckBoxValue(cblFixedPurchaseManagementMemoFlg, searchValues);
					break;

				// 作成日
				case Constants.FIELD_FIXEDPURCHASE_DATE_CREATED:
					tbCreatedDateTo.Text = searchValues[0];
					break;

				// 更新日
				case Constants.FIELD_FIXEDPURCHASE_DATE_CHANGED:
					tbUpdatedDateTo.Text = searchValues[0];
					break;

				// 次回配送日
				case Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_DATE:
					var nextShippingValues = searchValues[0].Split(WorkflowSetting.m_LAST_AUTH_DATE_SEPARATOR_CHARACTER);
					if (nextShippingValues.Length > 1)
					{
						tbNextShippingDateFrom.Text = nextShippingValues[0];
						tbNextShippingDateTo.Text = nextShippingValues[1];
					}
					break;

				// 次々回配送日
				case Constants.FIELD_FIXEDPURCHASE_NEXT_NEXT_SHIPPING_DATE:
					var nextNextShippingValues = searchValues[0].Split(WorkflowSetting.m_LAST_AUTH_DATE_SEPARATOR_CHARACTER);
					if (nextNextShippingValues.Length > 0)
					{
						tbNextNextShippingDateFrom.Text = nextNextShippingValues[0];
						tbNextNextShippingDateTo.Text = (nextNextShippingValues.Length > 1) ? nextNextShippingValues[1] : string.Empty;
					}
					break;

				// 最終購入日
				case Constants.FIELD_FIXEDPURCHASE_LAST_ORDER_DATE:
					tbLastOrderDateTo.Text = searchValues[0];
					break;

				// 購入開始日
				case Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_DATE_BGN:
					tbFixedPurchaseDateTo.Text = searchValues[0];
					break;

				// 定期再開予定日
				case Constants.FIELD_FIXEDPURCHASE_RESUME_DATE:
					foreach (string searchValue in searchValues)
					{
						switch (searchValue)
						{
							case WorkflowSetting.m_FIXEDPURCHASE_RESUME_DATE_SPECIFIED:
								cbResumeDateOn.Checked = true;
								break;

							case WorkflowSetting.m_FIXEDPURCHASE_RESUME_DATE_UNSPECIFIED:
								cbResumeDateOff.Checked = true;
								break;

							default:
								var values = searchValue.Split(WorkflowSetting.m_FIXEDPURCHASE_RESUME_DATE_CHARACTER);
								if (values.Length > 1)
								{
									tbResumeDateFrom.Text = values[0];
									tbResumeDateTo.Text = values[1];
								}
								break;
						}
					}
					break;

				// 領収書希望フラグ
				case Constants.FIELD_ORDER_RECEIPT_FLG:
					SetSearchCheckBoxValue(
						(this.RequestWorkflowType == WorkflowSetting.m_KBN_WORKFLOW_TYPE_ORDER)
							? cblOrderReceiptFlg
							: cblFixedPurchaseReceiptFlg,
						searchValues);
					break;

				// 領収書出力フラグ
				case Constants.FIELD_ORDER_RECEIPT_OUTPUT_FLG:
					SetSearchCheckBoxValue(cblOrderReceiptOutputFlg, searchValues);
					break;

				// 発票ステータス
				case Constants.FIELD_TWORDERINVOICE_TW_INVOICE_STATUS:
					SetSearchCheckBoxValue(cbInvoiceStatus, searchValues);
					break;

				// 配送状態
				case Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS:
					SetSearchCheckBoxValue(cblShippingStatus, searchValues);
					break;

				// 配送先 都道府県
				case Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_SHIPPING_PREFECTURES:
					var prefectures = PrefectureUtility.GetPrefectures(searchValues[0]);
					SetSearchCheckBoxValue(cblShippingPrefectures, prefectures);
					break;

				// 市区町村
				case Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_SHIPPING_CITY:
					tbShippingCity.Text = searchValues[0];
					break;

				// 完了状態コード
				case Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS_CODE:
					SetSearchCheckBoxValue(cblShippingStatusCode, searchValues);
					break;

				// 配送状態
				case Constants.FIELD_ORDERSHIPPING_SHIPPING_CURRENT_STATUS:
					SetSearchCheckBoxValue(cblShippingCurrentStatus, searchValues);
					break;

				// キャンセル可能時間帯注文フラグ
				case Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_TARGET_CANCELABLE_TIME_ORDERS_FLG:
					cbExtractCancelable.Checked = searchValues[0] == Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_TARGET_CANCELABLE_TIME_ORDERS_FLG_ON;
					break;

				// 店舗受取ステータス
				case Constants.FIELD_ORDER_STOREPICKUP_STATUS:
					SetSearchCheckBoxValue(cblStorePickupStatus, searchValues);
					break;

				// 受取店舗
				case Constants.FIELD_REALSHOP_REAL_SHOP_ID:
					this.RealShopId = searchValues[0];
					break;
			}
		}
	}

	/// <summary>
	/// 注文ワークフロー設定入力データ取得
	/// </summary>
	/// <returns></returns>
	private Hashtable GetInputOrderWorkflowSetting()
	{
		//------------------------------------------------------
		// 基本設定
		//------------------------------------------------------
		Hashtable orderWorkflowSetting = new Hashtable();
		orderWorkflowSetting.Add(Constants.FIELD_ORDERWORKFLOWSETTING_SHOP_ID, this.LoginOperatorShopId);					// 店舗ID
		orderWorkflowSetting.Add(Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_KBN, ddlWorkflowKbn.SelectedValue);		// ワークフロー区分
		orderWorkflowSetting.Add(Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_NAME, tbWorkflowName.Text);				// ワークフロー名
		orderWorkflowSetting.Add(Constants.FIELD_ORDERWORKFLOWSETTING_DESC1, tbDesc1.Text);								// 説明1
		orderWorkflowSetting.Add(Constants.FIELD_ORDERWORKFLOWSETTING_DISPLAY_ORDER, tbDisplayOrder.Text);				// 表示順
		orderWorkflowSetting.Add(Constants.FIELD_ORDERWORKFLOWSETTING_DISPLAY_COUNT, ddlDisplayCount.SelectedValue);		// 表示件数
		orderWorkflowSetting.Add(Constants.FIELD_ORDERWORKFLOWSETTING_VALID_FLG,
			(cbValidFlg.Checked ? Constants.FLG_ORDERWORKFLOWSETTING_VALID_FLG_VALID : Constants.FLG_ORDERWORKFLOWSETTING_VALID_FLG_INVALID));	// 有効フラグ
		// ワークフロー詳細区分を設定
		if (rbWorkFlowDetailKbnNormal.Checked)
		{
			orderWorkflowSetting.Add(Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN, Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN_NORMAL);
			orderWorkflowSetting.Add(Constants.FIELD_ORDERWORKFLOWSETTING_DISPLAY_KBN, this.DisplayKbnValue); // 一覧表示区分
		}
		else if (rbWorkFlowDetailKbnOrderImportPopUp.Checked)
		{
			orderWorkflowSetting.Add(Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN, Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN_ODR_IMP);
			orderWorkflowSetting.Add(Constants.FIELD_ORDERWORKFLOWSETTING_DISPLAY_KBN, this.DisplayKbnValue); // 一覧表示区分
		}
		else if (rbWorkFlowDetailKbnReturn.Checked)
		{
			orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN] = Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN_RETURN;
			orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_DISPLAY_KBN] = this.DisplayKbnValue;
		}

		orderWorkflowSetting.Add(Constants.FIELD_ORDERWORKFLOWSETTING_ADDITIONAL_SEARCH_FLG, rblAdditionalSearchFlg.SelectedValue); // 追加検索可否フラグ
		orderWorkflowSetting.Add(Constants.FIELD_ORDERWORKFLOWSETTING_LAST_CHANGED, this.LoginOperatorName); // 最終更新者

		//------------------------------------------------------
		// 抽出検索条件設定
		//------------------------------------------------------
		// ワークフロー区分が通常注文？
		StringBuilder searchSql = new StringBuilder();
		if (ddlWorkflowKbn.SelectedValue != Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_KBN_RETURN_EXCHANGE)
		{
			var scheduledShippingDateFrom = tbcbScheduledShippingDateFrom.Text.Trim();
			var scheduledShippingDateTo = tbcbScheduledShippingDateTo.Text.Trim();
			var shippingDateFrom = tbShippingDateFrom.Text.Trim();
			var shippingDateTo = tbShippingDateTo.Text.Trim();
			var productId = tbProductId.Text.Trim();
			var orderSetPromotionId = tbOrderSetPromotionId.Text.Trim();
			var noveltyId = tbNoveltyId.Text.Trim();
			var recommendId = tbRecommendId.Text.Trim();
			var lastAuthDateFrom = tbLastAuthDateFrom.Text.Trim();
			var lastAuthDateTo = tbLastAuthDateTo.Text.Trim();
			var advCode = tbAdvCode.Text.Trim();
			var shippingCity = tbShippingCity.Text.Trim();
			var subscriptionBoxCourseId = tbSubscriptionBoxCourseId.Text.Trim();
			var subscriptionBoxOrderCountFrom = tbSubscriptionBoxOrderCountFrom.Text.Trim();
			var subscriptionBoxOrderCountTo = tbSubscriptionBoxOrderCountTo.Text.Trim();

			// 注文区分
			searchSql.Append(Constants.FIELD_ORDER_ORDER_KBN).Append("=").Append(CreateSearchStringParts(cblOrderKbn.Items));
			// 注文者区分
			searchSql.Append("&").Append(Constants.FIELD_ORDEROWNER_OWNER_KBN).Append("=").Append(CreateSearchStringParts(cblOwnerKbn.Items));
			// 並び順
			searchSql.Append("&").Append(Constants.REQUEST_KEY_SORT_KBN).Append("=").Append(ddlSortKbn.SelectedValue);
			// 注文ステータス
			searchSql.Append("&").Append(Constants.FIELD_ORDER_ORDER_STATUS).Append("=").Append(CreateSearchStringParts(cblOrderStatus.Items));
			// 店舗受取ステータス
			searchSql.Append("&").Append(Constants.FIELD_ORDER_STOREPICKUP_STATUS)
				.Append("=").Append(CreateSearchStringParts(cblStorePickupStatus.Items));
			// ステータス更新日
			searchSql.Append("&").Append(WorkflowSetting.m_FIELD_UPDATE_STATUS).Append("=").Append(ddlUpdateStatusDate.SelectedValue);
			searchSql.Append("&").Append(WorkflowSetting.m_FIELD_UPDATE_STATUS_DAY).Append("=").Append(rblUpdateStatusDate.SelectedValue);
			// 期間指定の場合
			if (rblUpdateStatusDate.SelectedValue == "0")
			{
				searchSql.Append("&").Append(WorkflowSetting.m_FIELD_UPDATE_STATUS_FROM).Append("=").Append(ddlUpdateStatusDateFrom.SelectedValue);
				searchSql.Append("&").Append(WorkflowSetting.m_FIELD_UPDATE_STATUS_TO).Append("=").Append(ddlUpdateStatusDateTo.SelectedValue);
				searchSql.Append("&").Append(WorkflowSetting.m_FIELD_UPDATE_STATUS_HOUR).Append("=").Append(ddlUpdateStatusHourTo.SelectedValue);
				searchSql.Append("&").Append(WorkflowSetting.m_FIELD_UPDATE_STATUS_MINUTE).Append("=").Append(ddlUpdateStatusMinuteTo.SelectedValue);
				searchSql.Append("&").Append(WorkflowSetting.m_FIELD_UPDATE_STATUS_SECOND).Append("=").Append(ddlUpdateStatusSecondTo.SelectedValue);
				searchSql.Append("&").Append(WorkflowSetting.m_FIELD_UPDATE_STATUS_TIME).Append("=").Append(ddlUpdateStatusHourTo.SelectedValue).Append(":").Append(ddlUpdateStatusMinuteTo.SelectedValue).Append(":").Append(ddlUpdateStatusSecondTo.SelectedValue);
			}
			// 入金ステータス
			searchSql.Append("&").Append(Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS).Append("=").Append(CreateSearchStringParts(cblOrderPaymentStatus.Items));
			// 督促ステータス
			searchSql.Append("&").Append(Constants.FIELD_ORDER_DEMAND_STATUS).Append("=").Append(CreateSearchStringParts(cblDemandStatus.Items));
			// 在庫引当ステータス
			searchSql.Append("&").Append(Constants.FIELD_ORDER_ORDER_STOCKRESERVED_STATUS).Append("=").Append(CreateSearchStringParts(cblOrderStockRservedStatus.Items));
			// 出荷ステータス
			searchSql.Append("&").Append(Constants.FIELD_ORDER_ORDER_SHIPPED_STATUS).Append("=").Append(CreateSearchStringParts(cblOrderShippedStatus.Items));
			// 注文拡張ステータス変更区分
			foreach (RepeaterItem ri in rOrderExtendStatusForSearch.Items)
			{
				bool blHasSelected = false;
				searchSql.Append("&").Append(FIELD_ORDER_EXTEND_STATUS + ((HiddenField)(ri.FindControl("hfExtendStatusChangeNo1"))).Value + "=");
				if (((CheckBox)(ri.FindControl("cbExtendStatusOn"))).Checked)
				{
					searchSql.Append(Constants.FLG_ORDER_ORDER_EXTEND_STATUS_ON);
					blHasSelected = true;
				}
				if (((CheckBox)(ri.FindControl("cbExtendStatusOff"))).Checked)
				{
					if (blHasSelected)
					{
						searchSql.Append(",");
					}
					searchSql.Append(Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF);
				}
			}
			// 決済種別
			searchSql.Append("&").Append(Constants.FIELD_ORDER_ORDER_PAYMENT_KBN).Append("=").Append(CreateSearchStringParts(cblOrderPaymentKbn.Items, true));

			// 出荷予定日（出荷予定日指定の有無も連結して設定）
			searchSql.Append("&").Append(Constants.FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE).Append("=");
			searchSql.Append(cbScheduledShippingDateOn.Checked
				&& ((string.IsNullOrEmpty(scheduledShippingDateFrom) == false) || (string.IsNullOrEmpty(scheduledShippingDateTo) == false))
					? string.Format("{0}~{1},", scheduledShippingDateFrom, scheduledShippingDateTo)
					: string.Empty);
			searchSql.Append(cbScheduledShippingDateOn.Checked ? WorkflowSetting.m_SCHEDULED_SHIPPINGDATE_SPECIFIED : string.Empty);
			searchSql.Append((cbScheduledShippingDateOn.Checked && cbScheduledShippingDateOff.Checked) ? "," : string.Empty)
				.Append(cbScheduledShippingDateOff.Checked ? WorkflowSetting.m_SCHEDULED_SHIPPINGDATE_UNSPECIFIED : string.Empty);
			// 出荷予定日指定ありのチェックボックスがONの場合のみ、日数の入力チェックを行う
			if (cbScheduledShippingDateOn.Checked)
			{
				orderWorkflowSetting.Add(WorkflowSetting.m_SCHEDULED_SHIPPINGDATE_FROM, scheduledShippingDateFrom);
				orderWorkflowSetting.Add(WorkflowSetting.m_SCHEDULED_SHIPPINGDATE_TO, scheduledShippingDateTo);
			}

			// 配送種別
			searchSql.Append("&").Append(Constants.FIELD_ORDER_SHIPPING_ID).Append("=").Append(CreateSearchStringParts(cblOrderShippingKbn.Items));
			// 配送希望日（配送希望日指定の有無も連結して設定）
			searchSql.Append("&").Append(Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE).Append("=");
			searchSql.Append(cbShippingDateOn.Checked && ((shippingDateFrom != "") || (shippingDateTo != ""))
				? string.Format("{0}~{1},", shippingDateFrom, shippingDateTo)
				: "");
			searchSql.Append(cbShippingDateOn.Checked ? WorkflowSetting.m_SHIPPINGDATE_SPECIFIED : "");
			searchSql.Append((cbShippingDateOn.Checked && cbShippingDateOff.Checked) ? "," : "").Append(cbShippingDateOff.Checked ? WorkflowSetting.m_SHIPPINGDATE_UNSPECIFIED : "");
			// 配送希望日指定ありのチェックボックスがONの場合のみ、日数の入力チェックを行う
			if (cbShippingDateOn.Checked)
			{
				orderWorkflowSetting.Add(WorkflowSetting.m_SHIPPINGDATE_DATEFROM, shippingDateFrom);
				orderWorkflowSetting.Add(WorkflowSetting.m_SHIPPINGDATE_DATETO, shippingDateTo);
			}
			// 配送料の別途見積もりフラグ
			searchSql.Append("&").Append(Constants.FIELD_ORDER_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG).Append("=").Append(CreateSearchStringParts(cblSeparateEstimatesFlg.Items));
			// 配送伝票番号
			searchSql.Append("&").Append(Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO).Append("=").Append(CreateSearchStringParts(cblShippingCheckNo.Items));
			// 出荷後変更区分
			searchSql.Append("&").Append(Constants.FIELD_ORDER_SHIPPED_CHANGED_KBN).Append("=").Append(CreateSearchStringParts(cblShippedChangedKbn.Items));
			// サイト
			searchSql.Append("&").Append(Constants.FIELD_ORDER_MALL_ID).Append("=").Append(CreateSearchStringParts(cblSiteName.Items));
			// モール連携ステータス
			searchSql.Append("&").Append(Constants.FIELD_ORDER_MALL_LINK_STATUS).Append("=").Append(CreateSearchStringParts(cblMallLinkStatus.Items));
			// 楽天ポイント利用方法(外部連携メモ)
			searchSql.Append("&").Append(WorkflowSetting.m_FIELD_RAKUTEN_POINT_USE_TYPE).Append("=").Append(CreateSearchStringParts(cblRakutenPointUseType.Items));
			// 外部連携ステータス
			searchSql.Append("&").Append(Constants.FIELD_ORDER_EXTERNAL_IMPORT_STATUS).Append("=").Append(CreateSearchStringParts(cblExternalImportStatus.Items));
			// デジタルコンテンツ商品
			searchSql.Append("&").Append(Constants.FIELD_ORDER_DIGITAL_CONTENTS_FLG).Append("=").Append(CreateSearchStringParts(cblDigitalContentsFlg.Items));
			// 定期購買注文
			searchSql.Append("&").Append(Constants.FIELD_ORDER_FIXED_PURCHASE_ID).Append("=").Append(CreateSearchStringParts(cblFixedPurchase.Items));
			// 定期購入回数(注文時点)
			searchSql.Append("&").Append(Constants.FIELD_ORDER_FIXED_PURCHASE_ORDER_COUNT).Append("=");
			int orderCountFrom;
			int orderCountTo;
			if (int.TryParse(tbOrderCountFrom.Text, out orderCountFrom)) searchSql.Append(orderCountFrom.ToString());
			searchSql.Append(",");
			if (int.TryParse(tbOrderCountTo.Text, out orderCountTo)) searchSql.Append(orderCountTo.ToString());
			// 定期購入回数(出荷時点)
			searchSql.Append("&").Append(Constants.FIELD_ORDER_FIXED_PURCHASE_SHIPPED_COUNT).Append("=");
			int shippedCountFrom;
			int shippedCountTo;
			if (int.TryParse(tbShippedCountFrom.Text, out shippedCountFrom)) searchSql.Append(shippedCountFrom.ToString());
			searchSql.Append(",");
			if (int.TryParse(tbShippedCountTo.Text, out shippedCountTo)) searchSql.Append(shippedCountTo.ToString());

			// 注文メモ
			searchSql.AppendFormat(
				"&{0}={1}",
				Constants.FIELD_ORDER_MEMO,
				CreateSearchStringParts(cblOrderMemoFlg.Items));
			searchSql.AppendFormat(
				"&{0}={1}",
				Constants.FIELD_ORDER_MEMO + Constants.CONST_FIELD_EXTEND_SEARCH_TEXT,
				HttpUtility.UrlEncode(tbMemo.Text));

			// 管理メモ
			searchSql.AppendFormat(
				"&{0}={1}",
				Constants.FIELD_ORDER_MANAGEMENT_MEMO,
				CreateSearchStringParts(cblOrderManagementMemoFlg.Items));
			searchSql.AppendFormat(
				"&{0}={1}",
				Constants.FIELD_ORDER_MANAGEMENT_MEMO + Constants.CONST_FIELD_EXTEND_SEARCH_TEXT,
				HttpUtility.UrlEncode(tbManagementMemo.Text));

			// 配送メモ
			searchSql.AppendFormat(
				"&{0}={1}",
				Constants.FIELD_ORDER_SHIPPING_MEMO,
				CreateSearchStringParts(cblShippingMemoFlg.Items));
			searchSql.AppendFormat(
				"&{0}={1}",
				Constants.FIELD_ORDER_SHIPPING_MEMO + Constants.CONST_FIELD_EXTEND_SEARCH_TEXT,
				HttpUtility.UrlEncode(tbShippingMemo.Text));

			// 決済連携メモ
			searchSql.AppendFormat(
				"&{0}={1}",
				Constants.FIELD_ORDER_PAYMENT_MEMO,
				CreateSearchStringParts(cblOrderPaymentMemoFlg.Items));
			searchSql.AppendFormat(
				"&{0}={1}",
				Constants.FIELD_ORDER_PAYMENT_MEMO + Constants.CONST_FIELD_EXTEND_SEARCH_TEXT,
				HttpUtility.UrlEncode(tbPaymentMemo.Text));

			// 外部連携メモ
			searchSql.AppendFormat(
				"&{0}={1}",
				Constants.FIELD_ORDER_RELATION_MEMO,
				CreateSearchStringParts(cblOrderRelationMemoFlg.Items));
			searchSql.AppendFormat(
				"&{0}={1}",
				Constants.FIELD_ORDER_RELATION_MEMO + Constants.CONST_FIELD_EXTEND_SEARCH_TEXT,
				HttpUtility.UrlEncode(tbRelationMemo.Text));

			// ギフト購入フラグ
			searchSql.Append("&").Append(Constants.FIELD_ORDER_GIFT_FLG).Append("=").Append(CreateSearchStringParts(cblGiftFlg.Items));
			// ユーザー管理レベル
			searchSql.Append("&").Append(Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID).Append("=").Append(CreateSearchStringParts(cblUserManagementLevel.Items));
			// 別出荷フラグ
			searchSql.Append("&").Append(Constants.FIELD_ORDERSHIPPING_ANOTHER_SHIPPING_FLG).Append("=").Append(ddlAnotherShippingFlag.SelectedValue);
			// 請求書同梱フラグ
			searchSql.Append("&").Append(Constants.FIELD_ORDER_INVOICE_BUNDLE_FLG).Append("=").Append(CreateSearchStringParts(cblInvoiceBundleFlg.Items));
			// 配送方法
			searchSql.Append("&").Append(Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD).Append("=").Append(CreateSearchStringParts(cblShippingMethod.Items));
			// 配送会社
			searchSql.Append("&").Append(Constants.FIELD_ORDERSHIPPING_DELIVERY_COMPANY_ID).Append("=").Append(CreateSearchStringParts(cblDeliveryCompany.Items));
			// 合計金額
			decimal orderPriceTotal;
			string orderPriceTotalString = "";

			if (decimal.TryParse(tbOrderPriceTotal.Text, out orderPriceTotal))
			{
				orderPriceTotalString = orderPriceTotal.ToString();
			}
			else
			{
				orderPriceTotalString = tbOrderPriceTotal.Text;
			}

			searchSql.Append("&").Append(Constants.FIELD_ORDER_ORDER_PRICE_TOTAL).Append("=").Append(orderPriceTotalString);
			orderWorkflowSetting.Add(Constants.FIELD_ORDER_ORDER_PRICE_TOTAL, orderPriceTotalString);
			// 商品ID
			searchSql.Append("&").Append(Constants.FIELD_ORDERITEM_PRODUCT_ID).Append("=").Append(productId);
			orderWorkflowSetting.Add(Constants.FIELD_ORDERITEM_PRODUCT_ID, productId);
			// セットプロモーションID
			searchSql.Append("&").Append(Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_ID).Append("=").Append(orderSetPromotionId);
			orderWorkflowSetting.Add(Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_ID, orderSetPromotionId);
			// ノベルティID
			searchSql.Append("&").Append(Constants.FIELD_ORDERITEM_NOVELTY_ID).Append("=");
			var noveltyValue = new StringBuilder();
			if (cbNoveltyOn.Checked)
			{
				// ノベルティID指定あり?
				if (string.IsNullOrEmpty(noveltyId) == false)
				{
					noveltyValue.Append(noveltyId);
				}
				else
				{
					noveltyValue.Append(WorkflowSetting.m_NOVELTY_ID_SPECIFIED);
				}
			}
			if (cbNoveltyOff.Checked)
			{
				noveltyValue.Append((noveltyValue.Length != 0) ? "," : "").Append(WorkflowSetting.m_NOVELTY_ID_UNSPECIFIED);
			}
			searchSql.Append(noveltyValue.ToString());
			orderWorkflowSetting.Add(Constants.FIELD_ORDERITEM_NOVELTY_ID, noveltyValue.ToString());
			// レコメンドID
			searchSql.Append("&").Append(Constants.FIELD_ORDERITEM_RECOMMEND_ID).Append("=");
			var recommendValue = new StringBuilder();
			if (cbRecommendOn.Checked)
			{
				// レコメンドID指定ありの場合は値セット
				recommendValue.Append(
					(string.IsNullOrEmpty(recommendId) == false)
					? recommendId
					: WorkflowSetting.m_RECOMMEND_ID_SPECIFIED);
			}
			if (cbRecommendOff.Checked)
			{
				recommendValue.Append((recommendValue.Length != 0) ? "," : "").Append(WorkflowSetting.m_RECOMMEND_ID_UNSPECIFIED);
			}
			searchSql.Append(recommendValue.ToString());
			orderWorkflowSetting.Add(Constants.FIELD_ORDERITEM_RECOMMEND_ID, recommendValue.ToString());
			// 外部決済ステータス：クレジットカード
			searchSql.Append("&").Append(WorkflowSetting.m_ORDER_EXTERNAL_PAYMENT_STATUS_CARD).Append("=").Append(CreateSearchStringParts(cblOrderExternalPaymentStatusByCard.Items));
			// 外部決済ステータス：コンビニ（後払い）
			searchSql.Append("&").Append(WorkflowSetting.m_ORDER_EXTERNAL_PAYMENT_STATUS_CVS).Append("=").Append(CreateSearchStringParts(cblOrderExternalPaymentStatusByCVS.Items));
			// 外部決済ステータス：台湾後払い
			searchSql.Append("&").Append(WorkflowSetting.m_ORDER_EXTERNAL_PAYMENT_STATUS_TRYLINK_AFTERPAY).Append("=").Append(CreateSearchStringParts(cblOrderExternalPaymentStatusByTryLinkAfterPay.Items));

			// External payment status for all payment
			searchSql.Append("&").Append(Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS).Append("=").Append(CreateSearchStringParts(cblOrderExternalPaymentStatusForAllPayment.Items));

			// EcPay
			searchSql.Append("&").Append(WorkflowSetting.m_ORDER_EXTERNAL_PAYMENT_STATUS_ECPAY).Append("=").Append(CreateSearchStringParts(cblOrderExternalPaymentStatusByEcPay.Items));
			// NewebPay
			searchSql.Append("&").Append(WorkflowSetting.m_ORDER_EXTERNAL_PAYMENT_STATUS_NEWEBPAY).Append("=").Append(CreateSearchStringParts(cblOrderExternalPaymentStatusByNewebPay.Items));
			// 最終与信日時（最終与信日指定の有無も連結して設定）
			searchSql.Append("&").Append(Constants.FIELD_ORDER_EXTERNAL_PAYMENT_AUTH_DATE).Append("=");
			searchSql.Append(cbLastAuthDateOn.Checked && ((lastAuthDateFrom != string.Empty) || (lastAuthDateTo != string.Empty))
				? string.Format("{0}~{1},", lastAuthDateFrom, lastAuthDateTo)
				: string.Empty);
			searchSql.Append(cbLastAuthDateOn.Checked ? WorkflowSetting.m_LAST_AUTH_DATE_SPECIFIED : string.Empty);
			searchSql.Append((cbLastAuthDateOn.Checked && cbLastAuthDateOff.Checked) ? "," : string.Empty);
			searchSql.Append(cbLastAuthDateOff.Checked ? WorkflowSetting.m_LAST_AUTH_DATE_UNSPECIFIED : string.Empty);
			if (cbLastAuthDateOn.Checked)
			{
				// 最終与信日時指定ありのチェックボックスがONの場合のみ、日数の入力チェックを行う
				orderWorkflowSetting.Add(WorkflowSetting.m_LAST_AUTH_DATE_DATEFROM, lastAuthDateFrom);
				orderWorkflowSetting.Add(WorkflowSetting.m_LAST_AUTH_DATE_DATETO, lastAuthDateTo);
			}
			// Order User Memo
			searchSql.Append("&").Append(Constants.FIELD_USER_USER_MEMO).Append("=").Append(CreateSearchStringParts(cblUserUserMemoFlg.Items));
			// 商品付帯情報
			searchSql.Append("&").Append(Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS).Append("=").Append(CreateSearchStringParts(cblProductOptionFlg.Items));

			// Order AdvCode
			searchSql.AppendFormat(
				"&{0}={1}",
				OrderSearchParam.KEY_ORDER_ADVCODE_WORKFLOW,
				CreateSearchStringParts(cblAdvCodeFlg.Items));
			searchSql.AppendFormat(
				"&{0}={1}",
				OrderSearchParam.KEY_ORDER_ADVCODE + Constants.CONST_FIELD_EXTEND_SEARCH_TEXT,
				tbAdvCode.Enabled ? HttpUtility.UrlEncode(advCode) : String.Empty);

			// 配送先：国
			searchSql.Append("&").Append(Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE).Append("=").Append(CreateSearchStringParts(cblShippingCountry.Items));
			// 注文種別
			searchSql.Append("&").Append(WorkflowSetting.m_TARGET_ORDER_TYPE).Append("=").Append(CreateSearchStringParts(cblOrder.Items));
			// 領収書希望フラグ
			searchSql.AppendFormat(
				"&{0}={1}",
				Constants.FIELD_ORDER_RECEIPT_FLG,
				CreateSearchStringParts(cblOrderReceiptFlg.Items));
			// 領収書出力フラグ
			searchSql.AppendFormat(
				"&{0}={1}",
				Constants.FIELD_ORDER_RECEIPT_OUTPUT_FLG,
				CreateSearchStringParts(cblOrderReceiptOutputFlg.Items));
			// 配送状態
			searchSql.Append("&").Append(Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS).Append("=").Append(CreateSearchStringParts(cblShippingStatus.Items));
			// 配送先 都道府県
			var prefectures = cblShippingPrefectures.Items.Cast<ListItem>().Where(li => li.Selected)
				.Select(li => li.Value).ToArray();
			searchSql.AppendFormat(
				"&{0}={1}",
				Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_SHIPPING_PREFECTURES,
				PrefectureUtility.GetHashString(prefectures));
			// 市区町村
			searchSql.Append("&").Append(Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_SHIPPING_CITY).Append("=").Append(shippingCity);

			foreach (var item in rOrderExtend.Items.Cast<RepeaterItem>())
			{
				var whfSettingId = GetWrappedControl<WrappedHiddenField>(item, "hfSettingId").Value;
				var wcbOrderExtend = GetWrappedControl<WrappedCheckBoxList>(item, "cblOrderExtendAttribute");
				searchSql.AppendFormat(
					"&{0}={1}",
					whfSettingId,
					CreateSearchStringParts(wcbOrderExtend.Items));
			}

			// 完了状態コード
			searchSql.Append("&").Append(Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS_CODE).Append("=").Append(CreateSearchStringParts(cblShippingStatusCode.Items));
			// 現在の状態
			searchSql.Append("&").Append(Constants.FIELD_ORDERSHIPPING_SHIPPING_CURRENT_STATUS).Append("=").Append(CreateSearchStringParts(cblShippingCurrentStatus.Items));

			// キャンセル可能時間帯注文フラグ
			searchSql.Append("&").Append(Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_TARGET_CANCELABLE_TIME_ORDERS_FLG)
				.Append("=").Append(
					cbExtractCancelable.Checked
						? Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_TARGET_CANCELABLE_TIME_ORDERS_FLG_ON
						: Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_TARGET_CANCELABLE_TIME_ORDERS_FLG_OFF);

			// 頒布会ID
			searchSql.Append("&").Append(Constants.FIELD_ORDER_SUBSCRIPTION_BOX_COURSE_ID).Append("=").Append(subscriptionBoxCourseId);
			// 頒布会注文回数(from)
			searchSql.Append("&").Append(WorkflowSetting.SUBSCRIPTIONBOX_ORDER_COUNT_FROM).Append("=").Append(subscriptionBoxOrderCountFrom);
			orderWorkflowSetting.Add(WorkflowSetting.SUBSCRIPTIONBOX_ORDER_COUNT_FROM, subscriptionBoxOrderCountFrom);
			// 頒布会注文回数(to)
			searchSql.Append("&").Append(WorkflowSetting.SUBSCRIPTIONBOX_ORDER_COUNT_TO).Append("=").Append(subscriptionBoxOrderCountTo);
			orderWorkflowSetting.Add(WorkflowSetting.SUBSCRIPTIONBOX_ORDER_COUNT_TO, subscriptionBoxOrderCountTo);
			// 受取店舗
			if (trPickupStore.Visible)
			{
				searchSql.Append("&").Append(Constants.FIELD_REALSHOP_REAL_SHOP_ID)
					.Append("=").Append(ddlStorePickup.SelectedValue);
			}
		}
		// ワークフロー区分が返品交換注文？
		else if (ddlWorkflowKbn.SelectedValue == Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_KBN_RETURN_EXCHANGE)
		{
			// 返品交換区分
			searchSql.Append(Constants.FIELD_ORDER_RETURN_EXCHANGE_KBN).Append("=").Append(CreateSearchStringParts(cblReturnExchangeKbn.Items));
			// 返品交換都合区分
			searchSql.Append("&").Append(Constants.FIELD_ORDER_RETURN_EXCHANGE_REASON_KBN).Append("=").Append(CreateSearchStringParts(cblReturnExchangeReasonKbn.Items));
			// 並び順
			searchSql.Append("&").Append(Constants.REQUEST_KEY_RETURN_EXCHANGE_SORT_KBN).Append("=").Append(ddlReturnExchangeSortKbn.SelectedValue);
			// 返品交換ステータス
			searchSql.Append("&").Append(Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_STATUS).Append("=").Append(CreateSearchStringParts(cblOrderReturnExchangeStatus.Items));
			// 返金ステータス
			searchSql.Append("&").Append(Constants.FIELD_ORDER_ORDER_REPAYMENT_STATUS).Append("=").Append(CreateSearchStringParts(cblOrderRepaymentStatus.Items));
			// 返品交換返金更新日
			searchSql.Append("&").Append(WorkflowSetting.m_FIELD_RETURN_EXCHANGE_UPDATE_STATUS).Append("=").Append(ddlReturnExchangeUpdateStatusDate.SelectedValue);
			searchSql.Append("&").Append(WorkflowSetting.m_FIELD_RETURN_EXCHANGE_UPDATE_STATUS_DAY).Append("=").Append(rblReturnExchangeUpdateStatusDate.SelectedValue);
			// 期間指定の場合
			if (rblReturnExchangeUpdateStatusDate.SelectedValue == "0")
			{
				searchSql.Append("&").Append(WorkflowSetting.m_FIELD_RETURN_EXCHANGE_UPDATE_STATUS_FROM).Append("=").Append(ddlReturnExchangeUpdateStatusDateFrom.SelectedValue);
				searchSql.Append("&").Append(WorkflowSetting.m_FIELD_RETURN_EXCHANGE_UPDATE_STATUS_TO).Append("=").Append(ddlReturnExchangeUpdateStatusDateTo.SelectedValue);
				searchSql.Append("&").Append(WorkflowSetting.m_FIELD_RETURN_EXCHANGE_UPDATE_STATUS_HOUR).Append("=").Append(ddlReturnExchangeUpdateStatusHourTo.SelectedValue);
				searchSql.Append("&").Append(WorkflowSetting.m_FIELD_RETURN_EXCHANGE_UPDATE_STATUS_MINUTE).Append("=").Append(ddlReturnExchangeUpdateStatusMinuteTo.SelectedValue);
				searchSql.Append("&").Append(WorkflowSetting.m_FIELD_RETURN_EXCHANGE_UPDATE_STATUS_SECOND).Append("=").Append(ddlReturnExchangeUpdateStatusSecondTo.SelectedValue);
				searchSql.Append("&").Append(WorkflowSetting.m_FIELD_RETURN_EXCHANGE_UPDATE_STATUS_TIME).Append("=").Append(ddlReturnExchangeUpdateStatusHourTo.SelectedValue).Append(":").Append(ddlReturnExchangeUpdateStatusMinuteTo.SelectedValue).Append(":").Append(ddlReturnExchangeUpdateStatusSecondTo.SelectedValue);
			}
			// 決済種別
			searchSql.Append("&").Append(ORDER_RETURN_PAYMENT_KBN).Append("=").Append(CreateSearchStringParts(cblReturnOrderPaymentKbn.Items));
			// 外部決済ステータス：クレジットカード
			searchSql.Append("&").Append(WorkflowSetting.m_ORDER_RETURN_EXTERNAL_PAYMENT_STATUS_CARD).Append("=").Append(CreateSearchStringParts(cblReturnOrderExternalPaymentStatusByCard.Items));
			// 外部決済ステータス：コンビニ（後払い）
			searchSql.Append("&").Append(WorkflowSetting.m_ORDER_RETURN_EXTERNAL_PAYMENT_STATUS_CVS).Append("=").Append(CreateSearchStringParts(cblReturnOrderExternalPaymentStatusByCVS.Items));
			// 外部決済ステータス：台湾後払い
			searchSql.Append("&").Append(WorkflowSetting.m_ORDER_RETURN_EXTERNAL_PAYMENT_STATUS_TRYLINK_AFTERPAY).Append("=").Append(CreateSearchStringParts(cblReturnOrderExternalPaymentStatusByTryLinkAfterPay.Items));

			// External payment status for all payment
			searchSql.Append("&").Append(Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS).Append("=").Append(CreateSearchStringParts(cblReturnOrderExternalPaymentStatusForAllPayment.Items));
		}

		if (OrderCommon.DisplayTwInvoiceInfo())
		{
			//発票ステータス
			searchSql.Append("&").Append(Constants.FIELD_TWORDERINVOICE_TW_INVOICE_STATUS).Append("=").Append(CreateSearchStringParts(cbInvoiceStatus.Items));
		}

		orderWorkflowSetting.Add(Constants.FIELD_ORDERWORKFLOWSETTING_SEARCH_SETTING, searchSql.ToString());

		//------------------------------------------------------
		// アクション設定
		//------------------------------------------------------
		string orderStatusChange = "";
		string productRealStockChange = "";
		string paymentStatusChange = "";
		string externalPaymentAction = "";
		string demandStatusChange = "";
		string returnExchangeStatusChange = "";
		string repaymentStatusChange = "";
		var returnAction = Constants.FLG_ORDERWORKFLOWSETTING_RETURN_ACTION_NOT_CHANGE;
		var returnReasonMemo = string.Empty;
		var returnReasonKbn = Constants.FLG_ORDERWORKFLOWSETTING_RETURN_REASON_KBN_CUSTOMER_CONVENIENCE;
		var scheduledShippingDateAction = Constants.FLG_ORDERWORKFLOWSETTING_SCHEDULED_SHIPPING_DATE_ACTION_OFF;
		var shippingDateAction = Constants.FLG_ORDERWORKFLOWSETTING_SHIPPING_DATE_ACTION_OFF;
		string[] orderExtendStatusChange = new string[Constants.CONST_ORDER_EXTEND_STATUS_DBFIELDS_MAX];
		var receiptOutputFlgChange = "";
		var invoiceStatus = string.Empty;
		var invoiceStatusApi = string.Empty;
		var externalOrderInfoAction = string.Empty;
		var storePickupStatus = string.Empty;

		if (OrderCommon.DisplayTwInvoiceInfo())
		{
			invoiceStatus = rblInvoiceStatusChange.SelectedValue;
		}

		if (Constants.TWINVOICE_ENABLED)
		{
			invoiceStatusApi = rblInvoiceStatusApi.SelectedValue;
		}

		// 通常注文の場合
		if (ddlWorkflowKbn.SelectedValue != Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_KBN_RETURN_EXCHANGE)
		{
			orderStatusChange = rblOrderStatusChange.SelectedValue;				// 注文ステータス変更区分
			productRealStockChange = Constants.REALSTOCK_OPTION_ENABLED ? rblProductRealStockChange.SelectedValue : "";// 商品在庫変更区分
			paymentStatusChange = rblPaymentStatusChange.SelectedValue;	// 入金ステータス変更区分

			// 出荷予定日
			scheduledShippingDateAction = (string.IsNullOrEmpty(rblScheduledShippingDateAction.SelectedValue) == false)
				? rblScheduledShippingDateAction.SelectedValue
				: Constants.FLG_ORDERWORKFLOWSETTING_SCHEDULED_SHIPPING_DATE_ACTION_OFF;

			shippingDateAction = rblShippingDateAction.SelectedValue;	// 配送希望日
			// 外部決済連携処理区分
			// 指定無し
			if (rbExternalPaymentActionNone.Checked)
			{
				externalPaymentAction = "";
			}
			// Zeusクレジット決済連携
			else if (rbExternalPaymentActionCardRealSales.Checked)
			{
				externalPaymentAction = Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_ZEUS_CREDITCARD_PAYMENT;
			}
			// SBPSクレジット決済連携
			else if (rbExternalPaymentActionSBPSCreditCardSales.Checked)
			{
				externalPaymentAction = Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_SBPS_CREDITCARD_SALES;
			}
			// GMOクレジット売上請求確定
			else if (rbExternalPaymentActionGMOCreditCardSales.Checked)
			{
				externalPaymentAction = Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_GMO_CREDITCARD_SALES;
			}
			// GMO Invoice
			else if (rbExternalPaymentActionGMOInvoice.Checked)
			{
				externalPaymentAction = Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_GMO_INVOICE;
			}
			// ZCOMクレジット決済連携
			else if (rbExternalPaymentActionZCOMCreditCardSales.Checked)
			{
				externalPaymentAction = Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_ZCOM_CREDITCARD_SALES;
			}
			// e-SCOTTクレジット決済連携
			else if (rbExternalPaymentActionEScottCreditCardSales.Checked)
			{
				externalPaymentAction = Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_ESCOTT_CREDITCARD_SALES;
			}
			// ベリトランスクレジット決済連携
			else if (rbExternalPaymentActionVeriTransCreditCardSales.Checked)
			{
				externalPaymentAction = Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_VERITRANS_CREDITCARD_SALES;
			}
			// ペイジェントクレジット決済連携
			else if (rbExternalPaymentActionPaygentCreditCardSales.Checked)
			{
				externalPaymentAction = Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_PAYGENT_CREDITCARD_SALES;
			}
			// ドコモケータイ払い
			else if (rbExternalPaymentActionDocomoPayment.Checked)
			{
				externalPaymentAction = Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_DOCOMO_PAYMENT;
			}
			// SBPSキャリア決済売上連動
			else if (rbExternalPaymentActionSBPSSoftbankKetaiPayment.Checked)
			{
				externalPaymentAction = Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_SBPS_SOFTBANKKETAI_PAYMENT;
			}
			else if (rbExternalPaymentActionSBPSDocomoKetaiPayment.Checked)
			{
				externalPaymentAction = Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_SBPS_DOCOMOKETAI_PAYMENT;
			}
			else if (rbExternalPaymentActionSBPSAuKantanPayment.Checked)
			{
				externalPaymentAction = Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_SBPS_AUKANTAN_PAYMENT;
			}
			else if (rbExternalPaymentActionSBPSRecruitPayment.Checked)
			{
				externalPaymentAction = Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_SBPS_RECRUIT_PAYMENT;
			}
			else if (rbExternalPaymentActionSBPSRakutenIdPayment.Checked)
			{
				externalPaymentAction = Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_SBPS_RAKUTEN_ID_PAYMENT;
			}
			else if (rbCvsDefGmo.Checked)
			{
				externalPaymentAction = Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_GMO_CVS_DEF_SHIP;
			}
			else if (rbCvsDefAtodene.Checked)
			{
				externalPaymentAction = Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_ATODENE_CVS_DEF_SHIP;
			}
			else if (rbCvsDefDsk.Checked)
			{
				externalPaymentAction = Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_DSK_CVS_DEF_SHIP;
			}
			else if (rbCvsDefScore.Checked)
			{
				externalPaymentAction = Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_SCORE_CVS_DEF_SHIP;
			}
			else if (rbCvsDefVeritrans.Checked)
			{
				externalPaymentAction = Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_VERITRANS_CVS_DEF_SHIP;
			}
			else if (rbExternalPaymentActionAmazonPayment.Checked)
			{
				externalPaymentAction = Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_AMAZON_PAYMENT;
			}
			else if (rbExternalPaymentActionPayPalPayment.Checked)
			{
				externalPaymentAction = Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_PAYPAL_PAYMENT;
			}
			else if (rbExternalPaymentActionPaidyPayment.Checked)
			{
				externalPaymentAction = Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_PAIDY_PAYMENT;
			}
			else if (rbExternalPaymentActionAtonePayment.Checked)
			{
				externalPaymentAction
					= Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_ATONE_PAYMENT;
			}
			else if (rbExternalPaymentActionAfteePayment.Checked)
			{
				externalPaymentAction
					= Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_AFTEE_PAYMENT;
			}
			else if (rbExternalPaymentActionLinePayment.Checked)
			{
				externalPaymentAction = Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_LINE_PAYMENT;
			}
			else if (rbExternalPaymentActionNPPayment.Checked)
			{
				externalPaymentAction = Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_NP_AFTERPAY_SHIP;
			}
			else if (rbExternalPaymentActionEcPayment.Checked)
			{
				externalPaymentAction = Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_EC_PAYMENT;
			}
			else if (rbCvsDefAtobaraicom.Checked)
			{
				externalPaymentAction = Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_ATOBARAICOM_CVS_DEF_SHIP;
			}
			else if (rbRequestCvsDefInvoiceReissueGmo.Checked)
			{
				externalPaymentAction = Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_GMO_CVS_DEF_INVOICE_REISSUE;
			}
			else if (rbExternalPaymentActionNewebPayment.Checked)
			{
				externalPaymentAction = Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_NEWEB_PAYMENT;
			}
			else if (rbExternalPaymentActionPayPayPayment.Checked)
			{
				externalPaymentAction = Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_PAYPAY_PAYMENT;
			}
			else if (rbExternalPaymentActionRakutenCreditCardSales.Checked)
			{
				externalPaymentAction = Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_RAKUTEN_CREDITCARD_PAYMENT;
			}
			else if (rbExternalPaymentActionBokuPayment.Checked)
			{
				externalPaymentAction = Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_CARRIERBILLING_BOKU;
			}
			demandStatusChange = rblDemandStatusChange.SelectedValue;		// 督促ステータス変更区分
			// 領収書出力フラグ
			if (Constants.RECEIPT_OPTION_ENABLED) receiptOutputFlgChange = rblOrderReceiptOutputFlgChange.SelectedValue;

			if ((rblReturnOrderAction.SelectedValue != Constants.FLG_ORDERWORKFLOWSETTING_RETURN_ACTION_NOT_CHANGE)
				&& rbWorkFlowDetailKbnReturn.Checked)
			{
				returnAction = rblReturnOrderAction.SelectedValue;
				returnReasonMemo = tbReturnActionReasonMemo.Text;
				returnReasonKbn = rblReturnActionReasonKbn.SelectedValue;
			}
			if (rbExternalOrderInfoActionNone.Checked)
			{
				externalOrderInfoAction = string.Empty;
			}
			else if (rbExternalOrderInfoActionECPay.Checked)
			{
				externalOrderInfoAction = Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION_ECPAY;
			}
			else if (rbExternalorderInfoNextEngine.Checked)
			{
				externalOrderInfoAction = Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION_NEXTENGINE;
			}
			else if (rbExternalOrderInfoActionNextEngineImport.Checked)
			{
				externalOrderInfoAction = Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION_NEXTENGINE_IMPORT;
			}
			else if (rbExternalOderInfoActionCrossMallUpdateStatus.Checked)
			{
				externalOrderInfoAction = Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION_CROSSMALL_UPDATE_STATUS;
			}
			else if (rbExternalOrderInfoActionRecustomer.Checked)
			{
				externalOrderInfoAction = Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION_RECUSTOMER;
			}
			if (Constants.STORE_PICKUP_OPTION_ENABLED)
			{
				storePickupStatus = rblStorePickupStatus.SelectedValue;
			}
		}
		// 返品交換注文の場合
		else if (ddlWorkflowKbn.SelectedValue == Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_KBN_RETURN_EXCHANGE)
		{
			returnExchangeStatusChange = rblReturnExchangeStatusChange.SelectedValue;	// 返品交換ステータス変更区分
			repaymentStatusChange = rblRepaymentStatusChange.SelectedValue;		// 返金ステータス変更区分
		}
		// 注文拡張ステータス更新区分１～４０
		foreach (RepeaterItem ri in rOrderExtendStatusForAction.Items)
		{
			orderExtendStatusChange[int.Parse(((HiddenField)(ri.FindControl("hfExtendStatusChangeNo"))).Value) - 1] = ((RadioButtonList)(ri.FindControl("rblExtendStatusChange"))).SelectedValue;
		}

		//------------------------------------------------------
		// 複数指定アクション設定
		//------------------------------------------------------
		StringBuilder cassetteOrderStatusChange = new StringBuilder();
		StringBuilder cassetteProductRealStockChange = new StringBuilder();
		StringBuilder cassettePaymentStatusChange = new StringBuilder();
		StringBuilder cassetteExternalPaymentAction = new StringBuilder();
		StringBuilder cassetteDemandStatusChange = new StringBuilder();
		StringBuilder cassetteReturnExchangeStatusChange = new StringBuilder();
		StringBuilder cassetteExchangeStatusChange = new StringBuilder();
		StringBuilder cassetteRepaymentStatusChange = new StringBuilder();
		var cassetteReturnAction = new StringBuilder();
		var cassetteReturnReasonMemo = string.Empty;
		var cassetteReturnReasonKbn = Constants.FLG_ORDERWORKFLOWSETTING_RETURN_REASON_KBN_CUSTOMER_CONVENIENCE;
		Dictionary<int, StringBuilder> cassetteOrderExtendStatusList = new Dictionary<int, StringBuilder>();
		var cassetteReceiptOutputFlgChange = new StringBuilder();
		var cassetteInvoiceStatusChange = new StringBuilder();
		var cassetteInvoiceStatusApi = new StringBuilder();
		var cassetteExternalOrderInfoAction = new StringBuilder();
		var cassetteStorePickupStatusChange = new StringBuilder();

		// 通常注文の場合
		StringBuilder cassetteDefaultSelect = new StringBuilder();
		if (ddlWorkflowKbn.SelectedValue != Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_KBN_RETURN_EXCHANGE)
		{
			// 「何もしない」チェック有無設定
			orderWorkflowSetting.Add(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_NO_UPDATE, ((cbCassetteOrderDoNothingFlg.Checked) ? Constants.FLG_ORDERWORKFLOWSETTING_CASSETTE_NO_UPDATE_ON : Constants.FLG_ORDERWORKFLOWSETTING_CASSETTE_NO_UPDATE_OFF));

			// 注文ステータス
			foreach (RepeaterItem riSetting in rUpdateOrderStatusSettingList.Items)
			{
				if (((CheckBox)riSetting.FindControl("cbCassetteOrderStatusChange")).Checked)
				{
					GetInputCassetteAction(
						riSetting,
						Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_STATUS_CHANGE,
						cassetteDefaultSelect,
						cassetteOrderStatusChange);
				}
			}

			if (Constants.STORE_PICKUP_OPTION_ENABLED)
			{
				foreach (RepeaterItem riSetting in rUpdateStorePickupStatusSettingList.Items)
				{
					if (((CheckBox)riSetting.FindControl("cbStorePickupStatusChange")).Checked)
					{
						GetInputCassetteAction(
							riSetting,
							Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_STOREPICKUP_STATUS_CHANGE,
							cassetteDefaultSelect,
							cassetteStorePickupStatusChange);
					}
				}
			}

			// Order Invoice Status
			foreach (RepeaterItem riSetting in rUpdateOrderInvoiceStatusSettingList.Items)
			{
				if (((CheckBox)riSetting.FindControl("cbCassetteInvoiceStatusChange")).Checked)
				{
					GetInputCassetteAction(
						riSetting,
						Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_INVOICE_STATUS_CHANGE,
						cassetteDefaultSelect,
						cassetteInvoiceStatusChange);
				}
			}

			// Order Invoice Status Call Api
			foreach (RepeaterItem riSetting in rUpdateOrderInvoiceStatusCallApiSettingList.Items)
			{
				if (((CheckBox)riSetting.FindControl("cbCassetteInvoiceStatusApi")).Checked)
				{
					GetInputCassetteAction(
						riSetting,
						Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_INVOICE_STATUS_API,
						cassetteDefaultSelect,
						cassetteInvoiceStatusApi);
				}
			}

			// 実在庫連動処理
			foreach (RepeaterItem riSetting in rUpdateProductRealStockSettingList.Items)
			{
				if (((CheckBox)riSetting.FindControl("cbCassetteProductRealStockChange")).Checked)
				{
					GetInputCassetteAction(
						riSetting,
						Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_PRODUCT_REALSTOCK_CHANGE,
						cassetteDefaultSelect,
						cassetteProductRealStockChange);
				}
			}

			// 入金ステータス
			foreach (RepeaterItem riSetting in rUpdatePaymentStatusSettingList.Items)
			{
				if (((CheckBox)riSetting.FindControl("cbCassettePaymentStatusChange")).Checked)
				{
					GetInputCassetteAction(
						riSetting,
						Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_PAYMENT_STATUS_CHANGE,
						cassetteDefaultSelect,
						cassettePaymentStatusChange);
				}
			}

			// 外部決済連携
			foreach (RepeaterItem riSetting in rExternalPaymentActionSettingList.Items)
			{
				if (((CheckBox)riSetting.FindControl("cbCassetteExternalPaymentAction")).Checked)
				{
					GetInputCassetteAction(
						riSetting,
						Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_EXTERNAL_PAYMENT_ACTION,
						cassetteDefaultSelect,
						cassetteExternalPaymentAction);
				}
			}

			// 外部決済連携
			foreach (RepeaterItem riSetting in rExternalOrderInfoActionSettingList.Items)
			{
				if (((CheckBox)riSetting.FindControl("cbCassetteExternalOrderInfoAction")).Checked)
				{
					GetInputCassetteAction(
						riSetting,
						Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_EXTERNAL_ORDER_INFO_ACTION,
						cassetteDefaultSelect,
						cassetteExternalOrderInfoAction);
				}
			}

			// 督促ステータス
			foreach (RepeaterItem riSetting in rUpdateDemandStatusSettingList.Items)
			{
				if (((CheckBox)riSetting.FindControl("cbCassetteDemandStatusChange")).Checked)
				{
					GetInputCassetteAction(
						riSetting,
						Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_DEMAND_STATUS_CHANGE,
						cassetteDefaultSelect,
						cassetteDemandStatusChange);
				}
			}

			// 領収書出力フラグ
			if (Constants.RECEIPT_OPTION_ENABLED)
			{
				foreach (RepeaterItem riSetting in rReceiptOutputFlgSettingList.Items)
				{
					if (((CheckBox)riSetting.FindControl("cbCassetteReceiptOutputFlgChange")).Checked == false) continue;
					GetInputCassetteAction(
						riSetting,
						Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_RECEIPT_OUTPUT_FLG_CHANGE,
						cassetteDefaultSelect,
						cassetteReceiptOutputFlgChange);
				}
			}

			foreach (RepeaterItem repeaterItem in rReturnActionSettingList.Items)
			{
				if (((CheckBox)repeaterItem.FindControl("cbCassetteReturnAction")).Checked
					&& rbWorkFlowDetailKbnReturn.Checked)
				{
					// Cassette Return Action
					GetInputCassetteAction(
						repeaterItem,
						Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_RETURN_ACTION,
						cassetteDefaultSelect,
						cassetteReturnAction);

					// Get cassette return memo
					cassetteReturnReasonMemo = ((TextBox)repeaterItem.FindControl("tbCassetteReturnMemo")).Text;

					// Get cassette return memo
					cassetteReturnReasonKbn = ((RadioButtonList)repeaterItem.FindControl("rblCassetteReturnKbn")).SelectedValue;
				}
			}

			// 拡張ステータス１～４０
			foreach (RepeaterItem riParent in rOrderExtendStatusSettingList.Items)
			{
				StringBuilder cassetteOrderExtendStatus = new StringBuilder();
				foreach (RepeaterItem riSetting in ((Repeater)riParent.FindControl("rCassetteOrderExtendStatusChildList")).Items)
				{
					if (((CheckBox)riSetting.FindControl("cbCassetteOrderExtendStatus")).Checked)
					{
						GetInputCassetteAction(
							riSetting,
							WorkflowSetting.m_FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE[int.Parse(((HiddenField)riParent.FindControl("htExtendStatusNo")).Value) - 1],
							WorkflowSetting.m_FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE[int.Parse(((HiddenField)riParent.FindControl("htExtendStatusNo")).Value) - 1] + "_",
							cassetteDefaultSelect,
							cassetteOrderExtendStatus);
					}
				}

				int iExtendStatusNo;
				if (int.TryParse(((HiddenField)riParent.FindControl("htExtendStatusNo")).Value, out iExtendStatusNo) == false)
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				}

				cassetteOrderExtendStatusList.Add(iExtendStatusNo - 1, cassetteOrderExtendStatus);
			}
		}
		// 返品交換注文の場合
		else if (ddlWorkflowKbn.SelectedValue == Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_KBN_RETURN_EXCHANGE)
		{
			// 「何もしない」チェック有無設定
			orderWorkflowSetting.Add(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_NO_UPDATE, ((cbCassetteReturnExchangeDoNothingFlg.Checked) ? Constants.FLG_ORDERWORKFLOWSETTING_CASSETTE_NO_UPDATE_ON : Constants.FLG_ORDERWORKFLOWSETTING_CASSETTE_NO_UPDATE_OFF));

			// 返品交換ステータス
			foreach (RepeaterItem riSetting in rCassetteReturnExchangeStatusChangeList.Items)
			{
				if (((CheckBox)riSetting.FindControl("cbCassetteReturnExchangeStatusChange")).Checked)
				{
					GetInputCassetteAction(
						riSetting,
						Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_RETURN_EXCHANGE_STATUS_CHANGE,
						cassetteDefaultSelect,
						cassetteReturnExchangeStatusChange);
				}
			}

			// 返金ステータス
			foreach (RepeaterItem riSetting in rCassetteRepaymentStatusChangeList.Items)
			{
				if (((CheckBox)riSetting.FindControl("cbCassetteRepaymentStatusChange")).Checked)
				{
					GetInputCassetteAction(
						riSetting,
						Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_REPAYMENT_STATUS_CHANGE,
						cassetteDefaultSelect,
						cassetteRepaymentStatusChange);
				}
			}
		}

		//------------------------------------------------------
		// アクション設定格納（ライン表示・カセット表示）
		//------------------------------------------------------
		// ライン表示（初期化）
		orderWorkflowSetting.Add(Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_STATUS_CHANGE, "");
		orderWorkflowSetting.Add(Constants.FIELD_ORDERWORKFLOWSETTING_PRODUCT_REALSTOCK_CHANGE, "");
		orderWorkflowSetting.Add(Constants.FIELD_ORDERWORKFLOWSETTING_PAYMENT_STATUS_CHANGE, "");
		orderWorkflowSetting.Add(Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION, "");
		orderWorkflowSetting.Add(Constants.FIELD_ORDERWORKFLOWSETTING_DEMAND_STATUS_CHANGE, "");
		orderWorkflowSetting.Add(Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_EXCHANGE_STATUS_CHANGE, "");
		orderWorkflowSetting.Add(Constants.FIELD_ORDERWORKFLOWSETTING_REPAYMENT_STATUS_CHANGE, "");
		orderWorkflowSetting.Add(Constants.FIELD_ORDERWORKFLOWSETTING_SCHEDULED_SHIPPING_DATE_ACTION, scheduledShippingDateAction);
		orderWorkflowSetting.Add(Constants.FIELD_ORDERWORKFLOWSETTING_SHIPPING_DATE_ACTION, shippingDateAction);
		orderWorkflowSetting.Add(Constants.FIELD_ORDERWORKFLOWSETTING_RECEIPT_OUTPUT_FLG_CHANGE, "");
		for (int i = 0; i < Constants.CONST_ORDER_EXTEND_STATUS_DBFIELDS_MAX; i++)
		{
			orderWorkflowSetting.Add(WorkflowSetting.m_FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE[i], "");
		}
		orderWorkflowSetting.Add(Constants.FIELD_ORDERWORKFLOWSETTING_MAIL_ID, "");
		// カセット表示（初期化）
		orderWorkflowSetting.Add(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_DEFAULT_SELECT, "");
		orderWorkflowSetting.Add(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_STATUS_CHANGE, "");
		orderWorkflowSetting.Add(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_PRODUCT_REALSTOCK_CHANGE, "");
		orderWorkflowSetting.Add(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_PAYMENT_STATUS_CHANGE, "");
		orderWorkflowSetting.Add(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_EXTERNAL_PAYMENT_ACTION, "");
		orderWorkflowSetting.Add(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_DEMAND_STATUS_CHANGE, "");
		orderWorkflowSetting.Add(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_RETURN_EXCHANGE_STATUS_CHANGE, "");
		orderWorkflowSetting.Add(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_REPAYMENT_STATUS_CHANGE, "");
		orderWorkflowSetting.Add(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_RECEIPT_OUTPUT_FLG_CHANGE, "");
		for (int i = 0; i < Constants.CONST_ORDER_EXTEND_STATUS_DBFIELDS_MAX; i++)
		{
			orderWorkflowSetting.Add(WorkflowSetting.m_FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE[i], "");
		}
		orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_INVOICE_STATUS_CHANGE] = string.Empty;
		orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_INVOICE_STATUS_API] = string.Empty;
		orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_INVOICE_STATUS_CHANGE] = string.Empty;
		orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_INVOICE_STATUS_API] = string.Empty;
		orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_EXTERNAL_ORDER_INFO_ACTION] = string.Empty;
		orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION] = string.Empty;
		orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_STOREPICKUP_STATUS_CHANGE] = string.Empty;
		orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_STOREPICKUP_STATUS_CHANGE] = string.Empty;

		// セット
		switch ((string)orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_DISPLAY_KBN])
		{
			case Constants.FLG_ORDERWORKFLOWSETTING_DISPLAY_KBN_LINE:
				orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_STATUS_CHANGE] = orderStatusChange;
				orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_PRODUCT_REALSTOCK_CHANGE] = productRealStockChange;
				orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_PAYMENT_STATUS_CHANGE] = paymentStatusChange;
				orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION] = externalPaymentAction;
				orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_EXTERNAL_ORDER_INFO_ACTION] = externalOrderInfoAction;
				orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_DEMAND_STATUS_CHANGE] = demandStatusChange;
				orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_EXCHANGE_STATUS_CHANGE] = returnExchangeStatusChange;
				orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_REPAYMENT_STATUS_CHANGE] = repaymentStatusChange;
				for (int i = 0; i < Constants.CONST_ORDER_EXTEND_STATUS_DBFIELDS_MAX; i++)
				{
					orderWorkflowSetting[WorkflowSetting.m_FIELD_ORDERWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE[i]] = ((orderExtendStatusChange[i] != null) ? orderExtendStatusChange[i] : "");
				}
				orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_MAIL_ID] = ddlMailId.SelectedValue;

				// Set return action (display line)
				orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_ACTION] = returnAction;
				// Set return reason kbn (display line)
				orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_REASON_KBN] = returnReasonKbn;
				// Set return reason memo (display line)
				orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_REASON_MEMO] = returnReasonMemo;
				orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_RETURN_ACTION] = string.Empty;
				// 領収書出力フラグ
				orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_RECEIPT_OUTPUT_FLG_CHANGE] = receiptOutputFlgChange;
				// Taiwan Invoice Status
				orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_INVOICE_STATUS_CHANGE] = invoiceStatus;
				orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_INVOICE_STATUS_API] = invoiceStatusApi;
				// Set store pickup status
				orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_ORDER_STOREPICKUP_STATUS_CHANGE] = storePickupStatus;
				break;

			case Constants.FLG_ORDERWORKFLOWSETTING_DISPLAY_KBN_CASSETTE:
				orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_DEFAULT_SELECT] = cassetteDefaultSelect.ToString();
				orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_STATUS_CHANGE] = cassetteOrderStatusChange.ToString();
				orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_PRODUCT_REALSTOCK_CHANGE] = cassetteProductRealStockChange.ToString();
				orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_PAYMENT_STATUS_CHANGE] = cassettePaymentStatusChange.ToString();
				orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_EXTERNAL_PAYMENT_ACTION] = cassetteExternalPaymentAction.ToString();
				orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_EXTERNAL_ORDER_INFO_ACTION] = cassetteExternalOrderInfoAction.ToString();
				orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_DEMAND_STATUS_CHANGE] = cassetteDemandStatusChange.ToString();
				orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_RETURN_EXCHANGE_STATUS_CHANGE] = cassetteReturnExchangeStatusChange.ToString();
				orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_REPAYMENT_STATUS_CHANGE] = cassetteRepaymentStatusChange.ToString();
				for (int i = 0; i < Constants.CONST_ORDER_EXTEND_STATUS_DBFIELDS_MAX; i++)
				{
					orderWorkflowSetting[WorkflowSetting.m_FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE[i]] = (cassetteOrderExtendStatusList.ContainsKey(i)) ? cassetteOrderExtendStatusList[i].ToString() : "";
				}
				// Set return action (display cassette)
				orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_RETURN_ACTION] = cassetteReturnAction.ToString();
				// Set return reason kbn (display cassette)
				orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_REASON_KBN] = cassetteReturnReasonKbn;
				// Set return reason memo (display cassette)
				orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_REASON_MEMO] = cassetteReturnReasonMemo;
				orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_RETURN_ACTION] = Constants.FLG_ORDERWORKFLOWSETTING_RETURN_ACTION_NOT_CHANGE;
				// 領収書出力フラグ
				orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_RECEIPT_OUTPUT_FLG_CHANGE] = cassetteReceiptOutputFlgChange.ToString();
				// Taiwan Invoice
				orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_INVOICE_STATUS_CHANGE] = cassetteInvoiceStatusChange.ToString();
				orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_INVOICE_STATUS_API] = cassetteInvoiceStatusApi.ToString();
				orderWorkflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_ORDER_STOREPICKUP_STATUS_CHANGE] = cassetteStorePickupStatusChange.ToString();
				break;
		}

		return orderWorkflowSetting;
	}

	/// <summary>
	/// 定期ワークフロー設定入力データ取得
	/// </summary>
	/// <returns></returns>
	private Hashtable GetInputFixedPurchaseWorkflowSetting()
	{
		// 基本設定
		var fixedPurchaseWorkflowSetting = new Hashtable
		{
			{ Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_SHOP_ID, this.LoginOperatorShopId },
			{ Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_WORKFLOW_KBN, ddlWorkflowKbn.SelectedValue },
			{ Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_WORKFLOW_NAME, tbWorkflowName.Text },
			{ Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_DESC1, tbDesc1.Text },
			{ Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_DISPLAY_ORDER, tbDisplayOrder.Text },
			{ Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_DISPLAY_COUNT, ddlDisplayCount.SelectedValue },
			{ Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_VALID_FLG,
				(cbValidFlg.Checked
					? Constants.FLG_ORDERWORKFLOWSETTING_VALID_FLG_VALID
					: Constants.FLG_ORDERWORKFLOWSETTING_VALID_FLG_INVALID)},
			{ Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_WORKFLOW_DETAIL_KBN, Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_DETAIL_KBN_NORMAL },
			{ Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_DISPLAY_KBN, this.DisplayKbnValue },
			{ Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_ADDITIONAL_SEARCH_FLG, rblAdditionalSearchFlg.SelectedValue },
			{ Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_LAST_CHANGED, this.LoginOperatorName }
		};

		var fixedPurchaseItemProductId = tbfixedPurchaseItemProductId.Text.Trim();
		var fixedPurchaseCountFrom = tbFixedPurchaseCountFrom.Text.Trim();
		var fixedPurchaseCountTo = tbFixedPurchaseCountTo.Text.Trim();
		var fixedPurchaseShippedCountFrom = tbFixedPurchaseShippedCountFrom.Text.Trim();
		var fixedPurchaseShippedCountTo = tbFixedPurchaseShippedCountTo.Text.Trim();
		var fixedPurchaseSubscriptionBoxCourseId = tbFixedPurchaseSubscriptionBoxCourseId.Text.Trim();
		var fixedPurchaseSubscriptionBoxOrderCountFrom = tbFixedPurchaseSubscriptionBoxOrderCountFrom.Text.Trim();
		var fixedPurchaseSubscriptionBoxOrderCountTo = tbFixedPurchaseSubscriptionBoxOrderCountTo.Text.Trim();
		var createdDateTo = tbCreatedDateTo.Text.Trim();
		var updatedDateTo = tbUpdatedDateTo.Text.Trim();
		var nextShippingDateFrom = tbNextShippingDateFrom.Text.Trim();
		var nextShippingDateTo = tbNextShippingDateTo.Text.Trim();
		var nextNextShippingDateFrom = tbNextNextShippingDateFrom.Text.Trim();
		var nextNextShippingDateTo = tbNextNextShippingDateTo.Text.Trim();
		var lastOrderDateTo = tbLastOrderDateTo.Text.Trim();
		var fixedPurchaseDateTo = tbFixedPurchaseDateTo.Text.Trim();
		var resumeDateFrom = tbResumeDateFrom.Text.Trim();
		var resumeDateTo = tbResumeDateTo.Text.Trim();

		// 抽出検索条件設定
		var searchSql = new StringBuilder();
		// 注文区分
		searchSql.Append(Constants.FIELD_FIXEDPURCHASE_ORDER_KBN).Append("=").Append(CreateSearchStringParts(cblFixedPurchaseOrderKbn.Items));
		// 並び順
		searchSql.Append("&").Append(Constants.REQUEST_KEY_SORT_KBN).Append("=").Append(ddlFixedPurchaseSortKbn.SelectedValue);
		// 注文ステータス
		searchSql.Append("&").Append(Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_STATUS).Append("=").Append(CreateSearchStringParts(cblFixedPurchaseStatus.Items));
		// 商品ID
		searchSql.Append("&").Append(Constants.FIELD_FIXEDPURCHASEITEM_PRODUCT_ID).Append("=").Append(fixedPurchaseItemProductId);
		fixedPurchaseWorkflowSetting.Add(Constants.FIELD_FIXEDPURCHASEITEM_PRODUCT_ID, fixedPurchaseItemProductId);
		// 購入区分
		searchSql.Append("&").Append(Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_KBN).Append("=").Append(CreateSearchStringParts(cblFixedPurchaseKbn.Items));
		// 配送種別
		searchSql.Append("&").Append(Constants.FIELD_PRODUCT_SHIPPING_TYPE).Append("=").Append(CreateSearchStringParts(cblFixedPurchaseShippingKbn.Items));
		// 決済種別
		searchSql.Append("&").Append(Constants.FIELD_FIXEDPURCHASE_ORDER_PAYMENT_KBN).Append("=").Append(CreateSearchStringParts(cblFixedPurchasePaymentKbn.Items, true));

		// 管理メモ
		searchSql.AppendFormat(
			"&{0}={1}",
			Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_MANAGEMENT_MEMO,
			CreateSearchStringParts(cblFixedPurchaseManagementMemoFlg.Items));
		searchSql.AppendFormat(
			"&{0}={1}",
			Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_MANAGEMENT_MEMO + Constants.CONST_FIELD_EXTEND_SEARCH_TEXT,
			HttpUtility.UrlEncode(tbFixedPurchaseManagementMemo.Text));

		// 配送メモ
		searchSql.AppendFormat(
			"&{0}={1}",
			Constants.FIELD_FIXEDPURCHASE_SHIPPING_MEMO,
			CreateSearchStringParts(cblFpShippingMemoFlg.Items));
		searchSql.AppendFormat(
			"&{0}={1}",
			Constants.FIELD_FIXEDPURCHASE_SHIPPING_MEMO + Constants.CONST_FIELD_EXTEND_SEARCH_TEXT,
			HttpUtility.UrlEncode(tbFpShippingMemo.Text));

		// 購入回数(注文基準from)
		searchSql.Append("&").Append(WorkflowSetting.m_FIXEDPURCHASE_ORDER_COUNT_FROM).Append("=").Append(fixedPurchaseCountFrom);
		fixedPurchaseWorkflowSetting.Add(WorkflowSetting.m_FIXEDPURCHASE_ORDER_COUNT_FROM, fixedPurchaseCountFrom);
		// 購入回数(注文基準to)
		searchSql.Append("&").Append(WorkflowSetting.m_FIXEDPURCHASE_ORDER_COUNT_TO).Append("=").Append(fixedPurchaseCountTo);
		fixedPurchaseWorkflowSetting.Add(WorkflowSetting.m_FIXEDPURCHASE_ORDER_COUNT_TO, fixedPurchaseCountTo);
		// 購入回数(出荷基準from)
		searchSql.Append("&").Append(WorkflowSetting.m_FIXEDPURCHASE_SHIPPED_COUNT_FROM).Append("=").Append(fixedPurchaseShippedCountFrom);
		fixedPurchaseWorkflowSetting.Add(WorkflowSetting.m_FIXEDPURCHASE_SHIPPED_COUNT_FROM, fixedPurchaseShippedCountFrom);
		// 購入回数(出荷基準to)
		searchSql.Append("&").Append(WorkflowSetting.m_FIXEDPURCHASE_SHIPPED_COUNT_TO).Append("=").Append(fixedPurchaseShippedCountTo);
		fixedPurchaseWorkflowSetting.Add(WorkflowSetting.m_FIXEDPURCHASE_SHIPPED_COUNT_TO, fixedPurchaseShippedCountTo);
		// 頒布会コースID
		searchSql.Append("&").Append(Constants.FIELD_FIXEDPURCHASE_SUBSCRIPTION_BOX_COURSE_ID)
			.Append("=").Append(fixedPurchaseSubscriptionBoxCourseId);
		// 頒布会注文回数(from)
		searchSql.Append("&").Append(WorkflowSetting.SUBSCRIPTIONBOX_ORDER_COUNT_FROM)
			.Append("=").Append(fixedPurchaseSubscriptionBoxOrderCountFrom);
		fixedPurchaseWorkflowSetting.Add(WorkflowSetting.SUBSCRIPTIONBOX_ORDER_COUNT_FROM, fixedPurchaseSubscriptionBoxOrderCountFrom);
		// 頒布会注文回数(to)
		searchSql.Append("&").Append(WorkflowSetting.SUBSCRIPTIONBOX_ORDER_COUNT_TO).Append("=").Append(fixedPurchaseSubscriptionBoxOrderCountTo);
		fixedPurchaseWorkflowSetting.Add(WorkflowSetting.SUBSCRIPTIONBOX_ORDER_COUNT_TO, fixedPurchaseSubscriptionBoxOrderCountTo);

		// 作成日時(from)
		searchSql.Append("&").Append(Constants.FIELD_FIXEDPURCHASE_DATE_CREATED).Append("=");
		searchSql.Append(createdDateTo);
		if (string.IsNullOrEmpty(createdDateTo) == false)
		{
			// 日時指定ありのチェックボックスがONの場合のみ、日数の入力チェックを行う
			fixedPurchaseWorkflowSetting.Add(WorkflowSetting.m_FIXEDPURCHASE_DATE_CREATED_TO, createdDateTo);
		}
		// 更新日時(to)
		searchSql.Append("&").Append(Constants.FIELD_FIXEDPURCHASE_DATE_CHANGED).Append("=");
		searchSql.Append(updatedDateTo);
		if (string.IsNullOrEmpty(updatedDateTo) == false)
		{
			// 日時指定ありのチェックボックスがONの場合のみ、日数の入力チェックを行う
			fixedPurchaseWorkflowSetting.Add(WorkflowSetting.m_FIXEDPURCHASE_DATE_CHANGED_TO, updatedDateTo);
		}
		// 次回配送日時(from)
		searchSql.Append("&").Append(Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_DATE).Append("=");
		searchSql.Append(((nextShippingDateFrom != string.Empty) || (nextShippingDateTo != string.Empty))
			? string.Format("{0}~{1}", nextShippingDateFrom, nextShippingDateTo)
			: string.Empty);
		if (string.IsNullOrEmpty(nextShippingDateFrom) == false)
		{
			// 日時指定ありのチェックボックスがONの場合のみ、日数の入力チェックを行う
			fixedPurchaseWorkflowSetting.Add(WorkflowSetting.m_FIXEDPURCHASE_NEXT_SHIPPING_DATE_FROM, nextShippingDateFrom);
		}
		if (string.IsNullOrEmpty(nextShippingDateTo) == false)
		{
			// 日時指定ありのチェックボックスがONの場合のみ、日数の入力チェックを行う
			fixedPurchaseWorkflowSetting.Add(WorkflowSetting.m_FIXEDPURCHASE_NEXT_SHIPPING_DATE_TO, nextShippingDateTo);
		}
		// 次々回配送日時(from)
		searchSql.Append("&").Append(Constants.FIELD_FIXEDPURCHASE_NEXT_NEXT_SHIPPING_DATE).Append("=");
		searchSql.Append(((nextNextShippingDateFrom != string.Empty) || (nextNextShippingDateTo != string.Empty))
			? string.Format("{0}~{1}", nextNextShippingDateFrom, nextNextShippingDateTo)
			: string.Empty);
		if (string.IsNullOrEmpty(nextNextShippingDateFrom) == false)
		{
			// 日時指定ありのチェックボックスがONの場合のみ、日数の入力チェックを行う
			fixedPurchaseWorkflowSetting.Add(WorkflowSetting.m_FIXEDPURCHASE_NEXT_NEXT_SHIPPING_DATE_FROM, nextNextShippingDateFrom);
		}
		if (string.IsNullOrEmpty(nextNextShippingDateTo) == false)
		{
			// 日時指定ありのチェックボックスがONの場合のみ、日数の入力チェックを行う
			fixedPurchaseWorkflowSetting.Add(WorkflowSetting.m_FIXEDPURCHASE_NEXT_NEXT_SHIPPING_DATE_TO, nextNextShippingDateTo);
		}
		// 最終購入日(To)
		searchSql.Append("&").Append(Constants.FIELD_FIXEDPURCHASE_LAST_ORDER_DATE).Append("=");
		searchSql.Append(lastOrderDateTo);
		if (string.IsNullOrEmpty(lastOrderDateTo) == false)
		{
			// 日時指定ありのチェックボックスがONの場合のみ、日数の入力チェックを行う
			fixedPurchaseWorkflowSetting.Add(WorkflowSetting.m_FIXEDPURCHASE_LAST_ORDER_DATE_TO, lastOrderDateTo);
		}
		// 購入開始日(To)
		searchSql.Append("&").Append(Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_DATE_BGN).Append("=");
		searchSql.Append(fixedPurchaseDateTo);
		if (string.IsNullOrEmpty(fixedPurchaseDateTo) == false)
		{
			// 日時指定ありのチェックボックスがONの場合のみ、日数の入力チェックを行う
			fixedPurchaseWorkflowSetting.Add(WorkflowSetting.m_FIXEDPURCHASE_FIXED_PURCHASE_DATE_BGN_TO, fixedPurchaseDateTo);
		}

		// 注文拡張ステータス変更区分
		foreach (RepeaterItem ri in rFixedPurchaseExtendStatusForSearch.Items)
		{
			var blHasSelected = false;
			searchSql.Append("&").Append(Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_BASENAME + ((HiddenField)(ri.FindControl("hfExtendStatusChangeNo1"))).Value + "=");
			if (((CheckBox)(ri.FindControl("cbExtendStatusOn"))).Checked)
			{
				searchSql.Append(Constants.FLG_ORDER_ORDER_EXTEND_STATUS_ON);
				blHasSelected = true;
			}
			if (((CheckBox)(ri.FindControl("cbExtendStatusOff"))).Checked)
			{
				if (blHasSelected)
				{
					searchSql.Append(",");
				}
				searchSql.Append(Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF);
			}
		}

		// 定期再開予定日
		searchSql.Append("&").Append(Constants.FIELD_FIXEDPURCHASE_RESUME_DATE).Append("=");
		searchSql.Append((cbResumeDateOn.Checked
			&& ((string.IsNullOrEmpty(resumeDateFrom) == false) || (string.IsNullOrEmpty(resumeDateTo) == false)))
				? string.Format("{0}~{1},", resumeDateFrom, resumeDateTo)
				: string.Empty);
		searchSql.Append(cbResumeDateOn.Checked ? WorkflowSetting.m_FIXEDPURCHASE_RESUME_DATE_SPECIFIED : string.Empty);
		searchSql.Append((cbResumeDateOn.Checked && cbResumeDateOff.Checked) ? "," : "")
			.Append(cbResumeDateOff.Checked ? WorkflowSetting.m_FIXEDPURCHASE_RESUME_DATE_UNSPECIFIED : string.Empty);

		// 定期再開予定日指定ありのチェックボックスがONの場合のみ、日数の入力チェックを行う
		if (cbResumeDateOn.Checked)
		{
			fixedPurchaseWorkflowSetting.Add(WorkflowSetting.m_FIXEDPURCHASE_RESUME_DATE_FROM, resumeDateFrom);
			fixedPurchaseWorkflowSetting.Add(WorkflowSetting.m_FIXEDPURCHASE_RESUME_DATE_TO, resumeDateTo);
		}

		// 領収書希望フラグ
		searchSql.AppendFormat(
			"&{0}={1}",
			Constants.FIELD_FIXEDPURCHASE_RECEIPT_FLG,
			CreateSearchStringParts(cblFixedPurchaseReceiptFlg.Items));

		foreach (var item in rFixedPurchaseOrderExtend.Items.Cast<RepeaterItem>())
		{
			var whfSettingId = GetWrappedControl<WrappedHiddenField>(item, "hfSettingId").Value;
			var wcbOrderExtend = GetWrappedControl<WrappedCheckBoxList>(item, "cblOrderExtendAttribute");
			searchSql.AppendFormat(
				"&{0}={1}",
				whfSettingId,
				CreateSearchStringParts(wcbOrderExtend.Items));
		}

		fixedPurchaseWorkflowSetting.Add(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_SEARCH_SETTING, searchSql.ToString());

		// アクション設定
		var fixedPurchaseIsAliveChange = "";
		var fixedPurchasePaymentStatusChange = "";
		var fixedPurchaseNextShippingDate = "";
		var fixedPurchaseNextNextShippingDate = "";
		var fixedPurchaseStopUnavailableShippingAreaChange = "";
		var fixedPurchaseExtendStatusChange = new string[Constants.CONST_ORDER_EXTEND_STATUS_DBFIELDS_MAX];

		fixedPurchaseIsAliveChange = rblFixedPurchaseIsAliveChange.SelectedValue;				// 定期購入状況変更区分
		fixedPurchasePaymentStatusChange = rblFixedPurchasePaymentStatusChange.SelectedValue;	// 定期決済ステータス変更区分
		fixedPurchaseNextShippingDate = rblFixedPurchaseNextShippingDate.SelectedValue;	// 次回配送日変更区分
		fixedPurchaseNextNextShippingDate = rblFixedPurchaseNextNextShippingDate.SelectedValue;	// 次々回配送日変更区分
		fixedPurchaseStopUnavailableShippingAreaChange = rblFixedPurchaseStopUnavailableShippingAreaChange.SelectedValue;	// 配送不可エリア停止変更
		// 定期拡張ステータス更新区分１～４０
		foreach (RepeaterItem ri in rFixedPurchaseExtendStatusForAction.Items)
		{
			fixedPurchaseExtendStatusChange[int.Parse(((HiddenField)(ri.FindControl("hfExtendStatusChangeNo"))).Value) - 1]
				= ((RadioButtonList)(ri.FindControl("rblExtendStatusChange"))).SelectedValue;
		}

		//------------------------------------------------------
		// 複数指定アクション設定
		//------------------------------------------------------
		var cassetteFixedPurchaseIsAliveChange = new StringBuilder();
		var cassetteFixedPurchasePaymentStatusChange = new StringBuilder();
		var cassetteFixedPurchaseNextShippingDate = new StringBuilder();
		var cassetteFixedPurchaseNextNextShippingDate = new StringBuilder();
		var cassetteFixedPurchaseStopUnavailableShippingAreaChange = new StringBuilder();
		var cassetteFixedPurchaseExtendStatusChange = new Dictionary<int, StringBuilder>();

		// 通常注文の場合
		var cassetteDefaultSelect = new StringBuilder();
			// 「何もしない」チェック有無設定
		fixedPurchaseWorkflowSetting.Add(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_NO_UPDATE, ((cbCassetteFixedPurchaseDoNothingFlg.Checked)
			? Constants.FLG_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_NO_UPDATE_ON
			: Constants.FLG_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_NO_UPDATE_OFF));

		// 定期購入状況変更
		foreach (RepeaterItem riSetting in rUpdateFixedPurchaseIsAliveList.Items)
		{
			// Cancel fixed purchase
			fixedPurchaseWorkflowSetting[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CANCEL_REASON_ID] = string.Empty;
			fixedPurchaseWorkflowSetting[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CANCEL_MEMO] = string.Empty;

			if (((CheckBox)riSetting.FindControl("cbCassetteFixedPurchaseIsAliveChange")).Checked)
			{
				GetInputCassetteAction(
					riSetting,
					Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_IS_ALIVE_CHANGE,
					cassetteDefaultSelect,
					cassetteFixedPurchaseIsAliveChange);

				// Get Reason
				if (((HiddenField)riSetting.FindControl("hfCassetteActionFieldValue")).Value == Constants.FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_IS_ALIVE_CHANGE_ACTION_CANCEL)
				{
					var memo = ((TextBox)riSetting.FindControl("tbCancelMemo")).Text.Trim();
					var selectedValue = ((DropDownList)riSetting.FindControl("ddlCassetteCancelReason")).SelectedValue;
					fixedPurchaseWorkflowSetting[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CANCEL_REASON_ID] = selectedValue;
					fixedPurchaseWorkflowSetting[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CANCEL_MEMO] = memo;
				}
			}
		}

		// 定期決済ステータス変更
		foreach (RepeaterItem riSetting in rUpdatePaymentStatusList.Items)
		{
			if (((CheckBox)riSetting.FindControl("cbCassettePaymentStatusChange")).Checked)
			{
				GetInputCassetteAction(
					riSetting,
					Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_PAYMENT_STATUS_CHANGE,
					cassetteDefaultSelect,
					cassetteFixedPurchasePaymentStatusChange);
			}
		}

		// 配送不可エリア停止変更
		foreach (RepeaterItem riSetting in rUpdateStopUnavailableShippingAreaList.Items)
		{
			if (((CheckBox)riSetting.FindControl("cbCassetteStopUnavailableShippingAreaChange")).Checked)
			{
				GetInputCassetteAction(
					riSetting,
					Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_STOP_UNAVAILABLE_SHIPPING_AREA_CHANGE,
					cassetteDefaultSelect,
					cassetteFixedPurchaseStopUnavailableShippingAreaChange);
			}
		}

		// 拡張ステータス１～４０
		foreach (RepeaterItem riParent in rFixedPurchaseExtendStatusSettingList.Items)
		{
			var cassetteFixedPurchaseExtendStatus = new StringBuilder();
			foreach (RepeaterItem riSetting in ((Repeater)riParent.FindControl("rCassetteFixedPurchaseExtendStatusChildList")).Items)
			{
				if (((CheckBox)riSetting.FindControl("cbCassetteFixedPurchaseExtendStatus")).Checked)
				{
					GetInputCassetteAction(
						riSetting,
						WorkflowSetting.m_FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE[int.Parse(((HiddenField)riParent.FindControl("htExtendStatusNo")).Value) - 1],
						WorkflowSetting.m_FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE[int.Parse(((HiddenField)riParent.FindControl("htExtendStatusNo")).Value) - 1] + "_",
						cassetteDefaultSelect,
						cassetteFixedPurchaseExtendStatus);
				}
			}

			int iExtendStatusNo;
			if (int.TryParse(((HiddenField)riParent.FindControl("htExtendStatusNo")).Value, out iExtendStatusNo) == false)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			cassetteFixedPurchaseExtendStatusChange.Add(iExtendStatusNo - 1, cassetteFixedPurchaseExtendStatus);
		}

		// アクション設定格納（ライン表示・カセット表示）
		// ライン表示（初期化）
		fixedPurchaseWorkflowSetting.Add(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_IS_ALIVE_CHANGE, "");
		fixedPurchaseWorkflowSetting.Add(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_PAYMENT_STATUS_CHANGE, "");
		fixedPurchaseWorkflowSetting.Add(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_NEXT_SHIPPING_DATE_CHANGE, "");
		fixedPurchaseWorkflowSetting.Add(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_NEXT_NEXT_SHIPPING_DATE_CHANGE, "");
		fixedPurchaseWorkflowSetting.Add(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_STOP_UNAVAILABLE_SHIPPING_AREA_CHANGE, "");
		for (var i = 0; i < Constants.CONST_ORDER_EXTEND_STATUS_DBFIELDS_MAX; i++)
		{
			fixedPurchaseWorkflowSetting.Add(WorkflowSetting.m_FIELD_FIXEDPURCHASEWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE[i], "");
		}
		// カセット表示（初期化）
		fixedPurchaseWorkflowSetting.Add(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_DEFAULT_SELECT, "");
		fixedPurchaseWorkflowSetting.Add(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_IS_ALIVE_CHANGE, "");
		fixedPurchaseWorkflowSetting.Add(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_PAYMENT_STATUS_CHANGE, "");
		fixedPurchaseWorkflowSetting.Add(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_STOP_UNAVAILABLE_SHIPPING_AREA_CHANGE, "");
		fixedPurchaseWorkflowSetting.Add(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_NEXT_SHIPPING_DATE_CHANGE, "");
		fixedPurchaseWorkflowSetting.Add(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_NEXT_NEXT_SHIPPING_DATE_CHANGE, "");
		for (var i = 0; i < Constants.CONST_ORDER_EXTEND_STATUS_DBFIELDS_MAX; i++)
		{
			fixedPurchaseWorkflowSetting.Add(WorkflowSetting.m_FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE[i], "");
		}

		// セット
		switch ((string)fixedPurchaseWorkflowSetting[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_DISPLAY_KBN])
		{
			case Constants.FLG_ORDERWORKFLOWSETTING_DISPLAY_KBN_LINE:
				fixedPurchaseWorkflowSetting[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_IS_ALIVE_CHANGE] = fixedPurchaseIsAliveChange;
				fixedPurchaseWorkflowSetting[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_PAYMENT_STATUS_CHANGE] = fixedPurchasePaymentStatusChange;
				fixedPurchaseWorkflowSetting[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_NEXT_SHIPPING_DATE_CHANGE] = fixedPurchaseNextShippingDate;
				fixedPurchaseWorkflowSetting[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_NEXT_NEXT_SHIPPING_DATE_CHANGE] = fixedPurchaseNextNextShippingDate;
				fixedPurchaseWorkflowSetting[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_STOP_UNAVAILABLE_SHIPPING_AREA_CHANGE] = fixedPurchaseStopUnavailableShippingAreaChange;
				for (var i = 0; i < Constants.CONST_ORDER_EXTEND_STATUS_DBFIELDS_MAX; i++)
				{
					fixedPurchaseWorkflowSetting[WorkflowSetting.m_FIELD_FIXEDPURCHASEWORKFLOWSETTING_ORDER_EXTEND_STATUS_CHANGE[i]] = ((fixedPurchaseExtendStatusChange[i] != null) ? fixedPurchaseExtendStatusChange[i] : "");
				}
				if (rblFixedPurchaseIsAliveChange.SelectedValue == Constants.FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_IS_ALIVE_CHANGE_ACTION_CANCEL)
				{
					fixedPurchaseWorkflowSetting[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CANCEL_MEMO] = tbCancelMemo.Text;
					fixedPurchaseWorkflowSetting[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CANCEL_REASON_ID] = ddlCancelReason.SelectedValue;
				}
				else
				{
					fixedPurchaseWorkflowSetting[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CANCEL_MEMO] = string.Empty;
					fixedPurchaseWorkflowSetting[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CANCEL_REASON_ID] = string.Empty;
				}
				break;

			case Constants.FLG_ORDERWORKFLOWSETTING_DISPLAY_KBN_CASSETTE:
				fixedPurchaseWorkflowSetting[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_DEFAULT_SELECT] = cassetteDefaultSelect.ToString();
				fixedPurchaseWorkflowSetting[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_IS_ALIVE_CHANGE] = cassetteFixedPurchaseIsAliveChange.ToString();
				fixedPurchaseWorkflowSetting[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_PAYMENT_STATUS_CHANGE] = cassetteFixedPurchasePaymentStatusChange.ToString();
				fixedPurchaseWorkflowSetting[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_FIXED_PURCHASE_STOP_UNAVAILABLE_SHIPPING_AREA_CHANGE] = cassetteFixedPurchaseStopUnavailableShippingAreaChange.ToString();
				fixedPurchaseWorkflowSetting[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_NEXT_SHIPPING_DATE_CHANGE] = cassetteFixedPurchaseNextShippingDate.ToString();
				fixedPurchaseWorkflowSetting[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_NEXT_NEXT_SHIPPING_DATE_CHANGE] = cassetteFixedPurchaseNextNextShippingDate.ToString();
				for (var i = 0; i < Constants.CONST_ORDER_EXTEND_STATUS_DBFIELDS_MAX; i++)
				{
					fixedPurchaseWorkflowSetting[WorkflowSetting.m_FIELD_FIXEDPURCHASEWORKFLOWSETTING_CASSETTE_ORDER_EXTEND_STATUS_CHANGE[i]] = (cassetteFixedPurchaseExtendStatusChange.ContainsKey(i)) ? cassetteFixedPurchaseExtendStatusChange[i].ToString() : "";
				}
				break;
		}

		return fixedPurchaseWorkflowSetting;
	}

	/// <summary>
	/// 入力カセットアクション情報取得
	/// </summary>
	/// <param name="riTarget">対象リピータアイテム</param>
	/// <param name="strFieldName">対象フィールド名</param>
	/// <param name="sbCassetteDefaultSelectFieldValue">カセット表示用初期選択値（書き換えます）</param>
	/// <param name="sbStatusChangeFieldValue">カセット表示用変更区分（書き換えます）</param>
	private void GetInputCassetteAction(
		RepeaterItem riTarget,
		string strFieldName,
		StringBuilder sbCassetteDefaultSelectFieldValue,
		StringBuilder sbStatusChangeFieldValue)
	{
		GetInputCassetteAction(riTarget, strFieldName, strFieldName, sbCassetteDefaultSelectFieldValue, sbStatusChangeFieldValue);
	}
	/// <summary>
	/// 入力カセットアクション情報取得
	/// </summary>
	/// <param name="riTarget">対象リピータアイテム</param>
	/// <param name="strFieldName">対象フィールド名</param>
	/// <param name="strFieldNameCompare">比較対象フィールド名</param>
	/// <param name="sbCassetteDefaultSelectFieldValue">カセット表示用初期選択値（書き換えます）</param>
	/// <param name="sbStatusChangeFieldValue">カセット表示用変更区分（書き換えます）</param>
	private void GetInputCassetteAction(
		RepeaterItem riTarget,
		string strFieldName,
		string strFieldNameCompare,
		StringBuilder sbCassetteDefaultSelectFieldValue,
		StringBuilder sbStatusChangeFieldValue)
	{
		// 初期選択設定（「フィールド名＆ステータス値」）
		if (StringUtility.ToEmpty(Request.Form["CassetteDefaultSelect"]) == strFieldNameCompare + riTarget.ItemIndex.ToString())
		{
			sbCassetteDefaultSelectFieldValue.Append(strFieldName);
			sbCassetteDefaultSelectFieldValue.Append("&");
			sbCassetteDefaultSelectFieldValue.Append(((HiddenField)riTarget.FindControl("hfCassetteActionFieldValue")).Value);
		}
		// 変更区分設定（「ステータス値＆メールID,ステータス値＆メールID,･･･」）
		sbStatusChangeFieldValue.Append((sbStatusChangeFieldValue.Length != 0) ? "," : "");
		sbStatusChangeFieldValue.Append(((HiddenField)riTarget.FindControl("hfCassetteActionFieldValue")).Value);
		sbStatusChangeFieldValue.Append("&");
		var cassettemailId = (riTarget.FindControl("ddlCassetteMailId") != null)
			? ((DropDownList)riTarget.FindControl("ddlCassetteMailId")).SelectedValue
			: string.Empty;
		sbStatusChangeFieldValue.Append(cassettemailId);
	}

	/// <summary>
	/// 注文ステータス変更区分取得
	/// </summary>
	/// <param name="strStatusChange">注文ステータス変更区分</param>
	/// <returns>注文ステータス変更区分</returns>
	/// <remarks>
	/// 実在庫利用が無効な場合で、在庫引当へ変更 OR 出荷完了を設定していた場合
	/// エラーになるため回避策として作成しました
	/// </remarks>
	protected string SetStatusChange(string strStatusChange)
	{
		foreach (ListItem li in rblOrderStatusChange.Items)
		{
			if (li.Value == strStatusChange)
			{
				return strStatusChange;
			}
		}

		return "";
	}

	/// <summary>
	/// ワークフロー区分選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlWorkflowKbn_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		tbodyOrderSearch.Visible = (ddlWorkflowKbn.SelectedValue != Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_KBN_RETURN_EXCHANGE);
		tbodyOrderAction.Visible = (tbodyOrderSearch.Visible && (this.DisplayKbnValue == Constants.FLG_ORDERWORKFLOWSETTING_DISPLAY_KBN_LINE));
		tbodyOrderActionCassette.Visible = tbodyOrderSearch.Visible && (this.DisplayKbnValue == Constants.FLG_ORDERWORKFLOWSETTING_DISPLAY_KBN_CASSETTE);
		tbodyReturnExchangeSearch.Visible = (ddlWorkflowKbn.SelectedValue == Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_KBN_RETURN_EXCHANGE);
		tbodyReturnExchangeAction.Visible = (tbodyReturnExchangeSearch.Visible && (this.DisplayKbnValue == Constants.FLG_ORDERWORKFLOWSETTING_DISPLAY_KBN_LINE));
		tbodyCassetteReturnExchangeAction.Visible = (tbodyReturnExchangeSearch.Visible && (this.DisplayKbnValue == Constants.FLG_ORDERWORKFLOWSETTING_DISPLAY_KBN_CASSETTE));
		tbodyMailId.Visible = (this.DisplayKbnValue == Constants.FLG_ORDERWORKFLOWSETTING_DISPLAY_KBN_LINE);
		tbodyInvoiceSatatusAction.Visible = (this.DisplayKbnValue == Constants.FLG_ORDERWORKFLOWSETTING_DISPLAY_KBN_LINE);

		// Set visible Return workflow
		divReturnOrder.Visible = (this.IsSelectedOrderWorkflow
			&& (ddlWorkflowKbn.SelectedValue != Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_KBN_RETURN_EXCHANGE));
		if (rbWorkFlowDetailKbnReturn.Checked
			&& (ddlWorkflowKbn.SelectedValue == Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_KBN_RETURN_EXCHANGE))
		{
			rbWorkFlowDetailKbnReturn.Checked = true;
			rblDisplayKbnReturn.Enabled = true;
			rblDisplayKbn.Enabled = false;
			rbWorkFlowDetailKbnNormal.Checked = false;
		}

		rbExternalOderInfoAction_OnCheckedChanged(sender, e);
	}

	/// <summary>
	/// コピー新規登録ボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCopyInsert_Click(object sender, EventArgs e)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDERWORKFLOWSETTING_REGISTER)
			.AddParam(Constants.REQUEST_KEY_WORKFLOW_TYPE, StringUtility.ToEmpty(this.RequestWorkflowType))
			.AddParam(Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_KBN, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_KBN]))
			.AddParam(Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_NO, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_NO]))
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_COPY_INSERT);

		Response.Redirect(url.CreateUrl());
	}

	/// <summary>
	/// 削除するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, System.EventArgs e)
	{
		// 削除処理
		using (var accessor = new SqlAccessor())
		{
			accessor.OpenConnection();
			accessor.BeginTransaction();

			if (this.RequestWorkflowType == WorkflowSetting.m_KBN_WORKFLOW_TYPE_ORDER)
			{
				this.DeleteOrderWorkflowSetting(accessor);
			}
			else
			{
				this.DeleteFixedPurchaseWorkflowSetting(accessor);
			}

			accessor.CommitTransaction();
		}

		// 一覧画面へ遷移
		Hashtable searchParameters = (Hashtable)Session[Constants.SESSIONPARAM_KEY_ORDER_WORKFLOW_SEARCH_INFO];
		Response.Redirect(CreatOrderWorkflowSettingListUrl(searchParameters, true, null));
	}

	/// <summary>
	/// 登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, System.EventArgs e)
	{
		// 最大件数をチェック
		if ((Constants.ORDERWORKFLOWSETTING_MAXCOUNT.HasValue)
			&& (this.OrderWorkflowSettingItemsCount >= Constants.ORDERWORKFLOWSETTING_MAXCOUNT))
		{
			var errorMessage = string.Format(
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERWORKFLOWSETTING_REGISTER_OVER_MAX_COUNT),
				Constants.ORDERWORKFLOWSETTING_MAXCOUNT);
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		var insertParam = new Hashtable();
		var workflowType = "";
		if (this.IsSelectedOrderWorkflow)
		{
			CheckExchangeOrderGmo();
			insertParam = GetInputOrderWorkflowSetting();
			InsertOrderWorkflow(insertParam);
			workflowType = WorkflowSetting.m_KBN_WORKFLOW_TYPE_ORDER;
		}
		else
		{
			insertParam = GetInputFixedPurchaseWorkflowSetting();
			InsertFixedPurchaseWorkflow(insertParam);
			workflowType = WorkflowSetting.m_KBN_WORKFLOW_TYPE_FIXEDPURCHASE;
		}

		// Update order count
		new WorkflowUtility().UpdateOrderCountForWorkflowSetting(
			this.LoginOperatorShopId,
			workflowType,
			insertParam[Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_KBN].ToString(),
			insertParam[Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_NO].ToString());

		// 登録・更新完了メッセージをセット
		Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERWORKFLOWSETTING_REGIST_UPDATE_SUCCESS);

		// 詳細表示画面へ遷移
		this.RedirectToRegisterPage(
			workflowType,
			insertParam[Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_KBN].ToString(),
			insertParam[Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_NO].ToString());
	}

	/// <summary>
	/// 更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, System.EventArgs e)
	{
		var workflowNo = "";
		var workflowKbnNew = "";
		var workflowType = "";
		if (this.IsSelectedOrderWorkflow)
		{
			CheckExchangeOrderGmo();
			workflowKbnNew = ddlWorkflowKbn.SelectedValue;
			workflowNo = UpdateOrderWorkflow(GetInputOrderWorkflowSetting());
			workflowType = WorkflowSetting.m_KBN_WORKFLOW_TYPE_ORDER;
		}
		else
		{
			workflowKbnNew = ddlWorkflowKbn.SelectedValue;
			workflowNo = UpdateFixedPurchaseWorkflow(GetInputFixedPurchaseWorkflowSetting());
			workflowType = WorkflowSetting.m_KBN_WORKFLOW_TYPE_FIXEDPURCHASE;
		}

		// Update order count
		new WorkflowUtility().UpdateOrderCountForWorkflowSetting(
			this.LoginOperatorShopId,
			workflowType,
			workflowKbnNew,
			workflowNo);

		// 登録・更新完了メッセージをセット
		Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERWORKFLOWSETTING_REGIST_UPDATE_SUCCESS);

		// 詳細表示画面へ遷移
		this.RedirectToRegisterPage(workflowType, workflowKbnNew, workflowNo);
	}

	/// <summary>
	/// 戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBack_Click(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// 詳細表示画面へ遷移
		//------------------------------------------------------
		Hashtable searchParameters = (Hashtable)Session[Constants.SESSIONPARAM_KEY_ORDER_WORKFLOW_SEARCH_INFO];
		Response.Redirect(CreatOrderWorkflowSettingListUrl(searchParameters, true, null));
	}

	/// <summary>
	/// 注文日指定ラジオボタン変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblUpdateStatusDate_SelectedIndexChanged(object sender, EventArgs e)
	{
		ddlUpdateStatusDateFrom.Enabled
			= ddlUpdateStatusDateTo.Enabled
			= ddlUpdateStatusHourTo.Enabled
			= ddlUpdateStatusMinuteTo.Enabled
			= ddlUpdateStatusSecondTo.Enabled
			= (rblUpdateStatusDate.SelectedValue == "0");
	}

	/// <summary>
	/// 返品交換返金日指定ラジオボタン変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblReturnExchangeUpdateStatusDate_SelectedIndexChanged(object sender, EventArgs e)
	{
		ddlReturnExchangeUpdateStatusDateFrom.Enabled
			= ddlReturnExchangeUpdateStatusDateTo.Enabled
			= ddlReturnExchangeUpdateStatusHourTo.Enabled
			= ddlReturnExchangeUpdateStatusMinuteTo.Enabled
			= ddlReturnExchangeUpdateStatusSecondTo.Enabled
			= (rblReturnExchangeUpdateStatusDate.SelectedValue == "0");
	}

	/// <summary>
	/// アクション対象区分ラジオボタン変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblDisplayKbn_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		var isDisplayLine = this.DisplayKbnValue == Constants.FLG_ORDERWORKFLOWSETTING_DISPLAY_KBN_LINE;
		var isDisplayCassette = this.DisplayKbnValue == Constants.FLG_ORDERWORKFLOWSETTING_DISPLAY_KBN_CASSETTE;

		// 通常ワークフローが選択されている場合
		if (rbWorkFlowDetailKbnNormal.Checked)
		{
			trAdditionalSearchFlg.Visible = true;
			rUpdateOrderInvoiceStatusCallApiSettingList.Visible = true;
			rUpdateOrderInvoiceStatusSettingList.Visible = true;
			if (this.IsSelectedOrderWorkflow)
			{
				divSearch.Visible
					= divAction.Visible = true;
				divFixedPurchaseSearch.Visible
					= divFixedPurchaseAction.Visible = false;
			}
			else if (this.IsSelectedFixedPurchaseWorkflow)
			{
				divSearch.Visible
					= divAction.Visible = false;
				divFixedPurchaseSearch.Visible
					= divFixedPurchaseAction.Visible = true;
			}

			// Set enable (workflow normal, workflow return)
			rblDisplayKbn.Enabled = true;
			rblDisplayKbnReturn.Enabled = false;

			// Set visible (order status, scheduled shipping date, payment status, external payment)
			trOrderStatusChange.Visible
				= trScheduledShippingDateAction.Visible
				= trPaymentStatusChange.Visible
				= trExternalPaymentAction.Visible
				= trExternalOrderInfoAction.Visible
				= isDisplayLine;
			trStorePickupStatusAction.Visible = isDisplayLine && Constants.STORE_PICKUP_OPTION_ENABLED;
			// Set visible cassette (order status, real stock, payment status, external payment)
			rUpdateOrderStatusSettingList.Visible
				= rUpdateProductRealStockSettingList.Visible
				= rUpdatePaymentStatusSettingList.Visible
				= rExternalPaymentActionSettingList.Visible
				= rExternalOrderInfoActionSettingList.Visible
				= isDisplayCassette;
			rUpdateStorePickupStatusSettingList.Visible = isDisplayCassette && Constants.STORE_PICKUP_OPTION_ENABLED;

			// Set visible return
			trReturn.Visible = false;
			// Set visible cassette return
			rReturnActionSettingList.Visible = false;
			// Set visible real stock
			Tr1.Visible = Constants.REALSTOCK_OPTION_ENABLED;
			// Set invoice status for one-line display
			tbodyInvoiceSatatusAction.Visible = isDisplayLine;
		}
		else if (rbWorkFlowDetailKbnReturn.Checked)
		{
			trAdditionalSearchFlg.Visible = true;
			rUpdateOrderInvoiceStatusCallApiSettingList.Visible = false;
			rUpdateOrderInvoiceStatusSettingList.Visible = false;

			if (this.IsSelectedOrderWorkflow)
			{
				divSearch.Visible
					= divAction.Visible = true;
				divFixedPurchaseSearch.Visible
					= divFixedPurchaseAction.Visible = false;
			}
			else if (this.IsSelectedFixedPurchaseWorkflow)
			{
				divSearch.Visible
					= divAction.Visible = false;
				divFixedPurchaseSearch.Visible
					= divFixedPurchaseAction.Visible = true;
			}

			// Set enable (workflow normal, workflow return)
			rblDisplayKbn.Enabled = false;
			rblDisplayKbnReturn.Enabled = true;

			// Set visible (order status, scheduled shipping date, payment status, external payment)
			trOrderStatusChange.Visible
				= trScheduledShippingDateAction.Visible
				= trPaymentStatusChange.Visible
				= trExternalPaymentAction.Visible
				= trExternalOrderInfoAction.Visible = false;

			// Set visible cassette (order status, real stock, payment status, external payment)
			rUpdateOrderStatusSettingList.Visible
				= rUpdateProductRealStockSettingList.Visible
				= rUpdatePaymentStatusSettingList.Visible
				= rExternalPaymentActionSettingList.Visible
				= rExternalOrderInfoActionSettingList.Visible = false;

			// Set visible return
			trReturn.Visible = isDisplayLine;
			// Set visible cassette return
			rReturnActionSettingList.Visible = isDisplayCassette;
			// Set visible real stock
			Tr1.Visible = false;
		}
		else
		{
			trAdditionalSearchFlg.Visible = false;
			rUpdateOrderInvoiceStatusCallApiSettingList.Visible = false;
			rUpdateOrderInvoiceStatusSettingList.Visible = false;

			// Set enable (workflow normal, worflow return)
			rblDisplayKbn.Enabled = false;
			rblDisplayKbnReturn.Enabled = false;

			divSearch.Visible
				= divAction.Visible = false;
			divFixedPurchaseSearch.Visible
				= divFixedPurchaseAction.Visible = false;
		}
		tbodyOrderAction.Visible = tbodyOrderSearch.Visible && isDisplayLine;
		tbodyOrderActionCassette.Visible = tbodyOrderSearch.Visible && isDisplayCassette;
		tbodyReturnExchangeAction.Visible = tbodyReturnExchangeSearch.Visible && isDisplayLine;
		tbodyCassetteReturnExchangeAction.Visible = tbodyReturnExchangeSearch.Visible && isDisplayCassette;
		tbodyFixedPurchaseAction.Visible = tbodyFixedPurchaseSearch.Visible && isDisplayLine;
		tbodyFixedPurchaseActionCassette.Visible = tbodyFixedPurchaseSearch.Visible && isDisplayCassette;
		tbodyMailId.Visible = isDisplayLine;

		// Run Script Set Visibility
		RunScriptSetVisibility();

		rbExternalOderInfoAction_OnCheckedChanged(sender, e);
	}

	/// <summary>
	/// アクション対象区分ラジオボタン変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblFixedPurchaseNextShippingDate_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		if (rblFixedPurchaseNextShippingDate.SelectedValue == Constants
			.FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_NEXT_SHIPPING_DATE_CHANGE_ACTION_ON_WITH_CALCULATE_NEXT_NEXT_SHIPPINGDATE)
		{
			rblFixedPurchaseNextNextShippingDate.SelectedValue = Constants
				.FLG_FIXEDPURCHASEWORKFLOWSETTING_FIXED_PURCHASE_NEXT_NEXT_SHIPPING_DATE_CHANGE_ACTION_OFF;
			rblFixedPurchaseNextNextShippingDate.Enabled = false;
		}
		else
		{
			rblFixedPurchaseNextNextShippingDate.Enabled = true;
		}
	}

	/// <summary>
	/// ワークフロー設定取得（ドロップダウンデータバインド用）
	/// </summary>
	/// <param name="strTargetFieldName">対象フィールド名</param>
	/// <param name="strDefaultValue">デフォルト値</param>
	/// <returns>ワークフロー設定値</returns>
	protected string GetOrderWorkflowSettingValue(string strTargetFieldName, string strDefaultValue = "")
	{
		return GetOrderWorkflowSettingValue(strTargetFieldName, this.OrderWorkflowSetting.WorkflowType, strDefaultValue);
	}
	/// <summary>
	/// ワークフロー設定取得（ドロップダウンデータバインド用）
	/// </summary>
	/// <param name="strTargetFieldName">対象フィールド名</param>
	/// <param name="workflowType">ワークフロー種別</param>
	/// <param name="strDefaultValue">デフォルト値</param>
	/// <returns>ワークフロー設定値</returns>
	protected string GetOrderWorkflowSettingValue(string strTargetFieldName, WorkflowSetting.WorkflowTypes workflowType, string strDefaultValue = "")
	{
		return (StringUtility.ToValue(this.OrderWorkflowSetting.GetValue(strTargetFieldName, workflowType), strDefaultValue)).ToString();
	}

	/// <summary>
	/// 指定したキーの抽出条件の値を取得
	/// </summary>
	/// <param name="keySearch">抽出条件のキー</param>
	/// <param name="workflowType">ワークフロー種別</param>
	/// <returns>抽出条件の値</returns>
	protected string GetSearchSettingValue(string keySearch, WorkflowSetting.WorkflowTypes workflowType)
	{
		var result = string.Empty;
		if ((this.OrderWorkflowSetting.Setting != null) && (this.OrderWorkflowSetting.WorkflowType == workflowType))
		{
			foreach (var item in this.OrderWorkflowSetting.GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_SEARCH_SETTING, workflowType).ToString().Split('&').Where(item => item.StartsWith(keySearch + "=")))
			{
				result = item.Split('=')[1];
			}
		}
		return HttpUtility.UrlDecode(result);
	}

	/// <summary>
	/// 拡張ステータス設定の値がON画を取得
	/// </summary>
	/// <param name="iExtendStatusNo">拡張ステータスNO（1～30)</param>
	/// <param name="strTargetValue">比較対象値</param>
	/// <param name="workflowType">ワークフロー種別</param>
	/// <returns>一致するか否か</returns>
	protected bool CheckExtendStatusSame(int iExtendStatusNo, string strTargetValue, WorkflowSetting.WorkflowTypes workflowType)
	{
		if (this.OrderWorkflowSetting.WorkflowType != workflowType) return false;
		if (m_dicExtendStatus.ContainsKey(FIELD_ORDER_EXTEND_STATUS + iExtendStatusNo.ToString()))
		{
			foreach (string strValue in m_dicExtendStatus[FIELD_ORDER_EXTEND_STATUS + iExtendStatusNo.ToString()].Split(','))
			{
				if (strValue == strTargetValue)
				{
					return true;
				}
			}
		}

		return false;
	}

	/// <summary>
	/// カセット表示用ステータス有効設定取得（チェックボックスチェック状態データバインド用）
	/// </summary>
	/// <param name="strTargetFieldName">対象変更区分フィールド名</param>
	/// <param name="strTargetStatusValue">対象ステータスの値</param>
	/// <param name="workflowType">ワークフロー種別</param>
	/// <returns>カセット表示に表示するステータスかどうか</returns>
	protected bool GetCassetteOrderStatusChangeValidity(string strTargetFieldName, string strTargetStatusValue, WorkflowSetting.WorkflowTypes workflowType)
	{
		if ((this.OrderWorkflowSetting.Setting != null) && (this.OrderWorkflowSetting.WorkflowType == workflowType))
		{
			foreach (string strSetting in ((string)this.OrderWorkflowSetting.GetValue(strTargetFieldName, workflowType)).Split(','))
			{
				string[] strItems = strSetting.Split('&');
				if (strItems[0] == strTargetStatusValue)
				{
					return true;
				}
			}
		}

		return false;
	}

	/// <summary>
	/// カセット表示用メールID取得（ドロップダウン選択状態データバインド用）
	/// </summary>
	/// <param name="strTargetFieldName">対象変更区分フィールド名</param>
	/// <param name="strTargetStatusValue">対象ステータスの値</param>
	/// <param name="workflowType">ワークフロー種別</param>
	/// <returns>対象ステータスに対応するメールID</returns>
	protected string GetCassetteMailId(string strTargetFieldName, string strTargetStatusValue, WorkflowSetting.WorkflowTypes workflowType)
	{
		if ((this.OrderWorkflowSetting.Setting != null) && (this.OrderWorkflowSetting.WorkflowType == workflowType))
		{
			foreach (string strSetting in ((string)this.OrderWorkflowSetting.GetValue(strTargetFieldName, workflowType)).Split(','))
			{
				string[] strItems = strSetting.Split('&');
				if ((strItems[0] == strTargetStatusValue) && (m_licMailIds.Cast<ListItem>().Any(mailId => mailId.Value == strItems[1])))
				{
					return strItems[1];
				}
			}
		}

		return "";
	}

	/// <summary>
	/// カセット表示用初期選択値取得（ラジオボタンチェック状態データバインド用）
	/// </summary>
	/// <param name="strTargetFieldName">対象フィールド名（空の場合）</param>
	/// <param name="strTargetStatusValue">対象ステータスの値</param>
	/// <param name="workflowType">ワークフロー種別</param>
	/// <returns></returns>
	protected bool GetCassetteDefaultSelectValid(string strTargetFieldName, string strTargetStatusValue, WorkflowSetting.WorkflowTypes workflowType)
	{
		if ((this.OrderWorkflowSetting.Setting != null) && (this.OrderWorkflowSetting.WorkflowType == workflowType))
		{
			// 「何もしない」チェック有のみラジオボタンを有効とする
			if (strTargetFieldName == "")
			{
				return ((string)this.OrderWorkflowSetting.GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_NO_UPDATE, workflowType) == Constants.FLG_ORDERWORKFLOWSETTING_CASSETTE_NO_UPDATE_ON);
			}

			return (((string)this.OrderWorkflowSetting.GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_CASSETTE_DEFAULT_SELECT, workflowType)) == strTargetFieldName + "&" + strTargetStatusValue);
		}

		return false;
	}

	/// <summary>
	/// 最終与信日時指定ありチェックボックス変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbLastAuthDateOn_OnCheckedChanged(object sender, EventArgs e)
	{
		tbLastAuthDateFrom.Enabled
			= tbLastAuthDateTo.Enabled
			= cbLastAuthDateOn.Checked;
	}

	/// <summary>
	/// 配送希望日指定ありチェックボックス変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbShippingDateOn_OnCheckedChanged(object sender, System.EventArgs e)
	{
		tbShippingDateFrom.Enabled = cbShippingDateOn.Checked;
		tbShippingDateTo.Enabled = cbShippingDateOn.Checked;
	}

	/// <summary>
	/// ノベルティIDありチェックボックス変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbNoveltyOn_OnCheckedChanged(object sender, System.EventArgs e)
	{
		tbNoveltyId.Enabled = cbNoveltyOn.Checked;
	}

	/// <summary>
	/// レコメンドIDありチェックボックス変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbRecommendOn_OnCheckedChanged(object sender, System.EventArgs e)
	{
		tbRecommendId.Enabled = cbRecommendOn.Checked;
	}

	/// <summary>
	/// Setting textbox based on AdvCode
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbAdvCode_OnCheckedChanged(object sender, System.EventArgs e)
	{
		tbAdvCode.Enabled = (cblAdvCodeFlg.Items.Cast<ListItem>().Any(item => item.Selected));
	}

	/// <summary>
	/// 受注ワークフロー削除
	/// </summary>
	/// <param name="accessor">SqlAccessor情報</param>
	/// <param name="isDelete">ワークフロー削除時の呼び出しか</param>
	private void DeleteOrderWorkflowSetting(SqlAccessor accessor, bool isDelete = true)
	{
		var shopId = this.OrderWorkflowSetting.GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_SHOP_ID);
		var workflowKbn = this.OrderWorkflowSetting.GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_KBN);
		var workflowNo = this.OrderWorkflowSetting.GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_NO);
		var scenarios = new OrderWorkflowScenarioSettingService()
			.GetByOrderworkflowSettingPrimaryKey(shopId, workflowKbn, int.Parse(workflowNo));
		if (scenarios.Any())
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = string.Format(
				WebMessages.GetMessages(
					(isDelete
						? WebMessages.ERRMSG_MANAGER_ORDERWORKFLOWSETTING_NOT_DELETE_IN_THE_SCENARIO
						: WebMessages.ERRMSG_MANAGER_ORDERWORKFLOWSETTING_NOT_UPDATE_IN_THE_SCENARIO))
					.Replace("@@ 1 @@", string.Join(",", scenarios.Select(scenario => scenario.ScenarioName))));
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		using (var statement = new SqlStatement("OrderWorkflowSetting", "DeleteOrderWorkflowSetting"))
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_ORDERWORKFLOWSETTING_SHOP_ID, shopId},
				{ Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_KBN, workflowKbn},
				{ Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_NO, workflowNo},
			};
			statement.ExecStatement(accessor, input);
		}
	}

	/// <summary>
	/// 定期ワークフロー削除
	/// </summary>
	/// <param name="accessor">SqlAccessor情報</param>
	private void DeleteFixedPurchaseWorkflowSetting(SqlAccessor accessor)
	{
		using (var statement = new SqlStatement("FixedPurchaseWorkflowSetting", "DeleteFixedPurchaseWorkflowSetting"))
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_SHOP_ID, this.OrderWorkflowSetting.GetValue(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_SHOP_ID)},
				{ Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_WORKFLOW_KBN, this.OrderWorkflowSetting.GetValue(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_WORKFLOW_KBN)},
				{ Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_WORKFLOW_NO, this.OrderWorkflowSetting.GetValue(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_WORKFLOW_NO)},
			};
			statement.ExecStatement(accessor, input);
		}
	}

	/// <summary>
	/// ワークフロー枝番取得
	/// </summary>
	/// <param name="accessor">SqlAccessor情報</param>
	/// <param name="shopId">店舗ID</param>
	/// <param name="workflowKbn">ワークフロー区分</param>
	/// <returns>ワークフロー設定No</returns>
	private int GetNewWorkflowNo(SqlAccessor accessor, string shopId, string workflowKbn)
	{
		var pageName =
			(this.IsSelectedOrderWorkflow)
				? "OrderWorkflowSetting"
				: "FixedPurchaseWorkflowSetting";
		using (var statement = new SqlStatement(pageName, "GetNewWorkflowNo"))
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_ORDERWORKFLOWSETTING_SHOP_ID, shopId},
				{ Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_KBN, workflowKbn},
			};
			var dv = statement.SelectSingleStatement(accessor, input);
			return (int)dv[0][0];
		}
	}

	/// <summary>
	/// 受注ワークフロー登録
	/// </summary>
	/// <param name="accessor">SqlAccessor情報</param>
	/// <param name="input">登録情報</param>
	private void InsertOrderWorkflowSetting(SqlAccessor accessor, Hashtable input)
	{
		var pageName = "";
		var statementName = "";
		if (this.IsSelectedOrderWorkflow)
		{
			pageName = "OrderWorkflowSetting";
			statementName = "InsertOrderWorkflowSetting";
		}
		else
		{
			pageName = "FixedPurchaseWorkflowSetting";
			statementName = "InsertFixedPurchaseWorkflowSetting";
		}
		using (var statement = new SqlStatement(pageName, statementName))
		{
			statement.ExecStatement(accessor, input);
		}
	}

	/// <summary>
	/// 詳細表示画面へ遷移
	/// </summary>
	/// <param name="workflowType">ワークフロー種別</param>
	/// <param name="workflowKbn">ワークフロー区分</param>
	/// <param name="workflowNo">ワークフロー枝番</param>
	private void RedirectToRegisterPage(string workflowType, string workflowKbn, string workflowNo)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDERWORKFLOWSETTING_REGISTER)
			.AddParam(Constants.REQUEST_KEY_WORKFLOW_TYPE, workflowType)
			.AddParam(Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_KBN, workflowKbn)
			.AddParam(Constants.REQUEST_KEY_ORDERWORKFLOWSETTING_WORKFLOW_NO, workflowNo)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_COMPLETE)
			.CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// Scheduled Shipping Date With CheckBox Change
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbScheduledShippingDateOn_OnCheckedChanged(object sender, System.EventArgs e)
	{
		tbcbScheduledShippingDateFrom.Enabled
			= tbcbScheduledShippingDateTo.Enabled
			= cbScheduledShippingDateOn.Checked;
	}

	/// <summary>
	/// 出荷予定日アクション利用制御
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblEnabledKbn_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		var isValid = (this.rblShippingDateAction.SelectedValue
			== Constants.FLG_ORDERWORKFLOWSETTING_SHIPPING_DATE_ACTION_ON_CALCULATE_SCHEDULED_SHIPPING_DATE);
		rblScheduledShippingDateAction.Enabled = (isValid == false);
		if (isValid)
		{
			rblScheduledShippingDateAction.SelectedValue =
				Constants.FLG_ORDERWORKFLOWSETTING_SCHEDULED_SHIPPING_DATE_ACTION_OFF;
		}
	}

	/// <summary>
	/// 受注ワークフロー設定の新規作成を行う
	/// </summary>
	/// <param name="insertParam">登録用データ</param>
	protected void InsertOrderWorkflow(Hashtable insertParam)
	{
		ValidateOrderWorkflow(insertParam);

		using (var accessor = new SqlAccessor())
		{
			accessor.OpenConnection();
			accessor.BeginTransaction();

			// 1.ワークフロー枝番取得
			var workFlowNoNew = this.GetNewWorkflowNo(accessor, this.LoginOperatorShopId, ddlWorkflowKbn.SelectedValue);
			insertParam[Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_NO] = workFlowNoNew;

			// 2.ワークフロー登録
			this.InsertOrderWorkflowSetting(accessor, insertParam);

			accessor.CommitTransaction();
		}
	}

	/// <summary>
	/// 受注ワークフロー設定の更新を行う
	/// </summary>
	/// <param name="param">更新用データ</param>
	/// <returns>ワークフロー設定No</returns>
	protected string UpdateOrderWorkflow(Hashtable param)
	{
		param.Add(Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_NO, this.OrderWorkflowSetting.GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_NO).ToString());
		param.Add(Constants.FIELD_ORDERWORKFLOWSETTING_DATE_CREATED, this.OrderWorkflowSetting.GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_DATE_CREATED));
		param.Add(Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_KBN + "_old", this.OrderWorkflowSetting.GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_KBN));
		param.Add(Constants.FIELD_ORDERWORKFLOWSETTING_DISPLAY_ORDER + "_old", this.OrderWorkflowSetting.GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_DISPLAY_ORDER));

		ValidateOrderWorkflow(param);

		var workflowKbnOld = (string)param[Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_KBN + "_old"];
		var workflowKbnNew = (string)param[Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_KBN];
		var noChangeWorkflowType = (((this.RequestWorkflowType == WorkflowSetting.m_KBN_WORKFLOW_TYPE_ORDER)
				&& (this.IsSelectedOrderWorkflow))
			|| ((this.RequestWorkflowType == WorkflowSetting.m_KBN_WORKFLOW_TYPE_FIXEDPURCHASE)
				&& (this.IsSelectedFixedPurchaseWorkflow)));

		if ((workflowKbnNew != workflowKbnOld) || (noChangeWorkflowType == false))
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 1.ワークフロー削除
				if (this.RequestWorkflowType == WorkflowSetting.m_KBN_WORKFLOW_TYPE_ORDER)
				{
					this.DeleteOrderWorkflowSetting(accessor, false);
				}
				else
				{
					this.DeleteFixedPurchaseWorkflowSetting(accessor);
				}

				// 2.ワークフロー枝番取得
				var workFlowNoNew = this.GetNewWorkflowNo(accessor, this.OrderWorkflowSetting.GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_SHOP_ID).ToString(), workflowKbnNew);
				param[Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_NO] = workFlowNoNew;

				// 3.ワークフロー登録
				this.InsertOrderWorkflowSetting(accessor, param);

				accessor.CommitTransaction();
			}
		}
		else
		{
			// 更新
			using (var accessor = new SqlAccessor())
			using (var statement = new SqlStatement("OrderWorkflowSetting", "UpdateOrderWorkflowSetting"))
			{
				statement.ExecStatementWithOC(accessor, param);
			}
		}

		return param[Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_NO].ToString();
	}

	/// <summary>
	/// 定期ワークフロー設定の新規作成を行う
	/// </summary>
	/// <param name="insertParam">登録用データ</param>
	protected void InsertFixedPurchaseWorkflow(Hashtable insertParam)
	{
		ValidateFixedPurchaseWorkflow(insertParam);

		using (var accessor = new SqlAccessor())
		{
			accessor.OpenConnection();
			accessor.BeginTransaction();

			// 1.ワークフロー枝番取得
			var workFlowNoNew = this.GetNewWorkflowNo(accessor, this.LoginOperatorShopId, ddlWorkflowKbn.SelectedValue);
			insertParam[Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_NO] = workFlowNoNew;

			// 2.ワークフロー登録
			this.InsertOrderWorkflowSetting(accessor, insertParam);

			accessor.CommitTransaction();
		}

	}

	/// <summary>
	/// 定期ワークフロー設定の更新を行う
	/// </summary>
	/// <param name="htParam">更新用データ</param>
	/// <returns>ワークフロー設定No</returns>
	protected string UpdateFixedPurchaseWorkflow(Hashtable htParam)
	{
		htParam.Add(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_WORKFLOW_NO, this.OrderWorkflowSetting.GetValue(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_WORKFLOW_NO));
		htParam.Add(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_DATE_CREATED, this.OrderWorkflowSetting.GetValue(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_DATE_CREATED));
		htParam.Add(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_WORKFLOW_KBN + "_old", this.OrderWorkflowSetting.GetValue(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_WORKFLOW_KBN));
		htParam.Add(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_DISPLAY_ORDER + "_old", this.OrderWorkflowSetting.GetValue(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_DISPLAY_ORDER));

		ValidateFixedPurchaseWorkflow(htParam);

		var workflowKbnOld = (string)htParam[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_WORKFLOW_KBN + "_old"];
		var workflowKbnNew = (string)htParam[Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_WORKFLOW_KBN];
		var noChangeWorkflowType = (((this.RequestWorkflowType == WorkflowSetting.m_KBN_WORKFLOW_TYPE_ORDER)
			&& (this.IsSelectedOrderWorkflow))
				|| ((this.RequestWorkflowType == WorkflowSetting.m_KBN_WORKFLOW_TYPE_FIXEDPURCHASE)
			&& (this.IsSelectedFixedPurchaseWorkflow)));

		if ((workflowKbnNew != workflowKbnOld) || (noChangeWorkflowType == false))
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 1.ワークフロー削除
				if (this.RequestWorkflowType == WorkflowSetting.m_KBN_WORKFLOW_TYPE_ORDER)
				{
					this.DeleteOrderWorkflowSetting(accessor, false);
				}
				else
				{
					this.DeleteFixedPurchaseWorkflowSetting(accessor);
				}

				// 2.ワークフロー枝番取得
				var workFlowNoNew = this.GetNewWorkflowNo(accessor, this.OrderWorkflowSetting.GetValue(Constants.FIELD_ORDERWORKFLOWSETTING_SHOP_ID).ToString(), workflowKbnNew);
				htParam[Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_NO] = workFlowNoNew;

				// 3.ワークフロー登録
				this.InsertOrderWorkflowSetting(accessor, htParam);

				accessor.CommitTransaction();
			}
		}
		else
		{
			// 更新
			using (var accessor = new SqlAccessor())
			using (var statement = new SqlStatement("FixedPurchaseWorkflowSetting", "UpdateFixedPurchaseWorkflowSetting"))
			{
				statement.ExecStatementWithOC(accessor, htParam);
			}
		}

		return htParam[Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_NO].ToString();
	}

	/// <summary>
	/// 受注ワークフロー設定のヴァリデーションを行う
	/// </summary>
	/// <param name="workflowSetting">受注ワークフロー設定</param>
	protected void ValidateOrderWorkflow(Hashtable workflowSetting)
	{
		// 入力チェック＆重複チェック
		var pageName = this.OrderWorkflowSetting == null
			? "OrderWorkflowSettingRegist"
			: "OrderWorkflowSettingModify";
		var errorMessages = Validator.Validate(pageName, workflowSetting);
		if (errorMessages != "")
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// シナリオに入っている場合にシナリオに入れるための条件でバリデーションをかける
		ValidateInTheScenario(
			(string)workflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_SHOP_ID],
			(string)workflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_KBN],
			(string)workflowSetting[Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_NO]);

		// 配送希望日の入力チェック
		ValidateDateFromAndDateTo(
			StringUtility.ToValue(workflowSetting[WorkflowSetting.m_SHIPPINGDATE_DATEFROM], "").ToString(),
			StringUtility.ToValue(workflowSetting[WorkflowSetting.m_SHIPPINGDATE_DATETO], "").ToString(),
			ReplaceTag("@@DispText.order.ShippingDate@@"));

		// 出荷予定日の入力チェック
		ValidateDateFromAndDateTo(
			StringUtility.ToValue(workflowSetting[WorkflowSetting.m_SCHEDULED_SHIPPINGDATE_FROM], "").ToString(),
			StringUtility.ToValue(workflowSetting[WorkflowSetting.m_SCHEDULED_SHIPPINGDATE_TO], "").ToString(),
			ReplaceTag("@@DispText.order.ScheduledShippingDate@@"));
	}

	/// <summary>
	/// 定期ワークフロー設定のヴァリデーションを行う
	/// </summary>
	/// <param name="workflowSetting">定期ワークフロー設定</param>
	protected void ValidateFixedPurchaseWorkflow(Hashtable workflowSetting)
	{
		// 入力チェック＆重複チェック
		var pageName = this.OrderWorkflowSetting == null
			? "FixedPurchaseWorkflowSettingRegist"
			: "FixedPurchaseWorkflowSettingModify";
		var errorMessages = Validator.Validate(pageName, workflowSetting);
		if (errorMessages != "")
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 定期再開予定日の入力チェック
		ValidateDateFromAndDateTo(
			StringUtility.ToValue(workflowSetting[WorkflowSetting.m_FIXEDPURCHASE_RESUME_DATE_FROM], "").ToString(),
			StringUtility.ToValue(workflowSetting[WorkflowSetting.m_FIXEDPURCHASE_RESUME_DATE_TO], "").ToString(),
			ReplaceTag("@@DispText.fixed_purchase.FixedPurchaseResumeDate@@"));

		// 次回配送日の入力チェック
		ValidateDateFromAndDateTo(
			StringUtility.ToValue(workflowSetting[WorkflowSetting.m_FIXEDPURCHASE_NEXT_SHIPPING_DATE_FROM], "").ToString(),
			StringUtility.ToValue(workflowSetting[WorkflowSetting.m_FIXEDPURCHASE_NEXT_SHIPPING_DATE_TO], "").ToString(),
			ReplaceTag("@@DispText.fixed_purchase.FixedPurchaseNextShippingDate@@"));

		// 次々回配送日の入力チェック
		ValidateDateFromAndDateTo(
			StringUtility.ToValue(workflowSetting[WorkflowSetting.m_FIXEDPURCHASE_NEXT_NEXT_SHIPPING_DATE_FROM], "").ToString(),
			StringUtility.ToValue(workflowSetting[WorkflowSetting.m_FIXEDPURCHASE_NEXT_NEXT_SHIPPING_DATE_TO], "").ToString(),
			ReplaceTag("@@DispText.fixed_purchase.FixedPurchaseNextNextShippingDate@@"));
	}

	/// <summary>
	/// シナリオに入っている場合のバリデーション
	/// </summary>
	/// <param name="shopId">ショップID</param>
	/// <param name="workflowKbn">ワークフロー区分</param>
	/// <param name="workflowNo">ワークフローNo</param>
	private void ValidateInTheScenario(string shopId, string workflowKbn, string workflowNo)
	{
		if (string.IsNullOrEmpty(workflowNo)) return;
		var scenarios = new OrderWorkflowScenarioSettingService()
			.GetByOrderworkflowSettingPrimaryKey(shopId, workflowKbn, int.Parse(workflowNo));

		if (scenarios.Any() == false) return;

		var errorMessage = "";
		if (rblTargetWorkflowType.SelectedValue ==
			Constants.FLG_ORDERWORKFLOWSETTING_TARGET_WORKFLOW_TYPE_FIXED_PURCHASE)
		{
			errorMessage += string.Format(
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERWORKFLOWSETTING_NOT_CHANGE_WORKFLOW_TYPE_IN_THE_SCENARIO));
		}

		if (rbWorkFlowDetailKbnOrderImportPopUp.Checked)
		{
			errorMessage += string.Format(
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERWORKFLOWSETTING_NOT_UPDATE_THE_ORDER_IMPORT_POP_UP_IN_SCENARIO));
		}

		if (rblShippingDateAction.SelectedValue != Constants.FLG_ORDERWORKFLOWSETTING_SHIPPING_DATE_ACTION_OFF)
		{
			errorMessage += string.Format(
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERWORKFLOWSETTING_NOT_UPDATE_THE_SHIPPING_DATE_ACTION_IN_SCENARIO));
		}

		if (rblScheduledShippingDateAction.SelectedValue != Constants.FLG_ORDERWORKFLOWSETTING_SCHEDULED_SHIPPING_DATE_ACTION_OFF)
		{
			errorMessage += string.Format(
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERWORKFLOWSETTING_NOT_UPDATE_THE_SCHEDULED_SHIPPING_DATE_ACTION_IN_SCENARIO));
		}

		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			errorMessage += string.Format(
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERWORKFLOWSETTING_IN_THE_SCENARIO)
					.Replace("@@ 1 @@", string.Join(", ", scenarios.Select(scenario => scenario.ScenarioName))));
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
	}

	/// <summary>
	/// 日付FROMが日付TOより未来の場合はエラーメッセージを表示する
	/// </summary>
	/// <param name="dateFrom">Input parameters</param>
	/// <param name="dateTo">Input parameters</param>
	/// <param name="columnName">Input parameters</param>
	protected void ValidateDateFromAndDateTo(string dateFrom, string dateTo, string columnName)
	{
		var dateFromNum = 0;
		var dateToNum = 0;
		if ((int.TryParse(dateFrom, out dateFromNum) && int.TryParse(dateTo, out dateToNum)) == false) return;
		if (dateFromNum > dateToNum)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = string.Format(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERWORKFLOWSETTING_SCHEDULED_SHIPPING_DATE_INVALID_RANGE_ERROR), columnName);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
	}

	/// <summary>
	/// ワークフロー対象ラジオボタン変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblTargetWorkflowType_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		rblAdditionalSearchFlg.Items.Clear();
		// 追加検索フラグ
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDERWORKFLOWSETTING_ADDITIONAL_SEARCH_FLG))
		{
			if (this.IsSelectedOrderWorkflow)
			{
				divSearch.Visible = divAction.Visible = true;

				divFixedPurchaseSearch.Visible = divFixedPurchaseAction.Visible = false;
				if (li.Value != Constants.FLG_ORDERWORKFLOWSETTING_ADDITIONAL_SEARCH_FIXEDPURCHASE_FLG_ON)
				{
					rblAdditionalSearchFlg.Items.Add(li);
				}
			}
			else if (this.IsSelectedFixedPurchaseWorkflow)
			{
				divSearch.Visible = divAction.Visible = false;
				divFixedPurchaseSearch.Visible = divFixedPurchaseAction.Visible = true;
				if (li.Value != Constants.FLG_ORDERWORKFLOWSETTING_ADDITIONAL_SEARCH_FLG_ON)
				{
					rblAdditionalSearchFlg.Items.Add(li);
				}
			}
		}
		// ワークフロー区分
		var selectedWorkfloeType = ddlWorkflowKbn.SelectedValue;
		ddlWorkflowKbn.Items.Clear();
		ddlWorkflowKbn.Items.Add(new ListItem("", ""));
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDERWORKFLOWSETTING_WORKFLOW_KBN))
		{
			if (rblTargetWorkflowType.SelectedValue
				== Constants.FLG_ORDERWORKFLOWSETTING_TARGET_WORKFLOW_TYPE_FIXED_PURCHASE
				&& li.Value == Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_KBN_RETURN_EXCHANGE) continue;
			ddlWorkflowKbn.Items.Add(li);
		}
		ddlWorkflowKbn.SelectedValue =
			(rblTargetWorkflowType.SelectedValue
				== Constants.FLG_ORDERWORKFLOWSETTING_TARGET_WORKFLOW_TYPE_FIXED_PURCHASE
				&& selectedWorkfloeType == Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_KBN_RETURN_EXCHANGE)
				? Constants.FLG_ORDERWORKFLOWSETTING_WORKFLOW_KBN_DAY
				: selectedWorkfloeType;

		if (this.IsSelectedOrderWorkflow)
		{
			rbWorkFlowDetailKbnOrderImportPopUp.Enabled = true;
			divReturnOrder.Visible = true;
		}
		else if (this.IsSelectedFixedPurchaseWorkflow)
		{
			rbWorkFlowDetailKbnNormal.Checked = true;
			rbWorkFlowDetailKbnReturn.Checked = false;
			rbWorkFlowDetailKbnOrderImportPopUp.Checked = rbWorkFlowDetailKbnOrderImportPopUp.Enabled = false;
			divReturnOrder.Visible = false;
		}

		rblAdditionalSearchFlg.SelectedValue = Constants.FLG_ORDERWORKFLOWSETTING_ADDITIONAL_SEARCH_FLG_OFF;

		rblDisplayKbn_SelectedIndexChanged(sender, e);
		rbExternalOderInfoAction_OnCheckedChanged(sender, e);
	}

	/// <summary>
	/// 再開予定日指定ありチェックボックス変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbResumeDateOn_OnCheckedChanged(object sender, EventArgs e)
	{
		tbResumeDateFrom.Enabled = tbResumeDateTo.Enabled = cbResumeDateOn.Checked;
	}

	/// <summary>
	/// ワークフロー一覧表示件数リスト選択肢変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlDisplayCount_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		// 表示件数が100件超える場合は、「カセット表示」が非表示になる
		var displayKbnSeletedBefore = rblDisplayKbn.SelectedValue;
		var displayCount = int.Parse(ddlDisplayCount.SelectedValue);
		rblDisplayKbn.Items.Clear();

		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDERWORKFLOWSETTING, Constants.FIELD_ORDERWORKFLOWSETTING_DISPLAY_KBN + "_setting"))
		{
			if ((li.Value == Constants.FLG_ORDERWORKFLOWSETTING_DISPLAY_KBN_CASSETTE) && (displayCount > 100))
			{
				continue;
			}

			rblDisplayKbn.Items.Add(li);
		}

		rblDisplayKbn.SelectedValue = (displayCount > 100)
			? Constants.FLG_ORDERWORKFLOWSETTING_DISPLAY_KBN_LINE
			: displayKbnSeletedBefore;

		// Return work flow max is 100 order
		rbWorkFlowDetailKbnReturn.Enabled = (displayCount <= 100);
		if (rbWorkFlowDetailKbnReturn.Checked && (displayCount > 100))
		{
			rbWorkFlowDetailKbnReturn.Checked = false;
			rblDisplayKbnReturn.Enabled = false;
			rblDisplayKbn.Enabled = true;
			rbWorkFlowDetailKbnNormal.Checked = true;
			rblDisplayKbn_SelectedIndexChanged(sender, e);
		}

		rblDisplayKbn_SelectedIndexChanged(sender, e);
		rbExternalOderInfoAction_OnCheckedChanged(sender, e);
	}

	/// <summary>
	/// 外部受注情報連携の選択値変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rbExternalOderInfoAction_OnCheckedChanged(object sender, EventArgs e)
	{
		if (Constants.CROSS_MALL_OPTION_ENABLED == false) return;

		var isCrossMallUpdateStatusChecked = rbExternalOderInfoActionCrossMallUpdateStatus.Checked;
		lCrossMallOrderUpdateMessages.Visible = isCrossMallUpdateStatusChecked;
		trOrderStatusChange.Visible = isCrossMallUpdateStatusChecked == false;
		if(isCrossMallUpdateStatusChecked)
		{
			rblOrderStatusChange.SelectedValue = SetStatusChange(Constants.FLG_ORDERWORKFLOWSETTING_STATUS_CHANGE_SHIP_COMP);
		}else
		{
			rblOrderStatusChange.SelectedValue = string.Empty;
		}
	}

	/// <summary>
	/// Run Script Set Visibility
	/// </summary>
	protected void RunScriptSetVisibility()
	{
		ScriptManager.RegisterStartupScript(this, this.GetType(), "SetVisibilityForReasonMemo", "SetVisibilityForReasonMemo();", true);

		ScriptManager.RegisterStartupScript(this, this.GetType(), "SetVisibilityForFixedPurchaseCancelReason", "SetVisibilityForFixedPurchaseCancelReason()", true);
	}

	/// <summary>
	/// Update Fixed Purchase Is Alive List
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rUpdateFixedPurchaseIsAliveList_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		// Cassette Cancel Reason
		var ddlCassetteCancelReason = (e.Item.FindControl("ddlCassetteCancelReason") as DropDownList);

		if (ddlCassetteCancelReason.Items.Count > 0)
		{
			ddlCassetteCancelReason.SelectedValue = this.OrderWorkflowSetting.GetValue(Constants.FIELD_FIXEDPURCHASEWORKFLOWSETTING_CANCEL_REASON_ID, WorkflowSetting.WorkflowTypes.FixedPurchase);
		}
	}

	/// <summary>
	/// 注文拡張項目 各コンポーネント設定
	/// </summary>
	/// <param name="id">項目ID</param>
	/// <returns></returns>
	protected ListItemCollection SetOrderExtendItemList(string id)
	{
		var model = DataCacheControllerFacade.GetOrderExtendSettingCacheController().CacheData
			.SettingModels.FirstOrDefault(m => m.SettingId == id);
		var result = new ListItemCollection();
		if (model == null) return result;


		switch (model.InputType)
		{
			case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_TEXT:
				result.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ORDER, "order_extend_flg"));
				break;

			case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_DROPDOWN:
			case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_RADIO:
			case Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_CHECKBOX:
				result.AddRange(OrderExtendCommon.GetListItemForManager(model.InputDefault).ToArray());
				break;

			default:
				break;
		}
		return result;
	}

	/// <summary>
	/// GMO掛け払いを利用している場合は、受注ワークフローの作成は不可とする
	/// </summary>
	private void CheckExchangeOrderGmo()
	{
		// Validate cannot exchange when using gmo payment
		var returnExchangeKbn = CreateSearchStringParts(cblReturnExchangeKbn.Items);
		if (returnExchangeKbn.Contains(Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE))
		{
			var listPayments = CreateSearchStringParts(cblReturnOrderPaymentKbn.Items, true);
			if ((listPayments.Contains(Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO))
				|| (listPayments.Contains(Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)))
			{
				var errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_CANNOT_EXCHANGE_IF_USING_GMO);
				Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}
		}
	}

	/// <summary>
	/// Return Order Action Change
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblReturnOrderAction_SelectedIndexChanged(object sender, EventArgs e)
	{
		pReturnActionAreaReasonMemo.Visible = rblReturnOrderAction.SelectedValue == Constants.FLG_ORDERWORKFLOWSETTING_RETURN_ACTION_ACCEPT_RETURN;
	}

	/// <summary>
	/// Selected changed another shipping flag
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlAnotherShippingFlag_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (Constants.STORE_PICKUP_OPTION_ENABLED
			&& (ddlAnotherShippingFlag.SelectedValue == Constants.FLG_ORDERSHIPPING_SHIPPING_STORE_PICKUP_FLG))
		{
			trPickupStore.Visible = true;
			InitializeRealShopPickup();
			foreach (ListItem li in ddlStorePickup.Items)
			{
				li.Selected = (li.Value == this.RealShopId);
			}
			return;
		}
		trPickupStore.Visible = false;
	}

	/// <summary>
	/// Initialize real shop pickup
	/// </summary>
	private void InitializeRealShopPickup()
	{
		if (ddlStorePickup.Items.Count > 0) return;

		var realShops = new RealShopService().GetAll();
		realShops = realShops
			.Where(realShop => realShop.ValidFlg == Constants.FLG_REALSHOP_VALID_FLG_VALID)
			.ToArray();

		ddlStorePickup.Items.Add(new ListItem(string.Empty, string.Empty));
		foreach (var realShop in realShops)
		{
			ddlStorePickup.Items.Add(new ListItem(realShop.Name, realShop.RealShopId));
		}
	}

	/// <summary>受注ワークフロー設定（編集・コピー新規登録画面のみ）</summary>
	protected WorkflowSetting OrderWorkflowSetting { get; set; }
	/// <summary>受注ワークフロー選択か</summary>
	protected bool IsSelectedOrderWorkflow
	{
		get
		{
			return ((trWorkflowType.Visible == false)
				|| (rblTargetWorkflowType.SelectedValue == Constants.FLG_ORDERWORKFLOWSETTING_TARGET_WORKFLOW_TYPE_ORDER));
		}
	}
	/// <summary>定期ワークフロー選択か</summary>
	protected bool IsSelectedFixedPurchaseWorkflow
	{
		get
		{
			return ((trWorkflowType.Visible == true)
				&& (rblTargetWorkflowType.SelectedValue == Constants.FLG_ORDERWORKFLOWSETTING_TARGET_WORKFLOW_TYPE_FIXED_PURCHASE));
		}
	}
	/// <summary>元ワークフローの対象種別取得</summary>
	protected string WorkflowType
	{
		get
		{
			if (this.OrderWorkflowSetting == null) return WorkflowSetting.m_KBN_WORKFLOW_TYPE_ORDER;
			return (this.OrderWorkflowSetting.WorkflowType == WorkflowSetting.WorkflowTypes.Order)
				? WorkflowSetting.m_KBN_WORKFLOW_TYPE_ORDER
				: WorkflowSetting.m_KBN_WORKFLOW_TYPE_FIXEDPURCHASE;
		}
	}
	/// <summary>Display Kbn Value</summary>
	protected string DisplayKbnValue
	{
		get
		{
			if (rbWorkFlowDetailKbnReturn.Checked) return rblDisplayKbnReturn.SelectedValue;

			return rblDisplayKbn.SelectedValue;
		}
	}
	/// <summary>リアル店舗ID</summary>
	private string RealShopId { get; set; }
}
