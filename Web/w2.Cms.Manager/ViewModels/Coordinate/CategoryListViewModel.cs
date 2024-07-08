/*
=========================================================================================================
  Module      : カテゴリーリストビューモデル(CategoryListViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using w2.Domain.CoordinateCategory.Helper;

namespace w2.Cms.Manager.ViewModels.Coordinate
{
	/// <summary>
	/// コーディネートカテゴリビューモデル
	/// </summary>
	[Serializable]
	public class CategoryListViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CategoryListViewModel()
		{
			this.List = new List<CoordinateCategoryListSearchResult>();
		}

		/// <summary>リスト一覧</summary>
		public List<CoordinateCategoryListSearchResult> List { get; set; }
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessage { get; set; }
	}
}