/*
=========================================================================================================
  Module      : Gooddeal request (GooddealRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Gooddeal
{
	/// <summary>
	/// Gooddeal request
	/// </summary>
	public class GooddealRequest
	{
		/// <summary>corporation id</summary>
		[JsonProperty("corpid")]
		public string CorporationId
		{
			get { return Constants.TWSHIPPING_GOODDEAL_CORPORATION_ID; }
		}
		/// <summary>check time</summary>
		[JsonProperty("checktime")]
		public string CheckTime { set; get; }
		/// <summary>Delivery type id</summary>
		[JsonProperty("del_type_id")]
		public string DeliveryTypeId { set; get; }
		/// <summary>Order no</summary>
		[JsonProperty("OrderNo")]
		public string OrderNo { set; get; }
		/// <summary>Post id</summary>
		[JsonProperty("post_id")]
		public string PostId { set; get; }
		/// <summary>Address</summary>
		[JsonProperty("address")]
		public string Address { set; get; }
		/// <summary>Name</summary>
		[JsonProperty("name")]
		public string Name { set; get; }
		/// <summary>Phone no</summary>
		[JsonProperty("phone_no")]
		public string PhoneNo { set; get; }
		/// <summary>Mobile no</summary>
		[JsonProperty("mobile_no")]
		public string MobileNo { set; get; }
		/// <summary>Agency fee</summary>
		[JsonProperty("agency_fee")]
		public string AgencyFee { set; get; }
		/// <summary>Packstr</summary>
		[JsonProperty("packstr")]
		public string Packstr { set; get; }
		/// <summary>Store Id</summary>
		[JsonProperty("storeid")]
		public string StoreId { set; get; }
		/// <summary>Store name</summary>
		[JsonProperty("storename")]
		public string StoreName { set; get; }
		/// <summary>Delivery no</summary>
		[JsonProperty("deliver_no")]
		public string DeliveryNo { set; get; }
		/// <summary>Note</summary>
		[JsonProperty("note")]
		public string Note { set; get; }
		/// <summary>Items</summary>
		[JsonProperty("items")]
		public Product[] Items { set; get; }
		/// <summary>Register order no</summary>
		[JsonProperty("orderno")]
		public string RegisterOrderNo { set; get; }
		/// <summary>配送時間帯</summary>
		[JsonProperty("del_time_id")]
		public string DeliveryTimeId { set; get; }
		/// <summary>國家</summary>
		[JsonProperty("country")]
		public string Country { set; get; }
		/// <summary>郵編</summary>
		[JsonProperty("country_post")]
		public string CountryPost { set; get; }
		/// <summary>幣別</summary>
		[JsonProperty("currency_id")]
		public string CurrencyId { set; get; }
		/// <summary>商品價值</summary>
		[JsonProperty("post_value")]
		public string PostValue { set; get; }
		/// <summary>運費付款方式</summary>
		[JsonProperty("post_paytype")]
		public string PostPaytype { set; get; }

		/// <summary>
		/// Convert to Json request
		/// </summary>
		/// <returns>A json request</returns>
		public string ToJsonRequest()
		{
			var jsonRequest = JsonConvert.SerializeObject(
				this,
				new JsonSerializerSettings
				{
					Formatting = Formatting.Indented,
					NullValueHandling = NullValueHandling.Ignore
				});
			return jsonRequest;
		}
	}

	/// <summary>
	/// Product
	/// </summary>
	public class Product
	{
		/// <summary>Product id</summary>
		[JsonProperty("product_id")]
		public string ProductId { get; set; }
		/// <summary>Quantity</summary>
		[JsonProperty("qty")]
		public int Quantity { get; set; }
		/// <summary>Price</summary>
		[JsonProperty("price")]
		public decimal Price { get; set; }
	}
}
