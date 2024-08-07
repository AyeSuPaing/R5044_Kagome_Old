/*
=========================================================================================================
  Module      : 商品セットサービス (ProductSetService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.ProductSet
{
	/// <summary>
	/// 商品セットサービス
	/// </summary>
	public class ProductSetService : ServiceBase, IProductSetService
	{
		#region +GetProductSetItem 商品セットアイテム取得
		/// <summary>
		/// 商品セットアイテム取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productSetId">商品セットID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">ヴァリエーションID</param>
		/// <returns>商品セットアイテム</returns>
		public ProductSetItemModel GetProductSetItem(
			string shopId,
			string productSetId,
			string productId,
			string variationId)
		{
			using (var repository = new ProductSetRepository())
			{
				var model = repository.GetProductSetItem(
					shopId,
					productSetId,
					productId,
					variationId);
				return model;
			}
		}
		#endregion
	}
}
