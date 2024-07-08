/*
=========================================================================================================
  Module      : 注文返品交換確認ページ処理(OrderReturnExchangeConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.App.Common.Api;
using w2.App.Common.CrossPoint.Point;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Input.Order;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.NPAfterPay;
using w2.App.Common.Order.Payment.Paygent;
using w2.App.Common.Order.Payment.PayPal;
using w2.App.Common.Order.Reauth;
using w2.Domain;
using w2.Domain.Order;
using w2.Domain.ProductStock;
using w2.Domain.ProductStockHistory;
using w2.Domain.TwOrderInvoice;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

public partial class Form_Order_OrderReturnExchangeConfirm : OrderReturnExchangePage
{
	// 変数
	Hashtable m_orderNew = null;
	List<ReturnOrderItem> m_returnOrderItems = null;
	List<OrderPriceByTaxRateModel> m_returnOrderPriceInfoByTaxRate = null;
	List<OrderPriceByTaxRateModel> m_lastBilledPriceInfoByTaxRate = null;
	List<ReturnOrderItem> m_exchangeOrderItems = null;

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// 注文情報をセッションに格納
		Hashtable htParam = htParam = (Hashtable)Session[Constants.SESSION_KEY_PARAM];

		m_orderNew = (Hashtable)htParam[Constants.CONST_ORDER_RETURN_EXCHANGE_DATA];
		m_returnOrderItems = (List<ReturnOrderItem>)m_orderNew[Constants.CONST_ORDER_RETURN_ORDERITEMS];
		m_exchangeOrderItems = (List<ReturnOrderItem>)m_orderNew[Constants.CONST_ORDER_EXCHANGE_ORDERITEMS];
		m_returnOrderPriceInfoByTaxRate = (List<OrderPriceByTaxRateModel>)m_orderNew[Constants.TABLE_ORDERPRICEBYTAXRATE];
		m_lastBilledPriceInfoByTaxRate = (List<OrderPriceByTaxRateModel>)m_orderNew["last_billed_price_by_tax_rate"];

		// 初期表示？
		if (!IsPostBack)
		{
			//------------------------------------------------------
			// 各情報を設定
			//------------------------------------------------------
			// 基本情報
			lOrderId.Text = WebSanitizer.HtmlEncode(this.ReturnExchangeOrderOrg.OrderId);
			lReturnExchangeKbn.Text = WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_RETURN_EXCHANGE_KBN, m_orderNew[Constants.FIELD_ORDER_RETURN_EXCHANGE_KBN]));
			lOrderReturnExchangeReceiptDate.Text = WebSanitizer.HtmlEncode(
				DateTimeUtility.ToStringForManager(
					m_orderNew[Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_RECEIPT_DATE],
					DateTimeUtility.FormatType.ShortDate2Letter));
			lReturnExchangeReasonKbn.Text = WebSanitizer.HtmlEncode(ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_RETURN_EXCHANGE_REASON_KBN, m_orderNew[Constants.FIELD_ORDER_RETURN_EXCHANGE_REASON_KBN]));
			lReturnExchangeReasonMemo.Text = WebSanitizer.HtmlEncodeChangeToBr(m_orderNew[Constants.FIELD_ORDER_RETURN_EXCHANGE_REASON_MEMO]);
			trOrderPaymentKbn.Visible = (decimal.Parse((string)m_orderNew[Constants.FIELD_ORDER_LAST_BILLED_AMOUNT]) != 0)
				&& (this.ReturnExchangeOrderOrg.LastBilledAmount != (string)m_orderNew[Constants.FIELD_ORDER_LAST_BILLED_AMOUNT]);
			lOrderPaymentKbn.Text = WebSanitizer.HtmlEncode(m_orderNew[FIELD_ORDER_ORDER_PAYMENT_KBN_TEXT]);

			// 返金先情報
			var repaymentBank = OrderCommon.CreateRepaymentBankDictionary((string)m_orderNew[Constants.FIELD_ORDER_REPAYMENT_MEMO]);
			if (repaymentBank != null)
			{
				lRepaymentBankCode.Text = WebSanitizer.HtmlEncode(repaymentBank[Constants.CONST_ORDER_REPAYMENT_BANK_CODE]);
				lRepaymentBankName.Text = WebSanitizer.HtmlEncode(repaymentBank[Constants.CONST_ORDER_REPAYMENT_BANK_NAME]);
				lRepaymentBankBranch.Text = WebSanitizer.HtmlEncode(repaymentBank[Constants.CONST_ORDER_REPAYMENT_BANK_BRANCH]);
				lRepaymentBankAccountNo.Text = WebSanitizer.HtmlEncode(repaymentBank[Constants.CONST_ORDER_REPAYMENT_BANK_ACCOUNT_NO]);
				lRepaymentBankAccountName.Text = WebSanitizer.HtmlEncode(repaymentBank[Constants.CONST_ORDER_REPAYMENT_BANK_ACCOUNT_NAME]);

				trRepaymentMemo.Visible = false;
				tbodyRepaymentBank.Visible = true;
			}
			else
			{
				lRepaymentMemo.Text = WebSanitizer.HtmlEncodeChangeToBr(m_orderNew[Constants.FIELD_ORDER_REPAYMENT_MEMO]);
			}

			// 返品商品情報
			this.lOrderItems = (List<ReturnOrderItem>)m_orderNew[Constants.CONST_ORDER_RETURN_ORDERITEMS];

			// 返品配送先情報
			HashSet<string> lShippingNumbers = new HashSet<string>();
			List<ReturnOrderItem> lOrderShippings = new List<ReturnOrderItem>();
			foreach (ReturnOrderItem roi in lOrderItems)
			{
				if (lShippingNumbers.Add(roi.OrderShippingNo))
				{
					lOrderShippings.Add(roi);
				}
			}
			rReturnOrderShipping.DataSource = lOrderShippings;
			rReturnOrderShipping.DataBind();

			// 税率毎調整金額情報
			rPriceRegulation.DataSource = m_returnOrderPriceInfoByTaxRate.OrderBy(priceInfo => priceInfo.KeyTaxRate);
			rPriceRegulation.DataBind();

			// 返品商品合計
			decimal dReturnOrderPriceSubTotal = decimal.Parse((string)m_orderNew[FIELD_ORDER_ORDER_PRICE_SUBTOTAL_RETURN]);
			lbReturnOrderPriceSubTotal.ForeColor = GetPriceDisplayColor(dReturnOrderPriceSubTotal);
			lbReturnOrderPriceSubTotal.Text = dReturnOrderPriceSubTotal.ToPriceString(true);

			// 交換商品情報
			divExchangeOrderItem.Visible = (string)m_orderNew[Constants.FIELD_ORDER_RETURN_EXCHANGE_KBN] == Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE;
			rExchangeOrderItem.DataSource = (List<ReturnOrderItem>)m_orderNew[Constants.CONST_ORDER_EXCHANGE_ORDERITEMS];
			rExchangeOrderItem.DataBind();
			lbExchangeOrderPriceSubTotal.Text = m_orderNew[FIELD_ORDER_ORDER_PRICE_SUBTOTAL_EXCHANGE].ToPriceString(true);

			// 返品交換合計情報
			lbReturnExchangeOrderPriceSubTotal.ForeColor =
				GetPriceDisplayColor(decimal.Parse((string) m_orderNew[Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL]));
			lbReturnExchangeOrderPriceSubTotal.Text = m_orderNew[Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL].ToPriceString(true);
			lbReturnExchangeOrderPriceTax.ForeColor =
				GetPriceDisplayColor(decimal.Parse((string) m_orderNew[Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL_TAX]));
			lbReturnExchangeOrderPriceTax.Text = m_orderNew[Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL_TAX].ToPriceString(true);
			lbReturnExchangeOrderPriceTotal.ForeColor = GetPriceDisplayColor(decimal.Parse((string)m_orderNew[Constants.FIELD_ORDER_ORDER_PRICE_TOTAL]));
			lbReturnExchangeOrderPriceTotal.Text = WebSanitizer.HtmlEncode(m_orderNew[Constants.FIELD_ORDER_ORDER_PRICE_TOTAL].ToPriceString(true));
			lOrderPriceRepayment.Text = WebSanitizer.HtmlEncode(m_orderNew[Constants.FIELD_ORDER_ORDER_PRICE_REPAYMENT].ToPriceString(true));
			lRegulationMemo.Text = WebSanitizer.HtmlEncodeChangeToBr(m_orderNew[Constants.FIELD_ORDER_REGULATION_MEMO]);

			rReturnExchangePriceByTaxRate.DataSource = m_returnOrderPriceInfoByTaxRate.OrderBy(priceInfo => priceInfo.KeyTaxRate);
			rReturnExchangePriceByTaxRate.DataBind();

			// ポイントOP関連
			if (Constants.W2MP_POINT_OPTION_ENABLED)
			{
				// 調整後ポイント（仮ポイント）があり？
				if (((List<List<Hashtable>>)m_orderNew[CONST_ORDER_POINT_ADD_TEMP]).Count != 0)
				{
					rOrderPointAddTemp.DataSource = ((List<List<Hashtable>>)m_orderNew[CONST_ORDER_POINT_ADD_TEMP]);
					rOrderPointAddTemp.DataBind();
				}
				// 調整後ポイント（本ポイント）があり？
				else if (((Hashtable)m_orderNew[CONST_ORDER_BASE_POINT_ADD_COMP] != null) && ((Hashtable)m_orderNew[CONST_ORDER_LIMIT_POINT_ADD_COMP] != null))
				{
					var orderBasePointAddComp = (Hashtable)m_orderNew[CONST_ORDER_BASE_POINT_ADD_COMP];
					lOrderBasePointAddBefore.Text = WebSanitizer.HtmlEncode(StringUtility.ToNumeric(orderBasePointAddComp[Constants.FIELD_USERPOINT_POINT + Constants.FIELD_COMMON_BEFORE]));
					lOrderBasePointAddAdjustment.Text = WebSanitizer.HtmlEncode(StringUtility.ToNumeric(orderBasePointAddComp[CONST_ORDER_POINT_ADD_ADJUSTMENT]));
					lOrderBasePointAdd.Text = WebSanitizer.HtmlEncode(StringUtility.ToNumeric(orderBasePointAddComp[Constants.FIELD_USERPOINT_POINT]));
					trOrderBasePointAddComp.Visible = true;

					var orderLimitPointAddComp = (Hashtable)m_orderNew[CONST_ORDER_LIMIT_POINT_ADD_COMP];
					lOrderLimitPointAddBefore.Text = WebSanitizer.HtmlEncode(StringUtility.ToNumeric(orderLimitPointAddComp[Constants.FIELD_USERPOINT_POINT + Constants.FIELD_COMMON_BEFORE]));
					lOrderLimitPointAddAdjustment.Text = WebSanitizer.HtmlEncode(StringUtility.ToNumeric(orderLimitPointAddComp[CONST_ORDER_POINT_ADD_ADJUSTMENT]));
					lOrderLimitPointAdd.Text = WebSanitizer.HtmlEncode(StringUtility.ToNumeric(orderLimitPointAddComp[Constants.FIELD_USERPOINT_POINT]));
					trOrderLimitPointAddComp.Visible = true;
				}
				else
				{
					tdOrderPointAdd.Visible = false;
				}

				// 利用ポイント
				lLastOrderPointUseBefore.Text = WebSanitizer.HtmlEncode(m_orderNew[CONST_ORDER_ORDER_POINT_USE_BEFORE]);
				lOrderPointUseAdjustment.Text = WebSanitizer.HtmlEncode(m_orderNew[CONST_ORDER_ORDER_POINT_USE_ADJUSTMENT]);
				lLastOrderPointUse.Text = WebSanitizer.HtmlEncode(m_orderNew[Constants.FIELD_ORDER_LAST_ORDER_POINT_USE]);
			}

			// 最終請求金額
			var lastBilledAmount = decimal.Parse((string)m_orderNew[Constants.FIELD_ORDER_LAST_BILLED_AMOUNT]);
			lbReturnExchangeLastBilledAmount.Text = lastBilledAmount.ToPriceString(true);

			// 税率毎最終請求金額
			rLastBilledAmountByTaxRate.DataSource = m_lastBilledPriceInfoByTaxRate;
			rLastBilledAmountByTaxRate.DataBind();

			//決済金額
			var paymentKbn = (string)m_orderNew[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN];
			var settlementCurrency = CurrencyManager.GetSettlementCurrency(paymentKbn);
			var settlementRate = CurrencyManager.GetSettlementRate(settlementCurrency);
			var settlementAmount = CurrencyManager.GetSettlementAmount(OrderIdOrg, paymentKbn, lastBilledAmount, settlementRate, settlementCurrency);
			m_orderNew[Constants.FIELD_ORDER_SETTLEMENT_CURRENCY] = settlementCurrency;
			m_orderNew[Constants.FIELD_ORDER_SETTLEMENT_RATE] = settlementRate.ToString();
			m_orderNew[Constants.FIELD_ORDER_SETTLEMENT_AMOUNT] = settlementAmount.ToString();

			lbReturnExchangeSettlementAmount.Text = CurrencyManager.ToSettlementCurrencyNotation(
				settlementAmount,
				settlementCurrency);

			// 処理区分
			this.ExecuteType = (ReauthCreatorFacade.ExecuteTypes)Enum.Parse(typeof(ReauthCreatorFacade.ExecuteTypes), (string)htParam["execute_type"]);
			// 全返品
			this.IsReturnAllItems = (bool)m_orderNew["is_return_all_items"];
			// 全返品で請求金額が0より大きいの場合警告メッセージを表示する。
			if (this.IsReturnAllItems
				&& (lastBilledAmount > 0)
				&& Constants.PAYMENT_REAUTH_ENABLED
				&& (this.ExecuteType != ReauthCreatorFacade.ExecuteTypes.None)
				&& OrderCommon.CheckCanPaymentReauth((string)m_orderNew[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN]))
			{
				lbAllReturnNotZeroWarning.Text =
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_ALL_RETURN_NOT_ZERO)
						.Replace("@@ 1 @@", 0.ToPriceString(true))
						.Replace("@@ 2 @@", lastBilledAmount.ToPriceString(true));
			}

			// 元注文情報（元注文 or 最後の返品/交換注文）取得
			this.OrderOld = OrderCommon.GetLastAuthOrder(this.OrderIdOrg);

			// 入金済み検証
			CheckOrderHasPaid(this.OrderOld);
		}

		//------------------------------------------------------
		// エラーチェック
		// ※更新ボタンクリックイベント時にもチェックが実行
		//------------------------------------------------------
		// 連続投稿防止
		if (((string)Session[Constants.SESSION_KEY_ORDER_REGIST_INPUT_ERROR]).Length != 0)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_REGIST_INPUT_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		StringBuilder sbErrorMessage = new StringBuilder();
		sbErrorMessage.Append(CheckReturnOrderItem(m_returnOrderItems));
		// 返品交換区分が交換の場合
		if ((string)m_orderNew[Constants.FIELD_ORDER_RETURN_EXCHANGE_KBN] == Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE)
		{
			sbErrorMessage.Append(CheckExchangeOrderItem(m_exchangeOrderItems));
		}
		// エラー画面へ遷移
		if (sbErrorMessage.Length != 0)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = sbErrorMessage.ToString() + WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERREGIST_BACK_TO_REGIST_PAGE);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// ポップアップ表示制御（タイトルを非表示へ）
		trTitleOrderTop.Visible = trTitleOrderMiddle.Visible = trTitleOrderBottom.Visible = (this.IsPopUp == false);
	}

	/// <summary>
	/// 登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnRegist_Click(object sender, EventArgs e)
	{
		// 返品交換注文情報登録・取得
		var orderNew = CreateReturnExchangeOrder(UpdateHistoryAction.DoNotInsert);

		var twOrderInvoice = new TwOrderInvoiceService().GetOrderInvoice(
			orderNew.OrderIdOrg,
			orderNew.Shippings[0].OrderShippingNo);

		var isChangeToProvisionalCreditCard = (this.NeedsRegisterProvisionalCreditCardCardKbnExceptZeus
			&& (orderNew.OrderPaymentKbn == Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID));

		// 再与信可能な場合はカード枝番などセット
		if (Constants.PAYMENT_REAUTH_ENABLED && this.ReturnExchangeOrderOrg.IsPermitReauthOrderSiteKbn)
		{
			int? creditBranchNo = null;

			// クレジットカード登録／更新
			if (((orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT) && (orderNew.CreditBranchNo.HasValue == false))
				|| isChangeToProvisionalCreditCard)
			{
				var creditCardInput = (OrderCreditCardInput)m_orderNew["order_creadit_card"];
				var dispFlg = ((creditCardInput != null)
					&& (string.IsNullOrEmpty(creditCardInput.RegisterCardName) == false)
					&& creditCardInput.RegisterCardName != Constants.CREDITCARD_UNREGIST_DEFAULT_DISPLAY_NAME);
				if (isChangeToProvisionalCreditCard)
				{
					var userCreditCard = new ProvisionalCreditCardProcessor().RegisterUnregisterdCreditCard(
						orderNew.UserId,
						(creditCardInput != null) ? creditCardInput.RegisterCardName : "",
						Constants.FLG_USERCREDITCARD_REGISTER_ACTION_KBN_ORDER_RETURN_EXCHANGE,
						"",
						"",
						this.LoginOperatorName,
						UpdateHistoryAction.DoNotInsert);
					creditBranchNo = userCreditCard.BranchNo;
				}
				else
				{
					var result = RegisterUserCreditCard(
						orderNew.UserId,
						creditCardInput,
						dispFlg,
						UpdateHistoryAction.DoNotInsert);

					creditBranchNo = (result == null) ? 0 : result.BranchNo;

					OrderCommon.AppendExternalPaymentCooperationLog(
						result != null,
						this.OrderOld.OrderId,
						result != null ? LogCreator.CreateMessage(this.OrderOld.OrderId, "") : this.ApiErrorMessage,
						this.LoginOperatorName,
						UpdateHistoryAction.Insert);
				}
			}
			// PayPal
			else if (Constants.PAYPAL_LOGINPAYMENT_ENABLED && (orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL))
			{
				if (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
				{
					creditBranchNo = this.OrderOld.CreditBranchNo;
				}
				else
				{
					var userExtend = new UserService().GetUserExtend(orderNew.UserId);
					var userCreditCard = PayPalUtility.Payment.RegisterAsUserCreditCard(
						orderNew.UserId,
						new PayPalCooperationInfo(userExtend),
						this.LoginOperatorName,
						UpdateHistoryAction.Insert);
					creditBranchNo = userCreditCard.BranchNo;
				}
			}
			// Paidy Pay
			else if (Constants.PAYMENT_PAIDY_OPTION_ENABLED && (orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY))
			{
				if (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
				{
					creditBranchNo = this.OrderOld.CreditBranchNo;
				}
			}

			// 枝番が取得できていたらセット
			if (creditBranchNo.HasValue)
			{
				orderNew.CreditBranchNo = creditBranchNo.Value;
				m_orderNew[Constants.FIELD_ORDER_CREDIT_BRANCH_NO] = creditBranchNo.Value;
			}
		}
		// 外部決済連携実行（仮クレジットカードの場合はスキップ）
		var isExecuteExternalPayment = ExecuteExternalPayment(orderNew);

		// 返品交換注文情報登録
		var transaction = string.Empty;
		var isTwInvoiceEcPay = false;
		var errorMessages = string.Empty;
		var registReturnExchangeOrderErrorMessage = string.Empty;
		using (var accessor = new SqlAccessor())
		{
			// トランザクション開始
			accessor.OpenConnection();
			accessor.BeginTransaction();
			try
			{
				if (isExecuteExternalPayment || (orderNew.OrderPaymentKbn == Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID))
				{
					// 変更後の注文の最終与信フラグをONにする
					m_orderNew[Constants.FIELD_ORDER_LAST_AUTH_FLG] = Constants.FLG_ORDER_LAST_AUTH_FLG_ON;

					// 元注文情報更新
					transaction = "元注文情報（元注文 or 最後の返品・交換注文）UPDATE処理";
					ExecuteUpdateOrderOld(this.OrderOld, UpdateHistoryAction.DoNotInsert, accessor);
				}

				// For case order has NP after pay and payment has paid
				UpdatePaymentMemoForNPAfterPayHasPaid(
					this.OrderOld,
					this.IsNPAfterHasPaid,
					isExecuteExternalPayment,
					accessor);

				// 返品交換注文情報登録
				var orderIdNew = ExecuteRegistReturnExchangeOrder(
					UpdateHistoryAction.DoNotInsert,
					accessor,
					out transaction,
					out registReturnExchangeOrderErrorMessage);
				// 関連注文更新
				transaction = "2-1.関連注文更新処理";
				OrderCommon.UpdateRelatedOrder(this.ReturnExchangeOrderOrg.OrderId, m_orderNew, UpdateHistoryAction.Insert, this.LoginOperatorName, accessor);

				if (Constants.TWINVOICE_ENABLED
					&& (twOrderInvoice != null))
				{
					var suffixString = orderNew.OrderId.Split('-')[1];

					twOrderInvoice.TwInvoiceNo = string.Format("{0}-{1}",
						twOrderInvoice.TwInvoiceNo,
						int.Parse(suffixString));
					twOrderInvoice.TwInvoiceStatus = Constants.FLG_ORDER_INVOICE_STATUS_REFUND;
					twOrderInvoice.OrderId = orderNew.OrderId;
					twOrderInvoice.TwInvoiceDate = DateTime.Now;

					new TwOrderInvoiceService().Insert(twOrderInvoice, accessor);
				}

				// Execute Taiwan order invoice Ec Pay
				if (Constants.TWINVOICE_ECPAY_ENABLED
					&& (twOrderInvoice != null))
				{
					var suffixString = orderNew.OrderId.Split('-')[1];

					twOrderInvoice.TwInvoiceNo = string.Format("{0}-{1}",
						twOrderInvoice.TwInvoiceNo,
						int.Parse(suffixString));
					twOrderInvoice.TwInvoiceStatus = Constants.FLG_ORDER_INVOICE_STATUS_REFUND;
					twOrderInvoice.OrderId = orderNew.OrderId;
					twOrderInvoice.TwInvoiceDate = DateTime.Now;

					ExecuteTwOrderInvoiceEcPay(
						ref twOrderInvoice,
						orderNew,
						accessor,
						ref isTwInvoiceEcPay,
						ref errorMessages);

					new TwOrderInvoiceService().Insert(twOrderInvoice, accessor);
				}

				// Update store pickup status return
				if (string.IsNullOrEmpty(this.OrderOld.Shippings[0].StorePickupRealShopId) == false)
				{
					var storePickupInput = new Hashtable
					{
						{ Constants.FIELD_ORDER_ORDER_ID, orderIdNew },
						{ Constants.FIELD_ORDER_LAST_CHANGED, this.LoginOperatorName },
						{ Constants.FIELD_ORDER_STOREPICKUP_STATUS, Constants.FLG_STOREPICKUP_STATUS_RETURNED },
						{ Constants.FIELD_ORDER_STOREPICKUP_RETURN_DATE,
							this.OrderOld.StorePickupReturnDate.HasValue
								? this.OrderOld.StorePickupReturnDate.Value.ToString()
								: DateTime.Now.ToShortDateString() }
					};
					DomainFacade.Instance.OrderService.UpdateStorePickupStatus(storePickupInput, accessor);
				}

				// 更新履歴登録
				new UpdateHistoryService().InsertForOrder(orderIdNew, this.LoginOperatorName, accessor);
				new UpdateHistoryService().InsertForUser(this.OrderOld.UserId, this.LoginOperatorName, accessor);

				// API処理
				transaction = ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ORDER,
					Constants.VALUETEXT_PARAM_TRANSACTION_NAME,
					Constants.VALUETEXT_PARAM_TRANSACTION_NAME_CROSS_POINT_INTEGRATION);
				if (this.IsLinkedCrossPoint
					&& UserService.IsUser(new UserService().Get(ReturnExchangeOrderOrg.UserId).UserKbn))
				{
					// 関連注文情報から返品交換後の商品一覧を取得
					var orders = new OrderService().GetRelatedOrders(this.OrderIdOrg, accessor);
					var items = orders
						.SelectMany(order => order.Shippings.SelectMany(shipping => shipping.Items))
						.GroupBy(
							item => new
							{
								item.ShopId,
								item.ProductId,
								item.VariationId,
								item.ProductPrice,
							})
						.Select(
							orderItem => new OrderItemModel(orderItem.FirstOrDefault().DataSource)
							{
								ItemPrice = orderItem.Sum(item => item.ItemPrice),
								ItemPriceTax = orderItem.Sum(item => item.ItemPriceTax),
								ItemPriceSingle = orderItem.Sum(item => item.ItemPriceSingle),
								ItemDiscountedPrice = orderItem.Sum(item => item.ItemDiscountedPrice),
								ItemPriceRegulation = orderItem.Sum(item => item.ItemPriceRegulation),
								ItemQuantity = orderItem.Sum(item => item.ItemQuantity),
								ItemQuantitySingle = orderItem.Sum(item => item.ItemQuantitySingle)
							})
						.Where(item => (item.ItemQuantity > 0))
						.ToArray();

					// 全返品
					if (items.Any() == false)
					{
						var registerInput = new PointApiInput
						{
							MemberId = ReturnExchangeOrderOrg.UserId,
							OrderDate = (ReturnExchangeOrderOrg != null)
								? DateTime.Parse(ReturnExchangeOrderOrg.OrderDate)
								: (DateTime?)null,
							PosNo = w2.App.Common.Constants.CROSS_POINT_POS_NO,
							OrderId = ReturnExchangeOrderOrg.OrderId,
						};
						var result = new CrossPointPointApiService().Delete(registerInput.GetParam(PointApiInput.RequestType.Delete));

						DomainFacade.Instance.OrderService.UpdateOrderExtendStatus(
							ReturnExchangeOrderOrg.OrderId,
							Constants.ORDER_EXTEND_STATUS_NO_CROSSPOINT_GRANTED,
							Constants.FLG_ORDER_EXTEND_STATUS_ON,
							DateTime.Now,
							Constants.FLG_LASTCHANGED_SYSTEM,
							UpdateHistoryAction.Insert,
							accessor);

						// 伝票がないエラーのみの場合、登録完了した上でエラー出力
						if (CheckOnlyNoSlipError(result.Error))
						{
							accessor.CommitTransaction();
							transaction = WebMessages.GetMessages(
								WebMessages.ERRMSG_MANAGER_CONTENT_UPDATED_SUCCESS_BUT_CROSS_POINT_LINKAGE_FAILED);

							// 連続投稿防止用
							Session[Constants.SESSION_KEY_ORDER_REGIST_INPUT_ERROR] = string.Format(
								"error : {0}",
								WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_COUNTINUOUS_WIRTE));
						}

						if (result.IsSuccess == false)
						{
							throw new Exception(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_CROSS_POINT_LINKAGE_FAILED));
						}
					}
					// 一部返品・交換
					else
					{
						var newOrder = new OrderModel
						{
							OrderId = ReturnExchangeOrderOrg.OrderId,
							UserId = ReturnExchangeOrderOrg.UserId,
							OrderDate = (ReturnExchangeOrderOrg != null)
								? DateTime.Parse(ReturnExchangeOrderOrg.OrderDate)
								: (DateTime?)null,
							OrderPriceSubtotal = orders.Sum(order => order.OrderPriceSubtotal),
							OrderPriceSubtotalTax = orders.Sum(order => order.OrderPriceSubtotalTax),
							OrderPointAdd = orders.Sum(order => order.OrderPointAdd),
							OrderPointUseYen = orders.Sum(order => order.OrderPointUseYen),
							OrderPriceRegulation = orders.Sum(order => order.OrderPriceRegulation),
							OrderPriceShipping = orders.Sum(order => order.OrderPriceShipping),
							LastOrderPointUse = orders.OrderByDescending(order => order.OrderDate).First().LastOrderPointUse,
							MemberRankDiscountPrice = orders.Sum(order => order.MemberRankDiscountPrice),
							SetpromotionProductDiscountAmount = orders.Sum(order => order.SetpromotionProductDiscountAmount),
							SetpromotionShippingChargeDiscountAmount = orders.Sum(order => order.SetpromotionShippingChargeDiscountAmount),
							SetpromotionPaymentChargeDiscountAmount = orders.Sum(order => order.SetpromotionPaymentChargeDiscountAmount),
							OrderPointUse = orders.Sum(order => order.OrderPointUse),
							OrderCouponUse = orders.Sum(order => order.OrderCouponUse),
							FixedPurchaseMemberDiscountAmount = orders.Sum(order => order.FixedPurchaseMemberDiscountAmount),
							FixedPurchaseDiscountPrice = orders.Sum(order => order.FixedPurchaseDiscountPrice),
							Items = items,
							ReturnExchangeKbn = orderNew.ReturnExchangeKbn,
						};

						// 返品用金額補正の取得
						var returnPrice = new OrderPriceByTaxRateService().GetReturnPriceCorrections(newOrder.OrderId, accessor);
						newOrder.ReturnPriceCorrection = returnPrice.Sum();

						string[] errors;
						var isExcludeForGrant = false;

						// 返品交換する時は、仮ポイントと本ポイント状態の変更はしない。
						// CROSSPOINT側ポイントが確定済みの場合はポイント調整後確定する。
						var withGrant = (((this.OrderOld.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP)
								|| (this.OrderOld.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_DELIVERY_COMP))
							&& (this.OrderOld.OrderShippedDate.Value.AddDays(Constants.ORDER_POINT_BATCH_POINT_TEMP_TO_COMP_DAYS) <= DateTime.Now));

						var result = UpdatePointByApi(newOrder, out errors, out isExcludeForGrant, withGrant);

						// 伝票がないエラーのみの場合、登録完了した上でエラー出力
						if (CheckOnlyNoSlipError(errors))
						{
							accessor.CommitTransaction();
							transaction = WebMessages.GetMessages(
								WebMessages.ERRMSG_MANAGER_CONTENT_UPDATED_SUCCESS_BUT_CROSS_POINT_LINKAGE_FAILED);

							// 連続投稿防止用
							Session[Constants.SESSION_KEY_ORDER_REGIST_INPUT_ERROR] = string.Format(
								"error : {0}",
								WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_COUNTINUOUS_WIRTE));
						}

						if (result == false)
						{
							throw new Exception(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_CROSS_POINT_LINKAGE_FAILED));
						}
					}
				}
				accessor.CommitTransaction();
			}
			catch (Exception ex)
			{
				accessor.RollbackTransaction();

				if (isTwInvoiceEcPay == false)
				{
					// ログの記録をしておく
					errorMessages =
						transaction + "に失敗しました。"
						+ (isExecuteExternalPayment ? "既に外部連携実行済みです。" : string.Empty)
						+ registReturnExchangeOrderErrorMessage;
					AppLogger.WriteError(errorMessages, ex);
				}

				// エラー画面へ
				Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages;
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			// 連続投稿防止用
			Session[Constants.SESSION_KEY_ORDER_REGIST_INPUT_ERROR] = string.Format(
				"error : {0}",
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_COUNTINUOUS_WIRTE)); // これが残ってる時はエラー
		}

		// 登録完了したので受注詳細画面へ
		Response.Redirect(
			CreateOrderDetailUrl(
				(string)m_orderNew[Constants.FIELD_ORDER_ORDER_ID],
				false,
				(Request[Constants.REQUEST_KEY_WINDOW_KBN] == Constants.KBN_WINDOW_POPUP)));
	}

	/// <summary>
	/// 返品交換注文情報作成
	/// </summary>
	/// <remarks>実際にはDB更新は行わない</remarks>
	/// <returns>返品交換注文情報</returns>
	private OrderModel CreateReturnExchangeOrder(UpdateHistoryAction updateHistoryAction)
	{
		OrderModel result = null;
		var transaction = "";
		var registReturnExchangeOrderErrorMessage = string.Empty;
		// 以下の実施
		// DB更新が正しく行えるかチェック
		// 注文情報取得（変更後）
		using (var accessor = new SqlAccessor())
		{
			// トランザクション開始
			accessor.OpenConnection();
			accessor.BeginTransaction();

			try
			{
				var orderId = ExecuteRegistReturnExchangeOrder(
					updateHistoryAction,
					accessor,
					out transaction,
					out registReturnExchangeOrderErrorMessage);
				transaction = "2-1.関連注文更新処理";
				OrderCommon.UpdateRelatedOrder(this.ReturnExchangeOrderOrg.OrderId, m_orderNew, updateHistoryAction, this.LoginOperatorName, accessor);
				result = new OrderService().Get(orderId, accessor);
			}
			catch (Exception ex)
			{
				// ログの記録をしておく
				AppLogger.WriteError(ex);

				// エラー画面へ
				Session[Constants.SESSION_KEY_ERROR_MSG] =
					transaction + "に失敗しました。"
					+ registReturnExchangeOrderErrorMessage;
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}
			finally
			{
				// トランザクションロールバック
				accessor.RollbackTransaction();
			}
		}

		return result;

	}

	/// <summary>
	/// 返品交換注文登録
	/// </summary>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <param name="transaction">トランザクション名</param>
	/// <param name="errorMessage">out エラーメッセージ</param>
	/// <returns>注文ID</returns>
	private string ExecuteRegistReturnExchangeOrder(UpdateHistoryAction updateHistoryAction, SqlAccessor accessor, out string transaction, out string errorMessage)
	{
		errorMessage = string.Empty;
		// 最大注文ID(枝番付き)取得＆注文ID(上書き)
		var orderId = GetNewOrderReturnId(this.ReturnExchangeOrderOrg.OrderId, accessor);
		m_orderNew[Constants.FIELD_ORDER_ORDER_ID] = orderId;
		m_orderNew[Constants.FIELD_ORDER_FIXED_PURCHASE_ID] = this.ReturnExchangeOrderOrg.FixedPurchaseId;
		m_orderNew[Constants.FIELD_ORDER_SUBSCRIPTION_BOX_COURSE_ID] = ReturnExchangeOrderOrg.SubscriptionBoxCourseId;
		m_orderNew[Constants.FIELD_ORDER_SUBSCRIPTION_BOX_FIXED_AMOUNT] =
			ReturnExchangeOrderOrg.SubscriptionBoxFixedAmount;
		m_orderNew[Constants.FIELD_ORDER_ORDER_SUBSCRIPTION_BOX_ORDER_COUNT] =
			ReturnExchangeOrderOrg.OrderSubscriptionBoxOrderCount;

		// Get order input
		var orderExtend = new OrderExtend(
			this.RegisterCardTranId,
			this.RegisterPaymentOrderId,
			this.RegisterExternalPaymentStatus,
			this.RegisterExternalPaymentAuthDate,
			this.RegisterOnlinePaymentStatus,
			this.RegisterPaymentMemo);
		orderExtend.GetOrderInputForReturnExchange(m_orderNew, this.ReturnExchangeOrderOrg, this.OrderOld, false, this.LoginOperatorName, accessor);

		// 注文情報登録
		transaction = "1-1-1.注文情報INSERT処理";
		OrderCommon.OrderRegisterForReturnExchange(m_orderNew, this.ReturnExchangeOrderOrg, UpdateHistoryAction.DoNotInsert, this.LoginOperatorName,accessor);

		// 注文者情報登録
		transaction = "1-1-2.注文者情報INSERT処理";
		OrderCommon.RegistOrderOwnerForReturnExchange(m_orderNew, UpdateHistoryAction.DoNotInsert, this.LoginOperatorName, accessor);

		// 注文配送先&商品情報登録
		ExecuteRegistOrderShippingAndItem(UpdateHistoryAction.DoNotInsert, accessor, out transaction);

		// セットプロモーション情報登録
		transaction = "1-1-6.注文セットプロモーション情報INSERT処理";
		OrderCommon.RegistOrderSetPromotionForReturnExchange(m_orderNew, this.ReturnExchangeOrderOrg, m_returnOrderItems, m_exchangeOrderItems, UpdateHistoryAction.DoNotInsert, this.LoginOperatorName, accessor);

		// 税率毎価格情報登録
		ExecuteRegistOrderPriceInfoByTaxRate(accessor, out transaction);

		// 元注文情報の出荷後変更区分を「出荷後変更有り」に変更
		transaction = "1-2.元注文情報のUPDATE処理";
		OrderCommon.UpdateShippedChangedKbnForReturnExchange(this.ReturnExchangeOrderOrg.OrderId, UpdateHistoryAction.DoNotInsert, this.LoginOperatorName, accessor);

		// クーポン関連更新
		var updateCoupon = OrderCommon.UpdateCouponForReturnExchange(m_orderNew, this.ReturnExchangeOrderOrg, this.LoginOperatorName, accessor);
		transaction = updateCoupon;
		if (string.IsNullOrEmpty(updateCoupon) == false) throw new Exception(updateCoupon);

		// ユーザポイント関連更新
		var updatePoint = OrderCommon.UpdatePointForReturnExchange(
			m_orderNew,
			this.ReturnExchangeOrderOrg.OrderId,
			this.ReturnExchangeOrderOrg.UserId,
			UpdateHistoryAction.DoNotInsert,
			this.LoginOperatorName,
			accessor,
			out errorMessage);
		transaction = updatePoint;
		if (string.IsNullOrEmpty(updatePoint) == false) throw new Exception(updatePoint);

		// For case order has payment paidy
		if ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
			&& ((string)m_orderNew[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN] == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
			&& m_orderNew.ContainsKey(Constants.FIELD_ORDER_PAYMENT_ORDER_ID + "_for_paidypay")
			&& (string.IsNullOrEmpty((string)m_orderNew[Constants.FIELD_ORDER_PAYMENT_ORDER_ID + "_for_paidypay"]) == false))
		{
			var orderIdOrg = string.IsNullOrEmpty(this.OrderOld.OrderIdOrg)
				? this.OrderOld.OrderId
				: this.OrderOld.OrderIdOrg;
			new OrderService().UpdateRelatedPaymentOrderId(
				orderIdOrg,
				orderId,
				(string)m_orderNew[Constants.FIELD_ORDER_PAYMENT_ORDER_ID + "_for_paidypay"],
				this.LoginOperatorName,
				UpdateHistoryAction.Insert,
				accessor);
		}

		// 更新履歴登録
		if (updateHistoryAction == UpdateHistoryAction.Insert)
		{
			new UpdateHistoryService().InsertForOrder(orderId, this.LoginOperatorName, accessor);
			new UpdateHistoryService().InsertForUser(this.OrderOld.UserId, this.LoginOperatorName, accessor);
		}
		return orderId;
	}

	/// <summary>
	/// 元注文情報（元注文 or 最後の返品注文）更新
	/// </summary>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <param name="transaction">トランザクション名</param>
	private void ExecuteUpdateOrderOld(UpdateHistoryAction updateHistoryAction, SqlAccessor accessor, out string transaction)
	{
		transaction = "1-4.元注文情報（元注文 or 最後の返品注文）UPDATE処理";

		// 決済メモ更新
		var orderService = new OrderService();
		if (string.IsNullOrEmpty(this.UpdatePaymentMemoForOrderOld) == false)
		{
			orderService.AddPaymentMemo(
				this.OrderOld.OrderId,
				this.UpdatePaymentMemoForOrderOld,
				this.LoginOperatorName,
				UpdateHistoryAction.DoNotInsert,
				accessor);
		}

		// 外部決済ステータス更新
		if (string.IsNullOrEmpty(this.UpdateExternalPaymentStatusForOrderOld) == false)
		{
			orderService.UpdateExternalPaymentInfo(
				this.OrderOld.OrderId,
				this.UpdateExternalPaymentStatusForOrderOld,
				true,
				null,
				"",
				this.LoginOperatorName,
				UpdateHistoryAction.DoNotInsert,
				accessor);
		}

		// 更新履歴登録
		if (updateHistoryAction == UpdateHistoryAction.Insert)
		{
			new UpdateHistoryService().InsertForOrder(this.OrderOld.OrderId, this.LoginOperatorName, accessor);
		}
	}

	/// <summary>
	/// 外部決済連携実行
	/// </summary>
	/// <param name="orderNew">返品交換注文情報</param>
	/// <returns>外部決済連携実行したか？</returns>
	private bool ExecuteExternalPayment(OrderModel orderNew)
	{
		if (Constants.PAYMENT_REAUTH_ENABLED)
		{
			this.RegisterCardTranId = null;
			this.RegisterExternalPaymentAuthDate = null;
			this.RegisterExternalPaymentStatus = null;
			this.RegisterPaymentMemo = null;
			this.UpdatePaymentMemoForOrderOld = null;
			this.UpdateExternalPaymentStatusForOrderOld = null;

			// 外部決済連携実行
			var actionType = (orderNew.IsReturnOrder) ? ReauthCreatorFacade.OrderActionTypes.Return : ReauthCreatorFacade.OrderActionTypes.Exchange;
			var reauth = new ReauthCreatorFacade(this.OrderOld, orderNew, this.ExecuteType, actionType).CreateReauth();
			if (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
			{
				NPAfterPayUtility.IsReturnOrExchange = true;
			}
			var reauthResult = reauth.Execute();

			var isReauthResultSuccess = reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success;

			// 外部決済連携実行ログ処理
			string externalApiLog;

			if (isReauthResultSuccess == false)
			{
				externalApiLog = reauthResult.ApiErrorMessages;
			}
			else if (PaygentUtility.CheckIsPaidyPaygentPayment(this.OrderOld.OrderPaymentKbn))
			{
				string cancelMessage = this.OrderOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP
					? PaygentConstants.PAYGENT_PAIDY_REFUND_LOG_MESSAGE
					: PaygentConstants.PAYGENT_PAIDY_CANCEL_LOG_MESSAGE;

				externalApiLog = LogCreator.CreateMessageWithPaymentId(
					this.OrderOld.CardTranId,
					this.OrderOld.PaymentOrderId,
					this.OrderOld.LastBilledAmount.ToPriceString(),
					cancelMessage);
			}
			else
			{
				externalApiLog = LogCreator.CreateMessage(orderNew.OrderIdOrg, string.Empty);
			}

			OrderCommon.AppendExternalPaymentCooperationLog(
				isReauthResultSuccess,
				orderNew.OrderIdOrg,
				externalApiLog,
				this.LoginOperatorName,
				UpdateHistoryAction.Insert);

			var isNewebPayButFailure = ((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
				&& (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Failure));
			if (isNewebPayButFailure)
			{
				using (var accessor = new SqlAccessor())
				{
					// トランザクション開始
					accessor.OpenConnection();
					accessor.BeginTransaction();

					// 外部決済情報セット
					SetExternalPaymentInfoReauthSplit(reauth, reauthResult, orderNew, this.OrderOld, this.IsReturnAllItems);

					// 元注文情報更新
					ExecuteUpdateOrderOld(this.OrderOld, UpdateHistoryAction.DoNotInsert, accessor);

					// トランザクションコミット
					accessor.CommitTransaction();
				}
				Session[Constants.SESSION_KEY_ERROR_MSG] = StringUtility.ChangeToBrTag(reauthResult.ErrorMessages);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			if (PaygentUtility.CheckIsPaidyPaygentPayment(orderNew.OrderPaymentKbn))
			{
				SetExternalPaymentInfoReauthSplit(reauth, reauthResult, orderNew, this.OrderOld, this.IsReturnAllItems);
			}

			// 与信のみに失敗している場合エラー画面へ
			var isEcPayOrNewebPayAndFailureButAuthSuccess = (((this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
					|| (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
					|| (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
					|| (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY))
				&& (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSuccess));
			if (isEcPayOrNewebPayAndFailureButAuthSuccess
				|| (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Failure))
			{
				// Atodeneの場合、与信エラーにする
				if (reauth.AuthLostForError)
				{
					var service = new OrderService();
					service.UpdateExternalPaymentInfoForAuthError(
						this.OrderOld.OrderId,
						reauthResult.ErrorMessages,
						this.OrderOld.LastChanged,
						UpdateHistoryAction.Insert);
				}
				Session[Constants.SESSION_KEY_ERROR_MSG] = StringUtility.ChangeToBrTag(reauthResult.ErrorMessages);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}
			// 失敗したが、同額で与信を取り直している場合は元注文情報を更新し、エラー画面へ
			else if (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSameAmount)
			{
				using (var accessor = new SqlAccessor())
				{
					// トランザクション開始
					accessor.OpenConnection();
					accessor.BeginTransaction();

					// 外部決済情報セット
					SetExternalPaymentInfoReauthSplit(reauth, reauthResult, orderNew, this.OrderOld, this.IsReturnAllItems);

					// 元注文情報更新
					ExecuteUpdateOrderOld(this.OrderOld, UpdateHistoryAction.DoNotInsert, accessor);

					// 決済取引ID更新
					new OrderService().UpdateCardTranId(this.OrderOld.OrderId, reauthResult.CardTranId, this.LoginOperatorName, UpdateHistoryAction.DoNotInsert, accessor);

					// 更新履歴登録
					new UpdateHistoryService().InsertForOrder(this.OrderOld.OrderId, this.LoginOperatorName, accessor);

					// トランザクションコミット
					accessor.CommitTransaction();
				}
				Session[Constants.SESSION_KEY_ERROR_MSG] = StringUtility.ChangeToBrTag(reauthResult.ErrorMessages);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			// For case order has payment paidy
			if ((orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
				&& (this.OrderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY))
			{
				m_orderNew[Constants.FIELD_ORDER_PAYMENT_ORDER_ID + "_for_paidypay"] = reauthResult.PaymentOrderId;
			}

			SetExternalPaymentSession(reauthResult);
			// 外部決済連携情報セット
			return SetExternalPaymentInfoReauthSplit(reauth, reauthResult, orderNew, this.OrderOld, this.IsReturnAllItems);
		}
		return false;
	}

	/// <summary>
	/// 注文配送先＆商品情報登録
	/// </summary>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <param name="transaction">トランザクション名</param>
	private void ExecuteRegistOrderShippingAndItem(UpdateHistoryAction updateHistoryAction, SqlAccessor accessor, out string transaction)
	{
		transaction = string.Empty;
		int orderItemNo = 1;
		var orderShippingNumberForExchange = string.Empty;
		var orderShippingNumbers = new HashSet<int>(); // ギフト用（配送番号）
		var orderSetPromotionItemNoList = new Dictionary<string, int>(); // セットプロモーションアイテム枝番採番用
		this.ReturnExchangeOrderOrg.SetPromotions.ForEach(sp => orderSetPromotionItemNoList.Add(sp.OrderSetPromotionNo, 1));
		foreach (var roii in m_returnOrderItems)
		{
			if (orderShippingNumbers.Add(int.Parse(roii.OrderShippingNo)))
			{
				transaction = "1-1-3.注文配送先情報INSERT処理";
				OrderCommon.RegistOrderShippingForReturnExchange(m_orderNew, this.OrderOld, roii.OrderShippingNo, accessor);
			}

			// 注文商品情報登録
			transaction = "1-1-4.返品注文商品情報INSERT処理";
			OrderCommon.RegistReturnOrderItemForReturnExchange(
				m_orderNew,
				orderItemNo,
				roii.OrderShippingNo,
				orderSetPromotionItemNoList,
				roii,
				this.LoginOperatorName,
				accessor);
			orderItemNo++;
			orderShippingNumberForExchange = roii.OrderShippingNo;  // 交換時の配送情報No.は必ず1つ
		}

		// 返品交換区分が交換の場合
		if ((string)m_orderNew[Constants.FIELD_ORDER_RETURN_EXCHANGE_KBN] == Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE)
		{
			transaction = "1-1-5.交換注文商品情報INSERT処理＆論理在庫引当";
			foreach (var roii in m_exchangeOrderItems)
			{
				OrderCommon.RegistReturnOrderItemForReturnExchange(
					m_orderNew,
					orderItemNo,
					orderShippingNumberForExchange,
					orderSetPromotionItemNoList,
					roii,
					this.LoginOperatorName,
					accessor);
				orderItemNo++;

				if (Constants.ORDERMANAGEMENT_STOCKCOOPERATION_ENABLED)
				{
					// 論理在庫引当（在庫管理方法が「0.在庫管理をしない 」以外の場合）
					// 商品情報取得
					var productVariation = GetProductVariation(roii.ShopId, roii.ProductId, roii.VariationId);
					if (productVariation.Count == 0)
					{
						Session[Constants.SESSION_KEY_ERROR_MSG] = "返品交換対象の商品情報が取得できませんでした。";
						Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
					}
					if ((string)productVariation[0][Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN] != Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED)
					{
						new ProductStockService().AddProductStock(
							this.LoginOperatorShopId,
							roii.ProductId,
							roii.VariationId,
							-roii.ItemQuantity,
							0,
							0,
							0,
							0,
							this.LoginOperatorName,
							accessor);

						// 商品在庫履歴インサート
						var productStockHistoryModel = new ProductStockHistoryModel
						{
							OrderId = (string)m_orderNew[Constants.FIELD_ORDER_ORDER_ID],
							ShopId = this.LoginOperatorShopId,
							ProductId = roii.ProductId,
							VariationId = roii.VariationId,
							ActionStatus = Constants.FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_ORDER,
							AddStock = -roii.ItemQuantity,
							AddRealstock = 0,
							AddRealstockB = 0,
							AddRealstockC = 0,
							AddRealstockReserved = 0,
							UpdateStock = null,
							UpdateRealstock = null,
							UpdateRealstockB = null,
							UpdateRealstockC = null,
							UpdateRealstockReserved = null,
							UpdateMemo = string.Empty,
							LastChanged = this.LoginOperatorName
						};
						new ProductStockHistoryService().Insert(productStockHistoryModel, accessor);
					}
				}
			}
		}

		// 更新履歴登録
		if (updateHistoryAction == UpdateHistoryAction.Insert)
		{
			new UpdateHistoryService().InsertForOrder((string)m_orderNew[Constants.FIELD_ORDER_ORDER_ID], this.LoginOperatorName, accessor);
		}
	}

	/// <summary>
	/// 税率毎価格情報登録
	/// </summary>
	/// <param name="accessor">SQLアクセサ</param>
	/// <param name="transaction">トランザクション名</param>
	private void ExecuteRegistOrderPriceInfoByTaxRate(SqlAccessor accessor, out string transaction)
	{
		transaction = "1-1-7.税率毎価格情報INSERT処理";
		var orderPriceByTaxRateService = new OrderPriceByTaxRateService();
		foreach (var info in m_returnOrderPriceInfoByTaxRate)
		{
			info.OrderId = (string)m_orderNew[Constants.FIELD_ORDER_ORDER_ID];
			info.DateCreated = DateTime.Now;
			info.DateChanged = DateTime.Now;
			orderPriceByTaxRateService.Insert(info, accessor);
		}
	}

	/// <summary>
	/// 注文の入金済み検証
	/// </summary>
	/// <param name="order">注文情報</param>
	private void CheckOrderHasPaid(OrderModel order)
	{
		switch (order.OrderPaymentKbn)
		{
			case Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY:
				CheckNPAfterPayHasPaid(order);
				break;

			case Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF:
				CheckCvsDefPayHasPaid();
				break;
		}
	}

	/// <summary>
	/// Check NP After Pay Has Paid
	/// </summary>
	/// <param name="order">Order</param>
	private void CheckNPAfterPayHasPaid(OrderModel order)
	{
		if (NPAfterPayUtility.CheckIfExternalPaymentStatusHasBeenPaid(order.ExternalPaymentStatus) == false) return;

		if (NPAfterPayUtility.IsNPAfterPayHasPaid(order.CardTranId))
		{
			lbMessagePayment.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NPAFTERPAY_CANNOT_LINK_PAYMENT);
			this.IsNPAfterHasPaid = true;
		}
	}

	/// <summary>
	/// コンビニ後払いの注文が注文済み検証(現状はスコア後払いのみ)
	/// </summary>
	private void CheckCvsDefPayHasPaid()
	{
		if (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Score)
		{
			lbMessagePayment.Text = WebSanitizer.HtmlEncode(
				WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_SCORE_PAYMENT_CANCEL_ALERT));
		}
	}

	/// <summary>
	/// Update Payment Memo For NP After Pay Has Paid
	/// </summary>
	/// <param name="order">Order</param>
	/// <param name="isNPAfterHasPaid">Is NP After Has Paid</param>
	/// <param name="isExecuteExternalPayment">Is Execute External Payment</param>
	/// <param name="accessor">SQL Accessor</param>
	private void UpdatePaymentMemoForNPAfterPayHasPaid(
		OrderModel order,
		bool isNPAfterHasPaid,
		bool isExecuteExternalPayment,
		SqlAccessor accessor)
	{
		if ((isExecuteExternalPayment)
			|| (order.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
			|| (isNPAfterHasPaid == false)) return;

		var paymentMemo = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NPAFTERPAY_CANNOT_LINK_PAYMENT);
		new OrderService().AddPaymentMemo(
			order.OrderId,
			paymentMemo,
			this.LoginOperatorName,
			UpdateHistoryAction.DoNotInsert,
			accessor);
	}

	/// <summary>
	/// Execute Taiwan order invoice Ec Pay
	/// </summary>
	/// <param name="orderInvoiceNew">New order invoice</param>
	/// <param name="orderNew">New order</param>
	/// <param name="accessor">Sql accessor</param>
	/// <param name="isTwInvoiceEcPay">Is Taiwan invoice Ec Pay</param>
	/// <param name="errorMessage">Error message</param>
	private void ExecuteTwOrderInvoiceEcPay(
		ref TwOrderInvoiceModel orderInvoiceNew,
		OrderModel orderNew,
		SqlAccessor accessor,
		ref bool isTwInvoiceEcPay,
		ref string errorMessage)
	{
		if (orderNew.IsNotReturnExchangeOrder) return;

		try
		{
			isTwInvoiceEcPay = true;
			var invoiceEcPayApi = new TwInvoiceEcPayApi();
			var invoiceService = new TwOrderInvoiceService();

			// Get Taiwan old order invoice information
			var orderInvoiceOld = invoiceService.GetOrderInvoice(
				this.OrderOld.OrderId,
				this.OrderOld.Shippings.FirstOrDefault().OrderShippingNo);

			// Execute return Tw order invoice
			if (orderNew.IsReturnOrder)
			{
				var isReturnAllItems = (bool)m_orderNew["is_return_all_items"];
				if (isReturnAllItems
					&& (orderNew.LastBilledAmount == 0))
				{
					var beginDate = orderInvoiceOld.TwInvoiceDate ?? DateTime.Now;
					if (invoiceEcPayApi.IsSamePeriod(beginDate, DateTime.Now))
					{
						// Execute invalid api
						var request = invoiceEcPayApi.CreateRequestReturnObject(
							TwInvoiceEcPayApi.ExecuteTypes.Invalid,
							orderNew,
							orderInvoiceOld,
							accessor);

						var response = invoiceEcPayApi.ReceiveResponseObject(
							TwInvoiceEcPayApi.ExecuteTypes.Invalid,
							request);

						// Display error messages
						if (response.IsSuccess == false)
						{
							errorMessage = response.Message;
							throw new Exception(errorMessage);
						}

						// Update Taiwan old order invoice status
						invoiceService.UpdateTwOrderInvoiceStatus(
							this.OrderOld.OrderId,
							this.OrderOld.Shippings.FirstOrDefault().OrderShippingNo,
							Constants.FLG_ORDER_INVOICE_STATUS_CANCEL,
							orderInvoiceOld.TwInvoiceNo,
							accessor);

						// Update Taiwan new order invoice for modify
						orderInvoiceNew.TwInvoiceStatus = Constants.FLG_ORDER_INVOICE_STATUS_CANCEL;
						orderInvoiceNew.TwInvoiceNo = orderInvoiceOld.TwInvoiceNo;
					}
					else
					{
						// Execute allowance api
						var request = invoiceEcPayApi.CreateRequestReturnObject(
							TwInvoiceEcPayApi.ExecuteTypes.Allowance,
							orderNew,
							orderInvoiceOld,
							accessor);

						var response = invoiceEcPayApi.ReceiveResponseObject(
							TwInvoiceEcPayApi.ExecuteTypes.Allowance,
							request);

						// Display error messages
						if (response.IsSuccess == false)
						{
							errorMessage = response.Message;
							throw new Exception(errorMessage);
						}

						// Update Taiwan old order invoice status
						invoiceService.UpdateTwOrderInvoiceStatus(
							this.OrderOld.OrderId,
							this.OrderOld.Shippings.FirstOrDefault().OrderShippingNo,
							Constants.FLG_ORDER_INVOICE_STATUS_REFUND_COMPLETED,
							orderInvoiceOld.TwInvoiceNo,
							accessor);

						// Update Taiwan new order invoice status
						orderInvoiceNew.TwInvoiceStatus = Constants.FLG_ORDER_INVOICE_STATUS_REFUND_COMPLETED;
						orderInvoiceNew.TwInvoiceNo = response.Response.Data.IAAllowNo;
					}
				}
				else
				{
					// Execute allowance api
					var request = invoiceEcPayApi.CreateRequestReturnObject(
						TwInvoiceEcPayApi.ExecuteTypes.Allowance,
						orderNew,
						orderInvoiceOld,
						accessor);
					var response = invoiceEcPayApi.ReceiveResponseObject(
						TwInvoiceEcPayApi.ExecuteTypes.Allowance,
						request);

					// Display error messages
					if (response.IsSuccess == false)
					{
						errorMessage = response.Message;
						throw new Exception(errorMessage);
					}

					// Update Taiwan old order invoice status
					invoiceService.UpdateTwOrderInvoiceStatus(
						this.OrderOld.OrderId,
						this.OrderOld.Shippings.FirstOrDefault().OrderShippingNo,
						Constants.FLG_ORDER_INVOICE_STATUS_REFUND_COMPLETED,
						orderInvoiceOld.TwInvoiceNo,
						accessor);

					// Update Taiwan new order invoice status
					orderInvoiceNew.TwInvoiceStatus = Constants.FLG_ORDER_INVOICE_STATUS_REFUND_COMPLETED;
					orderInvoiceNew.TwInvoiceNo = response.Response.Data.IAAllowNo;
				}
			}

			// Execute exchange Tw order invoice
			if (orderNew.IsExchangeOrder)
			{
				var beginDate = orderInvoiceOld.TwInvoiceDate ?? DateTime.Now;
				if (invoiceEcPayApi.IsSamePeriod(beginDate, DateTime.Now))
				{
					// Execute invalid api
					var requestInvalidApi = invoiceEcPayApi.CreateRequestExchangeObject(
						TwInvoiceEcPayApi.ExecuteTypes.Invalid,
						this.OrderOld,
						orderInvoiceOld,
						accessor);
					var responseInvalidApi = invoiceEcPayApi.ReceiveResponseObject(
						TwInvoiceEcPayApi.ExecuteTypes.Invalid,
						requestInvalidApi);

					// Display error messages
					if (responseInvalidApi.IsSuccess == false)
					{
						errorMessage = responseInvalidApi.Message;
						throw new Exception(errorMessage);
					}

					// Execute issue api
					var requestIssueApi = invoiceEcPayApi.CreateRequestObject(
						TwInvoiceEcPayApi.ExecuteTypes.Issue,
						orderNew,
						orderInvoiceOld,
						accessor);
					var responseIssueApi = invoiceEcPayApi.ReceiveResponseObject(
						TwInvoiceEcPayApi.ExecuteTypes.Issue,
						requestIssueApi);

					if (responseIssueApi.IsSuccess == false)
					{
						// Update Taiwan old order invoice status
						invoiceService.UpdateTwOrderInvoiceStatus(
							this.OrderOld.OrderId,
							this.OrderOld.Shippings.FirstOrDefault().OrderShippingNo,
							Constants.FLG_ORDER_INVOICE_STATUS_CANCEL,
							orderInvoiceOld.TwInvoiceNo,
							accessor);

						// Update Taiwan new order invoice for modify
						orderInvoiceNew.TwInvoiceStatus = Constants.FLG_ORDER_INVOICE_STATUS_NOT_ISSUED;
						orderInvoiceNew.TwInvoiceNo = responseIssueApi.Response.Data.InvoiceNo;
						orderInvoiceNew.TwInvoiceDate = DateTime.Parse(responseIssueApi.Response.Data.InvoiceDate);
						errorMessage = responseIssueApi.Message;
						return;
					}

					// Update Taiwan old order invoice status
					invoiceService.UpdateTwOrderInvoiceStatus(
						this.OrderOld.OrderId,
						this.OrderOld.Shippings.FirstOrDefault().OrderShippingNo,
						Constants.FLG_ORDER_INVOICE_STATUS_CANCEL,
						orderInvoiceOld.TwInvoiceNo,
						accessor);

					// Update Taiwan new order invoice for modify
					orderInvoiceNew.TwInvoiceStatus = Constants.FLG_ORDER_INVOICE_STATUS_ISSUED_LINKED;
					orderInvoiceNew.TwInvoiceNo = responseIssueApi.Response.Data.InvoiceNo;
					orderInvoiceNew.TwInvoiceDate = DateTime.Parse(responseIssueApi.Response.Data.InvoiceDate);
				}
				else
				{
					// Execute allowance api
					var requestAllowanceApi = invoiceEcPayApi.CreateRequestExchangeObject(
						TwInvoiceEcPayApi.ExecuteTypes.Allowance,
						this.OrderOld,
						orderInvoiceOld,
						accessor);
					var responseAllowanceApi = invoiceEcPayApi.ReceiveResponseObject(
						TwInvoiceEcPayApi.ExecuteTypes.Allowance,
						requestAllowanceApi);

					// Display error messages
					if (responseAllowanceApi.IsSuccess == false)
					{
						errorMessage = responseAllowanceApi.Message;
						throw new Exception(errorMessage);
					}

					// Execute issue api
					var requestIssueApi = invoiceEcPayApi.CreateRequestObject(
						TwInvoiceEcPayApi.ExecuteTypes.Issue,
						orderNew,
						orderInvoiceOld,
						accessor);
					var responseIssueApi = invoiceEcPayApi.ReceiveResponseObject(
						TwInvoiceEcPayApi.ExecuteTypes.Issue,
						requestIssueApi);

					if (responseIssueApi.IsSuccess == false)
					{
						// Update Taiwan old order invoice status
						orderInvoiceOld.TwInvoiceStatus = Constants.FLG_ORDER_INVOICE_STATUS_REFUND_COMPLETED;
						orderInvoiceOld.TwInvoiceNo = responseAllowanceApi.Response.Data.IAAllowNo;
						invoiceService.UpdateTwOrderInvoiceForModify(
							orderInvoiceOld,
							this.LoginOperatorName,
							UpdateHistoryAction.Insert,
							accessor);

						// Update Taiwan new order invoice for modify
						orderInvoiceNew.TwInvoiceStatus = Constants.FLG_ORDER_INVOICE_STATUS_NOT_ISSUED;
						orderInvoiceNew.TwInvoiceNo = responseIssueApi.Response.Data.InvoiceNo;
						orderInvoiceNew.TwInvoiceDate = DateTime.Parse(responseIssueApi.Response.Data.InvoiceDate);
						errorMessage = responseIssueApi.Message;
						return;
					}

					// Update Taiwan old order invoice status
					orderInvoiceOld.TwInvoiceStatus = Constants.FLG_ORDER_INVOICE_STATUS_REFUND_COMPLETED;
					orderInvoiceOld.TwInvoiceNo = responseAllowanceApi.Response.Data.IAAllowNo;
					invoiceService.UpdateTwOrderInvoiceForModify(
						orderInvoiceOld,
						this.LoginOperatorName,
						UpdateHistoryAction.Insert,
						accessor);

					// Update Taiwan new order invoice for modify
					orderInvoiceNew.TwInvoiceStatus = Constants.FLG_ORDER_INVOICE_STATUS_ISSUED_LINKED;
					orderInvoiceNew.TwInvoiceNo = responseIssueApi.Response.Data.InvoiceNo;
					orderInvoiceNew.TwInvoiceDate = DateTime.Parse(responseIssueApi.Response.Data.InvoiceDate);
				}
			}
		}
		catch
		{
			// Rollback transaction
			accessor.RollbackTransaction();
			throw;
		}
	}

	/// <summary>
	/// 対象の頒布会定額コースの商品が全返品されるか
	/// </summary>
	/// <param name="subscriptionBoxCourseId">頒布会コースID</param>
	/// <returns>全返品であればTRUE</returns>
	protected bool IsFixedAmountCourseItemAllReturns(string subscriptionBoxCourseId)
	{
		// 元注文での数が0の場合、そもそも頒布会定額コースではないためFALSE
		var originalOrderQuantity = this.ReturnExchangeOrderOrg.GetFixedAmountCourseItemQuantityTotal(subscriptionBoxCourseId);
		if (originalOrderQuantity == 0) return false;

		var returnedOrderQuantity = 0;
		if (this.ReturnExchangeOrderOrg.ReturnedItemQuantityByFixedAmountCourse
				.TryGetValue(subscriptionBoxCourseId, out returnedOrderQuantity) == false) return false;

		var returnOrderQuantity = m_returnOrderItems
			.Where(
				item => item.IsReturnTarget
					&& item.IsSubscriptionBoxFixedAmount
					&& (item.SubscriptionBoxCourseId == subscriptionBoxCourseId))
			.Sum(item => item.ItemQuantity * -1);

		return originalOrderQuantity == (returnedOrderQuantity + returnOrderQuantity);
	}

	/// <summary>返品商品リスト</summary>
	protected List<ReturnOrderItem> lOrderItems;
	/// <summary>アイテムテーブル用追加カラム数</summary>
	protected int AddColumnCountForItemTable
	{
		get
		{
			return
				(Constants.PRODUCT_SALE_OPTION_ENABLED ? 1 : 0)
				+ ((Constants.NOVELTY_OPTION_ENABLED || Constants.RECOMMEND_OPTION_ENABLED) ? 1 : 0)
				+ (Constants.PRODUCTBUNDLE_OPTION_ENABLED ? 1 : 0)
				+ (this.DisplayItemSubscriptionBoxCourseIdArea ? 1 : 0);
		}
	}
	/// <summary>元注文情報（元注文 or 最後の返品注文）</summary>
	protected OrderModel OrderOld
	{
		get { return (OrderModel)ViewState["OrderOld"]; }
		set { ViewState["OrderOld"] = value; }
	}
	/// <summary>全返品か？</summary>
	protected bool IsReturnAllItems
	{
		get { return (bool)ViewState["IsReturnAllItems"]; }
		set { ViewState["IsReturnAllItems"] = value; }
	}
	/// <summary>処理区分</summary>
	protected ReauthCreatorFacade.ExecuteTypes ExecuteType
	{
		get { return (ReauthCreatorFacade.ExecuteTypes)ViewState["ExecuteType"]; }
		set { ViewState["ExecuteType"] = value; }
	}
	/// <summary>Is NP After Has Paid</summary>
	public bool IsNPAfterHasPaid
	{
		get { return (ViewState["IsNPAfterHasPaid"] != null) ? (bool)ViewState["IsNPAfterHasPaid"] : false; }
		set { ViewState["IsNPAfterHasPaid"] = value; }
	}
	/// <summary>CrossPointに連携した受注情報か</summary>
	protected bool IsLinkedCrossPoint
	{
		get
		{
			var result = Constants.CROSS_POINT_OPTION_ENABLED
				? Order.CheckLinkedCrossPoint(ReturnExchangeOrderOrg.OrderDate, ReturnExchangeOrderOrg.UserId)
				: false;

			return result;
		}
	}
}
