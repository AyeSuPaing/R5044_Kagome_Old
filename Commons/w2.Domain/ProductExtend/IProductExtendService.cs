/*
=========================================================================================================
  Module      : 商品拡張拡張項目サービスのインターフェース (IProductExtendService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Sql;

namespace w2.Domain.ProductExtend
{
	/// <summary>
	/// 商品拡張拡張項目サービスのインターフェース
	/// </summary>
	public interface IProductExtendService : IService
	{
		/// <summary>
		/// Insert
		/// </summary>
		/// <param name="model">Model</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Number of rows affected</returns>
		int Insert(ProductExtendModel model, SqlAccessor accessor = null);

		/// <summary>
		/// Delete
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="productId">Product ID</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Number of rows affected</returns>
		int Delete(string shopId, string productId, SqlAccessor accessor = null);
	}
}