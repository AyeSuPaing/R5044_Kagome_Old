/*
=========================================================================================================
  Module      : 配送料入力クラス (ShippingDeliveryPostageInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using w2.App.Common.Input;
using w2.Domain.ShopShipping;

/// <summary>
/// 配送料マスタ入力クラス
/// </summary>
public class ShippingDeliveryPostageInput : InputBase<ShippingDeliveryPostageModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public ShippingDeliveryPostageInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public ShippingDeliveryPostageInput(ShippingDeliveryPostageModel model)
		: this()
	{
		this.ShopId = model.ShopId;
		this.ShippingId = model.ShippingId;
		this.DeliveryCompanyId = model.DeliveryCompanyId;
		this.ShippingPriceKbn = model.ShippingPriceKbn;
		this.ShippingFreePriceFlg = model.ShippingFreePriceFlg;
		this.ShippingFreePrice = (model.ShippingFreePrice != null) ? model.ShippingFreePrice.ToString() : null;
		this.AnnounceFreeShippingFlg = model.AnnounceFreeShippingFlg;
		this.CalculationPluralKbn = model.CalculationPluralKbn;
		this.PluralShippingPrice = (model.PluralShippingPrice != null) ? model.PluralShippingPrice.ToString() : null;
		this.LastChanged = model.LastChanged;
		this.StorePickupFreePriceFlg = model.StorePickupFreePriceFlg;
		this.FreeShippingFee = model.FreeShippingFee != null ? model.FreeShippingFee.ToString() : null;
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override ShippingDeliveryPostageModel CreateModel()
	{
		var model = new ShippingDeliveryPostageModel
		{
			ShopId = this.ShopId,
			ShippingId = this.ShippingId,
			DeliveryCompanyId = this.DeliveryCompanyId,
			ShippingPriceKbn = this.ShippingPriceKbn,
			ShippingFreePriceFlg = this.ShippingFreePriceFlg,
			ShippingFreePrice = (this.ShippingFreePrice != null) ? decimal.Parse(this.ShippingFreePrice) : (decimal?)null,
			AnnounceFreeShippingFlg = this.AnnounceFreeShippingFlg,
			CalculationPluralKbn = this.CalculationPluralKbn,
			PluralShippingPrice = (this.PluralShippingPrice != null) ? decimal.Parse(this.PluralShippingPrice) : (decimal?)null,
			LastChanged = this.LastChanged,
			StorePickupFreePriceFlg = this.StorePickupFreePriceFlg,
			FreeShippingFee = this.FreeShippingFee != null ? decimal.Parse(this.FreeShippingFee) : (decimal?)null,
		};
		return model;
	}

	/// <summary>
	/// 検証
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	public string Validate()
	{
		var errorMessage = Validator.Validate("ShippingDeliveryPostage", this.DataSource);
		return errorMessage;
	}
	#endregion

	#region プロパティ
	/// <summary>店舗ID</summary>
	public string ShopId
	{
		get { return (string)this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_SHOP_ID]; }
		set { this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_SHOP_ID] = value; }
	}
	/// <summary>配送種別ID</summary>
	public string ShippingId
	{
		get { return (string)this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_SHIPPING_ID]; }
		set { this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_SHIPPING_ID] = value; }
	}
	/// <summary>配送会社ID</summary>
	public string DeliveryCompanyId
	{
		get { return (string)this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_DELIVERY_COMPANY_ID]; }
		set { this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_DELIVERY_COMPANY_ID] = value; }
	}
	/// <summary>配送料設定区分</summary>
	public string ShippingPriceKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_SHIPPING_PRICE_KBN]; }
		set { this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_SHIPPING_PRICE_KBN] = value; }
	}
	/// <summary>配送料無料購入金額設定フラグ</summary>
	public string ShippingFreePriceFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_SHIPPING_FREE_PRICE_FLG]; }
		set { this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_SHIPPING_FREE_PRICE_FLG] = value; }
	}
	/// <summary>配送料無料購入金額設定</summary>
	public string ShippingFreePrice
	{
		get { return (string)this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_SHIPPING_FREE_PRICE]; }
		set { this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_SHIPPING_FREE_PRICE] = value; }
	}
	/// <summary>配送料無料案内表示フラグ</summary>
	public string AnnounceFreeShippingFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_ANNOUNCE_FREE_SHIPPING_FLG]; }
		set { this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_ANNOUNCE_FREE_SHIPPING_FLG] = value; }
	}
	/// <summary>複数商品計算区分</summary>
	public string CalculationPluralKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_CALCULATION_PLURAL_KBN]; }
		set { this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_CALCULATION_PLURAL_KBN] = value; }
	}
	/// <summary>複数商品配送料</summary>
	public string PluralShippingPrice
	{
		get { return (string)this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_PLURAL_SHIPPING_PRICE]; }
		set { this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_PLURAL_SHIPPING_PRICE] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_DATE_CHANGED] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_LAST_CHANGED] = value; }
	}
	/// <summary>店舗受取時配送料無料フラグ</summary>
	public string StorePickupFreePriceFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_STOREPICKUP_FREE_PRICE_FLG]; }
		set { this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_STOREPICKUP_FREE_PRICE_FLG] = value; }
	}
	/// <summary>配送料無料時の請求料金</summary>
	public string FreeShippingFee
	{
		get { return (string)this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_FREE_SHIPPING_FEE]; }
		set { this.DataSource[Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_FREE_SHIPPING_FEE] = value; }
	}
	#endregion
}
