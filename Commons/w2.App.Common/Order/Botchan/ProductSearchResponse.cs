/*
=========================================================================================================
  Module      : Product Search Response(ProductSearchResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;

namespace w2.App.Common.Order.Botchan
{
	/// <summary>
	/// Product search response
	/// </summary>
	[Serializable]
	public class ProductSearchResponse
	{
		/// <summary>Result</summary>
		[JsonProperty("result")]
		public bool Result { get; set; }
		/// <summary>Message Id</summary>
		[JsonProperty("message_id")]
		public string MessageId { get; set; }
		/// <summary>Message</summary>
		[JsonProperty("message")]
		public string Message { get; set; }
		/// <summary>商品情報データ</summary>
		[JsonProperty("data")]
		public ProductInformation[] Data { get; set; }

		/// <summary>
		/// Product information
		/// </summary>
		[JsonObject("data")]
		public class ProductInformation
		{
			/// <summary>Product info</summary>
			[JsonProperty("product")]
			public Product ProductInfo { get; set; }
			/// <summary>Product stock info</summary>
			[JsonProperty("product_stock")]
			public ProductStock ProductStockInfo { get; set; }
		}

		/// <summary>
		/// Product
		/// </summary>
		[JsonObject("product")]
		public class Product
		{
			/// <summary>Shop id</summary>
			[JsonProperty("shop_id")]
			public string ShopId { get; set; }
			/// <summary>Supplier id</summary>
			[JsonProperty("supplier_id")]
			public string SupplierId { get; set; }
			/// <summary>Product id</summary>
			[JsonProperty("product_id")]
			public string ProductId { get; set; }
			/// <summary>Cooperation id 1</summary>
			[JsonProperty("cooperation_id1")]
			public string CooperationId1 { get; set; }
			/// <summary>Cooperation id 2</summary>
			[JsonProperty("cooperation_id2")]
			public string CooperationId2 { get; set; }
			/// <summary>Cooperation id 3</summary>
			[JsonProperty("cooperation_id3")]
			public string CooperationId3 { get; set; }
			/// <summary>Cooperation id 4</summary>
			[JsonProperty("cooperation_id4")]
			public string CooperationId4 { get; set; }
			/// <summary>Cooperation id 5</summary>
			[JsonProperty("cooperation_id5")]
			public string CooperationId5 { get; set; }
			/// <summary>Mall expansion product id</summary>
			[JsonProperty("mall_ex_product_id")]
			public string MallExProductId { get; set; }
			/// <summary>Maker id</summary>
			[JsonProperty("maker_id")]
			public string MakerId { get; set; }
			/// <summary>Maker name</summary>
			[JsonProperty("maker_name")]
			public string MakerName { get; set; }
			/// <summary>Name</summary>
			[JsonProperty("name")]
			public string Name { get; set; }
			/// <summary>Name Kana</summary>
			[JsonProperty("name_kana")]
			public string NameKana { get; set; }
			/// <summary>Name 2</summary>
			[JsonProperty("name2")]
			public string Name2 { get; set; }
			/// <summary>Name 2 Kana</summary>
			[JsonProperty("name2_kana")]
			public string Name2Kana { get; set; }
			/// <summary>Display price</summary>
			[JsonProperty("display_price")]
			public decimal DisplayPrice { get; set; }
			/// <summary>Display special price</summary>
			[JsonProperty("display_special_price")]
			public decimal? DisplaySpecialPrice { get; set; }
			/// <summary>Tax included flag</summary>
			[JsonProperty("tax_included_flg")]
			public string TaxIncludedFlg { get; set; }
			/// <summary>Tax rate</summary>
			[JsonProperty("tax_rate")]
			public decimal TaxRate { get; set; }
			/// <summary>Tax round type</summary>
			[JsonProperty("tax_round_type")]
			public string TaxRoundType { get; set; }
			/// <summary>Price pretax</summary>
			[JsonProperty("price_pretax")]
			public decimal PricePretax { get; set; }
			/// <summary>Price shipping</summary>
			[JsonProperty("price_shipping")]
			public decimal? PriceShipping { get; set; }
			/// <summary>Shipping type</summary>
			[JsonProperty("shipping_type")]
			public string ShippingType { get; set; }
			/// <summary>Shipping size kbn</summary>
			[JsonProperty("shipping_size_kbn")]
			public string ShippingSizeKbn { get; set; }
			/// <summary>Point kbn 1</summary>
			[JsonProperty("point_kbn1")]
			public string PointKbn1 { get; set; }
			/// <summary>Point 1</summary>
			[JsonProperty("point1")]
			public decimal Point1 { get; set; }
			/// <summary>Point kbn 2</summary>
			[JsonProperty("point_kbn2")]
			public string PointKbn2 { get; set; }
			/// <summary>Point 2</summary>
			[JsonProperty("point2")]
			public decimal Point2 { get; set; }
			/// <summary>Point kbn 3</summary>
			[JsonProperty("point_kbn3")]
			public string PointKbn3 { get; set; }
			/// <summary>Point 3</summary>
			[JsonProperty("point3")]
			public decimal Point3 { get; set; }
			/// <summary>Campaign from</summary>
			[JsonProperty("campaign_from")]
			public string CampaignFrom { get; set; }
			/// <summary>Campaign to</summary>
			[JsonProperty("campaign_to")]
			public string CampaignTo { get; set; }
			/// <summary>Campaign point kbn</summary>
			[JsonProperty("campaign_point_kbn")]
			public string CampaignPointKbn { get; set; }
			/// <summary>Campaign point</summary>
			[JsonProperty("campaign_point")]
			public int CampaignPoint { get; set; }
			/// <summary>Max sell quantity</summary>
			[JsonProperty("max_sell_quantity")]
			public int MaxSellQuantity { get; set; }
			/// <summary>Related product id cs 1</summary>
			[JsonProperty("related_product_id_cs1")]
			public string RelatedProductIdCs1 { get; set; }
			/// <summary>Related product id cs 2</summary>
			[JsonProperty("related_product_id_cs2")]
			public string RelatedProductIdCs2 { get; set; }
			/// <summary>Related product id cs 3</summary>
			[JsonProperty("related_product_id_cs3")]
			public string RelatedProductIdCs3 { get; set; }
			/// <summary>Related product id cs 4</summary>
			[JsonProperty("related_product_id_cs4")]
			public string RelatedProductIdCs4 { get; set; }
			/// <summary>Related product id cs 5</summary>
			[JsonProperty("related_product_id_cs5")]
			public string RelatedProductIdCs5 { get; set; }
			/// <summary>Related product id us 1</summary>
			[JsonProperty("related_product_id_us1")]
			public string RelatedProductIdUs1 { get; set; }
			/// <summary>Related product id us 2</summary>
			[JsonProperty("related_product_id_us2")]
			public string RelatedProductIdUs2 { get; set; }
			/// <summary>Related product id us 3</summary>
			[JsonProperty("related_product_id_us3")]
			public string RelatedProductIdUs3 { get; set; }
			/// <summary>Related product id us 4</summary>
			[JsonProperty("related_product_id_us4")]
			public string RelatedProductIdUs4 { get; set; }
			/// <summary>Related product id us 5</summary>
			[JsonProperty("related_product_id_us5")]
			public string RelatedProductIdUs5 { get; set; }
			/// <summary>Reservation flag</summary>
			[JsonProperty("reservation_flg")]
			public string ReservationFlg { get; set; }
			/// <summary>Fixed purchase flag</summary>
			[JsonProperty("fixed_purchase_flg")]
			public string FixedPurchaseFlg { get; set; }
			/// <summary>Date created</summary>
			[JsonProperty("date_created")]
			public DateTime DateCreated { get; set; }
			/// <summary>Date changed</summary>
			[JsonProperty("date_changed")]
			public DateTime DateChanged { get; set; }
			/// <summary>Member rank discount flag</summary>
			[JsonProperty("member_rank_discount_flg")]
			public string MemberRankDiscountFlg { get; set; }
			/// <summary>Display member rank</summary>
			[JsonProperty("display_member_rank")]
			public string DisplayMemberRank { get; set; }
			/// <summary>Buy able member rank</summary>
			[JsonProperty("buyable_member_rank")]
			public string BuyableMemberRank { get; set; }
			/// <summary>Brand id 1</summary>
			[JsonProperty("brand_id1")]
			public string BrandId1 { get; set; }
			/// <summary>Brand id 2</summary>
			[JsonProperty("brand_id2")]
			public string BrandId2 { get; set; }
			/// <summary>Brand id 3</summary>
			[JsonProperty("brand_id3")]
			public string BrandId3 { get; set; }
			/// <summary>Brand id 4</summary>
			[JsonProperty("brand_id4")]
			public string BrandId4 { get; set; }
			/// <summary>Brand id 5</summary>
			[JsonProperty("brand_id5")]
			public string BrandId5 { get; set; }
			/// <summary>Gift flag</summary>
			[JsonProperty("gift_flg")]
			public string GiftFlg { get; set; }
			/// <summary>Age limit flag</summary>
			[JsonProperty("age_limit_flg")]
			public string AgeLimitFlg { get; set; }
			/// <summary>Plural shipping price free flag</summary>
			[JsonProperty("plural_shipping_price_free_flg")]
			public string PluralShippingPriceFreeFlg { get; set; }
			/// <summary>Digital contents flag</summary>
			[JsonProperty("digital_contents_flg")]
			public string DigitalContentsFlg { get; set; }
			/// <summary>Download url</summary>
			[JsonProperty("download_url")]
			public string DownloadUrl { get; set; }
			/// <summary>Display sell flag</summary>
			[JsonProperty("display_sell_flg")]
			public string DisplaySellFlg { get; set; }
			/// <summary>Limited payment ids</summary>
			[JsonProperty("limited_payment_ids")]
			public string LimitedPaymentIds { get; set; }
			/// <summary>Fixed purchase first time price</summary>
			[JsonProperty("fixed_purchase_firsttime_price")]
			public decimal? FixedPurchaseFirstTimePrice { get; set; }
			/// <summary>Fixed purchase price</summary>
			[JsonProperty("fixed_purchase_price")]
			public decimal? FixedPurchasePrice { get; set; }
			/// <summary>Bundle item display type</summary>
			[JsonProperty("bundle_item_display_type")]
			public string BundleItemDisplayType { get; set; }
			/// <summary>Product type</summary>
			[JsonProperty("product_type")]
			public string ProductType { get; set; }
			/// <summary>Limited fixed purchase kbn 1 setting</summary>
			[JsonProperty("limited_fixed_purchase_kbn1_setting")]
			public string LimitedFixedPurchaseKbn1Setting { get; set; }
			/// <summary>Limited fixed purchase kbn3 setting</summary>
			[JsonProperty("limited_fixed_purchase_kbn3_setting")]
			public string LimitedFixedPurchaseKbn3Setting { get; set; }
			/// <summary>Limited fixed purchase kbn4 setting</summary>
			[JsonProperty("limited_fixed_purchase_kbn4_setting")]
			public string LimitedFixedPurchaseKbn4Setting { get; set; }
			/// <summary>Recommend product id</summary>
			[JsonProperty("recommend_product_id")]
			public string RecommendProductId { get; set; }
			/// <summary>Cooperation id 6</summary>
			[JsonProperty("cooperation_id6")]
			public string CooperationId6 { get; set; }
			/// <summary>Cooperation id 7</summary>
			[JsonProperty("cooperation_id7")]
			public string CooperationId7 { get; set; }
			/// <summary>Cooperation id 8</summary>
			[JsonProperty("cooperation_id8")]
			public string CooperationId8 { get; set; }
			/// <summary>Cooperation id 9</summary>
			[JsonProperty("cooperation_id9")]
			public string CooperationId9 { get; set; }
			/// <summary>Cooperation id 10</summary>
			[JsonProperty("cooperation_id10")]
			public string CooperationId10 { get; set; }
			/// <summary>Fixed purchase product order limit flag</summary>
			[JsonProperty("fixed_purchase_product_order_limit_flg")]
			public string FixedPurchaseProductOrderLimitFlg { get; set; }
			/// <summary>Display only fixed purchase member flag</summary>
			[JsonProperty("display_only_fixed_purchase_member_flg")]
			public string DisplayOnlyFixedPurchaseMemberFlg { get; set; }
			/// <summary>Sell only fixed purchase member flag</summary>
			[JsonProperty("sell_only_fixed_purchase_member_flg")]
			public string SellOnlyFixedPurchaseMemberFlg { get; set; }
			/// <summary>Product weight gram</summary>
			[JsonProperty("product_weight_gram")]
			public int ProductWeightGram { get; set; }
			/// <summary>Tax category id</summary>
			[JsonProperty("tax_category_id")]
			public string TaxCategoryId { get; set; }
			/// <summary>Product order limit flag fp</summary>
			[JsonProperty("product_order_limit_flg_fp")]
			public string ProductOrderLimitFlgFp { get; set; }
			/// <summary>Fixed purchase cancelable count</summary>
			[JsonProperty("fixed_purchase_cancelable_count")]
			public int FixedPurchaseCancelableCount { get; set; }
			/// <summary>Fixed purchase limited user level ids</summary>
			[JsonProperty("fixed_purchase_limited_user_level_ids")]
			public string FixedPurchaseLimitedUserLevelIds { get; set; }
			/// <summary>Fixed purchase next shipping product id</summary>
			[JsonProperty("fixed_purchase_next_shipping_product_id")]
			public string FixedPurchaseNextShippingProductId { get; set; }
			/// <summary>Fixed purchase next shipping variation id</summary>
			[JsonProperty("fixed_purchase_next_shipping_variation_id")]
			public string FixedPurchaseNextShippingVariationId { get; set; }
			/// <summary>Fixed purchase next shipping item quantity</summary>
			[JsonProperty("fixed_purchase_next_shipping_item_quantity")]
			public int FixedPurchaseNextShippingItemQuantity { get; set; }
			/// <summary>Fixed purchase limited skipped count</summary>
			[JsonProperty("fixed_purchase_limited_skipped_count")]
			public int? FixedPurchaseLimitedSkippedCount { get; set; }
		}

		/// <summary>
		/// Product stock
		/// </summary>
		[JsonObject("product_stock")]
		public class ProductStock
		{
			/// <summary>Shop id</summary>
			[JsonProperty("shop_id")]
			public string ShopId { get; set; }
			/// <summary>Product id</summary>
			[JsonProperty("product_id")]
			public string ProductId { get; set; }
			/// <summary>Variation id</summary>
			[JsonProperty("variation_id")]
			public string VariationId { get; set; }
			/// <summary>Stock</summary>
			[JsonProperty("stock")]
			public int Stock { get; set; }
			/// <summary>Real stock</summary>
			[JsonProperty("realstock")]
			public int RealStock { get; set; }
			/// <summary>Real stock b</summary>
			[JsonProperty("realstock_b")]
			public int RealStockB { get; set; }
			/// <summary>Real stock c</summary>
			[JsonProperty("realstock_c")]
			public int RealStockC { get; set; }
			/// <summary>Real stock reserved</summary>
			[JsonProperty("realstock_reserved")]
			public int RealStockReserved { get; set; }
			/// <summary>Date created</summary>
			[JsonProperty("date_created")]
			public DateTime DateCreated { get; set; }
			/// <summary>Date changed</summary>
			[JsonProperty("date_changed")]
			public DateTime DateChanged { get; set; }
		}
	}
}