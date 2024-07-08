/*
=========================================================================================================
  Module      : Rakuten AuthorizeHtml Request(RakutenAuthorizeHtmlRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;

namespace w2.App.Common.Order.Payment.Rakuten.AuthorizeHtml
{
	/// <summary>
	/// AuthorizeHtmlリクエストパラメータ
	/// </summary>
	[Serializable]
	public class RakutenAuthorizeHtmlRequest : RakutenRequestBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="ipAddress">クライアントIPアドレス</param>
		public RakutenAuthorizeHtmlRequest(string ipAddress = "")
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
		public string GrossAmount { get; set; }
		/// <summary>Callback Url</summary>
		[JsonProperty(PropertyName = "callbackUrl")]
		public string CallbackUrl { get; set; }
		/// <summary>Notification Url</summary>
		[JsonProperty(PropertyName = "notificationUrl")]
		public string NotificationUrl { get; set; }
		/// <summary>Order</summary>
		[JsonProperty(PropertyName = "order")]
		public Order Order { get; set; }
		/// <summary>Card Token</summary>
		[JsonProperty(PropertyName = "cardToken")]
		public CardTokenBase CardToken { get; set; }
	}

	/// <summary>
	/// カスタム
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
	/// 注文
	/// </summary>
	[JsonObject("order")]
	public class Order
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="ipAddress">クライアントIPアドレス</param>
		public Order(string ipAddress)
		{
			this.Version = RakutenConstants.VERSION_ORDER_DEFAULT;
			this.Email = RakutenConstants.EMAIL_DEFAULT;
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
	}

	/// <summary>
	/// カードトークン情報
	/// </summary>
	[JsonObject("cardToken")]
	public class CardTokenBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CardTokenBase()
		{
			this.Version = RakutenConstants.VERSION_CARD_TOKEN_DEFAULT_THREE_D_SECURE;
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
		/// <summary>Installments</summary>
		[JsonProperty(PropertyName = "installments", NullValueHandling = NullValueHandling.Ignore)]
		public string Installments { get; set; }
		/// <summary>With Bonus</summary>
		[JsonProperty(PropertyName = "withBonus")]
		public bool WithBonus { get; set; }
		/// <summary>With Revolving</summary>
		[JsonProperty(PropertyName = "withRevolving")]
		public bool WithRevolving { get; set; }
		/// <summary>Three D Secure</summary>
		[JsonProperty(PropertyName = "threeDSecure")]
		public ThreeDSecure ThreeDSecure { get; set; }
	}

	/// <summary>
	/// 3DSecure情報
	/// </summary>
	[JsonObject("threeDSecure")]
	public class ThreeDSecure
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ThreeDSecure()
		{
			this.AuthenticationIndicatorType = RakutenConstants.AUTHENTICATION_INDICATOR_TYPE_DEFAULT;
			this.TransactionType = RakutenConstants.TRANSACTION_TYPE_DEFAULT;
			this.MerchantName = (string.IsNullOrEmpty(Constants.PAYMENT_SETTING_RAKUTEN_3DSECURE_MERCHANT_NAME) == false)
				? Constants.PAYMENT_SETTING_RAKUTEN_3DSECURE_MERCHANT_NAME
				: null;
		}
		/// <summary>MerchantId</summary>
		[JsonProperty(PropertyName = "merchantId")]
		public string MerchantId { get; set; }
		/// <summary>MerchantName</summary>
		[JsonProperty(PropertyName = "merchantName")]
		public string MerchantName { get; set; }
		/// <summary>AuthenticationIndicatorType</summary>
		[JsonProperty(PropertyName = "authenticationIndicatorType")]
		public string AuthenticationIndicatorType { get; set; }
		/// <summary>MessageCategoryType</summary>
		[JsonProperty(PropertyName = "messageCategoryType")]
		public string MessageCategoryType { get; set; }
		/// <summary>TransactionType</summary>
		[JsonProperty(PropertyName = "transactionType")]
		public string TransactionType { get; set; }
		/// <summary>PurchaseDate</summary>
		[JsonProperty(PropertyName = "purchaseDate")]
		public string PurchaseDate { get; set; }
		/// <summary>ChallengeIndicatorType</summary>
		[JsonProperty(PropertyName = "challengeIndicatorType")]
		public string ChallengeIndicatorType { get; set; }
		/// <summary>CardHolderInformation</summary>
		[JsonProperty(PropertyName = "cardHolderInformation")]
		public CardHolderInformation CardHolderInformation { get; set; }
	}

	/// <summary>
	/// カード所有者情報
	/// </summary>
	[JsonObject("cardHolderInformation")]
	public class CardHolderInformation
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CardHolderInformation()
		{
		}
		/// <summary>BillingAddressCountry</summary>
		[JsonProperty(PropertyName = "billingAddressCountry")]
		public int BillingAddressCountry { get; set; }
		/// <summary>BillingAddressState</summary>
		[JsonProperty(PropertyName = "billingAddressState")]
		public string BillingAddressState { get; set; }
		/// <summary>BillingAddressCity</summary>
		[JsonProperty(PropertyName = "billingAddressCity")]
		public string BillingAddressCity { get; set; }
		/// <summary>BillingAddressLine1</summary>
		[JsonProperty(PropertyName = "billingAddressLine1")]
		public string BillingAddressLine1 { get; set; }
		/// <summary>BillingAddressLine2</summary>
		[JsonProperty(PropertyName = "billingAddressLine2")]
		public string BillingAddressLine2 { get; set; }
		/// <summary>BillingAddressPostCode</summary>
		[JsonProperty(PropertyName = "billingAddressPostCode")]
		public string BillingAddressPostCode { get; set; }
		/// <summary>ShippingAddressCountry</summary>
		[JsonProperty(PropertyName = "shippingAddressCountry")]
		public string ShippingAddressCountry { get; set; }
		/// <summary>ShippingAddressState</summary>
		[JsonProperty(PropertyName = "shippingAddressState")]
		public string ShippingAddressState { get; set; }
		/// <summary>ShippingAddressCity</summary>
		[JsonProperty(PropertyName = "shippingAddressCity")]
		public string ShippingAddressCity { get; set; }
		/// <summary>ShippingAddressLine1</summary>
		[JsonProperty(PropertyName = "shippingAddressLine1")]
		public string ShippingAddressLine1 { get; set; }
		/// <summary>ShippingAddressLine2</summary>
		[JsonProperty(PropertyName = "shippingAddressLine2")]
		public string ShippingAddressLine2 { get; set; }
		/// <summary>ShippingAddressPostCode</summary>
		[JsonProperty(PropertyName = "shippingAddressPostCode")]
		public string ShippingAddressPostCode { get; set; }
		/// <summary>CardHolderName</summary>
		[JsonProperty(PropertyName = "cardHolderName")]
		public string CardHolderName { get; set; }
		/// <summary>Email</summary>
		[JsonProperty(PropertyName = "email")]
		public string Email { get; set; }
		/// <summary>MobilePhoneNumber</summary>
		[JsonProperty(PropertyName = "homePhoneNumber")]
		public HomePhoneNumber HomePhoneNumber { get; set; }
		/// <summary>MobilePhoneNumber</summary>
		[JsonProperty(PropertyName = "mobilePhoneNumber")]
		public MobilePhoneNumber MobilePhoneNumber { get; set; }

	}

	/// <summary>
	/// 自宅電話番号
	/// </summary>
	[JsonObject("homePhoneNumber")]
	public class HomePhoneNumber
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public HomePhoneNumber()
		{
		}
		/// <summary>CountryCode</summary>
		[JsonProperty(PropertyName = "countryCode")]
		public string CountryCode { get; set; }
		/// <summary>Subscriber</summary>
		[JsonProperty(PropertyName = "subscriber")]
		public string Subscriber { get; set; }
	}

	/// <summary>
	/// 携帯番号
	/// </summary>
	[JsonObject("mobilePhoneNumber")]
	public class MobilePhoneNumber
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MobilePhoneNumber()
		{
		}
		/// <summary>CountryCode</summary>
		[JsonProperty(PropertyName = "countryCode")]
		public string CountryCode { get; set; }
		/// <summary>Subscriber</summary>
		[JsonProperty(PropertyName = "subscriber")]
		public string Subscriber { get; set; }
	}
}
