/*
=========================================================================================================
  Module      : 再計算APIレスポンス(RecalculationResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace w2.App.Common.Order.Botchan
{
	/// <summary>
	/// Recalculation Response
	/// </summary>
	[Serializable]
	public class RecalculationResponse
	{
		/// <summary>Result</summary>
		[JsonProperty("result")]
		public bool Result { get; set; }
		/// <summary>Message id</summary>
		[JsonProperty("message_id")]
		public string MessageId { get; set; }
		/// <summary>Message</summary>
		[JsonProperty("message")]
		public string Message { get; set; }
		/// <summary>再計算結果レスポンス</summary>
		[JsonProperty("data")]
		public Data DataResult { get; set; }

		/// <summary>
		/// 再計算結果レスポンス
		/// </summary>
		[Serializable]
		public class Data
		{
			/// <summary>Cart object</summary>
			[JsonProperty("cart")]
			public Cart CartObject { get; set; }
			/// <summary>Receive order info</summary>
			[JsonProperty("receive_order_info")]
			public ReceiveOrderInfo ReceiveOrderInfoObject { get; set; }
			/// <summary>Credit card info</summary>
			[JsonProperty("credit_info")]
			public CreditCardInfo CreditCardInfoObject { get; set; }
			/// <summary>Order product list</summary>
			[JsonProperty("order_product_list")]
			public OrderProductList[] OrderProducts { get; set; }
			/// <summary>Recommend products</summary>
			[JsonProperty("recommend_originally_product_list")]
			public RecommendOriginallyProductList[] RecommendOriginallyProducts { get; set; }
			/// <summary>指定可能配送希望日リスト</summary>
			[JsonProperty("shipping_date_list")]
			public List<string> ShippingDateList { get; set; }
			/// <summary>指定可能配送時間帯リスト</summary>
			[JsonProperty("shipping_time_list")]
			public ShippingTimeList[] ShippingTimes { get; set; }
			
		}

		/// <summary>
		/// Cart object
		/// </summary>
		[Serializable]
		public class Cart
		{
			/// <summary>Cart id</summary>
			[DefaultValue("")]
			[JsonProperty("cart_id")]
			public string CartId { get; set; }
			/// <summary>User id</summary>
			[DefaultValue("")]
			[JsonProperty("user_id")]
			public string UserId { get; set; }
			/// <summary>Shop id</summary>
			[DefaultValue("")]
			[JsonProperty("shop_id")]
			public string ShopId { get; set; }
			/// <summary>Supplier id</summary>
			[DefaultValue("")]
			[JsonProperty("supplier_id")]
			public string SupplierId { get; set; }
			/// <summary>Fixed purchase flg</summary>
			[DefaultValue("")]
			[JsonProperty("fixed_purchase_flg")]
			public string FixedPurchaseFlg { get; set; }
		}

		/// <summary>
		/// Receive order info
		/// </summary>
		[Serializable]
		public class ReceiveOrderInfo
		{
			/// <summary>Order price subtotal</summary>
			[DefaultValue("")]
			[JsonProperty("order_price_subtotal")]
			public string OrderPriceSubtotal { get; set; }
			/// <summary>Order price tax</summary>
			[DefaultValue("")]
			[JsonProperty("order_price_tax")]
			public string OrderPriceTax { get; set; }
			/// <summary>Order price shipping</summary>
			[DefaultValue("")]
			[JsonProperty("order_price_shipping")]
			public string OrderPriceShipping { get; set; }
			/// <summary>Order price exchange</summary>
			[DefaultValue("")]
			[JsonProperty("order_price_exchange")]
			public string OrderPriceExchange { get; set; }
			/// <summary>Order price regulation</summary>
			[DefaultValue("")]
			[JsonProperty("order_price_regulation")]
			public string OrderPriceRegulation { get; set; }
			/// <summary>Order price repayment</summary>
			[DefaultValue("")]
			[JsonProperty("order_price_repayment")]
			public string OrderPriceRepayment { get; set; }
			/// <summary>Order price total</summary>
			[DefaultValue("")]
			[JsonProperty("order_price_total")]
			public string OrderPriceTotal { get; set; }
			/// <summary>Order discount set_price</summary>
			[DefaultValue("")]
			[JsonProperty("order_discount_set_price")]
			public string OrderDiscountSetPrice { get; set; }
			/// <summary>Order point use</summary>
			[DefaultValue("")]
			[JsonProperty("order_point_use")]
			public string OrderPointUse { get; set; }
			/// <summary>Order point use yen</summary>
			[DefaultValue("")]
			[JsonProperty("order_point_use_yen")]
			public string OrderPointUseYen { get; set; }
			/// <summary>Order point add</summary>
			[DefaultValue("")]
			[JsonProperty("order_point_add")]
			public string OrderPointAdd { get; set; }
			/// <summary>Order coupon use</summary>
			[DefaultValue("")]
			[JsonProperty("order_coupon_use")]
			public string OrderCouponUse { get; set; }
			/// <summary>Card kbn</summary>
			[DefaultValue("")]
			[JsonProperty("card_kbn")]
			public string CardKbn { get; set; }
			/// <summary>Card instruments</summary>
			[DefaultValue("")]
			[JsonProperty("card_instruments")]
			public string CardInstruments { get; set; }
			/// <summary>Card tran id</summary>
			[DefaultValue("")]
			[JsonProperty("card_tran_id")]
			public string CardTranId { get; set; }
			/// <summary>Shipping id</summary>
			[DefaultValue("")]
			[JsonProperty("shipping_id")]
			public string ShippingId { get; set; }
			/// <summary>Fixed purchase id</summary>
			[DefaultValue("")]
			[JsonProperty("fixed_purchase_id")]
			public string FixedPurchaseId { get; set; }
			/// <summary>Member rank discount price</summary>
			[DefaultValue("")]
			[JsonProperty("member_rank_discount_price")]
			public string MemberRankDiscountPrice { get; set; }
			/// <summary>Member rank id</summary>
			[DefaultValue("")]
			[JsonProperty("member_rank_id")]
			public string MemberRankId { get; set; }
			/// <summary>Digital contents flg</summary>
			[DefaultValue("")]
			[JsonProperty("digital_contents_flg")]
			public string DigitalContentsFlg { get; set; }
			/// <summary>Card installments code</summary>
			[DefaultValue("")]
			[JsonProperty("card_installments_code")]
			public string CardInstallmentsCode { get; set; }
			/// <summary>Setpromotion product discount amount</summary>
			[DefaultValue("")]
			[JsonProperty("setpromotion_product_discount_amount")]
			public string SetpromotionProductDiscountAmount { get; set; }
			/// <summary>Setpromotion shipping charge discount amount</summary>
			[DefaultValue("")]
			[JsonProperty("setpromotion_shipping_charge_discount_amount")]
			public string SetpromotionShippingChargeDiscountAmount { get; set; }
			/// <summary>Setpromotion payment charge discount amount</summary>
			[DefaultValue("")]
			[JsonProperty("setpromotion_payment_charge_discount_amount")]
			public string SetpromotionPaymentChargeDiscountAmount { get; set; }
			/// <summary>Online payment status</summary>
			[DefaultValue("")]
			[JsonProperty("online_payment_status")]
			public string OnlinePaymentStatus { get; set; }
			/// <summary>Fixed purchase order count</summary>
			[DefaultValue("")]
			[JsonProperty("fixed_purchase_order_count")]
			public string FixedPurchaseOrderCount { get; set; }
			/// <summary>Fixed purchase shipped count</summary>
			[DefaultValue("")]
			[JsonProperty("fixed_purchase_shipped_count")]
			public string FixedPurchaseShippedCount { get; set; }
			/// <summary>Fixed purchase discount price</summary>
			[DefaultValue("")]
			[JsonProperty("fixed_purchase_discount_price")]
			public string FixedPurchaseDiscountPrice { get; set; }
			/// <summary>Payment order id</summary>
			[DefaultValue("")]
			[JsonProperty("payment_order_id")]
			public string PaymentOrderId { get; set; }
			/// <summary>Fixed purchase member discount amount</summary>
			[DefaultValue("")]
			[JsonProperty("fixed_purchase_member_discount_amount")]
			public string FixedPurchaseMemberDiscountAmount { get; set; }
			/// <summary>Last billed amount</summary>
			[DefaultValue("")]
			[JsonProperty("last_billed_amount")]
			public string LastBilledAmount { get; set; }
			/// <summary>External payment status</summary>
			[DefaultValue("")]
			[JsonProperty("external_payment_status")]
			public string ExternalPaymentStatus { get; set; }
			/// <summary>ExternalPaymentErrorMessage</summary>
			[DefaultValue("")]
			[JsonProperty("external_payment_error_message")]
			public string ExternalPaymentErrorMessage { get; set; }
			/// <summary>External payment auth date</summary>
			[DefaultValue("")]
			[JsonProperty("external_payment_auth_date")]
			public string ExternalPaymentAuthDate { get; set; }
			/// <summary>Order price subtotal tax</summary>
			[DefaultValue("")]
			[JsonProperty("order_price_subtotal_tax")]
			public string OrderPriceSubtotalTax { get; set; }
			/// <summary>Order price shipping tax</summary>
			[DefaultValue("")]
			[JsonProperty("order_price_shipping_tax")]
			public string OrderPriceShippingTax { get; set; }
			/// <summary>Order price exchange tax</summary>
			[DefaultValue("")]
			[JsonProperty("order_price_exchange_tax")]
			public string OrderPriceExchangeTax { get; set; }
			/// <summary>Shipping memo</summary>
			[DefaultValue("")]
			[JsonProperty("shipping_memo")]
			public string ShippingMemo { get; set; }
			/// <summary>External payment cooperation log</summary>
			[DefaultValue("")]
			[JsonProperty("external_payment_cooperation_log")]
			public string ExternalPaymentCooperationLog { get; set; }
			/// <summary>Shipping tax rate</summary>
			[DefaultValue("")]
			[JsonProperty("shipping_tax_rate")]
			public string ShippingTaxRate { get; set; }
			/// <summary>Payment tax rate</summary>
			[DefaultValue("")]
			[JsonProperty("payment_tax_rate")]
			public string PaymentTaxRate { get; set; }
			/// <summary>Order count order</summary>
			[DefaultValue("")]
			[JsonProperty("order_count_order")]
			public string OrderCountOrder { get; set; }
			/// <summary>Invoice bundle flg</summary>
			[DefaultValue("")]
			[JsonProperty("invoice_bundle_flg")]
			public string InvoiceBundleFlg { get; set; }
			/// <summary>Receipt flg</summary>
			[DefaultValue("")]
			[JsonProperty("receipt_flg")]
			public string ReceiptFlg { get; set; }
			/// <summary>Receipt output flg</summary>
			[DefaultValue("")]
			[JsonProperty("receipt_output_flg")]
			public string ReceiptOutputFlg { get; set; }
			/// <summary>Receipt address</summary>
			[DefaultValue("")]
			[JsonProperty("receipt_address")]
			public string ReceiptAddress { get; set; }
			/// <summary>Receipt proviso</summary>
			[DefaultValue("")]
			[JsonProperty("receipt_proviso")]
			public string ReceiptProviso { get; set; }
			/// <summary>定期購入区分</summary>
			[DefaultValue("")]
			[JsonProperty("fixed_purchase_kbn")]
			public string FixedPurchaseKbn { get; set; }
			/// <summary>定期購入設定</summary>
			[DefaultValue("")]
			[JsonProperty("fixed_purchase_setting")]
			public string FixedPurchaseSetting { get; set; }
		}

		/// <summary>
		/// 指定可能配送時間帯リスト
		/// </summary>
		[Serializable]
		public class ShippingTimeList
		{
			/// <summary>配送希望時間帯ID</summary>
			[DefaultValue("")]
			[JsonProperty("shipping_time_id")]
			public string ShippingTimeId { get; set; }
			/// <summary>配送希望時間帯文言</summary>
			[DefaultValue("")]
			[JsonProperty("shipping_time_message")]
			public string ShippingTimeMessage { get; set; }
		}

		/// <summary>
		/// Credit Card Info
		/// </summary>
		[Serializable]
		public class CreditCardInfo
		{
			/// <summary>Card disp name</summary>
			[DefaultValue("")]
			[JsonProperty("card_disp_name")]
			public string CardDispName { get; set; }
			/// <summary>Last four digit</summary>
			[DefaultValue("")]
			[JsonProperty("last_four_digit")]
			public string LastFourDigit { get; set; }
			/// <summary>Expiration month</summary>
			[DefaultValue("")]
			[JsonProperty("expiration_month")]
			public string ExpirationMonth { get; set; }
			/// <summary>Expiration year</summary>
			[DefaultValue("")]
			[JsonProperty("expiration_year")]
			public string ExpirationYear { get; set; }
			/// <summary>Author name</summary>
			[DefaultValue("")]
			[JsonProperty("author_name")]
			public string AuthorName { get; set; }
			/// <summary>クレジットカード登録名補完フラグ</summary>
			[DefaultValue("")]
			[JsonProperty("credit_name_complement_flg")]
			public string CreditNameComplementFlg { get; set; }
		}

		/// <summary>
		/// Order product list object
		/// </summary>
		[Serializable]
		public class OrderProductList
		{
			/// <summary>Product id</summary>
			[DefaultValue("")]
			[JsonProperty("product_id")]
			public string ProductId { get; set; }
			/// <summary>Variation id</summary>
			[DefaultValue("")]
			[JsonProperty("variation_id")]
			public string VariationId { get; set; }
			/// <summary>Item quantity</summary>
			[DefaultValue("")]
			[JsonProperty("item_quantity")]
			public string ItemQuantity { get; set; }
			/// <summary>Product sale id</summary>
			[DefaultValue("")]
			[JsonProperty("product_sale_id")]
			public string ProductSaleId { get; set; }
			/// <summary>Product name</summary>
			[DefaultValue("")]
			[JsonProperty("product_name")]
			public string ProductName { get; set; }
			/// <summary>Item price</summary>
			[DefaultValue("")]
			[JsonProperty("item_price")]
			public string ItemPrice { get; set; }
			/// <summary>Product tax rate</summary>
			[DefaultValue("")]
			[JsonProperty("product_tax_rate")]
			public string ProductTaxRate { get; set; }
			/// <summary>定期商品か</summary>
			[DefaultValue("")]
			[JsonProperty("is_fixed_purchase")]
			public bool IsFixedPurchase { get; set; }
		}

		/// <summary>
		/// レコメンド処理前商品リスト
		/// </summary>
		[Serializable]
		public class RecommendOriginallyProductList
		{
			/// <summary>Product id</summary>
			[DefaultValue("")]
			[JsonProperty("product_id")]
			public string ProductId { get; set; }
			/// <summary>Variation id</summary>
			[DefaultValue("")]
			[JsonProperty("variation_id")]
			public string VariationId { get; set; }
			/// <summary>Item quantity</summary>
			[DefaultValue("")]
			[JsonProperty("item_quantity")]
			public string ItemQuantity { get; set; }
			/// <summary>Product sale id</summary>
			[DefaultValue("")]
			[JsonProperty("product_sale_id")]
			public string ProductSaleId { get; set; }
			/// <summary>Product name</summary>
			[DefaultValue("")]
			[JsonProperty("product_name")]
			public string ProductName { get; set; }
			/// <summary>Item price</summary>
			[DefaultValue("")]
			[JsonProperty("item_price")]
			public string ItemPrice { get; set; }
			/// <summary>Product tax rate</summary>
			[DefaultValue("")]
			[JsonProperty("product_tax_rate")]
			public string ProductTaxRate { get; set; }
			/// <summary>Recommend kbn</summary>
			[DefaultValue("")]
			[JsonProperty("recommend_kbn")]
			public string RecommendKbn { get; set; }
			/// <summary>Originally product id</summary>
			[DefaultValue("")]
			[JsonProperty("recommend_product_id")]
			public string RecommendProductId { get; set; }
		}
	}
}
