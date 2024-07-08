/*
=========================================================================================================
  Module      : スコア後払い取引登録のリクエスト値(ScoreRequestOrderRegisterModify.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using w2.App.Common.Option;
using w2.App.Common.Order.Payment.Score.ObjectsElement;
using w2.Domain.Order;

namespace w2.App.Common.Order.Payment.Score.Order
{
	/// <summary>
	/// 取引登録のリクエスト値
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "request", IsNullable = false, Namespace = "")]
	public class ScoreRequestOrderRegisterModify : BaseScoreRequest
	{
		/// <summary>受注明細：送料</summary>
		private const string ORDER_DETAIL_NAME_SHIPPING = "送料";
		/// <summary>受注明細：決済手数料</summary>
		private const string ORDER_DETAIL_NAME_PAYMENT = "決済手数料";
		/// <summary>受注明細：割引等</summary>
		private const string ORDER_DETAIL_NAME_DISCOUNT_ETC = "割引等";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ScoreRequestOrderRegisterModify()
		{
			this.HttpInfo = new HttpInfoElement();
			this.Buyer = new BuyerElement();
			this.Deliveries = new DeliveryElements();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="order">注文情報</param>
		public ScoreRequestOrderRegisterModify(OrderModel order)
			: this()
		{
			this.HttpInfo.DeviceInfo = order.DeviceInfo;
			this.Buyer = new BuyerElement(order);
			this.Deliveries.Delivery = GetDeliveryElements(order);
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="cart">カート情報</param>
		public ScoreRequestOrderRegisterModify(CartObject cart)
			: this()
		{
			this.HttpInfo.DeviceInfo = cart.DeviceInfo;
			this.Buyer = new BuyerElement(cart);
			this.Deliveries.Delivery = GetDeliveryElements(cart);
		}

		/// <summary>
		/// 配送詳細要素取得
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>配送詳細</returns>
		private DeliveryElement[] GetDeliveryElements(OrderModel order)
		{
			var delivery = new DeliveryElement
			{
				DeliveryCustomer = new DeliveryCustomerElement(order.Shippings[0])
			};

			var detailList = new List<DetailElement>();

			// 商品
			var productsDetail = order.Items
				.Select(
					item => new DetailElement(
						detailName: item.ProductName,
						detailPrice: item.ProductPrice,
						detailQuantity: item.ItemQuantity))
				.ToArray();

			// 送料
			var shipDetail = new DetailElement(
				detailName: ORDER_DETAIL_NAME_SHIPPING,
				detailPrice: order.OrderPriceShipping,
				detailQuantity: 1);

			// 決済手数料
			var exchangeDetail = new DetailElement(
				detailName: ORDER_DETAIL_NAME_PAYMENT,
				detailPrice: order.OrderPriceExchange,
				detailQuantity: 1);

			// 割引等
			var discountDetail = new DetailElement(
				detailName: ORDER_DETAIL_NAME_DISCOUNT_ETC,
				detailPrice: (order.LastBilledAmount
					- (TaxCalculationUtility.GetPriceTaxIncluded(order.OrderPriceSubtotal, order.OrderPriceSubtotalTax)
						+ order.OrderPriceShipping
						+ order.OrderPriceExchange)),
				detailQuantity: 1);

			detailList.AddRange(productsDetail);
			detailList.Add(shipDetail);
			detailList.Add(exchangeDetail);
			detailList.Add(discountDetail);

			delivery.Details.Detail = detailList.ToArray();
			return new[] { delivery };
		}
		/// <summary>
		/// 配送詳細要素取得
		/// </summary>
		/// <param name="cart">カート情報</param>
		/// <returns>配送詳細要素</returns>
		private DeliveryElement[] GetDeliveryElements(CartObject cart)
		{
			var delivery = new DeliveryElement
			{
				DeliveryCustomer = new DeliveryCustomerElement(cart.GetShipping())
			};

			var detailList = new List<DetailElement>();

			// 商品
			var productsDetail = cart.Items
				.Select(
					item => new DetailElement(
						detailName: item.ProductName,
						detailPrice: TaxCalculationUtility.GetPriceTaxIncluded(item.Price, item.PriceTax),
						detailQuantity: item.Count))
				.ToArray();

			// 送料
			var shipDetail = new DetailElement(
				detailName: ORDER_DETAIL_NAME_SHIPPING,
				detailPrice: cart.PriceShipping,
				detailQuantity: 1);

			//決済手数料
			var exchangeDetail = new DetailElement(
				detailName: ORDER_DETAIL_NAME_PAYMENT,
				detailPrice: cart.Payment.PriceExchange,
				detailQuantity: 1);

			//割引等
			var discountDetail = new DetailElement(
				detailName: ORDER_DETAIL_NAME_DISCOUNT_ETC,
				detailPrice: (cart.PriceTotal
					- (TaxCalculationUtility.GetPriceTaxIncluded(cart.PriceSubtotal, cart.PriceSubtotalTax)
						+ cart.PriceShipping + cart.Payment.PriceExchange)),
				detailQuantity: 1);

			detailList.AddRange(productsDetail);
			detailList.Add(shipDetail);
			detailList.Add(exchangeDetail);
			detailList.Add(discountDetail);

			delivery.Details.Detail = detailList.ToArray();
			return new[] { delivery };
		}

		/// <summary>端末識別情報</summary>
		[XmlElement("httpInfo")]
		public HttpInfoElement HttpInfo { get; set; }
		/// <summary>購入者情報</summary>
		[XmlElement("buyer")]
		public BuyerElement Buyer { get; set; }
		/// <summary>配送先項目</summary>
		[XmlElement("deliveries")]
		public DeliveryElements Deliveries { get; set; }
	}
}
