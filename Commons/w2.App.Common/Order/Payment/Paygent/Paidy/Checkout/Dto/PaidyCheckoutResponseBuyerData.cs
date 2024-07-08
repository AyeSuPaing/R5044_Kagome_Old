/*
=========================================================================================================
  Module      :Paidy Checkout 購入者履歴情報モデル (PaidyAuthorizationResponseBuyerData.cs)
  ･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Order.Payment.Paygent.Paidy.Checkout.Dto
{
	/// <summary>
	/// Paidy Checkout 購入者履歴情報モデル
	/// </summary>
	public class PaidyCheckoutResponseBuyerData
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="age">年齢</param>
		/// <param name="agePlatform">アカウント作成経過日数</param>
		/// <param name="accountRegistrationDate">アカウント登録日(YYY-MM-DD)</param>
		/// <param name="daysSinceFirstTransaction">初回購入日(キャンセル・Paidy決済を除く)</param>
		/// <param name="ltv">LTV(キャンセル・Paidy決済を除く)</param>
		/// <param name="orderCount">注文回数(キャンセル・Paidy決済を除く)</param>
		/// <param name="lastOrderAmount">最終購入時の金額(キャンセル・Paidy決済を除く)</param>
		/// <param name="lastOrderAt">最終購入からの経過日数(キャンセル・Paidy決済を除く)</param>
		/// <param name="lastOrderDate">最終購入日(YYYY-MM-DD)</param>
		/// <param name="orderAmountLast3Months">過去3ヶ月の合計購入金額</param>
		/// <param name="orderCountLast3Months">過去3ヶ月の合計注文数</param>
		/// <param name="additionalShippingAddresses">配送先住所</param>
		/// <param name="billingAddress">請求先住所</param>
		/// <param name="gender">性別</param>
		/// <param name="numberOfPoints">ユーザーポイント数</param>
		/// <param name="orderItemCategories">商品カテゴリ</param>
		public PaidyCheckoutResponseBuyerData(
			string userId,
			int age,
			int agePlatform,
			string accountRegistrationDate,
			int daysSinceFirstTransaction,
			double ltv,
			int orderCount,
			double lastOrderAmount,
			int lastOrderAt,
			string lastOrderDate,
			int orderAmountLast3Months,
			int orderCountLast3Months,
			PaidyCheckoutResponseShippingAddress additionalShippingAddresses,
			PaidyCheckoutResponseShippingAddress billingAddress,
			string gender,
			int numberOfPoints,
			string[] orderItemCategories)
		{
			this.UserId = userId;
			this.Age = age;
			this.AgePlatform = agePlatform;
			this.AccountRegistrationDate = accountRegistrationDate;
			this.DaysSinceFirstTransaction = daysSinceFirstTransaction;
			this.Ltv = ltv;
			this.OrderCount = orderCount;
			this.LastOrderAmount = lastOrderAmount;
			this.LastOrderAt = lastOrderAt;
			this.LastOrderDate = lastOrderDate;
			this.OrderAmountLast3Months = orderAmountLast3Months;
			this.OrderCountLast3Months = orderCountLast3Months;
			this.AdditionalShippingAddresses = additionalShippingAddresses;
			this.BillingAddress = billingAddress;
			this.Gender = gender;
			this.NumberOfPoints = numberOfPoints;
			this.OrderItemCategories = orderItemCategories;
		}

		/// <summary>ユーザーID</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_USER_ID)]
		public string UserId { get; private set; }
		/// <summary>年齢</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_AGE)]
		public int Age { get; private set; }
		/// <summary>作成経過日数</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_AGE_PLATFORM)]
		public int AgePlatform { get; private set; }
		/// <summary>アカウント登録日(YYYY-MM-DD)</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_ACCOUNT_REGISTRATION_DATE)]
		public string AccountRegistrationDate { get; private set; }
		/// <summary>初回購入日(キャンセル・Paidy決済を除く)</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_DAYS_SINCE_FIRST_TRANSACTION)]
		public int DaysSinceFirstTransaction { get; private set; }
		/// <summary>LTV(キャンセル・Paidy決済を除く)</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_LTV)]
		public double Ltv { get; private set; }
		/// <summary>注文回数(キャンセル・Paidy決済を除く)</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_ORDER_COUNT)]
		public int OrderCount { get; private set; }
		/// <summary>最終購入時の金額(キャンセル・Paidy決済を除く)</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_LAST_ORDER_AMOUNT)]
		public double LastOrderAmount { get; private set; }
		/// <summary>最終購入からの経過日数(キャンセル・Paidy決済を除く)</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_LAST_ORDER_AT)]
		public int LastOrderAt { get; private set; }
		/// <summary>最終購入日(YYYY-MM-DD)</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_LAST_ORDER_DATE)]
		public string LastOrderDate { get; private set; }
		/// <summary>過去3ヶ月の合計購入金額</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_ORDER_AMOUNT_LAST3MONTHS)]
		public int OrderAmountLast3Months { get; private set; }
		/// <summary>過去3ヶ月の合計注文数</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_ORDER_COUNT_LAST3MONTHS)]
		public int OrderCountLast3Months { get; private set; }
		/// <summary>配送先住所</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_ADDITIONAL_SHIPPING_ADDRESSES)]
		public PaidyCheckoutResponseShippingAddress AdditionalShippingAddresses { get; private set; }
		/// <summary>請求先住所</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_BILLING_ADDRESSES)]
		public PaidyCheckoutResponseShippingAddress BillingAddress { get; private set; }
		/// <summary>配送先種別</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_DELIVERY_LOCN_TYPE)]
		public string DeliveryLocnType { get; private set; }
		/// <summary>性別</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_GENDER)]
		public string Gender { get; private set; }
		/// <summary>定期購入時の決済回数</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_SUBSCRIPTION_COUNTER)]
		public int SubscriptionCounter { get; private set; }
		/// <summary>過去ユーザーが利用したことのある決済種別</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_PREVIOUS_PAYMENT_METHODS)]
		public PaidyCheckoutResponsePreviousPaymentMethods PreviousPaymentMethods { get; private set; }
		/// <summary>ユーザーポイント</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_NUMBER_OF_POINTS)]
		public int NumberOfPoints { get; private set; }
		/// <summary>商品カテゴリ</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_ORDER_ITEM_CATEGORIES)]
		public string[] OrderItemCategories { get; private set; }
	}
}
