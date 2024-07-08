/*
=========================================================================================================
  Module      : 商品系基底ページ(ProductPage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using ProductListDispSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common.DataCacheController;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Global.Translation;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.Product;
using w2.App.Common.UserProductArrivalMail;
using w2.App.Common.Web.Page;
using w2.App.Common.Web.Process;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Extensions;
using w2.Common.Util.Security;
using w2.Common.Web;
using w2.Domain.ContentsLog;
using w2.Domain.Favorite;
using w2.Domain.FixedPurchase.Helper;
using w2.Domain.NameTranslationSetting;
using w2.Domain.NameTranslationSetting.Helper;
using w2.Domain.Order;
using w2.Domain.Product;
using w2.Domain.ProductTaxCategory;
using w2.Domain.SetPromotion;
using w2.Domain.SubscriptionBox;

///*********************************************************************************************
/// <summary>
/// 商品系基底ページ
/// </summary>
///*********************************************************************************************
public class ProductPage : BasePage
{
	/// <summary>バリエーション未選択によるエラーメッセージ</summary>
	protected string MESSAGE_ERROR_VARIATION_UNSELECTED = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_VARIATION_UNSELECTED);
	/// <summary>オプション未選択によるエラーメッセージ</summary>
	protected string MESSAGE_ERROR_OPTION_UNSELECTED = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_OPTION_UNSELECTED);
	/// <summary>JAFログイン連携：ログイン必須エラー</summary>
	protected string MESSAGE_ERROR_JAF_NEEDS_LOGIN_ERROR = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_JAF_NEEDS_LOGIN_ERROR);
	/// <summary>JAFログイン連携：会員登録時の詳細説明文・注意事項</summary>
	protected string MESSAGE_ERROR_JAF_REGISTER_DESCRIPTION = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_JAF_REGISTER_DESCRIPTION);
	/// <summary>ローディングアニメーションタイプ：上部</summary>
	protected const string LOADING_TYPE_UPPER = "Upper";
	/// <summary>ローディングアニメーションタイプ：下部</summary>
	protected const string LOADING_TYPE_LOWER = "Lower";

	/// <summary>
	/// ページ初期化
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected new void Page_Init(object sender, EventArgs e)
	{
		this.Process.Page_Init(sender, e);
	}

	/// <summary>
	/// 商品画面系パラメタ取得
	/// </summary>
	protected void GetParameters()
	{
		Dictionary<string, object> dicParams = GetParameters(Request);

		this.ShopId = (string)dicParams[Constants.REQUEST_KEY_SHOP_ID];
		this.CategoryId = (string)dicParams[Constants.REQUEST_KEY_CATEGORY_ID];
		this.ProductId = (string)dicParams[Constants.REQUEST_KEY_PRODUCT_ID];
		this.VariationId = (string)dicParams[Constants.REQUEST_KEY_VARIATION_ID];
		this.SearchWord = (string)dicParams[Constants.REQUEST_KEY_SEARCH_WORD];
		this.SortKbn = (string)dicParams[Constants.REQUEST_KEY_SORT_KBN];
		this.DispImageKbn = (string)dicParams[Constants.REQUEST_KEY_DISP_IMG_KBN];
		this.PageNumber = (int)dicParams[Constants.REQUEST_KEY_PAGE_NO];
		this.CampaignIcon = (string)dicParams[Constants.REQUEST_KEY_CAMPAINGN_ICOM];
		this.MinPrice = (string)dicParams[Constants.REQUEST_KEY_MIN_PRICE];
		this.MaxPrice = (string)dicParams[Constants.REQUEST_KEY_MAX_PRICE];
		this.Color = (string)dicParams[Constants.REQUEST_KEY_PRODUCT_COLOR_ID];
		this.ProductGroupId = (string)dicParams[Constants.REQUEST_KEY_PRODUCT_GROUP_ID];
		this.SubscriptionBoxSearchWord = (string)dicParams[Constants.REQUEST_KEY_SUBSCRIPTION_BOX_SEARCH_WORD];
		if ((int)dicParams[Constants.REQUEST_KEY_DISP_PRODUCT_COUNT] == -1)
		{
			this.DisplayCount = this.IsDispImageKbnOn ? ProductListDispSettingUtility.CountDispContentsImgOn : ProductListDispSettingUtility.CountDispContentsWindowShopping;
		}
		else
		{
			this.DisplayCount = (int)dicParams[Constants.REQUEST_KEY_DISP_PRODUCT_COUNT];
		}
		this.UndisplayNostock = (string)dicParams[Constants.REQUEST_KEY_UNDISPLAY_NOSTOCK_PRODUCT];
		this.FixedPurchaseFilter = (string)dicParams[Constants.REQUEST_KEY_FIXED_PURCHASE_FILTER];
		this.SaleFilter = (string)dicParams[Constants.REQUEST_KEY_PRODUCT_SALE_FILTER];
		this.DisplayOnlySpPrice = (string)dicParams[Constants.REQUEST_KEY_DISP_ONLY_SP_PRICE];
		this.ProductColorId = (string)dicParams[Constants.REQUEST_KEY_PRODUCT_COLOR_ID];
		this.RequestParameter = new Dictionary<string, string>();
		foreach (string requestKey in dicParams.Keys)
		{
			this.RequestParameter.Add(requestKey, dicParams[requestKey].ToString());
		}

		this.Process.SetBrandInfo();

		// パラメーター削除（ブランドID、ブランド名）
		this.RequestParameter.Remove(Constants.REQUEST_KEY_BRAND_ID);
		this.RequestParameter.Remove(ProductCommon.URL_KEY_BRAND_NAME);
		// パラメーターセット（ブランドID、ブランド名）
		this.RequestParameter.Add(Constants.REQUEST_KEY_BRAND_ID, this.BrandId);
		this.RequestParameter.Add(ProductCommon.URL_KEY_BRAND_NAME, this.BrandName);
	}

	/// <summary>
	/// 商品画面系パラメタ取得
	/// </summary>
	/// <param name="hrRequest">商品一覧・詳細のパラメタが格納されたHttpRequest</param>
	/// <returns>パラメタが格納されたDictionary</returns>
	public static Dictionary<string, object> GetParameters(HttpRequest hrRequest)
	{
		Dictionary<string, object> dicParam = new Dictionary<string, object>();

		// 店舗ID
		string strShopId = StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_SHOP_ID]);
		if (strShopId == "")
		{
			strShopId = Constants.CONST_DEFAULT_SHOP_ID;
		}
		dicParam.Add(Constants.REQUEST_KEY_SHOP_ID, strShopId);

		// 商品ID、バリエーションID
		dicParam.Add(Constants.REQUEST_KEY_PRODUCT_ID, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_PRODUCT_ID]));
		dicParam.Add(Constants.REQUEST_KEY_VARIATION_ID, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_VARIATION_ID]));

		// ページ番号（ページャ動作時のみもちまわる）
		int iPageNumber;
		if (int.TryParse((string)hrRequest[Constants.REQUEST_KEY_PAGE_NO], out iPageNumber) == false)
		{
			iPageNumber = 1;
		}
		dicParam.Add(Constants.REQUEST_KEY_PAGE_NO, iPageNumber);

		// カテゴリID
		dicParam.Add(Constants.REQUEST_KEY_CATEGORY_ID, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_CATEGORY_ID]));

		// 検索ワード
		dicParam.Add(
			Constants.REQUEST_KEY_SEARCH_WORD,
			StringUtility.StrTrim(StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_SEARCH_WORD]).Trim(), Constants.CONST_PRODUCT_SEARCH_WORD_MAX_LENGTH));

		// 頒布会検索ワード
		dicParam.Add(
			Constants.REQUEST_KEY_SUBSCRIPTION_BOX_SEARCH_WORD,
			StringUtility.StrTrim(
				StringUtility.ToEmpty(
					hrRequest[Constants.REQUEST_KEY_SUBSCRIPTION_BOX_SEARCH_WORD]).Trim(),
				Constants.CONST_PRODUCT_SEARCH_WORD_MAX_LENGTH));

		// カラー
		dicParam.Add(Constants.REQUEST_KEY_PRODUCT_COLOR_ID, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_PRODUCT_COLOR_ID]).Trim());

		// イメージ表示
		string strDispImageKbn = hrRequest[Constants.REQUEST_KEY_DISP_IMG_KBN];
		switch (strDispImageKbn)
		{
			case Constants.KBN_REQUEST_DISP_IMG_KBN_ON:
			case Constants.KBN_REQUEST_DIST_IMG_KBN_WINDOWSHOPPING:
				strDispImageKbn = GetProductListDispSettingIdDefaultOrUsable(
					strDispImageKbn,
					Constants.FLG_PRODUCTLISTDISPSETTING_SETTING_KBN_IMG);
				break;

			default:
				strDispImageKbn = ProductListDispSettingUtility.DispImgKbnDefault;	// デフォルトイメージ表示
				break;
		}
		dicParam.Add(Constants.REQUEST_KEY_DISP_IMG_KBN, strDispImageKbn);

		// ソート順
		string strSortKbn = hrRequest[Constants.REQUEST_KEY_SORT_KBN];
		switch (strSortKbn)
		{
			case Constants.KBN_SORT_PRODUCT_LIST_NAME_KANA_ASC:
			case Constants.KBN_SORT_PRODUCT_LIST_PRICE_ASC:
			case Constants.KBN_SORT_PRODUCT_LIST_PRICE_DESC:
			case Constants.KBN_SORT_PRODUCT_LIST_NEW_ASC:
			case Constants.KBN_SORT_PRODUCT_LIST_DATE_CREATED_DESC:
			case Constants.KBN_SORT_PRODUCT_LIST_FAVORITE_CNT_DESC:
				strSortKbn = GetProductListDispSettingIdDefaultOrUsable(
					strSortKbn,
					Constants.FLG_PRODUCTLISTDISPSETTING_SETTING_KBN_SORT);
				break;

			default:
				// 商品グループIDが指定されている場合は、商品グループの表示順を優先する
				strSortKbn = (string.IsNullOrEmpty(hrRequest[Constants.REQUEST_KEY_PRODUCT_GROUP_ID]) == false)
					? Constants.KBN_SORT_PRODUCT_LIST_PRODUCT_GROUP_ITEM_NO_ASC
					: ProductListDispSettingUtility.SortDefault;
				break;
		}
		dicParam.Add(Constants.REQUEST_KEY_SORT_KBN, strSortKbn);

		// キャンペーンアイコン
		string strCampaignIcon = StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_CAMPAINGN_ICOM]).Trim();
		switch (strCampaignIcon)
		{
			case Constants.FLG_PRODUCT_ICON_1:
			case Constants.FLG_PRODUCT_ICON_2:
			case Constants.FLG_PRODUCT_ICON_3:
			case Constants.FLG_PRODUCT_ICON_4:
			case Constants.FLG_PRODUCT_ICON_5:
			case Constants.FLG_PRODUCT_ICON_6:
			case Constants.FLG_PRODUCT_ICON_7:
			case Constants.FLG_PRODUCT_ICON_8:
			case Constants.FLG_PRODUCT_ICON_9:
			case Constants.FLG_PRODUCT_ICON_10:
				break;

			default:
				strCampaignIcon = "";
				break;
		}
		dicParam.Add(Constants.REQUEST_KEY_CAMPAINGN_ICOM, strCampaignIcon);

		// 最小価格
		string strMinPrice = StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_MIN_PRICE]).Trim();
		decimal dMinPrice;
		if (decimal.TryParse(strMinPrice, out dMinPrice))
		{
			dicParam.Add(Constants.REQUEST_KEY_MIN_PRICE, strMinPrice);
		}
		else
		{
			dicParam.Add(Constants.REQUEST_KEY_MIN_PRICE, "");
		}

		// 最大価格
		string strMaxPrice = StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_MAX_PRICE]).Trim();
		decimal dMaxPrice;
		if (decimal.TryParse(strMaxPrice, out dMaxPrice))
		{
			dicParam.Add(Constants.REQUEST_KEY_MAX_PRICE, strMaxPrice);
		}
		else
		{
			dicParam.Add(Constants.REQUEST_KEY_MAX_PRICE, "");
		}

		// 表示件数
		int iDisplayCount;
		if (int.TryParse(StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_DISP_PRODUCT_COUNT]).Trim(), out iDisplayCount))
		{
			// 設定値外の表示件数が指定された場合は、未指定とする。
			foreach (int iDisplayCountForCheck in ProductListDispSettingUtility.GetCountSetting(ProductListDispSettingUtility.CountSetting))
			{
				if (iDisplayCountForCheck == iDisplayCount)
				{
					dicParam.Add(Constants.REQUEST_KEY_DISP_PRODUCT_COUNT, iDisplayCount);
				}
			}
			if (dicParam.ContainsKey(Constants.REQUEST_KEY_DISP_PRODUCT_COUNT) == false)
			{
				dicParam.Add(Constants.REQUEST_KEY_DISP_PRODUCT_COUNT, -1);
			}
		}
		else
		{
			dicParam.Add(Constants.REQUEST_KEY_DISP_PRODUCT_COUNT, -1);
		}

		// 在庫有無検索
		string strStockExistenceSearch = StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_UNDISPLAY_NOSTOCK_PRODUCT]).Trim();
		switch (strStockExistenceSearch)
		{
			case Constants.KBN_PRODUCT_LIST_UNDISPLAY_NOSTOCK_PRODUCT_DISPLAY_NOSTOCK:
			case Constants.KBN_PRODUCT_LIST_UNDISPLAY_NOSTOCK_PRODUCT_UNDISPLAY_NOSTOCK:
			case Constants.KBN_PRODUCT_LIST_UNDISPLAY_NOSTOCK_PRODUCT_DISPLAY_NOSTOCK_BOTTOM:
			case Constants.KBN_PRODUCT_LIST_UNDISPLAY_NOSTOCK_PRODUCT_DISPLAY_NOSTOCK_ONLY:
				strStockExistenceSearch = GetProductListDispSettingIdDefaultOrUsable(
					strStockExistenceSearch,
					Constants.FLG_PRODUCTLISTDISPSETTING_SETTING_KBN_STOCK);
				break;

			default:
				strStockExistenceSearch = ProductListDispSettingUtility.UndisplayNostockProductDefault;	// デフォルト
				break;
		}
		dicParam.Add(Constants.REQUEST_KEY_UNDISPLAY_NOSTOCK_PRODUCT, strStockExistenceSearch);

		// 特別価格有無検索
		dicParam.Add(
			Constants.REQUEST_KEY_DISP_ONLY_SP_PRICE,
			StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_DISP_ONLY_SP_PRICE]).Trim());

		// 定期購入フィルタ
		string fixedPurchaseFilter = StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_FIXED_PURCHASE_FILTER]).Trim();
		switch (fixedPurchaseFilter)
		{
			case Constants.KBN_PRODUCT_LIST_FIXED_PURCHASE_FILTER_ALL:
			case Constants.KBN_PRODUCT_LIST_FIXED_PURCHASE_FILTER_NORMAL:
			case Constants.KBN_PRODUCT_LIST_FIXED_PURCHASE_FILTER_FIXED_PURCHASE:
				break;

			default:
				fixedPurchaseFilter = Constants.KBN_PRODUCT_LIST_FIXED_PURCHASE_FILTER_ALL;
				break;
		}
		dicParam.Add(Constants.REQUEST_KEY_FIXED_PURCHASE_FILTER, fixedPurchaseFilter);

		// セール対象フィルタ
		var saleFilter = CovertToQueryStringsForSaleFilter(StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_PRODUCT_SALE_FILTER]));
		dicParam.Add(Constants.REQUEST_KEY_PRODUCT_SALE_FILTER, saleFilter);

		// 除外するリクエストキー
		List<string> excludeKeys = new List<string>() { Constants.REQUEST_KEY_SHOP_ID,
							   Constants.REQUEST_KEY_PRODUCT_ID,
							   Constants.REQUEST_KEY_VARIATION_ID,
							   Constants.REQUEST_KEY_PAGE_NO,
							   Constants.REQUEST_KEY_CATEGORY_ID,
							   Constants.REQUEST_KEY_SEARCH_WORD,
							   Constants.REQUEST_KEY_DISP_IMG_KBN,
							   Constants.REQUEST_KEY_SORT_KBN,
							   Constants.REQUEST_KEY_CAMPAINGN_ICOM,
							   Constants.REQUEST_KEY_MIN_PRICE,
							   Constants.REQUEST_KEY_MAX_PRICE,
							   Constants.REQUEST_KEY_PRODUCT_COLOR_ID,
							   Constants.REQUEST_KEY_DISP_PRODUCT_COUNT,
							   Constants.REQUEST_KEY_UNDISPLAY_NOSTOCK_PRODUCT,
							   Constants.REQUEST_KEY_DISP_ONLY_SP_PRICE,
							   Constants.REQUEST_KEY_FIXED_PURCHASE_FILTER,
							   Constants.REQUEST_KEY_PRODUCT_SALE_FILTER,
							   Constants.REQUEST_KEY_SUBSCRIPTION_BOX_SEARCH_WORD,

		};
		// 残りのクエリストリングを格納
		foreach (string requestKey in hrRequest.QueryString.Keys)
		{
			if (excludeKeys.Contains(requestKey))
			{
				continue;
			}
			if (StringUtility.ToEmpty(requestKey) != "")
			{
				dicParam.Add(requestKey, hrRequest[requestKey]);
			}
		}
		// 商品グループID
		if (dicParam.ContainsKey(Constants.REQUEST_KEY_PRODUCT_GROUP_ID) == false)
		{
			dicParam.Add(Constants.REQUEST_KEY_PRODUCT_GROUP_ID, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_PRODUCT_GROUP_ID]));
		}

		// 商品カラーID
		if (dicParam.ContainsKey(Constants.REQUEST_KEY_PRODUCT_COLOR_ID) == false)
		{
			dicParam.Add(Constants.REQUEST_KEY_PRODUCT_COLOR_ID, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_PRODUCT_COLOR_ID]));
		}

		return dicParam;
	}

	/// <summary>
	/// ハッシュテーブルに入っているセールフィルター用の値を適切なクエリストリングに変換する
	/// </summary>
	/// <param name="saleFilterParameter">セールフィルター用の値</param>
	/// <returns>クエリストリングに利用できるセールフィルター用の値</returns>
	private static string CovertToQueryStringsForSaleFilter(string saleFilterParameter)
	{
		var saleFilter = (saleFilterParameter != null) ? saleFilterParameter.Trim() : string.Empty;
		switch (saleFilter)
		{
			case Constants.KBN_PRODUCT_LIST_SALE_ALL:
			case Constants.KBN_PRODUCT_LIST_SALE_ONLY:
				break;

			default:
				saleFilter = Constants.KBN_PRODUCT_LIST_SALE_ALL;
				break;
		}
		return saleFilter;
	}

	/// <summary>
	/// 商品一覧表示で利用するデフォルトソート区分を読み込む
	/// </summary>
	/// <returns>デフォルトソート区分</returns>
	private string GetProductDefaultSort()
	{
		string defaultSortKbn = "";
		foreach (var model in DataCacheControllerFacade.GetProductListDispSettingCacheController().CacheData)
		{
			// デフォルトソート区分を取得
			if (model.IsDefaultDispFlgOn)
			{
				defaultSortKbn = model.SettingId;
				break;
			}
		}

		return defaultSortKbn;
	}

	/// <summary>
	/// 商品データ取得（表示条件考慮しない）
	/// </summary>
	/// <param name="strShopId">店舗ID</param>
	/// <param name="strProductId">商品ID</param>
	/// <param name="strVariationId">バリエーションID(なしの場合、商品ID)</param>
	/// <returns>商品データ</returns>
	public DataView GetProduct(string strShopId, string strProductId, string strVariationId)
	{
		return this.Process.GetProduct(strShopId, strProductId, strVariationId);
	}

	/// <summary>
	/// 定期購入初回価格有効状態取得
	/// </summary>
	/// <param name="product">商品情報</param>
	/// <param name="targetVariation">バリエーション対象</param>
	/// <returns>有効状態</returns>
	public static bool GetProductFixedPurchaseFirsttimePriceValid(object product, bool targetVariation = false)
	{
		return (ProductPrice.GetFixedPurchaseProductPriceType(product, targetVariation) == ProductPrice.PriceTypes.FixedPurchaseFirsttimePrice);
	}

	/// <summary>
	/// 定期購入初回価格有効状態取得
	/// </summary>
	/// <param name="product">商品情報</param>
	/// <param name="targetVariation">バリエーション対象</param>
	/// <returns>有効状態</returns>
	public static bool IsProductFixedPurchaseFirsttimePriceValid(object product, bool targetVariation = false)
	{
		if ((string)GetKeyValue(product, Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG) == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID) return false;

		var fixedPurchaseFirsttimePrice = StringUtility.ToEmpty(GetKeyValue(product, targetVariation
			? Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE : Constants.FIELD_PRODUCT_FIXED_PURCHASE_FIRSTTIME_PRICE));

		return (fixedPurchaseFirsttimePrice != String.Empty);
	}

	/// <summary>
	/// 商品会員ランク価格有効状態取得
	/// </summary>
	/// <param name="objProduct">商品情報</param>
	/// <param name="targetVariation">バリエーション対象</param>
	/// <returns>有効状態</returns>
	public static bool GetProductMemberRankPriceValid(object objProduct, bool targetVariation = false)
	{
		return (ProductPrice.GetProductPriceType(objProduct, targetVariation) == ProductPrice.PriceTypes.MemberRankPrice);
	}

	/// <summary>
	/// 商品セール有効状態取得
	/// </summary>
	/// <param name="objProduct">商品情報</param>
	/// <param name="targetVariation"></param>
	/// <returns>有効状態</returns>
	/// <remarks>
	/// こちらはSQL発行時に商品マスタかバリエーションかの判定を行っているので
	/// バリエーションかどうかの判定を行う必要は無い。
	/// </remarks>
	public static bool GetProductTimeSalesValid(object objProduct, bool targetVariation = false)
	{
		return (ProductPrice.GetProductPriceType(objProduct, targetVariation) == ProductPrice.PriceTypes.TimeSale);
	}

	/// <summary>
	/// 闇市有効状態取得
	/// </summary>
	/// <param name="objProduct">商品情報</param>
	/// <param name="targetVariation">バリエーション対象</param>
	/// <returns>有効状態</returns>
	/// <remarks>
	/// こちらはSQL発行時に商品マスタかバリエーションかの判定を行っているので
	/// バリエーションかどうかの判定を行う必要は無い。
	/// </remarks>
	public static bool GetProductClosedMarketPriceValid(object objProduct, bool targetVariation = false)
	{
		return ProductPrice.GetProductPriceType(objProduct, targetVariation) == ProductPrice.PriceTypes.ClosedMarketPrice;
	}

	/// <summary>
	/// 商品特別価格有効状態取得
	/// </summary>
	/// <param name="objProduct">商品情報</param>
	/// <param name="targetVariation">バリエーション対象</param>
	/// <returns>有効状態</returns>
	public static bool GetProductSpecialPriceValid(object objProduct, bool targetVariation = false)
	{
		return (ProductPrice.GetProductPriceType(objProduct, targetVariation) == ProductPrice.PriceTypes.Special);
	}

	/// <summary>
	/// 商品通常価格有効状態取得
	/// </summary>
	/// <param name="objProduct">商品情報</param>
	/// <param name="targetVariation">バリエーション対象</param>
	/// <returns>有効状態</returns>
	public static bool GetProductNormalPriceValid(object objProduct, bool targetVariation = false)
	{
		return (ProductPrice.GetProductPriceType(objProduct, targetVariation) == ProductPrice.PriceTypes.Normal);
	}

	/// <summary>
	/// 商品価格数値取得
	/// </summary>
	/// <param name="objProduct">商品情報</param>
	/// <param name="blTargetVariation">バリエーション対象</param>
	/// <returns>商品価格</returns>
	public static string GetProductPriceNumeric(object objProduct, bool blTargetVariation = false)
	{
		return ProductCommon.GetProductPriceNumeric(objProduct, blTargetVariation);
	}

	/// <summary>
	/// 商品「特別価格」数値取得
	/// </summary>
	/// <param name="objProduct">商品情報</param>
	/// <param name="blTargetVariation">バリエーション対象</param>
	/// <returns>特別価格</returns>
	public static string GetProductSpecialPriceNumeric(object objProduct, bool blTargetVariation = false)
	{
		if (blTargetVariation == false)
		{
			return StringUtility.ToNumeric(StringUtility.ToEmpty(GetKeyValue(objProduct, Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE)).ToPriceString());
		}
		else
		{
			return StringUtility.ToNumeric(StringUtility.ToEmpty(GetKeyValue(objProduct, Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE)).ToPriceString());
		}
	}

	/// <summary>
	/// 商品「会員ランク価格」数値取得（バリエーション共用）
	/// </summary>
	/// <param name="objProduct">商品情報</param>
	/// <param name="isVariation">バリエーション対象かどうか</param>
	/// <returns>会員ランク価格</returns>
	public static string GetProductMemberRankPrice(object objProduct, bool isVariation = false)
	{
		if (isVariation == false)
		{
			return StringUtility.ToNumeric(StringUtility.ToEmpty(GetKeyValue(objProduct, Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE)).ToPriceString());
		}
		else
		{
			return StringUtility.ToNumeric(StringUtility.ToEmpty(GetKeyValue(objProduct, Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE_VARIATION)).ToPriceString());
		}
	}

	/// <summary>
	/// 商品「タイムセール価格」数値取得（バリエーション共用）
	/// </summary>
	/// <param name="objProduct">商品情報</param>
	/// <returns>タイムセールス価格</returns>
	public static string GetProductTimeSalePriceNumeric(object objProduct)
	{
		return StringUtility.ToNumeric(StringUtility.ToEmpty(GetKeyValue(objProduct, Constants.FIELD_PRODUCTSALEPRICE_SALE_PRICE)).ToPriceString());
	}

	/// <summary>
	/// 商品「闇市価格」数値取得（バリエーション共用）
	/// </summary>
	/// <param name="objProduct">商品情報</param>
	/// <returns>闇市価格</returns>
	public static string GetProductClosedMarketPriceNumeric(object objProduct)
	{
		return StringUtility.ToNumeric(StringUtility.ToEmpty(GetKeyValue(objProduct, "real_sale_price")).ToPriceString());
	}

	/// <summary>
	/// 商品「定期購入初回価格」数値取得（バリエーション共用）
	/// </summary>
	/// <param name="product">商品情報</param>
	/// <param name="isVariation">バリエーション対象</param>
	/// <returns>定期初回購入価格価格</returns>
	public static string GetProductFixedPurchaseFirsttimePrice(object product, bool isVariation = false)
	{
		var valueName = isVariation
			? Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE
			: Constants.FIELD_PRODUCT_FIXED_PURCHASE_FIRSTTIME_PRICE;

		return StringUtility.ToNumeric(StringUtility.ToEmpty(GetKeyValue(product, valueName)).ToPriceString());
	}

	/// <summary>
	/// 商品「定期購入通常価格」数値取得（バリエーション共用）
	/// </summary>
	/// <param name="product">商品情報</param>
	/// <param name="targetVariation">バリエーション対象</param>
	/// <returns>定期購入通常価格</returns>
	public static string GetProductFixedPurchasePrice(object product, bool targetVariation = false)
	{
		var fixedPurchasePrice = StringUtility.ToNumeric(ProductPrice.GetFixedPurchasePrice(product, targetVariation).ToPriceString());
		return fixedPurchasePrice;
	}

	/// <summary>
	/// バリエーションがあるか
	/// </summary>
	/// <param name="objProduct">商品情報</param>
	/// <returns>バリエーションがあるか</returns>
	protected bool HasVariation(object objProduct)
	{
		return ProductCommon.HasVariation(objProduct);
	}

	/// <summary>
	/// 税込表示取得
	/// </summary>
	/// <param name="objProduct">商品情報</param>
	/// <returns>税込み/税抜き表示</returns>
	public static string GetTaxIncludeString(object objProduct)
	{
		return ValueText.GetValueText(
			Constants.TABLE_PRODUCT,
			Constants.FIELD_PRODUCT_TAX_INCLUDED_FLG,
			TaxCalculationUtility.GetPrescribedOrderItemTaxIncludedFlag());
	}

	/// <summary>
	/// 商品データ取得
	/// </summary>
	/// <param name="key">商品情報</param>
	/// <param name="key">キー（フィールド）</param>
	/// <returns>商品データ</returns>
	protected object GetProductData(object product, string key)
	{
		switch (key)
		{
			case Constants.FIELD_PRODUCTVARIATION_VARIATION_ID:
				return (this.VariationId != "") ? GetKeyValue(product, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID) : GetKeyValue(product, Constants.FIELD_PRODUCT_PRODUCT_ID);
		}

		return GetKeyValue(product, key);
	}

	/// <summary>
	/// 商品説明取得(Text,Html判定）
	/// </summary>
	/// <param name="objProduct">商品情報</param>
	/// <param name="strDescField">フィールド名</param>
	/// <returns>商品説明</returns>
	protected string GetProductDataHtml(object objProduct, string strDescField)
	{
		string strKbnField = null;
		switch (strDescField)
		{
			case Constants.FIELD_PRODUCT_OUTLINE:
				strKbnField = Constants.FIELD_PRODUCT_OUTLINE_KBN;
				break;
			case Constants.FIELD_PRODUCT_DESC_DETAIL1:
				strKbnField = Constants.FIELD_PRODUCT_DESC_DETAIL_KBN1;
				break;
			case Constants.FIELD_PRODUCT_DESC_DETAIL2:
				strKbnField = Constants.FIELD_PRODUCT_DESC_DETAIL_KBN2;
				break;
			case Constants.FIELD_PRODUCT_DESC_DETAIL3:
				strKbnField = Constants.FIELD_PRODUCT_DESC_DETAIL_KBN3;
				break;
			case Constants.FIELD_PRODUCT_DESC_DETAIL4:
				strKbnField = Constants.FIELD_PRODUCT_DESC_DETAIL_KBN4;
				break;
		}

		return GetProductDescHtml(
			(string)GetKeyValue(objProduct, strKbnField),
			(string)GetKeyValue(objProduct, strDescField));
	}
	/// <summary>
	/// 商品説明取得(Text,Html判定）
	/// </summary>
	/// <param name="kbn">HTML区分（0:TEXT、1:HTML）</param>
	/// <param name="message">表示メッセージデータ</param>
	/// <returns>商品説明</returns>
	public static string GetProductDescHtml(object kbn, object message)
	{
		var messateString = StringUtility.ToEmpty(message);
		switch (StringUtility.ToEmpty(kbn))
		{
			case Constants.FLG_PRODUCT_DESC_DETAIL_HTML:
				// 相対パスを絶対パスに置換(aタグ、imgタグのみ）
				MatchCollection relativePath = Regex.Matches(messateString, "(a[\\s]+href=|img[\\s]+src=)([\"|']([^/|#][^\\\"':]+)[\"|'])", RegexOptions.IgnoreCase);
				foreach (Match match in relativePath)
				{
					Uri resourceUri = new Uri(HttpContext.Current.Request.Url, match.Groups[3].ToString());
					messateString = messateString.Replace(match.Groups[2].ToString(), "\"" + resourceUri.PathAndQuery + resourceUri.Fragment + "\"");
				}
				return messateString;

			case Constants.FLG_PRODUCT_DESC_DETAIL_TEXT:
				return WebSanitizer.HtmlEncodeChangeToBr(messateString);

			default:
				throw new ArgumentException("パラメタエラー: strKbn is [" + StringUtility.ToEmpty(kbn) + "]");
		}
	}

	/// <summary>
	/// 商品付与ポイント取得
	/// </summary>
	/// <param name="objProduct">商品情報</param>
	/// <param name="hasVariation">Has variation</param>
	/// <param name="isVariationSelected">Is variation selected</param>
	/// <param name="isFixedPurchase">定期購入かどうか</param>
	/// <returns>商品加算ポイント</returns>
	/// <remarks>商品が持つポイントを利用するときのみ表示</remarks>
	protected string GetProductAddPointString(object objProduct, bool hasVariation = false, bool isVariationSelected = false, bool isFixedPurchase = false)
	{
		var memberRankPointExcludeFlg = (string)GetKeyValue(objProduct, Constants.FIELD_PRODUCT_MEMBER_RANK_POINT_EXCLUDE_FLG) == Constants.FLG_PRODUCT_CHECK_MEMBER_RANK_POINT_EXCLUDE_FLG_VALID;
		//定期購入で、定期購入ポイントを設定してある場合
		if (isFixedPurchase
			&& (((string)GetKeyValue(objProduct, Constants.FIELD_PRODUCT_POINT_KBN2) == Constants.FLG_PRODUCT_POINT_KBN2_NUM)
				|| ((string)GetKeyValue(objProduct, Constants.FIELD_PRODUCT_POINT_KBN2) == Constants.FLG_PRODUCT_POINT_KBN2_RATE)))
		{
			return GetProductAddPointString(
				this.ShopId,
				StringUtility.ToEmpty(GetKeyValue(objProduct, Constants.FIELD_PRODUCT_POINT_KBN2)),
				(decimal)GetKeyValue(objProduct, Constants.FIELD_PRODUCT_POINT2),
				GetProductValidityPrice(objProduct, hasVariation, isVariationSelected),
				memberRankPointExcludeFlg);
		}
		//通常購入
		return GetProductAddPointString(
			this.ShopId,
			StringUtility.ToEmpty(GetKeyValue(objProduct, Constants.FIELD_PRODUCT_POINT_KBN1)),
			(decimal)GetKeyValue(objProduct, Constants.FIELD_PRODUCT_POINT1),
			GetProductValidityPrice(objProduct, hasVariation, isVariationSelected),
			memberRankPointExcludeFlg);
	}
	/// <summary>
	/// 商品付与ポイント取得
	/// </summary>
	/// <param name="strShopId">店舗ID</param>
	/// <param name="strPointKbn">付与ポイント区分</param>
	/// <param name="dProductPoint">商品ポイント</param>
	/// <param name="productPrice">Product price</param>
	/// <param name="memberRankPointExcludeFlg">会員ランクポイント除外フラグ</param>
	/// <returns>商品加算ポイント</returns>
	/// <remarks>商品が持つポイントを利用するときのみ表示</remarks>
	public string GetProductAddPointString(string strShopId, string strPointKbn, decimal dProductPoint, decimal productPrice, bool memberRankPointExcludeFlg = false)
	{
		return GetProductAddPointString(
			strShopId,
			strPointKbn,
			dProductPoint,
			productPrice,
			memberRankPointExcludeFlg
				? string.Empty
				: this.MemberRankId);
	}
	/// <summary>
	/// 商品付与ポイント取得
	/// </summary>
	/// <param name="strShopId">店舗ID</param>
	/// <param name="strPointKbn">付与ポイント区分</param>
	/// <param name="dProductPoint">商品ポイント</param>
	/// <param name="productPrice">Product price</param>
	/// <param name="MemberRankId">Member rank id</param>
	/// <returns>商品加算ポイント</returns>
	/// <remarks>商品が持つポイントを利用するときのみ表示</remarks>
	public static string GetProductAddPointString(string strShopId, string strPointKbn, decimal dProductPoint, decimal productPrice, string MemberRankId)
	{
		return PointOptionUtility.GetProductAddPointString(strShopId, strPointKbn, dProductPoint, productPrice, MemberRankId);
	}

	/// <summary>
	/// 商品付与ポイント取得(計算後ポイント)
	/// </summary>
	/// <param name="objProduct">商品情報</param>
	/// <param name="blHasVariation">バリエーションを持っているか</param>
	/// <param name="blIsVariationSelected">バリエーションが選択されているか</param>
	/// <param name="isFixedPurchase">定期購入かどうか</param>
	/// <returns>商品加算ポイント</returns>
	/// <remarks>商品が持つポイントを利用するときのみ表示</remarks>
	protected string GetProductAddPointCalculateAfterString(object objProduct, bool blHasVariation, bool blIsVariationSelected, bool isFixedPurchase = false)
	{
		//通常購入
		var point = Constants.FIELD_PRODUCT_POINT1;
		var pointKbn = Constants.FIELD_PRODUCT_POINT_KBN1;
		var price = GetProductValidityPrice(objProduct, blHasVariation, blIsVariationSelected);
		//定期購入で、定期購入ポイントを設定してある場合
		if (isFixedPurchase
			&& (((string)GetKeyValue(objProduct, Constants.FIELD_PRODUCT_POINT_KBN2) == Constants.FLG_PRODUCT_POINT_KBN2_NUM)
				|| ((string)GetKeyValue(objProduct, Constants.FIELD_PRODUCT_POINT_KBN2) == Constants.FLG_PRODUCT_POINT_KBN2_RATE)))
		{
			point = Constants.FIELD_PRODUCT_POINT2;
			pointKbn = Constants.FIELD_PRODUCT_POINT_KBN2;
			if (ProductPrice.GetFixedPurchasePrice(objProduct, true) != string.Empty)
			{
				price = Convert.ToDecimal(ProductPrice.GetFixedPurchasePrice(objProduct, true).Replace(",", ""));
			}
		}
		// 商品付与ポイント取得
		string strPoint = GetProductAddPointString(
				this.ShopId,
				StringUtility.ToEmpty(GetKeyValue(objProduct, pointKbn)),
				(decimal)GetKeyValue(objProduct, point),
				price,
				(string)GetKeyValue(objProduct, Constants.FIELD_PRODUCT_MEMBER_RANK_POINT_EXCLUDE_FLG) == Constants.FLG_PRODUCT_CHECK_MEMBER_RANK_POINT_EXCLUDE_FLG_VALID);

		// 商品が付与ポイントを持ち、かつ商品マスタの設定が「加算率」の場合、ポイントを計算
		if (PointOptionUtility.HasProductAddPoint(this.ShopId, StringUtility.ToEmpty(GetKeyValue(objProduct, pointKbn)))
			&& (StringUtility.ToEmpty(GetKeyValue(objProduct, pointKbn)) == Constants.FLG_PRODUCT_POINT_KBN1_RATE))
		{
			var productTaxCategory = new ProductTaxCategoryService().Get((string)GetKeyValue(objProduct, Constants.FIELD_PRODUCT_TAX_CATEGORY_ID));

			decimal dbPercentage = Convert.ToDecimal(strPoint.Replace("%", "")) / (decimal)100;
			return decimal.Floor(PointOptionUtility.GetTaxCaluculatedPrice(price, productTaxCategory.TaxRate) * dbPercentage).ToString() + Constants.CONST_UNIT_POINT_PT;
		}

		return strPoint;
	}

	/// <summary>
	/// Get product validity price
	/// </summary>
	/// <param name="product">Product</param>
	/// <param name="hasVariation">Has variation</param>
	/// <param name="isVariationSelected">Is variation selected</param>
	/// <returns>Product validity price</returns>
	public static decimal GetProductValidityPrice(object product, bool hasVariation, bool isVariationSelected)
	{
		decimal price = 0;

		// Normal price
		if (GetProductNormalPriceValid(product, (hasVariation && isVariationSelected)))
		{
			if (isVariationSelected == false)
			{
				price = (decimal)GetKeyValue(product, Constants.FIELD_PRODUCT_DISPLAY_PRICE);
			}
			else
			{
				price = (decimal)GetKeyValue(product, Constants.FIELD_PRODUCTVARIATION_PRICE);
			}
		}

		// Special price
		if (GetProductSpecialPriceValid(product, (hasVariation && isVariationSelected)))
		{
			if (isVariationSelected == false)
			{
				price = (decimal)StringUtility.ToValue(GetKeyValue(product, Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE), price);
			}
			else
			{
				price = (decimal)GetKeyValue(product, Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE);
			}
		}

		// Time sales price
		if (GetProductTimeSalesValid(product, (hasVariation && isVariationSelected)))
		{
			price = (decimal)GetKeyValue(product, Constants.FIELD_PRODUCTSALEPRICE_SALE_PRICE);
		}

		// Closed market price
		if (GetProductClosedMarketPriceValid(product, (hasVariation && isVariationSelected)))
		{
			price = (decimal)GetKeyValue(product, "closed_market_price");
		}

		// Member rank price
		if (Constants.MEMBER_RANK_OPTION_ENABLED
			&& GetProductMemberRankPriceValid(product, (hasVariation && isVariationSelected)))
		{
			if (isVariationSelected == false)
			{
				price = (decimal)GetKeyValue(product, Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE);
			}
			else
			{
				price = (decimal)GetKeyValue(product, Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE_VARIATION);
			}
		}

		return price;
	}

	#region キャンペーンアイコン有効状態取得（非推奨）
	/// <summary>
	/// キャンペーンアイコン有効状態取得
	/// </summary>
	/// <param name="objProduct">商品情報</param>
	/// <param name="iIcon">アイコン番号</param>
	/// <returns>IMGタグ</returns>
	[Obsolete("[V5.3] 使用しないのであれば削除します")]
	public static bool GetCampaignIconValid(object objProduct, int iIcon)
	{
		bool strResult = false;

		if (objProduct is DataRowView)
		{
			DataRowView drvProduct = (DataRowView)objProduct;
			switch (iIcon)
			{
				case 1:
					strResult = GetCampaignIconValid(drvProduct[Constants.FIELD_PRODUCT_ICON_FLG1].ToString(), drvProduct[Constants.FIELD_PRODUCT_ICON_TERM_END1], 1);
					break;
				case 2:
					strResult = GetCampaignIconValid(drvProduct[Constants.FIELD_PRODUCT_ICON_FLG2].ToString(), drvProduct[Constants.FIELD_PRODUCT_ICON_TERM_END2], 2);
					break;
				case 3:
					strResult = GetCampaignIconValid(drvProduct[Constants.FIELD_PRODUCT_ICON_FLG3].ToString(), drvProduct[Constants.FIELD_PRODUCT_ICON_TERM_END3], 3);
					break;
				case 4:
					strResult = GetCampaignIconValid(drvProduct[Constants.FIELD_PRODUCT_ICON_FLG4].ToString(), drvProduct[Constants.FIELD_PRODUCT_ICON_TERM_END4], 4);
					break;
				case 5:
					strResult = GetCampaignIconValid(drvProduct[Constants.FIELD_PRODUCT_ICON_FLG5].ToString(), drvProduct[Constants.FIELD_PRODUCT_ICON_TERM_END5], 5);
					break;
				case 6:
					strResult = GetCampaignIconValid(drvProduct[Constants.FIELD_PRODUCT_ICON_FLG6].ToString(), drvProduct[Constants.FIELD_PRODUCT_ICON_TERM_END6], 6);
					break;
				case 7:
					strResult = GetCampaignIconValid(drvProduct[Constants.FIELD_PRODUCT_ICON_FLG7].ToString(), drvProduct[Constants.FIELD_PRODUCT_ICON_TERM_END7], 7);
					break;
				case 8:
					strResult = GetCampaignIconValid(drvProduct[Constants.FIELD_PRODUCT_ICON_FLG8].ToString(), drvProduct[Constants.FIELD_PRODUCT_ICON_TERM_END8], 8);
					break;
				case 9:
					strResult = GetCampaignIconValid(drvProduct[Constants.FIELD_PRODUCT_ICON_FLG9].ToString(), drvProduct[Constants.FIELD_PRODUCT_ICON_TERM_END9], 9);
					break;
				case 10:
					strResult = GetCampaignIconValid(drvProduct[Constants.FIELD_PRODUCT_ICON_FLG10].ToString(), drvProduct[Constants.FIELD_PRODUCT_ICON_TERM_END10], 10);
					break;
				default:
					strResult = false;
					break;
			}
		}

		return strResult;
	}
	/// <summary>
	/// キャンペーンアイコン有効状態取得
	/// </summary>
	/// <param name="strIconFlag">アイコンフラグ</param>
	/// <param name="objIconTermEnd">アイコン表示期間</param>
	/// <param name="iIconNum">アイコン番号（1～）</param>
	/// <returns>IMGタグ</returns>
	[Obsolete("[V5.3] 使用しないのであれば削除します")]
	public static bool GetCampaignIconValid(string strIconFlag, object objIconTermEnd, int iIconNum)
	{
		if (strIconFlag == Constants.FLG_PRODUCT_ICON_ON)
		{
			if ((objIconTermEnd != System.DBNull.Value)
				&& (((DateTime)objIconTermEnd).CompareTo(DateTime.Now) >= 0))
			{
				return true;
			}
		}
		return false;
	}
	#endregion

	/// <summary>
	/// 商品一覧遷移URL作成
	/// </summary>
	/// <param name="objShopId">店舗ID</param>
	/// <param name="objCategoryId">カテゴリID</param>
	/// <param name="objSearchWord">検索文字列</param>
	/// <param name="objProductGroupId">商品グループID</param>
	/// <param name="objCampaignIcon">キャンペーンアイコン</param>
	/// <param name="objMinPrice">最小価格</param>
	/// <param name="objMaxPrice">最大価格</param>
	/// <param name="objSortKbn">ソート区分</param>
	/// <param name="objBrandId">ブランドID</param>
	/// <param name="objDispImageKbn">画像表示区分</param>
	/// <param name="objDispOnlySpPrice">特別価格設定商品検索フラグ</param>
	/// <param name="strCategoryName">カテゴリ名（フレンドリURL表示用）</param>
	/// <param name="strBrandName">ブランド名（フレンドリURL表示用）</param>
	/// <param name="strUndisplayNostock">在庫なし非表示</param>
	/// <param name="fixedPurchaseFilter">定期購入フィルタ</param>
	/// <param name="iDisplayCount">表示件数</param>
	/// <param name="saleFilter">セール対象フィルタ</param>
	/// <returns>作成URL</returns>
	public static string CreateProductListUrl(
		object objShopId,
		object objCategoryId,
		object objSearchWord,
		object objProductGroupId,
		object objCampaignIcon,
		object objMinPrice,
		object objMaxPrice,
		object objSortKbn,
		object objBrandId,
		object objDispImageKbn,
		object objDispOnlySpPrice,
		string strCategoryName,
		string strBrandName,
		string strUndisplayNostock,
		string fixedPurchaseFilter,
		int iDisplayCount,
		string saleFilter)
	{
		// ページ番号指定をしたくないため「-1」を渡す
		return ProductCommon.CreateProductListUrl(objShopId, objCategoryId, objSearchWord, objProductGroupId, objCampaignIcon, objMinPrice, objMaxPrice, objSortKbn, objBrandId, objDispImageKbn, objDispOnlySpPrice, strCategoryName, strBrandName, strUndisplayNostock, fixedPurchaseFilter, -1, iDisplayCount, saleFilter: saleFilter);
	}

	/// <summary>
	/// カテゴリリンク作成
	/// </summary>
	/// <param name="objShopId">店舗ID</param>
	/// <param name="objCategoryId">カテゴリID</param>
	/// <param name="objUrl">カテゴリトップURL（空の場合は通常の商品一覧へ遷移）</param>
	/// <param name="strCategoryName">カテゴリ名（フレンドリURL表示用）</param>
	/// <returns>作成URL</returns>
	protected string CreateCategoryLinkUrl(object objShopId, object objCategoryId, object objUrl, string strCategoryName)
	{
		return CreateCategoryLinkUrl(objShopId, objCategoryId, this.SortKbn, this.BrandId, this.DispImageKbn, this.DisplayOnlySpPrice, StringUtility.ToEmpty(objUrl), strCategoryName, this.BrandName, this.UndisplayNostock, this.FixedPurchaseFilter, this.DisplayCount, this.SaleFilter);
	}
	/// <summary>
	/// カテゴリリンク作成
	/// </summary>
	/// <param name="objShopId">店舗ID</param>
	/// <param name="objCategoryId">カテゴリID</param>
	/// <param name="objSortKbn">ソート区分</param>
	/// <param name="objBrandId">ブランドID</param>
	/// <param name="objDispImageKbn">画像表示区分</param>
	/// <param name="objDispOnlySpPrice">特別価格設定商品検索フラグ</param>
	/// <param name="strUrl">カテゴリトップURL（空の場合は通常の商品一覧へ遷移）</param>
	/// <param name="strCategoryName">カテゴリ名（フレンドリURL表示用）</param>
	/// <param name="strBrandName">ブランド名（フレンドリURL表示用）</param>
	/// <param name="strUndisplayNostock">在庫無し有無</param>
	/// <param name="fixedPurchaseFilter">定期購入フィルタ</param>
	/// <param name="iDisplayCount">表示件数</param>
	/// <param name="saleFilter">セール対象フィルタ</param>
	/// <returns>作成URL</returns>
	public static string CreateCategoryLinkUrl(object objShopId, object objCategoryId, object objSortKbn, object objBrandId, object objDispImageKbn, object objDispOnlySpPrice, string strUrl, string strCategoryName, string strBrandName, string strUndisplayNostock, string fixedPurchaseFilter, int iDisplayCount, string saleFilter)
	{
		if (strUrl != "")
		{
			if (strUrl.StartsWith(Uri.UriSchemeHttp) || strUrl.StartsWith(Uri.UriSchemeHttps))
			{
				return strUrl;
			}
			else
			{
				return Constants.PATH_ROOT + strUrl;
			}
		}
		return CreateProductListUrl(objShopId, objCategoryId, "", "", "", "", "", objSortKbn, objBrandId, objDispImageKbn, objDispOnlySpPrice, strCategoryName, strBrandName, strUndisplayNostock, fixedPurchaseFilter, iDisplayCount, saleFilter);
	}

	/// <summary>
	/// 商品詳細バリエーションURL作成
	/// </summary>
	/// <param name="objProduct">商品情報</param>
	/// <returns>商品詳細バリエーションURL</returns>
	public string CreateProductDetailVariationUrl(object objProduct)
	{
		return CreateProductDetailUrl(
			objProduct,
			(string)GetKeyValue(objProduct, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID));
	}

	/// <summary>
	/// 入荷通知メール登録URL作成
	/// </summary>
	/// <param name="strShopId">店舗ID</param>
	/// <param name="strProductId">商品ID</param>
	/// <param name="strVariationId">バリエーションID</param>
	/// <param name="strArrivalMailKbn">入荷通知メール区分</param>
	/// <param name="strProductUrl">登録元の商品詳細URL</param>
	/// <returns>入荷通知メール登録URL</returns>
	public static string CreateRegistUserProductArrivalMailUrl(string strShopId, string strProductId, string strVariationId, string strArrivalMailKbn, string strProductUrl)
	{
		StringBuilder sbUrl = new StringBuilder();
		sbUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_FRONT_USER_PRODUCT_ARRIVAL_MAIL_LIST);
		sbUrl.Append("?").Append(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_ACTION_KBN).Append("=").Append(HttpUtility.UrlEncode(Constants.KBN_REQUEST_USERPRODUCTARRIVALMAIL_REGIST));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_SHOP_ID).Append("=").Append(HttpUtility.UrlEncode(strShopId));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_PRODUCT_ID).Append("=").Append(HttpUtility.UrlEncode(strProductId));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_VARIATION_ID).Append("=").Append(HttpUtility.UrlEncode(strVariationId));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN).Append("=").Append(HttpUtility.UrlEncode(strArrivalMailKbn));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_BEFORE_PRODUCT_URL).Append("=").Append(HttpUtility.UrlEncode(strProductUrl));

		return sbUrl.ToString();
	}

	/// <summary>
	/// 商品問い合わせURL作成
	/// </summary>
	/// <param name="objProduct">商品情報</param>
	/// <param name="hasVariationSelected">バリエーションが選択されているか</param>
	/// <returns>商品問い合わせURL</returns>
	protected string CreateProductInquiryUrl(object objProduct, bool hasVariationSelected = false)
	{
		StringBuilder sbUrl = new StringBuilder();
		sbUrl.Append(this.SecurePageProtocolAndHost).Append(Constants.PATH_ROOT).Append(Constants.PAGE_FRONT_INQUIRY_INPUT);
		sbUrl.Append("?").Append(Constants.REQUEST_KEY_SHOP_ID).Append("=").Append(HttpUtility.UrlEncode((string)GetKeyValue(objProduct, Constants.FIELD_PRODUCT_SHOP_ID)));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_PRODUCT_ID).Append("=").Append(HttpUtility.UrlEncode((string)GetKeyValue(objProduct, Constants.FIELD_PRODUCT_PRODUCT_ID)));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_VARIATION_ID).Append("=").Append(hasVariationSelected ? HttpUtility.UrlEncode((string)GetKeyValue(objProduct, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID)) : "");

		return sbUrl.ToString();
	}

	/// <summary>
	/// 商品詳細URL作成(カテゴリIDをDBのCATEGORY_ID1から取得)
	/// </summary>
	/// <param name="product">商品情報</param>
	/// <param name="variationId">バリエーションID</param>
	/// <returns>商品詳細URL</returns>
	protected string CreateProductDetailUrlUseProductCategory(object product, string variationId)
	{
		return ProductCommon.CreateProductDetailUrlUseProductCategoryx(product, variationId, this.BrandId);
	}

	/// <summary>
	/// 商品詳細URL作成
	/// </summary>
	/// <param name="objProduct">商品情報</param>
	/// <param name="isVariation">バリエーションありか</param>
	/// <returns>商品詳細URL</returns>
	protected string CreateProductDetailUrl(object objProduct, bool isVariation = false)
	{
		var variationId = isVariation ? (string)GetKeyValueToNull(objProduct, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID) : "";
		return CreateProductDetailUrl(objProduct, variationId);
	}
	/// <summary>
	/// 商品詳細URL作成
	/// </summary>
	/// <param name="objProduct">商品情報</param>
	/// <param name="strVariationId">バリエーションID</param>
	/// <returns>商品詳細URL</returns>
	protected string CreateProductDetailUrl(object objProduct, string strVariationId)
	{
		var brandId = string.IsNullOrEmpty(this.BrandId) ? (string)GetKeyValue(objProduct, Constants.FIELD_PRODUCT_BRAND_ID1) : this.BrandId;
		return ProductCommon.CreateProductDetailUrl(
			objProduct,
			strVariationId,
			this.CategoryId,
			brandId,
			this.SearchWord,
			ProductBrandUtility.GetProductBrandName(brandId));
	}
	/// <summary>
	/// 商品詳細URL作成
	/// </summary>
	/// <param name="objShopId">店舗ID</param>
	/// <param name="objCategoryId">カテゴリID</param>
	/// <param name="objBrandId">ブランドID</param>
	/// <param name="objSearchWord">検索文字列</param>
	/// <param name="objProductId">商品ID</param>
	/// <param name="objVariationId">商品バリエーションID</param>
	/// <param name="strProductName">商品（バリエーション）名</param>
	/// <param name="strBrandName">ブランド名</param>
	/// <param name="previewPageNo">商品プレビューのページ番号</param>
	/// <returns>商品詳細URL</returns>
	public static string CreateProductDetailUrl(
		object objShopId,
		object objCategoryId,
		object objBrandId,
		object objSearchWord,
		object objProductId,
		object objVariationId,
		string strProductName,
		string strBrandName,
		string previewPageNo = "")
	{
		return ProductPageProcess.CreateProductDetailUrl(
			objShopId,
			objCategoryId,
			objBrandId,
			objSearchWord,
			objProductId,
			objVariationId,
			strProductName,
			strBrandName,
			previewPageNo);
	}

	/// <summary>
	/// 入荷通知メール情報削除URL作成
	/// </summary>
	/// <param name="objUserProductArrivalMail">入荷通知メール情報</param>
	/// <returns>入荷通知メール情報削除URL</returns>
	protected string CreateDeleteUserProductArrivalMailUrl(object objUserProductArrivalMail)
	{
		StringBuilder sbUrl = new StringBuilder();
		sbUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_FRONT_USER_PRODUCT_ARRIVAL_MAIL_LIST);
		sbUrl.Append("?").Append(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_ACTION_KBN).Append("=").Append(HttpUtility.UrlEncode(Constants.KBN_REQUEST_USERPRODUCTARRIVALMAIL_DELETE));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_USERPRODUCTARRIVALMAIL_MAIL_NO).Append("=").Append((int)GetKeyValue(objUserProductArrivalMail, Constants.FIELD_USERPRODUCTARRIVALMAIL_MAIL_NO));

		return sbUrl.ToString();
	}

	/// <summary>
	/// 商品在庫状況一覧URL作成（詳細用）
	/// </summary>
	/// <returns>商品在庫状況一覧URL</returns>
	protected string CreateProductStockListUrl()
	{
		StringBuilder sbUrl = new StringBuilder();
		sbUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_FRONT_PRODUCTSTOCK_LIST);
		sbUrl.Append("?").Append(Constants.REQUEST_KEY_SHOP_ID).Append("=").Append(HttpUtility.UrlEncode(this.ShopId));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_PRODUCT_ID).Append("=").Append(HttpUtility.UrlEncode(this.ProductId));

		return sbUrl.ToString();
	}

	/// <summary>
	/// リアル店舗商品在庫一覧URL作成（詳細用）
	/// </summary>
	/// <returns>店舗商品在庫一覧URL</returns>
	protected string CreateRealShopProductStockListUrl()
	{
		return CreateRealShopProductStockListUrl(this.ProductId, this.VariationId);
	}
	/// <summary>
	/// リアル店舗商品在庫一覧URL作成（詳細用）
	/// </summary>
	/// <param name="productId">商品ID</param>
	/// <param name="variationId">バリエーションID</param>
	/// <returns>店舗商品在庫一覧URL</returns>
	protected string CreateRealShopProductStockListUrl(string productId, string variationId)
	{
		StringBuilder result = new StringBuilder();

		result.Append(Constants.PATH_ROOT).Append(Constants.PAGE_FRONT_REALSHOPPRODUCTSTOCK_LIST);
		result.Append("?").Append(Constants.REQUEST_KEY_PRODUCT_ID).Append("=").Append(HttpUtility.UrlEncode(productId));
		result.Append("&").Append(Constants.REQUEST_KEY_VARIATION_ID).Append("=").Append(HttpUtility.UrlEncode(variationId));

		return result.ToString();
	}

	#region カート追加URL作成（非推奨）
	/// <summary>
	/// カート追加URL作成
	/// </summary>
	/// <param name="drvProduct">商品情報</param>
	/// <returns>カート追加URL</returns>
	[Obsolete("[V5.3] 使用しないのであれば削除します")]
	protected string CreateAddToCartUrl(DataRowView drvProduct, bool blIsFixedPurchase)
	{
		return CreateAddToCartUrl((string)drvProduct[Constants.FIELD_PRODUCT_SHOP_ID], (string)drvProduct[Constants.FIELD_PRODUCT_PRODUCT_ID], StringUtility.ToEmpty(drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]), blIsFixedPurchase);
	}
	/// <summary>
	/// カート追加URL作成
	/// </summary>
	/// <param name="drvProduct">商品情報</param>
	/// <param name="strVariationId">バリエーションID</param>
	/// <returns>カート追加URL</returns>
	[Obsolete("[V5.3] 使用しないのであれば削除します")]
	protected string CreateAddToCartUrl(DataRowView drvProduct, string strVariationId, bool blIsFixedPurchase)
	{
		return CreateAddToCartUrl((string)drvProduct[Constants.FIELD_PRODUCT_SHOP_ID], (string)drvProduct[Constants.FIELD_PRODUCT_PRODUCT_ID], strVariationId, blIsFixedPurchase);
	}
	/// <summary>
	/// カート追加URL作成
	/// </summary>
	/// <param name="strShopId">店舗ID</param>
	/// <param name="strProductId">商品ID</param>
	/// <returns>カート追加URL</returns>
	[Obsolete("[V5.3] 使用しないのであれば削除します")]
	public static string CreateAddToCartUrl(string strShopId, string strProductId, bool blIsFixedPurchase)
	{
		return CreateAddToCartUrl(strShopId, strProductId, "", blIsFixedPurchase);
	}
	/// <summary>
	/// カート追加URL作成
	/// </summary>
	/// <param name="strShopId">店舗ID</param>
	/// <param name="strProductId">商品ID</param>
	/// <param name="strVariationId">商品バリエーションID</param>
	/// <returns>カート追加URL</returns>
	[Obsolete("[V5.3] 使用しないのであれば削除します")]
	public static string CreateAddToCartUrl(string strShopId, string strProductId, string strVariationId, bool blIsFixedPurchase)
	{
		StringBuilder sbUrl = new StringBuilder();
		sbUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_FRONT_CART_LIST);
		sbUrl.Append("?").Append(Constants.REQUEST_KEY_SHOP_ID).Append("=").Append(HttpUtility.UrlEncode(strShopId));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_PRODUCT_ID).Append("=").Append(HttpUtility.UrlEncode(strProductId));
		if (strVariationId != "")
		{
			sbUrl.Append("&").Append(Constants.REQUEST_KEY_VARIATION_ID).Append("=").Append(HttpUtility.UrlEncode(strVariationId));
		}
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_CART_ACTION).Append("=").Append(HttpUtility.UrlEncode(Constants.KBN_REQUEST_CART_ACTION_ADD));
		if (blIsFixedPurchase)
		{
			sbUrl.Append("&").Append(Constants.REQUEST_KEY_FIXED_PURCHASE).Append("=").Append(HttpUtility.UrlEncode("1"));
		}

		return sbUrl.ToString();
	}
	#endregion

	/// <summary>
	/// 商品名＋バリエーション名作成
	/// </summary>
	/// <param name="objProduct">商品情報</param>
	/// <returns>商品名＋バリエーション名</returns>
	/// <remarks>バリエーションありなしを自動判定します。</remarks>
	public static string CreateProductJointName(object objProduct)
	{
		return ProductCommon.CreateProductJointName(objProduct);
	}
	/// <summary>
	/// 商品名＋バリエーション名作成
	/// </summary>
	/// <param name="strProductName">商品名</param>
	/// <param name="strVariationName1">バリエーション名１</param>
	/// <param name="strVariationName2">バリエーション名２</param>
	/// <param name="variationName3">バリエーション名3</param>
	/// <returns>商品名＋バリエーション名</returns>
	/// <remarks>バリエーションありなしを自動判定します。</remarks>
	public static string CreateProductJointName(string strProductName, string strVariationName1, string strVariationName2, string variationName3)
	{
		return ProductCommon.CreateProductJointName(strProductName, strVariationName1, strVariationName2, variationName3);
	}

	/// <summary>
	/// バリエーション名作成
	/// </summary>
	/// <param name="objProduct">商品情報</param>
	/// <returns>バリエーション名</returns>
	/// <remarks>バリエーションありなしを自動判定します。</remarks>
	public static string CreateVariationName(object objProduct)
	{
		if (objProduct is DataRowView)
		{
			return ProductCommon.CreateVariationName((DataRowView)objProduct);
		}
		throw new ArgumentException("パラメタエラー: objProduct is [" + objProduct.GetType().ToString() + "]");
	}
	/// <summary>
	/// バリエーション名作成
	/// </summary>
	/// <param name="objProduct">商品情報</param>
	/// <param name="strParenthesisLeft">左括弧</param>
	/// <param name="strParenthesisRight">右括弧</param>
	/// <param name="strPunctuation">区切り文字</param>
	/// <returns>バリエーション名</returns>
	/// <remarks>バリエーションありなしを自動判定します。</remarks>
	public static string CreateVariationName(object objProduct, string strParenthesisLeft, string strParenthesisRight, string strPunctuation)
	{
		if (objProduct is DataRowView)
		{
			return ProductCommon.CreateVariationName((DataRowView)objProduct, strParenthesisLeft, strParenthesisRight, strPunctuation);
		}
		else
		{
			return ProductCommon.CreateVariationName(
				(string)GetKeyValue(objProduct, Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1),
				(string)GetKeyValue(objProduct, Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2),
				(string)GetKeyValue(objProduct, Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3),
				strParenthesisLeft,
				strParenthesisRight,
				strPunctuation);
		}
	}
	/// <summary>
	/// バリエーション名作成
	/// </summary>
	/// <param name="strVariationName1">バリエーション名１</param>
	/// <param name="strVariationName2">バリエーション名２</param>
	/// <param name="variationName3">バリエーション名3</param>
	/// <returns>バリエーション名</returns>
	public static string CreateVariationName(string strVariationName1, string strVariationName2, string variationName3)
	{
		return ProductCommon.CreateVariationName(strVariationName1, strVariationName2, variationName3);
	}
	/// <summary>
	/// バリエーション名作成
	/// </summary>
	/// <param name="strVariationName1">バリエーション名１</param>
	/// <param name="strVariationName2">バリエーション名２</param>
	/// <param name="variationName3">バリエーション名3</param>
	/// <param name="strParenthesisLeft">左括弧</param>
	/// <param name="strParenthesisRight">右括弧</param>
	/// <param name="strPunctuation">区切り文字</param>
	/// <returns>バリエーション名</returns>
	public static string CreateVariationName(string strVariationName1, string strVariationName2, string variationName3, string strParenthesisLeft, string strParenthesisRight, string strPunctuation)
	{
		return ProductCommon.CreateVariationName(strVariationName1, strVariationName2, variationName3, strParenthesisLeft, strParenthesisRight, strPunctuation);
	}

	/// <summary>
	/// 商品の在庫切れ状態取得
	/// </summary>
	/// <param name="objectProduct">商品情報</param>
	/// <returns>有効状態 True:売切れ False:在庫有</returns>
	[Obsolete("[V5.4] V5.2の下位互換用のため今後使用しない")]
	public static bool IsProductSoldOut(object objectProduct)
	{
		return ProductListUtility.IsProductSoldOut(objectProduct);
	}

	/// <summary>
	/// 商品在庫購入可能チェック
	/// </summary>
	/// <param name="drvProductVariation">商品バリエーション</param>
	/// <param name="iBuyCount">購入数</param>
	/// <returns>商品在庫購入可能可否</returns>
	protected static bool CheckProductStockBuyable(DataRowView drvProductVariation, int iBuyCount)
	{
		return OrderCommon.CheckProductStockBuyable(drvProductVariation, iBuyCount);
	}
	/// <summary>
	/// 商品在庫購入可能チェック(全バリエーション)
	/// </summary>
	/// <param name="dvProductVariation">商品バリエーション</param>
	/// <param name="iBuyCount">購入数</param>
	/// <returns>バリエーション内に購入可能な在庫があるか</returns>
	protected static bool CheckProductStockBuyable(DataView dvProductVariation, int iBuyCount)
	{
		foreach (DataRowView drvProductVariation in dvProductVariation)
		{
			if (OrderCommon.CheckProductStockBuyable(drvProductVariation, iBuyCount))
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// 商品画像URL取得
	/// </summary>
	/// <param name="value">商品情報</param>
	/// <param name="imgSize">商品画像サイズ</param>
	/// <param name="isVariation">商品バリエーション画像を表示？</param>
	/// <param name="isGroupVariation">Is group variation</param>
	/// <returns>Product image url</returns>
	public static string CreateProductImageUrl(
		object value,
		string imgSize,
		bool isVariation = false,
		bool isGroupVariation = false)
	{
		// ファイル名ヘッダ
		string imageFileNameHead = null;
		if (isVariation)
		{
			imageFileNameHead = StringUtility.ToEmpty(ProductPage.GetKeyValue(value, Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD));
		}
		else if (Constants.SETTING_PRODUCT_LIST_SEARCH_KBN && isGroupVariation)
		{
			imageFileNameHead = StringUtility.ToEmpty(ProductPage.GetKeyValue(
				value,
				Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD));
		}
		else
		{
			imageFileNameHead = (string)ProductPage.GetKeyValue(value, Constants.FIELD_PRODUCT_IMAGE_HEAD);
		}

		// ファイル名フッタ
		string fileNameFoot = null;
		switch (StringUtility.ToEmpty(imgSize).ToUpper())
		{
			case "S":
				fileNameFoot = Constants.PRODUCTIMAGE_FOOTER_S;
				break;

			case "M":
				fileNameFoot = Constants.PRODUCTIMAGE_FOOTER_M;
				break;

			case "L":
				fileNameFoot = Constants.PRODUCTIMAGE_FOOTER_L;
				break;

			case "LL":
				fileNameFoot = Constants.PRODUCTIMAGE_FOOTER_LL;
				break;

			default:
				fileNameFoot = Constants.PRODUCTIMAGE_FOOTER_S;	// デフォルトはS
				break;
		}

		// 画像URL作成
		var imageUrl = new StringBuilder();
		if (imageFileNameHead.Contains(Uri.SchemeDelimiter))
		{
			// 外部画像URLの場合はスキーマをリプレース
			imageUrl.Append(imageFileNameHead.Replace(imageFileNameHead.Substring(0, imageFileNameHead.IndexOf(Uri.SchemeDelimiter)), HttpContext.Current.Request.Url.Scheme)).Append(fileNameFoot);
		}
		else
		{
			var isPreviewMode = (string.IsNullOrEmpty(PreviewGuidString) == false);
			if (isPreviewMode)
			{
				// For the case preview product from manager site
				imageUrl = Constants.PRODUCT_IMAGE_HEAD_ENABLED
					? new StringBuilder(Constants.PATH_ROOT + Constants.PATH_PRODUCTIMAGES)
						.AppendFormat(
							"{0}/{1}{2}",
							StringUtility.ToEmpty(ProductPage.GetKeyValue(value, Constants.FIELD_PRODUCT_SHOP_ID)),
							imageFileNameHead,
							Constants.PRODUCTIMAGE_FOOTER_LL)
					: new StringBuilder(Constants.PATH_ROOT + Constants.PATH_TEMP)
						.AppendFormat(
							"ProductImages/{0}/{1}{2}",
							PreviewGuidString,
							imageFileNameHead,
							Constants.PRODUCTIMAGE_FOOTER_LL);
			}
			else
			{
				var shopId = (string)ProductPage.GetKeyValue(value, Constants.FIELD_PRODUCT_SHOP_ID);
				imageUrl = new StringBuilder(Constants.PATH_ROOT + Constants.PATH_PRODUCTIMAGES)
					.AppendFormat(
						"{0}/{1}{2}",
						shopId,
						imageFileNameHead,
						fileNameFoot);
			}

			if (File.Exists(HttpContext.Current.Server.MapPath(imageUrl.ToString())) == false)
			{
				// 画像無しの場合はNOIMAGE画像
				imageUrl = new StringBuilder();
				imageUrl.Append(SmartPhoneUtility.GetSmartPhoneContentsUrl(Constants.PATH_PRODUCTIMAGES + Constants.PRODUCTIMAGE_NOIMAGE_HEADER + fileNameFoot));
			}
		}

		return EncodeImageUrl(imageUrl.ToString(), imageFileNameHead);
	}

	/// <summary>
	/// 商品サブ画像存在チェック
	/// </summary>
	/// <param name="drvProduct">商品情報</param>
	/// <param name="imageNo">サブ画像番号</param>
	/// <returns>True: If sub-image is exist</returns>
	public bool CheckProductSubImageExist(DataRowView drvProduct, int imageNo)
	{
		var subImageNames = new string[]
		{
			(string)drvProduct[Constants.FIELD_PRODUCT_IMAGE_HEAD] + Constants.PRODUCTSUBIMAGE_FOOTER + imageNo.ToString(Constants.PRODUCTSUBIMAGE_NUMBERFORMAT) + Constants.PRODUCTIMAGE_FOOTER_M,
			(string)drvProduct[Constants.FIELD_PRODUCT_IMAGE_HEAD] + Constants.PRODUCTSUBIMAGE_FOOTER + imageNo.ToString(Constants.PRODUCTSUBIMAGE_NUMBERFORMAT) + Constants.PRODUCTIMAGE_FOOTER_L,
			(string)drvProduct[Constants.FIELD_PRODUCT_IMAGE_HEAD] + Constants.PRODUCTSUBIMAGE_FOOTER + imageNo.ToString(Constants.PRODUCTSUBIMAGE_NUMBERFORMAT) + Constants.PRODUCTIMAGE_FOOTER_LL,
		};

		foreach (var imageName in subImageNames)
		{
			var subImagePath = string.IsNullOrEmpty(PreviewGuidString) || Constants.PRODUCT_IMAGE_HEAD_ENABLED
				? Path.Combine(
					AppDomain.CurrentDomain.BaseDirectory,
					Constants.PATH_PRODUCTSUBIMAGES,
					(string)drvProduct[Constants.FIELD_PRODUCT_SHOP_ID],
					imageName)
				: Path.Combine(
					AppDomain.CurrentDomain.BaseDirectory,
					Constants.PATH_TEMP,
					"ProductImages",
					PreviewGuidString,
					imageName);

			// 一つでも存在していればTRUE
			if (File.Exists(subImagePath))
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// 商品サブ画像画面URL作成
	/// </summary>
	/// <param name="iImageNo">サブ画像番号</param>
	/// <returns>商品サブ画像画面URL</returns>
	public string CreateProductSubImagePageUrl(int iImageNo)
	{
		StringBuilder sbUrl = new StringBuilder(CreateProductSubImagePageUrl());
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_PRODUCT_SUBIMAGE_NO).Append("=").Append(iImageNo.ToString(Constants.PRODUCTSUBIMAGE_NUMBERFORMAT));

		return sbUrl.ToString();
	}
	/// <summary>
	/// 商品サブ画像画面URL作成
	/// </summary>
	/// <returns>商品サブ画像画面URL</returns>
	public string CreateProductSubImagePageUrl()
	{
		StringBuilder sbUrl = new StringBuilder(Constants.PATH_ROOT).Append(Constants.PAGE_FRONT_PRODUCT_DETAIL_SUB_IMAGE);
		sbUrl.Append("?").Append(Constants.REQUEST_KEY_SHOP_ID).Append("=").Append(this.ShopId);
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_PRODUCT_ID).Append("=").Append(this.ProductId);

		return sbUrl.ToString();
	}

	/// <summary>
	/// 商品サブ画像URLの取得
	/// </summary>
	/// <param name="drvProduct">商品情報</param>
	/// <param name="strImageFooter">画像フッタ</param>
	/// <param name="iImageNo">サブ画像番号</param>
	/// <returns></returns>
	public string CreateProductSubImageUrl(object objValue, string strImageFooter, int iImageNo)
	{
		return CreateProductSubImageUrl((string)ProductPage.GetKeyValue(objValue, Constants.FIELD_PRODUCT_IMAGE_HEAD), strImageFooter, iImageNo);
	}
	/// <summary>
	/// 商品サブ画像URLの取得
	/// </summary>
	/// <param name="imageHeader">画像ヘッダ</param>
	/// <param name="imageFooter">画像フッタ</param>
	/// <param name="imageNo">サブ画像番号</param>
	/// <returns>Product sub-image Url</returns>
	public string CreateProductSubImageUrl(string imageHeader, string imageFooter, int imageNo)
	{
		var imageFilePath = new StringBuilder(Constants.PATH_ROOT);
		var isPreviewMode = (string.IsNullOrEmpty(PreviewGuidString) == false);
		if (isPreviewMode && Constants.PRODUCT_IMAGE_HEAD_ENABLED)
		{
			// For the case preview product from manager site
			imageFilePath.AppendFormat(
				"{0}{1}/",
				imageNo > Constants.PRODUCTSUBIMAGE_DEFAULT_SUB_IMAGE_NO ? Constants.PATH_PRODUCTIMAGES : Constants.PATH_PRODUCTSUBIMAGES,
				this.ShopId);
		}
		else if (isPreviewMode && (Constants.PRODUCT_IMAGE_HEAD_ENABLED == false))
		{
			imageFilePath.AppendFormat(
				"{0}ProductImages/{1}/",
				Constants.PATH_TEMP,
				PreviewGuidString);
		}
		else
		{
			imageFilePath.AppendFormat(
				"{0}{1}/",
				(imageNo > Constants.PRODUCTSUBIMAGE_DEFAULT_SUB_IMAGE_NO)
					? Constants.PATH_PRODUCTIMAGES
					: Constants.PATH_PRODUCTSUBIMAGES,
				this.ShopId);
		}

		imageFilePath
			.Append(imageHeader)
			.Append((imageNo > Constants.PRODUCTSUBIMAGE_DEFAULT_SUB_IMAGE_NO)
				? string.Empty
				: Constants.PRODUCTSUBIMAGE_FOOTER + imageNo.ToString(Constants.PRODUCTSUBIMAGE_NUMBERFORMAT))
			.Append(isPreviewMode
				? Constants.PRODUCTIMAGE_FOOTER_LL
				: imageFooter);

		// ファイルが存在しない場合はNowPrinting画像へのパスを作成
		if (File.Exists(Server.MapPath(imageFilePath.ToString())) == false)
		{
			imageFilePath = new StringBuilder();
			imageFilePath.Append(SmartPhoneUtility.GetSmartPhoneContentsUrl(Constants.PATH_PRODUCTSUBIMAGES + Constants.PRODUCTIMAGE_NOIMAGE_HEADER + imageFooter));
		}

		return EncodeImageUrl(imageFilePath.ToString(), imageHeader);
	}

	/// <summary>
	/// サブ画像番号の上限チェック
	/// </summary>
	/// <param name="subImageNo">サブ画像番号</param>
	/// <returns>サブ番号の上限値を超えていないか</returns>
	protected bool IsSubImagesNoLimit(int subImageNo)
	{
		return (subImageNo <= Constants.PRODUCTSUBIMAGE_MAXCOUNT);
	}

	/// <summary>
	/// 画像レイヤー表示用バリエーションリスト取得
	/// </summary>
	/// <param name="productVariationList">バリエーションリスト</param>
	/// <returns>画像レイヤー表示用バリエーションリスト</returns>
	public static Dictionary<string, List<DataRowView>> GetGroupedVariationList(DataView productVariationList)
	{
		if (productVariationList == null) return null;

		Dictionary<string, List<DataRowView>> groupedVariationList = new Dictionary<string, List<DataRowView>>();
		List<DataRowView> groupedVariation = new List<DataRowView>();
		List<string> variationNameList = new List<string>();
		string productIdTmp = null;

		foreach (DataRowView productVariation in productVariationList)
		{
			// 商品ID毎のバリエーション情報をリストに追加する
			if ((string)productVariation[Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID] != productIdTmp)
			{
				if (productIdTmp != null)
				{
					groupedVariationList.Add(productIdTmp, new List<DataRowView>(groupedVariation));
					groupedVariation.Clear();
					variationNameList.Clear();
				}
				productIdTmp = (string)productVariation[Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID];
			}

			// バリエーション情報をグループ毎に抽出
			bool variationNameAddFlg = false;
			foreach (string drv in variationNameList)
			{
				if (variationNameList.Contains((string)productVariation[Constants.LAYER_DISPLAY_VARIATION_GROUP_NAME]))
				{
					variationNameAddFlg = true;
					break;
				}
			}
			if (variationNameAddFlg)
			{
				continue;
			}

			// 対象商品のバリエーション情報を一時リストに追加する
			variationNameList.Add((string)productVariation[Constants.LAYER_DISPLAY_VARIATION_GROUP_NAME]);
			groupedVariation.Add(productVariation);
		}

		if (productIdTmp != null)
		{
			groupedVariationList.Add(productIdTmp, groupedVariation);
		}

		return groupedVariationList;
	}

	/// <summary>
	/// 対象商品(複数)のバリエーションリスト取得
	/// </summary>
	/// <param name="products">対象商品</param>
	/// <param name="memberRankId">会員ランクID</param>
	/// <returns>対象商品(複数)のバリエーションリスト</returns>
	public static DataView GetVariationList(DataView products, string memberRankId)
	{
		if (products.Count == 0)
		{
			return new DataView();
		}

		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("Product", "GetProductVariationList"))
		{
			Hashtable sqlParameters = new Hashtable();
			sqlParameters.Add(Constants.FIELD_PRODUCTVARIATION_SHOP_ID, (string)products[0][Constants.FIELD_PRODUCT_SHOP_ID]);
			sqlParameters.Add(Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID, memberRankId);

			// 商品ID検索用クエリ作成（SQLインジェクション対策：パラメタライズドクエリ）
			StringBuilder productIds = new StringBuilder();
			int count = 1;
			foreach (DataRowView product in products)
			{
				// インプットパラメタ作成＆設定
				string productIdsInputParam = "product_ids_" + count.ToString();
				sqlStatement.AddInputParameters(productIdsInputParam, SqlDbType.NVarChar, 30);
				sqlParameters.Add(productIdsInputParam, (string)product[Constants.FIELD_PRODUCT_PRODUCT_ID]);

				// IN句用の商品IDパラメタ作成
				productIds.Append((productIds.ToString() != "") ? "," : "");
				productIds.Append("@" + productIdsInputParam);

				count++;
			}
			sqlStatement.Statement = sqlStatement.Statement.Replace("@@ product_ids @@", productIds.ToString());

			// productViewセレクト用クエリ作成
			var columnNames = Constants.PRODUCT_VARIATIONLIST_SELECTCOLUMNS.Split(',');
			var selectColumns = columnNames.Select(columnName => "w2_ProductView." + columnName.Trim()).JoinToString(",");
			sqlStatement.Statement = sqlStatement.Statement.Replace("@@ productview_select_columns @@", selectColumns);

			// 商品バリエーション情報取得
			return sqlStatement.SelectSingleStatementWithOC(sqlAccessor, sqlParameters);
		}
	}

	/// <summary>
	/// バリエーションリスト(カート投入リスト用)作成
	/// </summary>
	/// <param name="productData">商品情報</param>
	/// <returns>バリエーションリスト(カート投入リスト用)</returns>
	protected List<Dictionary<string, object>> GetVariationAddCartList(DataView productData)
	{
		List<Dictionary<string, object>> variationAddCartList = new List<Dictionary<string, object>>();

		foreach (DataRowView variationData in productData)
		{
			// Continue when variation id is temp
			if (((string)variationData[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]).Contains("temp_")) continue;

			//------------------------------------------------------
			// 販売期間チェック & 在庫有無チェック（カートボタン非表示・在庫文言非表示、文言表示）
			//------------------------------------------------------
			// 購入会員ランク・販売期間・在庫有無チェック
			string errorMessage = CheckBuyableMemberRank(
						variationData,
						CheckProductStockBuyable(variationData, 1))
				.Replace("@@ 1 @@", variationData[Constants.FIELD_PRODUCT_NAME].ToString());

			// 注文数および加算数のチェック
			if (errorMessage == "")
			{
				// 注文数は1固定としておく
				errorMessage = CheckAddProductCount(StringUtility.ToHankaku("1"));
			}

			// 販売可能数量のチェック（カート内の数量を考慮する。投入量は1固定）
			if ((string.IsNullOrEmpty(errorMessage)) && (SessionManager.CartList != null))
			{
				errorMessage = GetMaxSellQuantityError(
					(string)variationData[Constants.FIELD_PRODUCT_SHOP_ID],
					(string)variationData[Constants.FIELD_PRODUCT_PRODUCT_ID],
					(string)variationData[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID],
					1);
			}

			// Check buyable fixed purchase member flag
			if (string.IsNullOrEmpty(errorMessage))
			{
				errorMessage = CheckBuyableFixedPurchaseMember(variationData);
			}

			// 購入可否判定
			bool buyable = (errorMessage == "");
			// 無効設定の商品 または モバイルのみ有効商品 の場合は購入不可
			if ((StringUtility.ToEmpty(variationData[Constants.FIELD_PRODUCT_VALID_FLG]) == Constants.FLG_PRODUCT_VALID_FLG_INVALID)
				 || (StringUtility.ToEmpty(variationData[Constants.FIELD_PRODUCT_MOBILE_DISP_FLG]) == Constants.FLG_PRODUCT_MOBILE_DISP_FLG_MOBLE)
				)
			{
				buyable = false;
			}

			//------------------------------------------------------
			// 表示用データ作成
			//------------------------------------------------------
			Dictionary<string, object> variationAddCartInfo = new Dictionary<string, object>();
			variationAddCartInfo.Add(Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID, (string)variationData[Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID]);
			variationAddCartInfo.Add(Constants.FIELD_PRODUCTVARIATION_VARIATION_ID, (string)variationData[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]);

			var variationName1 = (string)variationData[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1];
			var variationName2 = (string)variationData[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2];
			var variationName3 = (string)variationData[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3];
			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				var searchCondition = new NameTranslationSettingSearchCondition
				{
					DataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTVARIATION,
					MasterId1 = (string)variationData[Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID],
					MasterId2 = (string)variationData[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID],
					LanguageCode = RegionManager.GetInstance().Region.LanguageCode,
					LanguageLocaleId = RegionManager.GetInstance().Region.LanguageLocaleId,
				};
				var productTranslationSettings = new NameTranslationSettingService().GetTranslationSettingsByMasterIdAndLanguageCode(searchCondition);

				// foreachの繰り返し変数は編集できないため、別に格納して処理する
				var workData = variationData;
				workData = NameTranslationCommon.SetTranslationDataToDataRowView(workData, Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCT, productTranslationSettings);

				variationName1 = (string)workData[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1];
				variationName2 = (string)workData[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2];
				variationName3 = (string)workData[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3];
			}
			variationAddCartInfo.Add(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1, variationName1);
			variationAddCartInfo.Add(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2, variationName2);
			variationAddCartInfo.Add(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3, variationName3);

			variationAddCartInfo.Add(Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN, variationData[Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN]);
			variationAddCartInfo.Add(Constants.FIELD_PRODUCTSTOCK_STOCK,
				((variationData[Constants.FIELD_PRODUCTSTOCK_STOCK] is DBNull) || ((int)variationData[Constants.FIELD_PRODUCTSTOCK_STOCK] < 0)) ? 0 : (int)variationData[Constants.FIELD_PRODUCTSTOCK_STOCK]);
			variationAddCartInfo.Add(Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG, (string)variationData[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG]);

			// 在庫文言取得
			var stockMessage = ProductCommon.CreateProductStockMessage(variationData, true);
			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				var stockMessageTranslationSettings = (variationData[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_DEF] != DBNull.Value)
					? NameTranslationCommon.GetProductStockMessageTranslationSettings(
						(string)variationData[Constants.FIELD_PRODUCT_STOCK_MESSAGE_ID],
						RegionManager.GetInstance().Region.LanguageCode,
						RegionManager.GetInstance().Region.LanguageLocaleId)
					: null;
				var workVariation = variationData;
				workVariation = NameTranslationCommon.SetProductStockMessageTranslationData(workVariation, stockMessageTranslationSettings);
				stockMessage = ProductCommon.CreateProductStockMessage(workVariation, true);
			}
			variationAddCartInfo.Add("StockMessage", stockMessage);

			// カート投入ボタン表示判定
			var canAddCart = buyable
				&& ((Constants.GIFTORDER_OPTION_WITH_SHORTENING_GIFT_OPTION_ENABLED
						&& ((string)variationData[Constants.FIELD_PRODUCT_GIFT_FLG] != Constants.FLG_PRODUCT_GIFT_FLG_INVALID))
					|| (((string)variationData[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG] != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_ONLY)
						&& ((string)variationData[Constants.FIELD_PRODUCT_GIFT_FLG] != Constants.FLG_PRODUCT_GIFT_FLG_ONLY)
						&& ((string)variationData[Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG] != Constants.FLG_PRODUCT_SUBSCRIPTION_BOX_FLG_ONLY)));
			variationAddCartInfo.Add("CanCart", canAddCart);
			variationAddCartInfo.Add("CanFixedPurchase",
				buyable
				&& Constants.FIXEDPURCHASE_OPTION_ENABLED
				&& ((string)variationData[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG] != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID)
				&& ((string)variationData["shipping_" + Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_FLG] == Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_FLG_VALID));
			variationAddCartInfo.Add(
				"CanGiftOrder",
				buyable
					&& Constants.GIFTORDER_OPTION_ENABLED
					&& ((string)variationData[Constants.FIELD_PRODUCT_GIFT_FLG] != Constants.FLG_PRODUCT_GIFT_FLG_INVALID)
					&& (Constants.CART_LIST_LP_OPTION == false)
					&& (Constants.GIFTORDER_OPTION_WITH_SHORTENING_GIFT_OPTION_ENABLED == false));
			variationAddCartInfo.Add(Constants.ADD_CART_INFO_CAN_SUBSCRIPTION_BOX,
				buyable
				&& (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED
					&& ((string)variationData[Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG] != Constants.FLG_PRODUCT_SUBSCRIPTION_BOX_FLG_INVALID)));
			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				variationAddCartInfo.Add("ErrorMessage", errorMessage);
			}

			// 入荷通知メール区分取得
			variationAddCartInfo.Add("ArrivalMailKbn",
				UserProductArrivalMailCommon.GetArrivalMailKbn(HasVariation(variationData), true, productData, variationData));

			// 価格情報取得
			variationAddCartInfo.Add(Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE_VARIATION, variationData[Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE_VARIATION]);
			variationAddCartInfo.Add(Constants.FIELD_PRODUCTSALEPRICE_SALE_PRICE, variationData[Constants.FIELD_PRODUCTSALEPRICE_SALE_PRICE]);
			variationAddCartInfo.Add(Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE, variationData[Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE]);
			variationAddCartInfo.Add(Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE, variationData[Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE]);
			variationAddCartInfo.Add(Constants.FIELD_PRODUCTVARIATION_PRICE, variationData[Constants.FIELD_PRODUCTVARIATION_PRICE]);
			variationAddCartInfo.Add(Constants.FIELD_PRODUCT_DISPLAY_PRICE, variationData[Constants.FIELD_PRODUCT_DISPLAY_PRICE]);
			variationAddCartInfo.Add(Constants.FIELD_PRODUCT_FIXED_PURCHASE_FIRSTTIME_PRICE, variationData[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FIRSTTIME_PRICE]);
			variationAddCartInfo.Add(Constants.FIELD_PRODUCT_FIXED_PURCHASE_PRICE, variationData[Constants.FIELD_PRODUCT_FIXED_PURCHASE_PRICE]);
			variationAddCartInfo.Add(Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE, variationData[Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE]);
			variationAddCartInfo.Add(Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE, variationData[Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE]);

			// ポイント情報取得
			variationAddCartInfo.Add(Constants.FIELD_PRODUCT_POINT_KBN1, variationData[Constants.FIELD_PRODUCT_POINT_KBN1]);
			variationAddCartInfo.Add(Constants.FIELD_PRODUCT_POINT1, variationData[Constants.FIELD_PRODUCT_POINT1]);
			variationAddCartInfo.Add(Constants.FIELD_PRODUCT_POINT_KBN2, variationData[Constants.FIELD_PRODUCT_POINT_KBN2]);
			variationAddCartInfo.Add(Constants.FIELD_PRODUCT_POINT2, variationData[Constants.FIELD_PRODUCT_POINT2]);
			variationAddCartInfo.Add(Constants.FIELD_PRODUCTVARIATION_SHOP_ID, variationData[Constants.FIELD_PRODUCTVARIATION_SHOP_ID]);
			variationAddCartInfo.Add(Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG, variationData[Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG]);

			// 税込表示取得
			variationAddCartInfo.Add("TaxIncluded", GetTaxIncludeString(variationData));
			// 商品税率カテゴリID
			variationAddCartInfo.Add(Constants.FIELD_PRODUCT_TAX_CATEGORY_ID, variationData[Constants.FIELD_PRODUCT_TAX_CATEGORY_ID]);

			// セットプロモーションリスト
			var setPromotionList = DataCacheControllerFacade.GetSetPromotionCacheController().GetSetPromotionByProduct(
				variationData,
				true,
				this.MemberRankId,
				this.IsPc ? Constants.FLG_ORDER_ORDER_KBN_PC : Constants.FLG_ORDER_ORDER_KBN_SMARTPHONE,
				this.LoginUserHitTargetListIds);
			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				// セットプロモーション翻訳情報を設定
				setPromotionList = NameTranslationCommon.SetSetPromotionTranslationData(
					setPromotionList,
					RegionManager.GetInstance().Region.LanguageCode,
					RegionManager.GetInstance().Region.LanguageLocaleId);
			}
			variationAddCartInfo.Add("SetPromotionList", setPromotionList);

			var favorite = new FavoriteModel
			{
				ShopId = (string)variationData[Constants.FIELD_PRODUCTVARIATION_SHOP_ID],
				ProductId = (string)variationData[Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID],
				VariationId = (string)variationData[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID],
			};

			// 商品のお気に入り登録数取得
			variationAddCartInfo.Add("FavoriteCount", new FavoriteService().GetFavoriteByProduct(favorite));

			// リストに追加
			variationAddCartList.Add(variationAddCartInfo);
		}

		return variationAddCartList;
	}

	/// <summary>
	/// バリエーションDDL作成
	/// </summary>
	/// <param name="productData">商品情報</param>
	/// <returns>バリエーションDDL</returns>
	protected ListItemCollection GetVariationDropDownList(DataView productData)
	{
		var listItemCollection = new ListItemCollection();

		foreach (DataRowView variationData in productData)
		{
			// Continue when variation id is temp
			if (((string)variationData[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]).Contains("temp_")) continue;

			// 購入会員ランク・販売期間・在庫有無チェック
			var errorMessage = CheckBuyableMemberRank(
						variationData,
						CheckProductStockBuyable(variationData, 1))
				.Replace("@@ 1 @@", variationData[Constants.FIELD_PRODUCT_NAME].ToString());

			// 注文数および加算数のチェック
			if (errorMessage == "")
			{
				// 注文数は1固定としておく
				errorMessage = CheckAddProductCount(StringUtility.ToHankaku("1"));
			}

			// 販売可能数量のチェック（カート内の数量を考慮する。投入量は1固定）
			if ((string.IsNullOrEmpty(errorMessage)) && (SessionManager.CartList != null))
			{
				errorMessage = GetMaxSellQuantityError(
					(string)variationData[Constants.FIELD_PRODUCT_SHOP_ID],
					(string)variationData[Constants.FIELD_PRODUCT_PRODUCT_ID],
					(string)variationData[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID],
					1);
			}

			// Check buyable fixed purchase member flag
			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				continue;
			}

			// 無効設定の商品 または モバイルのみ有効商品 の場合は購入不可
			if ((StringUtility.ToEmpty(variationData[Constants.FIELD_PRODUCT_VALID_FLG]) == Constants.FLG_PRODUCT_VALID_FLG_INVALID)
				 || (StringUtility.ToEmpty(variationData[Constants.FIELD_PRODUCT_MOBILE_DISP_FLG]) == Constants.FLG_PRODUCT_MOBILE_DISP_FLG_MOBLE))
			{
				continue;
			}

			listItemCollection.Add(
				new ListItem(
					CreateVariationName(variationData),
					(string)variationData[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]));
		}
		return listItemCollection;
	}

	/// <summary>
	/// Check Buyable FixedPurchaseMember
	/// </summary>
	/// <param name="product">Product</param>
	/// <returns>Can buy or not</returns>
	protected string CheckBuyableFixedPurchaseMember(DataRowView product)
	{
		if (StringUtility.ToEmpty(product[Constants.FIELD_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG]) == Constants.FLG_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG_OFF)
			return string.Empty;

		var errorMessage = string.Empty;
		if ((StringUtility.ToEmpty(product[Constants.FIELD_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG]) == Constants.FLG_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG_ON)
			&& (this.IsFixedPurchaseMember == false))
			errorMessage = Constants.TAG_REPLACER_DATA_SCHEMA.ReplaceTextAll(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_BUYABLE_FIXED_PURCHASE_MEMBER));

		return errorMessage;
	}

	/// <summary>
	/// 販売可能数量超過エラー取得
	/// </summary>
	/// <param name="shopId">ショップID</param>
	/// <param name="productId">商品ID</param>
	/// <param name="variationId">バリエーションID</param>
	/// <param name="productCount">追加する個数</param>
	/// <returns></returns>
	public string GetMaxSellQuantityError(string shopId, string productId, string variationId, int productCount)
	{
		var product = GetProduct(shopId, productId, variationId);
		// 商品にバリエーションが設定されている場合、初回時にvariationId = ""が設定されており商品情報を取得出来ない
		if (product.Count == 0) return string.Empty;

		// カート内の情報を取得するために通常・定期の商品情報を取得
		var cartProductList = new List<CartProduct>
		{
			new CartProduct(
				product[0],
				Constants.AddCartKbn.Normal,
				"",
				productCount,
				false)
		};
		if (Constants.FIXEDPURCHASE_OPTION_ENABLED)
		{
			cartProductList.Add(
				new CartProduct(
					product[0],
					Constants.AddCartKbn.FixedPurchase,
					"",
					productCount,
					false)
				);
		}

		var cartProductCount = 0;
		// カート内の同一商品の個数を合算していく
		if (SessionManager.CartList != null)
		{
			foreach (CartObject cartObject in SessionManager.CartList)
			{
				foreach (var cartProduct in cartProductList)
				{
					var sameCart = cartObject.GetSameProductWithoutOptionSetting(cartProduct);
					if (sameCart != null)
					{
						cartProductCount += sameCart.CountSingle;
					}
				}
			}
		}

		var errorMessage = "";
		// カート内の合算した個数が販売可能数量をオーバーしていたらエラーメッセージを返す
		if ((productCount + cartProductCount) > cartProductList[0].ProductMaxSellQuantity)
		{ 
			errorMessage = OrderCommon.GetErrorMessage(
				OrderErrorcode.MaxSellQuantityError, 
				cartProductList[0].ProductJointName,
				(cartProductList[0].ProductMaxSellQuantity + 1).ToString("N0"));
		}

		return errorMessage;
	}

	/// <summary>
	/// 注文数および加算数の型チェック
	/// </summary>
	/// <param name="productCount">入力した購入数量</param>
	/// <returns>エラーメッセージ</returns>
	/// <remarks>在庫、販売可能数量を加味したチェックは別途行う</remarks>
	protected string CheckAddProductCount(string productCount)
	{
		Hashtable cartInput = new Hashtable()
			{
				{Constants.FIELD_CART_PRODUCT_COUNT, productCount}
			};

		return Validator.Validate("Cart", cartInput);
	}

	/// <summary>
	/// バリエーション単位でのお気に入り追加処理
	/// </summary>
	/// <param name="shopId">店舗ID</param>
	/// <param name="productId">商品ID</param>
	/// <param name="categoryId">カテゴリID</param>
	/// <param name="variationId">バリエーションID</param>
	protected void AddToFavorite(string shopId, string productId, string categoryId, string variationId)
	{
		if (string.IsNullOrEmpty(this.LoginUserId))
		{
			// 店IDと商品IDと現在時刻を繋げたものを暗号化
			var rcAuthenticationKey
				= new RijndaelCrypto(Constants.ENCRYPTION_USER_PASSWORD_KEY, Constants.ENCRYPTION_USER_PASSWORD_IV);
			var authenticationKey = rcAuthenticationKey.Encrypt(shopId + ":" + productId + ":" + variationId + ":" + DateTime.Now.ToString("yyyyMMddhhmmss"));

			this.Session[Constants.SESSION_KEY_FAV_TOKEN] = authenticationKey;

			var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_FAVORITE_LIST).AddParam(
				Constants.REQUEST_KEY_FAV_TOKEN,
				authenticationKey);
			var url = urlCreator.CreateUrl();
			this.Response.Redirect(url);
		}

		// お気に入りが既に登録してあるか確認する
		var isAlreadyRegister = IsAlreadyRegisterFavorite(shopId, this.LoginUserId, productId, variationId);
		if (this.IsDisplayPopupAddFavorite)
		{
			RegisterFavorite(shopId, this.LoginUserId, productId, variationId);
			ScriptManager.RegisterStartupScript(this, this.GetType(), "add_favorite_check", "add_favorite_check(" + isAlreadyRegister.ToString().ToLower() + ");", true);
			if (isAlreadyRegister) return;
		}

		// お気に入り登録
		RegisterFavorite(shopId, this.LoginUserId, productId, variationId);

		if (this.IsDisplayPopupAddFavorite == false)
		{
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_FAVORITE_LIST);
		}
	}

	/// <summary>
	/// お気に入り登録文言分岐
	/// </summary>
	/// <param name="shopId">店舗ID</param>
	/// <param name="userId">ユーザーID</param>
	/// <param name="productId">商品ID</param>
	/// <param name="variationId">バリエーションID</param>
	/// <returns>True:お気に入り登録済み, False:お気に入りに追加</returns>
	protected bool FavoriteDisplayWord(string shopId, string userId, string productId, string variationId)
	{
		return Constants.VARIATION_FAVORITE_CORRESPONDENCE && (IsAlreadyRegisterFavorite(shopId, userId, productId, variationId));
	}

	/// <summary>
	/// 在庫減少メール送信アラート表示有無
	/// </summary>
	/// <param name="shopId">店舗ID</param>
	/// <param name="userId">ユーザーID</param>
	/// <param name="productId">商品ID</param>
	/// <param name="variationId">バリエーションID</param>
	/// <returns>True:表示する, False:表示しない</returns>
	protected bool Alertdisplaycheck(string shopId, string userId, string productId, string variationId)
	{
		if (Constants.VARIATION_FAVORITE_CORRESPONDENCE && (IsAlreadyRegisterFavorite(shopId, userId, productId, variationId))) return false;

		if (this.IsLoggedIn
			&& (this.LoginUser.UserExtend != null)
			&& this.LoginUser.UserExtend.UserExtendColumns.Contains(Constants.FAVORITE_PRODUCT_DECREASE_MAILSENDFLG_USEREXRTEND_COLUMNNAME))
		{
			return Constants.PRODUCT_STOCK_OPTION_ENABLE
				&& ((this.LoginUser.UserExtend.UserExtendDataValue[key: Constants.FAVORITE_PRODUCT_DECREASE_MAILSENDFLG_USEREXRTEND_COLUMNNAME] ?? "0") == "1");
		}

		return false;
	}

	/// <summary>
	/// バリエーション単位でのお気に入り登録処理
	/// </summary>
	/// <param name="shopId">店舗ID</param>
	/// <param name="userId">ユーザID</param>
	/// <param name="productId">商品ID</param>
	/// <param name="variationId">バリエーションID</param>
	protected void RegisterFavorite(string shopId, string userId, string productId, string variationId)
	{
		var favorite = new FavoriteModel()
		{
			ShopId = shopId,
			UserId = userId,
			ProductId = productId,
			VariationId = variationId
		};
		new FavoriteService().Insert(favorite);
	}

	/// <summary>
	/// バリエーション単位でのお気に入りが既に存在するかどうか確認する
	/// </summary>
	/// <param name="shopId">店舗ID</param>
	/// <param name="userId">ユーザID</param>
	/// <param name="productId">商品ID</param>
	/// <param name="variationId">バリエーションID</param>
	/// <returns>true: 登録澄み、false:未登録</returns>
	protected bool IsAlreadyRegisterFavorite(string shopId, string userId, string productId, string variationId)
	{
		var favorite = new FavoriteModel()
		{
			ShopId = shopId,
			UserId = userId,
			ProductId = productId,
			VariationId = variationId
		};
		return new FavoriteService().IsAlreadyRegisterFavorite(favorite);
	}

	/// <summary>
	/// カート投入処理（カート投入後のリダイレクト指定あり）
	/// </summary>
	/// <param name="addCartKbn">カート投入区分</param>
	/// <param name="cartAddProductCount">注文数</param>
	/// <param name="redirectAfterAddProduct">カート投入後の遷移パラメタ</param>
	/// <param name="contentsLogModel">コンテンツログ</param>
	/// <returns></returns>
	protected string AddCart(Constants.AddCartKbn addCartKbn, string cartAddProductCount, string redirectAfterAddProduct, ContentsLogModel contentsLogModel = null)
	{
		// TODO: 商品一覧ページから受け取る時と同じになるか要確認 new ProductOptionSettingList()
		return AddCart(addCartKbn, cartAddProductCount, redirectAfterAddProduct, new ProductOptionSettingList(), contentsLogModel);
	}
	/// <summary>
	/// カート投入処理（カート投入後のリダイレクト指定＆商品付帯情報指定あり）
	/// </summary>
	/// <param name="addCartKbn">カート投入区分</param>
	/// <param name="cartAddProductCount">注文数</param>
	/// <param name="redirectAfterAddProduct">カート投入後の遷移パラメタ</param>
	/// <param name="productOptionSettingList">商品付帯情報</param>
	/// <param name="contentsLogModel">コンテンツログ</param>
	/// <param name="subscriptionBoxCourseId">頒布会コースID</param>
	/// <returns></returns>
	protected string AddCart(Constants.AddCartKbn addCartKbn, string cartAddProductCount, string redirectAfterAddProduct, ProductOptionSettingList productOptionSettingList, ContentsLogModel contentsLogModel = null, string subscriptionBoxCourseId = "")
	{
		//------------------------------------------------------
		// 商品情報取得
		//------------------------------------------------------
		DataView dvProduct = GetProduct(this.ShopId, this.ProductId, this.VariationId);
		if (dvProduct.Count == 0)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_NO_ITEM);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		// 商品付帯情報必須チェック
		var productOptionSettings = new ProductOptionSettingList((string)dvProduct[0][Constants.FIELD_PRODUCT_PRODUCT_OPTION_SETTINGS]);
		if ((productOptionSettings.Items.Count > 0)
			&& productOptionSettings.Items.Any(item => item.IsNecessary)
			&& (productOptionSettingList.Items.Count == 0))
		{
			var valueName = string.Empty;
			foreach (var productOptionSettingItems in productOptionSettings.Items)
			{
				if (productOptionSettingItems.IsNecessary) valueName = productOptionSettingItems.ValueName;
				break;
			}

			Session[Constants.SESSION_KEY_ERROR_FOR_ADD_CART] =
				WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_NO_ITEM).Replace("@@ 1 @@", valueName);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST);
		}

		// 購入数量型チェック
		string errorMessage = CheckAddProductCount(StringUtility.ToHankaku(cartAddProductCount));
		int productCount = (errorMessage == "") ? int.Parse(StringUtility.ToHankaku(cartAddProductCount)) : 1;

		// 商品在庫エラーor商品販売可能数量エラーの場合は詳細画面に文言表示
		OrderErrorcode productError = OrderCommon.CheckProductStatus(dvProduct[0], productCount, addCartKbn, this.LoginUserId);
		if (string.IsNullOrEmpty(errorMessage))
		{
			if ((productError == OrderErrorcode.ProductNoStockBeforeCart) 
					|| (productError == OrderErrorcode.MaxSellQuantityError))
			{
				errorMessage = OrderCommon.GetErrorMessage(
					productError,
					CreateProductJointName(dvProduct[0]),
					productCount.ToString());
			}
		}

		// 販売可能数量のチェック（カート内の数量を考慮する）
		if (string.IsNullOrEmpty(errorMessage))
		{
			errorMessage = GetMaxSellQuantityError(
				this.ShopId,
				this.ProductId,
				this.VariationId,
				productCount
			);
		}

		if (errorMessage == "")
		{
			if (productError != OrderErrorcode.NoError)
			{
				switch (productError)
				{
					case OrderErrorcode.SellMemberRankError:
						errorMessage = OrderCommon.GetErrorMessage(productError,
							CreateProductJointName(dvProduct[0]),
							MemberRankOptionUtility.GetMemberRankName((string)dvProduct[0][Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK]));
						break;

					default:
						errorMessage = OrderCommon.GetErrorMessage(productError, CreateProductJointName(dvProduct[0]));
						break;
				}

				Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}

			//------------------------------------------------------
			// カート投入処理
			//------------------------------------------------------
			// カート投入（商品付帯情報は何も選択されていない状態でカート投入）
			if (CanUseCartListLp() == false)
			{
				var cartList = GetCartObjectList();
				if (SessionManager.OrderCombineCartList == null)
				{
						cartList.AddProduct(
							dvProduct[0],
							addCartKbn,
						StringUtility.ToEmpty(dvProduct[0][Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID]),
						productCount,
						productOptionSettingList,
						contentsLogModel,
						null,
						null,
						subscriptionBoxCourseId);

					cartList.CartListShippingMethodUserUnSelected();
				}
				else
				{
						SessionManager.OrderCombineBeforeCartList.AddProduct(
							dvProduct[0],
							addCartKbn,
						StringUtility.ToEmpty(dvProduct[0][Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID]),
						productCount,
						productOptionSettingList,
						contentsLogModel);
				}

				var subscriptionBox = new SubscriptionBoxService().GetByCourseId(subscriptionBoxCourseId);
				if (subscriptionBox != null)
				{
					var subscriptionBoxFirstDefaultItem = subscriptionBox.IsNumberTime
						? subscriptionBox.DefaultOrderProducts.Where(
							item => (item.Count == 1)
							&& item.IsInSelectableTerm(DateTime.Now)).ToArray()
						: subscriptionBox.DefaultOrderProducts.Where(
							item => item.IsInTerm(DateTime.Now)).ToArray();
					var necessaryProducts = subscriptionBoxFirstDefaultItem.Where(
							product => product.IsNecessary).ToArray();
					if (necessaryProducts.Any() && (addCartKbn == Constants.AddCartKbn.SubscriptionBox))
					{
						foreach (var defaultOrderProduct in necessaryProducts)
						{
							var subscriptionProduct = GetProduct(
								this.ShopId,
								defaultOrderProduct.ProductId,
								defaultOrderProduct.VariationId);
							if (subscriptionProduct.Count == 0) continue;
							var subscriptionCart = cartList.Items.First(
								p => p.SubscriptionBoxCourseId == subscriptionBoxCourseId);
							if (subscriptionCart.Items.Any(p => p.VariationId == defaultOrderProduct.VariationId))
								continue;

							cartList.AddProduct(
								subscriptionProduct[0],
								addCartKbn,
								StringUtility.ToEmpty(
									subscriptionProduct[0][Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID]),
								defaultOrderProduct.ItemQuantity,
								productOptionSettingList,
								contentsLogModel,
								null,
								null,
								subscriptionBoxCourseId);
						}
					}
				}

				//------------------------------------------------------
				// カート投入後の画面遷移
				//------------------------------------------------------
				switch (redirectAfterAddProduct.ToUpper())
				{
					// カート一覧画面へ
					case Constants.KBN_REDIRECT_AFTER_ADDPRODUCT_CARTLIST:
						Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST);
						break;

					// 画面遷移しない
					default:
						break;
				}
			}
			else
			{
				// カート投入URLでLPカートリストページへ遷移する
				var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST_LP)
					.AddParam(Constants.REQUEST_KEY_CART_ACTION, "1")
					.AddParam(Constants.REQUEST_KEY_PRODUCT_ID, this.ProductId)
					.AddParam(Constants.REQUEST_KEY_VARIATION_ID, this.VariationId)
					.AddParam(Constants.REQUEST_KEY_PRODUCT_COUNT, cartAddProductCount);

				if (addCartKbn == Constants.AddCartKbn.FixedPurchase)
				{
					urlCreator.AddParam(Constants.REQUEST_KEY_FIXED_PURCHASE, "1");
				}

				if (addCartKbn == Constants.AddCartKbn.SubscriptionBox)
				{
					urlCreator.AddParam(Constants.REQUEST_KEY_SUBSCRIPTION_BOX, "1");
					urlCreator.AddParam(Constants.REQUEST_KEY_CART_SUBSCRIPTION_BOX_COURSE_ID, subscriptionBoxCourseId);
				}
				if (productOptionSettingList.Items.Count > 0)
				{
					for (var i = 0; i < productOptionSettingList.Items.Count; i++)
					{
						if (productOptionSettingList.Items[i].DisplayKbn
							== Constants.PRODUCTOPTIONVALUES_DISP_KBN_CHECKBOX)
						{
							var selectedSettingValues = productOptionSettingList.Items[i].SelectedSettingValue.Replace(
								Constants.PRODUCTOPTIONVALUES_SEPARATING_CHAR_SELECT_SETTING_VALUE,
								",")
								.Split(',');
							foreach (var selectedSettingValue in selectedSettingValues)
							{
								urlCreator.AddParam(
									Constants.REQUEST_KEY_PRODUCT_OPTION_VALUE + "_" + (i + 1),
									selectedSettingValue);
							}
						}
						else
						{
							urlCreator.AddParam(
								Constants.REQUEST_KEY_PRODUCT_OPTION_VALUE + "_" + (i + 1),
								productOptionSettingList.Items[i].SelectedSettingValue);
						}
					}
				}

				SessionManager.IsOnlyAddCartFirstTime = true;
				var url = urlCreator.CreateUrl();
				Response.Redirect(url);
			}
		}

		return errorMessage;
	}

	/// <summary>
	/// 入荷通知登録クライアントスクリプト作成
	/// </summary>
	/// <param name="amkbn">入荷通知区分</param>
	/// <returns>ポップアップスクリプト</returns>
	protected string CreateArivalMail2ClientScript(string amkbn)
	{
		return "if (request_user_product_arrival_mail_check()) return " + CreateArivalMail2ClientScript(amkbn, this.VariationId);
	}
	/// <summary>
	/// 入荷通知登録クライアントスクリプト作成
	/// </summary>
	/// <param name="amkbn">入荷通知区分</param>
	/// <param name="vid">バリエーションID</param>
	/// <returns>ポップアップスクリプト</returns>
	protected string CreateArivalMail2ClientScript(string amkbn, string vid)
	{
		return CreateArivalMail2ClientScript(amkbn, vid, this.ProductId);
	}
	/// <summary>
	/// 入荷通知登録クライアントスクリプト作成
	/// </summary>
	/// <param name="amkbn">入荷通知区分</param>
	/// <param name="vid">バリエーションID</param>
	/// <param name="pid">商品ID</param>
	/// <returns>ポップアップスクリプト</returns>
	protected string CreateArivalMail2ClientScript(string amkbn, string vid, string pid)
	{
		if ((vid != "") && (pid != ""))
		{
			return "show_arrival_mail_popup('" + HttpUtility.UrlEncode(pid) + "', '" + HttpUtility.UrlEncode(vid) + "', '" + amkbn + "');";
		}
		else
		{
			return "alert('" + MESSAGE_ERROR_VARIATION_UNSELECTED + "');";
		}
	}

	/// <summary>
	/// 入荷通知メールをPCメールアドレスで登録済みかどうか
	/// </summary>
	/// <param name="amkbn">入荷通知区分</param>
	/// <returns>登録済みであれば true</returns>
	protected bool IsArrivalMailPcRegistered(string amkbn)
	{
		return IsArrivalMailPcRegistered(amkbn, this.VariationId);
	}
	/// <summary>
	/// 入荷通知メールをPCメールアドレスで登録済みかどうか
	/// </summary>
	/// <param name="amkbn">入荷通知区分</param>
	/// <param name="vid">バリエーションID</param>
	/// <returns>登録済みであれば true</returns>
	protected bool IsArrivalMailPcRegistered(string amkbn, string vid)
	{
		return IsArrivalMailPcRegistered(amkbn, vid, this.ProductId);
	}
	/// <summary>
	/// 入荷通知メールをPCメールアドレスで登録済みかどうか
	/// </summary>
	/// <param name="amkbn">入荷通知区分</param>
	/// <param name="vid">バリエーションID</param>
	/// <param name="pid">商品ID</param>
	/// <returns>登録済みであれば true</returns>
	protected bool IsArrivalMailPcRegistered(string amkbn, string vid, string pid)
	{
		return IsArrivalMailRegistered(amkbn, vid, pid, Constants.FLG_USERPRODUCTARRIVALMAIL_PCMOBILE_KBN_PC);
	}

	/// <summary>
	/// 入荷通知メールをモバイルメールアドレスで登録済みかどうか
	/// </summary>
	/// <param name="amkbn">入荷通知区分</param>
	/// <returns>登録済みであれば true</returns>
	protected bool IsArrivalMailMobileRegistered(string amkbn)
	{
		return IsArrivalMailMobileRegistered(amkbn, this.VariationId);
	}
	/// <summary>
	/// 入荷通知メールをモバイルメールアドレスで登録済みかどうか
	/// </summary>
	/// <param name="amkbn">入荷通知区分</param>
	/// <param name="vid">バリエーションID</param>
	/// <returns>登録済みであれば true</returns>
	protected bool IsArrivalMailMobileRegistered(string amkbn, string vid)
	{
		return IsArrivalMailMobileRegistered(amkbn, vid, this.ProductId);
	}
	/// <summary>
	/// 入荷通知メールをモバイルメールアドレスで登録済みかどうか
	/// </summary>
	/// <param name="amkbn">入荷通知区分</param>
	/// <param name="vid">バリエーションID</param>
	/// <param name="pid">商品ID</param>
	/// <returns>登録済みであれば true</returns>
	protected bool IsArrivalMailMobileRegistered(string amkbn, string vid, string pid)
	{
		return IsArrivalMailRegistered(amkbn, vid, pid, Constants.FLG_USERPRODUCTARRIVALMAIL_PCMOBILE_KBN_MOBILE);
	}

	/// <summary>
	/// 入荷通知メールをその他メールアドレスで登録済みかどうか
	/// </summary>
	/// <param name="amkbn">入荷通知区分</param>
	/// <returns>登録済みであれば true</returns>
	protected bool IsArrivalMailGuestRegistered(string amkbn)
	{
		return IsArrivalMailGuestRegistered(amkbn, this.VariationId);
	}
	/// <summary>
	/// 入荷通知メールをその他メールアドレスで登録済みかどうか
	/// </summary>
	/// <param name="amkbn">入荷通知区分</param>
	/// <param name="vid">バリエーションID</param>
	/// <returns>登録済みであれば true</returns>
	protected bool IsArrivalMailGuestRegistered(string amkbn, string vid)
	{
		return IsArrivalMailGuestRegistered(amkbn, vid, this.ProductId);
	}

	/// <summary>
	/// 入荷通知メールをその他メールアドレスで登録済みかどうか
	/// </summary>
	/// <param name="amkbn">入荷通知区分</param>
	/// <param name="vid">バリエーションID</param>
	/// <param name="pid">商品ID</param>
	/// <returns>登録済みであれば true</returns>
	protected bool IsArrivalMailGuestRegistered(string amkbn, string vid, string pid)
	{
		return IsArrivalMailRegistered(amkbn, vid, pid, null);
	}

	/// <summary>
	/// 入荷通知メールを登録済みかどうか
	/// </summary>
	/// <param name="amkbn">入荷通知区分</param>
	/// <param name="vid">バリエーションID</param>
	/// <param name="pid">商品ID</param>
	/// <param name="pcMbKbn">PC/モバイル区分、null指定時はその他アドレス</param>
	/// <returns>登録済みであれば true</returns>
	private bool IsArrivalMailRegistered(string amkbn, string vid, string pid, string pcMbKbn)
	{
		foreach (var row in GetUserProductArrivalMailInfo(this.LoginUserId, pid, vid))
		{
			if ((DateTime)row[Constants.FIELD_USERPRODUCTARRIVALMAIL_DATE_EXPIRED] < DateTime.Now) continue; // 期限切れはスキップ
			if ((string)row[Constants.FIELD_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN] != amkbn) continue; // 異なる区分はスキップ

			if (pcMbKbn != null)
			{
				if ((string)row[Constants.FIELD_USERPRODUCTARRIVALMAIL_GUEST_MAIL_ADDR] != "") continue; // その他アドレスはスキップ
				if ((string)row[Constants.FIELD_USERPRODUCTARRIVALMAIL_PCMOBILE_KBN] == pcMbKbn) return true;
			}
			else
			{
				if ((string)row[Constants.FIELD_USERPRODUCTARRIVALMAIL_GUEST_MAIL_ADDR] != "") return true;
			}
		}
		return false;
	}

	/// <summary>
	/// いづれかの入荷通知メールが登録済みかどうか
	/// </summary>
	/// <param name="amkbn">入荷通知区分</param>
	/// <returns>登録済みであれば true</returns>
	protected bool IsArrivalMailAnyRegistered(string amkbn)
	{
		return IsArrivalMailAnyRegistered(amkbn, this.VariationId);
	}
	/// <summary>
	/// いづれかの入荷通知メールが登録済みかどうか
	/// </summary>
	/// <param name="amkbn">入荷通知区分</param>
	/// <param name="vid">バリエーションID</param>
	/// <returns>登録済みであれば true</returns>
	protected bool IsArrivalMailAnyRegistered(string amkbn, string vid)
	{
		return IsArrivalMailAnyRegistered(amkbn, vid, this.ProductId);
	}
	/// <summary>
	/// いづれかの入荷通知メールが登録済みかどうか
	/// </summary>
	/// <param name="amkbn">入荷通知区分</param>
	/// <param name="vid">バリエーションID</param>
	/// <param name="pid">商品ID</param>
	/// <returns>登録済みであれば true</returns>
	protected bool IsArrivalMailAnyRegistered(string amkbn, string vid, string pid)
	{
		foreach (var row in GetUserProductArrivalMailInfo(this.LoginUserId, pid, vid))
		{
			if ((DateTime)row[Constants.FIELD_USERPRODUCTARRIVALMAIL_DATE_EXPIRED] < DateTime.Now) continue; // 期限切れはスキップ
			if ((string)row[Constants.FIELD_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN] != amkbn) continue; // 異なる区分はスキップ

			// 登録済み
			return true;
		}
		return false;
	}

	/// <summary>
	/// ユーザー入荷通知メール情報取得（非静的オブジェクトにキャッシング）
	/// </summary>
	/// <param name="userId">ユーザーID</param>
	/// <param name="productId">商品ID</param>
	/// <param name="variationId">バリエーションID</param>
	/// <returns>ユーザー入荷通知メール情報</returns>
	private DataRowView[] GetUserProductArrivalMailInfo(string userId, string productId, string variationId)
	{
		return GetUserProductArrivalMailInfo(userId, productId)
			.Where(info => (string)info[Constants.FIELD_USERPRODUCTARRIVALMAIL_VARIATION_ID] == variationId)
			.ToArray();
	}
	/// <summary>
	/// ユーザー入荷通知メール情報取得（非静的オブジェクトにキャッシング）
	/// </summary>
	/// <param name="userId">ユーザーID</param>
	/// <param name="productId">商品ID</param>
	/// <returns>ユーザー入荷通知メール情報</returns>
	private DataRowView[] GetUserProductArrivalMailInfo(string userId, string productId)
	{
		// 恐らく必要ないのだが
		if (this.UserProductArrivalMailInfo.ContainsKey(productId) == false)
		{
			this.UserProductArrivalMailInfo[productId] =
				UserProductArrivalMailCommon.GetUserProductArrivalMailInfo(userId, productId)
					.Cast<DataRowView>().ToArray();
		}
		return this.UserProductArrivalMailInfo[productId];
	}

	/// <summary>
	/// 通知メール登録フォーム表示 - バリエーション項目クリック時の処理
	/// </summary>
	/// <param name="e">イベント</param>
	protected void ViewArrivalMailForm(RepeaterCommandEventArgs e)
	{
		var bpamrForm = GetWrappedControl<WrappedControl>(e.Item, "ucBpamr" + e.CommandArgument.ToString());
		bpamrForm.Visible = (bpamrForm.Visible == false);
	}

	/// <summary>
	/// 商品翻訳情報取得
	/// </summary>
	/// <param name="products">取得した商品情報一覧</param>
	/// <returns>商品翻訳情報</returns>
	protected DataView GetProductTranslationData(DataView products)
	{
		var productList = products.Cast<DataRowView>().Select(drv => new ProductModel(drv)).ToArray();
		var productTranslation = (DataView)NameTranslationCommon.Translate(products, productList);
		return productTranslation;
	}

	/// <summary>
	/// 商品バリエーション翻訳情報取得
	/// </summary>
	/// <param name="productVariations">取得した商品バリエーション情報一覧</param>
	/// <returns>商品バリエーション翻訳情報</returns>
	protected DataView GetProductVariationTranslationData(DataView productVariations)
	{
		var products = productVariations.Cast<DataRowView>().Select(drv => new ProductModel(drv)).ToArray();
		var productVariationsTranslation = (DataView)NameTranslationCommon.Translate(productVariations, products);
		return productVariationsTranslation;
	}

	/// <summary>
	/// 対象の商品を含むセットプロモーションを取得
	/// </summary>
	/// <param name="productInfo">商品情報</param>
	/// <returns>該当商品を含むセットプロモーション</returns>
	protected SetPromotionModel[] GetSetPromotionByProduct(object productInfo)
	{
		var setPromotionList = DataCacheControllerFacade.GetSetPromotionCacheController().GetSetPromotionByProduct(
			productInfo,
			false,
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
	/// Get Add Cart Kbn
	/// </summary>
	/// <param name="addCartKbn">Add cart kbn</param>
	/// <returns>Cart kbn</returns>
	protected string GetAddCartKbn(Constants.AddCartKbn addCartKbn)
	{
		if (addCartKbn == Constants.AddCartKbn.Normal) return Constants.FLG_ADD_CART_KBN_NORMAL;

		return Constants.FLG_ADD_CART_KBN_FIXEDPURCHASE;
	}

	/// <summary>
	/// オーダーデータでリードタイムを取得
	/// </summary>
	/// <param name="dataRowView">データビュー</param>
	/// <returns>リードタイム</returns>
	protected string GetLeadTimeByOrder(DataRowView dataRowView)
	{
		var shopId = (string)GetKeyValue(dataRowView, Constants.FIELD_ORDER_SHOP_ID);
		var deliveryCompanyId = (string)GetKeyValue(dataRowView, Constants.FIELD_ORDERSHIPPING_DELIVERY_COMPANY_ID);
		var prefecture = (string)GetKeyValue(dataRowView, Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1);
		var zip = (string)GetKeyValue(dataRowView, Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP);

		if ((deliveryCompanyId == null) || (prefecture == null) || (zip == null))
		{
			var orderShipping = new OrderService().GetShipping(
				(string)GetKeyValue(dataRowView, Constants.FIELD_ORDER_ORDER_ID),
				1);

			if (orderShipping != null)
			{
				if (deliveryCompanyId == null) deliveryCompanyId = orderShipping.DeliveryCompanyId;
				if (prefecture == null) prefecture = orderShipping.ShippingAddr1;
				if (zip == null) zip = orderShipping.ShippingZip;
			}
		}

		var leadTime = OrderCommon.GetTotalLeadTime(shopId, deliveryCompanyId, prefecture, zip);
		return leadTime.ToString();
	}

	/// <summary>
	/// 定期購入でリードタイムを取得
	/// </summary>
	/// <param name="fixedPurchase">定期台帳</param>
	/// <returns></returns>
	protected string GetLeadTimeByFixedPurchase(UserFixedPurchaseListSearchResult fixedPurchase)
	{
		var leadTime = OrderCommon.GetTotalLeadTime(
			fixedPurchase.ShopId,
			fixedPurchase.Shippings[0].DeliveryCompanyId,
			fixedPurchase.Shippings[0].ShippingAddr1,
			fixedPurchase.Shippings[0].ShippingZip);

		return leadTime.ToString();
	}
	/// <summary>
	/// 定期購入でリードタイムを取得
	/// </summary>
	/// <param name="fixedPurchase">定期台帳</param>
	/// <returns></returns>
	protected string GetLeadTimeByFixedPurchase(FixedPurchaseContainer fixedPurchase)
	{
		var leadTime = OrderCommon.GetTotalLeadTime(
			fixedPurchase.ShopId,
			fixedPurchase.Shippings[0].DeliveryCompanyId,
			fixedPurchase.Shippings[0].ShippingAddr1,
			fixedPurchase.Shippings[0].ShippingZip);

		return leadTime.ToString();
	}

	/// <summary>
	/// 予定出荷日取得
	/// </summary>
	/// <param name="fixedPurchase">定期台帳</param>
	/// <returns></returns>
	protected DateTime? GetScheduledShippingDateForFixedPurchase(UserFixedPurchaseListSearchResult fixedPurchase)
	{
		var scheduledShippingDate = OrderCommon.CalculateScheduledShippingDateBasedOnToday(
			fixedPurchase.ShopId,
			fixedPurchase.NextShippingDate,
			string.Empty,
			fixedPurchase.Shippings[0].DeliveryCompanyId,
			fixedPurchase.Shippings[0].ShippingCountryIsoCode,
			fixedPurchase.Shippings[0].ShippingAddr1,
			fixedPurchase.Shippings[0].ShippingZip);

		return scheduledShippingDate;
	}
	/// <summary>
	/// 予定出荷日取得
	/// </summary>
	/// <param name="fixedPurchase">定期台帳</param>
	/// <returns></returns>
	protected DateTime? GetScheduledShippingDateForFixedPurchase(FixedPurchaseContainer fixedPurchase)
	{
		var scheduledShippingDate = OrderCommon.CalculateScheduledShippingDateBasedOnToday(
			fixedPurchase.ShopId,
			fixedPurchase.NextShippingDate,
			string.Empty,
			fixedPurchase.Shippings[0].DeliveryCompanyId,
			fixedPurchase.Shippings[0].ShippingCountryIsoCode,
			fixedPurchase.Shippings[0].ShippingAddr1,
			fixedPurchase.Shippings[0].ShippingZip);

		return scheduledShippingDate;
	}

	/// <summary>
	/// 商品一覧表示設定をデフォルト設定もしくは表示可能かどうかに応じて取得
	/// </summary>
	/// <param name="settingId">設定ID</param>
	/// <param name="settingKbn">設定区分</param>
	/// <returns>商品一覧表示設定ID</returns>
	private static string GetProductListDispSettingIdDefaultOrUsable(string settingId, string settingKbn)
	{
		var productListDispSettings = DataCacheControllerFacade.GetProductListDispSettingCacheController().CacheData;
		var productListDispSetting = productListDispSettings
			.Where(drv => (drv.SettingKbn == settingKbn) && (drv.SettingId == settingId))
			.ToList();
		if (productListDispSetting.Any() == false)
		{
			var productListDispDefaultSetting = productListDispSettings.Where(
				drv => (drv.SettingKbn == settingKbn)
					&& (drv.DefaultDispFlg == Constants.FLG_PRODUCTLISTDISPSETTING_DEFAULT_DISP_FLG_ON)).ToList();
			if (productListDispDefaultSetting.Any()) return productListDispDefaultSetting.Select(drv => drv.SettingId).ToArray()[0];

			switch (settingKbn)
			{
				case Constants.FLG_PRODUCTLISTDISPSETTING_SETTING_KBN_SORT:
					return Constants.KBN_SORT_PRODUCT_LIST_DEFAULT;
				case Constants.FLG_PRODUCTLISTDISPSETTING_SETTING_KBN_IMG:
					return Constants.KBN_REQUEST_DISP_IMG_KBN_DEFAULT;
				case Constants.FLG_PRODUCTLISTDISPSETTING_SETTING_KBN_STOCK:
					return Constants.KBN_PRODUCT_LIST_UNDISPLAY_NOSTOCK_PRODUCT_DEFAULT;
			}
		}

		return productListDispSetting.Select(drv => drv.SettingId).ToArray()[0];
	}

	/// <summary>
	/// 頒布会の初回配送商品の取得
	/// </summary>
	/// <returns>初回配送商品</returns>
	public static SubscriptionBoxDefaultItemModel[] GetFirstProducts(SubscriptionBoxModel subscriptionBox)
	{
		var firstProducts = subscriptionBox.IsNumberTime
			? subscriptionBox.DefaultOrderProducts
				.Where(item => (item.Count == 1))
				.ToArray()
			: subscriptionBox.DefaultOrderProducts
				.Where(item => (item.TermSince <= DateTime.Now)
					&& (item.TermUntil > DateTime.Now))
				.ToArray();
		if (firstProducts.Any(item => item.IsNecessary))
		{
			firstProducts = firstProducts.Where(item => item.IsNecessary).ToArray();
		}

		return firstProducts;
	}

	/// <summary>
	/// 指定した商品が頒布会で選択可能な商品か？
	/// </summary>
	/// <param name="subscriptionBox">頒布会モデル</param>
	/// <param name="productId">商品ID</param>
	/// <param name="variationId">バリエーションID</param>
	/// <returns></returns>
	public static bool IsSelectableProduct(SubscriptionBoxModel subscriptionBox, string productId, string variationId)
	{
		var isSelectableProduct = false;
		// マイページからの商品変更可能な場合：選択可能商品のいずれかと合致するか
		// マイページからの商品変更不可な場合：初回デフォルト商品のいずれかと合致するか
		if (subscriptionBox.IsItemsChangeableByUser)
		{
			isSelectableProduct = subscriptionBox.SelectableProducts
				.Any(subscriptionBoxItem =>
					subscriptionBoxItem.IsInTerm(DateTime.Now)
					&& (subscriptionBoxItem.ProductId == productId)
					&& (subscriptionBoxItem.VariationId == variationId));
		}
		else
		{
			isSelectableProduct = subscriptionBox.GetDefaultProducts()
				.Any(subscriptionBoxDefaultItem =>
					subscriptionBoxDefaultItem.IsInTerm(DateTime.Now)
					&& (subscriptionBoxDefaultItem.ProductId == productId)
					&& (subscriptionBoxDefaultItem.VariationId == variationId));
		}

		return isSelectableProduct;
	}

	/// <summary>
	/// Encode image url
	/// </summary>
	/// <param name="url">Url</param>
	/// <param name="imageHead">Image head</param>
	/// <returns>A url after encode</returns>
	public static string EncodeImageUrl(string url, string imageHead)
	{
		var result = (string.IsNullOrEmpty(imageHead) == false)
			? url.Replace(imageHead, HttpUtility.UrlEncode(imageHead))
			: url;
		return result;
	}

	/// <summary>
	/// Is add cart by gift type
	/// </summary>
	/// <returns>True if is add cart by gift type, otherwise false</returns>
	protected bool IsAddCartByGiftType()
	{
		var products = GetProduct(this.ShopId, this.ProductId, this.VariationId);
		if (products.Count == 0) return false;

		var giftFlg = StringUtility.ToEmpty(products[0][Constants.FIELD_PRODUCT_GIFT_FLG]);
		var result = (Constants.GIFTORDER_OPTION_WITH_SHORTENING_GIFT_OPTION_ENABLED
			&& (giftFlg == Constants.FLG_PRODUCT_GIFT_FLG_ONLY));

		return result;
	}

	/// <summary>
	/// 税込商品価格を取得
	/// </summary>
	/// <param name="productVariation">商品情報</param>
	/// <returns>税込商品価格</returns>
	protected string GetVariationPriceTax(object productVariation)
	{
		var variationPrice = (decimal)GetKeyValueToNull(
			productVariation,
			Constants.SETTING_PRODUCT_LIST_SEARCH_KBN
				? Constants.FIELD_PRODUCTVARIATION_PRICE
				: Constants.FIELD_PRODUCT_DISPLAY_PRICE);
		var productTaxRate = (decimal)GetKeyValueToNull(productVariation, Constants.FIELD_PRODUCT_TAX_RATE);
		var variationTax = TaxCalculationUtility.GetTaxPrice(
			variationPrice,
			productTaxRate,
			Constants.TAX_EXCLUDED_FRACTION_ROUNDING);
		var variationPriceTax = CurrencyManager.ToPrice(TaxCalculationUtility.GetPriceTaxIncluded(variationPrice, variationTax));
		var result = string.Format(
			" ({0} {1})",
			CommonPage.ReplaceTag("@@DispText.tax_type.included_price@@"),
			variationPriceTax);
		return result;
	}

	/// <summary>
	/// 表示中商品数テキスト取得
	/// </summary>
	/// <param name="displayCount">表示件数</param>
	/// <returns>表示中商品数テキスト</returns>
	protected string GetDisplayPageCountText(int displayCount)
	{
		if (this.ProductListPaginationContentList == null) return string.Empty;
		var displayInfiniteLoadPageContentList = this.ProductListPaginationContentList.Where(x => x.IsLoaded).Select(x => x.PageNo).ToArray();
		var loadedPageMinimum = displayInfiniteLoadPageContentList.Min();
		var loadedPageMax = displayInfiniteLoadPageContentList.Max();
		var displayProductCountFrom = ((loadedPageMinimum - 1) * displayCount) + 1;
		var displayProductCountTo = (this.ProductListPaginationContentList.Length == loadedPageMax)
			? this.TotalProductCount
			: loadedPageMax * displayCount;

		return string.Format(
			CommonPage.ReplaceTag("@@DispText.product_display_count.name@@"),
			displayProductCountFrom,
			displayProductCountTo,
			this.TotalProductCount);
	}

	/// <summary>
	/// ページングコンテンツ取得
	/// </summary>
	/// <param name="pageNo">ページ番号</param>
	/// <returns>ページングコンテンツ</returns>
	protected ProductListPaginationContent GetProductListPaginationContent(int pageNo)
	{
		return this.ProductListPaginationContentList.Single(x => x.PageNo == pageNo);
	}

	/// <summary>
	/// 上方向の次ページ番号を取得
	/// </summary>
	/// <param name="pageNo">ページ番号</param>
	/// <returns>ページ番号</returns>
	protected int GetUpperNextLoadPageNo(int pageNo)
	{
		return GetUpperInfiniteLoadPageContent(pageNo).Where(x => x.IsLoaded == false).Select(x => x.PageNo).Max();
	}

	/// <summary>
	/// 下方向の次ページ番号を取得
	/// </summary>
	/// <param name="pageNo">ページ番号</param>
	/// <returns>ページ番号</returns>
	protected int GetLowerNextLoadPageNo(int pageNo)
	{
		return GetLowerInfiniteLoadPageContent(pageNo).Where(x => x.IsLoaded == false).Select(x => x.PageNo).Min();
	}

	/// <summary>
	/// 下方向のページコンテンツを取得
	/// </summary>
	/// <param name="pageNo">ページ番号</param>
	/// <returns>ページコンテンツ</returns>
	private ProductListPaginationContent[] GetLowerInfiniteLoadPageContent(int pageNo)
	{
		return this.ProductListPaginationContentList.Where(x => x.PageNo > pageNo).ToArray();
	}

	/// <summary>
	/// 上方向のページコンテンツを取得
	/// </summary>
	/// <param name="pageNo">ページ番号</param>
	/// <returns>ページコンテンツ</returns>
	private ProductListPaginationContent[] GetUpperInfiniteLoadPageContent(int pageNo)
	{
		return this.ProductListPaginationContentList.Where(x => x.PageNo < pageNo).ToArray();
	}

	/// <summary>プロセス</summary>
	protected new ProductPageProcess Process
	{
		get { return (ProductPageProcess)this.ProcessTemp; }
	}
	/// <summary>プロセステンポラリ</summary>
	protected override IPageProcess ProcessTemp
	{
		get
		{
			if (m_processTmp == null) m_processTmp = new ProductPageProcess(this, this.ViewState, this.Context);
			return m_processTmp;
		}
	}
	/// <summary>店舗ID</summary>
	protected string ShopId
	{
		get { return (ViewState["ShopId"] != null) ? (string)ViewState["ShopId"] : Constants.CONST_DEFAULT_SHOP_ID; }
		set { ViewState["ShopId"] = value; }
	}
	/// <summary>カテゴリID</summary>
	protected string CategoryId
	{
		get { return (string)ViewState["CategoryId"]; }
		set { ViewState["CategoryId"] = value; }
	}
	/// <summary>商品ID</summary>
	protected string ProductId
	{
		get { return (string)ViewState["ProductId"]; }
		set { ViewState["ProductId"] = value; }
	}
	/// <summary>バリエーションID</summary>
	protected string VariationId
	{
		get { return (string)ViewState["VariationId"]; }
		set { ViewState["VariationId"] = value; }
	}
	/// <summary>検索ワード</summary>
	protected string SearchWord
	{
		get { return (string)ViewState["SearchWord"]; }
		set { ViewState["SearchWord"] = value; }
	}
	/// <summary>キャンペーンアイコン</summary>
	public string CampaignIcon
	{
		get { return (string)ViewState["CampaignIcon"]; }
		set { ViewState["CampaignIcon"] = value; }
	}
	/// <summary>最小価格</summary>
	public string MinPrice
	{
		get { return (string)ViewState["MinPrice"]; }
		set { ViewState["MinPrice"] = value; }
	}
	/// <summary>最大価格</summary>
	public string MaxPrice
	{
		get { return (string)ViewState["MaxPrice"]; }
		set { ViewState["MaxPrice"] = value; }
	}
	/// <summary>表示件数</summary>
	public int DisplayCount
	{
		get { return (int)ViewState["DisplayCount"]; }
		set { ViewState["DisplayCount"] = value; }
	}
	/// <summary>ソート区分</summary>
	protected string SortKbn
	{
		get { return (string)ViewState["SortKbn"]; }
		set { ViewState["SortKbn"] = value; }
	}
	/// <summary>カラー</summary>
	protected string Color
	{
		get { return (string)ViewState["Color"]; }
		set { ViewState["Color"] = value; }
	}
	/// <summary>商品タグ</summary>
	public string ProductTag
	{
		get { return (string)ViewState["ProductTag"]; }
		set { ViewState["ProductTag"] = value; }
	}
	/// <summary>画像表示区分がオンか</summary>
	protected bool IsDispImageKbnOn
	{
		get { return (this.DispImageKbn == Constants.KBN_REQUEST_DISP_IMG_KBN_ON); }
	}
	/// <summary>画像表示区分がウィンドウショッピングか</summary>
	protected bool IsDispImageKbnWindowsShopping
	{
		get { return (this.DispImageKbn == Constants.KBN_REQUEST_DIST_IMG_KBN_WINDOWSHOPPING); }
	}
	/// <summary>画像表示</summary>
	protected string DispImageKbn
	{
		get { return (string)ViewState["DispImageKbn"]; }
		set { ViewState["DispImageKbn"] = value; }
	}
	/// <summary>在庫有無検索</summary>
	protected string UndisplayNostock
	{
		get { return (string)ViewState["StockExistenceSearch"]; }
		set { ViewState["StockExistenceSearch"] = value; }
	}
	/// <summary>定期購入フィルタ</summary>
	protected string FixedPurchaseFilter
	{
		get { return (string)ViewState["FixedPurchaseFilter"]; }
		set { ViewState["FixedPurchaseFilter"] = value; }
	}
	/// <summary>セール対象フィルタ</summary>
	protected string SaleFilter
	{
		get { return (string)ViewState["SaleFilter"]; }
		set { ViewState["SaleFilter"] = value; }
	}
	/// <summary>特別価格有無検索</summary>
	protected string DisplayOnlySpPrice
	{
		get { return (string)ViewState["DisplayOnlySpPrice"]; }
		set { ViewState["DisplayOnlySpPrice"] = value; }
	}
	/// <summary>ページ番号</summary>
	protected int PageNumber
	{
		get { return (int)ViewState["PageNumber"]; }
		set { ViewState["PageNumber"] = value; }
	}
	/// <summary>商品グループID</summary>
	protected string ProductGroupId
	{
		get { return (string)ViewState["ProductGroupId"]; }
		set { ViewState["ProductGroupId"] = value; }
	}
	/// <summary>商品カラーID</summary>
	protected string ProductColorId
	{
		get { return (string)ViewState["ProductColorId"]; }
		set { ViewState["ProductColorId"] = value; }
	}
	/// <summary>頒布会検索ワード</summary>
	protected string SubscriptionBoxSearchWord
	{
		get { return (string)ViewState["SubscriptionBoxSearchWord"]; }
		set { ViewState["SubscriptionBoxSearchWord"] = value; }
	}

	/// <summary>SEOキーワード</summary>
	public string SeoKeywords { get; set; }
	/// <summary>SEO説明</summary>
	public string SeoDescription { get; set; }
	/// <summary>SEOコメント</summary>
	public string SeoComment { get; set; }
	/// <summary>リクエストパラメーター</summary>
	public Dictionary<string, string> RequestParameter
	{
		get { return (Dictionary<string, string>)ViewState["RequestParameter"]; }
		set { ViewState["RequestParameter"] = value; }
	}

	/// <summary>お気に入り追加後ポップアップ表示フラグ（true:ポップアップ表示、false:お気に入り一覧ページへ遷移）</summary>
	public bool IsDisplayPopupAddFavorite
	{
		get { return (Session[Constants.SESSION_KEY_DISPPLAY_POPUP_ADD_FAVORITE] != null) ? (bool)Session[Constants.SESSION_KEY_DISPPLAY_POPUP_ADD_FAVORITE] : false; }
		set { Session[Constants.SESSION_KEY_DISPPLAY_POPUP_ADD_FAVORITE] = value; }
	}
	/// <summary>ユーザー入荷通知メール（商品IDがキー）</summary>
	private Dictionary<string, DataRowView[]> UserProductArrivalMailInfo
	{
		get { return m_userProductArrivalMailInfo; }
	}
	private readonly Dictionary<string, DataRowView[]> m_userProductArrivalMailInfo = new Dictionary<string, DataRowView[]>();

	/// <summary>Is fixed purchase member</summary>
	protected bool IsFixedPurchaseMember
	{
		get { return (StringUtility.ToEmpty(this.LoginUserFixedPurchaseMemberFlg) == Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_ON); }
	}
	/// <summary>ユーザーか定期購入可能か</summary>
	public bool IsUserFixedPurchaseAble { get; set; }
	/// <summary>Is subscription box Able</summary>
	public bool IsSubscriptionBoxValid { get; set; }
	/// <summary>Is subscription box Only</summary>
	public bool IsSubscriptionBoxOnly { get; set; }
	/// <summary>Preview guid string</summary>
	public static string PreviewGuidString
	{
		get
		{
			var guidString = HttpContext.Current.Request[Constants.REQUEST_KEY_PREVIEW_GUID_STRING];
			return guidString;
		}
	}
	/// <summary>トータル商品数</summary>
	protected int TotalProductCount
	{
		get { return (int)ViewState["TotalProductCount"]; }
		set { ViewState["TotalProductCount"] = value; }
	}
	/// <summary>ページングコンテンツリスト</summary>
	protected ProductListPaginationContent[] ProductListPaginationContentList
	{
		get { return (ProductListPaginationContent[])ViewState["ProductListPaginationContentList"]; }
		set { ViewState["ProductListPaginationContentList"] = value; }
	}
}
