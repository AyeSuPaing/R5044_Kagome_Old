/*
=========================================================================================================
  Module      : 商品サービス (ProductService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Domain.MasterExportSetting;
using w2.Domain.MasterExportSetting.Helper;
using w2.Domain.Product.Helper;
using w2.Domain.ProductExtend;
using w2.Domain.ProductStock;
using w2.Domain.ProductTag;
using w2.Domain.ProductVariation;

namespace w2.Domain.Product
{
	/// <summary>
	/// 商品サービス
	/// </summary>
	public class ProductService : ServiceBase, IProductService
	{
		#region +GetSearchHitCountOnCms 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <param name="shopId">店舗ID</param>
		/// <returns>件数</returns>
		public int GetSearchHitCountOnCms(ProductSearchParamModel condition, string shopId)
		{
			using (var repository = new ProductRepository())
			{
				var count = repository.GetSearchHitCountOnCms(condition, shopId);
				return count;
			}
		}
		#endregion

		#region +GetSearchVariationHitCountOnCms 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <param name="shopId">店舗ID</param>
		/// <returns>件数</returns>
		public int GetSearchVariationHitCountOnCms(ProductSearchParamModel condition, string shopId)
		{
			using (var repository = new ProductRepository())
			{
				var count = repository.GetSearchVariationHitCountOnCms(condition, shopId);
				return count;
			}
		}
		#endregion

		#region +SearchOnCms Cmsで検索
		/// <summary>
		/// Cmsで検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <param name="shopId">店舗ID</param>
		/// <returns>商品情報</returns>
		public ProductModel[] SearchOnCms(ProductSearchParamModel condition, string shopId)
		{
			using (var repository = new ProductRepository())
			{
				var results = repository.SearchOnCms(condition, shopId);
				return results;
			}
		}
		#endregion

		#region +SearchVariationOnCms Cmsで検索（バリエーション）
		/// <summary>
		/// Cmsで検索（バリエーション）
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <param name="shopId">店舗ID</param>
		/// <returns>商品情報（バリエーション）</returns>
		public ProductModel[] SearchVariationOnCms(ProductSearchParamModel condition, string shopId)
		{
			using (var repository = new ProductRepository())
			{
				var results = repository.SearchVariationOnCms(condition, shopId);
				return results;
			}
		}
		#endregion

		#region +Get 商品情報取得
		/// <summary>
		/// 商品情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>商品情報</returns>
		public ProductModel Get(string shopId, string productId, SqlAccessor accessor = null)
		{
			using (var repository = new ProductRepository(accessor))
			{
				var model = repository.Get(shopId, productId);
				return model;
			}
		}
		#endregion

		#region +GetProductVariationIds 商品バリエーションID取得
		/// <summary>
		///商品バリエーションID取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>商品バリエーションID配列</returns>
		public string[] GetProductVariationIds(string shopId, SqlAccessor accessor = null)
		{
			using (var repository = new ProductRepository(accessor))
			{
				var ids = repository.GetProductVariationIds(shopId);
				return ids;
			}
		}
		#endregion

		#region +CountProductView 商品情報の件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">商品バリエーションID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>商品情報の件数</returns>
		public int CountProductView(
			string shopId,
			string productId,
			string variationId,
			SqlAccessor accessor = null)
		{
			using (var repository = new ProductRepository(accessor))
			{
				var count = repository.CountProductView(shopId, productId, variationId);
				return count;
			}
		}
		#endregion

		#region +GetProductVariation 商品情報取得
		/// <summary>
		/// 商品情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">商品バリエーションID</param>
		/// <param name="memberRankId">会員ランクID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>商品情報</returns>
		public ProductModel GetProductVariation(string shopId, string productId, string variationId, string memberRankId, SqlAccessor accessor = null)
		{
			using (var repository = new ProductRepository(accessor))
			{
				var model = repository.GetProductVariation(shopId, productId, variationId, memberRankId);
				return model;
			}
		}

		/// <summary>
		/// 商品価格取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">商品バリエーションID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>商品情報</returns>
		public string GetProductPrice(string shopId, string productId, string variationId, SqlAccessor accessor = null)
		{
			using (var repository = new ProductRepository(accessor))
			{
				var result = repository.GetProductPrice(shopId, productId, variationId);
				return result;
			}
		}

		#endregion

		#region +GetProductVariationAtDataRowView 商品情報取得
				/// <summary>
				/// 商品情報取得
				/// </summary>
				/// <param name="shopId">店舗ID</param>
				/// <param name="productId">商品ID</param>
				/// <param name="variationId">商品バリエーションID</param>
				/// <param name="memberRankId">会員ランクID</param>
				/// <returns>商品情報</returns>
				public DataRowView GetProductVariationAtDataRowView(string shopId, string productId, string variationId, string memberRankId)
		{
			using (var repository = new ProductRepository())
			{
				var drv = repository.GetProductVariationAtDataRowView(shopId, productId, variationId, memberRankId);
				return drv;
			}
		}
		#endregion

		#region +GetProductVariationStockInfos 商品バリエーション在庫情報（在庫一覧・ドロップダウンなどに利用）
		/// <summary>
		/// 商品バリエーション在庫情報（在庫一覧・ドロップダウンなどに利用）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="memberRankId">会員ランクID</param>
		/// <returns>商品情報</returns>
		public ProductVariationStockInfo[] GetProductVariationStockInfos(string shopId, string productId, string memberRankId)
		{
			using (var repository = new ProductRepository())
			{
				var infos = repository.GetProductVariationStockInfos(shopId, productId, memberRankId);
				return infos;
			}
		}
		#endregion

		#region +GetProductByAmazonSKU 商品情報取得(Amazon)
		/// <summary>
		/// 商品情報取得(Amazon)
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="searchCondition">Amazon出品商品検索条件</param>
		/// <returns>商品情報</returns>
		public ProductModel[] GetProductByAmazonSku(string shopId, string searchCondition)
		{
			using (var repository = new ProductRepository())
			{
				var model = repository.GetProductByAmazonSku(shopId, searchCondition);
				return model;
			}
		}
		#endregion

		#region +IsStockManagement 在庫管理する商品か
		/// <summary>
		/// 在庫管理する商品か
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">商品バリエーションID</param>
		/// <param name="memberRankId">会員ランクID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>在庫管理する？</returns>
		public bool IsStockManagement(string shopId, string productId, string variationId, string memberRankId, SqlAccessor accessor = null)
		{
			var product = GetProductVariation(shopId, productId, variationId, memberRankId, accessor);
			var isStockManagement = ((product != null) && (product.IsStockUnmanaged == false));
			return isStockManagement;
		}
		#endregion

		#region +GetProductByLohacoItemCode Lohaco商品情報取得
		/// <summary>
		/// Lohaco商品情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="itemCode">Lohaco商品コード</param>
		/// <returns>商品情報</returns>
		public ProductModel GetProductByLohacoItemCode(string shopId, string itemCode)
		{
			using (var repository = new ProductRepository())
			{
				var model = repository.GetProductByLohacoItemCode(shopId, itemCode);
				return model;
			}
		}
		#endregion

		#region +GetProductByTaxCategoryId 商品税率カテゴリと紐づけられた商品情報を取得
		/// <summary>
		/// 商品税率カテゴリと紐づけられた商品情報を取得
		/// </summary>
		/// <param name="taxCategoryId">商品税率カテゴリID</param>
		/// <returns>商品情報</returns>
		public ProductModel GetProductByTaxCategoryId(string taxCategoryId)
		{
			using (var repository = new ProductRepository())
			{
				var model = repository.GetProductByTaxCategoryId(taxCategoryId);
				return model;
			}
		}
		#endregion

		#region +GetProductTopForPreview 先頭の商品取得 (プレビュー用)
		/// <summary>
		/// 先頭の商品取得 (プレビュー用)
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>先頭の商品 (プレビュー用)</returns>
		public ProductModel GetProductTopForPreview(string shopId, SqlAccessor accessor = null)
		{
			using (var repository = new ProductRepository(accessor))
			{
				var model = repository.GetProductTopForPreview(shopId);
				return model;
			}
		}
		#endregion

		#region +GetCooperationIdByProductId 商品連携IDを取得
		/// <summary>
		/// 商品連携IDを取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="cooperationIdNum">商品連携IDの番号(1～10)</param>
		/// <returns>商品情報</returns>
		public string GetCooperationIdByProductId(string shopId, string productId, int cooperationIdNum)
		{
			using (var repository = new ProductRepository())
			{
				var cooperationId = repository.GetCooperationIdByProductId(shopId, productId, cooperationIdNum);
				return cooperationId;
			}
		}
		#endregion

		#region +CheckFixedPurchaseProductExistByShippingType 指定する配送種別が設定されている定期商品の存在チェック
		/// <summary>
		/// 指定する配送種別が設定されている定期商品の存在チェック
		/// </summary>
		/// <param name="shopId">ショップID</param>
		/// <param name="shippingType">配送種別</param>
		/// <returns>存在するか</returns>
		public bool CheckFixedPurchaseProductExistByShippingType(string shopId, string shippingType)
		{
			using (var repository = new ProductRepository())
			{
				var isExist = repository.CheckFixedPurchaseProductExistByShippingType(shopId, shippingType);
				return isExist;
			}
		}
		#endregion

		#region +IsExistingProduct 存在する商品か
		/// <summary>
		/// 存在する商品か
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>存在するか</returns>
		public bool IsExistingProduct(string shopId, string productId, SqlAccessor accessor = null)
		{
			var result = Get(shopId, productId, accessor) != null;
			return result;
		}
		#endregion

		#region +商品リストを取得
		/// <summary>
		/// 商品リストを取得
		/// </summary>
		/// <param name="shopId">shop id</param>
		/// <param name="productIds">Product ids</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Array product model</returns>
		public ProductModel[] GetProducts(string shopId, IEnumerable<string> productIds, SqlAccessor accessor = null)
		{
			using (var repository = new ProductRepository(accessor))
			{
				var products = repository.GetProducts(shopId, productIds);
				return products;
			}
		}
		#endregion

		#region +GetProductListByGroup
		/// <summary>
		/// Get product list by group
		/// </summary>
		/// <param name="input">Input</param>
		/// <param name="advancedSearchWhere">Advanced search where</param>
		/// <param name="productTagFields">Product tag fields</param>
		/// <param name="productGroupWhere">Product group where</param>
		/// <param name="productGroupItemOrderBy">Product group item order by</param>
		/// <param name="favoriteCountOrderBy">Favorite count order by</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Product list by group</returns>
		public DataView GetProductListByGroup(
			Hashtable input,
			string advancedSearchWhere,
			string productTagFields,
			string productGroupWhere,
			string productGroupItemOrderBy,
			string favoriteCountOrderBy,
			SqlAccessor accessor = null)
		{
			using (var productRepository = new ProductRepository(accessor))
			{
				var productList = productRepository.GetProductListByGroup(
					input,
					advancedSearchWhere,
					productTagFields,
					productGroupWhere,
					productGroupItemOrderBy,
					favoriteCountOrderBy);

				return productList;
			}
		}
		#endregion

		#region +GetProductListByGroup
		/// <summary>
		///	Awooレコメンドに紐づいた商品情報を取得
		/// </summary>
		/// <param name="input">Input</param>
		/// <param name="productIdsWhere">商品ID条件</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>商品情報</returns>
		public DataView GetProductListByGroupAwoo(
			Hashtable input,
			string productIdsWhere,
			SqlAccessor accessor = null)
		{
			using (var productRepository = new ProductRepository(accessor))
			{
				var productList = productRepository.GetProductListByGroupAwoo(input, productIdsWhere);

				return productList;
			}
		}
		#endregion

		#region +マスタ出力
		/// <summary>
		///  CSVへ出力
		/// </summary>
		/// <param name="setting">出力設定</param>
		/// <param name="input">検索条件</param>
		/// <param name="sqlFieldNames">SQLフィールド名列</param>
		/// <param name="outputStream">出力ストリーム</param>
		/// <param name="formatDate">日付形式</param>
		/// <param name="digitsByKeyCurrency">基軸通貨 小数点以下の有効桁数</param>
		/// <param name="replacePrice">Replace文字列</param>
		/// <returns>成功か</returns>
		public bool ExportToCsv(
			MasterExportSettingModel setting,
			Hashtable input,
			string sqlFieldNames,
			Stream outputStream,
			string formatDate,
			int digitsByKeyCurrency,
			string replacePrice)
		{
			var statementName = GetStatementNameInfo(setting.MasterKbn);
			using (var accessor = new SqlAccessor())
			using (var repository = new ProductRepository(accessor))
			using (var reader = repository.GetMasterWithReader(
				input,
				statementName,
				new KeyValuePair<string, string>("@@ fields @@", sqlFieldNames)))
			{
				new MasterExportCsv().Exec(setting, reader, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
			}
			return true;
		}

		/// <summary>
		/// Excelへ出力
		/// </summary>
		/// <param name="setting">出力設定</param>
		/// <param name="input">検索条件</param>
		/// <param name="sqlFieldNames">SQLフィールド名列</param>
		/// <param name="excelTemplateSetting">Excelテンプレート設定</param>
		/// <param name="outputStream">出力ストリーム</param>
		/// <param name="formatDate">日付形式</param>
		/// <param name="digitsByKeyCurrency">基軸通貨 小数点以下の有効桁数</param>
		/// <param name="replacePrice">Replace文字列</param>
		/// <returns>成功か（件数エラーの場合は失敗）</returns>
		public bool ExportToExcel(
			MasterExportSettingModel setting,
			Hashtable input,
			string sqlFieldNames,
			ExcelTemplateSetting excelTemplateSetting,
			Stream outputStream,
			string formatDate,
			int digitsByKeyCurrency,
			string replacePrice)
		{
			var statementName = GetStatementNameInfo(setting.MasterKbn);
			using (var repository = new ProductRepository())
			{
				var dv = repository.GetMaster(input, statementName, new KeyValuePair<string, string>("@@ fields @@", sqlFieldNames));
				if (dv.Count >= 20000) return false;

				new MasterExportExcel().Exec(setting, excelTemplateSetting, dv, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				return true;
			}
		}

		/// <summary>
		/// マスタ区分で該当するStatementNameを取得
		/// </summary>
		/// <param name="masterKbn">マスタ区分</param>
		/// <returns>StatementName</returns>
		private string GetStatementNameInfo(string masterKbn)
		{
			switch (masterKbn)
			{
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCT: // 商品マスタ表示
					return "GetProductMaster";

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTVARIATION: // 商品バリエーション表示
					return "GetProductVariationMaster";

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTPRICE: // 会員ランク価格
					return "GetProductPriceMaster";

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTTAG: // 商品タグ
					return "GetProductTagMaster";

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTEXTEND: // 商品拡張項目表示
					return "GetProductExtendMaster";
			}
			throw new Exception("未対応のマスタ区分：" + masterKbn);
		}

		/// <summary>
		/// マスタ出力用フィールドチェック
		/// </summary>
		/// <param name="sqlFieldNames">SQLフィールド名列</param>
		/// <param name="masterKbn">マスタ区分</param>
		/// <param name="shopId">ショップID</param>
		/// <returns>チェックOKか</returns>
		public bool CheckFieldsForGetMaster(string sqlFieldNames, string masterKbn, string shopId)
		{
			try
			{
				using (var repository = new ProductRepository())
				{
					repository.CheckFieldsForGetMaster(
						new Hashtable { { Constants.FIELD_PRODUCT_SHOP_ID, shopId } },
						masterKbn,
						new KeyValuePair<string, string>("@@ fields @@", sqlFieldNames));
				}
			}
			catch (Exception ex)
			{
				AppLogger.WriteWarn(ex);
				return false;
			}
			return true;
		}
		#endregion

		#region +GetCartProducts カートに紐づいた商品情報を取得
		/// <summary>
		/// カートに紐づいた商品情報を取得
		/// </summary>
		/// <param name="cartId">カートID</param>
		/// <returns>商品情報</returns>
		public ProductModel[] GetCartProducts(string cartId)
		{
			using (var repository = new ProductRepository())
			{
				var productList = repository.GetCartProducts(cartId);
				return productList;
			}
		}
		#endregion

		#region +GetProductVariationsByProductId
		/// <summary>
		/// Get product variations by product id
		/// </summary>
		/// <param name="productId">Product id</param>
		/// <returns>Product variations</returns>
		public ProductVariationModel[] GetProductVariationsByProductId(string productId)
		{
			using (var repository = new ProductRepository())
			{
				var results = repository.GetProductVariartions(productId);
				return results;
			}
		}
		#endregion

		#region +SearchProductsForAutosuggest
		/// <summary>
		/// Search products for autosuggest
		/// </summary>
		/// <param name="memberRankId">Member rank id</param>
		/// <param name="searchWord">Search word</param>
		/// <returns>Products</returns>
		public ProductModel[] SearchProductsForAutosuggest(string memberRankId, string searchWord)
		{
			try
			{
				using (var repository = new ProductRepository())
				{
					repository.CommandTimeout = Constants.ORDERREGISTINPUT_SUGGEST_QUERY_TIMEOUT;
					var products = repository.SearchProductsForAutosuggest(memberRankId, searchWord);
					return products;
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		#endregion

		#region +GetProductsLikeIdOrName 指定値で部分一致の商品情報取得
		/// <summary>
		/// 指定値で部分一致の商品情報取得
		/// </summary>
		/// <param name="searchWord">検索値</param>
		/// <returns>商品モデル配列</returns>
		public ProductModel[] GetProductsLikeIdOrName(string searchWord)
		{
			using (var repository = new ProductRepository())
			{
				var products = repository.GetProductsLikeIdOrName(searchWord);
				return products;
			}
		}
		#endregion

		/// <summary>
		/// FLAPS同期処理によって取得した商品と商品バリエーションを挿入/更新する
		/// </summary>
		/// <param name="product">商品</param>
		/// <param name="productVariation">商品バリエーション</param>
		/// <returns>結果</returns>
		public bool UpsertForFlapsIntegration(ProductModel product, ProductVariationModel productVariation = null)
		{
			using (var accessor = new SqlAccessor())
			using (var repository = new ProductRepository(accessor))
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 商品更新または挿入
				var storedProduct = repository.Get(Constants.CONST_DEFAULT_SHOP_ID, product.ProductId);
				var result = (storedProduct != null)
					? UpdateForFlapsIntegration(product, accessor)
					: InsertForFlapsIntegration(product, accessor);

				if ((result == false)
					|| (product.UseVariationFlg != Constants.FLG_PRODUCT_USE_VARIATION_FLG_USE_USE)
					|| (productVariation == null))
				{
					return result;
				}

				// 商品バリエーション更新または挿入
				var storedProductVariation = repository.GetProductVariation(
					Constants.CONST_DEFAULT_SHOP_ID,
					productVariation.ProductId,
					productVariation.VariationId,
					"");
				result = (storedProductVariation != null)
					? UpdateVariationForFlapsIntegration(productVariation, accessor)
					: InsertVariationForFlapsIntegration(productVariation, accessor);
				
				accessor.CommitTransaction();

				return result;
			}
		}
	
		/// <summary>
		/// FLAPS同期処理によって取得した商品を挿入する
		/// </summary>
		/// <param name="product">商品</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>結果</returns>
		public bool InsertForFlapsIntegration(ProductModel product, SqlAccessor accessor)
		{
			using (var repository = new ProductRepository(accessor))
			{
				product.ValidFlg = Constants.FLG_ADVCODE_VALID_FLG_INVALID;
				var result = repository.InsertForFlapsIntegration(product);
				return (result > 0);
			}
		}

		/// <summary>
		/// FLAPS同期処理によって取得した商品バリエーションを挿入する
		/// </summary>
		/// <param name="pv">商品バリエーション</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>結果</returns>
		public bool InsertVariationForFlapsIntegration(ProductVariationModel pv, SqlAccessor accessor)
		{
			using (var repository = new ProductRepository(accessor))
			{
				pv.ValidFlg = Constants.FLG_ADVCODE_VALID_FLG_INVALID;
				var result = repository.InsertVariationForFlapsIntegration(pv);
				return (result > 0);
			}
		}

		/// <summary>
		/// FLAPS同期処理によって取得した商品を更新する
		/// </summary>
		/// <param name="product">商品</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>結果</returns>
		public bool UpdateForFlapsIntegration(ProductModel product, SqlAccessor accessor)
		{
			using (var repository = new ProductRepository(accessor))
			{
				// 配送種別IDは変更しない
				product.ShippingId = "";

				var result = repository.UpdateForFlapsIntegration(product);
				return (result > 0);
			}
		}

		/// <summary>
		/// FLAPS同期処理によって取得した商品バリエーションを更新する
		/// </summary>
		/// <param name="pv">商品バリエーション</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>結果</returns>
		public bool UpdateVariationForFlapsIntegration(ProductVariationModel pv, SqlAccessor accessor)
		{
			using (var repository = new ProductRepository(accessor))
			{
				var result = repository.UpdateVariationForFlapsIntegration(pv);
				return (result > 0);
			}
		}

		#region +GetAll
		/// <summary>
		/// Get all products
		/// </summary>
		/// <returns>A collection of product models</returns>
		public ProductModel[] GetAll()
		{
			using (var repository = new ProductRepository())
			{
				var result = repository.GetAll();
				return result;
			}
		}
		#endregion

		#region +GetProductsByProductIds
		/// <summary>
		/// Get products by product ids
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="productIds">Product ids</param>
		public ProductModel[] GetProductsByProductIds(string shopId, string[] productIds)
		{
			using (var repository = new ProductRepository())
			{
				var models = repository.GetProductsByProductIds(shopId, productIds);
				return models;
			}
		}
		#endregion

		#region +GetProductDetail
		/// <summary>
		/// Get product detail
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="productId">Product id</param>
		/// <returns>A product model</returns>
		public ProductModel GetProductDetail(
			string shopId,
			string productId)
		{
			using (var repository = new ProductRepository())
			{
				// Get products in database
				var dv = repository.GetProductDetailAtDataView(
					shopId,
					productId);
				if (dv.Count == 0) return null;

				// Create product model
				var model = new ProductModel(dv[0]);

				// Create product tag
				model.ProductTag = new ProductTagModel(dv[0]);

				// Create product extend
				model.ProductExtend = new ProductExtendModel(dv[0]); ;

				// Create product stock if product has setting stock managed
				if (model.IsStockUnmanaged == false)
				{
					model.ProductStocks = new[] { new ProductStockModel(dv[0]) };
				}

				// Create variations for product
				if (model.HasProductVariation)
				{
					var productVariations = new List<ProductVariationModel>();
					foreach (DataRowView row in dv)
					{
						var variation = new ProductVariationModel(row);

						// Create variation stock if product has setting stock managed
						if (model.IsStockUnmanaged == false)
						{
							variation.ProductStock = new ProductStockModel(row);
						}
						productVariations.Add(variation);
					}
					model.ProductVariations = productVariations.ToArray();
				}
				return model;
			}
		}
		#endregion

		#region +Insert
		/// <summary>
		/// Insert
		/// </summary>
		/// <param name="model">Model</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Number of rows affected</returns>
		public int Insert(ProductModel model, SqlAccessor accessor = null)
		{
			using (var repository = new ProductRepository(accessor))
			{
				var result = repository.Insert(model);
				return result;
			}
		}
		#endregion

		#region +Modify
		/// <summary>
		/// Modify
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="productId">Product id</param>
		/// <param name="updateAction">Update action</param>
		/// <param name="accessor">SQL accessor</param>
		/// <param name="execConditionFunc">Execute condition function</param>
		/// <returns>Number of rows affected</returns>
		public int Modify(
			string shopId,
			string productId,
			Action<ProductModel> updateAction,
			SqlAccessor accessor,
			Func<ProductModel, bool> execConditionFunc = null)
		{
			// 最新データ取得
			var product = Get(shopId, productId, accessor);

			// 条件
			var exec = (execConditionFunc == null) || execConditionFunc(product);
			if (exec == false) return 0;

			// モデル内容更新
			updateAction(product);

			// 更新
			int updated;
			using (var repository = new ProductRepository(accessor))
			{
				updated = repository.Update(product);
			}
			return updated;
		}
		#endregion

		#region +UpdateForModify
		/// <summary>
		/// Update for modify
		/// </summary>
		/// <param name="product">Product model</param>
		/// <param name="lastChanged">Last changed</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Number of rows affected</returns>
		public int UpdateForModify(
			ProductModel product,
			string lastChanged,
			SqlAccessor accessor)
		{
			var updated = Modify(
				product.ShopId,
				product.ProductId,
				model =>
				{
					model.Note = product.Note;
					model.SupplierId = product.SupplierId;
					model.CooperationId1 = product.CooperationId1;
					model.CooperationId2 = product.CooperationId2;
					model.CooperationId3 = product.CooperationId3;
					model.CooperationId4 = product.CooperationId4;
					model.CooperationId5 = product.CooperationId5;
					model.MallExProductId = product.MallExProductId;
					model.Name = product.Name;
					model.NameKana = product.NameKana;
					model.SeoKeywords = product.SeoKeywords;
					model.Catchcopy = product.Catchcopy;
					model.SearchKeyword = product.SearchKeyword;
					model.OutlineKbn = product.OutlineKbn;
					model.Outline = product.Outline;
					model.DescDetailKbn1 = product.DescDetailKbn1;
					model.DescDetail1 = product.DescDetail1;
					model.DescDetailKbn2 = product.DescDetailKbn2;
					model.DescDetail2 = product.DescDetail2;
					model.DescDetailKbn3 = product.DescDetailKbn3;
					model.DescDetail3 = product.DescDetail3;
					model.DescDetailKbn4 = product.DescDetailKbn4;
					model.DescDetail4 = product.DescDetail4;
					model.ReturnExchangeMessage = product.ReturnExchangeMessage;
					model.Url = product.Url;
					model.InquireEmail = product.InquireEmail;
					model.InquireTel = product.InquireTel;
					model.DisplayPrice = product.DisplayPrice;
					model.DisplaySpecialPrice = product.DisplaySpecialPrice;
					model.ShippingType = product.ShippingType;
					model.ShippingSizeKbn = product.ShippingSizeKbn;
					model.ProductColorId = product.ProductColorId;
					model.TaxIncludedFlg = product.TaxIncludedFlg;
					model.PointKbn1 = product.PointKbn1;
					model.Point1 = product.Point1;
					model.PointKbn2 = product.PointKbn2;
					model.Point2 = product.Point2;
					model.DisplayFrom = product.DisplayFrom;
					model.DisplayTo = product.DisplayTo;
					model.SellFrom = product.SellFrom;
					model.SellTo = product.SellTo;
					model.MaxSellQuantity = product.MaxSellQuantity;
					model.StockManagementKbn = product.StockManagementKbn;
					model.StockMessageId = product.StockMessageId;
					model.DisplayKbn = product.DisplayKbn;
					model.CategoryId1 = product.CategoryId1;
					model.CategoryId2 = product.CategoryId2;
					model.CategoryId3 = product.CategoryId3;
					model.CategoryId4 = product.CategoryId4;
					model.CategoryId5 = product.CategoryId5;
					model.RelatedProductIdCs1 = product.RelatedProductIdCs1;
					model.RelatedProductIdCs2 = product.RelatedProductIdCs2;
					model.RelatedProductIdCs3 = product.RelatedProductIdCs3;
					model.RelatedProductIdCs4 = product.RelatedProductIdCs4;
					model.RelatedProductIdCs5 = product.RelatedProductIdCs5;
					model.RelatedProductIdUs1 = product.RelatedProductIdUs1;
					model.RelatedProductIdUs2 = product.RelatedProductIdUs2;
					model.RelatedProductIdUs3 = product.RelatedProductIdUs3;
					model.RelatedProductIdUs4 = product.RelatedProductIdUs4;
					model.RelatedProductIdUs5 = product.RelatedProductIdUs5;
					model.ImageHead = product.ImageHead;
					model.IconFlg1 = product.IconFlg1;
					model.IconTermEnd1 = product.IconTermEnd1;
					model.IconFlg2 = product.IconFlg2;
					model.IconTermEnd2 = product.IconTermEnd2;
					model.IconFlg3 = product.IconFlg3;
					model.IconTermEnd3 = product.IconTermEnd3;
					model.IconFlg4 = product.IconFlg4;
					model.IconTermEnd4 = product.IconTermEnd4;
					model.IconFlg5 = product.IconFlg5;
					model.IconTermEnd5 = product.IconTermEnd5;
					model.IconFlg6 = product.IconFlg6;
					model.IconTermEnd6 = product.IconTermEnd6;
					model.IconFlg7 = product.IconFlg7;
					model.IconTermEnd7 = product.IconTermEnd7;
					model.IconFlg8 = product.IconFlg8;
					model.IconTermEnd8 = product.IconTermEnd8;
					model.IconFlg9 = product.IconFlg9;
					model.IconTermEnd9 = product.IconTermEnd9;
					model.IconFlg10 = product.IconFlg10;
					model.IconTermEnd10 = product.IconTermEnd10;
					model.UseVariationFlg = product.UseVariationFlg;
					model.FixedPurchaseFlg = product.FixedPurchaseFlg;
					model.CheckFixedProductOrderFlg = product.CheckFixedProductOrderFlg;
					model.CheckProductOrderFlg = product.CheckProductOrderFlg;
					model.MemberRankDiscountFlg = product.MemberRankDiscountFlg;
					model.DisplayMemberRank = product.DisplayMemberRank;
					model.BuyableMemberRank = product.BuyableMemberRank;
					model.ValidFlg = product.ValidFlg;
					model.LastChanged = lastChanged;
					model.GoogleShoppingFlg = product.GoogleShoppingFlg;
					model.ProductOptionSettings = product.ProductOptionSettings;
					model.ArrivalMailValidFlg = product.ArrivalMailValidFlg;
					model.ReleaseMailValidFlg = product.ReleaseMailValidFlg;
					model.ResaleMailValidFlg = product.ResaleMailValidFlg;
					model.SelectVariationKbn = product.SelectVariationKbn;
					model.BrandId1 = product.BrandId1;
					model.BrandId2 = product.BrandId2;
					model.BrandId3 = product.BrandId3;
					model.BrandId4 = product.BrandId4;
					model.BrandId5 = product.BrandId5;
					model.UseRecommendFlg = product.UseRecommendFlg;
					model.PluralShippingPriceFreeFlg = product.PluralShippingPriceFreeFlg;
					model.AgeLimitFlg = product.AgeLimitFlg;
					model.GiftFlg = product.GiftFlg;
					model.DigitalContentsFlg = product.DigitalContentsFlg;
					model.DownloadUrl = product.DownloadUrl;
					model.DisplaySellFlg = product.DisplaySellFlg;
					model.DisplayPriority = product.DisplayPriority;
					model.LimitedPaymentIds = product.LimitedPaymentIds;
					model.FixedPurchaseFirsttimePrice = product.FixedPurchaseFirsttimePrice;
					model.FixedPurchasePrice = product.FixedPurchasePrice;
					model.BundleItemDisplayType = product.BundleItemDisplayType;
					model.ProductType = product.ProductType;
					model.LimitedFixedPurchaseKbn1Setting = product.LimitedFixedPurchaseKbn1Setting;
					model.LimitedFixedPurchaseKbn3Setting = product.LimitedFixedPurchaseKbn3Setting;
					model.CooperationId6 = product.CooperationId6;
					model.CooperationId7 = product.CooperationId7;
					model.CooperationId8 = product.CooperationId8;
					model.CooperationId9 = product.CooperationId9;
					model.CooperationId10 = product.CooperationId10;
					model.AndmallReservationFlg = product.AndmallReservationFlg;
					model.TaxCategoryId = product.TaxCategoryId;
					model.DisplayOnlyFixedPurchaseMemberFlg = product.DisplayOnlyFixedPurchaseMemberFlg;
					model.SellOnlyFixedPurchaseMemberFlg = product.SellOnlyFixedPurchaseMemberFlg;
					model.ProductWeightGram = product.ProductWeightGram;
					model.FixedPurchasedCancelableCount = product.FixedPurchasedCancelableCount;
					model.FixedPurchasedLimitedUserLevelIds = product.FixedPurchasedLimitedUserLevelIds;
					model.FixedPurchaseNextShippingProductId = product.FixedPurchaseNextShippingProductId;
					model.FixedPurchaseNextShippingVariationId = product.FixedPurchaseNextShippingVariationId;
					model.FixedPurchaseNextShippingItemQuantity = product.FixedPurchaseNextShippingItemQuantity;
					model.FixedPurchaseLimitedSkippedCount = product.FixedPurchaseLimitedSkippedCount;
					model.NextShippingItemFixedPurchaseKbn = product.NextShippingItemFixedPurchaseKbn;
					model.NextShippingItemFixedPurchaseSetting = product.NextShippingItemFixedPurchaseSetting;
					model.ProductSizeFactor = product.ProductSizeFactor;
					model.AddCartUrlLimitFlg = product.AddCartUrlLimitFlg;
					model.LimitedFixedPurchaseKbn4Setting = product.LimitedFixedPurchaseKbn4Setting;
					model.SubscriptionBoxFlg = product.SubscriptionBoxFlg;
					model.MemberRankPointExcludeFlg = product.MemberRankPointExcludeFlg;
					model.StorePickupFlg = product.StorePickupFlg;
					model.ExcludeFreeShippingFlg = product.ExcludeFreeShippingFlg;
				},
				accessor);
			return updated;
		}
		#endregion

		#region +Delete
		/// <summary>
		/// Delete
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="productId">Product ID</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Number of rows affected</returns>
		public int Delete(string shopId, string productId, SqlAccessor accessor = null)
		{
			using (var repository = new ProductRepository(accessor))
			{
				var result = repository.Delete(shopId, productId);
				return result;
			}
		}
		#endregion

		#region ~ProductVariation

		#region +IsProductVariationExist
		/// <summary>
		/// Is product variation exist
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="productId">Product ID</param>
		/// <param name="variationId">Variation ID</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>True if product variation is existed, otherwise false</returns>
		public bool IsProductVariationExist(
			string shopId,
			string productId,
			string variationId,
			SqlAccessor accessor = null)
		{
			using (var repository = new ProductRepository(accessor))
			{
				var result = repository.IsProductVariationExist(shopId, productId, variationId);
				return result;
			}
		}
		#endregion

		#region +GetDeleteProductVariations
		/// <summary>
		/// Get delete product variations
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="productId">Product ID</param>
		/// <param name="ignoredVariationIds">Ignored variation ID</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Product price model</returns>
		public ProductVariationModel[] GetDeleteProductVariations(
			string shopId,
			string productId,
			string[] ignoredVariationIds,
			SqlAccessor accessor = null)
		{
			using (var repository = new ProductRepository(accessor))
			{
				var result = repository.GetDeleteProductVariations(
					shopId,
					productId,
					ignoredVariationIds);
				return result;
			}
		}
		#endregion

		#region +GetProductVariationByProductIdAndVariationId
		/// <summary>
		/// Get product variation by product id and variation id
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="productId">Product id</param>
		/// <param name="variationId">Variation id</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Model</returns>
		public ProductVariationModel GetProductVariationByProductIdAndVariationId(
			string shopId,
			string productId,
			string variationId,
			SqlAccessor accessor)
		{
			using (var repository = new ProductRepository(accessor))
			{
				var model = repository.GetProductVariationByProductIdAndVariationId(
					shopId,
					productId,
					variationId);
				return model;
			}
		}
		#endregion

		#region +InsertProductVariation
		/// <summary>
		/// Insert product variation
		/// </summary>
		/// <param name="model">Model</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Number of rows affected</returns>
		public int InsertProductVariation(ProductVariationModel model, SqlAccessor accessor = null)
		{
			using (var repository = new ProductRepository(accessor))
			{
				var result = repository.InsertProductVariation(model);
				return result;
			}
		}
		#endregion

		#region +ModifyProductVariation
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
		public int ModifyProductVariation(
			string shopId,
			string productId,
			string variationId,
			Action<ProductVariationModel> updateAction,
			SqlAccessor accessor,
			Func<ProductVariationModel, bool> execConditionFunc = null)
		{
			// 最新データ取得
			var productVariation = GetProductVariationByProductIdAndVariationId(
				shopId,
				productId,
				variationId,
				accessor);

			// 条件
			var exec = (execConditionFunc == null) || execConditionFunc(productVariation);
			if (exec == false) return 0;

			// モデル内容更新
			updateAction(productVariation);

			// 更新
			int updated;
			using (var repository = new ProductRepository(accessor))
			{
				updated = repository.UpdateProductVariation(productVariation);
			}
			return updated;
		}
		#endregion

		#region +UpdateProductVariationForModify
		/// <summary>
		/// Update product variation for modify
		/// </summary>
		/// <param name="variation">Variation model</param>
		/// <param name="lastChanged">Last changed</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Number of rows affected</returns>
		public int UpdateProductVariationForModify(
			ProductVariationModel variation,
			string lastChanged,
			SqlAccessor accessor)
		{
			var updated = ModifyProductVariation(
				variation.ShopId,
				variation.ProductId,
				variation.VariationId,
				model =>
				{
					model.VariationCooperationId1 = variation.VariationCooperationId1;
					model.VariationCooperationId2 = variation.VariationCooperationId2;
					model.VariationCooperationId3 = variation.VariationCooperationId3;
					model.VariationCooperationId4 = variation.VariationCooperationId4;
					model.VariationCooperationId5 = variation.VariationCooperationId5;
					model.VariationDownloadUrl = variation.VariationDownloadUrl;
					model.MallVariationId1 = variation.MallVariationId1;
					model.MallVariationId2 = variation.MallVariationId2;
					model.MallVariationType = variation.MallVariationType;
					model.VariationName1 = variation.VariationName1;
					model.VariationName2 = variation.VariationName2;
					model.VariationName3 = variation.VariationName3;
					model.VariationColorId = variation.VariationColorId;
					model.Price = variation.Price;
					model.SpecialPrice = variation.SpecialPrice;
					model.VariationImageHead = variation.VariationImageHead;
					model.DisplayOrder = variation.DisplayOrder;
					model.LastChanged = lastChanged;
					model.VariationFixedPurchaseFirsttimePrice = variation.VariationFixedPurchaseFirsttimePrice;
					model.VariationFixedPurchasePrice = variation.VariationFixedPurchasePrice;
					model.VariationCooperationId6 = variation.VariationCooperationId6;
					model.VariationCooperationId7 = variation.VariationCooperationId7;
					model.VariationCooperationId8 = variation.VariationCooperationId8;
					model.VariationCooperationId9 = variation.VariationCooperationId9;
					model.VariationCooperationId10 = variation.VariationCooperationId10;
					model.VariationAndmallReservationFlg = variation.VariationAndmallReservationFlg;
					model.VariationWeightGram = variation.VariationWeightGram;
					model.VariationAddCartUrlLimitFlg = variation.VariationAddCartUrlLimitFlg;
				},
				accessor);
			return updated;
		}
		#endregion

		#region +DeleteProductVariations
		/// <summary>
		/// Delete product variations
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="productId">Product ID</param>
		/// <param name="ignoredVariationIds">Ignored variation ID</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Number of rows affected</returns>
		public int DeleteProductVariations(
			string shopId,
			string productId,
			string[] ignoredVariationIds,
			SqlAccessor accessor = null)
		{
			using (var repository = new ProductRepository(accessor))
			{
				var result = repository.DeleteProductVariations(
					shopId,
					productId,
					ignoredVariationIds);
				return result;
			}
		}
		#endregion

		#region +DeleteProductVariationAll
		/// <summary>
		/// Delete product variation all
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="productId">Product ID</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Number of rows affected</returns>
		public int DeleteProductVariationAll(
			string shopId,
			string productId,
			SqlAccessor accessor = null)
		{
			using (var repository = new ProductRepository(accessor))
			{
				var result = repository.DeleteProductVariationAll(
					shopId,
					productId);
				return result;
			}
		}
		#endregion

		#endregion

		#region +GetProductByCooperationId1 商品バリエーション連携ID1から商品情報取得
		/// <summary>
		/// 商品バリエーション連携ID1から商品情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="cooperationId1">商品バリエーション連携ID1</param>
		/// <returns>商品情報</returns>
		public ProductModel GetProductByCooperationId1(string shopId, string cooperationId1)
		{
			using (var repository = new ProductRepository())
			{
				var model = repository.GetProductVariationByCooperationId1(shopId, cooperationId1)
					?? repository.GetProductByCooperationId1(shopId, cooperationId1);
				return model;
			}
		}
		#endregion

		/// <summary>
		/// 商品IDが既に「商品ID+バリエーションID」として使用されるかチェック
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <returns>存在するか</returns>
		public bool CheckProductIdIsUsedAsVariationId(string shopId, string productId)
		{
			using (var repository = new ProductRepository())
			{
				var isExist = repository.CheckProductIdIsUsedAsVariationId(shopId, productId);
				return isExist;
			}
		}

		/// <summary>
		/// 商品ID＋バリエーションIDが既に「商品ID」として使用されるかチェック
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="variationId">商品バリエーションID</param>
		/// <returns>存在するか</returns>
		public bool CheckVariationIdIsUsedAsProductId(string shopId, string variationId)
		{
			using (var repository = new ProductRepository())
			{
				var isExist = repository.CheckVariationIdIsUsedAsProductId(shopId, variationId);
				return isExist;
			}
		}

		/// <summary>
		/// 商品ID＋バリエーションIDが既に「商品ID+バリエーショID」として使用されるかチェック
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="variationId">商品バリエーションID</param>
		/// <returns>存在するか</returns>
		public bool CheckVariationIdIsUsedAsVariationId(string shopId, string variationId)
		{
			using (var repository = new ProductRepository())
			{
				var isExist = repository.CheckVariationIdIsUsedAsVariationId(shopId, variationId);
				return isExist;
			}
		}

		/// <summary>
		/// Awoo連携用商品データ取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="brandId">ブランドID</param>
		/// <param name="memberRankId">会員ランクID</param>
		/// <param name="userFixedPurchaseMemberFlg">定期会員フラグ</param>
		/// <param name="productIds">商品IDの配列</param>
		/// <returns>商品情報</returns>
		public DataView GetProductForAwooProductSync(
			string shopId,
			string brandId,
			string memberRankId,
			string userFixedPurchaseMemberFlg,
			string[] productIds)
		{
			using (var repository = new ProductRepository())
			{
				var dv = repository.GetProductForAwooProductSync(
					shopId,
					brandId,
					memberRankId,
					userFixedPurchaseMemberFlg,
					productIds);
				return dv;
			}
		}

		/// <summary>
		/// Awoo連携用商品データ取得
		/// </summary>
		/// <returns>商品情報</returns>
		public DataView GetProductForAwooProductSync()
		{
			using (var repository = new ProductRepository())
			{
				var dv = repository.GetProductForAwooProductSync();
				return dv;
			}
		}

		#region ~GetProductVariationCounts 商品バリエーション件数取得
		/// <summary>
		/// 商品バリエーション件数取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productIds">商品ID</param>
		/// <returns>商品バリエーション件数情報</returns>
		public static Dictionary<string, int> GetProductVariationCounts(string shopId, List<string> productIds)
		{
			using (var repository = new ProductRepository())
			{
				var productVariationCounts = repository.GetProductVariationCounts(shopId, productIds);
				return productVariationCounts;
			}
		}
		#endregion
	}
}
