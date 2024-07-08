/*
=========================================================================================================
Module      : 注文決済画面(OrderSettlement.aspx.cs)
･･･････････････････････････････････････････････････････････････････････････････････････････････････････
Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using w2.App.Common;
using w2.App.Common.AmazonCv2;
using w2.App.Common.Mail;
using w2.App.Common.NextEngine;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.Boku;
using w2.App.Common.Order.Payment.Boku.Utils;
using w2.App.Common.Order.Payment.GMO.Zcom;
using w2.App.Common.Order.Payment.LinePay;
using w2.App.Common.Order.Payment.Paygent;
using w2.App.Common.Order.Payment.Paygent.Paidy.Checkout;
using w2.App.Common.Order.Payment.Paypay;
using w2.App.Common.Order.Payment.Veritrans;
using w2.App.Common.Order.Payment.YamatoKa.Utils;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Extensions;
using w2.Common.Logger;
using w2.Common.Web;
using w2.Domain;
using w2.Domain.Order;
using w2.Domain.TwOrderInvoice;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.Domain.UserCreditCard;

public partial class Form_Order_OrderSettlement : OrderCartPage
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }    // httpsアクセス

	#region ラップ済みコントロール宣言
	WrappedRepeater WrOrder { get { return GetWrappedControl<WrappedRepeater>("rOrder"); } }
	public WrappedHiddenField WhfPaidyPaymentId { get { return GetWrappedControl<WrappedHiddenField>("hfPaidyPaymentId"); } }
	public WrappedHiddenField WhfPaidyStatus { get { return GetWrappedControl<WrappedHiddenField>("hfPaidyStatus"); } }
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		if (this.IsPreview)
		{
			Preview.PageInvalidateAction(this.Page);
			this.WrOrder.DataSource = this.CartList;
			this.WrOrder.DataBind();
			return;
		}

		if (!IsPostBack)
		{
			// 画面遷移の正当性チェック
			CheckOrderUrlSession();

			if (string.IsNullOrEmpty(Request[Constants.REQUEST_KEY_ORDER_ID]) == false)
			{
				// 外部サイト遷移後に来てる場合は、パラメータがつく
				// その場合はセッションを復元
				SessionSecurityManager.RestoreSessionFromDatabaseForGoToOtherSite(
					Session,
					Request[Constants.REQUEST_KEY_ORDER_ID],
					true);

				var bokuOrder = GetBokuOrder();
				if ((bokuOrder != null)
					&& (string.IsNullOrEmpty(StringUtility.ToEmpty(bokuOrder["optin_id"])) == false))
				{
					// Exec boku validate
					ExecBokuOptinFlow(bokuOrder);
				}
			}
		}

		// LPカートからの遷移だったらカートリスト再取得
		var landingCartSessionKey = (string)Session[Constants.SESSION_KEY_LANDING_CART_SESSION_KEY];
		if (landingCartSessionKey != null) this.CartList = (CartObjectList)Session[landingCartSessionKey];

		//------------------------------------------------------
		// パラメタ取得
		//------------------------------------------------------
		Hashtable htParam = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
		RestoreSession(htParam);
		List<string> lDispErrorMessages = (List<string>)htParam["error"];
		List<string> lDispAlertMessages = (List<string>)htParam["alert"];
		string gmoTransactionResult = (string)htParam["gmo_transaction_result"];

		if (!IsPostBack)
		{
			// カウント
			var zeusCard3DSecureCount = this.ZeusCard3DSecurePaymentOrder.Count;
			var rakutenCard3DSecureCount = this.RakutenCard3DSecurePaymentOrder.Count;
			var zeusLinkPointCount = this.ZeusLinkPointPaymentOrder.Count;
			var sbpsMultiCount = this.SBPSMultiPaymentOrders.Count;
			var linePayCount = this.LinePayOrders.Count;
			var amazonPayCv2Count = this.AmazonPayCv2Orders.Count;
			var zcomCard3DSecureCount = this.ZcomCard3DSecurePaymentOrder.Count;
			var veriTrans3DSecurePaymentCount = this.VeriTrans3DSecurePaymentOrder.Count;
			var paypayCount = this.PaypayOrders.Count;
			var gmoCard3DSecurePaymentOrderCount = this.GmoCard3DSecurePaymentOrder.Count;
			var bokuPaymentCount = this.BokuPaymentOrders.Count;
			var yamatoKwc3DSecureCount = this.YamatoKwc3DSecureOrders.Count;
			var paygentCard3DSecurePaymentOrder = this.Paygent3DSecurePaymentOrders.Count;
			var gmoAtokaraCount = this.GmoAtokaraOrders.Count;
			var paidyPaygentCount = this.PaidyPaygentOrders.Count;

			//------------------------------------------------------
			// 決済対象が１つのみなら即画面遷移
			//------------------------------------------------------
			if ((Constants.DISPLAY_ORDERSETTLEMENTPAGE_IN_SINGLE_CART_CASE == false)
				&& (this.CartList.Items.Count == 1)
				&& ((zeusCard3DSecureCount
					+ zeusLinkPointCount
					+ sbpsMultiCount
					+ linePayCount
					+ amazonPayCv2Count
					+ zcomCard3DSecureCount
					+ veriTrans3DSecurePaymentCount
					+ paypayCount
					+ rakutenCard3DSecureCount
					+ gmoCard3DSecurePaymentOrderCount
					+ bokuPaymentCount
					+ yamatoKwc3DSecureCount
					+ paygentCard3DSecurePaymentOrder
					+ gmoAtokaraCount
					+ paidyPaygentCount) == 1))
			{
				if (zeusCard3DSecureCount == 1)
				{
					GoPaymentPage((string)this.ZeusCard3DSecurePaymentOrder[0][Constants.FIELD_ORDER_ORDER_ID]);
				}
				if (rakutenCard3DSecureCount == 1)
				{
					GoPaymentPage((string)this.RakutenCard3DSecurePaymentOrder[0][Constants.FIELD_ORDER_ORDER_ID]);
				}
				if (zeusLinkPointCount == 1)
				{
					GoPaymentPage((string)this.ZeusLinkPointPaymentOrder[0][Constants.FIELD_ORDER_ORDER_ID]);
				}
				if (sbpsMultiCount == 1)
				{
					GoPaymentPage(this.SBPSMultiPaymentOrders[0].Key);
				}
				if (linePayCount == 1)
				{
					GoPaymentPage(this.LinePayOrders[0].Key);
				}
				if (amazonPayCv2Count == 1)
				{
					GoPaymentPage(this.AmazonPayCv2Orders[0].Key);
				}
				// Zcom card 3DSecure
				if (zcomCard3DSecureCount == 1)
				{
					var order = this.ZcomCard3DSecurePaymentOrder.First();
					var paymentId = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN]);

					if ((paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
						&& (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Zcom))
					{
						// 同梱商品をカート情報に追加(GoPaymentPageを利用した形にしたほうが望ましい)
						AddBundledProductToCartList();
						GoPaymentZcom3DSecure(order);
					}
				}
				if (veriTrans3DSecurePaymentCount == 1)
				{
					GoPaymentPage((string)this.VeriTrans3DSecurePaymentOrder[0][Constants.FIELD_ORDER_ORDER_ID]);
				}
				// Paypay
				if (this.PaypayOrders.Count == 1)
				{
					GoPaymentPage(this.PaypayOrders.Keys.First());
				}
				if (gmoCard3DSecurePaymentOrderCount == 1)
				{
					// 同梱商品をカート情報に追加(GoPaymentPageを利用した形にしたほうが望ましい)
					AddBundledProductToCartList();
					var order = this.GmoCard3DSecurePaymentOrder.First();
					GoPaymentGmo3DSecure(order);
				}
				if (bokuPaymentCount == 1)
				{
					GoPaymentPage(this.BokuPaymentOrders[0].Key);
				}
				if (yamatoKwc3DSecureCount == 1)
				{
					GoPaymentYamatokwcPage(this.YamatoKwc3DSecureOrders[0]);
				}
				if (paygentCard3DSecurePaymentOrder == 1)
				{
					GoPaymentPage((string)this.Paygent3DSecurePaymentOrders[0][Constants.FIELD_ORDER_ORDER_ID]);
				}
				if (gmoAtokaraCount == 1)
				{
					GoPaymentPage(this.GmoAtokaraOrders[0].Key);
				}
				if (paidyPaygentCount == 1)
				{
					ExecutePaidyPaygentOrder(this.PaidyPaygentOrders.Keys.First(), true);
				}
			}

			//------------------------------------------------------
			// 注文完了へ
			//----------------------------------------------------
			// 画面遷移決済なし？
			if ((zeusCard3DSecureCount == 0)
				&& (zeusLinkPointCount == 0)
				&& (sbpsMultiCount == 0)
				&& (linePayCount == 0)
				&& amazonPayCv2Count == 0
				&& (this.YamatoKaSmsOrders.Count == 0)
				&& (zcomCard3DSecureCount == 0)
				&& (veriTrans3DSecurePaymentCount == 0)
				&& (paypayCount == 0)
				&& (rakutenCard3DSecureCount == 0)
				&& (gmoCard3DSecurePaymentOrderCount == 0)
				&& (bokuPaymentCount == 0)
				&& (yamatoKwc3DSecureCount == 0)
				&& (paygentCard3DSecurePaymentOrder == 0)
				&& (gmoAtokaraCount == 0)
				&& (paidyPaygentCount == 0))
			{
				if (Constants.TWINVOICE_ECPAY_ENABLED)
				{
					foreach (var order in this.SuccessOrder)
					{
						for (var index = 0; index < this.CartList.Items[0].Shippings.Count; index++)
						{
							// 仮注文でECPayと藍新Payが電子発票を発行しない
							if ((this.CartList.Items[0].Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
								|| (this.CartList.Items[0].Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
								|| (this.CartList.Items[0].Shippings[index].IsShippingAddrTw == false)) continue;

							var orderInvoice = new TwOrderInvoiceService().GetOrderInvoice(
									StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]),
									index + 1);

							if ((orderInvoice == null)
								|| (orderInvoice.TwInvoiceStatus ==
									Constants.FLG_ORDER_INVOICE_STATUS_ISSUED_LINKED)) continue;

							var errorMessage = OrderCommon.EcPayInvoiceReleased(
								StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]),
								this.CartList.Items[0].Shippings[index],
								index + 1,
								StringUtility.ToEmpty(htParam[Constants.FIELD_ORDER_LAST_CHANGED]));
							if (string.IsNullOrEmpty(errorMessage) == false)
							{
								order[OrderCommon.ECPAY_INVOICE_API_MESSAGE] = errorMessage;
							}
						}
					}
				}
				//------------------------------------------------------
				// セッションポイント情報更新
				//------------------------------------------------------
				string strUserId = (string)htParam["user_id"];
				try
				{
					// ポイントオプション有効かつログイン済の場合
					if (Constants.W2MP_POINT_OPTION_ENABLED)
					{
						if (this.IsLoggedIn)
						{
							this.LoginUserPoint = PointOptionUtility.GetUserPoint(strUserId);
						}
					}
				}
				catch (Exception ex)
				{
					// ログ書き込み
					AppLogger.WriteError("セッションポイント情報更新", ex);

					// アラート追記
					lDispAlertMessages.Add(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_ORDER_UPDATE_SESSIONPOINT_ALERT));
				}

				//------------------------------------------------------
				// 正常終了したカート取得、カートオブジェクト削除（ＤＢ削除済み）
				//------------------------------------------------------
				List<CartObject> lSuccessCart = new List<CartObject>();
				foreach (Hashtable htOrder in this.SuccessOrder)
				{
					foreach (CartObject co in this.CartList)
					{
						if ((string)htOrder[Constants.FIELD_ORDER_ORDER_ID] == co.OrderId)
						{
							lSuccessCart.Add(co);
							this.CartList.DeleteCartVurtual(co);
							break;
						}
					}
				}

				// 外部画面に遷移したときに古いカート情報が残るのでセッション削除
				SessionManager.CartListLp = null;

				//------------------------------------------------------
				// ドコモケータイ払いのカートを削除する(仮注文に入っている)
				//------------------------------------------------------
				foreach (string strOrderId in this.DocomoPaymentOrder.Keys)
				{
					foreach (CartObject co in this.CartList)
					{
						if (strOrderId == co.OrderId)
						{
							this.CartList.DeleteCartVurtual(co);
							break;
						}
					}
				}

				//------------------------------------------------------
				// エラーあり、未完了カートが２以上の場合、アラート追加
				//------------------------------------------------------
				if ((lDispErrorMessages.Count > 0) && (this.CartList.Items.Count > 1))
				{
					lDispAlertMessages.Add(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_ORDER_UNSETTLED_CART_ALERT));
				}

				//------------------------------------------------------
				// １件も成功しなかった場合はエラー画面へ
				//------------------------------------------------------
				if ((this.SuccessOrder.Count == 0) && (this.DocomoPaymentOrder.Count == 0))
				{
					// PayPal注文失敗したらフラグ立てる
					if (this.CartList.Items.Any(cart => cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL))
					{
						SessionManager.IsPayPalOrderfailed = true;
					}

					Session[Constants.SESSION_KEY_ERROR_MSG] = lDispErrorMessages;

					string nextPage;
					if (landingCartSessionKey != null)
					{
						var creator = new UrlCreator(this.CartList.LandingCartInputAbsolutePath.RemoveLeft(Constants.PATH_ROOT));
						if (this.IsAmazonLoggedIn)
						{
							creator.WithUrlFragment(Constants.CONST_CONTROLL_NAME_FOR_PAYMENT_AREA_AMAZON_PAY);
						}
						else
						{
							creator.AddParam(
								Constants.REQUEST_KEY_FOCUS_ON,
								Constants.CONST_CONTROLL_NAME_FOR_PAYMENT_AREA);
						}

						nextPage = creator.CreateUrl();
					}
					else
					{
						nextPage = (this.CartList.Items.First().Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2)
							? Constants.PAGE_FRONT_CART_LIST
							: Constants.PAGE_FRONT_ORDER_CONFIRM;
					}

					Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = nextPage;
					if (landingCartSessionKey != null)
					{
						SessionManager.SetLandingCartNextPageForCheck(this.CartList, nextPage);
					}

					var errorPageUrl = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
					var hasRejectedPaymentOfPaidyPay = this.CartList.Items.Any(cart => (cart.Payment != null)
						&& (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
						&& cart.Payment.IsRejectedPayment);
					if (hasRejectedPaymentOfPaidyPay)
					{
						errorPageUrl.AddParam(Constants.REQUEST_KEY_DISPTITILE_KBN, Constants.KBN_DISPTITILE_NONE)
							.AddParam(Constants.REQUEST_KEY_BACK_URL, Constants.PATH_ROOT + nextPage);
					}
					else
					{
						errorPageUrl.AddParam(Constants.REQUEST_KEY_BACK_URL, Constants.PATH_ROOT + nextPage);
					}

					Response.Redirect(errorPageUrl.CreateUrl());
				}

				// 注文同梱の場合、完了される注文に対する追加処理を行う
				if (this.IsOrderCombined)
				{
					SuccessCombinedOrderAdditionProcess(
						this.SuccessOrder.First(),
						lDispErrorMessages,
						strUserId,
						lSuccessCart.First().Coupon,
						this.DisablePaymentCancelOrderId);

					// 注文同梱関連のセッション情報をクリア
					if ((string)this.SuccessOrder[0][Constants.FIELD_ORDER_ORDER_PAYMENT_KBN] != Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
					{
						SessionManager.OrderCombineCartList = null;
						SessionManager.OrderCombineBeforeCartList = null;
						SessionManager.OrderCombinePaymentReselect = false;
						SessionManager.CartList = null;
					}

					var nextEngineCancelSuccess = true;
					using (var accessor = new SqlAccessor())
					{
						accessor.OpenConnection();
						accessor.BeginTransaction();

						var parentOrderId = StringUtility.ToEmpty(this.SuccessOrder.First()[Constants.FIELD_ORDER_COMBINED_ORG_ORDER_IDS]);

						if (Constants.NE_OPTION_ENABLED
							&& (Constants.NE_COOPERATION_ORDERCOMBINE_ENABLED
								|| Constants.NE_COOPERATION_CANCEL_ENABLED)
							&& (OrderCommon.UpdateNextEngineOrderForCancel(parentOrderId, accessor).Item3 == false))
						{
							NextEngineApi.SendFailureCancelOrderMail(parentOrderId, strUserId);
							nextEngineCancelSuccess = false;
						}
					}

					if (Constants.NE_OPTION_ENABLED
						&& Constants.NE_COOPERATION_ORDERCOMBINE_ENABLED
						&& nextEngineCancelSuccess)
					{
						var input = new MailTemplateDataCreaterByCartAndOrder(this.IsPc).GetOrderMailDatas(
							this.SuccessOrder[0],
							lSuccessCart[0],
							true);
						input[Constants.FIELD_ORDER_MEMO] = ((string)input[Constants.FIELD_ORDER_MEMO]).Trim();

						using (MailSendUtility mailSender = new MailSendUtility(
							Constants.CONST_DEFAULT_SHOP_ID,
							Constants.CONST_MAIL_ID_NEXT_ENGINE_ORDER_COMBINE_COMPLETE_FOR_MANAGER,
							strUserId,
							input,
							this.IsPc,
							Constants.MailSendMethod.Auto,
							lSuccessCart[0].Owner.DispLanguageCode,
							lSuccessCart[0].Owner.DispLanguageLocaleId))
						{
							if (mailSender.SendMail() == false)
							{
								AppLogger.WriteError(
									string.Format(
										"{0} : {1}",
										this.GetType().BaseType,
										mailSender.MailSendException));
							}
						}
					}
				}

				//------------------------------------------------------
				// 注文完了後、会員登録を行う場合の情報を残しておく
				//------------------------------------------------------
				// ゲスト注文の場合
				if (UserService.IsGuest(this.CartList.Owner.OwnerKbn))
				{
					Session[Constants.SESSION_KEY_USER_REGIST_AFTER_ORDER_USER_ID] = strUserId;     // ユーザーID
					Session[Constants.SESSION_KEY_USER_REGIST_AFTER_ORDER_CART_LIST] = lSuccessCart;    // 成功したカート情報
				}

				Session[Constants.SESSION_KEY_LANDING_CART_SESSION_KEY] = null;

				//------------------------------------------------------
				// 完了画面へ
				//------------------------------------------------------
				// 画面遷移の正当性チェックのため遷移先ページURLを設定
				Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_COMPLETE;

				// 画面遷移
				Response.Redirect(this.SecurePageProtocolAndHost
					+ Constants.PATH_ROOT
					+ Constants.PAGE_FRONT_ORDER_COMPLETE
					+ ((string.IsNullOrEmpty(SessionManager.AdvCodeNow)
						|| (Constants.ADD_ADVC_TO_REQUEST_PARAMETER_OPTION_ENABLED == false))
							? string.Empty
							: ("?" + Constants.REQUEST_KEY_ADVCODE + "=" + SessionManager.AdvCodeNow)));
			}

			//カート情報セッションに同梱商品が含まれていたら同梱商品を削除
			DeleteBundledProduct();

			this.WrOrder.DataSource = this.CartList;
			this.DataBind();
		}
	}

	/// <summary>
	/// 戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbBack_Click(object sender, EventArgs e)
	{
		var backUrl = (Session[Constants.SESSION_KEY_LANDING_CART_SESSION_KEY] != null)
			? Constants.PAGE_FRONT_LANDING_LANDING_CART_CONFIRM
			: Constants.PAGE_FRONT_ORDER_CONFIRM;

		foreach (var order in this.SuccessOrder)
		{
			this.CartList.DeleteCartVurtual(
				this.CartList.Items.Find(item => item.OrderId == (string)order[Constants.FIELD_ORDER_ORDER_ID]));
		}

		// 画面遷移の正当性チェックのため遷移先ページURLを設定
		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = backUrl;
		Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + backUrl);
	}

	/// <summary>
	/// リピータイベント
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rOrder_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		// 決済するボタンクリック
		if (e.CommandName == "settlement")
		{
			// 注文ID取得
			var wlOrderId = GetWrappedControl<WrappedLiteral>(WrOrder.Items[int.Parse((string)e.CommandArgument)], "lOrderId");
			GoPaymentPage(wlOrderId.Text);
		}
		else if (e.CommandName == "smsauth")
		{
			var wlOrderId = GetWrappedControl<WrappedLiteral>(WrOrder.Items[int.Parse((string)e.CommandArgument)], "lOrderId");
			var wtbAuthCode = GetWrappedControl<WrappedTextBox>(WrOrder.Items[int.Parse((string)e.CommandArgument)], "tbAuthCode");
			this.SmsAuthErrorMessage = AuthorizeSms(wlOrderId.Text, wtbAuthCode.Text);

			this.WrOrder.DataSource = this.CartList;
			DataBind();
		}
		else if (e.CommandName == "transittelnuminput")
		{
			var wlOrderId = GetWrappedControl<WrappedLiteral>(WrOrder.Items[int.Parse((string)e.CommandArgument)], "lOrderId");
			TransitTelNumInput(wlOrderId.Text);
		}
		else if (e.CommandName == "smssend")
		{
			var wlOrderId = GetWrappedControl<WrappedLiteral>(WrOrder.Items[int.Parse((string)e.CommandArgument)], "lOrderId");
			var wtbCellPhoneNumber = GetWrappedControl<WrappedTextBox>(WrOrder.Items[int.Parse((string)e.CommandArgument)], "tbCellPhoneNumber");
			var cart = this.CartList.Items.Find(item => (item.OrderId == wlOrderId.Text));
			var order = this.YamatoKaSmsOrders.Find(o => (o.Key == wlOrderId.Text)).Value;

			var cellPhoneNumber = wtbCellPhoneNumber.Text.Insert(3, "-").Insert(8, "-");
			if (Validator.CheckValidCellPhoneNumber(cellPhoneNumber) == false)
			{
				this.SmsAuthErrorMessage =
					WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_YAMATO_KA_SMS_TELNUM_INVALID);
				return;
			}
			cart.Payment.UserCreditCard.CooperationId = cellPhoneNumber;
			var paymentOrderId = OrderCommon.CreatePaymentOrderId(cart.ShopId);
			order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = paymentOrderId;
			order[Constants.FIELD_ORDER_CARD_TRAN_ID] = paymentOrderId;

			this.YamatoKaSmsInputTelNumOrders.Remove(wlOrderId.Text);
			ExecYamatoSmsSettlement(order, cart);
		}
	}

	/// <summary>
	/// 決済画面へ遷移
	/// </summary>
	/// <param name="orderId"></param>
	private void GoPaymentPage(string orderId)
	{
		//同梱商品をカート情報に追加
		AddBundledProductToCartList();

		// 決済区分取得
		DataView order = OrderCommon.GetOrder(orderId);
		string paymentId = (string)order[0][Constants.FIELD_ORDER_ORDER_PAYMENT_KBN];

		// ZEUS 3Dセキュア？
		if ((paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
			&& (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Zeus)
			&& (Constants.PAYMENT_SETTING_ZEUS_3DSECURE))
		{
			GoPaymentZeus3DSecure(orderId);
		}

		// RAKUTEN 3Dセキュア？
		if ((paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
			&& (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten)
			&& (Constants.PAYMENT_SETTING_RAKUTEN_3DSECURE))
		{
			GoPaymentRakuten3DSecure(orderId);
		}

		// ソフトバンクペイメント マルチ決済？
		if ((paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_SOFTBANKKETAI_SBPS)
			|| (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AUKANTAN_SBPS)
			|| (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_SBPS)
			|| (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_RECRUIT_SBPS)
			|| (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL_SBPS)
			|| (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_RAKUTEN_ID_SBPS)
			|| ((paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
				&& (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.SBPS)
				&& Constants.PAYMENT_SETTING_SBPS_CREDIT_3DSECURE)
			|| ((paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
				&& (Constants.PAYMENT_PAYPAY_KBN == Constants.PaymentPayPayKbn.SBPS)))
		{
			GoPaymentSBPSMultiPayment(orderId);
		}

		// LINE Pay決済？
		if (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY)
		{
			GoPaymentLinePay(orderId);
		}

		// Zeus LinkPoint？
		if ((paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
			&& (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Zeus)
			&& (Constants.PAYMENT_SETTING_ZEUS_USE_LINKPOINT_ENABLED))
		{
			GoPaymentZeusLinkPoint(orderId);
		}

		// AmazonPay Cv2?
		if (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2)
		{
			GoPaymentAmazonPayCv2(this.AmazonPayCv2Orders.Find(o => (o.Key == orderId)).Value);
		}

		// ヤマト後払いSMS認証決済?
		if (OrderCommon.CheckPaymentYamatoKaSms(paymentId))
		{
			ExecYamatoSmsSettlement(
				this.YamatoKaSmsOrders.Find(o => (o.Key == orderId)).Value,
				this.CartList.Items.Find(item => (item.OrderId == orderId)));
		}

		// Zcom card 3DSecure
		if ((paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
			&& (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Zcom))
		{
			var param = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
			this.ZcomCard3DSecurePaymentOrder = (List<Hashtable>)param[Constants.SESSION_KEY_PAYMENT_CREDIT_ZCOM_ORDER_3DSECURE];
			var zcomCard3DSecurePaymentOrderInfo = this.ZcomCard3DSecurePaymentOrder.Find(item =>
				(StringUtility.ToEmpty(item[Constants.FIELD_ORDER_ORDER_ID]) == orderId));

			GoPaymentZcom3DSecure(zcomCard3DSecurePaymentOrderInfo);
		}

		// ベリトランス3Dセキュア
		if ((paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
			&& (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans)
			&& Constants.PAYMENT_VERITRANS4G_CREDIT_3DSECURE)
		{
			GoPaymentVeriTrans3DSecure();
		}

		// Go payment Paypay
		if ((paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
			&& (Constants.PAYMENT_PAYPAY_KBN == Constants.PaymentPayPayKbn.GMO))
		{
			GoPaypayPayment(orderId);
		}

		// VeriTrans PayPay
		if ((paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
			&& (Constants.PAYMENT_PAYPAY_KBN == Constants.PaymentPayPayKbn.VeriTrans))
		{
			GoPaymentVeriTransPayPay(orderId);
		}

		// GMO 3Dセキュアか
		if ((paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
			&& (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Gmo)
			&& Constants.PAYMENT_SETTING_GMO_3DSECURE)
		{
			var param = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
			this.GmoCard3DSecurePaymentOrder = (List<Hashtable>)param["gmo_order_3dsecure"];
			var gmoCard3DSecurePaymentOrder = this.GmoCard3DSecurePaymentOrder.Find(
				item => ((string)item[Constants.FIELD_ORDER_ORDER_ID] == orderId));
			GoPaymentGmo3DSecure(gmoCard3DSecurePaymentOrder);
		}

		// Boku
		if (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU)
		{
			GoPaymentBoku(this.BokuPaymentOrders.Find(o => (o.Key == orderId)).Value);
		}

		// YamatoKWC 3Dセキュアか
		if ((paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
			&& (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.YamatoKwc)
			&& Constants.PAYMENT_SETTING_YAMATO_KWC_3DSECURE)
		{
			var param = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
			this.YamatoKwc3DSecureOrders = (List<Hashtable>)param[Constants.SESSION_KEY_PAYMENT_CREDIT_YAMATOKWC_ORDER_3DSECURE];
			var yamatoKWCCardPaymentOrder = this.YamatoKwc3DSecureOrders.Find(
				item => ((string)item[Constants.FIELD_ORDER_ORDER_ID] == orderId));
			GoPaymentYamatokwcPage(yamatoKWCCardPaymentOrder);
		}

		// ペイジェント3Dセキュアか
		if ((paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
			&& (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Paygent)
			&& Constants.PAYMENT_PAYGENT_CREDIT_3DSECURE)
		{
			Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_PAYMENT_PAYGENT_POST_3DS_RESULT;
			var paygentCardPaymentOrder = this.Paygent3DSecurePaymentOrders.Find(
				item => (string)item[Constants.FIELD_ORDER_ORDER_ID] == orderId);
			GoPaymentPaygentPage(paygentCardPaymentOrder);
		}

		// GMOアトカラ
		if (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA)
		{
			GoPaymentGmoAtokara(orderId);
		}
	}

	/// <summary>
	/// 3Dセキュア決済画面へ
	/// </summary>
	/// <param name="orderId">注文ID</param>
	private void GoPaymentZeus3DSecure(string orderId)
	{
		// 画面遷移の正当性チェックのため本人認証後の受取先URLを設定
		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_PAYMENT_ZEUS_CARD_3DSECURE_GET_RESULT;

		// ゼウス3Dセキュアデータ送信ページへ
		StringBuilder sbUrl = new StringBuilder();
		sbUrl.Append(this.SecurePageProtocolAndHost).Append(Constants.PATH_ROOT).Append(Constants.PAGE_FRONT_PAYMENT_ZEUS_CARD_3DSECURE_POST);
		sbUrl.Append("?").Append(Constants.REQUEST_KEY_ORDER_ID).Append("=").Append(HttpUtility.UrlEncode(orderId));

		Response.Redirect(sbUrl.ToString());
	}

	/// <summary>
	/// ZEUS（LinkPoint）決済画面へ
	/// </summary>
	/// <param name="orderId">注文ID</param>
	private void GoPaymentZeusLinkPoint(string orderId)
	{
		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_PAYMENT_ZEUS_LINK_POINT_GET_NOTICE;

		var url = new UrlCreator(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_PAYMENT_ZEUS_LINK_POINT_POST)
			.AddParam(Constants.REQUEST_KEY_PAYMENTCREDITCARD_ACTION_TYPE, Constants.ActionTypes.RegisterOrderCreditCard.ToString())
			.AddParam(Constants.REQUEST_KEY_ORDER_ID, orderId)
			.CreateUrl();

		Response.Redirect(url);
	}

	/// <summary>
	/// ソフトバンクペイメント マルチ決済画面へ
	/// </summary>
	/// <param name="orderId">注文ID</param>
	private void GoPaymentSBPSMultiPayment(string orderId)
	{
		// 画面遷移の正当性チェックのため受取先URLを設定
		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_CONFIRM;

		// ソフトバンクマルチペイメント注文フォーム
		StringBuilder sbUrl = new StringBuilder();
		sbUrl.Append(this.SecurePageProtocolAndHost).Append(Constants.PATH_ROOT).Append(Constants.PAGE_FRONT_PAYMENT_SBPS_MULTIPAYMENT_POST_ORDER);
		sbUrl.Append("?").Append(Constants.REQUEST_KEY_ORDER_ID).Append("=").Append(HttpUtility.UrlEncode(orderId));

		Response.Redirect(sbUrl.ToString());
	}

	/// <summary>
	/// 楽天3Dセキュア決済画面へ
	/// </summary>
	/// <param name="orderId">注文ID</param>
	private void GoPaymentRakuten3DSecure(string orderId)
	{
		// 画面遷移の正当性チェックのため本人認証後の受取先URLを設定
		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_PAYMENT_RAKUTEN_3DS_RESULT;

		// 楽天3Dセキュアデータ送信ページへ
		var url = new UrlCreator(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_PAYMENT_RAKUTEN_POST_3DS_AUTH)
			.AddParam(Constants.REQUEST_KEY_ORDER_ID, orderId)
			.CreateUrl();

		Response.Redirect(url);
	}

	/// <summary>
	/// LINE Pay決済画面へ
	/// </summary>
	/// <param name="orderId">注文ID</param>
	private void GoPaymentLinePay(string orderId)
	{
		// 画面遷移の正当性チェックのため受取先URLを設定
		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_COMPLETE;

		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_PAYMENT_LINEPAY_RECEIVE)
			.AddParam("action", LinePayUtility.API_CALLBACK_REQUEST_FOR_ORDER)
			.AddParam(Constants.REQUEST_KEY_ORDER_ID, orderId)
			.AddParam("error", "1").CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// AmazonPayCv2決済画面へ
	/// </summary>
	/// <param name="order">注文</param>
	private void GoPaymentAmazonPayCv2(Hashtable order)
	{
		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_PAYMENT_AMAZONPAY_CV2_COMPLETE;

		var orderId = (string)order[Constants.FIELD_ORDER_ORDER_ID];

		var callBackUrl = new UrlCreator(Constants.PAGE_FRONT_PAYMENT_AMAZONPAY_CV2_COMPLETE)
			.AddParam(Constants.REQUEST_KEY_ORDER_ID, orderId)
			.CreateUrl();

		var facade = new AmazonCv2ApiFacade();

		var cs = (this.CartList.HasFixedPurchase)
			? facade.UpdateCheckoutSessionForFixedPurchase(
				this.AmazonCheckoutSessionId,
				callBackUrl,
				(decimal)order[Constants.FIELD_ORDER_ORDER_PRICE_TOTAL],
				orderId,
				this.CartList.Items[0].Shippings[0].FixedPurchaseKbn,
				this.CartList.Items[0].Shippings[0].FixedPurchaseSetting)
			: facade.UpdateCheckoutSession(
				this.AmazonCheckoutSessionId,
				callBackUrl,
				(decimal)order[Constants.FIELD_ORDER_ORDER_PRICE_TOTAL],
				orderId);

		// Amazonのリダイレクト先URLの取得に失敗（通常はAmazon側が原因）
		if (cs.Success == false)
		{
			AppLogger.WriteError("AmazonPay(CV2)決済エラー：注文ID=" + orderId);

			// エラーページへ
			var cartList = Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST;
			var next = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR)
				.AddParam(Constants.REQUEST_KEY_BACK_URL, cartList)
				.CreateUrl();

			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CHANGE_TO_ANOTHER_PAYMENT_FOR_AUTH_ERROR);
			Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = cartList;
			Response.Redirect(next);
		}

		var url = cs.WebCheckoutDetails.AmazonPayRedirectUrl;
		Response.Redirect(url);
	}

	/// <summary>
	/// Go payment boku
	/// </summary>
	/// <param name="order">Order</param>
	private void GoPaymentBoku(Hashtable order)
	{
		ExecBokuOptinFlow(order);
	}

	/// <summary>
	/// 電話番号入力状態に遷移
	/// </summary>
	/// <param name="orderId">ヤマト後払いSMS認証注文ID</param>
	private void TransitTelNumInput(string orderId)
	{
		this.YamatoKaSmsInputTelNumOrders.Add(orderId);

		this.WrOrder.DataSource = this.CartList;
		DataBind();
	}

	/// <summary>
	/// ヤマト後払いSMS認証決済実施
	/// </summary>
	/// <param name="order"></param>
	/// <param name="cart"></param>
	private void ExecYamatoSmsSettlement(Hashtable order, CartObject cart)
	{
		var telNum = cart.Payment.UserCreditCard.CooperationId;
		if (Validator.CheckValidCellPhoneNumber(telNum) == false)
		{
			TransitTelNumInput(cart.OrderId);
			return;
		}

		var paymentOrderId = (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID];
		var shipping = cart.GetShipping();
		var api = new PaymentYamatoKaEntryApi();
		if (api.Exec(
			paymentOrderId,
			DateTime.Now.ToString("yyyyMMdd"),
			PaymentYamatoKaUtility.CreateYamatoKaShipYmd(cart),
			cart.Owner.Name,
			StringUtility.ToHankakuKatakana(
				StringUtility.ToZenkakuKatakana(cart.Owner.NameKana)), // 全角カナにしてから半角カナへ変換
			cart.Owner.Zip,
			new PaymentYamatoKaAddress(cart.Owner.Addr1, cart.Owner.Addr2, cart.Owner.Addr3, cart.Owner.Addr4),
			telNum,
			cart.Owner.MailAddr,
			cart.PriceTotal,
			PaymentYamatoKaUtility.CreateSendDiv(OrderCommon.CheckPaymentYamatoKaSms((string)order[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN]), shipping.AnotherShippingFlag, false),
			PaymentYamatoKaUtility.CreateProductItemList(order, cart),
			shipping.Name,
			shipping.Zip,
			new PaymentYamatoKaAddress(shipping.Addr1, shipping.Addr2, shipping.Addr3, shipping.Addr4),
			cart.Owner.Tel1))
		{
			this.YamatoKaSmsExaminationOrders.Add((string)order[Constants.FIELD_ORDER_ORDER_ID]);
			Response.Redirect(this.RawUrl);
		}
		else
		{
			// ロールバック対象以外に注文がない注文時会員登録ユーザーを削除
			var isUserDelete = (((this.IsLoggedIn == false) || (this.RegisterUser != null))
				&& (this.SuccessOrder.Count == 0)
				&& (new OrderService().GetOrdersByUserId(cart.OrderUserId).Length == 1));

			// 注文ロールバック
			OrderCommon.RollbackPreOrder(
				order,
				cart,
				isUserDelete,
				(int)order[Constants.FIELD_USERSHIPPING_SHIPPING_NO],
				this.IsLoggedIn,
				UpdateHistoryAction.Insert);

			if (Constants.LANDING_CART_USER_REGISTER_WHEN_ORDER_COMPLETE && isUserDelete && this.IsLoggedIn)
			{
				this.LoginUserId = null;
			}

			// エラーメッセージを格納
			SessionManager.MessageForErrorPage =
				CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CVSDEFAUTH_ERROR);

			// ヤマト後払いSMS認証決済リストから該当注文を削除
			this.YamatoKaSmsOrders.RemoveAll(o => (o.Key == cart.OrderId));

			// 遷移先画面決定（まだ外部決済が残っている or 成功注文があるなら 決済ページへ、それ以外は 注文確認画面へ戻る）
			SessionManager.NextPageForCheck = ((this.HasUnsettledExternalPayments) || (this.SuccessOrder.Count > 0))
				? Constants.PAGE_FRONT_ORDER_SETTLEMENT
				: (order[Constants.SESSION_KEY_LANDING_CART_SESSION_KEY] != null)
					? Constants.PAGE_FRONT_LANDING_LANDING_CART_CONFIRM
					: Constants.PAGE_FRONT_ORDER_CONFIRM;

			// エラー画面遷移
			var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR)
				.AddParam(
					Constants.REQUEST_KEY_BACK_URL,
					this.SecurePageProtocolAndHost + Constants.PATH_ROOT + SessionManager.NextPageForCheck)
				.CreateUrl();
			Response.Redirect(url);
		}
	}

	/// <summary>
	/// ヤマトKWC3Dセキュア送信画面へ
	/// </summary>
	/// <param name="order">注文情報</param>
	private void GoPaymentYamatokwcPage(Hashtable order)
	{
		var url = new UrlCreator(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_PAYMENT_YAMATO_POST_3DS_AUTH)
			.AddParam(Constants.REQUEST_KEY_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID])
			.CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// ペイジェント3Dセキュア送信画面へ
	/// </summary>
	/// <param name="order">注文情報</param>
	private void GoPaymentPaygentPage(Hashtable order)
	{
		var url = new UrlCreator(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_PAYMENT_PAYGENT_POST_3DS_AUTH)
			.AddParam(Constants.REQUEST_KEY_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID])
			.CreateUrl();
		Session[PaygentConstants.PAYGENT_SESSION_ORDER] = order;
		Response.Redirect(url);
	}

	/// <summary>
	/// GMOペイメント マルチ決済画面へ
	/// </summary>
	/// <param name="order">注文情報</param>
	private void GoPaymentGmo3DSecure(Hashtable order)
	{
		// 画面遷移の正当性チェックのため受取先URLを設定
		this.Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] =
			Constants.PAGE_FRONT_PAYMENT_GMO_CARD_3DSECURE_GET_RESULT;

		var url = (string)order[Constants.FIELD_ORDER_CARD_3DSECURE_AUTH_URL];
		// 値がない場合は3Dセキュア1.0の処理
		if (string.IsNullOrEmpty(url))
		{
			url = new UrlCreator(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_PAYMENT_GMO_CARD_3DSECURE_POST)
				.AddParam(Constants.REQUEST_KEY_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID])
				.CreateUrl();
		}
		else
		{
			// セッション情報をファイルに保存
			SessionSecurityManager.SaveSesstionContetnsToDatabaseForGoToOtherSite(this.Session, (string)order[Constants.FIELD_ORDER_ORDER_ID]);
		}
		this.Response.Redirect(url);
	}

	/// <summary>
	/// SMS認証決済実施
	/// </summary>
	/// <param name="orderId">注文ID</param>
	/// <param name="authCode">認証コード</param>
	/// <returns>エラーメッセージ</returns>
	private string AuthorizeSms(string orderId, string authCode)
	{
		var errorMessage = new StringBuilder();

		if (ValidateAuthCode(authCode) == false)
		{
			return WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_YAMATO_KA_SMS_INVALID);
		}

		var api = new PaymentYamatoKaSmsAuthApi();
		var order = this.YamatoKaSmsOrders.Find(o => (o.Key == orderId)).Value;
		var cart = this.CartList.Items.Find(item => (item.OrderId == orderId));
		var paymentOrderId = (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID];
		var result = api.Exec(paymentOrderId, authCode);
		var value = PaymentYamatoKaSmsAuthResponseData.ResultValue.Ng;
		Enum.TryParse(api.ResponseData.Result, out value);

		switch (result)
		{
			case PaymentYamatoKaSmsAuthApi.ExecResult.Ok:
				OrderComplete(order);
				this.YamatoKaSmsOrders.Remove(this.YamatoKaSmsOrders.Find(o => (o.Key == (string)order[Constants.FIELD_ORDER_ORDER_ID])));
				this.YamatoKaSmsExaminationOrders.Remove(orderId);
				Response.Redirect(this.RawUrl);
				break;

			case PaymentYamatoKaSmsAuthApi.ExecResult.Ng:
				if (value == PaymentYamatoKaSmsAuthResponseData.ResultValue.Expired)
				{
					order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] =
						OrderCommon.CreatePaymentOrderId(cart.ShopId);
					errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_YAMATO_KA_SMS_EXPIRED));
					this.YamatoKaSmsExaminationOrders.Remove(orderId);
				}
				else
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_YAMATO_KA_SMS_FAILED);
					Response.Redirect(
						Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR + "?" + Constants.REQUEST_KEY_ERRORPAGE_KBN
						+ "=" + HttpUtility.UrlEncode(Constants.KBN_REQUEST_ERRORPAGE_GOTOP));
				}
				break;

			case PaymentYamatoKaSmsAuthApi.ExecResult.Reenter:
				errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_YAMATO_KA_SMS_MISMATCH_CODE));
				break;
		}

		return errorMessage.ToString();
	}

	/// <summary>
	/// 認証コード入力確認
	/// </summary>
	/// <param name="authCode">認証コード</param>
	/// <returns>確認結果</returns>
	private bool ValidateAuthCode(string authCode)
	{
		return (authCode.Length == 4) && (authCode.All(c => (char.IsNumber(c))));
	}

	/// <summary>
	/// 注文完了
	/// </summary>
	/// <param name="order">注文情報</param>
	/// <param name="hasCreditCard">Has credit card</param>
	private void OrderComplete(Hashtable order, bool hasCreditCard = true)
	{
		var cart = this.CartList.Items.Find(item => (item.OrderId == (string)order[Constants.FIELD_ORDER_ORDER_ID]));
		var register = new OrderRegisterFront(this.IsLoggedIn);

		// ３．注文確定処理
		//・ここを正常通過すれば何があっても注文完了。
		var success = register.UpdateForOrderComplete(order, cart, true, UpdateHistoryAction.DoNotInsert);

		// ４．後処理
		if (success)
		{
			if (this.IsLoggedIn && hasCreditCard)
			{
				new UserCreditCardService().UpdateCooperationId(
					LoginUserId,
					cart.Payment.UserCreditCard.BranchNo,
					cart.Payment.UserCreditCard.CooperationId,
					"user",
					UpdateHistoryAction.DoNotInsert);
			}
			// 注文完了時の処理
			register.OrderCompleteProcesses(order, cart, UpdateHistoryAction.DoNotInsert);

			// 注文完了後の処理（更新履歴とともに）
			register.AfterOrderCompleteProcesses(order, cart, UpdateHistoryAction.Insert);

			register.SuccessOrders.Add(order);
			Hashtable htParam = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
			var successOrder = (List<Hashtable>)htParam["order"];
			successOrder.Add(order);

			// 注文完了フラグを立てる
			cart.IsOrderDone = true;
		}
	}

	/// <summary>
	/// 注文決済状況取得
	/// </summary>
	/// <param name="strOrderId">注文ID</param>
	/// <returns></returns>
	protected string GetSettlementStatus(string strOrderId)
	{
		var failure = "failure";
		if (string.IsNullOrEmpty(strOrderId)) return failure;

		// 注文成功
		if (this.SuccessOrder.Exists(o => (string)o[Constants.FIELD_ORDER_ORDER_ID] == strOrderId)) return "success";

		// 未決済（ZEUS 3Dセキュア）
		if (this.ZeusCard3DSecurePaymentOrder.Exists(o => (string)o[Constants.FIELD_ORDER_ORDER_ID] == strOrderId)) return "incomplete_3dsecure";

		// 未決済（楽天 3Dセキュア）
		if (this.RakutenCard3DSecurePaymentOrder.Exists(o => (string)o[Constants.FIELD_ORDER_ORDER_ID] == strOrderId)) return "incomplete_rakuten_3dsecure";

		// 未決済（GMO 3Dセキュア）
		if (this.GmoCard3DSecurePaymentOrder.Exists(o => (string)o[Constants.FIELD_ORDER_ORDER_ID] == strOrderId))
		{
			return "incomplete_3dsecure";
		}

		// 未決済（ヤマトKWC 3Dセキュア2.0）
		if (this.YamatoKwc3DSecureOrders.Exists(order => (string)order[Constants.FIELD_ORDER_ORDER_ID] == strOrderId)) return "incomplete_3dsecure";

		// 未決済（SBPS）
		if (this.SBPSMultiPaymentOrders.Exists(o => o.Key == strOrderId)) return "incomplete_softbank_multipayment";

		// 未決済（LinePay）
		if (this.LinePayOrders.Exists(o => o.Key == strOrderId)) return "incomplete_linepay";

		// 未決済（ドコモケータイ払い）
		if (this.DocomoPaymentOrder.Keys.Contains(strOrderId)) return "incomplete_docomo";

		// 未決済（ZEUS LinkPoint）
		if (this.ZeusLinkPointPaymentOrder.Exists(o => (string)o[Constants.FIELD_ORDER_ORDER_ID] == strOrderId)) return "incomplete_linkpoint";

		// 未決済（AmazonPay Cv2）
		if (this.AmazonPayCv2Orders.Exists(o => o.Key == strOrderId)) return "incomplete_amazonpaycv2";

		// 携帯電話番号入力中
		if (this.YamatoKaSmsInputTelNumOrders.Exists(o => o == strOrderId)) return "under_input_telnum_yamato_ka_sms";

		// SMS認証中（ヤマト後払いSMS認証）
		if (this.YamatoKaSmsExaminationOrders.Exists(o => o == strOrderId)) return "under_examination_yamato_ka_sms";

		// 未決済（ヤマト後払いSMS認証）
		if (this.YamatoKaSmsOrders.Exists(o => (o.Key == strOrderId))) return "incomplete_yamato_ka_sms";

		// キャンセル注文
		if (this.CancelOrders.Exists(o => (string)o[Constants.FIELD_ORDER_ORDER_ID] == strOrderId)) return "cancel";

		// 未決済（Zcom 3Dセキュア）
		if (this.ZcomCard3DSecurePaymentOrder
			.Exists(order => StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]) == strOrderId))
		{
			return "incomplete_zcom_3dsecure";
		}

		// ベリトランス注文
		if (this.VeriTrans3DSecurePaymentOrder
			.Exists(order => StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]) == strOrderId))
		{
			return "incomplete_veritrans_3d_secure";
		}

		// ペイジェントクレカ
		if (this.Paygent3DSecurePaymentOrders
			.Exists(order => StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]) == strOrderId))
		{
			return "incomplete_paygent_3dsecure";
		}

		// 未決済（PayPay）
		if (this.PaypayOrders.Keys.Contains(strOrderId)) return "incomplete_paypay";

		// Boku
		if (this.BokuPaymentOrders.Exists(o => (o.Key == strOrderId))) return "incomplete_boku";

		// GMOアトカラ
		if (this.GmoAtokaraOrders.Exists(o => (o.Key == strOrderId))) return "incomplete_gmoatokara";

		// Paidy paygent
		if (this.PaidyPaygentOrders.ContainsKey(strOrderId)) return "incomplete_paidy_paygent";

		// 注文失敗
		return failure;
	}

	/// <summary>
	/// 決済遷移可能かチェック
	/// </summary>
	/// <param name="orderId">注文ID</param>
	/// <returns></returns>
	protected bool GetCanSettlement(string orderId)
	{
		switch (GetSettlementStatus(orderId))
		{
			case "incomplete_3dsecure":
			case "incomplete_rakuten_3dsecure":
			case "incomplete_softbank_multipayment":
			case "incomplete_linepay":
			case "incomplete_linkpoint":
			case "incomplete_amazonpaycv2":
			case "incomplete_yamato_ka_sms":
			case "incomplete_zcom_3dsecure":
			case "incomplete_veritrans_3d_secure":
			case "incomplete_paygent_3dsecure":
			case "incomplete_paypay":
			case "incomplete_boku":
			case "incomplete_gmoatokara":
			case "incomplete_paidy_paygent":
				return true;
		}
		return false;
	}

	/// <summary>
	/// SMS認証を行う電話番号取得
	/// </summary>
	/// <param name="cart">カート情報</param>
	/// <returns>電話番号</returns>
	protected string GetTelNumForSmsSend(CartObject cart)
	{
		if ((cart.Payment != null)
			&& (cart.Payment.UserCreditCard != null)
			&& (cart.Payment.UserCreditCard.CooperationType
				== Constants.FLG_USERCREDITCARD_COOPERATION_TYPE_YAMATOKASMS))
		{
			return cart.Payment.UserCreditCard.CooperationId;
		}

		return string.Empty;
	}

	/// <summary>
	/// Go payment Zcom 3DSecure
	/// </summary>
	/// <param name="order">Order</param>
	private void GoPaymentZcom3DSecure(Hashtable order)
	{
		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_PAYMENT_ZCOM_CARD_3DSECURE_GET_RESULT;

		var accessUrl = StringUtility.ToEmpty(order[ZcomConst.PARAM_ACCESS_URL]);
		var transCodeHash = StringUtility.ToEmpty(order[ZcomConst.PARAM_TRANS_CODE_HASH]);
		var paymentCode = StringUtility.ToEmpty(order[ZcomConst.PARAM_PAYMENT_CODE]);
		var mode = StringUtility.ToEmpty(order[ZcomConst.PARAM_MODE]);
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_PAYMENT_ZCOM_CARD_3DSECURE_POST)
			.AddParam(ZcomConst.PARAM_ACCESS_URL, accessUrl)
			.AddParam(ZcomConst.PARAM_TRANS_CODE_HASH, transCodeHash)
			.AddParam(ZcomConst.PARAM_PAYMENT_CODE, paymentCode)
			.AddParam(ZcomConst.PARAM_MODE, mode)
			.CreateUrl();

		Response.Redirect(url);
	}

	/// <summary>
	/// ベリトランス3Dセキュアへ遷移
	/// </summary>
	private void GoPaymentVeriTrans3DSecure()
	{
		var responseContents = this.VeriTrans3DSecurePaymentOrder[0][VeriTransConst.RESPONSE_CONTENTS];
		Session[VeriTransConst.RESPONSE_CONTENTS] = responseContents;

		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_PAYMENT_VERITRANS_CARD_3DSECURE_GET_POST).CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// セッションパラメータ復元
	/// </summary>
	/// <param name="param">セッションパラメータ</param>
	private void RestoreSession(Hashtable param)
	{
		this.YamatoKaSmsOrders = (List<KeyValuePair<string, Hashtable>>)param["order_yamato_ka_sms_order"];

		if (param["order_yamato_ka_sms_input_telnum_orders"] == null) param["order_yamato_ka_sms_input_telnum_orders"] = new List<string>();

		this.YamatoKaSmsInputTelNumOrders = (List<string>)param["order_yamato_ka_sms_input_telnum_orders"];

		if (param["order_yamato_ka_sms_examination_order"] == null) param["order_yamato_ka_sms_examination_order"] = new List<string>();

		this.YamatoKaSmsExaminationOrders = (List<string>)param["order_yamato_ka_sms_examination_order"];
		// 注文情報
		this.SuccessOrder = (List<Hashtable>)param["order"];
		this.ZeusCard3DSecurePaymentOrder = (List<Hashtable>)param["zeus_order_3dsecure"];
		this.RakutenCard3DSecurePaymentOrder = (List<Hashtable>)param["rakuten_order_3dsecure"];
		this.ZeusLinkPointPaymentOrder = (List<Hashtable>)param["zeus_linkpoint"];
		this.CancelOrders = (List<Hashtable>)param["cancel_orders"];
		this.SBPSMultiPaymentOrders = (List<KeyValuePair<string, Hashtable>>)param["order_sbps_multi"];
		this.LinePayOrders = (List<KeyValuePair<string, Hashtable>>)param["order_linepay"];
		this.DocomoPaymentOrder = (Dictionary<string, CartObject>)param["order_docomo"];
		this.AmazonPayCv2Orders = (List<KeyValuePair<string, Hashtable>>)param["order_amazonpay_cv2"];
		this.ZcomCard3DSecurePaymentOrder = (List<Hashtable>)param[Constants.SESSION_KEY_PAYMENT_CREDIT_ZCOM_ORDER_3DSECURE];
		this.VeriTrans3DSecurePaymentOrder = (List<Hashtable>)param["veritrans_order_3dsecure"];
		this.Paygent3DSecurePaymentOrders = (List<Hashtable>)param["paygent_order_3dsecure"];
		this.PaypayOrders = (Dictionary<string, Hashtable>)param["paypay_order"];
		this.DisablePaymentCancelOrderId = (string)param["disable_payment_cancel_order_id"];
		this.GmoCard3DSecurePaymentOrder = (List<Hashtable>)param["gmo_order_3dsecure"];
		this.OrderedCartList = (Dictionary<string, CartObject>)param["carts"];
		this.BokuPaymentOrders = (List<KeyValuePair<string, Hashtable>>)param["order_boku"];
		this.YamatoKwc3DSecureOrders = (List<Hashtable>)param[Constants.SESSION_KEY_PAYMENT_CREDIT_YAMATOKWC_ORDER_3DSECURE];
		this.GmoAtokaraOrders = (List<KeyValuePair<string, Hashtable>>)param["order_gmoatokara"];
		this.PaidyPaygentOrders = (Dictionary<string, Hashtable>)param["paidy_paygent_order"];
	}

	/// <summary>
	/// GMOマルチペイメント マルチ決済画面へ
	/// </summary>
	/// <param name="orderId">注文ID</param>
	private void GoPaypayPayment(string orderId)
	{
		var order = new OrderService().Get(orderId);
		var paypayFacade = new PaypayGmoFacade();
		var landingCartSessionKey = (string)Session[Constants.SESSION_KEY_LANDING_CART_SESSION_KEY];
		var cart = this.CartList.Cast<CartObject>().First(c => (c.OrderId == orderId));

		// すでに決済注文IDが発行されている場合は注文を失敗させる
		if (string.IsNullOrEmpty(order.PaymentOrderId) == false)
		{
			PaypayPaymentFailure(
				cart,
				order,
				false,
				string.IsNullOrEmpty(landingCartSessionKey) == false,
				WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PAYPAY_REPEATED_REQUEST_ERROR));
		}
		order.PaymentOrderId = OrderCommon.CreatePaymentOrderId(Constants.CONST_DEFAULT_SHOP_ID);

		if (landingCartSessionKey != null) this.CartList = (CartObjectList)Session[landingCartSessionKey];
		var result = paypayFacade.ExecPayment(cart, order);

		if (result.Result.HasFlag(Results.Failed))
		{
			PaypayPaymentFailure(
				cart,
				order,
				result.Result.HasFlag(Results.PreOrderRollbackIsRequired),
				string.IsNullOrEmpty(landingCartSessionKey) == false,
				result.ErrorMessage);
		}

		// GMO側の決済画面へ
		// セッション保存してリダイレクト
		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_SETTLEMENT;
		SessionSecurityManager.SaveSesstionContetnsToDatabaseForGoToOtherSite(Session, orderId);
		Response.Redirect(result.RedirectUrl);
	}

	/// <summary>
	/// PayPay決済失敗処理
	/// </summary>
	/// <param name="cart">カート</param>
	/// <param name="order">注文情報</param>]
	/// <param name="isRollback">仮注文ロールバックするか</param>
	/// <param name="isLandingCart">LandingCartか</param>
	/// <param name="errorMessage">エラーメッセージ</param>
	private void PaypayPaymentFailure(
		CartObject cart,
		OrderModel order,
		bool isRollback,
		bool isLandingCart,
		string errorMessage)
	{
		if (isRollback)
		{
			OrderCommon.RollbackPreOrder(
				order.DataSource,
				cart,
				((this.IsLoggedIn == false) && this.CartList.IsFirstCart(cart)),
				0,
				this.IsLoggedIn,
				UpdateHistoryAction.Insert);
		}

		if (this.PaypayOrders != null) this.PaypayOrders.Remove(order.OrderId);

		Session[Constants.SESSION_KEY_ERROR_FOR_PAYPAY_PAYMENT] = errorMessage;
		var urlNextPage = isLandingCart
			? Constants.PAGE_FRONT_LANDING_LANDING_CART_CONFIRM
			: Constants.PAGE_FRONT_ORDER_CONFIRM;

		if (isLandingCart)
		{
			var landingCartNextPageForCheck = string.Format(
				"{0}{1}",
				Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK,
				Session[Constants.SESSION_KEY_LANDING_CART_INPUT_ABSOLUTE_PATH]);
			Session[landingCartNextPageForCheck] = urlNextPage;
		}
		else
		{
			Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = urlNextPage;
		}

		this.CartList.Items.RemoveAll(item => item.IsOrderDone);
		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = urlNextPage;
		Response.Redirect(new UrlCreator(Constants.PATH_ROOT + urlNextPage).CreateUrl());
	}

	/// <summary>
	/// セッションのカート情報に同梱商品を追加
	/// </summary>
	private void AddBundledProductToCartList()
	{
		string userId = (this.IsLoggedIn)
			? this.LoginUserId
			: UserService.CreateNewUserId(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.NUMBER_KEY_USER_ID,
				Constants.CONST_USER_ID_HEADER,
				Constants.CONST_USER_ID_LENGTH);

		// 仮注文作成済みのため、自分の注文IDは除外
		var excludeOrderIds = this.CartList.Items.Select(item => item.OrderId).ToList();
		if (this.IsOrderCombined)
		{
			excludeOrderIds.AddRange(this.CartList.Items.Select(item => item.OrderCombineParentOrderId));
		}

		var productBundler = new ProductBundler(
			this.CartList.Items,
			userId,
			SessionManager.AdvCodeFirst,
			SessionManager.AdvCodeNow,
			excludeOrderIds.ToArray(),
			this.LoginUserHitTargetListIds,
			true);

		// カート情報セッションに同梱商品をセット
		SessionManager.CartList.Items.Select(item => item.Items)
			.Union(productBundler.CartList.Select(item => item.Items));
	}

	/// <summary>
	/// Exec boku optin flow
	/// </summary>
	/// <param name="order">Order</param>
	private void ExecBokuOptinFlow(Hashtable order)
	{
		var param = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
		RestoreSession(param);

		// LPカートからの遷移だったらカートリスト再取得
		var landingCartSessionKey = (string)Session[Constants.SESSION_KEY_LANDING_CART_SESSION_KEY];
		if (landingCartSessionKey != null) this.CartList = (CartObjectList)Session[landingCartSessionKey];

		var optinId = StringUtility.ToEmpty(order["optin_id"]);
		if (string.IsNullOrEmpty(optinId))
		{
			var orderId = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]);
			var paymentId = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN]);
			var forwardUrl = new UrlCreator(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SETTLEMENT)
				.AddParam(Constants.REQUEST_KEY_ORDER_ID, orderId)
				.CreateUrl();
			var hasError = false;

			// Call optin
			var optin = new PaymentBokuOptinApi().Exec(
				StringUtility.ToEmpty(order[Constants.FIELD_USER_ADDR_COUNTRY_ISO_CODE]),
				forwardUrl);

			if (optin == null)
			{
				hasError = true;
				this.BokuPaymentErrorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_BOKU_PAYMENT_ERROR);
				AppLogger.WriteError(
					string.Format(
						"{0} Payment Error: Order ID ={1}",
						BokuConstants.CONST_BOKU_PAYMENT_METHOD_CARRIERBILLING,
						orderId));
			}
			else if (optin.IsSuccess == false)
			{
				hasError = true;
				this.BokuPaymentErrorMessage = optin.Result.Message;
			}

			if (hasError) ExecBokuErrorProcess(order);

			// Redirect to optin url
			this.BokuPaymentOrders.Find(o => (o.Key == orderId)).Value["optin_id"] = optin.OptinId;
			SessionSecurityManager.SaveSesstionContetnsToDatabaseForGoToOtherSite(Session, orderId);
			Response.Redirect(optin.Hosted.OptinUrl);
		}
		else
		{
			var hasError = false;
			var validate = new PaymentBokuValidateOptinApi().Exec(optinId);

			if (validate == null)
			{
				hasError = true;
				this.BokuPaymentErrorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_BOKU_PAYMENT_ERROR);
				AppLogger.WriteError(
					string.Format(
						"{0} Payment Error: Order ID ={1}",
						order[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN],
						order[Constants.FIELD_ORDER_ORDER_ID]));
			}
			else if (validate.IsSuccess == false)
			{
				hasError = true;
				this.BokuPaymentErrorMessage = validate.Result.Message;
			}
			else
			{
				var response = new PaymentBokuConfirmOptinApi().Exec(optinId);

				if (response == null)
				{
					hasError = true;
					this.BokuPaymentErrorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_BOKU_PAYMENT_ERROR);
					AppLogger.WriteError(
						string.Format(
							"{0} Payment Error: Order ID ={1}",
							order[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN],
							order[Constants.FIELD_ORDER_ORDER_ID]));
				}
				else if (response.IsSuccess == false)
				{
					hasError = true;
					this.BokuPaymentErrorMessage = response.Result.Message;
				}
			}

			if (hasError) ExecBokuErrorProcess(order);

			ExecBokuCharge(order);
		}
	}

	/// <summary>
	/// Exec boku charge
	/// </summary>
	/// <param name="order">Order</param>
	private void ExecBokuCharge(Hashtable order)
	{
		var hasError = false;
		var optinId = StringUtility.ToEmpty(order["optin_id"]);
		var orderId = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]);
		var fixedPurchaseId = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_FIXED_PURCHASE_ID]);
		var fixedPurchase = DomainFacade.Instance.FixedPurchaseService.Get(fixedPurchaseId);
		var productNames = string.Join(
			",",
			this.CartList.Items
				.FirstOrDefault(cart => (cart.OrderId == orderId))
				.Items.Select(item => item.ProductName));
		var currency = string.IsNullOrEmpty(StringUtility.ToEmpty(order[Constants.FIELD_USER_DISP_CURRENCY_CODE]))
			? BokuConstants.CONST_BOKU_DEFAULT_CURRENCY_CODE
			: (string)order[Constants.FIELD_USER_DISP_CURRENCY_CODE];
		var charge = new PaymentBokuChargeApi().Exec(
			currency,
			Guid.NewGuid().ToString(),
			productNames,
			orderId,
			optinId,
			StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_PRICE_TOTAL]),
			(TaxCalculationUtility.GetPrescribedOrderTaxIncludedFlag() == Constants.FLG_ORDER_ORDER_TAX_INCLUDED_PRETAX),
			StringUtility.ToEmpty(order[Constants.FIELD_USER_REMOTE_ADDR]),
			(fixedPurchase != null),
			false,
			(fixedPurchase != null) ? fixedPurchase.FixedPurchaseKbn : string.Empty,
			(fixedPurchase != null) ? fixedPurchase.FixedPurchaseSetting1 : string.Empty);

		if (charge == null)
		{
			hasError = true;
			this.BokuPaymentErrorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_BOKU_PAYMENT_ERROR);
			AppLogger.WriteError(
				string.Format(
					"{0} Payment Error: Order ID ={1}",
					order[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN],
					order[Constants.FIELD_ORDER_ORDER_ID]));
		}
		else if ((charge.IsSuccess == false)
			|| (charge.ChargeStatus != BokuConstants.CONST_BOKU_CHARGE_STATUS_SUCCESS))
		{
			this.BokuPaymentErrorMessage = (charge.ChargeStatus == BokuConstants.CONST_BOKU_CHARGE_STATUS_IN_PROGRESS)
				? WebMessages.GetMessages(WebMessages.ERRMSG_BOKU_PAYMENT_PROCESS_TIME_OUT)
				: charge.Result.Message;
			hasError = true;
		}

		OrderCommon.AppendExternalPaymentCooperationLog(
			charge.IsSuccess,
			orderId,
			LogCreator.CreateMessage(orderId, charge.OptinId),
			Constants.FLG_LASTCHANGED_USER,
			UpdateHistoryAction.DoNotInsert);

		// Exec error process
		if (hasError) ExecBokuErrorProcess(order);

		// Exec complete order
		var paymentMemo = OrderCommon.CreateOrderPaymentMemoForAuth(
			orderId,
			charge.OptinId,
			StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN]),
			charge.ChargeId,
			decimal.Parse(StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_PRICE_TOTAL])));
		order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = charge.OptinId;
		order[Constants.FIELD_ORDER_CARD_TRAN_ID] = charge.ChargeId;
		order[Constants.FIELD_ORDER_PAYMENT_MEMO] = paymentMemo;

		if (string.IsNullOrEmpty(StringUtility.ToEmpty(order[Constants.FIELD_ORDER_FIXED_PURCHASE_ID])) == false)
		{
			DomainFacade.Instance.FixedPurchaseService.SetExternalPaymentAgreementId(
				StringUtility.ToEmpty(order[Constants.FIELD_ORDER_FIXED_PURCHASE_ID]),
				charge.OptinId,
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.Insert,
				fixedPurchaseHistoryKbn: Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_CONTINUOUS_ORDER_REGISTER);
		}

		OrderComplete(order, false);

		this.BokuPaymentOrders.Remove(
			this.BokuPaymentOrders.Find(o => o.Key == orderId));
		Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SETTLEMENT);
	}

	/// <summary>
	/// Exec boku error process
	/// </summary>
	/// <param name="order">Order</param>
	private void ExecBokuErrorProcess(Hashtable order)
	{
		var orderId = StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]);
		var cart = this.CartList.Items.Find(item => (item.OrderId == orderId));
		this.BokuPaymentOrders.Remove(this.BokuPaymentOrders.Find(o => (o.Key == orderId)));

		// Rollback order
		if (OrderCommon.GetOrder(orderId).Count != 0)
		{
			var isUserDelete = (((this.IsLoggedIn == false) || (this.RegisterUser != null))
				&& (this.SuccessOrder.Count == 0)
				&& (new OrderService().GetOrdersByUserId(cart.OrderUserId).Length == 1));

			OrderCommon.RollbackPreOrder(
				order,
				cart,
				isUserDelete,
				(int)order[Constants.FIELD_USERSHIPPING_SHIPPING_NO],
				this.IsLoggedIn,
				UpdateHistoryAction.Insert);

			if (Constants.LANDING_CART_USER_REGISTER_WHEN_ORDER_COMPLETE && isUserDelete && this.IsLoggedIn)
			{
				this.LoginUserId = null;
			}
		}

		// Redirect to next page
		Session[Constants.SESSION_KEY_ERROR_MSG] = this.BokuPaymentErrorMessage;
		var nextPage = string.Empty;
		var hasUnsettledExternalPayment =
			((this.SBPSMultiPaymentOrders.Count
				+ this.ZeusCard3DSecurePaymentOrder.Count
				+ this.LinePayOrders.Count
				+ this.BokuPaymentOrders.Count) > 0);

		if (hasUnsettledExternalPayment || (this.SuccessOrder.Count > 1))
		{
			nextPage = Constants.PAGE_FRONT_ORDER_SETTLEMENT;
		}
		else
		{
			nextPage = (order[Constants.SESSION_KEY_LANDING_CART_SESSION_KEY] != null)
				? Constants.PAGE_FRONT_LANDING_LANDING_CART_CONFIRM
				: Constants.PAGE_FRONT_ORDER_CONFIRM;
		}

		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = nextPage;
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR)
			.AddParam(
				Constants.REQUEST_KEY_BACK_URL,
				(Constants.PATH_ROOT + nextPage))
			.CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// Get boku order
	/// </summary>
	/// <returns>Boku order</returns>
	public Hashtable GetBokuOrder()
	{
		var bokuPaymentOrders = (List<KeyValuePair<string, Hashtable>>)((Hashtable)Session[Constants.SESSION_KEY_PARAM])["order_boku"];
		if (bokuPaymentOrders == null) return null;
		var bokuOrder = bokuPaymentOrders.Find(order => (order.Key == Request[Constants.REQUEST_KEY_ORDER_ID])).Value;
		return bokuOrder;
	}

	/// <summary>
	/// VeriTrans PayPay transition
	/// </summary>
	/// <param name="orderId">Order id</param>
	private void GoPaymentVeriTransPayPay(string orderId)
	{
		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_PAYMENT_VERITRANS_PAYPAY_RECEIVE;

		var order = this.PaypayOrders.First(o => o.Key == orderId).Value;
		var cart = this.CartList.Items.FirstOrDefault(co => co.OrderId == orderId);
		var paymentOrderId = OrderCommon.CreatePaymentOrderId(cart.ShopId);
		order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = paymentOrderId;
		order[Constants.FIELD_ORDER_CARD_TRAN_ID] = paymentOrderId;
		var authResult = new PaymentVeritransPaypay().Authorize(
			new OrderModel(order),
			cart);
		if (authResult.Mstatus != VeriTransConst.RESULT_STATUS_OK)
		{
			AppLogger.WriteError("ベリトランスPayPay決済エラー：注文ID=" + orderId);

			// ロールバック対象以外に注文がない注文時会員登録ユーザーを削除
			var isUserDelete = (((this.IsLoggedIn == false) || (this.RegisterUser != null))
				&& (this.SuccessOrder.Count == 0)
				&& (new OrderService().GetOrdersByUserId(cart.OrderUserId).Length == 1));

			// 注文ロールバック
			OrderCommon.RollbackPreOrder(
				order,
				cart,
				isUserDelete,
				(int)order[Constants.FIELD_USERSHIPPING_SHIPPING_NO],
				this.IsLoggedIn,
				UpdateHistoryAction.Insert);

			if (Constants.LANDING_CART_USER_REGISTER_WHEN_ORDER_COMPLETE && isUserDelete && this.IsLoggedIn)
			{
				this.LoginUserId = null;
			}

			// エラーメッセージを格納
			SessionManager.MessageForErrorPage =
				WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_AUTH_ERROR);

			if (this.PaypayOrders != null)
			{
				this.PaypayOrders.Remove(cart.OrderId);
			}

			// 遷移先画面決定（まだ外部決済が残っている or 成功注文があるなら 決済ページへ、それ以外は 注文確認画面へ戻る）
			SessionManager.NextPageForCheck = ((this.HasUnsettledExternalPayments) || (this.SuccessOrder.Count > 0))
				? Constants.PAGE_FRONT_ORDER_SETTLEMENT
				: (order[Constants.SESSION_KEY_LANDING_CART_SESSION_KEY] != null)
					? Constants.PAGE_FRONT_LANDING_LANDING_CART_CONFIRM
					: Constants.PAGE_FRONT_ORDER_CONFIRM;

			// エラー画面遷移
			var errorUrl = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR)
				.AddParam(
					Constants.REQUEST_KEY_BACK_URL,
					this.SecurePageProtocolAndHost + Constants.PATH_ROOT + SessionManager.NextPageForCheck)
				.CreateUrl();
			Response.Redirect(errorUrl);
		}

		Session[VeriTransConst.RESPONSE_CONTENTS] = authResult.ResponseContents;
		// セッション保存してリダイレクト
		SessionSecurityManager.SaveSesstionContetnsToDatabaseForGoToOtherSite(Session, orderId);
		var nextUrl = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_PAYMENT_VERITRANS_PAYPAY_AUTH)
			.CreateUrl();
		Response.Redirect(nextUrl);
	}

	/// <summary>
	/// Execute paidy paygent order
	/// </summary>
	/// <param name="orderId">Order id</param>
	/// <param name="isSkip">Is skip pop up</param>
	private void ExecutePaidyPaygentOrder(string orderId, bool isSkip = false)
	{
		var paygentApi = new PaygentApiFacade();
		if (this.IsOrderCombined)
		{
			var parentOrderId = StringUtility.ToEmpty(this.PaidyPaygentOrders[orderId][Constants.FIELD_ORDER_COMBINED_ORG_ORDER_IDS]);
			var parentOrder = new OrderService().GetOrderForOrderCancel(parentOrderId);
			if (parentOrder == null)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CANCEL_ORDER_FOR_COMBINE_FAIL);
				this.PaidyPaygentOrders.Remove(orderId);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}
			var paygentPaymentInformation = paygentApi.GetPaymentInformationInquiry(
				StringUtility.ToEmpty(parentOrder[0][Constants.FIELD_ORDER_CARD_TRAN_ID]));

			if ((paygentPaymentInformation.IsSuccess == false)
				|| (paygentPaymentInformation.Response.PaymentStatus != PaygentConstants.PAYGENT_API_RESPONSE_PAYMENT_STATUS_AUTH_COMP))
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_CANCEL_PAYMENT_FAILED);
				this.PaidyPaygentOrders.Remove(orderId);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}
		}

		if (this.WhfPaidyStatus.Value == Constants.FLG_PAYMENT_PAIDY_API_STATUS_REJECTED)
		{
			this.PaidyPaygentOrders.Remove(orderId);
			Response.Redirect(this.RawUrl);
			return;
		}

		var order = this.PaidyPaygentOrders[orderId];
		var cart = this.CartList.Items.FirstOrDefault(co => co.OrderId == orderId);
		this.TransactionName = "2-7-B.Paidy決済処理";
		var paidyPaymentId = isSkip
			? cart.Payment.CardTranId
			: this.WhfPaidyPaymentId.Value;
		var paidyAuthorizationResult = new PaygentApiFacade().PaidyAuthorize(paidyPaymentId);
		var errorMessage = string.Empty;
		if (paidyAuthorizationResult.IsSuccess)
		{
			order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = isSkip
				? cart.Payment.CardTranId
				: this.WhfPaidyPaymentId.Value;
		}
		else
		{
			// remaks:Paygentからの検証電文受信時には、Paidyへ既に与信を取得している状況のため、どんなエラーが返ってきても仮注文はロールバックしない。
			// 与信失敗系のエラーハンドリングは、Paidyチェックアウト処理(PaymentPaygentPaymentRecv.aspx)で既に行われているため。
			var isUserDelete = (((this.IsLoggedIn == false) || (this.RegisterUser != null))
				&& (this.SuccessOrder.Count == 0)
				&& (new OrderService().GetOrdersByUserId(cart.OrderUserId).Length == 1));

			if (Constants.LANDING_CART_USER_REGISTER_WHEN_ORDER_COMPLETE
				&& isUserDelete
				&& this.IsLoggedIn)
			{
				this.LoginUserId = null;
			}

			this.PaidyPaygentOrders.Remove(orderId);

			errorMessage = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_FAIL_PAIDY_AUTHORIZED);
			FileLogger.Write("PaidyError", errorMessage);

			// 遷移先画面決定（まだ外部決済が残っている or 成功注文があるなら 決済ページへ、それ以外は 注文確認画面へ戻る）
			SessionManager.NextPageForCheck = ((this.HasUnsettledExternalPayments) || (this.SuccessOrder.Count > 0))
				? Constants.PAGE_FRONT_ORDER_SETTLEMENT
				: (order[Constants.SESSION_KEY_LANDING_CART_SESSION_KEY] != null)
					? Constants.PAGE_FRONT_LANDING_LANDING_CART_CONFIRM
					: Constants.PAGE_FRONT_ORDER_CONFIRM;
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;

			// エラー画面遷移
			var errorUrl = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR)
				.AddParam(
					Constants.REQUEST_KEY_BACK_URL,
					this.SecurePageProtocolAndHost + Constants.PATH_ROOT + SessionManager.NextPageForCheck)
				.CreateUrl();

			Response.Redirect(errorUrl);
		}

		var dictionary = new Dictionary<string, string>
		{
			{ Constants.FIELD_ORDER_ORDER_ID, StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]) },
			{ Constants.FIELD_ORDER_EXTERNAL_ORDER_ID, StringUtility.ToEmpty(order[Constants.FIELD_ORDER_EXTERNAL_ORDER_ID]) },
		};

		OrderComplete(order, false);
		this.PaidyPaygentOrders.Remove(orderId);
		Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SETTLEMENT);
	}

	/// <summary>
	/// Generate paidy paygent script
	/// </summary>
	/// <param name="cart">Cart</param>
	/// <returns>Paidy paygent script</returns>
	protected string GeneratePaidyPaygentScript(CartObject cart, RepeaterItem repeaterItem)
	{
		var saleButtonId = repeaterItem.FindControl("lbPaidySettlement").ClientID;
		if (cart.Payment.IsPaymentPaygentPaidy)
		{
			var script = new StringBuilder()
				.AppendFormat(
					"var body = {0}{1};",
					new PaidyCheckout(cart).CreateParameterForPaidyCheckout(),
					Environment.NewLine)
				.AppendFormat(
					"lbNextProcess = '{0}'{1};",
					saleButtonId,
					Environment.NewLine)
				.Append("customBody = body;")
				.Append("PaidyPayProcess(body);");
			return string.Concat("(function() {", script.ToString().Replace("\"", "\'"), "})(); return false;");
		}

		return "return true;";
	}

	/// <summary>
	/// Paidy settlement click
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbPaidySettlement_Click(object sender, EventArgs e)
	{
		var rOrderItem = GetParentRepeaterItem((LinkButton)sender, "rOrder");
		var index = ((HiddenField)(rOrderItem.FindControl("hfCartIndex"))).Value;
		var wlOrderId = GetWrappedControl<WrappedLiteral>(WrOrder.Items[int.Parse(index)], "lOrderId");
		ExecutePaidyPaygentOrder(wlOrderId.Text);
	}

	/// <summary>
	/// GMOアトカラ決済ページへ
	/// </summary>
	/// <param name="orderId">注文ID</param>
	private void GoPaymentGmoAtokara(string orderId)
	{
		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_PAYMENT_GMO_ATOKARA_RESULT;

		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_PAYMENT_GMO_ATOKARA_POST)
			.AddParam(Constants.REQUEST_KEY_ORDER_ID, orderId)
			.CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>成功注文リスト</summary>
	private List<Hashtable> SuccessOrder { get; set; }
	/// <summary>キャンセル注文リスト</summary>
	private List<Hashtable> CancelOrders { get; set; }
	/// <summary>ZEUS 3Dセキュア決済注文リスト</summary>
	private List<Hashtable> ZeusCard3DSecurePaymentOrder { get; set; }
	/// <summary>楽天 3Dセキュア決済注文リスト</summary>
	private List<Hashtable> RakutenCard3DSecurePaymentOrder { get; set; }
	/// <summary>ZEUS LinkPoint決済注文リスト</summary>
	private List<Hashtable> ZeusLinkPointPaymentOrder { get; set; }
	/// <summary>ドコモケータイ払い決済注文リスト</summary>
	private Dictionary<string, CartObject> DocomoPaymentOrder { get; set; }
	/// <summary>SBPSマルチ決済注文リスト</summary>
	private List<KeyValuePair<string, Hashtable>> SBPSMultiPaymentOrders { get; set; }
	/// <summary>LINEペイ注文リスト</summary>
	private List<KeyValuePair<string, Hashtable>> LinePayOrders { get; set; }
	/// <summary>AmazonPayCv2注文リスト</summary>
	private List<KeyValuePair<string, Hashtable>> AmazonPayCv2Orders { get; set; }
	/// <summary>ヤマト後払いSMS認証決済</summary>
	private List<KeyValuePair<string, Hashtable>> YamatoKaSmsOrders { set; get; }
	/// <summary>ヤマト後払いSMS認証用電話番号入力中注文リスト</summary>
	private List<string> YamatoKaSmsInputTelNumOrders { get; set; }
	/// <summary>ヤマト後払いSMS認証注文リスト</summary>
	private List<string> YamatoKaSmsExaminationOrders { set; get; }
	/// <summary>ヤマトKWC 3Dセキュア決済注文リスト</summary>
	private List<Hashtable> YamatoKwc3DSecureOrders { get; set; }
	/// <summary>SMS認証時エラーメッセージ</summary>
	protected string SmsAuthErrorMessage { get; private set; }
	/// <summary>AmazonCv2チェックアウトセッションID</summary>
	protected string AmazonCheckoutSessionId
	{
		get { return (string)Session[AmazonCv2Constants.SESSION_KEY_AMAZON_CHECKOUT_SESSION_ID]; }
	}
	/// <summary>Zcom card 3DSecure payment order</summary>
	private List<Hashtable> ZcomCard3DSecurePaymentOrder { get; set; }
	/// <summary>ベリトランス3Dセキュア注文情報</summary>
	private List<Hashtable> VeriTrans3DSecurePaymentOrder { get; set; }
	/// <summary>ペイジェント3Dセキュア注文情報</summary>
	private List<Hashtable> Paygent3DSecurePaymentOrders { get; set; }
	/// <summary>Paypay orders</summary>
	private Dictionary<string, Hashtable> PaypayOrders { get; set; }
	/// <summary>決済キャンセル行わない注文ID</summary>
	private string DisablePaymentCancelOrderId { get; set; }
	/// <summary>GMO 3Dセキュア決済注文リスト</summary>
	private List<Hashtable> GmoCard3DSecurePaymentOrder { get; set; }
	/// <summary>Paidy paygent orders</summary>
	private Dictionary<string, Hashtable> PaidyPaygentOrders { get; set; }
	/// <summary>未決済の外部決済があるか</summary>
	protected bool HasUnsettledExternalPayments
	{
		get
		{
			return (this.SBPSMultiPaymentOrders.Count
				+ this.ZeusCard3DSecurePaymentOrder.Count
				+ this.ZeusLinkPointPaymentOrder.Count
				+ this.LinePayOrders.Count
				+ this.AmazonPayCv2Orders.Count
				+ this.ZcomCard3DSecurePaymentOrder.Count
				+ this.GmoCard3DSecurePaymentOrder.Count
				+ this.VeriTrans3DSecurePaymentOrder.Count
				+ this.RakutenCard3DSecurePaymentOrder.Count
				+ this.YamatoKaSmsOrders.Count
				+ this.YamatoKwc3DSecureOrders.Count
				+ this.Paygent3DSecurePaymentOrders.Count
				+ this.PaidyPaygentOrders.Count) != 0;
		}
	}
	/// <summary>注文時カートリスト</summary>
	private Dictionary<string, CartObject> OrderedCartList { get; set; }
	/// <summary>Boku payment orders</summary>
	private List<KeyValuePair<string, Hashtable>> BokuPaymentOrders { get; set; }
	/// <summary>Boku payment error message</summary>
	private string BokuPaymentErrorMessage { get; set; }
	/// <summary>GMOアトカラ注文</summary>
	public List<KeyValuePair<string, Hashtable>> GmoAtokaraOrders { set; get; }
}
