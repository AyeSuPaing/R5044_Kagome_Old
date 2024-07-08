/*
=========================================================================================================
  Module      : Aftee Create Payment Request (AfteeCreatePaymentRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace w2.App.Common.Order.Payment.Aftee
{
	/// <summary>
	/// Aftee create payment request object
	/// </summary>
	[Serializable]
	public class AfteeCreatePaymentRequest
	{
		/// <summary>
		/// Data Response
		/// </summary>
		[JsonProperty(PropertyName = "data")]
		public DataResponse Data { get; set; }

		/// <summary>
		/// Aftee Data Response
		/// </summary>
		[Serializable]
		public class DataResponse
		{
			/// <summary>Authentication token</summary>
			[JsonProperty(PropertyName = "authentication_token")]
			public string AuthenticationToken { get; set; }
			/// <summary>Amount</summary>
			[JsonProperty(PropertyName = "amount")]
			public string Amount { get; set; }
			/// <summary>Shop transaction no</summary>
			[JsonProperty(PropertyName = "shop_transaction_no")]
			public string ShopTransactionNo { get; set; }
			/// <summary>Sales settled</summary>
			[JsonProperty(PropertyName = "sales_settled")]
			public bool SalesSettled { get; set; }
			/// <summary>Transaction options</summary>
			[JsonProperty(PropertyName = "transaction_options")]
			public int[] TransactionOptions { get; set; }
			/// <summary>Related transaction</summary>
			[JsonProperty(PropertyName = "related_id")]
			public string RelatedTransaction { get; set; }
			/// <summary>Updated transactions</summary>
			[JsonProperty(PropertyName = "updated_transactions")]
			public string UpdatedTransactions { get; set; }
			/// <summary>Description trans</summary>
			[JsonProperty(PropertyName = "description_trans")]
			public string DescriptionTrans { get; set; }
			/// <summary>Customer</summary>
			[JsonProperty(PropertyName = "customer")]
			public SourceCustomer Customer { get; set; }
			/// <summary>Dest customers</summary>
			[JsonProperty(PropertyName = "dest_customers")]
			public List<DestinationCustomer> DestCustomers { get; set; }
			/// <summary>Items</summary>
			[JsonProperty(PropertyName = "items")]
			public List<Item> Items { get; set; }
		}

		/// <summary>
		/// Source customer object
		/// </summary>
		[Serializable]
		public class SourceCustomer
		{
			/// <summary>Customer name</summary>
			[JsonProperty(PropertyName = "customer_name")]
			public string CustomerName { get; set; }
			/// <summary>Customer family name</summary>
			[JsonProperty(PropertyName = "customer_family_name")]
			public string CustomerFamilyName { get; set; }
			/// <summary>Customer given name</summary>
			[JsonProperty(PropertyName = "customer_given_name")]
			public string CustomerGivenName { get; set; }
			/// <summary>Customer name Kana</summary>
			[JsonProperty(PropertyName = "customer_name_kana")]
			public string CustomerNameKana { get; set; }
			/// <summary>Customer family name Kana</summary>
			[JsonProperty(PropertyName = "customer_family_name_kana")]
			public string CustomerFamilyNameKana { get; set; }
			/// <summary>Customer given name Kana</summary>
			[JsonProperty(PropertyName = "customer_given_name_kana")]
			public string CustomerGivenNameKana { get; set; }
			/// <summary>Phone number</summary>
			[JsonProperty(PropertyName = "phone_number")]
			public string PhoneNumber { get; set; }
			/// <summary>Birthday</summary>
			[JsonProperty(PropertyName = "birthday")]
			public string Birthday { get; set; }
			/// <summary>Sex division</summary>
			[JsonProperty(PropertyName = "sex_division")]
			public string SexDivision { get; set; }
			/// <summary>Company name</summary>
			[JsonProperty(PropertyName = "company_name")]
			public string CompanyName { get; set; }
			/// <summary>Department</summary>
			[JsonProperty(PropertyName = "department")]
			public string Department { get; set; }
			/// <summary>Zip code</summary>
			[JsonProperty(PropertyName = "zip_code")]
			public string ZipCode { get; set; }
			/// <summary>Address</summary>
			[JsonProperty(PropertyName = "address")]
			public string Address { get; set; }
			/// <summary>Tel</summary>
			[JsonProperty(PropertyName = "tel")]
			public string Tel { get; set; }
			/// <summary>Email</summary>
			[JsonProperty(PropertyName = "email")]
			public string Email { get; set; }
			/// <summary>Total purchase count</summary>
			[JsonProperty(PropertyName = "total_purchase_count")]
			public string TotalPurchaseCount { get; set; }
			/// <summary>Total purchase amount</summary>
			[JsonProperty(PropertyName = "total_purchase_amount")]
			public string TotalPurchaseAmount { get; set; }
		}

		/// <summary>
		/// Destination customer object
		/// </summary>
		[Serializable]
		public class DestinationCustomer
		{
			/// <summary>Dest customer name</summary>
			[JsonProperty(PropertyName = "dest_customer_name")]
			public string DestCustomerName { get; set; }
			/// <summary>Dest customer name Kana</summary>
			[JsonProperty(PropertyName = "dest_customer_name_kana")]
			public string DestCustomerNameKana { get; set; }
			/// <summary>Dest company name</summary>
			[JsonProperty(PropertyName = "dest_company_name")]
			public string DestCompanyName { get; set; }
			/// <summary>Dest department</summary>
			[JsonProperty(PropertyName = "dest_department")]
			public string DestDepartment { get; set; }
			/// <summary>Dest zip code</summary>
			[JsonProperty(PropertyName = "dest_zip_code")]
			public string DestZipCode { get; set; }
			/// <summary>Dest address</summary>
			[JsonProperty(PropertyName = "dest_address")]
			public string DestAddress { get; set; }
			/// <summary>Dest tel</summary>
			[JsonProperty(PropertyName = "dest_tel")]
			public string DestTel { get; set; }
		}

		/// <summary>
		/// Item object
		/// </summary>
		[Serializable]
		public class Item
		{
			/// <summary>Shop item ID</summary>
			[JsonProperty(PropertyName = "shop_item_id")]
			public string ShopItemId { get; set; }
			/// <summary>Item name</summary>
			[JsonProperty(PropertyName = "item_name")]
			public string ItemName { get; set; }
			/// <summary>Item price</summary>
			[JsonProperty(PropertyName = "item_price")]
			public string ItemPrice { get; set; }
			/// <summary>Item count</summary>
			[JsonProperty(PropertyName = "item_count")]
			public string ItemCount { get; set; }
		}
	}
}
