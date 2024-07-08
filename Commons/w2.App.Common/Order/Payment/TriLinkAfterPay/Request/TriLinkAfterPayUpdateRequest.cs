/*
=========================================================================================================
  Module      : 後付款(TriLink後払い) 注文情報更新リクエストクラス(TriLinkAfterPayUpdateRequest.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using w2.Domain.Order;

namespace w2.App.Common.Order.Payment.TriLinkAfterPay.Request
{
	/// <summary>
	/// 後付款(TriLink後払い) 注文情報更新リクエストクラス
	/// </summary>
	[JsonObject]
	public class TriLinkAfterPayUpdateRequest : RequestBase
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="co">CartObject</param>
		/// <param name="cardTranId">決済取引ID</param>
		public TriLinkAfterPayUpdateRequest(string paymentOrderId, CartObject co, string cardTranId) 
			: this(
				paymentOrderId,
				new PaymentTriLinkAfterPayRequestCustomerData(co.Owner, co.OrderUserId),
				new PaymentTriLinkAfterPayRequestShipmentData(co.GetShipping()),
				Decimal.ToInt32(co.SendingAmount),
				co.Items.Select(item => new PaymentTriLinkAfterPayRequestItemData(item, co)).ToArray(),
				Constants.PAYMENT_SETTING_TRILINK_AFTERPAY_API_URL + "orders/" + Constants.PAYMENT_SETTING_TRILINK_AFTERPAY_SITE_CODE + "/" + cardTranId)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="order">OrderModel</param>
		public TriLinkAfterPayUpdateRequest(OrderModel order) 
			: this(
				order.PaymentOrderId,
				new PaymentTriLinkAfterPayRequestCustomerData(order.Owner, order.UserId),
				new PaymentTriLinkAfterPayRequestShipmentData(order.Shippings[0]),
				Decimal.ToInt32(GetSendingAmount(order)),
				order.Items.Select(orderItem => new PaymentTriLinkAfterPayRequestItemData(orderItem, order)).ToArray(),
				Constants.PAYMENT_SETTING_TRILINK_AFTERPAY_API_URL + "orders/" + Constants.PAYMENT_SETTING_TRILINK_AFTERPAY_SITE_CODE + "/" + order.CardTranId)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="storeOrderCode">加盟店注文番号</param>
		/// <param name="customer">購入者</param>
		/// <param name="shipment">配送先</param>
		/// <param name="billingAmount">請求金額</param>
		/// <param name="items">購入商品情報</param>
		/// <param name="requestUrl">リクエストURL</param>
		public TriLinkAfterPayUpdateRequest(
			string storeOrderCode,
			PaymentTriLinkAfterPayRequestCustomerData customer,
			PaymentTriLinkAfterPayRequestShipmentData shipment,
			int billingAmount,
			PaymentTriLinkAfterPayRequestItemData[] items,
			string requestUrl)
		{
			this.StoreOrderCode = storeOrderCode;
			this.BuyerOrderDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
			this.Customer = customer;
			this.Shipment = shipment;
			this.BillingAmount = billingAmount;
			this.Items = items;
			this.RequestUrl = requestUrl;
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
