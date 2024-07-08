/*
=========================================================================================================
  Module      : 配送先項目要素 (DeliveryElement.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using jp.veritrans.tercerog.mdk.dto;
using w2.Domain.Order;

namespace w2.App.Common.Order.Payment.Veritrans.ObjectElement
{
	/// <summary>
	/// 配送先情報要素
	/// </summary>
	public class DeliveryElement : ScoreatpayDeliveryDto
	{
		/// <summary>デフォルト：デリバリーID</summary>
		private const string DEFAULT_DERIVERY_ID = "1";

		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public DeliveryElement()
		{
			this.DeliveryId = string.Empty;
			this.Contact = null;
			this.Details = null;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="order">注文情報</param>
		public DeliveryElement(OrderModel order)
		{
			this.DeliveryId = order.Items[0].OrderShippingNo.ToString();
			this.Contact = new BuyerElement(order);
			this.Details = new DetailsElement(order).Details;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="cart">カート情報</param>
		public DeliveryElement(CartObject cart)
		{
			this.DeliveryId = string.IsNullOrEmpty(cart.Shippings[0].ShippingNo)
				? DEFAULT_DERIVERY_ID
				: cart.Shippings[0].ShippingNo;
			this.Contact = new BuyerElement(cart);
			this.Details = new DetailsElement(cart).Details;
		}
	}
}
