/*
=========================================================================================================
  Module      : NameValueCollection拡張メソッド (NameValueCollectionExtensions.cs)
･････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace w2.Common.Extensions
{
	/// <summary>
	/// NameValueCollection拡張メソッド
	/// </summary>
	public static class NameValueCollectionExtensions
	{
		/// <summary>
		/// KeyValuePairに変換
		/// </summary>
		/// <param name="collection">NameValueCollection</param>
		/// <returns>KeyValuePair</returns>
		public static IEnumerable<KeyValuePair<string, string>> ToPairs(this NameValueCollection collection)
		{
			if (collection == null) throw new ArgumentNullException();

			var result = collection.Cast<string>()
				.Select(key => new KeyValuePair<string, string>(key, collection[key]));
			return result;
		}
	}
}
