/*
=========================================================================================================
  Module      : 商品一覧ビューモデル(ProductInputModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using w2.Cms.Manager.ViewModels.Shared;

namespace w2.Cms.Manager.ViewModels.Coordinate
{
	/// <summary>
	/// 商品一覧ビューモデル
	/// </summary>
	public class ProductInputModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ProductInputModel()
		{
		}

		/// <summary> 利用元のパス </summary>
		public string BaseName { get; set; }
		/// <summary>商品リスト</summary>
		public ProductViewModel[] ProductList { get; set; }
		/// <summary>商品一覧タイトル</summary>
		public string Title { get; set; }
		/// <summary>グループID</summary>
		public string GroupId { get; set; }
		/// <summary>グループ名</summary>
		public string GroupName { get; set; }
	}
}