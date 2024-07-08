/*
=========================================================================================================
  Module      : ベリトランスPayPay決済(PaymentVeritransPaypay.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using jp.veritrans.tercerog.mdk;
using jp.veritrans.tercerog.mdk.dto;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Order.Payment.Paypay;
using w2.Common.Web;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;

namespace w2.App.Common.Order.Payment.Veritrans
{
	/// <summary>
	/// ベリトランスPayPay決済
	/// </summary>
	public class PaymentVeritransPaypay : PaymentVeriTransBase
	{
		/// <summary>ベリトランス決済種別</summary>
		protected override VeriTransConst.VeritransPaymentKbn VeritransPaymentType { get { return VeriTransConst.VeritransPaymentKbn.Paypay; } }

		/// <summary>
		/// 申込（与信・承諾）
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート情報</param>
		/// <returns>Paypay authorize response</returns>
		public PaypayAuthorizeResponseDto Authorize(OrderModel order, CartObject cart)
		{
			var baseUrl = string.Format(
				"{0}{1}{2}",
				Constants.PROTOCOL_HTTPS,
				string.IsNullOrEmpty(Constants.WEBHOOK_DOMAIN)
					? Constants.SITE_DOMAIN
					: Constants.WEBHOOK_DOMAIN,
				Constants.PATH_ROOT_FRONT_PC);
			var returnUrl = new UrlCreator(baseUrl + Constants.PAGE_FRONT_PAYMENT_VERITRANS_PAYPAY_RECEIVE)
				.AddParam(PaypayConstants.REQUEST_KEY_ORDERID, order.OrderId)
				.CreateUrl();
			var pushUrl = new UrlCreator(baseUrl + Constants.PAGE_FRONT_PAYMENT_VERITRANS_PAYPAY_NOTIFICATION_RECEIVE).CreateUrl();

			var requestData = new PaypayAuthorizeRequestDto
			{
				ServiceOptionType = VeriTransConst.VERITRANS_PAYMENT_SERVICE_OPTION_TYPE_ONLINE,
				OrderId = cart.HasFixedPurchase
					? order.FixedPurchaseId
					: order.PaymentOrderId,
				AccountingType = cart.HasFixedPurchase
					? VeriTransConst.ACCOUNTING_TYPE_ANYTIME
					: VeriTransConst.ACCOUNTING_TYPE_ONETIME,
				Amount = cart.HasFixedPurchase
					? string.Empty
					: cart.PriceTotal.ToPriceString(),
				ItemId = Constants.PAYMENT_PAYPAY_VERITRANS4G_ITEMID,
				ItemName = Constants.PAYMENT_PAYPAY_VERITRANS4G_ITEMNAME,
				SuccessUrl = returnUrl,
				CancelUrl = returnUrl,
				ErrorUrl = returnUrl,
				PushUrl = pushUrl,
			};
			var response = CallApi<PaypayAuthorizeResponseDto>(requestData);

			// 決済注文IDを保持する
			if (response.Mstatus == VeriTransConst.RESULT_STATUS_OK)
			{
				// 注文情報更新
				new OrderService().Modify(
					order.OrderId,
					model =>
					{
						model.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_WAIT;
						model.PaymentOrderId = order.PaymentOrderId;
					},
					UpdateHistoryAction.Insert);
			}
			return response;
		}

		/// <summary>
		/// キャンセル
		/// </summary>
		/// <param name="paymentOrderId">取引ID</param>
		/// <returns>キャンセルレスポンス</returns>
		public PaypayCancelResponseDto Cancel(string paymentOrderId)
		{
			var requestData = new PaypayCancelRequestDto
			{
				ServiceOptionType = VeriTransConst.VERITRANS_PAYMENT_SERVICE_OPTION_TYPE_ONLINE,
				OrderId = paymentOrderId
			};

			return CallApi<PaypayCancelResponseDto>(requestData);
		}

		/// <summary>
		/// 売上確定
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>売上確定レスポンス</returns>
		public PaypayCaptureResponseDto Capture(OrderModel order)
		{
			var requestData = new PaypayCaptureRequestDto
			{
				ServiceOptionType = VeriTransConst.VERITRANS_PAYMENT_SERVICE_OPTION_TYPE_ONLINE,
				OrderId = order.PaymentOrderId,
				Amount = order.OrderPriceTotal.ToPriceString(),
			};

			return CallApi<PaypayCaptureResponseDto>(requestData);
		}

		/// <summary>
		/// 再与信(随時決済)
		/// </summary>
		/// <param name="originalOrderId">元取引ID</param>
		/// <param name="newPaymentOrderId">取引ID</param>
		/// <param name="amount">請求金額</param>
		/// <returns>再与信レスポンス</returns>
		public PaypayReAuthorizeResponseDto ReAuthorize(
			string originalOrderId,
			string newPaymentOrderId,
			decimal amount)
		{
			var requestData = new PaypayReAuthorizeRequestDto
			{
				ServiceOptionType = VeriTransConst.VERITRANS_PAYMENT_SERVICE_OPTION_TYPE_ONLINE,
				OriginalOrderId = originalOrderId,
				OrderId = newPaymentOrderId,
				Amount = amount.ToPriceString(),
			};

			return CallApi<PaypayReAuthorizeResponseDto>(requestData);
		}

		/// <summary>
		/// 返金
		/// </summary>
		/// <param name="paymentOrderId">取引ID</param>
		/// <param name="refundAmount">返金金額</param>
		/// <returns>返金レスポンス</returns>
		public PaypayRefundResponseDto Refund(string paymentOrderId, decimal refundAmount)
		{
			var requestData = new PaypayRefundRequestDto
			{
				ServiceOptionType = VeriTransConst.VERITRANS_PAYMENT_SERVICE_OPTION_TYPE_ONLINE,
				OrderId = paymentOrderId,
				Amount = refundAmount.ToPriceString(),
			};

			return CallApi<PaypayRefundResponseDto>(requestData);
		}

		/// <summary>
		/// 解約
		/// </summary>
		/// <param name="paymentOrderId">取引ID</param>
		/// <returns>解約レスポンス</returns>
		public PaypayTerminateResponseDto Terminate(string paymentOrderId)
		{
			var requestData = new PaypayTerminateRequestDto
			{
				ServiceOptionType = VeriTransConst.VERITRANS_PAYMENT_SERVICE_OPTION_TYPE_ONLINE,
				OrderId = paymentOrderId,
				Force = "true"
			};

			return CallApi<PaypayTerminateResponseDto>(requestData);
		}

		/// <summary>
		/// API呼び出し
		/// </summary>
		/// <typeparam name="T">レスポンス型</typeparam>
		/// <param name="requestData">リクエスト情報</param>
		/// <returns>スポンス</returns>
		private T CallApi<T>(IRequestDto requestData)
			where T : MdkDtoBase, IResponseDto
		{
			var transaction = new Transaction();
			var responseData = (T)transaction.Execute(requestData);

			return responseData;
		}
	}
}
