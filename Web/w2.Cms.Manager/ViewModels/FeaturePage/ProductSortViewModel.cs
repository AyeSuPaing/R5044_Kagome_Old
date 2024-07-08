/*
=========================================================================================================
  Module      : 商品並び替えモデル (ProductSortViewModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

using w2.Cms.Manager.ViewModels.Shared;

namespace w2.Cms.Manager.ViewModels.FeaturePage
{
	/// <summary>
	/// 商品並び替えモデル
	/// </summary>
	public class ProductSortViewModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ProductSortViewModel()
		{
		}

		/// <summary> 利用元のパス </summary>
		public string BaseName { get; set; }
		/// <summary>商品リスト</summary>
		public ProductViewModel[] ProductList { get; set; }
	}
}