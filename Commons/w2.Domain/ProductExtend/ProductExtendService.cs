/*
=========================================================================================================
  Module      : 商品拡張拡張項目サービス (ProductExtendService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Sql;

namespace w2.Domain.ProductExtend
{
	/// <summary>
	/// 商品拡張拡張項目サービス
	/// </summary>
	public class ProductExtendService : ServiceBase, IProductExtendService
	{
		#region +Insert
		/// <summary>
		/// Insert
		/// </summary>
		/// <param name="model">Model</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Number of rows affected</returns>
		public int Insert(ProductExtendModel model, SqlAccessor accessor = null)
		{
			using (var repository = new ProductExtendRepository(accessor))
			{
				var result = repository.Insert(model);
				return result;
			}
		}
		#endregion

		#region +Delete
		/// <summary>
		/// Delete
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="productId">Product ID</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Number of rows affected</returns>
		public int Delete(string shopId, string productId, SqlAccessor accessor = null)
		{
			using (var repository = new ProductExtendRepository(accessor))
			{
				var result = repository.Delete(shopId, productId);
				return result;
			}
		}
		#endregion
	}
}