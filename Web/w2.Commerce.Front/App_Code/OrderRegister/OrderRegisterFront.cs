/*
=========================================================================================================
  Module      : ＰＣ注文向け注文登録クラス(OrderRegisterFront.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using w2.App.Common;
using w2.Common.Web;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.UpdateHistory;
using w2.App.Common.Global.Region;
using w2.App.Common.Order;
using w2.App.Common.Order.OrderCombine;
using w2.App.Common.Order.Register;
using w2.App.Common.Util;
using w2.Domain.ContentsLog;
using w2.Domain.MailTemplate;
using w2.App.Common.Order.OPlux;
using w2.Domain.FixedPurchase;

public class OrderRegisterFront : OrderRegisterBase
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="isUser">ユーザーか（ポイント付与判断など）</param>
	public OrderRegisterFront(bool isUser)
		: base(ExecTypes.Pc, isUser, Constants.FLG_LASTCHANGED_USER)
	{
		this.DocomoPaymentOrder = new Dictionary<string, CartObject>();
		this.SBPSMultiPaymentOrders = new List<KeyValuePair<string, Hashtable>>();
		this.LinePayOrders = new List<KeyValuePair<string, Hashtable>>();
		this.ZeusLinkPointPaymentOrders = new List<Hashtable>();
		this.GoogleAnalyticsParams = new List<Hashtable>();
		this.AmazonPayCv2Orders = new List<KeyValuePair<string, Hashtable>>();
		this.YamatoKaSmsOrders = new List<KeyValuePair<string, Hashtable>>();
		this.RakutenOrders = new List<KeyValuePair<string, Hashtable>>();
		this.PaypayOrders = new Dictionary<string, Hashtable>();
		this.BokuPaymentOrders = new List<KeyValuePair<string, Hashtable>>();
		this.GmoAtokaraOrders = new List<KeyValuePair<string, Hashtable>>();
		this.PaidyPaygentOrders = new Dictionary<string, Hashtable>();
	}

	/// <summary>
	/// 外部決済かどうかチェック
	/// </summary>
	/// <param name="order">注文情報</param>
	/// <param name="cart">カート</param>
	/// <returns>外部決済か</returns>
	protected override bool CheckExternalPayment(Hashtable order, CartObject cart)
	{
		// ドコモケータイ払い（ドコモ接続）
		if (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_ORG)
		{
			this.DocomoPaymentOrder.Add(cart.OrderId, cart);
			return true;
		}
		// SBPSマルチ決済（GoogleAnalyticsパラメタ格納は注文完了後処理で行う）
		else if ((cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_SOFTBANKKETAI_SBPS)
			|| (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AUKANTAN_SBPS)
			|| (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_SBPS)
			|| (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_RECRUIT_SBPS)
			|| (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL_SBPS)
			|| (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_RAKUTEN_ID_SBPS)
			|| ((cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
				&& (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.SBPS)
				&& Constants.PAYMENT_SETTING_SBPS_CREDIT_3DSECURE)
			|| ((cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
				&& (Constants.PAYMENT_PAYPAY_KBN == Constants.PaymentPayPayKbn.SBPS)))
		{
			this.SBPSMultiPaymentOrders.Add(new KeyValuePair<string, Hashtable>(cart.OrderId, order));
			return true;
		}
		else if (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY)
		{
			this.LinePayOrders.Add(new KeyValuePair<string, Hashtable>(cart.OrderId, order));
			return (cart.IsPreApprovedLinePayPayment == false);
		}
		// ゼウスLinkPoint
		else if ((cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
			&& ((cart.Payment.CreditCardBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW))
			&& (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Zeus)
			&& Constants.PAYMENT_SETTING_ZEUS_USE_LINKPOINT_ENABLED)
		{
			this.ZeusLinkPointPaymentOrders.Add(order);
			return true;
		}
		// AmazonPay(CV2)
		else if (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2)
		{
			order.Add(Constants.FIELD_ORDER_AMAZON_PAY_CV2_CHECKOUTSESSION_ID, cart.ExternalPaymentAgreementId);
			order.Add(Constants.FIELD_ORDER_ORDER_PRICE_TOTAL, cart.PriceTotal);

			this.AmazonPayCv2Orders.Add(new KeyValuePair<string, Hashtable>(cart.OrderId, order));
			return true;
		}
		else if (cart.Payment.IsPaymentYamatoKaSms)
		{
			this.YamatoKaSmsOrders.Add(new KeyValuePair<string, Hashtable>(cart.OrderId, order));
			return true;
		}
		// Paypay order
		else if (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
		{
			switch (Constants.PAYMENT_PAYPAY_KBN)
			{
				case Constants.PaymentPayPayKbn.GMO:
					if (cart.HasFixedPurchase && (string.IsNullOrEmpty(cart.OrderCombineParentOrderId) == false))
					{
						return false;
					}
					break;

				case Constants.PaymentPayPayKbn.VeriTrans:
					if (cart.HasFixedPurchase && string.IsNullOrEmpty((string)order[Constants.FIELD_ORDER_FIXED_PURCHASE_ID]))
					{
						return false;
					}
					break;

				default:
					return false;
			}

			this.PaypayOrders.Add(cart.OrderId, order);
			return true;
		}
		else if ((cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
				&& (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten)
				&& Constants.PAYMENT_SETTING_RAKUTEN_3DSECURE)
		{
			order.Add(CartPayment.FIELD_CREDIT_RAKUTEN_CVV_TOKEN, cart.Payment.RakutenCvvToken);
			this.RakutenCard3DSecurePaymentOrders.Add(order);
			return true;
		}
		// Boku payment
		else if (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU)
		{
			order[Constants.FIELD_ORDER_ORDER_PRICE_TOTAL] = cart.PriceTotal;
			order[Constants.FIELD_ORDER_MEMO] = cart.GetOrderMemos();
			order[Constants.FIELD_USER_ADDR_COUNTRY_ISO_CODE] = cart.Owner.AddrCountryIsoCode;
			order[Constants.FIELD_ORDER_SETTLEMENT_CURRENCY] = cart.SettlementCurrency;
			this.BokuPaymentOrders.Add(new KeyValuePair<string, Hashtable>(cart.OrderId, order));
			return true;
		}
		// ペイジェント
		else if ((cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
			&& (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Paygent)
			&& Constants.PAYMENT_PAYGENT_CREDIT_3DSECURE
			)
		{
			// 支払い回数を格納しておく
			order["paygent_installments"] = cart.Payment.CreditInstallmentsCode;
			this.PaygentCard3DSecurePaymentOrders.Add(order);
			return true;
		}
		// GMOアトカラ
		else if (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA)
		{
			this.GmoAtokaraOrders.Add(new KeyValuePair<string, Hashtable>(cart.OrderId, order));
			return true;
		}
		else if (cart.Payment.IsPaymentPaygentPaidy)
		{
			this.PaidyPaygentOrders.Add(cart.OrderId, order);
			return true;
		}

		return false;
	}

	/// <summary>
	/// 外部決済で決済注文ID発行が必要かどうか
	/// </summary>
	/// <param name="paymentId">決済種別ID</param>
	/// <returns>true: 発行必要、false: 発行不要</returns>
	protected override bool IsPaymentOrderIdIssue(string paymentId)
	{
		// SBPSクレジット決済（３Dセキュー）、かつ、キャリア決済
		if ((paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_SOFTBANKKETAI_SBPS)
			|| (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AUKANTAN_SBPS)
			|| (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_SBPS)
			|| (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_RECRUIT_SBPS)
			|| (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL_SBPS)
			|| (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_RAKUTEN_ID_SBPS)
			|| (OrderCommon.CheckPaymentYamatoKaSms(paymentId))
			|| ((paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT) &&
				(((Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.SBPS) && Constants.PAYMENT_SETTING_SBPS_CREDIT_3DSECURE)
					|| (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans)))
			|| ((paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
				&& (Constants.PAYMENT_PAYPAY_KBN == Constants.PaymentPayPayKbn.SBPS)))
		{
			return true;
		}
		return false;
	}

	/// <summary>
	/// 注文完了時の処理
	/// </summary>
	/// <param name="order">注文情報</param>
	/// <param name="cart">カート</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <returns>アラート文言</returns>
	public override string OrderCompleteProcesses(Hashtable order, CartObject cart, UpdateHistoryAction updateHistoryAction)
	{
		var dispAlertMessages = new StringBuilder();
		try
		{
			// 注文同梱で既存の注文台帳を更新する場合スキップ(親注文定期購購入なしでかつ子注文定期購入あり)
			// 注文同梱時、頒布会が含まれる場合は子注文は別台帳扱いになるため登録する（頒布会同コース同士であればスキップ）
			if (cart.HasFixedPurchase && ((cart.IsOrderCombined == false) || cart.IsRegisterFixedPurchaseWhenOrderCombine))
			{
				this.TransactionName = "4-1.定期購入ステータス更新処理";
				//注文者のリージョンデータを最新に更新
				cart.Owner.UpdateRegion(RegionManager.GetInstance().Region);
				// 仮登録の定期台帳を更新（更新履歴とともに）
				UpdateFixedPurchaseStatusTempToNormal(order, UpdateHistoryAction.Insert);
			}

			// コンテンツログ登録
			if (cart.ContentsLog != null)
			{
				var contentsLog = new ContentsLogModel(cart.ContentsLog.DataSource)
				{
					AccessKbn = ((string)order[Constants.FIELD_ORDER_ORDER_KBN] == Constants.FLG_ORDER_ORDER_KBN_SMARTPHONE ? Constants.FLG_CONTENTSLOG_ACCESS_KBN_SP : Constants.FLG_CONTENTSLOG_ACCESS_KBN_PC),
					ReportType = Constants.FLG_CONTENTSLOG_REPORT_TYPE_CV,
					Date = DateTime.Now,
					Price = cart.PriceTotal,
				};
				new ContentsLogService().Insert(contentsLog);
			}

			var ecInvoiceApiMessage = StringUtility.ToEmpty(order[OrderCommon.ECPAY_INVOICE_API_MESSAGE]);
			var isEcInvoiceApiSuccess = string.IsNullOrEmpty(ecInvoiceApiMessage);
			if (isEcInvoiceApiSuccess == false)
			{
				dispAlertMessages.Append(ecInvoiceApiMessage);
			}

			this.TransactionName = "4-2-1.メール送信処理";
			if (Constants.LANDING_CART_USER_REGISTER_WHEN_ORDER_COMPLETE
				&& (order[Constants.ORDER_KEY_MAIL_FOR_USER_REGISTER_WHEN_ORDER_COMPLETE] != null))
			{
				SendUserRegisterMail((Hashtable)order[Constants.ORDER_KEY_MAIL_FOR_USER_REGISTER_WHEN_ORDER_COMPLETE]);
			}
			if ((cart.Payment.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
				&& (cart.Payment.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
				&& isEcInvoiceApiSuccess)
			{
				if (string.IsNullOrEmpty(cart.OrderCombineParentOrderId))
				{
					// 注文完了メールを送信
					SendOrderMails(order, cart, this.IsUser);
				}
				else
				{
					// 注文同梱完了メールを送信
					OrderCombineUtility.SendOrderCombineCompleteMailToUser(order, cart);
				}
			}

			// LINEを送信
			if (Constants.REPEATLINE_OPTION_ENABLED == Constants.RepeatLineOption.CooperationAndMessaging)
			{
				this.TransactionName = "4-2-2.LINE連携処理";
				var sendLineMessageFlg = MailSendUtility.GetMailTemplateInfo(
					w2.Domain.Constants.CONST_DEFAULT_SHOP_ID,
					w2.App.Common.Constants.CONST_MAIL_ID_ORDER_COMPLETE).LineUseFlg;

				if (sendLineMessageFlg == MailTemplateModel.LINE_USE_FLG_ON) SendOrderCompleteToLine(order, cart);
			}

			this.TransactionName = "4-3クレジット登録確定処理";
			dispAlertMessages.Append(UpdateUserCreditCard(order, cart, this.IsUser, UpdateHistoryAction.DoNotInsert));

			// 注文同梱された場合、注文対象のカートがDBにないため削除処理をスキップ
			if (string.IsNullOrEmpty(cart.OrderCombineParentOrderId)
				&& (cart.Payment.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY))
			{
				this.TransactionName = "4-4.カート削除処理"; // セッションからはまだ消えない
				dispAlertMessages.Append(DeleteCart(order, cart));

				// 更新履歴登録
				if (updateHistoryAction == UpdateHistoryAction.Insert)
				{
					new UpdateHistoryService().InsertAllForOrder((string)order[Constants.FIELD_ORDER_ORDER_ID], this.LastChanged);
				}
			}
		}
		catch (Exception ex)
		{
			throw new Exception(this.TransactionName + " でエラーが発生しました。", ex);
		}
		return dispAlertMessages.ToString();
	}

	/// <summary>
	/// 注文完了後の処理（セッションを利用するもの）
	/// </summary>
	/// <param name="order">注文情報</param>
	/// <param name="cart">カート情報</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	public override void AfterOrderCompleteProcesses(
		Hashtable order,
		CartObject cart,
		UpdateHistoryAction updateHistoryAction)
	{
		try
		{
			this.TransactionName = "5-1.アフィリエイト情報登録処理";
			RegistAffiliateInfo(order, HttpContext.Current.Request);

			this.TransactionName = "5-2.GoogleAnalytics用パラメータ作成";
			this.GoogleAnalyticsParams.Add(CreateGoogleAnalyticsParams((string)order[Constants.FIELD_ORDER_ORDER_ID], cart));

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertAllForOrder((string)order[Constants.FIELD_ORDER_ORDER_ID], this.LastChanged);
			}

			// 頒布会の次回配送商品を変更する
			UpdateNextSubscriptionBoxProduct(cart, order);
		}
		catch (Exception ex)
		{
			throw new Exception(this.TransactionName + " でエラーが発生しました。", ex);
		}
	}

	/// <summary>
	/// 注文完了スキップ時の処理
	/// </summary>
	/// <param name="order">注文情報</param>
	/// <param name="cart">カート情報</param>
	public override void SkipOrderCompleteProcesses(Hashtable order, CartObject cart)
	{
		try
		{
			this.GoogleAnalyticsParams.Add(CreateGoogleAnalyticsParams((string)order[Constants.FIELD_ORDER_ORDER_ID], cart));
		}
		catch (Exception ex)
		{
			throw new Exception("5-1.GoogleAnalytics用パラメータ作成でエラーが発生しました。", ex);
		}
		SetOrderExtendForOplux(order, cart);
	}

	/// <summary>
	/// アフィリエイト情報登録
	/// </summary>
	/// <param name="order">注文情報</param>
	/// <param name="request">HTTPリクエスト</param>
	public static void RegistAffiliateInfo(Hashtable order, HttpRequest request)
	{
		var linkshareCookie = CookieManager.Get(Constants.COOKIE_KEY_AFFILIATE_LINKSHARE);

		if ((Constants.AFFILIATE_LINKSHARE_VALID) && (linkshareCookie != null))
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("AffiliateCoopLog", "InsertAffiliateCoopLog"))
			{
				Hashtable input = new Hashtable();
				input.Add(Constants.FIELD_AFFILIATECOOPLOG_AFFILIATE_KBN, Constants.FLG_AFFILIATECOOPLOG_AFFILIATE_KBN_LINKSHARE_REP);
				input.Add(Constants.FIELD_AFFILIATECOOPLOG_MASTER_ID, order[Constants.FIELD_ORDER_ORDER_ID]);
				input.Add(Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA1, linkshareCookie.Values[Constants.REQUEST_KEY_LINK_AFFILIATE_LST_ID]);
				input.Add(Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA2, linkshareCookie.Values[Constants.REQUEST_KEY_LINK_AFFILIATE_ARRIVE_DATETIME]);
				input.Add(Constants.FIELD_AFFILIATECOOPLOG_COOP_STATUS, Constants.FLG_AFFILIATECOOPLOG_COOP_STATUS_WAIT);
				input.Add(Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA11, linkshareCookie.Values[Constants.REQUEST_KEY_LINK_AFFILIATE_TAG_ID]);

				int update = sqlStatement.ExecStatementWithOC(sqlAccessor, input);
			}
		}
	}

	/// <summary>
	/// GoogleAnalytics用パラメータ作成
	/// </summary>
	/// <param name="orderId">注文ID</param>
	/// <param name="cart">カート情報</param>
	/// <returns>パラメータ情報</returns>
	protected static Hashtable CreateGoogleAnalyticsParams(string orderId, CartObject cart)
	{
		var orderParams = new Hashtable();

		// 注文情報
		orderParams.Add(Constants.FIELD_ORDER_ORDER_ID, orderId);
		orderParams.Add(Constants.FIELD_ORDER_USER_ID, cart.OrderUserId);
		orderParams.Add(Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL, cart.PriceSubtotal);
		// 商品小計税額（w2_Order.order_price_subtotal_tax）は注文割引分が含まれるため、
		// 明細金額（税金額）（w2_OrderItem.item_price_tax）の合計を税額商品合計金額とする
		// ※アフィリエイトタグも同様の計算を行っている
		orderParams.Add(Constants.FIELD_ORDERITEM_ITEM_PRICE_TAX + "_sum", cart.Items.Select(i => i.PriceTax).Sum());
		orderParams.Add(Constants.FIELD_ORDER_ORDER_PRICE_TOTAL, cart.PriceTotal);

		// 配送料（配送料が0円の場合は空文字を設定）
		orderParams.Add(Constants.FIELD_ORDER_ORDER_PRICE_SHIPPING, (cart.PriceShipping > 0) ? StringUtility.ToEmpty(cart.PriceShipping) : "");

		// 注文者
		orderParams.Add(Constants.FIELD_ORDEROWNER_OWNER_KBN, cart.Owner.OwnerKbn);
		orderParams.Add(Constants.FIELD_ORDEROWNER_OWNER_ZIP, cart.Owner.Zip1 + "-" + cart.Owner.Zip2);
		orderParams.Add(Constants.FIELD_ORDEROWNER_OWNER_ADDR1, cart.Owner.Addr1);
		orderParams.Add(Constants.FIELD_ORDEROWNER_OWNER_ADDR2, cart.Owner.Addr2);
		orderParams.Add(Constants.FIELD_ORDEROWNER_OWNER_SEX, cart.Owner.Sex);
		orderParams.Add(Constants.FIELD_ORDEROWNER_OWNER_BIRTH, cart.Owner.Birth);

		// 注文商品情報
		var items = new List<Hashtable>();
		foreach (CartProduct product in cart.Items)
		{
			// 同一商品が存在する？
			var tempItem = items.Find(ti => ((product.ShopId == (string)ti[Constants.FIELD_ORDERITEM_SHOP_ID])
				&& (product.ProductId == (string)ti[Constants.FIELD_ORDERITEM_PRODUCT_ID])
				&& (product.VariationId == (string)ti[Constants.FIELD_ORDERITEM_VARIATION_ID])));
			if (tempItem != null)
			{
				// 数量を加算
				tempItem[Constants.FIELD_ORDERITEM_ITEM_QUANTITY] = (int)tempItem[Constants.FIELD_ORDERITEM_ITEM_QUANTITY] + product.CountSingle;
				continue;
			}

			// 追加
			var item = new Hashtable();
			item.Add(Constants.FIELD_ORDERITEM_SHOP_ID, cart.ShopId);
			item.Add(Constants.FIELD_ORDERITEM_ORDER_ID, orderId);
			item.Add(Constants.FIELD_ORDERITEM_PRODUCT_ID, product.ProductId);
			item.Add(Constants.FIELD_ORDERITEM_VARIATION_ID, product.VariationId);
			item.Add(Constants.FIELD_ORDERITEM_PRODUCT_NAME, product.ProductJointName);
			item.Add(Constants.FIELD_ORDERITEM_PRODUCT_PRICE, product.Price);
			item.Add(Constants.FIELD_ORDERITEM_PRODUCT_TAX_RATE, product.TaxRate);
			item.Add(Constants.FIELD_ORDERITEM_ITEM_QUANTITY, product.CountSingle);
			items.Add(item);
		}
		orderParams.Add("order_items", items);

		return orderParams;
	}

	/// <summary>
	/// OPLUXの評価がREVIEWの場合、注文拡張ステータスを有効にする。
	/// </summary>
	/// <param name="order">注文情報</param>
	/// <param name="cart">カート情報</param>
	private void SetOrderExtendForOplux(Hashtable order, CartObject cart)
	{
		if (Constants.OPLUX_ENABLED
			&& (string.IsNullOrEmpty(Constants.OPLUX_REVIEW_EXTEND_NO) == false)
			&& (StringUtility.ToEmpty(order[Constants.OPLUX_REGISTER_EVENT_API_RESULT]) == OPluxConst.API_REGISTER_EVENT_RESULT_REVIEW))
		{
			new OrderService().UpdateOrderExtendStatus(
				StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]),
				int.Parse(Constants.OPLUX_REVIEW_EXTEND_NO),
				Constants.FLG_ORDER_ORDER_EXTEND_STATUS_ON,
				DateTime.Now,
				StringUtility.ToEmpty(order[Constants.FIELD_ORDER_LAST_CHANGED]),
				UpdateHistoryAction.DoNotInsert);

			if (Constants.FIXEDPURCHASE_OPTION_ENABLED
				&& cart.HasFixedPurchase)
			{
				new FixedPurchaseService().UpdateExtendStatus(
					StringUtility.ToEmpty(order[Constants.FIELD_ORDER_FIXED_PURCHASE_ID]),
					int.Parse(Constants.OPLUX_REVIEW_EXTEND_NO),
					Constants.FLG_ORDER_ORDER_EXTEND_STATUS_ON,
					DateTime.Now,
					StringUtility.ToEmpty(order[Constants.FIELD_ORDER_LAST_CHANGED]),
					UpdateHistoryAction.DoNotInsert);
			}
		}
	}

	/// <summary>
	/// GoogleAnalytics用パラメータ追加
	/// </summary>
	/// <param name="orderId">注文ID</param>
	/// <param name="cart">カート情報</param>
	public void AddGoogleAnalyticsParams(string orderId, CartObject cart)
	{
		this.GoogleAnalyticsParams.Add(CreateGoogleAnalyticsParams(orderId, cart));
	}

	/// <summary>ドコモ決済注文</summary>
	public Dictionary<string, CartObject> DocomoPaymentOrder { get; private set; }
	/// <summary>SBPSマルチ決済注文</summary>
	public List<KeyValuePair<string, Hashtable>> SBPSMultiPaymentOrders { get; private set; }
	/// <summary>LINEペイ決済注文</summary>
	public List<KeyValuePair<string, Hashtable>> LinePayOrders { get; private set; }
	/// <summary>ゼウスLinkPoint注文</summary>
	public List<Hashtable> ZeusLinkPointPaymentOrders { get; set; }
	/// <summary>GoogleAnaliticsログ出力用の注文情報</summary>
	public List<Hashtable> GoogleAnalyticsParams { get; set; }
	/// <summary>AmazonPan(CV2)決済注文</summary>
	public List<KeyValuePair<string, Hashtable>> AmazonPayCv2Orders { get; set; }
	/// <summary>ヤマト後払いSMS認証連携決済注文</summary>
	public List<KeyValuePair<string, Hashtable>> YamatoKaSmsOrders { set; get; }
	/// <summary>Paypay orders</summary>
	public Dictionary<string, Hashtable> PaypayOrders { get; set; }
	/// <summary>Rakutenマルチ決済注文</summary>
	public List<KeyValuePair<string, Hashtable>> RakutenOrders { get; set; }
	/// <summary>Boku payment orders</summary>
	public List<KeyValuePair<string, Hashtable>> BokuPaymentOrders { set; get; }
	/// <summary>GMOアトカラ注文</summary>
	public List<KeyValuePair<string, Hashtable>> GmoAtokaraOrders { set; get; }
	/// <summary>決済待ち注文数</summary>
	public int WaitPaymentOrderCount
	{
		get
		{
			var result =
				this.DocomoPaymentOrder.Count
				+ this.SBPSMultiPaymentOrders.Count
				+ this.LinePayOrders.Count
				+ this.ZeusLinkPointPaymentOrders.Count
				+ this.GoogleAnalyticsParams.Count
				+ this.AmazonPayCv2Orders.Count
				+ this.YamatoKaSmsOrders.Count
				+ this.RakutenOrders.Count
				+ this.PaypayOrders.Count
				+ this.BokuPaymentOrders.Count
				+ this.GmoAtokaraOrders.Count;
			return result;
		}
	}
	/// <summary>Paidy paygent orders</summary>
	public Dictionary<string, Hashtable> PaidyPaygentOrders { set; get; }
}
