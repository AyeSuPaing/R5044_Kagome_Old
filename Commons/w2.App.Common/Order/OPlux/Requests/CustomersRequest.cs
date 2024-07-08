/*
=========================================================================================================
  Module      : Customers Request(CustomersRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Order.OPlux.Requests.Helper;

namespace w2.App.Common.Order.OPlux.Requests
{
	/// <summary>
	/// Customers request
	/// </summary>
	public class CustomersRequest
	{
		/// <summary>Buyer</summary>
		[RequestKey("buyer")]
		public BuyerRequest Buyer { get; set; }
		/// <summary>Delivery</summary>
		[RequestKey("delivery")]
		public DeliveryRequest Delivery { get; set; }

		/// <summary>
		/// Buyer request
		/// </summary>
		public class BuyerRequest
		{
			/// <summary>Type</summary>
			[RequestKey("type")]
			public string Type { get; set; }
			/// <summary>購入者会員ID</summary>
			[RequestKey("ID")]
			public string ID { get; set; }
			/// <summary>Hashed name</summary>
			[RequestKey("hashed_name")]
			public HashedNameRequest HashedName { get; set; }
			/// <summary>Address</summary>
			[RequestKey("address")]
			public AddressRequest Address { get; set; }
			/// <summary>Tel</summary>
			[RequestKey("tel")]
			public TelRequest Tel { get; set; }
			/// <summary>Email</summary>
			[RequestKey("email")]
			public EmailRequest Email { get; set; }
			/// <summary>Company</summary>
			[RequestKey("company")]
			public CompanyRequest Company { get; set; }
		}

		/// <summary>
		/// Delivery request
		/// </summary>
		public class DeliveryRequest
		{
			/// <summary>Type</summary>
			[RequestKey("type")]
			public string Type { get; set; }
			/// <summary>Hashed name</summary>
			[RequestKey("hashed_name")]
			public HashedNameRequest HashedName { get; set; }
			/// <summary>Address</summary>
			[RequestKey("address")]
			public AddressRequest Address { get; set; }
			/// <summary>Tel</summary>
			[RequestKey("tel")]
			public TelRequest Tel { get; set; }
			/// <summary>Shipping</summary>
			[RequestKey("shipping")]
			public ShippingRequest Shipping { get; set; }
		}

		/// <summary>
		/// Hashed name request
		/// </summary>
		public class HashedNameRequest
		{
			/// <summary>名前ハッシュ</summary>
			[RequestKey("first_name")]
			public string FirstName { get; set; }
			/// <summary>正規化済み名前ハッシュ</summary>
			[RequestKey("normalized_first_name")]
			public string NormalizedFirstName { get; set; }
			/// <summary>苗字ハッシュ</summary>
			[RequestKey("last_name")]
			public string LastName { get; set; }
			/// <summary>正規化済み苗字ハッシュ</summary>
			[RequestKey("normalized_last_name")]
			public string NormalizedLastName { get; set; }
			/// <summary>氏名文字数</summary>
			[RequestKey("nameLength")]
			public int NameLength { get; set; }
			/// <summary>氏名漢字数</summary>
			[RequestKey("kanjiCountInName")]
			public int KanjiCountInName { get; set; }
			/// <summary>氏名ひらがな数</summary>
			[RequestKey("hiraganaCountInName")]
			public int HiraganaCountInName { get; set; }
			/// <summary>氏名カタカナ数</summary>
			[RequestKey("katakanaCountInName")]
			public int KatakanaCountInName { get; set; }
			/// <summary>氏名アルファベット数</summary>
			[RequestKey("alphabetCountInName")]
			public int AlphabetCountInName { get; set; }
			/// <summary>氏名その他文字数</summary>
			[RequestKey("otherCountInName")]
			public int OtherCountInName { get; set; }
			/// <summary>苗字辞書存在フラグ</summary>
			[RequestKey("validName")]
			public string ValidName { get; set; }
		}

		/// <summary>
		/// Address request
		/// </summary>
		public class AddressRequest
		{
			/// <summary>国コード</summary>
			[RequestKey("country")]
			public string Country { get; set; }
			/// <summary>郵便番号</summary>
			[RequestKey("zipcode")]
			public string Zipcode { get; set; }
			/// <summary>都道府県</summary>
			[RequestKey("addressA")]
			public string AddressA { get; set; }
			/// <summary>市区町村</summary>
			[RequestKey("addressB")]
			public string AddressB { get; set; }
			/// <summary>住所その他</summary>
			[RequestKey("addressC")]
			public string AddressC { get; set; }
		}

		/// <summary>
		/// Shipping request
		/// </summary>
		public class ShippingRequest
		{
			/// <summary>配送希望日時</summary>
			[RequestKey("specified_deliver_datetime")]
			public DateTime? SpecifiedDeliverDatetime { get; set; }
			/// <summary>配送希望有無</summary>
			[RequestKey("delivery_specified_existence")]
			public string DeliverySpecifiedExistence { get; set; }
			/// <summary>Items</summary>
			[RequestKey("items")]
			public ShippingItemRequest[] Items { get; set; }
		}

		/// <summary>
		/// Shipping item request
		/// </summary>
		[RequestKey("item")]
		public class ShippingItemRequest
		{
			/// <summary>加盟店商品ID</summary>
			[RequestKey("shop_item_id")]
			public string ShopItemId { get; set; }
			/// <summary>単価</summary>
			[RequestKey("item_price")]
			public decimal ItemPrice { get; set; }
			/// <summary>数量</summary>
			[RequestKey("item_quantity")]
			public int ItemQuantity { get; set; }
			/// <summary>商品名</summary>
			[RequestKey("item_name")]
			public string ItemName { get; set; }
			/// <summary>商品カテゴリ</summary>
			[RequestKey("item_category")]
			public string ItemCategory { get; set; }
		}

		/// <summary>
		/// Tel request
		/// </summary>
		public class TelRequest
		{
			/// <summary>固定電話番号</summary>
			[RequestKey("fixed_number")]
			public string FixedNumber { get; set; }
		}

		/// <summary>
		/// Email request
		/// </summary>
		public class EmailRequest
		{
			/// <summary>PC</summary>
			[RequestKey("pc")]
			public PCRequest PC { get; set; }
		}

		/// <summary>
		/// PC request
		/// </summary>
		public class PCRequest
		{
			/// <summary>メールアカウントハッシュ</summary>
			[RequestKey("hashed_account")]
			public string HashedAccount { get; set; }
			/// <summary>メールドメイン</summary>
			[RequestKey("domain")]
			public string Domain { get; set; }
		}

		/// <summary>
		/// Company request
		/// </summary>
		public class CompanyRequest
		{
			/// <summary>購入者会社名</summary>
			[RequestKey("name")]
			public string Name { get; set; }
		}
	}
}
