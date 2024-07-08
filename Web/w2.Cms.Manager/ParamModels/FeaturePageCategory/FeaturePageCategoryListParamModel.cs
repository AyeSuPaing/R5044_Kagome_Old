/*
=========================================================================================================
  Module      : 特集ページカテゴリ一覧パラメタモデル(FeaturePageCategoryListParamModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Binder;

namespace w2.Cms.Manager.ParamModels.FeaturePageCategory
{
	/// <summary>
	/// 特集ページカテゴリ一覧リストパラメタモデル
	/// </summary>
	[Serializable]
	public class FeaturePageCategoryListParamModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FeaturePageCategoryListParamModel()
		{
			this.CategoryId = string.Empty;
			this.ParentCategoryId = string.Empty;
			this.CategoryName = string.Empty;
			this.ValidFlg = "1";
			this.DelFlg = "0";
			this.DisplayOrder = string.Empty;
			this.CategoryOutline = string.Empty;
			this.PagerNo = 1;
		}

		/// <summary>並び順</summary>
		public string SortKbn { get; set; }
		/// <summary>検索用カテゴリID</summary>
		public string SortCategoryId { get; set; }
		/// <summary>ページ番号</summary>
		[BindAlias(Constants.REQUEST_KEY_PAGE_NO)]
		public int PagerNo { get; set; }
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
		/// <summary>カテゴリ詳細か</summary>
		public bool IsCategoryDetail { get; set; }
		/// <summary>変更前カテゴリID</summary>
		public string BeforeCategoryId { get; set; }
	}
}