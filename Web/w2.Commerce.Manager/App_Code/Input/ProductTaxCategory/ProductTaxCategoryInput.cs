/*
=========================================================================================================
  Module      : 商品税率カテゴリ入力クラス (ProductTaxCategoryInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.App.Common.Input;
using w2.Domain.ProductTaxCategory;

/// <summary>
/// 商品税率カテゴリマスタ入力クラス
/// </summary>
[Serializable]
public class ProductTaxCategoryInput : InputBase<ProductTaxCategoryModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public ProductTaxCategoryInput()
	{
		this.RegisteredKbn = Constants.FLG_PRODUCT_TAXCATEGORY_KBN_NOT_REGISTERED;
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public ProductTaxCategoryInput(ProductTaxCategoryModel model)
		: this()
	{
		this.TaxCategoryId = model.TaxCategoryId;
		this.TaxCategoryName = model.TaxCategoryName;
		this.TaxRate = model.TaxRate.ToString();
		this.DisplayOrder = model.DisplayOrder.ToString();
		this.DateCreated = model.DateCreated.ToString();
		this.DateChanged = model.DateChanged.ToString();
		this.LastChanged = model.LastChanged;
		this.RegisteredKbn = Constants.FLG_PRODUCT_TAXCATEGORY_KBN_REGISTERED;
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override ProductTaxCategoryModel CreateModel()
	{
		var model = new ProductTaxCategoryModel
		{
			TaxCategoryId = this.TaxCategoryId,
			TaxCategoryName = this.TaxCategoryName,
			TaxRate = decimal.Parse(this.TaxRate),
			DisplayOrder = int.Parse(this.DisplayOrder),
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
		// デフォルト税率カテゴリが表示順validate処理を通貨するために表示順を加工
		if (this.TaxCategoryId == Constants.DEFAULT_PRODUCT_TAXCATEGORY_ID)
			this.DisplayOrder = "1";

		var errorMessage = Validator.Validate("ProductTaxCategory", this.DataSource);

		if (this.TaxCategoryId == Constants.DEFAULT_PRODUCT_TAXCATEGORY_ID)
			this.DisplayOrder = "0";
		return errorMessage;
	}
	#endregion

	#region プロパティ
	/// <summary>税率カテゴリID</summary>
	public string TaxCategoryId
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTTAXCATEGORY_TAX_CATEGORY_ID]; }
		set { this.DataSource[Constants.FIELD_PRODUCTTAXCATEGORY_TAX_CATEGORY_ID] = value; }
	}
	/// <summary>税率カテゴリ名</summary>
	public string TaxCategoryName
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTTAXCATEGORY_TAX_CATEGORY_NAME]; }
		set { this.DataSource[Constants.FIELD_PRODUCTTAXCATEGORY_TAX_CATEGORY_NAME] = value; }
	}
	/// <summary>税率</summary>
	public string TaxRate
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTTAXCATEGORY_TAX_RATE]; }
		set { this.DataSource[Constants.FIELD_PRODUCTTAXCATEGORY_TAX_RATE] = value; }
	}
	/// <summary>表示順</summary>
	public string DisplayOrder
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTTAXCATEGORY_DISPLAY_ORDER]; }
		set { this.DataSource[Constants.FIELD_PRODUCTTAXCATEGORY_DISPLAY_ORDER] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTTAXCATEGORY_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_PRODUCTTAXCATEGORY_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTTAXCATEGORY_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_PRODUCTTAXCATEGORY_DATE_CHANGED] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTTAXCATEGORY_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_PRODUCTTAXCATEGORY_LAST_CHANGED] = value; }
	}
	/// <summary>登録済みか</summary>
	public string RegisteredKbn { get; set; }
	/// <summary>登録済みか</summary>
	public bool IsRegistered
	{
		get { return this.RegisteredKbn == Constants.FLG_PRODUCT_TAXCATEGORY_KBN_REGISTERED; }
	}
	#endregion
}
