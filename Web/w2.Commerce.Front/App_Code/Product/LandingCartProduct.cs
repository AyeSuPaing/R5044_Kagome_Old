/*
=========================================================================================================
  Module      : ランディングカート商品一覧クラス(LandingCartProduct.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using System.Collections;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Translation;
using w2.App.Common.Order;

/// <summary>
/// LandingCartProduct の概要の説明です
/// </summary>
[Serializable]
public class LandingCartProduct
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public LandingCartProduct()
	{
		// 何もしない
	}

	/// <summary>
	/// 商品結合翻訳名設定
	/// </summary>
	private string GetProductJointTranslationName()
	{
		// マスタに登録されている名称を取得
		var productJointName = ProductCommon.CreateProductJointName(this.Product);

		// 言語コード取得
		string languageCode = RegionManager.GetInstance().Region.LanguageCode;
		string languageLocaleId = RegionManager.GetInstance().Region.LanguageLocaleId;

		// 翻訳名取得
		var translationSettings = NameTranslationCommon.GetProductAndVariationTranslationSettingsByProductId(
			this.ProductId,
			languageCode,
			languageLocaleId);

		if (translationSettings.Any() == false) return productJointName;

		var productJointTranslationName = NameTranslationCommon.CreateProductJointTranslationName(
			translationSettings,
			(string)this.Product[Constants.FIELD_PRODUCT_NAME],
			(string)this.Product[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1],
			(string)this.Product[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2],
			(string)this.Product[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3],
			true);

		return productJointTranslationName;
	}

	/// <summary>インデックス</summary>
	public int Index { get; set; }
	/// <summary>数量</summary>
	public int ProductCount { get; set; }
	/// <summary>選択されているか</summary>
	public bool Selected { get; set; }
	/// <summary>商品情報</summary>
	public Hashtable Product { get; set; }
	/// <summary>定期商品か</summary>
	public bool IsFixedPurchase { get; set; }
	/// <summary>頒布会商品か</summary>
	public bool IsSubScriptionBox { get; set; }

	/// <summary>店舗ID</summary>
	public string ShopId { get { return (string)this.Product[Constants.FIELD_PRODUCT_SHOP_ID]; } }
	/// <summary>商品ID</summary>
	public string ProductId { get { return (string)this.Product[Constants.FIELD_PRODUCT_PRODUCT_ID]; } }
	/// <summary>バリエーションID</summary>
	public string VariationId { get { return (string)this.Product[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]; } }
	/// <summary>商品名（バリエーションなどを連結）</summary>
	public string ProductJointName
	{
		get
		{
			var productJointName = Constants.GLOBAL_OPTION_ENABLE
				? GetProductJointTranslationName()
				: ProductCommon.CreateProductJointName(this.Product);
			if (this.IsFixedPurchase)
			{
				productJointName = string.Format(
					"{0}{1}",
					this.IsSubScriptionBox
						? Constants.PRODUCT_SUBSCRIPTION_BOX_STRING
						: Constants.PRODUCT_FIXED_PURCHASE_STRING,
					productJointName);
			}
			return productJointName;
		}
	}
	/// <summary>単価</summary>
	public decimal Price
	{
		get
		{
			return this.IsFixedPurchase ? decimal.Parse(ProductPrice.GetFixedPurchasePrice(this.Product, true, true)) : ProductPage.GetProductValidityPrice(this.Product, true, true);
		}
	}
	/// <summary>税率</summary>
	public decimal TaxRate { get { return (decimal)this.Product[Constants.FIELD_PRODUCTTAXCATEGORY_TAX_RATE]; } }
	/// <summary>最大同時購入可能数</summary>
	public int MaxSellQuantity { get { return (int)this.Product[Constants.FIELD_PRODUCT_MAX_SELL_QUANTITY]; } }
	// <summary>頒布会コースID</summary>
	public string SubscriptionBoxCourseId
	{
		get { return (string)this.Product[Constants.FIELD_ORDER_SUBSCRIPTION_BOX_COURSE_ID]; }
	}
}