/*
=========================================================================================================
  Module      : 注文情報確認ページ処理(OrderConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using jp.veritrans.tercerog.mdk;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.UI.WebControls;
using w2.App.Common;
using w2.App.Common.Amazon;
using w2.App.Common.Amazon.Util;
using w2.App.Common.AmazonCv2;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Gooddeal;
using w2.App.Common.Input.Order;
using w2.App.Common.Mail;
using w2.App.Common.NextEngine;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.Order.OrderCombine;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.Aftee;
using w2.App.Common.Order.Payment.Atobaraicom.Shipping;
using w2.App.Common.Order.Payment.Atone;
using w2.App.Common.Order.Payment.Boku;
using w2.App.Common.Order.Payment.Boku.Utils;
using w2.App.Common.Order.Payment.DSKDeferred.OrderCancel;
using w2.App.Common.Order.Payment.ECPay;
using w2.App.Common.Order.Payment.EScott;
using w2.App.Common.Order.Payment.GMO;
using w2.App.Common.Order.Payment.GMO.GetDefPaymentStatus;
using w2.App.Common.Order.Payment.GMO.OrderModifyCancel;
using w2.App.Common.Order.Payment.GMO.Zcom.Cancel;
using w2.App.Common.Order.Payment.GMO.Zcom.Sales;
using w2.App.Common.Order.Payment.GMOAtokara;
using w2.App.Common.Order.Payment.GMOAtokara.Utils;
using w2.App.Common.Order.Payment.JACCS.ATODENE;
using w2.App.Common.Order.Payment.JACCS.ATODENE.ModifyTransaction;
using w2.App.Common.Order.Payment.LinePay;
using w2.App.Common.Order.Payment.NewebPay;
using w2.App.Common.Order.Payment.NPAfterPay;
using w2.App.Common.Order.Payment.Paidy;
using w2.App.Common.Order.Payment.Paygent;
using w2.App.Common.Order.Payment.PayPal;
using w2.App.Common.Order.Payment.Paypay;
using w2.App.Common.Order.Payment.Rakuten;
using w2.App.Common.Order.Payment.Score;
using w2.App.Common.Order.Payment.Score.Cancel;
using w2.App.Common.Order.Payment.TriLinkAfterPay;
using w2.App.Common.Order.Payment.TriLinkAfterPay.Request;
using w2.App.Common.Order.Payment.Veritrans;
using w2.App.Common.Order.Payment.YamatoKwc;
using w2.App.Common.Order.Payment.Zeus;
using w2.App.Common.Order.Reauth;
using w2.App.Common.Payment;
using w2.App.Common.Recustomer;
using w2.App.Common.Web.WebCustomControl;
using w2.Common.Extensions;
using w2.Common.Helper;
using w2.Common.Logger;
using w2.Common.Util;
using w2.Common.Web;
using w2.Common.Wrapper;
using w2.Domain;
using w2.Domain.DeliveryCompany;
using w2.Domain.FixedPurchase;
using w2.Domain.Order;
using w2.Domain.Point;
using w2.Domain.TwOrderInvoice;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.Domain.UserCreditCard;
using GMOReissue = w2.App.Common.Order.Payment.GMO.Reissue;

public partial class Form_Order_OrderConfirm : OrderPage
{
	/// <summary>親ウィンドウをリロードしないページリスト</summary>
	static string[] m_unreloadParentPages = new string[] { Constants.PAGE_MANAGER_ORDERFILEIMPORT_LIST, Constants.PAGE_W2CS_MANAGER_MESSAGE_USER_SEARCH };

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{
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

			// 親ウィンドウ更新有無をセット
			SetParentWindowReload();

			// 各コントロールの動的表示制御
			DisplayControl(sender, e);
		}
		else
		{
			// エラーメッセージのクリア
			trOrderStatusError.Visible = false;
			trOrderPaymentStatus.Visible = false;
			trOuterPaymentError.Visible = false;
			trDemandStatus.Visible = false;
			trOrderReturnExchagneStatus.Visible = false;
			trOrderRepaymentStatus.Visible = false;
			trExtendStatus.Visible = false;
			trExternalOrderImportStatus.Visible = false;
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
		// キャンセル、仮注文キャンセル、配送完了以外？
		if ((hfOrderStatus.Value != Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED)
			&& (hfOrderStatus.Value != Constants.FLG_ORDER_ORDER_STATUS_TEMP_CANCELED)
			&& (hfOrderStatus.Value != Constants.FLG_ORDER_ORDER_STATUS_DELIVERY_COMP))
		{
			// ステータス更新日指定を使用不可
			ucUpdateOrderStatusDate.Disabled = ((rblOrderStatus.SelectedValue == Constants.FLG_ORDER_ORDER_STATUS_TEMP)
				|| (rblOrderStatus.SelectedValue == Constants.FLG_ORDER_ORDER_STATUS_ORDERED));
		}
		else
		{
			// 実在庫利用を使用不可
			if (Constants.REALSTOCK_OPTION_ENABLED)
			{
				rblProductRealStockChange.Enabled
					= btnUpdateProductRealStock.Enabled = false;
			}
		}

		// Atodene後払い 出荷手配に変更時のアラート表示
		sAtodenePaymentDeadlineAlert.Visible =
			(OrderCommon.IsInvoiceBundleServiceUsable(this.OrderInput.OrderPaymentKbn)
			&& (this.OrderInput.InvoiceBundleFlg == Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_ON)
			&& (rblOrderStatus.SelectedValue == Constants.FLG_ORDER_ORDER_STATUS_SHIP_ARRANGED)
			&& (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Atodene)
			&& (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF));

		// 未入金の場合にはステータス更新日指定を使用不可とする
		ucUpdateOrderPaymentStatusDate.Disabled = (rblOrderPaymentStatus.SelectedValue == Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM);

		// 督促無しの場合にはステータス更新日指定を使用可とする
		ucUpdateDemandStatusDate.Disabled = (rblDemandStatus.SelectedValue == Constants.FLG_ORDER_DEMAND_STATUS_LEVEL0);

		// 返金無し、未返金の場合にステータス更新日指定を使用可とする
		ucOrderRepaymentStatusDate.SetEnable(
			(rblOrderRepaymentStatus.SelectedValue != Constants.FLG_ORDER_ORDER_REPAYMENT_STATUS_NOREPAYMENT)
				&& (rblOrderRepaymentStatus.SelectedValue != Constants.FLG_ORDER_ORDER_REPAYMENT_STATUS_CONFIRM));

		// Update store pickup status display
		ucUpdateStorePickupStatusDate.Disabled = (rblStorePickupStatus.SelectedValue == Constants.FLG_STOREPICKUP_STATUS_PENDING)
			|| (this.OrderInput.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP);
		cbSendMailStorePickupStatus.Enabled = (rblStorePickupStatus.SelectedValue == Constants.FLG_STOREPICKUP_STATUS_ARRIVED)
			&& (this.OrderInput.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP)
			&& (this.OrderInput.StorePickupStatus != Constants.FLG_STOREPICKUP_STATUS_ARRIVED);
		btnUpdateStorePickupStatus.Enabled = (this.OrderInput.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP);

		// 自社サイト かつ クレジット かつ キャンセル連動の場合はチェックボックスを表示する
		if (MallOptionUtility.CheckMallKbn(this.OrderInput.ShopId, this.OrderInput.MallId) == MallOptionUtility.MallKbn.OwnSite)
		{
			var isCancel = ((rblOrderStatus.SelectedValue == Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED)
				|| (rblOrderStatus.SelectedValue == Constants.FLG_ORDER_ORDER_STATUS_TEMP_CANCELED));
			var isOnlinePaymentCanceled = this.OrderInput.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_CANCELED;
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
					cbReauthCancel.Checked = cbReauthCancel.Enabled || ((cbReauthCancel.Enabled == false) && isOnlinePaymentCanceled);
				}
				else
				{
					// Amazon Payの場合は連動しない
					cbReauthCancel.Visible = isCancel;
					cbReauthCancel.Enabled = false;
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
						cbCreditZeusCancel.Enabled = cbCreditZeusCancel.Enabled && isHasCardTranId;
						cbCreditZeusCancel.Checked = cbCreditZeusCancel.Enabled || ((cbCreditZeusCancel.Enabled == false) && isOnlinePaymentCanceled && isHasCardTranId);
						break;

					case Constants.PaymentCard.SBPS:
						cbCreditSBPSCancel.Visible = isCancel;
						cbCreditSBPSCancel.Enabled = cbCreditSBPSCancel.Enabled && isHasCardTranId;
						cbCreditSBPSCancel.Checked = cbCreditSBPSCancel.Enabled || ((cbCreditSBPSCancel.Enabled == false) && isOnlinePaymentCanceled && isHasCardTranId);
						break;

					case Constants.PaymentCard.YamatoKwc:
						cbCreditYamatoKwcCancel.Visible = isCancel;
						cbCreditYamatoKwcCancel.Enabled = cbCreditYamatoKwcCancel.Enabled && isHasCardTranId;
						cbCreditYamatoKwcCancel.Checked = cbCreditYamatoKwcCancel.Enabled || ((cbCreditYamatoKwcCancel.Enabled == false) && isOnlinePaymentCanceled && isHasCardTranId);
						break;

					case Constants.PaymentCard.Gmo:
						var canCancelGmo = IsCancelEnabled() && isHasCardTranId;
						cbCreditGMOCancel.Visible = isCancel;
						cbCreditGMOCancel.Enabled = cbCreditGMOCancel.Enabled && canCancelGmo;
						cbCreditGMOCancel.Checked = cbCreditGMOCancel.Enabled || ((cbCreditGMOCancel.Enabled == false) && isOnlinePaymentCanceled && canCancelGmo);
						break;

					case Constants.PaymentCard.Zcom:
						cbCreditZcomCancel.Visible = isCancel;
						cbCreditZcomCancel.Enabled = cbCreditZcomCancel.Enabled && isHasCardTranId;
						cbCreditZcomCancel.Checked = cbCreditZcomCancel.Enabled || ((cbCreditZcomCancel.Enabled == false) && isOnlinePaymentCanceled && isHasCardTranId);
						break;

					case Constants.PaymentCard.EScott:
						cbCreditEScottCancel.Visible = isCancel;
						cbCreditEScottCancel.Enabled = cbCreditEScottCancel.Enabled && isHasCardTranId;
						cbCreditEScottCancel.Checked = cbCreditEScottCancel.Enabled || ((cbCreditEScottCancel.Enabled == false) && isOnlinePaymentCanceled && isHasCardTranId);
						break;

					case Constants.PaymentCard.VeriTrans:
						var canCancelVeritrans = IsCancelEnabled() && isHasCardTranId;
						cbVeriTransCancel.Visible = isCancel;
						cbVeriTransCancel.Enabled = cbVeriTransCancel.Enabled && canCancelVeritrans;
						cbVeriTransCancel.Checked = cbVeriTransCancel.Enabled || ((cbVeriTransCancel.Enabled == false) && isOnlinePaymentCanceled && canCancelVeritrans);
						break;

					case Constants.PaymentCard.Rakuten:
						cbCreditRakutenCancel.Visible = isCancel;
						cbCreditRakutenCancel.Enabled = (cbCreditRakutenCancel.Enabled && isHasCardTranId);
						cbCreditRakutenCancel.Checked = (cbCreditRakutenCancel.Enabled || ((cbCreditRakutenCancel.Enabled == false) && isOnlinePaymentCanceled && isHasCardTranId));
						break;

					case Constants.PaymentCard.Paygent:
						cbCreditPaygentCancel.Visible = isCancel;
						cbCreditPaygentCancel.Enabled = (cbCreditPaygentCancel.Enabled && isHasCardTranId);
						cbCreditPaygentCancel.Checked = (cbCreditPaygentCancel.Enabled || ((cbCreditPaygentCancel.Enabled == false) && isOnlinePaymentCanceled && isHasCardTranId));
						break;
				}
			}
			else if ((this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_SOFTBANKKETAI_SBPS)
				&& Constants.PAYMENT_SETTING_SBPS_SOFTBANKKETAI_CANCEL_ENABLED)
			{
				cbCareerSoftbankKetaiSBPSCancel.Visible = isCancel;
				cbCareerSoftbankKetaiSBPSCancel.Enabled = cbCareerSoftbankKetaiSBPSCancel.Enabled && isHasCardTranId;
				cbCareerSoftbankKetaiSBPSCancel.Checked = cbCareerSoftbankKetaiSBPSCancel.Enabled || ((cbCareerSoftbankKetaiSBPSCancel.Enabled == false) && isOnlinePaymentCanceled && isHasCardTranId);
			}
			else if ((this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_SBPS)
				&& Constants.PAYMENT_SETTING_SBPS_DOCOMOKETAI_CANCEL_ENABLED)
			{
				cbCareerDocoomKetaiSBPSCancel.Visible = isCancel;
				cbCareerDocoomKetaiSBPSCancel.Enabled = cbCareerDocoomKetaiSBPSCancel.Enabled && isHasCardTranId;
				cbCareerDocoomKetaiSBPSCancel.Checked = cbCareerDocoomKetaiSBPSCancel.Enabled || ((cbCareerDocoomKetaiSBPSCancel.Enabled == false) && isOnlinePaymentCanceled && isHasCardTranId);
			}
			else if ((this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AUKANTAN_SBPS)
				&& Constants.PAYMENT_SETTING_SBPS_AUKANTAN_CANCEL_ENABLED)
			{
				cbCareerAuKantanSBPSCancel.Visible = isCancel;
				cbCareerAuKantanSBPSCancel.Enabled = cbCareerAuKantanSBPSCancel.Enabled && isHasCardTranId;
				cbCareerAuKantanSBPSCancel.Checked = cbCareerAuKantanSBPSCancel.Enabled || ((cbCareerAuKantanSBPSCancel.Enabled == false) && isOnlinePaymentCanceled && isHasCardTranId);
			}
			else if ((this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_RECRUIT_SBPS)
				&& Constants.PAYMENT_SETTING_SBPS_RECRUIT_CANCEL_ENABLED)
			{
				cbRecruitSBPSCancel.Visible = isCancel;
				cbRecruitSBPSCancel.Enabled = cbRecruitSBPSCancel.Enabled && isHasCardTranId;
				cbRecruitSBPSCancel.Checked = cbRecruitSBPSCancel.Enabled || ((cbRecruitSBPSCancel.Enabled == false) && isOnlinePaymentCanceled && isHasCardTranId);
			}
			else if ((this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL_SBPS)
				&& Constants.PAYMENT_SETTING_SBPS_PAYPAL_CANCEL_ENABLED)
			{
				cbPaypalSBPSCancel.Visible = isCancel;
				cbPaypalSBPSCancel.Enabled = cbPaypalSBPSCancel.Enabled && isHasCardTranId;
				cbPaypalSBPSCancel.Checked = cbPaypalSBPSCancel.Enabled || ((cbPaypalSBPSCancel.Enabled == false) && isOnlinePaymentCanceled && isHasCardTranId);
			}
			else if ((this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_RAKUTEN_ID_SBPS)
				&& Constants.PAYMENT_SETTING_SBPS_RAKUTEN_ID_CANCEL_ENABLED)
			{
				cbRakutenIdSBPSCancel.Visible = isCancel;
				cbRakutenIdSBPSCancel.Enabled = cbRakutenIdSBPSCancel.Enabled && isHasCardTranId;
				cbRakutenIdSBPSCancel.Checked = cbRakutenIdSBPSCancel.Enabled || ((cbRakutenIdSBPSCancel.Enabled == false) && isOnlinePaymentCanceled && isHasCardTranId);
			}
			else if ((this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
				|| (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_SMS_DEF))
			{
				switch (Constants.PAYMENT_CVS_DEF_KBN)
				{
					case Constants.PaymentCvsDef.YamatoKa:
						cbYamatoKaCancel.Visible = isCancel;
						cbYamatoKaCancel.Enabled = cbYamatoKaCancel.Enabled && isHasCardTranId;
						cbYamatoKaCancel.Checked = cbYamatoKaCancel.Enabled || ((cbYamatoKaCancel.Enabled == false) && isOnlinePaymentCanceled && isHasCardTranId);
						break;
					case Constants.PaymentCvsDef.Gmo:
						cbCvsDefGmoCancel.Visible = isCancel;
						cbCvsDefGmoCancel.Enabled = cbCvsDefGmoCancel.Enabled && isHasCardTranId;
						cbCvsDefGmoCancel.Checked = cbCvsDefGmoCancel.Enabled || ((cbCvsDefGmoCancel.Enabled == false) && isOnlinePaymentCanceled && isHasCardTranId);
						break;
					case Constants.PaymentCvsDef.Atodene:
						cbCvsDefAtodeneCancel.Visible = isCancel;
						cbCvsDefAtodeneCancel.Enabled = cbCvsDefAtodeneCancel.Enabled && isHasCardTranId;
						cbCvsDefAtodeneCancel.Checked = cbCvsDefAtodeneCancel.Enabled || ((cbCvsDefAtodeneCancel.Enabled == false) && isOnlinePaymentCanceled && isHasCardTranId);
						break;
					case Constants.PaymentCvsDef.Dsk:
						cbCvsDefDskCancel.Visible = isCancel;
						cbCvsDefDskCancel.Enabled = cbCvsDefDskCancel.Enabled && isHasCardTranId;
						cbCvsDefDskCancel.Checked = cbCvsDefDskCancel.Enabled || ((cbCvsDefDskCancel.Enabled == false) && isOnlinePaymentCanceled && isHasCardTranId);
						break;
					case Constants.PaymentCvsDef.Atobaraicom:
						cbCvsDefAtobaraicomCancel.Visible = isCancel;
						cbCvsDefAtobaraicomCancel.Enabled = cbCvsDefAtobaraicomCancel.Enabled;
						cbCvsDefAtobaraicomCancel.Checked = cbCvsDefAtobaraicomCancel.Enabled;
						break;
					case Constants.PaymentCvsDef.Score:
						cbCvsDefScoreCancel.Visible = isCancel;
						cbCvsDefScoreCancel.Enabled = cbCvsDefScoreCancel.Enabled;
						cbCvsDefScoreCancel.Checked = cbCvsDefScoreCancel.Enabled;
						break;
					case Constants.PaymentCvsDef.Veritrans:
						cbCvsDefVeritransCancel.Visible = isCancel;
						cbCvsDefVeritransCancel.Enabled = cbCvsDefVeritransCancel.Enabled;
						cbCvsDefVeritransCancel.Checked = cbCvsDefVeritransCancel.Enabled;
						break;
				}

				var isNotOrderStatusCancel = ((this.OrderInput.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED)
					&& (this.OrderInput.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_TEMP_CANCELED));
				cbYamatoKaCancel.Enabled = isNotOrderStatusCancel;
				cbCvsDefGmoCancel.Enabled = isNotOrderStatusCancel;
				cbCvsDefAtodeneCancel.Enabled = isNotOrderStatusCancel;
				cbCvsDefAtobaraicomCancel.Enabled = isNotOrderStatusCancel;
				cbCvsDefScoreCancel.Enabled = isNotOrderStatusCancel;
				cbCvsDefVeritransCancel.Enabled = isNotOrderStatusCancel;
			}
			else if ((this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT))
			{
				cbAmazonPayCancel.Visible = isCancel;
				cbAmazonPayCancel.Enabled = cbAmazonPayCancel.Enabled && isHasCardTranId;
				cbAmazonPayCancel.Checked = cbAmazonPayCancel.Enabled || ((cbAmazonPayCancel.Enabled == false) && isOnlinePaymentCanceled && isHasCardTranId);
				if (this.OrderInput.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
				{
					canselAttentionMessage.Visible = isCancel;
					canselAttentionMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_REFUND_PROCESSING_AFTER_CAPTURE);
				}
			}
			else if ((this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO) || (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE))
			{
				cbGmoCancel.Visible = isCancel;
				cbGmoCancel.Enabled = cbGmoCancel.Enabled;
				cbGmoCancel.Checked = (cbGmoCancel.Enabled
					|| ((cbGmoCancel.Enabled == false) && isOnlinePaymentCanceled && isHasCardTranId));
            }
			else if ((this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2))
			{
				cbAmazonPayCV2Cancel.Visible = isCancel;
				cbAmazonPayCV2Cancel.Enabled = cbAmazonPayCV2Cancel.Enabled && isHasCardTranId;
				cbAmazonPayCV2Cancel.Checked = cbAmazonPayCV2Cancel.Enabled
					|| ((cbAmazonPayCV2Cancel.Enabled == false) && isOnlinePaymentCanceled && isHasCardTranId);
				if (this.OrderInput.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
				{
					canselAttentionMessage.Visible = isCancel;
					canselAttentionMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_REFUND_PROCESSING_AFTER_CAPTURE);
				}
			}
			else if (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
			{
				cbPaypalCancel.Visible = isCancel;
				cbPaypalCancel.Enabled = (cbPaypalCancel.Enabled && isHasCardTranId);
				cbPaypalCancel.Checked = (cbPaypalCancel.Enabled
					|| ((cbPaypalCancel.Enabled == false) && isOnlinePaymentCanceled && isHasCardTranId));
			}
			else if (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
			{
				cbTriLinkAfterPay.Visible = isCancel;
				cbTriLinkAfterPay.Enabled = (cbTriLinkAfterPay.Enabled && isHasPaymentOrderId);
				cbTriLinkAfterPay.Checked = (cbTriLinkAfterPay.Enabled || ((cbTriLinkAfterPay.Enabled == false) && isOnlinePaymentCanceled && isHasPaymentOrderId));
			}
			else if (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
			{
				cbPaidyPayCancel.Visible = isCancel;
				cbPaidyPayCancel.Enabled = cbPaidyPayCancel.Enabled
					&& isHasPaymentOrderId
					&& this.OrderInput.ExternalPaymentStatus != Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_EXPIRED
					&& this.OrderInput.ExternalPaymentStatus != Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_CANCEL_EXPIRED;
				cbPaidyPayCancel.Checked = cbPaidyPayCancel.Enabled
					|| ((cbPaidyPayCancel.Enabled == false)
						&& isOnlinePaymentCanceled
						&& isHasCardTranId
						&& this.OrderInput.ExternalPaymentStatus != Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_EXPIRED
						&& this.OrderInput.ExternalPaymentStatus != Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_CANCEL_EXPIRED);
				if (this.OrderInput.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
				{
					canselAttentionMessage.Visible = isCancel;
					canselAttentionMessage.InnerHtml = "売上確定後は与信取り消しではなく返金処理となります。";
				}
			}
			else if ((this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE))
			{
				cbAtonePayCancel.Visible = isCancel;
				cbAtonePayCancel.Enabled = (cbAtonePayCancel.Enabled && isHasCardTranId);
				cbAtonePayCancel.Checked = (cbAtonePayCancel.Enabled
					|| ((cbAtonePayCancel.Enabled == false)
						&& isOnlinePaymentCanceled
						&& isHasCardTranId));
			}
			else if ((this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE))
			{
				cbAfteePayCancel.Visible = isCancel;
				cbAfteePayCancel.Enabled = (cbAfteePayCancel.Enabled && isHasCardTranId);
				cbAfteePayCancel.Checked = (cbAfteePayCancel.Enabled
					|| ((cbAfteePayCancel.Enabled == false)
						&& isOnlinePaymentCanceled
						&& isHasCardTranId));
			}
			else if (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY)
			{
				cbLinePayCancel.Visible = isCancel;
				cbLinePayCancel.Enabled = (cbLinePayCancel.Enabled && isHasCardTranId);
				cbLinePayCancel.Checked = (cbLinePayCancel.Enabled
					|| ((cbLinePayCancel.Enabled == false) && isOnlinePaymentCanceled && isHasCardTranId));
			}
			else if (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
			{
				cbNPAfterPayCancel.Visible = isCancel;
				cbNPAfterPayCancel.Enabled = (cbNPAfterPayCancel.Enabled && isHasCardTranId);
				cbNPAfterPayCancel.Checked = (cbNPAfterPayCancel.Enabled
					|| ((cbNPAfterPayCancel.Enabled == false) && isOnlinePaymentCanceled && isHasCardTranId));
			}
			else if ((this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
				&& (this.OrderInput.ExternalPaymentType == Constants.FLG_PAYMENT_TYPE_ECPAY_CREDIT))
			{
				cbEcPayCancel.Visible = isCancel;
				cbEcPayCancel.Enabled = cbEcPayCancel.Enabled && isHasCardTranId;
				cbEcPayCancel.Checked = cbEcPayCancel.Enabled
					|| ((cbEcPayCancel.Enabled == false) && isOnlinePaymentCanceled && isHasCardTranId);
			}
			else if ((this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
				&& (this.OrderInput.ExternalPaymentType == Constants.FLG_PAYMENT_TYPE_NEWEBPAY_CREDIT))
			{
				cbNewebPayCancel.Visible = isCancel;
				cbNewebPayCancel.Enabled = cbNewebPayCancel.Enabled && isHasCardTranId;
				cbNewebPayCancel.Checked = cbNewebPayCancel.Enabled
					|| ((cbNewebPayCancel.Enabled == false) && isOnlinePaymentCanceled && isHasCardTranId);
			}
			else if (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
			{
				cbPayPayCancel.Visible = isCancel;
				cbPayPayCancel.Enabled = (cbPayPayCancel.Enabled
					&& (isHasCardTranId
						|| (isHasPaymentOrderId
							&& (Constants.PAYMENT_PAYPAY_KBN == Constants.PaymentPayPayKbn.VeriTrans))));
				cbPayPayCancel.Checked = (cbPayPayCancel.Enabled
					|| ((cbPayPayCancel.Enabled == false)
						&& isOnlinePaymentCanceled
						&& (isHasCardTranId
							|| (isHasPaymentOrderId
								&& (Constants.PAYMENT_PAYPAY_KBN == Constants.PaymentPayPayKbn.VeriTrans)))));
			}
			else if (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU)
			{
				cbBokuCancel.Visible = isCancel;
				cbBokuCancel.Enabled = cbBokuCancel.Enabled;
				cbBokuCancel.Checked = (cbBokuCancel.Enabled
					|| ((cbBokuCancel.Enabled == false) && isOnlinePaymentCanceled && isHasCardTranId));
			}
			else if (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA)
			{
				cbGmoAtokaraCancel.Visible = isCancel;
				cbGmoAtokaraCancel.Enabled = cbGmoAtokaraCancel.Enabled;
				cbGmoAtokaraCancel.Checked = (cbGmoAtokaraCancel.Enabled
					|| ((cbGmoAtokaraCancel.Enabled == false) && isOnlinePaymentCanceled && isHasCardTranId));
			}

			if (Constants.TWSHIPPING_GOODDEAL_OPTION_ENABLED)
			{
				var isRegisterShippingDelivery =
					(this.OrderInput.LogiCooperationStatus == Constants.FLG_ORDER_LOGI_COOPERATION_STATUS_COMPLETE);
				cbGooddealShippingCancel.Visible = isCancel;
				cbGooddealShippingCancel.Enabled = (cbGooddealShippingCancel.Enabled && isRegisterShippingDelivery);
				cbGooddealShippingCancel.Checked = (cbGooddealShippingCancel.Enabled
					|| ((cbGooddealShippingCancel.Enabled == false) && isRegisterShippingDelivery));
			}

			// Recustomer連携用チェックボックス表示処理
			if (Constants.RECUSTOMER_API_OPTION_ENABLED)
			{
				cbRecustomerCooperation.Visible = cbRecustomerCooperation.Checked
					= (rblOrderStatus.SelectedValue == Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP)
						&& (this.OrderInput.ExtendStatus47 == Constants.FLG_ORDER_EXTEND_STATUS_OFF);
			}

			if (OrderCommon.DisplayTwInvoiceInfo() == false) return;

			var twOrderInvoice = new TwOrderInvoiceService().GetOrderInvoice(
				this.OrderInput.OrderId,
				int.Parse(this.OrderInput.Shippings[0].OrderShippingNo));

			rbTwInvoiceCancel.Visible = rbTwInvoiceRefund.Visible =
				(isCancel
					&& (twOrderInvoice.TwInvoiceStatus == Constants.FLG_ORDER_INVOICE_STATUS_ISSUED_LINKED));
			if (rbTwInvoiceCancel.Visible) rbTwInvoiceCancel.Checked = true;
		}
	}

	/// <summary>
	/// 注文ステータス更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateOrderStatus_Click(object sender, System.EventArgs e)
	{
		// 注文がすでにキャンセルされていないか確認
		CheckOrderStatusIsNotCancel();

		// 注文ステータス取得
		var selectedOrderStatus = rblOrderStatus.SelectedValue;

		// ステータス更新日指定入力チェック
		// 注文ステータスが受注承認、在庫引当、出荷手配済み、出荷完了、配送完了、キャンセル、仮注文キャンセルの場合は
		// ステータス更新日指定チェック・取得
		string updateDate = string.Empty;
		if ((selectedOrderStatus == Constants.FLG_ORDER_ORDER_STATUS_ORDER_RECOGNIZED)
			|| (selectedOrderStatus == Constants.FLG_ORDER_ORDER_STATUS_STOCK_RESERVED)
			|| (selectedOrderStatus == Constants.FLG_ORDER_ORDER_STATUS_SHIP_ARRANGED)
			|| (selectedOrderStatus == Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP)
			|| (selectedOrderStatus == Constants.FLG_ORDER_ORDER_STATUS_DELIVERY_COMP)
			|| (selectedOrderStatus == Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED)
			|| (selectedOrderStatus == Constants.FLG_ORDER_ORDER_STATUS_TEMP_CANCELED))
		{
			var errorMessage = CheckInputUpdateDate(
				DateTime.Parse(ucUpdateOrderStatusDate.HfStartDate.Value).Year.ToString(),
				DateTime.Parse(ucUpdateOrderStatusDate.HfStartDate.Value).Month.ToString(),
				DateTime.Parse(ucUpdateOrderStatusDate.HfStartDate.Value).Day.ToString(),
				out updateDate);
			if (errorMessage.Length != 0)
			{
				trOrderStatusError.Visible = true;
				lOrderStatusError.Text = errorMessage;
				return;
			}
		}

		// 外部連携取込ステータスが「正常」でない場合は更新不可
		if (Constants.FLG_USER_MALL_ID_EXTERNAL_ORDER_SITES.Any(site => site == this.OrderInput.MallId)
			&& (rblExternalImportStatus.SelectedValue != Constants.FLG_ORDER_EXTERNAL_IMPORT_STATUS_SUCCESS))
		{
			trOrderStatusError.Visible = true;
			lOrderStatusError.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_EXTERNAL_IMPORT_STATUS_NOT_COMP_ERROR).Replace("@@ 1 @@", rblExternalImportStatus.Text);
			return;
		}

		// 注文ステータス更新して注文詳細画面へ
		UpdateStatusAndGotoOrderDetail(Constants.StatusType.Order, selectedOrderStatus, updateDate, UpdateHistoryAction.Insert);
	}

	/// <summary>
	/// 商品実在庫更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateProductRealStock_Click(object sender, System.EventArgs e)
	{
		var selectedProductRealStockChange = rblProductRealStockChange.SelectedValue;
		using (var accessor = new SqlAccessor())
		{
			// トランザクション開始
			accessor.OpenConnection();
			accessor.BeginTransaction();

			var history = new OrderHistory
			{
				OrderId = this.OrderInput.OrderId,
				Action = OrderHistory.ActionType.EcOrderConfirm,
				OpearatorName = this.LoginOperatorName,
				Accessor = accessor,
				UpdateAction = OrderHistory.GetProductRealStockAction(selectedProductRealStockChange),
			};

			// Begin write history
			history.HistoryBegin();

			switch (selectedProductRealStockChange)
			{
				// 実在庫引当処理
				case Constants.FLG_ORDERWORKFLOWSETTING_PRODUCT_REALSTOCK_CHANGE_RESERVED_STCOK:
					UpdateOrderItemRealStockReserved(this.OrderInput.OrderId, this.OrderInput.ShopId, UpdateHistoryAction.DoNotInsert, accessor);
					break;

				// 実在庫出荷処理
				case Constants.FLG_ORDERWORKFLOWSETTING_PRODUCT_REALSTOCK_CHANGE_FORWARD_STCOK:
					UpdateOrderItemRealStockShipped(this.OrderInput.OrderId, this.OrderInput.ShopId, UpdateHistoryAction.DoNotInsert, accessor);
					break;

				// 実在庫戻し処理
				case Constants.FLG_ORDERWORKFLOWSETTING_PRODUCT_REALSTOCK_CHANGE_CANCEL_REALSTCOK:
					UpdateOrderItemRealStockCanceled(this.OrderInput.OrderId, this.OrderInput.ShopId, UpdateHistoryAction.DoNotInsert, accessor);
					break;
			}

			// 更新履歴登録
			new UpdateHistoryService().InsertForOrder(this.OrderInput.OrderId, this.LoginOperatorName, accessor);

			// Write history complete
			history.HistoryComplete();

			// 更新処理確定
			accessor.CommitTransaction();
		}

		// 注文詳細へ
		Response.Redirect(CreateOrderDetailUrl(this.OrderInput.OrderId));
	}

	/// <summary>
	/// 入金ステータス更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateOrderPaymentStatus_Click(object sender, System.EventArgs e)
	{
		// ステータス更新日指定入力チェック
		// 入金ステータスが入金済み、一部入金の場合はステータス更新日指定チェック・取得
		var updateDate = string.Empty;
		if ((rblOrderPaymentStatus.SelectedValue == Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE)
			|| (rblOrderPaymentStatus.SelectedValue == Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_SHORTAGE))
		{
			var errorMessage = CheckInputUpdateDate(
				DateTime.Parse(ucUpdateOrderPaymentStatusDate.HfStartDate.Value).Year.ToString(),
				DateTime.Parse(ucUpdateOrderPaymentStatusDate.HfStartDate.Value).Month.ToString(),
				DateTime.Parse(ucUpdateOrderPaymentStatusDate.HfStartDate.Value).Day.ToString(),
				out updateDate);
			if (errorMessage.Length != 0)
			{
				trOrderPaymentStatus.Visible = true;
				lOrderPaymentStatus.Text = errorMessage;
				return;
			}
			else
			{
				updateDate = string.Format("{0} {1}",
					updateDate,
					DateTime.Parse(ucUpdateOrderPaymentStatusDate.HfStartTime.Value).ToString("HH:mm:ss"));
			}
		}

		// 入金ステータス更新して注文詳細画面へ
		UpdateStatusAndGotoOrderDetail(
			Constants.StatusType.Payment,
			rblOrderPaymentStatus.SelectedValue,
			updateDate,
			UpdateHistoryAction.Insert);
	}

	/// <summary>
	/// 実売上連動処理ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCardRealSales_Click(object sender, System.EventArgs e)
	{
		// 売上処理日指定入力チェック・取得
		var updateDate = string.Empty;
		var error = string.Empty;
		if (string.IsNullOrEmpty(ucCardRealSalesDate.HfStartDate.Value) == false)
		{
			error = CheckInputUpdateDate(
				DateTime.Parse(ucCardRealSalesDate.HfStartDate.Value).Year.ToString(),
				DateTime.Parse(ucCardRealSalesDate.HfStartDate.Value).Month.ToString(),
				DateTime.Parse(ucCardRealSalesDate.HfStartDate.Value).Day.ToString(),
			out updateDate);
			if (string.IsNullOrEmpty(error))
			{
				updateDate = string.Format("{0} {1}",
					updateDate,
					DateTime.Parse(ucCardRealSalesDate.HfStartTime.Value).ToString("HH:mm:ss"));
			}
		}
		var apiErrorMessage = "";

		// 最終請求金額が0円の場合はエラーとする
		if (this.SendingAmount == 0)
		{
			error =
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CARD_REALSALES_ERROR)
					.Replace("@@ 1 @@", WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_THE_FINAL_BILLING_AMOUNT));
		}

		// デジタルコンテンツ商品あり判定
		var isDigitalContents = false;
		if (Constants.DIGITAL_CONTENTS_OPTION_ENABLED)
		{
			foreach (OrderItemInput orderItem in this.OrderInput.Shippings.SelectMany(s => s.Items).ToArray())
			{
				var product = ProductCommon.GetProductVariationInfo(orderItem.ShopId, orderItem.ProductId, orderItem.VariationId, string.Empty);
				if ((product.Count != 0)
					&& ((string)product[0][Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG] == Constants.FLG_PRODUCT_DIGITAL_CONTENTS_FLG_VALID))
				{
					isDigitalContents = true;
					break;
				}
			}
		}

		if (error == "")
		{
			string orderId = this.OrderInput.OrderId;
			string paymentOrderId = this.OrderInput.PaymentOrderId;

			// Get data input action
			var actionInput = new Hashtable
			{
				{ Constants.TABLE_ORDER, new List<string>() { Constants.FIELD_ORDER_ONLINE_PAYMENT_STATUS } }
			};

			var history = new OrderHistory
			{
				OrderId = this.OrderInput.OrderId,
				Action = OrderHistory.ActionType.EcOrderConfirm,
				OpearatorName = this.LoginOperatorName,
				UpdateAction = actionInput,
			};

			// Begin write history
			history.HistoryBegin();

			// 実売上連動処理
			// ZEUS決済連動処理？
			if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Zeus)
			{
				// 仮売り・本売りの場合(Zeusクーリングオフ)
				if ((isDigitalContents ? Constants.PAYMENT_SETTING_ZEUS_PAYMENTMETHOD_FORDIGITALCONTENTS : Constants.PAYMENT_SETTING_ZEUS_PAYMENTMETHOD) == Constants.PaymentCreditCardPaymentMethod.PAYMENT_WITH_AUTH)
				{
					var result = new ZeusCoolingOffBatchApi().Exec(
						this.SendingAmount,
						StringUtility.ToDateString(updateDate, "yyyyMMdd"),
						this.OrderInput.CardTranId);
					if (result.Success)
					{
						UpdatePaymentStatusSalesComplete(orderId, Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED, string.Empty, UpdateHistoryAction.DoNotInsert);
					}
					else
					{
						error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CARD_REALSALES_ERROR).Replace("@@ 1 @@", result.ErrorMessage);
						apiErrorMessage = result.ErrorMessage;
					}
				}
				// 与信後決済の場合(SecureLinkで即売)
				else if ((isDigitalContents ? Constants.PAYMENT_SETTING_ZEUS_PAYMENTMETHOD_FORDIGITALCONTENTS : Constants.PAYMENT_SETTING_ZEUS_PAYMENTMETHOD) == Constants.PaymentCreditCardPaymentMethod.PAYMENT_AFTER_AUTH)
				{
					// 2重請求防止ロック
					lock (Session)
					{
						// 決済取引IDが空かどうかをthis.OrderMasterは参照せず改めて取得して確認
						if (this.GetCardTranId(orderId) == "")
						{
							// 決済取引IDに仮IDを格納
							new OrderService().UpdateCardTranId(
								orderId,
								Constants.FLG_REALSALES_TEMP_TRAN_ID,
								this.LoginOperatorName,
								UpdateHistoryAction.DoNotInsert);

							// 管理画面ではユーザの登録削除にかかわらず取得する
							var userCreditCard = UserCreditCard.Get(this.OrderInput.UserId, int.Parse(this.OrderInput.CreditBranchNo));

							var mailAddress = string.Empty;
							if (this.OrderInput.Owner.OwnerMailAddr.Length != 0)
							{
								mailAddress = this.OrderInput.Owner.OwnerMailAddr;
							}
							else if (this.OrderInput.Owner.OwnerMailAddr2.Length != 0)
							{
								mailAddress = this.OrderInput.Owner.OwnerMailAddr2;
							}

							// 仮決済取引IDが入っているかをthis.OrderMasterは参照せず改めて取得して確認
							if (this.GetCardTranId(orderId) == Constants.FLG_REALSALES_TEMP_TRAN_ID)
							{
								try
								{
									AppLogger.WriteDebug("受注確認画面ZEUS決済連携処理：" + orderId);

									var result = new ZeusSecureLinkBatchApi().Exec(
										this.SendingAmount,
										userCreditCard.CooperationInfo.ZeusTelNo,
										mailAddress,
										userCreditCard.CooperationInfo.ZeusSendId,
										this.OrderInput.CardInstallmentsCode);
									if (result.Success)
									{
										// 外部決済ステータス更新
										UpdatePaymentStatusSalesComplete(orderId, Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED, result.ZeusOrderId, UpdateHistoryAction.DoNotInsert);
									}
									else
									{
										error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CARD_REALSALES_ERROR).Replace("@@ 1 @@", result.ErrorMessage);
										apiErrorMessage = result.ErrorMessage;
									}
								}
								catch (WebException webEx)
								{
									error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CARD_REALSALES_CONNECTION_ERROR);
									AppLogger.WriteError(error + "[受注ID : " + orderId + "]", webEx);

									// 誤って再実行されるのを防ぐため、決済取引IDにエラーメッセージを格納
									new OrderService().UpdateCardTranId(orderId, error, this.LoginOperatorName, UpdateHistoryAction.DoNotInsert);
								}
							}
						}
						else
						{
							error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CARD_ALREADY_REALSALES_ERROR).Replace("@@ 1 @@", this.OrderInput.OrderId);
						}
					}
				}
			}
			// SBPS決済連動処理？
			else if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.SBPS)
			{
				// 「指定日売」のとき
				if ((isDigitalContents ? Constants.PAYMENT_SETTING_SBPS_CREDIT_PAYMENTMETHOD_FORDIGITALCONTENTS : Constants.PAYMENT_SETTING_SBPS_CREDIT_PAYMENTMETHOD) == Constants.PaymentCreditCardPaymentMethod.PAYMENT_WITH_AUTH)
				{
					var saleApi = new PaymentSBPSCreditSaleApi();
					if (saleApi.Exec(this.OrderInput.CardTranId, this.SendingAmount))
					{
						UpdatePaymentStatusSalesComplete(orderId, Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED, string.Empty, UpdateHistoryAction.DoNotInsert);
					}
					else
					{
						error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CARD_REALSALES_ERROR).Replace("@@ 1 @@", saleApi.ResponseData.ResErrMessages);
						apiErrorMessage = LogCreator.CreateErrorMessage(
							saleApi.ResponseData.ResErrCode,
							saleApi.ResponseData.ResErrMessages);

					}
				}
				// 与信後決済の場合(SBSP「顧客参照」＆「リアル与信」で即売)
				else if ((isDigitalContents ? Constants.PAYMENT_SETTING_SBPS_CREDIT_PAYMENTMETHOD_FORDIGITALCONTENTS : Constants.PAYMENT_SETTING_SBPS_CREDIT_PAYMENTMETHOD) == Constants.PaymentCreditCardPaymentMethod.PAYMENT_AFTER_AUTH)
				{
					// 2重請求防止ロック
					lock (Session)
					{
						// 決済取引IDが空かどうかをthis.OrderMasterは参照せず改めて取得して確認
						if (this.GetCardTranId(orderId) == "")
						{
							// 決済取引IDに仮IDを格納
							new OrderService().UpdateCardTranId(orderId, Constants.FLG_REALSALES_TEMP_TRAN_ID, this.LoginOperatorName, UpdateHistoryAction.DoNotInsert);

							// 管理画面ではユーザの登録削除にかかわらず取得する
							var userCreditCard = UserCreditCard.Get(this.OrderInput.UserId, int.Parse(this.OrderInput.CreditBranchNo));

							// 仮決済取引IDが入っているかをthis.OrderMasterは参照せず改めて取得して確認
							if (this.GetCardTranId(orderId) == Constants.FLG_REALSALES_TEMP_TRAN_ID)
							{
								try
								{
									AppLogger.WriteDebug("受注確認画面SBPS決済連携処理：" + orderId);

									// 決済注文ID生成
									paymentOrderId = OrderCommon.CreatePaymentOrderId(this.OrderInput.ShopId);

									// SBPSクレジット「リアル与信」実行
									var authApi = new PaymentSBPSCreditAuthApi();
									var apiError = authApi.Exec(
										userCreditCard.CooperationInfo.SBPSCustCode,
										paymentOrderId,
										Constants.PAYMENT_SETTING_SBPS_ITEM_ID,
										Constants.PAYMENT_SETTING_SBPS_ITEM_NAME,
										new List<PaymentSBPSBase.ProductItem>(),
										this.SendingAmount,
										PaymentSBPSUtil.GetCreditDivideInfo(this.OrderInput.CardInstallmentsCode))
										? "" : authApi.ResponseData.ResErrMessages;

									if (apiError == "")
									{
										apiErrorMessage = LogCreator.CreateErrorMessage(
											authApi.ResponseData.ResErrCode,
											authApi.ResponseData.ResErrMessages);

										// SBPSクレジット「コミット」処理
										var commitApi = new PaymentSBPSCreditCommitApi();
										if (commitApi.Exec(authApi.ResponseData.ResTrackingId))
										{
											// 外部決済ステータス更新
											UpdatePaymentStatusSalesComplete(orderId, Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED, authApi.ResponseData.ResTrackingId, UpdateHistoryAction.DoNotInsert);
										}
										else
										{
											apiError = commitApi.ResponseData.ResErrMessages;
											apiErrorMessage = string.Format("エラーコード:{0}\tエラーメッセージ{1}", commitApi.ResponseData.ResErrCode, commitApi.ResponseData.ResErrMessages);
										}
									}

									if (apiError != "") error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CARD_REALSALES_ERROR).Replace("@@ 1 @@", apiError);
								}
								catch (WebException webEx)
								{
									error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CARD_REALSALES_CONNECTION_ERROR);
									AppLogger.WriteError(error + "[受注ID : " + orderId + "]", webEx);

									// 誤って再実行されるのを防ぐため、決済取引IDにエラーメッセージを格納
									new OrderService().UpdateCardTranId(orderId, error, this.LoginOperatorName, UpdateHistoryAction.DoNotInsert);
								}
							}
						}
						else
						{
							error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CARD_ALREADY_REALSALES_ERROR).Replace("@@ 1 @@", this.OrderInput.OrderId);
						}
					}
				}
			}
			// GMO決済連動処理？
			else if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Gmo)
			{
				// 仮売り⇒本売りの場合
				if (Constants.PAYMENT_SETTING_GMO_PAYMENTMETHOD == Constants.GmoCreditCardPaymentMethod.Auth)
				{
					// w2、GMO側で金額が異なるか？
					var paymentGMO = new PaymentGmoCredit();
					if (paymentGMO.IsPriceChange(
						paymentOrderId,
						this.OrderInput.CardTranId,
						this.SendingAmount))
					{
						// GMO側の金額変更
						if (paymentGMO.ChangeTran(
							paymentOrderId,
							this.OrderInput.CardTranId,
							this.SendingAmount) == false)
						{
							error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CARD_REALSALES_ERROR).Replace("@@ 1 @@", paymentGMO.ErrorMessages);
							apiErrorMessage = paymentGMO.ErrorMessages;
						}
					}
					if (error == "")
					{
						// 仮売上⇒実売上
						if (paymentGMO.Sales(
							paymentOrderId,
							this.OrderInput.CardTranId,
							this.SendingAmount) == false)
						{
							apiErrorMessage = paymentGMO.ErrorMessages;
							error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CARD_REALSALES_ERROR).Replace("@@ 1 @@", paymentGMO.ErrorMessages);
						}
						else
						{
							UpdatePaymentStatusSalesComplete(
								orderId,
								Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED,
								string.Empty,
								UpdateHistoryAction.DoNotInsert);
						}
					}
				}
			}
			else if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Zcom)
			{
				// ZcomPayment
				var adp = new ZcomSalesRequestAdapter(paymentOrderId);
				var res = adp.Execute();

				if (res.IsSuccessResult() == false)
				{
					error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CARD_REALSALES_ERROR).Replace("@@ 1 @@", res.GetErrorDetailValue());
					apiErrorMessage = LogCreator.CreateErrorMessage(res.GetErrorCodeValue(), res.GetErrorDetailValue());
				}
				else
				{
					UpdatePaymentStatusSalesComplete(
						orderId,
						Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED,
						string.Empty,
						UpdateHistoryAction.DoNotInsert);
				}
			}
			else if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.EScott)
			{
				// 2重請求防止ロック
				lock (Session)
				{
					// EScott
					var success = false;
					var cardTranId = string.Empty;
					var responseCd = string.Empty;
					var responseMessage = string.Empty;

					var escottPaymentMethod = isDigitalContents
						? Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_PAYMENTMETHOD
						: Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_PAYMENTMETHOD_FORDIGITALCONTENTS;
					if (escottPaymentMethod == Constants.PaymentCreditCardPaymentMethod.PAYMENT_WITH_AUTH)
					{
						var adp = EScottProcess1CaptureApi.CreateEScottProcess1CaptureApi(
							this.OrderInput.CardTranId,
							paymentOrderId,
							DateTime.Parse(updateDate));
						var res = adp.ExecRequest();
						success = res.IsSuccess;
						responseCd = res.ResponseCd;
						responseMessage = res.ResponseMessage;
						cardTranId = res.CardTranId;
					}
					else
					{
						// 決済取引IDが空かどうかをthis.OrderMasterは参照せず改めて取得して確認
						if (string.IsNullOrEmpty(this.GetCardTranId(orderId)))
						{
							// 決済取引IDに仮IDを格納
							new OrderService().UpdateCardTranId(orderId, Constants.FLG_REALSALES_TEMP_TRAN_ID, this.LoginOperatorName, UpdateHistoryAction.DoNotInsert);

							var userCreditCard = new UserCreditCard(new UserCreditCardService().Get(
								this.OrderInput.UserId,
								int.Parse(this.OrderInput.CreditBranchNo)));

							// 仮決済取引IDが入っているかをthis.OrderMasterは参照せず改めて取得して確認
							if (this.GetCardTranId(orderId) == Constants.FLG_REALSALES_TEMP_TRAN_ID)
							{
								try
								{
									var adp = EScottMaster1GatheringApi.CreateEScottMaster1GatheringApi(
										this.OrderInput.PaymentOrderId,
										decimal.Parse(this.OrderInput.LastBilledAmount),
										this.OrderInput.CardInstallmentsCode,
										userCreditCard,
										DateTime.Parse(updateDate));
									var res = adp.ExecRequest();
									success = res.IsSuccess;
									responseCd = res.ResponseCd;
									responseMessage = res.ResponseMessage;
									cardTranId = res.CardTranId;
								}
								catch (WebException webEx)
								{
									error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CARD_REALSALES_CONNECTION_ERROR);
									AppLogger.WriteError(error + "[受注ID : " + orderId + "]", webEx);

									// 誤って再実行されるのを防ぐため、決済取引IDにエラーメッセージを格納
									new OrderService().UpdateCardTranId(orderId, error, this.LoginOperatorName, UpdateHistoryAction.DoNotInsert);
								}
							}
						}
					}

					if (success == false)
					{
						error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CARD_REALSALES_ERROR).Replace("@@ 1 @@", responseMessage);
						apiErrorMessage = LogCreator.CreateErrorMessage(responseCd, responseMessage);
					}
					else
					{
						UpdatePaymentStatusSalesComplete(
							orderId,
							Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED,
							cardTranId,
							UpdateHistoryAction.DoNotInsert);
					}
				}
			}
			else if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans)
			{
				var veritransPaymentMethod = isDigitalContents
					? Constants.PAYMENT_SETTING_VERITRANS4G_PAYMENTMETHOD_FORDIGITALCONTENTS
					: Constants.PAYMENT_SETTING_VERITRANS4G_PAYMENTMETHOD;

				if (veritransPaymentMethod == Constants.VeritransCreditCardPaymentMethod.Auth)
				{
					var paymentVeritransCredit = new PaymentVeritransCredit();
					var response = paymentVeritransCredit.Capture(paymentOrderId, this.SendingAmount.ToPriceString());

					if (response.Mstatus != "success")
					{
						error = MessageManager.GetMessages(
							CommerceMessages.ERRMSG_MANAGER_ORDER_CARD_REALSALES_ERROR).Replace("@@ 1 @@", response.MerrMsg);
						apiErrorMessage = LogCreator.CreateErrorMessage(response.VResultCode, response.MerrMsg);
					}
					else
					{
						UpdatePaymentStatusSalesComplete(
							orderId,
							Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED,
							string.Empty,
							UpdateHistoryAction.DoNotInsert);
					}
				}
			}
			else if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten)
			{
				if (Constants.PAYMENT_RAKUTEN_CREDIT_PAYMENT_METHOD == Constants.RakutenPaymentType.AUTH)
				{
					var rakutenApiExecResult = RakutenApiFacade.Capture(new RakutenCaptureRequest
					{
						PaymentId = paymentOrderId
					});
					if (rakutenApiExecResult.ResultType != RakutenConstants.RESULT_TYPE_SUCCESS)
					{
						error = MessageManager.GetMessages(
							CommerceMessages.ERRMSG_MANAGER_ORDER_CARD_REALSALES_ERROR).Replace("@@ 1 @@", rakutenApiExecResult.ErrorMessage);
						apiErrorMessage = LogCreator.CreateErrorMessage(rakutenApiExecResult.ErrorCode, rakutenApiExecResult.ErrorMessage);
					}
					else
					{
						UpdatePaymentStatusSalesComplete(
							orderId,
							Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED,
							string.Empty,
							UpdateHistoryAction.DoNotInsert);
					}
				}
			}
			// ペイジェントクレカ
			else if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Paygent)
			{
				// ヘッダのみを送る電文
				var sellsParams = new PaygentApiHeader(PaygentConstants.PAYGENT_APITYPE_CARD_REALSALE);
				// ペイジェント側決済IDはカード取引IDに格納されている
				sellsParams.PaymentId = this.OrderInput.CardTranId;
				var result = PaygentApiFacade.SendRequest(sellsParams);

				// 成功で返ってきたらW2側のステータスを更新
				if ((string)result[PaygentConstants.RESPONSE_STATUS] == PaygentConstants.PAYGENT_RESPONSE_STATUS_SUCCESS)
				{
					UpdatePaymentStatusSalesComplete(
						orderId,
						Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED,
						string.Empty,
						UpdateHistoryAction.DoNotInsert);

					UpdateStatusAndGotoOrderDetail(
						Constants.StatusType.Payment,
						Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE,
						updateDate,
						UpdateHistoryAction.DoNotInsert,
						false);
				}
				else
				{
					error = "ペイジェントクレジット売上確定に失敗しました。\n注文情報を確認し、決済管理画面より処理を行ってください。\nエラーメッセージ："
						+ result[PaygentConstants.RESPONSE_DETAIL];
				}
			}

			if (string.IsNullOrEmpty(error))
			{
				// 更新履歴登録
				new UpdateHistoryService().InsertForOrder(
					orderId,
					this.LoginOperatorName);

				// Write history complete
				history.HistoryComplete();
			}
		}

		var hasError = error.Length != 0;

		// 外部決済連携ログ格納処理
		OrderCommon.AppendExternalPaymentCooperationLog(
			hasError == false,
			this.OrderInput.OrderId,
			hasError ? apiErrorMessage : LogCreator.CreateMessage(this.OrderInput.OrderId, ""),
			this.LoginOperatorName,
			UpdateHistoryAction.Insert);

		// 注文詳細へ
		if (hasError == false)
		{
			Response.Redirect(CreateOrderDetailUrl(this.OrderInput.OrderId));
		}
		else
		{
			trOuterPaymentError.Visible = true;
			lOuterPaymentError.Text = error;
		}
	}

	/// <summary>
	/// 外部決済連携（カード売上確定以外）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnExternalPayment_Click(object sender, EventArgs e)
	{
		var error = string.Empty;
		var successOnlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;

		// 売上確定処理
		switch (hfOrderPaymentKbn.Value)
		{
			case Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_ORG:
				error = ExecRealSaleDocomoKetaiOrg();
				break;

			case Constants.FLG_PAYMENT_PAYMENT_ID_SMATOMETE_ORG:
				error = ExecRealSaleSMatometeOrg();
				break;

			case Constants.FLG_PAYMENT_PAYMENT_ID_SOFTBANKKETAI_SBPS:
				error = ExecRealSaleSoftbankKetaiSBPS();
				break;

			case Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_SBPS:
				error = ExecRealSaleDocomoKetaiSBPS();
				break;

			case Constants.FLG_PAYMENT_PAYMENT_ID_AUKANTAN_SBPS:
				error = ExecRealSaleAuKantanSBPS();
				break;

			case Constants.FLG_PAYMENT_PAYMENT_ID_RECRUIT_SBPS:
				error = ExecRealSaleRecruitSBPS();
				break;

			case Constants.FLG_PAYMENT_PAYMENT_ID_RAKUTEN_ID_SBPS:
				error = ExecRealSaleRakutenIdSBPS();
				break;

			case Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT:
				error = ExecRealSaleAmazonPay();
				break;

			case Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2:
				error = ExecRealSaleAmazonPayCV2();
				break;

			case Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL:
				error = ExecRealSalePayPal();
				break;

			case Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY:
				if ((Constants.PAYMENT_PAIDY_KBN == Constants.PaymentPaidyKbn.Paygent)
					&& (Constants.GLOBAL_OPTION_ENABLE
						|| (Constants.PAYMENT_PAIDY_OPTION_ENABLED == false)
						|| (this.OrderInput.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_EXPIRED)
						|| (this.OrderInput.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_CANCEL_EXPIRED)))
				{
					return;
				}

				error = ExecRealSalePaidy();
				break;

			case Constants.FLG_PAYMENT_PAYMENT_ID_ATONE:
				error = ExecRealSaleAtone();
				break;

			case Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE:
				error = ExecRealSaleAftee();
				break;

			case Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY:
				error = ExecRealSaleLinePay();
				break;

			case Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY:
				error = ExecRealSaleEcPay();
				break;

			case Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY:
				error = ExecRealSaleNewebPay();
				break;

			case Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY:
				error = ExecRealSalePayPay();
				break;

			case Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF:
				error = ExecRealSaleCsvDef();
				break;

			case Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU:
				error = ExecRealSaleBokuPayment();
				break;

			case Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO:
			case Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE:
				error = OrderCommon.ExecConfirmBillingGmoPost(this.OrderInput.OrderId, this.LoginOperatorName, this.OrderInput.CardTranId);
				break;

			default:
				throw new Exception("未対応の決済：" + hfOrderPaymentKbn.Value);
		}

		if (error == "")
		{
			// Get data input action
			var actionInput = new Hashtable
			{
				{ Constants.TABLE_ORDER, new List<string>() { Constants.FIELD_ORDER_ONLINE_PAYMENT_STATUS } }
			};
			var history = new OrderHistory
			{
				OrderId = this.OrderInput.OrderId,
				Action = OrderHistory.ActionType.EcOrderConfirm,
				OpearatorName = this.LoginOperatorName,
				UpdateAction = actionInput,
			};

			// Begin write history
			history.HistoryBegin();

			// 更新（更新履歴とともに）
			var orderService = new OrderService();

			//オンライン決済ステータス更新
			orderService.UpdateOnlinePaymentStatus(
				this.OrderInput.OrderId,
				successOnlinePaymentStatus,
				this.LoginOperatorName,
				UpdateHistoryAction.Insert);

			// 外部決済ステータス更新
			orderService.UpdateExternalPaymentStatusSalesComplete(
				this.OrderInput.OrderId,
				this.LoginOperatorName,
				UpdateHistoryAction.Insert);

			// Convert Amount Write Payment Memo When Payment With Atone Or Aftee
			var lastBilledAmount = decimal.Parse(this.OrderInput.LastBilledAmount);
			switch (this.OrderInput.OrderPaymentKbn)
			{
				case Constants.FLG_PAYMENT_PAYMENT_ID_ATONE:
				case Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE:
				case Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY:
				case Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY:
				case Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY:
					lastBilledAmount = CurrencyManager.GetSettlementAmount(
						lastBilledAmount,
						decimal.Parse(this.OrderInput.SettlementRate),
						this.OrderInput.SettlementCurrency);
					break;
			}

			// 決済連携メモ更新
			orderService.AddPaymentMemo(
				this.OrderInput.OrderId,
				OrderExternalPaymentMemoHelper.CreateOrderPaymentMemo(
					string.IsNullOrEmpty(this.OrderInput.PaymentOrderId)
						? this.OrderInput.OrderId
						: this.OrderInput.PaymentOrderId,
					this.OrderInput.OrderPaymentKbn,
					this.OrderInput.CardTranId,
					//「売上確定」
					ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_ORDER_CONFIRM,
						Constants.VALUETEXT_PARAM_PAYMENT_MEMO,
						Constants.VALUETEXT_PARAM_SALES_CONFIRMED),
					lastBilledAmount),
				this.LoginOperatorName,
				UpdateHistoryAction.DoNotInsert,
				null);

			// Write history complete
			history.HistoryComplete();

			var externalApiLog = LogCreator.CreateMessage(this.OrderInput.OrderId, string.Empty);
			if (PaygentUtility.CheckIsPaidyPaygentPayment(this.OrderInput.OrderPaymentKbn))
			{
				externalApiLog = string.Format(
					"決済取引ID:{0}・決済注文ID:{1}・{2}円 {3}",
					this.OrderInput.CardTranId,
					this.OrderInput.PaymentOrderId,
					this.OrderInput.LastBilledAmount.ToPriceString(),
					"Paidy決済売上確定");
			}
			// 外部決済連携ログ格納処理
			OrderCommon.AppendExternalPaymentCooperationLog(
				true,
				this.OrderInput.OrderId,
				externalApiLog,
				this.LoginOperatorName,
				UpdateHistoryAction.Insert);
			// 成功したら注文詳細へ
			Response.Redirect(CreateOrderDetailUrl(this.OrderInput.OrderId));
		}
		else
		{
			// 外部決済連携ログ格納処理
			OrderCommon.AppendExternalPaymentCooperationLog(
				false,
				this.OrderInput.OrderId,
				LogCreator.CreateErrorMessage(this.ApiErrorCode, this.ApiErrorMessage),
				this.LoginOperatorName,
				UpdateHistoryAction.Insert);

			trOuterPaymentError.Visible = true;
			lOuterPaymentError.Text = error;
		}
	}

	/// <summary>
	/// 督促ステータス更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateDemandStatus_Click(object sender, System.EventArgs e)
	{
		// 督促ステータス取得
		var demandStatus = rblDemandStatus.SelectedValue;

		// ステータス更新日指定入力チェック・取得
		// 督促ステータスが督促なし以外の場合のみ
		var updateDate = string.Empty;
		if (demandStatus != Constants.FLG_ORDER_DEMAND_STATUS_LEVEL0)
		{
			var errorMessage = CheckInputUpdateDate(
				DateTime.Parse(ucUpdateDemandStatusDate.HfStartDate.Value).Year.ToString(),
				DateTime.Parse(ucUpdateDemandStatusDate.HfStartDate.Value).Month.ToString(),
				DateTime.Parse(ucUpdateDemandStatusDate.HfStartDate.Value).Day.ToString(),
				out updateDate);

			if (errorMessage.Length != 0)
			{
				trDemandStatus.Visible = true;
				lDemandStatus.Text = errorMessage;
				return;
			}
			else
			{
				updateDate = string.Format("{0} {1}",
					updateDate,
					DateTime.Parse(ucUpdateDemandStatusDate.HfStartTime.Value).ToString("HH:mm:ss"));
			}
		}

		// 督促ステータス更新して注文詳細画面へ
		UpdateStatusAndGotoOrderDetail(Constants.StatusType.Demand, demandStatus, updateDate, UpdateHistoryAction.Insert);
	}

	/// <summary>
	/// 返品交換ステータス更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateOrderReturnExchangeStatus_Click(object sender, System.EventArgs e)
	{
		var errorMessages = new StringBuilder();

		// 返品交換ステータス更新日指定入力チェック・取得
		var updateDate = string.Empty;
		var returnExchangeStatus = rblOrderReturnExchangeStatus.SelectedValue;
		if (returnExchangeStatus != Constants.FLG_ORDER_ORDER_RETURN_EXCHANGE_STATUS_UNKNOWN)
		{
			var errorMessage = CheckInputUpdateDate(
				ucOrderReturnExchangeStatusDate.Year,
				ucOrderReturnExchangeStatusDate.Month,
				ucOrderReturnExchangeStatusDate.Day,
				out updateDate);

			errorMessages.Append(errorMessage);
		}

		// 返金ステータスチェック
		// 返品交換区分が「交換」かつ、返品交換ステータスが「返品交換完了」の場合、
		// 返金ステータスが「指定無し」、「返金済み」でなくてはならない。
		// ※交換注文⇒通常注文に更新されると、返金ステータスが更新できなくなるため。
		// 返品交換区分が「交換」かつ、返品交換ステータスが「返品交換完了」の場合
		if ((this.OrderInput.ReturnExchangeKbn == Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE)
			&& (this.OrderInput.ReturnExchangeKbn == Constants.FLG_ORDER_ORDER_RETURN_EXCHANGE_STATUS_COMPLETE))
		{
			// 返金ステータスが「返金無し」、「返金済み」以外の場合
			if ((this.OrderInput.OrderRepaymentStatus != Constants.FLG_ORDER_ORDER_REPAYMENT_STATUS_NOREPAYMENT)
				&& (this.OrderInput.OrderRepaymentStatus != Constants.FLG_ORDER_ORDER_REPAYMENT_STATUS_COMPLETE))
			{
				errorMessages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_ORDER_RETURN_EXCHANGE_STATUS_COMPLETE_ERROR));
			}
		}
		// エラー表示
		if (errorMessages.Length != 0)
		{
			trOrderReturnExchagneStatus.Visible = true;
			lOrderReturnExchangeStatus.Text = errorMessages.ToString();
			return;
		}

		// 返品ステータス更新して注文詳細へ
		UpdateStatusAndGotoOrderDetail(
			Constants.StatusType.RetuenExchange,
			returnExchangeStatus,
			updateDate,
			UpdateHistoryAction.Insert);
	}

	/// <summary>
	/// 返金ステータス更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateOrderRepaymentStatus_Click(object sender, System.EventArgs e)
	{
		// 返金ステータス取得
		var repaymentStatus = rblOrderRepaymentStatus.SelectedValue;

		// ステータス更新日指定入力チェック
		// 返金済みの場合のみ
		var updateDate = String.Empty;
		if (repaymentStatus == Constants.FLG_ORDER_ORDER_REPAYMENT_STATUS_COMPLETE)
		{
			var errorMessage = CheckInputUpdateDate(
				ucOrderRepaymentStatusDate.Year,
				ucOrderRepaymentStatusDate.Month,
				ucOrderRepaymentStatusDate.Day,
				out updateDate);

			if (errorMessage.Length != 0)
			{
				trOrderRepaymentStatus.Visible = true;
				lOrderRepaymentStatus.Text = errorMessage;
				return;
			}
		}

		// 返金ステータス更新して、注文詳細画面へ
		UpdateStatusAndGotoOrderDetail(Constants.StatusType.Repayment, repaymentStatus, updateDate, UpdateHistoryAction.Insert);
	}

	/// <summary>
	/// 編集ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnEdit_Click(object sender, System.EventArgs e)
	{
		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_UPDATE;
		Session[Constants.SESSION_KEY_PARAM_FOR_BACK2] = null;

		// 注文編集画面へ
		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDER_MODIFY_INPUT);
		urlCreator
			.AddParam(Constants.REQUEST_KEY_ORDER_ID, this.OrderInput.OrderId)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_UPDATE);
		var url = urlCreator.CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// 返品交換するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnReturnExchange_Click(object sender, System.EventArgs e)
	{
		if (PaygentUtility.CheckIsPaidyPaygentPayment(this.OrderInput.OrderPaymentKbn)
			&& ((this.OrderInput.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_NONE)
				|| (this.OrderInput.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED)
				|| (this.OrderInput.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP)))
		{
			lMessages.Visible = true;
			lMessages.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PAYMENT_PAIDY_PAYGENT_CANNOT_RETURN_EXCHANGE_BEFORE_SALE);
			return;
		}

		// セッションクリア
		Session[Constants.SESSION_KEY_ORDER_REGIST_INPUT_ERROR] = "";

		// 注文返品交換画面へ
		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDER_RETRUN_EXCHANGE_INPUT);
		urlCreator
			.AddParam(Constants.REQUEST_KEY_ORDER_ID, this.OrderInput.OrderId);
		var url = urlCreator.CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// リピーター：拡張ステータス更新ボタンクリック
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rExtendStatus_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		// 拡張ステータス更新ボタン
		if (e.CommandName == "update")
		{
			// コントロールの取得
			var lExtendNo = (Literal)((RepeaterItem)e.Item).FindControl("lExtendNo");
			var rblExtend = (RadioButtonList)((RepeaterItem)e.Item).FindControl("rblExtend");
			var ucDate = (DateTimePickerPeriodInputControl)((RepeaterItem)e.Item).FindControl("ucExtendStatusDate");

			// 入力チェック
			var date = string.Empty;
			var errorMessage = string.Empty;
			if (string.IsNullOrEmpty(ucDate.HfStartDate.Value) == false)
			{
				errorMessage = CheckInputUpdateDate(
					DateTime.Parse(ucDate.HfStartDate.Value).Year.ToString(),
					DateTime.Parse(ucDate.HfStartDate.Value).Month.ToString(),
					DateTime.Parse(ucDate.HfStartDate.Value).Day.ToString(),
					out date);

			if (errorMessage.Length != 0)
			{
				trExtendStatus.Visible = true;
				lExtendStatus.Text = errorMessage;
				return;
			}
				else
				{
					date = string.Format("{0} {1}",
						date,
						DateTime.Parse(ucDate.HfStartTime.Value).ToString("HH:mm:ss"));
				}
			}

			// 拡張ステータス更新
			// 入力値が有効なときのみ
			int iNo;
			if (int.TryParse(lExtendNo.Text, out iNo))
			{
				// Get data input action
				var actionInput = new Hashtable
				{
					{ Constants.TABLE_ORDER, new List<string>() { FIELD_ORDER_EXTEND_STATUS + iNo.ToString(), FIELD_ORDER_EXTEND_STATUS_DATE + iNo.ToString() }}
				};

				var history = new OrderHistory
				{
					OrderId = this.OrderInput.OrderId,
					Action = OrderHistory.ActionType.EcOrderConfirm,
					OpearatorName = this.LoginOperatorName,
					UpdateAction = actionInput,
				};

				//Begin write history
				history.HistoryBegin();

				// 注文拡張ステータス更新
				new OrderService().UpdateOrderExtendStatus(
					this.OrderInput.OrderId,
					iNo,
					rblExtend.SelectedValue,
					DateTime.Parse(date),
					this.LoginOperatorName,
					UpdateHistoryAction.Insert);

				// Write history complete
				history.HistoryComplete();
			}

			// 注文詳細へ
			Response.Redirect(CreateOrderDetailUrl(this.OrderInput.OrderId));
		}
	}

	/// <summary>
	/// 外部連携取込ステータス更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateExternalImportStatus_Click(object sender, EventArgs e)
	{
		var actionInput = new Hashtable
		{
			{ Constants.TABLE_ORDER, new List<string>() { Constants.FIELD_ORDER_EXTERNAL_IMPORT_STATUS } }
		};

		var history = new OrderHistory
		{
			OrderId = this.OrderInput.OrderId,
			Action = OrderHistory.ActionType.EcOrderConfirm,
			OpearatorName = this.LoginOperatorName,
			UpdateAction = actionInput
		};

		history.HistoryBegin();

		// 更新（更新履歴とともに）
		new OrderService().UpdateExternalOrderImportStatus(
			this.OrderInput.OrderId,
			rblExternalImportStatus.SelectedValue,
			this.LoginOperatorName,
			UpdateHistoryAction.Insert);

		history.HistoryComplete();

		Response.Redirect(CreateOrderDetailUrl(this.OrderInput.OrderId));
	}

	/// <summary>
	/// モール連携ステータス更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateMallLinkStatus_Click(object sender, EventArgs e)
	{
		var actionInput = new Hashtable
		{
			{ Constants.TABLE_ORDER, new List<string>() { Constants.FIELD_ORDER_MALL_LINK_STATUS } }
		};

		var history = new OrderHistory
		{
			OrderId = this.OrderInput.OrderId,
			Action = OrderHistory.ActionType.EcOrderConfirm,
			OpearatorName = this.LoginOperatorName,
			UpdateAction = actionInput
		};

		// 更新（更新履歴とともに）
		new OrderService().UpdateMallLinkStatus(
			this.OrderInput.OrderId,
			rblMallLinkStatus.SelectedValue,
			this.LoginOperatorName,
			UpdateHistoryAction.Insert);

		history.HistoryComplete();
		Response.Redirect(CreateOrderDetailUrl(this.OrderInput.OrderId));
	}

	/// <summary>
	/// Click update Gooddeal delivery status button
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateGooddealDeliveryStatus_Click(object sender, EventArgs e)
	{
		var actionInput = new Hashtable
		{
			{ Constants.TABLE_ORDER, new List<string> { Constants.FIELD_ORDER_LOGI_COOPERATION_STATUS } }
		};
		var history = new OrderHistory
		{
			OrderId = this.OrderInput.OrderId,
			Action = OrderHistory.ActionType.EcOrderConfirm,
			OpearatorName = this.LoginOperatorName,
			UpdateAction = actionInput
		};

		history.HistoryBegin();

		// Register shipping delivery to Gooddeal
		var timestamp = GooddealUtility.GetTimestamp();
		var request = GooddealUtility.CreateRegisterShippingDeliveryRequest(this.OrderInput.CreateModel(), timestamp);
		var response = GooddealApi.RegisterShippingDelivery(request, timestamp);
		var gooddealDeliveryStatus = response.IsRegisterSucceeded
			? Constants.FLG_ORDER_LOGI_COOPERATION_STATUS_COMPLETE
			: Constants.FLG_ORDER_LOGI_COOPERATION_STATUS_ERROR;

		// Update Gooddeal delivery status
		new OrderService().UpdateLogiCooperationStatus(
			this.OrderInput.OrderId,
			response.IsRegisterSucceeded
				? Constants.FLG_ORDER_ORDER_STATUS_ORDER_RECOGNIZED
				: this.OrderInput.OrderStatus,
			gooddealDeliveryStatus,
			this.LoginOperatorName,
			UpdateHistoryAction.Insert,
			null);

		history.HistoryComplete();

		if (response.IsRegisterSucceeded == false)
		{
			rblGooddealDeliveryStatus.SelectedValue = gooddealDeliveryStatus;
			lErrorGooddealDeliveryStatus.Text = WebSanitizer.HtmlEncode(response.GetApiMessage());
			trErrorGooddealDeliveryStatus.Visible = true;
			return;
		}

		// Update order status and redirect to order detail
		trErrorGooddealDeliveryStatus.Visible = false;
		UpdateStatusAndGotoOrderDetail(
			Constants.StatusType.Order,
			Constants.FLG_ORDER_ORDER_STATUS_ORDER_RECOGNIZED,
			StringUtility.ToDateString(DateTime.Now, "yyyyMMdd"),
			UpdateHistoryAction.Insert);
	}

	/// <summary>
	/// 一覧へ戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBackList_Click(object sender, System.EventArgs e)
	{
		Response.Redirect(
			CreateOrderListUrl(
				(Hashtable)Session[Constants.SESSIONPARAM_KEY_ORDER_SEARCH_INFO],
				true));
	}

	/// <summary>
	/// 再注文ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnReOrder_Click(object sender, EventArgs e)
	{
		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDER_REGIST_INPUT);
		urlCreator.AddParam(Constants.REQUEST_KEY_REORDER_ID, this.OrderInput.OrderId);
		var url = urlCreator.CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// External Order Info Action Click
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnExternalOrderInfoAction_Click(object sender, EventArgs e)
	{
		var actionInput = new Hashtable
		{
			{ Constants.TABLE_ORDER, new List<string>() { Constants.FIELD_ORDER_ONLINE_DELIVERY_STATUS } }
		};
		var history = new OrderHistory
		{
			OrderId = this.OrderInput.OrderId,
			Action = OrderHistory.ActionType.EcOrderConfirm,
			OpearatorName = this.LoginOperatorName,
			UpdateAction = actionInput,
		};

		history.HistoryBegin();

		var orderInfo = new OrderService().GetOrderInfoByOrderId(this.OrderInput.OrderId);
		var apiMessage = string.Empty;
		var alertMessage = string.Empty;
		var isSuccess = (this.OrderInput.IsReturnOrder)
			? ExecExternalForOrderReturnInfo(orderInfo, out apiMessage)
			: ExecExternalForOrderInfo( orderInfo, out apiMessage, out alertMessage);

		PaymentFileLogger.WritePaymentLog(
			isSuccess,
			this.OrderInput.PaymentName ?? string.Empty,
			PaymentFileLogger.PaymentType.Unknown,
			PaymentFileLogger.PaymentProcessingType.OrderInfoUpdate,
			apiMessage,
			new Dictionary<string, string>
			{
				{ Constants.FIELD_ORDER_ORDER_ID, this.OrderInput.OrderId }
			});

		if (isSuccess == false)
		{
			lbErrorExternalOrderInfo.Text = string.IsNullOrEmpty(alertMessage)
				? apiMessage
				: alertMessage;
			trErrorExternalOrderInfo.Visible = true;
			return;
		}

		history.HistoryComplete();

		Response.Redirect(CreateOrderDetailUrl(this.OrderInput.OrderId));
	}

	/// <summary>
	/// Gmo後払い入金状態確認ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCheckOrderPaymentStatus_Click(object sender, EventArgs e)
	{
		// Gmo後払い決済
		var request = new GmoRequestGetDefPaymentStatus(this.OrderInput.CardTranId);
		var result = new GmoDeferredApiFacade().GetDefPaymentStatus(request);
		if (result == null)
		{
			SetErrorPaymentStatus();
			return;
		}

		if (result.Result == ResultCode.NG)
		{
			if ((result.Errors != null)
				&& result.Errors.Error.Any(item => (string.IsNullOrEmpty(item.ErrorMessage) == false)))
			{
				SetErrorPaymentStatus(
					string.Format(
					"{0}：{1}",
					ValueText.GetValueText(
						Constants.TABLE_ORDER,
						Constants.VALUETEXT_ORDER_PAYMENT_MESSAGE_KBN,
						Constants.VALUETEXT_ORDER_PAYMENT_MESSAGE_KBN_MESSAGE_KBN_MSG_ERROR),
					result.Errors.Error
						.Select(item => item.ErrorMessage)
						.JoinToString("\n")));
				return;
			}

			// APIパラメータエラー
			SetErrorPaymentStatus();
			return;
		}
		CheckPaymentStatusCode(result);
	}

	/// <summary>
	/// 入金状態コードを確認をする
	/// </summary>
	/// <param name="value">入金状態確認のレスポンス値</param>
	private void CheckPaymentStatusCode(GmoResponseGetDefPaymentStatus value)
	{
		switch (value.TransactionResult.PaymentStatus)
		{
			case PaymentStatusCode.NOT:
				SetPaymentStatus(
					ValueText.GetValueText(
						Constants.TABLE_ORDER,
						Constants.VALUETEXT_ORDER_PAYMENT_STATUS_MESSAGE_KBN,
						Constants.VALUETEXT_ORDER_PAYMENT_STATUS_MESSAGE_KBN_MSG_NOT_PAYMENT));
				break;

			case PaymentStatusCode.DEFINITE:
			case PaymentStatusCode.PROMPT:
				SetPaymentStatus(
					ValueText.GetValueText(
						Constants.TABLE_ORDER,
						Constants.VALUETEXT_ORDER_PAYMENT_STATUS_MESSAGE_KBN,
						Constants.VALUETEXT_ORDER_PAYMENT_STATUS_MESSAGE_KBN_MSG_DEPOSITED),
					string.Format(
						"{0}：{1}",
						ValueText.GetValueText(
							Constants.TABLE_ORDER,
							Constants.VALUETEXT_ORDER_PAYMENT_MESSAGE_KBN,
							Constants.VALUETEXT_ORDER_PAYMENT_MESSAGE_KBN_MESSAGE_KBN_MSG_PAYMENT_DAY),
						value.TransactionResult.PromptDate));
				break;

			case PaymentStatusCode.IRREGULAR:
				SetErrorPaymentStatus(
					WebMessages.GetMessages(
						WebMessages.ERRMSG_MANAGER_CVS_GMO_GET_PAYMENT_STATUS_UNCLEAR_DEPOSITED));
				break;

			default:
				// APIパラメータエラー
				SetErrorPaymentStatus();
				break;
		}
	}

	/// <summary>
	/// Gmo後払いAPI取得処理がOKな場合
	/// 入金状態情報のセット
	/// </summary>
	/// <param name="paymentState">入金状態</param>
	/// <param name="paymentDate">入金日(速報)</param>
	private void SetPaymentStatus(string paymentState, string paymentDate = null)
	{
		lOrderPaymentState.Text = WebSanitizer.HtmlEncode(paymentState);
		lOrderPaymentDate.Text = WebSanitizer.HtmlEncode(paymentDate);
		spErrorPaymentStatus.Visible = false;
	}

	/// <summary>
	/// Gmo後払いAPI取得処理がNGな場合
	/// エラーメッセージのセット
	/// </summary>
	/// <param name="errorMessage">エラーメッセージ</param>
	private void SetErrorPaymentStatus(string errorMessage = null)
	{
		// Reset display payment status
		SetPaymentStatus(string.Empty);

		// Set and show display error message
		spErrorPaymentStatus.Visible = true;
		if (string.IsNullOrEmpty(errorMessage))
		{
			lOrderPatmentErrorMessage.Text = WebSanitizer.HtmlEncode(
				WebMessages.GetMessages(
					WebMessages.ERRMSG_MANAGER_CVS_GMO_GET_PAYMENT_STATUS_FAILED));
			return;
		}
		lOrderPatmentErrorMessage.Text = WebSanitizer.HtmlEncodeChangeToBr(errorMessage);
	}

	/// <summary>
	/// Can display get external order payment status
	/// </summary>
	/// <param name="orderPaymentKbn">Order payment kbn</param>
	/// <returns>True: Show diplay for get external order payment status</returns>
	protected bool CanDisplayGetExternalOrderPaymentStatus(string orderPaymentKbn)
	{
		var result = ((orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
			&& (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Gmo)
			&& Constants.PAYMENT_SETTING_GMO_GETDEFPAYMENTSTATUS_OPTION);
		return result;
	}

	/// <summary>
	/// Execute External For Order Information
	/// </summary>
	/// <param name="orderInfo">Order Information</param>
	/// <param name="apiMessage">Api Message</param>
	/// <param name="alertMessage">Alert Message</param>
	/// <returns>True: External order is success, otherwise: false</returns>
	protected bool ExecExternalForOrderInfo(
		OrderModel orderInfo,
		out string apiMessage,
		out string alertMessage)
	{
		var request = ECPayUtility.CreateRequestDataForSendOrder(orderInfo, dontNotifyUpdate: true);
		var response = ECPayApiFacade.SendOrderInfo(request);
		alertMessage = string.Empty;
		if (response.IsExecRegisterApiSuccess == false)
		{
			apiMessage = response.ErrorMessage;
			return false;
		}
		var isReceivingStore = (orderInfo.Shippings[0].DeliveryCompanyId == Constants.TWPELICANEXPRESS_CONVENIENCE_STORE_ID);
		var orderId = response.GetResponseValue(ECPayConstants.PARAM_MERCHANT_TRADE_NO);
		var responseDeliveryTranId = response.GetResponseValue(ECPayConstants.PARAM_ALL_PAY_LOGISTICS_ID);
		var responseRtnCode = response.GetResponseValue(ECPayConstants.PARAM_RTN_CODE);
		var responseRtnMsg = response.GetResponseValue(ECPayConstants.PARAM_RTN_MSG);
		var responseStatusUpdateDate = response.GetResponseValue(ECPayConstants.PARAM_UPDATE_STATUS_DATE);
		var responseCvsPaymentNo = response.GetResponseValue(ECPayConstants.PARAM_CVS_PAYMENT_NO);
		var responseCvsValidationNo = response.GetResponseValue(ECPayConstants.PARAM_CVS_VALIDATION_NO);
		var responseBookingNote = response.GetResponseValue(ECPayConstants.PARAM_BOOKING_NOTE);

		var relationMemo = ECPayUtility.CreateRelationMemo(
			responseRtnCode,
			responseRtnMsg,
			string.Empty);
		var logisticsSubType = (request.LogisticsSubType == Constants.FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_SUB_TYPE_7_ELEVEN_C2C)
			? request.LogisticsSubType
			: request.LogisticsSubType.Replace(ECPayConstants.CONST_CVS_LOGISTIC_METHOD_C2C, string.Empty);
		var shippingStatusConvert = ValueText.GetValueText(
			Constants.TABLE_ORDERSHIPPING,
			string.Format("{0}_{1}",
				Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS_CONVERT,
				logisticsSubType),
			responseRtnCode);
		var shippingCheckNo = (ECPayUtility.CheckLogisticsSubType7ElevenAndC2CFamilyMart(orderInfo.Shippings[0].ShippingReceivingStoreType))
			? string.Format("{0}{1}", responseCvsPaymentNo, responseCvsValidationNo)
			: (isReceivingStore == false)
				? responseBookingNote
				: string.Empty;

		var orderShipping = orderInfo.Shippings[0];
		orderShipping.ShippingExternalDelivertyStatus = responseRtnCode;
		orderShipping.ShippingStatus = shippingStatusConvert;
		orderShipping.ShippingCheckNo = shippingCheckNo;
		var isShippingStatusAbnormal = (shippingStatusConvert == Constants.FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_STATUS_ABNORMAL);
		if (isShippingStatusAbnormal == false)
		{
			orderShipping.ShippingStatusUpdateDate = (string.IsNullOrEmpty(responseStatusUpdateDate) == false)
				? DateTime.Parse(responseStatusUpdateDate)
				: (DateTime?)null;
		}

		using (var accessor = new SqlAccessor())
		{
			accessor.OpenConnection();
			accessor.BeginTransaction();

			var orderService = new OrderService();

			orderService.UpdateDeliveryTransactionIdAndRelationMemo(
				orderId,
				responseDeliveryTranId,
				relationMemo,
				this.LoginOperatorName,
				UpdateHistoryAction.DoNotInsert,
				accessor);
			if (isShippingStatusAbnormal == false)
			{
				orderService.UpdateOnlineDeliveryStatus(
					orderId,
					Constants.FLG_ORDER_ONLINE_DELIVERY_STATUS_SETTLED,
					this.LoginOperatorName,
					UpdateHistoryAction.DoNotInsert,
					accessor);
			}

			orderService.UpdateOrderShipping(orderShipping, accessor);
			accessor.CommitTransaction();
		}

		alertMessage = isShippingStatusAbnormal ? relationMemo : string.Empty;
		apiMessage = ECPayUtility.CreateLogMessageForSendOrderRegister(
			orderInfo.OrderId,
			responseDeliveryTranId,
			responseRtnCode,
			responseRtnMsg);
		return true;
	}

	/// <summary>
	/// Execute External For Order Return Information
	/// </summary>
	/// <param name="orderInfo">Order Information</param>
	/// <param name="apiMessage">Api Message</param>
	/// <returns>True: External order is success, otherwise: false</returns>
	protected bool ExecExternalForOrderReturnInfo(OrderModel orderInfo, out string apiMessage)
	{
		// Get order old information
		var orderService = new OrderService();
		var orderOld = orderService.GetOrderInfoByOrderId(orderInfo.OrderIdOrg);
		var shippingType = orderInfo.Shippings[0].ShippingReceivingStoreType;
		var isReceivingStore = (orderInfo.Shippings[0].DeliveryCompanyId == Constants.TWPELICANEXPRESS_CONVENIENCE_STORE_ID);
		var isFamilyMart = ((shippingType == Constants.FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_FAMILY_MART_PAYMENT)
			|| (shippingType == Constants.FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_FAMILY_MART));

		ECPayConvenienceStoreRequest request;
		ECPayResponseResult response;
		if (isReceivingStore)
		{
			request = ECPayUtility.CreateRequestDataForSendReturnAtConvenienceStore(
				orderInfo,
				orderOld.DeliveryTranId);
			response = ECPayApiFacade.SendReturnAtConvenienceStoreInfo(request, isFamilyMart);
			if (response.IsExecReturnCsvApiSuccess == false)
			{
				apiMessage = response.ErrorMessage;
				return false;
			}

			var responseReturnOrderNo = response.GetResponseValue(ECPayConstants.PARAM_RTN_ORDER_NO);
			var responseReturnMerchantTradeNo = response.GetResponseValue(ECPayConstants.PARAM_RTN_MERCHANT_TRADE_NO);
			var relationMemo = ECPayUtility.CreateRelationMemo(
				string.Empty,
				string.Empty,
				responseReturnOrderNo,
				true,
				isFamilyMart);
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				orderService.UpdateDeliveryTransactionIdAndRelationMemo(
					orderInfo.OrderId,
					responseReturnMerchantTradeNo,
					relationMemo,
					this.LoginOperatorName,
					UpdateHistoryAction.DoNotInsert,
					accessor);

				orderService.UpdateOnlineDeliveryStatus(
					orderInfo.OrderId,
					Constants.FLG_ORDER_ONLINE_DELIVERY_STATUS_SETTLED,
					this.LoginOperatorName,
					UpdateHistoryAction.DoNotInsert,
					accessor);

				accessor.CommitTransaction();
			}

			apiMessage = ECPayUtility.CreateLogMessageForSendOrderReturnCVS(
				orderInfo.OrderId,
				responseReturnMerchantTradeNo,
				responseReturnOrderNo,
				isFamilyMart);
			return true;
		}

		request = ECPayUtility.CreateRequestDataForOrderReturnHome(orderInfo, orderOld.DeliveryTranId);
		response = ECPayApiFacade.SendReturnAtHomeInfo(request);
		apiMessage = ECPayUtility.CreateLogMessageForSendOrderReturnHome(
			orderInfo.OrderId,
			orderOld.DeliveryTranId,
			response.ErrorMessage,
			response.IsExecReturnHomeApiSuccess);
		if (response.IsExecReturnHomeApiSuccess)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				orderService.UpdateOnlineDeliveryStatus(
					orderInfo.OrderId,
					Constants.FLG_ORDER_ONLINE_DELIVERY_STATUS_SETTLED,
					this.LoginOperatorName,
					UpdateHistoryAction.DoNotInsert,
					accessor);

				accessor.CommitTransaction();
			}
		}
		return response.IsExecReturnHomeApiSuccess;
	}

	/// <summary>
	/// Cvs def invoice reissue click
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCvsDefInvoiceReissue_Click(object sender, EventArgs e)
	{
		var request = new GMOReissue.GmoRequestReissue(this.OrderInput.CreateModel());
		var response = new GmoDeferredApiFacade().Reissue(request);

		if (response.IsSuccess == false)
		{
			var message = string.Format(
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CSVDEF_INVOICE_REISSUE_FAIL),
				response.GetErrorMessage());
			Session[Constants.SESSION_KEY_ERROR_MSG] = message;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// Be sure to turn off the extended status after output
		new OrderService().UpdateReissueInvoiceStatusAndPaymentMemo(
			this.OrderInput.OrderId,
			int.Parse(Constants.FLG_ORDERWORKFLOWSETTING_EXTERNAL_PAYMENT_ACTION_GMO_CVS_DEF_INVOICE_REISSUE),
			response.GetOrderPaymentMemoForReissue(this.OrderInput.PaymentOrderId),
			this.LoginOperatorName,
			UpdateHistoryAction.Insert);

		this.ActionCompleteMessage = WebMessages.GetMessages(
			WebMessages.ERRMSG_MANAGER_ORDER_CSVDEF_INVOICE_REISSUE_SUCCESS).Replace("@@ 1 @@", "GMO");
		Response.Redirect(CreateOrderDetailUrl(this.OrderInput.OrderId));
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
	/// 各プロパティセット
	/// </summary>
	private void SetProperty()
	{
		// 注文情報セット
		SetOrder();

		// 関連注文情報セット
		SetRelatedOrder();

		// ユーザークレジットカード情報セット
		SetUserCreditCardInfo();
	}

	/// <summary>
	/// 注文情報セット
	/// </summary>
	private void SetOrder()
	{
		// 注文情報取得
		var order = new OrderService().GetOrderInfoByOrderId(this.RequestOrderId);
		// 注文情報が存在しない？
		if (order == null)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 外部決済連携ログがあった場合、末尾の改行文字列を削除
		if (string.IsNullOrEmpty(order.ExternalPaymentCooperationLog) == false)
		{
			order.ExternalPaymentCooperationLog = order.ExternalPaymentCooperationLog.TrimEnd('\r', '\n');
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
					order = new OrderService().GetOrderInfoByOrderId(this.RequestOrderId);
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
		var orders = new OrderService().GetRelatedOrders(orderIdOrg);
		rRelatedOrder.DataSource = orders.Select(o => new OrderInput(o)).ToArray();
		rRelatedOrder.DataBind();
	}

	/// <summary>
	/// ユーザークレジットカード情報セット
	/// </summary>
	private void SetUserCreditCardInfo()
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
		// 各ラジオボタン・ドロップダウン作成
		// 「注文ステータス」作成
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_STATUS))
		{
			if (li.Value == Constants.FLG_ORDER_ORDER_STATUS_UNKNOWN)
			{
				continue;
			}
			// 実在庫を利用しない場合、「在庫引当済み」は追加しない
			if ((Constants.REALSTOCK_OPTION_ENABLED == false)
				&& (li.Value == Constants.FLG_ORDER_ORDER_STATUS_STOCK_RESERVED))
			{
				continue;
			}

			li.Selected = (this.OrderInput.OrderStatus == li.Value);
			rblOrderStatus.Items.Add(li);
		}
		hfOrderStatus.Value = this.OrderInput.OrderStatus;

		// 「商品実在庫更新」作成
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDERWORKFLOWSETTING_PRODUCT_REALSTOCK_CHANGE))
		{
			// 在庫「引当済み」以外の場合、「出荷処理（実在庫）を実行 」は選択不可にする
			if ((this.OrderInput.OrderStockreservedStatus != Constants.FLG_ORDER_ORDER_STOCKRESERVED_STATUS_RESERVED)
				&& (li.Value == Constants.FLG_ORDERWORKFLOWSETTING_PRODUCT_REALSTOCK_CHANGE_FORWARD_STCOK))
			{
				li.Enabled = false;
			}
			rblProductRealStockChange.Items.Add(li);
		}

		// 「入金ステータス更新」作成
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS))
		{
			if (li.Value == Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_UNKNOWN)
			{
				continue;
			}

			li.Selected = (li.Value == this.OrderInput.OrderPaymentStatus);
			rblOrderPaymentStatus.Items.Add(li);
		}

		// 「返品交換ステータス更新」作成
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_STATUS))
		{
			if (li.Value == Constants.FLG_ORDER_ORDER_RETURN_EXCHANGE_STATUS_UNKNOWN)
			{
				continue;
			}

			li.Selected = (li.Value == this.OrderInput.OrderReturnExchangeStatus);
			rblOrderReturnExchangeStatus.Items.Add(li);
		}

		// 「返金ステータス更新」作成
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_REPAYMENT_STATUS))
		{
			if (li.Value == Constants.FLG_ORDER_ORDER_REPAYMENT_STATUS_UNKNOWN)
			{
				continue;
			}

			li.Selected = (li.Value == this.OrderInput.OrderRepaymentStatus);
			rblOrderRepaymentStatus.Items.Add(li);
		}

		// 「督促ステータス更新」作成
		if (Constants.DEMAND_OPTION_ENABLE)
		{
			foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_DEMAND_STATUS))
			{
				if (li.Value == Constants.FLG_ORDER_DEMAND_STATUS_UNKNOWN)
				{
					continue;
				}

				li.Selected = (li.Value == this.OrderInput.DemandStatus);
				rblDemandStatus.Items.Add(li);
			}
		}
		
		// 「外部連携ステータス更新」作成
		// 外部連携注文かつ外部連携取込ステータスが空欄(=注文登録で手動登録している)の場合は外部連携取込ステータスの更新不可
		rblExternalImportStatus.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ORDER, Constants.FIELD_ORDER_EXTERNAL_IMPORT_STATUS));
		var canUpdateExternalImportStatus = ((Constants.FLG_USER_MALL_ID_EXTERNAL_ORDER_SITES.Any(site => site != this.OrderInput.MallId))
			|| (string.IsNullOrEmpty(this.OrderInput.ExternalImportStatus) == false));
		rblExternalImportStatus.Enabled = btnUpdateExternalImportStatus.Enabled = canUpdateExternalImportStatus;

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
		ddlOrderMailId.Items.Add(new ListItem("", ""));
		ddlOrderMailId.Items.AddRange(
			GetMailTemplateUtility.GetMailTemplateForOrder(this.LoginOperatorShopId).Select(mail => new ListItem(mail.MailName, mail.MailId)).ToArray());

		if (OrderCommon.DisplayTwInvoiceInfo())
		{
			// Get TwOrderInvoice
			var twOrderInvoice = new TwOrderInvoiceService().GetOrderInvoice(
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
		rblStorePickupStatus.Items.AddRange(
			ValueText.GetValueItemArray(
				Constants.TABLE_ORDER,
				Constants.FIELD_ORDER_STOREPICKUP_STATUS));
		rblStorePickupStatus.SelectedValue = this.OrderInput.StorePickupStatus;

		// 各種表示・非表示制御
		// 注文ステータスが出荷完了、配送完了、キャンセル、もしくはAmazonから取り込んだ注文の場合、編集ボタンを無効化
		if ((this.OrderInput.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP)
			|| (this.OrderInput.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_DELIVERY_COMP)
			|| (this.OrderInput.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED)
			|| (this.OrderInput.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_TEMP_CANCELED)
			|| (this.OrderInput.MallKbn == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_AMAZON)
			|| (OrderCommon.DisplayTwInvoiceInfo()
				&& OrderCommon.TwInvoiceStatusCanNotEditOrder(this.TwOrderInvoice, Constants.TWINVOICE_ENABLED)))
		{
			btnEditTop.Enabled = false;
			btnEditBottom.Enabled = false;
		}

		// 出荷ステータス「出荷済み」の場合、商品実在庫変更を実行不可にし、「出荷処理（実在庫）を実行 」を選択状態にする
		if (this.OrderInput.OrderShippedStatus == Constants.FLG_ORDER_ORDER_SHIPPED_STATUS_SHIPPED)
		{
			rblProductRealStockChange.Enabled = false;
			btnUpdateProductRealStock.Enabled = false;

			rblProductRealStockChange.SelectedValue = Constants.FLG_ORDERWORKFLOWSETTING_PRODUCT_REALSTOCK_CHANGE_FORWARD_STCOK;
		}
		// それ以外は「引当処理（実在庫）を実行」を選択状態にする
		else
		{
			rblProductRealStockChange.SelectedValue = Constants.FLG_ORDERWORKFLOWSETTING_PRODUCT_REALSTOCK_CHANGE_RESERVED_STCOK;
		}

		// 「再注文する」ボタン非表示／表示設定
		//  ■再注文ボタン非表示条件
		//  注文登録メニューの権限が持っていない場合、又は、ポップアップ画面
		//  又は、以下のいずれかの注文
		//  ・セット商品を購入した注文
		//  ・返品／交換注文
		//  ・定期購入のみの商品を購入した注文
		var orderItems = this.OrderInput.Shippings.SelectMany(s => s.Items).ToArray();
		var isFixedPurchaseOnly = false;
		foreach (var orderItem in orderItems)
		{
			var product = ProductCommon.GetProductVariationInfo(orderItem.ShopId, orderItem.ProductId, orderItem.VariationId, string.Empty);
			if ((product.Count != 0)
				&& (product[0][Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG].ToString() == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_ONLY))
			{
				isFixedPurchaseOnly = true;
				break;
			}
		}
		// ログインオペレータメニュ―に新規注文登録があるかで権限を判定
		var hasAuthorityToUseOrderRegistInput = this.LoginOperatorMenu.Exists(ml => (ml.SmallMenus.Any(ms => (Constants.PAGE_MANAGER_ORDER_REGIST_INPUT.IndexOf(ms.MenuPath) >= 0))));

		if ((this.OrderInput.ReturnExchangeKbn != Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_UNKNOWN)
			|| orderItems.Any(orderItem => orderItem.IsProductSet)
			|| isFixedPurchaseOnly
			|| (hasAuthorityToUseOrderRegistInput == false)
			|| (this.IsPopUp))
		{
			btnReOrderTop.Visible = false;
			btnReOrderBottom.Visible = false;
		}
		else
		{
			btnReOrderTop.Visible = true;
			btnReOrderBottom.Visible = true;
		}

		// Amazonからの注文の場合、「再注文する」ボタンを非活性にする
		if (this.OrderInput.MallKbn == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_AMAZON)
		{
			btnReOrderTop.Enabled = btnReOrderBottom.Enabled = false;
		}

		ucCardRealSalesDate.Disabled = (this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT);

		if ((MallOptionUtility.CheckMallKbn(this.OrderInput.ShopId, this.OrderInput.MallId) != MallOptionUtility.MallKbn.OwnSite)
			|| (this.OrderInput.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
			|| (this.OrderInput.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_CANCELED)
			|| ((this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL) && (Constants.PAYPAL_PAYMENT_METHOD == w2.App.Common.Constants.PayPalPaymentMethod.AUTH_WITH_SUBMIT))
			|| ((this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY) && (this.OrderInput.LastBilledAmount == "0")))
		{
			btnCardRealSales.Enabled = btnExternalPayment.Enabled = false;
		}

		if (((this.OrderInput.Shippings.Any(shipping => ((shipping.ShippingReceivingStoreType == Constants.FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_HI_LIFE)
				|| (shipping.ShippingReceivingStoreType == Constants.FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_HI_LIFE_PAYMENT))))
				&& this.OrderInput.IsReturnOrder)
			|| (this.OrderInput.OnlineDeliveryStatus == Constants.FLG_ORDER_ONLINE_DELIVERY_STATUS_SETTLED))
		{
			btnExternalOrderInfoAction.Enabled = false;
		}

		if ((Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED == false)
			|| this.OrderInput.Shippings.Any(shipping => ((shipping.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF)
				&& (ECPayUtility.IsShippingServiceOfECPay(shipping.DeliveryCompanyId) == false)))
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
			// 注文ステータスが出荷完了、配送完了の場合のみ返品交換ボタン表示
			if ((this.OrderInput.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP)
				|| (this.OrderInput.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_DELIVERY_COMP))
			{
				btnReturnExchangeTop.Visible = true;
				btnReturnExchangeBottom.Visible = true;

				if (OrderCommon.DisplayTwInvoiceInfo()
					&& (this.TwOrderInvoice != null)
					&& (this.TwOrderInvoice.TwInvoiceStatus != Constants.FLG_ORDER_INVOICE_STATUS_ISSUED_LINKED))
				{
					btnReturnExchangeTop.Enabled = false;
					btnReturnExchangeBottom.Enabled = false;
				}
			}
			else
			{
				btnReturnExchangeTop.Visible = false;
				btnReturnExchangeBottom.Visible = false;
			}
			// 注文、実在庫、入金、督促ステータス表示
			divOrderStatus.Visible = true;
			divRealStock.Visible = true;
			divOrderPaymentStauts.Visible = true;
			divDemandStatus.Visible = ((this.OrderInput.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)
				&& (this.OrderInput.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE));
			// 返品交換、返金ステータス非表示
			divOrderReturnExchangeStatus.Visible = false;
			divOrderRepaymentStatus.Visible = false;
		}
		// 交換注文かつ注文ステータスが指定無しの場合の制御
		// ※交換注文情報の場合、出荷処理を行う必要があるため、元注文情報と同様に表示する必要有り。
		else if ((this.OrderInput.ReturnExchangeKbn == Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE)
			&& (this.OrderInput.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_UNKNOWN))
		{
			// 返品交換ボタン非表示
			btnReturnExchangeTop.Visible = false;
			btnReturnExchangeBottom.Visible = false;
			// 各領域の表示非表示制御
			divOrderStatus.Visible = true;
			divRealStock.Visible = true;
			divOrderPaymentStauts.Visible = true;
			divDemandStatus.Visible = ((this.OrderInput.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)
				&& (this.OrderInput.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE));
			// 返品交換、返金ステータス非表示
			divOrderReturnExchangeStatus.Visible = false;
			divOrderRepaymentStatus.Visible = false;
		}
		// 返品交換注文の場合
		else if ((this.OrderInput.ReturnExchangeKbn == Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN)
			|| (this.OrderInput.ReturnExchangeKbn == Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE))
		{
			// 返品交換ボタン、編集ボタン非表示
			btnReturnExchangeTop.Visible = false;
			btnReturnExchangeBottom.Visible = false;
			btnEditTop.Visible = false;
			btnEditBottom.Visible = false;
			// 注文、実在庫、入金、督促ステータス非表示
			divOrderStatus.Visible = false;
			divRealStock.Visible = false;
			divOrderPaymentStauts.Visible = false;
			divDemandStatus.Visible = false;
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
			btnExternalPayment.Enabled = (getTrans.Status != Statuses.Captured);
		}

		if ((this.OrderInput.ExternalPaymentStatus != Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP)
			&& PaygentUtility.CheckIsPaidyPaygentPayment(this.OrderInput.OrderPaymentKbn))
		{
			btnExternalPayment.Enabled = (this.OrderInput.ExternalPaymentStatus != Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_EXPIRED)
				&& (this.OrderInput.ExternalPaymentStatus != Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_CANCEL_EXPIRED)
				&& (this.OrderInput.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED);
		}
	}

	/// <summary>
	/// コンポーネントに値セット
	/// </summary>
	private void SetValueToComponents()
	{
		// 更新日プルダウン表示設定（プルダウンに本日の日付を表示）
		ucCardRealSalesDate.SetStartDate(DateTime.Now);
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
		// 入金ステータスの更新日表示
		var updateOrderPaymentStatusDate = DateTime.Now;
		// 入金日が空でない、かつ、入金ステータスが入金済み、一部入金の場合ＤＢから更新日を取得して表示
		if ((string.IsNullOrEmpty(this.OrderInput.OrderPaymentDate) == false)
			&& ((this.OrderInput.OrderPaymentStatus == Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE)
			|| (this.OrderInput.OrderPaymentStatus == Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_SHORTAGE)))
		{
			updateOrderPaymentStatusDate = DateTime.Parse(this.OrderInput.OrderPaymentDate);
		}
		ucUpdateOrderPaymentStatusDate.SetStartDate(updateOrderPaymentStatusDate);

		// 督促ステータスの更新日表示
		var updateDemandStatusDate = DateTime.Now;
		// 督促日が空でない、かつ、督促ステータスが督促レベル１か２の場合ＤＢから更新日を取得して表示
		if ((string.IsNullOrEmpty(this.OrderInput.DemandDate) == false)
			&& ((this.OrderInput.DemandStatus == Constants.FLG_ORDER_DEMAND_STATUS_LEVEL1)
			|| (this.OrderInput.DemandStatus == Constants.FLG_ORDER_DEMAND_STATUS_LEVEL2)))
		{
			updateDemandStatusDate = DateTime.Parse(this.OrderInput.DemandDate);
		}
		ucUpdateDemandStatusDate.SetStartDate(updateDemandStatusDate);

		// 拡張ステータス設定
		// 拡張ステータスリスト作成
		var orderExtendStatusSettingList = GetOrderExtendStatusSettingList(this.LoginOperatorShopId);
		var extendStatusList = CreateExtendStatusListFromDataView(orderExtendStatusSettingList, this.OrderInput.DataSource);
		divExtend.Visible = (orderExtendStatusSettingList.Count != 0);
		rOrderExtendStatusList.DataSource = orderExtendStatusSettingList;
		rOrderExtendStatusList.DataBind();

		// リピータ内データセット（バインド後に実行する必要あり）
		for (int index = 0; index < rOrderExtendStatusList.Items.Count; index++)
		{
			var riRepeaterItem = rOrderExtendStatusList.Items[index];

			// 拡張パラメータ項目追加
			var rblExtend = (RadioButtonList)riRepeaterItem.FindControl("rblExtend");
			rblExtend.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ORDER, string.Format("{0}1", FIELD_ORDER_EXTEND_STATUS)));

			// 更新日時（年・月・日）追加
			var ucDate = (DateTimePickerPeriodInputControl)riRepeaterItem.FindControl("ucExtendStatusDate");

			// 初期設定
			var lExtendNo = (Literal)riRepeaterItem.FindControl("lExtendNo");
			var lExtendName = (Literal)riRepeaterItem.FindControl("lExtendName");
			var extendStatus = extendStatusList[index];
			lExtendNo.Text = extendStatus.No;
			lExtendName.Text = extendStatus.Name;
			foreach (ListItem li in rblExtend.Items)
			{
				li.Selected = (li.Value == extendStatus.Param);
			}
			ucDate.SetStartDate(extendStatus.Time);
		}

		// 外部連携ステータス設定
		rblExternalImportStatus.SelectedValue = this.OrderInput.ExternalImportStatus;

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
			ucUpdateOrderStatusDate.Disabled = true;
			btnUpdateOrderStatus.Enabled = false;

			cbReauthCancel.Enabled = false;
			cbCreditZeusCancel.Enabled = false;
			cbCreditSBPSCancel.Enabled = false;
			cbCreditYamatoKwcCancel.Enabled = false;
			cbCreditGMOCancel.Enabled = false;
			cbCreditZcomCancel.Enabled = false;
			cbCreditEScottCancel.Enabled = false;
			cbCareerSoftbankKetaiSBPSCancel.Enabled = false;
			cbCareerDocoomKetaiSBPSCancel.Enabled = false;
			cbCareerAuKantanSBPSCancel.Enabled = false;
			cbRecruitSBPSCancel.Enabled = false;
			cbPaypalSBPSCancel.Enabled = false;
			cbCvsDefGmoCancel.Enabled = false;
			cbYamatoKaCancel.Enabled = false;
			cbCvsDefAtodeneCancel.Enabled = false;
			cbAmazonPayCancel.Enabled = false;
			cbGmoCancel.Enabled = false;
			cbAmazonPayCV2Cancel.Enabled = false;
			cbPaypalCancel.Enabled = false;
			cbTriLinkAfterPay.Enabled = false;
			cbAtonePayCancel.Enabled = false;
			cbAfteePayCancel.Enabled = false;
			cbPaidyPayCancel.Enabled = false;
			cbLinePayCancel.Enabled = false;
			cbNPAfterPayCancel.Enabled = false;
			cbEcPayCancel.Enabled = false;
			cbGooddealShippingCancel.Enabled = false;
			cbNewebPayCancel.Enabled = false;
			cbVeriTransCancel.Enabled = false;
			cbPayPayCancel.Enabled = false;
			cbCreditRakutenCancel.Enabled = false;
			cbCreditPaygentCancel.Enabled = false;
			cbBokuCancel.Enabled = false;
			cbGmoAtokaraCancel.Enabled = false;
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

		var isNotAuthCompOrSaleComp = ((this.OrderInput.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
			&& (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Atobaraicom)
			&& (this.OrderInput.ExternalPaymentStatus != Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP)
			&& (this.OrderInput.ExternalPaymentStatus != Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP)
			&& (this.OrderInput.ExternalPaymentStatus != Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SHIP_COMP));

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

		// 決済種別設定
		hfOrderPaymentKbn.Value = this.OrderInput.OrderPaymentKbn;

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
			&& (m_unreloadParentPages.Any(p =>
				(p == Request[Constants.REQUEST_KEY_MANAGER_POPUP_PARENT_NAME])
					|| (p == ((string)Session[Constants.SESSIONPARAM_KEY_ORDERDETAIL_POPUP_PARENT_NAME])))))
		{
			// このウインドウがポップアップで、呼び出し元が注文関連ファイル取込画面の場合は親ウインドウを更新しない
			Session[Constants.SESSIONPARAM_KEY_ORDERDETAIL_POPUP_PARENT_NAME] = Constants.PAGE_MANAGER_ORDERFILEIMPORT_LIST;
			Session[Constants.SESSIONPARAM_KEY_ORDERDETAIL_RELOAD_PARENT_WINDOW] = Constants.KBN_RELOAD_PARENT_WINDOW_OFF;
		}
		else
		{
			// このウインドウがポップアップでない、または呼び出し元が特に指定されていないポップアップの場合は親ウインドウを更新する
			Session[Constants.SESSIONPARAM_KEY_ORDERDETAIL_POPUP_PARENT_NAME] = "";
			Session[Constants.SESSIONPARAM_KEY_ORDERDETAIL_RELOAD_PARENT_WINDOW] = Constants.KBN_RELOAD_PARENT_WINDOW_ON;
		}
	}

	/// <summary>
	/// ステータス更新をして、注文詳細画面に遷移する
	/// </summary>
	/// <param name="updateStatus">ステータス更新種別</param>
	/// <param name="orderStatus">ステータス値</param>
	/// <param name="updateDate">更新日時</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="isRedirectNextPage">リダイレクトするか</param>
	private void UpdateStatusAndGotoOrderDetail(
		Constants.StatusType updateStatus,
		string orderStatus,
		string updateDate,
		UpdateHistoryAction updateHistoryAction,
		bool isRedirectNextPage = true)
	{
		// 該当ステートメント取得
		var statement = GetUpdateStatusStatement(updateStatus, orderStatus);
		if (statement == null)
		{
			// 仮注文でステータス更新を行おうとした場合、更新処理を実行しない
			return;
		}

		// ステートメント実行用パラメータ作成
		var input = new Hashtable
		{
			{ Constants.FIELD_ORDER_ORDER_ID, this.OrderInput.OrderId },
			{ "update_date", updateDate },
			{ Constants.FIELD_ORDER_USER_ID, this.OrderInput.UserId },
			{ Constants.FIELD_ORDER_LAST_CHANGED, this.LoginOperatorName }
		};
		// 督促ステータス更新
		if (updateStatus == Constants.StatusType.Demand)
		{
			input.Add(Constants.FIELD_ORDER_DEMAND_STATUS, orderStatus);
		}
		// 返品交換ステータス更新
		else if (updateStatus == Constants.StatusType.RetuenExchange)
		{
			input.Add(Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_STATUS, orderStatus);
		}
		// 返金ステータス更新
		else if (updateStatus == Constants.StatusType.Repayment)
		{
			input.Add(Constants.FIELD_ORDER_ORDER_REPAYMENT_STATUS, orderStatus);
		}

		// ステータス更新処理
		using (var accessor = new SqlAccessor())
		{
			// トランザクション開始
			accessor.OpenConnection();
			accessor.BeginTransaction();

			try
			{
				// Get data input action
				var actionInput = new Hashtable
				{
					{ Constants.TABLE_ORDER, GetOrderStatusActionForHistory(updateStatus, orderStatus) }
				};
				OrderHistory history = new OrderHistory
				{
					OrderId = this.OrderInput.OrderId,
					Action = OrderHistory.ActionType.EcOrderConfirm,
					OpearatorName = this.LoginOperatorName,
					Accessor = accessor,
					UpdateAction = actionInput
				};

				// Begin write history
				history.HistoryBegin();

				// Cancel gooddeal shipping
				if (cbGooddealShippingCancel.Checked)
				{
					var timestamp = GooddealUtility.GetTimestamp();
					var request = GooddealUtility.CreateCancelShippingDeliveryRequest(this.OrderInput.OrderId, timestamp);
					var response = GooddealApi.CancelShippingDelivery(request, timestamp);
					if (response.IsCancelationSucceeded == false)
					{
						trOrderStatusError.Visible = true;
						lOrderStatusError.Text = WebMessages
							.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CAREER_CANCEL_ERROR)
							.Replace("@@ 1 @@", response.GetApiMessage());
						return;
					}
					else
					{
						input["update_date"] = (string.IsNullOrEmpty(response.Response.DateTime) == false)
							? response.Response.DateTime
							: input["update_date"];
					}
				}

				if (Constants.RECUSTOMER_API_OPTION_ENABLED && cbRecustomerCooperation.Checked)
				{
					var recustomerErrorMessage = string.Empty;
					var order = DomainFacade.Instance.OrderService.Get(this.OrderInput.OrderId);
					if (order.IsDigitalContents || order.IsGiftOrder)
					{
						recustomerErrorMessage
							= RecustomerApiLogger.CreateRelationMemoForError(order.IsGiftOrder ? RecustomerApiLogger.ErrorKbn.Gift : RecustomerApiLogger.ErrorKbn.DigitalContents);

						DomainFacade.Instance.OrderService.AppendRelationMemo(
							order.OrderId,
							((string.IsNullOrEmpty(order.RelationMemo) ? string.Empty : "\r\n") + recustomerErrorMessage),
							this.LoginOperatorName,
							UpdateHistoryAction.DoNotInsert,
							accessor);
					}
					else
					{
						recustomerErrorMessage = RecustomerApiFacade.OrderImporter(order, updateDate, this.LoginOperatorName, accessor);
					}

					if (string.IsNullOrEmpty(recustomerErrorMessage))
					{
						DomainFacade.Instance.OrderService.UpdateOrderExtendStatus(
							order.OrderId,
							Constants.RECUSTOMER_ORDER_EXTEND_STATUS_NO,
							Constants.FLG_ORDER_EXTEND_STATUS_ON,
							DateTimeWrapper.Instance.Now,
							this.LoginOperatorName,
							UpdateHistoryAction.DoNotInsert,
							accessor);
					}
					else
					{
						// 外部連携メモの更新があるため、失敗時も履歴の保存とコミットを行う
						if (updateHistoryAction == UpdateHistoryAction.Insert)
						{
							DomainFacade.Instance.UpdateHistoryService.InsertAllForOrder(this.OrderInput.OrderId, this.LoginOperatorName, accessor);
						}
						history.HistoryComplete();
						accessor.CommitTransaction();

						// エラーメッセージ表示
						trOrderStatusError.Visible = true;
						lOrderStatusError.Text = string.Format(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_RECUSTOMER_COOPERATION_ERROR), recustomerErrorMessage);

						return;
					}
				}

				bool isNeedUpdateCancelStatus = true;
				// １．外部連携キャンセル処理（現状はエラーの時はステータス変更しないようにする）
				// 注文ステータス更新かつ（キャンセルまたは仮注文キャンセル）の場合
				if ((updateStatus == Constants.StatusType.Order)
					&& ((orderStatus == Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED)
						|| (orderStatus == Constants.FLG_ORDER_ORDER_STATUS_TEMP_CANCELED))
					&& (MallOptionUtility.CheckMallKbn(this.OrderInput.ShopId, this.OrderInput.MallId)
						== MallOptionUtility.MallKbn.OwnSite))
				{
					var error = "";
					var apiErrorMessage = "";
					var isChecked = true;
					// 最後の注文取得 （与信＆更新対象注文は最後の注文）
					var orderOld = OrderCommon.GetLastAuthOrder(this.OrderInput.OrderId, accessor);
					var orderNew = (OrderModel)orderOld.Clone();
					// 交換注文キャンセルの場合、再与信実施
					if (Constants.PAYMENT_REAUTH_ENABLED && this.OrderInput.IsExchangeOrder
						&& OrderCommon.CheckCanPaymentReauth(orderNew.OrderPaymentKbn) && cbReauthCancel.Checked)
					{
						isChecked = true;
						// 最終請求金額計算 ＝ 現在の最終請求金額 - (交換注文の合計 - 返品注文分の合計)
						{
							var returnProductTotal =
								(from p in this.OrderInput.Shippings.SelectMany(s => (OrderItemInput[])s.Items).Distinct().ToArray()
								 where int.Parse(p.ItemQuantitySingle) < 0
								 select decimal.Parse(p.ItemPrice)).Sum();

							orderNew.LastBilledAmount = orderOld.LastBilledAmount
								- (decimal.Parse(this.OrderInput.OrderPriceTotal) - returnProductTotal);
						}
						// HACK: 売上確定処理の判断のため、キャンセル注文IDを設定する
						orderOld.OrderId = this.OrderInput.OrderId;

						// 外部連携実行
						var reauth =
							new ReauthCreatorFacade(
								orderOld,
								orderNew,
								ReauthCreatorFacade.ExecuteTypes.System,
								ReauthCreatorFacade.ExecuteTypes.System,
								ReauthCreatorFacade.OrderActionTypes.ExchangeCancel).CreateReauth();
						var reauthResult = reauth.Execute();

						var isResultDetailFailure = (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Failure);

						// 与信のみに失敗している場合エラー画面へ
						if (isResultDetailFailure)
						{
							// Atodeneの場合、与信エラーにする
							if (reauth.AuthLostForError)
							{
								var service = new OrderService();
								service.UpdateExternalPaymentInfoForAuthError(
									this.OrderInput.OrderId,
									reauthResult.ErrorMessages,
									this.OrderInput.LastChanged,
									UpdateHistoryAction.Insert);
							}
							Session[Constants.SESSION_KEY_ERROR_MSG] = StringUtility.ChangeToBrTag(reauthResult.ErrorMessages);
							Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
						}

						// 失敗？
						this.ReturnOrderReauthErrorMessages = null;
						var isResultDetailSuccess =
							(reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success);

						// 外部決済連携ログ格納処理
						OrderCommon.AppendExternalPaymentCooperationLog(
							isResultDetailSuccess,
							this.OrderInput.OrderId,
							isResultDetailSuccess
								? LogCreator.CreateMessage(this.OrderInput.OrderId, "")
								: reauthResult.ApiErrorMessages,
							this.LoginOperatorName,
							UpdateHistoryAction.Insert);

						if (isResultDetailSuccess == false)
						{
							this.ReturnOrderReauthErrorMessages = reauthResult.ErrorMessages + "\r\n"
								+ WebMessages.GetMessages(WebMessages.ERRMSG_EXTERNAL_PAYMENT_CANCEL_FAILED);

							// 登録完了後にエラーメッセージを表示するため、セッションにセット
							Session[Constants.SESSION_KEY_ERROR_MSG] = StringUtility.ChangeToBrTag(this.ReturnOrderReauthErrorMessages);
						}

						// 外部決済ステータス更新
						// 決済メモがある場合は決済情報を更新する（無い場合は更新しない）
						if (string.IsNullOrEmpty(reauthResult.PaymentMemo) == false)
						{
							// 決済連携メモセット
							orderNew.PaymentMemo = orderNew.PaymentMemo
								+ (string.IsNullOrEmpty(orderNew.PaymentMemo) ? string.Empty : "\r\n") + reauthResult.PaymentMemo;

							// 何かしらアクションを持っていたら、決済取引ID・決済注文IDセット（キャンセルのみの場合は空が格納される想定）
							if (reauth.HasAnyAction)
							{
								orderNew.CardTranId = reauthResult.CardTranId;
								orderNew.PaymentOrderId = reauthResult.PaymentOrderId;

								if (this.OrderInput.OrderId == orderNew.OrderId)
								{
									// キャンセル中の交換注文は最後の注文の場合、キャンセルステータスに更新しない
									isNeedUpdateCancelStatus = false;
								}
							}
							// 再与信・減額・請求書再発行・売上確定を持っていたら外部決済情報セット
							if (reauth.HasReauthOrReduceOrReprint || reauth.HasSales)
							{
								orderNew.ExternalPaymentAuthDate = reauth.GetUpdateReauthDate(
									orderOld.ExternalPaymentAuthDate,
									orderOld.OrderPaymentKbn,
									orderNew.OrderPaymentKbn);
								orderNew.ExternalPaymentStatus = reauth.HasSales
									? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SHIP_COMP
									: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
							}
							// キャンセルのみであれば外部済情報リセット
							else if (reauth.HasOnlyCancel)
							{
								orderNew.ExternalPaymentAuthDate = null;
								orderNew.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
							}

							// 失敗？
							if (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSuccess && reauth.HasSales)
							{
								orderNew.CardTranId = reauthResult.CardTranId;
								orderNew.PaymentOrderId = reauthResult.PaymentOrderId;
								orderNew.ExternalPaymentAuthDate = DateTime.Now;
								orderNew.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SHIP_ERROR;
							}
							else if (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthAndSalesSuccess
								|| (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSuccess
									&& (reauth.HasSales == false)))
							{
								orderOld.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL_ERROR;
							}
							else if (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success)
							{
								orderOld.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL;
							}
						}

						// 最後の注文更新
						new OrderService().UpdateForModify(orderNew, this.LoginOperatorName, UpdateHistoryAction.DoNotInsert, accessor);
					}
					else
					{
						// ＺＥＵＳクレジットキャンセル処理
						if (cbCreditZeusCancel.Checked)
						{
							var result = new ZeusSecureBatchCancelApi().Exec(this.OrderInput.CardTranId);
							if (result.Success == false)
							{
								error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CARD_CANCEL_ERROR)
									.Replace("@@ 1 @@", result.ErrorMessage);
								apiErrorMessage = result.ErrorMessage;
							}
						}
						// SBPSクレジット決算処理
						else if (cbCreditSBPSCancel.Checked)
						{
							// 与信後決済の場合はキャンセル処理をスルーする
							if (string.IsNullOrEmpty(this.OrderInput.CardTranId) == false)
							{
								var cancelApi = new PaymentSBPSCreditCancelApi();
								if (cancelApi.Exec(this.OrderInput.CardTranId) == false)
								{
									error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CARD_CANCEL_ERROR)
										.Replace("@@ 1 @@", cancelApi.ResponseData.ResErrMessages);
									apiErrorMessage = LogCreator.CreateErrorMessage(
										cancelApi.ResponseData.ResErrCode,
										cancelApi.ResponseData.ResErrMessages);
								}
							}
						}
						// YamatoKWCクレジット決算処理
						else if (cbCreditYamatoKwcCancel.Checked)
						{
							var cancelApi = new PaymentYamatoKwcCreditCancelApi();
							var response = cancelApi.Exec(this.OrderInput.PaymentOrderId);
							if (response.Success == false)
							{
								error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CARD_CANCEL_ERROR)
									.Replace("@@ 1 @@", response.ErrorInfoForLog);
								apiErrorMessage = LogCreator.CreateErrorMessage(
									response.ErrorCode,
									response.ErrorMessage);
							}
						}
						// GMOクレジット決算処理
						else if (cbCreditGMOCancel.Checked)
						{
							var paymentGMO = new PaymentGmoCredit();
							// 与信後決済の場合はキャンセル処理をスルーする
							if (string.IsNullOrEmpty(this.OrderInput.CardTranId) == false)
							{
								if (paymentGMO.Cancel(this.OrderInput.PaymentOrderId, this.OrderInput.CardTranId, OrderInput.OrderId) == false)
								{
									error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CARD_CANCEL_ERROR)
										.Replace("@@ 1 @@", paymentGMO.ErrorMessages);
									apiErrorMessage = paymentGMO.ErrorMessages;
								}
							}
						}
						else if (cbCreditZcomCancel.Checked)
						{
							// Zcom
							var adp = new ZcomCancelRequestAdapter(this.OrderInput.PaymentOrderId);
							var res = adp.Execute();
							if (res.IsSuccessResult() == false)
							{
								// キャンセル失敗
								error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CARD_CANCEL_ERROR)
									.Replace("@@ 1 @@", res.GetErrorDetailValue());
								apiErrorMessage = LogCreator.CreateErrorMessage(
									res.GetErrorCodeValue(),
									res.GetErrorDetailValue());
							}
						}
						else if (cbCreditEScottCancel.Checked)
						{
							// e-Scott
							var adp = EScottProcess1DeleteApi.CreateEScottMaster1DeleteApi(
								this.OrderInput.CardTranId,
								this.OrderInput.PaymentOrderId);
							var res = adp.ExecRequest();
							if (res.IsSuccess == false)
							{
								// キャンセル失敗
								error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CARD_CANCEL_ERROR)
									.Replace("@@ 1 @@", res.ResponseMessage);
								apiErrorMessage = LogCreator.CreateErrorMessage(
									res.ResponseCd,
									res.ResponseMessage);
							}
						}
						// ペイジェントクレカキャンセル処理
						else if (cbCreditPaygentCancel.Checked)
						{
							var result = false;
							var cancelError = string.Empty;
							switch (this.OrderInput.ExternalPaymentStatus)
							{
								// オーソリ済み時はオーソリキャンセル電文
								case Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP:
									var authCancelParams = new PaygentApiHeader(PaygentConstants.PAYGENT_APITYPE_CARD_AUTH_CANCEL);
									authCancelParams.PaymentId = this.OrderInput.CardTranId;
									var authCancelResult = PaygentApiFacade.SendRequest(authCancelParams);
									result = (string)authCancelResult[PaygentConstants.RESPONSE_STATUS] == PaygentConstants.PAYGENT_RESPONSE_STATUS_SUCCESS;
									cancelError = (string)authCancelResult[PaygentConstants.RESPONSE_CODE] + (string)authCancelResult[PaygentConstants.RESPONSE_DETAIL];
									break;

								// 売上確定済み時は売上キャンセル電文
								case Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP:
									var saleCancelParams = new PaygentApiHeader(PaygentConstants.PAYGENT_APITYPE_CARD_REALSALE_CANCEL);
									saleCancelParams.PaymentId = this.OrderInput.CardTranId;
									var saleCancelResult = PaygentApiFacade.SendRequest(saleCancelParams);
									result = (string)saleCancelResult[PaygentConstants.RESPONSE_STATUS] == PaygentConstants.PAYGENT_RESPONSE_STATUS_SUCCESS;
									cancelError = (string)saleCancelResult[PaygentConstants.RESPONSE_CODE] + (string)saleCancelResult[PaygentConstants.RESPONSE_DETAIL];
									break;

								default:
									error = "想定外の外部決済ステータスです。";
									break;
							}

							// キャンセル失敗時
							if (result == false)
							{
								// キャンセル失敗
								error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CARD_CANCEL_ERROR)
									.Replace("@@ 1 @@", cancelError);
								apiErrorMessage = cancelError;
							}
						}
						// ソフトバンク・ワイモバイルまとめて支払い(SBPS)
						else if (cbCareerSoftbankKetaiSBPSCancel.Checked)
						{
							var cancelApi = new PaymentSBPSCareerSoftbankKetaiCancelApi();
							if (cancelApi.Exec(this.OrderInput.CardTranId) == false)
							{
								error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CAREER_CANCEL_ERROR)
									.Replace("@@ 1 @@", cancelApi.ResponseData.ResErrMessages);
								apiErrorMessage = LogCreator.CreateErrorMessage(
									cancelApi.ResponseData.ResErrCode,
									cancelApi.ResponseData.ResErrMessages);
							}
						}
						// ドコモケータイ払い(SBPS)
						else if (cbCareerDocoomKetaiSBPSCancel.Checked)
						{
							var cancelApi = new PaymentSBPSCareerDocomoKetaiCancelApi();
							if (cancelApi.Exec(this.OrderInput.CardTranId) == false)
							{
								error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CAREER_CANCEL_ERROR)
									.Replace("@@ 1 @@", cancelApi.ResponseData.ResErrMessages);
								apiErrorMessage = LogCreator.CreateErrorMessage(
									cancelApi.ResponseData.ResErrCode,
									cancelApi.ResponseData.ResErrMessages);
							}
						}
						// auかんたん決済(SBPS)
						else if (cbCareerAuKantanSBPSCancel.Checked)
						{
							var cancelApi = new PaymentSBPSCareerAuKantanCancelApi();
							if (cancelApi.Exec(this.OrderInput.CardTranId) == false)
							{
								error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CAREER_CANCEL_ERROR)
								.Replace("@@ 1 @@", cancelApi.ResponseData.ResErrMessages);
								apiErrorMessage = LogCreator.CreateErrorMessage(
									cancelApi.ResponseData.ResErrCode,
									cancelApi.ResponseData.ResErrMessages);
							}
						}
						// リクルートかんたん支払い(SBPS)		
						else if (cbRecruitSBPSCancel.Checked)
						{
							var cancelApi = new PaymentSBPSRecruitCancelApi();
							if (cancelApi.Exec(this.OrderInput.CardTranId) == false)
							{
								error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CAREER_CANCEL_ERROR)
									.Replace("@@ 1 @@", cancelApi.ResponseData.ResErrMessages);
								apiErrorMessage = LogCreator.CreateErrorMessage(
									cancelApi.ResponseData.ResErrCode,
									cancelApi.ResponseData.ResErrMessages);
							}
						}
						// PayPal(SBPS)
						else if (cbPaypalSBPSCancel.Checked)
						{
							var cancelApi = new PaymentSBPSPaypalCancelApi();
							if (cancelApi.Exec(this.OrderInput.CardTranId) == false)
							{
								error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CAREER_CANCEL_ERROR)
									.Replace("@@ 1 @@", cancelApi.ResponseData.ResErrMessages);
								apiErrorMessage = LogCreator.CreateErrorMessage(
									cancelApi.ResponseData.ResErrCode,
									cancelApi.ResponseData.ResErrMessages);
							}
						}
						// 楽天ペイ(SBPS)
						else if (cbRakutenIdSBPSCancel.Checked)
						{
							var cancelApi = new PaymentSBPSRakutenIdCancelApi();
							if (cancelApi.Exec(this.OrderInput.CardTranId) == false)
							{
								error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CAREER_CANCEL_ERROR)
									.Replace("@@ 1 @@", cancelApi.ResponseData.ResErrMessages);
								apiErrorMessage = LogCreator.CreateErrorMessage(
									cancelApi.ResponseData.ResErrCode,
									cancelApi.ResponseData.ResErrMessages);
							}
						}
						// ヤマト後払い決済
						else if (cbYamatoKaCancel.Checked)
						{
							var cancelApi = new PaymentYamatoKaCancelApi();
							if (cancelApi.Exec(this.OrderInput.PaymentOrderId) == false)
							{
								error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CAREER_CANCEL_ERROR)
									.Replace("@@ 1 @@", cancelApi.ResponseData.ErrorMessages);
								apiErrorMessage = LogCreator.CreateErrorMessage(
									cancelApi.ResponseData.ErrorCode,
									cancelApi.ResponseData.ErrorMessages);
							}
						}
						// Gmo後払い決済
						else if (cbCvsDefGmoCancel.Checked)
						{
							var facade = new GmoDeferredApiFacade();
							var request = new GmoRequestOrderModifyCancel();
							request.KindInfo = new KindInfoElement();
							request.KindInfo.UpdateKind = UpdateKindType.OrderCancel;
							request.Buyer = new BuyerElement();
							request.Buyer.GmoTransactionId = this.OrderInput.CardTranId;
							request.Buyer.ShopTransactionId = this.OrderInput.PaymentOrderId;

							var result = facade.OrderModifyCancel(request);

							if (result.Result != ResultCode.OK)
							{
								error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CAREER_CANCEL_ERROR)
									.Replace(
										"@@ 1 @@",
										string.Join("\r\n", result.Errors.Error.Select(x => x.ErrorCode + "：" + x.ErrorMessage).ToArray()));
								foreach (var externalPaymentError in result.Errors.Error.Select(x => LogCreator.CreateErrorMessage(x.ErrorCode, x.ErrorMessage)).ToArray())
								{
									apiErrorMessage += "\t" + externalPaymentError;
								}
							}
						}
						// Atodene後払い決済
						else if (cbCvsDefAtodeneCancel.Checked)
						{
							var adp = new AtodeneCancelTransactionAdapter(this.OrderInput.CardTranId);
							var result = adp.ExecuteCancel();

							if (result.Result != AtodeneConst.RESULT_OK)
							{
								error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CAREER_CANCEL_ERROR)
									.Replace(
										"@@ 1 @@",
										string.Join("\r\n", result.Errors.Error.Select(x => LogCreator.CreateErrorMessage(x.ErrorCode, x.ErrorMessage)).ToArray()));
								apiErrorMessage = string.Join("\t", result.Errors.Error
									.Select(x => LogCreator.CreateErrorMessage(x.ErrorCode, x.ErrorMessage)).ToArray());
							}
						}
						// DSK後払い決済
						else if (cbCvsDefDskCancel.Checked)
						{
							var adapter = new DskDeferredOrderCancelAdapter(this.OrderInput.CardTranId, this.OrderInput.PaymentOrderId, decimal.Parse(this.OrderInput.LastBilledAmount).ToString("F0"));
							var result = adapter.Execute();

							if (result.IsResultOk == false)
							{
								error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CAREER_CANCEL_ERROR)
									.Replace(
										"@@ 1 @@",
										string.Join("\r\n", result.Errors.Error.Select(x => LogCreator.CreateErrorMessage(x.ErrorCode, x.ErrorMessage)).ToArray()));
								apiErrorMessage = string.Join("\t", result.Errors.Error
									.Select(x => LogCreator.CreateErrorMessage(x.ErrorCode, x.ErrorMessage)).ToArray());
							}
						}
						//後払いキャンセル
						else if (cbCvsDefAtobaraicomCancel.Checked)
						{
							var api = new AtobaraicomCancelationApi();
							var result = api.ExecCancel(this.OrderInput.PaymentOrderId);

							if (result == false)
							{
								error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CAREER_CANCEL_ERROR)
									.Replace("@@ 1 @@", api.ResponseMessage);
								apiErrorMessage = LogCreator.CreateErrorMessage(api.ErrorCode, api.ResponseMessage);
							}
						}
						//スコア後払いキャンセル
						else if (cbCvsDefScoreCancel.Checked)
						{
							var facade = new ScoreApiFacade();
							var request = new ScoreCancelRequest
							{
								Transaction =
								{
									BilledAmount = this.OrderInput.OrderPriceTotal.ToPriceString(),
									NissenTransactionId = this.OrderInput.CardTranId,
									ShopTransactionId = this.OrderInput.PaymentOrderId
								}
							};

							var result = facade.OrderCancel(request);

							if (result.Result != ScoreResult.Ok.ToText())
							{
								error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CAREER_CANCEL_ERROR)
									.Replace(
										"@@ 1 @@",
										string.Join("\r\n", result.Errors.ErrorList.Select(x => x.ErrorCode + "：" + x.ErrorMessage).ToArray()));
								foreach (var externalPaymentError in result.Errors.ErrorList.Select(x => LogCreator.CreateErrorMessage(x.ErrorCode, x.ErrorMessage)).ToArray())
								{
									apiErrorMessage += "\t" + externalPaymentError;
								}
							}
						}
						//ベリトランス後払いキャンセル
						else if (cbCvsDefVeritransCancel.Checked)
						{
							var result = new PaymentVeritransCvsDef().OrderCancel(this.OrderInput.PaymentOrderId);

							if (result.Mstatus != VeriTransConst.RESULT_STATUS_OK)
							{
								error = result.Errors != null
									? string.Join("\r\n", result.Errors.Select(e => string.Format("{0}：{1}", e.ErrorCode, e.ErrorMessage)).ToArray())
									: string.Format("{0}：{1}", result.VResultCode, result.MerrMsg);
								apiErrorMessage = LogCreator.CreateErrorMessage(result.VResultCode, result.MerrMsg);
							}
						}
						// Amazon Pay
						else if (cbAmazonPayCancel.Checked)
						{
							// オンライン決済ステータスが「売上確定済」の場合は全額返金処理
							if (this.OrderInput.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
							{
								var refund = AmazonApiFacade.Refund(this.OrderInput.CardTranId, decimal.Parse(this.OrderInput.LastBilledAmount), this.OrderInput.OrderId + "_" + DateTime.Now.ToString("HHmmssfff"));
								if (refund.GetSuccess() == false)
								{
									error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CAREER_CANCEL_ERROR)
										.Replace("@@ 1 @@", AmazonApiMessageManager.GetErrorMessage(refund.GetErrorCode()));
									apiErrorMessage = LogCreator.CreateErrorMessage(refund.GetReasonCode(), refund.GetReasonDescription());
								}
							}
							// オンライン決済ステータスが「未連携」の場合はオーソリクローズ処理
							else if (this.OrderInput.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_NONE)
							{
								var close = AmazonApiFacade.CloseAuthorization(orderOld.CardTranId);
								if (close.GetSuccess() == false)
								{
									error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CAREER_CANCEL_ERROR)
										.Replace("@@ 1 @@", AmazonApiMessageManager.GetErrorMessage(close.GetErrorCode()));
									apiErrorMessage = LogCreator.CreateErrorMessage(close.GetErrorCode(), close.GetErrorMessage());
								}
							}
						}
						// Amazon Pay CV2
						else if (cbAmazonPayCV2Cancel.Checked)
						{
							// オンライン決済ステータスが「売上確定済」の場合は全額返金処理
							if (this.OrderInput.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
							{
								var refund = this.AmazonFacade.CreateRefund(this.OrderInput.CardTranId, decimal.Parse(this.OrderInput.LastBilledAmount));
								if (refund.Success == false)
								{
									var responseError = AmazonCv2ApiFacade.GetErrorCodeAndMessage(refund);
									error = responseError.ReasonCode + "：" + responseError.Message;
									apiErrorMessage = LogCreator.CreateErrorMessage(responseError.ReasonCode, responseError.Message);
								}
							}
							// オンライン決済ステータスが「未連携」の場合はチャージキャンセル
							else if (this.OrderInput.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_NONE)
							{
								if (this.OrderInput.IsFixedPurchaseOrder)
								{
									var cancel = this.AmazonFacade.CancelCharge(this.OrderInput.CardTranId);
									if (cancel.Success == false)
									{
										var responseError = AmazonCv2ApiFacade.GetErrorCodeAndMessage(cancel);
										error = responseError.ReasonCode + "：" + responseError.Message;
										apiErrorMessage = LogCreator.CreateErrorMessage(responseError.ReasonCode, responseError.Message);
									}

								}
								else
								{
									var close = this.AmazonFacade.CloseChargePermission(this.OrderInput.PaymentOrderId);
									if (close.Success == false)
									{
										var responseError = AmazonCv2ApiFacade.GetErrorCodeAndMessage(close);
										error = responseError.ReasonCode + "：" + responseError.Message;
										apiErrorMessage = LogCreator.CreateErrorMessage(responseError.ReasonCode, responseError.Message);
									}
								}
							}
						}
						// 後付款(台湾後払い)
						else if (cbTriLinkAfterPay.Checked)
						{
							var result = TriLinkAfterPayApiFacade.CancelOrder(
								new TriLinkAfterPayCancelRequest(this.OrderInput.CardTranId));
							if (result.ResponseResult == false)
							{
								error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CAREER_CANCEL_ERROR)
									.Replace("@@ 1 @@", result.Message);

								apiErrorMessage = LogCreator.CreateErrorMessage(result.ErrorCode, result.Message);
							}
						}
						// PayPal
						else if (cbPaypalCancel.Checked)
						{
							var doRefund = ((Constants.PAYPAL_PAYMENT_METHOD == Constants.PayPalPaymentMethod.AUTH_WITH_SUBMIT)
								|| (this.OrderInput.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
								|| this.OrderInput.IsDigitalContents);
							var result = PayPalUtility.Payment.VoidOrRefund(this.OrderInput.CardTranId, doRefund);
							if (result.IsSuccess() == false)
							{
								error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CAREER_CANCEL_ERROR)
									.Replace("@@ 1 @@", result.Message);
								foreach (var res in result.Errors.All())
								{
									apiErrorMessage += "\t" + res;
								}
							}
						}
						// Paidy
						else if (cbPaidyPayCancel.Checked)
						{
							switch (Constants.PAYMENT_PAIDY_KBN)
							{
								case Constants.PaymentPaidyKbn.Direct:
									if (this.OrderInput.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
									{
										var resultOfRefund = PaidyPaymentApiFacade.RefundPayment(
											this.OrderInput.CardTranId,
											this.OrderInput.PaymentOrderId,
											decimal.Parse(this.OrderInput.LastBilledAmount));
										if (resultOfRefund.HasError)
										{
											error = WebMessages.GetMessages(
												WebMessages.ERRMSG_MANAGER_ORDER_CAREER_CANCEL_ERROR,
												resultOfRefund.GetApiErrorMessages());
											apiErrorMessage = resultOfRefund.GetApiErrorMessages();
										}

										DomainFacade.Instance.OrderService.UpdateCardTranId(
											this.OrderInput.OrderId,
											string.Empty,
											this.OrderInput.LastChanged,
											UpdateHistoryAction.DoNotInsert,
											accessor);
									}
									else if (this.OrderInput.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_NONE)
									{
										var resultOfClose = PaidyPaymentApiFacade.ClosePayment(orderOld.PaymentOrderId);
										if (resultOfClose.HasError)
										{
											error = WebMessages.GetMessages(
												WebMessages.ERRMSG_MANAGER_ORDER_CAREER_CANCEL_ERROR,
												resultOfClose.GetApiErrorMessages());
											apiErrorMessage = resultOfClose.GetApiErrorMessages();
										}
									}
									break;

								case Constants.PaymentPaidyKbn.Paygent:
									if (this.OrderInput.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP)
									{
										var refundResult = new PaygentApiFacade().PaidyRefund(
											this.OrderInput.CardTranId,
											decimal.Parse(this.OrderInput.LastBilledAmount));

										if (refundResult.IsSuccess == false)
										{
											error = WebMessages.GetMessages(
												WebMessages.ERRMSG_MANAGER_ORDER_EXTERNAL_PAYMENT_ERROR,
												refundResult.ReasonPhrase);
											apiErrorMessage = refundResult.ReasonPhrase;
										}
									}
									else
									{
										var authorizationCancellationResult = new PaygentApiFacade().PaidyAuthorizationCancellation(this.OrderInput.CardTranId);

										if (authorizationCancellationResult.IsSuccess == false)
										{
											error = WebMessages.GetMessages(
												WebMessages.ERRMSG_MANAGER_ORDER_CAREER_CANCEL_ERROR,
												authorizationCancellationResult.ReasonPhrase);
											apiErrorMessage = authorizationCancellationResult.ReasonPhrase;
										}
									}
									break;
							}
						}
						// NP後払い
						else if (cbNPAfterPayCancel.Checked)
						{
							var request = NPAfterPayUtility.CreateCancelOrGetPaymentRequestData(this.OrderInput.CardTranId);
							var result = NPAfterPayApiFacade.CancelOrder(request);
							if (result.IsSuccess == false)
							{
								error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CAREER_CANCEL_ERROR)
									.Replace("@@ 1 @@", result.GetApiErrorMessage());
								apiErrorMessage = result.GetApiErrorMessage();
							}
						}
						// Atone
						else if (cbAtonePayCancel.Checked)
						{
							var request = new AtoneRefundPaymentRequest()
							{
								AmountRefund = CurrencyManager.GetSettlementAmount(
									decimal.Parse(this.OrderInput.LastBilledAmount),
									decimal.Parse(this.OrderInput.SettlementRate),
									this.OrderInput.SettlementCurrency).ToString("0"),
								RefundReason = "キャンセル",
								DescriptionRefund = string.Empty
							};

							AtonePaymentApiFacade.RefundPayment(
								StringUtility.ToEmpty(this.OrderInput.CardTranId),
								request);
						}
						// Aftee
						else if (cbAfteePayCancel.Checked)
						{
							var request = new AfteeRefundPaymentRequest()
							{
								AmountRefund = CurrencyManager.GetSettlementAmount(
									decimal.Parse(this.OrderInput.LastBilledAmount),
									decimal.Parse(this.OrderInput.SettlementRate),
									this.OrderInput.SettlementCurrency).ToString("0"),
								RefundReason = "キャンセル",
								DescriptionRefund = string.Empty
							};

							AfteePaymentApiFacade.RefundPayment(
								StringUtility.ToEmpty(this.OrderInput.CardTranId),
								request);
						}
						// LINE Pay
						else if (cbLinePayCancel.Checked)
						{
							if (this.OrderInput.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
							{
								var refund = LinePayApiFacade.RefundPayment(
									this.OrderInput.CardTranId,
									decimal.Parse(this.OrderInput.SettlementAmount),
									new LinePayApiFacade.LinePayLogInfo(
										this.OrderInput.OrderId,
										this.OrderInput.PaymentOrderId,
										this.OrderInput.CardTranId));
								if (refund.IsSuccess == false)
								{
									apiErrorMessage = LogCreator.CreateErrorMessage(refund.ReturnCode, refund.ReturnMessage);
									error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CAREER_CANCEL_ERROR)
										.Replace("@@ 1 @@", apiErrorMessage);
								}
							}
							else if (this.OrderInput.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_NONE)
							{
								var cancel = LinePayApiFacade.VoidApiPayment(
									this.OrderInput.CardTranId,
									new LinePayApiFacade.LinePayLogInfo(
										this.OrderInput.OrderId,
										this.OrderInput.PaymentOrderId,
										this.OrderInput.CardTranId));
								if (cancel.IsSuccess == false)
								{
									apiErrorMessage = LogCreator.CreateErrorMessage(cancel.ReturnCode, cancel.ReturnMessage);
									error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CAREER_CANCEL_ERROR)
										.Replace("@@ 1 @@", apiErrorMessage);
								}
							}
						}
						// EcPay
						else if (cbEcPayCancel.Checked)
						{
							if (this.OrderInput.ExternalPaymentType == Constants.FLG_PAYMENT_TYPE_ECPAY_CREDIT)
							{
								// If order is sales then call api Refund else call api cancel
								var isSaleOrder = (this.OrderInput.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
									&& (this.OrderInput.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP);
								var request = (isSaleOrder
									? ECPayUtility.CreateRequestForCancelRefundAndCapturePayment(
										orderOld,
										false,
										true,
										decimal.Parse(this.OrderInput.SettlementAmount))
									: ECPayUtility.CreateRequestForCancelRefundAndCapturePayment(orderOld, true));
								var response = ECPayApiFacade.CancelRefundAndCapturePayment(request);

								if (response.IsSuccess == false)
								{
									error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CAREER_CANCEL_ERROR)
										.Replace("@@ 1 @@", response.ReturnMessage);
									apiErrorMessage = response.ReturnMessage;
								}

								new OrderService().UpdateCardTranId(
									this.OrderInput.OrderId,
									response.TradeNo,
									this.OrderInput.LastChanged,
									UpdateHistoryAction.DoNotInsert,
									accessor);
							}
						}
						// NewebPay
						else if (cbNewebPayCancel.Checked)
						{
							if (this.OrderInput.ExternalPaymentType == Constants.FLG_PAYMENT_TYPE_NEWEBPAY_CREDIT)
							{
								// If Order Is Sales Then Call Api Refund Else Call Api Cancel
								var isSaleOrder = (this.OrderInput.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
									&& (this.OrderInput.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP);
								var request = NewebPayUtility.CreateCancelRefundCaptureRequest(
									orderOld,
									(isSaleOrder == false),
									isSaleOrder,
									isSaleOrder ? orderOld.SettlementAmount : 0);
								var response = NewebPayApiFacade.SendCancelRefundAndCaptureOrder(request, (isSaleOrder == false));

								if (response.IsSuccess == false)
								{
									error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CAREER_CANCEL_ERROR)
										.Replace("@@ 1 @@", response.Response.Message);
									apiErrorMessage = response.Response.Message;
								}

								new OrderService().UpdateCardTranId(
									this.OrderInput.OrderId,
									response.Response.TradeNo,
									this.OrderInput.LastChanged,
									UpdateHistoryAction.DoNotInsert,
									accessor);
							}
						}
						// ベリトランス
						else if (cbVeriTransCancel.Checked)
						{
							var paymentVeritransCredit = new PaymentVeritransCredit();
							var response = paymentVeritransCredit.Cancel(this.OrderInput.PaymentOrderId);
							if (response.Mstatus != "success")
							{
								// キャンセル失敗
								error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CARD_CANCEL_ERROR)
									.Replace("@@ 1 @@", response.MerrMsg);
								apiErrorMessage = LogCreator.CreateErrorMessage(
									response.VResultCode,
									response.MerrMsg);
							}
						}
						// PayPay
						else if (cbPayPayCancel.Checked)
						{
							if (string.IsNullOrEmpty(this.OrderInput.CardTranId) == false)
							{
								switch (Constants.PAYMENT_PAYPAY_KBN)
								{
									case Constants.PaymentPayPayKbn.SBPS:
										var cancelApi = new PaymentSBPSPaypayCancelApi();
										var result = cancelApi.Exec(
											this.OrderInput.CardTranId,
											decimal.Parse(this.OrderInput.OrderPriceTotal));

										if (result == false)
										{
											error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CARD_CANCEL_ERROR)
												.Replace("@@ 1 @@", cancelApi.ResponseData.ResErrMessages);
											apiErrorMessage = LogCreator.CreateErrorMessage(
												cancelApi.ResponseData.ResErrCode,
												cancelApi.ResponseData.ResErrMessages);
										}
										break;

									case Constants.PaymentPayPayKbn.GMO:
										var order = OrderCommon.GetLastAuthOrder(this.OrderInput.OrderId, accessor);
										var cancel = (this.OrderInput.OnlinePaymentStatus == Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE)
											? new PaypayGmoFacade().RefundPayment(order, order.LastBilledAmount)
											: new PaypayGmoFacade().CancelPayment(order);

										if (string.IsNullOrEmpty(cancel.ErrorMessage) == false)
										{
											apiErrorMessage = LogCreator.CreateErrorMessage(cancel.ErrorMessage, cancel.ErrorMessage);
											error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CAREER_CANCEL_ERROR)
												.Replace("@@ 1 @@", apiErrorMessage);
										}
										break;
								}
							}

							if ((string.IsNullOrEmpty(this.OrderInput.PaymentOrderId) == false)
								&& (Constants.PAYMENT_PAYPAY_KBN == Constants.PaymentPayPayKbn.VeriTrans))
							{
								var paymentVeritransPaypayApi = new PaymentVeritransPaypay();
								var lastAuthOrder = OrderCommon.GetLastAuthOrder(this.OrderInput.OrderId, accessor);
								var response = (this.OrderInput.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
									? (IResponseDto)paymentVeritransPaypayApi.Refund(lastAuthOrder.PaymentOrderId, lastAuthOrder.LastBilledAmount)
									: (IResponseDto)paymentVeritransPaypayApi.Cancel(lastAuthOrder.PaymentOrderId);
								if (response.Mstatus != VeriTransConst.RESULT_STATUS_OK)
								{
									// キャンセル失敗
									error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CARD_CANCEL_ERROR)
										.Replace("@@ 1 @@", response.MerrMsg);
									apiErrorMessage = LogCreator.CreateErrorMessage(
										response.VResultCode,
										response.MerrMsg);
								}
							}
						}
						// Rakuten cancel
						else if (cbCreditRakutenCancel.Checked)
						{
							var request = new RakutenCancelOrRefundRequest
							{
								PaymentId = this.OrderInput.PaymentOrderId
							};

							// Call Rakuten Cancel Api
							var result = RakutenApiFacade.CancelOrRefund(request);
							if (result.ResultType != RakutenConstants.RESULT_TYPE_SUCCESS)
							{
								// キャンセル失敗
								error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CARD_CANCEL_ERROR)
									.Replace("@@ 1 @@", result.ErrorMessage);
								apiErrorMessage = LogCreator.CreateErrorMessage(
									result.ErrorCode,
									result.ErrorMessage);
							}
						}
						// Boku
						else if (cbBokuCancel.Checked)
						{
							var order = OrderCommon.GetLastAuthOrder(this.OrderInput.OrderId, accessor);
							var refundId = string.Format("refund{0}", order.CardTranId);
							var refund = new PaymentBokuRefundChargeApi().Exec(
								null,
								order.CardTranId,
								refundId,
								BokuConstants.CONST_BOKU_REASON_CODE_GOOD_WILL,
								null,
								(order.OrderTaxIncludedFlg == Constants.FLG_ORDER_ORDER_TAX_INCLUDED_PRETAX));

							if (refund == null)
							{
								apiErrorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_BOKU_PAYMENT_ERROR);
								error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CAREER_CANCEL_ERROR)
									.Replace("@@ 1 @@", apiErrorMessage);
							}
							else if ((refund.IsSuccess == false)
								|| (refund.RefundStatus == BokuConstants.CONST_BOKU_REFUND_STATUS_FAILED))
							{
								apiErrorMessage = LogCreator.CreateErrorMessage(refund.Result.ReasonCode, refund.Result.Message);
								error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CAREER_CANCEL_ERROR)
									.Replace("@@ 1 @@", apiErrorMessage);
							}
							else if (order.IsFixedPurchaseOrder == false)
							{
								var cancel = new PaymentBokuCancelOptinApi().Exec(order.PaymentOrderId);
								if ((cancel == null) || (cancel.IsSuccess == false))
								{
									apiErrorMessage = (cancel == null)
										? WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_BOKU_PAYMENT_ERROR)
										: LogCreator.CreateErrorMessage(cancel.Result.ReasonCode, cancel.Result.Message);
									error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CAREER_CANCEL_ERROR)
										.Replace("@@ 1 @@", apiErrorMessage);
								}
							}
						}
						//GMO cancel
						else if (cbGmoCancel.Checked)
						{
							var transactionAPi = new GmoTransactionApi();
							var request = new w2.App.Common.Order.Payment.GMO.TransactionModifyCancel.GmoRequestTransactionModifyCancel(this.OrderInput);
							request.KindInfo.UpdateKind = UpdateKindType.OrderCancel;
							var response = transactionAPi.TransactionModifyCancel(request);

							if (response.Result == ResultCode.NG)
							{
								var errormsg = response.Errors.Error[0];
								error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CARD_CANCEL_ERROR)
									.Replace("@@ 1 @@", errormsg.ErrorMessage);
								apiErrorMessage = LogCreator.CreateErrorMessage(
									errormsg.ErrCode,
									errormsg.ErrorMessage);
							}
						}
						//GMOアトカラ キャンセル
						else if (cbGmoAtokaraCancel.Checked)
						{
							var cancelApi = new PaymentGmoAtokaraCancelApi();
							var orderModel = new OrderService().Get(this.OrderInput.OrderId);
							var apiResult = cancelApi.Exec(PaymentGmoAtokaraTypes.UpdateKind.Cancel, orderModel);

							if (apiResult == false)
							{
								var errors = cancelApi.ResponseData.Errors;
								error = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_CARD_CANCEL_ERROR)
									.Replace("@@ 1 @@", errors.ErrorMessage);
								apiErrorMessage = LogCreator.CreateErrorMessage(
									errors.ErrorCode,
									errors.ErrorMessage);
							}
						}
						else
						{
							isChecked = false;
						}

						var hasApiErrorMessage = (string.IsNullOrEmpty(apiErrorMessage) == false);

						string externalApiLog;
						if (hasApiErrorMessage)
						{
							externalApiLog = apiErrorMessage;
						}
						else if (PaygentUtility.CheckIsPaidyPaygentPayment(this.OrderInput.OrderPaymentKbn))
						{
							string cancelMessage = orderOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP
								? PaygentConstants.PAYGENT_PAIDY_REFUND_LOG_MESSAGE
								: PaygentConstants.PAYGENT_PAIDY_CANCEL_LOG_MESSAGE;

							externalApiLog = LogCreator.CreateMessageWithPaymentId(
								this.OrderInput.CardTranId,
								this.OrderInput.PaymentOrderId,
								this.OrderInput.LastBilledAmount.ToPriceString(),
								cancelMessage);
						}
						else
						{
							externalApiLog = LogCreator.CrateMessageWithCardTranId(
								this.OrderInput.CardTranId ?? orderOld.CardTranId,
								string.Empty);
						}

						// 外部決済連携ログ格納処理
						OrderCommon.AppendExternalPaymentCooperationLog(
							hasApiErrorMessage == false,
							this.OrderInput.OrderId,
							externalApiLog,
							this.LoginOperatorName,
							UpdateHistoryAction.Insert,
							accessor);

						// If Paygent Paidy cancel, ignore log
						if ((PaygentUtility.CheckIsPaidyPaygentPayment(this.OrderInput.OrderPaymentKbn) == false)
							|| (rblOrderStatus.SelectedValue != Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED))
						{
							PaymentFileLogger.WritePaymentLog(
								hasApiErrorMessage == false,
								orderOld.PaymentName ?? "",
								PaymentFileLogger.PaymentType.Unknown,
								PaymentFileLogger.PaymentProcessingType.Unknown,
								hasApiErrorMessage
									? apiErrorMessage + string.Format("\tpaymentName : {0}", orderOld.PaymentName)
									: LogCreator.CrateMessageWithCardTranId(
										this.OrderInput.CardTranId ?? orderOld.CardTranId,
										"") + string.Format("\tpaymentName : {0}", orderOld.PaymentName));
						}
					}

					if (isChecked)
					{
						if (error == "")
						{
							// オンライン決済ステータス更新
							new OrderService().UpdateOnlinePaymentStatus(
								this.OrderInput.OrderId,
								Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_CANCELED,
								this.LoginOperatorName,
								UpdateHistoryAction.DoNotInsert,
								accessor);

							if (isNeedUpdateCancelStatus)
							{
								var actionName = cbPaidyPayCancel.Checked
									&& PaygentUtility.CheckIsPaidyPaygentPayment(this.OrderInput.OrderPaymentKbn)
									&& (this.OrderInput.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP)
										? "返金"
										: "キャンセル";

								// キャンセル向け外部決済ステータス系＆メモ更新（オンライン決済ステータス更新は二重更新になるが・・）
								OrderCommon.UpdateExternalPaymentStatusesAndMemoForCancel(
								this.OrderInput.OrderId,
									this.OrderInput.OrderPaymentKbn,
									this.OrderInput.PaymentOrderId,
									this.OrderInput.CardTranId,
									this.SendingAmount,
									this.OrderInput.IsExchangeOrder,
									this.LoginOperatorName,
									actionName,
									UpdateHistoryAction.DoNotInsert,
									accessor);
							}
						}
						else
						{
							trOrderStatusError.Visible = true;
							lOrderStatusError.Text = error;
							return;
						}
					}
				}

				var process = new ProcessAfterUpdateOrderStatus();

				// ステータス更新による請求書印字処理
				var errorMessage = process.UpdatedInvoiceByOrderStatus(this.OrderInput, updateStatus, orderStatus, accessor, this.LoginOperatorName);

				// エラメッセージ表示
				if (string.IsNullOrEmpty(errorMessage) == false)
				{
					trOrderStatusError.Visible = true;
					lOrderStatusError.Text = errorMessage;
					return;
				}

				// ２．ステータス更新実行
				var orderStatusUpdated = process.ModifyOrderStatus(statement, accessor, input);

				// ３．ステータス更新に伴う処理
				{
					// 返品交換区分が「交換」かつ返品交換ステータスが「返品交換完了」に更新された場合
					// ※交換注文を通常注文に変更する。
					//   具体的には、
					//   ・注文ステータスを「指定無し」→「注文済み」
					//   ・注文日時を「更新日時」★更新日指定の日時ではない！
					//   ・入金ステータスを「指定無し」→「入金確認待ち」
					//   ・督促ステータスを「指定無し」→「督促なし」
					//   に更新する。
					if ((this.OrderInput.ReturnExchangeKbn == Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE)
						&& (updateStatus == Constants.StatusType.RetuenExchange)
						&& (orderStatus == Constants.FLG_ORDER_ORDER_RETURN_EXCHANGE_STATUS_COMPLETE))
					{
						using (SqlStatement SqlStatement = new SqlStatement("Order", "UpdateOrderExchange"))
						{
							SqlStatement.ExecStatement(accessor, input);
						}
					}

					// 返品交換区分が「返品」かつ返品交換ステータスが「返品交換完了」に更新された場合、
					// 全返品チェックを行い、全返品であれば定期購入情報「購入回数（注文・出荷基準）」を減算する
					if ((this.OrderInput.ReturnExchangeKbn == Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN)
						&& (updateStatus == Constants.StatusType.RetuenExchange))
					{
						// 定期購入OP有効?
						if (Constants.FIXEDPURCHASE_OPTION_ENABLED)
						{
							var orderId = this.OrderInput.OrderId;
							var orderIdOrg = this.OrderInput.OrderIdOrg;

							// 注文（返品交換含む）取得
							var service = new OrderService();
							var relatedOrders = service.GetRelatedOrders(orderIdOrg, accessor);

							// 元注文取得
							var orderOrg = relatedOrders.First(o => o.IsOriginalOrder);

							// 返品注文取得
							var returnOrder = relatedOrders.First(oderModel => (oderModel.IsOriginalOrder == false));

							// 定期購入注文?
							if (orderOrg.FixedPurchaseId != "")
							{
								// 本注文の返品交換ステータスを「返品交換完了」にセット
								var order = relatedOrders.First(o => o.OrderId == orderId);
								if (orderStatus == Constants.FLG_ORDER_ORDER_RETURN_EXCHANGE_STATUS_COMPLETE)
								{
									order.OrderReturnExchangeStatus = Constants.FLG_ORDER_ORDER_RETURN_EXCHANGE_STATUS_COMPLETE;

									// 定期商品：返品商品の返品更新
									var fixedPurchaseService = new FixedPurchaseService();
									var returnOrders = relatedOrders.Where(orders => orders.IsReturnOrder);
									var relatedOrder = relatedOrders.First(item => (item.LastAuthFlg == Constants.FLG_ORDER_LAST_AUTH_FLG_ON));
									if (returnOrders.All(orders => orders.IsAlreadyReturnExchangeCompleted))
									{
										var listProductId = new List<string>();
										foreach (var item in order.Shippings[0].Items)
										{
											// Check return all product
											if ((relatedOrder.Items
													.Where(product => (product.VariationId == item.VariationId))
													.Sum(product => product.ItemQuantity) > 0)
												|| listProductId.Any(product => (product == item.ProductId))) continue;

											listProductId.Add(item.ProductId);
											fixedPurchaseService.UpdateForReturnOrderItem(
												orderOrg.FixedPurchaseId,
												orderOrg.OrderId,
												item.VariationId,
												this.LoginOperatorName,
												UpdateHistoryAction.DoNotInsert,
												accessor
											);
										}
									}

									// 注文商品が全て返品されているか？
									if (service.InspectReturnAllItems(relatedOrders, accessor))
									{
										// 定期購入：注文返品更新
										fixedPurchaseService.UpdateForReturnOrder(
											orderOrg.FixedPurchaseId,
											orderOrg.OrderId,
											this.LoginOperatorName,
											UpdateHistoryAction.DoNotInsert,
											accessor);
									}
								}
								if ((this.OrderInput.OrderReturnExchangeStatus == Constants.FLG_ORDER_ORDER_RETURN_EXCHANGE_STATUS_COMPLETE)
									&& ((orderStatus == Constants.FLG_ORDER_ORDER_RETURN_EXCHANGE_STATUS_ARRIVAL)
										|| (orderStatus == Constants.FLG_ORDER_ORDER_RETURN_EXCHANGE_STATUS_RECEIPT)))
								{
									var listProductId = new List<string>();
									foreach (var item in order.Shippings[0].Items)
									{
										if (listProductId.Any(product => (product == item.ProductId))) continue;

										listProductId.Add(item.ProductId);
										new FixedPurchaseService().UpdateForReturnOrderItem(
											orderOrg.FixedPurchaseId,
											orderOrg.OrderId,
											item.VariationId,
											this.LoginOperatorName,
											UpdateHistoryAction.DoNotInsert,
											accessor,
											true);
									}
								}
							}
						}
					}

					// ステータス更新による定期台帳処理
					process.UpdateFixedPurchaseByOrderStatus(
						orderStatusUpdated,
						updateStatus,
						orderStatus,
						this.OrderInput,
						this.LoginOperatorName,
						accessor);

					// 注文情報キャンセル処理（ステータス以外）
					// ステータス更新有りかつ注文ステータス更新かつ（キャンセルまたは仮注文キャンセル）？
					if ((orderStatusUpdated > 0)
						&& (updateStatus == Constants.StatusType.Order)
						&& ((orderStatus == Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED)
							|| (orderStatus == Constants.FLG_ORDER_ORDER_STATUS_TEMP_CANCELED)))
					{
						var order = OrderCommon.GetOrder(this.OrderInput.OrderId, accessor);

						// 付与ポイント戻し可能可能かどうかチェックしてからキャンセル処理を行う
						var cancelUserPointAddErrorMessage = CheckCanRevokeGrantedUserPoint(new OrderModel(order[0]), accessor);
						if (string.IsNullOrEmpty(cancelUserPointAddErrorMessage) == false)
						{
							trOrderStatusError.Visible = true;
							lOrderStatusError.Text = cancelUserPointAddErrorMessage;
							rblOrderStatus.SelectedValue = this.OrderInput.OrderStatus;

							accessor.RollbackTransaction();
							return;
						}
						var updateTwInvoiceStatus = string.Empty;
						if (OrderCommon.DisplayTwInvoiceInfo())
						{
							if (rbTwInvoiceCancel.Checked) updateTwInvoiceStatus = Constants.FLG_ORDER_INVOICE_STATUS_CANCEL;
							if (rbTwInvoiceRefund.Checked) updateTwInvoiceStatus = Constants.FLG_ORDER_INVOICE_STATUS_REFUND;
						}
						CancelOrder(order[0], updateTwInvoiceStatus, true, UpdateHistoryAction.DoNotInsert, accessor);

						// ユーザーリアルタイム累計購入回数処理
						var ht = new Hashtable
						{
							{ Constants.FIELD_ORDER_USER_ID, this.OrderInput.UserId },
							{ Constants.FIELD_USER_ORDER_COUNT_ORDER_REALTIME, new UserService().Get(this.OrderInput.UserId, accessor).OrderCountOrderRealtime }
						};
						OrderCommon.UpdateRealTimeOrderCount(ht, Constants.FLG_REAL_TIME_ORDER_COUNT_ACTION_CANCEL, accessor);
					}

					// ステータス更新による仮ポイント→本ポイントへ変更
					process.UpdateTempPointToRealPointByOrderStatus(
						this.OrderInput,
						updateStatus,
						orderStatus,
						accessor,
						(string)input[Constants.FIELD_ORDER_USER_ID],
						(string)input[Constants.FIELD_ORDER_ORDER_ID],
						this.LoginOperatorName);

					// ステータス更新によるシリアルキー割り当て
					var isUpdatedOrderStatusOrPaymentStatus = (updateStatus == Constants.StatusType.Order)
						|| (updateStatus == Constants.StatusType.Payment);
					process.DeliverSerialKeyByUpdateStatus(
						isUpdatedOrderStatusOrPaymentStatus,
						orderStatusUpdated,
						this.OrderInput.OrderId,
						this.OrderInput.DigitalContentsFlg,
						(updateStatus == Constants.StatusType.Order) ? orderStatus : this.OrderInput.OrderStatus,
						(updateStatus == Constants.StatusType.Payment) ? orderStatus : this.OrderInput.OrderPaymentStatus,
						this.LoginOperatorName,
						accessor);

					// ステータス更新による定期購入会員判定条件支払い
					process.UpdateFixPurChaseMemberFlgByPaymentStatus(
						updateStatus,
						orderStatus,
						this.OrderInput,
						this.LoginOperatorName,
						updateHistoryAction,
						accessor);
				}

				// 更新履歴登録
				if (updateHistoryAction == UpdateHistoryAction.Insert)
				{
					new UpdateHistoryService().InsertAllForOrder(this.OrderInput.OrderId, this.LoginOperatorName, accessor);
				}

				// Write history complete
				history.HistoryComplete();

				// トランザクションコミット
				accessor.CommitTransaction();
			}
			catch (Exception ex)
			{
				// トランザクションロールバック
				accessor.RollbackTransaction();
				FileLogger.WriteError(ex);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}
		}

		if (Constants.NE_OPTION_ENABLED
			&& Constants.NE_COOPERATION_CANCEL_ENABLED
			&& (updateStatus == Constants.StatusType.Order)
			&& ((orderStatus == Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED)
				|| (orderStatus == Constants.FLG_ORDER_ORDER_STATUS_TEMP_CANCELED))
			&& (OrderCommon.UpdateNextEngineOrderForCancel(this.RequestOrderId, null).Item3 == false))
		{
			NextEngineApi.SendFailureCancelOrderMail(this.RequestOrderId, this.OrderInput.UserId);
		}
		// 注文詳細へ遷移
		if (isRedirectNextPage) Response.Redirect(CreateOrderDetailUrl(this.OrderInput.OrderId));
	}

	/// <summary>
	/// 各ステータス更新ステートメント取得
	/// </summary>
	/// <param name="updateStatus">ステータス更新種別</param>
	/// <param name="status">ステータス値</param>
	/// <returns>ステータス更新ステートメント</returns>
	private string GetUpdateStatusStatement(Constants.StatusType updateStatus, string status)
	{
		// 注文ステータス？
		if (updateStatus == Constants.StatusType.Order)
		{
			return OrderCommon.GetUpdateOrderStatusStatement(status);
		}
		// 入金ステータス？
		else if (updateStatus == Constants.StatusType.Payment)
		{
			return OrderCommon.GetUpdateOrderPaymentStatusStatement(status);
		}
		// 督促ステータス？
		else if (updateStatus == Constants.StatusType.Demand)
		{
			return GetUpdateOrderDemandStatusStatement(status);
		}
		// 返品交換ステータス？
		else if (updateStatus == Constants.StatusType.RetuenExchange)
		{
			return OrderCommon.GetUpdateOrderReturnExchangeStatusStatement(status);
		}
		// 返金ステータス？
		else if (updateStatus == Constants.StatusType.Repayment)
		{
			return OrderCommon.GetUpdateOrderRepaymentStatusStatement(status);
		}

		return null;
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

		// 受注情報詳細ページへ
		var url = CreateOrderDetailUrl(
			orderId,
			(this.IsPopUp),
			((string)Session[Constants.SESSIONPARAM_KEY_ORDERDETAIL_RELOAD_PARENT_WINDOW] != Constants.KBN_RELOAD_PARENT_WINDOW_OFF),
			Constants.PAGE_MANAGER_ORDER_CONFIRM);
		return url;
	}

	/// <summary>
	/// 更新対象のユーザーに二重に初回購入ポイントが発行されていないかチェック
	/// </summary>
	/// <param name="userId">ユーザーID</param>
	/// <param name="orderId">注文ID</param>
	/// <returns></returns>
	private bool CheckHasAnotherFistBuyPoint(string userId, string orderId)
	{
		var sv = new PointService();

		// 初回購入履歴
		var history = sv.GetUserPointHistories(userId)
			.Where(x => x.PointKbn == Constants.FLG_USERPOINT_POINT_KBN_BASE && x.PointIncKbn == Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_FIRST_BUY)
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
	/// 更新日付入力チェック
	/// </summary>
	/// <param name="year">年</param>
	/// <param name="month">月</param>
	/// <param name="day">日</param>
	/// <param name="updateDate">日付文字列</param>
	/// <returns>エラーメッセージ</returns>
	private string CheckInputUpdateDate(string year, string month, string day, out string updateDate)
	{
		// 日付取得
		updateDate = year + "/" + month + "/" + day;

		// 入力チェック
		var input = new Hashtable
		{
			{ "update_date", updateDate },
			{ "update_date_year", year },
			{ "update_date_month", month },
			{ "update_date_day", day }
		};

		return Validator.Validate("OrderConfirm", input);
	}

	/// <summary>
	/// 決済取引ID取得
	/// </summary>
	/// <param name="orderId">受注ID</param>
	/// <returns>決済取引ID</returns>
	private string GetCardTranId(string orderId)
	{
		string cardTranId = null;
		DataView orderData = null;
		using (var accessor = new SqlAccessor())
		using (var statement = new SqlStatement("Order", "GetOrderCardTranId"))
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_ORDER_ORDER_ID, orderId }
			};
			orderData = statement.SelectSingleStatementWithOC(accessor, input);
		}
		if (orderData.Count > 0)
		{
			cardTranId = (string)orderData[0][Constants.FIELD_ORDER_CARD_TRAN_ID];
		}
		return cardTranId;
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

			case Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT:
				// Paygent以外の場合はデフォルト設定へ
				if (Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.Paygent) goto default;
				paymentStatusCompleteFlg = this.OrderInput.IsDigitalContents
					? Constants.PAYMENT_PAYGENT_CREDIT_PAYMENTMETHOD_FORDIGITALCONTENTS == Constants.PaygentCreditCardPaymentMethod.Capture
					: Constants.PAYMENT_PAYGENT_CREDIT_PAYMENTMETHOD == Constants.PaygentCreditCardPaymentMethod.Capture;
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
		return ((paymnetId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
			&& ((Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Zeus)
				|| (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.SBPS)
				|| (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Gmo)
				|| (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Zcom)
				|| (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.EScott)
				|| (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans)
				|| (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten)
				|| (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Paygent))
			&& Constants.PAYMENT_CARD_REALSALES_ENABLED);
	}

	/// <summary>
	/// Get list order status action field
	/// </summary>
	/// <param name="updateStatus">Status type</param>
	/// <param name="status">Status update</param>
	/// <returns>List action field</returns>
	private List<string> GetOrderStatusActionForHistory(Constants.StatusType updateStatus, string status)
	{
		var actionStatus = new List<string>();

		switch (updateStatus)
		{
			case Constants.StatusType.Order:	// 注文ステータス？
				actionStatus = OrderHistory.GetOrderStatusAction(status);

				if (((status == Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED) || (status == Constants.FLG_ORDER_ORDER_STATUS_TEMP_CANCELED))
					&& (MallOptionUtility.CheckMallKbn(this.OrderInput.ShopId, this.OrderInput.MallId) == MallOptionUtility.MallKbn.OwnSite)
					&& ((cbCreditGMOCancel.Checked)
						|| (cbCreditSBPSCancel.Checked)
						|| (cbCreditZeusCancel.Checked)
						|| (cbCreditZcomCancel.Checked)
						|| (cbCreditEScottCancel.Checked)
						|| (cbCareerSoftbankKetaiSBPSCancel.Checked)
						|| (cbCareerDocoomKetaiSBPSCancel.Checked)
						|| (cbCareerAuKantanSBPSCancel.Checked)
						|| (cbRecruitSBPSCancel.Checked)
						|| (cbPaypalSBPSCancel.Checked)
						|| (cbRakutenIdSBPSCancel.Checked)
						|| (cbYamatoKaCancel.Checked)
						|| (cbAmazonPayCancel.Checked)
						|| (cbTriLinkAfterPay.Checked)
						|| (cbPaidyPayCancel.Checked)
						|| (cbAtonePayCancel.Checked)
						|| (cbAfteePayCancel.Checked)
						|| (cbLinePayCancel.Checked)
						|| (cbNPAfterPayCancel.Checked)
						|| (cbEcPayCancel.Checked)
						|| (cbVeriTransCancel.Checked)
						|| (cbGooddealShippingCancel.Checked)
						|| (cbCreditRakutenCancel.Checked)))
				{
					actionStatus.Add(Constants.FIELD_ORDER_ONLINE_PAYMENT_STATUS);
				}
				break;

			case Constants.StatusType.Payment:// 入金ステータス？
				actionStatus = OrderHistory.GetOrderPaymentStatusAction();
				break;

			case Constants.StatusType.Demand:// 督促ステータス？
				actionStatus = OrderHistory.GetOrderDemandStatusAction();
				break;

			case Constants.StatusType.RetuenExchange:	// 返品交換ステータス？
				if ((this.OrderInput.ReturnExchangeKbn == Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE)
					&& (status == Constants.FLG_ORDER_ORDER_RETURN_EXCHANGE_STATUS_COMPLETE))
				{
					actionStatus.Add(Constants.FIELD_ORDER_ORDER_STATUS);
					actionStatus.Add(Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS);
					actionStatus.Add(Constants.FIELD_ORDER_DEMAND_STATUS);
				}

				actionStatus.AddRange(OrderHistory.GetOrderReturnExchangeStatusAction(status));
				break;

			case Constants.StatusType.Repayment:// 返金ステータス？
				actionStatus = OrderHistory.GetOrderRepaymentStatusAction();
				break;
		}

		return actionStatus;
	}

	/// <summary>
	/// クレジットカードで決済するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnRegisterCreditCard_Click(object sender, EventArgs e)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_USER_CREDITCARD_INPUT)
			.AddParam(Constants.REQUEST_KEY_ORDER_ID, Request[Constants.FIELD_ORDER_ORDER_ID])
			.AddParam(Constants.REQUEST_KEY_WINDOW_KBN, Constants.KBN_WINDOW_DEFAULT)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_INSERT).CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// クレジットカード登録チェックボックスチェック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbRegistCreditCard_CheckedChanged(object sender, EventArgs e)
	{
		trCreditCardName.Visible = cbRegistCreditCard.Checked;
	}

	/// <summary>
	/// クレジットカードで決済するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnReloadForRegisterdCreditCard_Click(object sender, EventArgs e)
	{
		DisplayForProvisionalCreditCard();
	}

	/// <summary>
	/// 仮クレジット向けフォーム表示
	/// </summary>
	private void DisplayForProvisionalCreditCard()
	{
		var isProvisionalCreditCardPaymentId = (this.OrderInput.OrderPaymentKbn == Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID);
		// ZEUS仮クレジット向け
		if (OrderCommon.IsPaymentCardTypeZeus)
		{
			var isOrderStatusTemp = this.OrderInput.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_TEMP;
			var isProvisionalCreditCardOrderZeus = isOrderStatusTemp && isProvisionalCreditCardPaymentId;

			tbdyCreditCardInputButtonZeus.Visible = (isProvisionalCreditCardOrderZeus && SessionManager.UsePaymentTabletZeus);
			tbdyCreditCardInputUnvisibleMessageZeus.Visible = isProvisionalCreditCardOrderZeus && (SessionManager.UsePaymentTabletZeus == false);
		}
		// ZEUS以外の仮クレジット向け
		else if (OrderCommon.NeedsRegisterProvisionalCreditCardCardKbnExceptZeus)
		{
			tbdyCreditCardRegisterForUnregisterd.Visible
				= tbdyCreditCardRegisternForUnauthed.Visible
				= divCreditCardAuthErrorMessage.Visible
				= btnRegisterUnregisterdCreditCardForAuthError.Visible = false;
			var user = new UserService().Get(this.OrderInput.UserId);
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
	}

	/// <summary>
	/// 仮クレジットカードを再登録する
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnRegisterUnregisterdCreditCardForAuthError_Click(object sender, EventArgs e)
	{
		lCreditCardRegisternForUnauthed.Text = "";

		var userCreditCardOld = UserCreditCard.Get(this.OrderInput.UserId, int.Parse(this.OrderInput.CreditBranchNo));
		var userCreditCardNew = new ProvisionalCreditCardProcessor().RegisterUnregisterdCreditCard(
			this.OrderInput.UserId,
			"",
			userCreditCardOld.RegisterActionKbn,
			"",
			userCreditCardOld.BeforeOrderStatus,
			this.LoginOperatorName,
			UpdateHistoryAction.Insert);

		new OrderService().Modify(
			this.RequestOrderId,
			order =>
			{
				order.CreditBranchNo = userCreditCardNew.BranchNo;
				order.PaymentOrderId = OrderCommon.CreatePaymentOrderId(this.LoginOperatorShopId);
				order.LastChanged = this.LoginOperatorName;
			},
			UpdateHistoryAction.Insert);
		this.OrderInput.CreditBranchNo = userCreditCardNew.BranchNo.ToString();

		DisplayForProvisionalCreditCard();
	}

	/// <summary>
	/// 与信を実行するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAuthRegisterdCreditCard_Click(object sender, EventArgs e)
	{
		// 2重で与信を実行しないようにステータスチェック（返品交換もあるので注文ステータスは見ない）
		var order = new OrderService().Get(this.RequestOrderId);
		if (order.OrderPaymentKbn != Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID)
		{
			lCreditCardRegisternForUnauthed.Text =
				WebSanitizer.HtmlEncode(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_ORDER_NOT_EXISTED));
			return;
		}

		if (cbRegistCreditCard.Checked)
		{
			var input = new OrderCreditCardInput
			{
				CreditBranchNo = CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW,	// 新規のみしかチェックされないため
				RegisterCardName = tbUserCreditCardName.Text,
				DoRegister = cbRegistCreditCard.Checked,
			};
			var creditCardErrorMessage = input.Validate();
			if (string.IsNullOrEmpty(creditCardErrorMessage) == false)
			{
				lCreditCardRegisternForUnauthed.Text = creditCardErrorMessage;
				return;
			}
		}

		var installmentsCode = OrderCommon.CreditInstallmentsSelectable
			? dllCreditInstallments.SelectedItem.Value
			: OrderCommon.GetCreditInstallmentsDefaultValue();
		var installmentsName = OrderCommon.CreditInstallmentsSelectable
			? dllCreditInstallments.SelectedItem.Text
			: ValueText.GetValueText(Constants.TABLE_ORDER, OrderCommon.CreditInstallmentsValueTextFieldName, installmentsCode);

		// 与信実行して注文を完了させる
		var errorMessage = new ProvisionalCreditCardProcessor().AuthAndUpdateOrderStatus(
			this.OrderInput.OrderId,
			int.Parse(this.OrderInput.CreditBranchNo),
			this.OrderInput.CardKbn,
			installmentsCode,
			installmentsName,
			this.LoginOperatorName,
			UpdateHistoryAction.DoNotInsert);
		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			//ここでは外部連携決済エラーログ処理はしない。AuthAndUpdateOrderStatusメソッド内でやっている。

			lCreditCardRegisternForUnauthed.Text = WebSanitizer.HtmlEncodeChangeToBr(errorMessage);
			btnRegisterUnregisterdCreditCardForAuthError.Visible = true;
			return;
		}

		// ユーザークレジットカードとして登録
		if (cbRegistCreditCard.Checked)
		{
			new UserCreditCardService().Modify(
				this.OrderInput.UserId,
				int.Parse(this.OrderInput.CreditBranchNo),
				userCreditCard =>
				{
					userCreditCard.CardDispName = tbUserCreditCardName.Text;
					userCreditCard.DispFlg = Constants.FLG_USERCREDITCARD_DISP_FLG_ON;
					userCreditCard.LastChanged = this.LoginOperatorName;
				},
				UpdateHistoryAction.DoNotInsert);
		}

		// 履歴登録
		new UpdateHistoryService().InsertAllForOrder(this.OrderInput.OrderId, this.LoginOperatorName);

		Response.Redirect(CreateOrderDetailUrl(this.RequestOrderId));
	}

	/// <summary>
	/// 注文ステータス更新時にすでに注文がキャンセルされていないか確認
	/// </summary>
	protected void CheckOrderStatusIsNotCancel()
	{
		var order = new OrderService().GetOrderInfoByOrderId(this.OrderInput.OrderId);
		if (order.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED)
		{
			var urlCreater = new UrlCreator(CreateOrderDetailUrl(this.OrderInput.OrderId));
			urlCreater.AddParam(Constants.REQUEST_KEY_ORDER_STATUS_UPDATED, "true");
			var url = urlCreater.CreateUrl();

			Response.Redirect(url);
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
			OrderCommon.SendMailStorePickUpInformation(this.OrderInput.OrderId);
		}

		// Update history
		DomainFacade.Instance.UpdateHistoryService.InsertAllForOrder(this.OrderInput.OrderId, this.LoginOperatorName);

		Response.Redirect(CreateOrderDetailUrl(this.OrderInput.OrderId));
	}

	/// <summary>
	/// キャンセル連動が有効か
	/// ※3dsがOFFの場合はTRUEを常に返す
	/// </summary>
	/// <returns>キャンセル連動が有効か</returns>
	private bool IsCancelEnabled()
	{
		// 3dsを利用していない場合は仮注文状態で離脱されるケースがないため
		// キャンセルを許可する
		if (((Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Gmo)
				&& (Constants.PAYMENT_SETTING_GMO_3DSECURE == false))
			|| ((Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans)
				&& (Constants.PAYMENT_VERITRANS4G_CREDIT_3DSECURE == false))) return true; 

		var result = (rblOrderStatus.SelectedValue == Constants.FLG_ORDER_ORDER_STATUS_TEMP_CANCELED)
			|| (string.IsNullOrEmpty(this.OrderInput.ExternalPaymentAuthDate) == false);

		return result;
	}

	#region 売上確定関連
	/// <summary>
	/// ドコモケータイ払い 売上確定処理
	/// </summary>
	/// <returns>売上確定処理エラーメッセージ</returns>
	protected string ExecRealSaleDocomoKetaiOrg()
	{
		var order = this.OrderInput.CreateModel();
		var docomoPayment = new DocomoPayment();
		if (docomoPayment.SendDecision(order) == false)
		{
			this.ApiErrorMessage = docomoPayment.ErrorMessage;
			return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_EXTERNAL_PAYMENT_ERROR)
				.Replace("@@ 1 @@", docomoPayment.ErrorMessage);
		}

		return "";
	}

	/// <summary>
	/// S!まとめて支払い 売上確定処理
	/// </summary>
	/// <returns>売上確定処理エラーメッセージ</returns>
	protected string ExecRealSaleSMatometeOrg()
	{
		var order = this.OrderInput.CreateModel();
		var softbankPayment = new SoftbankPayment();
		if (softbankPayment.SendDecision(order) == false)
		{
			this.ApiErrorMessage = softbankPayment.ErrorMessage;
			return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_EXTERNAL_PAYMENT_ERROR)
				.Replace("@@ 1 @@", softbankPayment.ErrorMessage);
		}

		return "";
	}

	/// <summary>
	///ソフトバンク・ワイモバイルまとめて支払い(SBPS) 売上確定処理
	/// </summary>
	/// <returns>売上確定処理エラーメッセージ</returns>
	protected string ExecRealSaleSoftbankKetaiSBPS()
	{
		var api = new PaymentSBPSCareerSoftbankKetaiSaleApi();
		if (api.Exec(this.OrderInput.CardTranId) == false)
		{
			this.ApiErrorMessage = api.ResponseData.ResErrMessages;
			this.ApiErrorCode = api.ResponseData.ResErrCode;
			return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_EXTERNAL_PAYMENT_ERROR)
				.Replace("@@ 1 @@", api.ResponseData.ResErrMessages);
		}

		return "";
	}

	/// <summary>
	/// ドコモケータイ払い(SBPS) 売上確定処理
	/// </summary>
	/// <returns>売上確定処理エラーメッセージ</returns>
	protected string ExecRealSaleDocomoKetaiSBPS()
	{
		var api = new PaymentSBPSCareerDocomoKetaiSaleApi();
		if (api.Exec(this.OrderInput.CardTranId, decimal.Parse(this.OrderInput.LastBilledAmount)) == false)
		{
			this.ApiErrorMessage = api.ResponseData.ResErrMessages;
			this.ApiErrorCode = api.ResponseData.ResErrCode;
			return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_EXTERNAL_PAYMENT_ERROR)
				.Replace("@@ 1 @@", api.ResponseData.ResErrMessages);
		}

		return "";
	}

	/// <summary>
	/// auかんたん決済(SBPS) 売上確定処理
	/// </summary>
	/// <returns>売上確定処理エラーメッセージ</returns>
	protected string ExecRealSaleAuKantanSBPS()
	{
		var api = new PaymentSBPSCareerAuKantanSaleApi();
		if (api.Exec(this.OrderInput.CardTranId, decimal.Parse(this.OrderInput.LastBilledAmount)) == false)
		{
			this.ApiErrorMessage = api.ResponseData.ResErrMessages;
			this.ApiErrorCode = api.ResponseData.ResErrCode;

			return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_EXTERNAL_PAYMENT_ERROR)
				.Replace("@@ 1 @@", api.ResponseData.ResErrMessages);
		}

		return "";
	}

	/// <summary>
	/// リクルートかんたん支払い(SBPS) 売上確定処理
	/// </summary>
	/// <returns>売上確定処理エラーメッセージ</returns>
	protected string ExecRealSaleRecruitSBPS()
	{
		var api = new PaymentSBPSRecruitSaleApi();
		if (api.Exec(this.OrderInput.CardTranId, decimal.Parse(this.OrderInput.LastBilledAmount)) == false)
		{
			this.ApiErrorMessage = api.ResponseData.ResErrMessages;
			this.ApiErrorCode = api.ResponseData.ResErrCode;

			return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_EXTERNAL_PAYMENT_ERROR)
				.Replace("@@ 1 @@", api.ResponseData.ResErrMessages);
		}

		return "";
	}

	/// <summary>
	/// 楽天ペイ(SBPS) 売上確定処理
	/// </summary>
	/// <returns>売上確定処理エラーメッセージ</returns>
	protected string ExecRealSaleRakutenIdSBPS()
	{
		var api = new PaymentSBPSRakutenIdSaleApi();
		if (api.Exec(this.OrderInput.CardTranId, decimal.Parse(this.OrderInput.LastBilledAmount)) == false)
		{
			this.ApiErrorMessage = api.ResponseData.ResErrMessages;
			this.ApiErrorCode = api.ResponseData.ResErrCode;
			return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_EXTERNAL_PAYMENT_ERROR)
				.Replace("@@ 1 @@", api.ResponseData.ResErrMessages);
		}

		return "";
	}

	/// <summary>
	/// Amazon Pay 売上確定処理
	/// </summary>
	protected string ExecRealSaleAmazonPay()
	{
		if (Constants.AMAZON_PAYMENT_CV2_ENABLED) return ExecRealSaleAmazonPayCV2();

		var response = AmazonApiFacade.Capture(
			this.OrderInput.CardTranId,
			decimal.Parse(this.OrderInput.LastBilledAmount),
			this.OrderInput.OrderId + "_" + DateTime.Now.ToString("HHmmssfff"));
		if (response.GetSuccess())
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var orderService = new OrderService();


				// 取引ID更新
				orderService.UpdateCardTranId(
					this.OrderInput.OrderId,
					response.GetCaptureId(),
					this.LoginOperatorName,
					UpdateHistoryAction.DoNotInsert,
					accessor);
				this.OrderInput.CardTranId = response.GetCaptureId();
				accessor.CommitTransaction();
			}

			return "";
		}
		else
		{
			this.ApiErrorMessage = response.GetErrorMessage();
			this.ApiErrorCode = response.GetErrorCode();
			return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_EXTERNAL_PAYMENT_ERROR).Replace("@@ 1 @@", AmazonApiMessageManager.GetErrorMessage(response.GetErrorCode()));
		}
	}

	/// <summary>
	/// Amazon Pay CV2 売上確定処理
	/// </summary>
	protected string ExecRealSaleAmazonPayCV2()
	{
		var response = this.AmazonFacade.CaptureCharge(
			this.OrderInput.CardTranId,
			decimal.Parse(this.OrderInput.LastBilledAmount));
		if (response.Success)
		{
			var orderService = new OrderService();

			orderService.UpdateCardTranId(
				this.OrderInput.OrderId,
				response.ChargeId,
				this.LoginOperatorName,
				UpdateHistoryAction.Insert);
			this.OrderInput.CardTranId = response.ChargeId;

			return string.Empty;
		}
		else
		{
			var error = AmazonCv2ApiFacade.GetErrorCodeAndMessage(response);

			this.ApiErrorCode = error.ReasonCode;
			this.ApiErrorMessage = error.Message;
			return error.ReasonCode + "：" + error.Message;
		}
	}

	/// <summary>
	/// ペイパル決済 売上確定処理
	/// </summary>
	/// <returns>売上確定処理エラーメッセージ</returns>
	protected string ExecRealSalePayPal()
	{
		if (Constants.PAYPAL_PAYMENT_METHOD == Constants.PayPalPaymentMethod.AUTH)
		{
			var result = PayPalUtility.Payment.Sales(
				this.OrderInput.CardTranId,
				decimal.Parse(this.OrderInput.LastBilledAmount));
			if (result.IsSuccess()) return "";

			this.ApiErrorMessage = string.Join("\t", result.Errors);
			var errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_EXTERNAL_PAYMENT_ERROR)
				.Replace("@@ 1 @@", result.Message);
			return errorMessage;
		}
		return "";
	}

	/// <summary>
	/// Paidy売上確定処理
	/// </summary>
	/// <returns>売上確定処理エラーメッセージ</returns>
	protected string ExecRealSalePaidy()
	{
		var cardTranId = string.Empty;
		switch (Constants.PAYMENT_PAIDY_KBN)
		{
			case Constants.PaymentPaidyKbn.Direct:
				// Get Paidy payment
				var paymentResponse = PaidyPaymentApiFacade.GetPayment(this.OrderInput.PaymentOrderId);
				if (paymentResponse.HasError)
				{
					this.ApiErrorMessage = paymentResponse.GetApiErrorMessages();

					return WebMessages.GetMessages(
						WebMessages.ERRMSG_MANAGER_ORDER_EXTERNAL_PAYMENT_ERROR,
						paymentResponse.GetApiErrorMessages());
				}

				// Capture Paidy payment
				var captureResponse = PaidyPaymentApiFacade.CapturePayment(this.OrderInput.PaymentOrderId);
				if (captureResponse.HasError)
				{
					this.ApiErrorMessage = captureResponse.GetApiErrorMessages();

					return WebMessages.GetMessages(
						WebMessages.ERRMSG_MANAGER_ORDER_EXTERNAL_PAYMENT_ERROR,
						captureResponse.GetApiErrorMessages());
				}

				cardTranId = captureResponse.Payment.Captures[0].Id;
				this.OrderInput.CardTranId = cardTranId;
				break;

			case Constants.PaymentPaidyKbn.Paygent:
				cardTranId = this.OrderInput.CardTranId;
				if (string.IsNullOrEmpty(cardTranId))
				{
					return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_EXTERNAL_PAYMENT_PAYMENT_TRANSACTION_ID_EMPTY);
				}

				if (string.IsNullOrEmpty(this.OrderInput.PaymentOrderId))
				{
					return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_EXTERNAL_PAYMENT_PAYMENT_ORDER_ID_EMPTY);
				}

				var settlementResult = new PaygentApiFacade().PaidySettlement(this.OrderInput.CardTranId);
				if (settlementResult.IsSuccess == false)
				{
					return WebMessages.GetMessages(
						WebMessages.ERRMSG_MANAGER_ORDER_EXTERNAL_PAYMENT_ERROR,
						settlementResult.GetErrorMessage());
				}
				break;
		}

		// 外部決済ステータス更新
		using (var accessor = new SqlAccessor())
		{
			accessor.OpenConnection();
			accessor.BeginTransaction();

			var orderService = new OrderService();
			orderService.UpdateExternalPaymentStatusSalesComplete(
				this.OrderInput.OrderId,
				this.LoginOperatorName,
				UpdateHistoryAction.DoNotInsert,
				accessor);

			// オンライン決済ステータス更新
			orderService.UpdateOnlinePaymentStatus(
				this.OrderInput.OrderId,
				Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED,
				this.LoginOperatorName,
				UpdateHistoryAction.DoNotInsert,
				accessor);

			// 取引ID更新
			if (string.IsNullOrEmpty(cardTranId) == false)
			{
				orderService.UpdateCardTranId(
					this.OrderInput.OrderId,
					cardTranId,
					this.LoginOperatorName,
					UpdateHistoryAction.DoNotInsert,
					accessor);
			}

			accessor.CommitTransaction();
		}

		return string.Empty;
	}

	/// <summary>
	/// Exec Real Sale LINE Pay
	/// </summary>
	/// <returns>Error Message</returns>
	protected string ExecRealSaleLinePay()
	{
		var amountCapture = decimal.Parse(this.OrderInput.LastBilledAmount) * decimal.Parse(this.OrderInput.SettlementRate);
		var response = LinePayApiFacade.CapturePayment(
			this.OrderInput.CardTranId,
			amountCapture,
			this.OrderInput.SettlementCurrency,
			new LinePayApiFacade.LinePayLogInfo(
				this.OrderInput.OrderId,
				this.OrderInput.PaymentOrderId,
				this.OrderInput.CardTranId));

		if (response.IsSuccess == false)
		{
			this.ApiErrorMessage = LogCreator.CreateErrorMessage(response.ReturnCode, response.ReturnMessage);

			return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_EXTERNAL_PAYMENT_ERROR)
				.Replace("@@ 1 @@", this.ApiErrorMessage);
		}

		using (var accessor = new SqlAccessor())
		{
			accessor.OpenConnection();
			accessor.BeginTransaction();

			var orderService = new OrderService();

			// 外部決済ステータス更新
			orderService.UpdateExternalPaymentStatusSalesComplete(
				this.OrderInput.OrderId,
				this.LoginOperatorName,
				UpdateHistoryAction.DoNotInsert,
				accessor);

			// オンライン決済ステータス更新
			orderService.UpdateOnlinePaymentStatus(
				this.OrderInput.OrderId,
				Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED,
				this.LoginOperatorName,
				UpdateHistoryAction.DoNotInsert,
				accessor);

			accessor.CommitTransaction();
		}

		this.OrderInput.CardTranId = response.Info.TransactionId;

		return string.Empty;
	}

	/// <summary>
	/// Exec Real Sale Atone
	/// </summary>
	/// <returns>Error Message If have Any</returns>
	protected string ExecRealSaleAtone()
	{
		var user = new UserService().Get(this.OrderInput.UserId);
		var tokenId = ((user != null)
			? user.UserExtend.UserExtendDataValue[Constants.FLG_USEREXTEND_USREX_ATONE_TOKEN_ID]
			: string.Empty);
		if (string.IsNullOrEmpty(tokenId) == false)
		{
			var order = this.OrderInput.CreateModel();
			order.IsOrderSalesSettled = true;
			AtoneResponse response = null;
			if (order.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_MIDST)
			{
				var requestAtone =
					AtonePaymentApiFacade.CreateDataAtoneAuthoriteForReturnExchange(order, order);
				requestAtone.Data.AuthenticationToken = tokenId;
				requestAtone.Data.UpdatedTransactions = new[] { order.CardTranId };
				response = AtonePaymentApiFacade.CreatePayment(requestAtone);
			}

			if ((response != null) && (response.IsSuccess == false))
			{
				var errorMessagesAtone = response.Errors.SelectMany(error => error.Messages).ToArray();
				return string.Join("<br />", errorMessagesAtone);
			}
			if ((response != null) && (response.IsAuthorizationSuccess == false))
			{
				return string.Join("<br />", response.AuthorizationResultNgReasonMessage);
			}

			var cardTranId = (response != null)
				? response.TranId
				: order.CardTranId;
			response = AtonePaymentApiFacade.CapturePayment(tokenId, cardTranId);
			if (response.IsSuccess == false)
			{
				var errorMessagesAtone = response.Errors.SelectMany(error => error.Messages);
				return string.Join("<br />", errorMessagesAtone);
			}
			if (response.IsAuthorizationSuccess == false)
			{
				return string.Join("<br />", response.AuthorizationResultNgReasonMessage);
			}
			var orderService = new OrderService();
			orderService.UpdateCardTranId(
				this.OrderInput.OrderId,
				response.TranId,
				this.LoginOperatorName,
				UpdateHistoryAction.DoNotInsert);
		}
		return string.Empty;
	}

	/// <summary>
	/// Exec Real Sale Aftee
	/// </summary>
	/// <returns>Error Message If have Any</returns>
	protected string ExecRealSaleAftee()
	{
		var user = new UserService().Get(this.OrderInput.UserId);
		var tokenId = ((user != null)
			? user.UserExtend.UserExtendDataValue[Constants.FLG_USEREXTEND_USREX_AFTEE_TOKEN_ID]
			: string.Empty);
		if (string.IsNullOrEmpty(tokenId) == false)
		{
			var order = this.OrderInput.CreateModel();
			order.IsOrderSalesSettled = true;
			AfteeResponse response = null;
			if (order.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_MIDST)
			{
				var requestAftee =
					AfteePaymentApiFacade.CreateDataAfteeAuthoriteForReturnExchange(order, order);
				requestAftee.Data.AuthenticationToken = tokenId;
				requestAftee.Data.UpdatedTransactions = order.CardTranId;
				response = AfteePaymentApiFacade.CreatePayment(requestAftee);
			}
			if ((response == null)
				|| response.IsSuccess)
			{
				var cardTranId = (response != null)
					? response.TranId
					: order.CardTranId;
				response = AfteePaymentApiFacade.CapturePayment(tokenId, cardTranId);
				if (response.IsSuccess == false)
				{
					var errorMessagesAftee = response.Errors.SelectMany(error => error.Messages);
					return string.Join("<br />", errorMessagesAftee);
				}
				var orderService = new OrderService();
				orderService.UpdateCardTranId(
					this.OrderInput.OrderId,
					response.TranId,
					this.LoginOperatorName,
					UpdateHistoryAction.DoNotInsert);
			}
			else
			{
				var errorMessagesAftee = response.Errors.SelectMany(error => error.Messages);
				return string.Join("<br />", errorMessagesAftee);
			}
		}
		return string.Empty;
	}

	/// <summary>
	/// EcPay決済の売上確定処理
	/// </summary>
	/// <returns>売上確定処理エラーメッセージ</returns>
	protected string ExecRealSaleEcPay()
	{
		// Capture Payment
		var orderInfo = this.OrderInput.CreateModel();
		var requestCapture = ECPayUtility.CreateRequestForCancelRefundAndCapturePayment(orderInfo);
		var responseCapture = ECPayApiFacade.CancelRefundAndCapturePayment(requestCapture);
		// Handle error message
		if (responseCapture.IsSuccess == false)
		{
			this.ApiErrorMessage = responseCapture.ReturnMessage;
			return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_EXTERNAL_PAYMENT_ERROR)
				.Replace("@@ 1 @@", responseCapture.ReturnMessage);
		}

		var cardTranId = responseCapture.TradeNo;
		this.OrderInput.CardTranId = cardTranId;

		// Update Card Tran Id
		if (string.IsNullOrEmpty(cardTranId) == false)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				new OrderService().UpdateCardTranId(
					this.OrderInput.OrderId,
					cardTranId,
					this.LoginOperatorName,
					UpdateHistoryAction.DoNotInsert,
					accessor);

				accessor.CommitTransaction();
			}
		}
		return string.Empty;
	}

	/// <summary>
	/// NewebPay決済の売上確定処理
	/// </summary>
	/// <returns>売上確定処理エラーメッセージ</returns>
	protected string ExecRealSaleNewebPay()
	{
		// Capture Payment
		var orderInfo = this.OrderInput.CreateModel();
		var requestCapture = NewebPayUtility.CreateCancelRefundCaptureRequest(orderInfo, false);
		var responseCapture = NewebPayApiFacade.SendCancelRefundAndCaptureOrder(requestCapture, false);
		// Handle Error Message
		if (responseCapture.IsSuccess == false)
		{
			this.ApiErrorMessage = responseCapture.Response.Message;
			return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_EXTERNAL_PAYMENT_ERROR)
				.Replace("@@ 1 @@", responseCapture.Response.Message);
		}

		var cardTranId = responseCapture.Response.TradeNo;
		this.OrderInput.CardTranId = cardTranId;

		// Update Card Tran Id
		if (string.IsNullOrEmpty(cardTranId) == false)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				new OrderService().UpdateCardTranId(
					this.OrderInput.OrderId,
					cardTranId,
					this.LoginOperatorName,
					UpdateHistoryAction.DoNotInsert,
					accessor);

				accessor.CommitTransaction();
			}
		}
		return string.Empty;
	}

	/// <summary>
	/// Exec real sale PayPay
	/// </summary>
	/// <returns>Error Message</returns>
	protected string ExecRealSalePayPay()
	{
		switch (Constants.PAYMENT_PAYPAY_KBN)
		{
			case Constants.PaymentPayPayKbn.SBPS:
				var api = new PaymentSBPSPaypaySaleApi();
				var result = api.Exec(
					this.OrderInput.CardTranId,
					decimal.Parse(this.OrderInput.OrderPriceTotal));

				if (result == false)
				{
					this.ApiErrorMessage = api.ResponseData.ResErrMessages;
					this.ApiErrorCode = api.ResponseData.ResErrCode;

					return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_EXTERNAL_PAYMENT_ERROR)
						.Replace("@@ 1 @@", api.ResponseData.ResErrMessages);
				}

				// Update paypay sbps info
				var resProcessDate = api.ResponseData.ResProcessDate;
				if (Validator.IsDate(resProcessDate))
				{
					using (var accessor = new SqlAccessor())
					{
						accessor.OpenConnection();
						accessor.BeginTransaction();

						DomainFacade.Instance.OrderService.UpdatePaypaySBPSInfo(
							this.OrderInput.OrderId,
							api.ResponseData.ResSpsTransactionId,
							this.LoginOperatorName,
							UpdateHistoryAction.DoNotInsert,
							accessor);

						accessor.CommitTransaction();
					}
				}
				break;

			case Constants.PaymentPayPayKbn.GMO:
				var order = OrderCommon.GetLastAuthOrder(this.OrderInput.OrderId);
				var paypayFacade = new PaypayGmoFacade();

				var response = paypayFacade.CapturePayment(order);
				if (string.IsNullOrEmpty(response.ErrorMessage) == false)
				{
					this.ApiErrorMessage = LogCreator.CreateErrorMessage(response.ErrorMessage, response.ErrorMessage);

					return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_EXTERNAL_PAYMENT_ERROR)
						.Replace("@@ 1 @@", this.ApiErrorMessage);
				}

				using (var accessor = new SqlAccessor())
				{
					accessor.OpenConnection();
					accessor.BeginTransaction();

					var orderService = new OrderService();

					// 外部決済ステータス更新
					orderService.UpdateExternalPaymentStatusSalesComplete(
						this.OrderInput.OrderId,
						this.LoginOperatorName,
						UpdateHistoryAction.DoNotInsert,
						accessor);

					// オンライン決済ステータス更新
					orderService.UpdateOnlinePaymentStatus(
						this.OrderInput.OrderId,
						Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED,
						this.LoginOperatorName,
						UpdateHistoryAction.DoNotInsert,
						accessor);

					accessor.CommitTransaction();
				}
				break;

			case Constants.PaymentPayPayKbn.VeriTrans:
				var lastAuthOrder = OrderCommon.GetLastAuthOrder(this.OrderInput.OrderId);
				var paypayCaptureResponse = new PaymentVeritransPaypay().Capture(lastAuthOrder);
				if (paypayCaptureResponse.Mstatus != VeriTransConst.RESULT_STATUS_OK)
				{
					this.ApiErrorMessage = LogCreator.CreateErrorMessage(
						paypayCaptureResponse.MerrMsg,
						paypayCaptureResponse.MerrMsg);

					return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_EXTERNAL_PAYMENT_ERROR)
						.Replace("@@ 1 @@", this.ApiErrorMessage);
				}

				using (var accessor = new SqlAccessor())
				{
					accessor.OpenConnection();
					accessor.BeginTransaction();

					var orderService = new OrderService();

					// 外部決済ステータス更新
					orderService.UpdateExternalPaymentStatusSalesComplete(
						this.OrderInput.OrderId,
						this.LoginOperatorName,
						UpdateHistoryAction.DoNotInsert,
						accessor);

					// オンライン決済ステータス更新
					orderService.UpdateOnlinePaymentStatus(
						this.OrderInput.OrderId,
						Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED,
						this.LoginOperatorName,
						UpdateHistoryAction.DoNotInsert,
						accessor);

					accessor.CommitTransaction();
				}
				break;
		}
		return string.Empty;
	}

	/// <summary>
	/// Execute real sale csv def
	/// </summary>
	/// <returns>Error message</returns>
	public string ExecRealSaleCsvDef()
	{
		var deliveryCompany = new DeliveryCompanyService().Get(this.OrderInput.Shippings[0].DeliveryCompanyId);
		var deliveryId = (deliveryCompany != null)
			? deliveryCompany.DeliveryCompanyTypePostPayment
			: string.Empty;
		var atobaraicomShippingResponse = new AtobaraicomShippingRegistrationApi().ExecShippingRegistration(
			this.OrderInput.PaymentOrderId,
			this.OrderInput.Shippings[0].ShippingCheckNo,
			deliveryId,
			this.OrderInput.InvoiceBundleFlg);

		return atobaraicomShippingResponse.IsSuccess
			? string.Empty
			: atobaraicomShippingResponse.ApiMessages;
	}

	/// <summary>
	/// 外部決済ステータス、かつ、オンライン決済ステータスを完了に更新する
	/// </summary>
	/// <param name="orderId">注文ID</param>
	/// <param name="onlinePaymentStatus">オンライン決済ステータス</param>
	/// <param name="cardTranId">取引ID</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	protected void UpdatePaymentStatusSalesComplete(string orderId, string onlinePaymentStatus, string cardTranId, UpdateHistoryAction updateHistoryAction)
	{
		using (var accessor = new SqlAccessor())
		{
			accessor.OpenConnection();
			accessor.BeginTransaction();

			var orderService = new OrderService();
			// 外部決済ステータス更新
			orderService.UpdateExternalPaymentStatusSalesComplete(orderId, this.LoginOperatorName, UpdateHistoryAction.DoNotInsert, accessor);
			// オンライン決済ステータス更新
			orderService.UpdateOnlinePaymentStatus(orderId, onlinePaymentStatus, this.LoginOperatorName, updateHistoryAction, accessor);
			// 取引ID更新
			if (string.IsNullOrEmpty(cardTranId) == false)
			{
				orderService.UpdateCardTranId(orderId, cardTranId, this.LoginOperatorName, updateHistoryAction, accessor);
			}
			// 決済連携メモ更新
			orderService.AddPaymentMemo(
				this.OrderInput.OrderId,
				OrderExternalPaymentMemoHelper.CreateOrderPaymentMemo(
					string.IsNullOrEmpty(this.OrderInput.PaymentOrderId)
						? this.OrderInput.OrderId
						: this.OrderInput.PaymentOrderId,
					this.OrderInput.OrderPaymentKbn,
					this.OrderInput.CardTranId,
					//「売上確定」
					ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_ORDER_CONFIRM,
						Constants.VALUETEXT_PARAM_PAYMENT_MEMO,
						Constants.VALUETEXT_PARAM_SALES_CONFIRMED),
					this.SendingAmount),
				this.LoginOperatorName,
				UpdateHistoryAction.DoNotInsert,
				accessor);

			accessor.CommitTransaction();
		}
	}

	/// <summary>
	/// Execute real sale boku payment
	/// </summary>
	/// <returns>Error message</returns>
	public string ExecRealSaleBokuPayment()
	{
		var apiMessage = string.Empty;
		var orderInfo = new OrderService().GetOrderInfoByOrderId(this.OrderInput.OrderId);
		var productNames = string.Join(
			",",
			orderInfo.Items.Select(item => item.ProductName));
		var charge = new PaymentBokuChargeApi().Exec(
			orderInfo.SettlementCurrency,
			string.Empty,
			productNames,
			orderInfo.OrderId,
			orderInfo.PaymentOrderId,
			orderInfo.SettlementAmount.ToString(),
			(orderInfo.OrderTaxIncludedFlg == Constants.FLG_ORDER_ORDER_TAX_INCLUDED_PRETAX),
			orderInfo.RemoteAddr,
			orderInfo.IsFixedPurchaseOrder,
			(orderInfo.IsFixedPurchaseOrder && (orderInfo.FixedPurchaseOrderCount > 1)),
			(orderInfo.IsFixedPurchaseOrder ? orderInfo.FixedPurchaseKbn : string.Empty),
			(orderInfo.IsFixedPurchaseOrder ? orderInfo.FixedPurchaseSetting1 : string.Empty));

		if (charge == null)
		{
			return CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_BOKU_PAYMENT_ERROR);
		}

		if ((charge.IsSuccess == false)
			|| (charge.ChargeStatus != BokuConstants.CONST_BOKU_CHARGE_STATUS_SUCCESS))
		{
			apiMessage = (charge.ChargeStatus == BokuConstants.CONST_BOKU_CHARGE_STATUS_IN_PROGRESS)
				? CommerceMessages.GetMessages(CommerceMessages.ERRMSG_BOKU_PAYMENT_PROCESS_TIME_OUT)
				: charge.Result.Message;
		}

		return apiMessage;
	}
	#endregion

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
	/// <summary>送金額</summary>
	protected decimal SendingAmount
	{
		get
		{
			return CurrencyManager.GetSendingAmount(
				decimal.Parse(this.OrderInput.LastBilledAmount),
				decimal.Parse(this.OrderInput.SettlementAmount),
				this.OrderInput.SettlementCurrency);
		}
	}
	/// <summary>
	/// APIエラーコード
	/// </summary>
	protected string ApiErrorCode { get; set; }
	/// <summary>Action complete message</summary>
	protected string ActionCompleteMessage
	{
		get { return (string)Session["ReissueInvoiceResultMessage"]; }
		set { Session["ReissueInvoiceResultMessage"] = value; }
	}
	/// <summary>アマゾンCV2ファサード</summary>
	private AmazonCv2ApiFacade AmazonFacade { get; set; }
	/// <summary>ユーザークレジットカード情報</summary>
	protected UserCreditCard UserCreditCard { get; set; }
	/// <summary>注文商品の頒布会コースが1種類か</summary>
	protected bool IsItemsOneSubscriptionBoxCourse
	{
		get { return this.OrderInput.ItemSubscriptionBoxCourseIds.Length == 1; }
	}
	/// <summary>表示用注文商品頒布会コースID（エンコード済み）</summary>
	protected string EncodedSubscriptionBoxCourseIdsForDisplay
	{
		get
		{
			var result = HtmlSanitizer.HtmlEncodeChangeToBr(
				string.Join(Environment.NewLine, this.OrderInput.ItemSubscriptionBoxCourseIds));
			return result;
		}
	}
	#endregion
}
