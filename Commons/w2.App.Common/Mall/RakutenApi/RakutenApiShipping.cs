/*
=========================================================================================================
  Module      : 楽天ペイ受注情報取得API 発送モデル (RakutenApiShipping.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Mall.RakutenApi
{
	/// <summary>
	/// 楽天ペイ受注API 発送モデル
	/// </summary>
	[JsonObject(Constants.RAKUTEN_PAY_API_PACKAGE_MODEL_SHIPPING_MODEL_LIST)]
	public class RakutenApiShipping
	{
		/// <summary>発送明細ID	</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_SHIPPING_MODEL_SHIPPING_DETAIL_ID)]
		public string ShippingDetailId { get; set; }
		/// <summary>お荷物伝票番号</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_SHIPPING_MODEL_SHIPPING_NUMBER)]
		public string ShippingNumber { get; set; }
		/// <summary>配送会社</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_SHIPPING_MODEL_DELIVERY_COMPANY)]
		public string DeliveryCompany { get; set; }
		/// <summary>配送会社名</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_SHIPPING_MODEL_DELIVERY_COMPANY_NAME)]
		public string DeliveryCompanyName { get; set; }
		/// <summary>発送日</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_SHIPPING_MODEL_SHIPPING_DATE)]
		public string ShippingDate { get; set; }
	}
}
