/*
=========================================================================================================
  Module      : 商品バリエーション入力クラス (ProductVariationInput.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Input;
using w2.App.Common.Web.Page;
using w2.Common.Util;
using w2.Domain.ProductPrice;
using w2.Domain.ProductVariation;

/// <summary>
/// 商品バリエーション入力クラス
/// </summary>
[Serializable]
public class ProductVariationInput : InputBase<ProductVariationModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public ProductVariationInput()
		: base()
	{
		this.ProductPrices = new ProductPriceInput[0];
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public ProductVariationInput(ProductVariationModel model)
		: this()
	{
		this.ShopId = model.ShopId;
		this.ProductId = model.ProductId;
		this.VariationId = model.VariationId;
		this.VariationCooperationId1 = model.VariationCooperationId1;
		this.VariationCooperationId2 = model.VariationCooperationId2;
		this.VariationCooperationId3 = model.VariationCooperationId3;
		this.VariationCooperationId4 = model.VariationCooperationId4;
		this.VariationCooperationId5 = model.VariationCooperationId5;
		this.VariationCooperationId6 = model.VariationCooperationId6;
		this.VariationCooperationId7 = model.VariationCooperationId7;
		this.VariationCooperationId8 = model.VariationCooperationId8;
		this.VariationCooperationId9 = model.VariationCooperationId9;
		this.VariationCooperationId10 = model.VariationCooperationId10;
		this.MallVariationId1 = model.MallVariationId1;
		this.MallVariationId2 = model.MallVariationId2;
		this.MallVariationType = model.MallVariationType;
		this.VariationName1 = model.VariationName1;
		this.VariationName2 = model.VariationName2;
		this.VariationName3 = model.VariationName3;
		this.Price = model.Price.ToPriceString();
		this.SpecialPrice = model.SpecialPrice.ToPriceString();
		this.VariationImageHead = model.VariationImageHead;
		this.VariationImageMobile = model.VariationImageMobile;
		this.ShippingType = model.ShippingType;
		this.ShippingSizeKbn = model.ShippingSizeKbn;
		this.DisplayOrder = StringUtility.ToEmpty(model.DisplayOrder);
		this.VariationMallCooperatedFlg = model.VariationMallCooperatedFlg;
		this.ValidFlg = model.ValidFlg;
		this.DelFlg = model.DelFlg;
		this.VariationDownloadUrl = model.VariationDownloadUrl;
		this.VariationFixedPurchaseFirstTimePrice = model.VariationFixedPurchaseFirsttimePrice.ToPriceString();
		this.VariationFixedPurchasePrice = model.VariationFixedPurchasePrice.ToPriceString();
		this.VariationAndMallReservationFlg = model.VariationAndmallReservationFlg;
		this.VariationColorId = model.VariationColorId;
		this.VariationWeightGram = StringUtility.ToEmpty(model.VariationWeightGram);
		this.LastChanged = model.LastChanged;
		this.VId = model.VId;
		this.ProductPrices = model.HasProductPrices
			? model.ProductPrices.Select(item => new ProductPriceInput(item)).ToArray()
			: new ProductPriceInput[0];
		this.ProductStock = (model.ProductStock != null)
			? new ProductStockInput(model.ProductStock)
			: null;
		this.VariationAddCartUrlLimitFlg = model.VariationAddCartUrlLimitFlg;
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="source">ソース</param>
	public ProductVariationInput(Hashtable source)
		: this(new ProductVariationModel())
	{
		// Copy input source values to current source
		foreach (DictionaryEntry item in source)
		{
			// If the value is null, then continue
			if (item.Value == null) continue;

			this.DataSource[item.Key] = item.Value;
		}
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override ProductVariationModel CreateModel()
	{
		var model = new ProductVariationModel
		{
			ShopId = StringUtility.ToEmpty(this.ShopId),
			ProductId = StringUtility.ToEmpty(this.ProductId),
			VariationId = StringUtility.ToEmpty(this.VariationId),
			VariationCooperationId1 = StringUtility.ToEmpty(this.VariationCooperationId1),
			VariationCooperationId2 = StringUtility.ToEmpty(this.VariationCooperationId2),
			VariationCooperationId3 = StringUtility.ToEmpty(this.VariationCooperationId3),
			VariationCooperationId4 = StringUtility.ToEmpty(this.VariationCooperationId4),
			VariationCooperationId5 = StringUtility.ToEmpty(this.VariationCooperationId5),
			VariationCooperationId6 = StringUtility.ToEmpty(this.VariationCooperationId6),
			VariationCooperationId7 = StringUtility.ToEmpty(this.VariationCooperationId7),
			VariationCooperationId8 = StringUtility.ToEmpty(this.VariationCooperationId8),
			VariationCooperationId9 = StringUtility.ToEmpty(this.VariationCooperationId9),
			VariationCooperationId10 = StringUtility.ToEmpty(this.VariationCooperationId10),
			MallVariationId1 = StringUtility.ToEmpty(this.MallVariationId1),
			MallVariationId2 = StringUtility.ToEmpty(this.MallVariationId2),
			MallVariationType = StringUtility.ToEmpty(this.MallVariationType),
			VariationName1 = StringUtility.ToEmpty(this.VariationName1),
			VariationName2 = StringUtility.ToEmpty(this.VariationName2),
			VariationName3 = StringUtility.ToEmpty(this.VariationName3),
			Price = ObjectUtility.TryParseDecimal(this.Price, 0),
			SpecialPrice = ObjectUtility.TryParseDecimal(this.SpecialPrice),
			VariationImageHead = StringUtility.ToEmpty(this.VariationImageHead),
			VariationImageMobile = StringUtility.ToEmpty(this.VariationImageMobile),
			ShippingType = StringUtility.ToEmpty(this.ShippingType),
			ShippingSizeKbn = StringUtility.ToEmpty(this.ShippingSizeKbn),
			DisplayOrder = ObjectUtility.TryParseInt(this.DisplayOrder, 0),
			VariationMallCooperatedFlg = StringUtility.ToEmpty(this.VariationMallCooperatedFlg),
			ValidFlg = StringUtility.ToEmpty(this.ValidFlg),
			DelFlg = StringUtility.ToEmpty(this.DelFlg),
			VariationDownloadUrl = StringUtility.ToEmpty(this.VariationDownloadUrl),
			VariationFixedPurchaseFirsttimePrice = ObjectUtility.TryParseDecimal(this.VariationFixedPurchaseFirstTimePrice),
			VariationFixedPurchasePrice = ObjectUtility.TryParseDecimal(this.VariationFixedPurchasePrice),
			VariationAndmallReservationFlg = StringUtility.ToEmpty(this.VariationAndMallReservationFlg),
			VariationColorId = StringUtility.ToEmpty(this.VariationColorId),
			VariationWeightGram = ObjectUtility.TryParseInt(this.VariationWeightGram, 0),
			LastChanged = StringUtility.ToEmpty(this.LastChanged),
			ProductPrices = this.HasProductPrices
				? this.ProductPrices.Select(item => item.CreateModel()).ToArray()
				: new ProductPriceModel[0],
			VariationAddCartUrlLimitFlg = this.VariationAddCartUrlLimitFlg
		};
		return model;
	}

	/// <summary>
	/// Validate
	/// </summary>
	/// <param name="variationNo">Variation no</param>
	/// <returns>Error messages as dictionary</returns>
	public Dictionary<string, string> Validate(int variationNo)
	{
		// For input variations
		var errorMessages = Validator.ValidateAndGetErrorContainer(
			"ProductVariationRegist",
			this.DataSource);

		if (errorMessages.Count != 0)
		{
			var errorKeys = new List<string>(errorMessages.Keys);
			foreach (var key in errorKeys)
			{
				var variationErrorMessage = string.Format(
					CommonPage.ReplaceTagByLocaleId(
						"@@DispText.product_error_message.variation@@",
						Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE),
					variationNo + 1);
				errorMessages[key] = errorMessages[key].Replace("@@ 1 @@", variationErrorMessage);
			}
		}

		// For case setting product prices
		if (this.HasProductPrices)
		{
			var productPriceErrors = CheckProductPrice();
			if (productPriceErrors.Count != 0)
			{
				errorMessages.Add(
					Constants.FIELD_PRODUCTVARIATION_PRODUCTPRICE_EXTEND,
					BasePageHelper.ConvertObjectToJsonString(productPriceErrors));
			}
		}
		return errorMessages;
	}

	/// <summary>
	/// Check product price
	/// </summary>
	/// <returns>Error messages as dictionary</returns>
	private Dictionary<string, string> CheckProductPrice()
	{
		var errorMessages = new Dictionary<string, string>();
		foreach (var item in this.ProductPrices)
		{
			var errorMessage = item.Validate();
			if (string.IsNullOrEmpty(errorMessage)) continue;

			errorMessages.Add(item.MemberRankId, errorMessage);
		}
		return errorMessages;
	}
	#endregion

	#region プロパティ
	/// <summary>店舗ID</summary>
	[JsonProperty(Constants.FIELD_PRODUCTVARIATION_SHOP_ID)]
	public string ShopId
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_SHOP_ID]; }
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_SHOP_ID] = value; }
	}
	/// <summary>商品ID</summary>
	[JsonProperty(Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID)]
	public string ProductId
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID]; }
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID] = value; }
	}
	/// <summary>商品バリエーションID</summary>
	[JsonProperty(Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)]
	public string VariationId
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]; }
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID] = value; }
	}
	/// <summary>商品バリエーション連携ID1</summary>
	[JsonProperty(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID1)]
	public string VariationCooperationId1
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID1]; }
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID1] = value; }
	}
	/// <summary>商品バリエーション連携ID2</summary>
	[JsonProperty(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID2)]
	public string VariationCooperationId2
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID2]; }
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID2] = value; }
	}
	/// <summary>商品バリエーション連携ID3</summary>
	[JsonProperty(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID3)]
	public string VariationCooperationId3
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID3]; }
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID3] = value; }
	}
	/// <summary>商品バリエーション連携ID4</summary>
	[JsonProperty(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID4)]
	public string VariationCooperationId4
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID4]; }
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID4] = value; }
	}
	/// <summary>商品バリエーション連携ID5</summary>
	[JsonProperty(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID5)]
	public string VariationCooperationId5
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID5]; }
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID5] = value; }
	}
	/// <summary>商品バリエーション連携ID6</summary>
	[JsonProperty(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID6)]
	public string VariationCooperationId6
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID6]; }
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID6] = value; }
	}
	/// <summary>商品バリエーション連携ID7</summary>
	[JsonProperty(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID7)]
	public string VariationCooperationId7
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID7]; }
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID7] = value; }
	}
	/// <summary>商品バリエーション連携ID8</summary>
	[JsonProperty(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID8)]
	public string VariationCooperationId8
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID8]; }
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID8] = value; }
	}
	/// <summary>商品バリエーション連携ID9</summary>
	[JsonProperty(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID9)]
	public string VariationCooperationId9
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID9]; }
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID9] = value; }
	}
	/// <summary>商品バリエーション連携ID10</summary>
	[JsonProperty(Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID10)]
	public string VariationCooperationId10
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID10]; }
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID10] = value; }
	}
	/// <summary>モールバリエーションID1</summary>
	[JsonProperty(Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_ID1)]
	public string MallVariationId1
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_ID1]; }
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_ID1] = value; }
	}
	/// <summary>モールバリエーションID2</summary>
	[JsonProperty(Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_ID2)]
	public string MallVariationId2
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_ID2]; }
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_ID2] = value; }
	}
	/// <summary>モールバリエーション種別</summary>
	[JsonProperty(Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_TYPE)]
	public string MallVariationType
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_TYPE]; }
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_MALL_VARIATION_TYPE] = value; }
	}
	/// <summary>バリエーション名1</summary>
	[JsonProperty(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1)]
	public string VariationName1
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1]; }
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1] = value; }
	}
	/// <summary>バリエーション名2</summary>
	[JsonProperty(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2)]
	public string VariationName2
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2]; }
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2] = value; }
	}
	/// <summary>バリエーション名3</summary>
	[JsonProperty(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3)]
	public string VariationName3
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3]; }
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3] = value; }
	}
	/// <summary>販売価格</summary>
	[JsonProperty(Constants.FIELD_PRODUCTVARIATION_PRICE)]
	public string Price
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_PRICE]; }
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_PRICE] = value; }
	}
	/// <summary>特別価格</summary>
	[JsonProperty(Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE)]
	public string SpecialPrice
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE]; }
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE] = value; }
	}
	/// <summary>バリエーション画像名ヘッダ</summary>
	[JsonProperty(Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD)]
	public string VariationImageHead
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD]; }
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD] = value; }
	}
	/// <summary>モバイルバリエーション画像名</summary>
	[JsonProperty(Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_MOBILE)]
	public string VariationImageMobile
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_MOBILE]; }
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_MOBILE] = value; }
	}
	/// <summary>配送料種別</summary>
	[JsonProperty(Constants.FIELD_PRODUCTVARIATION_SHIPPING_TYPE)]
	public string ShippingType
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_SHIPPING_TYPE]; }
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_SHIPPING_TYPE] = value; }
	}
	/// <summary>配送サイズ区分</summary>
	[JsonProperty(Constants.FIELD_PRODUCTVARIATION_SHIPPING_SIZE_KBN)]
	public string ShippingSizeKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_SHIPPING_SIZE_KBN]; }
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_SHIPPING_SIZE_KBN] = value; }
	}
	/// <summary>表示順</summary>
	[JsonProperty(Constants.FIELD_PRODUCTVARIATION_DISPLAY_ORDER)]
	public string DisplayOrder
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_DISPLAY_ORDER]; }
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_DISPLAY_ORDER] = value; }
	}
	/// <summary>バリエーションモール連携済フラグ</summary>
	[JsonProperty(Constants.FIELD_PRODUCTVARIATION_VARIATION_MALL_COOPERATED_FLG)]
	public string VariationMallCooperatedFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_MALL_COOPERATED_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_MALL_COOPERATED_FLG] = value; }
	}
	/// <summary>有効フラグ</summary>
	[JsonProperty(Constants.FIELD_PRODUCTVARIATION_VALID_FLG)]
	public string ValidFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VALID_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VALID_FLG] = value; }
	}
	/// <summary>削除フラグ</summary>
	[JsonProperty(Constants.FIELD_PRODUCTVARIATION_DEL_FLG)]
	public string DelFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_DEL_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_DEL_FLG] = value; }
	}
	/// <summary>ダウンロードURL</summary>
	[JsonProperty(Constants.FIELD_PRODUCTVARIATION_VARIATION_DOWNLOAD_URL)]
	public string VariationDownloadUrl
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_DOWNLOAD_URL]; }
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_DOWNLOAD_URL] = value; }
	}
	/// <summary>定期初回購入価格</summary>
	[JsonProperty(Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE)]
	public string VariationFixedPurchaseFirstTimePrice
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE]; }
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE] = value; }
	}
	/// <summary>定期購入価格</summary>
	[JsonProperty(Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE)]
	public string VariationFixedPurchasePrice
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE]; }
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE] = value; }
	}
	/// <summary>＆mallの予約商品フラグ</summary>
	[JsonProperty(Constants.FIELD_PRODUCTVARIATION_VARIATION_ANDMALL_RESERVATION_FLG)]
	public string VariationAndMallReservationFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_ANDMALL_RESERVATION_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_ANDMALL_RESERVATION_FLG] = value; }
	}
	/// <summary>商品カラーID</summary>
	[JsonProperty(Constants.FIELD_PRODUCTVARIATION_VARIATION_COLOR_ID)]
	public string VariationColorId
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COLOR_ID]; }
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COLOR_ID] = value; }
	}
	/// <summary>商品バリエーション重量（g）</summary>
	[JsonProperty(Constants.FIELD_PRODUCTVARIATION_VARIATION_WEIGHT_GRAM)]
	public string VariationWeightGram
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_WEIGHT_GRAM]; }
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_WEIGHT_GRAM] = value; }
	}
	/// <summary>最終更新者</summary>
	[JsonProperty(Constants.FIELD_PRODUCTVARIATION_LAST_CHANGED)]
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_LAST_CHANGED] = value; }
	}
	/// <summary>Product variation prices</summary>
	[JsonProperty(Constants.FIELD_PRODUCTVARIATION_PRODUCTPRICE_EXTEND)]
	public ProductPriceInput[] ProductPrices
	{
		get { return (ProductPriceInput[])this.DataSource[Constants.FIELD_PRODUCTVARIATION_PRODUCTPRICE_EXTEND]; }
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_PRODUCTPRICE_EXTEND] = value; }
	}
	/// <summary>Has product prices</summary>
	public bool HasProductPrices
	{
		get { return ((this.ProductPrices != null) && (this.ProductPrices.Length != 0)); }
	}
	/// <summary>Variation Id (Not include product id)</summary>
	[JsonProperty(Constants.FIELD_PRODUCTVARIATION_V_ID)]
	public string VId
	{
		get { return StringUtility.ToEmpty(this.DataSource["v_id"]); }
		set { this.DataSource["v_id"] = value; }
	}
	/// <summary>Product stock</summary>
	public ProductStockInput ProductStock
	{
		get { return (ProductStockInput)this.DataSource["EX_ProductVariationStock"]; }
		set { this.DataSource["EX_ProductVariationStock"] = value; }
	}
	/// <summary>カート投入URL制限フラグ</summary>
	[JsonProperty(Constants.FIELD_PRODUCTVARIATION_VARIATION_ADD_CART_URL_LIMIT_FLG)]
	public string VariationAddCartUrlLimitFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_ADD_CART_URL_LIMIT_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_ADD_CART_URL_LIMIT_FLG] = value; }
	}
	#endregion
}
