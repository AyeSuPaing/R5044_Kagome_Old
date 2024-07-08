/*
=========================================================================================================
  Module      : Line Pay Request Payment(LinePayRequestPayment.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;

namespace w2.App.Common.Order.Payment.LinePay
{
	/// <summary>
	/// LINE Pay Request Payment
	/// </summary>
	[Serializable]
	public class LinePayRequestPayment
	{
		/// <summary>Product Name</summary>
		[JsonProperty(PropertyName = "productName")]
		public string ProductName { get; set; }
		/// <summary>Amount</summary>
		[JsonProperty(PropertyName = "amount")]
		public int Amount { get; set; }
		/// <summary>Currency</summary>
		[JsonProperty(PropertyName = "currency")]
		public string Currency { get; set; }
		/// <summary>決済注文ID（LinePayのorderId）</summary>
		[JsonProperty(PropertyName = "orderId")]
		public string PaymentOrderId { get; set; }
		/// <summary>Packages</summary>
		[JsonProperty(PropertyName = "packages")]
		public Package[] Packages { get; set; }
		/// <summary>Redirect Urls</summary>
		[JsonProperty(PropertyName = "redirectUrls")]
		public RedirectUrls RedirectUrls { get; set; }
		/// <summary>Options</summary>
		[JsonProperty(PropertyName = "options")]
		public Options Options { get; set; }
	}

	/// <summary>
	/// Package
	/// </summary>
	[JsonObject("packages")]
	public class Package
	{
		/// <summary>Id</summary>
		[JsonProperty(PropertyName = "id")]
		public string Id { get; set; }
		/// <summary>Amount</summary>
		[JsonProperty(PropertyName = "amount")]
		public int Amount { get; set; }
		/// <summary>Products</summary>
		[JsonProperty(PropertyName = "products")]
		public Product[] Products { get; set; }
	}

	/// <summary>
	/// Product
	/// </summary>
	[JsonObject("products")]
	public class Product
	{
		/// <summary>Id</summary>
		[JsonProperty(PropertyName = "id")]
		public string Id { get; set; }
		/// <summary>Name</summary>
		[JsonProperty(PropertyName = "name")]
		public string Name { get; set; }
		/// <summary>Image Url</summary>
		[JsonProperty(PropertyName = "imageUrl")]
		public string ImageUrl { get; set; }
		/// <summary>Quantity</summary>
		[JsonProperty(PropertyName = "quantity")]
		public int Quantity { get; set; }
		/// <summary>Price</summary>
		[JsonProperty(PropertyName = "price")]
		public int Price { get; set; }
	}

	/// <summary>
	/// Redirect Urls
	/// </summary>
	[JsonObject("redirectUrls")]
	public class RedirectUrls
	{
		/// <summary>Confirm Url</summary>
		[JsonProperty(PropertyName = "confirmUrl")]
		public string ConfirmUrl { get; set; }
		/// <summary>Cancel Url</summary>
		[JsonProperty(PropertyName = "cancelUrl")]
		public string CancelUrl { get; set; }
		/// <summary>ConfirmUrlType</summary>
		[JsonProperty(PropertyName = "confirmUrlType")]
		public string ConfirmUrlType { get; set; }
	}

	/// <summary>
	/// Options
	/// </summary>
	[JsonObject("options")]
	public class Options
	{
		/// <summary>Shipping</summary>
		[JsonProperty(PropertyName = "shipping")]
		public Shipping Shipping { get; set; }
		/// <summary>Payment</summary>
		[JsonProperty(PropertyName = "payment")]
		public Payment Payment { get; set; }
		/// <summary>Display</summary>
		[JsonProperty(PropertyName = "display")]
		public Display Display { get; set; }
	}

	/// <summary>
	/// Shipping
	/// </summary>
	[JsonObject("shipping")]
	public class Shipping
	{
		/// <summary>Type</summary>
		[JsonProperty(PropertyName = "type")]
		public string Type { get; set; }
		/// <summary>Fee Amount</summary>
		[JsonProperty(PropertyName = "feeAmount")]
		public int FeeAmount { get; set; }
		/// <summary>Fee Inquiry Type</summary>
		[JsonProperty(PropertyName = "feeInquiryType")]
		public string FeeInquiryType { get; set; }
		/// <summary>Address</summary>
		[JsonProperty(PropertyName = "address")]
		public Address Address { get; set; }
	}

	/// <summary>
	/// Address
	/// </summary>
	[JsonObject("address")]
	public class Address
	{
		/// <summary>Country</summary>
		[JsonProperty(PropertyName = "country")]
		public string Country { get; set; }
		/// <summary>Postal Code</summary>
		[JsonProperty(PropertyName = "postalCode")]
		public string PostalCode { get; set; }
		/// <summary>State</summary>
		[JsonProperty(PropertyName = "state")]
		public string State { get; set; }
		/// <summary>City</summary>
		[JsonProperty(PropertyName = "city")]
		public string City { get; set; }
		/// <summary>Detail</summary>
		[JsonProperty(PropertyName = "detail")]
		public string Detail { get; set; }
		/// <summary>Optional</summary>
		[JsonProperty(PropertyName = "optional")]
		public string Optional { get; set; }
		/// <summary>Recipient</summary>
		[JsonProperty(PropertyName = "recipient")]
		public Recipient Recipient { get; set; }
	}

	/// <summary>
	/// Recipient
	/// </summary>
	[JsonObject("recipient")]
	public class Recipient
	{
		/// <summary>First Name</summary>
		[JsonProperty(PropertyName = "firstName")]
		public string FirstName { get; set; }
		/// <summary>Last Name</summary>
		[JsonProperty(PropertyName = "lastName")]
		public string LastName { get; set; }
		/// <summary>First Name Optional</summary>
		[JsonProperty(PropertyName = "firstNameOptional")]
		public string FirstNameOptional { get; set; }
		/// <summary>Last Name Optional</summary>
		[JsonProperty(PropertyName = "lastNameOptional")]
		public string LastNameOptional { get; set; }
		/// <summary>Email</summary>
		[JsonProperty(PropertyName = "email")]
		public string Email { get; set; }
		/// <summary>Phone No</summary>
		[JsonProperty(PropertyName = "phoneNo")]
		public string PhoneNo { get; set; }
	}

	/// <summary>
	/// Payment
	/// </summary>
	[JsonObject("payment")]
	public class Payment
	{
		/// <summary>Capture</summary>
		[JsonProperty(PropertyName = "capture")]
		public bool Capture { get; set; }
		/// <summary>Pay Type</summary>
		[JsonProperty(PropertyName = "payType")]
		public string PayType { get; set; }
	}

	/// <summary>
	/// Display
	/// </summary>
	[JsonObject("display")]
	public class Display
	{
		/// <summary>CheckConfirmUrlBrowser</summary>
		[JsonProperty(PropertyName = "checkConfirmUrlBrowser")]
		public bool CheckConfirmUrlBrowser { get; set; }
	}
}
