/*
=========================================================================================================
  Module      : Line Lastest Order Get Response (LineLastestOrderGetResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;

/// <summary>
/// LinePay Lastest Order Get Response
/// </summary>
[Serializable]
public class LineLastestOrderGetResponse
{
	/// <summary>Order Id</summary>
	[JsonProperty(PropertyName = "order_id")]
	public string OrderId { get; set; }
	/// <summary>User Id</summary>
	[JsonProperty(PropertyName = "user_id")]
	public string UserId { get; set; }
	/// <summary>Order Kbn</summary>
	[JsonProperty(PropertyName = "order_kbn")]
	public string OrderKbn { get; set; }
	/// <summary>Order Payment Kbn</summary>
	[JsonProperty(PropertyName = "order_payment_kbn")]
	public string OrderPaymentKbn { get; set; }
	/// <summary>Order Status</summary>
	[JsonProperty(PropertyName = "order_status")]
	public string OrderStatus { get; set; }
	/// <summary>Order Date</summary>
	[JsonProperty(PropertyName = "order_date")]
	public string OrderDate { get; set; }
	/// <summary>Order Shipped Date</summary>
	[JsonProperty(PropertyName = "order_shipped_date")]
	public string OrderShippedDate { get; set; }
	/// <summary>Order Payment Status</summary>
	[JsonProperty(PropertyName = "order_payment_status")]
	public string OrderPaymentStatus { get; set; }
	/// <summary>Order Item Count</summary>
	[JsonProperty(PropertyName = "order_item_count")]
	public int OrderItemCount { get; set; }
	/// <summary>Order Product Count</summary>
	[JsonProperty(PropertyName = "order_product_count")]
	public int OrderProductCount { get; set; }
	/// <summary>Order Price Subtotal</summary>
	[JsonProperty(PropertyName = "order_price_subtotal")]
	public decimal OrderPriceSubtotal { get; set; }
	/// <summary>Order Price Tax</summary>
	[JsonProperty(PropertyName = "order_price_tax")]
	public decimal OrderPriceTax { get; set; }
	/// <summary>Order Price Shipping</summary>
	[JsonProperty(PropertyName = "order_price_shipping")]
	public decimal OrderPriceShipping { get; set; }
	/// <summary>Order Price Exchange</summary>
	[JsonProperty(PropertyName = "order_price_exchange")]
	public decimal OrderPriceExchange { get; set; }
	/// <summary>Order Price Regulation</summary>
	[JsonProperty(PropertyName = "order_price_regulation")]
	public decimal OrderPriceRegulation { get; set; }
	/// <summary>Order Coupon Use</summary>
	[JsonProperty(PropertyName = "order_coupon_use")]
	public decimal OrderCouponUse { get; set; }
	/// <summary>Order Point Use</summary>
	[JsonProperty(PropertyName = "order_point_use")]
	public decimal OrderPointUse { get; set; }
	/// <summary>Order Point Use Yen</summary>
	[JsonProperty(PropertyName = "order_point_use_yen")]
	public decimal OrderPointUseYen { get; set; }
	/// <summary>Order Discount Set Price</summary>
	[JsonProperty(PropertyName = "order_discount_set_price")]
	public decimal OrderDiscountSetPrice { get; set; }
	/// <summary>Member Rank Discount Price</summary>
	[JsonProperty(PropertyName = "member_rank_discount_price")]
	public decimal MemberRankDiscountPrice { get; set; }
	/// <summary>Order Price Total</summary>
	[JsonProperty(PropertyName = "order_price_total")]
	public decimal OrderPriceTotal { get; set; }
	/// <summary>Order Point Add</summary>
	[JsonProperty(PropertyName = "order_point_add")]
	public decimal OrderPointAdd { get; set; }
	/// <summary>Fixed Purchase Id</summary>
	[JsonProperty(PropertyName = "fixed_purchase_id")]
	public string FixedPurchaseId { get; set; }
	/// <summary>Member Rank Id</summary>
	[JsonProperty(PropertyName = "member_rank_id")]
	public string MemberRankId { get; set; }
	/// <summary>Setpromotion Product Discount Amount</summary>
	[JsonProperty(PropertyName = "setpromotion_product_discount_amount")]
	public decimal SetpromotionProductDiscountAmount { get; set; }
	/// <summary>Setpromotion Shipping Charge Discount Amount</summary>
	[JsonProperty(PropertyName = "setpromotion_shipping_charge_discount_amount")]
	public decimal SetpromotionShippingChargeDiscountAmount { get; set; }
	/// <summary>Setpromotion Payment Charge Discount Amount</summary>
	[JsonProperty(PropertyName = "setpromotion_payment_charge_discount_amount")]
	public decimal SetpromotionPaymentChargeDiscountAmount { get; set; }
	/// <summary>Fixed Purchase Order Count</summary>
	[JsonProperty(PropertyName = "fixed_purchase_order_count")]
	public int FixedPurchaseOrderCount { get; set; }
	/// <summary>Fixed Purchase Shipped Count</summary>
	[JsonProperty(PropertyName = "fixed_purchase_shipped_count")]
	public int FixedPurchaseShippedCount { get; set; }
	/// <summary>Fixed Purchase Discount Price</summary>
	[JsonProperty(PropertyName = "fixed_purchase_discount_price")]
	public decimal FixedPurchaseDiscountPrice { get; set; }
	/// <summary>Fixed Purchase Member Discount Amount</summary>
	[JsonProperty(PropertyName = "fixed_purchase_member_discount_amount")]
	public decimal FixedPurchaseMemberDiscountAmount { get; set; }
	/// <summary>Order Price Subtotal Tax</summary>
	[JsonProperty(PropertyName = "order_price_subtotal_tax")]
	public decimal OrderPriceSubtotalTax { get; set; }
	/// <summary>Order Price ShippingT ax</summary>
	[JsonProperty(PropertyName = "order_price_shipping_tax")]
	public decimal OrderPriceShippingTax { get; set; }
	/// <summary>Order Price Exchange Tax</summary>
	[JsonProperty(PropertyName = "order_price_exchange_tax")]
	public decimal OrderPriceExchangeTax { get; set; }
	/// <summary>Scheduled Shipping Date</summary>
	[JsonProperty(PropertyName = "scheduled_shipping_date")]
	public string ScheduledShippingDate { get; set; }
	/// <summary>Shipping Date</summary>
	[JsonProperty(PropertyName = "shipping_date")]
	public string ShippingDate { get; set; }
	/// <summary>Shipping Time</summary>
	[JsonProperty(PropertyName = "shipping_time")]
	public string ShippingTime { get; set; }
	/// <summary>Shipping Check No</summary>
	[JsonProperty(PropertyName = "shipping_check_no")]
	public string ShippingCheckNo { get; set; }
	/// <summary>Date Created</summary>
	[JsonProperty(PropertyName = "date_created")]
	public string DateCreated { get; set; }
	/// <summary>Date Changed</summary>
	[JsonProperty(PropertyName = "date_changed")]
	public string DateChanged { get; set; }
	/// <summary>Status</summary>
	[JsonProperty(PropertyName = "status")]
	public int? Status { get; set; }
	/// <summary>商品情報明細</summary>
	[JsonProperty(PropertyName = "detail_product")]
	public DetailProduct[] DetailProduct { get; set; }
	/// <summary>配送情報明細</summary>
	[JsonProperty(PropertyName = "detail_shipping")]
	public DetailShipping[] DetailShipping { get; set; }
}

/// <summary>
/// 商品情報配列
/// </summary>
[Serializable]
public class DetailProduct
{
	/// <summary>Order Item No</summary>
	[JsonProperty(PropertyName = "order_item_no")]
	public int OrderItemNo { get; set; }
	/// <summary>配送先枝番</summary>
	[JsonProperty(PropertyName = "order_shipping_no")]
	public int OrderShippingNo { get; set; }
	/// <summary>Product Id</summary>
	[JsonProperty(PropertyName = "product_id")]
	public string ProductId { get; set; }
	/// <summary>Variation Id</summary>
	[JsonProperty(PropertyName = "variation_id")]
	public string VariationId { get; set; }
	/// <summary>Product Name</summary>
	[JsonProperty(PropertyName = "product_name")]
	public string ProductName { get; set; }
	/// <summary>Product Price</summary>
	[JsonProperty(PropertyName = "product_price")]
	public decimal ProductPrice { get; set; }
	/// <summary>Product Price Pretax</summary>
	[JsonProperty(PropertyName = "product_price_pretax")]
	public decimal ProductPricePretax { get; set; }
	/// <summary>Item Price Tax</summary>
	[JsonProperty(PropertyName = "item_price_tax")]
	public decimal ItemPriceTax { get; set; }
	/// <summary>Item Quantity</summary>
	[JsonProperty(PropertyName = "item_quantity")]
	public decimal ItemQuantity { get; set; }
	/// <summary>Item Price</summary>
	[JsonProperty(PropertyName = "item_price")]
	public decimal ItemPrice { get; set; }
	/// <summary>Fixed Purchase Product Flg</summary>
	[JsonProperty(PropertyName = "fixed_purchase_product_flg")]
	public string FixedPurchaseProductFlg { get; set; }
}

/// <summary>
/// 配送情報配列
/// </summary>
[Serializable]
public class DetailShipping
{
	/// <summary>注文番号</summary>
	[JsonProperty(PropertyName = "order_id")]
	public string OrderId { get; set; }
	/// <summary>配送先枝番</summary>
	[JsonProperty(PropertyName = "order_shipping_no")]
	public int OrderShippingNo { get; set; }
	/// <summary>配送種別名</summary>
	[JsonProperty(PropertyName = "shipping_method")]
	public string ShippingMethod { get; set; }
	/// <summary>配送会社ID</summary>
	[JsonProperty(PropertyName = "delivery_company_id")]
	public string DeliveryCompanyId { get; set; }
}