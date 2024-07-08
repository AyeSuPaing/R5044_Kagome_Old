/*
=========================================================================================================
  Module      : 店舗受取注文詳細画面(StorePickUpOrderDetail.aspx.cs)
  ････････････････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using w2.App.Common.AmazonCv2;
using w2.App.Common.Input.Order;
using w2.App.Common.Mail;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.Order.OrderCombine;
using w2.App.Common.Order.Payment.ECPay;
using w2.App.Common.Order.Payment.Paypay;
using w2.Common.Web;
using w2.Domain;

/// <summary>
/// Store pick up order detail
/// </summary>
public partial class Form_StorePickUp_StorePickUpOrderDetail : OrderPage
{
	/// <summary>親ウィンドウをリロードしないページリスト</summary>
	static string[] m_unreloadParentPages = new string[]
	{
		Constants.PAGE_MANAGER_ORDERFILEIMPORT_LIST,
		Constants.PAGE_W2CS_MANAGER_MESSAGE_USER_SEARCH
	};

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// 処理区分チェック
			CheckActionStatus();

			// 各プロパティセット
			SetProperty();

			CheckStorePickUpOrder();

			// コンポーネント初期化
			InitializeComponents();

			// コンポーネントに値セット
			SetValueToComponents();

			// 親ウィンドウ更新有無をセット
			SetParentWindowReload();

			// 各コントロールの動的表示制御
			DisplayControl(sender, e);
		}
		else
		{
			// エラーメッセージのクリア
			trOrderStatusError.Visible = false;
			trOrderReturnExchagneStatus.Visible = false;
			trOrderRepaymentStatus.Visible = false;
			trErrorGooddealDeliveryStatus.Visible = false;
			trSendMailError.Visible = false;
		}

		this.AmazonFacade = new AmazonCv2ApiFacade();
	}

	/// <summary>
	/// 各コントロールの動的表示制御
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void DisplayControl(object sender, EventArgs e)
	{
		// Atodene後払い 出荷手配に変更時のアラート表示
		sAtodenePaymentDeadlineAlert.Visible =
			OrderCommon.IsInvoiceBundleServiceUsable(this.OrderInput.OrderPaymentKbn)
			&& (this.OrderInput.InvoiceBundleFlg == Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_ON)
			&& (rblOrderStatus.SelectedValue == Constants.FLG_ORDER_ORDER_STATUS_SHIP_ARRANGED)
			&& (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Atodene)
			&& (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF);

		// Update store pickup status display
		ucUpdateStorePickupStatusDate.Disabled = (rblStorePickupStatus.SelectedValue == Constants.FLG_STOREPICKUP_STATUS_PENDING)
			|| (this.OrderInput.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP);
		cbSendMailStorePickupStatus.Enabled = (rblStorePickupStatus.SelectedValue == Constants.FLG_STOREPICKUP_STATUS_ARRIVED)
			&& (this.OrderInput.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP)
			&& (this.OrderInput.StorePickupStatus != Constants.FLG_STOREPICKUP_STATUS_ARRIVED);
		btnUpdateStorePickupStatus.Enabled = this.OrderInput.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP;

		// 自社サイト かつ クレジット かつ キャンセル連動の場合はチェックボックスを表示する
		if (MallOptionUtility.CheckMallKbn(this.OrderInput.ShopId, this.OrderInput.MallId) == MallOptionUtility.MallKbn.OwnSite)
		{
			var isCancel = (rblOrderStatus.SelectedValue == Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED)
				|| (rblOrderStatus.SelectedValue == Constants.FLG_ORDER_ORDER_STATUS_TEMP_CANCELED);
			var isOnlinePaymentCanceled = (this.OrderInput.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_CANCELED);
			var isHasCardTranId = (string.IsNullOrEmpty(this.OrderInput.CardTranId) == false);
			var isHasPaymentOrderId = (string.IsNullOrEmpty(this.OrderInput.PaymentOrderId) == false);

			if (Constants.PAYMENT_REAUTH_ENABLED
				&& this.OrderInput.IsExchangeOrder
				&& this.OrderInput.IsPermitReauthOrderSiteKbn
				&& OrderCommon.CheckCanPaymentReauth(OrderCommon.GetLastAuthOrder(this.OrderInput.OrderId).OrderPaymentKbn))
			{
				if (this.OrderInput.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT)
				{
					// 交換注文編集の場合、「再与信」のチェックボックスを表示する。
					cbReauthCancel.Visible = isCancel;
					cbReauthCancel.Checked = cbReauthCancel.Enabled
						|| ((cbReauthCancel.Enabled == false)
							&& isOnlinePaymentCanceled);
				}
				else
				{
					// Amazon Payの場合は連動しない
					cbReauthCancel.Visible = isCancel;
					cbReauthCancel.Checked = false;
				}
			}
			else if ((this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
				&& Constants.PAYMENT_CARD_CANCEL_ENABLED)
			{
				switch (Constants.PAYMENT_CARD_KBN)
				{
					case Constants.PaymentCard.Zeus:
						cbCreditZeusCancel.Visible = isCancel;
						cbCreditZeusCancel.Checked = cbCreditZeusCancel.Enabled
							|| ((cbCreditZeusCancel.Enabled == false)
								&& isOnlinePaymentCanceled
								&& isHasCardTranId);
						break;

					case Constants.PaymentCard.SBPS:
						cbCreditSBPSCancel.Visible = isCancel;
						cbCreditSBPSCancel.Checked = cbCreditSBPSCancel.Enabled
							|| ((cbCreditSBPSCancel.Enabled == false)
								&& isOnlinePaymentCanceled
								&& isHasCardTranId);
						break;

					case Constants.PaymentCard.YamatoKwc:
						cbCreditYamatoKwcCancel.Visible = isCancel;
						cbCreditYamatoKwcCancel.Checked = cbCreditYamatoKwcCancel.Enabled
							|| ((cbCreditYamatoKwcCancel.Enabled == false)
								&& isOnlinePaymentCanceled
								&& isHasCardTranId);
						break;

					case Constants.PaymentCard.Gmo:
						cbCreditGMOCancel.Visible = isCancel;
						cbCreditGMOCancel.Checked = cbCreditGMOCancel.Enabled
							|| ((cbCreditGMOCancel.Enabled == false)
								&& isOnlinePaymentCanceled
								&& isHasCardTranId);
						break;

					case Constants.PaymentCard.Zcom:
						cbCreditZcomCancel.Visible = isCancel;
						cbCreditZcomCancel.Checked = cbCreditZcomCancel.Enabled
							|| ((cbCreditZcomCancel.Enabled == false)
								&& isOnlinePaymentCanceled
								&& isHasCardTranId);
						break;

					case Constants.PaymentCard.EScott:
						cbCreditEScottCancel.Visible = isCancel;
						cbCreditEScottCancel.Checked = cbCreditEScottCancel.Enabled
							|| ((cbCreditEScottCancel.Enabled == false)
								&& isOnlinePaymentCanceled
								&& isHasCardTranId);
						break;

					case Constants.PaymentCard.VeriTrans:
						cbVeriTransCancel.Visible = isCancel;
						cbVeriTransCancel.Checked = cbVeriTransCancel.Enabled
							|| ((cbVeriTransCancel.Enabled == false)
								&& isOnlinePaymentCanceled
								&& isHasCardTranId);
						break;

					case Constants.PaymentCard.Rakuten:
						cbCreditRakutenCancel.Visible = isCancel;
						cbCreditRakutenCancel.Checked = (cbCreditRakutenCancel.Enabled
							|| ((cbCreditRakutenCancel.Enabled == false)
								&& isOnlinePaymentCanceled
								&& isHasCardTranId));
						break;
				}
			}
			else if ((this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_SOFTBANKKETAI_SBPS)
				&& Constants.PAYMENT_SETTING_SBPS_SOFTBANKKETAI_CANCEL_ENABLED)
			{
				cbCareerSoftbankKetaiSBPSCancel.Visible = isCancel;
				cbCareerSoftbankKetaiSBPSCancel.Checked = cbCareerSoftbankKetaiSBPSCancel.Enabled
					|| ((cbCareerSoftbankKetaiSBPSCancel.Enabled == false)
						&& isOnlinePaymentCanceled
						&& isHasCardTranId);
			}
			else if ((this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_SBPS)
				&& Constants.PAYMENT_SETTING_SBPS_DOCOMOKETAI_CANCEL_ENABLED)
			{
				cbCareerDocoomKetaiSBPSCancel.Visible = isCancel;
				cbCareerDocoomKetaiSBPSCancel.Checked = cbCareerDocoomKetaiSBPSCancel.Enabled
					|| ((cbCareerDocoomKetaiSBPSCancel.Enabled == false)
						&& isOnlinePaymentCanceled
						&& isHasCardTranId);
			}
			else if ((this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AUKANTAN_SBPS)
				&& Constants.PAYMENT_SETTING_SBPS_AUKANTAN_CANCEL_ENABLED)
			{
				cbCareerAuKantanSBPSCancel.Visible = isCancel;
				cbCareerAuKantanSBPSCancel.Checked = cbCareerAuKantanSBPSCancel.Enabled
					|| ((cbCareerAuKantanSBPSCancel.Enabled == false)
						&& isOnlinePaymentCanceled
						&& isHasCardTranId);
			}
			else if ((this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_RECRUIT_SBPS)
				&& Constants.PAYMENT_SETTING_SBPS_RECRUIT_CANCEL_ENABLED)
			{
				cbRecruitSBPSCancel.Visible = isCancel;
				cbRecruitSBPSCancel.Checked = cbRecruitSBPSCancel.Enabled
					|| ((cbRecruitSBPSCancel.Enabled == false)
						&& isOnlinePaymentCanceled
						&& isHasCardTranId);
			}
			else if ((this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL_SBPS)
				&& Constants.PAYMENT_SETTING_SBPS_PAYPAL_CANCEL_ENABLED)
			{
				cbPaypalSBPSCancel.Visible = isCancel;
				cbPaypalSBPSCancel.Checked = cbPaypalSBPSCancel.Enabled
					|| ((cbPaypalSBPSCancel.Enabled == false)
						&& isOnlinePaymentCanceled
						&& isHasCardTranId);
			}
			else if ((this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_RAKUTEN_ID_SBPS)
				&& Constants.PAYMENT_SETTING_SBPS_RAKUTEN_ID_CANCEL_ENABLED)
			{
				cbRakutenIdSBPSCancel.Visible = isCancel;
				cbRakutenIdSBPSCancel.Checked = cbRakutenIdSBPSCancel.Enabled
					|| ((cbRakutenIdSBPSCancel.Enabled == false)
						&& isOnlinePaymentCanceled
						&& isHasCardTranId);
			}
			else if ((this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
				|| (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_SMS_DEF))
			{
				switch (Constants.PAYMENT_CVS_DEF_KBN)
				{
					case Constants.PaymentCvsDef.YamatoKa:
						cbYamatoKaCancel.Visible = isCancel;
						cbYamatoKaCancel.Checked = cbYamatoKaCancel.Enabled
							|| ((cbYamatoKaCancel.Enabled == false)
								&& isOnlinePaymentCanceled
								&& isHasCardTranId);
						break;

					case Constants.PaymentCvsDef.Gmo:
						cbCvsDefGmoCancel.Visible = isCancel;
						cbCvsDefGmoCancel.Checked = cbCvsDefGmoCancel.Enabled
							|| ((cbCvsDefGmoCancel.Enabled == false)
								&& isOnlinePaymentCanceled
								&& isHasCardTranId);
						break;

					case Constants.PaymentCvsDef.Atodene:
						cbCvsDefAtodeneCancel.Visible = isCancel;
						cbCvsDefAtodeneCancel.Checked = cbCvsDefAtodeneCancel.Enabled
							|| ((cbCvsDefAtodeneCancel.Enabled == false)
								&& isOnlinePaymentCanceled
								&& isHasCardTranId);
						break;

					case Constants.PaymentCvsDef.Dsk:
						cbCvsDefDskCancel.Visible = isCancel;
						cbCvsDefDskCancel.Checked = cbCvsDefDskCancel.Enabled
							|| ((cbCvsDefDskCancel.Enabled == false)
								&& isOnlinePaymentCanceled
								&& isHasCardTranId);
						break;

					case Constants.PaymentCvsDef.Atobaraicom:
						cbCvsDefAtobaraicomCancel.Visible = isCancel;
						cbCvsDefAtobaraicomCancel.Checked = cbCvsDefAtobaraicomCancel.Enabled;
						break;
				}
			}
			else if (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT)
			{
				cbAmazonPayCancel.Visible = isCancel;
				cbAmazonPayCancel.Checked = cbAmazonPayCancel.Enabled
					|| ((cbAmazonPayCancel.Enabled == false)
						&& isOnlinePaymentCanceled
						&& isHasCardTranId);
				if (this.OrderInput.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
				{
					canselAttentionMessage.Visible = isCancel;
					canselAttentionMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_REFUND_PROCESSING_AFTER_CAPTURE);
				}
			}
			else if ((this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO)
				|| (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE))
			{
				cbGmoCancel.Visible = isCancel;
				cbGmoCancel.Checked = (cbGmoCancel.Enabled
					|| ((cbGmoCancel.Enabled == false)
						&& isOnlinePaymentCanceled
						&& isHasCardTranId));
			}
			else if (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2)
			{
				cbAmazonPayCV2Cancel.Visible = isCancel;
				cbAmazonPayCV2Cancel.Checked = cbAmazonPayCV2Cancel.Enabled
					|| ((cbAmazonPayCV2Cancel.Enabled == false)
						&& isOnlinePaymentCanceled
						&& isHasCardTranId);
				if (this.OrderInput.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
				{
					canselAttentionMessage.Visible = isCancel;
					canselAttentionMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_REFUND_PROCESSING_AFTER_CAPTURE);
				}
			}
			else if (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
			{
				cbPaypalCancel.Visible = isCancel;
				cbPaypalCancel.Checked = cbPaypalCancel.Enabled
					|| ((cbPaypalCancel.Enabled == false)
						&& isOnlinePaymentCanceled
						&& isHasCardTranId);
			}
			else if (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
			{
				cbTriLinkAfterPay.Visible = isCancel;
				cbTriLinkAfterPay.Checked = (cbTriLinkAfterPay.Enabled || ((cbTriLinkAfterPay.Enabled == false) && isOnlinePaymentCanceled && isHasPaymentOrderId));
			}
			else if (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
			{
				cbPaidyPayCancel.Visible = isCancel;
				cbPaidyPayCancel.Checked = cbPaidyPayCancel.Enabled
					|| ((cbPaidyPayCancel.Enabled == false)
						&& isOnlinePaymentCanceled
						&& isHasCardTranId);
				if (this.OrderInput.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
				{
					canselAttentionMessage.Visible = isCancel;
					canselAttentionMessage.InnerHtml = "売上確定後は与信取り消しではなく返金処理となります。";
				}
			}
			else if ((this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE))
			{
				cbAtonePayCancel.Visible = isCancel;
				cbAtonePayCancel.Checked = (cbAtonePayCancel.Enabled
					|| ((cbAtonePayCancel.Enabled == false)
						&& isOnlinePaymentCanceled
						&& isHasCardTranId));
			}
			else if ((this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE))
			{
				cbAfteePayCancel.Visible = isCancel;
				cbAfteePayCancel.Checked = (cbAfteePayCancel.Enabled
					|| ((cbAfteePayCancel.Enabled == false)
						&& isOnlinePaymentCanceled
						&& isHasCardTranId));
			}
			else if (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY)
			{
				cbLinePayCancel.Visible = isCancel;
				cbLinePayCancel.Checked = cbLinePayCancel.Enabled
					|| ((cbLinePayCancel.Enabled == false)
						&& isOnlinePaymentCanceled
						&& isHasCardTranId);
			}
			else if (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
			{
				cbNPAfterPayCancel.Visible = isCancel;
				cbNPAfterPayCancel.Checked = cbNPAfterPayCancel.Enabled
					|| ((cbNPAfterPayCancel.Enabled == false)
						&& isOnlinePaymentCanceled
						&& isHasCardTranId);
			}
			else if ((this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
				&& (this.OrderInput.ExternalPaymentType == Constants.FLG_PAYMENT_TYPE_ECPAY_CREDIT))
			{
				cbEcPayCancel.Visible = isCancel;
				cbEcPayCancel.Checked = cbEcPayCancel.Enabled
					|| ((cbEcPayCancel.Enabled == false)
						&& isOnlinePaymentCanceled
						&& isHasCardTranId);
			}
			else if ((this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
				&& (this.OrderInput.ExternalPaymentType == Constants.FLG_PAYMENT_TYPE_NEWEBPAY_CREDIT))
			{
				cbNewebPayCancel.Visible = isCancel;
				cbNewebPayCancel.Checked = cbNewebPayCancel.Enabled
					|| ((cbNewebPayCancel.Enabled == false)
						&& isOnlinePaymentCanceled
						&& isHasCardTranId);
			}
			else if (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
			{
				cbPayPayCancel.Visible = isCancel;
				cbPayPayCancel.Checked = cbPayPayCancel.Enabled
					|| ((cbPayPayCancel.Enabled == false)
						&& isOnlinePaymentCanceled
						&& isHasCardTranId);
			}
			else if (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU)
			{
				cbBokuCancel.Visible = isCancel;
				cbBokuCancel.Checked = cbBokuCancel.Enabled
					|| ((cbBokuCancel.Enabled == false)
						&& isOnlinePaymentCanceled
						&& isHasCardTranId);
			}

			if (Constants.TWSHIPPING_GOODDEAL_OPTION_ENABLED)
			{
				var isRegisterShippingDelivery =
					(this.OrderInput.LogiCooperationStatus == Constants.FLG_ORDER_LOGI_COOPERATION_STATUS_COMPLETE);
				cbGooddealShippingCancel.Visible = isCancel;
				cbGooddealShippingCancel.Checked = cbGooddealShippingCancel.Enabled
					|| ((cbGooddealShippingCancel.Enabled == false)
						&& isRegisterShippingDelivery);
			}

			if (Constants.TWINVOICE_ENABLED == false) return;
			var twOrderInvoice = DomainFacade.Instance.TwOrderInvoiceService.GetOrderInvoice(
				this.OrderInput.OrderId,
				int.Parse(this.OrderInput.Shippings[0].OrderShippingNo));

			rbTwInvoiceCancel.Visible = rbTwInvoiceRefund.Visible
				= isCancel
					&& OrderCommon.DisplayTwInvoiceInfo()
					&& (OrderCommon.TwInvoiceStatusCanNotEditOrder(
						twOrderInvoice,
						Constants.TWINVOICE_ECPAY_ENABLED) == false);
			if (rbTwInvoiceCancel.Visible) rbTwInvoiceCancel.Checked = true;
		}
	}

	/// <summary>
	/// 一覧へ戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBackList_Click(object sender, EventArgs e)
	{
		var urlBackList = StorePickUpOrderList(
			(Hashtable)Session[Constants.SESSIONPARAM_KEY_ORDER_SEARCH_INFO],
			true);

		Response.Redirect(urlBackList.CreateUrl());
	}

	/// <summary>
	/// Can display get external order payment status
	/// </summary>
	/// <param name="orderPaymentKbn">Order payment kbn</param>
	/// <returns>True: Show diplay for get external order payment status</returns>
	protected bool CanDisplayGetExternalOrderPaymentStatus(string orderPaymentKbn)
	{
		var result = (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
			&& (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Gmo)
			&& Constants.PAYMENT_SETTING_GMO_GETDEFPAYMENTSTATUS_OPTION;

		return result;
	}

	#region メソッド
	/// <summary>
	/// 処理区分チェック
	/// </summary>
	private void CheckActionStatus()
	{
		// 詳細でなければエラーページへ
		if (this.ActionStatus != Constants.ACTION_STATUS_DETAIL)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
	}

	/// <summary>
	/// Check store pick up order
	/// </summary>
	private void CheckStorePickUpOrder()
	{
		if (string.IsNullOrEmpty(this.OrderInput.FixedPurchaseId)) return;

		var operatorAuthoritys = DomainFacade.Instance.OperatorAuthorityService.Get(
			this.LoginOperatorShopId,
			this.LoginOperatorId);

		if (operatorAuthoritys == null) return;

		var realShopIds = this.OrderInput.Shippings.Select(ship => ship.StorePickupRealShopId).ToList();
		if (operatorAuthoritys.Any(item => realShopIds.Contains(item.ConditionValue))) return;

		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_STOREPICKUP_ORDER_LIST);
	}

	/// <summary>
	/// 各プロパティセット
	/// </summary>
	private void SetProperty()
	{
		// 注文情報セット
		SetOrder();

		// 関連注文情報セット
		SetRelatedOrder();
	}

	/// <summary>
	/// 注文情報セット
	/// </summary>
	private void SetOrder()
	{
		// 注文情報取得
		var order = DomainFacade.Instance.OrderService.GetOrderInfoByOrderId(this.RequestOrderId);
		// 注文情報が存在しない？
		if (order == null)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		if (Constants.PAYMENT_GMO_POST_ENABLED)
		{
			// GMO payment: get credit status
			var isGetGmoStatus = OrderCommon.IsGetGmoCreditStatus(order);
			if (isGetGmoStatus)
			{
				var isChangeStatus = OrderCommon.CheckGmoCredit(order);
				if (isChangeStatus)
				{
					// reload order after change status
					order = DomainFacade.Instance.OrderService.GetOrderInfoByOrderId(this.RequestOrderId);
				}
			}
		}

		// 注文情報セット
		this.OrderInput = new OrderInput(order);
	}

	/// <summary>
	/// 関連注文情報セット
	/// </summary>
	private void SetRelatedOrder()
	{
		var orderIdOrg = string.IsNullOrEmpty(this.OrderInput.OrderIdOrg)
			? this.OrderInput.OrderId
			: this.OrderInput.OrderIdOrg;

		var orders = DomainFacade.Instance.OrderService.GetRelatedOrders(orderIdOrg);
		rRelatedOrder.DataSource = orders.Select(o => new OrderInput(o)).ToArray();
		rRelatedOrder.DataBind();
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// 各ラジオボタン・ドロップダウン作成
		// 「注文ステータス」作成
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_STATUS))
		{
			if (li.Value == Constants.FLG_ORDER_ORDER_STATUS_UNKNOWN) continue;

			// 実在庫を利用しない場合、「在庫引当済み」は追加しない
			if ((Constants.REALSTOCK_OPTION_ENABLED == false)
				&& (li.Value == Constants.FLG_ORDER_ORDER_STATUS_STOCK_RESERVED)) continue;

			li.Selected = (this.OrderInput.OrderStatus == li.Value);
			rblOrderStatus.Items.Add(li);
		}

		hfOrderStatus.Value = this.OrderInput.OrderStatus;

		// 「返品交換ステータス更新」作成
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_STATUS))
		{
			if (li.Value == Constants.FLG_ORDER_ORDER_RETURN_EXCHANGE_STATUS_UNKNOWN) continue;

			li.Selected = (li.Value == this.OrderInput.OrderReturnExchangeStatus);
			rblOrderReturnExchangeStatus.Items.Add(li);
		}

		// 「返金ステータス更新」作成
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_REPAYMENT_STATUS))
		{
			if (li.Value == Constants.FLG_ORDER_ORDER_REPAYMENT_STATUS_UNKNOWN) continue;

			li.Selected = (li.Value == this.OrderInput.OrderRepaymentStatus);
			rblOrderRepaymentStatus.Items.Add(li);
		}

		// 「モール連携ステータス更新」作成
		rblMallLinkStatus.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ORDER, Constants.FIELD_ORDER_MALL_LINK_STATUS));

		// Gooddeal delivery status
		rblGooddealDeliveryStatus.Items.AddRange(
			ValueText.GetValueItemArray(
				Constants.TABLE_ORDER,
				Constants.FIELD_ORDER_LOGI_COOPERATION_STATUS));

		var canUpdateGooddealDeliveryStatus = (this.OrderInput.LogiCooperationStatus != Constants.FLG_ORDER_LOGI_COOPERATION_STATUS_COMPLETE);
		rblGooddealDeliveryStatus.Enabled = canUpdateGooddealDeliveryStatus;
		btnUpdateGooddealDeliveryStatus.Enabled = canUpdateGooddealDeliveryStatus;

		// 「メールテンプレート」セット
		ddlOrderMailId.Items.Add(new ListItem(string.Empty, string.Empty));
		ddlOrderMailId.Items.AddRange(
			GetMailTemplateUtility.GetMailTemplateForStorePickUpOrder(this.LoginOperatorShopId)
				.Select(mail => new ListItem(mail.MailName, mail.MailId)).ToArray());

		if (OrderCommon.DisplayTwInvoiceInfo())
		{
			// Get TwOrderInvoice
			var twOrderInvoice = DomainFacade.Instance.TwOrderInvoiceService.GetOrderInvoice(
				this.OrderInput.OrderId,
				int.Parse(this.OrderInput.Shippings[0].OrderShippingNo));

			if (twOrderInvoice != null)
			{
				// Set Taiwan Order Invoice
				this.TwOrderInvoiceInput = new TwOrderInvoiceInput(twOrderInvoice);

				this.TwOrderInvoice = twOrderInvoice;
			}
		}

		// 店舗受取ステータス
		rblStorePickupStatus.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ORDER, "storepickup_status"));
		rblStorePickupStatus.SelectedValue = this.OrderInput.StorePickupStatus;

		if (((this.OrderInput.Shippings.Any(shipping =>
			(shipping.ShippingReceivingStoreType == Constants.FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_HI_LIFE)
				|| (shipping.ShippingReceivingStoreType == Constants.FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_HI_LIFE_PAYMENT)))
				&& this.OrderInput.IsReturnOrder)
			|| (this.OrderInput.OnlineDeliveryStatus == Constants.FLG_ORDER_ONLINE_DELIVERY_STATUS_SETTLED))
		{
			btnExternalOrderInfoAction.Enabled = false;
		}

		if ((Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED == false)
			|| this.OrderInput.Shippings.Any(shipping =>
				(shipping.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF)
					&& (ECPayUtility.IsShippingServiceOfECPay(shipping.DeliveryCompanyId) == false))
			|| ((this.OrderInput.ReturnExchangeKbn == Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE)
				&& (this.OrderInput.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_UNKNOWN)))
		{
			divOrderDelivertyStatus.Visible = false;
		}

		// ポイントオプションあり＆仮注文？のとき、二重で初回購入ポイントが割り当てられている場合はアラート文言表示
		if (Constants.W2MP_POINT_OPTION_ENABLED
			&& (this.OrderInput.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_TEMP))
		{
			if (CheckHasAnotherFistBuyPoint(
				this.OrderInput.UserId,
				this.OrderInput.OrderId))
			{
				trFirstPointAlert.Visible = true;
			}
		}

		// 元注文の場合の制御
		if (this.OrderInput.ReturnExchangeKbn == Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_UNKNOWN)
		{
			// 注文、実在庫、入金、督促ステータス表示
			divOrderStatus.Visible = true;
			// 返品交換、返金ステータス非表示
			divOrderReturnExchangeStatus.Visible = false;
			divOrderRepaymentStatus.Visible = false;
		}
		// 交換注文かつ注文ステータスが指定無しの場合の制御
		// ※交換注文情報の場合、出荷処理を行う必要があるため、元注文情報と同様に表示する必要有り。
		else if ((this.OrderInput.ReturnExchangeKbn == Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE)
			&& (this.OrderInput.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_UNKNOWN))
		{
			// 各領域の表示非表示制御
			divOrderStatus.Visible = true;
			// 返品交換、返金ステータス非表示
			divOrderReturnExchangeStatus.Visible = false;
			divOrderRepaymentStatus.Visible = false;
		}
		// 返品交換注文の場合
		else if ((this.OrderInput.ReturnExchangeKbn == Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN)
			|| (this.OrderInput.ReturnExchangeKbn == Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE))
		{
			// 注文、実在庫、入金、督促ステータス非表示
			divOrderStatus.Visible = false;
			// 返品交換、返金ステータス非表示
			divOrderReturnExchangeStatus.Visible = true;
			divOrderRepaymentStatus.Visible = true;
		}

		// ポップアップ受注詳細のときは一覧へ戻るを非表示へ
		if (this.IsPopUp)
		{
			btnBackListTop.Visible = btnBackListBottom.Visible = false;
		}

		// カード分割支払い
		trInstallments.Visible = OrderCommon.CreditInstallmentsSelectable;
		if (OrderCommon.CreditInstallmentsSelectable)
		{
			dllCreditInstallments.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ORDER, OrderCommon.CreditInstallmentsValueTextFieldName));
		}

		if (string.IsNullOrEmpty(this.ActionCompleteMessage) == false)
		{
			lActionCompleteMessage.Text = this.ActionCompleteMessage;
		}

		if ((this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
			&& (Constants.PAYMENT_PAYPAY_KBN == Constants.PaymentPayPayKbn.GMO)
			&& (this.OrderInput.OnlinePaymentStatus != Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE)
			&& (this.OrderInput.OnlinePaymentStatus != Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_CANCELED))
		{
			var order = OrderCommon.GetLastAuthOrder(this.OrderInput.OrderId);
			var paypayFacade = new PaypayGmoFacade();
			var getTrans = paypayFacade.GetTransaction(order);
		}
	}

	/// <summary>
	/// コンポーネントに値セット
	/// </summary>
	private void SetValueToComponents()
	{
		// 更新日プルダウン表示設定（プルダウンに本日の日付を表示）
		ucOrderReturnExchangeStatusDate.SetDate(DateTime.Now);
		ucOrderRepaymentStatusDate.SetDate(DateTime.Now);

		// 注文ステータスの更新日を表示
		var updateOrderStatusDate = DateTime.Now;
		// 更新日プルダウン表示設定（ＤＢに更新日が入っている場合プルダウンに更新日を表示）
		// 注文日が空でない、かつ、注文ステータスが受注承認の場合ＤＢから更新日を取得して表示
		if ((string.IsNullOrEmpty(this.OrderInput.OrderDate) == false)
			&& (this.OrderInput.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_TEMP))
		{
			updateOrderStatusDate = DateTime.Parse(this.OrderInput.OrderDate);
		}

		// 受注承認日が空でない、かつ、注文ステータスが受注承認の場合ＤＢから更新日を取得して表示
		if ((string.IsNullOrEmpty(this.OrderInput.OrderRecognitionDate) == false)
			&& (this.OrderInput.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_ORDER_RECOGNIZED))
		{
			updateOrderStatusDate = DateTime.Parse(this.OrderInput.OrderRecognitionDate);
		}

		// 在庫引当日が空でない、かつ、注文ステータスが在庫引き当て済みの場合ＤＢから更新日を取得して表示
		if ((string.IsNullOrEmpty(this.OrderInput.OrderStockreservedDate) == false)
			&& (this.OrderInput.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_STOCK_RESERVED))
		{
			updateOrderStatusDate = DateTime.Parse(this.OrderInput.OrderStockreservedDate);
		}

		// 出荷手配済み日が空でない、かつ、注文ステータスが出荷手配済みの場合ＤＢから更新日を取得して表示
		if ((string.IsNullOrEmpty(this.OrderInput.OrderShippingDate) == false)
			&& (this.OrderInput.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_SHIP_ARRANGED))
		{
			updateOrderStatusDate = DateTime.Parse(this.OrderInput.OrderShippingDate);
		}

		// 出荷完了日が空でない、かつ、注文ステータスが出荷完了の場合ＤＢから更新日を取得して表示
		if ((string.IsNullOrEmpty(this.OrderInput.OrderShippedDate) == false)
			&& (this.OrderInput.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP))
		{
			updateOrderStatusDate = DateTime.Parse(this.OrderInput.OrderShippedDate);
		}

		// 配送完了日が空でない、かつ、注文ステータスが配送完了の場合ＤＢから更新日を取得して表示
		if ((string.IsNullOrEmpty(this.OrderInput.OrderDeliveringDate) == false)
			&& (this.OrderInput.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_DELIVERY_COMP))
		{
			updateOrderStatusDate = DateTime.Parse(this.OrderInput.OrderDeliveringDate);
		}

		// If the cancel completion date is not empty and the order status is cancel completion, the update date is acquired from the DB and displayed.
		if ((string.IsNullOrEmpty(this.OrderInput.OrderCancelDate) == false)
			&& ((this.OrderInput.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED)
				|| (this.OrderInput.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_TEMP_CANCELED)))
		{
			updateOrderStatusDate = DateTime.Parse(this.OrderInput.OrderCancelDate);
		}

		ucUpdateOrderStatusDate.SetStartDate(updateOrderStatusDate);
		ucUpdateOrderStatusDate.Disabled = true;
		// 入金ステータスの更新日表示
		var updateOrderPaymentStatusDate = DateTime.Now;
		// 入金日が空でない、かつ、入金ステータスが入金済み、一部入金の場合ＤＢから更新日を取得して表示
		if ((string.IsNullOrEmpty(this.OrderInput.OrderPaymentDate) == false)
			&& ((this.OrderInput.OrderPaymentStatus == Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE)
			|| (this.OrderInput.OrderPaymentStatus == Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_SHORTAGE)))
		{
			updateOrderPaymentStatusDate = DateTime.Parse(this.OrderInput.OrderPaymentDate);
		}

		// 督促ステータスの更新日表示
		var updateDemandStatusDate = DateTime.Now;
		// 督促日が空でない、かつ、督促ステータスが督促レベル１か２の場合ＤＢから更新日を取得して表示
		if ((string.IsNullOrEmpty(this.OrderInput.DemandDate) == false)
			&& ((this.OrderInput.DemandStatus == Constants.FLG_ORDER_DEMAND_STATUS_LEVEL1)
			|| (this.OrderInput.DemandStatus == Constants.FLG_ORDER_DEMAND_STATUS_LEVEL2)))
		{
			updateDemandStatusDate = DateTime.Parse(this.OrderInput.DemandDate);
		}

		// モール連携ステータス設定
		rblMallLinkStatus.SelectedValue = this.OrderInput.MallLinkStatus;
		rblMallLinkStatus.Enabled = btnUpdateMallLinkStatus.Enabled =
			((this.OrderInput.MallLinkStatus != Constants.FLG_ORDER_MALL_LINK_STATUS_SHIPNOTE_PROGRESS)
				&& (this.OrderInput.MallLinkStatus != Constants.FLG_ORDER_MALL_LINK_STATUS_SHIPNOTE_COMP)
				&& (this.OrderInput.MallLinkStatus != Constants.FLG_ORDER_MALL_LINK_STATUS_CANCEL));

		// Gooddeal delivery status
		rblGooddealDeliveryStatus.SelectedValue = this.OrderInput.LogiCooperationStatus;

		// 各コントロールの表示制御
		// キャンセル・仮注文キャンセルの場合は他ステータスを選択、各実行コントロールを使用不可にする
		if ((rblOrderStatus.SelectedValue == Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED)
			|| (rblOrderStatus.SelectedValue == Constants.FLG_ORDER_ORDER_STATUS_TEMP_CANCELED))
		{
			foreach (ListItem li in rblOrderStatus.Items)
			{
				li.Enabled = (li.Selected);
			}
		}
		// 仮注文の場合は仮注文、注文済み、仮注文キャンセルのみ使用可能とする
		else if (rblOrderStatus.SelectedValue == Constants.FLG_ORDER_ORDER_STATUS_TEMP)
		{
			foreach (ListItem li in rblOrderStatus.Items)
			{
				li.Enabled = ((li.Value == Constants.FLG_ORDER_ORDER_STATUS_TEMP)
					|| (li.Value == Constants.FLG_ORDER_ORDER_STATUS_ORDERED)
					|| (li.Value == Constants.FLG_ORDER_ORDER_STATUS_TEMP_CANCELED));
			}
		}
		// 出荷完了の場合は注文済み、受注承認、在庫引当済み、出荷手配済みを選択不可とする
		else if (rblOrderStatus.SelectedValue == Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP)
		{
			foreach (ListItem li in rblOrderStatus.Items)
			{
				if ((li.Value == Constants.FLG_ORDER_ORDER_STATUS_ORDERED)
					|| (li.Value == Constants.FLG_ORDER_ORDER_STATUS_ORDER_RECOGNIZED)
					|| (li.Value == Constants.FLG_ORDER_ORDER_STATUS_STOCK_RESERVED)
					|| (li.Value == Constants.FLG_ORDER_ORDER_STATUS_SHIP_ARRANGED))
				{
					li.Enabled = false;
				}
			}
		}
		// 配送完了の場合は注文済み、受注承認、在庫引当済み、出荷手配済み、出荷完了選択不可とする
		else if (rblOrderStatus.SelectedValue == Constants.FLG_ORDER_ORDER_STATUS_DELIVERY_COMP)
		{
			foreach (ListItem li in rblOrderStatus.Items)
			{
				if ((li.Value == Constants.FLG_ORDER_ORDER_STATUS_ORDERED)
					|| (li.Value == Constants.FLG_ORDER_ORDER_STATUS_ORDER_RECOGNIZED)
					|| (li.Value == Constants.FLG_ORDER_ORDER_STATUS_STOCK_RESERVED)
					|| (li.Value == Constants.FLG_ORDER_ORDER_STATUS_SHIP_ARRANGED)
					|| (li.Value == Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP))
				{
					li.Enabled = false;
				}
			}

			ucUpdateOrderStatusDate.Disabled = true;
			btnUpdateOrderStatus.Enabled = false;
		}

		// 出荷完了、配送完了の場合はキャンセル選択不可
		if ((rblOrderStatus.SelectedValue == Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP)
			|| (rblOrderStatus.SelectedValue == Constants.FLG_ORDER_ORDER_STATUS_DELIVERY_COMP))
		{
			foreach (ListItem li in rblOrderStatus.Items)
			{
				if (li.Value == Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED)
				{
					li.Enabled = false;
				}
			}
		}

		// 仮注文以外の場合は仮注文、仮注文キャンセル選択不可
		if (rblOrderStatus.SelectedValue != Constants.FLG_ORDER_ORDER_STATUS_TEMP)
		{
			foreach (ListItem li in rblOrderStatus.Items)
			{
				if ((li.Value == Constants.FLG_ORDER_ORDER_STATUS_TEMP)
					|| (li.Value == Constants.FLG_ORDER_ORDER_STATUS_TEMP_CANCELED))
				{
					li.Enabled = false;
				}
			}
		}

		var isNotAuthCompOrSaleComp = (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
			&& (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Atobaraicom)
			&& (this.OrderInput.ExternalPaymentStatus != Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP)
			&& (this.OrderInput.ExternalPaymentStatus != Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP)
			&& (this.OrderInput.ExternalPaymentStatus != Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SHIP_COMP);

		if (isNotAuthCompOrSaleComp)
		{
			foreach (ListItem listItem in rblOrderStatus.Items)
			{
				if ((listItem.Value == Constants.FLG_ORDER_ORDER_STATUS_SHIP_ARRANGED)
					|| (listItem.Value == Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP)
					|| (listItem.Value == Constants.FLG_ORDER_ORDER_STATUS_DELIVERY_COMP))
				{
					listItem.Enabled = false;
				}
			}
		}

		// 返品時再与信エラーメッセージ表示
		divReturnOrderReauthErrormessages.Visible = (this.ReturnOrderReauthErrorMessages.Length != 0);
		lReturnOrderReauthErrormessages.Text = StringUtility.ChangeToBrTag(WebSanitizer.HtmlEncode(this.ReturnOrderReauthErrorMessages));
		this.ReturnOrderReauthErrorMessages = string.Empty;

		rCombinedOrders.DataSource = OrderCombineUtility.GetCombinedOrders(this.OrderInput.CombinedOrgOrderIds);
		rCombinedOrders.DataBind();

		if (this.OrderInput.CanRequestCvsDefInvoiceReissue)
		{
			divCvsDefInvoiceReissue.Visible = true;
			lActionCompleteMessage.Text = this.ActionCompleteMessage;
			this.ActionCompleteMessage = string.Empty;
		}

		// EC管理画面での新規注文登録時の注文完了通知メール自動送信エラー
		if (string.IsNullOrEmpty(this.SendMailErrorMessage) == false)
		{
			trSendMailError.Visible = true;
			lSendMeilErrorMessage.Text = this.SendMailErrorMessage;
			this.SendMailErrorMessage = string.Empty;
			// メール送信のドロップダウンに自動で注文完了通知メール【管理者用】をセットする
			var targetMailListItem = ddlOrderMailId.Items.FindByValue(Constants.CONST_MAIL_ID_ORDER_COMPLETE_FOR_OPERATOR);
			var targetMailIndex = ddlOrderMailId.Items.IndexOf(targetMailListItem);
			ddlOrderMailId.SelectedIndex = targetMailIndex;
		}

		// 仮クレジット向けフォーム表示
		DisplayForProvisionalCreditCard();

		// Store pickup status display
		var updateStorePickupStatusDate = DateTime.Now;
		if ((string.IsNullOrEmpty(this.OrderInput.StorePickupStoreArrivedDate) == false)
			&& (this.OrderInput.StorePickupStatus == Constants.FLG_STOREPICKUP_STATUS_ARRIVED))
		{
			updateStorePickupStatusDate = DateTime.Parse(this.OrderInput.StorePickupStoreArrivedDate);
		}

		if ((string.IsNullOrEmpty(this.OrderInput.StorePickupDeliveredCompleteDate) == false)
			&& (this.OrderInput.StorePickupStatus == Constants.FLG_STOREPICKUP_STATUS_DELIVERED))
		{
			updateStorePickupStatusDate = DateTime.Parse(this.OrderInput.StorePickupDeliveredCompleteDate);
		}

		if ((string.IsNullOrEmpty(this.OrderInput.StorePickupReturnDate) == false)
			&& (this.OrderInput.StorePickupStatus == Constants.FLG_STOREPICKUP_STATUS_RETURNED))
		{
			updateStorePickupStatusDate = DateTime.Parse(this.OrderInput.StorePickupReturnDate);
		}
		ucUpdateStorePickupStatusDate.SetStartDate(updateStorePickupStatusDate);

		if (rblOrderStatus.SelectedValue != Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP)
		{
			foreach (ListItem li in rblStorePickupStatus.Items)
			{
				li.Enabled = false;
			}
			cbSendMailStorePickupStatus.Enabled = false;
			ucUpdateStorePickupStatusDate.Disabled = true;
		}
		else
		{
			foreach (ListItem li in rblStorePickupStatus.Items)
			{
				switch (this.OrderInput.StorePickupStatus)
				{
					case Constants.FLG_STOREPICKUP_STATUS_PENDING:
						li.Enabled = li.Value != Constants.FLG_STOREPICKUP_STATUS_RETURNED;
						break;

					case Constants.FLG_STOREPICKUP_STATUS_ARRIVED:
						li.Enabled = li.Value != Constants.FLG_STOREPICKUP_STATUS_PENDING;
						break;

					case Constants.FLG_STOREPICKUP_STATUS_DELIVERED:
						li.Enabled = li.Value == Constants.FLG_STOREPICKUP_STATUS_DELIVERED;
						break;

					case Constants.FLG_STOREPICKUP_STATUS_RETURNED:
						li.Enabled = true;
						break;
				}
			}
			ucUpdateStorePickupStatusDate.Disabled = this.OrderInput.StorePickupStatus != Constants.FLG_STOREPICKUP_STATUS_PENDING;
		}

		dvStorePickupStatus.Visible = OrderCommon.IsStorePickupOrder(this.OrderInput.Shippings[0].StorePickupRealShopId);
	}

	/// <summary>
	/// 親ウィンドウ更新有無をセット
	/// </summary>
	private void SetParentWindowReload()
	{
		// 呼び出され方で親ウインドウの更新を決める
		if ((this.IsPopUp)
			&& (Request[Constants.REQUEST_KEY_MANAGER_POPUP_PARENT_NAME] != null)
			&& m_unreloadParentPages.Any(p =>
				(p == Request[Constants.REQUEST_KEY_MANAGER_POPUP_PARENT_NAME])
					|| (p == ((string)Session[Constants.SESSIONPARAM_KEY_ORDERDETAIL_POPUP_PARENT_NAME]))))
		{
			// このウインドウがポップアップで、呼び出し元が注文関連ファイル取込画面の場合は親ウインドウを更新しない
			Session[Constants.SESSIONPARAM_KEY_ORDERDETAIL_POPUP_PARENT_NAME] = Constants.PAGE_MANAGER_ORDERFILEIMPORT_LIST;
			Session[Constants.SESSIONPARAM_KEY_ORDERDETAIL_RELOAD_PARENT_WINDOW] = Constants.KBN_RELOAD_PARENT_WINDOW_OFF;
		}
		else
		{
			// このウインドウがポップアップでない、または呼び出し元が特に指定されていないポップアップの場合は親ウインドウを更新する
			Session[Constants.SESSIONPARAM_KEY_ORDERDETAIL_POPUP_PARENT_NAME] = string.Empty;
			Session[Constants.SESSIONPARAM_KEY_ORDERDETAIL_RELOAD_PARENT_WINDOW] = Constants.KBN_RELOAD_PARENT_WINDOW_ON;
		}
	}

	/// <summary>
	/// 注文情報詳細URL作成
	/// </summary>
	/// <param name="orderId">注文ID</param>
	/// <returns>注文情報詳細URL</returns>
	protected string CreateOrderDetailUrl(string orderId)
	{
		// 注文IDのセッション退避
		SetSessionOrderId(orderId);

		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_STOREPICKUP_ORDER_DETAIL)
			.AddParam(Constants.REQUEST_KEY_ORDER_ID, HttpUtility.UrlEncode(orderId))
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, HttpUtility.UrlEncode(Constants.ACTION_STATUS_DETAIL));

		if (this.IsPopUp)
		{
			url.AddParam(Constants.REQUEST_KEY_WINDOW_KBN, HttpUtility.UrlEncode(Constants.KBN_WINDOW_POPUP));
		}

		if ((string)Session[Constants.SESSIONPARAM_KEY_ORDERDETAIL_RELOAD_PARENT_WINDOW] != Constants.KBN_RELOAD_PARENT_WINDOW_OFF)
		{
			url.AddParam(Constants.REQUEST_KEY_RELOAD_PARENT_WINDOW, HttpUtility.UrlEncode(Constants.KBN_RELOAD_PARENT_WINDOW_ON));
		}

		if (string.IsNullOrEmpty(Constants.PAGE_MANAGER_ORDER_CONFIRM) == false)
		{
			url.AddParam(Constants.REQUEST_KEY_MANAGER_POPUP_PARENT_NAME, HttpUtility.UrlEncode(Constants.PAGE_MANAGER_ORDER_CONFIRM));
		}

		return url.CreateUrl();
	}

	/// <summary>
	/// 更新対象のユーザーに二重に初回購入ポイントが発行されていないかチェック
	/// </summary>
	/// <param name="userId">ユーザーID</param>
	/// <param name="orderId">注文ID</param>
	/// <returns>True if the user to be renewed has not been issued double initial purchase points, otherwise false</returns>
	private bool CheckHasAnotherFistBuyPoint(string userId, string orderId)
	{
		var sv = DomainFacade.Instance.PointService;

		// 初回購入履歴
		var history = sv.GetUserPointHistories(userId)
			.Where(x => x.PointKbn == Constants.FLG_USERPOINT_POINT_KBN_BASE
				&& x.PointIncKbn == Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_FIRST_BUY)
			.ToArray();

		var blCurrent = false;
		var blAnother = false;
		foreach (var model in history)
		{
			if (model.Kbn1 == orderId)
			{
				blCurrent = true;
			}
			else
			{
				blAnother = true;
			}
		}

		return blCurrent && blAnother;
	}

	/// <summary>
	/// 外部決済本売り上げ可能かを取得する
	/// </summary>
	/// <param name="paymentId">決済ID</param>
	/// <returns>外部決済売り上げ可能か</returns>
	protected bool GetCanExternalPaymentRealSales(string paymentId)
	{
		// 即時決済かの判断
		var paymentStatusCompleteFlg = false;
		switch (paymentId)
		{
			case Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT:
			case Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2:
				paymentStatusCompleteFlg = Constants.PAYMENT_AMAZON_PAYMENTCAPTURENOW;
				break;

			case Constants.FLG_PAYMENT_PAYMENT_ID_RAKUTEN_ID_SBPS:
			case Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_SBPS:
			case Constants.FLG_PAYMENT_PAYMENT_ID_SOFTBANKKETAI_SBPS:
			case Constants.FLG_PAYMENT_PAYMENT_ID_RECRUIT_SBPS:
			case Constants.FLG_PAYMENT_PAYMENT_ID_AUKANTAN_SBPS:
			case Constants.FLG_PAYMENT_PAYMENT_ID_SMATOMETE_SBPS:
				paymentStatusCompleteFlg = Constants.PAYMENT_SETTING_SBPS_PAYMENT_STATUS_COMPLETE;
				break;

			default:
				paymentStatusCompleteFlg = (this.OrderInput.IsDigitalContents && Constants.DIGITAL_CONTENTS_OPTION_ENABLED)
					? Constants.PAYMENT_CARD_PATMENT_STAUS_COMPLETE_FORDIGITALCONTENTS
					: Constants.PAYMENT_CARD_PATMENT_STAUS_COMPLETE;
				break;
		}

		// 即時決済の場合本売上済みなので、売上確定不要
		var canExternalPaymentRealSales = (paymentStatusCompleteFlg == false)
			&& ((GetCanExternalPaymentCreditRealSales(paymentId)
					|| ((paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_ORG)
						&& Constants.PAYMENT_SETTING_DOCOMOKETAI_REALSALES_ENABLED)
					|| ((paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_SMATOMETE_ORG)
						&& Constants.PAYMENT_SETTING_SMATOMETE_REALSALES_ENABLED)
					|| ((paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_SOFTBANKKETAI_SBPS)
						&& Constants.PAYMENT_SETTING_SBPS_SOFTBANKKETAI_REALSALES_ENABLED)
					|| ((paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_SBPS)
						&& Constants.PAYMENT_SETTING_SBPS_DOCOMOKETAI_REALSALES_ENABLED)
					|| ((paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AUKANTAN_SBPS)
						&& Constants.PAYMENT_SETTING_SBPS_AUKANTAN_REALSALES_ENABLED)
					|| ((paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_RECRUIT_SBPS)
						&& Constants.PAYMENT_SETTING_SBPS_RECRUIT_REALSALES_ENABLED)
					|| ((paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_RAKUTEN_ID_SBPS)
						&& Constants.PAYMENT_SETTING_SBPS_RAKUTEN_ID_REALSALES_ENABLED))
				|| (OrderCommon.IsAmazonPayment(paymentId)
					&& (Constants.PAYMENT_AMAZON_PAYMENTCAPTURENOW == false))
				|| (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
				|| (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
				|| (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)
				|| (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE)
				|| (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY)
				|| ((paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
					&& (this.OrderInput.ExternalPaymentType == Constants.FLG_PAYMENT_TYPE_ECPAY_CREDIT))
				|| ((paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
					&& (this.OrderInput.ExternalPaymentType == Constants.FLG_PAYMENT_TYPE_NEWEBPAY_CREDIT))
				|| (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
				|| (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU)
				|| (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
				|| (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO)
				|| (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE));

		return canExternalPaymentRealSales;
	}

	/// <summary>
	/// クレジット外部決済・本売り上げ可能かを取得する
	/// </summary>
	/// <param name="paymnetId">決済ID</param>
	/// <returns>外部決済可能か</returns>
	protected bool GetCanExternalPaymentCreditRealSales(string paymnetId)
	{
		return (paymnetId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
			&& ((Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Zeus)
				|| (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.SBPS)
				|| (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Gmo)
				|| (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Zcom)
				|| (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.EScott)
				|| (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans)
				|| (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten))
			&& Constants.PAYMENT_CARD_REALSALES_ENABLED;
	}

	/// <summary>
	/// 仮クレジット向けフォーム表示
	/// </summary>
	private void DisplayForProvisionalCreditCard()
	{
		var isProvisionalCreditCardPaymentId = this.OrderInput.OrderPaymentKbn == Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID;
		// ZEUS仮クレジット向け
		if (OrderCommon.IsPaymentCardTypeZeus)
		{
			var isOrderStatusTemp = this.OrderInput.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_TEMP;
			var isProvisionalCreditCardOrderZeus = isOrderStatusTemp && isProvisionalCreditCardPaymentId;

			tbdyCreditCardInputButtonZeus.Visible = (isProvisionalCreditCardOrderZeus
				&& SessionManager.UsePaymentTabletZeus);
			tbdyCreditCardInputUnvisibleMessageZeus.Visible = isProvisionalCreditCardOrderZeus
				&& (SessionManager.UsePaymentTabletZeus == false);
		}
		// ZEUS以外の仮クレジット向け
		else if (OrderCommon.NeedsRegisterProvisionalCreditCardCardKbnExceptZeus)
		{
			tbdyCreditCardRegisterForUnregisterd.Visible
				= tbdyCreditCardRegisternForUnauthed.Visible
				= divCreditCardAuthErrorMessage.Visible
				= btnRegisterUnregisterdCreditCardForAuthError.Visible = false;

			var user = DomainFacade.Instance.UserService.Get(this.OrderInput.UserId);
			trRegistCreditCard.Visible
				= (user.IsMember && (Constants.MAX_NUM_REGIST_CREDITCARD > user.UserCreditCards.Length));

			trCreditCardName.Visible = cbRegistCreditCard.Checked;
			if (isProvisionalCreditCardPaymentId)
			{
				var userCreditCard = UserCreditCard.Get(this.OrderInput.UserId, int.Parse(this.OrderInput.CreditBranchNo));
				tbdyCreditCardRegisterForUnregisterd.Visible = userCreditCard.IsRegisterdStatusUnregisterd;
				tbdyCreditCardRegisternForUnauthed.Visible = (userCreditCard.IsRegisterdStatusUnauthed || userCreditCard.IsRegisterdStatusdAuthError);
				btnRegisterUnregisterdCreditCardForAuthError.Visible = userCreditCard.IsRegisterdStatusdAuthError;
				if (tbdyCreditCardRegisternForUnauthed.Visible)
				{
					lCreditCardInfo.Text = UserCreditCardHelper.CreateCreditCardInfoHtml(
						this.OrderInput.UserId,
						int.Parse(this.OrderInput.CreditBranchNo));
				}

				if (tbdyCreditCardRegisterForUnregisterd.Visible)
				{
					if (OrderCommon.IsPaymentCardTypeGmo)
					{
						lGmoMemberId.Text = WebSanitizer.HtmlEncode(string.Join("-",
							StringUtility.SplitByLength(userCreditCard.CooperationInfo.GMOMemberId, 4)));
					}
					else if (OrderCommon.IsPaymentCardTypeYamatoKwc)
					{
						lYamatoKwcOrderNo.Text = WebSanitizer.HtmlEncode(string.Join("-",
							StringUtility.SplitByLength(DateTime.Now.ToString("yyMMddHHmmss"), 4)));
						lYamatoKwcMemberId.Text = WebSanitizer.HtmlEncode(string.Join("-",
							StringUtility.SplitByLength(userCreditCard.CooperationInfo.YamatoKwcMemberId, 4)));
						lYamatoKwcAuthenticationKey.Text = WebSanitizer.HtmlEncode(userCreditCard.CooperationInfo.YamatoKwcAuthenticationKey);
					}
					else if (OrderCommon.IsPaymentCardTypeEScott)
					{
						lEScottKaiinId.Text = WebSanitizer.HtmlEncode(string.Join("-",
							StringUtility.SplitByLength(userCreditCard.CooperationInfo.CooperationId1, 4)));
					}
				}
			}
		}
	}

	/// <summary>
	/// Update store pickup status
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateStorePickupStatus_Click(object sender, EventArgs e)
	{
		var input = new Hashtable
		{
			{ Constants.FIELD_ORDER_ORDER_ID, this.OrderInput.OrderId },
			{ Constants.FIELD_ORDER_LAST_CHANGED, this.LoginOperatorName },
			{ Constants.FIELD_ORDER_STOREPICKUP_STATUS, rblStorePickupStatus.SelectedValue }
		};

		var storePickupStatusDate = ucUpdateStorePickupStatusDate.HfStartDate.Value.ToString();
		switch (rblStorePickupStatus.SelectedValue)
		{
			case Constants.FLG_STOREPICKUP_STATUS_ARRIVED:
				input[Constants.FIELD_ORDER_STOREPICKUP_STORE_ARRIVED_DATE] = storePickupStatusDate;
				break;

			case Constants.FLG_STOREPICKUP_STATUS_DELIVERED:
				input[Constants.FIELD_ORDER_STOREPICKUP_DELIVERED_COMPLETE_DATE] = storePickupStatusDate;
				break;

			case Constants.FLG_STOREPICKUP_STATUS_RETURNED:
				input[Constants.FIELD_ORDER_STOREPICKUP_RETURN_DATE] = storePickupStatusDate;
				break;
		}

		// Update store pickup status
		var storePickupStatusUpdated = DomainFacade.Instance.OrderService.UpdateStorePickupStatus(input);

		// Send mail to customer
		if ((rblStorePickupStatus.SelectedValue == Constants.FLG_STOREPICKUP_STATUS_ARRIVED)
			&& cbSendMailStorePickupStatus.Checked)
		{
			// Send mail
			OrderCommon.SendMailStorePickUpInformation(this.OrderInput.OrderId);
		}

		// Update history
		DomainFacade.Instance.UpdateHistoryService.InsertAllForOrder(this.OrderInput.OrderId, this.LoginOperatorName);

		Response.Redirect(CreateOrderDetailUrl(this.OrderInput.OrderId));
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
	/// <summary>Action complete message</summary>
	protected string ActionCompleteMessage
	{
		get { return (string)Session["ReissueInvoiceResultMessage"]; }
		set { Session["ReissueInvoiceResultMessage"] = value; }
	}
	/// <summary>アマゾンCV2ファサード</summary>
	private AmazonCv2ApiFacade AmazonFacade { get; set; }
	#endregion
}
