/*
=========================================================================================================
  Module      : LPリストパラメタモデル(LandingPageListParamModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;

namespace w2.Cms.Manager.ParamModels.LandingPaeg
{
	/// <summary>
	/// LPリストパラメタモデル
	/// </summary>
	[Serializable]
	public class LandingPageListParamModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public LandingPageListParamModel()
		{
			this.SearchWord = "";
			this.PublicDateKbn = "UNSELECTED";
			this.SearchPublicStatus = "";
			this.SearchPublicDesignMode = "";
			this.PagerNo = 1;
		}

		/// <summary>検索ワード</summary>
		public string SearchWord { get; set; }
		/// <summary>公開日区分</summary>
		public string PublicDateKbn { get; set; }
		/// <summary>検索公開状態</summary>
		public string SearchPublicStatus { get; set; }
		/// <summary>デザインモード</summary>
		public string SearchPublicDesignMode { get; set; }
		/// <summary>ページ番号</summary>
		public int PagerNo { get; set; }
	}
}
