/*
=========================================================================================================
  Module      : コーディネートカテゴリリストパラメタモデル(CoordinateCategoryParamModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Binder;

namespace w2.Cms.Manager.ParamModels.Coordinate
{
	/// <summary>
	/// コーディネートカテゴリリストパラメタモデル
	/// </summary>
	public class CoordinateParamModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CoordinateParamModel()
		{
			this.PagerNo = 1;
		}

		/// <summary>検索キーワード</summary>
		public string SearchKeyword { get; set; }
		/// <summary>スタッフ</summary>
		public string SearchStaff { get; set; }
		/// <summary>リアルショップ</summary>
		public string SearchRealShop { get; set; }
		/// <summary>カテゴリ</summary>
		public string SearchCategory { get; set; }
		/// <summary>公開日区分</summary>
		public string DisplayDateKbn { get; set; }
		/// <summary>ページ番号</summary>
		[BindAlias(Constants.REQUEST_KEY_PAGE_NO)]
		public int PagerNo { get; set; }
		/// <summary>データ出力タイプ</summary>
		public string DataExportType { get; set; }
	}
}