/*
=========================================================================================================
  Module      : 商品ヘルパークラス(ProductHelper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Data;

namespace w2.Domain.Product.Helper
{
	/// <summary>
	/// 商品ヘルパークラス
	/// </summary>
	public class ProductHelper
	{
		/// <summary>
		/// 商品バリエーション件数情報のDataViewをDictionaryに変換
		/// </summary>
		/// <param name="dataView">商品バリエーション件数情報を含むDataView</param>
		/// <returns>商品バリエーション件数情報</returns>
		public static Dictionary<string, int> ConvertDataViewToDictionaryForProductVariationCounts(DataView dataView)
		{
			var dictionary = new Dictionary<string, int>();
			foreach (DataRowView row in dataView)
			{
				var productId = (string)row.Row.ItemArray[0];
				var count = (int)row.Row.ItemArray[1];
				dictionary[productId] = count;
			}
			return dictionary;
		}
	}
}
