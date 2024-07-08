/*
=========================================================================================================
  Module      : 商品ビューモデル(ProductViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Web.Mvc;

namespace w2.Cms.Manager.ViewModels.Shared
{
	/// <summary>
	/// 商品ビューモデル
	/// </summary>
	public class ProductViewModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ProductViewModel()
		{
		}

		/// <summary>商品ID</summary>
		public string Id { get; set; }
		/// <summary>バリエーションID</summary>
		public string VariationId { get; set; }
		/// <summary>バリエーションIDリスト</summary>
		public List<SelectListItem> VariationList { get; set; }
		/// <summary>商品名</summary>
		public string Name { get; set; }
		/// <summary>画像パス</summary>
		public string ImagePath { get; set; }
		/// <summary>在庫数</summary>
		public string Stock { get; set; }
		/// <summary>表示順</summary>
		public int SortNo { get; set; }
	}
}