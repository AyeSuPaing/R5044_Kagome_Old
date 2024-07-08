/*
=========================================================================================================
  Module      : 定期購入情報確認ページ処理(FixedPurchaseConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common;
using w2.App.Common.DataCacheController;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Global.Translation;
using w2.App.Common.Mail;
using w2.App.Common.Option;
using w2.App.Common.Option.CrossPoint;
using w2.App.Common.Order;
using w2.App.Common.Order.FixedPurchase;
using w2.App.Common.Order.Payment.PayPal;
using w2.App.Common.Order.Payment.PayTg;
using w2.App.Common.Order.Payment.Veritrans;
using w2.App.Common.Order.Register;
using w2.App.Common.OrderExtend;
using w2.App.Common.User;
using w2.App.Common.User.UpdateAddressOfUserandFixedPurchase;
using w2.App.Common.Web.WebCustomControl;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Web;
using w2.Domain;
using w2.Domain.Coupon;
using w2.Domain.Coupon.Helper;
using w2.Domain.DeliveryCompany;
using w2.Domain.FixedPurchase;
using w2.Domain.FixedPurchase.Helper;
using w2.Domain.ShopShipping;
using w2.Domain.SubscriptionBox;
using w2.Domain.TwFixedPurchaseInvoice;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

public partial class Form_FixedPurchase_FixedPurchaseConfirm : FixedPurchasePage
{
	#region ラップ済みコントロール宣言
	public WrappedTextBox WtbSuspendReason { get { return GetWrappedControl<WrappedTextBox>("tbSuspendReason"); } }
	public WrappedHtmlGenericControl WdvFixedPurchaseSuspend { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvFixedPurchaseSuspend"); } }
	public WrappedHtmlGenericControl WtrFixedPurchaseSuspendButtonDisplay { get { return GetWrappedControl<WrappedHtmlGenericControl>("trFixedPurchaseSuspendButtonDisplay"); } }
	public WrappedHtmlGenericControl WtrFixedPurchaseSuspendReasonButtonDisplay { get { return GetWrappedControl<WrappedHtmlGenericControl>("trFixedPurchaseSuspendReasonButtonDisplay"); } }
	public WrappedHtmlGenericControl WtrFixedPurchaseSuspendErrorMessages { get { return GetWrappedControl<WrappedHtmlGenericControl>("trFixedPurchaseSuspendErrorMessages"); } }
	public WrappedLabel WlbFixedPurchaseSuspendErrorMessages { get { return GetWrappedControl<WrappedLabel>("lbFixedPurchaseSuspendErrorMessages"); } }
	public WrappedRepeater WrUserListForAddressUpdate { get { return GetWrappedControl<WrappedRepeater>("rUserListForAddressUpdate"); } }
	public WrappedRepeater WrFixedPurchaseListForAddressUpdate { get { return GetWrappedControl<WrappedRepeater>("rFixedPurchaseListForAddressUpdate"); } }
	public WrappedHtmlGenericControl WdvUserForAddressUpdate { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvUserForAddressUpdate"); } }
	public WrappedHtmlGenericControl WdvFixedPurchaseForAddressUpdate { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvFixedPurchaseForAddressUpdate"); } }
	public WrappedHtmlGenericControl WdvOrderConvenienceStore { get { return GetWrappedControl<WrappedHtmlGenericControl>("tbodyOrderConvenienceStore"); } }
	public WrappedHtmlGenericControl WdvUserShipping { get { return GetWrappedControl<WrappedHtmlGenericControl>("tbodyUserShipping"); } }
	public WrappedLiteral WlPointResetMessages { get { return GetWrappedControl<WrappedLiteral>("lPointResetMessages"); } }
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// パラメータが不正?
			if ((this.ActionStatus != Constants.ACTION_STATUS_CONFIRM)
				&& (this.ActionStatus != Constants.ACTION_STATUS_DETAIL)
				&& (this.ActionStatus != Constants.ACTION_STATUS_COMPLETE))
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			if ((this.ActionStatus == Constants.ACTION_STATUS_DETAIL)
				|| (this.ActionStatus == Constants.ACTION_STATUS_COMPLETE))
			{
				this.ModifyInfo = null;
				this.TwModifyInfo = null;
			}

			// 定期購入IDパラメータなし?
			var isExsit = (string.IsNullOrEmpty(this.RequestFixedPurchaseId) == false);

			// 定期購入情報取得
			if (isExsit)
			{
				this.FixedPurchaseContainer = new FixedPurchaseService().GetContainer(this.RequestFixedPurchaseId);
				if (this.FixedPurchaseContainer == null) isExsit = false;
			}
			// 定期商品が存在する場合は、配送種別情報取得
			if (this.IsItemsExist && isExsit)
			{
				this.ShopShipping = new ShopShippingService().Get(
					this.FixedPurchaseContainer.ShopId,
					this.FixedPurchaseContainer.Shippings[0].Items[0].ShippingType);
				if (this.ShopShipping == null) isExsit = false;
			}
			// 配送会社情報取得
			if (isExsit)
			{
				this.DeliveryCompany = new DeliveryCompanyService().Get(
					(this.ModifyInfo == null)
						? this.FixedPurchaseContainer.Shippings[0].DeliveryCompanyId
						: this.ModifyInfo.Shippings[0].DeliveryCompanyId);
				if (this.DeliveryCompany == null) isExsit = false;

				this.FieldMemoSettingList = GetFieldMemoSettingList(Constants.TABLE_FIXEDPURCHASE);
			}

			// 次回購入利用クーポン情報があれば、かつグローバル対応の場合、翻訳クーポン名を設定
			if (isExsit
				&& Constants.GLOBAL_OPTION_ENABLE
				&& (this.FixedPurchaseContainer.NextShippingUseCouponDetail != null))
			{
				this.FixedPurchaseContainer.NextShippingUseCouponDetail = NameTranslationCommon.SetCouponTranslationData(
					new[] { this.FixedPurchaseContainer.NextShippingUseCouponDetail },
					Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE.Split('-')[0],
					Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE)[0];
			}

			// FixedPurchase Invoice
			var canDisplay = OrderCommon.DisplayTwInvoiceInfo();
			if (canDisplay && isExsit)
			{
				this.TwFixedPurchaseInvoice = new TwFixedPurchaseInvoiceService().GetTaiwanFixedPurchaseInvoice(
					this.RequestFixedPurchaseId,
					this.FixedPurchaseContainer.Shippings[0].FixedPurchaseShippingNo);
			}

			// データが存在しない場合はエラーページへ遷移
			if (isExsit == false)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			// エラーメッセージ削除
			if (this.RequestActionKbn != ACTION_KBN_ORDER) this.ErrorMessages = string.Empty;

			if (canDisplay && (this.TwFixedPurchaseInvoice != null))
			{
				// Set Visible For Uniform
				SetVisibleForUniformOption(this.TwFixedPurchaseInvoice.TwUniformInvoice);
			}

			// 画面に値をセット
			// 確認?
			if (this.ActionStatus == Constants.ACTION_STATUS_CONFIRM)
			{
				SetValuesForConfirm();
			}
			// 詳細・完了?
			else
			{
				SetValuesForDetail();

				DisplayForProvisionalCreditCard();
			}
			// 初期化
			Initialize();

			// Set use point for purchase
			SetUsePointForPurchase();

			// ポイントをリセットした時のメッセージをクリア
			this.PointResetMessages = null;
		}

		// 項目メモ一覧取得
		this.FieldMemoSettingList = GetFieldMemoSettingList(Constants.TABLE_FIXEDPURCHASE);
	}

	/// <summary>
	/// 入力画面へ戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBack_Click(object sender, EventArgs e)
	{
		Response.Redirect(CreateFixedPurchaseModifyUrl(this.RequestActionKbn, true));
	}

	/// <summary>
	/// 一覧へ戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnToList_Click(object sender, EventArgs e)
	{
		Response.Redirect(this.SearchInfo.CreateFixedPurchaseListUrl());
	}

	/// <summary>
	/// 購入回数(注文基準)変更リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbFixedPurchaseOrderCountChange_Click(object sender, System.EventArgs e)
	{
		lFixedPurchaseOrderCount.Visible = false;
		tbFixedPurchaseOrderCount.Visible = true;
		tbFixedPurchaseOrderCount.Text = this.FixedPurchaseContainer.OrderCount.ToString();
		spanFixedPurchasetOrderCounChange.Visible = false;
		spanFixedPurchaseOrderCountUpdateCancel.Visible = true;
		pFixedPurchaseOrderCountError.Visible = false;
	}

	/// <summary>
	/// 購入回数(注文基準)更新リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbFixedPurchaseOrderCountUpdate_Click(object sender, System.EventArgs e)
	{
		// 入力チェック
		var input = new FixedPurchaseInput(this.FixedPurchaseContainer)
		{
			OrderCount = tbFixedPurchaseOrderCount.Text
		};
		var isValid = input.Validate(false);
		if (isValid == false)
		{
			pFixedPurchaseOrderCountError.Visible = true;
			pFixedPurchaseOrderCountError.InnerHtml = input.ErrorMessage;
			return;
		}

		// 購入回数(注文基準)更新（更新履歴とともに）
		new FixedPurchaseService()
			.UpdateOrderCount(
				this.FixedPurchaseContainer.FixedPurchaseId,
				int.Parse(input.OrderCount),
				this.LoginOperatorName,
				UpdateHistoryAction.Insert);

		// 再表示
		Response.Redirect(CreateFixedPurchaseCompleteUrl());
	}

	/// <summary>
	/// 購入回数(注文基準)キャンセルリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbFixedPurchaseOrderCountCancel_Click(object sender, System.EventArgs e)
	{
		lFixedPurchaseOrderCount.Visible = true;
		tbFixedPurchaseOrderCount.Visible = false;
		spanFixedPurchasetOrderCounChange.Visible = true;
		spanFixedPurchaseOrderCountUpdateCancel.Visible = false;
		pFixedPurchaseOrderCountError.Visible = false;
	}

	/// <summary>
	/// 購入回数(出荷基準)変更リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbFixedPurchaseShippedCountChange_Click(object sender, System.EventArgs e)
	{
		lFixedPurchaseShippedCount.Visible = false;
		tbFixedPurchaseShippedCount.Visible = true;
		tbFixedPurchaseShippedCount.Text = this.FixedPurchaseContainer.ShippedCount.ToString();
		spanFixedPurchaseShippedCountChange.Visible = false;
		spanFixedPurchaseShippedCountUpdateCancel.Visible = true;
		pFixedPurchaseShippedCountError.Visible = false;
	}

	/// <summary>
	/// 購入回数(出荷基準)更新リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbFixedPurchaseShippedCountUpdate_Click(object sender, System.EventArgs e)
	{
		// 入力チェック
		var input = new FixedPurchaseInput(this.FixedPurchaseContainer);
		input.ShippedCount = tbFixedPurchaseShippedCount.Text;
		var isValid = input.Validate(false);
		if (isValid == false)
		{
			pFixedPurchaseShippedCountError.Visible = true;
			pFixedPurchaseShippedCountError.InnerHtml = input.ErrorMessage;
			return;
		}

		// 購入回数(出荷基準)更新（更新履歴とともに）
		new FixedPurchaseService()
			.UpdateShippedCount(
				this.FixedPurchaseContainer.FixedPurchaseId,
				int.Parse(input.ShippedCount),
				this.LoginOperatorName,
				UpdateHistoryAction.Insert);

		// 再表示
		Response.Redirect(CreateFixedPurchaseCompleteUrl());
	}

	/// <summary>
	/// 購入回数(出荷基準)キャンセルリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbFixedPurchaseShippedCountCancel_Click(object sender, System.EventArgs e)
	{
		lFixedPurchaseShippedCount.Visible = true;
		tbFixedPurchaseShippedCount.Visible = false;
		spanFixedPurchaseShippedCountChange.Visible = true;
		spanFixedPurchaseShippedCountUpdateCancel.Visible = false;
		pFixedPurchaseShippedCountError.Visible = false;
	}

	/// <summary>
	/// 決済情報編集ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnOrderPaymentKbnEdit_Click(object sender, EventArgs e)
	{
		Response.Redirect(CreateFixedPurchaseModifyUrl(ACTION_KBN_PAYMENT));
	}

	/// <summary>
	/// 拡張ステータス更新ボタンクリック
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rExtendStatus_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		if (e.CommandName != "Update") return;
		var ucDate = (DateTimePickerPeriodInputControl)((RepeaterItem)e.Item).FindControl("ucExtendStatusDate");
		var updateDate = DateTime.Parse(ucDate.StartDateTimeString);
		// 拡張ステータス更新（更新履歴とともに）
		new FixedPurchaseService()
			.UpdateExtendStatus(
				this.FixedPurchaseContainer.FixedPurchaseId,
				int.Parse(((Literal)e.Item.FindControl("lExtendNo")).Text),
				((RadioButtonList)e.Item.FindControl("rblExtend")).SelectedValue,
				updateDate,
				this.LoginOperatorName,
				UpdateHistoryAction.Insert);

		// 再表示
		Response.Redirect(CreateFixedPurchaseCompleteUrl());
	}

	/// <summary>
	/// 配送先編集ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnEdit_Click(object sender, EventArgs e)
	{
		Response.Redirect(CreateFixedPurchaseModifyUrl(ACTION_KBN_SHIPPING));
	}

	/// <summary>
	/// 配送パターン編集ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnPatternEdit_Click(object sender, EventArgs e)
	{
		Response.Redirect(CreateFixedPurchaseModifyUrl(ACTION_KBN_PATTERN));
	}

	/// <summary>
	/// 商品情報編集ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnItemEdit_Click(object sender, EventArgs e)
	{
		Response.Redirect(CreateFixedPurchaseModifyUrl(ACTION_KBN_ITEM));
	}

	/// <summary>
	/// 定期商品購入回数(注文基準)変更リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbFixedPurchaseItemOrderCountChange_Click(object sender, EventArgs e)
	{
		var targetIndex = int.Parse(((LinkButton)sender).CommandArgument);
		rItemList.Items[targetIndex].FindControl("spanFixedPurchaseItemOrderCountUpdateCancel").Visible = true;
		rItemList.Items[targetIndex].FindControl("tbFixedPurchaseItemOrderCount").Visible = true;
		rItemList.Items[targetIndex].FindControl("lFixedPurchaseItemOrderCount").Visible = false;
		rItemList.Items[targetIndex].FindControl("lbFixedPurchaseItemOrderCountChange").Visible = false;
	}

	/// <summary>
	/// 定期商品購入回数(注文基準)更新リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbFixedPurchaseItemOrderCountUpdate_Click(object sender, EventArgs e)
	{
		var targetIndex = int.Parse(((LinkButton)sender).CommandArgument);
		// 入力チェック
		var input = new FixedPurchaseInput(this.FixedPurchaseContainer);
		input.Shippings[0].Items[targetIndex].ItemOrderCount = ((TextBox)rItemList.Items[targetIndex].FindControl("tbFixedPurchaseItemOrderCount")).Text;
		var isValid = input.Validate();
		// 入力エラー
		if (isValid == false)
		{
			trFixedPurchaseItemErrorMessagesTitle.Visible
				= trFixedPurchaseItemErrorMessages.Visible
				= lbFixedPurchaseItemOrderCountError.Visible = true;
			lbFixedPurchaseItemOrderCountError.Text = input.ErrorMessage;
			return;
		}

		// 商品バリエーションIDを取得
		var variationId = (((Literal)rItemList.Items[targetIndex].FindControl("lVariationId")).Text == "-")
			? ((Literal)rItemList.Items[targetIndex].FindControl("lProductId")).Text
			: ((Literal)rItemList.Items[targetIndex].FindControl("lProductId")).Text
				+ ((Literal)rItemList.Items[targetIndex].FindControl("lVariationId")).Text.Replace("商品ID + ", "");

		// 定期商品購入回数(注文基準)更新
		new FixedPurchaseService()
			.UpdateItemOrderCount(
				this.FixedPurchaseContainer.FixedPurchaseId,
				variationId,
				int.Parse(input.Shippings[0].Items[targetIndex].ItemOrderCount),
				this.LoginOperatorName,
				UpdateHistoryAction.Insert);

		// 再表示
		Response.Redirect(CreateFixedPurchaseCompleteUrl());
	}

	/// <summary>
	/// 定期商品購入回数(注文基準)キャンセルリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbFixedPurchaseItemOrderCountCancel_Click(object sender, EventArgs e)
	{
		var targetIndex = int.Parse(((LinkButton)sender).CommandArgument);
		rItemList.Items[targetIndex].FindControl("spanFixedPurchaseItemOrderCountUpdateCancel").Visible = false;
		rItemList.Items[targetIndex].FindControl("tbFixedPurchaseItemOrderCount").Visible = false;
		rItemList.Items[targetIndex].FindControl("lFixedPurchaseItemOrderCount").Visible = true;
		rItemList.Items[targetIndex].FindControl("lbFixedPurchaseItemOrderCountChange").Visible = true;
		trFixedPurchaseItemErrorMessagesTitle.Visible
			= trFixedPurchaseItemErrorMessages.Visible
			= lbFixedPurchaseItemOrderCountError.Visible = false;
	}

	/// <summary>
	/// 定期商品購入回数(出荷基準)変更リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbFixedPurchaseItemShippedCountChange_Click(object sender, System.EventArgs e)
	{
		var targetIndex = int.Parse(((LinkButton)sender).CommandArgument);
		rItemList.Items[targetIndex].FindControl("spanFixedPurchaseItemShippedCountUpdateCancel").Visible = true;
		rItemList.Items[targetIndex].FindControl("tbFixedPurchaseItemShippedCount").Visible = true;
		rItemList.Items[targetIndex].FindControl("lFixedPurchaseItemShippedCount").Visible = false;
		rItemList.Items[targetIndex].FindControl("lbFixedPurchaseItemShippedCountChange").Visible = false;
	}

	/// <summary>
	/// 定期商品購入回数(出荷基準)更新リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbFixedPurchaseItemShippedCountUpdate_Click(object sender, System.EventArgs e)
	{
		var targetIndex = int.Parse(((LinkButton)sender).CommandArgument);
		// 入力チェック
		var input = new FixedPurchaseInput(this.FixedPurchaseContainer);
		input.Shippings[0].Items[targetIndex].ItemShippedCount = ((TextBox)rItemList.Items[targetIndex].FindControl("tbFixedPurchaseItemShippedCount")).Text;
		var isValid = input.Validate();
		// 入力エラー
		if (isValid == false)
		{
			trFixedPurchaseItemErrorMessagesTitle.Visible
				= trFixedPurchaseItemErrorMessages.Visible
				= lbFixedPurchaseItemOrderCountError.Visible = true;
			lbFixedPurchaseItemOrderCountError.Text = input.ErrorMessage;
			return;
		}

		// 商品IDを取得
		var variationId = (((Literal)rItemList.Items[targetIndex].FindControl("lVariationId")).Text == "-")
			? ((Literal)rItemList.Items[targetIndex].FindControl("lProductId")).Text
			: ((Literal)rItemList.Items[targetIndex].FindControl("lProductId")).Text
				+ ((Literal)rItemList.Items[targetIndex].FindControl("lVariationId")).Text.Replace("商品ID + ", "");
		// 定期商品購入回数(出荷基準)更新
		new FixedPurchaseService()
			.UpdateItemShippedCount(
				this.FixedPurchaseContainer.FixedPurchaseId,
				variationId,
				int.Parse(input.Shippings[0].Items[targetIndex].ItemShippedCount),
				this.LoginOperatorName,
				UpdateHistoryAction.Insert);

		// 再表示
		Response.Redirect(CreateFixedPurchaseCompleteUrl());
	}

	/// <summary>
	/// 定期商品購入回数(出荷基準)キャンセルリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbFixedPurchaseItemShippedCountCancel_Click(object sender, System.EventArgs e)
	{
		var targetIndex = int.Parse(((LinkButton)sender).CommandArgument);
		rItemList.Items[targetIndex].FindControl("spanFixedPurchaseItemShippedCountUpdateCancel").Visible = false;
		rItemList.Items[targetIndex].FindControl("tbFixedPurchaseItemShippedCount").Visible = false;
		rItemList.Items[targetIndex].FindControl("lFixedPurchaseItemShippedCount").Visible = true;
		rItemList.Items[targetIndex].FindControl("lbFixedPurchaseItemShippedCountChange").Visible = true;
		trFixedPurchaseItemErrorMessagesTitle.Visible
			= trFixedPurchaseItemErrorMessages.Visible
			= lbFixedPurchaseItemOrderCountError.Visible = false;
	}

	/// <summary>
	/// Invoice Edit
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInvoiceEdit_Click(object sender, EventArgs e)
	{
		Response.Redirect(CreateFixedPurchaseModifyUrl(ACTION_KBN_INVOICE));
	}

	/// <summary>
	/// 更新するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, EventArgs e)
	{
		this.ModifyInfo.Shippings[0].ShippingReceivingStoreId = ((this.ModifyInfo.Shippings[0].ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
			? this.ModifyInfo.Shippings[0].ShippingReceivingStoreId
			: string.Empty);
		this.ModifyInfo.Shippings[0].ShippingReceivingStoreType = ((this.ModifyInfo.Shippings[0].ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
			? this.ModifyInfo.Shippings[0].ShippingReceivingStoreType
			: string.Empty);

		this.UpdatedTargets = null;
		var model = this.ModifyInfo.CreateModel();
		var externalPaymentCooperationLog = "";
		TwFixedPurchaseInvoiceModel twFixedPurchaseModel = null;
		if (this.RequestActionKbn == ACTION_KBN_INVOICE)
		{
			twFixedPurchaseModel = this.TwModifyInfo.CreateModel();
		}
		// 決済情報更新
		if (this.RequestActionKbn == ACTION_KBN_PAYMENT)
		{
			var isChangeToProvisionalCreditCard = (this.NeedsRegisterProvisionalCreditCardCardKbnExceptZeus
				&& (model.OrderPaymentKbn == Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID));

			// クレジットカード？
			int? creditBranchNo = null;
			string installmentsCode = "";
			if ((model.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT) || isChangeToProvisionalCreditCard)
			{
				// クレジットカード選択が「新しいクレジットカードを利用する」？
				if (this.ModifyInfo.IsSelectCreditCardNew)
				{
					// カード番号入力情報作成
					if (isChangeToProvisionalCreditCard)
					{
						var userCreditCard = new ProvisionalCreditCardProcessor().RegisterUnregisterdCreditCard(
							this.FixedPurchaseContainer.UserId,
							this.ModifyInfo.CreditCardInput.RegisterCardName,
							Constants.FLG_USERCREDITCARD_REGISTER_ACTION_KBN_FIXEDPURCHASE_MODIFY,
							model.FixedPurchaseId,
							"",
							this.LoginOperatorName,
							UpdateHistoryAction.DoNotInsert);
						creditBranchNo = userCreditCard.BranchNo;
						installmentsCode = this.ModifyInfo.CardInstallmentsCode;
					}
					else
					{
						// クレジットカード登録（追加枝番取得）
						var orderCreditCardInput = this.ModifyInfo.CreditCardInput;
						var userCreditCardInput = orderCreditCardInput.CeateUserCreditCardInput(this.FixedPurchaseContainer.UserId);

						Result resist;
						if (this.IsUserPayTg)
						{
							var customerId = this.VeriTransAccountId;
							userCreditCardInput.CreditToken = Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans
								? CartPayment.CreditTokenInfoBase.CreateCreditTokenInfo(" " + customerId)
								: CartPayment.CreditTokenInfoBase.CreateCreditTokenInfo(this.CreditTokenbyPayTg);
							userCreditCardInput.CompanyCode = orderCreditCardInput.CompanyCode;
							resist = new UserCreditCardRegister().ExecForPayTg(
								userCreditCardInput,
								SiteKbn.Pc,
								orderCreditCardInput.DoRegister,
								this.LoginOperatorName,
								UpdateHistoryAction.DoNotInsert);
						}
						else
						{
							resist = new UserCreditCardRegister().Exec(
								userCreditCardInput,
								SiteKbn.Pc,
								orderCreditCardInput.DoRegister,
								this.LoginOperatorName,
								UpdateHistoryAction.DoNotInsert);
						}
					
						// 与信OK?
						if (resist.Success)
						{
							// カード枝番、カード支払回数コードセット
							creditBranchNo = resist.BranchNo;
							installmentsCode = orderCreditCardInput.InstallmentsCode;

							externalPaymentCooperationLog = string.Format("[{0}] {1}:{2}",
								ReplaceTag("@@DispText.common_message.success@@"),
								//「新規発行枝番」
								ValueText.GetValueText(
									Constants.TABLE_FIXEDPURCHASE,
									Constants.VALUETEXT_PARAM_FIXED_PURCHASE_MESSAGE,
									Constants.VALUETEXT_PARAM_FIXED_PURCHASE_NEW_BRANCH_NUMBER),
								creditBranchNo);
						}
						// 与信NG?
						else
						{
							// 外部決済連携ログを追加
							externalPaymentCooperationLog = string.Format("[{0}] {1}:{2}",
								ReplaceTag("@@DispText.common_message.failure@@"),
								ReplaceTag("@@DispText.common_message.errormessage@@"),
								resist.ErrorMessage);
							new FixedPurchaseService()
								.UpdateForCreditRegisterFail(
									this.FixedPurchaseContainer.FixedPurchaseId,
									this.LoginOperatorName,
									UpdateHistoryAction.DoNotInsert,
									string.Join("\r\n", externalPaymentCooperationLog));

							// 完了メッセージの表示
							trModifyCardErrorMessages.Visible = true;
							lbModifyCardErrorMessages.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_CARD_AUTH_FAILED) + "(" + resist.ErrorMessage + ")";
							trModifyCardFinishMessages.Visible = false;

							// 処理を抜ける
							return;
						}
					}
				}
				else
				{
					// カード枝番、カード支払回数コードセット
					creditBranchNo = model.CreditBranchNo.Value;
					installmentsCode = model.CardInstallmentsCode;
				}

				// クレジットカード決済与信成功更新
				new FixedPurchaseService()
					.UpdateForAuthSuccess(
						this.FixedPurchaseContainer.FixedPurchaseId,
						creditBranchNo.Value,
						installmentsCode,
						this.LoginOperatorName,
						string.Join("\r\n", externalPaymentCooperationLog),
						UpdateHistoryAction.DoNotInsert
						);
			}
			// ペイパルの場合、新たなカード登録
			else if (model.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
			{
				if (this.FixedPurchaseContainer.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
				{
					var userExted = new UserService().GetUserExtend(this.FixedPurchaseContainer.UserId);
					var userCreditCard = PayPalUtility.Payment.RegisterAsUserCreditCard(
						userExted.UserId,
						new PayPalCooperationInfo(userExted),
						this.LoginOperatorName,
						UpdateHistoryAction.Insert);
					creditBranchNo = userCreditCard.BranchNo;
				}
				else
				{
					creditBranchNo = model.CreditBranchNo;
				}
			}

			// 定期購入解約されない状態で継続課金可能な決済変更の場合の処理
			if ((this.FixedPurchaseContainer.IsCancelFixedPurchaseStatus == false)
				&& (this.FixedPurchaseContainer.OrderPaymentKbn != model.OrderPaymentKbn))
			{
				var oldPaymentKbn = this.FixedPurchaseContainer.OrderPaymentKbn;
				var trackingId = this.FixedPurchaseContainer.ExternalPaymentAgreementId;

				// 継続課金の解約を行う
				if (OrderCommon.IsCancelablePaymentContinuousByApi(oldPaymentKbn)
					&& (string.IsNullOrEmpty(trackingId) == false))
				{
					string apiError;
					var result = FixedPurchaseHelper.CancelPaymentContinuous(
						this.FixedPurchaseContainer.FixedPurchaseId,
						oldPaymentKbn,
						trackingId,
						this.LoginOperatorName,
						out apiError);
					if (result == false)
					{
						// 継続課金解約失敗の場合、画面上に表示するメッセージをセッションに格納
						this.PaymentContinousCancelationMessage = WebMessages
							.GetMessages(WebMessages.ERRMSG_MANAGE_PAYMENT_CONTINUOUS_CANCEL_NG_FOR_CHANGE_PAYMENT_FP)
							.Replace("@@ 1 @@", apiError).Replace("@@ 2 @@", trackingId);
					}
				}

				// 継続課金解約APIがない決済方法には画面上に表示するメッセージをセッションに格納
				if (OrderCommon.IsOnlyCancelablePaymentContinuousByManual(oldPaymentKbn))
				{
					this.PaymentContinousCancelationMessage = WebMessages
						.GetMessages(WebMessages.ERRMSG_MANAGE_PAYMENT_CONTINUOUS_CANCEL_AT_SBPS_PAYMENT_TOOL_ALERT)
						.Replace("@@ 1 @@", this.FixedPurchaseContainer.OrderPaymentKbnText)
						.Replace("@@ 2 @@", trackingId);
				}
			}

			// 更新（更新履歴とともに）
			new FixedPurchaseService()
				.UpdateOrderPayment(
					model.FixedPurchaseId,
					model.OrderPaymentKbn,
					creditBranchNo,
					installmentsCode,
					model.ExternalPaymentAgreementId,
					this.LoginOperatorName,
					UpdateHistoryAction.DoNotInsert);
		}
		// 配送先更新（更新履歴とともに）
		else if (this.RequestActionKbn == ACTION_KBN_SHIPPING)
		{
			// 除外対象のユーザーIDを取得する
			var exceptUserIdList = new List<string>();
			foreach (RepeaterItem ri in this.WrUserListForAddressUpdate.Items)
			{
				if (((CheckBox)ri.FindControl("cbUpdateExceptUser")).Checked == false)
				{
					exceptUserIdList.Add(((HiddenField)ri.FindControl("hfUserIdForAddressUpdate")).Value);
				}
			}

			// 除外対象の定期購入IDを取得する
			var exceptFixedPurchaseIdList = new List<string>();
			foreach (RepeaterItem ri in this.WrFixedPurchaseListForAddressUpdate.Items)
			{
				if (((CheckBox)ri.FindControl("cbUpdateExceptFixedPurchase")).Checked == false)
				{
					exceptFixedPurchaseIdList.Add(((HiddenField)ri.FindControl("hfFixedPurchaseIdForAddressUpdate")).Value);
				}
			}

			// 変更した配送先情報をユーザ情報やその他の定期台帳にも反映させる
			var updateAddress = new UpdateAddressOfUserAndFixedPurchase(
				this.LoginOperatorName,
				UpdateHistoryAction.Insert,
				true,
				this.IsShippingAddrJp,
				new UpdateAddressOfUserAndFixedPurchase
					.ExceptId(UpdatedKbn.User, exceptUserIdList.ToArray()),
				new UpdateAddressOfUserAndFixedPurchase
					.ExceptId(UpdatedKbn.OtherFixedPurchase, exceptFixedPurchaseIdList.ToArray()));

			this.UpdatedTargets = updateAddress.AddressMassUpdate(this.AddressUpdatePattern, this.ModifyInfo.CreateModel());

			// 選択していた反映パターンを初期化する
			this.AddressUpdatePattern = null;
		}
		// 配送パターン更新（更新履歴とともに）
		else if (this.RequestActionKbn == ACTION_KBN_PATTERN)
		{
			new FixedPurchaseService()
				.UpdatePattern(
					model.FixedPurchaseId,
					model.FixedPurchaseKbn,
					model.FixedPurchaseSetting1,
					model.NextShippingDate,
					model.NextNextShippingDate,
					this.LoginOperatorName,
					UpdateHistoryAction.DoNotInsert);
		}
		// 商品更新（更新履歴とともに）
		else if (this.RequestActionKbn == ACTION_KBN_ITEM)
		{
			new FixedPurchaseService()
				.UpdateItems(
					model.Shippings[0].Items,
					this.LoginOperatorName,
					UpdateHistoryAction.DoNotInsert);
		}
		else if (this.RequestActionKbn == ACTION_KBN_INVOICE)
		{
			new TwFixedPurchaseInvoiceService()
				.UpdateTaiwanFixedPurchaseInvoice(twFixedPurchaseModel);
		}

		// 更新履歴登録
		new UpdateHistoryService().InsertForFixedPurchase(model.FixedPurchaseId, this.LoginOperatorName);
		new UpdateHistoryService().InsertForUser(model.UserId, this.LoginOperatorName);

		// 再表示
		Response.Redirect(CreateFixedPurchaseCompleteUrl());
	}

	/// <summary>
	/// 定期購入キャンセルボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnFixedPurchaseCancel_Click(object sender, EventArgs e)
	{
		if (Constants.CROSS_POINT_OPTION_ENABLED
			&& (this.FixedPurchaseContainer.NextShippingUsePoint > 0))
		{
			var user = new UserService().Get(this.FixedPurchaseContainer.UserId);

			var errorMessage = FixedPurchaseHelper.UpdateCrossPointApiUserPoint(this.FixedPurchaseContainer, user);

			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}
		}

		// 継続課金解約を行う
		string apiError;
		var success = FixedPurchaseHelper.CancelPaymentContinuous(
			this.FixedPurchaseContainer.FixedPurchaseId,
			this.FixedPurchaseContainer.OrderPaymentKbn,
			this.FixedPurchaseContainer.ExternalPaymentAgreementId,
			this.LoginOperatorName,
			out apiError);

		success = success && new FixedPurchaseService().CancelFixedPurchase(
			this.FixedPurchaseContainer,
				ddlCancelReason.Items.Count != 0 ? ddlCancelReason.SelectedValue : "",
				StringUtility.RemoveUnavailableControlCode(tbCancelMemo.Text),
			this.LoginOperatorName,
			Constants.CONST_DEFAULT_DEPT_ID,
			Constants.W2MP_POINT_OPTION_ENABLED,
			UpdateHistoryAction.Insert);

		if (success)
		{
			if (Constants.CROSS_POINT_OPTION_ENABLED
				&& (this.FixedPurchaseContainer.NextShippingUsePoint > 0))
			{
				var user = new UserService().Get(this.FixedPurchaseContainer.UserId);

				UserUtility.AdjustPointAndMemberRankByCrossPointApi(user);
			}

			// 継続課金解約APIがない決済方法には、画面上に表示するメッセージをセッションに格納
			if (OrderCommon.IsOnlyCancelablePaymentContinuousByManual(this.FixedPurchaseContainer.OrderPaymentKbn))
			{
				this.PaymentContinousCancelationMessage = WebMessages
					.GetMessages(WebMessages.ERRMSG_MANAGE_PAYMENT_CONTINUOUS_CANCEL_AT_SBPS_PAYMENT_TOOL_ALERT)
					.Replace("@@ 1 @@", this.FixedPurchaseContainer.OrderPaymentKbnText)
					.Replace("@@ 2 @@", this.FixedPurchaseContainer.ExternalPaymentAgreementId);
			}

			// 再表示
			Response.Redirect(CreateFixedPurchaseCompleteUrl());
		}
		else
		{
			// エラー画面へ
			Session[Constants.SESSION_KEY_ERROR_MSG] = string.IsNullOrEmpty(apiError)
				? WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_FIXED_PURCHASE_CANCEL_ALERT)
				: WebMessages.GetMessages(WebMessages.ERRMSG_MANAGE_PAYMENT_CONTINUOUS_CANCEL_NG_FOR_CANCEL_FP)
					.Replace("@@ 1 @@", apiError)
					.Replace("@@ 2 @@", this.FixedPurchaseContainer.ExternalPaymentAgreementId);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
	}

	/// <summary>
	/// 仮登録キャンセルボタンをクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnTemporaryRegistrationCancel_Click(object sender, EventArgs e)
	{
		new FixedPurchaseService().CancelTemporaryRegistrationFixedPurchase(
			this.FixedPurchaseContainer.FixedPurchaseId,
			this.LoginOperatorName,
			UpdateHistoryAction.Insert);

		Response.Redirect(CreateFixedPurchaseCompleteUrl());
	}

	/// <summary>
	/// 定期購入スキップボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnFixedPurchaseSkip_Click(object sender, EventArgs e)
	{
		// 定期購入スキップ（更新履歴とともに）
		new FixedPurchaseService()
			.SkipOrder(
				this.FixedPurchaseContainer.FixedPurchaseId,
				this.LoginOperatorName,
				this.ShopShipping,
				Constants.FIXEDPURCHASE_NEXT_SHIPPING_CALCULATION_MODE,
				UpdateHistoryAction.Insert);
		// Update next product
		if (string.IsNullOrEmpty(this.FixedPurchaseContainer.SubscriptionBoxCourseId) == false)
		{
			var userService = new UserService();
			var user = userService.Get(this.FixedPurchaseContainer.UserId);
			UpdateOrCancelFixedPurchaseItemsSubscriptionBox(
				this.FixedPurchaseContainer,
				user.MemberRankId,
				user.Name,
				UpdateHistoryAction.Insert);
		}

		// 再表示
		Response.Redirect(CreateFixedPurchaseCompleteUrl());
	}

	/// <summary>
	/// 定期購入管理メモ更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateFixedPurchaseManagementMemo_Click(object sender, EventArgs e)
	{
		// 定期購入管理メモ更新（更新履歴とともに）
		new FixedPurchaseService()
			.UpdateFixedPurchaseManagementMemo(
				this.FixedPurchaseContainer.FixedPurchaseId,
				StringUtility.RemoveUnavailableControlCode(tbFixedPurchaseManagementMemo.Text),
				this.LoginOperatorName,
				UpdateHistoryAction.Insert);

		// 再表示
		Response.Redirect(CreateFixedPurchaseCompleteUrl());
	}

	/// <summary>
	/// 配送メモ更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateShippingMemo_Click(object sender, EventArgs e)
	{
		// 配送メモ更新（更新履歴とともに）
		new FixedPurchaseService()
			.UpdateShippingMemo(
				this.FixedPurchaseContainer.FixedPurchaseId,
				StringUtility.RemoveUnavailableControlCode(tbShippingMemo.Text),
				this.LoginOperatorName,
				UpdateHistoryAction.Insert);

		// 再表示
		Response.Redirect(CreateFixedPurchaseCompleteUrl());
	}

	/// <summary>
	/// 定期購入今すぐ注文ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnFixedPurchaseOrder_Click(object sender, EventArgs e)
	{
		// 頒布会コースに紐づく定期台帳だが、対象の頒布会が削除されていた場合は定期購入ステータスをその他エラーにして注文を生成しない
		var existSubscriptionBox = CartObject.CheckExistFixedPurchaseLinkSubscriptionBox(
			this.FixedPurchaseContainer.SubscriptionBoxCourseId,
			this.FixedPurchaseContainer.FixedPurchaseId,
			this.LoginOperatorName);
		if (existSubscriptionBox == false)
		{
			var url = new UrlCreator(CreateFixedPurchaseCompleteUrl()).AddParam(
				REQUEST_KEY_ACTION_KBN,
				HttpUtility.UrlEncode(ACTION_KBN_ORDER)).CreateUrl();
			Response.Redirect(url);
		}

		// 注文登録実行
		var orderRegisterFixedPurchase = new OrderRegisterFixedPurchase(
			this.LoginOperatorName,
			Constants.SEND_FIXEDPURCHASE_MAIL_TO_USER,
			cbUpdateNextShippingDate.Checked,
			new FixedPurchaseMailSendTiming(""));
		CrossPointUtility.SetFixedPurchaseOrderForCrossPoint();
		var resultMessages = orderRegisterFixedPurchase.ExecByOrderNow(this.FixedPurchaseContainer.FixedPurchaseId);
		if (resultMessages.AlertMessages.Count > 0 || resultMessages.ErrorMessages.Count > 0)
		{
			var errorMessages = new List<string>();
			errorMessages.AddRange(resultMessages.ErrorMessages);
			errorMessages.AddRange(resultMessages.AlertMessages);
			this.ErrorMessages = StringUtility.ChangeToBrTag(WebSanitizer.HtmlEncode(string.Join("\r\n", errorMessages.ToArray())));
		}
		
		// 頒布会コースの自動繰り返し設定と無期限設定フラグがOFF且つ注文回数が満期になったものは定期購入ステータスを頒布会完了にする
		var subscriptionBox = new SubscriptionBoxService().GetByCourseId(this.FixedPurchaseContainer.SubscriptionBoxCourseId);
		if (subscriptionBox == null)
		{
			Response.Redirect(
				CreateFixedPurchaseCompleteUrl() + "&" + REQUEST_KEY_ACTION_KBN + "="
				+ HttpUtility.UrlEncode(ACTION_KBN_ORDER));
		}

		if ((subscriptionBox.IsNumberTime)
			&& (subscriptionBox.IsAutoRenewal == false)
			&& (subscriptionBox.IsIndefinitePeriod == false))
		{
			var maxCount = subscriptionBox.DefaultOrderProducts.Max(defaultItem => defaultItem.Count).Value;
			if (this.FixedPurchaseContainer.SubscriptionBoxOrderCount >= maxCount)
			{
				new FixedPurchaseService().Complete(
					this.FixedPurchaseContainer.FixedPurchaseId,
					this.LoginOperatorName,
					this.FixedPurchaseContainer.NextShippingDate,
					this.FixedPurchaseContainer.NextNextShippingDate,
					UpdateHistoryAction.Insert,
					null);
			}
		}

		if (subscriptionBox.IsNumberTime == false)
		{
			// 次回配送日がデフォルト商品配送期間の最終日より後の時に定期購入ステータスを頒布会完了にする
			var lastDate = subscriptionBox.DefaultOrderProducts.Max(dp => dp.TermUntil);
			if ((lastDate != null) && (lastDate < this.FixedPurchaseContainer.NextShippingDate))
			{
				new FixedPurchaseService().Complete(
					this.FixedPurchaseContainer.FixedPurchaseId,
					this.LoginOperatorName,
					this.FixedPurchaseContainer.NextShippingDate,
					this.FixedPurchaseContainer.NextNextShippingDate,
					UpdateHistoryAction.Insert,
					null);
			}
		}

		// 再表示
		Response.Redirect(CreateFixedPurchaseCompleteUrl() + "&" + REQUEST_KEY_ACTION_KBN + "=" + HttpUtility.UrlEncode(ACTION_KBN_ORDER));
	}

	/// <summary>
	/// 定期購入解約理由更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnFixedPurchaseCancelReason_Click(object sender, EventArgs e)
	{
		// 定期購入解約理由更新（更新履歴とともに）
		new FixedPurchaseService()
			.UpdateFixedPurchaseCancelReason(
				this.FixedPurchaseContainer.FixedPurchaseId,
				ddlCancelReason.Items.Count != 0 ? ddlCancelReason.SelectedValue : "",
				StringUtility.RemoveUnavailableControlCode(tbCancelMemo.Text),
				this.LoginOperatorName,
				UpdateHistoryAction.Insert);

		// 再表示
		Response.Redirect(CreateFixedPurchaseCompleteUrl());
	}

	/// <summary>
	/// Update payment status
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnPaymentStatus_Click(object sender, EventArgs e)
	{
		// 更新（更新履歴とともに）
		new FixedPurchaseService().UpdatePaymentStatus(
			this.FixedPurchaseContainer.FixedPurchaseId,
			rblPaymenStatus.SelectedValue,
			this.LoginOperatorName,
			UpdateHistoryAction.Insert);

		AppLogger.WriteInfo(
			string.Format(
				"Update: {0} FixedPurchaseId: {1} Operator: {2}({3})",
				//「決済ステータス（定期注文）」
				ValueText.GetValueText(
					Constants.TABLE_FIXEDPURCHASE,
					Constants.VALUETEXT_PARAM_PAYMENT_STATUS_LOG,
					Constants.VALUETEXT_PARAM_PAYMENT_STATUS_REGULAR_ORDER),
				this.FixedPurchaseContainer.FixedPurchaseId,
				this.LoginOperatorName,
				this.LoginOperatorId));
		Response.Redirect(CreateFixedPurchaseCompleteUrl());
	}

	/// <summary>
	/// 次回購入の利用ポイントの全適用フラグ変更時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbUseAllPointFlg_Changed(object sender, EventArgs e)
	{
		var useAllPointFlgInputMethod = (CheckBox)sender;
		if (useAllPointFlgInputMethod.Checked && this.CanUseAllPointFlg)
		{
			this.tbNextShippingUsePoint.Enabled = false;
			// 全ポイント継続利用するため、通常の利用ポイントは0にする
			this.tbNextShippingUsePoint.Text = "0";
			this.tbNextShippingUsePoint.BackColor = Color.Gainsboro;
		}
		else
		{
			this.tbNextShippingUsePoint.Enabled = true;
			this.tbNextShippingUsePoint.BackColor = Color.White;
		}
	}

	/// <summary>
	/// 次回購入の利用ポイントの全適用フラグデータバインド時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbUseAllPointFlg_DataBinding(object sender, EventArgs e)
	{
		var useAllPointFlgInputMethod = (CheckBox)sender;
		useAllPointFlgInputMethod.Checked = (this.FixedPurchaseContainer.UseAllPointFlg == Constants.FLG_FIELD_FIXEDPURCHASE_USE_ALL_POINT_FLG_ON);

		if (useAllPointFlgInputMethod.Checked && this.CanUseAllPointFlg)
		{
			this.tbNextShippingUsePoint.Enabled = false;
			// 全ポイント継続利用するため、通常の利用ポイントは0にする
			this.tbNextShippingUsePoint.Text = "0";
			this.tbNextShippingUsePoint.BackColor = Color.Gainsboro;
		}
		else
		{
			this.tbNextShippingUsePoint.Enabled = true;
			this.tbNextShippingUsePoint.BackColor = Color.White;
		}
	}

	/// <summary>
	/// 「利用ポイント変更」ボタン押下イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateNextShippingUsePoint_Click(object sender, EventArgs e)
	{
		dvNextShippingUsePoint.Visible = (this.dvNextShippingUsePoint.Visible == false);
		if (dvNextShippingUsePoint.Visible)
		{
			tbNextShippingUsePoint.Text = this.FixedPurchaseContainer.NextShippingUsePoint.ToString();
		}
		// 全ポイント継続利用フラグにチェックがついている場合は0ptで表示する
		if (cbUseAllPointFlg.Checked && this.CanUseAllPointFlg)
		{
			tbNextShippingUsePoint.Text = "0";
		}
	}

	/// <summary>
	/// 「利用ポイント変更」の「更新」リンク押下イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbNextShippingUsePointUpdate_Click(object sender, EventArgs e)
	{
		// ポイントOPが無効、または、ユーザではない場合、何も処理しない
		if (Constants.W2MP_POINT_OPTION_ENABLED == false) return;

		// カスタムバリデータ取得（正常完了でも次の画面に遷移しないためエラー初期化）
		var input = new Hashtable()
		{{
			Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_USE_POINT, tbNextShippingUsePoint.Text
		}};

		var errorMessages = Validator.Validate("FixedPurchaseModifyInput", input);
		if (string.IsNullOrEmpty(errorMessages) == false)
		{
			pNextShippingUsePointError.InnerHtml = errorMessages;
			return;
		}

		// 次回購入の利用ポイントの更新をエラーチェック
		var usePoint = decimal.Parse(tbNextShippingUsePoint.Text);
		var useAllPointFlg = (cbUseAllPointFlg.Checked ? Constants.FLG_FIELD_FIXEDPURCHASE_USE_ALL_POINT_FLG_ON : Constants.FLG_FIELD_FIXEDPURCHASE_USE_ALL_POINT_FLG_OFF);

		var nextShippingFixedPurchaseCart = CreateSimpleCartListForFixedPurchase(this.FixedPurchaseContainer.UserId, this.FixedPurchaseContainer.FixedPurchaseId).Items[0];

		var result = NextShippingUsePointUpdateErrorCheck.CheckNextShippingUsePoint(
			this.FixedPurchaseContainer,
			usePoint,
			useAllPointFlg,
			nextShippingFixedPurchaseCart.PriceSubtotalForCampaign);

		if (result != string.Empty)
		{
			pNextShippingUsePointError.InnerHtml = result;
			return;
		}

		// 次回購入の利用ポイントの更新を実行
		var success = false;
		// 全ポイント継続利用に更新がある場合は、フラグの更新も行う
		if (this.CanUseAllPointFlg && (this.FixedPurchaseContainer.UseAllPointFlg != useAllPointFlg))
		{
			success = new FixedPurchaseService().ApplyNextShippingUseAllPointChange(
				Constants.CONST_DEFAULT_DEPT_ID,
				this.FixedPurchaseContainer,
				usePoint,
				useAllPointFlg,
				this.LoginOperatorName,
				UpdateHistoryAction.Insert);
		}
		else
		{
			success = new FixedPurchaseService().ApplyNextShippingUsePointChange(
				Constants.CONST_DEFAULT_DEPT_ID,
				this.FixedPurchaseContainer,
				usePoint,
				this.LoginOperatorName,
				UpdateHistoryAction.Insert);
		}
		if (success)
		{
			if (Constants.CROSS_POINT_OPTION_ENABLED)
			{
				var user = DomainFacade.Instance.UserService.Get(this.FixedPurchaseContainer.UserId);
				var point = (this.FixedPurchaseContainer.NextShippingUsePoint - usePoint);
				var errorMessage = CrossPointUtility.UpdateCrossPointApiWithWebErrorMessage(
					user,
					point,
					CrossPointUtility.GetValue(
						Constants.CROSS_POINT_SETTING_ELEMENT_REASON_ID,
						w2.App.Common.Constants.CROSS_POINT_REASON_KBN_OPERATOR));

				UserUtility.AdjustPointAndMemberRankByCrossPointApi(user);

				if (string.IsNullOrEmpty(errorMessage) == false)
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
				}
			}

			// 再表示
			Response.Redirect(CreateFixedPurchaseDetailUrl(this.FixedPurchaseContainer.FixedPurchaseId));
		}
		else
		{
			// エラー画面へリダイレクト
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_NEXT_SHIPPING_USE_POINT_UPDATE_ALERT);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
	}

	/// <summary>
	/// 「利用ポイント変更」の「キャンセル」リンク押下イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbNextShippingUsePointCancel_Click(object sender, EventArgs e)
	{
		dvNextShippingUsePoint.Visible = false;
		pNextShippingUsePointError.InnerHtml = string.Empty;
	}

	/// <summary>
	/// 定期購入再開ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnResumeFixedPurchase_Click(object sender, EventArgs e)
	{
		// 定期購入再開（更新履歴とともに）
		new FixedPurchaseService().Resume(
			this.FixedPurchaseContainer.FixedPurchaseId,
			this.FixedPurchaseContainer.UserId,
			this.LoginOperatorName,
			null,
			null,
			UpdateHistoryAction.Insert);

		AccountManager.RestoreForFixedPurchaseCancel(this.FixedPurchaseContainer);

		// 再表示
		Response.Redirect(CreateFixedPurchaseCompleteUrl());
	}

	/// <summary>
	/// 初期化
	/// </summary>
	private void Initialize()
	{
		var input = ((this.ModifyInfo != null) ? this.ModifyInfo : new FixedPurchaseInput(this.FixedPurchaseContainer));
		// 表示制御
		switch (this.ActionStatus)
		{
			// 確認
			case Constants.ACTION_STATUS_CONFIRM:
				divFixedPurchase.Visible = false;
				trConfirm.Visible = true;
				btnBackTop.Visible = btnBackBottom.Visible = true;
				btnUpdateTop.Visible = btnUpdateBottom.Visible = true;
				btnToListTop.Visible = btnToListBottom.Visible = false;
				btnOrderPaymentKbnEdit.Visible = false;
				btnEdit.Visible = false;
				btnPatternEdit.Visible = false;
				btnItemEdit.Visible = false;
				btnInvoiceEdit.Visible = false;
				divOrderPayment.Visible = (this.RequestActionKbn == ACTION_KBN_PAYMENT);
				dvFixedPurchaseShipping.Visible = (this.RequestActionKbn == ACTION_KBN_SHIPPING);
				dvFixedPurchasePattern.Visible = (this.RequestActionKbn == ACTION_KBN_PATTERN);
				dvFixedPurchaseItemList.Visible = (this.RequestActionKbn == ACTION_KBN_ITEM);
				trFixedPurchaseDate1.Visible = trFixedPurchaseDate2.Visible = trFixedPurchaseDate3.Visible = false;
				divFixedPurchaseExtendStatus.Visible = false;
				divManagementMemo.Visible = false;
				divShippingMemo.Visible = false;
				dvFixedPurchaseCancel.Visible = false;
				this.WdvFixedPurchaseSuspend.Visible = false;
				dvFixedPurchaseSkip.Visible = false;
				dvFixedPurchaseHistory.Visible = false;
				divFixedPurchaseSendMail.Visible = false;
				divMemo.Visible = false;
				dvInvoice.Visible = (this.RequestActionKbn == ACTION_KBN_INVOICE);
				divShippingMemo.Visible = false;
				tbodyOrderConvenienceStore.Visible = ((dvFixedPurchaseShipping.Visible)
					&& (input.Shippings[0].ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON));
				tbodyUserShipping.Visible = ((dvFixedPurchaseShipping.Visible)
					&& (input.Shippings[0].ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF));
				if (((this.RequestActionKbn == ACTION_KBN_PAYMENT) || (this.RequestActionKbn == ACTION_KBN_SHIPPING))
					&& (this.ModifyInfo.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT)
					&& (this.ModifyInfo.Shippings[0].ShippingMethod == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL))
				{
					// アラートメッセージを表示する
					dvAlertErrorMessages.Visible = true;
					lbAlertErrorMessages.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FIXED_PURCHASE_SHIPPING_MAIL_WITH_COLLECT);
				}
				var fixedPurchase = this.ModifyInfo.CreateModel();

				if (fixedPurchase.Shippings[0].IsMail
					&& (OrderCommon.IsAvailableShippingKbnMail(this.FixedPurchaseContainer.Shippings[0].Items) == false)
					&& (this.RequestActionKbn == ACTION_KBN_SHIPPING))
				{
					dvFixedPurchaseShippingAlert.Visible = true;
					lbFixedPurchaseShippingAlertMessage.Text = WebSanitizer.HtmlEncode(
						WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERREGIST_CANNOT_SHIPPING_MAIL));
				}

				// メール便配送サービスアラート表示
				if (Constants.DELIVERYCOMPANY_MAIL_ESCALATION_ENBLED && fixedPurchase.Shippings[0].IsMail)
				{
					var itemQuantity = 0;
					var totalProductSize = OrderCommon.GetTotalProductSizeFactor(this.FixedPurchaseContainer.Shippings[0].Items
						.Select(item => new KeyValuePair<string, Tuple<int, string, string>>(
							item.ProductId,
							new Tuple<int, string, string>(
								(int.TryParse(item.ItemQuantity.ToString(), out itemQuantity)
									? itemQuantity
									: 0),
								item.ShopId,
								item.VariationId))));
					if (totalProductSize > this.DeliveryCompany.DeliveryCompanyMailSizeLimit)
					{
						dvFixedPurchaseShippingAlert.Visible = true;
						lbFixedPurchaseShippingAlertMessage.Text = WebMessages
							.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERREGIST_CANNOT_SHIPPING_SERVICE)
							.Replace("@@ 1 @@", this.DeliveryCompany.DeliveryCompanyName);
					}
				}

				divReceipt.Visible = false;
				dvOrderExtend.Visible = false;
				break;
			// 詳細・完了
			case Constants.ACTION_STATUS_DETAIL:
			case Constants.ACTION_STATUS_COMPLETE:
				// 注文登録の場合、完了メッセージ、または警告を表示する
				if (this.RequestActionKbn == ACTION_KBN_ORDER)
				{
					divOrderComp.Visible = (this.ActionStatus == Constants.ACTION_STATUS_COMPLETE);
					// 完了メッセージ表示
					if ((this.FixedPurchaseContainer.FixedPurchaseStatus == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_NORMAL)
						|| (this.FixedPurchaseContainer.FixedPurchaseStatus == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_COMPLETE))
					{
						lOrderCompMessages.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FIXED_PURCHASE_STATUS_NORMAL);
					}
					// エラーメッセージ表示
					else
					{
						lOrderCompMessages.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FIXED_PURCHASE_STATUS_ORTHER);
						if (this.ErrorMessages.Length != 0)
						{
							trErrorMessagesTitle.Visible = trErrorMessages.Visible = true;
							lbErrorMessages.Text = this.ErrorMessages;
						}
					}
				}
				else
				{
					divComp.Visible = (this.ActionStatus == Constants.ACTION_STATUS_COMPLETE);
				}

				// 定期商品が削除される場合はエラーメッセージが出る
				if (this.IsItemsExist == false)
				{
					trErrorMessagesTitle.Visible = trErrorMessages.Visible = true;
					lbErrorMessages.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FIXED_PURCHASE_ITEM_DELETED_ERROR);
				}

				// 継続課金解約に関するメッセージを表示
				if (string.IsNullOrEmpty(this.PaymentContinousCancelationMessage) == false)
				{
					lPaymentContinuousAlert.Text = this.PaymentContinousCancelationMessage;
					this.PaymentContinousCancelationMessage = null;
				}

				trDetail.Visible = true;
				btnBackTop.Visible = btnBackBottom.Visible = false;
				btnUpdateTop.Visible = btnUpdateBottom.Visible = false;
				btnToListTop.Visible = btnToListBottom.Visible = true;
				btnOrderPaymentKbnEdit.Visible = true;
				btnEdit.Visible = true;
				btnPatternEdit.Visible = true;
				trFixedPurchaseDate1.Visible = trFixedPurchaseDate2.Visible = trFixedPurchaseDate3.Visible = true;
				// Show anchor link
				divAnchor.Visible = true;
				this.UpdatedTargetsForDisplay = this.UpdatedTargets;
				tbodyOrderConvenienceStore.Visible = ((dvFixedPurchaseShipping.Visible)
					&& (input.Shippings[0].ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON));
				tbodyUserShipping.Visible = ((dvFixedPurchaseShipping.Visible)
					&& (input.Shippings[0].ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF));
				btnSendFixedPurchaseMail.Disabled = (this.IsItemsExist == false);
				break;
		}

		// 拡張ステータス
		if ((this.ActionStatus == Constants.ACTION_STATUS_DETAIL)
			|| (this.ActionStatus == Constants.ACTION_STATUS_COMPLETE))
		{
			var extendData = GetOrderExtendStatusSettingList(this.LoginOperatorShopId);
			var extendStatusList = CreateExtendStatusListFromDataView(extendData, this.FixedPurchaseContainer.DataSource);

			divFixedPurchaseExtendStatus.Visible = (extendStatusList.Count != 0);
			rExtendStatus.DataSource = extendStatusList;
			this.DataBind();

			for (int index = 0; index < rExtendStatus.Items.Count; index++)
			{
				var riRepeaterItem = rExtendStatus.Items[index];
				var extendStatus = extendStatusList[index];

				var rblExtend = (RadioButtonList)riRepeaterItem.FindControl("rblExtend");
				foreach (ListItem li in ValueText.GetValueItemList(
					Constants.TABLE_FIXEDPURCHASE,
					Constants.FIELD_FIXEDPURCHASE_EXTEND_STATUS_BASENAME + 1))
				{
					li.Selected = (li.Value == extendStatus.Param);
					rblExtend.Items.Add(li);
				}

				((Literal)riRepeaterItem.FindControl("lExtendNo")).Text = extendStatus.No;
				((Literal)riRepeaterItem.FindControl("lExtendName")).Text = extendStatus.Name;

				// 更新日時（年・月・日）追加
				var ucDate = (DateTimePickerPeriodInputControl)riRepeaterItem.FindControl("ucExtendStatusDate");
				ucDate.SetStartDate(extendStatus.Time);

				// 拡張ステータス更新日
				var extendStatusUpdateDate =
					this.FixedPurchaseContainer.DataSource[FIELD_ORDER_EXTEND_STATUS_DATE + extendStatus.No] != DBNull.Value
						? this.FixedPurchaseContainer.DataSource[FIELD_ORDER_EXTEND_STATUS_DATE + extendStatus.No].ToString()
						: null;
				((Literal)riRepeaterItem.FindControl("lExtendStatusUpdateDate")).Text =
					DateTimeUtility.ToStringForManager(
						extendStatusUpdateDate,
						DateTimeUtility.FormatType.ShortDate2Letter);
			}
		}

		if ((this.ActionStatus == Constants.ACTION_STATUS_DETAIL)
			|| (this.ActionStatus == Constants.ACTION_STATUS_COMPLETE))
		{
			// 「今すぐ注文」、「利用ポイント更新」、「利用クーポン変更」、「利用クーポン変更」ボタン有効/無効切替
			btnUpdateNextShippingUsePoint.Enabled
				= btnFixedPurchaseOrder.Enabled
				= btnUpdateNextShippingUseCoupon.Enabled = this.FixedPurchaseContainer.IsOrderRegister;

			// 定期購入再開ボタン有効/無効切替
			btnResumeFixedPurchase.Enabled = this.FixedPurchaseContainer.IsResumeOrder && (this.IsInvalidResumePaypay == false);

			// 「次回配送日を更新する」チェックボックスの有効/無効切替
			cbUpdateNextShippingDate.Enabled = this.FixedPurchaseContainer.IsOrderRegister;
			// 「次回配送日を更新する」チェックボックスのデフォルト値設定
			cbUpdateNextShippingDate.Checked = Constants.FIXEDPURCHASEORDERNOW_NEXT_SHIPPING_DATE_UPDATE_DEFAULT;

			// 休止表示切替
			if (this.FixedPurchaseContainer.IsSuspendFixedPurchaseStatus)
			{
				btnFixedPurchaseSkip.Enabled = btnPaymentStatus.Enabled = false;
				this.WtrFixedPurchaseSuspendButtonDisplay.Visible = false;
			}
			else
			{
				this.WtrFixedPurchaseSuspendReasonButtonDisplay.Visible = false;
			}

			// キャンセル表示切替
			if (this.FixedPurchaseContainer.IsCancelFixedPurchaseStatus)
			{
				btnFixedPurchaseSkip.Enabled = btnPaymentStatus.Enabled = false;
				trFixedPurchaseCancelButtonDisplay.Visible = false;
			}
			else
			{
				trFixedPurchaseCancelReasonButtonDisplay.Visible = false;
			}

			// 仮登録と仮登録キャンセルの表示切替
			if (this.FixedPurchaseContainer.IsTemporaryRegistrationStatus
				|| this.FixedPurchaseContainer.IsCancelTemporaryRegistrationStatus)
			{
				// 「利用ポイント更新」、「今すぐ注文」、「次回配送日を更新する」、
				// 「スキップする」、「決済ステータスを更新」、利用ポイント更新」ボタン無効化
				btnUpdateNextShippingUsePoint.Enabled
					= btnFixedPurchaseOrder.Enabled
					= cbUpdateNextShippingDate.Enabled
					= btnFixedPurchaseSkip.Enabled
					= btnPaymentStatus.Enabled
					= btnUpdateNextShippingUseCoupon.Enabled = false;

				//「解約する」、「解約理由メモ」、「解約理由」、「解約理由更新」の非表示
				trFixedPurchaseCancelButtonDisplay.Visible
					= trCancelMemo.Visible
					= trCancelReason.Visible
					= trFixedPurchaseCancelReasonButtonDisplay.Visible = false;

				//「定期購入の休止」の非表示
				this.WdvFixedPurchaseSuspend.Visible = false;

				btnResumeFixedPurchase.Text = WebMessages.GetMessages(
					WebMessages.ERRMSG_MANAGER_FIXED_PURCHASE_CHANGE_STATUS_TO_NORMAL);

				trTemporaryRegistrationCancelButtonDisplay.Visible = true;

				if (this.FixedPurchaseContainer.IsCancelTemporaryRegistrationStatus)
				{
					btnTemporaryRegistrationCancel.Enabled = false;
				}
			}

			// 利用クレジットカード情報表示切替
			trUserCreditCardInfo.Visible =
				((this.FixedPurchaseContainer.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
					|| (this.FixedPurchaseContainer.OrderPaymentKbn == Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID));
			trUserSmsInfo.Visible = OrderCommon.CheckPaymentYamatoKaSms(this.FixedPurchaseContainer.OrderPaymentKbn);
		}

		// ポップアップ表示?
		if (this.RequestWindowKbn == Constants.KBN_WINDOW_POPUP)
		{
			// タイトル、一覧へ戻るを非表示
			trTitleFixedpurchaseTop.Visible = trTitleFixedpurchaseMiddle.Visible = trTitleFixedpurchaseBottom.Visible = false;
			btnToListTop.Visible = btnToListBottom.Visible = false;
		}
		else
		{
			// タイトルを非表示
			trTitleFixedpurchaseTop.Visible = trTitleFixedpurchaseMiddle.Visible = trTitleFixedpurchaseBottom.Visible = true;
		}

		var orderExtend = OrderExtendCommon.CreateOrderExtendForManager(this.FixedPurchaseContainer);
		var orderExtendinput = new OrderExtendInput(orderExtend);
		rOrderExtendInput.DataSource = orderExtendinput.OrderExtendItems;
		rOrderExtendInput.DataBind();
		SetOrderExtendFromUserExtendObject(rOrderExtendInput, orderExtendinput);

		this.UpdatedTargets = null;

		switch (Constants.PAYMENT_PAYPAY_KBN)
		{
			case w2.App.Common.Constants.PaymentPayPayKbn.GMO:
				lInvalidResumePaypay.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FIXED_PURCHASE_RESUMED_PAYPAY_GMO_MESSAGE);
				lCancelPaypayNotification.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FIXED_PURCHASE_CANCEL_PAYPAY_GMO_MESSAGE);
				break;

			case w2.App.Common.Constants.PaymentPayPayKbn.VeriTrans:
				lInvalidResumePaypay.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FIXED_PURCHASE_RESUMED_PAYPAY_VERITRANS_MESSAGE);
				lCancelPaypayNotification.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FIXED_PURCHASE_CANCEL_PAYPAY_VERITRANS_MESSAGE);
				break;
		}
	}

	/// <summary>
	/// 画面に値をセット（詳細用）
	/// </summary>
	private void SetValuesForDetail()
	{
		// 定期購入情報
		var fixedPurchase = this.FixedPurchaseContainer;
		lFixedPurchaseId.Text = WebSanitizer.HtmlEncode(fixedPurchase.FixedPurchaseId);
		lFixedPurchaseSetting1.Text = WebSanitizer.HtmlEncode(OrderCommon.CreateFixedPurchaseSettingMessage(fixedPurchase));
		lFixedPurchaseDate.Text = WebSanitizer.HtmlEncode(
			DateTimeUtility.ToStringForManager(
				fixedPurchase.FixedPurchaseDateBgn,
				DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter));
		lFixedPurchaseOrderCount.Text = WebSanitizer.HtmlEncode(fixedPurchase.OrderCount);
		tbFixedPurchaseOrderCount.Text = fixedPurchase.OrderCount.ToString();
		tbFixedPurchaseOrderCount.Visible = false;
		spanFixedPurchasetOrderCounChange.Visible = true;
		spanFixedPurchaseOrderCountUpdateCancel.Visible = false;
		lFixedPurchaseShippedCount.Text = WebSanitizer.HtmlEncode(fixedPurchase.ShippedCount);
		tbFixedPurchaseShippedCount.Text = fixedPurchase.ShippedCount.ToString();
		tbFixedPurchaseShippedCount.Visible = false;
		spanFixedPurchaseShippedCountChange.Visible = true;
		spanFixedPurchaseShippedCountUpdateCancel.Visible = false;
		lFixedPurchaseLastOrderDate.Text = WebSanitizer.HtmlEncode(
			DateTimeUtility.ToStringForManager(
				fixedPurchase.LastOrderDate,
				DateTimeUtility.FormatType.ShortDate2Letter));
		lFixedPurchaseOrderKbn.Text = WebSanitizer.HtmlEncode(fixedPurchase.OrderKbnText);
		var fixedPurchaseOrderPaymentKbn = fixedPurchase.OrderPaymentKbnText;
		if (OrderCommon.CreditInstallmentsSelectable && (fixedPurchase.CardInstallmentsCode != ""))
		{
			fixedPurchaseOrderPaymentKbn +=
				string.Format("（{0}）",
					UpdateHistoryDisplayFormatSetting
						.GetFormat(Constants.UPDATE_HISTORY_DISPLAY_FORMAT_ORDER_PAYMENT_KBN)
						.Replace("{0}",
				//「（{0}払い）」
							ValueText.GetValueText(
								Constants.TABLE_ORDER,
								OrderCommon.CreditInstallmentsValueTextFieldName,
								fixedPurchase.CardInstallmentsCode)));
		}
		lFixedPurchaseOrderPaymentKbn.Text = WebSanitizer.HtmlEncode(fixedPurchaseOrderPaymentKbn);
		if (((fixedPurchase.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT) || (fixedPurchase.OrderPaymentKbn == Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID))
			&& (fixedPurchase.CreditBranchNo != null))
		{
			// ユーザクレジットカード情報取得
			var userCreditCard = UserCreditCard.Get(fixedPurchase.UserId, fixedPurchase.CreditBranchNo.Value);
			if (userCreditCard != null)
			{
				lUserCreditCardInfo.Text = UserCreditCardHelper.CreateCreditCardInfoHtml(userCreditCard);
			}
		}

		if (OrderCommon.CheckPaymentYamatoKaSms(fixedPurchase.OrderPaymentKbn) && (fixedPurchase.CreditBranchNo != null))
		{
			// ユーザクレジットカード情報取得
			var userCreditCard = UserCreditCard.Get(fixedPurchase.UserId, fixedPurchase.CreditBranchNo.Value);
			if (userCreditCard != null)
			{
				lUserSmsInfo.Text = userCreditCard.CooperationId;
			}
		}
		lOwnerUserId.Text = WebSanitizer.HtmlEncode(fixedPurchase.UserId);
		lOwnerName1.Text =
			lOwnerName2.Text =
				WebSanitizer.HtmlEncode(fixedPurchase.OwnerName);
		lFixedPurchaseStatus.Text = WebSanitizer.HtmlEncode(fixedPurchase.FixedPurchaseStatusText);
		lNextShippingUsePoint.Text = WebSanitizer.HtmlEncode(StringUtility.ToNumeric(fixedPurchase.NextShippingUsePoint));
		cbUseAllPointFlg.Checked = (fixedPurchase.UseAllPointFlg == Constants.FLG_FIELD_FIXEDPURCHASE_USE_ALL_POINT_FLG_ON);
		lUserUsablePoint.Text
			= (Constants.W2MP_POINT_OPTION_ENABLED)
				? WebSanitizer.HtmlEncode(StringUtility.ToNumeric(PointOptionUtility.GetUserPoint(fixedPurchase.UserId, Constants.FLG_USERPOINT_POINT_KBN_BASE).PointUsable))
				: "0";
		spFixedPurchaseStatus.Attributes["class"] = GetFixedPurchaseStatusCssClass(fixedPurchase.FixedPurchaseStatus);
		lFixedPurchaseValidFlg.Text = WebSanitizer.HtmlEncode(fixedPurchase.ValidFlgText);
		lFixedPurchaseDateCreated.Text = WebSanitizer.HtmlEncode(
			DateTimeUtility.ToStringForManager(
				fixedPurchase.DateCreated,
				DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter));
		lFixedPurchaseDateChanged.Text = WebSanitizer.HtmlEncode(
			DateTimeUtility.ToStringForManager(
				fixedPurchase.DateChanged,
				DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter));
		lFixedPurchaseLastChanged.Text = WebSanitizer.HtmlEncode(fixedPurchase.LastChanged);

		hfSubscriptionBoxCourseId.Value = WebSanitizer.HtmlEncode(fixedPurchase.SubscriptionBoxCourseId);
		lSubscriptionBoxCourseCount.Text = WebSanitizer.HtmlEncode(
			(fixedPurchase.SubscriptionBoxOrderCount != 0)
				? StringUtility.ToNumeric(fixedPurchase.SubscriptionBoxOrderCount)
				: " - ");

		// Create list payment status
		rblPaymenStatus.DataSource = ValueText.GetValueItemList(Constants.TABLE_FIXEDPURCHASE, Constants.FIELD_FIXEDPURCHASE_PAYMENT_STATUS);
		rblPaymenStatus.DataBind();
		rblPaymenStatus.SelectedValue = fixedPurchase.PaymentStatus;

		// 定期購入配送先情報
		var shipping = fixedPurchase.Shippings[0];
		lShippingName.Text = WebSanitizer.HtmlEncode(shipping.ShippingName);
		lShippingNameKana.Text = WebSanitizer.HtmlEncode(shipping.ShippingNameKana);
		lShippingTel1.Text = WebSanitizer.HtmlEncode(shipping.ShippingTel1);
		if (shipping.ShippingZip != null)
		{
			if (IsCountryJp(shipping.ShippingCountryIsoCode))
			{
				lShippingZip.Text = WebSanitizer.HtmlEncode("〒" + shipping.ShippingZip);
			}
			else
			{
				lShippingZipGlobal.Text = WebSanitizer.HtmlEncode(shipping.ShippingZip);
			}
		}
		lShippingAddr1.Text = WebSanitizer.HtmlEncode(shipping.ShippingAddr1);
		lShippingAddr2.Text = WebSanitizer.HtmlEncode(shipping.ShippingAddr2);
		lShippingAddr3.Text = WebSanitizer.HtmlEncode(shipping.ShippingAddr3);
		lShippingAddr4.Text = WebSanitizer.HtmlEncode(shipping.ShippingAddr4);
		lShippingAddr5.Text = WebSanitizer.HtmlEncode(shipping.ShippingAddr5);
		lShippingCountryName.Text = WebSanitizer.HtmlEncode(shipping.ShippingCountryName);
		lShippingCompanyName.Text = WebSanitizer.HtmlEncode(shipping.ShippingCompanyName);
		lShippingCompanyPostName.Text = WebSanitizer.HtmlEncode(shipping.ShippingCompanyPostName);
		lShippingMethod.Text = WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD, shipping.ShippingMethod));
		lShippingCompany.Text = WebSanitizer.HtmlEncode(this.DeliveryCompany.DeliveryCompanyName);
		var shippingTime = this.DeliveryCompany.GetShippingTimeMessage(shipping.ShippingTime);
		if (shippingTime == "") shippingTime = ReplaceTag("@@DispText.shipping_time_list.none@@");
		lShippingTime.Text = WebSanitizer.HtmlEncode(shippingTime);

		this.ShippingAddrCountryIsoCode = shipping.ShippingCountryIsoCode;

		// 配送パターン
		lFixedPurchaseShippingPattern.Text = WebSanitizer.HtmlEncode(OrderCommon.CreateFixedPurchaseSettingMessage(fixedPurchase));
		lNextShippingDate.Text = WebSanitizer.HtmlEncode(
			DateTimeUtility.ToStringForManager(
				fixedPurchase.NextShippingDate,
				DateTimeUtility.FormatType.ShortDateWeekOfDay2Letter));
		lNextNextShippingDate.Text = WebSanitizer.HtmlEncode(
			DateTimeUtility.ToStringForManager(
				fixedPurchase.NextNextShippingDate,
				DateTimeUtility.FormatType.ShortDateWeekOfDay2Letter));
		var totalLeadTime = 0;
		var nextScheduledShippingDate = new DateTime?();
		var nextNextScheduledShippingDate = new DateTime?();
		GetShippingInfo(
			fixedPurchase,
			out totalLeadTime,
			out nextScheduledShippingDate,
			out nextNextScheduledShippingDate);
		lTotalLeadTime.Text = WebSanitizer.HtmlEncode(totalLeadTime.ToString());
		lNextScheduledShippingDate.Text = WebSanitizer.HtmlEncode(
			DateTimeUtility.ToStringForManager(
				nextScheduledShippingDate,
				DateTimeUtility.FormatType.ShortDateWeekOfDay2Letter));
		lNextNextScheduledShippingDate.Text = WebSanitizer.HtmlEncode(
			DateTimeUtility.ToStringForManager(
				nextNextScheduledShippingDate,
				DateTimeUtility.FormatType.ShortDateWeekOfDay2Letter));

		var fixedPurchaseInputInfo = new FixedPurchaseInput(fixedPurchase).Shippings[0].Items;
		// 頒布会確認
		if (this.IsSubscriptionBox)
		{
			var selectedSubscriptionBox = DataCacheControllerFacade
					.GetSubscriptionBoxCacheController()
					.Get(fixedPurchase.SubscriptionBoxCourseId);
			foreach (var item in fixedPurchase.Shippings[0].Items.Select((value, index) => new { value, index }))
			{
				// 頒布会キャンペーン期間かどうか
				var selectedSubscriptionBoxItem = selectedSubscriptionBox.SelectableProducts.FirstOrDefault(
					x => (x.ProductId == item.value.ProductId) && (x.VariationId == item.value.VariationId));

				// 次回配送日が頒布会キャンペーン期間かどうか
				if (OrderCommon.IsSubscriptionBoxCampaignPeriodByNextShippingDate(selectedSubscriptionBoxItem, (DateTime)fixedPurchase.NextShippingDate))
				{
					fixedPurchaseInputInfo[item.index].FixedPurchasePrice = selectedSubscriptionBoxItem.CampaignPrice.ToPriceDecimal();
				}
			}
		}

		// 定期購入商品情報
		rItemList.DataSource = fixedPurchaseInputInfo;
		rItemList.DataBind();

		// 管理メモ
		tbFixedPurchaseManagementMemo.Text = fixedPurchase.FixedPurchaseManagementMemo;

		// 注文メモ
		tbMemo.Text = fixedPurchase.Memo;

		// 配送メモ
		tbShippingMemo.Text = fixedPurchase.ShippingMemo;

		// 休止
		if (fixedPurchase.ResumeDate != null)
		{
			ucFixedPurchaseResumetDate.SetStartDate(fixedPurchase.ResumeDate.Value);
		}
		this.WtbSuspendReason.Text = WebSanitizer.HtmlEncode(fixedPurchase.SuspendReason);

		// 解約
		var cancelReasonList = new FixedPurchaseService().GetCancelReasonForEC();
		if (cancelReasonList.Length != 0)
		{
			ddlCancelReason.Items.Add(new ListItem(string.Empty, string.Empty));
			foreach (var cancelReason in cancelReasonList)
			{
				var item = new ListItem(cancelReason.CancelReasonName, cancelReason.CancelReasonId);
				item.Selected = (item.Value == fixedPurchase.CancelReasonId);
				ddlCancelReason.Items.Add(item);
			}
		}
		else
		{
			// 非表示
			trCancelReason.Visible = false;
		}
		tbCancelMemo.Text = fixedPurchase.CancelMemo;

		// 定期購入履歴情報一覧
		var fixedPurchaseHistoryCondition =
			new FixedPurchaseHistoryListSearchCondition() { FixedPurchaseId = this.RequestFixedPurchaseId };
		var fixedPurchaseHistory = new FixedPurchaseService().SearchFixedPurchaseHistory(fixedPurchaseHistoryCondition);

		//Check display view more
		trFixedPurchaseMore.Visible = (fixedPurchaseHistory.Length > Constants.ITEMS_HISTORY_FIRST_DISPLAY);

		//Show fixed purchase history when list purchase history have other values 0
		if (fixedPurchaseHistory.Length != 0)
		{
			rFixedPurchaseHistory.DataSource = fixedPurchaseHistory;
			rFixedPurchaseHistory.DataBind();

			//Fixed Purchase History Count
			lbFixedPurchaseHistoryCount.Text = WebSanitizer.HtmlEncode(
				string.Format(
					"({0})",
					string.Format(
						ReplaceTag("@@DispText.common_message.unit_of_quantity@@"),
						fixedPurchaseHistory.Length)));
			lbFixedPurchaseHistoryCount.Visible = true;
		}
		else
		{
			rFixedPurchaseHistory.Visible = false;
			trFixedPurchaseHistoryError.Visible = true;
			tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
		}

		// メールテンプレートセット
		ddlFixedPurchaseMailId.Items.Add(new ListItem(string.Empty, string.Empty));
		ddlFixedPurchaseMailId.Items.AddRange(
			GetMailTemplateUtility.GetMailTemplateForFixedPurchase(this.LoginOperatorShopId).Select(mail => new ListItem(mail.MailName, mail.MailId)).ToArray());

		// Set data for canel date display
		lCancelDate.Text = fixedPurchase.CancelDate.HasValue
			? WebSanitizer.HtmlEncode(
				DateTimeUtility.ToStringForManager(
					fixedPurchase.CancelDate.Value,
					DateTimeUtility.FormatType.ShortDate2Letter))
			: string.Empty;

		// Set data for restart date display
		lRestartDate.Text = fixedPurchase.RestartDate.HasValue
			? WebSanitizer.HtmlEncode(
				DateTimeUtility.ToStringForManager(
					fixedPurchase.RestartDate.Value,
					DateTimeUtility.FormatType.ShortDate2Letter))
			: string.Empty;

		// 定期購入解約可能回数
		foreach (FixedPurchaseItemInput r in new FixedPurchaseInput(fixedPurchase).Shippings[0].Items)
		{
			if (this.FixedPurchaseCancelableCount == 0 || this.FixedPurchaseCancelableCount < r.FixedPurchaseCancelableCount)
				this.FixedPurchaseCancelableCount = r.FixedPurchaseCancelableCount;
		}

		// Skipped Count Display
		lFixedPurchaseSkippedCount.Text = WebSanitizer.HtmlEncode(fixedPurchase.SkippedCount);

		if (OrderCommon.DisplayTwInvoiceInfo()
			&& (this.TwFixedPurchaseInvoice != null))
		{
			// Invoice Uniform
			lInvoiceUniform.Text = ValueText.GetValueText(
				Constants.TABLE_TWFIXEDPURCHASEINVOICE,
				Constants.FIELD_TWFIXEDPURCHASEINVOICE_TW_UNIFORM_INVOICE,
				this.TwFixedPurchaseInvoice.TwUniformInvoice);

			// Invoice Carry Type Option
			lCarryTypeOption.Text = NameForCarryType(
				this.TwFixedPurchaseInvoice.TwCarryType,
				this.TwFixedPurchaseInvoice.TwCarryTypeOption);

			// Invoice Uniform Type Option
			lCompanyOption1.Text = (this.IsCompany)
				? this.TwFixedPurchaseInvoice.TwUniformInvoiceOption1
				: string.Empty;

			lCompanyOption2.Text = (this.IsCompany)
				? this.TwFixedPurchaseInvoice.TwUniformInvoiceOption2
				: string.Empty;

			lDonateOption.Text = (this.IsDonate)
				? this.TwFixedPurchaseInvoice.TwUniformInvoiceOption1
				: string.Empty;
		}
		var input = ((this.ModifyInfo != null) ? this.ModifyInfo : new FixedPurchaseInput(this.FixedPurchaseContainer));
		if (input.Shippings[0].ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
		{
			lShippingReceivingStoreId.Text = input.Shippings[0].ShippingReceivingStoreId;
			lShippingNameConvenienceStore.Text = input.Shippings[0].ShippingName;
			lShippingAddr4ConvenienceStore.Text = input.Shippings[0].ShippingAddr4;
			lShippingTel1ConvenienceStore.Text = input.Shippings[0].ShippingTel1;
		}

		var userDelFlg = new UserService().Get(fixedPurchase.UserId).DelFlg;
		this.CanResumeFixedPurchase = (userDelFlg == Constants.FLG_USER_DELFLG_UNDELETED);

		// ポイントリセットした時のメッセージセット
		this.WlPointResetMessages.Text = this.PointResetMessages;
	}

	/// <summary>
	/// 定期台帳商品入力クラスに変換
	/// </summary>
	/// <param name="cartProducts">カート商品</param>
	/// <param name="fixedPurchaseId">定期商品ID</param>
	/// <param name="shippingNo">配送先No</param>
	/// <param name="shopId">店舗ID</param>
	/// <param name="memberRankId">会員ランクID</param>
	/// <returns>定期台帳商品入力クラス</returns>
	public IEnumerable<FixedPurchaseItemInput> ToFixedPurchaseItems(
		List<CartProduct> cartProducts,
		string fixedPurchaseId,
		int shippingNo,
		string shopId,
		string memberRankId)
	{
		int itemNo = 1;
		foreach (var productItem in cartProducts)
		{
			var item = new FixedPurchaseItemInput()
			{
				FixedPurchaseId = this.FixedPurchaseContainer.FixedPurchaseId,
				FixedPurchaseItemNo = itemNo.ToString(),
				FixedPurchaseShippingNo = this.FixedPurchaseContainer.Shippings[0].FixedPurchaseShippingNo.ToString(),
				ShopId = this.FixedPurchaseContainer.ShopId,
				ProductId = productItem.ProductId,
				VariationId = productItem.VariationId,
				ItemQuantity = productItem.Count.ToString(),
				ItemQuantitySingle = productItem.CountSingle.ToString(),
				ProductOptionTexts = string.Empty,
				Price = productItem.Price,
			};
			var product = ProductCommon.GetProductVariationInfo(item.ShopId, item.ProductId, item.VariationId, memberRankId);
			if (product.Count != 0)
			{
				item.SupplierId = (string)product[0][Constants.FIELD_PRODUCT_SUPPLIER_ID];
				item.Name = (string)product[0][Constants.FIELD_PRODUCT_NAME];
				item.VariationName1 = (string)product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1];
				item.VariationName2 = (string)product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2];
				item.VariationName3 = (string)product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3];
				item.Price = (decimal)product[0][Constants.FIELD_PRODUCTVARIATION_PRICE];
				item.SpecialPrice = product[0][Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE] != DBNull.Value ? (decimal?)product[0][Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE] : null;
				item.MemberRankPrice = product[0][Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE_VARIATION] != DBNull.Value ? (decimal?)product[0][Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE_VARIATION] : null;
				item.FixedPurchasePrice = product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE] != DBNull.Value ? (decimal?)product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE] : null;
				item.ShippingType = (string)product[0][Constants.FIELD_PRODUCT_SHIPPING_TYPE];
				item.FixedPurchaseFlg = (string)product[0][Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG];
			}  
			itemNo++;  
			yield return item;
		}
	}

	/// <summary>
	/// Name For Carry Type
	/// </summary>
	/// <returns>name</returns>
	private string NameForCarryType(string carryType, string carryTypeOption)
	{
		var carryTypeName = string.Format("{0} : {1}",
			ValueText.GetValueText(
				Constants.TABLE_TWFIXEDPURCHASEINVOICE,
				Constants.FIELD_TWFIXEDPURCHASEINVOICE_TW_CARRY_TYPE,
			carryType),
			carryTypeOption);

		var position = carryTypeName.IndexOf(":");

		if (string.IsNullOrEmpty(carryTypeOption))
		{
			carryTypeName = carryTypeName.Remove(position);
		}

		return this.IsPersonal
			? carryTypeName
			: string.Empty;
	}

	/// <summary>
	/// 画面に値をセット（確認用）
	/// </summary>
	private void SetValuesForConfirm()
	{
		var fixedPurchase = this.ModifyInfo.CreateModel();

		// 決済方法変更
		if (this.RequestActionKbn == ACTION_KBN_PAYMENT)
		{
			var fixedPurchaseOrderPaymentKbn = fixedPurchase.OrderPaymentKbnText;
			var isProvisionalCreditCard = (fixedPurchase.OrderPaymentKbn == Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID);
			var creditCardInput = this.ModifyInfo.CreditCardInput;
			// 支払方法が「クレジットカード」の場合はカード情報表示
			trUserCreditCardInfo.Visible =
				((fixedPurchase.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
					|| (isProvisionalCreditCard && creditCardInput.DoRegister));
			// 分割払い可 AND 支払回数コードあり？
			if (OrderCommon.CreditInstallmentsSelectable && (fixedPurchase.CardInstallmentsCode != ""))
			{
				fixedPurchaseOrderPaymentKbn +=
					string.Format("（{0}）",
						UpdateHistoryDisplayFormatSetting
							.GetFormat(Constants.UPDATE_HISTORY_DISPLAY_FORMAT_ORDER_PAYMENT_KBN)
							.Replace("{0}",
								//「（{0}払い）」
								ValueText.GetValueText(
									Constants.TABLE_ORDER,
									OrderCommon.CreditInstallmentsValueTextFieldName,
									fixedPurchase.CardInstallmentsCode)));
			}
			if (trUserCreditCardInfo.Visible)
			{
				// 仮クレジットカードの場合
				if (isProvisionalCreditCard)
				{
					if (creditCardInput.DoRegister)
					{
						lUserCreditCardInfo.Text =
							WebSanitizer.HtmlEncode(
								string.Format(UserCreditCardHelper.CreateUserCreditCardNameDispFormat(), creditCardInput.RegisterCardName));
					}
				}
				// クレジットカード選択が「新しいクレジットカードを利用する」？
				else if (this.ModifyInfo.SelectCreditCard == FixedPurchaseInput.SelectCreditCardKbn.New)
				{
					var userCreditCardModel = creditCardInput.CeateUserCreditCardModelForDsip();
					lUserCreditCardInfo.Text = this.CreditTokenizedPanUse
						? UserCreditCardHelper.CreateCreditCardInfoHtmlForTokenaizedPan(userCreditCardModel)
						: UserCreditCardHelper.CreateCreditCardInfoHtml(userCreditCardModel);
				}
				// クレジットカード選択が「新しいクレジットカードを利用する」以外？
				else
				{
					lUserCreditCardInfo.Text = UserCreditCardHelper.CreateCreditCardInfoHtml(UserCreditCard.Get(fixedPurchase.UserId, fixedPurchase.CreditBranchNo.Value));
				}
			}
			lFixedPurchaseOrderPaymentKbn.Text = WebSanitizer.HtmlEncode(fixedPurchaseOrderPaymentKbn);
		}
		// 配送先変更
		else if (this.RequestActionKbn == ACTION_KBN_SHIPPING)
		{
			var shipping = fixedPurchase.Shippings[0];
			lShippingName.Text = WebSanitizer.HtmlEncode(shipping.ShippingName);
			lShippingNameKana.Text = WebSanitizer.HtmlEncode(shipping.ShippingNameKana);
			lShippingTel1.Text = WebSanitizer.HtmlEncode(shipping.ShippingTel1);
			if (shipping.ShippingZip != null)
			{
				if (IsCountryJp(shipping.ShippingCountryIsoCode))
				{
					lShippingZip.Text = WebSanitizer.HtmlEncode("〒" + shipping.ShippingZip);
				}
				else
				{
					lShippingZipGlobal.Text = WebSanitizer.HtmlEncode(shipping.ShippingZip);
				}
			}
			lShippingAddr1.Text = WebSanitizer.HtmlEncode(shipping.ShippingAddr1);
			lShippingAddr2.Text = WebSanitizer.HtmlEncode(shipping.ShippingAddr2);
			lShippingAddr3.Text = WebSanitizer.HtmlEncode(shipping.ShippingAddr3);
			lShippingAddr4.Text = WebSanitizer.HtmlEncode(shipping.ShippingAddr4);
			lShippingAddr5.Text = WebSanitizer.HtmlEncode(shipping.ShippingAddr5);
			lShippingCountryName.Text = WebSanitizer.HtmlEncode(shipping.ShippingCountryName);
			lShippingCompanyName.Text = WebSanitizer.HtmlEncode(shipping.ShippingCompanyName);
			lShippingCompanyPostName.Text = WebSanitizer.HtmlEncode(shipping.ShippingCompanyPostName);
			lShippingMethod.Text = WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD, shipping.ShippingMethod));
			lShippingCompany.Text = WebSanitizer.HtmlEncode(this.DeliveryCompany.DeliveryCompanyName);
			var shippingTime = this.DeliveryCompany.GetShippingTimeMessage(shipping.ShippingTime);
			if (shippingTime == "") shippingTime = ReplaceTag("@@DispText.shipping_time_list.none@@");
			lShippingTime.Text = WebSanitizer.HtmlEncode(shippingTime);

			this.ShippingAddrCountryIsoCode = shipping.ShippingCountryIsoCode;

			// 反映対象となるユーザ情報と定期購入IDを取得する
			var updateAddress = new UpdateAddressOfUserAndFixedPurchase(
				this.LoginOperatorName,
				UpdateHistoryAction.DoNotInsert,
				false,
				this.IsShippingAddrJp);
			this.DoUpdateTargets = updateAddress.AddressMassUpdate(this.AddressUpdatePattern, this.ModifyInfo.CreateModel());

			// 反映対象のユーザー情報
			var userInfo = this.DoUpdateTargets.Cast<IUpdated>()
				.Where(updated => (updated.UpdatedKbn == UpdatedKbn.User))
				.Select(updated => ((UpdatedUser)updated).User).ToArray();
			this.WrUserListForAddressUpdate.DataSource = this.DoUpdateUser ? userInfo : null;
			this.WrUserListForAddressUpdate.DataBind();

			this.WdvUserForAddressUpdate.Visible = ((userInfo != null) && (userInfo.Length > 0));

			// 反映対象のほか定期情報
			var fixedPurchaseInfo = this.DoUpdateTargets.Cast<IUpdated>()
				.Where(updated => (updated.UpdatedKbn == UpdatedKbn.OtherFixedPurchase))
				.Select(updated => ((UpdatedFixedPurchase)updated).FixedPurchase).ToArray();
			this.WrFixedPurchaseListForAddressUpdate.DataSource =
				this.DoUpdateOtherFixedPurchaseInfo ? fixedPurchaseInfo : null;
			this.WrFixedPurchaseListForAddressUpdate.DataBind();

			this.WdvFixedPurchaseForAddressUpdate.Visible = ((fixedPurchaseInfo != null) && (fixedPurchaseInfo.Length > 0));

			var input = ((this.ModifyInfo != null) ? this.ModifyInfo : new FixedPurchaseInput(this.FixedPurchaseContainer));
			this.WdvOrderConvenienceStore.Visible = (input.Shippings[0].ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON);
			this.WdvUserShipping.Visible = (input.Shippings[0].ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON);
			if (this.WdvOrderConvenienceStore.Visible)
			{
				lShippingReceivingStoreId.Text = input.Shippings[0].ShippingReceivingStoreId;
				lShippingNameConvenienceStore.Text = input.Shippings[0].ShippingName;
				lShippingAddr4ConvenienceStore.Text = input.Shippings[0].ShippingAddr4;
				lShippingTel1ConvenienceStore.Text = input.Shippings[0].ShippingTel1;
			}
		}
		// 配送パターン変更
		else if (this.RequestActionKbn == ACTION_KBN_PATTERN)
		{
			lFixedPurchaseShippingPattern.Text = WebSanitizer.HtmlEncode(OrderCommon.CreateFixedPurchaseSettingMessage(fixedPurchase));
			lNextShippingDate.Text = WebSanitizer.HtmlEncode(
				DateTimeUtility.ToStringForManager(
					fixedPurchase.NextShippingDate,
					DateTimeUtility.FormatType.ShortDateWeekOfDay2Letter));
			lNextNextShippingDate.Text = WebSanitizer.HtmlEncode(
				DateTimeUtility.ToStringForManager(
					fixedPurchase.NextNextShippingDate,
					DateTimeUtility.FormatType.ShortDateWeekOfDay2Letter));
			var totalLeadTime = 0;
			var nextScheduledShippingDate = new DateTime?();
			var nextNextScheduledShippingDate = new DateTime?();
			GetShippingInfo(
				fixedPurchase,
				out totalLeadTime,
				out nextScheduledShippingDate,
				out nextNextScheduledShippingDate);
			lTotalLeadTime.Text = WebSanitizer.HtmlEncode(totalLeadTime.ToString());
			lNextScheduledShippingDate.Text = WebSanitizer.HtmlEncode(
				DateTimeUtility.ToStringForManager(
					nextScheduledShippingDate,
					DateTimeUtility.FormatType.ShortDateWeekOfDay2Letter));
			lNextNextScheduledShippingDate.Text = WebSanitizer.HtmlEncode(
				DateTimeUtility.ToStringForManager(
					nextNextScheduledShippingDate,
					DateTimeUtility.FormatType.ShortDateWeekOfDay2Letter));
		}
		// 商品変更
		else if (this.RequestActionKbn == ACTION_KBN_ITEM)
		{
			rItemList.DataSource = this.ModifyInfo.Shippings[0].Items;
			rItemList.DataBind();
		}
		else if (this.RequestActionKbn == ACTION_KBN_INVOICE)
		{
			lInvoiceUniform.Text = ValueText.GetValueText(
				Constants.TABLE_TWFIXEDPURCHASEINVOICE,
				Constants.FIELD_TWFIXEDPURCHASEINVOICE_TW_UNIFORM_INVOICE,
				this.TwModifyInfo.TwUniformInvoice);

			SetVisibleForUniformOption(this.TwModifyInfo.TwUniformInvoice);

			SetVisibleForCarryTypeOption(this.TwModifyInfo.TwCarryType);

			divShippingMemo.Visible = false;

			lCompanyOption1.Text = (this.IsCompany) ? this.TwModifyInfo.TwUniformInvoiceOption1 : string.Empty;
			lCompanyOption2.Text = (this.IsCompany) ? this.TwModifyInfo.TwUniformInvoiceOption2 : string.Empty;

			lDonateOption.Text = (this.IsDonate) ? this.TwModifyInfo.TwUniformInvoiceOption1 : string.Empty;

			lCarryTypeOption.Text = NameForCarryType(this.TwModifyInfo.TwCarryType, this.TwModifyInfo.TwCarryTypeOption);
		}
	}

	/// <summary>
	/// 受注情報詳細リンク作成
	/// </summary>
	/// <param name="orderId">注文ID</param>
	/// <returns>受注情報詳細リンク</returns>
	protected string CreateOrderDetailLink(string orderId)
	{
		if (orderId.Length == 0) return "-";

		var link = new StringBuilder();

		if (MenuUtility.HasAuthorityEc(this.LoginShopOperator, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_ORDER_CONFIRM))
		{
			link.Append("<a href=\"").Append("javascript:open_window('")
				.Append(WebSanitizer.UrlAttrHtmlEncode(CreateOrderDetailUrl(orderId, true, false)))
				.Append("','ordercontact','width=1000,height=600,top=110,left=380,status=NO,scrollbars=yes');")
				.Append("\" >");
		}
		link.Append(WebSanitizer.HtmlEncode(orderId));
		if (MenuUtility.HasAuthorityEc(this.LoginShopOperator, Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_ORDER_CONFIRM))
		{
			link.Append("</a>");
		}

		return link.ToString();
	}

	/// <summary>
	/// ユーザ情報詳細URL作成
	/// </summary>
	/// <returns>ユーザ情報詳細URL</returns>
	protected string CreateUserDetailUrl()
	{
		var isPopup = (Request.FilePath.ToLower().Contains(Constants.PAGE_MANAGER_FIXEDPURCHASE_CONFIRM.ToLower()) == false);

		var url = new StringBuilder();
		url.Append(Constants.PATH_ROOT).Append(isPopup ? Constants.PAGE_MANAGER_USER_CONFIRM_POPUP : Constants.PAGE_MANAGER_USER_CONFIRM);
		url.Append("?").Append(Constants.REQUEST_KEY_USER_ID).Append("=").Append(HttpUtility.UrlEncode(this.FixedPurchaseContainer.UserId));
		url.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_DETAIL);
		url.Append("&").Append(Constants.REQUEST_KEY_WINDOW_KBN).Append("=").Append(Constants.KBN_WINDOW_POPUP);

		return url.ToString();
	}

	/// <summary>
	/// 定期購入編集URL作成
	/// </summary>
	/// <param name="actionKbn">アクション区分</param>
	/// <param name="isReinput">再入力（のため入力画面へ戻る）？</param>
	/// <returns>定期購入編集URL</returns>
	private string CreateFixedPurchaseModifyUrl(string actionKbn, bool isReinput = false)
	{
		var url = new StringBuilder();
		url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_FIXEDPURCHASE_MODIFY_INPUT);
		url.Append("?").Append(Constants.REQUEST_KEY_FIXEDPURCHASE_FIXED_PURCHASE_ID).Append("=").Append(StringUtility.ToEmpty(this.RequestFixedPurchaseId));
		url.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_UPDATE);
		url.Append("&").Append(REQUEST_KEY_ACTION_KBN).Append("=").Append(actionKbn);
		if (isReinput)
		{
			url.Append("&").Append(REQUEST_KEY_REINPUT_KBN).Append("=").Append(REINPUT_KBN_ON);
		}

		return url.ToString();
	}

	/// <summary>
	/// 外部決済連携ログ詳細へのhref属性値作成
	/// </summary>
	/// <param name="fixedPurchaseId">定期購入ID</param>
	/// <param name="fixedPurchaseHistoryNo">定期購入注文履歴NO</param>
	/// <returns>外部決済連携ログ詳細ページへのURLを含むhref属性値</returns>
	protected string CreateExternalPaymentCooperationHref(string fixedPurchaseId, string fixedPurchaseHistoryNo)
	{
		var noParamUrl = Constants.PATH_ROOT + Constants.PAGE_MANAGER_FIXEDPURCHASE_EXTERNAL_PAYMENT_COOPERATION_DETAILS;

		var url = new UrlCreator(noParamUrl)
			.AddParam(Constants.FIELD_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_ID, fixedPurchaseId).AddParam(
				Constants.FIELD_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_NO,
				fixedPurchaseHistoryNo).CreateUrl();

		var hrefStr = "javascript:open_window('" + url
			+ "','watchLog','width=800,height=425,top=5,left=600,status=NO,scrollbars=yes,resizable=yes')";

		return hrefStr;
	}

	/// <summary>
	/// リロードボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnReloadForRegisterdCreditCard_Click(object sender, EventArgs e)
	{
		Response.Redirect(CreateFixedPurchaseDetailUrl(this.FixedPurchaseContainer.FixedPurchaseId));
	}

	/// <summary>
	/// 仮クレジット向けフォーム表示
	/// </summary>
	private void DisplayForProvisionalCreditCard()
	{
		var isProvisionalCreditCardPaymentId =
			(this.FixedPurchaseContainer.OrderPaymentKbn == Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID);
		tbdyCreditCardRegisterForUnregisterd.Visible = false;
		var isProvisionalCreditCardExceptZeus = (this.NeedsRegisterProvisionalCreditCardCardKbnExceptZeus && isProvisionalCreditCardPaymentId);
		if (isProvisionalCreditCardExceptZeus)
		{
			var userCreditCard = UserCreditCard.Get(this.FixedPurchaseContainer.UserId, this.FixedPurchaseContainer.CreditBranchNo.Value);
			tbdyCreditCardRegisterForUnregisterd.Visible = userCreditCard.IsRegisterdStatusUnregisterd;
			if (tbdyCreditCardRegisterForUnregisterd.Visible)
			{
				if (OrderCommon.IsPaymentCardTypeGmo)
				{
					lGmoMemberId.Text =
						WebSanitizer.HtmlEncode(string.Join("-", StringUtility.SplitByLength(userCreditCard.CooperationInfo.GMOMemberId, 4)));
				}
				else if (OrderCommon.IsPaymentCardTypeYamatoKwc)
				{
					lYamatoKwcOrderNo.Text =
						WebSanitizer.HtmlEncode(string.Join("-", StringUtility.SplitByLength(DateTime.Now.ToString("yyMMddHHmmss"), 4)));
					lYamatoKwcMemberId.Text =
						WebSanitizer.HtmlEncode(string.Join("-", StringUtility.SplitByLength(userCreditCard.CooperationInfo.YamatoKwcMemberId, 4)));
					lYamatoKwcAuthenticationKey.Text =
						WebSanitizer.HtmlEncode(userCreditCard.CooperationInfo.YamatoKwcAuthenticationKey);
				}
				else if (OrderCommon.IsPaymentCardTypeEScott)
				{
					lEScottKaiinId.Text = WebSanitizer.HtmlEncode(
						string.Join("-", StringUtility.SplitByLength(userCreditCard.CooperationInfo.CooperationId1, 4)));
				}
			}
		}
	}

	/// <summary>
	/// Subscription memo update button click
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateFixedPurchaseMemo_Click(object sender, EventArgs e)
	{
		// Subscription memo update (with update history)
		new FixedPurchaseService()
			.UpdateFixedPurchaseMemo(
				this.FixedPurchaseContainer.FixedPurchaseId,
				StringUtility.RemoveUnavailableControlCode(tbMemo.Text),
				this.LoginOperatorName,
				UpdateHistoryAction.Insert);

		// Redirect
		Response.Redirect(CreateFixedPurchaseCompleteUrl());
	}

	/// <summary>
	/// リロードボタンクリック（ポップアップ子ウィンドウからの更新）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbReloadList_Click(object sender, EventArgs e)
	{
		Response.Redirect(CreateFixedPurchaseCompleteUrl());
	}

	/// <summary>
	/// 定期再開予定日リスト取得
	/// </summary>
	/// <returns>定期再開予定日リスト</returns>
	protected ListItem[] GetFixedPurchaseResumetDateList()
	{
		return DateTimeUtility.GetYearListItem(DateTime.Now.Year, DateTime.Now.Year + 10);
	}

	/// <summary>
	/// 定期購入休止理由更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnFixedPurchaseSuspendReason_Click(object sender, EventArgs e)
	{
		// 定期再開予定日正当性チェック
		var errorMessage = string.Empty;
		errorMessage = ValidateResumeDate(ucFixedPurchaseResumetDate.HfStartDate.Value);

		// 入力エラー?
		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			this.WtrFixedPurchaseSuspendErrorMessages.Visible = true;
			this.WlbFixedPurchaseSuspendErrorMessages.Text = errorMessage;
			// 処理を抜ける
			return;
		}

		// 定期購入休止理由更新（更新履歴とともに）
		new FixedPurchaseService()
			.UpdateFixedPurchaseSuspendReason(
				this.FixedPurchaseContainer.FixedPurchaseId,
				this.ResumeDate,
				StringUtility.RemoveUnavailableControlCode(this.WtbSuspendReason.Text),
				this.LoginOperatorName,
				UpdateHistoryAction.Insert);

		// 再表示
		Response.Redirect(CreateFixedPurchaseCompleteUrl());
	}

	/// <summary>
	/// 定期購入休止ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnFixedPurchaseSuspend_Click(object sender, EventArgs e)
	{
		// 定期再開予定日正当性チェック
		var errorMessage = string.Empty;
		errorMessage = ValidateResumeDate(ucFixedPurchaseResumetDate.HfStartDate.Value);

		// 入力エラー?
		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			this.WtrFixedPurchaseSuspendErrorMessages.Visible = true;
			this.WlbFixedPurchaseSuspendErrorMessages.Text = errorMessage;
			// 処理を抜ける
			return;
		}

		// 定期休止
		var success = new FixedPurchaseService().SuspendFixedPurchase(
			this.FixedPurchaseContainer,
			this.ResumeDate,
			null,
			null,
			StringUtility.RemoveUnavailableControlCode(this.WtbSuspendReason.Text),
			this.LoginOperatorName,
			UpdateHistoryAction.Insert);

		if (success)
		{
			// 再表示
			Response.Redirect(CreateFixedPurchaseCompleteUrl());
		}
		else
		{
			// エラー画面へ
			Session[Constants.SESSION_KEY_ERROR_MSG] =
				WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_FIXED_PURCHASE_SUSPEND_ALERT);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
	}

	/// <summary>
	/// 定期再開予定日正当性チェック
	/// </summary>
	/// <param name="resumeDateString">定期再開予定日</param>
	/// <returns>エラーメッセージ</returns>
	private string ValidateResumeDate(string resumeDateString)
	{
		// 定期再開予定日を指定しない場合はそのまま空文字を返す
		if (string.IsNullOrEmpty(resumeDateString)) return string.Empty;

		// 日付正当性チェック
		var returnMessages = new List<string>();
		DateTime resumeDate;
		if (DateTime.TryParse(resumeDateString, out resumeDate) == false)
		{
			returnMessages.Add(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FIXED_PURCHASE_RESUME_DATE_TYPE_ERROR));
		}

		if ((returnMessages.Count == 0) && (resumeDate <= DateTime.Today))
		{
			returnMessages.Add(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FIXED_PURCHASE_RESUME_DATE_BEFORE_NEXT_DAY_ERROR));
		}

		return string.Concat(returnMessages);
	}

	/// <summary>
	/// ログが空かnullかどうか。
	/// </summary>
	/// <param name="externalPaymentCooperationLog">外部決済連携ログ</param>
	/// <returns>空かnullであればtrue</returns>
	protected bool IsNullOrEmptyExternalPaymentCooperationLog(string externalPaymentCooperationLog)
	{
		var result = string.IsNullOrEmpty(externalPaymentCooperationLog);
		return result;
	}

	/// <summary>
	/// 「利用クーポン変更」ボタン押下イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateNextShippingUseCoupon_Click(object sender, EventArgs e)
	{
		dvNextShippingUseCoupon.Visible = (this.dvNextShippingUseCoupon.Visible == false);
		if (dvNextShippingUseCoupon.Visible)
		{
			tbCouponCode.Text = string.Empty;
		}
	}

	/// <summary>
	/// 「利用クーポン変更」の「キャンセル」リンク押下イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbNextShippingUseCouponCancel_Click(object sender, EventArgs e)
	{
		dvNextShippingUseCoupon.Visible = false;
		pNextShippingUseCouponError.InnerHtml = string.Empty;
	}

	/// <summary>
	/// 「クーポン適用」 ボタン押下イベント（利用クーポンをDBに更新）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateUseCoupon_Click(object sender, EventArgs e)
	{
		// クーポンOPが無効の場合、何も処理しない
		if (Constants.W2MP_COUPON_OPTION_ENABLED == false) return;

		// 入力したクーポンコード取得
		var couponCode = StringUtility.ToHankaku(tbCouponCode.Text).Trim();

		// 利用クーポンをリセットしないか
		var isNotResetUseCoupon = (string.IsNullOrEmpty(couponCode) == false);

		// 空のクーポンコードを入力した場合、かつ次回購入利用クーポンが未設定であり、エラーメッセージを表示する
		if ((isNotResetUseCoupon == false)
			&& string.IsNullOrEmpty(this.FixedPurchaseContainer.NextShippingUseCouponId))
		{
			pNextShippingUseCouponError.InnerHtml = CommerceMessages.GetMessages(
				CommerceMessages.ERRMSG_FRONT_NEXT_SHIPPING_USE_COUPON_NO_CHANGE_ERROR);
			return;
		}

		// 次回購入の利用クーポンの更新をエラーチェック
		var errorMessage = string.Empty;
		UserCouponDetailInfo inputCoupon = null;
		if (isNotResetUseCoupon)
		{
			// クーポン詳細情報取得
			var inputCoupons = new CouponService().GetAllUserCouponsFromCouponCode(
				Constants.W2MP_DEPT_ID,
				this.FixedPurchaseContainer.UserId,
				couponCode);

			// クーポンを利用可能のチェック
			inputCoupon = FixedPurchaseHelper.CheckAndGetUseCouponForNextShipping(
				couponCode,
				inputCoupons,
				this.FixedPurchaseContainer,
				CreateSimpleCartListForFixedPurchase(
					this.FixedPurchaseContainer.UserId,
					this.FixedPurchaseContainer.FixedPurchaseId),
				out errorMessage);
		}
		if (errorMessage != string.Empty)
		{
			pNextShippingUseCouponError.InnerHtml = errorMessage;
			return;
		}


		//次回購入利用クーポンで割引額が満たされる場合は、次回購入利用ポイントを0にする
		if ((inputCoupon != null) && this.FixedPurchaseContainer.NextShippingUsePoint > 0)
		{
			var nextShippingFixedPurchaseCart = CreateSimpleCartListForFixedPurchase(this.FixedPurchaseContainer.UserId, this.FixedPurchaseContainer.FixedPurchaseId).Items[0];

			var discountable = FixedPurchaseHelper.CheckDiscountableForNextFixedPurchase(
				inputCoupon,
				this.FixedPurchaseContainer.NextShippingUsePoint,
				nextShippingFixedPurchaseCart.PriceSubtotalForCampaign);

			if (discountable == false)
			{
				new FixedPurchaseService().ApplyNextShippingUsePointChange(
					Constants.CONST_DEFAULT_DEPT_ID,
					this.FixedPurchaseContainer,
					0,
					this.FixedPurchaseContainer.LastChanged,
					UpdateHistoryAction.Insert);

				this.PointResetMessages = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_NEXT_SHIPPING_USE_POINT_RESET_ALERT);
			}
		}
		// 利用クーポン情報更新の実行
		var success = FixedPurchaseHelper.ChangeNextShippingUseCoupon(
			this.FixedPurchaseContainer,
			inputCoupon,
			isNotResetUseCoupon,
			this.LoginOperatorName);

		if (success)
		{
			// 再表示
			Response.Redirect(CreateFixedPurchaseDetailUrl(this.FixedPurchaseContainer.FixedPurchaseId));
		}
		else
		{
			// エラーメッセージを表示
			pNextShippingUseCouponError.InnerHtml = CommerceMessages.GetMessages(
				CommerceMessages.ERRMSG_FRONT_NEXT_SHIPPING_USE_COUPON_UPDATE_ALERT);
		}
	}

	/// <summary>
	/// ユーザークーポン一覧URL作成
	/// </summary>
	/// <param name="actionStatus">アクションステータス</param>
	/// <returns>URL</returns>
	protected string CreateUserCouponListUrl(string actionStatus)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDER_USER_COUPON_LIST)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, actionStatus)
			.AddParam(Constants.REQUEST_KEY_USER_ID, this.FixedPurchaseContainer.UserId)
			.AddParam(Constants.REQUEST_KEY_USER_MAIL_ADDR, this.FixedPurchaseContainer.OwnerMailAddr)
			.AddParam(Constants.REQUEST_KEY_ORDER_OWNER_KBN, this.FixedPurchaseContainer.OrderKbn)
			.CreateUrl();
		return url;
	}

	/// <summary>
	/// ユーザークーポン数取得
	/// </summary>
	/// <param name="deptId">識別ID</param>
	/// <param name="userId">ユーザーID</param>
	/// <param name="mail">アドレス</param>
	/// <param name="userCouponsOnly">ユーザーの持つクーポンのみか</param>
	/// <returns>利用可能クーポン数</returns>
	protected int GetUsableCouponCount(string deptId, string userId, string mail, bool userCouponsOnly)
	{
		var condition = new CouponListSearchCondition
		{
			DeptId = deptId,
			UserId = userId,
			MailAddress = mail,
			CouponCode = "",
			CouponName = "",
			UserCouponOnly = userCouponsOnly ? "1" : "0",
			UsedUserJudgeType = Constants.COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE
		};

		var result = new CouponService().GetAllUserUsableCouponsCount(condition);
		return result;
	}

	/// <summary>
	/// 「領収書情報更新」 ボタン押下イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateReceipt_Click(object sender, EventArgs e)
	{
		var address = StringUtility.RemoveUnavailableControlCode(tbReceiptAddress.Text.Trim());
		var proviso = StringUtility.RemoveUnavailableControlCode(tbReceiptProviso.Text.Trim());
		// 領収書希望ありの時に、宛名と但し書きの入力チェックを行う
		if (rblReceiptFlg.SelectedValue == Constants.FLG_ORDER_RECEIPT_FLG_ON)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_ORDER_RECEIPT_ADDRESS, address },
				{ Constants.FIELD_ORDER_RECEIPT_PROVISO, proviso },
			};
			var errorMessage = Validator.Validate("OrderReceipt", input);
			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				tbdyReceiptErrorMessages.Visible = true;
				lbReceiptErrorMessages.Text = errorMessage;
				return;
			}
		}

		// 領収書情報更新
		new FixedPurchaseService().UpdateReceiptInfo(
			this.FixedPurchaseContainer.FixedPurchaseId,
			rblReceiptFlg.SelectedValue,
			address,
			proviso,
			this.LoginOperatorName,
			UpdateHistoryAction.Insert);

		// 再表示
		Response.Redirect(CreateFixedPurchaseCompleteUrl());
	}

	/// <summary>
	/// Clear Skipped Count Fixed Purchase
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnClearSkippedCount_Click(object sender, EventArgs e)
	{
		new FixedPurchaseService().ClearSkippedCount(this.FixedPurchaseContainer.FixedPurchaseId);

		// 再表示
		Response.Redirect(CreateFixedPurchaseCompleteUrl());
	}

	/// <summary>
	/// Set Visible For Uniform
	/// </summary>
	protected void SetVisibleForUniformOption(string uniformType)
	{
		var isPersonal = false;
		var isCompany = false;
		var isDonate = false;

		if (this.TwFixedPurchaseInvoice != null)
		{
			OrderCommon.CheckUniformType(
				uniformType,
				ref isPersonal,
				ref isDonate,
				ref isCompany);
		}

		this.IsCompany = isCompany;
		this.IsDonate = isDonate;
		this.IsPersonal = isPersonal;
	}

	/// <summary>
	/// Set Visible For Carry Type Option
	/// </summary>
	public void SetVisibleForCarryTypeOption(string carryType)
	{
		// Check Carry Type
		var isMobile = false;
		var isCertificate = false;
		var isNoCarryType = false;

		OrderCommon.CheckCarryType(
			carryType,
			ref isMobile,
			ref isCertificate,
			ref isNoCarryType);

		this.IsMobile = isMobile;
		this.IsCertificate = isCertificate;
		this.IsNoCarryType = isNoCarryType;
	}

	/// <summary>
	/// 領収書希望がクリックされた場合
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblReceiptFlg_SelectedIndexChanged(object sender, EventArgs e)
	{
		tbReceiptAddress.Enabled = tbReceiptProviso.Enabled = (rblReceiptFlg.SelectedValue == Constants.FLG_ORDER_RECEIPT_FLG_ON);
		if (rblReceiptFlg.SelectedValue == Constants.FLG_ORDER_RECEIPT_FLG_OFF) tbReceiptAddress.Text = tbReceiptProviso.Text = string.Empty;
	}


	/// <summary>
	/// 注文拡張項目 更新ボタンイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateOrderExtend_OnClick(object sender, EventArgs e)
	{
		tbByOrderExtendErrorMessages.Visible = false;
		lbOrderExtendErrorMessages.Text = string.Empty;

		var inputDictionary = CreateOrderExtendFromInputData(rOrderExtendInput);
		var input = new OrderExtendInput(inputDictionary);
		var errorMessage = input.Validate();
		if (string.IsNullOrEmpty(errorMessage))
		{
			new FixedPurchaseService().UpdateOrderExtend(
				this.FixedPurchaseContainer.FixedPurchaseId,
				this.LoginOperatorName,
				inputDictionary,
				UpdateHistoryAction.Insert);

			Response.Redirect(CreateFixedPurchaseCompleteUrl());
		}
		else
		{
			tbByOrderExtendErrorMessages.Visible = true;
			lbOrderExtendErrorMessages.Text = errorMessage;
		}
	}

	/// <summary>
	/// 配送先情報取得
	/// </summary>
	/// <param name="fixedPurchase">定期台帳情報</param>
	/// <param name="totalLeadTime">配送リードタイム</param>
	/// <param name="nextScheduledShippingDate">次回出荷予定日</param>
	/// <param name="nextNextScheduledShippingDate">次々回出荷予定日</param>
	private void GetShippingInfo(FixedPurchaseModel fixedPurchase, out int totalLeadTime, out DateTime? nextScheduledShippingDate, out DateTime? nextNextScheduledShippingDate)
	{
		var shipping = fixedPurchase.Shippings[0];
		this.IsLeadTimeFlgOff = OrderCommon.IsLeadTimeFlgOff(shipping.DeliveryCompanyId);
		totalLeadTime = OrderCommon.GetTotalLeadTime(
			fixedPurchase.ShopId,
			shipping.DeliveryCompanyId,
			shipping.ShippingAddr1,
			shipping.ShippingZip);
		nextScheduledShippingDate = OrderCommon.CalculateScheduledShippingDateBasedOnToday(
			fixedPurchase.ShopId,
			fixedPurchase.NextShippingDate,
			string.Empty,
			shipping.DeliveryCompanyId,
			shipping.ShippingCountryIsoCode,
			shipping.ShippingAddr1,
			shipping.ShippingZip,
			false);
		nextNextScheduledShippingDate = OrderCommon.CalculateScheduledShippingDateBasedOnToday(
			fixedPurchase.ShopId,
			fixedPurchase.NextNextShippingDate,
			string.Empty,
			shipping.DeliveryCompanyId,
			shipping.ShippingCountryIsoCode,
			shipping.ShippingAddr1,
			shipping.ShippingZip,
			false);
	}

	/// <summary>
	/// Set use point for purchase
	/// </summary>
	public void SetUsePointForPurchase()
	{
		var cartObject = CreateSimpleCartListForFixedPurchase(
				this.FixedPurchaseContainer.UserId,
				this.FixedPurchaseContainer.FixedPurchaseId)
			.Items
			.FirstOrDefault();

		if (cartObject != null)
		{
			this.PurchasePriceTotal = cartObject.PurchasePriceTotal;
			this.CanUsePointForPurchase = cartObject.CanUsePointForPurchase;
		}
	}

	/// <summary>
	/// Get price can purchase use point
	/// </summary>
	/// <returns>Price can purchase use point</returns>
	protected string GetPriceCanPurchaseUsePoint()
	{
		var result = GetPointMinimumPurchasePriceErrorMessage(this.FixedPurchaseContainer.DispCurrencyCode);
		return result;
	}

	#region UpdateFixedPurchaseItem or Cancel FixedPurchase

	/// <summary>
	/// Update next fixed purchase items or cancel fixed purchase
	/// </summary>
	/// <param name="fixedPurchase">定期台帳</param>
	/// <param name="memberRankId">会員ランクID</param>
	/// <param name="userName">ユーザー名</param>
	/// <param name="updateHistoryAction">履歴更新アクション</param>
	public void UpdateOrCancelFixedPurchaseItemsSubscriptionBox(FixedPurchaseContainer fixedPurchase,
		string memberRankId,
		string userName,
		UpdateHistoryAction updateHistoryAction)
	{
		if (string.IsNullOrEmpty(fixedPurchase.SubscriptionBoxCourseId)) return;
		var subscriptionBoxOrderCount = fixedPurchase.SubscriptionBoxOrderCount + 1;
		var fixedPurchaseService = new FixedPurchaseService();
		var resultFixedPurchase = new SubscriptionBoxService().GetFixedPurchaseNextProduct(
			fixedPurchase.SubscriptionBoxCourseId,
			fixedPurchase.FixedPurchaseId,
			memberRankId,
			fixedPurchase.NextNextShippingDate.Value,
			subscriptionBoxOrderCount,
			FixedPurchaseContainer.Shippings[0]);
		fixedPurchaseService.UpdateNextDeliveryForSubscriptionBox(
			fixedPurchase.FixedPurchaseId,
			Constants.FLG_LASTCHANGED_BATCH,
			Constants.W2MP_POINT_OPTION_ENABLED,
			resultFixedPurchase,
			updateHistoryAction);

		if (updateHistoryAction == UpdateHistoryAction.DoNotInsert) return;
		var updateHistoryService = new UpdateHistoryService();
		updateHistoryService.InsertForFixedPurchase(fixedPurchase.FixedPurchaseId, userName);
		updateHistoryService.InsertForUser(fixedPurchase.UserId, userName);
	}
	#endregion

	#region GetNextFixedPurchaseItems in hapukai
	/// <summary>
	/// Convert list carProducts to PurchaseItems
	/// </summary>
	/// <param name="cartProducts">カート商品リスト</param>
	/// <returns>定期購入アイテム入力リスト</returns>
	public FixedPurchaseItemInput[] ConvertCartProductToFixedPurchaseItems(
		List<CartProduct> cartProducts)
	{
		int itemNo = 1;
		var results = new List<FixedPurchaseItemInput>();
		foreach (var productItem in cartProducts)
		{
			var item = new FixedPurchaseItemInput()
			{
				FixedPurchaseId = this.FixedPurchaseContainer.FixedPurchaseId,
				FixedPurchaseItemNo = itemNo.ToString(),
				FixedPurchaseShippingNo = this.FixedPurchaseContainer.Shippings[0].FixedPurchaseShippingNo.ToString(),
				ShopId = this.FixedPurchaseContainer.ShopId,
				ProductId = productItem.ProductId,
				VariationId = productItem.VariationId,
				ItemQuantity = productItem.Count.ToString(),
				ItemQuantitySingle = productItem.CountSingle.ToString(),
				ProductOptionTexts = string.Empty,
				Price = productItem.Price,
			};

			// 商品情報が存在する?
			var product = ProductCommon.GetProductVariationInfo(item.ShopId, item.ProductId, item.VariationId, this.FixedPurchaseContainer.MemberRankId);
			if (product.Count != 0)
			{
				// 商品情報をセット
				item.SupplierId = (string)product[0][Constants.FIELD_PRODUCT_SUPPLIER_ID];
				item.Name = (string)product[0][Constants.FIELD_PRODUCT_NAME];
				item.VariationName1 = (string)product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1];
				item.VariationName2 = (string)product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2];
				item.VariationName3 = (string)product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3];
				item.Price = (decimal)product[0][Constants.FIELD_PRODUCTVARIATION_PRICE];
				item.SpecialPrice = product[0][Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE] != DBNull.Value ? (decimal?)product[0][Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE] : null;
				item.MemberRankPrice = product[0][Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE_VARIATION] != DBNull.Value ? (decimal?)product[0][Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE_VARIATION] : null;
				item.FixedPurchasePrice = product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE] != DBNull.Value ? (decimal?)product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE] : null;
				item.ShippingType = (string)product[0][Constants.FIELD_PRODUCT_SHIPPING_TYPE];
				item.FixedPurchaseFlg = (string)product[0][Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG];
			}
			itemNo++;
			results.Add(item);
		}
		return results.ToArray();
	}
	#endregion

	#region プロパティ
	/// <summary>配送先住所が日本か</summary>
	protected bool IsShippingAddrJp
	{
		get { return IsCountryJp(this.ShippingAddrCountryIsoCode); }
	}
	/// <summary>頒布会注文か</summary>
	protected bool IsSubscriptionBox
	{
		get
		{
			return Constants.SUBSCRIPTION_BOX_OPTION_ENABLED
				&& (string.IsNullOrEmpty(this.FixedPurchaseContainer.SubscriptionBoxCourseId) == false);
		}
	}
	/// <summary>配送先住所の国ISOコード</summary>
	public string ShippingAddrCountryIsoCode
	{
		get { return (string)this.ViewState["ShippingAddrCountryIsoCode"]; }
		set { this.ViewState["ShippingAddrCountryIsoCode"] = value; }
	}
	/// <summary>定期再開予定日</summary>
	protected DateTime? ResumeDate
	{
		get
		{
			DateTime? resumeDate = null;
			if (Validator.IsDate(ucFixedPurchaseResumetDate.StartDateTimeString))
			{
				resumeDate = DateTime.Parse(ucFixedPurchaseResumetDate.StartDateTimeString);
			}

			return resumeDate;
		}
	}
	/// <summary>ユーザー情報が配送情報更新対象として指定されるか</summary>
	protected bool DoUpdateUser
	{
		get
		{
			return this.AddressUpdatePattern.Contains(Constants.ADDRESS_UPDATE_PATTERN_USER_TOO);
		}
	}
	/// <summary>他の定期情報情報が配送情報更新対象として指定されるか</summary>
	protected bool DoUpdateOtherFixedPurchaseInfo
	{
		get
		{
			return this.AddressUpdatePattern.Contains(Constants.ADDRESS_UPDATE_PATTERN_OTHER_FIXED_PURCASES_TOO);
		}
	}
	/// <summary>配送情報反映反映対象は表示可能か</summary>
	protected bool CanDisplayAddressUpdatePattern
	{
		get
		{
			return ((this.RequestActionKbn == ACTION_KBN_SHIPPING)
				&& (this.AddressUpdatePattern != null)
				&& (this.AddressUpdatePattern.Length > 0));
		}
	}
	/// <summary>更新対象のユーザー情報があるか</summary>
	protected bool HasUpdateUser
	{
		get { return this.WrUserListForAddressUpdate.Items.Count > 0; }
	}
	/// <summary>更新対象の定期情報があるか</summary>
	protected bool HasUpdateFixedPurchase
	{
		get { return this.WrFixedPurchaseListForAddressUpdate.Items.Count > 0; }
	}
	/// <summary>定期購入解約可能回数</summary>
	protected int FixedPurchaseCancelableCount { get; set; }
	/// <summary>定期購入解約可能かどうか</summary>
	protected bool IsCancelable
	{
		get { return (this.FixedPurchaseContainer.ShippedCount >= this.FixedPurchaseCancelableCount); }
	}
	/// <summary>更新した対象のリスト(表示用)</summary>
	protected IEnumerable<IUpdated> UpdatedTargetsForDisplay { get; set; }
	/// <summary>定期購入キャンセル期限</summary>
	public string FixedPurchaseCancelDeadline
	{
		get
		{
			var deadline = (this.ShopShipping != null)
				? this.ShopShipping.FixedPurchaseCancelDeadline.ToString()
				: string.Empty;
			return deadline;
		}
	}
	/// <summary>継続課金解約に関するメッセージ</summary>
	protected string PaymentContinousCancelationMessage
	{
		get { return (string)Session["PaymentContinuousCancelMessage"] ?? string.Empty; }
		set { Session["PaymentContinuousCancelMessage"] = value; }
	}
	/// <summary>ポイントをリセットした時のメッセージ</summary>
	protected string PointResetMessages
	{
		get { return (string)Session["PointResetMessages"] ?? string.Empty; }
		set { Session["PointResetMessages"] = value; }
	}
	/// <summary>リードタイムフラグがOffか</summary>
	protected bool IsLeadTimeFlgOff { get; set; }
	/// <summary>再開不可のPaypay決済か</summary>
	public bool IsInvalidResumePaypay
	{
		get
		{
			return ((this.FixedPurchaseContainer.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
				&& this.FixedPurchaseContainer.IsCancelFixedPurchaseStatus)
				&& ((Constants.PAYMENT_PAYPAY_KBN == Constants.PaymentPayPayKbn.GMO)
					|| (Constants.PAYMENT_PAYPAY_KBN == Constants.PaymentPayPayKbn.VeriTrans));
		}
	}
	/// <summary>Purchase price total</summary>
	public decimal PurchasePriceTotal { get; set; }
	/// <summary>Can use point for purchase</summary>
	public bool CanUsePointForPurchase { get; set; }
	/// <summary>ユーザー非退会判定</summary>
	protected bool CanResumeFixedPurchase { get; set; }
	/// <summary>定期台帳は定期台帳同梱により解約となったか</summary>
	protected bool IsCancelledByFixedPurchaseCombine
	{
		get
		{
			return this.FixedPurchaseContainer.IsCancelFixedPurchaseStatus
				&& (this.FixedPurchaseContainer.ExtendStatus36 == Constants.FLG_ORDER_ORDER_EXTEND_STATUS_ON);
		}
	}
	/// <summary>PayTgクレジットトークン</summary>
	protected string CreditTokenbyPayTg
	{
		get { return (string)this.Session[PayTgConstants.PARAM_TOKEN]; }
		set { this.Session[PayTgConstants.PARAM_TOKEN] = value; }
	}
	#endregion
}
