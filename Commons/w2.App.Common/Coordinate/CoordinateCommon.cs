/*
=========================================================================================================
  Module      : コーディネート共通処理クラス(CoordinateCommon.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using w2.App.Common.DataCacheController;
using w2.App.Common.Preview;
using w2.App.Common.Web.Page;
using w2.Domain.ContentsTag;
using w2.Domain.Coordinate;
using w2.Domain.Coordinate.Helper;
using w2.Domain.CoordinateCategory;
using w2.Domain.Product;
using w2.Domain.SeoMetadatas;

namespace w2.App.Common.Coordinate
{
	/// <summary>
	/// コーディネート共通クラス
	/// </summary>
	public class CoordinateCommon
	{
		/// <summary>
		/// コーディネート詳細向けSEOメタデータ情報取得
		/// </summary>
		/// <param name="categoryId">カテゴリーID</param>
		/// <param name="coordinate">コーディネート</param>
		/// <returns>SEOメタデータ情報</returns>
		public static SeoMetadatasModel GetSeoMetadatasForCoordinateDetail(string categoryId, CoordinateModel coordinate)
		{
			return GetSeoMetadatas(categoryId, coordinate, Constants.FLG_SEOMETADATAS_DATA_KBN_COORDINATE_DETAIL);
		}

		/// <summary>
		/// コーディネート一覧向けSEOメタデータ情報取得
		/// </summary>
		/// <param name="categoryId">カテゴリーID</param>
		/// <returns>SEOメタデータ情報</returns>
		public static SeoMetadatasModel GetSeoMetadatasForCoordinateList(string categoryId)
		{
			return GetSeoMetadatas(categoryId, null, Constants.FLG_SEOMETADATAS_DATA_KBN_COORDINATE_LIST);
		}

		/// <summary>
		/// コーディネートトップ向けSEOメタデータ情報取得
		/// </summary>
		/// <returns>SEOメタデータ情報</returns>
		public static SeoMetadatasModel GetSeoMetadatasForCoordinateTop()
		{
			return GetSeoMetadatas(string.Empty, null, Constants.FLG_SEOMETADATAS_DATA_KBN_COORDINATE_TOP);
		}

		/// <summary>
		/// SEOメタデータ情報取得
		/// </summary>
		/// <param name="categoryId">カテゴリーID</param>
		/// <param name="targetPage">SEOメタデータ区分</param>
		/// <param name="coordinate">コーディネート</param>
		/// <returns>SEOメタデータ情報</returns>
		private static SeoMetadatasModel GetSeoMetadatas(string categoryId, CoordinateModel coordinate ,string targetPage)
		{
			// SEOメタデータ情報取得
			var seoSettingsModel = DataCacheControllerFacade.GetSeoMetadatasCacheController().CacheData
				.FirstOrDefault(m => (m.DataKbn == targetPage));
			if (seoSettingsModel == null) return new SeoMetadatasModel();

			// 置換用データ取得
			var repaceDatas = GetRepaceDatas(categoryId, coordinate, targetPage);

			// 置換処理
			var title = new StringBuilder(seoSettingsModel.HtmlTitle);
			var description = new StringBuilder(seoSettingsModel.MetadataDesc);
			foreach (string key in repaceDatas.Keys)
			{
				title.Replace(key, (string)repaceDatas[key]);
				description.Replace(key, (string)repaceDatas[key]);
			}

			var seoMetadatas = new SeoMetadatasModel
			{
				HtmlTitle = RemoveSeoSettingKey(title.ToString()),
				MetadataDesc = RemoveSeoSettingKey(description.ToString())
			};

			return seoMetadatas;
		}

		/// <summary>
		/// 置換データ取得
		/// </summary>
		/// <param name="categoryId">カテゴリーID</param>
		/// <param name="targetPage">SEOメタデータ区分</param>
		/// <param name="coordinate">コーディネート</param>
		/// <returns>置換データ</returns>
		private static Hashtable GetRepaceDatas(string categoryId,  CoordinateModel coordinate, string targetPage)
		{
			var repaceData = new Hashtable();

			// コーディネート詳細データ・セット
			if (targetPage == Constants.FLG_SEOMETADATAS_DATA_KBN_COORDINATE_DETAIL)
			{
				var productNameList = new List<string>();
				foreach (var product in coordinate.ProductList)
				{
					productNameList.Add(product.Name);
				}

				repaceData.Add(Constants.SEOSETTING_KEY_PRODUCT_NAMES, string.Join(",", productNameList));
				repaceData.Add(Constants.SEOSETTING_KEY_TITLE, coordinate.CoordinateTitle);
				repaceData.Add(Constants.SEOSETTING_KEY_SEO_TITLE, coordinate.HtmlTitle);
				repaceData.Add(Constants.SEOSETTING_KEY_SEO_DESCRIPTION, coordinate.MetadataDesc);
			}

			// コーディネートカテゴリー情報セット
			var categoryService = new CoordinateCategoryService();
			var category = categoryService.Get(categoryId);
			if ((category != null) && (category.ValidFlg == Constants.FLG_COORDINATECATEGORY_VALID_FLG_VALID))
			{
				// 親カテゴリ名
				var parentCategoryName = (category.CoordinateParentCategoryId == Constants.FLG_COORDINATECATEGORY_ROOT)
					? CommonPage.ReplaceTag("@@DispText.seo_text.category_top@@")
					: categoryService.Get(category.CoordinateParentCategoryId).CoordinateCategoryName;
				repaceData.Add(Constants.SEOSETTING_KEY_PARENT_CATEGORY_NAME, parentCategoryName);

				// カテゴリ名
				repaceData.Add(Constants.SEOSETTING_KEY_CATEGORY_NAME, category.CoordinateCategoryName);
			}

			if (targetPage == Constants.FLG_SEOMETADATAS_DATA_KBN_COORDINATE_LIST)
			{
				// 同一コーディネートカテゴリー名情報取得
				var categoryFamily = new CoordinateCategoryService().GetCoordinateCategoryFamily(categoryId);
				if (categoryFamily != null)
				{
					// 取得したカテゴリの子から上位X個取得する
					var categoryNames = categoryFamily.Select(c => c.CoordinateCategoryName)
						.Take(Constants.SEOSETTING_CHILD_CATEGORY_TOP_COUNT);
					var categories = string.Join(",", categoryNames);
					repaceData.Add(Constants.SEOSETTING_KEY_CHILD_CATEGORY_TOP, categories);
				}
			}

			return repaceData;
		}

		/// <summary>
		/// プレビュー情報を取得
		/// </summary>
		/// <returns>コーディネートモデル</returns>
		public static CoordinateModel GetPreview(string shopId, string coordinateId)
		{
			var dv = ProductPreview.GetPreview(
				Constants.FLG_PREVIEW_PREVIEW_KBN_COORDINATE_DETAIL,
				shopId,
				coordinateId,
				"",
				"",
				"");

			var models = dv.Cast<DataRowView>().Select(drv => new CoordinateListSearchResult(drv)).ToArray();
			var master = models[0];
			master.TagList = new List<ContentsTagModel>();
			master.CategoryList = new List<CoordinateCategoryModel>();
			master.ProductList = new List<ProductModel>();

			foreach (var model in models)
			{
				switch (model.ItemKbn)
				{
					case Constants.FLG_COORDINATE_ITEM_KBN_TAG:
						master.TagList.Add(new ContentsTagService().Get(Int64.Parse(model.ItemId)));
						break;

					case Constants.FLG_COORDINATE_ITEM_KBN_CATEGORY:
						master.CategoryList.Add(new CoordinateCategoryService().Get(model.ItemId));
						break;

					case Constants.FLG_COORDINATE_ITEM_KBN_PRODUCT:
						ProductModel productInfo;
						var productService = new ProductService();
						if (model.ItemId == model.ItemId2)
						{
							productInfo = productService.Get(shopId, model.ItemId);
							productInfo.VariationId = model.ItemId;
						}
						else
						{
							productInfo  = productService.GetProductVariation(shopId, model.ItemId, model.ItemId2, string.Empty);
						}
						master.ProductList.Add(productInfo);
						break;
				}
			}
			return master;
		}

		/// <summary>
		/// Seo設定値を削除
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static string RemoveSeoSettingKey(string text)
		{
			text = text.Replace(Constants.SEOSETTING_KEY_PRODUCT_NAME, string.Empty);
			text = text.Replace(Constants.SEOSETTING_KEY_PRODUCT_PRICE, string.Empty);
			text = text.Replace(Constants.SEOSETTING_KEY_VARIATION_NAME1, string.Empty);
			text = text.Replace(Constants.SEOSETTING_KEY_VARIATION_NAME2, string.Empty);
			text = text.Replace(Constants.SEOSETTING_KEY_VARIATION_NAME3, string.Empty);
			text = text.Replace(Constants.SEOSETTING_KEY_PARENT_CATEGORY_NAME, string.Empty);
			text = text.Replace(Constants.SEOSETTING_KEY_CATEGORY_NAME, string.Empty);
			text = text.Replace(Constants.SEOSETTING_KEY_PRODUCT_SEO_KEYWORDS, string.Empty);
			text = text.Replace(Constants.SEOSETTING_KEY_CHILD_CATEGORY_TOP, string.Empty);
			text = text.Replace(Constants.SEOSETTING_KEY_BRAND_TITLE, string.Empty);
			text = text.Replace(Constants.SEOSETTING_KEY_BRAND_SEO_KEYWORD, string.Empty);
			text = text.Replace(Constants.SEOSETTING_KEY_SEO_TITLE, string.Empty);
			text = text.Replace(Constants.SEOSETTING_KEY_SEO_DESCRIPTION, string.Empty);
			text = text.Replace(Constants.SEOSETTING_KEY_TITLE, string.Empty);
			text = text.Replace(Constants.SEOSETTING_KEY_PRODUCT_NAMES, string.Empty);
			return text;
		}
	}
}
