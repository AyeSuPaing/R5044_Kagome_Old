/*
=========================================================================================================
  Module      : 商品定期購入割引設定 ヘッダークラス (ProductFixedPurchaseDiscountSettingHeader.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using w2.Domain.ProductFixedPurchaseDiscountSetting;

/// <summary>
/// 商品定期購入割引設定 注文回数別割引設定クラス
/// </summary>
[Serializable]
public class ProductFixedPurchaseDiscountSettingHeader
{
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public ProductFixedPurchaseDiscountSettingHeader()
		: this(1, string.Empty, string.Empty, "0", Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_ORDER_COUNT_MORE_THAN_FLG_INVALID, null)
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">ソース</param>
	public ProductFixedPurchaseDiscountSettingHeader(ProductFixedPurchaseDiscountSettingModel model, int rowNo = 1, ProductFixedPurchaseDiscountSettingDetail detail = null)
		: this(rowNo, model.ShopId, model.ProductId, model.OrderCount.ToString(), model.OrderCountMoreThanFlg, detail)
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="rowNo">行番号</param>
	/// <param name="shopId">店舗ID</param>
	/// <param name="productId">商品ID</param>
	/// <param name="orderCount">購入回数</param>
	/// <param name="orderCountMoreThanFlg">購入回数以降フラグ</param>
	/// <param name="detail">詳細割引設定</param>
	private ProductFixedPurchaseDiscountSettingHeader(
		int rowNo,
		string shopId,
		string productId,
		string orderCount,
		string orderCountMoreThanFlg,
		ProductFixedPurchaseDiscountSettingDetail detail)
	{
		this.RowNo = rowNo;
		this.ShopId = shopId;
		this.ProductId = productId;
		this.OrderCount = orderCount;
		this.OrderCountMoreThanFlg = orderCountMoreThanFlg;
		if (detail == null)
		{
			this.ProductCountDiscounts = null;
		}
		else
		{
			this.ProductCountDiscounts = new List<ProductFixedPurchaseDiscountSettingDetail>();
			this.ProductCountDiscounts.Add(detail);
		}
		
	}

	/// <summary>
	/// モデルからヘッダーインスタンス生成
	/// </summary>
	/// <param name="models">モデル配列</param>
	/// <returns>定期購入割引ヘッダーリスト</returns>
	public static List<ProductFixedPurchaseDiscountSettingHeader> CreateProductFixedPurchaseDiscountSettingHeader(ProductFixedPurchaseDiscountSettingModel[] models)
	{
		var headerList = new List<ProductFixedPurchaseDiscountSettingHeader>();

		var rowNo = 1;
		foreach (var orderCountGroup in models.GroupBy(m => m.OrderCount))
		{
			var orderCountFirst = orderCountGroup.First();
			var header = new ProductFixedPurchaseDiscountSettingHeader
			{
				RowNo = rowNo++,
				ShopId = orderCountFirst.ShopId,
				ProductId = orderCountFirst.ProductId,
				OrderCount = orderCountGroup.Key.ToString(),
				OrderCountMoreThanFlg = orderCountFirst.OrderCountMoreThanFlg
			};

			var details = new List<ProductFixedPurchaseDiscountSettingDetail>();
			var colNo = 1;
			foreach (var productCountGroup in models.GroupBy(m => m.ProductCount))
			{
				var model = orderCountGroup.Where(m => m.ProductCount == productCountGroup.Key).ToArray();

				ProductFixedPurchaseDiscountSettingDetail detail;
				if (model.Length != 0)
				{
					detail = new ProductFixedPurchaseDiscountSettingDetail
					{
						ColumnNo = colNo++,
						ProductCount = model[0].ProductCount.ToString(),
						DiscountValue = model[0].DiscountValue.ToString(),
						DiscountType = model[0].DiscountType,
						PointValue = model[0].PointValue.ToString(),
						PointType = model[0].PointType
					};
				}
				else
				{
					// 不足している列分のdetail作成
					detail = new ProductFixedPurchaseDiscountSettingDetail
					{
						ColumnNo = colNo++,
						ProductCount = productCountGroup.Key.ToString(),
						DiscountValue = "",
						DiscountType = Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_YEN,
						PointValue = "",
						PointType = Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_POINT_TYPE_POINT
					};
				}

				details.Add(detail);
			}

			header.ProductCountDiscounts = details;
			headerList.Add(header);
		}
		return headerList;
	}

	/// <summary>
	/// モデル配列生成
	/// </summary>
	/// <param name="pfpDiscountSettingOrderCountList">ソース</param>
	/// <param name="lastChanged">最終更新者</param>
	/// <returns>モデル配列</returns>
	public static ProductFixedPurchaseDiscountSettingModel[] CreateProductFixedPurchaseDiscountSetting(
		List<ProductFixedPurchaseDiscountSettingHeader> pfpDiscountSettingOrderCountList,
		string lastChanged)
	{
		var models = new List<ProductFixedPurchaseDiscountSettingModel>();
		foreach (var oc in pfpDiscountSettingOrderCountList)
		{
			foreach (var pc in oc.ProductCountDiscounts)
			{
				// 値引きおよびポイント設定がされていない場合、レコードを追加しない
				if (string.IsNullOrEmpty(pc.DiscountValue) && string.IsNullOrEmpty(pc.PointValue)) continue;

				var model = new ProductFixedPurchaseDiscountSettingModel();
				model.ShopId = oc.ShopId;
				model.ProductId = oc.ProductId;
				model.OrderCount = int.Parse(oc.OrderCount);
				model.OrderCountMoreThanFlg = oc.OrderCountMoreThanFlg;
				model.ProductCount = int.Parse(pc.ProductCount);
				model.LastChanged = lastChanged;
				model.DiscountValue = (string.IsNullOrEmpty(pc.DiscountValue)) ? null : (decimal?)decimal.Parse(pc.DiscountValue);
				model.DiscountType = (string.IsNullOrEmpty(pc.DiscountValue)) ? null : pc.DiscountType;
				model.PointValue = (string.IsNullOrEmpty(pc.PointValue)) ? null : (decimal?)decimal.Parse(pc.PointValue);
				model.PointType = (string.IsNullOrEmpty(pc.PointValue)) ? null : pc.PointType;

				models.Add(model);
			}
		}
		return models.ToArray();
	}

	/// <summary>行番号</summary>
	public int RowNo { get; set; }
	/// <summary>店舗ID</summary>
	[JsonProperty(Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_SHOP_ID)]
	public string ShopId { get; set; }
	/// <summary>商品ID</summary>
	[JsonProperty(Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_PRODUCT_ID)]
	public string ProductId { get; set; }
	/// <summary>購入回数</summary>
	[JsonProperty(Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_ORDER_COUNT)]
	public string OrderCount { get; set; }
	/// <summary>購入回数以降フラグ</summary>
	[JsonProperty(Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_ORDER_COUNT_MORE_THAN_FLG)]
	public string OrderCountMoreThanFlg { get; set; }
	/// <summary>詳細割引設定</summary>
	[JsonProperty("ProductFixedPurchaseDiscountSettingDetail")]
	public List<ProductFixedPurchaseDiscountSettingDetail> ProductCountDiscounts { get; set; }
}