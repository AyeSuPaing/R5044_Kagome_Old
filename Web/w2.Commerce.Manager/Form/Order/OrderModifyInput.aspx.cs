/*
=========================================================================================================
  Module      : 注文情報編集ページ処理(OrderModifyInput.aspx.cs)
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
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using w2.App.Common;
using w2.App.Common.DataCacheController;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Global.Config;
using w2.App.Common.Input.Order;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.ECPay;
using w2.App.Common.Order.Payment.NPAfterPay;
using w2.App.Common.Order.Payment.Paygent;
using w2.App.Common.Order.Payment.PayPal;
using w2.App.Common.Order.Payment.PayTg;
using w2.App.Common.Order.Payment.TriLinkAfterPay.Helper;
using w2.App.Common.Order.Payment.Veritrans;
using w2.App.Common.Order.Reauth;
using w2.App.Common.Order.UserCreditCardCooperationInfos;
using w2.App.Common.OrderExtend;
using w2.App.Common.Product;
using w2.App.Common.Util;
using w2.App.Common.Web.WebCustomControl;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Web;
using w2.Domain;
using w2.Domain.AdvCode;
using w2.Domain.CountryLocation;
using w2.Domain.Coupon;
using w2.Domain.Coupon.Helper;
using w2.Domain.DeliveryCompany;
using w2.Domain.FixedPurchase;
using w2.Domain.Order;
using w2.Domain.Payment;
using w2.Domain.Point;
using w2.Domain.Point.Helper;
using w2.Domain.Product;
using w2.Domain.RealShop;
using w2.Domain.ShopShipping;
using w2.Domain.TwOrderInvoice;
using w2.Domain.TwUserInvoice;
using w2.Domain.User;
using w2.Domain.UserCreditCard;
using w2.Domain.UserIntegration;
using w2.Domain.UserManagementLevel;
using w2.Domain.UserShipping;

public partial class Form_Order_OrderModifyInput : OrderPage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		this.HasErrorOnPostback = false;

		if (!IsPostBack)
		{
			// 処理区分チェック
			CheckActionStatus();

			// 各プロパティセット
			SetProperty();

			// コンポーネント初期化
			InitializeComponents();

			// コンポーネントに値セット
			SetValueToComponents();

			// Get real shop list for selection
			var realShops = new RealShopService().GetAll();
			if (realShops == null)
			{
				this.RealShopModels = new RealShopModel[0];
			}
			else
			{
				this.RealShopModels = realShops.Where(rs => rs.ValidFlg == Constants.FLG_ON).ToArray();
			}

			// データバインド
			DataBind();

			// 配送先グローバル関連項目設定
			SetOrderShippingGlobalColumn();

			// 決済種別イベント実行
			ddlOrderPaymentKbn_SelectedIndexChanged(sender, e);

			if (Constants.ORDER_EXTEND_OPTION_ENABLED)
			{
				var model = this.OrderInput.CreateModel();
				var orderExtend = OrderExtendCommon.CreateOrderExtendForManager(model);
				var input = new OrderExtendInput(orderExtend);
				rOrderExtendInput.DataSource = input.OrderExtendItems;
				rOrderExtendInput.DataBind();
				SetOrderExtendFromUserExtendObject(rOrderExtendInput, input);
			}

			SetScheduleShipingAndDateShiping();

			if (PaygentUtility.CheckIsPaidyPaygentPayment(this.OrderInputOld.OrderPaymentKbn)
				&& ((this.OrderInputOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_EXPIRED)
					|| (this.OrderInputOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_CANCEL_EXPIRED)))
			{
				ddlNewExecuteType.SelectedValue = ReauthCreatorFacade.ExecuteTypes.None.ToString();
				lbExternalPaymentAlertMessage.Text = WebMessages.GetMessages(
					WebMessages.ERRMSG_MANAGER_PAYMENT_PAIDY_PAYGENT_CANNOT_LINKING_BY_EXTERNAL_PAYMENT_STATUS,
					ValueText.GetValueText(
						Constants.TABLE_ORDER,
						Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS,
						this.OrderInputOld.ExternalPaymentStatus));
				ddlNewExecuteType.Enabled = false;
			}

			if (this.IsUserPayTg
				&& (Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.VeriTrans))
			{
				// PayTg端末状態取得
				GetPayTgDeviceStatus();
			}

			// 画面内容設定終わったら、確認画面から戻ってきたフラグをクリアする
			Session[Constants.SESSION_KEY_PARAM_FOR_BACK2] = null;
		}
		else
		{
			SwitchDisplayForCreditTokenInput();

			TwInvoiceCarryTypeOptionDateBind();

			SetValueToComponentsSettlementAmountByTwConvenienceStore();
		}

		// 支払方法制限メッセージ表示
		DisplayLimitedPaymentMessages();
	}

	/// <summary>
	/// 電子発票 キャリータイプオプションへのデータバインド
	/// </summary>
	private void TwInvoiceCarryTypeOptionDateBind()
	{

		if (OrderCommon.DisplayTwInvoiceInfo())
		{
			// Set visible for invoice type
			SetVisibleForUniformOption(ddlUniformInvoice.SelectedValue);

			if (string.IsNullOrEmpty(ddlUniformInvoiceOrCarryTypeOption.SelectedValue))
			{
				// Refresh Uniform Invoice Or Carry Type Option
				ddlUniformInvoiceOrCarryTypeOption.DataSource = GetUniformInvoiceOrCarryTypeOption(ddlUniformInvoice.SelectedValue, ddlCarryType.SelectedValue);
				ddlUniformInvoiceOrCarryTypeOption.DataBind();
			}
		}
	}

	/// <summary>
	/// コンポーネントに値をセット 台湾コンビニ受け取り
	/// </summary>
	private void SetValueToComponentsSettlementAmountByTwConvenienceStore()
	{
		trSettlementAmount.Visible = (this.IsShippingConvenience
			&& (ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE));

		lSettlementAmount.Text = WebSanitizer.HtmlEncode(CreateSettlementAmount(
			this.OrderInput.OrderPaymentKbn,
			decimal.Parse(this.OrderInput.OrderPriceTotal),
			trSettlementAmount.Visible));
	}

	/// <summary>
	/// 再計算ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnReCalculate_Click(object sender, EventArgs e)
	{
		// 注文入力情報取得
		var order = GetOrderInput();
		// 注文ポイント入力情報
		var orderPoints = GetOrderPointsInput();

		// 商品入力チェック
		int index = 0;
		foreach (var orderShipping in order.Shippings)
		{
			var itemErrorMessage = string.Empty;
			if (sender is RepeaterItem)
			{
				itemErrorMessage = CheckOrderItemRegistable(this.OrderInput, this.OrderInputOld, orderShipping.Items, (RepeaterItem)sender, true);
			}
			else
			{
				itemErrorMessage = CheckOrderItem(this.OrderInput, this.OrderInputOld, orderShipping.Items, true);
			}

			// エラーメッセージセット
			this.OrderItemErrorMessages.Append(itemErrorMessage);
			rShippingList.Items[index].FindControl("trOrderItemErrorMessagesTitle").Visible = (itemErrorMessage.Length != 0);
			rShippingList.Items[index].FindControl("trOrderItemErrorMessages").Visible = (itemErrorMessage.Length != 0);
			((Label)rShippingList.Items[index].FindControl("lbOrderItemErrorMessages")).Text = itemErrorMessage.ToString();

			index++;
		}

		// 頒布会商品・金額チェック
		CheckSubscriptionBoxItemsAndPricesInput(order);

		// 決済入力チェック
		if (ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
		{
			CheckOrderCreditCardInput(GetOrderCreditCardInputForOrderPage(order.OrderPaymentKbn, order.UserId));
		}

		// 金額部分入力チェック
		CheckOrderPriceInput(order, orderPoints);

		// ポイント＆クーポン有効性チェック
		CheckPointAndCouponValidity(order, orderPoints);

		// 自動計算適用
		if (this.HasError == false)
		{
			ApplyAutoCalculation(order, orderPoints);
		}

		// 入力エラーなしであれば値チェック＆セット
		if (this.HasError == false)
		{
			StringBuilder targetErrorMessage = null;
			if (sender == btnReCalculatePoint)
			{
				targetErrorMessage = this.OrderPointErrorMessages;
			}
			else if (sender == btnReCalculateCoupon)
			{
				targetErrorMessage = this.OrderCouponErrorMessages;
			}
			else
			{
				targetErrorMessage = this.OrderPriceErrorMessages;
			}

			// 金額再計算
			CalculatePrice(order, targetErrorMessage);
		}

		// 正常の場合はデータバインド
		// ※入力チェック + 各有効性チェックがエラーではない場合のみ
		if (this.HasError == false)
		{
			// 暫定対応：別のバグがあって、自動計算適用の場合は、付与するポイントを合算して、orderPoints[0]に入れるので、処理を分ける
			var allPoints = orderPoints.Sum(op => decimal.Parse((string)op[Constants.FIELD_ORDER_ORDER_POINT_ADD]));
			order.OrderPointAdd = cbApplyAutoCalculation.Checked
				? (string)orderPoints[0][Constants.FIELD_ORDER_ORDER_POINT_ADD]
				: StringUtility.ToNumeric(allPoints);

			// 注文情報更新
			this.OrderInput.Update(order);

			var basePointComp = (this.IsNoPointPublished == Constants.FLG_USERPOINT_POINT_NOT_PUBLISHED)
				? orderPoints
					.Where(
						op => ((string)op[Constants.FIELD_USERPOINT_POINT_TYPE] == Constants.FLG_USERPOINT_POINT_TYPE_COMP))
					.Sum(op => decimal.Parse((string)op[Constants.FIELD_ORDER_ORDER_POINT_ADD]))
				: orderPoints
					.Where(
						op => ((string)op[Constants.FIELD_USERPOINT_POINT_TYPE] == Constants.FLG_USERPOINT_POINT_TYPE_COMP)
							&& (((string)op[Constants.FIELD_USERPOINT_POINT_KBN] == Constants.FLG_USERPOINT_POINT_KBN_BASE)))
					.Sum(op => decimal.Parse((string)op[Constants.FIELD_ORDER_ORDER_POINT_ADD]));
			tbOrderPointAddBaseComp.Text = StringUtility.ToNumeric(basePointComp);

			// 付与ポイント情報更新
			if ((this.UserPointRelatedThisOrder != null)
				&& (this.UserPointRelatedThisOrder.Items.Count > 0))
			{
				foreach (var orderPoint in orderPoints)
				{
					if (((string)orderPoint[Constants.FIELD_USERPOINT_POINT_TYPE] == Constants.FLG_USERPOINT_POINT_TYPE_COMP)
						&& ((string)orderPoint[Constants.FIELD_USERPOINT_POINT_KBN] == Constants.FLG_USERPOINT_POINT_KBN_BASE))
					{
						continue;
					}

					var userPoint = this.UserPointRelatedThisOrder.Items.FirstOrDefault(
						up => up.PointKbnNo == int.Parse((string)orderPoint[Constants.FIELD_USERPOINT_POINT_KBN_NO]));
					if (userPoint != null)
					{
						userPoint.Point = decimal.Parse((string)orderPoint[Constants.FIELD_ORDER_ORDER_POINT_ADD]);
					}
				}
			}

			// データバインド
			rShippingList.DataBind();
			dvRecalculateArea.DataBind();
			divPoint.DataBind();
			divCoupon.DataBind();

			// 配送先グローバル関連項目設定
			SetOrderShippingGlobalColumn();
		}

		// エラーメッセージ表示
		DisplayErrorMessages();

		this.HasErrorOnPostback = this.HasError;

		// 支払方法制限メッセージ表示
		DisplayLimitedPaymentMessages();
		SetScheduleShipingAndDateShiping();
	}

	/// <summary>
	/// 確認するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnConfirm_Click(object sender, EventArgs e)
	{
		var hasError = false;
		Session[Constants.SESSION_KEY_PARAM_FOR_BACK2] = 1;

		this.IsNeedShowConfirm = (this.OrderInput.MallId != Constants.FLG_USER_MALL_ID_OWN_SITE)
			|| (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2);

		// 注文入力情報取得
		var order = GetOrderInput();

		if (this.IsUpdateShippingConvenience)
		{
			order.OrderPaymentKbn = this.OrderInput.OrderPaymentKbn;
		}

		// 注文者情報の全角半角補正
		CorrectOrderOwnerWithFullHalfWidth(order);

		// 配送先情報の全角半角補正
		CorrectOrderShippingWithFullHalfWidth(order);

		// Taiwan Order Invoice
		var twOrderInvoice = GetTwOrderInvoiceInput();

		if (Constants.GLOBAL_OPTION_ENABLE && (this.IsUpdateShippingConvenience == false))
		{
			// 台湾後払いが利用可能か判定する
			var errorMessage = CheckUsedPaymentOrderModifyByShippingCountry(
				order.OrderPaymentKbn,
				order.Shippings[0].ShippingCountryIsoCode,
				order.Owner.OwnerAddrCountryIsoCode);
			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				DisplayOrderPaymentErrorMessage(errorMessage);
				this.HasErrorOnPostback = true;
				return;
			}
		}

		// PayPalのアカウントチェック
		if ((ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
			&& (this.OrderOld.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL))
		{
			if (string.IsNullOrEmpty(PayPalUtility.Account.GetCooperateAccountEmail(this.User.UserId)))
			{
				lbPaymentAlertMessage.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PAYPAL_ISNOT_LINK_ACCOUNT);
				ddlOrderPaymentKbn.Focus();
				return;
			}
		}


		if ((ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE)
			&& (order.Shippings[0].ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF))
		{
			ddlOrderPaymentKbn.SelectedValue = string.Empty;
			ddlOrderPaymentKbn.Focus();

			return;
		}

		if ((ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_DSK_DEF)
			&& (this.OrderOld.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_DSK_DEF))
		{
			order.PaymentOrderId = (order.OrderId.Length >= 10)
				? order.OrderId.Substring(order.OrderId.Length - 10)
				: order.OrderId.PadRight(10, '0');
		}

		// 決済方法再設定
		// クレジットカード情報の入力チェック
		var orderCreditCard = GetOrderCreditCardInputForOrderPage(order.OrderPaymentKbn, this.User.UserId);
		if (this.IsUserPayTg)
		{
			orderCreditCard.CreditToken = Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans
				? CartPayment.CreditTokenInfoBase.CreateCreditTokenInfo(" " + this.VeriTransAccountId)
				: CartPayment.CreditTokenInfoBase.CreateCreditTokenInfo(this.CreditTokenbyPayTg);
			if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten
				&& (ddlUserCreditCard.SelectedValue == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW))
			{
				orderCreditCard.ExpireMonth = ddlCreditExpireMonth.SelectedValue;
				orderCreditCard.ExpireYear = ddlCreditExpireYear.SelectedValue;
				orderCreditCard.CompanyCode = this.CreditCardCompanyCodebyPayTg;
			}
		}

		if (order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
		{
			if (CheckOrderCreditCardInput(orderCreditCard))
			{
				order.CardInstallmentsCode = orderCreditCard.InstallmentsCode;
				order.CardInstruments = orderCreditCard.InstallmentsName;

				// トークンが取得できていないときはエラーとして扱う(バグ#3554対策)
				if (orderCreditCard.CreditBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
				{
					if (OrderCommon.CreditTokenUse && (orderCreditCard.CreditToken == null))
					{
						spanErrorMessageForCreditCard.InnerHtml = WebSanitizer.HtmlEncodeChangeToBr(
							WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CARDAUTH_ERROR));
						spanErrorMessageForCreditCard.Style["display"] = "block";
						this.HasErrorOnPostback = true;
						return;
					}
				}
			}
		}
		else
		{
			order.CardInstallmentsCode = "";
			order.CardInstruments = "";
		}

		if (this.IsUpdateShippingConvenience == false)
		{
			// クレジットカード売上確定後キャンセル可能かチェック
			if (this.OrderPaymentErrorMessages.Length == 0)
			{
				CheckCancelableForCreditCardSalesCompleteOrder();
			}

			// 注文情報入力チェック
			this.OrderErrorMessages.Append(order.ValidateForOrder());
			if (string.IsNullOrEmpty(this.OrderErrorMessages.ToString()))
			{
				// 注文アフィリエイト情報入力チェック
				this.OrderAffiliateErrorMessages.Append(order.ValidateForOrderAffiliate());

				// 広告コード存在チェック
				this.OrderAffiliateErrorMessages.Append(CheckAdvCode(tbAdvCodeFirst.Text.Trim()));
				this.OrderAffiliateErrorMessages.Append(CheckAdvCode(tbAdvCodeNew.Text.Trim()));
			}
			else
			{
				this.IsNeedShowConfirm = false;
			}

			// 注文者情報入力チェック
			var orderOwnerErrorMessageList = order.Owner.Validate(order.OrderPaymentKbn);
			this.OrderOwnerErrorMessages.Append(Validator.ChangeToDisplay(orderOwnerErrorMessageList));
			if (((this.OrderInput.MallId == Constants.FLG_USER_MALL_ID_OWN_SITE)
				 || (orderOwnerErrorMessageList.All(x => this.OmitCheckForOrderOwner.Contains(x.Key)) == false))
				&& (this.OrderInput.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2))
			{
				this.IsNeedShowConfirm = false;
			}

			// Check owner tel no and owner name for EcPay
			var shippingReceivingStoreFlg = order.Shippings[0].ShippingReceivingStoreFlg;
			if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED
				&& Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED
				&& (shippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON))
			{
				var ownerOwner = order.Owner;
				var ownerTelNo = (string.IsNullOrEmpty(ownerOwner.OwnerTel1) == false)
					? (string)ownerOwner.OwnerTel1
					: string.Format(
						"{0}{1}{2}",
						ownerOwner.OwnerTel1_1,
						ownerOwner.OwnerTel1_2,
						ownerOwner.OwnerTel1_3);

				if (OrderCommon.CheckValidTelNoTaiwanForEcPay(ownerTelNo) == false)
				{
					this.OrderOwnerErrorMessages.Append(
						CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_TEL_NO_NOT_TAIWAN));
				}

				var ownerName = string.Format(
					"{0}{1}",
					DataInputUtility.ConvertToFullWidthBySetting(ownerOwner.OwnerName1),
					DataInputUtility.ConvertToFullWidthBySetting(ownerOwner.OwnerName2));
				if (OrderCommon.CheckOwnerNameForEcPay(ownerName) == false)
				{
					this.OrderOwnerErrorMessages.Append(
						CommerceMessages.GetMessages(
							CommerceMessages.ERRMSG_FRONT_OWNER_NAME_OF_CONVENIENCE_STORE_INVALID));
				}
			}
		}

		int index = 0;
		foreach (var orderShipping in order.Shippings)
		{
			// 注文配送先情報入力チェック
			var orderShippingErrorMessageList = orderShipping.Validate(
				this.OrderInput.GiftFlg,
				order.OrderPaymentKbn,
				(orderShipping.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON));
			var orderShippingErrorMessages = Validator.ChangeToDisplay(orderShippingErrorMessageList);
			this.OrderShippingErrorMessages.Append(orderShippingErrorMessages);
			if (((this.OrderInput.MallId == Constants.FLG_USER_MALL_ID_OWN_SITE)
				&& (orderShippingErrorMessageList.All(x => this.OmitCheckForOrderShipping.Contains(x.Key)) == false)))
			{
				this.IsNeedShowConfirm = false;
			}

			// 配送先情報エラーメッセージ表示
			rShippingList.Items[index].FindControl("trOrderShippingErrorMessagesTitle").Visible = (orderShippingErrorMessages.Length != 0);
			rShippingList.Items[index].FindControl("trOrderShippingErrorMessages").Visible = (orderShippingErrorMessages.Length != 0);
			((Label)rShippingList.Items[index].FindControl("lbOrderShippingErrorMessages")).Text = orderShippingErrorMessages;

			// 注文商品情報入力チェック
			var orderItemErrorMessages = string.Empty;
			if (this.IsUpdateShippingConvenience == false)
			{
				orderItemErrorMessages = CheckOrderItem(this.OrderInput, this.OrderInputOld, orderShipping.Items, true);
				this.OrderItemErrorMessages.Append(orderItemErrorMessages);
			}

			// 注文商品情報エラーメッセージ表示
			rShippingList.Items[index].FindControl("trOrderItemErrorMessagesTitle").Visible = (orderItemErrorMessages.Length != 0);
			rShippingList.Items[index].FindControl("trOrderItemErrorMessages").Visible = (orderItemErrorMessages.Length != 0);
			((Label)rShippingList.Items[index].FindControl("lbOrderItemErrorMessages")).Text = orderItemErrorMessages;

			index++;
		}

		// 配送種別情報チェック
		if (this.ShopShipping == null)
		{
			this.OrderErrorMessages.Append(
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_SHOP_SHIPPING_DELETED_ERROR));
			this.IsNeedShowConfirm = false;
		}

		// 請求書同梱フラグ判定&更新
		order.InvoiceBundleFlg = OrderCommon.IsInvoiceBundleServiceUsable(order.OrderPaymentKbn)
			? OrderCommon.JudgmentInvoiceBundleFlg(order.CreateModel())
			: Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF;

		// 注文ポイント入力情報
		var orderPoints = GetOrderPointsInput();

		if (this.IsUpdateShippingConvenience == false)
		{
			// 金額部分入力チェック
			CheckOrderPriceInput(order, orderPoints);

			// ポイント＆クーポン有効性チェック
			CheckPointAndCouponValidity(order, orderPoints);

			// 領収書情報入力チェック
			CheckReceiptInput(order);
		}

		if (this.IsUpdateShippingConvenience == false)
		{
			// 自動計算適用
			if (((this.HasError == false) || this.IsNeedShowConfirm) && (this.HasErrorForMallOrder == false))
			{
				ApplyAutoCalculation(order, orderPoints);
			}

			// 金額再計算
			if (((this.HasError == false) || this.IsNeedShowConfirm) && (this.HasErrorForMallOrder == false))
			{
				CalculatePrice(order, this.OrderPriceErrorMessages);
			}
		}

		// 注文情報更新
		try
		{
			// エラーがあれば例外を飛ばす
			if ((this.HasError && (this.IsNeedShowConfirm == false)) || this.HasErrorForMallOrder)
			{
				// 自サイト受注かつエラーあり、または注文金額・クーポン・ポイント関連エラーの場合
				throw new OrderErrorException();
			}

			// 注文アフィリエイト情報、また領収書情報に入力エラーがある場合、例外を飛ばす
			if ((this.OrderAffiliateErrorMessages.Length != 0)
				|| (this.OrderReceiptErrorMessages.Length != 0))
			{
				throw new OrderErrorException();
			}

			// ユーザポイント更新
			var userPointList = new UserPointList(new UserPointModel[] { });
			if (Constants.W2MP_POINT_OPTION_ENABLED && this.IsUser)
			{
				decimal orderPointAdd = 0;
				foreach (var ht in orderPoints)
				{
					var userPoint = new UserPointModel()
					{
						Point = decimal.Parse((string)ht[Constants.FIELD_ORDER_ORDER_POINT_ADD]),
						PointKbn = (string)ht[Constants.FIELD_USERPOINT_POINT_KBN],
						PointIncKbn = (string)ht[Constants.FIELD_USERPOINT_POINT_INC_KBN],
						PointType = (string)ht[Constants.FIELD_USERPOINT_POINT_TYPE],
						PointRuleId = (string)ht[Constants.FIELD_USERPOINT_POINT_RULE_ID],
						PointRuleKbn = (string)ht[Constants.FIELD_USERPOINT_POINT_RULE_KBN],
					};

					if (string.IsNullOrEmpty((string)ht[Constants.FIELD_USERPOINT_POINT_KBN_NO]) == false)
					{
						userPoint.PointKbnNo = int.Parse((string)ht[Constants.FIELD_USERPOINT_POINT_KBN_NO]);
					}

					if (string.IsNullOrEmpty((string)ht[Constants.FIELD_USERPOINT_POINT_EXP]) == false)
					{
						userPoint.PointExp = DateTime.Parse((string)ht[Constants.FIELD_USERPOINT_POINT_EXP]);
					}

					userPointList.Items.Add(userPoint);
					orderPointAdd += userPoint.Point;
				}
				order.OrderPointAdd = orderPointAdd.ToString();
			}

			// 注文情報更新
			this.OrderInput.Update(order);

			// Ec Pay
			if (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
			{
				var errorMessage = string.Empty;
				var paymentType = "@@ 1 @@";
				switch (this.OrderOld.ExternalPaymentType)
				{
					case Constants.FLG_PAYMENT_TYPE_ECPAY_CVS:
					case Constants.FLG_PAYMENT_TYPE_ECPAY_BARCODE:
					case Constants.FLG_PAYMENT_TYPE_ECPAY_WEBATM:
					case Constants.FLG_PAYMENT_TYPE_ECPAY_ATM:
						if (this.OrderOld.LastBilledAmount != decimal.Parse(this.OrderInput.LastBilledAmount))
						{
							errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ECPAY_TYPE)
								.Replace(paymentType, this.OrderOld.ExternalPaymentType);
						}
						break;

					case Constants.FLG_PAYMENT_TYPE_ECPAY_CREDIT:
						if (this.OrderOld.LastBilledAmount < decimal.Parse(this.OrderInput.LastBilledAmount))
						{
							errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ECPAY_TYPE_CREDIT);
						}
						break;
				}

				if (string.IsNullOrEmpty(errorMessage) == false)
				{
					this.OrderErrorMessages.Append(errorMessage);
					DisplayErrorMessages();
					this.HasErrorOnPostback = true;
					return;
				}
			}

			// NewebPay
			if (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
			{
				var errorMessage = string.Empty;
				var paymentType = "@@ 1 @@";
				switch (this.OrderOld.ExternalPaymentType)
				{
					case Constants.FLG_PAYMENT_TYPE_NEWEBPAY_LINEPAY:
					case Constants.FLG_PAYMENT_TYPE_NEWEBPAY_WEBATM:
					case Constants.FLG_PAYMENT_TYPE_NEWEBPAY_ATM:
					case Constants.FLG_PAYMENT_TYPE_NEWEBPAY_CVS:
					case Constants.FLG_PAYMENT_TYPE_NEWEBPAY_BARCODE:
					case Constants.FLG_PAYMENT_TYPE_NEWEBPAY_CVSCOM:
						if (this.OrderOld.LastBilledAmount != decimal.Parse(this.OrderInput.LastBilledAmount))
						{
							errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NEWEBPAY_TYPE)
								.Replace(paymentType, this.OrderOld.ExternalPaymentType);
						}
						break;

					case Constants.FLG_PAYMENT_TYPE_NEWEBPAY_CREDIT:
						if (this.OrderOld.LastBilledAmount < decimal.Parse(this.OrderInput.LastBilledAmount))
						{
							errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NEWEBPAY_TYPE_CREDIT);
						}
						break;
				}

				if (string.IsNullOrEmpty(errorMessage) == false)
				{
					this.OrderErrorMessages.Append(errorMessage);
					DisplayErrorMessages();
					this.HasErrorOnPostback = true;
					return;
				}
			}

			// PayPay
			if (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
			{
				var errorMessage = string.Empty;
				if (this.OrderOld.LastBilledAmount < decimal.Parse(this.OrderInput.LastBilledAmount))
				{
					errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_RETURNEXCHANGE_LAST_BILLED_AMOUNT)
						.Replace("@@ 1 @@", "PayPay");
				}

				if (string.IsNullOrEmpty(errorMessage) == false)
				{
					this.OrderErrorMessages.Append(errorMessage);
					DisplayErrorMessages();
					this.HasErrorOnPostback = true;
					return;
				}
			}

			if (OrderCommon.CheckPaymentYamatoKaSms(this.OrderInput.OrderPaymentKbn))
			{
				var errorMessage = string.Empty;

				if (this.OrderOld.LastBilledAmount < decimal.Parse(this.OrderInput.LastBilledAmount))
				{
					errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_YAMATOKA_SMS_LAST_BILLED_AMOUNT);
				}

				if (CheckShippingChanged(this.OrderOld.Shippings[0], this.OrderInput.Shippings[0].CreateModel()))
				{
					errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_YAMATOKA_SMS_SHIPPING_CHANGED);
				}

				if (string.IsNullOrEmpty(errorMessage) == false)
				{
					this.OrderErrorMessages.Append(errorMessage);
					DisplayErrorMessages();
					this.HasErrorOnPostback = true;
					return;
				}
			}

			// Boku Payment
			if (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU)
			{
				var errorMessage = string.Empty;

				if ((this.OrderOld.LastBilledAmount != decimal.Parse(this.OrderInput.LastBilledAmount))
					|| (CheckShippingChanged(this.OrderOld.Shippings[0], this.OrderInput.Shippings[0].CreateModel())))
				{
					errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_BOKU_TYPE)
						.Replace("@@ 1 @@", "Boku");
				}

				if (string.IsNullOrEmpty(errorMessage) == false)
				{
					this.OrderErrorMessages.Append(errorMessage);
					DisplayErrorMessages();
					ClientScript.RegisterStartupScript(this.GetType(), "ScrollToTop", "ScrollToTop()", true);
					return;
				}
			}

			if (PaygentUtility.CheckIsPaidyPaygentPayment(this.OrderInputOld.OrderPaymentKbn)
				&& (this.OrderInputOld.LastBilledAmount != this.OrderInput.LastBilledAmount))
			{
				CheckPaidyPaymentPriceChange();
				hasError = this.OrderErrorMessages.Length != 0;
				if (hasError)
				{
					DisplayErrorMessages();
					return;
				}
			}

			// Update Taiwan Order Invoice
			if (this.TwOrderInvoiceInput != null)
			{
				this.TwOrderInvoiceInput.Update(twOrderInvoice);
			}

			if ((this.OrderInputOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
				&& (this.OrderInput.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY))
			{
				this.OrderInput.PaymentOrderId = string.Empty;
			}

			// パラメータセット
			var parameters = new Hashtable
			{
				{ Constants.TABLE_ORDER, this.OrderInput },
				{ Constants.TABLE_ORDER + "_input_old", this.OrderInputOld },
				{ Constants.TABLE_ORDER + "_old", this.OrderOld },
			};

			parameters.Add(Constants.PAYMENT_GMO_CVS_TYPE, ddlGmoCvsType.SelectedValue);


			// Params for TwOrderInvoice
			var paramsforTwOrderInvoice = new Hashtable
			{
				{ Constants.TABLE_TWORDERINVOICE, this.TwOrderInvoiceInput },
			};

			if (OrderCommon.DisplayTwInvoiceInfo())
			{
				switch (this.TwOrderInvoiceInput.TwUniformInvoice)
				{
					case Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL:
						{
							if (this.IsMobile)
							{
								paramsforTwOrderInvoice.Add(
									string.Format("{0}_1", Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE_OPTION),
									tbCarryTypeOption1.Text);
							}
							else if (this.IsCertificate)
							{
								paramsforTwOrderInvoice.Add(
									string.Format("{0}_2", Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE_OPTION),
									tbCarryTypeOption2.Text);
							}
						}
						break;

					case Constants.FLG_TW_UNIFORM_INVOICE_COMPANY:
						{
							if (string.IsNullOrEmpty(this.TwOrderInvoiceInput.TwCarryType))
							{
								paramsforTwOrderInvoice.Add(
									Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE_OPTION1,
									tbCompanyOption1.Text);
								paramsforTwOrderInvoice.Add(
									Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE_OPTION2,
									tbCompanyOption2.Text);
							}
						}
						break;

					case Constants.FLG_TW_UNIFORM_INVOICE_DONATE:
						{
							if (string.IsNullOrEmpty(this.TwOrderInvoiceInput.TwCarryType))
							{
								paramsforTwOrderInvoice.Add(
									string.Format("{0}_donate", Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE_OPTION1),
									tbDonateOption1.Text);
							}
						}
						break;
				}
			}

			if (this.IsUserPayTg
				&& (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans)
				&& (hasError == false))
			{
				var errorMessage = (this.PayTgResponse == null)
					? string.Empty
					: (string)this.PayTgResponse[VeriTransConst.PAYTG_RESPONSE_ERROR] ?? string.Empty;
				if (string.IsNullOrEmpty(errorMessage) == false)
				{
					DisplayOrderPaymentErrorMessage(errorMessage);
					this.HasErrorOnPostback = true;
					return;
				}
			}
			else if (this.IsUserPayTg
				&& (this.IsSuccessfulCardRegistration == false)
				&& (ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
				&& (ddlUserCreditCard.SelectedValue == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
				&& (hasError == false))
			{
				var errorMessage = (this.PayTgResponse == null)
					? string.Empty
					: (string)this.PayTgResponse[PayTgConstants.PAYTG_RESPONSE_ERROR] ?? string.Empty;
				if (string.IsNullOrEmpty(errorMessage) == false)
				{
					DisplayOrderPaymentErrorMessage(errorMessage);
					this.HasErrorOnPostback = true;
					return;
				}
			}

			if (Constants.W2MP_POINT_OPTION_ENABLED)
			{
				parameters.Add(Constants.TABLE_USERPOINT, userPointList);
				parameters.Add(Constants.TABLE_USERPOINT + "_old", this.UserPointList);
				parameters.Add("added_base_point_comp_old", this.AddedBasePointCompOld);
			}
			parameters.Add(Constants.TABLE_ORDER + "_credit_card", orderCreditCard);
			parameters.Add("old_execute_type", ddlOldExecuteType.SelectedValue);
			parameters.Add("new_execute_type", ddlNewExecuteType.SelectedValue);

			Session[Constants.SESSIONPARAM_KEY_ORDER_MODIFY_INFO] = parameters;
			Session[Constants.SESSIONPARAM_KEY_TWORDERINVOICE_MODIFY_INFO] = paramsforTwOrderInvoice;
			Session[Constants.SESSION_KEY_ACTION_STATUS] = this.ActionStatus;

			// 連続投稿防止用
			Session[Constants.SESSION_KEY_ORDER_REGIST_INPUT_ERROR] = ""; // 空にする（この処理を通すことで連続制限にひっかからない）

			var messageConfirm = GetMessageConfirm();
			this.IsNeedShowConfirm = this.IsNeedShowConfirm && (messageConfirm.Length != 0);

			if (OrderCommon.DisplayTwInvoiceInfo(this.OrderInput.Shippings[0].ShippingCountryIsoCode))
			{
				hasError = DisplayUniformInvoiceErrorMessage(
					Validator.Validate("OrderModifyInput", paramsforTwOrderInvoice));
			}

			hfDiscountMessageConfirm.Value = WebSanitizer.HtmlEncode(this.DiscountLimitErrorMessages.ToString());
			// 確認画面へ
			if (this.IsNeedShowConfirm == false)
			{
				if ((decimal.Parse(this.OrderInputOld.OrderPointUse) > decimal.Parse(this.OrderInput.OrderPointUse))
					&& this.UserPointRelatedThisOrder.Items.Count <= 0)
				{
					hfMessageConfirm.Value = WebMessages.GetMessages(
						WebMessages.ERRMSG_FRONT_POINT_RETURN_EXPIRED_ALERT);
					Session[Constants.SESSION_KEY_USERPOINT_EXPIRED_ALEAT_MESSAGE] = WebMessages.GetMessages(
						WebMessages.ERRMSG_FRONT_POINT_RETURN_EXPIRED_ALERT);
				}
				else if (string.IsNullOrEmpty(hfDiscountMessageConfirm.Value)
					&& (hasError == false))
				{
					Response.Redirect(
						Constants.PATH_ROOT
						+ Constants.PAGE_MANAGER_ORDER_MODIFY_CONFIRM
						+ "?"
						+ Constants.REQUEST_KEY_ACTION_STATUS
						+ "="
						+ this.ActionStatus);
				}
			}
			else
			{
				// エラーメッセージ表示
				DisplayErrorMessages();

				this.HasErrorOnPostback = this.HasError;

				// Store confirm message
				hfMessageConfirm.Value = messageConfirm;
			}
		}
		catch (OrderErrorException)
		{
			// エラーメッセージ表示
			DisplayErrorMessages();
			this.HasErrorOnPostback = this.HasError;
			Session[Constants.SESSION_KEY_PARAM_FOR_BACK2] = null;
		}
	}

	/// <summary>
	/// 配送先追加ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAddShipping_Click(object sender, EventArgs e)
	{
		// 配送先追加
		var order = GetOrderInput();
		this.OrderInput.UpdateOrderShippings(order);
		this.OrderInput.AddShipping();
		// 配送方法情報＆配送会社情報再設定
		this.ShippingMethod = this.OrderInput.Shippings.Select(i => i.ShippingMethod).ToArray();
		this.DeliveryCompany = this.OrderInput.Shippings
			.Select(
				shipping => this.DeliveryCompanyList
					.FirstOrDefault(i => (i.DeliveryCompanyId == shipping.DeliveryCompanyId)))
			.Select(
				company => company ?? new DeliveryCompanyModel())
			.ToArray();

		// データバインド
		rShippingList.DataBind();

		// 配送先グローバル関連項目設定
		SetOrderShippingGlobalColumn();
		SetScheduleShipingAndDateShiping();
	}

	/// <summary>
	/// ユーザー管理レベル選択変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlUserManagementLevel_SelectedIndexChanged(object sender, EventArgs e)
	{
		DisplayPaymentUserManagementLevel(this.LoginOperatorShopId, ddlUserManagementLevel.SelectedValue);
	}

	/// <summary>
	/// 詳細へ戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBackDetail_Click(object sender, EventArgs e)
	{
		Response.Redirect(
			CreateOrderDetailUrl(
				this.OrderInput.OrderId,
				false,
				false));
	}

	/// <summary>
	/// 配送方法自動判定ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSetShippingMethod_Click(object sender, EventArgs e)
	{
		var ddlDeliveryCompany = ((DropDownList)this.rShippingList.Items[0].FindControl("ddlDeliveryCompany"));
		var orderInput = GetOrderInput();
		var cart = CreateCart(orderInput);
		var ddlShippingMethod = (DropDownList)rShippingList.Controls[0].FindControl("ddlShippingMethod");
		var ddlShippingTime = (DropDownList)rShippingList.Controls[0].FindControl("ddlShippingTime");

		foreach (var shipping in orderInput.Shippings)
		{
			if (OrderCommon.IsDecideDeliveryMethod(ddlOrderPaymentKbn.SelectedValue)
				&& string.IsNullOrEmpty(ddlShippingTime.SelectedValue)
				&& (shipping.Items.All(item => int.Parse(item.ItemQuantity) > 0)))
			{
				ddlShippingMethod.SelectedValue = shipping.ShippingMethod
					= OrderCommon.GetShippingMethod(shipping.Items.Select(item => item.CreateModel()));
			}
		}

		this.OrderInput.UpdateOrderShippings(orderInput);
		ddlShippingMethod_SelectedIndexChanged(ddlShippingMethod, e);

		var shopShipping = new ShopShippingService().Get(this.LoginOperatorShopId, cart.ShippingType);
		var companyList = (ddlShippingMethod.SelectedValue == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS)
			? shopShipping.CompanyListExpress
			: shopShipping.CompanyListMail;
		ddlDeliveryCompany.Items.Clear();
		foreach (var item in companyList)
		{
			var company = this.DeliveryCompanyList
				.First(itemCompany => (itemCompany.DeliveryCompanyId == item.DeliveryCompanyId));
			ddlDeliveryCompany.Items.Add(new ListItem(company.DeliveryCompanyName, company.DeliveryCompanyId));
		}
		// メール便配送サービスエスカレーション
		if (Constants.DELIVERYCOMPANY_MAIL_ESCALATION_ENBLED
			&& (ddlShippingMethod.SelectedValue == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL))
		{
			var deliveryCompanyId = OrderCommon.DeliveryCompanyMailEscalation(cart.Items, companyList);
			if (string.IsNullOrEmpty(deliveryCompanyId))
			{
				ddlShippingMethod.SelectedValue = Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS;
				var selectedCompany = companyList.FirstOrDefault(item => (item.DeliveryCompanyId == this.OrderInput.Shippings[0].DeliveryCompanyId));
				ddlDeliveryCompany.SelectedValue = (selectedCompany != null)
					? selectedCompany.DeliveryCompanyId
					: companyList.First(item => item.IsDefault).DeliveryCompanyId;
			}
			else
			{
				ddlDeliveryCompany.SelectedValue = deliveryCompanyId;
			}
		} else if (ddlShippingMethod.SelectedValue == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS)
		{
			var selectedCompany = companyList.FirstOrDefault(item => (item.DeliveryCompanyId == this.OrderInput.Shippings[0].DeliveryCompanyId));
			ddlDeliveryCompany.SelectedValue = (selectedCompany != null)
				? selectedCompany.DeliveryCompanyId
				: companyList.First(item => item.IsDefault).DeliveryCompanyId;
		}

		// 再計算
		btnReCalculate_Click(btnReCalculate, e);

		// 配送料変更メッセージ表示
		DisplayChangeShippingPriceMessage(rShippingList.Items[0]);
	}

	/// <summary>
	/// 配送情報リピータイベント
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	public void rShippingList_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		// 注文入力情報取得
		var order = GetOrderInput();

		// 注文配送先情報更新
		this.OrderInput.UpdateOrderShippings(order);

		// 注文商品情報追加
		var orderShippingNo = e.CommandArgument.ToString();
		switch (e.CommandName)
		{
			// 注文商品情報追加
			case "add_product":
				this.OrderInput.AddItem(orderShippingNo);

				ShippingListDataBindAndSetShipingDate();
				break;

			// 配送先情報削除
			case "delete_shipping":
				this.OrderInput.DeleteShipping(orderShippingNo);

				ShippingListDataBindAndSetShipingDate();
				btnReCalculate_Click(btnReCalculate, e);
				break;

			// Calculate shipping date
			case "CalculateScheduledShippingDate":
				{
					var repeaterItem = rShippingList.Items[int.Parse(e.CommandArgument.ToString())];
					var shippingDate = (IsShippingDateUsable(0))
						? ((DateTimePickerPeriodInputControl)repeaterItem.FindControl("ucShippingDate")).StartDateTimeString
						: string.Empty;

					var scheduledShippingDate = OrderCommon.CalculateScheduledShippingDateBasedOnToday(
						this.LoginOperatorShopId,
						(string.IsNullOrEmpty(shippingDate)) ? (DateTime?)null : DateTime.Parse(shippingDate),
						order.Shippings.First().ShippingMethod,
						order.Shippings.First().DeliveryCompanyId,
						order.Shippings.First().ShippingCountryIsoCode,
						(Constants.TW_COUNTRY_SHIPPING_ENABLE
								&& order.Shippings.First().IsShippingAddrTw)
							? order.Shippings.First().ShippingAddr2
							: order.Shippings.First().ShippingAddr1,
						order.Shippings.First().ShippingZip.Replace("-", string.Empty));

					var controlScheduledShippingDate =
						(DateTimePickerPeriodInputControl)repeaterItem.FindControl("ucScheduledShippingDate");
					if (scheduledShippingDate.HasValue)
					{
						controlScheduledShippingDate.SetStartDate(DateTime.Parse(scheduledShippingDate.Value.ToString()));
					}
					break;
				}
		}

		// 配送先グローバル関連項目設定
		SetOrderShippingGlobalColumn();
	}

	/// <summary>
	/// 商品情報リピータイベント
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rItemList_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		var itemsRepeater = (Repeater)source;
		var shippingRepeater = (Repeater)itemsRepeater.Parent.Parent;
		var shippingIndex = int.Parse(e.CommandArgument.ToString()) - 1;
		switch (e.CommandName)
		{
			case "get":
			{
				// 商品ID＆商品バリエーションID取得
				var productId = ((TextBox)itemsRepeater.Items[e.Item.ItemIndex].FindControl("tbProductId")).Text;
				var variationId = productId + ((TextBox)itemsRepeater.Items[e.Item.ItemIndex].FindControl("tbVariationId")).Text;
				var saleId = ((TextBox)itemsRepeater.Items[e.Item.ItemIndex].FindControl("tbProductSaleId")).Text;

				// 商品情報取得
				var productVariation = GetProductVariation(
					this.LoginOperatorShopId,
					productId,
					variationId,
					this.OrderInput.MemberRankId,
					saleId);

				var errorMessages = string.Empty;
				if (productVariation.Count != 0)
				{
					// 定期チェックボックス入力値
					var isFixedPurchaseProduct = ((CheckBox)itemsRepeater.Items[e.Item.ItemIndex].FindControl("cbFixedPurchaseProduct")).Checked;
					// 定期初回注文判定
					var isFirstOrder = (this.OrderInputOld.FixedPurchaseOrderCount == "") ||
						(this.OrderInputOld.FixedPurchaseOrderCount == "1");
					var productName = productVariation[0][Constants.FIELD_PRODUCT_NAME] + CreateVariationName(productVariation[0]);


					((TextBox)itemsRepeater.Items[e.Item.ItemIndex].FindControl("tbProductName")).Text = CreateFixedPurchaseProductName(productName, isFixedPurchaseProduct, this.IsSubscriptionBoxValid);
					((TextBox)itemsRepeater.Items[e.Item.ItemIndex].FindControl("tbItemQuantity")).Text = "1";
					if (this.OrderInput.IsSubscriptionBoxFixedAmount == false)
					{
						((TextBox)itemsRepeater.Items[e.Item.ItemIndex].FindControl("tbProductPrice")).Text
							= (isFixedPurchaseProduct)
								? GetFixedPurchaseProductValidityPrice(productVariation[0], isFirstOrder).ToPriceString()
								: GetProductValidityPrice(productVariation[0]).ToPriceString();
						((HiddenField)itemsRepeater.Items[e.Item.ItemIndex].FindControl("hfSupplierId")).Value = productVariation[0][Constants.FIELD_PRODUCT_SUPPLIER_ID].ToString();
					}
					((TextBox)itemsRepeater.Items[e.Item.ItemIndex].FindControl("tbProductSaleId")).Text = productVariation[0][Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID].ToString();
					((HiddenField)itemsRepeater.Items[e.Item.ItemIndex].FindControl("hfProductTaxRate")).Value = productVariation[0][Constants.FIELD_PRODUCTTAXCATEGORY_TAX_RATE].ToString();

					// 商品の有効フラグがオフの場合はエラーとする
					if ((string)productVariation[0][Constants.FIELD_PRODUCT_VALID_FLG] == Constants.FLG_PRODUCT_VALID_FLG_INVALID)
					{
						errorMessages = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_INVALID)
							.Replace("@@ 1 @@", (string)productVariation[0][Constants.FIELD_PRODUCT_NAME]);
					}
				}
				else
				{
					errorMessages = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERREGIST_NOPRODUCT);
				}

				// エラーメッセージセット
				shippingRepeater.Items[shippingIndex].FindControl("trOrderItemErrorMessagesTitle").Visible = (errorMessages.Length != 0);
				shippingRepeater.Items[shippingIndex].FindControl("trOrderItemErrorMessages").Visible = (errorMessages.Length != 0);
				((Label)shippingRepeater.Items[shippingIndex].FindControl("lbOrderItemErrorMessages")).Text = errorMessages;

				// 再計算
				if (errorMessages.Length == 0)
				{
					btnReCalculate_Click(itemsRepeater.Items[e.Item.ItemIndex], e);
				}

				break;
			}
			case "delete_item":
			case "delete_set":
			{
				// 商品削除の前に入力情報を取得する
				this.OrderInput.Update(GetOrderInput());

				var addProductButtonArgument = ((Button)((RepeaterItem)itemsRepeater.Parent).FindControl("btnAddProduct")).CommandArgument;
				this.OrderInput.RemoveItem(addProductButtonArgument, e.Item.ItemIndex);
				ShippingListDataBindAndSetShipingDate();

				btnReCalculate_Click(null, e);
				break;
			}
		}
	}

	/// <summary>
	/// 商品定期チェック変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbFixedPurchaseProduct_CheckedChanged(object sender, EventArgs e)
	{
		var repeater = ((CheckBox)sender).Parent.Parent;

		// 定期チェックボックス入力値
		var isFixedPurchaseProduct = ((CheckBox)sender).Checked;

		// 定期チェック有無に応じた商品名に編集
		var productNameTextBox = (TextBox)repeater.FindControl("tbProductName");
		productNameTextBox.Text = CreateFixedPurchaseProductName(productNameTextBox.Text, isFixedPurchaseProduct, this.IsSubscriptionBoxValid);

		// 商品情報表のデザイン変更
		ChangeOrderItemsTable(sender, repeater, isFixedPurchaseProduct);

		// Remove store pickup from shipping option if fixed purchase is checked
		var ddlShippingMethod = (DropDownList)rShippingList.Controls[0].FindControl("ddlShippingMethod");
		var parent = (RepeaterItem)((DropDownList)ddlShippingMethod).Parent;
		var ddlUserShipping = ((DropDownList)parent.FindControl("ddlUserShipping"));
		var fixedPurchaseChecked = ((CheckBox)sender).Checked;
		if (fixedPurchaseChecked && this.IsStorePickup)
		{
			ddlUserShipping.Items.Clear();
			ddlUserShipping.Items.AddRange(GetItemShippingSelect(false).ToArray());
			ddlUserShipping.SelectedValue = CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER;
			ddlUserShipping_SelectedIndexChanged(ddlUserShipping, e);
		}
		else if (ddlUserShipping.Items
			.FindByValue(CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_STORE_PICKUP) == null)
		{
			var selectedValue = ddlUserShipping.SelectedValue;
			ddlUserShipping.Items.Clear();
			ddlUserShipping.Items.AddRange(GetItemShippingSelect().ToArray());
			if (ddlUserShipping.Items.FindByValue(selectedValue) != null)
			{
				ddlUserShipping.SelectedValue = selectedValue;
			}
		}
	}

	/// <summary>
	/// 注文確認画面へ遷移
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbOrderConfirm_Click(object sender, EventArgs e)
	{
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDER_MODIFY_CONFIRM + "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + this.ActionStatus);
	}

	/// <summary>
	/// 配送料別途見積もり表示のクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbShippingPriceSeparateEstimateFlg_OnCheckedChanged(object sender, EventArgs e)
	{
		tbOrderPriceShipping.Enabled = (cbShippingPriceSeparateEstimateFlg.Checked == false);
		tbOrderPriceShipping.Text = "0";
	}

	/// <summary>
	/// 配送種別選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlShipping_SelectedIndexChanged(object sender, EventArgs e)
	{
		// 配送種別情報セット
		this.OrderInput.ShippingId = ddlShipping.SelectedValue;
		this.ShopShipping = new ShopShippingService().Get(this.LoginOperatorShopId, this.OrderInput.ShippingId);

		this.OrderInput.Shippings = GetOrderShippingsInput();

		// 決済種別
		var payments =
			GetPaymentValidListPermission(this.LoginOperatorShopId, this.ShopShipping.PaymentSelectionFlg, this.ShopShipping.PermittedPaymentIds)
			.ToArray();
		if (OrderCommon.CheckPaymentYamatoKaSms(this.OrderOld.OrderPaymentKbn) == false)
		{
			payments = payments.Where(item => (OrderCommon.CheckPaymentYamatoKaSms(item.PaymentId) == false)).ToArray();
		}
		// 決済種別が存在しない場合はエラー
		if (payments.Length == 0)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PAYMENT_NO_DATA);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// Remove invalid payment for store pickup
		if (this.IsStorePickup)
		{
			payments = GetValidStorePickupPayments();
		}

		var orderPaymentKbnOld = ddlOrderPaymentKbn.SelectedValue;
		ddlOrderPaymentKbn.Items.Clear();
		ddlOrderPaymentKbn.Items.Add(new ListItem("", ""));
		foreach (var payment in payments)
		{
			ddlOrderPaymentKbn.Items.Add(
				new ListItem(payment.PaymentName, payment.PaymentId));
			if (payment.PaymentId == orderPaymentKbnOld)
			{
				ddlOrderPaymentKbn.SelectedValue = payment.PaymentId;
			}
		}

		ControlPaymentWhenShippingConvenience();

		rShippingList.DataBind();

		// 配送先グローバル関連項目設定
		SetOrderShippingGlobalColumn();
	}

	/// <summary>
	/// 配送方法選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlShippingMethod_SelectedIndexChanged(object sender, EventArgs e)
	{
		var repeater = (RepeaterItem)((DropDownList)sender).Parent;

		var ddlShippingMethod = ((DropDownList)repeater.FindControl("ddlShippingMethod"));
		var ddlDeliveryCompany = ((DropDownList)repeater.FindControl("ddlDeliveryCompany"));
		var ddlUserShipping = ((DropDownList)repeater.FindControl("ddlUserShipping"));
		var index = repeater.ItemIndex;

		this.ShippingMethod[index] = ddlShippingMethod.SelectedValue;

		var selectedValue = ddlUserShipping.SelectedValue;
		ddlUserShipping.Items.Clear();
		ddlUserShipping.Items.AddRange(GetItemShippingSelect().ToArray());
		if (ddlUserShipping.Items.FindByValue(selectedValue) != null)
		{
			ddlUserShipping.SelectedValue = selectedValue;
		}

		var isShippingConvenience = (ddlUserShipping.SelectedValue == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE);
		var shippingNo = 0;
		if ((isShippingConvenience == false) && int.TryParse(ddlUserShipping.SelectedValue, out shippingNo))
		{
			var userShipping = this.UserShippingAddress.FirstOrDefault(shipping => (shipping.ShippingNo == shippingNo));
			if (userShipping != null)
			{
				isShippingConvenience = (userShipping.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON);
			}
		}

		// 配送会社
		ddlDeliveryCompany.Items.Clear();
		var companyList = (ddlShippingMethod.SelectedValue == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS ? this.ShopShipping.CompanyListExpress : this.ShopShipping.CompanyListMail);
		companyList = (isShippingConvenience
			? companyList
				.Where(
					company => (company.DeliveryCompanyId == Constants.TWPELICANEXPRESS_CONVENIENCE_STORE_ID))
				.ToArray()
			: companyList
				.Where(
					company => (company.DeliveryCompanyId != Constants.TWPELICANEXPRESS_CONVENIENCE_STORE_ID))
				.ToArray());

		var companyItemList = this.DeliveryCompanyList
			.Where(company => companyList.Any(c => company.DeliveryCompanyId == c.DeliveryCompanyId))
			.Select(company => new ListItem(company.DeliveryCompanyName, company.DeliveryCompanyId));
		ddlDeliveryCompany.Items.AddRange(companyItemList.ToArray());

		var selectedCompany = companyList.FirstOrDefault(item => (item.DeliveryCompanyId == this.OrderInput.Shippings[0].DeliveryCompanyId));
		ddlDeliveryCompany.SelectedValue = (selectedCompany != null)
			? selectedCompany.DeliveryCompanyId
			: companyList.First(item => item.IsDefault).DeliveryCompanyId;
		if (companyList.Any())
		{
			this.DeliveryCompany[index] = this.DeliveryCompanyList.First(i => (i.DeliveryCompanyId == ddlDeliveryCompany.SelectedValue));
		}

		ddlDeliveryCompany_SelectedIndexChanged(sender, e);

		// 決済種別
		var paymentSelectionFlg = string.Empty;
		var permittedPaymentIds = string.Empty;
		if (this.ShopShipping != null)
		{
			paymentSelectionFlg = this.ShopShipping.PaymentSelectionFlg;
			permittedPaymentIds = this.ShopShipping.PermittedPaymentIds;
		}
		var payments =
			GetPaymentValidListPermission(this.LoginOperatorShopId, paymentSelectionFlg, permittedPaymentIds)
			.ToArray();
		if (OrderCommon.CheckPaymentYamatoKaSms(this.OrderOld.OrderPaymentKbn) == false)
		{
			payments = payments.Where(item => (OrderCommon.CheckPaymentYamatoKaSms(item.PaymentId) == false)).ToArray();
		}
		// 決済種別が存在しない場合はエラー
		if (payments.Length == 0)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PAYMENT_NO_DATA);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// Remove invalid payment for store pickup
		if (this.IsStorePickup)
		{
			payments = GetValidStorePickupPayments();
		}

		var orderPaymentKbnOld = ddlOrderPaymentKbn.SelectedValue;
		ddlOrderPaymentKbn.Items.Clear();
		ddlOrderPaymentKbn.Items.Add(new ListItem("", ""));
		foreach (var payment in payments)
		{
			if ((this.ShippingMethod[index] == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL)
				&& (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT)) continue;
			if ((this.OrderInputOld.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
				&& (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)) continue;
			if ((this.OrderInputOld.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
				&& (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)) continue;
			if ((this.OrderInputOld.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
				&& (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)) continue;
			if ((this.OrderInputOld.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA)
				&& (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA)) continue;

			ddlOrderPaymentKbn.Items.Add(
				new ListItem(payment.PaymentName, payment.PaymentId));
			if (payment.PaymentId == orderPaymentKbnOld)
			{
				ddlOrderPaymentKbn.SelectedValue = payment.PaymentId;
			}
		}

		if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED)
		{
			ddlDeliveryCompany.Items.Clear();
			ddlDeliveryCompany.Items.AddRange(
				OrderCommon.GetShippingServiceRelatedProduct(
				this.ShopShipping.ShopId,
				this.ShopShipping.ShippingId,
				ddlShippingMethod.SelectedValue,
				(ddlUserShipping.SelectedValue == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE)));
		}

		ControlPaymentWhenShippingConvenience();
		ddlOrderPaymentKbn_SelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// 配送会社選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlDeliveryCompany_SelectedIndexChanged(object sender, EventArgs e)
	{
		var repeater = (RepeaterItem)((DropDownList)sender).Parent;

		var ddlDeliveryCompany = ((DropDownList)repeater.FindControl("ddlDeliveryCompany"));
		var ddlShippingTime = ((DropDownList)repeater.FindControl("ddlShippingTime"));
		var trShippingDate1 = ((HtmlControl)repeater.FindControl("trShippingDate1"));
		var trShippingDate2 = ((HtmlControl)repeater.FindControl("trShippingDate2"));
		var trShippingTime1 = ((HtmlControl)repeater.FindControl("trShippingTime1"));
		var trShippingTime2 = ((HtmlControl)repeater.FindControl("trShippingTime2"));

		// 配送料変更メッセージ表示
		DisplayChangeShippingPriceMessage(repeater);

		var index = repeater.ItemIndex;

		if (string.IsNullOrEmpty(ddlDeliveryCompany.SelectedValue) == false)
		{
			this.DeliveryCompany[index] = this.DeliveryCompanyList.First(i => (i.DeliveryCompanyId == ddlDeliveryCompany.SelectedValue));
		}

		if ((ViewState["DeliveryServiceSelected"] != null)
			&& (ddlDeliveryCompany.SelectedIndex == this.DeliveryServiceSelected)) return;

		ddlShippingTime.Items.Clear();

		trShippingDate1.Visible = IsShippingDateUsable(index);
		trShippingTime1.Visible = IsShippingTimeUsable(index);
		trShippingDate2.Visible = (IsShippingDateUsable(index) == false);
		trShippingTime2.Visible = (IsShippingTimeUsable(index) == false);

		if (IsShippingTimeUsable(index) == false) return;

		foreach (ListItem item in ShippingTimeList(index))
		{
			ddlShippingTime.Items.Add(item);
		}
	}

	/// <summary>
	/// 決済種別選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlOrderPaymentKbn_SelectedIndexChanged(object sender, EventArgs e)
	{
		lOrderPaymentInfo.Text = "";

		if (Constants.PAYMENT_REAUTH_ENABLED == false)
		{
			tbdyPaymentKbnCredit.Visible = false;
			return;
		}
		// クレジットカード入力項目の表示切り替え
		tbdyPaymentKbnCredit.Visible = ((ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
			&& Constants.REAUTH_COMPLETE_CREDITCARD_LIST.Contains(Constants.PAYMENT_CARD_KBN)
			&& this.OrderInput.IsPermitReauthOrderSiteKbn);

		if (Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT == ddlOrderPaymentKbn.SelectedValue)
		{
			lbPaymentAlertMessage.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_AMAZON_PAY_CHANGE_AMOUNT_NOTIFY);
		}

		if (Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2 == ddlOrderPaymentKbn.SelectedValue)
		{
			tbdyPaymentNoticeMessage.Visible = true;
			lbPaymentAlertMessage.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_AMAZON_PAY_CV2_CHANGE_AMOUNT_NOTIFY);
		}

		// 外部決済連携処理区分（変更前・変更後）設定
		ddlNewExecuteType.SelectedValue = ReauthCreatorFacade.ExecuteTypes.System.ToString();
		ddlNewExecuteType.Enabled = true;
		ddlNewExecuteType.Visible = true;
		//「決済連動」
		lbNewExternalPayment.Text = ValueText.GetValueText(
			Constants.VALUETEXT_PARAM_ORDER,
			Constants.VALUETEXT_PARAM_ORDER_MODIFY_INPUT,
			Constants.VALUETEXT_PARAM_ORDER_MODIFY_PAYMENT_INTERLOOKING);

		ddlOldExecuteType.SelectedValue = ReauthCreatorFacade.ExecuteTypes.None.ToString();
		ddlOldExecuteType.Enabled = false;
		ddlOldExecuteType.Visible = false;
		lbOldExternalPayment.Text = string.Empty;

		// モール注文の場合、固定に「連動しない」を設定する
		if (this.OrderInput.IsPermitReauthOrderSiteKbn == false)
		{
			ddlNewExecuteType.SelectedValue = ReauthCreatorFacade.ExecuteTypes.None.ToString();
			ddlNewExecuteType.Enabled = false;
			return;
		}

		// 自社サイトの注文の場合
		// 決済種別を変更した場合
		if ((this.OrderInputOld.OrderPaymentKbn != ddlOrderPaymentKbn.SelectedValue)
			&& (ddlOrderPaymentKbn.SelectedValue != string.Empty))
		{
			// キャリア決済やPayPal決済等与信未対応の決済に変更した場合アラートメッセージ表示
			if (IsSupportsReauthForAlert(ddlOrderPaymentKbn.SelectedValue) == false)
			{
				lbPaymentAlertMessage.Text =
					WebMessages.GetMessages(WebMessages.ERRMSG_EXTERNAL_PAYMENT_NOT_AUTH_ALERT);
			}

			var oldPayment = new PaymentService().GetPaymentName(
				this.LoginOperatorShopId,
				this.OrderInputOld.OrderPaymentKbn);
			lbOldExternalPayment.Text = string.Format(
				"{0}{1}",
				//「【変更前】」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ORDER,
					Constants.VALUETEXT_PARAM_ORDER_MODIFY_INPUT,
					Constants.VALUETEXT_PARAM_ORDER_MODIFY_CHANGE_BEFORE),
				oldPayment);
			lbNewExternalPayment.Text = string.Format(
				"{0}{1}",
				//「【変更後】」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ORDER,
					Constants.VALUETEXT_PARAM_ORDER_MODIFY_INPUT,
					Constants.VALUETEXT_PARAM_ORDER_MODIFY_AFTER_CHANGE),
				WebSanitizer.HtmlEncode(ddlOrderPaymentKbn.SelectedItem.Text));
			ddlOldExecuteType.Visible = true;

			// 変更前の決済連動設定
			// 再与信可能の決済の場合、「システム連動」＆有効にする
			if (IsCanReauthWhenChangePayment(this.OrderInputOld.OrderPaymentKbn, false))
			{
				ddlOldExecuteType.SelectedValue = ReauthCreatorFacade.ExecuteTypes.System.ToString();
				ddlOldExecuteType.Enabled = true;

				if (PaygentUtility.CheckIsPaidyPaygentPayment(this.OrderInputOld.OrderPaymentKbn)
					&& ((this.OrderInputOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_EXPIRED)
						|| (this.OrderInputOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_CANCEL_EXPIRED)))
				{
					ddlOldExecuteType.SelectedValue = ReauthCreatorFacade.ExecuteTypes.None.ToString();
					ddlOldExecuteType.Enabled = false;
				}
			}

			// 変更後の決済連動設定
			// 再与信不可能の決済の場合、「連動しない」＆無効にする
			if (IsCanReauthWhenChangePayment(ddlOrderPaymentKbn.SelectedValue, true) == false)
			{
				ddlNewExecuteType.SelectedValue = ReauthCreatorFacade.ExecuteTypes.None.ToString();
				ddlNewExecuteType.Enabled = false;
			}

			// PayPalのアカウントを表示
			lOrderPaymentInfo.Text = "";
			if ((ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
				&& (this.OrderOld.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL))
			{
				var accountEmail = PayPalUtility.Account.GetCooperateAccountEmail(this.User.UserId);
				lOrderPaymentInfo.Text = WebSanitizer.HtmlEncode(
					string.IsNullOrEmpty(accountEmail)
						? WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PAYPAL_ISNOT_LINK_ACCOUNT)
						: WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PAYPAL_AVAILABLE_ACCOUNT).Replace("@@ 1 @@", accountEmail));
			}
		}

		if (ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
		{
			ddlUserCreditCard_SelectedIndexChanged(sender, e);
		}

		if (ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT
			|| this.OrderInputOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
		{
			var userCreditCard = string.IsNullOrEmpty(this.OrderInputOld.CreditBranchNo) == false
				? UserCreditCard.Get(this.OrderInput.UserId, int.Parse(this.OrderInputOld.CreditBranchNo))
				: null;
			// 再与信不可能のカードの場合、「連動しない」＆無効にする
			if (IsReAuthPossibleCard(userCreditCard) == false)
			{
				ddlOldExecuteType.SelectedValue = ReauthCreatorFacade.ExecuteTypes.None.ToString();
				ddlOldExecuteType.Enabled = false;
				ddlNewExecuteType.SelectedValue = ReauthCreatorFacade.ExecuteTypes.None.ToString();
				ddlNewExecuteType.Enabled = false;
			}
		}

		RefreshExternalPaymentReauthSelect();

		// Get gmo cvs type
		GetGmoCvsType();

		SetPaymentNoticeMessageVisibility();
	}

	/// <summary>
	/// Set Visible For Uniform Option
	/// </summary>
	/// <param name="uniformType">Uniform Type</param>
	protected void SetVisibleForUniformOption(string uniformType)
	{
		var isPersonal = false;
		var isCompany = false;
		var isDonate = false;

		OrderCommon.CheckUniformType(
			uniformType,
			ref isPersonal,
			ref isDonate,
			ref isCompany);

		this.IsCompany = isCompany;
		this.IsDonate = isDonate;
		this.IsPersonal = isPersonal;

		tbCompanyOption1.Visible =
			tbCompanyOption2.Visible =
			tbDonateOption1.Visible =
			string.IsNullOrEmpty(ddlUniformInvoiceOrCarryTypeOption.SelectedValue);

		lbCompanyOption1.Visible =
			lbCompanyOption2.Visible =
			lbDonateOption1.Visible =
			(string.IsNullOrEmpty(ddlUniformInvoiceOrCarryTypeOption.SelectedValue) == false);
	}

	/// <summary>
	/// Set taiwan invoice data when modify order
	/// </summary>
	private void SetTwInvoiceDataWhenModifyOrder()
	{
		if (this.TwOrderInvoice == null) return;

		// Refresh Uniform Invoice Or Carry Type Option
		ddlUniformInvoiceOrCarryTypeOption.DataSource = GetUniformInvoiceOrCarryTypeOption(
			this.TwOrderInvoice.TwUniformInvoice,
			this.TwOrderInvoice.TwCarryType);
		ddlUniformInvoiceOrCarryTypeOption.DataBind();

		SetVisibleForCarryTypeOption(this.TwOrderInvoice.TwCarryType);
		SetVisibleForUniformOption(this.TwOrderInvoice.TwUniformInvoice);
		ddlUniformInvoiceOrCarryTypeOption.Visible =
			((this.IsPersonal
					&& (string.IsNullOrEmpty(this.TwOrderInvoiceInput.TwCarryTypeOption) == false))
				|| this.IsCompany
				|| this.IsDonate);

		tbCompanyOption1.Text = this.IsCompany
			? this.TwOrderInvoice.TwUniformInvoiceOption1
			: string.Empty;
		tbCompanyOption2.Text = this.IsCompany
			? this.TwOrderInvoice.TwUniformInvoiceOption2
			: string.Empty;
		tbDonateOption1.Text = this.IsDonate
			? this.TwOrderInvoice.TwUniformInvoiceOption1
			: string.Empty;
		tbCarryTypeOption1.Text = (this.IsPersonal && this.IsMobile)
			? this.TwOrderInvoice.TwCarryTypeOption
			: string.Empty;
		tbCarryTypeOption2.Text = (this.IsPersonal && this.IsCertificate)
			? this.TwOrderInvoice.TwCarryTypeOption
			: string.Empty;
	}

	/// <summary>
	///  Set edit taiwan invoice data when modify order
	/// </summary>
	/// <param name="canEdit">Can edit invoice data</param>
	private void SetEditTwInvoiceDataWhenModifyOrder(bool canEdit)
	{
		if (this.TwOrderInvoiceInput != null)
		{
			ddlUniformInvoice.SelectedValue = this.TwOrderInvoiceInput.TwUniformInvoice;
			ddlCarryType.SelectedValue = this.TwOrderInvoiceInput.TwCarryType;
		}
		ddlUniformInvoice.Enabled = canEdit;
		ddlCarryType.Enabled = canEdit;
		ddlUniformInvoiceOrCarryTypeOption.Enabled = canEdit;

		SetVisibleForUniformOption(ddlUniformInvoice.SelectedValue);

		if (this.IsPersonal)
		{
			tbCarryTypeOption1.Text = (this.IsMobile)
				? this.TwOrderInvoiceInput.TwCarryTypeOption
				: string.Empty;
			tbCarryTypeOption1.Enabled = canEdit;
			tbCarryTypeOption2.Text = (this.IsCertificate)
				? this.TwOrderInvoiceInput.TwCarryTypeOption
				: string.Empty;
			tbCarryTypeOption2.Enabled = canEdit;
		}

		if (this.IsCompany)
		{
			tbCompanyOption1.Text = this.TwOrderInvoiceInput.TwUniformInvoiceOption1;
			tbCompanyOption1.Enabled = canEdit;
			tbCompanyOption2.Text = this.TwOrderInvoiceInput.TwUniformInvoiceOption2;
			tbCompanyOption2.Enabled = canEdit;
		}

		if (this.IsDonate)
		{
			tbDonateOption1.Text = this.TwOrderInvoiceInput.TwUniformInvoiceOption1;
			tbDonateOption1.Enabled = canEdit;
		}
	}

	/// <summary>
	/// 再与信サポートしているか
	/// </summary>
	/// <param name="paymentId">決済種別ID</param>
	/// <returns>再与信サポートしているか</returns>
	private bool IsSupportsReauthForAlert(string paymentId)
	{
		switch (paymentId)
		{
			case Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT:
				return Constants.REAUTH_COMPLETE_CREDITCARD_LIST.Contains(Constants.PAYMENT_CARD_KBN);

			case Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF:
				return Constants.REAUTH_COMPLETE_CVSDEF_LIST.Contains(Constants.PAYMENT_CVS_DEF_KBN);

			case Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL:
				if (Constants.PAYPAL_LOGINPAYMENT_ENABLED == false) return false;
				if (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL) return true;
				return (string.IsNullOrEmpty(this.User.UserExtend.UserExtendDataValue[Constants.PAYPAL_USEREXTEND_COLUMNNAME_CUSTOMER_ID]) == false);

			case Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY:
				return Constants.PAYMENT_LINEPAY_OPTION_ENABLED;

			case Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY:
				if (Constants.PAYMENT_NP_AFTERPAY_OPTION_ENABLED == false) return false;
				return true;

			case Constants.FLG_PAYMENT_PAYMENT_ID_DSK_DEF:
				return true;

			default:
				return false;
		}
	}

	/// <summary>
	/// 決済種別変更時再与信可否判断
	/// </summary>
	/// <param name="paymentKbn">決済区分</param>
	/// <param name="newFlg">変更前・後フラグ（true:　変更後、false: 変更前）</param>
	/// <returns>再与信可否(true:可能、false:不可)</returns>
	private bool IsCanReauthWhenChangePayment(string paymentKbn, bool newFlg)
	{
		// 再与信可能なクレカの場合true
		if ((paymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
				&& Constants.REAUTH_COMPLETE_CREDITCARD_LIST.Contains(Constants.PAYMENT_CARD_KBN))
		{
			switch (Constants.PAYMENT_CARD_KBN)
			{
				case Constants.PaymentCard.EScott:
					return IsReAuthPossibleCard(this.UserCreditCard);

				default:
					return true;
			}
		}

		// 再与信可能な後払いの場合true
		if ((paymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
			&& Constants.REAUTH_COMPLETE_CVSDEF_LIST.Contains(Constants.PAYMENT_CVS_DEF_KBN))
		{
			return true;
		}

		// 変更前はキャンセル対応必須（キャリア決済など）
		if ((newFlg == false)
			&& Constants.REAUTH_CANCEL_PAYMENT_LIST.Contains(paymentKbn))
		{
			return true;
		}

		if (paymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU)
		{
			return true;
		}

		switch (paymentKbn)
		{
			// PayPal決済の場合true
			case Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL:
			// 台湾後払い決済の場合true
			case Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY:
			// PaidyPay決済の場合true
			case Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY:
			// atone翌月払い決済の場合true
			case Constants.FLG_PAYMENT_PAYMENT_ID_ATONE:
			// aftee翌月払い決済の場合true
			case Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE:
			// LINE Pay決済の場合true
			case Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY:
			// NP後払い決済の場合true
			case Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY:
			// Ec Pay決済の場合true
			case Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY:
			// 藍新Pay決済の場合true
			case Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY:
			// PayPay決済の場合true
			case Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY:
			// AmazonPayの場合true
			case Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT:
			// AmazonPay(CV2)の場合true
			case Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2:
			// GMO Invoice
			case Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO:
			case Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE:
			case Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA:
				return true;
			default:
				return false;
		}
	}

	/// <summary>
	/// 再与信可能カード判断(EScott)
	/// </summary>
	/// <param name="userCreditCard">クレジットカード情報</param>
	/// <returns>再与信可否(true:可能、false:不可)</returns>
	private bool IsReAuthPossibleCard(UserCreditCard userCreditCard)
	{
		if (userCreditCard == null) return true;
		var cooperationIdArray = userCreditCard.CooperationId.Split(',');
		// つくーるでの通常注文取り込み時再与信不可
		if (string.IsNullOrEmpty(cooperationIdArray[0]) && (cooperationIdArray.Length == 2))
		{
			return false;
		}
		return true;
	}

	/// <summary>
	/// 自動計算適用チェック変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbApplyAutoCalculation_CheckedChanged(object sender, EventArgs e)
	{
		// データバインド
		// 自動計算適用する入力域の有効無効を切替
		dvRecalculateArea.DataBind();
		divCoupon.DataBind();
		divPoint.DataBind();
	}

	#region メソッド
	/// <summary>
	/// 処理区分チェック
	/// </summary>
	private void CheckActionStatus()
	{
		// 編集でなければエラーページへ
		CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);
		if (this.IsUpdateShippingConvenience
			|| (this.ActionStatus == Constants.ACTION_STATUS_UPDATE)) return;

		Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
	}

	/// <summary>
	/// 各プロパティセット
	/// </summary>
	private void SetProperty()
	{
		SetOrderProperty();

		// ユーザ＆ユーザポイント情報セット
		SetUserPointProperty();

		this.User = new UserService().Get(this.OrderInput.UserId);

		SetUserPointProperty();

		SetShippingProperty();

		// 項目メモ一覧セット
		this.FieldMemoSettingList = GetFieldMemoSettingList(Constants.TABLE_ORDER);

		// 全ての関連する注文の最終与信フラグが正確であるかどうか
		var orderIdOrg = string.IsNullOrEmpty(this.OrderInput.OrderIdOrg) ? this.OrderInput.OrderId : this.OrderInput.OrderIdOrg;
		this.IsAllLastAuthFlgValid = new OrderService().GetRelatedOrders(orderIdOrg).Any(o => o.LastAuthFlg == null);

		SetTwInvoiceProperty();

		// ユーザークレジットカード情報セット
		SetUserCreditCard();
	}

	/// <summary>
	/// 各プロパティセット 注文情報
	/// </summary>
	private void SetOrderProperty()
	{
		var order = new OrderService().GetOrderInfoByOrderId(this.RequestOrderId);
		if (order == null)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		this.OrderInput = new OrderInput(order);
		this.OrderInputOld = new OrderInput(order);
		this.OrderOld = OrderCommon.GetLastAuthOrder(this.OrderInput.OrderId);
		this.OrderInput.LastBilledAmount = this.OrderOld.LastBilledAmount.ToString();
	}

	/// <summary>
	/// 各プロパティセット 配送
	/// </summary>
	private void SetShippingProperty()
	{
		this.ShopShipping = new ShopShippingService().Get(this.LoginOperatorShopId, this.OrderInput.ShippingId);

		this.ShippingMethod = this.OrderInput.Shippings.Select(i => i.ShippingMethod).ToArray();

		// 配送会社情報セット
		this.DeliveryCompany = this.OrderInput.Shippings
			.Select(
				shipping => this.DeliveryCompanyList
					.FirstOrDefault(
						i => (i.DeliveryCompanyId == shipping.DeliveryCompanyId)))
			.Select(company => company ?? new DeliveryCompanyModel())
			.ToArray();

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			this.ShippingAvailableCountryList = new CountryLocationService().GetShippingAvailableCountry();
		}
	}

	/// <summary>
	/// 各プロパティのセット 電子発票
	/// </summary>
	private void SetTwInvoiceProperty()
	{
		if (OrderCommon.DisplayTwInvoiceInfo())
		{
			var twOrderInvoice = new TwOrderInvoiceService().GetOrderInvoice(
				this.OrderInput.OrderId,
				int.Parse(this.OrderInput.Shippings[0].OrderShippingNo));

			if (twOrderInvoice != null)
			{
				this.TwOrderInvoiceInput = new TwOrderInvoiceInput(twOrderInvoice);

				this.TwOrderInvoice = twOrderInvoice;
			}
		}

		if ((Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED == false)
			|| (this.OrderInput.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE)) return;

		if (Session[Constants.SESSIONPARAM_KEY_ORDER_MODIFY_INFO] == null) return;

		var parameters = (Hashtable)Session[Constants.SESSIONPARAM_KEY_ORDER_MODIFY_INFO];
		if (parameters.ContainsKey(Constants.TABLE_ORDER) == false) return;

		this.OrderInput = (OrderInput)parameters[Constants.TABLE_ORDER];
		Session[Constants.SESSIONPARAM_KEY_ORDER_MODIFY_INFO] = null;
	}

	/// <summary>
	/// 各プロパティセット ユーザポイント
	/// </summary>
	private void SetUserPointProperty()
	{
		this.IsNoPointPublished = Constants.FLG_USERPOINT_POINT_PUBLISHED;
		// 会員ユーザの場合、ユーザポイントセット
		this.User = new UserService().Get(this.OrderInput.UserId);
		// 統合前のユーザを取得
		this.BeforeIntegrationUser = new UserIntegrationService().GetBeforeIntegrationUserByOrderId(this.OrderInput.OrderId);
		if (UserService.IsUser(this.User.UserKbn) == false) return;
		var userPointRelatedOrder = new PointService().GetUserPoint(this.User.UserId, string.Empty)
				.Where(x => (x.IsPointTypeComp && x.IsBasePoint)
						|| (x.IsLimitedTermPoint && x.OrderId == this.OrderOld.OrderId)
						|| (x.IsPointTypeTemp && x.OrderId == this.OrderOld.OrderId))
		.ToArray();

		this.UserPointList = new UserPointList(userPointRelatedOrder);
		// 注文関連ポイント情報(通常本ポイントだけ除く)
		this.UserPointRelatedThisOrder = new UserPointList(
			userPointRelatedOrder.Where(x => ((x.IsPointTypeComp && x.IsBasePoint) == false)).ToArray());
		// 比較用に古いのも保存する
		this.UserPointOldRelatedThisOrder = new UserPointList(
			userPointRelatedOrder.Where(x => ((x.IsPointTypeComp && x.IsBasePoint) == false)).ToArray());

		if (this.UserPointOldRelatedThisOrder.Items.Count == 0)
		{
			this.IsNoPointPublished = Constants.FLG_USERPOINT_POINT_NOT_PUBLISHED;
		}
		// 通常本ポイントの区分枝番を取得
		var basePointComp = userPointRelatedOrder.FirstOrDefault(up => (up.IsPointTypeComp && up.IsBasePoint));
		this.UserBasePointCompPointKbnNo = (basePointComp != null) ? basePointComp.PointKbnNo.ToString() : "";
	}

	/// <summary>
	/// ユーザークレジットカード情報セット
	/// </summary>
	private void SetUserCreditCard()
	{
		if ((string.IsNullOrEmpty(this.OrderInput.CreditBranchNo) == false)
			&& ((this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
				|| (this.OrderInput.OrderPaymentKbn == Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID)))
		{
			this.UserCreditCard = UserCreditCard.Get(this.OrderInput.UserId, int.Parse(this.OrderInput.CreditBranchNo));
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// 基本情報
		ddlOrderKbn.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_KBN));

		// 注文者情報
		InitializeComponentsOwner();

		// ユーザー情報
		ddlUserManagementLevel.Items.AddRange(
			new UserManagementLevelService().GetAllList()
				.Select(m => new ListItem(m.UserManagementLevelName, m.UserManagementLevelId)).ToArray());

		// 領収書
		InitializeComponentsReceipt();

		rblReturnExchangeReasonKbn.Items.AddRange(
			ValueText.GetValueItemArray(Constants.TABLE_ORDER, Constants.FIELD_ORDER_RETURN_EXCHANGE_REASON_KBN)
				.Where(li => li.Value != Constants.FLG_ORDER_RETURN_EXCHANGE_REASON_KBN_UNKNOWN).ToArray());

		// 支払い方法
		InitializeComponentsPayment();

		// 外部連携注文取込で配送種別IDが欠落している場合、仮注文時に限り選択可能にする
		if (this.CanSelectShipping)
		{
			ddlShipping.Items.Add(new ListItem("", ""));
			ddlShipping.Items.AddRange(
				new ShopShippingService().GetAll(this.OrderInput.ShopId).Select(
					shipping => new ListItem(shipping.ShopShippingName, shipping.ShippingId)).ToArray());
		}

		// 電子発票
		InitializeComponentsTwInvoice();

		InitializeComponentsPayTg();

		SetValueToComponentsSettlementAmountByTwConvenienceStore();
	}

	/// <summary>
	/// コンポーネント初期化 注文者
	/// </summary>
	private void InitializeComponentsOwner()
	{
		ddlOwnerAddr1.Items.Add(new ListItem("", ""));
		ddlOwnerAddr1.Items.AddRange(Constants.STR_PREFECTURES_LIST.Select(p => new ListItem(p)).ToArray());
		rblOwnerSex.Items.AddRange(
			ValueText.GetValueItemArray(Constants.TABLE_ORDEROWNER, Constants.FIELD_ORDEROWNER_OWNER_SEX));

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			ddlAccessCountryIsoCode.Items.Add(new ListItem("", ""));
			ddlDispCurrencyLocaleId.Items.Add(new ListItem("", ""));
			ddlDispLanguageLocaleId.Items.Add(new ListItem("", ""));
			ddlAccessCountryIsoCode.Items.AddRange(
				Constants.GLOBAL_CONFIGS.GlobalSettings.CountryIsoCodes
					.Select(cic => new ListItem(cic, cic))
					.ToArray());
			ddlDispLanguageLocaleId.Items.AddRange(
				Constants.GLOBAL_CONFIGS.GlobalSettings.Languages
					.Select(l => new ListItem(GlobalConfigUtil.LanguageLocaleIdDisplayFormat(l.LocaleId), l.LocaleId))
					.ToArray());
			ddlDispCurrencyLocaleId.Items.AddRange(
				Constants.GLOBAL_CONFIGS.GlobalSettings.Currencies
					.SelectMany(
					cu => cu.CurrencyLocales
						.Select(
						cul => new ListItem(
							GlobalConfigUtil.CurrencyLocaleIdDisplayFormat(cul.LocaleId),
							cul.LocaleId)))
					.ToArray());

			var countries = new CountryLocationService().GetCountryNames();
			ddlOwnerCountry.Items.Add(new ListItem("", ""));
			ddlOwnerCountry.Items.AddRange(
				countries.Select(c => new ListItem(c.CountryName, c.CountryIsoCode)).ToArray());
			ddlOwnerAddr5.Items.AddRange(Constants.US_STATES_LIST.Select(state => new ListItem(state)).ToArray());
		}
	}

	/// <summary>
	/// コンポーネント初期化 領収書
	/// </summary>
	private void InitializeComponentsReceipt()
	{
		if (Constants.RECEIPT_OPTION_ENABLED == false) return;

		// 領収書希望
		rblReceiptFlg.Items.AddRange(
			ValueText.GetValueItemArray(Constants.TABLE_ORDER, Constants.FIELD_ORDER_RECEIPT_FLG));
		rblReceiptFlg.SelectedValue = this.OrderInput.ReceiptFlg;

		// 領収書出力
		rblReceiptOutputFlg.Items.AddRange(
			ValueText.GetValueItemArray(Constants.TABLE_ORDER, Constants.FIELD_ORDER_RECEIPT_OUTPUT_FLG));
		rblReceiptOutputFlg.SelectedValue = this.OrderInput.ReceiptOutputFlg;
	}

	/// <summary>
	/// コンポーネント初期化 決済
	/// </summary>
	private void InitializeComponentsPayment()
	{
		// 外部決済連携
		ddlOldExecuteType.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ORDER, "execute_types"));
		ddlNewExecuteType.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ORDER, "execute_types"));
		ddlOldExecuteType.Enabled = ddlNewExecuteType.Enabled = false;
		ddlOldExecuteType.Visible = ddlNewExecuteType.Visible = false;
		lbOldExternalPayment.Text = lbNewExternalPayment.Text = string.Empty;

		var paymentSelectionFlg = string.Empty;
		var permittedPaymentIds = string.Empty;
		if (this.ShopShipping != null)
		{
			paymentSelectionFlg = this.ShopShipping.PaymentSelectionFlg;
			permittedPaymentIds = this.ShopShipping.PermittedPaymentIds;
		}

		var payments = GetPaymentValidListPermission(
			this.LoginOperatorShopId,
			paymentSelectionFlg,
			permittedPaymentIds);
		if (OrderCommon.CheckPaymentYamatoKaSms(this.OrderOld.OrderPaymentKbn) == false)
		{
			payments = payments.Where(item => (OrderCommon.CheckPaymentYamatoKaSms(item.PaymentId) == false)).ToArray();
		}
		// 決済種別が存在しない、かつ配送種別の変更が不可の場合はエラー
		if ((payments.Length == 0) && (this.CanSelectShipping == false))
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] =
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PAYMENT_NO_DATA);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		//督促ステータス、督促日の表示
		if (Constants.DEMAND_OPTION_ENABLE && ShowDisplayDemandStatus(this.OrderInput.OrderPaymentKbn))
		{
			trDemandStatus.Visible = true;
			trDemandDay.Visible = true;
		}

		// Remove invalid payment for store pickup
		if (string.IsNullOrEmpty(this.OrderInputOld.Shippings[0].StorePickupRealShopId) == false)
		{
			payments = GetValidStorePickupPayments();
		}

		ddlOrderPaymentKbn.Items.Add(new ListItem("", ""));
		foreach (var payment in payments)
		{
			if ((this.OrderInputOld.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
				&& (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)) continue;

			if ((this.OrderInputOld.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_RAKUTEN_ID_SBPS)
				&& (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_RAKUTEN_ID_SBPS)) continue;

			if ((this.OrderInputOld.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
				&& (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY))
			{
				continue;
			}

			if ((this.OrderInputOld.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
				&& (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY))
			{
				continue;
			}

			if ((this.OrderInputOld.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA)
				&& (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA))
			{
				continue;
			}

			ddlOrderPaymentKbn.Items.Add(
				new ListItem(payment.PaymentName, payment.PaymentId));
		}

		if (payments.Any(payment => (payment.PaymentId == this.OrderOld.OrderPaymentKbn)) == false)
		{
			var oldPayment = new PaymentModel(GetPayment(this.LoginOperatorShopId, this.OrderOld.OrderPaymentKbn));
			ddlOrderPaymentKbn.Items.Add(new ListItem(oldPayment.PaymentName, oldPayment.PaymentId));
		}

		ControlPaymentWhenShippingConvenience();

		InitializeComponentsPaymentCreditCard();
	}

	/// <summary>
	/// コンポーネント初期化 決済(クレジットカード)
	/// </summary>
	private void InitializeComponentsPaymentCreditCard()
	{
		if (OrderCommon.CreditCompanySelectable)
		{
			ddlCreditCardCompany.Items.AddRange(
				ValueText.GetValueItemArray(Constants.TABLE_ORDER, OrderCommon.CreditCompanyValueTextFieldName));
		}

		ddlCreditExpireMonth.Items.AddRange(this.CreditExpirationMonthListItems);
		ddlCreditExpireMonth.SelectedValue = DateTime.Now.Month.ToString("00");
		ddlCreditExpireYear.Items.AddRange(this.CreditExpirationYearListItems);
		ddlCreditExpireYear.SelectedValue = DateTime.Now.Year.ToString("00")
			.Substring((Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.SBPS) ? 0 : 2);
		// カード分割支払い
		trInstallments.Visible = OrderCommon.CreditInstallmentsSelectable;
		if (OrderCommon.CreditInstallmentsSelectable)
		{
			dllCreditInstallments.Items.AddRange(
				ValueText.GetValueItemArray(Constants.TABLE_ORDER, OrderCommon.CreditInstallmentsValueTextFieldName));
		}

		// カードセキュリティコード表示・非表示判定
		trSecurityCode.Visible = OrderCommon.CreditSecurityCodeEnable && (this.IsUserPayTg == false);
		// 登録クレジットカードセット
		this.CreditCardNum = 0;
		if (this.IsUser)
		{
			var creditCardList = UserCreditCard.GetUsable(this.OrderInput.UserId);

			// つくーる連携、EScottにて通常注文時再与信不可のカードが登録される為除いて表示
			var creditCardReAuthValidList = creditCardList
				.Where(ccl => IsReAuthPossibleCard(ccl)).ToArray();

			this.CreditCardNum = creditCardReAuthValidList.Length;
			ddlUserCreditCard.Items.AddRange(
				creditCardReAuthValidList.Select(cc => new ListItem(cc.CardDispName, cc.BranchNo.ToString())).ToArray());
			var userCreditCard = string.IsNullOrEmpty(this.OrderInput.CreditBranchNo) == false
				? UserCreditCard.Get(this.OrderInput.UserId, int.Parse(this.OrderInput.CreditBranchNo))
				: null;
			// 未登録カードで決済した場合、「未登録の決済カード」オプション追加する
			if ((string.IsNullOrEmpty(this.OrderInputOld.CreditBranchNo) == false)
				&& (ddlUserCreditCard.Items.FindByValue(this.OrderInputOld.CreditBranchNo) == null)
				&& (IsReAuthPossibleCard(userCreditCard)
					|| this.OrderInputOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT
						&& IsReAuthPossibleCard(userCreditCard) == false))
			{
				ddlUserCreditCard.Items.Add(
					new ListItem(
						Constants.CREDITCARD_UNREGIST_DEFAULT_DISPLAY_NAME,
						this.OrderInputOld.CreditBranchNo));
			}
		}
		// ゲストユーザーがクレジットカードで決済した場合、「未登録の決済カード」オプション追加する
		else if ((string.IsNullOrEmpty(this.OrderInputOld.CreditBranchNo) == false)
			&& (ddlUserCreditCard.Items.FindByValue(this.OrderInputOld.CreditBranchNo) == null))
		{
			ddlUserCreditCard.Items.Add(
				new ListItem(Constants.CREDITCARD_UNREGIST_DEFAULT_DISPLAY_NAME, this.OrderInputOld.CreditBranchNo));
		}

		if ((Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.Rakuten) || this.IsUserPayTg)
		{
			ddlUserCreditCard.Items.Add(
				new ListItem(
					ReplaceTag("@@DispText.credit_card_list.new@@"),
					CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW));
		}
		ddlUserCreditCard.SelectedIndex = 0;
		ddlUserCreditCard.DataBind();
		trRegistCreditCard.Visible
			= trCreditCardName.Visible
				= (this.IsUser && (Constants.MAX_NUM_REGIST_CREDITCARD > this.CreditCardNum));
	}

	/// <summary>
	/// コンポーネント初期化 電子発票
	/// </summary>
	private void InitializeComponentsTwInvoice()
	{
		ddlCarryType.Items.AddRange(
			ValueText.GetValueItemArray(Constants.TABLE_TWORDERINVOICE, Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE));
		ddlUniformInvoice.Items.AddRange(
			ValueText.GetValueItemArray(
				Constants.TABLE_TWORDERINVOICE,
				Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE));
		ddlUniformInvoiceOrCarryTypeOption.Items.Add(
			new ListItem(ReplaceTag("@@DispText.uniform_invoice_option.new@@"), string.Empty));
	}

	/// <summary>
	/// コンポーネント初期化 PayTG
	/// </summary>
	private void InitializeComponentsPayTg()
	{
		if (this.IsUserPayTg
			&& string.IsNullOrEmpty(tbCreditCardNo1.Text)
			&& (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans))
		{
			tbCreditCardNo1.Text = Constants.PAYMENT_SETTING_PAYTG_DEFAULT_CARD_NUMBER;
		}
		this.IsSuccessfulCardRegistration = false;
		tdCreditNumber.Visible = (this.IsUserPayTg == false);
		trCreditExpire.Visible = (this.IsUserPayTg == false);
		tdGetCardInfo.Visible = this.IsUserPayTg;
		this.phCreditCardNotRakuten.Visible = (this.IsNotRakutenAgency
			|| (this.IsUserPayTg && Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten));
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

		tbCarryTypeOption1.Visible = this.IsMobile;
		tbCarryTypeOption2.Visible = this.IsCertificate;
	}

	/// <summary>
	/// コンポーネントに値セット
	/// </summary>
	private void SetValueToComponents()
	{
		// 注文基本情報セット（基本はデータバインド）
		// 拡張ステータス・更新日データソースセット
		rOrderExtendStatusList.DataSource
			= rOrderExtendStatusDates.DataSource
			= GetOrderExtendStatusSettingList(this.LoginOperatorShopId);

		// Get gmo cvs type
		GetGmoCvsType();

		// 注文区分
		foreach (ListItem item in ddlOrderKbn.Items)
		{
			item.Selected = (this.OrderInput.OrderKbn == item.Value);
		}

		// 配送料別途見積もり表示が有効(見積もり中)の場合に配送料金フィールドを触れれないようにする
		tbOrderPriceShipping.Enabled = (
			(Constants.SHIPPINGPRICE_SEPARATE_ESTIMATE_ENABLED == false)
			|| (this.ShopShipping == null)
			|| (this.ShopShipping.ShippingPriceSeparateEstimatesFlg == Constants.FLG_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG_INVALID));

		// 外部連携受注ID
		tbExternalOrderId.Text = this.OrderInput.ExternalOrderId;

		SetValueToComponentsOwner();

		// 自動計算適用
		cbApplyAutoCalculation.Checked = Constants.ORDER_APPLYAUTOCALCULATION_DEFAULT;
		if (this.OrderInput.HasRealStockReservedProduct)
		{
			// 引当て済み商品があった場合は利用不可とする
			cbApplyAutoCalculation.Checked = false;
			cbApplyAutoCalculation.Enabled = false;
		}

		// ユーザー管理レベルセット
		var li = ddlUserManagementLevel.Items.FindByValue(this.User.UserManagementLevelId);
		if (li != null) li.Selected = true;
		tbUserMemo.Text = this.User.UserMemo;

		// 利用可能な決済種別か？チェック
		DisplayPaymentUserManagementLevel(this.LoginOperatorShopId, ddlUserManagementLevel.SelectedValue);

		DisplayPaymentOrderOwnerKbn(this.LoginOperatorShopId, this.OrderInput.Owner.OwnerKbn);

		SetValueToComponentsPayment();

		SetValueToComponentsOrderPoint();

		SetValueToComponentsTwOrderInvoice();

		// 返品交換都合区分
		foreach (ListItem item in rblReturnExchangeReasonKbn.Items)
		{
			item.Selected = (this.OrderInput.ReturnExchangeReasonKbn == item.Value);
		}

		// ポイント、クーポン情報表示
		if (this.OrderInput.OrderIdOrg != "")
		{
			divPoint.Visible = true;
			divCoupon.Visible = false;
		}

		// 広告コード（初回分）
		tbAdvCodeFirst.Text = this.OrderInput.AdvcodeFirst;

		// 広告コード（最新分）
		tbAdvCodeNew.Text = this.OrderInput.AdvcodeNew;

		// 返品交換理由メモ
		tbReturnExchangeReasonMemo.Text = this.OrderInput.ReturnExchangeReasonMemo;

		// 返金メモ
		tbRepaymentMemo.Text = this.OrderInput.RepaymentMemo;

		// 会員ランク割引額取得
		lMemberRankDiscountDisp.Text = this.OrderInput.MemberRankDiscountPrice.ToPriceString(true);

		// 定期会員割引額取得
		lFixedPurchaseMemberDiscount.Text = this.OrderInput.FixedPurchaseMemberDiscountAmount.ToPriceString(true);

		// 配送種別
		if (this.CanSelectShipping)
		{
			ddlShipping.SelectedValue = this.OrderInput.ShippingId;
		}

		// 付与ポイント
		this.OrderPointAdd = this.OrderInput.OrderPointAdd;

		// 注文配送先エラー表示
		if (this.OrderShippingErrorMessages.Length != 0)
		{
			lbTopErrorMessages.Text = this.OrderShippingErrorMessages.ToString();
		}

		this.OrderShippingErrorMessages.Clear();
	}

	/// <summary>
	/// コンポーネントに値セット 注文者
	/// </summary>
	private void SetValueToComponentsOwner()
	{
		// 生年月日
		if (this.OrderInput.Owner.OwnerBirth != null)
		{
			ucOwnerBirth.SelectedDate = DateTime.Parse(this.OrderInput.Owner.OwnerBirth);
		}

		// 注文者都道府県選択
		foreach (ListItem li in ddlOwnerAddr1.Items)
		{
			li.Selected = (this.OrderInput.Owner.OwnerAddr1 == li.Value);
		}

		// 性別
		foreach (ListItem li in rblOwnerSex.Items)
		{
			li.Selected = (this.OrderInput.Owner.OwnerSex == li.Value);
		}

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			ddlAccessCountryIsoCode.SelectedValue = this.OrderInput.Owner.AccessCountryIsoCode;
			lLanguageCode.Text = this.OrderInput.Owner.DispLanguageCode;
			ddlDispLanguageLocaleId.SelectedValue = this.OrderInput.Owner.DispLanguageLocaleId;
			lCurrencyCode.Text = this.OrderInput.Owner.DispCurrencyCode;
			ddlDispCurrencyLocaleId.SelectedValue = this.OrderInput.Owner.DispCurrencyLocaleId;
			ddlOwnerCountry.SelectedValue = this.OrderInput.Owner.OwnerAddrCountryIsoCode;
			if (this.IsOwnerAddrUs)
			{
				ddlOwnerAddr5.SelectedValue = this.OrderInput.Owner.OwnerAddr5;
			}
			else
			{
				tbOwnerAddr5.Text = this.OrderInput.Owner.OwnerAddr5;
			}
		}
	}

	/// <summary>
	/// コンポーネントに値セット　決済
	/// </summary>
	private void SetValueToComponentsPayment()
	{
		// 決済種別情報選択
		foreach (ListItem li in ddlOrderPaymentKbn.Items)
		{
			li.Selected = (this.OrderInputOld.OrderPaymentKbn == li.Value);
		}

		if (ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
		{
			if (string.IsNullOrEmpty(this.OrderInputOld.CreditBranchNo) == false)
			{
				foreach (ListItem li in ddlUserCreditCard.Items)
				{
					li.Selected = (li.Value == this.OrderInputOld.CreditBranchNo);
				}
			}

			// 支払い回数コード設定
			if ((string.IsNullOrEmpty(this.OrderInputOld.CardInstallmentsCode) == false)
				&& dllCreditInstallments.Items.FindByValue(this.OrderInputOld.CardInstallmentsCode) != null)
			{
				dllCreditInstallments.SelectedValue = this.OrderInputOld.CardInstallmentsCode;
			}
		}

		// AmazonPayが選択されていない場合はリストから削除
		if (ddlOrderPaymentKbn.SelectedValue != Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT)
		{
			ddlOrderPaymentKbn.Items.Remove(
				ddlOrderPaymentKbn.Items.FindByValue(Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT));
		}

		// atone翌月払いが選択されていない場合はリストから削除
		if (ddlOrderPaymentKbn.SelectedValue != Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)
		{
			ddlOrderPaymentKbn.Items.Remove(
				ddlOrderPaymentKbn.Items.FindByValue(Constants.FLG_PAYMENT_PAYMENT_ID_ATONE));
		}

		// aftee翌月払いが選択されていない場合はリストから削除
		if (ddlOrderPaymentKbn.SelectedValue != Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE)
		{
			ddlOrderPaymentKbn.Items.Remove(
				ddlOrderPaymentKbn.Items.FindByValue(Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE));
		}

		// 藍新Payが選択されていない場合はリストから削除
		if (ddlOrderPaymentKbn.SelectedValue != Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
		{
			ddlOrderPaymentKbn.Items.Remove(
				ddlOrderPaymentKbn.Items.FindByValue(Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY));
		}

		var regKey = Constants.PAYMENT_LINEPAY_OPTION_ENABLED
			? new UserService().GetUserExtend(this.OrderInputOld.UserId)
				.UserExtendDataValue[Constants.LINEPAY_USEREXRTEND_COLUMNNAME_REGKEY]
			: string.Empty;

		// Hide payment Line Pay if User does not have Reg Key
		if ((ddlOrderPaymentKbn.SelectedValue != Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY)
			&& string.IsNullOrEmpty(regKey))
		{
			ddlOrderPaymentKbn.Items.Remove(
				ddlOrderPaymentKbn.Items.FindByValue(Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY));
		}

		// Remove Payment Np After Pay If Has Digital Contents
		if ((ddlOrderPaymentKbn.SelectedValue != Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
			&& this.OrderInput.IsDigitalContents)
		{
			ddlOrderPaymentKbn.Items.Remove(
				ddlOrderPaymentKbn.Items.FindByValue(Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY));
		}

		// Paypayが選択されていない場合はリストから削除
		if (ddlOrderPaymentKbn.SelectedValue != Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
		{
			var payPayPaymentKbn = ddlOrderPaymentKbn.Items.FindByValue(Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY);
			if (payPayPaymentKbn != null) ddlOrderPaymentKbn.Items.Remove(payPayPaymentKbn);
		}

		// 注文者都道府県選択
		foreach (ListItem li in ddlOwnerAddr1.Items)
		{
			li.Selected = (this.OrderInput.Owner.OwnerAddr1 == li.Value);
		}

		// Remove Boku Payment
		if (ddlOrderPaymentKbn.SelectedValue != Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU)
		{
			ddlOrderPaymentKbn.Items.Remove(
				ddlOrderPaymentKbn.Items.FindByValue(Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU));
		}
	}

	/// <summary>
	/// コンポーネントに値セット 付与ポイント
	/// </summary>
	private void SetValueToComponentsOrderPoint()
	{
		if (Constants.W2MP_POINT_OPTION_ENABLED == false) return;

		trOrderPointErrorMessagesTitle.Visible = false;
		trOrderPointErrorMessages.Visible = false;
		trReCalculatePoint.Visible = false;
		trOrderPointUse.Visible = false;
		trOrderPointAddBasePoint.Visible = false;
		rOrderPointAddTempOrLimitedTermComp.Visible = false;

		// 会員ユーザー？
		if (this.IsUser)
		{
			// 利用ポイントセット
			tbOrderPointUse.Text = this.OrderInput.OrderPointUse;
			// 利用可能ポイントセット
			var userUsablePoint = new PointService().GetUserPointUsableTotal(this.User.UserId)
				+ int.Parse(this.OrderInput.OldOrderPointUse);
			lUserPoint.Text = WebSanitizer.HtmlEncode(StringUtility.ToNumeric(userUsablePoint));

			lLastOrderPointUseBefore.Text = this.OrderOld.LastOrderPointUse.ToString();
			trLastOrderPointUseBefore.Visible = (this.OrderOld.IsNotReturnExchangeOrder == false);

			// 表示制御
			trReCalculatePoint.Visible = true;
			trOrderPointUse.Visible = true;

			// 注文に紐づいたユーザポイント(仮ポイント／期間限定ポイント)が存在する？
			if (this.UserPointRelatedThisOrder.Items.Count > 0)
			{
				rOrderPointAddTempOrLimitedTermComp.Visible = true;
			}

			// 購入時には仮ポイントと本ポイントが同時に付与されない仕様で、付与された通常本ポイントを計算
			var addedBasePointComp = (decimal.Parse(this.OrderInput.OrderPointAdd) > 0)
				? (decimal.Parse(this.OrderInput.OrderPointAdd)
					- this.UserPointRelatedThisOrder.Items.Sum(up => up.Point))
				: 0m;
			this.AddedBasePointCompOld = StringUtility.ToNumeric(addedBasePointComp);
			tbOrderPointAddBaseComp.Text = this.AddedBasePointCompOld;
			trOrderPointAddBasePoint.Visible = (this.UserPointList.UserPointTemp.Length == 0);
		}
	}

	/// <summary>
	/// コンポーネントに値をセット　電子発票
	/// </summary>
	protected void SetValueToComponentsTwOrderInvoice()
	{
		if (OrderCommon.DisplayTwInvoiceInfo())
		{
			var canEdit = OrderCommon.TwInvoiceStatusCanEnableControl(this.TwOrderInvoice, Constants.TWINVOICE_ENABLED);

			// Set edit invoice data when modify order
			SetEditTwInvoiceDataWhenModifyOrder(canEdit);
		}

		if (OrderCommon.DisplayTwInvoiceInfo(this.OrderInput.Shippings[0].ShippingCountryIsoCode))
		{
			if (OrderCommon.TwInvoiceStatusCanNotEditOrder(this.TwOrderInvoice, Constants.TWINVOICE_ENABLED))
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			// Set taiwan invoice data when modify order
			SetTwInvoiceDataWhenModifyOrder();
		}

	}

	#region フォーム取得処理
	/// <summary>
	/// 注文入力情報取得
	/// </summary>
	/// <returns>注文入力情報</returns>
	private OrderInput GetOrderInput()
	{
		var liOrderPaymentKbn = GetSelectedOrderPaymentListItem(this.OrderInputOld.MallId);

		// 注文
		var order = new OrderInput(this.OrderInputOld.CreateModel())
		{
			OrderPaymentKbn = liOrderPaymentKbn.Value,
			PaymentName = liOrderPaymentKbn.Text,
			CardTranId = (Constants.CARDTRANIDOPTION_ENABLED)
				? StringUtility.RemoveUnavailableControlCode(tbOrderCardTranId.Text)
				: this.OrderInput.CardTranId,
			PaymentOrderId = tbPaymentOrderId.Text,
			OrderPriceShipping = tbOrderPriceShipping.Text,
			OrderPriceExchange = tbOrderPriceExchange.Text,
			MemberRankDiscountPrice = (Constants.MEMBER_RANK_OPTION_ENABLED) ? tbMemberRankDiscount.Text : "0",
			FixedPurchaseDiscountPrice = (Constants.FIXEDPURCHASE_OPTION_ENABLED) ? tbFixedPurchaseDiscountPrice.Text : "0",
			FixedPurchaseMemberDiscountAmount
				= (Constants.FIXEDPURCHASE_OPTION_ENABLED && Constants.MEMBER_RANK_OPTION_ENABLED) ? tbFixedPurchaseMemberDiscount.Text : "0",
			OrderPointUse = "0",
			OrderPointUseYen = "0",
			OrderPointAdd = "0",
			OrderCouponUse = "0",
			OrderPriceRegulation = tbOrderPriceRegulation.Text,
			RegulationMemo = StringUtility.RemoveUnavailableControlCode(tbRegulationMemo.Text),
			Memo = StringUtility.RemoveUnavailableControlCode(tbMemo.Text),
			PaymentMemo = StringUtility.RemoveUnavailableControlCode(tbPaymentMemo.Text),
			ManagementMemo = StringUtility.RemoveUnavailableControlCode(tbManagementMemo.Text),
			ShippingMemo = StringUtility.RemoveUnavailableControlCode(tbShippingMemo.Text),
			RelationMemo = StringUtility.RemoveUnavailableControlCode(tbRelationMemo.Text),
			ShippingPriceSeparateEstimatesFlg = (Constants.SHIPPINGPRICE_SEPARATE_ESTIMATE_ENABLED && cbShippingPriceSeparateEstimateFlg.Checked)
			? Constants.FLG_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG_VALID
			: Constants.FLG_SHOPSHIPPING_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG_INVALID,
			OrderKbn = ddlOrderKbn.SelectedValue,
			ReturnExchangeReasonKbn = String.IsNullOrEmpty(rblReturnExchangeReasonKbn.SelectedValue)
			? this.OrderInputOld.ReturnExchangeReasonKbn
			: rblReturnExchangeReasonKbn.SelectedValue,
			ReturnExchangeReasonMemo = StringUtility.RemoveUnavailableControlCode(tbReturnExchangeReasonMemo.Text),
			RepaymentMemo = StringUtility.RemoveUnavailableControlCode(tbRepaymentMemo.Text),
			AdvcodeFirst = tbAdvCodeFirst.Text.Trim(),
			AdvcodeNew = tbAdvCodeNew.Text.Trim(),
			ShippingId = (this.CanSelectShipping) ? ddlShipping.SelectedValue : this.OrderInputOld.ShippingId,
			ExternalOrderId = tbExternalOrderId.Text.Trim(),
			OldLastBilledAmount = this.OrderOld.LastBilledAmount.ToString(),
			ShippingTaxRate = this.OrderInputOld.ShippingTaxRate,
			PaymentTaxRate = this.OrderInputOld.PaymentTaxRate,
			OldLastOrderPointUse = this.OrderOld.LastOrderPointUse.ToString()
		};

		// 領収書情報設定
		if (Constants.RECEIPT_OPTION_ENABLED)
		{
			order.ReceiptFlg = rblReceiptFlg.SelectedValue;
			order.ReceiptOutputFlg = rblReceiptOutputFlg.SelectedValue;
			order.ReceiptAddress = StringUtility.RemoveUnavailableControlCode(tbReceiptAddress.Text);
			order.ReceiptProviso = StringUtility.RemoveUnavailableControlCode(tbReceiptProviso.Text);
		}

		if (Constants.ORDER_EXTEND_OPTION_ENABLED)
		{
			order.OrderExtendInput = CreateOrderExtendFromInputData(rOrderExtendInput);
		}

		if ((ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
			&& (ddlUserCreditCard.SelectedValue != CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW))
		{
			order.CreditBranchNo = ddlUserCreditCard.SelectedValue;
		}

		if (order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_DSK_DEF)
		{
			order.PaymentOrderId = (order.OrderId.Length >= 10)
				? order.OrderId.Substring(order.OrderId.Length - 10)
				: order.OrderId.PadRight(10, '0');
		}

		// 注文者
		order.Owner = GetOrderOwnerInput();
		// 注文配送先＆注文商品
		order.Shippings = GetOrderShippingsInput();
		// 注文セットプロモーション
		order.SetPromotions = GetOrderSetPromotionsInput(order);
		// 注文クーポン
		order.Coupons = GetOrderCouponInput(order);

		// Change store pickup status if shipping type or store pickup is changed
		if (string.IsNullOrEmpty(order.Shippings[0].StorePickupRealShopId))
		{
			order.StorePickupStatus = string.Empty;
			order.StorePickupStoreArrivedDate = string.Empty;
			order.StorePickupDeliveredCompleteDate = string.Empty;
			order.StorePickupReturnDate = string.Empty;
		}
		else if (this.OrderInputOld.Shippings[0].StorePickupRealShopId
			!= order.Shippings[0].StorePickupRealShopId)
		{
			order.StorePickupStatus = Constants.FLG_ORDER_ORDER_STORE_PICKUP_STATUS_PENDING;
			order.StorePickupStoreArrivedDate = string.Empty;
			order.StorePickupDeliveredCompleteDate = string.Empty;
			order.StorePickupReturnDate = string.Empty;
		}

		// 税率毎価格情報
		order.OrderPriceByTaxRates = rPriceCorrection.Items.Cast<RepeaterItem>().Select(
			ri => new OrderPriceByTaxRateInput()
			{
				KeyTaxRate = ((HiddenField)ri.FindControl("hfTaxRate")).Value,
				PriceCorrectionByRate = ((TextBox)ri.FindControl("tbPriceCorrection")).Text,
			}).ToArray();

		// 宅配通配送実績情報
		if (Constants.TWPELICAN_COOPERATION_EXTEND_ENABLED)
		{
			order.Shippings[0].ShippingStatusUpdateDate = this.OrderInputOld.Shippings[0].ShippingStatusUpdateDate;
			order.Shippings[0].ShippingHandyTime = this.OrderInputOld.Shippings[0].ShippingHandyTime;
			order.Shippings[0].ShippingOfficeName = this.OrderInputOld.Shippings[0].ShippingOfficeName;
			order.Shippings[0].ShippingStatus = this.OrderInputOld.Shippings[0].ShippingStatus;
			order.Shippings[0].ShippingCurrentStatus = this.OrderInputOld.Shippings[0].ShippingCurrentStatus;
			order.Shippings[0].ShippingStatusCode = this.OrderInputOld.Shippings[0].ShippingStatusCode;
			order.Shippings[0].ShippingStatusDetail = this.OrderInputOld.Shippings[0].ShippingStatusDetail;
		}

		return order;
	}

	/// <summary>
	/// 注文者入力情報取得
	/// </summary>
	/// <returns>注文者入力情報</returns>
	private OrderOwnerInput GetOrderOwnerInput()
	{
		var orderOwner = new OrderOwnerInput
		{
			OrderId = this.OrderInput.OrderId,
			OwnerName1 = tbOwnerName1.Text,
			OwnerName2 = tbOwnerName2.Text,
			OwnerNameKana1 = tbOwnerNameKana1.Text,
			OwnerNameKana2 = tbOwnerNameKana2.Text,
			OwnerTel1_1 = tbOwnerTel1_1.Text,
			OwnerTel1_2 = tbOwnerTel1_2.Text,
			OwnerTel1_3 = tbOwnerTel1_3.Text,
			OwnerMailAddr = tbOwnerMailAddr.Text,
			OwnerMailAddr2 = Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED
				? tbOwnerMailAddr2.Text
				: this.OrderInput.Owner.OwnerMailAddr2,
			OwnerZip1 = tbOwnerZip1_1.Text,
			OwnerZip2 = tbOwnerZip1_2.Text,
			OwnerAddr1 = ddlOwnerAddr1.SelectedValue,
			OwnerAddr2 = tbOwnerAddr2.Text.Trim(),
			OwnerAddr3 = tbOwnerAddr3.Text.Trim(),
			OwnerAddr4 = tbOwnerAddr4.Text.Trim(),
			OwnerCompanyName = tbOwneCompanyName.Text,
			OwnerCompanyPostName = tbOwnerCompanyPostName.Text,
			OwnerSex = rblOwnerSex.SelectedValue,
			UserManagementLevelId = ddlUserManagementLevel.SelectedValue,
			UserMemo = StringUtility.RemoveUnavailableControlCode(tbUserMemo.Text)
		};

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			orderOwner.AccessCountryIsoCode = ddlAccessCountryIsoCode.SelectedValue;
			orderOwner.DispLanguageCode = lLanguageCode.Text;
			orderOwner.DispLanguageLocaleId = ddlDispLanguageLocaleId.SelectedValue;
			orderOwner.DispCurrencyCode = lCurrencyCode.Text;
			orderOwner.DispCurrencyLocaleId = ddlDispCurrencyLocaleId.SelectedValue;
			orderOwner.OwnerAddrCountryIsoCode = ddlOwnerCountry.SelectedValue;
			orderOwner.OwnerAddrCountryName = ddlOwnerCountry.SelectedItem.Text;

			if (this.IsOwnerAddrJp == false)
			{
				orderOwner.OwnerZip = tbOwnerZipGlobal.Text;
				orderOwner.OwnerZip1 = string.Empty;
				orderOwner.OwnerZip2 = string.Empty;
				orderOwner.OwnerTel1 = tbOwnerTel1Global.Text;
				orderOwner.OwnerTel1_1 = string.Empty;
				orderOwner.OwnerTel1_2 = string.Empty;
				orderOwner.OwnerTel1_3 = string.Empty;
				orderOwner.OwnerAddr5 = this.IsOwnerAddrUs ? ddlOwnerAddr5.SelectedValue : tbOwnerAddr5.Text.Trim();
			}
		}
		// 生年月日が正常な日付の場合
		var birth = ucOwnerBirth.DateString;
		if (Validator.IsDate(birth))
		{
			orderOwner.OwnerBirth = birth;
		}
		return orderOwner;
	}

	/// <summary>
	/// 注文配送先入力情報取得
	/// </summary>
	/// <returns>注文配送先入力情報</returns>
	private OrderShippingInput[] GetOrderShippingsInput()
	{
		var orderShippingNo = 0;
		var orderShippings = new List<OrderShippingInput>();
		var orderShipping = new OrderShippingInput();
		foreach (RepeaterItem ri in rShippingList.Items)
		{
			orderShippingNo++;
			var ddlUserShipping = (DropDownList)ri.FindControl("ddlUserShipping");
			var controlShippingDate = (DateTimePickerPeriodInputControl)ri.FindControl("ucShippingDate");
			var controlScheduledShippingDate = (DateTimePickerPeriodInputControl)ri.FindControl("ucScheduledShippingDate");
			var scheduledShippingDate = controlScheduledShippingDate.StartDateTimeString;
			var shippingDate = controlShippingDate.StartDateTimeString;
			var ddlRealStore = (DropDownList)ri.FindControl("ddlRealStore");
			switch (ddlUserShipping.SelectedValue)
			{
				case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW:
					orderShipping = new OrderShippingInput
					{
						OrderId = this.OrderInput.OrderId,
						OrderShippingNo = orderShippingNo.ToString(),
						ShippingName1 = ((TextBox)ri.FindControl("tbShippingName1")).Text,
						ShippingName2 = ((TextBox)ri.FindControl("tbShippingName2")).Text,
						ShippingNameKana1 = ((TextBox)ri.FindControl("tbShippingNameKana1")).Text,
						ShippingNameKana2 = ((TextBox)ri.FindControl("tbShippingNameKana2")).Text,
						ShippingTel1_1 = ((TextBox)ri.FindControl("tbShippingTel1_1")).Text,
						ShippingTel1_2 = ((TextBox)ri.FindControl("tbShippingTel1_2")).Text,
						ShippingTel1_3 = ((TextBox)ri.FindControl("tbShippingTel1_3")).Text,
						ShippingZip1 = ((TextBox)ri.FindControl("tbShippingZip1_1")).Text,
						ShippingZip2 = ((TextBox)ri.FindControl("tbShippingZip1_2")).Text,
						ShippingAddr1 = ((DropDownList)ri.FindControl("ddlShippingAddr1")).SelectedValue,
						ShippingAddr2 = ((TextBox)ri.FindControl("tbShippingAddr2")).Text.Trim(),
						ShippingAddr3 = ((TextBox)ri.FindControl("tbShippingAddr3")).Text.Trim(),
						ShippingAddr4 = ((TextBox)ri.FindControl("tbShippingAddr4")).Text.Trim(),
						ShippingCompanyName = ((TextBox)ri.FindControl("tbShippingCompanyName")).Text,
						ShippingCompanyPostName = ((TextBox)ri.FindControl("tbShippingCompanyPostName")).Text,
						ShippingTime = ((DropDownList)ri.FindControl("ddlShippingTime")).SelectedValue,
						ShippingTimeText = (((DropDownList)ri.FindControl("ddlShippingTime")).SelectedItem != null)
							? ((DropDownList)ri.FindControl("ddlShippingTime")).SelectedItem.Text
							: string.Empty,
						ShippingCheckNo = ((TextBox)ri.FindControl("tbShippingCheckNo")).Text,
						ShippingMethod = ((DropDownList)ri.FindControl("ddlShippingMethod")).Text,
						ExternalShipmentEntry = ((CheckBox)ri.FindControl("cbExecExternalShipmentEntry")).Checked,
						OldShippingCheckNo = ((HiddenField)ri.FindControl("hfOldShippingCheckNo")).Value,
						DeliveryCompanyId = ((DropDownList)ri.FindControl("ddlDeliveryCompany")).Text,
						ShippingReceivingStoreFlg = Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF,
						ShippingReceivingStoreId = string.Empty,
						ShippingExternalDelivertyStatus = string.Empty,
						ShippingStatus = string.Empty,
						ShippingStatusUpdateDate = string.Empty,
						ShippingReceivingMailDate = string.Empty,
						StorePickupRealShopId = string.Empty
					};
					break;

				case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER:
					orderShipping = new OrderShippingInput
					{
						OrderId = this.OrderInput.OrderId,
						OrderShippingNo = orderShippingNo.ToString(),
						ShippingName1 = tbOwnerName1.Text,
						ShippingName2 = tbOwnerName2.Text,
						ShippingCountryName = ((ddlOwnerCountry.SelectedItem != null)
							? ddlOwnerCountry.SelectedItem.Text
							: string.Empty),
						ShippingCountryIsoCode = (string.IsNullOrEmpty(ddlOwnerCountry.SelectedValue) == false)
							? this.ShippingAvailableCountryList.FirstOrDefault(country
								=> country.CountryIsoCode == ddlOwnerCountry.SelectedValue).CountryIsoCode
							: string.Empty,
						ShippingNameKana1 = tbOwnerNameKana1.Text,
						ShippingNameKana2 = tbOwnerNameKana2.Text,
						ShippingTel1 = tbOwnerTel1Global.Text,
						ShippingTel1_1 = tbOwnerTel1_1.Text,
						ShippingTel1_2 = tbOwnerTel1_2.Text,
						ShippingTel1_3 = tbOwnerTel1_3.Text,
						ShippingZip = tbOwnerZipGlobal.Text,
						ShippingZip1 = tbOwnerZip1_1.Text,
						ShippingZip2 = tbOwnerZip1_2.Text,
						ShippingAddr1 = ddlOwnerAddr1.SelectedValue,
						ShippingAddr2 = tbOwnerAddr2.Text,
						ShippingAddr3 = tbOwnerAddr3.Text,
						ShippingAddr4 = tbOwnerAddr4.Text,
						ShippingAddr5 = this.IsOwnerAddrUs
							? ddlOwnerAddr5.SelectedValue
							: tbOwnerAddr5.Text,
						ShippingCompanyName = tbOwneCompanyName.Text,
						ShippingCompanyPostName = tbOwnerCompanyPostName.Text,
						ShippingTime = ((DropDownList)ri.FindControl("ddlShippingTime")).SelectedValue,
						ShippingTimeText = (((DropDownList)ri.FindControl("ddlShippingTime")).SelectedItem != null) ? ((DropDownList)ri.FindControl("ddlShippingTime")).SelectedItem.Text : string.Empty,
						ShippingCheckNo = ((TextBox)ri.FindControl("tbShippingCheckNo")).Text,
						ShippingMethod = ((DropDownList)ri.FindControl("ddlShippingMethod")).Text,
						DeliveryCompanyId = ((DropDownList)ri.FindControl("ddlDeliveryCompany")).Text,
						ExternalShipmentEntry = ((CheckBox)ri.FindControl("cbExecExternalShipmentEntry")).Checked,
						OldShippingCheckNo = ((HiddenField)ri.FindControl("hfOldShippingCheckNo")).Value,
						ShippingReceivingStoreFlg = Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF,
						ShippingReceivingStoreId = string.Empty,
						ShippingExternalDelivertyStatus = string.Empty,
						ShippingStatus = string.Empty,
						ShippingStatusUpdateDate = string.Empty,
						ShippingReceivingMailDate = string.Empty,
						StorePickupRealShopId = string.Empty,
						ShippingAddressKbn = CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER,
					};
					break;

				case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE:
					orderShipping.OrderShippingNo = orderShippingNo.ToString();
					GetConvenienceStoreShipping(orderShipping, ri);
					// 注文商品入力情報
					orderShipping.Items = GetOrderItemsInput(orderShippings, orderShipping, ri);
					orderShipping.StorePickupRealShopId = string.Empty;
					orderShippings.Add(orderShipping);
					continue;

				case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_STORE_PICKUP:
					var model = this.RealShopModels.FirstOrDefault(rs => rs.RealShopId == ddlRealStore.SelectedValue);
					if (model != null)
					{
						orderShipping = new OrderShippingInput
						{
							OrderId = this.OrderInput.OrderId,
							OrderShippingNo = orderShippingNo.ToString(),
							StorePickupRealShopId = model.RealShopId,
							ShippingName1 = model.Name,
							ShippingNameKana1 = model.NameKana,
							ShippingZip = model.Zip,
							ShippingAddr1 = model.Addr1,
							ShippingAddr2 = model.Addr2,
							ShippingAddr3 = model.Addr3,
							ShippingAddr4 = model.Addr4,
							ShippingAddr5 = model.Addr5,
							OpeningHours = model.OpeningHours,
							ShippingTel1_1 = model.Tel_1,
							ShippingTel1_2 = model.Tel_2,
							ShippingTel1_3 = model.Tel_3,
							ShippingZip1 = model.Zip1,
							ShippingZip2 = model.Zip2,
							ShippingCountryIsoCode = model.CountryIsoCode,
							ShippingCountryName = model.CountryName,
							ShippingCompanyName = string.Empty,
							ShippingCompanyPostName = string.Empty,
							DeliveryCompanyId = ((DropDownList)ri.FindControl("ddlDeliveryCompany")).Text,
							ShippingReceivingStoreFlg = Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF,
							ShippingAddressKbn = CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_STORE_PICKUP
						};
					}
					break;

				default:
					var userShipping = this.UserShippingAddress.FirstOrDefault(item => item.ShippingNo == int.Parse(ddlUserShipping.SelectedValue));
					if (userShipping != null)
					{
						if (userShipping.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF)
						{
							orderShipping = new OrderShippingInput
							{
								OrderId = this.OrderInput.OrderId,
								OrderShippingNo = orderShippingNo.ToString(),
								ShippingName1 = userShipping.ShippingName1,
								ShippingName2 = userShipping.ShippingName2,
								ShippingNameKana1 = userShipping.ShippingNameKana1,
								ShippingNameKana2 = userShipping.ShippingNameKana2,
								ShippingTel1 = userShipping.ShippingTel1,
								ShippingTel1_1 = userShipping.ShippingTel1_1,
								ShippingTel1_2 = userShipping.ShippingTel1_2,
								ShippingTel1_3 = userShipping.ShippingTel1_3,
								ShippingZip = userShipping.ShippingZip,
								ShippingZip1 = userShipping.ShippingZip1,
								ShippingZip2 = userShipping.ShippingZip2,
								ShippingAddr1 = userShipping.ShippingAddr1,
								ShippingAddr2 = userShipping.ShippingAddr2,
								ShippingAddr3 = userShipping.ShippingAddr3,
								ShippingAddr4 = userShipping.ShippingAddr4,
								ShippingCompanyName = userShipping.ShippingCompanyName,
								ShippingCompanyPostName = userShipping.ShippingCompanyPostName,
								ShippingTime = ((DropDownList)ri.FindControl("ddlShippingTime")).SelectedValue,
								ShippingTimeText = (((DropDownList)ri.FindControl("ddlShippingTime")).SelectedItem != null) ? ((DropDownList)ri.FindControl("ddlShippingTime")).SelectedItem.Text : string.Empty,
								ShippingCheckNo = ((TextBox)ri.FindControl("tbShippingCheckNo")).Text,
								ShippingMethod = ((DropDownList)ri.FindControl("ddlShippingMethod")).Text,
								DeliveryCompanyId = ((DropDownList)ri.FindControl("ddlDeliveryCompany")).Text,
								ExternalShipmentEntry = ((CheckBox)ri.FindControl("cbExecExternalShipmentEntry")).Checked,
								OldShippingCheckNo = ((HiddenField)ri.FindControl("hfOldShippingCheckNo")).Value,
								ShippingReceivingStoreFlg = Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF,
								ShippingReceivingStoreId = string.Empty,
								ShippingExternalDelivertyStatus = string.Empty,
								ShippingStatus = string.Empty,
								ShippingStatusUpdateDate = string.Empty,
								ShippingReceivingMailDate = string.Empty,
								ShippingCountryIsoCode = userShipping.ShippingCountryIsoCode,
								ShippingCountryName = userShipping.ShippingCountryName,
								ShippingDate = (shippingDate.Length > 2) ? shippingDate : null,
								ScheduledShippingDate = (scheduledShippingDate.Length > 2) ? scheduledShippingDate : string.Empty,
								StorePickupRealShopId = string.Empty
							};
							orderShipping.Items = GetOrderItemsInput(orderShippings, orderShipping, ri);

							orderShippings.Add(orderShipping);

							continue;
						}
						else
						{
							orderShipping.OrderShippingNo = orderShippingNo.ToString();
							GetConvenienceStoreShipping(orderShipping, ri, userShipping);

							// 注文商品入力情報
							orderShipping.Items = GetOrderItemsInput(orderShippings, orderShipping, ri);
							orderShipping.StorePickupRealShopId = string.Empty;
							orderShippings.Add(orderShipping);
							continue;
						}
					}
					break;
			}

			// 配送希望日指定有？
			orderShipping.ShippingDate = ((shippingDate.Length > 2) && orderShipping.IsExpress)
				? shippingDate
				: null;

			// Scheduled shipping date
			orderShipping.ScheduledShippingDate = (scheduledShippingDate.Length > 2)
				? scheduledShippingDate
				: string.Empty;

			var isShippingSameAsOwner = (orderShipping.ShippingAddressKbn == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER);

			if (Constants.GLOBAL_OPTION_ENABLE
				&& (isShippingSameAsOwner == false)
				&& (orderShipping.ShippingAddressKbn != CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_STORE_PICKUP))
			{
				orderShipping.ShippingCountryName = ((DropDownList)ri.FindControl("ddlShippingCountry")).SelectedItem.Text;
				orderShipping.ShippingCountryIsoCode = (string.IsNullOrEmpty(orderShipping.ShippingCountryName) == false)
					? this.ShippingAvailableCountryList.FirstOrDefault(c => c.CountryName == orderShipping.ShippingCountryName).CountryIsoCode
					: string.Empty;

				if (orderShipping.IsShippingAddrJp == false)
				{
					orderShipping.ShippingZip = ((TextBox)ri.FindControl("tbShippingZipGlobal")).Text;
					orderShipping.ShippingZip1 = string.Empty;
					orderShipping.ShippingZip2 = string.Empty;
					orderShipping.ShippingTel1 = ((TextBox)ri.FindControl("tbShippingTel1Global")).Text;
					orderShipping.ShippingTel1_1 = string.Empty;
					orderShipping.ShippingTel1_2 = string.Empty;
					orderShipping.ShippingTel1_3 = string.Empty;
					orderShipping.ShippingAddr5 = orderShipping.IsShippingAddrUs
						? ((DropDownList)ri.FindControl("ddlShippingAddr5")).SelectedValue
						: ((TextBox)ri.FindControl("tbShippingAddr5")).Text.Trim();

					orderShipping.ShippingAddr1 = string.Empty;
					orderShipping.ShippingNameKana1 = string.Empty;
					orderShipping.ShippingNameKana2 = string.Empty;
				}
			}

			// ギフト？
			if ((Constants.GIFTORDER_OPTION_ENABLED) && (this.OrderInput.GiftFlg == Constants.FLG_ORDER_GIFT_FLG_ON))
			{
				orderShipping.SenderName1 = ((TextBox)ri.FindControl("tbSenderName1")).Text;
				orderShipping.SenderName2 = ((TextBox)ri.FindControl("tbSenderName2")).Text;
				orderShipping.SenderNameKana1 = ((TextBox)ri.FindControl("tbSenderNameKana1")).Text;
				orderShipping.SenderNameKana2 = ((TextBox)ri.FindControl("tbSenderNameKana2")).Text;
				orderShipping.SenderTel1_1 = ((TextBox)ri.FindControl("tbSenderTel1_1")).Text;
				orderShipping.SenderTel1_2 = ((TextBox)ri.FindControl("tbSenderTel1_2")).Text;
				orderShipping.SenderTel1_3 = ((TextBox)ri.FindControl("tbSenderTel1_3")).Text;
				orderShipping.SenderZip1 = ((TextBox)ri.FindControl("tbSenderZip_1")).Text;
				orderShipping.SenderZip2 = ((TextBox)ri.FindControl("tbSenderZip_2")).Text;
				orderShipping.SenderAddr1 = ((DropDownList)ri.FindControl("ddlSenderAddr1")).SelectedValue;
				orderShipping.SenderAddr2 = ((TextBox)ri.FindControl("tbSenderAddr2")).Text.Trim();
				orderShipping.SenderAddr3 = ((TextBox)ri.FindControl("tbSenderAddr3")).Text.Trim();
				orderShipping.SenderAddr4 = ((TextBox)ri.FindControl("tbSenderAddr4")).Text.Trim();
				orderShipping.SenderCompanyName = ((TextBox)ri.FindControl("tbSenderCompanyName")).Text;
				orderShipping.SenderCompanyPostName = ((TextBox)ri.FindControl("tbSenderCompanyPostName")).Text;
				orderShipping.WrappingPaperType = ((DropDownList)ri.FindControl("ddlWrappingPaperType")).SelectedValue;
				orderShipping.WrappingPaperName = ((TextBox)ri.FindControl("tbWrappingPaperName")).Text;
				orderShipping.WrappingBagType = ((DropDownList)ri.FindControl("ddlWrappingBagType")).SelectedValue;

				if (Constants.GLOBAL_OPTION_ENABLE)
				{
					orderShipping.SenderCountryName = ((DropDownList)ri.FindControl("ddlSenderCountry")).SelectedItem.Text;
					orderShipping.SenderCountryIsoCode = (string.IsNullOrEmpty(orderShipping.SenderCountryName) == false)
						? this.UserCountryList.FirstOrDefault(c => c.CountryName == orderShipping.SenderCountryName).CountryIsoCode
						: string.Empty;

					if (orderShipping.IsSenderAddrJp == false)
					{
						orderShipping.SenderZip = ((TextBox)ri.FindControl("tbSenderZipGlobal")).Text;
						orderShipping.SenderZip1 = string.Empty;
						orderShipping.SenderZip2 = string.Empty;
						orderShipping.SenderTel1 = ((TextBox)ri.FindControl("tbSenderTel1Global")).Text;
						orderShipping.SenderTel1_1 = string.Empty;
						orderShipping.SenderTel1_2 = string.Empty;
						orderShipping.SenderTel1_3 = string.Empty;
						orderShipping.SenderAddr1 = string.Empty;
						orderShipping.SenderAddr5 = (orderShipping.IsSenderAddrUs)
							? ((DropDownList)ri.FindControl("ddlSenderAddr5")).SelectedValue
							: ((TextBox)ri.FindControl("tbSenderAddr5")).Text.Trim();

						orderShipping.SenderNameKana1 = string.Empty;
						orderShipping.SenderNameKana2 = string.Empty;
					}
				}
			}

			// 注文商品入力情報
			orderShipping.Items = GetOrderItemsInput(orderShippings, orderShipping, ri);

			orderShippings.Add(orderShipping);
		}

		return orderShippings.ToArray();
	}

	/// <summary>
	/// Get convenience store shipping
	/// </summary>
	/// <param name="shipping">Order shipping input</param>
	/// <param name="repeaterItem">The repeater item control</param>
	/// <param name="userShipping">User Shipping Model</param>
	private void GetConvenienceStoreShipping(OrderShippingInput shipping,
		RepeaterItem repeaterItem,
		UserShippingModel userShipping = null)
	{
		shipping.OrderId = this.OrderInput.OrderId;
		if (userShipping == null)
		{
			shipping.ShippingReceivingStoreId = hfCvsShopId.Value;
			shipping.ShippingName = hfCvsShopName.Value;
			shipping.ShippingAddr4 = hfCvsShopAddress.Value;
			shipping.ShippingTel1 = hfCvsShopTel.Value;
			if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED)
			{
				shipping.ShippingReceivingStoreType = ((DropDownList)repeaterItem.FindControl("ddlShippingReceivingStoreType")).SelectedValue;
			}
		}
		else
		{
			var selectedIndex = this.UserShippingAddress.ToList().IndexOf(userShipping);
			var rpAddressBook = (Repeater)repeaterItem.FindControl("rpAddressBook");
			shipping.ShippingReceivingStoreId = ((HiddenField)rpAddressBook.Items[selectedIndex].FindControl("hfCvsShopId")).Value;
			shipping.ShippingName = ((HiddenField)rpAddressBook.Items[selectedIndex].FindControl("hfCvsShopName")).Value;
			shipping.ShippingAddr4 = ((HiddenField)rpAddressBook.Items[selectedIndex].FindControl("hfCvsShopAddress")).Value;
			shipping.ShippingTel1 = ((HiddenField)rpAddressBook.Items[selectedIndex].FindControl("hfCvsShopTel")).Value;
		}
		shipping.ShippingReceivingStoreFlg = Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON;
		shipping.ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_TW;
		shipping.ShippingCountryName = "Taiwan";
		shipping.ShippingAddressKbn = CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE;
		shipping.ShippingCompanyName = ((TextBox)repeaterItem.FindControl("tbShippingCompanyName")).Text;
		shipping.ShippingCompanyPostName = ((TextBox)repeaterItem.FindControl("tbShippingCompanyPostName")).Text;
		shipping.ShippingTime = string.Empty;
		shipping.ShippingTimeText = ReplaceTag("@@DispText.shipping_time_list.none@@");
		shipping.ShippingCheckNo = ((TextBox)repeaterItem.FindControl("tbShippingCheckNo")).Text;
		shipping.ShippingMethod = ((DropDownList)repeaterItem.FindControl("ddlShippingMethod")).Text;
		shipping.DeliveryCompanyId = ((DropDownList)repeaterItem.FindControl("ddlDeliveryCompany")).Text;
		shipping.ExternalShipmentEntry = ((CheckBox)repeaterItem.FindControl("cbExecExternalShipmentEntry")).Checked;
		shipping.OldShippingCheckNo = ((HiddenField)repeaterItem.FindControl("hfOldShippingCheckNo")).Value;
		shipping.ShippingAddr1 = string.Empty;
		shipping.ShippingAddr2 = string.Empty;
		shipping.ShippingAddr3 = string.Empty;
		shipping.ShippingAddr5 = string.Empty;
		shipping.ShippingZip = string.Empty;
		shipping.ShippingZip1 = string.Empty;
		shipping.ShippingZip2 = string.Empty;
		shipping.ShippingTel1_1 = string.Empty;
		shipping.ShippingTel1_2 = string.Empty;
		shipping.ShippingTel1_3 = string.Empty;
		shipping.ShippingName1 = string.Empty;
		shipping.ShippingName2 = string.Empty;

		// 配送希望日指定有？
		var ucShippingDate = (DateTimePickerPeriodInputControl)repeaterItem.FindControl("ucShippingDate");
		var shippingDate = ucShippingDate.StartDateTimeString;
		if (Validator.IsDate(shippingDate))
		{
			shipping.ShippingDate = shippingDate;
		}

		// Scheduled shipping date
		var ucScheduledShippingDate = (DateTimePickerPeriodInputControl)repeaterItem.FindControl("ucScheduledShippingDate");
		var scheduledShippingDate = ucScheduledShippingDate.StartDateTimeString;
		shipping.ScheduledShippingDate = (scheduledShippingDate.Length > 2) ? scheduledShippingDate : string.Empty;
	}

	/// <summary>
	/// 注文商品入力情報取得
	/// </summary>
	/// <param name="orderShippings">注文配送先入力情報リスト</param>
	/// <param name="orderShipping">注文配送先入力情報</param>
	/// <param name="repeaterItemOrderShipping">注文配送先リピーターアイテム</param>
	/// <returns>注文商品入力情報</returns>
	private OrderItemInput[] GetOrderItemsInput(List<OrderShippingInput> orderShippings, OrderShippingInput orderShipping, RepeaterItem repeaterItemOrderShipping)
	{

		// 変更前の注文商品リスト取得
		var orderItemsBefore =
			this.OrderInput.Shippings
			.Where(s => s.OrderShippingNo == orderShipping.OrderShippingNo)
			.SelectMany(i => i.Items).ToArray();

		var orderItemNo = orderShippings.SelectMany(s => s.Items).Count() + 1;
		var orderItems = new List<OrderItemInput>();
		var productService = new ProductService();
		foreach (RepeaterItem ri in ((Repeater)repeaterItemOrderShipping.FindControl("rItemList")).Items)
		{
			OrderItemInput orderItem = null;

			// 変更前の同一商品が存在する？
			var shopId = this.LoginOperatorShopId;
			var productId = ((TextBox)ri.FindControl("tbProductId")).Text;
			var vId = ((TextBox)ri.FindControl("tbVariationId")).Text;
			var variationId = productId + vId;
			var itemIndex = ((HiddenField)ri.FindControl("hfItemIndex")).Value;
			var orderItemBefore =
				orderItemsBefore.FirstOrDefault(i =>
					(i.ItemIndex == itemIndex)
					&& (i.ShopId == shopId)
					&& (i.ProductId == productId)
					&& (i.VariationId == variationId));
			if (orderItemBefore != null)
			{
				orderItem = new OrderItemInput(orderItemBefore.CreateModel(), int.Parse(itemIndex));
			}

			// 通常商品？
			var productSetId = ((HiddenField)ri.FindControl("hfProductSetId")).Value;
			if (productSetId.Length == 0)
			{
				// 変更前に存在しない商品の場合はインスタンス作成
				var addItem = false;
				if (orderItem == null)
				{
					addItem = true;
					orderItem = new OrderItemInput();
				}

				// 入力値セット
				var deleteTarget = bool.Parse(((HiddenField)ri.FindControl("hfDeleteProduct")).Value);
				orderItem.DeleteTarget =
					orderItem.DeleteTargetSet = deleteTarget;
				orderItem.OrderId = this.OrderInput.OrderId;
				orderItem.OrderShippingNo = orderShipping.OrderShippingNo;
				orderItem.ProductTaxRate = ((HiddenField)ri.FindControl("hfProductTaxRate")).Value;
				orderItem.ShopId = shopId;
				orderItem.ProductId = productId;
				orderItem.VId = vId;
				orderItem.VariationId = variationId;
				orderItem.ProductName = ((TextBox)ri.FindControl("tbProductName")).Text;
				orderItem.ProductPrice = ((TextBox)ri.FindControl("tbProductPrice")).Text;
				orderItem.ItemQuantity = ((TextBox)ri.FindControl("tbItemQuantity")).Text;
				orderItem.FixedPurchaseItemOrderCount = ((TextBox)ri.FindControl("tbFixedPurchaseItemOrderCount")).Text;
				orderItem.FixedPurchaseItemShippedCount = ((TextBox)ri.FindControl("tbFixedPurchaseItemShippedCount")).Text;
				orderItem.ProductsaleId = ((TextBox)ri.FindControl("tbProductSaleId")).Text;
				orderItem.NoveltyId = ((TextBox)ri.FindControl("tbNoveltyId")).Text;
				orderItem.FixedPurchaseProductFlg = (Constants.FIXEDPURCHASE_OPTION_ENABLED && ((CheckBox)ri.FindControl("cbFixedPurchaseProduct")).Checked)
					? Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON
					: Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_OFF;
				orderItem.RecommendId = (Constants.RECOMMEND_OPTION_ENABLED && (Constants.NOVELTY_OPTION_ENABLED == false))
					? ((TextBox)ri.FindControl("tbRecommendId")).Text
					: ((TextBox)ri.FindControl("tbRecommendId2")).Text;
				orderItem.ProductBundleId = Constants.PRODUCTBUNDLE_OPTION_ENABLED
					? ((TextBox)ri.FindControl("tbProductBundleId")).Text
					: string.Empty;
				orderItem.ProductOptionTexts = CreateProductOptionSettingList(ri);
				orderItem.OrderSetpromotionNo = null;
				orderItem.OrderSetpromotionItemNo = null;
				var orderSetpromotionNo = ((DropDownList)ri.FindControl("ddlOrderSetPromotion")).SelectedValue;
				if ((orderItem.DeleteTarget == false)
					&& (string.IsNullOrEmpty(orderSetpromotionNo) == false))
				{
					orderItem.OrderSetpromotionNo = orderSetpromotionNo;
				}
				orderItem.ItemReturnExchangeKbn = ((HiddenField)ri.FindControl("hfItemReturnExchangeKbn")).Value;
				orderItem.ItemIndex = itemIndex;
				decimal productPrice;
				if (decimal.TryParse(orderItem.ProductPrice, out productPrice) == false)
				{
					productPrice = 0;
				}
				int itemQuantity;
				if (int.TryParse(orderItem.ItemQuantity, out itemQuantity) == false)
				{
					itemQuantity = 0;
				}

				var productPriceTax = TaxCalculationUtility.GetTaxPrice(
					productPrice + orderItem.OptionPrice,
					decimal.Parse(((HiddenField)ri.FindControl("hfProductTaxRate")).Value),
					orderShipping.ShippingCountryIsoCode,
					orderShipping.ShippingAddr5,
					(orderItemBefore != null)
						? orderItemBefore.ProductTaxRoundType
						: Constants.TAX_EXCLUDED_FRACTION_ROUNDING);
				var productPricePretax = TaxCalculationUtility.GetPriceTaxIncluded(productPrice + orderItem.OptionPrice, productPriceTax);
				orderItem.ProductPrice = productPrice.ToString();
				orderItem.ProductPricePretax = productPricePretax.ToString();
				orderItem.ItemQuantity =
					orderItem.ItemQuantitySingle = itemQuantity.ToString();
				orderItem.ItemPrice =
					orderItem.ItemPriceSingle = PriceCalculator.GetItemPrice(productPrice + orderItem.OptionPrice, itemQuantity).ToString();
				orderItem.ItemPriceTax = PriceCalculator.GetItemPrice(productPriceTax, itemQuantity).ToString();
				if (this.OrderInput.IsSubscriptionBoxFixedAmount && this.OrderInput.IsOrderCombined == false)
				{
					orderItem.SubscriptionBoxCourseId = this.OrderInput.SubscriptionBoxCourseId;
					orderItem.SubscriptionBoxFixedAmount = this.OrderInput.SubscriptionBoxFixedAmount;
				}

				// 入力値以外は商品情報から取得＆セット
				if (addItem)
				{
					var productVariation = productService.GetProductVariation(
						this.LoginOperatorShopId,
						orderItem.ProductId,
						orderItem.VariationId,
						"");
					if (productVariation != null)
					{
						orderItem.SupplierId = productVariation.SupplierId;
						orderItem.ProductNameKana =
							productVariation.NameKana
							+ ProductCommon.CreateVariationName(
								productVariation.VariationName1,
								productVariation.VariationName2,
								productVariation.VariationName3);
						orderItem.ProductPriceOrg = productVariation.Price.ToString();
						orderItem.BrandId = productVariation.BrandId1;
						orderItem.DownloadUrl = productVariation.DownloadUrl;
						orderItem.CooperationId1 = productVariation.VariationCooperationId1;
						orderItem.CooperationId2 = productVariation.VariationCooperationId2;
						orderItem.CooperationId3 = productVariation.VariationCooperationId3;
						orderItem.CooperationId4 = productVariation.VariationCooperationId4;
						orderItem.CooperationId5 = productVariation.VariationCooperationId5;
						orderItem.CooperationId6 = productVariation.VariationCooperationId6;
						orderItem.CooperationId7 = productVariation.VariationCooperationId7;
						orderItem.CooperationId8 = productVariation.VariationCooperationId8;
						orderItem.CooperationId9 = productVariation.VariationCooperationId9;
						orderItem.CooperationId10 = productVariation.VariationCooperationId10;
						orderItem.BundleItemDisplayType = productVariation.BundleItemDisplayType;
						orderItem.ProductTaxRate = productVariation.TaxRate.ToString();
						var productTaxPrice = TaxCalculationUtility.GetTaxPrice(
							productPrice,
							productVariation.TaxRate,
							orderShipping.ShippingCountryIsoCode,
							orderShipping.ShippingAddr5,
							productVariation.TaxRoundType);
						orderItem.ItemPriceTax = PriceCalculator.GetItemPrice(productTaxPrice, int.Parse(orderItem.ItemQuantity)).ToString();
						orderItem.ShippingSizeKbn = productVariation.ShippingSizeKbn;
					}
				}
			}
			// セット商品？
			else
			{
				// 削除対象セット
				var deleteTargetSet = false;
				var setOrderItem = orderItems.FirstOrDefault(i => i.ProductSetId == orderItem.ProductSetId);
				if (setOrderItem != null)
				{
					deleteTargetSet = setOrderItem.DeleteTargetSet;
				}
				else
				{
					deleteTargetSet = bool.Parse(((HiddenField)ri.FindControl("hfDeleteProductSet")).Value);
				}
				orderItem.DeleteTarget = orderItem.DeleteTargetSet = deleteTargetSet;
			}

			// 削除対象ではない場合は注文商品枝番セット
			if (orderItem.DeleteTarget == false)
			{
				orderItem.OrderItemNo = orderItemNo.ToString();
				orderItemNo++;
			}

			// 追加
			orderItems.Add(orderItem);
		}

		return SetCanDeleteToOrderItem(orderItems.ToArray());
	}

	/// <summary>
	/// 注文セットプロモーション入力情報取得
	/// </summary>
	/// <param name="order">注文入力情報</param>
	/// <returns>注文セットプロモーション入力情報</returns>
	private OrderSetPromotionInput[] GetOrderSetPromotionsInput(OrderInput order)
	{
		var orderSetPromotions = new List<OrderSetPromotionInput>();

		for (var i = 0; i < this.OrderInput.SetPromotions.Length; i++)
		{
			// 対象商品取得
			var targetItems = order.Shippings
				.SelectMany(
					shipping => shipping.Items,
					(value, item) => new
					{
						IsTaxable = TaxCalculationUtility.CheckShippingPlace(
							value.ShippingCountryIsoCode,
							value.ShippingAddr5),
						item = item
					})
				.Where(
					target => (target.item.DeleteTarget == false) && (target.item.OrderSetpromotionNo
						== this.OrderInput.SetPromotions[i].OrderSetpromotionNo))
				.ToArray();

			if (targetItems.Length <= 0) continue;

			var orderSetPromotion = new OrderSetPromotionInput
			{
				OrderId = this.OrderInput.OrderId,
				OrderSetpromotionNo = this.OrderInput.SetPromotions[i].OrderSetpromotionNo,
				SetpromotionId = this.OrderInput.SetPromotions[i].SetpromotionId,
				SetpromotionName = this.OrderInput.SetPromotions[i].SetpromotionName,
				SetpromotionDispName = this.OrderInput.SetPromotions[i].SetpromotionDispName,
				ProductDiscountFlg = this.OrderInput.SetPromotions[i].ProductDiscountFlg,
				ShippingChargeFreeFlg = this.OrderInput.SetPromotions[i].ShippingChargeFreeFlg,
				PaymentChargeFreeFlg = this.OrderInput.SetPromotions[i].PaymentChargeFreeFlg,
			};
			// 商品割引額
			var productDiscountAmount = ((TextBox)rSetPromotionProductDiscount.Items[i]
				.FindControl("tbSetPromotionProductDiscount")).Text;
			orderSetPromotion.ProductDiscountAmount = (productDiscountAmount != "") ? productDiscountAmount : "0";
			// 配送料割引額
			var shippingChargeDiscountAmount = ((TextBox)rSetPromotionShippingChargeDiscount.Items[i]
				.FindControl("tbSetPromotionShippingChargeDiscount")).Text;
			orderSetPromotion.ShippingChargeDiscountAmount =
				shippingChargeDiscountAmount != "" ? shippingChargeDiscountAmount : "0";
			// 決済手数料割引額
			var paymentChargeDiscountAmount = ((TextBox)rSetPromotionPaymentChargeDiscount.Items[i]
				.FindControl("tbSetPromotionPaymentChargeDiscount")).Text;
			orderSetPromotion.PaymentChargeDiscountAmount = (paymentChargeDiscountAmount != "")
				? paymentChargeDiscountAmount
				: "0";

			// 対象商品の合計金額
			var orderSetPromotionItemNo = 0;
			decimal undiscountedProductSubtotal = 0;
			foreach (var target in targetItems)
			{
				orderSetPromotionItemNo++;

				var itemPricePreTax = 0m;
				decimal pricePreTax;
				int itemQuantity;
				if (decimal.TryParse(
					target.IsTaxable ? target.item.ProductPricePretax : target.item.ProductPrice,
					out pricePreTax) && int.TryParse(target.item.ItemQuantity, out itemQuantity))
				{
					itemPricePreTax = PriceCalculator.GetItemPrice(pricePreTax, itemQuantity);
				}

				undiscountedProductSubtotal += itemPricePreTax;

				// 注文商品セットプロモーション枝番
				target.item.OrderSetpromotionItemNo = orderSetPromotionItemNo.ToString();
			}

			orderSetPromotion.UndiscountedProductSubtotal = undiscountedProductSubtotal.ToString();

			orderSetPromotions.Add(orderSetPromotion);
		}

		return orderSetPromotions.ToArray();
	}

	/// <summary>
	/// 注文クーポン入力情報取得
	/// </summary>
	/// <returns>注文クーポン入力情報</returns>
	private OrderCouponInput[] GetOrderCouponInput(OrderInput order)
	{
		var orderCoupons = new List<OrderCouponInput>();
		if (Constants.W2MP_COUPON_OPTION_ENABLED == false) return orderCoupons.ToArray();

		if (string.IsNullOrEmpty(tbOrderCouponCode.Text)) tbOrderCouponUse.Text = "0";
		order.OrderCouponUse = tbOrderCouponUse.Text;
		var orderCoupon = new OrderCouponInput
		{
			OrderId = this.OrderInput.OrderId,
			CouponCode = tbOrderCouponCode.Text,
			CouponDispName = tbOrderCouponDispName.Text,
			CouponName = tbOrderCouponName.Text,
			LastChanged = this.LoginOperatorName
		};
		orderCoupons.Add(orderCoupon);
		return orderCoupons.ToArray();
	}

	/// <summary>
	/// 注文ポイント情報取得
	/// </summary>
	/// <returns>注文ポイント情報</returns>
	private Hashtable[] GetOrderPointsInput()
	{
		var orderPoints = new List<Hashtable>();

		// 購入時には同時に仮ポイントと本ポイントが付与されない仕様で、仮ポイントが付与されたかの判定
		var hasOrderPointAddTemp = ((this.UserPointRelatedThisOrder != null)
			&& (this.UserPointRelatedThisOrder.Items.Any(up => up.IsPointTypeTemp)));

		// 注文に紐付いたユーザポイントが存在する
		if ((this.UserPointRelatedThisOrder != null)
			&& (this.UserPointRelatedThisOrder.Items.Count > 0))
		{
			// 注文に紐付いたユーザポイント(仮ポイント/期間限定ポイント)数分ループ
			foreach (RepeaterItem ri in rOrderPointAddTempOrLimitedTermComp.Items)
			{
				var userPoint = this.UserPointRelatedThisOrder.GetUserPointByPointKbnNo(int.Parse(((HiddenField)ri.FindControl("hfPointKbnNo")).Value));
				var orderPoint = new Hashtable
				{
					{ Constants.FIELD_ORDER_ORDER_POINT_USE, tbOrderPointUse.Text },
					{ Constants.FIELD_USERPOINT_POINT_KBN, (userPoint != null) ? userPoint.PointKbn : "" },
					{ Constants.FIELD_ORDER_ORDER_POINT_ADD, ((TextBox)ri.FindControl("tbOrderPointAdd")).Text },
					{ Constants.FIELD_USERPOINT_POINT_KBN_NO, (userPoint != null) ? userPoint.PointKbnNo.ToString() : "" },
					{ Constants.FIELD_USERPOINT_POINT_RULE_ID, (userPoint != null) ? userPoint.PointRuleId : "" },
					{ Constants.FIELD_USERPOINT_POINT_INC_KBN, (userPoint != null) ? userPoint.PointIncKbn : "" },
					{
						Constants.FIELD_USERPOINT_POINT_TYPE,
						hasOrderPointAddTemp ? Constants.FLG_USERPOINT_POINT_TYPE_TEMP : Constants.FLG_USERPOINT_POINT_TYPE_COMP
					}
				};
				orderPoints.Add(orderPoint);
			}
		}

		// 通常本付与ポイント有り
		var orderPointAddBaseCompText = tbOrderPointAddBaseComp.Text.Trim();
		if (UserService.IsUser(this.User.UserKbn)
			&& trOrderPointAddBasePoint.Visible
			&& (((string.IsNullOrEmpty(this.AddedBasePointCompOld) == false)
				&& (decimal.Parse(this.AddedBasePointCompOld) > 0))
				|| (((string.IsNullOrEmpty(orderPointAddBaseCompText) == false)
					&& decimal.Parse(orderPointAddBaseCompText) != 0))))
		{
			var orderPoint = new Hashtable
			{
				{ Constants.FIELD_ORDER_ORDER_POINT_USE, tbOrderPointUse.Text },
				{ Constants.FIELD_ORDER_ORDER_POINT_ADD, orderPointAddBaseCompText },
				{ Constants.FIELD_USERPOINT_POINT_TYPE, Constants.FLG_USERPOINT_POINT_TYPE_COMP },
				{ Constants.FIELD_USERPOINT_POINT_KBN, Constants.FLG_USERPOINT_POINT_KBN_BASE },
				{ Constants.FIELD_USERPOINT_POINT_KBN_NO, hfBasePointKbnNo.Value },
			};
			orderPoints.Add(orderPoint);
		}

		// 付与ポイント無しの場合
		if (orderPoints.Count == 0)
		{
			orderPoints.Add(
				new Hashtable
				{
					{ Constants.FIELD_ORDER_ORDER_POINT_USE, tbOrderPointUse.Text },
					{ Constants.FIELD_ORDER_ORDER_POINT_ADD, "0" },
				});
		}

		return orderPoints.ToArray();
	}

	/// <summary>
	/// Get Tw Order Invoice Input
	/// </summary>
	/// <returns>Input</returns>
	private TwOrderInvoiceInput GetTwOrderInvoiceInput()
	{
		if (OrderCommon.DisplayTwInvoiceInfo())
		{
			if (this.OrderInput.Shippings[0].ShippingCountryIsoCode == Constants.COUNTRY_ISO_CODE_TW)
			{
				SetVisibleForCarryTypeOption(ddlCarryType.SelectedItem.Value);
				SetVisibleForUniformOption(ddlUniformInvoice.SelectedItem.Value);
			}

			var twOrderInvoice = new TwOrderInvoiceInput
			{
				TwUniformInvoiceOption2 = (this.IsCompany)
					? tbCompanyOption2.Text
					: string.Empty,
				TwUniformInvoiceOption1 = ((this.IsCompany)
					? tbCompanyOption1.Text
					: (this.IsDonate)
						? tbDonateOption1.Text
						: string.Empty),
				TwCarryTypeOption = ((this.IsMobile)
					? tbCarryTypeOption1.Text
					: (this.IsCertificate)
						? tbCarryTypeOption2.Text
						: string.Empty),
				TwCarryType = ((this.IsPersonal)
					? ddlCarryType.SelectedValue
					: string.Empty),
				TwUniformInvoice = ddlUniformInvoice.SelectedValue
			};
			return twOrderInvoice;
		}
		else
		{
			return new TwOrderInvoiceInput();
		}
	}
	#endregion

	#region 再計算処理
	/// <summary>
	/// 自動計算適用
	/// </summary>
	/// <param name="order">注文入力情報</param>
	/// <param name="orderPoints">注文ポイント入力情報</param>
	private void ApplyAutoCalculation(OrderInput order, Hashtable[] orderPoints)
	{
		// 自動計算適用チェックなし？
		if (cbApplyAutoCalculation.Checked == false)
		{
			// 消費税金額を計算
			var orderForCalculate = order.CreateModel();
			var errorMassage = OrderCommon.SetCalculateTax(orderForCalculate);
			if (string.IsNullOrEmpty(errorMassage) == false)
			{
				this.DiscountLimitErrorMessages.Append(errorMassage
					+ "<br />"
					+ CommerceMessages.GetMessages(CommerceMessages.ERRMSG_MANAGER_DISCOUNT_LIMIT_ALERT));
			}
			order.OrderPriceTax = orderForCalculate.OrderPriceTax.ToString();
			order.OrderPriceSubtotalTax = orderForCalculate.OrderPriceSubtotalTax.ToString();
			order.OrderPriceByTaxRates = orderForCalculate.OrderPriceByTaxRates.Select(
				orderPriceByTaxRate => new OrderPriceByTaxRateInput(orderPriceByTaxRate)).ToArray();
			return;
		}

		// カート作成
		var cart = CreateCart(order);

		// カート再計算
		CalculateCart(order, cart);

		trSettlementAmount.Visible = (this.IsShippingConvenience
			&& (ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE));

		lSettlementAmount.Text = WebSanitizer.HtmlEncode(CreateSettlementAmount(
			this.OrderInput.OrderPaymentKbn,
			decimal.Parse(this.OrderInput.OrderPriceTotal),
			trSettlementAmount.Visible));

		// カート情報を注文入力情報にセット
		SetCartInfoToOrder(order, orderPoints, cart);

		// 自動計算適用アラート表示
		SetApplyAutoCalculationAlertMessages(order);
	}

	/// <summary>
	/// カート情報作成
	/// </summary>
	/// <param name="order">注文入力情報</param>
	/// <returns>カート情報</returns>
	private CartObject CreateCart(OrderInput order)
	{
		// カートオブジェクト作成
		var cart = new CartObject(order.UserId, order.OrderKbn, order.ShopId, order.ShippingId, false, false)
		{
			MallId = order.MallId,
			ShippingTaxRate = decimal.Parse(order.ShippingTaxRate),
			PaymentTaxRate = decimal.Parse(order.PaymentTaxRate)
		};
		cart.Shippings[0].UpdateShippingAddr(
			order.Shippings[0].DataSource,
			true,
			CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW);
		cart.Shippings[0].ShippingMethod = order.Shippings[0].ShippingMethod;
		cart.Shippings[0].DeliveryCompanyId = order.Shippings[0].DeliveryCompanyId;

		// 決済方法セット
		cart.Payment = new CartPayment();
		cart.Payment.UpdateCartPayment(
			order.OrderPaymentKbn,
			ddlOrderPaymentKbn.SelectedItem.Text,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			null,
			true,
			string.Empty);
		decimal orderPriceExchange = 0;
		decimal.TryParse(order.OrderPriceExchange, out orderPriceExchange);
		cart.Payment.PriceExchange = orderPriceExchange;

		// 注文商品リスト取得
		var orderItems = order.Shippings.SelectMany(s => s.Items).ToArray();

		// カート商品セットリスト取得
		var cartProductSets = new Dictionary<string, CartProductSet>();
		foreach (var orderItem in orderItems.Where(i => i.IsProductSet).ToArray())
		{
			if (cartProductSets.ContainsKey(orderItem.ProductSetId)) continue;

			var productSetInfo = ProductCommon.GetProductSetInfo(orderItem.ShopId, orderItem.ProductSetId);
			if (productSetInfo.Count == 0) continue;

			var cartProductSet = new CartProductSet(productSetInfo[0], int.Parse(orderItem.ProductSetCount), int.Parse(orderItem.ProductSetNo), false);
			cartProductSets.Add(orderItem.ProductSetId, cartProductSet);
		}

		// カート商品追加
		foreach (var orderItem in orderItems)
		{
			// 返品商品の場合は、次の商品へ
			if (orderItem.IsReturnItem) continue;
			// 削除対象の場合は、次の商品へ
			if (orderItem.DeleteTarget) continue;

			var itemQuantity = int.Parse(orderItem.ItemQuantity);
			var productPrice = decimal.Parse(orderItem.ProductPrice);
			var taxRate = orderItem.ProductTaxRate;
			var addCartKbn = order.IsGiftOrder
				? w2.App.Common.Constants.AddCartKbn.GiftOrder
				: orderItem.IsFixedPurchaseItem
					? (string.IsNullOrEmpty(order.SubscriptionBoxCourseId) == false)
						? Constants.AddCartKbn.SubscriptionBox
						: w2.App.Common.Constants.AddCartKbn.FixedPurchase
					: w2.App.Common.Constants.AddCartKbn.Normal;

			var cartProduct = cart.GetProduct(
				orderItem.ShopId,
				orderItem.ProductId,
				orderItem.VariationId,
				orderItem.IsProductSet,
				orderItem.IsFixedPurchaseItem,
				orderItem.ProductsaleId,
				orderItem.ProductOptionTexts,
				orderItem.ProductBundleId);

			// カート商品が存在する場合は、商品数セット
			if (cartProduct != null)
			{
				cartProduct.Count = cartProduct.Count + itemQuantity;
				cartProduct.CountSingle = cartProduct.CountSingle + itemQuantity;
			}
			// カート商品が存在しない場合は、カート商品追加
			else
			{
				var product = ProductCommon.GetProductVariationInfo(
					orderItem.ShopId,
					orderItem.ProductId,
					orderItem.VariationId,
					order.MemberRankId);
				if (product.Count == 0) continue;
				var cartProductTemp =
						new CartProduct(
							product[0],
							addCartKbn,
							orderItem.ProductsaleId,
							itemQuantity,
							false,
							orderItem.ProductOptionTexts,
							orderItem.IsFixedPurchaseItem ? order.FixedPurchaseId : string.Empty)
						{
							NoveltyId = orderItem.NoveltyId,
							RecommendId = orderItem.RecommendId,
							ProductBundleId = orderItem.ProductBundleId,
							BundleItemDisplayType = orderItem.BundleItemDisplayType,
							FixedPurchaseItemOrderCountInput = orderItem.IsFixedPurchaseItem
								? orderItem.FixedPurchaseItemOrderCount
								: string.Empty,
							SubscriptionBoxCourseId = orderItem.SubscriptionBoxCourseId,
							SubscriptionBoxFixedAmount = orderItem.IsSubscriptionBoxFixedAmount
								? (decimal?)decimal.Parse(orderItem.SubscriptionBoxFixedAmount)
								: null,
						};

				// セット商品？
				if (cartProductSets.ContainsKey(orderItem.ProductSetId))
				{
					cartProductTemp = cartProductSets[orderItem.ProductSetId].AddProductVirtual(product[0], cartProductTemp.CountSingle);
				}
				if (cartProductTemp == null) continue;
				// 商品税率セット
				cartProductTemp.TaxRate = decimal.Parse(taxRate);

				// 商品価格セット
				// ※入力内容を正とする
				cartProductTemp.SetPrice(productPrice);

				// カートに追加
				cart.AddVirtural(cartProductTemp, false);
			}
		}

		// ギフト注文？
		if (order.IsGiftOrder)
		{
			// 配送先追加
			for (var i = 1; i < order.Shippings.Length; i++)
			{
				var cartShipping = new CartShipping(cart);
				cartShipping.ShippingAddrKbn = CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW;
				cartShipping.UpdateShippingAddr(
					order.Shippings[i].DataSource,
					true,
					CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW);
				cartShipping.ShippingMethod = order.Shippings[i].ShippingMethod;
				cartShipping.DeliveryCompanyId = order.Shippings[i].DeliveryCompanyId;

				cart.Shippings.Add(cartShipping);
			}
			// 配送先に紐づける
			var cartShippingIndex = 0;
			foreach (var orderShipping in order.Shippings)
			{
				var cartShipping = cart.Shippings[cartShippingIndex];
				foreach (var orderItem in orderShipping.Items)
				{
					// 返品商品の場合は、次の商品へ
					if (orderItem.IsReturnItem) continue;
					// 削除対象の場合は、次の商品へ
					if (orderItem.DeleteTarget) continue;
					// Not calculate item empty
					if (string.IsNullOrEmpty(orderItem.ProductId)) continue;

					var cartProduct = cart.Items.Find(i =>
						(i.ShopId == orderItem.ShopId)
						&& (i.ProductId == orderItem.ProductId)
						&& (i.VariationId == orderItem.VariationId));
					cart.Shippings[cartShippingIndex].ProductCounts.Add(new CartShipping.ProductCount(cartProduct, int.Parse(orderItem.ItemQuantity)));
				}
				cartShippingIndex++;
			}
			// 商品数セット
			foreach (var cartProduct in cart.Items)
			{
				var productCounts = cart.Shippings.Select(s => s.ProductCounts.Where(p => p.Product == cartProduct).Where(p => p != null));
				var sum = productCounts.Sum(s => s.Sum(p => p.Count));
				cartProduct.CountSingle = cart.Shippings
					.Select(s => s.ProductCounts
						.Where(p => p.Product == cartProduct)
						.Where(p => p != null))
					.Sum(s => s.Sum(p => p.Count));
				cartProduct.Calculate();
			}
			cart.CalculateWithCartShipping();
		}

		cart.PriceInfoByTaxRate.AddRange(order.OrderPriceByTaxRates
			.Select(orderPriceByTaxRate => new CartPriceInfoByTaxRate(orderPriceByTaxRate.CreateModel())).ToList());
		cart.PriceRegulation = decimal.Parse(order.OrderPriceRegulation);
		cart.EnteredShippingPrice = decimal.Parse(order.OrderPriceShipping);
		cart.EnteredPaymentPrice = decimal.Parse(order.OrderPriceExchange);
		cart.SubscriptionBoxCourseId = order.SubscriptionBoxCourseId;
		cart.SubscriptionBoxFixedAmount = order.IsSubscriptionBoxFixedAmount
			? (decimal?)decimal.Parse(order.SubscriptionBoxFixedAmount)
			: null;
		int parsedOrderSubscriptionBoxOrderCount;
		cart.OrderSubscriptionBoxOrderCount =
			int.TryParse(order.OrderSubscriptionBoxOrderCount, out parsedOrderSubscriptionBoxOrderCount)
				? parsedOrderSubscriptionBoxOrderCount
				: 0;

		return cart;
	}

	/// <summary>
	/// カート再計算
	/// </summary>
	/// <param name="order">注文入力情報</param>
	/// <param name="cart">カート情報</param>
	private void CalculateCart(OrderInput order, CartObject cart)
	{
		// 利用ポイントセット
		if (Constants.W2MP_POINT_OPTION_ENABLED
			&& (UserService.IsUser(this.User.UserKbn)))
		{
			cart.SetUsePoint(decimal.Parse(order.OrderPointUse), decimal.Parse(order.OrderPointUseYen));
		}

		// 利用クーポンセット
		if (Constants.W2MP_COUPON_OPTION_ENABLED)
		{
			if ((order.Coupon != null)
				&& (string.IsNullOrEmpty(order.Coupon.CouponCode) == false))
			{
				// クーポン変更なし？
				UserCouponDetailInfo coupon = null;
				if (this.OrderInputOld.Coupon != null
					&& (order.Coupon.CouponCode == this.OrderInputOld.Coupon.CouponCode))
				{
					var coupons =
						new CouponService().GetAllUserCouponsFromCouponId(
						this.LoginOperatorDeptId,
						order.UserId,
						this.OrderInputOld.Coupon.CouponId,
						int.Parse(this.OrderInputOld.Coupon.CouponNo));
					if (coupons.Length != 0) coupon = coupons[0];
				}
				// クーポン指定あり or クーポン変更あり？
				else
				{
					var coupons =
						new CouponService().GetAllUserCouponsFromCouponCode(
						this.LoginOperatorDeptId,
						order.UserId,
						order.Coupon.CouponCode);
					if (coupons.Length != 0) coupon = coupons[0];
				}
				if (coupon != null)
				{
					cart.Coupon = new CartCoupon(coupon);
				}
			}
		}

		if (Constants.FIXEDPURCHASE_OPTION_ENABLED)
		{
			// 定期会員判定セット
			cart.IsFixedPurchaseMember = (this.User.FixedPurchaseMemberFlg == Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_ON);

			// 定期購入情報セット
			if (order.IsFixedPurchaseOrder)
			{
				// 定期購入回数を注文情報の「定期購入回数(注文時点)-1」に変更
				// ※cart.Calculateで+1しているため、-1している
				var fixedPurchase = new FixedPurchaseService().Get(order.FixedPurchaseId);
				fixedPurchase.OrderCount = int.Parse(order.FixedPurchaseOrderCount) - 1;

				// 定期商品購入も同様に変更
				foreach (var item in fixedPurchase.Shippings[0].Items)
				{
					var orderItem = order.Shippings[0].Items.FirstOrDefault(i => (i.ProductId == item.ProductId) && (i.VariationId == item.VariationId));
					var orderItemCount = ((orderItem == null) || (string.IsNullOrEmpty(orderItem.FixedPurchaseItemOrderCount))) ? "1" : orderItem.FixedPurchaseItemOrderCount;
					item.ItemOrderCount = int.Parse(orderItemCount) - 1;
				}

				cart.FixedPurchase = fixedPurchase;
			}
		}

		// 再計算
		cart.Calculate(false, isShippingChanged: true, isManagerModify: true);

		// 初回購入ポイントセット
		// ※cart.Calculateでセットされないため
		if (Constants.W2MP_POINT_OPTION_ENABLED
			&& UserService.IsUser(this.User.UserKbn)
			&& cart.IsOrderGrantablePoint)
		{
			var firstOrder = new OrderService().GetFirstOrder(this.User.UserId);
			if ((firstOrder != null) && (firstOrder.OrderId == order.OrderId))
			{
				cart.SetFirstBuyPoint();
			}
		}
	}

	/// <summary>
	/// カート情報を注文入力情報にセット
	/// </summary>
	/// <param name="order">注文入力情報</param>
	/// <param name="orderPoints">注文ポイント入力情報</param>
	/// <param name="cart">カート情報</param>
	private void SetCartInfoToOrder(OrderInput order, Hashtable[] orderPoints, CartObject cart)
	{
		// 注文商品セット
		var orderShippingNo = 1;
		var orderItems = CreateOrderItems(order, cart);

		order.ShippingTaxRate = cart.ShippingTaxRate.ToString();
		order.PaymentTaxRate = cart.PaymentTaxRate.ToString();

		foreach (var orderShipping in order.Shippings)
		{
			orderShipping.Items = orderItems.Where(i => i.OrderShippingNo == orderShippingNo.ToString()).ToArray();
			orderShippingNo++;
		}
		var newOrderPriceByTaxRates = order.OrderPriceByTaxRates.ToList();

		var newCartPriceInfoByTaxRate = cart.PriceInfoByTaxRate.ToList();

		var returnItemInfoByTaxRate = order.Shippings
			.SelectMany(s => s.Items)
			.Where(item => item.IsReturnItem)
			.GroupBy(item => item.ProductTaxRate);

		foreach (var returnItem in returnItemInfoByTaxRate)
		{
			var returnItemPriceTax = returnItem.Sum(item => decimal.Parse(item.ItemPriceTax));
			var returnItemPrice = returnItem
				.Sum(item => TaxCalculationUtility.GetPriceTaxIncluded(decimal.Parse(item.ItemPrice), decimal.Parse(item.ItemPriceTax)));

			var targetCartPriceInfoByTaxRate = newCartPriceInfoByTaxRate.FirstOrDefault(
				cartPriceInfoByTaxRate => cartPriceInfoByTaxRate.TaxRate == decimal.Parse(returnItem.Key));

			if (targetCartPriceInfoByTaxRate == null)
			{
				targetCartPriceInfoByTaxRate = new CartPriceInfoByTaxRate()
				{
					TaxRate = decimal.Parse(returnItem.Key)
				};
				newCartPriceInfoByTaxRate.Add(targetCartPriceInfoByTaxRate);
			}

			targetCartPriceInfoByTaxRate.PriceSubtotal += returnItemPrice;
			targetCartPriceInfoByTaxRate.PriceTotal += returnItemPrice;
			targetCartPriceInfoByTaxRate.TaxPrice = TaxCalculationUtility.GetTaxPrice(
				targetCartPriceInfoByTaxRate.PriceTotal,
				targetCartPriceInfoByTaxRate.TaxRate,
				Constants.TAX_EXCLUDED_FRACTION_ROUNDING);
			cart.PriceSubtotalTax += returnItemPriceTax;
		}

		order.OrderPriceTax = newCartPriceInfoByTaxRate.Sum(priceByTaxRate => priceByTaxRate.TaxPrice).ToString();
		order.OrderPriceSubtotalTax = cart.PriceSubtotalTax.ToString();
		newOrderPriceByTaxRates.Clear();
		newOrderPriceByTaxRates.AddRange(newCartPriceInfoByTaxRate.Select(priceInfoByTax => new OrderPriceByTaxRateInput(priceInfoByTax.CreateModel())));
		newOrderPriceByTaxRates.ForEach(priceInfoByTax => priceInfoByTax.OrderId = order.OrderId);
		order.OrderPriceByTaxRates = newOrderPriceByTaxRates.ToArray();

		// 配送料自動計算
		if (Constants.FREE_SHIPPING_FEE_OPTION_ENABLED
			&& cart.Items.Any(item => item.ExcludeFreeShippingFlg == Constants.FLG_PRODUCT_EXCLUDE_FREE_SHIPPING_FLG_VALID)
			&& (order.OrderPriceShipping == "0")
			&& (order.MallId == Constants.FLG_USER_MALL_ID_OWN_SITE))
		{
			order.OrderPriceShipping = cart.PriceShipping.ToString();
		}
		// 注文セットプロモーションセット
		if (Constants.SETPROMOTION_OPTION_ENABLED)
		{
			order.SetPromotions = CreateOrderSetPromotions(order, cart);
		}
		// 会員ランク割引セット
		if (Constants.MEMBER_RANK_OPTION_ENABLED)
		{
			order.MemberRankDiscountPrice = cart.MemberRankDiscount.ToString();
		}
		// クーポン割引セット
		if (Constants.W2MP_COUPON_OPTION_ENABLED)
		{
			order.OrderCouponUse = cart.UseCouponPrice.ToString();
		}
		// 定期会員割引＆定期購入割引セット
		if (Constants.FIXEDPURCHASE_OPTION_ENABLED)
		{
			if (Constants.MEMBER_RANK_OPTION_ENABLED)
			{
				order.FixedPurchaseMemberDiscountAmount = cart.FixedPurchaseMemberDiscountAmount.ToString();
			}
			order.FixedPurchaseDiscountPrice = cart.FixedPurchaseDiscount.ToString();
		}
		// 付与ポイントセット
		if (Constants.W2MP_POINT_OPTION_ENABLED
			&& (UserService.IsUser(this.User.UserKbn)))
		{
			// ユーザポイント(仮ポイント)が存在する
			if (this.UserPointList.UserPointTemp.Length > 0)
			{
				foreach (var orderPoint in orderPoints)
				{
					// 初回購入ポイント？
					var pointIncKbn = (string)orderPoint[Constants.FIELD_USERPOINT_POINT_INC_KBN];
					var pointRuleId = (string)orderPoint[Constants.FIELD_USERPOINT_POINT_RULE_ID];
					if (pointIncKbn == Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_FIRST_BUY)
					{
						if (cart.FirstBuyPoints.ContainsKey(pointRuleId))
						{
							orderPoint[Constants.FIELD_ORDER_ORDER_POINT_ADD] = cart.FirstBuyPoints[pointRuleId].ToString();
						}
						else
						{
							orderPoint[Constants.FIELD_ORDER_ORDER_POINT_ADD] = "0";
						}
					}
					// 購入ポイント？
					else
					{
						if (cart.BuyPoints.ContainsKey(pointRuleId))
						{
							orderPoint[Constants.FIELD_ORDER_ORDER_POINT_ADD] = cart.BuyPoints[pointRuleId].ToString();
						}
						else
						{
							orderPoint[Constants.FIELD_ORDER_ORDER_POINT_ADD] = "0";
						}
					}
				}
			}
			// 統合前のユーザが存在しゲストの場合はカートから付与ポイントは取得しない
			// 仮ポイントなしで、元注文の場合
			else if (this.OrderOld.IsNotReturnExchangeOrder
				&& ((this.BeforeIntegrationUser == null)
					|| (UserService.IsUser(this.BeforeIntegrationUser.UserKbn))))
			{
				orderPoints[0][Constants.FIELD_ORDER_ORDER_POINT_ADD] = cart.AddPoint.ToString();

				// 付与ポイントがある場合
				if (cart.AddPoint != 0)
				{
					orderPoints[0][Constants.FIELD_USERPOINT_POINT_KBN] = cart.PointKbns[0][Constants.FIELD_USERPOINT_POINT_KBN];
					orderPoints[0][Constants.FIELD_USERPOINT_POINT_INC_KBN] = cart.PointKbns[0][Constants.FIELD_USERPOINT_POINT_INC_KBN];
					orderPoints[0][Constants.FIELD_USERPOINT_POINT_TYPE] = Constants.FLG_USERPOINT_POINT_TYPE_COMP;
					orderPoints[0][Constants.FIELD_USERPOINT_POINT_RULE_ID] = cart.PointKbns[0][Constants.FIELD_USERPOINT_POINT_RULE_ID];
					orderPoints[0][Constants.FIELD_USERPOINT_POINT_RULE_KBN] = cart.PointKbns[0][Constants.FIELD_USERPOINT_POINT_RULE_KBN];
				}
			}
		}
	}

	/// <summary>
	/// 自動計算適用アラート表示
	/// </summary>
	/// <param name="order">注文入力情報</param>
	private void SetApplyAutoCalculationAlertMessages(OrderInput order)
	{
		var alertMessages = new List<string>();

		if (Constants.SETPROMOTION_OPTION_ENABLED)
		{
			var oldOrderSetPromotions = this.OrderInputOld.SetPromotions;
			var newOrderSetPromotions = order.SetPromotions;

			// セットプロモーションが未適用になった？
			var notApplyOrderSetPromotions =
				oldOrderSetPromotions.Where(oldOrderSetPromotion =>
					newOrderSetPromotions.Any(newOrderSetPromotion =>
						(newOrderSetPromotion.OrderSetpromotionNo == oldOrderSetPromotion.OrderSetpromotionNo)
						&& (newOrderSetPromotion.SetpromotionId == oldOrderSetPromotion.SetpromotionId)
						) == false).ToArray();
			if (notApplyOrderSetPromotions.Length != 0)
			{
				var alertMessage = string.Format(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERSETPROMOTION_NOTAPPLY_ALERT),
					string.Join("、", notApplyOrderSetPromotions.Select(s => string.Format("「{0}:{1}」", s.OrderSetpromotionNo, s.SetpromotionName))));
				alertMessages.Add(alertMessage);
			}

			// セットプロモーションが新しく適用された？
			var applyOrderSetPromotions =
				newOrderSetPromotions.Where(newOrderSetPromotion =>
					oldOrderSetPromotions.Any(oldOrderSetPromotion =>
						(oldOrderSetPromotion.OrderSetpromotionNo == newOrderSetPromotion.OrderSetpromotionNo)
						&& (oldOrderSetPromotion.SetpromotionId == newOrderSetPromotion.SetpromotionId)
						) == false).ToArray();
			if (applyOrderSetPromotions.Length != 0)
			{
				var alertMessage = string.Format(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERSETPROMOTION_APPLY_ALERT),
					string.Join("、", applyOrderSetPromotions.Select(s => string.Format("「{0}:{1}」", s.OrderSetpromotionNo, s.SetpromotionName))));
				alertMessages.Add(alertMessage);
			}
		}

		// アラート表示
		trOrderPriceAlertMessagesTitle.Visible =
			trOrderPriceAlertMessages.Visible = (alertMessages.Count != 0);
		lbOrderPriceAlertMessages.Text = StringUtility.ChangeToBrTag(string.Join("\r\n", alertMessages));
	}

	/// <summary>
	/// カート情報から注文商品入力情報リスト作成
	/// </summary>
	/// <param name="order">注文入力情報</param>
	/// <param name="cart">カート情報</param>
	/// <returns>注文商品入力情報リスト</returns>
	private OrderItemInput[] CreateOrderItems(OrderInput order, CartObject cart)
	{
		// 返品商品追加
		var orderItems = new List<OrderItemInput>();
		orderItems.AddRange(
			order.Shippings.SelectMany(s => s.Items).Where(i => (i.IsReturnItem)).ToArray());

		// 通常注文？
		if (cart.IsGift == false)
		{
			var orderItemNo = orderItems.Count + 1;
			foreach (var cartProduct in cart.Items.Where(cp => (cp.QuantitiyUnallocatedToSet != 0) && (cp.IsBundle == false)).ToArray())
			{
				var itemQuantity = cartProduct.IsSetItem ? cartProduct.Count : cartProduct.QuantitiyUnallocatedToSet;
				var itemQuantitySingle = cartProduct.IsSetItem ? cartProduct.CountSingle : cartProduct.QuantitiyUnallocatedToSet;
				var orderItem = CreateOrderItem(
					order,
					cartProduct,
					itemQuantity,
					itemQuantitySingle,
					1,
					orderItemNo,
					null,
					null,
					cart.Shippings[0].IsDutyFree);
				orderItems.Add(orderItem);

				orderItemNo++;
			}

			// セットプロモーション商品登録
			foreach (CartSetPromotion setpromotion in cart.SetPromotions)
			{
				var orderSetpromotionItemNo = 1;
				foreach (var cartProduct in setpromotion.Items)
				{
					var itemQuantity = cartProduct.QuantityAllocatedToSet[setpromotion.CartSetPromotionNo];
					var orderItem = CreateOrderItem(
						order,
						cartProduct,
						itemQuantity,
						itemQuantity,
						1,
						Math.Min(orderItemNo, order.Shippings[0].Items.Length),
						setpromotion.CartSetPromotionNo,
						orderSetpromotionItemNo,
						cart.Shippings[0].IsDutyFree);
					orderItems.Add(orderItem);

					orderItemNo++;
					orderSetpromotionItemNo++;
				}
			}

			// 同梱商品を登録
			foreach (var cartProduct in cart.Items.Where(cp => (cp.QuantitiyUnallocatedToSet != 0) && cp.IsBundle).ToArray())
			{
				var itemQuantity = cartProduct.IsSetItem ? cartProduct.Count : cartProduct.QuantitiyUnallocatedToSet;
				var itemQuantitySingle = cartProduct.IsSetItem ? cartProduct.CountSingle : cartProduct.QuantitiyUnallocatedToSet;
				var orderItem = CreateOrderItem(
					order,
					cartProduct,
					itemQuantity,
					itemQuantitySingle,
					1,
					orderItemNo,
					null,
					null,
					cart.Shippings[0].IsDutyFree);
				orderItems.Add(orderItem);

				orderItemNo++;
			}
		}
		// ギフト注文？
		else
		{
			var productsAllocatedToSetAndShipping = new List<Hashtable>();

			// セットプロモーションあり？
			if (cart.SetPromotions.Items.Count != 0)
			{
				// 配送先商品をばらす
				var orderShippingNo = 1;
				var cartProducts = new List<Hashtable>();
				foreach (var cartShipping in cart.Shippings)
				{
					foreach (var productCount in cartShipping.ProductCounts)
					{
						for (var i = 0; i < productCount.Count; i++)
						{
							cartProducts.Add(new Hashtable
								{
								{"product", productCount.Product},
								{Constants.FIELD_ORDERITEM_ORDER_SHIPPING_NO, orderShippingNo},
								{"isDutyFree", cartShipping.IsDutyFree}
								});
						}
					}
					orderShippingNo++;
				}
				foreach (var cartProduct in cart.Items)
				{
					// 対象商品を抽出
					var targetCartProducts = cartProducts.FindAll(cp => (CartProduct)cp["product"] == cartProduct);

					// セットプロモーション情報を追加
					var i = 0;
					for (var j = 0; j < cartProduct.QuantitiyUnallocatedToSet; j++)
					{
						targetCartProducts[i].Add(Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO, "");
						i++;
					}
					foreach (KeyValuePair<int, int> setpromotionitem in cartProduct.QuantityAllocatedToSet)
					{
						for (var j = 0; j < setpromotionitem.Value; j++)
						{
							targetCartProducts[i].Add(Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO, setpromotionitem.Key.ToString());
							i++;
						}
					}

					// 配送先、セットプロモーションでグループ化
					var groupedTargetCartProducts = targetCartProducts.GroupBy(p => p[Constants.FIELD_ORDERITEM_ORDER_SHIPPING_NO] + "," + p[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO]);

					productsAllocatedToSetAndShipping.AddRange(
						groupedTargetCartProducts.Select(
							groupedTargetCartProduct => new Hashtable
							{
								{ "product", cartProduct },
								{
									Constants.FIELD_ORDERITEM_ORDER_SHIPPING_NO,
									int.Parse(groupedTargetCartProduct.Key.Split(',')[0])
								},
								{
									Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO,
									groupedTargetCartProduct.Key.Split(',')[1]
								},
								{ Constants.FIELD_ORDERITEM_ITEM_QUANTITY, groupedTargetCartProduct.ToList().Count },
								{ "isDutyFree", groupedTargetCartProduct.First()["isDutyFree"] }
							}));
				}
			}
			else
			{
				var orderShippingNo = 1;
				foreach (var cartShipping in cart.Shippings)
				{
					productsAllocatedToSetAndShipping.AddRange(
						cartShipping.ProductCounts.Select(
							productCount => new Hashtable
							{
								{ "product", productCount.Product },
								{ Constants.FIELD_ORDERITEM_ORDER_SHIPPING_NO, orderShippingNo },
								{ Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO, "" },
								{ Constants.FIELD_ORDERITEM_ITEM_QUANTITY, productCount.Count },
								{ "isDutyFree", cartShipping.IsDutyFree }
							}));
					orderShippingNo++;
				}
			}

			// 通常商品登録
			var orderItemNo = 1;
			foreach (var product in productsAllocatedToSetAndShipping
				.Where(product => ((string)product[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO] == "")
					&& (((CartProduct)product["product"]).IsBundle == false))
				.ToArray())
			{
				var cartProduct = (CartProduct)product["product"];
				var itemQuantity = (int)product[Constants.FIELD_ORDERITEM_ITEM_QUANTITY];
				var orderShippingNo = (int)product[Constants.FIELD_ORDERITEM_ORDER_SHIPPING_NO];
				var orderItem = CreateOrderItem(
					order,
					cartProduct,
					itemQuantity,
					itemQuantity,
					orderShippingNo,
					orderItemNo,
					null,
					null,
					(bool)product["isDutyFree"]);
				orderItems.Add(orderItem);
				orderItemNo++;
			}
			// セットプロモーション商品登録
			foreach (CartSetPromotion setpromotion in cart.SetPromotions)
			{
				var orderSetpromotionItemNo = 1;
				foreach (var product in productsAllocatedToSetAndShipping.Where(product => (string)product[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO] == setpromotion.CartSetPromotionNo.ToString()))
				{
					var cartProduct = (CartProduct)product["product"];
					var itemQuantity = (int)product[Constants.FIELD_ORDERITEM_ITEM_QUANTITY];
					var orderShippingNo = (int)product[Constants.FIELD_ORDERITEM_ORDER_SHIPPING_NO];
					var orderItem = CreateOrderItem(
						order,
						cartProduct,
						itemQuantity,
						itemQuantity,
						orderShippingNo,
						orderItemNo,
						setpromotion.CartSetPromotionNo,
						orderSetpromotionItemNo,
						(bool)product["isDutyFree"]);
					orderItems.Add(orderItem);
					orderItemNo++;
					orderSetpromotionItemNo++;
				}
			}

			// 同梱商品を登録
			foreach (var product in productsAllocatedToSetAndShipping
				.Where(product => ((string)product[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO] == "")
					&& ((CartProduct)product["product"]).IsBundle)
				.ToArray())
			{
				var cartProduct = (CartProduct)product["product"];
				var itemQuantity = (int)product[Constants.FIELD_ORDERITEM_ITEM_QUANTITY];
				var orderShippingNo = (int)product[Constants.FIELD_ORDERITEM_ORDER_SHIPPING_NO];
				var orderItem = CreateOrderItem(
					order,
					cartProduct,
					itemQuantity,
					itemQuantity,
					orderShippingNo,
					orderItemNo,
					null,
					null,
					(bool)product["isDutyFree"]);
				orderItems.Add(orderItem);
				orderItemNo++;
			}
		}

		// 商品連番セット（配送先毎）
		foreach (var orderItemsGroupByOrderShippingNo in orderItems.GroupBy(orderitem => orderitem.OrderShippingNo).ToArray())
		{
			var itemIndex = 0;
			foreach (var orderitem in orderItemsGroupByOrderShippingNo.ToArray())
			{
				orderitem.ItemIndex = itemIndex.ToString();
				itemIndex++;
			}
		}

		return orderItems.ToArray();
	}

	/// <summary>
	/// カート商品情報から注文商品入力情報作成
	/// </summary>
	/// <param name="order">注文入力情報</param>
	/// <param name="cartProduct">カート商品情報</param>
	/// <param name="itemQuantity">商品数</param>
	/// <param name="itemQuantitySingle">商品単品数</param>
	/// <param name="orderShippingNo">配送先枝番</param>
	/// <param name="orderItemNo">注文商品枝番</param>
	/// <param name="orderSetPromotionNo">注文セットプロモーション枝番</param>
	/// <param name="orderSetpromotionItemNo">注文セットプロモーション商品枝番</param>
	/// <param name="isDutyFree">免税か</param>
	/// <returns>注文商品入力情報</returns>
	private OrderItemInput CreateOrderItem(
		OrderInput order,
		CartProduct cartProduct,
		int itemQuantity,
		int itemQuantitySingle,
		int orderShippingNo,
		int orderItemNo,
		int? orderSetPromotionNo,
		int? orderSetpromotionItemNo,
		bool isDutyFree)
	{
		var orderItemInputByOrderItemNo = order.Shippings[orderShippingNo - 1].Items
			.FirstOrDefault(orderItemInput => orderItemInput.OrderItemNo == orderItemNo.ToString());
		cartProduct.FixedPurchaseItemOrderCount
			= string.IsNullOrEmpty(cartProduct.FixedPurchaseItemOrderCountInput)
				? 0
				: int.Parse(cartProduct.FixedPurchaseItemOrderCountInput);

		var orderItem = new OrderItemInput()
		{
			OrderId = order.OrderId,
			OrderItemNo = orderItemNo.ToString(),
			OrderShippingNo = orderShippingNo.ToString(),
			ShopId = order.ShopId,
			ProductId = cartProduct.ProductId,
			VariationId = cartProduct.VariationId,
			VId = cartProduct.VId,
			SupplierId = cartProduct.SupplierId,
			ProductName = cartProduct.ProductJointName,
			ProductNameKana = cartProduct.ProductNameKana,
			ProductPrice = cartProduct.Price.ToString(),
			ProductPriceOrg = cartProduct.PriceOrg.ToString(),
			ProductTaxIncludedFlg = cartProduct.TaxIncludedFlg,
			ProductTaxRate = cartProduct.TaxRate.ToString(),
			ProductTaxRoundType = cartProduct.TaxRoundType,
			ProductPricePretax = cartProduct.PricePretax.ToString(),
			ItemQuantity = itemQuantity.ToString(),
			ItemQuantitySingle = itemQuantitySingle.ToString(),
			ItemPrice = PriceCalculator.GetItemPrice(cartProduct.PriceIncludedOptionPrice, itemQuantity).ToString(),
			ItemPriceTax = isDutyFree ? "0" : PriceCalculator.GetItemPrice(cartProduct.PriceTax + cartProduct.OptionPriceTax, itemQuantity).ToString(),
			ItemPriceSingle = PriceCalculator.GetItemPrice(cartProduct.PriceIncludedOptionPrice, itemQuantitySingle).ToString(),
			ProductSetId = cartProduct.IsSetItem ? cartProduct.ProductSet.ProductSetId : string.Empty,
			ProductSetNo = cartProduct.IsSetItem ? cartProduct.ProductSet.ProductSetNo.ToString() : null,
			ProductSetCount = cartProduct.IsSetItem ? cartProduct.ProductSet.ProductSetCount.ToString() : null,
			DateCreated = order.DateCreated,
			DateChanged = order.DateChanged,
			ProductOptionTexts = GetSelectedProductOptionText(order, cartProduct, orderShippingNo, orderItemNo),
			BrandId = cartProduct.BrandId,
			DownloadUrl = cartProduct.DownloadUrl,
			ProductsaleId = cartProduct.ProductSaleId,
			CooperationId1 = cartProduct.CooperationId[0],
			CooperationId2 = cartProduct.CooperationId[1],
			CooperationId3 = cartProduct.CooperationId[2],
			CooperationId4 = cartProduct.CooperationId[3],
			CooperationId5 = cartProduct.CooperationId[4],
			CooperationId6 = cartProduct.CooperationId[5],
			CooperationId7 = cartProduct.CooperationId[6],
			CooperationId8 = cartProduct.CooperationId[7],
			CooperationId9 = cartProduct.CooperationId[8],
			CooperationId10 = cartProduct.CooperationId[9],
			OrderSetpromotionNo = orderSetPromotionNo.HasValue ? orderSetPromotionNo.ToString() : null,
			OrderSetpromotionItemNo = orderSetpromotionItemNo.HasValue ? orderSetpromotionItemNo.ToString() : null,
			NoveltyId = cartProduct.NoveltyId,
			RecommendId = cartProduct.RecommendId,
			FixedPurchaseProductFlg = (cartProduct.AddCartKbn == (Constants.AddCartKbn.FixedPurchase) || (cartProduct.AddCartKbn == Constants.AddCartKbn.SubscriptionBox))
			? Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON : Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_OFF,
			ProductBundleId = cartProduct.ProductBundleId,
			BundleItemDisplayType = cartProduct.BundleItemDisplayType,
			FixedPurchaseDiscountType = cartProduct.FixedPurchaseDiscountType,
			FixedPurchaseDiscountValue = cartProduct.FixedPurchaseDiscountValue.HasValue
				? cartProduct.FixedPurchaseDiscountValue.ToString()
				: null,
			ItemDiscountedPrice = (orderSetPromotionNo == null)
				? cartProduct.DiscountedPriceUnAllocatedToSet.ToString()
				: cartProduct.DiscountedPrice[int.Parse(orderSetPromotionNo.ToString())].ToString(),
			ShippingSizeKbn = cartProduct.ShippingSizeKbn,
			FixedPurchaseItemOrderCount = cartProduct.FixedPurchaseItemOrderCount.Value.ToString(),
			FixedPurchaseItemShippedCount = order.IsFixedPurchaseOrder ?  order.Shippings[0].Items[(orderItemNo - 1)].FixedPurchaseItemShippedCount : string.Empty,
			SubscriptionBoxCourseId = (orderItemInputByOrderItemNo != null)
				? orderItemInputByOrderItemNo.SubscriptionBoxCourseId
				: string.Empty,
			SubscriptionBoxFixedAmount = (orderItemInputByOrderItemNo != null)
				? orderItemInputByOrderItemNo.SubscriptionBoxFixedAmount
				: string.Empty,
		};

		return orderItem;
	}

	/// <summary>
	/// 商品付帯情報選択値文字列を取得
	/// </summary>
	/// <param name="order">注文入力情報</param>
	/// <param name="cartProduct">カート商品情報</param>
	/// <param name="orderShippingNo">配送先枝番</param>
	/// <param name="orderItemNo">注文商品枝番</param>
	/// <returns></returns>
	private string GetSelectedProductOptionText(
		OrderInput order,
		CartProduct cartProduct,
		int orderShippingNo,
		int orderItemNo)
	{
		// 付帯価格オプションが無効の際は入力値をそのまま返す
		// NOTE: テキスト入力の為入力された値をそのまま返すようにしています
		if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED == false)
		{
			var orderItemInputByOrderItemNo = order.Shippings[orderShippingNo - 1].Items
				.FirstOrDefault(orderItemInput => orderItemInput.OrderItemNo == orderItemNo.ToString());
			var productOptionTexts = (orderItemInputByOrderItemNo != null)
				? orderItemInputByOrderItemNo.ProductOptionTexts
				: string.Empty;

			return productOptionTexts;
		}

		return cartProduct.ProductOptionSettingList != null
			? ProductOptionSettingHelper.GetSelectedOptionSettingForOrderItem(cartProduct.ProductOptionSettingList)
			: string.Empty;
	}

	/// <summary>
	/// カート情報から注文セットプロモーション入力情報作成
	/// </summary>
	/// <param name="order">注文入力情報</param>
	/// <param name="cart">カート情報</param>
	/// <returns>注文セットプロモーション入力情報</returns>
	private OrderSetPromotionInput[] CreateOrderSetPromotions(OrderInput order, CartObject cart)
	{
		// 注文セットプロモーション入力情報作成
		var orderSetPromotions =
			cart.SetPromotions.Cast<CartSetPromotion>().Select(cartSetPromotion =>
				new OrderSetPromotionInput
				{
					OrderId = order.OrderId,
					OrderSetpromotionNo = cartSetPromotion.CartSetPromotionNo.ToString(),
					SetpromotionId = cartSetPromotion.SetpromotionId,
					SetpromotionName = cartSetPromotion.SetpromotionName,
					SetpromotionDispName =
						((order.OrderKbn == Constants.FLG_ORDER_ORDER_KBN_MOBILE)
							&& (cartSetPromotion.SetpromotionDispNameMobile != ""))
							? cartSetPromotion.SetpromotionDispNameMobile
							: cartSetPromotion.SetpromotionDispName,
					UndiscountedProductSubtotal = cartSetPromotion.UndiscountedProductSubtotal.ToString(),
					ProductDiscountFlg = cartSetPromotion.ProductDiscountFlg,
					ProductDiscountAmount = cartSetPromotion.IsDiscountTypeProductDiscount ? cartSetPromotion.ProductDiscountAmount.ToString() : "0",
					ShippingChargeFreeFlg = cartSetPromotion.ShippingChargeFreeFlg,
					//ShippingChargeDiscountAmount = cartSetPromotion.IsDiscountTypeShippingChargeFree ? cartSetPromotion.ShippingChargeDiscountAmount.ToString() : "0",
					PaymentChargeFreeFlg = cartSetPromotion.PaymentChargeFreeFlg,
					//PaymentChargeDiscountAmount = cartSetPromotion.IsDiscountTypePaymentChargeFree ? cartSetPromotion.PaymentChargeDiscountAmount.ToString() : "0",
				}).ToArray();

		// 配送料＆決済手数料は注文情報から取得＆セット
		var appliedShippingChargeFree = false;
		var appliedPaymentChargeFree = false;
		foreach (var orderSetPromotion in orderSetPromotions)
		{
			orderSetPromotion.ShippingChargeDiscountAmount = "0";
			if (orderSetPromotion.IsDiscountTypeShippingChargeFree
				&& (appliedShippingChargeFree == false))
			{
				var couponType = (order.Coupon != null) ? order.Coupon.CouponType : string.Empty;
				var couponFreeShippingFlg = ((order.Coupon != null) && (order.Coupon.CouponCode != ""))
					? new CouponService().GetCoupon(Constants.CONST_DEFAULT_DEPT_ID, order.Coupon.CouponId).FreeShippingFlg
					: Constants.FLG_COUPON_FREE_SHIPPING_INVALID;
				if ((CouponOptionUtility.IsFreeShipping(couponType) == false)
					&& (couponFreeShippingFlg != Constants.FLG_COUPON_FREE_SHIPPING_VALID))
				{
					orderSetPromotion.ShippingChargeDiscountAmount = order.OrderPriceShipping;
				}

				appliedShippingChargeFree = true;
			}
			orderSetPromotion.PaymentChargeDiscountAmount = "0";
			if (orderSetPromotion.IsDiscountTypePaymentChargeFree
				&& (appliedPaymentChargeFree == false))
			{
				orderSetPromotion.PaymentChargeDiscountAmount = order.OrderPriceExchange;
				appliedPaymentChargeFree = true;
			}
		}

		return orderSetPromotions;
	}

	#endregion

	/// <summary>
	/// 配送希望時間帯ドロップダウンリストアイテム取得
	/// </summary>
	/// <param name="shippingTime">配送希望時間帯</param>
	/// <returns>配送希望時間帯ドロップダウンリストアイテム</returns>
	protected ListItemCollection GetShippingTimeList(int index, string shippingTime)
	{
		var shippingTimeList = ShippingTimeList(index);

		// 配送希望時間帯がリストにない場合は追加
		if ((shippingTime != null) && (shippingTimeList.FindByValue(shippingTime) == null))
		{
			shippingTimeList.Insert(shippingTimeList.Count, new ListItem(GetShippingTimeMessage(shippingTime), shippingTime));
		}

		return shippingTimeList;
	}

	/// <summary>
	/// のし種類ドロップダウンリストアイテム取得
	/// </summary>
	/// <param name="wrappingPaperType">のし種類</param>
	/// <returns>のし種類ドロップダウンリストアイテム</returns>
	protected ListItemCollection GetWrappingPaperTypeList(string wrappingPaperType)
	{
		var wrappingPaperTypeList = this.WrappingPaperTypeList;

		// のし種類がリストにない場合は追加
		if ((wrappingPaperType != null) && (wrappingPaperTypeList.FindByValue(wrappingPaperType) == null))
		{
			wrappingPaperTypeList.Insert(wrappingPaperTypeList.Count, wrappingPaperType);
		}

		return wrappingPaperTypeList;
	}

	/// <summary>
	/// 包装種類ドロップダウンリストアイテム取得
	/// </summary>
	/// <param name="wrappingBagType">のし種類</param>
	/// <returns>包装種類ドロップダウンリストアイテム</returns>
	protected ListItemCollection GetWrappingBagTypeList(string wrappingBagType)
	{
		var wrappingBagTypeList = this.WrappingBagTypeList;

		// 包装種類がリストにない場合は追加
		if ((wrappingBagType != null) && (wrappingBagTypeList.FindByValue(wrappingBagType) == null))
		{
			wrappingBagTypeList.Insert(wrappingBagTypeList.Count, wrappingBagType);
		}

		return wrappingBagTypeList;
	}

	/// <summary>
	/// 配送希望日表示文言取得（配送希望日が利用不可の場合）
	/// </summary>
	/// <param name="shippingDate">配送希望日</param>
	/// <returns>配送希望日表示文言</returns>
	protected string GetShippingDateMessage(string shippingDate)
	{
		if (this.ShopShipping == null) return string.Empty;

		if (Constants.MALLCOOPERATION_OPTION_ENABLED
			&& (shippingDate != null)
			&& (shippingDate.StartsWith("1900/01/01")))
		{
			return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SHIPPING_DATE_DESIGNATION_MEMO_FIELD);
		}
		var result = shippingDate ?? ReplaceTag("@@DispText.shipping_date_list.none@@");
		return result;
	}

	/// <summary>
	/// 配送希望時間帯表示文言取得（配送希望時間帯が利用不可の場合）
	/// </summary>
	/// <param name="shippingTime">配送希望時間帯</param>
	/// <returns>配送希望時間帯表示文言</returns>
	protected string GetShippingTimeMessage(string shippingTime)
	{
		if (this.ShopShipping == null) return string.Empty;

		for (var count = 1; count <= 10; count++)
		{
			if (shippingTime == StringUtility.ToEmpty(this.ShopShipping.DataSource["shipping_time_id" + count.ToString()]))
			{
				return StringUtility.ToEmpty(this.ShopShipping.DataSource["shipping_time_message" + count]);
			}
		}
		return string.Empty;
	}

	/// <summary>
	/// Get gmo cvs type
	/// </summary>
	private void GetGmoCvsType()
	{
		var isDisplayGmoCvsType = ((Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Gmo)
			&& (ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE)
			&& (this.OrderOld.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE));

		tbdGmoCvsType.Visible = isDisplayGmoCvsType;

		if (isDisplayGmoCvsType)
		{
			ddlGmoCvsType.Items.Clear();
			ddlGmoCvsType.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_PAYMENT, Constants.PAYMENT_GMO_CVS_TYPE));
		}
	}

	/// <summary>
	/// エラーメッセージ表示
	/// </summary>
	private void DisplayErrorMessages()
	{
		// 注文情報
		trOrderErrorMessagesTitle.Visible = (this.OrderErrorMessages.Length != 0);
		trOrderErrorMessages.Visible = (this.OrderErrorMessages.Length != 0);
		lbOrderErrorMessages.Text = this.OrderErrorMessages.ToString();
		// 注文者情報
		trOrderOwnerErrorMessagesTitle.Visible = (this.OrderOwnerErrorMessages.Length != 0);
		trOrderOwnerErrorMessages.Visible = (this.OrderOwnerErrorMessages.Length != 0);
		lbOrderOwnerErrorMessages.Text = this.OrderOwnerErrorMessages.ToString();
		// 配送料金、決済手数料
		trOrderPriceErrorMessagesTitle.Visible = (this.OrderPriceErrorMessages.Length != 0);
		trOrderPriceErrorMessages.Visible = (this.OrderPriceErrorMessages.Length != 0);
		lbOrderPriceErrorMessages.Text = this.OrderPriceErrorMessages.ToString();
		// クーポン情報
		if (Constants.W2MP_COUPON_OPTION_ENABLED)
		{
			trOrderCouponErrorMessagesTitle.Visible = (this.OrderCouponErrorMessages.Length != 0);
			trOrderCouponErrorMessages.Visible = (this.OrderCouponErrorMessages.Length != 0);
			lbOrderCouponErrorMessages.Text = HtmlSanitizer.HtmlEncode(this.OrderCouponErrorMessages.ToString());
		}
		// ポイント情報
		if (Constants.W2MP_POINT_OPTION_ENABLED)
		{
			trOrderPointErrorMessagesTitle.Visible = (this.OrderPointErrorMessages.Length != 0);
			trOrderPointErrorMessages.Visible = (this.OrderPointErrorMessages.Length != 0);
			lbOrderPointErrorMessages.Text = this.OrderPointErrorMessages.ToString();
		}
		// アフィリエイト情報
		if (Constants.W2MP_AFFILIATE_OPTION_ENABLED)
		{
			trOrderAffiliateErrorMessagesTitle.Visible = (this.OrderAffiliateErrorMessages.Length != 0);
			trOrderAffiliateErrorMessages.Visible = (this.OrderAffiliateErrorMessages.Length != 0);
			lbOrderAffiliateErrorMessages.Text = this.OrderAffiliateErrorMessages.ToString();
		}

		// 領収書情報
		if (Constants.RECEIPT_OPTION_ENABLED)
		{
			tbdyReceiptErrorMessages.Visible = (this.OrderReceiptErrorMessages.Length != 0);
			lbReceiptErrorMessages.Text = this.OrderReceiptErrorMessages.ToString();
		}

		// 注文拡張項目情報
		if (Constants.ORDER_EXTEND_OPTION_ENABLED)
		{
			trOrderExtendErrorMessagesTitle.Visible = (this.OrderExtendErrorMessages.Length != 0);
			trOrderExtendErrorMessages.Visible = (this.OrderExtendErrorMessages.Length != 0);
			lbOrderExtendErrorMessages.Text = this.OrderExtendErrorMessages.ToString();
		}
	}

	/// <summary>
	/// Display Uniform Invoice Error Message
	/// </summary>
	/// <param name="errorMessage">Error Message</param>
	/// <returns>Display Error Message</returns>
	private bool DisplayUniformInvoiceErrorMessage(string errorMessage)
	{
		lbInvoiceErrorMessages.Text = errorMessage;
		tbdyInvoiceErrorMessages.Visible = (string.IsNullOrEmpty(errorMessage) == false);
		if (tbdyInvoiceErrorMessages.Visible) ddlUniformInvoice.Focus();

		return tbdyInvoiceErrorMessages.Visible;
	}

	/// <summary>
	/// Get message for popup confirm
	/// </summary>
	/// <returns>Message</returns>
	private string GetMessageConfirm()
	{
		var message = new StringBuilder();
		if (this.OrderErrorMessages.Length != 0)
		{
			message.Append(this.OrderErrorMessages);
		}

		if (this.OrderOwnerErrorMessages.Length != 0)
		{
			message.Append(this.OrderOwnerErrorMessages);
		}

		if (this.OrderShippingErrorMessages.Length != 0)
		{
			message.Append(this.OrderShippingErrorMessages);
		}

		if (this.OrderItemErrorMessages.Length != 0)
		{
			message.Append(this.OrderItemErrorMessages);
		}

		return message.Replace("<br />", "\r\n").ToString();
	}

	/// <summary>
	/// 金額部分入力チェック
	/// </summary>
	/// <param name="order">注文入力情報</param>
	/// <param name="orderPoints">注文ポイント入力情報</param>
	private void CheckOrderPriceInput(OrderInput order, Hashtable[] orderPoints)
	{
		// 配送料金、決済手数料、セットプロモーション割引額、チェック
		this.OrderPriceErrorMessages.Append(order.ValidateForOrderPrice());

		// ポイント情報入力チェック
		if (Constants.W2MP_POINT_OPTION_ENABLED && this.IsUser)
		{
			var validatorName = this.OrderOld.IsExchangeOrder ? "OrderPointModifyInputForExchangeOrder" : "OrderPointModifyInput";

			var index = 1;
			foreach (var orderPoint in orderPoints)
			{
				this.OrderPointErrorMessages.Append(Validator.Validate(validatorName, orderPoint).Replace("@@ 1 @@", index.ToString()));

				// 交換注文の場合は、最終利用ポイントが0以上かを確認する
				if ((index == 1) && this.OrderPointErrorMessages.Length == 0)
				{
					var lastOrderPointUse = PriceCalculator.GetLastOrderPointUse(
						this.OrderOld.LastOrderPointUse,
						this.OrderOld.OrderPointUse,
						decimal.Parse((string)orderPoint[Constants.FIELD_ORDER_ORDER_POINT_USE]));

					this.OrderPointErrorMessages.Append(
						Validator.Validate(
							"OrderPointModifyInputForExchangeOrderMinCheck",
							new Hashtable
							{
								{ Constants.FIELD_ORDER_LAST_ORDER_POINT_USE, lastOrderPointUse.ToString() },
							}).Replace("@@ 1 @@", index.ToString()));
				}
				index++;
			}
		}

		// クーポン情報入力チェック
		if (Constants.W2MP_COUPON_OPTION_ENABLED)
		{
			this.OrderCouponErrorMessages.Append(order.ValidateForOrderCoupon());
		}
	}

	/// <summary>
	/// ポイント＆クーポン有効性チェック
	/// </summary>
	/// <param name="order">注文入力情報</param>
	/// <param name="orderPoints">注文ポイント入力情報</param>
	private void CheckPointAndCouponValidity(OrderInput order, Hashtable[] orderPoints)
	{
		// ポイント情報有効性チェック
		if ((Constants.W2MP_POINT_OPTION_ENABLED)
			&& (this.OrderPointErrorMessages.Length == 0))
		{
			this.OrderPointErrorMessages.Append(CheckPointValidity(order, orderPoints));
		}

		// クーポン情報有効性チェック
		if ((Constants.W2MP_COUPON_OPTION_ENABLED)
			&& (this.OrderCouponErrorMessages.Length == 0))
		{
			this.OrderCouponErrorMessages.Append(CheckCouponValidity(order));
		}
	}

	/// <summary>
	/// 広告コードチェック
	/// </summary>
	/// <param name="advCode">広告コード</param>
	/// <returns>エラーメッセージ</returns>
	private string CheckAdvCode(string advCode)
	{
		if (string.IsNullOrEmpty(advCode)) return string.Empty;

		var errorMessages = (new AdvCodeService().GetAdvCodeFromAdvertisementCode(advCode) == null)
			? WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ADVCODE_NO_EXIST_ERROR).Replace("@@ 1 @@", advCode) + "<br />"
			: string.Empty;
		return errorMessages;
	}

	/// <summary>
	/// 金額計算
	/// </summary>
	/// <param name="order">注文入力情報（※小計・合計が更新される）</param>
	/// <param name="targetErrorMessage">エラー追加対象エラーメッセージ</param>
	private void CalculatePrice(OrderInput order, StringBuilder targetErrorMessage)
	{
		// 各種金額取得
		// 商品小計取得（削除対象ではないもののみ）
		var orderPriceSubtotal = order.CalculateItemSubtotalExcludeDeleteTarget();

		// 配送料金、決済手数料、調整金額設定
		var orderPriceShipping = decimal.Parse(order.OrderPriceShipping);
		var orderPriceExchange = decimal.Parse(order.OrderPriceExchange);
		var orderPriceRegulationTotal = decimal.Parse(order.OrderPriceRegulation)
			+ order.OrderPriceByTaxRates.Sum(orderPriceByTaxRate => decimal.Parse(orderPriceByTaxRate.PriceCorrectionByRate));

		var memberRankDiscount = decimal.Parse(order.MemberRankDiscountPrice);
		var setPromotionProductDiscountAmount = order.SetPromotions.Sum(sp => decimal.Parse(sp.ProductDiscountAmount));
		var setPromotionShippingChargeDiscountAmount = order.SetPromotions.Sum(sp => decimal.Parse(sp.ShippingChargeDiscountAmount));
		var setPromotionPaymentChargeDiscountAmount = order.SetPromotions.Sum(sp => decimal.Parse(sp.PaymentChargeDiscountAmount));
		var orderCouponUse = decimal.Parse(order.OrderCouponUse);
		var orderPointUseYen = decimal.Parse(order.OrderPointUseYen);
		var fixedPurchaseMemberDiscountAmount = decimal.Parse(order.FixedPurchaseMemberDiscountAmount);
		var fixedPurchaseDiscountPrice = decimal.Parse(order.FixedPurchaseDiscountPrice);
		var orderPriceSubtotalTax = decimal.Parse(order.OrderPriceSubtotalTax);
		var orderPriceTax = decimal.Parse(order.OrderPriceTax);

		var orderPriceTotal = order.RecalculatePriceTotal();

		// 注文金額チェック（決済手段の金額範囲に含まれているかどうか）
		// 決済種別マスタチェック
		if (order.ReturnExchangeKbn != Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE)
		{
			this.OrderErrorMessages.Append(CheckPaymentPriceEnabled(this.LoginOperatorShopId, order.OrderPaymentKbn, orderPriceTotal, orderPointUseYen));
		}

		// セットプロモーション割引額整合性チェック
		// セットプロモーション毎に、商品合計金額 >= 商品割引額 であること
		var setPromotionErrorMessages = new StringBuilder();
		foreach (var setPromotion in order.SetPromotions.Where(os => os.IsDiscountTypeProductDiscount))
		{
			var undiscountedProductSubtotal = decimal.Parse(setPromotion.UndiscountedProductSubtotal);

			if ((undiscountedProductSubtotal > 0)
				&& (undiscountedProductSubtotal < decimal.Parse(setPromotion.ProductDiscountAmount)))
			{
				setPromotionErrorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERSETPROMOTION_PRODUCT_DISCOUNT_AMOUNT_ERROR).Replace("@@ 1 @@", setPromotion.OrderSetpromotionNo + "：" + setPromotion.SetpromotionName));
			}
		}

		var couponType = (order.Coupon != null) ? order.Coupon.CouponType : string.Empty;
		var couponFreeShippingFlg = ((order.Coupon != null) && (order.Coupon.CouponCode != ""))
			? new CouponService().GetCoupon(Constants.CONST_DEFAULT_DEPT_ID, order.Coupon.CouponId).FreeShippingFlg
			: Constants.FLG_COUPON_FREE_SHIPPING_INVALID;
		if (CouponOptionUtility.IsFreeShipping(couponType) || (couponFreeShippingFlg == Constants.FLG_COUPON_FREE_SHIPPING_VALID))
		{
			// 配送料無料クーポン設定中は、セットプロモーションの配送料割引は無効になる。配送料割引額が入っていたらエラー
			foreach (var setPromotion in order.SetPromotions.Where(os => os.IsDiscountTypeShippingChargeFree && (decimal.Parse(os.ShippingChargeDiscountAmount) != 0)))
			{
				setPromotionErrorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERSETPROMOTION_FREESHIPPING_COUPON_USED).Replace("@@ 1 @@", setPromotion.OrderSetpromotionNo + "：" + setPromotion.SetpromotionName));
			}
		}
		else
		{
			// 配送料割引額が配送料と異なっていたらエラー
			var shippingChargeDiscountAmount = order.SetPromotions.Where(s => s.IsDiscountTypeShippingChargeFree).ToArray();
			if ((shippingChargeDiscountAmount.Length != 0)
				&& orderPriceShipping != shippingChargeDiscountAmount.Sum(s => decimal.Parse(s.ShippingChargeDiscountAmount)))
			{
				setPromotionErrorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERSETPROMOTION_SHIPPING_CHARGE_DISCOUNT_AMOUNT_ERROR));
			}
		}

		// 決済手数料割引額が決済手数料と異なっていたらエラー
		var paymentChargeDiscountAmount = order.SetPromotions.Where(s => s.IsDiscountTypePaymentChargeFree).ToArray();
		if ((paymentChargeDiscountAmount.Length != 0)
			&& orderPriceExchange != paymentChargeDiscountAmount.Sum(s => decimal.Parse(s.PaymentChargeDiscountAmount)))
		{
			setPromotionErrorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERSETPROMOTION_PAYMENT_CHARGE_DISCOUNT_AMOUNT_ERROR));
		}

		this.OrderPriceErrorMessages.Append(setPromotionErrorMessages);

		// 共通整合性チェック
		// クーポンオプション又は、ポイントオプション、会員ランク割引が有効の場合
		if ((setPromotionErrorMessages.Length == 0) && (Constants.W2MP_COUPON_OPTION_ENABLED || Constants.W2MP_POINT_OPTION_ENABLED || Constants.MEMBER_RANK_OPTION_ENABLED))
		{
			// クーポン割引額の内、商品から割り引かれる金額(配送無料クーポンではクーポン割引額が0で計算)
			var couponDiscountForProduct = (CouponOptionUtility.IsFreeShipping(couponType) == false) ? orderCouponUse : 0m;

			// 商品合計 + 配送料 + 調整金額 + 決済手数料 + 調整金額 < (会員ランク割引 + クーポン割引額 + ポイント利用額 + セットプロモーション割引額（商品割引分） + セットプロモーション配送料割引額 + セットプロモーション決済手数料割引額)、
			// 会員ランク割引＋クーポン割引額 + ポイント利用額が0以上の場合（※返品交換注文の場合、商品合計が0以下も考えられるため）
			if (orderPriceSubtotal + orderPriceShipping + orderPriceExchange + orderPriceRegulationTotal < (memberRankDiscount + orderCouponUse + orderPointUseYen + setPromotionProductDiscountAmount + setPromotionShippingChargeDiscountAmount + setPromotionPaymentChargeDiscountAmount) && ((memberRankDiscount + couponDiscountForProduct + orderPointUseYen) > 0))
			{
				var errorTarget = new StringBuilder();
				if (order.SetPromotions.Any(sp => sp.IsDiscountTypeProductDiscount))
				{
					errorTarget.Append(ReplaceTag("@@DispText.order_setpromotion.MemberRankDiscoutPrice@@"));
				}
				if (Constants.MEMBER_RANK_OPTION_ENABLED)
				{
					errorTarget.Append((errorTarget.Length != 0) ? "、" : string.Empty);
					errorTarget.Append(ReplaceTag("@@DispText.order.MemberRankDiscountPrice@@"));
				}
				if (Constants.W2MP_POINT_OPTION_ENABLED)
				{
					errorTarget.Append((errorTarget.Length != 0) ? "、" : string.Empty);
					errorTarget.Append(ReplaceTag("@@DispText.order.PointUseAmout@@"));
				}
				if ((Constants.W2MP_COUPON_OPTION_ENABLED)
					&& (CouponOptionUtility.IsFreeShipping(couponType) == false))
				{
					errorTarget.Append((errorTarget.Length != 0) ? "、" : string.Empty);
					errorTarget.Append(ReplaceTag("@@Order.order_coupon_use.name@@"));
				}
				targetErrorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_ORDER_ORDER_PRICE_SUBTOTAL_MINUS).Replace("@@ 1 @@", errorTarget.ToString()));
			}
		}

		// 配送無料クーポン、配送料とクーポン割引額の同値チェック
		if (CouponOptionUtility.IsFreeShipping(couponType)
			&& (orderPriceShipping != orderCouponUse))
		{
			targetErrorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_COUPON_FREESHIPPING_INEQUIVALENCE));
		}

		// チェックを通れば値セット（金額・アイテムのみ）
		if (this.HasError != false) return;
		order.OrderPriceTotal = orderPriceTotal.ToString();
		order.OrderPointUseYen = orderPointUseYen.ToString();
		order.OrderCouponUse = orderCouponUse.ToString();
		order.SetpromotionProductDiscountAmount = setPromotionProductDiscountAmount.ToString();
		order.SetpromotionShippingChargeDiscountAmount = setPromotionShippingChargeDiscountAmount.ToString();
		order.SetpromotionPaymentChargeDiscountAmount = setPromotionPaymentChargeDiscountAmount.ToString();
		order.OrderPriceTax = orderPriceTax.ToString();
		order.OrderPriceSubtotalTax = orderPriceSubtotalTax.ToString();
	}

	/// <summary>
	/// クーポン情報有効性チェック
	/// </summary>
	/// <param name="order">注文入力情報</param>
	private string CheckCouponValidity(OrderInput order)
	{
		// クーポン割引額取得
		decimal orderCouponUse = 0;
		decimal.TryParse(order.OrderCouponUse, out orderCouponUse);

		// エラーの場合は0円とする
		order.OrderCouponUse = "0";

		if (!Constants.W2MP_COUPON_OPTION_ENABLED) return string.Empty;
		// クーポン情報設定（※メソッド「CalculatePrice」にて金額計算に利用されているため、格納）
		var orderCoupon = order.Coupon;
		if (this.OrderInputOld.Coupon != null)
		{
			orderCoupon.CouponType = this.OrderInputOld.Coupon.CouponType;
			orderCoupon.DeptId = this.OrderInputOld.Coupon.DeptId;
			orderCoupon.CouponId = this.OrderInputOld.Coupon.CouponId;
			orderCoupon.CouponNo = this.OrderInputOld.Coupon.CouponNo;
			orderCoupon.CouponDiscountPrice = this.OrderInputOld.Coupon.CouponDiscountPrice;
			orderCoupon.CouponDiscountRate = this.OrderInputOld.Coupon.CouponDiscountRate;
		}

		// クーポンコードが存在する場合
		var couponCode = order.Coupon.CouponCode;
		var couponCodeOld = (this.OrderInputOld.Coupon != null) ? this.OrderInputOld.Coupon.CouponCode : string.Empty;
		if (couponCode != "")
		{
			// クーポンコードに変更があった場合
			if (couponCode != couponCodeOld)
			{
				// クーポンコード所持チェック
				// クーポン情報取得（出力パラメタに格納）
				var coupons = new CouponService().GetAllUserCouponsFromCouponCode(
					this.LoginOperatorDeptId,
					this.OrderInput.UserId,
					couponCode);
				if (coupons.Length == 0)
				{
					return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_COUPON_NO_COUPONCODE).Replace("@@ 1 @@", couponCode);
				}

				// クーポン設定（取得したクーポンで設定を上書）
				orderCoupon.CouponType = coupons[0].CouponType;

				// 未使用クーポンチェック(回数制限有りクーポンのみ)
				UserCouponDetailInfo targetCoupon = null;
				var couponErrorCode = CouponErrorcode.NoError;
				foreach (var coupon in coupons)
				{
					// 未使用の場合は利用クーポン確定
					couponErrorCode = CouponOptionUtility.CheckUseCoupon(coupon, this.OrderInput.UserId, this.OrderInput.Owner.OwnerMailAddr);
					if (couponErrorCode != CouponErrorcode.NoError) continue;
					targetCoupon = coupon;
					break;
				}
				if (targetCoupon == null)
				{
					return CouponOptionUtility.GetErrorMessage(couponErrorCode).Replace("@@ 1 @@", couponCode);
				}

				// カート作成
				var cart = CreateCart(order);
				// 再計算
				cart.Calculate(true);
				// クーポン有効チェック
				var errorMessage = CouponOptionUtility.CheckCouponValidWithCart(cart, targetCoupon);
				if (errorMessage != "") return errorMessage.Replace("@@ 1 @@", couponCode);

				// クーポン情報セット
				orderCoupon.DeptId = coupons[0].DeptId;
				orderCoupon.CouponId = coupons[0].CouponId;
				orderCoupon.CouponNo = (coupons[0].CouponNo != null) ? coupons[0].CouponNo.ToString() : "1";
				orderCoupon.CouponDiscountPrice = (coupons[0].DiscountPrice != null) ? coupons[0].DiscountPrice.ToString() : null;
				orderCoupon.CouponDiscountRate = (coupons[0].DiscountRate != null) ? coupons[0].DiscountRate.ToString() : null;
			}
			else
			{
				var coupons = new CouponService().GetAllUserCouponsFromCouponId(
					this.LoginOperatorDeptId,
					this.OrderInput.UserId,
					order.Coupon.CouponId,
					int.Parse(order.Coupon.CouponNo));
				if (coupons.Length == 0)
				{
					return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_COUPON_NO_COUPONCODE)
						.Replace("@@ 1 @@", couponCode);
				}
			}
		}
		else
		{
			// クーポン割引額チェック
			if (orderCouponUse > 0)
			{
				return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_COUPON_DISCOUNT_PRICE);
			}
		}

		// クーポンリスト＆クーポン割引額セット
		order.Coupons = new[] { orderCoupon };
		order.OrderCouponUse = orderCouponUse.ToString();

		return string.Empty;
	}

	/// <summary>
	/// 注文ポイント情報有効性チェック
	/// </summary>
	/// <param name="order">注文入力情報</param>
	/// <param name="orderPoints">注文ポイント入力情報</param>
	private string CheckPointValidity(OrderInput order, Hashtable[] orderPoints)
	{
		decimal orderPointUse = 0;
		decimal orderPointUseYen = 0;
		var pointService = new PointService();

		if (Constants.W2MP_POINT_OPTION_ENABLED && this.IsUser)
		{
			// 利用ポイント取得（変更後）
			orderPointUse = decimal.Parse((string)orderPoints[0][Constants.FIELD_ORDER_ORDER_POINT_USE]);

			// 利用ポイントチェック
			var userPointUsable = PriceCalculator.GetUserPointUsable(
				decimal.Parse(this.OrderInputOld.OrderPointUse),
				pointService.GetUserPointUsableTotal(this.OrderInput.UserId),
				orderPoints.Sum(p => decimal.Parse((string)p[Constants.FIELD_ORDER_ORDER_POINT_ADD])),
				decimal.Parse(this.OrderInputOld.OrderPointAdd),
				(string)orderPoints[0][Constants.FIELD_USERPOINT_POINT_TYPE]);

			// 補正（本ポイントがマイナスだとポイントチェックでエラーが発生するため）
			var dUserPointCorrectedZero = (userPointUsable > 0) ? userPointUsable : 0;

			// 「本ポイント + 利用ポイント(変更前) < (利用ポイント(変更後)」の場合はエラーとする
			if (dUserPointCorrectedZero < orderPointUse)
			{
				return WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_ORDER_ORDER_POINT_USE_MAX).Replace("@@ 1 @@", StringUtility.ToNumeric(userPointUsable) + "pt");
			}

			// ポイント利用可能単位チェック
			var pointMaster = pointService.GetPointMaster().FirstOrDefault();
			// 単位チェック
			if ((orderPointUse % pointMaster.UsableUnit) != 0)
			{
				return WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_POINT_USABLE_UNIT_ERROR).Replace("@@ 1 @@", StringUtility.ToNumeric(pointMaster.UsableUnit));
			}

			// ポイント利用金額取得（出力パラメタに格納）
			orderPointUseYen = PointOptionUtility.GetOrderPointUsePriceDecimal(orderPointUse, pointMaster);
		}

		// 利用ポイント数、ポイント利用額セット
		order.OrderPointUse = orderPointUse.ToString();
		order.OrderPointUseYen = orderPointUseYen.ToString();

		return string.Empty;
	}

	/// <summary>
	/// 利用可能な決済種別か？チェック
	/// </summary>
	/// <param name="shopId">店舗ID</param>
	/// <param name="userManagementLevelId">ユーザー管理レベルID</param>
	private void DisplayPaymentUserManagementLevel(string shopId, string userManagementLevelId)
	{
		var message = (this.IsUser)
			? GetPaymentUserManagementLevelNotUseMessage(shopId, userManagementLevelId)
			: "";
		lbSelectPaymentUserManagementLevelMessage.Text = message;
		tbdySelectPaymentUserManagementLevelMessage.Visible = (message != "");
		lbPaymentUserManagementLevelMessage.Text = message;
		lbPaymentUserManagementLevelAlertMessage.Text = message;
		SetPaymentNoticeMessageVisibility();
	}

	/// <summary>
	/// 注文者区分選択できない決済種別を文言で表示
	/// </summary>
	/// <param name="shopId">ShopID</param>
	/// <param name="userKbn">注文者区分</param>
	private void DisplayPaymentOrderOwnerKbn(string shopId, string userKbn)
	{
		string message = GetPaymentOrderOwnerKbnNotUseMessage(shopId, userKbn);
		lbPaymentOrderOwnerKbnMessage.Text = message;
		lbPaymentOrderOwnerKbnAlertMessage.Text = message;
		SetPaymentNoticeMessageVisibility();
	}

	/// <summary>
	/// 商品で制御されている決済種別メッセージ表示
	/// </summary>
	private void DisplayLimitedPaymentMessages()
	{
		var limitedPaymentMessages = string.Empty;
		foreach (RepeaterItem shippingItem in rShippingList.Items)
		{
			var productList = new List<KeyValuePair<string, string>>();
			foreach (RepeaterItem item in ((Repeater)shippingItem.FindControl("rItemList")).Items)
			{
				var productId = ((TextBox)item.FindControl("tbProductId")).Text;
				var variationId = string.Format("{0}{1}", productId, ((TextBox)item.FindControl("tbVariationId")).Text);

				productList.Add(new KeyValuePair<string, string>(productId, variationId));
			}

			limitedPaymentMessages += string.Format("{0}{1}", GetProductsLimitedPaymentMessage(this.LoginOperatorShopId, productList), Environment.NewLine);
		}

		lbPaymentLimitedMessage.Text = limitedPaymentMessages.Trim();
		SetPaymentNoticeMessageVisibility();
	}

	/// <summary>
	/// 決済種別の注意文言を表示、非表示
	/// </summary>
	private void SetPaymentNoticeMessageVisibility()
	{
		tbPaymentNoticeMessage.Visible = tbdyPaymentNoticeMessage.Visible =
			((string.IsNullOrEmpty(lbPaymentLimitedMessage.Text) == false)
				|| (string.IsNullOrEmpty(lbPaymentUserManagementLevelMessage.Text) == false)
				|| (string.IsNullOrEmpty(lbPaymentOrderOwnerKbnMessage.Text) == false));
	}

	/// <summary>
	/// 配送会社リストアイテム取得
	/// </summary>
	/// <param name="shippingMethod">配送方法</param>
	/// <param name="userShipping">User Shipping</param>
	/// <returns>配送会社リストアイテム</returns>
	public ListItemCollection GetDeliveryCompanyItemList(string shippingMethod, string userShipping)
	{
		if (this.ShopShipping == null) return null;
		var result = new ListItemCollection();
		var shippingCompanyList = (shippingMethod == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS ? this.ShopShipping.CompanyListExpress : this.ShopShipping.CompanyListMail);

		shippingCompanyList = ((userShipping == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
			? shippingCompanyList
				.Where(company => (company.DeliveryCompanyId == Constants.TWPELICANEXPRESS_CONVENIENCE_STORE_ID)).ToArray()
			: shippingCompanyList
				.Where(company => (company.DeliveryCompanyId != Constants.TWPELICANEXPRESS_CONVENIENCE_STORE_ID)).ToArray());

		result.AddRange
			(shippingCompanyList
			.Select(shippingCompany => new ListItem(this.DeliveryCompanyList
				.First(i => i.DeliveryCompanyId == shippingCompany.DeliveryCompanyId)
				.DeliveryCompanyName
				, shippingCompany.DeliveryCompanyId))
				.OrderBy(item => this.DeliveryCompanyList.First(company => (company.DeliveryCompanyId == item.Value)).DisplayOrder)
				.ThenBy(item => item.Value)
				.ToArray());
		this.DelilveryCompanyList = result;

		// デフォルト配送会社ID取得
		var shippingCompanyDefault = shippingCompanyList
			.FirstOrDefault(list => list.DefaultDeliveryCompany == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_DEFAULT_DELIVERY_COMPANY_VALID);
		if (shippingCompanyDefault != null)
		{
			this.DefaultDeliveryCompany = shippingCompanyDefault.DeliveryCompanyId;
		}

		if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED)
		{
			result.Clear();
			result.AddRange(
				OrderCommon.GetShippingServiceRelatedProduct(
				this.ShopShipping.ShopId,
				this.ShopShipping.ShippingId,
				shippingMethod,
				userShipping == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
				.OrderBy(item => this.DeliveryCompanyList.First(company => (company.DeliveryCompanyId == item.Value)).DisplayOrder)
				.ThenBy(item => item.Value)
				.ToArray());
		}

		return result;
	}

	/// <summary>
	/// Select User Shipping
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlUserShipping_SelectedIndexChanged(object sender, EventArgs e)
	{
		var repeater = (RepeaterItem)((DropDownList)sender).Parent;

		var ddlShippingMethod = ((DropDownList)repeater.FindControl("ddlShippingMethod"));
		var ddlDeliveryCompany = ((DropDownList)repeater.FindControl("ddlDeliveryCompany"));
		var ddlShippingTime = ((DropDownList)repeater.FindControl("ddlShippingTime"));
		var ddlUserShipping = ((DropDownList)repeater.FindControl("ddlUserShipping"));
		var index = repeater.ItemIndex;

		this.ShippingMethod[index] = ddlShippingMethod.SelectedValue;
		this.DeliveryServiceSelected = ddlDeliveryCompany.SelectedIndex;

		// 配送会社
		ddlDeliveryCompany.Items.Clear();
		var companyList = (ddlShippingMethod.SelectedValue == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS)
			? this.ShopShipping.CompanyListExpress
			: this.ShopShipping.CompanyListMail;

		companyList =
			(ddlUserShipping.SelectedValue == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE)
				? companyList
					.Where(company =>
						(company.DeliveryCompanyId == Constants.TWPELICANEXPRESS_CONVENIENCE_STORE_ID)).ToArray()
				: companyList
					.Where(company =>
						(company.DeliveryCompanyId != Constants.TWPELICANEXPRESS_CONVENIENCE_STORE_ID))
					.ToArray();

		foreach (var deliveryCompany in companyList)
		{
			var company = this.DeliveryCompanyList.First(item => (item.DeliveryCompanyId == deliveryCompany.DeliveryCompanyId));
			ddlDeliveryCompany.Items.Add(new ListItem(company.DeliveryCompanyName, company.DeliveryCompanyId));
		}

		ddlDeliveryCompany.SelectedValue = companyList.First().DeliveryCompanyId;

		if (ddlUserShipping.SelectedValue != CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE)
		{
			ddlDeliveryCompany.SelectedIndex = this.DeliveryServiceSelected;
		}

		if (companyList.Any())
		{
			this.DeliveryCompany[index] = this.DeliveryCompanyList.First(i => i.DeliveryCompanyId == companyList[0].DeliveryCompanyId);
		}

		ddlDeliveryCompany_SelectedIndexChanged(sender, e);

		// 決済種別
		var paymentSelectionFlg = string.Empty;
		var permittedPaymentIds = string.Empty;
		if (this.ShopShipping != null)
		{
			paymentSelectionFlg = this.ShopShipping.PaymentSelectionFlg;
			permittedPaymentIds = this.ShopShipping.PermittedPaymentIds;
		}
		var payments =
			GetPaymentValidListPermission(this.LoginOperatorShopId, paymentSelectionFlg, permittedPaymentIds);
		// 決済種別が存在しない場合はエラー
		if (payments.Length == 0)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PAYMENT_NO_DATA);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// Remove invalid payment for store pickup
		if (this.IsStorePickup)
		{
			payments = GetValidStorePickupPayments();
		}

		var orderPaymentOld = ddlOrderPaymentKbn.SelectedValue;
		ddlOrderPaymentKbn.Items.Clear();
		ddlOrderPaymentKbn.Items.Add(new ListItem(string.Empty, string.Empty));
		foreach (var payment in payments)
		{
			if ((this.ShippingMethod[index] == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL)
				&& (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT))
			{
				continue;
			}

			ddlOrderPaymentKbn.Items.Add(new ListItem(payment.PaymentName, payment.PaymentId));
		}

		if (ddlOrderPaymentKbn.Items.FindByValue(orderPaymentOld) != null)
		{
			ddlOrderPaymentKbn.SelectedValue = orderPaymentOld;
		}

		// Reset shipping receiving store type and payment
		if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED
			&& Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED)
		{
			var ddlShippingReceivingStoreType = ((DropDownList)repeater.FindControl("ddlShippingReceivingStoreType"));
			var btnOpenConvenienceStoreMapEcPay = ((Button)repeater.FindControl("btnOpenConvenienceStoreMapEcPay"));
			if ((ddlUserShipping.SelectedValue != CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW)
				&& (ddlUserShipping.SelectedValue != CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE))
			{
				var userShipping = this.UserShippingAddress.FirstOrDefault(shipping =>
					((shipping.ShippingNo.ToString() == ddlUserShipping.SelectedValue)
					&& (shipping.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
					&& (string.IsNullOrEmpty(shipping.ShippingReceivingStoreType) == false)));
				if (userShipping != null)
				{
					ddlShippingReceivingStoreType.Enabled = false;
					btnOpenConvenienceStoreMapEcPay.Visible = false;
					ddlShippingReceivingStoreType.SelectedValue = userShipping.ShippingReceivingStoreType;
					ddlShippingReceivingStoreType_SelectedIndexChanged(sender, e);
					this.OrderInput.Shippings[0].ShippingReceivingStoreType = userShipping.ShippingReceivingStoreType;
					this.OrderInput.OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE;
				}
			}
			else if (ddlUserShipping.SelectedValue == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE)
			{
				ddlShippingReceivingStoreType.Enabled = true;
				btnOpenConvenienceStoreMapEcPay.Visible = true;
				ddlShippingReceivingStoreType.SelectedIndex = 0;
			}
		}

		ViewState.Remove("DeliveryServiceSelected");

		ControlPaymentWhenShippingConvenience();
		ddlOrderPaymentKbn_SelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// 選択された配送会社の有効チェック
	/// </summary>
	/// <param name="deliveryCompanyId">配送会社ID</param>
	/// <param name="orderShippingNo">注文番号</param>
	/// <returns>配送会社ID</returns>
	/// <remarks>配送会社リストに含まれる場合はそのまま、含まれない場合はデフォルト配送会社とする</remarks>
	public string CheckSelectDeliveryCompany(string deliveryCompanyId, string orderShippingNo)
	{
		if (this.DelilveryCompanyList.FindByValue(deliveryCompanyId) != null) return deliveryCompanyId;

		this.OrderShippingErrorMessages.Append(
			WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERREGIST_DELIVERYCOMPANY_ERROR)
				.Replace("@@ 1 @@", orderShippingNo));
		return this.DefaultDeliveryCompany;
	}

	/// <summary>
	/// クレジットカード売上確定後キャンセル可能か？
	/// キャンセル不可の場合はエラーメッセージ表示
	/// </summary>
	private void CheckCancelableForCreditCardSalesCompleteOrder()
	{
		// 再与信しない？
		if (this.ddlNewExecuteType.SelectedValue == ReauthCreatorFacade.ExecuteTypes.None.ToString()) return;

		// 注文情報取得
		var order = new OrderService().GetOrderInfoByOrderId(this.RequestOrderId);

		// クレジットカード売上確定後キャンセル可能かできるか？
		var isCancelable = OrderCommon.IsCancelableForCreditCardSalesCompleteOrder(order);
		// エラーの場合、エラーメッセージセット
		if (OrderCommon.IsCancelableForCreditCardSalesCompleteOrder(order)) return;
		var paymentErrorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_CREDIT_SALES_COMPLETE_CANCEL_ERROR);
			DisplayOrderPaymentErrorMessage(paymentErrorMessage);
	}

	/// <summary>
	/// 決済情報入力チェック
	/// </summary>
	/// <param name="input">入力情報</param>
	/// <returns>結果</returns>
	private bool CheckOrderCreditCardInput(OrderCreditCardInput input)
	{
		// 入力カード情報なし、再与信オプション無効、又は、モール注文の場合チェックしない
		if ((input == null)
			|| (Constants.PAYMENT_REAUTH_ENABLED == false)
			|| (this.OrderInput.IsPermitReauthOrderSiteKbn == false)) return false;

		// ZEUSではクレジットカード入力不可のときはエラーとする。
		if ((Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Zeus)
			&& (input.CreditBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
			&& (this.CanUseCreditCardNoForm == false))
		{
			this.OrderPaymentErrorMessages.Append("dummy");
			ddlOrderPaymentKbn.Focus();
			return false;
		}

		// エラーメッセージセット
		var paymentErrorMessage = "";
		if (((this.IsNotRakutenAgency == false) && (this.IsUserPayTg == false))
			&& (input.CreditBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW))
		{
			paymentErrorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_RAKUTEN_CREDIT_NOT_EXIST_USER_CARD);
		}
		else
		{
			paymentErrorMessage = input.Validate();
		}
		DisplayOrderPaymentErrorMessage(paymentErrorMessage);
		return string.IsNullOrEmpty(paymentErrorMessage);
	}

	/// <summary>
	/// 頒布会注文商品・価格入力チェック
	/// </summary>
	/// <param name="order">注文入力情報</param>
	/// <returns>エラーメッセージ</returns>
	private void CheckSubscriptionBoxItemsAndPricesInput(OrderInput order)
	{
		// 頒布会コース選択時、商品合計金額下限（税込）・商品合計金額上限（税込）を満たしているか
		if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED && string.IsNullOrEmpty(this.OrderInput.SubscriptionBoxCourseId) == false)
		{
			var orderPriceSubtotal = order.Shippings.SelectMany(s => s.Items)
				.Where(orderItem => (orderItem.DeleteTarget == false)).Sum(orderItem => decimal.Parse(orderItem.ItemPrice));
			lbSubscriptionBoxOrderPriceErrorMessages.Text = OrderCommon.GetSubscriptionBoxTotalAmountError(
				this.OrderInput.SubscriptionBoxCourseId,
				orderPriceSubtotal);

			trOrderPriceErrorMessagesTitle.Visible =
				trSubscriptionBoxOrderPriceErrorMessages.Visible =
					string.IsNullOrEmpty(lbSubscriptionBoxOrderPriceErrorMessages.Text) == false;
		}
	}

	/// <summary>
	/// Select User Invoice Of Uniform Invoice Type Or Carry Type Option
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlUniformInvoiceOrCarryTypeOption_SelectedIndexChanged(object sender, EventArgs e)
	{
		tbCarryTypeOption1.Text
			= tbCarryTypeOption2.Text
			= tbCompanyOption1.Text
			= tbCompanyOption2.Text
			= tbDonateOption1.Text
			= lbCompanyOption1.Text
			= lbCompanyOption2.Text
			= lbDonateOption1.Text
			= string.Empty;

		if (string.IsNullOrEmpty(ddlUniformInvoiceOrCarryTypeOption.SelectedValue) == false)
		{
			var userInvoice = new TwUserInvoiceService().Get(this.OrderInput.UserId, int.Parse(ddlUniformInvoiceOrCarryTypeOption.SelectedValue));

			if (userInvoice == null) return;

			switch (ddlUniformInvoice.SelectedValue)
			{
				case Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL:
					switch (ddlCarryType.SelectedValue)
					{
						case Constants.FLG_ORDER_TW_CARRY_TYPE_MOBILE:
							tbCarryTypeOption1.Text = userInvoice.TwCarryTypeOption;
							tbCarryTypeOption1.Enabled = false;
							break;
						case Constants.FLG_ORDER_TW_CARRY_TYPE_CERTIFICATE:
							tbCarryTypeOption2.Text = userInvoice.TwCarryTypeOption;
							tbCarryTypeOption2.Enabled = false;
							break;
					}
					break;

				case Constants.FLG_TW_UNIFORM_INVOICE_COMPANY:
					tbCompanyOption1.Text = lbCompanyOption1.Text = userInvoice.TwUniformInvoiceOption1;
					tbCompanyOption1.Enabled = false;
					tbCompanyOption2.Text = lbCompanyOption2.Text = userInvoice.TwUniformInvoiceOption2;
					tbCompanyOption2.Enabled = false;
					break;

				case Constants.FLG_TW_UNIFORM_INVOICE_DONATE:
					tbDonateOption1.Text = lbDonateOption1.Text = userInvoice.TwUniformInvoiceOption1;
					tbDonateOption1.Enabled = false;
					break;
			}
		}
		else
		{
			tbDonateOption1.Enabled = true;
			tbCompanyOption1.Enabled = true;
			tbCompanyOption2.Enabled = true;
			tbCarryTypeOption1.Enabled = true;
			tbCarryTypeOption2.Enabled = true;
		}
		SetVisibleForCarryTypeOption(ddlCarryType.SelectedItem.Value);
		SetVisibleForUniformOption(ddlUniformInvoice.SelectedItem.Value);

		ddlUniformInvoiceOrCarryTypeOption.Visible =
			((this.IsPersonal && (string.IsNullOrEmpty(ddlCarryType.SelectedValue) == false))
				|| this.IsCompany
				|| this.IsDonate);
	}

	/// <summary>
	/// Select Uniform Invoice Type
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlUniformInvoice_SelectedIndexChanged(object sender, EventArgs e)
	{
		tbCarryTypeOption1.Text
			= tbCarryTypeOption2.Text
			= tbCompanyOption1.Text
			= tbCompanyOption2.Text
			= tbDonateOption1.Text
			= lbCompanyOption1.Text
			= lbCompanyOption2.Text
			= lbDonateOption1.Text
			= string.Empty;

		// Refresh Uniform Invoice Or Carry Type Option
		ddlUniformInvoiceOrCarryTypeOption.DataSource = GetUniformInvoiceOrCarryTypeOption(ddlUniformInvoice.SelectedValue, ddlCarryType.SelectedValue);
		ddlUniformInvoiceOrCarryTypeOption.DataBind();

		tbdyInvoiceErrorMessages.Visible = false;
		SetVisibleForCarryTypeOption(ddlCarryType.SelectedItem.Value);
		SetVisibleForUniformOption(ddlUniformInvoice.SelectedItem.Value);

		ddlUniformInvoiceOrCarryTypeOption.Visible =
			((this.IsPersonal && (string.IsNullOrEmpty(ddlCarryType.SelectedValue) == false))
				|| this.IsCompany
				|| this.IsDonate);
	}

	/// <summary>
	/// Select Carry Type
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlCarryType_SelectedIndexChanged(object sender, EventArgs e)
	{
		tbCarryTypeOption1.Text
			= tbCarryTypeOption2.Text
			= tbCompanyOption1.Text
			= tbCompanyOption2.Text
			= tbDonateOption1.Text
			= lbCompanyOption1.Text
			= lbCompanyOption2.Text
			= lbDonateOption1.Text
			= string.Empty;

		// Refresh Uniform Invoice Or Carry Type Option
		ddlUniformInvoiceOrCarryTypeOption.DataSource = GetUniformInvoiceOrCarryTypeOption(ddlUniformInvoice.SelectedValue, ddlCarryType.SelectedValue);
		ddlUniformInvoiceOrCarryTypeOption.DataBind();

		tbdyInvoiceErrorMessages.Visible = false;
		SetVisibleForCarryTypeOption(ddlCarryType.SelectedItem.Value);
		SetVisibleForUniformOption(ddlUniformInvoice.SelectedItem.Value);

		ddlUniformInvoiceOrCarryTypeOption.Visible =
			((this.IsPersonal && (string.IsNullOrEmpty(ddlCarryType.SelectedValue) == false))
				|| this.IsCompany
				|| this.IsDonate);
	}

	/// <summary>
	/// Get Uniform Invoice Or Carry Type Option
	/// </summary>
	/// <param name="uniformInvoiceType">Uniform Invoice Type</param>
	/// <param name="invoiceCarryType">Invoice Carry Type</param>
	/// <returns>List Item</returns>
	protected ListItemCollection GetUniformInvoiceOrCarryTypeOption(string uniformInvoiceType, string invoiceCarryType)
	{
		var listItem = new ListItemCollection
		{
			new ListItem(ReplaceTag("@@DispText.invoice_carry_type_option.new@@"), string.Empty),
		};

		var userInvoice = (string.IsNullOrEmpty(this.OrderInput.UserId)
			? null
			: new TwUserInvoiceService().GetAllUserInvoiceByUserId(this.OrderInput.UserId));
		if (userInvoice == null) return listItem;
		if (uniformInvoiceType == Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL)
		{
			if (uniformInvoiceType != Constants.FLG_ORDER_TW_CARRY_TYPE_NONE)
				listItem.AddRange(userInvoice
					.Where(item => (item.TwUniformInvoice == uniformInvoiceType) && (item.TwCarryType == invoiceCarryType))
					.Select(item => new ListItem(item.TwInvoiceName, item.TwInvoiceNo.ToString())).ToArray());
		}
		else
		{
			listItem.AddRange(userInvoice
				.Where(item => (item.TwUniformInvoice == uniformInvoiceType))
				.Select(item => new ListItem(item.TwInvoiceName, item.TwInvoiceNo.ToString())).ToArray());
		}

		return listItem;
	}

	/// <summary>
	/// 商品情報表のデザイン変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="repeater">コントローラ</param>
	/// <param name="isFixedPurchaseProduct">定期チェックボックス入力値</param>
	private void ChangeOrderItemsTable(object sender, Control repeater, bool isFixedPurchaseProduct)
	{
		var repeaterItems = ((Repeater)((CheckBox)sender).Parent.Parent.Parent.Parent).Items;

		// 商品情報上の全体エラーメッセージエリア
		var tableDataOrderItemErrorMessagesTitle = (HtmlTableCell)rShippingList.Items[index: 0].FindControl("tdOrderItemErrorMessagesTitle");
		var tableDataOrderItemErrorMessagesTitleColSpan = tableDataOrderItemErrorMessagesTitle.ColSpan;
		// 商品情報上のエラーメッセージエリア
		var tableDataOrderItemErrorMessages = (HtmlTableCell)rShippingList.Items[index: 0].FindControl("tdOrderItemErrorMessages");
		var tableDataOrderItemErrorMessagesColSpan = tableDataOrderItemErrorMessagesTitle.ColSpan;
		// 商品情報エリア
		var tableOrderItems = (HtmlTableCell)rShippingList.Items[index: 0].FindControl("tdOrderItems");
		var tableOrderItemsColSpan = tableOrderItems.ColSpan;

		// 定期購入回数エリア
		var tableDataFixedPurchaseItemCount = rShippingList.Items[index: 0].FindControl("tdFixedPurchaseItemCount");
		// 注文基準エリア
		var tableDataFixedPurchaseItemOrderCount = rShippingList.Items[index: 0].FindControl("tdFixedPurchaseItemOrderCount");
		// 出荷基準エリア
		var tableDataFixedPurchaseItemShippedCount = rShippingList.Items[index: 0].FindControl("tdFixedPurchaseItemShippedCount");

		// 商品情報、商品ごと定期購入回数（注文基準）入力テキストボックス
		var tableDataFixedPurchaseItemOrderCountTextBox = (TextBox)repeater.FindControl("tbFixedPurchaseItemOrderCount");
		// 商品情報、商品ごと定期購入回数（出荷基準）入力テキストボックス
		var tableDataFixedPurchaseItemShippedCountTextBox = (TextBox)repeater.FindControl("tbFixedPurchaseItemShippedCount");
		// 商品情報、商品ごと定期購入回数（注文基準）表示ラベル
		var tableDataFixedPurchaseItemOrderCountLabel = (Label)repeater.FindControl("lbFixedPurchaseItemOrderCount");
		// 商品情報、商品ごと定期購入回数（出荷基準）表示ラベル
		var tableDataFixedPurchaseItemShippedCountLabel = (Label)repeater.FindControl("lbFixedPurchaseItemShippedCount");

		// チェック入れ／外し動作後、チェック入れ状態のチェックボックスの数を計算する
		var countFixedPurchaseCheck = 0;
		foreach (RepeaterItem item in repeaterItems)
		{
			var cbFixedPurchaseProduct = (CheckBox)item.FindControl("cbFixedPurchaseProduct");
			if (cbFixedPurchaseProduct.Checked)
			{
				countFixedPurchaseCheck++;
			}
		}

		// 最初のチェック入れと最後のチェック外し
		if ((isFixedPurchaseProduct && (countFixedPurchaseCheck == 1))
			|| ((isFixedPurchaseProduct == false) && (countFixedPurchaseCheck == 0)))
		{
			// 商品情報の商品ごと対応
			foreach (RepeaterItem item in repeaterItems)
			{
				// 定期購入回数（注文基準）
				var productFixedPurchaseItemOrderCount = (HtmlTableCell)item.FindControl("tdFixedPurchaseItemOrderCount");
				productFixedPurchaseItemOrderCount.Visible = isFixedPurchaseProduct;
				tableDataFixedPurchaseItemOrderCountTextBox.Visible = isFixedPurchaseProduct;
				tableDataFixedPurchaseItemOrderCountLabel.Text = isFixedPurchaseProduct ? "-" : string.Empty;
				// 定期購入回数（出荷基準）
				var productFixedPurchaseItemShippedCount = (HtmlTableCell)item.FindControl("tdFixedPurchaseItemShippedCount");
				productFixedPurchaseItemShippedCount.Visible = isFixedPurchaseProduct;
				tableDataFixedPurchaseItemShippedCountTextBox.Visible = isFixedPurchaseProduct;
				tableDataFixedPurchaseItemShippedCountLabel.Text = isFixedPurchaseProduct ? "-" : string.Empty;

				// 付帯情報エリア、サイズ変更
				var prodectOptionText = (HtmlTableCell)item.FindControl("tdItem1");
				var prodectOptionTextColSpan = prodectOptionText.ColSpan;
				prodectOptionText.ColSpan = prodectOptionTextColSpan + (isFixedPurchaseProduct ? 2 : -2);
			}

			// 商品情報上の全体エラーメッセージエリア、サイズ変更
			tableDataOrderItemErrorMessagesTitle.ColSpan = tableDataOrderItemErrorMessagesTitleColSpan
				+ (isFixedPurchaseProduct ? 2 : -2);
			// 商品情報上のエラーメッセージエリア、サイズ変更
			tableDataOrderItemErrorMessages.ColSpan = tableDataOrderItemErrorMessagesColSpan
				+ (isFixedPurchaseProduct ? 2 : -2);
			// 商品情報エリア、サイズ変更
			tableOrderItems.ColSpan = tableOrderItemsColSpan + (isFixedPurchaseProduct ? 2 : -2);

			// 定期購入回数エリア
			tableDataFixedPurchaseItemCount.Visible = isFixedPurchaseProduct;
			// 注文基準エリア
			tableDataFixedPurchaseItemOrderCount.Visible = isFixedPurchaseProduct;
			// 出荷基準エリア
			tableDataFixedPurchaseItemShippedCount.Visible = isFixedPurchaseProduct;
		}

		tableDataFixedPurchaseItemOrderCountTextBox.Text = isFixedPurchaseProduct ? "0" : string.Empty;
		tableDataFixedPurchaseItemShippedCountTextBox.Text = isFixedPurchaseProduct ? "0" : string.Empty;
		tableDataFixedPurchaseItemOrderCountTextBox.Visible = isFixedPurchaseProduct;
		tableDataFixedPurchaseItemShippedCountTextBox.Visible = isFixedPurchaseProduct;
		tableDataFixedPurchaseItemOrderCountLabel.Text = isFixedPurchaseProduct ? " 回" : "-";
		tableDataFixedPurchaseItemShippedCountLabel.Text = isFixedPurchaseProduct ? " 回" : "-";
	}
	#endregion

	#region プロパティ
	/// <summary>リクエスト：注文ID</summary>
	protected string RequestOrderId
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_ID]).Trim(); }
	}
	/// <summary>注文入力情報</summary>
	protected OrderInput OrderInput
	{
		get { return (OrderInput)ViewState["OrderInput"]; }
		set { ViewState["OrderInput"] = value; }
	}
	/// <summary>注文入力情報情報（変更前）</summary>
	protected OrderInput OrderInputOld
	{
		get { return (OrderInput)ViewState["OrderInputOld"]; }
		set { ViewState["OrderInputOld"] = value; }
	}
	/// <summary>元注文情報（元注文 or 最後の返品注文）</summary>
	protected OrderModel OrderOld
	{
		get { return (OrderModel)ViewState["OrderOld"]; }
		set { ViewState["OrderOld"] = value; }
	}
	/// <summary>ユーザ情報</summary>
	protected new UserModel User
	{
		get { return (UserModel)ViewState["User"]; }
		set { ViewState["User"] = value; }
	}
	/// <summary>ユーザ統合前ユーザ情報</summary>
	protected UserModel BeforeIntegrationUser
	{
		get { return (UserModel)ViewState["BeforeIntegrationUser"]; }
		set { ViewState["BeforeIntegrationUser"] = value; }
	}
	/// <summary>配送種別情報</summary>
	protected ShopShippingModel ShopShipping
	{
		get { return (ShopShippingModel)ViewState["ShopShipping"]; }
		set { ViewState["ShopShipping"] = value; }
	}
	/// <summary>配送方法情報</summary>
	protected string[] ShippingMethod
	{
		get { return (string[])ViewState["ShippingMethod"]; }
		set { ViewState["ShippingMethod"] = value; }
	}
	/// <summary>配送会社情報</summary>
	protected DeliveryCompanyModel[] DeliveryCompany
	{
		get { return (DeliveryCompanyModel[])ViewState["DeliveryCompany"]; }
		set { ViewState["DeliveryCompany"] = value; }
	}
	/// <summary>ユーザーポイント</summary>
	protected UserPointList UserPointList
	{
		get { return (UserPointList)ViewState["UserPointList"]; }
		set { ViewState["UserPointList"] = value; }
	}
	/// <summary>ユーザー仮ポイント情報</summary>
	protected UserPointList UserPointTempList
	{
		get { return (UserPointList)ViewState["UserPointTempList"]; }
		set { ViewState["UserPointTempList"] = value; }
	}
	/// <summary>ユーザー本ポイント</summary>
	protected UserPointList UserPointCompList
	{
		get { return (UserPointList)ViewState["UserPointCompList"]; }
		set { ViewState["UserPointCompList"] = value; }
	}
	/// <summary>注文に関連するポイント</summary>
	protected UserPointList UserPointRelatedThisOrder
	{
		get { return (UserPointList)ViewState["UserPointRelatedThisOrder"]; }
		set { ViewState["UserPointRelatedThisOrder"] = value; }
	}
	/// <summary>注文に関連するポイント</summary>
	protected UserPointList UserPointRelatedThisOrderOld
	{
		get { return (UserPointList)ViewState["UserPointRelatedThisOrderOld"]; }
		set { ViewState["UserPointRelatedThisOrderOld"] = value; }
	}
	/// <summary>ユーザーポイント通常本ポイント区分枝番</summary>
	protected string UserBasePointCompPointKbnNo
	{
		get { return StringUtility.ToNumeric((string)ViewState["UserBasePointCompPointKbnNo"]); }
		set { ViewState["UserBasePointCompPointKbnNo"] = value; }
	}
	/// <summary>注文エラーメッセージ</summary>
	protected UserPointList UserPointOldRelatedThisOrder
	{
		get { return (UserPointList)ViewState["UserPointRelatedThisOrderOld"]; }
		set { ViewState["UserPointRelatedThisOrderOld"] = value; }
	}
	/// <summary>注文エラーメッセージ</summary>
	protected StringBuilder OrderErrorMessages
	{
		get { return m_orderErrorMessages; }
	}
	readonly StringBuilder m_orderErrorMessages = new StringBuilder();
	/// <summary>注文者エラーメッセージ</summary>
	protected StringBuilder OrderOwnerErrorMessages
	{
		get { return m_orderOwnerErrorMessages; }
	}
	readonly StringBuilder m_orderOwnerErrorMessages = new StringBuilder();
	/// <summary>注文配送先エラーメッセージ</summary>
	protected StringBuilder OrderShippingErrorMessages
	{
		get { return m_orderShippingErrorMessages; }
	}
	readonly StringBuilder m_orderShippingErrorMessages = new StringBuilder();
	/// <summary>注文アイテムエラーメッセージ</summary>
	protected StringBuilder OrderItemErrorMessages
	{
		get { return m_orderItemErrorMessages; }
	}
	readonly StringBuilder m_orderItemErrorMessages = new StringBuilder();
	/// <summary>注文金額エラーメッセージ</summary>
	protected StringBuilder OrderPriceErrorMessages
	{
		get { return m_orderPriceErrorMessages; }
	}
	readonly StringBuilder m_orderPriceErrorMessages = new StringBuilder();
	/// <summary>注文ポイントエラーメッセージ</summary>
	protected StringBuilder OrderPointErrorMessages
	{
		get { return m_orderPointErrorMessages; }
	}
	readonly StringBuilder m_orderPointErrorMessages = new StringBuilder();
	/// <summary>注文クーポンエラーメッセージ</summary>
	protected StringBuilder OrderCouponErrorMessages
	{
		get { return m_orderCouponErrorMessages; }
	}
	readonly StringBuilder m_orderCouponErrorMessages = new StringBuilder();
	/// <summary>注文アフィリエイトエラーメッセージ</summary>
	protected StringBuilder OrderAffiliateErrorMessages
	{
		get { return m_orderAffiliateErrorMessages; }
	}
	readonly StringBuilder m_orderAffiliateErrorMessages = new StringBuilder();
	/// <summary>割引額上限エラーメッセージ</summary>
	protected StringBuilder DiscountLimitErrorMessages
	{
		get { return m_discountLimitErrorMessages; }
	}
	readonly StringBuilder m_discountLimitErrorMessages = new StringBuilder();
	/// <summary>注文拡張項目エラーメッセージ</summary>
	protected StringBuilder OrderExtendErrorMessages
	{
		get { return m_orderExtendErrorMessages; }
	}
	readonly StringBuilder m_orderExtendErrorMessages = new StringBuilder();
	/// <summary>エラーありなし判定</summary>
	protected bool HasError
	{
		get
		{
			return (this.OrderErrorMessages.Length
				+ this.OrderOwnerErrorMessages.Length
				+ this.OrderShippingErrorMessages.Length
				+ this.OrderItemErrorMessages.Length
				+ this.OrderPriceErrorMessages.Length
				+ this.OrderPointErrorMessages.Length
				+ this.OrderCouponErrorMessages.Length
				+ this.OrderPaymentErrorMessages.Length
				+ this.OrderReceiptErrorMessages.Length
				+ this.OrderExtendErrorMessages.Length != 0);
		}
	}
	/// <summary>モール注文向けエラー判定</summary>
	protected bool HasErrorForMallOrder
	{
		get
		{
			return (this.OrderPriceErrorMessages.Length
				+ this.OrderPointErrorMessages.Length
				+ this.OrderCouponErrorMessages.Length != 0);
		}
	}
	/// <summary>ユーザーか会員かどうか</summary>
	protected bool IsUser
	{
		get
		{
			return UserService.IsUser(this.User.UserKbn);
		}
	}

	/// <summary>
	/// 配送希望日が利用可能かどうか
	/// </summary>
	/// <param name="index">インデックス</param>
	/// <returns>配送希望日が利用可能か</returns>
	protected bool IsShippingDateUsable(int index)
	{
		var result = ((this.ShopShipping != null)
			&& (this.ShippingMethod != null)
			&& (this.OrderInput.IsFixedPurchaseOrder || this.ShopShipping.IsValidShippingDateSetFlg)
			&& ((Constants.GLOBAL_OPTION_ENABLE == false)
				|| (Constants.GLOBAL_OPTION_ENABLE)
			&& ((this.OrderInput.Shippings[index].ShippingCountryIsoCode == Constants.OPERATIONAL_BASE_ISO_CODE)
				&& ((this.OrderInput.Shippings[index].IsShippingAddrUs == false)
					|| ((this.OrderInput.Shippings[index].ShippingAddr5 ?? string.Empty)
							== Constants.OPERATIONAL_BASE_PROVINCE))))
			&& (this.ShippingMethod[index] == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS));

		return result;
	}

	/// <summary>
	/// 配送希望日時が利用可能かどうか
	/// </summary>
	/// <param name="index">インデックス</param>
	/// <returns>配送希望日時が利用可能か</returns>
	protected bool IsShippingTimeUsable(int index)
	{
		var result = ((this.DeliveryCompany != null)
			&& (this.ShippingMethod != null)
			&& this.DeliveryCompany[index].IsValidShippingTimeSetFlg
			&& (this.ShippingMethod[index] == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS));

		return result;
	}

	/// <summary>のしが利用可能かどうか</summary>
	protected bool IsWrappingPaperTypeUsable
	{
		get
		{
			return ((this.ShopShipping != null)
				&& this.ShopShipping.IsValidWrappingPaperFlg);
		}
	}
	/// <summary>包装が利用可能かどうか</summary>
	protected bool IsWrappingBagTypeUsable
	{
		get
		{
			return ((this.ShopShipping != null)
				&& this.ShopShipping.IsValidWrappingBagFlg);
		}
	}
	/// <summary>配送先削除可能かどうか</summary>
	protected bool CanDeleteShipping
	{
		get
		{
			return (this.OrderInput.Shippings.Length > 1);
		}
	}
	/// <summary>全ての関連する注文の最終与信フラグが正確かどうか</summary>
	protected bool IsAllLastAuthFlgValid
	{
		get { return (bool)ViewState["all_last_auth_valid_flg"]; }
		set { ViewState["all_last_auth_valid_flg"] = value; }
	}
	/// <summary>配送可能な国情報</summary>
	private CountryLocationModel[] ShippingAvailableCountryList
	{
		get { return (CountryLocationModel[])Session["shipping_available_country_list"]; }
		set { Session["shipping_available_country_list"] = value; }
	}
	/// <summary>配送可能な国表示情報</summary>
	protected ListItemCollection ShippingAvailableCountryDisplayList
	{
		get
		{
			var list = new ListItemCollection
			{
				new ListItem("")
			};
			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				list.AddRange(this.ShippingAvailableCountryList
					.Select(c => new ListItem(c.CountryName))
					.ToArray());
			}

			return list;
		}
	}
	/// <summary>ユーザー国情報</summary>
	private CountryLocationModel[] UserCountryList
	{
		get
		{
			var list = new CountryLocationService().GetCountryNames();
			return list;
		}
	}
	/// <summary>ユーザー国表示情報</summary>
	protected ListItemCollection UserCountryDisplayList
	{
		get
		{
			var list = new ListItemCollection
			{
				new ListItem("", "")
			};
			list.AddRange(this.UserCountryList
				.Select(c => new ListItem(c.CountryName, c.CountryIsoCode))
				.ToArray());
			return list;
		}
	}
	/// <summary>都道府県リスト</summary>
	protected ListItemCollection PrefectureList
	{
		get
		{
			var prefectures = new ListItemCollection
			{
				new ListItem("", "")
			};
			prefectures.AddRange(Constants.STR_PREFECTURES_LIST
				.Select(p => new ListItem(p))
				.ToArray());

			return prefectures;
		}
	}
	/// <summary>アメリカ州リスト</summary>
	protected ListItemCollection UsStateList
	{
		get
		{
			var list = new ListItemCollection
			{
				new ListItem("", "")
			};
			list.AddRange(Constants.US_STATES_LIST
				.Select(state => new ListItem(state))
				.ToArray());
			return list;
		}
	}

	/// <summary>配送希望時間帯リスト</summary>
	protected ListItemCollection ShippingTimeList(int index)
	{
		var shippingTimes = new ListItemCollection();
		if (this.DeliveryCompany == null) return shippingTimes;

		shippingTimes.Add(new ListItem(ReplaceTag("@@DispText.shipping_time_list.none@@"), ""));
		shippingTimes.AddRange(GetShippingTimeList(this.DeliveryCompany[index]).Cast<ListItem>().ToArray());

		return shippingTimes;
	}

	/// <summary>のし種類リスト</summary>
	protected ListItemCollection WrappingPaperTypeList
	{
		get
		{
			var wrappingPaperTypes = new ListItemCollection
				{
				new ListItem("", "")
			};
			if (this.ShopShipping == null) return wrappingPaperTypes;

			wrappingPaperTypes.AddRange(StringUtility.SplitCsvLine(this.ShopShipping.WrappingPaperTypes)
				.Select(w => new ListItem(w))
				.ToArray());

			return wrappingPaperTypes;
		}
	}
	/// <summary>包装種類リスト</summary>
	protected ListItemCollection WrappingBagTypeList
	{
		get
		{
			var wrappingBagTypes = new ListItemCollection
			{
				new ListItem("", "")
			};
			if (this.ShopShipping == null) return wrappingBagTypes;

			wrappingBagTypes.AddRange(
				StringUtility.SplitCsvLine(this.ShopShipping.WrappingBagTypes).Select(w => new ListItem(w)).ToArray());

			return wrappingBagTypes;
		}
	}
	/// <summary>注文セットプロモーションリスト</summary>
	protected ListItemCollection OrderSetPromotionList
	{
		get { return GetOrderSetPromotionList(this.OrderInput); }
	}
	/// <summary>アイテムテーブル用追加カラム数</summary>
	protected int AddColumnCountForItemTable
	{
		get
		{
			return (Constants.PRODUCT_SALE_OPTION_ENABLED ? 1 : 0)
				+ ((Constants.NOVELTY_OPTION_ENABLED || Constants.RECOMMEND_OPTION_ENABLED) ? 1 : 0)
				+ (Constants.FIXEDPURCHASE_OPTION_ENABLED ? 1 : 0)
				+ (this.OrderInput.HasSetProduct ? 1 : 0)
				+ (Constants.PRODUCTBUNDLE_OPTION_ENABLED ? 1 : 0)
				+ (this.IsFixedPurchaseCountAreaShow ? 2 : 0)
				+ (this.DisplayItemSubscriptionBoxCourseIdArea ? 1 : 0);
		}
	}
	/// <summary>金額変更API実行可能か</summary>
	protected bool CanExecChangePriceApi
	{
		get
		{
			return ((ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
					&& (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.YamatoKwc));
		}
	}
	/// <summary>Array of omit check for OrderOwner</summary>
	protected string[] OmitCheckForOrderOwner
	{
		get
		{
			return new[]
			{
				Constants.FIELD_ORDEROWNER_OWNER_NAME1,
				Constants.FIELD_ORDEROWNER_OWNER_NAME2,
				Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA1,
				Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA2,
				Constants.FIELD_ORDEROWNER_OWNER_ADDR1,
				Constants.FIELD_ORDEROWNER_OWNER_ADDR2,
				Constants.FIELD_ORDEROWNER_OWNER_ADDR3,
				Constants.FIELD_ORDEROWNER_OWNER_ADDR4,
				Constants.FIELD_ORDEROWNER_OWNER_TEL1 + "_1",
				Constants.FIELD_ORDEROWNER_OWNER_TEL1 + "_2",
				Constants.FIELD_ORDEROWNER_OWNER_TEL1 + "_3",
				Constants.FIELD_ORDEROWNER_OWNER_ADDR_COUNTRY_ISO_CODE,
			};
		}
	}
	/// <summary>Array of omit check for OrderShipping</summary>
	protected string[] OmitCheckForOrderShipping
	{
		get
		{
			return new[]
			{
				Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1,
				Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME2,
				Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA1,
				Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA2,
				Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1,
				Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR2,
				Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR3,
				Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4,
				Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1 + "_1",
				Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1 + "_2",
				Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1 + "_3",
				Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE,
			};
		}
	}
	/// <summary>決済種別メッセージ</summary>
	protected StringBuilder OrderPaymentErrorMessages
	{
		get { return m_orderPaymentErrorMessages; }
	}
	readonly StringBuilder m_orderPaymentErrorMessages = new StringBuilder();
	/// <summary>
	/// ユーザーが所属しているカード数
	/// </summary>
	protected int CreditCardNum
	{
		get { return (int)ViewState["credit_card_num"]; }
		set { ViewState["credit_card_num"] = value; }
	}
	/// <summary>配送種別変更可能か</summary>
	protected bool CanSelectShipping
	{
		get
		{
			return ((this.OrderInput.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_TEMP)
				&& Constants.FLG_USER_MALL_ID_EXTERNAL_ORDER_SITES
					.Any(site => site == this.OrderInput.MallId));
		}
	}
	#endregion

	/// <summary>
	/// クレジットカード一覧選択処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlUserCreditCard_SelectedIndexChanged(object sender, EventArgs e)
	{
		DisplayCreditInputForm();

		if ((Constants.PAYMENT_REAUTH_ENABLED == false)
			|| (this.OrderInput.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)) return;

			// 他のカードで支払い場合
			if (ddlUserCreditCard.SelectedValue != OrderInput.CreditBranchNo)
			{
				ddlOldExecuteType.Visible = true;
				ddlOldExecuteType.Enabled = ddlNewExecuteType.Enabled = Constants.REAUTH_COMPLETE_CREDITCARD_LIST.Contains(Constants.PAYMENT_CARD_KBN);
				ddlOldExecuteType.SelectedValue = ddlNewExecuteType.SelectedValue = Constants.REAUTH_COMPLETE_CREDITCARD_LIST.Contains(Constants.PAYMENT_CARD_KBN)
					? ReauthCreatorFacade.ExecuteTypes.System.ToString() : ReauthCreatorFacade.ExecuteTypes.None.ToString();
				//「【変更前】クレジットカード」
				lbOldExternalPayment.Text = ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ORDER,
					Constants.VALUETEXT_PARAM_ORDER_MODIFY_INPUT,
					Constants.VALUETEXT_PARAM_ORDER_MODIFY_CHANGE_CREDIT_CARD_BEFORE);
				//「【変更後】クレジットカード(他のカード)」
				lbNewExternalPayment.Text = ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ORDER,
					Constants.VALUETEXT_PARAM_ORDER_MODIFY_INPUT,
					Constants.VALUETEXT_PARAM_ORDER_MODIFY_CHANGE_CREDIT_CARD_AFTER_OTHER);
			}
			else
			{
				// 同じカードで支払い場合
				ddlOldExecuteType.Visible = false;
				ddlOldExecuteType.Enabled = false;
				ddlOldExecuteType.SelectedValue = ReauthCreatorFacade.ExecuteTypes.None.ToString();
				lbOldExternalPayment.Text = string.Empty;
				ddlNewExecuteType.Enabled = true;
				ddlNewExecuteType.SelectedValue = ReauthCreatorFacade.ExecuteTypes.System.ToString();
				//「決済連動」
				lbNewExternalPayment.Text = ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ORDER,
					Constants.VALUETEXT_PARAM_ORDER_MODIFY_INPUT,
					Constants.VALUETEXT_PARAM_ORDER_MODIFY_PAYMENT_INTERLOOKING);
			}
			dllCreditInstallments_SelectedIndexChanged(sender, e);

		// 最終与信フラグが不正場合、再与信しない
		if (this.IsAllLastAuthFlgValid)
		{
			ddlOldExecuteType.Enabled = false;
			ddlOldExecuteType.SelectedValue = ReauthCreatorFacade.ExecuteTypes.None.ToString();
			ddlNewExecuteType.Enabled = false;
			ddlNewExecuteType.SelectedValue = ReauthCreatorFacade.ExecuteTypes.None.ToString();
		}

		RefreshExternalPaymentReauthSelect();
	}

	/// <summary>
	/// クレジットカード利用回数更新
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void dllCreditInstallments_SelectedIndexChanged(object sender, EventArgs e)
	{
		if ((Constants.PAYMENT_REAUTH_ENABLED == false)
			|| (this.OrderInput.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
			|| (ddlUserCreditCard.SelectedValue != this.OrderInputOld.CreditBranchNo)) return;

		// 他のカードで支払い場合
		if (dllCreditInstallments.SelectedValue != this.OrderInputOld.CardInstallmentsCode)
		{
			ddlOldExecuteType.Visible = true;
			ddlOldExecuteType.Enabled = ddlNewExecuteType.Enabled = Constants.REAUTH_COMPLETE_CREDITCARD_LIST.Contains(Constants.PAYMENT_CARD_KBN);
			ddlOldExecuteType.SelectedValue = ddlNewExecuteType.SelectedValue = Constants.REAUTH_COMPLETE_CREDITCARD_LIST.Contains(Constants.PAYMENT_CARD_KBN)
				? ReauthCreatorFacade.ExecuteTypes.System.ToString() : ReauthCreatorFacade.ExecuteTypes.None.ToString();
			//「【変更前】クレジットカード」
			lbOldExternalPayment.Text = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ORDER,
				Constants.VALUETEXT_PARAM_ORDER_MODIFY_INPUT,
				Constants.VALUETEXT_PARAM_ORDER_MODIFY_CHANGE_CREDIT_CARD_BEFORE);
			//「【変更後】クレジットカード(支払回数変更)」
			lbNewExternalPayment.Text = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ORDER,
				Constants.VALUETEXT_PARAM_ORDER_MODIFY_INPUT,
				Constants.VALUETEXT_PARAM_ORDER_MODIFY_CHANGE_NUMBER_CREDIT_CARD_AFTER);
		}
		else
		{
			// 同じカードで支払い場合
			ddlOldExecuteType.Visible = false;
			ddlOldExecuteType.Enabled = false;
			ddlOldExecuteType.SelectedValue = ReauthCreatorFacade.ExecuteTypes.None.ToString();
			lbOldExternalPayment.Text = string.Empty;
			ddlNewExecuteType.Enabled = true;
			ddlNewExecuteType.SelectedValue = ReauthCreatorFacade.ExecuteTypes.System.ToString();
			//「決済連動」
			lbNewExternalPayment.Text = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ORDER,
				Constants.VALUETEXT_PARAM_ORDER_MODIFY_INPUT,
				Constants.VALUETEXT_PARAM_ORDER_MODIFY_PAYMENT_INTERLOOKING);
		}
		RefreshExternalPaymentReauthSelect();
	}

	/// <summary>
	/// Shipping Receiving Store Type Selected Index Changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlShippingReceivingStoreType_SelectedIndexChanged(object sender, EventArgs e)
	{
		var repeater = (RepeaterItem)((DropDownList)sender).Parent;
		var ddlShippingMethod = ((DropDownList)repeater.FindControl("ddlShippingMethod"));
		var ddlDeliveryCompany = ((DropDownList)repeater.FindControl("ddlDeliveryCompany"));
		var ddlUserShipping = ((DropDownList)repeater.FindControl("ddlUserShipping"));
		var ddlShippingReceivingStoreType = ((DropDownList)repeater.FindControl("ddlShippingReceivingStoreType"));
		var lCvsShopId = ((Literal)repeater.FindControl("lCvsShopId"));
		var lCvsShopName = ((Literal)repeater.FindControl("lCvsShopName"));
		var lCvsShopAddress = ((Literal)repeater.FindControl("lCvsShopAddress"));
		var lCvsShopTel = ((Literal)repeater.FindControl("lCvsShopTel"));

		ddlDeliveryCompany.Items.Clear();
		ddlDeliveryCompany.Items.AddRange(
			OrderCommon.GetShippingServiceRelatedProduct(
			this.ShopShipping.ShopId,
			this.ShopShipping.ShippingId,
			ddlShippingMethod.SelectedValue,
			(ddlUserShipping.SelectedValue == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE)));
		lCvsShopId.Text = string.Empty;
		hfCvsShopId.Value = string.Empty;
		lCvsShopName.Text = string.Empty;
		hfCvsShopName.Value = string.Empty;
		lCvsShopAddress.Text = string.Empty;
		hfCvsShopAddress.Value = string.Empty;
		lCvsShopTel.Text = string.Empty;
		hfCvsShopTel.Value = string.Empty;
		hfSelectedShopId.Value = string.Empty;

		// 決済種別
		var paymentSelectionFlg = string.Empty;
		var permittedPaymentIds = string.Empty;
		if (this.ShopShipping != null)
		{
			paymentSelectionFlg = this.ShopShipping.PaymentSelectionFlg;
			permittedPaymentIds = this.ShopShipping.PermittedPaymentIds;
		}
		var payments = GetPaymentValidListPermission(this.LoginOperatorShopId, paymentSelectionFlg, permittedPaymentIds).ToArray();
		if (OrderCommon.CheckPaymentYamatoKaSms(this.OrderOld.OrderPaymentKbn) == false)
		{
			payments = payments.Where(item => (OrderCommon.CheckPaymentYamatoKaSms(item.PaymentId) == false)).ToArray();
		}

		// 決済種別が存在しない場合はエラー
		if (payments.Length == 0)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PAYMENT_NO_DATA);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// Remove invalid payment for store pickup
		if (this.IsStorePickup)
		{
			payments = GetValidStorePickupPayments();
		}

		var orderPaymentOld = ddlOrderPaymentKbn.SelectedValue;
		ddlOrderPaymentKbn.Items.Clear();
		ddlOrderPaymentKbn.Items.Add(new ListItem(string.Empty, string.Empty));
		var orderPaymentKbnEnabled = true;
		foreach (var payment in payments)
		{
			if (this.IsPaymentAtConvenienceStore(ddlShippingReceivingStoreType.SelectedValue)
				&& (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE))
			{
				ddlOrderPaymentKbn.Items.Clear();
				ddlOrderPaymentKbn.Items.Add(
					new ListItem(payment.PaymentName, payment.PaymentId));
				orderPaymentKbnEnabled = false;
				break;
			}

			if (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE) continue;

			ddlOrderPaymentKbn.Items.Add(
				new ListItem(payment.PaymentName, payment.PaymentId));
		}

		if (ddlOrderPaymentKbn.Items.FindByValue(orderPaymentOld) != null)
		{
			ddlOrderPaymentKbn.SelectedValue = orderPaymentOld;
		}

		ddlOrderPaymentKbn.Enabled = orderPaymentKbnEnabled;
	}

	/// <summary>
	/// 外部決済連携のドロップダウンリストをリフレッシュ
	/// </summary>
	protected void RefreshExternalPaymentReauthSelect()
	{
		// 交換注文の場合
		if (this.OrderInput.IsExchangeOrder)
		{
			// Amazon Payの交換注文は決済連動しない。
			if ((this.OrderInputOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT)
				&& this.OrderInputOld.IsExchangeOrder)
			{
				ddlOldExecuteType.SelectedValue = ReauthCreatorFacade.ExecuteTypes.None.ToString();
				lbExternalPaymentAlertMessage.Text = WebMessages
					.GetMessages(WebMessages.ERRMSG_MANAGER_AMAZON_PAY_CANNOT_LINK_PAYMENT);

				if (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT)
				{
					ddlNewExecuteType.Enabled = false;
					ddlNewExecuteType.SelectedValue = ReauthCreatorFacade.ExecuteTypes.None.ToString();
				}
			}
		}

		// 最終与信フラグがNULL場合、再与信しない
		if (string.IsNullOrEmpty(this.OrderInput.LastAuthFlg))
		{
			ddlOldExecuteType.Enabled = false;
			ddlOldExecuteType.SelectedValue = ReauthCreatorFacade.ExecuteTypes.None.ToString();
			ddlNewExecuteType.Enabled = false;
			ddlNewExecuteType.SelectedValue = ReauthCreatorFacade.ExecuteTypes.None.ToString();
		}
	}

	/// <summary>
	/// クレジットカード登録チェックボックスチェック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbRegistCreditCard_CheckedChanged(object sender, EventArgs e)
	{
		DisplayCreditInputForm();
	}

	/// <summary>
	/// クレジット入力フォーム表示切り替え
	/// </summary>
	private void DisplayCreditInputForm()
	{
		// 支払回数表示
		trInstallments.Visible = OrderCommon.CreditInstallmentsSelectable
			&& (this.CanUseCreditCardNoForm
				|| (ddlUserCreditCard.SelectedValue != CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW));

		switch (ddlUserCreditCard.SelectedValue)
		{
			case CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW:
				divUserCreditCard.Visible = false;
				divCreditCardInputNew.Visible = true;
				trRegistCreditCard.Visible = trCreditCardName.Visible = (this.IsUser && (Constants.MAX_NUM_REGIST_CREDITCARD > this.CreditCardNum));
				if (this.IsUserPayTg)
				{
					btnGetCreditCardInfo.Enabled = (this.IsSuccessfulCardRegistration == false);
					ddlCreditExpireMonth.Enabled = (this.IsSuccessfulCardRegistration == false);
					ddlCreditExpireYear.Enabled = (this.IsSuccessfulCardRegistration == false);
					tbCreditCardNo1.Enabled = (this.IsSuccessfulCardRegistration == false);
					trCreditExpire.Visible = this.IsSuccessfulCardRegistration;
					tdCreditNumber.Visible = this.IsSuccessfulCardRegistration;
					tdGetCardInfo.Visible = (this.IsSuccessfulCardRegistration == false);
				}
				break;

			default:
				int branchNo;
				if (int.TryParse(ddlUserCreditCard.SelectedValue, out branchNo))
				{
					var userCreditCard = new UserCreditCardService().Get(this.OrderInputOld.UserId, branchNo);
					lCreditCompany.Text = WebSanitizer.HtmlEncode(
							ValueText.GetValueText(
								Constants.TABLE_ORDER,
								OrderCommon.CreditCompanyValueTextFieldName,
								userCreditCard.CompanyCode));
					lCreditLastFourDigit.Text = WebSanitizer.HtmlEncode(userCreditCard.LastFourDigit);
					lCreditExpirationMonth.Text = WebSanitizer.HtmlEncode(userCreditCard.ExpirationMonth);
					lCreditExpirationYear.Text = WebSanitizer.HtmlEncode(userCreditCard.ExpirationYear);
					lCreditAuthorName.Text = WebSanitizer.HtmlEncode(userCreditCard.AuthorName);
					divUserCreditCard.Visible = true;
					divCreditCardInputNew.Visible = false;
					trRegistCreditCard.Visible = trCreditCardName.Visible = false;
				}

				tbUserCreditCardName.Text = string.Empty;
				break;
		}

		trCreditCardName.Visible = cbRegistCreditCard.Checked;
	}

	#region トークン決済系
	/// <summary>
	/// （トークン決済向け）カード情報編集（再入力）リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbEditCreditCardNoForToken_Click(object sender, EventArgs e)
	{
		// トークンなどクレジットカード情報削除
		ResetCreditTokenInfoFromForm(null, "DUMMY");

		SwitchDisplayForCreditTokenInput();
	}

	/// <summary>
	/// クレジットトークン向け  カード情報取得JSスクリプト作成（カート内ではない）
	/// </summary>
	/// <returns>
	/// カード情報取得スクリプト
	/// 文字列を返すのでevalで動的実行すれば連想配列でカード情報がとれます
	/// </returns>
	protected string CreateGetCardInfoJsScriptForCreditToken()
	{
		if (ddlOrderPaymentKbn.SelectedValue != Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT) return "";

		var script = CreateGetCardInfoJsScriptForCreditTokenInner();
		return script;
	}

	/// <summary>
	/// クレジットトークン取得＆フォームセットJSスクリプト作成
	/// </summary>
	/// <returns>JSスクリプト</returns>
	protected string CreateGetCreditTokenAndSetToFormJsScript()
	{
		var script = CreateGetCreditTokenAndSetToFormJsScriptInner();
		return script;
	}

	/// <summary>
	/// クレジットトークン向け フォームマスキングJSスクリプト作成
	/// </summary>
	/// <returns>JSスクリプト</returns>
	protected string CreateMaskFormsForCreditTokenJsScript()
	{
		var maskingScripts = CreateMaskFormsForCreditTokenJsScriptInner();
		return maskingScripts;
	}
	#endregion

	/// <summary>
	/// 表示通貨ロケールIDドロップダウンリスト変更時イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlDispCurrencyLocaleId_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(ddlDispCurrencyLocaleId.SelectedValue))
		{
			lCurrencyCode.Text = string.Empty;
			return;
		}

		lCurrencyCode.Text = GlobalConfigUtil.GetCurrencyByLocaleId(ddlDispCurrencyLocaleId.SelectedValue).Code;
	}

	/// <summary>
	/// 表示言語ロケールIDドロップダウンリスト変更時イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlDispLanguageLocaleId_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(ddlDispLanguageLocaleId.SelectedValue))
		{
			lLanguageCode.Text = string.Empty;
			return;
		}

		lLanguageCode.Text = GlobalConfigUtil.GetLanguageByLocaleId(ddlDispLanguageLocaleId.SelectedValue).Code;
	}

	/// <summary>
	/// コンポーネントに値をセット 配送先グローバル関連項目設定
	/// </summary>
	private void SetOrderShippingGlobalColumn()
	{
		// グローバル対応なしの場合、何もしない
		if (Constants.GLOBAL_OPTION_ENABLE == false) return;

		// 配送先グローバル関連項目設定
		rShippingList.Items
			.Cast<RepeaterItem>()
			.ToList()
			.ForEach(ri => SetOrderShippingGlobalColumn(ri, this.OrderInput.Shippings[ri.ItemIndex]));
	}

	/// <summary>
	/// 配送先グローバル関連項目設定
	/// </summary>
	/// <param name="ri">リピーターアイテム</param>
	/// <param name="orderShipping">配送先情報</param>
	private void SetOrderShippingGlobalColumn(RepeaterItem ri, OrderShippingInput orderShipping)
	{
		// 配送先「州」ドロップダウン設定
		if (orderShipping.IsShippingAddrUs)
		{
			var wddlShippingAddr5 = GetWrappedControl<WrappedDropDownList>(ri, "ddlShippingAddr5");
			wddlShippingAddr5.SelectItemByText(orderShipping.ShippingAddr5);
		}

		// 送り主「州」ドロップダウン設定
		if (Constants.GIFTORDER_OPTION_ENABLED && this.OrderInput.IsGiftOrder && (orderShipping.IsSenderAddrUs))
		{
			var wddlSenderAddr5 = GetWrappedControl<WrappedDropDownList>(ri, "ddlSenderAddr5");
			wddlSenderAddr5.SelectItemByText(orderShipping.SenderAddr5);
		}

		// 配送先「国名」ドロップダウン設定
		var wddlShippingCountry = GetWrappedControl<WrappedDropDownList>(ri, "ddlShippingCountry");
		wddlShippingCountry.SelectItemByText(orderShipping.ShippingCountryName);

		// 送り主「国名」ドロップダウン設定
		var wddlSenderCountry = GetWrappedControl<WrappedDropDownList>(ri, "ddlSenderCountry");
		wddlSenderCountry.SelectItemByText(orderShipping.SenderCountryName);
	}

	/// <summary>
	/// 配送先／送り主国ドロップダウンリスト変更時イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlShippingCountrySenderCountry_SelectedIndexChanged(object sender, EventArgs e)
	{
		// 配送先追加
		var order = GetOrderInput();
		this.OrderInput.UpdateOrderShippings(order);

		// データバインド
		rShippingList.DataBind();

		// 配送先グローバル関連項目設定
		SetOrderShippingGlobalColumn();
	}

	/// <summary>
	/// 領収書希望がクリックされた場合
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblReceiptFlg_SelectedIndexChanged(object sender, EventArgs e)
	{
		tbReceiptAddress.Enabled
			= tbReceiptProviso.Enabled
				= rblReceiptOutputFlg.Enabled
					= (rblReceiptFlg.SelectedValue == Constants.FLG_ORDER_RECEIPT_FLG_ON);
		if (rblReceiptFlg.SelectedValue == Constants.FLG_ORDER_RECEIPT_FLG_OFF)
			tbReceiptAddress.Text =
				tbReceiptProviso.Text = string.Empty;
	}

	/// <summary>
	/// 注文者の住所国と配送先国から、台湾後払いが利用可能かを判定しエラーメッセージを返す
	/// </summary>
	/// <param name="paymentId">判定する決済種別ID</param>
	/// <param name="shippingCountryIsoCode">配送先国コード</param>
	/// <param name="ownerCountryIsoCode">注文者の住所国コード</param>
	/// <returns>可能:true 不可能:false</returns>
	private string CheckUsedPaymentOrderModifyByShippingCountry(
		string paymentId,
		string shippingCountryIsoCode,
		string ownerCountryIsoCode)
	{
		var errorMessage =
			(TriLinkAfterPayHelper.CheckUsedPaymentForTriLinkAfterPay(
				paymentId,
				shippingCountryIsoCode,
				ownerCountryIsoCode))
				? WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PAYMENT_USBABLE_COUNTRY_ERROR)
					.Replace("@@ 1 @@", "Taiwan")
				: string.Empty;

		// Check Shipping Country Iso Code For Paidy Payment
		if (string.IsNullOrEmpty(errorMessage)
			&& (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
			&& (IsCountryJp(shippingCountryIsoCode) == false))
		{
			errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PAIDY_COUNTRY_SHIPPING_NOT_JAPAN_ERROR);
		}
		// Check Country Iso Code Can Order NP After Pay
		else if (string.IsNullOrEmpty(errorMessage)
			&& (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
			&& ((IsCountryJp(shippingCountryIsoCode) == false)
				|| (IsCountryJp(ownerCountryIsoCode) == false)))
		{
			errorMessage = NPAfterPayUtility.GetErrorMessages(Constants.FLG_PAYMENT_NP_AFTERPAY_CUSTOM_ERROR_CODE_3);
		}
		// Check Country Iso Code Can Order Ec Pay
		else if (string.IsNullOrEmpty(errorMessage)
			&& (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
			&& ((IsCountryTw(shippingCountryIsoCode) == false)
				|| (IsCountryTw(ownerCountryIsoCode) == false)))
		{
			errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_CHECK_COUNTRY_FOR_PAYMENT_ECPAY);
		}
		// Check Country Iso Code Can Order Neweb Pay
		else if (string.IsNullOrEmpty(errorMessage)
			&& (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
			&& ((IsCountryTw(shippingCountryIsoCode) == false)
				|| (IsCountryTw(ownerCountryIsoCode) == false)))
		{
			errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_CHECK_COUNTRY_FOR_PAYMENT_NEWEBPAY);
		}

		return errorMessage;
	}

	/// <summary>
	/// 決済系 エラー文言表示
	/// </summary>
	/// <param name="errorMessage">エラー文言</param>
	private void DisplayOrderPaymentErrorMessage(string errorMessage)
	{
		this.OrderPaymentErrorMessages.Append(errorMessage);
		lbPaymentErrorMessages.Text = errorMessage;
		tbdyPaymentErrorMessages.Visible = (errorMessage.Length != 0);
		if (tbdyPaymentErrorMessages.Visible) ddlOrderPaymentKbn.Focus();
	}

	/// <summary>
	/// Get scheduled shipping date value
	/// </summary>
	/// <param name="scheduledShippingDate">Scheduled shipping date</param>
	/// <returns>Scheduled shipping date</returns>
	public DateTime? GetScheduledShippingDateValue(string scheduledShippingDate)
	{
		var result = Validator.IsDate(scheduledShippingDate) ? DateTime.Parse(scheduledShippingDate) : (DateTime?)null;

		return result;
	}

	/// <summary>
	/// 領収書情報入力チェック
	/// </summary>
	/// <param name="order">注文情報</param>
	private void CheckReceiptInput(OrderInput order)
	{
		// 領収書対応なし、また領収書希望なしの場合は、チェックを行わない
		if ((Constants.RECEIPT_OPTION_ENABLED == false)
			|| (order.ReceiptFlg == Constants.FLG_ORDER_RECEIPT_FLG_OFF)) return;

		// 入力チェック
		var input = new Hashtable
		{
			{ Constants.FIELD_ORDER_RECEIPT_ADDRESS, order.ReceiptAddress },
			{ Constants.FIELD_ORDER_RECEIPT_PROVISO, order.ReceiptProviso }
		};
		this.OrderReceiptErrorMessages.Append(Validator.Validate("OrderReceipt", input));
	}

	/// <summary>
	/// 配送料変更メッセージ表示
	/// </summary>
	/// <param name="item">リピーターアイテム</param>
	protected void DisplayChangeShippingPriceMessage(RepeaterItem item)
	{
		var divChangeShippingPriceMessage =
			(HtmlGenericControl)item.FindControl("divChangeShippingPriceMessage");
		var lChangeShippingPriceMessage = (Literal)item.FindControl("lChangeShippingPriceMessage");
		divChangeShippingPriceMessage.Visible = false;

		// 注文入力情報取得
		var order = GetOrderInput();
		// カート作成
		var cart = CreateCart(order);
		// 配送料計算時は会員ランク割引、定期会員割引額、定期購入割引額が必要
		cart.MemberRankDiscount = decimal.Parse(order.MemberRankDiscountPrice);
		cart.FixedPurchaseMemberDiscountAmount = decimal.Parse(order.FixedPurchaseMemberDiscountAmount);
		cart.FixedPurchaseDiscount = decimal.Parse(order.FixedPurchaseDiscountPrice);

		// カート再計算
		CalculateCart(order, cart);

		var beforePrice = decimal.Parse(order.OrderPriceShipping);
		var afterPrice = cart.PriceShipping;

		// 送料が変更しない場合、何もしない
		if (beforePrice == afterPrice) return;

		// 送料変更のメッセージ表示
		divChangeShippingPriceMessage.Visible = true;
		lChangeShippingPriceMessage.Text =
			WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_CHANGE_SHIPPING_PRICE_ALERT)
				.Replace("@@ 1 @@", beforePrice.ToPriceString(true))
				.Replace("@@ 2 @@", afterPrice.ToPriceString(true));
	}

	/// <summary>
	/// Control Payment When Shipping Convenience
	/// </summary>
	private void ControlPaymentWhenShippingConvenience()
	{
		if (this.IsShippingConvenience && Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED)
		{
			if ((this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE)
				&& this.OrderInput.Shippings
					.Any(shipping => (shipping.ShippingReceivingStoreType == Constants.FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_FAMILY_MART_PAYMENT)
						|| (shipping.ShippingReceivingStoreType == Constants.FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_7_ELEVENT_PAYMENT)
						|| (shipping.ShippingReceivingStoreType == Constants.FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_HI_LIFE_PAYMENT)))
			{
				ddlOrderPaymentKbn.Enabled = false;
				return;
			}

			var item = ddlOrderPaymentKbn.Items.FindByValue(Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE);
			if (item != null) ddlOrderPaymentKbn.Items.Remove(item);
			return;
		}

		if (this.IsShippingConvenience
			&& (this.OrderInputOld.Shippings.Any(shipping => string.IsNullOrEmpty(shipping.ShippingCheckNo) == false)))
		{
			if (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE)
			{
				ddlOrderPaymentKbn.Enabled = false;
			}
			else
			{
				var item = ddlOrderPaymentKbn.Items.FindByValue(Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE);
				if (item != null)
				{
					ddlOrderPaymentKbn.Items.Remove(item);
				}
			}
		}
	}

	/// <summary>
	/// Open Convenience Store Map Ec Pay Click Event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnOpenConvenienceStoreMapEcPay_Click(object sender, EventArgs e)
	{
		foreach (RepeaterItem ri in rShippingList.Items)
		{
			var ddlShippingReceivingStoreType = (DropDownList)ri.FindControl("ddlShippingReceivingStoreType");

			if (string.IsNullOrEmpty(ddlShippingReceivingStoreType.SelectedValue)) continue;

			var shippingReceivingStoreType = ddlShippingReceivingStoreType.SelectedValue;
			if (string.IsNullOrEmpty(shippingReceivingStoreType)) return;

			var script = ECPayUtility.CreateScriptForOpenConvenienceStoreMap(shippingReceivingStoreType);
			ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "OpenConvenienceStoreMap", script, true);
		}
	}

	/// <summary>
	/// Get item shipping ECPay select
	/// </summary>
	/// <returns>A list item collection</returns>
	protected ListItemCollection GetItemShippingECPaySelect()
	{
		var itemCollection = ValueText.GetValueItemList(
			Constants.TABLE_ORDERSHIPPING,
			Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_TYPE);
		return itemCollection;
	}

	/// <summary>
	/// Get Item Shipping Select
	/// </summary>
	/// <param name="addStorePickup">Add store pickup</param>
	/// <returns>Item Collection</returns>
	protected List<ListItem> GetItemShippingSelect(bool addStorePickup = true)
	{
		var shopShipping = new ShopShippingService().Get(this.LoginOperatorShopId, this.OrderInput.ShippingId);
		var items = new List<ListItem>();
		if ((this.OrderInput.Shippings[0].ShippingReceivingStoreFlg
				== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
			&& (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE)
			&& Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED
			&& CheckItemRelateWithServiceConvenienceStore(shopShipping))
		{
			this.UserShippingAddress = this.UserShippingAddress.Where(
				item => (item.ShippingReceivingStoreFlg
					== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)).ToArray();
			items.AddRange(
				this.UserShippingAddress.Select(us => new ListItem(us.Name, us.ShippingNo.ToString())).ToArray());
			items.Add(
				new ListItem(
					ReplaceTag("@@DispText.shipping_addr_kbn_list.convenience_store@@"),
					CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE));
		}
		else
		{
			items.Add(
				new ListItem(
					ReplaceTag("@@DispText.shipping_addr_kbn_list.new@@"),
					CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW));
			items.Add(
				new ListItem(
					ReplaceTag("@@DispText.shipping_addr_kbn_list.owner@@"),
					CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER));

			if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED
				&& CheckItemRelateWithServiceConvenienceStore(shopShipping))
			{
				items.AddRange(
					this.UserShippingAddress.Select(us => new ListItem(us.Name, us.ShippingNo.ToString())).ToArray());
				items.Add(
					new ListItem(
						ReplaceTag("@@DispText.shipping_addr_kbn_list.convenience_store@@"),
						CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE));
			}
			else
			{
				var userShippingNormal = this.UserShippingAddress
					.Where(item => (item.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF));
				items.AddRange(
					userShippingNormal.Select(us => new ListItem(us.Name, us.ShippingNo.ToString())).ToArray());
			}
		}

		// Add store pickup option if both real shop and store pickup option are enabled and product allowed for store pickup
		if (Constants.STORE_PICKUP_OPTION_ENABLED
			&& addStorePickup
			&& this.CanUseStorePickup)
		{
			items.Add(new ListItem(ReplaceTag("@@DispText.shipping_addr_kbn_list.storepickup@@"),
				CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_STORE_PICKUP));
		}

		return items;
	}

	/// <summary>
	/// Check Item Relate With Service Convenience Store
	/// </summary>
	/// <param name="shopShipping">Shop Shipping</param>
	/// <returns>True: shipping Kbn is convenience store and relate product </returns>
	protected bool CheckItemRelateWithServiceConvenienceStore(ShopShippingModel shopShipping)
	{
		if ((Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED == false) || (shopShipping == null)) return false;

		var deliveryCompanyList =
			(this.ShippingMethod[0] == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS)
				? shopShipping.CompanyListExpress
				: shopShipping.CompanyListMail;

		var serviceShipping = deliveryCompanyList
				.Select(company => new DeliveryCompanyService().Get(company.DeliveryCompanyId))
				.Any(company => company.DeliveryCompanyId == Constants.TWPELICANEXPRESS_CONVENIENCE_STORE_ID);

		return serviceShipping;
	}

	/// <summary>
	/// Check Store Id Valid
	/// </summary>
	/// <param name="storeId">Convenience Store Id</param>
	/// <returns>Store is valid or not</returns>
	[WebMethod]
	public static bool CheckStoreIdValid(string storeId)
	{
		if (string.IsNullOrEmpty(storeId)) return false;

		if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED) return true;

		return OrderCommon.CheckIdExistsInXmlStoreBatchFixedPurchase(storeId);
	}

	/// <summary>User Shipping Address</summary>
	protected UserShippingModel[] UserShippingAddress
	{
		get
		{
			if (ViewState["UserShippingAddress"] != null) return (UserShippingModel[])ViewState["UserShippingAddress"];

			ViewState["UserShippingAddress"] = new UserShippingService().GetAllOrderByShippingNoDesc(this.OrderInput.UserId)
				.OrderBy(item => item.ShippingNo)
				.ToArray();
			return (UserShippingModel[])ViewState["UserShippingAddress"];

		}
		set { ViewState["UserShippingAddress"] = value; }
	}

	/// <summary>
	/// 注文者情報の全角半角補正
	/// </summary>
	/// <param name="order">受注情報</param>
	private void CorrectOrderOwnerWithFullHalfWidth(OrderInput order)
	{
		var owner = order.Owner;
		owner.OwnerName1 = DataInputUtility.ConvertToFullWidthBySetting(owner.OwnerName1, owner.IsAddrJp);
		owner.OwnerName2 = DataInputUtility.ConvertToFullWidthBySetting(owner.OwnerName2, owner.IsAddrJp);
		owner.OwnerName = DataInputUtility.ConvertToFullWidthBySetting(owner.OwnerName, owner.IsAddrJp);
		owner.OwnerNameKana1 = StringUtility.ToZenkaku(owner.OwnerNameKana1);
		owner.OwnerNameKana2 = StringUtility.ToZenkaku(owner.OwnerNameKana2);
		owner.OwnerNameKana = StringUtility.ToZenkaku(owner.OwnerNameKana);
		owner.OwnerTel1_1 = StringUtility.ToHankaku(owner.OwnerTel1_1);
		owner.OwnerTel1_2 = StringUtility.ToHankaku(owner.OwnerTel1_2);
		owner.OwnerTel1_3 = StringUtility.ToHankaku(owner.OwnerTel1_3);
		owner.OwnerTel1 = StringUtility.ToHankaku(owner.OwnerTel1);
		owner.OwnerMailAddr = StringUtility.ToHankaku(owner.OwnerMailAddr);
		owner.OwnerMailAddr2 = StringUtility.ToHankaku(owner.OwnerMailAddr2);
		owner.OwnerZip1 = StringUtility.ToHankaku(owner.OwnerZip1);
		owner.OwnerZip2 = StringUtility.ToHankaku(owner.OwnerZip2);
		owner.OwnerZip = StringUtility.ToHankaku(owner.OwnerZip);
		owner.OwnerAddr2 = DataInputUtility.ConvertToFullWidthBySetting(owner.OwnerAddr2, owner.IsAddrJp);
		owner.OwnerAddr3 = DataInputUtility.ConvertToFullWidthBySetting(owner.OwnerAddr3, owner.IsAddrJp);
		owner.OwnerAddr4 = DataInputUtility.ConvertToFullWidthBySetting(owner.OwnerAddr4, owner.IsAddrJp);
		owner.OwnerAddr5 = DataInputUtility.ConvertToFullWidthBySetting(owner.OwnerAddr5, owner.IsAddrJp);
	}

	/// <summary>
	/// 配送先の全角半角補正
	/// </summary>
	/// <param name="order">受注情報</param>
	private void CorrectOrderShippingWithFullHalfWidth(OrderInput order)
	{
		foreach (var shipping in order.Shippings)
		{
			// 配送先情報
			shipping.ShippingName1 = DataInputUtility.ConvertToFullWidthBySetting(
				shipping.ShippingName1,
				shipping.IsShippingAddrJp);
			shipping.ShippingName2 = DataInputUtility.ConvertToFullWidthBySetting(
				shipping.ShippingName2,
				shipping.IsShippingAddrJp);
			shipping.ShippingName = DataInputUtility.ConvertToFullWidthBySetting(
				shipping.ShippingName,
				shipping.IsShippingAddrJp);
			shipping.ShippingNameKana1 = StringUtility.ToZenkaku(shipping.ShippingNameKana1);
			shipping.ShippingNameKana2 = StringUtility.ToZenkaku(shipping.ShippingNameKana2);
			shipping.ShippingNameKana = StringUtility.ToZenkaku(shipping.ShippingNameKana);
			shipping.ShippingTel1_1 = StringUtility.ToHankaku(shipping.ShippingTel1_1);
			shipping.ShippingTel1_2 = StringUtility.ToHankaku(shipping.ShippingTel1_2);
			shipping.ShippingTel1_3 = StringUtility.ToHankaku(shipping.ShippingTel1_3);
			shipping.ShippingTel1 = StringUtility.ToHankaku(shipping.ShippingTel1);
			shipping.ShippingZip1 = StringUtility.ToHankaku(shipping.ShippingZip1);
			shipping.ShippingZip2 = StringUtility.ToHankaku(shipping.ShippingZip2);
			shipping.ShippingZip = StringUtility.ToHankaku(shipping.ShippingZip);
			shipping.ShippingAddr2 = DataInputUtility.ConvertToFullWidthBySetting(
				shipping.ShippingAddr2,
				shipping.IsShippingAddrJp);
			shipping.ShippingAddr3 = DataInputUtility.ConvertToFullWidthBySetting(
				shipping.ShippingAddr3,
				shipping.IsShippingAddrJp);
			shipping.ShippingAddr4 = DataInputUtility.ConvertToFullWidthBySetting(
				shipping.ShippingAddr4,
				shipping.IsShippingAddrJp);
			shipping.ShippingAddr5 = DataInputUtility.ConvertToFullWidthBySetting(
				shipping.ShippingAddr5,
				shipping.IsShippingAddrJp);

			// ギフト注文でない場合は送り主情報補正をしない
			if (order.IsGiftOrder == false) continue;

			// 送り主情報
			shipping.SenderName1 = DataInputUtility.ConvertToFullWidthBySetting(
				shipping.SenderName1,
				shipping.IsSenderAddrJp);
			shipping.SenderName2 = DataInputUtility.ConvertToFullWidthBySetting(
				shipping.SenderName2,
				shipping.IsSenderAddrJp);
			shipping.SenderName = DataInputUtility.ConvertToFullWidthBySetting(
				shipping.SenderName,
				shipping.IsSenderAddrJp);
			shipping.SenderNameKana1 = StringUtility.ToZenkaku(shipping.SenderNameKana1);
			shipping.SenderNameKana2 = StringUtility.ToZenkaku(shipping.SenderNameKana2);
			shipping.SenderNameKana = StringUtility.ToZenkaku(shipping.SenderNameKana);
			shipping.SenderTel1_1 = StringUtility.ToHankaku(shipping.SenderTel1_1);
			shipping.SenderTel1_2 = StringUtility.ToHankaku(shipping.SenderTel1_2);
			shipping.SenderTel1_3 = StringUtility.ToHankaku(shipping.SenderTel1_3);
			shipping.SenderTel1 = StringUtility.ToHankaku(shipping.SenderTel1);
			shipping.SenderZip1 = StringUtility.ToHankaku(shipping.SenderZip1);
			shipping.SenderZip2 = StringUtility.ToHankaku(shipping.SenderZip2);
			shipping.SenderZip = StringUtility.ToHankaku(shipping.SenderZip);
			shipping.SenderAddr2 = DataInputUtility.ConvertToFullWidthBySetting(
				shipping.SenderAddr2,
				shipping.IsSenderAddrJp);
			shipping.SenderAddr3 = DataInputUtility.ConvertToFullWidthBySetting(
				shipping.SenderAddr3,
				shipping.IsSenderAddrJp);
			shipping.SenderAddr4 = DataInputUtility.ConvertToFullWidthBySetting(
				shipping.SenderAddr4,
				shipping.IsSenderAddrJp);
			shipping.SenderAddr5 = DataInputUtility.ConvertToFullWidthBySetting(
				shipping.SenderAddr5,
				shipping.IsSenderAddrJp);
		}
	}

	/// <summary>
	/// お届け先情報が変更されているか
	/// </summary>
	/// <param name="oldShipping">配送先情報（変更後）</param>
	/// <param name="newShipping">配送先情報（変更前）</param>
	/// <returns>変更有:true 変更無:false</returns>
	private bool CheckShippingChanged(OrderShippingModel oldShipping, OrderShippingModel newShipping)
	{
		return ((oldShipping.ShippingCountryIsoCode != newShipping.ShippingCountryIsoCode)
			|| (oldShipping.ShippingAddr1 != newShipping.ShippingAddr1)
			|| (oldShipping.ShippingAddr2 != newShipping.ShippingAddr2)
			|| (oldShipping.ShippingAddr3 != newShipping.ShippingAddr3)
			|| (oldShipping.ShippingAddr4 != newShipping.ShippingAddr4)
			|| (oldShipping.ShippingAddr5 != newShipping.ShippingAddr5)
			|| (oldShipping.ShippingZip != newShipping.ShippingZip)
			|| (oldShipping.ShippingTel1 != newShipping.ShippingTel1)
			|| (oldShipping.ShippingTel2 != newShipping.ShippingTel2)
			|| (oldShipping.ShippingName != newShipping.ShippingName)
			|| (oldShipping.ShippingNameKana != newShipping.ShippingNameKana)
			|| (oldShipping.ShippingCompanyName != newShipping.ShippingCompanyName)
			|| (oldShipping.ShippingCompanyPostName != newShipping.ShippingCompanyPostName));
	}

	/// <summary>
	/// Is Payment At Convenience Store
	/// </summary>
	/// <param name="shippingReceivingStoreType">Shipping Receiving Store Type</param>
	/// <returns>True: Payment at convenience store, otherwise: false</returns>
	protected bool IsPaymentAtConvenienceStore(string shippingReceivingStoreType)
	{
		var result = ((shippingReceivingStoreType == Constants.FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_FAMILY_MART_PAYMENT)
			|| (shippingReceivingStoreType == Constants.FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_7_ELEVENT_PAYMENT)
			|| (shippingReceivingStoreType == Constants.FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_HI_LIFE_PAYMENT));
		return result;
	}

	/// <summary>
	/// カード情報取得ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnGetCardInfo_Click(object sender, EventArgs e)
	{
		if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans)
		{
			// カード情報入力チェック
			var orderCreditCard = GetOrderCreditCardInputForOrderPage(Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT, this.User.UserId);
			var paymentErrorMessage = orderCreditCard.Validate(true, WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_VERITRANS_PAYTG_CARD_UNREGISTERD));
			if (string.IsNullOrEmpty(paymentErrorMessage) == false)
			{
				DisplayOrderPaymentErrorMessage(paymentErrorMessage);
				return;
			}

			hfPayTgSendId.Value = new UserCreditCardCooperationInfoVeritrans(this.User.UserId).CooperationId1;

			// PayTG連携APIのポストデータ作成
			hfPayTgPostData.Value = Constants.PAYMENT_SETTING_PAYTG_MOCK_ENABLED
				? string.Empty
				: new PayTgApiForVeriTrans(hfPayTgSendId.Value).CreatePostData();

			var apiUrl = Constants.PAYMENT_SETTING_PAYTG_MOCK_ENABLED
				? CreateRegisterCardVeriTransMockUrl(hfPayTgSendId.Value)
				: Constants.PAYMENT_SETTING_PAYTG_REGISTCREDITURL;

			// PayTG連動でカード登録実行
			ScriptManager.RegisterStartupScript(
				this,
				GetType(),
				"execRegistration",
				string.Format("execCardRegistration('{0}');", apiUrl),
				true);
		}
		else if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten)
		{
			// PayTg WebApiで利用するため決済注文IDを採番
			var paymentOrderId = OrderCommon.CreatePaymentOrderId(this.LoginOperatorShopId);
			hfPayTgSendId.Value = paymentOrderId;

			// PayTG連携APIのポストデータ作成
			hfPayTgPostData.Value = new PayTgApi(paymentOrderId).CreatePostData();

			var apiUrl = Constants.PAYMENT_SETTING_PAYTG_MOCK_ENABLED
				? CreateRegisterCardMockUrl()
				: Constants.PAYMENT_SETTING_PAYTG_BASEURL + PayTgConstants.PspShortName.RAKUTEN + PayTgConstants.DealingTypes.URL_CHECKCARD;

			// PayTG連動でカード登録実行
			ScriptManager.RegisterStartupScript(
				this,
				GetType(),
				"execRegistration",
				string.Format("execCardRegistration('{0}');", apiUrl),
				true);
		}
	}

	/// <summary>
	/// PayTG連携のレスポンス処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnProcessPayTgResponse_Click(object sender, EventArgs e)
	{
		if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans)
		{
			var payTg = new PayTgApiForVeriTrans(hfPayTgSendId.Value);
			payTg.ParseResponse(hfPayTgResponse.Value);

			this.IsSuccessfulCardRegistration = false;

			if (payTg.Result.IsSuccess)
			{
				var cardExpire = payTg.Result.Response.CardExpire.Split('/');
				var cardNumber = payTg.Result.Response.CardNumber;
				var payTgResponse = new Hashtable
				{
					{ VeriTransConst.PAYTG_CARD_EXPIRE_MONTH, cardExpire[0] },
					{ VeriTransConst.PAYTG_CARD_EXPIRE_YEAR, cardExpire[1] },
					{ VeriTransConst.PAYTG_CARD_NUMBER, cardNumber },
					{ VeriTransConst.PAYTG_RESPONSE_ERROR, string.Empty },
				};
				this.PayTgResponse = payTgResponse;

				ddlCreditExpireMonth.SelectedValue = (string)this.PayTgResponse[VeriTransConst.PAYTG_CARD_EXPIRE_MONTH];
				ddlCreditExpireYear.SelectedValue = (string)this.PayTgResponse[VeriTransConst.PAYTG_CARD_EXPIRE_YEAR];
				tbCreditCardNo1.Text = cardNumber;
				DisplayOrderPaymentErrorMessage(string.Empty);
				this.IsSuccessfulCardRegistration = true;
			}
			else
			{
				var payTgResponse =
					new Hashtable { { VeriTransConst.PAYTG_RESPONSE_ERROR, payTg.Result.ErrorMessages } };
				this.PayTgResponse = payTgResponse;
				DisplayOrderPaymentErrorMessage(payTg.Result.ErrorMessages);
				this.IsSuccessfulCardRegistration = false;
			}
		}
		else if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten)
		{
			var payTg = new PayTgApi(hfPayTgSendId.Value);
			payTg.ParseResponse(hfPayTgResponse.Value);

			this.IsSuccessfulCardRegistration = false;

			if (payTg.Result.IsSuccess)
			{
				var cardExpireMonth = payTg.Result.Response.McAcntNo1;
				var cardExpireYear = payTg.Result.Response.Expire;
				var lastFourDigit = payTg.Result.Response.Last4;
				var cardNumber = "XXXXXXXXXXXX" + lastFourDigit;
				var companyCode = payTg.Result.Response.AcqName;
				var token = payTg.Result.Response.Token;
				var vResultCode = payTg.Result.Response.VResultCode;
				var errorMsg = payTg.Result.Response.ErrorMsg;

				var payTgResponse = new Hashtable
				{
					{ PayTgConstants.PAYTG_CARD_EXPIRE_MONTH, cardExpireMonth },
					{ PayTgConstants.PAYTG_CARD_EXPIRE_YEAR, cardExpireYear },
					{ PayTgConstants.PARAM_LAST4, lastFourDigit },
					{ PayTgConstants.PAYTG_CARD_NUMBER, cardNumber },
					{ PayTgConstants.PARAM_ACQNAME, companyCode },
					{ PayTgConstants.PARAM_TOKEN, token },
					{ PayTgConstants.PAYTG_RESPONSE_ERROR, string.Empty },
				};
				Session[PayTgConstants.PARAM_TOKEN] = hfPayTgSendId.Value = payTg.Result.Response.Token;
				Session[PayTgConstants.PARAM_ACQNAME] = companyCode;
				this.PayTgResponse = payTgResponse;

				ddlCreditExpireMonth.SelectedValue = cardExpireMonth;
				ddlCreditExpireYear.SelectedValue = cardExpireYear.Substring(cardExpireYear.Length - 2);
				tbCreditCardNo1.Text = cardNumber;

				DisplayOrderPaymentErrorMessage(string.Empty);
				this.IsSuccessfulCardRegistration = true;
			}
			else
			{
				// PCサイト向けに優先したいクレジットカードメッセージ
				var cardErrorMessageForPc = string.Empty;

				this.IsSuccessfulCardRegistration = false;
				var resultCode = payTg.Result.Response.VResultCode;

				if (string.IsNullOrEmpty(resultCode))
				{
					// PayTg端末のエラーの場合はエラーメッセージを統一
					resultCode = PayTgConstants.ERRMSG_PAYTG_UNAVAILABLE;
				}
				var creditError = new CreditErrorMessage();
				creditError.SetCreditErrorMessages(Constants.FILE_XML_RAKUTEN_CREDIT_ERROR_MESSAGE);
				var errorList = creditError.GetValueItemArray();
				cardErrorMessageForPc = (errorList.Any(s => s.Value == resultCode))
					? errorList.First(s => (s.Value == resultCode)).Text
					: string.Empty;

				var payTgResponse = new Hashtable { { PayTgConstants.PAYTG_RESPONSE_ERROR, cardErrorMessageForPc } };
				this.PayTgResponse = payTgResponse;
				DisplayOrderPaymentErrorMessage(cardErrorMessageForPc);
			}
		}
		DisplayCreditInputForm();
	}

	/// <summary>
	/// PayTg端末状態取得
	/// </summary>
	private void GetPayTgDeviceStatus()
	{
		var apiUrl = Constants.PAYMENT_SETTING_PAYTG_MOCK_ENABLED
			? Constants.PATH_ROOT + Constants.PAGE_MANAGER_CHECK_DEVICE_STATUS_MOCK
			: Constants.PAYMENT_SETTING_PAYTG_DEVICE_STATUS_CHECK_URL;

		// PayTG連動でカード登録実行
		ScriptManager.RegisterStartupScript(
			this,
			GetType(),
			"execGetDeviceStatus",
			string.Format("execGetPayTgDeviceStatus('{0}');", apiUrl),
			true);
	}

	/// <summary>
	/// Set schedule shiping and date shiping
	/// </summary>
	public void SetScheduleShipingAndDateShiping()
	{
		foreach (RepeaterItem ri in rShippingList.Items)
		{
			var ucScheduledShippingDate = (DateTimePickerPeriodInputControl)ri.FindControl("ucScheduledShippingDate");
			var ucShippingDate = (DateTimePickerPeriodInputControl)ri.FindControl("ucShippingDate");
			var scheduledShippingDate = string.Empty;
			var shippingDate = string.Empty;

			if (this.IsBackFromConfirm)
			{
				var orderModify = (Hashtable)Session[Constants.SESSIONPARAM_KEY_ORDER_MODIFY_INFO];
				var order = (OrderInput)orderModify[Constants.TABLE_ORDER];

				scheduledShippingDate = order.Shippings[ri.ItemIndex].ScheduledShippingDate;
				shippingDate = order.Shippings[ri.ItemIndex].ShippingDate;
			}
			else
			{
				scheduledShippingDate = this.OrderInput.Shippings[ri.ItemIndex].ScheduledShippingDate;
				shippingDate = this.OrderInput.Shippings[ri.ItemIndex].ShippingDate;
			}

			if (Validator.IsDate(scheduledShippingDate))
			{
				ucScheduledShippingDate.SetStartDate(DateTime.Parse(scheduledShippingDate));
			}
			if (Validator.IsDate(shippingDate))
			{
				ucShippingDate.SetStartDate(DateTime.Parse(shippingDate));
			}
		}
	}

	/// <summary>
	/// Linkbutton search address from owner zipcode global click event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSearchAddrFromOwnerZipGlobal_Click(object sender, EventArgs e)
	{
		BindingAddressByGlobalZipcode(
			this.OwnerAddrCountryIsoCode,
			StringUtility.ToHankaku(tbOwnerZipGlobal.Text.Trim()),
			tbOwnerAddr2,
			tbOwnerAddr3,
			tbOwnerAddr4,
			tbOwnerAddr5,
			ddlOwnerAddr5);
	}

	/// <summary>
	/// Linkbutton search address from shipping zipcode global click event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSearchAddrFromShippingZipGlobal_Click(object sender, EventArgs e)
	{
		var firstRepeaterItem = rShippingList.Items[0];
		var ddlShippingCountry = (DropDownList)firstRepeaterItem.FindControl("ddlShippingCountry");
		var shippingCountryName = ddlShippingCountry.SelectedItem.Text;
		var countryIsoCode = (string.IsNullOrEmpty(shippingCountryName) == false)
			? this.ShippingAvailableCountryList.FirstOrDefault(country => (country.CountryName == shippingCountryName)).CountryIsoCode
			: string.Empty;

		var tbShippingZipGlobal = (TextBox)firstRepeaterItem.FindControl("tbShippingZipGlobal");
		var tbShippingAddr2 = (TextBox)firstRepeaterItem.FindControl("tbShippingAddr2");
		var tbShippingAddr3 = (TextBox)firstRepeaterItem.FindControl("tbShippingAddr3");
		var tbShippingAddr4 = (TextBox)firstRepeaterItem.FindControl("tbShippingAddr4");
		var tbShippingAddr5 = (TextBox)firstRepeaterItem.FindControl("tbShippingAddr5");
		var ddlShippingAddr5 = (DropDownList)firstRepeaterItem.FindControl("ddlShippingAddr5");

		BindingAddressByGlobalZipcode(
			countryIsoCode,
			StringUtility.ToHankaku(tbShippingZipGlobal.Text.Trim()),
			tbShippingAddr2,
			tbShippingAddr3,
			tbShippingAddr4,
			tbShippingAddr5,
			ddlShippingAddr5);
	}

	/// <summary>
	/// Linkbutton search address from sender zipcode global click event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSearchAddrFromSenderZipGlobal_Click(object sender, EventArgs e)
	{
		var firstRepeaterItem = rShippingList.Items[0];
		var ddlSenderCountry = (DropDownList)firstRepeaterItem.FindControl("ddlSenderCountry");
		var senderCountryName = ddlSenderCountry.SelectedItem.Text;
		var countryIsoCode = (string.IsNullOrEmpty(senderCountryName) == false)
			? this.UserCountryList.FirstOrDefault(country => (country.CountryName == senderCountryName)).CountryIsoCode
			: string.Empty;

		var tbSenderZipGlobal = (TextBox)firstRepeaterItem.FindControl("tbSenderZipGlobal");
		var tbSenderAddr2 = (TextBox)firstRepeaterItem.FindControl("tbSenderAddr2");
		var tbSenderAddr3 = (TextBox)firstRepeaterItem.FindControl("tbSenderAddr3");
		var tbSenderAddr4 = (TextBox)firstRepeaterItem.FindControl("tbSenderAddr4");
		var tbSenderAddr5 = (TextBox)firstRepeaterItem.FindControl("tbSenderAddr5");
		var ddlSenderAddr5 = (DropDownList)firstRepeaterItem.FindControl("ddlSenderAddr5");

		BindingAddressByGlobalZipcode(
			countryIsoCode,
			StringUtility.ToHankaku(tbSenderZipGlobal.Text.Trim()),
			tbSenderAddr2,
			tbSenderAddr3,
			tbSenderAddr4,
			tbSenderAddr5,
			ddlSenderAddr5);
	}

	/// <summary>
	/// リピータ内にあるコントロールのコントロールIDを取得する
	/// </summary>
	/// <param name="controlName">コントロール名</param>
	/// <returns>コントロールID</returns>
	protected string GetControlIdByRepeaterItem(string controlName)
	{
		foreach (RepeaterItem ri in rShippingList.Items)
		{
			var control = (ri.FindControl(controlName));
			if (control != null)
			{
				return control.ClientID;
			}
		}
		return string.Empty;
	}

	/// <summary>
	/// 配送情報リピータデータバインドとリピータ内出荷予定日と配送希望日を再表示
	/// </summary>
	private void ShippingListDataBindAndSetShipingDate()
	{
		// 出荷予定日と配送希望日はデータバインド形式で値設定していないので、
		// リピータデータバインド後、設定済みの出荷予定日と配送希望日を表示したい場合は
		// 再設定する必要があり
		rShippingList.DataBind();
		SetScheduleShipingAndDateShiping();
	}

	/// <summary>
	/// 商品情報エリアリピータイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rItemList_OnItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		var productPriceControl = (TextBox)e.Item.FindControl("tbProductPrice");
		if (productPriceControl == null) return;

		var itemQuantityControl = (TextBox)e.Item.FindControl("tbItemQuantity");
		var itemPriceControl = (HtmlGenericControl)e.Item.FindControl("sItemPrice");

		// 頒布会注文オプションがFalse または 頒布会コースIDがセットされていない（=頒布会注文ではない） ならreturn
		if ((Constants.SUBSCRIPTION_BOX_OPTION_ENABLED == false) || string.IsNullOrEmpty(this.OrderInput.SubscriptionBoxCourseId)) return;

		var item = (OrderItemInput)(e.Item.DataItem);

		var selectedSubscriptionBox = DataCacheControllerFacade
			.GetSubscriptionBoxCacheController()
			.Get(this.OrderInput.SubscriptionBoxCourseId);

		// 頒布会キャンペーン期間かどうか
		var selectedSubscriptionBoxItem = selectedSubscriptionBox.SelectableProducts.FirstOrDefault(
			x => (x.ProductId == item.ProductId) && (x.VariationId == item.VariationId));

		if (OrderCommon.IsSubscriptionBoxCampaignPeriod(selectedSubscriptionBoxItem))
		{
			var itemQuantity = 0;
			productPriceControl.Text = selectedSubscriptionBoxItem.CampaignPrice.ToPriceString();
			if (int.TryParse(itemQuantityControl.Text, out itemQuantity))
			{
				itemPriceControl.InnerHtml = GetMinusNumberNoticeHtml(
					(selectedSubscriptionBoxItem.CampaignPrice * itemQuantity).ToPriceString(),
					true);
			}
		}
	}

	/// <summary>
	/// 商品付帯情報作成
	/// </summary>
	/// <param name="riItem">付帯情報選択領域のリピータアイテム</param>
	/// <returns>商品付帯情報設定リスト</returns>
	private string CreateProductOptionSettingList(RepeaterItem riItem)
	{
		if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED == false)
		{
			var optionTexts = ((TextBox)riItem.FindControl("tbProductOptionSettingSelectedTexts")).Text;
			return optionTexts;
		}

		var productId = ((HiddenField)riItem.FindControl("hfProductId")).Value;
		var settingList = new ProductOptionSettingList(this.LoginOperatorShopId, productId);

		// 選択値セット
		foreach (var settingItem in settingList.Items)
		{
			var index = settingList.Items.IndexOf(settingItem);
			var riTarget = ((Repeater)riItem.FindControl("rProductOptionValueSettings")).Items[index];
			settingItem.SelectedSettingValue = GetSelectedProductOptionValue(riTarget);
		}
		return settingList.GetJsonStringFromSelectValues();
	}

	/// <summary>
	/// 選択されている商品付帯情報値を取得する
	/// </summary>
	/// <param name="riItem">付帯情報選択用リピータ</param>
	/// <returns>選択されている付帯情報</returns>
	private string GetSelectedProductOptionValue(RepeaterItem riItem)
	{
		var rCblProductOptionValueSetting = (Repeater)riItem.FindControl("rCblProductOptionValueSetting");
		var ddlProductOptionValueSetting = (DropDownList)riItem.FindControl("ddlProductOptionValueSetting");
		var tbProductOptionValueSetting = (TextBox)riItem.FindControl("tbProductOptionValueSetting");
		if (rCblProductOptionValueSetting.Visible)
		{
			var lSelectedValues = new List<string>();
			foreach (RepeaterItem riCheckBox in rCblProductOptionValueSetting.Items)
			{
				var cbOption = ((CheckBox)(riCheckBox.FindControl("cbProductOptionValueSetting")));
				if (cbOption.Checked == false) continue;

				lSelectedValues.Add(cbOption.Text);
			}
			return string.Join(
				Constants.PRODUCTOPTIONVALUES_SEPARATING_CHAR_SELECT_SETTING_VALUE,
				lSelectedValues.ToArray());
		}

		if (ddlProductOptionValueSetting.Visible)
		{
			return ddlProductOptionValueSetting.SelectedValue;
		}

		if (tbProductOptionValueSetting.Visible)
		{
			return tbProductOptionValueSetting.Text;
		}
		return null;
	}

	/// <summary>
	/// Get valid store pickup payments
	/// </summary>
	/// <returns>List payment models</returns>
	private PaymentModel[] GetValidStorePickupPayments()
	{
		var payments = DomainFacade.Instance.PaymentService
			.GetValidPayments(this.LoginOperatorShopId)
			.Where(item => Constants.SETTING_CAN_STORE_PICKUP_OPTION_PAYMENT_IDS.Contains(item.PaymentId)
				&& (Constants.SETTING_CAN_NOT_STORE_PICKUP_OPTION_PAYMENT_IDS.Contains(item.PaymentId) == false))
			.ToArray();

		return payments;
	}

	/// <summary>
	/// Get real shop value
	/// </summary>
	/// <param name="realShopId">Real shop ID</param>
	/// <param name="dataType">Data type</param>
	/// <returns>Real shop value</returns>
	protected string GetRealShopValue(string realShopId, string dataType)
	{
		var result = string.Empty;
		if (RealShopModels.Any(rs => rs.RealShopId == realShopId) == false) return result;

		var model = RealShopModels.FirstOrDefault(rs => rs.RealShopId == realShopId);
		switch (dataType)
		{
			case "Address":
				result = string.Format("〒{0}<br />{1} {2}<br />{3}<br />{4}<br />{5}",
					model.Zip,
					model.Addr1,
					model.Addr2,
					model.Addr3,
					model.Addr4,
					model.Addr5);
				break;

			case "OpeningHours":
				result = model.OpeningHours;
				break;

			case "Tel":
				result = model.Tel;
				break;
		}
		return result;
	}

	/// <summary>
	/// Real store selected index changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlRealStore_SelectedIndexChanged(object sender, EventArgs e)
	{
		var ddlRealStore = (DropDownList)sender;
		if (this.RealShopModels.Any(rs => rs.RealShopId == ddlRealStore.SelectedValue) == false) return;

		var parentControl = ((DropDownList)sender).Parent;
		var lbStoreAddress = (Label)parentControl.FindControl("lbStoreAddress");
		var lbStoreOpeningHours = (Label)parentControl.FindControl("lbStoreOpeningHours");
		var lbStoreTel = (Label)parentControl.FindControl("lbStoreTel");
		lbStoreAddress.Text = GetRealShopValue(ddlRealStore.SelectedValue, "Address");
		lbStoreOpeningHours.Text = GetRealShopValue(ddlRealStore.SelectedValue, "OpeningHours");
		lbStoreTel.Text = GetRealShopValue(ddlRealStore.SelectedValue, "Tel");
	}

	/// <summary>
	/// Check Paidy payment price change
	/// </summary>
	private void CheckPaidyPaymentPriceChange()
	{
		if ((this.OrderInputOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_EXPIRED)
			|| (this.OrderInputOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_CANCEL_EXPIRED))
		{
			return;
		}

		var oldLastBilledAmount = decimal.Parse(this.OrderInputOld.LastBilledAmount);
		var lastBilledAmount = decimal.Parse(this.OrderInput.LastBilledAmount);

		if ((oldLastBilledAmount < lastBilledAmount)
			&& (this.OrderInputOld.OrderPaymentKbn == ddlOrderPaymentKbn.SelectedValue))
		{
			this.OrderErrorMessages.AppendLine(WebMessages.GetMessages(
				WebMessages.ERRMSG_MANAGER_PAYMENT_AMOUNT_PAIDY_CANNOT_INCREASE));
			return;
		}

		if ((oldLastBilledAmount > lastBilledAmount)
			&& (this.OrderInput.ExternalPaymentStatus != Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP))
		{
			this.OrderErrorMessages.AppendLine(WebMessages.GetMessages(
				WebMessages.ERRMSG_MANAGER_PAYMENT_AMOUNT_PAIDY_ORDER_PAYMENT_STATUS_NOT_COMP));
			return;
		}
	}

	/// <summary>注文者の住所が日本か</summary>
	protected bool IsOwnerAddrJp
	{
		get { return IsCountryJp(this.OwnerAddrCountryIsoCode); }
	}
	/// <summary>注文者の住所がアメリカか</summary>
	protected bool IsOwnerAddrUs
	{
		get { return IsCountryUs(this.OwnerAddrCountryIsoCode); }
	}
	/// <summary>注文者の住所郵便番号が必須か</summary>
	protected bool IsOwnerAddrZipNecessary
	{
		get { return IsAddrZipcodeNecessary(this.OwnerAddrCountryIsoCode); }
	}
	/// <summary>注文者の住所国ISOコード</summary>
	protected string OwnerAddrCountryIsoCode
	{
		get { return ddlOwnerCountry.SelectedValue; }
	}
	/// <summary>配送会社リスト</summary>
	protected ListItemCollection DelilveryCompanyList { get; set; }
	/// <summary>デフォルト配送会社</summary>
	protected string DefaultDeliveryCompany { get; set; }
	/// <summary>注文領収書エラーメッセージ</summary>
	private StringBuilder OrderReceiptErrorMessages
	{
		get { return m_orderReceiptErrorMessages; }
	}
	readonly StringBuilder m_orderReceiptErrorMessages = new StringBuilder();
	/// <summary>現在の設定 付与ポイント</summary>
	protected string OrderPointAdd
	{
		get { return StringUtility.ToNumeric((string)ViewState["OrderPointAdd"]); }
		set { ViewState["OrderPointAdd"] = value; }
	}
	/// <summary>Is Shipping Convenience</summary>
	protected bool IsShippingConvenience
	{
		get
		{
			return this.OrderInput.Shippings
				.Any(shipping =>
					(shipping.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON));
		}
	}
	/// <summary>Is Update Shipping Convenience</summary>
	protected bool IsUpdateShippingConvenience
	{
		get
		{
			var result = ((this.ActionStatus == Constants.ACTION_STATUS_UPDATE_SHIPPING)
				&& Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED);
			return result;
		}
	}
	/// <summary>配送サービス選択情報</summary>
	protected int DeliveryServiceSelected
	{
		get { return (int)ViewState["DeliveryServiceSelected"]; }
		set { ViewState["DeliveryServiceSelected"] = value; }
	}
	/// <summary>楽天連携以外か</summary>
	protected bool IsNotRakutenAgency
	{
		get { return (Constants.PAYMENT_CARD_KBN != w2.App.Common.Constants.PaymentCard.Rakuten); }
	}

	/// <summary>確認画面から戻ってきたか</summary>
	protected bool IsBackFromConfirm
	{
		get { return (Session[Constants.SESSION_KEY_PARAM_FOR_BACK2] != null); }
	}
	/// <summary>ポストバック時、エラーが発生したか</summary>
	protected bool HasErrorOnPostback
	{
		get { return (bool)ViewState["HasErrorOnPostback"]; }
		set { ViewState["HasErrorOnPostback"] = value; }
	}
	/// <summary>頒布会カートか</summary>
	protected bool IsSubscriptionBoxValid
	{
		get { return (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED && (string.IsNullOrEmpty(this.OrderInput.SubscriptionBoxCourseId) == false)); }
	}
	/// <summary>定期購入回数エリア表示するかどうか</summary>
	protected bool IsFixedPurchaseCountAreaShow
	{
		get
		{
			var result = (this.OrderInput.Shippings[0].Items.Any(orderItem => orderItem.IsFixedPurchaseItem)
				&& Constants.FIXEDPURCHASE_OPTION_ENABLED);
			return result;
		}
	}
	/// <summary>ユーザークレジットカード情報</summary>
	protected UserCreditCard UserCreditCard { get; set; }
	/// <summary>注文商品の頒布会コースIDエリアを表示するか</summary>
	protected bool DisplayItemSubscriptionBoxCourseIdArea
	{
		get
		{
			return Constants.SUBSCRIPTION_BOX_OPTION_ENABLED && this.OrderInput.IsOrderCombinedWithSubscriptionBoxItem;
		}
	}
	/// <summary>表示用注文商品頒布会コースID（エンコード済み）</summary>
	protected string EncodedSubscriptionBoxCourseIdForDisplay
	{
		get
		{
			var result = HtmlSanitizer.HtmlEncodeChangeToBr(
				string.Join(Environment.NewLine, this.OrderInput.ItemSubscriptionBoxCourseIds));
			return result;
		}
	}
	/// <summary>Check if shipping type store pickup can be use</summary>
	protected bool CanUseStorePickup
	{
		get
		{
			var storePickupFlg = (DomainFacade
				.Instance
				.ProductService
				.GetProducts(
					Constants.CONST_DEFAULT_SHOP_ID,
					this.OrderInput.Shippings.SelectMany(sp => sp.Items.Select(it => it.ProductId)))
				.Any(pr => pr.StorePickupFlg == Constants.FLG_OFF) == false);
			return storePickupFlg;
		}
	}
	/// <summary>Check if selected shipping type is store pickup</summary>
	protected bool IsStorePickup
	{
		get
		{
			var ddlShippingMethod = (DropDownList)rShippingList.Controls[0].FindControl("ddlShippingMethod");
			var repeater = (RepeaterItem)((DropDownList)ddlShippingMethod).Parent;
			var ddlUserShipping = ((DropDownList)repeater.FindControl("ddlUserShipping"));
			return (ddlUserShipping.SelectedValue == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_STORE_PICKUP);
		}
	}
	/// <summary>Real shop model list</summary>
	protected RealShopModel[] RealShopModels
	{
		get { return (RealShopModel[])ViewState["real_shop_models"]; }
		set { ViewState["real_shop_models"] = value; }
	}
	/// <summary>Create list item data source for real shop</summary>
	protected ListItem[] RealShopDataSource
	{
		get
		{
			var value = new List<ListItem>() { new ListItem(string.Empty, string.Empty) };
			value.AddRange(RealShopModels.Select(rs => new ListItem(rs.Name, rs.RealShopId)));
			return value.ToArray();
		}
	}
	/// <summary>PayTgクレジットトークン</summary>
	protected string CreditTokenbyPayTg
	{
		get { return (string)this.Session[PayTgConstants.PARAM_TOKEN]; }
		set { this.Session[PayTgConstants.PARAM_TOKEN] = value; }
	}
	/// <summary>PayTgクレジット会社コード</summary>
	protected string CreditCardCompanyCodebyPayTg
	{
		get { return (string)this.Session[PayTgConstants.PARAM_ACQNAME]; }
		set { this.Session[PayTgConstants.PARAM_ACQNAME] = value; }
	}
}

/// <summary>注文エラー例外クラス</summary>
public class OrderErrorException : Exception { }
