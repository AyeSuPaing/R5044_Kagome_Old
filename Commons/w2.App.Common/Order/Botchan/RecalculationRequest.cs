/*
=========================================================================================================
  Module      : Recalculation Request(RecalculationRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;

namespace w2.App.Common.Order.Botchan
{
	/// <summary>
	/// Recalculation Request
	/// </summary>
	[Serializable]
	public class RecalculationRequest
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
		/// <summary>注文者情報</summary>
		[JsonProperty("order_owner")]
		public OrderOwner OrderOwnerObject { get; set; }
		/// <summary>Auth text</summary>
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
			/// <summary>Order kbn</summary>
			[JsonProperty("order_kbn")]
			public string OrderKbn { get; set; }
			/// <summary>Add novelty flag</summary>
			[JsonProperty("add_novelty_flag")]
			public string AddNoveltyFlag { get; set; }
			/// <summary>Recommend flag</summary>
			[JsonProperty("recommend_flag")]
			public string RecommendFlag { get; set; }
		}

		/// <summary>
		/// Order shipping info
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
			/// <summary>Shipping name1</summary>
			[JsonProperty("shipping_name1")]
			public string ShippingName1 { get; set; }
			/// <summary>Shipping name2</summary>
			[JsonProperty("shipping_name2")]
			public string ShippingName2 { get; set; }
			/// <summary>Shipping name kana</summary>
			[JsonProperty("shipping_name_kana")]
			public string ShippingNameKana { get; set; }
			/// <summary>Shipping name kana1</summary>
			[JsonProperty("shipping_name_kana1")]
			public string ShippingNameKana1 { get; set; }
			/// <summary>Shipping name kana2</summary>
			[JsonProperty("shipping_name_kana2")]
			public string ShippingNameKana2 { get; set; }
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
			/// <summary>配送希望日</summary>
			[JsonProperty("shipping_date")]
			public string ShippingDate { get; set; }
			/// <summary>配送希望時間帯</summary>
			[JsonProperty("shipping_time")]
			public string ShippingTime { get; set; }
			/// <summary>配送方法</summary>
			[JsonProperty("shipping_method")]
			public string ShippingMethod { get; set; }
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
			/// <summary>クレカ枝番</summary>
			[JsonProperty("branch_no")]
			public int BranchNo { get; set; }
			/// <summary>Credit token</summary>
			[JsonProperty("credit_token")]
			public string CreditToken { get; set; }
			/// <summary>Credit card no</summary>
			[JsonProperty("credit_card_no")]
			public string CreditCardNo { get; set; }
			/// <summary>Expiration month</summary>
			[JsonProperty("expiration_month")]
			public int ExpirationMonth { get; set; }
			/// <summary>Expiration year</summary>
			[JsonProperty("expiration_year")]
			public int ExpirationYear { get; set; }
			/// <summary>Author name</summary>
			[JsonProperty("author_name")]
			public string AuthorName { get; set; }
			/// <summary>Credit security code</summary>
			[JsonProperty("credit_security_code")]
			public string CreditSecurityCode { get; set; }
			/// <summary>Credit installments</summary>
			[JsonProperty("credit_installments")]
			public string CreditInstallments { get; set; }
			/// <summary>クレジットカード登録フラグ</summary>
			[JsonProperty("credit_regist_flag")]
			public string CreditRegistrationFlag { get; set; }
			/// <summary>クレジットカード登録名</summary>
			[JsonProperty("credit_regist_name")]
			public string CreditRegistrationName { get; set; }
		}

		/// <summary>
		/// Object
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
			public int ProductCount { get; set; }
		}

		/// <summary>
		/// 注文者情報
		/// </summary>
		[Serializable]
		public class OrderOwner
		{
			/// <summary>氏名1</summary>
			[JsonProperty("name1")]
			public string Name1 { get; set; }
			/// <summary>氏名2</summary>
			[JsonProperty("name2")]
			public string Name2 { get; set; }
			/// <summary>氏名かな1</summary>
			[JsonProperty("name_kana1")]
			public string NameKana1 { get; set; }
			/// <summary>氏名かな2</summary>
			[JsonProperty("name_kana2")]
			public string NameKana2 { get; set; }
			/// <summary>生年月日</summary>
			[JsonProperty("birth")]
			public string Birth { get; set; }
			/// <summary>性別</summary>
			[JsonProperty("sex")]
			public string Sex { get; set; }
			/// <summary>メールアドレス</summary>
			[JsonProperty("mail_addr")]
			public string MailAddr { get; set; }
			/// <summary>郵便番号</summary>
			[JsonProperty("zip")]
			public string Zip { get; set; }
			/// <summary>住所1</summary>
			[JsonProperty("addr1")]
			public string Addr1 { get; set; }
			/// <summary>住所2</summary>
			[JsonProperty("addr2")]
			public string Addr2 { get; set; }
			/// <summary>住所3</summary>
			[JsonProperty("addr3")]
			public string Addr3 { get; set; }
			/// <summary>住所4</summary>
			[JsonProperty("addr4")]
			public string Addr4 { get; set; }
			/// <summary>電話番号1</summary>
			[JsonProperty("tel1")]
			public string Tel1 { get; set; }
		}
	}
}
