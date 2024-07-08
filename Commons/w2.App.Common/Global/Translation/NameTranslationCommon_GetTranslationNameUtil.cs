/*
=========================================================================================================
  Module      : 名称翻訳処理クラス(翻訳名称取得ユーティリティ) (NameTranslationCommon_GetTranslationNameUtil.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using w2.App.Common.Global.Region;
using w2.App.Common.Order;
using w2.Domain.NameTranslationSetting;
using w2.Domain.NameTranslationSetting.Helper;
using w2.Domain.Product;

namespace w2.App.Common.Global.Translation
{
	/// <summary>
	/// 名称翻訳処理クラス(翻訳名称取得ユーティリティ)
	/// </summary>
	public partial class NameTranslationCommon
	{
		#region +GetTranslationName 翻訳名取得
		/// <summary>
		/// 翻訳名取得
		/// </summary>
		/// <param name="masterId1">マスタID1</param>
		/// <param name="dataKbn">データ区分</param>
		/// <param name="translateTargetColumnName">翻訳対象項目</param>
		/// <param name="beforeTranslationName">翻訳前名称</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>翻訳後名称</returns>
		public static string GetTranslationName(
			string masterId1,
			string dataKbn,
			string translateTargetColumnName,
			string beforeTranslationName,
			string languageCode = null,
			string languageLocaleId = null)
		{
			if (Constants.GLOBAL_OPTION_ENABLE == false) return beforeTranslationName;

			var searchCondition = new NameTranslationSettingSearchCondition
			{
				DataKbn = dataKbn,
				TranslationTargetColumn = translateTargetColumnName,
				MasterId1 = masterId1,
				LanguageCode = languageCode ?? RegionManager.GetInstance().Region.LanguageCode,
				LanguageLocaleId = languageLocaleId ?? RegionManager.GetInstance().Region.LanguageLocaleId,
			};
			var translationSetting =
				new NameTranslationSettingService().GetAfterTranslationalNameSetting(searchCondition);
			var afterTranslationName = (translationSetting != null)
				? translationSetting.AfterTranslationalName
				: beforeTranslationName;

			return afterTranslationName;
		}
		#endregion

		#region +GetOrderItemProductTranslationName 注文商品翻訳名取得
		/// <summary>
		/// 注文商品翻訳名取得
		/// </summary>
		/// <param name="orderItemProductName">注文商品名(w2_OrderItem.product_nameに登録された値)</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">商品バリエーションID</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>注文商品翻訳名</returns>
		public static string GetOrderItemProductTranslationName(
			string orderItemProductName,
			string productId,
			string variationId,
			string languageCode,
			string languageLocaleId)
		{
			// 商品情報を取得
			var product = new ProductService().GetProductVariation(
				Constants.CONST_DEFAULT_SHOP_ID,
				productId,
				variationId,
				string.Empty);
			if (product == null) return orderItemProductName;

			// 表示するのがProductJointNameなので、商品名＋バリエーション名の翻訳情報を取得する
			var translationSettings = product.HasProductVariation
				? GetProductAndVariationTranslationSettingsByVariationId(
					productId,
					variationId,
					languageCode,
					languageLocaleId)
				: GetProductAndVariationTranslationSettingsByProductId(
					productId,
					languageCode,
					languageLocaleId);

			// ProductJointName生成
			var productTranslationName = CreateProductJointTranslationName(
				translationSettings,
				product.Name,
				product.VariationName1,
				product.VariationName2,
				product.VariationName3,
				product.HasProductVariation);

			return productTranslationName;
		}
		#endregion

		#region +CreateProductJointTranslationName 商品結合翻訳名作成
		/// <summary>
		/// 商品結合翻訳名作成
		/// </summary>
		/// <param name="translationSettings">翻訳設定情報</param>
		/// <param name="productName">翻訳前商品名</param>
		/// <param name="variationName1">翻訳前バリエーション名1</param>
		/// <param name="variationName2">翻訳前バリエーション名2</param>
		/// <param name="variationName3">翻訳前バリエーション名3</param>
		/// <param name="hasVariation">バリエーションが存在するか</param>
		/// <returns>商品結合翻訳名</returns>
		public static string CreateProductJointTranslationName(
			NameTranslationSettingModel[] translationSettings,
			string productName,
			string variationName1,
			string variationName2,
			string variationName3,
			bool hasVariation)
		{
			var productTranslationNameSetting = translationSettings.FirstOrDefault(
				setting => setting.TranslationTargetColumn
					== Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_NAME);

			var productJointTranslationName = (productTranslationNameSetting != null)
				? productTranslationNameSetting.AfterTranslationalName
				: productName;

			if (hasVariation)
			{
				var variationTranslationSettings = translationSettings.Where(
					setting => setting.DataKbn == Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTVARIATION).ToArray();

				var variationName1Setting = variationTranslationSettings.FirstOrDefault(
					setting => setting.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCTVARIATION_VARIATION_NAME1);
				var variationName2Setting = variationTranslationSettings.FirstOrDefault(
					setting => setting.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCTVARIATION_VARIATION_NAME2);
				var variationName3Setting = variationTranslationSettings.FirstOrDefault(
					setting => setting.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCTVARIATION_VARIATION_NAME3);

				var variationTranslationName1 = (variationName1Setting != null) ? variationName1Setting.AfterTranslationalName : variationName1;
				var variationTranslationName2 = (variationName2Setting != null) ? variationName2Setting.AfterTranslationalName : variationName2;
				var variationTranslationName3 = (variationName3Setting != null) ? variationName3Setting.AfterTranslationalName : variationName3;

				productJointTranslationName += ProductCommon.CreateVariationName(
					variationTranslationName1,
					variationTranslationName2,
					variationTranslationName3);
			}
			return productJointTranslationName;
		}
		#endregion

		#region +GetBrandNameTranslationName ブランド翻訳名取得
		/// <summary>
		/// ブランド翻訳名取得
		/// </summary>
		/// <param name="brandId">ブランドID</param>
		/// <param name="beforeBrandName">翻訳前ブランド名</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>翻訳後ブランド名</returns>
		public static string GetBrandNameTranslationName(
			string brandId,
			string beforeBrandName,
			string languageCode = null,
			string languageLocaleId = null)
		{
			var result = GetTranslationName(
				brandId,
				Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTBRAND,
				Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCTBRAND_BRAND_NAME,
				beforeBrandName,
				languageCode,
				languageLocaleId);

			return result;
		}
		#endregion

		#region +GetCategoryNameTranslationName カテゴリ翻訳名取得
		/// <summary>
		/// カテゴリ翻訳名取得
		/// </summary>
		/// <param name="categoryId">カテゴリID</param>
		/// <param name="beforeCategoryName">翻訳前カテゴリ名</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>翻訳後カテゴリ名</returns>
		public static string GetCategoryNameTranslationName(
			string categoryId,
			string beforeCategoryName,
			string languageCode = null,
			string languageLocaleId = null)
		{
			var result = GetTranslationName(
				categoryId,
				Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTCATEGORY,
				Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCTCATEGORY_NAME,
				beforeCategoryName,
				languageCode,
				languageLocaleId);

			return result;
		}
		#endregion
	}
}
