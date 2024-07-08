/*
=========================================================================================================
  Module      : 商品セットサービスのインターフェース (IProductSetService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.ProductSet
{
	/// <summary>
	/// 商品セットサービスのインターフェース
	/// </summary>
	public interface IProductSetService : IService
	{
		/// <summary>
		/// 商品セットアイテム取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productSetId">商品セットID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">ヴァリエーションID</param>
		/// <returns>商品セットアイテム</returns>
		ProductSetItemModel GetProductSetItem(string shopId, string productSetId, string productId, string variationId);
	}
}