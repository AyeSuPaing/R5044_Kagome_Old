/*
=========================================================================================================
  Module      : Rakuten Authorize Request(RakutenAuthorizeRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;

namespace w2.App.Common.Order.Payment.Rakuten.Authorize
{
	/// <summary>
	/// Rakuten Authorize Request
	/// </summary>
	[Serializable]
	public class RakutenAuthorizeRequest : RakutenRequestBase
	{
		/// <summary>
		/// Construction
		/// </summary>
		/// <param name="ipAddress">クライアントIPアドレス</param>
		public RakutenAuthorizeRequest(string ipAddress = "")
		{
			this.AgencyCode = RakutenConstants.AGENCY_CODE_DEFAULT;
			this.CurrencyCode = RakutenConstants.CURRENCY_CODE_DEFAULT;
			this.Order = new Order(ipAddress);
		}

		/// <summary>Service Reference Id</summary>
		[JsonProperty(PropertyName = "serviceReferenceId")]
		public string ServiceReferenceId { get; set; }
		/// <summary>Agency Code</summary>
		[JsonProperty(PropertyName = "agencyCode")]
		public string AgencyCode { get; set; }
		/// <summary>Item Name</summary>
		[JsonProperty(PropertyName = "custom")]
		public Custom Custom { get; set; }
		/// <summary>Currency Code</summary>
		[JsonProperty(PropertyName = "currencyCode")]
		public string CurrencyCode { get; set; }
		/// <summary>Gross Amount</summary>
		[JsonProperty(PropertyName = "grossAmount")]
		public decimal GrossAmount { get; set; }
		/// <summary>Notification Url</summary>
		[JsonProperty(PropertyName = "notificationUrl")]
		public string NotificationUrl { get; set; }
		/// <summary>Order</summary>
		[JsonProperty(PropertyName = "order")]
		public Order Order { get; set; }
		/// <summary>Card Token</summary>
		[JsonProperty(PropertyName = "cardToken")]
		public CardTokenBase CardToken { get; set; }
		/// <summary>Cvs payment</summary>
		[JsonProperty(PropertyName = "cvsPayment")]
		public CvsPayment CvsPayment { get; set; }
	}

	/// <summary>
	/// Custom
	/// </summary>
	[JsonObject("custom")]
	public class Custom
	{
		/// <summary>Choose Payment</summary>
		[JsonProperty(PropertyName = "japanBillingAddress")]
		public JapanBillingAddress JapanBillingAddress { get; set; }
	}

	/// <summary>
	/// Japan Billing Address
	/// </summary>
	[JsonObject("japanBillingAddress")]
	public class JapanBillingAddress
	{
		/// <summary>First Name Kana</summary>
		[JsonProperty(PropertyName = "firstNameKana")]
		public string FirstNameKana { get; set; }
		/// <summary>Last Name Kana</summary>
		[JsonProperty(PropertyName = "lastNameKana")]
		public string LastNameKana { get; set; }
		/// <summary>Postal Code</summary>
		[JsonProperty(PropertyName = "postalCode")]
		public int PostalCode { get; set; }
	}

	/// <summary>
	/// Order
	/// </summary>
	[JsonObject("order")]
	public class Order
	{
		/// <summary>
		/// Construction
		/// </summary>
		/// <param name="ipAddress">クライアントIPアドレス</param>
		/// <param name="isPaymentCvs">Is payment cvs</param>
		public Order(string ipAddress, bool isPaymentCvs = false)
		{
			this.Version = RakutenConstants.VERSION_ORDER_DEFAULT;
			this.Email = isPaymentCvs
				? RakutenConstants.CVS_EMAIL_DEFAULT
				: RakutenConstants.EMAIL_DEFAULT;
			this.IpAddress = (string.IsNullOrEmpty(ipAddress) == false) ? ipAddress : RakutenConstants.IPADDRESS_DEFAULT;
		}
		/// <summary>Version</summary>
		[JsonProperty(PropertyName = "version")]
		public int Version { get; set; }
		/// <summary>Email</summary>
		[JsonProperty(PropertyName = "email")]
		public string Email { get; set; }
		/// <summary>IpAddress</summary>
		[JsonProperty(PropertyName = "ipAddress")]
		public string IpAddress { get; set; }
		/// <summary>Customer</summary>
		[JsonProperty(PropertyName = "customer")]
		public Customer Customer { get; set; }
		/// <summary>Items</summary>
		[JsonProperty(PropertyName = "items")]
		public Items[] Items { get; set; }
	}

	/// <summary>
	/// Card Token
	/// </summary>
	[JsonObject("cardToken")]
	public class CardTokenBase
	{
		/// <summary>
		/// Construction
		/// </summary>
		public CardTokenBase()
		{
			this.Version = RakutenConstants.VERSION_CARD_TOKEN_DEFAULT;
		}
		/// <summary>Version</summary>
		[JsonProperty(PropertyName = "version")]
		public int Version { get; set; }
		/// <summary>Amount</summary>
		[JsonProperty(PropertyName = "amount")]
		public string Amount { get; set; }
		/// <summary>Card Token</summary>
		[JsonProperty(PropertyName = "cardToken")]
		public string CardToken { get; set; }
		/// <summary>Cvv Token</summary>
		[JsonProperty(PropertyName = "cvvToken")]
		public string CvvToken { get; set; }
		/// <summary>With Three DSecure</summary>
		[JsonProperty(PropertyName = "withThreeDSecure")]
		public bool WithThreeDSecure { get; set; }
		/// <summary>Installments</summary>
		[JsonProperty(PropertyName = "installments", NullValueHandling = NullValueHandling.Ignore)]
		public string Installments { get; set; }
		/// <summary>With Bonus</summary>
		[JsonProperty(PropertyName = "withBonus")]
		public bool WithBonus { get; set; }
		/// <summary>With Revolving</summary>
		[JsonProperty(PropertyName = "withRevolving")]
		public bool WithRevolving { get; set; }
	}

	/// <summary>
	/// Items
	/// </summary>
	[JsonObject("items")]
	public class Items
	{
		/// <summary>Id</summary>
		[JsonProperty(PropertyName = "id")]
		public string Id { get; set; }
		/// <summary>Name</summary>
		[JsonProperty(PropertyName = "name")]
		public string Name { get; set; }
		/// <summary>Price</summary>
		[JsonProperty(PropertyName = "price")]
		public decimal Price { get; set; }
		/// <summary>Quantity</summary>
		[JsonProperty(PropertyName = "quantity")]
		public int Quantity { get; set; }
	}

	/// <summary>
	/// Cvs payment
	/// </summary>
	[JsonObject("cvsPayment")]
	public class CvsPayment
	{
		/// <summary>Version</summary>
		[JsonProperty(PropertyName = "version")]
		public string Version { get; set; }
		/// <summary>Amount</summary>
		[JsonProperty(PropertyName = "amount")]
		public decimal Amount { get; set; }
		/// <summary>Order date</summary>
		[JsonProperty(PropertyName = "orderDate")]
		public int OrderDate { get; set; }
		/// <summary>Expiration date</summary>
		[JsonProperty(PropertyName = "expirationDate")]
		public int ExpirationDate { get; set; }
		/// <summary>Add handling fee</summary>
		[JsonProperty(PropertyName = "addHandlingFee")]
		public bool AddHandlingFee { get; set; }
	}

	/// <summary>
	/// Customer
	/// </summary>
	[JsonObject("customer")]
	public class Customer
	{
		/// <summary>First name</summary>
		[JsonProperty(PropertyName = "firstName")]
		public string FirstName { get; set; }
		/// <summary>Last name</summary>
		[JsonProperty(PropertyName = "lastName")]
		public string LastName { get; set; }
		/// <summary>Phone</summary>
		[JsonProperty(PropertyName = "phone")]
		public string Phone { get; set; }
	}
}
