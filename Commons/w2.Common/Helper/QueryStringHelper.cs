/*
=========================================================================================================
  Module      : クエリ文字列ヘルパークラス (QueryStringHelper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using w2.Common.Helper.Attribute;

namespace w2.Common.Helper
{
	/// <summary>
	/// クエリ文字列ヘルパークラス
	/// </summary>
	public class QueryStringHelper
	{
		/// <summary>
		/// クエリ文字列の生成
		/// </summary>
		/// <typeparam name="T">型</typeparam>
		/// <param name="obj">オブジェクト</param>
		/// <returns>クエリ文字列</returns>
		public static string GenerateQueryString<T>(T obj)
		{
			var parameters = GetQueryStringVariables(obj);
			var queryString = string.Join("&", parameters.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}")); // HttpUtility.UrlEncode を使用しない。space を "%20" とエンコードしたいため。HttpUtilityだと、space は "+" となる
			return queryString;
		}
		
		/// <summary>
		/// クエリ文字列の変数群を取得
		/// </summary>
		/// <typeparam name="T">型</typeparam>
		/// <param name="obj">オブジェk津尾</param>
		/// <returns>クエリ文字列変数群</returns>
		private static Dictionary<string, string> GetQueryStringVariables<T>(T obj)
		{
			var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
			var parameters = properties
				.ToDictionary(
					p =>
					{
						var attributes = p.GetCustomAttributes(typeof(QueryStringVariableName), false);
						if (attributes.Any() == false) return "";
						var attribute = attributes.First();
						return ((QueryStringVariableName)attribute).PropertyName;
					},
					p => (string)p.GetValue(obj))
				.Where(p => string.IsNullOrEmpty(p.Value) == false)
				.ToDictionary(p => p.Key, p => p.Value);
			return parameters;
		}
	}
}
