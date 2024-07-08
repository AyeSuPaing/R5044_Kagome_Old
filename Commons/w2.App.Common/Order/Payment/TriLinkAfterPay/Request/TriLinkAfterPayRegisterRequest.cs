/*
=========================================================================================================
  Module      : 後付款(TriLink後払い) 注文登録リクエストクラス(TriLinkAfterPayRegisterRequest.cs)
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
	/// 後付款(TriLink後払い) 注文登録リクエストクラス
	/// </summary>
	[JsonObject]
	public class TriLinkAfterPayRegisterRequest : RequestBase
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="cart">カート情報</param>
		/// /// <param name="paymentOrderId">決済注文ID</param>
		public TriLinkAfterPayRegisterRequest(CartObject cart, string paymentOrderId)
			: this(
				paymentOrderId,
				new PaymentTriLinkAfterPayRequestCustomerData(cart.Owner, cart.OrderUserId),
				new PaymentTriLinkAfterPayRequestShipmentData(cart.GetShipping()),
				Decimal.ToInt32(cart.SendingAmount),
				cart.Items.Select(cartItem => new PaymentTriLinkAfterPayRequestItemData(cartItem, cart)).ToArray())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="order">注文情報モデル</param>
		/// <param name="paymentOrderId">決済注文ID</param>
		public TriLinkAfterPayRegisterRequest(OrderModel order, string paymentOrderId)
			: this(
				paymentOrderId,
				new PaymentTriLinkAfterPayRequestCustomerData(order.Owner, order.UserId),
				new PaymentTriLinkAfterPayRequestShipmentData(order.Shippings[0]),
				Decimal.ToInt32(GetSendingAmount(order)),
				order.Items.Select(orderItem => new PaymentTriLinkAfterPayRequestItemData(orderItem, order)).ToArray())
		{
		}
		/// /// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="storeOrderCode">加盟店注文番号</param>
		/// <param name="customer">購入者</param>
		/// <param name="shipment">配送先</param>
		/// <param name="billingAmount">請求金額</param>
		/// <param name="items">購入商品情報</param>
		public TriLinkAfterPayRegisterRequest(
			string storeOrderCode,
			PaymentTriLinkAfterPayRequestCustomerData customer,
			PaymentTriLinkAfterPayRequestShipmentData shipment,
			int billingAmount,
			PaymentTriLinkAfterPayRequestItemData[] items)
		{
			this.StoreOrderCode = storeOrderCode;
			this.BuyerOrderDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
			this.Customer = customer;
			this.Shipment = shipment;
			this.BillingAmount = billingAmount;
			this.Items = items;
			this.RequestUrl = Constants.PAYMENT_SETTING_TRILINK_AFTERPAY_API_URL + "orders/" + Constants.PAYMENT_SETTING_TRILINK_AFTERPAY_SITE_CODE;
		}
		#endregion

		#region プロパティ
		/// <summary>加盟店注文番号</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_STORE_ORDER_CODE)]
		public string StoreOrderCode { get; set; }
		/// <summary>購入者注文日</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_BUYER_ORDER_DATE)]
		public string BuyerOrderDate { get; set; }
		/// <summary>購入者</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_CUSTOMER)]
		public PaymentTriLinkAfterPayRequestCustomerData Customer { get; set; }
		/// <summary>配送先</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_SHIPMENT)]
		public PaymentTriLinkAfterPayRequestShipmentData Shipment { get; set; }
		/// <summary>請求金額</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_BILLING_AMOUNT)]
		public int BillingAmount { get; set; }
		/// <summary>購入商品情報</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_ITEMS)]
		public PaymentTriLinkAfterPayRequestItemData[] Items { get; set; }
		#endregion
	}
}
