/*
=========================================================================================================
  Module      : 商品サービスのインタフェース(IProductService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using w2.Common.Sql;
using w2.Domain.Product.Helper;
using w2.Domain.ProductVariation;

namespace w2.Domain.Product
{
	/// <summary>
	/// 商品サービスのインタフェース
	/// </summary>
	public interface IProductService : IService
	{
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <param name="shopId">店舗ID</param>
		/// <returns>件数</returns>
		int GetSearchHitCountOnCms(ProductSearchParamModel condition, string shopId);

		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <param name="shopId">店舗ID</param>
		/// <returns>件数</returns>
		int GetSearchVariationHitCountOnCms(ProductSearchParamModel condition, string shopId);

		/// <summary>
		/// Cmsで検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <param name="shopId">店舗ID</param>
		/// <returns>商品情報</returns>
		ProductModel[] SearchOnCms(ProductSearchParamModel condition, string shopId);

		/// <summary>
		/// Cmsで検索（バリエーション）
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <param name="shopId">店舗ID</param>
		/// <returns>商品情報（バリエーション）</returns>
		ProductModel[] SearchVariationOnCms(ProductSearchParamModel condition, string shopId);

		/// <summary>
		/// 商品情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>商品情報</returns>
		ProductModel Get(string shopId, string productId, SqlAccessor accessor = null);

		/// <summary>
		/// 商品情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">商品バリエーションID</param>
		/// <param name="memberRankId">会員ランクID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>商品情報</returns>
		ProductModel GetProductVariation(string shopId, string productId, string variationId, string memberRankId, SqlAccessor accessor = null);

		/// <summary>
		/// 商品情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">商品バリエーションID</param>
		/// <param name="memberRankId">会員ランクID</param>
		/// <returns>商品情報</returns>
		DataRowView GetProductVariationAtDataRowView(string shopId, string productId, string variationId, string memberRankId);

		/// <summary>
		/// 商品バリエーション在庫情報（在庫一覧・ドロップダウンなどに利用）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="memberRankId">会員ランクID</param>
		/// <returns>商品情報</returns>
		ProductVariationStockInfo[] GetProductVariationStockInfos(string shopId, string productId, string memberRankId);

		/// <summary>
		/// 商品情報取得(Amazon)
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="searchCondition">Amazon出品商品検索条件</param>
		/// <returns>商品情報</returns>
		ProductModel[] GetProductByAmazonSku(string shopId, string searchCondition);

		/// <summary>
		/// 在庫管理する商品か
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">商品バリエーションID</param>
		/// <param name="memberRankId">会員ランクID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>在庫管理する？</returns>
		bool IsStockManagement(string shopId, string productId, string variationId, string memberRankId, SqlAccessor accessor = null);

		/// <summary>
		/// Lohaco商品情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="itemCode">Lohaco商品コード</param>
		/// <returns>商品情報</returns>
		ProductModel GetProductByLohacoItemCode(string shopId, string itemCode);

		/// <summary>
		/// 商品税率カテゴリと紐づけられた商品情報を取得
		/// </summary>
		/// <param name="taxCategoryId">商品税率カテゴリID</param>
		/// <returns>商品情報</returns>
		ProductModel GetProductByTaxCategoryId(string taxCategoryId);

		/// <summary>
		/// 先頭の商品取得 (プレビュー用)
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>先頭の商品 (プレビュー用)</returns>
		ProductModel GetProductTopForPreview(string shopId, SqlAccessor accessor = null);

		/// <summary>
		/// 商品連携IDを取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="cooperationIdNum">商品連携IDの番号(1～10)</param>
		/// <returns>商品情報</returns>
		string GetCooperationIdByProductId(string shopId, string productId, int cooperationIdNum);

		/// <summary>
		/// 指定する配送種別が設定されている定期商品の存在チェック
		/// </summary>
		/// <param name="shopId">ショップID</param>
		/// <param name="shippingType">配送種別</param>
		/// <returns>存在するか</returns>
		bool CheckFixedPurchaseProductExistByShippingType(string shopId, string shippingType);

		/// <summary>
		/// 存在する商品か
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>存在するか</returns>
		bool IsExistingProduct(string shopId, string productId, SqlAccessor accessor = null);

		#region +商品リストを取得
		/// <summary>
		/// 商品リストを取得
		/// </summary>
		/// <param name="shopId">shop id</param>
		/// <param name="productIds">Product ids</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Array product model</returns>
		ProductModel[] GetProducts(string shopId, IEnumerable<string> productIds, SqlAccessor accessor = null);
		#endregion

		/// <summary>
		/// カートに紐づいた商品情報を取得
		/// </summary>
		/// <param name="cartId">カートID</param>
		/// <returns>商品情報</returns>
		ProductModel[] GetCartProducts(string cartId);

		/// <summary>
		/// Get product variations by product id
		/// </summary>
		/// <param name="productId">Product id</param>
		/// <returns>Product variations</returns>
		ProductVariationModel[] GetProductVariationsByProductId(string productId);

		/// <summary>
		/// Search products for autosuggest
		/// </summary>
		/// <param name="memberRankId">Member rank id</param>
		/// <param name="searchWord">Search word</param>
		/// <returns>Products</returns>
		ProductModel[] SearchProductsForAutosuggest(string memberRankId, string searchWord);

		/// <summary>
		/// 指定値で部分一致の商品情報取得
		/// </summary>
		/// <param name="searchWord">検索値</param>
		/// <returns>商品モデル配列</returns>
		ProductModel[] GetProductsLikeIdOrName(string searchWord);

		/// <summary>
		/// Get all products
		/// </summary>
		/// <returns>A collection of product models</returns>
		ProductModel[] GetAll();

		/// <summary>
		/// Get products by product ids
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="productIds">Product ids</param>
		ProductModel[] GetProductsByProductIds(string shopId, string[] productIds);

		/// <summary>
		/// Get product
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="productId">Product id</param>
		/// <returns>A product model</returns>
		ProductModel GetProductDetail(
			string shopId,
			string productId);

		/// <summary>
		/// Insert
		/// </summary>
		/// <param name="model">Model</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Number of rows affected</returns>
		int Insert(ProductModel model, SqlAccessor accessor = null);

		/// <summary>
		/// Modify
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="productId">Product id</param>
		/// <param name="updateAction">Update action</param>
		/// <param name="accessor">SQL accessor</param>
		/// <param name="execConditionFunc">Execute condition function</param>
		/// <returns>Number of rows affected</returns>
		int Modify(
			string shopId,
			string productId,
			Action<ProductModel> updateAction,
			SqlAccessor accessor,
			Func<ProductModel, bool> execConditionFunc = null);

		/// <summary>
		/// Update for modify
		/// </summary>
		/// <param name="product">Product model</param>
		/// <param name="lastChanged">Last changed</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Number of rows affected</returns>
		int UpdateForModify(
			ProductModel product,
			string lastChanged,
			SqlAccessor accessor);

		/// <summary>
		/// Delete
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="productId">Product ID</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Number of rows affected</returns>
		int Delete(string shopId, string productId, SqlAccessor accessor = null);

		#region ~ProductVariation

		/// <summary>
		/// Is product variation exist
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="productId">Product ID</param>
		/// <param name="variationId">Variation ID</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>True if product variation is existed, otherwise false</returns>
		bool IsProductVariationExist(
			string shopId,
			string productId,
			string variationId,
			SqlAccessor accessor = null);

		/// <summary>
		/// Get delete product variations
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="productId">Product ID</param>
		/// <param name="ignoredVariationIds">Ignored variation ID</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Product price model</returns>
		ProductVariationModel[] GetDeleteProductVariations(
			string shopId,
			string productId,
			string[] ignoredVariationIds,
			SqlAccessor accessor = null);

		/// <summary>
		/// Get product variation by product id and variation id
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="productId">Product id</param>
		/// <param name="variationId">Variation id</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Model</returns>
		ProductVariationModel GetProductVariationByProductIdAndVariationId(
			string shopId,
			string productId,
			string variationId,
			SqlAccessor accessor);

		/// <summary>
		/// Insert product variation
		/// </summary>
		/// <param name="model">Model</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Number of rows affected</returns>
		int InsertProductVariation(ProductVariationModel model, SqlAccessor accessor = null);

		/// <summary>
		/// Modify product variation
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="productId">Product id</param>
		/// <param name="variationId">Variation id</param>
		/// <param name="updateAction">Update action</param>
		/// <param name="accessor">SQL accessor</param>
		/// <param name="execConditionFunc">Execute condition function</param>
		/// <returns>Number of rows affected</returns>
		int ModifyProductVariation(
			string shopId,
			string productId,
			string variationId,
			Action<ProductVariationModel> updateAction,
			SqlAccessor accessor,
			Func<ProductVariationModel, bool> execConditionFunc = null);

		/// <summary>
		/// Update product variation for modify
		/// </summary>
		/// <param name="variation">Variation model</param>
		/// <param name="lastChanged">Last changed</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Number of rows affected</returns>
		int UpdateProductVariationForModify(
			ProductVariationModel variation,
			string lastChanged,
			SqlAccessor accessor);

		/// <summary>
		/// Delete product variations
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="productId">Product ID</param>
		/// <param name="ignoredVariationIds">Ignored variation ID</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Number of rows affected</returns>
		int DeleteProductVariations(
			string shopId,
			string productId,
			string[] ignoredVariationIds,
			SqlAccessor accessor = null);

		/// <summary>
		/// Delete product variation all
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="productId">Product ID</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Number of rows affected</returns>
		int DeleteProductVariationAll(string shopId, string productId, SqlAccessor accessor = null);

		#endregion

		/// <summary>
		/// 商品バリエーション連携ID1から商品情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="cooperationId1">商品バリエーション連携ID1</param>
		/// <returns>商品情報</returns>
		ProductModel GetProductByCooperationId1(string shopId, string cooperationId1);
	}
}
