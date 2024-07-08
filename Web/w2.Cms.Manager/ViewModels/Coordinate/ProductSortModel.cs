/*
=========================================================================================================
  Module      : 商品並び替えモデル(ProductSortModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using w2.Cms.Manager.ViewModels.Shared;

namespace w2.Cms.Manager.ViewModels.Coordinate
{
	/// <summary>
	/// 商品並び替えモデル
	/// </summary>
	public class ProductSortModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ProductSortModel()
		{
		}

		/// <summary> 利用元のパス </summary>
		public string BaseName { get; set; }
		/// <summary>商品リスト</summary>
		public ProductViewModel[] ProductList { get; set; }
	}
}