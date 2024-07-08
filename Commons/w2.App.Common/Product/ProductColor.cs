/*
=========================================================================================================
  Module      : 商品カラークラス(ProductColor.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.App.Common.Product
{
	/// <summary>
	/// 商品カラークラス
	/// </summary>
	[Serializable]
	public class ProductColor
	{
		/// <summary>商品カラーID</summary>
		public string Id { get; set; }
		/// <summary>表示名</summary>
		public string DispName { get; set; }
		/// <summary>ファイル名</summary>
		public string Filaname { get; set; }
		/// <summary>URL</summary>
		public string Url { get; set; }
	}
}
