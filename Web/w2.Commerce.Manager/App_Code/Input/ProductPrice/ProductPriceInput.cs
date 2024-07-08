/*
=========================================================================================================
  Module      : 商品価格入力クラス (ProductPriceInput.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Collections;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Input;
using w2.Common.Util;
using w2.Domain.ProductPrice;

/// <summary>
/// 商品価格入力クラス
/// </summary>
[Serializable]
public class ProductPriceInput : InputBase<ProductPriceModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public ProductPriceInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public ProductPriceInput(ProductPriceModel model)
		: this()
	{
		this.ShopId = model.ShopId;
		this.ProductId = model.ProductId;
		this.VariationId = model.VariationId;
		this.MemberRankId = model.MemberRankId;
		this.MemberRankPrice = StringUtility.ToEmpty(model.MemberRankPrice);
		this.IsSetProductPrice = model.IsSetProductPrice;
		this.LastChanged = LastChanged;
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="source">ソース</param>
	public ProductPriceInput(Hashtable source)
	{
		this.DataSource = source;
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override ProductPriceModel CreateModel()
	{
		var model = new ProductPriceModel
		{
			ShopId = StringUtility.ToEmpty(this.ShopId),
			ProductId = StringUtility.ToEmpty(this.ProductId),
			VariationId = StringUtility.ToEmpty(this.VariationId),
			MemberRankId = StringUtility.ToEmpty(this.MemberRankId),
			MemberRankPrice = ObjectUtility.TryParseDecimal(this.MemberRankPrice, 0m),
			IsSetProductPrice = IsSetProductPrice,
			LastChanged = StringUtility.ToEmpty(this.LastChanged)
		};
		return model;
	}

	/// <summary>
	/// Validate
	/// </summary>
	/// <returns>Error message</returns>
	public string Validate()
	{
		var errorMessage = Validator.Validate("ProductPriceRegist", this.DataSource)
			.Replace("@@ 1 @@", StringUtility.ToEmpty(this.MemberRankPrice.ToPriceString()));
		return errorMessage;
	}
	#endregion

	#region プロパティ
	/// <summary>店舗ID</summary>
	[JsonProperty(Constants.FIELD_PRODUCTPRICE_SHOP_ID)]
	public string ShopId
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTPRICE_SHOP_ID]; }
		set { this.DataSource[Constants.FIELD_PRODUCTPRICE_SHOP_ID] = value; }
	}
	/// <summary>商品ID</summary>
	[JsonProperty(Constants.FIELD_PRODUCTPRICE_PRODUCT_ID)]
	public string ProductId
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTPRICE_PRODUCT_ID]; }
		set { this.DataSource[Constants.FIELD_PRODUCTPRICE_PRODUCT_ID] = value; }
	}
	/// <summary>商品バリエーションID</summary>
	[JsonProperty(Constants.FIELD_PRODUCTPRICE_VARIATION_ID)]
	public string VariationId
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTPRICE_VARIATION_ID]; }
		set { this.DataSource[Constants.FIELD_PRODUCTPRICE_VARIATION_ID] = value; }
	}
	/// <summary>会員ランクID</summary>
	[JsonProperty(Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_ID)]
	public string MemberRankId
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_ID]; }
		set { this.DataSource[Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_ID] = value; }
	}
	/// <summary>会員価格</summary>
	[JsonProperty(Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE)]
	public string MemberRankPrice
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE]; }
		set { this.DataSource[Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE] = value; }
	}
	/// <summary>Is set product price</summary>
	public bool IsSetProductPrice
	{
		get { return (bool)this.DataSource["is_set_product_price"]; }
		set { this.DataSource["is_set_product_price"] = value; }
	}
	/// <summary>最終更新者</summary>
	[JsonProperty(Constants.FIELD_PRODUCTPRICE_LAST_CHANGED)]
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTPRICE_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_PRODUCTPRICE_LAST_CHANGED] = value; }
	}
	#endregion
}