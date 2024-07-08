/*
=========================================================================================================
  Module      : 定期購入情報一覧ページ処理(FixedPurchaseList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.DataCacheController;
using w2.App.Common.OrderExtend;
using w2.Domain.FixedPurchase;
using w2.Domain.FixedPurchase.Helper;

public partial class Form_FixedPurchase_FixedPurchaseList : FixedPurchasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// 初期化
			Initialize();

			if (this.IsNotSearchDefault) return;

			// 定期購入情報一覧表示
			DisplayFixedPurchaseList();
		}

		// ユーザーコントロール割り当て
		uMasterDownload.OnCreateSearchInputParams += this.CreateSearchCondition().CreateHashtableParams;
	}

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, System.EventArgs e)
	{
		if (string.IsNullOrEmpty(ucDatePeriod.HfStartDate.Value) == false)
		{
			var datePeriodStart = DateTime.Parse(ucDatePeriod.HfStartDate.Value);
		// 末日補正処理
			if (DateTimeUtility.IsLastDayOfMonth(
				datePeriodStart.Year,
				datePeriodStart.Month,
				datePeriodStart.Day))
		{
				var lastDay = DateTimeUtility.GetLastDayOfMonth(
					datePeriodStart.Year,
					datePeriodStart.Month).ToString();
				ucDatePeriod.SetStartDate(datePeriodStart.AddDays(int.Parse(lastDay)));
		}
		}
		if (string.IsNullOrEmpty(ucDatePeriod.HfEndDate.Value) == false)
		{
			var datePeriodEnd = DateTime.Parse(ucDatePeriod.HfEndDate.Value);
			if (DateTimeUtility.IsLastDayOfMonth(
				datePeriodEnd.Year,
				datePeriodEnd.Month,
				datePeriodEnd.Day))
			{
				var lastDay = DateTimeUtility.GetLastDayOfMonth(
					datePeriodEnd.Year,
					datePeriodEnd.Month).ToString();
				ucDatePeriod.SetEndDate(datePeriodEnd.AddDays(int.Parse(lastDay)));
			}
		}

		var orderExtendName = string.Empty;
		var orderExtendFlg = string.Empty;
		var orderExtendText = string.Empty;
		var orderExtendType = string.Empty;
		if (Constants.ORDER_EXTEND_OPTION_ENABLED)
		{
			orderExtendName = ddlOrderExtendName.SelectedValue;
			orderExtendFlg = ddlOrderExtendFlg.SelectedValue;
			if (string.IsNullOrEmpty(ddlOrderExtendName.SelectedValue) == false)
			{
				var orderExtendSettingModel = DataCacheControllerFacade.GetOrderExtendSettingCacheController().CacheData
					.SettingModels.FirstOrDefault(
						orderExtend => orderExtend.SettingId == ddlOrderExtendName.SelectedValue);
				if (orderExtendSettingModel != null)
				{
					orderExtendText = (orderExtendSettingModel.IsInputTypeText)
						? tbOrderExtendText.Text.Trim()
						: ddlOrderExtendText.SelectedValue;
					orderExtendType = orderExtendSettingModel.InputType;
				}
			}
		}

		var searchValues = new SearchValues(
			tbFixedPurchaseId.Text,
			ddlFixedPurchaseStatusKbn.SelectedValue,
			tbUserId.Text,
			tbUserName.Text,
			tbOrderCountFrom.Text,
			tbOrderCountTo.Text,
			tbShippedCountFrom.Text,
			tbShippedCountTo.Text,
			ddlOrderKbn.SelectedValue,
			ddlOrderPaymentKbn.SelectedValue,
			ddlPaymentStatus.SelectedValue,
			ddlFixedPurchaseKbn.SelectedValue,
			ddlShipping.SelectedValue,
			tbProductId.Text,
			ddlManagementMemoFlg.SelectedValue,
			tbManagementMemo.Text,
			ddlShippingMemoFlg.SelectedValue,
			tbShippingMemo.Text,
			ddlDateType.SelectedValue,
			ddlExtendStatusNo.SelectedValue,
			ddlExtendStatus.SelectedValue,
			ddlSortKbn.SelectedValue,
			ddlShippingMethod.SelectedValue,
			1,
			ddlUpdateDateExtendStatus.SelectedValue,
			orderExtendName,
			orderExtendFlg,
			orderExtendType,
			orderExtendText,
			ucExtendStatusUpdateDatePeriod.HfStartDate.Value,
			ucExtendStatusUpdateDatePeriod.HfEndDate.Value,
			ucExtendStatusUpdateDatePeriod.HfStartTime.Value,
			ucExtendStatusUpdateDatePeriod.HfEndTime.Value,
			ucDatePeriod.HfStartDate.Value,
			ucDatePeriod.HfEndDate.Value,
			ucDatePeriod.HfStartTime.Value,
			ucDatePeriod.HfEndTime.Value,
			tbSubscriptionBoxCourseId.Text.Trim(),
			ddlIsSubscriptionBox.SelectedValue,
			tbSubscriptionBoxOrderCountFrom.Text.Trim(),
			tbSubscriptionBoxOrderCountTo.Text.Trim(),
			Constants.RECEIPT_OPTION_ENABLED ? ddlReceiptFlg.SelectedValue : string.Empty);
		Response.Redirect(searchValues.CreateFixedPurchaseListUrl());
	}

	/// <summary>
	/// 初期化
	/// </summary>
	private void Initialize()
	{
		// 定期購入ステータス
		ddlFixedPurchaseStatusKbn.Items.Add(new ListItem(string.Empty, string.Empty));
		ddlFixedPurchaseStatusKbn.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_FIXEDPURCHASE, Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_STATUS));
		// 注文区分
		ddlOrderKbn.Items.Add(new ListItem(string.Empty, string.Empty));
		foreach (ListItem li in ValueText.GetValueItemArray(Constants.TABLE_FIXEDPURCHASE, Constants.FIELD_FIXEDPURCHASE_ORDER_KBN))
		{
			// モバイルデータの表示OPがOFFの場合は注文区分がモバイル注文を追加しない
			if ((Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED == false)
				&& (li.Value == Constants.FLG_FIXEDPURCHASE_ORDER_KBN_MOBILE)) continue;
			ddlOrderKbn.Items.Add(li);
		}
		// 決済種別
		ddlOrderPaymentKbn.Items.Add(new ListItem(string.Empty, string.Empty));
		foreach (DataRowView payment in GetPaymentValidList(this.LoginOperatorShopId))
		{
			ddlOrderPaymentKbn.Items.Add(
				new ListItem(
					AbbreviateString((string)payment[Constants.FIELD_PAYMENT_PAYMENT_NAME], 12),
					(string)payment[Constants.FIELD_PAYMENT_PAYMENT_ID]));
		}
		// 決済ステータス
		ddlPaymentStatus.Items.Add(new ListItem(string.Empty, string.Empty));
		ddlPaymentStatus.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_FIXEDPURCHASE, Constants.FIELD_FIXEDPURCHASE_PAYMENT_STATUS));
		// 定期購入区分
		ddlFixedPurchaseKbn.Items.Add(new ListItem(string.Empty, string.Empty));
		ddlFixedPurchaseKbn.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_FIXEDPURCHASE, Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_KBN));
		// 配送先
		ddlShipping.Items.Add(new ListItem(string.Empty, string.Empty));
		ddlShipping.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_FIXEDPURCHASE, "shipping_compare_kbn"));
		// 管理メモありなし
		ddlManagementMemoFlg.Items.Add(new ListItem(string.Empty, string.Empty));
		ddlManagementMemoFlg.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_FIXEDPURCHASE, Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_MANAGEMENT_MEMO));
		// 日付
		ddlDateType.Items.Add(new ListItem(string.Empty, string.Empty));
		ddlDateType.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_FIXEDPURCHASE, "date_type"));
		// 並び順
		ddlSortKbn.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_FIXEDPURCHASE, Constants.REQUEST_KEY_SORT_KBN));
		ddlSortKbn.SelectedValue = this.RequestSortKbn;
		// 拡張ステータス
		var extendStatusList = GetOrderExtendStatusSettingList(this.LoginOperatorShopId).Cast<DataRowView>().ToList<DataRowView>();
		trExtendStatus.Visible = (extendStatusList.Count > 0);
		ddlExtendStatusNo.Items.Add(new ListItem("", ""));
		extendStatusList.ForEach(item =>
			ddlExtendStatusNo.Items.Add(new ListItem(
				string.Format("{0}：{1}", item[Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO], item[Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NAME]),
				item[Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO].ToString())));
		ddlExtendStatus.Items.Add(new ListItem("", ""));
		ddlExtendStatus.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_FIXEDPURCHASE, string.Format("{0}2", Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_BASENAME)));
		// 拡張ステータス更新日
		ddlUpdateDateExtendStatus.Items.Add(new ListItem("", ""));
		extendStatusList.ForEach(
			item => ddlUpdateDateExtendStatus.Items.Add(
				new ListItem(
					string.Format(
						"{0}：{1}",
						item[Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO],
						item[Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NAME]),
					item[Constants.FIELD_ORDEREXTENDSTATUSSETTING_EXTEND_STATUS_NO].ToString())));
		// 配送方法
		ddlShippingMethod.Items.Add(new ListItem("", ""));
		ddlShippingMethod.Items.AddRange(
			ValueText.GetValueItemArray(Constants.TABLE_FIXEDPURCHASE, Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_METHOD));
		// 配送メモありなし
		ddlShippingMemoFlg.Items.Add(new ListItem(string.Empty, string.Empty));
		ddlShippingMemoFlg.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_FIXEDPURCHASE, Constants.FIELD_FIXEDPURCHASE_SHIPPING_MEMO));
		// 領収書希望フラグ
		if (Constants.RECEIPT_OPTION_ENABLED)
		{
			ddlReceiptFlg.Items.Add(new ListItem("", ""));
			ddlReceiptFlg.Items.AddRange(
				ValueText.GetValueItemArray(Constants.TABLE_FIXEDPURCHASE, Constants.FIELD_FIXEDPURCHASE_RECEIPT_FLG));
		}

		if (Constants.ORDER_EXTEND_OPTION_ENABLED)
		{
			//注文拡張項目の設定
			ddlOrderExtendName.Items.Add(new ListItem("", ""));
			ddlOrderExtendName.Items.AddRange(DataCacheControllerFacade.GetOrderExtendSettingCacheController().CacheData.SettingModels.Select(ues => new ListItem(ues.SettingName, ues.SettingId)).ToArray());
			ddlOrderExtendFlg.Items.Add(new ListItem("", ""));
			ddlOrderExtendFlg.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ORDER, "order_extend_flg"));
		}
		// 頒布会か
		if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED)
		{
			ddlIsSubscriptionBox.Items.Add(new ListItem(string.Empty, string.Empty));
			ddlIsSubscriptionBox.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_FIXEDPURCHASE, "is_subscription_box"));
		}
	}

	/// <summary>
	/// 定期購入情報一覧表示
	/// </summary>
	private void DisplayFixedPurchaseList()
	{
		// 検索フォームにパラメータをセット
		tbFixedPurchaseId.Text = this.RequestFixedPurchaseId;
		ddlFixedPurchaseStatusKbn.SelectedValue = this.RequestFixedPurchaseStatus;
		tbUserId.Text = this.RequestUserId;
		tbUserName.Text = this.RequestUserName;
		tbOrderCountFrom.Text = this.RequestOrderCountFrom;
		tbOrderCountTo.Text = this.RequestOrderCountTo;
		tbShippedCountFrom.Text = this.RequestShippedCountFrom;
		tbShippedCountTo.Text = this.RequestShippedCountTo;
		ddlOrderKbn.SelectedValue = this.RequestOrderKbn;
		ddlOrderPaymentKbn.SelectedValue = this.RequestOrderPaymentKbn;
		ddlPaymentStatus.SelectedValue = this.RequestPaymentStatus;
		ddlFixedPurchaseKbn.SelectedValue = this.RequestFixedPurchaseKbn;
		ddlShipping.SelectedValue = this.RequestShipping;
		tbProductId.Text = this.RequestProductId;
		ddlManagementMemoFlg.SelectedValue = this.RequestManagementMemoFlg;
		tbManagementMemo.Text = this.RequestManagementMemo;
		ddlShippingMemoFlg.SelectedValue = this.RequestShippingMemoFlg;
		tbShippingMemo.Text = this.RequestShippingMemo;
		ddlDateType.SelectedValue = this.RequestDateType;
		ddlExtendStatusNo.SelectedValue = this.RequestExtendStatusNo;
		ddlExtendStatus.SelectedValue = this.RequestExtendStatus;
		ddlUpdateDateExtendStatus.SelectedValue = this.RequestUpdateDateExtendStatus;
		ddlSortKbn.SelectedValue = this.RequestSortKbn;
		ddlShippingMethod.SelectedValue = this.RequestShippingMethod;

		var extendStatusUpdateDateFrom = string.Format("{0}{1}",
			this.RequestUpdateExtendStatusDateFrom.Replace("/", string.Empty),
			this.RequestUpdateExtendStatusTimeFrom.Replace(":", string.Empty));
		var extendStatusUpdateDateTo = string.Format("{0}{1}",
			this.RequestUpdateExtendStatusDateTo.Replace("/", string.Empty),
			this.RequestUpdateExtendStatusTimeTo.Replace(":", string.Empty));
		ucExtendStatusUpdateDatePeriod.SetPeriodDate(extendStatusUpdateDateFrom, extendStatusUpdateDateTo);

		var dateFrom = string.Format("{0}{1}",
			this.RequestDateFrom.Replace("/", string.Empty),
			this.RequestTimeFrom.Replace(":", string.Empty));
		var dateTo = string.Format("{0}{1}",
			this.RequestDateTo.Replace("/", string.Empty),
			this.RequestTimeTo.Replace(":", string.Empty));
		ucDatePeriod.SetPeriodDate(dateFrom, dateTo);

		if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED)
		{
			tbSubscriptionBoxCourseId.Text = this.RequestSubscriptionBoxCourseId;
			ddlIsSubscriptionBox.SelectedValue = this.RequestIsSubscriptionBox;
			tbSubscriptionBoxOrderCountFrom.Text = this.RequestSubscriptionBoxOrderCountFrom;
			tbSubscriptionBoxOrderCountTo.Text = this.RequestSubscriptionBoxOrderCountTo;
		}

		// 領収書希望
		if (Constants.RECEIPT_OPTION_ENABLED)
		{
			ddlReceiptFlg.SelectedValue = this.RequestReceiptFlg;
		}

		var replace = new KeyValuePair<string, string>[]{};
		if (Constants.ORDER_EXTEND_OPTION_ENABLED)
		{
			ddlOrderExtendName.SelectedValue = this.OrderExtendName;
			InitializeUserExtendText(
				this.OrderExtendFlg,
				this.OrderExtendLikeEscaped);
			replace = OrderExtendCommon.SetReplaceOrderExtendFieldName(replace, Constants.TABLE_FIXEDPURCHASE, this.OrderExtendName);
		}
		// 定期購入情報一覧取得
		var searchCondition = CreateSearchCondition();
		var service = new FixedPurchaseService();
		var totalCount = service.GetCountOfSearchFixedPurchase(searchCondition, replace);
		var result = service.SearchFixedPurchase(searchCondition, replace);
		rList.DataSource = result;
		rList.DataBind();
		rTableFixColumn.DataSource = result;
		rTableFixColumn.DataBind();

		// 件数取得、エラー表示制御
		if (totalCount != 0)
		{
			// 検索情報取得
			var searchSqlInfo = searchCondition.CreateHashtableParams();
			var sqlParams = new Hashtable
			{
				{ Constants.FIELD_TARGETLIST_TARGET_TYPE, Constants.FLG_TARGETLIST_TARGET_TYPE_FIXEDPURCHASE_LIST },
				{ Constants.TABLE_USER, searchSqlInfo }
			};
			Session[Constants.SESSION_KEY_PARAM + "EC"] = sqlParams;
			trListError.Visible = false;
			btnImportTargetList.Enabled = true;
		}
		else
		{
			trListError.Visible = true;
			tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
			btnImportTargetList.Enabled = false;
		}

		// 定期購入一覧検索情報を格納
		this.SearchInfo = new SearchValues(
			this.RequestFixedPurchaseId,
			this.RequestFixedPurchaseStatus,
			this.RequestUserId,
			this.RequestUserName,
			this.RequestOrderCountFrom,
			this.RequestOrderCountTo,
			this.RequestShippedCountFrom,
			this.RequestShippedCountTo,
			this.RequestOrderKbn,
			this.RequestOrderPaymentKbn,
			this.RequestPaymentStatus,
			this.RequestFixedPurchaseKbn,
			this.RequestShipping,
			this.RequestProductId,
			this.RequestManagementMemoFlg,
			this.RequestManagementMemo,
			this.RequestShippingMemoFlg,
			this.RequestShippingMemo,
			this.RequestDateType,
			this.RequestExtendStatusNo,
			this.RequestExtendStatus,
			this.RequestSortKbn,
			this.RequestShippingMethod,
			this.RequestPageNum,
			this.RequestUpdateDateExtendStatus,
			this.OrderExtendName,
			this.OrderExtendFlg,
			this.OrderExtendType,
			this.OrderExtendLikeEscaped,
			this.RequestUpdateExtendStatusDateFrom,
			this.RequestUpdateExtendStatusDateTo,
			this.RequestUpdateExtendStatusTimeFrom,
			this.RequestUpdateExtendStatusTimeTo,
			this.RequestDateFrom,
			this.RequestDateTo,
			this.RequestTimeFrom,
			this.RequestTimeTo,
			this.RequestSubscriptionBoxCourseId,
			this.RequestIsSubscriptionBox,
			this.RequestSubscriptionBoxOrderCountFrom,
			this.RequestSubscriptionBoxOrderCountTo,
			this.RequestReceiptFlg);

		// ページャ作成
		string nextUrl = this.SearchInfo.CreateFixedPurchaseListUrl(false);
		lbPager1.Text = WebPager.CreateDefaultListPager(totalCount, this.RequestPageNum, nextUrl);
	}

	/// <summary>
	/// 検索条件作成
	/// </summary>
	/// <returns>検索条件</returns>
	private FixedPurchaseListSearchCondition CreateSearchCondition()
	{
		var result = new FixedPurchaseListSearchCondition
		{
			FixedPurchaseId = this.RequestFixedPurchaseId,
			FixedPurchaseStatus = this.RequestFixedPurchaseStatus,
			UserId = this.RequestUserId,
			Name = this.RequestUserName,
			OrderKbn = this.RequestOrderKbn,
			OrderPaymentKbn = this.RequestOrderPaymentKbn,
			PaymentStatus = this.RequestPaymentStatus,
			FixedPurchaseKbn = this.RequestFixedPurchaseKbn,
			ShippingCompareKbn = this.RequestShipping,
			VariationId = this.RequestProductId,
			ManagementMemoFlg = this.RequestManagementMemoFlg,
			ManagementMemo = this.RequestManagementMemo,
			ShippingMemoFlg = this.RequestShippingMemoFlg,
			ShippingMemo = this.RequestShippingMemo,
			DateType = this.RequestDateType,
			SortKbn = this.RequestSortKbn,
			ShippingMethod = this.RequestShippingMethod,
			BeginRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (this.RequestPageNum - 1) + 1,
			EndRowNumber = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * this.RequestPageNum,
			ExtendStatusDateNo = this.RequestUpdateDateExtendStatus,
			ReceiptFlg = Constants.RECEIPT_OPTION_ENABLED ? this.RequestReceiptFlg : string.Empty,
		};
		var orderCount = 0;
		if (int.TryParse(this.RequestOrderCountFrom, out orderCount))
		{
			result.OrderCountFrom = int.Parse(this.RequestOrderCountFrom);
		}
		if (int.TryParse(this.RequestOrderCountTo, out orderCount))
		{
			result.OrderCountTo = int.Parse(this.RequestOrderCountTo);
		}
		var shippedCount = 0;
		if (int.TryParse(this.RequestShippedCountFrom, out shippedCount))
		{
			result.ShippedCountFrom = int.Parse(this.RequestShippedCountFrom);
		}
		if (int.TryParse(this.RequestShippedCountTo, out shippedCount))
		{
			result.ShippedCountTo = int.Parse(this.RequestShippedCountTo);
		}
		if (string.IsNullOrEmpty(result.DateType) == false)
		{
			if (Validator.IsDate(this.StartDateTimeString))
			{
				result.DateFrom = DateTime.Parse(this.StartDateTimeString);
			}

			if (Validator.IsDate(this.EndDateTimeString))
			{
				result.DateTo = DateTime.Parse(this.EndDateTimeString).AddSeconds(1);
			}
		}
		if (this.RequestExtendStatus != "")
		{
			if (this.RequestExtendStatusNo == "1") result.ExtendStatus1 = this.RequestExtendStatus;
			if (this.RequestExtendStatusNo == "2") result.ExtendStatus2 = this.RequestExtendStatus;
			if (this.RequestExtendStatusNo == "3") result.ExtendStatus3 = this.RequestExtendStatus;
			if (this.RequestExtendStatusNo == "4") result.ExtendStatus4 = this.RequestExtendStatus;
			if (this.RequestExtendStatusNo == "5") result.ExtendStatus5 = this.RequestExtendStatus;
			if (this.RequestExtendStatusNo == "6") result.ExtendStatus6 = this.RequestExtendStatus;
			if (this.RequestExtendStatusNo == "7") result.ExtendStatus7 = this.RequestExtendStatus;
			if (this.RequestExtendStatusNo == "8") result.ExtendStatus8 = this.RequestExtendStatus;
			if (this.RequestExtendStatusNo == "9") result.ExtendStatus9 = this.RequestExtendStatus;
			if (this.RequestExtendStatusNo == "10") result.ExtendStatus10 = this.RequestExtendStatus;
			if (this.RequestExtendStatusNo == "11") result.ExtendStatus11 = this.RequestExtendStatus;
			if (this.RequestExtendStatusNo == "12") result.ExtendStatus12 = this.RequestExtendStatus;
			if (this.RequestExtendStatusNo == "13") result.ExtendStatus13 = this.RequestExtendStatus;
			if (this.RequestExtendStatusNo == "14") result.ExtendStatus14 = this.RequestExtendStatus;
			if (this.RequestExtendStatusNo == "15") result.ExtendStatus15 = this.RequestExtendStatus;
			if (this.RequestExtendStatusNo == "16") result.ExtendStatus16 = this.RequestExtendStatus;
			if (this.RequestExtendStatusNo == "17") result.ExtendStatus17 = this.RequestExtendStatus;
			if (this.RequestExtendStatusNo == "18") result.ExtendStatus18 = this.RequestExtendStatus;
			if (this.RequestExtendStatusNo == "19") result.ExtendStatus19 = this.RequestExtendStatus;
			if (this.RequestExtendStatusNo == "20") result.ExtendStatus20 = this.RequestExtendStatus;
			if (this.RequestExtendStatusNo == "21") result.ExtendStatus21 = this.RequestExtendStatus;
			if (this.RequestExtendStatusNo == "22") result.ExtendStatus22 = this.RequestExtendStatus;
			if (this.RequestExtendStatusNo == "23") result.ExtendStatus23 = this.RequestExtendStatus;
			if (this.RequestExtendStatusNo == "24") result.ExtendStatus24 = this.RequestExtendStatus;
			if (this.RequestExtendStatusNo == "25") result.ExtendStatus25 = this.RequestExtendStatus;
			if (this.RequestExtendStatusNo == "26") result.ExtendStatus26 = this.RequestExtendStatus;
			if (this.RequestExtendStatusNo == "27") result.ExtendStatus27 = this.RequestExtendStatus;
			if (this.RequestExtendStatusNo == "28") result.ExtendStatus28 = this.RequestExtendStatus;
			if (this.RequestExtendStatusNo == "29") result.ExtendStatus29 = this.RequestExtendStatus;
			if (this.RequestExtendStatusNo == "30") result.ExtendStatus30 = this.RequestExtendStatus;
			if (this.RequestExtendStatusNo == "31") result.ExtendStatus31 = this.RequestExtendStatus;
			if (this.RequestExtendStatusNo == "32") result.ExtendStatus32 = this.RequestExtendStatus;
			if (this.RequestExtendStatusNo == "33") result.ExtendStatus33 = this.RequestExtendStatus;
			if (this.RequestExtendStatusNo == "34") result.ExtendStatus34 = this.RequestExtendStatus;
			if (this.RequestExtendStatusNo == "35") result.ExtendStatus35 = this.RequestExtendStatus;
			if (this.RequestExtendStatusNo == "36") result.ExtendStatus36 = this.RequestExtendStatus;
			if (this.RequestExtendStatusNo == "37") result.ExtendStatus37 = this.RequestExtendStatus;
			if (this.RequestExtendStatusNo == "38") result.ExtendStatus38 = this.RequestExtendStatus;
			if (this.RequestExtendStatusNo == "39") result.ExtendStatus39 = this.RequestExtendStatus;
			if (this.RequestExtendStatusNo == "40") result.ExtendStatus40 = this.RequestExtendStatus;
		}

		// 拡張ステータス更新日
		for (int i = 1; i <= Constants.CONST_ORDER_EXTEND_STATUS_DBFIELDS_MAX; i++)
		{
			if (result.ExtendStatusDateNo == i.ToString())
			{
				if (Validator.IsDate(this.ExtendStartDateTimeString))
				{
					result.ExtendStatusDateFrom = DateTime.Parse(this.ExtendStartDateTimeString);
				}

				if (Validator.IsDate(this.ExtendEndDateTimeString))
				{
					result.ExtendStatusDateTo = DateTime.Parse(this.ExtendEndDateTimeString).AddSeconds(1);
				}
			}
		}

		result.OrderExtendName = string.Empty;
		result.OrderExtendFlg = string.Empty;
		result.OrderExtendType = string.Empty;
		result.OrderExtendLikeEscaped = string.Empty;
		if (Constants.ORDER_EXTEND_OPTION_ENABLED)
		{
			result.OrderExtendName = this.OrderExtendName;
			result.OrderExtendFlg = this.OrderExtendFlg;
			if (string.IsNullOrEmpty(this.OrderExtendName))
			{
				result.OrderExtendLikeEscaped = string.Empty;
			}
			else
			{
				var orderExtendSetting =
					DataCacheControllerFacade.GetOrderExtendSettingCacheController().CacheData.SettingModels.FirstOrDefault(
						orderExtend => orderExtend.SettingId == this.OrderExtendName);
				if (orderExtendSetting != null)
				{
					result.OrderExtendLikeEscaped = this.OrderExtendLikeEscaped;
					result.OrderExtendType = this.OrderExtendType;
				}
			}
		}

		if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED)
		{
			result.SubscriptionBoxCourseId = this.RequestSubscriptionBoxCourseId;
			result.IsSubscriptionBox = this.RequestIsSubscriptionBox;

			var subscriptionBoxOrderCount = 0;
			if (int.TryParse(this.RequestSubscriptionBoxOrderCountFrom, out subscriptionBoxOrderCount))
			{
				result.SubscriptionBoxOrderCountFrom = int.Parse(this.RequestSubscriptionBoxOrderCountFrom);
			}
			if (int.TryParse(this.RequestSubscriptionBoxOrderCountTo, out subscriptionBoxOrderCount))
			{
				result.SubscriptionBoxOrderCountTo = int.Parse(this.RequestSubscriptionBoxOrderCountTo);
			}
		}

		return result;
	}

	/// <summary>
	/// Calculate search rowspan
	/// </summary>
	/// <returns>Number of rowspan</returns>
	protected int CalculateSearchRowSpan()
	{
		var result = 11
			+ (Constants.RECEIPT_OPTION_ENABLED ? 1 : 0)
			+ (Constants.ORDER_EXTEND_OPTION_ENABLED ? 1 : 0)
			+ (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED ? 1 : 0);
		return result;
	}

	/// <summary>
	/// ユーザー拡張項目選択変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlOrderExtend_SelectedIndexChanged(object sender, EventArgs e)
	{
		InitializeUserExtendText("", "");
	}

	/// <summary>
	/// ユーザー拡張項目テキストの設定
	/// </summary>
	/// <param name="defaultFlg">ありなしフラグ</param>
	/// <param name="defaultText">検索値</param>
	private void InitializeUserExtendText(string defaultFlg, string defaultText)
	{
		ddlOrderExtendText.Items.Clear();
		tbOrderExtendText.Text = "";
		ddlOrderExtendFlg.Visible = false;
		tbOrderExtendText.Visible = false;
		ddlOrderExtendText.Visible = false;
		if (string.IsNullOrEmpty(ddlOrderExtendName.SelectedValue)) return;

		ddlOrderExtendFlg.Visible = true;
		ddlOrderExtendFlg.SelectedValue = defaultFlg;
		var orderExtendSetting =
			DataCacheControllerFacade.GetOrderExtendSettingCacheController().CacheData.SettingModels.FirstOrDefault(
				orderExtend => orderExtend.SettingId == ddlOrderExtendName.SelectedValue);

		if (orderExtendSetting == null) return;

		// 拡張項目の設定方法がテキストボックスの場合は検索用テキストボックスを表示する。
		// そうではない場合は検索用ドロップダウンリストを表示する
		if (orderExtendSetting.InputType == Constants.FLG_ORDEREXTENDSETTING_INPUT_TYPE_TEXT)
		{
			tbOrderExtendText.Visible = true;
			tbOrderExtendText.Text = defaultText;
		}
		else
		{
			ddlOrderExtendText.Visible = true;
			ddlOrderExtendText.Items.Add(new ListItem("", ""));
			AddItemToDropDownList(
				ddlOrderExtendText,
				OrderExtendCommon.GetListItemForManager(orderExtendSetting.InputDefault),
				defaultText);
		}
	}

	#region プロパティ
	/// <summary>リクエスト：定期購入ステータス</summary>
	private string RequestFixedPurchaseStatus
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_FIXED_PURCHASE_STATUS]).Trim(); }
	}
	/// <summary>リクエスト：ユーザID</summary>
	private string RequestUserId
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_USER_ID]).Trim(); }
	}
	/// <summary>リクエスト：注文者名</summary>
	private string RequestUserName
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_USER_NAME]).Trim(); }
	}
	/// <summary>リクエスト：購入回数(注文基準)（開始）</summary>
	private string RequestOrderCountFrom
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_ORDER_COUNT_FROM]).Trim(); }
	}
	/// <summary>リクエスト：購入回数(注文基準)（終了）</summary>
	private string RequestOrderCountTo
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_ORDER_COUNT_TO]).Trim(); }
	}
	/// <summary>リクエスト：購入回数(出荷基準)（開始）</summary>
	private string RequestShippedCountFrom
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_SHIPPED_COUNT_FROM]).Trim(); }
	}
	/// <summary>リクエスト：購入回数(出荷基準)（終了）</summary>
	private string RequestShippedCountTo
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_SHIPPED_COUNT_TO]).Trim(); }
	}
	/// <summary>リクエスト：注文区分</summary>
	private string RequestOrderKbn
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_ORDER_KBN]).Trim(); }
	}
	/// <summary>リクエスト：決済種別</summary>
	private string RequestOrderPaymentKbn
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_ORDER_PAYMENT_KBN]).Trim(); }
	}
	/// <summary>リクエスト：決済ステータス</summary>
	private string RequestPaymentStatus
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_PAYMENT_STATUS]).Trim(); }
	}
	/// <summary>リクエスト：定期購入区分</summary>
	private string RequestFixedPurchaseKbn
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_FIXED_PURCHASE_KBN]).Trim(); }
	}
	/// <summary>リクエスト：配送先</summary>
	private string RequestShipping
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_SHIPPING]).Trim(); }
	}
	/// <summary>リクエスト：商品ID</summary>
	private string RequestProductId
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_PRODUCT_SEARCH_VALUE]).Trim(); }
	}
	/// <summary>リクエスト：管理メモフラグ（ブランク or あり or なし）</summary>
	private string RequestManagementMemoFlg
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_MANAGEMENT_MEMO_FLG]).Trim(); }
	}
	/// <summary>リクエスト：管理メモ</summary>
	private string RequestManagementMemo
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_MANAGEMENT_MEMO]).Trim(); }
	}
	/// <summary>リクエスト：配送メモフラグ（ブランク or あり or なし）</summary>
	private string RequestShippingMemoFlg
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_SHIPPING_MEMO_FLG]).Trim(); }
	}
	/// <summary>リクエスト：配送メモ</summary>
	private string RequestShippingMemo
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_SHIPPING_MEMO]).Trim(); }
	}
	/// <summary>リクエスト：日付</summary>
	private string RequestDateType
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_DATE_TYPE]).Trim(); }
	}
	/// <summary>リクエスト：拡張ステータスNo</summary>
	private string RequestExtendStatusNo
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_EXTEND_STATUS_NO]).Trim(); }
	}
	/// <summary>リクエスト：拡張ステータス</summary>
	private string RequestExtendStatus
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_EXTEND_STATUS]).Trim(); }
	}
	/// <summary>リクエスト：拡張ステータス更新日</summary>
	private string RequestUpdateDateExtendStatus
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_EXTEND_STATUS_NO_UPDATE_DATE]).Trim(); }
	}
	/// <summary>リクエスト：並び順</summary>
	private string RequestSortKbn
	{
		get
		{
			var sortKbn = Constants.KBN_SORT_FIXEDPURCHASE_DATE_DEFAULT;
			switch (Request[Constants.REQUEST_KEY_SORT_KBN])
			{
				case Constants.KBN_SORT_FIXEDPURCHASE_DATE_CREATED_ASC:
				case Constants.KBN_SORT_FIXEDPURCHASE_DATE_CREATED_DESC:
				case Constants.KBN_SORT_FIXEDPURCHASE_DATE_CHANGED_ASC:
				case Constants.KBN_SORT_FIXEDPURCHASE_DATE_CHANGED_DESC:
				case Constants.KBN_SORT_FIXEDPURCHASE_ID_ASC:
				case Constants.KBN_SORT_FIXEDPURCHASE_ID_DESC:
				case Constants.KBN_SORT_FIXEDPURCHASE_NEXT_SHIPPING_DATE_ASC:
				case Constants.KBN_SORT_FIXEDPURCHASE_NEXT_SHIPPING_DATE_DESC:
				case Constants.KBN_SORT_FIXEDPURCHASE_NEXT_NEXT_SHIPPING_DATE_ASC:
				case Constants.KBN_SORT_FIXEDPURCHASE_NEXT_NEXT_SHIPPING_DATE_DESC:
				case Constants.KBN_SORT_FIXEDPURCHASE_FIXED_PURCHASE_DATE_BGN_ASC:
				case Constants.KBN_SORT_FIXEDPURCHASE_FIXED_PURCHASE_DATE_BGN_DESC:
				case Constants.KBN_SORT_FIXEDPURCHASE_LAST_ORDER_DATE_ASC:
				case Constants.KBN_SORT_FIXEDPURCHASE_LAST_ORDER_DATE_DESC:
					sortKbn = Request[Constants.REQUEST_KEY_SORT_KBN];
					break;
			}
			return sortKbn;
		}
	}
	/// <summary>リクエスト：ページ番号</summary>
	private int RequestPageNum
	{
		get
		{
			int pageNum;
			return int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out pageNum) ? pageNum : 1;
		}
	}
	/// <summary>リクエスト：配送方法</summary>
	private string RequestShippingMethod
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_SHIPPING_METHOD]).Trim(); }
	}
	/// <summary>リクエスト：領収書希望</summary>
	private string RequestReceiptFlg
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_RECEIPT_FLG]).Trim(); }
	}
	/// <summary>リクエスト：注文拡張項目</summary>
	private string OrderExtendName
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_ORDER_EXTEND_NAME]).Trim(); }
	}
	/// <summary>リクエスト：注文拡張項目</summary>
	private string OrderExtendFlg
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_ORDER_EXTEND_FLG]).Trim(); }
	}
	/// <summary>リクエスト：注文拡張項目</summary>
	private string OrderExtendType
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_ORDER_EXTEND_TYPE]).Trim(); }
	}
	/// <summary>リクエスト：注文拡張項目</summary>
	private string OrderExtendLikeEscaped
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_ORDER_EXTEND_TEXT]).Trim(); }
	}
	/// <summary>Request update extend status date from</summary>
	private string RequestUpdateExtendStatusDateFrom
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_EXTEND_STATUS_UPDATE_DATE_FROM]).Trim(); }
	}
	/// <summary>Request update extend status date to</summary>
	private string RequestUpdateExtendStatusDateTo
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_EXTEND_STATUS_UPDATE_DATE_TO]).Trim(); }
	}
	/// <summary>Request update extend status time from</summary>
	private string RequestUpdateExtendStatusTimeFrom
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_EXTEND_STATUS_UPDATE_TIME_FROM]).Trim(); }
	}
	/// <summary>Request update extend status time to</summary>
	private string RequestUpdateExtendStatusTimeTo
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_EXTEND_STATUS_UPDATE_TIME_TO]).Trim(); }
	}
	/// <summary>拡張ステータス更新日「yyyy/MM/dd HH:mm:ss」の日付時刻文字列 (Start)</summary>
	private string ExtendStartDateTimeString
	{
		get
		{
			var date = Validator.IsDate(this.RequestUpdateExtendStatusDateFrom) ? this.RequestUpdateExtendStatusDateFrom : string.Empty;
			var dateTime = String.Format("{0} {1}", date, this.RequestUpdateExtendStatusTimeFrom);
			return dateTime.Trim();
		}
	}
	/// <summary>拡張ステータス更新日「yyyy/MM/dd HH:mm:ss」の日付時刻文字列 (End)</summary>
	private string ExtendEndDateTimeString
	{
		get
		{
			var date = Validator.IsDate(this.RequestUpdateExtendStatusDateTo) ? this.RequestUpdateExtendStatusDateTo : string.Empty;
			var dateTime = String.Format("{0} {1}", date, this.RequestUpdateExtendStatusTimeTo);
			return dateTime.Trim();
		}
	}
	/// <summary>Request date from</summary>
	private string RequestDateFrom
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_DATE_FROM]).Trim(); }
	}
	/// <summary>Request date to</summary>
	private string RequestDateTo
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_DATE_TO]).Trim(); }
	}
	/// <summary>Request time from</summary>
	private string RequestTimeFrom
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_TIME_FROM]).Trim(); }
	}
	/// <summary>Request date to</summary>
	private string RequestTimeTo
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_TIME_TO]).Trim(); }
	}
	/// <summary>日付「yyyy/MM/dd HH:mm:ss」の日付時刻文字列 (Start)</summary>
	private string StartDateTimeString
	{
		get
		{
			var date = Validator.IsDate(this.RequestDateFrom) ? this.RequestDateFrom : string.Empty;
			var dateTime = String.Format("{0} {1}", date, this.RequestTimeFrom);
			return dateTime.Trim();
		}
	}
	/// <summary>日付「yyyy/MM/dd HH:mm:ss」の日付時刻文字列 (End)</summary>
	private string EndDateTimeString
	{
		get
		{
			var date = Validator.IsDate(this.RequestDateTo) ? this.RequestDateTo : string.Empty;
			var dateTime = String.Format("{0} {1}", date, this.RequestTimeTo);
			return dateTime.Trim();
		}
	}
	/// <summary>リクエスト：頒布会コースID</summary>
	private string RequestSubscriptionBoxCourseId
	{
		get { return (Request[Constants.REQUEST_KEY_SUBSCRIPTION_BOX_COURSE_ID] ?? "").Trim(); }
	}
	/// <summary>リクエスト：頒布会コースか</summary>
	private string RequestIsSubscriptionBox
	{
		get { return (Request[Constants.REQUEST_KEY_SUBSCRIPTION_BOX] ?? "").Trim(); }
	}
	/// <summary>リクエスト：頒布会購入回数FROM</summary>
	private string RequestSubscriptionBoxOrderCountFrom
	{
		get { return (Request[Constants.REQUEST_KEY_SUBSCRIPTION_BOX_ORDER_COUNT_FROM] ?? "").Trim(); }
	}
	/// <summary>リクエスト：頒布会購入回数TO</summary>
	private string RequestSubscriptionBoxOrderCountTo
	{
		get { return (Request[Constants.REQUEST_KEY_SUBSCRIPTION_BOX_ORDER_COUNT_TO] ?? "").Trim(); }
	}
	#endregion
}