/*
=========================================================================================================
  Module      : FLAPS商品情報モデル (ProductItem.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using Newtonsoft.Json;
using w2.Domain.Product;
using w2.Domain.ProductVariation;

namespace w2.App.Common.Flaps.Product
{
	/// <summary>
	/// FLAPS商品情報モデル
	/// </summary>
	[JsonObject(Constants.FLAPS_API_RESULT_PRODUCT)]
	public partial class ProductItem
	{
		/// <summary>
		/// コンストラクタ
		/// /// </summary>
		public ProductItem()
		{
			this.Barcode = string.Empty;
			this.BigExchangeRate = string.Empty;
			this.BigUnitCode = string.Empty;
			this.BrandCode = string.Empty;
			this.BrandLOGOCode = string.Empty;
			this.BrandLOGOName = string.Empty;
			this.BrandName = string.Empty;
			this.BrokenAttribute = string.Empty;
			this.BrokenCodeName = string.Empty;
			this.Code = string.Empty;
			this.ColorCode = string.Empty;
			this.ColorName = string.Empty;
			this.SellYear = string.Empty;
			this.SerNo = string.Empty;
			this.StyleCode = string.Empty;
			this.Name = string.Empty;
			this.Status = 0;
			this.MiddleBrandCode = string.Empty;
			this.MiddleBrandName = string.Empty;
			this.SizeCode = string.Empty;
			this.SizeName = string.Empty;
			this.SellSeason = string.Empty;
			this.LargeCategoryCode = string.Empty;
			this.LargeCategoryName = string.Empty;
			this.MiddleCategoryCode = string.Empty;
			this.MiddleCategoryName = string.Empty;
			this.SmallCategoryCode = string.Empty;
			this.SmallCategoryName = string.Empty;
			this.InternationalBarcode = string.Empty;
			this.TaxedStdPrice = string.Empty;
			this.SpecName = string.Empty;
			this.GoodsPropertiesCode = string.Empty;
			this.GoodsPropertiesName = string.Empty;
			this.GoodsProperties2Code = string.Empty;
			this.GoodsProperties2Name = string.Empty;
			this.UnitCode = string.Empty;
			this.MiddleUnitCode = string.Empty;
			this.GoodsType = string.Empty;
			this.GoodsTypeName = string.Empty;
			this.MiddleExchangeRate = string.Empty;
			this.Remark = string.Empty;
			this.Remark2 = string.Empty;
			this.Remark3 = string.Empty;
			this.Remark4 = string.Empty;
			this.Remark5 = string.Empty;
			this.SalePrice = string.Empty;
			this.TaxedSalePrice = string.Empty;
			this.WebDate = string.Empty;
			this.Weight = string.Empty;
			this.Material = new[] { new ProductMaterial() };
			this.ProductModel = new ProductModel();
			this.ProductVariationModel = new ProductVariationModel();
		}

		/// <summary>バーコード</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_BARCODE)]
		public string Barcode { get; set; }
		/// <summary>大単位変換率</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_BIG_EXCHANGE_RATE)]
		public string BigExchangeRate { get; set; }
		/// <summary>大ユニットコード</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_BIG_UNIT_CODE)]
		public string BigUnitCode { get; set; }
		/// <summary>ブランドコード</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_BRAND_CODE)]
		public string BrandCode { get; set; }
		/// <summary>ブランドロゴコード</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_BRAND_LOGO_CODE)]
		public string BrandLOGOCode { get; set; }
		/// <summary>ブランドロゴ名</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_BRAND_LOGO_NAME)]
		public string BrandLOGOName { get; set; }
		/// <summary>ブランド名</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_BRAND_NAME)]
		public string BrandName { get; set; }
		/// <summary>セグメントコード</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_BROKEN_ATTRIBUTE)]
		public string BrokenAttribute { get; set; }
		/// <summary>セグメントコード名</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_BROKEN_CODE_NAME)]
		public string BrokenCodeName { get; set; }
		/// <summary>商品コード</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_CODE)]
		public string Code { get; set; }
		/// <summary>色コード</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_COLOR_CODE)]
		public string ColorCode { get; set; }
		/// <summary>色名</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_COLOR_NAME)]
		public string ColorName { get; set; }
		/// <summary>販売年</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_SELL_YEAR)]
		public string SellYear { get; set; }
		/// <summary>商品唯一番号</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_SER_NO)]
		public string SerNo { get; set; }
		/// <summary>スタイルコード</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_STYLE_CODE)]
		public string StyleCode { get; set; }
		/// <summary>商品名</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_NAME)]
		public string Name { get; set; }
		/// <summary>ステータス</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_STATUS)]
		public int Status { get; set; }
		/// <summary>ブランドシリーズコード</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_MIDDLE_BRAND_CODE)]
		public string MiddleBrandCode { get; set; }
		/// <summary>ブランドシリーズ名</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_MIDDLE_BRAND_NAME)]
		public string MiddleBrandName { get; set; }
		/// <summary>サイズコード</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_SIZE_CODE)]
		public string SizeCode { get; set; }
		/// <summary>サイズ名</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_SIZE_NAME)]
		public string SizeName { get; set; }
		/// <summary>販売シーズン</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_SELL_SEASON)]
		public string SellSeason { get; set; }
		/// <summary>大カテゴリコード</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_LARGE_CATEGORY_CODE)]
		public string LargeCategoryCode { get; set; }
		/// <summary>大カテゴリ名</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_LARGE_CATEGORY_NAME)]
		public string LargeCategoryName { get; set; }
		/// <summary>中カテゴリコード</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_MIDDLE_CATEGORY_CODE)]
		public string MiddleCategoryCode { get; set; }
		/// <summary>中カテゴリ名</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_MIDDLE_CATEGORY_NAME)]
		public string MiddleCategoryName { get; set; }
		/// <summary>小カテゴリコード</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_SMALL_CATEGORY_CODE)]
		public string SmallCategoryCode { get; set; }
		/// <summary>小カテゴリ名</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_SMALL_CATEGORY_NAME)]
		public string SmallCategoryName { get; set; }
		/// <summary>国際バーコード</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_INTERNATIONAL_BARCODE)]
		public string InternationalBarcode { get; set; }
		/// <summary>税込標準価格</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_TAXED_STD_PRICE)]
		public string TaxedStdPrice { get; set; }
		/// <summary>規格名称</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_SPEC_NAME)]
		public string SpecName { get; set; }
		/// <summary>商品プロパティコード</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_GOODS_PROPERTIES_CODE)]
		public string GoodsPropertiesCode { get; set; }
		/// <summary>商品プロパティ名</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_GOODS_PROPERTIES_NAME)]
		public string GoodsPropertiesName { get; set; }
		/// <summary>商品プロパティコード2</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_GOODS_PROPERTIES2_CODE)]
		public string GoodsProperties2Code { get; set; }
		/// <summary>商品プロパティ名2</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_GOODS_PROPERTIES2_NAME)]
		public string GoodsProperties2Name { get; set; }
		/// <summary>ユニットコード</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_UNIT_CODE)]
		public string UnitCode { get; set; }
		/// <summary>中ユニットコード</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_MIDDLE_UNIT_CODE)]
		public string MiddleUnitCode { get; set; }
		/// <summary>中単位変換率</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_MIDDLE_EXCHANGE_RATE)]
		public string MiddleExchangeRate { get; set; }
		/// <summary>商品タイプ</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_GOODS_TYPE)]
		public string GoodsType { get; set; }
		/// <summary>商品タイプ名</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_GOODS_TYPE_NAME)]
		public string GoodsTypeName { get; set; }
		/// <summary>備考1</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_REMARK)]
		public string Remark { get; set; }
		/// <summary>備考2</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_REMARK2)]
		public string Remark2 { get; set; }
		/// <summary>備考3</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_REMARK3)]
		public string Remark3 { get; set; }
		/// <summary>備考4</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_REMARK4)]
		public string Remark4 { get; set; }
		/// <summary>備考5</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_REMARK5)]
		public string Remark5 { get; set; }
		/// <summary>税抜特別価格</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_SALE_PRICE)]
		public string SalePrice { get; set; }
		/// <summary>税込特別価格</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_TAXED_SALE_PRICE)]
		public string TaxedSalePrice { get; set; }
		/// <summary>EC販売開始日</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_WEB_DATE)]
		public string WebDate { get; set; }
		/// <summary>重量</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_WEIGHT)]
		public string Weight { get; set; }
		/// <summary>商品マテリア</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_MATERIAL)]
		public ProductMaterial[] Material { get; set; }
		/// <summary>商品バリエーションがあるかどうか</summary>
		protected string UseVariationFlg
		{
			get
			{
				return (this.StyleCode == this.Code)
					? Constants.FLG_PRODUCT_USE_VARIATION_FLG_USE_UNUSE
					: Constants.FLG_PRODUCT_USE_VARIATION_FLG_USE_USE;
			}
		}
		/// <summary>商品モデル</summary>
		protected ProductModel ProductModel { get; set; }
		/// <summary>商品バリエーションモデル</summary>
		protected ProductVariationModel ProductVariationModel { get; set; }
	}
}
