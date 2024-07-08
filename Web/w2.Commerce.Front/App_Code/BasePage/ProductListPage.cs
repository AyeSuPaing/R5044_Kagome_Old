/*
=========================================================================================================
  Module      : 商品一覧系基底ページ(ProductListPage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.App.Common.DataCacheController;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Translation;
using w2.App.Common.Order;
using w2.Domain.Favorite;
using w2.Domain.Product;
using w2.Domain.SetPromotion;

/// <summary>
/// 商品一覧系基底ページ
/// </summary>
public abstract class ProductListPage : ProductPage
{
	/// <summary>詳細検索除外パラメタキー</summary>
	private static readonly string[] m_advancedSearchRemoveParamKeys = 
	{
		Constants.REQUEST_KEY_SHOP_ID,
		Constants.REQUEST_KEY_CATEGORY_ID,
		Constants.REQUEST_KEY_SEARCH_WORD,
		Constants.REQUEST_KEY_BRAND_ID,
		Constants.REQUEST_KEY_CAMPAINGN_ICOM,
		Constants.REQUEST_KEY_MIN_PRICE,
		Constants.REQUEST_KEY_MAX_PRICE,
		Constants.REQUEST_KEY_UNDISPLAY_NOSTOCK_PRODUCT,
		Constants.REQUEST_KEY_SORT_KBN,
		Constants.REQUEST_KEY_PAGE_NO,
		Constants.REQUEST_KEY_DISP_PRODUCT_COUNT,
		Constants.REQUEST_KEY_DISP_ONLY_SP_PRICE
	};

	/// <summary>
	/// 商品一覧取得SQLパラメタ作成
	/// </summary>
	/// <returns>SQLパラメタ</returns>
	public abstract Hashtable CreateGetProductListSqlParam();

	/// <summary>
	/// 商品一覧取得
	/// </summary>
	/// <returns>商品一覧データビュー</returns>
	public DataView GetProductsList(Dictionary<string, string> requestParam = null, string productIds = "", string variationIds = "")
	{
		// SQLパラメタ作成
		var ht = CreateGetProductListSqlParam();

		// 商品マスタ、商品タグマスタのカラム取得
		var productMasterColumns = ProductCommon.GetProductColumns();
		var productTagMasterColumns = ProductCommon.GetProductTagColumns(productMasterColumns);
		productMasterColumns.AddRange(productTagMasterColumns);

		// SQL置換文字列作成
		var advancedSearchWhere = (requestParam != null) ? CreateAdvancedSearchWhere(requestParam, productMasterColumns) : string.Empty;
		var productTagFields = (productTagMasterColumns.Count > 0)
			? "w2_ProductTag." + String.Join(", w2_ProductTag.", productTagMasterColumns)
			: "0 AS w2tag_dummy";
		var productGroupWhere = (String.IsNullOrEmpty(this.ProductGroupId) == false) ? ProductCommon.CreateProductGroupWhere() : null;
		var productGroupItemOrderBy =
			((String.IsNullOrEmpty(this.ProductGroupId) == false)
				&& (this.SortKbn == Constants.KBN_SORT_PRODUCT_LIST_PRODUCT_GROUP_ITEM_NO_ASC))
				? ProductCommon.CreateProductGroupItemOrderBy()
				: null;
		var favoriteCountOrderBy = (this.SortKbn == Constants.KBN_SORT_PRODUCT_LIST_FAVORITE_CNT_DESC)
			? ProductCommon.CreateFavariteCountOrderBy()
			: null;

		if (Constants.SETTING_PRODUCT_LIST_SEARCH_KBN
			&& (requestParam != null)
			&& (StringUtility.ToEmpty(requestParam[Constants.REQUEST_KEY_DISP_IMG_KBN]) == Constants.KBN_REQUEST_DIST_IMG_KBN_WINDOWSHOPPING))
		{
			var productList = new ProductService().GetProductListByGroup(
				ht,
				advancedSearchWhere,
				productTagFields,
				productGroupWhere,
				productGroupItemOrderBy,
				favoriteCountOrderBy);
			return productList;
		}

		// 商品一覧情報取得
		using (var accessor = new SqlAccessor())
		using (var statement = new SqlStatement("Product", "GetProductList"))
		{
			statement.Statement = statement.Statement.Replace("@@ advanced_search_where @@", advancedSearchWhere);
			statement.Statement = statement.Statement.Replace("@@ product_tag_field @@", productTagFields);
			statement.Statement = statement.Statement.Replace("@@ product_group_search_where @@", productGroupWhere);
			statement.Statement = statement.Statement.Replace("@@ product_group_item_order_by @@", productGroupItemOrderBy);
			statement.Statement = statement.Statement.Replace("@@ favorite_count_order_by @@", favoriteCountOrderBy);

			statement.Statement = statement.Statement.Replace("@@ product_ids @@", productIds);
			statement.Statement = statement.Statement.Replace("@@ variation_ids @@", variationIds);

			var dv = statement.SelectSingleStatementWithOC(accessor, ht);
			return dv;
		}
	}

	/// <summary>
	/// 詳細検索WHERE作成
	/// </summary>
	/// <param name="requestParamOrg">リクエストパラメタ</param>
	/// <param name="productMasterColumns">商品マスタカラム</param>
	/// <returns> 詳細検索WHERE</returns>
	private string CreateAdvancedSearchWhere(Dictionary<string, string> requestParamOrg, List<string> productMasterColumns)
	{
		// リクエストパラメタから除外キーを除外したものを作成
		var requestParam = requestParamOrg.Where(kvp => m_advancedSearchRemoveParamKeys.Contains(kvp.Key) == false)
			.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

		var result = ProductCommon.CreateAdvancedSearchWhere(requestParam, productMasterColumns).ToString();
		return result;
	}

	/// <summary>
	/// 対象の商品バリエーションを含むセットプロモーションを取得
	/// </summary>
	/// <param name="productId">商品ID</param>
	/// <param name="variationId">商品バリエーションID</param>
	/// <param name="variationList">バリエーションリスト</param>
	/// <returns>該当商品バリエーションを含むセットプロモーション</returns>
	protected SetPromotionModel[] GetSetPromotionByVariation(
		string productId,
		string variationId,
		DataView variationList)
	{
		variationList.RowFilter = string.Format(
			"{0} = '{1}' AND {2} = '{3}'",
			Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID,
			productId,
			Constants.FIELD_PRODUCTVARIATION_VARIATION_ID,
			variationId);

		var setPromotionList = DataCacheControllerFacade.GetSetPromotionCacheController().GetSetPromotionByProduct(
			variationList[0],
			true,
			this.MemberRankId,
			(this.IsPc ? Constants.FLG_ORDER_ORDER_KBN_PC : Constants.FLG_ORDER_ORDER_KBN_SMARTPHONE),
			this.LoginUserHitTargetListIds);

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			setPromotionList = NameTranslationCommon.SetSetPromotionTranslationData(
				setPromotionList,
				RegionManager.GetInstance().Region.LanguageCode,
				RegionManager.GetInstance().Region.LanguageLocaleId);
		}
		return setPromotionList;
	}

	/// <summary>
	/// お気に入りの登録人数取得
	/// </summary>
	/// <param name="productId">商品ID情報</param>
	/// <returns>お気に入りの登録人数</returns>
	protected int GetFavoriteCount(string productId)
	{
		return ((this.FavoriteCountList != null) && this.FavoriteCountList.ContainsKey(productId))
			? (int)this.FavoriteCountList[productId]
			: 0;
	}

	/// <summary>
	/// SEO用商品タグ設定取得
	/// </summary>
	/// <param name="requestParamOrg">リクエストパラメタ</param>
	/// <returns>商品タグ</returns>
	public static DataView GetTagSettingListForSeo(Dictionary<string, string> requestParamOrg = null)
	{
		// 商品マスタ、商品タグマスタのカラム取得
		var productMasterColumns = ProductCommon.GetProductColumns();
		var productTagMasterColumns = ProductCommon.GetProductTagColumns(productMasterColumns);
		productMasterColumns.AddRange(productTagMasterColumns);

		// SQL置換文字列作成
		var advancedSearchWhere = string.Empty;

		if (requestParamOrg != null)
		{
			// リクエストパラメタから除外キーを除外したものを作成
			var requestParam = requestParamOrg.Where(kvp => m_advancedSearchRemoveParamKeys.Contains(kvp.Key) == false)
				.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

			advancedSearchWhere = ProductCommon.CreateAdvancedSearchWhereForSeo(requestParam, productMasterColumns).ToString();
		}

		if (string.IsNullOrEmpty(advancedSearchWhere)) return null;

		// 商品一覧情報取得
		using (var accessor = new SqlAccessor())
		using (var statement = new SqlStatement("ProductTag", "GetTagSettingListForSeo"))
		{
			statement.Statement = statement.Statement.Replace("@@ advanced_search_where @@", advancedSearchWhere);

			var dv = statement.SelectSingleStatementWithOC(accessor);
			return dv;
		}
	}

	/// <summary>
	/// お気に入りデータ設定
	/// </summary>
	/// <param name="productMasterList">商品マスターリスト</param>
	protected void SetFavoriteDataForDisplay(DataView productMasterList)
	{
		// 表示対象の商品ID取得
		var productIds = productMasterList.Cast<DataRowView>().Select(drv => (string)drv[Constants.FIELD_PRODUCT_PRODUCT_ID]).Distinct().ToArray();

		if (productIds.Any())
		{
			// 商品IDリストでお気に入り情報取得
			var favorites = new FavoriteService().GetFavoriteTotalByProduct(this.ShopId, productIds);
			if (favorites.Any())
			{
				// 商品ID毎のお気に入り登録数を設定
				this.FavoriteCountList = new Hashtable();
				foreach (var productId in productIds)
				{
					foreach (var favorite in favorites)
					{
						if ((favorite.Key == productId))
						{
							this.FavoriteCountList.Add(productId, favorite.Value);
						}
					}
				}
			}
		}
	}

	/// <summary>お気に入りの登録人数一覧</summary>
	protected Hashtable FavoriteCountList { get; set; }
}
