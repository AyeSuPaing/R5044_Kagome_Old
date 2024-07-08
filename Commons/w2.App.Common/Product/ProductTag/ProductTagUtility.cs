/*
=========================================================================================================
  Module      : 商品タグ情報ユーティリティクラス(ProductTagUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using w2.Domain;
using w2.Domain.ProductTag;

namespace w2.App.Common.Product
{
	public class ProductTagUtility
	{
		/// <summary>
		/// 商品タグ情報取得
		/// </summary>
		/// <param name="productId">商品ID</param>
		/// <returns>商品タグ情報</returns>
		public static ProductTagModel GetProductTag(string productId)
		{
			var productTag = DomainFacade.Instance.ProductTagService.GetProductTag(productId);
			return productTag;
		}

		/// <summary>
		/// 商品タグ情報取得
		/// </summary>
		public static Hashtable GetProductTagData(string productId)
		{
			var productTag = GetProductTag(productId);
			return (productTag != null)
				? productTag.DataSource
				: new Hashtable();
		}

		/// <summary>
		/// 有効な商品タグ設定取得
		/// </summary>
		/// <returns>商品タグ設定</returns>
		public static ProductTagSettingModel[] GetProductTagSetting()
		{
			var productTagSetting = DomainFacade.Instance.ProductTagService.GetProductTagSetting();
			return productTagSetting;
		}
	}
}
