/*
=========================================================================================================
  Module      : 商品定期購入割引設定詳細クラス (ProductFixedPurchaseDiscountSettingDetail.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using w2.Domain.ProductFixedPurchaseDiscountSetting;

/// <summary>
/// 商品定期購入割引設定 商品個数別割引設定クラス
/// </summary>
[Serializable]
public class ProductFixedPurchaseDiscountSettingDetail
{
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public ProductFixedPurchaseDiscountSettingDetail()
		: this(1, "1", "0", Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_YEN, "0", Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_POINT_TYPE_POINT)
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">ソース</param>
	public ProductFixedPurchaseDiscountSettingDetail(ProductFixedPurchaseDiscountSettingModel model)
		: this(1, model.ProductCount.ToString(), model.DiscountValue.ToString(), model.DiscountType, model.PointValue.ToString(), model.PointType)
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="columnNo">列番号</param>
	/// <param name="model">ソース</param>
	public ProductFixedPurchaseDiscountSettingDetail(ProductFixedPurchaseDiscountSettingModel model, int columnNo = 1)
		: this(columnNo, model.ProductCount.ToString(), model.DiscountValue.ToString(), model.DiscountType, model.PointValue.ToString(), model.PointType)
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="columnNo">列番号</param>
	/// <param name="productCount">商品個数</param>
	/// <param name="discountValue">値引</param>
	/// <param name="discountType">値引タイプ</param>
	/// <param name="pointValue">付与ポイント</param>
	/// <param name="pointType">付与ポイントタイプ</param>
	private ProductFixedPurchaseDiscountSettingDetail(
		int columnNo,
		string productCount,
		string discountValue,
		string discountType,
		string pointValue,
		string pointType)
	{
		this.ColumnNo = columnNo;
		this.ProductCount = productCount;
		this.DiscountValue = discountValue;
		this.DiscountType = discountType;
		this.PointValue = pointValue;
		this.PointType = pointType;
	}

	/// <summary>列番号</summary>
	public int ColumnNo { get; set; }
	/// <summary>商品個数</summary>
	[JsonProperty(Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_PRODUCT_COUNT)]
	public string ProductCount { get; set; }
	/// <summary>値引</summary>
	[JsonProperty(Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_VALUE)]
	public string DiscountValue { get; set; }
	/// <summary>値引タイプ</summary>
	[JsonProperty(Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE)]
	public string DiscountType { get; set; }
	/// <summary>付与ポイント</summary>
	[JsonProperty(Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_POINT_VALUE)]
	public string PointValue { get; set; }
	/// <summary>付与ポイントタイプ</summary>
	[JsonProperty(Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_POINT_TYPE)]
	public string PointType { get; set; }
}
