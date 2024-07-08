/*
=========================================================================================================
  Module      : SBPS マルチ決済 購入要求ページ(PaymentSBPSMultiPaymentExecOrder.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Web;
using w2.Domain.UpdateHistory.Helper;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment;
using w2.Domain.Order;

public partial class Payment_SBPS_PaymentSBPSMultiPaymentExecOrder : OrderCartPageExternalPayment
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
			string orderId = Request[Constants.REQUEST_KEY_ORDER_ID];

			PaymentFileLogger.WritePaymentLog(
				null,
				"",
				PaymentFileLogger.PaymentType.Sbps,
				PaymentFileLogger.PaymentProcessingType.Unknown,
				"マルチ決済 購入要求通知\t" + LogCreator.CreateMessage(orderId, ""));

			// 注文IDチェック（二重送信しようとしていたらロールバックして確認画面へ）
			CheckOrderId(orderId, UpdateHistoryAction.Insert);

			// 注文プロパティ情報取得
			GetOrderPropertiesFromSession();

			// 該当注文取得
			var order = this.SBPSMultiPaymentOrders.Find(kvp => kvp.Key == orderId).Value;
			// 注文情報復元できなければエラーページへ
			if (order == null) RedirectToNextPage(order, WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CARDAUTH_ERROR));

			// 該当カート取得
			this.CartList = GetTargetCartList(order);

			if ((this.CartList.Items.Count == 0)
				&& (Session[Constants.SESSION_KEY_CART_LIST_FOR_PAYPAY] != null))
			{
				this.CartList = (CartObjectList)Session[Constants.SESSION_KEY_CART_LIST_FOR_PAYPAY];
			}
			var cart = this.CartList.Items.Find(c => c.OrderId == orderId);

			// カート内容と注文内容の金額を比較
			var orderModel = new OrderService().Get(orderId);
			if ((cart.PriceTotal != orderModel.OrderPriceTotal)
				&& (cart.Payment.PaymentId == orderModel.OrderPaymentKbn))
			{
				RedirectToNextPage(null, WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CART_CHANGED), Constants.PAGE_FRONT_CART_LIST);
			}

			// クレジットカード枝番紐付け
			if (cart.Payment.UserCreditCard != null)
			{
				order[Constants.FIELD_ORDER_CREDIT_BRANCH_NO] = cart.Payment.UserCreditCard.BranchNo;
			}

			// セッション情報をDB保存
			SessionSecurityManager.SaveSesstionContetnsToDatabaseForGoToOtherSite(Session, orderId);

			// フォーム作成
			PaymentSBPSMultiPaymentExecOrder payment = new PaymentSBPSMultiPaymentExecOrder();

			// クレジットカード決済の場合、決済注文IDで連携する。その以外、注文IDで連携する
			var paymentOrderId = (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID];

			// 遷移先・購入結果通知受取URLセット
			string orderNoticeUrl = null;
			if (Constants.PAYMENT_SETTING_SBPS_ORDER_LINK_URL_LOCALTEST == "")
			{
				// 本番向け
				this.OrderUrl = Constants.PAYMENT_SETTING_SBPS_ORDER_LINK_URL;
				orderNoticeUrl = this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_PAYMENT_SBPS_MULTIPAYMENT_ORDER_NOTICE + "?" + Constants.REQUEST_KEY_ORDER_ID + "=" + HttpUtility.UrlEncode(orderId);
			}
			else
			{
				// ローカルテストONの場合はローカル遷移先とする
				this.OrderUrl = Constants.PAYMENT_SETTING_SBPS_ORDER_LINK_URL_LOCALTEST;
				orderNoticeUrl = Request.Url.Scheme + Uri.SchemeDelimiter + Environment.MachineName + Constants.PATH_ROOT + Constants.PAGE_FRONT_PAYMENT_SBPS_MULTIPAYMENT_ORDER_NOTICE + "?" + Constants.REQUEST_KEY_ORDER_ID + "=" + HttpUtility.UrlEncode(orderId);
			}

			// SBPS顧客登録用顧客ID決定
			string sbpsCustCode = null;
			if (cart.Payment.UserCreditCard != null)
			{
				sbpsCustCode = cart.Payment.UserCreditCard.CooperationInfo.SBPSCustCode;
			}
			else
			{
				sbpsCustCode = PaymentSBPSUtil.CreateCustCode(cart.OrderUserId);
			}

			// 継続課金（定期・従量）サービス利用されるかの判定
			var isRecurringCharge = (cart.HasFixedPurchase
				&& OrderCommon.IsUsablePaymentContinuous(cart.Payment.PaymentId));

			// FORMタグ作成
			lFormInputs.Text = payment.CreateOrderFromInputs(
				PaymentSBPSUtil.ConvertPaymentIdToPayMethodType(cart.Payment.PaymentId),
				sbpsCustCode,
				paymentOrderId,
				Constants.PAYMENT_SETTING_SBPS_ITEM_ID,
				Constants.PAYMENT_SETTING_SBPS_ITEM_NAME,
				new List<PaymentSBPSBase.ProductItem>(),
				cart.SendingAmount,
				false,
				this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_PAYMENT_SBPS_MULTIPAYMENT_ORDER_RESULT + "?" + Constants.REQUEST_KEY_ORDER_ID + "=" + HttpUtility.UrlEncode(orderId),
				this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_PAYMENT_SBPS_MULTIPAYMENT_ORDER_CANCEL + "?" + Constants.REQUEST_KEY_ORDER_ID + "=" + HttpUtility.UrlEncode(orderId),
				this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_PAYMENT_SBPS_MULTIPAYMENT_ORDER_ERROR + "?" + Constants.REQUEST_KEY_ORDER_ID + "=" + HttpUtility.UrlEncode(orderId),
				orderNoticeUrl,
				GetFreeCsv(cart),
				isRecurringCharge);

			// チェック用注文IDセット
			Session[Constants.SESSION_KEY_ORDER_ID_CHECK_FOR_LINK_TYPE_PAYMENT] = orderId;
		}
	}

	/// <summary>
	/// 注文IDチェック（二重送信しようとしていたらロールバックして確認画面へ）
	/// </summary>
	/// <param name="orderId">対象注文ID</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	private void CheckOrderId(string orderId, UpdateHistoryAction updateHistoryAction)
	{
		if (orderId != (string)Session[Constants.SESSION_KEY_ORDER_ID_CHECK_FOR_LINK_TYPE_PAYMENT]) return;

		// セッション復元＆注文ロールバック
		var order = RestoreSessionAndRollbackOrder(orderId, updateHistoryAction);

		// 注文リストから削除＆キャンセルリストに追加
		this.SBPSMultiPaymentOrders.RemoveAll(o => o.Key == orderId);
		this.CancelOrders.Add(order);

		// 次の画面へ遷移
		RedirectToNextPage(order);
	}

	/// <summary>
	/// プリー項目取得
	/// </summary>
	/// <param name="cart">カート情報</param>
	/// <returns></returns>
	private IPaymentSBPSFreeCSV GetFreeCsv(CartObject cart)
	{
		switch (cart.Payment.PaymentId)
		{
			case Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT:
				// 新規カード利用
				if (cart.Payment.CreditCardBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
				{
					return new PaymentSBPSFreeCSVCredit(
						((CartPayment.CreditTokenInfoSbps)cart.Payment.CreditToken).Token,
						((CartPayment.CreditTokenInfoSbps)cart.Payment.CreditToken).TokenKey,
						PaymentSBPSUtil.GetCreditDivideInfo(cart.Payment.CreditInstallmentsCode),
						cart.Payment.UserCreditCardRegistFlg || cart.HasFixedPurchase);
				}
				// 登録カード利用
				else
				{
					return new PaymentSBPSFreeCSVCredit(
						PaymentSBPSUtil.GetCreditDivideInfo(cart.Payment.CreditInstallmentsCode));
				}
		}
		return null;
	}

	/// <summary>注文URL</summary>
	protected string OrderUrl { get; set; }
	/// <summary>ログイン必須判定</summary>
	public override bool NeedsLogin { get { return false; } }
}