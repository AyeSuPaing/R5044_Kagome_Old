/*
=========================================================================================================
 Module      : Mainビューモデル(MainViewModel.cs)
･･･････････････････････････････････････････････････････････････････････････････････････････････････････
 Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Linq;
using w2.Cms.Manager.Codes.Sitemap;
using w2.Cms.Manager.ParamModels.Sitemap;

namespace w2.Cms.Manager.ViewModels.Sitemap
{
	/// <summary>
	/// Mainビューモデル
	/// </summary>
	[Serializable]
	public class MainViewModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MainViewModel()
		{
			this.ParamModel = new MainParamModel();
			this.PageItems = Enumerable.Empty<SitemapPageItem>().ToArray();
		}

		/// <summary>パラメータモデル</summary>
		public MainParamModel ParamModel { get; set; }
		/// <summary>検索結果・更新操作内容</summary>
		public SitemapPageItem[] PageItems { get; set; }
		/// <summary>メッセージ</summary>
		public string Message { get; set; }
		/// <summary>ページャHTML</summary>
		public string PagerHtml { get; set; }
	}
}