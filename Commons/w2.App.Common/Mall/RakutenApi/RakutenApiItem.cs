/*
=========================================================================================================
  Module      : 楽天ペイ受注情報取得API 商品モデル (RakutenApiItem.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System.Collections.Generic;
using w2.App.Common.Mall.Rakuten;

namespace w2.App.Common.Mall.RakutenApi
{
	/// <summary>
	/// 楽天ペイ受注API 注文商品モデル
	/// </summary>
	[JsonObject(Constants.RAKUTEN_PAY_API_PACKAGE_MODEL_ITEM_MODEL_LIST)]
	public class RakutenApiItem
	{
		/// <summary>商品明細ID</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ITEM_MODEL_ITEM_DETAIL_ID)]
		public string ItemDetailId { get; set; }
		/// <summary>商品名</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ITEM_MODEL_ITEM_NAME)]
		public string ItemName { get; set; }
		/// <summary>商品ID	</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ITEM_MODEL_ITEM_ID)]
		public string ItemId { get; set; }
		/// <summary>商品番号</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ITEM_MODEL_ITEM_NUMBER)]
		public string ItemNumber { get; set; }
		/// <summary>商品管理番号</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ITEM_MODEL_MANAGE_NUMBER)]
		public string ManageNumber { get; set; }
		/// <summary>単価</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ITEM_MODEL_PRICE)]
		public decimal Price { get; set; }
		/// <summary>個数</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ITEM_MODEL_UNITS)]
		public int Units { get; set; }
		/// <summary>送料込別</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ITEM_MODEL_INCLUDE_POSTAGE_FLAG)]
		public int? IncludePostageFlag { get; set; }
		/// <summary>税込別</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ITEM_MODEL_INCLUDE_TAX_FLAG)]
		public int? IncludeTaxFlag { get; set; }
		/// <summary>代引手数料込別</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ITEM_MODEL_INCLUDE_CASH_ON_DELIVERY_POSTAGE_FLAG)]
		public int? IncludeCashOnDeliveryPostageFlag { get; set; }
		/// <summary>項目・選択肢</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ITEM_MODEL_SELECTED_CHOICE)]
		public object SelectedChoice { get; set; }
		/// <summary>ポイント倍率</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ITEM_MODEL_POINT_RATE)]
		public int? PointRate { get; set; }
		/// <summary>ポイントタイプ</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ITEM_MODEL_POINT_TYPE)]
		public int? PointType { get; set; }
		/// <summary>在庫タイプ</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ITEM_MODEL_INVENTORY_TYPE)]
		public int? InventoryType { get; set; }
		/// <summary>納期情報</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ITEM_MODEL_DELVDATE_INFO)]
		public string DelvdateInfo { get; set; }
		/// <summary>在庫連動オプション</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ITEM_MODEL_RESTORE_INVENTORY_FLAG)]
		public int? RestoreInventoryFlag { get; set; }
		/// <summary>楽天スーパーDEAL商品フラグ</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ITEM_MODEL_DEAL_FLAG)]
		public int? DealFlag { get; set; }
		/// <summary>医薬品フラグ</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ITEM_MODEL_DRUG_FLAG)]
		public int? DrugFlag { get; set; }
		/// <summary>商品削除フラグ</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ITEM_MODEL_DELETE_ITEM_FLAG)]
		public int? DeleteItemFlag { get; set; }
		/// <summary>商品税率</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ITEM_MODEL_TAX_RATE)]
		public decimal TaxRate { get; set; }
		/// <summary>商品毎税込価格</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ITEM_MODEL_PRICE_TAX_INCL)]
		public decimal PriceTaxIncl { get; set; }
		/// <summary>単品配送フラグ</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_ITEM_MODEL_IS_SINGLE_ITEM_SHIPPING)]
		public decimal IsSingleItemShipping { get; set; }
		/// <summary>SKUモデルリスト</summary>
		[JsonProperty(Constants.RAKUTEN_ORDER_API_RESPONSE_ORDER_SKU_MODEL_LIST)]
		public List<SkuModel> SkuModelList { get; set; }
	}
}
