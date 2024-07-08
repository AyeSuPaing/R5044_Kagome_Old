/*
=========================================================================================================
  Module      : 商品セットリポジトリ (ProductSetRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using w2.Common.Sql;

namespace w2.Domain.ProductSet
{
	/// <summary>
	/// 商品セットリポジトリ
	/// </summary>
	internal class ProductSetRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "ProductSet";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal ProductSetRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		internal ProductSetRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~GetProductSetItem 商品セットアイテム取得
		/// <summary>
		/// 商品セットアイテム取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productSetId">商品セットID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">ヴァリエーションID</param>
		/// <returns>商品セットアイテム</returns>
		internal ProductSetItemModel GetProductSetItem(string shopId, string productSetId, string productId, string variationId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_PRODUCTSETITEM_SHOP_ID, shopId },
				{ Constants.FIELD_PRODUCTSETITEM_PRODUCT_SET_ID, productSetId },
				{ Constants.FIELD_PRODUCTSETITEM_PRODUCT_ID, productId },
				{ Constants.FIELD_PRODUCTSETITEM_VARIATION_ID, variationId }
			};
			var dv = Get(XML_KEY_NAME, "GetProductSetItem", ht);
			return (dv.Count > 0) ? new ProductSetItemModel(dv[0]) : null;
		}
		#endregion
	}
}
