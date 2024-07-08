/*
=========================================================================================================
  Module      : カテゴリービューモデル(CategoryListViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Domain.Coordinate;
using w2.Domain.CoordinateCategory;

namespace w2.Cms.Manager.ViewModels.Coordinate
{
	/// <summary>
	/// コーディネートカテゴリビューモデル
	/// </summary>
	[Serializable]
	public class CategoryViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CategoryViewModel(string coordinateId, string shopId)
		{
			var model = new CoordinateService().GetWithChilds(coordinateId, shopId);
			if (model == null) return;

			// カテゴリ
			var categoryList = new CoordinateCategoryService().GetAll();
			if (categoryList != null)
			{
				foreach (var category in categoryList)
				{
					if (string.IsNullOrEmpty(category.CoordinateCategoryName) == false) this.Categories += category.CoordinateCategoryName + ',';
				}
			}

			foreach (var category in model.CategoryList)
			{
				if (string.IsNullOrEmpty(category.CoordinateCategoryName) == false) this.CoordinateCategoryNames += category.CoordinateCategoryName + ',';
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CategoryViewModel(string categories)
		{
			// カテゴリ
			var categoryList = new CoordinateCategoryService().GetAll();
			if (categoryList != null)
			{
				foreach (var category in categoryList)
				{
					if (string.IsNullOrEmpty(category.CoordinateCategoryName) == false) this.Categories += category.CoordinateCategoryName + ',';
				}
			}

			this.CoordinateCategoryNames = categories;
		}

		/// <summary>カテゴリリスト（カンマ区切り）</summary>
		public string Categories { get; set; }
		/// <summary>カテゴリ名（カンマ区切り）</summary>
		public string CoordinateCategoryNames { get; set; }
	}
}