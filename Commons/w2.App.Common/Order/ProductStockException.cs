/*
=========================================================================================================
  Module      : 商品在庫チェック例外(ProductStockException.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.App.Common.Order
{
	/// <summary>
	/// 商品在庫チェック例外
	/// </summary>
	public class ProductStockException : Exception
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="productName">商品名</param>
		public ProductStockException(string productName)
		{
			this.ProductName = productName;
		}
		/// <summary>
		/// コンストラクタ（複数商品対応）
		/// </summary>
		/// <param name="productNames">商品名</param>
		public ProductStockException(string[] productNames)
		{
			this.ProductNames = productNames;
		}

		/// <summary>商品名</summary>
		public string ProductName { get; private set; }
		/// <summary>商品名（複数）</summary>
		public string[] ProductNames { get; private set; }
	}
}
