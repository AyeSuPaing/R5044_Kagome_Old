/*
=========================================================================================================
  Module      : 商品系基底ユーザコントロール(ProductUserControl.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
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
using w2.App.Common.Global.Config;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Global.Translation;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.Product;
using w2.App.Common.ProductDefaultSetting;
using w2.App.Common.Web.Process;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Web;
using w2.Domain.ContentsLog;
using w2.Domain.LandingPage;
using w2.Domain.ProductCategory;
using w2.Domain.ProductTaxCategory;
using w2.Domain.SubscriptionBox;
using w2.App.Common.UserProductArrivalMail;

///*********************************************************************************************
/// <summary>
/// 商品系基底ユーザコントロール
/// </summary>
///*********************************************************************************************
public class ProductUserControl : BaseUserControl
{
	/// <summary>
	/// ページ初期化
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks>Page_Initイベント内でViewStateに入れた値はポストバック時に取得できなくなるため必ずパラメタ取得を行う</remarks>
	protected new void Page_Init(object sender, System.EventArgs e)
	{
		//------------------------------------------------------
		// 基底メソッドコール
		//------------------------------------------------------
		base.Page_Init(sender, e);

		//------------------------------------------------------
		// リクエストよりパラメタ取得（商品系画面共通処理）
		//------------------------------------------------------
		GetParameters();
	}

	/// <summary>
	/// 商品画面系パラメタ取得
	/// </summary>
	protected void GetParameters()
	{
		Dictionary<string, object> dicParams = ProductPage.GetParameters(Request);

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
		this.ProductGroupId = (string)dicParams[Constants.REQUEST_KEY_PRODUCT_GROUP_ID];
		this.ProductColorId = (string)dicParams[Constants.REQUEST_KEY_PRODUCT_COLOR_ID];
		this.SubscriptionBoxSearchWord = (string)dicParams[Constants.REQUEST_KEY_SUBSCRIPTION_BOX_SEARCH_WORD];
		if ((int)dicParams[Constants.REQUEST_KEY_DISP_PRODUCT_COUNT] == -1)
		{
			this.DisplayCount = (this.IsDispImageKbnOn) ? ProductListDispSettingUtility.CountDispContentsImgOn : ProductListDispSettingUtility.CountDispContentsWindowShopping;
		}
		else
		{
			this.DisplayCount = (int)dicParams[Constants.REQUEST_KEY_DISP_PRODUCT_COUNT];
		}
		this.UndisplayNostock = (string)dicParams[Constants.REQUEST_KEY_UNDISPLAY_NOSTOCK_PRODUCT];
		this.DisplayOnlySpPrice = (string)dicParams[Constants.REQUEST_KEY_DISP_ONLY_SP_PRICE];
		this.FixedPurchaseFilter = (string)dicParams[Constants.REQUEST_KEY_FIXED_PURCHASE_FILTER];
		this.ProductColorId = (string)dicParams[Constants.REQUEST_KEY_PRODUCT_COLOR_ID];
		this.SaleFilter = (string)dicParams[Constants.REQUEST_KEY_PRODUCT_SALE_FILTER];
		this.RequestParameter = new Dictionary<string, string>();
		foreach (string requestKey in dicParams.Keys)
		{
			this.RequestParameter.Add(requestKey, dicParams[requestKey].ToString());
		}
		// パラメーター削除（ブランドID、ブランド名）
		this.RequestParameter.Remove(Constants.REQUEST_KEY_BRAND_ID);
		this.RequestParameter.Remove(ProductCommon.URL_KEY_BRAND_NAME);
		// パラメーターセット（ブランドID、ブランド名）
		this.RequestParameter.Add(Constants.REQUEST_KEY_BRAND_ID, this.BrandId);
		this.RequestParameter.Add(ProductCommon.URL_KEY_BRAND_NAME, this.BrandName);
	}

	/// <summary>
	/// 親カテゴリ一覧取得（商品IDのカテゴリID1より取得）
	/// </summary>
	/// <param name="strShopId">店舗ID</param>
	/// <param name="strProductId">商品ID</param>
	/// <param name="onlyFixedPurchaseMemberFlg">Only fixed purchase member flag</param>
	/// <returns>親カテゴリ一覧（TOPカテゴリから順に格納されている）</returns>
	public static DataView GetParentCategoriesFromProductId(string strShopId, string strProductId, string onlyFixedPurchaseMemberFlg)
	{
		DataView dvParentCategories = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("Product", "GetParentCategoriesFromProductId"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_PRODUCT_SHOP_ID, strShopId);
			htInput.Add(Constants.FIELD_PRODUCT_PRODUCT_ID, strProductId);
			htInput.Add(Constants.FIELD_PRODUCTCATEGORY_ONLY_FIXED_PURCHASE_MEMBER_FLG, onlyFixedPurchaseMemberFlg);

			dvParentCategories = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
		}

		return dvParentCategories;
	}

	/// <summary>
	/// カテゴリリンク作成
	/// </summary>
	/// <returns>カテゴリURL</returns>
	protected string CreateCategoryLinkUrl(object objRecommendCategory)
	{
		// HACK:ProductPageProcessへ共通化したい
		// ProductPageの継承先が多すぎるため、共通化の際は影響範囲の精査とテストが必要

		return CreateCategoryLinkUrl(
			GetKeyValue(objRecommendCategory, Constants.FIELD_PRODUCTCATEGORY_SHOP_ID),
			GetKeyValue(objRecommendCategory, Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID),
			GetKeyValue(objRecommendCategory, Constants.FIELD_PRODUCTCATEGORY_URL),
			(string)GetKeyValue(objRecommendCategory, Constants.FIELD_PRODUCTCATEGORY_NAME));
	}
	/// <summary>
	/// カテゴリリンク作成
	/// </summary>
	/// <param name="strShopId">店舗ID</param>
	/// <param name="strCategoryId">カテゴリID</param>
	/// <param name="strUrl">カテゴリトップURL（空の場合は通常の商品一覧へ遷移）</param>
	/// <param name="strCategoryName">カテゴリ名（URL表示用）</param>
	/// <returns>カテゴリURL</returns>
	protected string CreateCategoryLinkUrl(
		object objShopId,
		object objCategoryId,
		object objUrl,
		string strCategoryName)
	{
		// HACK:ProductPageProcessへ共通化したい
		// ProductPageの継承先が多すぎるため、共通化の際は影響範囲の精査とテストが必要

		return ProductPage.CreateCategoryLinkUrl(
			objShopId,
			objCategoryId,
			this.SortKbn,
			this.BrandId,
			this.DispImageKbn,
			this.DisplayOnlySpPrice,
			StringUtility.ToEmpty(objUrl),
			strCategoryName,
			this.BrandName,
			this.UndisplayNostock,
			this.FixedPurchaseFilter,
			this.DisplayCount,
			this.SaleFilter);
	}

	/// <summary>
	/// 価格帯リンク作成
	/// </summary>
	/// <param name="minPrice">下限価格</param>
	/// <param name="maxPrice">上限価格</param>
	/// <returns>価格帯リンクURL</returns>
	protected string CreatePriceLinkUrl(int minPrice, int? maxPrice = null)
	{
		var priceLinkUrl = ProductPage.CreateProductListUrl(
			null,
			null,
			null,
			null,
			null,
			minPrice,
			maxPrice,
			this.SortKbn,
			this.BrandId,
			this.DispImageKbn,
			this.DisplayOnlySpPrice,
			string.Empty,
			this.BrandName,
			this.UndisplayNostock,
			this.FixedPurchaseFilter,
			this.DisplayCount,
			this.SaleFilter);
		return priceLinkUrl;
	}
	/// <summary>
	/// 価格帯リンク作成
	/// </summary>
	/// <param name="minPrice">下限価格</param>
	/// <param name="maxPrice">上限価格</param>
	/// <returns>価格帯リンクURL</returns>
	protected string CreatePriceLinkUrl(bool mustExchangePrice, decimal minPrice, decimal? maxPrice = null)
	{
		// 基軸通貨で価格変換が必要なら、変換
		if (mustExchangePrice)
		{
			minPrice = CurrencyManager.ExchangePriceToKeyCurrency(minPrice, this.CurrentCurrencyCode);
			if (maxPrice.HasValue)
			{
				maxPrice = CurrencyManager.ExchangePriceToKeyCurrency(maxPrice.Value, this.CurrentCurrencyCode);
			}
		}

		var priceLinkUrl = ProductPage.CreateProductListUrl(
			null,
			null,
			null,
			null,
			null,
			minPrice,
			maxPrice,
			this.SortKbn,
			this.BrandId,
			this.DispImageKbn,
			this.DisplayOnlySpPrice,
			string.Empty,
			this.BrandName,
			this.UndisplayNostock,
			this.FixedPurchaseFilter,
			this.DisplayCount,
			this.SaleFilter);
		return priceLinkUrl;
	}

	/// <summary>
	/// ルートカテゴリリストノード取得
	/// </summary>
	/// <param name="strShopId">店舗ID</param>
	/// <returns>トップカテゴリ一覧（カテゴリID順）</returns>
	protected List<ProductCategoryTreeNode> GetRootCategoryNodes(string strShopId)
	{
		Hashtable htInput = new Hashtable();
		htInput.Add(Constants.FIELD_PRODUCTCATEGORY_SHOP_ID, strShopId);
		htInput.Add(Constants.FIELD_PRODUCT_BRAND_ID1, this.BrandId);
		htInput.Add("root_category_sort_kbn", Constants.ROOT_CATEGORY_SORT_KBN);
		htInput.Add(Constants.FIELD_PRODUCTCATEGORY_ONLY_FIXED_PURCHASE_MEMBER_FLG, this.UserFixedPurchaseMemberFlg);

		DataView dvTopCategories = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("Product", "GetRootCategories"))
		{
			dvTopCategories = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
		}

		// 翻訳情報設定
		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			dvTopCategories = SetCategoryTranslationData(dvTopCategories);
		}

		List<ProductCategoryTreeNode> lRootCategoryList = new List<ProductCategoryTreeNode>();
		foreach (DataRowView drv in dvTopCategories)
		{
			lRootCategoryList.Add(new ProductCategoryTreeNode(drv));
		}

		return lRootCategoryList;
	}

	/// <summary>
	/// 商品在庫購入可能チェック(全バリエーション)
	/// </summary>
	/// <param name="dvProductVariation">商品バリエーション</param>
	/// <param name="iBuyCount">購入数</param>
	/// <returns>バリエーション内に購入可能な在庫があるか</returns>
	protected static bool CheckProductStockBuyable(DataView dvProductVariation, int iBuyCount)
	{
		// HACK:ProductPageProcessへ共通化したい
		// ProductPageの継承先が多すぎるため、共通化の際は影響範囲の精査とテストが必要

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
	/// 商品在庫購入可能チェック
	/// </summary>
	/// <param name="drvProductVariation">商品バリエーション</param>
	/// <param name="iBuyCount">購入数</param>
	/// <returns>商品在庫購入可能可否</returns>
	protected static bool CheckProductStockBuyable(DataRowView drvProductVariation, int iBuyCount)
	{
		// HACK:ProductPageProcessへ共通化したい
		// ProductPageの継承先が多すぎるため、共通化の際は影響範囲の精査とテストが必要

		return OrderCommon.CheckProductStockBuyable(drvProductVariation, iBuyCount);
	}

	/// <summary>
	/// 購入会員ランク・販売期間・在庫有無チェック
	/// </summary>
	/// <param name="productData">商品情報</param>
	/// <param name="stockFlg">在庫フラグ</param>
	/// <returns>エラーメッセージ</returns>
	/// <remarks>購入会員ランクチェック、販売期間チェック、在庫有無チェックの順に優先度が高い</remarks>
	public string CheckBuyableMemberRank(DataRowView productData, bool stockFlg)
	{
		// HACK:ProductPageProcessへ共通化したい
		// ProductPageの継承先が多すぎるため、共通化の際は影響範囲の精査とテストが必要

		// 購入可能会員ランク
		string buyableMemberRank = (string)productData[Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK];

		// 購入可能な会員ランク？
		if (MemberRankOptionUtility.CheckMemberRankPermission(this.MemberRankId, buyableMemberRank) == false)
		{
			return OrderCommon.GetErrorMessage(
				OrderErrorcode.SellMemberRankError,
				productData[Constants.FIELD_PRODUCT_NAME].ToString(),
				MemberRankOptionUtility.GetMemberRankName(buyableMemberRank));
		}

		// 商品販売前？
		if (ProductCommon.IsSellBefore(productData))
		{
			return OrderCommon.GetErrorMessage(
				OrderErrorcode.ProductBeforeSellTerm,
				productData[Constants.FIELD_PRODUCT_NAME].ToString());
		}

		// 商品販売後？
		if (ProductCommon.IsSellAfter(productData))
		{
			return OrderCommon.GetErrorMessage(
				OrderErrorcode.ProductOutOfSellTerm,
				productData[Constants.FIELD_PRODUCT_NAME].ToString());
		}

		// 商品販売期間外？
		if (ProductCommon.IsSellTerm(productData) == false)
		{
			return OrderCommon.GetErrorMessage(
				OrderErrorcode.ProductInvalid,
				productData[Constants.FIELD_PRODUCT_NAME].ToString());
		}
		// 在庫なし？
		if (stockFlg == false)
		{
			return OrderCommon.GetErrorMessage(
				OrderErrorcode.ProductNoStockBeforeCart,
				productData[Constants.FIELD_PRODUCT_NAME].ToString());
		}

		return "";
	}

	/// <summary>
	/// 商品サブ画像存在チェック
	/// </summary>
	/// <param name="drvProduct">商品情報</param>
	/// <param name="imageNo">サブ画像番号</param>
	/// <returns>True: If sub-image is exist</returns>
	public bool CheckProductSubImageExist(DataRowView drvProduct, int imageNo)
	{
		/// HACK:ProductPageProcessへ共通化したい
		/// ProductPageの継承先が多すぎるため、共通化の際は影響範囲の精査とテストが必要
		var subImageNames = new string[]
		{
			(string)drvProduct[Constants.FIELD_PRODUCT_IMAGE_HEAD] + Constants.PRODUCTSUBIMAGE_FOOTER + imageNo.ToString(Constants.PRODUCTSUBIMAGE_NUMBERFORMAT) + Constants.PRODUCTIMAGE_FOOTER_M,
			(string)drvProduct[Constants.FIELD_PRODUCT_IMAGE_HEAD] + Constants.PRODUCTSUBIMAGE_FOOTER + imageNo.ToString(Constants.PRODUCTSUBIMAGE_NUMBERFORMAT) + Constants.PRODUCTIMAGE_FOOTER_L,
			(string)drvProduct[Constants.FIELD_PRODUCT_IMAGE_HEAD] + Constants.PRODUCTSUBIMAGE_FOOTER + imageNo.ToString(Constants.PRODUCTSUBIMAGE_NUMBERFORMAT) + Constants.PRODUCTIMAGE_FOOTER_LL,
		};

		foreach (var imageName in subImageNames)
		{
			var subImagePath = Path.Combine(
				AppDomain.CurrentDomain.BaseDirectory,
				Constants.PATH_PRODUCTSUBIMAGES,
				(string)drvProduct[Constants.FIELD_PRODUCT_SHOP_ID],
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
		// HACK:ProductPageProcessへ共通化したい
		// ProductPageの継承先が多すぎるため、共通化の際は影響範囲の精査とテストが必要

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
	/// 商品会員ランク価格有効状態取得
	/// </summary>
	/// <param name="objProduct">商品情報</param>
	/// <param name="targetVariation">バリエーション対象</param>
	/// <returns>有効状態</returns>
	public static bool GetProductMemberRankPriceValid(object objProduct, bool targetVariation = false)
	{
		// HACK:ProductPageProcessへ共通化したい
		// ProductPageの継承先が多すぎるため、共通化の際は影響範囲の精査とテストが必要

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
		// HACK:ProductPageProcessへ共通化したい
		// ProductPageの継承先が多すぎるため、共通化の際は影響範囲の精査とテストが必要

		return (ProductPrice.GetProductPriceType(objProduct, targetVariation) == ProductPrice.PriceTypes.TimeSale);
	}

	/// <summary>
	/// 商品特別価格有効状態取得
	/// </summary>
	/// <param name="objProduct">商品情報</param>
	/// <param name="targetVariation">バリエーション対象</param>
	/// <returns>有効状態</returns>
	public static bool GetProductSpecialPriceValid(object objProduct, bool targetVariation = false)
	{
		// HACK:ProductPageProcessへ共通化したい
		// ProductPageの継承先が多すぎるため、共通化の際は影響範囲の精査とテストが必要

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
		// HACK:ProductPageProcessへ共通化したい
		// ProductPageの継承先が多すぎるため、共通化の際は影響範囲の精査とテストが必要

		return (ProductPrice.GetProductPriceType(objProduct, targetVariation) == ProductPrice.PriceTypes.Normal);
	}

	/// <summary>
	/// 商品付与ポイント取得
	/// </summary>
	/// <param name="objProduct">商品情報</param>
	/// <param name="hasVariation">バリエーションがあるか</param>
	/// <param name="isVariationSelected">バリエーションを選択しているか</param>
	/// <param name="isFixedPurchase">定期購入かどうか</param>
	/// <returns>商品加算ポイント</returns>
	/// <remarks>商品が持つポイントを利用するときのみ表示</remarks>
	protected string GetProductAddPointString(object objProduct, bool hasVariation = false, bool isVariationSelected = false, bool isFixedPurchase = false)
	{
		// HACK:ProductPageProcessへ共通化したい
		// ProductPageの継承先が多すぎるため、共通化の際は影響範囲の精査とテストが必要

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
	/// 商品の有効な価格を取得する
	/// </summary>
	/// <param name="product">商品</param>
	/// <param name="hasVariation">バリエーションが存在するか</param>
	/// <param name="isVariationSelected">バリエーションが選択されているか</param>
	/// <returns>商品の有効な価格</returns>
	public static decimal GetProductValidityPrice(object product, bool hasVariation, bool isVariationSelected)
	{
		// HACK:ProductPageProcessへ共通化したい
		// ProductPageの継承先が多すぎるため、共通化の際は影響範囲の精査とテストが必要

		decimal price = 0;

		// 通常価格
		if (GetProductNormalPriceValid(product, (hasVariation && isVariationSelected)))
		{
			price = (decimal)GetKeyValue(product, isVariationSelected
				? Constants.FIELD_PRODUCTVARIATION_PRICE
				: Constants.FIELD_PRODUCT_DISPLAY_PRICE);
		}

		// 特別価格
		if (GetProductSpecialPriceValid(product, (hasVariation && isVariationSelected)))
		{
			price = isVariationSelected
				? (decimal)GetKeyValue(product, Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE)
				: (decimal)StringUtility.ToValue(GetKeyValue(product, Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE), price);
		}


		// タイムセール価格
		if (GetProductTimeSalesValid(product, (hasVariation && isVariationSelected)))
		{
			price = (decimal)GetKeyValue(product, Constants.FIELD_PRODUCTSALEPRICE_SALE_PRICE);
		}

		// 会員ランク価格
		if (Constants.MEMBER_RANK_OPTION_ENABLED
			&& GetProductMemberRankPriceValid(product, (hasVariation && isVariationSelected)))
		{
			price = (decimal)GetKeyValue(product, isVariationSelected
				? Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE_VARIATION
				: Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE);
		}

		return price;
	}

	/// <summary>
	/// 販売可能数量超過エラー取得
	/// </summary>
	/// <param name="shopId">ショップID</param>
	/// <param name="productId">商品ID</param>
	/// <param name="variationId">バリエーションID</param>
	/// <param name="productCount">追加する個数</param>
	/// <returns>販売可能数量超過エラーメッセージ</returns>
	public string GetMaxSellQuantityError(string shopId, string productId, string variationId, int productCount)
	{
		// HACK:ProductPageProcessへ共通化したい
		// ProductPageの継承先が多すぎるため、共通化の際は影響範囲の精査とテストが必要

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
	/// 税込表示取得
	/// </summary>
	/// <param name="objProduct">商品情報</param>
	/// <returns>税込み/税抜き表示</returns>
	public static string GetTaxIncludeString(object objProduct)
	{
		// HACK:ProductPageProcessへ共通化したい
		// ProductPageの継承先が多すぎるため、共通化の際は影響範囲の精査とテストが必要

		return ValueText.GetValueText(
			Constants.TABLE_PRODUCT,
			Constants.FIELD_PRODUCT_TAX_INCLUDED_FLG,
			TaxCalculationUtility.GetPrescribedOrderItemTaxIncludedFlag());
	}

	/// <summary>
	/// 注文数および加算数の型チェック
	/// </summary>
	/// <param name="productCount">入力した購入数量</param>
	/// <returns>エラーメッセージ</returns>
	/// <remarks>在庫、販売可能数量を加味したチェックは別途行う</remarks>
	protected string CheckAddProductCount(string productCount)
	{
		// HACK:ProductPageProcessへ共通化したい
		// ProductPageの継承先が多すぎるため、共通化の際は影響範囲の精査とテストが必要

		Hashtable cartInput = new Hashtable()
			{
				{ Constants.FIELD_CART_PRODUCT_COUNT, productCount }
			};

		return Validator.Validate("Cart", cartInput);
	}

	/// <summary>
	/// 定期会員商品が購入可能か
	/// </summary>
	/// <param name="product">商品</param>
	/// <returns>購入可能かどうか</returns>

	protected string CheckBuyableFixedPurchaseMember(DataRowView product)
	{
		// HACK:ProductPageProcessへ共通化したい
		// ProductPageの継承先が多すぎるため、共通化の際は影響範囲の精査とテストが必要

		if (StringUtility.ToEmpty(product[Constants.FIELD_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG]) == Constants.FLG_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG_OFF)
		{
			return string.Empty;
		}

		var errorMessage = string.Empty;
		if ((StringUtility.ToEmpty(product[Constants.FIELD_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG]) == Constants.FLG_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG_ON)
			&& (this.IsFixedPurchaseMember == false))
		{
			errorMessage = Constants.TAG_REPLACER_DATA_SCHEMA.ReplaceTextAll(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_BUYABLE_FIXED_PURCHASE_MEMBER));
		}

		return errorMessage;
	}

	/// <summary>
	/// バリエーション変更向けデータバインド
	/// </summary>
	/// <param name="target">コントロールを探すターゲット</param>
	/// <param name="className">クラス名</param>
	public static void DataBindByClass(Control target, string className)
	{
		foreach (Control cInner in target.Controls)
		{
			if ((cInner is System.Web.UI.HtmlControls.HtmlGenericControl)
				&& ((System.Web.UI.HtmlControls.HtmlGenericControl)cInner).Attributes["class"] == className)
			{
				cInner.DataBind();
			}
			else if (cInner.Controls.Count != 0)
			{
				DataBindByClass(cInner, className);
			}
		}
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
	/// <returns>エラーメッセージ</returns>
	protected string AddCart(Constants.AddCartKbn addCartKbn, string cartAddProductCount, string redirectAfterAddProduct, ProductOptionSettingList productOptionSettingList, ContentsLogModel contentsLogModel = null, string subscriptionBoxCourseId = "")
	{
		// HACK:ProductPageProcessへ共通化したい
		// ProductPageの継承先が多すぎるため、共通化の際は影響範囲の精査とテストが必要

		// 商品情報取得
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
		int productCount = string.IsNullOrEmpty(errorMessage) ? int.Parse(StringUtility.ToHankaku(cartAddProductCount)) : 1;

		// 商品在庫エラーor商品販売可能数量エラーの場合は詳細画面に文言表示
		var productError = OrderCommon.CheckProductStatus(dvProduct[0], productCount, addCartKbn, this.LoginUserId);
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

		if (string.IsNullOrEmpty(errorMessage))
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
							{
								continue;
							}

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

				// カート投入後の画面遷移
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
				if (productOptionSettingList.Items.Any())
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
	/// 新LPカートリストが利用可能か
	/// </summary>
	/// <returns>利用可能：TRUE、利用不可：FALSE</returns>
	public bool CanUseCartListLp()
	{
		// HACK:ProductPageProcessへ共通化したい
		// ProductPageの継承先が多すぎるため、共通化の際は影響範囲の精査とテストが必要

		var result = false;
		if (Constants.CART_LIST_LP_OPTION)
		{
			var landingPageDesignModel = new LandingPageService().GetPageByFileName(Constants.CARTLIST_LP_FILE_NAME.Replace(".aspx", string.Empty));
			result = landingPageDesignModel.Any() && (landingPageDesignModel[0].IsPublished);
		}

		return result;
	}

	/// <summary>
	/// ギフトタイプでカートに追加するかどうか
	/// </summary>
	/// <returns>ギフトタイプでカートに追加する場合はtrue、そうでない場合はfalse</returns>
	protected bool IsAddCartByGiftType()
	{
		// HACK:ProductPageProcessへ共通化したい
		// ProductPageの継承先が多すぎるため、共通化の際は影響範囲の精査とテストが必要

		var products = GetProduct(this.ShopId, this.ProductId, this.VariationId);
		if (products.Count == 0) return false;

		var giftFlg = StringUtility.ToEmpty(products[0][Constants.FIELD_PRODUCT_GIFT_FLG]);
		var result = (Constants.GIFTORDER_OPTION_WITH_SHORTENING_GIFT_OPTION_ENABLED
			&& (giftFlg == Constants.FLG_PRODUCT_GIFT_FLG_ONLY));

		return result;
	}

	/// <summary>
	/// 商品名＋バリエーション名作成
	/// </summary>
	/// <param name="objProduct">商品情報</param>
	/// <returns>商品名＋バリエーション名</returns>
	/// <remarks>バリエーションありなしを自動判定します。</remarks>
	public static string CreateProductJointName(object objProduct)
	{
		// HACK:ProductPageProcessへ共通化したい
		// ProductPageの継承先が多すぎるため、共通化の際は影響範囲の精査とテストが必要

		return ProductCommon.CreateProductJointName(objProduct);
	}

	/// <summary>
	/// 商品データ取得
	/// </summary>
	/// <param name="key">商品情報</param>
	/// <param name="key">キー（フィールド）</param>
	/// <returns>商品データ</returns>
	protected object GetProductData(object product, string key)
	{
		// HACK:ProductPageProcessへ共通化したい
		// ProductPageの継承先が多すぎるため、共通化の際は影響範囲の精査とテストが必要

		switch (key)
		{
			case Constants.FIELD_PRODUCTVARIATION_VARIATION_ID:
				return (string.IsNullOrEmpty(this.VariationId) != false) ? GetKeyValue(product, Constants.FIELD_PRODUCTVARIATION_VARIATION_ID) : GetKeyValue(product, Constants.FIELD_PRODUCT_PRODUCT_ID);
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
		// HACK:ProductPageProcessへ共通化したい
		// ProductPageの継承先が多すぎるため、共通化の際は影響範囲の精査とテストが必要

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
		// HACK:ProductPageProcessへ共通化したい
		// ProductPageの継承先が多すぎるため、共通化の際は影響範囲の精査とテストが必要

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
		// HACK:ProductPageProcessへ共通化したい
		// ProductPageの継承先が多すぎるため、共通化の際は影響範囲の精査とテストが必要

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
	/// 商品在庫状況一覧URL作成（詳細用）
	/// </summary>
	/// <returns>商品在庫状況一覧URL</returns>
	protected string CreateProductStockListUrl()
	{
		// HACK:ProductPageProcessへ共通化したい
		// ProductPageの継承先が多すぎるため、共通化の際は影響範囲の精査とテストが必要

		StringBuilder sbUrl = new StringBuilder();
		sbUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_FRONT_PRODUCTSTOCK_LIST);
		sbUrl.Append("?").Append(Constants.REQUEST_KEY_SHOP_ID).Append("=").Append(HttpUtility.UrlEncode(this.ShopId));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_PRODUCT_ID).Append("=").Append(HttpUtility.UrlEncode(this.ProductId));

		return sbUrl.ToString();
	}

	/// <summary>
	/// 定期購入初回価格有効状態取得
	/// </summary>
	/// <param name="product">商品情報</param>
	/// <param name="targetVariation">バリエーション対象</param>
	/// <returns>有効状態</returns>
	public static bool IsProductFixedPurchaseFirsttimePriceValid(object product, bool targetVariation = false)
	{
		// HACK:ProductPageProcessへ共通化したい
		// ProductPageの継承先が多すぎるため、共通化の際は影響範囲の精査とテストが必要

		if ((string)GetKeyValue(product, Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG) == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID) return false;

		var fixedPurchaseFirsttimePrice = StringUtility.ToEmpty(GetKeyValue(product, targetVariation
			? Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE
			: Constants.FIELD_PRODUCT_FIXED_PURCHASE_FIRSTTIME_PRICE));

		return (fixedPurchaseFirsttimePrice != string.Empty);
	}

	/// <summary>
	/// 商品サブ画像URLの取得
	/// </summary>
	/// <param name="drvProduct">商品情報</param>
	/// <param name="strImageFooter">画像フッタ</param>
	/// <param name="iImageNo">サブ画像番号</param>
	/// <returns>サブ画像URL</returns>
	public string CreateProductSubImageUrl(object objValue, string strImageFooter, int iImageNo)
	{
		return CreateProductSubImageUrl((string)ProductPage.GetKeyValue(objValue, Constants.FIELD_PRODUCT_IMAGE_HEAD), strImageFooter, iImageNo);
	}

	/// <summary>
	/// 商品詳細URL作成
	/// </summary>
	/// <param name="product">商品マスタ</param>
	/// <returns>商品詳細URL</returns>
	protected string CreateProductDetailUrl(object product)
	{
		return CreateProductDetailUrl(product, "");
	}
	/// <summary>
	/// 商品詳細URL作成
	/// </summary>
	/// <param name="product">商品マスタ</param>
	/// <param name="variationId">バリエーションID</param>
	/// <returns>商品詳細URL</returns>
	protected string CreateProductDetailUrl(object product, string variationId)
	{
		return ProductCommon.CreateProductDetailUrlUseProductCategoryx(product, variationId, this.BrandId);
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
	public string CreateProductDetailUrl(
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
	/// 商品サブ画像URLの取得
	/// </summary>
	/// <param name="imageHeader">画像ヘッダ</param>
	/// <param name="imageFooter">画像フッタ</param>
	/// <param name="imageNo">サブ画像番号</param>
	/// <returns>サブ画像URL</returns>
	public string CreateProductSubImageUrl(string imageHeader, string imageFooter, int imageNo)
	{
		// HACK:ProductPageProcessへ共通化したい
		// ProductPageの継承先が多すぎるため、共通化の際は影響範囲の精査とテストが必要

		var imageFilePath = new StringBuilder(Constants.PATH_ROOT);
		imageFilePath.AppendFormat(
			"{0}{1}/",
			(imageNo > Constants.PRODUCTSUBIMAGE_DEFAULT_SUB_IMAGE_NO)
				? Constants.PATH_PRODUCTIMAGES
				: Constants.PATH_PRODUCTSUBIMAGES,
			this.ShopId);

		imageFilePath
			.Append(imageHeader)
			.Append((imageNo > Constants.PRODUCTSUBIMAGE_DEFAULT_SUB_IMAGE_NO)
				? string.Empty
				: Constants.PRODUCTSUBIMAGE_FOOTER + imageNo.ToString(Constants.PRODUCTSUBIMAGE_NUMBERFORMAT))
			.Append(imageFooter);

		// ファイルが存在しない場合はNowPrinting画像へのパスを作成
		if (File.Exists(Server.MapPath(imageFilePath.ToString())) == false)
		{
			imageFilePath = new StringBuilder();
			imageFilePath.Append(SmartPhoneUtility.GetSmartPhoneContentsUrl(Constants.PATH_PRODUCTSUBIMAGES + Constants.PRODUCTIMAGE_NOIMAGE_HEADER + imageFooter));
		}

		return EncodeImageUrl(imageFilePath.ToString(), imageHeader);
	}

	/// <summary>
	/// 画像URLエンコード
	/// </summary>
	/// <param name="url">Url</param>
	/// <param name="imageHead">Image head</param>
	/// <returns>A url after encode</returns>
	public static string EncodeImageUrl(string url, string imageHead)
	{
		// HACK:ProductPageProcessへ共通化したい
		// ProductPageの継承先が多すぎるため、共通化の際は影響範囲の精査とテストが必要

		var result = (string.IsNullOrEmpty(imageHead) == false)
			? url.Replace(imageHead, HttpUtility.UrlEncode(imageHead))
			: url;
		return result;
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
		// HACK:ProductPageProcessへ共通化したい
		// ProductPageの継承先が多すぎるため、共通化の際は影響範囲の精査とテストが必要

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
			if (string.IsNullOrEmpty(ProductPrice.GetFixedPurchasePrice(objProduct, true)) != false)
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
	/// サブ画像番号の上限チェック
	/// </summary>
	/// <param name="subImageNo">サブ画像番号</param>
	/// <returns>サブ番号の上限値を超えていないか</returns>
	protected bool IsSubImagesNoLimit(int subImageNo)
	{
		// HACK:ProductPageProcessへ共通化したい
		// ProductPageの継承先が多すぎるため、共通化の際は影響範囲の精査とテストが必要

		return (subImageNo <= Constants.PRODUCTSUBIMAGE_MAXCOUNT);
	}

	/// <summary>
	/// 商品付与ポイント取得
	/// </summary>
	/// <param name="strShopId">店舗ID</param>
	/// <param name="strPointKbn">付与ポイント区分</param>
	/// <param name="dProductPoint">商品ポイント</param>
	/// <param name="productPrice">Product price</param>
	/// <returns>商品加算ポイント</returns>
	/// <remarks>商品が持つポイントを利用するときのみ表示</remarks>
	public string GetProductAddPointString(string strShopId, string strPointKbn, decimal dProductPoint, decimal productPrice)
	{
		return ProductPage.GetProductAddPointString(strShopId, strPointKbn, dProductPoint, productPrice, this.MemberRankId);
	}

	/// <summary>
	/// トラッキングログ送信ページ遷移URL作成
	/// </summary>
	/// <param name="objRecommendItem">レコメンド商品 または レコメンドカテゴリ</param>
	/// <param name="rkRecommendKbn">レコメンド区分</param>
	/// <returns>トラッキングログ送信ページURL</returns>
	protected string CreateSendTrackingLogUrl(object objRecommendItem, Constants.RecommendKbn rkRecommendKbn)
	{
		StringBuilder sbUrl = new StringBuilder();
		sbUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_FRONT_RECOMMEND_SEND_TRACKING_LOG);
		sbUrl.Append("?icd=").Append(HttpUtility.UrlEncode((string)GetKeyValue(objRecommendItem, "item_code")));
		sbUrl.Append("&resid=").Append(HttpUtility.UrlEncode((string)GetKeyValue(objRecommendItem, "response_id")));
		sbUrl.Append("&recdt=").Append(HttpUtility.UrlEncode(StringUtility.ToDateString(DateTime.Now, "yyyy-MM-dd HH:mm:ss")));
		sbUrl.Append("&rnk=").Append(HttpUtility.UrlEncode((string)GetKeyValue(objRecommendItem, "rank")));
		sbUrl.Append("&numrnk=").Append(HttpUtility.UrlEncode((string)GetKeyValue(objRecommendItem, "num_rank")));

		if (rkRecommendKbn == Constants.RecommendKbn.ProductRecommend)
		{
			// 商品レコメンドの場合は商品詳細ページ
			sbUrl.Append("&").Append(Constants.REQUEST_KEY_NEXT_URL).Append("=").Append(HttpUtility.UrlEncode(CreateProductDetailUrl(objRecommendItem)));
		}
		else
		{
			// カテゴリレコメンドの場合は商品一覧ページ
			sbUrl.Append("&").Append(Constants.REQUEST_KEY_NEXT_URL).Append("=").Append(HttpUtility.UrlEncode(CreateCategoryLinkUrl(objRecommendItem)));
		}

		return sbUrl.ToString();
	}

	/// <summary>
	/// ドロップダウンリスト用カテゴリリスト取得
	/// </summary>
	/// <param name="rootCategoryNodes">カテゴリノードリスト</param>
	/// <returns>ドロップダウン表示用ListItemArray</returns>
	protected ListItem[] GetProductCategoryListForDropDownList(List<ProductCategoryTreeNode> rootCategoryNodes)
	{
		ListItemCollection tempCategories = new ListItemCollection();

		string selectedCategory = (this.CategoryId.Length > Constants.CONST_CATEGORY_ID_LENGTH) ? this.CategoryId.Substring(0, Constants.CONST_CATEGORY_ID_LENGTH) : this.CategoryId;

		tempCategories.Add(new ListItem(ReplaceTag("@@DispText.search_category_list.all@@"), ""));
		foreach (ProductCategoryTreeNode pctn in rootCategoryNodes)
		{
			if ((pctn.LowerMemberCanDisplayTreeFlg == Constants.FLG_PRODUCTCATEGORY_LOWER_MEMBER_CAN_DISPLAY_TREE_FLG_VALID)
				|| MemberRankOptionUtility.CheckMemberRankPermission(this.MemberRankId, pctn.MemberRankId))
			{
				ListItem listItem = new ListItem(pctn.CategoryName, pctn.CategoryId);
				listItem.Selected = (listItem.Value.Equals(selectedCategory, StringComparison.OrdinalIgnoreCase));

				tempCategories.Add(listItem);
			}
		}
		ListItem[] categoryList = new ListItem[tempCategories.Count];
		tempCategories.CopyTo(categoryList, 0);
		return categoryList;
	}

	/// <summary>
	/// ドロップダウンリスト用カラーリスト取得
	/// </summary>
	/// <returns>ドロップダウン表示用ListItemArray</returns>
	protected ListItem[] GetColorListForDropDownList()
	{
		var productColors = new[] { new ListItem("", "") }.Concat(
			ProductColorUtility.GetProductColorList().Select(
				color =>
				{
					var item = new ListItem(color.DispName, color.Id);
					if (item.Value == this.ProductColorId) item.Selected = true;
					return item;
				})).ToArray();
		return productColors;
	}

	/// <summary>
	/// 在庫有無の検索パラメータ取得
	/// </summary>
	/// <param name="stockExistenceCheckBox">「在庫あり」チェックボックス</param>
	/// <returns>在庫有無の検索パラメータ</returns>
	protected string GetStockDispKbn(WrappedCheckBox stockExistenceCheckBox)
	{
		if (stockExistenceCheckBox.InnerControl != null)
		{
			if (stockExistenceCheckBox.Checked)
			{
				// 在庫ありチェックがついていれば「在庫ありのみ」
				return Constants.KBN_PRODUCT_LIST_UNDISPLAY_NOSTOCK_PRODUCT_UNDISPLAY_NOSTOCK;
			}
			else
			{
				// 在庫ありチェックがない場合は、「すべて表示」または「すべて表示(在庫あり優先)」。デフォルト値に従う。
				return Constants.DISPLAY_NOSTOCK_PRODUCT_BOTTOM ? Constants.KBN_PRODUCT_LIST_UNDISPLAY_NOSTOCK_PRODUCT_DISPLAY_NOSTOCK_BOTTOM : Constants.KBN_PRODUCT_LIST_UNDISPLAY_NOSTOCK_PRODUCT_DISPLAY_NOSTOCK;
			}
		}
		else
		{
			// 在庫ありチェックボックスがない場合は元のパラメータ引き継ぎ
			return this.UndisplayNostock;
		}
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
		// HACK:ProductPageProcessへ共通化したい
		// ProductPageの継承先が多すぎるため、共通化の際は影響範囲の精査とテストが必要

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
		// HACK:ProductPageProcessへ共通化したい
		// ProductPageの継承先が多すぎるため、共通化の際は影響範囲の精査とテストが必要

		if (this.UserProductArrivalMailInfo.ContainsKey(productId) == false)
		{
			this.UserProductArrivalMailInfo[productId] =
				UserProductArrivalMailCommon.GetUserProductArrivalMailInfo(userId, productId)
					.Cast<DataRowView>().ToArray();
		}
		return this.UserProductArrivalMailInfo[productId];
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
	/// 入荷通知メールを登録済みかどうか
	/// </summary>
	/// <param name="amkbn">入荷通知区分</param>
	/// <param name="vid">バリエーションID</param>
	/// <param name="pid">商品ID</param>
	/// <param name="pcMbKbn">PC/モバイル区分、null指定時はその他アドレス</param>
	/// <returns>登録済みであれば true</returns>
	private bool IsArrivalMailRegistered(string amkbn, string vid, string pid, string pcMbKbn)
	{
		// HACK:ProductPageProcessへ共通化したい
		// ProductPageの継承先が多すぎるため、共通化の際は影響範囲の精査とテストが必要

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
	/// バリエーションがあるか
	/// </summary>
	/// <param name="objProduct">商品情報</param>
	/// <returns>バリエーションがあるか</returns>
	protected bool HasVariation(object objProduct)
	{
		return ProductCommon.HasVariation(objProduct);
	}

	/// <summary>カート商品数</summary>
	protected int CartProductCount
	{
		get
		{
			int iProductCountTotal = 0;
			foreach (CartObject co in GetCartObjectList())
			{
				iProductCountTotal += co.Items.Count;
			}

			return iProductCountTotal;
		}
	}
	/// <summary>カート商品小計</summary>
	protected decimal CartProductPriceSubtotal
	{
		get
		{
			decimal dCartProductPriceSubtotal = 0;
			foreach (CartObject co in GetCartObjectList())
			{
				dCartProductPriceSubtotal += co.PriceSubtotal;
			}

			return dCartProductPriceSubtotal;
		}
	}

	/// <summary>
	/// カテゴリ翻訳情報設定
	/// </summary>
	/// <param name="categoriesData">カテゴリ情報</param>
	/// <returns>翻訳後カテゴリ情報</returns>
	private DataView SetCategoryTranslationData(DataView categoriesData)
	{
		var categories = categoriesData.Cast<DataRowView>().Select(
			drv => new ProductCategoryModel
			{
				CategoryId = ((string)drv[Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID])
			}).ToArray();
		categoriesData = (DataView)NameTranslationCommon.Translate(categoriesData, categories);
		return categoriesData;
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
	public string ShopId
	{
		get { return (ViewState["ShopId"] != null) ? (string)ViewState["ShopId"] : Constants.CONST_DEFAULT_SHOP_ID; }
		set { ViewState["ShopId"] = value; }
	}
	/// <summary>カテゴリID</summary>
	public string CategoryId
	{
		get { return (string)ViewState["CategoryId"]; }
		set { ViewState["CategoryId"] = value; }
	}
	/// <summary>商品ID</summary>
	public string ProductId
	{
		get { return (string)ViewState["ProductId"]; }
		set { ViewState["ProductId"] = value; }
	}

	/// <summary>商品マスタ</summary>
	public DataRowView ProductMaster
	{
		get { return (DataRowView)ViewState["ProductMaster"]; }
		set { ViewState["ProductMaster"] = value; }
	}
	/// <summary>バリエーションID</summary>
	public string VariationId
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
	/// <summary>ソート区分</summary>
	protected string SortKbn
	{
		get { return (string)ViewState["SortKbn"]; }
		set { ViewState["SortKbn"] = value; }
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
	/// <summary>ページ番号</summary>
	protected int PageNumber
	{
		get { return (int)ViewState["PageNumber"]; }
		set { ViewState["PageNumber"] = value; }
	}
	/// <summary>キャンペンアイコン</summary>
	protected string CampaignIcon
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
	/// <summary>在庫無し区分</summary>
	public string UndisplayNostock
	{
		get { return (string)ViewState["UndisplayNostock"]; }
		set { ViewState["UndisplayNostock"] = value; }
	}
	/// <summary>表示件数</summary>
	public int DisplayCount
	{
		get { return (int)ViewState["DisplayCount"]; }
		set { ViewState["DisplayCount"] = value; }
	}
	/// <summary>特別価格有無検索</summary>
	protected string DisplayOnlySpPrice
	{
		get { return (string)ViewState["DisplayOnlySpPrice"]; }
		set { ViewState["DisplayOnlySpPrice"] = value; }
	}
	/// <summary>商品グループID</summary>
	protected string ProductGroupId
	{
		get { return (string)ViewState["ProductGroupId"]; }
		set { ViewState["ProductGroupId"] = value; }
	}
	/// <summary>定期購入フィルタ</summary>
	public string FixedPurchaseFilter
	{
		get { return (string)ViewState["FixedPurchaseFilter"]; }
		set { ViewState["FixedPurchaseFilter"] = value; }
	}
	/// <summary>セール対象フィルタ</summary>
	public string SaleFilter
	{
		get { return (string)ViewState["SaleFiltere"]; }
		set { ViewState["SaleFiltere"] = value; }
	}
	/// <summary>商品カラーID</summary>
	public string ProductColorId
	{
		get { return (string)ViewState["ProductColorId"]; }
		set { ViewState["ProductColorId"] = value; }
	}
	/// <summary>リクエストパラメーター</summary>
	public Dictionary<string, string> RequestParameter
	{
		get { return (Dictionary<string, string>)ViewState["RequestParameter"]; }
		set { ViewState["RequestParameter"] = value; }
	}
	/// <summary>現行の通貨コード</summary>
	public string CurrentCurrencyCode
	{
		get
		{
			// グローバルOPが無効の場合、Configファイルに設定した基軸通貨を戻す
			if (Constants.GLOBAL_OPTION_ENABLE == false) return Constants.CONST_KEY_CURRENCY_CODE;

			// クッキーから取得できない場合、または、非対応通貨の場合はグローバル設定ファイルの基軸通貨を戻す
			var region = RegionManager.GetInstance().Region;
			if ((region == null)
				|| (GlobalConfigUtil.CheckCurrencyPossible(region.CurrencyCode) == false))
			{
				return Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.Code;
			}
			return region.CurrencyCode;
		}
	}
	/// <summary>Has Brands</summary>
	protected bool? HasBrands
	{
		get { return (bool?)ViewState["BrandCount"]; }
		set { ViewState["BrandCount"] = value; }
	}
	/// <summary>Cart list page url</summary>
	public string CartListPageUrl
	{
		get
		{
			var cartListPageUrl = Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST;
			return cartListPageUrl;
		}
	}
	/// <summary>頒布会検索ワード</summary>
	protected string SubscriptionBoxSearchWord
	{
		get { return (string)ViewState["SubscriptionBoxSearchWord"]; }
		set { ViewState["SubscriptionBoxSearchWord"] = value; }
	}
	/// <summary>ユーザーか定期購入可能か</summary>
	protected bool IsUserFixedPurchaseAble { get; set; }
	/// <summary>頒布会のみか</summary>
	protected bool IsSubscriptionBoxOnly { get; set; }
	/// <summary>頒布会可能か</summary>
	protected bool IsSubscriptionBoxValid { get; set; }
	/// <summary>定期会員か</summary>
	protected bool IsFixedPurchaseMember
	{
		get { return (StringUtility.ToEmpty(this.LoginUserFixedPurchaseMemberFlg) == Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_ON); }
	}
	/// <summary>ユーザー入荷通知メール（商品IDがキー）</summary>
	private Dictionary<string, DataRowView[]> UserProductArrivalMailInfo
	{
		get { return m_userProductArrivalMailInfo; }
	}
	private readonly Dictionary<string, DataRowView[]> m_userProductArrivalMailInfo = new Dictionary<string, DataRowView[]>();
}
