/*
=========================================================================================================
  Module      : クエリ文字列名属性 (QueryStringVariableName.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Common.Helper.Attribute
{
	/// <summary>
	/// クエリ文字列名
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class QueryStringVariableName : System.Attribute
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="variableName">テキスト名</param>
		public QueryStringVariableName(string variableName)
		{
			this.PropertyName = variableName;
		}

		/// <summary>テキスト名</summary>
		public string PropertyName { get; }
	}
}
