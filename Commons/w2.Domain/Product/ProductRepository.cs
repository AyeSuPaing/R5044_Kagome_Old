/*
=========================================================================================================
  Module      : 商品リポジトリ (ProductRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.Product.Helper;
using w2.Domain.ProductVariation;

namespace w2.Domain.Product
{
	/// <summary>
	/// 商品リポジトリ
	/// </summary>
	internal class ProductRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "Product";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal ProductRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		public ProductRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~GetSearchHitCountOnCms 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <param name="shopId">店舗ID</param>
		/// <returns>件数</returns>
		internal int GetSearchHitCountOnCms(ProductSearchParamModel condition, string shopId)
		{
			var input = condition.CreateHashtableParams();
			input.Add(Constants.FIELD_PRODUCT_SHOP_ID, shopId);
			var dv = Get(XML_KEY_NAME, "GetSearchHitCountOnCms", input);
			return (int)dv[0][0];
		}
		#endregion

		#region ~GetSearchVariationHitCountOnCms 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <param name="shopId">店舗ID</param>
		/// <returns>件数</returns>
		internal int GetSearchVariationHitCountOnCms(ProductSearchParamModel condition, string shopId)
		{
			var input = condition.CreateHashtableParams();
			input.Add(Constants.FIELD_PRODUCT_SHOP_ID, shopId);
			var dv = Get(XML_KEY_NAME, "GetSearchVariationHitCountOnCms", input);
			return (int)dv[0][0];
		}
		#endregion

		#region ~SearchOnCms Cmsで検索
		/// <summary>
		/// Cmsで検索
		/// </summary>
		/// <param name="condition">商品検索条件</param>
		/// <param name="shopId">店舗ID</param>
		/// <returns>商品情報</returns>
		internal ProductModel[] SearchOnCms(ProductSearchParamModel condition, string shopId)
		{
			var input = condition.CreateHashtableParams();
			input.Add(Constants.FIELD_PRODUCT_SHOP_ID, shopId);

			var dv = Get(XML_KEY_NAME, "SearchOnCms", input);
			return dv.Cast<DataRowView>().Select(drv => new ProductModel(drv)).ToArray();
		}
		#endregion

		#region ~SearchVariationOnCms Cmsで検索（バリエーション）
		/// <summary>
		/// Cmsで検索（バリエーション）
		/// </summary>
		/// <param name="condition">商品検索条件</param>
		/// <param name="shopId">店舗ID</param>
		/// <returns>商品情報（バリエーション）</returns>
		internal ProductModel[] SearchVariationOnCms(ProductSearchParamModel condition, string shopId)
		{
			var input = condition.CreateHashtableParams();
			input.Add(Constants.FIELD_PRODUCT_SHOP_ID, shopId);

			var dv = Get(XML_KEY_NAME, "SearchVariationOnCms", input);
			return dv.Cast<DataRowView>().Select(drv => new ProductModel(drv)).ToArray();
		}
		#endregion

		#region ~Get 商品情報取得
		/// <summary>
		/// 商品情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <returns>商品情報</returns>
		internal ProductModel Get(string shopId, string productId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_PRODUCT_SHOP_ID, shopId },
				{ Constants.FIELD_PRODUCT_PRODUCT_ID, productId },
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			var result = dv.Cast<DataRowView>().Select(drv => new ProductModel(drv)).ToArray();
			return (result.Length > 0) ? result[0] : null;
		}
		#endregion

		#region ~GetProductVariationIds 商品バリエーションID取得
		/// <summary>
		/// 商品バリエーションID取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>商品バリエーションID配列</returns>
		internal string[] GetProductVariationIds(string shopId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_PRODUCT_SHOP_ID, shopId },
			};
			var dv = Get(XML_KEY_NAME, "GetProductVariationIds", ht);
			var result = dv.Cast<DataRowView>().Select(drv => StringUtility.ToEmpty(drv[0])).ToArray();
			return result;
		}
		#endregion

		#region ~CountProductView 商品情報の件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">商品バリエーションID</param>
		/// <returns>商品情報の件数</returns>
		internal int CountProductView(string shopId, string productId, string variationId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_PRODUCT_SHOP_ID, shopId },
				{ Constants.FIELD_PRODUCT_PRODUCT_ID, productId },
				{ Constants.FIELD_PRODUCTVARIATION_VARIATION_ID, variationId },
			};
			var dv = Get(XML_KEY_NAME, "CountProductView", ht);
			return (int)dv[0][0];
		}
		#endregion

		#region ~GetProductVariation 商品情報取得
		/// <summary>
		/// 商品情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">商品バリエーションID</param>
		/// <param name="memberRankId">会員ランクID</param>
		/// <returns>商品情報</returns>
		internal ProductModel GetProductVariation(string shopId, string productId, string variationId, string memberRankId)
		{
			var drv = GetProductVariationAtDataRowView(shopId, productId, variationId, memberRankId);
			return (drv == null) ? null : new ProductModel(drv);
		}

		/// <summary>
		/// 商品価格取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">商品バリエーションID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>商品価格取得</returns>
		internal string GetProductPrice(
			string shopId,
			string productId,
			string variationId,
			SqlAccessor accessor = null)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_PRODUCT_SHOP_ID, shopId },
				{ Constants.FIELD_PRODUCT_PRODUCT_ID, productId },
				{ Constants.FIELD_PRODUCTVARIATION_VARIATION_ID, variationId },
			};
			var dv = Get(XML_KEY_NAME, "GetProductPrice", ht);
			var result = dv.Count > 0 ? dv[0][0].ToString() : string.Empty;
			return result;
		}
		#endregion

		#region ~GetProductVariationAtDataRowView 商品情報取得
		/// <summary>
		/// 商品情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">商品バリエーションID</param>
		/// <param name="memberRankId">会員ランクID</param>
		/// <returns>商品情報</returns>
		/// <remarks>DataRowViewのほうが都合がよい場面もあるので使い分けられるようにする</remarks>
		internal DataRowView GetProductVariationAtDataRowView(string shopId, string productId, string variationId, string memberRankId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_PRODUCT_SHOP_ID, shopId },
				{ Constants.FIELD_PRODUCT_PRODUCT_ID, productId },
				{ Constants.FIELD_PRODUCTVARIATION_VARIATION_ID, variationId },
				{ Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID, memberRankId}
			};
			var dv = Get(XML_KEY_NAME, "GetProductVariation", ht);
			return (dv.Count > 0) ? dv[0] : null;
		}
		#endregion

		#region ~GetProductVariationStockInfos 商品バリエーション在庫情報
		/// <summary>
		/// 商品バリエーション在庫情報
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="memberRankId">会員ランクID</param>
		/// <returns>商品情報</returns>
		internal ProductVariationStockInfo[] GetProductVariationStockInfos(string shopId, string productId, string memberRankId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_PRODUCT_SHOP_ID, shopId },
				{ Constants.FIELD_PRODUCT_PRODUCT_ID, productId },
				{ Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID, memberRankId}
			};
			var dv = Get(XML_KEY_NAME, "GetProductVariationStockInfos", ht);
			return dv.Cast<DataRowView>().Select(drv => new ProductVariationStockInfo(drv)).ToArray();
		}
		#endregion

		#region ~GetProductByAmazonSKU 商品情報取得(Amazon)
		/// <summary>
		/// 商品情報取得(Amazon)
		/// </summary>
		/// <param name="shop_id">店舗ID</param>
		/// <param name="searchCondition">Amazon出品商品検索条件</param>
		/// <returns>商品情報</returns>
		/// <remarks>一意に登録してもらう前提だが念のため複数レコード取得されることを考慮</remarks>
		internal ProductModel[] GetProductByAmazonSku(string shop_id, string searchCondition)
		{
			using (var statement = new SqlStatement(XML_KEY_NAME, "GetProductByAmazonSKU"))
			{
				var ht = new Hashtable
				{
					{ Constants.FIELD_PRODUCT_SHOP_ID, shop_id }
				};
				statement.ReplaceStatement("@@ amazon_product_where @@", searchCondition);
				var dv = statement.SelectSingleStatementWithOC(this.Accessor, ht);
				return dv.Cast<DataRowView>().Select(drv => new ProductModel(drv)).ToArray();
			}
		}
		#endregion

		#region ~GetProductByLohacoItemCode Lohaco商品情報取得
		/// <summary>
		/// Lohaco商品情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="itemCode">Lohaco商品コード</param>
		/// <returns>商品情報</returns>
		internal ProductModel GetProductByLohacoItemCode(string shopId, string itemCode)
		{
			using (var statement = new SqlStatement(XML_KEY_NAME, "GetProductVariationByLohacoItemCode"))
			{
				var ht = new Hashtable
				{
					{ Constants.FIELD_PRODUCT_SHOP_ID, shopId },
					{ Constants.FIELD_PRODUCTVARIATION_VARIATION_ID, itemCode },
				};
				var dv = statement.SelectSingleStatementWithOC(this.Accessor, ht);
				if (dv.Count == 0) return null;
				return new ProductModel(dv[0]);
			}
		}
		#endregion

		#region ~GetProductByTaxCategoryId 商品税率カテゴリと紐づけられた商品情報を取得
		/// <summary>
		/// 商品税率カテゴリと紐づけられた商品情報を取得
		/// </summary>
		/// <param name="taxCategoryId">商品税率カテゴリID</param>
		/// <returns>商品情報</returns>
		internal ProductModel GetProductByTaxCategoryId(string taxCategoryId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_PRODUCT_TAX_CATEGORY_ID, taxCategoryId },
			};
			var dv = Get(XML_KEY_NAME, "GetProductByTaxCategoryId", ht);
			if (dv.Count == 0) return null;
			return new ProductModel(dv[0]);
		}
		#endregion

		#region ~GetProductTopForPreview 先頭の商品取得 (プレビュー用)
		/// <summary>
		/// 先頭の商品取得 (プレビュー用)
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>先頭の商品 (プレビュー用)</returns>
		internal ProductModel GetProductTopForPreview(string shopId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_PRODUCT_SHOP_ID, shopId }
			};
			var dv = Get(XML_KEY_NAME, "GetProductTopForPreview", ht);
			return (dv.Count > 0) ? new ProductModel(dv[0]) : null;
		}
		#endregion
		
		#region ~GetCooperationIdByProductId 商品連携IDを取得
		/// <summary>
		/// 商品連携IDを取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="cooperationIdNum">商品連携IDの番号(1～10)</param>
		/// <returns>商品連携ID</returns>
		internal string GetCooperationIdByProductId(string shopId, string productId, int cooperationIdNum)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_PRODUCT_SHOP_ID, shopId },
				{ Constants.FIELD_PRODUCT_PRODUCT_ID, productId },
				{ "cooperation_id_num", cooperationIdNum },
			};
			var dv = Get(XML_KEY_NAME, "GetCooperationIdByProductId", ht);
			return (dv[0][0] == null) ? null : dv[0][0].ToString();
		}
		#endregion

		#region ~CheckFixedPurchaseProductExistByShippingType 指定する配送種別が設定されている定期商品の存在チェック
		/// <summary>
		/// 指定する配送種別が設定されている定期商品の存在チェック
		/// </summary>
		/// <param name="shopId">ショップID</param>
		/// <param name="shippingType">配送種別</param>
		/// <returns>存在するか</returns>
		internal bool CheckFixedPurchaseProductExistByShippingType(
			string shopId,
			string shippingType)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_PRODUCT_SHOP_ID, shopId },
				{ Constants.FIELD_PRODUCT_SHIPPING_TYPE, shippingType },
			};
			var dv = Get(XML_KEY_NAME, "CheckFixedPurchaseProductExistByShippingType", input);
			return ((int)dv[0][0] > 0);
		}
		#endregion
				
		#region +商品リストを取得
		/// <summary>
		/// 商品リストを取得
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="productIds">Product ids</param>
		/// <returns>Array product model</returns>
		public ProductModel[] GetProducts(string shopId, IEnumerable<string> productIds)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_PRODUCT_SHOP_ID, shopId },
			};
			var productIdsParam = string.Format(
				"'{0}'",
				string.Join("','", productIds.Select(productId => productId)));

			var replaceString = new KeyValuePair<string, string>("@@ product_ids @@", productIdsParam);
			var result = Get(XML_KEY_NAME, "GetProducts", input, null, replaceString);
			return result.Cast<DataRowView>().Select(row => new ProductModel(row)).ToArray();
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
		/// <returns>Product list by group</returns>
		public DataView GetProductListByGroup(
			Hashtable input,
			string advancedSearchWhere,
			string productTagFields,
			string productGroupWhere,
			string productGroupItemOrderBy,
			string favoriteCountOrderBy)
		{
			var replaces = new[]
			{
				new KeyValuePair<string, string>("@@ advanced_search_where @@", advancedSearchWhere),
				new KeyValuePair<string, string>("@@ product_tag_field @@", productTagFields),
				new KeyValuePair<string, string>("@@ product_group_search_where @@", productGroupWhere),
				new KeyValuePair<string, string>("@@ product_group_item_order_by @@", productGroupItemOrderBy),
				new KeyValuePair<string, string>("@@ favorite_count_order_by @@", favoriteCountOrderBy),
			};
			var products = Get(
				XML_KEY_NAME,
				"GetProductListByGroup",
				input,
				replaces: replaces);
			return products;
		}
		#endregion

		#region +GetProductListByGroupAwoo
		/// <summary>
		///	Awooレコメンドに紐づいた商品情報を取得
		/// </summary>
		/// <param name="input">Input</param>
		/// <param name="productIdsWhere">商品ID条件</param>
		/// <returns>商品情報</returns>
		public DataView GetProductListByGroupAwoo(
			Hashtable input,
			string productIdsWhere)
		{
			var replaces = new[]
			{
				new KeyValuePair<string, string>("@@ product_ids_where @@", productIdsWhere),
			};
			var products = Get(
				XML_KEY_NAME,
				"GetProductListByGroupAwoo",
				input,
				replaces: replaces);
			return products;
		}
		#endregion

		#region +マスタ出力
		/// <summary>
		/// マスタをReaderで取得（CSV出力用）
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="statementName">マスタ区分</param>
		/// <param name="replaces">置換値</param>
		/// <returns>Reader</returns>
		public SqlStatementDataReader GetMasterWithReader(Hashtable input, string statementName, params KeyValuePair<string, string>[] replaces)
		{
			var reader = GetWithReader(XML_KEY_NAME, statementName, input, replaces);
			return reader;
		}

		/// <summary>
		/// マスタ取得（Excel出力用）
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="statementName">マスタ区分</param>
		/// <param name="replaces">置換値</param>
		/// <returns>DataView</returns>
		public DataView GetMaster(Hashtable input, string statementName, params KeyValuePair<string, string>[] replaces)
		{
			var dv = Get(XML_KEY_NAME, statementName, input, replaces: replaces);
			return dv;
		}

		/// <summary>
		/// マスタ出力用フィールドチェック
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="masterKbn">マスタ区分</param>
		/// <param name="replaces">置換値</param>
		public void CheckFieldsForGetMaster(Hashtable input, string masterKbn, params KeyValuePair<string, string>[] replaces)
		{
			var statement = string.Empty;

			switch (masterKbn)
			{
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCT: // 商品マスタ表示
					statement = "CheckProductFields";
					break;

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTVARIATION: // 商品価格
					statement = "CheckProductVariationFields";
					break;

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTPRICE: // 商品バリエーション表示
					statement = "CheckProductPriceFields";
					break;

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTTAG: // 商品タグ表示
					statement = "CheckProductTagFields";
					break;

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_PRODUCTEXTEND: // 商品拡張項目表示
					statement = "CheckProductExtendFields";
					break;
			}
			Get(XML_KEY_NAME, statement, input, replaces: replaces);
		}
		#endregion

		#region ~GetCartProducts カートに紐づいた商品情報を取得
		/// <summary>
		/// カートに紐づいた商品情報を取得
		/// </summary>
		/// <param name="cartId">カートID</param>
		/// <returns>商品情報</returns>
		internal ProductModel[] GetCartProducts(string cartId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_CART_CART_ID, cartId }
			};

			var dv = Get(XML_KEY_NAME, "GetCartProducts", input);
			return dv.Cast<DataRowView>().Select(drv => new ProductModel(drv)).ToArray();
		}
		#endregion

		#region +GetProductVariartions
		/// <summary>
		/// 紐づく商品バリエーションを取得
		/// </summary>
		/// <param name="productId">商品ID</param>
		/// <returns>商品バリエーション</returns>
		internal ProductVariationModel[] GetProductVariartions(string productId)
		{
			var dv = Get(
				XML_KEY_NAME,
				"GetProductVariations",
				new Hashtable
				{
					{ Constants.FIELD_PRODUCT_SHOP_ID, Constants.CONST_DEFAULT_SHOP_ID },
					{ Constants.FIELD_PRODUCT_PRODUCT_ID, productId }
				});
			var productVariations = dv.Cast<DataRowView>()
				.Select(drv => new ProductVariationModel(drv))
				.ToArray();
			return productVariations;
		}
		#endregion

		#region +SearchProductsForAutosuggest
		/// <summary>
		/// Search products for autosuggest
		/// </summary>
		/// <param name="memberRankId">Member rank id</param>
		/// <param name="searchWord">Search word</param>
		/// <returns>Products</returns>
		internal ProductModel[] SearchProductsForAutosuggest(string memberRankId, string searchWord)
		{
			var dv = Get(
				XML_KEY_NAME,
				"SearchProductsForAutosuggest",
				new Hashtable
				{
					{ Constants.FIELD_PRODUCT_SHOP_ID, Constants.CONST_DEFAULT_SHOP_ID },
					{ Constants.FIELD_USER_MEMBER_RANK_ID, memberRankId },
					{ "search_word_like_escaped", StringUtility.SqlLikeStringSharpEscape(searchWord) },
				});
			var products = dv.Cast<DataRowView>()
				.Select(drv => new ProductModel(drv))
				.ToArray();
			return products;
		}
		#endregion

		#region +GetProductsLikeIdOrName 指定値で部分一致の商品情報取得
		/// <summary>
		/// 指定値で部分一致の商品情報取得
		/// </summary>
		/// <param name="searchWord">検索値</param>
		/// <returns>商品モデル配列</returns>
		internal ProductModel[] GetProductsLikeIdOrName(string searchWord)
		{
			var dv = Get(
				XML_KEY_NAME,
				"GetProductsLikeIdOrName",
				new Hashtable
				{
					{ Constants.FIELD_PRODUCT_SHOP_ID, Constants.CONST_DEFAULT_SHOP_ID },
					{ "search_word_like_escaped", StringUtility.SqlLikeStringSharpEscape(searchWord) },
				});
			var products = dv.Cast<DataRowView>()
				.Select(drv => new ProductModel(drv))
				.ToArray();
			return products;
		}
		#endregion

		/// <summary>
		/// FLAPS同期処理によって取得した商品を更新する
		/// </summary>
		/// <param name="product">商品</param>
		/// <returns>影響行数</returns>
		internal int InsertForFlapsIntegration(ProductModel product)
		{
			var result = Exec(XML_KEY_NAME, "InsertForFlapsIntegration", product.DataSource);
			return result;
		}

		/// <summary>
		/// FLAPS同期処理によって取得した商品バリエーションを挿入する
		/// </summary>
		/// <param name="pv">商品バリエーション</param>
		/// <returns>影響行数</returns>
		internal int InsertVariationForFlapsIntegration(ProductVariationModel pv)
		{
			var result = Exec(XML_KEY_NAME, "InsertVariationForFlapsIntegration", pv.DataSource);
			return result;
		}

		/// <summary>
		/// FLAPS同期処理によって取得した商品を更新する
		/// </summary>
		/// <param name="product">商品</param>
		/// <returns>影響行数</returns>
		internal int UpdateForFlapsIntegration(ProductModel product)
		{
			var result = Exec(XML_KEY_NAME, "UpdateForFlapsIntegration", product.DataSource);
			return result;
		}

		/// <summary>
		/// FLAPS同期処理によって取得した商品バリエーションを更新する
		/// </summary>
		/// <param name="pv">商品バリエーション</param>
		/// <returns>影響行数</returns>
		internal int UpdateVariationForFlapsIntegration(ProductVariationModel pv)
		{
			var result = Exec(XML_KEY_NAME, "UpdateVariationForFlapsIntegration", pv.DataSource);
			return result;
		}

		#region +GetAll
		/// <summary>
		/// Get all products
		/// </summary>
		/// <returns>A collection of product models</returns>
		internal ProductModel[] GetAll()
		{
			var result = base.Get(XML_KEY_NAME, "GetAll");
			return result.Cast<DataRowView>()
				.Select(row => new ProductModel(row))
				.ToArray();
		}
		#endregion

		#region +GetProductsByProductIds
		/// <summary>
		/// Get products by product ids
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="productIds">Product ids</param>
		/// <returns>A collection of product models</returns>
		internal ProductModel[] GetProductsByProductIds(string shopId, string[] productIds)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_PRODUCT_SHOP_ID, Constants.CONST_DEFAULT_SHOP_ID },
			};
			var replacer = new KeyValuePair<string, string>[]
			{
				new KeyValuePair<string, string>(
					"@@ product_ids @@",
					string.Format("'{0}'", string.Join("','", productIds)))
			};

			var dv = base.Get(
				XML_KEY_NAME,
				"GetProductsByProductIds",
				input,
				replaces: replacer);
			return dv.Cast<DataRowView>()
				.Select(row => new ProductModel(row))
				.ToArray();
		}
		#endregion

		#region +GetProductDetailAtDataView
		/// <summary>
		/// Get product detail at DataView
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="productId">Product id</param>
		/// <returns>A product at DataView</returns>
		internal DataView GetProductDetailAtDataView(
			string shopId,
			string productId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_PRODUCT_SHOP_ID, shopId },
				{ Constants.FIELD_PRODUCT_PRODUCT_ID, productId },
			};
			var dv = base.Get(XML_KEY_NAME, "GetProductDetail", input);
			return dv;
		}
		#endregion

		#region +Insert
		/// <summary>
		/// Insert
		/// </summary>
		/// <param name="model">Model</param>
		/// <returns>Number of rows affected</returns>
		internal int Insert(ProductModel model)
		{
			var result = Exec(XML_KEY_NAME, "Insert", model.DataSource);
			return result;
		}
		#endregion

		#region +Update
		/// <summary>
		/// Update
		/// </summary>
		/// <param name="model">Model</param>
		/// <returns>Number of rows affected</returns>
		internal int Update(ProductModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion

		#region +Delete
		/// <summary>
		/// Delete
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="productId">Product ID</param>
		/// <returns>Number of rows affected</returns>
		internal int Delete(string shopId, string productId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_PRODUCT_SHOP_ID, shopId },
				{ Constants.FIELD_PRODUCT_PRODUCT_ID, productId }
			};

			var result = Exec(XML_KEY_NAME, "Delete", input);
			return result;
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
		/// <returns>True if product variation is existed, otherwise false</returns>
		internal bool IsProductVariationExist(
			string shopId,
			string productId,
			string variationId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_PRODUCTVARIATION_SHOP_ID, shopId },
				{ Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID, productId },
				{ Constants.FIELD_PRODUCTVARIATION_VARIATION_ID, variationId },
			};

			var result = Get(XML_KEY_NAME, "IsProductVariationExist", input);
			return ((int)result[0][0] > 0);
		}
		#endregion

		#region +GetDeleteProductVariations
		/// <summary>
		/// Get delete product variations
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="productId">Product ID</param>
		/// <param name="ignoredVariationIds">Ignored variation ID</param>
		/// <returns>A collection of product variation models</returns>
		internal ProductVariationModel[] GetDeleteProductVariations(
			string shopId,
			string productId,
			string[] ignoredVariationIds)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_PRODUCTVARIATION_SHOP_ID, shopId },
				{ Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID, productId },
			};

			var replacer = new KeyValuePair<string, string>[]
			{
				new KeyValuePair<string, string>(
					"@@ ignored_variation_ids @@",
					string.Format("'{0}'", string.Join("','", ignoredVariationIds)))
			};

			var result = Get(XML_KEY_NAME, "GetDeleteProductVariations", input, replaces: replacer);
			return result.Cast<DataRowView>()
				.Select(row => new ProductVariationModel(row))
				.ToArray();
		}
		#endregion

		#region +GetProductVariationByProductIdAndVariationId
		/// <summary>
		/// Get product variation by product id and variation id
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="productId">Product id</param>
		/// <param name="variationId">Variation</param>
		/// <returns>Model</returns>
		internal ProductVariationModel GetProductVariationByProductIdAndVariationId(
			string shopId,
			string productId,
			string variationId)
		{
			var dv = Get(
				XML_KEY_NAME,
				"GetProductVariationByProductIdAndVariationId",
				new Hashtable
				{
					{ Constants.FIELD_PRODUCTVARIATION_SHOP_ID, shopId },
					{ Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID, productId },
					{ Constants.FIELD_PRODUCTVARIATION_VARIATION_ID, variationId },
				});
			var productVariation = (dv.Count != 0) ? new ProductVariationModel(dv[0]) : null;
			return productVariation;
		}
		#endregion

		#region +InsertProductVariation
		/// <summary>
		/// Insert product variation
		/// </summary>
		/// <param name="model">Model</param>
		/// <returns>Number of rows affected</returns>
		internal int InsertProductVariation(ProductVariationModel model)
		{
			var result = Exec(XML_KEY_NAME, "InsertProductVariation", model.DataSource);
			return result;
		}
		#endregion

		#region +UpdateProductVariation
		/// <summary>
		/// Update product variation
		/// </summary>
		/// <param name="model">Model</param>
		/// <returns>Number of rows affected</returns>
		internal int UpdateProductVariation(ProductVariationModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpdateProductVariation", model.DataSource);
			return result;
		}
		#endregion

		#region +DeleteProductVariations
		/// <summary>
		/// Delete product variations
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="productId">Product ID</param>
		/// <param name="ignoredVariationIds">Ignored variation ID</param>
		/// <returns>Number of rows affected</returns>
		internal int DeleteProductVariations(
			string shopId,
			string productId,
			string[] ignoredVariationIds)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_PRODUCTVARIATION_SHOP_ID, shopId },
				{ Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID, productId }
			};

			var replacer = new KeyValuePair<string, string>[]
			{
				new KeyValuePair<string, string>(
					"@@ ignored_variation_ids @@",
					string.Format("'{0}'", string.Join("','", ignoredVariationIds)))
			};

			var result = Exec(XML_KEY_NAME, "DeleteProductVariations", input, replacer);
			return result;
		}
		#endregion

		#region +DeleteProductVariationAll
		/// <summary>
		/// Delete product variation all
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="productId">Product ID</param>
		/// <returns>Number of rows affected</returns>
		internal int DeleteProductVariationAll(
			string shopId,
			string productId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_PRODUCTVARIATION_SHOP_ID, shopId },
				{ Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID, productId }
			};

			var result = Exec(XML_KEY_NAME, "DeleteProductVariationAll", input);
			return result;
		}
		#endregion

		#endregion

		#region ~GetProductByCooperationId1 商品バリエーション連携ID1から商品情報取得
		/// <summary>
		/// 商品バリエーション連携ID1から商品情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="cooperationId1">商品バリエーション連携ID1</param>
		/// <returns>商品情報</returns>
		internal ProductModel GetProductByCooperationId1(string shopId, string cooperationId1)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_PRODUCT_SHOP_ID, shopId },
				{ Constants.FIELD_PRODUCT_COOPERATION_ID1, cooperationId1 },
			};
			var drv = Get(XML_KEY_NAME, "GetProductByCooperationId1", input)
				.Cast<DataRowView>()
				.FirstOrDefault();
			var result = (drv != null)
				? new ProductModel(drv)
				: null;

			return result;
		}
		#endregion

		#region ~GetProductVariationByCooperationId1 商品バリエーション連携ID1から商品情報取得
		/// <summary>
		/// 商品バリエーション連携ID1から商品情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="cooperationId1">商品バリエーション連携ID1</param>
		/// <returns>商品情報</returns>
		internal ProductModel GetProductVariationByCooperationId1(string shopId, string cooperationId1)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_PRODUCT_SHOP_ID, shopId },
				{ Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID1, cooperationId1 },
			};
			var drv = Get(XML_KEY_NAME, "GetProductVariationByCooperationId1", input)
				.Cast<DataRowView>()
				.FirstOrDefault();
			var result = (drv != null)
				? new ProductModel(drv)
				: null;

			return result;
		}
		#endregion

		/// <summary>
		/// 商品IDが既に「商品ID+バリエーションID」として使用されるかチェック
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <returns>存在するか</returns>
		internal bool CheckProductIdIsUsedAsVariationId(
			string shopId,
			string productId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_PRODUCT_SHOP_ID, shopId },
				{ Constants.FIELD_PRODUCT_PRODUCT_ID, productId },
			};
			var dv = Get(XML_KEY_NAME, "CheckProductIdIsUsedAsVariationId", input);
			return ((int)dv[0][0] > 0);
		}

		/// <summary>
		/// 商品ID＋バリエーションIDが既に「商品ID」として使用されるかチェック
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="variationId">商品バリエーションID</param>
		/// <returns>存在するか</returns>
		internal bool CheckVariationIdIsUsedAsProductId(
			string shopId,
			string variationId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_PRODUCT_SHOP_ID, shopId },
				{ Constants.FIELD_PRODUCT_VARIATION_ID, variationId },
			};
			var dv = Get(XML_KEY_NAME, "CheckVariationIdIsUsedAsProductId", input);
			return ((int)dv[0][0] > 0);
		}

		/// <summary>
		/// 商品ID＋バリエーションIDが既に「商品ID+バリエーショID」として使用されるかチェック
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="variationId">商品バリエーションID</param>
		/// <returns>存在するか</returns>
		internal bool CheckVariationIdIsUsedAsVariationId(
			string shopId,
			string variationId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_PRODUCT_SHOP_ID, shopId },
				{ Constants.FIELD_PRODUCT_VARIATION_ID, variationId },
			};
			var dv = Get(XML_KEY_NAME, "CheckVariationIdIsUsedAsVariationId", input);
			return ((int)dv[0][0] > 0);
		}

		#region ~GetProductForAwooProductSync Awoo連携用商品情報取得
		/// <summary>
		/// Awoo連携用商品データ取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="brandId">ブランドID</param>
		/// <param name="memberRankId">会員ランクID</param>
		/// <param name="userFixedPurchaseMemberFlg">定期会員フラグ</param>
		/// <param name="productIds">商品IDの配列</param>
		/// <returns>商品情報</returns>
		internal DataView GetProductForAwooProductSync(
			string shopId,
			string brandId,
			string memberRankId,
			string userFixedPurchaseMemberFlg,
			string[] productIds)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_DISPPRODUCTINFO_SHOP_ID, shopId },
				{ Constants.FIELD_PRODUCTBRAND_BRAND_ID, brandId },
				{ Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID, memberRankId },
				{ Constants.FIELD_USER_FIXED_PURCHASE_MEMBER_FLG, userFixedPurchaseMemberFlg },
			};

			var productIdsParam = string.Format(
				"'{0}'",
				string.Join("','", productIds.Select(productId => productId)));
			var replaceString = new KeyValuePair<string, string>("@@ product_ids @@", productIdsParam);

			var result = Get(
				XML_KEY_NAME,
				"GetProductRecommendByAwoo",
				input,
				null,
				replaceString);
			return result;
		}
		#endregion

		#region ~GetProductForAwooProductSync Awoo連携用商品情報取得
		/// <summary>
		/// Awoo連携用商品データ取得
		/// </summary>
		/// <returns>商品情報</returns>
		internal DataView GetProductForAwooProductSync()
		{
			var dv = Get(XML_KEY_NAME, "GetProductForAwooProductSync", new Hashtable());
			return dv;
		}
		#endregion

		#region ~GetProductVariationCounts 商品バリエーション件数取得
		/// <summary>
		/// 商品バリエーション件数取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productIds">商品ID</param>
		/// <returns>商品バリエーション件数情報</returns>
		internal Dictionary<string, int> GetProductVariationCounts(string shopId, List<string> productIds)
		{
			// 入力パラメータを準備
			var input = new Hashtable
			{
				{ Constants.FIELD_PRODUCT_SHOP_ID, shopId }
			};

			var productIdsParam = $"'{string.Join("','", productIds)}'";
			var replaceString = new KeyValuePair<string, string>("@@ product_ids @@", productIdsParam);
			
			var dv = Get(XML_KEY_NAME, "GetProductVariationCounts", input, null, replaceString);

			// DataViewをDictionaryに変換
			var result = ProductHelper.ConvertDataViewToDictionaryForProductVariationCounts(dv);

			return result;
		}
		#endregion
	}
}
