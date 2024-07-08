/*
=========================================================================================================
  Module      : BindAliasAttribute(BindAliasAttribute.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;

namespace w2.Cms.Manager.Codes.Binder
{
	/// <summary>
	/// Allows you to create aliases that can be used for model properties at
	/// model binding time (i.e. when data comes in from a request).
	/// 
	/// The type needs to be using the DefaultModelBinderEx model binder in 
	/// order for this to work.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
	public class BindAliasAttribute : Attribute
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="alias">エイリアス</param>
		public BindAliasAttribute(string alias)
		{
			//ommitted: parameter checking
			this.Alias = alias;
		}

		/// <summary>エイリアス</summary>
		public string Alias { get; private set; }
	}
}