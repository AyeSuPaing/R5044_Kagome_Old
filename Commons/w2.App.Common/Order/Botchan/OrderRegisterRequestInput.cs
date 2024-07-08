/*
=========================================================================================================
  Module      : Order Register Request Input(OrderRegisterRequestInput.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;

namespace w2.App.Common.Order.Botchan
{
	/// <summary>
	/// Order register request object
	/// </summary>
	[Serializable]
	public class OrderRegisterRequestInput
	{
		/// <summary>Cart object</summary>
		[JsonProperty("cart")]
		public Cart CartObject { get; set; }
		/// <summary>Order shipping info</summary>
		[JsonProperty("order_shipping_info")]
		public OrderShippingInfo OrderShippingObject { get; set; }
		/// <summary>Order payment info</summary>
		[JsonProperty("order_payment_info")]
		public OrderPaymentInfo OrderPaymentObject { get; set; }
		/// <summary>Discount info object</summary>
		[JsonProperty("discount_info")]
		public DiscountInfo DiscountInfoObject { get; set; }
		/// <summary>Order product list</summary>
		[JsonProperty("order_product_list")]
		public OrderProductList[] OrderProductLists { get; set; }
		/// <summary>Authentication key</summary>
		[JsonProperty("auth_text")]
		public string AuthText { get; set; }

		/// <summary>
		/// Cart object
		/// </summary>
		[Serializable]
		public class Cart
		{
			/// <summary>Cart id</summary>
			[JsonProperty("cart_id")]
			public string CartId { get; set; }
			/// <summary>User id</summary>
			[JsonProperty("user_id")]
			public string UserId { get; set; }
			/// <summary>Order division</summary>
			[JsonProperty("order_division")]
			public string OrderDivision { get; set; }
			/// <summary>Product option texts</summary>
			[JsonProperty("product_option_texts")]
			public string ProductOptionTexts { get; set; }
			/// <summary>Add novelty flag</summary>
			[JsonProperty("add_novelty_flag")]
			public string AddNoveltyFlag { get; set; }
		}

		/// <summary>
		/// Order shipping info object
		/// </summary>
		[Serializable]
		public class OrderShippingInfo
		{
			/// <summary>Name</summary>
			[JsonProperty("name")]
			public string Name { get; set; }
			/// <summary>Shipping name</summary>
			[JsonProperty("shipping_name")]
			public string ShippingName { get; set; }
			/// <summary>Shipping name kana</summary>
			[JsonProperty("shipping_name_kana")]
			public string ShippingNameKana { get; set; }
			/// <summary>Shipping zip</summary>
			[JsonProperty("shipping_zip")]
			public string ShippingZip { get; set; }
			/// <summary>Shipping address 1</summary>
			[JsonProperty("shipping_addr1")]
			public string ShippingAddr1 { get; set; }
			/// <summary>Shipping address 2</summary>
			[JsonProperty("shipping_addr2")]
			public string ShippingAddr2 { get; set; }
			/// <summary>Shipping address 3</summary>
			[JsonProperty("shipping_addr3")]
			public string ShippingAddr3 { get; set; }
			/// <summary>Shipping address 4</summary>
			[JsonProperty("shipping_addr4")]
			public string ShippingAddr4 { get; set; }
			/// <summary>Shipping phone number 1</summary>
			[JsonProperty("shipping_tel1")]
			public string ShippingTel1 { get; set; }
			/// <summary>Shipping company name</summary>
			[JsonProperty("shipping_company_name")]
			public string ShippingCompanyName { get; set; }
			/// <summary>Shipping company post name</summary>
			[JsonProperty("shipping_company_post_name")]
			public string ShippingCompanyPostName { get; set; }
			/// <summary>Fixed purchase kbn</summary>
			[JsonProperty("fixed_purchase_kbn")]
			public string FixedPurchaseKbn { get; set; }
			/// <summary>Course buy setting</summary>
			[JsonProperty("course_buy_setting")]
			public string CourseBuySetting { get; set; }
		}

		/// <summary>
		/// Order payment info
		/// </summary>
		[Serializable]
		public class OrderPaymentInfo
		{
			/// <summary>Payment id</summary>
			[JsonProperty("payment_id")]
			public string PaymentId { get; set; }
			/// <summary>Receipt flag</summary>
			[JsonProperty("receipt_flg")]
			public string ReceiptFlg { get; set; }
			/// <summary>Card disp name</summary>
			[JsonProperty("card_disp_name")]
			public string CardDispName { get; set; }
			/// <summary>Credit token</summary>
			[JsonProperty("credit_token")]
			public string CreditToken { get; set; }
			/// <summary>Credit card no</summary>
			[JsonProperty("credit_card_no")]
			public string CreditCardNo { get; set; }
			/// <summary>Expiration month</summary>
			[JsonProperty("expiration_month")]
			public string ExpirationMonth { get; set; }
			/// <summary>Expiration year</summary>
			[JsonProperty("expiration_year")]
			public string ExpirationYear { get; set; }
			/// <summary>Author name</summary>
			[JsonProperty("author_name")]
			public string AuthorName { get; set; }
			/// <summary>Credit security code</summary>
			[JsonProperty("credit_security_code")]
			public string CreditSecurityCode { get; set; }
			/// <summary>Credit installments</summary>
			[JsonProperty("credit_installments")]
			public string CreditInstallments { get; set; }
		}

		/// <summary>
		/// Discount info object
		/// </summary>
		[Serializable]
		public class DiscountInfo
		{
			/// <summary>Order point use</summary>
			[JsonProperty("order_point_use")]
			public string OrderPointUse { get; set; }
			/// <summary>Coupon code</summary>
			[JsonProperty("coupon_code")]
			public string CouponCode { get; set; }
			/// <summary>Gift flag</summary>
			[JsonProperty("gift_flg")]
			public string GiftFlg { get; set; }
		}

		/// <summary>
		/// Order product list object
		/// </summary>
		[Serializable]
		public class OrderProductList
		{
			/// <summary>Product id</summary>
			[JsonProperty("product_id")]
			public string ProductId { get; set; }
			/// <summary>Variation id</summary>
			[JsonProperty("variation_id")]
			public string VariationId { get; set; }
			/// <summary>Product count</summary>
			[JsonProperty("product_count")]
			public string ProductCount { get; set; }
			/// <summary>Recommend flag</summary>
			[JsonProperty("recommend_flag")]
			public string RecommendFlag { get; set; }
		}
	}
}
