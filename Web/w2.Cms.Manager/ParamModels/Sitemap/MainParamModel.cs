/*
=========================================================================================================
 Module      : Mainパラメータモデル(MainParamModel)
･･･････････････････････････････････････････････････････････････････････････････････････････････････････
 Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Binder;
using w2.Cms.Manager.Codes.Sitemap;

namespace w2.Cms.Manager.ParamModels.Sitemap
{
	/// <summary>
	/// Mainパラメータモデル
	/// </summary>
	[Serializable]
	public class MainParamModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MainParamModel()
		{
			// 初期表示時の検索条件
			this.Keyword = string.Empty;
			this.PageType = PageTypeEnum.None;
			this.ForPcPage = true;
			this.ForSpPage = Constants.SMARTPHONE_OPTION_ENABLED;
			this.PagerNo = 1;
		}

		/// <summary>検索ワード</summary>
		public string Keyword { get; set; }
		/// <summary>PCページを含むか</summary>
		public bool ForPcPage { get; set; }
		/// <summary>SPページを含むか</summary>
		public bool ForSpPage { get; set; }
		/// <summary>ページ種別</summary>
		public PageTypeEnum PageType { get; set; }
		/// <summary>ページ番号</summary>
		[BindAlias(Constants.REQUEST_KEY_PAGE_NO)]
		public int PagerNo { get; set; }
	}
}