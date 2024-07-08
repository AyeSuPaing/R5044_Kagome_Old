/*
=========================================================================================================
  Module      : 商品同梱付与商品テーブル入力クラス (ProductBundleItemInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using w2.App.Common.Input;
using w2.App.Common.Order;
using w2.Domain.ProductBundle;

/// <summary>
/// 商品同梱付与商品テーブル入力クラス
/// </summary>
public class ProductBundleItemInput : InputBase<ProductBundleItemModel>
{
	#region コンストラクタ
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public ProductBundleItemInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="bundleItem"></param>
	public ProductBundleItemInput(ProductBundleItemModel bundleItem)
		: this()
	{
		this.GrantProductId = bundleItem.GrantProductId;
		this.GrantProductVariationId = CreateInputGrantProductVariationId(bundleItem.GrantProductId, bundleItem.GrantProductVariationId);
		this.GrantProductCount = bundleItem.GrantProductCount.ToString();
		this.OrderedProductExceptFlg =
			(bundleItem.OrderedProductExceptFlg == Constants.FLG_PRODUCTBUNDLEITEM_ORDERED_PRODUCT_EXCEPT_FLG_BUNDLED_EXCEPT);
	}
	#endregion

	/// <summary>
	/// 同梱商品バリエーションIDから同梱商品IDとの共通部分を削除
	/// </summary>
	/// <param name="grantProductId">同梱商品ID</param>
	/// <param name="grantProductVariationId">同梱商品バリエーションID</param>
	/// <returns>同梱商品IDとの共通部分を除いた同梱商品バリエーションID</returns>
	private string CreateInputGrantProductVariationId(string grantProductId, string grantProductVariationId)
	{
		if ((string.IsNullOrEmpty(grantProductId)) || string.IsNullOrEmpty(grantProductVariationId)) return string.Empty;

		var vid = grantProductVariationId.Substring(grantProductId.Length);
		return vid;
	}

	/// <summary>
	/// モデル生成
	/// </summary>
	/// <returns>モデル</returns>
	public override ProductBundleItemModel CreateModel()
	{
		var bundleItem = new ProductBundleItemModel
		{
			GrantProductId = this.GrantProductId,
			GrantProductVariationId = this.GrantProductId + this.GrantProductVariationId,
			GrantProductCount = int.Parse(this.GrantProductCount),
			OrderedProductExceptFlg = this.OrderedProductExceptFlg
				? Constants.FLG_PRODUCTBUNDLEITEM_ORDERED_PRODUCT_EXCEPT_FLG_BUNDLED_EXCEPT
				: Constants.FLG_PRODUCTBUNDLEITEM_ORDERED_PRODUCT_EXCEPT_FLG_BUNDLED_TARGET
		};
		return bundleItem;
	}

	#region プロパティ
	/// <summary>同梱商品ID</summary>
	public string GrantProductId
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLEITEM_GRANT_PRODUCT_ID]; }
		set { this.DataSource[Constants.FIELD_PRODUCTBUNDLEITEM_GRANT_PRODUCT_ID] = value; }
	}
	/// <summary>同梱商品バリエーションID</summary>
	public string GrantProductVariationId
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLEITEM_GRANT_PRODUCT_VARIATION_ID]; }
		set { this.DataSource[Constants.FIELD_PRODUCTBUNDLEITEM_GRANT_PRODUCT_VARIATION_ID] = value; }
	}
	/// <summary>同梱数量</summary>
	public string GrantProductCount
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLEITEM_GRANT_PRODUCT_COUNT]; }
		set { this.DataSource[Constants.FIELD_PRODUCTBUNDLEITEM_GRANT_PRODUCT_COUNT] = value; }
	}
	/// <summary>初回のみ同梱フラグ</summary>
	public bool OrderedProductExceptFlg
	{
		get { return (bool)this.DataSource[Constants.FIELD_PRODUCTBUNDLEITEM_ORDERED_PRODUCT_EXCEPT_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCTBUNDLEITEM_ORDERED_PRODUCT_EXCEPT_FLG] = value; }
	}
	#endregion
}