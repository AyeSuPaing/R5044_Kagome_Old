/*
=========================================================================================================
  Module      : タグマネージャー 検索用パラメタモデル(TagManagerListParamModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Binder;

namespace w2.Cms.Manager.ParamModels.TagManager
{
	/// <summary>
	/// タグマネージャー 検索用パラメタモデル
	/// </summary>
	[Serializable]
	public class TagManagerListParamModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public TagManagerListParamModel()
		{
			this.PagerNo = 1;

			this.AffiliateName = string.Empty;
			this.AffiliateKbn = string.Empty;
			this.Page = string.Empty;
			this.ValidFlg = string.Empty;
			this.AdvCodeMediaType = string.Empty;
			this.SearchAdvertisementCode = string.Empty;
			this.SearchProductId = string.Empty;
		}

		/// <summary>タグ名称</summary>
		public string AffiliateName { get; set; }
		/// <summary>タグ表示区分</summary>
		public string AffiliateKbn { get; set; }
		/// <summary>設置箇所</summary>
		public string Page { get; set; }
		/// <summary>有効フラグ</summary>
		public string ValidFlg { get; set; }
		/// <summary>広告媒体区分ID</summary>
		public string AdvCodeMediaType { get; set; }
		/// <summary>広告コード</summary>
		public string SearchAdvertisementCode { get; set; }
		/// <summary>商品ID</summary>
		public string SearchProductId { get; set; }
		/// <summary>ページ番号</summary>
		[BindAlias(Constants.REQUEST_KEY_PAGE_NO)]
		public int PagerNo { get; set; }
	}
}