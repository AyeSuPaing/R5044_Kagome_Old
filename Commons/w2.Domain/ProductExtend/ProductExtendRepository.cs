/*
=========================================================================================================
  Module      : 商品価格リポジトリ (ProductExtendRepository.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using w2.Common.Sql;

namespace w2.Domain.ProductExtend
{
	/// <summary>
	/// 商品価格リポジトリ
	/// </summary>
	internal class ProductExtendRepository : RepositoryBase
	{
		/// <summary>SQLファイル</summary>
		private const string XML_KEY_NAME = "ProductExtend";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal ProductExtendRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		public ProductExtendRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +Insert
		/// <summary>
		/// Insert
		/// </summary>
		/// <param name="model">Model</param>
		/// <returns>Number of rows affected</returns>
		internal int Insert(ProductExtendModel model)
		{
			var result = Exec(XML_KEY_NAME, "Insert", model.DataSource);
			return result;
		}
		#endregion

		#region +Delete
		/// <summary>
		/// Delete
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="productId">Product ID</param>
		/// <returns>Number of rows affected</returns>
		internal int Delete(string shopId, string productId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_PRODUCTEXTEND_SHOP_ID, shopId },
				{ Constants.FIELD_PRODUCTEXTEND_PRODUCT_ID, productId },
			};

			var result = Exec(XML_KEY_NAME, "Delete", input);
			return result;
		}
		#endregion
	}
}