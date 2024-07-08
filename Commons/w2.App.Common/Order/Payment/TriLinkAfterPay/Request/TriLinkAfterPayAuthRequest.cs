/*
=========================================================================================================
  Module      : 後付款(TriLink後払い) 注文審査リクエストクラス(TriLinkAfterPayAuthRequest.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using w2.Domain.Order;

namespace w2.App.Common.Order.Payment.TriLinkAfterPay.Request
{
	/// <summary>
	/// 後付款(TriLink後払い) 注文審査リクエストクラス
	/// </summary>
	[JsonObject]
	public class TriLinkAfterPayAuthRequest : RequestBase
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="cart">CartObject</param>
		/// <param name="accessToken">アクセストークン</param>
		public TriLinkAfterPayAuthRequest(CartObject cart, string accessToken) : this(
			new PaymentTriLinkAfterPayRequestCustomerData(cart.Owner, cart.OrderUserId),
			new PaymentTriLinkAfterPayRequestShipmentData(cart.GetShipping()),
			Decimal.ToInt32(cart.SendingAmount),
			cart.Items.Select(cartItem => new PaymentTriLinkAfterPayRequestItemData(cartItem, cart)).ToArray(),
			accessToken)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="order">OrderModel</param>
		/// <param name="accessToken">アクセストークン</param>
		public TriLinkAfterPayAuthRequest(OrderModel order, string accessToken) : this(
			new PaymentTriLinkAfterPayRequestCustomerData(order.Owner, order.UserId),
			new PaymentTriLinkAfterPayRequestShipmentData(order.Shippings[0]),
			Decimal.ToInt32(GetSendingAmount(order)),
			order.Items.Select(orderItem => new PaymentTriLinkAfterPayRequestItemData(orderItem, order)).ToArray(),
			accessToken)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="customer">購入者</param>
		/// <param name="shipment">配送先</param>
		/// <param name="billingAmount">請求金額</param>
		/// <param name="items">購入商品情報</param>
		/// <param name="accessToken">アクセストークン</param>
		public TriLinkAfterPayAuthRequest(
			PaymentTriLinkAfterPayRequestCustomerData customer,
			PaymentTriLinkAfterPayRequestShipmentData shipment,
			int billingAmount,
			PaymentTriLinkAfterPayRequestItemData[] items,
			string accessToken)
		{
			this.BuyerOrderDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
			this.RequestUrl = Constants.PAYMENT_SETTING_TRILINK_AFTERPAY_API_URL + "orders/authorization/" + Constants.PAYMENT_SETTING_TRILINK_AFTERPAY_SITE_CODE;
			this.Customer = customer;
			this.Shipment = shipment;
			this.BillingAmount = billingAmount;
			this.Items = items;
			this.AddRequestHeaders = new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>("Authorization","Bearer " + accessToken)
			};
		}
		#endregion

		#region プロパティ
		/// <summary>購入者注文日</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_BUYER_ORDER_DATE)]
		public string BuyerOrderDate { get; private set; }
		/// <summary>購入者</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_CUSTOMER)]
		public PaymentTriLinkAfterPayRequestCustomerData Customer { get; private set; }
		/// <summary>配送先</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_SHIPMENT)]
		public PaymentTriLinkAfterPayRequestShipmentData Shipment { get; private set; }
		/// <summary>請求金額</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_BILLING_AMOUNT)]
		public int BillingAmount { get; private set; }
		/// <summary>購入商品情報</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_ITEMS)]
		public PaymentTriLinkAfterPayRequestItemData[] Items { get; private set; }
		#endregion
	}
}
