/*
=========================================================================================================
  Module      : 商品ページプロセス(ProductPageProcess.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Data;
using System.Web;
using System.Web.UI;
using ProductListDispSetting;
using w2.App.Common.Order;

/// <summary>
/// ProductPageProcess の概要の説明です
/// </summary>
public partial class ProductPageProcess : BasePageProcess
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="caller">呼び出し元</param>
	/// <param name="viewState">ビューステート</param>
	/// <param name="context">コンテキスト</param>
	public ProductPageProcess(object caller, StateBag viewState, HttpContext context)
		: base(caller, viewState, context)
	{
	}

	/// <summary>
	/// ページ初期化
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected new void Page_Init(object sender, EventArgs e)
	{
		// 基底メソッドコール
		base.Page_Init(sender, e);

		// 商品一覧表示時のデフォルトソート順をDBから取得
		Constants.KBN_SORT_PRODUCT_LIST_DEFAULT = ProductListDispSettingUtility.SortDefault;
	}

	/// <summary>
	/// 商品データ取得（表示条件考慮しない）
	/// </summary>
	/// <param name="shopId">店舗ID</param>
	/// <param name="productId">商品ID</param>
	/// <param name="variationId">バリエーションID(なしの場合、商品ID)</param>
	/// <returns>商品データ</returns>
	public DataView GetProduct(string shopId, string productId, string variationId)
	{
		return ProductCommon.GetProductVariationInfo(shopId, productId, variationId, this.MemberRankId);
	}

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
		return ProductCommon.CreateProductDetailUrl(
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

	/// <summary>モーダル表示用商品ID</summary>
	public string ProductIdForModal
	{
		get { return (string)Session[Constants.SESSION_KEY_PRODUCT_LIST_PRODUCT_ID_FOR_MODAL]; }
		set { Session[Constants.SESSION_KEY_PRODUCT_LIST_PRODUCT_ID_FOR_MODAL] = value; }
	}
}
