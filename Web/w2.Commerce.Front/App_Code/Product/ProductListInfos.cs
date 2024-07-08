/*
=========================================================================================================
  Module      : 商品一覧情報クラス(ProductListInfos.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// ProductListInfos の概要の説明です
/// </summary>
/// <summary>
/// 商品一覧情報クラス
/// </summary>
public class ProductListInfos
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="products">商品リスト</param>
	/// <param name="productVariations">商品バリエーションリスト</param>
	/// <param name="productGroupedVariations">商品グループ化バリエーションリスト（サムネイル画像に利用）</param>
	/// <param name="childCategories">商品小カテゴリリスト（サムネイル画像に利用）</param>
	public ProductListInfos(
		DataView products,
		DataView productVariations,
		Dictionary<string, List<DataRowView>> productGroupedVariations,
		List<DataRowView> childCategories)
	{
		this.Products = products;
		this.ProductVariations = productVariations;
		this.ProductGroupedVariations = productGroupedVariations;
		this.ChildCategories = childCategories;
	}

	/// <summary>
	/// 商品一覧翻訳情報設定
	/// </summary>
	/// <param name="productTranslation">翻訳後商品リスト</param>
	/// <param name="productVariationTranslation">翻訳後商品バリエーションリスト</param>
	public void SetProductListTranslationData(
		DataView productTranslation,
		DataView productVariationTranslation)
	{
		this.Products = productTranslation;
		this.ProductVariations = productVariationTranslation;
	}

	/// <summary>商品リスト</summary>
	public DataView Products { get; private set; }
	/// <summary>商品バリエーションリスト</summary>
	public DataView ProductVariations { get; private set; }
	/// <summary>商品グループ化バリエーションリスト（サムネイル画像に利用）</summary>
	public Dictionary<string, List<DataRowView>> ProductGroupedVariations { get; private set; }
	/// <summary>商品小カテゴリリスト（サムネイル画像に利用）</summary>
	public List<DataRowView> ChildCategories { get; private set; }
}