/*
=========================================================================================================
  Module      : 配送種別配送会社入力クラス (ShopShippingCompanyInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.App.Common.Input;
using w2.Domain.ShopShipping;

/// <summary>
/// 配送種別配送会社マスタ入力クラス
/// </summary>
[Serializable]
public class ShopShippingCompanyInput : InputBase<ShopShippingCompanyModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public ShopShippingCompanyInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public ShopShippingCompanyInput(ShopShippingCompanyModel model)
		: this()
	{
		this.ShippingId = model.ShippingId;
		this.ShippingKbn = model.ShippingKbn;
		this.DeliveryCompanyId = model.DeliveryCompanyId;
		this.DefaultDeliveryCompany = model.DefaultDeliveryCompany;
		this.DateCreated = model.DateCreated.ToString();
		this.DateChanged = model.DateChanged.ToString();
		this.LastChanged = model.LastChanged;
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override ShopShippingCompanyModel CreateModel()
	{
		var model = new ShopShippingCompanyModel
		{
			ShippingId = this.ShippingId,
			ShippingKbn = this.ShippingKbn,
			DeliveryCompanyId = this.DeliveryCompanyId,
			DefaultDeliveryCompany = this.DefaultDeliveryCompany,
			LastChanged = this.LastChanged,
		};
		return model;
	}

	/// <summary>
	/// 検証
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	public string Validate()
	{
		// TODO:Validatorチェック
		// var errorMessage = Validator.Validate("ShopShippingCompany", this.DataSource));
		// return errorMessage;
		return "";
	}
	#endregion

	#region プロパティ
	/// <summary>配送種別ID</summary>
	public string ShippingId
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPINGCOMPANY_SHIPPING_ID]; }
		set { this.DataSource[Constants.FIELD_SHOPSHIPPINGCOMPANY_SHIPPING_ID] = value; }
	}
	/// <summary>配送区分</summary>
	public string ShippingKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPINGCOMPANY_SHIPPING_KBN]; }
		set { this.DataSource[Constants.FIELD_SHOPSHIPPINGCOMPANY_SHIPPING_KBN] = value; }
	}
	/// <summary>配送会社ID</summary>
	public string DeliveryCompanyId
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPINGCOMPANY_DELIVERY_COMPANY_ID]; }
		set { this.DataSource[Constants.FIELD_SHOPSHIPPINGCOMPANY_DELIVERY_COMPANY_ID] = value; }
	}
	/// <summary>初期配送会社</summary>
	public string DefaultDeliveryCompany
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPINGCOMPANY_DEFAULT_DELIVERY_COMPANY]; }
		set { this.DataSource[Constants.FIELD_SHOPSHIPPINGCOMPANY_DEFAULT_DELIVERY_COMPANY] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPINGCOMPANY_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_SHOPSHIPPINGCOMPANY_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPINGCOMPANY_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_SHOPSHIPPINGCOMPANY_DATE_CHANGED] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_SHOPSHIPPINGCOMPANY_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_SHOPSHIPPINGCOMPANY_LAST_CHANGED] = value; }
	}
	#endregion
}
