/*
=========================================================================================================
  Module      : 楽天ペイ受注情報取得API 送付先モデル (RakutenApiPackage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Mall.RakutenApi
{
	/// <summary>
	/// 楽天ペイ受注API 送付先モデル
	/// </summary>
	[JsonObject(Constants.RAKUTEN_PAY_API_ORDER_MODEL_PACKAGE_MODEL_LIST)]
	public class RakutenApiPackage
	{
		/// <summary>送付先ID</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_PACKAGE_MODEL_BASKET_ID)]
		public string BasketId { get; set; }
		/// <summary>送料</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_PACKAGE_MODEL_POSTAGE_PRICE)]
		public decimal PostagePrice { get; set; }
		/// <summary>送料税率</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_PACKAGE_MODEL_POSTAGE_TAX_RATE)]
		public decimal PostageTaxRate { get; set; }
		/// <summary>代引料</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_PACKAGE_MODEL_DELIVERY_PRICE)]
		public decimal DeliveryPrice { get; set; }
		/// <summary>代引料税率</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_PACKAGE_MODEL_DELIVERY_TAX_RATE)]
		public decimal DeliveryTaxRate { get; set; }
		/// <summary>消費税</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_PACKAGE_MODEL_GOODS_TAX)]
		public decimal GoodsTax { get; set; }
		/// <summary>商品合計金額</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_PACKAGE_MODEL_GOODS_PRICE)]
		public decimal GoodsPrice { get; set; }
		/// <summary>合計金額</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_PACKAGE_MODEL_TOTAL_PRICE)]
		public decimal TotalPrice { get; set; }
		/// <summary>のし</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_PACKAGE_MODEL_NOSHI)]
		public string Noshi { get; set; }
		/// <summary>購入時配送会社</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_PACKAGE_MODEL_DEFAULT_DELIVERY_COMPANY_CODE)]
		public string DefaultDeliveryCompanyCode { get; set; }
		/// <summary>送付先モデル削除フラグ</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_PACKAGE_MODEL_PACKAGE_DELETE_FLAG)]
		public int? PackageDeleteFlag { get; set; }
		/// <summary>送付者モデル</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_PACKAGE_MODEL_SENDER_MODEL)]
		public RakutenApiSender SenderModel { get; set; }
		/// <summary>商品モデルリスト</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_PACKAGE_MODEL_ITEM_MODEL_LIST)]
		public RakutenApiItem[] ItemModelList { get; set; }
		/// <summary>発送モデルリスト</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_PACKAGE_MODEL_SHIPPING_MODEL_LIST)]
		public RakutenApiShipping[] ShippingModelList { get; set; }
	}
}
