/*
=========================================================================================================
  Module      : お知らせ一覧リストパラメタモデル(NewsListParamModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Binder;
using w2.Cms.Manager.Codes.Cms;

namespace w2.Cms.Manager.ParamModels.News
{
	/// <summary>
	/// お知らせ一覧リストパラメタモデル
	/// </summary>
	[Serializable]
	public class NewsListParamModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public NewsListParamModel()
		{
			this.PagerNo = 1;
			this.NewsText = string.Empty;
			this.SortKbn = "2";
			this.Valid = string.Empty;
			this.Disp = string.Empty;
			this.Dates = new SearchParamDateModel();
		}

		/// <summary>本文</summary>
		public string NewsText { get; set; }
		/// <summary>並び順</summary>
		public string SortKbn { get; set; }
		/// <summary>有効</summary>
		public string Valid { get; set; }
		/// <summary>Top表示</summary>
		public string Disp { get; set; }
		/// <summary>検索パラメータ日付</summary>
		public SearchParamDateModel Dates { get; set; }
		/// <summary>ページ番号</summary>
		[BindAlias(Constants.REQUEST_KEY_PAGE_NO)]
		public int PagerNo { get; set; }
	}
}