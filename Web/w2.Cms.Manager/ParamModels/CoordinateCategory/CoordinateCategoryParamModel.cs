/*
=========================================================================================================
  Module      : コーディネートカテゴリリストパラメタモデル(CoordinateCategoryParamModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Binder;

namespace w2.Cms.Manager.ParamModels.CoordinateCategory
{
	/// <summary>
	/// コーディネートカテゴリリストパラメタモデル
	/// </summary>
	public class CoordinateCategoryParamModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CoordinateCategoryParamModel()
		{
			this.PagerNo = 1;
			this.PageLayout = Constants.LAYOUT_PATH_DEFAULT;
		}

		/// <summary>検索カテゴリーID</summary>
		public string SearchCoordinateCategoryId { get; set; }
		/// <summary>検索親カテゴリーID</summary>
		public string SearchCoordinateParentCategoryId { get; set; }
		/// <summary>カテゴリーId</summary>
		public string CoordinateCategoryId { get; set; }
		/// <summary>親カテゴリーId</summary>
		public string CoordinateParentCategoryId { get; set; }
		/// <summary>更新か</summary>
		public bool IsUpdate { get; set; }
		/// <summary>ページレイアウト</summary>
		public string PageLayout { get; set; }
		/// <summary>ページ番号</summary>
		[BindAlias(Constants.REQUEST_KEY_PAGE_NO)]
		public int PagerNo { get; set; }
		/// <summary>選択されたインデックス</summary>
		public int SelectedIndex { get; set; }
		/// <summary>データ出力タイプ</summary>
		public string DataExportType { get; set; }
	}
}