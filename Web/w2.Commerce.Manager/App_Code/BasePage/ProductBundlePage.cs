/*
=========================================================================================================
  Module      : 商品同梱共通ページ (ProductBundlePage.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using w2.Common.Web;
using w2.Domain.ProductBundle;

/// <summary>
/// 商品同梱共通ページ
/// </summary>
public class ProductBundlePage : BasePage
{
	/// <summary>ページ番号既定値</summary>
	protected const int DEFAULT_PAGE_NO = 1;

	/// <summary>
	/// 商品同梱設定一覧ページの検索用パラメタつきURLを生成
	/// </summary>
	/// <param name="parameters">検索パラメタ</param>
	/// <param name="isAddPageNo">パラメタにページをつけるか</param>
	/// <param name="pageNo">ページ番号(任意の値をセットする場合)</param>
	/// <returns>商品同梱設定一覧ページ遷移URL</returns>
	protected string CreateProductBundleListUrl(Hashtable parameters, bool isAddPageNo, int? pageNo = null)
	{
		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTBUNDLE_LIST)
			.AddParam(Constants.REQUEST_KEY_PRODUCTBUNDLE_PRODUCT_BUNDLE_ID, (string)parameters[Constants.REQUEST_KEY_PRODUCTBUNDLE_PRODUCT_BUNDLE_ID])
			.AddParam(Constants.REQUEST_KEY_PRODUCTBUNDLE_PRODUCT_BUNDLE_NAME, (string)parameters[Constants.REQUEST_KEY_PRODUCTBUNDLE_PRODUCT_BUNDLE_NAME])
			.AddParam(Constants.REQUEST_KEY_PRODUCTBUNDLE_TARGET_ORDER_TYPE, (string)parameters[Constants.REQUEST_KEY_PRODUCTBUNDLE_TARGET_ORDER_TYPE])
			.AddParam(Constants.REQUEST_KEY_SORT_KBN, (string)parameters[Constants.REQUEST_KEY_SORT_KBN])
			.AddParam(Constants.REQUEST_KEY_PRODUCTBUNDLE_TARGET_PRODUCT_ID, (string)parameters[Constants.REQUEST_KEY_PRODUCTBUNDLE_TARGET_PRODUCT_ID])
			.AddParam(Constants.REQUEST_KEY_PRODUCTBUNDLE_BUNDLE_PRODUCT_ID, (string)parameters[Constants.REQUEST_KEY_PRODUCTBUNDLE_BUNDLE_PRODUCT_ID]);
		if (isAddPageNo) urlCreator.AddParam(Constants.REQUEST_KEY_PAGE_NO, pageNo.HasValue 
			? pageNo.Value.ToString()
			: parameters[Constants.REQUEST_KEY_PAGE_NO].ToString());

		return urlCreator.CreateUrl();
	}

	/// <summary>
	/// 商品同梱設定編集ページへのURLを生成
	/// </summary>
	/// <param name="actionStatus">処理ステータス</param>
	/// <param name="productBundleId">商品同梱ID</param>
	/// <returns>商品同梱設定編集ページのURL</returns>
	protected string CreateProductBundleRegister(string actionStatus, string productBundleId)
	{
		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTBUNDLE_REGISTER);
		if (actionStatus != Constants.ACTION_STATUS_INSERT)
		{
			urlCreator.AddParam(Constants.REQUEST_KEY_PRODUCTBUNDLE_PRODUCT_BUNDLE_ID, productBundleId);
		}
		urlCreator.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, actionStatus);

		return urlCreator.CreateUrl();
	}
}