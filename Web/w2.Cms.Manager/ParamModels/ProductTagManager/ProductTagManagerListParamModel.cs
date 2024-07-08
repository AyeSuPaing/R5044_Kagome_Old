/*
=========================================================================================================
  Module      : 商品タグマネージャー 検索用パラメタモデル(ProductTagManagerListParamModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Binder;

namespace w2.Cms.Manager.ParamModels.ProductTagManager
{
	/// <summary>
	/// 商品タグマネージャー 検索用パラメタモデル
	/// </summary>
	[Serializable]
	public class ProductTagManagerListParamModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ProductTagManagerListParamModel()
		{
			this.PagerNo = 1;
			this.AffiliateProductTagName = string.Empty;
		}

		/// <summary>商品タグ名称</summary>
		public string AffiliateProductTagName { get; set; }
		/// <summary>ページ番号</summary>
		[BindAlias(Constants.REQUEST_KEY_PAGE_NO)]
		public int PagerNo { get; set; }
	}
}