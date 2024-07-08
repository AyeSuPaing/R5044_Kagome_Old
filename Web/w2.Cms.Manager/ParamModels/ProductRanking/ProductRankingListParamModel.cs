/*
=========================================================================================================
  Module      : 商品ランキング一覧パラメタモデル(NewsListParamModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Binder;

namespace w2.Cms.Manager.ParamModels.ProductRanking
{
	/// <summary>
	/// 商品ランキング一覧リストパラメタモデル
	/// </summary>
	[Serializable]
	public class ProductRankingListParamModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ProductRankingListParamModel()
		{
			this.RankingId = string.Empty;
			this.TotalType = string.Empty;
			this.ValidFlg = string.Empty;
			this.SortKbn = "0";
			this.PagerNo = 1;
		}

		/// <summary>商品ランキングID</summary>
		public string RankingId { get; set; }
		/// <summary>集計タイプ</summary>
		public string TotalType { get; set; }
		/// <summary>有効フラグ</summary>
		public string ValidFlg { get; set; }
		/// <summary>並び順</summary>
		public string SortKbn { get; set; }
		/// <summary>ページ番号</summary>
		[BindAlias(Constants.REQUEST_KEY_PAGE_NO)]
		public int PagerNo { get; set; }
	}
}