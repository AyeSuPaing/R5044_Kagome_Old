/*
=========================================================================================================
  Module      : 特集ページ一覧ビューモデル(ListViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using System.Web.Mvc;
using w2.Cms.Manager.ParamModels.FeaturePageCategory;
using w2.Domain.FeaturePageCategory;

namespace w2.Cms.Manager.ViewModels.FeaturePageCategory
{
	/// <summary>
	/// 特集ページカテゴリ一覧ビューモデル
	/// </summary>
	public class ListViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ListViewModel()
		{
			this.List = new SearchResultViewModel[0];
			var defaultList = new[]
				{
				new SelectListItem
				{
					Value = string.Empty,
					Text = string.Empty
				}
			};

			this.SortKbnItems = defaultList.Concat(
				new FeaturePageCategoryService()
					.GetRootCategoryItem()
					.Select(
					s => new SelectListItem
					{
						Value = s.CategoryName,
						Text = s.CategoryName
					})).ToArray();
		}

		/// <summary>パラメタモデル</summary>
		public FeaturePageCategoryListParamModel ParamModel { get; set; }
		/// <summary>検索結果 Viewモデル</summary>
		public SearchResultViewModel[] List { get; set; }
		/// <summary>選択肢群 並び順</summary>
		public SelectListItem[] SortKbnItems { get; private set; }
		/// <summary>選択肢群 有効</summary>
		public SelectListItem[] ValidItems { get; private set; }
		/// <summary>ページャHTML</summary>
		public string PagerHtml { get; set; }
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessage { get; set; }
		/// <summary>店舗ID</summary>
		public string ShopId { get; set; }
		/// <summary>カテゴリID</summary>
		public string CategoryId { get; set; }
		/// <summary>親カテゴリID</summary>
		public string ParentCategoryId { get; set; }
		/// <summary>カテゴリ名</summary>
		public string CategoryName { get; set; }
		/// <summary>有効フラグ</summary>
		public string ValidFlg { get; set; }
		/// <summary>削除フラグ</summary>
		public string DelFlg { get; set; }
		/// <summary>表示順</summary>
		public string DisplayOrder { get; set; }
		/// <summary>カテゴリ説明文</summary>
		public string CategoryOutline { get; set; }
	}

	/// <summary>
	/// 検索結果 Viewモデル
	/// </summary>
	public class SearchResultViewModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SearchResultViewModel()
		{
			this.CategoryId = string.Empty;
			this.ParentCategoryId = string.Empty;
			this.CategoryName = string.Empty;
			this.ValidFlg = string.Empty;
			this.DisplayOrder = 0;
			this.CategoryOutline = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="searchResult">検索結果</param>
		public SearchResultViewModel(FeaturePageCategoryModel searchResult)
			: this()
		{
			this.CategoryId = searchResult.CategoryId;
			this.ParentCategoryId = searchResult.ParentCategoryId;
			this.CategoryName = searchResult.CategoryName;
			this.ValidFlg = searchResult.ValidFlg;
			this.DisplayOrder = searchResult.DisplayOrder;
			this.CategoryOutline = searchResult.CategoryOutline;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="searchResult">検索結果</param>
		public SearchResultViewModel(FeaturePageCategoryModel[] searchResult)
			: this()
		{
		}

		/// <summary>店舗ID</summary>
		public string ShopId { get; set; }
		/// <summary>カテゴリID</summary>
		public string CategoryId { get; set; }
		/// <summary>親カテゴリID</summary>
		public string ParentCategoryId { get; set; }
		/// <summary>カテゴリ名</summary>
		public string CategoryName { get; set; }
		/// <summary>有効フラグ</summary>
		public string ValidFlg { get; set; }
		/// <summary>表示順</summary>
		public int DisplayOrder { get; set; }
		/// <summary>カテゴリ説明文</summary>
		public string CategoryOutline { get; set; }
	}
}