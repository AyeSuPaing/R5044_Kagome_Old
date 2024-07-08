/*
=========================================================================================================
  Module      : 商品在庫入力クラス (ProductStockInput.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Input;
using w2.Common.Util;
using w2.Domain.ProductStock;

/// <summary>
/// 商品在庫入力クラス
/// </summary>
[Serializable]
public class ProductStockInput : InputBase<ProductStockModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public ProductStockInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public ProductStockInput(ProductStockModel model)
		: this()
	{
		this.ShopId = model.ShopId;
		this.ProductId = model.ProductId;
		this.VariationId = model.VariationId;
		this.LastChanged = model.LastChanged;
		//在庫管理かつ在庫情報のない時に、InvalidCastExceptionが発生しないように
		try
		{
			this.Stock = StringUtility.ToEmpty(model.Stock);
			this.StockAlert = StringUtility.ToEmpty(model.StockAlert);
			this.Realstock = StringUtility.ToEmpty(model.Realstock);
			this.RealstockB = StringUtility.ToEmpty(model.RealstockB);
			this.RealstockC = StringUtility.ToEmpty(model.RealstockC);
			this.RealstockReserved = StringUtility.ToEmpty(model.RealstockReserved);
		}
		catch (Exception e)
		{
			this.Stock = ValueText.GetValueText(Constants.VALUETEXT_PARAM_PRODUCT, Constants.VALUETEXT_PARAM_PRODUCT_CONFIRM, Constants.VALUETEXT_PARAM_PRODUCT_CONFIRM_DATA_INCONSISTENCY);
			this.StockAlert = ValueText.GetValueText(Constants.VALUETEXT_PARAM_PRODUCT, Constants.VALUETEXT_PARAM_PRODUCT_CONFIRM, Constants.VALUETEXT_PARAM_PRODUCT_CONFIRM_DATA_INCONSISTENCY);
			this.Realstock = ValueText.GetValueText(Constants.VALUETEXT_PARAM_PRODUCT, Constants.VALUETEXT_PARAM_PRODUCT_CONFIRM, Constants.VALUETEXT_PARAM_PRODUCT_CONFIRM_DATA_INCONSISTENCY);
			this.RealstockB = ValueText.GetValueText(Constants.VALUETEXT_PARAM_PRODUCT, Constants.VALUETEXT_PARAM_PRODUCT_CONFIRM, Constants.VALUETEXT_PARAM_PRODUCT_CONFIRM_DATA_INCONSISTENCY);
			this.RealstockC = ValueText.GetValueText(Constants.VALUETEXT_PARAM_PRODUCT, Constants.VALUETEXT_PARAM_PRODUCT_CONFIRM, Constants.VALUETEXT_PARAM_PRODUCT_CONFIRM_DATA_INCONSISTENCY);
			this.RealstockReserved = ValueText.GetValueText(Constants.VALUETEXT_PARAM_PRODUCT, Constants.VALUETEXT_PARAM_PRODUCT_CONFIRM, Constants.VALUETEXT_PARAM_PRODUCT_CONFIRM_DATA_INCONSISTENCY);
		}
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override ProductStockModel CreateModel()
	{
		var model = new ProductStockModel
		{
			ShopId = this.ShopId,
			ProductId = this.ProductId,
			VariationId = this.VariationId,
			Stock = ObjectUtility.TryParseInt(this.Stock, 0),
			StockAlert = ObjectUtility.TryParseInt(this.StockAlert, 0),
			Realstock = ObjectUtility.TryParseInt(this.Realstock, 0),
			RealstockB = ObjectUtility.TryParseInt(this.RealstockB, 0),
			RealstockC = ObjectUtility.TryParseInt(this.RealstockC, 0),
			RealstockReserved = ObjectUtility.TryParseInt(this.RealstockReserved, 0),
			LastChanged = this.LastChanged
		};
		return model;
	}
	#endregion

	#region プロパティ
	/// <summary>店舗ID</summary>
	public string ShopId
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCK_SHOP_ID]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCK_SHOP_ID] = value; }
	}
	/// <summary>商品ID</summary>
	public string ProductId
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID] = value; }
	}
	/// <summary>商品バリエーションID</summary>
	public string VariationId
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCK_VARIATION_ID]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCK_VARIATION_ID] = value; }
	}
	/// <summary>商品在庫数</summary>
	public string Stock
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCK_STOCK]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCK_STOCK] = value; }
	}
	/// <summary>商品在庫安全基準</summary>
	public string StockAlert
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCK_STOCK_ALERT]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCK_STOCK_ALERT] = value; }
	}
	/// <summary>実在庫数</summary>
	public string Realstock
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCK_REALSTOCK]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCK_REALSTOCK] = value; }
	}
	/// <summary>実在庫数B</summary>
	public string RealstockB
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCK_REALSTOCK_B]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCK_REALSTOCK_B] = value; }
	}
	/// <summary>実在庫数C</summary>
	public string RealstockC
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCK_REALSTOCK_C]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCK_REALSTOCK_C] = value; }
	}
	/// <summary>引当済実在庫数</summary>
	public string RealstockReserved
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCK_REALSTOCK_RESERVED]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCK_REALSTOCK_RESERVED] = value; }
	}
	/// <summary>在庫更新メモ</summary>
	public string UpdateMemo
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_MEMO]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_MEMO] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCK_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCK_LAST_CHANGED] = value; }
	}
	#endregion
}
