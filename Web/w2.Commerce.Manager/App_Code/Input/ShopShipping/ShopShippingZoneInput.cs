/*
=========================================================================================================
  Module      : 店舗配送料地帯入力クラス (ShopShippingZoneInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Collections;
using w2.App.Common.Input;
using w2.Domain.ShopShipping;

/// <summary>
/// 店舗配送料地帯マスタ入力クラス
/// </summary>
public class ShopShippingZoneInput : InputBase<ShopShippingZoneModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public ShopShippingZoneInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public ShopShippingZoneInput(ShopShippingZoneModel model)
		: this()
	{
		this.ShopId = model.ShopId;
		this.ShippingId = model.ShippingId;
		this.DeliveryCompanyId = model.DeliveryCompanyId;
		this.ShippingZoneNo = model.ShippingZoneNo.ToString();
		this.ShippingZoneName = model.ShippingZoneName;
		this.Zip = model.Zip;
		this.SizeXxsShippingPrice = model.SizeXxsShippingPrice.ToString();
		this.SizeXsShippingPrice = model.SizeXsShippingPrice.ToString();
		this.SizeSShippingPrice = model.SizeSShippingPrice.ToString();
		this.SizeMShippingPrice = model.SizeMShippingPrice.ToString();
		this.SizeLShippingPrice = model.SizeLShippingPrice.ToString();
		this.SizeXlShippingPrice = model.SizeXlShippingPrice.ToString();
		this.SizeXxlShippingPrice = model.SizeXxlShippingPrice.ToString();
		this.DelFlg = model.DelFlg;
		this.LastChanged = model.LastChanged;
		this.SizeMailShippingPrice = model.SizeMailShippingPrice.ToString();
		this.ConditionalShippingPriceThreshold = model.ConditionalShippingPriceThreshold.ToString();
		this.ConditionalShippingPrice = model.ConditionalShippingPrice.ToString();
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override ShopShippingZoneModel CreateModel()
	{
		var model = new ShopShippingZoneModel
		{
			ShopId = this.ShopId,
			ShippingId = this.ShippingId,
			DeliveryCompanyId = this.DeliveryCompanyId,
			ShippingZoneNo = int.Parse(this.ShippingZoneNo),
			ShippingZoneName = this.ShippingZoneName,
			Zip = this.Zip,
			SizeXxsShippingPrice = decimal.Parse(this.SizeXxsShippingPrice),
			SizeXsShippingPrice = decimal.Parse(this.SizeXsShippingPrice),
			SizeSShippingPrice = decimal.Parse(this.SizeSShippingPrice),
			SizeMShippingPrice = decimal.Parse(this.SizeMShippingPrice),
			SizeLShippingPrice = decimal.Parse(this.SizeLShippingPrice),
			SizeXlShippingPrice = decimal.Parse(this.SizeXlShippingPrice),
			SizeXxlShippingPrice = decimal.Parse(this.SizeXxlShippingPrice),
			DelFlg = this.DelFlg,
			LastChanged = this.LastChanged,
			SizeMailShippingPrice = decimal.Parse(this.SizeMailShippingPrice),
			ConditionalShippingPriceThreshold = string.IsNullOrEmpty(this.ConditionalShippingPriceThreshold) ? (decimal?)null : decimal.Parse(this.ConditionalShippingPriceThreshold),
			ConditionalShippingPrice = string.IsNullOrEmpty(this.ConditionalShippingPrice) ? (decimal?)null : decimal.Parse(this.ConditionalShippingPrice),
		};
		return model;
	}

	/// <summary>
	/// 検証
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	public string Validate()
	{
		var input = (Hashtable)this.DataSource.Clone();
		if (string.IsNullOrEmpty(this.ConditionalShippingPriceThreshold)
			&& string.IsNullOrEmpty(this.ConditionalShippingPrice))
		{
			input.Remove(Constants.FIELD_SHOPSHIPPINGZONE_CONDITIONAL_SHIPPING_PRICE_THRESHOLD);
			input.Remove(Constants.FIELD_SHOPSHIPPINGZONE_CONDITIONAL_SHIPPING_PRICE);
		}

		var errorMessage = Validator.Validate("ShopShippingZone", input);
		return errorMessage;
	}
	#endregion

	#region プロパティ
	/// <summary>店舗ID</summary>
	public string ShopId
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SHOP_ID]; }
		set { this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SHOP_ID] = value; }
	}
	/// <summary>配送料設定ID</summary>
	public string ShippingId
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ID]; }
		set { this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ID] = value; }
	}
	/// <summary>配送会社ID</summary>
	public string DeliveryCompanyId
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_DELIVERY_COMPANY_ID]; }
		set { this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_DELIVERY_COMPANY_ID] = value; }
	}
	/// <summary>配送料地帯区分</summary>
	public string ShippingZoneNo
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ZONE_NO]; }
		set { this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ZONE_NO] = value; }
	}
	/// <summary>地帯名</summary>
	public string ShippingZoneName
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ZONE_NAME]; }
		set { this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ZONE_NAME] = value; }
	}
	/// <summary>郵便番号</summary>
	public string Zip
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_ZIP]; }
		set { this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_ZIP] = value; }
	}
	/// <summary>XXSサイズ商品配送料</summary>
	public string SizeXxsShippingPrice
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SIZE_XXS_SHIPPING_PRICE]; }
		set { this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SIZE_XXS_SHIPPING_PRICE] = value; }
	}
	/// <summary>XSサイズ商品配送料</summary>
	public string SizeXsShippingPrice
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SIZE_XS_SHIPPING_PRICE]; }
		set { this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SIZE_XS_SHIPPING_PRICE] = value; }
	}
	/// <summary>Sサイズ商品配送料</summary>
	public string SizeSShippingPrice
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SIZE_S_SHIPPING_PRICE]; }
		set { this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SIZE_S_SHIPPING_PRICE] = value; }
	}
	/// <summary>Mサイズ商品配送料</summary>
	public string SizeMShippingPrice
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SIZE_M_SHIPPING_PRICE]; }
		set { this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SIZE_M_SHIPPING_PRICE] = value; }
	}
	/// <summary>Lサイズ商品配送料</summary>
	public string SizeLShippingPrice
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SIZE_L_SHIPPING_PRICE]; }
		set { this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SIZE_L_SHIPPING_PRICE] = value; }
	}
	/// <summary>XLサイズ商品配送料</summary>
	public string SizeXlShippingPrice
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SIZE_XL_SHIPPING_PRICE]; }
		set { this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SIZE_XL_SHIPPING_PRICE] = value; }
	}
	/// <summary>XXLサイズ商品配送料</summary>
	public string SizeXxlShippingPrice
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SIZE_XXL_SHIPPING_PRICE]; }
		set { this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SIZE_XXL_SHIPPING_PRICE] = value; }
	}
	/// <summary>削除フラグ</summary>
	public string DelFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_DEL_FLG]; }
		set { this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_DEL_FLG] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_DATE_CHANGED] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_LAST_CHANGED] = value; }
	}
	/// <summary>メールサイズ商品配送料</summary>
	public string SizeMailShippingPrice
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SIZE_MAIL_SHIPPING_PRICE]; }
		set { this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_SIZE_MAIL_SHIPPING_PRICE] = value; }
	}
	/// <summary>条件付き配送料閾値</summary>
	public string ConditionalShippingPriceThreshold
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_CONDITIONAL_SHIPPING_PRICE_THRESHOLD]; }
		set { this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_CONDITIONAL_SHIPPING_PRICE_THRESHOLD] = value; }
	}
	/// <summary>条件付き配送料</summary>
	public string ConditionalShippingPrice
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_CONDITIONAL_SHIPPING_PRICE]; }
		set { this.DataSource[Constants.FIELD_SHOPSHIPPINGZONE_CONDITIONAL_SHIPPING_PRICE] = value; }
	}
	#endregion
}
