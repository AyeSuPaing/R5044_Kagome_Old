using System;
using w2.Database.Common;
using w2.Domain.Order;

namespace w2.DomainTests.Order
{
	/// <summary>
	/// 注文モデル ユニットテスト用ファクトリ
	/// </summary>
	public static class OrderModelFactoryForTest
	{
		/// <summary>
		/// オーソリ済みクレカ注文を作成
		/// </summary>
		/// <param name="lastBilledAmount">最終請求金額</param>
		/// <returns>注文モデル</returns>
		public static OrderModel CreateAuthorizedCreditCard(decimal lastBilledAmount) =>
			new OrderModel
			{
				OrderId = "TEST-ORDER-00001",
				LastBilledAmount = lastBilledAmount,
				OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
				CardTranId = "test-transaction-id1",
				ExternalPaymentAuthDate = DateTime.Now,
				CreditBranchNo = 1,
				CardInstallmentsCode = "1",
			};

		/// <summary>
		/// オーソリ済みクレカ注文を作成（与信日時なし）
		/// </summary>
		/// <param name="lastBilledAmount">最終請求金額</param>
		/// <param name="externalPaymentStatus">外部決済ステータス</param>
		/// <returns>注文モデル</returns>
		public static OrderModel CreateAuthorizedCreditCardWithoutAuthDate(
			decimal lastBilledAmount,
			string externalPaymentStatus = "") =>
			new OrderModel
			{
				OrderId = "TEST-ORDER-00001",
				LastBilledAmount = lastBilledAmount,
				OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
				CardTranId = "test-transaction-id1",
				ExternalPaymentStatus = externalPaymentStatus,
				ExternalPaymentAuthDate = null,
				CreditBranchNo = 1,
				CardInstallmentsCode = "1",
			};

		/// <summary>
		/// 未オーソリクレカ注文を作成
		/// </summary>
		/// <param name="lastBilledAmount">最終請求金額</param>
		/// <returns>注文モデル</returns>
		public static OrderModel CreateUnauthorizeCreditCard(decimal lastBilledAmount) =>
			new OrderModel
			{
				OrderId = "TEST-ORDER-00001",
				LastBilledAmount = lastBilledAmount,
				OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
				CreditBranchNo = 1,
				CardInstallmentsCode = "1",
			};

		/// <summary>
		/// 代引き注文を作成
		/// </summary>
		/// <param name="lastBilledAmount">最終請求金額</param>
		/// <returns>注文モデル</returns>
		public static OrderModel CreateCollectOnDelivery(decimal lastBilledAmount) =>
			new OrderModel
			{
				OrderId = "TEST-ORDER-00001",
				LastBilledAmount = lastBilledAmount,
				OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT,
			};

		/// <summary>
		/// コンビニ後払い注文を作成
		/// </summary>
		/// <param name="lastBilledAmount">最終請求金額</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="externalPaymentStatus">外部決済ステータス</param>
		/// <param name="orderPaymentStatus">入金ステータス</param>
		/// <param name="ownerName">注文者名</param>
		/// <param name="shippingDate">配送希望日</param>
		/// <param name="hasRecommendItem">レコメンド商品を含むか</param>
		/// <returns>注文モデル</returns>
		public static OrderModel CreateCvsDef(
			decimal lastBilledAmount = 1000m,
			string orderId = "TEST-ORDER-00001",
			string paymentOrderId = "test-payment-order-id1",
			string externalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED,
			string orderPaymentStatus = Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM,
			string ownerName = "テスト注文者1",
			DateTime? shippingDate = null,
			bool hasRecommendItem = false) =>
			new OrderModel
			{
				OrderId = orderId,
				LastBilledAmount = lastBilledAmount,
				OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF,
				CardTranId = "test-transaction-id1",
				PaymentOrderId = paymentOrderId,
				ExternalPaymentStatus = externalPaymentStatus,
				OrderPaymentStatus = orderPaymentStatus,
				Owner = new OrderOwnerModel
				{
					OwnerName = ownerName,
				},
				Shippings = new[]
				{
					new OrderShippingModel
					{
						ShippingDate = shippingDate,
					},
				},
				Items = new[]
				{
					new OrderItemModel { RecommendId = hasRecommendItem ? "test-recommend1" : string.Empty },
				},
			};

		/// <summary>
		/// GMO掛け払い(都度与信)注文を生成
		/// </summary>
		/// <param name="lastBilledAmount">最終請求金額</param>
		/// <param name="onlinePaymentStatus">オンライン決済ステータス</param>
		/// <param name="orderStatus">注文ステータス</param>
		/// <returns>注文</returns>
		public static OrderModel CreatePayAsYouGoOrder(
			decimal lastBilledAmount = 1000m,
			string onlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED,
			string orderStatus = Constants.FLG_ORDER_ORDER_STATUS_ORDERED) =>
			new OrderModel
			{
				LastBilledAmount = lastBilledAmount,
				OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO,
				OnlinePaymentStatus = onlinePaymentStatus,
				OrderStatus = orderStatus,
				PaymentOrderId = "test-payment-order-id1",
				Items = new[] { new OrderItemModel() },
				Shippings = new[] { new OrderShippingModel() },
				OrderPriceByTaxRates = new[] { new OrderPriceByTaxRateModel(), new OrderPriceByTaxRateModel() },
			};

		/// <summary>
		/// GMO掛け払い(枠保証)注文を生成
		/// </summary>
		/// <param name="lastBilledAmount">最終請求金額</param>
		/// <param name="onlinePaymentStatus">オンライン決済ステータス</param>
		/// <param name="orderStatus">注文ステータス</param>
		/// <returns>注文</returns>
		public static OrderModel CreateFrameGuaranteeOrder(
			decimal lastBilledAmount = 1000m,
			string onlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED,
			string orderStatus = Constants.FLG_ORDER_ORDER_STATUS_ORDERED) =>
			new OrderModel
			{
				LastBilledAmount = lastBilledAmount,
				OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE,
				OnlinePaymentStatus = onlinePaymentStatus,
				OrderStatus = orderStatus,
				PaymentOrderId = "test-payment-order-id1",
				Items = new[] { new OrderItemModel() },
				Shippings = new[] { new OrderShippingModel() },
				OrderPriceByTaxRates = new[] { new OrderPriceByTaxRateModel(), new OrderPriceByTaxRateModel() },
			};

		/// <summary>
		/// PayPay注文を生成
		/// </summary>
		/// <param name="lastBilledAmount">最終請求金額</param>
		/// <param name="cardTranId">決済カード取引ID</param>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="onlinePaymentStatus">オンライン決済ステータス</param>
		/// <param name="externalPaymentStatus">外部決済ステータス</param>
		/// <returns>注文</returns>
		public static OrderModel CreatePayPayOrder(
			decimal lastBilledAmount = 1000m,
			string cardTranId = "test-transaction-id1",
			string paymentOrderId = "test-payment-order-id1",
			string onlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED,
			string externalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP) =>
			new OrderModel
			{
				LastBilledAmount = lastBilledAmount,
				OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY,
				CardTranId = cardTranId,
				PaymentOrderId = paymentOrderId,
				OnlinePaymentStatus = onlinePaymentStatus,
				ExternalPaymentStatus = externalPaymentStatus,
			};

		/// <summary>
		/// AmazonPay注文を作成
		/// </summary>
		/// <param name="lastBilledAmount">最終請求金額</param>
		/// <param name="cartdTranId">決済カード取引ID</param>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="externalPaymentStatus">外部決済ステータス</param>
		/// <param name="onlinePaymentStatus">オンライン決済ステータス</param>
		/// <param name="returnExchangeKbn">返品交換区分</param>
		/// <returns></returns>
		public static OrderModel CreateAmazonPay(
			decimal lastBilledAmount,
			string cartdTranId = "test-transaction-id1",
			string paymentOrderId = "test-payment-order-id1",
			string externalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED,
			string onlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_NONE,
			string returnExchangeKbn = Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_UNKNOWN) =>
			new OrderModel
			{
				OrderId = "TEST-ORDER-00001",
				LastBilledAmount = lastBilledAmount,
				OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT,
				CardTranId = cartdTranId,
				PaymentOrderId = paymentOrderId,
				ExternalPaymentStatus = externalPaymentStatus,
				OnlinePaymentStatus = onlinePaymentStatus,
				ReturnExchangeKbn = returnExchangeKbn,
			};

		/// <summary>
		/// AmazonPayCv2注文を作成
		/// </summary>
		/// <param name="lastBilledAmount">最終請求金額</param>
		/// <param name="cartdTranId">決済カード取引ID</param>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="externalPaymentStatus">外部決済ステータス</param>
		/// <param name="onlinePaymentStatus">オンライン決済ステータス</param>
		/// <param name="returnExchangeKbn">返品交換区分</param>
		/// <returns></returns>
		public static OrderModel CreateAmazonPayCv2(
			decimal lastBilledAmount,
			string cartdTranId = "test-transaction-id1",
			string paymentOrderId = "test-payment-order-id1",
			string externalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED,
			string onlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_NONE,
			string returnExchangeKbn = Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_UNKNOWN) =>
			new OrderModel
			{
				OrderId = "TEST-ORDER-00001",
				LastBilledAmount = lastBilledAmount,
				OrderPaymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2,
				CardTranId = cartdTranId,
				PaymentOrderId = paymentOrderId,
				ExternalPaymentStatus = externalPaymentStatus,
				OnlinePaymentStatus = onlinePaymentStatus,
				ReturnExchangeKbn = returnExchangeKbn,
			};
	}
}
