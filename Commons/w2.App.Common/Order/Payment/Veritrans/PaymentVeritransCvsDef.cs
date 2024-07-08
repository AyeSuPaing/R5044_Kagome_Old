/*
=========================================================================================================
  Module      : ベリトランス後払い連携(PaymentVeritransCvsDef.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using jp.veritrans.tercerog.mdk;
using jp.veritrans.tercerog.mdk.dto;
using System;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Order.Payment.Veritrans.Helper;
using w2.App.Common.Order.Payment.Veritrans.ObjectElement;
using w2.Domain.Order;

namespace w2.App.Common.Order.Payment.Veritrans
{
	/// <summary>
	/// ベリトランス後払い連携
	/// </summary>
	public class PaymentVeritransCvsDef : PaymentVeriTransBase
	{
		/// <summary>ベリトランス決済種別</summary>
		protected override VeriTransConst.VeritransPaymentKbn VeritransPaymentType { get { return VeriTransConst.VeritransPaymentKbn.CvsDef; } }

		/// <summary>
		/// 注文情報登録
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>レスポンス値</returns>
		public ScoreatpayAuthorizeResponseDto OrderRegister(OrderModel order)
		{
			var requestData = new ScoreatpayAuthorizeRequestDto
			{
				OrderId = order.PaymentOrderId,
				Amount = order.OrderPriceTotal.ToPriceString(),
				ShopOrderDate = order.OrderDate?.ToString("yyyy/MM/dd"),
				BuyerContact = new BuyerElement(order),
				PaymentType = (order.InvoiceBundleFlg == Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_ON)
					? VeriTransConst.VERITRANS_PAYMENT_TYPE_INCLUDE_SERVICE
					: VeriTransConst.VERITRANS_PAYMENT_TYPE_SEPARATE_SERVICE,
				Delivery = new DeliveryElement(order),
			};
			var responseData = (ScoreatpayAuthorizeResponseDto)new Transaction().Execute(requestData);

			return responseData;
		}

		/// <summary>
		/// 注文情報登録
		/// </summary>
		/// <param name="cart">カート情報</param>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <returns>レスポンス値</returns>
		public ScoreatpayAuthorizeResponseDto OrderRegister(CartObject cart, string paymentOrderId)
		{
			var requestData = new ScoreatpayAuthorizeRequestDto
			{
				OrderId = paymentOrderId,
				Amount = cart.PriceTotal.ToPriceString(),
				ShopOrderDate = DateTime.Now.ToString("yyyy/MM/dd"),
				BuyerContact = new BuyerElement(cart),
				PaymentType = cart.GetInvoiceBundleFlg() == Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_ON
					? VeriTransConst.VERITRANS_PAYMENT_TYPE_INCLUDE_SERVICE
					: VeriTransConst.VERITRANS_PAYMENT_TYPE_SEPARATE_SERVICE,
				Delivery = new DeliveryElement(cart),
			};
			var responseData = (ScoreatpayAuthorizeResponseDto)new Transaction().Execute(requestData);

			return responseData;
		}

		/// <summary>
		/// 注文情報変更
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>レスポンス値</returns>
		public ScoreatpayCorrectAuthResponseDto OrderModify(OrderModel order)
		{
			var requestData = new ScoreatpayCorrectAuthRequestDto
			{
				OrderId = order.PaymentOrderId,
				Amount = order.OrderPriceTotal.ToPriceString(),
				ShopOrderDate = order.OrderDate?.ToString("yyyy/MM/dd"),
				BuyerContact = new BuyerElement(order),
				PaymentType = order.InvoiceBundleFlg == Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_ON
					? VeriTransConst.VERITRANS_PAYMENT_TYPE_SEPARATE_SERVICE
					: VeriTransConst.VERITRANS_PAYMENT_TYPE_INCLUDE_SERVICE,
				Delivery = new DeliveryElement(order),
			};
			var responseData = (ScoreatpayCorrectAuthResponseDto)new Transaction().Execute(requestData);

			return responseData;
		}

		/// <summary>
		/// 発送情報登録
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="slipNo">配送伝票番号</param>
		/// <returns>レスポンス値</returns>
		public ScoreatpayCaptureResponseDto DeliveryRegister(OrderModel order, string slipNo = "")
		{
			var requestData = new ScoreatpayCaptureRequestDto
			{
				OrderId = order.PaymentOrderId,
				PdCompanyCode = PaymentVeritransDeferredDeliveryServiceCode.GetDeliveryServiceCode(order.Shippings[0].DeliveryCompanyName),
				SlipNo = slipNo,
				DeliveryId = order.Shippings[0].OrderShippingNo.ToString(),
			};
			var responseData = (ScoreatpayCaptureResponseDto)new Transaction().Execute(requestData);

			return responseData;
		}

		/// <summary>
		/// 発送情報登録
		/// </summary>
		/// <param name="requestData">リクエスト値</param>
		/// <returns>レスポンス値</returns>
		public ScoreatpayCaptureResponseDto DeliveryRegister(ScoreatpayCaptureRequestDto requestData)
		{
			var responseData = (ScoreatpayCaptureResponseDto)new Transaction().Execute(requestData);

			return responseData;
		}

		/// <summary>
		/// 注文キャンセル
		/// </summary>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <returns>レスポンス値</returns>
		public ScoreatpayCancelResponseDto OrderCancel(string paymentOrderId)
		{
			var requestData = new ScoreatpayCancelRequestDto
			{
				OrderId = paymentOrderId,
			};
			var responseData = (ScoreatpayCancelResponseDto)new Transaction().Execute(requestData);

			return responseData;
		}

		/// <summary>
		/// 与信結果取得
		/// </summary>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <returns>レスポンス値</returns>
		public ScoreatpayConfirmResponseDto GetAuthResult(string paymentOrderId)
		{
			var requestData = new ScoreatpayConfirmRequestDto
			{
				OrderId = paymentOrderId,
			};
			var responseData = (ScoreatpayConfirmResponseDto)new Transaction().Execute(requestData);

			return responseData;
		}

		/// <summary>
		/// 請求書印字データ取得
		/// </summary>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <returns>レスポンス値</returns>
		public ScoreatpayGetInvoiceDataResponseDto GetInvoiceData(string paymentOrderId)
		{
			var requestData = new ScoreatpayGetInvoiceDataRequestDto
			{
				OrderId = paymentOrderId,
			};
			var responseData = (ScoreatpayGetInvoiceDataResponseDto)new Transaction().Execute(requestData);

			return responseData;
		}
	}
}
