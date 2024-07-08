/*
=========================================================================================================
  Module      : 注文返品交換登録ページ処理(OrderReturnExchangeInput.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Input.Order.OrderReturnExchange;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment.Paygent;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.PayPal;
using w2.App.Common.Order.Payment.PayTg;
using w2.App.Common.Order.Payment.Veritrans;
using w2.App.Common.Order.Reauth;
using w2.App.Common.Order.UserCreditCardCooperationInfos;
using w2.App.Common.Product;
using w2.App.Common.Util;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Web;
using w2.Domain.Coupon;
using w2.Domain.Order;
using w2.Domain.Payment;
using w2.Domain.Point;
using w2.Domain.ProductTaxCategory;
using w2.Domain.TwOrderInvoice;
using w2.Domain.User;
using w2.Domain.UserCreditCard;

public partial class Form_Order_OrderReturnExchangeInput : OrderReturnExchangePage
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
			// コンポーネント初期化
			InitializeComponents();

			// 基本情報
			lOrderIdOrg.Text = WebSanitizer.HtmlEncode(this.OrderIdOrg);
			lSubscriptionBoxCourseId.Text = this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem()
				? HtmlSanitizer.HtmlEncode(this.ReturnExchangeOrderOrg.SubscriptionBoxCourseId)
				: HtmlSanitizer.HtmlEncodeChangeToBr(
					string.Join(Environment.NewLine, this.ReturnExchangeOrderOrg.ItemSubscriptionBoxCourseIds));
			hfCvsShopId.Value = this.ReturnExchangeOrderOrg.ShopId;

			// 返品商品作成(元注文）
			{
				// 元注文情報表示部分作成
				this.OrderOrg = new ReturnOrder(this.ReturnExchangeOrderOrg);

				if (OrderCommon.DisplayTwInvoiceInfo())
				{
					// Get TwOrderInvoice
					var twOrderInvoice = new TwOrderInvoiceService().GetOrderInvoice(
						this.OrderOrg.OrderId,
						1);

					if (twOrderInvoice != null)
					{
						// Set Taiwan Order Invoice
						this.TwOrderInvoiceInput = new TwOrderInvoiceInput(twOrderInvoice);

						this.TwOrderInvoice = twOrderInvoice;
					}

					if ((this.TwOrderInvoice != null)
						&& (this.TwOrderInvoice.TwInvoiceStatus != Constants.FLG_ORDER_INVOICE_STATUS_ISSUED_LINKED))
					{
						Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
						Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
					}
				}

				//------------------------------------------------------
				// 表示処理
				//------------------------------------------------------
				// 返品商品
				rReturnOrderItem.DataSource = this.OrderOrg.Shippings;
				this.OrderItems = this.OrderOrg.Items;
				rReturnOrderItem.DataBind();

				// 税率毎商品価格調整金を初期化したものをリピータに格納
				var orderPriceByTaxRate = this.OrderOrg.OrderPriceByTaxRate
					.Select(item =>
					{
						item.ReturnPriceCorrectionByRate = 0;
						return item;
					}).ToList();

				rPriceCorrection.DataSource = orderPriceByTaxRate;
				rPriceCorrection.DataBind();

				// ポイントOP関連
				if (Constants.W2MP_POINT_OPTION_ENABLED)
				{
					InitializePointAssignment();

					// 利用ポイント
					divOrderPointUse.Visible = this.OrderOrg.VisibleOrderPointUse;
					lLastOrderPointUseBefore.Text = WebSanitizer.HtmlEncode(StringUtility.ToNumeric(this.OrderOrg.LastOrderPointUse));
					hfLastOrderPointUse.Value = this.OrderOrg.LastOrderPointUse.ToString();
					lOrderPointUseAdjustment.Text = "0";
					tbLastOrderPointUse.Text = this.OrderOrg.LastOrderPointUse.ToString();
				}

				// 元注文合計情報
				lOrderPriceSubTotal.Text = WebSanitizer.HtmlEncode(this.OrderOrg.OrderSummaryInfo["OrderPriceSubtotal"].ToPriceString(true));
				lOrderPriceShipping.Text = WebSanitizer.HtmlEncode(this.OrderOrg.OrderSummaryInfo["OrderPriceShipping"].ToPriceString(true));
				lMemberRankDiscount.Text = WebSanitizer.HtmlEncode(this.OrderOrg.OrderSummaryInfo["MemberRankDiscount"].ToPriceString(true));
				lFixedPurchaseMemberDiscountAmount.Text = WebSanitizer.HtmlEncode(this.OrderOrg.OrderSummaryInfo["FixedPurchaseMemberDiscountAmount"].ToPriceString(true));
				lOrderPointUseYen.Text = WebSanitizer.HtmlEncode(this.OrderOrg.OrderSummaryInfo["OrderPointUseYen"].ToPriceString(true));
				lOrderCouponUse.Text = WebSanitizer.HtmlEncode(this.OrderOrg.OrderSummaryInfo["OrderCouponUse"].ToPriceString(true));
				lFixedPurchaseDiscountPrice.Text = WebSanitizer.HtmlEncode(this.OrderOrg.OrderSummaryInfo["FixedPurchaseDiscountPrice"].ToPriceString(true));
				lOrderPriceExchange.Text = WebSanitizer.HtmlEncode(this.OrderOrg.OrderSummaryInfo["OrderPriceExchange"].ToPriceString(true));
				lOrderPriceTotal.Text = WebSanitizer.HtmlEncode(this.OrderOrg.OrderSummaryInfo["OrderPriceTotal"].ToPriceString(true));
				lOrderPriceTax.Text = WebSanitizer.HtmlEncode(this.OrderOrg.OrderSummaryInfo["OrderPriceSubtotalTax"].ToPriceString(true));
				lOrderPriceRegulation.Text = WebSanitizer.HtmlEncode(this.OrderOrg.OrderSummaryInfo["OrderPriceRegulation"].ToPriceString(true));

				if (decimal.Parse(this.OrderOrg.OrderSummaryInfo["MemberRankDiscount"]) < 0) lMemberRankDiscount.CssClass = "notice";
				if (decimal.Parse(this.OrderOrg.OrderSummaryInfo["FixedPurchaseMemberDiscountAmount"]) < 0) lFixedPurchaseMemberDiscountAmount.CssClass = "notice";
				if (decimal.Parse(this.OrderOrg.OrderSummaryInfo["OrderPointUseYen"]) < 0) lOrderPointUseYen.CssClass = "notice";
				if (decimal.Parse(this.OrderOrg.OrderSummaryInfo["OrderCouponUse"]) < 0) lOrderCouponUse.CssClass = "notice";

				rSetPromotionProductDiscount.DataSource
					= rSetPromotionShippingChargeDiscount.DataSource
					= rSetPromotionPaymentChargeDiscount.DataSource
					= this.ReturnExchangeOrderOrg.SetPromotions;
				rSetPromotionProductDiscount.DataBind();
				rSetPromotionShippingChargeDiscount.DataBind();
				rSetPromotionPaymentChargeDiscount.DataBind();
			}

			// 返品商品作成（交換済み注文）
			var exchangedOrderIds = new OrderService().GetExchangedOrderIds(this.ReturnExchangeOrderOrg.ShopId, this.OrderIdOrg);
			var orderReturnInfos = exchangedOrderIds
				.Select(orderId => new OrderService().GetExchangedOrdersInDataView(this.ReturnExchangeOrderOrg.ShopId, orderId, this.OrderIdOrg))
				.Where(x => (x.Count > 0))
				.Select(x => new Order(x))
				.Select(x => new ReturnOrder(x))
				.ToArray();
			rExchangedOrder.DataSource = orderReturnInfos;
			rExchangedOrder.DataBind();

			// 最終請求金額
			rReturnExchangeLastBilledAmountBeforeByTaxRate.DataSource = this.ReturnExchangeOrderOrg.OrderPriceByTaxRate;
			rReturnExchangeLastBilledAmountBeforeByTaxRate.DataBind();

			// 元注文情報（元注文 or 最後の返品注文）取得
			var order =
				new OrderService().GetRelatedOrders(this.OrderIdOrg)
				.Where(o => (o.IsOriginalOrder || o.IsReturnOrder))
				.OrderByDescending(o => o.OrderId).FirstOrDefault();
			// 決済種別をセット
			// ※決済無しの場合は、ブランクをデフォルトとする
			// ※注文決済がAmazon(CV1)かつ、CV2オプションが有効の場合、Amazon(CV2)を選択。
			ddlOrderPaymentKbn.SelectedValue =
				(order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT)
				? string.Empty
				: ((order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT)
					&& Constants.AMAZON_PAYMENT_CV2_ENABLED)
					? Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2
				: order.OrderPaymentKbn;

			// 決済種別イベント実行
			ddlOrderPaymentKbn_OnSelectedIndexChanged(sender, e);

			// クレジットカード表示切り替え
			DisplayCreditInputForm();
			// データバインド
			//DataBind();

			if (this.IsUserPayTg && Constants.PAYMENT_SETTING_PAYTG_MOCK_ENABLED && (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten))
			{
				//PayTg端末状態取得
				GetPayTgDeviceStatus();
			}
		}

		this.Input = CreateInput();

		// トークンが入力されていたら入力画面を切り替える
		SwitchDisplayForCreditTokenInput();
		// Check error when back from browser
		if (((string)Session[Constants.SESSION_KEY_ORDER_REGIST_INPUT_ERROR]).Length != 0)
		{
			Response.Redirect(CreateOrderDetailUrl(Request[Constants.FIELD_ORDER_ORDER_ID], false, (this.IsPopUp)));
		}

		// 返品交換区分による表示制御
		tbdyPaymentKbnCredit.Visible = Constants.PAYMENT_REAUTH_ENABLED
			&& (this.ReturnExchangeOrderOrg.IsPermitReauthOrderSiteKbn)
			&& this.IsAllLastAuthFlgValid
			&& (trOrderPaymentKbn.Visible
			&& (ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
			&& Constants.REAUTH_COMPLETE_CREDITCARD_LIST.Contains(Constants.PAYMENT_CARD_KBN));
		divExchangeOrderItem.Visible =
			(rblReturnExchangeKbn.SelectedValue == Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE);

		// ポップアップ表示制御（タイトルを非表示へ）
		trTitleOrderTop.Visible
			= trTitleOrderMiddle.Visible
			= trTitleOrderBottom.Visible
			= (this.IsPopUp == false);

		// 自動計算表示切替
		cbOrderPointAddReCalculate_CheckedChanged(sender, e);
		cbOrderPriceCorrectionReCalculate_CheckedChanged(sender, e);

		// 支払方法制限メッセージ表示
		DisplayLimitedPaymentMessages();
		DisplayLimitedPaymentUserManagementLevel();
		DisplayLimitedPaymentOrderOwnerKbn();

		// 返金メモ入力欄の表示
		trRepaymentMemoFreeText.Visible =
			(rblRepaymentMemoType.SelectedValue == Constants.FLG_ORDER_REPAYMENT_MEMO_TYPE_FREE_TEXT);
		tbodyRepaymentBankText.Visible =
			(rblRepaymentMemoType.SelectedValue == Constants.FLG_ORDER_REPAYMENT_MEMO_TYPE_REPAYMENT_BANK_TEXT);
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// 外部決済連携
		ddlExecuteType.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ORDER, "execute_types"));
		ddlExecuteType.Visible = false;

		//------------------------------------------------------
		// 基本情報
		//------------------------------------------------------
		// 返品交換区分
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_RETURN_EXCHANGE_KBN))
		{
			if (li.Value != Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_UNKNOWN)	// 「指定無し」は追加しない
			{
				if ((li.Value == Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE)
					&& ((this.ReturnExchangeOrderOrg.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO) || (this.ReturnExchangeOrderOrg.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)))
				{ 
					// can not exchange if use GMO payment
					continue;
				}
				rblReturnExchangeKbn.Items.Add(li);
			}
		}
		foreach (ListItem li in rblReturnExchangeKbn.Items)
		{
			li.Selected = (li.Value == Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN);		// デフォルト返品
		}

		// 返品交換受付日
		ucOrderReturnExchangeReceiptDate.SetDate(DateTime.Now);

		// 返品交換都合区分
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_RETURN_EXCHANGE_REASON_KBN))
		{
			if (li.Value != Constants.FLG_ORDER_RETURN_EXCHANGE_REASON_KBN_UNKNOWN)	// 「指定無し」は追加しない
			{
				rblReturnExchangeReasonKbn.Items.Add(li);
			}
		}
		foreach (ListItem li in rblReturnExchangeReasonKbn.Items)
		{
			li.Selected = (li.Value == Constants.FLG_ORDER_RETURN_EXCHANGE_REASON_KBN_USER);	// デフォルト「顧客都合」
		}

		// 決済種別
		var payments = GetPaymentValidListPermission(this.LoginOperatorDeptId, this.ReturnExchangeOrderOrg.PaymentSelectionFlg, this.ReturnExchangeOrderOrg.PermittedPaymentIds);
		if (payments.Length == 0)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PAYMENT_NO_DATA);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		ddlOrderPaymentKbn.Items.Add(new ListItem("", ""));
		foreach (var payment in payments)
		{

			if ((payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)
				&& (this.ReturnExchangeOrderOrg.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)) continue;

			if ((payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE)
				&& (this.ReturnExchangeOrderOrg.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE)) continue;

			if ((payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
				&& (this.ReturnExchangeOrderOrg.DigitalContentsFlg == Constants.FLG_ORDER_DIGITAL_CONTENTS_FLG_ON)) continue;

			if ((payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
				&& (this.ReturnExchangeOrderOrg.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)) continue;

			if ((payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
				&& (this.ReturnExchangeOrderOrg.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)) continue;

			if ((payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
				&& (this.ReturnExchangeOrderOrg.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)) continue;

			if ((payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU)
				&& (this.ReturnExchangeOrderOrg.OrderPaymentKbn != payment.PaymentId)) continue;

			if (PaygentUtility.CheckIsPaidyPaygentPayment(payment.PaymentId)
				&& (this.ReturnExchangeOrderOrg.OrderPaymentKbn != payment.PaymentId)) continue;

			ddlOrderPaymentKbn.Items.Add(
				new ListItem(payment.PaymentName, payment.PaymentId));
		}

		// カード会社
		if (OrderCommon.CreditCompanySelectable)
		{
			ddlCreditCardCompany.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ORDER, OrderCommon.CreditCompanyValueTextFieldName));
		}
		// カード有効期限(月)
		ddlCreditExpireMonth.Items.AddRange(this.CreditExpirationMonthListItems);
		ddlCreditExpireMonth.SelectedValue = DateTime.Now.Month.ToString("00");
		// カード有効期限(年)
		ddlCreditExpireYear.Items.AddRange(this.CreditExpirationYearListItems);
		ddlCreditExpireYear.SelectedValue = DateTime.Now.Year.ToString("00").Substring(2);
		// カード分割支払い
		trInstallments.Visible = OrderCommon.CreditInstallmentsSelectable;
		// 支払回数
		if (OrderCommon.CreditInstallmentsSelectable)
		{
			dllCreditInstallments.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ORDER, OrderCommon.CreditInstallmentsValueTextFieldName));
		}
		// カードセキュリティコード表示・非表示判定
		trSecurityCode.Visible = OrderCommon.CreditSecurityCodeEnable && (this.IsUserPayTg == false);
		// 登録クレジットカードセット
		this.User = new UserService().Get(this.ReturnExchangeOrderOrg.UserId);
		this.CreditCardNum = 0;
		if (UserService.IsUser(this.User.UserKbn))
		{
			var creditCardList = UserCreditCard.GetUsable(this.User.UserId);

			// つくーる連携、EScottにて通常注文時再与信不可のカードが登録される為除く
			var creditCardReAuthValidList = creditCardList
				.Where(ccl => (ccl.CooperationId.Split(',').Length != 2) || (ccl.CooperationId.Split(',')[0] != string.Empty)).ToArray();

			this.CreditCardNum = creditCardReAuthValidList.Length;
			foreach (var creditCard in creditCardReAuthValidList)
			{
				ddlUserCreditCard.Items.Add(new ListItem(creditCard.CardDispName, creditCard.BranchNo.ToString()));
			}
		}
		// 未登録カードで決済した場合、「未登録の決済カード」オプション追加する
		if ((this.ReturnExchangeOrderOrg.CreditBranchNo.HasValue)
				&& (ddlUserCreditCard.Items.FindByValue(StringUtility.ToEmpty(this.ReturnExchangeOrderOrg.CreditBranchNo)) == null))
		{
			ddlUserCreditCard.Items.Add(
				new ListItem(Constants.CREDITCARD_UNREGIST_DEFAULT_DISPLAY_NAME,
					StringUtility.ToEmpty(this.ReturnExchangeOrderOrg.CreditBranchNo)));
		}

		ddlUserCreditCard.Items.Add(
			new ListItem(
				ReplaceTag("@@DispText.credit_card_list.new@@"),
				CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW,
				(this.IsNotRakutenAgency || this.IsUserPayTg)));
		if (this.ReturnExchangeOrderOrg.CreditBranchNo.HasValue)
		{
			foreach (ListItem li in ddlUserCreditCard.Items)
			{
				li.Selected = (li.Value == this.ReturnExchangeOrderOrg.CreditBranchNo.Value.ToString());
			}
		}
		else
		{
			ddlUserCreditCard.SelectedIndex = 0;
		}

		// 付与ポイント自動計算
		cbOrderPointAddReCalculate.Checked =
			cbOrderPriceCorrectionReCalculate.Checked = Constants.ORDER_APPLYAUTOCALCULATION_DEFAULT;

		// 返金メモ
		rblRepaymentMemoType.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ORDER, Constants.VALUETEXT_ORDER_REPAYMENT_MEMO_TYPE));
		// デフォルトを返金メモのテキストエリアに設定
		foreach (ListItem li in rblRepaymentMemoType.Items)
		{
			li.Selected = (li.Value == Constants.FLG_ORDER_REPAYMENT_MEMO_TYPE_FREE_TEXT);
		}

		//------------------------------------------------------
		// 返品商品選択
		//------------------------------------------------------
		// 返品商品合計
		lbReturnOrderPriceSubTotal.Text = "0".ToPriceString(true);

		//------------------------------------------------------
		// 交換商品注文登録
		//------------------------------------------------------
		// 交換商品情報(空行を1行追加)
		var exchangeOrderItems = new []
		{
			new ReturnOrderItem()
		};
		rExchangeOrderItem.DataSource = exchangeOrderItems;
		rExchangeOrderItem.DataBind();

		// 交換商品合計
		lbExchangeOrderPriceSubTotal.Text = "0".ToPriceString(true);

		//------------------------------------------------------
		// 返品交換合計情報
		//------------------------------------------------------
		// 返品交換商品合計
		lbReturnExchangeOrderPriceSubTotal.Text = "0".ToPriceString(true);

		// 最終商品合計
		lbReturnExchangeOrderPriceTotal.Text = "0".ToPriceString(true);

		// 全ての関連する注文の最終与信フラグが正確であるかどうか
		this.IsAllLastAuthFlgValid = new OrderService().GetRelatedOrders(this.OrderIdOrg).All(o => o.LastAuthFlg != null);

		// 楽天連携かつPayTg非利用の場合、新規クレカ入力領域を非表示にする
		this.phCreditCardNotRakuten.Visible = (this.IsNotRakutenAgency || this.IsUserPayTg);
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
	}

	/// <summary>
	/// ポイント付与の初期化
	/// </summary>
	private void InitializePointAssignment()
	{
		// 初期処理
		tdOrderPoint.Visible = (this.OrderOrg.VisibleOrderPointAdd || this.OrderOrg.VisibleOrderPointUse);
		rOrderPointAddTemp.Visible = false;
		trOrderBasePointAddComp.Visible = false;
		trOrderLimitPointAddComp.Visible = false;

		// 仮ポイントが付与されている場合
		if (this.OrderOrg.VisibleOrderPointAddTemp)
		{
			rOrderPointAddTemp.Visible = this.OrderOrg.VisibleOrderPointAddTemp;
			rOrderPointAddTemp.DataSource = this.OrderOrg.UserPointTemp.Items;
			rOrderPointAddTemp.DataBind();
		}
		// 既に本ポイントに移行している場合
		else if (this.OrderOrg.VisibleOrderPointAddComp)
		{
			if (Constants.CROSS_POINT_OPTION_ENABLED == false)
			{
				trOrderBasePointAddComp.Visible
					= trOrderLimitPointAddComp.Visible
						= this.OrderOrg.VisibleOrderPointAddComp;
			}
			lOrderBasePointAdd.Text = WebSanitizer.HtmlEncode(StringUtility.ToNumeric(this.OrderOrg.OrderBasePointAddComp));
			lOrderLimitPointAdd.Text = WebSanitizer.HtmlEncode(StringUtility.ToNumeric(this.OrderOrg.OrderLimitPointAddComp));
			lOrderBasePointAddAdjustment.Text
				= lOrderLimitPointAddAdjustment.Text
					= "0";
			tbOrderBasePointAdd.Text = this.OrderOrg.OrderBasePointAddComp.ToString();
			tbOrderLimitPointAdd.Text = this.OrderOrg.OrderLimitPointAddComp.ToString();
			hfOrderBasePointAdd.Value = this.OrderOrg.OrderBasePointAddComp.ToString();
			hfOrderLimitPointAdd.Value = this.OrderOrg.OrderLimitPointAddComp.ToString();
		}
		// ポイントが付与されていない場合
		else
		{
			divOrderPointAdd.Visible = this.OrderOrg.VisibleOrderPointAdd;
		}
	}
	
	/// <summary>
	/// 再計算
	/// </summary>
	/// <param name="returnOrderItems">返品商品情報</param>
	/// <param name="exchangeOrderItems">交換商品情報</param>
	/// <param name="returnPriceByTaxRate">税率返品毎価格情報</param>
	/// <param name="lastBilledPriceByTaxRate">税率毎請求価格情報</param>
	/// <param name="sender">イベント</param>
	/// <returns>
	///  引数の各変数に返品交換商品情報を格納している。
	///  「確認するボタンクリック」イベントで利用。
	/// </returns>
	private bool Recalculate(
		out List<ReturnOrderItem> returnOrderItems,
		out List<ReturnOrderItem> exchangeOrderItems,
		ref List<OrderPriceByTaxRateModel> returnPriceByTaxRate,
		ref List<OrderPriceByTaxRateModel> lastBilledPriceByTaxRate,
		object sender)
	{
		// 初期化
		returnOrderItems = new List<ReturnOrderItem>();
		exchangeOrderItems = new List<ReturnOrderItem>();

		StringBuilder exchangeErrorMessages = new StringBuilder();

		// 返品商品情報取得＆返品商品チェック
		var shippingCountryIsoCode = "";
		var shippingProvision = "";
		var shippingCountryName = "";
		bool cheackReturnOrder = true;
		var returnOrderPriceSubTotal = 0m;
		var returnOrderPriceTaxSubTotal = 0m;
		var relatedOrderItems = this.Input.CreateAllReturnOrderItems();
		foreach (var item in relatedOrderItems)
		{
			// 返品対象の場合
			if (item.ReturnStatus == Constants.FLG_ORDER_RETURN_STATUS_RETURN_TARGET)
			{
				item.ItemPriceTax = -item.ItemPriceTax;
				item.ItemQuantity = -item.ItemQuantity;
				item.DiscountedPrice = -item.DiscountedPrice;
				item.ItemReturnExchangeKbn = Constants.FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_RETURN;

				returnOrderItems.Add(item);
				if (item.IsSubscriptionBoxFixedAmount == false) returnOrderPriceSubTotal += item.ItemPrice;
				returnOrderPriceTaxSubTotal += item.ItemPriceTax;
				shippingCountryIsoCode = item.ShippingCountryIsoCode;
				shippingProvision = item.ShippingAddr5;
				shippingCountryName = item.ShippingCountryName;
				// 返品交換対象配送先IDを設定
				ReturnExchangeShippingNo = item.OrderShippingNo;
			}
		}

		// 頒布会定額コース分の商品小計を加算
		returnOrderPriceSubTotal += relatedOrderItems
			.Where(item => IsFixedAmountCourseItemAllReturns(item.SubscriptionBoxCourseId))
			.GroupBy(item => item.SubscriptionBoxCourseId)
			.Sum(items => items.First().SubscriptionBoxFixedAmount.Value * -1);

		// 返品商品チェック
		var returnErrorMessages = CheckReturnOrderItem(returnOrderItems);

		if (returnErrorMessages.Length != 0)
		{
			cheackReturnOrder = false;
		}

		// 付与・利用ポイント入力チェック
		if (Constants.W2MP_POINT_OPTION_ENABLED)
		{
			// 付与ポイント入力チェック
			{
				var pointErrorMessage = new StringBuilder();

				// 仮ポイントが付与されている場合
				if (this.OrderOrg.VisibleOrderPointAddTemp)
				{
					foreach (var tempPoint in this.Input.OrderTempPoints)
					{
						var pointIncKbnString =
							string.Format(
								"{0}({1})",
								ReplaceTag("@@DispText.order.PointAdd@@"),
								ValueText.GetValueText(
									Constants.TABLE_USERPOINT,
									Constants.FIELD_USERPOINT_POINT_INC_KBN,
									tempPoint.PointIncKbn));
						pointErrorMessage.Append(CheckOrderPointAdd(pointIncKbnString, tempPoint.OrderPointAdd));
					}
				}
				// 既に本ポイントに移行している場合
				else if (this.OrderOrg.VisibleOrderPointAddComp)
				{
					pointErrorMessage.Append(CheckOrderPointAdd(ReplaceTag("@@DispText.order.NormalPointAdd@@"), this.Input.OrderBasePointAdd));
					pointErrorMessage.Append(CheckOrderPointAdd(ReplaceTag("@@DispText.order.LimitPointAdd@@"), this.Input.OrderLimitPointAdd));
				}

				if (pointErrorMessage.Length != 0)
				{
					tbdyReturnOrderAddPointErrorMessages.Visible = true;
					lbReturnOrderAddPointErrorMessages.Text = pointErrorMessage.ToString();
					cheackReturnOrder = false;
				}
			}

			// 利用ポイント入力チェック
			{
				var pointErrorMessage = this.Input.CheckLastOrderPointUseInput();
				if (pointErrorMessage.Length != 0)
				{
					tbdyReturnOrderUsePointErrorMessages.Visible = true;
					lbReturnOrderUsePointErrorMessages.Text = pointErrorMessage;
					cheackReturnOrder = false;
				}
			}
		}

		// 交換商品情報取得＆交換商品入力チェック＆交換商品チェック
		var exchangeOrderPriceSubTotal = 0m;
		var exchangeOrderPriceTaxSubTotal = 0m;
		var tempExchangeOrderItems = this.Input.CreateExchangeOrderItems(
			this.ReturnExchangeShippingNo,
			this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem());
		// 返品交換区分が「交換」の場合のみ
		if (rblReturnExchangeKbn.SelectedValue == Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE)
		{
			// ギフト時の返品商品チェック
			//（返品商品チェックでエラーが出ているときは、上書きしてしまわないようここではチェックしない）
			if (returnErrorMessages.Length == 0)
			{
				returnErrorMessages = CheckReturnOrderItemForGift(returnOrderItems);
				if (returnErrorMessages.Length != 0)
				{
					cheackReturnOrder = false;
				}
			}

			// 交換商品入力チェック
			int index = 1;
			foreach (var exchangeOrderItem in this.Input.ExchangeItems)
			{
				// 「@@ 1 @@」を商品番号で置換し、エラーメッセージを連結
				exchangeErrorMessages.Append(Validator.Validate("OrderItemReturnExchangeInput", exchangeOrderItem.DataSource)
					.Replace("@@ 1 @@",
						string.Format(
							ReplaceTag("@@DispText.common_message.location_no@@"),
							index,
							string.Empty)));

				index++;

				// 定期購入フラグ整合性チェック
				DataView dvProductVariation = GetProductVariation(
					this.LoginOperatorShopId,
					StringUtility.ToEmpty(exchangeOrderItem.ProductId),
					StringUtility.ToEmpty(exchangeOrderItem.VariationId));
				if (dvProductVariation.Count == 0) continue;

				var productName = StringUtility.ToEmpty(dvProductVariation[0][Constants.FIELD_PRODUCT_NAME]);
				var productFixedPurchaseFlg = dvProductVariation[0][Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG].ToString();
				var isFixedPurchaseValid = (StringUtility.ToEmpty(exchangeOrderItem.FixedPurchaseProductFlg) == Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON);
				exchangeErrorMessages.Append(CheckFixedPurchaseFlgValid(productName, productFixedPurchaseFlg, isFixedPurchaseValid));

				// 商品有効性チェック
				if ((string)dvProductVariation[0][Constants.FIELD_PRODUCT_VALID_FLG] == Constants.FLG_PRODUCT_VALID_FLG_INVALID)
				{
					exchangeErrorMessages.Append(
						WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_INVALID).Replace("@@ 1 @@", productName));
				}
			}
			if (exchangeErrorMessages.Length == 0)
			{
				foreach (var item in tempExchangeOrderItems)
				{
					var itemPriceTax = TaxCalculationUtility.GetTaxPrice(
						item.ProductPrice,
						item.ProductTaxRate,
						shippingCountryIsoCode,
						shippingProvision,
						Constants.TAX_EXCLUDED_FRACTION_ROUNDING) * item.ItemQuantity;

					item.ItemPriceTax = itemPriceTax;
					item.ItemReturnExchangeKbn = Constants.FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_EXCHANGE;
					item.ShippingAddr5 = shippingProvision;
					item.ShippingCountryIsoCode = shippingCountryIsoCode;
					item.ShippingCountryName = shippingCountryName;

					exchangeOrderItems.Add(item);
					exchangeOrderPriceSubTotal += item.ItemPrice;
					exchangeOrderPriceTaxSubTotal += item.ItemPriceTax;
				}

				// 交換商品チェック
				exchangeErrorMessages.Append(CheckExchangeOrderItem(exchangeOrderItems));
			}
		}

		// 返品交換合計入力チェック
		var returnExchangeErrorMessages = this.Input.CheckInputOrderPriceRepayment();
		returnExchangeErrorMessages += this.Input.CheckInputProductCorrectionPrice();

		bool success = ((cheackReturnOrder)
			&& (exchangeErrorMessages.Length == 0)
			&& (returnExchangeErrorMessages.Length == 0));
		if (success)
		{
			// カートオブジェクト作成
			var cart = CreateCart();

			// ポイントOPが有効？
			if (Constants.W2MP_POINT_OPTION_ENABLED)
			{
				// 付与ポイント自動計算あり AND 再計算ボタンクリック時
				if (this.Input.IsOrderPointAddReCalculate
					&& (((Button)sender).ID == btnReCalculate.ID))
				{
					// 付与ポイントセット
					SetCartInfoToOrderPointAdd(
						cart,
						this.Input.OrderTempPoints,
						this.Input.OrderBasePointAddBefore,
						this.Input.OrderLimitPointAddBefore);
				}
				// 利用ポイントセット
				success = SetCartInfoToOrderPointUse(cart);
			}

			if (success)
			{
				// 交換商品情報設定
				rExchangeOrderItem.DataSource = tempExchangeOrderItems;
				rExchangeOrderItem.DataBind();
				lbExchangeOrderPriceSubTotal.Text = exchangeOrderPriceSubTotal.ToPriceString(true);

				var productCorrectionPriceBox = this.Input.SetProductCorrectionPriceBox(
					this.OrderOrg.OrderId,
					this.OrderOrg.OrderPriceByTaxRate,
					exchangeOrderItems);

				rPriceCorrection.DataSource = productCorrectionPriceBox;
				rPriceCorrection.DataBind(); 

				var returnAndExchangeItems = new List<ReturnOrderItem>(returnOrderItems);
				returnAndExchangeItems.AddRange(exchangeOrderItems);

				var pointPriceByTaxRate = OrderCommon.CalculateAdjustmentPointPriceByTaxRate(
					this.Input.AdjustmentPointPrice,
					returnAndExchangeItems,
					cart);

				cart.IsReturnAllItems = this.Input.IsReturnAllItems();

				var allReturnFixedAmountCourseIds = returnOrderItems
					.Where(item => IsFixedAmountCourseItemAllReturns(item.SubscriptionBoxCourseId))
					.Select(item => item.SubscriptionBoxCourseId)
					.Distinct()
					.ToArray();

				// 返品用金額補正：自動計算して画面にセット
				if (this.Input.IsOrderPriceCorrectionReCalculate
					&& ((((Button)sender).ID == btnReCalculate.ID)
						|| cart.IsPaymentAtoneOrAftee))
				{
					var calculatedCorrectionPriceByTaxRate = OrderCommon.CalculatePriceCorrectionByTaxRate(
						cart,
						this.ReturnExchangeOrderOrg.OrderPriceByTaxRate,
						pointPriceByTaxRate,
						returnAndExchangeItems,
						allReturnFixedAmountCourseIds);
					rPriceCorrection.DataSource = calculatedCorrectionPriceByTaxRate
						.OrderBy(priceInfo => priceInfo.KeyTaxRate);
					rPriceCorrection.DataBind();
				}
				// HACK:DataBindした値を利用しているので、画面値を再取得
				this.Input = CreateInput();
				
				// 入力値から返品用金額補正の金額を取得
				var correctionPriceByTaxRate = this.Input.GetPriceInfoFromPriceCorrectionBox();

				// 返品交換合計情報設定
				var returnExchangeOrderPriceSubTotal = this.Input.CalculateReturnExchangeOrderPriceSubTotal(
					returnOrderPriceSubTotal,
					exchangeOrderPriceSubTotal);

				var returnExchangeOrderPriceTaxSubTotal = this.Input.CalculateReturnExchangeOrderPriceTaxSubTotal(
					returnOrderPriceTaxSubTotal,
					exchangeOrderPriceTaxSubTotal);

				var returnExchangeOrderPriceTotal = this.Input.CalculateReturnExchangeOrderPriceTotal(
					returnExchangeOrderPriceSubTotal,
					returnExchangeOrderPriceTaxSubTotal,
					this.Input.AdjustmentPointPrice,
					this.Input.ReturnPriceCorrectionTotal);

				var returnExchangeLastBilledAmount = this.Input.CalculateReturnExchangeLastBilledAmount(
					decimal.Parse(this.ReturnExchangeOrderOrg.RelatedOrderLastBilledAmount),
					returnExchangeOrderPriceTotal);

				returnPriceByTaxRate.AddRange(
					OrderCommon.CalculateReturnPriceInfoByTaxRate(
						returnAndExchangeItems,
						correctionPriceByTaxRate,
						pointPriceByTaxRate,
						allReturnFixedAmountCourseIds));

				lbReturnExchangeOrderPriceSubTotal.ForeColor = GetPriceDisplayColor(returnExchangeOrderPriceSubTotal);	// 商品合計
				lbReturnExchangeOrderPriceSubTotal.Text = returnExchangeOrderPriceSubTotal.ToPriceString(true);
				lbReturnExchangeOrderPriceTax.ForeColor = GetPriceDisplayColor(returnExchangeOrderPriceTaxSubTotal);	// 消費税
				lbReturnExchangeOrderPriceTax.Text = returnExchangeOrderPriceTaxSubTotal.ToPriceString(true);
				lbReturnExchangeOrderPriceTotal.ForeColor = GetPriceDisplayColor(returnExchangeOrderPriceTotal);	// 最終合計金額
				lbReturnExchangeOrderPriceTotal.Text = returnExchangeOrderPriceTotal.ToPriceString(true);
				
				var orderPriceByTaxRateSummary = new List<OrderPriceByTaxRateModel>();
				orderPriceByTaxRateSummary.AddRange(this.ReturnExchangeOrderOrg.OrderPriceByTaxRate);
				orderPriceByTaxRateSummary.AddRange(returnPriceByTaxRate);
				lastBilledPriceByTaxRate.AddRange(orderPriceByTaxRateSummary.GroupBy(
					priceByTaxRateSummary => new
					{
						priceByTaxRateSummary.KeyTaxRate
					})
					.Select(groupedPriceByTaxRateSummary => new OrderPriceByTaxRateModel()
					{
						KeyTaxRate = groupedPriceByTaxRateSummary.Key.KeyTaxRate,
						PriceSubtotalByRate = groupedPriceByTaxRateSummary.Sum(itemKey => itemKey.PriceSubtotalByRate),
						PriceShippingByRate = groupedPriceByTaxRateSummary.Sum(itemKey => itemKey.PriceShippingByRate),
						PricePaymentByRate = groupedPriceByTaxRateSummary.Sum(itemKey => itemKey.PricePaymentByRate),
						ReturnPriceCorrectionByRate = groupedPriceByTaxRateSummary.Sum(itemKey => itemKey.ReturnPriceCorrectionByRate),
						PriceTotalByRate = groupedPriceByTaxRateSummary.Sum(itemKey => itemKey.PriceTotalByRate),
						TaxPriceByRate = groupedPriceByTaxRateSummary.Sum(itemKey => itemKey.TaxPriceByRate),
					}).ToList());

				rReturnExchangeLastBilledAmountByTaxRate.DataSource = lastBilledPriceByTaxRate;
				rReturnExchangeLastBilledAmountByTaxRate.DataBind();
				((Label)(rReturnExchangeLastBilledAmountByTaxRate.Items[0].FindControl("lbReturnExchangeLastBilledAmount"))).Text = StringUtility.ToPrice(returnExchangeLastBilledAmount);
				// 最終合計金額がマイナスの場合は返金金額をセット
				tbOrderPriceRepayment.Text =
					((returnExchangeOrderPriceTotal < 0) ? -returnExchangeOrderPriceTotal : 0).ToPriceString();

				rReturnExchangePriceByTaxRate.DataSource = returnPriceByTaxRate.OrderBy(priceInfo => priceInfo.KeyTaxRate);
				rReturnExchangePriceByTaxRate.DataBind();

				if ((Constants.ECPAY_PAYMENT_OPTION_ENABLED)
					&& (ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
					&& (this.ReturnExchangeOrderOrg.ExternalPaymentType == Constants.FLG_PAYMENT_TYPE_ECPAY_CREDIT))
				{
					var lastBilledAmountBefore = decimal.Parse(this.ReturnExchangeOrderOrg.RelatedOrderLastBilledAmount);
					success = (returnExchangeLastBilledAmount <= lastBilledAmountBefore);
					lbPaymentAlertMessage.Text = success
						? string.Empty
						: WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ECPAY_TYPE_CREDIT);

					lbPaymentAlertMessage.Visible = (success == false);
				}

				if ((Constants.NEWEBPAY_PAYMENT_OPTION_ENABLED)
					&& (ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
					&& (this.ReturnExchangeOrderOrg.ExternalPaymentType == Constants.FLG_PAYMENT_TYPE_NEWEBPAY_CREDIT))
				{
					var lastBilledAmountBefore = decimal.Parse(this.ReturnExchangeOrderOrg.RelatedOrderLastBilledAmount);
					success = (returnExchangeLastBilledAmount <= lastBilledAmountBefore);
					lbPaymentAlertMessage.Text = success
						? string.Empty
						: WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NEWEBPAY_TYPE_CREDIT);

					lbPaymentAlertMessage.Visible = (success == false);
				}

				if ((Constants.PAYMENT_PAYPAYOPTION_ENABLED)
					&& (ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY))
				{
					var lastBilledAmountBefore = decimal.Parse(this.ReturnExchangeOrderOrg.RelatedOrderLastBilledAmount);
					success = (returnExchangeLastBilledAmount <= lastBilledAmountBefore);
					lbPaymentAlertMessage.Text = success
						? string.Empty
						: WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_RETURNEXCHANGE_LAST_BILLED_AMOUNT)
							.Replace("@@ 1 @@", "PayPay");

					lbPaymentAlertMessage.Visible = (success == false);
				}
			}
		}

		// エラーメッセージ制御
		lbReturnOrderItemErrorMessages.Text = returnErrorMessages;
		tbdyReturnOrderItemErrorMessages.Visible = (returnErrorMessages != "");

		lbExchangeOrderItemErrorMessages.Text = exchangeErrorMessages.ToString();
		tbdyExchangeOrderItemErrorMessages.Visible = (exchangeErrorMessages.ToString() != "");

		lbReturnExchangeErrorMessages.Text = returnExchangeErrorMessages;
		tbdyReturnExchangeErrorMessages.Visible = (returnExchangeErrorMessages != "");

		return success;
	}

	/// <summary>
	/// 再計算
	/// </summary>
	/// <param name="sender">イベント</param>
	/// <returns>成功か非</returns>
	private bool Recalculate(object sender)
	{
		List<ReturnOrderItem> returnOrderItems = null;
		List<ReturnOrderItem> exchangeOrderItems = null;
		var orderPriceByTaxRateList = new List<OrderPriceByTaxRateModel>();
		var lastBilledPriceByTaxRateList = new List<OrderPriceByTaxRateModel>();
		var result = Recalculate(out returnOrderItems,
			out exchangeOrderItems,
			ref orderPriceByTaxRateList,
			ref lastBilledPriceByTaxRateList,
			sender);
		return result;
	}

	/// <summary>
	/// 支払方法制限メッセージ表示
	/// </summary>
	private void DisplayLimitedPaymentMessages()
	{
		// 交換商品情報作成
		var exchangeOrderItems = this.Input.CreateExchangeOrderItems(
			this.ReturnExchangeShippingNo,
			this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem());

		// 支払方法制限チェック
		var limitedPaymentMessages = string.Empty;
		var productList = new List<KeyValuePair<string, string>>();
		foreach (var exchangeOrderItem in exchangeOrderItems)
		{
			var productId = exchangeOrderItem.ProductId;
			var variationId = exchangeOrderItem.VariationId;
			if (productId.Length == 0) continue;

			productList.Add(new KeyValuePair<string, string>(productId, variationId));
		}
		limitedPaymentMessages += string.Format("{0}{1}", GetProductsLimitedPaymentMessage(this.LoginOperatorShopId, productList), Environment.NewLine);
		lbPaymentLimitedMessage.Text = limitedPaymentMessages.Trim();
	}

	/// <summary>
	/// 決済種別選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlOrderPaymentKbn_OnSelectedIndexChanged(object sender, System.EventArgs e)
	{
		if (Constants.PAYMENT_REAUTH_ENABLED == false) return;
		// モール注文の場合、固定に「連動しない」を設定する
		if (this.ReturnExchangeOrderOrg.IsPermitReauthOrderSiteKbn == false
			|| (this.IsAllLastAuthFlgValid == false))
		{
			lbExternalPayment.Text = string.Format("{0} : ",
				//「{0}連携 :」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ORDER,
					Constants.VALUETEXT_PARAM_ORDER_RETURN_EXCHANGE_INPUT,
					Constants.VALUETEXT_PARAM_ORDER_PAYMENT_COOPERATION));
			ddlExecuteType.SelectedValue = ReauthCreatorFacade.ExecuteTypes.None.ToString();
			ddlExecuteType.Visible = true;
			ddlExecuteType.Enabled = false;
			return;
		}

		// 再与信不可能のカードの場合、「連動しない」＆無効にする
		var userCreditCard = this.ReturnExchangeOrderOrg.CreditBranchNo != null
			? new UserCreditCardService().Get(this.User.UserId, (int)this.ReturnExchangeOrderOrg.CreditBranchNo)
			: null;
		var cooperationIdArray = (userCreditCard != null ? userCreditCard.CooperationId : string.Empty).Split(',');
		if ((cooperationIdArray[0] == string.Empty) && (cooperationIdArray.Length == 2))
		{
			ddlExecuteType.SelectedValue = ReauthCreatorFacade.ExecuteTypes.None.ToString();
			ddlExecuteType.Visible = true;
			ddlExecuteType.Enabled = false;
		}

		// 自社サイトの注文の場合
		ddlExecuteType.Visible = true;
		var paymentName = new PaymentService().GetPaymentName(this.LoginOperatorShopId, ddlOrderPaymentKbn.SelectedValue);
		lbExternalPayment.Text = string.Format(
			//「{0}連携 :」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ORDER,
				Constants.VALUETEXT_PARAM_ORDER_RETURN_EXCHANGE_INPUT,
				Constants.VALUETEXT_PARAM_ORDER_COOPERATION),
			WebSanitizer.HtmlEncode(paymentName));

		lbOrderPaymentInfo.Visible = false;
		if (ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT)
		{
			lbPaymentAlertMessage.Visible = true;
			lbPaymentAlertMessage.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_AMAZON_PAY_CHANGE_AMOUNT_NOTIFY);
		}
		else if (ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2)
		{
			lbPaymentAlertMessage.Visible = true;
			lbPaymentAlertMessage.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_AMAZON_PAY_CV2_CHANGE_AMOUNT_NOTIFY);
		}
		else if (ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
		{
			var accountEmail = PayPalUtility.Account.GetCooperateAccountEmail(this.User.UserId);
			if (string.IsNullOrEmpty(accountEmail))
			{
				lbPaymentAlertMessage.Visible = true;
				lbPaymentAlertMessage.Text = WebSanitizer.HtmlEncode(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PAYPAL_ISNOT_LINK_ACCOUNT));
			}
			else
			{
				lbOrderPaymentInfo.Visible = true;
				lbOrderPaymentInfo.Text = WebSanitizer.HtmlEncode(WebMessages
					.GetMessages(WebMessages.ERRMSG_MANAGER_PAYPAL_AVAILABLE_ACCOUNT)
					.Replace("@@ 1 @@", accountEmail));
			}
		}
		else
		{
			lbPaymentAlertMessage.Visible = false;
		}

		// クレジットカード以外は抜ける
		tbdyPaymentKbnCredit.Visible = true;
		if ((ddlOrderPaymentKbn.SelectedValue != Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
			|| (trOrderPaymentKbn.Visible == false)
			|| ((ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
				&& (Constants.REAUTH_COMPLETE_CREDITCARD_LIST.Contains(Constants.PAYMENT_CARD_KBN) == false))) tbdyPaymentKbnCredit.Visible = false;

		IsValidExternalPaymentStatus();
	}

	/// <summary>
	/// 利用クレジットカードドロップダウン変更イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlUserCreditCard_SelectedIndexChanged(object sender, EventArgs e)
	{
		DisplayCreditInputForm();
	}

	/// <summary>
	/// 返品交換区分選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblReturnExchangeKbn_OnSelectedIndexChanged(object sender, System.EventArgs e)
	{
		// ページロード内で表示制御処理を行っている
	}

	/// <summary>
	/// 追加ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAddProduct_Click(object sender, EventArgs e)
	{
		// 交換商品情報作成
		var exchangeOrderItems = new List<ReturnOrderItem>();
		exchangeOrderItems.AddRange(this.Input.CreateExchangeOrderItems(
			this.ReturnExchangeShippingNo,
			this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem()));
		exchangeOrderItems.Add(new ReturnOrderItem());

		// データバインド
		rExchangeOrderItem.DataSource = exchangeOrderItems.ToArray();
		rExchangeOrderItem.DataBind();
	}

	/// <summary>
	/// 返品商品全選択チェックイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbReturnProductAll_CheckedChanged(object sender, EventArgs e)
	{
		var containedRepeaterItem = (RepeaterItem)((WebControl)sender).BindingContainer;
		foreach (RepeaterItem ri in ((Repeater)containedRepeaterItem.FindControl("rReturnOrderItem2")).Items)
		{
			if (((CheckBox)ri.FindControl("cbReturnProduct")).Visible)
			{
				((CheckBox)ri.FindControl("cbReturnProduct")).Checked = ((CheckBox)sender).Checked;
			}
		}

		this.Input = CreateInput();

		ChangeCbReturnProduct();

		if (Constants.W2MP_POINT_OPTION_ENABLED && Constants.CROSS_POINT_OPTION_ENABLED)
		{
			DisplayCheckBoxOrderPointAddReCalculate();
		}
	}

	/// <summary>
	/// 返品商品チェックイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbReturnProduct_CheckedChanged(object sender, EventArgs e)
	{
		var containedRepeaterItem = (RepeaterItem)((WebControl)sender).BindingContainer.BindingContainer.BindingContainer;
		((CheckBox)containedRepeaterItem.FindControl("cbReturnProductAll")).Checked =
			((Repeater)containedRepeaterItem.FindControl("rReturnOrderItem2")).Items
			.Cast<RepeaterItem>()
			.Where(ri => ((CheckBox)ri.FindControl("cbReturnProduct")).Visible)
			.All(ri => ((CheckBox)ri.FindControl("cbReturnProduct")).Checked);

		this.Input = CreateInput();

		ChangeCbReturnProduct();

		if (Constants.W2MP_POINT_OPTION_ENABLED && Constants.CROSS_POINT_OPTION_ENABLED)
		{
			DisplayCheckBoxOrderPointAddReCalculate();
		}
	}

	/// <summary>
	/// 返品商品チェックイベント(交換注文用)
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbReturnProduct_CheckedChangedExchange(object sender, EventArgs e)
	{
		foreach (RepeaterItem ri in rExchangedOrder.Items)
		{
			var returnOrderItems = this.Input.CreateReturnOrderItems(
				this.Input.ShopId,
				this.Input.ExchangedItems[ri.ItemIndex]);
			var allReturnFixedAmountCourseIds = returnOrderItems
				.Where(item => IsFixedAmountCourseItemAllReturns(item.SubscriptionBoxCourseId))
				.Select(item => item.SubscriptionBoxCourseId)
				.Distinct()
				.ToArray();
			DrawReturnOrderItems(
				(Repeater)ri.FindControl("rReturnOrderItemExchanged"),
				(Label)ri.FindControl("lbReturnOrderPriceSubTotal"),
				returnOrderItems,
				this.Input.CalculateReturnOrderPriceSubTotal(returnOrderItems, allReturnFixedAmountCourseIds));
		}
	}

	/// <summary>
	/// 返品商品選択
	/// </summary>
	private void ChangeCbReturnProduct()
	{
		var allReturnOrderItems = this.Input.CreateReturnOrderItems(
			this.Input.ShopId,
			this.Input.ReturnItems.SelectMany(x => x).ToArray());
		var allReturnFixedAmountCourseIds = allReturnOrderItems
			.Where(item => IsFixedAmountCourseItemAllReturns(item.SubscriptionBoxCourseId))
			.Select(item => item.SubscriptionBoxCourseId)
			.Distinct()
			.ToArray();
		var returnOrderPriceSubTotal = this.Input.CalculateReturnOrderPriceSubTotal(
			allReturnOrderItems,
			allReturnFixedAmountCourseIds);
		foreach (RepeaterItem ri in rReturnOrderItem.Items)
		{
			var returnOrderItems = this.Input.CreateReturnOrderItems(
				this.Input.ShopId,
				this.Input.ReturnItems[ri.ItemIndex]);
			DrawReturnOrderItems(
				(Repeater)ri.FindControl("rReturnOrderItem2"),
				lbReturnOrderPriceSubTotal,
				returnOrderItems,
				returnOrderPriceSubTotal);
		}
	}

	/// <summary>
	/// 調整後付与ポイントリスト作成（仮ポイント）
	/// </summary>
	/// <param name="userId"></param>
	/// <param name="orderId">注文ID</param>
	/// <param name="tempPoints">仮ポイントリスト</param>
	/// <returns>返品時調整ポイントリスト</returns>
	private List<Hashtable> CreateReturnAddPointTemp(
		string userId,
		string orderId,
		OrderReturnExchangeTempPointInput[] tempPoints)
	{
		return tempPoints.Select(tempPoint =>
			new Hashtable
		{
				{ Constants.FIELD_USERPOINT_USER_ID, userId },
				{ Constants.FIELD_ORDER_ORDER_ID, orderId },
				{ Constants.FIELD_USERPOINT_POINT_KBN_NO, tempPoint.PointKbnNo },
				{ Constants.FIELD_USERPOINT_POINT_KBN, tempPoint.PointKbn },
				{ Constants.FIELD_USERPOINT_POINT_INC_KBN, tempPoint.PointIncKbn },
				{ Constants.FIELD_USERPOINT_POINT, decimal.Parse(tempPoint.OrderPointAdd) },
				{ Constants.FIELD_USERPOINT_POINT + Constants.FIELD_COMMON_BEFORE, decimal.Parse(tempPoint.OrderPointAddBefore) },
				{ CONST_ORDER_POINT_ADD_ADJUSTMENT, decimal.Parse(tempPoint.OrderPointAdd) - decimal.Parse(tempPoint.OrderPointAddBefore) }
		}
		).ToList();
	}

	/// <summary>
	/// 返品商品チェックイベント描画処理
	/// </summary>
	/// <param name="rReturnOrderItem">返品商品リピータ</param>
	/// <param name="lbReturnOrderPriceSubTotal">返品商品合計ラベル></param>
	/// <param name="returnOrderItems">返品商品リスト</param>
	/// <param name="returnOrderPriceSubTotal">返品商品合計</param>
	protected void DrawReturnOrderItems(
		Repeater rReturnOrderItem,
		Label lbReturnOrderPriceSubTotal,
		ReturnOrderItem[] returnOrderItems,
		decimal returnOrderPriceSubTotal)
	{
		// 行カラー切替処理
		foreach (RepeaterItem item in rReturnOrderItem.Items)
		{
			string strCss = null;
			string strFont = null;
			string strMinus = null;
			switch (returnOrderItems[item.ItemIndex].ReturnStatus)
			{
				case Constants.FLG_ORDER_RETURN_STATUS_RETURN_TARGET:
					strCss = "edit_item_bg";
					strFont = "color:Red";
					strMinus = "-";
					break;

				case Constants.FLG_ORDER_RETURN_STATUS_RETURN_UNTARGET:
				case Constants.FLG_ORDER_RETURN_STATUS_RETURN_COMPLETE:
					strCss = "mobile_item_bg";
					strFont = "";
					strMinus = "";
					break;
			}

			((HtmlTableRow)item.FindControl("trItem")).Attributes["class"] = strCss;
			((HtmlTableRow)item.FindControl("trItem1")).Attributes["class"] = strCss;
			((HtmlTableCell)item.FindControl("tdItem1")).Attributes["class"] = strCss;
			((HtmlGenericControl)item.FindControl("spanProductPrice")).Attributes["style"] = strFont;
			((HtmlGenericControl)item.FindControl("spanItemQuantity")).Attributes["style"] = strFont;
			((HtmlGenericControl)item.FindControl("spanMinusProductPrice")).InnerText = strMinus;
			((HtmlGenericControl)item.FindControl("spanMinusItemQuantity")).InnerText = strMinus;

			// 注文同梱時、頒布会定額コースの小計部分は、同コース商品が全て返品される場合のみ赤字にする
			if (this.ReturnExchangeOrderOrg.IsOrderCombinedWithSubscriptionBoxItem == false) continue;

			var wspanFixedAmount = GetWrappedControl<WrappedHtmlGenericControl>(item, "spanFixedAmount");
			var wspanFixedAmountMinus = GetWrappedControl<WrappedHtmlGenericControl>(item, "spanFixedAmountMinus");
			var isFixedAmountCourseItemAllReturns = IsFixedAmountCourseItemAllReturns(
				returnOrderItems[item.ItemIndex].SubscriptionBoxCourseId);
			if (isFixedAmountCourseItemAllReturns && returnOrderItems[item.ItemIndex].IsReturnTarget)
			{
				wspanFixedAmount.Attributes["style"] = "color:Red";
				wspanFixedAmountMinus.InnerText = "-";
			}
			else
			{
				wspanFixedAmount.Attributes["style"] = string.Empty;
				wspanFixedAmountMinus.InnerText = string.Empty;
			}
		}

		//------------------------------------------------------
		// 返品商品合計設定
		//------------------------------------------------------
		// マイナスの場合はフォントカラーを赤へ
		lbReturnOrderPriceSubTotal.ForeColor = GetPriceDisplayColor(-returnOrderPriceSubTotal);
		lbReturnOrderPriceSubTotal.Text = (-returnOrderPriceSubTotal).ToPriceString(true);
	}

	/// <summary>
	/// 「付与ポイント自動計算」の表示（CrossPoint運用時）
	/// </summary>
	private void DisplayCheckBoxOrderPointAddReCalculate()
	{
		var cbReturnProductAllFlgs = new List<bool>();
		foreach (RepeaterItem returnOrder in rReturnOrderItem.Items)
		{
			var cbReturnProductAllFlg = ((Repeater)returnOrder.FindControl("rReturnOrderItem2")).Items
				.Cast<RepeaterItem>()
				.Where(ri => ((CheckBox)ri.FindControl("cbReturnProduct")).Visible)
				.All(ri => ((CheckBox)ri.FindControl("cbReturnProduct")).Checked);
			cbReturnProductAllFlgs.Add(cbReturnProductAllFlg);
		}

		// 全返品の場合は「付与ポイント自動計算」をチェックつけ、外さないようにし
		// 付与されているポイントは0にする。（CrossPoin側と合わせるため）
		var orderPointAddReCalculateFlg = ((cbReturnProductAllFlgs.Count(flg => (flg == false))) > 0);
		cbOrderPointAddReCalculate.Enabled = orderPointAddReCalculateFlg;
		if (orderPointAddReCalculateFlg == false)
		{
			var cart = CreateCart();
			SetCartInfoToOrderPointAdd(
				cart,
				this.Input.OrderTempPoints,
				this.Input.OrderBasePointAddBefore,
				this.Input.OrderLimitPointAddBefore);
		}
		else
		{
			InitializePointAssignment();
		}
	}

	/// <summary>
	/// 交換商品リピータイベント
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rExchangeOrderItem_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		// 交換商品情報取得
		var exchangeOrderItems = this.Input.CreateExchangeOrderItems(
			this.ReturnExchangeShippingNo,
			this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem());
		switch (e.CommandName)
		{
			// 取得
			case "get":
				var targetItem = exchangeOrderItems[int.Parse(e.CommandArgument.ToString())];
				DataView dvProductVariation = GetProductVariation(
					this.LoginOperatorShopId,
					targetItem.ProductId,
					targetItem.VariationId,
					this.ReturnExchangeOrderOrg.MemberRankId,
					targetItem.ProductSaleId);
				if (dvProductVariation.Count != 0)
				{
					targetItem.ProductName = dvProductVariation[0][Constants.FIELD_PRODUCT_NAME] + CreateVariationName(dvProductVariation[0]);
					targetItem.ItemQuantity = 1;
					targetItem.ProductTaxRate = new ProductTaxCategoryService().Get((string)dvProductVariation[0][Constants.FIELD_PRODUCT_TAX_CATEGORY_ID]).TaxRate;

					// 定期購入のみ可の場合定期にチェックを付け定期価格を設定
					if (dvProductVariation[0][Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG].ToString() == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_ONLY)
					{
						targetItem.FixedPurchaseProductFlg = Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON;
						var productId = dvProductVariation[0][Constants.FIELD_PRODUCT_PRODUCT_ID].ToString();
						var variationId = dvProductVariation[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_ID].ToString();
						var OrderItem = this.ReturnExchangeOrderOrg.Items
							.FirstOrDefault(item => (item.ProductId == productId)
								&& (item.VariationId == variationId));
						var isFirstOrder = ((OrderItem == null) || (OrderItem.FixedPurchaseItemIsFirstOrder));
						targetItem.ProductPrice = GetFixedPurchaseProductValidityPrice(
							dvProductVariation[0],
							isFirstOrder);
						targetItem.ProductName = CreateFixedPurchaseProductName(
							targetItem.ProductName,
							true,
							this.IsSubscriptionBoxValid);
					}
					else
					{
						targetItem.FixedPurchaseProductFlg = Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_OFF;
						targetItem.ProductPrice = GetProductValidityPrice(dvProductVariation[0]);
					}

					targetItem.ProductSaleId = StringUtility.ToEmpty(dvProductVariation[0][Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID]);
					targetItem.BundleItemDisplayType = StringUtility.ToEmpty(dvProductVariation[0][Constants.FIELD_ORDERITEM_BUNDLE_ITEM_DISPLAY_TYPE]);
					tbdyExchangeOrderItemErrorMessages.Visible = false; // エラーは一度消す
				}
				else
				{
					lbExchangeOrderItemErrorMessages.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERREGIST_NOPRODUCT);
					tbdyExchangeOrderItemErrorMessages.Visible = true;
				}

				if (targetItem.SupplierId == "")
				{
					DataView dvProductInfo = ProductCommon.GetProductInfo(this.LoginOperatorShopId, targetItem.ProductId, "", string.Empty);
					if (dvProductInfo.Count != 0)
					{
						targetItem.SupplierId = (string)dvProductInfo[0][Constants.FIELD_PRODUCT_SUPPLIER_ID];
					}
				}
				rExchangeOrderItem.DataSource = exchangeOrderItems;
				rExchangeOrderItem.DataBind();

				// HACK:DataBindした値を利用しているので、画面値を再取得
				this.Input = CreateInput();

				// 商品が存在する場合は注文できるか判定にかける
				if (dvProductVariation.Count != 0) CheckExchangeItemRegisterable(targetItem.ItemIndex);

				break;

			// 削除
			case "delete":
				rExchangeOrderItem.DataSource = exchangeOrderItems.Where(x => (x.ItemIndex != e.CommandArgument.ToString())).ToArray();
				rExchangeOrderItem.DataBind();
				tbdyExchangeOrderItemErrorMessages.Visible = false; // エラーは一度消す
				break;
		}
	}

	/// <summary>
	/// 再計算ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnRecalculate_Click(object sender, EventArgs e)
	{
		// 再計算・入力チェック
		Recalculate(sender);
		cbOrderPriceCorrectionReCalculate_CheckedChanged(sender, e);
	}

	/// <summary>
	/// 確認するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnConfirm_Click(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// 再計算・入力チェック
		//------------------------------------------------------
		// 正常入力の場合
		List<ReturnOrderItem> returnOrderItems = null;
		List<ReturnOrderItem> exchangeOrderItems = null;
		var returnPriceByTaxRateList = new List<OrderPriceByTaxRateModel>();
		var lastBilledPriceByTaxRateList = new List<OrderPriceByTaxRateModel>();
		if (Recalculate(
			out returnOrderItems,
			out exchangeOrderItems,
			ref returnPriceByTaxRateList,
			ref lastBilledPriceByTaxRateList,
			sender))
		{
			//------------------------------------------------------
			// 返品交換受付日、決済種別入力チェック
			//------------------------------------------------------
			// 返品交換受付日
			Hashtable htOrder = new Hashtable();
			htOrder.Add("return_exchange_date", this.Input.OrderReturnExchangeReceiptDate);
			// 選択された決済種別設定
			var liOrderPaymentKbn = GetSelectedOrderPaymentListItem(this.ReturnExchangeOrderOrg.MallId);
			htOrder.Add(Constants.FIELD_ORDER_ORDER_PAYMENT_KBN, liOrderPaymentKbn.Value);
			htOrder.Add(Constants.FIELD_ORDER_ORDER_PAYMENT_KBN + "_text", liOrderPaymentKbn.Text);
			// 返品交換ステータス(返品交換受付)、返品交換受付日時セット
			htOrder.Add(Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_STATUS, Constants.FLG_ORDER_ORDER_RETURN_EXCHANGE_STATUS_RECEIPT);
			htOrder.Add(Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_RECEIPT_DATE, this.Input.OrderReturnExchangeReceiptDate);

			string strErrorMessages = Validator.Validate("OrderReturnExchangeInput", htOrder);
			if ((strErrorMessages.Length == 0)
				&& ((liOrderPaymentKbn.Value == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2)
					|| (liOrderPaymentKbn.Value == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT)))
			{
				var isReturnable = CheckOrderRetrunReceiptDateWithinReturnTerms(this.Input.OrderReturnExchangeReceiptDate);
				if (isReturnable == false)
				{
					strErrorMessages = WebSanitizer.HtmlEncode(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_AMAZON_PAY_CANNOT_RETURN_PRODUCTS, liOrderPaymentKbn.Text));
				}
			}
			tbdyErrorMessages.Visible = (strErrorMessages.Length != 0);
			lbErrorMessages.Text = strErrorMessages;
			if (strErrorMessages.Length != 0)
			{
				return;	// エラーの場合は抜ける
			}

			// 付帯情報必須チェック
			var index = 1;
			if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED)
			{
				var exchangeErrorMessages = new StringBuilder();
				foreach (var exchangeOrderItem in exchangeOrderItems)
				{
					var productOptionSettingList
						= ProductOptionSettingHelper.GetProductOptionSettingList(
							this.LoginOperatorShopId,
							exchangeOrderItem.ProductId,
							exchangeOrderItem.ProductOptionValue);
					var productOptionNecessaryErrorMessages = ValidateProductOptionNecessary(productOptionSettingList, index);
					if (string.IsNullOrEmpty(productOptionNecessaryErrorMessages) == false)
					{
						exchangeErrorMessages.Append(productOptionNecessaryErrorMessages);
					}
					index++;
				}

				if (exchangeErrorMessages.Length != 0)
				{
					tbdyExchangeOrderItemErrorMessages.Visible = true;
					lbExchangeOrderItemErrorMessages.Text = exchangeErrorMessages.ToString();
					return;
				}
			}

			// クレジットカード売上確定後キャンセル可能かチェック
			strErrorMessages = CheckCancelableForCreditCardSalesCompleteOrder(this.Input.ExecuteType);
			tbdyErrorMessages.Visible = (strErrorMessages.Length != 0);
			lbErrorMessages.Text = strErrorMessages;
			if (strErrorMessages.Length != 0)
			{
				return;	// エラーの場合は抜ける
			}

			// PayPalの場合、PayPalアカウントとの紐づきをチェック
			if (liOrderPaymentKbn.Value == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
			{
				var accountEmail = PayPalUtility.Account.GetCooperateAccountEmail(this.User.UserId);
				if (string.IsNullOrEmpty(accountEmail))
				{
					lbPaymentAlertMessage.Visible = true;
					lbPaymentAlertMessage.Text = WebSanitizer.HtmlEncode(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PAYPAL_ISNOT_LINK_ACCOUNT));
					return;
				}
			}

			// GMOアトカラ
			if ((liOrderPaymentKbn.Value == Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA)
				&& (this.Input.ExecuteType != ReauthCreatorFacade.ExecuteTypes.None.ToString()))
			{
				tbdyErrorMessages.Visible = true;
				lbErrorMessages.Text = WebSanitizer.HtmlEncode(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_GMOATOKARA_NOT_EXTERNAL));
				return;
			}

			//------------------------------------------------------
			// 各情報を格納
			//------------------------------------------------------
			// 基本情報
			htOrder.Add(Constants.FIELD_ORDER_CREDIT_BRANCH_NO, null);
			htOrder.Add(Constants.FIELD_ORDER_CARD_INSTRUMENTS, string.Empty);
			htOrder.Add(Constants.FIELD_ORDER_CARD_INSTALLMENTS_CODE, string.Empty);
			// カード枝番、クレジットカードの場合、カード支払い回数セット
			if (Constants.PAYMENT_REAUTH_ENABLED
				&& this.ReturnExchangeOrderOrg.IsPermitReauthOrderSiteKbn
				&& this.IsAllLastAuthFlgValid
				&& (liOrderPaymentKbn.Value == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
				&& (Constants.REAUTH_COMPLETE_CREDITCARD_LIST.Contains(Constants.PAYMENT_CARD_KBN)))
			{
				if (ddlUserCreditCard.SelectedValue != CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
				{
					htOrder[Constants.FIELD_ORDER_CREDIT_BRANCH_NO] = ddlUserCreditCard.SelectedValue;
				}
				var installments = ValueText.GetValueText(Constants.TABLE_ORDER, OrderCommon.CreditInstallmentsValueTextFieldName, dllCreditInstallments.SelectedValue);
				htOrder[Constants.FIELD_ORDER_CARD_INSTRUMENTS] = installments;
				htOrder[Constants.FIELD_ORDER_CARD_INSTALLMENTS_CODE] = dllCreditInstallments.SelectedValue;
				if (ddlUserCreditCard.SelectedValue == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
				{
					if ((this.CanUseCreditCardNoForm == false) && (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Zeus)) return;

					var orderCreditCardInput = GetOrderCreditCardInputForOrderPage(
						this.ReturnExchangeOrderOrg.OrderPaymentKbn,
						this.ReturnExchangeOrderOrg.UserId);

					if (this.IsUserPayTg)
					{
						orderCreditCardInput.CreditToken = Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans
							? CartPayment.CreditTokenInfoBase.CreateCreditTokenInfo(" " + hfPayTgSendId.Value)
							: CartPayment.CreditTokenInfoBase.CreateCreditTokenInfo(this.CreditTokenbyPayTg);
						if ((Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten)
							&& (ddlUserCreditCard.SelectedValue == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW))
						{
							orderCreditCardInput.ExpireMonth = ddlCreditExpireMonth.SelectedValue;
							orderCreditCardInput.ExpireYear = ddlCreditExpireYear.SelectedValue;
							orderCreditCardInput.CompanyCode = this.CreditCardCompanyCodebyPayTg;
						}

					}

					htOrder["order_creadit_card"] = orderCreditCardInput;

					var errorMessageHtmlEncoded = orderCreditCardInput.Validate();

					// トークンが取得できていないときはエラーとして扱う(バグ#3554対策)
					if (OrderCommon.CreditTokenUse && (orderCreditCardInput.CreditToken == null))
					{
						errorMessageHtmlEncoded += WebSanitizer.HtmlEncodeChangeToBr(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CARDAUTH_ERROR));
					}
					if (string.IsNullOrEmpty(errorMessageHtmlEncoded) == false)
					{
						spanErrorMessageForCreditCard.InnerHtml = errorMessageHtmlEncoded;
						spanErrorMessageForCreditCard.Style["display"] = "block";
						return;
					}
				}
			}

			if (this.IsUserPayTg)
			{
				var errorMessage = (this.PayTgResponse == null)
					? string.Empty
					: (string)this.PayTgResponse[VeriTransConst.PAYTG_RESPONSE_ERROR] ?? string.Empty;
				if (string.IsNullOrEmpty(errorMessage) == false)
				{
					spanErrorMessageForPayTg.InnerHtml = WebSanitizer.HtmlEncodeChangeToBr(errorMessage);
					spanErrorMessageForPayTg.Style["display"] = "block";
					return;
				}
			}

			htOrder.Add(Constants.FIELD_ORDER_RETURN_EXCHANGE_KBN, rblReturnExchangeKbn.SelectedValue);					// 返品交換区分
			htOrder.Add(Constants.FIELD_ORDER_RETURN_EXCHANGE_REASON_KBN, rblReturnExchangeReasonKbn.SelectedValue);	// 返品交換都合区分
			htOrder.Add(Constants.FIELD_ORDER_RETURN_EXCHANGE_REASON_MEMO, StringUtility.RemoveUnavailableControlCode(tbReturnExchangeReasonMemo.Text));	// 返品交換理由メモ

			// 返金先口座情報
			var repaymentBank = new Dictionary<string, string>
			{
				{ Constants.CONST_ORDER_REPAYMENT_BANK_CODE, this.Input.RepaymentBankCode },
				{ Constants.CONST_ORDER_REPAYMENT_BANK_NAME, this.Input.RepaymentBankName },
				{ Constants.CONST_ORDER_REPAYMENT_BANK_BRANCH, this.Input.RepaymentBankBranch },
				{ Constants.CONST_ORDER_REPAYMENT_BANK_ACCOUNT_NO, this.Input.RepaymentBankAccountNo },
				{ Constants.CONST_ORDER_REPAYMENT_BANK_ACCOUNT_NAME, this.Input.RepaymentBankAccountName },
			};

			htOrder.Add(Constants.FIELD_ORDER_REPAYMENT_MEMO,
				(rblRepaymentMemoType.SelectedValue == Constants.FLG_ORDER_REPAYMENT_MEMO_TYPE_REPAYMENT_BANK_TEXT)
					? JsonConvert.SerializeObject(repaymentBank)
					: StringUtility.RemoveUnavailableControlCode(this.Input.RepaymentMemo));	// 返金メモ

			// カートオブジェクト作成
			var cart = CreateCart();

			htOrder.Add(Constants.TABLE_ORDERPRICEBYTAXRATE, returnPriceByTaxRateList);	// 税率毎価格情報
			htOrder.Add(Constants.CONST_ORDER_RETURN_ORDERITEMS, returnOrderItems); // 返品商品情報

			// 定額頒布会以外で計算
			var fixedAmountItemExcluded = returnOrderItems
				.Where(item => item.IsSubscriptionBoxFixedAmount == false)
				.ToArray();
			var dReturnOrderPriceSubTotal = fixedAmountItemExcluded.Sum(x => x.ItemPrice);

			// 定額頒布会分を加算
			var fixedAmountItemSubtotal = returnOrderItems
				.Where(item => IsFixedAmountCourseItemAllReturns(item.SubscriptionBoxCourseId))
				.GroupBy(item => item.SubscriptionBoxCourseId)
				.Sum(items => items.First().SubscriptionBoxFixedAmount.Value * -1);
			dReturnOrderPriceSubTotal += fixedAmountItemSubtotal;

			var dReturnOrderPriceSubTotalTax = fixedAmountItemExcluded.Sum(x => x.ItemPriceTax);
			// HACK:返品商品は注文アイテム数、注文商品数にカウントしない？？

			// 交換商品情報取得
			var orderItemCount = 0;
			var orderProductCount = 0;
			var exchangeOrderPriceSubTotal = 0m;
			var exchangeOrderPriceSubTotalTax = 0m;

			if (this.Input.OrderReturnExchangeKbn == Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE)
			{
				if (ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU)
				{
					lbPaymentAlertMessage.Visible = true;
					lbPaymentAlertMessage.Text = WebSanitizer.HtmlEncode(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PAYMENT_BOKU_ONLY_RETURN_ALL))
						.Replace("@@ 1 @@", "Boku");
					return;
				}
				else
				{
					lbPaymentAlertMessage.Visible = false;
				}

				htOrder.Add(Constants.CONST_ORDER_EXCHANGE_ORDERITEMS, exchangeOrderItems);	// 交換商品情報

				// 商品合計、注文アイテム数、注文商品に加算
				exchangeOrderPriceSubTotal = exchangeOrderItems.Sum(x => x.ItemPrice);
				exchangeOrderPriceSubTotalTax = exchangeOrderItems.Sum(x => x.ItemPriceTax);
				orderProductCount = exchangeOrderItems.Sum(x => x.ItemQuantity);
				orderItemCount = exchangeOrderItems.Count;
			}

			if (rblReturnExchangeKbn.SelectedValue == Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN)
			{
				var isReturnAll = ((CheckBox)rReturnOrderItem.Items[0].FindControl("cbReturnProductAll"));
				if ((isReturnAll.Checked == false)
					&& (ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU))
				{
					lbPaymentAlertMessage.Visible = true;
					lbPaymentAlertMessage.Text = WebSanitizer.HtmlEncode(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PAYMENT_BOKU_ONLY_RETURN_ALL))
						.Replace("@@ 1 @@", "Boku");
					return;
				}
				else
				{
					lbPaymentAlertMessage.Visible = false;
				}
			}

			htOrder.Add(FIELD_ORDER_ORDER_PRICE_SUBTOTAL_RETURN, (dReturnOrderPriceSubTotal).ToString());	// 返品商品合計
			htOrder.Add(FIELD_ORDER_ORDER_PRICE_SUBTOTAL_EXCHANGE, (exchangeOrderPriceSubTotal).ToString());	// 交換商品合計
			htOrder.Add(Constants.FIELD_ORDER_ORDER_ITEM_COUNT, orderItemCount);	// 注文アイテム数
			htOrder.Add(Constants.FIELD_ORDER_ORDER_PRODUCT_COUNT, orderProductCount);	// 注文商品数

			// 返品交換合計情報
			htOrder.Add(Constants.FIELD_ORDER_ORDER_PRICE_REGULATION, "0");	// 調整金額
			var returnExchangeOrderPriceSubTotal = this.Input.CalculateReturnExchangeOrderPriceSubTotal(
				dReturnOrderPriceSubTotal,
				exchangeOrderPriceSubTotal);
			var returnExchangeOrderPriceTaxSubTotal = this.Input.CalculateReturnExchangeOrderPriceTaxSubTotal(
				dReturnOrderPriceSubTotalTax,
				exchangeOrderPriceSubTotalTax);
			var returnExchangeOrderPriceTotal = this.Input.CalculateReturnExchangeOrderPriceTotal(
				returnExchangeOrderPriceSubTotal,
				returnExchangeOrderPriceTaxSubTotal,
				this.Input.AdjustmentPointPrice,
				this.Input.ReturnPriceCorrectionTotal);
			var orderPriceTax = returnPriceByTaxRateList.Sum(priceByRate => (decimal)priceByRate.TaxPriceByRate);

			htOrder.Add(Constants.FIELD_ORDER_ORDER_PRICE_TOTAL, returnExchangeOrderPriceTotal.ToString());	// 合計金額
			htOrder.Add(Constants.FIELD_ORDER_ORDER_PRICE_TAX, orderPriceTax.ToString());	// 消費税額
			htOrder.Add(Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL, returnExchangeOrderPriceSubTotal.ToString());	// 商品合計
			htOrder.Add(Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL_TAX, returnExchangeOrderPriceTaxSubTotal.ToString());	// 商品消費税額
			htOrder.Add(Constants.FIELD_ORDER_ORDER_PRICE_REPAYMENT, this.Input.OrderPriceRepayment);	// 返金金額
			htOrder.Add(Constants.FIELD_ORDER_REGULATION_MEMO, StringUtility.RemoveUnavailableControlCode(this.Input.RegulationMemo));	// 調整金額メモ

			// 返品金額が0以下の場合は、返金ステータスは「返金無し」
			// 返品金額が1以上の場合は、返金ステータスは「未返金」
			htOrder.Add(Constants.FIELD_ORDER_ORDER_REPAYMENT_STATUS, decimal.Parse(this.Input.OrderPriceRepayment) > 0 ?	
				Constants.FLG_ORDER_ORDER_REPAYMENT_STATUS_CONFIRM : Constants.FLG_ORDER_ORDER_REPAYMENT_STATUS_NOREPAYMENT);	// 返金ステータス

			// 最終請求額
			var lastBilledAmount = decimal.Parse(this.ReturnExchangeOrderOrg.RelatedOrderLastBilledAmount)
				+ returnExchangeOrderPriceTotal;
			var isReturnAllItems = this.Input.IsReturnAllItems();
			htOrder.Add(Constants.FIELD_ORDER_LAST_BILLED_AMOUNT, lastBilledAmount.ToString());
			htOrder.Add("is_return_all_items", isReturnAllItems);
			htOrder.Add(Constants.FIELD_ORDER_LAST_AUTH_FLG, Constants.FLG_ORDER_LAST_AUTH_FLG_OFF);

			// クーポン情報格納
			OrderCoupon orderCoupon = null;
			if (Constants.W2MP_POINT_OPTION_ENABLED)
			{
				// 元注文で回数制限ありクーポンを利用している？
				if ((this.ReturnExchangeOrderOrg.Coupon != null)
					&& (this.ReturnExchangeOrderOrg.Coupon.IsCouponLimit
					|| this.ReturnExchangeOrderOrg.Coupon.IsCouponAllLimit
					|| this.ReturnExchangeOrderOrg.Coupon.IsBlacklistCoupon))
				{
					// クーポン利用条件を満たしていない or 全返品
					if ((cart.Coupon == null) || isReturnAllItems)
					{
						orderCoupon = OrderCommon.GetOrderCoupon(this.ReturnExchangeOrderOrg);
					}
				}
			}
			htOrder[Constants.TABLE_ORDERCOUPON] = orderCoupon;

			// 入力されたポイント情報を取得して格納
			if (Constants.W2MP_POINT_OPTION_ENABLED)
			{
				// 返品時調整ポイント
				{
					if (this.Input.IsOrderPointAddReCalculate)
					{
						// 付与ポイントセット
						SetCartInfoToOrderPointAdd(
							cart,
							this.Input.OrderTempPoints,
							this.Input.OrderBasePointAddBefore,
							this.Input.OrderLimitPointAddBefore);
					}

					// 仮ポイントが付与されている場合
					var returnPointsTemp = new List<List<Hashtable>>();
					Hashtable returnBasePointComp = null;
					Hashtable returnLimitPointComp = null;
					if (this.OrderOrg.VisibleOrderPointAddTemp)
					{
						returnPointsTemp.Add(CreateReturnAddPointTemp(
							this.ReturnExchangeOrderOrg.UserId,
							this.ReturnExchangeOrderOrg.OrderId,
							this.Input.OrderTempPoints));
					}
					// 既に本ポイントに移行している場合
					else if (this.OrderOrg.VisibleOrderPointAddComp)
					{
						returnBasePointComp = new Hashtable
						{
							{ Constants.FIELD_USERPOINT_USER_ID, this.ReturnExchangeOrderOrg.UserId },
							{ Constants.FIELD_ORDER_ORDER_ID, this.ReturnExchangeOrderOrg.OrderId },
							{ Constants.FIELD_USERPOINT_POINT, decimal.Parse(this.Input.OrderBasePointAdd) },
							{ Constants.FIELD_USERPOINT_POINT + Constants.FIELD_COMMON_BEFORE, decimal.Parse(this.Input.OrderBasePointAddBefore) },
							{ CONST_ORDER_POINT_ADD_ADJUSTMENT, decimal.Parse(this.Input.OrderBasePointAdd) - decimal.Parse(this.Input.OrderBasePointAddBefore) }
						};
						returnLimitPointComp = new Hashtable
						{
							{ Constants.FIELD_USERPOINT_USER_ID, this.ReturnExchangeOrderOrg.UserId },
							{ Constants.FIELD_ORDER_ORDER_ID, this.ReturnExchangeOrderOrg.OrderId },
							{ Constants.FIELD_USERPOINT_POINT, decimal.Parse(this.Input.OrderLimitPointAdd) },
							{ Constants.FIELD_USERPOINT_POINT + Constants.FIELD_COMMON_BEFORE, decimal.Parse(this.Input.OrderLimitPointAddBefore) },
							{ CONST_ORDER_POINT_ADD_ADJUSTMENT, decimal.Parse(this.Input.OrderLimitPointAdd) - decimal.Parse(this.Input.OrderLimitPointAddBefore) }
						};
					}

					// 返品時調整ポイント作成を格納
					htOrder.Add(CONST_ORDER_POINT_ADD_TEMP, returnPointsTemp);
					htOrder.Add(CONST_ORDER_BASE_POINT_ADD_COMP, returnBasePointComp);
					htOrder.Add(CONST_ORDER_LIMIT_POINT_ADD_COMP, returnLimitPointComp);
				}

				// 最終利用ポイント数
				var lastOrderPointUseBefore = decimal.Parse(this.ReturnExchangeOrderOrg.RelatedOrderLastOrderPointUse);
				var lastOrderPointUse = decimal.Parse(this.Input.LastOrderPointUse);
				var orderPointUseAjustment = (lastOrderPointUse - lastOrderPointUseBefore);
				htOrder.Add(Constants.FIELD_ORDER_LAST_ORDER_POINT_USE, lastOrderPointUse);
				htOrder.Add(CONST_ORDER_ORDER_POINT_USE_BEFORE, lastOrderPointUseBefore);
				htOrder.Add(CONST_ORDER_ORDER_POINT_USE_ADJUSTMENT, orderPointUseAjustment);
				// 最終ポイント利用額
				var sv = new PointService();
				var pointMaster = sv.GetPointMaster()
					.FirstOrDefault(x => (x.DeptId == this.LoginOperatorDeptId));
				var lastOrderPointUsePrice = PointOptionUtility.GetOrderPointUsePriceDecimal(lastOrderPointUse, pointMaster);
				htOrder.Add(Constants.FIELD_ORDER_LAST_ORDER_POINT_USE_YEN, lastOrderPointUsePrice);
				htOrder.Add(Constants.FIELD_ORDER_ORDER_POINT_USE, orderPointUseAjustment);
				var orderPointUseAjustmentPrice = PointOptionUtility.GetOrderPointUsePriceDecimal(orderPointUseAjustment, pointMaster);
				htOrder.Add(Constants.FIELD_ORDER_ORDER_POINT_USE_YEN, orderPointUseAjustmentPrice);
			}
			else
			{
				// ポイントOPがオフの場合、固定で0設定
				htOrder.Add(Constants.FIELD_ORDER_LAST_ORDER_POINT_USE, 0);
				htOrder.Add(Constants.FIELD_ORDER_LAST_ORDER_POINT_USE_YEN, 0);
				htOrder.Add(Constants.FIELD_ORDER_ORDER_POINT_USE, 0);
				htOrder.Add(Constants.FIELD_ORDER_ORDER_POINT_USE_YEN, 0);
			}

			htOrder.Add(Constants.FIELD_ORDER_ORDER_COUNT_ORDER, null);
			// Return Global Owner Address
			OrderCommon.ReturnGlobalOwnerAddress(htOrder, this.ReturnExchangeOrderOrg);
			
			// 税率毎最終請求金額
			htOrder.Add("last_billed_price_by_tax_rate", lastBilledPriceByTaxRateList);

			// 請求書同梱フラグ
			string invoiceBundleFlg;
			if (Constants.PAYMENT_SETTING_ATODENE_USE_INVOICE_BUNDLE_SERVICE
				&& Constants.PAYMENT_REAUTH_ENABLED
				&& (this.Input.OrderReturnExchangeKbn!= Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN)
				&& (lastBilledAmount > 0m))
			{
				invoiceBundleFlg = ReturnExchangeOrderOrg.InvoiceBundleFlg;
			}
			else
			{
				invoiceBundleFlg = Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF;
			}
			htOrder.Add(Constants.FIELD_ORDER_INVOICE_BUNDLE_FLG, invoiceBundleFlg);

			// 注文返品交換情報をセッションに格納
			var parameters = new Hashtable()
			{
				{ Constants.CONST_ORDER_RETURN_EXCHANGE_DATA, htOrder },
				{ "execute_type", this.Input.ExecuteType }
			};
			Session[Constants.SESSION_KEY_PARAM] = parameters;

			if (Constants.PAYMENT_PAIDY_OPTION_ENABLED
				&& (Constants.GLOBAL_OPTION_ENABLE == false)
				&& PaygentUtility.CheckIsPaidyPaygentPayment(ddlOrderPaymentKbn.SelectedValue))
			{
				CheckPaidyPaymentPriceChange(lastBilledAmount);
				if (this.lbPaymentAlertMessage.Text.Length != 0)
				{
					this.lbPaymentAlertMessage.Visible = true;
					return;
				}
			}

			// 連続投稿防止用
			Session[Constants.SESSION_KEY_ORDER_REGIST_INPUT_ERROR] = ""; // 空にする（この処理を通すことで連続制限にひっかからない）

			// 確認画面へ遷移
			StringBuilder sbConfirmUrl = new StringBuilder();
			sbConfirmUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_ORDER_RETRUN_EXCHANGE_CONFIRM);
			sbConfirmUrl.Append("?").Append(Constants.REQUEST_KEY_ORDER_ID).Append("=").Append(this.OrderIdOrg);
			sbConfirmUrl.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_DETAIL);

			Response.Redirect(sbConfirmUrl.ToString());
		}
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
				this.OrderIdOrg,
				false,
				false));
	}

	/// <summary>
	/// 交換商品取得時の商品チェック
	/// </summary>
	/// <param name="targetIndex">交換商品の画面上インデックス</param>
	private void CheckExchangeItemRegisterable(string targetIndex)
	{
		var returnOrderPriceSubTotal = 0m;
		var returnOrderPriceTaxSubTotal = 0m;

		// 返品商品と交換商品の情報を取得
		var shippingCountryIsoCode = "";
		var shippingProvision = "";
		var shippingCountryName = "";

		var orderItems = this.Input.CreateAllReturnOrderItems();

		foreach (var item in orderItems)
		{
			// 返品対象の場合
			if (item.ReturnStatus == Constants.FLG_ORDER_RETURN_STATUS_RETURN_TARGET)
			{
				item.ItemPriceTax = -item.ItemPriceTax;
				item.ItemQuantity = -item.ItemQuantity;
				item.ItemReturnExchangeKbn = Constants.FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_RETURN;
				
				returnOrderPriceSubTotal += item.ItemPrice;
				returnOrderPriceTaxSubTotal += item.ItemPriceTax;

				shippingCountryIsoCode = item.ShippingCountryIsoCode;
				shippingProvision = item.ShippingAddr5;
				shippingCountryName = item.ShippingCountryName;
				// 返品交換対象配送先IDを設定
				ReturnExchangeShippingNo = item.OrderShippingNo;
			}
		}

		// 総額計算等のために交換商品を全て取得する
		var exchangeOrderItems = this.Input.CreateExchangeOrderItems(
			this.ReturnExchangeShippingNo,
			this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem());

		// 詳細を取得する商品を特定
		var exchangeTarget = exchangeOrderItems.FirstOrDefault(x => (x.ItemIndex == targetIndex));

		// 商品の情報をクラスにまとめる
		exchangeTarget.ItemReturnExchangeKbn = Constants.FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_EXCHANGE;
		exchangeTarget.ShippingAddr5 = shippingProvision;
		exchangeTarget.SenderCountryIsoCode = shippingCountryIsoCode;
		exchangeTarget.ShippingCountryName = shippingCountryName;

		// 判定のためリストに加える
		var exchangeOrderItemsForCheck = new List<ReturnOrderItem>();
		exchangeOrderItemsForCheck.Add(exchangeTarget);

		// 交換商品チェック
		StringBuilder exchangeErrorMessages = new StringBuilder();
		exchangeErrorMessages.Append(CheckExchangeOrderItem(exchangeOrderItemsForCheck));

		//------------------------------------------------------ 
		// 交換商品の金額を計算し商品の小計と配送の合計に反映
		//------------------------------------------------------ 
		// 定期購入可否整合性チェック
		DataView dvProductVariation = GetProductVariation(
			this.LoginOperatorShopId,
			exchangeTarget.ProductId,
			exchangeTarget.VariationId);
		var productName = StringUtility.ToEmpty(dvProductVariation[0][Constants.FIELD_PRODUCT_NAME]);
		var productFixedPurchaseFlg = dvProductVariation[0][Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG].ToString();
		var isFixedPurchaseValid = (StringUtility.ToEmpty(exchangeTarget.FixedPurchaseProductFlg) == Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON);
		exchangeErrorMessages.Append(CheckFixedPurchaseFlgValid(productName, productFixedPurchaseFlg, isFixedPurchaseValid));

		// 商品有効性チェック
		if ((string)dvProductVariation[0][Constants.FIELD_PRODUCT_VALID_FLG] == Constants.FLG_PRODUCT_VALID_FLG_INVALID)
		{
			exchangeErrorMessages.Append(
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_INVALID).Replace("@@ 1 @@", productName));
		}

		// 返品用金額チェック
		var priceCorrectionErrorMessage = this.Input.CheckInputProductCorrectionPrice();
		lbReturnExchangeErrorMessages.Text = priceCorrectionErrorMessage;
		tbdyReturnExchangeErrorMessages.Visible = (priceCorrectionErrorMessage != "");

		if ((exchangeErrorMessages.Length == 0) && (string.IsNullOrEmpty(priceCorrectionErrorMessage)))
		{
			// カートオブジェクト作成
			var cart = CreateCart();
			var success = true;
			// ポイントOPが有効？
			if (Constants.W2MP_POINT_OPTION_ENABLED)
			{
				// 付与ポイント自動計算あり AND 再計算ボタンクリック時
				if (this.Input.IsOrderPointAddReCalculate)
				{
					// 付与ポイントセット
					SetCartInfoToOrderPointAdd(
						cart,
						this.Input.OrderTempPoints,
						this.Input.OrderBasePointAddBefore,
						this.Input.OrderLimitPointAddBefore);
				}

				// 利用ポイントセット
				success = SetCartInfoToOrderPointUse(cart);
			}
			if (success)
			{
				// 交換商品情報設定
				rExchangeOrderItem.DataSource = exchangeOrderItems;
				rExchangeOrderItem.DataBind();

					// 交換商品合計額算出
				var exchangeOrderPriceSubTotal = exchangeOrderItems.Sum(x => PriceCalculator.GetItemPrice(x.ProductPrice, x.ItemQuantity));
				var exchangeOrderPriceTaxSubTotal = exchangeOrderItems.Sum(x => x.ItemPriceTax);

				// 交換商品合計額設定
				lbExchangeOrderPriceSubTotal.Text = exchangeOrderPriceSubTotal.ToPriceString(true);

				var returnExchangeOrderPriceSubTotal = this.Input.CalculateReturnExchangeOrderPriceSubTotal(
					returnOrderPriceSubTotal,
					exchangeOrderPriceSubTotal);

				var returnExchangeOrderPriceTaxSubTotal = this.Input.CalculateReturnExchangeOrderPriceTaxSubTotal(
					returnOrderPriceTaxSubTotal,
					exchangeOrderPriceTaxSubTotal);

				var returnExchangeOrderPriceTotal = this.Input.CalculateReturnExchangeOrderPriceTotal(
					returnExchangeOrderPriceSubTotal,
					returnExchangeOrderPriceTaxSubTotal,
					this.Input.AdjustmentPointPrice,
					this.Input.ReturnPriceCorrectionTotal);

				// 返品交換合計情報設定
				lbReturnExchangeOrderPriceSubTotal.ForeColor = GetPriceDisplayColor(returnExchangeOrderPriceSubTotal);
				lbReturnExchangeOrderPriceSubTotal.Text = returnExchangeOrderPriceSubTotal.ToPriceString(true);	// 返品商品合計 + 交換商品合計
				lbReturnExchangeOrderPriceTax.ForeColor = GetPriceDisplayColor(returnExchangeOrderPriceTaxSubTotal);
				lbReturnExchangeOrderPriceTax.Text = returnExchangeOrderPriceTaxSubTotal.ToPriceString(true);	// 返品消費税合計 + 交換消費税合計
				lbReturnExchangeOrderPriceTotal.ForeColor = GetPriceDisplayColor(returnExchangeOrderPriceTotal);
				lbReturnExchangeOrderPriceTotal.Text = returnExchangeOrderPriceTotal.ToPriceString(true);
			}
		}

		// エラーメッセージ制御
		lbExchangeOrderItemErrorMessages.Text = exchangeErrorMessages.ToString();
		tbdyExchangeOrderItemErrorMessages.Visible = (exchangeErrorMessages.ToString() != "");
	}

	/// <summary>
	/// 商品定期チェック変更処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbFixedPurchaseProductFlg_CheckedChanged(object sender, EventArgs e)
	{
		var repeater = ((CheckBox)sender).Parent;

		// チェック状態取得
		bool isFixedPurchaseValid = ((CheckBox)sender).Checked;

		// 商品情報取得
		DataView dvProductVariation = GetProductVariation(
			this.LoginOperatorShopId,
			((TextBox)repeater.FindControl("tbProductId")).Text,
			((TextBox)repeater.FindControl("tbProductId")).Text + ((TextBox)repeater.FindControl("tbVId")).Text);

		// 定期チェック有無に応じた商品名に編集
		var productNameTextBox = (TextBox)repeater.FindControl("tbProductName");
		productNameTextBox.Text = CreateFixedPurchaseProductName(productNameTextBox.Text, isFixedPurchaseValid, this.IsSubscriptionBoxValid);

		if (dvProductVariation.Count > 0)
		{
			var productName = StringUtility.ToEmpty(dvProductVariation[0][Constants.FIELD_PRODUCT_NAME]);
			var productFixedPurchaseFlg = dvProductVariation[0][Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG].ToString();

			// 定期購入フラグ整合性チェック
			tbdyExchangeOrderItemErrorMessages.Visible = false;
			var errMessage = CheckFixedPurchaseFlgValid(productName, productFixedPurchaseFlg, isFixedPurchaseValid);
			if (errMessage != "")
			{
				lbExchangeOrderItemErrorMessages.Text = errMessage;
				tbdyExchangeOrderItemErrorMessages.Visible = true;
				return;
			}

			// 定期チェック有無に応じた商品価格に更新(商品の定期購入可否設定とチェック有無が不整合の場合は更新しない)
			var productPriceTextBox = (TextBox)repeater.FindControl("tbProductPrice");
			if ((isFixedPurchaseValid) && (productFixedPurchaseFlg != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID))
			{
				var productId = dvProductVariation[0][Constants.FIELD_PRODUCT_PRODUCT_ID].ToString();
				var variationId = dvProductVariation[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_ID].ToString();
				var OrderItem = this.ReturnExchangeOrderOrg.Items.FirstOrDefault(i => (i.ProductId == productId) && (i.VariationId == variationId));
				var isFirstOrder = ((OrderItem == null) || OrderItem.FixedPurchaseItemIsFirstOrder);
				productPriceTextBox.Text = GetFixedPurchaseProductValidityPrice(dvProductVariation[0], isFirstOrder).ToPriceString();
			}
			else if ((isFixedPurchaseValid == false) && (productFixedPurchaseFlg != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_ONLY))
			{
				productPriceTextBox.Text = GetProductValidityPrice(dvProductVariation[0]).ToPriceString();
			}
		}
	}

	/// <summary>
	/// 定期購入フラグ整合性チェック
	/// </summary>
	/// <param name="productName">商品名</param>
	/// <param name="productFixedPurchaseFlg">商品定期購入可否</param>
	/// <param name="isFixedPurchase">定期チェック有無</param>
	/// <returns>エラーメッセージ(エラーがない場合空文字)</returns>
	private string CheckFixedPurchaseFlgValid(string productName, string productFixedPurchaseFlg, bool isFixedPurchase)
	{
		if ((isFixedPurchase) && (productFixedPurchaseFlg == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID))
		{
			return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_FIXED_PURCHASE_DISABLE).Replace("@@ 1 @@", productName);
		}
		else if ((isFixedPurchase == false) && (productFixedPurchaseFlg == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_ONLY))
		{
			return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_FIXED_PURCHASE_ONLY).Replace("@@ 1 @@", productName);
		}

		return "";
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
				trRegistCreditCard.Visible = trCreditCardName.Visible = (UserService.IsUser(this.User.UserKbn) && (Constants.MAX_NUM_REGIST_CREDITCARD > this.CreditCardNum));
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
					var userCreditCard = new UserCreditCardService().Get(this.ReturnExchangeOrderOrg.UserId, branchNo);
					lCreditCompany.Text =
						WebSanitizer.HtmlEncode(
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
	/// 付与ポイント自動計算チェック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbOrderPointAddReCalculate_CheckedChanged(object sender, EventArgs e)
	{
		foreach (RepeaterItem ri in rOrderPointAddTemp.Items)
		{
			((TextBox)ri.FindControl("tbOrderPointAdd")).Enabled = (this.Input.IsOrderPointAddReCalculate == false);
		}
		tbOrderBasePointAdd.Enabled
			= tbOrderLimitPointAdd.Enabled
			= (this.Input.IsOrderPointAddReCalculate == false);
	}

	/// <summary>
	/// 補正金額の自動計算チェック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbOrderPriceCorrectionReCalculate_CheckedChanged(object sender, EventArgs e)
	{
		foreach (RepeaterItem ri in rPriceCorrection.Items)
		{
			((TextBox)ri.FindControl("tbPriceCorrection")).Enabled = (this.Input.IsOrderPriceCorrectionReCalculate == false);
		}

		tbOrderPriceRepayment.Enabled = (this.Input.IsOrderPriceCorrectionReCalculate == false);
	}

	/// <summary>
	/// クレジットカード売上確定後キャンセル可能か？
	/// キャンセル不可の場合はエラーメッセージ表示
	/// </summary>
	/// <param name="executeType"></param>
	/// <returns>エラーメッセージ</returns>
	private string CheckCancelableForCreditCardSalesCompleteOrder(string executeType)
	{
		// 再与信しない？
		if (executeType == ReauthCreatorFacade.ExecuteTypes.None.ToString()) return "";

		// 元注文情報（元注文 or 最後の返品/交換注文）取得
		var order = OrderCommon.GetLastAuthOrder(this.OrderIdOrg);

		// クレジットカード売上確定後キャンセル可能かできるか？
		var isCancelable = OrderCommon.IsCancelableForCreditCardSalesCompleteOrder(order);
		// エラーの場合、エラーメッセージを返す
		if (isCancelable == false)
		{
			return WebMessages.GetMessages(WebMessages.ERRMSG_CREDIT_SALES_COMPLETE_CANCEL_ERROR);
		}

		return "";
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
	/// カード情報取得ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnGetCardInfo_Click(object sender, EventArgs e)
	{
		if (this.IsUserPayTg == false)
		{
			// カード情報入力チェック
			var orderCreditCardInput = GetOrderCreditCardInputForOrderPage(
				this.ReturnExchangeOrderOrg.OrderPaymentKbn,
				this.ReturnExchangeOrderOrg.UserId);
			var errorMessageHtmlEncoded = orderCreditCardInput.Validate(true, WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_VERITRANS_PAYTG_CARD_UNREGISTERD));
			
			if (string.IsNullOrEmpty(errorMessageHtmlEncoded) == false)
			{
				spanErrorMessageForCreditCard.InnerHtml = errorMessageHtmlEncoded;
				spanErrorMessageForCreditCard.Style["display"] = "block";
				return;
			}
		}

		if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans)
		{
			hfPayTgSendId.Value = new UserCreditCardCooperationInfoVeritrans(this.User.UserId).CooperationId1;

			var tel = this.User.Tel1.Replace("-", "");

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

			// PayTG連動でカード有効性チェック
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

				spanErrorMessageForPayTg.InnerText = string.Empty;
				spanErrorMessageForPayTg.Style["display"] = "none";

				this.IsSuccessfulCardRegistration = true;
			}
			else
			{
				var payTgResponse =
					new Hashtable { { VeriTransConst.PAYTG_RESPONSE_ERROR, payTg.Result.ErrorMessages } };
				this.PayTgResponse = payTgResponse;
				spanErrorMessageForPayTg.InnerText = WebSanitizer.HtmlEncodeChangeToBr(payTg.Result.ErrorMessages);
				spanErrorMessageForPayTg.Style["display"] = "block";
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
				spanErrorMessageForPayTg.InnerText = WebSanitizer.HtmlEncodeChangeToBr(cardErrorMessageForPc);
				spanErrorMessageForPayTg.Style["display"] = "block";
			}
		}
		DisplayCreditInputForm();
	}

	/// <summary>
	/// ユーザー管理レベルで制御されている決済種別注意文言
	/// </summary>
	private void DisplayLimitedPaymentUserManagementLevel()
	{
		string message = GetPaymentUserManagementLevelNotUseMessage(this.LoginOperatorShopId, this.User.UserManagementLevelId);
		lbPaymentUserManagementLevelMessage.Text = message;
	}

	/// <summary>
	/// 注文者区分で制御されている決済種別注意文言
	/// </summary>
	private void DisplayLimitedPaymentOrderOwnerKbn()
	{
		string message = GetPaymentOrderOwnerKbnNotUseMessage(this.LoginOperatorShopId, this.User.UserKbn);
		lbPaymentOrderOwnerKbnMessage.Text = message;
	}

	/// <summary>
	/// 今日が売上確定日から180日未満かをチェックする
	/// </summary>
	/// <param name="orderReturnExchangeReceiptDate">売上確定日</param>
	/// <returns>True：180日未満　False：180日以上</returns>
	private bool CheckOrderRetrunReceiptDateWithinReturnTerms(string orderReturnExchangeReceiptDate)
	{
		var returnExchangeDay = DateTime.Parse(orderReturnExchangeReceiptDate).AddDays(180);
		var result = (returnExchangeDay > DateTime.Now);
		return result;
	}

	/// <summary>
	/// 対象の頒布会定額コースの商品が全返品されるか
	/// </summary>
	/// <param name="subscriptionBoxCourseId">頒布会コースID</param>
	/// <returns>全返品であればTRUE</returns>
	private bool IsFixedAmountCourseItemAllReturns(string subscriptionBoxCourseId)
	{
		// 元注文での数が0の場合、そもそも頒布会定額コースではないためFALSE
		var originalOrderQuantity = this.ReturnExchangeOrderOrg.GetFixedAmountCourseItemQuantityTotal(subscriptionBoxCourseId);
		if (originalOrderQuantity == 0) return false;

		// 返品済み商品
		var returnedOrderQuantity = 0;
		if (this.ReturnExchangeOrderOrg.ReturnedItemQuantityByFixedAmountCourse
				.TryGetValue(subscriptionBoxCourseId, out returnedOrderQuantity) == false) return false;

		var returnOrderCount = this.Input.GetFixedAmountCourseItemQuantityOnReturnItems(subscriptionBoxCourseId);
		return originalOrderQuantity == (returnedOrderQuantity + returnOrderCount);
	}

	#region トークン決済系
	/// <summary>
	/// （トークン決済向け）カード情報編集（再入力）リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbEditCreditCardNoForToken_Click(object sender, System.EventArgs e)
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

	/// <summary>
	/// 商品付帯情報必須チェック
	/// </summary>
	/// <param name="productOptionSettingList">商品付帯情報</param>
	/// <param name="exchangeOrderItemIndex">交換商品インデックス</param>
	/// <returns></returns>
	private string ValidateProductOptionNecessary(ProductOptionSettingList productOptionSettingList, int exchangeOrderItemIndex)
	{
		var errorMessages = new StringBuilder();
		foreach (var productOptionSetting in productOptionSettingList.Items)
		{
			var tmpValueName = productOptionSetting.ValueName;
			productOptionSetting.ValueName = string.Format(
				ReplaceTag("@@DispText.common_message.location_no@@"),
				(exchangeOrderItemIndex).ToString(),
				productOptionSetting.ValueName);

			// 付帯情報必須チェック
			if (productOptionSetting.IsTextBox)
			{
				if (productOptionSetting.IsNecessary && string.IsNullOrEmpty(productOptionSetting.SelectedSettingValue))
				{
					errorMessages.Append(
						WebMessages.GetMessages(WebMessages.ERRMSG_CHECK_INPUT_PRODUCT_OPTION_NO_OPTIONS_SET_ERROR)
							.Replace("@@ 1 @@", productOptionSetting.ValueName));
				}
				productOptionSetting.ValueName = tmpValueName;
			}
			else if (productOptionSetting.IsCheckBox)
			{
				var productOptionSettingCheckBoxList =
					productOptionSetting.SettingValuesListItemCollection.Cast<ListItem>().ToList();
				if (productOptionSetting.IsNecessary && (productOptionSettingCheckBoxList.Count(value => value.Selected) == 0))
				{
					errorMessages.Append(
						WebMessages.GetMessages(WebMessages.ERRMSG_CHECK_INPUT_PRODUCT_OPTION_NO_OPTIONS_SET_ERROR)
							.Replace("@@ 1 @@", productOptionSetting.ValueName));
					productOptionSetting.ValueName = tmpValueName;
				}
			}
			else if (productOptionSetting.IsSelectMenu)
			{
				if (productOptionSetting.IsNecessary && (productOptionSetting.SelectedSettingValue == productOptionSetting.SettingValues.First()))
				{
					errorMessages.Append(
						WebMessages.GetMessages(WebMessages.ERRMSG_CHECK_INPUT_PRODUCT_OPTION_NO_OPTIONS_SET_ERROR)
							.Replace("@@ 1 @@", productOptionSetting.ValueName));
					productOptionSetting.ValueName = tmpValueName;
				}
			}
		}

		return errorMessages.ToString();
	}
	#endregion

	#region 再計算処理

	/// <summary>
	/// 付与ポイントセット
	/// </summary>
	/// <param name="cart">カート情報</param>
	/// <param name="orderTempPoints">仮ポイント情報</param>
	/// <param name="basePointAddBefore">通常付与ポイント（更新前）</param>
	/// <param name="limitPointAddBefore">期間限定付与ポイント（更新前）</param>
	private void SetCartInfoToOrderPointAdd(
		CartObject cart,
		OrderReturnExchangeTempPointInput[] orderTempPoints,
		string basePointAddBefore,
		string limitPointAddBefore)
	{
		// 仮ポイントが付与されている場合
		if (this.OrderOrg.VisibleOrderPointAddTemp)
		{
			var i = 0;
			foreach (RepeaterItem item in rOrderPointAddTemp.Items)
			{
				// ポイント加算区分取得
				var pointIncKbn = orderTempPoints[i].PointIncKbn;

				// 更新前ポイント、調整後ポイント取得
				var beforePoint = decimal.Parse(orderTempPoints[i].OrderPointAddBefore);
				var afterPoint = (pointIncKbn == Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_FIRST_BUY) ? cart.FirstBuyPoint : cart.BuyPoint;

				// 調整ポイント＆調整後ポイントセット
				(((Literal)item.FindControl("lOrderPointAddAdjustment"))).Text = StringUtility.ToNumeric(afterPoint - beforePoint);
				(((TextBox)item.FindControl("tbOrderPointAdd"))).Text = afterPoint.ToString();
				i++;
			}
		}
		// 既に本ポイントに移行している場合
		else if (this.OrderOrg.VisibleOrderPointAddComp)
		{
			var afterBasePoint = 0m;
			var afterLimitPoint = 0m;
			if (cart.BuyPoints != null)
			{
				foreach (var buyPoint in cart.BuyPoints)
				{
					var pointRule = new PointService().GetPointRule(Constants.CONST_DEFAULT_DEPT_ID, buyPoint.Key);
					if (pointRule.PointKbn == Constants.FLG_USERPOINT_POINT_KBN_BASE)
					{
						afterBasePoint += buyPoint.Value;
					}
					else if (pointRule.PointKbn != Constants.FLG_USERPOINT_POINT_KBN_BASE)
					{
						afterLimitPoint += buyPoint.Value;
					}
				}
			}

			// 調整ポイント＆調整後ポイントセット
			lOrderBasePointAddAdjustment.Text = WebSanitizer.HtmlEncode(StringUtility.ToNumeric(afterBasePoint - decimal.Parse(basePointAddBefore)));
			tbOrderBasePointAdd.Text = WebSanitizer.HtmlEncode(afterBasePoint);

			// 調整ポイント＆調整後ポイントセット
			lOrderLimitPointAddAdjustment.Text = WebSanitizer.HtmlEncode(StringUtility.ToNumeric(afterLimitPoint - decimal.Parse(limitPointAddBefore)));
			tbOrderLimitPointAdd.Text = WebSanitizer.HtmlEncode(afterLimitPoint.ToString());
		}
	}

	/// <summary>
	/// 利用ポイントセット
	/// </summary>
	/// <param name="cart">カート情報</param>
	/// <returns>正常：true、エラー：false</returns>
	private bool SetCartInfoToOrderPointUse(CartObject cart)
	{
		// 最終利用ポイント入力チェック
		var orderPointUseErrorMessage = this.Input.CheckLastOrderPointUseInput();

		// 利用ポイント有効性チェックしてカートにセット
		decimal? orderPointUseAdjustment = null;
		if (orderPointUseErrorMessage.Length == 0) orderPointUseErrorMessage += CheckOrderPointUseValidityAndSetToCart(cart, out orderPointUseAdjustment);

		// 正常な場合は利用ポイントセット
		tbdyReturnOrderUsePointErrorMessages.Visible = false;
		if (orderPointUseErrorMessage.Length == 0)
		{
			lOrderPointUseAdjustment.Text = WebSanitizer.HtmlEncode(StringUtility.ToNumeric(orderPointUseAdjustment.Value));
			return true;
		}
		// エラーの場合はエラーメッセージセット
		else
		{
			tbdyReturnOrderUsePointErrorMessages.Visible = true;
			lbReturnOrderUsePointErrorMessages.Text = orderPointUseErrorMessage;
			return false;
		}
	}

	/// <summary>
	/// 利用ポイント有効性チェックしてカートにセット
	/// </summary>
	/// <param name="cart">カート情報</param>
	/// <param name="orderPointUseAdjustment">調整ポイント（変更前最終利用ポイント－変更後最終利用ポイント）</param>
	/// <returns>エラーメッセージ</returns>
	private string CheckOrderPointUseValidityAndSetToCart(CartObject cart, out decimal? orderPointUseAdjustment)
	{
		orderPointUseAdjustment = null;

		// 利用可能ポイント取得（本ポイント + 最終利用ポイント（変更前））
		var userPointComp = new PointService().GetUserPoint(this.ReturnExchangeOrderOrg.UserId, string.Empty)
				.Where(up => up.IsUsableForOrder)
				.Sum(up => up.Point);

		var userPointUsable = decimal.Parse(this.ReturnExchangeOrderOrg.RelatedOrderLastOrderPointUse) + userPointComp;
		userPointUsable = (userPointUsable > 0) ? userPointUsable : 0;

		// （変更後）最終利用ポイント取得
		var lastOrderPointUse = decimal.Parse(this.Input.LastOrderPointUse);

		// 「本ポイント + 最終利用ポイント(変更前) < (最終利用ポイント(変更後)」の場合はエラーとする
		if (userPointUsable < lastOrderPointUse)
		{
			return WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_ORDER_ORDER_POINT_USE_MAX).Replace("@@ 1 @@", StringUtility.ToNumeric(userPointUsable) + "pt");
		}

		// ポイント利用可能単位チェック
		var sv = new PointService();
		var pointMaster = sv.GetPointMaster()
			.FirstOrDefault(x => x.DeptId == this.LoginOperatorDeptId);
		if ((lastOrderPointUse % pointMaster.UsableUnit) != 0)
		{
			return WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_POINT_USABLE_UNIT_ERROR).Replace("@@ 1 @@", StringUtility.ToNumeric(pointMaster.UsableUnit));
		}

		// ポイント利用金額 = ((利用ポイント / ポイント利用可能単位) * ポイント換算率)
		var lastOrderPointUsePrice = PointOptionUtility.GetOrderPointUsePriceDecimal(lastOrderPointUse, pointMaster);

		// ポイント利用範囲チェック
		if (lastOrderPointUsePrice > cart.PointUsablePrice)
		{
			return WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_POINT_PRICE_SUBTOTAL_MINUS_ERROR).Replace("@@ 1 @@", cart.PointUsablePrice.ToPriceString(true));
		}

		// カートにセット（調整ポイントをカートに利用ポイントとしてセット）
		orderPointUseAdjustment = decimal.Parse(this.Input.LastOrderPointUse) - decimal.Parse(this.Input.LastOrderPointUseBefore);

		cart.SetUsePoint(lastOrderPointUse, lastOrderPointUsePrice);

		return string.Empty;
	}

	/// <summary>
	/// カート情報作成
	/// </summary>
	/// <returns>カート情報</returns>
	private CartObject CreateCart()
	{
		// カートオブジェクト作成
		var cart = new CartObject(
			this.ReturnExchangeOrderOrg.UserId,
			this.ReturnExchangeOrderOrg.OrderKbn,
			this.ReturnExchangeOrderOrg.ShopId,
			this.ReturnExchangeOrderOrg.ShippingId,
			false,
			false)
		{
			MallId = ReturnExchangeOrderOrg.MallId,
			SubscriptionBoxCourseId = ReturnExchangeOrderOrg.SubscriptionBoxCourseId,
			SubscriptionBoxFixedAmount = ReturnExchangeOrderOrg.SubscriptionBoxFixedAmount
		};
		cart.IsReturnAllItems = this.Input.IsReturnAllItems();

		// カート商品セット
		SetCartProducts(cart);

		// カート再計算
		CalculateCart(cart);

		// Set Online Payment Status To Cart
		cart.OnlinePaymentStatus = this.ReturnExchangeOrderOrg.OnlinePaymentStatus;

		return cart;
	}

	/// <summary>
	/// カート商品セット
	/// </summary>
	/// <param name="cart">カート情報</param>
	private void SetCartProducts(CartObject cart)
	{
		cart.Shippings[0].UpdateShippingAddr(
			this.OrderOrg.Shippings[0].DataSource,
			true,
			CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW);

		// 返品＆交換商品取得
		var orderItems = new List<ReturnOrderItem>();
		orderItems.AddRange(this.Input.CreateAllReturnOrderItems());
		if (this.Input.OrderReturnExchangeKbn == Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE)
		{
			orderItems.AddRange(this.Input.CreateExchangeOrderItems(
				this.ReturnExchangeShippingNo,
				this.ReturnExchangeOrderOrg.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem()));
		}

		// 返品商品 > 交換商品の順に並び替え
		orderItems = orderItems
			.OrderBy(item => ((item.ReturnStatus == Constants.FLG_ORDER_RETURN_STATUS_RETURN_COMPLETE)
				|| (item.ReturnStatus == Constants.FLG_ORDER_RETURN_STATUS_RETURN_TARGET))
				? 1
				: 0)
			.ToList();

		foreach (var item in orderItems)
		{
			var shopId = item.ShopId;
			var productId = item.ProductId;
			var variationId = item.VariationId;
			var isFixedPurchaseItem =
				(item.FixedPurchaseProductFlg == Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON);
			var productSaleId = item.ProductSaleId;
			var productOptionTexts = item.ProductOptionValue;
			var productBundleId = item.ProductBundleId;

			// 商品ID指定がない場合、次へ
			if (productId == string.Empty) continue;

			// 返品対象の場合、次へ
			var returnStatus = item.ReturnStatus;
			if ((returnStatus == Constants.FLG_ORDER_RETURN_STATUS_RETURN_COMPLETE)
				|| (returnStatus == Constants.FLG_ORDER_RETURN_STATUS_RETURN_TARGET)) continue;

			// カートに追加
			var itemQuantity = int.Parse(item.ItemQuantity.ToString());
			var productPrice = decimal.Parse(item.ProductPrice.ToString());
			var addCartKbn = this.ReturnExchangeOrderOrg.IsGiftOrder
				? w2.App.Common.Constants.AddCartKbn.GiftOrder
				: isFixedPurchaseItem
				? w2.App.Common.Constants.AddCartKbn.FixedPurchase : w2.App.Common.Constants.AddCartKbn.Normal;

			var cartProduct = cart.GetProduct(
				shopId,
				productId,
				variationId,
				false,
				isFixedPurchaseItem,
				productSaleId,
				productOptionTexts,
				productBundleId);

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
					shopId,
					productId,
					variationId,
					this.ReturnExchangeOrderOrg.MemberRankId);
				if (product.Count == 0) continue;
				var cartProductTemp =
					new CartProduct(product[0],
						addCartKbn,
						productSaleId,
						itemQuantity,
						false,
						productOptionTexts,
						isFixedPurchaseItem ? this.ReturnExchangeOrderOrg.FixedPurchaseId : string.Empty)
					{
						NoveltyId = item.NoveltyId,
						RecommendId = item.RecommendId,
						ProductBundleId = item.ProductBundleId,
						BundleItemDisplayType = item.BundleItemDisplayType,
						SubscriptionBoxCourseId = item.SubscriptionBoxCourseId,
						SubscriptionBoxFixedAmount = item.SubscriptionBoxFixedAmount,
					};
				cartProductTemp.SetPrice(productPrice);
				cart.AddVirtural(cartProductTemp);
			}
		}

		// ギフト注文？
		if (this.OrderOrg.Shippings.Count > 1)
		{
			// 配送先追加
			for (int i = 1; i < this.OrderOrg.Shippings.Count; i++)
			{
				var cartShipping = new CartShipping(cart);
				var tel1 = ((string)this.OrderOrg.Shippings[i].ShippingTel1).Split('-');
				var zip = ((string)this.OrderOrg.Shippings[i].ShippingZip).Split('-');
				cartShipping.ShippingAddrKbn = CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW;
				cartShipping.UpdateShippingAddr(
					this.OrderOrg.Shippings[i].DataSource,
					true,
					CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW);

				cart.Shippings.Add(cartShipping);
			}
			// 配送先に紐づける
			int cartShippingIndex = 0;
			this.OrderOrg.Shippings.ForEach(orderShipping =>
			{
				orderItems
					.Where(item => (item.OrderShippingNo == orderShipping.OrderShippingNo)
						&& ((item.ReturnStatus != Constants.FLG_ORDER_RETURN_STATUS_RETURN_COMPLETE)
							&& (item.ReturnStatus != Constants.FLG_ORDER_RETURN_STATUS_RETURN_TARGET)))
					.ToList()
					.ForEach(item =>
					{
						var cartProduct = cart.Items.Find(i =>
							(i.ShopId == item.ShopId)
							&& (i.ProductId == item.ProductId)
							&& (i.VariationId == item.VariationId));
						cart.Shippings[cartShippingIndex].ProductCounts.Add(new CartShipping.ProductCount(cartProduct, item.ItemQuantity));
					});
				cartShippingIndex++;
			});
			// 商品数セット
			cart.Items.ForEach(cartProduct =>
			{
				var productCounts = cart.Shippings.Select(s => s.ProductCounts.Where(p => p.Product == cartProduct).Where(p => p != null));
				var sum = productCounts.Sum(s => s.Sum(p => p.Count));
				cartProduct.CountSingle = sum;
				cartProduct.Calculate();
			});
			cart.CalculateWithCartShipping();
		}
	}

	/// <summary>
	/// カート再計算
	/// </summary>
	/// <param name="cart">カート情報</param>
	private void CalculateCart(CartObject cart)
	{
		// 配送料・決済手数料税率は親注文が生まれた当時の税率を使う
		cart.ShippingTaxRate = decimal.Parse(this.ReturnExchangeOrderOrg.ShippingTaxRate);
		cart.PaymentTaxRate = decimal.Parse(this.ReturnExchangeOrderOrg.PaymentTaxRate);

		var user = new UserService().Get(this.ReturnExchangeOrderOrg.UserId);

		// 利用ポイントセット
		if (Constants.W2MP_POINT_OPTION_ENABLED
			&& (UserService.IsUser(this.User.UserKbn)))
		{
			decimal? orderPointUseAdjustment;
			CheckOrderPointUseValidityAndSetToCart(cart, out orderPointUseAdjustment);
		}

		// クーポンの利用条件を満たしてる場合はクーポンをセット
		if (this.ReturnExchangeOrderOrg.Coupon != null)
		{
			var coupons =
				new CouponService().GetAllUserCouponsFromCouponId(
				this.LoginOperatorDeptId,
				this.ReturnExchangeOrderOrg.UserId,
				this.ReturnExchangeOrderOrg.Coupon.CouponId,
				int.Parse(this.ReturnExchangeOrderOrg.Coupon.CouponNo));
			if (coupons.Length != 0)
			{
				if (CouponOptionUtility.CheckCouponUseConditions(cart, coupons[0]).Length == 0)
				{
					cart.Coupon = new CartCoupon(coupons[0]);
				}
			}
		}

		// カート再計算
		OrderCommon.CalculateCartForReturnExchange(cart, this.ReturnExchangeOrderOrg, user);
	}
	
	/// <summary>
	/// コントローラから画面入力情報を生成します
	/// </summary>
	/// <returns>画面入力情報</returns>
	private OrderReturnExchangeInput CreateInput()
	{
		var input = new OrderReturnExchangeInput();
		input.ExecuteType = GetWrappedControl<WrappedDropDownList>("ddlExecuteType").SelectedValue;
		input.ShopId = GetWrappedControl<WrappedHiddenField>("hfCvsShopId").Value;
		input.OrderReturnExchangeKbn = GetWrappedControl<WrappedRadioButtonList>("rblReturnExchangeKbn").SelectedValue;
		input.OrderReturnExchangeReceiptDate = ucOrderReturnExchangeReceiptDate.DateString;
		input.ReturnExchangeReasonMemo = GetWrappedControl<WrappedTextBox>("tbReturnExchangeReasonMemo").Text;
		input.ReturnExchangeReasonKbn = GetWrappedControl<WrappedRadioButtonList>("rblReturnExchangeReasonKbn").Text;
		input.OrderPaymentKbn = GetWrappedControl<WrappedDropDownList>("ddlOrderPaymentKbn").Text;
		input.CreditToken = GetWrappedControl<WrappedHiddenField>("hfCreditToken").Value;
		input.LastFourDigit = GetWrappedControl<WrappedHiddenField>("hfLastFourDigit").Value;
		input.UserCreditCard = GetWrappedControl<WrappedDropDownList>("ddlUserCreditCard").SelectedValue;
		input.CreditCardCompany = GetWrappedControl<WrappedDropDownList>("ddlCreditCardCompany").SelectedValue;
		input.CreditCardNo1 = GetWrappedControl<WrappedTextBox>("tbCreditCardNo1").Text;
		input.CreditExpireMonth = GetWrappedControl<WrappedDropDownList>("ddlCreditExpireMonth").SelectedValue;
		input.CreditExpireYear = GetWrappedControl<WrappedDropDownList>("ddlCreditExpireYear").SelectedValue;
		input.CreditAuthorName = GetWrappedControl<WrappedTextBox>("tbCreditAuthorName").Text;
		input.CreditSecurityCode = GetWrappedControl<WrappedTextBox>("tbCreditSecurityCode").Text;
		input.CreditInstallments = GetWrappedControl<WrappedDropDownList>("dllCreditInstallments").SelectedValue;
		input.IsRegisterCreditCard = GetWrappedControl<WrappedCheckBox>("cbRegistCreditCard").Checked;
		input.UserCreditCardName = GetWrappedControl<WrappedTextBox>("tbUserCreditCardName").Text;
		input.IsReturnProductAll = GetWrappedControl<WrappedCheckBox>("cbReturnProductAll").Checked;
		var returnRepeater = GetWrappedControl<WrappedRepeater>("rReturnOrderItem");
		input.ReturnItems = returnRepeater.Items
			.Cast<RepeaterItem>()
			.Select(ri =>
			{
				var items = CreateInputOrderReturnExchangeItems(
					GetWrappedControl<WrappedRepeater>(ri, "rReturnOrderItem2"));
				return items;
			})
			.ToArray();

		input.IsOrderPointAddReCalculate = GetWrappedControl<WrappedCheckBox>("cbOrderPointAddReCalculate").Checked;

		var pointRepeater = GetWrappedControl<WrappedRepeater>("rOrderPointAddTemp");
		input.OrderTempPoints = pointRepeater.Items
			.Cast<RepeaterItem>()
			.Select(ri =>
					{
						var item = new OrderReturnExchangeTempPointInput
						{
							OrderPointAdd = GetWrappedControl<WrappedTextBox>(ri, "tbOrderPointAdd").Text,
							PointIncKbn = GetWrappedControl<WrappedHiddenField>(ri, "hfPointIncKbn").Value,
							PointKbnNo = GetWrappedControl<WrappedHiddenField>(ri, "hfPointKbnNo").Value,
							PointKbn = GetWrappedControl<WrappedHiddenField>(ri, "hfPointKbn").Value,
							OrderPointAddBefore = GetWrappedControl<WrappedHiddenField>(ri, "hfOrderPointAdd").Value,
						};
						return item;
					})
			.ToArray();

		input.OrderBasePointAdd = GetWrappedControl<WrappedTextBox>("tbOrderBasePointAdd").Text;
		input.OrderBasePointAddBefore = GetWrappedControl<WrappedHiddenField>("hfOrderBasePointAdd").Value;
		input.OrderLimitPointAdd = GetWrappedControl<WrappedTextBox>("tbOrderLimitPointAdd").Text;
		input.OrderLimitPointAddBefore = GetWrappedControl<WrappedHiddenField>("hfOrderLimitPointAdd").Value;
		input.LastOrderPointUseBefore = GetWrappedControl<WrappedHiddenField>("hfLastOrderPointUse").Value;
		input.LastOrderPointUse = GetWrappedControl<WrappedTextBox>("tbLastOrderPointUse").Text;

		var exchangedRepeater = GetWrappedControl<WrappedRepeater>("rExchangedOrder");
		input.ExchangedItems = exchangedRepeater.Items
			.Cast<RepeaterItem>()
			.Select(ri => 
			{
				var items = CreateInputOrderReturnExchangeItems(
					GetWrappedControl<WrappedRepeater>(ri, "rReturnOrderItemExchanged"));
				return items;
			})
			.ToArray();

		input.ExchangeItems = CreateInputOrderReturnExchangingItems(GetWrappedControl<WrappedRepeater>("rExchangeOrderItem"));

		input.PriceCorrectionItems = GetWrappedControl<WrappedRepeater>("rPriceCorrection").Items
			.Cast<RepeaterItem>()
			.Select(ri =>
			{
				var item = new OrderReturnExchangePriceCorrectionInput();
				item.ProductTaxRate = GetWrappedControl<WrappedHiddenField>(ri, "hfTaxRate").Value;
				item.PriceCorrection = GetWrappedControl<WrappedTextBox>(ri, "tbPriceCorrection").Text;
				return item;
			})
			.ToArray();

		input.ExchangeAddPoint = GetWrappedControl<WrappedTextBox>("tbExchangeAddPoint").Text;
		input.OrderPriceRepayment = GetWrappedControl<WrappedTextBox>("tbOrderPriceRepayment").Text;
		input.RepaymentMemo = GetWrappedControl<WrappedTextBox>("tbRepaymentMemo").Text;
		input.IsOrderPriceCorrectionReCalculate = GetWrappedControl<WrappedCheckBox>("cbOrderPriceCorrectionReCalculate").Checked;
		input.RegulationMemo = GetWrappedControl<WrappedTextBox>("tbRegulationMemo").Text;
		input.RepaymentBankCode = GetWrappedControl<WrappedTextBox>("tbRepaymentBankCode").Text;
		input.RepaymentBankName = GetWrappedControl<WrappedTextBox>("tbRepaymentBankName").Text;
		input.RepaymentBankBranch = GetWrappedControl<WrappedTextBox>("tbRepaymentBankBranch").Text;
		input.RepaymentBankAccountNo = GetWrappedControl<WrappedTextBox>("tbRepaymentBankAccountNo").Text;
		input.RepaymentBankAccountName = GetWrappedControl<WrappedTextBox>("tbRepaymentBankAccountName").Text;

		return input;
	}

	/// <summary>
	/// 返品・交換済商品の入力値取得
	/// </summary>
	/// <param name="repeater">ラップ済みリピーター</param>
	/// <returns>返品・交換商品の入力値</returns>
	private OrderReturnExchangedItemInput[] CreateInputOrderReturnExchangeItems(WrappedRepeater repeater)
				{
		return repeater
			.Items
			.Cast<RepeaterItem>()
			.Select(ri =>
		{
				var item = new OrderReturnExchangedItemInput();

				item.IsReturnProduct = GetWrappedControl<WrappedCheckBox>(ri, "cbReturnProduct").Checked;
				item.ReturnStatusBefore = GetWrappedControl<WrappedHiddenField>(ri, "hfReturnStatus").Value;
				item.ProductId = GetWrappedControl<WrappedHiddenField>(ri, "hfProductId").Value;
				item.VId = GetWrappedControl<WrappedHiddenField>(ri, "hfVId").Value;
				item.SupplierId = GetWrappedControl<WrappedHiddenField>(ri, "hfSupplierId").Value;
				item.ProductName = GetWrappedControl<WrappedHiddenField>(ri, "hfProductName").Value;
				item.ProductNameKana = GetWrappedControl<WrappedHiddenField>(ri, "hfProductNameKana").Value;
				item.ProductPrice = GetWrappedControl<WrappedHiddenField>(ri, "hfProductPrice").Value;
				item.ItemQuantity = GetWrappedControl<WrappedHiddenField>(ri, "hfItemQuantity").Value;
				item.ProductSaleId = GetWrappedControl<WrappedHiddenField>(ri, "hfProductSaleId").Value;
				item.NoveltyId = GetWrappedControl<WrappedHiddenField>(ri, "hfNoveltyId").Value;
				item.RecommendId = GetWrappedControl<WrappedHiddenField>(ri, "hfRecommendId").Value;
				item.ProductBundleId = GetWrappedControl<WrappedHiddenField>(ri, "hfProductBundleId").Value;
				item.BundleItemDisplayType = GetWrappedControl<WrappedHiddenField>(ri, "hfBundleItemDisplayType").Value;
				item.BrandId = GetWrappedControl<WrappedHiddenField>(ri, "hfBrandId").Value;
				item.DownloadUrl = GetWrappedControl<WrappedHiddenField>(ri, "hfDownloadUrl").Value;
				item.CooperationId1 = GetWrappedControl<WrappedHiddenField>(ri, "hfCooperationId1").Value;
				item.CooperationId2 = GetWrappedControl<WrappedHiddenField>(ri, "hfCooperationId2").Value;
				item.CooperationId3 = GetWrappedControl<WrappedHiddenField>(ri, "hfCooperationId3").Value;
				item.CooperationId4 = GetWrappedControl<WrappedHiddenField>(ri, "hfCooperationId4").Value;
				item.CooperationId5 = GetWrappedControl<WrappedHiddenField>(ri, "hfCooperationId5").Value;
				item.CooperationId6 = GetWrappedControl<WrappedHiddenField>(ri, "hfCooperationId6").Value;
				item.CooperationId7 = GetWrappedControl<WrappedHiddenField>(ri, "hfCooperationId7").Value;
				item.CooperationId8 = GetWrappedControl<WrappedHiddenField>(ri, "hfCooperationId8").Value;
				item.CooperationId9 = GetWrappedControl<WrappedHiddenField>(ri, "hfCooperationId9").Value;
				item.CooperationId10 = GetWrappedControl<WrappedHiddenField>(ri, "hfCooperationId10").Value;
				item.ProductOptionValue = GetWrappedControl<WrappedHiddenField>(ri, "hfProductOptionValue").Value;
				item.OrderShippingNo = GetWrappedControl<WrappedHiddenField>(ri, "hfOrderShippingNo").Value;
				item.ShippingName = GetWrappedControl<WrappedHiddenField>(ri, "hfShippingName").Value;
				item.ShippingNameKana = GetWrappedControl<WrappedHiddenField>(ri, "hfShippingNameKana").Value;
				item.ShippingTel1 = GetWrappedControl<WrappedHiddenField>(ri, "hfShippingTel1").Value;
				item.ShippingCountryIsoCode = GetWrappedControl<WrappedHiddenField>(ri, "hfShippingCountryIsoCode").Value;
				item.ShippingCountryName = GetWrappedControl<WrappedHiddenField>(ri, "hfShippingCountryName").Value;
				item.ShippingZip = GetWrappedControl<WrappedHiddenField>(ri, "hfShippingZip").Value;
				item.ShippingAddr1 = GetWrappedControl<WrappedHiddenField>(ri, "hfShippingAddr1").Value;
				item.ShippingAddr2 = GetWrappedControl<WrappedHiddenField>(ri, "hfShippingAddr2").Value;
				item.ShippingAddr3 = GetWrappedControl<WrappedHiddenField>(ri, "hfShippingAddr3").Value;
				item.ShippingAddr4 = GetWrappedControl<WrappedHiddenField>(ri, "hfShippingAddr4").Value;
				item.ShippingAddr5 = GetWrappedControl<WrappedHiddenField>(ri, "hfShippingAddr5").Value;
				item.ShippingCompanyName = GetWrappedControl<WrappedHiddenField>(ri, "hfShippingCompanyName").Value;
				item.ShippingCompanyPostName = GetWrappedControl<WrappedHiddenField>(ri, "hfShippingCompanyPostName").Value;
				item.ProductPricePretax = GetWrappedControl<WrappedHiddenField>(ri, "hfProductPricePretax").Value;
				item.ProductTaxIncludedFlg = GetWrappedControl<WrappedHiddenField>(ri, "hfProductTaxIncludedFlg").Value;
				item.ProductTaxRate = GetWrappedControl<WrappedHiddenField>(ri, "hfProductTaxRate").Value;
				item.ProductTaxRoundType = GetWrappedControl<WrappedHiddenField>(ri, "hfProductTaxRoundType").Value;
				item.OrderSetPromotionNo = GetWrappedControl<WrappedHiddenField>(ri, "hfOrderSetPromotionNo").Value;
				item.FixedPurchaseProductFlg = GetWrappedControl<WrappedHiddenField>(ri, "hfFixedPurchaseProductFlg").Value;
				item.DiscountedPrice = GetWrappedControl<WrappedHiddenField>(ri, "hfDiscountedPrice").Value;
				item.SubscriptionBoxCourseId = GetWrappedControl<WrappedHiddenField>(ri, "hfSubscriptionBoxCourseId").Value;
				item.SubscriptionBoxFixedAmount = GetWrappedControl<WrappedHiddenField>(ri, "hfSubscriptionBoxFixedAmount").Value;

				return item;
			})
			.ToArray();
				}

	/// <summary>
	///	交換商品の入力値取得
	/// </summary>
	/// <param name="repeater">ラップ済みリピーター</param>
	/// <returns>返品・交換商品の入力値</returns>
	private OrderReturnExchangeItemInput[] CreateInputOrderReturnExchangingItems(WrappedRepeater repeater)
		{
		return repeater
			.Items
			.Cast<RepeaterItem>()
			.Select(ri =>
			{
				var item = new OrderReturnExchangeItemInput();
				item.ProductTaxRate = GetWrappedControl<WrappedHiddenField>(ri, "hfProductTaxRate").Value;
				item.ProductSaleId = GetWrappedControl<WrappedTextBox>(ri, "tbProductSaleId").Text;
				item.NoveltyId = GetWrappedControl<WrappedTextBox>(ri, "tbNoveltyId").Text;
				item.RecommendId = GetWrappedControl<WrappedTextBox>(ri, "tbRecommendId").Text;
				item.ProductBundleId = GetWrappedControl<WrappedTextBox>(ri, "tbProductBundleId").Text;
				item.IsFixedPurchaseItem = GetWrappedControl<WrappedCheckBox>(ri, "cbFixedPurchase").Checked;
				item.ProductId = GetWrappedControl<WrappedTextBox>(ri, "tbProductId").Text;
				item.SupplierId = GetWrappedControl<WrappedHiddenField>(ri, "hfSupplierId").Value;
				item.ProductName = GetWrappedControl<WrappedTextBox>(ri, "tbProductName").Text;
				item.OrderSetPromotionNo = GetWrappedControl<WrappedDropDownList>(ri, "ddlOrderSetPromotion").SelectedValue;
				item.ProductPrice = GetWrappedControl<WrappedTextBox>(ri, "tbProductPrice").Text;
				item.ItemQuantity = GetWrappedControl<WrappedTextBox>(ri, "tbItemQuantity").Text;
				item.RecommendId2 = GetWrappedControl<WrappedTextBox>(ri, "tbRecommendId2").Text;
				item.VId = GetWrappedControl<WrappedTextBox>(ri, "tbVId").Text;
				item.ItemIndex = GetWrappedControl<WrappedHiddenField>(ri, "hfItemIndex").Value;
				// OptionSelectValuesの取得
				item.ProductOptionValue = Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED
					? CreateProductOptionSettingList(ri).GetJsonStringFromSelectValues()
					: GetWrappedControl<WrappedTextBox>(ri, "tbProductOptionValue").Text;
				item.SubscriptionBoxCourseId = GetWrappedControl<WrappedHiddenField>(ri, "hfSubscriptionBoxCourseId").Value;
				item.SubscriptionBoxFixedAmount = GetWrappedControl<WrappedHiddenField>(ri, "hfSubscriptionBoxFixedAmount").Value;

				return item;
			})
			.ToArray();
	}

	/// <summary>
	/// 返金メモ入力タイプ選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblRepaymentMemoType_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		// ページロード内で表示制御処理を行う
	}

	/// <summary>
	/// 商品付帯情報作成
	/// </summary>
	/// <param name="riItem">付帯情報選択領域のリピータアイテム</param>
	/// <returns>商品付帯情報設定リスト</returns>
	private ProductOptionSettingList CreateProductOptionSettingList(RepeaterItem riItem)
	{
		var productId = ((HiddenField)riItem.FindControl("hfProductId")).Value;
		var settingList = new ProductOptionSettingList(this.LoginOperatorShopId, productId);

		// 選択値セット
		foreach (var settingItem in settingList.Items)
		{
			var index = settingList.Items.IndexOf(settingItem);
			var riTarget = ((Repeater)riItem.FindControl("rProductOptionValueSettings")).Items[index];
			settingItem.SelectedSettingValue = GetSelectedProductOptionValue(riTarget);
		}
		return settingList;
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
	#endregion

	/// <summary>
	/// Check Paidy payment price change
	/// </summary>
	/// <param name="returnExchangePriceTotal">Return exchange price total</param>
	/// <returns>True if can refund/cancel, otherwise false</returns>
	private bool CheckPaidyPaymentPriceChange(decimal returnExchangePriceTotal)
	{
		var oldLastBilledAmount = decimal.Parse(this.ReturnExchangeOrderOrg.LastBilledAmount);
		if (oldLastBilledAmount >= returnExchangePriceTotal) return true;

		this.lbPaymentAlertMessage.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PAYMENT_AMOUNT_PAIDY_CANNOT_INCREASE);
		return false;
	}

	/// <summary>
	/// Is valid external payment status
	/// </summary>
	/// <returns>True if is valid, otherwise false</returns>
	private bool IsValidExternalPaymentStatus()
	{
		lbPaygentErrorMessages.Text = string.Empty;

		if ((ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
			&& ((this.ReturnExchangeOrderOrg.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_CANCEL_EXPIRED)
				|| (this.ReturnExchangeOrderOrg.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_EXPIRED)
				|| string.IsNullOrEmpty(this.ReturnExchangeOrderOrg.CardTranId)))
		{
			ddlExecuteType.SelectedValue = ReauthCreatorFacade.ExecuteTypes.None.ToString();
			ddlExecuteType.Enabled = false;
			lbPaygentErrorMessages.Text = WebSanitizer.HtmlEncode(WebMessages.GetMessages(
				WebMessages.ERRMSG_MANAGER_PAYMENT_PAIDY_PAYGENT_CANNOT_LINKING_BY_EXTERNAL_PAYMENT_STATUS,
				ValueText.GetValueText(
					Constants.TABLE_ORDER,
					Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS,
					this.ReturnExchangeOrderOrg.ExternalPaymentStatus)));
			lbPaygentErrorMessages.Visible = true;
			return false;
		}
		return true;
	}
	#region プロパティ
	/// <summary>画面入力情報</summary>
	protected OrderReturnExchangeInput Input { get; set; }
	/// <summary>元注文情報</summary>
	protected ReturnOrder OrderOrg
	{
		get { return (ReturnOrder)ViewState["OrderOrg"]; }
		set { ViewState["OrderOrg"] = value; }
	}
	/// <summary>返品商品リスト</summary>
	protected List<ReturnOrderItem> OrderItems;
	/// <summary>注文セットプロモーション選択ドロップダウンリストアイテム</summary>
	protected ListItemCollection OrderSetPromotionList
	{
		get { return GetOrderSetPromotionList(this.ReturnExchangeOrderOrg); }
	}
	/// <summary>
	/// ユーザーが所属しているカード数
	/// </summary>
	protected int CreditCardNum
	{
		get { return (int)ViewState["credit_card_num"]; }
		set { ViewState["credit_card_num"] = value; }
	}
	/// <summary>アイテムテーブル用追加カラム数</summary>
	protected int AddColumnCountForItemTable
	{
		get
		{
			return
				(Constants.PRODUCT_SALE_OPTION_ENABLED ? 1 : 0)
				+ ((Constants.NOVELTY_OPTION_ENABLED || Constants.RECOMMEND_OPTION_ENABLED) ? 1 : 0)
				+ (Constants.FIXEDPURCHASE_OPTION_ENABLED ? 1 : 0)
				+ (Constants.PRODUCTBUNDLE_OPTION_ENABLED ? 1 : 0)
				+ (this.DisplayItemSubscriptionBoxCourseIdArea ? 1 : 0);
		}
	}
	/// <summary>ユーザ情報</summary>
	protected new UserModel User
	{
		get { return (UserModel)ViewState["User"]; }
		set { ViewState["User"] = value; }
	}
	/// <summary>全ての関連する注文の最終与信フラグが正確かどうか</summary>
	protected bool IsAllLastAuthFlgValid
	{
		get { return (bool)ViewState["all_last_auth_valid_flg"]; }
		set { ViewState["all_last_auth_valid_flg"] = value; }
	}
	/// <summary>返品交換対象配送先ID</summary>
	protected string ReturnExchangeShippingNo { get; set; }
	/// <summary>楽天連携以外か</summary>
	protected bool IsNotRakutenAgency
	{
		get { return (Constants.PAYMENT_CARD_KBN != w2.App.Common.Constants.PaymentCard.Rakuten); }
	}
	/// <summary>頒布会カートか</summary>
	protected bool IsSubscriptionBoxValid
	{
		get { return (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED && (string.IsNullOrEmpty(this.SubscriptionBoxCourseId) == false));; }
	}
	/// <summary>PayTgクレジットトークン</summary>
	protected string CreditTokenbyPayTg
	{
		get { return (string)this.Session[PayTgConstants.PARAM_CUSTOMERID]; }
		set { this.Session[PayTgConstants.PARAM_CUSTOMERID] = value; }
	}
	/// <summary>PayTgクレジット会社コード</summary>
	protected string CreditCardCompanyCodebyPayTg
	{
		get { return (string)this.Session[PayTgConstants.PARAM_ACQNAME]; }
		set { this.Session[PayTgConstants.PARAM_ACQNAME] = value; }
	}
	#endregion
}
