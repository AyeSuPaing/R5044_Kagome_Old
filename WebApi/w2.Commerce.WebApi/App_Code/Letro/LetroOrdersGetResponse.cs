/*
=========================================================================================================
  Module      : Letro Orders Get Response (LetroOrdersGetResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Data;
using w2.Common.Util;
using w2.Domain.Order;

namespace Letro
{
	/// <summary>
	/// Letro orders get response
	/// </summary>
	[Serializable]
	public class LetroOrdersGetResponse
	{
		/// <summary>注文情報</summary>
		[JsonProperty("orders")]
		public OrderDetail[] Orders { get; set; }
	}

	/// <summary>
	/// Order detail
	/// </summary>
	[Serializable]
	public class OrderDetail
	{
		/// <summary>キャンセルや返品交換などされていない通常の注文</summary>
		public const string RESPONSE_PARAM_ORDER_STATUS_NORMAL = "NORMAL";
		/// <summary>キャンセル</summary>
		public const string RESPONSE_PARAM_ORDER_STATUS_CANCEL = "CANCEL";
		/// <summary>返品交換元注文</summary>
		public const string RESPONSE_PARAM_ORDER_STATUS_ORDER_RETURN_EXCHANGE = "ORDER_RETURN_EXCHANGE";
		/// <summary>返品</summary>
		public const string RESPONSE_PARAM_ORDER_STATUS_RETURN = "RETURN";
		/// <summary>交換</summary>
		public const string RESPONSE_PARAM_ORDER_STATUS_EXCHANGE = "EXCHANGE";
		/// <summary>date format</summary>
		public const string DATE_FORMAT = "yyyy/MM/dd";

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="data">Data</param>
		public OrderDetail(OrderModel data)
		{
			this.OrderId = StringUtility.ToEmpty(data.OrderId);
			this.UserId = StringUtility.ToEmpty(data.UserId);
			this.OrderDate = StringUtility.ToDateFormat(data.OrderDate);
			this.ShippedDate = StringUtility.ToDateFormat(
				data.OrderShippedDate,
				DATE_FORMAT);
			this.OrderCount = StringUtility.ToEmpty(data.FixedPurchaseOrderCount);
			this.ShippedCount = StringUtility.ToEmpty(data.FixedPurchaseShippedCount);
			this.OrderStatus = GetOrderStatus(
				StringUtility.ToEmpty(data.OrderStatus),
				StringUtility.ToEmpty(data.ShippedChangedKbn),
				StringUtility.ToEmpty(data.ReturnExchangeKbn));
			this.FixedPurchaseId = StringUtility.ToEmpty(data.FixedPurchaseId);
		}

		/// <summary>
		/// Get order status for response
		/// </summary>
		/// <param name="orderStatus">Order status</param>
		/// <param name="shippedChangedKbn">Shipped changed kbn</param>
		/// <param name="returnExchangeKbn">Return exchange kbn</param>
		/// <returns>Order status</returns>
		private string GetOrderStatus(string orderStatus, string shippedChangedKbn, string returnExchangeKbn)
		{
			if (orderStatus == Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED)
			{
				return RESPONSE_PARAM_ORDER_STATUS_CANCEL;
			}

			if (shippedChangedKbn == Constants.FLG_ORDER_SHIPPED_CHANGED_KBN_CHANAGED)
			{
				return RESPONSE_PARAM_ORDER_STATUS_ORDER_RETURN_EXCHANGE;
			}

			switch (returnExchangeKbn)
			{
				case Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_RETURN:
					return RESPONSE_PARAM_ORDER_STATUS_RETURN;

				case Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE:
					return RESPONSE_PARAM_ORDER_STATUS_EXCHANGE;

				default:
					return RESPONSE_PARAM_ORDER_STATUS_NORMAL;
			}
		}

		/// <summary>注文ID</summary>
		[JsonProperty(Constants.FIELD_ORDER_ORDER_ID)]
		public string OrderId { get; set; }
		/// <summary>顧客ID</summary>
		[JsonProperty(Constants.FIELD_ORDER_USER_ID)]
		public string UserId { get; set; }
		/// <summary>注文日</summary>
		[JsonProperty(Constants.FIELD_ORDER_ORDER_DATE)]
		public string OrderDate { get; set; }
		/// <summary>出荷日</summary>
		[JsonProperty(Constants.FIELD_ORDER_ORDER_SHIPPED_DATE)]
		public string ShippedDate { get; set; }
		/// <summary>定期購入回数</summary>
		[JsonProperty(Constants.FIELD_FIXEDPURCHASE_ORDER_COUNT)]
		public string OrderCount { get; set; }
		/// <summary>定期出荷回数</summary>
		[JsonProperty(Constants.FIELD_FIXEDPURCHASE_SHIPPED_COUNT)]
		public string ShippedCount { get; set; }
		/// <summary>注文ステータス</summary>
		[JsonProperty(Constants.FIELD_ORDER_ORDER_STATUS)]
		public string OrderStatus { get; set; }
		/// <summary>商品情報</summary>
		[JsonProperty("order_products")]
		public OrderProduct[] OrderProducts { get; set; }
		/// <summary>定期購入ID</summary>
		[JsonIgnore]
		public string FixedPurchaseId { get; set; }
		/// <summary>定期購入注文か</summary>
		[JsonIgnore]
		public bool IsFixedPurchase
		{
			get { return string.IsNullOrEmpty(this.FixedPurchaseId) == false; }
		}
	}

	/// <summary>
	/// Order product
	/// </summary>
	[Serializable]
	public class OrderProduct
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="data">Data</param>
		/// <param name="isFixedPurchase">Is fixed purchase</param>
		public OrderProduct(OrderItemModel data, bool isFixedPurchase = false)
		{
			this.ProductId = StringUtility.ToEmpty(data.ProductId);
			this.VariationId = StringUtility.ToEmpty(data.VariationId);
			this.ProductOrderCount = isFixedPurchase
				? StringUtility.ToEmpty(data.FixedPurchaseItemOrderCount)
				: string.Empty;
			this.ProductShippedCount = isFixedPurchase
				? StringUtility.ToEmpty(data.FixedPurchaseItemShippedCount)
				: string.Empty;
		}

		/// <summary>商品ID</summary>
		[JsonProperty(Constants.FIELD_ORDERITEM_PRODUCT_ID)]
		public string ProductId { get; set; }
		/// <summary>商品バリエーションID</summary>
		[JsonProperty(Constants.FIELD_ORDERITEM_VARIATION_ID)]
		public string VariationId { get; set; }
		/// <summary>定期商品購入回数</summary>
		[JsonProperty("product_order_count")]
		public string ProductOrderCount { get; set; }
		/// <summary>定期商品出荷回数</summary>
		[JsonProperty("product_shipped_count")]
		public string ProductShippedCount { get; set; }
	}
}
