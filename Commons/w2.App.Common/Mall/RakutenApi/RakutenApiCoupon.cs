/*
=========================================================================================================
  Module      : 楽天ペイ受注情報取得API クーポンモデル (RakutenApiCoupon.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Mall.RakutenApi
{
	/// <summary>
	/// 楽天ペイ受注API クーポンモデル
	/// </summary>
	[JsonObject(Constants.RAKUTEN_PAY_API_ORDER_MODEL_COUPON_MODEL_LIST)]
	public class RakutenApiCoupon
	{
		/// <summary>クーポンコード</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_COUPON_MODEL_COUPON_CODE)]
		public string CouponCode { get; set; }
		/// <summary>クーポン対象の商品ID</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_COUPON_MODEL_ITEM_ID)]
		public string ItemId { get; set; }
		/// <summary>クーポン名</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_COUPON_MODEL_COUPON_NAME)]
		public string CouponName { get; set; }
		/// <summary>クーポン効果(サマリー)</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_COUPON_MODEL_COUPON_SUMMARY)]
		public string CouponSummary { get; set; }
		/// <summary>クーポン原資</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_COUPON_MODEL_COUPON_CAPITAL)]
		public string CouponCapital { get; set; }
		/// <summary>クーポン原資コード</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_COUPON_MODEL_COUPON_CAPITAL_CODE)]
		public string CouponCapitalCode { get; set; }
		/// <summary>有効期限</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_COUPON_MODEL_EXPIRY_DATE)]
		public string ExpiryDate { get; set; }
		/// <summary>クーポン割引単価</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_COUPON_MODEL_COUPON_PRICE)]
		public decimal CouponPrice { get; set; }
		/// <summary>クーポン利用数</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_COUPON_MODEL_COUPON_UNIT)]
		public int? CouponUnit { get; set; }
		/// <summary>クーポン利用金額</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_COUPON_MODEL_COUPON_TOTAL_PRICE)]
		public decimal CouponTotalPrice { get; set; }
	}
}
