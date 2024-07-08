/*
=========================================================================================================
  Module      : 商品共通処理クラスSEO設定部分(ProductCommon_SeoSetting.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using w2.App.Common.DataCacheController;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Product;
using w2.Common.Extensions;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.SeoMetadatas;

namespace w2.App.Common.Order
{
	/// <summary>
	/// 商品共通処理クラス（SEOメタデータ部分）
	/// </summary>
	public partial class ProductCommon
	{
		/// <summary>SEOマッピングタイプカラー</summary>
		private const string SEO_MAPPING_TYPE_COLOR = "color";
		/// <summary>アイコンフラグ</summary>
		private const string ICON_FLG = "icon_flg";
		/// <summary>テキストボックスタイプタイトル</summary>
		private const string TEXT_BOX_TYPE_TITLE = "title";
		/// <summary>テキストボックスタイプキーワード</summary>
		private const string TEXT_BOX_TYPE_KEY_WORD = "keyWord";
		/// <summary>テキストボックスタイプ概要</summary>
		private const string TEXT_BOX_TYPE_DESCRIPTION = "description";
		/// <summary>テキストボックスタイプコメント</summary>
		private const string TEXT_BOX_TYPE_COMMENT = "comment";
		/// <summary>テキストボックスタイプSeoテキスト</summary>
		private const string TEXT_BOX_TYPE_SEO_TEXT = "seoText";
		/// <summary>キャンペーンアイコンタグリスト</summary>
		private static readonly List<string> CAMPAIN_ICON_TAG_LIST = new List<string>()
		{
			{ "@@ product_icon1 @@" },
			{ "@@ product_icon2 @@" },
			{ "@@ product_icon3 @@" },
			{ "@@ product_icon4 @@" },
			{ "@@ product_icon5 @@" },
			{ "@@ product_icon6 @@" },
			{ "@@ product_icon7 @@" },
			{ "@@ product_icon8 @@" },
			{ "@@ product_icon9 @@" },
			{ "@@ product_icon10 @@" },
		};
		/// <summary>親カテゴリID開始位置</summary>
		private const int PARENT_CATEGORY_ID_START_IDX = 0;
		/// <summary>親カテゴリID文字列長</summary>
		private const int PARENT_CATEGORY_ID_LENGTH = 3;
		/// <summary>ロックオブジェクト</summary>
		private static readonly object s_lockObject = new object();

		/// <summary>
		/// 商品詳細向けSEOメタデータ情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="categoryId">カテゴリーID</param>
		/// <param name="brandId">ブランドID</param>
		/// <param name="product">商品情報</param>
		/// <returns>SEOメタデータ情報</returns>
		public static SeoMetadatas GetSeoMetadatasForProductDetail(
			string shopId,
			string categoryId,
			string brandId,
			DataRowView product)
		{
			return GetSeoMetadatas(
				shopId,
				categoryId,
				brandId,
				null,
				Constants.FLG_SEOMETADATAS_DATA_KBN_PRODUCT_DETAIL,
				product,
				"",
				"",
				"",
				"",
				"",
				"");
		}

		/// <summary>
		/// 商品一覧向けSEOメタデータ情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="categoryId">カテゴリーID</param>
		/// <param name="brandId">ブランドID</param>
		/// <param name="productGroupId">商品グループID</param>
		/// <param name="product">商品情報</param>
		/// <param name="priceMin">最小価格</param>
		/// <param name="priceMax">最大価格</param>
		/// <param name="color">カラー</param>
		/// <param name="campaignIcon">キャンペーンアイコン</param>
		/// <param name="productTag">商品タグ</param>
		/// <param name="searchWord">検索ワード</param>
		/// <returns>SEOメタデータ情報</returns>
		public static SeoMetadatas GetSeoMetadatasForProductList(
			string shopId,
			string categoryId,
			string brandId,
			string productGroupId,
			DataRowView product,
			string priceMin,
			string priceMax,
			string color,
			string searchWord,
			string campaignIcon,
			string productTag)
		{
			return GetSeoMetadatas(
				shopId,
				categoryId,
				brandId,
				productGroupId,
				Constants.FLG_SEOMETADATAS_DATA_KBN_PRODUCT_LIST,
				product,
				priceMin,
				priceMax,
				color,
				searchWord,
				campaignIcon,
				productTag);
		}

		/// <summary>
		/// SEOメタデータ情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="categoryId">カテゴリーID</param>
		/// <param name="brandId">ブランドID</param>
		/// <param name="productGroupId">商品グループID</param>
		/// <param name="targetPage">SEOメタデータ区分</param>
		/// <param name="product">商品情報</param>
		/// <param name="priceMin">最小価格</param>
		/// <param name="priceMax">最大価格</param>
		/// <param name="color">カラー</param>
		/// <param name="campaignIcon">キャンペーンアイコン</param>
		/// <param name="productTag">商品タグ</param>
		/// <param name="searchWord">検索ワード</param>
		/// <returns>SEOメタデータ情報</returns>
		private static SeoMetadatas GetSeoMetadatas(
			string shopId,
			string categoryId,
			string brandId,
			string productGroupId,
			string targetPage,
			DataRowView product,
			string priceMin,
			string priceMax,
			string color,
			string searchWord,
			string campaignIcon,
			string productTag)
		{
			// SEOメタデータ情報取得 FirstOrDefault は読み取り操作なので排他制御が必要そう インフラ調査結果対応（PRODUCT_BASE-1024）
			SeoMetadatasModel seoSettingsModel;
			var seoMetadatasCacheOrDb = DataCacheControllerFacade.GetSeoMetadatasCacheController().CacheData;
			lock(s_lockObject)
			{
				seoSettingsModel = seoMetadatasCacheOrDb.FirstOrDefault(m => (m.ShopId == shopId) && (m.DataKbn == targetPage));
			}
			if (seoSettingsModel == null) return new SeoMetadatas();

			// 置換用データ取得
			var replaceDatas = GetReplaceDatas(
				shopId,
				categoryId,
				brandId,
				productGroupId,
				targetPage,
				product,
				priceMin,
				priceMax,
				color,
				searchWord,
				campaignIcon,
				productTag);

			// 置換処理
			var title = new StringBuilder(seoSettingsModel.HtmlTitle);
			var keywords = new StringBuilder(seoSettingsModel.MetadataKeywords);
			var description = new StringBuilder(seoSettingsModel.MetadataDesc);
			var comment = new StringBuilder(seoSettingsModel.Comment);
			var defaultText = new StringBuilder(seoSettingsModel.DefaultText);

			TagConvertResultEmptyFlgsForTitle = new List<int>();
			TagConvertResultEmptyFlgsForKeyWord = new List<int>();
			TagConvertResultEmptyFlgsForDescription = new List<int>();
			TagConvertResultEmptyFlgsForComment = new List<int>();

			// replaceDatasのループ読み取り操作が存在するため排他制御 インフラ調査結果対応（PRODUCT_BASE-1024）
			lock (s_lockObject)
			{
				title = ReplaceTarget(title, replaceDatas, TEXT_BOX_TYPE_TITLE);
				keywords = ReplaceTarget(keywords, replaceDatas, TEXT_BOX_TYPE_KEY_WORD);
				description = ReplaceTarget(description, replaceDatas, TEXT_BOX_TYPE_DESCRIPTION);
				comment = ReplaceTarget(comment, replaceDatas, TEXT_BOX_TYPE_COMMENT);
			}

			title = ConvertDefaultText(title, defaultText, IsAllTagConvertResultEmptyForTitle);
			keywords = ConvertDefaultText(keywords, defaultText, IsAllTagConvertResultEmptyForKeyWord);
			description = ConvertDefaultText(description, defaultText, IsAllTagConvertResultEmptyForDescription);
			comment = ConvertDefaultText(comment, defaultText, IsAllTagConvertResultEmptyForComment);

			// SEOメタデータ情報をハッシュテーブルに格納
			var seoMetadatas = new SeoMetadatas((Hashtable)seoSettingsModel.DataSource.Clone())
			{
				HtmlTitle = title.ToString(),
				MetadataKeywords = keywords.ToString(),
				MetadataDesc = description.ToString(),
				Comment = comment.ToString(),
			};
			return seoMetadatas;
		}

		/// <summary>
		/// 商品一覧用SEOテキスト取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="categoryId">カテゴリーID</param>
		/// <param name="brandId">ブランドID</param>
		/// <param name="productGroupId">商品グループID</param>
		/// <param name="priceMin">最小価格</param>
		/// <param name="priceMax">最大価格</param>
		/// <param name="color">カラー</param>
		/// <param name="searchWord">検索ワード</param>
		/// <param name="campaignIcon">キャンペーンアイコン</param>
		/// <param name="productTag">商品タグ</param>
		/// <param name="target">置換対象</param>
		/// <returns>SEOテキスト</returns>
		public static string GetSeoTextForProductList(
			string shopId,
			string categoryId,
			string brandId,
			string productGroupId,
			string priceMin,
			string priceMax,
			string color,
			string searchWord,
			string campaignIcon,
			string productTag,
			StringBuilder target)
		{
			// 置換用データ取得
			var replaceDatas = GetReplaceDatas(
				shopId,
				categoryId,
				brandId,
				productGroupId,
				Constants.FLG_SEOMETADATAS_DATA_KBN_PRODUCT_LIST,
				null,
				priceMin,
				priceMax,
				color,
				searchWord,
				campaignIcon,
				productTag);
			TagConvertResultEmptyFlgsForSeoText = new List<int>();

			// SEOメタデータ情報取得（デフォルト文言取得用）
			var seoSettingsModel = DataCacheControllerFacade.GetSeoMetadatasCacheController().CacheData.FirstOrDefault(
				m => (m.ShopId == shopId) && (m.DataKbn == Constants.FLG_SEOMETADATAS_DATA_KBN_PRODUCT_LIST));

			var defaultText = (seoSettingsModel == null)
				? new StringBuilder("")
				: new StringBuilder(seoSettingsModel.DefaultText);

			// 置換処理
			target = ReplaceTarget(target, replaceDatas, TEXT_BOX_TYPE_SEO_TEXT);
			target = ConvertDefaultText(target, defaultText, IsAllTagConvertResultEmptyForSeoText);
			return target.ToString();
		}

		/// <summary>
		/// 置換文字列の有無で判定し、囲みタグの置換結果を返す
		/// </summary>
		/// <param name="target">対象文字列</param>
		/// <param name="replaceValue">置換文字列</param>
		/// <param name="boxTagStart">開始囲いタグ</param>
		/// <param name="boxTagEnd">終了囲いタグ</param>
		/// <param name="convertTagKey">置換タグキー</param>
		/// <param name="textBoxType">テキストボックスタイプ</param>
		/// <returns>置換結果</returns>
		private static string GetTargetResultConvertBoxTag(
			string target,
			string replaceValue,
			string boxTagStart,
			string boxTagEnd,
			string convertTagKey,
			string textBoxType)
		{
			SetEmptyFlg(target, replaceValue, textBoxType, convertTagKey);

			if (string.IsNullOrEmpty(replaceValue))
			{
				if (CheckExistBoxTag(target, boxTagStart, boxTagEnd))
				{
					return target.Replace(
						target.Substring(
							target.IndexOf(boxTagStart),
							(target.IndexOf(boxTagEnd) + boxTagEnd.Length - target.IndexOf(boxTagStart))),
						"");
				}
				else
				{
					return target.Replace(convertTagKey, replaceValue);
				}
			}
			else
			{
				return target.Replace(boxTagStart, "").Replace(boxTagEnd, "").Replace(convertTagKey, replaceValue);
			}
		}

		/// <summary>
		/// 空文字フラグ設定
		/// </summary>
		/// <param name="target">対象文字列</param>
		/// <param name="replaceValue">置換文字列</param>
		/// <param name="textBoxType">テキストボックスタイプ</param>
		/// <param name="convertTagKey">置換タグキー</param>
		private static void SetEmptyFlg(string target, string replaceValue, string textBoxType, string convertTagKey)
		{
			if (target.Contains(convertTagKey) == false) return;

			var emptyFlg = 0;
			if (string.IsNullOrEmpty(replaceValue))
			{
				emptyFlg = 1;
			}

			switch (textBoxType)
			{
				case TEXT_BOX_TYPE_TITLE:
					TagConvertResultEmptyFlgsForTitle.Add(emptyFlg);
					break;

				case TEXT_BOX_TYPE_KEY_WORD:
					TagConvertResultEmptyFlgsForKeyWord.Add(emptyFlg);
					break;

				case TEXT_BOX_TYPE_DESCRIPTION:
					TagConvertResultEmptyFlgsForDescription.Add(emptyFlg);
					break;

				case TEXT_BOX_TYPE_COMMENT:
					TagConvertResultEmptyFlgsForComment.Add(emptyFlg);
					break;

				case TEXT_BOX_TYPE_SEO_TEXT:
					TagConvertResultEmptyFlgsForSeoText.Add(emptyFlg);
					break;
			}
		}

		/// <summary>
		/// 囲いタグの存在チェック
		/// </summary>
		/// <param name="target">対象文字列</param>
		/// <param name="boxTagStart">開始囲いタグ</param>
		/// <param name="boxTagEnd">終了囲いタグ</param>
		/// <returns>囲いタグが存在するか</returns>
		private static bool CheckExistBoxTag(string target, string boxTagStart, string boxTagEnd)
		{
			return (target.Contains(boxTagStart) && target.Contains(boxTagEnd));
		}

		/// <summary>
		/// 置換用データをもとに置換対象文字列を置換する
		/// </summary>
		/// <param name="target">対象文字列</param>
		/// <param name="replaceDatas">置換用データ</param>
		/// <param name="textBoxType">テキストボックスタイプ</param>
		/// <returns>置換結果</returns>
		private static StringBuilder ReplaceTarget(StringBuilder target, Hashtable replaceDatas, string textBoxType)
		{
			foreach (string key in replaceDatas.Keys)
			{
				var boxTagKey = key.Replace("@@ ", "").Replace(" @@", "");
				var boxTagStart = string.Format("{0}{1}{2}", "<@@", boxTagKey, "@@>");
				var boxTagEnd = string.Format("{0}{1}{2}", "</@@", boxTagKey, "@@>");
				var replaceResultValue = (string)replaceDatas[key];
				target = new StringBuilder(GetTargetResultConvertBoxTag(
					target.ToString(),
					replaceResultValue,
					boxTagStart,
					boxTagEnd,
					key,
					textBoxType));
			}
			return target;
		}

		/// <summary>
		/// デフォルトタグの置換
		/// </summary>
		/// <param name="target">対象文字列</param>
		/// <param name="defaultText">デフォルト文言</param>
		/// <param name="isAllTagConvertResultEmpty">タグ置換の結果がすべて空文字か</param>
		/// <remarks>タイトルタグ、アイコンタグ、カラータグ等のタグ置換結果が空の場合、デフォルト文言を表示する</remarks>
		/// <returns>置換結果</returns>
		private static StringBuilder ConvertDefaultText(
			StringBuilder target,
			StringBuilder defaultText,
			bool isAllTagConvertResultEmpty)
		{
			var convertResult = (target.ToString().Contains(Constants.SEOSETTING_KEY_DEFAULT_TEXT)
				&& isAllTagConvertResultEmpty)
					? target.Replace(Constants.SEOSETTING_KEY_DEFAULT_TEXT, defaultText.ToString())
					: target.Replace(Constants.SEOSETTING_KEY_DEFAULT_TEXT, "");

			return convertResult;
		}

		/// <summary>
		/// 置換データ取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="categoryId">カテゴリーID</param>
		/// <param name="brandId">ブランドID</param>
		/// <param name="productGroupId">商品グループID</param>
		/// <param name="targetPage">SEOメタデータ区分</param>
		/// <param name="product">商品情報</param>
		/// <param name="priceMin">最小価格</param>
		/// <param name="priceMax">最大価格</param>
		/// <param name="color">カラー</param>
		/// <param name="campaignIcon">キャンペーンアイコン</param>
		/// <param name="productTag">商品タグ</param>
		/// <param name="searchWord">検索ワード</param>
		/// <returns>置換データ</returns>
		private static Hashtable GetReplaceDatas(
			string shopId,
			string categoryId,
			string brandId,
			string productGroupId,
			string targetPage,
			DataRowView product,
			string priceMin,
			string priceMax,
			string color,
			string searchWord,
			string campaignIcon,
			string productTag)
		{
			var repaceData = new Hashtable();

			// 商品データ・セット
			if (targetPage == Constants.FLG_SEOMETADATAS_DATA_KBN_PRODUCT_DETAIL)
			{
				repaceData.Add(Constants.SEOSETTING_KEY_PRODUCT_NAME, product[Constants.FIELD_PRODUCT_NAME]);
				repaceData.Add(Constants.SEOSETTING_KEY_PRODUCT_NAME_KANA, product[Constants.FIELD_PRODUCT_NAME_KANA]);
				repaceData.Add(Constants.SEOSETTING_KEY_PRODUCT_PRICE, CurrencyManager.ToPrice(ProductCommon.GetProductPriceNumeric(product, true)));
				repaceData.Add(Constants.SEOSETTING_KEY_VARIATION_NAME1, product[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1]);
				repaceData.Add(Constants.SEOSETTING_KEY_VARIATION_NAME2, product[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2]);
				repaceData.Add(Constants.SEOSETTING_KEY_VARIATION_NAME3, product[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3]);
				repaceData.Add(Constants.SEOSETTING_KEY_PRODUCT_SEO_KEYWORDS, product[Constants.FIELD_PRODUCT_SEO_KEYWORDS]);
			}

			// 商品カテゴリー情報セット
			var categoryData = GetCategoryData(shopId, categoryId);
			if (categoryData.Count != 0)
			{
				var parentCategoryName = ((string)categoryData[0][Constants.FIELD_PRODUCTCATEGORY_PARENT_CATEGORY_ID] == "root")
					? ""
					: categoryData[0]["parent_category_name"].ToString();

				var rootParentCategoryName = "";
				if (string.IsNullOrEmpty(parentCategoryName) == false)
				{
					var rootParentCategoryId = (categoryId.Length >= PARENT_CATEGORY_ID_LENGTH)
						? categoryId.Substring(PARENT_CATEGORY_ID_START_IDX, PARENT_CATEGORY_ID_LENGTH)
						: "";
					var rootParentCategoryData = GetCategoryData(shopId, rootParentCategoryId);
					rootParentCategoryName = rootParentCategoryData[0]["category_name"].ToString();
				}
				if (parentCategoryName == rootParentCategoryName)
				{
					parentCategoryName = "";
				}

				repaceData.Add(Constants.SEOSETTING_KEY_PARENT_CATEGORY_NAME, parentCategoryName);
				repaceData.Add(Constants.SEOSETTING_KEY_ROOT_PARENT_CATEGORY_NAME, rootParentCategoryName);


				// 商品グループIDが指定されている場合は、商品特集グループ設定名を設定する
				repaceData.Add(Constants.SEOSETTING_KEY_CATEGORY_NAME,
					string.IsNullOrEmpty(productGroupId) ? categoryData[0]["category_name"] : GetProductGroupName(productGroupId));

				if (targetPage == Constants.FLG_SEOMETADATAS_DATA_KBN_PRODUCT_LIST)
				{
					repaceData.Add(Constants.SEOSETTING_KEY_PRODUCT_SEO_KEYWORDS, categoryData[0][Constants.FIELD_PRODUCTCATEGORY_SEO_KEYWORDS]);

					// SEO表示用アイコン1～10
					var productDefaultSetting = new ProductDefaultSetting.ProductDefaultSetting();
					productDefaultSetting.LoadSetting(shopId);

					var iconKeyValues = CAMPAIN_ICON_TAG_LIST.Select(iconTag => GetCampaignIcon(campaignIcon, productDefaultSetting, iconTag)).ToList();
					iconKeyValues.ForEach(iconKeyValue => repaceData.Add(iconKeyValue.Key, iconKeyValue.Value));

					// SEO表示用カラー
					repaceData.Add(Constants.SEOSETTING_KEY_PRODUCT_COLOR_KEYWORD, SeoMapping.GetValueText(SEO_MAPPING_TYPE_COLOR, color));

					// SEO表示用フリーワード
					repaceData.Add(Constants.SEOSETTING_KEY_FREE_WORD_KEYWORD, searchWord);

					// SEO表示用商品タグ
					repaceData.Add(Constants.SEOSETTING_KEY_PRODUCT_TAG, productTag);

					// SEO表示用価格
					var displayPriceMin = (string.IsNullOrEmpty(priceMin)) ? "" : CurrencyManager.ToPrice(priceMin);
					var displayPriceMax = (string.IsNullOrEmpty(priceMax)) ? "" : CurrencyManager.ToPrice(priceMax);
					var productPrice = (string.IsNullOrEmpty(priceMin) && string.IsNullOrEmpty(priceMax))
						? ""
						: string.Format("{0}～{1}", displayPriceMin, displayPriceMax);
					repaceData.Add(Constants.SEOSETTING_KEY_PRODUCT_PRICE_KEYWORD, productPrice);

					// 同一商品カテゴリー名情報取得
					var categoryFamily = GetProductCategoryFamily(shopId, categoryId);

					// 取得したカテゴリの子から上位X個取得する
					var categoryNames = categoryFamily.Cast<DataRowView>()
						.Select(drv => drv[Constants.FIELD_PRODUCTCATEGORY_NAME])
						.Take(Constants.SEOSETTING_CHILD_CATEGORY_TOP_COUNT);
					var categories = string.Join(",", categoryNames);
					repaceData.Add(Constants.SEOSETTING_KEY_CHILD_CATEGORY_TOP, categories);
				}
			}

			// ブランド情報取得
			var brandList = ProductBrandUtility.GetBrandDataFromCache(brandId);
			if (brandList.Count != 0)
			{
				// ブランド有効 && ブランドサイトアクセス時
				repaceData.Add(Constants.SEOSETTING_KEY_BRAND_TITLE, brandList[0][Constants.FIELD_PRODUCTBRAND_BRAND_TITLE]);
				repaceData.Add(Constants.SEOSETTING_KEY_BRAND_SEO_KEYWORD, brandList[0][Constants.FIELD_PRODUCTBRAND_SEO_KEYWORD]);
			}
			else
			{
				// ブランド有効 && デフォルトブランド設定なし時
				// ブランド無効（タグマニュアル非表示となるので起こらないはず）
				repaceData.Add(Constants.SEOSETTING_KEY_BRAND_TITLE, "");
				repaceData.Add(Constants.SEOSETTING_KEY_BRAND_SEO_KEYWORD, "");
			}
			return repaceData;
		}

		/// <summary>
		/// 指定のキャンペーンアイコンNoの情報を取得する
		/// </summary>
		/// <param name="campaignIcon">キャンペーンアイコン</param>
		/// <param name="productDefaultSetting">商品初期設定</param>
		/// <param name="iconTag">アイコンタグ</param>
		/// <returns>アイコンNoの情報</returns>
		private static KeyValuePair<string, string> GetCampaignIcon(
			string campaignIcon,
			ProductDefaultSetting.ProductDefaultSetting productDefaultSetting,
			string iconTag)
		{
			// 指定のキャンペーンアイコンNoの情報を取得する なければ 空文字を設定する
			int iconNo;
			var productIcon = (int.TryParse(campaignIcon, out iconNo)) ? string.Format("{0}{1}", ICON_FLG, iconNo) : "";
			var iconIndex = Regex.Replace(iconTag, @"[^0-9]", "");

			if (string.IsNullOrEmpty(productIcon) || (iconIndex != iconNo.ToString()))
			{
				return new KeyValuePair<string, string>(
					iconTag,
					"");
			}
			else
			{
				return new KeyValuePair<string, string>(
					iconTag,
					productDefaultSetting.Product.GetComment(productIcon));
			}
		}

		// HACK：本来はここに記述したくない
		/// <summary>
		/// 商品カテゴリデータ取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="categoryId">カテゴリーID</param>
		/// <returns>商品カテゴリ情報</returns>
		private static DataView GetCategoryData(string shopId, string categoryId)
		{
			using (var accessor = new SqlAccessor())
			using (var statement = new SqlStatement("ProductSeoSetting", "GetProductCategory"))
			{
				var ht = new Hashtable
				{
					{Constants.FIELD_PRODUCTCATEGORY_SHOP_ID, shopId},
					{Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID, categoryId}
				};
				var dv = statement.SelectSingleStatementWithOC(accessor, ht);
				return dv;
			}
		}

		/// <summary>
		/// 商品カテゴリデータビュー取得(同一カテゴリー以下)
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="categoryId">カテゴリーID</param>
		/// <returns>商品カテゴリ情報</returns>
		private static DataView GetProductCategoryFamily(string shopId, string categoryId)
		{
			using (var accessor = new SqlAccessor())
			using (var statement = new SqlStatement("ProductSeoSetting", "GetProductCategoryFamily"))
			{
				var ht = new Hashtable
				{
					{Constants.FIELD_PRODUCTCATEGORY_SHOP_ID, shopId},
					{Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID, categoryId}
				};
				var dv = statement.SelectSingleStatementWithOC(accessor, ht);
				return dv;
			}
		}

		/// <summary>
		/// 商品グループ名取得
		/// </summary>
		/// <param name="productGroupId">商品グループID</param>
		/// <returns>商品グループ名</returns>
		private static string GetProductGroupName(string productGroupId)
		{
			// 商品グループを取得
			var ApplicableProductGroups = DataCacheControllerFacade.GetProductGroupCacheController().GetApplicableProductGroup();
			var productGroup =
				ApplicableProductGroups
					.Where(grp => grp.ProductGroupId == productGroupId)
					.FirstOrDefault();

			return (productGroup == null) ? null : productGroup.ProductGroupName;
		}

		/// <summary>
		/// SEOメタデータ（モデルを更新したくないのでこちらでラップする）
		/// </summary>
		public class SeoMetadatas : SeoMetadatasModel
		{
			#region コンストラクタ
			/// <summary>
			/// デフォルトコンストラクタ
			/// </summary>
			public SeoMetadatas()
			{
				this.DelFlg = "0";
			}

			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="source">ソース</param>
			public SeoMetadatas(DataRowView source)
				: this(source.ToHashtable())
			{
			}

			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="source">ソース</param>
			public SeoMetadatas(Hashtable source)
				: this()
			{
				this.DataSource = source;
			}
			#endregion
		}

		/// <summary>タグの置換結果がすべて空文字か（タイトル）</summary>
		protected static bool IsAllTagConvertResultEmptyForTitle
		{
			get { return (TagConvertResultEmptyFlgsForTitle.All(flg => (flg == 1))); }
		}
		/// <summary>タグの置換結果がすべて空文字か（キーワード）</summary>
		protected static bool IsAllTagConvertResultEmptyForKeyWord
		{
			get { return (TagConvertResultEmptyFlgsForKeyWord.All(flg => (flg == 1))); }
		}
		/// <summary>タグの置換結果がすべて空文字か（概要）</summary>
		protected static bool IsAllTagConvertResultEmptyForDescription
		{
			get { return (TagConvertResultEmptyFlgsForDescription.All(flg => (flg == 1))); }
		}
		/// <summary>タグの置換結果がすべて空文字か（コメント）</summary>
		protected static bool IsAllTagConvertResultEmptyForComment
		{
			get { return (TagConvertResultEmptyFlgsForComment.All(flg => (flg == 1))); }
		}
		/// <summary>タグの置換結果がすべて空文字か（Seoテキスト）</summary>
		protected static bool IsAllTagConvertResultEmptyForSeoText
		{
			get { return (TagConvertResultEmptyFlgsForSeoText.All(flg => (flg == 1))); }
		}
		/// <summary>置換結果空文字フラグリスト（タイトル）</summary>
		protected static List<int> TagConvertResultEmptyFlgsForTitle { get; set; }
		/// <summary>置換結果空文字フラグリスト（キーワード）</summary>
		protected static List<int> TagConvertResultEmptyFlgsForKeyWord { get; set; }
		/// <summary>置換結果空文字フラグリスト（概要）</summary>
		protected static List<int> TagConvertResultEmptyFlgsForDescription { get; set; }
		/// <summary>置換結果空文字フラグリスト（コメント）</summary>
		protected static List<int> TagConvertResultEmptyFlgsForComment { get; set; }
		/// <summary>置換結果空文字フラグリスト（Seoテキスト）</summary>
		protected static List<int> TagConvertResultEmptyFlgsForSeoText { get; set; }
	}
}
