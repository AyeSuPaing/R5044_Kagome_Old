/*
=========================================================================================================
  Module      : 定期変更元商品入力クラス (FixedPurchaseBeforeChangeItemInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Input;
using w2.App.Common.Order;
using w2.Domain.FixedPurchaseProductChangeSetting;
using w2.Domain.FixedPurchaseProductChangeSetting.Helper;

/// <summary>
/// 定期変更元商品入力クラス
/// </summary>
[Serializable]
public class FixedPurchaseBeforeChangeItemInput : InputBase<FixedPurchaseBeforeChangeItemModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public FixedPurchaseBeforeChangeItemInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">定期変更元商品モデル</param>
	public FixedPurchaseBeforeChangeItemInput(FixedPurchaseBeforeChangeItemContainer model)
	{
		this.FixedPurchaseProductChangeId = model.FixedPurchaseProductChangeId;
		this.ItemUnitType = model.ItemUnitType;
		this.ShopId = model.ShopId;
		this.ProductId = model.ProductId;
		this.VariationId = model.VariationId;
		this.ProductName = model.ProductName;
		this.LastChanged = model.LastChanged;
		this.ShippingType = model.ShippingType;
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>定期変更元商品モデル</returns>
	public override FixedPurchaseBeforeChangeItemModel CreateModel()
	{
		return new FixedPurchaseBeforeChangeItemModel
		{
			FixedPurchaseProductChangeId = this.FixedPurchaseProductChangeId,
			ItemUnitType = this.ItemUnitType,
			ShopId = this.ShopId,
			ProductId = this.ProductId,
			VariationId = this.VariationId,
			LastChanged = this.LastChanged,
		};
	}

	/// <summary>
	/// 商品有効性チェック
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	public string CheckValidProduct()
	{
		var product = ProductCommon.GetProductInfoUnuseMemberRankPrice(this.ShopId, this.ProductId);
		if (product.Count == 0)
		{
			return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_DELETE).Replace("@@ 1 @@", this.ProductName);
		}
		if ((string)product[0][Constants.FIELD_PRODUCT_VALID_FLG] == Constants.FLG_PRODUCT_VALID_FLG_INVALID)
		{
			return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_INVALID).Replace("@@ 1 @@", this.ProductName);
		}
		if ((string)product[0][Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG] == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID)
		{
			return WebMessages.GetMessages(
				WebMessages.ERRMSG_MANAGER_FIXED_PURCHASE_CHANGE_ITEM_FIXED_PURCHASE_FLG_INVALID).Replace("@@ 1 @@", this.ProductId);
		}

		return string.Empty;
	}
	#endregion

	#region プロパティ
	/// <summary>定期商品変更ID</summary>
	public string FixedPurchaseProductChangeId
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEBEFORECHANGEITEM_FIXED_PURCHASE_PRODUCT_CHANGE_ID]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASEBEFORECHANGEITEM_FIXED_PURCHASE_PRODUCT_CHANGE_ID] = value; }
	}
	/// <summary>商品単位種別</summary>
	public string ItemUnitType
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEBEFORECHANGEITEM_ITEM_UNIT_TYPE]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASEBEFORECHANGEITEM_ITEM_UNIT_TYPE] = value; }
	}
	/// <summary>ショップID</summary>
	public string ShopId
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEBEFORECHANGEITEM_SHOP_ID]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASEBEFORECHANGEITEM_SHOP_ID] = value; }
	}
	/// <summary>商品ID</summary>
	public string ProductId
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEBEFORECHANGEITEM_PRODUCT_ID]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASEBEFORECHANGEITEM_PRODUCT_ID] = value; }
	}
	/// <summary>バリエーションID</summary>
	public string VariationId
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEBEFORECHANGEITEM_VARIATION_ID]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASEBEFORECHANGEITEM_VARIATION_ID] = value; }
	}
	/// <summary>商品名</summary>
	public string ProductName
	{
		get { return (string)this.DataSource["product_name"]; }
		set { this.DataSource["product_name"] = value; }
	}
	/// <summary>配送種別</summary>
	public string ShippingType
	{
		get { return (string)this.DataSource["shipping_type"]; }
		set { this.DataSource["shipping_type"] = value; }
	}
	/// <summary>商品単位種別がバリエーションか</summary>
	public bool IsVariationItemUnitType
	{
		get
		{
			return this.ItemUnitType == Constants.FLG_FIXEDPURCHASEAFTERPRODUCTCHANGEITEM_ITEM_UNIT_TYPE_VARIATION;
		}
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEBEFORECHANGEITEM_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_FIXEDPURCHASEBEFORECHANGEITEM_LAST_CHANGED] = value; }
	}
	#endregion
}
