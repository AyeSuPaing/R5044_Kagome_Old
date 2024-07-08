/*
=========================================================================================================
  Module      : 楽天ペイ受注情報取得API 税率モデル (RakutenApiTaxSummary.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Mall.RakutenApi
{
	/// <summary>
	/// 楽天ペイ受注API 税率モデル
	/// </summary>
	[JsonObject(Constants.RAKUTEN_PAY_API_ORDER_MODEL_TAX_SUMMARY_MODEL_LIST)]
	public class RakutenApiTaxSummary
	{
		/// <summary>税率</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_TAX_SUMMARY_MODEL_TAX_RATE)]
		public decimal TaxRate { get; set; }
		/// <summary>請求金額</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_TAX_SUMMARY_MODEL_REQ_PRICE)]
		public decimal ReqPrice { get; set; }
		/// <summary>請求額に対する税額</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_TAX_SUMMARY_MODEL_REQ_PRICE_TAX)]
		public decimal ReqPriceTax { get; set; }
		/// <summary>合計金額</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_TAX_SUMMARY_MODEL_TOTAL_PRICE)]
		public decimal TotalPrice { get; set; }
		/// <summary>決済手数料</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_TAX_SUMMARY_MODEL_PATMENT_CHARGE)]
		public decimal PaymentChange { get; set; }
		/// <summary>クーポン割引額</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_TAX_SUMMARY_MODEL_COUPON_PRICE)]
		public decimal CouponPrice { get; set; }
		/// <summary>利用ポイント数</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_TAX_SUMMARY_MODEL_POINT)]
		public decimal Point { get; set; }
	}
}
