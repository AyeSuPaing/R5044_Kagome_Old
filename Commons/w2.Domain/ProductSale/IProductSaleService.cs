/*
=========================================================================================================
  Module      : Product Sale Service Interface (IProductSaleService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;

namespace w2.Domain.ProductSale
{
	/// <summary>
	/// Product sale service interface
	/// </summary>
	public interface IProductSaleService : IService
	{
		/// <summary>
		/// Get product sale count
		/// </summary>
		/// <param name="productSale">Product sale</param>
		/// <returns>Number</returns>
		int GetProductSaleCount(Hashtable productSale);

		/// <summary>
		/// Get product sale list
		/// </summary>
		/// <param name="productSale">Product sale</param>
		/// <returns>Data product sale</returns>
		DataView GetProductSaleList(Hashtable productSale);
	}
}
