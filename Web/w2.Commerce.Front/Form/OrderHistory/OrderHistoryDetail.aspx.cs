/*
=========================================================================================================
  Module      : 注文履歴詳細画面処理(OrderHistoryDetail.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using Amazon.Pay.API.WebStore.CheckoutSession;
using Newtonsoft.Json;
using SessionWrapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using w2.App.Common;
using w2.App.Common.Amazon;
using w2.App.Common.Amazon.Helper;
using w2.App.Common.Amazon.Util;
using w2.App.Common.AmazonCv2;
using w2.App.Common.CrossPoint.Point;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Global;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Input.Order;
using w2.App.Common.NextEngine;
using w2.App.Common.Option;
using w2.App.Common.Option.CrossPoint;
using w2.App.Common.Order;
using w2.App.Common.Order.Cart;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.Boku;
using w2.App.Common.Order.Payment.Boku.Response;
using w2.App.Common.Order.Payment.Boku.Utils;
using w2.App.Common.Order.Payment.ECPay;
using w2.App.Common.Order.Payment.LinePay;
using w2.App.Common.Order.Payment.NPAfterPay;
using w2.App.Common.Order.Payment.Paidy;
using w2.App.Common.Order.Payment.Paygent;
using w2.App.Common.Order.Payment.PayPal;
using w2.App.Common.Order.Payment.Paypay;
using w2.App.Common.Order.Payment.YamatoKa.Utils;
using w2.App.Common.Order.Reauth;
using w2.App.Common.Order.Register;
using w2.App.Common.OrderExtend;
using w2.App.Common.Product;
using w2.App.Common.SendMail;
using w2.App.Common.User;
using w2.App.Common.Util;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Logger;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain;
using w2.Domain.CountryLocation;
using w2.Domain.DeliveryCompany;
using w2.Domain.FixedPurchase;
using w2.Domain.Holiday.Helper;
using w2.Domain.Order;
using w2.Domain.Point;
using w2.Domain.Product;
using w2.Domain.ProductStock;
using w2.Domain.RealShop;
using w2.Domain.ShopShipping;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.Domain.User.Helper;
using w2.Domain.UserCreditCard;
using w2.Domain.UserShipping;

public partial class Form_Order_OrderHistoryDetail : OrderHistoryPage
{
	# region ラップ済みコントロール宣言
	public WrappedRepeater WrOrderShipping { get { return GetWrappedControl<WrappedRepeater>("rOrderShipping"); } }
	public WrappedTextBox WtbOrderPointUse { get { return GetWrappedControl<WrappedTextBox>("tbOrderPointUse"); } }
	public WrappedHiddenField WhfPaymentNameSelected { get { return GetWrappedControl<WrappedHiddenField>("hfPaymentNameSelected"); } }
	public WrappedHiddenField WhfPaymentIdSelected { get { return GetWrappedControl<WrappedHiddenField>("hfPaymentIdSelected"); } }
	public WrappedHiddenField WhfTotalPrice { get { return GetWrappedControl<WrappedHiddenField>("hfTotalPrice"); } }
	public WrappedHiddenField WhfPaymentTotalPriceNew { get { return GetWrappedControl<WrappedHiddenField>("hfPaymentTotalPriceNew"); } }
	public WrappedHiddenField WhfShippingTotalPriceNew { get { return GetWrappedControl<WrappedHiddenField>("hfShippingTotalPriceNew"); } }
	public WrappedHtmlGenericControl WlsErrorMessagePointUse { get { return GetWrappedControl<WrappedHtmlGenericControl>("slErrorMessagePointUse"); } }
	protected WrappedHtmlGenericControl WdvOrderPaymentPattern { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvOrderPaymentPattern"); } }
	protected WrappedCheckBox WcbIsUpdateFixedPurchaseByOrderPayment { get { return GetWrappedControl<WrappedCheckBox>("cbIsUpdateFixedPurchaseByOrderPayment"); } }
	protected WrappedCheckBox WcbIsUpdateFixedPurchaseByOrderShippingInfo { get { return GetWrappedControl<WrappedCheckBox>("cbIsUpdateFixedPurchaseByOrderShippingInfo"); } }
	public WrappedHiddenField WhfConfirmSenderId { get { return GetWrappedControl<WrappedHiddenField>("hfConfirmSenderId"); } }
	public WrappedHiddenField WhfIsCheckFixedPurchaseFirstTime { get { return GetWrappedControl<WrappedHiddenField>("hfIsCheckFixedPurchaseFirstTime"); } }
	protected WrappedDropDownList WddlReceiptFlg { get { return GetWrappedControl<WrappedDropDownList>("ddlReceiptFlg"); } }
	protected WrappedTextBox WtbReceiptAddress { get { return GetWrappedControl<WrappedTextBox>("tbReceiptAddress"); } }
	protected WrappedTextBox WtbReceiptProviso { get { return GetWrappedControl<WrappedTextBox>("tbReceiptProviso"); } }
	protected WrappedHtmlGenericControl WtrReceiptAddressInput { get { return GetWrappedControl<WrappedHtmlGenericControl>("trReceiptAddressInput"); } }
	protected WrappedHtmlGenericControl WtrReceiptProvisoInput { get { return GetWrappedControl<WrappedHtmlGenericControl>("trReceiptProvisoInput"); } }
	protected WrappedHtmlGenericControl WdvReceiptAddressProvisoInput { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvReceiptAddressProvisoInput"); } }
	protected WrappedLinkButton WlbUpdatePayment { get { return GetWrappedControl<WrappedLinkButton>("lbUpdatePayment"); } }
	public WrappedHiddenField WhfAtoneTransactionId { get { return GetWrappedControl<WrappedHiddenField>("hfAtoneTransactionId"); } }
	public WrappedHiddenField WhfAfteeTransactionId { get { return GetWrappedControl<WrappedHiddenField>("hfAfteeTransactionId"); } }
	public WrappedHiddenField WhfFixedPurchasePaymentId { get { return GetWrappedControl<WrappedHiddenField>("hfFixedPurchasePaymentId"); } }
	protected WrappedLinkButton WlbDisplayInputOrderPointUse { get { return GetWrappedControl<WrappedLinkButton>("lbDisplayInputOrderPointUse"); } }
	protected WrappedHtmlGenericControl WslErrorMessageChangePointUse { get { return GetWrappedControl<WrappedHtmlGenericControl>("slErrorMessageChangePointUse"); } }
	protected WrappedLinkButton WlbDisplayInputOrderPaymentKbn { get { return GetWrappedControl<WrappedLinkButton>("lbDisplayInputOrderPaymentKbn"); } }
	protected WrappedHiddenField WhfAtoneToken { get { return GetWrappedControl<WrappedHiddenField>("hfAtoneToken"); } }
	protected WrappedHiddenField WhfAfteeToken { get { return GetWrappedControl<WrappedHiddenField>("hfAfteeToken"); } }
	protected WrappedLinkButton WlbRequestCvsDefInvoiceReissue { get { return GetWrappedControl<WrappedLinkButton>("lbRequestCvsDefInvoiceReissue"); } }
	protected WrappedLiteral WlCvsDefInvoiceReissueRequestResultMessage { get { return GetWrappedControl<WrappedLiteral>("lCvsDefInvoiceReissueRequestResultMessage"); } }
	protected WrappedHiddenField whfShowModal { get { return GetWrappedControl<WrappedHiddenField>("hfShowModal"); } }
	protected WrappedLinkButton WlbOrderExtend { get { return GetWrappedControl<WrappedLinkButton>("lbOrderExtend"); } }
	protected WrappedRepeater WrOrderExtendInput { get { return GetWrappedControl<WrappedRepeater>("rOrderExtendInput"); } }
	protected WrappedRepeater WrOrderExtendDisplay { get { return GetWrappedControl<WrappedRepeater>("rOrderExtendDisplay"); } }
	protected WrappedHiddenField WhfShippingAddress { get { return GetWrappedControl<WrappedHiddenField>("hfShippingAddress"); } }
	protected WrappedLinkButton WlbReceiptDownload { get { return GetWrappedControl<WrappedLinkButton>("lbReceiptDownload"); } }
	protected WrappedRadioButton WrbTwInvoiceCancel { get { return GetWrappedControl<WrappedRadioButton>("rbTwInvoiceCancel"); } }
	protected WrappedRadioButton WrbTwInvoiceRefund { get { return GetWrappedControl<WrappedRadioButton>("rbTwInvoiceRefund"); } }
	protected WrappedLinkButton WbtnModifyProducts { get { return GetWrappedControl<WrappedLinkButton>("btnModifyProducts"); } }
	protected WrappedLinkButton WbtnModifyConfirm { get { return GetWrappedControl<WrappedLinkButton>("btnModifyConfirm"); } }
	protected WrappedLinkButton WbtnModifyCancel { get { return GetWrappedControl<WrappedLinkButton>("btnModifyCancel"); } }
	protected WrappedHtmlGenericControl WsModifyErrorMessage { get { return GetWrappedControl<WrappedHtmlGenericControl>("sModifyErrorMessage"); } }
	protected WrappedLiteral WlOrderPriceSubtotal { get { return GetWrappedControl<WrappedLiteral>("lOrderPriceSubtotal"); } }
	protected WrappedLiteral WlOrderPriceSubtptalTax { get { return GetWrappedControl<WrappedLiteral>("lOrderPriceSubtptalTax"); } }
	protected WrappedLiteral WlOrderMemberRankDiscountPrice { get { return GetWrappedControl<WrappedLiteral>("lOrderMemberRankDiscountPrice"); } }
	protected WrappedLiteral WlFixedPurchaseMemberDiscountAmount { get { return GetWrappedControl<WrappedLiteral>("lFixedPurchaseMemberDiscountAmount"); } }
	protected WrappedLiteral WlCouponName { get { return GetWrappedControl<WrappedLiteral>("lCouponName"); } }
	protected WrappedLiteral WlOrderCouponUse { get { return GetWrappedControl<WrappedLiteral>("lOrderCouponUse"); } }
	protected WrappedLiteral WlOrderPointUseYen { get { return GetWrappedControl<WrappedLiteral>("lOrderPointUseYen"); } }
	protected WrappedLiteral WlFixedPurchaseDiscountPrice { get { return GetWrappedControl<WrappedLiteral>("lFixedPurchaseDiscountPrice"); } }
	protected WrappedLiteral WlOrderPriceRegulation { get { return GetWrappedControl<WrappedLiteral>("lOrderPriceRegulation"); } }
	protected WrappedLiteral WlOrderPriceShipping { get { return GetWrappedControl<WrappedLiteral>("lOrderPriceShipping"); } }
	protected WrappedLiteral WlOrderPriceExchange { get { return GetWrappedControl<WrappedLiteral>("lOrderPriceExchange"); } }
	protected WrappedLiteral WlOrderPriceTotal { get { return GetWrappedControl<WrappedLiteral>("lOrderPriceTotal"); } }
	protected WrappedRepeater WrProductSetPromotions { get { return GetWrappedControl<WrappedRepeater>("rProductSetPromotions"); } }
	protected WrappedRepeater WrShippingSetPromotions { get { return GetWrappedControl<WrappedRepeater>("rShippingSetPromotions"); } }
	protected WrappedRepeater WrPaymentSetPromotions { get { return GetWrappedControl<WrappedRepeater>("rPaymentSetPromotions"); } }
	protected WrappedHiddenField WhfhfProducts { get { return GetWrappedControl<WrappedHiddenField>("hfProducts"); } }
	protected WrappedHiddenField WhfCounts { get { return GetWrappedControl<WrappedHiddenField>("hfCounts"); } }
	protected WrappedHiddenField WhfOrderPriceSubtotalNew { get { return GetWrappedControl<WrappedHiddenField>("hfOrderPriceSubtotalNew"); } }
	protected WrappedLiteral WlPaymentErrorMessage { get { return GetWrappedControl<WrappedLiteral>("lPaymentErrorMessage"); } }
	protected WrappedHtmlGenericControl WsNoveltyChangeNoticeMessage { get { return GetWrappedControl<WrappedHtmlGenericControl>("sNoveltyChangeNoticeMessage"); } }
	public WrappedHiddenField WhfPaidyPaymentId { get { return GetWrappedControl<WrappedHiddenField>("hfPaidyPaymentId"); } }
	public WrappedHiddenField WhfPaidyStatus { get { return GetWrappedControl<WrappedHiddenField>("hfPaidyStatus"); } }
	#endregion

	#region Constants
	/// <summary>Order history error message constant</summary>
	protected const string ORDER_HISTORY_ERROR_MESSAGE = "OrderHistoryErrorMessage";
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		// HTTPS通信チェック（HTTPの場合、HTTPSで再読込）
		CheckHttps(this.SecurePageProtocolAndHost + Request.Url.PathAndQuery);

		Session[Constants.SESSION_KEY_CART_LIST_FOR_PAYPAY] = null;

		// データ読み込み
		SetOrderHistory();

		// 配送情報入力画面初期処理（共通）
		InitComponentsOrderShipping();

		InitAmazonPay();

		var wucPaymentYamatoKaSmsAuthModal = GetYamatoKaSmsAuthModal();
		if (wucPaymentYamatoKaSmsAuthModal != null)
		{
			wucPaymentYamatoKaSmsAuthModal.OnAuthorizeComplete = ActionOnSmsAuthComplete;
		}

		if (!IsPostBack)
		{
			ViewState["RealShopModel"] = new RealShopService().Get(StringUtility.ToEmpty(this.OrderModel.Shippings[0].StorePickupRealShopId));

			ActionForFromLinePay();

			DataBind();

			ActionForFormBoku();

			whfShowModal.Value = "false";

			if (this.OrderModel.CreditBranchNo.HasValue)
			{
				var userCreditCard = new UserCreditCardService().Get(this.OrderModel.UserId, this.OrderModel.CreditBranchNo.Value);
				if (userCreditCard.CooperationType == Constants.FLG_USERCREDITCARD_COOPERATION_TYPE_YAMATOKASMS)
				{
					if (wucPaymentYamatoKaSmsAuthModal != null)
					{
						wucPaymentYamatoKaSmsAuthModal.TelNum = userCreditCard.CooperationId;
					}

				}
			}

			//表示コンポーネントの初期化
			InitializeComponents();

			// トークン決済の場合はクライアント検証をオフ
			DisableCreditInputCustomValidatorForGetCreditToken(this.CreditRepeaterItem);

			// Reset value
			this.OrderHistoryErrorMessage = string.Empty;

			if (this.NeedScrollToShippingArea)
			{
				ClientScript.RegisterStartupScript(this.GetType(), "hash", "location.hash = '#ShippingArea';", true);
				this.NeedScrollToShippingArea = false;
			}

			ActionFromAmazonPay();

			SetOrderInput(this.OrderModel);
		}
		else
		{
			// トークンが入力されていたら入力画面を切り替える
			SwitchDisplayForCreditTokenInput(this.CreditRepeaterItem);
			this.WlCvsDefInvoiceReissueRequestResultMessage.Text = CommerceMessages.GetMessages(
				CommerceMessages.ERRMSG_CVSDEF_INVOICE_REISSUE_COMPLETE);
		}

		// 選択された登録カードセット
		SetUserCreditCard(this.UserCreditCardsUsable);
		RefreshCreditForm();

		if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED)
		{
			CheckSiteDomainAndRedirectWithPostData();
			SetInformationReceivingStore();
		}

		if (Session[Constants.SESSION_KEY_ERROR_FOR_PAYPAY_PAYMENT] != null)
		{
			lbDisplayInputOrderPaymentKbn_Click(sender, e);
			this.WsErrorMessagePayment.InnerHtml = WebSanitizer.HtmlEncodeChangeToBr(
				WebMessages.GetMessages(WebMessages.ERRMSG_SYSTEM_ERROR));

			Session.Remove(Constants.SESSION_KEY_ERROR_FOR_PAYPAY_PAYMENT);
		}

		// For case the order with payout PayPay
		if (this.IsExecutionPayPayOrder)
		{
			ExecutePayPayAndUpdateOrder(sender, e);
		}

		// Handle display of control
		if (this.WlbRequestCvsDefInvoiceReissue.HasInnerControl)
		{
			this.WlbRequestCvsDefInvoiceReissue.InnerControl.OnClientClick = (this.WlbRequestCvsDefInvoiceReissue.InnerControl.Enabled == false)
				? string.Empty
				: string.Format(
					"return confirm('{0}');",
					WebMessages.GetMessages(WebMessages.ERRMSG_CVSDEF_INVOICE_REISSUE_CONFIRM));

			this.WlbRequestCvsDefInvoiceReissue.InnerControl.Text = WebMessages.GetMessages(WebMessages.ERRMSG_CVSDEF_INVOICE_REISSUE);
		}

		// 継続課金解約エラーメッセージを表示
		if (string.IsNullOrEmpty(this.PaymentContinousCancelErrorMessage) == false)
		{
			lbDisplayInputOrderPaymentKbn_Click(sender, e);
			this.WsErrorMessagePayment.InnerHtml = this.PaymentContinousCancelErrorMessage;
			this.PaymentContinousCancelErrorMessage = null;
		}
	}

	/// <summary>
	/// AmazonPay初期化
	/// </summary>
	private void InitAmazonPay()
	{
		this.OrderModel.OrderPaymentKbn
			= OrderCommon.ConvertAmazonPaymentId(this.OrderModel.OrderPaymentKbn);

		var amazonCallbackPath = AmazonCv2ApiFacade.CreateCallbackPathForOneTime(
			AmazonCv2Constants.AMAZON_ACTION_STATUS_CREATE_SESSION,
			this.OrderModel.OrderId);

		this.AmazonRequest = AmazonCv2Redirect.SignPayloadForOneTime(amazonCallbackPath);

		this.AmazonFacade = new AmazonCv2ApiFacade();

		if (this.Request[AmazonCv2Constants.REQUEST_KEY_AMAZON_CHECKOUT_SESSION_ID] != null)
		{
			this.AmazonCheckoutSessionId = this.Request[AmazonCv2Constants.REQUEST_KEY_AMAZON_CHECKOUT_SESSION_ID];
			this.AmazonCheckoutSession = this.AmazonFacade.GetCheckoutSession(this.AmazonCheckoutSessionId);
		}
	}

	/// <summary>
	/// LinePayから戻ってきたときのアクション（取得できたトランザクションIDをもとに再与信実施）
	/// </summary>
	private void ActionForFromLinePay()
	{
		if (this.Request["action"] == "linepay")
		{
			var param = (Hashtable)this.Session[Constants.SESSION_KEY_PARAM];
			if ((param == null)
				|| (param.ContainsKey("order_new") == false)
				|| (((OrderModel)param["order_new"]).OrderId != this.OrderModel.OrderId))
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_ANOTHER_BROWSER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}

			var orderNew = (OrderModel)param["order_new"];
			orderNew.CardTranId = this.Request["transactionId"];

			var reauthResult = ExecuteChangeOrderForPayment(this.OrderModel, orderNew);
			if (reauthResult)
			{
				this.Session[Constants.SESSION_KEY_PARAM] = null;
				this.Response.Redirect(
					new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_HISTORY_DETAIL).AddParam(
						Constants.REQUEST_KEY_ORDER_ID,
						this.OrderModel.OrderId).CreateUrl());
			}
		}
	}

	/// <summary>
	/// AmazonPayCv2から遷移
	/// </summary>
	protected void ActionFromAmazonPay()
	{
		if (string.IsNullOrEmpty(this.AmazonCheckoutSessionId)) return;

		switch (Request[AmazonCv2Constants.REQUEST_KEY_AMAZON_ACTION_STATUS])
		{
			case AmazonCv2Constants.AMAZON_ACTION_STATUS_CREATE_SESSION:
				this.WdvOrderPaymentPattern.Visible = true;
				if (Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_RB)
				{
					foreach (RepeaterItem wrPaymentItem in this.WrPayment.Items)
					{
						var wrbgTmpPayment = GetWrappedControl<WrappedRadioButtonGroup>(wrPaymentItem, "rbgPayment");
						wrbgTmpPayment.Checked = false;
					}
					var wrbgPayment = GetWrappedControl<WrappedRadioButtonGroup>(this.AmazonPayCv2RepeaterItem, "rbgPayment");
					wrbgPayment.Checked = true;
					if (wrbgPayment.InnerControl != null)
					{
						rbgPayment_OnCheckedChanged(wrbgPayment, EventArgs.Empty);
						var sender = GetWrappedControl<WrappedLinkButton>("lbUpdatePayment");
						btnUpdatePaymentPatternInfo_Click(sender, EventArgs.Empty);
					}
				}
				else if (Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_DDL)
				{
					var wddlPayment = GetWrappedControl<WrappedDropDownList>(this.AmazonPayCv2RepeaterItem.Parent.Parent, "ddlPayment");
					wddlPayment.SelectedValue = Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2;
					if (wddlPayment.InnerControl != null)
					{
						rbgPayment_OnCheckedChanged(wddlPayment, EventArgs.Empty);
						var sender = GetWrappedControl<WrappedLinkButton>("lbUpdatePayment");
						btnUpdatePaymentPatternInfo_Click(sender, EventArgs.Empty);
					}
				}
				break;

			case AmazonCv2Constants.AMAZON_ACTION_STATUS_AUTH:
				var param = (Hashtable)this.Session[Constants.SESSION_KEY_PARAM];
				if ((param == null)
					|| (param.ContainsKey("order_new") == false)
					|| (((OrderModel)param["order_new"]).OrderId != this.OrderModel.OrderId))
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_ANOTHER_BROWSER_ERROR);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
				}
				var orderNew = (OrderModel)param["order_new"];

				var con = this.AmazonFacade.CompleteCheckoutSession(this.AmazonCheckoutSessionId, orderNew.OrderPriceTotal);
				var conResult = con.Success;
				var conError = AmazonCv2ApiFacade.GetErrorCodeAndMessage(con);

				// 新規与信時のPaymetログ出力
				PaymentFileLogger.WritePaymentLog(
					conResult,
					this.OrderModel.OrderPaymentKbn,
					PaymentFileLogger.PaymentType.AmazonCv2,
					PaymentFileLogger.PaymentProcessingType.OrderInfoApproval,
					conResult
						? string.Empty
						: OrderCommon.CreateOrderFailedLogMessage(
							this.TransactionName,
							new Hashtable
							{
								{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, con.ChargePermissionId },
								{ Constants.FIELD_ORDER_ORDER_ID, this.OrderModel.OrderId },
								{ Constants.FIELD_ORDER_USER_ID, this.OrderModel.UserId }
							},
							CartObject.CreateCartByOrder(this.OrderModel),
							LogCreator.CreateErrorMessage(conError.ReasonCode, conError.Message)),
					new Dictionary<string, string>
					{
						{ Constants.FIELD_ORDER_ORDER_ID, this.OrderModel.OrderId },
						{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, con.ChargePermissionId },
					});

				OrderCommon.AppendExternalPaymentCooperationLog(
					conResult,
					this.OrderModel.OrderId,
					conResult
						? string.Format("hfAmazonOrderRefID:{0}", con.ChargePermissionId)
						: LogCreator.CreateErrorMessage(conError.ReasonCode, conError.Message),
					Constants.FLG_LASTCHANGED_USER,
					UpdateHistoryAction.Insert);

				if (conResult == false)
				{
					this.WdvOrderPaymentPattern.Visible = true;
					if (Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_RB)
					{
						foreach (RepeaterItem wrPaymentItem in this.WrPayment.Items)
						{
							var wrbgTmpPayment = GetWrappedControl<WrappedRadioButtonGroup>(wrPaymentItem, "rbgPayment");
							wrbgTmpPayment.Checked = false;
						}
						var wrbgAmazonPayment = GetWrappedControl<WrappedRadioButtonGroup>(this.AmazonPayCv2RepeaterItem, "rbgPayment");
						wrbgAmazonPayment.Checked = true;
						if (wrbgAmazonPayment.InnerControl != null)
						{
							rbgPayment_OnCheckedChanged(wrbgAmazonPayment, EventArgs.Empty);
						}
					}
					else if (Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_DDL)
					{
						var wddlAmazonPayment = GetWrappedControl<WrappedDropDownList>(this.AmazonPayCv2RepeaterItem.Parent.Parent, "ddlPayment");
						wddlAmazonPayment.SelectedValue = Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2;
						if (wddlAmazonPayment.InnerControl != null)
						{
							rbgPayment_OnCheckedChanged(wddlAmazonPayment, EventArgs.Empty);
						}
					}
					this.WsErrorMessagePayment.InnerHtml = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_AUTH_EXCEPTION);
					return;
				}

				// 与信取得に成功した場合、外部決済連携メモに記録する
				orderNew.AppendPaymentMemo(
					OrderCommon.CreateOrderPaymentMemo(
						con.ChargePermissionId,
						orderNew.OrderPaymentKbn,
						con.ChargeId,
						"与信",
						orderNew.LastBilledAmount));

				orderNew.PaymentOrderId = con.ChargePermissionId;
				orderNew.CardTranId = con.ChargeId;

				// 元与信キャンセル
				var errorResponse = OrderCommon.CancelAmazonpayCv2(
					this.OrderModel.CardTranId,
					this.OrderModel.LastBilledAmount,
					this.OrderModel.OnlinePaymentStatus,
					this.AmazonFacade);

				// 元与信キャンセルのPaymentログ出力
				PaymentFileLogger.WritePaymentLog(
					string.IsNullOrEmpty(errorResponse),
					this.OrderModel.OrderPaymentKbn,
					PaymentFileLogger.PaymentType.AmazonCv2,
					PaymentFileLogger.PaymentProcessingType.CancelPayment,
					string.IsNullOrEmpty(errorResponse)
						? string.Empty
						: errorResponse,
					new Dictionary<string, string>
					{
						{ Constants.FIELD_ORDER_ORDER_ID, this.OrderModel.OrderId },
						{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, this.OrderModel.PaymentOrderId},
						{ Constants.FOR_LOG_KEY_AMAZON_ORDER_REFERENCE_ID, this.OrderModel.PaymentOrderId}
					});

				// 古い承認を（ChargePermission）終了
				var closeResponse = this.AmazonFacade.CloseChargePermission(this.OrderModel.PaymentOrderId);

				orderNew.PaymentOrderId = con.ChargePermissionId;
				orderNew.CardTranId = con.ChargeId;

				// キャンセル情報を外部決済連携メモに記録する
				orderNew.AppendPaymentMemo(
					OrderCommon.CreateOrderPaymentMemo(
						this.OrderModel.PaymentOrderId,
						this.OrderModel.OrderPaymentKbn,
						this.OrderModel.CardTranId,
						(string.IsNullOrEmpty(errorResponse) && closeResponse.Success)
							? "キャンセル"
							: "キャンセル失敗",
						orderNew.LastBilledAmount));

				// 受注情報の更新(再与信実施なし)
				var reauthResult = ExecuteChangeOrderForPayment(this.OrderModel, orderNew);

				if (reauthResult)
				{
					new OrderService().UpdateCardTranId(
						this.OrderModel.OrderId,
						con.ChargeId,
						Constants.FLG_LASTCHANGED_USER,
						UpdateHistoryAction.Insert);
					this.Response.Redirect(
						new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_HISTORY_DETAIL)
							.AddParam(Constants.REQUEST_KEY_ORDER_ID, this.OrderModel.OrderId).CreateUrl());
				}
				break;
		}
	}

	/// <summary>
	/// スマホ決済SMS完了時アクション
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void ActionOnSmsAuthComplete(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(this.OnAuthorizeCompleteMethodName)) return;
		var method = GetType().GetMethod(this.OnAuthorizeCompleteMethodName, BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance);
		this.OnAuthorizeCompleteMethodName = string.Empty;
		if (method != null) method.Invoke(this, new[] { sender, e });
	}

	/// <summary>
	/// セットプロモーションRowSpan取得 (データバインド用)
	/// </summary>
	/// <param name="orderItem">対象の注文商品</param>
	/// <param name="orderItems">全注文商品</param>
	/// <returns></returns>
	protected int GetSetPromotionRowSpan(object orderItem, object orderItems)
	{
		if ((((DataRowView)orderItem)[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO]).ToString() == "") return 1;

		var setPromotionRowSpan = ((List<DataRowView>)orderItems).FindAll(
			oi => ((oi[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO]).ToString() != "")
				&& ((int)oi[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO] == (int)((DataRowView)orderItem)[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO])).Count;
		return setPromotionRowSpan;
	}
	/// <summary>
	/// セットプロモーションRowSpan取得 (データバインド用)
	/// </summary>
	/// <param name="orderItem">対象の注文商品</param>
	/// <param name="orderItems">全注文商品</param>
	/// <returns>RowSpan</returns>
	protected int GetSetPromotionRowSpan(OrderItemInput orderItem, object orderItems)
	{
		if (string.IsNullOrEmpty(orderItem.OrderSetpromotionNo)) return 1;

		var setPromotionRowSpan = ((List<OrderItemInput>)orderItems).FindAll(
			product => (string.IsNullOrEmpty(product.OrderSetpromotionNo) == false)
				&& (product.OrderSetpromotionNo == orderItem.OrderSetpromotionNo)
				&& (product.ModifyDeleteTarget == false)).Count;
		return setPromotionRowSpan;
	}

	/// <summary>
	/// 商品セットアイテムRowSpan取得（データバインド用）
	/// </summary>
	/// <param name="objOrderItem">注文商品</param>
	/// <param name="objOrderItems">全注文商品</param>
	/// <returns></returns>
	protected int GetProductSetRowspan(object objOrderItem, object objOrderItems)
	{
		if (((string)((DataRowView)objOrderItem)[Constants.FIELD_ORDERITEM_PRODUCT_SET_ID]).Length == 0) return 1;

		int iSettIems = 0;
		foreach (DataRowView drv in (List<DataRowView>)objOrderItems)
		{
			if (((string)drv[Constants.FIELD_ORDERITEM_PRODUCT_SET_ID]).Length != 0)
			{
				if (((string)((DataRowView)objOrderItem)[Constants.FIELD_ORDERITEM_PRODUCT_SET_ID] == (string)drv[Constants.FIELD_ORDERITEM_PRODUCT_SET_ID])
					&& ((int)((DataRowView)objOrderItem)[Constants.FIELD_ORDERITEM_PRODUCT_SET_NO] == (int)drv[Constants.FIELD_ORDERITEM_PRODUCT_SET_NO]))
				{
					iSettIems++;
				}
			}
		}
		return iSettIems;
	}
	/// <summary>
	/// 商品セットアイテムRowSpan取得（データバインド用）
	/// </summary>
	/// <param name="orderItem">注文商品</param>
	/// <param name="orderItems">全注文商品</param>
	/// <returns>RowSpan</returns>
	protected int GetProductSetRowspan(OrderItemInput orderItem, object orderItems)
	{
		if (orderItem.IsProductSet == false) return 1;

		var result = ((List<OrderItemInput>)orderItems)
			.Where(product => product.IsProductSet)
			.Count(product => (orderItem.ProductSetId == product.ProductSetId)
				&& (int.Parse(orderItem.ProductSetNo) == int.Parse(product.ProductSetNo)));
		return result;
	}

	/// <summary>
	/// 商品セットのアイテムTOPか否か取得（データバインド用）
	/// </summary>
	/// <param name="objOrderItem"></param>
	/// <param name="objOrderItems"></param>
	/// <returns></returns>
	protected bool IsProductSetItemTop(object objOrderItem, object objOrderItems)
	{
		if (((string)((DataRowView)objOrderItem)[Constants.FIELD_ORDERITEM_PRODUCT_SET_ID]).Length == 0) return false;

		DataRowView drvBefore = null;
		foreach (DataRowView drv in (List<DataRowView>)objOrderItems)
		{
			if ((DataRowView)objOrderItem == drv)
			{
				if (drvBefore == null)
				{
					return true;
				}
				else if (((string)((DataRowView)objOrderItem)[Constants.FIELD_ORDERITEM_PRODUCT_SET_ID] != (string)drvBefore[Constants.FIELD_ORDERITEM_PRODUCT_SET_ID])
					|| ((int)((DataRowView)objOrderItem)[Constants.FIELD_ORDERITEM_PRODUCT_SET_NO] != (int)drvBefore[Constants.FIELD_ORDERITEM_PRODUCT_SET_NO]))
				{
					return true;
				}
				break;
			}
			drvBefore = drv;
		}
		return false;
	}
	/// <summary>
	/// 商品セットのアイテムTOPか否か取得（データバインド用）
	/// </summary>
	/// <param name="orderItem">注文商品</param>
	/// <param name="orderItems">全注文商品</param>
	/// <returns>結果</returns>
	protected bool IsProductSetItemTop(OrderItemInput orderItem, object orderItems)
	{
		if (orderItem.IsProductSet == false) return false;

		OrderItemInput beforeProduct = null;
		foreach (var product in (List<OrderItemInput>)orderItems)
		{
			if (orderItem == product)
			{
				if (beforeProduct == null)
				{
					return true;
				}
				if ((orderItem.ProductSetId != beforeProduct.ProductSetId)
						|| (int.Parse(orderItem.ProductSetNo) != int.Parse(beforeProduct.ProductSetNo)))
				{
					return true;
				}
				break;
			}
			beforeProduct = product;
		}
		return false;
	}

	/// <summary>
	/// 商品セットの最後のアイテムか否か取得（データバインド用）
	/// </summary>
	/// <param name="objOrderItem">チェック対象の注文商品</param>
	/// <param name="objOrderItems">該当注文の注文商品すべて</param>
	/// <returns></returns>
	protected bool IsProductSetItemLast(object objOrderItem, object objOrderItems)
	{
		if (((string)((DataRowView)objOrderItem)[Constants.FIELD_ORDERITEM_PRODUCT_SET_ID]).Length == 0) return false;

		DataRowView drvCurrent = null;
		foreach (DataRowView drv in (List<DataRowView>)objOrderItems)
		{
			if (drvCurrent == null)
			{
				if ((DataRowView)objOrderItem == drv)
				{
					// 対象商品の情報を保持して次へ
					drvCurrent = drv;
					continue;
				}
			}
			else
			{
				// 対象商品とその次の商品を比較して同じセット商品でなければTrueを返す
				return (((string)drvCurrent[Constants.FIELD_ORDERITEM_PRODUCT_SET_ID] != (string)drv[Constants.FIELD_ORDERITEM_PRODUCT_SET_ID])
					|| ((int)drvCurrent[Constants.FIELD_ORDERITEM_PRODUCT_SET_NO] != (int)drv[Constants.FIELD_ORDERITEM_PRODUCT_SET_NO]));
			}
		}
		// 最後の商品の場合は対象商品情報の保持だけしてループを抜けるのでここでTrueを返す
		return (drvCurrent != null);
	}

	/// <summary>
	/// 商品セット価格小計取得（セット価格ｘ個数 データバインド用）
	/// </summary>
	/// <param name="objOrderItem"></param>
	/// <param name="objOrderItems"></param>
	/// <returns></returns>
	protected decimal CreateSetPriceSubtotal(object objOrderItem, object objOrderItems)
	{
		decimal dSetPriceSubtotal = 0;
		foreach (DataRowView drv in (List<DataRowView>)objOrderItems)
		{
			if (((string)drv[Constants.FIELD_ORDERITEM_PRODUCT_SET_ID]).Length != 0)
			{
				if (((string)((DataRowView)objOrderItem)[Constants.FIELD_ORDERITEM_PRODUCT_SET_ID] == (string)drv[Constants.FIELD_ORDERITEM_PRODUCT_SET_ID])
					&& ((int)((DataRowView)objOrderItem)[Constants.FIELD_ORDERITEM_PRODUCT_SET_NO] == (int)drv[Constants.FIELD_ORDERITEM_PRODUCT_SET_NO]))
				{
					dSetPriceSubtotal += (decimal)drv[Constants.FIELD_ORDERITEM_PRODUCT_PRICE]
						* (int)drv[Constants.FIELD_ORDERITEM_ITEM_QUANTITY];
				}
			}
		}
		return dSetPriceSubtotal;
	}
	/// <summary>
	/// 商品セット価格小計取得（セット価格ｘ個数 データバインド用）
	/// </summary>
	/// <param name="orderItem">注文商品</param>
	/// <param name="orderItems">全注文商品</param>
	/// <returns>商品セット価格小計</returns>
	protected decimal CreateSetPriceSubtotal(OrderItemInput orderItem, object orderItems)
	{
		decimal setPriceSubtotal = 0;
		foreach (var product in (List<OrderItemInput>)orderItems)
		{
			if (product.IsProductSet)
			{
				if ((orderItem.ProductSetId == product.ProductSetId)
					&& (int.Parse(orderItem.ProductSetNo) != int.Parse(product.ProductSetNo)))
				{
					setPriceSubtotal += decimal.Parse(product.ProductPrice)
						* int.Parse(product.ItemQuantity);
				}
			}
		}
		return setPriceSubtotal;
	}

	/// <summary>
	/// 定期注文リンクボタン クリックイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbDisplayFixedPurchaseDetail_Click(object sender, EventArgs e)
	{
		Response.Redirect(PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(this.OrderModel.FixedPurchaseId));
	}

	/// <summary>
	/// お支払い方法変更ボタンのクリックイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbDisplayInputOrderPaymentKbn_Click(object sender, EventArgs e)
	{
		this.WdvOrderPaymentPattern.Visible = (this.WdvOrderPaymentPattern.Visible == false);

		// 決済種別ラジオボタンクリックイベント
		if (Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_RB)
		{
			var wrbgPayment = GetWrappedControl<WrappedRadioButtonGroup>(this.PaymentRepeaterItem ?? this.CreditRepeaterItem, "rbgPayment");
			if (wrbgPayment.InnerControl != null) rbgPayment_OnCheckedChanged(wrbgPayment, EventArgs.Empty);
		}
		else if (Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_DDL)
		{
			var wddlPayment = GetWrappedControl<WrappedDropDownList>(
				this.PaymentRepeaterItem != null
					? this.PaymentRepeaterItem.Parent.Parent
					: this.CreditRepeaterItem.Parent.Parent,
				"ddlPayment");
			if (wddlPayment.InnerControl != null) rbgPayment_OnCheckedChanged(wddlPayment, EventArgs.Empty);
		}
	}

	/// <summary>
	/// 支払い方法 キャンセルボタンのクリックイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnClosePaymentPatternInfo_Click(object sender, EventArgs e)
	{
		this.WdvOrderPaymentPattern.Visible = (this.WdvOrderPaymentPattern.Visible == false);
	}

	/// <summary>
	/// 決済種別変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected new void rbgPayment_OnCheckedChanged(object sender, EventArgs e)
	{
		var isRb = (Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_RB);
		var orderPriceTotal = 0m;
		foreach (RepeaterItem riPayment in this.WrPayment.Items)
		{
			var wrbgPayment = GetWrappedControl<WrappedRadioButtonGroup>(riPayment, "rbgPayment");
			var wddlPayment = GetWrappedControl<WrappedDropDownList>(riPayment.Parent.Parent, "ddlPayment");
			var whfPaymentName = GetWrappedControl<WrappedHiddenField>(riPayment, "hfPaymentName", string.Empty);
			var whfPaymentPrice = GetWrappedControl<WrappedHiddenField>(riPayment, "hfPaymentPrice", string.Empty);
			var whfPaymentId = GetWrappedControl<WrappedHiddenField>(riPayment, "hfPaymentId", "");
			var wddCredit = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddCredit");
			var wddCvsDef = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddCvsDef");
			var wddSmsDef = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddSmsDef");
			var wddTriLinkAfterPayPayment = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddTriLinkAfterPayPayment");
			var wddCollect = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddCollect");
			var wddAmazonPay = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddAmazonPay");
			var wddAmazonPayCv2 = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddAmazonPayCv2");
			var wddPayPal = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddPayPal");
			var wddNoPayment = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddNoPayment");
			var wddPaidy = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddPaidy");
			var wucPaidyCheckoutControl = GetWrappedControl<WrappedPaidyCheckoutControl>(riPayment, "ucPaidyCheckoutControl");
			var wddAtonePay = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddPaymentAtone");
			var wddAfteePay = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddPaymentAftee");
			var wddNpAfterPay = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddNpAfterPay");
			var wddCarrierbillingBokuPayment = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddCarrierbillingBokuPayment");
			var wddLinePay = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddLinePay");
			var wddDskDef = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddDskDef");
			var wddPayPay = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddPayPay");

			wddCredit.Visible = (isRb
				? ((wrbgPayment.InnerControl != null) && wrbgPayment.Checked)
				: ((wddlPayment.InnerControl != null) && (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)))
				&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT);
			wddCvsDef.Visible = (isRb
				? ((wrbgPayment.InnerControl != null) && wrbgPayment.Checked)
				: ((wddlPayment.InnerControl != null) && (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)))
				&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF);
			wddSmsDef.Visible = (isRb
				? ((wrbgPayment.InnerControl != null) && wrbgPayment.Checked)
				: ((wddlPayment.InnerControl != null) && (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_SMS_DEF)))
				&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_SMS_DEF);
			wddCollect.Visible = (isRb
				? ((wrbgPayment.InnerControl != null) && wrbgPayment.Checked)
				: ((wddlPayment.InnerControl != null) && (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT)))
				&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT);
			wddAmazonPay.Visible = (isRb
				? ((wrbgPayment.InnerControl != null) && wrbgPayment.Checked)
				: ((wddlPayment.InnerControl != null) && (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT)))
				&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT);
			wddAmazonPayCv2.Visible = (isRb
				? ((wrbgPayment.InnerControl != null) && wrbgPayment.Checked)
				: ((wddlPayment.InnerControl != null) && (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2)))
				&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2);
			wddPayPal.Visible = (isRb
				? ((wrbgPayment.InnerControl != null) && wrbgPayment.Checked)
				: ((wddlPayment.InnerControl != null) && (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)))
				&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL);
			wddTriLinkAfterPayPayment.Visible = (isRb
				? ((wrbgPayment.InnerControl != null) && wrbgPayment.Checked)
				: ((wddlPayment.InnerControl != null) && (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)))
				&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY);
			wddNoPayment.Visible = (isRb
				? ((wrbgPayment.InnerControl != null) && wrbgPayment.Checked)
				: ((wddlPayment.InnerControl != null) && (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT)))
				&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT);
			wddPaidy.Visible = (isRb
				? ((wrbgPayment.InnerControl != null) && wrbgPayment.Checked)
				: ((wddlPayment.InnerControl != null) && (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)))
				&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY);
			wucPaidyCheckoutControl.DisplayUserControl = wddPaidy.Visible;
			if (isRb ? wrbgPayment.Checked : wddlPayment.SelectedValue == whfPaymentId.Value)
			{
				this.WhfPaidyPaySelected.Value = wddPaidy.Visible.ToString();
			}
			wddAtonePay.Visible = (isRb
				? (wrbgPayment.HasInnerControl && wrbgPayment.Checked)
				: (wddlPayment.HasInnerControl && (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)))
				&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE);
			wddAfteePay.Visible = (isRb
				? (wrbgPayment.HasInnerControl && wrbgPayment.Checked)
				: (wddlPayment.HasInnerControl && (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE)))
				&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE);
			wddNpAfterPay.Visible = (isRb
				? ((wrbgPayment.InnerControl != null) && wrbgPayment.Checked)
				: ((wddlPayment.InnerControl != null) && (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)))
				&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY);
			wddCarrierbillingBokuPayment.Visible = ((wrbgPayment.InnerControl != null)
				&& wrbgPayment.Checked
				&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU));
			wddLinePay.Visible = (isRb
				? (wrbgPayment.HasInnerControl && wrbgPayment.Checked)
				: (wddlPayment.HasInnerControl && (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY)))
				&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY);
			wddDskDef.Visible = (isRb
				? (wrbgPayment.HasInnerControl && wrbgPayment.Checked)
				: (wddlPayment.HasInnerControl && (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_DSK_DEF)))
				&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_DSK_DEF);
			wddPayPay.Visible = (isRb
				? (wrbgPayment.HasInnerControl && wrbgPayment.Checked)
				: (wddlPayment.HasInnerControl && (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)))
				&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY);

			if (isRb
				? ((wrbgPayment.InnerControl != null) && wrbgPayment.Checked)
				: ((wddlPayment.InnerControl != null) && (wddlPayment.SelectedValue == whfPaymentId.Value)))
			{
				this.WhfPaymentNameSelected.Value = whfPaymentName.Value;
				this.WhfPaymentIdSelected.Value = whfPaymentId.Value;
				this.CheckedPaymentIdChanged = whfPaymentId.Value;

				//決済手数料変更による合計金額の再計算
				orderPriceTotal = this.OrderModel.OrderPriceTotal;
				if (GetSetPromotionPaymentChargeFree() != Constants.FLG_ORDERSETPROMOTION_PAYMENT_CHARGE_FREE_FLG_ON)
				{
					orderPriceTotal = orderPriceTotal - this.OrderModel.OrderPriceExchange + decimal.Parse(whfPaymentPrice.Value);
				}
				this.WhfPaymentTotalPriceNew.Value = orderPriceTotal.ToString();
			}

			if (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT)
			{
				this.WcbIsUpdateFixedPurchaseByOrderPayment.Checked
					= this.WcbIsUpdateFixedPurchaseByOrderPayment.Visible
					= this.WcbIsUpdateFixedPurchaseByOrderShippingInfo.Checked
					= this.WcbIsUpdateFixedPurchaseByOrderShippingInfo.Visible
					= (wddAmazonPay.Visible == false);
			}

			this.WcbIsUpdateFixedPurchaseByOrderPayment.Enabled = true;

			if (Constants.PAYMENT_ATONEOPTION_ENABLED
				|| Constants.PAYMENT_AFTEEOPTION_ENABLED)
			{
				CheckValidTelNoAndCountryForPaymentAtoneAndAftee(this.WhfPaymentIdSelected.Value);
			}
		}
		//エラー文言の初期化
		this.WsErrorMessagePayment.InnerHtml = "";

		StringBuilder paymentErrorMessage;
		var paymentModel = GetAndValidatePaymentInput(CartObject.CreateCartByOrder(this.OrderModel), out paymentErrorMessage);
		if (paymentErrorMessage.Length > 0 || paymentModel == null)
		{
			this.WsErrorMessagePayment.InnerHtml = paymentErrorMessage.ToString();
			return;
		}

		//決済手数料変更による合計金額の再計算
		orderPriceTotal = this.OrderModel.OrderPriceTotal;
		if (GetSetPromotionPaymentChargeFree() != Constants.FLG_ORDERSETPROMOTION_PAYMENT_CHARGE_FREE_FLG_ON)
		{
			orderPriceTotal = orderPriceTotal - this.OrderModel.OrderPriceExchange + paymentModel.PaymentPrice;
		}

		// 注文情報（変更後）取得
		var orderOld = new OrderService().Get(this.OrderModel.OrderId);
		var orderNew = orderOld.Clone();
		orderNew.ShopId = paymentModel.ShopId;
		orderNew.OrderPaymentKbn = paymentModel.PaymentId;
		orderNew.PaymentName = paymentModel.PaymentName;
		orderNew.OrderPriceExchange = paymentModel.PaymentPrice;
		orderNew.LastBilledAmount = orderPriceTotal;
		orderNew.OrderPriceTotal = orderPriceTotal;

		if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED
			&& orderNew.Shippings.Any(item => (item.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)))
		{
			var cart = CartObject.CreateCartByOrder(orderNew);
			if (cart != null)
			{
				orderNew.OrderPriceShipping = cart.PriceShipping;
				orderNew.OrderPriceTotal = cart.PriceTotal;
				this.WhfPaymentTotalPriceNew.Value = cart.PriceTotal.ToString();
			}
		}
	}

	/// <summary>
	/// 支払い方法 更新ボタンのクリックイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdatePaymentPatternInfo_Click(object sender, EventArgs e)
	{
		//支払い情報の取得
		StringBuilder paymentErrorMessage;
		var paymentModel = GetAndValidatePaymentInput(CartObject.CreateCartByOrder(this.OrderModel), out paymentErrorMessage);
		if (paymentErrorMessage.Length > 0 || paymentModel == null)
		{
			this.WsErrorMessagePayment.InnerHtml = paymentErrorMessage.ToString();
			return;
		}

		//決済手数料変更による合計金額の再計算
		var orderPriceTotal = this.OrderModel.OrderPriceTotal;
		var setpromotionPaymentChargeDiscountAmount = 0m;
		if (GetSetPromotionPaymentChargeFree() != Constants.FLG_ORDERSETPROMOTION_PAYMENT_CHARGE_FREE_FLG_ON)
		{
			orderPriceTotal = orderPriceTotal - this.OrderModel.OrderPriceExchange + paymentModel.PaymentPrice;
		}
		else
		{
			setpromotionPaymentChargeDiscountAmount = paymentModel.PaymentPrice;
		}

		// 注文情報（変更後）取得
		var orderOld = new OrderService().Get(this.OrderModel.OrderId);
		var orderNew = orderOld.Clone();
		var shippingModel = new ShopShippingService().Get(this.ShopId, orderNew.ShippingId);
		orderNew.ShopId = paymentModel.ShopId;
		orderNew.OrderPaymentKbn = paymentModel.PaymentId;
		orderNew.PaymentName = paymentModel.PaymentName;
		orderNew.OrderPriceExchange = paymentModel.PaymentPrice;
		orderNew.LastBilledAmount = orderPriceTotal;
		orderNew.OrderPriceTotal = orderPriceTotal;
		orderNew.DeviceInfo = this.Request[Constants.REQUEST_GMO_DEFERRED_DEVICE_INFO];
		orderNew.SetpromotionPaymentChargeDiscountAmount = setpromotionPaymentChargeDiscountAmount;

		if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED
			&& orderNew.Shippings.Any(item => (item.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON))
			&& CheckItemRelateWithServiceConvenienceStore(shippingModel))
		{
			var cart = CartObject.CreateCartByOrder(orderNew);
			if (cart != null)
			{
				orderNew.OrderPriceShipping = cart.PriceShipping;
				orderNew.OrderPriceTotal = cart.PriceTotal;
			}
		}

		if (paymentModel.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
		{
			var creditErrorMessage = "";
			var apiErrorMessage = "";
			// クレジットカード登録（更新履歴とともに）
			var userCreditCardInput = CreditCardProcessing(
				Constants.CREDITCARD_UNREGIST_DEFAULT_DISPLAY_NAME,
				out creditErrorMessage,
				out apiErrorMessage,
				UpdateHistoryAction.Insert);

			var hasApiErrorMessage = (string.IsNullOrEmpty(apiErrorMessage) == false);

			OrderCommon.AppendExternalPaymentCooperationLog(
				hasApiErrorMessage == false,
				orderOld.OrderId,
				hasApiErrorMessage ? apiErrorMessage : "クレジットカード登録成功",
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.Insert);

			if (userCreditCardInput == null || string.IsNullOrEmpty(creditErrorMessage) == false)
			{
				var wspanErrorMessageForCreditCard = GetWrappedControl<WrappedHtmlGenericControl>(
					this.CreditRepeaterItem, "spanErrorMessageForCreditCard");
				if (wspanErrorMessageForCreditCard.HasInnerControl)
				{
					wspanErrorMessageForCreditCard.InnerHtml = WebSanitizer.HtmlEncodeChangeToBr(creditErrorMessage);
					wspanErrorMessageForCreditCard.InnerControl.Style["display"] = "block";
				}
				return;
			}
			//支払い回数を設定
			var installmentsCode = (IsNewCreditCard())
				? Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten
					? GetWrappedControl<WrappedDropDownList>(this.RakutenCreditCardControll, "dllCreditInstallmentsRakuten").SelectedValue
					: this.WciCardInputs.WddlInstallments.SelectedValue
				: this.WciCardInputs.WdllCreditInstallments2.SelectedValue;
			orderNew.CreditBranchNo = int.Parse(userCreditCardInput.BranchNo);
			orderNew.CardInstallmentsCode = installmentsCode;
			orderNew.CardInstruments = ValueText.GetValueText(
				Constants.TABLE_ORDER,
				OrderCommon.CreditInstallmentsValueTextFieldName,
				installmentsCode);
			orderNew.CardKbn = userCreditCardInput.CompanyCode;
		}
		else if (paymentModel.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
		{
			if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
			{
				orderNew.CreditBranchNo = orderOld.CreditBranchNo;
			}
			else if (SessionManager.PayPalCooperationInfo != null)
			{
				var userCreditCard = PayPalUtility.Payment.RegisterAsUserCreditCard(
					this.LoginUserId,
					SessionManager.PayPalCooperationInfo,
					Constants.FLG_LASTCHANGED_USER,
					UpdateHistoryAction.Insert);
				orderNew.CreditBranchNo = userCreditCard.BranchNo;
			}
			else
			{
				this.WsErrorMessagePayment.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PAYPAL_NEEDS_LOGIN_ERROR);
				return;
			}
		}
		else if ((Constants.PAYMENT_PAIDY_KBN == Constants.PaymentPaidyKbn.Direct)
			&& paymentModel.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
		{
			// Check Country Shipping Valid If Payment Is Paidy
			if (Constants.GLOBAL_OPTION_ENABLE
				&& orderNew.Shippings.Any(shipping => (IsCountryJp(shipping.ShippingCountryIsoCode) == false)))
			{
				this.WsErrorMessagePayment.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PAIDY_COUNTRY_SHIPPING_NOT_JAPAN_ERROR);
				return;
			}

			if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
			{
				orderNew.CreditBranchNo = orderOld.CreditBranchNo;
			}
			else
			{
				var paidyTokenId = this.WhfPaidyTokenId.Value;
				if (string.IsNullOrEmpty(paidyTokenId) == false)
				{
					var userCredit = new UserCreditCardService().GetByCooperationId1(paidyTokenId);
					if (PaidyUtility.IsTokenIdExist(paidyTokenId))
					{
						this.WsErrorMessagePayment.InnerHtml = WebMessages.GetMessages(
							WebMessages.ERRMSG_FRONT_PAIDY_TOKEN_ID_EXISTED_ERROR).Replace("@@ 1 @@", paidyTokenId);
						return;
					}

					var userCreditCard = PaidyUtility.RegisterAsUserCreditCard(
							this.LoginUserId,
							paidyTokenId,
							Constants.FLG_LASTCHANGED_USER,
							UpdateHistoryAction.Insert);
					orderNew.CreditBranchNo = userCreditCard.BranchNo;
				}
				else
				{
					this.WsErrorMessagePayment.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PAIDY_GET_TOKEN_ERROR);
					return;
				}
			}
		}
		else if ((Constants.PAYMENT_PAIDY_KBN == Constants.PaymentPaidyKbn.Paygent)
			&& (paymentModel.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY))
		{
			if (this.WhfPaidyStatus.Value == Constants.FLG_PAYMENT_PAIDY_API_STATUS_REJECTED)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] =
					WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_FAIL_PAIDY_AUTHORIZED);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}

			if (this.WhfPaidyStatus.Value == Constants.FLG_PAYMENT_PAIDY_API_STATUS_AUTHORIZED)
			{
				orderNew.PaymentOrderId = this.WhfPaidyPaymentId.Value;
				var paidyAuthorizationResult = new PaygentApiFacade().PaidyAuthorize(orderNew.PaymentOrderId);
				var success = paidyAuthorizationResult.IsSuccess;
				var paidyErrorMessage = string.Empty;

				if (success == false)
				{
					paidyErrorMessage = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_FAIL_PAIDY_AUTHORIZED);
					this.WsErrorMessagePayment.InnerHtml = paidyErrorMessage;
					FileLogger.Write("PaidyError", paidyErrorMessage);
					return;
				}

				var dictionary = new Dictionary<string, string>
				{
					{ Constants.FIELD_ORDER_ORDER_ID, StringUtility.ToEmpty(orderNew.OrderId) },
					{ Constants.FIELD_ORDER_EXTERNAL_ORDER_ID, StringUtility.ToEmpty(orderNew.ExternalOrderId) },
				};

				PaymentFileLogger.WritePaymentLog(
					success,
					paymentModel.PaymentId,
					PaymentFileLogger.PaymentType.Paidy,
					PaymentFileLogger.PaymentProcessingType.ExecPayment,
					paidyErrorMessage,
					dictionary);
			}
		}
		else if ((paymentModel.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE))
		{
			if (string.IsNullOrEmpty(
				this.LoginUser.UserExtend.UserExtendDataValue[Constants.FLG_USEREXTEND_USREX_AFTEE_TOKEN_ID])
					&& (string.IsNullOrEmpty(this.WhfAfteeToken.Value) == false))
			{
				this.LoginUser
					.UserExtend.UserExtendDataValue[Constants.FLG_USEREXTEND_USREX_AFTEE_TOKEN_ID] = this.WhfAfteeToken.Value;
				new UserService().UpdateWithUserExtend(this.LoginUser, UpdateHistoryAction.Insert);
			}
			if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE)
			{
				ReloadOrderHistoryDetail(false);
			}

			if (string.IsNullOrEmpty(this.WhfAfteeTransactionId.Value)) return;
		}
		else if ((paymentModel.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE))
		{
			if (string.IsNullOrEmpty(
				this.LoginUser.UserExtend.UserExtendDataValue[Constants.FLG_USEREXTEND_USREX_ATONE_TOKEN_ID])
					&& (string.IsNullOrEmpty(this.WhfAtoneToken.Value) == false))
			{
				this.LoginUser
					.UserExtend.UserExtendDataValue[Constants.FLG_USEREXTEND_USREX_ATONE_TOKEN_ID] = this.WhfAtoneToken.Value;
				new UserService().UpdateWithUserExtend(this.LoginUser, UpdateHistoryAction.Insert);
			}
			if (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)
			{
				ReloadOrderHistoryDetail(false);
			}

			if (string.IsNullOrEmpty(this.WhfAtoneTransactionId.Value)) return;
		}
		else if (paymentModel.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
		{
			// Check Country Iso Code Can Order NP After Pay
			if (Constants.GLOBAL_OPTION_ENABLE
				&& (orderNew.Shippings.Any(shipping => (IsCountryJp(shipping.ShippingCountryIsoCode) == false))
					|| (IsCountryJp(orderNew.Owner.OwnerAddrCountryIsoCode) == false)))
			{
				this.WsErrorMessagePayment.InnerHtml = NPAfterPayUtility.GetErrorMessages(Constants.FLG_PAYMENT_NP_AFTERPAY_CUSTOM_ERROR_CODE_3);
				return;
			}
		}
		else
		{
			orderNew.CreditBranchNo = null;
			orderNew.CardInstallmentsCode = "";
			orderNew.CardInstruments = "";
			orderNew.CardKbn = "";
		}

		// 消費税金額を計算
		OrderCommon.SetCalculateTax(orderNew);
		foreach (var orderNewOrderPriceByTaxRate in orderNew.OrderPriceByTaxRates)
		{
			orderNewOrderPriceByTaxRate.OrderId = orderNew.OrderId;
		}

		// AmazonPay向け
		if (paymentModel.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT)
		{
			// セッションにエラーメッセージがあればエラー
			var amazonShippingAddressErrorMesage = (string)Session[AmazonConstants.SESSION_KEY_AMAZON_ADDRESS_ERROR_MSG];

			if (string.IsNullOrEmpty(amazonShippingAddressErrorMesage) == false) return;

			var whfAmazonOrderRefID = GetWrappedControl<WrappedHiddenField>(this.AmazonPayRepeaterItem, "hfAmazonOrderRefID");
			var newOrderReferenceId = whfAmazonOrderRefID.Value;

			if (string.IsNullOrEmpty(newOrderReferenceId))
			{
				this.WsErrorMessagePayment.InnerHtml =
					CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_USER_LOGIN_FOR_AMAZON);
				return;
			}

			orderNew.PaymentOrderId = newOrderReferenceId;

			var set = AmazonApiFacade.SetOrderReferenceDetails(newOrderReferenceId, orderNew.LastBilledAmount, orderNew.OrderId);

			var setResult = set.GetSuccess();

			// 外部決済連携ログ格納処理
			OrderCommon.AppendExternalPaymentCooperationLog(
				setResult,
				orderOld.OrderId,
				setResult ? LogCreator.CreateMessage(orderOld.OrderId, "") : LogCreator.CreateErrorMessage(set.GetErrorCode(), set.GetErrorMessage()),
				orderNew.LastChanged,
				UpdateHistoryAction.Insert);
			if (setResult == false)
			{
				return;
			}

			if (set.GetConstraintIdList().Any())
			{
				var messages = set.GetConstraintIdList().Select(
					constraintId => AmazonApiMessageManager.GetOrderReferenceConstraintMessage(constraintId));
				this.WsErrorMessagePayment.InnerHtml = WebSanitizer.HtmlEncodeChangeToBr(string.Join("\r\n", messages));
				return;
			}

			var con = AmazonApiFacade.ConfirmOrderReference(newOrderReferenceId);
			var conResult = con.GetSuccess();
			OrderCommon.AppendExternalPaymentCooperationLog(
				conResult,
				orderOld.OrderId,
				conResult
					? string.Format("hfAmazonOrderRefID:{0}", newOrderReferenceId)
					: LogCreator.CreateErrorMessage(con.GetErrorCode(), con.GetErrorMessage()),
				orderNew.LastChanged,
				UpdateHistoryAction.Insert);
			if (con.GetSuccess() == false)
			{
				this.WsErrorMessagePayment.InnerHtml = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_AUTH_EXCEPTION);
				return;
			}

			// ウィジェットから住所情報取得
			var amazonModel = (AmazonModel)Session[AmazonConstants.SESSION_KEY_AMAZON_MODEL];
			var token = amazonModel.Token;
			var res = AmazonApiFacade.GetOrderReferenceDetails(newOrderReferenceId, token);
			var input = new AmazonAddressInput(res);
			var address = AmazonAddressParser.Parse(input);

			using (var accessor = new SqlAccessor())
			{
				// トランザクション開始
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 配送先1番目をクローンし、住所情報を上書き
				var shipping = ((OrderShippingModel[])orderOld.Shippings.Clone())[0];
				shipping.ShippingName1 = address.Name1;
				shipping.ShippingName2 = address.Name2;
				shipping.ShippingName = address.Name;
				shipping.ShippingNameKana1 = address.NameKana1;
				shipping.ShippingNameKana2 = address.NameKana2;
				shipping.ShippingNameKana = address.NameKana;
				shipping.ShippingZip = address.Zip;
				shipping.ShippingAddr1 = address.Addr1;
				shipping.ShippingAddr2 = address.Addr2;
				shipping.ShippingAddr3 = address.Addr3;
				shipping.ShippingAddr4 = address.Addr4;
				shipping.ShippingTel1 = address.Tel1 + "-" + address.Tel2 + "-" + address.Tel3;

				// 配送先更新
				var updateShippingCount = new OrderService().UpdateShippingForModify(
					new[] { shipping },
					orderNew.LastChanged,
					UpdateHistoryAction.DoNotInsert,
					accessor);
				if (updateShippingCount == 0)
				{
					this.WsErrorMessagePayment.InnerHtml = WebSanitizer.HtmlEncodeChangeToBr(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_ORDER_INSERT_ERROR));
					return;
				}
				accessor.CommitTransaction();
			}
		}
		// LINE Pay以外→LINE Pay
		else if ((paymentModel.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY)
			&& (orderOld.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY))
		{
			orderNew.IsNeedConfirmLinePayPayment = true;
			orderNew.PaymentOrderId = OrderCommon.CreatePaymentOrderId(orderNew.ShopId);
		}
		else
		{
			switch (paymentModel.PaymentId)
			{
				case Constants.FLG_PAYMENT_PAYMENT_ID_ATONE:
					orderNew.CardTranId = this.WhfAtoneTransactionId.Value;
					orderNew.IsUpdateAtonePaymentFromMyPage = true;
					break;

				case Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE:
					orderNew.CardTranId = this.WhfAfteeTransactionId.Value;
					orderNew.IsUpdateAfteePaymentFromMyPage = true;
					break;
			}
			if (paymentModel.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU)
			{
				orderNew.IsUpdateBokuPaymentFromMyPage = true;
			}
		}

		// 決済種別金額範囲チェック
		var errorMessage = OrderCommon.CheckPaymentPriceEnabledForOrder(orderOld, paymentModel.PaymentId);
		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			this.WsErrorMessagePayment.InnerHtml = errorMessage;
			return;
		}

		// クレジットカード売上確定後キャンセル可能かできるか？
		var isCancelable = OrderCommon.IsCancelableForCreditCardSalesCompleteOrder(orderOld);
		// エラーの場合、エラーメッセージセット
		if (isCancelable == false)
		{
			this.WsErrorMessagePayment.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_CREDIT_SALES_COMPLETE_CANCEL_ERROR);
			return;
		}

		// Paypay settlement execution
		if ((paymentModel.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
			&& (orderOld.OrderPaymentKbn != paymentModel.PaymentId))
		{
			switch (Constants.PAYMENT_PAYPAY_KBN)
			{
				case Constants.PaymentPayPayKbn.SBPS:
					var orderPaypay = SBPSSessionWrapper.FindSBPSMultiPendingOrder(this.OrderModel.OrderId);
					if ((orderPaypay == null)
						|| (Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_PAYPAY_SBPS] != null))
					{
						orderNew.PaymentOrderId = OrderCommon.CreatePaymentOrderId(orderOld.ShopId);

						// Go to Paypay payment screen
						SBPSSessionWrapper.AddSbpsMultiPendingOrder(orderNew);
						SessionSecurityManager.SaveSesstionContetnsToDatabaseForGoToOtherSite(this.Session, orderNew.OrderId);

						var orderSbps = new List<KeyValuePair<string, Hashtable>>
						{
							new KeyValuePair<string, Hashtable>(orderOld.OrderId, orderOld.DataSource)
						};

						var param = new Hashtable
						{
							{ "order_sbps_multi", orderSbps },
							{ "googleanaytics_params", new List<Hashtable>() },
							{ "order", new List<Hashtable>() },
							{ "order_linepay", new List<KeyValuePair<string, Hashtable>>() },
							{ "zeus_order_3dsecure", new List<Hashtable>() },
							{ "rakuten_order_3dsecure", new List<Hashtable>() },
						};

						Session[Constants.SESSION_KEY_CART_LIST] = null;
						Session[Constants.SESSION_KEY_LANDING_CART_SESSION_KEY] = null;
						Session[Constants.SESSION_KEY_PARAM] = param;
						Session[Constants.SESSION_KEY_ORDER_ID_CHECK_FOR_LINK_TYPE_PAYMENT] = string.Empty;
						Session[Constants.SESSION_KEY_CART_LIST_FOR_PAYPAY] = CartObjectList.CreateCartObjectListByOrder(
							orderNew.UserId,
							orderNew.OrderKbn,
							new OrderModel[] { orderNew });
						Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_PAYPAY_SBPS] = Constants.PAGE_FRONT_PAYMENT_PAYPAY_SBPS_RECEIVE;

						var url = new UrlCreator(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_PAYMENT_SBPS_MULTIPAYMENT_POST_ORDER)
							.AddParam(Constants.REQUEST_KEY_ORDER_ID, orderOld.OrderId)
							.CreateUrl();
						Response.Redirect(url);
					}
					else
					{
						orderNew = SBPSSessionWrapper.FindSBPSMultiPendingOrder(this.OrderModel.OrderId);
						SBPSSessionWrapper.RemoveSbpsMultiPendingOrder(this.OrderModel.OrderId);
					}
					break;

				case Constants.PaymentPayPayKbn.GMO:
					if (GmoSessionWrapper.FindGmoMultiPendingOrder(this.OrderModel.OrderId) == null)
					{
						orderNew.PaymentOrderId = OrderCommon.CreatePaymentOrderId(orderOld.ShopId);
						var result = new PaypayGmoFacade().ExecPayment(null, orderNew);

						if (string.IsNullOrEmpty(result.ErrorMessage) == false)
						{
							this.WsErrorMessagePayment.InnerHtml = WebSanitizer.HtmlEncodeChangeToBr(
								WebMessages.GetMessages(
									WebMessages.ERRMSG_SYSTEM_ERROR,
									result.ErrorMessage));
							return;
						}

						// Go to Paypay payment screen
						GmoSessionWrapper.AddGmoMultiPendingOrder(orderNew);
						SessionSecurityManager.SaveSesstionContetnsToDatabaseForGoToOtherSite(this.Session, orderNew.OrderId);
						Response.Redirect(result.RedirectUrl);
					}
					else
					{
						orderNew = GmoSessionWrapper.FindGmoMultiPendingOrder(this.OrderModel.OrderId);
						GmoSessionWrapper.RemoveGmoMultiPendingOrder(this.OrderModel.OrderId);
					}
					break;
			}
		}

		//決済金額決定
		orderNew.SettlementCurrency = CurrencyManager.GetSettlementCurrency(orderNew.OrderPaymentKbn);
		orderNew.SettlementRate = CurrencyManager.GetSettlementRate(orderNew.SettlementCurrency);
		orderNew.SettlementAmount = CurrencyManager.GetSettlementAmount(
			orderOld.OrderId,
			orderNew.OrderPaymentKbn,
			orderNew.OrderPriceTotal,
			orderNew.SettlementRate,
			orderNew.SettlementCurrency);

		// 請求書同梱フラグ判定
		orderNew.InvoiceBundleFlg = JudgementInvoiceBundleFlg(paymentModel.PaymentId, orderNew);

		// If Payment not EcPay set external payment type is empty
		if ((paymentModel.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
			|| (paymentModel.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY))
		{
			orderNew.ExternalPaymentType = string.Empty;
		}

		// ゼウスリンク式決済で新規クレカの場合は画面遷移
		var wcbRegistCreditCard = GetWrappedControl<WrappedCheckBox>(this.CreditRepeaterItem, "cbRegistCreditCard");
		if (this.IsCreditCardLinkPayment() && IsNewCreditCard())
		{
			Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_PAYMENT_ZEUS_LINK_POINT_GET_NOTICE;

			var wcbUpdateFixedPurchase = GetWrappedControl<WrappedCheckBox>("cbIsUpdateFixedPurchaseByOrderPayment");

			var param = new Hashtable
			{
				{"order_new", orderNew},
				{"is_card_register", ((wcbRegistCreditCard.InnerControl != null) && wcbRegistCreditCard.Checked)},
				{"is_update_fixedpurchase", ((wcbUpdateFixedPurchase.InnerControl != null)
					&& wcbUpdateFixedPurchase.Checked
					&& (string.IsNullOrEmpty(this.OrderModel.FixedPurchaseId) == false))},
			};
			Session[Constants.SESSION_KEY_PARAM] = param;

			var url =
				new UrlCreator(
					this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_PAYMENT_ZEUS_LINK_POINT_POST)
					.AddParam(Constants.REQUEST_KEY_PAYMENTCREDITCARD_ACTION_TYPE, Constants.ActionTypes.ChangeOrderCreditCard.ToString())
					.AddParam(Constants.REQUEST_KEY_ORDER_ID, orderOld.OrderId)
				.CreateUrl();
			Response.Redirect(url);
		}
		// LINE Pay以外→LINE Payの場合は画面遷移
		if ((paymentModel.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY)
			&& (orderOld.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY))
		{
			//Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_PAYMENT_ZEUS_LINK_POINT_GET_NOTICE;
			Session[Constants.SESSION_KEY_PARAM] = new Hashtable
			{
				{"order_new", orderNew},
			};

			var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_PAYMENT_LINEPAY_RECEIVE)
				.AddParam("action", LinePayUtility.API_CALLBACK_REQUEST_FOR_MODIFY)
				.AddParam(Constants.REQUEST_KEY_ORDER_ID, orderOld.OrderId)
				.AddParam("error", "1")
				.AddParam("next", "orderHistory").CreateUrl();
			Response.Redirect(url);

		}

		if (OrderCommon.CheckPaymentYamatoKaSms(orderNew.OrderPaymentKbn)
			&& (bool.Parse(whfShowModal.Value) == false))
		{
			Session[Constants.SESSION_KEY_PARAM] = new Hashtable
			{
				{"order_new", orderNew},
			};
			whfShowModal.Value = "true";
			this.OnAuthorizeCompleteMethodName = MethodBase.GetCurrentMethod().Name;
			return;
		}
		else if (Session[Constants.SESSION_KEY_PAYMENT_ORDER_ID] != null)
		{
			orderNew.CardTranId = (string)Session[Constants.SESSION_KEY_PAYMENT_ORDER_ID];
			orderNew.PaymentOrderId = (string)Session[Constants.SESSION_KEY_PAYMENT_ORDER_ID];
			Session.Remove(Constants.SESSION_KEY_PAYMENT_ORDER_ID);

			orderNew.CreditBranchNo = OrderPreorderRegister.RegisterTelNumForYamatoKaSmsAsUserCreditCard(
				orderNew.UserId,
				(string)Session[Constants.SESSION_KEY_SMS_TEL_NUMBER],
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.DoNotInsert).BranchNo;
			new OrderService().UpdateCreditBranchNo(
				orderNew.OrderId,
				orderNew.CreditBranchNo.Value,
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.DoNotInsert);

			whfShowModal.Value = "false";
		}

		// AmazonPayCv2
		if (paymentModel.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2)
		{
			Session[Constants.SESSION_KEY_PARAM] = new Hashtable
			{
				{"order_new", orderNew},
			};

			var checkoutSessionId = this.AmazonCheckoutSession != null
				? this.AmazonCheckoutSession.CheckoutSessionId
				: String.Empty;

			if (string.IsNullOrEmpty(checkoutSessionId))
			{
				this.WsErrorMessagePayment.InnerHtml = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_USER_LOGIN_FOR_AMAZON);
				return;
			}

			var addressInput = new AmazonAddressInput(this.AmazonCheckoutSession.ShippingAddress);
			var address = AmazonAddressParser.Parse(addressInput);

			// 配送先1番目をクローンし、住所情報を上書き
			var shipping = ((OrderShippingModel[])this.OrderModel.Shippings.Clone())[0];
			shipping.ShippingName1 = address.Name1;
			shipping.ShippingName2 = address.Name2;
			shipping.ShippingName = address.Name;
			shipping.ShippingNameKana1 = address.NameKana1;
			shipping.ShippingNameKana2 = address.NameKana2;
			shipping.ShippingNameKana = address.NameKana;
			shipping.ShippingZip = address.Zip;
			shipping.ShippingAddr1 = address.Addr1;
			shipping.ShippingAddr2 = address.Addr2;
			shipping.ShippingAddr3 = address.Addr3;
			shipping.ShippingAddr4 = address.Addr4;
			shipping.ShippingTel1 = address.Tel1 + "-" + address.Tel2 + "-" + address.Tel3;

			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				// 配送先更新
				var updateShippingCount = new OrderService().UpdateShippingForModify(
					new[] { shipping },
					orderNew.LastChanged,
					UpdateHistoryAction.DoNotInsert,
					accessor);

				if (updateShippingCount == 0)
				{
					this.WsErrorMessagePayment.InnerHtml = WebSanitizer.HtmlEncodeChangeToBr(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_ORDER_INSERT_ERROR));
					return;
				}

				var orderPaymentStatus = Constants.PAYMENT_AMAZON_PAYMENTSTATUSCOMPLETE
					? Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE
					: Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM;

				new OrderService().UpdatePaymentStatus(
					orderNew.OrderId,
					orderPaymentStatus,
					orderNew.OrderPaymentDate,
					orderNew.LastChanged,
					UpdateHistoryAction.DoNotInsert,
					accessor);
			}

			if (Constants.PAYMENT_AMAZON_PAYMENTCAPTURENOW)
			{
				orderNew.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP;
			}
			else
			{
				orderNew.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
			}

			var oldCaptured = (orderOld.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED);

			var amazonCallbackPath = AmazonCv2ApiFacade.CreateCallbackPathForOneTime(
				AmazonCv2Constants.AMAZON_ACTION_STATUS_AUTH,
				this.OrderModel.OrderId);

			var checkoutSession =
				this.AmazonFacade.UpdateCheckoutSession(
					checkoutSessionId,
					amazonCallbackPath,
					orderNew.LastBilledAmount,
					orderNew.OrderId,
					oldCaptured);
			var redirectUrl = checkoutSession.WebCheckoutDetails.AmazonPayRedirectUrl;
			Response.Redirect(redirectUrl);
		}

		// Boku
		if (paymentModel.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU)
		{
			var forwardUrl = new UrlCreator(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_HISTORY_DETAIL)
				.AddParam(Constants.REQUEST_KEY_ORDER_ID, orderNew.OrderId)
				.AddParam("action", "boku")
				.CreateUrl();
			var optinResponse = new PaymentBokuOptinApi().Exec(
				this.CountryIsoCode,
				forwardUrl);

			if (optinResponse == null)
			{
				this.WsErrorMessagePayment.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_BOKU_PAYMENT_ERROR);
				AppLogger.WriteError(
					string.Format(
						"{0} Payment Error: Order ID ={1}",
						BokuConstants.CONST_BOKU_PAYMENT_METHOD_CARRIERBILLING,
						this.OrderModel.OrderId));
				return;
			}
			else if (optinResponse.IsSuccess == false)
			{
				this.WsErrorMessagePayment.InnerHtml = optinResponse.Result.Message;
				return;
			}

			orderNew.PaymentOrderId = optinResponse.OptinId;
			Session[Constants.SESSION_KEY_PARAM] = new Hashtable
			{
				{ "order_new", orderNew },
				{ "is_update_fixed_purchase", WcbIsUpdateFixedPurchaseByOrderPayment.Checked }
			};
			Response.Redirect(optinResponse.Hosted.OptinUrl);
		}

		if ((Constants.PAYMENT_PAIDY_KBN == Constants.PaymentPaidyKbn.Paygent)
			&& (orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY))
		{
			orderNew.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
			orderNew.OnlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_NONE;
		}

		// 再与信実施＆画面リロード
		var reauthResult = ExecuteChangeOrderForPayment(orderOld, orderNew);
		if (reauthResult) ReloadOrderHistoryDetail(false);
	}

	/// <summary>
	/// 決済情報変更向け注文変更
	/// </summary>
	/// <param name="orderOld">旧注文</param>
	/// <param name="orderNew">新注文</param>
	/// <param name="isUpdateFixedPurchaseBoku">Is update fixed purchase Boku</param>
	/// <returns>再与信結果</returns>
	private bool ExecuteChangeOrderForPayment(
		OrderModel orderOld,
		OrderModel orderNew,
		bool isUpdateFixedPurchaseBoku = false)
	{
		var wcbRegistCreditCard = GetWrappedControl<WrappedCheckBox>(this.CreditRepeaterItem, "cbRegistCreditCard");

		var isCardRegister =
			(IsNewCreditCard() && (wcbRegistCreditCard.InnerControl != null) && wcbRegistCreditCard.Checked);
		var wcbIsUpdateFixedPurchaseByOrderPayment =
			GetWrappedControl<WrappedCheckBox>("cbIsUpdateFixedPurchaseByOrderPayment");
		var isUpdateFixedPurchase = (wcbIsUpdateFixedPurchaseByOrderPayment.InnerControl != null
			&& wcbIsUpdateFixedPurchaseByOrderPayment.Checked
			&& string.IsNullOrEmpty(this.OrderModel.FixedPurchaseId) == false);

		if (isUpdateFixedPurchaseBoku) isUpdateFixedPurchase = true;

		// 注文変更処理
		string errorMessage;
		string externalPaymentApiErrorMassage;
		ExecuteChangeOrder(
			orderOld,
			orderNew,
			orderNew.OrderPaymentKbn,
			isCardRegister,
			isUpdateFixedPurchase,
			out errorMessage,
			out externalPaymentApiErrorMassage);

		if (string.IsNullOrEmpty(externalPaymentApiErrorMassage) == false)
		{
			externalPaymentApiErrorMassage =
				((orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
					&& externalPaymentApiErrorMassage.Contains(
						NPAfterPayUtility.GetErrorMessages(Constants.FLG_PAYMENT_NP_AFTERPAY_CUSTOM_ERROR_CODE_2)))
				? CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_AUTH_ERROR)
				: externalPaymentApiErrorMassage;
			this.WsErrorMessagePayment.InnerHtml = externalPaymentApiErrorMassage;
			return false;
		}

		return true;
	}

	/// <summary>
	/// 利用ポイント変更ボタン クリックイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbDisplayInputOrderPointUse_Click(object sender, EventArgs e)
	{
		this.IsOrderPointAddDisplayStatus = (this.IsOrderPointAddDisplayStatus == false);
		this.WlbDisplayInputOrderPointUse.Visible = false;
		this.WlsErrorMessagePointUse.InnerHtml = "";
	}

	/// <summary>
	/// 利用ポイント更新ボタンのクリックイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbUpdateOrderPointUse_Click(object sender, EventArgs e)
	{
		// ポイント入力チェック (check point input)
		this.ErrorMessageOrderPointUse = Validator.Validate(
			"OrderModifyOrderInput",
			new Hashtable
			{
				{ "order_point_use", this.WtbOrderPointUse.Text.Trim()}
			});

		if (string.IsNullOrEmpty(this.ErrorMessageOrderPointUse))
		{
			// 注文情報（変更前）取得
			var orderService = new OrderService();
			var orderOld = orderService.Get(this.OrderModel.OrderId);

			//ポイントチェック
			var orderPointUse = decimal.Parse(this.WtbOrderPointUse.Text);
			if (orderPointUse == orderOld.OrderPointUse) ReloadOrderHistoryDetail();
			if (orderPointUse > (this.LoginUserPointUsable + orderOld.OrderPointUse))
			{
				this.ErrorMessageOrderPointUse =
					WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_POINT_USE_MAX_ERROR)
						.Replace("@@ 1 @@", GetNumeric(this.LoginUserPointUsable) + Constants.CONST_UNIT_POINT_PT);
			}

			// クレジットカード売上確定後キャンセル可能かできるか？
			var isCancelable = OrderCommon.IsCancelableForCreditCardSalesCompleteOrder(orderOld);
			// エラーの場合、エラーメッセージセット
			if (isCancelable == false)
			{
				this.ErrorMessageOrderPointUse = WebMessages.GetMessages(WebMessages.ERRMSG_CREDIT_SALES_COMPLETE_CANCEL_ERROR);
			}

			if (orderOld.LastOrderPointUse > orderPointUse)
			{
				switch (orderOld.OrderPaymentKbn)
				{
					case Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY:
						if (orderOld.ExternalPaymentType == Constants.FLG_PAYMENT_TYPE_ECPAY_CREDIT)
						{
							this.ErrorMessageOrderPointUse
								= WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CHANGE_POINT_USE_FOR_ECPAY);
						}
						break;

					case Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY:
						if (orderOld.ExternalPaymentType == Constants.FLG_PAYMENT_TYPE_NEWEBPAY_CREDIT)
						{
							this.ErrorMessageOrderPointUse
								= WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CHANGE_POINT_USE_FOR_NEWEBPAY);
						}
						break;

					case Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY:
						this.ErrorMessageOrderPointUse
							= WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CHANGE_POINT_USE_FOR_PAYPAY);
						break;
				}
			}

			if (string.IsNullOrEmpty(this.ErrorMessageOrderPointUse))
			{
				// 注文情報（変更後）取得
				var orderNew = (OrderModel)orderOld.Clone();
				orderNew.OrderPointUse = orderPointUse;
				orderNew.LastOrderPointUse = orderNew.OrderPointUse;
				var pointService = new PointService();
				orderNew.OrderPointUseYen = pointService.GetOrderPointUsePrice(orderNew.OrderPointUse, Constants.FLG_POINT_POINT_KBN_BASE);
				orderNew.LastOrderPointUseYen = pointService.GetOrderPointUsePrice(orderNew.LastOrderPointUse, Constants.FLG_POINT_POINT_KBN_BASE);
				orderNew.DeviceInfo = this.Request[Constants.REQUEST_GMO_DEFERRED_DEVICE_INFO];

				// 初回購入か
				var isFirstOrder = DomainFacade.Instance.OrderService.CheckOrderFirstBuy(orderNew.UserId, orderNew.OrderId);
				// 初回購入発行ポイント
				var pointFirstBuy = isFirstOrder
					? PointOptionUtility.GetOrderPointAddForOrder(orderNew, Constants.FLG_POINTRULE_POINT_INC_KBN_FIRST_BUY)
					: 0m;
				// 購入時発行ポイント
				var pointOrder = PointOptionUtility.GetOrderPointAddForOrder(orderNew, Constants.FLG_POINTRULE_POINT_INC_KBN_BUY);
				// 付与ポイント
				orderNew.OrderPointAdd = pointFirstBuy + pointOrder;

				// 消費税金額を計算
				OrderCommon.SetCalculateTax(orderNew);
				foreach (var orderNewOrderPriceByTaxRate in orderNew.OrderPriceByTaxRates)
				{
					orderNewOrderPriceByTaxRate.OrderId = orderNew.OrderId;
				}

				// （ポイント変更向け）再計算処理
				if (CalculatePriceForOrderPointUse(orderOld, orderNew))
				{
					// 決済なしに変更が可能かどうか
					if (IsChangeNoPaymentEnable(orderNew))
					{
						// 決済金額決定
						orderNew.SettlementCurrency = CurrencyManager.GetSettlementCurrency(orderNew.OrderPaymentKbn);
						orderNew.SettlementRate = CurrencyManager.GetSettlementRate(orderNew.SettlementCurrency);
						orderNew.SettlementAmount = CurrencyManager.GetSettlementAmount(
							orderOld.OrderId,
							orderNew.OrderPaymentKbn,
							orderNew.OrderPriceTotal,
							orderNew.SettlementRate,
							orderNew.SettlementCurrency);

						// 再与信＆更新（更新履歴とともに）
						var authErrorMessage =
							ReauthAndUpdateOrderForUpdatePointUse(
								orderOld,
								orderNew,
								isFirstOrder,
								pointFirstBuy,
								pointOrder,
								UpdateHistoryAction.Insert);

						var hasAuthApiErrorMessage = (string.IsNullOrEmpty(this.ApiErrorMessage) == false);

						OrderCommon.AppendExternalPaymentCooperationLog(
							hasAuthApiErrorMessage == false,
							orderOld.OrderId,
							hasAuthApiErrorMessage ? this.ApiErrorMessage : LogCreator.CreateMessage(orderOld.OrderId, ""),
							Constants.FLG_LASTCHANGED_USER,
							UpdateHistoryAction.Insert);

						// 再与信失敗エラー表示
						if (string.IsNullOrEmpty(authErrorMessage) == false)
						{
							this.ErrorMessageOrderPointUse = authErrorMessage;
						}
					}
					else
					{
						this.ErrorMessageOrderPointUse = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PAYMENT_USEABLE_PRICE_OVER_UNSELECTABLE_ERROR)
							.Replace("@@ 1 @@", CurrencyManager.ToPrice((this.OrderModel.OrderPriceSubtotal - orderPointUse)));
					}

					if (Constants.CROSS_POINT_OPTION_ENABLED)
					{
						// CROSSPOINT側の変更
						var discount = (orderNew.MemberRankDiscountPrice
							+ orderNew.OrderCouponUse
							+ orderNew.SetpromotionProductDiscountAmount
							+ orderNew.FixedPurchaseDiscountPrice
							+ orderNew.FixedPurchaseMemberDiscountAmount
							- orderNew.OrderPriceRegulation);

						var priceTaxIncluded = TaxCalculationUtility.GetPriceTaxIncluded(
							orderNew.OrderPriceSubtotal,
							orderNew.OrderPriceSubtotalTax);

						var input = new PointApiInput
						{
							MemberId = orderOld.UserId,
							OrderDate = orderOld.OrderDate,
							PosNo = w2.App.Common.Constants.CROSS_POINT_POS_NO,
							OrderId = orderOld.OrderId,
							BaseGrantPoint = orderNew.OrderPointAdd,
							SpecialGrantPoint = 0m,
							PriceTotalInTax = (orderNew.OrderPriceSubtotal - discount),
							PriceTotalNoTax = (priceTaxIncluded - discount),
							UsePoint = orderNew.LastOrderPointUse,
							Items = CartObject.GetOrderDetails(orderNew),
							ReasonId = CrossPointUtility.GetValue(Constants.CROSS_POINT_SETTING_ELEMENT_REASON_ID, w2.App.Common.Constants.CROSS_POINT_REASON_KBN_OPERATOR),
						};
						var result = new CrossPointPointApiService().Modify(input.GetParam(PointApiInput.RequestType.Modify));

						this.ErrorMessageOrderPointUse = (result.IsSuccess == false)
							? string.Format(
								"{0}{1}{2}",
								CommerceMessages.GetMessages(CommerceMessages.ERRMSG_SYSTEM_ERROR),
								Environment.NewLine,
								result.ErrorCodeList)
							: string.Empty;
					}
				}
			}
		}

		if (string.IsNullOrEmpty(this.ErrorMessageOrderPointUse) == false)
		{
			this.WlsErrorMessagePointUse.InnerHtml = this.ErrorMessageOrderPointUse;
			this.IsOrderPointAddDisplayStatus = true;
			return;
		}

		// メール送信
		SendMailCommon.SendModifyPurchaseHistoryMail(this.OrderModel.OrderId, SendMailCommon.PurchaseHistoryModify.OrderPoint);

		// ユーザポイントが変更されたため、最新のユーザポイントを取得
		this.LoginUserPoint = PointOptionUtility.GetUserPoint(this.LoginUserId);

		// Reload page
		ReloadOrderHistoryDetail();
	}

	/// <summary>
	/// 利用ポイント変更 キャンセルボタンのクリックイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbHideOrderPointUse_Click(object sender, EventArgs e)
	{
		this.IsOrderPointAddDisplayStatus = (this.IsOrderPointAddDisplayStatus == false);
		this.WlbDisplayInputOrderPointUse.Visible = true;
	}

	/// <summary>
	/// お届け先変更ボタンのクリックイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbDisplayUserShippingInfoForm_Click(object sender, EventArgs e)
	{
		var index = int.Parse(((LinkButton)sender).CommandArgument);
		var wdvShippingInfo = GetWrappedControl<WrappedHtmlGenericControl>(this.WrOrderShipping.Items[index], "dShippingInfo");
		var wdvShippngInput = GetWrappedControl<WrappedHtmlGenericControl>(this.WrOrderShipping.Items[index], "dShippngInput");
		var wsErrorMessageShipping = GetWrappedControl<WrappedHtmlGenericControl>(this.WrOrderShipping.Items[index], "sErrorMessageShipping");
		wdvShippingInfo.Visible = (wdvShippingInfo.Visible == false);
		wdvShippngInput.Visible = (wdvShippngInput.Visible == false);
		wsErrorMessageShipping.InnerHtml = string.Empty;

		var wdvShippngInputFormInner = GetWrappedControl<WrappedHtmlGenericControl>(this.WrOrderShipping.Items[index], "divShippingInputFormInner");
		var wdvShippngInputFormConvenience = GetWrappedControl<WrappedHtmlGenericControl>(this.WrOrderShipping.Items[index], "divConvenienceStore");

		if (wdvShippngInput.Visible)
		{
			var wtbShippingName1 = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingName1");
			var wtbShippingName2 = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingName2");
			var wtbShippingNameKana1 = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingNameKana1");
			var wtbShippingNameKana2 = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingNameKana2");
			var wddlShippingCountry = GetWrappedControl<WrappedDropDownList>(this.WrOrderShipping.Items[index], "ddlShippingCountry");
			var wtbShippingZipGlobal = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingZipGlobal");
			var wtbShippingZip1 = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingZip1");
			var wtbShippingZip2 = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingZip2");
			var wddlShippingAddr1 = GetWrappedControl<WrappedDropDownList>(this.WrOrderShipping.Items[index], "ddlShippingAddr1");
			var wtbShippingAddr2 = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingAddr2");
			var wtbShippingAddr3 = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingAddr3");
			var wtbShippingAddr4 = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingAddr4");
			var wddlShippingAddr5 = GetWrappedControl<WrappedDropDownList>(this.WrOrderShipping.Items[index], "ddlShippingAddr5");
			var wtbShippingAddr5 = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingAddr5");
			var wtbShippingCompanyName = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingCompanyName");
			var wtbShippingCompanyPostName = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingCompanyPostName");
			var wtbShippingTel1 = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingTel1_1");
			var wtbShippingTel2 = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingTel1_2");
			var wtbShippingTel3 = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingTel1_3");
			var wtbShippingTel1Global = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingTel1Global");
			var wtbShippingZip = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingZip");
			var wtbShippingTel = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingTel1");

			// Load value
			if (this.OrderShippingItems[index] != null)
			{
				var shipping = new OrderShippingModel((DataRowView)this.OrderShippingItems[index]["row"]);

				wtbShippingName1.Text = shipping.ShippingName1;
				wtbShippingName2.Text = shipping.ShippingName2;

				wtbShippingNameKana1.Text = shipping.ShippingNameKana1;
				wtbShippingNameKana2.Text = shipping.ShippingNameKana2;

				foreach (ListItem item in wddlShippingCountry.Items)
				{
					item.Selected = (item.Value == shipping.ShippingCountryIsoCode);
				}

				foreach (ListItem item in wddlShippingAddr1.Items)
				{
					item.Selected = (item.Value == shipping.ShippingAddr1);
				}
				wtbShippingAddr2.Text = shipping.ShippingAddr2;
				wtbShippingAddr3.Text = shipping.ShippingAddr3;
				wtbShippingAddr4.Text = shipping.ShippingAddr4;

				if (IsCountryUs(shipping.ShippingCountryIsoCode))
				{
					wddlShippingAddr5.SelectedValue = shipping.ShippingAddr5;
				}
				else
				{
					wtbShippingAddr5.Text = shipping.ShippingAddr5;
				}

				wtbShippingCompanyName.Text = shipping.ShippingCompanyName;
				wtbShippingCompanyPostName.Text = shipping.ShippingCompanyPostName;

				var shippingAddrCountryIsoCode = wddlShippingCountry.SelectedValue;

				if (IsCountryJp(shippingAddrCountryIsoCode))
				{
					// Set value for telephone
					SetTelTextbox(
						wtbShippingTel,
						wtbShippingTel1,
						wtbShippingTel2,
						wtbShippingTel3,
						shipping.ShippingTel1);

					// Set value for zip code
					SetZipCodeTextbox(
						wtbShippingZip,
						wtbShippingZip1,
						wtbShippingZip2,
						shipping.ShippingZip);
				}
				else
				{
					wtbShippingZipGlobal.Text = shipping.ShippingZip;
					wtbShippingTel1Global.Text = shipping.ShippingTel1;
				}

				if (shipping.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
				{
					wdvShippngInputFormInner.Visible = false;
					wdvShippngInputFormConvenience.Visible = true;
				}
				else
				{
					wdvShippngInputFormInner.Visible = true;
					wdvShippngInputFormConvenience.Visible = false;
				}
			}

			// 国切替初期化
			ddlShippingCountry_SelectedIndexChanged(sender, e);
		}
	}

	/// <summary>
	/// 郵便番号検索ボタンクリック(TextBox用)
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSearchAddr_TextBox_Click(object sender, System.EventArgs e)
	{
		var index = int.Parse(((TextBox)sender).ValidationGroup);
		SearchAddr(index);
	}

	/// <summary>
	/// 郵便番号検索ボタンクリック(LinkButton用)
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSearchAddr_LinkButton_Click(object sender, System.EventArgs e)
	{
		var index = int.Parse(((LinkButton)sender).CommandArgument);
		SearchAddr(index);
	}

	/// <summary>
	/// 郵便番号検索処理
	/// </summary>
	/// <param name="index">リピータインデックス</param>
	protected void SearchAddr(int index)
	{
		var wtbShippingZip1 = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingZip1");
		var wtbShippingZip2 = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingZip2");
		var wddlShippingAddr1 = GetWrappedControl<WrappedDropDownList>(this.WrOrderShipping.Items[index], "ddlShippingAddr1");
		var wtbShippingAddr2 = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingAddr2");
		var wsShippingZipError = GetWrappedControl<WrappedHtmlGenericControl>(this.WrOrderShipping.Items[index], "sShippingZipError");
		var wtbShippingZip = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingZip");

		// 郵便番号入力チェック
		var zip = wtbShippingZip1.HasInnerControl
			? StringUtility.ToHankaku(wtbShippingZip1.Text.Trim() + wtbShippingZip2.Text.Trim())
			: StringUtility.ToHankaku(wtbShippingZip.Text.Trim());
		var errorMessages = new StringBuilder();
		errorMessages.Append(Validator.CheckZipCode(zip));

		wsShippingZipError.InnerHtml = "";

		// 入力チェックOKの場合、郵便番号検索実行＆セット
		if (errorMessages.Length == 0)
		{
			// 配送不可エリアエラーチェック
			var inputZip = wtbShippingZip1.HasInnerControl
			? wtbShippingZip1.Text.Trim() + wtbShippingZip2.Text.Trim()
			: wtbShippingZip.Text.Trim().Replace("-", "");

			if (CheckUnavailableShippingAreaForOrderHistoryDetail(index, inputZip))
			{
				errorMessages.Append(
					WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_ZIPCODE_UNAVAILABLE_SHIPPING_AREA));
			}

			errorMessages.Append(
				SearchAddrFromZip(wtbShippingZip1, wtbShippingZip2, wddlShippingAddr1, wtbShippingAddr2, wtbShippingZip));
		}

		// エラーメッセージ表示
		if (errorMessages.ToString() != "")
		{
			wsShippingZipError.InnerHtml = errorMessages.ToString();
			return;
		}

		//金額更新
		ChangeShippingPriceView(index);
	}

	/// <summary>
	/// 配送不可エリアエラーか（購入履歴）
	/// </summary>
	/// <param name="index">リピータインデックス</param>
	/// <param name="inputZip">郵便番号</param>
	/// <returns>配送不可エリアエラーならtrue</returns>
	private bool CheckUnavailableShippingAreaForOrderHistoryDetail(int index, string inputZip)
	{
		var order = new OrderService().Get(this.OrderModel.OrderId);
		var unavailableShippingZip = new ShopShippingService().GetUnavailableShippingZipFromShippingDelivery(
			order.ShippingId,
			order.Shippings[index].DeliveryCompanyId);

		return OrderCommon.CheckUnavailableShippingArea(unavailableShippingZip, inputZip);
	}

	/// <summary>
	/// お届け先 情報更新ボタンクリックイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbUpdateUserShippingInfo_Click(object sender, EventArgs e)
	{
		var index = int.Parse(((LinkButton)sender).CommandArgument);
		var wsErrorMessageShipping = GetWrappedControl<WrappedHtmlGenericControl>(this.WrOrderShipping.Items[index], "sErrorMessageShipping");
		var wlOrderHistoryErrorMessage = GetWrappedControl<WrappedLabel>(WrOrderShipping.Items[index], "lOrderHistoryErrorMessage");
		var wcbIsUpdateFixedPurchaseByOrderShippingInfo = GetWrappedControl<WrappedCheckBox>(
			this.WrOrderShipping.Items[index],
			"cbIsUpdateFixedPurchaseByOrderShippingInfo");

		var shippings = GetInputShippings(index);
		if (shippings[index].ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF)
		{
			var ht = shippings[index].DataSource;
			var tel = shippings[index].ShippingTel1.Split('-');
			if (tel.Length == 3)
			{
				ht.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1 + "_1", tel[0]);
				ht.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1 + "_2", tel[1]);
				ht.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1 + "_3", tel[2]);
			}

			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				ht.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_NAME + "_for_check", shippings[index].ShippingCountryName);
			}

			var inputForValidate = (Hashtable)ht.Clone();
			var wtbShippingTel = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingTel1_1");
			if (wtbShippingTel.HasInnerControl)
			{
				inputForValidate.Remove(Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1);
			}
			var validationName = IsCountryJp(shippings[index].ShippingCountryIsoCode) ? "OrderModifyOrderShippingInput" : "OrderModifyOrderShippingGlobal";
			var validationErrorMessages = Validator.ValidateAndGetErrorContainer(validationName, inputForValidate, Constants.GLOBAL_OPTION_ENABLE ? shippings[index].ShippingCountryIsoCode : "");

			if (validationErrorMessages.Count != 0)
			{
				// カスタムバリデータ取得
				var lCustomValidators = new List<CustomValidator>();
				CreateCustomValidators(this, lCustomValidators);

				// エラーをカスタムバリデータへ
				SetControlViewsForError(validationName, validationErrorMessages, lCustomValidators);
				return;
			}

			// 注文商品の税額を再計算
			shippings
				.ToList()
				.ForEach(shipping => shipping.Items
					.ToList()
					.ForEach(item => item.ItemPriceTax = TaxCalculationUtility.GetTaxPrice(
						item.ProductPrice,
						item.ProductTaxRate,
						shipping.ShippingCountryIsoCode,
						shipping.ShippingAddr5,
						Constants.TAX_EXCLUDED_FRACTION_ROUNDING) * item.ItemQuantity));
			var prefectures = new List<string>(Constants.STR_PREFECTURES_LIST);

			if (string.IsNullOrEmpty(this.WhfIsCheckFixedPurchaseFirstTime.Value))
			{
				this.WhfIsCheckFixedPurchaseFirstTime.Value = "true";

				if (Constants.PRODUCT_ORDER_LIMIT_ENABLED)
				{
					var productIdList = string.Format("'{0}'", string.Join("','", shippings.SelectMany(x => x.Items.Select(y => y.ProductId))));
					if (this.IsFixedPurchase)
					{
						this.SimilarShippingOrderIdList = shippings.SelectMany(
							shipping => new OrderService().GetOrderIdForFixedProductOrderLimitCheck(
								shipping,
							this.OrderModel,
							this.OrderModel.Owner,
								this.ShopId,
								productIdList,
								new[] { shipping.OrderId })).ToArray();
					}
					else
					{
						this.SimilarShippingOrderIdList = shippings.SelectMany(
							shipping => new OrderService().GetOrderIdForProductOrderLimitCheck(
								shipping,
							this.OrderModel,
							this.OrderModel.Owner,
								this.ShopId,
								productIdList,
								this.OrderModel.CombinedOrgOrderIds.Split(',')
									.Concat(new string[] { this.OrderModel.OrderId }).ToArray())).ToArray();
					}
					if (this.SimilarShippingOrderIdList.Length > 0)
					{
						this.WhfConfirmSenderId.Value = ((LinkButton)sender).UniqueID;
						return;
					}
				}
			}
		}
		else if (string.IsNullOrEmpty(shippings[index].ShippingReceivingStoreId)
			|| ((OrderCommon.CheckIdExistsInXmlStore(shippings[index].ShippingReceivingStoreId) == false)
				&& Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED == false)) return;

		// 配送不可エリアかチェック
		var inputZip = shippings[index].HyphenlessShippingZip;
		if (CheckUnavailableShippingAreaForOrderHistoryDetail(index, inputZip)) return;

		// 注文情報（変更前）取得
		var orderOld = new OrderService().Get(this.OrderModel.OrderId);

		CartObject cart = null;
		try
		{
			// 配送先入力情報更新後のカート情報取得して再計算
			cart = CreateCartAfterShippingAddr(index, shippings[index]);
			cart.Calculate(false);
		}
		catch (GlobalShippingPriceCalcException ex)
		{
			FileLogger.WriteError(ex.Message);
			wsErrorMessageShipping.InnerHtml = WebSanitizer.HtmlEncodeChangeToBr(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_ORDER_INSERT_ERROR));
			return;
		}

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			// Check Country Shipping Valid If Payment Of Order Is Paidy
			if ((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
				&& shippings.Any(shipping => (IsCountryJp(shipping.ShippingCountryIsoCode) == false)))
			{
				wsErrorMessageShipping.InnerHtml = WebSanitizer.HtmlEncodeChangeToBr(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PAIDY_COUNTRY_SHIPPING_NOT_JAPAN_ERROR));
				return;
			}
			// Check Country Iso Code Can Order NP After Pay
			else if ((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
				&& (shippings.Any(shipping => (IsCountryJp(shipping.ShippingCountryIsoCode) == false))
					|| (IsCountryJp(orderOld.Owner.OwnerAddrCountryIsoCode) == false)))
			{
				wsErrorMessageShipping.InnerHtml =
					WebSanitizer.HtmlEncodeChangeToBr(NPAfterPayUtility.GetErrorMessages(Constants.FLG_PAYMENT_NP_AFTERPAY_CUSTOM_ERROR_CODE_3));
				return;
			}
		}

		// 注文情報（変更後）取得
		var orderNew = (OrderModel)orderOld.Clone();
		orderNew.LastChanged = Constants.FLG_LASTCHANGED_USER;
		orderNew.Shippings = shippings;
		orderNew.OrderPriceShipping = cart.PriceShipping;
		orderNew.DeviceInfo = this.Request[Constants.REQUEST_GMO_DEFERRED_DEVICE_INFO];

		// 別出荷フラグを更新
		cart.UpdateAnotherShippingFlag();
		orderNew.Shippings[index].AnotherShippingFlg = cart.Shippings[index].AnotherShippingFlag;

		OrderCommon.SetCalculateTax(orderNew);
		foreach (var orderNewOrderPriceByTaxRate in orderNew.OrderPriceByTaxRates)
		{
			orderNewOrderPriceByTaxRate.OrderId = orderNew.OrderId;
		}
		var orderPriceTotal = orderOld.OrderPriceTotal;
		var diffShippingPrice = (CheckSetPromotionShippingFree(orderNew))
			? 0m
			: orderNew.OrderPriceShipping - orderOld.OrderPriceShipping;
		var difItemPriceSubtotalWithTax =
			TaxCalculationUtility.GetPriceTaxIncluded(orderNew.OrderPriceSubtotal, orderNew.OrderPriceSubtotalTax)
			- TaxCalculationUtility.GetPriceTaxIncluded(orderOld.OrderPriceSubtotal, orderOld.OrderPriceSubtotalTax);
		orderPriceTotal = orderPriceTotal + diffShippingPrice + difItemPriceSubtotalWithTax;

		orderNew.OrderPriceTotal = orderPriceTotal;
		orderNew.LastBilledAmount = orderPriceTotal;
		orderNew.OrderPointAdd = cart.FirstBuyPoint + cart.BuyPoint;

		// 決済なしに変更が可能かどうか
		IsChangeNoPaymentEnable(orderNew);

		// 金額変更 または 配送先変更
		var isExecuteExternalPayment = false;
		var isChangeLastBilledAmount = (orderOld.LastBilledAmount != orderNew.LastBilledAmount);
		if (isChangeLastBilledAmount
			|| CheckShippingChanged(orderOld.Shippings[0], orderNew.Shippings[0]))
		{
			var errorPaymentMessage = "";
			// 決済種別マスタチェック
			if (orderNew.ReturnExchangeKbn != Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE)
			{
				this.PaymentModel.PaymentId = orderNew.OrderPaymentKbn;
				errorPaymentMessage = WebSanitizer.HtmlEncodeChangeToBr(OrderCommon.CheckPaymentPriceEnabledForOrder(orderNew, this.PaymentModel.PaymentId));
				if (string.IsNullOrEmpty(errorPaymentMessage) == false)
				{
					wsErrorMessageShipping.InnerHtml = errorPaymentMessage;
					return;
				}
			}

			if (Constants.ECPAY_PAYMENT_OPTION_ENABLED
				&& (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY))
			{
				if (Constants.GLOBAL_OPTION_ENABLE
					&& (IsCountryTw(shippings[0].ShippingCountryIsoCode) == false))
				{
					wsErrorMessageShipping.InnerHtml =
						WebSanitizer.HtmlEncodeChangeToBr(WebMessages.GetMessages(
							WebMessages.ERRMSG_CHECK_COUNTRY_FOR_PAYMENT_ECPAY));
					return;
				}

				if (((orderOld.ExternalPaymentType == Constants.FLG_PAYMENT_TYPE_ECPAY_CREDIT)
						&& (orderNew.LastBilledAmount > orderOld.LastBilledAmount))
					|| ((orderOld.ExternalPaymentType != Constants.FLG_PAYMENT_TYPE_ECPAY_CREDIT)
						&& isChangeLastBilledAmount))
				{
					var companyName = GetDeliveryCompanyNameByCompanyId(
						this.AllDeliveryCompanyList, shippings[0].DeliveryCompanyId);
					wsErrorMessageShipping.InnerHtml =
						WebSanitizer.HtmlEncodeChangeToBr(WebMessages.GetMessages(
							WebMessages.ERRMSG_FRONT_SHIPPING_TYPE_CHANGE)
							.Replace("@@ 1 @@", companyName));
					return;
				}
			}

			// Check Can Change Address For NewebPay
			if (Constants.NEWEBPAY_PAYMENT_OPTION_ENABLED
				&& (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY))
			{
				if (Constants.GLOBAL_OPTION_ENABLE
					&& (IsCountryTw(shippings[0].ShippingCountryIsoCode) == false))
				{
					wsErrorMessageShipping.InnerHtml =
						WebSanitizer.HtmlEncodeChangeToBr(WebMessages.GetMessages(
							WebMessages.ERRMSG_CHECK_COUNTRY_FOR_PAYMENT_NEWEBPAY));
					return;
				}

				if (((orderOld.ExternalPaymentType == Constants.FLG_PAYMENT_TYPE_NEWEBPAY_CREDIT)
						&& (orderNew.LastBilledAmount > orderOld.LastBilledAmount))
					|| ((orderOld.ExternalPaymentType != Constants.FLG_PAYMENT_TYPE_NEWEBPAY_CREDIT)
						&& isChangeLastBilledAmount))
				{
					return;
				}
			}

			if (Constants.PAYMENT_PAYPAYOPTION_ENABLED
				&& (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY))
			{
				if (((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
						&& (orderNew.LastBilledAmount > orderOld.LastBilledAmount))
					|| ((orderOld.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
						&& isChangeLastBilledAmount)) return;
			}

			// クレジットカード売上確定後キャンセル可能かできるか？
			var isCancelable = OrderCommon.IsCancelableForCreditCardSalesCompleteOrder(orderOld);
			// エラーの場合、エラーメッセージセット
			if (isCancelable == false)
			{
				var errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_CREDIT_SALES_COMPLETE_CANCEL_ERROR);
				wsErrorMessageShipping.InnerHtml = errorMessage;
				return;
			}

			//決済金額決定
			orderNew.SettlementCurrency = CurrencyManager.GetSettlementCurrency(orderNew.OrderPaymentKbn);
			orderNew.SettlementRate = CurrencyManager.GetSettlementRate(orderNew.SettlementCurrency);
			orderNew.SettlementAmount = CurrencyManager.GetSettlementAmount(
				orderOld.OrderId,
				orderNew.OrderPaymentKbn,
				orderNew.OrderPriceTotal,
				orderNew.SettlementRate,
				orderNew.SettlementCurrency);

			// 請求書同梱フラグ判定
			orderNew.InvoiceBundleFlg = JudgementInvoiceBundleFlg(orderNew.OrderPaymentKbn, orderNew);

			// Check Valid Tel No And Country For Payment Atone And Aftee
			if ((cart.Payment != null)
				&& (Constants.PAYMENT_ATONEOPTION_ENABLED
					|| Constants.PAYMENT_AFTEEOPTION_ENABLED))
			{
				var ownerTel = cart.Owner.Tel1;
				var paymentId = cart.Payment.PaymentId;
				var errorMessage = string.Empty;

				switch (paymentId)
				{
					case Constants.FLG_PAYMENT_PAYMENT_ID_ATONE:
						// Check Country
						var hasShippingJp = (cart.Shippings.Any(shipping => shipping.IsShippingAddrJp));
						if ((cart.Owner.IsAddrJp == false)
							|| (hasShippingJp == false))
						{
							errorMessage += WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CHECK_COUNTRY_FOR_PAYMENT_ATONE);
						}
						break;

					case Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE:
						// Check Country
						var hasShippingTaiwan = (cart.Shippings.Any(shipping => shipping.IsShippingAddrTw));
						if ((cart.Owner.IsAddrTw == false)
							|| (hasShippingTaiwan == false))
						{
							errorMessage += WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CHECK_COUNTRY_FOR_PAYMENT_AFTEE);
						}
						break;
				}

				if (string.IsNullOrEmpty(errorMessage)
					&& wcbIsUpdateFixedPurchaseByOrderShippingInfo.HasInnerControl
					&& wcbIsUpdateFixedPurchaseByOrderShippingInfo.Checked)
				{
					errorMessage += CheckValidTelNoForPaymentAtoneAndAfteeForFixedPurchase(cart);
				}

				if (string.IsNullOrEmpty(errorMessage) == false)
				{
					wsErrorMessageShipping.InnerHtml = errorMessage;
					return;
				}
			}

			if (OrderCommon.CheckPaymentYamatoKaSms(orderNew.OrderPaymentKbn)
				&& (bool.Parse(whfShowModal.Value) == false))
			{
				Session[Constants.SESSION_KEY_PARAM] = new Hashtable
				{
					{"order_new", orderNew},
				};
				var telNum = new UserCreditCardService().Get(this.OrderModel.UserId, this.OrderModel.CreditBranchNo.Value).CooperationId;
				ExecutePaymentYamatoKaSms(orderNew, telNum);
				var wucPaymentYamatoKaSmsAuthModal = GetYamatoKaSmsAuthModal();
				var lbAuthorize = wucPaymentYamatoKaSmsAuthModal.FindControl("lbAuthorize") as LinkButton;
				if (lbAuthorize != null) lbAuthorize.CommandArgument = index.ToString();
				this.OnAuthorizeCompleteMethodName = MethodBase.GetCurrentMethod().Name;
				return;
			}
			else if (Session[Constants.SESSION_KEY_PAYMENT_ORDER_ID] != null)
			{
				orderNew.CardTranId = (string)Session[Constants.SESSION_KEY_PAYMENT_ORDER_ID];
				orderNew.PaymentOrderId = (string)Session[Constants.SESSION_KEY_PAYMENT_ORDER_ID];
				Session.Remove(Constants.SESSION_KEY_PAYMENT_ORDER_ID);

				whfShowModal.Value = "false";
			}

			// 外部決済連携実行
			string apiErrorMassage;
			isExecuteExternalPayment = ExecuteExternalPayment(orderOld, orderNew, out apiErrorMassage);

			var hasApiErrorMassage = (string.IsNullOrEmpty(apiErrorMassage) == false);

			// 外部決済連携実施時のみログを残す
			if (isExecuteExternalPayment)
			{
				// 外部決済連携ログ処理
				OrderCommon.AppendExternalPaymentCooperationLog(
					hasApiErrorMassage == false,
					orderOld.OrderId,
					hasApiErrorMassage ? apiErrorMassage : LogCreator.CreateMessage(orderOld.OrderId, ""),
					Constants.FLG_LASTCHANGED_USER,
					UpdateHistoryAction.Insert);
			}

			if (hasApiErrorMassage)
			{
				//再与信失敗エラー表示
				if (this.PaymentModel.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
				{
					errorPaymentMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CARDAUTH_ERROR);
				}
				else if (
					(this.PaymentModel.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
					|| (this.PaymentModel.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_SMS_DEF))
				{
					errorPaymentMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CVSAUTH_ERROR);
				}
				else if (this.PaymentModel.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
				{
					errorPaymentMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_TRYLINKAFTERPAYAUTH_ERROR);
				}
				else if (this.PaymentModel.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
				{
					errorPaymentMessage =
						apiErrorMassage.Contains(NPAfterPayUtility.GetErrorMessages(Constants.FLG_PAYMENT_NP_AFTERPAY_CUSTOM_ERROR_CODE_2))
							? CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_AUTH_ERROR)
							: apiErrorMassage;
				}
				else
				{
					errorPaymentMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_AUTH_EXCEPTION);
				}
				wsErrorMessageShipping.InnerHtml = errorPaymentMessage;

				return;
			}
		}

		this.OrderHistoryErrorMessage = string.Empty;
		var isChangeShippingAddress = ((orderOld.Shippings[0].ShippingZip != orderNew.Shippings[0].ShippingZip)
			|| (orderOld.Shippings[0].ShippingAddr1 != orderNew.Shippings[0].ShippingAddr1)
			|| (orderOld.Shippings[0].ShippingAddr2 != orderNew.Shippings[0].ShippingAddr2)
			|| (orderOld.Shippings[0].ShippingAddr3 != orderNew.Shippings[0].ShippingAddr3)
			|| (orderOld.Shippings[0].ShippingAddr4 != orderNew.Shippings[0].ShippingAddr4)
			|| (orderOld.Shippings[0].ShippingAddr5 != orderNew.Shippings[0].ShippingAddr5));
		var isChangeShippingInfo = (isChangeShippingAddress
			|| (orderOld.Shippings[0].ShippingName1 != orderNew.Shippings[0].ShippingName1)
			|| (orderOld.Shippings[0].ShippingName2 != orderNew.Shippings[0].ShippingName2)
			|| (orderOld.Shippings[0].ShippingName != orderNew.Shippings[0].ShippingName)
			|| (orderOld.Shippings[0].ShippingNameKana1 != orderNew.Shippings[0].ShippingNameKana1)
			|| (orderOld.Shippings[0].ShippingNameKana2 != orderNew.Shippings[0].ShippingNameKana2)
			|| (orderOld.Shippings[0].ShippingNameKana != orderNew.Shippings[0].ShippingNameKana)
			|| (orderOld.Shippings[0].ShippingCountryIsoCode != orderNew.Shippings[0].ShippingCountryIsoCode)
			|| (orderOld.Shippings[0].ShippingTel1 != orderNew.Shippings[0].ShippingTel1));

		// 初回購入か
		var isFirstOrder = DomainFacade.Instance.OrderService.CheckOrderFirstBuy(orderNew.UserId, orderNew.OrderId);
		using (var accessor = new SqlAccessor())
		{
			// トランザクション開始
			accessor.OpenConnection();
			accessor.BeginTransaction();
			var errorShippingMessage = "";
			try
			{
				if ((orderOld.OrderPriceTotal != orderNew.OrderPriceTotal)
					|| ((this.PaymentModel.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
						&& ((orderNew.CardTranId != orderOld.CardTranId)
							|| ((Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Atodene)
								|| (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Score)
									&& CheckShippingChanged(orderOld.Shippings[0], orderNew.Shippings[0]))))
					|| (isChangeShippingAddress
						&& (orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY))
					|| (isChangeShippingInfo
						&& (orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)))
				{
					if (ExecuteUpdateOrderAndRegisterUpdateHistory(orderNew, UpdateHistoryAction.DoNotInsert, accessor, isExecuteExternalPayment) == false)
					{
						errorShippingMessage = WebSanitizer.HtmlEncodeChangeToBr(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_ORDER_INSERT_ERROR));
						throw new Exception("注文情報の更新に失敗しました。");
					}

					// セットプロモーションに配送料割引があれば金額を合わせる
					if (orderNew.SetPromotions.Any())
					{
						var setpromotions =
							orderNew.SetPromotions.Where(
								sp => (sp.ShippingChargeFreeFlg == Constants.FLG_SETPROMOTION_SHIPPING_CHARGE_FREE_FLG_ON)
									&& (sp.ShippingChargeDiscountAmount != 0));
						foreach (var setpromotion in setpromotions)
						{
							setpromotion.ShippingChargeDiscountAmount = orderNew.OrderPriceShipping;
							var updateSetpromotionCount = new OrderService().UpdateSetPromotionForModify(new[] { setpromotion }, accessor);
							if (updateSetpromotionCount == 0)
							{
								errorShippingMessage = WebSanitizer.HtmlEncodeChangeToBr(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_ORDER_INSERT_ERROR));
								throw new Exception("注文セットプロモーション情報の更新に失敗しました。");
							}
						}
					}
				}

				// Display shipping date error message
				if (CheckShippingDate(shippings[index]) == false)
				{
					// Calculate
					shippings[index].ShippingDate = HolidayUtil.GetShortestDeliveryDate(
						this.ShopId,
						shippings[index].DeliveryCompanyId,
						shippings[index].ShippingAddr1,
						shippings[index].ShippingZip);
					shippings[index].ScheduledShippingDate
						= HolidayUtil.GetShortestShippingDateBasedOnToday(shippings[index].DeliveryCompanyId);

					this.OrderHistoryErrorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_SHIPPINGDATE_CHANGED)
						.Replace("@@ 1 @@", shippings[index].ShippingDate.Value.ToString("yyyy年MM月dd日(ddd)", new CultureInfo("ja-JP")));
				}

				// 配送先更新
				var updateShippingCount = new OrderService().UpdateShippingForModify(shippings, orderNew.LastChanged, UpdateHistoryAction.DoNotInsert, accessor);
				if (updateShippingCount == 0)
				{
					errorShippingMessage = WebSanitizer.HtmlEncodeChangeToBr(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_ORDER_INSERT_ERROR));
					throw new Exception("注文配送先の更新に失敗しました。");
				}

				var updatedOrderPriceInfoByTaxRate =
					new OrderService().UpdateOrderPriceInfoByTaxRateModify(orderNew.OrderPriceByTaxRates, accessor);
				if (updatedOrderPriceInfoByTaxRate == 0)
				{
					errorShippingMessage = WebSanitizer.HtmlEncodeChangeToBr(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_ORDER_INSERT_ERROR));
					throw new Exception("税率毎価格情報の更新に失敗しました。");
				}

				// 注文商品更新
				var updateItemCount = new OrderService().UpdateItemForModify(shippings.SelectMany(shipping => shipping.Items).ToArray(), orderNew.LastChanged, UpdateHistoryAction.DoNotInsert, accessor);
				if (updateItemCount == 0)
				{
					errorShippingMessage = WebSanitizer.HtmlEncodeChangeToBr(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_ORDER_INSERT_ERROR));
					throw new Exception("注文商品の更新に失敗しました。");
				}

				var afterExtendStatus39 = (this.SimilarShippingOrderIdList.Length == 0)
					? Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF
					: Constants.FLG_ORDER_ORDER_EXTEND_STATUS_ON;

				// 管理メモ更新
				new OrderService().UpdateManagementMemo(
					orderNew.OrderId,
					OrderCommon.GetNotFirstTimeFixedPurchaseManagementMemo(
						orderOld.ManagementMemo,
						this.SimilarShippingOrderIdList,
						false,
						orderOld.ExtendStatus39,
						afterExtendStatus39),
					orderNew.LastChanged,
					UpdateHistoryAction.DoNotInsert,
					accessor);

				// 注文拡張項目更新
				new OrderService().UpdateOrderExtendStatus(
					orderNew.OrderId,
					Constants.ORDER_EXTEND_STATUS_NO_CHECK_FIRSTTIME_OWNER_DUPLICATE,
					afterExtendStatus39,
					DateTime.Now,
					orderNew.LastChanged,
					UpdateHistoryAction.DoNotInsert,
					accessor);
				if ((orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)
					|| (orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE))
				{
					// Update Card Tran Id
					new OrderService().Modify(
						orderNew.OrderId,
						model =>
						{
							model.CardTranId = orderNew.CardTranId;
						},
						UpdateHistoryAction.DoNotInsert,
						accessor);
				}

				// Update invoice bundle flag
				if ((Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Atobaraicom)
					&& (orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF))
				{
					var updateInvoiceBundleFlg = new OrderService().UpdateInvoiceBundleFlg(
						orderNew.OrderId,
						orderNew.InvoiceBundleFlg,
						this.LoginUserName,
						UpdateHistoryAction.DoNotInsert,
						accessor);

					if (updateInvoiceBundleFlg == false)
					{
						errorShippingMessage = WebSanitizer.HtmlEncodeChangeToBr(
							WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_ORDER_INSERT_ERROR));
						throw new Exception("請求書更新フラグ更新に失敗しました。");
					}
				}

				// 更新履歴登録
				new UpdateHistoryService().InsertForOrder(orderNew.OrderId, orderNew.LastChanged, accessor);

				// ポイント付与情報更新
				ExecUpdateUserPoint(orderOld, orderNew, isFirstOrder, cart.FirstBuyPoint, cart.BuyPoint, UpdateHistoryAction.DoNotInsert, accessor);

				accessor.CommitTransaction();
			}
			catch (Exception ex)
			{
				AppLogger.WriteError(ex);
				accessor.RollbackTransaction();
				wsErrorMessageShipping.InnerHtml = errorShippingMessage;
				return;
			}
		}

		// 定期情報も変更するにチェックがある場合に定期情報も更新
		if ((wcbIsUpdateFixedPurchaseByOrderShippingInfo.InnerControl != null)
			&& wcbIsUpdateFixedPurchaseByOrderShippingInfo.Checked)
		{
			var whfCvsShopFlg = GetWrappedControl<WrappedHiddenField>(this.WrOrderShipping.Items[index], "hfCvsShopFlg");
			var fixedPurchaseService = new FixedPurchaseService();
			var fixedPurchase = fixedPurchaseService.Get(this.OrderModel.FixedPurchaseId);
			var fixedPurchaseShipping = fixedPurchase.Shippings[index];
			if ((whfCvsShopFlg.InnerControl == null)
				|| (whfCvsShopFlg.Value == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF))
			{
				// 入力情報作成
				fixedPurchaseShipping.ShippingName1 = shippings[index].ShippingName1;
				fixedPurchaseShipping.ShippingName2 = shippings[index].ShippingName2;
				fixedPurchaseShipping.ShippingName = shippings[index].ShippingName;
				fixedPurchaseShipping.ShippingNameKana1 = shippings[index].ShippingNameKana1;
				fixedPurchaseShipping.ShippingNameKana2 = shippings[index].ShippingNameKana2;
				fixedPurchaseShipping.ShippingNameKana = shippings[index].ShippingNameKana;
				fixedPurchaseShipping.ShippingCountryName = shippings[index].ShippingCountryName;
				fixedPurchaseShipping.ShippingCountryIsoCode = shippings[index].ShippingCountryIsoCode;
				fixedPurchaseShipping.ShippingZip = shippings[index].ShippingZip;
				fixedPurchaseShipping.ShippingAddr1 = shippings[index].ShippingAddr1;
				fixedPurchaseShipping.ShippingAddr2 = shippings[index].ShippingAddr2;
				fixedPurchaseShipping.ShippingAddr3 = shippings[index].ShippingAddr3;
				fixedPurchaseShipping.ShippingAddr4 = shippings[index].ShippingAddr4;
				fixedPurchaseShipping.ShippingAddr5 = shippings[index].ShippingAddr5;
				fixedPurchaseShipping.ShippingCompanyName = shippings[index].ShippingCompanyName;
				fixedPurchaseShipping.ShippingCompanyPostName = shippings[index].ShippingCompanyPostName;
				fixedPurchaseShipping.ShippingTel1 = shippings[index].ShippingTel1;
				fixedPurchaseShipping.ShippingReceivingStoreFlg = Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF;
			}
			else
			{
				fixedPurchaseShipping.ShippingReceivingStoreFlg = Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON;
				fixedPurchaseShipping.ShippingReceivingStoreId = shippings[index].ShippingReceivingStoreId;
				fixedPurchaseShipping.ShippingName = shippings[index].ShippingName;
				fixedPurchaseShipping.ShippingAddr4 = shippings[index].ShippingAddr4;
				fixedPurchaseShipping.ShippingTel1 = shippings[index].ShippingTel1;
				fixedPurchaseShipping.ShippingReceivingStoreType = shippings[index].ShippingReceivingStoreType;
			}

			// 定期情報の更新（更新履歴とともに）
			fixedPurchaseService.UpdateShipping(fixedPurchaseShipping, Constants.FLG_LASTCHANGED_USER, UpdateHistoryAction.Insert);
		}

		// Update payment convenience store
		if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED
			&& Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED)
		{
			var wddlShippingReceivingStoreType = GetWrappedControl<WrappedDropDownList>(
				(Control)sender,
				"ddlShippingReceivingStoreType");
			if ((wddlShippingReceivingStoreType.HasInnerControl)
				&& (ECPayUtility.GetIsCollection(wddlShippingReceivingStoreType.SelectedValue)
					== Constants.FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_COLLECTION_ON))
			{
				var shippingReceivingStoreTypeSelected = wddlShippingReceivingStoreType.SelectedValue;
				this.OrderModel.Shippings[0].ShippingReceivingStoreType = shippingReceivingStoreTypeSelected;
				var wcbUpdateFixedPurchase = GetWrappedControl<WrappedCheckBox>("cbIsUpdateFixedPurchaseByOrderPayment");
				wcbUpdateFixedPurchase.Checked = wcbIsUpdateFixedPurchaseByOrderShippingInfo.Checked;
				btnUpdatePaymentPatternInfo_Click(sender, e);
			}
		}

		// メール送信
		SendMailCommon.SendModifyPurchaseHistoryMail(this.OrderModel.OrderId, SendMailCommon.PurchaseHistoryModify.Shipping);

		ReloadOrderHistoryDetail();
	}

	/// <summary>
	/// 配送会社IDで配送会社名を取得
	/// </summary>
	/// <param name="companylist">配送会社リスト</param>
	/// <param name="companyId">配送会社ID</param>
	/// <returns></returns>
	private string GetDeliveryCompanyNameByCompanyId(DeliveryCompanyModel[] companylist, string companyId)
	{
		var deliveryCompany = companylist
			.First(company => (company.DeliveryCompanyId == companyId));
		var name = (deliveryCompany != null)
			? deliveryCompany.DeliveryCompanyName
			: string.Empty;
		return name;
	}

	/// <summary>
	/// ヤマト後払いSMS認証決済連携
	/// </summary>
	/// <param name="orderNew">変更後注文情報</param>
	/// <param name="telNum">電話番号</param>
	private void ExecutePaymentYamatoKaSms(OrderModel orderNew, string telNum)
	{
		var result = ExecutePaymentYamatoKaSmsImp(orderNew, telNum);

		if (result)
		{
			whfShowModal.Value = "true";
		}
		else
		{
			this.WsErrorMessagePayment.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CVSAUTH_ERROR);
		}
	}

	/// <summary>
	/// ヤマト後払いSMS認証決済連携
	/// </summary>
	/// <param name="orderNew">変更後注文情報</param>
	private static bool ExecutePaymentYamatoKaSmsImp(OrderModel orderNew, string telNum)
	{
		var tranId = OrderCommon.CreatePaymentOrderId(orderNew.ShopId);

		var isSms = OrderCommon.CheckPaymentYamatoKaSms(orderNew.OrderPaymentKbn);
		var entryApi = new PaymentYamatoKaEntryApi();
		var result = entryApi.Exec(
			tranId,
			DateTime.Now.ToString("yyyyMMdd"),
			PaymentYamatoKaUtility.CreateYamatoKaShipYmd(orderNew),
			orderNew.Owner.OwnerName,
			StringUtility.ToHankakuKatakana(StringUtility.ToZenkakuKatakana(orderNew.Owner.OwnerNameKana)),
			orderNew.Owner.OwnerZip,
			new PaymentYamatoKaAddress(
				orderNew.Owner.OwnerAddr1,
				orderNew.Owner.OwnerAddr2,
				orderNew.Owner.OwnerAddr3,
				orderNew.Owner.OwnerAddr4),
			telNum,
			orderNew.Owner.OwnerMailAddr,
			orderNew.LastBilledAmount,
			PaymentYamatoKaUtility.CreateSendDiv(isSms, orderNew.Shippings[0].AnotherShippingFlg),
			PaymentYamatoKaUtility.CreateProductItemList(orderNew.LastBilledAmount),
			orderNew.Shippings[0].ShippingName,
			orderNew.Shippings[0].ShippingZip,
			new PaymentYamatoKaAddress(
				orderNew.Shippings[0].ShippingAddr1,
				orderNew.Shippings[0].ShippingAddr2,
				orderNew.Shippings[0].ShippingAddr3,
				orderNew.Shippings[0].ShippingAddr4),
			orderNew.Shippings[0].ShippingTel1);

		OrderCommon.AppendExternalPaymentCooperationLog(
			result,
			orderNew.OrderId,
			LogCreator.CreateErrorMessage(entryApi.ResponseData.ErrorCode, entryApi.ResponseData.ErrorMessages),
			orderNew.LastChanged,
			UpdateHistoryAction.Insert);

		if (result)
		{
			HttpContext.Current.Session[Constants.SESSION_KEY_PAYMENT_ORDER_ID] = tranId;
		}

		return result;
	}

	/// <summary>
	/// ポイント分で小計すべてを払いきれる場合
	/// 決済種別を[決済なし]に変更する
	/// </summary>
	/// <param name="orderNew">受注情報</param>
	private bool IsChangeNoPaymentEnable(OrderModel orderNew)
	{
		var discountTotalPrice = orderNew.IsContainPaymentChargeFreeSet
			? orderNew.OrderPriceTotal
			: orderNew.OrderPriceTotal - orderNew.OrderPriceExchange;
		var order = orderNew.Clone();
		order.FixedPurchaseId = null;

		var enablePayments = OrderCommon.GetValidPaymentListForOrder(
			order,
			this.ShopShippingModel.PaymentSelectionFlg,
			this.ShopShippingModel.PermittedPaymentIds,
				this.LoginUserId)
			.Select(p => p.PaymentId)
			.ToArray();

		if ((discountTotalPrice == decimal.Zero))
		{
			if ((enablePayments.Contains(Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT) == false)) return false;

			orderNew.OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT;
			orderNew.OrderPriceExchange = 0m;
			orderNew.OrderPriceExchangeTax = 0m;
			orderNew.OrderPriceTotal = 0m;
			orderNew.LastBilledAmount = 0m;
		}

		return true;
	}

	/// <summary>
	/// 配送先入力情報更新後のカート情報取得
	/// </summary>
	/// <param name="index">配送先枝番</param>
	/// <param name="shippingInput">配送先入力情報</param>
	/// <returns>配送先入力情報更新後のカート情報</returns>
	private CartObject CreateCartAfterShippingAddr(int index, OrderShippingModel shippingInput)
	{
		var order = (OrderModel)this.OrderModel.Clone();
		order.Shippings[index].ShippingName1 = shippingInput.ShippingName1;
		order.Shippings[index].ShippingName2 = shippingInput.ShippingName2;
		order.Shippings[index].ShippingNameKana1 = shippingInput.ShippingNameKana1;
		order.Shippings[index].ShippingNameKana2 = shippingInput.ShippingNameKana2;
		order.Shippings[index].ShippingZip = shippingInput.ShippingZip;
		order.Shippings[index].ShippingAddr1 = shippingInput.ShippingAddr1;
		order.Shippings[index].ShippingAddr2 = shippingInput.ShippingAddr2;
		order.Shippings[index].ShippingAddr2 = shippingInput.ShippingAddr2;
		order.Shippings[index].ShippingAddr3 = shippingInput.ShippingAddr3;
		order.Shippings[index].ShippingAddr4 = shippingInput.ShippingAddr4;
		order.Shippings[index].ShippingAddr5 = shippingInput.ShippingAddr5;
		order.Shippings[index].ShippingCountryIsoCode = shippingInput.ShippingCountryIsoCode;
		order.Shippings[index].ShippingCountryName = shippingInput.ShippingCountryName;
		order.Shippings[index].ShippingCompanyName = shippingInput.ShippingCompanyName;
		order.Shippings[index].ShippingCompanyPostName = shippingInput.ShippingCompanyPostName;
		order.Shippings[index].ShippingTel1 = shippingInput.ShippingTel1;
		order.Shippings[index].ShippingTel2 = shippingInput.ShippingTel2;
		order.Shippings[index].ShippingTel3 = shippingInput.ShippingTel3;
		order.Shippings[index].ShippingReceivingStoreId = shippingInput.ShippingReceivingStoreId;
		order.Shippings[index].ShippingReceivingStoreFlg = shippingInput.ShippingReceivingStoreFlg;
		var cart = CartObject.CreateCartByOrder(order);

		return cart;
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
	/// 対象の注文のセットプロモーションに配送無料のフラグがONに設定されているか
	/// </summary>
	/// <param name="orderModel">対象の注文モデル</param>
	/// <returns>配送無料のフラグON:true フラグOFF:false</returns>
	protected bool CheckSetPromotionShippingFree(OrderModel orderModel)
	{
		if (this.OrderModel.SetPromotions.Any() == false) return false;
		var setpromotions =
			orderModel.SetPromotions.Where(
				sp => (sp.ShippingChargeFreeFlg == Constants.FLG_SETPROMOTION_SHIPPING_CHARGE_FREE_FLG_ON)
					&& (sp.ShippingChargeDiscountAmount != 0)).ToArray();
		return (setpromotions.Length > 0);
	}

	/// <summary>
	/// お届け先 キャンセルボタン クリックイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbHideUserShippingInfoForm_Click(object sender, EventArgs e)
	{
		var index = int.Parse(((LinkButton)sender).CommandArgument);
		var wdvShippingInfo = GetWrappedControl<WrappedHtmlGenericControl>(this.WrOrderShipping.Items[index], "dShippingInfo");
		var wdvShippngInput = GetWrappedControl<WrappedHtmlGenericControl>(this.WrOrderShipping.Items[index], "dShippngInput");
		wdvShippingInfo.Visible = (wdvShippingInfo.Visible == false);
		wdvShippngInput.Visible = (wdvShippngInput.Visible == false);
		this.WhfConfirmSenderId.Value = "";

		// Reset selected value in ddlShippingType
		var wddlShippingType = GetWrappedControl<WrappedDropDownList>(this.WrOrderShipping.Items[index], "ddlShippingType");
		wddlShippingType.SelectedValue = IsDisplayButtonConvenienceStore(this.OrderModel.Shippings[0].ShippingReceivingStoreFlg)
			? CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE
			: null;

		if (wddlShippingType.HasInnerControl)
			ddlShippingType_SelectedIndexChanged(wddlShippingType.InnerControl, null);
	}

	/// <summary>
	/// 配送希望日時ドロップダウンリスト作成 (バインド用)
	/// </summary>
	/// <returns>配送希望時間帯リスト</returns>
	protected ListItemCollection GetListShippingDate()
	{
		var licShippingDate = new ListItemCollection();
		if (this.ShopShippingModel.ShippingDateSetBegin == null || this.ShopShippingModel.ShippingDateSetEnd == null) return licShippingDate;

		var startDateAfterHoliday = HolidayUtil.GetDateOfBusinessDay(DateTime.Now, this.ShopShippingModel.BusinessDaysForShipping, true);

		// 配送希望日リスト作成
		if (Constants.DISPLAY_DEFAULTSHIPPINGDATE_ENABLED)
		{
			licShippingDate.Add(new ListItem(ReplaceTag("@@DispText.shipping_date_list.none@@"), ""));
		}
		for (int i = this.ShopShippingModel.ShippingDateSetBegin.Value; i < (this.ShopShippingModel.ShippingDateSetBegin.Value + this.ShopShippingModel.ShippingDateSetEnd.Value); i++)
		{
			var dtTarget = startDateAfterHoliday.AddDays(i);
			licShippingDate.Add(new ListItem(
				DateTimeUtility.ToStringFromRegion(dtTarget, DateTimeUtility.FormatType.LongDateWeekOfDay1Letter),
				dtTarget.ToString("yyyy/MM/dd")));
		}
		return licShippingDate;
	}

	/// <summary>
	/// 配送希望日 変更ボタン クリックイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbDisplayInputShippingDate_Click(object sender, EventArgs e)
	{
		var index = int.Parse(((LinkButton)sender).CommandArgument);
		var wddlShippingDateList = GetWrappedControl<WrappedDropDownList>(this.WrOrderShipping.Items[index], "ddlShippingDateList");
		var wdvShippingDateText = GetWrappedControl<WrappedHtmlGenericControl>(this.WrOrderShipping.Items[index], "dvShippingDateText");
		var wtrShippingDateInput = GetWrappedControl<WrappedHtmlGenericControl>(this.WrOrderShipping.Items[index], "trShippingDateInput");
		var wlShippingDateErrorMessage = GetWrappedControl<WrappedLabel>(this.WrOrderShipping.Items[index], "lShippingDateErrorMessage");
		wddlShippingDateList.Visible = (wddlShippingDateList.Visible == false);
		wdvShippingDateText.Visible = (wdvShippingDateText.Visible == false);
		wtrShippingDateInput.Visible = (wtrShippingDateInput.Visible == false);
		wlShippingDateErrorMessage.Visible = false;
		wlShippingDateErrorMessage.Text = string.Empty;

		if (wtrShippingDateInput.Visible)
		{
			if (this.OrderShippingItems[index] == null) return;
			var shipping = new OrderShippingModel((DataRowView)this.OrderShippingItems[index]["row"]);

			//配送希望日の設定
			var shippingDate = (shipping.ShippingDate != null) ? shipping.ShippingDate.Value.ToString("yyyy/MM/dd") : string.Empty;
			foreach (ListItem item in wddlShippingDateList.Items)
			{
				item.Selected = (item.Value == shippingDate);
			}
		}
	}

	/// <summary>
	/// 配送希望日時ドロップダウンリスト作成 (バインド用)
	/// </summary>
	/// <param name="shippingCompanyId">配送種別情報</param>
	/// <returns>配送希望時間帯ドロップダウンリスト</returns>
	protected ListItemCollection GetShippingTimeList(string shippingCompanyId)
	{
		var result = new ListItemCollection();
		var shippingCompany = new DeliveryCompanyService().Get(shippingCompanyId);
		// 配送希望時間帯設定可能フラグが有効?
		if (shippingCompany.IsValidShippingTimeSetFlg)
		{
			result.Add(new ListItem(ReplaceTag("@@DispText.shipping_time_list.none@@"), ""));
			result.AddRange(shippingCompany.GetShippingTimeList().Select(kvp => new ListItem(kvp.Value, kvp.Key)).ToArray());
		}
		return result;
	}

	/// <summary>
	/// 配送希望日 情報更新ボタン クリックイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbUpdateShippingDate_Click(object sender, EventArgs e)
	{
		var index = int.Parse(((LinkButton)sender).CommandArgument);

		var orderService = new OrderService();
		// 注文情報（変更前）取得
		var orderOld = orderService.Get(this.OrderModel.OrderId);
		// 注文情報（変更後）取得
		var wddlShippingDateList = GetWrappedControl<WrappedDropDownList>(this.WrOrderShipping.Items[index], "ddlShippingDateList");
		var wsErrorMessageShippingDate = GetWrappedControl<WrappedHtmlGenericControl>(this.WrOrderShipping.Items[index], "sErrorMessageShippingDate");
		var wlShippingDateErrorMessage = GetWrappedControl<WrappedLabel>(this.WrOrderShipping.Items[index], "lShippingDateErrorMessage");

		var orderNew = orderService.Get(this.OrderModel.OrderId);
		var shipping = orderNew.Shippings[index];

		shipping.ShippingDate = (wddlShippingDateList.SelectedValue != string.Empty)
			? (DateTime?)DateTime.Parse(wddlShippingDateList.SelectedValue)
			: null;

		// Display shipping date error message
		if (CheckShippingDate(orderNew.Shippings[index]) == false)
		{
			wlShippingDateErrorMessage.Visible = true;
			wlShippingDateErrorMessage.Text = WebMessages.GetMessages(WebMessages.ERRMSG_SHIPPINGDATE_INVALID)
				.Replace("@@ 1 @@", DateTimeUtility.ToStringFromRegion(
					HolidayUtil.GetShortestDeliveryDate(
						this.ShopId,
						shipping.DeliveryCompanyId,
						shipping.ShippingAddr1,
						shipping.ShippingZip),
					DateTimeUtility.FormatType.LongDateWeekOfDay2Letter));
			return;
		}

		// Calculate scheduled shipping date
		shipping.ScheduledShippingDate = OrderCommon.CalculateScheduledShippingDateBasedOnToday(
			this.ShopId,
			shipping);

		// クレジットカード売上確定後キャンセル可能かできるか？
		var isCancelable = OrderCommon.IsCancelableForCreditCardSalesCompleteOrder(orderOld);
		// エラーの場合、エラーメッセージセット
		if (isCancelable == false)
		{
			wsErrorMessageShippingDate.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_CREDIT_SALES_COMPLETE_CANCEL_ERROR);
			return;
		}

		// 配送希望日が変更されたため外部決済連携実行
		string apiErrorMassage;
		ExecuteExternalPayment(orderOld, orderNew, out apiErrorMassage);

		var hasApiErrorMessage = (string.IsNullOrEmpty(apiErrorMassage) == false);

		// 外部決済連携ログ処理
		OrderCommon.AppendExternalPaymentCooperationLog(
			hasApiErrorMessage == false,
			orderNew.OrderId,
			hasApiErrorMessage ? apiErrorMassage : LogCreator.CreateMessage(orderOld.OrderId, ""),
			Constants.FLG_LASTCHANGED_USER,
			UpdateHistoryAction.Insert);

		if (hasApiErrorMessage)
		{
			if (this.PaymentModel.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
			{
				wsErrorMessageShippingDate.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CARDAUTH_ERROR);
			}
			else if ((this.PaymentModel.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
				|| (this.PaymentModel.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_SMS_DEF))
			{
				wsErrorMessageShippingDate.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CVSAUTH_ERROR);
			}
			else if (this.PaymentModel.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
			{
				wsErrorMessageShippingDate.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_TRYLINKAFTERPAYAUTH_ERROR);
			}
			else
			{
				wsErrorMessageShippingDate.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_AUTH_EXCEPTION);
			}
			return;
		}

		// 配送希望更新（更新履歴とともに）
		if (UpdateShippingDateTime(orderNew, UpdateHistoryAction.Insert))
		{
			// メール送信
			SendMailCommon.SendModifyPurchaseHistoryMail(this.OrderModel.OrderId, SendMailCommon.PurchaseHistoryModify.ShippingDate);
			ReloadOrderHistoryDetail();
		}
		else
		{
			wsErrorMessageShippingDate.InnerHtml = WebSanitizer.HtmlEncodeChangeToBr(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_ORDER_INSERT_ERROR));
		}
	}

	/// <summary>
	/// 配送希望日 キャンセルボタン クリックイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbHideShippingDate_Click(object sender, EventArgs e)
	{
		var index = int.Parse(((LinkButton)sender).CommandArgument);
		var wddlShippingDateList = GetWrappedControl<WrappedDropDownList>(this.WrOrderShipping.Items[index], "ddlShippingDateList");
		var wdvShippingDateText = GetWrappedControl<WrappedHtmlGenericControl>(this.WrOrderShipping.Items[index], "dvShippingDateText");
		var wtrShippingDateInput = GetWrappedControl<WrappedHtmlGenericControl>(this.WrOrderShipping.Items[index], "trShippingDateInput");
		var wlShippingDateErrorMessage = GetWrappedControl<WrappedLabel>(this.WrOrderShipping.Items[index], "lShippingDateErrorMessage");
		wddlShippingDateList.Visible = (wddlShippingDateList.Visible == false);
		wdvShippingDateText.Visible = (wdvShippingDateText.Visible == false);
		wtrShippingDateInput.Visible = (wtrShippingDateInput.Visible == false);
		wlShippingDateErrorMessage.Visible = false;
		wlShippingDateErrorMessage.Text = string.Empty;
	}

	/// <summary>
	/// 配送希望時間帯 変更ボタン クリックイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbDisplayInputShippingTime_Click(object sender, EventArgs e)
	{
		var index = int.Parse(((LinkButton)sender).CommandArgument);
		var wddlShippingTimeList = GetWrappedControl<WrappedDropDownList>(this.WrOrderShipping.Items[index], "ddlShippingTimeList");
		var wdvShippingTimeText = GetWrappedControl<WrappedHtmlGenericControl>(this.WrOrderShipping.Items[index], "dvShippingTimeText");
		var wtrShippingTimeInput = GetWrappedControl<WrappedHtmlGenericControl>(this.WrOrderShipping.Items[index], "trShippingTimeInput");
		wddlShippingTimeList.Visible = (wddlShippingTimeList.Visible == false);
		wdvShippingTimeText.Visible = (wdvShippingTimeText.Visible == false);
		wtrShippingTimeInput.Visible = (wtrShippingTimeInput.Visible == false);

		if (wtrShippingTimeInput.Visible)
		{
			if (this.OrderShippingItems[index] == null) return;
			var shipping = new OrderShippingModel((DataRowView)this.OrderShippingItems[index]["row"]);

			//配送希望時間帯の設定
			var shippingTime = (shipping.ShippingTime != null) ? shipping.ShippingTime.ToString() : string.Empty;
			foreach (ListItem item in wddlShippingTimeList.Items)
			{
				item.Selected = (item.Value == shippingTime);
			}
		}
	}

	/// <summary>
	/// 領収書情報変更フォーム表示
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbDisplayReceiptInfoForm_Click(object sender, EventArgs e)
	{
		this.IsReceiptInfoModify = (this.IsReceiptInfoModify == false);
		if (this.IsReceiptInfoModify == false) return;

		// 領収書情報を入力項目にセット
		this.WddlReceiptFlg.SelectedValue = this.OrderModel.ReceiptFlg;
		this.WtbReceiptAddress.Text = this.OrderModel.ReceiptAddress;
		this.WtbReceiptProviso.Text = this.OrderModel.ReceiptProviso;

		// 宛名・但し書き入力項目表示の制御
		ddlReceiptFlg_OnSelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// 領収書希望有無ドロップダウンリスト変更イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected new void ddlReceiptFlg_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		// 宛名・但し書き入力項目表示の制御
		this.WtrReceiptAddressInput.Visible
			= this.WtrReceiptProvisoInput.Visible
			= this.WdvReceiptAddressProvisoInput.Visible
			= (this.WddlReceiptFlg.SelectedValue == Constants.FLG_ORDER_RECEIPT_FLG_ON);
	}

	/// <summary>
	/// 領収書情報 情報更新ボタン クリックイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbUpdateReceiptInfo_Click(object sender, EventArgs e)
	{
		// 領収書対応OPが無効、または、ユーザではない場合、何も処理しない
		if ((Constants.RECEIPT_OPTION_ENABLED == false) || (this.IsLoggedIn == false)) return;

		// 領収書情報更新可能かをチェック
		var order = new OrderService().Get(this.OrderModel.OrderId);
		var errorMessage = CheckUpdateReceiptInfoError(order);
		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			this.ReceiptInfoModifyErrorMessage = errorMessage;
			return;
		}

		// 入力値取得
		var hasReceipt = (this.WddlReceiptFlg.SelectedValue == Constants.FLG_ORDER_RECEIPT_FLG_ON);
		var address = StringUtility.RemoveUnavailableControlCode(this.WtbReceiptAddress.Text.Trim());
		var proviso = StringUtility.RemoveUnavailableControlCode(this.WtbReceiptProviso.Text.Trim());

		// 領収書希望ありの時に、宛名と但し書きの入力チェックを行う
		if (hasReceipt)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_ORDER_RECEIPT_ADDRESS, address },
				{ Constants.FIELD_ORDER_RECEIPT_PROVISO, proviso },
			};
			var errorMessages = Validator.ValidateAndGetErrorContainer("ReceiptRegisterModify", input);
			if (errorMessages.Count != 0)
			{
				// カスタムバリデータ取得
				var customValidators = new List<CustomValidator>();
				CreateCustomValidators(this, customValidators);
				// エラーをカスタムバリデータへ
				SetControlViewsForError("ReceiptRegisterModify", errorMessages, customValidators);
				return;
			}
		}

		// 領収書情報更新※出力フラグ変更なしのため、nullを渡す
		var updated = new OrderService().UpdateOrderReceiptInfo(
			this.OrderModel.OrderId,
			hasReceipt ? Constants.FLG_ORDER_RECEIPT_FLG_ON : Constants.FLG_ORDER_RECEIPT_FLG_OFF,
			null,
			hasReceipt ? address : string.Empty,
			hasReceipt ? proviso : string.Empty,
			Constants.FLG_LASTCHANGED_USER,
			UpdateHistoryAction.Insert);
		if (updated == 0)
		{
			this.ReceiptInfoModifyErrorMessage = WebSanitizer.HtmlEncodeChangeToBr(
				CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_RECEIPT_INFO_UPDATE_ERROR));
		}
		else
		{
			ReloadOrderHistoryDetail(false);
		}
	}

	/// <summary>
	/// 配送時間帯 情報更新ボタン クリックイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbUpdateShippingTime_Click(object sender, EventArgs e)
	{
		var index = int.Parse(((LinkButton)sender).CommandArgument);
		var wddlShippingTimeList = GetWrappedControl<WrappedDropDownList>(this.WrOrderShipping.Items[index], "ddlShippingTimeList");

		// 注文情報（変更後）取得
		var orderNew = new OrderService().Get(this.OrderModel.OrderId);
		orderNew.Shippings[index].ShippingTime = wddlShippingTimeList.SelectedValue.Trim();

		// 配送希望更新（更新履歴とともに）
		if (UpdateShippingDateTime(orderNew, UpdateHistoryAction.Insert))
		{
			// メール送信
			SendMailCommon.SendModifyPurchaseHistoryMail(this.OrderModel.OrderId, SendMailCommon.PurchaseHistoryModify.ShippingDate);
			ReloadOrderHistoryDetail();
		}
		else
		{
			var wsErrorMessageShippingTime = GetWrappedControl<WrappedHtmlGenericControl>(this.WrOrderShipping.Items[index], "sErrorMessageShippingTime");
			wsErrorMessageShippingTime.InnerHtml = WebSanitizer.HtmlEncodeChangeToBr(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_ORDER_INSERT_ERROR));
		}
	}

	/// <summary>
	/// 配送時間帯 キャンセルボタン クリックイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbHideShippingTime_Click(object sender, EventArgs e)
	{
		var index = int.Parse(((LinkButton)sender).CommandArgument);
		var wddlShippingTimeList = GetWrappedControl<WrappedDropDownList>(this.WrOrderShipping.Items[index], "ddlShippingTimeList");
		var wdvShippingTimeText = GetWrappedControl<WrappedHtmlGenericControl>(this.WrOrderShipping.Items[index], "dvShippingTimeText");
		var wtrShippingTimeInput = GetWrappedControl<WrappedHtmlGenericControl>(this.WrOrderShipping.Items[index], "trShippingTimeInput");
		wddlShippingTimeList.Visible = (wddlShippingTimeList.Visible == false);
		wdvShippingTimeText.Visible = (wdvShippingTimeText.Visible == false);
		wtrShippingTimeInput.Visible = (wtrShippingTimeInput.Visible == false);
	}

	/// <summary>
	/// 配送希望日・時間帯の更新処理
	/// </summary>
	/// <param name="orderNew">変更後の注文モデル</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <returns>結果</returns>
	protected bool UpdateShippingDateTime(OrderModel orderNew, UpdateHistoryAction updateHistoryAction)
	{
		using (var accessor = new SqlAccessor())
		{
			// トランザクション開始
			accessor.OpenConnection();
			accessor.BeginTransaction();
			try
			{
				// 注文情報更新
				if (ExecuteUpdateOrderAndRegisterUpdateHistory(orderNew, UpdateHistoryAction.DoNotInsert, accessor) == false)
				{
					throw new Exception("注文情報の更新に失敗しました。");
				}

				// 配送先の更新
				if (new OrderService().UpdateShippingForModify(
					orderNew.Shippings,
					orderNew.LastChanged,
					UpdateHistoryAction.DoNotInsert,
					accessor) == 0)
				{
					throw new Exception("注文配送先の更新に失敗しました。");
				}

				// 更新履歴登録
				if (updateHistoryAction == UpdateHistoryAction.Insert)
				{
					new UpdateHistoryService().InsertForOrder(orderNew.OrderId, orderNew.LastChanged, accessor);
				}

				accessor.CommitTransaction();
			}
			catch (Exception ex)
			{
				AppLogger.WriteError(ex);
				accessor.RollbackTransaction();
				return false;
			}
			return true;
		}
	}

	/// <summary>
	/// 都道府県の項目変更時のChangeイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlShippingAddr1_SelectedIndexChanged(object sender, EventArgs e)
	{
		var index = int.Parse(((DropDownList)sender).DataTextField);
		ChangeShippingPriceView(index);
	}

	/// <summary>
	/// 表示コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		//クレジットカードフォームのコンポーネントをセット
		SetCreditCardComponents(
			this.OrderModel.OrderPaymentKbn,
			this.OrderModel.CreditBranchNo.ToString(),
			this.OrderModel.CardInstallmentsCode);

		// 都道府県、国の選択肢の設定
		foreach (RepeaterItem rOrderShippingItem in this.WrOrderShipping.Items)
		{
			var WddlShippingAddr1 = GetWrappedControl<WrappedDropDownList>(rOrderShippingItem, "ddlShippingAddr1");
			var wlOrderHistoryErrorMessage = GetWrappedControl<WrappedLabel>(rOrderShippingItem, "lOrderHistoryErrorMessage");
			foreach (string strPrefecture in Constants.STR_PREFECTURES_LIST)
			{
				WddlShippingAddr1.AddItem(new ListItem(strPrefecture));
			}

			var shippingAvailableCountries = new CountryLocationService().GetShippingAvailableCountry();
			var WddlShippingCountry = GetWrappedControl<WrappedDropDownList>(rOrderShippingItem, "ddlShippingCountry");
			WddlShippingCountry.Items.Add(new ListItem("", ""));
			WddlShippingCountry.Items.AddRange(shippingAvailableCountries.Select(c => new ListItem(c.CountryName, c.CountryIsoCode)).ToArray());

			var WddlShippingAddr5 = GetWrappedControl<WrappedDropDownList>(rOrderShippingItem, "ddlShippingAddr5");
			WddlShippingAddr5.Items.Add(new ListItem("", ""));
			WddlShippingAddr5.Items.AddRange(Constants.US_STATES_LIST.Select(state => new ListItem(state)).ToArray());

			// Display update shipping error message
			if (string.IsNullOrEmpty(this.OrderHistoryErrorMessage))
			{
				wlOrderHistoryErrorMessage.Visible = false;
				wlOrderHistoryErrorMessage.Text = string.Empty;
			}
			else
			{
				wlOrderHistoryErrorMessage.Visible = true;
				wlOrderHistoryErrorMessage.Text = this.OrderHistoryErrorMessage;
			}
		}

		// 合計金額
		this.WhfTotalPrice.Value = this.OrderModel.OrderPriceTotal.ToString();
		this.WhfPaymentTotalPriceNew.Value = this.OrderModel.OrderPriceTotal.ToString();
		this.WhfShippingTotalPriceNew.Value = this.OrderModel.OrderPriceTotal.ToString();

		// 定期購入履歴の注文IDリスト
		this.SimilarShippingOrderIdList = new string[0];

		switch (this.OrderModel.OrderPaymentKbn)
		{
			case Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY:
				SetDisabledButton(
					Constants.FLG_PAYMENT_TYPE_ECPAY_CREDIT,
					WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CHANGE_INFORMATION_FOR_ECPAY));
				break;

			case Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY:
				SetDisabledButton(
					Constants.FLG_PAYMENT_TYPE_NEWEBPAY_CREDIT,
					WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CHANGE_INFORMATION_FOR_NEWEBPAY));
				break;
		}

		// Display Taiwan Address
		DisplayTaiwanAddress();

		var orderExtend = OrderExtendCommon.ConvertOrderExtend(this.OrderModel);
		this.WrOrderExtendDisplay.DataSource = new OrderExtendInput(OrderExtendInput.UseType.Modify, orderExtend).OrderExtendItems;
		this.WrOrderExtendDisplay.DataBind();

		// 領収書出力エラーメッセージを表示
		DisplayReceiptDownloadError();
	}

	/// <summary>
	/// ページの再読み込み
	/// <param name="isScrollToShippingArea">届け先情報の部分へ移動か</param>
	/// </summary>
	private void ReloadOrderHistoryDetail(bool isScrollToShippingArea = true)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_HISTORY_DETAIL)
			.AddParam(Constants.REQUEST_KEY_ORDER_ID, this.OrderModel.OrderId)
			.CreateUrl();

		this.NeedScrollToShippingArea = isScrollToShippingArea;
		Response.Redirect(url);
	}

	/// <summary>
	/// 配送先変更時のアラート表示する合計金額の更新
	/// </summary>
	/// <param name="index">リピーターインデックス</param>
	private void ChangeShippingPriceView(int index)
	{
		var shippings = GetInputShippings(index);
		var prefectures = new List<string>(Constants.STR_PREFECTURES_LIST);
		// 配送先入力情報更新後のカート情報取得して再計算
		var cart = CreateCartAfterShippingAddr(index, shippings[index]);
		cart.Calculate(false);
		var orderPriceTotal = (CheckSetPromotionShippingFree(this.OrderModel))
						? this.OrderModel.OrderPriceTotal
						: this.OrderModel.OrderPriceTotal - this.OrderModel.OrderPriceShipping + cart.PriceShipping;
		this.WhfShippingTotalPriceNew.Value = orderPriceTotal.ToString();
	}

	/// <summary>
	/// 現在入力されている配送先を取得
	/// </summary>
	/// <returns>現在入力されている配送先</returns>
	private OrderShippingModel[] GetInputShippings(int index)
	{
		var wtbShippingName1 = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingName1");
		var wtbShippingName2 = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingName2");
		var wtbShippingNameKana1 = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingNameKana1");
		var wtbShippingNameKana2 = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingNameKana2");
		var wddlShippingCountry = GetWrappedControl<WrappedDropDownList>(this.WrOrderShipping.Items[index], "ddlShippingCountry");
		var wtbShippingZip1 = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingZip1");
		var wtbShippingZip2 = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingZip2");
		var wtbShippingZipGlobal = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingZipGlobal");
		var wddlShippingAddr1 = GetWrappedControl<WrappedDropDownList>(this.WrOrderShipping.Items[index], "ddlShippingAddr1");
		var wtbShippingAddr2 = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingAddr2");
		var wtbShippingAddr3 = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingAddr3");
		var wtbShippingAddr4 = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingAddr4");
		var wddlShippingAddr5 = GetWrappedControl<WrappedDropDownList>(this.WrOrderShipping.Items[index], "ddlShippingAddr5");
		var wtbShippingAddr5 = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingAddr5");
		var wtbShippingCompanyName = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingCompanyName");
		var wtbShippingCompanyPostName = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingCompanyPostName");
		var wtbShippingTel1 = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingTel1_1");
		var wtbShippingTel2 = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingTel1_2");
		var wtbShippingTel3 = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingTel1_3");
		var wtbShippingTel1Global = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingTel1Global");
		var wddlShippingAddr2 = GetWrappedControl<WrappedDropDownList>(this.WrOrderShipping.Items[index], "ddlShippingAddr2");
		var wddlShippingAddr3 = GetWrappedControl<WrappedDropDownList>(this.WrOrderShipping.Items[index], "ddlShippingAddr3");
		var whfCvsShopId = GetWrappedControl<WrappedHiddenField>(this.WrOrderShipping.Items[index], "hfCvsShopId");
		var whfCvsShopName = GetWrappedControl<WrappedHiddenField>(this.WrOrderShipping.Items[index], "hfCvsShopName");
		var whfCvsShopAddress = GetWrappedControl<WrappedHiddenField>(this.WrOrderShipping.Items[index], "hfCvsShopAddress");
		var whfCvsShopTel = GetWrappedControl<WrappedHiddenField>(this.WrOrderShipping.Items[index], "hfCvsShopTel");
		var wddlShippingType = GetWrappedControl<WrappedDropDownList>(this.WrOrderShipping.Items[index], "ddlShippingType");
		var wrOrderShippingList = GetWrappedControl<WrappedRepeater>(this.WrOrderShipping.Items[index], "rOrderShippingList");
		var wddlShippingReceivingStoreType = GetWrappedControl<WrappedDropDownList>(this.WrOrderShipping.Items[index], "ddlShippingReceivingStoreType");
		var wtbShippingZip = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingZip");
		var wtbShippingTel = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingTel1");

		var orderOld = new OrderService().Get(this.OrderModel.OrderId);

		var shippingAddrCountryIsoCode = wddlShippingCountry.SelectedValue;
		var isShippingAddrJp = IsCountryJp(shippingAddrCountryIsoCode);
		var isShippingAddrTw = IsCountryTw(shippingAddrCountryIsoCode);

		var shippings = (OrderShippingModel[])orderOld.Shippings.Clone();
		shippings[index].OrderId = this.OrderModel.OrderId;
		shippings[index].OrderShippingNo = this.WrOrderShipping.Items[index].ItemIndex + 1;

		if (string.IsNullOrEmpty(wddlShippingType.SelectedValue))
		{
			wddlShippingType.SelectedValue = CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW;
		}

		switch (wddlShippingType.SelectedValue)
		{
			case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER:
				shippings[index].ShippingName1 = this.LoginUser.Name1;
				shippings[index].ShippingName2 = this.LoginUser.Name2;
				shippings[index].ShippingName = this.LoginUser.Name;
				shippings[index].ShippingNameKana1 = this.LoginUser.NameKana1;
				shippings[index].ShippingNameKana2 = this.LoginUser.NameKana2;
				shippings[index].ShippingNameKana = this.LoginUser.NameKana;
				shippings[index].ShippingZip = this.LoginUser.Zip;
				shippings[index].ShippingAddr1 = this.LoginUser.Addr1;
				shippings[index].ShippingAddr2 = this.LoginUser.Addr2;
				shippings[index].ShippingAddr3 = this.LoginUser.Addr3;
				shippings[index].ShippingAddr4 = this.LoginUser.Addr4;
				shippings[index].ShippingAddr5 = this.LoginUser.Addr5;
				shippings[index].ShippingCompanyName = this.LoginUser.CompanyName;
				shippings[index].ShippingCompanyPostName = this.LoginUser.CompanyPostName;
				shippings[index].ShippingTel1 = this.LoginUser.Tel1;
				shippings[index].ShippingCountryName = this.LoginUser.AddrCountryName;
				shippings[index].ShippingCountryIsoCode = this.LoginUser.AddrCountryIsoCode;
				shippings[index].ShippingReceivingStoreFlg = Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF;
				return shippings;
			case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW:
				shippings[index].ShippingName1 = DataInputUtility.ConvertToFullWidthBySetting(wtbShippingName1.Text.Trim());
				shippings[index].ShippingName2 = DataInputUtility.ConvertToFullWidthBySetting(wtbShippingName2.Text.Trim());
				shippings[index].ShippingName = shippings[index].ShippingName1 + shippings[index].ShippingName2;
				shippings[index].ShippingNameKana1 = StringUtility.ToZenkaku(wtbShippingNameKana1.Text.Trim());
				shippings[index].ShippingNameKana2 = StringUtility.ToZenkaku(wtbShippingNameKana2.Text.Trim());
				shippings[index].ShippingNameKana = shippings[index].ShippingNameKana1 + shippings[index].ShippingNameKana2;

				// Set value for zip code
				var inputZipCode = (wtbShippingZip1.HasInnerControl)
					? StringUtility.ToHankaku(wtbShippingZip1.Text.Trim())
					: StringUtility.ToHankaku(wtbShippingZip.Text.Trim());
				if (wtbShippingZip1.HasInnerControl) inputZipCode += ("-" + StringUtility.ToHankaku(wtbShippingZip2.Text.Trim()));
				var zipCode = new ZipCode(inputZipCode);
				var shippingZip = isShippingAddrJp
					? (string.IsNullOrEmpty(zipCode.Zip) == false)
						? zipCode.Zip
						: inputZipCode
					: string.Empty;

				shippings[index].ShippingZip = shippingZip;

				shippings[index].ShippingAddr1 = isShippingAddrJp
					? wddlShippingAddr1.SelectedValue
					: string.Empty;

				// Taiwan address
				var shippingAddr2 = (isShippingAddrTw && wddlShippingAddr2.HasInnerControl)
					? wddlShippingAddr2.SelectedValue
					: wtbShippingAddr2.Text.Trim();
				var shippingAddr3 = (isShippingAddrTw && wddlShippingAddr3.HasInnerControl)
					? wddlShippingAddr3.SelectedText
					: wtbShippingAddr3.Text.Trim();

				shippings[index].ShippingAddr2 = DataInputUtility.ConvertToFullWidthBySetting(shippingAddr2, isShippingAddrJp);
				shippings[index].ShippingAddr3 = DataInputUtility.ConvertToFullWidthBySetting(shippingAddr3, isShippingAddrJp);
				shippings[index].ShippingAddr4 = DataInputUtility.ConvertToFullWidthBySetting(wtbShippingAddr4.Text.Trim(), isShippingAddrJp);

				shippings[index].ShippingCompanyName = DataInputUtility.ConvertToFullWidthBySetting(wtbShippingCompanyName.Text.Trim(), isShippingAddrJp);
				shippings[index].ShippingCompanyPostName = DataInputUtility.ConvertToFullWidthBySetting(wtbShippingCompanyPostName.Text.Trim(), isShippingAddrJp);

				// Set value for telephone
				var inputTel = (wtbShippingTel1.HasInnerControl)
					? StringUtility.ToHankaku(wtbShippingTel1.Text)
					: StringUtility.ToHankaku(wtbShippingTel.Text);
				if (wtbShippingTel1.HasInnerControl)
				{
					inputTel = UserService.CreatePhoneNo(
						inputTel,
						StringUtility.ToHankaku(wtbShippingTel2.Text.Trim()),
						StringUtility.ToHankaku(wtbShippingTel3.Text.Trim()));
				}
				var tel = new Tel(inputTel);
				var shippingTel1 = isShippingAddrJp
					? (string.IsNullOrEmpty(tel.TelNo) == false)
						? tel.TelNo
						: inputTel
					: StringUtility.ToHankaku(wtbShippingTel1Global.Text.Trim());
				shippings[index].ShippingTel1 = shippingTel1;

				if (Constants.GLOBAL_OPTION_ENABLE)
				{
					shippings[index].ShippingCountryName = wddlShippingCountry.SelectedText;
					shippings[index].ShippingCountryIsoCode = wddlShippingCountry.SelectedValue;

					if (isShippingAddrJp == false)
					{
						shippings[index].ShippingAddr5 = IsCountryUs(shippingAddrCountryIsoCode)
							? wddlShippingAddr5.SelectedText
							: DataInputUtility.ConvertToFullWidthBySetting(wtbShippingAddr5.Text.Trim(), isShippingAddrJp);
						shippings[index].ShippingZip = StringUtility.ToHankaku(wtbShippingZipGlobal.Text.Trim());

						shippings[index].ShippingNameKana1 = string.Empty;
						shippings[index].ShippingNameKana2 = string.Empty;
						shippings[index].ShippingNameKana = string.Empty;
					}
				}
				shippings[index].ShippingReceivingStoreFlg = Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF;
				return shippings;
			case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE:
				shippings[index].ShippingReceivingStoreId = whfCvsShopId.Value;
				shippings[index].ShippingName = whfCvsShopName.Value;
				shippings[index].ShippingAddr4 = whfCvsShopAddress.Value;
				shippings[index].ShippingTel1 = whfCvsShopTel.Value;
				shippings[index].ShippingReceivingStoreFlg = Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON;
				shippings[index].ShippingReceivingStoreType = wddlShippingReceivingStoreType.SelectedValue;
				return shippings;

			default:
				var selectedIndexShippingNo = this.UserShippingAddr.ToList().FindIndex(item => item.ShippingNo == int.Parse(wddlShippingType.SelectedValue));
				whfCvsShopId = GetWrappedControl<WrappedHiddenField>(wrOrderShippingList.Items[selectedIndexShippingNo], "hfCvsShopId");
				whfCvsShopName = GetWrappedControl<WrappedHiddenField>(wrOrderShippingList.Items[selectedIndexShippingNo], "hfCvsShopName");
				whfCvsShopAddress = GetWrappedControl<WrappedHiddenField>(wrOrderShippingList.Items[selectedIndexShippingNo], "hfCvsShopAddress");
				whfCvsShopTel = GetWrappedControl<WrappedHiddenField>(wrOrderShippingList.Items[selectedIndexShippingNo], "hfCvsShopTel");

				var selectedUserShipping = this.UserShippingAddr[selectedIndexShippingNo];
				if (selectedUserShipping.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF)
				{
					shippings[index].ShippingName1 = selectedUserShipping.ShippingName1;
					shippings[index].ShippingName2 = selectedUserShipping.ShippingName2;
					shippings[index].ShippingName = selectedUserShipping.ShippingName;
					shippings[index].ShippingNameKana1 = selectedUserShipping.ShippingNameKana1;
					shippings[index].ShippingNameKana2 = selectedUserShipping.ShippingNameKana2;
					shippings[index].ShippingNameKana = selectedUserShipping.ShippingNameKana;
					shippings[index].ShippingZip = selectedUserShipping.ShippingZip;
					shippings[index].ShippingAddr1 = selectedUserShipping.ShippingAddr1;
					shippings[index].ShippingAddr2 = selectedUserShipping.ShippingAddr2;
					shippings[index].ShippingAddr3 = selectedUserShipping.ShippingAddr3;
					shippings[index].ShippingAddr4 = selectedUserShipping.ShippingAddr4;
					shippings[index].ShippingAddr5 = selectedUserShipping.ShippingAddr5;
					shippings[index].ShippingCompanyName = selectedUserShipping.ShippingCompanyName;
					shippings[index].ShippingCompanyPostName = selectedUserShipping.ShippingCompanyPostName;
					shippings[index].ShippingTel1 = selectedUserShipping.ShippingTel1;
					shippings[index].ShippingCountryName = selectedUserShipping.ShippingCountryName;
					shippings[index].ShippingCountryIsoCode = selectedUserShipping.ShippingCountryIsoCode;
					shippings[index].ShippingReceivingStoreFlg = selectedUserShipping.ShippingReceivingStoreFlg;
					shippings[index].ShippingReceivingStoreId = selectedUserShipping.ShippingReceivingStoreId;
					shippings[index].ShippingReceivingStoreType = selectedUserShipping.ShippingReceivingStoreType;
				}
				else
				{
					shippings[index].ShippingReceivingStoreId = whfCvsShopId.Value;
					shippings[index].ShippingName = whfCvsShopName.Value;
					shippings[index].ShippingAddr4 = whfCvsShopAddress.Value;
					shippings[index].ShippingTel1 = whfCvsShopTel.Value;
					shippings[index].ShippingReceivingStoreFlg = selectedUserShipping.ShippingReceivingStoreFlg;
					shippings[index].ShippingReceivingStoreType = selectedUserShipping.ShippingReceivingStoreType;
				}
				return shippings;
		}
	}

	/// <summary>
	/// ポイント利用額変更向け再与信＆注文情報更新
	/// </summary>
	/// <param name="orderOld">変更前の注文モデル</param>
	/// <param name="orderNew">変更後の注文モデル</param>
	/// <param name="isFirstBuy">初回購入か</param>
	/// <param name="pointFirstBuy">初回購入発行ポイント</param>
	/// <param name="pointOrder">購入時発行ポイント</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <returns>エラーメッセージ</returns>
	private string ReauthAndUpdateOrderForUpdatePointUse(
		OrderModel orderOld,
		OrderModel orderNew,
		bool isFirstBuy,
		decimal pointFirstBuy,
		decimal pointOrder,
		UpdateHistoryAction updateHistoryAction)
	{
		// 外部決済連携実行
		string apiErrorMassage;
		var isExecuteExternalPayment = ExecuteExternalPayment(orderOld, orderNew, out apiErrorMassage);

		if (string.IsNullOrEmpty(apiErrorMassage) == false)
		{
			this.ApiErrorMessage = apiErrorMassage;

			if (this.PaymentModel.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
			{
				return WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CARDAUTH_ERROR);
			}
			else if ((this.PaymentModel.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
				|| (this.PaymentModel.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_SMS_DEF))
			{
				return WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CVSAUTH_ERROR);
			}
			else if (this.PaymentModel.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
			{
				return WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_TRYLINKAFTERPAYAUTH_ERROR);
			}
			else if (this.PaymentModel.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
			{
				var errorPaymentMessage =
					apiErrorMassage.Contains(NPAfterPayUtility.GetErrorMessages(Constants.FLG_PAYMENT_NP_AFTERPAY_CUSTOM_ERROR_CODE_2))
						? CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_AUTH_ERROR)
						: apiErrorMassage;
				return errorPaymentMessage;
			}
			else if (this.PaymentModel.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU)
			{
				return apiErrorMassage;
			}
			else
			{
				return WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_AUTH_EXCEPTION);
			}
		}

		using (var accessor = new SqlAccessor())
		{
			// トランザクション開始
			accessor.OpenConnection();
			accessor.BeginTransaction();
			try
			{
				orderNew.LastChanged = Constants.FLG_LASTCHANGED_USER;
				// 注文情報更新
				if (ExecuteUpdateOrderAndRegisterUpdateHistory(orderNew, UpdateHistoryAction.DoNotInsert, accessor, isExecuteExternalPayment, isChangePoint: true) == false)
				{
					throw new Exception("注文情報の更新に失敗しました。");
				}
				var updatedOrderPriceInfoByTaxRate =
					new OrderService().UpdateOrderPriceInfoByTaxRateModify(orderNew.OrderPriceByTaxRates, accessor);
				if (updatedOrderPriceInfoByTaxRate == 0)
				{
					throw new Exception("税率毎価格情報の更新に失敗しました。");
				}
				ExecUpdateUserPoint(orderOld, orderNew, isFirstBuy, pointFirstBuy, pointOrder, updateHistoryAction, accessor);
			}
			catch (Exception ex)
			{
				AppLogger.WriteError(ex);
				accessor.RollbackTransaction();
				return WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_ORDER_INSERT_ERROR);
			}
		}
		return "";
	}

	/// <summary>
	/// ポイント利用額変更向け再与信＆注文情報更新
	/// </summary>
	/// <param name="orderOld">変更前の注文モデル</param>
	/// <param name="orderNew">変更後の注文モデル</param>
	/// <param name="isFirstBuy">初回購入か</param>
	/// <param name="pointFirstBuy">初回購入発行ポイント</param>
	/// <param name="pointOrder">購入時発行ポイント</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <returns>エラーメッセージ</returns>
	private string ExecUpdateUserPoint(
		OrderModel orderOld,
		OrderModel orderNew,
		bool isFirstBuy,
		decimal pointFirstBuy,
		decimal pointOrder,
		UpdateHistoryAction updateHistoryAction,
		SqlAccessor accessor)
	{
		try
		{
			// ユーザーの所有ポイントを更新
			if ((orderOld.OrderPointUse > 0) || (orderNew.OrderPointUse > 0))
			{
				var updated = new PointService().RecalcOrderUsePoint(
					orderOld.UserId,
					orderOld.OrderId,
					orderNew.OrderId,
					orderNew.OrderPointUse,
					Constants.FLG_LASTCHANGED_USER,
					UpdateHistoryAction.DoNotInsert,
					accessor);
				if (updated == 0)
				{
					throw new Exception("ユーザーの所有ポイント更新に失敗しました。");
				}
			}

			if (orderOld.OrderPointAdd != orderNew.OrderPointAdd)
			{
				// 付与仮ポイントの変更
				var isUpdateUserPointSuccess =
					new PointService().AdjustOrderPointAddForPointTempAtUsePointChange(
						StringUtility.ToEmpty(orderNew.UserId),
						orderNew.OrderId,
						isFirstBuy,
						pointFirstBuy,
						pointOrder,
						orderNew.LastChanged,
						UpdateHistoryAction.DoNotInsert,
						accessor) > 0;
				if (isUpdateUserPointSuccess == false) throw new Exception("付与ポイントの変更に失敗しました。");
			}

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForOrder(orderNew.OrderId, orderNew.LastChanged, accessor);
				new UpdateHistoryService().InsertForUser(orderNew.UserId, orderNew.LastChanged, accessor);
			}

			// トランザクションコミット
			accessor.CommitTransaction();
		}
		catch (Exception ex)
		{
			AppLogger.WriteError(ex);
			accessor.RollbackTransaction();
			return WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_ORDER_INSERT_ERROR);
		}
		return string.Empty;
	}

	/// <summary>
	/// ユーザのポイント更新
	/// </summary>
	/// <param name="orderOld">変更前の注文モデル</param>
	/// <param name="orderNew">変更後の注文モデル</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <returns>更新成功:true 更新失敗:false</returns>
	private bool UpdateUserPoint(
		OrderModel orderOld,
		OrderModel orderNew,
		UpdateHistoryAction updateHistoryAction,
		SqlAccessor accessor)
	{
		var updated = new PointService().RecalcOrderUsePoint(
			orderOld.UserId,
			orderOld.OrderId,
			orderNew.OrderId,
			orderNew.OrderPointUse,
			Constants.FLG_LASTCHANGED_USER,
			updateHistoryAction,
			accessor);

		// 登録できていれば成功
		return updated > 0;
	}

	/// <summary>
	/// 利用ポイント数変更後の金額再計算
	/// </summary>
	/// <param name="orderOld">変更前の注文モデル</param>
	/// <param name="orderNew">変更後の注文モデル</param>
	/// <returns>利用ポイントが商品小計以内:true 超過:false</returns>
	private bool CalculatePriceForOrderPointUse(OrderModel orderOld, OrderModel orderNew)
	{
		var orderPointUsablePrice = OrderCommon.GetOrderPointUsable(
				orderNew.OrderPriceSubtotal,
				orderNew.OrderPriceRegulationTotal,
				orderNew.MemberRankDiscountPrice,
				orderNew.OrderCouponUse,
				orderNew.SetpromotionProductDiscountAmount,
				orderNew.FixedPurchaseMemberDiscountAmount,
				orderNew.FixedPurchaseDiscountPrice);

		orderNew.OrderPriceTotal = orderOld.OrderPriceTotal + orderOld.OrderPointUse - orderNew.OrderPointUse;
		orderNew.LastBilledAmount = orderNew.OrderPriceTotal;

		// 利用ポイントチェック
		if (orderNew.OrderPointUseYen > orderPointUsablePrice)
		{
			this.ErrorMessageOrderPointUse = WebMessages.GetMessages(
				WebMessages.ERRMSG_FRONT_POINT_PRICE_SUBTOTAL_MINUS_ERROR).Replace("@@ 1 @@", StringUtility.ToPrice(orderPointUsablePrice));
			return false;
		}
		return true;
	}

	/// <summary>
	/// 領収書情報更新可能かをチェック
	/// </summary>
	/// <param name="order">チェック対象注文</param>
	/// <returns>エラーメッセージ</returns>
	private string CheckUpdateReceiptInfoError(OrderModel order)
	{
		var errorMessage = string.Empty;

		if (order == null)
		{
			errorMessage = WebSanitizer.HtmlEncodeChangeToBr(
				CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_RECEIPT_INFO_UPDATE_ERROR));
			return errorMessage;
		}

		if (order.ReceiptOutputFlg == Constants.FLG_ORDER_RECEIPT_OUTPUT_FLG_ON)
		{
			errorMessage = WebSanitizer.HtmlEncodeChangeToBr(
				CommerceMessages.GetMessages(
					CommerceMessages.ERRMSG_FRONT_RECEIPT_INFO_UPDATE_ERROR_SETTLED_RECEIPTDOWNLOAD));
			return errorMessage;
		}

		if (Constants.NOT_OUTPUT_RECEIPT_PAYMENT_KBN.Contains(order.OrderPaymentKbn))
		{
			errorMessage = WebSanitizer.HtmlEncodeChangeToBr(
				CommerceMessages.GetMessages(
					CommerceMessages.ERRMSG_FRONT_RECEIPT_INFO_UPDATE_ERROR_NOT_OUTPUT_RECEIPT_PAYMENT_KBN));
			return errorMessage;
		}

		return errorMessage;
	}

	#region #ddlShippingCountry_SelectedIndexChanged 配送先国ドロップダウンリスト変更時イベント
	/// <summary>
	/// 配送先国ドロップダウンリスト変更時イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks>そのままだとOrderCartPageクラスのメソッドを参照してしまうので</remarks>
	protected new void ddlShippingCountry_SelectedIndexChanged(object sender, EventArgs e)
	{
		foreach (RepeaterItem ri in this.WrOrderShipping.Items)
		{
			ddlShippingCountry_SelectedIndexChangedInner(ri);
		}

		if (sender is DropDownList)
		{
			var index = int.Parse(((DropDownList)sender).DataTextField);
			ChangeShippingPriceView(index);
		}

		var isCountryTaiwan = IsCountryTw(this.OrderModel.Shippings[0].ShippingCountryIsoCode);
		var ddlShippingAddr2 = GetWrappedControl<WrappedDropDownList>(this.WrOrderShipping.Items.Cast<RepeaterItem>().ToList()[0], "ddlShippingAddr2");
		var ddlShippingAddr3 = GetWrappedControl<WrappedDropDownList>(this.WrOrderShipping.Items.Cast<RepeaterItem>().ToList()[0], "ddlShippingAddr3");
		if (isCountryTaiwan && ddlShippingAddr2.HasInnerControl && ddlShippingAddr3.HasInnerControl)
		{
			this.Process.BindingDdlShippingAddr3(ddlShippingAddr2);
		}
	}
	#endregion

	/// <summary>
	/// 配送先住所国ISOコード取得
	/// </summary>
	/// <param name="shippingIndex">配送先インデックス</param>
	/// <returns>ISOコード</returns>
	protected string GetShippingAddrCountryIsoCode(int shippingIndex)
	{
		var wddlCountry = GetWrappedControl<WrappedDropDownList>(
			this.WrOrderShipping.Items[shippingIndex],
			"ddlShippingCountry");
		return wddlCountry.SelectedValue;
	}

	/// <summary>
	/// 表示通貨価格取得
	/// </summary>
	/// <param name="price">価格</param>
	/// <returns>表示通貨価格</returns>
	[WebMethod]
	public static string GetPriceString(string price)
	{
		return CurrencyManager.ToPrice(price);
	}

	/// <summary>
	/// 出荷できるか確認
	/// </summary>
	/// <param name="model">注文配送先モデル</param>
	/// <returns>出荷できるか</returns>
	protected bool CheckShippingDate(OrderShippingModel model)
	{
		return ((model.ShippingDate.HasValue == false)
			|| OrderCommon.CanCalculateScheduledShippingDate(this.ShopId, model));
	}

	/// <summary>
	/// Set Disabled Button
	/// </summary>
	/// <param name="externalPaymentType">External Payment Type</param>
	/// <param name="message">Message</param>
	private void SetDisabledButton(string externalPaymentType, string message)
	{
		// Disable Update Point
		if (this.OrderModel.ExternalPaymentType != externalPaymentType)
		{
			this.WlbDisplayInputOrderPointUse.InnerControl.Enabled = false;
			this.WslErrorMessageChangePointUse.InnerHtml =
				message.Replace("@@ 1 @@", this.OrderModel.ExternalPaymentType);
		}

		// Disable Update Payment
		if (this.OrderModel.ExternalPaymentStatus
			== Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP)
		{
			this.WlbDisplayInputOrderPaymentKbn.InnerControl.Enabled = false;
		}
	}

	/// <summary>
	/// Display Taiwan Address
	/// </summary>
	private void DisplayTaiwanAddress()
	{
		var isCountryTaiwan = IsCountryTw(this.OrderModel.Shippings[0].ShippingCountryIsoCode);
		var ddlShippingAddr2 = GetWrappedControl<WrappedDropDownList>(this.WrOrderShipping.Items.Cast<RepeaterItem>().ToList()[0], "ddlShippingAddr2");
		var ddlShippingAddr3 = GetWrappedControl<WrappedDropDownList>(this.WrOrderShipping.Items.Cast<RepeaterItem>().ToList()[0], "ddlShippingAddr3");
		if (isCountryTaiwan
			&& ddlShippingAddr2.HasInnerControl
			&& ddlShippingAddr3.HasInnerControl)
		{
			ddlShippingAddr2.SelectedValue = this.OrderModel.Shippings[0].ShippingAddr2;

			ddlShippingAddr3.DataSource = this.UserTwDistrictDict[ddlShippingAddr2.SelectedItem.ToString()];
			ddlShippingAddr3.DataBind();
			ddlShippingAddr3.ForceSelectItemByText(this.OrderModel.Shippings[0].ShippingAddr3);
		}
	}

	/// <summary>
	/// 請求書同梱フラグ判定
	/// </summary>
	/// <param name="paymentId">決済種別ID</param>
	/// <param name="order">注文情報</param>
	/// <returns>請求書同梱フラグ</returns>
	private static string JudgementInvoiceBundleFlg(string paymentId, OrderModel order)
	{
		string invoiceBundleFlg;
		switch (Constants.PAYMENT_CVS_DEF_KBN)
		{
			case Constants.PaymentCvsDef.Atodene:
			case Constants.PaymentCvsDef.Gmo:
			case Constants.PaymentCvsDef.Atobaraicom:
			case Constants.PaymentCvsDef.Score:
				invoiceBundleFlg = OrderCommon.IsInvoiceBundleServiceUsable(paymentId)
					? OrderCommon.JudgmentInvoiceBundleFlg(order)
					: Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF;
				break;

			default:
				invoiceBundleFlg = Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF;
				break;
		}

		if ((string.IsNullOrEmpty(paymentId) == false)
			&& (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY))
		{
			invoiceBundleFlg = OrderCommon.IsInvoiceBundleServiceUsable(paymentId)
				? OrderCommon.JudgmentInvoiceBundleFlg(order)
				: Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF;
		}

		return invoiceBundleFlg;
	}

	/// <summary>
	/// Shipping Type Selected Changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlShippingType_SelectedIndexChanged(object sender, EventArgs e)
	{
		var control = (Control)sender;
		while ((control is RepeaterItem) == false)
		{
			control = control.Parent;
		}

		var repeaterItem = (RepeaterItem)control;
		var wrOrderShippingList = GetWrappedControl<WrappedRepeater>(this.WrOrderShipping.Items.Cast<RepeaterItem>().ToList()[repeaterItem.ItemIndex], "rOrderShippingList");
		var wtbOwnerAddress = GetWrappedControl<WrappedHtmlGenericControl>(this.WrOrderShipping.Items.Cast<RepeaterItem>().ToList()[repeaterItem.ItemIndex], "tbOwnerAddress");
		var wdivShippingInputFormInner = GetWrappedControl<WrappedHtmlGenericControl>(this.WrOrderShipping.Items.Cast<RepeaterItem>().ToList()[repeaterItem.ItemIndex], "divShippingInputFormInner");
		var wdivConvenienceStore = GetWrappedControl<WrappedHtmlGenericControl>(this.WrOrderShipping.Items.Cast<RepeaterItem>().ToList()[repeaterItem.ItemIndex], "divConvenienceStore");
		var wspConvenienceStoreSelect = GetWrappedControl<WrappedHtmlGenericControl>(this.WrOrderShipping.Items.Cast<RepeaterItem>().ToList()[repeaterItem.ItemIndex], "spConvenienceStoreSelect");
		var whfCvsShopFlg = GetWrappedControl<WrappedHiddenField>(this.WrOrderShipping.Items[repeaterItem.ItemIndex], "hfCvsShopFlg");
		var wspConvenienceStoreEcPaySelect = GetWrappedControl<WrappedHtmlGenericControl>(
			this.WrOrderShipping.Items.Cast<RepeaterItem>().ToList()[repeaterItem.ItemIndex],
			"spConvenienceStoreEcPaySelect");
		var wddlShippingReceivingStoreType = GetWrappedControl<WrappedDropDownList>(
			this.WrOrderShipping.Items.Cast<RepeaterItem>().ToList()[repeaterItem.ItemIndex],
			"ddlShippingReceivingStoreType");
		var wdvErrorShippingConvenience = GetWrappedControl<WrappedHtmlGenericControl>(
			this.WrOrderShipping.Items.Cast<RepeaterItem>().ToList()[repeaterItem.ItemIndex],
			"dvErrorPaymentAndShippingConvenience");
		wdvErrorShippingConvenience.Visible = false;

		var ddlShippingType = (DropDownList)sender;
		switch (ddlShippingType.SelectedValue)
		{
			case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW:
				wdivShippingInputFormInner.Visible = true;
				wrOrderShippingList.Visible = false;
				wtbOwnerAddress.Visible = false;
				wdivConvenienceStore.Visible = false;
				wspConvenienceStoreSelect.Visible = false;
				this.IsUpdateShippingFixedPurchase
					= ((this.FixedPurchaseModel != null) && (this.FixedPurchaseModel.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE));
				wspConvenienceStoreEcPaySelect.Visible = false;
				wddlShippingReceivingStoreType.Visible = false;
				break;

			case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER:
				wrOrderShippingList.Visible = false;
				wtbOwnerAddress.Visible = true;
				wdivShippingInputFormInner.Visible = false;
				wdivConvenienceStore.Visible = false;
				wspConvenienceStoreSelect.Visible = false;
				this.IsUpdateShippingFixedPurchase
					= ((this.FixedPurchaseModel != null) && (this.FixedPurchaseModel.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE));
				wspConvenienceStoreEcPaySelect.Visible = false;
				wddlShippingReceivingStoreType.Visible = false;
				break;

			case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE:
				wdivConvenienceStore.Visible = true;
				wrOrderShippingList.Visible = false;
				wtbOwnerAddress.Visible = false;
				wdivShippingInputFormInner.Visible = false;
				wspConvenienceStoreSelect.Visible = (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED == false);
				this.IsUpdateShippingFixedPurchase = true;
				wspConvenienceStoreEcPaySelect.Visible = Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED;
				wddlShippingReceivingStoreType.Visible = Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED;
				wddlShippingReceivingStoreType.Items.Clear();
				wddlShippingReceivingStoreType.Items.AddRange(ShippingReceivingStoreType());
				wddlShippingReceivingStoreType.Items.Cast<ListItem>().ToList()
					.ForEach(li => li.Selected = (li.Value == this.OrderModel.Shippings[repeaterItem.ItemIndex].ShippingReceivingStoreType));

				if (this.OrderModel.Shippings[repeaterItem.ItemIndex].ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF)
				{
					var whfCvsShopId = GetWrappedControl<WrappedHiddenField>(this.WrOrderShipping.Items[repeaterItem.ItemIndex], "hfCvsShopId");
					var whfCvsShopName = GetWrappedControl<WrappedHiddenField>(this.WrOrderShipping.Items[repeaterItem.ItemIndex], "hfCvsShopName");
					var whfCvsShopAddress = GetWrappedControl<WrappedHiddenField>(this.WrOrderShipping.Items[repeaterItem.ItemIndex], "hfCvsShopAddress");
					var whfCvsShopTel = GetWrappedControl<WrappedHiddenField>(this.WrOrderShipping.Items[repeaterItem.ItemIndex], "hfCvsShopTel");
					var wlCvsShopId = GetWrappedControl<WrappedLiteral>(this.WrOrderShipping.Items[repeaterItem.ItemIndex], "lCvsShopId");
					var wlCvsShopName = GetWrappedControl<WrappedLiteral>(this.WrOrderShipping.Items[repeaterItem.ItemIndex], "lCvsShopName");
					var wlCvsShopAddress = GetWrappedControl<WrappedLiteral>(this.WrOrderShipping.Items[repeaterItem.ItemIndex], "lCvsShopAddress");
					var wlCvsShopTel = GetWrappedControl<WrappedLiteral>(this.WrOrderShipping.Items[repeaterItem.ItemIndex], "lCvsShopTel");

					whfCvsShopId.Value = string.Empty;
					whfCvsShopName.Value = string.Empty;
					whfCvsShopAddress.Value = string.Empty;
					whfCvsShopTel.Value = string.Empty;
					whfCvsShopFlg.Value = Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON;

					wlCvsShopId.Text = string.Empty;
					wlCvsShopName.Text = string.Empty;
					wlCvsShopAddress.Text = string.Empty;
					wlCvsShopTel.Text = string.Empty;
				}
				break;

			default:
				wtbOwnerAddress.Visible = false;
				wdivShippingInputFormInner.Visible = false;
				wdivConvenienceStore.Visible = false;
				var whfSelectedShopId = GetWrappedControl<WrappedHiddenField>(this.WrOrderShipping.Items[repeaterItem.ItemIndex], "hfSelectedShopId");
				var shippingAddres = this.UserShippingAddr.FirstOrDefault(item => item.ShippingNo == int.Parse(ddlShippingType.SelectedValue));
				if (shippingAddres != null)
				{
					var userShippingAddress = new
					{
						ShippingName = shippingAddres.ShippingName,
						ShippingZip = shippingAddres.ShippingZip,
						ShippingAddr1 = shippingAddres.ShippingAddr1,
						ShippingAddr2 = shippingAddres.ShippingAddr2,
						ShippingAddr3 = shippingAddres.ShippingAddr3,
						ShippingAddr4 = shippingAddres.ShippingAddr4,
						ShippingAddr5 = shippingAddres.ShippingAddr5,
						ShippingCountryName = shippingAddres.ShippingCountryName,
						ShippingTel1 = shippingAddres.ShippingTel1,
						ShippingCompanyName = shippingAddres.ShippingCompanyName,
						ShippingCompanyPostName = shippingAddres.ShippingCompanyPostName
					};
					this.WhfShippingAddress.Value = new JavaScriptSerializer().Serialize(userShippingAddress);
					whfCvsShopFlg.Value = shippingAddres.ShippingReceivingStoreFlg;
					whfSelectedShopId.Value = shippingAddres.ShippingReceivingStoreId;
					wspConvenienceStoreSelect.Visible =
						((shippingAddres.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
							&& (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED == false));
					wspConvenienceStoreEcPaySelect.Visible = false;
					wddlShippingReceivingStoreType.Visible =
						((shippingAddres.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
							&& Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED);
					if ((shippingAddres.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
						&& Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED)
					{
						var shippingReceivingStoreType = shippingAddres.ShippingReceivingStoreType;
						wddlShippingReceivingStoreType.Items.Clear();
						wddlShippingReceivingStoreType.AddItem(new ListItem(
							ValueText.GetValueText(
								Constants.TABLE_ORDERSHIPPING,
								Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_TYPE,
								shippingAddres.ShippingReceivingStoreType),
							shippingReceivingStoreType));
						wdvErrorShippingConvenience.Visible = ((this.OrderModel.OrderPaymentKbn
								!= Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE)
							&& (ECPayUtility.GetIsCollection(shippingReceivingStoreType)
								== Constants.FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_COLLECTION_ON));
					}
				}

				wrOrderShippingList.Visible = true;
				foreach (RepeaterItem item in wrOrderShippingList.Items)
				{
					var shippingNo = ((HiddenField)item.FindControl("hfShippingNo")).Value;
					item.Visible = (shippingNo == ddlShippingType.SelectedValue);
				}

				if ((this.FixedPurchaseModel != null)
					&& (this.FixedPurchaseModel.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE)
					&& (whfCvsShopFlg.Value == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF))
				{
					this.IsUpdateShippingFixedPurchase = false;
				}
				else
				{
					this.IsUpdateShippingFixedPurchase = true;
				}

				break;
		}
	}

	/// <summary>
	/// Get Possible Shipping Type
	/// </summary>
	/// <param name="shippingStoreFlg">Shipping Store Flg</param>
	/// <returns>Possible Shipping Type</returns>
	protected ListItemCollection GetPossibleShippingType(string shippingStoreFlg)
	{
		var itemCollection = new ListItemCollection();
		var shippingModel = new ShopShippingService().Get(this.ShopId, this.ShopShippingModel.ShippingId);
		var userShippingAddress = new UserShippingService().GetAllOrderByShippingNoDesc(this.LoginUserId).OrderBy(item => item.ShippingNo).ToArray();

		// コンビニ受取→コンビニ受取以外(注文者の住所など)、コンビニ受取以外(注文者の住所など)→コンビニ受取という配送先の変更はさせない
		if ((shippingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
			&& Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED
			&& CheckItemRelateWithServiceConvenienceStore(shippingModel))
		{
			itemCollection.AddRange(
				userShippingAddress
					.Where(
						item => (item.ShippingReceivingStoreFlg
							== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON))
					.Select(us => new ListItem(us.Name, us.ShippingNo.ToString())).ToArray());
			itemCollection.Add(
				new ListItem(
					ReplaceTag("@@DispText.shipping_addr_kbn_list.convenience_store@@"),
					CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE));
		}
		else
		{
			itemCollection.Add(
				new ListItem(
					ReplaceTag("@@DispText.shipping_addr_kbn_list.new@@"),
					CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW));
			itemCollection.Add(
				new ListItem(
					ReplaceTag("@@DispText.shipping_addr_kbn_list.owner@@"),
					CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER));

			var userShippingNormal = userShippingAddress.Where(
				item => item.ShippingReceivingStoreFlg
					== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF);

			itemCollection.AddRange(
				userShippingNormal.Select(us => new ListItem(us.Name, us.ShippingNo.ToString())).ToArray());
		}

		return itemCollection;
	}

	/// <summary>
	/// IsDisplayButtonConvenienceStore
	/// </summary>
	/// <param name="shippingStoreFlg">Shipping Store Flg</param>
	/// <returns>Is Display or not</returns>
	protected bool IsDisplayButtonConvenienceStore(string shippingStoreFlg)
	{
		return (shippingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
			&& Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED;
	}

	/// <summary>
	/// Is Show Check Delivery Status
	/// </summary>
	/// <returns>Show Check Delivery Status</returns>
	protected bool IsShowCheckDeliveryStatus()
	{
		if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED
			&& (string.IsNullOrEmpty(this.OrderModel.Shippings[0].ShippingCheckNo) == false))
		{
			return false;
		}

		var listOrderStatus = new List<string>()
		{
			Constants.FLG_ORDER_ORDER_STATUS_TEMP,
			Constants.FLG_ORDER_ORDER_STATUS_ORDERED,
			Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED,
			Constants.FLG_ORDER_ORDER_STATUS_TEMP_CANCELED
		};
		var result = listOrderStatus.Any(status => status == this.OrderModel.OrderStatus);

		return (result == false);
	}

	/// <summary>
	/// Click open window EcPay
	/// </summary>
	/// <param name="sender">Object</param>
	/// <param name="e">Event Args</param>
	protected new void lbOpenEcPay_Click(object sender, EventArgs e)
	{
		var index = int.Parse(((LinkButton)sender).CommandArgument);

		var wddlShippingReceivingStoreType = GetWrappedControl<WrappedDropDownList>(
			(Control)sender,
			"ddlShippingReceivingStoreType");
		var shippingService = wddlShippingReceivingStoreType.SelectedValue;
		this.OrderModel.Shippings[index].ShippingReceivingStoreFlg = Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON;
		this.OrderModel.Shippings[index].ShippingReceivingStoreType = wddlShippingReceivingStoreType.SelectedValue;

		var baseUrl = string.Format(
			"{0}{1}{2}",
			Constants.PROTOCOL_HTTP,
			(string.IsNullOrEmpty(Constants.WEBHOOK_DOMAIN)
				? Constants.SITE_DOMAIN
				: Constants.WEBHOOK_DOMAIN),
			this.RawUrl);

		var uri = new Uri(baseUrl);
		var newQueryString = HttpUtility.ParseQueryString(uri.Query);
		newQueryString[Constants.REQUEST_KEY_SHIPPING_RECEIVING_STORE_TYPE] = shippingService;

		var url = string.Format("{0}?{1}", uri.GetLeftPart(UriPartial.Path), newQueryString);
		var parameters = ECPayUtility.CreateParametersForOpenConvenienceStoreMap(
			shippingService,
			url,
			this.IsSmartPhone);

		var json = JsonConvert.SerializeObject(parameters);
		var script = "NextPageSelectReceivingStore(JSON.parse('" + json + "'));";
		ScriptManager.RegisterStartupScript(
			this.Page,
			this.GetType(),
			"ReceivingStore",
			script,
			true);
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

		var result = (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED
			|| OrderCommon.CheckIdExistsInXmlStore(storeId));

		return result;
	}

	/// <summary>
	/// Check Item Relate With Service Convenience Store
	/// </summary>
	/// <param name="shopShipping"></param>
	/// <returns>True: shipping Kbn is convenience store and relate product </returns>
	protected bool CheckItemRelateWithServiceConvenienceStore(ShopShippingModel shopShipping)
	{
		var deliveryCompanyList = (this.OrderModel.Shippings[0].ShippingMethod
				== Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS)
					? shopShipping.CompanyListExpress
					: shopShipping.CompanyListMail;

		var deliveryCompanyIds = deliveryCompanyList
			.Select(model => model.DeliveryCompanyId)
			.Distinct()
			.ToArray();

		var result = deliveryCompanyIds.Any(item =>
			item == Constants.TWPELICANEXPRESS_CONVENIENCE_STORE_ID);

		return result;
	}

	/// <summary>
	/// Check Valid Tel No For Payment Atone And Aftee For Fixed Purchase
	/// </summary>
	/// <param name="cart">Cart</param>
	/// <returns>errorMessage</returns>
	private string CheckValidTelNoForPaymentAtoneAndAfteeForFixedPurchase(CartObject cart)
	{
		var fixedPurchaseService = new FixedPurchaseService();
		var fixedPurchase = fixedPurchaseService.Get(this.OrderModel.FixedPurchaseId);
		var errorMessage = string.Empty;
		// Check Country
		if (fixedPurchase != null)
		{
			switch (fixedPurchase.OrderPaymentKbn)
			{
				case Constants.FLG_PAYMENT_PAYMENT_ID_ATONE:
					var hasShippingJp = (cart.Shippings.Any(shipping => shipping.IsShippingAddrJp));
					if ((cart.Owner.IsAddrJp == false)
						|| (hasShippingJp == false))
					{
						errorMessage += WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CHECK_COUNTRY_FOR_PAYMENT_ATONE);
					}
					break;

				case Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE:
					var hasShippingTw = (cart.Shippings.Any(shipping => shipping.IsShippingAddrTw));
					if ((cart.Owner.IsAddrTw == false)
						|| (hasShippingTw == false))
					{
						errorMessage += WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CHECK_COUNTRY_FOR_PAYMENT_AFTEE);
					}
					break;
			}
		}
		return errorMessage;
	}

	/// <summary>
	/// Create Data Atone Aftee Token
	/// </summary>
	/// <param name="orderId">Order id</param>
	/// <param name="isAtone">Is Atone or Aftee</param>
	/// <param name="paymentId">Payment id</param>
	/// <returns>Data Info</returns>
	[WebMethod]
	public static string CreateDataAtoneAfteeToken(string orderId, string paymentId, bool isAtone)
	{
		//支払い情報の取得
		StringBuilder paymentErrorMessage;
		var order = new OrderService().Get(orderId);
		var cart = CartObject.CreateCartByOrder(order);
		var paymentModel =
			HistoryPage.GetAndValidatePaymentInput(
			cart,
			paymentId,
			out paymentErrorMessage);
		if ((paymentErrorMessage.Length > 0)
			|| (paymentModel == null)) return string.Empty;

		//決済手数料変更による合計金額の再計算
		var orderPriceTotal = order.OrderPriceTotal;
		var hasDiscountTypePaymentChargeFree =
			(order.SetPromotions.Any(item => item.IsDiscountTypePaymentChargeFree));
		if ((order.SetPromotions == null)
			|| (hasDiscountTypePaymentChargeFree == false))
		{
			orderPriceTotal =
				orderPriceTotal
				- order.OrderPriceExchange
				+ paymentModel.PaymentPrice;
		}

		// 注文情報（変更後）取得
		var orderOld = order;
		var orderNew = (OrderModel)orderOld.Clone();
		orderNew.ShopId = paymentModel.ShopId;
		orderNew.OrderPaymentKbn = paymentModel.PaymentId;
		orderNew.PaymentName = paymentModel.PaymentName;
		orderNew.OrderPriceExchange = paymentModel.PaymentPrice;
		orderNew.LastBilledAmount = orderPriceTotal;
		orderNew.OrderPriceTotal = orderPriceTotal;
		cart = CartObject.CreateCartByOrder(orderNew);
		cart.SettlementAmount = orderNew.OrderPriceTotal;
		var result = OrderCommon.CreateDataForAuthorizingAtoneAftee(cart, isAtone);
		return result;
	}

	/// <summary>
	/// ヤマト後払いSMS送信
	/// </summary>
	/// <returns>送信結果</returns>
	[WebMethod]
	public static string SmsSend(string telNum)
	{
		telNum = telNum.Insert(3, "-").Insert(8, "-");
		var htParam = (Hashtable)HttpContext.Current.Session[Constants.SESSION_KEY_PARAM];
		var orderNew = (OrderModel)htParam["order_new"];
		var result = ExecutePaymentYamatoKaSmsImp(orderNew, telNum);
		HttpContext.Current.Session[Constants.SESSION_KEY_SMS_TEL_NUMBER] = telNum;

		return JsonConvert.SerializeObject(
			new
			{
				result = result ? "Ok" : "Ng"
			});
	}

	/// <summary>
	/// ヤマト後払いSMS認証
	/// </summary>
	/// <returns>認証結果</returns>
	[WebMethod]
	public static string Authorize(string ninCode)
	{
		if (ValidateAuthCode(ninCode) == false)
		{
			return JsonConvert.SerializeObject(
				new
				{
					result = "Invalid"
				});
		}

		if (HttpContext.Current.Session[Constants.SESSION_KEY_PAYMENT_ORDER_ID] == null)
		{
			return JsonConvert.SerializeObject(
				new
				{
					result = PaymentYamatoKaSmsAuthResponseData.ResultValue.Ng.ToString()
				});
		}

		var smsAuthApi = new PaymentYamatoKaSmsAuthApi();
		var result = smsAuthApi.Exec((string)HttpContext.Current.Session[Constants.SESSION_KEY_PAYMENT_ORDER_ID], ninCode);

		return JsonConvert.SerializeObject(
			new
			{
				result = result.ToString()
			});
	}

	/// <summary>
	/// 認証コード入力確認
	/// </summary>
	/// <param name="authCode">認証コード</param>
	/// <returns>確認結果</returns>
	private static bool ValidateAuthCode(string authCode)
	{
		return (authCode.Length == 4) && (authCode.All(c => (char.IsNumber(c))));
	}

	/// <summary>
	/// Check Valid Tel No And Country For PaymentAtone And Aftee
	/// </summary>
	/// <returns>True: If Valid Tel No And Country Or False: If Invalid Tel No And Country </returns>
	protected bool CheckValidTelNoAndCountryForPaymentAtoneAndAftee(string paymentId)
	{
		var errorMessages = string.Empty;
		var result = false;
		var wspanErrorMessageForAtone = GetWrappedControl<WrappedHtmlGenericControl>(this.AtoneRepeaterItem, "spanErrorMessageForAtone");
		var wspanErrorMessageForAftee = GetWrappedControl<WrappedHtmlGenericControl>(this.AfteeRepeaterItem, "spanErrorMessageForAftee");
		switch (paymentId)
		{
			case Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE:
				if ((GlobalAddressUtil.IsCountryTw(this.OrderModel.Shippings[0].ShippingCountryIsoCode) == false)
					|| (GlobalAddressUtil.IsCountryTw(this.OrderModel.Owner.OwnerAddrCountryIsoCode) == false))
				{
					if (string.IsNullOrEmpty(errorMessages) == false) errorMessages += Environment.NewLine;
					errorMessages += WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CHECK_COUNTRY_FOR_PAYMENT_AFTEE);
				}
				result = string.IsNullOrEmpty(errorMessages);
				if ((result == false) && wspanErrorMessageForAftee.HasInnerControl)
				{
					wspanErrorMessageForAftee.InnerHtml =
						WebSanitizer.HtmlEncodeChangeToBr(errorMessages);
					wspanErrorMessageForAftee.InnerControl.Style["display"] = "block";
				}
				return result;

			case Constants.FLG_PAYMENT_PAYMENT_ID_ATONE:
				if ((GlobalAddressUtil.IsCountryJp(this.OrderModel.Shippings[0].ShippingCountryIsoCode) == false)
					|| (GlobalAddressUtil.IsCountryJp(this.OrderModel.Owner.OwnerAddrCountryIsoCode) == false))
				{
					if (string.IsNullOrEmpty(errorMessages) == false) errorMessages += Environment.NewLine;
					errorMessages += WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CHECK_COUNTRY_FOR_PAYMENT_ATONE);
				}

				result = string.IsNullOrEmpty(errorMessages);
				if ((result == false) && wspanErrorMessageForAtone.HasInnerControl)
				{
					wspanErrorMessageForAtone.InnerHtml =
						WebSanitizer.HtmlEncodeChangeToBr(errorMessages);
					wspanErrorMessageForAtone.InnerControl.Style["display"] = "block";
				}
				return result;

			default:
				if (wspanErrorMessageForAtone.HasInnerControl)
				{
					wspanErrorMessageForAtone.InnerControl.Style["display"] = "none";
				}
				if (wspanErrorMessageForAftee.HasInnerControl)
				{
					wspanErrorMessageForAftee.InnerControl.Style["display"] = "none";
				}
				return result;
		}
	}

	/// <summary>
	/// DropDownList shipping receiving store type selected index changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected new void ddlShippingReceivingStoreType_SelectedIndexChanged(object sender, EventArgs e)
	{
		var wddlShippingReceivingStoreType = GetWrappedControl<WrappedDropDownList>(
			((Control)sender).Parent,
			"ddlShippingReceivingStoreType");
		var wdvErrorShippingConvenience = GetWrappedControl<WrappedHtmlGenericControl>(
			wddlShippingReceivingStoreType.Parent,
			"dvErrorPaymentAndShippingConvenience");
		var isPaymentConvenienceStore = (this.OrderModel.OrderPaymentKbn
			!= Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE);

		wdvErrorShippingConvenience.Visible = (isPaymentConvenienceStore
			&& (ECPayUtility.GetIsCollection(wddlShippingReceivingStoreType.SelectedValue)
				== Constants.FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_COLLECTION_ON));

		var whfCvsShopId = GetWrappedControl<WrappedHiddenField>(wddlShippingReceivingStoreType.Parent, "hfCvsShopId");
		var whfCvsShopName = GetWrappedControl<WrappedHiddenField>(wddlShippingReceivingStoreType.Parent, "hfCvsShopName");
		var whfCvsShopAddress = GetWrappedControl<WrappedHiddenField>(wddlShippingReceivingStoreType.Parent, "hfCvsShopAddress");
		var whfCvsShopTel = GetWrappedControl<WrappedHiddenField>(wddlShippingReceivingStoreType.Parent, "hfCvsShopTel");
		var wlCvsShopId = GetWrappedControl<WrappedLiteral>(wddlShippingReceivingStoreType.Parent, "lCvsShopId");
		var wlCvsShopName = GetWrappedControl<WrappedLiteral>(wddlShippingReceivingStoreType.Parent, "lCvsShopName");
		var wlCvsShopAddress = GetWrappedControl<WrappedLiteral>(wddlShippingReceivingStoreType.Parent, "lCvsShopAddress");
		var wlCvsShopTel = GetWrappedControl<WrappedLiteral>(wddlShippingReceivingStoreType.Parent, "lCvsShopTel");

		whfCvsShopId.Value = string.Empty;
		whfCvsShopName.Value = string.Empty;
		whfCvsShopAddress.Value = string.Empty;
		whfCvsShopTel.Value = string.Empty;

		wlCvsShopId.Text = string.Empty;
		wlCvsShopName.Text = string.Empty;
		wlCvsShopAddress.Text = string.Empty;
		wlCvsShopTel.Text = string.Empty;
	}

	/// <summary>
	/// Set Information Receiving Store
	/// </summary>
	public void SetInformationReceivingStore()
	{
		if (this.PostParams.Count == 0) return;

		var repeaterItem = (RepeaterItem)rOrderShipping.Items[0];
		var control = this.WrOrderShipping.Items[repeaterItem.ItemIndex];
		var wdvShippngInput = GetWrappedControl<WrappedHtmlGenericControl>(control, "dShippngInput");
		var wdvShippingInfo = GetWrappedControl<WrappedHtmlGenericControl>(control, "dShippingInfo");
		var wdivConvenienceStore = GetWrappedControl<WrappedHtmlGenericControl>(control, "divConvenienceStore");
		var wdivShippingInputFormInner = GetWrappedControl<WrappedHtmlGenericControl>(control, "divShippingInputFormInner");
		var whfCvsShopId = GetWrappedControl<WrappedHiddenField>(control, "hfCvsShopId");
		var whfCvsShopName = GetWrappedControl<WrappedHiddenField>(control, "hfCvsShopName");
		var whfCvsShopAddress = GetWrappedControl<WrappedHiddenField>(control, "hfCvsShopAddress");
		var whfCvsShopTel = GetWrappedControl<WrappedHiddenField>(control, "hfCvsShopTel");
		var whfCvsShopFlg = GetWrappedControl<WrappedHiddenField>(control, "hfCvsShopFlg");
		var wlCvsShopId = GetWrappedControl<WrappedLiteral>(control, "lCvsShopId");
		var wlCvsShopName = GetWrappedControl<WrappedLiteral>(control, "lCvsShopName");
		var wlCvsShopAddress = GetWrappedControl<WrappedLiteral>(control, "lCvsShopAddress");
		var wlCvsShopTel = GetWrappedControl<WrappedLiteral>(control, "lCvsShopTel");
		var wddlShippingType = GetWrappedControl<WrappedDropDownList>(control, "ddlShippingType");
		var wddlShippingReceivingStoreType = GetWrappedControl<WrappedDropDownList>(control, "ddlShippingReceivingStoreType");
		var wspConvenienceStoreEcPaySelect = GetWrappedControl<WrappedHtmlGenericControl>(control, "spConvenienceStoreEcPaySelect");
		var wdvErrorPaymentAndShippingConvenience = GetWrappedControl<WrappedHtmlGenericControl>(control, "dvErrorPaymentAndShippingConvenience");
		var shippingReceivingStoreTypeSeleted = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SHIPPING_RECEIVING_STORE_TYPE]);

		// Display convenience store Ec Pay information
		wdvShippngInput.Visible = true;
		wdvShippingInfo.Visible = false;
		wdivConvenienceStore.Visible = true;
		wdivShippingInputFormInner.Visible = false;
		wddlShippingReceivingStoreType.Visible = true;
		wspConvenienceStoreEcPaySelect.Visible = true;

		// Set convenience store Ec Pay data
		whfCvsShopId.Value = this.PostParams[ECPayConstants.PARAM_CVSSTOREID];
		whfCvsShopName.Value = this.PostParams[ECPayConstants.PARAM_CVSSTORENAME];
		whfCvsShopAddress.Value = this.PostParams[ECPayConstants.PARAM_CVSADDRESS];
		whfCvsShopTel.Value = this.PostParams[ECPayConstants.PARAM_CVSTELEPHONE];
		whfCvsShopFlg.Value = Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON;
		wlCvsShopId.Text = this.PostParams[ECPayConstants.PARAM_CVSSTOREID];
		wlCvsShopName.Text = this.PostParams[ECPayConstants.PARAM_CVSSTORENAME];
		wlCvsShopAddress.Text = this.PostParams[ECPayConstants.PARAM_CVSADDRESS];
		wlCvsShopTel.Text = this.PostParams[ECPayConstants.PARAM_CVSTELEPHONE];
		wddlShippingType.SelectedValue = CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE;
		wddlShippingReceivingStoreType.SelectedValue = shippingReceivingStoreTypeSeleted;
		wdvErrorPaymentAndShippingConvenience.Visible = (ECPayUtility.GetIsCollection(shippingReceivingStoreTypeSeleted)
			== Constants.FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_COLLECTION_ON);
	}

	/// <summary>
	/// Request Cvs Def Invoice Reissue
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbRequestCvsDefInvoiceReissue_Click(object sender, EventArgs e)
	{
		// Update order extend status 38 ON
		var isUpdateOrderExtendStatusSuccess = (new OrderService().UpdateOrderExtendStatus(
			this.OrderModel.OrderId,
			int.Parse(Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_GMO_CVS_DEF_INVOICE_REISSUE),
			Constants.FLG_ORDER_ORDER_EXTEND_STATUS_ON,
			DateTime.Now.Date,
			Constants.FLG_LASTCHANGED_USER,
			UpdateHistoryAction.Insert,
			null) > 0);

		// Handle display of control
		this.WlCvsDefInvoiceReissueRequestResultMessage.Text = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_CVSDEF_INVOICE_REISSUE_COMPLETE);
		this.WlCvsDefInvoiceReissueRequestResultMessage.Visible = isUpdateOrderExtendStatusSuccess;

		if (isUpdateOrderExtendStatusSuccess)
		{
			this.WlbRequestCvsDefInvoiceReissue.InnerControl.Enabled = false;
		}

		if (this.WlbRequestCvsDefInvoiceReissue.InnerControl.Enabled == false)
		{
			this.WlbRequestCvsDefInvoiceReissue.InnerControl.OnClientClick = string.Empty;
		}
		if (IsSmartPhone && isUpdateOrderExtendStatusSuccess)
		{
			this.WlbRequestCvsDefInvoiceReissue.InnerControl.Visible = false;
		}

		if (this.OrderModel.ExtendStatus38 == Constants.FLG_ORDER_ORDER_EXTEND_STATUS_ON)
		{
			this.WlCvsDefInvoiceReissueRequestResultMessage.Visible = false;
		}
	}

	/// <summary>
	/// 注文拡張項目 変更ボタンイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbOrderExtend_OnClick(object sender, EventArgs e)
	{
		this.IsOrderExtendModify = (this.IsOrderExtendModify == false);
		this.WlbOrderExtend.Visible = false;
		var orderExtend = OrderExtendCommon.ConvertOrderExtend(this.OrderModel);
		var input = new OrderExtendInput(OrderExtendInput.UseType.Modify, orderExtend);
		this.WrOrderExtendInput.DataSource = input.OrderExtendItems;
		this.WrOrderExtendInput.DataBind();
		this.Process.SetOrderExtendFromUserExtendObject(this.WrOrderExtendInput, input);
	}

	/// <summary>
	/// 注文拡張項目 更新ボタンイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbUpdateOrderExtend_OnClick(object sender, EventArgs e)
	{
		var inputDictionary = this.Process.CreateOrderExtendFromInputData(this.WrOrderExtendInput);
		var input = new OrderExtendInput(OrderExtendInput.UseType.Modify, inputDictionary);
		var errorMessage = input.Validate();
		if (errorMessage.Count == 0)
		{
			this.IsOrderExtendModify = (this.IsOrderExtendModify == false);
			this.WlbOrderExtend.Visible = true;

			var orderExtend = OrderExtendCommon.ConvertOrderExtend(this.OrderModel);

			foreach (var value in inputDictionary.Where(value => orderExtend.ContainsKey(value.Key)))
			{
				orderExtend[value.Key] = inputDictionary[value.Key];
			}

			var inputorderExtend = orderExtend.ToDictionary(v => v.Key, v => v.Value.Value);
			new OrderService().UpdateOrderExtend(
				this.OrderModel.OrderId,
				Constants.FLG_LASTCHANGED_USER,
				inputorderExtend,
				UpdateHistoryAction.Insert);

			this.WrOrderExtendDisplay.DataSource = input.OrderExtendItems;
			this.WrOrderExtendDisplay.DataBind();
		}
		else
		{
			this.Process.SetOrderExtendErrMessage(this.WrOrderExtendInput, errorMessage);
		}
	}

	/// <summary>
	/// 注文拡張項目 キャンセルボタンイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbHideOrderExtend_OnClick(object sender, EventArgs e)
	{
		this.IsOrderExtendModify = (this.IsOrderExtendModify == false);
		this.WlbOrderExtend.Visible = true;
	}

	/// <summary>
	/// SMS認証モーダル取得
	/// </summary>
	/// <returns>SMS認証モーダル</returns>
	private PaymentYamatoKaSmsAuthModalBase GetYamatoKaSmsAuthModal()
	{
		var result = this.Master.FindControl("ContentPlaceHolder1").FindControl("ucPaymentYamatoKaSmsAuthModal");
		return (result is PaymentYamatoKaSmsAuthModalBase) ? (PaymentYamatoKaSmsAuthModalBase)result : null;
	}

	/// <summary>
	/// Linkbutton search address from shipping zipcode global click event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSearchAddrFromShippingZipGlobal_Click(object sender, EventArgs e)
	{
		var index = int.Parse(((LinkButton)sender).CommandArgument);
		var countryIsoCode = StringUtility.ToEmpty(GetShippingAddrCountryIsoCode(index));
		if (IsNotCountryJp(countryIsoCode) == false) return;

		var wtbShippingZipGlobal = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingZipGlobal");
		var wtbShippingAddr2 = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingAddr2");
		var wtbShippingAddr4 = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingAddr4");
		var wtbShippingAddr5 = GetWrappedControl<WrappedTextBox>(this.WrOrderShipping.Items[index], "tbShippingAddr5");
		var wddlShippingAddr2 = GetWrappedControl<WrappedDropDownList>(this.WrOrderShipping.Items[index], "ddlShippingAddr2");
		var wddlShippingAddr3 = GetWrappedControl<WrappedDropDownList>(this.WrOrderShipping.Items[index], "ddlShippingAddr3");
		var wddlShippingAddr5 = GetWrappedControl<WrappedDropDownList>(this.WrOrderShipping.Items[index], "ddlShippingAddr5");

		BindingAddressByGlobalZipcode(
			countryIsoCode,
			StringUtility.ToHankaku(wtbShippingZipGlobal.Text.Trim()),
			wtbShippingAddr2,
			wtbShippingAddr4,
			wtbShippingAddr5,
			wddlShippingAddr2,
			wddlShippingAddr3,
			wddlShippingAddr5);
	}

	/// <summary>
	/// 領収書出力エラーメッセージを表示
	/// </summary>
	private void DisplayReceiptDownloadError()
	{
		if (this.OrderModel.IsCanceled)
		{
			this.ReceiptDownloadErrorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_RECEIPT_ORDER_CANCELED);
		}
		else if (this.OrderModel.ReceiptOutputFlg == Constants.FLG_ORDER_RECEIPT_OUTPUT_FLG_ON)
		{
			this.ReceiptDownloadErrorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_RECEIPT_SETTLED_RECEIPTDOWNLOAD);
		}

		if (string.IsNullOrEmpty(this.ReceiptDownloadErrorMessage) == false)
		{
			WlbReceiptDownload.Enabled = false;
			WlbReceiptDownload.OnClientClick = null;
		}
	}

	/// <summary>
	/// 領収書出力URL追加
	/// </summary>
	/// <param name="orderId">注文ID</param>
	/// <returns>領収書出力URL</returns>
	public string CreateReceiptDownloadUrl(string orderId)
	{
		var encryptOrderId = UserPassowordCryptor.PasswordEncrypt(orderId);
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_RECEIPTDOWNLOAD)
			.AddParam(Constants.REQUEST_KEY_ORDER_ID_FOR_RECEIPT, encryptOrderId)
			.CreateUrl();
		return url;
	}

	/// <summary>
	/// 注文キャンセルボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbCancelOrder_Click(object sender, EventArgs e)
	{
		if ((this.OrderModel.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_ORDERED)
			&& (DateTime.Now < ((DateTime)this.OrderModel.OrderDate).AddMinutes(Constants.ORDER_HISTORY_DETAIL_ORDER_CANCEL_TIME)))
		{
			// 入金済みだとキャンセルできない決済種別か
			var useModifyPaymentStatus = CheckUseModifyPaymentStatus();
			this.CanModifyPayment = this.IsModifyOrder && (this.ValidPayments[0].Length > 0) && useModifyPaymentStatus;
			if (this.CanModifyPayment == false)
			{
				if (useModifyPaymentStatus == false)
				{
					// エラーページへ
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_MODIFY_NG_PAYMENT_STATUS_COMPLETE_CANCEL);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
				}
			}

			// ステータス更新
			if (StatusUpdate() == false)
			{
				// VeritransのPayPayの場合、エラーページに飛ばず、エラー表示する
				if ((this.OrderModel.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
					&& (Constants.PAYMENT_PAYPAY_KBN == Constants.PaymentPayPayKbn.VeriTrans))
				{
					WlPaymentErrorMessage.Text = StringUtility.ToEmpty(Session[Constants.SESSION_KEY_ERROR_MSG]);
					return;
				}
				// エラーページへ
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}

			//注文キャンセルメール送信
			OrderCommon.SendOrderMail(this.OrderModel.OrderId, Constants.CONST_MAIL_ID_ORDER_CANCEL);

			if (Constants.NE_OPTION_ENABLED
				&& Constants.NE_COOPERATION_CANCEL_ENABLED
				&& (OrderCommon.UpdateNextEngineOrderForCancel(this.OrderModel).Item3 == false))
			{
				NextEngineApi.SendFailureCancelOrderMail(this.OrderModel.OrderId, this.LoginUserId);
			}

			// ページをリロード
			ReloadOrderHistoryDetail(false);
		}
		else
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_ORDER_CANCELABLE_TIMEOUT_ORDER_STATUS_UPDATED);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}
	}

	/// <summary>
	/// 外部決済連携ログ格納
	/// </summary>
	/// <param name="apiErrorMessage">apiエラーメッセージ</param>
	/// <param name="orderOld">注文情報</param>
	/// <param name="accessor">アクセサ</param>
	private void ExternalPaymentLog(string apiErrorMessage, OrderModel orderOld, SqlAccessor accessor)
	{
		var hasApiErrorMessage = (string.IsNullOrEmpty(apiErrorMessage) == false);

		string externalApiLog;
		if (hasApiErrorMessage)
		{
			externalApiLog = apiErrorMessage;
		}
		else if (PaygentUtility.CheckIsPaidyPaygentPayment(this.OrderModel.OrderPaymentKbn))
		{
			var cancelMessage = this.OrderModel.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP
				? PaygentConstants.PAYGENT_PAIDY_REFUND_LOG_MESSAGE
				: PaygentConstants.PAYGENT_PAIDY_CANCEL_LOG_MESSAGE;

			externalApiLog = externalApiLog = LogCreator.CreateMessageWithPaymentId(
				this.OrderInput.CardTranId,
				this.OrderInput.PaymentOrderId,
				this.OrderInput.LastBilledAmount.ToPriceString(),
				cancelMessage);
		}
		else
		{
			externalApiLog = LogCreator.CrateMessageWithCardTranId(
				this.OrderModel.CardTranId ?? orderOld.CardTranId,
				string.Empty);
		}

		// 外部決済連携ログ格納処理
		OrderCommon.AppendExternalPaymentCooperationLog(
			hasApiErrorMessage == false,
			this.OrderModel.OrderId,
			externalApiLog,
			Constants.FLG_LASTCHANGED_USER,
			UpdateHistoryAction.Insert,
			accessor);

		PaymentFileLogger.WritePaymentLog(
			hasApiErrorMessage == false,
			orderOld.PaymentName ?? "",
			PaymentFileLogger.PaymentType.Unknown,
			PaymentFileLogger.PaymentProcessingType.Unknown,
			hasApiErrorMessage
				? apiErrorMessage + string.Format("\tpaymentName : {0}", orderOld.PaymentName)
				: LogCreator.CrateMessageWithCardTranId(
					this.OrderModel.CardTranId ?? orderOld.CardTranId,
					"") + string.Format("\tpaymentName : {0}", orderOld.PaymentName));
	}

	/// <summary>
	/// 注文情報キャンセル処理（ステータス以外）
	/// </summary>
	/// <param name="accessor">アクセサ</param>
	/// <returns>成功か</returns>
	private bool OrderInformationCancel(SqlAccessor accessor)
	{
		var order = OrderCommon.GetOrder(this.OrderModel.OrderId, accessor);

		// 付与ポイント戻し可能可能かどうかチェックしてからキャンセル処理を行う
		var canCancelUserPoint = CheckCanRevokeGrantedUserPoint(new OrderModel(order[0]), accessor);
		if (canCancelUserPoint == false)
		{
			accessor.RollbackTransaction();
			return false;
		}

		var updateTwInvoiceStatus = string.Empty;
		if (OrderCommon.DisplayTwInvoiceInfo())
		{
			if (WrbTwInvoiceCancel.Checked) updateTwInvoiceStatus = Constants.FLG_ORDER_INVOICE_STATUS_CANCEL;
			if (WrbTwInvoiceRefund.Checked) updateTwInvoiceStatus = Constants.FLG_ORDER_INVOICE_STATUS_REFUND;
		}

		OrderCommon.CancelOrder(
			order[0],
			true,
			Constants.FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_ORDER_CANCEL,
			this.OrderModel.ShopId,
			Constants.FLG_LASTCHANGED_USER,
			UpdateHistoryAction.DoNotInsert,
			accessor,
			updateTwInvoiceStatus);

		// ユーザーリアルタイム累計購入回数処理
		var ht = new Hashtable
		{
			{ Constants.FIELD_ORDER_USER_ID, order[0][Constants.FIELD_ORDER_USER_ID] },
			{
				Constants.FIELD_USER_ORDER_COUNT_ORDER_REALTIME,
				new UserService().Get(this.OrderModel.UserId, accessor).OrderCountOrderRealtime
			}
		};
		OrderCommon.UpdateRealTimeOrderCount(
			ht,
			Constants.FLG_REAL_TIME_ORDER_COUNT_ACTION_CANCEL,
			accessor);

		return true;
	}

	/// <summary>
	/// ステータス更新処理
	/// </summary>
	/// <returns>成功か</returns>
	private bool StatusUpdate()
	{
		// 該当ステートメント取得
		var statement = OrderCommon.GetUpdateOrderStatusStatement(Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED);

		var orderOld = this.OrderModel;

		// ステートメント実行用パラメータ作成
		var input = new Hashtable
		{
			{ Constants.FIELD_ORDER_ORDER_ID, this.OrderModel.OrderId },
			{ "update_date", DateTime.Now },
			{ Constants.FIELD_ORDER_USER_ID, this.OrderModel.UserId },
			{ Constants.FIELD_ORDER_LAST_CHANGED, Constants.FLG_LASTCHANGED_USER }
		};

		// ステータス更新処理
		using (var accessor = new SqlAccessor())
		{
			// トランザクション開始
			accessor.OpenConnection();
			accessor.BeginTransaction();
			try
			{
				var actionStatus =
					OrderHistory.GetOrderStatusAction(Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED);
				if ((MallOptionUtility.CheckMallKbn(this.OrderModel.ShopId, this.OrderModel.MallId)
					== MallOptionUtility.MallKbn.OwnSite))
				{
					actionStatus.Add(Constants.FIELD_ORDER_ONLINE_PAYMENT_STATUS);
				}

				// 更新履歴作成準備
				var actionInput = new Hashtable
					{
						{ Constants.TABLE_ORDER, actionStatus }
					};
				OrderHistory history = new OrderHistory
				{
					OrderId = this.OrderModel.OrderId,
					Action = OrderHistory.ActionType.FrontOrderCancel,
					OpearatorName = this.LoginUserName,
					Accessor = accessor,
					UpdateAction = actionInput
				};

				// 更新用データ取得
				history.HistoryBegin();

				// １．外部連携キャンセル処理（現状はエラーの時はステータス変更しないようにする）
				// 注文ステータス更新かつ（キャンセルまたは仮注文キャンセル）の場合
				if (MallOptionUtility.CheckMallKbn(this.OrderModel.ShopId, this.OrderModel.MallId)
					== MallOptionUtility.MallKbn.OwnSite)
				{
					// 外部連携キャンセル
					var apiErrorMessage = OrderCommon.CancelExternalCooperationPayment(this.OrderModel, accessor);

					// 外部決済連携ログ格納処理
					ExternalPaymentLog(apiErrorMessage, orderOld, accessor);

					if (apiErrorMessage == String.Empty)
					{
						var sendingAmount = CurrencyManager.GetSendingAmount(
							this.OrderModel.LastBilledAmount,
							this.OrderModel.SettlementAmount,
							this.OrderModel.SettlementCurrency);

						// Paygent(Paidy)利用時、売上確定後にキャンセル連動した際は、返金したことをメモへ残す
						var actionName = PaygentUtility.CheckIsPaidyPaygentPayment(this.OrderModel.OrderPaymentKbn)
							&& this.OrderModel.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP
								? "返金"
								: "キャンセル";

						// キャンセル向け外部決済ステータス系＆メモ更新（オンライン決済ステータス更新は二重更新になるが・・）
						OrderCommon.UpdateExternalPaymentStatusesAndMemoForCancel(
							this.OrderModel.OrderId,
							this.OrderModel.OrderPaymentKbn,
							this.OrderModel.PaymentOrderId,
							this.OrderModel.CardTranId,
							sendingAmount,
							this.OrderModel.IsExchangeOrder,
							this.OrderModel.UserId,
							actionName,
							UpdateHistoryAction.DoNotInsert,
							accessor);
					}
					else
					{
						// 決済連動キャンセルエラー
						Session[Constants.SESSION_KEY_ERROR_MSG] =
							WebMessages.GetMessages(WebMessages.ERRMSG_CANCEL_PAYMENT_FAILED);
						return false;
					}
				}

				// ２．ステータス更新実行
				int orderStatusUpdated;
				using (var sqlStatement = new SqlStatement("Order", statement))
				{
					orderStatusUpdated = sqlStatement.ExecStatement(accessor, input);
				}

				// ３．ステータス更新に伴う処理
				// ステータス更新有りかつ注文ステータス更新かつ（キャンセルまたは仮注文キャンセル）？
				if (orderStatusUpdated > 0)
				{
					// 注文情報キャンセル処理（ステータス以外）
					if (OrderInformationCancel(accessor) == false)
					{
						// 注文情報キャンセルエラー
						Session[Constants.SESSION_KEY_ERROR_MSG] =
							WebMessages.GetMessages(WebMessages.ERRMSG_UPDATE_ORDER_STATUS_FAILED);
						return false;
					}
				}

				// 更新履歴登録
				new UpdateHistoryService().InsertAllForOrder(
					this.OrderModel.OrderId,
					Constants.FLG_LASTCHANGED_USER,
					accessor);

				// 履歴更新
				history.HistoryComplete();

				// トランザクションコミット
				accessor.CommitTransaction();
				return true;

			}
			catch (Exception ex)
			{
				// トランザクションロールバック
				accessor.RollbackTransaction();

				//ログ出力
				var message = new StringBuilder();
				message.AppendLine("HTTPエラー: " + ex);
				message.AppendLine("  要求URL: " + (WebUtility.GetRawUrl(Request) ?? "(null)"));
				message.AppendLine("  実行URL: " + (Request.Url.PathAndQuery ?? "(null)"));
				message.AppendLine("  IPアドレス: " + (Request.UserHostAddress ?? "(null)"));
				message.AppendLine("  User-Agent: " + (Request.UserAgent ?? "(null)"));
				message.AppendLine("  ユーザー情報: " + this.OrderModel.UserId);
				message.AppendLine("  注文情報: " + this.OrderModel.OrderId);

				// ログファイル記述
				AppLogger.WriteError(message.ToString(), ex);

				var mailData = new Hashtable
					{
						{ Constants.FIELD_ORDER_ORDER_ID, this.OrderModel.OrderId }
					};
				using (MailSendUtility msMailSend = new MailSendUtility(
					Constants.CONST_DEFAULT_SHOP_ID,
					Constants.CONST_MAIL_ID_SEND_UPDATE_ORDER_STATUS_FAILED,
					"",
					mailData,
					true,
					Constants.MailSendMethod.Auto))
				{
					// エラーメール送信
					msMailSend.SendMail();
				}

				Session[Constants.SESSION_KEY_ERROR_MSG] =
					WebMessages.GetMessages(WebMessages.ERRMSG_UPDATE_ORDER_STATUS_FAILED);
				return false;
			}
		}
	}

	/// <summary>
	/// Execute PayPay and update order
	/// </summary>
	/// <param name="sender">Sender</param>
	/// <param name="e">Event args</param>
	private void ExecutePayPayAndUpdateOrder(object sender, EventArgs e)
	{
		switch (Constants.PAYMENT_PAYPAY_KBN)
		{
			case Constants.PaymentPayPayKbn.SBPS:
				var paypayOrder = SBPSSessionWrapper.FindSBPSMultiPendingOrder(this.OrderModel.OrderId);
				if (paypayOrder == null) return;
				break;

			case Constants.PaymentPayPayKbn.GMO:
				var paypayGmoOrder = GmoSessionWrapper.FindGmoMultiPendingOrder(this.OrderModel.OrderId);
				if (paypayGmoOrder == null) return;
				break;
		}

		// 無理やりメルペイを選択させる！！！！
		lbDisplayInputOrderPaymentKbn_Click(sender, e);

		foreach (RepeaterItem riPayment in this.WrPayment.Items)
		{
			var wrbgPayment = GetWrappedControl<WrappedRadioButtonGroup>(riPayment, "rbgPayment");
			var whfPaymentId = GetWrappedControl<WrappedHiddenField>(riPayment, "hfPaymentId", string.Empty);

			if (wrbgPayment.Checked) wrbgPayment.Checked = false;
			if (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
			{
				wrbgPayment.Checked = true;
				break;
			}
		}

		// Suspend if something goes wrong with SbpsMultiReceive
		if (Request[PaypayConstants.REQUEST_KEY_RECEIVE_RESULT] != PaypayConstants.FLG_SBPS_EXECUTE_RESULT_OK)
		{
			this.WsErrorMessagePayment.InnerHtml = WebSanitizer.HtmlEncodeChangeToBr(
				WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PAYPAY_AUTH_ERROR));
			return;
		}

		// 決済変更クリックイベント発火
		btnUpdatePaymentPatternInfo_Click(sender, e);
	}

	/// <summary>
	/// Action for form boku
	/// </summary>
	private void ActionForFormBoku()
	{
		if (Request["action"] == "boku")
		{
			var param = (Hashtable)this.Session[Constants.SESSION_KEY_PARAM];
			if ((param == null)
				|| (param.ContainsKey("order_new") == false)
				|| (((OrderModel)param["order_new"]).OrderId != this.OrderModel.OrderId))
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_ANOTHER_BROWSER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}

			var orderNew = (OrderModel)param["order_new"];

			WrappedRadioButtonGroup wrbgPayment;
			this.WdvOrderPaymentPattern.Visible = true;
			foreach (RepeaterItem wrPaymentItem in this.WrPayment.Items)
			{
				var wrbgTmpPayment = GetWrappedControl<WrappedRadioButtonGroup>(wrPaymentItem, "rbgPayment");
				var whfPaymentId = GetWrappedControl<WrappedHiddenField>(wrPaymentItem, "hfPaymentId", string.Empty);
				if (whfPaymentId.Value == orderNew.OrderPaymentKbn)
				{
					wrbgPayment = wrbgTmpPayment;
					wrbgTmpPayment.Checked = true;
					rbgPayment_OnCheckedChanged(wrbgPayment, EventArgs.Empty);
				}
				else
				{
					wrbgTmpPayment.Checked = false;
				}
			}

			var charge = BokuReauthProcess(orderNew);
			if (charge == null) return;

			orderNew.CardTranId = charge.ChargeId;
			orderNew.ExternalPaymentAuthDate = DateTime.Now;
			orderNew.PaymentMemo = OrderCommon.CreateOrderPaymentMemoForAuth(
				orderNew.OrderId,
				charge.OptinId,
				orderNew.OrderPaymentKbn,
				charge.ChargeId,
				orderNew.OrderPriceTotal);

			var isUpdateFixedPurchase = (bool)param["is_update_fixed_purchase"];
			orderNew.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP;
			orderNew.OrderPaymentStatus = Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM;
			orderNew.OnlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;

			if (string.IsNullOrEmpty(OrderModel.FixedPurchaseId)) isUpdateFixedPurchase = false;

			var reauthResult = ExecuteChangeOrderForPayment(this.OrderModel, orderNew, isUpdateFixedPurchase);
			OrderCommon.AppendExternalPaymentCooperationLog(
				reauthResult,
				orderNew.OrderId,
				LogCreator.CreateMessage(orderNew.OrderId, orderNew.PaymentOrderId),
				orderNew.LastChanged,
				UpdateHistoryAction.Insert);

			if (reauthResult)
			{
				if (isUpdateFixedPurchase)
				{
					new FixedPurchaseService().SetExternalPaymentAgreementId(
						OrderModel.FixedPurchaseId,
						charge.OptinId,
						Constants.FLG_LASTCHANGED_USER,
						UpdateHistoryAction.Insert,
						fixedPurchaseHistoryKbn: Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_CONTINUOUS_ORDER_REGISTER);
				}

				this.Session[Constants.SESSION_KEY_PARAM] = null;
				this.Response.Redirect(
					new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_HISTORY_DETAIL)
						.AddParam(Constants.REQUEST_KEY_ORDER_ID, this.OrderModel.OrderId)
						.CreateUrl());
			}
		}
	}

	/// <summary>
	/// Boku reauth process
	/// </summary>
	/// <param name="orderNew">Order new</param>
	/// <returns>Payment boku charge response</returns>
	private PaymentBokuChargeResponse BokuReauthProcess(OrderModel orderNew)
	{
		// Exec validate api
		var validate = new PaymentBokuValidateOptinApi().Exec(orderNew.PaymentOrderId);
		if (validate == null)
		{
			this.WsErrorMessagePayment.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_BOKU_PAYMENT_ERROR);
			AppLogger.WriteError(
				string.Format(
					"{0} Payment Error: Order ID ={1}",
					orderNew.OrderPaymentKbn,
					orderNew.OrderId));
			return null;
		}
		else if (validate.IsSuccess == false)
		{
			this.WsErrorMessagePayment.InnerHtml = validate.Result.Message;
			return null;
		}

		// Exec confirm api
		var response = new PaymentBokuConfirmOptinApi().Exec(orderNew.PaymentOrderId);
		if (response == null)
		{
			this.WsErrorMessagePayment.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_BOKU_PAYMENT_ERROR);
			AppLogger.WriteError(
				string.Format(
					"{0} Payment Error: Order ID ={1}",
					orderNew.OrderPaymentKbn,
					orderNew.OrderId));
			return null;
		}
		else if (response.IsSuccess == false)
		{
			this.WsErrorMessagePayment.InnerHtml = response.Result.Message;
			return null;
		}

		// Exec charge api
		var fixedPurchase = new FixedPurchaseService().Get(orderNew.FixedPurchaseId);
		if (orderNew.IsFixedPurchaseOrder && (fixedPurchase == null))
		{
			var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_HISTORY_DETAIL)
				.AddParam(Constants.REQUEST_KEY_ORDER_ID, orderNew.OrderId)
				.CreateUrl();
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_SYSTEM_ERROR);
			Response.Redirect(
				new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR)
					.AddParam(Constants.REQUEST_KEY_BACK_URL, url)
					.CreateUrl());
		}

		var itemDesciption = string.Join(
			",",
			orderNew.Items.Select(item => item.ProductName));
		var charge = new PaymentBokuChargeApi().Exec(
			orderNew.SettlementCurrency,
			string.Empty,
			itemDesciption,
			orderNew.OrderId,
			orderNew.PaymentOrderId,
			orderNew.OrderPriceTotal.ToString(),
			(orderNew.OrderTaxIncludedFlg == Constants.FLG_ORDER_ORDER_TAX_INCLUDED_PRETAX),
			orderNew.RemoteAddr,
			orderNew.IsFixedPurchaseOrder,
			orderNew.IsFixedPurchaseOrder ? (fixedPurchase.OrderCount > 1) : false,
			orderNew.IsFixedPurchaseOrder ? fixedPurchase.FixedPurchaseKbn : string.Empty,
			orderNew.IsFixedPurchaseOrder ? fixedPurchase.FixedPurchaseSetting1 : string.Empty);

		if (charge == null)
		{
			this.WsErrorMessagePayment.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_BOKU_PAYMENT_ERROR);
			AppLogger.WriteError(
				string.Format(
					"{0} Payment Error: Order ID ={1}",
					orderNew.OrderPaymentKbn,
					orderNew.OrderId));
			return null;
		}
		else if ((charge.IsSuccess == false)
			|| (charge.ChargeStatus != BokuConstants.CONST_BOKU_CHARGE_STATUS_SUCCESS))
		{
			this.WsErrorMessagePayment.InnerHtml = (charge.ChargeStatus == BokuConstants.CONST_BOKU_CHARGE_STATUS_IN_PROGRESS)
				? CommerceMessages.GetMessages(CommerceMessages.ERRMSG_BOKU_PAYMENT_PROCESS_TIME_OUT)
				: charge.Result.Message;
			return null;
		}

		return charge;
	}

	/// <summary>
	/// 戻るボタンのサイトパス取得
	/// </summary>
	/// <param name="fixedPurchaseId">定期ID</param>
	/// <returns>サイトパス</returns>
	protected string GetBackBtnPath(string fixedPurchaseId = "")
	{
		var url = Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_HISTORY_LIST;
		if (string.IsNullOrEmpty(fixedPurchaseId) == false)
		{
			url = PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(fixedPurchaseId);
		}
		return url;
	}

	/// <summary>
	/// 商品数量変更ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnModifyProducts_OnClick(object sender, EventArgs e)
	{
		this.IsProductModify = true;
		this.WbtnModifyProducts.Visible = false;
		this.WbtnModifyConfirm.Visible = true;
		this.WbtnModifyCancel.Visible = true;
		foreach (RepeaterItem riShipping in WrOrderShipping.Items)
		{
			var dvOrderHistoryProduct = (HtmlGenericControl)riShipping.FindControl("dvOrderHistoryProduct");
			var dvOrderModifyInput = (HtmlGenericControl)riShipping.FindControl("dvOrderModifyInput");
			dvOrderHistoryProduct.Visible = false;
			dvOrderModifyInput.Visible = true;
			this.OrderInputOld = new OrderInput(this.OrderModel);
			var orderShippingItem = this.OrderShippingItems.Select(item => (List<DataRowView>)item["childs"]);
			var dispProduct = orderShippingItem.SelectMany(childs => childs
				.Select(orderItem => new OrderItemModel(orderItem)))
				.SelectMany((product, index) => this.OrderInput.Shippings[riShipping.ItemIndex].Items
					.Where(item => item.IsSameProductWithPrice(new OrderItemInput(product, index)))).ToList();
			var rProduct = (Repeater)riShipping.FindControl("rOrderInputShippingItem");
			rProduct.DataSource = dispProduct;
			rProduct.DataBind();
		}
	}

	/// <summary>
	/// 情報更新ボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnModifyConfirm_OnClick(object sender, EventArgs e)
	{
		this.WsNoveltyChangeNoticeMessage.InnerHtml = string.Empty;
		this.WsModifyErrorMessage.InnerHtml = string.Empty;

		var orderService = new OrderService();

		this.OrderInputOld = new OrderInput(this.OrderModel);
		SetCoupon(this.OrderInputOld);
		var errorMessage = GetOrderInput();
		SetCoupon(this.OrderInput);

		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			this.WsModifyErrorMessage.InnerHtml = errorMessage;
			return;
		}

		SetCoupon(this.OrderInputOld);
		SetCoupon(this.OrderInput);

		var calculate = new OrderModifyCalculate();
		errorMessage = calculate.ReCalculate(this.OrderInputOld, this.OrderInput, addAutoNovelty: false);
		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			this.WsModifyErrorMessage.InnerHtml = errorMessage;
			return;
		}

		// 更新前後の商品数を比較します、変更ありの場合のみに更新します
		var orderItemsOld = calculate.OrderInputOld.Shippings
			.SelectMany(item => item.Items)
			.Select(product => new { ProductId = product.ProductId, ItemQuantity = product.ItemQuantity })
			.ToArray();
		var orderItemsNew = calculate.OrderInput.Shippings
			.SelectMany(item => item.Items)
			.Select(product => new { ProductId = product.ProductId, ItemQuantity = product.ItemQuantity })
			.ToArray();

		if (orderItemsOld.SequenceEqual(orderItemsNew) == false)
		{
			this.OrderInput = calculate.OrderInput;

		// 決済種別がPayPayの場合、最終請求金額チェックを実施
		decimal oldLastBilledAmount;
		decimal.TryParse(this.OrderInputOld.LastBilledAmount, out oldLastBilledAmount);

		decimal lastBilledAmount;
		decimal.TryParse(this.OrderInput.LastBilledAmount, out lastBilledAmount);

		if ((this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
			&& (oldLastBilledAmount < lastBilledAmount))
		{
			errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CHANGE_INFORMATION_FOR_PAYPAY);

			this.WsModifyErrorMessage.InnerHtml = errorMessage;
			return;
		}

		// 決済上限、下限のチェック
		var paymentErrorMessage = IsPaymentPriceInRange(this.OrderInput);
		if (string.IsNullOrEmpty(paymentErrorMessage) == false)
		{
			this.WsModifyErrorMessage.InnerHtml = paymentErrorMessage;
			return;
		}

			// 更新
			UpdateOrder(this.OrderInput.CreateModel(), calculate.User, calculate.Cart);

			// 拡張ステータス45の値確認、OFFの場合をONに更新
			if (string.Equals(this.OrderModel.ExtendStatus45, "1") == false)
			{
				// 拡張ステータス45更新
				orderService.UpdateOrderExtendStatus(
					this.OrderInput.OrderId,
					45,
					Constants.FLG_ORDER_ORDER_EXTEND_STATUS_ON,
					DateTime.Now,
					Constants.FLG_LASTCHANGED_USER,
					UpdateHistoryAction.Insert);
			}
		}

		ReloadOrderHistoryDetail();
	}

	/// <summary>
	/// クーポン設定
	/// </summary>
	/// <param name="orderInput">受注情報</param>
	private void SetCoupon(OrderInput orderInput)
	{
		if (orderInput.Coupon == null)
		{
			var orderCoupons = new List<OrderCouponInput>();

			var orderCoupon = new OrderCouponInput
			{
				OrderId = this.OrderInput.OrderId,
				CouponCode = string.Empty,
				CouponDispName = string.Empty,
				CouponName = string.Empty,
				LastChanged = this.LoginUser.Name
			};
			orderCoupons.Add(orderCoupon);
			orderInput.Coupons = orderCoupons.ToArray();
		}
	}

	/// <summary>
	/// キャンセル
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnModifyCancel_OnClick(object sender, EventArgs e)
	{
		ReloadOrderHistoryDetail();
	}

	/// <summary>
	/// 数量変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlProductCount_SelectedIndexChanged(object sender, EventArgs e)
	{
		WsNoveltyChangeNoticeMessage.InnerHtml = string.Empty;
		WsModifyErrorMessage.InnerHtml = string.Empty;

		var productId = ((DropDownList)sender).Attributes["data-productId"];
		var variationId = ((DropDownList)sender).Attributes["data-variationId"];
		var subscriptionBoxId = ((DropDownList)sender).Attributes["data-subscriptionBoxId"];
		var beforeCount = ((DropDownList)sender).Attributes["data-beforeCount"];

		var isSuccess = true;
		int inputCount;
		if ((int.TryParse(((DropDownList)sender).SelectedValue, out inputCount) == false) || (inputCount < 1))
		{
			WsModifyErrorMessage.InnerText = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_QUANTITY_UPDATE_ALERT);
			isSuccess = false;
		}

		var riShipping = GetParentRepeaterItem((DropDownList)sender, "rOrderShipping");
		var riShippingItem = GetParentRepeaterItem((DropDownList)sender, "rOrderInputShippingItem");

		if (riShipping == null || riShippingItem == null) return;
		var shippingProducts = this.OrderInput.Shippings[riShipping.ItemIndex].Items.ToList();
		if (this.OrderInputItemDisplay == null) this.OrderInputItemDisplay = new List<OrderItemInput>();
		var deletedProducts = this.OrderInputItemDisplay
			.Where(
				product => (product.OrderShippingNo == (riShipping.ItemIndex + 1).ToString())
					&& (shippingProducts.Any(product.IsSameProduct) == false))
			.ToList();
		if (deletedProducts.Any()) shippingProducts.AddRange(deletedProducts);
		var orderItem = shippingProducts[riShippingItem.ItemIndex];
		if (isSuccess)
		{
			var errorMessage = CalculatePrice(productId, variationId, true, orderItem, riShipping.ItemIndex);
			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				WsModifyErrorMessage.InnerHtml = errorMessage;
				isSuccess = false;
			}
		}

		if (isSuccess)
		{
			var subscriptionProducts = this.OrderInput.Shippings
				.SelectMany(s => s.Items
					.Where(p => string.IsNullOrEmpty(p.SubscriptionBoxCourseId)
						&& (p.SubscriptionBoxCourseId == subscriptionBoxId)))
				.ToList();
			var productCount = subscriptionProducts.Sum(s => int.Parse(s.ItemQuantity));
			var subscriptionPriceSubtotal = subscriptionProducts.Sum(s => decimal.Parse(s.ItemPrice));
			var productsCount = subscriptionProducts.Count;
			var errorMessage = CheckSubscriptionBoxInput(subscriptionBoxId, productCount, subscriptionPriceSubtotal, productsCount);
			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				WsModifyErrorMessage.InnerHtml = errorMessage;
				isSuccess = false;
			}
		}

		if (isSuccess)
		{
			// 決済上限、下限のチェック
			var paymentErrorMessage = IsPaymentPriceInRange(this.OrderInput);
			if (string.IsNullOrEmpty(paymentErrorMessage) == false)
			{
				this.WsModifyErrorMessage.InnerHtml = paymentErrorMessage;
				isSuccess = false;
			}
		}

		if (isSuccess)
		{
			// 半角の状態で上書き
			((DropDownList)sender).SelectedValue = inputCount.ToString();
		}
		else
		{
			((DropDownList)sender).SelectedValue = beforeCount;
			CalculatePrice(productId, variationId, true, orderItem, riShipping.ItemIndex);
		}

		// 数量変更時ノベルティ商品を外した場合、注意文言を表示
		if (Constants.NOVELTY_OPTION_ENABLED && isSuccess)
		{
			// 数量変更前ノベルティ数
			var noveltyOld = this.OrderInputOld.Shippings
				.SelectMany(item => item.Items).Where(p => string.IsNullOrEmpty(p.NoveltyId) == false)
				.ToList();
			var noveltyOldCount = noveltyOld.Sum(s => int.Parse(s.ItemQuantity));
			
			// 数量変更後ノベルティ数
			var noveltyNew = this.OrderInput.Shippings
				.SelectMany(item => item.Items).Where(p => string.IsNullOrEmpty(p.NoveltyId) == false)
				.ToList();
			var noveltyNewCount = noveltyNew.Sum(s => int.Parse(s.ItemQuantity));

			if (noveltyOldCount > noveltyNewCount)
			{
				this.WsNoveltyChangeNoticeMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_ORDER_MODIFY_REMOVE_NOVELTY);
			}
		}

		SetDisplayData();
	}

	/// <summary>
	/// 削除チェックボックス押下時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbDeleteProduct_OnCheckedChanged(object sender, EventArgs e)
	{
		WsNoveltyChangeNoticeMessage.InnerHtml = string.Empty;
		WsModifyErrorMessage.InnerHtml = string.Empty;

		var productId = ((CheckBox)sender).Attributes["data-productId"];
		var variationId = ((CheckBox)sender).Attributes["data-variationId"];
		var subscriptionBoxId = ((CheckBox)sender).Attributes["data-subscriptionBoxId"];

		var subscriptionProducts = this.OrderInput.Shippings
			.SelectMany(s => s.Items
				.Where(p => string.IsNullOrEmpty(p.SubscriptionBoxCourseId)
					&& (p.SubscriptionBoxCourseId ==subscriptionBoxId)))
			.ToList();
		var productCount = subscriptionProducts.Sum(s => int.Parse(s.ItemQuantity));
		var subscriptionPriceSubtotal = subscriptionProducts.Sum(s => decimal.Parse(s.ItemPrice));
		var productsCount = subscriptionProducts.Count;
		var errorMessage = CheckSubscriptionBoxInput(subscriptionBoxId, productCount, subscriptionPriceSubtotal, productsCount);
		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			WsModifyErrorMessage.InnerHtml = errorMessage;
			return;
		}

		var riShipping = GetParentRepeaterItem((CheckBox)sender, "rOrderShipping");
		var riShippingItem = GetParentRepeaterItem((CheckBox)sender, "rOrderInputShippingItem");
		var shippingProducts = this.OrderInput.Shippings[riShipping.ItemIndex].Items.ToList();
		if (this.OrderInputItemDisplay == null) this.OrderInputItemDisplay = new List<OrderItemInput>();
		var deletedProducts = this.OrderInputItemDisplay
			.Where(
				product => (product.OrderShippingNo == (riShipping.ItemIndex + 1).ToString())
					&& (shippingProducts.Any(product.IsSameProduct) == false))
			.ToList();
		if (deletedProducts.Any()) shippingProducts.AddRange(deletedProducts);
		var orderItem = shippingProducts[riShippingItem.ItemIndex];
		errorMessage = CalculatePrice(productId, variationId, false, orderItem, riShipping.ItemIndex);
		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			WsModifyErrorMessage.InnerHtml = errorMessage;
			return;
		}

		// 決済上限、下限のチェック
		var paymentErrorMessage = IsPaymentPriceInRange(this.OrderInput);
		if (string.IsNullOrEmpty(paymentErrorMessage) == false)
		{
			this.WsModifyErrorMessage.InnerHtml = paymentErrorMessage;
			return;
		}

		SetDisplayData();
	}

	/// <summary>
	/// 金額計算
	/// </summary>
	/// <param name="productId">商品ID</param>
	/// <param name="variationId">バリエーションID</param>
	/// <param name="addAutoNovelty">ノベルティを自動付与するか</param>
	/// <param name="orderItem">注文商品情報</param>
	/// <param name="shippingIndex">配送先リスト番号</param>
	/// <returns>エラーメッセージ</returns>
	private string CalculatePrice(string productId, string variationId, bool addAutoNovelty, OrderItemInput orderItem, int shippingIndex)
	{
		var product = new ProductService().GetProductVariation(this.ShopId, productId, variationId, this.MemberRankId);
		this.OrderInputOld = new OrderInput(this.OrderModel);
		// 現在の数量変更前の入力情報
		var beforeOrderInput = ObjectUtility.DeepCopy(this.OrderInput);
		var beforeOrderItemDisplay = ObjectUtility.DeepCopy(this.OrderInputItemDisplay);
		var errorMessage = GetOrderInput();

		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			this.OrderInput = beforeOrderInput;
			this.OrderInputItemDisplay = beforeOrderItemDisplay;
			return errorMessage;
		}

		var targetProductsOld = this.OrderInputOld.Shippings[shippingIndex].Items
			.Where(oldItem => oldItem.IsSameProduct(orderItem))
			.ToArray();
		var targetProducts = this.OrderInput.Shippings[shippingIndex].Items
			.Where(oldItem => orderItem.IsSameProduct(orderItem))
			.ToArray();

		var targetProductsCountOld = targetProductsOld.Sum(t => int.Parse(t.ItemQuantity));
		var targetProductsCount = targetProducts.Sum(t => int.Parse(t.ItemQuantity));
		if (targetProductsCountOld < targetProductsCount)
		{
			var targetProduct = targetProducts.FirstOrDefault(itemInput => itemInput.VariationId == variationId);
			int tmpItemQuantity;
			var targetProductCount =
				((targetProduct != null) && int.TryParse(targetProduct.ItemQuantity, out tmpItemQuantity))
					? tmpItemQuantity
					: 0;
			var hasStock = CheckProductStock(targetProductsCountOld, targetProductsCount, product);
			errorMessage = CanProductBuyable(
				product,
				hasStock,
				targetProductCount);
			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				this.OrderInput = beforeOrderInput;
				this.OrderInputItemDisplay = beforeOrderItemDisplay;
				return errorMessage;
			}
		}

		SetCoupon(this.OrderInputOld);
		SetCoupon(this.OrderInput);

		var calculate = new OrderModifyCalculate();
		errorMessage = calculate.ReCalculate(this.OrderInputOld, this.OrderInput, addAutoNovelty);

		if(string.IsNullOrEmpty(errorMessage)) this.OrderInput = calculate.OrderInput;

		return errorMessage;
	}

	/// <summary>
	/// 注文情報更新
	/// </summary>
	/// <param name="newOrder">注文情報</param>
	/// <param name="user">ユーザー情報</param>
	/// <param name="cart">カート</param>
	private void UpdateOrder(OrderModel newOrder, UserModel user, CartObject cart)
	{
		var oldOrder = this.OrderModel;
		newOrder.Items = newOrder.Shippings.SelectMany(s => s.Items).ToArray();
		var updater = new OrderUpdaterFront(
			user,
			newOrder,
			oldOrder,
			user.UserMemo,
			user.UserManagementLevelId,
			Constants.FLG_LASTCHANGED_USER,
			isMyPageModify: true);
		var frontErrorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_ORDER_MODIFY_FAILURE);
		var transactionName = updater.CreateOrderNew(
			doStockCooperation: true,
			doAlertOutOfStock: true,
			updateHistoryAction: UpdateHistoryAction.DoNotInsert,
			cart: cart);
		if (string.IsNullOrEmpty(transactionName) == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = frontErrorMessage;
			var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR)
				.AddParam(Constants.REQUEST_KEY_ERRORPAGE_KBN, Constants.KBN_REQUEST_ERRORPAGE_GOTOP)
				.CreateUrl();
			Response.Redirect(url);
		}

		// 外部決済連携実行
		var errorMessage = string.Empty;
		var isExecuteExternalPayment = updater.ExecuteExternalPayment(
			ReauthCreatorFacade.ExecuteTypes.System,
			ReauthCreatorFacade.ExecuteTypes.System,
			UpdateHistoryAction.DoNotInsert,
			out errorMessage);

		if (isExecuteExternalPayment || (string.IsNullOrEmpty(errorMessage) == false))
		{
			OrderCommon.AppendExternalPaymentCooperationLog(
				isExecuteExternalPayment,
				newOrder.OrderId,
				isExecuteExternalPayment ? LogCreator.CreateMessage(newOrder.OrderId, "") : errorMessage,
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.Insert);
		}

		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = frontErrorMessage;
			var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR)
				.AddParam(Constants.REQUEST_KEY_ERRORPAGE_KBN, Constants.KBN_REQUEST_ERRORPAGE_GOTOP)
				.CreateUrl();
			Response.Redirect(url);
		}
		// 注文情報更新
		var isSuccess = updater.ExecuteUpdateOrderAndRegisterUpdateHistory(
			isRegisterCreditCard: false,
			isUpdateCreditCard: false,
			isExecuteExternalPayment: isExecuteExternalPayment,
			doStockCooperation: true,
			doAlertOutOfStock: true,
			actionType: OrderHistory.ActionType.FrontOrderConfirm,
			updateHisoryAction: UpdateHistoryAction.Insert,
			cart: cart,
			errorMessage: out errorMessage,
			newOrder: newOrder,
			isOrderHisoryBulkUpdate: true);
		if (isSuccess != OrderUpdaterFront.ResultType.Success)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = frontErrorMessage;
			var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR)
				.AddParam(Constants.REQUEST_KEY_ERRORPAGE_KBN, Constants.KBN_REQUEST_ERRORPAGE_GOTOP)
				.CreateUrl();
			Response.Redirect(url);
		}

		// メール送信
		SendMailCommon.SendModifyPurchaseHistoryMail(this.OrderModel.OrderId, SendMailCommon.PurchaseHistoryModify.Product);
	}

	/// <summary>
	/// 商品在庫チェック
	/// </summary>
	/// <param name="beforeProductCount">現在の商品数</param>
	/// <param name="inputProductCount">商品数の入力値</param>
	/// <param name="productVariation">商品情報</param>
	/// <returns>在庫エラー商品名リスト</returns>
	protected bool CheckProductStock(int beforeProductCount, int inputProductCount, ProductModel productVariation)
	{
		// 在庫管理しない・在庫が無くても販売可能の場合は在庫チェック無し
		if ((productVariation.StockManagementKbn == Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED)
			|| (productVariation.StockManagementKbn == Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_DISPOK_BUYOK)) return true;

		var productStock = new ProductStockService().Get(
			productVariation.ShopId,
			productVariation.ProductId,
			productVariation.VariationId);
		// 「購入数(変更後購入数 - 変更前購入数) > 在庫数」の場合エラー
		if ((productStock == null) || ((inputProductCount - beforeProductCount) > productStock.Stock))
		{
			return false;
		}

		return true;
	}

	/// <summary>
	/// 購入可能判定
	/// </summary>
	/// <param name="product">商品</param>
	/// <param name="hasStock">在庫有無</param>
	/// <param name="targetProductCount">更新後商品数</param>
	/// <returns>エラーメッセージ</returns>
	private string CanProductBuyable(ProductModel product, bool hasStock, int targetProductCount)
	{
		var hasVariation = product.HasProductVariation;
		// バリエーション選択状況に応じて「商品名(＋バリエーション名)」を置換
		var productName = (hasVariation)
			? product.Name
			: ProductCommon.CreateProductJointName(product);
		var maxSellQuantity = (hasVariation)
			? (int)product.MaxSellQuantity + 1
			: (int)GetKeyValue(product, Constants.FIELD_PRODUCT_MAX_SELL_QUANTITY) + 1;
		// 販売可能か判定
		var canBuyable = CanSalable(product);
		var errorMessage = string.Empty;
		if ((hasStock == false)
			|| (canBuyable == false)
			|| (this.CanSalableMaxSellQuantityLimit(product.MaxSellQuantity, targetProductCount) == false))
		{
			errorMessage = "「@@ 1 @@」は「@@ 2 @@」個以上一度に購入することは出来ません。"
				.Replace("@@ 1 @@", productName)
				.Replace("@@ 2 @@", maxSellQuantity.ToString());
			return errorMessage;
		}

		return errorMessage;
	}

	/// <summary>
	/// 販売可能か判定
	/// </summary>
	/// <param name="product">商品情報</param>
	/// <returns>結果</returns>
	public bool CanSalable(ProductModel product)
	{
		// 購入可能会員ランク
		var buyableMemberRank = product.BuyableMemberRank;

		// 購入可能な会員ランク
		if (MemberRankOptionUtility.CheckMemberRankPermission(this.MemberRankId, buyableMemberRank) == false)
		{
			return false;
		}

		// 商品販売前
		if (ProductCommon.IsSellBefore(product))
		{
			return false;
		}

		// 商品販売後
		if ((product.SellFrom > DateTime.Now)
			&& (product.SellTo.HasValue
				|| (product.SellTo < DateTime.Now)))
		{
			return false;
		}

		// 商品販売期間外
		if (((product.SellFrom <= DateTime.Now)
				&& (product.SellTo.HasValue
					|| (product.SellTo >= DateTime.Now))) == false)
		{
			//return false;
		}

		return true;
	}

	/// <summary>
	/// 注文入力情報取得
	/// </summary>
	/// <returns>注文入力情報</returns>
	private string GetOrderInput()
	{
		var orderOld = new OrderInput(this.OrderModel);
		var order = ObjectUtility.DeepCopy(this.OrderInput) ?? new OrderInput(this.OrderModel);
		foreach (var shipping in order.Shippings)
		{
			if (this.OrderInputItemDisplay == null) this.OrderInputItemDisplay = new List<OrderItemInput>();

			var deletedProducts = this.OrderInputItemDisplay
				.Where(product => product.OrderShippingNo == shipping.OrderShippingNo)
				.ToList();
			if (deletedProducts.Any())
			{
				var deleteProduct = shipping.Items.ToList();
				deleteProduct.AddRange(deletedProducts);
				shipping.Items = deleteProduct.ToArray();
			}

			// 更新前の注文情報より商品の並びをソート(削除チェック)
			var shippingOld = this.OrderInputOld.Shippings.FirstOrDefault(
				shippingInput => shippingInput.OrderShippingNo == shipping.OrderShippingNo);
			if (shippingOld != null)
			{
				var sortProducts = shippingOld.Items
					.SelectMany(productOld => shipping.Items
						.Where(product => product.CheckOrderHistoryModifyCopyTargetByOldOrderItem(productOld)))
					.ToList();

				sortProducts.AddRange(shipping.Items
					.Where(product => (string.IsNullOrEmpty(product.ProductBundleId) == false)
						&& (product.BundleItemDisplayType == Constants.FLG_ORDERITEM_BUNDLE_ITEM_DISPLAY_TYPE_VALID)
						&& (shippingOld.Items
							.Any(productOld => product.IsSameProductWithPrice(productOld)) == false)));
				shipping.Items = sortProducts.ToArray();
			}
		}
		var loopNo = 1;
		foreach (RepeaterItem item in this.WrOrderShipping.Items)
		{
			var orderItems = new List<OrderItemInput>();
			var productrepeater = (Repeater)item.FindControl("rOrderInputShippingItem");
			foreach (RepeaterItem rItem in productrepeater.Items)
			{
				var quantity = ((DropDownList)rItem.FindControl("ddlProductCount")).SelectedValue;
				var quantityOld = quantity;
				var targetProduct = order.Shippings[item.ItemIndex].Items[rItem.ItemIndex];

				// 削除対象の場合
				var deleteProduct = GetWrappedControl<WrappedCheckBox>(rItem, "cbDeleteProduct");
				if (deleteProduct.HasInnerControl == false || deleteProduct.Checked)
				{
					targetProduct.ModifyDeleteTarget = true;
					if (this.OrderInputItemDisplay == null) this.OrderInputItemDisplay = new List<OrderItemInput>();
					var hasSameProduct = this.OrderInputItemDisplay.Any(
						orderItem => orderItem.IsSameProduct(targetProduct));
					if(hasSameProduct == false) this.OrderInputItemDisplay.Add(targetProduct);
					continue;
				}
				else
				{
					targetProduct.ModifyDeleteTarget = false;
					if (this.OrderInputItemDisplay == null) this.OrderInputItemDisplay = new List<OrderItemInput>();
					var hasSameProduct = this.OrderInputItemDisplay.Any(
						orderItem => orderItem.IsSameProduct(targetProduct));
					if (hasSameProduct) this.OrderInputItemDisplay.Remove(targetProduct);
				}

				var product = ProductCommon.GetProductVariationInfo(
					targetProduct.ShopId,
					targetProduct.ProductId,
					targetProduct.VariationId,
					order.MemberRankId,
					targetProduct.ProductsaleId);
				var hasVariation = targetProduct.ProductId != targetProduct.VariationId;
				var productPrice = (targetProduct.IsFixedPurchaseItem)
					? decimal.Parse(ProductPrice.GetFixedPurchasePrice(product[0], hasVariation, targetProduct.FixedPurchaseItemIsFirstOrder))
					: GetProductValidityPrice(product[0], hasVariation, hasVariation);
				var hasOldProduct = orderOld.Shippings[item.ItemIndex].Items
					.FirstOrDefault(orderItem =>
						orderItem.IsSameProductWithPrice(targetProduct)
						&& (targetProduct.IsOrderHistoryModifyCopyTarget == false));
				var oldProduct = ObjectUtility.DeepCopy(hasOldProduct ?? targetProduct);

				// 変更前の価格の方が安いかつ商品数が増えてた場合は行を分ける
				if ((decimal.Parse(oldProduct.ProductPrice) < productPrice)
					&& (int.Parse(oldProduct.ItemQuantity) < int.Parse(quantity)))
				{
					oldProduct.OrderItemNo = loopNo.ToString();
					oldProduct.ItemIndex = loopNo.ToString();
					loopNo++;
					orderItems.Add(oldProduct);
					quantity = (int.Parse(quantity) - int.Parse(oldProduct.ItemQuantity)).ToString();
					targetProduct.ItemIndex = (order.Shippings[item.ItemIndex].Items.Length + 1).ToString();
					targetProduct.IsOrderHistoryModifyCopyTarget = true;
				}

				// 変更前の価格の方が安いかつ商品数が減っていた場合は価格を引き継ぐ
				if ((decimal.Parse(oldProduct.ProductPrice) <= productPrice)
					&& (int.Parse(oldProduct.ItemQuantity) >= int.Parse(quantityOld)))
				{
					productPrice = decimal.Parse(oldProduct.ProductPrice);
				}

				// 既に追加済みの場合は個数を増やす
				var addedProduct = orderItems.FirstOrDefault(
					orderItem => (orderItem.IsSameProduct(targetProduct) && (decimal.Parse(orderItem.ProductPrice) == productPrice)));
				if (addedProduct != null)
				{
					var addQuantity = int.Parse(quantity);
					addedProduct.ItemQuantity = (int.Parse(addedProduct.ItemQuantity) + addQuantity).ToString();
					addedProduct.ItemQuantitySingle = (int.Parse(addedProduct.ItemQuantitySingle) + addQuantity).ToString();
				}
				else
				{
					targetProduct.OrderItemNo = loopNo.ToString();
					targetProduct.ItemIndex = loopNo.ToString();
					loopNo++;
					targetProduct.ItemQuantity = quantity;
					targetProduct.ItemQuantitySingle = quantity;
					targetProduct.ProductPrice = productPrice.ToString();
					orderItems.Add(targetProduct);
				}
			}

			order.Shippings[item.ItemIndex].Items = orderItems.ToArray();
		}

		var errorMessage = string.Empty;
		if (order.Shippings.Any(s => s.Items.Any(p => string.IsNullOrEmpty(p.NoveltyId) && string.IsNullOrEmpty(p.ProductBundleId) && (p.ModifyDeleteTarget == false)) == false))
		{
			errorMessage = "商品が設定されていません。商品は１つ以上設定して下さい。";
		}
		else
		{
			this.OrderInput = order;
			SetCoupon(this.OrderInput);
		}

		return errorMessage;
	}

	/// <summary>
	/// 頒布会チェック
	/// </summary>
	/// <param name="subscriptionBoxId">頒布会コースID</param>
	/// <param name="productCount">商品数</param>
	/// <param name="subscriptionPriceSubtotal">金額</param>
	/// <param name="productsCount">商品種類数</param>
	/// <returns>エラーメッセージ</returns>
	private string CheckSubscriptionBoxInput(
		string subscriptionBoxId,
		int productCount,
		decimal subscriptionPriceSubtotal,
		int productsCount)
	{
		var errorMessage = string.Empty;
		if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED && (string.IsNullOrEmpty(subscriptionBoxId) == false))
		{
			errorMessage = OrderCommon.CheckLimitProductOrderForSubscriptionBox(subscriptionBoxId, productCount);
			if (string.IsNullOrEmpty(errorMessage)) errorMessage = OrderCommon.GetSubscriptionBoxTotalAmountError(subscriptionBoxId, subscriptionPriceSubtotal);
			if (string.IsNullOrEmpty(errorMessage)) errorMessage = OrderCommon.GetSubscriptionBoxProductOfNumberError(subscriptionBoxId, productsCount, true);
		}

		return errorMessage;
	}

	/// <summary>
	/// 画面表示用データ設定
	/// </summary>
	private void SetDisplayData()
	{
		this.OrderItemSerialKeys = new Dictionary<string, DataView>();
		var orderShippingItem = this.OrderShippingItems.Select(item => (List<DataRowView>)item["childs"]);
		foreach (RepeaterItem riShipping in WrOrderShipping.Items)
		{
			var rProduct = (Repeater)riShipping.FindControl("rOrderInputShippingItem");
			var shippingProducts = orderShippingItem.SelectMany(childs => childs
				.Select(orderItem => new OrderItemModel(orderItem)))
				.SelectMany((product, index) => this.OrderInput.Shippings[riShipping.ItemIndex].Items
					.Where(item => item.CheckOrderHistoryModifyCopyTargetByOldOrderItem(new OrderItemInput(product, index))))
				.ToList();

			if (this.OrderInputItemDisplay == null) this.OrderInputItemDisplay = new List<OrderItemInput>();
			var deletedProducts = this.OrderInputItemDisplay
				.Where(
					product => (product.OrderShippingNo == (riShipping.ItemIndex + 1).ToString())
						&& (shippingProducts.Any(product.IsSameProduct) == false))
				.ToList();
			if (deletedProducts.Any())
			{
				// 削除対象商品はセットプロモーション情報を表示しない
				foreach (var product in deletedProducts)
				{
					product.OrderSetpromotionNo = null;
				}
				shippingProducts.AddRange(deletedProducts);
			}

			// 更新前の注文情報より商品の並びをソート(商品表示)
			var sortShippingProducts = new List<OrderItemInput>();

			sortShippingProducts.AddRange(this.OrderInputOld.Shippings[riShipping.ItemIndex].Items
				.SelectMany(productOld => shippingProducts
					.Where(product => product.CheckOrderHistoryModifyCopyTargetByOldOrderItem(productOld))));

			sortShippingProducts.AddRange(this.OrderInput.Shippings[riShipping.ItemIndex].Items
				.Where(product => (string.IsNullOrEmpty(product.ProductBundleId) == false)
					&& (product.BundleItemDisplayType == Constants.FLG_ORDERITEM_BUNDLE_ITEM_DISPLAY_TYPE_VALID)
					&& (this.OrderInputOld.Shippings[riShipping.ItemIndex].Items
						.Any(productOld => product.IsSameProductWithPrice(productOld)) == false)));

			foreach (var product in sortShippingProducts)
			{
				// 「購入商品毎 ｘ 数量」に応じたシリアルキーを取得
				if(this.OrderItemSerialKeys.ContainsKey(product.OrderId + product.OrderItemNo)) continue;
				this.OrderItemSerialKeys.Add(
					product.OrderId + product.OrderItemNo,
					OrderCommon.GetSerialKeyList(product));
			}
			rProduct.DataSource = sortShippingProducts;
			rProduct.DataBind();
		}

		var orderModel = this.OrderInput.CreateModel();
		this.WlOrderPriceSubtotal.Text = CurrencyManager.ToPrice(orderModel.OrderPriceSubtotal);
		this.WlOrderPriceSubtptalTax.Text = CurrencyManager.ToPrice(orderModel.OrderPriceSubtotalTax);
		this.WlOrderMemberRankDiscountPrice.Text = ((orderModel.MemberRankDiscountPrice > 0) ? "-" : "")
			+ CurrencyManager.ToPrice(orderModel.MemberRankDiscountPrice);
		this.WlFixedPurchaseMemberDiscountAmount.Text = ((orderModel.FixedPurchaseMemberDiscountAmount > 0) ? "-" : "")
			+ CurrencyManager.ToPrice(orderModel.FixedPurchaseMemberDiscountAmount);
		this.WlCouponName.Text = GetCouponName(orderModel);
		this.WlOrderCouponUse.Text = ((orderModel.OrderCouponUse > 0) ? "-" : "")
			+ CurrencyManager.ToPrice(orderModel.OrderCouponUse);
		this.WlOrderPointUseYen.Text = ((orderModel.OrderPointUseYen > 0) ? "-" : "")
			+ CurrencyManager.ToPrice(orderModel.OrderPointUseYen);
		this.WlFixedPurchaseDiscountPrice.Text = ((orderModel.FixedPurchaseDiscountPrice > 0) ? "-" : "")
			+ CurrencyManager.ToPrice(orderModel.FixedPurchaseDiscountPrice);
		this.WlOrderPriceRegulation.Text = ((orderModel.OrderPriceRegulation < 0) ? "-" : "")
			+ CurrencyManager.ToPrice(Math.Abs(orderModel.OrderPriceRegulation));
		this.WlOrderPriceShipping.Text = CurrencyManager.ToPrice(orderModel.OrderPriceShipping);
		this.WlOrderPriceExchange.Text = CurrencyManager.ToPrice(orderModel.OrderPriceExchange);
		this.WlOrderPriceTotal.Text = CurrencyManager.ToPrice(orderModel.OrderPriceTotal);

		var products = orderModel.Shippings.SelectMany(s => s.Items).ToArray();
		this.WhfhfProducts.Value = string.Join(",", products.Select(i => i.ProductName).ToArray());
		this.WhfCounts.Value = string.Join(",", products.Select(i => i.ItemQuantity.ToString()).ToArray());
		this.WhfOrderPriceSubtotalNew.Value = CurrencyManager.ToPrice(orderModel.OrderPriceTotal);
		// 注文セットプロモーション情報セット
		this.OrderSetPromotions = new List<Hashtable>();
		var htSetPromotions = orderModel.SetPromotions.Select(s => s.DataSource).ToList();

		this.OrderSetPromotions = htSetPromotions;
		this.WrProductSetPromotions.DataSource = htSetPromotions;
		this.WrProductSetPromotions.DataBind();
		this.WrShippingSetPromotions.DataSource = htSetPromotions;
		this.WrShippingSetPromotions.DataBind();
		this.WrPaymentSetPromotions.DataSource = htSetPromotions;
		this.WrPaymentSetPromotions.DataBind();
	}

	/// <summary>
	/// セットプロモーション割引可否判定
	/// </summary>
	/// <param name="product">商品</param>
	/// <param name="command">対象</param>
	/// <returns>結果</returns>
	protected bool HasSetPromotionDiscount(OrderItemInput product, string command)
	{
		var setPromotion = this.OrderInput.SetPromotions.FirstOrDefault(s => s.OrderSetpromotionNo == product.OrderSetpromotionNo);
		if (setPromotion == null) return false;

		var result = false;
		switch (command)
		{
			case Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_FLG:
				result = setPromotion.IsDiscountTypeProductDiscount;
				break;

			case Constants.FIELD_ORDERSETPROMOTION_SHIPPING_CHARGE_FREE_FLG:
				result = setPromotion.IsDiscountTypeShippingChargeFree;
				break;

			case Constants.FIELD_ORDERSETPROMOTION_PAYMENT_CHARGE_FREE_FLG:
				result = setPromotion.IsDiscountTypePaymentChargeFree;
				break;
		}

		return result;
	}

	/// <summary>
	/// セットプロモーション割引額取得
	/// </summary>
	/// <param name="product">商品</param>
	/// <param name="command">対象</param>
	/// <param name="beforeDiscounted">割引前か</param>
	/// <returns></returns>
	protected decimal GetSetPromotionDiscount(OrderItemInput product, string command, bool beforeDiscounted)
	{
		var setPromotion = this.OrderInput.SetPromotions.FirstOrDefault(s => s.OrderSetpromotionNo == product.OrderSetpromotionNo);
		if (setPromotion == null) return 0m;

		var result = 0m;
		switch (command)
		{
			case Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_FLG:
				result = beforeDiscounted
					? decimal.Parse(setPromotion.UndiscountedProductSubtotal)
					: decimal.Parse(setPromotion.UndiscountedProductSubtotal) - decimal.Parse(setPromotion.ProductDiscountAmount);
				break;

			case Constants.FIELD_ORDERSETPROMOTION_SHIPPING_CHARGE_FREE_FLG:
				result = beforeDiscounted
					? decimal.Parse(setPromotion.UndiscountedProductSubtotal)
					: decimal.Parse(setPromotion.UndiscountedProductSubtotal) - decimal.Parse(setPromotion.ProductDiscountAmount);
				break;

			case Constants.FIELD_ORDERSETPROMOTION_PAYMENT_CHARGE_FREE_FLG:
				result = beforeDiscounted
					? decimal.Parse(setPromotion.UndiscountedProductSubtotal)
					: decimal.Parse(setPromotion.UndiscountedProductSubtotal) - decimal.Parse(setPromotion.ProductDiscountAmount);
				break;
		}

		return result;
	}

	/// <summary>
	/// セットプロモーション名取得
	/// </summary>
	/// <param name="product">商品</param>
	/// <returns>セットプロモーション名</returns>
	protected string GetSetPromotionDispName(OrderItemInput product)
	{
		var setPromotion = this.OrderInput.SetPromotions.FirstOrDefault(s => s.OrderSetpromotionNo == product.OrderSetpromotionNo);
		if (setPromotion == null) return string.Empty;

		return setPromotion.SetpromotionDispName;
	}

	/// <summary>
	/// 数量変更可能か判定
	/// </summary>
	/// <param name="product">商品情報</param>
	/// <returns>結果</returns>
	protected bool GetCanModifiyProductCount(OrderItemInput product)
	{
		var result = string.IsNullOrEmpty(product.NoveltyId) && string.IsNullOrEmpty(product.ProductBundleId);

		return result;
	}

	/// <summary>
	/// 削除可能か判定
	/// </summary>
	/// <param name="product">商品情報</param>
	/// <returns>結果</returns>
	protected bool GetCanDeleteProduct(OrderItemInput product)
	{
		var result = string.IsNullOrEmpty(product.ProductBundleId);

		return result;
	}

	/// <summary>
	/// 注文情報入力クラス設定
	/// </summary>
	/// <param name="order">注文情報</param>
	protected void SetOrderInput(OrderModel order)
	{
		this.OrderInput = new OrderInput(order);

		foreach (var shipping in this.OrderInput.Shippings)
		{
			foreach (var product in shipping.Items)
			{
				var oldProduct = order.Items
					.FirstOrDefault(p => (p.ShopId == product.ShopId)
						&& (p.ProductId == product.ProductId)
						&& (p.VariationId == product.VariationId));
				if (oldProduct == null) continue;

				product.NoveltyId = oldProduct.NoveltyId;
				product.ProductBundleId = oldProduct.ProductBundleId;
				product.BundleItemDisplayType = oldProduct.BundleItemDisplayType;
			}
		}
	}

	/// <summary>
	/// 商品情報設定
	/// </summary>
	/// <param name="command">対象</param>
	/// <returns>商品情報</returns>
	protected string SetProduct(int command)
	{
		var result = "";
		var products = this.OrderModel.Shippings.SelectMany(s => s.Items).ToArray();
		if (command == 0)
		{
			result = string.Join(",", products.Select(i => i.ProductName).ToArray());
		}
		else
		{
			result = string.Join(",", products.Select(i => i.ItemQuantity.ToString()).ToArray());
		}

		return result;
	}

	/// <summary>
	/// オプション価格があるか
	/// </summary>
	/// <param name="orderItems">注文商品一覧</param>
	/// <returns>オプション価格あり：true、オプション価格なし：false</returns>
	protected bool HasOptionPrice(List<Hashtable> orderItems)
	{
		if (this.OrderModel.IsSubscriptionBoxFixedAmount
			|| (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED == false))
		{
			return false;
		}

		var result = orderItems.SelectMany(orderItem => (List<DataRowView>)orderItem["childs"])
			.Any(orderItemChilds =>
				ProductOptionSettingHelper.HasOptionPrice(
					(string)orderItemChilds[Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS]));

		return result;
	}

	/// <summary>
	/// 商品変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rOrderInputShippingItem_OnItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		CreateItemQuantityDropdownList(sender, e);
	}

	/// <summary>
	/// 商品数変更のドロップダウンリスト作成
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void CreateItemQuantityDropdownList(object sender, RepeaterItemEventArgs e)
	{
		if ((e.Item.ItemType == ListItemType.Item) || (e.Item.ItemType == ListItemType.AlternatingItem))
		{
			var ddlQuantityUpdate = (DropDownList)e.Item.FindControl("ddlProductCount");
			var maxValueHiddenField = (HiddenField)e.Item.FindControl("hfMaxItemQuantity");
			var itemQuantityHiddenField = (HiddenField)e.Item.FindControl("hfItemQuantity");

			var maxValue = int.Parse(maxValueHiddenField.Value);
			var itemQuantity = int.Parse(itemQuantityHiddenField.Value);

			// 配送先リピーターアイテムを取得
			var parentItem = (RepeaterItem)((Repeater)sender).Parent.Parent;
			// 配送先リピーターを取得
			var parentRepeater = (Repeater)parentItem.Parent;

			// 子リピーターのインデックスで初期化
			var result = e.Item.ItemIndex;
			for (var num = 0; num < parentRepeater.Items.Count - 1; num++)
			{
				// 配送先が1つの場合はe.ItemIndexをそのまま使う
				if (parentItem.ItemIndex == 0) break;
				var childRepeater = (Repeater)parentRepeater.Items[num].FindControl("rOrderInputShippingItem");
				result += childRepeater.Items.Count;
			}

			// hiddenfieldに設定されている規定最大値と注文数のうち大きい方を最大値にする
			var limit = Math.Max(maxValue, this.OldItems[result].ItemQuantity);

			// 最大値を元にドロップダウンリストを作成する
			ddlQuantityUpdate.Items.Clear();
			for (var i = 1; i <= limit; i++)
			{
				ddlQuantityUpdate.Items.Add(new ListItem(i.ToString(), i.ToString()));
			}
			// 選択値をセットする
			ddlQuantityUpdate.SelectedValue = itemQuantity.ToString();
		}
	}

	#region プロパティ
	/// <summary>送金額</summary>
	protected string SendingAmount
	{
		get { return CurrencyManager.ToSettlementCurrencyNotation(this.OrderModel.SettlementAmount, this.OrderModel.SettlementCurrency); }
	}

	/// <summary>ポイントのエラーメッセージ</summary>
	protected string ErrorMessageOrderPointUse { get; private set; }
	/// <summary>利用ポイント入力フォームの表示状態</summary>
	protected bool IsOrderPointAddDisplayStatus
	{
		get { return (bool)(ViewState["IsOrderPointAddDisplayStatus"] ?? false); }
		set { ViewState["IsOrderPointAddDisplayStatus"] = value; }
	}
	/// <summary>定期購入履歴の注文IDリスト</summary>
	protected string[] SimilarShippingOrderIdList
	{
		get { return (string[])(ViewState["SimilarShippingOrderIdList"]); }
		set { ViewState["SimilarShippingOrderIdList"] = value; }
	}
	/// <summary>Order history error message</summary>
	protected string OrderHistoryErrorMessage
	{
		get { return (string)Session[ORDER_HISTORY_ERROR_MESSAGE]; }
		set { Session[ORDER_HISTORY_ERROR_MESSAGE] = value; }
	}
	/// <summary>Need Scroll To Shipping Area</summary>
	protected bool NeedScrollToShippingArea
	{
		get { return (Session["NeedScrollToShippingArea"] != null) ? (bool)Session["NeedScrollToShippingArea"] : false; }
		set { Session["NeedScrollToShippingArea"] = value; }
	}
	/// <summary>Apiエラーメッセージ</summary>
	public string ApiErrorMessage { get; private set; }
	/// <summary>領収書情報更新のエラーメッセージ</summary>
	protected string ReceiptInfoModifyErrorMessage { get; set; }
	/// <summary>領収書情報変更有無</summary>
	protected bool IsReceiptInfoModify
	{
		get { return ((this.ViewState["IsReceiptInfoModify"] != null) && (bool)this.ViewState["IsReceiptInfoModify"]); }
		set { this.ViewState["IsReceiptInfoModify"] = value; }
	}
	/// <summary>領収書情報変更可能かの判定</summary>
	protected bool CanModifyReceiptInfo
	{
		get
		{
			return ((string.IsNullOrEmpty(CheckUpdateReceiptInfoError(this.OrderModel)))
				&& (this.IsModifyOrder));
		}
	}
	/// <summary>Is Show Taiwan Order Invoice Information</summary>
	protected bool IsShowTaiwanOrderInvoiceInfo
	{
		get
		{
			return (OrderCommon.DisplayTwInvoiceInfo(this.OrderModel.Shippings[0].ShippingCountryIsoCode)
				&& (this.TwOrderInvoiceModel != null));
		}
	}
	/// <summary> Use Shipping Address </summary>
	protected bool UseShippingAddress
	{
		get
		{
			return (this.OrderModel.Shippings[0].ShippingReceivingStoreFlg
				== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF);
		}
	}
	/// <summary> Use Shipping Receiving Store </summary>
	protected bool UseShippingReceivingStore
	{
		get
		{
			return (this.OrderModel.Shippings[0].ShippingReceivingStoreFlg
				== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON);
		}
	}
	/// <summary>Is Checked Payment Id Changed</summary>
	public string CheckedPaymentIdChanged { get; set; }
	/// <summary>後払い請求書が再発行可能か（利用可能かは決済代行会社にもよります）</summary>
	public bool CanRequestCvsDefInvoiceReissue
	{
		get
		{
			return (this.OrderModel.CanRequestCvsDefInvoiceReissue
				&& ((Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Gmo)
					|| (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Dsk)
					|| (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Dsk)));
		}
	}
	/// <summary>Is Order Cvs Def Invoice Reissue Requested</summary>
	public bool IsOrderCvsDefInvoiceReissueRequested
	{
		get
		{
			return (this.OrderModel.ExtendStatus38 == Constants.FLG_ORDER_ORDER_EXTEND_STATUS_ON);
		}
	}
	/// <summary>アマゾンリクエスト</summary>
	protected AmazonCv2Redirect AmazonRequest { get; set; }
	/// <summary>アマゾンチェックアウトセッション</summary>
	private CheckoutSessionResponse AmazonCheckoutSession { get; set; }
	/// <summary>アマゾンチェックアウトセッションId</summary>
	private string AmazonCheckoutSessionId { get; set; }
	/// <summary>アマゾンCv2Facade</summary>
	private AmazonCv2ApiFacade AmazonFacade { get; set; }
	/// <summary>スマホ決済完了時に実行するメソッド名</summary>
	private string OnAuthorizeCompleteMethodName
	{
		get { return (string)ViewState["OnAuthorizeCompleteMethodName"]; }
		set { ViewState["OnAuthorizeCompleteMethodName"] = value; }
	}
	/// <summary>注文拡張項目の変更有無</summary>
	protected bool IsOrderExtendModify
	{
		get { return ((this.ViewState["IsOrderExtendModify"] != null) && (bool)this.ViewState["IsOrderExtendModify"]); }
		set { this.ViewState["IsOrderExtendModify"] = value; }
	}
	/// <summary>定期購入の支払い方法にも更新するかの項目表示可能か</summary>
	protected bool CanDisplayChangeFixedPurchasePayment
	{
		get
		{
			return (this.IsFixedPurchase
				&& (this.FixedPurchaseModel.IsCancelFixedPurchaseStatus == false)
				&& (OrderCommon.IsOnlyCancelablePaymentContinuousByManual(this.FixedPurchaseModel.OrderPaymentKbn) == false));
		}
	}
	/// <summary>与信結果がHOLDまたは保留の注文か(現在はコンビニ後払い「DSK」のみ利用)</summary>
	protected bool IsHoldOrder
	{
		get
		{
			return ((this.OrderModel.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_MIDST)
					|| (this.OrderModel.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_PEND));
		}
	}
	/// <summary>領収書出力エラーメッセージ</summary>
	public string ReceiptDownloadErrorMessage { get; set; }
	/// <summary>Is execution PayPay order</summary>
	protected bool IsExecutionPayPayOrder
	{
		get
		{
			return (string.IsNullOrEmpty(Request[PaypayConstants.REQUEST_KEY_RECEIVE_RESULT]) == false);
		}
	}
	/// <summary>Can use point for purchase</summary>
	protected bool CanUsePointForPurchase
	{
		get
		{
			var result = (this.OrderModel.PurchasePriceTotal >= Constants.POINT_MINIMUM_PURCHASEPRICE);
			return result;
		}
	}
	/// <summary>Can display point use</summary>
	protected bool CanDisplayPointUse
	{
		get
		{
			var result = (this.IsOrderPointAddDisplayStatus
				&& this.CanUsePointForPurchase);
			return result;
		}
	}
	/// <summary>商品数変更状態か</summary>
	protected bool IsProductModify
	{
		get { return (ViewState["IsProductModify"] != null) && (bool)ViewState["IsProductModify"]; }
		set { ViewState["IsProductModify"] = value; }
	}
	/// <summary>変更後商品情報</summary>
	protected OrderModel OrderModelNew
	{
		get { return (OrderModel)ViewState["OrderInputNEW"]; }
		set { ViewState["OrderInputNEW"] = value; }
	}
	/// <summary>入力情報</summary>
	protected OrderInput OrderInput
	{
		get { return (OrderInput)ViewState["OrderInput"]; }
		set { ViewState["OrderInput"] = value; }
	}
	/// <summary>入力情報(表示用)</summary>
	protected OrderInput OrderInputDisplay
	{
		get { return (OrderInput)ViewState["OrderInputDisplay"] ?? this.OrderInput; }
		set { ViewState["OrderInputDisplay"] = value; }
	}
	protected List<OrderItemInput> OrderInputItemDisplay
	{
		get { return (List<OrderItemInput>)ViewState["OrderInputItemDisplay"]; }
		set { ViewState["OrderInputItemDisplay"] = value; }
	}
	/// <summary>更新前の入力情報</summary>
	protected OrderInput OrderInputOld
	{
		get { return (OrderInput)ViewState["OrderInputOld"]; }
		set { ViewState["OrderInputOld"] = value; }
	}
	/// <summary>商品数変更ボタンを表示するか</summary>
	protected bool IsOrderModifyBtnDisplay
	{
		get
		{
			return this.IsModifyCancel
				&& this.IsModifyOrder
				&& this.CanUseModifyPayment
				&& (PaygentUtility.CheckIsPaidyPaygentPayment(this.OrderModel.OrderPaymentKbn) == false);
		}
	}
	/// <summary>Get real shop by id</summary>
	protected RealShopModel RealShopModel
	{
		get
		{
			return (RealShopModel)ViewState["RealShopModel"];
		}
	}
	/// <summary>Is pickup real shop</summary>
	protected bool IsPickupRealShop
	{
		get
		{
			return (this.OrderModel.Shippings[0].StorePickupRealShopId != string.Empty);
		}
	}
	/// <summary>Has subcription box item</summary>
	protected bool HasSubscriptionBoxItemModify
	{
		get
		{
			return this.IsProductModify && this.OrderModel.HasSubscriptionBoxItem;
		}
	}
	#endregion
}
